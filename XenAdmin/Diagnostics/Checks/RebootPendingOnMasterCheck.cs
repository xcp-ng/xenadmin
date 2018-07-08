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
using XenAdmin.Diagnostics.Hotfixing;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAdmin.Diagnostics.Problems.PoolProblem;
using XenAPI;

namespace XenAdmin.Diagnostics.Checks
{
    class RestartHostOrToolstackPendingOnMasterCheck : Check
    {
        public string UpdateUuid { get; private set; }
        private readonly Pool pool;

        public RestartHostOrToolstackPendingOnMasterCheck(Pool pool, string updateUuid)
            : base(Helpers.GetMaster(pool.Connection))
        {
            this.pool = pool;
            this.UpdateUuid = updateUuid;
        }

        protected override Problem RunCheck()
        {
            if (!Host.IsLive())
                return new HostNotLiveWarning(this, Host);
            
            var elyOrGreater = Helpers.ElyOrGreater(Host);

            double bootTime = Host.BootTime();
            double agentStart = Host.AgentStartTime();

            //check reboot
            if (elyOrGreater)
            {
                foreach (var updateRef in Host.updates_requiring_reboot)
                {
                    var update = Host.Connection.Resolve(updateRef);

                    if (string.IsNullOrEmpty(UpdateUuid) || //automated mode, any update
                        (update != null && string.Equals(update.uuid, UpdateUuid, System.StringComparison.InvariantCultureIgnoreCase))) //normal mode the given update
                    {
                        return new MasterIsPendingRestartHostProblem(this, pool);
                    }
                }
            }
            else
            {
                if (bootTime == 0.0 || agentStart == 0.0)
                    return null; //fine

                var hostRestartRequiredPatches = Host.AppliedPatches().Where(p => p.after_apply_guidance.Contains(after_apply_guidance.restartHost) && ((double)Util.ToUnixTime(p.AppliedOn(Host)) > agentStart));

                foreach (Pool_patch patch in hostRestartRequiredPatches)
                {
                    if (string.IsNullOrEmpty(UpdateUuid) //automated mode, any update
                            || string.Equals(patch.uuid, UpdateUuid, System.StringComparison.InvariantCultureIgnoreCase)) //normal mode the given update
                    {
                        return new MasterIsPendingRestartHostProblem(this, pool);
                    }
                }
            }

            //check toolstack restart
            var toolstackRestartRequiredPatches = Host.AppliedPatches().Where(p => p.after_apply_guidance.Contains(after_apply_guidance.restartXAPI) && ((double)Util.ToUnixTime(p.AppliedOn(Host)) > agentStart));
            foreach (Pool_patch patch in toolstackRestartRequiredPatches)
            {
                if (string.IsNullOrEmpty(UpdateUuid)) //automated mode
                {
                    return new MasterIsPendingRestartToolstackProblem(this, pool);
                }

                if (!elyOrGreater) //normal mode pre-Ely
                {
                    if (bootTime == 0.0 || agentStart == 0.0)
                        return null; //fine

                    if (string.Equals(patch.uuid, UpdateUuid, System.StringComparison.InvariantCultureIgnoreCase))
                        return new MasterIsPendingRestartToolstackProblem(this, pool);
                }
                else //normal mode Ely+
                {
                    var poolUpdate = Host.Connection.Resolve(patch.pool_update);
                    if (poolUpdate != null && string.Equals(UpdateUuid, poolUpdate.uuid, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        return new MasterIsPendingRestartToolstackProblem(this, pool);
                    }
                }
            }
            return null;
        }

        public override string Description
        {
            get { return Messages.PENDING_RESTART_CHECK; }
        }
    }
}
