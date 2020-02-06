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
using XenAPI;

namespace XenAdmin.Actions
{
    public class PoolPatchCleanAction : AsyncAction
    {
        private readonly Pool_patch patch;

        public PoolPatchCleanAction(Pool pool, Pool_patch patch, bool suppressHistory)
            : base(pool.Connection, string.Format(Messages.UPDATES_WIZARD_REMOVING_UPDATE, patch.Name(), pool.Name()), suppressHistory)
        {
            this.patch = patch;
            if (patch == null)
                throw new ArgumentNullException("patch");

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("pool_patch.pool_clean");
            #endregion
            
        }

        protected override void Run()
        {
            this.Description = string.Format(Messages.REMOVING_UPDATE, patch.Name());
            List<Pool_patch> poolPatches = new List<Pool_patch>(Connection.Cache.Pool_patches);
            var poolPatch = poolPatches.Find(p => string.Equals(p.uuid, patch.uuid, StringComparison.OrdinalIgnoreCase));

            if (poolPatch != null)
            {
                Pool_patch.pool_clean(Session, poolPatch.opaque_ref);
            }
            Description = String.Format(Messages.REMOVED_UPDATE, patch.Name());
        }
    }
}
