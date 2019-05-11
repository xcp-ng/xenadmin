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
using XenAdmin.Network;
using XenAdmin.Dialogs;
using XenAdmin.Core;
using System.Windows.Forms;
using System.Drawing;
using XenAdmin.Actions;
using XenCenterLib;
using System.IO;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Shows the ExportVMDialog dialog for the selected VM.
    /// </summary>
    internal class ExportVMCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public ExportVMCommand()
        {
        }

        public ExportVMCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public ExportVMCommand(IMainWindow mainWindow, SelectedItem selection)
            : base(mainWindow, selection)
        {
        }

        public ExportVMCommand(IMainWindow mainWindow, VM vm)
            : base(mainWindow, vm)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="vm">The VM to export.</param>
        /// <param name="host">Used for filtering purposes. May be null.</param>
        private void Execute(IXenConnection connection, VM vm, Host host)
        {
            /*
             * These properties have not been copied over to the new save file dialog.
             *  
            dlg.AddExtension = true;
            dlg.CheckPathExists = true;
            dlg.CreatePrompt = false;
            dlg.CheckFileExists = false;
            dlg.OverwritePrompt = true;
            dlg.ValidateNames = true;*/

            string filename;
            bool verify;

            // Showing this dialog has the (undocumented) side effect of changing the working directory
            // to that of the file selected. This means a handle to the directory persists, making
            // it undeletable until the program exits, or the working dir moves on. So, save and
            // restore the working dir...
            String oldDir = "";
            try
            {
                oldDir = Directory.GetCurrentDirectory();
                while (true)
                {
                    ExportVMDialog dlg = new ExportVMDialog();
                    dlg.DefaultExt = "xva";
                    dlg.Filter = Messages.MAINWINDOW_XVA_BLURB;
                    dlg.Title = Messages.MAINWINDOW_XVA_TITLE;

                    if (dlg.ShowDialog(Parent) != DialogResult.OK)
                        return;

                    filename = dlg.FileName;
                    verify = dlg.Verify;

                    // CA-12975: Warn the user if the export operation does not have enough disk space to
                    // complete.  This is an approximation only.
                    Win32.DiskSpaceInfo diskSpaceInfo = Win32.GetDiskSpaceInfo(dlg.FileName);

                    if (diskSpaceInfo == null)
                    {
                        // Could not determine free disk space. Carry on regardless.
                        break;
                    }
                    else
                    {
                        ulong freeSpace = diskSpaceInfo.FreeBytesAvailable;
                        decimal neededSpace = vm.GetRecommendedExportSpace(Properties.Settings.Default.ShowHiddenVMs);
                        ulong spaceLeft = 100 * Util.BINARY_MEGA; // We want the user to be left with some disk space afterwards
                        if (neededSpace >= freeSpace - spaceLeft)
                        {
                            string msg = string.Format(Messages.CONFIRM_EXPORT_NOT_ENOUGH_MEMORY, Util.DiskSizeString((long)neededSpace),
                                Util.DiskSizeString((long)freeSpace), vm.Name());

                            DialogResult dr;
                            using (var d = new ThreeButtonDialog(
                                new ThreeButtonDialog.Details(SystemIcons.Warning, msg),
                                "ExportVmDialogInsufficientDiskSpace",
                                new ThreeButtonDialog.TBDButton(Messages.CONTINUE_WITH_EXPORT, DialogResult.OK),
                                new ThreeButtonDialog.TBDButton(Messages.CHOOSE_ANOTHER_DESTINATION, DialogResult.Retry),
                                ThreeButtonDialog.ButtonCancel))
                            {
                                dr = d.ShowDialog(Parent);
                            }

                            if (dr == DialogResult.Retry)
                            {
                                continue;
                            }
                            else if (dr == DialogResult.Cancel)
                            {
                                return;
                            }
                        }
                        if (diskSpaceInfo.IsFAT && neededSpace > (4 * Util.BINARY_GIGA) - 1)
                        {
                            string msg = string.Format(Messages.CONFIRM_EXPORT_FAT, Util.DiskSizeString((long)neededSpace),
                                Util.DiskSizeString(4 * Util.BINARY_GIGA), vm.Name());

                            DialogResult dr;
                            using (var d = new ThreeButtonDialog(
                                new ThreeButtonDialog.Details(SystemIcons.Warning, msg),
                                "ExportVmDialogFSLimitExceeded",
                                new ThreeButtonDialog.TBDButton(Messages.CONTINUE_WITH_EXPORT, DialogResult.OK),
                                new ThreeButtonDialog.TBDButton(Messages.CHOOSE_ANOTHER_DESTINATION, DialogResult.Retry),
                                ThreeButtonDialog.ButtonCancel))
                            {
                                dr = d.ShowDialog(Parent);
                            }
 
                            if (dr == DialogResult.Retry)
                            {
                                continue;
                            }
                            else if (dr == DialogResult.Cancel)
                            {
                                return;
                            }
                        }
                        break;
                    }
                }
            }
            finally
            {
                Directory.SetCurrentDirectory(oldDir);
            }

            new ExportVmAction(connection, host, vm, filename, verify).RunAsync();
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            VM vm = (VM)selection[0].XenObject;
            Host host = vm.Home();
            Pool pool = Helpers.GetPool(vm.Connection);

            if (host == null && pool != null)
            {
                host = pool.Connection.Resolve(pool.master);
            }

            Execute(selection[0].Connection, vm, host);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.Count == 1)
            {
                VM vm = selection[0].XenObject as VM;

                if (vm != null && !vm.is_a_template && !vm.Locked && vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.export))
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
                return Messages.MAINWINDOW_EXPORT_VM_AS_BACKUP;
            }
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            VM vm = item as VM;
            if (vm == null)
                return base.GetCantExecuteReasonCore(item);
            if (vm.power_state == vm_power_state.Running)
                return Messages.MAINWINDOW_EXPORT_VM_AS_BACKUP_TOOLTIP;

            return null;
        }
    }
}
