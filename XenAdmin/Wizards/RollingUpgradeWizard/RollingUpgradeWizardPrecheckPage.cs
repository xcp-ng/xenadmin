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
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Diagnostics.Hotfixing;
using XenAdmin.Wizards.PatchingWizard;
using XenAPI;
using System.Linq;
using XenAdmin.Network;
using CheckGroup = System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.List<XenAdmin.Diagnostics.Checks.Check>>;


namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    public partial class RollingUpgradeWizardPrecheckPage : PatchingWizard_PrecheckPage
    {
        public RollingUpgradeWizardPrecheckPage()
        {
            InitializeComponent();
            ManualUpgrade = true;
        }

        private void AddEventHandlersToMasters()
        {
            foreach (Host master in SelectedMasters)
            {
                master.Connection.ConnectionStateChanged += connection_ConnectionChanged;
                master.Connection.CachePopulated += connection_CachePopulated;
            }
        }

        private void RemoveEventHandlersToMasters()
        {
            foreach (Host master in SelectedMasters)
            {
                master.Connection.ConnectionStateChanged -= connection_ConnectionChanged;
                master.Connection.CachePopulated -= connection_CachePopulated;
            }
        }

        private void connection_ConnectionChanged(IXenConnection conn)
        {
            Program.Invoke(this, RefreshRechecks);
        }

        private void connection_CachePopulated(IXenConnection conn)
        {
            Program.Invoke(this, RefreshRechecks);
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction == PageLoadedDirection.Back)
            {
                RefreshRechecks();
                return;
            }
            var selectedMasters = new List<Host>(SelectedMasters);
            RemoveEventHandlersToMasters();
            SelectedServers.Clear();
            foreach (Host selectedMaster in selectedMasters)
            {
                Host master = selectedMaster;
                if (master != null)
                {
                    Pool pool = Helpers.GetPoolOfOne(master.Connection);
                    if (pool != null)
                        SelectedServers.AddRange(pool.HostsToUpgrade());
                    else
                        SelectedServers.Add(master);
                }
            }
            AddEventHandlersToMasters();
            labelPrechecksFirstLine.Text = Messages.ROLLINGUPGRADE_PRECHECKS;
            RefreshRechecks();
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            RemoveEventHandlersToMasters();
        }

        public override void PageCancelled(ref bool cancel)
        {
            base.PageCancelled(ref cancel);
            if (cancel)
                return;
            RemoveEventHandlersToMasters();
        }

        public override string PageTitle
        {
            get
            {
                return Messages.UPGRADE_PRECHECKS_TITLE;
            }
        }

        public override string Text
        {
            get
            {
                return Messages.UPGRADE_PRECHECKS_TEXT;
            }
        }

        public override string HelpID
        {
            get { return "Upgradeprechecks"; }
        }

        public override string NextText(bool isLastPage)
        {
            return Messages.START_UPGRADE;
        }

        protected override List<CheckGroup> GenerateChecks(Pool_patch patch)
        {
            var groups = new List<CheckGroup>();

            List<Host> hostsToUpgrade = new List<Host>();
            List<Host> hostsToUpgradeOrUpdate = new List<Host>();
            foreach (var pool in SelectedPools)
            {
                var poolHostsToUpgrade = pool.HostsToUpgrade();
                hostsToUpgrade.AddRange(poolHostsToUpgrade);
                hostsToUpgradeOrUpdate.AddRange(ApplyUpdatesToNewVersion
                    ? HostsToUpgradeOrUpdate(pool)
                    : poolHostsToUpgrade);
            }

            //XenCenter version check (if any of the selected server version is not the latest)
            var latestCrVersion = Updates.XenServerVersions.FindAll(item => item.LatestCr).OrderByDescending(v => v.Version).FirstOrDefault();
            if (latestCrVersion != null &&
                hostsToUpgradeOrUpdate.Any(host => new Version(Helpers.HostProductVersion(host)) < latestCrVersion.Version))
            {
                groups.Add(new CheckGroup(Messages.CHECKING_XENCENTER_VERSION, new List<Check> {new XenCenterVersionCheck(null)}));
            }

            //HostMaintenanceModeCheck checks - for hosts that will be upgraded or updated
            var livenessChecks = new List<Check>();
            foreach (Host host in hostsToUpgradeOrUpdate)
                livenessChecks.Add(new HostLivenessCheck(host, hostsToUpgrade.Contains(host)));
            groups.Add(new CheckGroup(Messages.CHECKING_HOST_LIVENESS_STATUS, livenessChecks));

            //HotfixesCheck - for hosts that will be upgraded
            var hotfixChecks = new List<Check>();
            foreach (var host in hostsToUpgrade)
            {
                if (new HotfixFactory().IsHotfixRequired(host) && !ManualUpgrade)
                    hotfixChecks.Add(new HostHasHotfixCheck(host));
            }
            if (hotfixChecks.Count > 0)
                groups.Add(new CheckGroup(Messages.CHECKING_UPGRADE_HOTFIX_STATUS, hotfixChecks));

            //SafeToUpgrade- and PrepareToUpgrade- checks - in automatic mode only, for hosts that will be upgraded
            if (!ManualUpgrade)
            {
                var prepareToUpgradeChecks = new List<Check>();
                foreach (var host in hostsToUpgrade)
                    prepareToUpgradeChecks.Add(new PrepareToUpgradeCheck(host, InstallMethodConfig));
                groups.Add(new CheckGroup(Messages.CHECKING_PREPARE_TO_UPGRADE, prepareToUpgradeChecks));

                var safeToUpgradeChecks = new List<Check>();
                foreach (var host in hostsToUpgrade)
                    safeToUpgradeChecks.Add(new SafeToUpgradeCheck(host));
                groups.Add(new CheckGroup(Messages.CHECKING_SAFE_TO_UPGRADE, safeToUpgradeChecks));
            }

            //HA checks - for each pool
            var haChecks = (from Host server in SelectedMasters
                select new HAOffCheck(server) as Check).ToList();
            groups.Add(new CheckGroup(Messages.CHECKING_HA_STATUS, haChecks));

            //Checking can evacuate host - for hosts that will be upgraded or updated
            var evacuateChecks = new List<Check>();
            foreach (Host host in hostsToUpgradeOrUpdate)
                evacuateChecks.Add(new AssertCanEvacuateUpgradeCheck(host));
            groups.Add(new CheckGroup(Messages.CHECKING_CANEVACUATE_STATUS, evacuateChecks));

            //PBDsPluggedCheck -  for hosts that will be upgraded or updated
            var pbdChecks = new List<Check>();
            foreach (Host host in hostsToUpgradeOrUpdate)
                pbdChecks.Add(new PBDsPluggedCheck(host));
            groups.Add(new CheckGroup(Messages.CHECKING_STORAGE_CONNECTIONS_STATUS, pbdChecks));

            //HostMemoryPostUpgradeCheck - for hosts that will be upgraded
            var mostMemoryPostUpgradeChecks = new List<Check>();
            foreach (var host in hostsToUpgrade)
            {
                mostMemoryPostUpgradeChecks.Add(new HostMemoryPostUpgradeCheck(host, InstallMethodConfig));
            }
            if (mostMemoryPostUpgradeChecks.Count > 0)
                groups.Add(new CheckGroup(Messages.CHECKING_HOST_MEMORY_POST_UPGRADE, mostMemoryPostUpgradeChecks));
          
            var gfs2Checks = new List<Check>();
            foreach (Pool pool in SelectedPools.Where(p =>
                Helpers.KolkataOrGreater(p.Connection) && !Helpers.LimaOrGreater(p.Connection)))
            {
                Host host = pool.Connection.Resolve(pool.master);
                gfs2Checks.Add(new PoolHasGFS2SR(host));
            }

            if (gfs2Checks.Count > 0)
            {
                groups.Add(new CheckGroup(Messages.CHECKING_CLUSTERING_STATUS, gfs2Checks));
            }

            //Checking PV guests - for hosts that have any PV guests and warn the user before the upgrade.
            var pvChecks = new List<Check>();
            foreach (Pool pool in SelectedPools.Where(p => !Helpers.QuebecOrGreater(p.Connection)))
            {
                if (pool.Connection.Resolve(pool.master) != null)
                    pvChecks.Add(new PVGuestsCheck(pool, true, InstallMethodConfig, ManualUpgrade)); 
            }
            if (pvChecks.Count > 0)
                groups.Add(new CheckGroup(Messages.CHECKING_PV_GUESTS, pvChecks));

            //Checking automated updates are possible if apply updates checkbox is ticked
            if (ApplyUpdatesToNewVersion)
            {
                var automatedUpdateChecks = (from Host server in SelectedMasters
                    select new AutomatedUpdatesLicenseCheck(server) as Check).ToList();

                automatedUpdateChecks.Add(new CfuAvailabilityCheck());

                groups.Add(new CheckGroup(Messages.CHECKING_AUTOMATED_UPDATES_POSSIBLE,
                    automatedUpdateChecks));
            }

            return groups;
        }

        public IEnumerable<Host> SelectedMasters { private get; set; }
        public bool ManualUpgrade { set; private get; }

        public Dictionary<string, string> InstallMethodConfig { private get; set; }

        #region private methods
        public static List<Host> HostsToUpgradeOrUpdate(Pool pool)
        {
            var result = new List<Host>();

            if (pool == null)
                return result;

            var master = Helpers.GetMaster(pool);
            if (master == null)
                return result;

            if (pool.IsMasterUpgraded())
            {
                foreach (var h in pool.Connection.Cache.Hosts)
                {
                    if (h.LongProductVersion() != master.LongProductVersion()) // host needs to be upgraded
                        result.Add(h); // host 
                    else
                    {
                        //check update sequence for already-upgraded hosts
                        var us = Updates.GetPatchSequenceForHost(h, Updates.GetMinimalPatches(master));
                        if (us != null && us.Count > 0)
                        {
                            result.Add(h);
                        }
                    }
                }
            }
            else
                result.AddRange(pool.Connection.Cache.Hosts);

            result.Sort();

            return result;
        }
        #endregion

    }
}
