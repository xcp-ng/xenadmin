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

using System.Collections.Generic;
using System.Linq;

using XenAdmin.Actions;
using XenAdmin.Core;

using XenAPI;
using XenAdmin.Wizards.PatchingWizard.PlanActions;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Reboots the selected host. Shows a confirmation dialog.
    /// </summary>
    internal class RestartToolstackCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public RestartToolstackCommand()
        {
        }

        public RestartToolstackCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            var actions = new List<AsyncAction>();
            var liveHosts = selection.AsXenObjects<Host>().Where(h => h.IsLive());

            foreach (Host host in liveHosts)
            {
                MainWindowCommandInterface.CloseActiveWizards(host.Connection);
                var action = new RestartToolstackAction(host);
                actions.Add(action);
            }
            RunMultipleActions(actions, null, Messages.ACTION_TOOLSTACK_RESTARTING, Messages.ACTION_TOOLSTACK_RESTARTED, true);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<Host>() && selection.Any(item => ((Host)item.XenObject).IsLive());
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            Host host = item as Host;
            
            if (host == null)
                return base.GetCantExecuteReasonCore(item);

            if (!host.IsLive())
                return Messages.HOST_NOT_LIVE;

            return base.GetCantExecuteReasonCore(item);
        }

        public override string MenuText
        {
            get { return Messages.MAINWINDOW_RESTART_TOOLSTACK; }
        }

        public override string ContextMenuText
        {
            get { return Messages.MAINWINDOW_RESTART_TOOLSTACK; }
        }

        protected override bool ConfirmationRequired
        {
            get { return true; }
        }

        protected override string ConfirmationDialogText
        {
            get
            {
                List<Host> hosts = GetSelection().AsXenObjects<Host>();

                return hosts.Count == 1
                           ? string.Format(Messages.CONFIRM_RESTART_TOOLSTACK_ONE_SERVER, hosts[0].Name().Ellipsise(30))
                           : Messages.CONFIRM_RESTART_TOOLSTACK_MANY_SERVERS;
            }
        }

        protected override string ConfirmationDialogTitle
        {
            get { return Messages.CONFIRM_RESTART_TOOLSTACK_TITLE; }
        }
    }
}
