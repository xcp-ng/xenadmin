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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Actions;

using XenAPI;
using XenAdmin.Dialogs;
using XenAdmin.Commands;
using XenCenterLib;

namespace XenAdmin.TabPages
{
    internal partial class SrStoragePage : BaseTabPage
    {
        private SR sr;
        private bool rebuildRequired;

        private readonly VDIsDataGridViewBuilder dataGridViewBuilder;

        public SrStoragePage()
        {
            InitializeComponent();

            for (int i = 0; i < 5; i++)
            {
                dataGridViewVDIs.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
            }

            ConnectionsManager.History.CollectionChanged += History_CollectionChanged;
            base.Text = Messages.VIRTUAL_DISKS;
            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
            dataGridViewBuilder = new VDIsDataGridViewBuilder(this);
        }

        public override string HelpID => "TabPageStorage";

        private bool disposed;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                // Deregister listeners.
                SR = null;

                ConnectionsManager.History.CollectionChanged -= History_CollectionChanged;
                Properties.Settings.Default.PropertyChanged -= Default_PropertyChanged;

                if (disposing)
                {
                    if (components != null)
                        components.Dispose();

                    dataGridViewBuilder.Stop();
                }

                disposed = true;
            }
            base.Dispose(disposing);
        }

        private void SetupDeprecationBanner()
        {
            if (sr != null && SR.IsIslOrIslLegacy(sr))
            {
                Banner.AppliesToVersion = string.Format(Messages.XENSERVER_6_5, BrandManager.ProductVersion65);
                Banner.BannerType = DeprecationBanner.Type.Removal;
                Banner.FeatureName = Messages.ISL_SR;
                Banner.LinkUri = HiddenFeatures.LinkLabelHidden ? null : new Uri(InvisibleMessages.ISL_DEPRECATION_URL);
                Banner.Visible = !HiddenFeatures.LinkLabelHidden;
            }
            else
                Banner.Visible = false;
        }

        public SR SR
        {
            set
            {
                Program.AssertOnEventThread();

                if (sr != null)
                {
                    sr.PropertyChanged -= sr_PropertyChanged;
                    sr.Connection.Cache.DeregisterBatchCollectionChanged<VDI>(VDI_BatchCollectionChanged);
                    sr.Connection.XenObjectsUpdated -= Connection_XenObjectsUpdated;
                }

                sr = value;
                
                if (sr != null)
                {
                    sr.PropertyChanged += sr_PropertyChanged;
                    sr.Connection.Cache.RegisterBatchCollectionChanged<VDI>(VDI_BatchCollectionChanged);
                    addVirtualDiskButton.Visible = sr.SupportsVdiCreate();
                    sr.Connection.XenObjectsUpdated += Connection_XenObjectsUpdated;
                }

                BuildList(true);
                SetupDeprecationBanner();
            }
        }

        private void RefreshDataGridView(VDIsData data)
        {
            dataGridViewVDIs.SuspendLayout();
            try
            {
                ColumnVolume.Visible = data.ShowStorageLink;
                var showCbtColumn = !Helpers.FeatureForbidden(sr.Connection, Host.RestrictChangedBlockTracking);
                ColumnCBT.Visible = showCbtColumn;

                // Update existing rows
                foreach (var vdiRow in data.VdiRowsToUpdate)
                {
                    vdiRow.RefreshRowDetails(showCbtColumn);
                }

                // Remove rows for deleted VDIs
                foreach (var vdiRow in data.VdiRowsToRemove)
                {
                    dataGridViewVDIs.Rows.RemoveAt(vdiRow.Index);
                }

                // Add rows for new VDIs
                foreach (var vdi in data.VdisToAdd)
                {
                    dataGridViewVDIs.Rows.Add(new VDIRow(vdi, showCbtColumn));   
                }
            }
            finally
            {
                if (dataGridViewVDIs.SortedColumn != null && dataGridViewVDIs.SortOrder != SortOrder.None)
                    dataGridViewVDIs.Sort(dataGridViewVDIs.SortedColumn,
                                          dataGridViewVDIs.SortOrder == SortOrder.Ascending
                                              ? ListSortDirection.Ascending
                                              : ListSortDirection.Descending);

                dataGridViewVDIs.ResumeLayout();
            }

            RefreshButtons();
        }

        private IEnumerable<VDIRow> GetCurrentVDIRows()
        {
            return dataGridViewVDIs.Rows.OfType<VDIRow>();
        }

        private void BuildList(bool reset)
        {
            Program.AssertOnEventThread();

            if (sr == null)
                return;

            dataGridViewBuilder.AddRequest(new RefreshGridRequest(sr, reset));
        }

        private SelectedItemCollection SelectedVDIs
        {
            get
            {
                List<SelectedItem> vdis = new List<SelectedItem>();
                foreach (DataGridViewRow r in dataGridViewVDIs.SelectedRows)
                {
                    VDIRow row = r as VDIRow;
                    if (row != null)
                        vdis.Add(new SelectedItem(row.VDI));
                }
                return new SelectedItemCollection(vdis);
            }
        }

        private void UnregisterHandlers()
        {
            ConnectionsManager.History.CollectionChanged -= History_CollectionChanged;
            Properties.Settings.Default.PropertyChanged -= Default_PropertyChanged;

            if (sr != null)
            {
                sr.PropertyChanged -= sr_PropertyChanged;
                sr.Connection.Cache.DeregisterBatchCollectionChanged<VDI>(VDI_BatchCollectionChanged);
                sr.Connection.XenObjectsUpdated -= Connection_XenObjectsUpdated;
            }
        }

        public override void PageHidden()
        {
            UnregisterHandlers();
        }

        #region events
        void History_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.BeginInvoke(Program.MainWindow, () =>
            {
                SrRefreshAction a = e.Element as SrRefreshAction;

                if (e.Action == CollectionChangeAction.Add)
                {
                    if (a != null)
                        a.Completed += a_Completed;
                    else
                        return;
                }

                if (e.Action == CollectionChangeAction.Remove)
                {
                    if (a != null)
                    {
                        a.Completed -= a_Completed;
                    }
                    else
                    {
                        var range = e.Element as List<SrRefreshAction>;
                        if (range != null)
                        {
                            foreach (var sra in range)
                                sra.Completed -= a_Completed;
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                RefreshButtons();
            });
        }

        void a_Completed(ActionBase sender)
        {
            Program.Invoke(Program.MainWindow, RefreshButtons);
        }

        void Connection_XenObjectsUpdated(object sender, EventArgs e)
        {
            if (rebuildRequired)
                BuildList(false);

            rebuildRequired = false;
        }

        void sr_PropertyChanged(object sender1, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VDIs")
                rebuildRequired = true;
        }

        private void VDI_BatchCollectionChanged(object sender, EventArgs e)
        {
            rebuildRequired = true;
        }

        private void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "ShowHiddenVMs")
                return;
            Program.Invoke(this, () => BuildList(false));
        }
        #endregion

        #region datagridvie wevents
        private void  DataGridViewObject_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == ColumnName.Index)
            {
                var vdi1 = ((VDIRow) dataGridViewVDIs.Rows[e.RowIndex1]).VDI;
                var vdi2 = ((VDIRow) dataGridViewVDIs.Rows[e.RowIndex2]).VDI;

                e.SortResult = vdi1.CompareTo(vdi2);
                e.Handled = true;
                return;
            }

            if (e.Column.Index == ColumnDesc.Index)
            {
                var vdi1 = ((VDIRow)dataGridViewVDIs.Rows[e.RowIndex1]).VDI;
                var vdi2 = ((VDIRow)dataGridViewVDIs.Rows[e.RowIndex2]).VDI;

                var descCompare = StringUtility.NaturalCompare(vdi1.Description(), vdi2.Description());
                if (descCompare != 0)
                {
                    e.SortResult = descCompare;
                }
                else
                {
                    var refCompare = string.Compare(vdi1.opaque_ref, vdi2.opaque_ref, StringComparison.Ordinal);
                    e.SortResult = refCompare;
                }
                e.Handled = true;
                return;
            }

            if (e.Column.Index == ColumnSize.Index)
            {
                VDI vdi1 = ((VDIRow)dataGridViewVDIs.Rows[e.RowIndex1]).VDI;
                VDI vdi2 = ((VDIRow)dataGridViewVDIs.Rows[e.RowIndex2]).VDI;
                long diff = vdi1.virtual_size - vdi2.virtual_size;
                e.SortResult =
                    diff > 0 ? 1 :
                    diff < 0 ? -1 :
                               0;
                e.Handled = true;
            }
        }

        private void dataGridViewVDIs_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshButtons();
        }

        private void dataGridViewVDIs_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Apps)
                return;

            if (dataGridViewVDIs.SelectedRows.Count == 0)
            {
                // 3 is the defaul control margin
                contextMenuStrip1.Show(dataGridViewVDIs, 3, dataGridViewVDIs.ColumnHeadersHeight + 3);
            }
            else
            {
                DataGridViewRow row = dataGridViewVDIs.SelectedRows[0];
                contextMenuStrip1.Show(dataGridViewVDIs, 3, row.Height * (row.Index + 2));
            }
        }

        private void dataGridViewVDIs_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hitTestInfo = dataGridViewVDIs.HitTest(e.X, e.Y);

            if (hitTestInfo.Type == DataGridViewHitTestType.None)
            {
                dataGridViewVDIs.ClearSelection();
            }
            else if (hitTestInfo.Type == DataGridViewHitTestType.Cell && e.Button == MouseButtons.Right
                     && 0 <= hitTestInfo.RowIndex && hitTestInfo.RowIndex < dataGridViewVDIs.Rows.Count
                     && !dataGridViewVDIs.Rows[hitTestInfo.RowIndex].Selected)
            {
                // Select the row that the user right clicked on (similiar to outlook) if it's not already in the selection
                // (avoids clearing a multiselect if you right click inside it)
                // Check if the CurrentCell is the cell the user right clicked on (but the row is not Selected) [CA-64954]
                // This happens when the grid is initially shown: the current cell is the first cell in the first column, but the row is not selected

                if (dataGridViewVDIs.CurrentCell == dataGridViewVDIs[hitTestInfo.ColumnIndex, hitTestInfo.RowIndex])
                    dataGridViewVDIs.Rows[hitTestInfo.RowIndex].Selected = true;
                else
                    dataGridViewVDIs.CurrentCell = dataGridViewVDIs[hitTestInfo.ColumnIndex, hitTestInfo.RowIndex];
            }

            if ((hitTestInfo.Type == DataGridViewHitTestType.None || hitTestInfo.Type == DataGridViewHitTestType.Cell)
                && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(dataGridViewVDIs, new Point(e.X, e.Y));
            }
        }

        #endregion

        #region Button and context menu population
        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            bool rescan = buttonRescan.Enabled;
            bool add = addVirtualDiskButton.Enabled;
            bool move = buttonMove.Enabled;
            bool delete = RemoveButton.Enabled;
            bool edit = EditButton.Enabled;
            
            if (!(rescan || add || move || delete || edit))
            {
                e.Cancel = true;
                return;
            }

            rescanToolStripMenuItem.Visible = rescan;
            addToolStripMenuItem.Visible = add;
            moveVirtualDiskToolStripMenuItem.Visible = move;
            deleteVirtualDiskToolStripMenuItem.Visible = delete;
            editVirtualDiskToolStripMenuItem.Visible = edit;
            toolStripSeparator1.Visible = (rescan || add || move || delete) && edit;
        }

        

        private void RefreshButtons()
        {
            SelectedItemCollection vdis = SelectedVDIs;

            // Delete button
            // The user can see that this disk is attached to more than one VMs. Allow deletion of multiple VBDs (non default behaviour),
            // but don't allow them to be deleted if a running vm is using the disk (default behaviour).

            DeleteVirtualDiskCommand deleteCmd = new DeleteVirtualDiskCommand(Program.MainWindow, vdis) {AllowMultipleVBDDelete = true};
            if (deleteCmd.CanExecute())
            {
                RemoveButton.Enabled = true;
                RemoveButtonContainer.RemoveAll();
            }
            else
            {
                RemoveButton.Enabled = false;
                RemoveButtonContainer.SetToolTip(deleteCmd.ToolTipText);
            }

            // Move button
            Command moveCmd = MoveVirtualDiskDialog.MoveMigrateCommand(Program.MainWindow, vdis);
            if (moveCmd.CanExecute())
            {
                buttonMove.Enabled = true;
                toolTipContainerMove.RemoveAll();
            }
            else
            {
                buttonMove.Enabled = false;
                toolTipContainerMove.SetToolTip(moveCmd.ToolTipText);
            }

            // Rescan button
            if (sr == null || sr.Locked)
            {
                buttonRescan.Enabled = false;
            }
            else if (HelpersGUI.BeingScanned(sr))
            {
                buttonRescan.Enabled = false;
                toolTipContainerRescan.SetToolTip(Messages.SCAN_IN_PROGRESS_TOOLTIP);
            }
            else
            {
                buttonRescan.Enabled = true;
                toolTipContainerRescan.RemoveAll();
            }

            // Add VDI button
            addVirtualDiskButton.Enabled = sr != null && !sr.Locked;

            // Properties button
            if (vdis.Count == 1)
            {
                VDI vdi = vdis.AsXenObjects<VDI>()[0];
                EditButton.Enabled = sr != null && !sr.Locked && !vdi.is_a_snapshot && !vdi.Locked;
            }
            else
                EditButton.Enabled = false;
        }

        #endregion

        #region Actions on Vdis

        private void Rescan()
        {
            SrRefreshAction a = new SrRefreshAction(sr);
            a.RunAsync();
        }

        private void AddVdi()
        {
            if (sr != null)
                Program.MainWindow.ShowPerConnectionWizard(sr.Connection, new NewDiskDialog(sr.Connection, sr));
        }

        private void MoveSelectedVdis()
        {
            SelectedItemCollection vdis = SelectedVDIs;
            Command cmd = MoveVirtualDiskDialog.MoveMigrateCommand(Program.MainWindow, vdis);
            if (cmd.CanExecute())
                cmd.Execute();
        }

        private void RemoveSelectedVdis()
        {
            SelectedItemCollection vdis = SelectedVDIs;
            DeleteVirtualDiskCommand cmd = new DeleteVirtualDiskCommand(Program.MainWindow, vdis) {AllowMultipleVBDDelete = true};
            if (cmd.CanExecute())
                cmd.Execute();
        }

        private void EditSelectedVdis()
        {
            SelectedItemCollection vdis = SelectedVDIs;
            if (vdis.Count != 1)
                return;

            VDI vdi = vdis.AsXenObjects<VDI>()[0];
            if (vdi.is_a_snapshot)
                return;

            new PropertiesDialog(vdi).ShowDialog(this);
        }

        #endregion

        #region Button and ToolStripMenuItem handlers

        private void addVirtualDiskButton_Click(object sender, EventArgs e)
        {
            AddVdi();
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            RemoveSelectedVdis();
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            EditSelectedVdis();
        }

        private void buttonRescan_Click(object sender, EventArgs e)
        {
            Rescan();
        }

        private void buttonMove_Click(object sender, EventArgs e)
        {
            MoveSelectedVdis();
        }


        private void rescanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rescan();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddVdi();
        }

        private void editVirtualDiskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditSelectedVdis();
        }

        private void moveVirtualDiskToolStripMenuItem_Click(object sender, EventArgs e)
        {
           MoveSelectedVdis();
        }

        private void deleteVirtualDiskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedVdis();
        }

        #endregion


        public class VDIRow : DataGridViewRow
        {
            public VDI VDI { get; private set; }

            public VDIRow(VDI vdi, bool show_cbt)
            {
                VDI = vdi;
                for (int i = 0; i < 5; i++)
                {
                    Cells.Add(new DataGridViewTextBoxCell());
                    Cells[i].Value = GetCellText(i);
                }
                if (show_cbt)
                {
                    Cells.Add(new DataGridViewTextBoxCell());
                    Cells[5].Value = GetCellText(5);
                }
            }

            private string GetCellText(int cellIndex)
            {
                switch (cellIndex)
                {
                    case 0:
                        return VDI.Name();
                    case 1:
                        string name;
                        return VDI.sm_config.TryGetValue("displayname", out name) ? name : "";
                    case 2:
                        return VDI.Description();
                    case 3:
                        return VDI.SizeText();
                    case 4:
                        return VDI.VMsOfVDI();
                    case 5:
                        return VDI.cbt_enabled ? Messages.ENABLED : Messages.DISABLED;
                    default:
                        return "";
                }
            }

            public void RefreshRowDetails(bool show_cbt)
            {
                for (int i = 0; i < 5; i++)
                {
                    Cells[i].Value = GetCellText(i);
                }
                if (show_cbt)
                    Cells[5].Value = GetCellText(5);
            }
        }

        public struct VDIsData
        {
            public List<VDIRow> VdiRowsToUpdate { get; private set; }
            public List<VDIRow> VdiRowsToRemove { get; private set; }
            public List<VDI> VdisToAdd { get; private set; }
            public bool ShowStorageLink { get; private set; }

            public VDIsData(List<VDIRow> vdiRowsToUpdate, List<VDIRow> vdiRowsToRemove, List<VDI> vdisToAdd,
                bool showStorageLink) : this()
            {
                VdiRowsToUpdate = vdiRowsToUpdate;
                VdiRowsToRemove = vdiRowsToRemove;
                VdisToAdd = vdisToAdd;
                ShowStorageLink = showStorageLink;
            }
        }

        public class RefreshGridRequest
        {
            public SR SR { get; private set; }
            public bool Reset { get; private set; }

            public RefreshGridRequest(SR sr, bool reset)
            {
                SR = sr;
                Reset = reset;
            }
        }

        public class VDIsDataGridViewBuilder
        {
            private readonly Control owner;
            private readonly object _locker = new object();
            private BackgroundWorker worker;
            private Queue<RefreshGridRequest> queue = new Queue<RefreshGridRequest>();

            public VDIsDataGridViewBuilder(Control owner)
            {
                this.owner = owner;
                worker = new BackgroundWorker {WorkerSupportsCancellation = true};
                worker.DoWork += DoWork;
                worker.RunWorkerCompleted += (RunWorkerCompleted);
            }

            private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                if (e.Cancelled || e.Error != null)
                    return;

                lock (_locker)
                {
                    if (queue.Count > 0 && !worker.IsBusy)
                        worker.RunWorkerAsync();
                }
            }

            private VDIsData GetCurrentData(SR sr, IEnumerable<VDIRow> currentVDIRows)
            {
                List<VDI> vdis =
                        sr.Connection.ResolveAll(sr.VDIs).Where(
                            vdi =>
                            vdi.Show(Properties.Settings.Default.ShowHiddenVMs) &&
                            !vdi.IsAnIntermediateStorageMotionSnapshot())
                            .ToList();

                bool showStorageLink = vdis.Find(v => v.sm_config.ContainsKey("SVID")) != null;

                var vdiRowsToRemove =
                    currentVDIRows.Where(vdiRow => !vdis.Contains(vdiRow.VDI)).OrderByDescending(row => row.Index).ToList();

                var vdiRowsToUpdate = currentVDIRows.Except(vdiRowsToRemove).ToList();

                var vdisToAdd = vdis.Except(vdiRowsToUpdate.ConvertAll(vdiRow => vdiRow.VDI)).ToList();

                return new VDIsData(vdiRowsToUpdate, vdiRowsToRemove, vdisToAdd, showStorageLink);
            }

            private void DoWork(object sender, DoWorkEventArgs e)
            {
                RefreshGridRequest refreshRequest;
                lock (_locker)
                {
                    refreshRequest = queue.Count > 0 ? queue.Dequeue() : null;
                }

                if (worker.CancellationPending)
                    return;

                if (refreshRequest == null || refreshRequest.SR == null)
                    return;

                SrStoragePage page = owner as SrStoragePage;
                if (page == null)
                    return;

                if (refreshRequest.Reset)
                    Program.Invoke(owner, page.dataGridViewVDIs.Rows.Clear);

                Program.Invoke(owner, page.RefreshButtons);

                IEnumerable<VDIRow> currentVDIRows = Enumerable.Empty<VDIRow>();
                Program.Invoke(owner, () => currentVDIRows = page.GetCurrentVDIRows());
                VDIsData data = GetCurrentData(refreshRequest.SR, currentVDIRows);

                if (worker.CancellationPending)
                    return;

                Program.Invoke(owner, () => ((SrStoragePage)owner).RefreshDataGridView(data));
            }

            public void AddRequest(RefreshGridRequest refreshGridRequest)
            {
                lock (_locker)
                {
                    if (refreshGridRequest.Reset)
                        queue.Clear();

                    queue.Enqueue(refreshGridRequest);

                    if (!worker.IsBusy)
                        worker.RunWorkerAsync();
                }
            }

            public void Stop()
            {
                if (!worker.CancellationPending)
                    worker.CancelAsync();
            }
        }
    }
}
