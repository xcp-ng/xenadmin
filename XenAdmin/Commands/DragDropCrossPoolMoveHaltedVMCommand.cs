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
using XenAdmin.Controls;
using XenAPI;
using XenAdmin.Core;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using System.Drawing;


namespace XenAdmin.Commands
{
    internal class DragDropCrossPoolMoveHaltedVMCommand : DragDropCommand
    {
        public DragDropCrossPoolMoveHaltedVMCommand(IMainWindow mainWindow, VirtualTreeNode targetNode, IDataObject dragData)
            : base(mainWindow, targetNode, dragData)
        {

        }

        public override string StatusBarText
        {
            get
            {
                if (!CanExecute())
                {
                    Host targetHost = GetTargetNodeAncestorAsXenObjectOrGroupingTag<Host>();
                    
                    if (targetHost != null)
                    {
                        foreach (VM draggedVM in GetDraggedItemsAsXenObjects<VM>())
                        {
                            Host draggedVMHome = draggedVM.Home();

                            if(!LiveMigrateAllowedInVersion(targetHost, draggedVM))
                            {
                                if (IsACrossPoolMigrate(targetHost, draggedVM))
                                {
                                    return Messages.MIGRATION_NOT_ALLOWED_OUTSIDE_POOL;
                                }
                                if (draggedVM.GetStorageHost(true) != null)
                                {
                                    // Non-agile.
                                    return Messages.MIGRATION_NOT_ALLOWED_NO_SHARED_STORAGE;
                                }
                            }
                            
                            if (Helpers.productVersionCompare(Helpers.HostProductVersion(targetHost), Helpers.HostProductVersion(draggedVMHome ?? Helpers.GetMaster(draggedVM.Connection))) < 0)
                                return Messages.OLDER_THAN_CURRENT_SERVER;

                            if (targetHost != draggedVMHome && VMOperationHostCommand.VmCpuIncompatibleWithHost(targetHost, draggedVM))
                                return Messages.MIGRATION_NOT_ALLOWED_CPU_FEATURES;
                        }
                    }
                }
                return null;
            }
        }

        private bool LiveMigrateAllowedInVersion(Host targetHost, VM draggedVM)
        {
            return !Helpers.FeatureForbidden(targetHost.Connection, Host.RestrictCrossPoolMigrate) &&
                   !Helpers.FeatureForbidden(draggedVM.Connection, Host.RestrictCrossPoolMigrate);
        }

        private bool IsACrossPoolMigrate(Host targetHost, VM draggedVM )
        {
            Pool draggedVMPool = Helpers.GetPool(draggedVM.Connection);
            Pool targetPool = targetHost == null ? null : Helpers.GetPool(targetHost.Connection);
            return targetHost != null && (targetPool == null || draggedVMPool == null || targetPool.opaque_ref != draggedVMPool.opaque_ref);
        }

        protected override bool CanExecuteCore()
        {
            Host targetHost = GetTargetNodeAncestorAsXenObjectOrGroupingTag<Host>();

            if (targetHost != null)
            {
                List<VM> draggedVMs = GetDraggedItemsAsXenObjects<VM>();

                if (draggedVMs.Count > 0)
                {
                    foreach (VM draggedVM in draggedVMs)
                    {
                        if (draggedVM == null || draggedVM.is_a_template || draggedVM.Locked || !(draggedVM.power_state == vm_power_state.Halted || draggedVM.power_state == vm_power_state.Suspended))
                            return false;

                        var draggedVMHome = draggedVM.Home();
                        if (draggedVMHome != null && draggedVMHome == targetHost)
                            return false;

                        if (!targetHost.Connection.IsConnected)
                            return false;

                        if (draggedVM.allowed_operations == null || !draggedVM.allowed_operations.Contains(vm_operations.migrate_send))
                            return false;

                        if (Helpers.productVersionCompare(Helpers.HostProductVersion(targetHost), Helpers.HostProductVersion(draggedVMHome ?? Helpers.GetMaster(draggedVM.Connection))) < 0)
                            return false;

                        if (VMOperationHostCommand.VmCpuIncompatibleWithHost(targetHost, draggedVM))
                            return false;
                    }

                    return true;
                }
            }

            return false;
        }

        protected override void ExecuteCore()
        {
            Host targetHost = GetTargetNodeAncestorAsXenObjectOrGroupingTag<Host>();
            List<VM> draggedVMs = GetDraggedItemsAsXenObjects<VM>();

            foreach (VM draggedVM in draggedVMs)
            {
                //Check if agile vm.
                VBD cd = draggedVM.FindVMCDROM();
                if (cd != null)
                {
                    VDI vdi = cd.Connection.Resolve<VDI>(cd.VDI);
                    if (vdi != null)
                    {
                        SR sr = cd.Connection.Resolve<SR>(vdi.SR);
                        if (sr != null && !sr.shared)
                        {
                            using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Exclamation,
                                                                                Messages.DRAG_DROP_LOCAL_CD_LOADED,
                                                                                Messages.DRAG_DROP_LOCAL_CD_LOADED_TITLE)))
                            {
                                dlg.ShowDialog(MainWindowCommandInterface.Form);
                            }
                            return;
                        }
                    }
                }
            }

            if (draggedVMs.TrueForAll(vm => LiveMigrateAllowedInVersion(targetHost, vm)))
            {
                List<SelectedItem> selectedItems = new List<SelectedItem>();
                draggedVMs.ForEach(vm => selectedItems.Add(new SelectedItem(vm)));
                    
                new CrossPoolMoveVMCommand(MainWindowCommandInterface, selectedItems, targetHost, false)
                    .Execute();
            }
        }

        public override VirtualTreeNode HighlightNode
        {
            get
            {
                return CanExecute() ? GetTargetNodeAncestor<Host>() : null;
            }
        }
    }
}
