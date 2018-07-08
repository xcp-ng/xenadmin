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

using System.Linq;
using XenAPI;

namespace XenAdmin.Actions
{
    public class DisableClusteringAction : AsyncAction
    {
        public DisableClusteringAction(Pool pool)
            : base(pool.Connection, Messages.DISABLE_CLUSTERING_ON_POOL,
            string.Format(Messages.DISABLING_CLUSTERING_ON_POOL, pool.Name()), true)
        {
            #region RBAC Dependencies
            //ApiMethodsToRoleCheck.Add("pif.set_disallow_unplug");
            #endregion
        }

        protected override void Run()
        {
            var existingCluster = Connection.Cache.Clusters.FirstOrDefault();
            if (existingCluster != null)
            {
                Cluster.pool_destroy(Session, existingCluster.opaque_ref);
                var clusterHosts = Connection.ResolveAll(existingCluster.cluster_hosts);

                foreach (var clusterHost in clusterHosts)
                {
                    PIF.set_disallow_unplug(Session, clusterHost.PIF.opaque_ref, false);
                }
            }
            Description = string.Format(Messages.DISABLED_CLUSTERING_ON_POOL, Pool.Name());
        }
    }
}
