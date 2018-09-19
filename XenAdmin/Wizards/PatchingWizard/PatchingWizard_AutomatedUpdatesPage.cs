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
using XenAPI;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Alerts;


namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_AutomatedUpdatesPage : AutomatedUpdatesBasePage
    {
        public XenServerPatchAlert UpdateAlert { private get; set; }
        public WizardMode WizardMode { private get; set; }
      
        public KeyValuePair<XenServerPatch, string> PatchFromDisk { private get; set; }

        public PatchingWizard_AutomatedUpdatesPage()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides
        public override string Text
        {
            get
            {
                return Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_TEXT;
            }
        }

        public override string PageTitle
        {
            get
            {
                return Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_TITLE;
            }
        }

        public override string HelpID
        {
            get { return ""; }
        }
        #endregion

        #region AutomatedUpdatesBesePage overrides

        protected override string BlurbText()
        {
            return WizardMode == WizardMode.AutomatedUpdates
                ? Messages.PATCHINGWIZARD_UPLOAD_AND_INSTALL_TITLE_AUTOMATED_MODE
                : Messages.PATCHINGWIZARD_UPLOAD_AND_INSTALL_TITLE_NEW_VERSION_AUTOMATED_MODE;
        }

        protected override string SuccessMessageOnCompletion(bool multiplePools)
        {
            return multiplePools ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_SUCCESS_MANY : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_SUCCESS_ONE;
        }

        protected override string FailureMessageOnCompletion(bool multiplePools)
        {
            return multiplePools ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_MANY : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_ONE;
        }

        protected override string SuccessMessagePerPool(Pool pool)
        {
            return Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_SUCCESS_ONE;
        }

        protected override string FailureMessagePerPool(bool multipleErrors)
        {
            return multipleErrors ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_POOL_MANY : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_POOL_ONE;
        }

        protected override string UserCancellationMessage()
        {
            return Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_CANCELLATION;
        }

        protected override List<HostPlan> GenerateHostPlans(Pool pool, out List<Host> applicableHosts)
        {
            bool automatedUpdatesRestricted = pool.Connection.Cache.Hosts.Any(Host.RestrictBatchHotfixApply);

            var minimalPatches = WizardMode == WizardMode.NewVersion
                ? Updates.GetMinimalPatches(UpdateAlert, ApplyUpdatesToNewVersion && !automatedUpdatesRestricted)
                : Updates.GetMinimalPatches(pool.Connection);

            if (minimalPatches == null)
            {
                applicableHosts = new List<Host>();
                return new List<HostPlan>();
            }

            var uploadedPatches = new List<XenServerPatch>();
            var hosts = pool.Connection.Cache.Hosts.ToList();
            hosts.Sort();//master first

            applicableHosts = new List<Host>(hosts);
            return hosts.Select(h => GetUpdatePlanActionsForHost(h, hosts, minimalPatches, uploadedPatches, PatchFromDisk)).ToList();
        }
        #endregion
    }
}
