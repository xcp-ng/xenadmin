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
    /// Launches the Copy-VM dialog for the selected VM.
    /// </summary>
    internal class CopyVMCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public CopyVMCommand()
        {
        }

        public CopyVMCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public CopyVMCommand(IMainWindow mainWindow, VM vm)
            : base(mainWindow, vm)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            VM vm = (VM)selection[0].XenObject;

            if (CrossPoolCopyVMCommand.CanExecute(vm, null, false))
                new CrossPoolCopyVMCommand(MainWindowCommandInterface, selection).Execute();
            else
                new CopyVMDialog(vm).ShowPerXenObject(vm, Program.MainWindow);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.ContainsOneItemOfType<VM>() && selection.AtLeastOneXenObjectCan<VM>(CanExecute);
        }

        private static bool CanExecute(VM vm)
        {
            return vm != null && (CrossPoolCopyVMCommand.CanExecute(vm, null, false) || vm.CanBeCopied());
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_COPY_VM;
            }
        }
    }
}
