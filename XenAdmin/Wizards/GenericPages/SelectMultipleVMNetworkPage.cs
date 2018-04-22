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
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Mappings;
using XenAdmin.Network;
using XenAPI;
using System.Linq;
using XenCenterLib;

namespace XenAdmin.Wizards.GenericPages
{
	/// <summary>
	/// Class representing the page of the ImportAppliance wizard where the user specifies
	/// a network for the VMs in the appliance that require network access. 
	/// </summary>
	internal abstract partial class SelectMultipleVMNetworkPage : XenTabPage
	{
        private bool m_buttonNextEnabled;
        private bool m_buttonPreviousEnabled;

		private class NetworkDetail
		{
			public string SysId { get; set; }
			public string NetworkId { get; set; }
		}
		
		private Dictionary<string, VmMapping> m_vmMappings;

        public SelectMultipleVMNetworkPage()
		{
			InitializeComponent();
            InitializeText();
		}

        public void InitializeText()
        {
            m_labelIntro.Text = IntroductionText;
            label2.Text = TableIntroductionText;
            m_colVmNetwork.HeaderText = NetworkColumnHeaderText;
        }

        protected virtual string NetworkColumnHeaderText
        {
            get
            {
                return m_colVmNetwork.HeaderText;
            }
        }

	    public abstract string IntroductionText { get; }
        public abstract string TableIntroductionText { get; }

        private IXenConnection targetConnection;
        /// <summary>
        /// The connection from which the target networks are selected
        /// Defaults to the base class connection if not set
        /// </summary>
        public IXenConnection TargetConnection
        {
            get
            {
                if (targetConnection == null)
                    return Connection;
                return targetConnection;
            }
            set { targetConnection = value; }
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            targetConnection = null;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
		{
            SetButtonPreviousEnabled(true);
			SetButtonNextEnabled(true);
		}

	    public DataGridView DataTable
	    {
            get { return m_dataGridView; }
	    }

        public override void PopulatePage()
		{
			m_dataGridView.Rows.Clear();
			SetButtonNextEnabled(true);
			FillTableRows();
            HelpersGUI.ResizeLastGridViewColumn(m_colTargetNet);//set properly the width of the last column
		}

	    public abstract NetworkResourceContainer NetworkData(string sysId);

        private void FillTableRows()
        {
            foreach (var kvp in VmMappings)
            {
                string sysId = kvp.Key;
                var vmMapping = kvp.Value;

                var cb = FillGridComboBox(vmMapping.XenRef, sysId);

                foreach (INetworkResource networkResource in NetworkData(sysId))
                {
                    var cellVmNetwork = CreateFormattedNetworkCell(sysId, networkResource.NetworkID, vmMapping.VmNameLabel,
                                                                   networkResource.NetworkName, networkResource.MACAddress);

                    DataGridViewRow row = new DataGridViewRow();
                    row.Cells.Add(cellVmNetwork);

                    var cbClone = (DataGridViewComboBoxCell)cb.Clone();

                    if (cbClone.Items.Count > 0)
                    {
                        cbClone.DisplayMember = ToStringWrapper<XenAPI.Network>.DisplayMember;
                            //this is the ToStringProperty
                        cbClone.ValueMember = ToStringWrapper<XenAPI.Network>.ValueMember;
                            //this is the ToStringWrapper<XenAPI.Network> object itself
                        cbClone.Value = cb.Items[0]; // Default selection of the combobox cell

                        //Select the network if the names of the target and source networks match in the combobox cell
                        foreach (ToStringWrapper<XenAPI.Network> item in cb.Items)
                        {
                            if (item.ToStringProperty == networkResource.NetworkName)
                                cbClone.Value = item;
                        }

                        row.Cells.Add(cbClone);
                    }
                    else
                    {
                        var cellError = new DataGridViewTextBoxCell { Value = Messages.IMPORT_SELECT_NETWORK_PAGE_NO_AVAIL_NETWORKS };
                        row.Cells.Add(cellError);
                        cellError.ReadOnly = true; //this has to be set after the cell is added to a row
                        SetButtonNextEnabled(false);
                    }

                    DataTable.Rows.Add(row);
                }
            }
        }

        protected DataGridViewTextBoxCell CreateFormattedNetworkCell(string sysID, string networkID, string vmName, string netName, string macAddress)
        {
            return new DataGridViewTextBoxCell
            {
                Tag = new NetworkDetail { SysId = sysID, NetworkId = networkID },
                Value = string.Format("{0} - {1} ({2})", vmName, netName, macAddress)
            };
        }

	    public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

        public override bool EnablePrevious()
        {
            return m_buttonPreviousEnabled;
        }

		public Dictionary<string, VmMapping> VmMappings
		{
			get
			{
				foreach (DataGridViewRow row in m_dataGridView.Rows)
				{
					var networkDetail = (NetworkDetail)row.Cells[0].Tag;

					if (m_vmMappings.ContainsKey(networkDetail.SysId))
					{
						var mapping = m_vmMappings[networkDetail.SysId];
						var selectedItem = row.Cells[1].Value as ToStringWrapper<XenAPI.Network>;

					    if (selectedItem != null)
							mapping.Networks[networkDetail.NetworkId] = selectedItem.item;
					}
				}

				return m_vmMappings;
			}
			set
			{
			    m_vmMappings = value;
                InitializeText();
			}
		}

        protected void SetButtonNextEnabled(bool enabled)
        {
            m_buttonNextEnabled = enabled;
            OnPageUpdated();
        }

        protected void SetButtonPreviousEnabled(bool enabled)
        {
            m_buttonPreviousEnabled = enabled;
            OnPageUpdated();
        }   

		private DataGridViewComboBoxCell FillGridComboBox(object xenRef, string vsId)
		{
		    var cb = new DataGridViewComboBoxCell {FlatStyle = FlatStyle.Flat, Sorted = true};

			XenRef<Host> hostRef = xenRef as XenRef<Host>;
			Host host = TargetConnection.Resolve(hostRef);

            var availableNetworks = TargetConnection.Cache.Networks.Where(net => ShowNetwork(host, net, vsId));

			foreach (XenAPI.Network netWork in availableNetworks)
			{
				if (!Messages.IMPORT_SELECT_NETWORK_PAGE_NETWORK_FILTER.Contains(netWork.Name()))
				{
					var wrapperItem = new ToStringWrapper<XenAPI.Network>(netWork, netWork.Name());

					if (!cb.Items.Contains(wrapperItem))
						cb.Items.Add(wrapperItem);
				}
			}

			return cb;
		}

        private bool ShowNetwork(Host targetHost, XenAPI.Network network, string vsId)
        {
            if (network.IsSriov() && !AllowSriovNetwork(network, vsId))
                return false;

            if (!network.Show(Properties.Settings.Default.ShowHiddenVMs))
                return false;

            if (network.IsSlave())
                return false;

            if (targetHost != null && !targetHost.CanSeeNetwork(network))
                return false;

            if (targetHost == null && !network.AllHostsCanSeeNetwork())
                return false;

            return true;
        }

        protected virtual bool AllowSriovNetwork(XenAPI.Network network, string sysId)
	    {
	        return true;
	    }

	    private void m_dataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != m_colTargetNet.Index || e.RowIndex < 0 || e.RowIndex >= m_dataGridView.RowCount)
				return;

			m_dataGridView.BeginEdit(false);

			if (m_dataGridView.EditingControl != null && m_dataGridView.EditingControl is ComboBox)
				(m_dataGridView.EditingControl as ComboBox).DroppedDown = true;
		}

		private void m_dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			m_dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
			IsDirty = true;
		}
	}
}
