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
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAPI;

namespace XenAdmin.Diagnostics.Problems.HostProblem
{
    public enum HostNotSafeToUpgradeReason { NotEnoughSpace, Default }

    public class HostNotSafeToUpgradeWarning : WarningWithMoreInfo
    {
        private readonly Host host;
        private HostNotSafeToUpgradeReason reason;

        public HostNotSafeToUpgradeWarning(Check check, Host host, HostNotSafeToUpgradeReason reason)
            : base(check)
        {
            this.host = host;
            this.reason = reason;
        }

        public override string Title => Description;

        public override string Description => String.Format(ShortMessage, host.name_label);

        public override string Message
        {
            get
            {
                switch (reason)
                {
                    case HostNotSafeToUpgradeReason.NotEnoughSpace :
                        return string.Format(Messages.NOT_SAFE_TO_UPGRADE_NOT_ENOUGH_SPACE_LONG, BrandManager.ProductVersion70);
                    
                    default:
                        return string.Format(Messages.NOT_SAFE_TO_UPGRADE_DEFAULT_WARNING_LONG, BrandManager.ProductVersion70);
                }
            }
        }

        private string ShortMessage
        {
            get
            {
                switch (reason)
                {
                    case HostNotSafeToUpgradeReason.NotEnoughSpace:
                        return Messages.NOT_SAFE_TO_UPGRADE_NOT_ENOUGH_SPACE_SHORT;

                    default:
                        return Messages.NOT_SAFE_TO_UPGRADE_DEFAULT_WARNING_SHORT;
                }
            }
        }
    }
}
