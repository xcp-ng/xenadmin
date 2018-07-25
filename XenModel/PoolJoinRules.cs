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
using XenAdmin.Network;
using XenAPI;
using System.Linq;

namespace XenAdmin.Core
{
    /// <summary>
    /// Encapsulates whether a slave can join a given pool
    /// </summary>
    public static class PoolJoinRules
    {
        // The order of the enum determines the order we sort the hosts in, in the New Pool Wizard
        public enum Reason
        {
            WillBeMaster,
            Allowed,
            Connecting,
            MasterNotConnected,
            MasterConnecting,
            DifferentAdConfig,
            HasRunningVMs,
            HasSharedStorage,
            IsAPool,
            LicenseRestriction,
            NotSameLinuxPack,
            LicensedHostUnlicensedMaster,
            UnlicensedHostLicensedMaster,
            LicenseMismatch,
            MasterPoolMaxNumberHostReached,
            WillExceedPoolMaxSize,
            DifferentServerVersion,
            DifferentHomogeneousUpdatesFromMaster,
            DifferentHomogeneousUpdatesFromPool,
            DifferentCPUs,
            DifferentNetworkBackends,
            MasterHasHA,
            NotPhysicalPif,
            NonCompatibleManagementInterface,
            WrongRoleOnMaster,
            WrongRoleOnSlave,
            WrongNumberOfIpsCluster,
            WrongNumberOfIpsBond,
            NotConnected,
        }

        // The order of if's in CanJoinPool() determines which reason we display if there is more than one.
        // At least as far as WillBeMaster, some callers may rely on the order.
        // Also, some of the private functions in this file rely on previous tests (especially for
        // null-ness and connectedness) having been done first.

        /// <summary>
        /// Whether a server can join a pool (or form a pool with a standalone server)
        /// </summary>
        /// <param name="slaveConnection">The connection of the server that wants to join the pool</param>
        /// <param name="masterConnection">The connection of the existing pool or of the proposed master of a new pool</param>
        /// <param name="allowLicenseUpgrade">Whether we can upgrade a free host to a v6 license of the pool it's joining</param>
        /// <param name="allowCpuLevelling">Whether we can apply CPU levelling to the slave before it joins the pool</param>
        /// <returns>The reason why the server can't join the pool, or Reason.Allowed if it's OK</returns>
        public static Reason CanJoinPool(IXenConnection slaveConnection, IXenConnection masterConnection, bool allowLicenseUpgrade, bool allowCpuLevelling, bool allowSlaveAdConfig, int poolSizeIncrement = 1)
        {
            if (!Helpers.IsConnected(slaveConnection))  // also implies slaveConnection != null
                return Reason.NotConnected;

            Host slaveHost = Helpers.GetMaster(slaveConnection);
            if (Connecting(slaveHost))  // also implies slaveHost != null
                return Reason.Connecting;

            if (LicenseRestriction(slaveHost))
                return Reason.LicenseRestriction;

            if (IsAPool(slaveConnection))
                return Reason.IsAPool;

            if (!Helpers.IsConnected(masterConnection))  // also implies masterConnection != null
                return Reason.MasterNotConnected;

            Host masterHost = Helpers.GetMaster(masterConnection);
            if (Connecting(masterHost))  // also implies masterHost != null
                return Reason.MasterConnecting;

            if (WillBeMaster(slaveConnection, masterConnection))
                return Reason.WillBeMaster;

            if (!RoleOK(masterConnection))
                return Reason.WrongRoleOnMaster;

            if (!CompatibleCPUs(slaveHost, masterHost, allowCpuLevelling))
                return Reason.DifferentCPUs;

            if (DifferentServerVersion(slaveHost, masterHost))
                return Reason.DifferentServerVersion;

            if (DifferentHomogeneousUpdates(slaveHost, masterHost))
                return masterHost.Connection.Cache.Hosts.Length > 1 ? Reason.DifferentHomogeneousUpdatesFromPool : Reason.DifferentHomogeneousUpdatesFromMaster;

            if (FreeHostPaidMaster(slaveHost, masterHost, allowLicenseUpgrade))
                return Reason.UnlicensedHostLicensedMaster;

            if (PaidHostFreeMaster(slaveHost, masterHost))
                return Reason.LicensedHostUnlicensedMaster;

            if (LicenseMismatch(slaveHost, masterHost))
                return Reason.LicenseMismatch;

            if (MasterPoolMaxNumberHostReached(masterConnection))
                return Reason.MasterPoolMaxNumberHostReached;

            if (WillExceedPoolMaxSize(masterConnection, poolSizeIncrement))
                return Reason.WillExceedPoolMaxSize;
            
            if (!SameLinuxPack(slaveHost, masterHost))
                return Reason.NotSameLinuxPack;

            if (!RoleOK(slaveConnection))
                return Reason.WrongRoleOnSlave;

            if (HasSharedStorage(slaveConnection))
                return Reason.HasSharedStorage;

            if (HasRunningVMs(slaveConnection))
                return Reason.HasRunningVMs;

            if (DifferentNetworkBackends(slaveHost, masterHost))
                return Reason.DifferentNetworkBackends;

            if (!CompatibleAdConfig(slaveHost, masterHost, allowSlaveAdConfig))
                return Reason.DifferentAdConfig;

            if (HaEnabled(masterConnection))
                return Reason.MasterHasHA;

            if (Helpers.FeatureForbidden(slaveConnection, Host.RestrictManagementOnVLAN) && HasSlaveAnyNonPhysicalPif(slaveConnection))
                return Reason.NotPhysicalPif;

            if (!Helpers.FeatureForbidden(slaveConnection, Host.RestrictManagementOnVLAN) && !HasCompatibleManagementInterface(slaveConnection))
                return Reason.NonCompatibleManagementInterface;

            bool clusterHostInBond;
            if (!HasIpForClusterNetwork(masterConnection, slaveHost, out clusterHostInBond))
            {
                return clusterHostInBond ? Reason.WrongNumberOfIpsBond : Reason.WrongNumberOfIpsCluster;
            }    

            return Reason.Allowed;
        }

        public static string ReasonMessage(Reason reason)
        {
            switch (reason)
            {
                case Reason.WillBeMaster:
                    return Messages.MASTER;
                case Reason.Allowed:
                    return "";
                case Reason.Connecting:
                    return Messages.CONNECTING;
                case Reason.MasterNotConnected:
                    return Messages.NEWPOOL_MASTER_DISCONNECTED;
                case Reason.MasterConnecting:
                    return Messages.NEWPOOL_MASTER_CONNECTING;
                case Reason.DifferentAdConfig:
                    return Messages.NEWPOOL_DIFFERING_AD_CONFIG;
                case Reason.HasRunningVMs:
                    return Messages.NEWPOOL_HAS_RUNNING_VMS;
                case Reason.HasSharedStorage:
                    return Messages.NEWPOOL_HAS_SHARED_STORAGE;
                case Reason.IsAPool:
                    return Messages.NEWPOOL_IS_A_POOL;
                case Reason.LicenseRestriction:
                    return Messages.NEWPOOL_POOLINGRESTRICTED;
                case Reason.NotSameLinuxPack:
                    return Messages.NEWPOOL_LINUXPACK;
                case Reason.LicensedHostUnlicensedMaster:
                    return Messages.NEWPOOL_LICENSED_HOST_UNLICENSED_MASTER;
                case Reason.UnlicensedHostLicensedMaster:
                    return Messages.NEWPOOL_UNLICENSED_HOST_LICENSED_MASTER;
                case Reason.LicenseMismatch:
                    return Messages.NEWPOOL_LICENSEMISMATCH;
                case Reason.MasterPoolMaxNumberHostReached:
                    return Messages.NEWPOOL_MAX_NUMBER_HOST_REACHED;
                case Reason.WillExceedPoolMaxSize:
                    return Messages.NEWPOOL_WILL_EXCEED_POOL_MAX_SIZE;
                case Reason.DifferentServerVersion:
                    return Messages.NEWPOOL_DIFF_SERVER;
                case Reason.DifferentHomogeneousUpdatesFromMaster:
                    return Messages.NEWPOOL_DIFFERENT_HOMOGENEOUS_UPDATES_FROM_MASTER;
                case Reason.DifferentHomogeneousUpdatesFromPool:
                    return Messages.NEWPOOL_DIFFERENT_HOMOGENEOUS_UPDATES_FROM_POOL;
                case Reason.DifferentCPUs:
                    return Messages.NEWPOOL_DIFF_HARDWARE;
                case Reason.DifferentNetworkBackends:
                    return Messages.NEWPOOL_DIFFERENT_NETWORK_BACKENDS;
                case Reason.NotConnected:
                    return Messages.DISCONNECTED;
                case Reason.MasterHasHA:
                    return Messages.POOL_JOIN_FORBIDDEN_BY_HA;
                case Reason.NotPhysicalPif :
                    return Messages.POOL_JOIN_NOT_PHYSICAL_PIF;
                case Reason.NonCompatibleManagementInterface :
                    return Messages.POOL_JOIN_NON_COMPATIBLE_MANAGEMENT_INTERFACE;
                case Reason.WrongRoleOnMaster:
                    return Messages.NEWPOOL_MASTER_ROLE;
                case Reason.WrongRoleOnSlave:
                    return Messages.NEWPOOL_SLAVE_ROLE;
                case Reason.WrongNumberOfIpsCluster:
                    return Messages.NEWPOOL_IP_COUNT_CLUSTER;
                case Reason.WrongNumberOfIpsBond:
                    return Messages.NEWPOOL_IP_COUNT_BOND;
                default:
                    System.Diagnostics.Trace.Assert(false, "Unknown reason");
                    return "";
            }
        }

        private static bool WillBeMaster(IXenConnection slave, IXenConnection master)
        {
            // Assume we have already tested that the connection has no other reason why it can't be a master
            // (e.g., connected, licensing restrictions, ...)
            return slave == master;
        }

        private static bool HasSharedStorage(IXenConnection connection)
        {
            foreach (SR sr in connection.Cache.SRs)
            {
                if (sr.shared && !sr.IsToolsSR())
                    return true;
            }
            return false;
        }

        private static bool HasRunningVMs(IXenConnection connection)
        {
            foreach (VM vm in connection.Cache.VMs)
            {
                if (vm.is_a_real_vm() && vm.power_state == XenAPI.vm_power_state.Running)
                    return true;
            }
            return false;
        }

        private static bool Connecting(Host host)
        {
            return host == null;
        }

        private static bool IsAPool(IXenConnection connection)
        {
            return Helpers.GetPool(connection) != null;
        }

        private static bool LicenseRestriction(Host host)
        {
            return Host.RestrictPooling(host);
        }

        // If CompatibleCPUs(slave, master, false) is true, the CPUs can be pooled without masking first.
        // If CompatibleCPUs(slave, master, true) is true but CompatibleCPUs(slave, master, false) is false,
        // the CPUs can be pooled but only if they are masked first.
        public static bool CompatibleCPUs(Host slave, Host master, bool allowCpuLevelling)
        {
            if (slave == null || master == null)
                return true;

            Dictionary<string, string> slave_cpu_info = slave.cpu_info;
            Dictionary<string, string> master_cpu_info = master.cpu_info;
            if (!Helper.AreEqual2(slave_cpu_info, null) && !Helper.AreEqual2(master_cpu_info, null))
            {
                // Host.cpu_info is supported
                // From this point on, it is only necessary to have matching vendor and features
                // (after masking the slave, if allowed).
                if (slave_cpu_info["vendor"] != master_cpu_info["vendor"])
                    return false;

                // As of Dundee, feature levelling makes all CPUs from the same vendor compatible
                if (Helpers.DundeeOrGreater(master) || Helpers.DundeeOrGreater(slave))
                    return true;

                if (slave_cpu_info["features"] == master_cpu_info["features"])
                    return true;

                if (allowCpuLevelling)
                {
                    string cpuid_feature_mask = null;
                    Pool pool = Helpers.GetPoolOfOne(master.Connection);
                    if (pool != null && pool.other_config.ContainsKey("cpuid_feature_mask"))
                        cpuid_feature_mask = pool.other_config["cpuid_feature_mask"];

                    return MaskableTo(slave_cpu_info["maskable"], slave_cpu_info["physical_features"], master_cpu_info["features"], cpuid_feature_mask);
                }

                return false;    
            }

            // Host.cpu_info not supported: use the old method which compares vendor, family, model and flags
            foreach (Host_cpu cpu1 in slave.Connection.Cache.Host_cpus)
            {
                foreach (Host_cpu cpu2 in master.Connection.Cache.Host_cpus)
                {
                    if (cpu1.vendor != cpu2.vendor ||
                        cpu1.family != cpu2.family ||
                        cpu1.model != cpu2.model ||
                        cpu1.flags != cpu2.flags)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Can the features represented by "from" be masked to become the features represented by "to"?
        /// </summary>
        /// <param name="mask_type">"no", "base" (can only mask first 64 bits) or "full"</param>
        /// <param name="from">The unmasked features, as a set of 32 hex digits, possibly separated by spaces or dashes</param>
        /// <param name="to">The target features, as a set of 32 hex digits, possibly separated by spaces or dashes</param>
        /// <param name="feature_mask">A mask of features to ignore for the comparison</param>
        public static bool MaskableTo(string mask_type, string from, string to, string feature_mask)
        {
            if (mask_type == "no")
                return false;

            string from2 = from.Replace(" ", "");
            from2 = from2.Replace("-", "");
            string to2 = to.Replace(" ", "");
            to2 = to2.Replace("-", "");
            if (from2.Length != 32 || to2.Length != 32)
                return false;

            string fm2;
            if (feature_mask == null)
                fm2 = "ffffffffffffffffffffffffffffffff";
            else
            {
                fm2 = feature_mask.Replace(" ", "");
                fm2 = fm2.Replace("-", "");
                if (fm2.Length != 32)
                    fm2 = "ffffffffffffffffffffffffffffffff";
            }

            // Compare int64-by-int64. AMD can mask all the bits; some Intel chips can only mask the first half.
            for (int i = 0; i < 2; ++i)
            {
                string fromPart = from2.Substring(16 * i, 16);
                string toPart = to2.Substring(16 * i, 16);
                string fmPart = fm2.Substring(16 * i, 16);

                ulong fromInt = Convert.ToUInt64(fromPart, 16);
                ulong toInt = Convert.ToUInt64(toPart, 16);
                ulong fmInt = Convert.ToUInt64(fmPart, 16);
                fromInt &= fmInt;
                toInt &= fmInt;

                if (i == 1 && mask_type == "base")  // must be an exact match
                {
                    if (fromInt != toInt)
                        return false;
                }
                else  // "from" must be maskable to "to"
                {
                    if ((fromInt & toInt) != toInt)
                        return false;
                }
            }

            return true;
        }

        private static bool DifferentServerVersion(Host slave, Host master)
        {
            if (slave.API_version_major != master.API_version_major ||
                slave.API_version_minor != master.API_version_minor)
                return true;

            if (Helpers.FalconOrGreater(slave) && string.IsNullOrEmpty(slave.GetDatabaseSchema()))
                return true;
            if (Helpers.FalconOrGreater(master) && string.IsNullOrEmpty(master.GetDatabaseSchema()))
                return true;

            if (slave.GetDatabaseSchema() != master.GetDatabaseSchema())
                return true;

            return
                !Helpers.ElyOrGreater(master) && !Helpers.ElyOrGreater(slave) && slave.BuildNumber() != master.BuildNumber() ||
                slave.PlatformVersion() != master.PlatformVersion() ||
                slave.ProductBrand() != master.ProductBrand();
        }

        /// <summary>
        /// Check whether all updates that request homogeneity are in fact homogeneous
        /// between master and slave. This is used in CanJoinPool and prevents the pool from being created
        /// </summary>
        private static bool DifferentHomogeneousUpdates(Host slave, Host master)
        {
            if (slave == null || master == null)
                return false;

            if (!Helpers.ElyOrGreater(slave) || !Helpers.ElyOrGreater(master))
                return false;

            var masterUpdates = master.AppliedUpdates().Where(update => update.EnforceHomogeneity()).Select(update => update.uuid).ToList();
            var slaveUpdates = slave.AppliedUpdates().Where(update => update.EnforceHomogeneity()).Select(update => update.uuid).ToList();

            return masterUpdates.Count != slaveUpdates.Count || !masterUpdates.All(slaveUpdates.Contains);
        }

        private static bool SameLinuxPack(Host slave, Host master)
        {
            return slave.LinuxPackPresent() == master.LinuxPackPresent();
        }

        public static bool CompatibleAdConfig(Host slave, Host master, bool allowSlaveConfig)
        {
            if (slave == null || master == null)
                return false;

            // CA-30223: There is no need to check the config of the auth services are the same, as xapi only relies on these two values being equal
            if (slave.external_auth_type != master.external_auth_type ||
                slave.external_auth_service_name != master.external_auth_service_name)
            {
                // if the slave is AD free and we are allowing the configure then we can solve this
                if (slave.external_auth_type == Auth.AUTH_TYPE_NONE && allowSlaveConfig)
                    return true;

                return false;
            }
            return true;
        }

        public static bool FreeHostPaidMaster(Host slave, Host master, bool allowLicenseUpgrade)
        {
            if (slave == null || master == null)
                return false;
            
            return slave.IsFreeLicense() && !master.IsFreeLicense() && !allowLicenseUpgrade;
        }

        private static bool PaidHostFreeMaster(Host slave, Host master)
        {
            if (slave == null || master == null)
                return false;

            return !slave.IsFreeLicense() && master.IsFreeLicense();
        }

        private static bool LicenseMismatch(Host slave, Host master)
        {
            Host.Edition slaveEdition = Host.GetEdition(slave.edition);
            Host.Edition masterEdition = Host.GetEdition(master.edition);

            return slaveEdition != Host.Edition.Free && masterEdition != Host.Edition.Free && slaveEdition != masterEdition;
        }

        private static bool MasterPoolMaxNumberHostReached(IXenConnection connection)
        {
            return Helpers.FeatureForbidden(connection, Host.RestrictPoolSize) && connection.Cache.HostCount > 2;
        }

        public static bool WillExceedPoolMaxSize(IXenConnection connection, int poolSizeIncrement)
        {
            return Helpers.FeatureForbidden(connection, Host.RestrictPoolSize) && connection.Cache.HostCount + poolSizeIncrement > 3;
        }

        private static bool HaEnabled(IXenConnection connection)
        {
            Pool pool = Helpers.GetPoolOfOne(connection);
            return (pool != null && pool.ha_enabled);
        }

        private static bool RoleOK(IXenConnection connection)
        {
            return Role.CanPerform(new RbacMethodList("pool.join"), connection);
        }

        private static bool DifferentNetworkBackends(Host slave, Host master)
        {
            if (slave.software_version.ContainsKey("network_backend") &&
                master.software_version.ContainsKey("network_backend") &&
                slave.software_version["network_backend"] != master.software_version["network_backend"])
                return true;

            var masterPool = Helpers.GetPoolOfOne(master.Connection);
            var slavePool = Helpers.GetPoolOfOne(slave.Connection);

            return masterPool != null && slavePool != null &&
                   masterPool.vSwitchController() && slavePool.vSwitchController() &&
                   masterPool.vswitch_controller != slavePool.vswitch_controller;
        }

        /// <summary>
        /// Check whether all supp packs that request homogeneity are in fact homogeneous
        /// across the proposed pool. This is only a warning; it does not prevent the pool being created;
        /// so it is not used by CanJoinPool.
        /// </summary>
        /// <returns>A list of strings: each string is the name of a bad supp pack with a reason in brackets</returns>
        public static List<string> HomogeneousSuppPacksDiffering(List<Host> slaves, IXenObject poolOrMaster)
        {
            List<Host> allHosts = new List<Host>(slaves);
            allHosts.AddRange(poolOrMaster.Connection.Cache.Hosts);

            // Make a dictionary of all supp packs that should be homogeneous
            Dictionary<string, string> homogeneousPacks = new Dictionary<string, string>();
            foreach (Host host in allHosts)
            {
                var suppPacks = host.SuppPacks();
                foreach (Host.SuppPack suppPack in suppPacks)
                {
                    if (suppPack.Homogeneous)
                        homogeneousPacks[suppPack.OriginatorAndName] = suppPack.Description;
                }
            }

            // Now for each such supp pack, see whether it is in fact homogeneous.
            // If not, add it to the list of bad packs with a reason.

            List<string> badPacks = new List<string>();

            foreach (string pack in homogeneousPacks.Keys)
            {
                // Is it present on all hosts, with the same version?
                List<Host> missingHosts = new List<Host>();
                string expectedVersion = null;
                bool versionsDiffer = false;

                foreach (Host host in allHosts)
                {
                    Host.SuppPack matchingPack = host.SuppPacks().Find(sp => (sp.OriginatorAndName == pack));
                    if (matchingPack == null)
                        missingHosts.Add(host);
                    else if (expectedVersion == null)
                        expectedVersion = matchingPack.Version;
                    else if (matchingPack.Version != expectedVersion)
                        versionsDiffer = true;
                }

                if (missingHosts.Count != 0)
                {
                    badPacks.Add(string.Format(Messages.SUPP_PACK_MISSING_ON,
                        homogeneousPacks[pack],  // the Description field from the pack, which we cunningly remembered to save above
                        string.Join("\n", missingHosts.ConvertAll(host => host.ToString()).ToArray())));  // comma-separated list of hosts
                }
                else if (versionsDiffer)
                {
                    badPacks.Add(string.Format(Messages.SUPP_PACK_VERSIONS_DIFFER, homogeneousPacks[pack]));
                }
            }

            return badPacks;
        }

        /// <summary>
        /// Check whether the host that joins the pool has a more extensive feature set than the pool (as long as the CPU vendor is common)
        /// In this case the host will transparently be down-levelled to the pool level (without needing reboots)
        /// </summary>
        public static bool HostHasMoreFeatures(Host slave, Pool pool)
        {
            if (slave == null || pool == null)
                return false;

            Dictionary<string, string> slave_cpu_info = slave.cpu_info;
            Dictionary<string, string> pool_cpu_info = pool.cpu_info;
            if (!Helper.AreEqual2(slave_cpu_info, null) && !Helper.AreEqual2(pool_cpu_info, null))
            {
                // if pool has less features than slave, then slave will be down-levelled
                return FewerFeatures(pool_cpu_info, slave_cpu_info);
            }
            return false;
        }

        /// <summary>
        /// Check whether the host that joins the pool has a less extensive feature set than the pool (as long as the CPU vendor is common)
        /// In this case the pool is transparently down-levelled to the new host's level (without needing reboots)
        /// </summary>
        public static bool HostHasFewerFeatures(Host slave, Pool pool)
        {
            if (slave == null || pool == null)
                return false;

            Dictionary<string, string> slave_cpu_info = slave.cpu_info;
            Dictionary<string, string> pool_cpu_info = pool.cpu_info;
            if (!Helper.AreEqual2(slave_cpu_info, null) && !Helper.AreEqual2(pool_cpu_info, null))
            {
                // if slave has less features than pool, then pool will be down-levelled
                return FewerFeatures(slave_cpu_info, pool_cpu_info);
            }
            return false;
        }

        /// <summary>
        /// Check whether first CPU has fewer features than the second. 
        /// It returns true if the first feature set is less than the second one in at least one bit
        /// </summary>
        public static bool FewerFeatures(Dictionary<string, string> cpu_infoA, Dictionary<string, string> cpu_infoB)
        {
            if (cpu_infoA.ContainsKey("features_hvm") && cpu_infoB.ContainsKey("features_hvm") &&
                FewerFeatures(cpu_infoA["features_hvm"], cpu_infoB["features_hvm"]))
                return true;
            if (cpu_infoA.ContainsKey("features_pv") && cpu_infoB.ContainsKey("features_pv") &&
                FewerFeatures(cpu_infoA["features_pv"], cpu_infoB["features_pv"]))
                return true;

            return false;
        }

        public static bool FewerFeatures(string featureSetA, string featureSetB)
        {
            if (string.IsNullOrEmpty(featureSetA) || string.IsNullOrEmpty(featureSetB))
                return false;

            string stringA = featureSetA.Replace(" ", "");
            stringA = stringA.Replace("-", "");
            string stringB = featureSetB.Replace(" ", "");
            stringB = stringB.Replace("-", "");

            if (stringA.Length < stringB.Length)
                stringA = stringA.PadRight(stringB.Length, '0');
            if (stringB.Length < stringA.Length)
                stringB = stringB.PadRight(stringA.Length, '0');
            
            for (int i = 0; i < stringA.Length / 8; ++i)
            {
                uint intA = Convert.ToUInt32(stringA.Substring(8 * i, 8), 16);
                uint intB = Convert.ToUInt32(stringB.Substring(8 * i, 8), 16);

                if ((intA & intB) != intB)
                    return true;
            }
            return false;
        }

        public static bool HasSlaveAnyNonPhysicalPif(IXenConnection slaveConnection)
        {
            return
                slaveConnection.Cache.PIFs.Any(p => !p.physical);
        }

        public static bool HasCompatibleManagementInterface(IXenConnection slaveConnection)
        {
            /* if there are non physical pifs present then the slave should have 
             * only one VLAN and it has to be the management interface. 
             * Bonds and cross server private networks are not allowed */

            int numberOfNonPhysicalPifs = slaveConnection.Cache.PIFs.Count(p => !p.physical);
            
            /* allow the case where there are only physical pifs */
            if (numberOfNonPhysicalPifs == 0)
                return true;

            if(numberOfNonPhysicalPifs != 1)
                return false;
            else
                return slaveConnection.Cache.PIFs.Any(p => !p.physical && p.management && p.VLAN != -1);
        }

        public static bool HasIpForClusterNetwork(IXenConnection masterConnection, Host slaveHost, out bool clusterHostInBond)
        {
            clusterHostInBond = false;
            var clusterHost = masterConnection.Cache.Cluster_hosts.FirstOrDefault();

            if (clusterHost == null)
                return true;

            var clusterHostPif = clusterHost.Connection.Resolve(clusterHost.PIF);

            if (clusterHostPif == null)
                return true;

            clusterHostInBond = clusterHostPif.IsBondNIC();

            var pifsWithIPAddress = 0;

            List<string> ids = new List<string>();

            if (clusterHostInBond)
            {

                List<PIF> slaves = new List<PIF>();

                var bonds = masterConnection.ResolveAll(clusterHostPif.bond_master_of);

                foreach (var bond in bonds)
                {
                    slaves.AddRange(masterConnection.ResolveAll(bond.slaves));
                }

                ids.AddRange(slaves.Select(slave => slave.device));
            }
            else
            {
                ids.Add(clusterHostPif.device);
            }

            var pifs = slaveHost.Connection.ResolveAll(slaveHost.PIFs);

            foreach (var pif in pifs)
            {
                if (pif.IsManagementInterface(false) && ids.Contains(pif.device))
                {
                    pifsWithIPAddress += 1;
                }
            }

            return pifsWithIPAddress == 1;

        }
    }
}
