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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Controls;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Dialogs.HealthCheck
{
    public partial class HealthCheckOverviewDialog : XenDialogBase
    {

        internal override string HelpName { get { return "HealthCheckOverviewDialog"; } }

        public HealthCheckOverviewDialog()
        {
            InitializeComponent();
            Core.HealthCheck.CheckForAnalysisResultsCompleted += HealthCheck_CheckForUpdatesCompleted;
        }

        private Pool currentSelected = null;

        private void LoadPools()
        {
            Program.AssertOnEventThread();

            var selectedPool = currentSelected;
            poolsDataGridView.SuspendLayout();
            try
            {
                poolsDataGridView.Rows.Clear();

                foreach (IXenConnection xenConnection in ConnectionsManager.XenConnectionsCopy)
                {
                    if (!xenConnection.IsConnected || Helpers.FeatureForbidden(xenConnection, Host.RestrictHealthCheck))
                        continue;
                    var pool = Helpers.GetPoolOfOne(xenConnection);
                    if (pool != null && poolsDataGridView.ColumnCount > 0)
                    {
                        poolsDataGridView.Rows.Add(new PoolRow(pool));
                    }
                }
                RefreshButtons();
                RefreshDetailsPanel();
                if (selectedPool != null)
                {
                    foreach (DataGridViewRow row in poolsDataGridView.Rows)
                    {
                        if (row is PoolRow &&
                            (row as PoolRow).Pool.uuid == selectedPool.uuid)
                        {
                            poolsDataGridView.ClearSelection();
                            row.Selected = true;
                            break;
                        }
                    }
                }
            }
            finally
            {
                poolsDataGridView.ResumeLayout();
            }
        }

        void Pool_BatchCollectionChanged(object sender, EventArgs e)
        {
            Program.Invoke(Program.MainWindow, LoadPools);
        }
        
        #region PoolRow
        private class PoolRow : DataGridViewRow
        {
            private DataGridViewTextAndImageCell _nameCell = new DataGridViewTextAndImageCell();
            private DataGridViewTextAndImageCell _statusCell = new DataGridViewTextAndImageCell();
            public readonly Pool Pool;
            public PoolRow(Pool pool)
            {
                Cells.Add(_nameCell);
                Cells.Add(_statusCell);
                Pool = pool;
                RefreshRow();
            }

            public void RefreshRow()
            {
                _nameCell.Value = Pool.Name();
                _nameCell.Image = null;
                
                var healthCheckSettings = Pool.HealthCheckSettings();
                _statusCell.Value = healthCheckSettings.StatusDescription;
                _statusCell.Image = healthCheckSettings.Status != HealthCheckStatus.Enabled || !healthCheckSettings.HasAnalysisResult
                    ? null
                    : GetSeverityImage(healthCheckSettings);
            }

            private Image GetSeverityImage(HealthCheckSettings healthCheckSettings)
            {
                if (healthCheckSettings.ReportAnalysisIssuesDetected == 0)
                    return Properties.Resources._000_Tick_h32bit_16;
                switch (healthCheckSettings.ReportAnalysisSeverity)
                {
                    case DiagnosticAlertSeverity.Error:
                        return Properties.Resources._000_error_h32bit_16;
                    case DiagnosticAlertSeverity.Warning:
                        return Properties.Resources._000_Alert2_h32bit_16;
                    default:
                        return Properties.Resources._000_Info3_h32bit_16;
                }
            }
        }
        #endregion
        
        
        private void RefreshButtons()
        {
            if (poolsDataGridView.SelectedRows.Count == 1 && poolsDataGridView.SelectedRows[0] is PoolRow)
            {
                currentSelected = (Pool)((PoolRow)poolsDataGridView.SelectedRows[0]).Pool;
            }
            else
            {
                if (poolsDataGridView.SelectedRows.Count == 0)
                    currentSelected = null;
            }

            poolDetailsPanel.Visible = (currentSelected != null);
        }

        private void RefreshDetailsPanel()
        {
            if (poolsDataGridView.SelectedRows.Count != 1 || !(poolsDataGridView.SelectedRows[0] is PoolRow))
            {
                poolNameLabel.Text = "";
                return;
            }

            var poolRow = (PoolRow)poolsDataGridView.SelectedRows[0];
            var healthcheckSettings = poolRow.Pool.HealthCheckSettings();
            poolNameLabel.Text = poolRow.Pool.Name().Ellipsise(120);
            scheduleLabel.Text = GetScheduleDescription(healthcheckSettings);
            lastUploadLabel.Visible = lastUploadDateLabel.Visible = !string.IsNullOrEmpty(healthcheckSettings.LastSuccessfulUpload);
            lastUploadDateLabel.Text = GetLastUploadDescription(healthcheckSettings);

            // show the "Last failed upload" if we have a failed upload AND 
            // there is no successful upload or the failed upload happened after a successful upload
            var lastFailedUploadTime = healthcheckSettings.LastFailedUploadTime;
            bool showFailedUpload = lastFailedUploadTime > DateTime.MinValue 
                && (string.IsNullOrEmpty(healthcheckSettings.LastSuccessfulUpload) || lastFailedUploadTime > healthcheckSettings.LastSuccessfulUploadTime);
            failedUploadLabel.Visible = failedUploadDateLabel.Visible = showFailedUpload;
            if (showFailedUpload)
                failedUploadDateLabel.Text = HelpersGUI.DateTimeToString(lastFailedUploadTime.ToLocalTime(), Messages.DATEFORMAT_DMY_HM, true);

            UpdateButtonsVisibility(poolRow.Pool);

            healthCheckStatusPanel.Visible = healthcheckSettings.Status == HealthCheckStatus.Enabled;
            notEnrolledPanel.Visible = healthcheckSettings.Status != HealthCheckStatus.Enabled;
            UpdateUploadRequestDescription(healthcheckSettings);

            UpdateAnalysisResult(healthcheckSettings);
        }

        public string GetScheduleDescription(HealthCheckSettings healthCheckSettings)
        {
            {
                var time = new DateTime(1900, 1, 1, healthCheckSettings.TimeOfDay, 0, 0);
                return healthCheckSettings.Status == HealthCheckStatus.Enabled
                    ? string.Format(Messages.HEALTHCHECK_SCHEDULE_DESCRIPTION, healthCheckSettings.IntervalInWeeks,
                                    HelpersGUI.DayOfWeekToString(healthCheckSettings.DayOfWeek, true),
                                    HelpersGUI.DateTimeToString(time, Messages.DATEFORMAT_HM, true))
                    : string.Empty;
            }
        }

        public void UpdateUploadRequestDescription(HealthCheckSettings healthCheckSettings)
        {
            {
                if (!healthCheckSettings.CanRequestNewUpload)
                {
                        uploadRequestLinkLabel.Text = string.Format(Messages.HEALTHCHECK_ON_DEMAND_REQUESTED_AT,
                                                                    HelpersGUI.DateTimeToString(healthCheckSettings.NewUploadRequestTime.ToLocalTime(), 
                                                                        Messages.DATEFORMAT_HM, true));
                    uploadRequestLinkLabel.LinkArea = new LinkArea(0, 0);
                    return;
                }
                uploadRequestLinkLabel.Text = Messages.HEALTHCHECK_ON_DEMAND_REQUEST;
                uploadRequestLinkLabel.LinkArea = new LinkArea(0, uploadRequestLinkLabel.Text.Length);
            }
        }

        public string GetLastUploadDescription(HealthCheckSettings healthCheckSettings)
        {
            if (!string.IsNullOrEmpty(healthCheckSettings.LastSuccessfulUpload))
            {
                DateTime lastSuccessfulUpload;
                if (HealthCheckSettings.TryParseStringToDateTime(healthCheckSettings.LastSuccessfulUpload, out lastSuccessfulUpload))
                {
                    return HelpersGUI.DateTimeToString(lastSuccessfulUpload.ToLocalTime(), Messages.DATEFORMAT_DMY_HM, true);
                }
            }
            return string.Empty;
        }

        public void UpdateAnalysisResult(HealthCheckSettings healthCheckSettings)
        {
            issuesLabel.Text = healthCheckSettings.StatusDescription;
            ReportAnalysisLinkLabel.Visible = healthCheckSettings.HasAnalysisResult;

            if (healthCheckSettings.HasAnalysisResult)
                switch (healthCheckSettings.ReportAnalysisSeverity)
                {
                    case DiagnosticAlertSeverity.Error:
                        issuesLabel.ForeColor = Color.Red;
                        break;
                    case DiagnosticAlertSeverity.Warning:
                        issuesLabel.ForeColor = Color.OrangeRed;
                        break;
                    default:
                        issuesLabel.ForeColor = 
                            healthCheckSettings.ReportAnalysisIssuesDetected > 0 ? SystemColors.ControlText : Color.Green;
                        break;
                }
            else
            {
                issuesLabel.ForeColor = SystemColors.ControlText;
            }

            refreshLinkLabel.Visible = healthCheckSettings.HasUpload && !healthCheckSettings.HasAnalysisResult;

            if (healthCheckSettings.HasOldAnalysisResult)
            {
                previousUploadPanel.Visible = healthCheckSettings.HasOldAnalysisResult;

                DateTime previousUpload;
                if (HealthCheckSettings.TryParseStringToDateTime(healthCheckSettings.ReportAnalysisUploadTime,
                    out previousUpload))
                {
                    previousUploadDateLabel.Text = HelpersGUI.DateTimeToString(previousUpload.ToLocalTime(),
                        Messages.DATEFORMAT_DMY_HM, true);
                }
            }
            else
            {
                previousUploadPanel.Visible = false;
            }
        }

        public void UpdateButtonsVisibility(Pool pool)
        {
            refreshLinkLabel.Visible =
                editLinkLabel.Visible =
                disableLinkLabel.Visible =
                uploadRequestLinkLabel.Visible =
                enrollNowLinkLabel.Visible = Core.HealthCheck.PassedRbacChecks(pool.Connection);

        }

        private void HealthCheckOverviewDialog_Load(object sender, EventArgs e)
        {
            LoadPools();
            foreach (IXenConnection xenConnection in ConnectionsManager.XenConnectionsCopy)
            {
                xenConnection.Cache.RegisterBatchCollectionChanged<Pool>(Pool_BatchCollectionChanged);
            }
            showAgainCheckBox.Checked = Properties.Settings.Default.ShowHealthCheckEnrollmentReminder;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            RefreshButtons();
            RefreshDetailsPanel();
        }

        private void HealthCheckOOverviewDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (IXenConnection xenConnection in ConnectionsManager.XenConnectionsCopy)
            {
                xenConnection.Cache.DeregisterBatchCollectionChanged<Pool>(Pool_BatchCollectionChanged);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        private void editlinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (poolsDataGridView.SelectedRows.Count != 1 || !(poolsDataGridView.SelectedRows[0] is PoolRow))
                return;

            var poolRow = (PoolRow)poolsDataGridView.SelectedRows[0];
            new HealthCheckSettingsDialog(poolRow.Pool, false).ShowDialog(this);
        }

        public DialogResult ShowDialog(IWin32Window parent, List<IXenObject> selectedItems)
        {
            SelectPool(selectedItems);
            return ShowDialog(parent);
        }

        public void RefreshView(List<IXenObject> selectedItems)
        {
            SelectPool(selectedItems);
            LoadPools();
        }

        private void SelectPool(List<IXenObject> selectedItems)
        {
            IXenObject xo = selectedItems.Count > 0 ? selectedItems.FirstOrDefault() : null;
            if (xo is Pool)
                currentSelected = xo as Pool;
        }

        private void enrollNowLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (poolsDataGridView.SelectedRows.Count != 1 || !(poolsDataGridView.SelectedRows[0] is PoolRow))
                return;

            var poolRow = (PoolRow)poolsDataGridView.SelectedRows[0];
            new HealthCheckSettingsDialog(poolRow.Pool, true).ShowDialog(this);
        }

        private void uploadRequestLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (poolsDataGridView.SelectedRows.Count != 1 || !(poolsDataGridView.SelectedRows[0] is PoolRow))
                return;

            var poolRow = (PoolRow)poolsDataGridView.SelectedRows[0];
            var healthCheckSettings = poolRow.Pool.HealthCheckSettings();
            if (healthCheckSettings.CanRequestNewUpload)
            {
                healthCheckSettings.NewUploadRequest = HealthCheckSettings.DateTimeToString(DateTime.UtcNow);
                var token = healthCheckSettings.GetSecretyInfo(poolRow.Pool.Connection, HealthCheckSettings.UPLOAD_TOKEN_SECRET);
                var diagnosticToken = healthCheckSettings.GetSecretyInfo(poolRow.Pool.Connection, HealthCheckSettings.DIAGNOSTIC_TOKEN_SECRET);
                var user = healthCheckSettings.GetSecretyInfo(poolRow.Pool.Connection, HealthCheckSettings.UPLOAD_CREDENTIAL_USER_SECRET);
                var password = healthCheckSettings.GetSecretyInfo(poolRow.Pool.Connection, HealthCheckSettings.UPLOAD_CREDENTIAL_PASSWORD_SECRET);
                new SaveHealthCheckSettingsAction(poolRow.Pool, healthCheckSettings, token, diagnosticToken, user, password, false).RunAsync();
            }
        }

        private void showAgainCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.ShowHealthCheckEnrollmentReminder != showAgainCheckBox.Checked)
            {
                Properties.Settings.Default.ShowHealthCheckEnrollmentReminder = showAgainCheckBox.Checked;
                Settings.TrySaveSettings();
            }
        }

        private void disableLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (poolsDataGridView.SelectedRows.Count != 1 || !(poolsDataGridView.SelectedRows[0] is PoolRow))
                return;

            var poolRow = (PoolRow)poolsDataGridView.SelectedRows[0];
            if (poolRow.Pool == null)
                return;
            var healthCheckSettings = poolRow.Pool.HealthCheckSettings();
            if (healthCheckSettings.Status == HealthCheckStatus.Enabled)
            {
                string msg = Helpers.GetPool(poolRow.Pool.Connection) == null 
                    ? Messages.CONFIRM_DISABLE_HEALTH_CHECK_SERVER 
                    : Messages.CONFIRM_DISABLE_HEALTH_CHECK_POOL;
                using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(null, msg, Messages.XENCENTER), 
                    ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo))
                {
                    if (dlg.ShowDialog(this) != DialogResult.Yes)
                        return;
                }
                healthCheckSettings.Status = HealthCheckStatus.Disabled;
                new SaveHealthCheckSettingsAction(poolRow.Pool, healthCheckSettings, null, null, null, null, false).RunAsync();
                
            }
        }

        private void ReportAnalysisLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (poolsDataGridView.SelectedRows.Count != 1 || !(poolsDataGridView.SelectedRows[0] is PoolRow))
                return;

            var poolRow = (PoolRow)poolsDataGridView.SelectedRows[0];
            if (poolRow.Pool == null)
                return;
            var url = poolRow.Pool.HealthCheckSettings().GetReportAnalysisLink(Registry.HealthCheckDiagnosticDomainName);
            if (!string.IsNullOrEmpty(url))
                Program.OpenURL(url);
        }

        private void viewReportLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (poolsDataGridView.SelectedRows.Count != 1 || !(poolsDataGridView.SelectedRows[0] is PoolRow))
                return;

            var poolRow = (PoolRow)poolsDataGridView.SelectedRows[0];
            if (poolRow.Pool == null)
                return;
            var url = poolRow.Pool.HealthCheckSettings().GetReportAnalysisLink(Registry.HealthCheckDiagnosticDomainName);
            if (!string.IsNullOrEmpty(url))
                Program.OpenURL(url);
        }

        private void refreshLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var poolRow = (PoolRow)poolsDataGridView.SelectedRows[0];
            if (poolRow.Pool == null)
                return;
            
            Core.HealthCheck.CheckForAnalysisResults(poolRow.Pool.Connection, false);
        }

        private void HealthCheck_CheckForUpdatesCompleted(bool succeeded)
        {
            Program.Invoke(Program.MainWindow, LoadPools);
        }

        private void PolicyStatementLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new HealthCheckPolicyStatementDialog().ShowDialog(this);
        }
    }
}
