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
using System.Linq;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.PoolProblem;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace XenAdmin.Diagnostics.Checks
{
    class PVGuestsCheck : HostPostLivenessCheck
    {
        private readonly Pool _pool;
        private readonly bool _upgrade;
        private readonly Dictionary<string, string> _installMethodConfig;
        private readonly bool _manualUpgrade;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PVGuestsCheck(Pool pool, bool upgrade, Dictionary<string, string> installMethodConfig = null, bool manualUpgrade = false)
            : base(Helpers.GetMaster(pool.Connection))
        {
            _pool = pool;
            _upgrade = upgrade;
            _installMethodConfig = installMethodConfig;
            _manualUpgrade = manualUpgrade;
        }

        protected override Problem RunHostCheck()
        {
            string upgradePlatformVersion;
            if (!_pool.Connection.Cache.VMs.Any(vm => vm.IsPvVm()))
                return null;
            if (!_upgrade || _manualUpgrade)
                return new PoolHasPVGuestWarningUrl(this, _pool);
            try
            {
                var result = Host.call_plugin(Host.Connection.Session, Host.opaque_ref, "prepare_host_upgrade.py", "getVersion", _installMethodConfig);
                var serializer = new JavaScriptSerializer();
                var version = (Dictionary<string, object>)serializer.DeserializeObject(result);
                upgradePlatformVersion = version.ContainsKey("platform-version") ? (string)version["platform-version"] : null;
            }
            catch (Exception exception)
            {
                log.Warn($"Plugin call prepare_host_upgrade.getVersion on {Host.Name()} threw an exception.", exception);
                return new PoolHasPVGuestWarningUrl(this, _pool);
            }
            if (Helpers.QuebecOrGreater(upgradePlatformVersion))
                return new PoolHasPVGuestWarningUrl(this, _pool);
            return null;
        }

        public override string Description => Messages.PV_GUESTS_CHECK_DESCRIPTION;
    }
}
