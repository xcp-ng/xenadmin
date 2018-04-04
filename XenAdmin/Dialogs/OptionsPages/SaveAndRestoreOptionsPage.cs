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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs.RestoreSession;
using XenAdmin.Properties;


namespace XenAdmin.Dialogs.OptionsPages
{
    /// <summary>
    /// The page is used to set whether or not to save server usernames and passwords and whether a master password should be set to protect these passwords
    /// </summary>
    public partial class SaveAndRestoreOptionsPage : UserControl, IOptionsPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private byte[] TemporaryMasterPassword;
        // call save serverlist on OK
        protected internal bool SaveAllAfter { get; set; }

        public SaveAndRestoreOptionsPage() 
        {
            InitializeComponent();
            // setup all the controls with the current state of the settings
            FillCurrentSettings();
        }

        public static void Log()
        {
            // SSL Certificates
            log.Info("=== SaveSession: " + Properties.Settings.Default.SaveSession.ToString());
            log.Info("=== RequirePass: " + Properties.Settings.Default.RequirePass.ToString());
        }

        // all prompts for old password should have been made
        private void SaveEverything()
        {
            if (!Registry.AllowCredentialSave)
            {
                return;
            }
            if (!saveStateCheckBox.Checked)
            {
                // save nothing and nobody (personally my two favourite servers anyway...)
                Properties.Settings.Default.SaveSession = false;
                Properties.Settings.Default.RequirePass = false;

                Program.MasterPassword = null;
            }
            else if (!requireMasterPasswordCheckBox.Checked)
            {
                // we need to save stuff but without a password
                Properties.Settings.Default.SaveSession = true;
                Properties.Settings.Default.RequirePass = false;

                Program.MasterPassword = null;
            }
            else
            {
                // password protect stuff
                Properties.Settings.Default.SaveSession = true;
                Properties.Settings.Default.RequirePass = true;

                // set password
                if (Program.MasterPassword != TemporaryMasterPassword) 
                {
                    Program.MasterPassword = TemporaryMasterPassword;
                    new ActionBase(Messages.CHANGED_MASTER_PASSWORD,
                        Messages.CHANGED_MASTER_PASSWORD_LONG, false, true);
                }
            }
            if (SaveAllAfter)
                Settings.SaveServerList();
        }
        
        #region Control event handlers

        private void changeMasterPasswordButton_Click(object sender, EventArgs e)
        {
            // tell the dialog what to use as the "current" password
            using (var changePassword = new ChangeMasterPasswordDialog(TemporaryMasterPassword))
            {
                if (changePassword.ShowDialog(this) == DialogResult.OK)
                {
                    // password has been successfully changed
                    TemporaryMasterPassword = changePassword.NewPassword;
                }
            }
        }

        private void requireMasterPasswordCheckBox_Click(object sender, EventArgs e)
        {
            // requireMasterPasswordCheckBox.Checked was the state before the click
            // if previously checked, the user is trying to clear it => request authorization
            // if previously unchecked, the user is trying to set a password

            if (requireMasterPasswordCheckBox.Checked)
            {
                using (var enterPassword = new EnterMasterPasswordDialog(TemporaryMasterPassword))
                {
                    if (enterPassword.ShowDialog(this) == DialogResult.OK)
                    {
                        TemporaryMasterPassword = null;
                        requireMasterPasswordCheckBox.Checked = false;
                        changeMasterPasswordButton.Enabled = false;
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.Assert(TemporaryMasterPassword == null, "Master password is set, but not reflected on GUI");

                if (TemporaryMasterPassword == null)
                {
                    // no previous password existed => set a new one
                    using (var setPassword = new SetMasterPasswordDialog())
                    {
                        if (setPassword.ShowDialog(this) == DialogResult.OK)
                        {
                            TemporaryMasterPassword = setPassword.NewPassword;
                            requireMasterPasswordCheckBox.Checked = true;
                            changeMasterPasswordButton.Enabled = true;
                        }
                    }
                }
                else
                {
                    // a previous password existed (should never get here but just in case)
                    // enable button to facilitate password change
                    requireMasterPasswordCheckBox.Checked = true;
                    changeMasterPasswordButton.Enabled = true;
                }
            }
        }

        private void saveStateCheckBox_Click(object sender, EventArgs e)
        {
            // need to prevent the user from going to an open terminal and clearing
            // the save state, then setting the master password to anything they like

            // saveStateCheckBox.Checked was the state before the click
            // if previously checked, the user is trying to clear it => authorization maybe required
            // (depending on the state of the requireMasterPasswordCheckBox; this should be cleared too if checked)

            if (saveStateCheckBox.Checked && requireMasterPasswordCheckBox.Checked)
            {
                using (var enterPassword = new EnterMasterPasswordDialog(TemporaryMasterPassword))
                {
                    if (enterPassword.ShowDialog(this) == DialogResult.OK)
                    {
                        TemporaryMasterPassword = null;
                        saveStateCheckBox.Checked = false;
                        requireMasterPasswordCheckBox.Checked = false;
                        masterPasswordGroupBox.Enabled = false;
                    }
                }
            }
            else
            {
                saveStateCheckBox.Checked = !saveStateCheckBox.Checked;
                masterPasswordGroupBox.Enabled = saveStateCheckBox.Checked;
                changeMasterPasswordButton.Enabled = requireMasterPasswordCheckBox.Checked;
            }
        }

        #endregion

        private void FillCurrentSettings()
        {
            bool allowCredSave = Registry.AllowCredentialSave;
            bool saveSession = Properties.Settings.Default.SaveSession;
            bool reqPass = Properties.Settings.Default.RequirePass;

            saveStateLabel.Enabled = allowCredSave;
            saveStateCheckBox.Enabled = allowCredSave;
            
            // use the SaveSession variable to denote whether to save passwords or not
            saveStateCheckBox.Checked = saveSession && allowCredSave;
            masterPasswordGroupBox.Enabled = saveSession && allowCredSave;
            
            // use the RequirePass variable to say if a master password has been set
            requireMasterPasswordCheckBox.Checked = reqPass && Program.MasterPassword != null && allowCredSave;
            changeMasterPasswordButton.Enabled = reqPass && Program.MasterPassword != null && allowCredSave;
            
            // the temporary password starts as the MasterPassword
            TemporaryMasterPassword = Program.MasterPassword;
        }

        #region IOptionsPage Members

        public void Save()
        {
            SaveEverything();
        }

        #endregion

        #region IVerticalTab Members

        public override string Text
        {
            get { return Messages.SAVE_AND_RESTORE; }
        }

        public string SubText
        {
            get { return Messages.SAVE_AND_RESTORE_DESC; }
        }

        public Image Image
        {
            get { return Resources._000_BackupMetadata_h32bit_16; }
        }

        #endregion
    }
}
