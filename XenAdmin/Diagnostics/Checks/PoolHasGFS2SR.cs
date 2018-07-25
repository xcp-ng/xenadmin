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
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.PoolProblem;

namespace XenAdmin.Diagnostics.Checks
{
    class PoolHasGFS2SR : HostPostLivenessCheck
    {
        public PoolHasGFS2SR(Host host)
            : base(host)
        {
        }

        protected override Problem RunHostCheck()
        {
            var clusteringEnabled = Host.Connection.Cache.Cluster_hosts.Any(cluster => cluster.enabled);
            var hasGfs2Sr = Host.Connection.Cache.SRs.Any(sr => sr.GetSRType(true) == SR.SRTypes.gfs2);

            if (clusteringEnabled || hasGfs2Sr)
                return new PoolHasGFS2SRProblem(this, Helpers.GetPoolOfOne(Host.Connection), clusteringEnabled, hasGfs2Sr);


            return null;
        }

        public override string Description
        {
            get
            {
                return string.Format(Messages.CLUSTERING_STATUS_CHECK, Helpers.GetPoolOfOne(Host.Connection));
            }
        }
    }
}
