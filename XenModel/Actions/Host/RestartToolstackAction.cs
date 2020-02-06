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

using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Actions
{
    public class RestartToolstackAction : AsyncAction
    {
        public RestartToolstackAction(Host host)
            : base(host.Connection, string.Format(Messages.ACTION_TOOLSTACK_RESTART_ON, host.Name().Ellipsise(30)))
        {
            Host = host;
            ApiMethodsToRoleCheck.Add("host.restart_agent");
        }

        protected override void Run()
        {
            var session = NewSession();

            Description = string.Format(Messages.ACTION_TOOLSTACK_RESTARTING_ON, Host.Name().Ellipsise(30));
            RelatedTask = Host.async_restart_agent(session, Host.opaque_ref);
            PollToCompletion(0, 100);

            //call interrupt so we can reconnect afterwards
            if (Helpers.HostIsMaster(Host))
                Host.Connection.Interrupt();

            Description = string.Format(Messages.ACTION_TOOLSTACK_RESTARTED_ON, Host.Name().Ellipsise(30));
        }
    }
}
