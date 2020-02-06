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
using System.Text;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Actions
{
    public class RestoreDismissedUpdatesAction : AsyncAction
    {
        public RestoreDismissedUpdatesAction(IXenConnection connection)
            : base(connection, "restore_dismissed_updates", "restore_dismissed_updates", true)
        {
        }

        protected override void Run()
        {
            if (!AllowedToRestoreDismissedUpdates())
                return;

            XenAPI.Pool pool = Helpers.GetPoolOfOne(Connection);
            if (pool == null)
                return;

            Dictionary<string, string> other_config = pool.other_config;

            if (other_config.ContainsKey(Updates.IgnorePatchKey))
                other_config.Remove(Updates.IgnorePatchKey);

            if (other_config.ContainsKey(Updates.LAST_SEEN_SERVER_VERSION_KEY))
                other_config.Remove(Updates.LAST_SEEN_SERVER_VERSION_KEY);


            XenAPI.Pool.set_other_config(Connection.Session, pool.opaque_ref, other_config);
        }

        /// <summary>
        /// Checks the user has sufficient RBAC privileges to restore dismissed alerts on a given connection
        /// </summary>
        private bool AllowedToRestoreDismissedUpdates()
        {
            if (Connection == null || Connection.Session == null)
                return false;

            if (Connection.Session.IsLocalSuperuser)
                return true;

            List<Role> rolesAbleToCompleteAction = Role.ValidRoleList("Pool.set_other_config", Connection);
            foreach (Role possibleRole in rolesAbleToCompleteAction)
            {
                if (Connection.Session.Roles.Contains(possibleRole))
                    return true;
            }
            return false;
        }
    }
}
