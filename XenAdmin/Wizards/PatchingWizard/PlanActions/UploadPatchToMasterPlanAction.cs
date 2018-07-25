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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;
using System.Linq;
using XenAdmin.Network;


namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    class UploadPatchToMasterPlanAction : PlanActionWithSession
    {
        private readonly XenServerPatch patch;
        private readonly List<PoolPatchMapping> mappings;
        private Dictionary<XenServerPatch, string> AllDownloadedPatches = new Dictionary<XenServerPatch, string>();
        private KeyValuePair<XenServerPatch, string> patchFromDisk;
        private AsyncAction inProgressAction = null;

        public UploadPatchToMasterPlanAction(IXenConnection connection, XenServerPatch patch, List<PoolPatchMapping> mappings, Dictionary<XenServerPatch, string> allDownloadedPatches, KeyValuePair<XenServerPatch, string> patchFromDisk)
            : base(connection)
        {
            this.patch = patch;
            this.mappings = mappings;
            this.AllDownloadedPatches = allDownloadedPatches;
            this.patchFromDisk = patchFromDisk;
        }

        protected override void RunWithSession(ref Session session)
        {
            var path = AllDownloadedPatches.ContainsKey(patch) 
                ? AllDownloadedPatches[patch]
                : patchFromDisk.Key == patch ? patchFromDisk.Value : null;

            var poolPatches = new List<Pool_patch>(session.Connection.Cache.Pool_patches);
            var poolUpdates = new List<Pool_update>(session.Connection.Cache.Pool_updates);

            var conn = session.Connection;
            var master = Helpers.GetMaster(conn);

            var existingMapping = mappings.Find(m => m.MasterHost != null && master != null &&
                                                     m.MasterHost.uuid == master.uuid && (m.Pool_patch != null || m.Pool_update != null) && m.XenServerPatch.Equals(patch));

            if (existingMapping == null
                || !(existingMapping.Pool_patch != null && poolPatches.Any(p => string.Equals(p.uuid, existingMapping.Pool_patch.uuid, StringComparison.OrdinalIgnoreCase)))
                && !(existingMapping.Pool_update != null && poolUpdates.Any(p => string.Equals(p.uuid, existingMapping.Pool_update.uuid, StringComparison.OrdinalIgnoreCase)))
            )
            {
                AddProgressStep(string.Format(Messages.UPDATES_WIZARD_UPLOADING_UPDATE, patch.Name, conn.Name));
                PoolPatchMapping newMapping;

                if (Helpers.ElyOrGreater(master))
                {
                    var uploadIsoAction = new UploadSupplementalPackAction(session.Connection, new List<Host>() {master}, path, true);
                    uploadIsoAction.Changed += uploadAction_Changed;
                    uploadIsoAction.Completed += uploadAction_Completed;
                    inProgressAction = uploadIsoAction;
                    uploadIsoAction.RunExternal(session);

                    var poolupdate = uploadIsoAction.PoolUpdate;

                    if (poolupdate == null)
                    {
                        log.ErrorFormat("Upload finished successfully, but Pool_update object has not been found for update (uuid={0}) on host (uuid={1}).", patch.Uuid, session.Connection);

                        throw new Exception(Messages.ACTION_UPLOADPATCHTOMASTERPLANACTION_FAILED);
                    }

                    newMapping = new PoolPatchMapping(patch, poolupdate, Helpers.GetMaster(session.Connection));
                }
                else
                {
                    var checkSpaceForUpload = new CheckDiskSpaceForPatchUploadAction(Helpers.GetMaster(conn), path, true);
                    inProgressAction = checkSpaceForUpload;
                    checkSpaceForUpload.RunExternal(session);

                    var uploadPatchAction = new UploadPatchAction(session.Connection, path, true, false);
                    uploadPatchAction.Changed += uploadAction_Changed;
                    uploadPatchAction.Completed += uploadAction_Completed;
                    inProgressAction = uploadPatchAction;
                    uploadPatchAction.RunExternal(session);

                    // this has to be run again to refresh poolPatches (to get the recently uploaded one as well)
                    poolPatches = new List<Pool_patch>(session.Connection.Cache.Pool_patches);

                    var poolPatch = poolPatches.Find(p => string.Equals(p.uuid, patch.Uuid, StringComparison.OrdinalIgnoreCase));
                    if (poolPatch == null)
                    {
                        log.ErrorFormat("Upload finished successfully, but Pool_patch object has not been found for patch (uuid={0}) on host (uuid={1}).", patch.Uuid, session.Connection);

                        throw new Exception(Messages.ACTION_UPLOADPATCHTOMASTERPLANACTION_FAILED);
                    }

                    newMapping = new PoolPatchMapping(patch, poolPatch, Helpers.GetMaster(session.Connection));
                }

                if (!mappings.Contains(newMapping))
                    mappings.Add(newMapping);
            }
        }

        private void uploadAction_Changed(ActionBase action)
        {
            if (action == null)
                return;

            if (Cancelling)
                action.Cancel();

            var bpAction = action as IByteProgressAction;
            if (bpAction == null)
                return;

            if (!string.IsNullOrEmpty(bpAction.ByteProgressDescription))
                ReplaceProgressStep(bpAction.ByteProgressDescription);
        }

        private void uploadAction_Completed(ActionBase action)
        {
            if (action == null)
                return;

            action.Changed -= uploadAction_Changed;
            action.Completed -= uploadAction_Completed;
        }

        public override void Cancel()
        {
            if (inProgressAction != null)
                inProgressAction.Cancel();

            base.Cancel();
        }
    }
}
