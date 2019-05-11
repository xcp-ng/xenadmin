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
using System.Linq;
using XenAdmin.Core;
using XenAPI;

using XenAdmin.Wizards.ExportWizard;

namespace XenAdmin.Commands
{
    class ExportCommand : Command
    {
        public ExportCommand()
        { }

        public ExportCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        { }

        public override string MenuText { get { return Messages.MENU_EXPORT; } }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            Host hostAncestor = selection.HostAncestorFromConnection;
            Pool poolAncestor = selection.PooAncestorFromConnection;

            if ((poolAncestor != null || hostAncestor != null) //CA-61207: this check ensures there's no cross-pool selection
                && (selection.FirstIs<Pool>() || selection.FirstIs<Host>() || selection.FirstIsRealVM || selection.FirstIs<VM_appliance>()))
            {
				if (selection.AllItemsAre<VM>())
					return selection.AtLeastOneXenObjectCan<VM>(CanExportVm);

				if (selection.AllItemsAre<VM_appliance>())
				{
					if (selection.Count != 1)
						return false;

					var appliance = ((VM_appliance)selection.FirstAsXenObject);
					return appliance.VMs.TrueForAll(vmRef =>
					                                	{
					                                		var vm = appliance.Connection.Resolve(vmRef);
					                                		return vm != null
					                                		       && CanExportVm(vm);
					                                	});
				}

                if ((hostAncestor != null && hostAncestor.enabled && hostAncestor.IsLive() && selection[0].Connection.IsConnected)
                    || (poolAncestor != null && Helpers.PoolHasEnabledHosts(poolAncestor)))
                {
                    var vms = selection.FirstAsXenObject.Connection.Cache.VMs.Where(vm => vm.is_a_real_vm() && CanExportVm(vm) && vm.Show(Properties.Settings.Default.ShowHiddenVMs)).ToList();
                    if (vms.Count > 0)
                        return vms.Any(CanExportVm);
                }
            }

            return false;
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            VM vm = item as VM;
            if (vm != null && vm.power_state != vm_power_state.Halted)
                return Messages.SHUTDOWN_BEFORE_EXPORT;

            if (item != null && item.Connection != null)
            {
                var vms = item.Connection.Cache.VMs.Where(xvm =>
                    xvm.is_a_real_vm() && CanExportVm(xvm) &&
                    xvm.Show(Properties.Settings.Default.ShowHiddenVMs)).ToList();

                if (vms.Count == 0)
                    return Messages.NO_HALTED_VMS;
            }
            return string.Empty;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            var con = selection.GetConnectionOfFirstItem();
            MainWindowCommandInterface.ShowPerConnectionWizard(con, new ExportApplianceWizard(con, selection));
        }

		private bool CanExportVm(VM vm)
		{
			return !vm.is_a_template && !vm.Locked && vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.export);
		}
    }
}
