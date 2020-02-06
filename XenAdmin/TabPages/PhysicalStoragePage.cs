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
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Commands;
using XenAdmin.Dialogs;
using XenAdmin.Controls.DataGridViewEx;
using XenCenterLib;


namespace XenAdmin.TabPages
{
    public partial class PhysicalStoragePage : BaseTabPage
    {
        private IXenConnection connection;
        private Host host;

        private bool NeedBuildList;

        private SelectionManager selectionManager;
        private bool showTrimButton;

        public PhysicalStoragePage()
        {
            InitializeComponent();

            base.Text = Messages.STORAGE_TAB_TITLE;
            PBD_CollectionChangedWithInvoke=Program.ProgramInvokeHandler(PBD_CollectionChanged);

        }

        public override string HelpID => "TabPageStorage";

        /// <summary>
        /// Make sure you set this before you set the connection, 
        /// as the connection is the one which rebuilds the list
        /// </summary>
        public Host Host
        {
            set
            {
                if (host != null)
                {
                    host.PropertyChanged -= host_PropertyChanged;
                }

                host = value;

                if (host != null)
                {
                    host.PropertyChanged += host_PropertyChanged;
                }
            }
        }

        private readonly CollectionChangeEventHandler PBD_CollectionChangedWithInvoke;
        public IXenConnection Connection
        {
            set
            {
                UnregisterHandlers();

                connection = value;

                if (connection != null)
                {
                    connection.Cache.RegisterCollectionChanged<PBD>(PBD_CollectionChangedWithInvoke);
                    connection.XenObjectsUpdated += XenObjectUpdated;

                    Pool pool = Helpers.GetPoolOfOne(connection);
                    if (pool != null)
                        pool.PropertyChanged += pool_PropertyChanged;
                }

                RefreshTrimButton();
                BuildList();
            }
        }

        internal void SetSelectionBroadcaster(SelectionBroadcaster selectionBroadcaster, IMainWindow mainWindow)
        {
            selectionBroadcaster.BindTo(newSRButton, mainWindow);
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (dataGridViewSr.SelectedRows.Count != 1)
            {
                e.Cancel = true;
                return;
            }

            contextMenuStrip.Items.Clear();

            var row = dataGridViewSr.SelectedRows[0] as SRRow;
            if (row != null)
            {
                contextMenuStrip.Items.AddRange(Program.MainWindow.ContextMenuBuilder.Build(row.SR));
            }
            
        }

        private void XenObjectUpdated(object sender, EventArgs e)
        {
            if (NeedBuildList)
            {
                BuildList();
            }
        }

        private void PBD_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.AssertOnEventThread();

            NeedBuildList = true;
            BuildList();
        }

        private void pool_PropertyChanged(object sender1, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "default_SR")
                NeedBuildList = true;
        }

        private void host_PropertyChanged(object sender1, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PBDs")
                NeedBuildList = true;
        }

        private void sr_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PBDs")
            {
                NeedBuildList = true;
            }
            else
            {
                Program.Invoke(Program.MainWindow, () => RefreshItem(sender as SR));
            }
        }

        private void RefreshItem(SR sr=null)
        {
            if (sr == null)
                return;

            foreach (DataGridViewRow row in dataGridViewSr.Rows)
            {
                var srRow = row as SRRow;
                if (srRow != null && sr.Equals(srRow.SR))
                {
                    srRow.UpdateDetails();
                    break;
                }
            }
        }

        private void UnregisterHandlers()
        {
            if (connection == null) 
                return;

            connection.Cache.DeregisterCollectionChanged<PBD>(PBD_CollectionChangedWithInvoke);
            connection.XenObjectsUpdated -= XenObjectUpdated;

            foreach (SR sr in connection.Cache.SRs)
            {
                sr.PropertyChanged -= sr_PropertyChanged;
            }

            Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool != null)
                pool.PropertyChanged -= pool_PropertyChanged;
        }

        public override void PageHidden()
        {
            UnregisterHandlers();
        }

        private void BuildList()
        {
            if (!this.Visible)
                return;
            int selectedIndex = dataGridViewSr.SelectedRows.Count == 1 ? dataGridViewSr.SelectedRows[0].Index : -1;

            dataGridViewSr.SuspendLayout();

            try
            {
                dataGridViewSr.Rows.Clear();

                if (connection == null)
                    return;

                List<PBD> pbds = host != null ? new List<PBD>(connection.ResolveAll(host.PBDs))
                    : new List<PBD>(connection.Cache.PBDs);

                List<String> srs = new List<String>();

                foreach (PBD pbd in pbds)
                {
                    SR sr = pbd.Connection.Resolve(pbd.SR);

                    if (sr == null || sr.IsToolsSR() || !sr.Show(Properties.Settings.Default.ShowHiddenVMs))
                        continue;

                    // From MSDN:
                    // Returns the zero-based index of item in the sorted List<T>, if item is found;
                    // otherwise, a negative number that is the bitwise complement of the index of
                    // the next element that is larger than item or, if there is no larger element,
                    // the bitwise complement of Count. 
                    int index = srs.BinarySearch(sr.opaque_ref);
                    
                    // Don't allow duplicates
                    if (index >= 0)
                        continue;

                    sr.PropertyChanged -= sr_PropertyChanged;
                    sr.PropertyChanged += sr_PropertyChanged;

                    index = ~index;
                    srs.Insert(index, sr.opaque_ref);

                    SRRow row = new SRRow(sr);
                    dataGridViewSr.Rows.Add(row);
                }

                if (selectedIndex >= 0 && selectedIndex < dataGridViewSr.Rows.Count)
                {
                    // Select previously selected item
                    dataGridViewSr.Rows[selectedIndex].Selected = true;
                }
                else
                {
                    // Select first item
                    if (dataGridViewSr.Rows.Count > 0)
                        dataGridViewSr.Rows[0].Selected = true;
                }
            }
            finally
            {
                dataGridViewSr.ResumeLayout();
            }
            RefreshButtons();
            NeedBuildList = false;
        }

        private void dataGridViewSr_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hitTestInfo = dataGridViewSr.HitTest(e.X, e.Y);

            if (hitTestInfo.Type == DataGridViewHitTestType.None)
            {
                dataGridViewSr.ClearSelection();
            }
            else if (hitTestInfo.Type == DataGridViewHitTestType.Cell && e.Button == MouseButtons.Right
                     && 0 <= hitTestInfo.RowIndex && hitTestInfo.RowIndex < dataGridViewSr.Rows.Count
                     && !dataGridViewSr.Rows[hitTestInfo.RowIndex].Selected)
            {
                if (dataGridViewSr.CurrentCell == dataGridViewSr[hitTestInfo.ColumnIndex, hitTestInfo.RowIndex])
                    dataGridViewSr.Rows[hitTestInfo.RowIndex].Selected = true;
                else
                    dataGridViewSr.CurrentCell = dataGridViewSr[hitTestInfo.ColumnIndex, hitTestInfo.RowIndex];
            }

            if ((hitTestInfo.Type == DataGridViewHitTestType.None || hitTestInfo.Type == DataGridViewHitTestType.Cell)
                && e.Button == MouseButtons.Right)
            {
                contextMenuStrip.Show(dataGridViewSr, new Point(e.X, e.Y));
            }
        }

        private void dataGridViewSr_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            var sr1 = ((SRRow)dataGridViewSr.Rows[e.RowIndex1]).SR;
            var sr2 = ((SRRow)dataGridViewSr.Rows[e.RowIndex2]).SR;

            string sr1CompStr = null;
            string sr2CompStr = null;

            if (e.Column.Index == columnUsage.Index)
            {
                int percent1 = sr1.physical_size == 0 ? 0 : (int)(100.0 * sr1.physical_utilisation / (double)sr1.physical_size);
                int percent2 = sr2.physical_size == 0 ? 0 : (int)(100.0 * sr2.physical_utilisation / (double)sr2.physical_size);
                long diff = percent1 - percent2;
                e.SortResult = diff > 0 ? 1 : diff < 0 ? -1 : 0;
                e.Handled = true;
                return;
            }
            else if (e.Column.Index == columnSize.Index)
            {
                long diff = sr1.physical_size - sr2.physical_size;
                e.SortResult = diff > 0 ? 1 : diff < 0 ? -1 : 0;
                e.Handled = true;
                return;
            }
            else if (e.Column.Index == columnVirtAlloc.Index)
            {
                long diff = sr1.virtual_allocation - sr2.virtual_allocation;
                e.SortResult = diff > 0 ? 1 : diff < 0 ? -1 : 0;
                e.Handled = true;
                return;
            }
            else if (e.Column.Index == columnName.Index)
            {
                sr1CompStr = sr1.Name();
                sr2CompStr = sr2.Name();
            }
            else if (e.Column.Index == columnDescription.Index)
            {
                sr1CompStr = sr1.Description();
                sr2CompStr = sr2.Description();
            }

            if (sr1CompStr != null && sr2CompStr != null)
            {
                var descCompare = StringUtility.NaturalCompare(sr1CompStr, sr2CompStr);
                if (descCompare != 0)
                {
                    e.SortResult = descCompare;
                }
                else
                {
                    var refCompare = string.Compare(sr1.opaque_ref, sr2.opaque_ref, StringComparison.Ordinal);
                    e.SortResult = refCompare;
                }
                e.Handled = true;
            }
        }

        private void buttonProperties_Click(object sender, EventArgs e)
        {
            if (dataGridViewSr.SelectedRows.Count != 1)
                return;

            var srRow = dataGridViewSr.SelectedRows[0] as SRRow;

            if (srRow != null)
            {
                SR sr = srRow.SR;

                new PropertiesDialog(sr).ShowDialog(this);
            }
        }

        void dataGridViewSrs_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshButtons();
            if (showTrimButton)
            {
                List<SelectedItem> selectedSRs = (from SRRow row in dataGridViewSr.SelectedRows select new SelectedItem(row.SR)).ToList();
                selectionManager.SetSelection(selectedSRs);
            }
        }

        private void RefreshButtons()
        {
            buttonProperties.Enabled = dataGridViewSr.SelectedRows.Count == 1;
        }

        private void RefreshTrimButton()
        {
            showTrimButton = connection != null;
            if (showTrimButton)
            {
                trimButtonContainer.Visible = true;
                if (selectionManager == null)
                    selectionManager = new SelectionManager();
                selectionManager.BindTo(trimButton, Program.MainWindow);
            }
            else
            {
                trimButtonContainer.Visible = false;
                trimButton.SelectionBroadcaster = null;
            }
        }

        protected class SRRow: DataGridViewRow
        {
            private SR sr;
            public SR SR { get { return sr; } }

            private DataGridViewExImageCell imageCell = new DataGridViewExImageCell();
            private DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell descriptionCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell typeCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell sharedCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell usageCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell sizeCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell virtAllocCell = new DataGridViewTextBoxCell();
            

            public SRRow(SR sr)
            {
                this.sr = sr;
                Cells.AddRange(imageCell,
                                   nameCell,
                                   descriptionCell,
                                   typeCell,
                                   sharedCell,
                                   usageCell,
                                   sizeCell,
                                   virtAllocCell);

                sr.PropertyChanged += sr_PropertyChanged;
                UpdateDetails();
            }

            public void UpdateDetails()
            {
                if (this.sr != null)
                {
                    // Work out percent usage
                    int percent = 0;
                    double ratio = 0;
                    if (this.sr.physical_size > 0)
                    {
                        ratio = this.sr.physical_utilisation / (double)this.sr.physical_size;
                        percent = (int)(100.0 * ratio);
                    }
                    string percentString = string.Format(Messages.DISK_PERCENT_USED, percent, Util.DiskSizeString(this.sr.physical_utilisation));

                    imageCell.Value = Images.GetImage16For(Images.GetIconFor(this.sr));
                    nameCell.Value = this.sr.Name();
                    descriptionCell.Value = this.sr.Description();
                    typeCell.Value = this.sr.FriendlyTypeName();
                    sharedCell.Value = this.sr.shared ? Messages.YES : Messages.NO;
                    usageCell.Value = percentString;
                    sizeCell.Value = Util.DiskSizeString(this.sr.physical_size);
                    virtAllocCell.Value = Util.DiskSizeString(this.sr.virtual_allocation);
                }
            }

            void sr_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                Program.Invoke(Program.MainWindow, UpdateDetails);
            }
        }
    }
}
