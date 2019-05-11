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
using System.Windows.Forms;
using XenAdmin.Actions;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Reverts the specified VM to the specified snapshot.
    /// </summary>
    internal class RevertToSnapshotCommand : Command
    {
        private readonly VM _snapshot;
        private readonly VM _VM;

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public RevertToSnapshotCommand()
        {
        }

        public RevertToSnapshotCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection, VM snapshot)
            : base(mainWindow, selection)
        {
            Util.ThrowIfParameterNull(snapshot, "snapshot");
            _snapshot = snapshot;
            _VM = _snapshot.Connection.Resolve(_snapshot.snapshot_of);
        }

        public RevertToSnapshotCommand(IMainWindow mainWindow, VM vm, VM snapshot)
            : base(mainWindow, vm)
        {
            Util.ThrowIfParameterNull(snapshot, "snapshot");
            _snapshot = snapshot;
            _VM = vm;
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.Count == 1)
            {
                VM vm = selection[0].XenObject as VM;
                return vm != null && !vm.is_a_template && !vm.is_a_snapshot;
            }
            return false;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            VM vm = (VM)selection[0].XenObject;

            Program.Invoke(Program.MainWindow, () => Program.MainWindow.ConsolePanel.setCurrentSource(vm));

            RevertDialog dialog = new RevertDialog(vm, _snapshot.Name());

            if (dialog.ShowDialog() == DialogResult.Yes)
            {
                if (dialog.TakeSnapshot)
                {
                    TakeSnapshotCommand command = new TakeSnapshotCommand(MainWindowCommandInterface, vm);
                    var action = command.GetCreateSnapshotAction();
                    if (action != null)
                    {
                        // if new snapshot taken, then only revert if this succeeded
                        action.Completed += delegate
                        {
                            if (action.Succeeded)
                                ExecuteRevertAction();
                        };
                        action.RunAsync();
                    } 
                }
                else
                {
                    ExecuteRevertAction();
                }
            }
        }

        private void ExecuteRevertAction()
        {
            var action = new VMSnapshotRevertAction(_snapshot);
            action.RunAsync();
        }
    }
}
