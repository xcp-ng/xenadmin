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
using System.Windows.Forms;
using XenCenterLib;


namespace XenAdmin.Dialogs.RestoreSession
{
    public partial class LoadSessionDialog : XenDialogBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LoadSessionDialog(bool isRetry = false)
        {
            InitializeComponent();
            passwordFailure1.Visible = isRetry;
        }

        internal override string HelpName => "LoadSessionDialog";
        public byte[] PasswordHash
        {
            get
            {
                if (!string.IsNullOrEmpty(passBox.Text))
                {
                    try
                    {
                        return EncryptionUtils.ComputeHash(passBox.Text);
                    }
                    catch (Exception exp)
                    {
                        log.Error("Could not hash entered password.", exp);
                    }
                }

                return null;
            }
        }

        private void passBox_TextChanged(object sender, EventArgs e)
        {
            passwordFailure1.Visible = false;
            okButton.Enabled = !string.IsNullOrEmpty(passBox.Text);
        }

        private void LoadSessionDialog_Load(object sender, EventArgs e)
        {
            if (Location.IsEmpty)
            {
                Screen s = Screen.FromControl(this);
                Location = new Point((s.Bounds.Width - Width) / 2, (s.Bounds.Height - Height) / 2);
            }
        }
    }
}