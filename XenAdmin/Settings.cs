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
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;

using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using System.Configuration;
using XenCenterLib;
using System.Drawing;
using System.Linq;


namespace XenAdmin
{
    public static class Settings
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
        private const string NO_PASSWORD = "\x02c1\x07f7\xaa5e\x31b3";
        private const string DISCONNECTED = "disconnected";
        private const string CONNECTED = "connected";

        /// <summary>
        /// vm_uuid -> password.  The password may be null if it's been deleted.
        /// </summary>
        private static Dictionary<string, string> VNCPasswords = new Dictionary<string, string>();

        /// <summary>
        /// MSDN info regarding the path to the user.config file is somewhat confusing. It turns out
        /// it is not in Application.UserAppDataPath as stated on http://msdn.microsoft.com/en-us/library/8eyb2ct1.aspx
        /// but rather in a location like BasePath\CompanyName\AppName_EvidenceType_EvidenceHash\Version as described on
        /// http://stackoverflow.com/questions/1075204/when-using-a-settings-settings-file-in-net-where-is-the-config-actually-stored
        /// and on http://msdn.microsoft.com/en-us/library/ms379611.aspx.
        /// Trying to retrieve the filename at the places where this method's caller caught the ConfigurationErrorsException
        /// returns null, so this one has to be called.
        /// </summary>
        /// <returns></returns>
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
            // Only try if the user has specified he actually wants to save sessions...
            if (Properties.Settings.Default.SaveSession == true || Properties.Settings.Default.RequirePass)
            {
                // Only try if we actually have a saved session list...
                if ((Properties.Settings.Default.ServerList != null && Properties.Settings.Default.ServerList.Length > 0) || (Properties.Settings.Default.ServerAddressList != null && Properties.Settings.Default.ServerAddressList.Length > 0))
                {
                    if (!Properties.Settings.Default.RequirePass)
                    {
                        Program.MasterPassword = null;
                        Program.SkipSessionSave = true;
                        RestoreSessionWithPassword(null, true);
                        return;
                    }
                    byte[] passHash = PromptForMasterPassword(false);

                    // passHash will be null if the user cancelled...
                    if (passHash != null)
                    {
                        if (!RestoreSessionWithPassword(passHash, true))
                        {
                            // User got the password wrong. Repeat until he gets it
                            // right or cancels...
                            while (passHash != null)
                            {
                                passHash = PromptForMasterPassword(true);
                                if (passHash != null)
                                {
                                    if (RestoreSessionWithPassword(passHash, true))
                                        break;
                                }
                            }
                        }
                    }
                    // if the user has cancelled we start a new session
                    if (passHash == null)
                    {
                        // an error state which can only occur on cancelled clicked
                        Properties.Settings.Default.SaveSession = false;
                        Properties.Settings.Default.RequirePass = true;
                        RestoreSessionWithPassword(null, false);
                    }
                    else
                    {
                        // otherwise make sure we have the correct settings
                        Properties.Settings.Default.SaveSession = true;
                        Properties.Settings.Default.RequirePass = true;
                    }
                    Program.SkipSessionSave = true;
                    Program.MasterPassword = passHash;
                }
                else
                {
                    // this is where the user comes in if it is the first time connecting
                    Properties.Settings.Default.RequirePass = false;
                    Properties.Settings.Default.SaveSession = false;
                    Program.MasterPassword = null;
                }
            }
            else
            {
                Program.MasterPassword = null;
                Program.SkipSessionSave = true;
                RestoreSessionWithPassword(null, false);
            }
        }

        /// <summary>
        /// Tries to restore the session list using the given password hash as
        /// the key. Returns true if successful, false otherwise (usu. due to
        /// a decryption failure, in turn due to a wrong password).
        /// </summary>
        /// <param name="passHash"></param>
        /// <param name="useOriginalList"></param>
        /// <returns></returns>
        private static bool RestoreSessionWithPassword(byte[] passHash, bool useOriginalList)
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
                        decryptedList[idx] = EncryptionUtils.DecryptString(encEntry, passHash);
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
                    // If password is NO_PASSWORD, this indicates we didn't save a password for this connection
                    if (entryComps[3] == NO_PASSWORD)
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

                    // We save a comma-separated list of hostnames of each of the slaves.
                    // This enables us to connect to a former slave in the event of master failover while the GUI isn't running.
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
        /// Prompts for the password to use to decrypt the server list.
        /// Returns the secure hash of the password entered.
        /// </summary>
        /// <param name="isRetry"></param>
        /// <returns></returns>
        private static byte[] PromptForMasterPassword(bool isRetry)
        {
            // close the splash screen before opening the password dialog (the dialog comes up behind the splash screen)
            Program.CloseSplash();

            LoadSessionDialog dialog = new LoadSessionDialog(isRetry);
            dialog.ShowDialog(Program.MainWindow);
            if (dialog.DialogResult == DialogResult.OK)
            {
                return dialog.PasswordHash;
            }
            else
            {
                return null;
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
                    new Dialogs.RestoreSession.SaveAndRestoreDialog(false).ShowDialog(Program.MainWindow);
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
            catch (ConfigurationErrorsException ex)
            {
                log.Error("Could not save settings. Reload config and try once more.");
                log.Error(ex, ex);

                // The following excpetion was reported to appear when AppData is redirected to an UNC path on NAS
                // System.Configuration.ConfigurationErrorsException: Failed to save settings: The configuration file has been changed by another program.
                // Reloading and saving again was reported to help
                try
                {
                    Properties.Settings.Default.Reload();
                    Properties.Settings.Default.Save();

                }
                catch (ConfigurationErrorsException er)
                {
                    // Show a warning to the user and exit the application.
                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(
                            SystemIcons.Error,
                            string.Format(Messages.MESSAGEBOX_SAVE_CORRUPTED, Settings.GetUserConfigPath()),
                            Messages.MESSAGEBOX_SAVE_CORRUPTED_TITLE)
                        ))
                    {
                        dlg.ShowDialog(Program.MainWindow);
                    }

                    log.Error("Could not save settings. Exiting application.");
                    log.Error(er, er);
                    Application.Exit();
                }
            }

            HealthCheck.SendMetadataToHealthCheck();
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
                    string entryAddress = protectCredentials(connection.Hostname, port, connection.FriendlyName);
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
                    using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Exclamation, Messages.MESSAGEBOX_SESSION_SAVE_UNABLE, Messages.MESSAGEBOX_SESSION_SAVE_UNABLE_TITLE)))
                    {
                        dlg.ShowDialog();
                    }
            }
        }

        private static string protectCredentials(String serverName, int port, string friendlyName)
        {
            string entryStr = string.Join(SEPARATOR.ToString(), new string[] { serverName, port.ToString(), friendlyName });
            return EncryptionUtils.Protect(entryStr);
        }

        private static String EncryptCredentials(String serverName, int port, String username, String password, bool saveDisconnected, string friendlyName, List<string> poolMembers)
        {
            if (password == null)
            {
                // We don't have a password saved for this connection: save a special marker value
                password = NO_PASSWORD;
            }
            string entryStr = string.Join(SEPARATOR.ToString(), new string[] { username, serverName, port.ToString(), password, (saveDisconnected ? DISCONNECTED : CONNECTED), friendlyName });
            if (poolMembers != null && poolMembers.Count > 0)
            {
                string members = string.Join(",", poolMembers.ToArray());
                entryStr += SEPARATOR.ToString();
                entryStr += members;
            }
            return Properties.Settings.Default.RequirePass && Program.MasterPassword != null ? EncryptionUtils.EncryptString(entryStr, Program.MasterPassword) : EncryptionUtils.Protect(entryStr);
        }

        public static AutoCompleteStringCollection GetServerHistory()
        {
            if (Properties.Settings.Default.ServerHistory == null)
                Properties.Settings.Default.ServerHistory = new AutoCompleteStringCollection();

            return Properties.Settings.Default.ServerHistory;
        }

        public static void UpdateServerHistory(string hostnameWithPort)
        {
            AutoCompleteStringCollection history = GetServerHistory();
            if (!history.Contains(hostnameWithPort))
            {
                while (history.Count >= 20)
                    history.RemoveAt(0);
                history.Add(hostnameWithPort);
                Properties.Settings.Default.ServerHistory = history;
                TrySaveSettings();
            }
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
            EncryptServerList();
            SaveServerList();
        }

        /// <summary>
        /// Compares the hash of 'p' with the given byte array.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="temporaryPassword"></param>
        /// <returns></returns>
        internal static bool PassCorrect(string p, byte[] temporaryPassword)
        {
            if (p.Length == 0)
                return false;

            return Helpers.ArrayElementsEqual(EncryptionUtils.ComputeHash(p), temporaryPassword);
        }

        public static void SaveIfRequired()
        {
            SaveServerList();
            TrySaveSettings();
        }

        public static void AddCertificate(X509Certificate certificate, string hostname)
        {
            Dictionary<string, string> known_servers = KnownServers;
            known_servers.Add(hostname, certificate.GetCertHashString());
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

        public static void ReplaceCertificate(string hostname, X509Certificate certificate)
        {
            Dictionary<string, string> known_servers = KnownServers;
            known_servers[hostname] = certificate.GetCertHashString();
            KnownServers = known_servers;
        }

        public static void UpdateDisabledPluginsList(List<string> list)
        {
            Properties.Settings.Default.DisabledPlugins = list.ToArray();
            TrySaveSettings();
        }

        public static bool IsPluginEnabled(string name, string org)
        {
            string id = string.Format("{0}::{1}", org, name);
            foreach (string s in Properties.Settings.Default.DisabledPlugins)
                if (s == id)
                    return false;
            return true;
        }
    }
}
