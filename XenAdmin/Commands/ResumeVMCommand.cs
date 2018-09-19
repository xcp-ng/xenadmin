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
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Properties;
using XenAPI;
using XenAdmin.Dialogs;
using XenAdmin.Actions.VMActions;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Resumes the selected VMs.
    /// </summary>
    internal class ResumeVMCommand : VMLifeCycleCommand
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public ResumeVMCommand()
        {
        }

        public ResumeVMCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public ResumeVMCommand(IMainWindow mainWindow, VM vm, Control parent)
            : base(mainWindow, vm, parent)
        {

        }

        public ResumeVMCommand(IMainWindow mainWindow, VM vm)
            : base(mainWindow, vm)
        {
        }

        protected override void Execute(List<VM> vms)
        {
            RunAction(vms, Messages.ACTION_VMS_RESUMING_ON_TITLE, Messages.ACTION_VMS_RESUMING_ON_TITLE, Messages.ACTION_VM_RESUMED, null);
        }

        protected override bool CanExecute(VM vm)
        {
            ReadOnlyCollection<SelectedItem> selection = GetSelection();

            if (vm != null && !vm.is_a_template && vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.resume) && vm.power_state == vm_power_state.Suspended)
            {
                if (Helpers.EnabledTargetExists(selection[0].HostAncestor, selection[0].Connection))
                {
                    return true;
                }
            }
            return false;
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_RESUME;
            }
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.MAINWINDOW_RESUME_CONTEXT_MENU;
            }
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._000_Resumed_h32bit_16;
            }
        }

        public override Image ToolBarImage
        {
            get
            {
                return Images.StaticImages._000_Resumed_h32bit_24;
            }
        }

        protected override string EnabledToolTipText
        {
            get
            {
                return Messages.MAINWINDOW_TOOLBAR_RESUMEVM;
            }
        }

        public override string ToolBarText
        {
            get
            {
                return Messages.MAINWINDOW_TOOLBAR_RESUME;
            }
        }

        protected override CommandErrorDialog GetErrorDialogCore(IDictionary<SelectedItem, string> cantExecuteReasons)
        {
            foreach (VM vm in GetSelection().AsXenObjects<VM>())
            {
                if (!CanExecute(vm) && vm.power_state == vm_power_state.Suspended)
                {
                    return new CommandErrorDialog(Messages.ERROR_DIALOG_RESUME_VM_TITLE, Messages.ERROR_DIALOG_RESUME_VM_TEXT, cantExecuteReasons);
                }
            }
            return null;
        }

        public override string ShortcutKeyDisplayString
        {
            get
            {
                return Messages.MAINWINDOW_CTRL_Y;
            }
        }

        public override Keys ShortcutKeys
        {
            get
            {
                return Keys.Control | Keys.Y;
            }
        }

        protected override string GetCantExecuteReasonCore(SelectedItem item)
        {
            VM vm = item.XenObject as VM;
            if (vm == null)
            {
                return base.GetCantExecuteReasonCore(item);
            }
            if (vm.power_state != vm_power_state.Suspended)
            {
                return Messages.VM_NOT_SUSPENDED;
            }

            return GetCantExecuteNoToolsOrDriversReasonCore(item) ?? base.GetCantExecuteReasonCore(item);
        }

        protected override AsyncAction BuildAction(VM vm)
        {
            return new VMResumeAction(vm, VMOperationCommand.WarningDialogHAInvalidConfig, VMOperationCommand.StartDiagnosisForm);
        }
    }
}
