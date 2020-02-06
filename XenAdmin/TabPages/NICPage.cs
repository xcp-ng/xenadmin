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

using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Commands;


namespace XenAdmin.TabPages
{
    internal partial class NICPage : BaseTabPage
    {
        private Host host = null;

        public NICPage()
        {
            InitializeComponent();

            base.Text = Messages.NIC_TAB_TITLE;
            PIF_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(PIF_CollectionChanged);
        }

        public override string HelpID => "TabPageNICs";

        private readonly CollectionChangeEventHandler PIF_CollectionChangedWithInvoke;

        public Host Host
        {
            get
            {
                return host;
            }
            set
            {
                UnregisterHandlers();

                host = value;

                if (host != null)
                {
                    host.Connection.Cache.RegisterCollectionChanged<PIF>(PIF_CollectionChangedWithInvoke);
                }

                updateList();
                CreateBondButton.Enabled = host != null;
            }
        }

        void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            Program.Invoke(Program.MainWindow, updateList);
        }

        void PIF_PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "current_operations" &&
                e.PropertyName != "allowed_operations")
            {
                Program.Invoke(Program.MainWindow, updateList);
            }
        }

        void PIF_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Remove)
            {
                PIF pif = e.Element as PIF;
                UnregisterPIFEventHandlers(pif);
            }
            Program.Invoke(this, updateList);
        }

        private void RegisterPIFEventHandlers(PIF pif)
        {
            pif.PropertyChanged -= PIF_PropertyChangedEventHandler;
            pif.PropertyChanged += PIF_PropertyChangedEventHandler;

            var pifMetrics = pif.PIFMetrics();
            if (pifMetrics != null)
            {
                pifMetrics.PropertyChanged -= PropertyChangedEventHandler;
                pifMetrics.PropertyChanged += PropertyChangedEventHandler;
            }
        }

        private void UnregisterPIFEventHandlers(PIF pif)
        {
            pif.PropertyChanged -= PIF_PropertyChangedEventHandler;
            var pifMetrics = pif.PIFMetrics();

            if (pifMetrics != null)
                pifMetrics.PropertyChanged -= PropertyChangedEventHandler;
        }

        private void UnregisterHandlers()
        {
            if (host == null)
                return;

            host.Connection.Cache.DeregisterCollectionChanged<PIF>(PIF_CollectionChangedWithInvoke);
            foreach (PIF PIF in host.Connection.ResolveAll(host.PIFs))
            {
                UnregisterPIFEventHandlers(PIF);
            }
        }

        public override void PageHidden()
        {
            UnregisterHandlers();
        }


        private void updateList()
        {
            PIFRow selected = null;
            if (dataGridView1.SelectedRows.Count > 0)
                selected = dataGridView1.SelectedRows[0] as PIFRow;

            try
            {
                dataGridView1.SuspendLayout();
                dataGridView1.Rows.Clear();

                if (host != null)
                {
                    foreach (PIF PIF in host.Connection.ResolveAll(host.PIFs))
                    {
                        if (!PIF.IsPhysical())
                            continue;

                        RegisterPIFEventHandlers(PIF);

                        PIFRow p = new PIFRow(PIF);
                        dataGridView1.Rows.Add(p);
                        if (selected != null && p.Pif == selected.Pif)
                            p.Selected = true;
                    }

                    //show the FCoE column for Dundee or higher hosts only
                    ColumnFCoECapable.Visible = Helpers.DundeeOrGreater(host);

                    //show the SR-IOV column for Kolkata or higher hosts only
                    ColumnSriovCapable.Visible = Helpers.KolkataOrGreater(host);

                    HelpersGUI.ResizeGridViewColumnToHeader(ColumnDeviceName);
                    HelpersGUI.ResizeGridViewColumnToHeader(ColumnSriovCapable);

                    if (dataGridView1.SortedColumn != null)
                        dataGridView1.Sort(dataGridView1.SortedColumn, dataGridView1.SortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
                }
            }
            finally
            {
                dataGridView1.ResumeLayout();
            }
        }

        private class PIFRow : DataGridViewRow
        {
            public readonly PIF Pif;

            private readonly DataGridViewTextBoxCell _cellName = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _cellMac = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _cellConnected = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _cellSpeed = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _cellDuplex = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _cellVendor = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _cellDevice = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _cellBusPath = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _cellFcoe = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _cellSriov = new DataGridViewTextBoxCell();
            
            public PIFRow(PIF pif)
            {
                Pif = pif;
                Cells.AddRange(_cellName, _cellMac, _cellConnected, _cellSpeed, _cellDuplex,
                    _cellVendor, _cellDevice, _cellBusPath, _cellFcoe, _cellSriov);
                Update();
            }            

            private void Update()
            {
                _cellName.Value = Pif.Name();
                _cellMac.Value = Pif.MAC;
                _cellConnected.Value = Pif.Carrier() ? Messages.CONNECTED : Messages.DISCONNECTED;
                _cellSpeed.Value = Pif.Carrier() ? Pif.Speed() : Messages.HYPHEN;
                _cellDuplex.Value = Pif.Carrier() ? Pif.Duplex() : Messages.HYPHEN;

                var pifMetrics = Pif.PIFMetrics();
                _cellVendor.Value = pifMetrics == null ? Messages.HYPHEN : pifMetrics.vendor_name;
                _cellDevice.Value = pifMetrics == null ? Messages.HYPHEN : pifMetrics.device_name;
                _cellBusPath.Value = pifMetrics == null ? Messages.HYPHEN : pifMetrics.pci_bus_path;

                _cellFcoe.Value = Pif.FCoECapable().ToYesNoStringI18n();

                if (!Pif.SriovCapable())
                {
                    _cellSriov.Value = Messages.NO;
                }
                else if (!Pif.IsSriovPhysicalPIF())
                {
                    _cellSriov.Value = Messages.SRIOV_NETWORK_SHOULD_BE_CREATED;
                }
                else
                {
                    var networkSriov = Pif.Connection.Resolve(Pif.sriov_physical_PIF_of[0]);

                    if(networkSriov == null || networkSriov.requires_reboot)
                    {
                        _cellSriov.Value = Messages.HOST_NEEDS_REBOOT_ENABLE_SRIOV;
                        return;
                    }

                    PIF sriovLogicalPif = Pif.Connection.Resolve(networkSriov.logical_PIF);

                    if (sriovLogicalPif == null || !sriovLogicalPif.currently_attached)
                    {
                        _cellSriov.Value = Messages.SRIOV_LOGICAL_PIF_UNPLUGGED;
                        return;
                    }

                    var sriovSupported = "";
                    var action = new DelegatedAsyncAction(Pif.Connection, "", "", "", delegate(Session session)
                        {
                            try
                            {
                                var remainingCapacity = Network_sriov.get_remaining_capacity(session, Pif.sriov_physical_PIF_of[0].opaque_ref);
                                sriovSupported = string.Format(Messages.REMAINING_VFS, remainingCapacity);
                            }
                            catch
                            {
                                sriovSupported = Messages.YES;
                            }
                        },
                        true);

                    action.Completed += delegate
                    {
                        Program.Invoke(Program.MainWindow, () => _cellSriov.Value = sriovSupported);
                    };
                    action.RunAsync();
                }
            }
        }

        private void datagridview_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                DeleteBondButton.Enabled = false;
                return;
            }

            PIF pif = ((PIFRow)dataGridView1.SelectedRows[0]).Pif;
            if (pif == null)
            {
                DeleteBondButton.Enabled = false;
                return;
            }

            var bondMasterOf = pif.BondMasterOf();
            DeleteBondButton.Enabled = bondMasterOf != null && !bondMasterOf.Locked;
        }

        private void CreateBondButton_Click(object sender, EventArgs e)
        {
            BondProperties dialog = new BondProperties();
            dialog.SetHost(host);
            dialog.ShowDialog(Program.MainWindow);
        }

        private void DeleteBondButton_Click(object sender, EventArgs e)
        {
            PIF pif = ((PIFRow)dataGridView1.SelectedRows[0]).Pif;
            System.Diagnostics.Trace.Assert(pif.IsBondNIC());
            XenAPI.Network network = pif.Connection.Resolve(pif.network);
            var destroyBondCommand = new DestroyBondCommand(Program.MainWindow, network);
            destroyBondCommand.Execute();
        }

        private void buttonRescan_Click(object sender, EventArgs e)
        {
            RescanPIFsCommand cmd = new RescanPIFsCommand(Program.MainWindow, host);
            if (cmd.CanExecute())
                cmd.Execute();
        }

        private void CopyItemClick(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(dataGridView1.SelectedRows[0].Cells[1].Value);
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            DataGridView.HitTestInfo i = dataGridView1.HitTest(e.X, e.Y);
            if (i == DataGridView.HitTestInfo.Nowhere || i.RowIndex < 0 || i.ColumnIndex < 0)
                return;
            dataGridView1.Rows[i.RowIndex].Selected = true;
            dataGridView1.Focus();

            contextMenuStrip1.Items.Clear();
            var deleteItem = new ToolStripMenuItem(DeleteBondButton.Text, null, DeleteBondButton_Click) {Enabled = DeleteBondButton.Enabled};
            var copyItem = new ToolStripMenuItem(Messages.COPY_MENU_ITEM, null, CopyItemClick);
            contextMenuStrip1.Items.Add(deleteItem);
            contextMenuStrip1.Items.Add(new ToolStripSeparator());
            contextMenuStrip1.Items.Add(copyItem);

            contextMenuStrip1.Show(dataGridView1, dataGridView1.PointToClient(MousePosition));
        }
    }
}
