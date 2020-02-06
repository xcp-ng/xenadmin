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
using System.ComponentModel;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.XCM;
using XenCenterLib;


namespace XenAdmin.Wizards.ConversionWizard
{
    public partial class CredentialsPage : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool _buttonNextEnabled;

        public CredentialsPage()
        {
            InitializeComponent();
            tableLayoutStatus.Visible = false;
        }

        #region XenTabPage Implementation

        public override string Text => Messages.CONVERSION_CREDENTIALS_PAGE_TEXT;

        public override string PageTitle => Messages.CONVERSION_CREDENTIALS_PAGE_TITLE;

        public override string HelpID => "Credentials";

        public override bool EnableNext()
        {
            return _buttonNextEnabled;
        }

        public override bool EnablePrevious()
        {
            return false;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction == PageLoadedDirection.Forward)
            {
                var history = Settings.GetVMwareServerHistory();

                string[] historyArray = new string[history.Count];
                history.CopyTo(historyArray, 0);
                Array.Sort(historyArray, StringUtility.NaturalCompare);

                foreach (string serverName in historyArray)
                {
                    if (serverName != null)
                        comboBoxServer.Items.Add(serverName);
                }

                // Use a clone as the auto-complete source because of CA-38715
                var historyClone = new AutoCompleteStringCollection();
                historyClone.AddRange(historyArray);
                comboBoxServer.AutoCompleteCustomSource = historyClone;

                UpdateButtons();
            }
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Back)
                return;
            {
                VmwareCredInfo = new ServerInfo
                {
                    ServerType = (int)ServerType.VirtualCenter,
                    Hostname = VmWareServer,
                    Username = Username,
                    Password = Password
                };

                tableLayoutPanel2.Enabled = false;
                tableLayoutStatus.Visible = true;
                pictureBoxStatus.Image = Images.StaticImages.ajax_loader;
                labelStatus.Text = Messages.CONVERSION_CONNECTING_VMWARE;
                VMwareVMs = null;

                _backgroundWorker.RunWorkerAsync(VmwareCredInfo);
                UpdateButtons();

                while(_backgroundWorker.IsBusy)
                    Application.DoEvents();

                cancel = VMwareVMs == null;
            }
        }

        public override void PageCancelled(ref bool cancel)
        {
            _backgroundWorker.CancelAsync();
        }

        #endregion

        #region Properties

        public ConversionClient ConversionClient { private get; set; }
        public VmInstance[] VMwareVMs { get; private set; }
        public ServerInfo VmwareCredInfo { get; private set; }

        private string Username => textBoxUser.Text.Trim();
        private string Password => textBoxPassword.Text.Trim();
        public string VmWareServer => comboBoxServer.Text.Trim();

        #endregion

        private void UpdateButtons()
        {
            _buttonNextEnabled = !_backgroundWorker.IsBusy &&
                                 !string.IsNullOrEmpty(VmWareServer) && !string.IsNullOrEmpty(Username);
            OnPageUpdated();
        }

        #region Event handlers

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = ConversionClient.GetSourceVMs((ServerInfo)e.Argument);
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                tableLayoutStatus.Visible = false;
            }
            if (e.Error != null)
            {
                log.Error(e.Error);
                pictureBoxStatus.Image = Images.StaticImages._000_error_h32bit_16;
                labelStatus.Text = Messages.CONVERSION_CONNECTING_VMWARE_FAILURE;
            }
            else
            {
                VMwareVMs = e.Result as VmInstance[];
                pictureBoxStatus.Image = Images.StaticImages._000_Tick_h32bit_16;
                labelStatus.Text = Messages.CONVERSION_CONNECTING_VMWARE_SUCCESS;
                Settings.UpdateVMwareServerHistory(VmWareServer);
            }

            tableLayoutPanel2.Enabled = true;
            UpdateButtons();
        }

        private void comboBoxServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void comboBoxServer_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void textBoxUser_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        #endregion
    }
}
