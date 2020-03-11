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
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAPI;
using System.IO;
using XenAdmin.Dialogs;
using System.Drawing;
using XenAdmin.Core;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Shows an open-file dialog for importing a search.
    /// </summary>
    internal class ImportSearchCommand : Command
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _filename;
        private readonly bool _filenameSpecified;

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public ImportSearchCommand()
        {
        }

        public ImportSearchCommand(IMainWindow mainWindow)
            : base(mainWindow)
        {
        }

        public ImportSearchCommand(IMainWindow mainWindow, string filename)
            : base(mainWindow)
        {
            _filename = filename;
            _filenameSpecified = true;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            if (_filenameSpecified)
            {
                Execute(_filename);
            }
            else
            {
                // Showing this dialog has the (undocumented) side effect of changing the working directory
                // to that of the file selected. This means a handle to the directory persists, making
                // it undeletable until the program exits, or the working dir moves on. So, save and
                // restore the working dir...
                string oldDir = "";
                try
                {
                    oldDir = Directory.GetCurrentDirectory();
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.AddExtension = true;
                    dialog.Filter = string.Format(Messages.XENSEARCH_SAVED_SEARCH, BrandManager.ExtensionSearch);
                    dialog.FilterIndex = 0;
                    dialog.RestoreDirectory = true;
                    dialog.DefaultExt = BrandManager.ExtensionSearch;
                    dialog.CheckPathExists = false;

                    if (dialog.ShowDialog(Parent) == DialogResult.OK)
                        Execute(dialog.FileName);
                }
                finally
                {
                    Directory.SetCurrentDirectory(oldDir);
                }
            }
        }

        private void Execute(string filename)
        {
            log.InfoFormat("Importing search from '{0}'", filename);

            if (filename.EndsWith("." + BrandManager.ExtensionSearch) && MainWindowCommandInterface.DoSearch(filename))
            {
                log.InfoFormat("Imported search from '{0}' successfully.", filename);
            }
            else
            {
                log.ErrorFormat("Failed to import search from '{0}'", filename);

                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(
                        SystemIcons.Error,
                        String.Format(Messages.UNABLE_TO_IMPORT_SEARCH, filename, BrandManager.ExtensionSearch),
                        Messages.XENCENTER)))
                {
                    dlg.ShowDialog(Parent);
                }
            }
        }
    }
}
