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

using XenAPI;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Core;
using System.IO;
using XenAdmin.Diagnostics.Problems.HostProblem;
using System.Collections.Generic;
using XenAdmin.Actions;
using System.Linq;


namespace XenAdmin.Diagnostics.Checks
{
    class DiskSpaceForAutomatedUpdatesCheck : HostPostLivenessCheck
    {
        private readonly Dictionary<Host, List<XenServerPatch>> updateSequence;

        public DiskSpaceForAutomatedUpdatesCheck(Host host, Dictionary<Host, List<XenServerPatch>> updateSequence)
            : base(host)
        {
            this.updateSequence = updateSequence;
        }

        protected override Problem RunHostCheck()
        {
            if (!Host.Connection.IsConnected)
                throw new EndOfStreamException(Helpers.GetName(Host.Connection));

            if (!Helpers.CreamOrGreater(Host.Connection))
                return null;

            var elyOrGreater = Helpers.ElyOrGreater(Host.Connection);

            // check the disk space on host
            var requiredDiskSpace = elyOrGreater
                ? updateSequence[Host].Sum(p => p.InstallationSize) // all updates on this host (for installation)
                : Host.IsMaster()
                    ? updateSequence[Host].Sum(p => p.InstallationSize) + updateSequence.Values.SelectMany(p => p).Distinct().Sum(p => p.InstallationSize) // master: all updates on master (for installation) + all updates in pool (for upload)
                    : updateSequence[Host].Sum(p => p.InstallationSize) * 2; // non-master: all updates on this host x 2 (for installation + upload)

            var action = new GetDiskSpaceRequirementsAction(Host, requiredDiskSpace, true, DiskSpaceRequirements.OperationTypes.automatedUpdates);

            try
            {
                action.RunExternal(action.Session);
            }
            catch
            {
                log.WarnFormat("Could not get disk space requirements");
            }

            if (action.Succeeded && action.DiskSpaceRequirements.AvailableDiskSpace < requiredDiskSpace)
                return new HostOutOfSpaceProblem(this, Host, action.DiskSpaceRequirements);

            // check the disk space for uploading the update files to the pool's SRs (only needs to be done once, so only run this on master)
            if (elyOrGreater && Host.IsMaster())
            {
                var allPatches = updateSequence.Values.SelectMany(p => p).Distinct().ToList();
                var maxFileSize = allPatches.Max(p => p.DownloadSize);

                var availableSRs = Host.Connection.Cache.SRs.Where(sr => sr.SupportsVdiCreate() && !sr.IsDetached()).ToList();
                var maxSrSize = availableSRs.Max(sr => sr.FreeSpace());

                // can accomodate the largest file?
                if (maxSrSize < maxFileSize)
                {
                    return new HostOutOfSpaceProblem(this, Host,
                        new DiskSpaceRequirements(DiskSpaceRequirements.OperationTypes.automatedUpdatesUploadOne, Host, null, maxFileSize, maxSrSize, 0));
                }

                // can accomodate all files?
                var totalFileSize = allPatches.Sum(p => p.DownloadSize);
                var totalAvailableSpace = availableSRs.Sum(sr => sr.FreeSpace());

                if (totalAvailableSpace < totalFileSize)
                {
                    return new HostOutOfSpaceProblem(this, Host,
                        new DiskSpaceRequirements(DiskSpaceRequirements.OperationTypes.automatedUpdatesUploadAll, Host, null, totalFileSize, totalAvailableSpace, 0));
                }
            }

            return null;
        }

        public override string Description
        {
            get { return Messages.SERVER_SIDE_CHECK_AUTO_MODE_DESCRIPTION; }
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
