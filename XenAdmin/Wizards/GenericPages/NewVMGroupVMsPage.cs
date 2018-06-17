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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Wizards.GenericPages
{
    // This design for this class is in NewVMGroupVMsPageBase: see notes there.

    public class NewVMGroupVMsPage<T> : NewVMGroupVMsPageBase where T : XenObject<T>
    {
        private Pool _pool;
        public Pool Pool
        {
            get { return _pool; }
            set
            {
                _pool = value;
                if (_pool != null)
                    label1.Text = string.Format(Helpers.IsPool(_pool.Connection) ? Messages.VMS_IN_POOL : Messages.VMS_IN_SERVER,
                                                _pool.Name().Ellipsise(60));
                label2.Text = VMGroup<T>.ChooseVMsPage_Rubric;
                ColumnCurrentGroup.HeaderText = VMGroup<T>.ChooseVMsPage_CurrentGroup;
            }
        }

        public NewVMGroupVMsPage()
        {
        	SelectedVMs = new List<VM>();
            dataGridView1.SortCompare += dataGridView1_SortCompare;
        }

        public override string Text
        {
            get { return VMGroup<T>.ChooseVMsPage_Text; }
        }

        public override string SubText
        {
            get { return labelCounterVMs.Text; }
        }

        public override string HelpID
        {
            get { return VMGroup<T>.ChooseVMsPage_HelpID; } 
        }

        public override Image Image
        {
            get { return Properties.Resources._000_VM_h32bit_16; }
        }

        public override string PageTitle
        {
            get { return VMGroup<T>.ChooseVMsPage_Title; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal List<VM> SelectedVMs { get; set; }

        public string GroupName { private get; set; }

        public List<XenRef<VM>> SelectedVMsRefs
        {
            get { return SelectedVMs.ConvertAll<XenRef<VM>>(converter); }
        }

        private static XenRef<VM> converter(VM vm)
        {
            return new XenRef<VM>(vm.opaque_ref);
        }

        protected override void searchTextBox1_TextChanged(object sender, System.EventArgs e)
        {
            if (Pool != null)
            {
				var sortedColumn = dataGridView1.SortedColumn;
				var sortOrder = dataGridView1.SortOrder;

                dataGridView1.Rows.Clear();
                foreach (var vm in Pool.Connection.Cache.VMs)
                {
                    if (searchTextBox1.Matches(vm.Name()) && vm.is_a_real_vm() && vm.Show(Properties.Settings.Default.ShowHiddenVMs))
						dataGridView1.Rows.Add(new VMDataGridViewRow(SelectedVMs.Contains(vm), vm));
                }

				if (sortOrder != SortOrder.None && sortedColumn != null)
					dataGridView1.Sort(sortedColumn, (sortOrder == SortOrder.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending);
                UpdateCounterLabelAndButtons();
            }
        }

        private void UpdateCounterLabelAndButtons()
        {
            labelCounterVMs.Text = SelectedVMs.Count == 1 ? Messages.ONE_VM_SELECTED : string.Format(Messages.MOREONE_VM_SELECTED, SelectedVMs.Count);
            buttonClearAll.Enabled = SelectedVMs.Count > 0;
            buttonSelectAll.Enabled = SelectedVMs.Count < dataGridView1.RowCount;
        }

        // How to sort the VMs in the grid view: CA-69817
        void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            e.Handled = false;

            bool sortByName = false;

            if (e.Column == ColumnCheckBox)
            {
                // Sort by check mark first, then by name
                int checked1 = (bool)e.CellValue1 ? 1 : 0;
                int checked2 = (bool)e.CellValue2 ? 1 : 0;
                if (checked1 != checked2)
                {
                    e.SortResult = checked2 - checked1;
                    e.Handled = true;
                }
                else
                    sortByName = true;
            }

            if (e.Column == ColumnName || sortByName)
            {
                // In order to sort by name, we actually use the built-in VM sort.
                // This ensures we use the correct name sorting, and gives a consistent tie-breaker.
                VM vm1 = ((VMDataGridViewRow)dataGridView1.Rows[e.RowIndex1]).Vm;
                VM vm2 = ((VMDataGridViewRow)dataGridView1.Rows[e.RowIndex2]).Vm;
                e.SortResult = vm1.CompareTo(vm2);
                e.Handled = true;
            }

            // For any other column, we fall off the end of this function with e.Handled still false,
            // and let the framework handle it.
        }

        private class VMDataGridViewRow : DataGridViewRow
        {
            private DataGridViewCheckBoxCell _selectedCell = new DataGridViewCheckBoxCell();
            private DataGridViewTextBoxCell _nameCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _descriptionCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _currentGroupCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _quiesce_supported;
            public readonly VM Vm;
            public VMDataGridViewRow(bool selected, VM vm)
            {
                Vm = vm;
                Cells.Add(_selectedCell);
                Cells.Add(_nameCell);
                Cells.Add(_descriptionCell);
                Cells.Add(_currentGroupCell);
                if (VMGroup<T>.isQuescingSupported)
                {
                    _quiesce_supported = new DataGridViewTextBoxCell();
                    Cells.Add(_quiesce_supported);
                }
                Refresh(selected);
            }

            void Refresh(bool selected)
            {
                _selectedCell.Value = selected;
                _nameCell.Value = Vm.Name();
                _descriptionCell.Value = Vm.Description();
                T group = Vm.Connection.Resolve(VMGroup<T>.VmToGroup(Vm));
                _currentGroupCell.Value = group == null ? Messages.NONE : group.Name();
                if(VMGroup<T>.isQuescingSupported)
                {
                    if (Vm.allowed_operations.Contains((vm_operations.snapshot_with_quiesce)) && !Helpers.FeatureForbidden(Vm, Host.RestrictVss))
                    {
                        _quiesce_supported.Value = Messages.YES;
                    }
                    else
                    {
                        _quiesce_supported.Value = Messages.NO;
                    }
                }
            }
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            RefreshTab(null);
        } 

        private T _group = null;
        private void RefreshTab(T group)
        {
            ColumnQuiesceSupported.Visible = VMGroup<T>.isQuescingSupported;
            _group = group;
            if (Pool != null)
            {
                dataGridView1.Rows.Clear();
                foreach (var vm in Pool.Connection.Cache.VMs)
                {
                    int index = 0;
                    if (vm.is_a_real_vm() && vm.Show(Properties.Settings.Default.ShowHiddenVMs))
                    {
                        bool selected = group != null && VMGroup<T>.GroupToVMs(group).Contains(new XenRef<VM>(vm.opaque_ref));
						index = dataGridView1.Rows.Add(new VMDataGridViewRow(selected, vm));

                        if (SelectedVMs.Contains(vm))
                            dataGridView1.Rows[index].Cells[0].Value = true;
						else if (selected && !SelectedVMs.Contains(vm))
							SelectedVMs.Add(vm);
                    }
                }

                dataGridView1.Sort(ColumnCheckBox, ListSortDirection.Ascending);
                dataGridView1.AutoResizeColumns();
            }
            UpdateCounterLabelAndButtons();
        }

        protected override void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].Cells[0].Value = !(bool)dataGridView1.Rows[e.RowIndex].Cells[0].Value;
            }
        }

        protected override void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != 0 || e.RowIndex < 0 && e.RowIndex >= dataGridView1.RowCount)
				return;

			var row = dataGridView1.Rows[e.RowIndex] as VMDataGridViewRow;

			if (row != null)
			{
				if ((bool)row.Cells[0].Value)
				{
					if (!SelectedVMs.Contains(row.Vm))
						SelectedVMs.Add(row.Vm);
				}
				else
					SelectedVMs.Remove(row.Vm);
			}

            UpdateCounterLabelAndButtons();
		}

        protected override void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
		{
			if ((e.Row.State & DataGridViewElementStates.Selected) != 0)
				e.Row.Selected = false;
		}

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Back)
                return;

            if (!userAcceptsWarning())
                cancel = true;
        }

        private bool userAcceptsWarning()
        {
            string groupName = _group != null ? _group.Name() : GroupName;
            return AssignGroupToolStripMenuItem<T>.AssignGroupToVMCommand.ChangesOK(SelectedVMs, _group, groupName);
        }

        protected override void buttonClearAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dataGridViewRow in dataGridView1.Rows)
            {
                dataGridViewRow.Cells[0].Value = false;
            }
        }

        protected override void buttonSelectAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dataGridViewRow in dataGridView1.Rows)
            {
                dataGridViewRow.Cells[0].Value = true;
            }
        }

        #region IEditPage implementation
        public override AsyncAction SaveSettings()
        {
            return VMGroup<T>.AssignVMsToGroupAction(_clone, SelectedVMsRefs, true);
        }

        private T _clone;
        public override void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _clone = (T)clone;
            Pool = Helpers.GetPoolOfOne(_clone.Connection);
            RefreshTab(_clone);
        }

        public override bool ValidToSave
        {
            get { return userAcceptsWarning(); }
        }

        public override void ShowLocalValidationMessages()
        {
        }

        public override void Cleanup()
        {
            dataGridView1.Rows.Clear();
        }

        public override bool HasChanged
        {
            get
            {
                return (!Helpers.CompareLists(SelectedVMsRefs, VMGroup<T>.GroupToVMs(_clone)));
            }
        }
        #endregion
    }
}
