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
using System.Data;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Commands;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Wizards;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenAdmin.Controls.DataGridViewEx;
using XenCenterLib;

namespace XenAdmin.Controls.NetworkingTab
{
    public partial class NetworkList : UserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public NetworkList()
        {
            InitializeComponent();
            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
        }

        private IXenObject _xenObject = null;
        public IXenObject XenObject
        {
            get
            {
                Program.AssertOnEventThread();
                return _xenObject;
            }
            set
            {
                Program.AssertOnEventThread();

                // Remove all old property change listeners.  They will get added back in the
                // population of the DataGridView that follows (see BuildList).
                UnregisterHandlers();

                _xenObject = value;

                if (_xenObject != null)
                    _xenObject.PropertyChanged += Server_PropertyChanged;

                if (_xenObject is Host || _xenObject is Pool)
                {
                    AddNetworkColumns();

                    _xenObject.Connection.Cache.RegisterBatchCollectionChanged<XenAPI.Network>(NetworkCollectionChanged);
                    _xenObject.Connection.Cache.RegisterBatchCollectionChanged<PIF>(PIFCollectionChanged);

                    AddNetworkButton.Text = Messages.HOST_NETWORK_TAB_ADD_BUTTON_LABEL;
                    EditNetworkButton.Text = Messages.HOST_NETWORK_TAB_EDIT_BUTTON_LABEL;
                    RemoveNetworkButton.Text = Messages.HOST_NETWORK_TAB_REMOVE_BUTTON_LABEL;
                    // This is the divider for the activate button, hide this as well as the button
                    groupBox1.Visible = false;
                    buttonActivateToggle.Visible = false;
                }
                else if (_xenObject is VM)
                {
                    AddVifColumns();

                    _xenObject.Connection.Cache.RegisterBatchCollectionChanged<VIF>(CollectionChanged);

                    // update the list when we get new metrics
                    _xenObject.Connection.Cache.RegisterBatchCollectionChanged<VM_guest_metrics>(VM_guest_metrics_BatchCollectionChanged);

                    AddNetworkButton.Text = Messages.VM_NETWORK_TAB_ADD_BUTTON_LABEL;
                    EditNetworkButton.Text = Messages.VM_NETWORK_TAB_EDIT_BUTTON_LABEL;
                    RemoveNetworkButton.Text = Messages.VM_NETWORK_TAB_REMOVE_BUTTON_LABEL;
                    groupBox1.Visible = true;
                    buttonActivateToggle.Visible = true;
                }

                BuildList();
            }
        }

        private void DegregisterEventsOnXmo()
        {
            if (XenObject == null)
                return;
            
            XenObject.PropertyChanged -= Server_PropertyChanged;

            if (XenObject is VM)
            {
                VM vm = (VM)XenObject;

                VM_guest_metrics vmGuestMetrics = vm.Connection.Resolve(vm.guest_metrics);
                if (vmGuestMetrics != null)
                    vmGuestMetrics.PropertyChanged -= Server_PropertyChanged;
            }
        }

        internal void UnregisterHandlers()
        {
            DegregisterEventsOnXmo();
            DeregisterEventsOnGridRows();

            if (_xenObject is Host || _xenObject is Pool)
            {
                _xenObject.Connection.Cache.DeregisterBatchCollectionChanged<XenAPI.Network>(NetworkCollectionChanged);
                _xenObject.Connection.Cache.DeregisterBatchCollectionChanged<PIF>(PIFCollectionChanged);
            }
            else if (_xenObject is VM)
            {
                _xenObject.Connection.Cache.DeregisterBatchCollectionChanged<VIF>(CollectionChanged);
                _xenObject.Connection.Cache.DeregisterBatchCollectionChanged<VM_guest_metrics>(VM_guest_metrics_BatchCollectionChanged);
            }
        }

        private void AddNetworkColumns()
        {
            // catch the sorting column and sorting order, so they could be restored after the columns are added to the grid (SCTX-501)
            DataGridViewColumn previousSortColumn = NetworksGridView.SortedColumn;
            SortOrder previousSortOrder = NetworksGridView.SortOrder == SortOrder.None ? SortOrder.Ascending : NetworksGridView.SortOrder;

            try
            {
                NetworksGridView.SuspendLayout();

                NetworksGridView.Columns.Clear();
                NetworksGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                        this.ImageColumn,
                        this.NameColumn,
                        this.DescriptionColumn,
                        this.NicColumn,
                        this.VlanColumn,
                        this.AutoColumn,
                        this.LinkStatusColumn,
                        this.NetworkMacColumn,
                        this.MtuColumn,
                        this.NetworkSriovColumn});

                //CA-47050: the Description column should be autosized to Fill, but should not become smaller than a minimum
                //width, which here is chosen to be the column header width. To find what this width is set temporarily the
                //column's autosize mode to ColumnHeader.
                this.DescriptionColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                int storedWidth = this.DescriptionColumn.Width;
                this.DescriptionColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.DescriptionColumn.MinimumWidth = storedWidth;

                NetworkSriovColumn.Visible = Helpers.KolkataOrGreater(_xenObject.Connection);
            }
            finally
            {
                NetworksGridView.ResumeLayout();
            }

            // restore the sorting column (SCTX-501)
            RestoreSortingColumn(previousSortColumn, previousSortOrder, NicColumn);
        }

        private void AddVifColumns()
        {
            // catch the sorting column and sorting order, so they could be restored after the columns are added to the grid (SCTX-501)
            DataGridViewColumn previousSortColumn = NetworksGridView.SortedColumn;
            SortOrder previousSortOrder = NetworksGridView.SortOrder == SortOrder.None ? SortOrder.Ascending : NetworksGridView.SortOrder; 

            NetworksGridView.Columns.Clear();
            NetworksGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ImageColumn,
            this.DeviceColumn,
            this.VifMacColumn,
            this.LimitColumn,
            this.NetworkColumn,
            this.IpColumn,
            this.ActiveColumn});

            //the IP column should be autosized to Fill, but should not become smaller than a minimum
            //width, which here is chosen to be the column header width. To find what this width is set temporarily the
            //column's autosize mode to ColumnHeader.
            this.IpColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            int storedWidth = this.DescriptionColumn.Width;
            this.IpColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.IpColumn.MinimumWidth = storedWidth;

            // restore the sorting column (SCTX-501)
            RestoreSortingColumn(previousSortColumn, previousSortOrder, DeviceColumn);            
        }

        private void RestoreSortingColumn(DataGridViewColumn previousSortColumn, SortOrder previousSortOrder, DataGridViewColumn defaultSortColumn)
        {
            // SCTX-501 preserve the sorting column
            if ((previousSortColumn == null) || !NetworksGridView.Columns.Contains(previousSortColumn))
            {
                previousSortColumn = defaultSortColumn;
                previousSortOrder = SortOrder.Ascending;
                // try to get the old sort order from the SortGlyph
                foreach (System.Windows.Forms.DataGridViewColumn column in NetworksGridView.Columns)
                    if (column.HeaderCell.SortGlyphDirection != SortOrder.None)
                    {
                        previousSortColumn = column;
                        previousSortOrder = column.HeaderCell.SortGlyphDirection;
                        break;
                    }
            }
            if (previousSortColumn != null)
            {
                // clear any old sort glyphs; this is necessary as a result of the 
                // change for CA-45643 (MTU column only added for Cowley or greater pools)
                foreach (System.Windows.Forms.DataGridViewColumn column in NetworksGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None; 

                NetworksGridView.Sort(
                   previousSortColumn,
                   previousSortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
            }
        }

        void VM_guest_metrics_BatchCollectionChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, BuildList);
        }

        void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShowHiddenVMs")
            {
                BuildList();
            }
        }

        public bool InBuildList = false;
        public void BuildList()
        {
            Program.AssertOnEventThread();
            if (!this.Visible)
                return;

            if (InBuildList)
                return;

            InBuildList = true;

            try
            {
                if (XenObject == null)
                    return;

                if (!XenObject.Connection.CacheIsPopulated)
                    return;

                if (XenObject is VM)
                {
                    DeregisterEventsOnGridRows();
                    VIF selectedVIF = SelectedVif;
                    VM vm = XenObject as VM;

                    NetworksGridView.SuspendLayout();
                    NetworksGridView.Rows.Clear();

                    List<VIF> vifs = vm.Connection.ResolveAll(vm.VIFs);
                    vifs.Sort();

                    // CA-8981 - Listen for guest metric changes which is necessary for IP Address updates
                    VM_guest_metrics vmGuestMetrics = vm.Connection.Resolve(vm.guest_metrics);
                    if (vmGuestMetrics != null)
                        vmGuestMetrics.PropertyChanged += Server_PropertyChanged;

                    var vifRowsToAdd = new List<VifRow>();
                    foreach (var vif in vifs)
                    {
                        var network = vif.Connection.Resolve(vif.network);
                        if (network != null &&
                            // CA-218956 - Expose HIMN when showing hidden objects
                            (network.IsGuestInstallerNetwork() && !XenAdmin.Properties.Settings.Default.ShowHiddenVMs))
                            continue;   // Don't show the guest installer network in the network tab (CA-73056)
                        vifRowsToAdd.Add(new VifRow(vif));
                    }
                    NetworksGridView.Rows.AddRange(vifRowsToAdd.ToArray());

                    bool selected = true;

                    if (selectedVIF != null)
                    {
                        foreach (VifRow row in NetworksGridView.Rows)
                        {
                            // Cannot compare opaque_ref as VIFs get destroyed / recreated on each edit.
                            if (row.Vif.device == selectedVIF.device)
                            {
                                row.Selected = true;
                                break;
                            }
                        }
                    }

                    if (!selected && NetworksGridView.Rows.Count > 0)
                    {
                        NetworksGridView.Rows[0].Selected = true;
                    }
                }
                else if (XenObject is Host || XenObject is Pool)
                {
                    DeregisterEventsOnGridRows();
                    XenAPI.Network selectedNetwork = SelectedNetwork;

                    NetworksGridView.SuspendLayout();
                    NetworksGridView.Rows.Clear();

                    XenAPI.Network[] networks = XenObject.Connection.Cache.Networks;
                    Array.Sort < XenAPI.Network>(networks);

                    List<NetworkRow> networkRowsToAdd = new List<NetworkRow>();
                    for (int i = 0; i < networks.Length; i++)
                    {
                        if (!networks[i].Show(XenAdmin.Properties.Settings.Default.ShowHiddenVMs))
                            continue;
                        networkRowsToAdd.Add(new NetworkRow(networks[i], XenObject));
                    }
                    NetworksGridView.Rows.AddRange(networkRowsToAdd.ToArray());
                    // The following update causes this to be a lot slower with many networks. Alot! CA-43944
                    //foreach(NetworkRow r in NetworksGridView.Rows)
                        //r.UpdateDefaultCellStyle();  // Has to be done again after adding to the grid view, even though it's already called in the constructor

                    if (selectedNetwork != null)
                    {
                        foreach (NetworkRow row in NetworksGridView.Rows)
                        {
                            if (row.Network.opaque_ref == selectedNetwork.opaque_ref && selectedNetwork.Show(XenAdmin.Properties.Settings.Default.ShowHiddenVMs))
                            {
                                row.Selected = true;
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (NetworksGridView.SortedColumn != null)
                {
                    NetworksGridView.Sort(
                        NetworksGridView.SortedColumn,
                        NetworksGridView.SortOrder == SortOrder.Ascending
                            ? ListSortDirection.Ascending : ListSortDirection.Descending);
                }
                NetworksGridView.ResumeLayout();
                InBuildList = false;
            }
        }

        public void DeregisterEventsOnGridRows()
        {
            foreach (DataGridViewExRow row in NetworksGridView.Rows)
            {
                if (row is VifRow)
                    ((VifRow)row).DeregisterEvents();
                else if (row is NetworkRow)
                    ((NetworkRow)row).DeregisterEvents();
            }
        }


        private void NetworksGridView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateEnablement();
        }

        private void UpdateEnablement()
        {
            bool locked = _xenObject.Locked;

            if (SelectedNetwork != null)
            {
                XenAPI.Network TheNetwork = SelectedNetwork;

                AddNetworkButton.Enabled = !locked;

                EditNetworkButton.Enabled = !locked && !TheNetwork.Locked && !TheNetwork.IsSlave() && !TheNetwork.CreateInProgress()
                    && !TheNetwork.IsGuestInstallerNetwork();
                // CA-218956 - Expose HIMN when showing hidden objects
                // HIMN should not be editable

                if (HasPhysicalNonBondNIC(TheNetwork))
                {
                    RemoveNetworkButton.Enabled = false;
                    RemoveButtonContainer.SetToolTip(Messages.TOOLTIP_REMOVE_PIF);
                }
                else
                {
                    RemoveNetworkButton.Enabled = EditNetworkButton.Enabled;
                    // CA-218956 - Expose HIMN when showing hidden objects
                    // HIMN should not be removable

                    RemoveButtonContainer.SetToolTip("");
                }
            }
            else if (SelectedVif != null)
            {
                VIF vif = SelectedVif;
                AddNetworkButton.Enabled = !locked;
                // In this case read vif.currently_attached as is-it-plugged
                RemoveNetworkButton.Enabled = !locked && (vif.allowed_operations.Contains(vif_operations.unplug) || !vif.currently_attached);
                EditNetworkButton.Enabled = !locked && (vif.allowed_operations.Contains(vif_operations.unplug) || !vif.currently_attached);
                buttonActivateToggle.Enabled = !locked && (
                    vif.currently_attached && vif.allowed_operations.Contains(vif_operations.unplug)
                    || !vif.currently_attached && vif.allowed_operations.Contains(vif_operations.plug));

                buttonActivateToggle.Text = vif.currently_attached ? Messages.VM_NETWORK_TAB_DEACTIVATE_BUTTON_LABEL : Messages.VM_NETWORK_TAB_ACTIVATE_BUTTON_LABEL; 

                VM vm = (VM)XenObject;
                if (vm.power_state == vm_power_state.Suspended)
                {
                    RemoveButtonContainer.SetToolTip(Messages.TOOLTIP_REMOVE_NETWORK_SUSPENDED);
                    EditButtonContainer.SetToolTip(vm.HasNewVirtualisationStates() ? Messages.TOOLTIP_EDIT_NETWORK_IO_DRIVERS : Messages.TOOLTIP_EDIT_NETWORK_TOOLS);
                    toolTipContainerActivateToggle.SetToolTip(vif.currently_attached 
                        ? Messages.TOOLTIP_DEACTIVATE_VIF_SUSPENDED : Messages.TOOLTIP_ACTIVATE_VIF_SUSPENDED);
                }
                else
                {
                    if (vm.power_state == vm_power_state.Running && !vm.GetVirtualisationStatus().HasFlag(VM.VirtualisationStatus.IO_DRIVERS_INSTALLED))
                    {
                        RemoveButtonContainer.SetToolTip(vm.HasNewVirtualisationStates() ? Messages.TOOLTIP_REMOVE_NETWORK_IO_DRIVERS : Messages.TOOLTIP_REMOVE_NETWORK_TOOLS);
                        EditButtonContainer.SetToolTip(vm.HasNewVirtualisationStates() ? Messages.TOOLTIP_EDIT_NETWORK_IO_DRIVERS : Messages.TOOLTIP_EDIT_NETWORK_TOOLS);
                        toolTipContainerActivateToggle.SetToolTip(vif.currently_attached
                            ? Messages.TOOLTIP_DEACTIVATE_VIF_TOOLS : Messages.TOOLTIP_ACTIVATE_VIF_TOOLS);
                    }
                    else
                    {
                        RemoveButtonContainer.RemoveAll();
                        EditButtonContainer.RemoveAll();
                        toolTipContainerActivateToggle.RemoveAll();
                    }
                }
            }
            else
            {
                AddNetworkButton.Enabled = !locked;
                RemoveNetworkButton.Enabled = false;
                EditNetworkButton.Enabled = false;
                buttonActivateToggle.Enabled = false;
            }
        }

        /// <summary>
        /// Return true if the given network has a PIF that represents a physical NIC -- i.e.
        /// IsPhysical == true and IsBondNIC == false.
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        private static bool HasPhysicalNonBondNIC(XenAPI.Network network)
        {
            foreach (PIF pif in network.Connection.ResolveAll(network.PIFs))
            {
                if (pif.IsPhysical() && !pif.IsBondNIC())
                    return true;
            }
            return false;
        }

        private void AddNetworkButton_Click(object sender, EventArgs e)
        {
            if (XenObject is VM)
            {
                VM vm = (VM)_xenObject;

                if (NetworksGridView.Rows.Count >= vm.MaxVIFsAllowed())
                {
                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(
                            SystemIcons.Error,
                            FriendlyErrorNames.VIFS_MAX_ALLOWED,
                            FriendlyErrorNames.VIFS_MAX_ALLOWED_TITLE)))
                    {
                        dlg.ShowDialog(Program.MainWindow);
                    }
                    return;
                }

                Host master = Helpers.GetMaster(vm.Connection);
                if (master == null)
                {
                    // Cache populating?
                    return;
                }

                VIF pVif;
                using (var d = new VIFDialog(vm.Connection, null, VIF.GetDeviceId(vm), vm.HasSriovRecommendation()))
                {
                    if (d.ShowDialog(this) != DialogResult.OK)
                        return;

                    pVif = d.GetNewSettings();
                }

                pVif.VM = new XenRef<VM>(vm.opaque_ref);
                var action = new CreateVIFAction(vm, pVif);
                action.Completed += createVIFAction_Completed;
                action.RunAsync();
            }
            else if (XenObject is Host)
            {
                Host host = (Host)_xenObject;
                Program.MainWindow.ShowPerConnectionWizard(_xenObject.Connection,
                    new NewNetworkWizard(_xenObject.Connection, null, host));
            }
            else if (XenObject is Pool)
            {
                Pool pool = (Pool)_xenObject;
                Host host = pool.Connection.Resolve(pool.master);
                if (host != null)
                {
                    Program.MainWindow.ShowPerConnectionWizard(_xenObject.Connection,
                        new NewNetworkWizard(_xenObject.Connection, pool, host));
                }
            }
        }

        private void createVIFAction_Completed(ActionBase sender)
        {
            sender.Completed -= createVIFAction_Completed;

            if (sender is CreateVIFAction action && action.RebootRequired)
                Program.Invoke(Program.MainWindow, ShowHotPlugError);
        }

        private void ShowHotPlugError()
        {
            using (var dlg = new ThreeButtonDialog(
                new ThreeButtonDialog.Details(
                    SystemIcons.Information,
                    Messages.VIF_HOTPLUG_FAILED_MESSAGE,
                    Messages.VIF_HOTPLUG_FAILED_TITLE)))
            {
                dlg.ShowDialog(Program.MainWindow);
            }
        }

        private XenAPI.Network SelectedNetwork
        {
            get
            {
                if (NetworksGridView.SelectedRows.Count == 0)
                    return null;

                NetworkRow row = NetworksGridView.SelectedRows[0] as NetworkRow;

                if (row == null)
                    return null;

                return row.Network;
            }
        }

        private VIF SelectedVif
        {
            get
            {
                if (NetworksGridView.SelectedRows.Count == 0)
                    return null;

                VifRow row = NetworksGridView.SelectedRows[0] as VifRow;

                if (row == null)
                    return null;

                return row.Vif;
            }
        }

        private void RemoveNetworkButton_Click(object sender, EventArgs e)
        {
            if (NetworksGridView.SelectedRows.Count == 0)
                return;

            XenAPI.Network network = SelectedNetwork;
            if (network != null && network.IsBond())
            {
                var destroyBondCommand = new DestroyBondCommand(Program.MainWindow, network);
                destroyBondCommand.Execute();
            }
            else
            {
                // Check and see if the system is running in automation test mode.  If so, then
                // do not launch the popup Y/N dialog.
                DialogResult result;
                if (Program.RunInAutomatedTestMode)
                {
                    result = DialogResult.Yes;
                }
                else if (XenObject is VM)
                {
                    // Deleting a VIF, not a Network.
                    using (var dlg = new ThreeButtonDialog(
                                new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.MESSAGEBOX_VIF_DELETE, Messages.MESSAGEBOX_VIF_DELETE_TITLE),
                                ThreeButtonDialog.ButtonYes,
                                ThreeButtonDialog.ButtonNo))
                    {
                        result = dlg.ShowDialog(Program.MainWindow);
                    }
                }
                else
                {
                    using (var dlg = new ThreeButtonDialog(
                                new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.MESSAGEBOX_NETWORK_DELETE, Messages.MESSAGEBOX_NETWORK_DELETE_TITLE),
                                ThreeButtonDialog.ButtonYes,
                                ThreeButtonDialog.ButtonNo))
                    {
                        result = dlg.ShowDialog(Program.MainWindow);
                    }
                }

                if (result == DialogResult.Yes)
                {
                    DoRemoveNetwork();
                }
            }
        }

        private void DoRemoveNetwork()
        {
            if (SelectedVif != null)
            {
                VM vm = XenObject as VM;

                var action = new DeleteVIFAction(SelectedVif);
                action.Completed += action_Completed;
                action.RunAsync();
            }
            else if(SelectedNetwork != null)
            {
                NetworkAction action = new NetworkAction(XenObject.Connection, SelectedNetwork,false);
                action.Completed += action_Completed;
                action.RunAsync();
            }
        }

        private void buttonActivateToggle_Click(object sender, EventArgs e)
        {
            if (SelectedVif != null)
            {
                VM vm = XenObject as VM;
                AsyncAction action;
                if (SelectedVif.currently_attached)
                    action = new UnplugVIFAction(SelectedVif);
                else
                    action = new PlugVIFAction(SelectedVif);

                action.Completed += action_Completed;
                action.RunAsync();
            }
        }

        void NetworkCollectionChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, BuildList);
        }

        void PIFCollectionChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, BuildList);
        }

        void CollectionChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, BuildList);
        }

        void action_Completed(ActionBase sender)
        {
            Program.Invoke(this, BuildList);
        }

        void Server_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // JJN- Provide a temporary fix to the bug reported in CA-9322 where the
            // editing of the MAC and Limit fields is barely usable due to the continuous
            // reconstruction of the list within this view.  Ensure the button enablement
            // does not digress with this fix.
            if (shouldRefreshBuildList(e))
            {
                Program.Invoke(this, RefreshAllItems);
            }
        }

        private void RefreshAllItems()
        {
            foreach (DataGridViewExRow row in NetworksGridView.Rows)
            {
                if (row is VifRow)
                    ((VifRow)row).UpdateDetails();
                else if (row is NetworkRow)
                    ((NetworkRow)row).UpdateDetails();
            }
        }

        public static bool shouldRefreshBuildList(PropertyChangedEventArgs e)
        {
            return (e.PropertyName != "allowed_operations"
                 && e.PropertyName != "current_operations"
                 && e.PropertyName != "io_read_kbs"
                 && e.PropertyName != "io_write_kbs"
                 && e.PropertyName != "last_updated");
        }

        

        private void EditNetworkButton_Click(object sender, EventArgs e)
        {
            if (NetworksGridView.SelectedRows.Count > 0)
            {
                if (XenObject is VM)
                {
                    launchVmNetworkSettingsDialog();
                }
                else
                {
                    launchHostOrPoolNetworkSettingsDialog();
                }
            }
        }

        private void launchVmNetworkSettingsDialog()
        {
            VM vm = XenObject as VM;
            VIF vif = SelectedVif;

            if (vm == null || vif == null)
                return;

            int device;
            if (!int.TryParse(vif.device, out device))
            {
                log.ErrorFormat("Aborting vif edit. Could not parse existing vif device to int. Value is: '{0}'", vif.device);
                return;
            }

            VIF proxyVIF;
            using (var d = new VIFDialog(vm.Connection, vif, device, vm.HasSriovRecommendation()))
            {
                if (d.ShowDialog() != DialogResult.OK)
                    return;

                proxyVIF = d.GetNewSettings();
            }

            var action = new UpdateVIFAction(vm, vif, proxyVIF);
            action.Completed += updateVIFAction_Completed;
            InBuildList = true;
            action.RunAsync();
        }

        private void updateVIFAction_Completed(ActionBase sender)
        {
            sender.Completed -= updateVIFAction_Completed;

            if (sender is UpdateVIFAction action)
            {
                Program.Invoke(Program.MainWindow, () =>
                {
                    InBuildList = false;
                    BuildList();

                    if (action.RebootRequired)
                        ShowHotPlugError();
                });
            }
        }

        private void launchHostOrPoolNetworkSettingsDialog()
        {
            XenAPI.Network network = SelectedNetwork;
            if (network == null)
                return;

            new PropertiesDialog(network).ShowDialog(this);
        }

        private void EditMenuItemHandler(object sender, EventArgs e)
        {
            EditNetworkButton_Click(null, null);
        }

        private void AddMenuItemHandler(object sender, EventArgs e)
        {
            AddNetworkButton_Click(null, null);
        }

        private void RemoveMenuItemHandler(object sender, EventArgs e)
        {
            RemoveNetworkButton_Click(null, null);
        }

        private void NetworksGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            UpdateEnablement();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            Point pt = NetworksGridView.PointToClient(new Point(contextMenuStrip1.Left, contextMenuStrip1.Top));
            DataGridView.HitTestInfo info = NetworksGridView.HitTest(pt.X, pt.Y);
            if (info != null && info.RowIndex >= 0 && info.RowIndex < NetworksGridView.Rows.Count)
            {
                DataGridViewRow row = NetworksGridView.Rows[info.RowIndex];
                if (row != null)
                {
                    row.Selected = true;
                    copyToolStripMenuItem.Visible = true;
                    addToolStripMenuItem.Visible = AddNetworkButton.Visible && AddNetworkButton.Enabled;
                    propertiesToolStripMenuItem.Visible = EditNetworkButton.Visible && EditNetworkButton.Enabled;
                    removeToolStripMenuItem.Visible = RemoveNetworkButton.Visible && RemoveNetworkButton.Enabled;
                    return;
                }
            }

            // else just show add button
            copyToolStripMenuItem.Visible = false;
            addToolStripMenuItem.Visible = AddNetworkButton.Visible && AddNetworkButton.Enabled;
            propertiesToolStripMenuItem.Visible = false;
            removeToolStripMenuItem.Visible = false;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Point pt = NetworksGridView.PointToClient(new Point(contextMenuStrip1.Left, contextMenuStrip1.Top));
            DataGridView.HitTestInfo info = NetworksGridView.HitTest(pt.X, pt.Y);
            if (info != null && info.RowIndex >= 0 && info.RowIndex < NetworksGridView.Rows.Count)
            {
                DataGridViewRow row = NetworksGridView.Rows[info.RowIndex];
                if (row != null)
                {
                    string t = row.Cells[info.ColumnIndex].Value.ToString();
                    if (String.IsNullOrEmpty(t))
                        return;

                    Clip.SetClipboardText(t);
                }
            }
        }

        class VifRow : DataGridViewExRow
        {
            public VIF Vif;
            private DataGridViewExImageCell ImageCell = new DataGridViewExImageCell();
            private DataGridViewTextBoxCell DeviceCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell MacCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell LimitCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell NetworkCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell IpCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell AttachedCell = new DataGridViewTextBoxCell();

            private VIF_metrics Metrics;

            public VifRow(VIF vif)
            {
                Vif = vif;

                Cells.AddRange(ImageCell,
                               DeviceCell,
                               MacCell,
                               LimitCell,
                               NetworkCell,
                               IpCell,
                               AttachedCell);

                Vif.PropertyChanged += Server_PropertyChanged;

                UpdateDetails();
            }

            public void UpdateDetails()
            {
                if(Metrics != null)
                    Metrics.PropertyChanged -= Server_PropertyChanged;

                Metrics = Vif.Connection.Resolve(Vif.metrics);

                if (Metrics != null)
                    Metrics.PropertyChanged += Server_PropertyChanged;

                ImageCell.Value = Properties.Resources._000_Network_h32bit_16;
                DeviceCell.Value = Vif.device;
                MacCell.Value = Helpers.GetMacString(Vif.MAC);
                LimitCell.Value = Vif.qos_algorithm_type != "" ? Vif.LimitString() : "";
                NetworkCell.Value = Vif.NetworkName();
                IpCell.Value = Vif.IPAddressesAsString();
                AttachedCell.Value = Vif.currently_attached ? Messages.YES : Messages.NO;
            }

            public void DeregisterEvents()
            {
                Vif.PropertyChanged -= Server_PropertyChanged;

                if (Metrics != null)
                    Metrics.PropertyChanged -= Server_PropertyChanged;
            }

            private void Server_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (NetworkList.shouldRefreshBuildList(e))
                    Program.Invoke(Program.MainWindow, UpdateDetails);
            }
        }

        class NetworkRow : DataGridViewExRow
        {
            public XenAPI.Network Network;
            private DataGridViewExImageCell ImageCell = new DataGridViewExImageCell();
            private DataGridViewTextBoxCell NameCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell DescriptionCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell NicCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell VlanCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell AutoCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell LinkStatusCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell MacCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell MtuCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell SriovCell = new DataGridViewTextBoxCell();
            private IXenObject Xmo;
            PIF Pif;

            public NetworkRow(XenAPI.Network network, IXenObject xmo)
            {
                Network = network;
                Xmo = xmo;

                Cells.AddRange(ImageCell,
                               NameCell,
                               DescriptionCell,
                               NicCell,
                               VlanCell,
                               AutoCell,
                               LinkStatusCell,
                               MacCell,
                               MtuCell,
                               SriovCell);

                Network.PropertyChanged += Server_PropertyChanged;

                Program.Invoke(Program.MainWindow, UpdateDetails);
            }

            public void UpdateDetails()
            {
                Enabled = !Network.IsSlave();

                DeregisterPifEvents();

                Pif = Helpers.FindPIF(Network, Xmo as Host);

                RegisterPifEvents();

                ImageCell.Value = Properties.Resources._000_Network_h32bit_16;
                NameCell.Value = NetworkName();
                DescriptionCell.Value = Network.Description();
                NicCell.Value = Helpers.GetName(Pif);
                VlanCell.Value = Helpers.VlanString(Pif);
                AutoCell.Value = Network.GetAutoPlug() ? Messages.YES : Messages.NO;
                LinkStatusCell.Value = Xmo is Pool ? Network.LinkStatusString() : 
                    Pif == null ? Messages.NONE : Pif.LinkStatusString();
                MacCell.Value = Pif != null && Pif.IsPhysical() ? Pif.MAC : Messages.SPACED_HYPHEN;
                MtuCell.Value = Network.CanUseJumboFrames() ? Network.MTU.ToString() : Messages.SPACED_HYPHEN;

                var networkSriov = Pif != null ? Pif.NetworkSriov() : null;
                if(networkSriov == null)
                {
                    SriovCell.Value = Messages.NO;
                }
                else
                {
                    var sriov = Pif.Connection.Resolve(networkSriov);
                    SriovCell.Value = sriov == null
                        ? Messages.NO
                        : sriov.requires_reboot
                            ? Messages.HOST_NEEDS_REBOOT_ENABLE_SRIOV
                            : Messages.YES;
                }
            }

            public void DeregisterEvents()
            {
                Network.PropertyChanged -= Server_PropertyChanged;

                DeregisterPifEvents();
            }

            private object NetworkName()
            {
                bool isSlave = Network.IsSlave();
                if (Network.Show(XenAdmin.Properties.Settings.Default.ShowHiddenVMs) && !isSlave)
                    return Helpers.GetName(Network);
                else if (isSlave && Properties.Settings.Default.ShowHiddenVMs)
                    return string.Format(Messages.NIC_SLAVE, Helpers.GetName(Network));
                else if (Properties.Settings.Default.ShowHiddenVMs)
                    return string.Format(Messages.NIC_HIDDEN, Helpers.GetName(Network));
                else
                    return string.Empty;
            }

            private void Server_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if(NetworkList.shouldRefreshBuildList(e))
                    Program.Invoke(Program.MainWindow, UpdateDetails);
            }

            private void RegisterPifEvents()
            {
                if (Pif != null)
                {
                    Pif.PropertyChanged += Server_PropertyChanged;

                    // Listen for Tunnel and PIF_metrics changes which is necessary for Link Status updates (CA-46103)
                    if (Pif.IsTunnelAccessPIF())
                    {
                        Tunnel tunnel = Pif.Connection.Resolve(Pif.tunnel_access_PIF_of[0]);
                        if (tunnel != null)
                            tunnel.PropertyChanged += Pif_PropertyChanged;
                    }
                    else
                    {
                        PIF_metrics metrics = Pif.PIFMetrics();
                        if (metrics != null)
                            metrics.PropertyChanged += Pif_PropertyChanged;
                    }
                }
            }

            private void DeregisterPifEvents()
            {
                if (Pif != null)
                {
                    Pif.PropertyChanged -= Server_PropertyChanged;

                    // Remove Tunnel and PIF_metrics property change listeners (CA-46103)
                    if (Pif.IsTunnelAccessPIF())
                    {
                        Tunnel tunnel = Pif.Connection.Resolve(Pif.tunnel_access_PIF_of[0]);
                        if (tunnel != null)
                            tunnel.PropertyChanged -= Pif_PropertyChanged;
                    }
                    else
                    {
                        PIF_metrics metrics = Pif.PIFMetrics();
                        if (metrics != null)
                            metrics.PropertyChanged -= Pif_PropertyChanged;
                    }
                }
            }

            private void Pif_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                // Tunnel.status or PIF_metrics.carrier (CA-46103)
                if (e.PropertyName == "status" || e.PropertyName == "carrier")
                    Program.Invoke(Program.MainWindow, UpdateDetails);
            }
        }

        private void NetworksGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (XenObject != null && XenObject is VM && e.Column.Index == DeviceColumn.Index)
            {
                int val1, val2;
                if (e.CellValue1 != null && int.TryParse(e.CellValue1.ToString(), out val1)
                    && e.CellValue2 != null && int.TryParse(e.CellValue2.ToString(), out val2))
                {
                    e.SortResult = val1.CompareTo(val2);
                    e.Handled = true;
                    return;
                }
            }

            if (e.CellValue1 == null && e.CellValue2 == null)
                e.SortResult = 0;
            else if (e.CellValue1 == null && e.CellValue2 != null)
                e.SortResult = 1;
            else if (e.CellValue1 != null && e.CellValue2 == null)
                e.SortResult = -1;
            else
                e.SortResult = StringUtility.NaturalCompare(e.CellValue1.ToString(), e.CellValue2.ToString());

            e.Handled = true;
        }
    }
}
