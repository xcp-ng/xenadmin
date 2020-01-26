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
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public class ApplyXenServerPatchPlanAction : HostPlanAction
    {
        private readonly XenServerPatch xenServerPatch;
        private readonly List<HostUpdateMapping> mappings;

        public ApplyXenServerPatchPlanAction(Host host, XenServerPatch xenServerPatch, List<HostUpdateMapping> mappings)
            : base(host)
        {
            this.xenServerPatch = xenServerPatch;
            this.mappings = mappings;
        }

        protected override void RunWithSession(ref Session session)
        {
            var master = Helpers.GetMaster(Connection);
            var mapping = (from HostUpdateMapping hum in mappings
                let xpm = hum as XenServerPatchMapping
                where xpm != null && xpm.Matches(master, xenServerPatch)
                select xpm).FirstOrDefault();

            if (mapping == null || !mapping.IsValid)
            {
                if (xenServerPatch != null)
                    log.ErrorFormat("Mapping not found for patch {0} on master {1}", xenServerPatch.Uuid, master.uuid);

                throw new Exception("Pool_patch or Pool_update not found.");
            }

            var host = GetResolvedHost();
            try
            {
                // evacuate the host, if needed, before applying the update
                if (mapping.HostsThatNeedEvacuated.Contains(host.uuid))
                    EvacuateHost(ref session);

                AddProgressStep(string.Format(Messages.UPDATES_WIZARD_APPLYING_UPDATE, xenServerPatch.Name,
                    host.Name()));

                XenRef<Task> task = null;

                if (mapping is PoolPatchMapping patchMapping)
                    task = Pool_patch.async_apply(session, patchMapping.Pool_patch.opaque_ref, host.opaque_ref);
                else if (mapping is PoolUpdateMapping updateMapping)
                    task = Pool_update.async_apply(session, updateMapping.Pool_update.opaque_ref, host.opaque_ref);

                PollTaskForResultAndDestroy(Connection, ref session, task);
            }
            catch (Failure f)
            {
                if (f.ErrorDescription.Count > 1 && (f.ErrorDescription[0] == Failure.PATCH_ALREADY_APPLIED || f.ErrorDescription[0] == Failure.UPDATE_ALREADY_APPLIED))
                {
                    log.InfoFormat("The update {0} is already applied on {1}. Ignoring this error.", xenServerPatch.Name, host.Name());
                    ReplaceProgressStep(string.Format(Messages.UPDATES_WIZARD_SKIPPING_UPDATE, xenServerPatch.Name, host.Name()));
                }
                else
                    throw;
            }
        }
    }
}