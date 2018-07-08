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
using System.Drawing;
using System.Windows.Forms;

using XenAPI;
using XenAdmin.Network;
using XenAdmin.Dialogs;
using XenAdmin.Core;
using System.Linq;

namespace XenAdmin.Controls
{
    public partial class BondDetails : UserControl
    {
        private int bondSizeLimit = 2;

        private int m_numberOfCheckedNics;

        /// <summary>
        /// Style to use for checkable rows
        /// </summary>
        private DataGridViewCellStyle regStyle;
        /// <summary>
        /// Style to use for non-checkable rows (should appear grayed out)
        /// </summary>
        private DataGridViewCellStyle dimmedStyle;

        public event EventHandler ValidChanged;

        internal IXenConnection Connection;

        private bool valid = false;
        internal bool Valid
        {
            get
            {
                return valid;
            }
            set
            {
                if (valid != value)
                {
                    valid = value;
                    if (ValidChanged != null)
                        ValidChanged(this, null);
                }
            }
        }

        internal string BondName
        {
            get
            {
                return string.Format(Messages.PIF_BOND, PIF.BondSuffix(BondedPIFs));
            }
        }

        internal List<PIF> BondedPIFs
        {
            get
            {
                List<PIF> result = new List<PIF>();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (IsRowChecked(row))
                        result.Add((PIF)row.Tag);
                }

                return result;
            }
        }

        internal List<PIF> AllPIFs
        {
            get
            {
                return dataGridView1.Rows.OfType<DataGridViewRow>().Select(r => r.Tag as PIF).Where(t => t != null).ToList();
            }
        }

        internal bond_mode BondMode
        {
            get
            {
                return radioButtonActiveBackup.Checked
                           ? bond_mode.active_backup
                           : radioButtonBalanceSlb.Checked
                                 ? bond_mode.balance_slb
                                 : bond_mode.lacp;
            }
        }

        internal Bond.hashing_algoritm HashingAlgorithm
        {
            get
            {
                return radioButtonLacpSrcMac.Checked
                    ? Bond.hashing_algoritm.src_mac
                    : radioButtonLacpTcpudpPorts.Checked ? Bond.hashing_algoritm.tcpudp_ports : Bond.hashing_algoritm.unknown;
            }
        }

        internal long MTU
        {
            get { return (long)numericUpDownMTU.Value; }
        }

        internal bool AutoPlug
        {
            get { return cbxAutomatic.Checked; }
        }

        public BondDetails()
        {
            InitializeComponent();

            //setup cell styles
            regStyle = dataGridView1.DefaultCellStyle.Clone();
            dimmedStyle = dataGridView1.DefaultCellStyle.Clone();
            dimmedStyle.ForeColor = SystemColors.GrayText;
        }

        internal void SetHost(Host host)
        {
            Connection = host.Connection;
            bondSizeLimit = Helpers.BondSizeLimit(Connection);
            PopulateDataGridView(NetworkingHelper.GetAllPhysicalPIFs(host));
            SetDefaultBoundariesOfMtuDropDown();
            ShowHideControls();
        }

        internal void SetPool(Pool pool)
        {
            Connection = pool.Connection;
            bondSizeLimit = Helpers.BondSizeLimit(Connection);
            PopulateDataGridView(NetworkingHelper.GetAllPhysicalPIFs(pool));
            SetDefaultBoundariesOfMtuDropDown();
            ShowHideControls();
        }

        private void PopulateDataGridView(List<PIF> pifs)
        {
            try
            {
                dataGridView1.SuspendLayout();
                dataGridView1.Rows.Clear();
                m_numberOfCheckedNics = 0;

				foreach (PIF pif in pifs)
				{
					string description = PIFDescription(pif);
					XenAPI.Network network = Connection.Resolve<XenAPI.Network>(pif.network);
					PIF_metrics metrics = pif.PIFMetrics();

					int rowIndex = dataGridView1.Rows.Add(new object[]
					                                      	{
					                                      		false,
					                                      		String.Format("{0} {1}", pif.Name(), description),
					                                      		pif.MAC,
					                                      		(network.PIFs.Count > 1) ? network.LinkStatusString() : pif.LinkStatusString(),
					                                      		pif.Carrier() ? pif.Speed() : Messages.HYPHEN,
					                                      		pif.Carrier() ? pif.Duplex() : Messages.HYPHEN,
					                                      		metrics == null ? "" : metrics.vendor_name,
					                                      		metrics == null ? "" : metrics.device_name,
					                                      		metrics == null ? "" : metrics.pci_bus_path
					                                      	});

					DataGridViewRow row = dataGridView1.Rows[rowIndex];
					row.Tag = pif;
					ToggleNICRowCheckable(row);
				}

            	//CA-47050: the ColumnPci should be autosized to Fill, but should not become smaller than a minimum
                //width, which is chosen to be the column's contents (including header) width. To find what this is
                //set temporarily the column's autosize mode to AllCells.
                ColumnPci.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                int storedWidth = ColumnPci.Width;
                ColumnPci.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                ColumnPci.MinimumWidth = storedWidth;
            }
            finally
            {
                dataGridView1.ResumeLayout();
            }

            SetValid();
        }

        private void ShowHideControls()
        {
            radioButtonLacpSrcMac.Visible = radioButtonLacpTcpudpPorts.Visible = Helpers.SupportsLinkAggregationBond(Connection);
        }

        private string PIFDescription(PIF pif)
        {
            Bond bond = pif.BondSlaveOf();
            return bond == null ? "" : string.Format(Messages.ALREADY_IN_BOND, bond.Name());
        }

        internal DialogResult ShowCreationWarning()
        {
            List<PIF> pifs = BondedPIFs;

            bool will_disturb_primary = NetworkingHelper.ContainsPrimaryManagement(pifs);
            bool will_disturb_secondary = NetworkingHelper.ContainsSecondaryManagement(pifs);
            bool will_disturb_clustering = NetworkingHelper.ContainsClusteringPif(pifs);

            // It is not allowed to bond primary and secondary interfaces together.
            if (will_disturb_primary && will_disturb_secondary)
            {
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(
                        SystemIcons.Error,
                        Messages.BOND_CREATE_WILL_DISTURB_BOTH,
                        Messages.BOND_CREATE)))
                {
                    dlg.ShowDialog(this);
                }

                return DialogResult.Cancel;
            }

            // Only primary management interface.
            // In this case, clustering interface warning is hidden if it happens to be the management interface.
            if (will_disturb_primary)
            {
                Pool pool = Helpers.GetPool(Connection);
                if (pool != null && pool.ha_enabled)
                {
                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(
                            SystemIcons.Error,
                            string.Format(Messages.BOND_CREATE_HA_ENABLED, pool.Name()),
                            Messages.BOND_CREATE)))
                    {
                        dlg.ShowDialog(this);
                    }
                    
                    return DialogResult.Cancel;
                }

                DialogResult dialogResult;
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.BOND_CREATE_WILL_DISTURB_PRIMARY, Messages.BOND_CREATE),
                    "BondConfigError",
                    new ThreeButtonDialog.TBDButton(Messages.BOND_CREATE_CONTINUE, DialogResult.OK),
                    ThreeButtonDialog.ButtonCancel))
                {
                    dialogResult = dlg.ShowDialog(this);
                }
                return dialogResult;
            }
            
            // Only secondary interface.
            // If there is clustering interface, shows clustering warning. Otherwise, shows secondary interface warning.
            if (will_disturb_secondary)
            {
                DialogResult dialogResult;
                if (will_disturb_clustering)
                {
                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(
                            SystemIcons.Warning,
                            Messages.BOND_CREATE_WILL_DISTURB_CLUSTERING,
                            Messages.BOND_CREATE),
                        ThreeButtonDialog.ButtonOK,
                        ThreeButtonDialog.ButtonCancel))
                    {
                        dialogResult = dlg.ShowDialog(this);
                    }
                }

                else
                {
                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(
                            SystemIcons.Warning,
                            Messages.BOND_CREATE_WILL_DISTURB_SECONDARY,
                            Messages.BOND_CREATE),
                        ThreeButtonDialog.ButtonOK,
                        ThreeButtonDialog.ButtonCancel))
                    {
                        dialogResult = dlg.ShowDialog(this);
                    }
                }
                
                return dialogResult;
            }
            
            return DialogResult.OK;
        }

        private void UpdateCellsReadOnlyState()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
                ToggleNICRowCheckable(row);
        }

        private void ToggleNICRowCheckable(DataGridViewRow row)
        {
            PIF pif = row.Tag as PIF;

            if (pif != null)
            {
                string description = PIFDescription(pif);
                bool checkable = String.IsNullOrEmpty(description) && m_numberOfCheckedNics < bondSizeLimit;

                //if it's already checked do not consider it
                if (IsRowChecked(row))
                    return;

                row.Cells[0].ReadOnly = !checkable;
                row.DefaultCellStyle = checkable ? regStyle : dimmedStyle;
            }
        }

        private bool IsRowChecked(DataGridViewRow row)
        {
            return (bool)(row.Cells[0] as DataGridViewCheckBoxCell).Value;
        }

        private bool IsRowChecked(int rowIndex)
        {
            return IsRowChecked(dataGridView1.Rows[rowIndex]);
        }

        private void SetValid()
        {
            Valid = (2 <= m_numberOfCheckedNics && m_numberOfCheckedNics <= bondSizeLimit);
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0)
                return;

            DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (IsRowChecked(e.RowIndex))
                m_numberOfCheckedNics++;
            else
                m_numberOfCheckedNics--;

            UpdateCellsReadOnlyState();
            UpdateMtuControls();

            SetValid();
        }

        private void UpdateMtuControls()
        {
            if (BondedPIFs.Count > 0)
                numericUpDownMTU.Maximum = Math.Min(BondedPIFs.Select(p => p.MTU).DefaultIfEmpty(XenAPI.Network.MTU_MAX).Min(), XenAPI.Network.MTU_MAX);
            else
                SetDefaultBoundariesOfMtuDropDown();

            ShowOrHideMtuInfo();
        }

        private void SetDefaultBoundariesOfMtuDropDown()
        {
            numericUpDownMTU.Maximum = XenAPI.Network.MTU_MAX;// Math.Min(AllPIFs.Select(p => p.MTU).DefaultIfEmpty(XenAPI.Network.MTU_MAX).Max(), XenAPI.Network.MTU_MAX);
            numericUpDownMTU.Minimum = XenAPI.Network.MTU_MIN;

            numericUpDownMTU.Value = XenAPI.Network.MTU_DEFAULT;

            ShowOrHideMtuInfo();
        }

        private void ShowOrHideMtuInfo()
        {
            numericUpDownMTU.Enabled = numericUpDownMTU.Minimum != numericUpDownMTU.Maximum;

            infoMtuMessage.Text = numericUpDownMTU.Minimum == numericUpDownMTU.Maximum
                                    ? string.Format(Messages.ALLOWED_MTU_VALUE, numericUpDownMTU.Minimum)
                                    : string.Format(Messages.ALLOWED_MTU_RANGE, numericUpDownMTU.Minimum, numericUpDownMTU.Maximum);
        }

        private void BondMode_CheckedChanged(object sender, EventArgs e)
        {
            ShowHideLacpWarning();
        }

        private void ShowHideLacpWarning()
        {
            panelLACPWarning.Visible = radioButtonLacpSrcMac.Checked || radioButtonLacpTcpudpPorts.Checked;
        }
    }
}
