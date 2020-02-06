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
using System.Diagnostics;

namespace XenAdmin.Core
{
    [DebuggerDisplay("XenServerVersion (Name={Name}; Patches.Count={Patches.Count}; MinimalPatches.Count={MinimalPatches.Count})")]
    public class XenServerVersion
    {
        public Version Version;
        public string Name;
        public bool Latest;
        public bool LatestCr;
        public string Url;
        public List<XenServerPatch> Patches;
        public string PatchUuid;
        public bool PresentAsUpdate;
        public Version MinimumXcVersion;
        public hotfix_eligibility HotfixEligibility;
        public DateTime HotfixEligibilityPremiumDate;
        public DateTime HotfixEligibilityNoneDate;
        public DateTime EolDate;
        
        /// <summary>
        /// A host of this version is considered up-to-date when it has all the patches in this list installed on it
        /// <value>null</value> means that the list is not known (automated updates are not supported)
        /// </summary>
        public List<XenServerPatch> MinimalPatches;
        
        public DateTime TimeStamp;
        public string BuildNumber;

        /// <summary>
        /// Defines metadata for a XenServer version
        /// </summary>
        /// <param name="version_oem"></param>
        /// <param name="name"></param>
        /// <param name="latest"></param>
        /// <param name="latestCr"></param>
        /// <param name="url"></param>
        /// <param name="patches"></param>
        /// <param name="minimumPatches">can be null (see <paramref name="MinimalPatches"/></param>
        /// <param name="timestamp"></param>
        /// <param name="buildNumber"></param>
        /// <param name="patchUuid"></param>
        /// <param name="presentAsUpdate">Indicates that the new version (usually a CU) should be presented as an update where possible</param>
        public XenServerVersion(string version_oem, string name, bool latest, bool latestCr, string url, List<XenServerPatch> patches, List<XenServerPatch> minimumPatches,
            string timestamp, string buildNumber, string patchUuid, bool presentAsUpdate, string minXcVersion, string hotfixEligibility, string hotfixEligibilityPremiumDate,
            string hotfixEligibilityNoneDate, string eolDate)
        {
            ParseVersion(version_oem);
            Name = name;
            Latest = latest;
            LatestCr = latestCr;
            Url = url;
            Patches = patches;
            MinimalPatches = minimumPatches;
            DateTime.TryParse(timestamp, out TimeStamp);
            BuildNumber = buildNumber;
            PatchUuid = patchUuid;
            PresentAsUpdate = presentAsUpdate;
            ParseMinXcVersion(minXcVersion);
            Enum.TryParse(hotfixEligibility, out HotfixEligibility);
            DateTime.TryParse(hotfixEligibilityPremiumDate, out HotfixEligibilityPremiumDate);
            DateTime.TryParse(hotfixEligibilityNoneDate, out HotfixEligibilityNoneDate);
            DateTime.TryParse(eolDate, out EolDate);
        }

        private void ParseVersion(string version_oem)
        {
            string[] bits = version_oem.Split('.');
            List<string> ver = new List<string>();
            foreach (string bit in bits)
            {
                int num;
                if (Int32.TryParse(bit, out num))
                    ver.Add(bit);
            }
            Version = new Version(string.Join(".", ver.ToArray()));
        }

        private void ParseMinXcVersion(string minXcVersion)
        {
            Version ver;
            Version.TryParse(minXcVersion, out ver);
            MinimumXcVersion = ver;
        }

        public bool IsVersionAvailableAsAnUpdate
        {
            get
            {
                return !string.IsNullOrEmpty(PatchUuid);
            }
        }
    }

    public enum hotfix_eligibility
    {
        /// <summary>
        /// All customers are eligible for hotfixes.
        /// </summary>
        all,
        /// <summary>
        /// Only paying customers are eligible for hotfixes.
        /// </summary>
        premium,
        /// <summary>
        /// The only hotfix available is a Cumulative Update, for paying customers.
        /// </summary>
        cu,
        /// <summary>
        /// The version has reached EOL for all customers and no more hotfixes will be released.
        /// </summary>
        none
    }
}