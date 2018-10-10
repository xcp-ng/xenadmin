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
using XenAdmin.Alerts;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAdmin.Wizards.NewPolicyWizard;
using XenAPI;


namespace XenAdmin.Dialogs.ScheduledSnapshots
{
    public partial class ScheduledSnapshotsDialog: XenDialogBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Pool Pool;
        private bool updatingPolicies;

        public ScheduledSnapshotsDialog(Pool pool)
            : base(pool.Connection)
        {
            Pool = pool;
            InitializeComponent();      
            chevronButton1.Text = Messages.SHOW_RUN_HISTORY;
            chevronButton1.Image = Properties.Resources.PDChevronDown;

            ColumnExpand.DefaultCellStyle.NullValue = null;
            comboBoxTimeSpan.SelectedIndex = 0;
            dataGridViewRunHistory.Columns[2].ValueType = typeof(DateTime);
            dataGridViewRunHistory.Columns[2].DefaultCellStyle.Format = Messages.DATEFORMAT_DMY_HM;
            panelHistory.Visible = false;
            RefreshPoolTitle(pool);
            RefreshButtons();
        }

        public ScheduledSnapshotsDialog() { }

        private PolicyRow SelectedVmssRow
        {
            get
            {
                if (dataGridViewPolicies.SelectedRows.Count > 0)
                    return dataGridViewPolicies.SelectedRows[0] as PolicyRow;

                return null;
            }
        }

        private int RunHistoryTimeSpan
        {
            get
            {
                switch (comboBoxTimeSpan.SelectedIndex)
                {
                    case 0: //top 10 messages (default)
                        return 0;
                    case 1:
                        return 24; //messages from past 24 Hrs
                    case 2:
                        return 7 * 24; //messages from last 7 days
                    default:
                        return 0;
                }
            }
        }

        private class PolicyRow : DataGridViewRow
        {
            private DataGridViewTextBoxCell _name = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _numVMs = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _nextRunTime = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _status = new DataGridViewTextBoxCell();
            private DataGridViewTextAndImageCell _lastResult = new DataGridViewTextAndImageCell();

            private readonly DateTime? _serverLocalTime;
            private readonly VMSS _policy;
            private readonly List<XenAPI.Message> _alertMessages;

            public bool IsBusy { get; set; }

            public VMSS Policy { get { return _policy; } }

            public List<XenAPI.Message> AlertMessages { get { return _alertMessages; } }

            public string PolicyName { get; private set; }
            public string PolicyStatus { get; private set; }
            public int PolicyVmCount { get; private set; }
            public DateTime? PolicyNextRunTime { get; private set; }
            public string PolicyLastResult { get; private set; }
            private Bitmap PolicyLastResultImage { get; set; }

            public PolicyRow(VMSS policy, List<XenAPI.Message> alertMessages, DateTime? serverLocalTime)
            {
                Cells.AddRange(_name, _status, _numVMs, _nextRunTime, _lastResult);
                _policy = policy;
                _alertMessages = alertMessages;
                _serverLocalTime = serverLocalTime;
                RefreshRow();
            }

            private void RefreshRow()
            {
                PolicyName = _policy.Name();
                PolicyVmCount = _policy.VMs.FindAll(vm => _policy.Connection.Resolve(vm).is_a_real_vm()).Count;
                PolicyStatus = _policy.enabled ? Messages.ENABLED : Messages.DISABLED;

                if (_serverLocalTime.HasValue)
                    PolicyNextRunTime = _policy.GetNextRunTime(_serverLocalTime.Value);
                else
                    PolicyNextRunTime = null;

                if (_alertMessages.Count > 0)
                {
                    if (_alertMessages[0].priority == PolicyAlert.INFO_PRIORITY)
                    {
                        PolicyLastResult = Messages.VMSS_SUCCEEDED;
                        PolicyLastResultImage = Resources._075_TickRound_h32bit_16;
                    }
                    else
                    {
                        PolicyLastResult = Messages.FAILED;
                        PolicyLastResultImage = Resources._075_WarningRound_h32bit_16;
                    }
                }
                else
                {
                    PolicyLastResult = Messages.NOT_YET_RUN;
                    PolicyLastResultImage = null;
                }

                _name.Value = PolicyName;
                _numVMs.Value = PolicyVmCount;
                _status.Value = PolicyStatus;
                _lastResult.Value = PolicyLastResult;
                _lastResult.Image = PolicyLastResultImage;
                _nextRunTime.Value = PolicyNextRunTime.HasValue
                    ? HelpersGUI.DateTimeToString(PolicyNextRunTime.Value, Messages.DATEFORMAT_DMY_HM, true)
                    : Messages.VMSS_HOST_NOT_LIVE;
            }
        }

        private void RefreshPoolTitle(Pool pool)
        {
            int protectedVMs = 0;
            int realVMs = 0;

            foreach (var vm in pool.Connection.Cache.VMs)
            {
                if (vm.is_a_real_vm() && vm.Show(Properties.Settings.Default.ShowHiddenVMs))
                {
                    realVMs++;
                    if (vm.Connection.Resolve(vm.snapshot_schedule) != null)
                        protectedVMs++;
                }
            }

            labelPolicyTitle.Text = string.Format(Helpers.IsPool(pool.Connection)
                                                        ? Messages.VMSS_SCHEDULED_SNAPSHOTS_DEFINED_FOR_POOL
                                                        : Messages.VMSS_SCHEDULED_SNAPSHOTS_DEFINED_FOR_SERVER,
                                                    pool.Name().Ellipsise(45), protectedVMs, realVMs);
        }

        private void VMSSCollectionChanged(object sender, EventArgs e)
        {
            LoadPolicies();
        }

        private void LoadPolicies()
        {
            var action = new LoadVmssAction(Pool.Connection);
            action.Completed += action_Completed;
            action.RunAsync();
        }

        private void action_Completed(ActionBase sender)
        {
            var action = sender as LoadVmssAction;
            if (action == null || !action.Succeeded)
                return;

            Program.Invoke(Program.MainWindow, () =>
            {
                try
                {
                    panelLoading.Visible = false;
                    updatingPolicies = true;

                    var selectedPolicyUuids = (from PolicyRow row in dataGridViewPolicies.SelectedRows
                        select row.Policy.uuid).ToList();

                    var rowList = from kvp in action.SnapshotSchedules select new PolicyRow(kvp.Key, kvp.Value, action.ServerLocalTime);

                    Func<PolicyRow, object> comparer = p => p.PolicyName;
                    if (dataGridViewPolicies.SortedColumn != null)
                    {
                        if (dataGridViewPolicies.SortedColumn.Index == NameColum.Index)
                            comparer = p => p.PolicyName;
                        else if (dataGridViewPolicies.SortedColumn.Index == EnabledColumn.Index)
                            comparer = p => p.PolicyStatus;
                        else if (dataGridViewPolicies.SortedColumn.Index == ColumnVMs.Index)
                            comparer = p => p.PolicyVmCount;
                        else if (dataGridViewPolicies.SortedColumn.Index == DescriptionColum.Index)
                            comparer = p => p.PolicyNextRunTime;
                        else if (dataGridViewPolicies.SortedColumn.Index == ColumnLastResult.Index)
                            comparer = p => p.PolicyLastResult;
                    }

                    var rows = dataGridViewPolicies.SortOrder == SortOrder.Descending
                        ? rowList.OrderByDescending(comparer) : rowList.OrderBy(comparer);

                    dataGridViewPolicies.SuspendLayout();
                    dataGridViewPolicies.Rows.Clear();
                    dataGridViewPolicies.Rows.AddRange(rows.Cast<DataGridViewRow>().ToArray());

                    foreach (PolicyRow row in dataGridViewPolicies.Rows)
                        row.Selected = selectedPolicyUuids.Contains(row.Policy.uuid);

                    if (dataGridViewPolicies.SelectedRows.Count == 0 && dataGridViewPolicies.Rows.Count > 0)
                        dataGridViewPolicies.Rows[0].Selected = true;
                }
                finally
                {
                    dataGridViewPolicies.ResumeLayout();
                    updatingPolicies = false;

                    if (action.ServerLocalTime.HasValue)
                    {
                        string time= HelpersGUI.DateTimeToString(action.ServerLocalTime.Value, Messages.DATEFORMAT_WDMY_HM_LONG, true);
                        labelServerTime.Text = string.Format(Messages.SERVER_TIME, time);
                    }

                    RefreshPoolTitle(Pool);
                    RefreshButtons();
                    RefreshHistoryLabel();
                    RefreshHistoryGrid();
                }
            });
        }

        private void VMProtectionPoliciesDialog_Load(object sender, EventArgs e)
        {
            panelLoading.Visible = true;
            Pool.Connection.Cache.RegisterBatchCollectionChanged<VMSS>(VMSSCollectionChanged);
            LoadPolicies();
        }

        private void VMProtectionPoliciesDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Pool.Connection.Cache.DeregisterBatchCollectionChanged<VMSS>(VMSSCollectionChanged);
        }

        private void dataGridViewPolicies_SelectionChanged(object sender, EventArgs e)
        {
            if (updatingPolicies)
                return;

            RefreshButtons();
            RefreshHistoryLabel();
            RefreshHistoryGrid();
        }

        private void RefreshButtons()
        {
            if (dataGridViewPolicies.SelectedRows.Count == 1)
            {
                var row = SelectedVmssRow;
                buttonEnable.Text = row.Policy.enabled ? Messages.DISABLE : Messages.ENABLE;
                buttonEnable.Enabled = !row.IsBusy && (row.Policy.VMs.Count != 0 || row.Policy.enabled);
                buttonProperties.Enabled = !row.IsBusy;
                buttonRunNow.Enabled = !row.IsBusy && row.Policy.enabled;
                comboBoxTimeSpan.Enabled = !row.IsBusy;
            }
            else
            {
                buttonProperties.Enabled = buttonEnable.Enabled = buttonRunNow.Enabled =
                    comboBoxTimeSpan.Enabled = false;
            }

            buttonDelete.Enabled = (from PolicyRow row in dataGridViewPolicies.SelectedRows where !row.IsBusy select row).Any();
        }

        #region Button event handlers

        private void buttonNew_Click(object sender, System.EventArgs e)
        {
            new NewPolicyWizard(Pool).Show(this);
        }

        private void buttonCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void buttonEnable_Click(object sender, EventArgs e)
        {
            var row = SelectedVmssRow;
            if (row != null)
            {
                row.IsBusy = true;
                RefreshButtons();
                new ChangePolicyEnabledAction(row.Policy).RunAsync();
            }
        }

        private void buttonRunNow_Click(object sender, EventArgs e)
        {
            var row = SelectedVmssRow;
            if (row != null)
            {
                row.IsBusy = true;
                RefreshButtons();
                new RunPolicyNowAction(row.Policy).RunAsync();
            }
        }

        private void buttonProperties_Click(object sender, EventArgs e)
        {
            var row = SelectedVmssRow;
            if (row != null)
            {
                using (PropertiesDialog propertiesDialog = new PropertiesDialog(row.Policy))
                    propertiesDialog.ShowDialog(this);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            var selectedPolicies = new List<VMSS>();
            int numberOfProtectedVMs = 0;

            foreach (PolicyRow row in dataGridViewPolicies.SelectedRows)
            {
                selectedPolicies.Add(row.Policy);
                numberOfProtectedVMs += row.Policy.VMs.Count;
            }

            string text = selectedPolicies.Count == 1
                ? String.Format(numberOfProtectedVMs == 0
                    ? Messages.CONFIRM_DELETE_POLICY_0
                    : Messages.CONFIRM_DELETE_POLICY, selectedPolicies[0].Name(), numberOfProtectedVMs)
                : string.Format(numberOfProtectedVMs == 0
                    ? Messages.CONFIRM_DELETE_POLICIES_0
                    : Messages.CONFIRM_DELETE_POLICIES, numberOfProtectedVMs);

            using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, text, Messages.DELETE_VMSS_TITLE),
                    ThreeButtonDialog.ButtonYes,
                    ThreeButtonDialog.ButtonNo))
            {
                if (dlg.ShowDialog(this) == DialogResult.Yes)
                {
                    foreach (PolicyRow row in dataGridViewPolicies.SelectedRows)
                        row.IsBusy = true;

                    RefreshButtons();
                    new DestroyPolicyAction(Pool.Connection, selectedPolicies).RunAsync();
                }
            }
        }

        private void chevronButton1_ButtonClick(object sender, EventArgs e)
        {
            if (chevronButton1.Text == Messages.HIDE_RUN_HISTORY)
            {
                chevronButton1.Text = Messages.SHOW_RUN_HISTORY;
                chevronButton1.Image = Properties.Resources.PDChevronDown;
                panelHistory.Visible = false;
            }
            else
            {
                chevronButton1.Text = Messages.HIDE_RUN_HISTORY;
                chevronButton1.Image = Properties.Resources.PDChevronUp;
                panelHistory.Visible = true;
            }
        }

        private void chevronButton1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                chevronButton1_ButtonClick(sender, e);
        }

        #endregion

        internal override string HelpName
        {
            get
            {
                return "VMSnapshotSchedulesDialog";
            }
        }

        private void dataGridViewRunHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                HistoryRow row = (HistoryRow)dataGridViewRunHistory.Rows[e.RowIndex];
                if (row.Alert.Type != "info")
                {
                    row.Expanded = !row.Expanded;
                    row.RefreshRow();
                }
            }
        }

        private void comboBoxTimeSpan_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshHistoryGrid();
        }

        private void RefreshHistoryGrid()
        {
            try
            {
                dataGridViewRunHistory.SuspendLayout();
                dataGridViewRunHistory.Rows.Clear();

                var row = SelectedVmssRow;
                if (row == null)
                    return;

                var vmss = row.Policy;
                var messages = row.AlertMessages;

                int hoursFromNow = RunHistoryTimeSpan;

                DateTime currentTime = DateTime.Now;
                DateTime offset = currentTime.Add(new TimeSpan(-hoursFromNow, 0, 0));

                if (hoursFromNow == 0)
                {
                    for (int i = 0; i < 10 && i < messages.Count; i++)
                    {
                        var msg = messages[i];
                        var alert = new PolicyAlert(msg.priority, msg.name, msg.TimestampLocal(), msg.body, vmss.Name());
                        dataGridViewRunHistory.Rows.Add(new HistoryRow(alert));
                    }
                }
                else
                {
                    foreach (var msg in messages)
                    {
                        if (msg.TimestampLocal() >= offset)
                        {
                            var alert = new PolicyAlert(msg.priority, msg.name, msg.TimestampLocal(), msg.body, vmss.Name());
                            dataGridViewRunHistory.Rows.Add(new HistoryRow(alert));
                        }
                        else
                            break;
                    }
                }
            }
            finally
            {
                dataGridViewRunHistory.ResumeLayout();
            }
        }

        private void RefreshHistoryLabel()
        {
            var row = SelectedVmssRow;
            if (row == null)
            {
                labelHistory.Text = "";
                return;
            }

            string name = row.Policy.Name();

            // ellipsise if necessary
            using (Graphics g = labelHistory.CreateGraphics())
            {
                int maxWidth = labelShow.Left - labelHistory.Left;
                int availableWidth = maxWidth - (int)g.MeasureString(string.Format(Messages.HISTORY_FOR_POLICY, ""), labelHistory.Font).Width;
                name = name.Ellipsise(new Rectangle(0, 0, availableWidth, labelHistory.Height), labelHistory.Font);
            }
            labelHistory.Text = string.Format(Messages.HISTORY_FOR_POLICY, name);
        }

        private class HistoryRow : DataGridViewRow
        {
            private DataGridViewImageCell _expand = new DataGridViewImageCell();
            private DataGridViewTextAndImageCell _result = new DataGridViewTextAndImageCell();
            private DataGridViewTextBoxCell _dateTime = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _description = new DataGridViewTextBoxCell();
            public readonly PolicyAlert Alert;

            public HistoryRow(PolicyAlert alert)
            {
                Alert = alert;
                Cells.AddRange(_expand, _result, _dateTime, _description);
                RefreshRow();
            }

            [DefaultValue(false)]
            public bool Expanded { get; set; }

            public void RefreshRow()
            {
                _expand.Value = Expanded ? Resources.expanded_triangle : Resources.contracted_triangle;
                if (Alert.Type == "info")
                    _expand.Value = null;

                if (Alert.Type == "error")
                {
                    _result.Image = Properties.Resources._075_WarningRound_h32bit_16;
                    _result.Value = Messages.ERROR;
                }
                else if (Alert.Type == "warn")
                {
                    _result.Image = Properties.Resources._075_WarningRound_h32bit_16;
                    _result.Value = Messages.WARNING;
                }
                else if (Alert.Type == "info")
                {
                    _result.Image = Properties.Resources._075_TickRound_h32bit_16;
                    _result.Value = Messages.INFORMATION;
                }
                _dateTime.Value = Alert.Time;
                if (Alert.Type == "error")
                    _description.Value = Expanded ? string.Format("{0}\r\n{1}", Alert.ShortFormatBody, Alert.Text) : Alert.ShortFormatBody.Ellipsise(80);
                else
                    _description.Value = Expanded ? Alert.Text : Alert.ShortFormatBody.Ellipsise(90);
            }
        }
    }
}
