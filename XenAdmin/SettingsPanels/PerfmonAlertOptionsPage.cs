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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using XenAPI;
using XenAdmin;
using XenAdmin.SettingsPanels;
using XenAdmin.Alerts;
using XenAdmin.Network;
using XenAdmin.Actions;
using XenAdmin.Core;


namespace XenAdmin.SettingsPanels
{
    public partial class PerfmonAlertOptionsPage : UserControl, IEditPage
    {
        private IXenObject _XenModelObject;
        private PerfmonOptionsDefinition _PerfmonOptions;

        private bool _OrigEmailNotificationCheckBox;
        private string _OrigEmailAddressTextBox;
        private string _OrigSmtpServerAddrTextBox;
        private string _OrigSmtpServerPortTextBox;
        private string _OrigMailLanguageCode;
        private bool _bSupportMailLanguage;

        private readonly ToolTip InvalidParamToolTip;

        public PerfmonAlertOptionsPage()
        {
            InitializeComponent();

            Text = Messages.EMAIL_OPTIONS;

            InvalidParamToolTip = new ToolTip();
            InvalidParamToolTip.IsBalloon = true;
            InvalidParamToolTip.ToolTipIcon = ToolTipIcon.Warning;
            InvalidParamToolTip.ToolTipTitle = Messages.INVALID_PARAMETER;

            MailLanguageComboBox.DataSource = new BindingSource(PerfmonOptionsDefinition.MailLanguageDataSource(), null);
            MailLanguageComboBox.DisplayMember = "Value";
            MailLanguageComboBox.ValueMember = "Key";

            EmailNotificationCheckBox_CheckedChanged(null, null);
        }

        public String SubText
        {
            get
            {
                if (!EmailNotificationCheckBox.Checked)
                    return Messages.NONE_DEFINED;

                return EmailAddressTextBox.Text;
            }
        }

        public Image Image
        {
            get
            {
                return Properties.Resources._000_Email_h32bit_16;
            }
        }

        // match anything with an @ sign in the middle
        private static readonly Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);
        public static bool IsValidEmail(string s)
        {
            return emailRegex.IsMatch(s);
        }

        private bool IsValidSmtpAddress()
        {
            return !SmtpServerAddrTextBox.Text.ToCharArray().Any((c) => c >= 128) && SmtpServerAddrTextBox.Text.Trim().Length > 0;
        }

        public void SetMailLanguageComboBoxValue(String code)
        {
            if (_bSupportMailLanguage && PerfmonOptionsDefinition.MailLanguageHasCode(code))
                MailLanguageComboBox.SelectedValue = code;  // Feature supported and code is valid
            else
            {
                // Set default value
                if (PerfmonOptionsDefinition.MailLanguageHasCode(BrandManager.PerfAlertMailDefaultLanguage))
                    MailLanguageComboBox.SelectedValue = BrandManager.PerfAlertMailDefaultLanguage;
                else
                    MailLanguageComboBox.SelectedIndex = 0;
            }
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _XenModelObject = clone;

            _bSupportMailLanguage = Helpers.InvernessOrGreater(_XenModelObject.Connection);

            Repopulate();

            // Save original settings for change detection
            _OrigEmailNotificationCheckBox = EmailNotificationCheckBox.Checked;
            _OrigEmailAddressTextBox = EmailAddressTextBox.Text;
            _OrigSmtpServerAddrTextBox = SmtpServerAddrTextBox.Text;
            _OrigSmtpServerPortTextBox = SmtpServerPortTextBox.Text;
        }
        public void Repopulate()
        {
            if (_XenModelObject == null)
                return;
            try
            {
                MailLanguageLabel.Visible = MailLanguageComboBox.Visible = _bSupportMailLanguage;

                _PerfmonOptions = PerfmonOptionsDefinition.GetPerfmonOptionsDefinitions(_XenModelObject);
                if (_PerfmonOptions != null)
                {
                    EmailNotificationCheckBox.Checked = true;
                    EmailAddressTextBox.Text = _PerfmonOptions.MailDestination;
                    SmtpServerAddrTextBox.Text = PerfmonOptionsDefinition.GetSmtpServerAddress(_PerfmonOptions.MailHub);
                    SmtpServerPortTextBox.Text = PerfmonOptionsDefinition.GetSmtpPort(_PerfmonOptions.MailHub);

                    SetMailLanguageComboBoxValue(_PerfmonOptions.MailLanguageCode);
                    if (_bSupportMailLanguage)  // Save original MailLanguageCode for change detection
                        _OrigMailLanguageCode = _PerfmonOptions.MailLanguageCode;
                    else
                        _OrigMailLanguageCode = null;
                }
                else
                {
                    SetMailLanguageComboBoxValue(null);
                    _OrigMailLanguageCode = null;
                }
            }
            catch { }
        } // Repopulate()

        public bool HasChanged
        {
            get
            {
                return ((_OrigEmailNotificationCheckBox != EmailNotificationCheckBox.Checked) ||
                        (_OrigEmailAddressTextBox != EmailAddressTextBox.Text) ||
                        (_OrigSmtpServerAddrTextBox != SmtpServerAddrTextBox.Text) ||
                        (_OrigSmtpServerPortTextBox != SmtpServerPortTextBox.Text) ||
                        (_bSupportMailLanguage && _OrigMailLanguageCode != MailLanguageComboBox.SelectedValue.ToString()));
            }
        }

        public void ShowLocalValidationMessages()
        {
            if (!IsValidEmail(EmailAddressTextBox.Text))
            {
                HelpersGUI.ShowBalloonMessage(EmailAddressTextBox, Messages.INVALID_PARAMETER, InvalidParamToolTip);
            }
            else if (!IsValidSmtpAddress())
            {
                HelpersGUI.ShowBalloonMessage(SmtpServerAddrTextBox, Messages.INVALID_PARAMETER, InvalidParamToolTip);
            }
            else if (!Util.IsValidPort(SmtpServerPortTextBox.Text))
            {
                HelpersGUI.ShowBalloonMessage(SmtpServerPortTextBox, Messages.INVALID_PARAMETER, InvalidParamToolTip);
            }
        }

        public bool ValidToSave
        {
            get
            {
                if (EmailNotificationCheckBox.Checked)
                {
                    return IsValidEmail(EmailAddressTextBox.Text) && Util.IsValidPort(SmtpServerPortTextBox.Text) && IsValidSmtpAddress();
                }
                else
                {
                    return true;
                }
            }
        }

        public void Cleanup()
        {
            InvalidParamToolTip.Dispose();
        }

        public AsyncAction SaveSettings()
        {
            PerfmonOptionsDefinition perfmonOptions = null; // a null value will clear the definitions
            if (EmailNotificationCheckBox.Checked)
            {
                string smtpMailHub = SmtpServerAddrTextBox.Text + ":" + SmtpServerPortTextBox.Text;
                string mailLanguageCode = null;
                if (_bSupportMailLanguage && null != MailLanguageComboBox.SelectedValue)
                    mailLanguageCode = MailLanguageComboBox.SelectedValue.ToString();
                perfmonOptions = new PerfmonOptionsDefinition(smtpMailHub, EmailAddressTextBox.Text, mailLanguageCode);
            }

            return new PerfmonOptionsDefinitionAction(_XenModelObject.Connection, perfmonOptions, true);
        }

        private void EmailNotificationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            EmailAddressTextBox.Enabled = EmailNotificationCheckBox.Checked;
            SmtpServerAddrTextBox.Enabled = EmailNotificationCheckBox.Checked;
            SmtpServerPortTextBox.Enabled = EmailNotificationCheckBox.Checked;
            MailLanguageComboBox.Enabled = EmailNotificationCheckBox.Checked;
        }
    }
}
