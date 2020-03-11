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
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using XenAdmin.Actions;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;


namespace XenAdmin.Commands
{
    internal enum HostMaintenanceModeCommandParameter { Enter, Exit }

    /// <summary>
    /// Enters/exits the specified host from maintenance mode.
    /// </summary>
    internal class HostMaintenanceModeCommand : Command
    {
        private readonly HostMaintenanceModeCommandParameter _parameter;
        private readonly bool _parameterSpecified;

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public HostMaintenanceModeCommand()
        {
        }

        public HostMaintenanceModeCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public HostMaintenanceModeCommand(IMainWindow mainWindow, Host host, HostMaintenanceModeCommandParameter parameter)
            : base(mainWindow, host)
        {
            _parameter = parameter;
            _parameterSpecified = true;
        }

        private void EnterMaintenanceMode(Host host)
        {
            Pool pool = Helpers.GetPool(host.Connection);

            if (pool != null && pool.ha_enabled && host.IsMaster())
            {
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(
                        SystemIcons.Error,
                        String.Format(Messages.HA_CANNOT_EVACUATE_MASTER,
                            Helpers.GetName(host).Ellipsise(Helpers.DEFAULT_NAME_TRIM_LENGTH)),
                        Messages.XENCENTER)))
                {
                    dlg.ShowDialog(Parent);
                }

                return;
            }

            if (!host.GetRunningVMs().Any() && (pool == null || !host.IsMaster()))
            {
                Program.MainWindow.CloseActiveWizards(host.Connection);
                var action = new EvacuateHostAction(host, null, new Dictionary<XenRef<VM>, string[]>(), AddHostToPoolCommand.NtolDialog, AddHostToPoolCommand.EnableNtolDialog);
                action.Completed += Program.MainWindow.action_Completed;
                action.RunAsync();
                return;
            }
            
            new EvacuateHostDialog(host).ShowPerXenObject(host, Program.MainWindow);
        }

        private void ExitMaintenanceMode(Host host)
        {
            var vmsToRestore = new List<VM>();
            vmsToRestore.AddRange(host.GetHaltedEvacuatedVMs());
            vmsToRestore.AddRange(host.GetMigratedEvacuatedVMs());
            vmsToRestore.AddRange(host.GetSuspendedEvacuatedVMs());

            List<VM> to_remove = new List<VM>();
            foreach (VM vm in vmsToRestore)
            {
                if (vm.resident_on == host.opaque_ref)
                    to_remove.Add(vm);
            }
            foreach (VM vm in to_remove)
            {
                vmsToRestore.Remove(vm);
            }

            DialogResult result = DialogResult.No;

            if (vmsToRestore.Count > 0 && !MainWindowCommandInterface.RunInAutomatedTestMode)
            {
                using (var dlg = new ExitMaintenanceModeDialog(vmsToRestore, host))
                    result = dlg.ShowDialog();

                if (result == DialogResult.Cancel)
                    return;

                if (!host.Connection.IsConnected)
                {
                    MainWindow.ShowDisconnectedMessage(null);
                    return;
                }
            }

            MainWindowCommandInterface.CloseActiveWizards(host.Connection);
            var action = new EnableHostAction(host, result == DialogResult.Yes, AddHostToPoolCommand.EnableNtolDialog);
            action.Completed += Program.MainWindow.action_Completed;
            action.RunAsync();
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Host host = selection[0].HostAncestor;

            if (_parameterSpecified)
            {
                if (_parameter == HostMaintenanceModeCommandParameter.Enter)
                {
                    EnterMaintenanceMode(host);
                }
                else if (_parameter == HostMaintenanceModeCommandParameter.Exit)
                {
                    ExitMaintenanceMode(host);
                }
            }
            else
            {
                if (!host.enabled)
                {
                    ExitMaintenanceMode(host);
                }
                else
                {
                    EnterMaintenanceMode(host);
                }
            }
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.Count == 1)
            {
                IXenConnection connection = selection[0].Connection;
                Host hostAncestor = selection[0].HostAncestor;
                return hostAncestor != null &&  Helpers.GetMaster(connection) != null;
            }
            return false;
        }

        public override string MenuText
        {
            get
            {
                ReadOnlyCollection<SelectedItem> selection = GetSelection();

                if (selection.Count == 1)
                {
                    IXenConnection connection = selection[0].Connection;
                    Host hostAncestor = selection[0].HostAncestor;

                    if (hostAncestor != null )
                    {
                        return !hostAncestor.enabled ? Messages.EXIT_MAINTENANCE_MODE : Messages.ENTER_MAINTENANCE_MODE;
                    }
                }
                return Messages.ENTER_MAINTENANCE_MODE;
            }
        }

        public override string ContextMenuText
        {
            get
            {
                ReadOnlyCollection<SelectedItem> selection = GetSelection();

                if (selection.Count == 1)
                {
                    IXenConnection connection = selection[0].Connection;
                    Host hostAncestor = selection[0].HostAncestor;

                    if (hostAncestor != null)
                    {
                        return !hostAncestor.enabled ? Messages.EXIT_MAINTENANCE_MODE_CONTEXT_MENU : Messages.ENTER_MAINTENANCE_MODE_CONTEXT_MENU;
                    }
                }
                return Messages.ENTER_MAINTENANCE_MODE_CONTEXT_MENU;
            }
        }
    }
}
