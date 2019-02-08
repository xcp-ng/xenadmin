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
using XenAdmin.Actions;
using XenAPI;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;


namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class VHDoNFS : XenTabPage
    {
        private const string SERVER = "server";
        private const string SERVERPATH = "serverpath";
        private const string OPTIONS = "options";
        private const string NFSVERSION = "nfsversion";
        private const string PROBEVERSION = "probeversion";

        public VHDoNFS()
        {
            InitializeComponent();
            ToggleReattachControlsEnabledState(false);
        }

        #region XenTabPage overrides

        public override string Text { get { return Messages.NEWSR_LOCATION; } }

        public override string PageTitle { get { return Messages.NEWSR_PATH_NFS; } }

        public override string HelpID { get { return "Location_NFSVHD"; } }

        public override bool EnableNext()
        {
            return SrWizardHelpers.ValidateNfsSharename(NfsServerPathTextBox.Text)
                && (radioButtonNfsNew.Checked || listBoxNfsSRs.SelectedIndex > -1);
        }
        
        public override bool EnablePrevious()
        {
            if (SrWizardType.DisasterRecoveryTask && SrWizardType.SrToReattach == null)
                return false;

            return true;
        }

        public override void PopulatePage()
        {
            if (!SrWizardType.AllowToCreateNewSr)
                HideCreateControls();

            if (SrWizardType.UUID != null)
                listBoxNfsSRs.SetMustSelectUUID(SrWizardType.UUID);

            //Setting visibility of NFS Version controls
            nfsVersionLabel.Visible = nfsVersionSelectorTableLayoutPanel.Visible = Helpers.DundeeOrGreater(Connection);
        }

        public override void SelectDefaultControl()
        {
            NfsServerPathTextBox.Select();
        }

        #endregion

        private void UpdateButtons()
        {
            NfsScanButton.Enabled = SrWizardHelpers.ValidateNfsSharename(NfsServerPathTextBox.Text);

            OnPageUpdated();
        }

        private void NfsServerEnableVersionButtons(bool enable)
        {
            nfsVersion3RadioButton.Enabled = nfsVersion4RadioButton.Enabled = nfsVersion41RadioButton.Enabled = enable;
        }

        private void NfsServerPathTextBox_TextChanged(object sender, EventArgs e)
        {
            NfsScanButton.Enabled = SrWizardHelpers.ValidateNfsSharename(NfsServerPathTextBox.Text);

            listBoxNfsSRs.Items.Clear();
            ToggleReattachControlsEnabledState(false);

            if(radioButtonNfsNew.Enabled)
                radioButtonNfsNew.Checked = true;

            NfsServerEnableVersionButtons(true);

            UpdateButtons();
        }

        private void radioButtonNfsReattach_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonNfsNew.Checked = !radioButtonNfsReattach.Checked;
            UpdateButtons();
        }

        private void radioButtonNfsNew_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonNfsNew.Checked)
                listBoxNfsSRs.SelectedIndex = -1;
            
            radioButtonNfsReattach.Checked = !radioButtonNfsNew.Checked;
            UpdateButtons();
        }

        private void buttonNfsScan_Click(object sender, EventArgs e)
        {
            NfsScanButton.Enabled = false;

            // Perform an SR.probe to see if there is already an SR present
            Dictionary<String, String> dconf = new Dictionary<String, String>();
            string[] fullpath = NfsServerPathTextBox.Text.Trim().Split(new char[] { ':' });
            dconf[SERVER] = fullpath[0];
            if (fullpath.Length > 1)
            {
                dconf[SERVERPATH] = fullpath[1];
            }
            dconf[OPTIONS] = serverOptionsTextBox.Text;

            Host master = Helpers.GetMaster(Connection);
            if (master == null)
                return;

            if (Helpers.DundeeOrGreater(Connection))
                dconf[PROBEVERSION] = string.Empty; //this needs to be passed to the API in order to get back the NFS versions supported

            // Start probe
            SrProbeAction action = new SrProbeAction(Connection, master, SR.SRTypes.nfs, dconf);
            using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
            {
                dialog.ShowCancel = true;
                dialog.ShowDialog(this);
            }

            try
            {
                NfsScanButton.Enabled = true;
                if (radioButtonNfsNew.Enabled)
                    radioButtonNfsNew.Checked = true;

                listBoxNfsSRs.Items.Clear();

                if (!action.Succeeded)
                    return;

                GetSupportedNfsVersionsAndSetUI(action.Result);

                List<SR.SRInfo> SRs = SR.ParseSRListXML(action.Result);
                if (SRs.Count == 0)
                {
                    // Disable box
                    ToggleReattachControlsEnabledState(false);
                    listBoxNfsSRs.Items.Add(Messages.NEWSR_NO_SRS_FOUND);
                    return;
                }

                // Fill box
                foreach(SR.SRInfo info in SRs)
                    listBoxNfsSRs.Items.Add(info);

                listBoxNfsSRs.TryAndSelectUUID();
                
                ToggleReattachControlsEnabledState(true);
            }
            finally
            {
                UpdateButtons();
            }
        }

        private void GetSupportedNfsVersionsAndSetUI(string xml)
        {
            var supportedVersions = SR.ParseSupportedVersionsListXML(xml);

            if (supportedVersions.Count == 0)
            {
                //supported NFS version are not known: either we have not got a result back (older host) or neither version is supported (not likely)

                NfsServerEnableVersionButtons(true);
            }
            else
            {
                //supported NFS versions are known
                nfsVersion3RadioButton.Enabled = supportedVersions.Contains("3");
                nfsVersion4RadioButton.Enabled = supportedVersions.Contains("4");
                nfsVersion41RadioButton.Enabled = supportedVersions.Contains("4.1");

                // TODO: improve handling with 4.1
                //when only one version is supported, check/select the one that is
                if (!(nfsVersion3RadioButton.Enabled && nfsVersion4RadioButton.Enabled))
                {
                    nfsVersion3RadioButton.Checked = nfsVersion3RadioButton.Enabled;
                    nfsVersion4RadioButton.Checked = nfsVersion4RadioButton.Enabled;
                }
            }
        }

        private void listBoxNfsSRs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxNfsSRs.SelectedIndex == -1)
                return;

            radioButtonNfsReattach.Checked = true;
            UpdateButtons();
        }

        #region Accessors

        public SrWizardType SrWizardType { private get; set; }

        public string UUID
        {
            get
            {
                if (radioButtonNfsNew.Checked)
                    return null;

                SR.SRInfo srInfo = listBoxNfsSRs.SelectedItem as SR.SRInfo;
                if (srInfo == null)
                    return null;

                return srInfo.UUID;
            }
        }

        public Dictionary<string, string> DeviceConfig
        {
            get
            {
                var dconf = new Dictionary<string, string>();

                string[] fullpath = NfsServerPathTextBox.Text.Trim().Split(new char[] { ':' });

                dconf[OPTIONS] = serverOptionsTextBox.Text;
                dconf[SERVER] = fullpath[0];

                if (fullpath.Length > 1)
                {
                    dconf[SERVERPATH] = fullpath[1];
                }

                if (nfsVersion4RadioButton.Checked)
                    dconf[NFSVERSION] = "4";
                else if (nfsVersion41RadioButton.Checked)
                    dconf[NFSVERSION] = "4.1";

                return dconf;
            }
        }

        public string SrDescription
        {
            get
            {
                return string.IsNullOrEmpty(NfsServerPathTextBox.Text)
                           ? null
                           : string.Format(Messages.NEWSR_ACTION, NfsServerPathTextBox.Text);
            }
        }

        #endregion

        private void HideCreateControls()
        {
            radioButtonNfsNew.Checked = false;
            radioButtonNfsReattach.Checked = true;

            radioButtonNfsNew.Enabled = false;
            radioButtonNfsReattach.Enabled = true;
        }

        private void ToggleReattachControlsEnabledState(bool enable)
        {
            radioButtonNfsReattach.Enabled = enable;
            listBoxNfsSRs.Enabled = enable;
        }
    }
}
