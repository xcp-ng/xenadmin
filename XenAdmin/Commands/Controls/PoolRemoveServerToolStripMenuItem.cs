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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Core;


namespace XenAdmin.Commands
{
    internal class PoolRemoveServerToolStripMenuItem : CommandToolStripMenuItem
    {
        public PoolRemoveServerToolStripMenuItem()
            : base(new PoolRemoveServerCommand(), false)
        {
            base.DropDownItems.Add(new ToolStripMenuItem());
        }

        public PoolRemoveServerToolStripMenuItem(IMainWindow mainWindow, IEnumerable<SelectedItem> selection, bool inContextMenu)
            : base(new PoolRemoveServerCommand(mainWindow, selection), inContextMenu)
        {
            base.DropDownItems.Add(new ToolStripMenuItem());
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Command Command => base.Command;

        protected override void OnDropDownOpening(EventArgs e)
        {
            DropDownItems.Clear();

            var hosts = Command.GetSelection().GetConnectionOfAllItems().Cache.Hosts;

            foreach (var host in hosts)
            {
                if (!RemoveHostFromPoolCommand.CanExecute(host))
                    continue;

                var cmd = new RemoveHostFromPoolCommand(Command.MainWindowCommandInterface, host);
                DropDownItems.Add(new CommandToolStripMenuItem(cmd, host.Name(), Images.GetImage16For(host)));
            }
        }

        private class PoolRemoveServerCommand : Command
        {
            public PoolRemoveServerCommand()
            {
            }

            public PoolRemoveServerCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
                : base(mainWindow, selection)
            {
            }

            public override string MenuText => Messages.REMOVE_SERVER_MENU_ITEM;

            protected override bool CanExecuteCore(SelectedItemCollection selection)
            {
                var connection = selection.GetConnectionOfAllItems();
                var pool = Helpers.GetPool(connection);
                if (pool == null)
                    return false;

                return connection.Cache.Hosts.Any(RemoveHostFromPoolCommand.CanExecute);
            }
        }
    }
}
