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
using XenAdmin.Network;
using XenAdmin.Model;
using System.Windows.Forms;
using System.Drawing;
using XenAdmin.Dialogs;
using XenAdmin.Actions;
using XenAPI;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Launches the dialog for creating a new folder.
    /// </summary>
    internal class NewFolderCommand : Command
    {
        public event Action<string[]> FoldersCreated;

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public NewFolderCommand()
        {
        }

        public NewFolderCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public NewFolderCommand(IMainWindow mainWindow, Folder folder)
            : base(mainWindow, folder)
        {
        }

        public NewFolderCommand(IMainWindow mainWindow, Folder folder, Control parent)
            : base(mainWindow, folder)
        {
            SetParent(parent);
        }

        private void Execute(Folder folder, IWin32Window ownerWindow)
        {
            IXenConnection connection;
            String name;

            // Different dialogs depending whether we're at the top level or adding a subfolder.
            // Also offer them a choice of connections if the connection they're on is Read Only
            // (although we will also sudo a Read Only command when the FolderAction is run).
            if (folder == null || folder.IsRootFolder || folder.Connection == null || CrossConnectionCommand.IsReadOnly(folder.Connection))
            {
                using (var dialog = new NameAndConnectionPrompt
                {
                    Text = Messages.NEW_FOLDER_DIALOG_TITLE,
                    OKText = Messages.CREATE_MNEMONIC_R,
                    HelpID = "NewFolderDialog"
                })
                {
                    if (dialog.ShowDialog(ownerWindow) != DialogResult.OK)
                        return;
                    name = dialog.PromptedName;
                    connection = dialog.Connection;
                }
            }
            else
            {
                using (var dialog = new InputPromptDialog {
                    Text = Messages.NEW_FOLDER_DIALOG_TITLE,
                    PromptText = Messages.NEW_FOLDER_NAME,
                    OkButtonText = Messages.NEW_FOLDER_BUTTON,
                    HelpID = "NewFolderDialog"
                })
                {
                    if (dialog.ShowDialog(ownerWindow) != DialogResult.OK)
                        return;
                    name = dialog.InputText;
                }
                connection = folder.Connection;
            }


            List<string> newPaths = new List<string>();
            foreach (string s in name.Split(';'))
            {
                string n = s;
                Folders.FixupRelativePath(ref n);
                if (string.IsNullOrEmpty(n))
                    continue;

                newPaths.Add(Folders.AppendPath(folder == null ? Folders.PATH_SEPARATOR : folder.opaque_ref, n));
            }

            if (newPaths.Count > 0)
            {
                var action = new CreateFolderAction(connection, newPaths.ToArray());

                action.Completed += Action_Completed;
                action.RunAsync();
            }
        }

        private void Action_Completed(ActionBase sender)
        {
            sender.Completed -= Action_Completed;

            var action = sender as CreateFolderAction;
            if (action != null && action.Succeeded)
            {
                if (FoldersCreated != null)
                    FoldersCreated(action.NewPaths);

                Program.MainWindow.TrySelectNewNode(delegate(object o)
                {
                    Folder ff = o as Folder;
                    return ff != null && action.NewPaths[0] == ff.opaque_ref;
                }, true, true, true);
            }
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Execute((Folder)selection[0].XenObject, Parent);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return (selection.ContainsOneItemOfType<Folder>()
                    || selection.ContainsOneItemOfType<GroupingTag>(t => t.Grouping is OrganizationViewFolders))
                   && ConnectionAvailable();
        }

        private bool ConnectionAvailable()
        {
            foreach (IXenConnection connection in MainWindowCommandInterface.GetXenConnectionsCopy())
            {
                if (connection.IsConnected)
                    return true;
            }
            return false;
        }

        public override string MenuText
        {
            get
            {
                return Messages.NEW_FOLDER;
            }
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._000_Folder_open_h32bit_16;
            }
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            return ConnectionAvailable() ? base.GetCantExecuteReasonCore(item) : Messages.FOLDER_NO_CONNECTION;
        }
    }
}
