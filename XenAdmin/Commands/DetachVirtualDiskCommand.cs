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
using System.Linq;
using System.Text;
using XenAPI;
using XenAdmin.Commands.Controls;
using XenAdmin.Actions;
using XenAdmin.Dialogs;


namespace XenAdmin.Commands
{
    class DetachVirtualDiskCommand : Command
    {

        private VM targetVM = null;

        /// <summary>
        /// Detaches a list of VDIs from all their attached VMs (VBD unplug + VBD destroy)
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="selection"></param>
        public DetachVirtualDiskCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
            
        }

        /// <summary>
        /// Detaches the VDI from all VMs (VBD unplug + VBD destroy)
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="vdi"></param>
        public DetachVirtualDiskCommand(IMainWindow mainWindow, VDI vdi)
            : base(mainWindow, vdi)
        {

        }

        /// <summary>
        /// Detaches a list of VDIs from a specific VM (VBD unplug + VBD destroy)
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="selection">The VDIs to detach</param>
        /// <param name="vm">The VM to detach the VDIs from</param>
        public DetachVirtualDiskCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection, VM vm)
            : base(mainWindow, selection)
        {
            this.targetVM = vm;
        }

        /// <summary>
        /// Detaches a specific VDI from a specific VM (VBD unplug + VBD destroy)
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="vdi">The VDI to detach</param>
        /// <param name="vm">The VM to detach it from</param>
        public DetachVirtualDiskCommand(IMainWindow mainWindow, VDI vdi, VM vm)
            : base(mainWindow, vdi)
        {
            this.targetVM = vm;
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.MESSAGEBOX_DETACH_VD_TITLE;
            }
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<VDI>() && selection.AtLeastOneXenObjectCan<VDI>(CanExecute);
        }

        protected bool CanExecute(VDI vdi)
        {  
            if (vdi == null)
                return false;

            foreach (VBD vbd in vdi.Connection.ResolveAll<VBD>(vdi.VBDs))
            {
                if (targetVM == null || vbd.VM.opaque_ref == targetVM.opaque_ref)
                {
                    if (!vbd.currently_attached)
                        continue;

                    DeactivateVBDCommand cmd = new DeactivateVBDCommand(Program.MainWindow, vbd);
                    if (!cmd.CanExecute())
                        return false;
                }
            }

            return true;
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            VDI vdi = item as VDI;
            if (vdi == null)
                return base.GetCantExecuteReasonCore(item);

            
            foreach (VBD vbd in vdi.Connection.ResolveAll<VBD>(vdi.VBDs))
            {
                if (targetVM == null || vbd.VM.opaque_ref == targetVM.opaque_ref)
                {
                    if (!vbd.currently_attached)
                        continue;

                    DeactivateVBDCommand cmd = new DeactivateVBDCommand(Program.MainWindow, vbd);
                    if (!cmd.CanExecute())
                    {
                        var reasons = cmd.GetCantExecuteReasons();
                        return reasons.Count > 0 ? reasons.ElementAt(0).Value : Messages.UNKNOWN;
                    }
                }
            }
            return base.GetCantExecuteReasonCore(item);
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<AsyncAction> actionsToComplete = new List<AsyncAction>();
            foreach (VDI vdi in selection.AsXenObjects<VDI>())
            {
                if (vdi.Locked || !vdi.Show(XenAdmin.Properties.Settings.Default.ShowHiddenVMs))
                    continue;

                actionsToComplete.AddRange(getDetachVDIAction(vdi));
            }

            if (actionsToComplete.Count < 1)
                return;
            
            if (actionsToComplete.Count == 1)
                actionsToComplete[0].RunAsync();
            else
                RunMultipleActions(actionsToComplete, Messages.ACTION_DETACHING_MULTIPLE_VDIS_TITLE, 
                                   Messages.ACTION_DETACHING_MULTIPLE_VDIS_STATUS, Messages.COMPLETED, true);
        }

        private List<AsyncAction> getDetachVDIAction(VDI vdi)
        {
            List<AsyncAction> actions = new List<AsyncAction>();
            foreach (VBD vbd in vdi.Connection.ResolveAll<VBD>(vdi.VBDs))
            {
                if (targetVM == null || vbd.VM.opaque_ref == targetVM.opaque_ref)
                {
                    VM vm = vdi.Connection.Resolve<VM>(vbd.VM);
                    DetachVirtualDiskAction action = new DetachVirtualDiskAction(vdi, vm, true);
                    actions.Add(action);
                }
            }
            return actions;
        }

        protected override bool Confirm()
        {
            if (Program.RunInAutomatedTestMode)
                return true;

            MultipleWarningDialog warningDialog = new MultipleWarningDialog(
                Messages.MESSAGEBOX_DETACH_VD_TITLE_MUTLIPLE,
                Messages.MULTI_VDI_DETACH_WARNING,
                Messages.DETACH_ALL_BUTTON_LABEL);

            SelectedItemCollection selectedItems = GetSelection();
            List<VDI> sysDisks = new List<VDI>();
            List<VDI> regularDisks = new List<VDI>();
            foreach (VDI vdi in selectedItems.AsXenObjects<VDI>())
            {
                if (vdi.type == vdi_type.system)
                    sysDisks.Add(vdi);
                else
                    regularDisks.Add(vdi);
            }
            // Use the regular confirm dialog if we only have one warning type
            if (sysDisks.Count == 0 || regularDisks.Count == 0)
                return base.Confirm();

            warningDialog.AddWarningMessage(
                Messages.MESSAGEBOX_DETACH_SYSTEMVD_TITLE,
                Messages.MESSAGEBOX_DETACH_SYSTEMVD,
                sysDisks.ConvertAll<IXenObject>(delegate(VDI v) { return (IXenObject)v; }));

            warningDialog.AddWarningMessage(
                Messages.MESSAGEBOX_DETACH_VD_TITLE,
                Messages.MESSAGEBOX_DETACH_VD,
                regularDisks.ConvertAll<IXenObject>(delegate(VDI v) { return (IXenObject)v; }));

            return warningDialog.ShowDialog(MainWindowCommandInterface.Form) == System.Windows.Forms.DialogResult.Yes;
        }

        protected override string ConfirmationDialogText
        {
            get
            {
                if (GetSelection().ContainsOneItemOfType<VDI>(delegate(VDI vdi) { return vdi.type == vdi_type.system; }))
                {
                    return Messages.MESSAGEBOX_DETACH_SYSTEMVD;
                }
                else
                {
                    return Messages.MESSAGEBOX_DETACH_VD;
                }
            }
        }

        protected override string ConfirmationDialogTitle
        {
            get
            {
                if (GetSelection().ContainsOneItemOfType<VDI>(delegate(VDI vdi) { return vdi.type == vdi_type.system; }))
                {
                    return Messages.MESSAGEBOX_DETACH_SYSTEMVD_TITLE;
                }
                else
                {
                    return Messages.MESSAGEBOX_DETACH_VD_TITLE;
                }
            }
        }

        protected override bool ConfirmationRequired
        {
            get
            {
                return true;
            }
        }

        protected override CommandErrorDialog GetErrorDialogCore(IDictionary<IXenObject, string> cantExecuteReasons)
        {
            return new CommandErrorDialog(Messages.ERROR_ACTIVATING_VDIS_TITLE, Messages.ERROR_ACTIVATING_VDIS_MESSAGE, cantExecuteReasons);
        }

    }
}
