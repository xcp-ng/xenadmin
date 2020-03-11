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
using XenAdmin.Diagnostics.Checks;
using XenAPI;

namespace XenAdmin.Diagnostics.Problems.HostProblem
{
    class HostMemoryPostUpgradeWarning : WarningWithMoreInfo
    {
        private readonly Host host;
        private readonly long dom0MemoryAfterUpgrade;
        private readonly string upgradeProductVersion;

        public HostMemoryPostUpgradeWarning(Check check, Host host, long dom0MemoryAfterUpgrade = 0, string upgradeProductVersion = null)
            : base(check)
        {
            this.host = host;
            this.upgradeProductVersion = upgradeProductVersion;
            this.dom0MemoryAfterUpgrade = dom0MemoryAfterUpgrade;
        }

        public override string Title => Description;

        public override string Description
        {
            get
            {
                if (dom0MemoryAfterUpgrade > 0)
                    return string.Format(Messages.HOST_MEMORY_POST_UPGRADE_DOM0_MEMORY_WARNING_SHORT, Helpers.GetName(host).Ellipsise(30));
                if (string.IsNullOrEmpty(upgradeProductVersion))
                    return string.Format(Messages.HOST_MEMORY_POST_UPGRADE_DEFAULT_WARNING_SHORT, Helpers.GetName(host).Ellipsise(30));
                return string.Format(Messages.HOST_MEMORY_POST_UPGRADE_VERSION_WARNING_SHORT, Helpers.GetName(host).Ellipsise(30));
            }
        }

        public override string Message
        {
            get
            {
                if (dom0MemoryAfterUpgrade > 0)
                    return string.Format(Messages.HOST_MEMORY_POST_UPGRADE_DOM0_MEMORY_WARNING_LONG, Helpers.GetName(host), Util.MemorySizeStringSuitableUnits(dom0MemoryAfterUpgrade, true));
                if (string.IsNullOrEmpty(upgradeProductVersion))
                    return string.Format(Messages.HOST_MEMORY_POST_UPGRADE_DEFAULT_WARNING_LONG, Helpers.GetName(host), BrandManager.ProductVersion80);
                return string.Format(Messages.HOST_MEMORY_POST_UPGRADE_VERSION_WARNING_LONG, Helpers.GetName(host), upgradeProductVersion);
            }
        }
    }
}
