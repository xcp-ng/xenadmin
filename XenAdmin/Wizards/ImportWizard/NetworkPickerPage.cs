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
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAPI;
using XenCenterLib;


namespace XenAdmin.Wizards.ImportWizard
{
	public partial class NetworkPickerPage : XenTabPage
	{
		#region Private fields
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		
		private IXenConnection m_selectedConnection;
        private Host m_selectedAffinity;
        private VM m_vm;
        private bool m_buttonNextEnabled;
		#endregion
		
		public NetworkPickerPage()
		{
			InitializeComponent();
			m_invalidMacLabel.Visible = false;
		}

		#region Base class (XenTabPage) overrides

		/// <summary>
		/// Gets the page's title (headline)
		/// </summary>
		public override string PageTitle { get { return Messages.IMPORT_SELECT_NETWORK_PAGE_TITLE; } }

		/// <summary>
		/// Gets the page's label in the (left hand side) wizard progress panel
		/// </summary>
		public override string Text { get { return Messages.IMPORT_SELECT_NETWORK_PAGE_TEXT; } }

		/// <summary>
		/// Gets the value by which the help files section for this page is identified
		/// </summary>
		public override string HelpID { get { return "NetworkPicker"; } }

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
		{
		    m_buttonNextEnabled = true;
		}

        public override void PopulatePage()
		{
			SetNetworkList();
			BuildVIFList();
		}

		public override bool EnablePrevious()
		{
			return false;
		}

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

		#endregion

		#region Accessors

		public List<VIF> VIFs
		{
			get
			{
				var vifs = new List<VIF>();

				foreach (DataGridViewRow row in m_networkGridView.Rows)
				{
                    var vifRow = row as VifRow;
                    if (vifRow == null)
                        continue;

                    VIF vif = vifRow.Vif;
                    if (vif == null)
                        continue;

					if (vif.MAC == Messages.MAC_AUTOGENERATE)
						vif.MAC = "";

					vifs.Add(vif);
				}

				return vifs;
			}
		}

		#endregion

		public void SetAffinity(Host host)
		{
			m_selectedAffinity = host;
		}

		public void SetVm(VM vm)
		{
			m_vm = vm;
		}
		
		/// <summary>
		/// Should be called before the Affinity is set.
		/// </summary>
		public void SetConnection(IXenConnection con)
		{
			m_selectedConnection = con;
		}

		#region Private methods

        private void UpdateControlsEnabledState(bool enabled)
        {
            m_buttonNextEnabled = enabled;
            m_invalidMacLabel.Visible = !enabled;
            OnPageUpdated();
        }

		private void SetNetworkList()
        {
            NetworkNetworkColumn.Items.Clear();

        	var networks = m_selectedConnection.Cache.Networks.Where(ShowNetwork);

			foreach (XenAPI.Network network in networks)
            {
                NetworkNetworkColumn.Items.Add(new ToStringWrapper<XenAPI.Network>(network, network.Name()));
            }

            NetworkNetworkColumn.DisplayMember = ToStringWrapper<XenAPI.Network>.DisplayMember;
            NetworkNetworkColumn.ValueMember = ToStringWrapper<XenAPI.Network>.ValueMember;
            NetworkNetworkColumn.Sorted = true;
        }

		private void BuildVIFList()
		{
			try
			{
				m_networkGridView.SuspendLayout();
				m_networkGridView.Rows.Clear();

				if (m_vm.is_a_template && m_vm.VIFs.Count < 1)
				{
					// We need to automatically generate VIFs for Networks marked AutoPlug=true

					var networks = m_selectedConnection.Cache.Networks;
					foreach (XenAPI.Network network in networks)
					{
						if (m_networkGridView.Rows.Count < m_vm.MaxVIFsAllowed() && ShowNetwork(network) && network.GetAutoPlug())
						{
							AddVIFRow(new VIF
							          	{
							          		Connection = m_selectedConnection,
							          		device = m_networkGridView.Rows.Count.ToString(),
							          		network = new XenRef<XenAPI.Network>(network.opaque_ref),
							          		MAC = Messages.MAC_AUTOGENERATE
							          	});
						}
					}
				}
				else if (m_vm.is_a_template)
				{
					// We need to create off the _vmTemplate

					var vifs = m_selectedConnection.ResolveAll(m_vm.VIFs);
					foreach (VIF vif in vifs)
					{
						AddVIFRow(new VIF
						          	{
						          		Connection = m_selectedConnection,
						          		device = vif.device,
						          		network = vif.network,
						          		MAC = Messages.MAC_AUTOGENERATE
						          	});
					}
				}
				else
				{
				    //We need to recreate off vm
					var vifs = m_selectedConnection.ResolveAll(m_vm.VIFs);
				    foreach (VIF vif in vifs)
				        AddVIFRow(vif);
				}

                m_networkGridView.Sort(NameNetworkColumn, ListSortDirection.Ascending);
			}
			finally
			{
				m_networkGridView.ResumeLayout();
                m_buttonDeleteNetwork.Enabled = m_networkGridView.SelectedRows.Count > 0;
			}
		}

		private XenAPI.Network GetDefaultNetwork()
		{
			foreach (XenAPI.Network network in m_selectedConnection.Cache.Networks)
				if (ShowNetwork(network))
					return network;

			return null;
		}

		/// <summary>
		/// Function tells you when you can / cannot show the network based on the following rules
		/// 1) Don't show the guest installer network or networks with HideFromXenCenter==true.
		/// 2) If you selected an affinity, only show networks that host can see
		/// 3) If you haven't selected an affinity, only show networks all hosts can see
		/// </summary>
		private bool ShowNetwork(XenAPI.Network network)
		{
			if (!network.Show(Properties.Settings.Default.ShowHiddenVMs))
				return false;

			if (network.IsSlave())
				return false;

			if (m_selectedAffinity != null && !m_selectedAffinity.CanSeeNetwork(network))
				return false;

			if (m_selectedAffinity == null && !network.AllHostsCanSeeNetwork())
				return false;

            if (network.IsSriov() && !m_vm.HasSriovRecommendation())
                return false;

            return true;
		}

		private void AddVIFRow(VIF vif)
		{
            var row = new VifRow(vif);
            XenAPI.Network network = m_selectedConnection.Resolve(vif.network);
            bool isGuestInstallerNetwork = network != null && network.IsGuestInstallerNetwork();

            ToStringWrapper<XenAPI.Network> comboBoxEntry = FindComboBoxEntryForNetwork(network);
            // CA-66962: Don't choose disallowed networks: choose a better one instead.
            // CA-79930/CA-73056: Except for the guest installer network, which we let
            // through for now, but hide it below.
            if (comboBoxEntry == null && !isGuestInstallerNetwork)
            {
                network = GetDefaultNetwork();
                comboBoxEntry = FindComboBoxEntryForNetwork(network);
                vif.network = new XenRef<XenAPI.Network>(network.opaque_ref);
            }
            
            row.CreateCells(m_networkGridView,
			                String.Format(Messages.NETWORKPICKER_INTERFACE, vif.device),
			                vif.MAC,
			                comboBoxEntry);
			row.Cells[0].ReadOnly = true;

            // CA-73056: A row for the guest installer network shouldn't show up. But we still need
            // it present but invisible, otherwise the corresponding VIF doesn't get created at all.
            // CA-218956 - Expose HIMN when showing hidden objects
            if (isGuestInstallerNetwork && !XenAdmin.Properties.Settings.Default.ShowHiddenVMs)
                row.Visible = false;

			m_networkGridView.Rows.Add(row);
		}

        private ToStringWrapper<XenAPI.Network> FindComboBoxEntryForNetwork(XenAPI.Network network)
		{
			if (network == null)
				return null;

            foreach (ToStringWrapper<XenAPI.Network> entry in NetworkNetworkColumn.Items)
			{
				if (entry.item.uuid == network.uuid)
					return entry;
			}
			return null;
		}

        private bool AllMacAddressesValid()
        {
            foreach (var row in m_networkGridView.Rows)
            {
                var vifRow = row as VifRow;
                if (vifRow != null && !vifRow.HasValidMac)
                    return false;
            }

            return true;
        }

		#endregion

		#region Event handlers

		private void m_buttonDeleteNetwork_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in m_networkGridView.SelectedRows)
                m_networkGridView.Rows.Remove(row);

            UpdateControlsEnabledState(AllMacAddressesValid());
        }

		private void m_buttonAddNetwork_Click(object sender, EventArgs e)
		{
			if (m_networkGridView.Rows.Count >= m_vm.MaxVIFsAllowed())
			{
				using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(
				                      	SystemIcons.Error,
                                        FriendlyErrorNames.VIFS_MAX_ALLOWED, FriendlyErrorNames.VIFS_MAX_ALLOWED_TITLE)))
                {
                    dlg.ShowDialog(Program.MainWindow);
                }
				return;
			}

			VIF vif = new VIF {Connection = m_selectedConnection};

			int i = 0;
			while (true)
			{
				bool exists = false;
				foreach (DataGridViewRow row in m_networkGridView.Rows)
				{
				    VifRow vifRow = row as VifRow;
                    if (vifRow == null)
                        continue;

                    VIF v = vifRow.Vif;
                    if (v == null)
                        continue;

					if (int.Parse(v.device) == i)
						exists = true;
				}

				if (exists)
					i++;
				else
					break;
			}

			vif.device = i.ToString();
			vif.MAC = Messages.MAC_AUTOGENERATE;

			if (GetDefaultNetwork() != null)
				vif.network = new XenRef<XenAPI.Network>(GetDefaultNetwork().opaque_ref);

			AddVIFRow(vif);
		}

		private void m_networkGridView_SelectionChanged(object sender, EventArgs e)
		{
			m_buttonDeleteNetwork.Enabled = m_networkGridView.SelectedRows.Count > 0;
		}

		private void m_networkGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            VifRow row = m_networkGridView.Rows[e.RowIndex] as VifRow;
            if (row == null)
                return;

            VIF vif = row.Vif;
            if (vif == null)
                return;

            if (e.ColumnIndex == MACAddressNetworkColumn.Index)
            {
                row.HasValidMac = true;
                string mac = (string)row.Cells[e.ColumnIndex].Value;

                if (mac == null || mac.Trim().Length == 0)
                {
                    // We take it that zero-length mac means the user wants to auto-generate
                    row.Cells[e.ColumnIndex].Value = Messages.MAC_AUTOGENERATE;
                }
                else if (mac.Trim() == Messages.MAC_AUTOGENERATE)
                {
                    row.Cells[e.ColumnIndex].Value = Messages.MAC_AUTOGENERATE;
                    vif.MAC = Messages.MAC_AUTOGENERATE;
                }
                else if (Helpers.IsValidMAC(mac))
                {
                    vif.MAC = mac;
                }
                else
                {
                    row.HasValidMac = false;
                }

                UpdateControlsEnabledState(AllMacAddressesValid());
            }
            else if (e.ColumnIndex == NetworkNetworkColumn.Index)
            {
                object enteredValue = row.Cells[e.ColumnIndex].Value;
                var entry = enteredValue as ToStringWrapper<XenAPI.Network>;
            	XenAPI.Network network = (entry == null) ? (XenAPI.Network)enteredValue : entry.item;

            	if (network != null)
                    vif.network = new XenRef<XenAPI.Network>(network.opaque_ref);
            }
        }

        private void m_networkGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= m_networkGridView.RowCount)
                return;

            if (e.ColumnIndex == MACAddressNetworkColumn.Index)
            {
                m_networkGridView.BeginEdit(false);
                return;
            }

            if (e.ColumnIndex == NetworkNetworkColumn.Index)
            {
                m_networkGridView.BeginEdit(false);
                var combobox = m_networkGridView.EditingControl as ComboBox;

                if (combobox != null)
                    combobox.DroppedDown = true;
            }
        }

		private void m_networkGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			m_networkGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
			IsDirty = true;
		}

		private void m_networkGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			e.ThrowException = false;
            log.Error("Error from VIF table:", e.Exception);

        }

        private void m_networkGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= m_networkGridView.RowCount)
                return;

            if (e.ColumnIndex == MACAddressNetworkColumn.Index)
            {
                bool enable = true;
                foreach (DataGridViewRow row in m_networkGridView.Rows)
                {
                    if (row.Index == e.RowIndex)
                        continue;

                    var vifRow = row as VifRow;
                    if (vifRow != null && !vifRow.HasValidMac)
                    {
                        enable = false;
                        break;
                    }
                }

                UpdateControlsEnabledState(enable);
            }
        }

		#endregion

        #region Nested classes

        private class VifRow : DataGridViewRow
        {
            public readonly VIF Vif;

            public bool HasValidMac = true;

            public VifRow(VIF vif)
            {
                Vif = vif;
            }
        }

        #endregion
	}
}
