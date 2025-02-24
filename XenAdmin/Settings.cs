/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.RestoreSession;
using XenAdmin.Network;
using XenAPI;
using XenCenterLib;


namespace XenAdmin
{
    public static class Settings
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Module for authenticating with proxy server using the Basic authentication scheme.
        /// </summary>
        private static IAuthenticationModule BasicAuthenticationModule;
        /// <summary>
        /// Module for authenticating with proxy server using the Digest authentication scheme.
        /// </summary>
        private static IAuthenticationModule DigestAuthenticationModule;

        /// <summary>
        /// Used in place of the username, to indicate that a record is for a VM's VNC connection, rather
        /// than a connection to a server.
        /// </summary>
        private const string VNC_INDICATOR = "$VNC$";

        private const char SEPARATOR = '\x202f'; // narrow non-breaking space.
        /// <summary>
        /// Anybody who chooses this for their password deserves a crash:
        /// MODIFIER LETTER REVERSED GLOTTAL STOP, NKO SYMBOL GBAKURUNEN, CHAM PUNCTUATION DOUBLE DANDA, BOPOMOFO LETTER INNN
        /// </summary>
        private const string MARKER_VALUE = "\x02c1\x07f7\xaa5e\x31b3";
        private const string DISCONNECTED = "disconnected";
        private const string CONNECTED = "connected";

        /// <summary>
        /// vm_uuid -> password.  The password may be null if it's been deleted.
        /// </summary>
        private static Dictionary<string, string> VNCPasswords = new Dictionary<string, string>();


        static Settings()
        {
            // Store the Basic and Digest authentication modules, used for proxy server authentication, 
            // for later use; this is needed because we cannot create new instances of them and it 
            // saves us needing to create our own custom authentication modules.

            var authModules = AuthenticationManager.RegisteredModules;
            while (authModules.MoveNext())
            {
                if (!(authModules.Current is IAuthenticationModule module))
                    continue;

                if (module.AuthenticationType == "Basic")
                    BasicAuthenticationModule = module;
                else if (module.AuthenticationType == "Digest")
                    DigestAuthenticationModule = module;
            }
        }

        /// <summary>
        /// MSDN info regarding the path to the user.config file is somewhat confusing.
        /// It turns out it is not in Application.UserAppDataPath as stated on
        /// http://msdn.microsoft.com/en-us/library/8eyb2ct1.aspx. As described on
        /// http://stackoverflow.com/questions/1075204/when-using-a-settings-settings-file-in-net-where-is-the-config-actually-stored
        /// and on http://msdn.microsoft.com/en-us/library/ms379611.aspx, it is
        /// ProfileDirectory\CompanyName\AppName_EvidenceType_EvidenceHash\Version\user.config 
        /// where
        /// - ProfileDirectory: either the roaming profile directory or the local one.
        ///   Settings are stored by default in the local user.config file. To store a
        ///   setting in the roaming user.config file, you need to mark the setting with
        ///   the SettingsManageabilityAttribute with SettingsManageability set to Roaming.
        /// - CompanyName: typically the string specified by the AssemblyCompanyAttribute
        ///   (with the caveat that the string is escaped and truncated as necessary, and if
        ///   not specified on the assembly, we have a fallback procedure).
        /// - AppName: typically the string specified by the AssemblyProductAttribute
        ///   (same caveats as for company name).
        /// - EvidenceType and EvidenceHash: information derived from the app domain evidence
        ///   to provide proper app domain and assembly isolation.
        /// - Version: typically the version specified in the AssemblyVersionAttribute.
        ///   This is required to isolate different versions of the app deployed side by side.
        /// - The file name is always simply 'user.config
        /// Trying to retrieve the filename at the places where this method's caller caught
        /// the ConfigurationErrorsException returns null, so this one has to be called.
        /// </summary>
        public static string GetUserConfigPath()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);
                return config.FilePath;
            }
            catch (ConfigurationErrorsException exc)
            {
                return exc.Filename;
            }
        }

        public static void RestoreSession()
        {
            if (Program.RunInAutomatedTestMode)
            {
                Program.SkipSessionSave = true;
                Properties.Settings.Default.SaveSession = false;
                return;
            }

            if (!Registry.AllowCredentialSave)
            {
                Program.SkipSessionSave = true;
                Properties.Settings.Default.SaveSession = false;
                Properties.Settings.Default.RequirePass = false;
                RestoreSessionWithPassword(null, false);
                return;
            }

            Program.MainPassword = null;

            if (Properties.Settings.Default.SaveSession || Properties.Settings.Default.RequirePass)
            {
                if (Properties.Settings.Default.ServerList != null && Properties.Settings.Default.ServerList.Length > 0 ||
                    Properties.Settings.Default.ServerAddressList != null && Properties.Settings.Default.ServerAddressList.Length > 0)
                {
                    if (!Properties.Settings.Default.RequirePass)
                    {
                        Program.SkipSessionSave = true;
                        RestoreSessionWithPassword(null, true);
                        return;
                    }

                    Program.MainWindow.CloseSplashScreen();

                    string password = null;
                    do
                    {
                        using (var dialog = new LoadSessionDialog(password != null))
                            password = dialog.ShowDialog(Program.MainWindow) == DialogResult.OK
                                ? dialog.Password
                                : null;
                    } while (password != null && !RestoreSessionWithPassword(password, true));

                    Properties.Settings.Default.SaveSession = password != null;
                    Properties.Settings.Default.RequirePass = true;
                    Program.SkipSessionSave = true;

                    if (password == null)
                        RestoreSessionWithPassword(null, false); //if the user has cancelled start a new session
                    else
                        Program.MainPassword = EncryptionUtils.ComputeHash(password);
                }
                else
                {
                    Properties.Settings.Default.RequirePass = false;
                    Properties.Settings.Default.SaveSession = false;
                }
            }
            else
            {
                Program.SkipSessionSave = true;
                RestoreSessionWithPassword(null, false);
            }
        }

        /// <summary>
        /// Tries to restore the session list using the given password as the key.
        /// Returns true if successful, false otherwise (usually due to
        /// a decryption failure, in turn due to a wrong password).
        /// </summary>
        private static bool RestoreSessionWithPassword(string password, bool useOriginalList)
        {
            string[] encServerList;

            if (useOriginalList)
            {
                // if we are resuming without a password or have a valid password use Settings.ServerList
                encServerList = Properties.Settings.Default.ServerList ?? new string[0];
            }
            else
            {
                // user has cancelled, use the ServerAddressList (no usernames or passwords)
                encServerList = Properties.Settings.Default.ServerAddressList ?? new string[0];
            }

            string[] decryptedList = new string[encServerList.Length];
            if (!Properties.Settings.Default.RequirePass || !useOriginalList)
            {
                int idx = 0;
                try
                {
                    foreach (string encEntry in encServerList)
                    {
                        decryptedList[idx] = EncryptionUtils.Unprotect(encEntry);
                        idx++;
                    }
                }
                catch (Exception exp)
                {
                    log.Warn("Could not unprotect session information", exp);
                    return false;
                }
            }
            else
            {
                int idx = 0;
                try
                {
                    foreach (string encEntry in encServerList)
                    {
                        decryptedList[idx] = EncryptionUtils.DecryptString(encEntry, password);
                        idx++;
                    }
                }
                catch (Exception exp)
                {
                    log.Warn("Could not decrypt session information", exp);
                    // Problem decrypting -> wrong password entered...
                    return false;
                }
            }

            // Validate the decrypted entries - the session entries may 
            // have been decrypted to nonsense...
            foreach (string entry in decryptedList)
            {
                string[] entryComps = entry.Split(SEPARATOR);
                if (entryComps.Length < 3 || entryComps.Length > 7)
                {
                    log.Warn("Did not decrypt session list to a valid entry...");
                    return false;
                }
            }

            foreach (string entry in decryptedList)
            {
                // $VNC$, VM UUID, unused, password
                // username, hostname, port, password
                // username, hostname, port, password, connected
                // username, hostname, port, password, connected, friendly name
                // username, hostname, port, password, connected, friendly name, pool members

                // If the user cancels the restore dialog, we use the ServerAddressList instead:
                // hostname, port, friendly name

                string[] entryComps = entry.Split(SEPARATOR);

                int port;
                if (!int.TryParse(entryComps[2], out port))
                {
                    port = ConnectionsManager.DEFAULT_XEN_PORT;
                }

                if (entryComps[0] == VNC_INDICATOR)
                {
                    string vm_uuid = entryComps[1];
                    VNCPasswords[vm_uuid] = entryComps[3];
                }
                else if (entryComps.Length == 3)
                {
                    if (!int.TryParse(entryComps[1], out port))
                    {
                        port = ConnectionsManager.DEFAULT_XEN_PORT;
                    }
                    IXenConnection connection = new XenConnection();
                    connection.Hostname = entryComps[0];
                    connection.Port = port;
                    connection.Password = null;
                    connection.SaveDisconnected = true;
                    connection.FriendlyName = entryComps[2] != "" ? entryComps[2] : entryComps[0];

                    AddConnection(connection);
                }
                else
                {
                    IXenConnection connection = new XenConnection();
                    connection.Username = entryComps[0];
                    connection.Hostname = entryComps[1];
                    connection.Port = port;
                    // If password is MARKER_VALUE, this indicates we didn't save a password for this connection
                    if (entryComps[3] == MARKER_VALUE)
                    {
                        connection.Password = null;
                        connection.ExpectPasswordIsCorrect = false;
                    }
                    else
                    {
                        connection.Password = entryComps[3];
                    }
                    connection.SaveDisconnected = entryComps.Length > 4 && entryComps[4] == DISCONNECTED;
                    connection.FriendlyName = entryComps.Length > 5 ? entryComps[5] : entryComps[1];

                    // We save a comma-separated list of hostnames of each of the supporters.
                    // This enables us to connect to a former supporter in the event of coordinator failover while the GUI isn't running.
                    if (entryComps.Length == 7 && entryComps[6] != "")
                    {
                        connection.PoolMembers = new List<string>(entryComps[6].Split(new char[] { ',' }));
                    }
                    else
                    {
                        connection.PoolMembers = new List<string>(new string[] { connection.Hostname });
                    }

                    AddConnection(connection);
                }
            }

            return true;
        }

        public static void ConfigureExternalSshClientSettings()
        {
            var customSshClient = Properties.Settings.Default.CustomSshConsole;
            var puttyLocation = Properties.Settings.Default.PuttyLocation;
            var openSshLocation = Properties.Settings.Default.OpenSSHLocation;

            if (string.IsNullOrEmpty(puttyLocation) && customSshClient == SshConsole.Putty ||
                string.IsNullOrEmpty(openSshLocation) && customSshClient == SshConsole.OpenSSH)
            {
                customSshClient = SshConsole.None;
            }

            // attempt to locate clients in their default locations
            if (string.IsNullOrEmpty(puttyLocation))
            {
                var defaultPaths = new[] {
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "PuTTY\\putty.exe"),
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "PuTTY\\putty.exe")
                };
                puttyLocation = defaultPaths.Where(File.Exists).FirstOrDefault();
            }
            if (string.IsNullOrEmpty(openSshLocation))
            {
                // https://docs.microsoft.com/en-us/windows-server/administration/openssh/openssh_server_configuration
                var defaultPath = Path.Combine(Environment.SystemDirectory, "OpenSSH\\ssh.exe");
                openSshLocation = File.Exists(defaultPath) ? defaultPath : openSshLocation;
            }

            // we prioritize PuTTY since that must have been installed by the user
            if (customSshClient == SshConsole.None)
            {
                if (!string.IsNullOrEmpty(puttyLocation))
                {
                    customSshClient = SshConsole.Putty;
                }
                else if (!string.IsNullOrEmpty(openSshLocation))
                {
                    customSshClient = SshConsole.OpenSSH;
                }
            }

            var needToSave = false;
            // avoid updating settings if they haven't changed
            if (customSshClient != Properties.Settings.Default.CustomSshConsole)
            {
                Properties.Settings.Default.CustomSshConsole = customSshClient;
                needToSave = true;
            }
            if (puttyLocation != null && !puttyLocation.Equals(Properties.Settings.Default.PuttyLocation))
            {
                Properties.Settings.Default.PuttyLocation = puttyLocation;
                needToSave = true;
            }
            if (openSshLocation != null && !openSshLocation.Equals(Properties.Settings.Default.OpenSSHLocation))
            {
                Properties.Settings.Default.OpenSSHLocation = openSshLocation;
                needToSave = true;
            }

            if (needToSave)
                TrySaveSettings();
        }

        private static void AddConnection(IXenConnection connection)
        {
            if (ConnectionsManager.XenConnectionsContains(connection))
                return;

            XenConnection conn = connection as XenConnection;
            conn.LastConnectionFullName =
                string.IsNullOrEmpty(connection.FriendlyName) ?
                    connection.Hostname :
                    string.Format("{0} ({1})", connection.FriendlyName, connection.Hostname);

            lock (ConnectionsManager.ConnectionsLock)
            {
                ConnectionsManager.XenConnections.Add(connection);
            }
        }

        /// <summary>
        /// If an exception is thrown while saving the list, this method will notify the user
        /// and then call Application.Exit().
        /// </summary>
        public static void SaveServerList()
        {
            Program.AssertOnEventThread();

            // Check that there is something to do...
            if (ConnectionsManager.XenConnectionsCopy.Count == 0)
            {
                // Ensure the list is empty in the serialized settings file...
                Properties.Settings.Default.ServerList = new string[0];
                Properties.Settings.Default.ServerAddressList = new string[0];
                TrySaveSettings();
                return;
            }

            if (!Program.SkipSessionSave && Registry.AllowCredentialSave)
            {
                if (Program.RunInAutomatedTestMode)
                    log.Debug("In automated test mode: not showing save session dialog");
                else
                {
                    using (var dialog = new SaveAndRestoreDialog())
                        dialog.ShowDialog(Program.MainWindow);
                    Program.SkipSessionSave = true;
                }
            }

            EncryptServerList();
            TrySaveSettings();
        }

        /// <summary>
        /// Attempts to save the settings. On catching an exception will prompt the user and close XC.
        /// </summary>
        public static void TrySaveSettings()
        {
            try
            {
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                //catch all as ConfigurationErrorsException does not account for all errors that may happen
                log.Error("Could not save settings. Exiting application.", ex);

                using (var dlg = new ErrorDialog(string.Format(Messages.MESSAGEBOX_SAVE_CORRUPTED, GetUserConfigPath()))
                { WindowTitle = Messages.MESSAGEBOX_SAVE_CORRUPTED_TITLE })
                {
                    dlg.ShowDialog(Program.MainWindow);
                }

                Application.Exit();
            }
        }

        private static void EncryptServerList()
        {
            List<string> encServerList = new List<string>();
            List<string> encServerAddressList = new List<string>();

            try
            {
                foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                {
                    int port = connection.Port;
                    if (port <= 0)
                        port = ConnectionsManager.DEFAULT_XEN_PORT;

                    if (Properties.Settings.Default.SaveSession)
                    {
                        encServerList.Add(EncryptCredentials(connection.Hostname, port, connection.Username, connection.Password,
                            !connection.IsConnected, connection.FriendlyName, connection.PoolMembers));
                    }

                    // Save the address, port and friendly name in case the user clicks cancel on the resume password dialog
                    string entryAddress = ProtectCredentials(connection.Hostname, port, connection.FriendlyName);
                    encServerAddressList.Add(entryAddress);
                }

                if (Properties.Settings.Default.SaveSession)
                {
                    foreach (string vm_uuid in VNCPasswords.Keys)
                    {
                        string pwd = VNCPasswords[vm_uuid];
                        if (pwd != null)
                            encServerList.Add(EncryptCredentials(vm_uuid, -1, VNC_INDICATOR, pwd, false, "", null));
                    }
                }

                if (Properties.Settings.Default.SaveSession)
                    Properties.Settings.Default.ServerList = encServerList.ToArray();
                Properties.Settings.Default.ServerAddressList = encServerAddressList.ToArray();
            }
            catch (Exception exp)
            {
                // Problem encrypting
                log.Error("Could not encrypt session.", exp);

                // Ugly, but we need to warn the user...
                if (!Program.RunInAutomatedTestMode)
                    using (var dlg = new WarningDialog(Messages.MESSAGEBOX_SESSION_SAVE_UNABLE)
                    { WindowTitle = Messages.MESSAGEBOX_SESSION_SAVE_UNABLE_TITLE })
                    {
                        dlg.ShowDialog();
                    }
            }
        }

        private static string ProtectCredentials(String serverName, int port, string friendlyName)
        {
            string entryStr = string.Join(SEPARATOR.ToString(), new string[] { serverName, port.ToString(), friendlyName });
            return EncryptionUtils.Protect(entryStr);
        }

        private static String EncryptCredentials(String serverName, int port, String username, String password, bool saveDisconnected, string friendlyName, List<string> poolMembers)
        {
            if (password == null)
            {
                // We don't have a password saved for this connection: save a special marker value
                password = MARKER_VALUE;
            }
            string entryStr = string.Join(SEPARATOR.ToString(), new string[] { username, serverName, port.ToString(), password, (saveDisconnected ? DISCONNECTED : CONNECTED), friendlyName });
            if (poolMembers != null && poolMembers.Count > 0)
            {
                string members = string.Join(",", poolMembers.ToArray());
                entryStr += SEPARATOR.ToString();
                entryStr += members;
            }
            return Properties.Settings.Default.RequirePass && Program.MainPassword != null ? EncryptionUtils.EncryptString(entryStr, Program.MainPassword) : EncryptionUtils.Protect(entryStr);
        }

        public static string[] GetServerHistory()
        {
            if (Properties.Settings.Default.ServerHistory == null)
                Properties.Settings.Default.ServerHistory = Array.Empty<string>();

            return Properties.Settings.Default.ServerHistory;
        }

        public static void UpdateServerHistory(string hostnameWithPort)
        {
            var history = new List<string>(GetServerHistory());

            if (!history.Contains(hostnameWithPort))
            {
                while (history.Count >= 20)
                    history.RemoveAt(0);
                history.Add(hostnameWithPort);
                Properties.Settings.Default.ServerHistory = history.ToArray();
                TrySaveSettings();
            }
        }

        public static AutoCompleteStringCollection GetVMwareServerHistory()
        {
            if (Properties.Settings.Default.VMwareServerHistory == null)
                Properties.Settings.Default.VMwareServerHistory = new AutoCompleteStringCollection();

            return Properties.Settings.Default.VMwareServerHistory;
        }

        public static void UpdateVMwareServerHistory(string server)
        {
            var history = GetVMwareServerHistory();
            if (history.Contains(server))
                return;

            while (history.Count >= 20)
                history.RemoveAt(0);

            history.Add(server);
            Properties.Settings.Default.VMwareServerHistory = history;
            TrySaveSettings();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vm_uuid"></param>
        /// <returns>The password corresponding to the given VM, or null if no password is stored.</returns>
        public static char[] GetVNCPassword(string vm_uuid)
        {
            return
                VNCPasswords.ContainsKey(vm_uuid) && VNCPasswords[vm_uuid] != null ?
                    VNCPasswords[vm_uuid].ToCharArray() :
                    null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vm_uuid"></param>
        /// <param name="password">May be null, to clear the password</param>
        public static void SetVNCPassword(string vm_uuid, char[] password)
        {
            VNCPasswords[vm_uuid] = password == null ? null : new string(password);
            SaveServerList();
        }

        public static void AddCertificate(string hashString, string hostname)
        {
            Dictionary<string, string> known_servers = KnownServers;
            known_servers.Add(hostname, hashString);
            KnownServers = known_servers;
        }

        public static Dictionary<string, string> KnownServers
        {
            get
            {
                var known = new Dictionary<string, string>();

                var knownServers = Properties.Settings.Default.KnownServers;
                if (knownServers == null)
                    return known;

                foreach (string knownHost in knownServers)
                {
                    string[] hostCert = knownHost.Split(' ');
                    if (hostCert.Length != 2)
                        continue;

                    known.Add(hostCert[0], hostCert[1]);
                }
                return known;
            }
            set
            {
                Properties.Settings.Default.KnownServers = value == null
                    ? new string[0]
                    : value.Select(kvp => string.Format("{0} {1}", kvp.Key, kvp.Value)).ToArray();

                TrySaveSettings();
            }
        }

        public static void ReplaceCertificate(string hostname, string hashString)
        {
            Dictionary<string, string> known_servers = KnownServers;
            known_servers[hostname] = hashString;
            KnownServers = known_servers;
        }

        public static void UpdateDisabledPluginsList(List<string> list)
        {
            Properties.Settings.Default.DisabledPlugins = list.ToArray();
            TrySaveSettings();
        }

        public static bool IsPluginEnabled(string name, string org)
        {
            string id = $"{org}::{name}";
            return Properties.Settings.Default.DisabledPlugins.All(s => s != id);
            //returns true for empty collection, which is correct
        }

        public static void Log()
        {
            log.Info("Tools Options Settings -");

            log.Info($"=== ProxySetting: {Properties.Settings.Default.ProxySetting}");
            log.Info($"=== ProxyAddress: {Properties.Settings.Default.ProxyAddress}");
            log.Info($"=== ProxyPort: {Properties.Settings.Default.ProxyPort}");
            log.Info($"=== ByPassProxyForServers: {Properties.Settings.Default.BypassProxyForServers}");
            log.Info($"=== ProvideProxyAuthentication: {Properties.Settings.Default.ProvideProxyAuthentication}");
            log.Info($"=== ProxyAuthenticationMethod: {Properties.Settings.Default.ProxyAuthenticationMethod}");
            log.Info($"=== ConnectionTimeout: {Properties.Settings.Default.ConnectionTimeout}");

            log.Info($"=== FullScreenShortcutKey: {Properties.Settings.Default.FullScreenShortcutKey}");
            log.Info($"=== DockShortcutKey: {Properties.Settings.Default.DockShortcutKey}");
            log.Info($"=== UncaptureShortcutKey: {Properties.Settings.Default.UncaptureShortcutKey}");
            log.Info($"=== ClipboardAndPrinterRedirection: {Properties.Settings.Default.ClipboardAndPrinterRedirection}");
            log.Info($"=== WindowsShortcuts: {Properties.Settings.Default.WindowsShortcuts}");
            log.Info($"=== ReceiveSoundFromRDP: {Properties.Settings.Default.ReceiveSoundFromRDP}");
            log.Info($"=== AutoSwitchToRDP: {Properties.Settings.Default.AutoSwitchToRDP}");
            log.Info($"=== ConnectToServerConsole: {Properties.Settings.Default.ConnectToServerConsole}");
            log.Info($"=== PreserveScaleWhenUndocked: {Properties.Settings.Default.PreserveScaleWhenUndocked}");
            log.Info($"=== PreserveScaleWhenSwitchBackToVNC: {Properties.Settings.Default.PreserveScaleWhenSwitchBackToVNC}");

            log.Info($"=== WarnUnrecognizedCertificate: {Properties.Settings.Default.WarnUnrecognizedCertificate}");
            log.Info($"=== WarnChangedCertificate: {Properties.Settings.Default.WarnChangedCertificate}");
            log.Info($"=== RemindChangePassword: {Properties.Settings.Default.RemindChangePassword}");

            //do not log Fileservice settings
            log.Info($"=== AllowXenCenterUpdates: {Properties.Settings.Default.AllowXenCenterUpdates}");
            log.Info($"=== AllowPatchesUpdates: {Properties.Settings.Default.AllowPatchesUpdates}");
            log.Info($"=== AllowXenServerUpdates: {Properties.Settings.Default.AllowXenServerUpdates}");

            log.Info($"=== FillAreaUnderGraphs: {Properties.Settings.Default.FillAreaUnderGraphs}");
            log.Info($"=== RememberLastSelectedTab: {Properties.Settings.Default.RememberLastSelectedTab}");

            log.Info($"=== SaveSession: {Properties.Settings.Default.SaveSession}");
            log.Info($"=== RequirePass: {Properties.Settings.Default.RequirePass}");

            var disabledPlugins = Properties.Settings.Default.DisabledPlugins.Length == 0
                ? "<None>"
                : string.Join(", ", Properties.Settings.Default.DisabledPlugins);
            log.InfoFormat($"=== DisabledPlugins: {disabledPlugins}");

            log.Info($"=== DoNotConfirmDismissAlerts: {Properties.Settings.Default.DoNotConfirmDismissAlerts}");
            log.Info($"=== DoNotConfirmDismissUpdates: {Properties.Settings.Default.DoNotConfirmDismissUpdates}");
            log.Info($"=== DoNotConfirmDismissEvents: {Properties.Settings.Default.DoNotConfirmDismissEvents}");
            log.Info($"=== IgnoreOvfValidationWarnings: {Properties.Settings.Default.IgnoreOvfValidationWarnings}");
        }

        public static void Load()
        {
            Properties.Settings.Load();

            string appVersionString = Program.Version.ToString();
            log.InfoFormat("Application version of current settings {0}", appVersionString);

            if (Properties.Settings.Default.ApplicationVersion != appVersionString)
            {
                log.Info("Upgrading settings...");
                Properties.Settings.Default.Upgrade();

                // if program's hash has changed (e.g. by upgrading to .NET 4.0), then Upgrade() doesn't import the previous application settings 
                // because it cannot locate a previous user.config file. In this case a new user.config file is created with the default settings.
                // We will try and find a config file from a previous installation and update the settings from it

                if (Properties.Settings.Default.ApplicationVersion == "" && Properties.Settings.Default.DoUpgrade)
                    UpgradeFromPreviousInstallation();

                log.InfoFormat("Settings upgraded from '{0}' to '{1}'", Properties.Settings.Default.ApplicationVersion, appVersionString);
                Properties.Settings.Default.ApplicationVersion = appVersionString;
                TrySaveSettings();
            }
        }

        /// <summary>
        /// Looks for a config file from a previous installation of the application and updates the settings from it.
        /// </summary>
        private static void UpgradeFromPreviousInstallation()
        {
            try
            {
                // The path of the user.config files looks something like this:
                // <Profile Directory>\<Company Name>\<App Name>_<Evidence Type>_<Evidence Hash>\<Version>\user.config
                // Get a previous user.config file by enumerating through all the folders in <Profile Directory>\<Company Name> 

                var currentConfigFolder = new DirectoryInfo(GetUserConfigPath()).Parent;

                var companyFolder = currentConfigFolder?.Parent?.Parent;
                if (companyFolder == null)
                    return;

                FileInfo previousConfig = null;
                Version previousVersion = null;
                Version currentVersion = Program.Version;

                var directories = companyFolder.GetDirectories($"{BrandManager.BrandConsole}*");

                foreach (var dir in directories)
                {
                    var configFiles = dir.GetFiles("user.config", SearchOption.AllDirectories);

                    foreach (var file in configFiles)
                    {
                        var configFolderName = Path.GetFileName(Path.GetDirectoryName(file.FullName));
                        if (configFolderName != null)
                        {
                            var configVersion = new Version(configFolderName);

                            if (configVersion <= currentVersion && (previousVersion == null || configVersion > previousVersion))
                            {
                                previousVersion = configVersion;
                                previousConfig = file;
                            }
                        }
                    }
                }

                if (previousConfig != null)
                {
                    // copy previous config file to current config location
                    var destinationFile = Path.GetDirectoryName(currentConfigFolder.FullName);

                    destinationFile = Path.Combine(destinationFile, previousVersion.ToString());

                    if (!Directory.Exists(destinationFile))
                        Directory.CreateDirectory(destinationFile);

                    destinationFile = Path.Combine(destinationFile, previousConfig.Name);

                    File.Copy(previousConfig.FullName, destinationFile);

                    // upgrade settings
                    XenAdmin.Properties.Settings.Default.Upgrade();
                }
            }
            catch (Exception ex)
            {
                log.Debug("Exception while updating settings.", ex);
            }
        }

        /// <summary>
        /// Configures .NET's AuthenticationManager to only use the authentication module that is 
        /// specified in the ProxyAuthenticationMethod setting. Also sets XenAPI's HTTP class to 
        /// use the same authentication method.
        /// </summary>
        public static void ReconfigureProxyAuthenticationSettings()
        {
            var authModules = AuthenticationManager.RegisteredModules;
            var modulesToUnregister = new List<IAuthenticationModule>();

            while (authModules.MoveNext())
            {
                var module = (IAuthenticationModule)authModules.Current;
                modulesToUnregister.Add(module);
            }

            foreach (var module in modulesToUnregister)
                AuthenticationManager.Unregister(module);

            var authSetting = (HTTP.ProxyAuthenticationMethod)Properties.Settings.Default.ProxyAuthenticationMethod;
            if (authSetting == HTTP.ProxyAuthenticationMethod.Basic)
                AuthenticationManager.Register(BasicAuthenticationModule);
            else
                AuthenticationManager.Register(DigestAuthenticationModule);

            HTTP.CurrentProxyAuthenticationMethod = authSetting;
            Session.Proxy = XenAdminConfigManager.Provider.GetProxyFromSettings(null);
        }
    }
}
