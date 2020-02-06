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
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Dialogs
{
    public partial class VmSnapshotDialog : XenDialogBase
    {
        private readonly VM _VM;

        public VmSnapshotDialog(VM vm)
        {
            InitializeComponent();
            _VM = vm;
            UpdateWarnings();
            diskRadioButton.Enabled = _VM.allowed_operations.Contains(vm_operations.snapshot);
            pictureBoxSnapshotsInfo.Visible = labelSnapshotInfo.Visible = !diskRadioButton.Enabled;
            var quiesceAvailable = !Helpers.QuebecOrGreater(_VM.Connection);
            quiesceCheckBox.Visible = quiesceAvailable;
            quiesceCheckBox.Enabled = quiesceAvailable && _VM.allowed_operations.Contains(vm_operations.snapshot_with_quiesce) 
                && !Helpers.FeatureForbidden(_VM, Host.RestrictVss);
            pictureBoxQuiesceInfo.Visible = labelQuiesceInfo.Visible = quiesceAvailable && !quiesceCheckBox.Enabled;
            memoryRadioButton.Enabled = _VM.allowed_operations.Contains(vm_operations.checkpoint)
                && !Helpers.FeatureForbidden(_VM, Host.RestrictCheckpoint);
            CheckpointInfoPictureBox.Visible = labelCheckpointInfo.Visible = !memoryRadioButton.Enabled;
            UpdateOK();
        }

        /// <summary>
        /// Must be accessed on the GUI thread.
        /// </summary>
        public string SnapshotName => textBoxName.Text;

        public string SnapshotDescription => textBoxDescription.Text;

        public SnapshotType SnapshotType
        {
            get
            {
                if (quiesceCheckBox.Checked)
                    return SnapshotType.QUIESCED_DISK;
                if (diskRadioButton.Checked)
                    return SnapshotType.DISK;
                return SnapshotType.DISK_AND_MEMORY;
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            UpdateOK();
        }

        private void UpdateOK()
        {
            buttonOk.Enabled = !String.IsNullOrEmpty(textBoxName.Text.Trim()) && (diskRadioButton.Enabled || quiesceCheckBox.Enabled || memoryRadioButton.Enabled );
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioSender = (RadioButton)sender;
            if (radioSender.Checked)
            {
                if (radioSender == diskRadioButton)
                {
                    memoryRadioButton.Checked = false;
                }
                else
                {
                    diskRadioButton.Checked = false;
                    quiesceCheckBox.Checked = false;
                }
            }
        }

        private void VmSnapshotDialog_Load(object sender, EventArgs e)
        {
            textBoxName.Select();
        }

        private void quiesceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (quiesceCheckBox.Checked)
            {
                diskRadioButton.Checked = true;
            }
        }

        private void UpdateWarnings()
        {
            labelSnapshotInfo.Text = Messages.INFO_DISK_MODE;

            if (Helpers.FeatureForbidden(_VM, Host.RestrictVss))
                labelQuiesceInfo.Text = Messages.FIELD_DISABLED;
            else if (_VM.power_state != vm_power_state.Running)
                labelQuiesceInfo.Text = Messages.INFO_QUIESCE_MODE_POWER_STATE;
            else if (!_VM.GetVirtualisationStatus().HasFlag(VM.VirtualisationStatus.MANAGEMENT_INSTALLED))
                labelQuiesceInfo.Text = _VM.HasNewVirtualisationStates()
                    ? Messages.INFO_QUIESCE_MODE_NO_MGMNT
                    : Messages.INFO_QUIESCE_MODE_NO_TOOLS;
            else
                labelQuiesceInfo.Text = Messages.INFO_QUIESCE_MODE; // This says that VSS must be enabled. This is a guess, because we can't tell whether it is or not.

            if (Helpers.FeatureForbidden(_VM, Host.RestrictCheckpoint))
                labelCheckpointInfo.Text = Messages.FIELD_DISABLED;
            else if (_VM.power_state != vm_power_state.Running)
                labelCheckpointInfo.Text = Messages.INFO_DISKMEMORY_MODE_POWER_STATE;
            else if (!_VM.GetVirtualisationStatus().HasFlag(VM.VirtualisationStatus.IO_DRIVERS_INSTALLED))
                labelCheckpointInfo.Text = (_VM.HasNewVirtualisationStates() ? Messages.INFO_DISKMEMORY_MODE_NO_IO_DRIVERS : Messages.INFO_DISKMEMORY_MODE_NO_TOOLS);
            else
                labelCheckpointInfo.Text = Messages.INFO_DISKMEMORY_MODE_MISC;
        }
    }
}
