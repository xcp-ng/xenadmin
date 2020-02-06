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
using XenAdmin.Actions.VMActions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Dialogs
{
    public partial class CopyVMDialog : XenDialogBase
    {
        private readonly VM TheVM;
        private readonly bool IsRealVm;

        public CopyVMDialog(VM vm)
        {
            InitializeComponent();
            this.MinimumSize = this.Size;
            IsRealVm = !vm.is_a_template;
            TheVM = vm;
            srPicker1.Usage = SrPicker.SRPickerType.MoveOrCopy;
            Host affinity = TheVM.Home();
            srPicker1.Connection = TheVM.Connection;
            srPicker1.DiskSize = vm.TotalVMSize();
            labelSrHint.Text = IsRealVm ? Messages.COPY_VM_SELECT_SR : Messages.COPY_TEMPLATE_SELECT_SR;
            srPicker1.SetAffinity(affinity);
            Pool pool = Helpers.GetPoolOfOne(vm.Connection);
            if (pool != null)
                srPicker1.DefaultSR = vm.Connection.Resolve(pool.default_SR);

            NameTextBox.Text = GetDefaultCopyName(TheVM);

            Text = IsRealVm ? Messages.COPY_VM : Messages.COPY_TEMPLATE;

            bool allow_copy = !vm.is_a_template || vm.allowed_operations.Contains(vm_operations.copy);
            bool anyDiskFastCloneable = vm.AnyDiskFastClonable();
            bool hasAtLeastOneDisk = vm.HasAtLeastOneDisk();

            CopyRadioButton.Enabled = allow_copy && hasAtLeastOneDisk;
            FastClonePanel.Enabled = !allow_copy || anyDiskFastCloneable || !hasAtLeastOneDisk;
            if (!FastClonePanel.Enabled)
            {
                CloneRadioButton.Checked = false;
            }
            toolTipContainer1.SetToolTip(Messages.FAST_CLONE_UNAVAILABLE);
            if (vm.is_a_template && !(anyDiskFastCloneable || allow_copy))
            {
                CloneRadioButton.Text = Messages.COPY_VM_CLONE_TEMPLATE_SLOW;
                FastCloneDescription.Text = Messages.COPY_VM_SLOW_CLONE_DESCRIPTION;
            }
            else
            {
                FastCloneDescription.Text = !vm.is_a_template ? Messages.COPY_VM_FAST_CLONE_DESCRIPTION : Messages.COPY_TEMPLATE_FAST_CLONE_DESCRIPTION;
            }

            if (!CloneRadioButton.Enabled)
                CopyRadioButton.Checked = true;

            if (vm.DescriptionType() != VM.VmDescriptionType.None)
                DescriptionTextBox.Text = vm.Description();

            srPicker1.Invalidate();
            srPicker1.selectDefaultSROrAny();
        }

        private void srPicker1_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableMoveButton();
        }

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            EnableMoveButton();
        }

        private void EnableMoveButton()
        {
            MoveButton.Enabled = NameTextBox.Text.Trim().Length > 0 && srPicker1.SR != null;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MoveButton_Click(object sender, EventArgs e)
        {
            if (!tableLayoutPanelSrPicker.Enabled || CloneRadioButton.Checked)
            {
                new VMCloneAction(TheVM,NameTextBox.Text, DescriptionTextBox.Text).RunAsync();
                Close();
                return;
            }

            if (srPicker1.SR == null)
                return;
            //TODO  this
            //bool sameSr = true;
            //XenObject<SR> vm_sr = null;
            //foreach (XenObject<VBD> vbd in TheVM.Connection.ResolveAll(TheVM.VBDs))
            //{
            //    if (vbd.type != vbd_type.Disk)
            //        continue;

            //    XenObject<VDI> vdi = TheVM.Connection.Resolve(vbd.VDI);
            //    if (vdi == null)
            //        continue;

            //    XenObject<SR> sr = TheVM.Connection.Resolve(vdi.SR);
            //    if(sr == null)
            //        continue;

            //    if (vm_sr == null)
            //    {
            //        vm_sr = sr;
            //        continue;
            //    }

            //    if (vm_sr.opaque_ref != sr.opaque_ref)
            //    {
            //        sameSr = false;
            //        break;
            //    }
            //}


            //if (sameSr && vm_sr != null && srPicker1.SR.opaque_ref == vm_sr.opaque_ref) // if we dont change the sr use clone
            //{
            //    action = new VmAction(TheVM.Connection, TheVM, TheVM.GetStorageHost(TheVM.Connection), NameTextBox.Text);
            //}
            //else
            //{

            var action = new VMCopyAction(TheVM, TheVM.GetStorageHost(false), srPicker1.SR, NameTextBox.Text, DescriptionTextBox.Text);
            //}
            action.RunAsync();
            Close();
        }


        private static string GetDefaultCopyName(VM vmToCopy)
        {
            List<string> takenNames = new List<string>();
            foreach (VM vm in vmToCopy.Connection.Cache.VMs)
            {
                takenNames.Add(vm.Name());
            }
            return Helpers.MakeUniqueName(string.Format(Messages.ACTION_TEMPLATE_CLONE_NEW_NAME, vmToCopy.Name()), takenNames);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            tableLayoutPanelSrPicker.Enabled = CopyRadioButton.Checked;
            // Since the radiobuttons aren't in the same panel, we have to do manual mutual exclusion
            CloneRadioButton.Checked = !CopyRadioButton.Checked;
        }

        private void CloneRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Since the radiobuttons aren't in the same panel, we have to do manual mutual exclusion
            CopyRadioButton.Checked = !CloneRadioButton.Checked;
        }

        private void CopyVMDialog_Shown(object sender, EventArgs e)
        {
            tableLayoutPanelSrPicker.Enabled = CopyRadioButton.Enabled && CopyRadioButton.Checked;
        }
    }
}
