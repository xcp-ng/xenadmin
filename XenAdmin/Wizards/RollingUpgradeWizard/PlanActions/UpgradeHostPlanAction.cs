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
using System.Windows.Forms;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Wizards.RollingUpgradeWizard.PlanActions
{
    public class UpgradeAutomatedHostPlanAction : UpgradeHostPlanAction
    {
        private readonly Dictionary<string, string> _arguments;

        public UpgradeAutomatedHostPlanAction(Host host, Control invokingControl, Dictionary<string, string> arguments)
            : base(host, invokingControl)
        {
            _arguments = arguments ?? new Dictionary<string, string>();
        }

        protected override void RunWithSession(ref Session session)
        {
            string hostVersionBefore = CurrentHost.LongProductVersion();
            log.InfoFormat("Host '{0}' upgrading from version '{1}'", CurrentHost.Name(), hostVersionBefore);

            string value = Host.call_plugin(session, HostXenRef.opaque_ref, "prepare_host_upgrade.py", "main", _arguments);
            if (value.ToLower() == "true")
                Upgrade(ref session);
            else
                throw new Exception(value);

            Host hostAfterReboot = GetResolvedHost();
            if (hostAfterReboot == null)
            {
                log.InfoFormat("Cannot check host's version after reboot because the host '{0}' cannot be resolved", CurrentHost.Name());
            }
            else
            {
                if (Helpers.SameServerVersion(hostAfterReboot, hostVersionBefore))
                {
                    log.ErrorFormat("Host '{0}' rebooted with the same version '{1}'", hostAfterReboot.Name(), hostAfterReboot.LongProductVersion());
                    var error = new Exception(Messages.REBOOT_WITH_SAME_VERSION);
                    //when the slave reboots with the same version do not interrupt the process,
                    //so set the error without throwing it
                    if (hostAfterReboot.IsMaster())
                        throw error;
                    Error = error;
                }

                log.InfoFormat("Host '{0}' upgraded with version '{1}'", hostAfterReboot.Name(), hostAfterReboot.LongProductVersion());
            }
        }
    }
}
