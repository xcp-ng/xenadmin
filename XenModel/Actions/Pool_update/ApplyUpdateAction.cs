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
using System.Linq;


namespace XenAdmin.Actions
{
    public class ApplyUpdateAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Pool_update update;
        private readonly Host host;

        public ApplyUpdateAction(Pool_update update, Host host, bool suppressHistory)
            : base(host.Connection, string.Format(Messages.UPDATES_WIZARD_APPLYING_UPDATE, update.Name(), host.Name()), suppressHistory)
        {
            this.update = update;
            this.host = host;
        }

        protected override void Run()
        {
            SafeToExit = false;

            if (update.AppliedOn(host))
                return;

            Description = string.Format(Messages.APPLYING_PATCH, update.Name(), host.Name());
            log.DebugFormat("Applying update '{0}' to server '{1}'...", update.Name(), host.Name());

            var poolUpdates = new List<Pool_update>(Connection.Cache.Pool_updates);
            var poolUpdate = poolUpdates.FirstOrDefault(u => u != null && string.Equals(u.uuid, update.uuid, StringComparison.OrdinalIgnoreCase));

            if (poolUpdate == null)
                throw new Failure(Failure.INTERNAL_ERROR, Messages.POOL_UPDATE_GONE);

            if (!poolUpdate.AppliedOn(host))
            {
                RelatedTask = Pool_update.async_apply(Session, poolUpdate.opaque_ref, host.opaque_ref);
                PollToCompletion();
                Description = string.Format(Messages.PATCH_APPLIED, update.Name(), host.Name());
            }
            else
            {
                Description = string.Format(Messages.PATCH_APPLIED_ALREADY, update.Name(), host.Name());
            }
        }
    }

}
