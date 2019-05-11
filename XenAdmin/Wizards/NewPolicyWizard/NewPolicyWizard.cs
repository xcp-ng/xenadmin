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
using System.Globalization;
using System.Linq;
using System.Text;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Wizards.GenericPages;

namespace XenAdmin.Wizards.NewPolicyWizard
{
    public partial class NewPolicyWizard : XenWizardBase
    {
        private readonly NewPolicyPolicyNamePage xenTabPagePolicy;
        private readonly NewVMGroupVMsPage<VMSS> xenTabPageVMsPage;
        private readonly NewPolicySnapshotTypePage xenTabPageSnapshotType;
        private readonly NewPolicySnapshotFrequencyPage xenTabPageSnapshotFrequency;
        private readonly NewPolicyFinishPage xenTabPageFinish;
        private readonly RBACWarningPage xenTabPageRBAC; 

        public readonly Pool Pool;

        public NewPolicyWizard(Pool pool)
            : base(pool.Connection)
        {
            InitializeComponent();
            Pool = pool;

            this.Text = Messages.VMSS_WIZARD_TITLE;

            xenTabPagePolicy = new NewPolicyPolicyNamePage();
            xenTabPageSnapshotType = new NewPolicySnapshotTypePage();
            xenTabPageVMsPage = new NewVMGroupVMsPage<VMSS>();
            xenTabPageFinish = new NewPolicyFinishPage();
            xenTabPageRBAC = new RBACWarningPage();
            xenTabPageVMsPage.Pool = pool;
            xenTabPageSnapshotFrequency = new NewPolicySnapshotFrequencyPage();
            xenTabPageSnapshotFrequency.Connection = pool.Connection;
            
            #region RBAC Warning Page Checks
            if (Pool.Connection.Session.IsLocalSuperuser || Helpers.GetMaster(Pool.Connection).external_auth_type == Auth.AUTH_TYPE_NONE)
            {
                //do nothing
            }
            else
            {
                RBACWarningPage.WizardPermissionCheck check;
                check = new RBACWarningPage.WizardPermissionCheck(Messages.RBAC_WARNING_VMSS);
                check.AddApiCheck("VMSS.async_create");
                check.Blocking = true;
                xenTabPageRBAC.AddPermissionChecks(xenConnection, check);
                AddPage(xenTabPageRBAC, 0);
            }
            #endregion

            AddPages(xenTabPagePolicy, xenTabPageVMsPage);
            AddPage(xenTabPageSnapshotType);
            AddPages(xenTabPageSnapshotFrequency);
            AddPages(xenTabPageFinish);
        }

        public NewPolicyWizard(Pool pool, List<VM> selection)
            : this(pool)
        {
            this.xenTabPageVMsPage.SelectedVMs = selection;
        }

        protected override void UpdateWizardContent(XenTabPage senderPage)
        {
            var prevPageType = senderPage.GetType();

            if (prevPageType == typeof(NewPolicyPolicyNamePage))
            {
                xenTabPageVMsPage.GroupName = xenTabPagePolicy.PolicyName;
            }
            else
            {
                if (prevPageType == typeof(NewPolicySnapshotFrequencyPage))
                {
                    xenTabPageFinish.Summary = string.Format(Messages.VMSS_POLICY_SUMMARY,
                        xenTabPagePolicy.PolicyName,
                        string.Join(", ", xenTabPageVMsPage.SelectedVMs.Select(vm => vm.Name())),
                        xenTabPageSnapshotType.BackupTypeToString,
                        xenTabPageSnapshotFrequency.FormattedSchedule);
                    xenTabPageFinish.SelectedVMsCount = xenTabPageVMsPage.SelectedVMs.Count;
                }
                else if (prevPageType == typeof(NewVMGroupVMsPage<VMSS>))
                {
                    xenTabPageSnapshotType.SelectedVMs = xenTabPageVMsPage.SelectedVMs;
                }
            }
        }

        protected override void FinishWizard()
        {
             var vmss = new VMSS
                {
                    name_label = xenTabPagePolicy.PolicyName,
                    name_description = xenTabPagePolicy.PolicyDescription,
                    type = xenTabPageSnapshotType.BackupType,
                    frequency = xenTabPageSnapshotFrequency.Frequency,
                    schedule = xenTabPageSnapshotFrequency.Schedule,
                    retained_snapshots = xenTabPageSnapshotFrequency.BackupRetention,
                    enabled = xenTabPageVMsPage.SelectedVMs.Count != 0,
                    Connection = Pool.Connection
                };

            var action = new CreateVMPolicy(vmss, xenTabPageVMsPage.SelectedVMs, xenTabPageFinish.RunNow);

            action.RunAsync();
            base.FinishWizard();
        }

        protected override string WizardPaneHelpID()
        {
            if (CurrentStepTabPage is RBACWarningPage)
            {
                return FormatHelpId("Rbac");
            }

            return "NewPolicyWizardVMSS_" + CurrentStepTabPage.HelpID + "Pane";    
        }
    }
}
