/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Plugins
{
    /// <summary>
    /// Class describing a plugin feature which loads a URL to display as an extra tab inside XenCenter; this can be used
    /// to allow access to web management consoles or to add additional user interface features into XenCenter.
    /// </summary>
    internal class TabPageFeature : Feature
    {
        /*
         * Each TabPageFeature has an associated WebBrowser2 instance.  There is just one per feature, and therefore they are shared if a
         * feature appears on multiple XenObjects.  Each time the user switches XenObject, the state of the browser for that particular
         * XenObject is persisted in BrowserStates.
         * 
         * 
         * We have a fairly complicated scheme for managing credentials for webpages:
         * 
         * Normally, when you hit a page that requires authorization, IE will pop up a prompt.  We intercept this prompt, and instead we
         * persist credentials on the XenServer host.  We also prompt if we see HTTP response 401 Authorization Required or 403 Forbidden,
         * on the assumption that the password on the server has changed.
         * 
         * Credentials are persisted server-side as Secrets, with a reference to the appropriate Secret stored in Pool.gui_config (see
         * Pool.*XCPluginSecret).  If the user chooses not to persist credentials, we still persist an empty string in Pool.gui_config,
         * to record the fact that the user has made that choice.
         * 
         * When we see an authentication request, we enter Browser_AuthenticationPrompt on the UI thread.  We check the server for
         * persisted credentials, which has to occur on a background thread, but we can't return from Browser_AuthenticationPrompt until
         * we have the credentials.  To handle this, we spawn a thread in TriggerGetSecret, and then use Application.DoEvents to wait on
         * the UI thread without blocking redraws.  See CompleteGetSecret.  The background thread will set BrowserState.Credentials when
         * it is finished, releasing the UI thread.
         * 
         * If there's nothing in Pool.gui_config for us, or if there is the empty string (indicating that the user has chosen not to
         * persist credentials), then we need to prompt for the credentials from the user.  In that case, it's back onto the UI thread
         * to show a TabPageCredentialsDialog before returning to the background thread to persist the new credentials.
         */

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern long DeleteUrlCacheEntry(string url);

        #region Fields

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const char CREDENTIALS_SEPARATOR = '\x0294';

        /// <summary>
        /// required - "url" attribute, the local or remote url of the HTML page to load
        /// </summary>
        private readonly string Url;
        /// <summary>
        /// optional - "context-menu" attribute, if set the context menu will be disabled
        /// </summary>
        private readonly bool ContextEnabled;
        /// <summary>
        /// optional - "xencenter-only" attribute, if set this tabpage will appear on the XenCenter node and no where else
        /// </summary>
        private readonly bool XenCenterOnly;
        /// <summary>
        /// optional - "relative" attribute, if set the url will be resolved relative to the XenCenter directory
        /// </summary>
        private readonly bool RelativeUrl;
        /// <summary>
        /// optional - "help-link" attribute, if set when the help button is pressed this url will be loaded in a separate browser
        /// </summary>
        private readonly string HelpLink;
        /// <summary>
        /// optional - "credentials" attribute, if set XenCenter will duplicate a session for use by the htmls JavaScript
        /// </summary>
        private readonly bool Credentials;
        /// <summary>
        /// optional - "console" attribute on the "TabPage" tag.
        /// Indicates that this tab-page is a replacement for the console tab.
        /// </summary>
        private readonly bool Console;

        public const string ELEMENT_NAME = "TabPage";
        public const string ATT_XC_ONLY = "xencenter-only";
        public const string ATT_CONTEXT_MENU = "context-menu";
        public const string ATT_HELP_LINK = "help-link";
        public const string ATT_RELATIVE = "relative";
        public const string ATT_CREDENTIALS = "credentials";
        public const string ATT_CONSOLE = "console";
        public const string ATT_URL = "url";

        private readonly Dictionary<IXenObject, BrowserState> BrowserStates = new Dictionary<IXenObject, BrowserState>();
        private BrowserState lastBrowserState;

        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;

        private TabPage _tabPage;
        private WebBrowser2 Browser;

        #endregion

        public TabPageFeature(ResourceManager resourceManager, XmlNode node, PluginDescriptor plugin)
            : base(resourceManager, node, plugin)
        {
            XenCenterOnly = Helpers.GetBoolXmlAttribute(node, ATT_XC_ONLY, false);
            ContextEnabled = Helpers.GetBoolXmlAttribute(node, ATT_CONTEXT_MENU, false);

            // plugins v2
            HelpLink = Helpers.GetStringXmlAttribute(node, ATT_HELP_LINK, "");
            RelativeUrl = Helpers.GetBoolXmlAttribute(node, ATT_RELATIVE, false);
            Credentials = Helpers.GetBoolXmlAttribute(node, ATT_CREDENTIALS, false);

            Console = Helpers.GetBoolXmlAttribute(node, ATT_CONSOLE, false);
            string urlString = Helpers.GetStringXmlAttribute(node, ATT_URL);
            Url = urlString == null ? "" : string.Format("{0}{1}", RelativeUrl ? string.Format("{0}/", Application.StartupPath) : "", urlString);
        }

        #region Properties

        public bool HasHelp
        {
            get { return !string.IsNullOrEmpty(HelpLink); }
        }

        public TabPage TabPage
        {
            get { return _tabPage; }
        }

        public IXenObject SelectedXenObject { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance should replace the console tab.
        /// </summary>
        public bool IsConsoleReplacement
        {
            get
            {
                return Console;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the most recent navigation for the selected xen object
        /// resulted in an error.
        /// </summary>
        public bool IsError
        {
            get
            {
                if (SelectedXenObject != null && BrowserStates.ContainsKey(SelectedXenObject))
                    return BrowserStates[SelectedXenObject].IsError;

                return false;
            }
        }

        #endregion

        public override string CheckForError()
        {
            if (Url == "")
                return string.Format(Messages.CANNOT_PARSE_NODE_PARAM, node.Name, ATT_URL); 

            return base.CheckForError();
        }

        public override void Initialize()
        {
            _tabPage = new TabPage();
            try
            {
                _tabPage.SuspendLayout();
                _tabPage.Text = ToString();
                _tabPage.ToolTipText = Tooltip ?? "";

                if (Program.MainWindow != null) // for unit tests
                    _tabPage.HelpRequested += Program.MainWindow.MainWindow_HelpRequested;

                CreateStatusBar();
                CreateBrowser();
                _tabPage.Controls.Add(statusStrip);
                _tabPage.Controls.Add(Browser);
                Browser.BringToFront();

                _tabPage.Tag = this; // Tag the tab page so we can reload when the tab is focused.
            }
            finally
            {
                _tabPage.ResumeLayout();
            }
        }

        private void CreateStatusBar()
        {
            statusLabel = new ToolStripStatusLabel
            {
                Overflow = ToolStripItemOverflow.Never,
                Spring = true,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            };
            statusStrip = new StatusStrip
            {
                SizingGrip = false,
                AutoSize = false,
                Items = {statusLabel},
                Visible = false
            };
        }

        private void CreateBrowser()
        {
            Browser = new WebBrowser2 {Dock = DockStyle.Fill, IsWebBrowserContextMenuEnabled = ContextEnabled};
            Browser.ProgressChanged += Browser_ProgressChanged;
            Browser.DocumentCompleted += Browser_DocumentCompleted;
            Browser.Navigating += Browser_Navigating;
            Browser.Navigated += Browser_Navigated;
            Browser.NavigateError += Browser_NavigateError;
            Browser.WindowClosed += Browser_WindowClosed;
            Browser.PreviewKeyDown += Browser_PreviewKeyDown;
            Browser.AuthenticationPrompt += Browser_AuthenticationPrompt;

            // Navigate to about:blank to work around http://support.microsoft.com/kb/320153.
            Browser.Navigate("about:blank");
        }
        
        public bool ShowTab
        {
            get
            {
                try
                {
                    if (_tabPage == null)
                        return false;

                    if (XenCenterOnly)
                        return SelectedXenObject == null;

                    if (SelectedXenObject == null || !Enabled || !Placeholders.UriValid(Url, SelectedXenObject))
                        return false;

                    return Search == null || Search.Query.Match(SelectedXenObject);
                }
                catch (UriFormatException e)
                {
                    log.Debug(string.Format("Not displaying tab '{0}' for plugin '{1}'. Invalid properties in url '{2}'", ToString(), PluginDescriptor.Name, Url), e);
                    return false;
                }
            }
        }

        public void SetUrl()
        {
            // Never update XenCenter node tabs once loaded
            if (Browser.Url != null && Browser.Url.AbsoluteUri != "about:blank" && XenCenterOnly)
                return;

            BrowserState state;
            if (SelectedXenObject == null)
            {
                // XenCenter node is selected, the placeholder code will sub in "null" for all placeholders
                // After this point we will never update this url again for this node, so there is no need to store a browser state
                state = new BrowserState(Placeholders.SubstituteUri(Url, SelectedXenObject), SelectedXenObject, Browser);
            }
            else if (BrowserStates.ContainsKey(SelectedXenObject))
            {
                state = BrowserStates[SelectedXenObject];
                state.Uris = Placeholders.SubstituteUri(Url, SelectedXenObject);
            }
            else
            {
                state = new BrowserState(Placeholders.SubstituteUri(Url, SelectedXenObject), SelectedXenObject, Browser);
                BrowserStates[SelectedXenObject] = state;
            }

            try
            {
                if (state.ObjectForScripting != null)
                {
                    if (Credentials)
                        state.ObjectForScripting.LoginSession();
                    Browser.ObjectForScripting = state.ObjectForScripting;
                }

                lastBrowserState = state;

                Browser.DocumentText = string.Empty;
                Application.DoEvents();

                lastBrowserState.IsError = false;
                ShowStatus(string.Format(Messages.WEB_BROWSER_WAITING, ShortUri(state.Uris[0])));
                Browser.Navigate(state.Uris[0]);
            }
            catch (Exception e)
            {
                log.Error(string.Format("Failed to set TabPage url to '{0}' for plugin '{1}'",
                    string.Join(",", state.Uris.ConvertAll(u => u.ToString()).ToArray()), PluginDescriptor.Name), e);
            }
        }

        private void ShowStatus(string text)
        {
            statusLabel.Text = text;
            statusStrip.Visible = true;
        }

        private void HideStatus()
        {
            statusStrip.Visible = false;
        }

        private string ShortUri(Uri uri)
        {
            return uri.ToString().Ellipsise(80);
        }

        #region WebBrowser2 event handlers

        private void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            Program.AssertOnEventThread();

            if (XenCenterOnly || lastBrowserState == null)
                return;

            log.DebugFormat("Navigating to {0} for {1}", e.Url, Helpers.GetName(lastBrowserState.Obj));
            DeleteUrlCacheEntry(e.Url.AbsoluteUri);
            if (Console)
            {
                // delete this page from the cache.... that we can be sure that if the page stops working then
                // the user reliably gets the real console back.
                DeleteUrlCacheEntry(e.Url.AbsoluteUri);
            }
        }

        private void Browser_NavigateError(object sender, WebBrowser2.NavigateErrorEventArgs e)
        {
            Program.AssertOnEventThread();
            lastBrowserState.IsError = true;
            log.DebugFormat("Got error {0} while navigating to {1}.", e.StatusCode, e.Url);

            try
            {
                if (e.StatusCode == 401 || e.StatusCode == 403)
                {
                    log.Debug("Clearing secret and re-prompting.");
                    CompleteClearSecret(lastBrowserState);
                    TriggerGetSecret(lastBrowserState);
                }
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
            }
        }

        private void Browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (XenCenterOnly || lastBrowserState == null)
            {
                HideStatus();
                return;
            }
            
            log.DebugFormat("Navigated to {0} for {1}", e.Url, Helpers.GetName(lastBrowserState.Obj));

            if (lastBrowserState.IsError && e.Url != null && e.Url.AbsoluteUri != "about:blank")
            {
                if (Console)
                {
                    // if this plugin tab-page is a console replacement and the error-state of the navigation has changed
                    // then update the tabs. This ensures that user gets the real console tab back.
                    Program.MainWindow.UpdateToolbars();
                }

                lastBrowserState.Uris.Remove(e.Url);

                if (lastBrowserState.Uris.Count > 0)
                {
                    lastBrowserState.IsError = false;
                    ShowStatus(string.Format(Messages.WEB_BROWSER_FAILED_RETRYING, ShortUri(e.Url),
                        ShortUri(lastBrowserState.Uris[0])));
                    Browser.Navigate(lastBrowserState.Uris[0]);
                }
                else
                {
                    ShowStatus(string.Format(Messages.WEB_BROWSER_FAILED, ShortUri(e.Url)));
                    Browser.Navigate("about:blank");
                }
            }
        }

        private void Browser_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            Program.AssertOnEventThread();
            if (XenCenterOnly || lastBrowserState == null || lastBrowserState.Uris.Count == 0 ||
                Browser.ReadyState == WebBrowserReadyState.Complete)
                return;

            if (e.MaximumProgress > 0 && e.CurrentProgress >= 0 && e.CurrentProgress < e.MaximumProgress)
            {
                int progr = Convert.ToInt32(e.CurrentProgress * 100L / e.MaximumProgress);
                if (progr > 100)
                    progr = 100;

                ShowStatus(string.Format(Messages.WEB_BROWSER_LOADING_PERCENT, ShortUri(lastBrowserState.Uris[0]), progr));
            }
            else
            {
                ShowStatus(string.Format(Messages.WEB_BROWSER_LOADING, ShortUri(lastBrowserState.Uris[0])));
            }
        }

        private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Program.AssertOnEventThread();
            
            if (XenCenterOnly || lastBrowserState == null || lastBrowserState.IsError)
                return;
            
            HideStatus();
        }

        private void Browser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.F1 && Browser.Focused)
            {
                Program.MainWindow.MainWindow_HelpRequested(null, null);
            }
        }

        private void Browser_AuthenticationPrompt(WebBrowser2 sender, WebBrowser2.AuthenticationPromptEventArgs e)
        {
            try
            {
                Program.AssertOnEventThread();

                log.Debug("Prompting for authentication...");

                CompleteClearSecret(lastBrowserState);
                CompleteGetSecret(lastBrowserState);
                BrowserState.BrowserCredentials creds = lastBrowserState.Credentials;
                if (creds != null && creds.Valid)
                {
                    e.Username = creds.Username;
                    e.Password = creds.Password;
                    e.Success = true;

                    log.Debug("Prompt for authentication successful.");
                }
                else
                {
                    e.Success = false;
                    log.Debug("Prompt for authentication cancelled / failed.");
                }
            }
            catch (Exception exn)
            {
                log.Error("Prompt for authentication failed", exn);
                e.Success = false;
            }
        }

        private void Browser_WindowClosed(object sender, EventArgs e)
        {
            if (!Program.Exiting && Enabled && !_tabPage.Disposing)
            {
                _tabPage.Controls.Remove(Browser);
                CreateBrowser();
                _tabPage.Controls.Add(Browser);
                if (SelectedXenObject != null)
                {
                    BrowserStates.Remove(SelectedXenObject);
                    SetUrl();
                }
            }
        }

        #endregion

        private void CompleteGetSecret(BrowserState state)
        {
            if (state.Credentials != null)
                return;

            TriggerGetSecret(state);
            while (state.Credentials == null)
                Application.DoEvents();
        }

        /// <summary>
        /// Load the secret where we've saved credentials.  This will be placed on a background thread, and we'll poll for
        /// its results.
        /// </summary>
        private void TriggerGetSecret(BrowserState state)
        {
            Thread t = new Thread(GetSecret);
            t.IsBackground = true;
            t.Start(state);
        }

        /// <summary>
        /// Get the persisted secret from the server, or prompt for new credentials and persist those back to the server.
        /// Completion of this thread is indicated by state.Credentials being set.
        /// </summary>
        /// <param name="obj"></param>
        private void GetSecret(object obj)
        {
            Program.AssertOffEventThread();

            BrowserState state = (BrowserState)obj;

            try
            {
                Session session = state.Obj.Connection.Session;

                do
                {
                    Pool pool = Helpers.GetPoolOfOne(state.Obj.Connection);
                    if (pool == null)
                    {
                        log.Warn("Failed to get Pool!");
                        Thread.Sleep(5000);
                        continue;
                    }

                    string secret_uuid = pool.GetXCPluginSecret(PluginDescriptor.Name, state.Obj);
                    if (string.IsNullOrEmpty(secret_uuid))
                    {
                        var msg = secret_uuid == null ? "Nothing persisted." : "User chose not to persist these credentials.";
                        log.Debug(msg + " Prompting for new credentials.");

                        Program.Invoke(Program.MainWindow, () => { state.Credentials = PromptForUsernamePassword(secret_uuid == null); });
                        MaybePersistCredentials(session, pool, state.Obj, state.Credentials);
                        return;
                    }
                    else
                    {
                        log.Debug("Found a secret.");
                        XenRef<Secret> secret = null;
                        try
                        {
                            secret = Secret.get_by_uuid(session, secret_uuid);
                        }
                        catch (Failure exn)
                        {
                            log.Warn(string.Format("Secret {0} for {1} on plugin {2} has disappeared! Removing from pool.gui_config.",
                                    secret_uuid, Helpers.GetName(state.Obj), PluginDescriptor.Name), exn);
                            TryToRemoveSecret(pool, session, PluginDescriptor.Name, state.Obj);
                            continue;
                        }

                        string val = Secret.get_value(session, secret);
                        string[] bits = val.Split(CREDENTIALS_SEPARATOR);
                        if (bits.Length != 2)
                        {
                            log.WarnFormat("Corrupt secret {0} at {1} for {2} on plugin {3}!  Deleting.", val, secret_uuid,
                                Helpers.GetName(state.Obj), PluginDescriptor.Name);

                            TryToDestroySecret(session, secret.opaque_ref);
                            TryToRemoveSecret(pool, session, PluginDescriptor.Name, state.Obj);
                            continue;
                        }

                        log.Debug("Secret successfully read.");

                        state.Credentials = new BrowserState.BrowserCredentials
                        {
                            Username = bits[0],
                            Password = bits[1],
                            PersistCredentials = true,
                            Valid = true
                        };
                       return;
                    }

                    // Unreachable.  Should either have returned, or continued (to retry).

                } while (true);
            }
            catch (Exception exn)
            {
                log.Warn("Ignoring exception when trying to get secret", exn);

                // Note that it's essential that we set state.Credentials before leaving this function,
                // because other threads are waiting for that value to appear.
                state.Credentials = new BrowserState.BrowserCredentials {Valid = false};
            }
        }

        private void MaybePersistCredentials(Session session, Pool pool, IXenObject obj, BrowserState.BrowserCredentials creds)
        {
            if (creds != null && creds.Valid)
            {
                string secretUuid = creds.PersistCredentials ? CreateSecret(session, creds.Username, creds.Password) : "";
                pool.SetXCPluginSecret(session, PluginDescriptor.Name, obj, secretUuid);
            }
        }

        private string CreateSecret(Session session, string username, string password)
        {
            string val = string.Format("{0}{1}{2}", username, CREDENTIALS_SEPARATOR, password);
            return Secret.CreateSecret(session, val);
        }

        private BrowserState.BrowserCredentials PromptForUsernamePassword(bool persistCredentials)
        {
            Program.AssertOnEventThread();

            using (var d = new TabPageCredentialsDialog {ServiceName = Label, PersistCredentials = persistCredentials})
            {
                if (d.ShowDialog(Program.MainWindow) == DialogResult.OK)
                {
                    return new BrowserState.BrowserCredentials
                    {
                        Username = d.Username,
                        Password = d.Password,
                        PersistCredentials = d.PersistCredentials,
                        Valid = true
                    };
                }
                return new BrowserState.BrowserCredentials {Valid = false};
            }
        }

        private void CompleteClearSecret(BrowserState state)
        {
            if (state.Credentials == null)
                return;

            TriggerClearSecret(state);
            while (state.Credentials != null)
                Application.DoEvents();
        }

        private void TriggerClearSecret(BrowserState state)
        {
            Thread t = new Thread(ClearSecret);
            t.IsBackground = true;
            t.Start(state);
        }

        /// <summary>
        /// Clear the persisted secret from the server.
        /// Completion of this thread is indicated by state.Credentials being set to null.
        /// </summary>
        private void ClearSecret(object obj)
        {
            Program.AssertOffEventThread();

            BrowserState state = (BrowserState)obj;

            try
            {
                Session session = state.Obj.Connection.Session;
                Pool pool = Helpers.GetPoolOfOne(state.Obj.Connection);
                if (pool == null)
                    return;

                string secret_uuid = pool.GetXCPluginSecret(PluginDescriptor.Name, state.Obj);
                if (!string.IsNullOrEmpty(secret_uuid))
                {
                    XenRef<Secret> secret_ref = Secret.get_by_uuid(session, secret_uuid);
                    TryToDestroySecret(session, secret_ref.opaque_ref);
                    TryToRemoveSecret(pool, session, PluginDescriptor.Name, state.Obj);
                }
            }
            catch (Exception exn)
            {
                log.Warn("Ignoring exception when trying to clear secret", exn);
            }

            state.Credentials = null;
        }

        private static void TryToDestroySecret(Session session, string opaque_ref)
        {
            try
            {
                Secret.destroy(session, opaque_ref);
                log.DebugFormat("Successfully removed secret {0}", opaque_ref);
            }
            catch (Exception exn)
            {
                log.Error(string.Format("Failed to destroy secret {0}", opaque_ref), exn);
            }
        }

        private static void TryToRemoveSecret(Pool pool, Session session, string name, IXenObject obj)
        {
            try
            {
                pool.RemoveXCPluginSecret(session, name, obj);
                log.DebugFormat("Successfully removed secret for {0}/{1}", name, Helpers.GetName(obj));
            }
            catch (Exception exn)
            {
                log.Error(string.Format("Failed to remove secret for {0}/{1}", name, obj), exn);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            BrowserStates.Clear();

            if (Browser != null)
            {
                Browser.Dispose();
                Browser = null;
            }
            if (_tabPage != null)
            {
                _tabPage.Dispose();
                _tabPage = null;
            }
        }

        public void DisposeURL(IXenObject xmo)
        {
            if (BrowserStates.ContainsKey(xmo))
            {
                BrowserStates.Remove(xmo);
            }
        }

        public void LaunchHelp()
        {
            if (HasHelp)
            {
                Program.OpenURL(HelpLink);
            }
        }


        private class BrowserState
        {
            /// <summary>
            /// May be null, if this is the XenCenter node.
            /// </summary>
            public readonly ScriptingObject ObjectForScripting;
            public List<Uri> Uris;

            /// <summary>
            /// May be null, if this is the XenCenter node.
            /// </summary>
            public IXenObject Obj;

            /// <summary>
            /// May be null.
            /// </summary>
            public BrowserCredentials Credentials = null;

            /// <summary>
            /// Indicates that the browser is currently in an error state i.e. the page couldn't be loaded. This is used when the console is being replaced
            /// by this tab-page so that the user can get the real console back.
            /// </summary>
            public bool IsError;

            public BrowserState(List<Uri> uris, IXenObject obj, WebBrowser2 browser)
            {
                Uris = new List<Uri>(uris);
                Obj = obj;
                ObjectForScripting = obj == null ? null : new ScriptingObject(browser, obj);
            }

            public class BrowserCredentials
            {
                /// <summary>
                /// True if the other fields here are valid (i.e. the user pressed OK, not Cancel, when prompted for credentials).
                /// </summary>
                internal bool Valid;
                internal string Username;
                internal string Password;
                internal bool PersistCredentials;
            }
        }
    }

    [ComVisible(true)]
    public class ScriptingObject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public const string JAVASCRIPT_CALLBACK_METHOD = "XenCenterXML";

        [Obsolete("Use SessionOpaqueRef instead.")]
        public string SessionUuid;
        public string SessionOpaqueRef;
        public string SessionUrl;
        private IXenConnection connection;
        public WebBrowser2 browser;
        public string SelectedObjectType;
        public string SelectedObjectRef;

        public ScriptingObject(WebBrowser2 Browser, IXenObject XenObject)
        {
            browser = Browser;
            connection = XenObject.Connection;
            if (connection != null)
            {
                connection.ConnectionResult += connection_ConnectionResult;
            }
            SetObject(XenObject);
        }

        public void SetObject(IXenObject XenObject)
        {
            // Annoyingly all of our classes start with an uppercase character, whereas the servers only uppercase abbreviations
            var abbreviations = new List<string> {"SR", "VDI", "VBD", "VM", "PIF", "VIF", "PBD"};
            SelectedObjectType = XenObject.GetType().Name;
            if (abbreviations.Find(s => SelectedObjectType.StartsWith(s)) == null)
            {
                string firstLetter = SelectedObjectType.Substring(0, 1);
                SelectedObjectType = firstLetter.ToLowerInvariant() + SelectedObjectType.Substring(1, SelectedObjectType.Length - 1);
            }
            SelectedObjectRef = XenObject.opaque_ref;
        }

        private void connection_ConnectionResult(object sender, ConnectionResultEventArgs e)
        {
            if (connection.IsConnected)
            {
                // If the connection has been reconnected then we need to update the session ref, as the old one is invalid
                LoginSession();
                Program.Invoke(Program.MainWindow, delegate
                {
                    if (!browser.IsDisposed && browser.Document != null)
                        browser.Document.InvokeScript("RefreshPage");
                    else
                        log.Debug("Tried to access disposed webbrowser, ignoring refresh.");
                });
            }
        }

        public void LoginSession()
        {
            if (connection != null)
            {
                SessionUrl = connection.Session.Url;
                SessionOpaqueRef = connection.DuplicateSession().opaque_ref;
#pragma warning disable 612, 618
                SessionUuid = SessionOpaqueRef;
#pragma warning restore 612, 618
            }
        }

        /// <summary>
        /// This is the method called by any javascript logic in the webpage to make an api call. 
        /// We take the data and post it straight through to xapi on a background thread to allow 
        /// the calling method in the JS to exit and the page to continue drawing.
        /// </summary>
        /// <param name="jsCallback">Method name we will call in the JS to return the server response</param>
        /// <param name="data"></param>
        public void WriteXML(string jsCallback, string data)
        {
            Thread t = new Thread(writeXML);
            t.IsBackground = true;
            t.Start(new string[] { jsCallback, data });
        }

        private void writeXML(object context)
        {
            try
            {
                // We post to the json url so that xapi returns the information in a way that is easy to consume by the JS
                string[] jsCallbackAndData = context as string[];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/json", SessionUrl));
                request.Method = "POST";
                request.ContentType = "xml";
                request.ContentLength = Encoding.UTF8.GetBytes(jsCallbackAndData[1]).Length;
                request.UserAgent = Branding.BRAND_CONSOLE + "\\Plugin";
                request.Proxy = XenAdminConfigManager.Provider.GetProxyFromSettings(connection, true);

                using (StreamWriter xmlstream = new StreamWriter(request.GetRequestStream()))
                {
                    xmlstream.Write(jsCallbackAndData[1]);
                }
                WebResponse response = request.GetResponse();
                StringBuilder outputBuilder = new StringBuilder();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    while (reader.Peek() != -1)
                    {
                        outputBuilder.Append(reader.ReadLine());
                    }
                }
                // The xmlrpc bit of jquery expects a function object, which turns up here as a string that you could eval to get the function
                // We just want the name, so we filter it out.
                Regex functionNameReg = new Regex("^function(.*)\\(");
                string funcName = "";
                Match m = functionNameReg.Match(jsCallbackAndData[0]);
                if (m.Success)
                {
                    funcName = m.Groups[1].Value.Trim();
                }
                Program.Invoke(Program.MainWindow, delegate
                {
                    if (browser.IsDisposed || browser.Disposing)
                    {
                        log.DebugFormat("Browser has been disposed, cannot return message to plugin: {0}", outputBuilder);
                    }
                    else if (browser.ObjectForScripting != this)
                    {
                        // If you don't do this, you can get old data re-entering the javascript execution after you have switched to a new object
                        log.DebugFormat("Scripting object has been changed, discarding message to plugin: {0}", outputBuilder);
                    }
                    else if (browser.Document != null)
                    {
                        browser.Document.InvokeScript(JAVASCRIPT_CALLBACK_METHOD, new object[] { funcName, outputBuilder.ToString() });
                    }
                });
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            //TODO: What's the sensible way to let the JS know that there has been an error? Invoke a method with the info? How does this work with regards to message callback?
        }
    }
}
