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
using System.Linq;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAPI;


namespace XenAdmin.Diagnostics.Checks
{
    class SafeToUpgradeCheck : HostPostLivenessCheck
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SafeToUpgradeCheck(Host host)
            : base(host)
        {
        }

        protected override Problem RunHostCheck()
        {
            try
            {
                var config = new Dictionary<string, string>();
                var result = Host.call_plugin(Host.Connection.Session, Host.opaque_ref, "prepare_host_upgrade.py", "testSafe2Upgrade", config);

                switch (result.ToLowerInvariant())
                {
                    case "true":
                        return null;

                    case "not_enough_space":
                        return new HostNotSafeToUpgradeWarning(this, Host, HostNotSafeToUpgradeReason.NotEnoughSpace);

                    default:
                        return new HostNotSafeToUpgradeWarning(this, Host, HostNotSafeToUpgradeReason.Default);
                }
            }
            catch (Failure failure)
            {
                //note: handle the case when we get UNKNOWN_XENAPI_PLUGIN_FUNCTION - testSafe2Upgrade
                log.WarnFormat("Plugin call prepare_host_upgrade.testSafe2Upgrade on {0} failed with {1}", Host.Name(), failure.Message);
            }

            return null;
        }

        public override string Description
        {
            get { return Messages.CHECKING_SAFE_TO_UPGRADE_DESCRIPTION; }
        }
    }
}
