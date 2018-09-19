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
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Actions.VMActions;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{
    public partial class IntraPoolCopyPage : XenTabPage
    {
        public readonly VM TheVM;

        public IntraPoolCopyPage(List<VM> selectedVMs)
        {
            this.TheVM = selectedVMs[0]; 
            InitializeComponent();
        }

        private bool _buttonNextEnabled;
        private bool _buttonPreviousEnabled;

        public bool CloneVM
        {
            get { return !srPicker1.Enabled || CloneRadioButton.Checked; }
        }

        public SR SelectedSR
        {
            get { return srPicker1.SR; }
        }

        public string NewVmName
        {
            get { return NameTextBox.Text; }
        }

        public string NewVMmDescription
        {
            get { return DescriptionTextBox.Text; }
        }

        #region Base class (XenTabPage) overrides

        /// <summary>
        /// Gets the page's title (headline)
        /// </summary>
        public override string PageTitle { get { return Messages.CPM_WIZARD_INTRA_POOL_COPY_TITLE; } }

        /// <summary>
        /// Gets the page's label in the (left hand side) wizard progress panel
        /// </summary>
        public override string Text { get { return Messages.CPM_WIZARD_INTRA_POOL_COPY_TAB_TITLE; } }

        /// <summary>
        /// Gets the value by which the help files section for this page is identified
        /// </summary>
        public override string HelpID { get { return "IntraPoolCopy"; } }

        protected override bool ImplementsIsDirty()
        {
            return false;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            SetButtonsEnabled(true);
        }

        public override void PopulatePage()
        {
            srPicker1.Usage = SrPicker.SRPickerType.MoveOrCopy;
            srPicker1.ItemSelectionNotNull += srPicker1_ItemSelectionNotNull;
            srPicker1.ItemSelectionNull += srPicker1_ItemSelectionNull;
            Host affinity = TheVM.Home();
            srPicker1.Connection = TheVM.Connection;
            srPicker1.DiskSize = TheVM.TotalVMSize();
            srPicker1.SrHint.Text = TheVM.is_a_template ? Messages.COPY_TEMPLATE_SELECT_SR : Messages.COPY_VM_SELECT_SR;
            srPicker1.SetAffinity(affinity);
            Pool pool = Helpers.GetPoolOfOne(TheVM.Connection);
            if (pool != null)
                srPicker1.DefaultSR = TheVM.Connection.Resolve(pool.default_SR);

            NameTextBox.Text = GetDefaultCopyName(TheVM);

            bool allow_copy = !TheVM.is_a_template || TheVM.allowed_operations.Contains(vm_operations.copy);
            bool anyDiskFastCloneable = TheVM.AnyDiskFastClonable();
            bool hasAtLeastOneDisk = TheVM.HasAtLeastOneDisk();

            CopyRadioButton.Enabled = allow_copy && hasAtLeastOneDisk;
            FastClonePanel.Enabled = !allow_copy || anyDiskFastCloneable || !hasAtLeastOneDisk;
            if (!FastClonePanel.Enabled)
            {
                CloneRadioButton.Checked = false;
            }
            toolTipContainer1.SetToolTip(Messages.FAST_CLONE_UNAVAILABLE);
            if (TheVM.is_a_template && !(anyDiskFastCloneable || allow_copy))
            {
                CloneRadioButton.Text = Messages.COPY_VM_CLONE_TEMPLATE_SLOW;
                FastCloneDescription.Text = Messages.COPY_VM_SLOW_CLONE_DESCRIPTION;
            }
            else
            {
                FastCloneDescription.Text = !TheVM.is_a_template ? Messages.COPY_VM_FAST_CLONE_DESCRIPTION : Messages.COPY_TEMPLATE_FAST_CLONE_DESCRIPTION;
            }

            if (!CloneRadioButton.Enabled)
                CopyRadioButton.Checked = true;

            if (TheVM.DescriptionType() != VM.VmDescriptionType.None)
                DescriptionTextBox.Text = TheVM.Description();

            srPicker1.srListBox.Invalidate();
            srPicker1.selectDefaultSROrAny();

            srPicker1.Enabled = CopyRadioButton.Enabled && CopyRadioButton.Checked;

            labelRubric.Text = TheVM.is_a_template
                                   ? Messages.COPY_TEMPLATE_INTRA_POOL_RUBRIC
                                   : Messages.COPY_VM_INTRA_POOL_RUBRIC;
        }

        public override bool EnableNext()
        {
            return _buttonNextEnabled;
        }

        public override bool EnablePrevious()
        {
            return _buttonPreviousEnabled;
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            var l = new List<VM>();
            l.Add(TheVM);
            if (!CrossPoolMigrateWizard.AllVMsAvailable(l))
            {
                cancel = true;
                SetButtonsEnabled(false);
            }
        }
        #endregion

        private void SetButtonsEnabled(bool enabled)
        {
            _buttonNextEnabled = enabled;
            _buttonPreviousEnabled = enabled;
            OnPageUpdated();
        }

        private void srPicker1_ItemSelectionNull()
        {
            EnableMoveButton();
        }

        private void srPicker1_ItemSelectionNotNull()
        {
            EnableMoveButton();
        }

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            EnableMoveButton();
        }

        private void EnableMoveButton()
        {
            _buttonNextEnabled = NameTextBox.Text.Trim().Length > 0 && srPicker1.SR != null;
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

        private void CloneRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Since the radiobuttons aren't in the same panel, we have to do manual mutual exclusion
            CopyRadioButton.Checked = !CloneRadioButton.Checked;
        }

        private void CopyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            srPicker1.Enabled = CopyRadioButton.Checked;
            // Since the radiobuttons aren't in the same panel, we have to do manual mutual exclusion
            CloneRadioButton.Checked = !CopyRadioButton.Checked;
        }
    }
}
