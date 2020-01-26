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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Alerts;
using XenAdmin.Controls;
using XenAdmin.Wizards.PatchingWizard.PlanActions;


namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_UploadPage : AutomatedUpdatesBasePage
    {
        public PatchingWizard_UploadPage()
        {
            InitializeComponent();
        }

        #region Accessors
        public readonly List<HostUpdateMapping> PatchMappings = new List<HostUpdateMapping>();

        public UpdateType SelectedUpdateType { private get; set; }
        public string SelectedPatchFilePath { get; set; }
        public XenServerPatchAlert SelectedUpdateAlert { private get; set; }
        public KeyValuePair<XenServerPatch, string> PatchFromDisk { private get; set; }
        public List<Host> SelectedServers { private get; set; }

        public Pool_patch Patch
        {
            get
            {
                if (SelectedUpdateAlert != null)
                    return (from HostUpdateMapping mapping in PatchMappings
                        let m = mapping as PoolPatchMapping
                        where m != null && m.XenServerPatch.Equals(SelectedUpdateAlert.Patch)
                        select m.Pool_patch).FirstOrDefault();

                if (SelectedPatchFilePath != null)
                    return (from HostUpdateMapping mapping in PatchMappings
                        let m = mapping as OtherLegacyMapping
                        where m != null && m.Path.Equals(SelectedPatchFilePath)
                        select m.Pool_patch).FirstOrDefault();

                return null;
            }
        }

        public Pool_update PoolUpdate
        {
            get
            {
                foreach (var mapping in PatchMappings)
                {
                    if (mapping is PoolUpdateMapping updateMapping &&
                        SelectedUpdateAlert != null && updateMapping.XenServerPatch.Equals(SelectedUpdateAlert.Patch))
                        return updateMapping.Pool_update;
                    
                    if (mapping is SuppPackMapping suppPackMapping && suppPackMapping.Path.Equals(SelectedPatchFilePath))
                        return suppPackMapping.Pool_update;
                }

                return null;
            }
        }

        public Dictionary<Host, VDI> SuppPackVdis
        {
            get
            {
                if (string.IsNullOrEmpty(SelectedPatchFilePath))
                    return null;

                var suppPackVdis = new Dictionary<Host, VDI>();
                foreach (var mapping in PatchMappings)
                {
                    if (mapping is SuppPackMapping m && m.Path.Equals(SelectedPatchFilePath))
                    {
                        foreach (var kvp in m.SuppPackVdis)
                            suppPackVdis[kvp.Key] = kvp.Value;
                    }
                }

                return suppPackVdis;
            }
        }
        #endregion


        #region XenTabPage overrides

        public override string Text { get { return Messages.PATCHINGWIZARD_UPLOADPAGE_TEXT; } }

        public override string PageTitle
        {
            get { return Messages.PATCHINGWIZARD_UPLOADPAGE_TITLE_ONLY_UPLOAD; }
        }

        public override string HelpID { get { return "UploadPatch"; } }

        public override bool EnableNext()
        {
            return IsSuccess;
        }

        public override bool EnableCancel()
        {
            return true;
        }

        public override bool EnablePrevious()
        {
            return _thisPageIsCompleted;
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Back)
                _thisPageIsCompleted = false;
        }

        #endregion

       
        protected override string BlurbText()
        {
            return Messages.PATCHINGWIZARD_SINGLEUPLOAD_TITLE;
        }

        protected override string SuccessMessageOnCompletion(bool multiplePools)
        {
            var msg = multiplePools
                ? Messages.PATCHINGWIZARD_SINGLEUPLOAD_SUCCESS_MANY
                : Messages.PATCHINGWIZARD_SINGLEUPLOAD_SUCCESS_ONE;
            return string.Format(msg, GetUpdateName());
        }

        protected override string FailureMessageOnCompletion(bool multiplePools)
        {
            var msg = multiplePools
                ? Messages.PATCHINGWIZARD_SINGLEUPLOAD_FAILURE_MANY
                : Messages.PATCHINGWIZARD_SINGLEUPLOAD_FAILURE_ONE;
            return string.Format(msg, GetUpdateName());
        }

        protected override string WarningMessageOnCompletion(bool multiplePools)
        {
            var msg = multiplePools
                ? Messages.PATCHINGWIZARD_SINGLEUPLOAD_WARNING_MANY
                : Messages.PATCHINGWIZARD_SINGLEUPLOAD_WARNING_ONE;
            return string.Format(msg, GetUpdateName());
        }

        protected override string SuccessMessagePerPool(Pool pool)
        {
            return string.Format( Messages.PATCHINGWIZARD_SINGLEUPLOAD_SUCCESS_ONE, GetUpdateName());
        }

        protected override string FailureMessagePerPool(bool multipleErrors)
        {
            var msg = multipleErrors
                ? Messages.PATCHINGWIZARD_SINGLEUPLOAD_FAILURE_PER_POOL_MANY
                : Messages.PATCHINGWIZARD_SINGLEUPLOAD_FAILURE_PER_POOL_ONE;
            return string.Format(msg, GetUpdateName());
        }

        protected override string WarningMessagePerPool(Pool pool)
        {
            return null;
        }

        protected override string UserCancellationMessage()
        {
            return Messages.PATCHINGWIZARD_SINGLEUPLOAD_CANCELLATION;
        }

        protected override string ReconsiderCancellationMessage()
        {
            return Messages.PATCHINGWIZARD_SINGLEUPLOAD_CANCELLATION_RECONSIDER;
        }

        protected override List<HostPlan> GenerateHostPlans(Pool pool, out List<Host> applicableHosts)
        {
            var master = Helpers.GetMaster(pool);
            var planActions = new List<PlanAction>();

            var alertPatch = SelectedUpdateAlert == null ? null : SelectedUpdateAlert.Patch;

            bool download = alertPatch != null && PatchFromDisk.Key == null &&
                            (!AllDownloadedPatches.ContainsKey(alertPatch) || !File.Exists(AllDownloadedPatches[alertPatch]));

            if (download)
                planActions.Add(new DownloadPatchPlanAction(master.Connection, alertPatch, AllDownloadedPatches, PatchFromDisk));

            var skipDiskSpaceCheck = SelectedUpdateType != UpdateType.Legacy ||
                                     Helpers.ElyOrGreater(master.Connection); //this is superfluous; just added for completeness

            if (alertPatch != null)
            {
                planActions.Add(new UploadPatchToMasterPlanAction(this, master.Connection, alertPatch,
                    PatchMappings, AllDownloadedPatches, PatchFromDisk, skipDiskSpaceCheck));

            }
            else if (!string.IsNullOrEmpty(SelectedPatchFilePath))
            {
                var servers = SelectedServers.Where(s => s.Connection == master.Connection).ToList();
                planActions.Add(new UploadPatchToMasterPlanAction(this, master.Connection, servers, SelectedPatchFilePath,
                    SelectedUpdateType, PatchMappings, skipDiskSpaceCheck));
            }

            var hostPlan = new HostPlan(master, null, planActions, null);
            applicableHosts = new List<Host> {master};
            return new List<HostPlan> {hostPlan};
        }

        private string GetUpdateName()
        {
            if (SelectedUpdateAlert != null)
                return SelectedUpdateAlert.Patch.Name;

            try
            {
                return Path.GetFileName(SelectedPatchFilePath);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}