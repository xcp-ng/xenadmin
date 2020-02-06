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
using System.Net.Sockets;
using System.Threading;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Dialogs;
using System.Drawing;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Removes the selected host from its pool.
    /// </summary>
    internal class RemoveHostFromPoolCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public RemoveHostFromPoolCommand()
        {
        }

        public RemoveHostFromPoolCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public RemoveHostFromPoolCommand(IMainWindow mainWindow, Host host)
            : base(mainWindow, host)
        {
        }

        public RemoveHostFromPoolCommand(IMainWindow mainWindow, IEnumerable<Host> hosts)
            : base(mainWindow, ConvertToSelection<Host>(hosts))
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<AsyncAction> actions = new List<AsyncAction>();
            foreach (Host host in selection.AsXenObjects<Host>())
            {
                string opaque_ref = host.opaque_ref;
                Pool pool = Helpers.GetPool(host.Connection);

                if (selection.Count == 1 && pool.master == host.opaque_ref)
                {
                    // Trying to remove the master from a pool.
                    using (var dlg = new ThreeButtonDialog(
                       new ThreeButtonDialog.Details(
                           SystemIcons.Error,
                           Messages.MESSAGEBOX_POOL_MASTER_REMOVE,
                           Messages.XENCENTER)))
                    {
                        dlg.ShowDialog(MainWindowCommandInterface.Form);
                    }
                    return;
                }

                // Optimistically, add the ejected host as a standalone host (but don't connect to it yet)
                IXenConnection connection = new XenConnection();
                connection.Hostname = host.address;
                connection.Username = host.Connection.Username;
                connection.Password = host.Connection.Password;
                connection.FriendlyName = host.Name();

                lock (ConnectionsManager.ConnectionsLock)
                {
                    ConnectionsManager.XenConnections.Add(connection);
                }

                Program.HideObject(opaque_ref);

                var action = new EjectHostFromPoolAction(pool, host);
                action.Completed += delegate
                {
                    if (action.Succeeded)
                    {
                        ThreadPool.QueueUserWorkItem(WaitForReboot, connection);
                    }
                    else
                    {
                        Program.ShowObject(opaque_ref);
                        MainWindowCommandInterface.RemoveConnection(connection);
                    }
                };

                actions.Add(action);
            }

            RunMultipleActions(actions, Messages.REMOVING_SERVERS_FROM_POOL, Messages.POOLCREATE_REMOVING, Messages.POOLCREATE_REMOVED, true);
        }

        public static bool CanExecute(Host host)
        {
            if (host != null && host.Connection != null)
            {
                Pool pool = Helpers.GetPool(host.Connection);
                return pool != null && host.opaque_ref != pool.master && host.resident_VMs != null && host.resident_VMs.Count < 2 && host.IsLive();
            }
            return false;
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.AllItemsAre<Host>())
            {
                // all hosts must be in same pool.
                Pool commonPool = null;
                foreach (Host host in selection.AsXenObjects<Host>())
                {
                    if (!CanExecute(host))
                    {
                        return false;
                    }
                    Pool pool = Helpers.GetPool(host.Connection);
                    if (commonPool != null && !pool.Equals(commonPool))
                    {
                        return false;
                    }
                    commonPool = pool;
                }
                return true;
            }
            return false;
        }

        private void WaitForReboot(object o)
        {
            while (true)
            {
                IXenConnection connection = (IXenConnection)o;
                Thread.Sleep(30 * 1000);           // wait 30s for server to shutdown
                int i = 0;
                int max = 27;                                       // giveup after 5 mins
                while (true)
                {
                    if (i > max)
                    {
                        MainWindowCommandInterface.Invoke(delegate
                        {
                            using (var dlg = new ThreeButtonDialog(
                               new ThreeButtonDialog.Details(
                                   SystemIcons.Exclamation,
                                   string.Format(Messages.MESSAGEBOX_RECONNECT_FAIL, connection.Hostname),
                                   Messages.MESSAGEBOX_RECONNECT_FAIL_TITLE)))
                            {
                                dlg.ShowDialog(Parent);
                            }
                        });
                        return;
                    }

                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    try
                    {
                        socket.Connect(connection.Hostname, connection.Port);
                    }
                    catch (Exception)
                    {

                    }

                    if (socket.Connected)
                    {
                        MainWindowCommandInterface.Invoke(() => XenConnectionUI.BeginConnect(connection, false, Program.MainWindow, false));
                        return;
                    }
                    i++;
                    Thread.Sleep(15 * 1000);
                }
            }
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.REMOVE_SERVER_FROM_POOL_CONTEXT_MENU_ITEM_TEXT;
            }
        }

        protected override bool ConfirmationRequired
        {
            get
            {
                return true;
            }
        }

        protected override string ConfirmationDialogText
        {
            get
            {
                SelectedItemCollection selection = GetSelection();

                if (selection.Count > 0)
                {
                    Host host = (Host)selection[0].XenObject;
                    Pool pool = Helpers.GetPool(host.Connection);
                    
                    if (selection.Count == 1)
                    {
                        return string.Format(Messages.MAINWINDOW_CONFIRM_REMOVE_FROM_POOL, host.Name().Ellipsise(500),
                                             Helpers.GetName(pool).Ellipsise(500), host.Name().Ellipsise(500));
                    }

                    return string.Format(Messages.MAINWINDOW_CONFIRM_REMOVE_FROM_POOL_MULTIPLE,
                                         Helpers.GetName(pool).Ellipsise(500));
                }
                return null;
            }
        }

        protected override string ConfirmationDialogTitle
        {
            get
            {
                return Messages.MAINWINDOW_CONFIRM_REMOVE_FROM_POOL_TITLE;
            }
        }

        protected override string ConfirmationDialogYesButtonLabel
        {
            get
            {
                return Messages.MAINWINDOW_CONFIRM_REMOVE_FROM_POOL_YES_BUTTON_LABEL;
            }
        }

        protected override bool ConfirmationDialogNoButtonSelected
        {
            get
            {
                return true;
            }
        }
    }
}
