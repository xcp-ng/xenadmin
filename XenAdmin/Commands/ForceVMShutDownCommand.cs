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
using System.Windows.Forms;
using XenAPI;
using System.Drawing;
using XenAdmin.Core;
using XenAdmin.Actions;
using XenAdmin.Network;
using XenAdmin.Properties;
using System.Collections.ObjectModel;
using XenAdmin.Dialogs;
using XenAdmin.Actions.VMActions;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Forces the selected VM to shut-down. Shows a confirmation dialog.
    /// </summary>
    internal class ForceVMShutDownCommand : VMLifeCycleCommand
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public ForceVMShutDownCommand()
        {
        }

        public ForceVMShutDownCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public ForceVMShutDownCommand(IMainWindow mainWindow, VM vm, Control parent)
            : base(mainWindow, vm, parent)
        {
        }
        protected override void Execute(List<VM> vms)
        {
            CancelAllTasks(vms);
            RunAction(vms, Messages.ACTION_VMS_SHUTTING_DOWN_TITLE, Messages.ACTION_VMS_SHUTTING_DOWN_TITLE, Messages.ACTION_VM_SHUT_DOWN, null);
        }

        protected override bool CanExecute(VM vm)
        {
            if (vm != null && !vm.Locked && !vm.is_a_template)
            {
                // CA-16960 If the VM is up and has a running task, we will disregard the allowed_operations
                // and always allow forced options.
                if (vm.power_state == vm_power_state.Running && HelpersGUI.HasRunningTasks(vm))
                {
                    return true;
                }
                else
                {
                    return vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.hard_shutdown);
                }
            }
            return false;
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._001_ForceShutDown_h32bit_16;
            }
        }

        public override Image ToolBarImage
        {
            get
            {
                return Images.StaticImages._001_ForceShutDown_h32bit_24;
            }
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_FORCE_SHUTDOWN;
            }
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.MAINWINDOW_FORCE_SHUTDOWN_CONTEXT_MENU;
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
                bool hasRunningTasks = false;
                bool isHAProtected = false;

                foreach (VM vm in selection.AsXenObjects<VM>())
                {
                    if (HelpersGUI.HasRunningTasks(vm))
                    {
                        hasRunningTasks = true;
                    }
                    if (vm.HAIsProtected())
                    {
                        isHAProtected = true;
                    }
                }

                string msg;
                if (selection.Count == 1)
                {
                    if (hasRunningTasks)
                    {
                        if (isHAProtected)
                        {
                            msg = Messages.HA_CONFIRM_FORCESHUTDOWN_VM;
                        }
                        else
                        {
                            msg = Messages.CONFIRM_FORCESHUTDOWN_VM;
                        }
                    }
                    else
                    {
                        if (isHAProtected)
                        {
                            msg = Messages.HA_CONFIRM_FORCESHUTDOWN_VM_NO_CANCEL_TASKS;
                        }
                        else
                        {
                            msg = Messages.CONFIRM_FORCESHUTDOWN_VM_NO_CANCEL_TASKS;
                        }
                    }
                    return string.Format(msg, ((VM)selection[0].XenObject).Name());
                }

                if (hasRunningTasks)
                {
                    if (isHAProtected)
                    {
                        msg = Messages.HA_CONFIRM_FORCESHUTDOWN_VMS;
                    }
                    else
                    {
                        msg = Messages.CONFIRM_FORCESHUTDOWN_VMS;
                    }
                }
                else
                {
                    if (isHAProtected)
                    {
                        msg = Messages.HA_CONFIRM_FORCESHUTDOWN_VMS_NO_CANCEL_TASKS;
                    }
                    else
                    {
                        msg = Messages.CONFIRM_FORCESHUTDOWN_VMS_NO_CANCEL_TASKS;
                    }
                }
                return msg;
            }
        }

        protected override string ConfirmationDialogTitle
        {
            get
            {
                if (GetSelection().Count == 1)
                {
                    return Messages.CONFIRM_FORCESHUTDOWN_VM_TITLE;
                }
                return Messages.CONFIRM_FORCESHUTDOWN_VMS_TITLE;
            }
        }

        protected override string ConfirmationDialogHelpId
        {
            get { return "WarningVmLifeCycleForceShutDown"; }
        }

        protected override CommandErrorDialog GetErrorDialogCore(IDictionary<SelectedItem, string> cantExecuteReasons)
        {
            foreach (VM vm in GetSelection().AsXenObjects<VM>())
            {
                if (!CanExecute(vm) && vm.power_state != vm_power_state.Halted)
                {
                    return new CommandErrorDialog(Messages.ERROR_DIALOG_FORCE_SHUTDOWN_VM_TITLE, Messages.ERROR_DIALOG_FORCE_SHUTDOWN_VM_TEXT, cantExecuteReasons);
                }
            }
            return null;
        }

        private static bool ShowOnMainToolBarInternal(VM vm)
        {
            return !vm.allowed_operations.Contains(XenAPI.vm_operations.clean_shutdown);
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
            return new VMHardShutdown(vm);
        }
    }
}
