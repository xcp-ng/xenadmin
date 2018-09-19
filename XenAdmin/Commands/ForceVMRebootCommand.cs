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
using System.Text;
using XenAdmin.Actions;
using System.Drawing;
using XenAdmin.Properties;
using XenAPI;
using XenAdmin.Network;
using System.Windows.Forms;
using XenAdmin.Core;
using System.Collections.ObjectModel;
using XenAdmin.Dialogs;
using XenAdmin.Actions.VMActions;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Forces the selected VM to reboot. Shows a confirmation dialog.
    /// </summary>
    internal class ForceVMRebootCommand : VMLifeCycleCommand
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public ForceVMRebootCommand()
        {
        }

        public ForceVMRebootCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public ForceVMRebootCommand(IMainWindow mainWindow, VM vm, Control parent)
            : base(mainWindow, vm, parent)
        {
        }

        protected override void Execute(List<VM> vms)
        {
            CancelAllTasks(vms);
            RunAction(vms, Messages.ACTION_VMS_REBOOTING_TITLE, Messages.ACTION_VMS_REBOOTING_TITLE, Messages.ACTION_VM_REBOOTED, null);
        }


        

        protected override bool CanExecute(VM vm)
        {
            if (vm != null && !vm.is_a_template && !vm.Locked)
            {
                if (vm.power_state == vm_power_state.Running && HelpersGUI.HasRunningTasks(vm))
                {
                    return true;
                }
                else
                {
                    return vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.hard_reboot) && EnabledTargetExists(vm.Home(), vm.Connection);
                }
            }
            return false;
        }

        private static bool EnabledTargetExists(Host host, IXenConnection connection)
        {
            //if the vm has a home server check it's enabled
            if (host != null)
            {
                return host.enabled;
            }

            foreach (Host h in connection.Cache.Hosts)
            {
                if (h.enabled)
                {
                    return true;
                }
            }
            return false;
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._001_ForceReboot_h32bit_16;
            }
        }

        public override Image ToolBarImage
        {
            get
            {
                return Images.StaticImages._001_ForceReboot_h32bit_24;
            }
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_FORCE_REBOOT;
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
                bool hasRunningTasks = false;
                List<VM> vms = GetSelection().AsXenObjects<VM>();
                foreach (VM vm in vms)
                {
                    if (HelpersGUI.HasRunningTasks(vm))
                    {
                        hasRunningTasks = true;
                        break;
                    }
                }

                if (vms.Count == 1)
                {
                    return hasRunningTasks ? Messages.CONFIRM_FORCEREBOOT_VM : Messages.CONFIRM_FORCEREBOOT_VM_NO_CANCEL_TASKS;
                }

                return hasRunningTasks ? Messages.CONFIRM_FORCEREBOOT_VMS : Messages.CONFIRM_FORCEREBOOT_VMS_NO_CANCEL_TASKS;
            }
        }

        protected override string ConfirmationDialogTitle
        {
            get
            {
                if (GetSelection().Count == 1)
                {
                    return Messages.CONFIRM_FORCEREBOOT_VM_TITLE;
                }
                return Messages.CONFIRM_FORCEREBOOT_VMS_TITLE;
            }
        }

        protected override string ConfirmationDialogHelpId
        {
            get { return "WarningVmLifeCycleForceReboot"; }
        }

        protected override CommandErrorDialog GetErrorDialogCore(IDictionary<SelectedItem, string> cantExecuteReasons)
        {
            foreach (VM vm in GetSelection().AsXenObjects<VM>())
            {
                if (!CanExecute(vm) && vm.power_state != vm_power_state.Halted)
                {
                    return new CommandErrorDialog(Messages.ERROR_DIALOG_FORCE_REBOOT_VM_TITLE, Messages.ERROR_DIALOG_FORCE_REBOOT_VM_TEXT, cantExecuteReasons);
                }
            }
            return null;
        }

        private static bool ShowOnMainToolBarInternal(VM vm)
        {
            return !vm.allowed_operations.Contains(XenAPI.vm_operations.clean_reboot);
        }

        public bool ShowOnMainToolBar
        {
            get
            {
                return CanExecute() && GetSelection().AllItemsAre<VM>(ShowOnMainToolBarInternal);
            }
        }

        protected override AsyncAction BuildAction(VM vm)
        {
            return new VMHardReboot(vm);
        }
    }
}
