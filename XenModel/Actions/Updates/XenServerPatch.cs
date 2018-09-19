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
using XenAPI;

namespace XenAdmin.Core
{
    [DebuggerDisplay("XenServerPatch (Name={Name}; Uuid={Uuid})")]
    public class XenServerPatch : IEquatable<XenServerPatch>
    {
        private string _uuid;
        public readonly string Name;
        public readonly string Description;
        public readonly string Guidance;
        public readonly bool GuidanceMandatory;
        public readonly Version Version;
        public readonly string Url;
        public readonly string PatchUrl;
        public readonly DateTime TimeStamp;
        public readonly int Priority;
        public readonly long InstallationSize; // installation size, in btyes
        public readonly long DownloadSize; // download size, in btyes
        public readonly bool ContainsLivepatch;

        public readonly List<string> ConflictingPatches;
        public readonly List<string> RequiredPatches;

        private const int DEFAULT_PRIORITY = 2;

        public XenServerPatch(string uuid, string name, string description, string guidance, string guidance_mandatory , string version, string url,
            string patchUrl, string timestamp, string priority, string installationSize, string downloadSize, string containsLivepatch = "false")
        {
            _uuid = uuid;
            Name = name;
            Description = description;
            Guidance = guidance;
            GuidanceMandatory = !string.IsNullOrEmpty(guidance_mandatory) && guidance_mandatory.ToLowerInvariant().Contains("true");
            Version = new Version(version);
            Url = url;
            PatchUrl = patchUrl;
            DateTime.TryParse(timestamp, out TimeStamp);
            if (!Int32.TryParse(priority, out Priority))
                Priority = DEFAULT_PRIORITY;
            if (!Int64.TryParse(installationSize, out InstallationSize))
                InstallationSize = 0;
            if (!Int64.TryParse(downloadSize, out DownloadSize))
                DownloadSize = 0;
            ContainsLivepatch = !string.IsNullOrEmpty(containsLivepatch) && containsLivepatch.ToLowerInvariant().Contains("true");
        }

        public XenServerPatch(string uuid, string name, string description, string guidance, string guidance_mandatory, string version, string url,
            string patchUrl, string timestamp, string priority, string installationSize, string downloadSize, string containsLivepatch, List<string> conflictingPatches, List<string> requiredPatches)
            : this(uuid, name, description, guidance, guidance_mandatory, version, url, patchUrl, timestamp, priority, installationSize, downloadSize, containsLivepatch)
        {

            ConflictingPatches = conflictingPatches;
            RequiredPatches = requiredPatches;
        }

        public string Uuid
        {
            get { return _uuid; }
        }

        public bool Equals(XenServerPatch other)
        {
            if (other == null)
                return false;

            return string.Equals(Uuid, other.Uuid, StringComparison.OrdinalIgnoreCase);
        }

        public after_apply_guidance after_apply_guidance
        {
            get
            {
                switch (Guidance)
                {
                    case "restartHVM":
                        return after_apply_guidance.restartHVM;

                    case "restartPV":
                        return after_apply_guidance.restartPV;

                    case "restartHost":
                        return after_apply_guidance.restartHost;

                    case "restartXAPI":
                        return after_apply_guidance.restartXAPI;

                    default:
                        return after_apply_guidance.unknown;
                }
            }
        }
    }
}