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
using XenAdmin.Network;


namespace XenAdmin.Dialogs
{
    public partial class ConnectingToServerDialog : XenDialogBase
    {
        private Form ownerForm;
        private bool endBegun;

        public ConnectingToServerDialog(IXenConnection connection)
            : base(connection)
        {
            InitializeComponent();
            Icon = Properties.Resources.AppIcon;
            lblStatus.Text = string.Format(Messages.LABEL_ATTEMPT, connection.Hostname);
        }

        internal override string HelpName => "AddServerDialog";

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (connection != null && connection.InProgress && !connection.IsConnected && !endBegun)
            {
                //if we are still trying to connect then we need to end it. This call will close this dialog as well,
                //so set a guard to skip this block when we re-enter.
                endBegun = true;
                connection.EndConnect();
                e.Cancel = true;

                //CA-65004: if this dialog is closed by closing the parent form, returning
                //without calling the base class method causes the parent form to stay open
                if (e.CloseReason == CloseReason.FormOwnerClosing && GetOwnerForm() == ownerForm)
                    ownerForm.Close();
                return;
            }

            if (connection != null)
                XenConnectionUI.connectionDialogs.Remove(connection);

            base.OnFormClosing(e);
        }

        /// <summary>
        /// Gets the Form set as this Connection's owner in BeginConnect. All the dialogs spawned during the connection attempt
        /// are displayed with this Form as their Owner. If no Owner has been set, will return Program.MainWindow.
        /// </summary>
        public Form GetOwnerForm()
        {
            return ownerForm != null && !ownerForm.IsDisposed && !ownerForm.Disposing && !Program.Exiting
                ? ownerForm
                : Program.MainWindow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner">The form that connecting dialogs will be displayed in front of.
        /// May be null, in which case Program.MainWindow is used.</param>
        /// <param name="initiateMasterSearch">If true, if connection to the master fails we will start trying to connect to
        /// each remembered slave in turn.</param>
        internal bool BeginConnect(Form owner, bool initiateMasterSearch)
        {
            if (connection is XenConnection conn)
            {
                ownerForm = owner;

                RegisterEventHandlers();
                conn.BeginConnect(initiateMasterSearch, HideAndPromptForNewPassword);

                if (conn.InProgress && !IsDisposed && !Disposing && !Program.Exiting) //CA-328267
                {
                    if (!Visible)
                        Show(GetOwnerForm());

                    Focus();
                    return true;
                }
            }

            return false;
        }

        private void RegisterEventHandlers()
        {
            connection.ConnectionClosed += Connection_ConnectionClosed;
            connection.BeforeConnectionEnd += Connection_BeforeConnectionEnd;
            connection.ConnectionMessageChanged += Connection_ConnectionMessageChanged;
        }

        private void UnregisterEventHandlers()
        {
            connection.ConnectionClosed -= Connection_ConnectionClosed;
            connection.BeforeConnectionEnd -= Connection_BeforeConnectionEnd;
            connection.ConnectionMessageChanged -= Connection_ConnectionMessageChanged;
        }

        private void Connection_ConnectionClosed(IXenConnection conn)
        {
            CloseConnectingDialog();
        }

        private void Connection_BeforeConnectionEnd(IXenConnection conn)
        {
            CloseConnectingDialog();
        }

        private void Connection_ConnectionMessageChanged(IXenConnection conn, string message)
        {
            Program.Invoke(Program.MainWindow, () =>
            {
                if (Visible)
                    lblStatus.Text = message;
            });
        }

        public void CloseConnectingDialog()
        {
            UnregisterEventHandlers();

            Program.Invoke(Program.MainWindow, () =>
            {
                if (Visible)
                    OwnerActivatedOnClosed = false;

                Close();
            });
        }

        private bool HideAndPromptForNewPassword(IXenConnection xenConnection, string oldPassword)
        {
            bool result = false;
            Program.Invoke(Program.MainWindow, () =>
            {
                bool wasVisible = Visible;
                if (wasVisible)
                    Hide();

                // show an altered version of the add server dialog with the hostname greyed out
                using (var dialog = new AddServerDialog(xenConnection, true))
                    result = dialog.ShowDialog(GetOwnerForm()) == DialogResult.OK;

                if (result && wasVisible)
                    Show(GetOwnerForm());
            });
            return result;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
