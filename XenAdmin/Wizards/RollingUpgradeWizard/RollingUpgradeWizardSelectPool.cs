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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Properties;
using System.Linq;
using XenAPI;
using XenAdmin.Dialogs;


namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    public partial class RollingUpgradeWizardSelectPool : XenTabPage
    {
        public RollingUpgradeWizardSelectPool()
        {
            InitializeComponent();
            this.dataGridView1.CheckBoxClicked += DataGridRowClicked;
            this.Dock = DockStyle.Fill;
        }

        public override string Text
        {
            get
            {
                return Messages.SELECT_POOL;
            }
        }

        public override string PageTitle
        {
            get
            {
                return Messages.SELECT_POOL;
            }
        }

        public override string HelpID
        {
            get { return "Selectpool"; }
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Forward)
            {
                if (!AllSelectedHostsConnected())
                {
                    dataGridView1.Invalidate();
                    cancel = true;
                    return;
                }

                foreach (var selectedMaster in SelectedMasters)
                {
                    if (!(selectedMaster.Connection.Session.IsLocalSuperuser || selectedMaster.Connection.Session.Roles.Any(role => role.name_label == Role.MR_ROLE_POOL_ADMIN)))
                    {
                        using (var dlg = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(SystemIcons.Warning, string.Format(Messages.RBAC_UPGRADE_WIZARD_MESSAGE, selectedMaster.Connection.Username,
                                selectedMaster.Name()), Messages.ROLLING_POOL_UPGRADE)))
                        {
                            dlg.ShowDialog(this);
                        }

                        cancel = true;
                        return;
                    }
                }
            }
        }

        private bool AllSelectedHostsConnected()
        {
            var disconnectedServerNames = new List<string>();

            foreach (UpgradeDataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Checked == CheckState.Checked && ((row.Tag is Host && !row.HasPool) || row.Tag is Pool))
                {
                    IXenConnection connection = ((IXenObject)row.Tag).Connection;

                    if (connection == null || !connection.IsConnected)
                        disconnectedServerNames.Add(((IXenObject)row.Tag).Name());
                }
            }

            if (disconnectedServerNames.Count > 0)
            {
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning,
                        string.Format(Messages.ROLLING_UPGRADE_DISCONNECTED_SERVER, Helpers.StringifyList(disconnectedServerNames)),
                        Messages.ROLLING_POOL_UPGRADE)))
                {
                    dlg.ShowDialog(this);
                }
                return false;
            }
            return true;
        }

        public IList<Host> SelectedMasters
        {
            get
            {
                List<Host> hosts = new List<Host>();
                foreach (UpgradeDataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Tag is Host)
                    {
                        if (!row.HasPool && ((int)row.Cells[1].Value) == 1)
                            hosts.Add((Host)row.Tag);
                    }
                    else if (row.Tag is Pool && ((int)row.Cells[1].Value) == 1)
                    {
                        Pool pool = ((Pool)(row.Tag));
                        Host host = pool.Connection.Resolve(pool.master);
                        hosts.Add(host);
                    }
                }
                return hosts;
            }
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            BuildServerList();
        }

        public override void SelectDefaultControl()
        {
            dataGridView1.Select();
        }

        private void BuildServerList()
        {
            IList<Host> masters = SelectedMasters;
            dataGridView1.Rows.Clear();
            List<IXenConnection> xenConnections = ConnectionsManager.XenConnectionsCopy;
            xenConnections.Sort();

            foreach (IXenConnection xenConnection in xenConnections)
            {
                Pool pool = Helpers.GetPool(xenConnection);
                Pool poolOfOne = Helpers.GetPoolOfOne(xenConnection);

                bool restricted = Helpers.FeatureForbidden(xenConnection, Host.RestrictRpu);

                bool hasPool = true;
                if (pool != null)
                {
                    int index = dataGridView1.Rows.Add(new UpgradeDataGridViewRow(pool));
                    if ((restricted && !pool.RollingUpgrade()) || pool.IsUpgradeForbidden())
                    {
                        ((DataGridViewExRow)dataGridView1.Rows[index]).Enabled = false;
                        if (restricted)
                            dataGridView1.Rows[index].Cells[NameColumn.Name].ToolTipText = Messages.ROLLING_UPGRADE_UNLICENSED_POOL;
                    }
                    else if (masters.Contains(pool.Connection.Resolve(pool.master)))
                        dataGridView1.CheckBoxChange(index, 1);
                }
                else
                {
                    hasPool = false;
                }

                Host[] hosts = xenConnection.Cache.Hosts;
                Array.Sort(hosts);
                foreach (Host host in hosts)
                {
                    int index = dataGridView1.Rows.Add(new UpgradeDataGridViewRow(host, hasPool));
                    if (restricted || (poolOfOne != null && poolOfOne.IsUpgradeForbidden()))
                    {
                        ((DataGridViewExRow)dataGridView1.Rows[index]).Enabled = false;
                        if (restricted)
                            dataGridView1.Rows[index].Cells[NameColumn.Name].ToolTipText = Messages.ROLLING_UPGRADE_UNLICENSED_HOST;
                    }
                    else if (!hasPool && masters.Contains(host))
                        dataGridView1.CheckBoxChange(index, 1);
                }
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                dataGridView1.ExpandCollapseClicked(row.Index);
            }
        }

        public override bool EnableNext()
        {
            bool clearAllButtonEnabled = false;
            bool selectAllButtonEnabled = false;

            foreach (UpgradeDataGridViewRow row in dataGridView1.Rows)
            {
                if ((row.Tag is Host && !row.HasPool) || row.Tag is Pool)
                {
                    int val = (int)row.Cells[1].Value;

                    if (val > 0)
                        clearAllButtonEnabled = true;
                    else
                        selectAllButtonEnabled = true;
                }
            }

            buttonClearAll.Enabled = clearAllButtonEnabled;
            buttonSelectAll.Enabled = selectAllButtonEnabled;
            return clearAllButtonEnabled;
        }

        private class UpgradeDataGridView : PoolHostDataGridViewOneCheckbox
        {
            public UpgradeDataGridView(){}
            public UpgradeDataGridView(IContainer container) : base(container){}
            
            protected override void SortColumns()
            {
                UpgradeDataGridViewRow firstRow = Rows[0] as UpgradeDataGridViewRow;
                if (firstRow == null) return;

                if (columnToBeSortedIndex == firstRow.NameCellIndex ||
                    columnToBeSortedIndex == firstRow.DescriptionCellIndex ||
                    columnToBeSortedIndex == firstRow.VersionCellIndex)
                    SortAndRebuildTree(new CollapsingPoolHostRowSorter<UpgradeDataGridViewRow>(direction, columnToBeSortedIndex));
            }

            protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
            {
                base.OnCellPainting(e);

                if (e.RowIndex >= 0)
                {
                    var row = (PoolHostDataGridViewOneCheckboxRow)Rows[e.RowIndex];
                    if (row != null && !row.Enabled && e.ColumnIndex == row.PoolCheckBoxCellIndex)
                    {
                        e.PaintBackground(e.ClipBounds, true);
                        e.Handled = true;
                    }
                }
            }
        }

        public class UpgradeDataGridViewRow : PoolHostDataGridViewOneCheckboxRow
        {
            private DataGridViewTextBoxCell _description;
            private DataGridViewTextBoxCell _version;

            public UpgradeDataGridViewRow(Pool pool) 
                : base(pool)
            {
            }

            public UpgradeDataGridViewRow(Host host, bool hasPool)
                : base(host, hasPool)
            {
            }

            public int DescriptionCellIndex
            {
                get { return Cells.IndexOf(_description); }
            }

            public int VersionCellIndex
            {
                get { return Cells.IndexOf(_version); }
            }

            public string DescriptionText
            {
                get { return (string) Cells[DescriptionCellIndex].Value; }
            }

            public string VersionText
            {
                get { return (string)Cells[VersionCellIndex].Value; }
            }

            protected override void SetupAdditionalDetailsColumns()
            {
                _description = new DataGridViewTextBoxCell();
                _version = new DataGridViewTextBoxCell();
                Cells.Add(_description);
                Cells.Add(_version);
            }

            protected override void UpdateAdditionalDetailsForPool(Pool pool, Host master)
            {
                _description.Value = pool.Description();
                _version.Value = pool.SmallerVersionHost().ProductVersionTextShort();
            }

            protected override void UpdateAdditionalDetailsForHost( Host host )
            {
                _description.Value = host.Description();
                _version.Value = host.ProductVersionTextShort();
            }
        }

        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            foreach (UpgradeDataGridViewRow row in dataGridView1.Rows)
            {
                var checkrow = row.Cells[1] as DataGridViewCheckBoxCell;
                if (row.Enabled)
                    checkrow.Value = 1;
            }
            OnPageUpdated();
        }

        private void buttonClearAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var checkrow = row.Cells[1] as DataGridViewCheckBoxCell;
                checkrow.Value = 0;
            }
            OnPageUpdated();
        }

        private void DataGridRowClicked(object sender, EventArgs e)
        {
            OnPageUpdated();
        }
    }
}
