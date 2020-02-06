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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.XCM;


namespace XenAdmin.Wizards.ConversionWizard
{
    public partial class VmSelectionPage : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool _buttonNextEnabled;
        private bool _updating;

        public VmSelectionPage()
        {
            InitializeComponent();
            tableLayoutPanelError.Visible = false;
        }

        #region XenTabPage implementation

        public override string Text => Messages.CONVERSION_VM_PAGE_TEXT;

        public override string PageTitle => Messages.CONVERSION_VM_PAGE_TITLE;

        public override string HelpID => "VmSelection";

        public override bool EnableNext()
        {
            return _buttonNextEnabled;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction == PageLoadedDirection.Forward)
                Build();
        }

        public override void PageCancelled(ref bool cancel)
        {
            _backgroundWorker.CancelAsync();
        }

        #endregion

        public ConversionClient ConversionClient { private get; set; }
        public VmInstance[] VMwareVMs { private get; set; }
        public ServerInfo VmwareCredInfo { private get; set; }

        public VmInstance[] SelectedVms
        {
            get
            {
                return (from SourceVmRow row in dataGridViewVms.Rows
                    where row.IsChecked
                    select row.Vm).ToArray();
            }
        }

        private bool SelectedVmsExist
        {
            get
            {
                foreach (SourceVmRow row in dataGridViewVms.Rows)
                {
                    if (row.IsChecked)
                        return true;
                }

                return false;
            }
        }

        private void Build()
        {
            try
            {
                dataGridViewVms.SuspendLayout();
                dataGridViewVms.Rows.Clear();

                if (VMwareVMs == null)
                    return;

                foreach (VmInstance vm in VMwareVMs)
                {
                    if (vm.PowerState == (int)VmPowerState.Off)
                        dataGridViewVms.Rows.Add(new SourceVmRow(vm));   
                }
            }
            finally 
            {
                dataGridViewVms.ResumeLayout();
                UpdateButtons();
            }
        }

        private void UpdateButtons()
        {
            _buttonNextEnabled = SelectedVmsExist;
            OnPageUpdated();
        }

        private void BulkCheck(bool check)
        {
            try
            {
                _updating = true;

                foreach (SourceVmRow row in dataGridViewVms.Rows)
                    row.IsChecked = check;
            }
            finally
            {
                _updating = false;
                UpdateButtons();
            }
        }

        #region Control event handlers

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = ConversionClient.GetSourceVMs(VmwareCredInfo);
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                tableLayoutPanelError.Visible = false;
            }
            if (e.Error != null)
            {
                log.Error(e.Error);
                pictureBoxError.Image = Images.StaticImages._000_error_h32bit_16;
                labelError.Text = Messages.CONVERSION_CONNECTING_VMWARE_FAILURE;
            }
            else
            {
                VMwareVMs = e.Result as VmInstance[];
                tableLayoutPanelError.Visible = false;
                Build();
            }

            UpdateButtons();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            dataGridViewVms.Rows.Clear();
            pictureBoxError.Image = Images.StaticImages.ajax_loader;
            labelError.Text = Messages.CONVERSION_CONNECTING_VMWARE;
            tableLayoutPanelError.Visible = true;
            VMwareVMs = null;

            _backgroundWorker.RunWorkerAsync();
            UpdateButtons();
        }

        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            BulkCheck(true);
        }

        private void buttonClearAll_Click(object sender, EventArgs e)
        {
            BulkCheck(false);
        }

        private void dataGridViewVms_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridViewVms.IsCurrentCellDirty)
                dataGridViewVms.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dataGridViewVms_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_updating)
                return;

            if (e.ColumnIndex != 0 || e.RowIndex < 0 || e.RowIndex >= dataGridViewVms.RowCount)
                return;

            UpdateButtons();
        }

        #endregion

        private class SourceVmRow : DataGridViewRow
        {
            private readonly DataGridViewCheckBoxCell cellCheck = new DataGridViewCheckBoxCell();
            private readonly DataGridViewTextBoxCell cellVm = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell cellOs = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell cellDiskSize = new DataGridViewTextBoxCell();

            public SourceVmRow(VmInstance vm)
            {
                Cells.AddRange(cellCheck, cellVm, cellOs, cellDiskSize);

                Vm = vm;
                cellCheck.Value = false;
                cellVm.Value = vm.Template ? string.Format(Messages.CONVERSION_TEMPLATE, vm.Name) : vm.Name;
                cellOs.Value = vm.OSType;
                cellDiskSize.Value = Util.DiskSizeString(vm.CommittedStorage + vm.UncommittedStorage);
            }

            public bool IsChecked
            {
                get => (bool)cellCheck.Value;
                set => cellCheck.Value = value;
            }

            public VmInstance Vm { get; }
        }
    }
}
