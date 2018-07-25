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
using XenAdmin.Core;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.VMProblem;
using XenAdmin.Diagnostics.Problems.PoolProblem;
using XenAPI;

namespace XenAdmin.Diagnostics.Checks
{
    class AssertCanEvacuateUpgradeCheck : AssertCanEvacuateCheck
    {
        public AssertCanEvacuateUpgradeCheck(Host host)
            : base(host)
        {
        }

        protected override Problem RunHostCheck()
        {
            // check if the pool has incompatible CPUs
            Pool pool = Helpers.GetPoolOfOne(Host.Connection);
            if (pool != null && PoolHasCpuIncompatibilityProblem(pool))
            {
                if (Host.IsMaster())
                    return new CPUIncompatibilityProblem(this, pool);
                return new CPUCIncompatibilityWarning(this, pool, Host);
            }

            //vCPU configuration check
            foreach (var vm in Host.Connection.Cache.VMs.Where(vm => vm.is_a_real_vm()))
            {
                if (!vm.HasValidVCPUConfiguration())
                    return new InvalidVCPUConfiguration(this, vm);
            }

            return base.RunHostCheck();
        }

        private bool PoolHasCpuIncompatibilityProblem(Pool pool)
        {
            if (pool == null)
                return false;

            if (!pool.Connection.Cache.VMs.Any(vm => vm.is_a_real_vm() && vm.power_state != vm_power_state.Halted))
                return false;

            foreach (var host1 in pool.Connection.Cache.Hosts)
            {
                foreach (var host2 in pool.Connection.Cache.Hosts.Where(h => h.uuid != host1.uuid))
                {
                    if (!PoolJoinRules.CompatibleCPUs(host1, host2, false))
                        return true;
                }
            }
            return false;

        }
    }
}