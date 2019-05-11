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
using XenAPI;

namespace XenAdmin.Diagnostics.Hotfixing
{
    internal sealed class HotfixFactory
    {
        public enum HotfixableServerVersion
        {
            Dundee,
            ElyLima
        }

        private readonly Hotfix dundeeHotfix = new SingleHotfix
        {
            Filename = "RPU003",
            UUID = "149be566-421d-4661-bfca-e70970f86a36"
        };

        private readonly Hotfix elyLimaHotfix = new SingleHotfix
        {
            Filename = "RPU004",
            UUID = "d72c237a-eaaf-4d98-be63-48e2add8dc3a"
        };

        public Hotfix Hotfix(Host host)
        {
            if (Helpers.ElyOrGreater(host) && !Helpers.NaplesOrGreater(host))
                return Hotfix(HotfixableServerVersion.ElyLima);
            if (Helpers.DundeeOrGreater(host) && !Helpers.ElyOrGreater(host))
                return Hotfix(HotfixableServerVersion.Dundee);
            return null;
        }

        public Hotfix Hotfix(HotfixableServerVersion version)
        {
            if (version == HotfixableServerVersion.ElyLima)
                return elyLimaHotfix;
            if (version == HotfixableServerVersion.Dundee)
                return dundeeHotfix;

            throw new ArgumentException("A version was provided for which there is no hotfix filename");
        }

        public bool IsHotfixRequired(Host host)
        {
            return Hotfix(host) != null;
        }
    }
}
