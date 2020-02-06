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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Controls.Common;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenCenterLib;
using Registry = XenAdmin.Core.Registry;


namespace XenAdmin.Wizards.BugToolWizardFiles
{
    public partial class BugToolPageDestination : XenTabPage
    {
        private bool m_buttonNextEnabled;

        private const int TokenExpiration = 86400; // 24 hours

        public BugToolPageDestination()
        {
            InitializeComponent();
        }

        public override string Text => Messages.BUGTOOL_PAGE_DESTINATION_TEXT;

        public override string PageTitle => Messages.BUGTOOL_PAGE_DESTINATION_PAGETITLE;

        public override string HelpID => "ReportDestination";

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction != PageLoadedDirection.Forward)
                return;

            m_textBoxName.Text = string.Format("{0}{1}.zip", Messages.BUGTOOL_FILE_PREFIX, HelpersGUI.DateTimeToString(DateTime.Now, "yyyy-MM-dd-HH-mm-ss", false));

            string initialDirectory = Properties.Settings.Default.ServerStatusPath;
            
            try
            {
                //previous versions were storing the zip file name rather than the directory; check if this is the case
                if (!string.IsNullOrEmpty(initialDirectory) && !Directory.Exists(initialDirectory))
                    initialDirectory = Path.GetDirectoryName(initialDirectory);
            }
            catch
            {
                //ignore
            }

            if (string.IsNullOrEmpty(initialDirectory) || !Directory.Exists(initialDirectory))
                m_textBoxLocation.Text = Win32.GetKnownFolderPath(Win32.KnownFolders.Downloads);
            else
                m_textBoxLocation.Text = initialDirectory;

            string enterCredentialsMessage = string.Format(Messages.STATUS_REPORT_ENTER_CREDENTIALS_MESSAGE, Messages.MY_CITRIX_CREDENTIALS_URL);
            enterCredentialsLinkLabel.Text = enterCredentialsMessage;
            enterCredentialsLinkLabel.LinkArea = new LinkArea(enterCredentialsMessage.IndexOf(Messages.MY_CITRIX_CREDENTIALS_URL), Messages.MY_CITRIX_CREDENTIALS_URL.Length);

            usernameTextBox.Visible = usernameLabel.Visible = passwordLabel.Visible = passwordTextBox.Visible =
                caseNumberLabel.Visible = caseNumberTextBox.Visible = optionalLabel.Visible =
                enterCredentialsLinkLabel.Visible = uploadCheckBox.Visible = !HiddenFeatures.UploadOptionHidden;

            PerformCheck(CheckPathValid, CheckCredentialsEntered);
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction != PageLoadedDirection.Forward)
                return;

            if (!PerformCheck(CheckPathValid, CheckDestinationFolderExists, CheckCredentialsEntered, CheckUploadAuthentication))
            {
                cancel = true;
                return;
            }

            string path = OutputFile;

            if (File.Exists(path)) //confirm ok to overwrite
            {
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, string.Format(Messages.FILE_X_EXISTS_OVERWRITE, path), Messages.XENCENTER),
                    ThreeButtonDialog.ButtonOK,
                    new ThreeButtonDialog.TBDButton(Messages.CANCEL, DialogResult.Cancel, ThreeButtonDialog.ButtonType.CANCEL, true)))
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK)
                    {
                        cancel = true;
                        return;
                    }
                }
            }

            // Check we can write to the destination file - otherwise we only find out at the 
            // end of the ZipStatusReportAction, and the downloaded server files are lost,
            // and the user will have to run the wizard again.
            try
            {
                using (File.OpenWrite(path)) { }
            }
            catch (Exception exn)
            {
                // Failure
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(
                        SystemIcons.Error,
                        string.Format(Messages.COULD_NOT_WRITE_FILE, path, exn.Message),
                        Messages.XENCENTER)))
                {
                    dlg.ShowDialog(this);
                }

                cancel = true;
                return;
            }

            // Save away the output directory for next time
            Properties.Settings.Default.ServerStatusPath = m_textBoxLocation.Text.Trim();
        }

        public override void SelectDefaultControl()
        {
            if (string.IsNullOrEmpty(m_textBoxLocation.Text))
                m_textBoxLocation.Select();
        }

        public string OutputFile
        {
            get
            {
                var name = m_textBoxName.Text.Trim();
                var folder = m_textBoxLocation.Text.Trim();
                if (!name.EndsWith(".zip"))
                    name = string.Concat(name, ".zip");

                return string.Format(@"{0}\{1}", folder, name);
            }
        }

        public bool Upload
        {
            get
            {
                return uploadCheckBox.Checked;
            }
        }

        public string UploadToken { get; private set; }

        public string CaseNumber
        {
            get
            {
                return caseNumberTextBox.Text.Trim();
            }
        }

        private bool PerformCheck(params CheckDelegate[] checks)
        {
            bool success = m_ctrlError.PerformCheck(checks);
            m_buttonNextEnabled = success;
            OnPageUpdated();
            return success;
        }

        private bool CheckPathValid(out string error)
        {
            error = string.Empty;

            var name = m_textBoxName.Text.Trim();
            var folder = m_textBoxLocation.Text.Trim();

            if (String.IsNullOrEmpty(name))
                return false;

            if (!PathValidator.IsFileNameValid(name))
            {
                error = Messages.BUGTOOL_PAGE_DESTINATION_INVALID_NAME;
                return false;
            }

            if (String.IsNullOrEmpty(folder))
                return false;

            string path = String.Format("{0}\\{1}", folder, name);

            if (!PathValidator.IsPathValid(path))
            {
                error = Messages.BUGTOOL_PAGE_DESTINATION_INVALID_FOLDER;
                return false;
            }

            return true;
        }

        private bool CheckDestinationFolderExists(out string error)
        {
            error = string.Empty;
            var folder = m_textBoxLocation.Text.Trim();

            if (Directory.Exists(folder))
                return true;

            error = Messages.ERROR_DESTINATION_DIR_NON_EXIST;
            return false;
        }

        private bool CheckCredentialsEntered(out string error)
        {
            error = string.Empty;

            if (!uploadCheckBox.Checked)
                return true;

            if (string.IsNullOrEmpty(usernameTextBox.Text.Trim()) || string.IsNullOrEmpty(passwordTextBox.Text))
                return false;

            return true;
        }

        private bool CheckUploadAuthentication(out string error)
        {
            error = string.Empty;

            if (!uploadCheckBox.Checked)
                return true;
            
            if (string.IsNullOrEmpty(usernameTextBox.Text.Trim()) || string.IsNullOrEmpty(passwordTextBox.Text))
                return false;

            var action = new HealthCheckAuthenticationAction(usernameTextBox.Text.Trim(), passwordTextBox.Text.Trim(),
                Registry.HealthCheckIdentityTokenDomainName, Registry.HealthCheckUploadGrantTokenDomainName,
                Registry.HealthCheckUploadTokenDomainName, Registry.HealthCheckDiagnosticDomainName, Registry.HealthCheckProductKey, 
                TokenExpiration, false);

            using (var dlg = new ActionProgressDialog(action, ProgressBarStyle.Blocks))
                dlg.ShowDialog(Parent);

            if (!action.Succeeded)
            {
                error = action.Exception != null ? action.Exception.Message : Messages.ERROR_UNKNOWN;
                UploadToken = null;
                return false;
            }
           
            UploadToken = action.UploadToken;  // current upload token
            return !string.IsNullOrEmpty(UploadToken);
        }

        private bool CheckCaseNumberValid(out string error)
        {
            error = string.Empty;

            if (!uploadCheckBox.Checked || string.IsNullOrEmpty(caseNumberTextBox.Text.Trim()))
                return true;

            ulong val;
            if (ulong.TryParse(caseNumberTextBox.Text.Trim(), out val) && val > 0 && val < 1000000000)
                return true;

            error = Messages.BUGTOOL_PAGE_DESTINATION_INVALID_CASE_NO;
            return false;
        }

        #region Control event handlers

        private void m_textBoxName_TextChanged(object sender, EventArgs e)
        {
            PerformCheck(CheckPathValid, CheckCaseNumberValid, CheckCredentialsEntered);
        }

        private void m_textBoxLocation_TextChanged(object sender, EventArgs e)
        {
            PerformCheck(CheckPathValid, CheckCaseNumberValid, CheckCredentialsEntered);
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (var dlog = new FolderBrowserDialog
            {
                SelectedPath = m_textBoxLocation.Text.Trim(),
                Description = Messages.FOLDER_BROWSER_BUG_TOOL
            })
            {
                if (dlog.ShowDialog() == DialogResult.OK)
                    m_textBoxLocation.Text = dlog.SelectedPath;
            }
        }

        private void uploadCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            PerformCheck(CheckPathValid, CheckCaseNumberValid, CheckCredentialsEntered);
        }

        private void credentials_TextChanged(object sender, EventArgs e)
        {
            uploadCheckBox.Checked = true;
            PerformCheck(CheckPathValid, CheckCaseNumberValid, CheckCredentialsEntered);
        }
        
        private void caseNumberLabelTextBox_TextChanged(object sender, EventArgs e)
        {
            uploadCheckBox.Checked = true;
            PerformCheck(CheckPathValid, CheckCaseNumberValid, CheckCredentialsEntered);
        }

        private void enterCredentialsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.enterCredentialsLinkLabel.LinkVisited = true;
            Program.OpenURL(Messages.MY_CITRIX_CREDENTIALS_URL);
        }

        #endregion
    }
}
