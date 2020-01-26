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

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public abstract class HostUpdateMapping
    {
        protected readonly Host MasterHost;
        public List<string> HostsThatNeedEvacuated = new List<string>();

        protected HostUpdateMapping(Host masterHost)
        {
            MasterHost = masterHost ?? throw new ArgumentNullException("masterHost");
        }

        protected bool Matches(Host masterHost)
        {
            return MasterHost != null && masterHost != null && MasterHost.uuid == masterHost.uuid;
        }

        public override bool Equals(object obj)
        {
            return obj is HostUpdateMapping other && Matches(other.MasterHost);
        }

        public override int GetHashCode()
        {
            return MasterHost.GetHashCode();
        }

        public abstract bool IsValid { get; }
    }

    
    public abstract class XenServerPatchMapping : HostUpdateMapping
    {
        public readonly XenServerPatch XenServerPatch;

        protected XenServerPatchMapping(XenServerPatch xenServerPatch, Host masterHost)
            : base(masterHost)
        {
            XenServerPatch = xenServerPatch ?? throw new ArgumentNullException("xenServerPatch");
        }

        public bool Matches(Host masterHost, XenServerPatch xenServerPatch)
        {
            return Matches(masterHost) && XenServerPatch.Equals(xenServerPatch);
        }

        public override bool Equals(object obj)
        {
            return obj is XenServerPatchMapping other && Matches(other.MasterHost, other.XenServerPatch);
        }

        public override int GetHashCode()
        {
            return XenServerPatch.GetHashCode() ^ base.GetHashCode();
        }
    }


    public class PoolPatchMapping : XenServerPatchMapping
    {
        public readonly Pool_patch Pool_patch;

        public PoolPatchMapping(XenServerPatch xenServerPatch, Pool_patch pool_patch, Host masterHost)
            : base(xenServerPatch, masterHost)
        {
            Pool_patch = pool_patch ?? throw new ArgumentNullException("pool_patch");
        }

        public bool Matches(Host masterHost, XenServerPatch xenServerPatch, Pool_patch patch )
        {
            return Matches(masterHost, xenServerPatch) && patch != null &&
                   string.Equals(patch.uuid, Pool_patch.uuid, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return obj is PoolPatchMapping other && Matches(other.MasterHost, other.XenServerPatch, other.Pool_patch);
        }

        public override int GetHashCode()
        {
            return Pool_patch.GetHashCode() ^ base.GetHashCode();
        }

        public override bool IsValid
        {
            get { return Pool_patch != null && Pool_patch.opaque_ref != null; }
        }
    }

    
    public class PoolUpdateMapping : XenServerPatchMapping
    {
        public readonly Pool_update Pool_update;
        public Dictionary<Host, SR> SrsWithUploadedUpdatesPerHost = new Dictionary<Host, SR>();

        public PoolUpdateMapping(XenServerPatch xenServerPatch, Pool_update pool_update, Host masterHost)
            : base(xenServerPatch, masterHost)
        {
            Pool_update = pool_update ?? throw new ArgumentNullException("pool_update");
        }

        public bool Matches(Host masterHost, XenServerPatch xenServerPatch, Pool_update update)
        {
            return Matches(masterHost, xenServerPatch) && update != null &&
                   string.Equals(update.uuid, Pool_update.uuid, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return obj is PoolUpdateMapping other && Matches(other.MasterHost, other.XenServerPatch, other.Pool_update);
        }

        public override int GetHashCode()
        {
            return Pool_update.GetHashCode() ^ base.GetHashCode();
        }

        public override bool IsValid
        {
            get { return Pool_update != null && Pool_update.opaque_ref != null; }
        }
    }


    public class OtherLegacyMapping : HostUpdateMapping
    {
        public readonly string Path;
        public readonly Pool_patch Pool_patch;

        public OtherLegacyMapping(string path, Pool_patch pool_patch, Host masterHost)
            : base(masterHost)
        {
            Path = !string.IsNullOrEmpty(path) ? path : throw new ArgumentNullException("path");
            Pool_patch = pool_patch;
        }

        public bool Matches(Host masterHost, string path, Pool_patch patch = null)
        {
            if (patch == null)
                return Matches(masterHost) && Path == path;

            return Matches(masterHost) && Path == path && Pool_patch != null &&
                   string.Equals(patch.uuid, Pool_patch.uuid, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return obj is OtherLegacyMapping other && Matches(other.MasterHost, other.Path, other.Pool_patch);
        }

        public override int GetHashCode()
        {
            if (Pool_patch == null)
                return Path.GetHashCode() ^ base.GetHashCode();

            return Path.GetHashCode() ^ Pool_patch.GetHashCode() ^ base.GetHashCode();
        }

        public override bool IsValid
        {
            get
            {
                if (Pool_patch == null)
                    return true;

                return Pool_patch.opaque_ref != null;
            }
        }
    }


    public class SuppPackMapping : HostUpdateMapping
    {
        public readonly string Path;
        public readonly Pool_update Pool_update;
        public Dictionary<Host, SR> SrsWithUploadedUpdatesPerHost = new Dictionary<Host, SR>();
        public Dictionary<Host, VDI> SuppPackVdis = new Dictionary<Host, VDI>();

        public SuppPackMapping(string path, Pool_update pool_update, Host masterHost)
            : base(masterHost)
        {
            Path = !string.IsNullOrEmpty(path) ? path : throw new ArgumentNullException("path");
            Pool_update = pool_update;
        }

        public bool Matches(Host masterHost, string path, Pool_update update = null)
        {
            if (update == null)
                return Matches(masterHost) && Path == path;

            return Matches(masterHost) && Path == path && Pool_update != null &&
                   string.Equals(update.uuid, Pool_update.uuid, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return obj is SuppPackMapping other && Matches(other.MasterHost, other.Path, other.Pool_update);
        }

        public override int GetHashCode()
        {
            if (Pool_update == null)
                return Path.GetHashCode() ^ base.GetHashCode();

            return Path.GetHashCode() ^ Pool_update.GetHashCode() ^ base.GetHashCode();
        }

        public override bool IsValid
        {
            get
            {
                if (Pool_update == null)
                    return true;

                return Pool_update.opaque_ref != null;
            }
        }
    }
}
