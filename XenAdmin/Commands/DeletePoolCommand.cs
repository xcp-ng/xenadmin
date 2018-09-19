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

using System.Collections.Generic;
using XenAdmin.Actions;
using XenAPI;
using XenAdmin.Network;
using XenAdmin.Dialogs;
using System.Drawing;
using System.Linq;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Deletes a pool. Shows a confirmation dialog.
    /// </summary>
    internal class DeletePoolCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public DeletePoolCommand()
        {
        }

        public DeletePoolCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public DeletePoolCommand(IMainWindow mainWindow, Pool pool)
            : base(mainWindow, pool)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            IXenConnection conn = selection[0].Connection;

            Pool pool = selection[0].PoolAncestor;

            string msg = null;
            if (pool.current_operations.Values.Contains(pool_allowed_operations.ha_enable))
                msg = Messages.POOL_DELETE_HA_ENABLING;
            else if (pool.ha_enabled)
                msg = Messages.POOL_DELETE_HA_ENABLED;

            if (msg != null)
            {
                using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Exclamation,
                    string.Format(msg, pool.Name()), Messages.XENCENTER)))
                {
                    dlg.ShowDialog(Program.MainWindow);
                }
                return;
            }

            if (conn.Cache.HostCount > 1)
            {
                using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Exclamation, Messages.MESSAGEBOX_SLAVES_EJECT, Messages.XENCENTER)))
                {
                    dlg.ShowDialog(Program.MainWindow);
                } 
                return;
            }

            if (conn.IsConnected)
            {
                new DestroyPoolAction(pool).RunAsync();
            }
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.Count == 1)
            {
                IXenConnection connection = selection[0].Connection;

                Pool pool = selection[0].PoolAncestor;
                return pool != null && pool.IsVisible() && connection.Cache.HostCount == 1;
            }
            return false;

        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_DELETE_POOL;
            }
        }
    }
}
