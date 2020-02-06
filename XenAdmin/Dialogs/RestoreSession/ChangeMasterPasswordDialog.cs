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
using XenCenterLib;


namespace XenAdmin.Dialogs.RestoreSession
{
    public partial class ChangeMasterPasswordDialog : XenDialogBase
    {
        private readonly byte[] OldProposedPassword;

        public ChangeMasterPasswordDialog(byte[] proposedPassword)
        {
            InitializeComponent();
            OldProposedPassword = proposedPassword;
            currentPasswordError.Visible = false;
            newPasswordError.Visible = false;
        }

        public byte[] NewPassword { get; private set; }

        private void currentTextBox_TextChanged(object sender, EventArgs e)
        {
            currentPasswordError.Visible = false;
        }

        private void masterTextBox_TextChanged(object sender, EventArgs e)
        {
            newPasswordError.Visible = false;
        }

        private void reEnterMasterTextBox_TextChanged(object sender, EventArgs e)
        {
            newPasswordError.Visible = false;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            var oldPasswordCorrect = Settings.PassCorrect(currentTextBox.Text, OldProposedPassword);

            if (oldPasswordCorrect && !string.IsNullOrEmpty(masterTextBox.Text) &&
                masterTextBox.Text == reEnterMasterTextBox.Text)
            {
                NewPassword = EncryptionUtils.ComputeHash(masterTextBox.Text);
                DialogResult = DialogResult.OK;
                return;
            }

            if (!oldPasswordCorrect)
                currentPasswordError.ShowError(Messages.PASSWORD_INCORRECT);
            else if (masterTextBox.Text != reEnterMasterTextBox.Text)
                newPasswordError.ShowError(Messages.PASSWORDS_DONT_MATCH);
            else
                newPasswordError.ShowError(Messages.PASSWORDS_EMPTY);

            currentTextBox.Focus();
            currentTextBox.SelectAll();
        }
    }
}
