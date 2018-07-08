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

using System.Linq;
using System.Collections.Generic;
using XenAdmin.Core;
using XenAdmin.Wizards.CrossPoolMigrateWizard;
using XenAPI;
using XenAdmin.Dialogs.VMDialogs;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Launches the Move-VM dialog for the selected VM.
    /// </summary>
    internal class MoveVMCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public MoveVMCommand()
        {
        }

        public MoveVMCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            var cmd = new CrossPoolMoveVMCommand(MainWindowCommandInterface, selection);
            var con = selection.GetConnectionOfFirstItem();

            if (cmd.CanExecute() && !Helpers.FeatureForbidden(con, Host.RestrictCrossPoolMigrate))
            {
                cmd.Execute();
            }
            else
            {
                VM vm = (VM) selection[0].XenObject;
                MainWindowCommandInterface.ShowPerXenModelObjectWizard(vm, new MoveVMDialog(vm));
            }
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<VM>(CBTDisabled) &&
                   (new CrossPoolMoveVMCommand(MainWindowCommandInterface, selection).CanExecute() ||
                   selection.ContainsOneItemOfType<VM>(CanExecute));
        }

        private bool CBTDisabled(VM vm)
        {
            if (vm == null)
                return false;
            foreach (var vbd in vm.Connection.ResolveAll(vm.VBDs))
            {
                var vdi = vm.Connection.Resolve(vbd.VDI);
                if (vdi != null && vdi.cbt_enabled) 
                    return false;
            }
            return true;
        }

        private static bool CanExecute(VM vm)
        {
            return vm != null && (CrossPoolMoveVMCommand.CanExecute(vm, null) || vm.CanBeMoved());
        }

        public override string MenuText
        {
            get
            {
                return CrossPoolMoveVMCommand.GetWizardMode(GetSelection()) == WizardMode.Migrate
                           ? Messages.MAINWINDOW_MIGRATEVM
                           : Messages.MAINWINDOW_MOVEVM;
            }
        }
    }
}
