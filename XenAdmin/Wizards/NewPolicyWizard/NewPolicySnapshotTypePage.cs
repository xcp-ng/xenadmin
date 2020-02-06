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
using System.Linq;
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
                if (BackupType == vmss_type.snapshot_with_quiesce)
                    return Messages.QUIESCED_SNAPSHOTS;
                return Messages.DISKS_AND_MEMORY;
            }

        }

        public override string HelpID => "Snapshottype";

        public override string PageTitle => Messages.SNAPSHOT_TYPE_TITLE;

        public override string Text => Messages.SNAPSHOT_TYPE;

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction == PageLoadedDirection.Forward)
                EnableShapshotTypes(Connection);
        }

        public Image Image => Properties.Resources._000_VMSession_h32bit_16;

        public bool ValidToSave => true;

        public void ShowLocalValidationMessages()
        { }

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
                if (radioButtonDiskAndMemory.Checked)
                    return vmss_type.checkpoint;
                return vmss_type.unknown;
            }
        }

        public string BackupTypeToString
        {
            get
            {
                switch (BackupType)
                {
                    case vmss_type.snapshot:
                        return Messages.DISKS_ONLY;
                    case vmss_type.checkpoint:
                        return Messages.DISKS_AND_MEMORY;
                    case vmss_type.snapshot_with_quiesce:
                        return Messages.QUIESCED_SNAPSHOTS;
                    default:
                        return Messages.UNKNOWN;
                }
            }
        }

        public void ToggleQuiesceCheckBox(List<VM> selectedVMs)
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

            foreach (VM vm in selectedVMs)
            {
                if (!vm.allowed_operations.Contains(vm_operations.snapshot_with_quiesce) || Helpers.FeatureForbidden(vm, Host.RestrictVss))
                {
                    quiesceCheckBox.Enabled = false;
                    quiesceCheckBox.Checked = false;
                    break;
                }
            }

        }

        private void EnableShapshotTypes(IXenConnection connection)
        {
            radioButtonDiskAndMemory.Enabled =
                label3.Enabled = !Helpers.FeatureForbidden(connection, Host.RestrictCheckpoint);
            tableLayoutPanelCheckpoint.Visible = !radioButtonDiskAndMemory.Enabled;
            pictureBoxWarning.Visible = labelWarning.Visible = radioButtonDiskAndMemory.Enabled;

            var vssFeatureExists = !Helpers.QuebecOrGreater(connection);
            quiesceCheckBox.Visible = vssFeatureExists;

            if (_policy == null) // new policy
            {
                quiesceCheckBox.Enabled = vssFeatureExists && !Helpers.FeatureForbidden(connection, Host.RestrictVss);
                if (quiesceCheckBox.Enabled && SelectedVMs != null && SelectedVMs.Any(vm => !vm.allowed_operations.Contains(vm_operations.snapshot_with_quiesce)))
                    quiesceCheckBox.Enabled = quiesceCheckBox.Checked = false;
            }
            else // editing existing policy
            {
                switch (_policy.type)
                {
                    case vmss_type.checkpoint:
                        radioButtonDiskAndMemory.Checked = true;
                        quiesceCheckBox.Enabled = false;
                        break;
                    case vmss_type.snapshot:
                        radioButtonDiskOnly.Checked = true;
                        // when a policy does not have any VMs, irrespective of the snapshot type, enable Quiesce if supported
                        quiesceCheckBox.Enabled = vssFeatureExists && _policy.VMs.Count == 0;
                        break;
                    case vmss_type.snapshot_with_quiesce:
                        radioButtonDiskOnly.Checked = true;
                        // when the snapshot type itself is quiesce then we need to enable it irrespective of the number of VMs ( > 1 condition)
                        quiesceCheckBox.Visible = quiesceCheckBox.Enabled = true;
                        quiesceCheckBox.Checked = true;
                        break;
                }
            }
            tableLayoutPanelVss.Visible = vssFeatureExists && !quiesceCheckBox.Enabled;
            tableLayoutPanelVssRemoved.Visible = !vssFeatureExists && quiesceCheckBox.Checked;
        }

        public AsyncAction SaveSettings()
        {
            _policy.type = BackupType;
            return null;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _policy = (VMSS)clone;
            EnableShapshotTypes(_policy.Connection);
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
            tableLayoutPanelVssRemoved.Visible = Helpers.QuebecOrGreater(_policy != null ? _policy.Connection : Connection) && quiesceCheckBox.Checked;
        }

        private void radioButtonDiskAndMemory_CheckedChanged(object sender, System.EventArgs e)
        {
            if (radioButtonDiskAndMemory.Checked)
                quiesceCheckBox.Checked = false;
        }

        #endregion
    }
}
