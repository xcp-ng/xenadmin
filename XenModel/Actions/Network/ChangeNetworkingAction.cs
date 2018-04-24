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
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public class ChangeNetworkingAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<PIF> newPIFs;
        private List<PIF> downPIFs;
        private PIF downManagement;
        private PIF newManagement;
        private bool managementIPChanged;

        private Host[] Hosts;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pool">May be null, in which case we're acting on a single host basis.</param>
        /// <param name="host">If pool is set, then host should be the pool master.</param>
        /// <param name="newPIFs">New PIF instances that are to be created on the server.</param>
        /// <param name="downPIFs"></param>
        /// <param name="newManagement">May be null, in which case the management network will not be disturbed.
        /// Note that it is still possible for the management interface to receive a new IP address even if this
        /// parameter is null, because it may be in newPIFs.  You only need to use this parameter if the management
        /// interface is switching onto a different PIF.</param>
        /// <param name="downManagement">May be null iff newManagement is null.</param>
        public ChangeNetworkingAction(IXenConnection connection, Pool pool, Host host, List<PIF> newPIFs, List<PIF> downPIFs,
            PIF newManagement, PIF downManagement, bool managementIPChanged)
            : base(connection, Messages.ACTION_CHANGE_NETWORKING_TITLE)
        {
            Pool = pool;
            Host = host;

            this.newPIFs = newPIFs;
            this.downPIFs = downPIFs;
            this.newManagement = newManagement;
            this.downManagement = downManagement;
            this.managementIPChanged = managementIPChanged;

            if (pool != null)
            {
                // If we're going to compute address ranges, then we need a sorted list of hosts (to keep the addresses stable).
                Hosts = Connection.Cache.Hosts;
                Array.Sort(Hosts);
                foreach (Host h in Hosts)
                {
                    AppliesTo.Add(h.opaque_ref);
                }
            }
            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("vm.set_memory_limits");
            ApiMethodsToRoleCheck.Add("host.management_reconfigure");
            ApiMethodsToRoleCheck.Add("pif.reconfigure_ip");
            ApiMethodsToRoleCheck.Add("pif.plug");
            ApiMethodsToRoleCheck.AddRange(XenAPI.Role.CommonSessionApiList);
            ApiMethodsToRoleCheck.AddRange(XenAPI.Role.CommonTaskApiList);
            if(!Helpers.FeatureForbidden(Connection, Host.RestrictManagementOnVLAN))
                ApiMethodsToRoleCheck.Add("pool.management_reconfigure");
            if (!Helpers.FeatureForbidden(Connection, Host.RestrictCorosync))
            {
                ApiMethodsToRoleCheck.Add("pbd.unplug");
                ApiMethodsToRoleCheck.Add("cluster_host.disable");
                ApiMethodsToRoleCheck.Add("pif.set_disallow_unplug");
                ApiMethodsToRoleCheck.Add("cluster_host.enable");
                ApiMethodsToRoleCheck.Add("pbd.plug");

            }
            #endregion

        }

        protected override void Run()
        {
            Connection.ExpectDisruption = !managementIPChanged;
            try
            {
                int inc = (Pool == null ? 100 : 50) / (newPIFs.Count + downPIFs.Count + (downManagement != null ? 1 : 0));
                int progress = PercentComplete;
                
                // We bring up / reconfigure the interfaces on the slaves.
                // Then the master.
                // Then we reconfigure the management interface on the slaves.
                // Then the master.
                // Then we bring down the other interfaces on the slaves.
                // And then the master.
                // If Pool isn't set, then we're just doing this for the one host.

                if (Pool != null)
                {
                    foreach (PIF pif in newPIFs)
                    {
                        progress += inc;
                        Reconfigure(pif, true, false, progress);
                    }
                }

                foreach (PIF pif in newPIFs)
                {
                    progress += inc;
                    Reconfigure(pif, true, true, progress);
                }

                if (downManagement != null)
                {
                    if (Pool != null)
                    {
                        progress += inc;
                        if (!Helpers.FeatureForbidden(Connection, Host.RestrictManagementOnVLAN))
                       {
                            PoolReconfigureManagement(progress);
                            return;
                       }

                        ReconfigureManagement(false, progress);
                    }

                    progress += inc;
                    ReconfigureManagement(true, progress);
                }

                if (Pool != null)
                {
                    foreach (PIF pif in downPIFs)
                    {
                        progress += inc;
                        Reconfigure(pif, false, false, progress);
                    }
                }
                foreach (PIF pif in downPIFs)
                {
                    progress += inc;
                    Reconfigure(pif, false, true, progress);
                }

                Description = Messages.ACTION_CHANGE_NETWORKING_DONE;
            }
            finally
            {
                Connection.ExpectDisruption = false;
            }
        }

        private void Reconfigure(PIF pif, bool up, bool this_host, int hi)
        {
            NetworkingActionHelpers.ForSomeHosts(this, pif, this_host, true, hi,
                delegate(AsyncAction a, PIF p, int h)
                {
                    List<PBD> gfs2Pbds;
                    DisableClustering(p, out gfs2Pbds);
                    if (up)
                        BringUp(pif, p, h);
                    else
                        NetworkingActionHelpers.BringDown(a, p, h);
                    EnableClustering(p, gfs2Pbds);
                });
        }

        private void ReconfigureManagement(bool this_host, int hi)
        {
            System.Diagnostics.Trace.Assert(downManagement != null);
            System.Diagnostics.Trace.Assert(newManagement != null);

            bool clearDownManagementIP = true;
            foreach (PIF p in newPIFs)
            {
                if (p.uuid == downManagement.uuid)
                {
                    clearDownManagementIP = false;
                    break;
                }
            }

            NetworkingActionHelpers.ReconfigureManagement(this, downManagement, newManagement, this_host, true, hi,
                                                          clearDownManagementIP);
        }

        private void PoolReconfigureManagement(int hi)
        {
            System.Diagnostics.Trace.Assert(downManagement != null);

            if (newManagement == null)
                return;

            NetworkingActionHelpers.PoolReconfigureManagement(this, Pool, newManagement, downManagement, hi);
        }

        private void BringUp(PIF new_pif, PIF existing_pif, int hi)
        {
            string ip = existing_pif.IP;
            if (new_pif.ip_configuration_mode == ip_configuration_mode.Static)
                ip = Pool == null ? new_pif.IP : GetIPInRange(new_pif.IP, existing_pif);

            NetworkingActionHelpers.BringUp(this, new_pif, ip, existing_pif, hi);
        }

        private string GetIPInRange(string range_start, PIF existing_pif)
        {
            int i = Array.FindIndex(Hosts, h => h.opaque_ref == existing_pif.host);
            if (i == -1)
                throw new Failure(Failure.INTERNAL_ERROR, Messages.HOST_GONE);

            string[] bits = range_start.Split('.');
            return string.Format("{0}.{1}.{2}.{3}", bits[0], bits[1], bits[2], int.Parse(bits[3]) + i);
        }

        /// <summary>
        /// Disable clustering on the host (if the network is used by clustering), before changing the management interface; 
        /// Before disabling clustering we also unplug all the GFS2 SRs
        /// </summary>
        private void DisableClustering(PIF pif, out List<PBD> gfs2Pbds)
        {
            gfs2Pbds = new List<PBD>();
            var isUsedByClustering = Connection.Cache.Clusters.Any(cluster => cluster.network.opaque_ref == pif.network.opaque_ref);
            if (!isUsedByClustering)
                return;

            var host = Connection.Resolve(pif.host);
            if (host == null)
                return;

            var clusterHost = Connection.Cache.Cluster_hosts.FirstOrDefault(c => c.host.opaque_ref == host.opaque_ref);
            if (clusterHost == null) 
                return;

            // unplug the GFS2 SRs, saving the list of the PBDs unplugged, to plug back later
            foreach (var pbd in Connection.ResolveAll(host.PBDs).Where(pbd => pbd.currently_attached))
            {
                var sr = Connection.Resolve(pbd.SR);
                if (sr != null && sr.GetSRType(true) == SR.SRTypes.gfs2)
                {
                    gfs2Pbds.Add(pbd);
                    Description = string.Format(Messages.ACTION_SR_DETACHING, sr.Name(), host.Name());
                    PBD.unplug(Session, pbd.opaque_ref);
                }
            }

            // disable clustering
            Description = string.Format(Messages.DISABLING_CLUSTERING_ON_POOL, host.Name());
            log.Debug(Description);
            Cluster_host.disable(Session, clusterHost.opaque_ref);
        }

        /// <summary>
        /// Enable clustering on the host (if the network is used by clustering), after the management interface has been changed; 
        /// After enabling clustering we also plug back all the GFS2 SRs that we unplugged
        /// </summary>
        private void EnableClustering(PIF pif, List<PBD> gfs2Pbds)
        {
            var isUsedByClustering = Connection.Cache.Clusters.Any(cluster => cluster.network.opaque_ref == pif.network.opaque_ref);
            if (!isUsedByClustering)
                return;

            var host = Connection.Resolve(pif.host);
            if (host == null)
                return;

            var clusterHost = Connection.Cache.Cluster_hosts.FirstOrDefault(c => c.host.opaque_ref == host.opaque_ref);
            if (clusterHost == null)
                return;

            Description = string.Format(Messages.ENABLING_CLUSTERING_ON_POOL, host.Name());
            log.Debug(Description);
            PIF.set_disallow_unplug(Session, pif.opaque_ref, true);
            Cluster_host.enable(Session, clusterHost.opaque_ref);

            // plug the GFS2 SRs
            foreach (var pbd in gfs2Pbds.Where(pbd => !pbd.currently_attached))
            {
                var sr = Connection.Resolve(pbd.SR);
                if (sr != null)
                    Description = string.Format(Messages.ACTION_SR_ATTACHING_TITLE, sr.Name(), host.Name());
                PBD.plug(Session, pbd.opaque_ref);
            }
        }
    }
}
