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
using XenAdmin.Network;
using XenAPI;
using System.Windows.Forms;
using XenAdmin.Actions;
using System.Collections.ObjectModel;
using XenAdmin.Core;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Shows a SaveFileDialog for backing up the selected host and then runs the backup.
    /// </summary>
    internal class BackupHostCommand : Command
    {
        private readonly string _filename;

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public BackupHostCommand()
        {
        }

        public BackupHostCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public BackupHostCommand(IMainWindow mainWindow, Host host)
            : base(mainWindow, host)
        {
        }

        public BackupHostCommand(IMainWindow mainWindow, Host host, string filename)
            : base(mainWindow, host)
        {
            _filename = filename;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Host host = (Host)selection[0].XenObject;

            if (_filename == null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.AddExtension = true;
                dialog.Filter = string.Format("{0} (*.{1})|*.{1}|{2} (*.*)|*.*", Messages.XS_BACKUP_FILES, BrandManager.ExtensionBackup, Messages.ALL_FILES);
                dialog.FilterIndex = 0;
                dialog.RestoreDirectory = true;
                dialog.DefaultExt = BrandManager.ExtensionBackup;

                if (dialog.ShowDialog(Parent) != DialogResult.Cancel)
                    new HostBackupRestoreAction(host, HostBackupRestoreAction.HostBackupRestoreType.backup, dialog.FileName).RunAsync();
            }
            else
            {
                new HostBackupRestoreAction(host, HostBackupRestoreAction.HostBackupRestoreType.backup, _filename).RunAsync();
            }
        }

        private bool CanExecute(Host host)
        {
            return host != null && host.IsLive();
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.ContainsOneItemOfType<Host>() && selection.AtLeastOneXenObjectCan<Host>(CanExecute);
        }
    }
}
