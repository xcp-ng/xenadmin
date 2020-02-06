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
using System.Linq;
using XenAPI;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Commands.Controls;
using XenAdmin.Dialogs;


namespace XenAdmin.Commands
{
    class DeleteVirtualDiskCommand : Command
    {
        /// <summary>
        /// Allows auto unplug and delete on running VMs
        /// </summary>
        public bool AllowRunningVMDelete = false;

        /// <summary>
        /// Allows deletion of the VDI when multiple VMs are using this VDI. If the VMs are running you also need to set AllowRunningVMDelete
        /// </summary>
        public bool AllowMultipleVBDDelete = false;

        public DeleteVirtualDiskCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
            
        }

        public override string ContextMenuText
        {
            get
            {
                var selection = GetSelection();
                if (selection.Count > 1)
                    return Messages.MAINWINDOW_DELETE_OBJECTS;

                return selection.AsXenObjects<VDI>().All(v => v.is_a_snapshot)
                    ? Messages.DELETE_SNAPSHOT_MENU_ITEM
                    : Messages.DELETE_VIRTUAL_DISK;
            }
        }


        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<VDI>(CanExecute);
        }

        protected override bool ConfirmationRequired
        {
            get
            {
                // We always do some sort of confirmation for this delete
                return true;
            }
        }

        protected override string ConfirmationDialogText
        {
            get
            {
                if (!NeedMultipleWarnings)
                {
                    // In the single warning type case we use the in built confirmation dialog to show a single warning
                    // if there are mixed vdi types then we will use the multiple warning dialog in confirm() override
                    SelectedItemCollection selectedItems = GetSelection();
                    VDI vdi = selectedItems[0].XenObject as VDI;
                    SR sr = vdi.Connection.Resolve<SR>(vdi.SR);
                    bool single = selectedItems.Count == 1;

                    var typ = vdi.VDIType();
                    switch (typ)
                    {
                        case VDI.FriendlyType.SNAPSHOT:
                            return single ? Messages.MESSAGEBOX_DELETE_SNAPSHOT : Messages.MESSAGEBOX_DELETE_SNAPSHOT_MULTIPLE;
                        case VDI.FriendlyType.ISO:
                            return single ? string.Format(Messages.MESSAGEBOX_DELETE_ISO, Helpers.GetName(vdi)) : Messages.MESSAGEBOX_DELETE_ISO_MULTIPLE;
                        case VDI.FriendlyType.SYSTEM_DISK:
                            return single ? Messages.MESSAGEBOX_DELETE_SYS_DISK : Messages.MESSAGEBOX_DELETE_SYS_DISK_MULTIPLE;
                        case VDI.FriendlyType.VIRTUAL_DISK:
                            return single ? Messages.MESSAGEBOX_DELETE_VD : Messages.MESSAGEBOX_DELETE_VD_MULTIPLE;
                        case VDI.FriendlyType.NONE:
                            return "";
                    }
                }
                return base.ConfirmationDialogText;
            }
        }

        protected override string ConfirmationDialogTitle
        {
            get
            {
                if (!NeedMultipleWarnings)
                {
                    // In the single warning type case we use the in built confirmation dialog to show a single warning
                    // if there are mixed vdi types then we will use the multiple warning dialog in confirm() override
                    SelectedItemCollection selectedItems = GetSelection();
                    VDI vdi = selectedItems[0].XenObject as VDI;
                    bool single = selectedItems.Count == 1;

                    var typ = vdi.VDIType();
                    switch (typ)
                    {
                        case VDI.FriendlyType.SNAPSHOT:
                            return single ? Messages.MESSAGEBOX_DELETE_SNAPSHOT_TITLE : Messages.MESSAGEBOX_DELETE_SNAPSHOTS_TITLE_MULTIPLE;
                        case VDI.FriendlyType.ISO:
                            return single ? Messages.MESSAGEBOX_DELETE_ISO_TITLE : Messages.MESSAGEBOX_DELETE_ISO_TITLE_MULTIPLE;
                        case VDI.FriendlyType.SYSTEM_DISK:
                            return single ? Messages.MESSAGEBOX_DELETE_SYS_DISK_TITLE : Messages.MESSAGEBOX_DELETE_SYS_DISK_TITLE_MULTIPLE;
                        case VDI.FriendlyType.VIRTUAL_DISK:
                            return single ? Messages.MESSAGEBOX_DELETE_VD_TITLE : Messages.MESSAGEBOX_DELETE_VD_TITLE_MUTLIPLE;
                        case VDI.FriendlyType.NONE:
                            return "";     
                    }
                }
                return base.ConfirmationDialogTitle;
            }
        }

        private bool? _needMultipleWarnings = null;
        private bool NeedMultipleWarnings
        {
            get
            {
                // While just O(n) it does cast an entire list, so we cache the first result.
                if (_needMultipleWarnings.HasValue)
                    return _needMultipleWarnings.Value;

                _needMultipleWarnings = false;
                SelectedItemCollection selection = GetSelection();
                VDI.FriendlyType current = VDI.FriendlyType.NONE;
                VDI.FriendlyType previous = VDI.FriendlyType.NONE;
                if (selection.Count > 1)
                {
                    for (int i = 0; i < selection.Count; i++)
                    {
                        VDI v = selection[i].XenObject as VDI;
                        if (v == null)
                            current = VDI.FriendlyType.NONE;
                        else
                            current = v.VDIType();

                        if (i > 0 && current != previous)
                        {
                            _needMultipleWarnings = true;
                            break;
                        }
                        previous = current;
                    }
                }
                return _needMultipleWarnings.Value;
            }
        }

        protected override bool Confirm()
        {
            if (Program.RunInAutomatedTestMode)
                return true;

            if (NeedMultipleWarnings)
            {
                MultipleWarningDialog warningDialog = new MultipleWarningDialog(
                    Messages.MESSAGEBOX_DELETE_VD_TITLE_MUTLIPLE,
                    Messages.MULTI_VDI_DELETE_WARNING,
                    Messages.DELETE_ALL_BUTTON_LABEL);

                SelectedItemCollection selectedItems = GetSelection();
                List<VDI> snapshots = new List<VDI>();
                List<VDI> isos = new List<VDI>();
                List<VDI> systemVDisks = new List<VDI>();
                List<VDI> virtualDisks = new List<VDI>();
                foreach (VDI vdi in selectedItems.AsXenObjects<VDI>())
                {
                    var typ = vdi.VDIType();
                    switch (typ)
                    {
                        case VDI.FriendlyType.SNAPSHOT:
                            snapshots.Add(vdi); 
                            break;
                        case VDI.FriendlyType.ISO:
                            isos.Add(vdi); 
                            break;
                        case VDI.FriendlyType.SYSTEM_DISK:
                            systemVDisks.Add(vdi); 
                            break;
                        case VDI.FriendlyType.VIRTUAL_DISK:
                            virtualDisks.Add(vdi); 
                            break;
                        case VDI.FriendlyType.NONE:
                            break;
                    }
                }
                if (snapshots.Count == 1)
                {
                    warningDialog.AddWarningMessage(
                        Messages.MESSAGEBOX_DELETE_SNAPSHOT_TITLE,
                        Messages.WARNING_DELETE_SNAPSHOT,
                        snapshots.ConvertAll<IXenObject>(delegate(VDI v){return (IXenObject)v;}));
                }
                else if (snapshots.Count > 1)
                {
                    warningDialog.AddWarningMessage(
                        Messages.MESSAGEBOX_DELETE_SNAPSHOTS_TITLE_MULTIPLE,
                        Messages.WARNING_DELETE_SNAPSHOT_MULTIPLE,
                        snapshots.ConvertAll<IXenObject>(delegate(VDI v){return (IXenObject)v;}));
                }

                if (isos.Count == 1)
                {
                    warningDialog.AddWarningMessage(
                        Messages.MESSAGEBOX_DELETE_ISO_TITLE,
                        Messages.WARNING_DELETE_ISO,
                        isos.ConvertAll<IXenObject>(delegate(VDI v) { return (IXenObject)v; }));
                }
                else if (isos.Count > 1)
                {
                    warningDialog.AddWarningMessage(
                        Messages.MESSAGEBOX_DELETE_ISO_TITLE_MULTIPLE,
                        Messages.WARNING_DELETE_ISO_MULTIPLE,
                        isos.ConvertAll<IXenObject>(delegate(VDI v) { return (IXenObject)v; }));
                }

                if (systemVDisks.Count == 1)
                {
                    warningDialog.AddWarningMessage(
                        Messages.MESSAGEBOX_DELETE_SYS_DISK_TITLE,
                        Messages.WARNING_DELETE_SYS_DISK,
                        systemVDisks.ConvertAll<IXenObject>(delegate(VDI v) { return (IXenObject)v; }));
                }
                else if (systemVDisks.Count > 1)
                {
                    warningDialog.AddWarningMessage(
                        Messages.MESSAGEBOX_DELETE_SYS_DISK_TITLE_MULTIPLE,
                        Messages.WARNING_DELETE_SYS_DISK_MULTIPLE,
                        systemVDisks.ConvertAll<IXenObject>(delegate(VDI v) { return (IXenObject)v; }));
                }

                if (virtualDisks.Count == 1)
                {
                    warningDialog.AddWarningMessage(
                        Messages.MESSAGEBOX_DELETE_VD_TITLE,
                        Messages.WARNING_DELETE_VD,
                        virtualDisks.ConvertAll<IXenObject>(delegate(VDI v) { return (IXenObject)v; }));
                }
                else if (virtualDisks.Count > 1)
                {
                    warningDialog.AddWarningMessage(
                        Messages.MESSAGEBOX_DELETE_VD_TITLE_MUTLIPLE,
                        Messages.WARNING_DELETE_VD_MULTIPLE,
                        virtualDisks.ConvertAll<IXenObject>(delegate(VDI v) { return (IXenObject)v; }));
                }
                return warningDialog.ShowDialog(Parent) == DialogResult.Yes;
            }
            else
            {
                return base.Confirm();
            }
        }

        protected bool CanExecute(VDI vdi)
        {
            if (vdi == null)
                return false;
            SR sr = vdi.Connection.Resolve<SR>(vdi.SR);
            if (sr == null)
                return false;
            if (vdi.Locked)
                return false;
            if (sr.Physical())
                return false;
            if (sr.IsToolsSR())
                return false;
            if (vdi.IsUsedByHA())
            {
                return false;
            }
            List<VBD> vbds = vdi.Connection.ResolveAll<VBD>(vdi.VBDs);
            if (vbds.Count > 1 && !AllowMultipleVBDDelete)
                return false;
            foreach (VBD vbd in vbds)
            {
                VM vm = vdi.Connection.Resolve<VM>(vbd.VM);

                if (vdi.type == vdi_type.system)
                {
                    
                    if (vm.power_state == vm_power_state.Running)
                        return false;
                }
                if (vbd.Locked)
                    return false;
                if (vbd.currently_attached)
                {
                    //Check if we can unplug
                    DeactivateVBDCommand cmd = new DeactivateVBDCommand(Program.MainWindow, vbd);
                    if (!AllowRunningVMDelete || !cmd.CanExecute())
                        return false;
                }
            }
            if (sr.HBALunPerVDI())
                return true;
            if (!vdi.allowed_operations.Contains(vdi_operations.destroy))
            {
                if (AllowRunningVMDelete)
                {
                    // We deliberately DONT call allowed operations because we assume we know better :)
                    // Xapi will think we can't delete because VBDs are plugged. We are going to unplug them.

                    // Known risks of this method that will make us fail because we are disrespecting xapi:
                    // - someone else is calling a delete on this vdi already, altering the allowed ops
                    // - the storage manager cannot perform a delete on the SR due to drivers

                    return true;
                }
                return false;
            }
            return true;
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            VDI vdi = item as VDI;
            if (vdi == null)
                return base.GetCantExecuteReasonCore(item);

            SR sr = vdi.Connection.Resolve<SR>(vdi.SR);
            if (sr == null)
                return Messages.SR_COULD_NOT_BE_CONTACTED;

            VDI.FriendlyType vdiType = vdi.VDIType();
           
            if (vdi.Locked)
                return vdiType == VDI.FriendlyType.SNAPSHOT ? Messages.CANNOT_DELETE_SNAPSHOT_IN_USE
                    : vdiType == VDI.FriendlyType.ISO ? Messages.CANNOT_DELETE_ISO_IN_USE
                    : Messages.CANNOT_DELETE_VD_IN_USE;

            if (sr.Physical())
                return FriendlyErrorNames.VDI_IS_A_PHYSICAL_DEVICE;

            if (sr.IsToolsSR())
                return Messages.CANNOT_DELETE_TOOLS_SR;

            if (vdi.IsUsedByHA())
                return Messages.CANNOT_DELETE_HA_VD;

            if (vdi.IsMetadataForDR())
                return Messages.CANNOT_DELETE_DR_VD;

            List<VBD> vbds = vdi.Connection.ResolveAll<VBD>(vdi.VBDs);
            if (vbds.Count > 1 && !AllowMultipleVBDDelete)
                return Messages.CANNOT_DELETE_VDI_MULTIPLE_VBDS;

            foreach (VBD vbd in vbds)
            {
                VM vm = vdi.Connection.Resolve<VM>(vbd.VM);

                if (vdiType == VDI.FriendlyType.SYSTEM_DISK)
                {
                  

                    if (vm.power_state == vm_power_state.Running)
                        return string.Format(
                            Messages.CANNOT_DELETE_IN_USE_SYS_VD,
                            Helpers.GetName(vm).Ellipsise(20));
                }
                if (vbd.Locked)
                    return vdiType == VDI.FriendlyType.SNAPSHOT ? Messages.CANNOT_DELETE_SNAPSHOT_IN_USE
                    : vdiType == VDI.FriendlyType.ISO ? Messages.CANNOT_DELETE_ISO_IN_USE
                    : Messages.CANNOT_DELETE_VD_IN_USE;

                if (vbd.currently_attached)
                {
                    if (!AllowRunningVMDelete)
                    {
                        return string.Format(Messages.CANNOT_DELETE_VDI_ACTIVE_ON,
                            Helpers.GetName(vm).Ellipsise(20));
                    }
                    DeactivateVBDCommand cmd = new DeactivateVBDCommand(Program.MainWindow, vbd);
                    if (!cmd.CanExecute())
                    {
                        var reasons = cmd.GetCantExecuteReasons();
                        return reasons.Count > 0
                            ? string.Format(Messages.CANNOT_DELETE_CANNOT_DEACTIVATE_REASON,
                                Helpers.GetName(vm).Ellipsise(20), reasons.ElementAt(0).Value)
                            : Messages.UNKNOWN;
                    }
                }
            }

            // This is a necessary final check, there are other blocking reasons non covered in this method
            // Known examples:
            // - someone else is calling a delete on this vdi already, altering the allowed ops
            // - the storage manager cannot perform a delete on the SR due to drivers
            if (!vdi.allowed_operations.Contains(vdi_operations.destroy))
                return vdiType == VDI.FriendlyType.SNAPSHOT ? Messages.CANNOT_DELETE_SNAPSHOT_GENERIC
                    : vdiType == VDI.FriendlyType.ISO ? Messages.CANNOT_DELETE_ISO_GENERIC
                    : Messages.CANNOT_DELETE_VD_GENERIC;

            return base.GetCantExecuteReasonCore(item);
        }

        protected override CommandErrorDialog GetErrorDialogCore(IDictionary<IXenObject, string> cantExecuteReasons)
        {
            return new CommandErrorDialog(Messages.ERROR_DESTROYING_STORAGE_ITEMS_TITLE, Messages.ERROR_DESTROYING_STORAGE_ITEMS_MESSAGE, cantExecuteReasons);
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<AsyncAction> actionsToComplete = new List<AsyncAction>();
            List<VM> deletedVMSnapshots = new List<VM>();

            foreach (VDI vdi in selection.AsXenObjects<VDI>())
            {
                if (vdi.Locked || !vdi.Show(XenAdmin.Properties.Settings.Default.ShowHiddenVMs))
                    continue;

                actionsToComplete.AddRange(getDestroyVDIAction(vdi, deletedVMSnapshots));
            }

            if (actionsToComplete.Count == 0)
                return;

            if (actionsToComplete.Count > 1)
                RunMultipleActions(actionsToComplete, Messages.ACTION_DELETING_MULTIPLE_STORAGE_ITEMS_TITLE, Messages.ACTION_DELETING_MULTIPLE_STORAGE_ITEMS_STATUS, Messages.COMPLETED, true);
            else
                actionsToComplete[0].RunAsync();
        }

        private List<AsyncAction> getDestroyVDIAction(VDI vdi, List<VM> deletedVMSnapshots)
        {
            List<AsyncAction> actions = new List<AsyncAction>();

            // Destroy the entire snapshot if it exists.  Else destroy disk
            if (vdi.is_a_snapshot && vdi.GetVMs().Count >= 1)
            {
                foreach (VM vm in vdi.GetVMs())
                {
                    if (!vm.is_a_snapshot || deletedVMSnapshots.Contains(vm))
                        continue;

                    AsyncAction action = new VMSnapshotDeleteAction(vm);
                    actions.Add(action);
                    deletedVMSnapshots.Add(vm);
                }
            }
            else
            {
                SR sr = vdi.Connection.Resolve(vdi.SR);
                if (sr == null)
                {
                    // Nothing we can do here, but this should have been caught in the getcantexecutereason method and prompted
                    return actions;
                }
                DestroyDiskAction a = new DestroyDiskAction(vdi);
                a.AllowRunningVMDelete = AllowRunningVMDelete;
                actions.Add(a);
            }

            return actions;
        }
    }
}
