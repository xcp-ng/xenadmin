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
using System.Windows.Forms;


namespace XenAdmin.Dialogs
{
    public partial class AdPasswordPrompt : XenDialogBase
    {
        private string helpName;

        public string Domain
        {
            get { return textBoxDomain.Text.Trim(); }
            set { textBoxDomain.Text = value ?? string.Empty; }
        }

        public string Username
        {
            get { return textBoxUsername.Text.Trim(); }
            set { textBoxUsername.Text = value ?? string.Empty; }
        }

        public string Password
        {
            get
            {
                return textBoxPassword.Text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="join">If true, we're joining a domain; if the domain is unknown (currentDomainName null or empty)
        /// prompt for domain, username and password; if it's known, only for username and password.
        /// If false, we're removing the machine from the domain; prompt for credentials to disable the account on
        /// the AD server, giving options 'Disable', 'Ignore' and 'Cancel'.</param>
        /// <param name="currentDomainName">The current domain name to populate the dialog with. Maybe null or empty if
        /// joining an unknown domain. Should not be null or empty when leaving a domain.</param>
        public AdPasswordPrompt(bool join, string currentDomainName = null)
        {
            InitializeComponent();

            if (join)
            {
                bool promptForDomain = string.IsNullOrEmpty(currentDomainName);

                labelBlurb.Text = promptForDomain
                      ? Messages.AD_JOIN_DOMAIN_BLURB_SHORT
                      : string.Format(Messages.JOINING_AD, currentDomainName);

                label1.Visible = promptForDomain;
                textBoxDomain.Visible = promptForDomain;
                SkipButton.Visible = false;
                helpName = Name + "Enable";
            }
            else
            {
                System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(currentDomainName));

                Text = Messages.AD_DISABLING;
                labelBlurb.Text = Messages.LEAVING_AD;
                textBoxDomain.Text = currentDomainName;
                textBoxDomain.Enabled = false;
                labelAdditionalInfo.Visible = true;
                labelAdditionalInfo.Text = Messages.AD_LEAVING_ADDITIONAL_BLURB;
                buttonOk.Text = Messages.DISABLE;
                helpName = Name + "Disable";
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void textBoxDomain_TextChanged(object sender, EventArgs e)
        {
            EnableOkButton();
        }

        private void textBoxUsername_TextChanged(object sender, EventArgs e)
        {
            EnableOkButton();
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            EnableOkButton();
        }

        private void SkipButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Ignore;
            Close();
        }

        private void EnableOkButton()
        {
            buttonOk.Enabled = textBoxDomain.Visible
                                   ? textBoxDomain.Text.Trim().Length > 0 && textBoxUsername.Text.Trim().Length > 0 && textBoxPassword.Text.Trim().Length > 0
                                   : textBoxUsername.Text.Trim().Length > 0;
        }

        internal override string HelpName
        {
            get
            {
                return helpName;
            }
        }
    }
}
