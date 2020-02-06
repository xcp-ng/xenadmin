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
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Network;
using XenAdmin.Core;


namespace XenAdmin.Dialogs
{
    public partial class RoleElevationDialog : XenDialogBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Session elevatedSession;
        public string elevatedPassword;
        public string elevatedUsername;
        public string originalUsername;
        public string originalPassword;

        private List<Role> authorizedRoles;

        /// <summary>
        /// Displays a dialog informing the user they need a different role to complete the task, and offers the chance to switch user. Optionally logs 
        /// out the elevated session. If successful exposes the elevated session, password and username as fields.
        /// </summary>
        /// <param name="connection">The current server connection with the role information</param>
        /// <param name="session">The session on which we have been denied access</param>
        /// <param name="authorizedRoles">A list of roles that are able to complete the task</param>
        /// <param name="actionTitle">A description of the current action, if null or empty will not be displayed</param>
        public RoleElevationDialog(IXenConnection connection, Session session, List<Role> authorizedRoles, string actionTitle)
            :base(connection)
        {
            InitializeComponent();
            UserDetails ud = session.CurrentUserDetails;
            labelCurrentUserValue.Text = ud.UserDisplayName ?? ud.UserName ?? Messages.UNKNOWN_AD_USER;
            labelCurrentRoleValue.Text = Role.FriendlyCSVRoleList(session.Roles);
            authorizedRoles.Sort((r1, r2) => r2.CompareTo(r1));
            labelRequiredRoleValue.Text = Role.FriendlyCSVRoleList(authorizedRoles);
            labelServerValue.Text = Helpers.GetName(connection);
            labelServer.Text = Helpers.IsPool(connection) ? Messages.POOL_COLON : Messages.SERVER_COLON;
            originalUsername = session.Connection.Username;
            originalPassword = session.Connection.Password;

            if (string.IsNullOrEmpty(actionTitle))
            {
                labelCurrentAction.Visible = false;
                labelCurrentActionValue.Visible = false;
            }
            else
            {
                labelCurrentActionValue.Text = actionTitle;
            }

            this.authorizedRoles = authorizedRoles;
        }

        private void buttonAuthorize_Click(object sender, EventArgs e)
        {
            try
            {
                Exception delegateException = null;
                log.Debug("Testing logging in with the new credentials");
                DelegatedAsyncAction loginAction = new DelegatedAsyncAction(connection, 
                    Messages.AUTHORIZING_USER, 
                    Messages.CREDENTIALS_CHECKING, 
                    Messages.CREDENTIALS_CHECK_COMPLETE, 
                    delegate
                {
                    try
                    {
                        elevatedSession = connection.ElevatedSession(TextBoxUsername.Text.Trim(), TextBoxPassword.Text);
                    }
                    catch (Exception ex)
                    {
                        delegateException = ex;
                    }
                });

                using (var dlg = new ActionProgressDialog(loginAction, ProgressBarStyle.Marquee, false))
                    dlg.ShowDialog(this);
                
                // The exception would have been handled by the action progress dialog, just return the user to the sudo dialog
                if (loginAction.Exception != null)
                    return;

                if(HandledAnyDelegateException(delegateException))
                    return;

                if (elevatedSession.IsLocalSuperuser || SessionAuthorized(elevatedSession))
                {
                    elevatedUsername = TextBoxUsername.Text.Trim();
                    elevatedPassword = TextBoxPassword.Text;
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }

                ShowNotAuthorisedDialog();
            }
            catch (Exception ex)
            {
                log.DebugFormat("Exception when attempting to sudo action: {0} ", ex);
                using (var dlg = new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                       SystemIcons.Error,
                       String.Format(Messages.USER_AUTHORIZATION_FAILED, TextBoxUsername.Text),
                       Messages.XENCENTER)))
                {
                    dlg.ShowDialog(Parent);
                }

                TextBoxPassword.Focus();
                TextBoxPassword.SelectAll();
            }
            finally
            {
                // Check whether we have a successful elevated session and whether we have been asked to log it out
                // If non successful (most likely the new subject is not authorized) then log it out anyway. 
                if (elevatedSession != null && DialogResult != DialogResult.OK)
                {
                    elevatedSession.Connection.Logout(elevatedSession);
                    elevatedSession = null;
                }
            }
            
        }

        private bool HandledAnyDelegateException(Exception delegateException)
        {
            if (delegateException != null)
            {
                Failure f = delegateException as Failure;
                if (f != null && f.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                {
                    ShowNotAuthorisedDialog();
                    return true;
                }

                throw delegateException;
            }
            return false;
        }


        private void ShowNotAuthorisedDialog()
        {
            using (var dlg = new ThreeButtonDialog(
                new ThreeButtonDialog.Details(
                    SystemIcons.Error,
                    Messages.USER_NOT_AUTHORIZED,
                    Messages.PERMISSION_DENIED)))
            {
                dlg.ShowDialog(this);
            }

            TextBoxPassword.Focus();
            TextBoxPassword.SelectAll();
        }

        private bool SessionAuthorized(Session s)
        {
            UserDetails ud = s.CurrentUserDetails;
            foreach (Role r in s.Roles)
            {
                if (authorizedRoles.Contains(r))
                {
                   
                    log.DebugFormat("Subject '{0}' is authorized to complete the action", ud.UserDisplayName ?? ud.UserName ?? ud.UserSid);
                    return true;
                }
            }
            log.DebugFormat("Subject '{0}' is not authorized to complete the action", ud.UserDisplayName ?? ud.UserName ?? ud.UserSid);
            return false;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {   
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void UpdateButtons()
        {
            buttonAuthorize.Enabled = TextBoxUsername.Text.Trim() != "" && TextBoxPassword.Text != "";
        }

        private void TextBoxUsername_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void TextBoxPassword_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }
    }
}