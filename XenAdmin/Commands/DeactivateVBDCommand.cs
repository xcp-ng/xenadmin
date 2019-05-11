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
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Actions;


namespace XenAdmin.Commands
{
    class DeactivateVBDCommand : Command
    {
        /// <summary>
        /// Deactivates a selection of VBDs
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="selection"></param>
        public DeactivateVBDCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {

        }

        /// <summary>
        /// Deactivates a single VBD
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="vbd"></param>
        public DeactivateVBDCommand(IMainWindow mainWindow, VBD vbd)
            : base(mainWindow, vbd)
        {

        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.MESSAGEBOX_DEACTIVATE_VD_TITLE;
            }
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<VBD>() && selection.AtLeastOneXenObjectCan<VBD>(CanExecute);
        }

        // We only need to check for IO Drivers for hosts before Ely
        private bool AreIODriversNeededAndMissing(VM vm)
        {
            if (Helpers.ElyOrGreater(vm.Connection))
            {
                return false;
            }

            return !vm.GetVirtualisationStatus().HasFlag(VM.VirtualisationStatus.IO_DRIVERS_INSTALLED);
        }

        private bool CanExecute(VBD vbd)
        {
            VDI vdi = vbd.Connection.Resolve<VDI>(vbd.VDI);
            VM vm = vbd.Connection.Resolve<VM>(vbd.VM);
            if (vm == null || vm.not_a_real_vm() || vdi == null || vdi.Locked || vbd.Locked)
                return false;
            if (vm.power_state != vm_power_state.Running)
                return false;
            if (vdi.type == vdi_type.system && vbd.GetIsOwner())
                return false;
            if (AreIODriversNeededAndMissing(vm))
                return false;
            if (!vbd.currently_attached)
                return false;

            return vbd.allowed_operations.Contains(vbd_operations.unplug);
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            VBD vbd = item as VBD;
            if (vbd == null)
                return base.GetCantExecuteReasonCore(item);

            VDI vdi = vbd.Connection.Resolve<VDI>(vbd.VDI);
            VM vm = vbd.Connection.Resolve<VM>(vbd.VM);
            if (vm == null || vm.not_a_real_vm() || vdi == null)
                return base.GetCantExecuteReasonCore(item);


            SR sr = vdi.Connection.Resolve<SR>(vdi.SR);
            if (sr == null)
                return Messages.SR_COULD_NOT_BE_CONTACTED;

            if (vdi.Locked)
            {
                var vdiType = vdi.VDIType();
                return vdiType == VDI.FriendlyType.SNAPSHOT
                    ? Messages.CANNOT_DEACTIVATE_SNAPSHOT_IN_USE
                    : vdiType == VDI.FriendlyType.ISO
                        ? Messages.CANNOT_DEACTIVATE_ISO_IN_USE
                        : Messages.CANNOT_DEACTIVATE_VDI_IN_USE;
            }

            if (vm.power_state != vm_power_state.Running)
                return string.Format(
                    Messages.CANNOT_DEACTIVATE_VDI_VM_NOT_RUNNING,
                    Helpers.GetName(vm).Ellipsise(50));

            if (vdi.type == vdi_type.system && vbd.GetIsOwner())
                return Messages.TOOLTIP_DEACTIVATE_SYSVDI;

            if (AreIODriversNeededAndMissing(vm))
                return string.Format(
                    vm.HasNewVirtualisationStates() ? Messages.CANNOT_DEACTIVATE_VDI_NEEDS_IO_DRIVERS : Messages.CANNOT_DEACTIVATE_VDI_NEEDS_TOOLS,
                    Helpers.GetName(vm).Ellipsise(50));

            if (!vbd.currently_attached)
                return string.Format(Messages.CANNOT_DEACTIVATE_NOT_ACTIVE, Helpers.GetName(vm).Ellipsise(50));

            return base.GetCantExecuteReasonCore(item);
        }

        protected override CommandErrorDialog GetErrorDialogCore(IDictionary<IXenObject, string> cantExecuteReasons)
        {
            return new CommandErrorDialog(Messages.ERROR_DEACTIVATING_MULTIPLE_VDIS_TITLE, Messages.ERROR_DEACTIVATING_MULTIPLE_VDIS_MESSAGE, cantExecuteReasons);
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<AsyncAction> actionsToComplete = new List<AsyncAction>();
            foreach (VBD vbd in selection.AsXenObjects<VBD>())
            {
                if (vbd.Locked)
                    continue;

                actionsToComplete.Add(getDeactivateVBDAction(vbd));
            }

            if (actionsToComplete.Count == 0)
                return;

            if (actionsToComplete.Count > 1)
                RunMultipleActions(actionsToComplete, Messages.ACTION_DEACTIVATING_MULTIPLE_VDIS_TITLE, Messages.ACTION_DEACTIVATING_MULTIPLE_VDIS_STATUS, Messages.COMPLETED, true);
            else
                actionsToComplete[0].RunAsync();
        }

        private AsyncAction getDeactivateVBDAction(VBD vbd)
        {
            VDI vdi = vbd.Connection.Resolve<VDI>(vbd.VDI);
            VM vm = vbd.Connection.Resolve<VM>(vbd.VM);
            String title = String.Format(Messages.ACTION_DISK_DEACTIVATING_TITLE, vdi.Name(), vm.Name());
            String startDesc = Messages.ACTION_DISK_DEACTIVATING;
            String endDesc = Messages.ACTION_DISK_DEACTIVATED;

            AsyncAction action = new DelegatedAsyncAction(vbd.Connection,
                title, startDesc, endDesc,session => VBD.unplug(session, vbd.opaque_ref), "vbd.unplug");
            action.VM = vm;
            return action;
        }
    }
}
