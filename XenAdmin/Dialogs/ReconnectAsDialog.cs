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
using XenAdmin.Network;
using XenAdmin.Core;
using XenAdmin.Commands;
using System.Timers;


namespace XenAdmin.Dialogs
{
    public partial class ReconnectAsDialog : XenDialogBase
    {
        // Do not use the connection on XenDialogBase as setting it will register us to be closed when we dc
        private readonly IXenConnection xc;

        public ReconnectAsDialog(IXenConnection connection)
        {
            InitializeComponent();
            xc = connection;
            labelBlurb.Text = String.Format(Messages.RECONNECT_AS_BLURB, Helpers.GetName(connection).Ellipsise(30), connection.Session.UserFriendlyName().Ellipsise(30));
        }

        private void SetButtonEnablement()
        {
            buttonOK.Enabled = !String.IsNullOrEmpty(textBoxPassword.Text) && !String.IsNullOrEmpty(textBoxUsername.Text.Trim());
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            xc.Username = textBoxUsername.Text.Trim();
            xc.Password = textBoxPassword.Text;
            xc.ExpectPasswordIsCorrect = true;
            // start logout then wait for connection to become disconnected
            xc.ConnectionStateChanged += xc_ConnectionStateChanged;

            if (!new DisconnectCommand(Program.MainWindow, xc, true).Execute())
            {
                // User wimped out
                xc.ConnectionStateChanged -= xc_ConnectionStateChanged;
            }
            Close();
        }

        private void xc_ConnectionStateChanged(IXenConnection conn)
        {
            if (xc.IsConnected)
                return;

            xc.ConnectionStateChanged -= xc_ConnectionStateChanged;
            Timer t = new Timer(500);
            t.AutoReset = false;
            t.Elapsed += t_Elapsed;
            t.Start();
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            Program.Invoke(Program.MainWindow, delegate
            {
                xc.CachePopulated += xc_CachePopulated;
                XenConnectionUI.BeginConnect(xc, true, Program.MainWindow, true);
            });  
        }

        void xc_CachePopulated(IXenConnection conn)
        {
            xc.CachePopulated -= xc_CachePopulated;
            Program.MainWindow.TrySelectNewObjectInTree(xc, true, true, false);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBoxUsername_TextChanged(object sender, EventArgs e)
        {
            SetButtonEnablement();
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            SetButtonEnablement();
        }
    }
}