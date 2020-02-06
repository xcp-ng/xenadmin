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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Alerts;
using XenAdmin.Controls;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Wizards.PatchingWizard;
using XenCenterLib;
using XenAPI;
using Timer = System.Windows.Forms.Timer;


namespace XenAdmin.TabPages
{
    public partial class ManageUpdatesPage : NotificationsBasePage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private int currentSpinningFrame;
        private Timer spinningTimer = new Timer();
        private ImageList imageList = new ImageList();

        private Dictionary<string, bool> expandedState = new Dictionary<string, bool>();
        private List<string> selectedUpdates = new List<string>();
        private List<string> collapsedPoolRowsList = new List<string>();
        private int checksQueue;
        private bool CheckForUpdatesInProgress;
        private readonly CollectionChangeEventHandler m_updateCollectionChangedWithInvoke;

        public ManageUpdatesPage()
        {
            InitializeComponent();
            InitializeProgressControls();
            tableLayoutPanel1.Visible = false;
            UpdateButtonEnablement();
            m_updateCollectionChangedWithInvoke = Program.ProgramInvokeHandler(UpdatesCollectionChanged);
            toolStripSplitButtonDismiss.DefaultItem = dismissAllToolStripMenuItem;
            toolStripSplitButtonDismiss.Text = dismissAllToolStripMenuItem.Text;

            try
            {
                //ensure we won't try to rebuild the list while setting the initial view
                checksQueue++;
                byHostToolStripMenuItem.Checked = Properties.Settings.Default.ShowUpdatesByServer;
                byUpdateToolStripMenuItem.Checked = !Properties.Settings.Default.ShowUpdatesByServer;
            }
            finally
            {
                checksQueue--;
            }
        }

        #region NotificationPage overrides
        protected override void RefreshPage()
        {
            toolStripDropDownButtonServerFilter.InitializeHostList();
            toolStripDropDownButtonServerFilter.BuildFilterList();
            Rebuild(); 
        }

        protected override void RegisterEventHandlers()
        {
            Updates.RegisterCollectionChanged(m_updateCollectionChangedWithInvoke);
            Updates.RestoreDismissedUpdatesStarted += Updates_RestoreDismissedUpdatesStarted;
            Updates.CheckForUpdatesStarted += CheckForUpdates_CheckForUpdatesStarted;
            Updates.CheckForUpdatesCompleted += CheckForUpdates_CheckForUpdatesCompleted;
        }

        protected override void DeregisterEventHandlers()
        {
            Updates.DeregisterCollectionChanged(m_updateCollectionChangedWithInvoke);
            Updates.RestoreDismissedUpdatesStarted -= Updates_RestoreDismissedUpdatesStarted;
            Updates.CheckForUpdatesStarted -= CheckForUpdates_CheckForUpdatesStarted;
            Updates.CheckForUpdatesCompleted -= CheckForUpdates_CheckForUpdatesCompleted;
        }

        public override string HelpID => "ManageUpdatesDialog";

        #endregion

        private void UpdatesCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.AssertOnEventThread();
            
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    Rebuild(); // rebuild entire alert list to ensure filtering and sorting
                    break;
                case CollectionChangeAction.Remove:
                    Alert a = e.Element as Alert;
                    if (a != null)
                    {
                        RemoveUpdateRow(a);
                    }
                    else
                    {
                        var range = e.Element as List<Alert>;
                        if (range != null)
                            Rebuild();
                    }
                    break;
            }
        }

        private void CheckForUpdates_CheckForUpdatesStarted()
        {
            Program.Invoke(Program.MainWindow, StartCheckForUpdates);
        }

        private void Updates_RestoreDismissedUpdatesStarted()
        {
            Program.Invoke(Program.MainWindow, StartCheckForUpdates);
        }

        private void StartCheckForUpdates()
        {
            if (CheckForUpdatesInProgress)
                return;

            CheckForUpdatesInProgress = true;

            checksQueue++;
            if (checksQueue > 1)
                return;

            toolStripButtonRefresh.Enabled = false;
            toolStripButtonRestoreDismissed.Enabled = false;

            StoreStateOfRows();
            dataGridViewUpdates.Rows.Clear();
            dataGridViewUpdates.Refresh();
            dataGridViewHosts.Rows.Clear();
            dataGridViewHosts.Refresh();

            checkForUpdatesNowLink.Enabled = false;
            checkForUpdatesNowButton.Visible = false;
            spinningTimer.Start();
            labelProgress.Text = Messages.AVAILABLE_UPDATES_SEARCHING;
            tableLayoutPanel3.Visible = true;
        }

        private void CheckForUpdates_CheckForUpdatesCompleted(bool succeeded, string errorMessage)
        {
            Program.Invoke(Program.MainWindow, delegate
                {
                    checksQueue--;
                    toolStripButtonRefresh.Enabled = true;
                    toolStripButtonRestoreDismissed.Enabled = true;
                    checkForUpdatesNowLink.Enabled = true;
                    checkForUpdatesNowButton.Visible = true;
                    spinningTimer.Stop();

                    if (succeeded)
                    {
                        Rebuild();
                    }
                    else
                    {
                        pictureBoxProgress.Image = SystemIcons.Error.ToBitmap();
                        labelProgress.Text = string.IsNullOrEmpty(errorMessage)
                                                 ? Messages.AVAILABLE_UPDATES_NOT_FOUND
                                                 : errorMessage;
                    }

                    CheckForUpdatesInProgress = false;
                });
        }

        private void InitializeProgressControls()
        {
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(32, 32);
            imageList.Images.AddRange(new Image[]
                {
                    Properties.Resources.SpinningFrame0,
                    Properties.Resources.SpinningFrame1,
                    Properties.Resources.SpinningFrame2,
                    Properties.Resources.SpinningFrame3,
                    Properties.Resources.SpinningFrame4,
                    Properties.Resources.SpinningFrame5,
                    Properties.Resources.SpinningFrame6,
                    Properties.Resources.SpinningFrame7
                });

            spinningTimer.Tick += timer_Tick;
            spinningTimer.Interval = 150;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            int imageIndex = ++currentSpinningFrame <= 7 ? currentSpinningFrame : currentSpinningFrame = 0;
            pictureBoxProgress.Image = imageList.Images[imageIndex];
        }

        private void SetFilterLabel()
        {
            toolStripLabelFiltersOnOff.Text = FilterIsOn
                                                  ? Messages.FILTERS_ON
                                                  : Messages.FILTERS_OFF;
        }
        
        private bool FilterIsOn
        {
            get
            {
                return toolStripDropDownButtonDateFilter.FilterIsOn ||
                    toolStripDropDownButtonServerFilter.FilterIsOn;
            }
        }

        private void Rebuild()
        {
            Program.AssertOnEventThread();

            if (!Visible || checksQueue > 0)
                return;

            SetFilterLabel();
            ToggleTopWarningVisibility();

            if (byUpdateToolStripMenuItem.Checked)
                RebuildUpdateView();
            else
                RebuildHostView();
        }

        private class UpdatePageByHostDataGridView : CollapsingPoolHostDataGridView
        {
            protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
            {
                base.OnCellPainting(e);

                if (e.RowIndex >= 0)
                {
                    UpdatePageDataGridViewRow row = (UpdatePageDataGridViewRow) Rows[e.RowIndex];

                    if (!row.IsFullyPopulated && e.ColumnIndex == row.PatchingStatusCellIndex
                       || (row.Tag is Host && (e.ColumnIndex == row.ExpansionCellIndex
                                               || (row.HasPool && (e.ColumnIndex == row.IconCellIndex || e.ColumnIndex == row.PatchingStatusCellIndex)))))
                    {
                        e.PaintBackground(e.ClipBounds, true);
                        e.Handled = true;
                    }
                }
            }

            protected override void SortColumns()
            {
                UpdatePageDataGridViewRow firstRow = Rows[0] as UpdatePageDataGridViewRow;
                if (firstRow == null)
                    return;

                if (columnToBeSortedIndex == firstRow.NameCellIndex ||
                    columnToBeSortedIndex == firstRow.VersionCellIndex ||
                    columnToBeSortedIndex == firstRow.StatusCellIndex)
                    SortAndRebuildTree(new CollapsingPoolHostRowSorter<UpdatePageDataGridViewRow>(direction, columnToBeSortedIndex));
            }
        }

        private class UpdatePageDataGridViewRow : CollapsingPoolHostDataGridViewRow
        {
            private readonly DataGridViewImageCell _poolIconCell = new DataGridViewImageCell();
            private readonly DataGridViewTextBoxCell _versionCell = new DataGridViewTextBoxCell();
            private readonly DataGridViewImageCell _patchingStatusCell = new DataGridViewImageCell();
            private readonly DataGridViewTextBoxCell _statusCell = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _requiredUpdateCell = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _installedUpdateCell = new DataGridViewTextBoxCell();

            public UpdatePageDataGridViewRow(Pool pool, bool isFullyPopulated)
                : base(pool)
            {
                IsFullyPopulated = isFullyPopulated;
                SetupCells();
            }

            public UpdatePageDataGridViewRow(Host host, bool hasPool, bool isFullyPopulated)
                : base(host, hasPool)
            {
                IsFullyPopulated = isFullyPopulated;
                SetupCells();
            }

            public int IconCellIndex
            {
                get { return Cells.IndexOf(_poolIconCell);  }
            }

            public int VersionCellIndex
            {
                get { return Cells.IndexOf(_versionCell); }
            }

            public int PatchingStatusCellIndex
            {
                get { return Cells.IndexOf(_patchingStatusCell); }
            }

            public int StatusCellIndex
            {
                get { return Cells.IndexOf(_statusCell); }
            }

            public override bool IsCheckable
            {
                get { return IsPoolOrStandaloneHost; }
            }

            public bool IsFullyPopulated { get; private set; }

            private void SetupCells()
            {
                _nameCell = new DataGridViewTextAndImageCell();
                Cells.AddRange(_expansionCell, _poolIconCell, _nameCell, _versionCell, _patchingStatusCell, _statusCell, _requiredUpdateCell, _installedUpdateCell);

                UpdateDetails();
            }

            // fill data into row
            private void UpdateDetails()
            {
                Pool pool = Tag as Pool;

                if (pool != null)
                {
                    Host master = pool.Connection.Resolve(pool.master);
                    SetCollapseIcon();
                    _poolIconCell.Value = Images.GetImage16For(pool);

                    DataGridViewTextAndImageCell nc = _nameCell as DataGridViewTextAndImageCell;
                    if (nc != null)
                        nc.Image = null;

                    _nameCell.Value = pool.Name();
                    _versionCell.Value = master.ProductVersionTextShort();
                    _requiredUpdateCell.Value = String.Empty;
                    _installedUpdateCell.Value = String.Empty;

                    if (IsFullyPopulated)
                    {
                        var outOfDate = pool.Connection.Cache.Hosts.Any(h => RequiredUpdatesForHost(h).Length > 0);
                        _patchingStatusCell.Value = outOfDate
                            ? Properties.Resources._000_error_h32bit_16
                            : Properties.Resources._000_Tick_h32bit_16;
                        _statusCell.Value = outOfDate ? Messages.NOT_UPDATED : Messages.UPDATED;
                    }
                    else
                    {
                        _statusCell.Value = String.Empty;
                    }
                }
                
                else
                {
                    Host host = Tag as Host;
                    if (host != null)
                    {
                        var hostRequired = RequiredUpdatesForHost(host);
                        var hostInstalled = InstalledUpdatesForHost(host);
                        var outOfDate = hostRequired.Length > 0;

                        DataGridViewTextAndImageCell nc = _nameCell as DataGridViewTextAndImageCell;

                        if (_hasPool && nc != null) // host in pool
                        {
                            nc.Image = Images.GetImage16For(host);
                            _statusCell.Value = String.Empty;
                        }
                        else if (!_hasPool && nc != null) // standalone host
                        {
                            _poolIconCell.Value = Images.GetImage16For(host);
                            nc.Image = null;
                            if (IsFullyPopulated)
                            {
                                _patchingStatusCell.Value = outOfDate
                                    ? Properties.Resources._000_error_h32bit_16
                                    : Properties.Resources._000_Tick_h32bit_16;
                                _statusCell.Value = outOfDate ? Messages.NOT_UPDATED : Messages.UPDATED;
                            }
                            else
                            {
                                _statusCell.Value = String.Empty;
                            }
                        }

                        _nameCell.Value = host.Name();
                        _versionCell.Value = host.ProductVersionTextShort();
                        _installedUpdateCell.Value = hostInstalled;
                        if (IsFullyPopulated)
                            _requiredUpdateCell.Value = hostRequired;
                        else
                            _requiredUpdateCell.Value = String.Empty;
                    }
                }
            }

        }

        private static string RequiredUpdatesForHost(Host host)
        {
            var requiredUpdates = Updates.RecommendedPatchesForHost(host);

            if (requiredUpdates == null)
            {
                // versions with no minimum patches
                var updatesList = new List<string>();
                var alerts = new List<Alert>(Updates.UpdateAlerts);
                if (alerts.Count == 0)
                    return String.Empty;

                foreach (Alert alert in alerts)
                {
                    var patchAlert = alert as XenServerPatchAlert;
                    if (patchAlert == null)
                        continue;
                    if (patchAlert.DistinctHosts.Contains(host))
                        updatesList.Add(patchAlert.Name);
                }

                updatesList.Sort(StringUtility.NaturalCompare);
                return string.Join(", ", updatesList.ToArray());
            }

            else
            {
                // versions with minimum patches
                var result = new List<string>();
                foreach (var patch in requiredUpdates)
                    result.Add(patch.Name);

                return string.Join(", ", result.ToArray());
            }
        }

        private static string InstalledUpdatesForHost(Host host)
        {
            List<string> result = new List<string>();

            result.AddRange(Helpers.ElyOrGreater(host)
                ? host.AppliedUpdates().Select(u => u.Name())
                : host.AppliedPatches().Select(p => p.Name()));

            result.Sort(StringUtility.NaturalCompare);
            return string.Join(", ", result.ToArray());
        }

        private bool VersionFoundInUpdatesXml(IXenConnection connection)
        {
            return connection.Cache.Hosts.All(h => Updates.GetServerVersions(h, Updates.XenServerVersions).Count > 0);
        }

        private bool VersionFoundInUpdatesXml(Host host)
        {
            return Updates.GetServerVersions(host, Updates.XenServerVersions).Count > 0;
        }

        private void RebuildHostView()
        {
            try
            {
                dataGridViewHosts.SuspendLayout();

                if (dataGridViewHosts.RowCount > 0)
                {
                    StoreStateOfRows();
                    dataGridViewHosts.Rows.Clear();
                    dataGridViewHosts.Refresh();
                }

                ToggleCentreWarningVisibility();

                List<IXenConnection> xenConnections = ConnectionsManager.XenConnectionsCopy;
                xenConnections.Sort();

                var rowList = new List<DataGridViewRow>();

                foreach (IXenConnection c in xenConnections)
                {
                    Pool pool = Helpers.GetPool(c);

                    if (pool != null)          // pool row
                    {
                        var row = new UpdatePageDataGridViewRow(pool, VersionFoundInUpdatesXml(c));
                        var hostUuidList = new List<string>();

                        foreach (Host h in c.Cache.Hosts)
                            hostUuidList.Add(h.uuid);

                        // add row based on server filter status
                        if (!toolStripDropDownButtonServerFilter.HideByLocation(hostUuidList))
                            rowList.Add(row);
                    }

                    Host[] hosts = c.Cache.Hosts;
                    Array.Sort(hosts);
                    foreach (Host h in hosts)       // host row
                    {
                        var row = new UpdatePageDataGridViewRow(h, pool != null, VersionFoundInUpdatesXml(h));

                        // add row based on server filter status
                        if (!toolStripDropDownButtonServerFilter.HideByLocation(h.uuid))
                            rowList.Add(row);
                    }
                }

                dataGridViewHosts.Rows.AddRange(rowList.ToArray());

                // restore selected state and pool collapsed state
                bool checkHostRow = false;
                foreach (UpdatePageDataGridViewRow row in dataGridViewHosts.Rows)
                {
                    if (checkHostRow)
                    {
                        if (!row.IsPoolOrStandaloneHost)
                        {
                            row.Visible = !row.Visible;
                            continue;
                        }
                        else
                            checkHostRow = false;
                    }

                    Pool pool = row.Tag as Pool;
                    if (pool != null)
                    {
                        row.Selected = selectedUpdates.Contains(pool.uuid);
                        if (collapsedPoolRowsList.Contains(pool.uuid))
                        {
                            row.SetExpandIcon();
                            checkHostRow = true;
                        }
                    }
                    else
                    {
                        Host host = row.Tag as Host;
                        if (host != null)
                            row.Selected = selectedUpdates.Contains(host.uuid);
                    }
                }

                if (dataGridViewHosts.SelectedRows.Count == 0 && dataGridViewHosts.Rows.Count > 0)
                    dataGridViewHosts.Rows[0].Selected = true;
            }
            finally
            {
                dataGridViewHosts.ResumeLayout();
                UpdateButtonEnablement();
            }
        }

        private void RebuildUpdateView()
        {
            try
            {
                dataGridViewUpdates.SuspendLayout();

                if (dataGridViewUpdates.RowCount > 0)
                {
                    StoreStateOfRows();
                    dataGridViewUpdates.Rows.Clear();
                    dataGridViewUpdates.Refresh();
                }

                ToggleCentreWarningVisibility();
                
                var updates = new List<Alert>(Updates.UpdateAlerts);
                if (updates.Count == 0)
                    return;

                updates.RemoveAll(FilterAlert);
                sortUpdates(updates);

                var rowList = new List<DataGridViewRow>();

                foreach (var myAlert in updates)
                    rowList.Add(NewUpdateRow(myAlert));

                dataGridViewUpdates.Rows.AddRange(rowList.ToArray());

                foreach (DataGridViewRow row in dataGridViewUpdates.Rows)
                    row.Selected = selectedUpdates.Contains(((Alert)row.Tag).uuid);

                if (dataGridViewUpdates.SelectedRows.Count == 0 && dataGridViewUpdates.Rows.Count > 0)
                    dataGridViewUpdates.Rows[0].Selected = true;
            }
            finally
            {
                dataGridViewUpdates.ResumeLayout();
                UpdateButtonEnablement();
            }
        }

        private void ToggleCentreWarningVisibility()
        {
            if (byUpdateToolStripMenuItem.Checked && Updates.UpdateAlertsCount == 0)
            {
                pictureBoxProgress.Image = SystemIcons.Information.ToBitmap();
                labelProgress.Text =  Messages.AVAILABLE_UPDATES_NOT_FOUND;
                tableLayoutPanel3.Visible = true;
            }
            else
            {
                tableLayoutPanel3.Visible = false;
            }
        }

        /// <summary>
        /// Toggles the visibility of the warning that appears above the grid saying:
        /// "Automatic checking for updates is disabled for some types of updates".
        /// </summary>
        private void ToggleTopWarningVisibility()
        {
            bool visible = !Properties.Settings.Default.AllowPatchesUpdates ||
                           !Properties.Settings.Default.AllowXenCenterUpdates ||
                           !Properties.Settings.Default.AllowXenServerUpdates;

            pictureBox1.Visible = visible;
            AutoCheckForUpdatesDisabledLabel.Visible = visible;
            checkForUpdatesNowLink.Visible = visible;
        }

        /// <summary>
        /// Runs all the current filters on the alert to determine if it should be shown in the list or not.
        /// </summary>
        /// <param name="alert"></param>
        private bool FilterAlert(Alert alert)
        {
            var hosts = new List<string>();
            var serverUpdate = alert as XenServerUpdateAlert;
            if (serverUpdate != null)
                hosts = serverUpdate.DistinctHosts.Select(h => h.uuid).ToList();

            bool hide = false; 

            Program.Invoke(Program.MainWindow, () =>
                                 hide = toolStripDropDownButtonDateFilter.HideByDate(alert.Timestamp.ToLocalTime())
                                        || toolStripDropDownButtonServerFilter.HideByLocation(hosts) || alert.IsDismissed());
            return hide;
        }

        private void StoreStateOfRows()
        {
            selectedUpdates.Clear();

            if (byUpdateToolStripMenuItem.Checked)    // by update view
            {
                selectedUpdates = (dataGridViewUpdates.SelectedRows.Cast<DataGridViewRow>().Select(
                    selectedRow => ((Alert) selectedRow.Tag).uuid)).ToList();
            }
            else    // by host view
            {
                collapsedPoolRowsList.Clear();
                foreach (UpdatePageDataGridViewRow row in dataGridViewHosts.Rows)
                {
                    Pool pool = row.Tag as Pool;
                    if (pool != null)
                    {
                        if (row.Selected)
                            selectedUpdates.Add(pool.uuid);
                        if (row.IsACollapsedRow)
                            collapsedPoolRowsList.Add(pool.uuid);
                    }
                    else
                    {
                        Host host = row.Tag as Host;
                        if (host != null && row.Selected)
                            selectedUpdates.Add(host.uuid);
                    }
                }
            }
        }

        private void UpdateButtonEnablement()
        {
            if (byUpdateToolStripMenuItem.Checked)
            {
                toolStripButtonExportAll.Enabled = toolStripSplitButtonDismiss.Enabled = Updates.UpdateAlertsCount > 0;
            }
            else
            {
                var connectionList = ConnectionsManager.XenConnectionsCopy;
                toolStripButtonUpdate.Enabled = toolStripButtonExportAll.Enabled = connectionList.Any(xenConnection => xenConnection.IsConnected);
            }
        }

        private void ShowInformationHelper(string reason)
        {
            if (string.IsNullOrEmpty(reason))
            {
                tableLayoutPanel1.Visible = false;
            }
            else
            {
                informationLabel.Text = reason;
                tableLayoutPanel1.Visible = !HiddenFeatures.LinkLabelHidden;
            }
        }

        private DataGridViewRow NewUpdateRow(Alert alert)
        {
            var expanderCell = new DataGridViewImageCell();
            var appliesCell = new DataGridViewTextBoxCell();
            var detailCell = new DataGridViewTextBoxCell();
            var dateCell = new DataGridViewTextBoxCell();

            var actionItems = GetAlertActionItems(alert);
            var actionCell = new DataGridViewDropDownSplitButtonCell(actionItems.ToArray());
            var newRow = new DataGridViewRow { Tag = alert, MinimumHeight = DataGridViewDropDownSplitButtonCell.MIN_ROW_HEIGHT };

            // Set the detail cell content and expanding arrow
            if (expandedState.ContainsKey(alert.uuid))
            {
                // show the expanded arrow and the body detail
                expanderCell.Value = Properties.Resources.expanded_triangle;
                detailCell.Value = String.Format("{0}\n\n{1}", alert.Title, alert.Description);
            }
            else
            {
                // show the expand arrow and just the title
                expanderCell.Value = Properties.Resources.contracted_triangle;
                detailCell.Value = alert.Title;
            }

            appliesCell.Value = alert.AppliesTo;
            dateCell.Value = HelpersGUI.DateTimeToString(alert.Timestamp.ToLocalTime(), Messages.DATEFORMAT_DMY, true);
            newRow.Cells.AddRange(expanderCell, detailCell, appliesCell, dateCell, actionCell);

            return newRow;
        }

        private void RemoveUpdateRow(Alert a)
        {
            for (int i = 0; i < dataGridViewUpdates.Rows.Count; i++)
            {
                if (((Alert)dataGridViewUpdates.Rows[i].Tag).uuid == a.uuid)
                    dataGridViewUpdates.Rows.RemoveAt(i);
            }
        }

        private List<ToolStripItem> GetAlertActionItems(Alert alert)
        {
            var items = new List<ToolStripItem>();

            var patchAlert = alert as XenServerPatchAlert;

            if (Alert.AllowedToDismiss(alert))
            {
                var dismiss = new ToolStripMenuItem(Messages.ALERT_DISMISS);
                dismiss.Click += ToolStripMenuItemDismiss_Click;
                items.Add(dismiss);
            }

            if (patchAlert != null && patchAlert.CanApply && !string.IsNullOrEmpty(patchAlert.Patch.PatchUrl) && patchAlert.RequiredXenCenterVersion == null)
            {
                var download = new ToolStripMenuItem(Messages.UPDATES_DOWNLOAD_AND_INSTALL);
                download.Click += ToolStripMenuItemDownload_Click;
                items.Add(download);
            }

            var updateAlert = alert as XenServerUpdateAlert;

            if (updateAlert != null && updateAlert.RequiredXenCenterVersion != null)
            {
                var downloadNewXenCenter = new ToolStripMenuItem(Messages.UPDATES_DOWNLOAD_REQUIRED_XENCENTER);
                downloadNewXenCenter.Click += ToolStripMenuItemDownloadNewXenCenter_Click;
                items.Add(downloadNewXenCenter);
            }

            if (!string.IsNullOrEmpty(alert.WebPageLabel))
            {
                var fix = new ToolStripMenuItem(alert.FixLinkText);
                fix.Click += ToolStripMenuItemGoToWebPage_Click;
                items.Add(fix);
            }

            if (items.Count > 0)
                items.Add(new ToolStripSeparator());

            var copy = new ToolStripMenuItem(Messages.COPY);
            copy.Click += ToolStripMenuItemCopy_Click;
            items.Add(copy);

            return items;
        }

      
        #region Update dismissal
 
        private void DismissUpdates(IEnumerable<Alert> alerts)
        {
            var groups = from Alert alert in alerts
                         where alert != null && !alert.Dismissing
                         group alert by alert.Connection
                         into g
                         select new { Connection = g.Key, Alerts = g };

            foreach (var g in groups)
            {
                if (Alert.AllowedToDismiss(g.Connection))
                {
                    foreach (Alert alert in g.Alerts)
                    {
                        alert.Dismissing = true;
                    }
                    toolStripButtonRestoreDismissed.Enabled = false;
                    DeleteAllAlertsAction action = new DeleteAllAlertsAction(g.Connection, g.Alerts);
                    action.Completed += DeleteAllAlertsAction_Completed;
                    action.RunAsync();
                }
            }
        }

        /// <summary>
        /// After the Delete action is completed the page is refreshed and the restore dismissed 
        /// button is enabled again.
        /// </summary>
        /// <param name="sender"></param>
        private void DeleteAllAlertsAction_Completed(ActionBase sender)
        {
            Program.Invoke(Program.MainWindow, () =>
            {
                toolStripButtonRestoreDismissed.Enabled = true;
                ToggleCentreWarningVisibility();
                ToggleTopWarningVisibility();
            });
        }

        #endregion


        #region Actions DropDown event handlers

        private void toolStripSplitButtonDismiss_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            toolStripSplitButtonDismiss.DefaultItem = e.ClickedItem;
            toolStripSplitButtonDismiss.Text = toolStripSplitButtonDismiss.DefaultItem.Text;
        }

        /// <summary>
        /// If the answer of the user to the dialog is YES, then make a list with all the updates and call 
        /// DismissUpdates on that list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dismissAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = DialogResult.Yes;

            if (FilterIsOn)
            {
                using (var dlog = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.UPDATE_DISMISS_ALL_CONTINUE),
                    new ThreeButtonDialog.TBDButton(Messages.DISMISS_ALL_CONFIRM_BUTTON, DialogResult.Yes),
                    new ThreeButtonDialog.TBDButton(Messages.DISMISS_FILTERED_CONFIRM_BUTTON, DialogResult.No, ThreeButtonDialog.ButtonType.NONE),
                    ThreeButtonDialog.ButtonCancel))
                {
                    result = dlog.ShowDialog(this);
                }
            }
            else if (!Properties.Settings.Default.DoNotConfirmDismissUpdates)
            {
                using (var dlog = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.UPDATE_DISMISS_ALL_NO_FILTER_CONTINUE),
                    new ThreeButtonDialog.TBDButton(Messages.DISMISS_ALL_YES_CONFIRM_BUTTON, DialogResult.Yes),
                    ThreeButtonDialog.ButtonCancel)
                {
                    ShowCheckbox = true,
                    CheckboxCaption = Messages.DO_NOT_SHOW_THIS_MESSAGE
                })
                {
                    result = dlog.ShowDialog(this);
                    Properties.Settings.Default.DoNotConfirmDismissUpdates = dlog.IsCheckBoxChecked;
                    Settings.TrySaveSettings();
                }
            }

            if (result == DialogResult.Cancel)
                return;

            var alerts = result == DialogResult.No
                         ? from DataGridViewRow row in dataGridViewUpdates.Rows select row.Tag as Alert
                         : Updates.UpdateAlerts;

            DismissUpdates(alerts);
        }

        /// <summary>
        /// If the answer of the user to the dialog is YES, then make a list of all the selected rows
        /// and call DismissUpdates on that list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dismissSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.DoNotConfirmDismissUpdates)
            {
                using (var dlog = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.UPDATE_DISMISS_SELECTED_CONFIRM, Messages.XENCENTER),
                    ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                {
                    ShowCheckbox = true,
                    CheckboxCaption = Messages.DO_NOT_SHOW_THIS_MESSAGE
                })
                {
                    var result = dlog.ShowDialog(this);
                    Properties.Settings.Default.DoNotConfirmDismissUpdates = dlog.IsCheckBoxChecked;
                    Settings.TrySaveSettings();
                    
                    if (result != DialogResult.Yes)
                        return;
                }
            }

            if (dataGridViewUpdates.SelectedRows.Count > 0)
            {
                var selectedAlerts = from DataGridViewRow row in dataGridViewUpdates.SelectedRows select row.Tag as Alert;
                DismissUpdates(selectedAlerts);
            }
        }

        private void ToolStripMenuItemDismiss_Click(object sender, EventArgs e)
        {
            if (dataGridViewUpdates.SelectedRows.Count != 1)
                log.DebugFormat("Only 1 update can be dismissed at a time (Attempted to dismiss {0}). Dismissing the clicked item.", dataGridViewUpdates.SelectedRows.Count);

            DataGridViewRow clickedRow = FindAlertRow(sender as ToolStripMenuItem);
            if (clickedRow == null)
            {
                log.Debug("Attempted to dismiss update with no update selected.");
                return;
            }

            Alert alert = (Alert)clickedRow.Tag;
            if (alert == null)
                return;

            if (!Properties.Settings.Default.DoNotConfirmDismissUpdates)
            {
                using (var dlog = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.UPDATE_DISMISS_CONFIRM, Messages.XENCENTER),
                    ThreeButtonDialog.ButtonYes,
                    ThreeButtonDialog.ButtonNo)
                {
                    ShowCheckbox = true,
                    CheckboxCaption = Messages.DO_NOT_SHOW_THIS_MESSAGE
                })
                {
                    var result = dlog.ShowDialog(this);
                    Properties.Settings.Default.DoNotConfirmDismissUpdates = dlog.IsCheckBoxChecked;
                    Settings.TrySaveSettings();
                    
                    if (result != DialogResult.Yes)
                        return;
                }
            }

            DismissUpdates(new List<Alert> { (Alert)clickedRow.Tag });
        }

        private DataGridViewRow FindAlertRow(ToolStripMenuItem toolStripMenuItem)
        {
            if (toolStripMenuItem == null)
                return null;

            return (from DataGridViewRow row in dataGridViewUpdates.Rows
                    where row.Cells.Count > 0
                    let actionCell = row.Cells[row.Cells.Count - 1] as DataGridViewDropDownSplitButtonCell
                    where actionCell != null && actionCell.ContextMenu.Items.Cast<object>().Any(item => item is ToolStripMenuItem && item == toolStripMenuItem)
                    select row).FirstOrDefault();
        }

        private void ToolStripMenuItemGoToWebPage_Click(object sender, EventArgs e)
        {
            DataGridViewRow clickedRow = FindAlertRow(sender as ToolStripMenuItem);
            if (clickedRow == null)
                return;

            Alert alert = (Alert) clickedRow.Tag;
            if (alert != null && alert.FixLinkAction != null)
                alert.FixLinkAction.Invoke();
        }

        private void ToolStripMenuItemDownload_Click(object sender, EventArgs e)
        {
            DataGridViewRow clickedRow = FindAlertRow(sender as ToolStripMenuItem);
            if (clickedRow == null)
                return;

            XenServerPatchAlert patchAlert = (XenServerPatchAlert) clickedRow.Tag;

            if (patchAlert == null)
                return;

            string patchUri = patchAlert.Patch.PatchUrl;
            if (string.IsNullOrEmpty(patchUri))
                return;

            PatchingWizard wizard = (PatchingWizard)Program.MainWindow.ShowForm(typeof(PatchingWizard));
            if (!wizard.IsFirstPage())
                return;
            wizard.NextStep();
            wizard.AddAlert(patchAlert);
            wizard.NextStep();

            var hosts = patchAlert.DistinctHosts;
            if (hosts.Count > 0)
            {                          
                wizard.SelectServers(hosts);
            }
            else
            {
                string disconnectedServerNames =
                       clickedRow.Cells[ColumnLocation.Index].Value.ToString();

                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning,
                                                  string.Format(Messages.UPDATES_WIZARD_DISCONNECTED_SERVER, 
                                                               disconnectedServerNames),
                                                  Messages.UPDATES_WIZARD)))
                {
                    dlg.ShowDialog(this);
                }
            }
        }

        private void ToolStripMenuItemDownloadNewXenCenter_Click(object sender, EventArgs e)
        {
            DataGridViewRow clickedRow = FindAlertRow(sender as ToolStripMenuItem);
            if (clickedRow == null)
                return;

            XenServerUpdateAlert updateAlert = (XenServerUpdateAlert)clickedRow.Tag;

            if (updateAlert == null || updateAlert.RequiredXenCenterVersion == null)
                return;

            string xenCenterUrl = updateAlert.RequiredXenCenterVersion.Url;
            if (string.IsNullOrEmpty(xenCenterUrl))
                return;

            Program.Invoke(Program.MainWindow, () => Program.OpenURL(xenCenterUrl));
        }

        private void ToolStripMenuItemCopy_Click(object sender, EventArgs e)
        {
            DataGridViewRow clickedRow = FindAlertRow(sender as ToolStripMenuItem);
            if (clickedRow == null)
                return;

            StringBuilder sb = new StringBuilder();
            
            if (dataGridViewUpdates.SelectedRows.Count > 1 && dataGridViewUpdates.SelectedRows.Contains(clickedRow))
            {
                foreach (DataGridViewRow r in dataGridViewUpdates.SelectedRows)
                {
                    Alert alert = (Alert) r.Tag;
                    sb.AppendLine(alert.GetUpdateDetailsCSVQuotes());
                }
            }
            else
            {
                Alert alert = (Alert) clickedRow.Tag;
                if (alert != null) 
                    sb.AppendLine(alert.GetUpdateDetailsCSVQuotes());
            }

            Clip.SetClipboardText(sb.ToString());
        }

        #endregion

        #region DataGridView event handlers

        private void dataGridViewUpdates_SelectionChanged(object sender, EventArgs e)
        {
            string reason = null;

            if (dataGridViewUpdates.SelectedRows.Count > 0)
            {
                var alert = dataGridViewUpdates.SelectedRows[0].Tag as XenServerPatchAlert;
                
                if (alert != null)
                    reason = alert.CannotApplyReason;
            }

            ShowInformationHelper(reason);
        }

        private void dataGridViewUpdates_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridViewUpdates.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.Automatic)
                Rebuild();
        }

        /// <summary>
        /// Handles the automatic sorting of the AlertsGridView for the non-string columns
        /// </summary>
        private void dataGridViewUpdates_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            Alert alert1 = (Alert)dataGridViewUpdates.Rows[e.RowIndex1].Tag;
            Alert alert2 = (Alert)dataGridViewUpdates.Rows[e.RowIndex2].Tag;

            if (e.Column.Index == ColumnDate.Index)
            {
                int sortResult = DateTime.Compare(alert1.Timestamp, alert2.Timestamp);
                e.SortResult = (dataGridViewUpdates.SortOrder == SortOrder.Descending) ? sortResult *= -1 : sortResult;
                e.Handled = true;
            }
        }

        private void dataGridViewUpdates_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // If you click on the headers you can get -1 as the index.
            if (e.ColumnIndex < 0 || e.RowIndex < 0 || e.ColumnIndex != ColumnExpand.Index)
                return;

            ToggleExpandedState(e.RowIndex);
        }

        private void dataGridViewUpdates_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // If you click on the headers you can get -1 as the index.
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;
            
            ToggleExpandedState(e.RowIndex);
        }

        private void dataGridViewUpdates_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right) // expand all selected rows
            {
                foreach (DataGridViewBand row in dataGridViewUpdates.SelectedRows)
                {
                    Alert alert = (Alert)dataGridViewUpdates.Rows[row.Index].Tag;
                    
                    if (!expandedState.ContainsKey(alert.uuid))
                        ToggleExpandedState(row.Index);
                }
            }
            else if (e.KeyCode == Keys.Left) // collapse all selected rows
            {
                foreach (DataGridViewBand row in dataGridViewUpdates.SelectedRows)
                {
                    Alert alert = (Alert)dataGridViewUpdates.Rows[row.Index].Tag;
                    
                    if (expandedState.ContainsKey(alert.uuid))
                        ToggleExpandedState(row.Index);
                }
            }
            else if (e.KeyCode == Keys.Enter) // toggle expanded state for all selected rows
            {
                foreach (DataGridViewBand row in dataGridViewUpdates.SelectedRows)
                    ToggleExpandedState(row.Index);
            }
        }

        /// <summary>
        /// Toggles the row specified between the expanded and contracted state
        /// </summary>
        private void ToggleExpandedState(int rowIndex)
        {
            Alert alert = (Alert)dataGridViewUpdates.Rows[rowIndex].Tag;

            if (expandedState.ContainsKey(alert.uuid))
            {
                expandedState.Remove(alert.uuid);
                dataGridViewUpdates.Rows[rowIndex].Cells[ColumnMessage.Index].Value = alert.Title;
                dataGridViewUpdates.Rows[rowIndex].Cells[ColumnExpand.Index].Value = Properties.Resources.contracted_triangle;
            }
            else
            {
                expandedState.Add(alert.uuid, true);
                dataGridViewUpdates.Rows[rowIndex].Cells[ColumnMessage.Index].Value
                    = string.Format("{0}\n\n{1}", alert.Title, alert.Description);
                dataGridViewUpdates.Rows[rowIndex].Cells[ColumnExpand.Index].Value = Properties.Resources.expanded_triangle;
            }
        }
        
        #endregion

        
        #region Top ToolStripButtons event handlers

        private void toolStripDropDownButtonDateFilter_FilterChanged()
        {
            Rebuild();
        }

        private void toolStripDropDownButtonServerFilter_FilterChanged()
        {
            Rebuild();
        }
        
        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            Updates.CheckForUpdates(true);
        }

        private void toolStripButtonExportAll_Click(object sender, EventArgs e)
        {
            bool exportAll = true;

            if (FilterIsOn)
            {
                using (var dlog = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.UPDATE_EXPORT_ALL_OR_FILTERED),
                    new ThreeButtonDialog.TBDButton(Messages.EXPORT_ALL_BUTTON, DialogResult.Yes),
                    new ThreeButtonDialog.TBDButton(Messages.EXPORT_FILTERED_BUTTON, DialogResult.No, ThreeButtonDialog.ButtonType.NONE),
                    ThreeButtonDialog.ButtonCancel))
                {
                    var result = dlog.ShowDialog(this);
                    if (result == DialogResult.No)
                        exportAll = false;
                    else if (result == DialogResult.Cancel)
                        return;
                }
            }

            string fileName;
            using (SaveFileDialog dialog = new SaveFileDialog
            {
                AddExtension = true,
                Filter = string.Format("{0} (*.csv)|*.csv|{1} (*.*)|*.*",
                                       Messages.CSV_DESCRIPTION, Messages.ALL_FILES),
                FilterIndex = 0,
                Title = Messages.EXPORT_ALL,
                RestoreDirectory = true,
                DefaultExt = "csv",
                CheckPathExists = false,
                OverwritePrompt = true
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;
                fileName = dialog.FileName;
            }

            new DelegatedAsyncAction(null,
                string.Format(Messages.EXPORT_UPDATES, fileName),
                string.Format(Messages.EXPORTING_UPDATES, fileName),
                string.Format(Messages.EXPORTED_UPDATES, fileName),
                delegate
                {
                    using (StreamWriter stream = new StreamWriter(fileName, false, Encoding.UTF8))
                    {
                        if (byUpdateToolStripMenuItem.Checked)     // update view
                        {
                            stream.WriteLine("{0},{1},{2},{3},{4}", Messages.TITLE,
                                Messages.DESCRIPTION, Messages.APPLIES_TO,
                                Messages.TIMESTAMP, Messages.WEB_PAGE);

                            if (exportAll)
                            {
                                foreach (Alert a in Updates.UpdateAlerts)
                                    stream.WriteLine(a.GetUpdateDetailsCSVQuotes());
                            }
                            else
                            {
                                foreach (DataGridViewRow row in dataGridViewUpdates.Rows)
                                {
                                    var a = row.Tag as Alert;
                                    if (a != null)
                                        stream.WriteLine(a.GetUpdateDetailsCSVQuotes());
                                }
                            }
                        }
                        else      // host view
                        {
                            stream.WriteLine("{0},{1},{2},{3},{4},{5}", Messages.POOL,
                                Messages.SERVER, Messages.VERSION, Messages.STATUS,
                                Messages.REQUIRED_UPDATES, Messages.INSTALLED_UPDATES);

                            if (exportAll)
                            {
                                List<IXenConnection> xenConnections = ConnectionsManager.XenConnectionsCopy;
                                xenConnections.Sort();

                                foreach (IXenConnection xenConnection in xenConnections)
                                {
                                    Pool pool = Helpers.GetPool(xenConnection);
                                    var hasPool = (pool != null);

                                    Host[] hosts = xenConnection.Cache.Hosts;
                                    Array.Sort(hosts);
                                    foreach (Host host in hosts)
                                        stream.WriteLine(GetHostUpdateDetailsCsvQuotes(xenConnection, host, hasPool));
                                }
                            }
                            else
                            {
                                foreach (UpdatePageDataGridViewRow row in dataGridViewHosts.Rows)
                                {
                                    Host host = row.Tag as Host;
                                    if (host != null)
                                        stream.WriteLine(GetHostUpdateDetailsCsvQuotes(host.Connection, host, row.HasPool));
                                }
                            }
                        }
                    }
                }).RunAsync();
        }

        private string GetHostUpdateDetailsCsvQuotes(IXenConnection xenConnection, Host host, bool hasPool)
        {
            string pool = String.Empty;
            string patchingStatus = String.Empty;
            string requiredUpdates = String.Empty;
            string installedUpdates = String.Empty;
            var versionIsFound = VersionFoundInUpdatesXml(host);

            pool = hasPool ? Helpers.GetPool(xenConnection).Name() : String.Empty;
            requiredUpdates = versionIsFound ? RequiredUpdatesForHost(host) : String.Empty;
            installedUpdates = InstalledUpdatesForHost(host);
            patchingStatus = versionIsFound
                ? (requiredUpdates.Length > 0 ? Messages.NOT_UPDATED : Messages.UPDATED)
                : String.Empty;

            return String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",
                                 pool.EscapeQuotes(),
                                 host.Name().EscapeQuotes(),
                                 host.ProductVersionTextShort().EscapeQuotes(),
                                 patchingStatus.EscapeQuotes(),
                                 requiredUpdates.EscapeQuotes(),
                                 installedUpdates.EscapeQuotes());
        }

        #endregion


        private void informationLabel_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(InvisibleMessages.UPSELL_SA);
            }
            catch (Exception)
            {
                using (var dlg = new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                       SystemIcons.Error,
                       string.Format(Messages.LICENSE_SERVER_COULD_NOT_OPEN_LINK, InvisibleMessages.LICENSE_SERVER_DOWNLOAD_LINK),
                       Messages.XENCENTER)))
                {
                    dlg.ShowDialog(this);
                }
            }
        }

        private void sortUpdates(List<Alert> updatesList)
        {
            if (dataGridViewUpdates.SortedColumn != null)
            {
                if (dataGridViewUpdates.SortedColumn.Index == ColumnMessage.Index)
                    updatesList.Sort(Alert.CompareOnTitle);
                else if (dataGridViewUpdates.SortedColumn.Index == ColumnDate.Index)
                    updatesList.Sort(Alert.CompareOnDate);
                else if (dataGridViewUpdates.SortedColumn.Index == ColumnLocation.Index)
                    updatesList.Sort(Alert.CompareOnAppliesTo);

                if (dataGridViewUpdates.SortOrder == SortOrder.Descending)
                    updatesList.Reverse();
            }
            else
            {
                updatesList.Sort(new NewVersionPriorityAlertComparer());
            }        
        }
        
        private void checkForUpdatesNowButton_Click(object sender, EventArgs e)
        {
            Updates.CheckForUpdates(true);
        }

        private void toolStripButtonRestoreDismissed_Click(object sender, EventArgs e)
        {
            Updates.RestoreDismissedUpdates();
        }

        private void checkForUpdatesNowLink_Click(object sender, EventArgs e)
        {
            Updates.CheckForUpdates(true);
        }

        private void tableLayoutPanel3_Resize(object sender, EventArgs e)
        {
            labelProgress.MaximumSize = new Size(tableLayoutPanel3.Width - 60, tableLayoutPanel3.Size.Height);
        }

        private void byUpdateToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (byUpdateToolStripMenuItem.Checked)
            {
                byHostToolStripMenuItem.Checked = false;
                ToggleView();
            }
        }

        private void byHostToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (byHostToolStripMenuItem.Checked)
            {
                byUpdateToolStripMenuItem.Checked = false;
                ToggleView();
            }
        }

        private void ToggleView()
        {
            //store the view
            Properties.Settings.Default.ShowUpdatesByServer = byHostToolStripMenuItem.Checked;

            // buttons
            toolStripDropDownButtonDateFilter.Visible = byUpdateToolStripMenuItem.Checked;
            toolStripSplitButtonDismiss.Visible = byUpdateToolStripMenuItem.Checked;
            toolStripButtonRestoreDismissed.Visible = byUpdateToolStripMenuItem.Checked;
            toolStripButtonUpdate.Visible = byHostToolStripMenuItem.Checked;

            // Switch the grid view
            dataGridViewUpdates.Visible = byUpdateToolStripMenuItem.Checked;
            dataGridViewHosts.Visible = byHostToolStripMenuItem.Checked;

            // Turn off Date Filter for the updates-by-server view
            if (byHostToolStripMenuItem.Checked)
                toolStripDropDownButtonDateFilter.ResetFilterDates();

            Rebuild();
        }

        private void toolStripButtonUpdate_Click(object sender, EventArgs e)
        {
            PatchingWizard wizard = (PatchingWizard)Program.MainWindow.ShowForm(typeof(PatchingWizard));
            if (wizard.IsFirstPage())
                wizard.NextStep();
        }
    }
}
