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
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using XenAPI;
using XenAdmin.Actions.Wlb;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAdmin.Controls;



namespace XenAdmin.Dialogs.Wlb
{
    public partial class WlbReportSubscriptionDialog : XenAdmin.Dialogs.XenDialogBase
    {

        #region Variables

        private Pool _pool;
        private string _reportDisplayName;
        private Dictionary<string, string> _rpParams;
        private WlbReportSubscription _subscription;
        private string _subId = String.Empty;
        
        ToolTip InvalidParamToolTip = new ToolTip();
        // Due to localization, changed email regex from @"^[A-Z0-9._%+-]+@([A-Z0-9-]+\.)*[A-Z0-9-]+$" 
        // to match anything with an @ sign in the middle
        private static readonly Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);
        
        #endregion


        #region Constructors
        // Create new subscription
        public WlbReportSubscriptionDialog(string reportDisplayName, Dictionary<string, string> reportParams, Pool pool)
            : base(pool.Connection)
        {
            _rpParams = reportParams;
            _subscription = null;
            _reportDisplayName = reportDisplayName;
            _pool = pool;
            init();
        }

        // Edit existing subscription
        public WlbReportSubscriptionDialog(string reportDisplayName, WlbReportSubscription subscription, Pool pool)
            : base(pool.Connection)
        {
            _subId = subscription.Id;
            _rpParams = subscription.ReportParameters;
            _subscription = subscription;
            _reportDisplayName = reportDisplayName;
            _pool = pool;
            init();
        }
        #endregion


        #region Private Methods

        private void init()
        {
            InitializeComponent();
            InitializeControls();

            // Initialize InvalidParamToolTip
            InvalidParamToolTip.IsBalloon = true;
            InvalidParamToolTip.ToolTipIcon = ToolTipIcon.Warning;
            InvalidParamToolTip.ToolTipTitle = Messages.INVALID_PARAMETER;

            if (null != _subscription)
            {
                LoadSubscription();
            }
        }

        private void InitializeControls()
        {
            this.Text = String.Concat(this.Text, this._reportDisplayName);
            this.rpParamComboBox.SelectedIndex = 0;
            this.rpRenderComboBox.SelectedIndex = 0;

            this.schedDeliverComboBox.DataSource = new BindingSource(BuildDaysOfWeek(), null);
            this.schedDeliverComboBox.ValueMember = "key";
            this.schedDeliverComboBox.DisplayMember = "value"; ;
            this.schedDeliverComboBox.SelectedIndex = 1;

            this.dateTimePickerSchedEnd.Value = DateTime.Now.AddMonths(1);
            this.dateTimePickerSchedStart.Value = DateTime.Now;
        }


        private Dictionary<int, string> BuildDaysOfWeek()
        {
            Dictionary<int, string> days = new Dictionary<int, string>();

            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.All, GetLocalizedDayOfWeek(WlbScheduledTask.WlbTaskDaysOfWeek.All));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Weekdays, GetLocalizedDayOfWeek(WlbScheduledTask.WlbTaskDaysOfWeek.Weekdays));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Weekends, GetLocalizedDayOfWeek(WlbScheduledTask.WlbTaskDaysOfWeek.Weekends));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Sunday, GetLocalizedDayOfWeek(WlbScheduledTask.WlbTaskDaysOfWeek.Sunday));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Monday, GetLocalizedDayOfWeek(WlbScheduledTask.WlbTaskDaysOfWeek.Monday));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Tuesday, GetLocalizedDayOfWeek(WlbScheduledTask.WlbTaskDaysOfWeek.Tuesday));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Wednesday, GetLocalizedDayOfWeek(WlbScheduledTask.WlbTaskDaysOfWeek.Wednesday));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Thursday, GetLocalizedDayOfWeek(WlbScheduledTask.WlbTaskDaysOfWeek.Thursday));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Friday, GetLocalizedDayOfWeek(WlbScheduledTask.WlbTaskDaysOfWeek.Friday));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Saturday, GetLocalizedDayOfWeek(WlbScheduledTask.WlbTaskDaysOfWeek.Saturday));
            
            return days;
        }

        private string GetLocalizedDayOfWeek(WlbScheduledTask.WlbTaskDaysOfWeek daysOfWeek)
        {
            return WlbScheduledTask.DaysOfWeekL10N(daysOfWeek);
        }

        private void LoadSubscription()
        {
            // subscription name
            this.subNameTextBox.Text = this._subscription.Description;

            // report data range
            int days = 0;
            if (this._subscription.ReportParameters != null)
            {
                int.TryParse(this._subscription.ReportParameters["Start"], out days);
            }
            this.rpParamComboBox.SelectedIndex = (days-1)/-7 -1;
            
            // email info
            this.emailToTextBox.Text = this._subscription.EmailTo;
            this.emailCcTextBox.Text = this._subscription.EmailCc;
            this.emailBccTextBox.Text = this._subscription.EmailBcc;
            this.emailReplyTextBox.Text = this._subscription.EmailReplyTo;
            this.emailSubjectTextBox.Text = this._subscription.EmailSubject;
            this.emailCommentRichTextBox.Text = this._subscription.EmailComment;
            this.rpRenderComboBox.SelectedIndex = (int)this._subscription.ReportRenderFormat;

            // convert utc days of week and utc execute time to local days of week and local execute time
            DateTime localExecuteTime;
            WlbScheduledTask.WlbTaskDaysOfWeek localDaysOfWeek;
            WlbScheduledTask.GetLocalTaskTimes(this._subscription.DaysOfWeek, this._subscription.ExecuteTimeOfDay, out localDaysOfWeek, out localExecuteTime);

            // subscription run time
            this.dateTimePickerSubscriptionRunTime.Value = localExecuteTime;

            // subscription delivery day
            this.schedDeliverComboBox.SelectedValue = (int)localDaysOfWeek;
           
            // subscription enable start and end dates
            if (this._subscription.DisableDate != DateTime.MinValue)
            {
                this.dateTimePickerSchedEnd.Value = this._subscription.DisableDate.ToLocalTime();
            }
            if (this._subscription.EnableDate != DateTime.MinValue)
            {
                this.dateTimePickerSchedStart.Value = this._subscription.EnableDate.ToLocalTime();
            }
        }

        #endregion //Private Methods


        #region DateTimePicker and ComboBox Event Handler

        private void rpParamComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.rpParamComboBox.SelectedIndex)
            {
                case 0:
                    this.dateTimePickerSchedStart.Value = DateTime.Now.AddDays(-7);
                    break;
                case 1:
                    this.dateTimePickerSchedStart.Value = DateTime.Now.AddDays(-14);
                    break;
                case 2:
                    this.dateTimePickerSchedStart.Value = DateTime.Now.AddDays(-21);
                    break;
                case 3:
                    this.dateTimePickerSchedStart.Value = DateTime.Now.AddMonths(-1);
                    break;
            }
            this.dateTimePickerSchedEnd.Value = DateTime.Now;
        }

        private void dateTimePickerSchedEnd_ValueChanged(object sender, EventArgs e)
        {
            if (this.dateTimePickerSchedEnd.Value < this.dateTimePickerSchedStart.Value)
            {
                this.dateTimePickerSchedStart.Value = this.dateTimePickerSchedEnd.Value;
            }
        }

        private void dateTimePickerSchedStart_ValueChanged(object sender, EventArgs e)
        {
            if (this.dateTimePickerSchedStart.Value > this.dateTimePickerSchedEnd.Value)
            {
                this.dateTimePickerSchedEnd.Value = this.dateTimePickerSchedStart.Value;
            }
        }

        #endregion //DateTimePicker and ComboBox Event Handler


        #region Validators

        private bool ValidToSave()
        {
            foreach (Control ctl in this.tableLayoutPanelSubscriptionName.Controls)
            {
                if (!IsValidControl(ctl))
                {
                    HelpersGUI.ShowBalloonMessage(ctl, Messages.INVALID_PARAMETER, InvalidParamToolTip);
                    return false;
                }
            }

            foreach (Control ctl in this.tableLayoutPanelDeliveryOptions.Controls)
            {
                if (!IsValidControl(ctl))
                {
                    HelpersGUI.ShowBalloonMessage(ctl, Messages.INVALID_PARAMETER, InvalidParamToolTip);
                    return false;
                }
            }

            return true;
        }

        private bool IsValidControl(Control ctl)
        {
            if (String.Compare(this.subNameTextBox.Name, ctl.Name) == 0)
            {
                return !String.IsNullOrEmpty(ctl.Text);
            }
            else if (String.Compare(this.emailToTextBox.Name, ctl.Name) == 0 || String.Compare(this.emailReplyTextBox.Name, ctl.Name) == 0)
            {
                return IsValidEmail(ctl.Text);
            }
            else if (String.Compare(this.emailBccTextBox.Name, ctl.Name) == 0 || String.Compare(this.emailCcTextBox.Name, ctl.Name) == 0)
            {
                if (!String.IsNullOrEmpty(ctl.Text))
                {
                    return IsValidEmail(ctl.Text);
                }
            }
            return true;
        }

        private static bool IsValidEmail(string emailAddress)
        {
            foreach (string address in emailAddress.Split(new char[] { ';' }))
            {
                if (!emailRegex.IsMatch(address))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion 


        #region Button Click Event Handler

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!ValidToSave())
            {
                this.DialogResult = DialogResult.None;
            }
            else
            {
                SaveSubscription();
                InvalidParamToolTip.Dispose();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            InvalidParamToolTip.Dispose();
        }

        #endregion


        #region Private Methods

        private string GetLoggedInAsText()
        {
            if (_pool.Connection == null)
            {
                // Shouldn't happen
                return String.Empty;
            }

            Session session = _pool.Connection.Session;
            if (session == null)
            {
                return String.Empty;
            }

            return session.UserFriendlyName();
        }

        private void SaveSubscription()
        {
            if (this._subscription == null)
            {
                _subscription = new WlbReportSubscription(String.Empty);
                _subscription.SubscriberName = GetLoggedInAsText();
                _subscription.Created = DateTime.UtcNow;
            }
            _subscription.Name = this.subNameTextBox.Text;
            _subscription.Description = this.subNameTextBox.Text;

            DateTime utcExecuteTime;
            WlbScheduledTask.WlbTaskDaysOfWeek utcDaysOfWeek;
            WlbScheduledTask.GetUTCTaskTimes((WlbScheduledTask.WlbTaskDaysOfWeek)this.schedDeliverComboBox.SelectedValue, this.dateTimePickerSubscriptionRunTime.Value, out utcDaysOfWeek, out utcExecuteTime);
            _subscription.ExecuteTimeOfDay = utcExecuteTime;
            _subscription.DaysOfWeek = utcDaysOfWeek;
            if (_subscription.DaysOfWeek != WlbScheduledTask.WlbTaskDaysOfWeek.All)
            {
                _subscription.TriggerType = (int)WlbScheduledTask.WlbTaskTriggerType.Weekly;
            }
            else
            {
                _subscription.TriggerType = (int)WlbScheduledTask.WlbTaskTriggerType.Daily;
            }
            _subscription.Enabled = true;
            _subscription.EnableDate = this.dateTimePickerSchedStart.Value == DateTime.MinValue ? DateTime.UtcNow : this.dateTimePickerSchedStart.Value.ToUniversalTime();
            _subscription.DisableDate = this.dateTimePickerSchedEnd.Value == DateTime.MinValue ? DateTime.UtcNow.AddMonths(1) : this.dateTimePickerSchedEnd.Value.ToUniversalTime();
            _subscription.LastTouched = DateTime.UtcNow;
            _subscription.LastTouchedBy = GetLoggedInAsText();

            // store email info
            _subscription.EmailTo = this.emailToTextBox.Text.Trim();
            _subscription.EmailCc = this.emailCcTextBox.Text.Trim();
            _subscription.EmailBcc = this.emailBccTextBox.Text.Trim();
            _subscription.EmailReplyTo = this.emailReplyTextBox.Text.Trim();
            _subscription.EmailSubject = this.emailSubjectTextBox.Text.Trim();
            _subscription.EmailComment = this.emailCommentRichTextBox.Text;

            // store reoprt Info
            //sub.ReportId = ;
            _subscription.ReportRenderFormat = this.rpRenderComboBox.SelectedIndex;
            Dictionary<string, string> rps = new Dictionary<string, string>();
            foreach(string key in this._rpParams.Keys)
            {
                if (String.Compare(key, WlbReportSubscription.REPORT_NAME, true) == 0)
                    _subscription.ReportName = this._rpParams[WlbReportSubscription.REPORT_NAME];
                else
                {
                    //Get start date range
                    if (String.Compare(key, "start", true) == 0)
                    {
                        rps.Add(key, ((this.rpParamComboBox.SelectedIndex + 1) * (-7)+1).ToString());
                    }
                    else
                    {
                        rps.Add(key, _rpParams[key]);
                    }
                }
            }
            _subscription.ReportParameters = rps;

            SendWlbConfigurationAction action = new SendWlbConfigurationAction(this._pool, _subscription.ToDictionary(), SendWlbConfigurationKind.SetReportSubscription);
            using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Blocks))
            {
                dialog.ShowCancel = true;
                dialog.ShowDialog(this);
            }
           
            if (action.Succeeded)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
            else if(!action.Cancelled)
            {
                using (var dlg = new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                       SystemIcons.Error,
                       String.Format(Messages.WLB_SUBSCRIPTION_ERROR, _subscription.Description),
                       Messages.XENCENTER)))
                {
                    dlg.ShowDialog(this);
                }
                //log.ErrorFormat("There was an error calling SendWlbConfigurationAction to SetReportSubscription {0}, Action Result: {1}.", _subscription.Description, action.Result);
                DialogResult = DialogResult.None;
            }
        }
        #endregion //Button Click Event Handler
    
    }
}