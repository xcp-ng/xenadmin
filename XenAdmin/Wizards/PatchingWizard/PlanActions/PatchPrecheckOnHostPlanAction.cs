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
using XenAdmin.Network;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Diagnostics.Problems;


namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    class PatchPrecheckOnHostPlanAction : PlanActionWithSession
    {
        private readonly XenServerPatch xenServerPatch;
        private readonly List<HostUpdateMapping> mappings;
        private readonly Host host;
        private readonly List<string> hostsThatWillRequireReboot;
        private readonly Dictionary<string, List<string>> livePatchAttempts;

        public PatchPrecheckOnHostPlanAction(IXenConnection connection, XenServerPatch xenServerPatch, Host host, List<HostUpdateMapping> mappings, List<string> hostsThatWillRequireReboot, Dictionary<string, List<string>> livePatchAttempts)
            : base(connection)
        {
            this.xenServerPatch = xenServerPatch;
            this.host = host;
            this.mappings = mappings;
            this.hostsThatWillRequireReboot = hostsThatWillRequireReboot;
            this.livePatchAttempts = livePatchAttempts;
        }

        protected override void RunWithSession(ref Session session)
        {
            var master = Helpers.GetMaster(Connection);

            var mapping = (from HostUpdateMapping hum in mappings
                let xpm = hum as XenServerPatchMapping
                where xpm != null && xpm.Matches(master, xenServerPatch)
                select xpm).FirstOrDefault();

            if (mapping == null || !mapping.IsValid)
                return;

            var livePatchStatus = new Dictionary<string, livepatch_status>();

            if (Cancelling)
                throw new CancelledException();

            var updateRequiresHostReboot = false;

            try
            {
                AddProgressStep(string.Format(Messages.UPDATES_WIZARD_RUNNING_PRECHECK, xenServerPatch.Name, host.Name()));

                RefreshUpdate(host, mapping, session);

                List<Problem> problems = null;

                if (mapping is PoolPatchMapping patchMapping)
                {
                    log.InfoFormat("Running patch precheck on '{0}'. Patch = '{1}' (uuid = '{2}'; opaque_ref = '{3}')", host.Name(), patchMapping.Pool_patch.Name(), patchMapping.Pool_patch.uuid, patchMapping.Pool_patch.opaque_ref);
                    problems = new PatchPrecheckCheck(host, patchMapping.Pool_patch, livePatchStatus).RunAllChecks();
                    updateRequiresHostReboot = WizardHelpers.IsHostRebootRequiredForUpdate(host, patchMapping.Pool_patch, livePatchStatus);
                }
                else if (mapping is PoolUpdateMapping updateMapping)
                {
                    log.InfoFormat("Running update precheck on '{0}'. Update = '{1}' (uuid = '{2}'; opaque_ref = '{3}'", host.Name(), updateMapping.Pool_update.Name(), updateMapping.Pool_update.uuid, updateMapping.Pool_update.opaque_ref);
                    problems = new PatchPrecheckCheck(host, updateMapping.Pool_update, livePatchStatus).RunAllChecks();
                    updateRequiresHostReboot = WizardHelpers.IsHostRebootRequiredForUpdate(host, updateMapping.Pool_update, livePatchStatus);
                }

                Problem problem = null;

                if (problems != null && problems.Count > 0)
                    problem = problems[0];

                if (problem != null)
                    throw new Exception(problem.Description);
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Precheck failed on host {0}", host.Name()), ex);
                throw;
            }

            if(updateRequiresHostReboot && !hostsThatWillRequireReboot.Contains(host.uuid))
                hostsThatWillRequireReboot.Add(host.uuid);
            if (updateRequiresHostReboot && !mapping.HostsThatNeedEvacuated.Contains(host.uuid))
                mapping.HostsThatNeedEvacuated.Add(host.uuid);
            if (livePatchStatus.ContainsKey(host.uuid) && livePatchStatus[host.uuid] == livepatch_status.ok_livepatch_complete)
            {
                if (!livePatchAttempts.ContainsKey(host.uuid) || livePatchAttempts[host.uuid] == null)
                    livePatchAttempts[host.uuid] = new List<string>();
                livePatchAttempts[host.uuid].Add(xenServerPatch.Uuid);
            }
        }

        public static void RefreshUpdate(Host host, HostUpdateMapping mapping, Session session)
        {
            // re-introduce pool_update if needed
            if (mapping is PoolUpdateMapping poolUpdateMapping 
                && session.Connection.Cache.Pool_updates.FirstOrDefault(u => string.Equals(u.uuid, poolUpdateMapping.Pool_update.uuid, StringComparison.OrdinalIgnoreCase)) == null)
            {
                log.InfoFormat("Re-introduce update on '{0}'. Update = '{1}' (uuid = '{2}'; old opaque_ref = '{3}')",
                    host.Name(), poolUpdateMapping.Pool_update.Name(), poolUpdateMapping.Pool_update.uuid,
                    poolUpdateMapping.Pool_update.opaque_ref);
                try
                {
                    var newUpdateRef = Pool_update.introduce(session, poolUpdateMapping.Pool_update.vdi.opaque_ref);
                    session.Connection.WaitForCache(newUpdateRef);
                }
                catch (Exception e)
                {
                    if (e is Failure failure && failure.ErrorDescription != null && failure.ErrorDescription.Count > 1 && failure.ErrorDescription[0] == Failure.UPDATE_ALREADY_EXISTS)
                    {
                        log.InfoFormat("Update '{0}' already exists", poolUpdateMapping.Pool_update.Name());
                    }
                    else
                    {
                        log.Error("Failed to re-introduce the update", e);
                        throw;
                    }
                }
            }

            // refresh the update/patch record based on uuid
            mapping.RefreshUpdate();
        }
    }
}
