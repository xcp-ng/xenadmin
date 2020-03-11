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
using System.Threading;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Diagnostics.Hotfixing
{
    internal class SingleHotfix : Hotfix
    {
        public override string UUID { get; set; }
        public override string Filename { get; set; }

        public sealed override void Apply(Host host, Session session)
        {
            bool elyOrGreater = Helpers.ElyOrGreater(host);
            if (elyOrGreater)
                UploadAndApplyUpdate(host, session);
            else
                UploadAndApplyPatch(host, session);

            IXenObject patch;
            int numberRetries = 0;
            do
            {
                Thread.Sleep(500);
                patch = elyOrGreater
                    ? (IXenObject)host.Connection.Cache.Find_By_Uuid<Pool_update>(UUID)
                    : host.Connection.Cache.Find_By_Uuid<Pool_patch>(UUID);
                numberRetries++;

            } while (patch == null && numberRetries < 10);
        }

        private void UploadAndApplyPatch(Host host, Session session)
        {
            var patch = host.Connection.Cache.Find_By_Uuid<Pool_patch>(UUID);
            if (patch == null)
            {
                var master = Helpers.GetMaster(host.Connection);
                var filePath = Path.Combine(Program.AssemblyDir, String.Format("{0}.{1}", Filename, BrandManager.ExtensionUpdate));
                var action = new UploadPatchAction(master.Connection, filePath, false, false);
                action.RunExternal(session);
                patch = action.Patch;
            }
            new ApplyPatchAction(patch, host).RunExternal(session);
        }

        private void UploadAndApplyUpdate(Host host, Session session)
        {
            var update = host.Connection.Cache.Find_By_Uuid<Pool_update>(UUID);
            if (update == null)
            {
                var master = Helpers.GetMaster(host.Connection);
                var filePath = Path.Combine(Program.AssemblyDir, string.Format("{0}.{1}", Filename, InvisibleMessages.ISO_UPDATE));
                var action = new UploadSupplementalPackAction(master.Connection, new List<Host> { master }, filePath, false);
                action.RunExternal(session);
                update = action.PoolUpdate;
            }
            new ApplyUpdateAction(update, host, false).RunExternal(session);
        }
        
        public override bool ShouldBeAppliedTo(Host host)
        {
            if (Helpers.ElyOrGreater(host))
            {
                var updates = host.Connection.ResolveAll(host.updates);
                return !updates.Any(update => UUID.ToLowerInvariant().Contains(update.uuid.ToLowerInvariant()));
            }
            var patches = host.Connection.ResolveAll(host.patches);
            var poolPatches = patches.Select(hostPatch => hostPatch.Connection.Resolve(hostPatch.pool_patch));
            return !poolPatches.Any(patch => UUID.ToLowerInvariant().Contains(patch.uuid.ToLowerInvariant()));
        }
    }

}
