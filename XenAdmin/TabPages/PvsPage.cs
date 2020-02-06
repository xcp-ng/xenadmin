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
using XenAdmin.Network;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Commands;
using XenAdmin.Core;

namespace XenAdmin.TabPages
{
    public partial class PvsPage : BaseTabPage
    {
        private IXenConnection connection;
        private SelectionManager enableSelectionManager;
        private SelectionManager disableSelectionManager;

        private DataGridViewVms_DefaultSort vmDefaultSort;

        private readonly CollectionChangeEventHandler PvsProxy_CollectionChangedWithInvoke;
        
        public PvsPage()
        {
            InitializeComponent();

            enableButton.Command = new EnablePvsReadCachingCommand();
            disableButton.Command = new DisablePvsReadCachingCommand();

            enableSelectionManager = new SelectionManager();
            disableSelectionManager = new SelectionManager();

            vmDefaultSort = new DataGridViewVms_DefaultSort();

            PvsProxy_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(PvsProxyCollectionChanged);

            base.Text = Messages.PVS_TAB_TITLE;
        }

        public override string HelpID => "TabPagePvs";

        public IXenConnection Connection
        {
            get
            {
                return connection;
            }
            set
            {
                UnregisterHandlers();

                connection = value;

                if (connection != null)
                {
                    connection.Cache.RegisterCollectionChanged<PVS_proxy>(PvsProxy_CollectionChangedWithInvoke);
                    connection.Cache.RegisterCollectionChanged<VM>(PvsProxy_CollectionChangedWithInvoke);
                }

                LoadVMs();
            }
        }

        #region VMs

        private static bool VmShouldBeVisible(VM vm)
        {
            if (XenAdminConfigManager.Provider.ObjectIsHidden(vm.opaque_ref))
                return false;

            return vm.is_a_real_vm() && vm.Show(Properties.Settings.Default.ShowHiddenVMs) && !vm.IsBeingCreated;
        }

        private static bool VmIsJustAdded(VM vm)
        {
            return vm.is_a_template && vm.name_label.StartsWith(Helpers.GuiTempObjectPrefix);
        }

        private void LoadVMs()
        {
            Program.AssertOnEventThread();

            if (!Visible)
                return;

            try
            {
                dataGridViewVms.SuspendLayout();

                var previousSelection = GetSelectedVMs();

                UnregisterVmEventHandlers();

                dataGridViewVms.SortCompare += dataGridViewVms_SortCompare;

                dataGridViewVms.Rows.Clear();

                enableSelectionManager.BindTo(enableButton, Program.MainWindow);
                disableSelectionManager.BindTo(disableButton, Program.MainWindow);

                //clear selection
                enableSelectionManager.SetSelection(new SelectedItemCollection());
                disableSelectionManager.SetSelection(new SelectedItemCollection());

                foreach (var vm in Connection.Cache.VMs)
                {
                    // Add all real VMs and templates that begin with __gui__ (because this may be a new VM)
                    var addVm = vm.is_a_real_vm() || VmIsJustAdded(vm) || vm.IsBeingCreated;

                    if (!addVm)
                        continue;

                    var show = VmShouldBeVisible(vm);
                     
                    dataGridViewVms.Rows.Add(NewVmRow(vm, show));
                }

                SortVmTable();

                if (dataGridViewVms.Rows.Count > 0)
                {
                    dataGridViewVms.SelectionChanged += VmSelectionChanged;

                    UnselectAllVMs(); // Component defaults the first row to selected
                    if (previousSelection.Any())
                    {
                        foreach (var row in dataGridViewVms.Rows.Cast<DataGridViewRow>())
                        {
                            if (previousSelection.Contains((VM)row.Tag))
                            {
                                row.Selected = true;
                            }
                        }
                    }
                    else
                    {
                        dataGridViewVms.Rows[0].Selected = true;
                    }
                }
            }
            finally
            {
                dataGridViewVms.ResumeLayout();
            }
        }

        private void UnselectAllVMs()
        {
            foreach (var row in dataGridViewVms.SelectedRows.Cast<DataGridViewRow>())
            {
                row.Selected = false;
            }
        }

        private IList<VM> GetSelectedVMs()
        {
            return dataGridViewVms.SelectedRows.Cast<DataGridViewRow>().Select(row => (VM)row.Tag).ToList();
        }

        private void VmSelectionChanged(object sender, EventArgs e)
        {
            var selectedVMs = GetSelectedVMs().Select(vm => new SelectedItem(vm));
            enableSelectionManager.SetSelection(selectedVMs);
            disableSelectionManager.SetSelection(selectedVMs);
        }

        private DataGridViewRow NewVmRow(VM vm, bool showRow)
        {
            System.Diagnostics.Trace.Assert(vm != null);

            var pvsProxy = vm.PvsProxy();
            
            var vmCell = new DataGridViewTextBoxCell();
            var cacheEnabledCell = new DataGridViewTextBoxCell();
            var pvsSiteCell = new DataGridViewTextBoxCell();
            var statusCell = new DataGridViewTextBoxCell();

            var newRow = new DataGridViewRow { Tag = vm };
            newRow.Cells.AddRange(vmCell, cacheEnabledCell, pvsSiteCell, statusCell);

            UpdateRow(newRow, vm, pvsProxy, showRow);
            RegisterVmEventHandlers(vm, pvsProxy);
            return newRow;
        }

        private void UpdateRow(DataGridViewRow row, VM vm, PVS_proxy pvsProxy, bool showRow=true)
        {
            PVS_site pvsSite = pvsProxy == null ? null : Connection.Resolve(pvsProxy.site);
            row.Cells[0].Value = vm.Name();
            row.Cells[1].Value = pvsProxy == null ? Messages.NO : Messages.YES;
            row.Cells[2].Value = pvsProxy == null || pvsSite == null ? Messages.NO_VALUE : pvsSite.NameWithWarning();
            row.Cells[3].Value = pvsProxy == null ? Messages.NO_VALUE : pvs_proxy_status_extensions.ToFriendlyString(pvsProxy.status);

            row.Visible = showRow;
        }

        private void RegisterVmEventHandlers(VM vm, PVS_proxy pvsProxy)
        {
            vm.PropertyChanged -= VmPropertyChanged;
            vm.PropertyChanged += VmPropertyChanged;
            if (pvsProxy != null)
            {
                pvsProxy.PropertyChanged -= PvsProxyPropertyChanged;
                pvsProxy.PropertyChanged += PvsProxyPropertyChanged;
                PVS_site pvsSite = pvsProxy == null ? null : Connection.Resolve(pvsProxy.site);
                if (pvsSite != null)
                {
                    pvsSite.PropertyChanged -= PvsSitePropertyChanged;
                    pvsSite.PropertyChanged += PvsSitePropertyChanged;
                }
            }
        }

        private void UnregisterVmEventHandlers()
        {
            dataGridViewVms.SelectionChanged -= VmSelectionChanged;
            dataGridViewVms.SortCompare -= dataGridViewVms_SortCompare;

            foreach (DataGridViewRow row in dataGridViewVms.Rows)
            {
                var vm = row.Tag as VM;
                System.Diagnostics.Trace.Assert(vm != null);
                vm.PropertyChanged -= VmPropertyChanged;

                var pvsProxy = vm.PvsProxy();
                if (pvsProxy != null)
                {
                    pvsProxy.PropertyChanged -= PvsProxyPropertyChanged;
                    var pvsSite = Connection.Resolve(pvsProxy.site);
                    if (pvsSite != null)
                        pvsSite.PropertyChanged -= PvsSitePropertyChanged;
                }
            }
        }

        private void UnregisterHandlers()
        {
            if (connection == null) 
                return;
            connection.Cache.DeregisterCollectionChanged<PVS_proxy>(PvsProxy_CollectionChangedWithInvoke);
            connection.Cache.DeregisterCollectionChanged<VM>(PvsProxy_CollectionChangedWithInvoke);
        }

        public override void PageHidden()
        {
            UnregisterHandlers();
        }

        void dataGridViewVms_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            e.Handled = false;

            if (!e.Column.Equals(columnVM))
            {
                // All columns in this table except VM name are likely to contain a lot of duplicates
                // so always use VM as the tiebreaker for consistency
                var cellValue1 = e.CellValue1.ToString();
                var cellValue2 = e.CellValue2.ToString();

                if (cellValue1 != cellValue2)
                {
                    e.SortResult = cellValue1.CompareTo(cellValue2);
                    e.Handled = true;
                    return;
                }
            }

            // If we get here, either sorting by VM name column, or using it as tie breaker
            // In order to sort by name, we actually use the built-in VM sort.
            // This ensures we use the correct name sorting, and gives a consistent tie-breaker.
            VM vm1 = (VM)dataGridViewVms.Rows[e.RowIndex1].Tag;
            VM vm2 = (VM)dataGridViewVms.Rows[e.RowIndex2].Tag;
            e.SortResult = vm1.CompareTo(vm2);
            e.Handled = true;
        }

        #endregion

        private void PvsProxyCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            LoadVMs();
        }

        private void SortVmTable()
        {
            var sortedColumn = dataGridViewVms.SortedColumn;
            var sortDirection = ListSortDirection.Ascending;

            if (dataGridViewVms.SortOrder.Equals(SortOrder.Descending))
                sortDirection = ListSortDirection.Descending;


            if (sortedColumn != null)
            {
                dataGridViewVms.Sort(sortedColumn, sortDirection);
            }
            else
            {
                dataGridViewVms.Sort(vmDefaultSort);
            }
        }

        private void VmPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsBeingCreated" && e.PropertyName != "name_label")
                return;

            if (e.PropertyName.Equals("IsBeingCreated"))
            {
                var senderAsVm = sender as VM;
                if (senderAsVm == null || senderAsVm.IsBeingCreated)
                {
                    return;
                }

                Program.Invoke(this, () =>
                {
                    foreach (DataGridViewRow row in dataGridViewVms.Rows)
                    {
                        var vm = row.Tag as VM;
                        if (vm != null && vm.Equals(sender))
                        {
                            var wasHidden = !row.Visible;

                            dataGridViewVms.SuspendLayout();

                            if (VmShouldBeVisible(vm))
                            {
                                row.Visible = true;

                                if (wasHidden)
                                    SortVmTable(); // This appears as an add to the user, so retain table sorting
                            }

                            dataGridViewVms.ResumeLayout();
                            break;
                        }
                    }
                });

                return;
            }

            Program.Invoke(this, () =>
            {
                foreach (DataGridViewRow row in dataGridViewVms.Rows)
                {
                    var vm = row.Tag as VM;
                    if (vm != null && vm.Equals(sender))
                    {
                        row.Cells[columnVM.Name].Value = vm.Name();
                        break;
                    }
                }
            });
        }

        private void PvsProxyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "status" && e.PropertyName != "site")
                return;

            Program.Invoke(this, () =>
            {
                var pvsProxy = sender as PVS_proxy;
                foreach (DataGridViewRow row in dataGridViewVms.Rows)
                {
                    var vm = row.Tag as VM;
                    if (vm != null)
                    {
                        var pvsProxyVm = pvsProxy.VM();
                        if (vm.Equals(pvsProxyVm))
                        {
                            UpdateRow(row, pvsProxyVm, pvsProxy);
                            break;
                        }
                    }
                }
            });
        }

        private void PvsSitePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "name_label" && e.PropertyName != "cache_storage")
                return;

            Program.Invoke(this, () =>
            {
                var pvsSite = sender as PVS_site;
                foreach (DataGridViewRow row in dataGridViewVms.Rows)
                {
                    var vm = row.Tag as VM;
                    if (vm == null)
                        continue;
                    var pvsProxy = vm.PvsProxy();
                    if (pvsProxy != null && pvsProxy.site != null && pvsProxy.site.opaque_ref == pvsSite.opaque_ref)
                    {
                        UpdateRow(row, vm, pvsProxy);
                    }
                }
            });
        }

        private void ConfigureButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new PvsCacheConfigurationDialog(connection))
                dialog.ShowDialog(this);
        }
    }

    class DataGridViewVms_DefaultSort : System.Collections.IComparer
    {
        public int Compare(object first, object second)
        {
            // Default sort: Sort by whether caching is enabled (yes before no), using VM name (asc) as tiebreaker
            DataGridViewRow row1 = (DataGridViewRow)first;
            DataGridViewRow row2 = (DataGridViewRow)second;

            int cachingEnabled1 = row1.Cells[1].Value.ToString().Equals(Messages.YES) ? 0 : 1;
            int cachingEnabled2 = row2.Cells[1].Value.ToString().Equals(Messages.YES) ? 0 : 1;

            if (cachingEnabled1 != cachingEnabled2)
            {
                return cachingEnabled1.CompareTo(cachingEnabled2);
            }
            else
            {
                // VM name as tiebreaker
                var vm1 = row1.Tag as VM;
                var vm2 = row2.Tag as VM;

                return vm1.CompareTo(vm2);
            }
        }
    }
}
