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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Properties;
using XenAPI;
using XenAdmin.Dialogs;
using XenAdmin.Actions.VMActions;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Suspends the selected VMs. Shows a confirmation dialog.
    /// </summary>
    internal class SuspendVMCommand : VMLifeCycleCommand
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public SuspendVMCommand()
        {
        }

        public SuspendVMCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public SuspendVMCommand(IMainWindow mainWindow, VM vm)
            : base(mainWindow, vm)
        {
        }

        public SuspendVMCommand(IMainWindow mainWindow, VM vm, Control parent)
            : base(mainWindow, vm, parent)
        {
        }

        protected override bool CanExecute(VM vm)
        {
            if (!vm.is_a_template && vm.power_state == vm_power_state.Running && vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.suspend))
            {
                return true;
            }
            return false;
        }

        protected override void Execute(List<VM> vms)
        {
            RunAction(vms, Messages.ACTION_VMS_SUSPENDING_TITLE, Messages.ACTION_VMS_SUSPENDING_TITLE, Messages.ACTION_VM_SUSPENDED, null);
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_SUSPEND;
            }
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._000_paused_h32bit_16;
            }
        }

        public override Image ToolBarImage
        {
            get
            {
                return Images.StaticImages._000_Paused_h32bit_24;
            }
        }

        public override string ToolBarText
        {
            get
            {
                return Messages.MAINWINDOW_TOOLBAR_SUSPEND;
            }
        }

        protected override string EnabledToolTipText
        {
            get
            {
                return Messages.MAINWINDOW_TOOLBAR_SUSPENDVM;
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
                    return Messages.CONFIRM_SUSPEND_VM_TITLE;
                }
                return Messages.CONFIRM_SUSPEND_VMS_TITLE;
            }
        }

        protected override string ConfirmationDialogText
        {
            get
            {
                SelectedItemCollection selection = GetSelection();
                if (selection.Count == 1)
                {
                    VM vm = (VM)selection[0].XenObject;
                    return vm.HAIsProtected() ? Messages.HA_CONFIRM_SUSPEND_VM : Messages.CONFIRM_SUSPEND_VM;
                }
                return Messages.CONFIRM_SUSPEND_VMS;
            }
        }

        protected override string ConfirmationDialogHelpId
        {
            get { return "WarningVmLifeCycleSuspend"; }
        }

        protected override CommandErrorDialog GetErrorDialogCore(IDictionary<IXenObject, string> cantExecuteReasons)
        {
            foreach (VM vm in GetSelection().AsXenObjects<VM>())
            {
                if (!CanExecute(vm) && vm.power_state == vm_power_state.Running)
                {
                    return new CommandErrorDialog(Messages.ERROR_DIALOG_SUSPEND_VM_TITLE, Messages.ERROR_DIALOG_SUSPEND_VM_TEXT, cantExecuteReasons);
                }
            }
            return null;
        }

        public override Keys ShortcutKeys
        {
            get
            {
                return Keys.Control | Keys.Y;
            }
        }

        public override string ShortcutKeyDisplayString
        {
            get
            {
                return Messages.MAINWINDOW_CTRL_Y;
            }
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            VM vm = item as VM;
            if (vm == null)
            {
                return base.GetCantExecuteReasonCore(item);
            }
            if (vm.power_state == vm_power_state.Halted)
            {
                return Messages.VM_SHUT_DOWN;
            }
            if (vm.power_state == vm_power_state.Suspended)
            {
                return Messages.VM_ALREADY_SUSPENDED;
            }

            var noToolsOrDriversReason = GetCantExecuteNoToolsOrDriversReasonCore(item);
            if (noToolsOrDriversReason != null)
            {
                return noToolsOrDriversReason;
            }

            if (vm.HasGPUPassthrough())
            {
                return Messages.VM_HAS_GPU_PASSTHROUGH;
            }

            return base.GetCantExecuteReasonCore(item);
        }

        protected override AsyncAction BuildAction(VM vm)
        {
            return new VMSuspendAction(vm);
        }
    }
}
