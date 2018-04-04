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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Properties;
using XenAPI;
using XenCenterLib;

namespace XenAdmin.Dialogs.OptionsPages
{
    public partial class ConnectionOptionsPage : UserControl, IOptionsPage
    {
        private const string ConnectionTabSettingsHeader = "Connection Tab Settings -";
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private OptionsDialog optionsDialog;

        // used for preventing the event handlers from doing anything when changing controls through code
        private bool eventsDisabled = true;

        public ConnectionOptionsPage()
        {
            InitializeComponent();

            build();
        }

        public OptionsDialog OptionsDialog
        {
            set { this.optionsDialog = value; }
        }

        private void build()
        {
            // Proxy server
            switch ((HTTPHelper.ProxyStyle)Properties.Settings.Default.ProxySetting)
            {
                case HTTPHelper.ProxyStyle.DirectConnection:
                    DirectConnectionRadioButton.Checked = true;
                    break;
                case HTTPHelper.ProxyStyle.SystemProxy:
                    UseIERadioButton.Checked = true;
                    break;
                case HTTPHelper.ProxyStyle.SpecifiedProxy:
                    UseProxyRadioButton.Checked = true;
                    break;
                default:
                    DirectConnectionRadioButton.Checked = true;
                    break;
            }

            ProxyAddressTextBox.Text = Properties.Settings.Default.ProxyAddress;
            ProxyPortTextBox.Text = Properties.Settings.Default.ProxyPort.ToString();
            BypassForServersCheckbox.Checked = Properties.Settings.Default.BypassProxyForServers;

            AuthenticationCheckBox.Checked = Properties.Settings.Default.ProvideProxyAuthentication;
                
            switch ((HTTP.ProxyAuthenticationMethod)Properties.Settings.Default.ProxyAuthenticationMethod)
            {
                case HTTP.ProxyAuthenticationMethod.Basic:
                    BasicRadioButton.Checked = true;
                    break;
                case HTTP.ProxyAuthenticationMethod.Digest:
                    DigestRadioButton.Checked = true;
                    break;
                default:
                    DigestRadioButton.Checked = true;
                    break;
            }

            // checks for empty default username/password which starts out unencrypted
            string protectedUsername = Properties.Settings.Default.ProxyUsername;
            ProxyUsernameTextBox.Text = string.IsNullOrEmpty(protectedUsername) ? "" : EncryptionUtils.Unprotect(Properties.Settings.Default.ProxyUsername);
            string protectedPassword = Properties.Settings.Default.ProxyPassword;
            ProxyPasswordTextBox.Text = string.IsNullOrEmpty(protectedPassword) ? "" : EncryptionUtils.Unprotect(Properties.Settings.Default.ProxyPassword);
            
            ConnectionTimeoutNud.Value = Properties.Settings.Default.ConnectionTimeout / 1000;

            eventsDisabled = false;
        }

        private void UseProxyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (eventsDisabled)
                return;

            enableOK();
        }

        private void AuthenticationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (eventsDisabled)
                return;

            eventsDisabled = true;

            if (!AuthenticationCheckBox.Checked)
            {
                ProxyUsernameTextBox.Clear();
                ProxyPasswordTextBox.Clear();
            }
            SelectUseThisProxyServer();

            eventsDisabled = false;

            enableOK();
        }

        private void GeneralProxySettingsChanged(object sender, EventArgs e)
        {
            if (eventsDisabled)
                return;
            SelectUseThisProxyServer();
            enableOK();
        }

        private void ProxyAuthenticationSettingsChanged(object sender, EventArgs e)
        {
            if (eventsDisabled)
                return;
            SelectProvideCredentials();
            enableOK();
        }

        private void SelectUseThisProxyServer()
        {
            UseProxyRadioButton.Checked = true;
        }

        private void SelectProvideCredentials()
        {
            AuthenticationCheckBox.Checked = true;
            UseProxyRadioButton.Checked = true;
        }

        private void enableOK()
        {
            if (optionsDialog == null)
                return;

            if (!UseProxyRadioButton.Checked)
            {
                optionsDialog.okButton.Enabled = true;
                return;
            }
            else if (AuthenticationCheckBox.Checked && string.IsNullOrEmpty(ProxyUsernameTextBox.Text))
            {
                optionsDialog.okButton.Enabled = false;
                return;
            }

            try
            {
                if (!Util.IsValidPort(ProxyPortTextBox.Text))
                {
                    optionsDialog.okButton.Enabled = false;
                    return;
                }

                var uriHostNameType = Uri.CheckHostName(ProxyAddressTextBox.Text);

                optionsDialog.okButton.Enabled =  uriHostNameType != UriHostNameType.Unknown && uriHostNameType != UriHostNameType.IPv6;
                return;
            }
            catch
            {
                optionsDialog.okButton.Enabled = false;
            }
        }

        public static void Log()
        {
            log.Info(ConnectionTabSettingsHeader);
            // Proxy server
            log.Info("=== ProxySetting: " + Properties.Settings.Default.ProxySetting.ToString());
            log.Info("=== ProxyAddress: " + Properties.Settings.Default.ProxyAddress.ToString());
            log.Info("=== ProxyPort: " + Properties.Settings.Default.ProxyPort.ToString());
            log.Info("=== ByPassProxyForServers: " + Properties.Settings.Default.BypassProxyForServers.ToString());
            log.Info("=== ProvideProxyAuthentication: " + Properties.Settings.Default.ProvideProxyAuthentication.ToString());
            log.Info("=== ProxyAuthenticationMethod: " + Properties.Settings.Default.ProxyAuthenticationMethod.ToString());
            log.Info("=== ConnectionTimeout: " + Properties.Settings.Default.ConnectionTimeout.ToString());
        }

        #region IOptionsPage Members

        public void Save()
        {
            // Proxy server settings
            HTTPHelper.ProxyStyle new_proxy_style =
                DirectConnectionRadioButton.Checked ? HTTPHelper.ProxyStyle.DirectConnection :
                UseIERadioButton.Checked ? HTTPHelper.ProxyStyle.SystemProxy :
                                                      HTTPHelper.ProxyStyle.SpecifiedProxy;
            
            if (Properties.Settings.Default.ProxySetting != (int)new_proxy_style)
                Properties.Settings.Default.ProxySetting = (int)new_proxy_style;
            
            if (ProxyAddressTextBox.Text != Properties.Settings.Default.ProxyAddress && !string.IsNullOrEmpty(ProxyAddressTextBox.Text))
                Properties.Settings.Default.ProxyAddress = ProxyAddressTextBox.Text;

            Properties.Settings.Default.ProxyUsername = EncryptionUtils.Protect(ProxyUsernameTextBox.Text);
            Properties.Settings.Default.ProxyPassword = EncryptionUtils.Protect(ProxyPasswordTextBox.Text);
            Properties.Settings.Default.ProvideProxyAuthentication = AuthenticationCheckBox.Checked;

            HTTP.ProxyAuthenticationMethod new_auth_method = BasicRadioButton.Checked ?
                HTTP.ProxyAuthenticationMethod.Basic : HTTP.ProxyAuthenticationMethod.Digest;
            if (Properties.Settings.Default.ProxyAuthenticationMethod != (int)new_auth_method)
                Properties.Settings.Default.ProxyAuthenticationMethod = (int)new_auth_method;

            try
            {
                int port = int.Parse(ProxyPortTextBox.Text);
                if (port != Properties.Settings.Default.ProxyPort)
                    Properties.Settings.Default.ProxyPort = port;
            }
            catch
            {
                Properties.Settings.Default.ProxyPort = 80;
            }

            if (BypassForServersCheckbox.Checked != Properties.Settings.Default.BypassProxyForServers)
                Properties.Settings.Default.BypassProxyForServers = BypassForServersCheckbox.Checked;

            // timeout settings
            int timeout = (int)ConnectionTimeoutNud.Value;
            if (timeout * 1000 != Properties.Settings.Default.ConnectionTimeout)
            {
                Properties.Settings.Default.ConnectionTimeout = timeout * 1000;
            }

            Program.ReconfigureConnectionSettings();

            Core.HealthCheck.SendProxySettingsToHealthCheck();
        }

        #endregion

        #region IVerticalTab Members

        public override string Text
        {
            get { return Messages.CONNECTION; }
        }

        public string SubText
        {
            get { return Messages.CONNECTION_DESC; }
        }

        public Image Image
        {
            get { return Resources._000_Network_h32bit_16; }
        }

        #endregion
    }
}
