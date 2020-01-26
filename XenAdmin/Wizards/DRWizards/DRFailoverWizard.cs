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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Wizards.GenericPages;
using XenAPI;

using XenAdmin.Actions.DR;

namespace XenAdmin.Wizards.DRWizards
{
    public enum DRWizardType { Failover, Failback, Dryrun, Unknown }

    public partial class DRFailoverWizard : XenWizardBase
    {
        private readonly RBACWarningPage RBACWarningPage;
        private readonly DRFailoverWizardFirstPage DRFailoverWizardFirstPage;
        private readonly DRFailoverWizardStoragePage DRFailoverWizardStoragePage1;
        private readonly DRFailoverWizardPrecheckPage DRFailoverWizardPrecheckPage1;
        private readonly DRFailoverWizardRecoverPage DRFailoverWizardRecoverPage1;
        private readonly DRFailoverWizardAppliancesPage DRFailoverWizardAppliancesPage1;
        private readonly DRFailoverWizardWelcomePage DRFailoverWizardWelcomePage;
        private readonly DRFailoverWizardReportPage DRFailoverWizardReportPage1;

        private readonly Pool Pool;
        private DRWizardType WizardType;
        private SummaryReport SummaryReport = new SummaryReport();

        public DRFailoverWizard(Pool pool)
            : this(pool, DRWizardType.Unknown)
        { }
         
        public DRFailoverWizard(Pool pool, DRWizardType wizardType)
            : base(pool.Connection)
        {
            InitializeComponent();

            RBACWarningPage = new RBACWarningPage();
            DRFailoverWizardFirstPage = new DRFailoverWizardFirstPage();
            DRFailoverWizardStoragePage1 = new DRFailoverWizardStoragePage();
            DRFailoverWizardPrecheckPage1 = new DRFailoverWizardPrecheckPage();
            DRFailoverWizardRecoverPage1 = new DRFailoverWizardRecoverPage();
            DRFailoverWizardAppliancesPage1 = new DRFailoverWizardAppliancesPage();
            DRFailoverWizardWelcomePage = new DRFailoverWizardWelcomePage();
            DRFailoverWizardReportPage1 = new DRFailoverWizardReportPage();

            Pool = pool;
            WizardType = wizardType; 

            #region RBAC Warning Page Checks
            if (Pool.Connection.Session.IsLocalSuperuser || Helpers.GetMaster(Pool.Connection).external_auth_type == Auth.AUTH_TYPE_NONE)
            {
            }
            else
            {
                RBACWarningPage.WizardPermissionCheck check = new RBACWarningPage.WizardPermissionCheck(Messages.RBAC_DR_WIZARD_MESSAGE);
                check.AddApiCheck("DR_task.async_create");
                check.Blocking = true;
                RBACWarningPage.AddPermissionChecks(xenConnection, check);
                AddPage(RBACWarningPage, 0);
            }
            #endregion

            DRFailoverWizardReportPage1.SummaryRetreiver = GetSummaryReport;

            DRFailoverWizardWelcomePage.WizardTypeChanged += DRFailoverWizardWelcomePage_WizardTypeChanged;
            DRFailoverWizardWelcomePage.SetWizardType(wizardType);

            DRFailoverWizardRecoverPage1.ReportStarted += DRFailoverWizardRecoverPage1_ReportStarted;
            DRFailoverWizardRecoverPage1.ReportLineGot += DRFailoverWizardRecoverPage1_ReportLineGot;
            DRFailoverWizardRecoverPage1.ReportActionResultGot += DRFailoverWizardRecoverPage1_ReportActionResultGot;

            DRFailoverWizardAppliancesPage1.Pool = pool;
            DRFailoverWizardPrecheckPage1.Pool = pool;

            DRFailoverWizardStoragePage1.NewDrTaskIntroduced += NewDrTaskIntroduced;
            
            DRFailoverWizardPrecheckPage1.NewDrTaskIntroduced += NewDrTaskIntroduced;
            DRFailoverWizardPrecheckPage1.SrIntroduced += DRFailoverWizardPrecheckPage1_SrIntroduced;

            AddPages(DRFailoverWizardWelcomePage, DRFailoverWizardFirstPage, DRFailoverWizardStoragePage1, DRFailoverWizardAppliancesPage1,
                     DRFailoverWizardPrecheckPage1, DRFailoverWizardRecoverPage1, DRFailoverWizardReportPage1);
        }

        protected override string WizardPaneHelpID()
        {
            if (CurrentStepTabPage is RBACWarningPage)
            {
                string helpId;
                switch (WizardType)
                {
                    case DRWizardType.Failover:
                        helpId = "Failover_Rbac";
                        break;
                    case DRWizardType.Failback:
                        helpId = "Failback_Rbac";
                        break;
                    case DRWizardType.Dryrun:
                        helpId = "Dryrun_Rbac";
                        break;
                    default:
                        helpId = "Rbac";
                        break;
                }
                return FormatHelpId(helpId);
            }
            return base.WizardPaneHelpID();
        }

        protected override void UpdateWizardContent(XenTabPage senderPage)
        {
            var prevPageType = senderPage.GetType();

            if (prevPageType == typeof(DRFailoverWizardStoragePage))
                DRFailoverWizardAppliancesPage1.AllPoolMetadata = DRFailoverWizardStoragePage1.AllPoolMetadata;
            else if (prevPageType == typeof(DRFailoverWizardAppliancesPage))
            {
                DRFailoverWizardRecoverPage1.StartActionAfterRecovery = DRFailoverWizardAppliancesPage1.StartActionAfterRecovery;
                DRFailoverWizardPrecheckPage1.SelectedPoolMetadata = DRFailoverWizardAppliancesPage1.SelectedPoolMetadata;
                DRFailoverWizardRecoverPage1.SelectedPoolMetadata = DRFailoverWizardAppliancesPage1.SelectedPoolMetadata;

            }
            else if (prevPageType == typeof(DRFailoverWizardRecoverPage))
                DoFinalCleanup();
        }

        private List<DR_task> DrTasks = new List<DR_task>();
        private List<XenRef<SR>> IntroducedSrs = new List<XenRef<SR>>();

        private void NewDrTask(string opaqueRef)
        {
            DR_task drTask = DR_task.get_record(Pool.Connection.Session, opaqueRef);
            drTask.opaque_ref = opaqueRef;
            DrTasks.Add(drTask);
            IntroducedSrs.AddRange(drTask.introduced_SRs);
        }

        private bool cleanupExecuted = false;
        private void DoFinalCleanup()
        {
            if (cleanupExecuted)
                return;

            SummaryReport.AddLine("");
         
            // destroy DR tasks
            DestroyDrTasks();

            // dry-run clean-up
            if (WizardType == DRWizardType.Dryrun)
            {
                DoDryRunCleanup();
            }

            // revert pre-check resolved problems
            if (DRFailoverWizardPrecheckPage1.RevertActions.Count > 0)
            {
                SummaryReport.AddLine(Messages.REVERT_PRECHECK_ACTIONS);
                MultipleAction action = new MultipleAction(xenConnection, Messages.REVERT_PRECHECK_ACTIONS, "", "", DRFailoverWizardPrecheckPage1.RevertActions);
                new Dialogs.ActionProgressDialog(action, ProgressBarStyle.Blocks).ShowDialog();
                foreach (var subAction in DRFailoverWizardPrecheckPage1.RevertActions)
                    SummaryReport.AddActionResult(subAction);
            }
            cleanupExecuted = true;
        }

        private void DestroyDrTasks()
        {
            // destroy DR tasks
            if (DrTasks.Count > 0)
                SummaryReport.AddLine(Messages.DR_WIZARD_REPORT_DR_CLEANUP);

            foreach (var drTask in DrTasks.ToList())
            {
                List<SR> srs = Pool.Connection.ResolveAll(drTask.introduced_SRs);
                string srNames = string.Join(", ", (from sr in srs select sr.Name()).ToArray());
                
                DR_task task = drTask;
                var action = new DelegatedAsyncAction(xenConnection,
                    string.Format(Messages.ACTION_DR_TASK_DESTROY_TITLE, srNames) , Messages.ACTION_DR_TASK_DESTROY_STATUS, Messages.ACTION_DR_TASK_DESTROY_DONE,   
                    s => DR_task.destroy(s, task.opaque_ref)) { Pool = this.Pool };
                new Dialogs.ActionProgressDialog(action, ProgressBarStyle.Blocks).ShowDialog();
                
                if (action.Succeeded)
                    DrTasks.Remove(drTask);
                
                SummaryReport.AddActionResult(action);
            }
        }

        private void DoDryRunCleanup()
        {
            SummaryReport.AddLine(Messages.DR_WIZARD_REPORT_DRYRUN_CLEANUP); 
            List<VM> recoveredVms = Pool.Connection.Cache.VMs.Where(vm => DRFailoverWizardRecoverPage1.RecoveredVmsUuids.Contains(vm.uuid)).ToList();
            List<VM_appliance> recoveredAppliances = Pool.Connection.Cache.VM_appliances.Where(appliance => DRFailoverWizardRecoverPage1.RecoveredVmAppliancesUuids.Contains(appliance.uuid)).ToList();

            // call hard_shutdown on the recovered VMs and delete the recovered VMs
            if (recoveredVms.Count > 0)
                ShutdownAndDestroyVMs(recoveredVms);

            // call hard_shutdown on the recovered appliances and delete the recovered appliances
            if (recoveredAppliances.Count > 0)
                ShutdownAndDestroyAppliances(recoveredAppliances);

            // detach and forget SRs
            DetachAndForgetSRs();
        }

        private void ShutdownAndDestroyVMs(List<VM> recoveredVMs)
        {
            var action = new ShutdownAndDestroyVMsAction(Pool.Connection, recoveredVMs);
            new Dialogs.ActionProgressDialog(action, ProgressBarStyle.Blocks).ShowDialog();
            SummaryReport.AddActionResult(action);
        }

        private void ShutdownAndDestroyAppliances(List<VM_appliance> recoveredAppliances)
        {
            var action = new ShutdownAndDestroyVmAppliancesAction(Pool.Connection, recoveredAppliances);
            new Dialogs.ActionProgressDialog(action, ProgressBarStyle.Blocks).ShowDialog();
            SummaryReport.AddActionResult(action);
        }

        private void DetachAndForgetSRs()
        {
            if (IntroducedSrs.Count == 0)
                return;

            List<SR> srs = Pool.Connection.ResolveAll(IntroducedSrs);

            List<AsyncAction> actions = new List<AsyncAction>();
            foreach (SR sr in srs)
            {
                actions.Add(new SrAction(SrActionKind.Forget, sr));
            }

            if (actions.Count == 0)
                return;

            var action = new MultipleAction(Pool.Connection, Messages.ACTION_SRS_FORGETTING, Messages.FORGETTING_SRS, Messages.SRS_FORGOTTEN, actions);
            new Dialogs.ActionProgressDialog(action, ProgressBarStyle.Blocks).ShowDialog();
            foreach (var subAction in actions)
            {
                SummaryReport.AddActionResult(subAction);
            }
        }

        private void StartSummaryReport()
        {
            // wizard type
            SummaryReport.AddLine(Text);

            // selected SRs
            SummaryReport.AddLine(Messages.DR_WIZARD_REPORT_SELECTED_SRS);
            foreach (var srName in DRFailoverWizardStoragePage1.GetSelectedSRsNames())
            {
                SummaryReport.AddLine(srName, 1);
            }

            // selected VMs and appliances
            SummaryReport.AddLine(Messages.DR_WIZARD_REPORT_SELECTED_METADATA);
            foreach (var poolMetadata in DRFailoverWizardAppliancesPage1.SelectedPoolMetadata.Values)
            {
                SummaryReport.AddLine(poolMetadata.Pool.Name(), 1);
                foreach (var vmAppliance in poolMetadata.VmAppliances.Values)
                {
                    SummaryReport.AddLine(vmAppliance.Name(), 2);
                    foreach (XenRef<VM> vmRef in vmAppliance.VMs)
                    {
                        if (poolMetadata.Vms.ContainsKey(vmRef))
                        {
                            SummaryReport.AddLine(poolMetadata.Vms[vmRef].Name(), 3);
                        }
                    }
                }
                foreach (var vm in poolMetadata.Vms.Values)
                {
                    if (vm.IsAssignedToVapp())
                        continue;
                    SummaryReport.AddLine(vm.Name(), 2);
                }
            }

            // power state after recovery
            SummaryReport.AddLine(String.Format(Messages.DR_WIZARD_REPORT_SELECTED_POWER_STATE, DRFailoverWizardAppliancesPage1.GetSelectedPowerStateDescription()));

            // introduced SRs
            if (IntroducedSrs.Count > 0)
            {
                List<SR> srs = Pool.Connection.ResolveAll(IntroducedSrs);
                string srNames = string.Join(", ", (from sr in srs select sr.Name()).ToArray());
                SummaryReport.AddLine(string.Format(Messages.DR_WIZARD_REPORT_INTRODUCED_SRS, srNames));
            }
            else
            {
                SummaryReport.AddLine(Messages.DR_WIZARD_REPORT_INTRODUCED_SRS_NONE);
            }

            // Pre-check warnings
            List<string> warnings = DRFailoverWizardPrecheckPage1.GetWarnings();
            if (warnings.Count > 0)
            {
                SummaryReport.AddLine(Messages.DR_WIZARD_REPORT_PRECHECK_WARNINGS);
                foreach (var warning in warnings)
                    SummaryReport.AddLine(warning, 1);
            }
            else
            {
                SummaryReport.AddLine(Messages.DR_WIZARD_REPORT_PRECHECK_WARNINGS_NONE);
            }

            SummaryReport.AddLine("");
            SummaryReport.AddLine(Messages.DR_WIZARD_REPORT_RECOVERY_STARTED, 0, true);
        }

        private string GetSummaryReport()
        {
            return SummaryReport.ToString();
        }

        protected override void OnCancel(ref bool cancel)
        {
            base.OnCancel(ref cancel);
            if (cancel)
                return;
            DoFinalCleanup();
        }

        #region Page event handlers

        private void DRFailoverWizardWelcomePage_WizardTypeChanged(DRWizardType type)
        {
            WizardType = type;

            switch (type)
            {
                case DRWizardType.Failover:
                    Text = string.Format(Messages.DR_WIZARD_FAILOVER_TITLE, xenConnection.Name);
                    pictureBoxWizard.Image = Properties.Resources._000_Failover_h32bit_32;
                    break;
                case DRWizardType.Failback:
                    Text = string.Format(Messages.DR_WIZARD_FAILBACK_TITLE, xenConnection.Name);
                    pictureBoxWizard.Image = Properties.Resources._000_Failback_h32bit_32;
                    break;
                case DRWizardType.Dryrun:
                    Text = string.Format(Messages.DR_WIZARD_DRYRUN_TITLE, xenConnection.Name);
                    pictureBoxWizard.Image = Properties.Resources._000_TestFailover_h32bit_32;
                    break;
                default:
                    pictureBoxWizard.Image = Properties.Resources._000_DisasterRecovery_h32bit_32;
                    break;
            }

            DRFailoverWizardReportPage1.WizardType = WizardType;
            DRFailoverWizardStoragePage1.WizardType = WizardType;
            DRFailoverWizardAppliancesPage1.WizardType = WizardType;
            DRFailoverWizardRecoverPage1.WizardType = WizardType;
            DRFailoverWizardFirstPage.WizardType = WizardType;
            DRFailoverWizardPrecheckPage1.WizardType = WizardType;
        }

        private void NewDrTaskIntroduced(string opaqueRef)
        {
            NewDrTask(opaqueRef);
        }

        private void DRFailoverWizardPrecheckPage1_SrIntroduced(XenRef<SR> srRef)
        {
            IntroducedSrs.Add(srRef);
        }

        private void DRFailoverWizardRecoverPage1_ReportStarted()
        {
            StartSummaryReport();
        }

        private void DRFailoverWizardRecoverPage1_ReportActionResultGot(AsyncAction action)
        {
            SummaryReport.AddActionResult(action);
        }

        private void DRFailoverWizardRecoverPage1_ReportLineGot(string text, int indent, bool timeStamp)
        {
            SummaryReport.AddLine(text, indent, timeStamp);
        }

        #endregion
    }

    public class SummaryReport
    {
        private StringBuilder report;
        public SummaryReport()
        {
            report = new StringBuilder();
        }

        public void AddLine(string line, int indent, bool timeStamp)
        {
            const string space = "  ";
            const string bullet = "\u2022";

            var indentText = "";
            if (indent > 0)
            {
                // add space
                for (var i = 0; i < indent; i++)
                    indentText = String.Format("{0}{1}", space, indentText);
                // add bullet
                indentText = String.Format("{0}{1} ", indentText, bullet);
            }
            report.AppendLine(timeStamp
                                  ? String.Format("{0} - {1}{2}", HelpersGUI.DateTimeToString(DateTime.Now, Messages.DATEFORMAT_DMY_HMS, true), indentText, line)
                                  : String.Format("{0}{1}", indentText, line));
        }

        public void AddLine(string line, int indent)
        {
            AddLine(line, indent, false);
        }

        public void AddLine(string line)
        {
            AddLine(line, 0, false);
        }

        public void AddActionResult(AsyncAction action)
        {
            AddLine(action.Succeeded
                                  ? String.Format(Messages.DR_WIZARD_REPORT_ACTION_SUCCEEDED, action.Title)
                                  : String.Format(Messages.DR_WIZARD_REPORT_ACTION_FAILED, action.Title, action.Exception.Message), 1);
        }

        public override string  ToString()
        {
 	         return report.ToString();
        }
    }
    
}
