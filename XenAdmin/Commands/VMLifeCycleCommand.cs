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
using XenAdmin.Actions;
using System.Windows.Forms;
using XenAdmin.Network;
using System.Collections.ObjectModel;
using XenAdmin.Core;
using System.Threading;
using XenAdmin.Actions.VMActions;

namespace XenAdmin.Commands
{
    internal abstract class VMLifeCycleCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        protected VMLifeCycleCommand()
        {
        }

        protected VMLifeCycleCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected VMLifeCycleCommand(IMainWindow mainWindow, VM vm)
            : this(mainWindow, vm, null)
        {
        }

        protected VMLifeCycleCommand(IMainWindow mainWindow, VM vm, Control parent)
            : base(mainWindow, new SelectedItem(vm))
        {
            SetParent(parent);
        }

        protected abstract bool CanExecute(VM vm);

        protected abstract void Execute(List<VM> vms);

        protected void CancelAllTasks(IEnumerable<VM> vms)
        {
            foreach (VM vm in vms)
            {
                foreach (ActionBase action in ConnectionsManager.History)
                {
                    if (vm.Equals(action.VM) && !action.IsCompleted && action.CanCancel)
                    {
                        action.Cancel();
                    }
                }
            }
        }

        protected void RunAction(IEnumerable<VM> vms, string title, string startDescription, string endDescription,
            Dictionary<VM, List<VBD>> vbdsToEjectDict)
        {
            var actions = GetActions(vms, title, startDescription, endDescription, vbdsToEjectDict);
            RunMultipleActions(actions, title, startDescription, endDescription, true);
        }

        private List<AsyncAction> GetActions(IEnumerable<VM> vms, string title, string startDescription,
            string endDescription, Dictionary<VM, List<VBD>> vbdsToEjectDict)
        {
            List<AsyncAction> actions = new List<AsyncAction>();
            foreach (VM vm in vms)
            {
                if (vm.Connection != null && vm.Connection.IsConnected)
                {
                    if (vbdsToEjectDict != null && vbdsToEjectDict.ContainsKey(vm))
                    {
                        List<AsyncAction> subActions = GetVBDsToEjectActions(vm, vbdsToEjectDict[vm]);
                        subActions.Add(BuildAction(vm));
                        actions.Add(new MultipleAction(vm.Connection, title, startDescription, endDescription,
                                                       subActions));
                    }
                    else
                    {
                        actions.Add(BuildAction(vm));
                    }
                }
            }
            return actions;
        }
  
        private List<AsyncAction> GetVBDsToEjectActions(VM vm, List<VBD> vbdsToEject)
        {
            List<AsyncAction> actions = new List<AsyncAction>();
            if (vbdsToEject != null)
            {
                foreach (var vbd in vbdsToEject)
                {
                    actions.Add(new ChangeVMISOAction(vbd.Connection, vm, null, vbd));
                }
            }
            return actions;
        }

        protected abstract AsyncAction BuildAction(VM vm);

        protected sealed override void ExecuteCore(SelectedItemCollection selection)
        {
            List<VM> vms = selection.AsXenObjects<VM>(CanExecute);

            // sort so actions execute in correct order.
            vms.Sort();
            Execute(vms);
        }

        protected sealed override bool CanExecuteCore(SelectedItemCollection selection)
        {
            bool atLeastOneCanExecute = false;

            foreach (SelectedItem item in selection)
            {
                VM vm = item.XenObject as VM;

                if (vm == null || vm.is_a_template || vm.is_a_snapshot)
                {
                    return false;
                }
                else if (CanExecute(vm))
                {
                    atLeastOneCanExecute = true;
                }
            }

            return atLeastOneCanExecute;
        }

        protected string GetCantExecuteNoToolsOrDriversReasonCore(IXenObject item)
        {
            VM vm = item as VM;
            if (vm == null)
                return null;

            var status = vm.GetVirtualisationStatus();
            //trying to guess the reason
            if (vm.HasNewVirtualisationStates())
            {
                if (!status.HasFlag(VM.VirtualisationStatus.IO_DRIVERS_INSTALLED)) //note: this will also be true when the enum is in Unknown state
                    return Messages.VM_MISSING_IO_DRIVERS;
            }
            else
            {
                if (status == 0 || status.HasFlag(VM.VirtualisationStatus.UNKNOWN))
                    return FriendlyErrorNames.VM_MISSING_PV_DRIVERS;

                if (status.HasFlag(VM.VirtualisationStatus.PV_DRIVERS_OUT_OF_DATE))
                    return FriendlyErrorNames.VM_OLD_PV_DRIVERS;
            }

            return null;
        }
    }
}
