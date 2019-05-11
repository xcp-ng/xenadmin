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
using System.Windows.Forms;
using XenAPI;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Explicit request to save the current session. Turn the SaveSession property to true and save the
    /// session details - this will cause a master-password dialog to be shown.
    /// </summary>
    internal class SaveSessionCommand : Command
    {
        public SaveSessionCommand()
        {
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (XenAdmin.Properties.Settings.Default.RequirePass && Program.MasterPassword == null)
            {
                // A master password is set, but they didn't enter it. Don't let them fiddle with the saved mater password session
                return false;
            }
            return base.CanExecuteCore(selection);
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            if (XenAdmin.Properties.Settings.Default.RequirePass && Program.MasterPassword == null)
                return Messages.ENTER_MASTER_PASSWORD_TO_ACCESS_SETTINGS_TT;

            return base.GetCantExecuteReasonCore(item);
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            new Dialogs.RestoreSession.SaveAndRestoreDialog(true).ShowDialog(Parent);
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_SAVE_AND_RESTORE;
            }
        }

        public override string ShortcutKeyDisplayString
        {
            get
            {
                return Messages.MAINWINDOW_CTRL_S;
            }
        }

        public override Keys ShortcutKeys
        {
            get
            {
                return Keys.Control | Keys.S;
            }
        }
    }
}
