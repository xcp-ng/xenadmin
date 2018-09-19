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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Network;
using XenAdmin.Properties;
using XenAPI;
using XenAdmin.Dialogs;
using XenAdmin.Actions.VMActions;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Reboots the selected VM. Shows a confirmation dialog.
    /// </summary>
    internal class RebootVMCommand : VMLifeCycleCommand
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public RebootVMCommand()
        {
        }

        public RebootVMCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public RebootVMCommand(IMainWindow mainWindow, VM vm, Control parent)
            : base(mainWindow, vm, parent)
        {
        }

        public RebootVMCommand(IMainWindow mainWindow, VM vm)
            : base(mainWindow, vm)
        {
        }

        protected override bool CanExecute(VM vm)
        {
            ReadOnlyCollection<SelectedItem> selection = GetSelection();

            if (vm != null && vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.clean_reboot) && !vm.is_a_template && !vm.Locked)
            {
                if (EnabledTargetExists(selection[0].HostAncestor, selection[0].Connection))
                {
                    return true;
                }
            }
            return false;
        }

        protected override void Execute(List<VM> vms)
        {
            RunAction(vms, Messages.ACTION_VMS_REBOOTING_TITLE, Messages.ACTION_VMS_REBOOTING_TITLE, Messages.ACTION_VM_REBOOTED, null);
        }

        private static bool EnabledTargetExists(Host host, IXenConnection connection)
        {
            if (host != null)
                return host.enabled;

            foreach (Host h in connection.Cache.Hosts)
            {
                if (h.enabled)
                    return true;
            }
            return false;
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._001_Reboot_h32bit_16;
            }
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_REBOOT;
            }
        }

        protected override bool ConfirmationRequired
        {
            get
            {
                return true;
            }
        }

        protected override string ConfirmationDialogTitle
        {
            get
            {
                if (GetSelection().Count == 1)
                {
                    return Messages.CONFIRM_REBOOT_VM_TITLE;
                }

                return Messages.CONFIRM_REBOOT_VMS_TITLE;
            }
        }

        protected override string ConfirmationDialogText
        {
            get
            {
                if (GetSelection().Count == 1)
                {
                    return Messages.CONFIRM_REBOOT_VM;
                }

                return Messages.CONFIRM_REBOOT_VMS;
            }
        }

        protected override string ConfirmationDialogHelpId
        {
            get { return "WarningVmLifeCycleReboot"; }
        }

        protected override CommandErrorDialog GetErrorDialogCore(IDictionary<SelectedItem, string> cantExecuteReasons)
        {
            return new CommandErrorDialog(Messages.ERROR_DIALOG_REBOOT_VM_TITLE, Messages.ERROR_DIALOG_REBOOT_VM_TEXT, cantExecuteReasons);
        }

        public override Keys ShortcutKeys
        {
            get
            {
                return Keys.Control | Keys.R;
            }
        }

        public override string ShortcutKeyDisplayString
        {
            get
            {
                return Messages.MAINWINDOW_CTRL_R;
            }
        }

        protected override string GetCantExecuteReasonCore(SelectedItem item)
        {
            VM vm = item.XenObject as VM;
            if (vm == null)
            {
                return base.GetCantExecuteReasonCore(item);
            }

            switch (vm.power_state)
            {
                case vm_power_state.Halted:
                    return Messages.VM_SHUT_DOWN;
                case vm_power_state.Paused:
                    return Messages.VM_PAUSED;
                case vm_power_state.Suspended:
                    return Messages.VM_SUSPENDED;
                case vm_power_state.unknown:
                    return base.GetCantExecuteReasonCore(item);
            }

            return GetCantExecuteNoToolsOrDriversReasonCore(item) ?? base.GetCantExecuteReasonCore(item);
        }

        protected override AsyncAction BuildAction(VM vm)
        {
            return new VMCleanReboot(vm);
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.MAINWINDOW_REBOOT_CONTEXT_MENU;
            }
        }
    }
}
