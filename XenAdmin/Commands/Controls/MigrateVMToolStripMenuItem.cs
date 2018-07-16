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

using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Network;
using System.Collections.ObjectModel;


namespace XenAdmin.Commands
{
    internal class MigrateVMToolStripMenuItem : VMOperationToolStripMenuItem
    {

        public MigrateVMToolStripMenuItem()
            : base(new MigrateVmToServerCommand(), false, vm_operations.pool_migrate)
        {
        }

        public MigrateVMToolStripMenuItem(IMainWindow mainWindow, IEnumerable<SelectedItem> selection, bool inContextMenu)
            : base(new MigrateVmToServerCommand(mainWindow, selection), inContextMenu, vm_operations.pool_migrate)
        {
        }

        protected override void AddAdditionalMenuItems(SelectedItemCollection selection)
        {
            if (selection.ToList().All(item => !Helpers.CrossPoolMigrationRestrictedWithWlb(item.Connection)))
            {
                DropDownItems.Add(new ToolStripSeparator());

                VMOperationCommand cmd = new CrossPoolMigrateCommand(Command.MainWindowCommandInterface, selection, false);
                VMOperationToolStripMenuSubItem lastItem = new VMOperationToolStripMenuSubItem(cmd);
                DropDownItems.Add(lastItem);

                VMOperationCommand cmdForce = new CrossPoolMigrateCommand(Command.MainWindowCommandInterface, selection, true);
                VMOperationToolStripMenuSubItem lastItemForce = new VMOperationToolStripMenuSubItem(cmdForce);
                DropDownItems.Add(lastItemForce);
            }            
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Command Command
        {
            get
            {
                return base.Command;
            }
        }

        /// <summary>
        /// This is the command used for enabling the VM's drop-right ToolStripMenuItem "Migrate to Server >"
        /// </summary>
        private class MigrateVmToServerCommand : Command
        {
            public MigrateVmToServerCommand()
            {
            }

            public MigrateVmToServerCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
                : base(mainWindow, selection)
            {
            }

            private static bool CanExecute(VM vm)
            {
                if (vm == null || vm.Connection == null || !vm.Connection.IsConnected)
                    return false;

                if (vm.is_a_template || vm.Locked)
                    return false;

                if (Helpers.FeatureForbidden(vm.Connection, Host.RestrictIntraPoolMigrate))
                        return false;

                return vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.pool_migrate);
            }

            protected override bool CanExecuteCore(SelectedItemCollection selection)
            {
                IXenConnection connection = selection.GetConnectionOfFirstItem();

                bool atLeastOneCanExecute = false;
                foreach (SelectedItem item in selection)
                {
                    //all items should be VMs
                    VM vm = item.XenObject as VM;
                    if (vm == null)
                        return false;

                    // all VMs must be on the same connection
                    if (connection != null && vm.Connection != connection)
                        return false;

                    //at least one VM should be able to execute
                    if (CanExecute(vm))
                        atLeastOneCanExecute = true;

                }
                return atLeastOneCanExecute;
            }

            public override string MenuText
            {
                get
                {
                    return Messages.MAINWINDOW_MIGRATE_TO_SERVER;
                }
            }
        }
    }
}
