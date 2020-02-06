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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Model;
using XenAdmin.Properties;
using XenAPI;


namespace XenAdmin.Dialogs.HealthCheck
{
    public partial class HealthCheckSettingsDialog : XenDialogBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Pool pool;
        private HealthCheckSettings healthCheckSettings;
        private bool authenticationRequired;
        private bool authenticated;
        private string authenticationToken;
        private string diagnosticToken;
        private string xsUserName;
        private string xsPassword;

        internal override string HelpName { get { return "HealthCheckSettingsDialog"; } }

        public HealthCheckSettingsDialog(Pool pool, bool enrollNow)
            :base(pool.Connection)
        {
            this.pool = pool;
            healthCheckSettings = pool.HealthCheckSettings();
            if (enrollNow)
                healthCheckSettings.Status = HealthCheckStatus.Enabled;
            authenticated = healthCheckSettings.TryGetExistingTokens(pool.Connection, out authenticationToken, out diagnosticToken);
            authenticationRequired = !authenticated;
            xsUserName = healthCheckSettings.GetSecretyInfo(pool.Connection, HealthCheckSettings.UPLOAD_CREDENTIAL_USER_SECRET);
            xsPassword = healthCheckSettings.GetSecretyInfo(pool.Connection, HealthCheckSettings.UPLOAD_CREDENTIAL_PASSWORD_SECRET);
            InitializeComponent();
            PopulateControls();
            InitializeControls();
            UpdateButtons();
        }

        private void PopulateControls()
        {
            var list = BuildDays();
            var ds = new BindingSource(list, null);
            dayOfWeekComboBox.DataSource = ds;
            dayOfWeekComboBox.ValueMember = "key";
            dayOfWeekComboBox.DisplayMember = "value";

            var list1 = BuildHours();
            var ds1 = new BindingSource(list1, null);
            timeOfDayComboBox.DataSource = ds1;
            timeOfDayComboBox.ValueMember = "key";
            timeOfDayComboBox.DisplayMember = "value";
        }

        private Dictionary<int, string> BuildDays()
        {
            Dictionary<int, string> days = new Dictionary<int, string>();
            foreach (var dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                days.Add((int)dayOfWeek, HelpersGUI.DayOfWeekToString((DayOfWeek)dayOfWeek, true));
            }
            return days;
        }

        private SortedDictionary<int, string> BuildHours()
        {
            SortedDictionary<int, string> hours = new SortedDictionary<int, string>();
            for (int hour = 0; hour <= 23; hour++)
            {
                DateTime time = new DateTime(1900, 1, 1, hour, 0, 0);
                hours.Add(hour, HelpersGUI.DateTimeToString(time, Messages.DATEFORMAT_HM, true));
            }
            return hours;
        }

        private void InitializeControls()
        {
            Text = String.Format(Messages.HEALTHCHECK_ENROLLMENT_TITLE, pool.Name());
            
            string noAuthTokenMessage = string.Format(Messages.HEALTHCHECK_AUTHENTICATION_RUBRIC_NO_TOKEN, Messages.MY_CITRIX_CREDENTIALS_URL);
            string existingAuthTokenMessage = Messages.HEALTHCHECK_AUTHENTICATION_RUBRIC_EXISTING_TOKEN;
            string authenticationRubricLabelText = authenticationRequired ? noAuthTokenMessage : existingAuthTokenMessage;

            if (authenticationRubricLabelText == noAuthTokenMessage)
            {
                authRubricTextLabel.Visible = false;
                authRubricLinkLabel.Text = noAuthTokenMessage;
                authRubricLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(authenticationRubricLabelText.IndexOf(Messages.MY_CITRIX_CREDENTIALS_URL), Messages.MY_CITRIX_CREDENTIALS_URL.Length);
            }
            else
            {
                authRubricLinkLabel.Visible = false;
                authRubricTextLabel.Text = existingAuthTokenMessage;
            }

            enrollmentCheckBox.Checked = healthCheckSettings.Status != HealthCheckStatus.Disabled;
            frequencyNumericBox.Value = healthCheckSettings.IntervalInWeeks;
            dayOfWeekComboBox.SelectedValue = (int)healthCheckSettings.DayOfWeek;
            timeOfDayComboBox.SelectedValue = healthCheckSettings.TimeOfDay;
            
            existingAuthenticationRadioButton.Enabled = existingAuthenticationRadioButton.Checked = !authenticationRequired;
            newAuthenticationRadioButton.Checked = authenticationRequired;
            SetMyCitrixCredentials(existingAuthenticationRadioButton.Checked);

            bool useCurrentXsCredentials = string.IsNullOrEmpty(xsUserName) || xsUserName == pool.Connection.Username;
            newXsCredentialsRadioButton.Checked = !useCurrentXsCredentials;
            currentXsCredentialsRadioButton.Checked = useCurrentXsCredentials;
            SetXSCredentials(currentXsCredentialsRadioButton.Checked);
        }

        private bool ChangesMade()
        {
            if (enrollmentCheckBox.Checked && healthCheckSettings.Status != HealthCheckStatus.Enabled)
                return true;
            if (!enrollmentCheckBox.Checked && healthCheckSettings.Status != HealthCheckStatus.Disabled)
                return true;
            if (frequencyNumericBox.Value != healthCheckSettings.IntervalInWeeks)
                return true;
            if (dayOfWeekComboBox.SelectedIndex != (int)healthCheckSettings.DayOfWeek)
                return true;
            if (timeOfDayComboBox.SelectedIndex != healthCheckSettings.TimeOfDay)
                return true;
            if (authenticationToken != healthCheckSettings.GetSecretyInfo(pool.Connection, HealthCheckSettings.UPLOAD_TOKEN_SECRET))
                return true;
            if (textboxXSUserName.Text != xsUserName)
                return true;
            if (textboxXSPassword.Text != xsPassword)
                return true;
            return false;
        }

        private void UpdateButtons()
        {
            okButton.Enabled = m_ctrlError.PerformCheck(CheckCredentialsEntered) && !errorLabel.Visible;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            okButton.Enabled = false;
            if (enrollmentCheckBox.Checked && newAuthenticationRadioButton.Checked 
                && !m_ctrlError.PerformCheck(CheckUploadAuthentication))
            {
                okButton.Enabled = true;
                return;
            }

            if (ChangesMade())
            {
                var newHealthCheckSettings = new HealthCheckSettings(pool.health_check_config);

                newHealthCheckSettings.Status = enrollmentCheckBox.Checked ? HealthCheckStatus.Enabled : HealthCheckStatus.Disabled;
                newHealthCheckSettings.IntervalInDays = (int)(frequencyNumericBox.Value * 7);
                newHealthCheckSettings.DayOfWeek = (DayOfWeek)dayOfWeekComboBox.SelectedValue;
                newHealthCheckSettings.TimeOfDay = (int)timeOfDayComboBox.SelectedValue;
                newHealthCheckSettings. RetryInterval = HealthCheckSettings.DEFAULT_RETRY_INTERVAL;
                
                new SaveHealthCheckSettingsAction(pool, newHealthCheckSettings, authenticationToken, diagnosticToken, textboxXSUserName.Text.Trim(), textboxXSPassword.Text, false).RunAsync();
                new TransferHealthCheckSettingsAction(pool, newHealthCheckSettings, textboxXSUserName.Text.Trim(), textboxXSPassword.Text, true).RunAsync();
            }
            okButton.Enabled = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void enrollmentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!enrollmentCheckBox.Checked)
                HideTestCredentialsStatus();
            UpdateButtons();
        }

        private void newAuthenticationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetMyCitrixCredentials(existingAuthenticationRadioButton.Checked);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            SetXSCredentials(currentXsCredentialsRadioButton.Checked);
            testCredentialsButton.Enabled = newXsCredentialsRadioButton.Checked &&
                !string.IsNullOrEmpty(textboxXSUserName.Text.Trim()) && !string.IsNullOrEmpty(textboxXSPassword.Text);
            UpdateButtons();
        }

        private void SetXSCredentials(bool useCurrent)
        {
            if (useCurrent)
            {
                textboxXSUserName.Text = pool.Connection.Username;
                textboxXSPassword.Text = pool.Connection.Password;
                textboxXSUserName.Enabled = false;
                textboxXSPassword.Enabled = false;
            }
            else
            {
                textboxXSUserName.Text = xsUserName;
                textboxXSPassword.Text = xsPassword;
                textboxXSUserName.Enabled = true;
                textboxXSPassword.Enabled = true;
            }
        }

        private void SetMyCitrixCredentials(bool useExisting)
        {
            if (useExisting)
            {
                //textBoxMyCitrixUsername.Text = String.Empty;
                //textBoxMyCitrixPassword.Text = String.Empty;
                textBoxMyCitrixUsername.Enabled = false;
                textBoxMyCitrixPassword.Enabled = false;
            }
            else
            {
                //textBoxMyCitrixUsername.Text = String.Empty;
                //textBoxMyCitrixPassword.Text = String.Empty;
                textBoxMyCitrixUsername.Enabled = true;
                textBoxMyCitrixPassword.Enabled = true;
            }
        }
        
        private bool CheckCredentialsEntered()
        {
            if (!enrollmentCheckBox.Checked)
                return true;

            if (newAuthenticationRadioButton.Checked && 
                (string.IsNullOrEmpty(textBoxMyCitrixUsername.Text.Trim()) || string.IsNullOrEmpty(textBoxMyCitrixPassword.Text)))
                return false;

            if (newXsCredentialsRadioButton.Checked &&
                (string.IsNullOrEmpty(textboxXSUserName.Text.Trim()) || string.IsNullOrEmpty(textboxXSPassword.Text)))
                return false;

            return true;
        }

        private bool CheckCredentialsEntered(out string error)
        {
            error = string.Empty;
            return CheckCredentialsEntered();
        }

        private bool CheckUploadAuthentication(out string error)
        {
            error = string.Empty;

            if (!CheckCredentialsEntered())
                return false;

            var action = new HealthCheckAuthenticationAction(textBoxMyCitrixUsername.Text.Trim(), textBoxMyCitrixPassword.Text.Trim(),
                Registry.HealthCheckIdentityTokenDomainName, Registry.HealthCheckUploadGrantTokenDomainName, Registry.HealthCheckUploadTokenDomainName,
                Registry.HealthCheckDiagnosticDomainName, Registry.HealthCheckProductKey, 0, false);

            try
            {
                action.RunExternal(null);
            }
            catch
            {
                error = action.Exception != null ? action.Exception.Message : Messages.ERROR_UNKNOWN;
                authenticationToken = null;
                authenticated = false;
                return authenticated;
            }

            authenticationToken = action.UploadToken;  // curent upload token
            diagnosticToken = action.DiagnosticToken;  // curent diagnostic token
            authenticated = !String.IsNullOrEmpty(authenticationToken) && !String.IsNullOrEmpty(diagnosticToken);
            return authenticated;
        }

        private void credentials_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void xsCredentials_TextChanged(object sender, EventArgs e)
        {
            testCredentialsButton.Enabled = newXsCredentialsRadioButton.Checked &&
                !string.IsNullOrEmpty(textboxXSUserName.Text.Trim()) && !string.IsNullOrEmpty(textboxXSPassword.Text);
            HideTestCredentialsStatus();
        }

        private void PolicyStatementLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new HealthCheckPolicyStatementDialog().ShowDialog(this);
        }

        private void testCredentialsButton_Click(object sender, EventArgs e)
        {
            okButton.Enabled = false;
            CheckXenServerCredentials();
        }

        private void CheckXenServerCredentials()
        {
            if (string.IsNullOrEmpty(textboxXSUserName.Text.Trim()) || string.IsNullOrEmpty(textboxXSPassword.Text))
                return;

            bool passedRbacChecks = false;
            DelegatedAsyncAction action = new DelegatedAsyncAction(connection,
                Messages.CREDENTIALS_CHECKING, "", "", 
                delegate
                {
                    Session elevatedSession = null;
                    try
                    {
                        elevatedSession = connection.ElevatedSession(textboxXSUserName.Text.Trim(), textboxXSPassword.Text);
                        if (elevatedSession != null && (elevatedSession.IsLocalSuperuser || SessionAuthorized(elevatedSession, Role.ValidRoleList("pool.set_health_check_config", connection))))
                            passedRbacChecks = true;
                    }
                    catch (Failure f)
                    {
                        if (f.ErrorDescription.Count > 0 && f.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                        {
                            // we use a different error message here from the standard one in friendly names
                            throw new Exception(Messages.HEALTH_CHECK_USER_HAS_NO_PERMISSION_TO_CONNECT);
                        }
                        throw;
                    }
                    finally
                    {
                        if (elevatedSession != null)
                        {
                            elevatedSession.Connection.Logout(elevatedSession);
                            elevatedSession = null;
                        }
                    }
                },
                true);

            action.Completed += delegate
            {
                log.DebugFormat("Logging with the new credentials returned: {0} ", passedRbacChecks);
                Program.Invoke(Program.MainWindow, () =>
                {
                    if (passedRbacChecks)
                    {
                         ShowTestCredentialsStatus(Resources._000_Tick_h32bit_16, null);
                         okButton.Enabled = true;
                    }
                    else
                    {
                        okButton.Enabled = false;
                        ShowTestCredentialsStatus(Resources._000_error_h32bit_16, action.Exception != null ? action.Exception.Message : Messages.HEALTH_CHECK_USER_NOT_AUTHORIZED);
                    }   
                    textboxXSUserName.Enabled = textboxXSPassword.Enabled = testCredentialsButton.Enabled = newXsCredentialsRadioButton.Checked;
                });
            };

            log.Debug("Testing logging in with the new credentials");
            ShowTestCredentialsStatus(Resources.ajax_loader, null);
            textboxXSUserName.Enabled = textboxXSPassword.Enabled = testCredentialsButton.Enabled = false;
            action.RunAsync();
        }

        private void ShowTestCredentialsStatus(Image image, string errorMessage)
        {
            testCredentialsStatusImage.Visible = true;
            testCredentialsStatusImage.Image = image;
            errorLabel.Text = errorMessage;
            errorLabel.Visible = !string.IsNullOrEmpty(errorMessage);
        }

        private void HideTestCredentialsStatus()
        {
            testCredentialsStatusImage.Visible = false;
            errorLabel.Visible = false;
        }

        private bool SessionAuthorized(Session s, List<Role> authorizedRoles)
        {
            UserDetails ud = s.CurrentUserDetails;
            foreach (Role r in s.Roles)
            {
                if (authorizedRoles.Contains(r))
                {
                    log.DebugFormat("Subject '{0}' is authorized to complete the action", ud.UserName ?? ud.UserSid);
                    return true;
                }
            }
            log.DebugFormat("Subject '{0}' is not authorized to complete the action", ud.UserName ?? ud.UserSid);
            return false;
        }

        private void existingAuthenticationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void authRubricLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.OpenURL(Messages.MY_CITRIX_CREDENTIALS_URL);
        }

    }
}
