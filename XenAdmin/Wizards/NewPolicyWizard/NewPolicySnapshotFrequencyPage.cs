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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.SettingsPanels;
using XenAPI;


namespace XenAdmin.Wizards.NewPolicyWizard
{
    public partial class NewPolicySnapshotFrequencyPage : XenTabPage, IEditPage
    {
        private VMSS _policyCopy;
        private ServerTimeInfo? _serverTimeInfo;
        private bool updating;
        private readonly ToolTip InvalidParamToolTip;

        public NewPolicySnapshotFrequencyPage()
        {
            InitializeComponent();
            MainTableLayoutPanel.Visible = false;
            LoadingBox.Visible = true;

            InvalidParamToolTip = new ToolTip
            {
                IsBalloon = true,
                ToolTipIcon = ToolTipIcon.Warning,
                ToolTipTitle = Messages.INVALID_PARAMETER
            };

            try
            {
                updating = true;
                radioButtonDaily.Checked = true;
                numericUpDownRetention.Value = 7;
                SetHourlyMinutes(15);
                daysWeekCheckboxes.SelectedDays = new[] {DayOfWeek.Monday};
            }
            finally
            {
                updating = false;
            }
        }

        #region IVerticalTab implementation

        public override string Text => Messages.SNAPSHOT_FREQUENCY;

        public string SubText { get; private set; }

        public Image Image => Properties.Resources.notif_events_16;

        #endregion

        #region XenTabPage implementation

        public override string PageTitle => Messages.SNAPSHOT_FREQUENCY_TITLE;

        public override string HelpID => "Snapshotfrequency";

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction == PageLoadedDirection.Forward)
                GetServerTime();
        }

        public override bool EnableNext()
        {
            if (radioButtonHourly.Checked && comboBoxMin.SelectedIndex < 0)
                return false;
            if (radioButtonWeekly.Checked && !daysWeekCheckboxes.AnySelected())
                return false;
            return true;
        }

        #endregion

        public long BackupRetention {get; private set; }

        public Dictionary<string, string> Schedule { get; private set; }

        public vmss_frequency Frequency { get; private set; }

        public string FormattedSchedule { get; private set; }

        public void GetServerTime()
        {
            var master = Helpers.GetMaster(Connection);
            if (master == null)
                return;

            var action = new GetServerLocalTimeAction(master);
            action.Completed += action_CompletedTimeServer;
            MainTableLayoutPanel.Visible = false;
            LoadingBox.Visible = true;
            spinnerIcon1.StartSpinning();
            action.RunAsync();
        }

        private void action_CompletedTimeServer(ActionBase sender)
        {
            sender.Completed -= action_CompletedTimeServer;

            var action = sender as GetServerLocalTimeAction;
            if (action == null)
                return;

            Program.Invoke(ParentForm, () =>
            {
                spinnerIcon1.StopSpinning();
                LoadingBox.Visible = false;
                MainTableLayoutPanel.Visible = true;
                _serverTimeInfo = action.ServerTimeInfo;
                PopulateTab();
            });
        }

        private void PopulateTab()
        {
            try
            {
                updating = true;

                if (_policyCopy == null || !_serverTimeInfo.HasValue)
                    return;

                var nextRunOnServer = _policyCopy.GetNextRunTime(_serverTimeInfo.Value.ServerLocalTime);
                if (!nextRunOnServer.HasValue)
                    return;

                var nextRunOnClient = HelpersGUI.RoundToNearestQuarter(nextRunOnServer.Value + _serverTimeInfo.Value.ServerClientTimeZoneDiff);

                switch (_policyCopy.frequency)
                {
                    case vmss_frequency.hourly:
                        SetHourlyMinutes(nextRunOnClient.Minute);
                        radioButtonHourly.Checked = true;
                        break;
                    case vmss_frequency.daily:
                        dateTimePickerDaily.Value = new DateTime(1970, 1, 1, nextRunOnClient.Hour, nextRunOnClient.Minute, 0);
                        radioButtonDaily.Checked = true;
                        break;
                    case vmss_frequency.weekly:
                        dateTimePickerWeekly.Value = new DateTime(1970, 1, 1, nextRunOnClient.Hour, nextRunOnClient.Minute, 0);
                        daysWeekCheckboxes.SelectedDays = VMSS.BackUpScheduleDays(_policyCopy.schedule);
                        radioButtonWeekly.Checked = true;
                        break;
                }

                numericUpDownRetention.Value = _policyCopy.retained_snapshots;
            }
            finally
            {
                updating = false;
                RecalculateSchedule();
            }
        }

        private void SetHourlyMinutes(int min)
        {
            if (0 <= min && min < 15)
                comboBoxMin.SelectedIndex = 0;
            else if (15 <= min && min < 30)
                comboBoxMin.SelectedIndex = 1;
            else if (30 <= min && min <45)
                comboBoxMin.SelectedIndex = 2;
            else if (45 <= min && min < 60)
                comboBoxMin.SelectedIndex = 3;
            else
                throw new ArgumentException("min");
        }

        private void RecalculateSchedule()
        {
            if (updating)
                return;

            Schedule = new Dictionary<string, string>();
            DateTime? nextRunOnClient = null;
            DateTime? nextRunOnServer = null;

            if (radioButtonHourly.Checked && int.TryParse(comboBoxMin.SelectedItem.ToString(), out int min))
            {
                SubText = FormattedSchedule = string.Format(Messages.HOURLY_SCHEDULE_FORMAT, min);
                nextRunOnClient = VMSS.GetHourlyDate(DateTime.Now, min);

                if (_serverTimeInfo.HasValue)
                {
                    nextRunOnServer = HelpersGUI.RoundToNearestQuarter(nextRunOnClient.Value - _serverTimeInfo.Value.ServerClientTimeZoneDiff);
                    Schedule["min"] = nextRunOnServer.Value.Minute.ToString();
                }
            }
            else if (radioButtonDaily.Checked)
            {
                SubText = FormattedSchedule = string.Format(Messages.DAILY_SCHEDULE_FORMAT,
                    HelpersGUI.DateTimeToString(dateTimePickerDaily.Value, Messages.DATEFORMAT_HM, true));
                nextRunOnClient = VMSS.GetDailyDate(DateTime.Now, dateTimePickerDaily.Value.Minute, dateTimePickerDaily.Value.Hour);

                if (_serverTimeInfo.HasValue)
                {
                    nextRunOnServer = HelpersGUI.RoundToNearestQuarter(nextRunOnClient.Value - _serverTimeInfo.Value.ServerClientTimeZoneDiff);
                    Schedule["hour"] = nextRunOnServer.Value.Hour.ToString();
                    Schedule["min"] = nextRunOnServer.Value.Minute.ToString();
                }
            }
            else if (radioButtonWeekly.Checked && daysWeekCheckboxes.AnySelected())
            {
                var days = daysWeekCheckboxes.SelectedDays;
                var longString = string.Join(", ", days.Select(d => HelpersGUI.DayOfWeekToString(d)));
                var shortString = string.Join(", ", days.Select(d => HelpersGUI.DayOfWeekToShortString(d)));

                FormattedSchedule = string.Format(Messages.WEEKLY_SCHEDULE_FORMAT,
                    HelpersGUI.DateTimeToString(dateTimePickerWeekly.Value, Messages.DATEFORMAT_HM, true), longString);
                SubText = string.Format(Messages.WEEKLY_SCHEDULE_FORMAT,
                    HelpersGUI.DateTimeToString(dateTimePickerWeekly.Value, Messages.DATEFORMAT_HM, true), shortString);

                var nextClientRuns = VMSS.GetWeeklyDates(DateTime.Now, dateTimePickerWeekly.Value.Minute,
                    dateTimePickerWeekly.Value.Hour, days);

                if (_serverTimeInfo.HasValue && nextClientRuns.Count > 0)
                {
                    nextRunOnClient = nextClientRuns[0];
                    var nextServerRuns = nextClientRuns.Select(n =>
                        HelpersGUI.RoundToNearestQuarter(n - _serverTimeInfo.Value.ServerClientTimeZoneDiff)).ToList();
                    nextRunOnServer = nextServerRuns[0];

                    Schedule["hour"] = nextRunOnServer.Value.Hour.ToString();
                    Schedule["min"] = nextRunOnServer.Value.Minute.ToString();
                    Schedule["days"] = string.Join(",", nextServerRuns.Select(n => n.DayOfWeek.ToString())).ToLower();
                }
            }

            if (string.IsNullOrEmpty(FormattedSchedule))
                FormattedSchedule = Messages.UNKNOWN;
            if (string.IsNullOrEmpty(SubText))
                SubText = Messages.UNKNOWN;

            RefreshTime(nextRunOnServer, nextRunOnClient);
            OnPageUpdated();

            if (Populated != null)
                Populated();
        }

        private void RefreshTime(DateTime? nextRunOnServer = null, DateTime? nextRunOnClient = null)
        { 
            var localRun = nextRunOnClient.HasValue
                ? HelpersGUI.DateTimeToString(nextRunOnClient.Value, Messages.DATEFORMAT_WDMY_HM_LONG, true)
                : Messages.UNKNOWN;

            var serverRun = nextRunOnServer.HasValue
                ? HelpersGUI.DateTimeToString(nextRunOnServer.Value, Messages.DATEFORMAT_WDMY_HM_LONG, true)
                : Messages.UNKNOWN;

            labelClientNextRun.Text = string.Format(Messages.VMSS_NEXT_CLIENT_LOCAL_RUN, localRun);
            labelServerNextRun.Text = string.Format(Messages.VMSS_NEXT_SERVER_LOCAL_RUN, serverRun);
        }

        #region IEditPage implementation

        public AsyncAction SaveSettings()
        {
            _policyCopy.frequency = Frequency;
            _policyCopy.schedule = Schedule;
            _policyCopy.retained_snapshots = BackupRetention;
            return null;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _policyCopy = (VMSS)clone;
            GetServerTime();
        }

        public bool ValidToSave => EnableNext();

        public void ShowLocalValidationMessages()
        {
            HelpersGUI.ShowBalloonMessage(flowLayoutPanel1, Messages.VMSS_INVALID_SCHEDULE, InvalidParamToolTip);
        }

        public void Cleanup()
        {
            InvalidParamToolTip.Dispose();
        }

        public bool HasChanged
        {
            get
            {
                if (!Helper.AreEqual2(_policyCopy.frequency, Frequency))
                    return true;

                if (!Helper.AreEqual2(_policyCopy.schedule, Schedule))
                    return true;

                if (!Helper.AreEqual2(_policyCopy.retained_snapshots, BackupRetention))
                    return true;

                return false;
            }
        }

        public event Action Populated;

        #endregion

        #region Control event handlers

        private void NewPolicySnapshotFrequencyPage_ParentChanged(object sender, EventArgs e)
        {
            if (Parent == null || ParentForm == null)
                return;

            var parentFormType = ParentForm.GetType();

            if (parentFormType == typeof(XenWizardBase))
                sectionLabelSchedule.LineColor = sectionLabelNumber.LineColor = SystemColors.Window;
            else if (parentFormType == typeof(PropertiesDialog))
                sectionLabelSchedule.LineColor = sectionLabelNumber.LineColor = SystemColors.ActiveBorder;
        }

        private void comboBoxMin_SelectedIndexChanged(object sender, EventArgs e)
        {
            RecalculateSchedule();
        }

        private void dateTimePickerDaily_ValueChanged(object sender, EventArgs e)
        {
            if (!dateTimePickerDaily.AutoCorrecting)
                RecalculateSchedule();
        }

        private void dateTimePickerWeekly_ValueChanged(object sender, EventArgs e)
        {
            if (!dateTimePickerWeekly.AutoCorrecting)
                RecalculateSchedule();
        }

        private void daysWeekCheckboxes_CheckBoxChanged(object sender, EventArgs e)
        {
            RecalculateSchedule();
        }

        private void radioButtonHourly_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == null || !radioButtonHourly.Checked)
                return;

            panelWeekly.Visible = panelDaily.Visible = false; //hide the others first
            panelHourly.Visible = true;

            Frequency = vmss_frequency.hourly;
            numericUpDownRetention.Value = 10;
            RecalculateSchedule();
        }

        private void radioButtonDaily_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == null || !radioButtonDaily.Checked)
                return;

            panelWeekly.Visible = panelHourly.Visible = false; //hide the others first
            panelDaily.Visible = true;

            Frequency = vmss_frequency.daily;
            numericUpDownRetention.Value = 7;
            RecalculateSchedule();
        }

        private void radioButtonWeekly_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == null || !radioButtonWeekly.Checked)
                return;

            panelHourly.Visible = panelDaily.Visible = false; //hide the others first
            panelWeekly.Visible = true;

            Frequency = vmss_frequency.weekly;
            numericUpDownRetention.Value = 4;
            RecalculateSchedule();
        }

        private void numericUpDownRetention_ValueChanged(object sender, EventArgs e)
        {
            BackupRetention = (long)numericUpDownRetention.Value;
            RecalculateSchedule();
        }

        #endregion
    }
}
