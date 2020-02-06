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
using System.Web.Script.Serialization;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAPI;

namespace XenAdmin.Diagnostics.Checks
{
    class HostMemoryPostUpgradeCheck : HostPostLivenessCheck
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<string, string> installMethodConfig;

        public HostMemoryPostUpgradeCheck(Host host, Dictionary<string, string> installMethodConfig)
            : base(host)
        {
            this.installMethodConfig = installMethodConfig;
        }
        public override string Description => Messages.CHECKING_HOST_MEMORY_POST_UPGRADE_DESCRIPTION;

        protected override Problem RunHostCheck()
        {
            if (TryGetDom0MemoryPostUpgrade(out var dom0MemoryPostUpgrade))
            {
                var currentDom0Memory = Host.dom0_memory();
                // we know the Dom0 memory after the upgrade, check if it will greater then the current value
                if (dom0MemoryPostUpgrade > currentDom0Memory)
                {
                    // add warning that the the Dom0 memory will be changed after the upgrade 
                    return new HostMemoryPostUpgradeWarning(this, Host, dom0MemoryPostUpgrade);
                }
            }
            else
            {
                if (Helpers.NaplesOrGreater(Host))
                    return null;

                // we don't know the Dom0 memory after the upgrade, so add generic warning if upgrading from pre-Naples 
                string upgradePlatformVersion = null;
                string upgradeProductVersion = null;
                if (installMethodConfig != null)
                    TryGetUpgradeVersion(out upgradePlatformVersion, out upgradeProductVersion);
                
                if (Helpers.NaplesOrGreater(upgradePlatformVersion))
                {
                    // we know that they are upgrading to Naples or greater, so add specific warning
                    return new HostMemoryPostUpgradeWarning(this, Host, 0, upgradeProductVersion);
                }
                // we don't know the upgrade version, so add generic warning (this is the case of the manual upgrade or when the rpu plugin doesn't have the function)
                return new HostMemoryPostUpgradeWarning(this, Host);
            }
            return null;
        }

        private bool TryGetDom0MemoryPostUpgrade(out long dom0MemoryAfterUpgrade)
        {
            dom0MemoryAfterUpgrade = 0;

            try
            {
                var result = Host.call_plugin(Host.Connection.Session, Host.opaque_ref, "prepare_host_upgrade.py", "getDom0DefaultMemory", installMethodConfig);
                return long.TryParse(result, out dom0MemoryAfterUpgrade);
            }
            catch (Exception exception)
            {
                var failure = exception as Failure;
                log.WarnFormat("Plugin call prepare_host_upgrade.getDom0DefaultMemory on {0} failed with {1}({2})", Host.Name(), exception.Message, failure?.ErrorDescription.Count>3? failure.ErrorDescription[3]:"");
                return false;
            }
        }

        private bool TryGetUpgradeVersion(out string platformVersion, out string productVersion)
        {
            platformVersion = productVersion = null;
            try
            {
                var result = Host.call_plugin(Host.Connection.Session, Host.opaque_ref, "prepare_host_upgrade.py", "getVersion", installMethodConfig);
                var serializer = new JavaScriptSerializer();
                var version = (Dictionary<string, object>)serializer.DeserializeObject(result);
                platformVersion = version.ContainsKey("platform-version") ? (string)version["platform-version"] : null;
                productVersion = version.ContainsKey("product-version") ? (string)version["product-version"] : null;
                return platformVersion != null || productVersion != null;
            }
            catch (Exception exception)
            {
                log.WarnFormat("Plugin call prepare_host_upgrade.getVersion on {0} failed with {1}", Host.Name(), exception.Message);
                return false;
            }
        }
    }
}
