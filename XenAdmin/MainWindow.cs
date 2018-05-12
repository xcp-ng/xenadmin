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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using XenAdmin.Actions.GUIActions;
using XenAdmin.Controls.MainWindowControls;
using XenAdmin.Wizards.ImportWizard;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Alerts;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAdmin.TabPages;
using XenAdmin.XenSearch;
using XenAdmin.Wizards.PatchingWizard;
using XenAdmin.Plugins;

using System.Linq;

namespace XenAdmin
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisibleAttribute(true)]
    public partial class MainWindow : Form, ISynchronizeInvoke, IMainWindow
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// A mapping between objects in the tree and the associated selected tab.
        /// </summary>
        private Dictionary<object, TabPage> selectedTabs = new Dictionary<object, TabPage>();

        /// <summary>
        /// The selected tab for the overview node.
        /// </summary>
        private TabPage selectedOverviewTab = null;

        internal readonly PerformancePage PerformancePage = new PerformancePage();
        internal readonly GeneralTabPage GeneralPage = new GeneralTabPage();
        internal readonly BallooningPage BallooningPage = new BallooningPage();
        internal readonly BallooningUpsellPage BallooningUpsellPage = new BallooningUpsellPage();
        internal readonly ConsolePanel ConsolePanel = new ConsolePanel();
        internal readonly CvmConsolePanel CvmConsolePanel = new CvmConsolePanel();
        internal readonly HAPage HAPage = new HAPage();
        internal readonly HAUpsellPage HAUpsellPage = new HAUpsellPage();
        internal readonly HomePage HomePage = new HomePage();
        internal readonly SearchPage SearchPage = new SearchPage();
        internal readonly NetworkPage NetworkPage = new NetworkPage();
        internal readonly NICPage NICPage = new NICPage();
        internal readonly WlbPage WlbPage = new WlbPage();
        internal readonly WLBUpsellPage WLBUpsellPage = new WLBUpsellPage();
        internal readonly SrStoragePage SrStoragePage = new SrStoragePage();
        internal readonly PhysicalStoragePage PhysicalStoragePage = new PhysicalStoragePage();
        internal readonly VMStoragePage VMStoragePage = new VMStoragePage();
        internal readonly AdPage AdPage = new AdPage();
        internal readonly ADUpsellPage AdUpsellPage = new ADUpsellPage();
        internal readonly GpuPage GpuPage = new GpuPage();
        internal readonly PvsPage PvsPage = new PvsPage();
        internal readonly DockerProcessPage DockerProcessPage = new DockerProcessPage();
        internal readonly DockerDetailsPage DockerDetailsPage = new DockerDetailsPage();
        internal readonly UsbPage UsbPage = new UsbPage();

        private ActionBase statusBarAction = null;
        public ActionBase StatusBarAction { get { return statusBarAction; } }
      
        private bool IgnoreTabChanges = false;
        private bool ToolbarsEnabled;

        /// <summary>
        /// Helper boolean to only trigger Resize_End when window is really resized by dragging edges
        /// Without this Resize_End is triggered even when window is moved around and not resized
        /// </summary>
        private bool mainWindowResized = false;

        private readonly Dictionary<IXenConnection, IList<Form>> activePoolWizards = new Dictionary<IXenConnection, IList<Form>>();
        private readonly Dictionary<IXenObject, Form> activeXenModelObjectWizards = new Dictionary<IXenObject, Form>();

        /// <summary>
        /// The arguments passed in on the command line.
        /// </summary>
        private string[] CommandLineParam = null;
        private ArgType CommandLineArgType = ArgType.None;

        private static readonly System.Windows.Forms.Timer CheckForUpdatesTimer = new System.Windows.Forms.Timer();

        private readonly PluginManager pluginManager;
        private readonly ContextMenuBuilder contextMenuBuilder;

        private readonly LicenseManagerLauncher licenseManagerLauncher;
        private readonly LicenseTimer licenseTimer;

        public readonly HealthCheckOverviewLauncher HealthCheckOverviewLauncher;
        private readonly System.Windows.Forms.Timer healthCheckResultTimer = new System.Windows.Forms.Timer();

        private Dictionary<ToolStripMenuItem, int> pluginMenuItemStartIndexes = new Dictionary<ToolStripMenuItem, int>();

        private bool expandTreeNodesOnStartup;
        private int connectionsInProgressOnStartup;

        public MainWindow(ArgType argType, string[] args)
        {
            Program.MainWindow = this;
            licenseManagerLauncher = new LicenseManagerLauncher(Program.MainWindow);
            HealthCheckOverviewLauncher = new HealthCheckOverviewLauncher(Program.MainWindow);
            InvokeHelper.Initialize(this);

            InitializeComponent();
            SetMenuItemStartIndexes();
            Icon = Properties.Resources.AppIcon;

            #region Add Tab pages

            components.Add(NICPage);
            components.Add(VMStoragePage);
            components.Add(SrStoragePage);
            components.Add(PerformancePage);
            components.Add(GeneralPage);
            components.Add(BallooningPage);
            components.Add(ConsolePanel);
            components.Add(CvmConsolePanel);
            components.Add(NetworkPage);
            components.Add(HAPage);
            components.Add(HomePage);
            components.Add(WlbPage);
            components.Add(AdPage);
            components.Add(GpuPage);
            components.Add(PvsPage);
            components.Add(SearchPage);
            components.Add(DockerProcessPage);
            components.Add(DockerDetailsPage);
            components.Add(UsbPage);

            AddTabContents(VMStoragePage, TabPageStorage);
            AddTabContents(SrStoragePage, TabPageSR);
            AddTabContents(NICPage, TabPageNICs);
            AddTabContents(PerformancePage, TabPagePeformance);
            AddTabContents(GeneralPage, TabPageGeneral);
            AddTabContents(BallooningPage, TabPageBallooning);
            AddTabContents(BallooningUpsellPage, TabPageBallooningUpsell);
            AddTabContents(ConsolePanel, TabPageConsole);
            AddTabContents(CvmConsolePanel, TabPageCvmConsole);
            AddTabContents(NetworkPage, TabPageNetwork);
            AddTabContents(HAPage, TabPageHA);
            AddTabContents(HAUpsellPage, TabPageHAUpsell);
            AddTabContents(HomePage, TabPageHome);
            AddTabContents(WlbPage, TabPageWLB);
            AddTabContents(WLBUpsellPage, TabPageWLBUpsell);
            AddTabContents(PhysicalStoragePage, TabPagePhysicalStorage);
            AddTabContents(AdPage, TabPageAD);
            AddTabContents(AdUpsellPage, TabPageADUpsell);
            AddTabContents(GpuPage, TabPageGPU);
            AddTabContents(PvsPage, TabPagePvs);
            AddTabContents(SearchPage, TabPageSearch);
            AddTabContents(DockerProcessPage, TabPageDockerProcess);
            AddTabContents(DockerDetailsPage, TabPageDockerDetails);
            AddTabContents(UsbPage, TabPageUSB);

            #endregion

            TheTabControl.SelectedIndexChanged += TheTabControl_SelectedIndexChanged;
            TheTabControl.Deselected += TheTabControl_Deselected;
            navigationPane.DragDropCommandActivated += navigationPane_DragDropCommandActivated;

            PoolCollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged<Pool>);
            MessageCollectionChangedWithInvoke = Program.ProgramInvokeHandler(MessageCollectionChanged);
            HostCollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged<Host>);
            VMCollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged<VM>);
            SRCollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged<SR>);
            FolderCollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged<Folder>);
            TaskCollectionChangedWithInvoke = Program.ProgramInvokeHandler(MeddlingActionManager.TaskCollectionChanged);
            ConnectionsManager.History.CollectionChanged += History_CollectionChanged;

            CommandLineArgType = argType;
            CommandLineParam = args;

            pluginManager = new PluginManager();
            pluginManager.PluginsChanged += pluginManager_PluginsChanged;
            pluginManager.LoadPlugins();
            contextMenuBuilder = new ContextMenuBuilder(pluginManager, this);
            ((WinformsXenAdminConfigProvider) XenAdminConfigManager.Provider).PluginManager = pluginManager;

            eventsPage.GoToXenObjectRequested += eventsPage_GoToXenObjectRequested;
            SearchPage.SearchChanged += SearchPanel_SearchChanged;
            Alert.RegisterAlertCollectionChanged(XenCenterAlerts_CollectionChanged);
            Updates.RegisterCollectionChanged(Updates_CollectionChanged);

            FormFontFixer.Fix(this);

            Folders.InitFolders();
            DockerContainers.InitDockerContainers();
            OtherConfigAndTagsWatcher.InitEventHandlers();

            // Fix colour of text on gradient panels
            TitleLabel.ForeColor = Program.TitleBarForeColor;
            loggedInLabel1.SetTextColor(Program.TitleBarForeColor);

            statusProgressBar.Visible = false;

            SelectionManager.BindTo(MainMenuBar.Items, this);
            SelectionManager.BindTo(ToolStrip.Items, this);
            Properties.Settings.Default.SettingChanging += Default_SettingChanging;

            licenseTimer = new LicenseTimer(licenseManagerLauncher);
            GeneralPage.LicenseLauncher = licenseManagerLauncher;

            toolStripSeparator7.Visible = xenSourceOnTheWebToolStripMenuItem.Visible = xenCenterPluginsOnlineToolStripMenuItem.Visible = !HiddenFeatures.ToolStripMenuItemHidden;
            healthCheckToolStripMenuItem1.Visible = !HiddenFeatures.HealthCheckHidden;
        }

        private void Default_SettingChanging(object sender, SettingChangingEventArgs e)
        {
			if (e == null)
				return;

            if (e.SettingName == "AutoSwitchToRDP" || e.SettingName == "EnableRDPPolling")
            {
                ConsolePanel.ResetAllViews();

				if (SelectionManager.Selection.FirstIsRealVM)
					ConsolePanel.setCurrentSource((VM)SelectionManager.Selection.First);
                else if (SelectionManager.Selection.FirstIs<Host>())
                    ConsolePanel.setCurrentSource((Host)SelectionManager.Selection.First);

                UnpauseVNC(sender == TheTabControl);
            }
        }

        private void SetMenuItemStartIndexes()
        {
            foreach (ToolStripMenuItem menu in MainMenuBar.Items)
            {
                foreach (ToolStripItem item in menu.DropDownItems)
                {
                    if (item != null && item.Text == "PluginItemsPlaceHolder")
                    {
                        pluginMenuItemStartIndexes.Add(menu, menu.DropDownItems.IndexOf(item));
                        menu.DropDownItems.Remove(item);
                        break;
                    }
                }
            }
        }

        internal SelectionBroadcaster SelectionManager
        {
            get
            {
                return navigationPane.SelectionManager;
            }
        }

        internal ContextMenuBuilder ContextMenuBuilder
        {
            get
            {
                return contextMenuBuilder;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Program.AssertOnEventThread();

            History.EnableHistoryButtons();
            History.NewHistoryItem(new XenModelObjectHistoryItem(null, TabPageHome));

            /*
             * Resume window size and location
             */
            try
            {
                // Bring in previous version user setting for the first time.  
                if (Properties.Settings.Default.DoUpgrade)
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.DoUpgrade = false;
                    XenAdmin.Settings.TrySaveSettings();
                }

                Point savedLocation = Properties.Settings.Default.WindowLocation;
                Size savedSize = Properties.Settings.Default.WindowSize;

                if (HelpersGUI.WindowIsOnScreen(savedLocation, savedSize))
                {
                    this.Location = savedLocation;
                    this.Size = savedSize;
                }
            }
            catch
            {
            }

            // Using the Load event ensures that the handle has been 
            // created:
            base.OnLoad(e);

            NewTabs[0] = TabPageHome;
            NewTabCount = 1;
            ChangeToNewTabs();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            TheTabControl.Visible = true;
            alertPage.Visible = updatesPage.Visible = eventsPage.Visible = false;
            navigationPane.FocusTreeView();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Clip.RegisterClipboardViewer();
        }

        protected override void WndProc(ref System.Windows.Forms.Message e)
        {
            //System.Console.WriteLine(Win32.GetWindowsMessageName(e.Msg));
            switch (e.Msg)
            {
                case Win32.WM_CHANGECBCHAIN: // Clipboard chain has changed.
                    Clip.ProcessWMChangeCBChain(e);
                    break;

                case Win32.WM_DRAWCLIPBOARD: // Content of clipboard has changed.
                    Clip.ProcessWMDrawClipboard(e);
                    break;

                case Win32.WM_DESTROY:
                    Clip.UnregisterClipboardViewer();
                    base.WndProc(ref e);
                    break;

                default:
                    base.WndProc(ref e);
                    break;
            }
        }

        private void AddTabContents(Control contents, TabPage TabPage)
        {
            contents.Location = new Point(0, 0);
            contents.Size = TabPage.Size;
            contents.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Right;
            TabPage.Controls.Add(contents);
        }

        void History_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (Program.Exiting)
                return;

            Program.BeginInvoke(Program.MainWindow, () =>
            {
                ActionBase action = e.Element as ActionBase;
                
                switch (e.Action)
                {
                    case CollectionChangeAction.Add:
                        {
                            if (action == null)
                                return;

                            var meddlingAction = action as MeddlingAction;
                            if (meddlingAction == null)
                            {
                                SetStatusBar(null, null);
                                if (statusBarAction != null)
                                {
                                    statusBarAction.Changed -= actionChanged;
                                    statusBarAction.Completed -= actionCompleted;
                                }
                                statusBarAction = action;
                            }
                            action.Changed += actionChanged;
                            action.Completed += actionCompleted;
                            actionChanged(action);
                            break;
                        }
                    case CollectionChangeAction.Remove:
                        {
                            if (action != null)
                            {
                                action.Changed -= actionChanged;
                                action.Completed -= actionCompleted;
                            }
                            else
                            {
                                var range = e.Element as List<ActionBase>;
                                if (range != null)
                                {
                                    foreach (var a in range)
                                    {
                                        a.Changed -= actionChanged;
                                        a.Completed -= actionCompleted;
                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }

                            int errors = ConnectionsManager.History.Count(a => a.IsCompleted && !a.Succeeded);
                            navigationPane.UpdateNotificationsButton(NotificationsSubMode.Events, errors);

                            if (eventsPage.Visible)
                            {
                                TitleLabel.Text = NotificationsSubModeItem.GetText(NotificationsSubMode.Events, errors);
                                TitleIcon.Image = NotificationsSubModeItem.GetImage(NotificationsSubMode.Events, errors);
                            }
                            break;
                        }
                }
            });
        }

        void actionCompleted(ActionBase action)
        {
            actionChanged(action);
            if (action is SrAction)
                Program.Invoke(this, UpdateToolbars);
        }

        void actionChanged(ActionBase action)
        {
            if (Program.Exiting)
                return;

            Program.Invoke(this, () => actionChanged_(action));
        }

        void actionChanged_(ActionBase action)
        {
            if (action.SuppressProgressReport) // suppress updates when the PureAsyncAction runs the action to populate the ApiMethodsToRoleCheck
                return;
             var meddlingAction = action as MeddlingAction;
             if (meddlingAction == null)
                 statusProgressBar.Visible = action.ShowProgress && !action.IsCompleted;

            // Be defensive against CA-8517.
            if (action.PercentComplete < 0 || action.PercentComplete > 100)
            {
                log.ErrorFormat("PercentComplete is erroneously {0}", action.PercentComplete);
            }
            else if (meddlingAction == null)
            {
                statusProgressBar.Value = action.PercentComplete;
            }

            // Don't show cancelled exception
            if (action.Exception != null && !(action.Exception is CancelledException))
            {
                if (meddlingAction == null)
                    SetStatusBar(Properties.Resources._000_error_h32bit_16, action.Exception.Message);
            }
            else if (meddlingAction == null)
            {
                SetStatusBar(null, action.IsCompleted
                                       ? null
                                       : !string.IsNullOrEmpty(action.Description)
                                             ? action.Description
                                             : !string.IsNullOrEmpty(action.Title)
                                                   ? action.Title
                                                   : null);
            }

            int errors = ConnectionsManager.History.Count(a => a.IsCompleted && !a.Succeeded && !((a is CancellingAction) && ((CancellingAction)a).Cancelled));
            navigationPane.UpdateNotificationsButton(NotificationsSubMode.Events, errors);

            if (eventsPage.Visible)
            {
                TitleLabel.Text = NotificationsSubModeItem.GetText(NotificationsSubMode.Events, errors);
                TitleIcon.Image = NotificationsSubModeItem.GetImage(NotificationsSubMode.Events, errors);
            }
        }

        public void SetStatusBar(Image image, string message)
        {
            if ((statusLabel.Text != null && statusLabel.Text.Equals(message))
                || (statusLabel.Image != null && statusLabel.Equals(image)))
                return;

            statusLabel.Image = image;
            statusLabel.Text = Helpers.FirstLine(message);
        }

        public void SetProgressBar(bool visible, int progress)
        {
            statusProgressBar.Visible = visible;
            statusProgressBar.Value = progress;
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            MainMenuBar.Location = new Point(0, 0);

            if (ToolStrip.Renderer is ToolStripProfessionalRenderer)
            {
                ((ToolStripProfessionalRenderer)ToolStrip.Renderer).RoundedEdges = false;
            }

            ConnectionsManager.XenConnections.CollectionChanged += XenConnection_CollectionChanged;
            try
            {
                Settings.RestoreSession();
                HealthCheck.SendProxySettingsToHealthCheck();
            }
            catch (ConfigurationErrorsException ex)
            {
                log.Error("Could not load settings.", ex);
                Program.CloseSplash();
                using (var dlg = new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                       SystemIcons.Error,
                       string.Format(Messages.MESSAGEBOX_LOAD_CORRUPTED, Settings.GetUserConfigPath()),
                       Messages.MESSAGEBOX_LOAD_CORRUPTED_TITLE)))
                {
                    dlg.ShowDialog(this);
                }
                Application.Exit();
                return; // Application.Exit() does not exit the current method.
            }
            ToolbarsEnabled = Properties.Settings.Default.ToolbarsEnabled;
            RequestRefreshTreeView();
            UpdateToolbars();

            // if there are fewer than 30 connections, then expand the tree nodes.
            expandTreeNodesOnStartup = ConnectionsManager.XenConnectionsCopy.Count < 30;

            connectionsInProgressOnStartup = 0;
            // kick-off connections for all the loaded server list
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (!connection.SaveDisconnected)
                {
                    connectionsInProgressOnStartup++;
                    connection.ConnectionStateChanged += Connection_ConnectionStateChangedOnStartup;
                    connection.CachePopulated += connection_CachePopulatedOnStartup;
                    XenConnectionUI.BeginConnect(connection, true, this, true);
                }
            }

            RequestRefreshTreeView();

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate(object o)
            {
                // Sleep a short time before closing the splash
                Thread.Sleep(500);
                Program.Invoke(Program.MainWindow, Program.CloseSplash);
            });

            if (!Program.RunInAutomatedTestMode && !Helpers.CommonCriteriaCertificationRelease)
            {
                if (!Properties.Settings.Default.SeenAllowUpdatesDialog)
                    new AllowUpdatesDialog(pluginManager).ShowDialog(this);

                // start checkforupdates thread
                CheckForUpdatesTimer.Interval = 1000 * 60 * 60 * 24; // 24 hours
                CheckForUpdatesTimer.Tick += CheckForUpdatesTimer_Tick;
                CheckForUpdatesTimer.Start();
                Updates.CheckForUpdates(false);
            }

            if (!Program.RunInAutomatedTestMode)
            {
                // start healthCheckResult thread
                healthCheckResultTimer.Interval = 1000 * 60 * 60; // 1 hour
                healthCheckResultTimer.Tick += HealthCheckResultTimer_Tick;
                healthCheckResultTimer.Start();
            }

            ProcessCommand(CommandLineArgType, CommandLineParam);
        }

        private void CheckForUpdatesTimer_Tick(object sender, EventArgs e)
        {
            Updates.CheckForUpdates(false);
        }

        private void HealthCheckResultTimer_Tick(object sender, EventArgs e)
        {
            HealthCheck.CheckForAnalysisResults();
        }

        private void LoadTasksAsMeddlingActions(IXenConnection connection)
        {
            if (!connection.IsConnected || connection.Session == null)
                return;

            Dictionary<XenRef<Task>, Task> tasks = Task.get_all_records(connection.Session);
            foreach (KeyValuePair<XenRef<Task>, Task> pair in tasks)
            {
                pair.Value.Connection = connection;
                pair.Value.opaque_ref = pair.Key;
                MeddlingActionManager.ForceAddTask(pair.Value);
            }
        }

        private void Cache_Changed(object sender, EventArgs e)
        {
            RequestRefreshTreeView();
        }

        private void connection_CachePopulatedOnStartup(object sender, EventArgs e)
        {
            IXenConnection c = (IXenConnection)sender;
            c.CachePopulated -= connection_CachePopulatedOnStartup;
            if (expandTreeNodesOnStartup)
                TrySelectNewNode(c, false, true, false);

            Program.Invoke(this, ShowAboutDialogOnStartup);
        }

        private void Connection_ConnectionStateChangedOnStartup(object sender, EventArgs e)
        {
            IXenConnection c = (IXenConnection)sender;
            c.ConnectionStateChanged -= Connection_ConnectionStateChangedOnStartup;

            Program.Invoke(Program.MainWindow, delegate
            {
                connectionsInProgressOnStartup--;
                // show the About dialog if this was the last connection in progress and the connection failed
                if (!c.IsConnected)
                    ShowAboutDialogOnStartup();
            }); 
        }

        /// <summary>
        /// Show the About dialog after all conncections kicked-off on startup have finished the connection phase (cache populated)
        /// Must be called on the event thread.
        /// </summary>
        private void ShowAboutDialogOnStartup()
        {
            Program.AssertOnEventThread();
            if (connectionsInProgressOnStartup > 0)
                return;
            if (Properties.Settings.Default.ShowAboutDialog && HiddenFeatures.LicenseNagVisible)
                ShowForm(typeof(AboutDialog));
        }

        private bool Launched = false;
        internal void ProcessCommand(ArgType argType, string[] args)
        {
            switch (argType)
            {
                case ArgType.Import:
                    log.DebugFormat("Importing VM export from {0}", args[0]);
                    OpenGlobalImportWizard(args[0]);
                    break;
                case ArgType.License:
                    log.DebugFormat("Installing license from {0}", args[0]);
                    LaunchLicensePicker(args[0]);
                    break;
                case ArgType.Restore:
                    log.DebugFormat("Restoring host backup from {0}", args[0]);
                    new RestoreHostFromBackupCommand(this, null, args[0]).Execute();
                    break;
                case ArgType.Update:
                    log.DebugFormat("Installing server update from {0}", args[0]);
                    InstallUpdate(args[0]);
                    break;
                case ArgType.XenSearch:
                    log.DebugFormat("Importing saved XenSearch from '{0}'", args[0]);
                    new ImportSearchCommand(this, args[0]).Execute();
                    break;
                case ArgType.Connect:
                    log.DebugFormat("Connecting to server '{0}'", args[0]);
                    IXenConnection connection = new XenConnection();
                    connection.Hostname = args[0];
                    connection.Port = ConnectionsManager.DEFAULT_XEN_PORT;
                    connection.Username = args[1];
                    connection.Password = args[2];
                    if (ConnectionsManager.XenConnectionsContains(connection))
                        break;

                    lock (ConnectionsManager.ConnectionsLock)
                        ConnectionsManager.XenConnections.Add(connection);

                    XenConnectionUI.BeginConnect(connection, true, null, false);
                    break;
                case ArgType.None:
                    if (Launched)
                    {
                        // The user has launched the splash screen, but we're already running.
                        // Draw his attention.
                        HelpersGUI.BringFormToFront(this);
                        Activate();
                    }
                    break;
                case ArgType.Passwords:
                    System.Diagnostics.Trace.Assert(false);
                    break;
            }
            Launched = true;
        }

        // Manages UI and network updates whenever hosts are added and removed
        void XenConnection_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (Program.Exiting)
                return;

            //Program.AssertOnEventThread();
            Program.BeginInvoke(Program.MainWindow, () => XenConnectionCollectionChanged(e));
        }


        private readonly CollectionChangeEventHandler PoolCollectionChangedWithInvoke = null;
        private readonly CollectionChangeEventHandler MessageCollectionChangedWithInvoke = null;
        private readonly CollectionChangeEventHandler HostCollectionChangedWithInvoke = null;
        private readonly CollectionChangeEventHandler VMCollectionChangedWithInvoke = null;
        private readonly CollectionChangeEventHandler SRCollectionChangedWithInvoke = null;
        private readonly CollectionChangeEventHandler FolderCollectionChangedWithInvoke = null;
        private readonly CollectionChangeEventHandler TaskCollectionChangedWithInvoke = null;

        private void XenConnectionCollectionChanged(CollectionChangeEventArgs e)
        {
            try
            {
                IXenConnection connection = e.Element as IXenConnection;

                navigationPane.XenConnectionCollectionChanged(e);

                if (e.Action == CollectionChangeAction.Add)
                {
                    if (connection == null)
                        return;

                    connection.ClearingCache += connection_ClearingCache;
                    connection.ConnectionResult += Connection_ConnectionResult;
                    connection.ConnectionLost += Connection_ConnectionLost;
                    connection.ConnectionClosed += Connection_ConnectionClosed;
                    connection.ConnectionReconnecting += connection_ConnectionReconnecting;
                    connection.XenObjectsUpdated += Connection_XenObjectsUpdated;
                    connection.Cache.RegisterCollectionChanged<XenAPI.Message>(MessageCollectionChangedWithInvoke);
                    connection.Cache.RegisterCollectionChanged<Pool>(PoolCollectionChangedWithInvoke);
                    connection.Cache.RegisterCollectionChanged<Host>(HostCollectionChangedWithInvoke);
                    connection.Cache.RegisterCollectionChanged<VM>(VMCollectionChangedWithInvoke);
                    connection.Cache.RegisterCollectionChanged<SR>(SRCollectionChangedWithInvoke);
                    connection.Cache.RegisterCollectionChanged<Folder>(FolderCollectionChangedWithInvoke);

                    connection.Cache.RegisterCollectionChanged<Task>(TaskCollectionChangedWithInvoke);

                    connection.CachePopulated += connection_CachePopulated;
                }
                else if (e.Action == CollectionChangeAction.Remove)
                {
                    var range = new List<IXenConnection>();
                    if (connection != null)
                    {
                        range.Add(connection);
                    }
                    else
                    {
                        var r = e.Element as List<IXenConnection>;
                        if (r != null)
                            range = r;
                        else
                            return;
                    }

                    foreach (var con in range)
                    {
                        con.ClearingCache -= connection_ClearingCache;
                        con.ConnectionResult -= Connection_ConnectionResult;
                        con.ConnectionLost -= Connection_ConnectionLost;
                        con.ConnectionClosed -= Connection_ConnectionClosed;
                        con.ConnectionReconnecting -= connection_ConnectionReconnecting;
                        con.XenObjectsUpdated -= Connection_XenObjectsUpdated;
                        con.Cache.DeregisterCollectionChanged<XenAPI.Message>(MessageCollectionChangedWithInvoke);
                        con.Cache.DeregisterCollectionChanged<Pool>(PoolCollectionChangedWithInvoke);
                        con.Cache.DeregisterCollectionChanged<Host>(HostCollectionChangedWithInvoke);
                        con.Cache.DeregisterCollectionChanged<VM>(VMCollectionChangedWithInvoke);
                        con.Cache.DeregisterCollectionChanged<SR>(SRCollectionChangedWithInvoke);
                        con.Cache.DeregisterCollectionChanged<Folder>(FolderCollectionChangedWithInvoke);

                        con.Cache.DeregisterCollectionChanged<Task>(TaskCollectionChangedWithInvoke);

                        con.CachePopulated -= connection_CachePopulated;

                        foreach (VM vm in con.Cache.VMs)
                        {
                            ConsolePanel.closeVNCForSource(vm);
                        }

                        foreach (Host host in con.Cache.Hosts)
                        {
                            ConsolePanel.closeVNCForSource(host.ControlDomainZero());

                            foreach (VM vm in host.OtherControlDomains())
                                CvmConsolePanel.closeVNCForSource(vm);
                        }

                        con.EndConnect();
                    }
                    RequestRefreshTreeView();
                    //CA-41228 refresh submenu items when there are no connections
                    SelectionManager.RefreshSelection();
                }
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
                // Can't do any more about this.
            }
        }

        /// <summary>
        /// Closes any wizards for this connection. Must be done before we clear the cache so that per-VM wizards are closed.
        /// In many cases this is already covered (e.g. if the user explicitly disconnects). This method ensures we also
        /// do it when we unexpectedly lose the connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connection_ClearingCache(object sender, EventArgs e)
        {
            IXenConnection connection = (IXenConnection)sender;
            CloseActiveWizards(connection);
            Alert.RemoveAlert(alert => alert.Connection != null && alert.Connection.Equals(connection));
            Updates.CheckServerPatches();
            Updates.CheckServerVersion();

            RequestRefreshTreeView();
        }

        void connection_CachePopulated(object sender, EventArgs e)
        {
            IXenConnection connection = sender as IXenConnection;
            if (connection == null)
                return;

            Host master = Helpers.GetMaster(connection);
            if (master == null)
                return;

            log.InfoFormat("Connected to {0} (version {1}, build {2}.{3}) with {4} {5} (build {6}.{7})",
                Helpers.GetName(master), Helpers.HostProductVersionText(master), Helpers.HostProductVersion(master),
                master.BuildNumberRaw(), Messages.XENCENTER, Branding.PRODUCT_VERSION_TEXT,
                Branding.XENCENTER_VERSION, Program.Version.Revision);

            // Check the PRODUCT_BRAND
            if (!Program.RunInAutomatedTestMode && !SameProductBrand(master))
            {
                connection.EndConnect();

                Program.Invoke(Program.MainWindow, delegate()
                {
                    string msg = string.Format(Messages.INCOMPATIBLE_PRODUCTS, Helpers.GetName(master));
                    string url = "";

                    using (var dlog = new ConnectionRefusedDialog())
                    {
                        dlog.ErrorMessage = msg;
                        dlog.Url = "";
                        dlog.ShowDialog(this);
                    }

                    new ActionBase(Messages.CONNECTION_REFUSED_TITLE,
                                   string.Format("{0}\n{1}", msg, url), false,
                                   true, Messages.CONNECTION_REFUSED);
                });
                return;
            }

            // When releasing a new version of the server, we should set xencenter_min and xencenter_max on the server
            // as follows:
            //
            // xencenter_min should be the lowest version of XenCenter we want the new server to work with. In the
            // (common) case that we want to force the user to upgrade XenCenter when they upgrade the server,
            // xencenter_min should equal the current version of XenCenter.  // if (server_min > current_version)
            //
            // xencenter_max should always equal the current version of XenCenter. This ensures that even if they are
            // not required to upgrade, we at least warn them.  // else if (server_max > current_version)

            int server_min = master.XenCenterMin();
            int server_max = master.XenCenterMax();

            if (server_min > 0 && server_max > 0)
            {
                int current_version = (int)API_Version.LATEST;

                if (server_min > current_version)
                {
                    connection.EndConnect();

                    Program.Invoke(Program.MainWindow, delegate()
                    {
                        string msg = string.Format(Messages.GUI_OUT_OF_DATE, Helpers.GetName(master));
                        msg = msg + Messages.GUI_OUT_OF_DATE_MORE;
                        string url = "https://" + connection.Hostname;

                        using (var dlog = new ConnectionRefusedDialog())
                        {
                            dlog.ErrorMessage = msg;
                            dlog.Url = url;
                            dlog.ShowDialog(this);
                        }
                        
                        new ActionBase(Messages.CONNECTION_REFUSED_TITLE,
                                       string.Format("{0}\n{1}", msg, url), false,
                                       true, Messages.CONNECTION_REFUSED);
                    });
                    return;
                }
                else if (server_max > current_version)
                {
                    Alert.AddAlert(new GuiOldAlert());
                }

                LoadTasksAsMeddlingActions(connection);
            }

            //
            // Every time we connect, make sure any host with other_config[maintenance_mode] == true
            // is disabled.
            //
            CheckMaintenanceMode(connection);

            if (HelpersGUI.iSCSIisUsed())
                HelpersGUI.PerformIQNCheck();

            if(licenseTimer != null)
                licenseTimer.CheckActiveServerLicense(connection, false);

            if (Properties.Settings.Default.ShowHealthCheckEnrollmentReminder)
                ThreadPool.QueueUserWorkItem(CheckHealthCheckEnrollment, connection);
            ThreadPool.QueueUserWorkItem(HealthCheck.CheckForAnalysisResults, connection);
            ThreadPool.QueueUserWorkItem(InformHealthCheckEnrollment, connection);
            
            Updates.CheckServerPatches();
            Updates.CheckServerVersion();

            HealthCheck.SendMetadataToHealthCheck();
            RequestRefreshTreeView();
        }

        private void CheckHealthCheckEnrollment(object connection)
        {
            if (HealthCheckOverviewLauncher != null && !HiddenFeatures.HealthCheckHidden)
                HealthCheckOverviewLauncher.CheckHealthCheckEnrollment((IXenConnection) connection);
        }

        private void InformHealthCheckEnrollment(object connection)
        {
            Pool pool = Helpers.GetPoolOfOne((IXenConnection)connection);
            if (pool == null)
                return;
            var newHealthCheckSSettings = pool.HealthCheckSettings();
            new TransferHealthCheckSettingsAction(pool, newHealthCheckSSettings,
                newHealthCheckSSettings.GetSecretyInfo(pool.Connection, HealthCheckSettings.UPLOAD_CREDENTIAL_USER_SECRET),
                newHealthCheckSSettings.GetSecretyInfo(pool.Connection, HealthCheckSettings.UPLOAD_CREDENTIAL_PASSWORD_SECRET), true).RunAsync();
        }

        public static bool SameProductBrand(Host host)
        {
            // XCP-ng Console: we want to connect to any flavor of XenServer
            return true;
            //var brand = host.ProductBrand();
            //return brand == Branding.PRODUCT_BRAND || Branding.PRODUCT_BRAND == "[XenServer product]";
        }

        /// <summary>
        /// Ensures all hosts on the connection are disabled if they are in maintenance mode.
        /// </summary>
        /// <param name="connection"></param>
        private void CheckMaintenanceMode(IXenConnection connection)
        {
            foreach (Host host in connection.Cache.Hosts)
            {
                CheckMaintenanceMode(host);
            }
        }

        /// <summary>
        /// Ensures the host is disabled if it is in maintenance mode by spawning a new HostAction if necessary.
        /// </summary>
        /// <param name="host"></param>
        private void CheckMaintenanceMode(Host host)
        {
            if (host.IsLive() && host.MaintenanceMode() && host.enabled)
            {
                Program.MainWindow.CloseActiveWizards(host);

                var action = new DisableHostAction(host);
                action.Completed += action_Completed;
                action.RunAsync();
                Program.Invoke(this, UpdateToolbars);
            }
        }

        void MessageCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.AssertOnEventThread();

            XenAPI.Message m = (XenAPI.Message)e.Element;
            if (e.Action == CollectionChangeAction.Add)
            {
                if (!m.ShowOnGraphs() && !m.IsSquelched())
                    Alert.AddAlert(MessageAlert.ParseMessage(m));
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                if (!m.ShowOnGraphs())
                    MessageAlert.RemoveAlert(m);
            }
        }

        void CollectionChanged<T>(object sender, CollectionChangeEventArgs e) where T : XenObject<T>
        {
            Program.AssertOnEventThread();

            T o = (T)e.Element;
            if (e.Action == CollectionChangeAction.Add)
            {
                if (o is Pool)
                    ((Pool)e.Element).PropertyChanged += Pool_PropertyChanged;
                else if (o is Host)
                    ((Host)e.Element).PropertyChanged += Host_PropertyChanged;
                else if (o is VM)
                    ((VM)e.Element).PropertyChanged += VM_PropertyChanged;
                else
                    o.PropertyChanged += o_PropertyChanged;
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                if (o is Pool)
                    ((Pool)e.Element).PropertyChanged -= Pool_PropertyChanged;
                else if (o is Host)
                    ((Host)e.Element).PropertyChanged -= Host_PropertyChanged;
                else if (o is VM)
                    ((VM)e.Element).PropertyChanged -= VM_PropertyChanged;
                else
                    o.PropertyChanged -= o_PropertyChanged;

                if (o is VM)
                {
                    VM vm = (VM)e.Element;
                    ConsolePanel.closeVNCForSource(vm);
                    CloseActiveWizards(vm);
                }

                selectedTabs.Remove(o);
                pluginManager.DisposeURLs(o);
            }
        }

        private void Pool_PropertyChanged(object obj, PropertyChangedEventArgs e)
        {
            Pool pool = (Pool)obj;
            switch (e.PropertyName)
            {
                case "other_config":
                    // other_config may contain HideFromXenCenter
                    UpdateToolbars();
                    // other_config contains which patches to ignore
                    Updates.CheckServerPatches();
                    Updates.CheckServerVersion();
                    break;

                case "name_label":
                    pool.Connection.FriendlyName = Helpers.GetName(pool);
                    break;
            }
        }

        private void Host_PropertyChanged(object obj, PropertyChangedEventArgs e)
        {
            Host host = (Host)obj;
            switch (e.PropertyName)
            {
                case "allowed_operations":
                case "enabled":
                    // We want to ensure that a host is disabled if it is in maintenance mode, by starting a new DisableHostAction if necessary (CheckMaintenanceMode)
                    if (host.enabled && host.MaintenanceMode())
                    {
                        // This is an invalid state: the host is enabled but still "in maintenance mode";
                        // But maybe MaintenanceMode hasn't been updated yet, because host.enabled being processed before host.other_config (CA-75625);
                        // We'll check it again after the cache update operation is complete, in Connection_XenObjectsUpdated 
                        hostsInInvalidState.Add(host);
                    }
                    UpdateToolbars();
                    break;
                case "edition":
                case "license_server":
                case "license_params":
                    UpdateHeader();
                    UpdateToolbars();
                    break;
                case "other_config":
                    // other_config may contain HideFromXenCenter
                    UpdateToolbars();
                    break;

                case "name_label":
                    //check whether it's a standalone host
                    if(Helpers.GetPool(host.Connection) == null)
                        host.Connection.FriendlyName = Helpers.GetName(host);
                    break;
            }
        }

        private void VM_PropertyChanged(object obj, PropertyChangedEventArgs e)
        {
            VM vm = (VM)obj;
            switch (e.PropertyName)
            {
                case "allowed_operations":
                case "is_a_template":
                case "resident_on":
                    UpdateToolbars();
                    break;

                case "power_state":
                case "other_config": // other_config may contain HideFromXenCenter
                    UpdateToolbars();
                    // Make all vms have the correct start times
                    UpdateBodgedTime(vm, e.PropertyName);
                    break;
            }
        }

        void o_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "allowed_operations":
                case "power_state":
                case "is_a_template":
                case "enabled":
                case "other_config": // other_config may contain HideFromXenCenter
                    UpdateToolbars();
                    break;
            }
        }

        // Update bodged startup time if the powerstate goes to running (vm started from halted), otherconfig last shutdown changed (vm rebooted) or start time changed (occurs a few seconds after start)
        private void UpdateBodgedTime(VM vm, string p)
        {
            if (vm == null)
                return;
            if (p == "power_state")
            {
                vm.SetBodgeStartupTime(DateTime.UtcNow); // always newer than current bodge startup time
            }
            else if (p == "other_config" && vm.other_config.ContainsKey("last_shutdown_time"))
            {
                DateTime newTime = vm.LastShutdownTime();
                if (newTime != DateTime.MinValue && newTime.Ticks > vm.GetBodgeStartupTime().Ticks)
                    vm.SetBodgeStartupTime(newTime); // only update if is newer than current bodge startup time
            }
        }

        void Connection_ConnectionResult(object sender, Network.ConnectionResultEventArgs e)
        {
            RequestRefreshTreeView();
            Program.Invoke(this, (EventHandler<ConnectionResultEventArgs>)Connection_ConnectionResult_, sender, e);
        }

        private void Connection_ConnectionResult_(object sender, Network.ConnectionResultEventArgs e)
        {
            Program.AssertOnEventThread();
            try
            {
                UpdateToolbars();
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
                // Can do nothing more about this.
            }
        }

        void Connection_ConnectionClosed(object sender, EventArgs e)
        {
            RequestRefreshTreeView();
            Program.Invoke(this, (EventHandler<Network.ConnectionResultEventArgs>)Connection_ConnectionClosed_, sender, e);
            gc();
        }

        private void Connection_ConnectionClosed_(object sender, EventArgs e)
        {
            Program.AssertOnEventThread();
            try
            {
                UpdateToolbars();
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
                // Nothing more we can do with this.
            }
        }

        // called whenever our connection with the Xen server fails (i.e., after we've successfully logged in)
        void Connection_ConnectionLost(object sender, EventArgs e)
        {
            if (Program.Exiting)
                return;
            Program.Invoke(this, (EventHandler)Connection_ConnectionLost_, sender, e);
            RequestRefreshTreeView();
            gc();
        }

        private void Connection_ConnectionLost_(object sender, EventArgs e)
        {
            Program.AssertOnEventThread();
            try
            {
                IXenConnection connection = (IXenConnection)sender;
                CloseActiveWizards(connection);

                UpdateToolbars();
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
                // Can do nothing about this.
            }
        }

        private static void gc()
        {
            //log_gc("Before");  // Don't log this, CA-159791
            GC.Collect();
            //log_gc("After");
        }

        private static void log_gc(string when)
        {
            log.DebugFormat("{0} GC: approx {1} bytes in use", when, GC.GetTotalMemory(false));
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                log.DebugFormat("Number of times GC has occurred for generation {0} objects: {1}", i, GC.CollectionCount(i));
            }
            log.Debug("GDI objects in use: " + Win32.GetGuiResourcesGDICount(Process.GetCurrentProcess().Handle));
            log.Debug("USER objects in use: " + Win32.GetGuiResourcesUserCount(Process.GetCurrentProcess().Handle));
        }

        void connection_ConnectionReconnecting(object sender, EventArgs e)
        {
            if (Program.Exiting)
                return;
            RequestRefreshTreeView();
            gc();
        }

        private List<Host> hostsInInvalidState = new List<Host>();

        // called whenever Xen objects on the server change state
        void Connection_XenObjectsUpdated(object sender, EventArgs e)
        {
            if (Program.Exiting)
                return;

            IXenConnection connection = (IXenConnection) sender;

            if (hostsInInvalidState.Count > 0)
            {
                foreach (var host in hostsInInvalidState.Where(host => host.Connection == connection))
                    CheckMaintenanceMode(host);
                hostsInInvalidState.RemoveAll(host => host.Connection == connection);
            }

            RequestRefreshTreeView();
        }

        private int ignoreUpdateToolbars = 0;
        private bool calledUpdateToolbars = false;

        /// <summary>
        /// Requests a refresh of the main tree view. The refresh will be managed such that we are not overloaded using an UpdateManager.
        /// </summary>
        public void RequestRefreshTreeView()
        {
            Program.Invoke(this, navigationPane.RequestRefreshTreeView);
        }

        private void UpdateHeaderAndTabPages()
        {
            Program.Invoke(this, () =>
                {
                    // This is required to update search results when things change.
                    if (TheTabControl.SelectedTab == TabPageGeneral)
                        GeneralPage.BuildList();
                    else if (TheTabControl.SelectedTab == TabPageSearch)
                        SearchPage.BuildList();

                    UpdateHeader();
                });
        }

        void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool _menuShortcuts = true;
        public bool MenuShortcuts
        {
            set
            {
                if (value != _menuShortcuts)
                {
                    //if the VNC Console is active (the user is typing into it etc) all of the shortcuts for XenCenter are disabled
                    //IMPORTANT! add any shortcuts you want to pass to the VNC console into this if, else statement
                    _menuShortcuts = value;

                    // update the selection so menu items can enable/disable keyboard shortcuts as appropriate.
                    SelectionManager.RefreshSelection();
                }
            }
        }

        /// <summary>
        /// Must be called on the event thread.
        /// </summary>
        public void UpdateToolbars()
        {
            Program.AssertOnEventThread();

            if (ignoreUpdateToolbars > 0)
            {
                calledUpdateToolbars = true;
                return;
            }

            ToolStrip.SuspendLayout();

            UpdateToolbarsCore();
            MainMenuBar_MenuActivate(null, null);

            ToolStrip.ResumeLayout();
        }

        private static int TOOLBAR_HEIGHT = 31;
        /// <summary>
        /// Updates the toolbar buttons. Also updates which tabs are visible.
        /// </summary>
        public void UpdateToolbarsCore()
        {
            // refresh the selection-manager
            SelectionManager.RefreshSelection();

            ToolStrip.Height = ToolbarsEnabled ? TOOLBAR_HEIGHT : 0;
            ToolStrip.Enabled = ToolbarsEnabled;
            ShowToolbarMenuItem.Checked = toolbarToolStripMenuItem.Checked = ToolbarsEnabled;

            bool containerButtonsAvailable = startContainerToolStripButton.Enabled || stopContainerToolStripButton.Enabled || 
                resumeContainerToolStripButton.Enabled || pauseContainerToolStripButton.Enabled || restartContainerToolStripButton.Enabled;

            startContainerToolStripButton.Available = containerButtonsAvailable && startContainerToolStripButton.Enabled;
            stopContainerToolStripButton.Available = containerButtonsAvailable && (stopContainerToolStripButton.Enabled || !startContainerToolStripButton.Available);
            resumeContainerToolStripButton.Available = containerButtonsAvailable && resumeContainerToolStripButton.Enabled;
            pauseContainerToolStripButton.Available = containerButtonsAvailable && (pauseContainerToolStripButton.Enabled || !resumeContainerToolStripButton.Available);
            restartContainerToolStripButton.Available = containerButtonsAvailable;

            powerOnHostToolStripButton.Available = powerOnHostToolStripButton.Enabled;
            startVMToolStripButton.Available = startVMToolStripButton.Enabled;
            shutDownToolStripButton.Available = shutDownToolStripButton.Enabled || (!startVMToolStripButton.Available && !powerOnHostToolStripButton.Available && !containerButtonsAvailable);
            RebootToolbarButton.Available = RebootToolbarButton.Enabled || !containerButtonsAvailable;

            resumeToolStripButton.Available = resumeToolStripButton.Enabled;
            SuspendToolbarButton.Available = SuspendToolbarButton.Enabled || (!resumeToolStripButton.Available && !containerButtonsAvailable);

            ForceRebootToolbarButton.Available = ((ForceVMRebootCommand)ForceRebootToolbarButton.Command).ShowOnMainToolBar;
            ForceShutdownToolbarButton.Available = ((ForceVMShutDownCommand)ForceShutdownToolbarButton.Command).ShowOnMainToolBar;

            IXenConnection selectionConnection = SelectionManager.Selection.GetConnectionOfFirstItem();
            Pool selectionPool = selectionConnection == null ? null : Helpers.GetPool(selectionConnection);
            Host selectionMaster = null == selectionPool ? null : selectionPool.Connection.Resolve(selectionPool.master);

            // 'Home' tab is only visible if the 'Overview' tree node is selected, or if the tree is
            // empty (i.e. at startup).
            bool show_home = SelectionManager.Selection.Count == 1 && SelectionManager.Selection[0].Value == null;
            // The upsell pages use the first selected XenObject: but they're only shown if there is only one selected object (see calls to ShowTab() below).
            bool dmc_upsell = Helpers.FeatureForbidden(SelectionManager.Selection.FirstAsXenObject, Host.RestrictDMC);
            bool ha_upsell = Helpers.FeatureForbidden(SelectionManager.Selection.FirstAsXenObject, Host.RestrictHA) && (selectionPool != null && !selectionPool.ha_enabled);
            bool wlb_upsell = Helpers.FeatureForbidden(SelectionManager.Selection.FirstAsXenObject, Host.RestrictWLB);
            bool ad_upsell = Helpers.FeatureForbidden(SelectionManager.Selection.FirstAsXenObject, Host.RestrictAD);
            bool is_connected = selectionConnection != null && selectionConnection.IsConnected;

            bool multi = SelectionManager.Selection.Count > 1;

            bool isPoolSelected = SelectionManager.Selection.FirstIs<Pool>();
            bool isVMSelected = SelectionManager.Selection.FirstIs<VM>();
            bool isHostSelected = SelectionManager.Selection.FirstIs<Host>();
            bool isSRSelected = SelectionManager.Selection.FirstIs<SR>();
            bool isVdiSelected = SelectionManager.Selection.FirstIs<VDI>();
            bool isRealVMSelected = SelectionManager.Selection.FirstIsRealVM;
            bool isTemplateSelected = SelectionManager.Selection.FirstIsTemplate;
            bool isHostLive = SelectionManager.Selection.FirstIsLiveHost;
            bool isDockerContainerSelected = SelectionManager.Selection.First is DockerContainer;
            bool hasManyControlDomains = isHostSelected && ((Host)SelectionManager.Selection.First).HasManyControlDomains();

            bool selectedTemplateHasProvisionXML = SelectionManager.Selection.FirstIsTemplate && ((VM)SelectionManager.Selection[0].XenObject).HasProvisionXML();

            NewTabCount = 0;
            ShowTab(TabPageHome, !SearchMode && show_home);
            ShowTab(TabPageGeneral, !multi && !SearchMode && (isVMSelected || (isHostSelected && (isHostLive || !is_connected)) || isPoolSelected || isSRSelected || isVdiSelected || isDockerContainerSelected));
            ShowTab(dmc_upsell ? TabPageBallooningUpsell : TabPageBallooning, !multi && !SearchMode && (isVMSelected || (isHostSelected && isHostLive) || isPoolSelected));
            ShowTab(TabPageStorage, !multi && !SearchMode && (isRealVMSelected || (isTemplateSelected && !selectedTemplateHasProvisionXML)));
            ShowTab(TabPageSR, !multi && !SearchMode && isSRSelected);
            ShowTab(TabPagePhysicalStorage, !multi && !SearchMode && ((isHostSelected && isHostLive) || isPoolSelected));
            ShowTab(TabPageNetwork, !multi && !SearchMode && (isVMSelected || (isHostSelected && isHostLive) || isPoolSelected));
            ShowTab(TabPageNICs, !multi && !SearchMode && ((isHostSelected && isHostLive)));
            ShowTab(TabPageDockerProcess, !multi && !SearchMode && isDockerContainerSelected && !(SelectionManager.Selection.First as DockerContainer).Parent.IsWindows());
            ShowTab(TabPageDockerDetails, !multi && !SearchMode && isDockerContainerSelected);

            bool isPoolOrLiveStandaloneHost = isPoolSelected || (isHostSelected && isHostLive && selectionPool == null);
            ShowTab(TabPageGPU, !multi && !SearchMode && ((isHostSelected && isHostLive) || isPoolOrLiveStandaloneHost) && Helpers.ClearwaterSp1OrGreater(selectionConnection) && !Helpers.FeatureForbidden(selectionConnection, Host.RestrictGpu));
            ShowTab(TabPageUSB, !multi && !SearchMode && (isHostSelected && isHostLive && (((Host)SelectionManager.Selection.First).PUSBs.Count > 0)) && !Helpers.FeatureForbidden(selectionConnection, Host.RestrictUsbPassthrough));

            pluginManager.SetSelectedXenObject(SelectionManager.Selection.FirstAsXenObject);

            bool shownConsoleReplacement = false;
            foreach (TabPageFeature f in pluginManager.GetAllFeatures<TabPageFeature>(f => f.IsConsoleReplacement && !f.IsError && !multi && f.ShowTab))
            {
                ShowTab(f.TabPage, true);
                shownConsoleReplacement = true;
            }

            ShowTab(TabPageConsole, !shownConsoleReplacement && !multi && !SearchMode && (isRealVMSelected || (isHostSelected && isHostLive)));
            ShowTab(TabPageCvmConsole, !shownConsoleReplacement && !multi && !SearchMode && isHostLive && hasManyControlDomains);
            ShowTab(TabPagePeformance, !multi && !SearchMode && (isRealVMSelected || (isHostSelected && isHostLive)));
            ShowTab(ha_upsell ? TabPageHAUpsell : TabPageHA, !multi && !SearchMode && isPoolSelected);
            ShowTab(TabPageSnapshots, !multi && !SearchMode && isRealVMSelected);

            //Any Clearwater XenServer, or WLB is not licensed on XenServer, the WLB tab and any WLB menu items disappear completely.
            if(!(SelectionManager.Selection.All(s => Helpers.IsClearwater(s.Connection)) || wlb_upsell ))
                ShowTab(TabPageWLB, !multi && !SearchMode && isPoolSelected);

            ShowTab(ad_upsell ? TabPageADUpsell : TabPageAD, !multi && !SearchMode && (isPoolSelected || isHostSelected && isHostLive));

            ShowTab(TabPagePvs, !multi && !SearchMode && isPoolOrLiveStandaloneHost && !Helpers.FeatureForbidden(SelectionManager.Selection.FirstAsXenObject, Host.RestrictPvsCache)
                && Helpers.PvsCacheCapability(selectionConnection));

            foreach (TabPageFeature f in pluginManager.GetAllFeatures<TabPageFeature>(f => !f.IsConsoleReplacement && !multi && f.ShowTab))
                ShowTab(f.TabPage, true);

            ShowTab(TabPageSearch, true);

            // N.B. Change NewTabs definition if you add more tabs here.

            // Save and restore focus on treeView, since selecting tabs in ChangeToNewTabs() has the
            // unavoidable side-effect of giving them focus - this is irritating if trying to navigate
            // the tree using the keyboard.

            navigationPane.SaveAndRestoreTreeViewFocus(ChangeToNewTabs);
        }

        private readonly TabPage[] NewTabs = new TabPage[512];
        int NewTabCount;
        private void ShowTab(TabPage page, bool visible)
        {
            if (visible)
            {
                NewTabs[NewTabCount] = page;
                NewTabCount++;
            }
        }

        private void ChangeToNewTabs()
        {
            TabPage new_selected_page = NewSelectedPage();
            TheTabControl.SuspendLayout();
            IgnoreTabChanges = true;
            try
            {
                TabControl.TabPageCollection p = TheTabControl.TabPages;
                int i = 0; // Index into NewTabs
                int m = 0; // Index into p

                while (i < NewTabCount)
                {
                    if (m == p.Count)
                    {
                        p.Add(NewTabs[i]);
                        if (new_selected_page == NewTabs[i])
                            TheTabControl.SelectedTab = new_selected_page;
                        m++;
                        i++;
                    }
                    else if (p[m] == NewTabs[i])
                    {
                        if (new_selected_page == NewTabs[i])
                            TheTabControl.SelectedTab = new_selected_page;
                        m++;
                        i++;
                    }
                    else if (NewTabs.Contains(p[m]))
                    {
                        p.Insert(m, NewTabs[i]);
                        if (new_selected_page == NewTabs[i])
                            TheTabControl.SelectedTab = new_selected_page;
                        m++;
                        i++;
                    }
                    else
                    {
                        if (TheTabControl.SelectedTab == p[m] && new_selected_page == NewTabs[i])
                        {
                            // This clause is deliberately targeted at the case when you go from
                            // Overview:Home to Host:Overview.
                            p.Insert(m, NewTabs[i]);
                            TheTabControl.SelectedTab = new_selected_page;
                            m++;
                            i++;
                        }
                        p.Remove(p[m]);
                    }
                }

                // Remove any tabs that are left at the end of the list.
                while (m < p.Count)
                {
                    TabPage removed = p[p.Count - 1];
                    p.Remove(removed);
                    int index = NewTabsIndexOf(removed);
                    if (index != -1)
                    {
                        // If this is a tab that we want, then we've got it in the list twice -- one
                        // reference was here when we entered this function, and is the one that we're
                        // pointing at now, and the other reference we've inserted through the loop above.
                        // This is bad -- p.Remove(removed) above has now invalidated removed.Parent
                        // (removed is still in the list through the other reference).  Fix this up by
                        // removing the other reference too and starting over.
                        // We can't do the Remove call in the loop above, because this has a poor visual
                        // effect.
                        p.Remove(removed);
                        p.Insert(index, removed);
                        if (new_selected_page == removed)
                            TheTabControl.SelectedTab = removed;
                    }
                }
            }
            finally
            {
                IgnoreTabChanges = false;
                TheTabControl.ResumeLayout();

                SetLastSelectedPage(SelectionManager.Selection.First, TheTabControl.SelectedTab);
            }
        }

        private TabPage NewSelectedPage()
        {
            Object o = SelectionManager.Selection.First;
            IXenObject s = o as IXenObject;

            TabPage last_selected_page = GetLastSelectedPage(o);
            return last_selected_page != null && NewTabs.Contains(last_selected_page)
                       ? last_selected_page
                       : NewTabs[0];
        }

        private void SetLastSelectedPage(object o, TabPage p)
        {
            if (SearchMode)
                return;

            if (o == null)
            {
                selectedOverviewTab = p;
            }
            else
            {
                selectedTabs[o] = p;
            }
        }

        private TabPage GetLastSelectedPage(object o)
        {
            return
                o == null ? selectedOverviewTab :
                selectedTabs.ContainsKey(o) ? selectedTabs[o] :
                                               null;
        }

        private int NewTabsIndexOf(TabPage tp)
        {
            for (int i = 0; i < NewTabCount; i++)
            {
                if (NewTabs[i] == tp)
                    return i;
            }
            return -1;
        }

        private void pluginManager_PluginsChanged()
        {
            foreach (ToolStripMenuItem menu in MainMenuBar.Items)
            {
                //clear existing plugin items
                for (int i = menu.DropDownItems.Count - 1; i >= 0; i--)
                {
                    CommandToolStripMenuItem commandMenuItem = menu.DropDownItems[i] as CommandToolStripMenuItem;

                    if (commandMenuItem != null && (commandMenuItem.Command is MenuItemFeatureCommand 
                                                 || commandMenuItem.Command is ParentMenuItemFeatureCommand))
                    {
                        menu.DropDownItems.RemoveAt(i);

                        if (menu.DropDownItems.Count > 0 && menu.DropDownItems[i] is ToolStripSeparator)
                            menu.DropDownItems.RemoveAt(i);
                    }
                }

                // get insert index using the placeholder
                int insertIndex = pluginMenuItemStartIndexes[menu];

                bool itemAdded = false;

                // add plugin items for this menu at insertIndex
                foreach (PluginDescriptor plugin in pluginManager.Plugins)
                {
                    if (!plugin.Enabled)
                        continue;

                    foreach (Plugins.Feature feature in plugin.Features)
                    {
                        var menuItemFeature = feature as MenuItemFeature;

                        if (menuItemFeature != null && menuItemFeature.ParentFeature == null && (int)menuItemFeature.Menu == MainMenuBar.Items.IndexOf(menu))
                        {
                            Command cmd = menuItemFeature.GetCommand(this, SelectionManager.Selection);

                            menu.DropDownItems.Insert(insertIndex, new CommandToolStripMenuItem(cmd));
                            insertIndex++;
                            itemAdded = true;
                        }

                        var parentMenuItemFeature = feature as ParentMenuItemFeature;

                        if (parentMenuItemFeature != null && (int)parentMenuItemFeature.Menu == MainMenuBar.Items.IndexOf(menu))
                        {
                            Command cmd = parentMenuItemFeature.GetCommand(this, SelectionManager.Selection);
                            CommandToolStripMenuItem parentMenuItem = new CommandToolStripMenuItem(cmd);

                            menu.DropDownItems.Insert(insertIndex, parentMenuItem);
                            insertIndex++;
                            itemAdded = true;

                            foreach (MenuItemFeature childFeature in parentMenuItemFeature.Features)
                            {
                                Command childCommand = childFeature.GetCommand(this, SelectionManager.Selection);
                                parentMenuItem.DropDownItems.Add(new CommandToolStripMenuItem(childCommand));
                            }
                        }
                    }
                }

                if (itemAdded && insertIndex != menu.DropDownItems.Count)
                    menu.DropDownItems.Insert(insertIndex, new ToolStripSeparator());
            }
        }

        private void MainMenuBar_MenuActivate(object sender, EventArgs e)
        {
            Host hostAncestor = SelectionManager.Selection.Count == 1 ? SelectionManager.Selection[0].HostAncestor : null;
            IXenConnection connection = SelectionManager.Selection.GetConnectionOfFirstItem();
            bool vm = SelectionManager.Selection.FirstIsRealVM && !((VM)SelectionManager.Selection.First).Locked;

            exportSettingsToolStripMenuItem.Enabled = ConnectionsManager.XenConnectionsCopy.Count > 0;

            this.MenuShortcuts = true;

            startOnHostToolStripMenuItem.Available = startOnHostToolStripMenuItem.Enabled;
            resumeOnToolStripMenuItem.Available = resumeOnToolStripMenuItem.Enabled;
            relocateToolStripMenuItem.Available = relocateToolStripMenuItem.Enabled;
            sendCtrlAltDelToolStripMenuItem.Enabled = (TheTabControl.SelectedTab == TabPageConsole) && vm && ((VM)SelectionManager.Selection.First).power_state == vm_power_state.Running;

            IXenConnection conn;
            conn = SelectionManager.Selection.GetConnectionOfAllItems();
            if (SelectionManager.Selection.Count > 0 && (Helpers.GetMaster(conn) != null) && (Helpers.FalconOrGreater(conn)))
            {
                assignSnapshotScheduleToolStripMenuItem.Available = true;
                VMSnapshotScheduleToolStripMenuItem.Available = true;

            }
            else /* hide VMSS */
            {
                assignSnapshotScheduleToolStripMenuItem.Available = false;
                VMSnapshotScheduleToolStripMenuItem.Available = false;
            }
            
            templatesToolStripMenuItem1.Checked = Properties.Settings.Default.DefaultTemplatesVisible;
            customTemplatesToolStripMenuItem.Checked = Properties.Settings.Default.UserTemplatesVisible;
            localStorageToolStripMenuItem.Checked = Properties.Settings.Default.LocalSRsVisible;
            ShowHiddenObjectsToolStripMenuItem.Checked = Properties.Settings.Default.ShowHiddenVMs;
            connectDisconnectToolStripMenuItem.Enabled = ConnectionsManager.XenConnectionsCopy.Count > 0;
        }

        private void xenSourceOnTheWebToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.OpenURL(InvisibleMessages.HOMEPAGE);
        }

        private void xenCenterPluginsOnTheWebToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.OpenURL(InvisibleMessages.PLUGINS_URL);
        }

        private void aboutXenSourceAdminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(typeof(AboutDialog));
        }

        /// <summary>
        /// Apply license, if HostAncestorOfSelectedNode is null, show host picker, if filepath == "" show filepicker
        /// </summary>
        public void LaunchLicensePicker(string filepath)
        {
            HelpersGUI.BringFormToFront(this);
            OpenFileDialog dialog = null;
            DialogResult result = DialogResult.Cancel;
            if (filepath == "")
            {
                if (!Program.RunInAutomatedTestMode)
                {
                    dialog = new OpenFileDialog();
                    dialog.Multiselect = false;
                    dialog.Title = Messages.INSTALL_LICENSE_KEY;
                    dialog.CheckFileExists = true;
                    dialog.CheckPathExists = true;
                    dialog.Filter = string.Format("{0} (*.xslic)|*.xslic|{1} (*.*)|*.*", Messages.XS_LICENSE_FILES, Messages.ALL_FILES);
                    dialog.ShowHelp = true;
                    dialog.HelpRequest += new EventHandler(dialog_HelpRequest);
                    result = dialog.ShowDialog(this);
                }
            }
            else
            {
                result = DialogResult.OK;
            }

            if (result == DialogResult.OK || Program.RunInAutomatedTestMode)
            {
                filepath = Program.RunInAutomatedTestMode ? "" : filepath == "" ? dialog.FileName : filepath;

                Host hostAncestor = SelectionManager.Selection.Count == 1 ? SelectionManager.Selection[0].HostAncestor : null;

                if (SelectionManager.Selection.Count == 1 && hostAncestor == null)
                {
                    SelectHostDialog hostdialog = new SelectHostDialog();
                    hostdialog.TheHost = null;
                    hostdialog.Owner = this;
                    hostdialog.ShowDialog(this);
                    if (string.IsNullOrEmpty(filepath) || hostdialog.DialogResult != DialogResult.OK)
                    {
                        return;
                    }
                    hostAncestor = hostdialog.TheHost;
                }

                DoLicenseAction(hostAncestor, filepath);

            }
        }

        private void DoLicenseAction(Host host, string filePath)
        {
            ApplyLicenseAction action = new ApplyLicenseAction(host.Connection, host, filePath);
            using (var actionProgress = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
            {
                actionProgress.Text = Messages.INSTALL_LICENSE_KEY;
                actionProgress.ShowDialog(this);
            }
        }

        private void dialog_HelpRequest(object sender, EventArgs e)
        {
            Help.HelpManager.Launch("LicenseKeyDialog");
        }

        private void TheTabControl_Deselected(object sender, TabControlEventArgs e)
        {
            TabPage t = e.TabPage;
            if (t == null)
                return;
            BaseTabPage tabPage = t.Controls.OfType<BaseTabPage>().FirstOrDefault();
            if (tabPage != null)
                tabPage.PageHidden();
        }

        /// <param name="e">
        /// If null, then we deduce the method was called by TreeView_AfterSelect
        /// and don't focus the VNC console. i.e. we only focus the VNC console if the user
        /// explicitly clicked on the console tab rather than arriving there by navigating
        /// in treeView.
        /// </param>
        private void TheTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IgnoreTabChanges)
                return;

            TabPage t = TheTabControl.SelectedTab;

            if (!SearchMode)
                History.NewHistoryItem(new XenModelObjectHistoryItem(SelectionManager.Selection.FirstAsXenObject, t));

            if (t == TabPageConsole)
            {
                if (SelectionManager.Selection.FirstIsRealVM)
                {
                    ConsolePanel.setCurrentSource((VM)SelectionManager.Selection.First);
                    UnpauseVNC(e != null && sender == TheTabControl);
                }
                else if (SelectionManager.Selection.FirstIs<Host>())
                {
                    ConsolePanel.setCurrentSource((Host)SelectionManager.Selection.First);
                    UnpauseVNC(e != null && sender == TheTabControl);
                }
                ConsolePanel.UpdateRDPResolution();
            }
            else if (t == TabPageCvmConsole)
            {
                if (SelectionManager.Selection.FirstIs<Host>())
                {
                    CvmConsolePanel.setCurrentSource((Host)SelectionManager.Selection.First);
                    UnpauseVNC(e != null && sender == TheTabControl);
                }
            }
            else
            {
                ConsolePanel.PauseAllViews();  
                CvmConsolePanel.PauseAllViews();
                
                // Start timer for closing the VNC connection after an interval (20 seconds)
                // when the console tab is not selected
                ConsolePanel.StartCloseVNCTimer(ConsolePanel.activeVNCView);
                CvmConsolePanel.StartCloseVNCTimer(CvmConsolePanel.activeVNCView);

                if (t == TabPageGeneral)
                {
                    GeneralPage.XenObject = SelectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageBallooning)
                {
                    BallooningPage.XenObject = SelectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageSR)
                {
                    SrStoragePage.SR = SelectionManager.Selection.First as SR;
                }
                else if (t == TabPageNetwork)
                {
                    NetworkPage.XenObject = SelectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageUSB)
                {
                    UsbPage.XenObject = SelectionManager.Selection.FirstAsXenObject as Host;
                }
                else if (t == TabPageNICs)
                {
                    NICPage.Host = SelectionManager.Selection.First as Host;
                }
                else if (t == TabPageStorage)
                {
                    VMStoragePage.VM = SelectionManager.Selection.First as VM;
                }
                else if (t == TabPagePeformance)
                {
                    PerformancePage.XenObject = SelectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageSearch && !SearchMode)
                {
                    var rootNode = SelectionManager.Selection.RootNode;
                    var rootNodeGrouping = rootNode == null ? null : rootNode.Tag as GroupingTag;
                    var search = rootNode == null ? null : rootNode.Tag as Search;

                    if (search != null)
                    {
                        SearchPage.Search = search;
                    }
                    else if (rootNodeGrouping != null)
                    {
                        var objectsView = rootNodeGrouping.Grouping as OrganizationViewObjects;
                        var vappsView = rootNodeGrouping.Grouping as OrganizationViewVapps;
                        var foldersView = rootNodeGrouping.Grouping as OrganizationViewFolders;

                        if (vappsView != null)
                        {
                            SearchPage.Search = Search.SearchForVappGroup(rootNodeGrouping.Grouping,
                                rootNodeGrouping.Parent, rootNodeGrouping.Group);
                        }
                        else if (objectsView != null)
                        {
                            //We are in Objects View
                            GroupingTag gt = null;

                            if (SelectionManager.Selection.Count == 1)
                            {
                                gt = SelectionManager.Selection.First as GroupingTag
                                    ?? SelectionManager.Selection[0].GroupAncestor;
                            }
                            else
                            {
                                //If multiple items have been selected we count the number of the grouping tags in the selection
                                var selectedGroups = SelectionManager.Selection.Where(s => s.GroupingTag != null);

                                //if exactly one grouping tag has been selected we show the search view for that one tag, but only if all the other items in the selection belong to this group/tag
                                if (selectedGroups.Count() == 1)
                                {
                                    var groupingTag = selectedGroups.First().GroupingTag;

                                    if (SelectionManager.Selection.Where(s => s.GroupingTag == null).All(s => s.GroupAncestor == groupingTag))
                                        gt = groupingTag;
                                    else
                                        gt = null;
                                }
                                else
                                {
                                    gt = SelectionManager.Selection.GroupAncestor;
                                }
                            }

                            //if there has been a grouping tag determined above we use that
                            //if not we show the search view for the root node
                            if (gt != null)
                            {
                                SearchPage.Search = Search.SearchForNonVappGroup(gt.Grouping, gt.Parent, gt.Group);
                            }
                            else
                            {
                                SearchPage.Search = Search.SearchForNonVappGroup(rootNodeGrouping.Grouping, rootNodeGrouping.Parent, rootNodeGrouping.Group);
                            }
                        }
                        else if (foldersView != null)
                        {
                            SearchPage.Search = Search.SearchForFolderGroup(rootNodeGrouping.Grouping,
                                  rootNodeGrouping.Parent, rootNodeGrouping.Group);
                        }
                        else
                        {
                            SearchPage.Search = Search.SearchForNonVappGroup(rootNodeGrouping.Grouping,
                                  rootNodeGrouping.Parent, rootNodeGrouping.Group);
                        }
                    }
                    else
                    {
                        // Infrastructure View:
                        // If XenCenter node or a  disconnected host is selected, show the default search
                        // Otherwise, find the top-level parent (= pool or standalone server) and show the search restricted to that
                        // In the case of multiselect, if all the selections are within one pool (or standalone server), then show that report.
                        // Otherwise show everything, as on the XenCenter node.
                        var connection = SelectionManager.Selection.GetConnectionOfAllItems(); // null for cross-pool selection
                        if (connection != null)
                        {
                            //If ShowJustHostInSearch is enabled and only one live host is selected, we show the search for the host only
                            if (Properties.Settings.Default.ShowJustHostInSearch && SelectionManager.Selection.Count == 1 
                                && SelectionManager.Selection.FirstIsLiveHost)
                            {
                                SearchPage.XenObject = SelectionManager.Selection.FirstAsXenObject;
                            }
                            else
                            {
                                var pool = Helpers.GetPool(connection);
                                SearchPage.XenObject = pool ?? (IXenObject)Helpers.GetMaster(connection); // pool or standalone server
                            }
                        }
                        else
                            SearchPage.XenObject = null;
                    }
                }
                else if (t == TabPageHA)
                {
                    HAPage.XenObject = SelectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageWLB)
                {
                    WlbPage.Pool = SelectionManager.Selection.First as Pool;
                }
                else if (t == TabPageSnapshots)
                {
                    snapshotPage.VM = SelectionManager.Selection.First as VM;
                }
                else if (t == TabPagePhysicalStorage)
                {
                    PhysicalStoragePage.SetSelectionBroadcaster(SelectionManager, this);
                    PhysicalStoragePage.Host = SelectionManager.Selection.First as Host;
                    PhysicalStoragePage.Connection = SelectionManager.Selection.GetConnectionOfFirstItem();
                }
                else if (t == TabPageAD)
                {
                    AdPage.XenObject = SelectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageGPU)
                {
                    GpuPage.XenObject = SelectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageDockerProcess)
                {
                    DockerProcessPage.DockerContainer = SelectionManager.Selection.First as DockerContainer;
                }
                else if (t == TabPageDockerDetails)
                {
                    DockerDetailsPage.DockerContainer = SelectionManager.Selection.First as DockerContainer;
                }
                else if (t == TabPagePvs)
                {
                    PvsPage.Connection = SelectionManager.Selection.GetConnectionOfFirstItem();
                }
            }

            if (t == TabPageSearch)
                SearchPage.PanelShown();
            else
                SearchPage.PanelHidden();

            if (t == TabPageDockerDetails)
                DockerDetailsPage.ResumeRefresh();
            else
                DockerDetailsPage.PauseRefresh();

            if (t == TabPageDockerProcess)
                DockerProcessPage.ResumeRefresh();
            else
                DockerProcessPage.PauseRefresh();

            if (t != null)
                SetLastSelectedPage(SelectionManager.Selection.First, t);
        }

        private void UnpauseVNC(bool focus)
        {
            ConsolePanel.UnpauseActiveView();
            CvmConsolePanel.UnpauseActiveView();

            if (focus)
            {
                ConsolePanel.FocusActiveView();
                CvmConsolePanel.FocusActiveView();
                ConsolePanel.SwitchIfRequired();
                CvmConsolePanel.SwitchIfRequired();
            }
        }

        /// <summary>
        /// The tabs that may be visible in the main GUI window. Used in SwitchToTab().
        /// </summary>
        public enum Tab
        {
            Overview, Home, Settings, Storage, Network, Console, CvmConsole, Performance, NICs, SR, DockerProcess, DockerDetails, USB
        }

        public void SwitchToTab(Tab tab)
        {
            switch (tab)
            {
                case Tab.Overview:
                    TheTabControl.SelectedTab = TabPageSearch;
                    break;
                case Tab.Home:
                    TheTabControl.SelectedTab = TabPageHome;
                    break;
                case Tab.Settings:
                    TheTabControl.SelectedTab = TabPageGeneral;
                    break;
                case Tab.Storage:
                    TheTabControl.SelectedTab = TabPageStorage;
                    break;
                case Tab.Network:
                    TheTabControl.SelectedTab = TabPageNetwork;
                    break;
                case Tab.Console:
                    TheTabControl.SelectedTab = TabPageConsole;
                    break;
                case Tab.CvmConsole:
                    TheTabControl.SelectedTab = TabPageCvmConsole;
                    break;
                case Tab.Performance:
                    TheTabControl.SelectedTab = TabPagePeformance;
                    break;
                case Tab.NICs:
                    TheTabControl.SelectedTab = TabPageNICs;
                    break;
                case Tab.SR:
                    TheTabControl.SelectedTab = TabPageSR;
                    break;
                case Tab.DockerProcess:
                    TheTabControl.SelectedTab = TabPageDockerProcess;
                    break;
                case Tab.DockerDetails:
                    TheTabControl.SelectedTab = TabPageDockerDetails;
                    break;
                case Tab.USB:
                    TheTabControl.SelectedTab = TabPageUSB;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void templatesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            templatesToolStripMenuItem1.Checked = !templatesToolStripMenuItem1.Checked;
            Properties.Settings.Default.DefaultTemplatesVisible = templatesToolStripMenuItem1.Checked;
            ViewSettingsChanged();
        }

        private void customTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customTemplatesToolStripMenuItem.Checked = !customTemplatesToolStripMenuItem.Checked;
            Properties.Settings.Default.UserTemplatesVisible = customTemplatesToolStripMenuItem.Checked;
            ViewSettingsChanged();
        }

        private void localStorageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            localStorageToolStripMenuItem.Checked = !localStorageToolStripMenuItem.Checked;
            Properties.Settings.Default.LocalSRsVisible = localStorageToolStripMenuItem.Checked;
            ViewSettingsChanged();
        }

        private void ShowHiddenObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHiddenObjectsToolStripMenuItem.Checked = !ShowHiddenObjectsToolStripMenuItem.Checked;
            Properties.Settings.Default.ShowHiddenVMs = ShowHiddenObjectsToolStripMenuItem.Checked;
            ViewSettingsChanged();
        }

        private void ViewSettingsChanged()
        {
            Settings.TrySaveSettings();
            navigationPane.UpdateSearch();
            RequestRefreshTreeView();
        }

        private void EditSelectedNodeInTreeView()
        {
            navigationPane.EditSelectedNode();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            bool currentTasks = false;
            foreach (ActionBase a in ConnectionsManager.History)
            {
                if (a is MeddlingAction || a.IsCompleted)
                    continue;

                currentTasks = true;
                break;
            }

            if (currentTasks)
            {
                e.Cancel = true;

                if (Program.RunInAutomatedTestMode ||
                    new Dialogs.WarningDialogs.CloseXenCenterWarningDialog().ShowDialog(this) == DialogResult.OK)
                {
                    this.Hide();

                    // Close all open forms
                    List<Form> forms = new List<Form>();
                    foreach (Form form in Application.OpenForms)
                    {
                        if (form != this)
                        {
                            forms.Add(form);
                        }
                    }
                    foreach (Form form in forms)
                    {
                        form.Close();
                    }

                    // Disconnect the named pipe
                    Program.DisconnectPipe();
                    foreach (ActionBase a in ConnectionsManager.History)
                    {
                        if(!Program.RunInAutomatedTestMode)
                        {
                            if (a is AsyncAction)
                            {
                                AsyncAction aa = (AsyncAction) a;
                                aa.PrepareForLogReloadAfterRestart();
                            }

                            if (!a.IsCompleted && a.CanCancel && !a.SafeToExit)
                                a.Cancel();
                        }
                        else
                        {
                            if (!a.IsCompleted && a.CanCancel)
                                a.Cancel();
                        }
                    }
                    ThreadPool.QueueUserWorkItem(CloseWhenActionsCanceled);
                }
                return;
            }

            // Disconnect the named pipe
            Program.DisconnectPipe();

            Properties.Settings.Default.WindowSize = this.Size;
            Properties.Settings.Default.WindowLocation = this.Location;
            try
            {
                Settings.SaveServerList();
                Properties.Settings.Default.Save();
            }
            catch (ConfigurationErrorsException ex)
            {
                using (var dlg = new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                       SystemIcons.Error,
                       string.Format(Messages.MESSAGEBOX_SAVE_CORRUPTED, Settings.GetUserConfigPath()),
                       Messages.MESSAGEBOX_SAVE_CORRUPTED_TITLE)))
                {
                    dlg.ShowDialog(this);
                }
                log.Error("Couldn't save settings");
                log.Error(ex, ex);
            }
            base.OnClosing(e);
        }

        private void sendCtrlAltDelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConsolePanel.SendCAD();
        }

        #region IMainWindowCommandInterface Members

        /// <summary>
        /// Closes all per-Connection and per-VM wizards for the given connection.
        /// </summary>
        /// <param name="connection"></param>
        public void CloseActiveWizards(IXenConnection connection)
        {
            Program.Invoke(Program.MainWindow, delegate
            {
                // Close and remove any active wizards for any VMs
                foreach (VM vm in connection.Cache.VMs)
                {
                    CloseActiveWizards(vm);
                }
                closeActivePoolWizards(connection);
            });
        }

        /// <summary>
        /// Closes all per-Connection wizards.
        /// </summary>
        /// <param name="connection"></param>
        private void closeActivePoolWizards(IXenConnection connection)
        {
            IList<Form> wizards;
            if (activePoolWizards.TryGetValue(connection, out wizards))
            {
                foreach (var wizard in wizards)
                {
                    if (!wizard.IsDisposed)
                    {
                        wizard.Close();
                    }
                }

                activePoolWizards.Remove(connection);
            }
        }

        /// <summary>
        /// Closes all per-XenObject wizards.
        /// </summary>
        /// <param name="obj"></param>
        public void CloseActiveWizards(IXenObject obj)
        {
            Program.Invoke(Program.MainWindow, delegate
            {
                Form wizard;
                if (activeXenModelObjectWizards.TryGetValue(obj, out wizard))
                {
                    if (!wizard.IsDisposed)
                    {
                        wizard.Close();
                    }
                    activeXenModelObjectWizards.Remove(obj);
                }
            });
        }

        /// <summary>
        /// Show the given wizard, and impose a one-wizard-per-XenObject limit.
        /// </summary>
        /// <param name="obj">The relevant VM</param>
        /// <param name="wizard">The new wizard to show</param>
        public void ShowPerXenModelObjectWizard(IXenObject obj, Form wizard)
        {
            CloseActiveWizards(obj);
            activeXenModelObjectWizards.Add(obj, wizard);
            wizard.Show(this);
        }

        /// <summary>
        /// Show the given wizard, and impose a one-wizard-per-connection limit.
        /// </summary>
        /// <param name="connection">The connection.  May be null, in which case the wizard
        /// is not addded to any dictionary.  This should happen iff this is the New Pool Wizard.</param>
        /// <param name="wizard">The new wizard to show. May not be null.</param>
        public void ShowPerConnectionWizard(IXenConnection connection, Form wizard)
        {
            if (connection != null)
            {

                if (activePoolWizards.ContainsKey(connection))
                {
                    var w = activePoolWizards[connection].Where(x => x.GetType() == wizard.GetType()).FirstOrDefault();
                    if (w != null && !w.IsDisposed)
                    {
                        if (w.WindowState == FormWindowState.Minimized)
                        {
                            w.WindowState = FormWindowState.Normal;
                        }
                        w.Focus();
                        return;
                    }
                    if (w != null && w.IsDisposed)
                        activePoolWizards[connection].Remove(w);

                }

                //closeActivePoolWizards(connection);
                if (activePoolWizards.ContainsKey(connection))
                    activePoolWizards[connection].Add(wizard);
                else
                    activePoolWizards.Add(connection, new List<Form>() { wizard });

            }

            if (!wizard.Disposing && !wizard.IsDisposed && !Program.Exiting)
            {
                wizard.Show(this);
            }
        }

        /// <summary>
        /// Shows a form of the specified type if it has already been created. If the form doesn't exist yet
        /// it is created first and then shown.
        /// </summary>
        /// <param name="type">The type of the form to be shown.</param>
        public void ShowForm(Type type)
        {
            ShowForm(type, null);
        }

        /// <summary>
        /// Shows a form of the specified type if it has already been created. If the form doesn't exist yet
        /// it is created first and then shown.
        /// </summary>
        /// <param name="type">The type of the form to be shown.</param>
        /// <param name="args">The arguments to pass to the form's consructor</param>
        public void ShowForm(Type type, object[] args)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == type)
                {
                    HelpersGUI.BringFormToFront(form);
                    return;
                }
            }

            Form newForm = (Form)Activator.CreateInstance(type, args);
            newForm.Show(this);
        }

        public Form Form
        {
            get { return this; }
        }

        public void Invoke(MethodInvoker method)
        {
            Program.Invoke(this, method);
        }

        public bool SelectObjectInTree(IXenObject xenObject)
        {
            return SelectObject(xenObject);
        }

        public Collection<IXenConnection> GetXenConnectionsCopy()
        {
            return new Collection<IXenConnection>(ConnectionsManager.XenConnectionsCopy);
        }

        public void SaveServerList()
        {
            Settings.SaveServerList();
        }

        public bool RunInAutomatedTestMode
        {
            get { return Program.RunInAutomatedTestMode; }
        }

        public void RemoveConnection(IXenConnection connection)
        {
            ConnectionsManager.ClearCacheAndRemoveConnection(connection);
        }

        public void PutSelectedNodeIntoEditMode()
        {
            EditSelectedNodeInTreeView();
        }


        public void TrySelectNewObjectInTree(Predicate<object> tagMatch, bool selectNode, bool expandNode, bool ensureNodeVisible)
        {
            TrySelectNewNode(tagMatch, selectNode, expandNode, ensureNodeVisible);
        }

        public void TrySelectNewObjectInTree(IXenConnection c, bool selectNode, bool expandNode, bool ensureNodeVisible)
        {
            TrySelectNewNode(c, selectNode, expandNode, ensureNodeVisible);
        }

        public bool MenuShortcutsEnabled
        {
            get { return _menuShortcuts; }
        }

        #endregion

        #region Help

        private string TabHelpID()
        {
            string modelObj = getSelectedXenModelObjectType();

            if (TheTabControl.SelectedTab == TabPageHome)
                return "TabPageHome" + modelObj;
            if (TheTabControl.SelectedTab == TabPageSearch)
                return "TabPageSearch" + modelObj;
            if (TheTabControl.SelectedTab == TabPageConsole)
                return "TabPageConsole" + modelObj;
            if (TheTabControl.SelectedTab == TabPageCvmConsole)
                return "TabPageCvmConsole" + modelObj;
            if (TheTabControl.SelectedTab == TabPageGeneral)
                return "TabPageSettings" + modelObj;
            if (TheTabControl.SelectedTab == TabPagePhysicalStorage || TheTabControl.SelectedTab == TabPageStorage || TheTabControl.SelectedTab == TabPageSR)
                return "TabPageStorage" + modelObj;
            if (TheTabControl.SelectedTab == TabPageNetwork)
                return "TabPageNetwork" + modelObj;
            if (TheTabControl.SelectedTab == TabPageNICs)
                return "TabPageNICs" + modelObj;
            if (TheTabControl.SelectedTab == TabPageWLB)
                return "TabPageWLB" + modelObj;
            if (TheTabControl.SelectedTab == TabPagePeformance)
                return "TabPagePerformance" + modelObj;
            if (TheTabControl.SelectedTab == TabPageHA)
                return "TabPageHA" + modelObj;
            if (TheTabControl.SelectedTab == TabPageHAUpsell)
                return "TabPageHAUpsell" + modelObj;
            if (TheTabControl.SelectedTab == TabPageSnapshots)
                return "TabPageSnapshots" + modelObj;
            if (TheTabControl.SelectedTab == TabPageBallooning)
                return "TabPageBallooning" + modelObj;
            if (TheTabControl.SelectedTab == TabPageAD)
                return "TabPageAD" + modelObj;
            if (TheTabControl.SelectedTab == TabPageADUpsell)
                return "TabPageADUpsell" + modelObj;
            if (TheTabControl.SelectedTab == TabPageBallooningUpsell)
                return "TabPageBallooningUpsell";
            if (TheTabControl.SelectedTab == TabPageWLBUpsell)
                return "TabPageWLBUpsell";
            if (TheTabControl.SelectedTab == TabPageGPU)
                return "TabPageGPU" + modelObj;
            if (TheTabControl.SelectedTab == TabPageDockerProcess)
                return "TabPageDockerProcess" + modelObj;
            if (TheTabControl.SelectedTab == TabPageDockerDetails)
                return "TabPageDockerDetails" + modelObj;
            if (TheTabControl.SelectedTab == TabPagePvs)
                return "TabPagePvs" + modelObj;
            if (TheTabControl.SelectedTab == TabPageUSB)
                return "TabPageUSB" + modelObj;
            return "TabPageUnknown";
        }

        private string getSelectedXenModelObjectType()
        {
            // for now, since there are few topics which depend on the selected object we shall just check the special cases
            // when more topic are added we can just return the ModelObjectName

            if (TheTabControl.SelectedTab == TabPageGeneral && SelectionManager.Selection.First is VM)
            {
                return "VM";
            }

            if (TheTabControl.SelectedTab == TabPagePhysicalStorage || TheTabControl.SelectedTab == TabPageStorage || TheTabControl.SelectedTab == TabPageSR)
            {
                if (SelectionManager.Selection.FirstIs<Pool>())
                    return "Pool";
                if (SelectionManager.Selection.FirstIs<Host>())
                    return "Server";
                if (SelectionManager.Selection.FirstIs<VM>())
                    return "VM";
                if (SelectionManager.Selection.FirstIs<SR>())
                    return "Storage";
            }

            if (TheTabControl.SelectedTab == TabPageNetwork)
            {
                if (SelectionManager.Selection.FirstIs<Host>())
                    return "Server";
                if (SelectionManager.Selection.FirstIs<VM>())
                    return "VM";
            }

            return "";
        }

        public void ShowHelpTOC()
        {
            ShowHelpTopic(null);
        }

        public void ShowHelpTopic(string topicID)
        {
            // Abandon all hope, ye who enter here: if you're ever tempted to directly invoke hh.exe, see first:
            // JAXC-43: Online help doesn't work if install XenCenter into the path that contains special characters.
            // hh.exe can't seem to cope with certain multi-byte characters in the path to the chm.
            // System.Windows.Forms.Help.ShowHelp() can cope with the special characters in the path, but has the
            // irritating behaviour that the launched help is always on top of the app window (CA-8863).
            // So we show the help 'on top' of an invisible dummy Form.
             
            using (var helpForm = new Form())
            {
                string chm = Path.Combine(Program.AssemblyDir, InvisibleMessages.MAINWINDOW_HELP_PATH);
                if (topicID == null)
                {
                    // Show TOC
                    System.Windows.Forms.Help.ShowHelp(helpForm, chm, HelpNavigator.TableOfContents);
                }
                else
                {
                    System.Windows.Forms.Help.ShowHelp(helpForm, chm, HelpNavigator.TopicId, topicID);
                }
            }
            // record help usage
            Properties.Settings.Default.HelpLastUsed = DateTime.UtcNow.ToString("u");
            Settings.TrySaveSettings();
        }

        public void MainWindow_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            // CA-28064. MessageBox hack to kill the hlpevent it passes to MainWindows.
            if (Program.MainWindow.ContainsFocus && _menuShortcuts)
                LaunchHelp();
        }

        private void helpTopicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHelpTOC();
        }

        private void helpContextMenuItem_Click(object sender, EventArgs e)
        {
            LaunchHelp();
        }

        private void LaunchHelp()
        {
            if (alertPage.Visible)
                Help.HelpManager.Launch("AlertSummaryDialog");
            else if (updatesPage.Visible)
                Help.HelpManager.Launch("ManageUpdatesDialog");
            else if (eventsPage.Visible)
                Help.HelpManager.Launch("EventsPane");
            else
            {
                if (TheTabControl.SelectedTab.Tag is TabPageFeature && ((TabPageFeature)TheTabControl.SelectedTab.Tag).HasHelp)
                    ((TabPageFeature)TheTabControl.SelectedTab.Tag).LaunchHelp();
                else
                    Help.HelpManager.Launch(TabHelpID());
            }
        }

        public bool HasHelp()
        {
            return Help.HelpManager.HasHelpFor(TabHelpID());
        }

        private void viewApplicationLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.ViewLogFiles();
        }

        #endregion

        /// <summary>
        /// Used to select the pool or standalone host node for the specified connection which is about to appear in the tree.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="selectNode">if set to <c>true</c> then the pool/standalone host node will be selected.</param>
        /// <param name="expandNode">if set to <c>true</c> then the pool/standalone host node will be expanded.</param>
        /// <param name="ensureNodeVisible">if set to <c>true</c> then the matched node will be made visible.</param>
        public void TrySelectNewNode(IXenConnection connection, bool selectNode, bool expandNode, bool ensureNodeVisible)
        {
            if (connection != null)
            {
                TrySelectNewNode(delegate(object o)
                {
                    if (o == null)
                    {
                        return false;
                    }
                    else if (o.Equals(Helpers.GetPool(connection)))
                    {
                        return true;
                    }
                    Host[] hosts = connection.Cache.Hosts;
                    return hosts.Length > 0 && o.Equals(hosts[0]);
                }, selectNode, expandNode, ensureNodeVisible);
            }
        }

        /// <summary>
        /// Used to select or expand a node which is about to appear in the tree. This is used so that new hosts, folders, pools
        /// etc. can be picked and then selected/expanded. 
        /// 
        /// It fires off a new thread and then repeatedly tries to select a node which matches the specified match
        /// delegate. It stops if it times out or is successful.
        /// </summary>
        /// <param name="tagMatch">A match for the tag of the node.</param>
        /// <param name="selectNode">if set to <c>true</c> then the matched node will be selected.</param>
        /// <param name="expandNode">if set to <c>true</c> then the matched node will be expanded.</param>
        /// <param name="ensureNodeVisible">if set to <c>true</c> then the matched node will be made visible.</param>
        public void TrySelectNewNode(Predicate<object> tagMatch, bool selectNode, bool expandNode, bool ensureNodeVisible)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                bool success = false;

                for (int i = 0; i < 20 && !success; i++)
                {
                    Program.Invoke(Program.MainWindow, delegate
                        {
                            success = navigationPane.TryToSelectNewNode(tagMatch, selectNode, expandNode, ensureNodeVisible);
                        });
                    Thread.Sleep(500);
                }
            });
        }

        /// <summary>
        /// Selects the specified object in the treeview.
        /// </summary>
        /// <param name="o">The object to be selected.</param>
        /// <returns>A value indicating whether selection was successful.</returns>
        public bool SelectObject(IXenObject o)
        {
            return navigationPane.SelectObject(o);
        }

        private void eventsPage_GoToXenObjectRequested(IXenObject obj)
        {
            navigationPane.SwitchToInfrastructureMode();
            navigationPane.SelectObject(obj);
        }

        private void Updates_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.Invoke(this, () =>
                {
                    int updatesCount = Updates.UpdateAlertsCount;
                    navigationPane.UpdateNotificationsButton(NotificationsSubMode.Updates, updatesCount);

                    if (updatesPage.Visible)
                    {
                        TitleLabel.Text = NotificationsSubModeItem.GetText(NotificationsSubMode.Updates, updatesCount);
                        TitleIcon.Image = NotificationsSubModeItem.GetImage(NotificationsSubMode.Updates, updatesCount);
                    }
                });
        }

        private void CloseWhenActionsCanceled(object o)
        {
            int i = 0;
            while (true)
            {
                if (i > 20)
                    Program.ForcedExiting = true;

                if (i > 40 || AllActionsCompleted())
                {
                    Program.Invoke(this, Application.Exit);
                    break;
                }

                i++;
                System.Threading.Thread.Sleep(500);
            }
        }

        private bool AllActionsCompleted()
        {
            foreach (ActionBase a in ConnectionsManager.History)
            {
                if (!a.IsCompleted)
                    return false;
            }
            return true;
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsDialog dialog = new OptionsDialog(pluginManager);
            dialog.ShowDialog(this);
        }

        internal void action_Completed(ActionBase sender)
        {
            if (Program.Exiting)
            {
                return;
            }

            RequestRefreshTreeView();
            
            //Update toolbars, since if an action has just completed, various
            //buttons may need to be re-enabled. Applies to:
            // HostAction
            // EnableHAAction
            // DisableHAAction
            Program.Invoke(this, UpdateToolbars);
        }

        internal void OpenGlobalImportWizard(string param)
        {
            HelpersGUI.BringFormToFront(this);
            Host hostAncestor = SelectionManager.Selection.Count == 1 ? SelectionManager.Selection[0].HostAncestor : null;
            new ImportWizard(SelectionManager.Selection.GetConnectionOfFirstItem(), hostAncestor, param, false).Show();
        }

        internal void InstallUpdate(string path)
        {
            var wizard = new PatchingWizard();
            wizard.Show(this);
            wizard.NextStep();
            wizard.AddFile(path);
        }

        #region XenSearch

        // SearchMode doesn't just mean we are looking at the Search tab.
        // It's set when we import a search from a file; or when we double-click
        // on a folder or tag name to search for it.
        private bool searchMode = false;
        public bool SearchMode
        {
            get
            {
                return searchMode;
            }
            set
            {
                if (searchMode == value)
                    return;

                searchMode = value;
                navigationPane.InSearchMode = value;
                UpdateToolbarsCore();
            }
        }

        public bool DoSearch(string filename)
        {
            List<Search> searches = Search.LoadFile(filename);
            if (searches != null && searches.Count > 0)
            {
                Program.Invoke(Program.MainWindow, delegate()
                {
                    DoSearch(searches[0]);
                });
                return true;
            }
            return false;
        }

        public void DoSearch(Search search)
        {
            History.NewHistoryItem(new SearchHistoryItem(search));

            SearchMode = true;
            SearchPage.Search = search;

            UpdateHeader();
        }

        public void SearchForTag(string tag)
        {
            DoSearch(Search.SearchForTag(tag));
        }

        public void SearchForFolder(string path)
        {
            DoSearch(Search.SearchForFolder(path));
        }

        void SearchPanel_SearchChanged()
        {
            if (SearchMode)
                History.ReplaceHistoryItem(new SearchHistoryItem(SearchPage.Search));
            else
                History.ReplaceHistoryItem(new ModifiedSearchHistoryItem(
                    SelectionManager.Selection.FirstAsXenObject, SearchPage.Search));
        }

        /// <summary>
        /// Updates the shiny gradient bar with selected object name and icon.
        /// Also updates 'Logged in as:'.
        /// </summary>
        private void UpdateHeader()
        {
            bool licenseStatusTitleLabelHandled = false;

            if (navigationPane.currentMode == NavigationPane.NavigationMode.Notifications)
                return;

            if (SearchMode && SearchPage.Search != null)
            {
                TitleLabel.Text = HelpersGUI.GetLocalizedSearchName(SearchPage.Search);
                TitleIcon.Image = Images.GetImage16For(SearchPage.Search);
            }
            else if (!SearchMode && SelectionManager.Selection.ContainsOneItemOfType<IXenObject>())
            {
                IXenObject xenObject = SelectionManager.Selection[0].XenObject;
                TitleLabel.Text = xenObject.NameWithLocation();

                UpdateLicenseStatusTitleLabel(xenObject);
                licenseStatusTitleLabelHandled = true;

                TitleIcon.Image = Images.GetImage16For(xenObject);
                // When in folder view only show the logged in label if it is clear to which connection the object belongs (most likely pools and hosts)

                if (SelectionManager.Selection[0].PoolAncestor == null && SelectionManager.Selection[0].HostAncestor == null)
                    loggedInLabel1.Connection = null;
                else
                    loggedInLabel1.Connection = xenObject.Connection;
            }
            else
            {
                TitleLabel.Text = Messages.XENCENTER;
                TitleIcon.Image = Properties.Resources.Logo;
                loggedInLabel1.Connection = null;
            }

            if (!licenseStatusTitleLabelHandled)
                ResetLicenseStatusTitleLabel();

            SetTitleLabelMaxWidth();
        }

        private void UpdateLicenseStatusTitleLabel(IXenObject xenObject)
        {
            if (xenObject is Pool)
            {
                var pool = xenObject as Pool;

                if (pool.Connection != null && pool.Connection.CacheIsPopulated)
                {
                    if (pool.IsExpired)
                    {
                        LicenseStatusTitleLabel.Text = Messages.MAINWINDOW_HEADER_UNLICENSED;
                        LicenseStatusTitleLabel.ForeColor = Color.Red;
                    }
                    else
                    {
                        LicenseStatusTitleLabel.Text = string.Format(Messages.MAINWINDOW_HEADER_LICENSED_WITH, pool.LicenseString());
                        LicenseStatusTitleLabel.ForeColor = Program.TitleBarForeColor;
                    }
                }
            }
            else if (xenObject is Host)
            {
                var host = xenObject as Host;

                if (host.Connection != null && host.Connection.CacheIsPopulated)
                {
                    if (host.IsExpired())
                    {
                        LicenseStatusTitleLabel.Text = Messages.MAINWINDOW_HEADER_UNLICENSED;
                        LicenseStatusTitleLabel.ForeColor = Color.Red;
                    }
                    else
                    {
                        LicenseStatusTitleLabel.Text = string.Format(Messages.MAINWINDOW_HEADER_LICENSED_WITH, Helpers.GetFriendlyLicenseName(host));
                        LicenseStatusTitleLabel.ForeColor = Program.TitleBarForeColor;
                    }
                }
            }
            else
            {
                ResetLicenseStatusTitleLabel();
            }
        }

        private void ResetLicenseStatusTitleLabel()
        {
            if (string.IsNullOrEmpty(LicenseStatusTitleLabel.Text))
                return;

            LicenseStatusTitleLabel.Text = string.Empty;
        }

        private void SetTitleLabelMaxWidth()
        {
            TitleLabel.MaximumSize = new Size(tableLayoutPanel1.Width - loggedInLabel1.Width - LicenseStatusTitleLabel.Width - 6, TitleLabel.Height);
        }

        private void UpdateViewMenu(NavigationPane.NavigationMode mode)
        {
            //the order is the reverse from the order in which we want them to appear
            var items = new ToolStripItem []
                {
                    toolStripSeparator24,
                    ShowHiddenObjectsToolStripMenuItem,
                    localStorageToolStripMenuItem,
                    templatesToolStripMenuItem1,
                    customTemplatesToolStripMenuItem
                };

            if (mode == NavigationPane.NavigationMode.Infrastructure)
            {
                foreach (var item in items)
                {
                    if (!viewToolStripMenuItem.DropDownItems.Contains(item))
                        viewToolStripMenuItem.DropDownItems.Insert(0, item);
                }
            }
            else if (mode == NavigationPane.NavigationMode.Notifications)
            {
                 foreach (var item in items)
                    viewToolStripMenuItem.DropDownItems.Remove(item);
            }
            else
            {
                for (int i = 2; i < items.Length; i++)
                    viewToolStripMenuItem.DropDownItems.Remove(items[i]);

                for (int i = 0; i < 2; i++)
                    if (!viewToolStripMenuItem.DropDownItems.Contains(items[i]))
                        viewToolStripMenuItem.DropDownItems.Insert(0, items[i]);
            }

            pluginMenuItemStartIndexes[viewToolStripMenuItem] = viewToolStripMenuItem.DropDownItems.IndexOf(toolStripSeparator24) + 1;
        }

        void navigationPane_DragDropCommandActivated(string cmdText)
        {
            SetStatusBar(null, cmdText);
        }

        private void navigationPane_TreeViewSelectionChanged()
        {
            UpdateToolbars();

            //
            // NB do not trigger updates to the panels in this method
            // instead, put them in TheTabControl_SelectedIndexChanged,
            // so only the selected tab is updated
            //

            TheTabControl_SelectedIndexChanged(null, EventArgs.Empty);

            if (TheTabControl.SelectedTab != null)
                TheTabControl.SelectedTab.Refresh();

            UpdateHeader();
        }

        private void navigationPane_NotificationsSubModeChanged(NotificationsSubModeItem submodeItem)
        {
            alertPage.Visible = submodeItem.SubMode == NotificationsSubMode.Alerts;
            updatesPage.Visible = submodeItem.SubMode == NotificationsSubMode.Updates;
            eventsPage.Visible = submodeItem.SubMode == NotificationsSubMode.Events;
            TheTabControl.Visible = false;

            if (alertPage.Visible)
                alertPage.RefreshAlertList();

            if (updatesPage.Visible)
                updatesPage.RefreshUpdateList();

            if (eventsPage.Visible)
            {
                eventsPage.RefreshDisplayedEvents();
            }

            loggedInLabel1.Connection = null;
            TitleLabel.Text = submodeItem.Text;
            TitleIcon.Image = submodeItem.Image;
        }

        private void navigationPane_NavigationModeChanged(NavigationPane.NavigationMode mode)
        {
            if (mode == NavigationPane.NavigationMode.Notifications)
            {
                ResetLicenseStatusTitleLabel();
                TheTabControl.Visible = false;
            }
            else
            {
                bool tabControlWasVisible = TheTabControl.Visible;
                TheTabControl.Visible = true;
                alertPage.Visible = updatesPage.Visible = eventsPage.Visible = false;

                // force an update of the selected tab when switching back from Notification view, 
                // as some tabs ignore the update events when not visible (e.g. Snapshots, HA)
                if (!tabControlWasVisible)
                    TheTabControl_SelectedIndexChanged(null, null);
            }

            UpdateViewMenu(mode);
        }

        private void navigationPane_TreeNodeBeforeSelected()
        {
            SearchMode = false;
        }

        private void navigationPane_TreeNodeClicked()
        {
            if (SearchMode)
            {
                SearchMode = false;
                TheTabControl_SelectedIndexChanged(null, null);
                UpdateHeader();
            }
        }

        private void navigationPane_TreeNodeRightClicked()
        {
           MainMenuBar_MenuActivate(MainMenuBar, new EventArgs());
        }

        private void navigationPane_TreeViewRefreshed()
        {
            UpdateHeaderAndTabPages();
        }

        private void navigationPane_TreeViewRefreshResumed()
        {
            ignoreUpdateToolbars--;
            if (ignoreUpdateToolbars == 0 && calledUpdateToolbars)
                UpdateToolbars();
        }

        private void navigationPane_TreeViewRefreshSuspended()
        {
            if (ignoreUpdateToolbars == 0)
                calledUpdateToolbars = false;
            ignoreUpdateToolbars++;
        }

        #endregion

        void XenCenterAlerts_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.BeginInvoke(Program.MainWindow, () =>
                {
                    navigationPane.UpdateNotificationsButton(
                        NotificationsSubMode.Alerts, Alert.NonDismissingAlertCount);

                    if (alertPage.Visible)
                    {
                        TitleLabel.Text = NotificationsSubModeItem.GetText(NotificationsSubMode.Alerts, Alert.NonDismissingAlertCount);
                        TitleIcon.Image = NotificationsSubModeItem.GetImage(NotificationsSubMode.Alerts, Alert.NonDismissingAlertCount);
                    }
                });
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            History.Back(1);
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            History.Forward(1);
        }

        private void backButton_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripSplitButton button = sender as ToolStripSplitButton;
            if (button == null)
                return;

            History.PopulateBackDropDown(button);
        }

        private void forwardButton_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripSplitButton button = sender as ToolStripSplitButton;
            if (button == null)
                return;

            History.PopulateForwardDropDown(button);
        }


        private void LicenseManagerMenuItem_Click(object sender, EventArgs e)
        {
            licenseManagerLauncher.LaunchIfRequired(false, ConnectionsManager.XenConnections, SelectionManager.Selection);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                RequestRefreshTreeView();
                if (TheTabControl.SelectedTab == TabPageSearch)
                    SearchPage.PanelProd();
            }
        }

        private void ShowToolbarMenuItem_Click(object sender, EventArgs e)
        {
            ToolbarsEnabled = !ToolbarsEnabled;
            Properties.Settings.Default.ToolbarsEnabled = ToolbarsEnabled;
            UpdateToolbars();
        }

        private void MainMenuBar_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ToolBarContextMenu.Show(Program.MainWindow, e.Location);
            }
        }

        /// <summary>
        /// Equivalent to MainController.Confirm(conn, this, msg, args).
        /// </summary>
        public bool Confirm(IXenConnection conn, string title, string msg, params object[] args)
        {
            return Confirm(conn, this, title, msg, args);
        }

        /// <summary>
        /// Show a MessageBox asking to confirm an operation. The MessageBox will be parented to the given Control.
        /// Displays default "Yes" and "No" buttons ("Yes" button is selected by default).
        /// The args given will be ellipsised to Helpers.DEFAULT_NAME_TRIM_LENGTH, if they are strings.
        /// If in automated test mode, then always returns true.
        /// If the user refuses the operation, then returns false.
        /// If the given connection has disconnected in the time it takes the user to confirm,
        /// then shows an information MessageBox, and returns false.
        /// Otherwise, the user has agreed and the connection is still alive, so
        /// sets MainWindow.AllowHistorySwitch to true and returns true.
        /// </summary>
        public static bool Confirm(IXenConnection conn, Control parent, string title, string msg, params object[] args)
        {
            return Confirm(conn, parent, title, null, null, null, msg, args);
        }

        /// <summary>
        /// Show a MessageBox asking to confirm an operation. The MessageBox will be parented to the given Control.
        /// "Yes" and "No" buttons can be customized.
        /// The args given will be ellipsised to Helpers.DEFAULT_NAME_TRIM_LENGTH, if they are strings.
        /// If in automated test mode, then always returns true.
        /// If the user refuses the operation, then returns false.
        /// If the given connection has disconnected in the time it takes the user to confirm,
        /// then shows an information MessageBox, and returns false.
        /// Otherwise, the user has agreed and the connection is still alive, so
        /// sets MainWindow.AllowHistorySwitch to true and returns true.
        /// </summary>
        public static bool Confirm(IXenConnection conn, Control parent, string title,
            string helpName, ThreeButtonDialog.TBDButton buttonYes, ThreeButtonDialog.TBDButton buttonNo,
            string msg, params object[] args)
        {
            if (Program.RunInAutomatedTestMode)
                return true;

            Trim(args);

            var buttons = new[]
                {
                    buttonYes ?? ThreeButtonDialog.ButtonYes,
                    buttonNo ?? ThreeButtonDialog.ButtonNo
                };

            var details = new ThreeButtonDialog.Details(SystemIcons.Exclamation,
                args.Length == 0 ? msg : string.Format(msg, args), title);

            DialogResult dialogResult;
            using (var dialog = String.IsNullOrEmpty(helpName)
                             ? new ThreeButtonDialog(details, buttons)
                             : new ThreeButtonDialog(details, helpName, buttons))
            {
                dialogResult = dialog.ShowDialog(parent ?? Program.MainWindow);
            }

            if (dialogResult != DialogResult.Yes)
                return false;


            if (conn != null && !conn.IsConnected)
            {
                ShowDisconnectedMessage(parent);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Show a message telling the user that the connection has disappeared.  We check this after
        /// we've shown a dialog, in case it's happened in the time it took them to click OK.
        /// </summary>
        public static void ShowDisconnectedMessage(Control parent)
        {
            // We could have done some teardown by now, so we need to be paranoid about things going away
            // beneath us.
            if (Program.Exiting)
                return;
            if (parent == null || parent.Disposing || parent.IsDisposed)
            {
                parent = Program.MainWindow;
                if (parent.Disposing || parent.IsDisposed)
                    return;
            }
            using (var dlg = new ThreeButtonDialog(
               new ThreeButtonDialog.Details(
                   SystemIcons.Warning,
                   Messages.DISCONNECTED_BEFORE_ACTION_STARTED,
                   Messages.XENCENTER)))
            {
                dlg.ShowDialog(parent);
            }
        }

        private static void Trim(object[] args)
        {
            int n = args.Length;
            for (int i = 0; i < n; i++)
            {
                if (args[i] is string)
                    args[i] = ((string)args[i]).Ellipsise(Helpers.DEFAULT_NAME_TRIM_LENGTH);
            }
        }

        #region ISynchronizeInvoke Members

        // this explicit implementation of ISynchronizeInvoke is used to allow the model to update 
        // its API on the main program thread while being decoupled from MainWindow.

        IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
        {
            return Program.BeginInvoke(this, method, args);
        }

        object ISynchronizeInvoke.EndInvoke(IAsyncResult result)
        {
            return EndInvoke(result);
        }

        object ISynchronizeInvoke.Invoke(Delegate method, object[] args)
        {
            return Program.Invoke(this, method, args);
        }

        bool ISynchronizeInvoke.InvokeRequired
        {
            get { return InvokeRequired; }
        }

        #endregion

        private void importSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = Messages.XENCENTER_CONFIG_FILTER;
                if (dialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    try
                    {
                        log.InfoFormat("Importing server list from '{0}'", dialog.FileName);

                        XmlDocument xmlDocument = new XmlDocument();
                        using (var stream = dialog.OpenFile())
                            xmlDocument.Load(stream);
                        
                        foreach (XmlNode itemConnection in xmlDocument.GetElementsByTagName("XenConnection"))
                        {
                            var conn = new XenConnection();
                            foreach (XmlNode item in itemConnection.ChildNodes)
                            {

                                switch (item.Name)
                                {
                                    case "Hostname":
                                        conn.Hostname = item.InnerText;
                                        break;
                                    case "Port":
                                        conn.Port = int.Parse(item.InnerText);
                                        break;
                                    case "FriendlyName":
                                        conn.FriendlyName = item.InnerText;
                                        break;
                                }
                            }
                            if (null == ConnectionsManager.XenConnections.Find(existing => (existing.Hostname == conn.Hostname && existing.Port == conn.Port)))
                                ConnectionsManager.XenConnections.Add(conn);
                            RequestRefreshTreeView();
                        }

                        log.InfoFormat("Imported server list from '{0}' successfully.", dialog.FileName);
                    }
                    catch (XmlException)
                    {
                        log.ErrorFormat("Failed to import server list from '{0}'", dialog.FileName);

                        using (var dlg = new ThreeButtonDialog(
                                  new ThreeButtonDialog.Details(SystemIcons.Error, Messages.ERRO_IMPORTING_SERVER_LIST, Messages.XENCENTER)))
                        {
                            dlg.ShowDialog(this);
                        }
                    }
                }
            }
        }

        private void exportSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = Messages.XENCENTER_CONFIG_FILTER;
                dialog.Title = Messages.ACTION_SAVE_CHANGES_IN_PROGRESS;
                dialog.CheckPathExists = true;
                if (dialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    log.InfoFormat("Exporting server list to '{0}'", dialog.FileName);

                    try
                    {
                        using (var xmlWriter = new XmlTextWriter(dialog.OpenFile(), Encoding.Unicode))
                        {
                            xmlWriter.WriteStartDocument();
                            xmlWriter.WriteStartElement("XenConnections");
                            xmlWriter.WriteWhitespace("\n");

                            foreach (var connection in ConnectionsManager.XenConnections)
                            {
                                xmlWriter.WriteStartElement("XenConnection");
                                {
                                    xmlWriter.WriteElementString("Hostname", connection.Hostname);
                                    xmlWriter.WriteElementString("Port", connection.Port.ToString());
                                    xmlWriter.WriteWhitespace("\n  ");
                                    xmlWriter.WriteElementString("FriendlyName", connection.FriendlyName);
                                }
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteWhitespace("\n");
                            }

                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteEndDocument();
                        }

                        log.InfoFormat("Exported server list to '{0}' successfully.", dialog.FileName);
                    }
                    catch
                    {
                        log.ErrorFormat("Failed to export server list to '{0}'.", dialog.FileName);
                        throw;
                    }
                }
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            SetSplitterDistance();
        }

        FormWindowState lastState = FormWindowState.Normal;
        private void MainWindow_Resize(object sender, EventArgs e)
        {
            TabPage t = TheTabControl.SelectedTab;
            if (t == TabPageConsole)
            {
                if (WindowState != lastState && WindowState != FormWindowState.Minimized)
                {
                    lastState = WindowState;
                    ConsolePanel.UpdateRDPResolution();
                }
                mainWindowResized = true;
            }
            SetSplitterDistance();
            SetTitleLabelMaxWidth();
        }

        private void SetSplitterDistance()
        {
            //CA-71697: chosen min size so the tab contents are visible
            int chosenPanel2MinSize = MinimumSize.Width * 3 / 5;
            int min = splitContainer1.Panel1MinSize;
            int max = splitContainer1.Width - chosenPanel2MinSize;

            if (max < min)
                return;

            splitContainer1.Panel2MinSize = chosenPanel2MinSize;

            if (splitContainer1.SplitterDistance < min)
                splitContainer1.SplitterDistance = min;
            else if (splitContainer1.SplitterDistance > max)
                splitContainer1.SplitterDistance = max;
        }
      
        private void MainWindow_ResizeEnd(object sender, EventArgs e)
        {
            TabPage t = TheTabControl.SelectedTab;
            if (t == TabPageConsole) 
            {
                if (mainWindowResized)
                    ConsolePanel.UpdateRDPResolution();
                mainWindowResized = false;
            }
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            TabPage t = TheTabControl.SelectedTab;
            if (t == TabPageConsole)
                ConsolePanel.UpdateRDPResolution();

            SetTitleLabelMaxWidth();
        }
    }
}
