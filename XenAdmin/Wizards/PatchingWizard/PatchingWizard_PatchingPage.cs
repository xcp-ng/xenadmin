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
using System.Text;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_PatchingPage : AutomatedUpdatesBasePage
    {
        public Dictionary<string, livepatch_status> LivePatchCodesByHost
        {
            get;
            set;
        }

        public PatchingWizard_PatchingPage()
        {
            InitializeComponent();
        }

        #region Accessors
        public List<Host> SelectedServers { private get; set; }
        public UpdateType SelectedUpdateType { private get; set; }
        public Pool_patch Patch { private get; set; }
        public Pool_update PoolUpdate { private get; set; }

        public Dictionary<Pool, StringBuilder> ManualTextInstructions { private get; set; }
        public bool IsAutomaticMode { private get; set; }
        public bool RemoveUpdateFile { private get; set; }
        public string SelectedPatchFilePatch { private get; set; }
        public Dictionary<Host, VDI> SuppPackVdis { private get; set; }
        #endregion

        #region XenTabPage overrides

        public override string Text
        {
            get
            {
                return Messages.PATCHINGWIZARD_PATCHINGPAGE_TEXT;
            }
        }

        public override string PageTitle
        {
            get
            {
                return Messages.PATCHINGWIZARD_PATCHINGPAGE_TITLE;
            }
        }

        public override string HelpID
        {
            get { return "InstallUpdate"; }
        }

        #endregion

        #region AutomatedUpdatesBesePage overrides

        protected override List<HostPlan> GenerateHostPlans(Pool pool, out List<Host> applicableHosts)
        {
            return IsAutomaticMode
                ? CompileAutomaticHostPlan(pool, out applicableHosts)
                : CompileManualHostPlan(pool, out applicableHosts);
        }

        protected override string BlurbText()
        {
            return string.Format(Messages.PATCHINGWIZARD_SINGLEUPDATE_TITLE, GetUpdateName());
        }

        protected override string SuccessMessageOnCompletion(bool multiplePools)
        {
            var msg = multiplePools
                ? Messages.PATCHINGWIZARD_SINGLEUPDATE_SUCCESS_MANY
                : Messages.PATCHINGWIZARD_SINGLEUPDATE_SUCCESS_ONE;
            return string.Format(msg, GetUpdateName());
        }

        protected override string WarningMessageOnCompletion(bool multiplePools)
        {
            var msg = multiplePools
                ? Messages.PATCHINGWIZARD_SINGLEUPDATE_WARNING_MANY
                : Messages.PATCHINGWIZARD_SINGLEUPDATE_WARNING_ONE;
            return string.Format(msg, GetUpdateName());
        }

        protected override string SuccessMessagePerPool(Pool pool)
        {
            var sb = new StringBuilder();

            sb.AppendLine(string.Format(Messages.PATCHINGWIZARD_SINGLEUPDATE_SUCCESS_ONE, GetUpdateName())).AppendLine();

            if (!IsAutomaticMode && ManualTextInstructions != null && ManualTextInstructions.ContainsKey(pool))
            {
                sb.AppendLine(Messages.PATCHINGWIZARD_SINGLEUPDATE_MANUAL_POST_UPDATE);
                sb.Append(ManualTextInstructions[pool]).AppendLine();
            }
            
            return sb.ToString();
        }
        
        protected override string WarningMessagePerPool(Pool pool)
        {
            var sb = new StringBuilder();

            var poolHosts = pool.Connection.Cache.Hosts.ToList();

            var livePatchingFailedHosts = new List<Host>();
            foreach (var host in SelectedServers)
            {
                if (poolHosts.Contains(host) && LivePatchingAttemptedForHost(host) && HostRequiresReboot(host))
                    livePatchingFailedHosts.Add(host);
            }

            if (livePatchingFailedHosts.Count == 1)
            {
                sb.AppendFormat(Messages.LIVE_PATCHING_FAILED_ONE_HOST, livePatchingFailedHosts[0].Name()).AppendLine();
                return sb.ToString();
            }

            if (livePatchingFailedHosts.Count > 1)
            {
                var hostnames = string.Join(", ", livePatchingFailedHosts.Select(h => string.Format("'{0}'", h.Name())));
                sb.AppendFormat(Messages.LIVE_PATCHING_FAILED_MULTI_HOST, hostnames).AppendLine();
                return sb.ToString();
            }
            return null;
        }


        protected override string FailureMessageOnCompletion(bool multiplePools)
        {
            var msg = multiplePools
                ? Messages.PATCHINGWIZARD_SINGLEUPDATE_FAILURE_MANY
                : Messages.PATCHINGWIZARD_SINGLEUPDATE_FAILURE_ONE;
            return string.Format(msg, GetUpdateName());
        }

        protected override string FailureMessagePerPool(bool multipleErrors)
        {
            var msg = multipleErrors
                ? Messages.PATCHINGWIZARD_SINGLEUPDATE_FAILURE_PER_POOL_MANY
                : Messages.PATCHINGWIZARD_SINGLEUPDATE_FAILURE_PER_POOL_ONE;
            return string.Format(msg, GetUpdateName());
        }
        protected override string UserCancellationMessage()
        {
            return Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_CANCELLATION;
        }

        protected override string ReconsiderCancellationMessage()
        {
            return Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_CANCELLATION_RECONSIDER;
        }

        #endregion

        #region Manual mode

        private List<HostPlan> CompileManualHostPlan(Pool pool, out List<Host> applicableHosts)
        {
            var poolHosts = pool.Connection.Cache.Hosts.ToList();
            SelectedServers.Sort(); //master first and then the slaves
            var hostplans = new List<HostPlan>();

            if (SelectedUpdateType == UpdateType.ISO)
            {
                if (PoolUpdate != null) //ely or greater
                {
                    foreach (var server in SelectedServers)
                        if (poolHosts.Contains(server))
                        {
                            var hostRebootRequired = WizardHelpers.IsHostRebootRequiredForUpdate(server, PoolUpdate, LivePatchCodesByHost);

                            var updateActions = new List<PlanAction> {new ApplyPoolUpdatePlanAction(server, PoolUpdate, hostRebootRequired) };
                            hostplans.Add(new HostPlan(server, null, updateActions, null));
                        }
                }
                else if (SuppPackVdis != null) //supp pack
                {
                    foreach (var server in SelectedServers)
                        if (SuppPackVdis.ContainsKey(server) && poolHosts.Contains(server))
                        {
                            var updateActions = new List<PlanAction> {new InstallSupplementalPackPlanAction(server, SuppPackVdis[server])};
                            hostplans.Add(new HostPlan(server, null, updateActions, null));
                        }
                }
            }
            else // legacy
            {
                foreach (var server in SelectedServers)
                    if (poolHosts.Contains(server))
                    {
                        var hostRebootRequired = WizardHelpers.IsHostRebootRequiredForUpdate(server, Patch, LivePatchCodesByHost);

                        var updateActions = new List<PlanAction> { new ApplyPatchPlanAction(server, Patch, hostRebootRequired) };
                        hostplans.Add(new HostPlan(server, null, updateActions, null));
                    }

                if (RemoveUpdateFile && hostplans.Count > 0)
                    hostplans[hostplans.Count - 1].UpdatesPlanActions.Add(new RemoveUpdateFile(pool, Patch));
            }

            applicableHosts = hostplans.Select(h => h.Host).ToList();
            return hostplans;
        }

        #endregion

        #region Automatic mode

        private List<HostPlan> CompileAutomaticHostPlan(Pool pool, out List<Host> applicableHosts)
        {
            var poolHosts = pool.Connection.Cache.Hosts.ToList();
            SelectedServers.Sort(); //master first and then the slaves
            var hostplans = new List<HostPlan>();

            if (SelectedUpdateType == UpdateType.ISO)
            {
                var poolUpdates = new List<Pool_update>(pool.Connection.Cache.Pool_updates);
                var poolUpdate = poolUpdates.FirstOrDefault(u => u != null && string.Equals(u.uuid, PoolUpdate.uuid, StringComparison.OrdinalIgnoreCase));

                if (poolUpdate != null) //ely or greater
                {
                    foreach (var server in SelectedServers)
                        if (poolHosts.Contains(server) && !poolUpdate.AppliedOn(server))
                            hostplans.Add(new HostPlan(server, null, CompilePoolUpdateActionList(server, poolUpdate), null));
                }
                else if (SuppPackVdis != null) // supp pack
                {
                    foreach (var server in SelectedServers)
                        if (SuppPackVdis.ContainsKey(server) && poolHosts.Contains(server))
                        {
                            var updateActions = new List<PlanAction>
                            {
                                new InstallSupplementalPackPlanAction(server, SuppPackVdis[server]),
                                new RestartHostPlanAction(server, server.GetRunningVMs())
                            };
                            hostplans.Add(new HostPlan(server, null, updateActions, null));
                        }
                }
            }
            else //legacy
            {
                var poolPatches = new List<Pool_patch>(pool.Connection.Cache.Pool_patches);
                var poolPatch = poolPatches.Find(p => Patch != null && string.Equals(p.uuid, Patch.uuid, StringComparison.OrdinalIgnoreCase));

                if (poolPatch != null)
                {
                    foreach (Host server in SelectedServers)
                        if (poolHosts.Contains(server) && poolPatch.AppliedOn(server) == DateTime.MaxValue)
                            hostplans.Add(new HostPlan(server, null, CompilePatchActionList(server, poolPatch), null));
                }

                if (RemoveUpdateFile && hostplans.Count > 0)
                    hostplans[hostplans.Count - 1].UpdatesPlanActions.Add(new RemoveUpdateFile(pool, poolPatch));
            }

            applicableHosts = hostplans.Select(h => h.Host).ToList();
            return hostplans;
        }

        private List<PlanAction> CompilePatchActionList(Host host, Pool_patch patch)
        {
            var hostRebootRequired = WizardHelpers.IsHostRebootRequiredForUpdate(host, patch, LivePatchCodesByHost);

            var actions = new List<PlanAction> {new ApplyPatchPlanAction(host, patch, hostRebootRequired) };

            if (hostRebootRequired)
                actions.Add(new RestartHostPlanAction(host, host.GetRunningVMs()));

            if (patch.after_apply_guidance.Contains(after_apply_guidance.restartXAPI))
                actions.Add(new RestartAgentPlanAction(host));

            if (patch.after_apply_guidance.Contains(after_apply_guidance.restartHVM))
                actions.Add(new RebootVMsPlanAction(host, host.GetRunningHvmVMs()));

            if (patch.after_apply_guidance.Contains(after_apply_guidance.restartPV))
                actions.Add(new RebootVMsPlanAction(host, host.GetRunningPvVMs()));

            return actions;
        }

        private List<PlanAction> CompilePoolUpdateActionList(Host host, Pool_update poolUpdate)
        {
            var hostRebootRequired = WizardHelpers.IsHostRebootRequiredForUpdate(host, poolUpdate, LivePatchCodesByHost);

            var actions = new List<PlanAction> {new ApplyPoolUpdatePlanAction(host, poolUpdate, hostRebootRequired) };

            if (hostRebootRequired)
                actions.Add(new RestartHostPlanAction(host, host.GetRunningVMs()));

            if (poolUpdate.after_apply_guidance.Contains(update_after_apply_guidance.restartXAPI))
                actions.Add(new RestartAgentPlanAction(host));

            if (poolUpdate.after_apply_guidance.Contains(update_after_apply_guidance.restartHVM))
                actions.Add(new RebootVMsPlanAction(host, host.GetRunningHvmVMs()));

            if (poolUpdate.after_apply_guidance.Contains(update_after_apply_guidance.restartPV))
                actions.Add(new RebootVMsPlanAction(host, host.GetRunningPvVMs()));

            return actions;
        }

        #endregion

        /// <summary>
        /// Live patching is attempted for a host if the LivePatchCodesByHost
        /// contains the LIVEPATCH_COMPLETE value for that host
        /// </summary>
        private bool LivePatchingAttemptedForHost(Host host)
        {
            return LivePatchCodesByHost != null && LivePatchCodesByHost.ContainsKey(host.uuid) &&
                   LivePatchCodesByHost[host.uuid] == livepatch_status.ok_livepatch_complete;

        }

        /// <summary>
        /// Returns true if the host has to be rebooted for this update
        /// </summary>
        private bool HostRequiresReboot(Host host)
        {
            return host.updates_requiring_reboot != null && PoolUpdate != null
                && host.updates_requiring_reboot.Select(uRef => host.Connection.Resolve(uRef)).Any(u => u != null && u.uuid.Equals(PoolUpdate.uuid));
        }

        private string GetUpdateName()
        {
            if (PoolUpdate != null)
                return PoolUpdate.Name();

            if (Patch != null)
                return Patch.Name();

            try
            {
                return new FileInfo(SelectedPatchFilePatch).Name;
            }
            catch (Exception)
            {
                return SelectedPatchFilePatch;
            }
        }
    }
}
