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
using System.Configuration;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Plugins;
using XenAdmin.ServerDBs;
using XenAPI;
using XenCenterLib;

namespace XenAdmin
{
    public class WinformsXenAdminConfigProvider : IXenAdminConfigProvider
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Func<List<Role>, IXenConnection, string, AsyncAction.SudoElevationResult> SudoDialogDelegate => SudoDialog;

        public int ConnectionTimeout => Properties.Settings.Default.ConnectionTimeout;

        public Session CreateActionSession(Session session, IXenConnection connection)
        {
            return SessionFactory.DuplicateSession(session, connection, ConnectionTimeout);
        }

        public bool Exiting => Program.Exiting;

        public bool ForcedExiting => Program.ForcedExiting;

        public string XenCenterUUID => Program.XenCenterUUID;

        public bool DontSudo => Registry.DontSudo;

        public IWebProxy GetProxyFromSettings(IXenConnection connection)
        {
            return GetProxyFromSettings(connection, true);
        }

        public IWebProxy GetProxyFromSettings(IXenConnection connection, bool isForXenServer)
        {
            try
            {
                if (connection != null && connection.Session != null && connection.Session.opaque_ref == "dummy")
                    return new XenAdminSimulatorWebProxy(DbProxy.proxys[connection]);

                switch ((HTTPHelper.ProxyStyle)XenAdmin.Properties.Settings.Default.ProxySetting)
                {
                    case HTTPHelper.ProxyStyle.SpecifiedProxy:
                        if (isForXenServer && Properties.Settings.Default.BypassProxyForServers)
                            return null;

                        string address = string.Format("http://{0}:{1}",
                            XenAdmin.Properties.Settings.Default.ProxyAddress,
                            XenAdmin.Properties.Settings.Default.ProxyPort);

                        if (XenAdmin.Properties.Settings.Default.ProvideProxyAuthentication)
                        {
                            string protectedUsername = XenAdmin.Properties.Settings.Default.ProxyUsername;
                            string protectedPassword = XenAdmin.Properties.Settings.Default.ProxyPassword;
                            return new WebProxy(address, false, null, new NetworkCredential(
                                // checks for empty default username/password which starts out unencrypted
                                string.IsNullOrEmpty(protectedUsername) ? "" : EncryptionUtils.Unprotect(protectedUsername),
                                string.IsNullOrEmpty(protectedPassword) ? "" : EncryptionUtils.Unprotect(protectedPassword)));
                        }
                        else
                            return new WebProxy(address, false);

                    case HTTPHelper.ProxyStyle.SystemProxy:
                        return WebRequest.GetSystemWebProxy();

                    default:
                        return null;
                }
            }
            catch (ConfigurationErrorsException e)
            {
                log.Error("Error parsing 'ProxySetting' from settings - settings file deemed corrupt", e);
                using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Error,
                    string.Format(Messages.MESSAGEBOX_LOAD_CORRUPTED, Settings.GetUserConfigPath()),
                    Messages.MESSAGEBOX_LOAD_CORRUPTED_TITLE)))
                {
                    dlg.ShowDialog();
                }

                Environment.Exit(1);
            }
            return null;
        }

        public int GetProxyTimeout(bool timeout)
        {
            return timeout ? XenAdmin.Properties.Settings.Default.HttpTimeout : 0;
        }

        public void ShowObject(string newVMRef)
        {
            Program.ShowObject(newVMRef);
        }

        public void HideObject(string newVMRef)
        {
            Program.HideObject(newVMRef);
        }

        public bool ObjectIsHidden(string opaqueRef)
        {
            return Program.ObjectIsHidden(opaqueRef);
        }

        public string GetLogFile()
        {
            return Program.GetLogFile();
        }

        public void UpdateServerHistory(string hostnameWithPort)
        {
            Settings.UpdateServerHistory(hostnameWithPort);
        }

        public void SaveSettingsIfRequired()
        {
            Settings.SaveIfRequired();
        }

        private AsyncAction.SudoElevationResult SudoDialog(List<Role> rolesAbleToCompleteAction,
            IXenConnection connection, string actionTitle)
        {
            var d = new Dialogs.RoleElevationDialog(connection, connection.Session, rolesAbleToCompleteAction,
                                                    actionTitle);

            DialogResult result = DialogResult.None;
            Program.Invoke(Program.MainWindow, delegate
                                                   {
                                                       result = d.ShowDialog(Program.MainWindow);
                                                   });

            return new AsyncAction.SudoElevationResult(result == DialogResult.OK, d.elevatedUsername, 
                                                                 d.elevatedPassword, d.elevatedSession);

        }

        public bool ShowHiddenVMs => Properties.Settings.Default.ShowHiddenVMs;

        public PluginManager PluginManager;

        public string GetXenCenterMetadata(bool isForXenCenter)
        {
            return Metadata.Generate(PluginManager, isForXenCenter);
        }      
    }
}
