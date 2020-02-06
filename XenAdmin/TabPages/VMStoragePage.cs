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
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Commands;
using XenCenterLib;


namespace XenAdmin.TabPages
{
    internal partial class VMStoragePage : BaseTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly DataGridViewColumn storageLinkColumn;

        private VM vm;

        public VMStoragePage()
        {
            InitializeComponent();
            storageLinkColumn = ColumnSRVolume;

            Text = Messages.VIRTUAL_DISKS;
            dataGridViewStorage.Sort(ColumnDevicePosition, ListSortDirection.Ascending);
        }

        public override string HelpID => "TabPageStorage";

        public VM VM
        {
            set
            {
                Program.AssertOnEventThread();

                UnregisterHandlers();

                vm = value;
                multipleDvdIsoList1.VM = vm;

                if (vm != null)
                    vm.PropertyChanged += vm_PropertyChanged;

                BuildList();

                // set all columns to be preferred width but allow user to resize.
                foreach (DataGridViewTextBoxColumn col in dataGridViewStorage.Columns)
                {
                    if (col != storageLinkColumn)
                    {
                        col.Width = col.GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, false);
                    }
                }
            }
        }

        private void vdi_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Nothing changing on any of the VDIs will require a rebuild of the list
            Program.Invoke(this, () =>
            {
                if (e.PropertyName == "allowed_operations")
                    UpdateButtons();
                else
                    UpdateData();
            });
        }

        private void vbd_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Only rebuild list if VDIs changed - otherwise just refresh

            Program.Invoke(this, () =>
            {
                if (e.PropertyName == "VDI" || e.PropertyName == "empty" || e.PropertyName == "device")
                {
                    BuildList();
                }
                else if (e.PropertyName == "allowed_operations")
                {
                    UpdateButtons();
                }
                else if (e.PropertyName == "currently_attached")
                {
                    UpdateButtons();
                    UpdateData();
                }
                else
                {
                    UpdateData();
                }
            });
        }

        private void UnregisterHandlers()
        {
            if (vm == null) 
                return;

            foreach (VBD vbd in vm.Connection.ResolveAll(vm.VBDs))
            {
                vbd.PropertyChanged -= vbd_PropertyChanged;

                VDI vdi = vm.Connection.Resolve(vbd.VDI);
                if (vdi == null)
                    continue;

                vdi.PropertyChanged -= vdi_PropertyChanged;
            }

            vm.PropertyChanged -= vm_PropertyChanged;

            multipleDvdIsoList1.DeregisterEvents();
        }

        public override void PageHidden()
        {
            UnregisterHandlers();
        }

        private void UpdateData()
        {
            try
            {
                dataGridViewStorage.SuspendLayout();
                foreach (DataGridViewRow r in dataGridViewStorage.Rows)
                {
                    VBDRow row = r as VBDRow;
                    row.UpdateCells();
                }
                dataGridViewStorage.Sort(dataGridViewStorage.SortedColumn,
                                         dataGridViewStorage.SortOrder == SortOrder.Ascending
                                             ? ListSortDirection.Ascending
                                             : ListSortDirection.Descending);

            }
            catch (Exception e)
            {
                log.Error("Error updating vm storage list.", e);
            }
            finally
            {
                dataGridViewStorage.ResumeLayout();
            }
        }

        private void vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VBDs")
                Program.Invoke(this, BuildList);
            else if (e.PropertyName == "power_state" || e.PropertyName == "Locked")
                Program.Invoke(this, UpdateButtons);
        }

        private void BuildList()
        {
            Program.AssertOnEventThread();
            if (!this.Visible)
                return;
            try
            {
                dataGridViewStorage.SuspendLayout();
                DataGridViewSelectedRowCollection vbdSavedItems = dataGridViewStorage.SelectedRows;

                dataGridViewStorage.Rows.Clear();

                if (vm == null)
                    return;

                bool storageLinkColumnVisible = false;

                List<bool> devices_in_use = new List<bool>();
                foreach (VBD vbd in vm.Connection.ResolveAll(vm.VBDs))
                {
                    vbd.PropertyChanged -= vbd_PropertyChanged;
                    vbd.PropertyChanged += vbd_PropertyChanged;

                    if (!vbd.IsCDROM() && !vbd.IsFloppyDrive())
                    {
                        VDI vdi = vm.Connection.Resolve(vbd.VDI);
                        if (vdi == null || !vdi.Show(Properties.Settings.Default.ShowHiddenVMs))
                            continue;

                        SR sr = vm.Connection.Resolve(vdi.SR);
                        if (sr == null || sr.IsToolsSR())
                            continue;

                        storageLinkColumnVisible = vdi.sm_config.ContainsKey("SVID");

                        vdi.PropertyChanged -= vdi_PropertyChanged;
                        vdi.PropertyChanged += vdi_PropertyChanged;

                        dataGridViewStorage.Rows.Add(new VBDRow(vbd, vdi, sr));

                        int i;
                        if (int.TryParse(vbd.userdevice, out i))
                        {
                            while (devices_in_use.Count <= i)
                            {
                                devices_in_use.Add(false);
                            }
                            devices_in_use[i] = true;
                        }
                    }

					//CA-47050: the dnsColumn should be autosized to Fill, but should not become smaller than a minimum
					//width, which is chosen to be the column's contents (including header) width. To find what this is
					//set temporarily the column's autosize mode to AllCells.
					HelpersGUI.ResizeGridViewColumnToAllCells(ColumnDevicePath);
                }


                storageLinkColumn.Visible = storageLinkColumnVisible;
                dataGridViewStorage.Sort(dataGridViewStorage.SortedColumn, dataGridViewStorage.SortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);

                IEnumerable<VBD> vbdsSelected = from VBDRow row in vbdSavedItems select row.VBD;
                foreach (VBDRow row in dataGridViewStorage.Rows)
                {
                    row.Selected = vbdsSelected.Contains(row.VBD);
                }
            }
            catch (Exception e)
            {
                log.Error("Exception while building the VM storage list.", e);
            }
            finally
            {
                dataGridViewStorage.ResumeLayout();
            }
            UpdateButtons();
        }

        private List<VBDRow> SelectedVBDRows
        {
            get
            {
                if (dataGridViewStorage.SelectedRows.Count == 0)
                    return null;

                List<VBDRow> rows = new List<VBDRow>();
                foreach (DataGridViewRow r in dataGridViewStorage.SelectedRows)
                    rows.Add(r as VBDRow);

                return rows;
            }
        }

        private void UpdateButtons()
        {
            AttachVirtualDiskCommand attachCmd = new AttachVirtualDiskCommand(Program.MainWindow, vm);
            AttachButton.Enabled = attachCmd.CanExecute();
            AddButton.Enabled = attachCmd.CanExecute();

            List<VBDRow> vbdRows = SelectedVBDRows;

            if (dataGridViewStorage.Rows.Count == 0 || vbdRows == null || vm == null)
            {
                DeactivateButton.Enabled = false;
                DetachButton.Enabled = false;
                DeleteButton.Enabled = false;
                EditButton.Enabled = false;
                MoveButton.Enabled = false;
                return;
            }
            EditButton.Enabled = vbdRows.Count == 1 && !vbdRows[0].VBD.Locked && !vbdRows[0].VDI.Locked;

            List<SelectedItem> selectedVDIs = new List<SelectedItem>();
            List<SelectedItem> selectedVBDs = new List<SelectedItem>();

            foreach (VBDRow r in vbdRows)
            {
                selectedVDIs.Add(new SelectedItem(r.VDI));
                selectedVBDs.Add(new SelectedItem(r.VBD));
            }
            DeleteVirtualDiskCommand deleteCmd = new DeleteVirtualDiskCommand(Program.MainWindow, selectedVDIs);
            // User has visibility that this disk in use by this VM. Allow unplug + delete in single step (non default behaviour),
            // but only if we are the only VBD (default behaviour)
            deleteCmd.AllowRunningVMDelete = true;
            if (deleteCmd.CanExecute())
            {
                DeleteButtonContainer.RemoveAll();
                DeleteButton.Enabled = true;
            }
            else
            {
                DeleteButtonContainer.SetToolTip(deleteCmd.ToolTipText);
                DeleteButton.Enabled = false;
            }

            Command activationCmd = null;

            SelectedItemCollection vbdCol = new SelectedItemCollection(selectedVBDs);
            if (vbdCol.AsXenObjects<VBD>().Find(delegate(VBD vbd) { return !vbd.currently_attached; }) == null)
            {
                // no VBDs are attached so we are deactivating
                toolStripMenuItemDeactivate.Text = DeactivateButton.Text = Messages.DEACTIVATE;
                activationCmd = new DeactivateVBDCommand(Program.MainWindow, selectedVBDs);
            }
            else
            {
                // this is the default cause in the mixed attached/detached scenario. We try to activate all the selection
                // The command error reports afterwards about the ones which are already attached
                toolStripMenuItemDeactivate.Text = DeactivateButton.Text = Messages.ACTIVATE;
                activationCmd = new ActivateVBDCommand(Program.MainWindow, selectedVBDs);
            }

            if (activationCmd.CanExecute())
            {
                DeactivateButtonContainer.RemoveAll();
                DeactivateButton.Enabled = true;
            }
            else
            {
                DeactivateButtonContainer.SetToolTip(activationCmd.ToolTipText);
                DeactivateButton.Enabled = false;
            }

            DetachVirtualDiskCommand detachCmd = new DetachVirtualDiskCommand(Program.MainWindow, selectedVDIs, vm);
            if (detachCmd.CanExecute())
            {
                DetachButtonContainer.RemoveAll();
                DetachButton.Enabled = true;
            }
            else
            {
                DetachButtonContainer.SetToolTip(detachCmd.ToolTipText);
                DetachButton.Enabled = false;
            }

            // Move button
            Command moveCmd = MoveVirtualDiskDialog.MoveMigrateCommand(Program.MainWindow, new SelectedItemCollection(selectedVDIs));
            if (moveCmd.CanExecute())
            {
                MoveButton.Enabled = true;
                MoveButtonContainer.RemoveAll();
            }
            else
            {
                MoveButton.Enabled = false;
                MoveButtonContainer.SetToolTip(moveCmd.ToolTipText);
            }
        }

        #region Actions on VDIs

        private void AddVdi()
        {
            var cmd = new AddVirtualDiskCommand(Program.MainWindow, vm);
            if (cmd.CanExecute())
                cmd.Execute();
        }

        private void AttachVdi()
        {
            AttachVirtualDiskCommand cmd = new AttachVirtualDiskCommand(Program.MainWindow, vm);
            if (cmd.CanExecute())
                cmd.Execute();
        }

        private void MoveVdi()
        {
            List<VBDRow> rows = SelectedVBDRows;
            if (rows == null)
                return;

            var vdis = (from VBDRow r in rows select new SelectedItem(r.VDI)).ToList();

            var cmd = MoveVirtualDiskDialog.MoveMigrateCommand(Program.MainWindow, new SelectedItemCollection(vdis));
            if (cmd.CanExecute())
                cmd.Execute();
        }

        private void DetachVdi()
        {
            List<VBDRow> rows = SelectedVBDRows;
            if (rows == null)
                return;
            List<SelectedItem> l = new List<SelectedItem>();
            foreach (VBDRow r in rows)
                l.Add(new SelectedItem(r.VDI));

            DetachVirtualDiskCommand cmd = new DetachVirtualDiskCommand(Program.MainWindow, l, vm);
            if (cmd.CanExecute())
                cmd.Execute();
        }

        private void DeleteVdi()
        {
            List<VBDRow> rows = SelectedVBDRows;
            if (rows == null)
                return;
            List<SelectedItem> l = new List<SelectedItem>();
            foreach (VBDRow r in rows)
                l.Add(new SelectedItem(r.VDI));

            DeleteVirtualDiskCommand cmd = new DeleteVirtualDiskCommand(Program.MainWindow, l);
            // User has visibility that this disk in use by this VM. Allow unplug + delete in single step (non default behaviour),
            // but only if we are the only VBD (default behaviour)
            cmd.AllowRunningVMDelete = true;
            if (cmd.CanExecute())
                cmd.Execute();
        }

        private void EditVdi()
        {
            if (vm == null)
                return;

            List<VBDRow> rows = SelectedVBDRows;

            if (rows == null)
                return;

            new PropertiesDialog(rows[0].VDI).ShowDialog(this);
        }

        private void ActivateDeactivateVdi()
        {
            List<VBDRow> rows = SelectedVBDRows;
            if (rows == null)
                return;

            var selection = from VBDRow r in rows select new SelectedItem(r.VBD);
            SelectedItemCollection col = new SelectedItemCollection(selection);

            Command cmd = null;
            if (col.AsXenObjects<VBD>().Find(vbd => !vbd.currently_attached) == null)
                cmd = new DeactivateVBDCommand(Program.MainWindow, col);
            else
                cmd = new ActivateVBDCommand(Program.MainWindow, col);

            if (cmd.CanExecute())
                cmd.Execute();
        }

        #endregion


        #region Datagridview event handlers

        private void dataGridViewStorage_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column == ColumnDevicePosition)
            {
                e.SortResult = StringUtility.NaturalCompare(e.CellValue1.ToString(), e.CellValue2.ToString());

                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void dataGridViewStorage_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void dataGridViewStorage_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && EditButton.Enabled)
            {
                // If double-click was on data row (not including header), launch the storage edit box
                EditVdi();
            }
        }

        private void dataGridViewStorage_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hitTestInfo = dataGridViewStorage.HitTest(e.X, e.Y);

            if (hitTestInfo.Type == DataGridViewHitTestType.None)
            {
                dataGridViewStorage.ClearSelection();
            }
            else if (hitTestInfo.Type == DataGridViewHitTestType.Cell && e.Button == MouseButtons.Right
                     && 0 <= hitTestInfo.RowIndex && hitTestInfo.RowIndex < dataGridViewStorage.Rows.Count
                     && !dataGridViewStorage.Rows[hitTestInfo.RowIndex].Selected)
            {
                // Select the row that the user right clicked on (similiar to outlook) if it's not already in the selection
                // (avoids clearing a multiselect if you right click inside it)
                // Check if the CurrentCell is the cell the user right clicked on (but the row is not Selected) [CA-64954]
                // This happens when the grid is initially shown: the current cell is the first cell in the first column, but the row is not selected

                if (dataGridViewStorage.CurrentCell == dataGridViewStorage[hitTestInfo.ColumnIndex, hitTestInfo.RowIndex])
                    dataGridViewStorage.Rows[hitTestInfo.RowIndex].Selected = true;
                else
                    dataGridViewStorage.CurrentCell = dataGridViewStorage[hitTestInfo.ColumnIndex, hitTestInfo.RowIndex];
            }

            if ((hitTestInfo.Type == DataGridViewHitTestType.None || hitTestInfo.Type == DataGridViewHitTestType.Cell)
                && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(dataGridViewStorage, new Point(e.X, e.Y));
            }
        }

        private void dataGridViewStorage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Apps)
                return;
            
            if (dataGridViewStorage.SelectedRows.Count == 0)
            {
                // 3 is the defaul control margin
                contextMenuStrip1.Show(dataGridViewStorage, 3, dataGridViewStorage.ColumnHeadersHeight + 3);
            }
            else
            {
                DataGridViewRow row = dataGridViewStorage.SelectedRows[0];
                contextMenuStrip1.Show(dataGridViewStorage, 3, row.Height * (row.Index + 2));
            }
        }

        #endregion


        #region Button and Toolstrip event handlers

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            bool add = AddButton.Visible && AddButton.Enabled;
            bool attach = AttachButton.Visible && AttachButton.Enabled;
            bool deactivate = DeactivateButton.Enabled;
            bool move = MoveButton.Enabled;
            bool delete = DeleteButton.Visible && DeleteButton.Enabled;
            bool detach = DetachButton.Visible && DetachButton.Enabled;
            bool edit = EditButton.Visible && EditButton.Enabled;

            if (!(add || attach || deactivate || move || delete || detach || edit))
            {
                e.Cancel = true;
                return;
            }

            toolStripMenuItemAdd.Visible = add;
            toolStripMenuItemAttach.Visible = attach;
            toolStripMenuItemDeactivate.Visible = deactivate;
            toolStripMenuItemMove.Visible = move;
            toolStripMenuItemDelete.Visible = delete;
            toolStripMenuItemDetach.Visible = detach;
            toolStripMenuItemProperties.Visible = edit;
            toolStripSeparator1.Visible = (add || attach || deactivate || move || delete || detach) && edit;
        }


        private void toolStripMenuItemAdd_Click(object sender, EventArgs e)
        {
            AddVdi();
            UpdateButtons();
        }

        private void toolStripMenuItemAttach_Click(object sender, EventArgs e)
        {
            AttachVdi();
            UpdateButtons();
        }

        private void toolStripMenuItemDeactivate_Click(object sender, EventArgs e)
        {
            ActivateDeactivateVdi();
        }

        private void toolStripMenuItemMove_Click(object sender, EventArgs e)
        {
            MoveVdi();
        }

        private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            DeleteVdi();
        }

        private void toolStripMenuItemDetach_Click(object sender, EventArgs e)
        {
            DetachVdi();
        }

        private void toolStripMenuItemProperties_Click(object sender, EventArgs e)
        {
            EditVdi();
        }

        
        private void AddButton_Click(object sender, EventArgs e)
        {
            AddVdi();
            UpdateButtons();
        }

        private void AttachButton_Click(object sender, EventArgs e)
        {
            AttachVdi();
            UpdateButtons();
        }

        private void DeactivateButton_Click(object sender, EventArgs e)
        {
            ActivateDeactivateVdi();
        }

        private void MoveButton_Click(object sender, EventArgs e)
        {
            MoveVdi();
        }

        private void DeleteDriveButton_Click(object sender, EventArgs e)
        {
            DeleteVdi();
        }

        private void DetachButton_Click(object sender, EventArgs e)
        {
            DetachVdi();
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            EditVdi();
        }

        #endregion
    }

    public class VBDRow : DataGridViewRow
    {
        public readonly VBD VBD;

        public readonly VDI VDI;

        public readonly SR SR;

        /// <summary>
        /// Arguments should never be null
        /// </summary>
        public VBDRow(VBD vbd, VDI vdi, SR sr)
        {
            Debug.Assert(vbd != null && vdi != null && sr != null, "vbd, vdi and sr must be set to a non-null value");

            VBD = vbd;
            VDI = vdi;
            SR = sr;

            for (int i = 0; i < 10; i++)
            {
                DataGridViewTextBoxCell c = new DataGridViewTextBoxCell();
                c.Value = CellValue(i);
                Cells.Add(c);
            }
        }

        public int CompareTo(object o)
        {
            VBDRow r = (VBDRow)o;
            string c1 = this.VBD.userdevice;
            string c2 = r.VBD.userdevice;
            int i1;
            int i2;
            if (int.TryParse(c1, out i1) && int.TryParse(c2, out i2))
            {
                return i1 - i2;
            }
            else
            {
                return c1.CompareTo(c2);
            }
        }

        private object CellValue(int col)
        {
            switch (col)
            {
                case 0:
                    return VBD.userdevice;
                case 1:
                    return VDI.Name();
                case 2:
                    return VDI.Description();
                case 3:
                    return SR.Name();
                case 4:
                    string name;
                    return VDI.sm_config.TryGetValue("displayname", out name) ? name : "";
                case 5:
                    return VDI.SizeText();
                case 6:
                    return VBD.IsReadOnly() ? Messages.YES : Messages.NO;
                case 7:
                    return GetPriorityString(VBD.GetIoNice());
                case 8:
                    return VBD.currently_attached ? Messages.YES : Messages.NO;
                case 9:
                    return VBD.device == "" ? Messages.STORAGE_PANEL_UNKNOWN : string.Format("/dev/{0}", VBD.device);
                default:
                    throw new ArgumentException(String.Format("Invalid column number {0} in VBDRenderer.CellValue()", col));
            }
        }

        public void UpdateCells()
        {
            for (int i = 0; i < 10; i++)
            {
                Cells[i].Value = CellValue(i);
            }
        }

        private static string GetPriorityString(int p)
        {
            switch (p)
            {
                case 0:
                    return Messages.STORAGE_WORST;
                case 7:
                    return Messages.STORAGE_BEST;
                default:
                    return p.ToString();
            }
        }
    }
}
