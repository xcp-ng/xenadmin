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
using System.Drawing;
using System.Text;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Threading;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Takes a snapshot of the selected VM.
    /// </summary>
    internal class TakeSnapshotCommand : Command
    {

        /// <summary>
        /// Occurs when the snapshot action is completed.
        /// </summary>
        public event EventHandler<TakeSnapshotCommandCompletedEventArgs> Completed;
        public event EventHandler<EventArgs> Started;

        private VM _VM;

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public TakeSnapshotCommand()
        {
        }

        public TakeSnapshotCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public TakeSnapshotCommand(IMainWindow mainWindow, VM snapshot)
            : base(mainWindow, snapshot)
        {
            _VM = snapshot;
        }

        public VMSnapshotCreateAction GetCreateSnapshotAction()
        {
            if (GetSelection().ContainsOneItemOfType<VM>())
            {
                VM vm = (VM)GetSelection()[0].XenObject;

                if (CanExecute(vm))
                {
                    using (VmSnapshotDialog dialog = new VmSnapshotDialog(vm))
                    {
                        if (dialog.ShowDialog(Parent) != DialogResult.Cancel && dialog.SnapshotName != null)
                        {
                            Program.Invoke(Program.MainWindow, () => Program.MainWindow.ConsolePanel.setCurrentSource(vm));
                            return new VMSnapshotCreateAction(vm, dialog.SnapshotName, dialog.SnapshotDescription, dialog.SnapshotType,
                                                              (vmToSnapshot, username, password) =>
                                                              Program.MainWindow.ConsolePanel.Snapshot(
                                                                  vmToSnapshot, username, password));
                        }
                    }
                }
                else
                {
                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.TAKE_SNAPSHOT_ERROR,
                            Messages.XENCENTER)))
                    {
                        dlg.ShowDialog(MainWindowCommandInterface.Form);
                    }
                }
            }
            return null;

        }

        private void snapshotAction_Completed(ActionBase sender)
        {
            OnCompleted(new TakeSnapshotCommandCompletedEventArgs(sender.Succeeded));
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            AsyncAction snapshotAction = GetCreateSnapshotAction();

            if (snapshotAction != null)
            {
                snapshotAction.Completed += snapshotAction_Completed;
                if (Started != null)
                    Started(_VM, null);
                snapshotAction.RunAsync();
            }
        }

        private static bool CanExecute(VM vm)
        {
            return vm != null && !vm.is_a_template && !vm.Locked && (vm.allowed_operations.Contains(vm_operations.snapshot) || vm.allowed_operations.Contains(vm_operations.checkpoint)); 
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.Count == 1)
            {
                return CanExecute(selection[0].XenObject as VM);
            }
            return false;
        }

        protected virtual void OnCompleted(TakeSnapshotCommandCompletedEventArgs e)
        {
            EventHandler<TakeSnapshotCommandCompletedEventArgs> handler = Completed;

            if (handler != null)
            {
                handler(_VM, e);
            }
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_TAKE_SNAPSHOT;
            }
        }
    }

    internal class TakeSnapshotCommandCompletedEventArgs : EventArgs
    {
        private readonly bool _success;
        public TakeSnapshotCommandCompletedEventArgs(bool success)
        {
            Util.ThrowIfParameterNull(success, "success");
            _success = success;
        }

        public bool Success
        {
            get
            {
                return _success;
            }
        }
    }
}
