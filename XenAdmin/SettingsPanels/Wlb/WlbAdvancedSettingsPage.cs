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
using System.Data;
using System.Text;
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAPI;


namespace XenAdmin.SettingsPanels
{
    public partial class WlbAdvancedSettingsPage : UserControl, IEditPage
    {
        private const double DAYS_IN_WEEK = 7; //seven days per week
        private const String SMTP_MAILHUB_KEY_NAME = "ssmtp-mailhub";

        private bool _loading = false;
        private bool _hasChanged = false;
        private Pool _pool;
        private WlbPoolConfiguration _poolConfiguration;
        
        private readonly ToolTip InvalidParamToolTip;

        public WlbAdvancedSettingsPage()
        {
            InitializeComponent();
            Text = Messages.WLB_ADVANCED_CONFIGURATION;

            InvalidParamToolTip = new ToolTip();
            InvalidParamToolTip.IsBalloon = true;
            InvalidParamToolTip.ToolTipIcon = ToolTipIcon.Warning;
            InvalidParamToolTip.ToolTipTitle = Messages.INVALID_PARAMETER;
        }

        public Pool Pool
        {
            set
            {
                if (null != value)
                {
                    _pool = value;
                    if (null != _poolConfiguration)
                    {
                        InitializeControls();
                    }
                }
            }
        }

        public WlbPoolConfiguration PoolConfiguration
        {
            set
            {
                if (null != value)
                {
                    _poolConfiguration = value;
                    if (null != _pool)
                    {
                        InitializeControls();
                    }
                }
            }
        }


        private void InitializeControls()
        {
            _loading = true;
            
            // Set up the Relocation Interval numeric up/down
            numericUpDownRelocationInterval.Value = (decimal)_poolConfiguration.RecentMoveMinutes;

            // Disable it for alpha 3
            // Set up the Use Reporting Services checkbox and SMTP Server textbox
            // checkBoxUseReportingServices.Checked = _poolConfiguration.ReportingUseRSServer;

            //Set up the Optimization Severity combobox
            comboBoxOptimizationSeverity.DataSource = new BindingSource(BuildSeverity(), null);
            comboBoxOptimizationSeverity.ValueMember = "key";
            comboBoxOptimizationSeverity.DisplayMember = "value";
            comboBoxOptimizationSeverity.SelectedValue = _poolConfiguration.AutoBalanceSeverity;

            //Set up the Autobalance Aggressiveness combobox
            comboBoxAutoBalanceAggressiveness.DataSource = new BindingSource(BuildAggressiveness(), null);
            comboBoxAutoBalanceAggressiveness.ValueMember = "key";
            comboBoxAutoBalanceAggressiveness.DisplayMember = "value";
            comboBoxAutoBalanceAggressiveness.SelectedValue = _poolConfiguration.AutoBalanceAggressiveness;

            //Set up the Pool Audit Trail Granularity
            //This only works from Creedence
            if(_poolConfiguration.IsCreedenceOrLater)
            {
                comboBoxPoolAuditTrailLevel.DataSource = new BindingSource(PoolAuditGranularity(), null);
                comboBoxPoolAuditTrailLevel.ValueMember = "key";
                comboBoxPoolAuditTrailLevel.DisplayMember = "value";
                comboBoxPoolAuditTrailLevel.SelectedValue = _poolConfiguration.PoolAuditGranularity;
            }
            else
            {
                HidePoolAuditTrailGranularitySection();
            }

            numericUpDownPollInterval.Value = (decimal)_poolConfiguration.AutoBalancePollIntervals;

            numericUpDownRelocationInterval.Value = (decimal)_poolConfiguration.RecentMoveMinutes;

            // For Boston, we do not expose grooming, and no longer support report subscriptions
            HideHistoricalDataSection();
            HideReportSubscriptionSection();

            _loading = false;
        }

        private Dictionary<WlbPoolAutoBalanceSeverity, string> BuildSeverity()
        {
            Dictionary<WlbPoolAutoBalanceSeverity, string> severities = new Dictionary<WlbPoolAutoBalanceSeverity, string>();

            severities.Add(WlbPoolAutoBalanceSeverity.Critical, Messages.WLB_SEVERITY_CRITICAL);
            severities.Add(WlbPoolAutoBalanceSeverity.High, Messages.WLB_SEVERITY_HIGH);
            severities.Add(WlbPoolAutoBalanceSeverity.Medium, Messages.WLB_SEVERITY_MEDIUM);
            severities.Add(WlbPoolAutoBalanceSeverity.Low, Messages.WLB_SEVERITY_LOW);

            return severities;
        }

        private Dictionary<WlbPoolAutoBalanceAggressiveness, string> BuildAggressiveness()
        {
            Dictionary<WlbPoolAutoBalanceAggressiveness, string> aggressiveness = new Dictionary<WlbPoolAutoBalanceAggressiveness, string>();

            aggressiveness.Add(WlbPoolAutoBalanceAggressiveness.High, Messages.WLB_SEVERITY_HIGH);
            aggressiveness.Add(WlbPoolAutoBalanceAggressiveness.Medium, Messages.WLB_SEVERITY_MEDIUM);
            aggressiveness.Add(WlbPoolAutoBalanceAggressiveness.Low, Messages.WLB_SEVERITY_LOW);

            return aggressiveness;
        }

        private Dictionary<WlbAuditTrailLogGranularity, string> PoolAuditGranularity()
        {
            Dictionary<WlbAuditTrailLogGranularity, string> auditLogGranularity = new Dictionary<WlbAuditTrailLogGranularity, string>();

            auditLogGranularity.Add(WlbAuditTrailLogGranularity.Minimum, Messages.WLB_AUDIT_LOG_MINIMUM);
            auditLogGranularity.Add(WlbAuditTrailLogGranularity.Medium, Messages.WLB_AUDIT_LOG_MEDIUM);
            auditLogGranularity.Add(WlbAuditTrailLogGranularity.Maximum, Messages.WLB_AUDIT_LOG_MAXIMUM);

            return auditLogGranularity;
        }

        private bool IsValidSmtpAddress()
        {
            return this.textBoxSMTPServer.Text.Trim().Length > 0;
        }

        private void HideMigrationIntervalSection()
        {
            sectionHeaderLabelVmMigInt.Visible = false;
            labelVmMigInt.Visible = false;
            panelVmMigInt.Visible = false;
        }

        private void HideReportSubscriptionSection()
        {
            sectionHeaderLabelRepSub.Visible = false;
            labelRepSub.Visible = false;
            panelRepSub.Visible = false;
        }

        private void HideHistoricalDataSection()
        {
            sectionHeaderLabelHistData.Visible = false;
            labelHistData.Visible = false;
            panelHistData.Visible = false;
        }

        private void HidePoolAuditTrailGranularitySection()
        {
            label2.Visible = false;
            label3.Visible = false;
            sectionHeaderLabelAuditTrail.Visible = false;
            labelAuditTrail.Visible = false;
            auditTrailPanel.Visible = false;
        }

        private void ToggleShowAggressiveness()
        {
            sectionHeaderLabelOptAgr.Visible = !sectionHeaderLabelOptAgr.Visible;
            labelOptAgr.Visible = !labelOptAgr.Visible;
            panelOptAgr.Visible = !panelOptAgr.Visible;
        }

        #region Control Event Handlers

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                _hasChanged = true;
            }
        }
        
        private void textBoxSMTPServer_TextChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                _hasChanged = true;
            }
        }

        private void numericUpDownPollInterval_ValueChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                _hasChanged = true;
            }
        }

        private void comboBoxOptimizationSeverity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                _hasChanged = true;
            }
        }

        private void comboBoxAutoBalanceAggressiveness_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                _hasChanged = true;
            }
        }
        private void comboBoxPoolAuditTrailLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                _hasChanged = true;
            }
        }


        private void numericUpDownRelocationInterval_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.Control && e.KeyCode == Keys.A)
            {
                ToggleShowAggressiveness();
            }
        }

        #endregion

        #region IEditPage Members

        public AsyncAction SaveSettings()
        {
            _poolConfiguration.AutoBalanceSeverity = (WlbPoolAutoBalanceSeverity)comboBoxOptimizationSeverity.SelectedValue;
            _poolConfiguration.AutoBalanceAggressiveness = (WlbPoolAutoBalanceAggressiveness)comboBoxAutoBalanceAggressiveness.SelectedValue;
            _poolConfiguration.AutoBalancePollIntervals = (double)numericUpDownPollInterval.Value;

            if(_poolConfiguration.IsCreedenceOrLater)
            {
                _poolConfiguration.PoolAuditGranularity = (WlbAuditTrailLogGranularity)comboBoxPoolAuditTrailLevel.SelectedValue;
            }
                
            _poolConfiguration.RecentMoveMinutes = (double)numericUpDownRelocationInterval.Value;
            
            return null;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            throw new NotImplementedException();
        }

        public bool ValidToSave => true;

        public void ShowLocalValidationMessages()
        {
            if (!this.ValidToSave)
            {
                HelpersGUI.ShowBalloonMessage(textBoxSMTPServer, Messages.INVALID_PARAMETER, InvalidParamToolTip);
            }
            //BL: Disable it for now, will enable it after adding the port to WLB side
            /*
            else if (!PerfmonAlertOptionsPage.IsValidPort(TextBoxSMTPServerPort.Text))
            {
                Dialogs.EditVIFDialog.ShowBalloonMessage(TextBoxSMTPServerPort, Messages.INVALID_PARAMETER, InvalidParamToolTip);
            }
            */
        }

        public void Cleanup()
        {
            throw new NotImplementedException();
        }

        public bool HasChanged => _hasChanged;

        #endregion

        #region IVerticalTab Members


        public string SubText => Messages.WLB_ADVANCED_CONFIGURATION_SUBTEXT;

        public Image Image => Properties.Resources._002_Configure_h32bit_16;

        #endregion
    }
}
