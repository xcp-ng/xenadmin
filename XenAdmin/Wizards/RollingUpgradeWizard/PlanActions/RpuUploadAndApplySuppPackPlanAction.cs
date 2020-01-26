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
using System.IO;
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAdmin.Network;
using XenAdmin.Wizards.PatchingWizard;
using XenAdmin.Wizards.PatchingWizard.PlanActions;


namespace XenAdmin.Wizards.RollingUpgradeWizard.PlanActions
{
    class RpuUploadAndApplySuppPackPlanAction : HostPlanAction
    {
        private Host host;
        private List<Host> hosts;
        private string suppPackPath;
        private Dictionary<Host, Pool_update> uploadedSuppPacks;
        public List<PlanAction> DelayedPlanActions;
        private readonly List<string> hostsThatWillRequireReboot;

        public RpuUploadAndApplySuppPackPlanAction(IXenConnection connection, Host host, List<Host> hosts, string path, Dictionary<Host, Pool_update> uploadedUpdate, List<string> hostsThatWillRequireReboot)
            : base(host)
        {
            this.host = host;
            this.hosts = hosts;
            suppPackPath = path;
            uploadedSuppPacks = uploadedUpdate;
            DelayedPlanActions = new List<PlanAction>();
            this.hostsThatWillRequireReboot = hostsThatWillRequireReboot;
        }

        public override bool IsSkippable
        {
            get { return true; }
        }

        public override string Title
        {
            get
            {
                return string.Format(Messages.RPU_WIZARD_INSTALL_SUPPPACK_TITLE, Path.GetFileName(suppPackPath), host.Name());
            }
        }

        protected override void DoOnSkip()
        {
            AddProgressStep(string.Format(Messages.RPU_WIZARD_SKIP_INSTALL_SUPPPACK, Path.GetFileName(suppPackPath), host.Name()));
        }

        protected override void RunWithSession(ref Session session)
        {
            var conn = session.Connection;
            var master = Helpers.GetMaster(conn);
            var suppPackName = Path.GetFileName(suppPackPath);

            host = GetResolvedHost();

            // upload
            UploadSuppPack(master, conn, session, suppPackName);

            if (uploadedSuppPacks.ContainsKey(master))
            {
                var update = uploadedSuppPacks[master];
                if (update != null)
                {
                    // precheck
                    PrecheckSuppPack(update, suppPackName, out var alreadyApplied, out var updateRequiresHostEvacuation);
                    if (alreadyApplied)
                    {
                        // do after-apply-supppack step in case that this is a retry after
                        // the update already applied but after-apply-supppack hasn't done yet
                        AfterApplySuppPack(update);
                        RemoveSuppPackFromMaster(session, master, suppPackName, update);
                        return;
                    }

                    // apply
                    ApplySuppPack(conn, session, suppPackName, update, updateRequiresHostEvacuation);

                    // after apply guidance
                    AfterApplySuppPack(update);

                    // remove from master
                    RemoveSuppPackFromMaster(session, master, suppPackName, update);
                }
            }
        }

        private void UploadSuppPack(Host master, IXenConnection connection, Session session, string suppPack)
        {
            if (!uploadedSuppPacks.ContainsKey(master))
            {
                UploadSupplementalPackAction uploadIsoAction;
                try
                {
                    AddProgressStep(string.Format(Messages.UPDATES_WIZARD_UPLOADING_UPDATE, suppPack, connection.Name));

                    uploadIsoAction = new UploadSupplementalPackAction(connection, new List<Host> { master }, suppPackPath, true);
                    uploadIsoAction.Changed += uploadAction_Changed;
                    uploadIsoAction.Completed += uploadAction_Completed;
                    uploadIsoAction.RunExternal(session);
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Upload failed for update {0} on connection {1}", suppPack, connection), ex);
                    throw;
                }

                var poolupdate = uploadIsoAction.PoolUpdate;
                if (poolupdate == null)
                {
                    log.ErrorFormat(
                        "Upload finished successfully, but Pool_update object has not been found for update {0} on connection {1}.",
                        suppPack, connection);
                    throw new Exception(Messages.ACTION_UPLOADPATCHTOMASTERPLANACTION_FAILED);
                }

                uploadedSuppPacks.Add(master, poolupdate);
            }
            else if (host.uuid == master.uuid)
            {
                AddProgressStep(string.Format(Messages.UPDATES_WIZARD_SKIPPING_UPLOAD, suppPack, connection.Name));
            }
        }

        private void PrecheckSuppPack(Pool_update update, string suppPack, out bool alreadyApplied, out bool updateRequiresHostEvacuation)
        {
            alreadyApplied = false;
            if (Cancelling)
                throw new CancelledException();

            var livePatchStatus = new Dictionary<string, livepatch_status>();

            try
            {
                AddProgressStep(string.Format(Messages.UPDATES_WIZARD_RUNNING_PRECHECK, suppPack, host.Name()));


                PatchPrecheckCheck check = new PatchPrecheckCheck(host, update, livePatchStatus);
                var problems = check.RunAllChecks();
                updateRequiresHostEvacuation = WizardHelpers.IsHostRebootRequiredForUpdate(host, update, livePatchStatus);
                if (problems != null && problems.Count > 0)
                {
                    if (problems[0] is PatchAlreadyApplied)
                    {
                        log.InfoFormat("The update {0} is already applied on {1}. Ignore it.", suppPack, host.Name());
                        ReplaceProgressStep(string.Format(Messages.UPDATES_WIZARD_SKIPPING_UPDATE, suppPack, host.Name()));
                        alreadyApplied = true;
                    }
                    else
                        throw new Exception(problems[0].Description);
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Precheck failed on host {0}", host.Name()), ex);
                throw;
            }

            if (livePatchStatus.ContainsKey(host.uuid)
                && livePatchStatus[host.uuid] != livepatch_status.ok_livepatch_complete
                && !hostsThatWillRequireReboot.Contains(host.uuid))
                hostsThatWillRequireReboot.Add(host.uuid);
        }

        private void ApplySuppPack(IXenConnection connection, Session session, string suppPack, Pool_update update, bool updateRequiresHostEvacuation)
        {
            try
            {
                // evacuate the host, if needed, before applying the update
                if (updateRequiresHostEvacuation)
                    EvacuateHost(ref session);

                AddProgressStep(string.Format(Messages.UPDATES_WIZARD_APPLYING_UPDATE, suppPack, host.Name()));

                var task = Pool_update.async_apply(session, update.opaque_ref, host.opaque_ref);
                PollTaskForResultAndDestroy(connection, ref session, task);
            }
            catch (Failure f)
            {
                if (f.ErrorDescription.Count > 1 && f.ErrorDescription[0] == Failure.UPDATE_ALREADY_APPLIED)
                {
                    log.InfoFormat("The update {0} is already applied on {1}. Ignoring this error.", suppPack, host.Name());
                    ReplaceProgressStep(string.Format(Messages.UPDATES_WIZARD_SKIPPING_UPDATE, suppPack, host.Name()));
                }
                else
                    throw;
            }
        }

        private void AfterApplySuppPack(Pool_update update)
        {
            var afterApplyGuidanceLists = update.after_apply_guidance;
            if (afterApplyGuidanceLists != null && afterApplyGuidanceLists.Count > 0)
            {
                foreach (update_after_apply_guidance afterApplyGuidance in afterApplyGuidanceLists)
                {
                    var planAction = GetAfterApplyGuidancePlanAction(host, afterApplyGuidance);
                    DelayedPlanActions.Add(planAction);
                }
            }
        }

        private void RemoveSuppPackFromMaster(Session session, Host master, string suppPack, Pool_update update)
        {
            var isLastHostInPool = hosts.IndexOf(host) == hosts.Count - 1;
            if (isLastHostInPool)
            {
                try
                {
                    AddProgressStep(string.Format(Messages.UPDATES_WIZARD_REMOVING_UPDATES_FROM_POOL, suppPack));

                    Pool_update.pool_clean(session, update.opaque_ref);
                    if (!update.AppliedOnHosts().Any())
                        Pool_update.destroy(session, update.opaque_ref);

                    uploadedSuppPacks.Remove(master);
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Remove update file from master failed on host {0}", master.Name()), ex);
                }
            }
        }

        private PlanAction GetAfterApplyGuidancePlanAction(Host host, update_after_apply_guidance guidance)
        {
            switch (guidance)
            {
                case update_after_apply_guidance.restartHost:
                    return new RestartHostPlanAction(host, host.GetRunningVMs(), true, false, hostsThatWillRequireReboot);
                case update_after_apply_guidance.restartXAPI:
                    return new RestartAgentPlanAction(host);
                case update_after_apply_guidance.restartHVM:
                    return new RebootVMsPlanAction(host, host.GetRunningHvmVMs());
                case update_after_apply_guidance.restartPV:
                    return new RebootVMsPlanAction(host, host.GetRunningPvVMs());
                default:
                    return null;
            }
        }

        private void uploadAction_Changed(ActionBase action)
        {
            if (action == null)
                return;

            if (Cancelling)
                action.Cancel();

            var bpAction = action as IByteProgressAction;
            if (bpAction == null)
                return;

            if (!string.IsNullOrEmpty(bpAction.ByteProgressDescription))
                ReplaceProgressStep(bpAction.ByteProgressDescription);
        }

        private void uploadAction_Completed(ActionBase action)
        {
            if (action == null)
                return;

            action.Changed -= uploadAction_Changed;
            action.Completed -= uploadAction_Completed;
        }
    }
}
