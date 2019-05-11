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
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Properties;
using XenAdmin.Dialogs;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.SettingsPanels
{
    public partial class USBEditPage : XenTabPage, IEditPage
    {
        private VM _vm;
        public VM.HA_Restart_Priority SelectedPriority { private get; set; }

        public USBEditPage()
        {
            InitializeComponent();
        }

        #region override IEditPage

        public event Action Populated;

        public bool ValidToSave
        {
            get { return true; }
        }

        public bool HasChanged
        {
            get { return true; }
        }

        public override string Text
        {
            get { return Messages.USB_EDIT_TEXT; }
        }

        public string SubText
        { 
            get
            {
                if (_vm != null)
                {
                    if (_vm.VUSBs.Count == 0)
                        return Messages.USB_EDIT_SUBTEXT_NODEVICES;
                    if (_vm.VUSBs.Count == 1)
                        return Messages.USB_EDIT_SUBTEXT_1_DEVICE;
                    return string.Format(Messages.USB_EDIT_SUBTEXT_MULTIPLE_DEVICES, _vm.VUSBs.Count);
                }
                else
                    return "";
            }
        }

        public Image Image
        {
            get { return Resources.usb_16; }
        }

        public AsyncAction SaveSettings()
        {
            return null;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            Trace.Assert(orig is VM);

            _vm = (VM)orig;
            if (Connection == null)
                Connection = _vm.Connection;

            _vm.PropertyChanged += Vm_PropertyChanged;

            BuildList();
            SelectedPriority = _vm.HARestartPriority();
            ShowHideWarnings();
            dataGridViewUsbList_SelectionChanged(null, null);
        }

        public void ShowHideWarnings()
        {
            // Check:
            //    If VM is halted and
            //    If HA was enabled on pool and Restart priority was set on VM.
            Pool pool = Helpers.GetPool(_vm.Connection);
            bool haEnabled = (pool != null &&
                pool.ha_enabled &&
                VM.HaPriorityIsRestart(_vm.Connection, SelectedPriority));
            if (_vm.power_state != vm_power_state.Halted)
            {
                labelWarning.Text = Messages.USB_ATTACH_NOT_ALLOWED_NOT_HALTED;
                buttonAttach.Enabled = false;
            }
            else if (haEnabled)
            {
                labelWarning.Text = Messages.USB_ATTACH_NOT_ALLOWED_HA;
                buttonAttach.Enabled = false;
            }
            else
            {
                buttonAttach.Enabled = true;
            }
            flowLayoutPanelWarning.Visible = !buttonAttach.Enabled;
        }

        public bool InBuildList = false;
        private void BuildList()
        {
            Program.AssertOnEventThread();

            if (InBuildList)
                return;

            InBuildList = true;

            try
            {
                dataGridViewUsbList.SuspendLayout();
                dataGridViewUsbList.Rows.Clear();

                List<VUSB> vusbs = _vm.Connection.ResolveAll(_vm.VUSBs);
                foreach (VUSB vusb in vusbs)
                {
                    dataGridViewUsbList.Rows.Add(new VMUsbRow(vusb));
                }
            }
            finally
            {
                if (dataGridViewUsbList.SortedColumn != null)
                {
                    dataGridViewUsbList.Sort(
                        dataGridViewUsbList.SortedColumn,
                        dataGridViewUsbList.SortOrder == SortOrder.Ascending
                            ? ListSortDirection.Ascending : ListSortDirection.Descending);
                }
                dataGridViewUsbList.ResumeLayout();
                InBuildList = false;
            }
        }

        public void ShowLocalValidationMessages()
        {

        }

        public void Cleanup()
        {
            _vm.PropertyChanged -= Vm_PropertyChanged;
            foreach (DataGridViewExRow row in dataGridViewUsbList.Rows)
            {
                var usbRow = row as VMUsbRow;
                if (usbRow != null)
                    usbRow.DeregisterEvents();
            }
        }

        
        #endregion

        void Vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VUSBs")
            {
                BuildList();
                if (Populated != null)
                    Populated();
            }
        }

        private VMUsbRow selectedRow = null;
        private void dataGridViewUsbList_SelectionChanged(object sender, System.EventArgs e)
        {
            selectedRow = null;
            buttonDetach.Enabled = false;

            if (dataGridViewUsbList.SelectedRows.Count > 0)
            {
                selectedRow = (VMUsbRow)dataGridViewUsbList.SelectedRows[0];
                buttonDetach.Enabled = true;
            }
        }

        private void buttonAttach_Click(object sender, System.EventArgs e)
        {
            if (_vm != null)
            {
                AttachUsbDialog dialog = new AttachUsbDialog(_vm);
                dialog.ShowDialog(Program.MainWindow);
            }
        }

        private void buttonDetach_Click(object sender, System.EventArgs e)
        {
            if ((selectedRow != null) && (_vm != null))
            {
                bool confirmed = false;
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.ACTION_VUSB_DETACH_CONFIRM, Messages.ACTION_VUSB_DETACH),
                    ThreeButtonDialog.ButtonYes,
                    new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, ThreeButtonDialog.ButtonType.CANCEL, true)))
                {
                    if (dlg.ShowDialog(Program.MainWindow) == DialogResult.Yes)
                    {
                        confirmed = true;
                    }
                }
                if (confirmed)
                {
                    new XenAdmin.Actions.DeleteVUSBAction(selectedRow.Vusb, _vm).RunAsync();
                }
            }
            
        }

        private class VMUsbRow : DataGridViewExRow
        {
            private VUSB _vusb;
            private DataGridViewTextBoxCell locationCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell descriptionCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell attachedCell = new DataGridViewTextBoxCell();

            public VMUsbRow(VUSB vusb)
            {
                _vusb = vusb;
                _vusb.PropertyChanged += Vusb_PropertyChanged;

                Cells.AddRange(locationCell,
                    descriptionCell,
                    attachedCell);
                UpdateDetails();
            }

            public VUSB Vusb
            {
                get { return (VUSB)_vusb; }
            }

            public void UpdateDetails()
            {
                USB_group usbgroup = _vusb.Connection.Resolve(_vusb.USB_group);
                PUSB pusb = _vusb.Connection.Resolve(usbgroup.PUSBs[0]);

                locationCell.Value = pusb.path;
                descriptionCell.Value = pusb.Description();
                attachedCell.Value = _vusb.currently_attached.ToYesNoStringI18n();
            }

            public void DeregisterEvents()
            {
                _vusb.PropertyChanged -= Vusb_PropertyChanged;
            }

            private void Vusb_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                UpdateDetails();
            }
        }
    }
}
