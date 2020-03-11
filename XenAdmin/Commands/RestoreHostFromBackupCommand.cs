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
using System.Windows.Forms;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Actions;
using XenAdmin.Core;
using System.Collections.ObjectModel;
using System.IO;
using System.Drawing;
using XenAdmin.Properties;

namespace XenAdmin.Commands
{
    /// <summary>
    /// Shows an open-file dialog for restoring the selected host.
    /// </summary>
    internal class RestoreHostFromBackupCommand : Command
    {
        private readonly string _filePath = string.Empty;

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public RestoreHostFromBackupCommand()
        {
        }

        public RestoreHostFromBackupCommand(IMainWindow mainWindow, Host host, string filePath)
            : base(mainWindow, host)
        {
            _filePath = filePath;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Execute(selection[0].XenObject as Host, _filePath);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.Count == 1)
            {
                IXenConnection connection = selection[0].Connection;
                Host host = selection[0].XenObject as Host;
                return host != null && host.IsLive();
            }
            return false;
        }

        private void Execute(Host host, string filepath)
        {
            HelpersGUI.BringFormToFront(MainWindowCommandInterface.Form);

            if (filepath == "")
            {
                // Showing this dialog has the (undocumented) side effect of changing the working directory
                // to that of the file selected. This means a handle to the directory persists, making
                // it undeletable until the program exits, or the working dir moves on. So, save and
                // restore the working dir...
                String oldDir = "";
                try
                {
                    oldDir = Directory.GetCurrentDirectory();
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.AddExtension = true;
                    dialog.Filter = string.Format("{0} (*.{1})|*.{1}|{2} (*.*)|*.*",
                        Messages.XS_BACKUP_FILES, BrandManager.ExtensionBackup, Messages.ALL_FILES);
                    dialog.FilterIndex = 0;
                    dialog.RestoreDirectory = true;
                    dialog.DefaultExt = BrandManager.ExtensionBackup;
                    dialog.CheckPathExists = false;
                    if (dialog.ShowDialog(Parent) == DialogResult.Cancel)
                        return;
                    filepath = dialog.FileName;
                }
                finally
                {
                    Directory.SetCurrentDirectory(oldDir);
                }
            }

            if (host == null)
            {
                SelectHostDialog hostdialog = new SelectHostDialog
                {
                    TheHost = host,
                    TopBlurb = Messages.BACKUP_SELECT_HOST,
                    TopPicture = Images.StaticImages.backup_restore_32,
                    HelpString = "Backup", // don't i18n
                    Text = Messages.BACKUP_SELECT_HOST_TITLE,
                    OkButtonText = Messages.BACKUP_SELECT_HOST_BUTTON
                };

                hostdialog.FormClosed += delegate
                {
                    if (hostdialog.DialogResult != DialogResult.OK)
                        return;
                    host = hostdialog.TheHost;
                    HostBackupRestoreAction action = new HostBackupRestoreAction(host, HostBackupRestoreAction.HostBackupRestoreType.restore, filepath);
                    action.Completed += RestoreAction_Completed;
                    action.RunAsync();
                };
                hostdialog.Show(Parent);
            }
            else
            {
                HostBackupRestoreAction action = new HostBackupRestoreAction(host, HostBackupRestoreAction.HostBackupRestoreType.restore, filepath);
                action.Completed += RestoreAction_Completed;
                action.RunAsync();
            }
        }

        private void RestoreAction_Completed(ActionBase sender)
        {
            HostBackupRestoreAction action = (HostBackupRestoreAction)sender;
            
            if (!action.Succeeded)
            {
                // Do nothing - failure will be reflected in the logs tab.
                return;
            }

            MainWindowCommandInterface.Invoke(delegate
            {
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(
                        SystemIcons.Information,
                        string.Format(Messages.RESTORE_FROM_BACKUP_FINALIZE, Helpers.GetName(action.Host)),
                        Messages.XENCENTER)))
                {
                    dlg.ShowDialog(Parent);
                }
            });
        }
    }
}
