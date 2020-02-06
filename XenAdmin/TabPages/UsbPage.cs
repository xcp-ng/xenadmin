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
using System.ComponentModel;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Dialogs;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;


namespace XenAdmin.TabPages
{
    public partial class UsbPage : BaseTabPage
    {
        private Host _host;

        public UsbPage()
        {
            InitializeComponent();
            Text = Messages.USB;
            labelGridTitle.Text = Messages.USB_DEVICES;

            buttonPassthrough.Enabled = false;
            buttonPassthrough.Text = Messages.USBLIST_ENABLE_PASSTHROUGH_HOTKEY;
        }

        public override string HelpID => "TabPageUSB";

        public IXenObject XenObject
        {
            get
            {
                Program.AssertOnEventThread();
                return _host;
            }
            set
            {
                Program.AssertOnEventThread();

                UnregisterHandlers();

                _host = value as Host;
                if (_host != null)
                {
                    _host.PropertyChanged += Host_PropertyChanged;
                    _host.Connection.Cache.RegisterBatchCollectionChanged<PUSB>(UsbCollectionChanged);
                }

                BuildList();
            }
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
                if (_host == null || _host.Locked)
                    return;

                if (!_host.Connection.CacheIsPopulated)
                    return;

                dataGridViewUsbList.SuspendLayout();
                dataGridViewUsbList.Rows.Clear();

                List<PUSB> pusbs = _host.Connection.ResolveAll(_host.PUSBs);
                foreach (PUSB pusb in pusbs)
                {
                    dataGridViewUsbList.Rows.Add(new HostUsbRow(pusb));
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

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
        }

        public override void PageHidden()
        {
            UnregisterHandlers();

            base.PageHidden();
        }

        void Host_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshAllItems();
        }

        void UsbCollectionChanged(object sender, EventArgs e)
        {
            BuildList();
        }

        private void RefreshAllItems()
        {
            foreach (DataGridViewExRow row in dataGridViewUsbList.Rows)
            {
                var usbRow = row as HostUsbRow;
                if (usbRow != null)
                    usbRow.UpdateDetails();
            }
        }

        internal void UnregisterHandlers()
        {
            if (_host != null)
            {
                _host.PropertyChanged -= Host_PropertyChanged;
                _host.Connection.Cache.DeregisterBatchCollectionChanged<PUSB>(UsbCollectionChanged);
            }

            foreach (DataGridViewExRow row in dataGridViewUsbList.Rows)
            {
                var usbRow = row as HostUsbRow;
                if (usbRow != null)
                    usbRow.DeregisterEvents();
            }
        }

        private HostUsbRow selectedRow = null;
        private void dataGridViewUsbList_SelectionChanged(object sender, EventArgs e)
        {
            selectedRow = null;

            if (dataGridViewUsbList.SelectedRows.Count > 0)
                selectedRow = (HostUsbRow)dataGridViewUsbList.SelectedRows[0];

            if (selectedRow == null)
            {
                // When none item selected, disable the button and show 
                // "Enable Passthrough" on the button for better experience.
                buttonPassthrough.Text = Messages.USBLIST_ENABLE_PASSTHROUGH_HOTKEY;
                buttonPassthrough.Enabled = false;
            }
            else if (selectedRow.Pusb.passthrough_enabled)
            {
                // Selected item has been passthrough enabled, set the availability of the button 
                // as per whether the device has been attached or not.
                PUSB pusb = selectedRow.Pusb;
                USB_group usbGroup = pusb.Connection.Resolve(pusb.USB_group);
                bool attached = (usbGroup != null) && (usbGroup.VUSBs != null) && (usbGroup.VUSBs.Count > 0);

                buttonPassthrough.Text = Messages.USBLIST_DISABLE_PASSTHROUGH_HOTKEY;
                buttonPassthrough.Enabled = !attached;
            }
            else
            {
                // Selected item has not been passthrough enabled.
                buttonPassthrough.Text = Messages.USBLIST_ENABLE_PASSTHROUGH_HOTKEY;
                buttonPassthrough.Enabled = true;
            } 
        }

        private void buttonPassthrough_Click(object sender, EventArgs e)
        {
            if (selectedRow != null)
            {
                UsbUsageDialog dialog = new UsbUsageDialog(selectedRow.Pusb);
                dialog.ShowDialog(Program.MainWindow);
            }
        }

        private class HostUsbRow : DataGridViewExRow
        {
            private PUSB _pusb;
            private VM _vm;

            private DataGridViewTextBoxCell locationCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell descriptionCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell passthroughCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell vmCell = new DataGridViewTextBoxCell();

            public HostUsbRow(PUSB pusb)
            {
                _pusb = pusb;
                SetVm();

                _pusb.PropertyChanged += Pusb_PropertyChanged;

                Cells.AddRange(locationCell,
                    descriptionCell,
                    passthroughCell,
                    vmCell);
                UpdateDetails();
            }

            private void SetVm()
            {
                if (_pusb != null)
                {
                    USB_group usbgroup = _pusb.Connection.Resolve(_pusb.USB_group);
                    VUSB vusb = (usbgroup != null && usbgroup.VUSBs != null && usbgroup.VUSBs.Count > 0) ? _pusb.Connection.Resolve(usbgroup.VUSBs[0]) : null;
                    _vm = vusb == null ? null : _pusb.Connection.Resolve(vusb.VM);
                    if (_vm != null)
                        _vm.PropertyChanged += Vm_PropertyChanged;
                }
            }

            public PUSB Pusb
            {
                get { return _pusb; }
            }

            public void UpdateDetails()
            {
                locationCell.Value = _pusb.path;
                descriptionCell.Value = _pusb.Description();
                passthroughCell.Value = _pusb.passthrough_enabled ? Messages.ENABLED : Messages.DISABLED;
                vmCell.Value = _vm == null ? "" : _vm.name_label;
            }

            public void DeregisterEvents()
            {
                _pusb.PropertyChanged -= Pusb_PropertyChanged;
                if (_vm != null)
                    _vm.PropertyChanged -= Vm_PropertyChanged;
            }

            private void Pusb_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "attached")
                {
                    if (_vm != null)  // Remove PropertyChanged handler for old VM
                        _vm.PropertyChanged -= Vm_PropertyChanged;

                    SetVm();
                }
                UpdateDetails();
            }

            private void Vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "name_label")
                    UpdateDetails();
            }
        }
    }
}
