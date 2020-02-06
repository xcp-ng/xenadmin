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

using System.Drawing;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Network;


namespace XenAdmin.Controls
{
    public partial class LoggedInLabel : UserControl
    {
        private IXenConnection connection;
        public IXenConnection Connection
        {
            get
            {
                return connection;
            }
            set
            {
                // Check if we need to bother updating
                if (value == connection)
                    return;

                // if the old value was not null then we need to deregister the event handlers
                if (connection != null)
                {
                    connection.ConnectionStateChanged -= connection_ConnectionStateChanged;
                    connection.CachePopulated -= connection_CachePopulated;
                }

                // Now set to the new value, if it's not null then we set the labels and tooltip relevant to the new connection
                connection = value;
                if (connection != null)
                {
                    // If the current connection disconnects we need to clear the labels
                    connection.ConnectionStateChanged += connection_ConnectionStateChanged;
                    // if the cache isn't populated yet we can clear the lables and update later off this event handler
                    connection.CachePopulated += connection_CachePopulated;
                    if (connection.CacheIsPopulated)
                    {
                        setLabelText();
                        return;
                    }
                }
                // clear labels
                labelUsername.Text = "";
                labelLoggedInAs.Visible = false;
            }
        }

        void connection_ConnectionStateChanged(IXenConnection conn)
        {
            Program.Invoke(Program.MainWindow, setLabelText);
        }

        void connection_CachePopulated(IXenConnection conn)
        {
            Program.Invoke(Program.MainWindow, setLabelText);
        }

        private void setLabelText()
        {
            // If we are connecting we should actually only update when the cache is populated. 
            if (connection == null || connection.Session == null || !connection.IsConnected || !connection.CacheIsPopulated)
            {
                labelUsername.Text = "";
                labelLoggedInAs.Visible = false;
                return;
            }        
            labelLoggedInAs.Visible = true;

            // get the logged in username from the session to update the logged in label
            if (connection.Session.IsLocalSuperuser || XenAdmin.Core.Helpers.GetMaster(connection).external_auth_type != Auth.AUTH_TYPE_AD)
            {
                labelUsername.Text = connection.Session.UserFriendlyName();
            }
            else
            {
                labelUsername.Text = string.Format("{0} ({1})", 
                    connection.Session.UserFriendlyName(), 
                    connection.Session.FriendlySingleRoleDescription());
            }
        }

        public LoggedInLabel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets all the labels on the control to use this text color
        /// </summary>
        /// <param name="c"></param>
        public void SetTextColor(Color c)
        {
            labelLoggedInAs.ForeColor = c;
            labelUsername.ForeColor = c;
        }
    }
}
