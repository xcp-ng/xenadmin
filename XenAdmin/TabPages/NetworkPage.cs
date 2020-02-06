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
using System.Windows.Forms;

using XenAPI;
using XenAdmin.Core;
using XenAdmin.Dialogs;


namespace XenAdmin.TabPages
{
    public partial class NetworkPage : BaseTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // We don't rebuild the controls while the tab is not visible, but instead queue it up later for when the page is displayed.
        private bool refreshNeeded = false;

        private IXenObject _xenObject;
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

                if (value is Pool)
                {
                    base.Text = Messages.POOL_NETWORK_TAB_TITLE;
                    panelManagementInterfaces.Visible = true;
                    tableLayoutPanel1.RowStyles[1].SizeType = SizeType.Percent;
                }
                else if (value is Host)
                {
                    base.Text = Messages.HOST_NETWORK_TAB_TITLE;
                    panelManagementInterfaces.Visible = true;
                    tableLayoutPanel1.RowStyles[1].SizeType = SizeType.Percent;
                }
                else
                {
                    base.Text = Messages.VM_NETWORK_TAB_TITLE;
                    panelManagementInterfaces.Visible = false;
                    tableLayoutPanel1.RowStyles[1].SizeType = SizeType.AutoSize;
                }

                //Management Interfaces
                UnregisterHandlers();
                _xenObject = value;
                if (_xenObject != null)
                    _xenObject.Connection.Cache.RegisterBatchCollectionChanged<PIF>(CollectionChanged);
                RepopulateManagementInterfaces();

                networkList1.XenObject = value;
                if (_xenObject != null && Helpers.HAEnabled(_xenObject.Connection))
                {
                    button1.Enabled = false;
                    toolTipContainerConfigureButton.SetToolTip(Messages.DISABLE_HA_CONFIGURE_MANAGEMENT_INTERFACES);
                }
                else
                {
                    button1.Enabled = true;
                    toolTipContainerConfigureButton.RemoveAll();
                }
            }
        }

        public NetworkPage()
        {
            InitializeComponent();
        }

        public override string HelpID => "TabPageNetwork";

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (refreshNeeded)
            {
                RepopulateManagementInterfaces();
                refreshNeeded = false;
            }
            base.OnVisibleChanged(e);
        }

        private void UnregisterHandlers()
        {
            if (_xenObject == null) 
                return;
            _xenObject.Connection.Cache.DeregisterBatchCollectionChanged<PIF>(CollectionChanged);
        }

        public override void PageHidden()
        {
            UnregisterHandlers();
            networkList1.UnregisterHandlers();
        }


        #region Management Interfaces

        public void RepopulateManagementInterfaces()
        {
            if (!Visible)
            {
                refreshNeeded = true;
                return;
            }
            dataGridViewEx1.SuspendLayout();
            try
            {
                dataGridViewEx1.Rows.Clear();

                List<Host> hostList = new List<Host>();
                if (_xenObject is Host)
                    hostList.Add((Host)_xenObject);
                else if (_xenObject is Pool)
                    hostList.AddRange(_xenObject.Connection.Cache.Hosts);
                if (hostList.Count == 0)
                    return;

                List<PIF> pifList = new List<PIF>(_xenObject.Connection.Cache.PIFs);
                hostList.Sort();  // usual order, i.e. master first
                pifList.Sort();  // This sort ensures that the primary PIF comes before other management PIFs

                List<PIFRow> rows = new List<PIFRow>();
                foreach (Host _host in hostList)
                {
                    foreach (PIF pif in pifList)
                    {
                        if (pif.host.opaque_ref != _host.opaque_ref)
                            continue;

                        if (pif.IsManagementInterface(XenAdmin.Properties.Settings.Default.ShowHiddenVMs))
                            rows.Add(new PIFRow(_host, pif));
                    }
                }
                dataGridViewEx1.Rows.AddRange(rows.ToArray());

                //CA-47050: the dnsColumn should be autosized to Fill, but should not become smaller than a minimum
                //width, which is chosen to be the column's contents (including header) width. To find what this is
                //set temporarily the column's autosize mode to AllCells.
                dnsColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                int storedWidth = dnsColumn.Width;
                dnsColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dnsColumn.MinimumWidth = storedWidth;
            }
            catch (Exception e)
            {
                log.ErrorFormat("Error encountered when building management interfaces list: {0}", e);
            }
            finally
            {
                dataGridViewEx1.ResumeLayout();
            }

        }

        protected class PIFRow : DataGridViewRow
        {
            private PIF pif;
            public PIF PIF
            {
                get { return pif; }
            }

            public PIFRow(Host host, PIF pif)
            {
                this.pif = pif;

                // cell for the name of the management interface 
                DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
                nameCell.Value = Helpers.GetName(host);
                Cells.Add(nameCell);

                // the icon cell 
                DataGridViewImageCell iconCell = new DataGridViewImageCell();
                iconCell.Value = Images.GetImage16For(pif);
                Cells.Add(iconCell);

                // the management purpose cell
                DataGridViewTextBoxCell interfaceCell = new DataGridViewTextBoxCell();
                string purpose;
                if (pif.management)
                {
                    purpose = Messages.MANAGEMENT;
                }
                else
                {
                    var managementPurpose = pif.GetManagementPurpose();
                    purpose = string.IsNullOrEmpty(managementPurpose) ? Messages.NETWORKING_PROPERTIES_PURPOSE_UNKNOWN : managementPurpose;
                }
                interfaceCell.Value = purpose;
                Cells.Add(interfaceCell);

                // the network cell
                DataGridViewTextBoxCell networkCell = new DataGridViewTextBoxCell();
                networkCell.Value = host.Connection.Resolve(pif.network).ToString();
                Cells.Add(networkCell);

                // the NIC cell
                DataGridViewTextBoxCell nicCell = new DataGridViewTextBoxCell();
                nicCell.Value = Helpers.GetName(pif);
                Cells.Add(nicCell);

                // the IP Setup cell
                DataGridViewTextBoxCell ipSetupCell = new DataGridViewTextBoxCell();
                ipSetupCell.Value = pif.IpConfigurationModeString();
                Cells.Add(ipSetupCell);

                // the ip address of the interface
                DataGridViewTextBoxCell ipCell = new DataGridViewTextBoxCell();
                ipCell.Value = pif.IP;
                Cells.Add(ipCell);
                // the subnet mask address of the interface
                DataGridViewTextBoxCell subnetCell = new DataGridViewTextBoxCell();
                subnetCell.Value = pif.netmask;
                Cells.Add(subnetCell);

                // the gateway address of the interface
                DataGridViewTextBoxCell gatewayCell = new DataGridViewTextBoxCell();
                gatewayCell.Value = pif.gateway;
                Cells.Add(gatewayCell);
                // the dns address of the interface
                DataGridViewTextBoxCell dnsCell = new DataGridViewTextBoxCell();
                dnsCell.Value = pif.DNS;
                Cells.Add(dnsCell);
            }
        }

        private void buttonManagementInterfaces_Click(object sender, EventArgs e)
        {
            PIF selectedPIF = null;
            if (dataGridViewEx1.SelectedRows.Count == 1)
            {
                selectedPIF = ((PIFRow)dataGridViewEx1.SelectedRows[0]).PIF;
            }


            NetworkingProperties interfacesDialog = null;
            Host host = _xenObject as Host;
            if (host != null)
            {
                interfacesDialog = new NetworkingProperties(host, selectedPIF);
            }
            else
            {
                System.Diagnostics.Trace.Assert(_xenObject is Pool);
                Pool pool = (Pool)_xenObject;
                if (pool.Connection.Cache.HostCount <= 1)
                {
                    // Standalone host -- we redirect to the per-host dialog
                    Host _host = pool.Connection.Resolve(pool.master);
                    interfacesDialog = new NetworkingProperties(_host, selectedPIF);
                }
                else
                    interfacesDialog = new NetworkingProperties(pool, selectedPIF);
            }

            interfacesDialog.ShowDialog(this);
        }

        void CollectionChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, RepopulateManagementInterfaces);
        }
        #endregion
    }
}
