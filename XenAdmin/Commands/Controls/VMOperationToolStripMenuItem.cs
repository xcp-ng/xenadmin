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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Actions;
using XenAdmin.Network;



namespace XenAdmin.Commands
{
    /// <summary>
    /// This is the base ToolStripMenuItem for StartVMOnHostToolStripMenuItem, ResumeVMOnHostToolStripMenuItem and MigrateVMToolStripMenuItem.
    /// </summary>
    internal abstract class VMOperationToolStripMenuItem : CommandToolStripMenuItem
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly vm_operations _operation;
        private readonly bool _resumeAfter;

        protected VMOperationToolStripMenuItem(Command command, bool inContextMenu, vm_operations operation)
            : base(command, inContextMenu)
        {
            if (operation != vm_operations.start_on && operation != vm_operations.resume_on && operation != vm_operations.pool_migrate)
            {
                throw new ArgumentException("Invalid operation", "operation");
            }

            if (operation.Equals(vm_operations.resume_on))
                _resumeAfter = true;

            _operation = operation;
            base.DropDownItems.Add(new ToolStripMenuItem());
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            Stop();
        }


        protected override void OnDropDownOpening(EventArgs e)
        {
            base.DropDownItems.Clear();

            // Work around bug in tool kit where disabled menu items show their dropdown menus
            if (!Enabled)
            {
                ToolStripMenuItem emptyMenuItem = new ToolStripMenuItem(Messages.HOST_MENU_EMPTY);
                emptyMenuItem.Font = Program.DefaultFont;
                emptyMenuItem.Enabled = false;
                base.DropDownItems.Add(emptyMenuItem);
                return;
            }

            VisualMenuItemAlignData.ParentStrip = this;
            var selection = Command.GetSelection();
            IXenConnection connection = selection[0].Connection;
            bool wlb = Helpers.WlbEnabled(connection);

            if (wlb)
            {
                base.DropDownItems.Add(new VMOperationToolStripMenuSubItem(Messages.WLB_OPT_MENU_OPTIMAL_SERVER, Images.StaticImages._000_ServerWlb_h32bit_16));
            }
            else
            {
                base.DropDownItems.Add(new VMOperationToolStripMenuSubItem(Messages.HOME_SERVER_MENU_ITEM, Images.StaticImages._000_ServerHome_h32bit_16));
            }

            List<Host> hosts = new List<Host>(connection.Cache.Hosts);
            hosts.Sort();
            foreach (Host host in hosts)
            {
                VMOperationToolStripMenuSubItem item = new VMOperationToolStripMenuSubItem(String.Format(Messages.MAINWINDOW_CONTEXT_UPDATING, host.name_label.EscapeAmpersands()), Images.StaticImages._000_ServerDisconnected_h32bit_16);
                item.Tag = host;
                base.DropDownItems.Add(item);
            }

            // Adds the migrate wizard button, do this before the enable checks on the other items
            AddAdditionalMenuItems(selection);
            
            UpdateHostList();
        }

        /// <summary>
        /// Hook to add additional members to the menu item
        /// Note: Called on main window thread by executing code
        /// </summary>
        protected virtual void AddAdditionalMenuItems(SelectedItemCollection selection) { return; }

        #region UpdateHostList

        private ProduceConsumerQueue workerQueueWithoutWlb;
        readonly object locker = new object();
        private bool stopped;

        private bool Stopped
        {
            set
            {
                lock (locker)
                {
                    stopped = value;
                }
            }
            get
            {
                lock (locker)
                {
                    return stopped;
                }
            }
        }

        private void Stop()
        {
            Stopped = true;
            if (workerQueueWithoutWlb != null)
                workerQueueWithoutWlb.CancelWorkers(false);
        }

        private void UpdateHostList()
        {
            Stopped = false;

            var selection = Command.GetSelection();
            var connection = selection[0].Connection;

            if (Helpers.WlbEnabled(connection))
            {
                var vms = selection.AsXenObjects<VM>();
                var retrieveVmRecommendationsAction = new WlbRetrieveVmRecommendationsAction(connection, vms);
                retrieveVmRecommendationsAction.Completed += delegate
                {
                    if (Stopped || retrieveVmRecommendationsAction.Cancelled ||
                        !retrieveVmRecommendationsAction.Succeeded)
                        return;

                    var recommendations = new WlbRecommendations(vms, retrieveVmRecommendationsAction.Recommendations);

                    Program.Invoke(Program.MainWindow, delegate
                    {
                        if (recommendations.IsError)
                            EnableAppropriateHostsNoWlb();
                        else
                            EnableAppropriateHostsWlb(recommendations);
                    });
                };
                retrieveVmRecommendationsAction.RunAsync();
            }
            else
            {
                EnableAppropriateHostsNoWlb();
            }
        }

        private void EnableAppropriateHostsWlb(WlbRecommendations recommendations)
        {
            if (Stopped || DropDownItems.Count == 0)
                return;

            // set the first menu item to be the WLB optimal server menu item
            var firstItem = DropDownItems[0] as VMOperationToolStripMenuSubItem;
            if (firstItem == null)
                return;

            var selection = Command.GetSelection();

            var firstItemCmd = new VMOperationWlbOptimalServerCommand(Command.MainWindowCommandInterface,
                selection, _operation, recommendations);

            firstItem.Command = firstItemCmd;
            firstItem.Enabled = firstItemCmd.CanExecute();

            var hostMenuItems = new List<VMOperationToolStripMenuSubItem>();
            foreach (var item in DropDownItems)
            {
                var hostMenuItem = item as VMOperationToolStripMenuSubItem;
                if (hostMenuItem == null)
                    continue;

                var host = hostMenuItem.Tag as Host;
                if (host != null)
                {
                    var cmd = new VMOperationWlbHostCommand(Command.MainWindowCommandInterface, selection, host,
                        _operation, recommendations.GetStarRating(host));

                    hostMenuItem.Command = cmd;
                    hostMenuItem.Enabled = cmd.CanExecute();

                    hostMenuItems.Add(hostMenuItem);
                }
            }

            // sort the hostMenuItems by star rating
            hostMenuItems.Sort(new WlbHostStarCompare());

            // refresh the drop-down-items from the menuItems.
            foreach (VMOperationToolStripMenuSubItem menuItem in hostMenuItems)
            {
                DropDownItems.Insert(hostMenuItems.IndexOf(menuItem) + 1, menuItem);
            }
        }

        private void EnableAppropriateHostsNoWlb()
        {
            if (Stopped || DropDownItems.Count == 0)
                return;

            var firstItem = DropDownItems[0] as VMOperationToolStripMenuSubItem;
            if (firstItem == null)
                return;
                
            // API calls could happen in CanExecute(), which take time to wait. So a Producer-Consumer-Queue with size 25 is used here to :
            //   1. Make API calls for different menu items happen in parallel;
            //   2. Limit the count of concurrent threads (now it's 25).
            workerQueueWithoutWlb = new ProduceConsumerQueue(25);

            var selection = Command.GetSelection();
            var connection = selection[0].Connection;
            var session = connection.DuplicateSession();

            var affinityHost = connection.Resolve(((VM) selection[0].XenObject).affinity);

            EnqueueHostMenuItem(this, session, affinityHost, firstItem, true);

            var hostMenuItems = DropDownItems.OfType<VMOperationToolStripMenuSubItem>().ToList();

            if (Stopped)
                return;

            foreach (VMOperationToolStripMenuSubItem item in hostMenuItems)
            {
                var host = item.Tag as Host;
                if (host != null)
                {
                    var tempItem = item;
                    EnqueueHostMenuItem(this, session, host, tempItem, false);
                }
            }
        }

        private void EnqueueHostMenuItem(VMOperationToolStripMenuItem menu, Session session, Host host, VMOperationToolStripMenuSubItem hostMenuItem, bool isHomeServer)
        {
            workerQueueWithoutWlb.EnqueueItem(() =>
            {
                var selection = menu.Command.GetSelection();
                var cmd = isHomeServer 
                    ? new VMOperationHomeServerCommand(menu.Command.MainWindowCommandInterface, selection, menu._operation, session)
                    : new VMOperationHostCommand(menu.Command.MainWindowCommandInterface, selection, delegate { return host; }, host.Name().EscapeAmpersands(), menu._operation, session);

                var oldMigrateCmdCanRun = cmd.CanExecute();
                if (Stopped)
                    return;

                if (host == null || menu._operation == vm_operations.start_on || oldMigrateCmdCanRun)
                {
                    Program.Invoke(Program.MainWindow, delegate
                    {
                        hostMenuItem.Command = cmd;
                        hostMenuItem.Enabled = oldMigrateCmdCanRun;
                    });
                }
                else
                {
                    var cpmCmd = isHomeServer
                        ? new CrossPoolMigrateToHomeCommand(menu.Command.MainWindowCommandInterface, selection, host)
                        : new CrossPoolMigrateCommand(menu.Command.MainWindowCommandInterface, selection, host, menu._resumeAfter);

                    var crossPoolMigrateCmdCanRun = cpmCmd.CanExecute();
                    if (Stopped)
                        return;

                    Program.Invoke(Program.MainWindow, delegate
                    {
                        if (crossPoolMigrateCmdCanRun || !string.IsNullOrEmpty(cpmCmd.CantExecuteReason))
                        {
                            hostMenuItem.Command = cpmCmd;
                            hostMenuItem.Enabled = crossPoolMigrateCmdCanRun;
                        }
                        else
                        {
                            hostMenuItem.Command = cmd;
                            hostMenuItem.Enabled = false;
                        }
                    });
                }
            });
        }
        
        #endregion

        /// <summary>
        /// This class is an implementation of the 'IComparer' interface 
        /// for sorting vm placement menuItem List when wlb is enabled 
        /// </summary>
        private class WlbHostStarCompare : IComparer<VMOperationToolStripMenuSubItem>
        {
            public int Compare(VMOperationToolStripMenuSubItem x, VMOperationToolStripMenuSubItem y)
            {
                int result = 0;

                // if x and y are enabled, compare their start rating
                if (x.Enabled && y.Enabled)
                    result = y.StarRating.CompareTo(x.StarRating);

                // if x and y are disabled, they are equal
                else if (!x.Enabled && !y.Enabled)
                    result = 0;

                // if x is disabled, y is greater
                else if (!x.Enabled)
                    result = 1;

                // if y is disabled, x is greater
                else if (!y.Enabled)
                    result = -1;

                return result;
            }
        }
    }
}
