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
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAPI;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.SRProblem;


namespace XenAdmin.Diagnostics.Checks
{
    public class PBDsPluggedCheck : Check
    {
        SR srUploadedUpdates;

        public PBDsPluggedCheck(Host host, SR sr = null) : base(host)
        {
            srUploadedUpdates = sr;
        }

        protected override Problem RunCheck()
        {
            if (!Host.IsLive())
                return new HostNotLiveWarning(this, Host);

            IEnumerable<VM> runningOrPausedVMs = GetRunningOrPausedVMs(Host);
            IEnumerable<SR> brokenSRs = PBD.GetSRs(PBD.GetUnpluggedPBDsFor(runningOrPausedVMs));

            foreach (SR sr in brokenSRs)
            {
                if ((sr.shared && !sr.CanBeSeenFrom(Host)) || (!sr.shared && sr.GetStorageHost() == Host && !sr.CanBeSeenFrom(Host)))
                    return new BrokenSR(this, sr, Host);
            }

            if (srUploadedUpdates != null
                && ((srUploadedUpdates.shared && !srUploadedUpdates.CanBeSeenFrom(Host))
                 || (!srUploadedUpdates.shared && srUploadedUpdates.IsBroken())))
            {
                return new BrokenSR(this, srUploadedUpdates, Host);
            }

            return null;
        }

        private static IEnumerable<VM> GetRunningOrPausedVMs(Host host)
        {
            List<VM> runningOrPausedVMs = new List<VM>();


                foreach (VM vm in host.Connection.Cache.VMs)
                {
                    if (vm.power_state == vm_power_state.Running || vm.power_state == vm_power_state.Paused)
                        runningOrPausedVMs.Add(vm);
                }
            

            return runningOrPausedVMs;
        }


        public override string Description
        {
            get { return Messages.PBDS_CHECK_DESCRIPTION; }
        }
    }
}
