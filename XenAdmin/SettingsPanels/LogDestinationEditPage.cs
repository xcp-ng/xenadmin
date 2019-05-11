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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Core;


namespace XenAdmin.SettingsPanels
{
    public partial class LogDestinationEditPage : UserControl, IEditPage
    {
        private Host _host;
        private bool _validToSave;
        private string _origLocation;

        private Regex regex = new Regex(@"^[a-zA-Z0-9]([-a-zA-Z0-9]{0,61}[a-zA-Z0-9])?(\.[a-zA-Z0-9]([-a-zA-Z0-9]{0,61}[a-zA-Z0-9])?)*$");

        private readonly ToolTip InvalidParamToolTip;

        public LogDestinationEditPage()
        {
            InitializeComponent();

            Text = Messages.LOG_DESTINATION;

            InvalidParamToolTip = new ToolTip
            {
                IsBalloon = true,
                ToolTipIcon = ToolTipIcon.Warning,
                ToolTipTitle = Messages.INVALID_PARAMETER
            };
        }

        private string RemoteServer => ServerTextBox.Text.Trim();

        #region IVerticalTabs implementation

        public Image Image => Properties.Resources.log_destination_16;

        public string SubText
        {
            get
            {
                return checkBoxRemote.Checked
                    ? string.Format(Messages.HOST_LOG_DESTINATION_LOCAL_AND_REMOTE, RemoteServer)
                    : Messages.HOST_LOG_DESTINATION_LOCAL;
            }
        }

        #endregion

        private void Repopulate()
        {
            if (_host == null)
                return;

            _origLocation = _host.GetSysLogDestination();
            checkBoxRemote.Checked = !string.IsNullOrWhiteSpace(_origLocation);
            ServerTextBox.Text = _origLocation;
            ReValidate();
        }

        private void ReValidate()
        {
            _validToSave = !checkBoxRemote.Checked ||
                           !string.IsNullOrEmpty(RemoteServer) && regex.IsMatch(RemoteServer);
        }

        #region IEditPage implementation

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _host = clone as Host;
            Repopulate();
        }

        public bool ValidToSave => _validToSave;

        public bool HasChanged
        {
            get
            {
                if (checkBoxRemote.Checked)
                    return _origLocation != RemoteServer;

                return !string.IsNullOrWhiteSpace(_origLocation);
            }
        }

        public AsyncAction SaveSettings()
        {
            if (_host == null)
                return null;

            _host.SetSysLogDestination(checkBoxRemote.Checked ? RemoteServer : null);

            return new DelegatedAsyncAction(
                _host.Connection,
                Messages.ACTION_CHANGE_LOG_DESTINATION,
                string.Format(Messages.ACTION_CHANGING_LOG_DESTINATION_FOR, _host),
                null,
                delegate(Session session) { Host.syslog_reconfigure(session, _host.opaque_ref); },
                true,
                "host.syslog_reconfigure"
            );
        }

        public void ShowLocalValidationMessages()
        {
            if (!_validToSave)
                HelpersGUI.ShowBalloonMessage(ServerTextBox, Messages.GENERAL_EDIT_INVALID_REMOTE, InvalidParamToolTip);
        }

        public void Cleanup()
        {
            InvalidParamToolTip.Dispose();
        }

        #endregion

        private void checkBoxRemote_CheckedChanged(object sender, EventArgs e)
        {
            ReValidate();
        }

        private void ServerTextBox_TextChanged(object sender, EventArgs e)
        {
            ReValidate();
        }

        private void ServerTextBox_Enter(object sender, EventArgs e)
        {
            checkBoxRemote.Checked = true;
        }
    }
}
