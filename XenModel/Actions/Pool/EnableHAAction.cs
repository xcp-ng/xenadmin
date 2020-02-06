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

using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public class EnableHAAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<VM, VMStartupOptions> startupOptions;
        private readonly SR[] heartbeatSRs;
        private readonly long failuresToTolerate;

        public EnableHAAction(Pool pool, Dictionary<VM, VMStartupOptions> startupOptions, List<SR> heartbeatSRs, long failuresToTolerate)
            : base(pool.Connection, string.Format(Messages.ENABLING_HA_ON, Helpers.GetName(pool).Ellipsise(50)), Messages.ENABLING_HA, false)
        {
            if (pool == null)
                throw new ArgumentNullException("pool");
            if (heartbeatSRs.Count == 0)
                throw new ArgumentException("You must specify at least 1 heartbeat SR");

            this.Pool = pool;
            this.startupOptions = startupOptions;
            this.heartbeatSRs = heartbeatSRs.ToArray();
            this.failuresToTolerate = failuresToTolerate;
        }

        public List<SR> HeartbeatSRs
        {
            get { return new List<SR>(heartbeatSRs); }
        }

        protected override void Run()
        {
            if (startupOptions != null)
            {
                double increment = 10 / Math.Max(startupOptions.Count, 1);
                int i = 0;
                // First set any VM restart priorities supplied
                foreach (VM vm in startupOptions.Keys)
                {
                    // Set new VM restart priority and ha_always_run
                    log.DebugFormat("Setting HA priority on {0} to {1}", vm.Name(), startupOptions[vm].HaRestartPriority);
                    XenAPI.VM.SetHaRestartPriority(this.Session, vm, (VM.HA_Restart_Priority)startupOptions[vm].HaRestartPriority);

                    // Set new VM order and start_delay
                    log.DebugFormat("Setting start order on {0} to {1}", vm.Name(), startupOptions[vm].Order);
                    XenAPI.VM.set_order(this.Session, vm.opaque_ref, startupOptions[vm].Order);

                    log.DebugFormat("Setting start order on {0} to {1}", vm.Name(), startupOptions[vm].StartDelay);
                    XenAPI.VM.set_start_delay(this.Session, vm.opaque_ref, startupOptions[vm].StartDelay);

                    this.PercentComplete = (int)(++i * increment);
                }
            }
            this.PercentComplete = 10;

            log.DebugFormat("Setting ha_host_failures_to_tolerate to {0}", failuresToTolerate);
            XenAPI.Pool.set_ha_host_failures_to_tolerate(this.Session, Pool.opaque_ref, failuresToTolerate);

            var refs = heartbeatSRs.Select(sr => new XenRef<SR>(sr.opaque_ref)).ToList();

            try
            {
                log.Debug("Enabling HA for pool " + Pool.Name());
                // NB the line below also performs a pool db sync
                RelatedTask = XenAPI.Pool.async_enable_ha(this.Session, refs, new Dictionary<string, string>());
                PollToCompletion(15, 100);
                log.Debug("Success enabling HA on pool " + Pool.Name());
            }
            catch (Failure f)
            {
                if (f.ErrorDescription.Count > 1 && f.ErrorDescription[0] == "VDI_NOT_AVAILABLE")
                {
                    var vdi = Connection.Resolve(new XenRef<VDI>(f.ErrorDescription[1]));
                    if (vdi != null)
                        throw new Failure(string.Format(FriendlyErrorNames.VDI_NOT_AVAILABLE, vdi.uuid));
                }

                throw;
            }

            this.Description = Messages.COMPLETED;
        }
    }
}
