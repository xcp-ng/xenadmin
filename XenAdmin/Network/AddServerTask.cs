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
using System.Windows.Forms;

using XenAPI;

using XenAdmin.Dialogs;

namespace XenAdmin.Network
{
    class AddServerTask
    {
        private readonly IWin32Window _parentForm;

        /// <summary>
        /// May be null, in which case a new connection will be created by the AddServerDialog.
        /// </summary>
        private readonly IXenConnection _connection;
        
        /// <summary>
        /// The pool that this new connection is destined to be added to. May be set by anyone;
        /// passed to any ConnectedEventHandlers on the ServerConnected event.  May be null.
        /// </summary>
        private readonly Pool _destinationPool;

        public AddServerTask(IWin32Window parentForm, IXenConnection connection, Pool destinationPool)
        {
            _parentForm = parentForm;
            _connection = connection;
            _destinationPool = destinationPool;
        }

        public void Start()
        {
            AddServerDialog dialog = new AddServerDialog(_connection, false);
            dialog.FormClosing += formClosing;
            dialog.Show(_parentForm);
        }

        private void formClosing(object sender, FormClosingEventArgs e)
        {
            Form dialog = (Form)sender;

            if (dialog.DialogResult == DialogResult.Cancel)
            {
                if (_connection != null)
                {
                    _connection.EndConnect();
                }
            }
        }
    }
}
