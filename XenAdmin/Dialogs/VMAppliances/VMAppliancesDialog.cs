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
using XenAdmin.Actions;
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAdmin.Controls;
using XenAdmin.Wizards.ImportWizard;
using XenAdmin.Wizards.NewVMApplianceWizard;
using XenAdmin.Wizards.ExportWizard;
using XenAPI;


namespace XenAdmin.Dialogs.VMAppliances
{
    public partial class VMAppliancesDialog : XenDialogBase
    {
        public readonly Pool Pool;

        public VMAppliancesDialog(Pool pool)
        {
            Pool = pool;
            InitializeComponent();
            Text = string.Format(Messages.VM_APPLIANCES_TITLE, pool.Name());
            labelVMAppliancesInPool.Text = string.Format(Helpers.IsPool(pool.Connection)
                                                             ? Messages.VM_APPLIANCES_DEFINED_FOR_POOL
                                                             : Messages.VM_APPLIANCES_DEFINED_FOR_SERVER,
                                                         pool.Name().Ellipsise(250));

            listViewVMs.SmallImageList = Images.ImageList16;
        }

        private VM_appliance currentSelected = null;

        private void LoadVmAppliances()
        {
            dataGridViewVMAppliances.SuspendLayout();
            try
            {
                dataGridViewVMAppliances.Rows.Clear();

                foreach (var vmAppliance in Pool.Connection.Cache.VM_appliances)
                {
                    var row = new VMApplianceRow(vmAppliance);
                    dataGridViewVMAppliances.Rows.Add(row);
                }

                RefreshButtons();
                RefreshGroupMembersPanel();

                var selectedVMAppliance = currentSelected;
                if (selectedVMAppliance != null)
                {
                    foreach (DataGridViewRow row in dataGridViewVMAppliances.Rows)
                    {
                        if (row is VMApplianceRow &&
                            (row as VMApplianceRow).VMAppliance.uuid == selectedVMAppliance.uuid)
                        {
                            dataGridViewVMAppliances.ClearSelection();
                            row.Selected = true;
                            break;
                        }
                    }
                }
            }
            finally
            {
                dataGridViewVMAppliances.ResumeLayout();
            }
        }

        private void VMApplianceCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            VM_appliance vmAppliance = (VM_appliance)e.Element;
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    vmAppliance.PropertyChanged -= vApp_PropertyChanged;
                    vmAppliance.PropertyChanged += vApp_PropertyChanged;
                    break;
                case CollectionChangeAction.Remove:
                    vmAppliance.PropertyChanged -= vApp_PropertyChanged;
                    break;
            }
            Program.Invoke(this, LoadVmAppliances);
        }

        #region VMApplianceRow
        private class VMApplianceRow : DataGridViewRow
        {
            private DataGridViewTextAndImageCell _nameCell = new DataGridViewTextAndImageCell();
            private DataGridViewTextBoxCell _descriptionCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _numVMsCell = new DataGridViewTextBoxCell();
            public readonly VM_appliance VMAppliance;
            public VMApplianceRow(VM_appliance vmAppliance)
            {
                Cells.Add(_nameCell);
                Cells.Add(_descriptionCell);
                Cells.Add(_numVMsCell);
                VMAppliance = vmAppliance;
                RefreshRow(true);
            }

            private List<ListViewItem> vmItems = new List<ListViewItem>();

            public List<ListViewItem> VmItems
            {
                get { return vmItems; }
            }

            private void RefreshVmItems()
            {
                vmItems.Clear();
                List<VM> vms = VMAppliance.Connection.ResolveAll(VMAppliance.VMs);
                vms.Sort();
                foreach (var vm in vms)
                {
                    // Create a new row for this VM
                    ListViewItem vmRow = new ListViewItem();
                    vmRow.Tag = vm;
                    vmItems.Add(vmRow);
                }
            }

            public void RefreshRow(bool refreshItems)
            {
                _nameCell.Value = VMAppliance.Name();
                _nameCell.Image = null;
                _descriptionCell.Value = VMAppliance.Description();
                _numVMsCell.Value = VMAppliance.VMs.Count;
                if (refreshItems)
                    RefreshVmItems();
            }
        }

        private VMApplianceRow FindVmApplianceRow(VM_appliance vmAppliance)
        {
            if (vmAppliance == null)
                return null;
            return dataGridViewVMAppliances.Rows.Cast<VMApplianceRow>().FirstOrDefault(row => vmAppliance.Equals(row.VMAppliance));
        }

        private void vApp_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vmAppliance = (VM_appliance)sender;

            if (vmAppliance == null)
                return;

            var selectedAppliance = dataGridViewVMAppliances.SelectedRows.Count > 0
                ? ((VMApplianceRow)dataGridViewVMAppliances.SelectedRows[0]).VMAppliance
                : null;

            var selectedRowAffected = selectedAppliance != null && selectedAppliance.opaque_ref == vmAppliance.opaque_ref;

            switch (e.PropertyName)
            {
                case "allowed_operations":
                    if (selectedRowAffected)
                        Program.Invoke(this, RefreshButtons);
                    break;
                case "name_label":
                case "name_description":
                    Program.Invoke(this, () =>
                    {
                        VMApplianceRow row = FindVmApplianceRow(vmAppliance);
                        if (row != null)
                            row.RefreshRow(false);
                    });
                    break;
                case "VMs":
                    Program.Invoke(this, () =>
                    {
                        VMApplianceRow row = FindVmApplianceRow(vmAppliance);
                        if (row != null)
                        {
                            row.RefreshRow(true);
                            if (selectedRowAffected)
                                RefreshGroupMembersPanel();
                        }
                    });
                    break;
            }
        }
        #endregion
        
        #region VMs
        private static void UpdateVmRow(ListViewItem row)
        {
            VM vm = (VM) row.Tag;

            // Icon/name
            row.ImageIndex = (int)Images.GetIconFor(vm);
            row.Text = vm.Name();
        }

        /// <summary>
        /// Finds the ListViewItem for the given VM. Will return null if no corresponding item could be found.
        /// Must be called on the event thread.
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        private ListViewItem FindItemFromVM(VM vm)
        {
            Program.AssertOnEventThread();

            return listViewVMs.Items.Cast<ListViewItem>().FirstOrDefault(item => item.Tag == vm);
        }

        private void vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = (VM) sender;

            if (vm == null)
                return;

            var selectedAppliance = dataGridViewVMAppliances.SelectedRows.Count > 0
                ? ((VMApplianceRow)dataGridViewVMAppliances.SelectedRows[0]).VMAppliance
                : null;

            var selectedRowAffected = selectedAppliance != null && selectedAppliance.opaque_ref == vm.appliance.opaque_ref;

            if (!selectedRowAffected)
                return;

            switch (e.PropertyName)
            {
                case "allowed_operations":
                    Program.Invoke(this, RefreshButtons);
                    break;
                case "name_label":
                case "power_state":
                case "current_operations":
                    Program.Invoke(this, () =>
                    {
                        ListViewItem item = FindItemFromVM((VM) sender);
                        if (item == null)
                            return;
                        listViewVMs.BeginUpdate();
                        try
                        {
                            UpdateVmRow(item);
                        }
                        finally
                        {
                            listViewVMs.EndUpdate();
                        }
                    });
                    break;
            }
        }

        void VMCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.AssertOnEventThread();

            VM vm = (VM)e.Element;
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    vm.PropertyChanged -= vm_PropertyChanged;
                    vm.PropertyChanged += vm_PropertyChanged;
                    break;
                case CollectionChangeAction.Remove:
                    vm.PropertyChanged -= vm_PropertyChanged;
                    break;
            }

            Program.Invoke(this, () =>
            {
                // Find row for VM
                ListViewItem item = FindItemFromVM(vm);
                if (item != null)
                {
                    listViewVMs.BeginUpdate();
                    try
                    {
                        UpdateVmRow(item);
                    }
                    finally
                    {
                        listViewVMs.EndUpdate();
                    }
                }
            });
        }
        #endregion


        private void RefreshButtons()
        {
            if (dataGridViewVMAppliances.SelectedRows.Count == 1 && dataGridViewVMAppliances.SelectedRows[0] is VMApplianceRow)
            {
                currentSelected = (VM_appliance)((VMApplianceRow)dataGridViewVMAppliances.SelectedRows[0]).VMAppliance;
                toolStripButtonEdit.Enabled = true;
            }
            else
            {
                if (dataGridViewVMAppliances.SelectedRows.Count == 0)
                    currentSelected = null;
                toolStripButtonEdit.Enabled = false;
            }
            toolStripButtonDelete.Enabled = (dataGridViewVMAppliances.SelectedRows.Count != 0);

        	toolStripButtonExport.Enabled = currentSelected != null && currentSelected.VMs.TrueForAll(vmRef =>
        	                                                                                          	{
        	                                                                                          		var vm = currentSelected.Connection.Resolve(vmRef);
        	                                                                                          		return vm != null
        	                                                                                          		       && !vm.is_a_template
        	                                                                                          		       && !vm.Locked
        	                                                                                          		       && vm.allowed_operations != null
        	                                                                                          		       && vm.allowed_operations.Contains(vm_operations.export);
        	                                                                                          	});

			if (currentSelected == null)
			{
				toolStripButtonStart.Enabled = toolStripButtonShutdown.Enabled = false;
			}
			else
			{
				toolStripButtonStart.Enabled = currentSelected.allowed_operations.Contains(vm_appliance_operation.start);

				toolStripButtonShutdown.Enabled = currentSelected.allowed_operations.Contains(vm_appliance_operation.clean_shutdown)
				                                  || currentSelected.allowed_operations.Contains(vm_appliance_operation.hard_shutdown);
    		}

            tableLayoutPanel2.Visible = (currentSelected != null);
            tableLayoutPanel1.ColumnStyles[1].Width = currentSelected == null ? 0 : 200;
        }

        private void VMAppliancesDialog_Load(object sender, EventArgs e)
        {
            LoadVmAppliances();
            Pool.Connection.Cache.RegisterCollectionChanged<VM_appliance>(VMApplianceCollectionChanged);
            Pool.Connection.Cache.RegisterCollectionChanged<VM>(VMCollectionChanged);
            foreach (VM vm in Pool.Connection.Cache.VMs)
            {
                vm.PropertyChanged -= vm_PropertyChanged;
                vm.PropertyChanged += vm_PropertyChanged;
            }
            foreach (var vApp in Pool.Connection.Cache.VM_appliances)
            {
                vApp.PropertyChanged -= vApp_PropertyChanged;
                vApp.PropertyChanged += vApp_PropertyChanged;
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            RefreshButtons();
            RefreshGroupMembersPanel();
        }

        private void VMAppliancesDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Pool.Connection.Cache.DeregisterCollectionChanged<VM_appliance>(VMApplianceCollectionChanged);
            Pool.Connection.Cache.DeregisterCollectionChanged<VM>(VMCollectionChanged);
            foreach (VM vm in Pool.Connection.Cache.VMs)
            {
                vm.PropertyChanged -= vm_PropertyChanged;
            }
            foreach (VM_appliance vmAppliance in Pool.Connection.Cache.VM_appliances)
            {
                vmAppliance.PropertyChanged -= vApp_PropertyChanged;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toolStripButtonNew_Click(object sender, EventArgs e)
        {
            new NewVMApplianceWizard(Pool).Show(this);
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            var selectedAppliances = new List<VM_appliance>();
            int numberOfProtectedVMs = 0;
            foreach (DataGridViewRow row in dataGridViewVMAppliances.SelectedRows)
            {
                var appliance = ((VMApplianceRow)row).VMAppliance;
                selectedAppliances.Add(appliance);
                numberOfProtectedVMs += appliance.VMs.Count;

            }
            string text = "";
            if (selectedAppliances.Count == 1)
            {
                if (numberOfProtectedVMs == 1)
                    text = String.Format(Messages.CONFIRM_DELETE_VM_APPLIANCE_1, selectedAppliances[0].Name().Ellipsise(120), numberOfProtectedVMs);
                else
                    text = String.Format(numberOfProtectedVMs == 0 ? Messages.CONFIRM_DELETE_VM_APPLIANCE_0 : Messages.CONFIRM_DELETE_VM_APPLIANCE, selectedAppliances[0].Name().Ellipsise(120), numberOfProtectedVMs);
            }
            else
            {
                if (numberOfProtectedVMs == 1)
                    text = String.Format(Messages.CONFIRM_DELETE_VM_APPLIANCES_1, numberOfProtectedVMs);
                else
                    text = string.Format(numberOfProtectedVMs == 0 ? Messages.CONFIRM_DELETE_VM_APPLIANCES_0 : Messages.CONFIRM_DELETE_VM_APPLIANCES, numberOfProtectedVMs);
            }

            using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, text, Messages.DELETE_VM_APPLIANCE_TITLE),
                    ThreeButtonDialog.ButtonYes,
                    ThreeButtonDialog.ButtonNo))
            {
                if (dlg.ShowDialog(this) == DialogResult.Yes)
                    new DestroyVMApplianceAction(Pool.Connection, selectedAppliances).RunAsync();
            }
        }

        private void toolStripButtonEdit_Click(object sender, EventArgs e)
        {
            using (PropertiesDialog propertiesDialog = new PropertiesDialog(currentSelected))
            {
                propertiesDialog.ShowDialog(this);
            }
        }

        private void RefreshGroupMembersPanel()
        {
            if (dataGridViewVMAppliances.SelectedRows.Count != 1 || !(dataGridViewVMAppliances.SelectedRows[0] is VMApplianceRow))
            {
                labelVMApplianceName.Text = "";
                return;
            }

            var applianceRow = (VMApplianceRow)dataGridViewVMAppliances.SelectedRows[0];
            labelVMApplianceName.Text = applianceRow.VMAppliance.Name().Ellipsise(120);

            listViewVMs.Clear();
            foreach (var vmItem in applianceRow.VmItems)
            {
                listViewVMs.Items.Add(vmItem);
                UpdateVmRow(vmItem);
            }
        }

		private void toolStripButtonShutdown_Click(object sender, EventArgs e)
		{
			if (currentSelected == null)
				return;

			using (var confirmDialog = new ThreeButtonDialog(
				new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.CONFIRM_SHUT_DOWN_APPLIANCE, Messages.VM_APPLIANCE_SHUT_DOWN),
				ThreeButtonDialog.ButtonYes,
				ThreeButtonDialog.ButtonNo))
			{
				if (confirmDialog.ShowDialog(this) != DialogResult.Yes)
					return;
				//shut down appliance
				(new ShutDownApplianceAction(currentSelected)).RunAsync();
			}
		}

    	private void toolStripMenuItemStart_Click(object sender, EventArgs e)
		{
			if (currentSelected == null)
				return;

			(new StartApplianceAction(currentSelected, false)).RunAsync();
		}

		private void toolStripButtonExport_Click(object sender, EventArgs e)
		{
			if (currentSelected == null)
				return;

			var selection = new SelectedItemCollection(new SelectedItem(currentSelected));
			(new ExportApplianceWizard(Pool.Connection, selection)).Show(this);
		}

		private void toolStripButtonImport_Click(object sender, EventArgs e)
		{
			new ImportWizard(Pool.Connection, Pool, null, true).Show(this);
		}

    }
}
