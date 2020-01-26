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
using XenAdmin.Wizards.PatchingWizard;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAdmin.Wizards.RollingUpgradeWizard.PlanActions;
using XenAPI;


namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    public partial class RollingUpgradeUpgradePage : AutomatedUpdatesBasePage
    {
        public RollingUpgradeUpgradePage()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides
        public override string PageTitle { get { return Messages.UPGRADE_PLAN; } }

        public override string Text { get { return Messages.UPGRADE_PLAN; } }

        public override string HelpID { get { return "Upgradepools"; } }
        #endregion

        #region Accessors
        public bool ManualModeSelected { private get; set; }
        public Dictionary<string, string> InstallMethodConfig { private get; set; }
        public bool ApplySuppPackAfterUpgrade { private get; set; }
        public string SelectedSuppPackPath { private get; set; }
        #endregion

        #region Fields
        private Dictionary<UpdateProgressBackgroundWorker, List<XenServerPatch>> AllUploadedPatches = new Dictionary<UpdateProgressBackgroundWorker, List<XenServerPatch>>();
        private Dictionary<UpdateProgressBackgroundWorker, List<XenServerPatch>> MinimalPatches = new Dictionary<UpdateProgressBackgroundWorker, List<XenServerPatch>>(); // should be calculated only once per pool (to ensure update homogeneity)
        private Dictionary<Host, Pool_update> UploadedSuppPacks = new Dictionary<Host, Pool_update>();
        #endregion

        #region AutomatedUpdatesBesePage overrides

        protected override string BlurbText()
        {
            return Messages.ROLLING_UPGRADE_UPGRADE_IN_PROGRESS;
        }

        protected override string SuccessMessageOnCompletion(bool multiplePools)
        {
            return multiplePools ? Messages.ROLLING_UPGRADE_SUCCESS_MANY : Messages.ROLLING_UPGRADE_SUCCESS_ONE;
        }

        protected override string FailureMessageOnCompletion(bool multiplePools)
        {
            return multiplePools ? Messages.ROLLING_UPGRADE_ERROR_MANY : Messages.ROLLING_UPGRADE_ERROR_ONE;
        }

        protected override string WarningMessageOnCompletion(bool multiplePools)
        {
            return multiplePools ? Messages.ROLLING_UPGRADE_WARNING_MANY : Messages.ROLLING_UPGRADE_WARNING_ONE;
        }

        protected override string SuccessMessagePerPool(Pool pool)
        {
            return Messages.ROLLING_UPGRADE_SUCCESS_ONE;
        }

        protected override string FailureMessagePerPool(bool multipleErrors)
        {
            return multipleErrors ? Messages.ROLLING_UPGRADE_ERROR_POOL_MANY : Messages.ROLLING_UPGRADE_ERROR_POOL_ONE;
        }

        protected override string WarningMessagePerPool(Pool pool)
        {
            return LivePatchWarningMessagePerPool(pool);
        }

        protected override string UserCancellationMessage()
        {
            return Messages.ROLLING_UPGRADE_CANCELLATION;
        }

        protected override string ReconsiderCancellationMessage()
        {
            return Messages.ROLLING_UPGRADE_CANCELLATION_RECONSIDER;
        }

        protected override List<HostPlan> GenerateHostPlans(Pool pool, out List<Host> applicableHosts)
        {
            //Add masters first, then the slaves (add all hosts for now, they will be skipped from upgrade if already upgraded)
            applicableHosts = pool.Connection.Cache.Hosts.ToList();
            applicableHosts.Sort();
            return applicableHosts.Select(GetSubTasksFor).ToList();
        }

        protected override bool SkipInitialPlanActions(Host host)
        {
            //Skip hosts already upgraded
            if (host.IsMaster())
            {
                var pool = Helpers.GetPoolOfOne(host.Connection);
                if (pool != null && pool.IsMasterUpgraded())
                {
                    log.Debug(string.Format("Skipping master '{0}' because it is upgraded", host.Name()));
                    return true;
                }
            }
            else
            {
                var master = Helpers.GetMaster(host.Connection);
                if (master != null && host.LongProductVersion() == master.LongProductVersion())
                {
                    log.Debug(string.Format("Skipping host '{0}' because it is upgraded", host.Name()));
                    return true;
                }
            }
            return false;
        }

        protected override void DoAfterInitialPlanActions(UpdateProgressBackgroundWorker bgw, Host host, List<Host> hosts)
        {
            if (!ApplyUpdatesToNewVersion && !ApplySuppPackAfterUpgrade)
                return;

            var theHostPlan = bgw.HostPlans.FirstOrDefault(ha => ha.Host.Equals(host));
            if (theHostPlan == null)
                return;

            if (theHostPlan.UpdatesPlanActions.Count > 0) // this is a retry; do not recreate actions
                return;

            host = host.Connection.TryResolveWithTimeout(new XenRef<Host>(host.opaque_ref));

            if (ApplyUpdatesToNewVersion)
            {
                var automatedUpdatesRestricted = host.Connection.Cache.Hosts.Any(h => Helpers.DundeeOrGreater(h) && Host.RestrictBatchHotfixApply(h)); //if any host is not licensed for automated updates (only considering DundeeOrGreater hosts)

                if (!automatedUpdatesRestricted)
                {
                    if (!MinimalPatches.ContainsKey(bgw))
                    {
                        log.InfoFormat("Calculating minimal patches for {0}", host.Name());
                        Updates.CheckForUpdatesSync(null);
                        var mp = Updates.GetMinimalPatches(host);
                        log.InfoFormat("Minimal patches for {0}: {1}", host.Name(),
                            mp == null ? "None" : string.Join(",", mp.Select(p => p.Name)));

                        MinimalPatches.Add(bgw, mp);
                    }

                    var minimalPatches = MinimalPatches[bgw];

                    if (minimalPatches != null)
                    {
                        if (!AllUploadedPatches.ContainsKey(bgw))
                            AllUploadedPatches.Add(bgw, new List<XenServerPatch>());
                        var uploadedPatches = AllUploadedPatches[bgw];

                        var hp = GetUpdatePlanActionsForHost(host, hosts, minimalPatches, uploadedPatches,
                            new KeyValuePair<XenServerPatch, string>(), false);
                        if (hp.UpdatesPlanActions != null && hp.UpdatesPlanActions.Count > 0)
                        {
                            theHostPlan.UpdatesPlanActions.AddRange(hp.UpdatesPlanActions);
                            theHostPlan.DelayedPlanActions.InsertRange(0, hp.DelayedPlanActions);
                        }
                    }
                }
                else
                {
                    log.InfoFormat("Skipping updates installation on {0} because the batch hotfix application is restricted in the pool", host.Name());
                }
            }

            if (ApplySuppPackAfterUpgrade && Helpers.ElyOrGreater(host))
            {
                var suppPackPlanAction = new RpuUploadAndApplySuppPackPlanAction(host.Connection,
                    host, hosts, SelectedSuppPackPath, UploadedSuppPacks, hostsThatWillRequireReboot);
                theHostPlan.UpdatesPlanActions.Add(suppPackPlanAction);
            }
        }
        #endregion

        #region Private methods

        private HostPlan GetSubTasksFor(Host host)
        {
            var runningVMs = host.GetRunningVMs();

            UpgradeHostPlanAction upgradeAction;
            if (ManualModeSelected)
                upgradeAction = new UpgradeManualHostPlanAction(host, this);
            else
                upgradeAction = new UpgradeAutomatedHostPlanAction(host, this, InstallMethodConfig);

            var initialPlanActions = new List<PlanAction>()
            {
                new EvacuateHostPlanAction(host),
                upgradeAction,
                new BringBabiesBackAction(runningVMs, host, true)
            };

            return new HostPlan(host, initialPlanActions, null, new List<PlanAction>());
        }

        #endregion
    }
}
