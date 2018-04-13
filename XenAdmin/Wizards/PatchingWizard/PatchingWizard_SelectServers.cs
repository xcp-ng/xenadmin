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
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Properties;
using XenAPI;
using XenAdmin.Alerts;
using System.Linq;
using System.Diagnostics;

namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_SelectServers : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int PLUS_MINUS_COL = 0;
        private const int POOL_CHECKBOX_COL = 1;
        private const int POOL_ICON_HOST_CHECKBOX_COL = 2;

        private const int UNCHECKED = 0;
        private const int CHECKED = 1;
        private const int INDETERMINATE = 2;

        private bool poolSelectionOnly;

        public XenServerPatchAlert SelectedUpdateAlert { private get; set; }
        public XenServerPatchAlert FileFromDiskAlert { private get; set; }
        public WizardMode WizardMode { private get; set; }

        public PatchingWizard_SelectServers()
        {
            InitializeComponent();
            dataGridViewHosts.CheckBoxClicked += dataGridViewHosts_CheckBoxClicked;
        }

        public override string Text
        {
            get
            {
                return Messages.PATCHINGWIZARD_SELECTSERVERPAGE_TEXT;
            }
        }

        public override string PageTitle
        {
            get
            {
                return Messages.PATCHINGWIZARD_SELECTSERVERPAGE_TITLE;
            }
        }

        public override string HelpID
        {
            get { return "SelectServers"; }
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            try
            {
                poolSelectionOnly = WizardMode == WizardMode.AutomatedUpdates || SelectedUpdateAlert != null || FileFromDiskAlert != null;

                switch (WizardMode)
                {
                    case WizardMode.AutomatedUpdates :
                        label1.Text = Messages.PATCHINGWIZARD_SELECTSERVERPAGE_RUBRIC_AUTOMATED_MODE;
                        break;
                    case WizardMode.NewVersion :
                        label1.Text = Messages.PATCHINGWIZARD_SELECTSERVERPAGE_RUBRIC_NEW_VERSION_MODE;
                        break;
                    case WizardMode.SingleUpdate :
                        label1.Text = poolSelectionOnly ? Messages.PATCHINGWIZARD_SELECTSERVERPAGE_RUBRIC_POOL_SELECTION : Messages.PATCHINGWIZARD_SELECTSERVERPAGE_RUBRIC_DEFAULT;
                        break;
                }
                
                // catch selected servers, in order to restore selection after the dataGrid is reloaded
                List<Host> selectedServers = SelectedServers;

                dataGridViewHosts.Rows.Clear();

                List<IXenConnection> xenConnections = ConnectionsManager.XenConnectionsCopy;
                xenConnections.Sort();
                int licensedPoolCount = 0;
                int poolCount = 0;
                foreach (IXenConnection xenConnection in xenConnections)
                {
                    // add pools, their members and standalone hosts
                    Pool pool = Helpers.GetPool(xenConnection);
                    bool hasPool = pool != null;
                    if (hasPool)
                    {
                        int index = dataGridViewHosts.Rows.Add(new PatchingHostsDataGridViewRow(pool));
                        Host master = pool.Connection.Resolve(pool.master);
                        EnabledRow(master, SelectedUpdateType, index);
                    }

                    Host[] hosts = xenConnection.Cache.Hosts;

                    if (hosts.Length > 0)
                    {
                        poolCount++;
                        var automatedUpdatesRestricted = hosts.Any(Host.RestrictBatchHotfixApply); //if any host is not licensed for automated updates
                        if (!automatedUpdatesRestricted)
                            licensedPoolCount++;
                    }

                    Array.Sort(hosts);
                    foreach (Host host in hosts)
                    {
                        int index = dataGridViewHosts.Rows.Add(new PatchingHostsDataGridViewRow(host, hasPool, !poolSelectionOnly));
                        EnabledRow(host, SelectedUpdateType, index);
                    }
                }

                if (WizardMode == WizardMode.NewVersion && licensedPoolCount > 0) // in NewVersion mode and at least one pool licensed for automated updates 
                {
                    applyUpdatesCheckBox.Visible = true;
                    applyUpdatesCheckBox.Text = poolCount == licensedPoolCount
                        ? Messages.PATCHINGWIZARD_SELECTSERVERPAGE_APPLY_UPDATES
                        : Messages.PATCHINGWIZARD_SELECTSERVERPAGE_APPLY_UPDATES_MIXED;
                }
                else  // not in NewVersion mode or all pools unlicensed
                {
                    applyUpdatesCheckBox.Visible = false;
                }

                // restore server selection
                SelectServers(selectedServers);
            }
            catch (Exception e)
            {
                log.Error(e, e);
                throw;//better throw an exception rather than closing the wizard suddenly and silently
            }
        }

        public override void SelectDefaultControl()
        {
            dataGridViewHosts.Select();
        }
        
        private void EnabledRow(Host host, UpdateType type, int index)
        {
            var row = (PatchingHostsDataGridViewRow)dataGridViewHosts.Rows[index];

            var poolOfOne = Helpers.GetPoolOfOne(host.Connection);

            if (WizardMode == WizardMode.AutomatedUpdates)
            {
                // This check is first because it generally can't be fixed, it's a property of the host
                if (poolOfOne != null && poolOfOne.IsAutoUpdateRestartsForbidden()) // Forbids update auto restarts
                {
                    row.Enabled = false;
                    row.Cells[3].ToolTipText = Messages.POOL_FORBIDS_AUTOMATED_UPDATES;
                    return;
                }

                var pool = Helpers.GetPool(host.Connection);
                if (pool != null && !pool.IsPoolFullyUpgraded()) //partially upgraded pool is not supported
                {
                    row.Enabled = false;
                    row.Cells[3].ToolTipText = Messages.PATCHINGWIZARD_SELECTSERVERPAGE_AUTOMATED_UPDATES_NOT_SUPPORTED_PARTIALLY_UPGRADED;

                    return;
                }

                //check updgrade sequences
                Updates.UpgradeSequence us = Updates.GetUpgradeSequence(host.Connection);

                if (us == null) //version not supported
                {
                    row.Enabled = false;
                    row.Cells[3].ToolTipText = Messages.PATCHINGWIZARD_SELECTSERVERPAGE_AUTOMATED_UPDATES_NOT_SUPPORTED_HOST_VERSION;

                    return;
                }

                //unlicensed servers are not enabled
                if (Host.RestrictBatchHotfixApply(host))
                {
                    row.Enabled = false;
                    row.Cells[3].ToolTipText = Messages.PATCHINGWIZARD_SELECTSERVERPAGE_HOST_UNLICENSED_FOR_AUTOMATED_UPDATES;

                    return;
                }

                //if there is a host missing from the upgrade sequence
                if (host.Connection.Cache.Hosts.Any(h => !us.Keys.Contains(h)))
                {
                    row.Enabled = false;
                    row.Cells[3].ToolTipText = Messages.PATCHINGWIZARD_SELECTSERVERPAGE_SERVER_NOT_AUTO_UPGRADABLE;

                    return;
                }

                //if all hosts are up-to-date
                if (us.AllHostsUpToDate)
                {
                    row.Enabled = false;
                    row.Cells[3].ToolTipText = Messages.PATCHINGWIZARD_SELECTSERVERPAGE_SERVER_UP_TO_DATE;

                    return;
                }

                //if host is unreachable
                if (!host.IsLive())
                {
                    row.Enabled = false;
                    row.Cells[3].ToolTipText = Messages.PATCHINGWIZARD_SELECTSERVERPAGE_HOST_UNREACHABLE;
                    return;
                }

                return;
            }

            List<Host> applicableHosts = null;
            if (SelectedUpdateAlert != null)
            {
                applicableHosts = SelectedUpdateAlert.DistinctHosts;
            }
            else if (FileFromDiskAlert != null)
            {
                applicableHosts = FileFromDiskAlert.DistinctHosts;
            }

            if (!host.CanApplyHotfixes() && (Helpers.ElyOrGreater(host) || type != UpdateType.ISO))
            {
                row.Enabled = false;
                row.Cells[3].ToolTipText = Messages.PATCHINGWIZARD_SELECTSERVERPAGE_HOST_UNLICENSED;
                return;
            }

            if (!host.IsLive())
            {
                row.Enabled = false;
                row.Cells[3].ToolTipText = Messages.PATCHINGWIZARD_SELECTSERVERPAGE_HOST_UNREACHABLE;
                return;
            }

            switch (type)
            {
                case UpdateType.NewRetail:
                case UpdateType.Existing:
                    if (Helpers.ElyOrGreater(host))
                    {
                        row.Enabled = false;
                        row.Cells[3].ToolTipText = Messages.PATCHINGWIZARD_SELECTSERVERPAGE_PATCH_NOT_APPLICABLE;
                    }
                    disableNotApplicableHosts(row, applicableHosts, host);
                    break;
                case UpdateType.ISO:
                    if (!Helpers.CreamOrGreater(host.Connection) && !Helpers.ElyOrGreater(host)) //from Ely, iso does not mean supplemental pack
                    {
                        row.Enabled = false;
                        row.Cells[3].ToolTipText = Messages.PATCHINGWIZARD_SELECTSERVERPAGE_CANNOT_INSTALL_SUPP_PACKS;
                    }
                    else
                    {
                        if (applicableHosts != null)
                        {
                            disableNotApplicableHosts(row, applicableHosts, host);
                        }
                        else
                        {
                            var firstCheckedHost = GetFirstCheckedHost();
                            if (firstCheckedHost != null && (Helpers.ElyOrGreater(firstCheckedHost) != Helpers.ElyOrGreater(host)))
                            {
                                row.Enabled = false;
                                row.Cells[3].ToolTipText = string.Format(Messages.PATCHINGWIZARD_SELECTSERVERPAGE_MIXED_VERSIONS, firstCheckedHost.ProductVersionTextShort(), host.ProductVersionTextShort());
                            }
                            else if (!row.Enabled)
                            {
                                row.Enabled = true;
                                row.Cells[3].ToolTipText = null;
                            }
                        }
                    }
                    break;
            }
        }

        private Host GetFirstCheckedHost()
        {
            var firstCheckedRow = dataGridViewHosts.Rows.Cast<PatchingHostsDataGridViewRow>().FirstOrDefault(row => row.CheckValue > UNCHECKED);
            if (firstCheckedRow == null)
                return null;
            return firstCheckedRow.Tag as Host ?? Helpers.GetMaster(firstCheckedRow.Tag as Pool);
        }

        private void disableNotApplicableHosts(PatchingHostsDataGridViewRow row, List<Host> applicableHosts, Host host)
        {
            if (applicableHosts == null)
                return;

            if (poolSelectionOnly && row.IsPoolOrStandaloneHost && host.Connection.Cache.Hosts.Any(applicableHosts.Contains))
                return;

            if (!applicableHosts.Contains(host))
            {
                string patchUuidFromAlert = null;
                if (SelectedUpdateAlert != null && SelectedUpdateAlert.Patch != null)
                {
                    patchUuidFromAlert = SelectedUpdateAlert.Patch.Uuid;
                }
                else if (FileFromDiskAlert != null)
                {
                    patchUuidFromAlert = FileFromDiskAlert.Patch.Uuid;
                }

                if (!string.IsNullOrEmpty(patchUuidFromAlert))
                {
                    if (isPatchApplied(patchUuidFromAlert, host))
                    {
                        row.Cells[3].ToolTipText = Messages.PATCHINGWIZARD_SELECTSERVERPAGE_PATCH_ALREADY_APPLIED;
                    }
                    else
                    {
                        row.Cells[3].ToolTipText = Messages.PATCHINGWIZARD_SELECTSERVERPAGE_PATCH_NOT_APPLICABLE;
                    }
                    row.Enabled = false;
                }
            }
        }

        private bool isPatchApplied(string uuid, Host host) 
        {
            if (Helpers.ElyOrGreater(host))
            {
                return host.AppliedUpdates().Any(u => u != null && string.Equals(u.uuid, uuid, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                List<Pool_patch> hostPatches = host.AppliedPatches();
                foreach (Pool_patch patch in hostPatches)
                {
                    if (string.Equals(patch.uuid, uuid, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            try
            {
                if (direction == PageLoadedDirection.Forward)
                {
                    if (!AllSelectedHostsConnected())
                    {
                        foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
                            row.UpdateIcon();
                        dataGridViewHosts.Invalidate();
                        cancel = true;
                        return;
                    }

                    //Upload the patches to the masters if it is necessary
                    List<Host> masters = SelectedMasters;

                    //Do RBAC check
                    foreach (Host master in masters)
                    {
                        if (!(Role.CanPerform(new RbacMethodList("pool_patch.apply"), master.Connection)))
                        {
                            string nameLabel = master.Name();
                            Pool pool = Helpers.GetPoolOfOne(master.Connection);
                            if (pool != null)
                                nameLabel = pool.Name();

                            using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Warning, string.Format(Messages.RBAC_UPDATES_WIZARD, master.Connection.Username, nameLabel), Messages.UPDATES_WIZARD)))
                            {
                                dlg.ShowDialog(this);
                            }
                            
                            cancel = true;
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Debug(e);
                throw;//better throw an exception rather than closing the wizard suddenly and silently
            }
        }

        private bool AllSelectedHostsConnected()
        {
            var disconnectedServerNames = new List<string>();

            foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
            {
                if ((int)row.Cells[POOL_CHECKBOX_COL].Value > UNCHECKED && row.IsPoolOrStandaloneHost)
                {
                    IXenConnection connection = ((IXenObject) row.Tag).Connection;
                    if (connection == null || !connection.IsConnected)
                        disconnectedServerNames.Add(((IXenObject) row.Tag).Name());
                }
            }

            if (disconnectedServerNames.Count > 0)
            {
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning,
                        string.Format(Messages.UPDATES_WIZARD_DISCONNECTED_SERVER, Helpers.StringifyList(disconnectedServerNames)),
                        Messages.UPDATES_WIZARD)))
                {
                    dlg.ShowDialog(this);
                }
                return false;
            }
            return true;
        }

        public override bool EnableNext()
        {
            bool clearAllButtonEnabled = false;
            bool selectAllButtonEnabled = false;

            foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
            {
                if (row.IsPoolOrStandaloneHost && row.Enabled)
                {
                    int val = (int)row.Cells[POOL_CHECKBOX_COL].Value;
                    if (val > UNCHECKED)
                    {
                        clearAllButtonEnabled = true;
                        if (val == INDETERMINATE)
                            selectAllButtonEnabled = true;
                    }
                    else
                        selectAllButtonEnabled = true;
                }
            }

            buttonClearAll.Enabled = clearAllButtonEnabled;
            buttonSelectAll.Enabled = selectAllButtonEnabled;
            return clearAllButtonEnabled;
        }

        #region Accessors

        public Pool_patch Patch { private get; set; }

        public List<Host> SelectedMasters
        {
            get
            {
                List<Host> result = new List<Host>();
                foreach (Host selectedServer in SelectedServers)
                {
                    Host master = Helpers.GetMaster(selectedServer.Connection);
                    if (!result.Contains(master))
                        result.Add(master);
                }
                return result;
            }
        }

        public List<Host> SelectedServers
        {
            get
            {
                if (poolSelectionOnly)
                {
                    if (WizardMode != WizardMode.SingleUpdate)
                        //prechecks will fail in automated updates mode if one of the hosts is unreachable
                        return SelectedPools.SelectMany(p => p.Connection.Cache.Hosts).ToList();
                    //prechecks will issue warning but allow updates to be installed on the reachable hosts only
                    return SelectedPools.SelectMany(p => p.Connection.Cache.Hosts.Where(host => host.IsLive())).ToList();
                }
                else
                {
                    List<Host> hosts = new List<Host>();
                    foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
                    {
                        if (row.IsSelectableHost)
                        {
                            if ((row.HasPool && ((int)row.Cells[POOL_ICON_HOST_CHECKBOX_COL].Value) == CHECKED) || (!row.HasPool && ((int)row.Cells[POOL_CHECKBOX_COL].Value) == CHECKED))
                                hosts.Add((Host)row.Tag);
                        }
                    }
                    return hosts;
                }
            }
        }

        public List<Pool> SelectedPools
        {
            get
            {
                List<Pool> pools = new List<Pool>();
                foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
                {
                    if (row.Tag is Pool)
                    {
                        if (((int)row.Cells[POOL_CHECKBOX_COL].Value) != UNCHECKED && !pools.Contains((Pool)row.Tag))
                            pools.Add((Pool)row.Tag);
                    }
                    else if (row.Tag is Host)
                    {
                        if (((int)row.Cells[POOL_CHECKBOX_COL].Value) != UNCHECKED)
                        {
                            Host host = (Host)row.Tag;
                            Pool pool = Helpers.GetPoolOfOne(host.Connection);
                            if (pool != null && !pools.Contains(pool))
                                pools.Add(pool);
                        }
                    }
                }
                return pools;
            }
        }

        public bool ApplyUpdatesToNewVersion
        {
            get
            {
                return applyUpdatesCheckBox.Visible && applyUpdatesCheckBox.Checked;
            }
        }

        public UpdateType SelectedUpdateType { private get; set; }

        public void SelectServers(List<Host> selectedServers)
        {
            if (selectedServers.Count > 0)
            {
                foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
                {
                    if (row.IsSelectableHost)
                    {
                        var host = (Host) row.Tag;
                        if (selectedServers.Contains(host))
                            dataGridViewHosts.CheckBoxChange(row.Index,
                                                             Helpers.GetPool(host.Connection) != null
                                                                 ? POOL_ICON_HOST_CHECKBOX_COL
                                                                 : POOL_CHECKBOX_COL);
                    }
                    else if (poolSelectionOnly && row.IsSelectablePool)
                    {
                        // select the pools of the selected servers
                        var pool = (Pool) row.Tag;;
                        foreach (var host in pool.Connection.Cache.Hosts)
                        {
                            if (selectedServers.Contains(host))
                            {
                                dataGridViewHosts.CheckBoxChange(row.Index, POOL_CHECKBOX_COL);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void DisableUnselectedServers()
        {
            foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
            {
                if (row.Enabled && row.CheckValue == UNCHECKED)
                {
                    row.Enabled = false;
                }
            }
        }

        #endregion
       
        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            CheckAllCheckBoxes(CHECKED);
        }

        private void CheckAllCheckBoxes(int value)
        {
            foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
            {
                if (row.IsSelectableHost)
                {
                    if (row.HasPool && (int)row.Cells[POOL_ICON_HOST_CHECKBOX_COL].Value != value)
                        dataGridViewHosts.CheckBoxChange(row.Index, POOL_ICON_HOST_CHECKBOX_COL);
                    else if ((int)row.Cells[POOL_CHECKBOX_COL].Value != value)
                        dataGridViewHosts.CheckBoxChange(row.Index, POOL_CHECKBOX_COL);
                }
                else if (row.IsSelectablePool && (int)row.Cells[POOL_CHECKBOX_COL].Value != value)
                {
                    dataGridViewHosts.CheckBoxChange(row.Index, POOL_CHECKBOX_COL);
                }
            }
        }

        private void buttonClearAll_Click(object sender, EventArgs e)
        {
            CheckAllCheckBoxes(UNCHECKED);
        }

        private void dataGridViewHosts_CheckBoxClicked(object sender, EventArgs e)
        {
            foreach (PatchingHostsDataGridViewRow row in dataGridViewHosts.Rows)
            {
                var host = row.Tag as Host ?? Helpers.GetMaster(row.Tag as Pool);
                if (host != null)
                    EnabledRow(host, SelectedUpdateType, row.Index);
            }
            OnPageUpdated();
        }

        #region Nested items

        private class PatchingHostsDataGridView : CollapsingPoolHostDataGridView
        {
            protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
            {
                base.OnCellPainting(e);

                if (e.RowIndex >= 0 && Rows[e.RowIndex].Tag is Host)
                {
                    PatchingHostsDataGridViewRow row = (PatchingHostsDataGridViewRow)Rows[e.RowIndex];
                    if (row.HasPool && (e.ColumnIndex == POOL_CHECKBOX_COL || e.ColumnIndex == PLUS_MINUS_COL))
                    {
                        e.PaintBackground(e.ClipBounds, true);
                        e.Handled = true;
                    }
                    else if (!row.HasPool && e.ColumnIndex == PLUS_MINUS_COL)
                    {
                        e.PaintBackground(e.ClipBounds, true);
                        e.Handled = true;
                    }
                    else if (row.HasPool && !row.IsSelectableHost && e.ColumnIndex == POOL_ICON_HOST_CHECKBOX_COL)
                    {
                        e.PaintBackground(e.ClipBounds, true);
                        e.Handled = true;
                    }
                    else if (!row.HasPool && !row.Enabled && e.ColumnIndex == POOL_CHECKBOX_COL)
                    {
                        e.PaintBackground(e.ClipBounds, true);
                        e.Handled = true;
                    }
                }

                if (e.RowIndex >= 0 && Rows[e.RowIndex].Tag is Pool)
                {
                    PatchingHostsDataGridViewRow row = (PatchingHostsDataGridViewRow)Rows[e.RowIndex];
                    if (!row.Enabled && e.ColumnIndex == POOL_CHECKBOX_COL)
                    {
                        e.PaintBackground(e.ClipBounds, true);
                        e.Handled = true;
                    }
                }
            }

            public override void CheckBoxChange(int RowIndex, int ColumnIndex)
            {
                if (RowIndex >= 0 && !((PatchingHostsDataGridViewRow)Rows[RowIndex]).Enabled)
                    return;

                if (RowIndex >= 0 && Rows[RowIndex].Tag is Host)
                {
                    if (ColumnIndex == POOL_ICON_HOST_CHECKBOX_COL && Rows[RowIndex].Cells[ColumnIndex].Value is int) //Checkbox
                    {
                        int hostNewValue = ClickCheckBox(RowIndex, ColumnIndex);

                        PatchingHostsDataGridViewRow poolRow = FindPoolRow(RowIndex);
                        bool allHostSameValue = true;
                        bool atLeastOneHostChecked = false;
                        for (int i = poolRow.Index + 1; i < Rows.Count; i++)
                        {
                            if (Rows[i].Tag is Host &&
                                ((PatchingHostsDataGridViewRow)Rows[i]).HasPool)
                            {
                                if (((int)Rows[i].Cells[POOL_ICON_HOST_CHECKBOX_COL].Value) == CHECKED)
                                {
                                    atLeastOneHostChecked = true;
                                }
                                if (((PatchingHostsDataGridViewRow)Rows[i]).Enabled && ((int)Rows[i].Cells[POOL_ICON_HOST_CHECKBOX_COL].Value) != hostNewValue)
                                {
                                    allHostSameValue = false;
                                }
                            }
                            else
                                break;
                        }
                        if (allHostSameValue)
                        {
                            poolRow.Cells[POOL_CHECKBOX_COL].Value = hostNewValue;
                        }
                        else if (atLeastOneHostChecked)
                        {
                            poolRow.Cells[POOL_CHECKBOX_COL].Value = INDETERMINATE;
                        }
                    }
                    else if (ColumnIndex == POOL_CHECKBOX_COL)
                    {
                        ClickCheckBox(RowIndex, ColumnIndex);
                    }
                }
                else if (RowIndex >= 0 && Rows[RowIndex].Tag is Pool)
                {
                    if (ColumnIndex == POOL_CHECKBOX_COL)
                    {
                        ClickCheckBox(RowIndex, ColumnIndex);
                        for (int i = RowIndex + 1; i < Rows.Count; i++)
                        {
                            if (Rows[i].Tag is Host && ((PatchingHostsDataGridViewRow)Rows[i]).HasPool)
                            {
                                var value = (int)Rows[RowIndex].Cells[ColumnIndex].Value;

                                if (value == UNCHECKED || value == CHECKED)
                                    Rows[i].Cells[POOL_ICON_HOST_CHECKBOX_COL].Value = value;
                            }
                            else break;
                        }
                    }
                }
                OnCheckBoxClicked();
            }

            private PatchingHostsDataGridViewRow FindPoolRow(int rowIndex)
            {
                for (int i = rowIndex; i >= 0; i--)
                {
                    if (Rows[i].Tag is Pool)
                        return (PatchingHostsDataGridViewRow)Rows[i];
                }
                return null;
            }

            private int ClickCheckBox(int rowIndex, int columnIndex)
            {
                int value = (int)Rows[rowIndex].Cells[columnIndex].Value;
                if (value == CHECKED)
                {
                    Rows[rowIndex].Cells[columnIndex].Value = UNCHECKED;
                    return UNCHECKED;
                }
                else
                {
                    Rows[rowIndex].Cells[columnIndex].Value = CHECKED;
                    return CHECKED;
                }
            }

            protected override void SortColumns()
            {
                PatchingHostsDataGridViewRow firstRow = Rows[0] as PatchingHostsDataGridViewRow;
                if (firstRow == null)
                    return;

                if (columnToBeSortedIndex == firstRow.NameCellIndex ||
                    columnToBeSortedIndex == firstRow.VersionCellIndex)
                    SortAndRebuildTree(new CollapsingPoolHostRowSorter<PatchingHostsDataGridViewRow>(direction, columnToBeSortedIndex));
            }
        }

        private class PatchingHostsDataGridViewRow : CollapsingPoolHostDataGridViewRow
        {
            private class DataGridViewNameCell : DataGridViewTextBoxCell
            {
                protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
                {
                    Pool pool = value as Pool;

                    if (pool != null)
                        base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
                    else
                    {
                        Host host = value as Host;
                        if (host != null)
                        {
                            PatchingHostsDataGridViewRow row = (PatchingHostsDataGridViewRow)this.DataGridView.Rows[this.RowIndex];
                            if (row.HasPool)
                            {
                                Image hostIcon = Images.GetImage16For(host);
                                base.Paint(graphics, clipBounds,
                                           new Rectangle(cellBounds.X + 16, cellBounds.Y, cellBounds.Width - 16,
                                                         cellBounds.Height), rowIndex, cellState, value, formattedValue,
                                           errorText, cellStyle, advancedBorderStyle, paintParts);

                                if ((cellState & DataGridViewElementStates.Selected) != 0 && row.Enabled)
                                {
                                    using (var brush = new SolidBrush(DataGridView.DefaultCellStyle.SelectionBackColor))
                                        graphics.FillRectangle(
                                            brush, cellBounds.X,
                                            cellBounds.Y, hostIcon.Width, cellBounds.Height);
                                }
                                else
                                {
                                    using (var brush = new SolidBrush(DataGridView.DefaultCellStyle.BackColor))
                                        graphics.FillRectangle(brush,
                                                               cellBounds.X, cellBounds.Y, hostIcon.Width, cellBounds.Height);
                                }

                                if (row.Enabled)
                                    graphics.DrawImage(hostIcon, cellBounds.X, cellBounds.Y + 3, hostIcon.Width,
                                                       hostIcon.Height);
                                else
                                    graphics.DrawImage(hostIcon,
                                                       new Rectangle(cellBounds.X, cellBounds.Y + 3,
                                                                     hostIcon.Width, hostIcon.Height),
                                                       0, 0, hostIcon.Width, hostIcon.Height, GraphicsUnit.Pixel,
                                                       Drawing.GreyScaleAttributes);
                            }
                            else
                            {
                                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue,
                                           errorText, cellStyle, advancedBorderStyle, paintParts);
                            }
                        }
                    }
                }
            }

            private class DataGridViewIconCell : DataGridViewImageCell
            {
                protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
                {
                    Image icon = value as Image;
                    if (icon == null)
                        return;

                    PatchingHostsDataGridViewRow row = (PatchingHostsDataGridViewRow)DataGridView.Rows[RowIndex];
                    if ((cellState & DataGridViewElementStates.Selected) != 0 && row.Enabled)
                    {
                        using (var brush = new SolidBrush(DataGridView.DefaultCellStyle.SelectionBackColor))
                            graphics.FillRectangle(
                                brush, cellBounds.X,
                                cellBounds.Y, cellBounds.Width, cellBounds.Height);
                    }
                    else
                    {
                        using (var brush = new SolidBrush(DataGridView.DefaultCellStyle.BackColor))
                            graphics.FillRectangle(brush, cellBounds.X,
                                                   cellBounds.Y, cellBounds.Width, cellBounds.Height);
                    }

                    if (row.Enabled)
                        graphics.DrawImage(icon, cellBounds.X, cellBounds.Y + 3, icon.Width, icon.Height);
                    else
                        graphics.DrawImage(icon, new Rectangle(cellBounds.X, cellBounds.Y + 3, icon.Width, icon.Height),
                                           0, 0, icon.Width, icon.Height, GraphicsUnit.Pixel,
                                           Drawing.GreyScaleAttributes);
                }
            }

            private DataGridViewCell _poolIconHostCheckCell;
            private DataGridViewTextBoxCell _versionCell;

            private readonly bool _showHostCheckBox = true;

            public PatchingHostsDataGridViewRow(Pool pool)
                : base(pool)
            {
                SetupCells();
            }

            public PatchingHostsDataGridViewRow(Host host, bool hasPool, bool showHostCheckBox = true)
                : base(host, hasPool)
            {
                _showHostCheckBox = showHostCheckBox;
                SetupCells();
            }

            public int VersionCellIndex
            {
                get { return Cells.IndexOf(_versionCell); }
            }

            public override bool IsCheckable
            {
                get { return !HasPool; }
            }

            public override bool Enabled
            {
                get
                {
                    return base.Enabled;
                }
                set
                {
                    base.Enabled = value;
                    UpdateDetails();
                }
            }

            public int CheckValue
            {
                get {
                    return IsPoolOrStandaloneHost
                               ? (int) Cells[POOL_CHECKBOX_COL].Value
                               : (int) Cells[POOL_ICON_HOST_CHECKBOX_COL].Value;
                }
            }

            public bool IsSelectableHost
            {
                get { return IsAHostRow && Enabled && (_showHostCheckBox || !HasPool); }
            }

            public bool IsSelectablePool
            {
                get { return IsAPoolRow && Enabled; }
            }

            private void SetupCells()
            {
                _poolCheckBoxCell = new DataGridViewCheckBoxCell { ThreeState = true };

                _expansionCell = new DataGridViewImageCell();

                if (IsPoolOrStandaloneHost)
                    _poolIconHostCheckCell = new DataGridViewIconCell(); 
                else
                    _poolIconHostCheckCell = new DataGridViewCheckBoxCell();

                _nameCell = new DataGridViewNameCell();
                _versionCell = new DataGridViewTextBoxCell();

                Cells.AddRange(new[] { _expansionCell, _poolCheckBoxCell, _poolIconHostCheckCell, _nameCell, _versionCell });

                this.UpdateDetails();
            }

            private void UpdateDetails()
            {
                if (Tag is Pool)
                {
                    Pool pool = (Pool)Tag;
                    Host master = pool.Connection.Resolve(pool.master);
                    _poolCheckBoxCell.Value = 0;
                    _expansionCell.Value = Resources.tree_minus;
                    _poolIconHostCheckCell.Value = Images.GetImage16For(pool);
                    _nameCell.Value = pool;
                    _versionCell.Value = master.ProductVersionTextShort();
                }
                else if (Tag is Host)
                {
                    Host host = (Host) Tag;
                    _poolCheckBoxCell.Value = 0;
                    _expansionCell.Value = Resources.tree_plus;
                    if (_hasPool)
                        _poolIconHostCheckCell.Value = 0;
                    else 
                        _poolIconHostCheckCell.Value = Images.GetImage16For(host);
                    _nameCell.Value = host;
                    _versionCell.Value = host.ProductVersionTextShort();
                }
            }

            internal void UpdateIcon()
            {
                if (_poolIconHostCheckCell is DataGridViewImageCell)
                {
                    _poolIconHostCheckCell.Value = Images.GetImage16For((IXenObject)Tag);
                }
            }
        }

        #endregion

    }
}
