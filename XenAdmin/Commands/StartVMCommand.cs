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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Network;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using XenAdmin.Properties;
using XenAdmin.Dialogs;
using XenAdmin.Actions.VMActions;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Starts the selected VMs.
    /// </summary>
    internal class StartVMCommand : VMLifeCycleCommand
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public StartVMCommand()
        {
        }

        public StartVMCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public StartVMCommand(IMainWindow mainWindow, VM vm, Control parent)
            : base(mainWindow, vm, parent)
        {
        }

        public StartVMCommand(IMainWindow mainWindow, VM vm)
            : base(mainWindow, vm)
        {
        }

        protected override void Execute(List<VM> vms)
        {
            Dictionary<VM, List<VBD>> brokenCDs = new Dictionary<VM, List<VBD>>();
            foreach (VM vm in vms)
            {
                foreach (VBD vbd in vm.Connection.ResolveAll<VBD>(vm.VBDs))
                {
                    if (vbd.type == vbd_type.CD && !vbd.empty)
                    {
                        VDI vdi = vm.Connection.Resolve<VDI>(vbd.VDI);
                        SR sr = null;
                        if (vdi != null)
                            sr = vm.Connection.Resolve<SR>(vdi.SR);
                        if (vdi == null || sr.IsBroken(true) || sr.IsDetached())
                        {
                            if (!brokenCDs.ContainsKey(vm))
                            {
                                brokenCDs.Add(vm, new List<VBD>());
                            }

                            brokenCDs[vm].Add(vbd);
                        }
                    }
                }
            }
            if (brokenCDs.Count > 0)
            {
                DialogResult d;
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.EJECT_BEFORE_VM_START_MESSAGE_BOX, vms.Count > 1 ? Messages.STARTING_VMS_MESSAGEBOX_TITLE : Messages.STARTING_VM_MESSAGEBOX_TITLE), 
                    new ThreeButtonDialog.TBDButton(Messages.EJECT_BUTTON_LABEL, DialogResult.OK, ThreeButtonDialog.ButtonType.ACCEPT, true), 
                    new ThreeButtonDialog.TBDButton(Messages.IGNORE_BUTTON_LABEL, DialogResult.Ignore), 
                    ThreeButtonDialog.ButtonCancel))
                {
                    d = dlg.ShowDialog(MainWindowCommandInterface.Form);
                }
                if (d == DialogResult.Cancel)
                    return;
                if (d == DialogResult.Ignore)
                    brokenCDs = null;
            }
            RunAction(vms, Messages.ACTION_VMS_STARTING_ON_TITLE, Messages.ACTION_VMS_STARTING_ON_TITLE, Messages.ACTION_VM_STARTED, brokenCDs);
        }

        protected override bool CanExecute(VM vm)
        {
            ReadOnlyCollection<SelectedItem> selection = GetSelection();

            if (!vm.is_a_template && !vm.Locked && vm.allowed_operations.Contains(vm_operations.start) && EnabledTargetExists(vm, selection[0].Connection) && vm.power_state == vm_power_state.Halted)
            {
                return true;
            }
            return false;
        }

        private static bool EnabledTargetExists(VM vm, IXenConnection connection)
        {
            //if the vm has a home server check it's enabled
            Host host = vm.power_state == vm_power_state.Running ? vm.Connection.Resolve(vm.resident_on) : vm.GetStorageHost(false);
            return Helpers.EnabledTargetExists(host, connection);
        }

        protected override string EnabledToolTipText
        {
            get
            {
                return Messages.MAINWINDOW_TOOLBAR_STARTVM;
            }
        }

        public override Image ToolBarImage
        {
            get
            {
                return Images.StaticImages._001_PowerOn_h32bit_24;
            }
        }

        public override string ToolBarText
        {
            get
            {
                return Messages.MAINWINDOW_TOOLBAR_START;
            }
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._001_PowerOn_h32bit_16;
            }
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_START;
            }
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.MAINWINDOW_START_CONTEXT_MENU;
            }
        }

        protected override CommandErrorDialog GetErrorDialogCore(IDictionary<SelectedItem, string> cantExecuteReasons)
        {
            // a start-vm-diagnostic-dialog is shown by VmAction if VMs cant be started.

            return null;
        }

        public override Keys ShortcutKeys
        {
            get
            {
                return Keys.Control | Keys.B;
            }
        }

        public override string ShortcutKeyDisplayString
        {
            get
            {
                return Messages.MAINWINDOW_CTRL_B;
            }
        }

        protected override AsyncAction BuildAction(VM vm)
        {
            return new VMStartAction(vm, VMOperationCommand.WarningDialogHAInvalidConfig, VMOperationCommand.StartDiagnosisForm);
        }
    }
}
