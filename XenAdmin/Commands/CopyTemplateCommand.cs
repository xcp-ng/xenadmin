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
using XenAPI;
using XenAdmin.Dialogs;
using System.Collections.ObjectModel;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Launches the Copy-Template dialog for the specified VM.
    /// </summary>
    internal class CopyTemplateCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public CopyTemplateCommand()
        {
        }

        public CopyTemplateCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public CopyTemplateCommand(IMainWindow mainWindow, VM vm)
            : base(mainWindow, vm)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            VM template = (VM)selection[0].XenObject;

            if (CrossPoolCopyTemplateCommand.CanExecute(template, null, false))
                new CrossPoolCopyTemplateCommand(MainWindowCommandInterface, selection).Execute();
            else
                new CopyVMDialog(template).ShowPerXenObject(template, Program.MainWindow);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.ContainsOneItemOfType<VM>() && selection.AtLeastOneXenObjectCan<VM>(CanExecute);
        }

        private static bool CanExecute(VM vm)
        {
            if (vm != null && vm.is_a_template && !vm.is_a_snapshot && !vm.Locked && vm.allowed_operations != null && !vm.InternalTemplate())
            {
                if (CrossPoolCopyTemplateCommand.CanExecute(vm, null, false))
                    return true;
                if (vm.allowed_operations.Contains(vm_operations.clone) || vm.allowed_operations.Contains(vm_operations.copy))
                    return true;
            }
            return false;
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_COPY_TEMPLATE;
            }
        }
    }
}
