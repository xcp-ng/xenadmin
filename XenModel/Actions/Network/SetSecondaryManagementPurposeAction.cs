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
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Actions
{
    public class SetSecondaryManagementPurposeAction : AsyncAction
    {
        private Pool pool;
        private List<PIF> pifs;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SetSecondaryManagementPurposeAction(IXenConnection connection, Pool pool, List<PIF> pifs)
            : base(connection, Messages.ACTION_SET_SECONDARY_MANAGEMENT_PURPOSE_TITLE)
        {
            this.pool = pool;
            this.pifs = pifs;
            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("pif.set_other_config");
            #endregion
        }

        protected override void Run()
        {
            foreach (PIF pif in pifs)
            {
                XenAPI.Network network = Connection.Resolve(pif.network);
                if (network == null)
                {
                    log.Warn("Network has gone away");
                    return;
                }

                List<PIF> allPifs = Connection.ResolveAll(network.PIFs);
                List<PIF> toReconfigure = pool != null ? allPifs : allPifs.FindAll(
                    p => p.host.opaque_ref == pif.host.opaque_ref);

                if (toReconfigure.Count == 0)
                    return;

                foreach (PIF p in toReconfigure)
                {
                    p.Locked = true;
                    try
                    {
                        pif.SaveChanges(Session, p.opaque_ref, p);
                    }
                    finally
                    {
                        p.Locked = false;
                    }
                }
            }
            Description = Messages.ACTION_SET_SECONDARY_MANAGEMENT_PURPOSE_DONE;
        }
    }
}
