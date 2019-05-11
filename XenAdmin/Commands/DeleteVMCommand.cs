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
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Actions.VMActions;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Deletes the selected VMs. Shows a confirmation dialog.
    /// </summary>
    internal class DeleteVMCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public DeleteVMCommand()
        {
        }

        public DeleteVMCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public DeleteVMCommand(IMainWindow mainWindow, SelectedItem selection)
            : base(mainWindow, selection)
        {
        }

        public DeleteVMCommand(IMainWindow mainWindow, VM vm)
            : base(mainWindow, vm)
        {
        }

        private AsyncAction GetAction(VM vm, List<VBD> deleteDisks, List<VM> deleteSnapshots)
        {
            return new VMDestroyAction(vm, deleteDisks, deleteSnapshots);
        }

        protected sealed override void ExecuteCore(SelectedItemCollection selection)
        {
            ConfirmVMDeleteDialog dialog = new ConfirmVMDeleteDialog(selection.AsXenObjects<VM>());

            if (MainWindowCommandInterface.RunInAutomatedTestMode || dialog.ShowDialog(Parent) == DialogResult.Yes)
            {
                CommandErrorDialog errorDialog = null;
                var cantExecuteReasons = GetCantExecuteReasons();

                if (cantExecuteReasons.Count > 0)
                {
                    errorDialog = new CommandErrorDialog(ErrorDialogTitle, ErrorDialogText, GetCantExecuteReasons());
                }

                List<AsyncAction> actions = new List<AsyncAction>();
                foreach (VM vm in selection.AsXenObjects<VM>(CanExecute))
                {
                    var snapshotsToDelete = dialog.DeleteSnapshots.FindAll(x => x.Connection.Resolve(x.snapshot_of) == vm);
                    actions.Add(GetAction(vm, dialog.DeleteDisks, snapshotsToDelete));
                }
                RunMultipleActions(actions, Messages.ACTION_VMS_DESTROYING_TITLE, Messages.ACTION_VM_DESTROYING, Messages.ACTION_VM_DESTROYED, true);

                if (errorDialog != null)
                {
                    errorDialog.ShowDialog(Parent);
                }
            }
        }

        protected virtual bool CanExecute(VM vm)
        {
            return vm != null && !vm.is_a_template && !vm.Locked && vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.destroy);
        }

        protected sealed override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<VM>() && selection.AtLeastOneXenObjectCan<VM>(CanExecute);
        }

        public override string MenuText
        {
            get
            {
                if (GetSelection().Count > 1)
                {
                    return Messages.MAINWINDOW_DELETE_OBJECTS;
                }

                return Messages.MAINWINDOW_DELETE_VM;
            }
        }

        protected virtual string ErrorDialogText
        {
            get
            {
                return Messages.ERROR_DIALOG_DELETE_VM_TEXT;
            }
        }

        protected virtual string ErrorDialogTitle
        {
            get
            {
                return Messages.ERROR_DIALOG_DELETE_VM_TITLE;
            }
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            VM vm = item as VM;
            if (vm == null)
            {
                return base.GetCantExecuteReasonCore(item);
            }
            if (!vm.is_a_template && vm.power_state != vm_power_state.Halted)
            {
                return Messages.VM_NOT_SHUT_DOWN;
            }
            else if (vm.is_a_template && vm.DefaultTemplate())
            {
                return Messages.CANNOT_DELETE_DEFAULT_TEMPLATE;
            }
            return base.GetCantExecuteReasonCore(item);
        }
    }
}
