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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Actions.VMActions;
using System.Windows.Forms;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Converts the selected VM to a template. Shows a confirmation dialog.
    /// </summary>
    internal class ConvertVMToTemplateCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public ConvertVMToTemplateCommand()
        {
        }

        public ConvertVMToTemplateCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public ConvertVMToTemplateCommand(IMainWindow mainWindow, VM vm)
            : base(mainWindow, vm)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            VM vm = (VM)selection[0].XenObject;

            using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(
                            SystemIcons.Warning,
                            string.Format(Messages.CONVERT_TEMPLATE_DIALOG_TEXT, vm.Name().Ellipsise(25)),
                            Messages.CONVERT_TO_TEMPLATE),
                        new ThreeButtonDialog.TBDButton(Messages.CONVERT, DialogResult.OK, ThreeButtonDialog.ButtonType.ACCEPT, true),
                        ThreeButtonDialog.ButtonCancel))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    List<AsyncAction> actions = new List<AsyncAction>();
                    actions.Add(new SetVMOtherConfigAction(vm.Connection, vm, "instant", "true"));
                    actions.Add(new VMToTemplateAction(vm));

                    XenDialogBase.CloseAll(vm);

                    RunMultipleActions(actions, string.Format(Messages.ACTION_VM_TEMPLATIZING_TITLE, vm.Name()),
                                       Messages.ACTION_VM_TEMPLATIZING, Messages.ACTION_VM_TEMPLATIZED, true);
                }
            }
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.ContainsOneItemOfType<VM>() && selection.AtLeastOneXenObjectCan<VM>(CanExecute);
        }

        private static bool CanExecute(VM vm)
        {
            return vm != null && !vm.is_a_template && !vm.Locked && vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.make_into_template);
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_CONVERT_VM_TO_TEMPLATE;
            }
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.MAINWINDOW_CONVERT_VM_TO_TEMPLATE_CONTEXT_MENU;
            }
        }
    }
}
