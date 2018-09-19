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

using System.Collections.Generic;
using System.Drawing;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.SettingsPanels;
using XenAPI;


namespace XenAdmin.Wizards.NewPolicyWizard
{
    public partial class NewPolicySnapshotTypePage : XenTabPage, IEditPage
    {
        private VMSS _policy;

        public List<VM> SelectedVMs { private get; set; }
        
        public NewPolicySnapshotTypePage()
        {
            InitializeComponent();
        }

        public string SubText
        {
            get
            {
                if (BackupType == vmss_type.snapshot)
                    return Messages.DISKS_ONLY;
                else if (BackupType == vmss_type.snapshot_with_quiesce)
                    return Messages.QUIESCED_SNAPSHOTS;
                else
                    return Messages.DISKS_AND_MEMORY;
            }

        }

        public override string HelpID
        {
            get { return "Snapshottype"; }
        }

        public override string PageTitle
        {
            get
            {
                return Messages.SNAPSHOT_TYPE_TITLE;
            }
        }

        public override string Text
        {
            get
            {
                return Messages.SNAPSHOT_TYPE;
            }
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction == PageLoadedDirection.Forward)
                EnableShapshotTypes(Connection, false);
        }

        public Image Image
        {
            get { return Properties.Resources._000_VMSession_h32bit_16; }
        }

        public bool ValidToSave
        {
            get { return true; }
        }

        public void ShowLocalValidationMessages()
        {

        }

        public void Cleanup()
        {
            radioButtonDiskOnly.Checked = true;
        }

        public vmss_type BackupType
        {
            get
            {
                if (quiesceCheckBox.Checked)
                    return vmss_type.snapshot_with_quiesce;
                if (radioButtonDiskOnly.Checked)
                    return vmss_type.snapshot;
                else if (radioButtonDiskAndMemory.Checked)
                    return vmss_type.checkpoint;
                else
                {
                    return vmss_type.unknown;
                }
            }
        }

        public void ToggleQuiesceCheckBox(List<VM> SelectedVMs)
        {
            switch (BackupType)
            {
                case vmss_type.snapshot:
                    quiesceCheckBox.Enabled = true;
                    quiesceCheckBox.Checked = false;
                    break;

                case vmss_type.snapshot_with_quiesce:
                    quiesceCheckBox.Enabled = true;
                    quiesceCheckBox.Checked = true;
                    break;

                case vmss_type.checkpoint:
                    quiesceCheckBox.Enabled = true;
                    quiesceCheckBox.Checked = false;
                    break;
            }

            foreach (VM vm in SelectedVMs)
            {
                if (!vm.allowed_operations.Contains(vm_operations.snapshot_with_quiesce) || Helpers.FeatureForbidden(vm, Host.RestrictVss))
                {
                    quiesceCheckBox.Enabled = false;
                    quiesceCheckBox.Checked = false;
                    break;
                }
            }

        }

        private void RefreshTab(VMSS policy)
        {
            /* when a policy does not have any VMs, irrespective of
             * the snapshot type, enable Quiesce 
             */

            quiesceCheckBox.Enabled = (policy.VMs.Count == 0);

            switch (policy.type)
            {
                case vmss_type.checkpoint:
                    radioButtonDiskAndMemory.Checked = true;
                    quiesceCheckBox.Enabled = false;
                    break;
                case vmss_type.snapshot:
                    radioButtonDiskOnly.Checked = true;
                    break;
                case vmss_type.snapshot_with_quiesce:
                    radioButtonDiskOnly.Checked = true;

                    /* when the snapshot type itself is quiesce then we need to 
                     * enable it irrespective of the number of VMs ( > 1 condition)
                     */

                    quiesceCheckBox.Enabled = true;
                    quiesceCheckBox.Checked = true;
                    break;
            }
            EnableShapshotTypes(policy.Connection, quiesceCheckBox.Enabled);
        }

        private void EnableShapshotTypes(IXenConnection connection, bool isQuiesceEnabled)
        {
            radioButtonDiskAndMemory.Enabled =
                label3.Enabled = !Helpers.FeatureForbidden(connection, Host.RestrictCheckpoint);
            tableLayoutPanelCheckpoint.Visible = !radioButtonDiskAndMemory.Enabled;
            pictureBoxWarning.Visible = labelWarning.Visible = radioButtonDiskAndMemory.Enabled;

            quiesceCheckBox.Enabled = true;
            quiesceCheckBox.Visible = true;
            if (SelectedVMs != null)
            {
                if (SelectedVMs.Count > 0)
                {
                    foreach (VM vm in SelectedVMs)
                    {
                        if (!vm.allowed_operations.Contains(vm_operations.snapshot_with_quiesce) ||
                            Helpers.FeatureForbidden(vm, Host.RestrictVss))
                        {
                            quiesceCheckBox.Enabled = false;
                            quiesceCheckBox.Checked = false;
                            break;
                        }
                    }
                }
            }
            else /* we enter this block only when we are editing a policy, in that case the decision has already been taken in RefreshTab function */
            {
                quiesceCheckBox.Enabled = isQuiesceEnabled;
            }
            tableLayoutPanelVss.Visible = !quiesceCheckBox.Enabled;
        }

        public AsyncAction SaveSettings()
        {
            _policy.type = BackupType;
            return null;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _policy = (VMSS)clone;
            RefreshTab(_policy);
        }

        public bool HasChanged
        {
            get
            {
                return BackupType != _policy.type;
            }

        }

        #region Control event handlers

        private void quiesceCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            if (quiesceCheckBox.Checked)
                radioButtonDiskOnly.Checked = true;
        }

        private void radioButtonDiskAndMemory_CheckedChanged(object sender, System.EventArgs e)
        {
            if (radioButtonDiskAndMemory.Checked)
                quiesceCheckBox.Checked = false;
        }

        #endregion
    }
}
