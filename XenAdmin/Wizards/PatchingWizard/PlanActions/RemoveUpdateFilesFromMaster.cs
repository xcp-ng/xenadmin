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
using XenAdmin.Core;
using XenAPI;
using System.Linq;
using System;


namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    class RemoveUpdateFileFromMasterPlanAction : PlanActionWithSession
    {
        private readonly List<PoolPatchMapping> patchMappings = new List<PoolPatchMapping>();
        private readonly XenServerPatch patch = null;
        private readonly Host master = null;

        public RemoveUpdateFileFromMasterPlanAction(Host master, List<PoolPatchMapping> patchMappings, XenServerPatch patch)
            : base(master.Connection)
        {
            this.patchMappings = patchMappings;
            this.patch = patch;
            this.master = master;
        }

        protected override void RunWithSession(ref Session session)
        {
            try
            {
                var mapping = patchMappings.FirstOrDefault(pm => pm.MasterHost != null && master != null &&
                                                                 pm.MasterHost.uuid == master.uuid && pm.XenServerPatch.Equals(patch));

                if (!Helpers.ElyOrGreater(session.Connection))
                {
                    Pool_patch poolPatch = null;

                    if (mapping != null && mapping.Pool_patch != null && mapping.Pool_patch.opaque_ref != null)
                    {
                        poolPatch = mapping.Pool_patch;
                    }
                    else
                    {
                        poolPatch = session.Connection.Cache.Pool_patches.FirstOrDefault(pp => string.Equals(pp.uuid, patch.Uuid, StringComparison.InvariantCultureIgnoreCase));
                    }

                    if (poolPatch != null && poolPatch.opaque_ref != null)
                    {
                        AddProgressStep(string.Format(Messages.UPDATES_WIZARD_REMOVING_UPDATES_FROM_POOL, poolPatch.Name(), master.Name()));
                        var task = Pool_patch.async_pool_clean(session, mapping.Pool_patch.opaque_ref);
                        PollTaskForResultAndDestroy(Connection, ref session, task);

                        patchMappings.Remove(mapping);
                    }
                }
                else
                {
                    if (mapping != null && mapping.Pool_update != null && mapping.Pool_update.opaque_ref != null)
                    {
                        var poolUpdate = mapping.Pool_update;
                        AddProgressStep(string.Format(Messages.UPDATES_WIZARD_REMOVING_UPDATES_FROM_POOL, poolUpdate.Name(), master.Name()));

                        Pool_update.pool_clean(session, poolUpdate.opaque_ref);
                        
                        if (!poolUpdate.AppliedOnHosts().Any())
                            Pool_update.destroy(session, poolUpdate.opaque_ref);

                        patchMappings.Remove(mapping);
                    }
                }
            }
            catch (Exception ex)
            {
                //best effort
                log.Error("Failed to remove update from the server.", ex);
            }
        }
    }
}
