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
using XenAdmin.Diagnostics.Checks;
using XenAPI;
using XenAdmin.Actions;
using System.Collections.Generic;
using XenAdmin.Actions.VMActions;
using XenAdmin.Commands;
using System.Linq;


namespace XenAdmin.Diagnostics.Problems.PoolProblem
{
    class CPUIncompatibilityProblem : PoolProblem
    {
        private Dictionary<VM, XenRef<Host>> vmsOnHosts = new Dictionary<VM, XenRef<Host>>();

        public CPUIncompatibilityProblem(Check check, Pool pool)
            : base(check, pool)
        {
            foreach (var vm in pool.Connection.Cache.VMs.Where(vm => vm.is_a_real_vm() && vm.power_state != vm_power_state.Halted))
            {
                vmsOnHosts.Add(vm, vm.resident_on);
            }
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            var actions = new List<AsyncAction>();
            foreach (var vm in vmsOnHosts.Keys)
            {
                if (vm.power_state == vm_power_state.Halted)
                    continue;
                if (vm.allowed_operations.Contains(vm_operations.clean_shutdown))
                    actions.Add(new VMCleanShutdown(vm));
                else
                    actions.Add(new VMHardShutdown(vm));
            }
            if (actions.Count > 0)
                return new MultipleAction(Pool.Connection, Messages.ACTION_VMS_SHUTTING_DOWN_TITLE,
                                      Messages.ACTION_VMS_SHUTTING_DOWN_TITLE, Messages.ACTION_SHUT_DOWN, actions, true, false, true);
            return null;
        }

        public override AsyncAction CreateUnwindChangesAction()
        {

            var actions = new List<AsyncAction>();
            foreach (var vmOnHost in vmsOnHosts)
            {
                var vm = vmOnHost.Key;

                // check if the vm is still in the cache and is halted
                if (vm.Connection.Resolve(new XenRef<VM>(vm.opaque_ref)) == null || vm.power_state != vm_power_state.Halted) 
                    continue;

                var startOn = vm.Connection.Resolve(vmOnHost.Value);

                if (startOn != null)
                    actions.Add(new VMStartOnAction(vm, startOn, VMOperationCommand.WarningDialogHAInvalidConfig, VMOperationCommand.StartDiagnosisForm));
                else
                    actions.Add(new VMStartAction(vm, VMOperationCommand.WarningDialogHAInvalidConfig, VMOperationCommand.StartDiagnosisForm));
            }

            if (actions.Count > 0)
                return new MultipleAction(Pool.Connection, Messages.ACTION_VMS_STARTING_ON_TITLE,
                                      Messages.ACTION_VMS_STARTING_ON_TITLE, Messages.ACTION_VM_STARTED, actions, true, false, true);
            return null;
        }

        public override string Description
        {
            get
            {
                return String.Format(Messages.UPGRADEWIZARD_PROBLEM_INCOMPATIBLE_CPUS, Pool);
            }
        }

        public override string HelpMessage
        {
            get
            {
                return Messages.UPGRADEWIZARD_PROBLEM_INCOMPATIBLE_CPUS_HELPMESSAGE;
            }
        }
    }

    class CPUCIncompatibilityWarning : Warning
    {
        private readonly Pool pool;
        private readonly Host host;

        public CPUCIncompatibilityWarning(Check check, Pool pool, Host host)
            : base(check)
        {
            this.pool = pool;
            this.host = host;
        }

        public override string Title
        {
            get { return Check.Description; }
        }

        public override string Description
        {
            get
            {
                return string.Format(Messages.UPGRADEWIZARD_WARNING_INCOMPATIBLE_CPUS, host, pool);
            }
        }
    }
}
