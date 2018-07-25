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
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Network;
using XenAdmin.Diagnostics.Checks;


namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    class PatchPrecheckOnHostPlanAction : PlanActionWithSession
    {
        private readonly XenServerPatch patch;
        private readonly List<PoolPatchMapping> mappings;
        private readonly Host host;

        public PatchPrecheckOnHostPlanAction(IXenConnection connection, XenServerPatch patch, Host host, List<PoolPatchMapping> mappings)
            : base(connection)
        {
            this.patch = patch;
            this.host = host;
            this.mappings = mappings;
        }

        protected override void RunWithSession(ref Session session)
        {
            var master = Helpers.GetMaster(Connection);
            var mapping = mappings.Find(m => m.XenServerPatch.Equals(patch) && m.MasterHost != null && master != null && m.MasterHost.uuid == master.uuid);

            if (mapping != null && (mapping.Pool_patch != null || mapping.Pool_update != null))
            {
                if (Cancelling)
                    throw new CancelledException();

                try
                {
                    AddProgressStep(string.Format(Messages.UPDATES_WIZARD_RUNNING_PRECHECK, patch.Name, host.Name()));

                    PatchPrecheckCheck check = mapping.Pool_patch == null
                        ? new PatchPrecheckCheck(host, mapping.Pool_update)
                        : new PatchPrecheckCheck(host, mapping.Pool_patch);

                    var problems = check.RunAllChecks();

                    Diagnostics.Problems.Problem problem = null;

                    if (problems != null && problems.Count > 0)
                        problem = problems[0];

                    if (problem != null)
                        throw new Exception(string.Format("{0}: {1}. {2}", host, problem.Title, problem.Description));
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Precheck failed on host {0}", host.Name()), ex);
                    throw;
                }
            }
        }
    }
}
