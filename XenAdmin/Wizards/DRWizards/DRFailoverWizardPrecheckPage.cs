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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Diagnostics.Checks.DR;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Dialogs;
using XenAPI;

using XenAdmin.Actions.DR;

namespace XenAdmin.Wizards.DRWizards
{
    public partial class DRFailoverWizardPrecheckPage : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public event Action<string> NewDrTaskIntroduced;
        public event Action<XenRef<SR>> SrIntroduced;

        private Pool _pool;
        public Pool Pool { set { _pool = value; } }

        private Dictionary<XenRef<VDI>, PoolMetadata> selectedPoolMetadata;
        public Dictionary<XenRef<VDI>, PoolMetadata> SelectedPoolMetadata {set { selectedPoolMetadata = value; }}

        public List<AsyncAction> RevertActions = new List<AsyncAction>();

        private string _nextButtonText = Messages.WIZARD_BUTTON_NEXT;
        
        public override string NextText(bool isLastPage)
        {
            return _nextButtonText;
        }

        public DRFailoverWizardPrecheckPage()
        {
            InitializeComponent();
            dataGridView1.BackgroundColor = dataGridView1.DefaultCellStyle.BackColor;
        }

        public override string PageTitle
        {
            get
            {
                switch (WizardType)
                {
                    case DRWizardType.Failback:
                        return Messages.DR_WIZARD_PRECHECKPAGE_TITLE_FAILBACK;
                    default:
                        return Messages.DR_WIZARD_PRECHECKPAGE_TITLE_FAILOVER;
                } 
            }
        }

        public override string Text
        {
            get
            {
                return Messages.DR_WIZARD_PRECHECKPAGE_TEXT;
            }
        }

        public override string HelpID
        {
            get 
            {
                switch (WizardType)
                {
                    case DRWizardType.Failback:
                        return "Failback_Prechecks";
                    case DRWizardType.Dryrun:
                        return "Dryrun_Prechecks";
                    default:
                        return "Failover_Prechecks";
                }
            }
        }

        public override bool EnablePrevious()
        {
            return _worker != null && !_worker.IsBusy;
        }

        private int PercentageSelectedObjects(int i)
        {
            int count = 0;
            foreach (var poolMetadata in selectedPoolMetadata.Values)
            {
                count += (poolMetadata.VmAppliances.Count + poolMetadata.Vms.Count);
            }
            return (int)(((float)i / (float)(count)) * 100);
        }

        private BackgroundWorker _worker = null;

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            SetupLabels();

            try
            {
                if (direction == PageLoadedDirection.Back)
                    return;

                RefreshRechecks();
            }
            catch (Exception e)
            {
                log.Error(e, e);
                throw;//better throw an exception rather than closing the wizard suddenly and silently
            }
        }

        public DRWizardType WizardType { private get; set; }

        private void SetupLabels()
        {
            switch (WizardType)
            {
                case DRWizardType.Failback:
                    labelContinue.Text = Messages.DR_WIZARD_PRECHECKPAGE_CONTINUE_FAILBACK;
                    _nextButtonText = Messages.DR_WIZARD_PRECHECKPAGE_NEXT_FAILBACK;
                    break;
                case DRWizardType.Dryrun:
                    labelContinue.Text = Messages.DR_WIZARD_PRECHECKPAGE_CONTINUE_DRYRUN;
                    _nextButtonText = Messages.DR_WIZARD_PRECHECKPAGE_NEXT_FAILOVER;
                    break;
                default:
                    labelContinue.Text = Messages.DR_WIZARD_PRECHECKPAGE_CONTINUE_FAILOVER;
                    _nextButtonText = Messages.DR_WIZARD_PRECHECKPAGE_NEXT_FAILOVER;
                    break;
            } 
        }

        protected void RefreshRechecks()
        {
            buttonResolveAll.Enabled = buttonReCheckProblems.Enabled = false;
            _worker = null;
            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            _worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
            _worker.RunWorkerAsync();
        }

        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
                OnPageUpdated();
            progressBar1.Value = 100;
            bool problemsFound = false;
            foreach (PreCheckGridRow row in dataGridView1.Rows)
            {
                PreCheckItemRow preCheckRow = row as PreCheckItemRow;
                if (preCheckRow != null && preCheckRow.Problem != null)
                {
                    problemsFound = true;
                    break;
                }
            }
            buttonResolveAll.Enabled = problemsFound;
            buttonReCheckProblems.Enabled = true;
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                var row = e.UserState as DataGridViewRow;
                if (row != null && !dataGridView1.Rows.Contains(row))
                {
                    PreCheckItemRow preCheckRow = row as PreCheckItemRow;
                    if (preCheckRow != null && preCheckRow.IsProblem)
                        preCheckRow.Visible = true;
                    else
                        row.Visible = !checkBoxViewPrecheckFailuresOnly.Checked;
                    dataGridView1.Rows.Add(row);
                }

                int step = (int)((1.0 / ((float)_numberChecks)) * e.ProgressPercentage);
                progressBar1.Value += (step + progressBar1.Value) > 100 ? 0 : step;
            }
            catch (Exception) { }
        }

        private DataGridViewRow ExecuteCheck(Check check)
        {
            var problems = check.RunAllChecks();
            if (problems.Count != 0)
            {
                // None of the checks used in the DR wizard returns more than one problem, so we just take the first.
                // (Even if they did, we would only suffer the usability problem described in CA-77990).
                return new PreCheckItemRow(problems[0]);
            } 
            return new PreCheckItemRow(check);
        }

        private int _numberChecks = 0;
        private readonly object _lock = new object();
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (_lock)
            {
                Program.Invoke(this, () =>
                                         {
                                             dataGridView1.Rows.Clear();
                                             progressBar1.Value = 0;
                                         });

                RevertActions.Clear();
                List<KeyValuePair<string, List<Check>>> checks = GenerateChecks();
                _numberChecks = checks.Count;
                for (int i = 0; i < checks.Count; i++)
                {
                    List<Check> checkGroup = checks[i].Value;
                    DataGridViewRow row =
                        new PreCheckHeaderRow(string.Format(Messages.DR_WIZARD_PRECHECKPAGE_HEADER, i + 1,
                                                            checks.Count, checks[i].Key));
                    _worker.ReportProgress(5, row);

                    Session metadataSession = null;

                    // execute checks
                    for (int j = 0; j < checkGroup.Count; j++)
                    {
                        if (_worker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                        Check check = checkGroup[j];

                        // special case - AssertCanBeRecoveredCheck - we need to open the metadata database
                        if (check is AssertCanBeRecoveredCheck)
                        {
                            AssertCanBeRecoveredCheck thisCheck = check as AssertCanBeRecoveredCheck;
                            AssertCanBeRecoveredCheck prevCheck = j > 0
                                                                      ? checkGroup[j - 1] as
                                                                        AssertCanBeRecoveredCheck
                                                                      : null;
                            if (prevCheck == null || prevCheck.Vdi.uuid != thisCheck.Vdi.uuid)
                            {
                                // close previous metadata session
                                if (metadataSession != null)
                                    metadataSession.logout();

                                // open metadata database
                                VdiOpenDatabaseAction action = new VdiOpenDatabaseAction(Connection,
                                                                                         ((AssertCanBeRecoveredCheck
                                                                                          )checkGroup[0]).Vdi);
                                action.RunExternal(action.Session);
                                if (action.Succeeded && action.MetadataSession != null)
                                    metadataSession = action.MetadataSession;
                            }

                            // run check
                            if (metadataSession != null)
                            {
                                thisCheck.MetadataSession = metadataSession;
                                row = ExecuteCheck(thisCheck);
                                _worker.ReportProgress(PercentageSelectedObjects(j + 1), row);

                                // close metadata session if this is the last check
                                if (j == checkGroup.Count - 1)
                                    metadataSession.logout();
                            }
                        }
                        else
                        {
                            row = ExecuteCheck(check);
                            _worker.ReportProgress(PercentageSelectedObjects(j + 1), row);
                        }
                    }
                }
            }
        }

        private List<KeyValuePair<string, List<Check>>> GenerateChecks()
        {
            List<KeyValuePair<string, List<Check>>> checks = new List<KeyValuePair<string, List<Check>>>();
            List<Check> checkGroup;

            //HA checks
            if (WizardType == DRWizardType.Dryrun)
            {
                checks.Add(new KeyValuePair<string, List<Check>>(Messages.DR_WIZARD_CHECKING_HA_STATUS,
                                                                 new List<Check>()));
                checkGroup = checks[checks.Count - 1].Value;
                checkGroup.Add(new DrHAEnabledCheck(_pool));
            }

            // check if selected VMs and appliances are still running in the source pool
            if (WizardType != DRWizardType.Dryrun)
            {
                checkGroup = new List<Check>();
                foreach (PoolMetadata poolMetadata in selectedPoolMetadata.Values)
                {
                    // is source pool connected?
                    PoolMetadata metadata = poolMetadata;
                    Pool sourcePool = ConnectionsManager.XenConnectionsCopy.Select
                        (connection => Helpers.GetPoolOfOne(connection)).FirstOrDefault
                        (pool => pool != null && pool.uuid == metadata.Pool.uuid);
                    if (sourcePool != null)
                    {
                        foreach (VM_appliance vmAppliance in poolMetadata.VmAppliances.Values)
                        {
                            checkGroup.Add(new RunningVmApplianceCheck(vmAppliance, sourcePool));
                        }
                        
                        foreach (VM vm in poolMetadata.Vms.Values)
                        {
                            if (vm.IsAssignedToVapp())
                            {
                                //VM included in an appliance
                                continue;
                            }
                            checkGroup.Add(new RunningVmCheck(vm, sourcePool));
                        }
                    }
                }
                if (checkGroup.Count > 0)
                    checks.Add(
                        new KeyValuePair<string, List<Check>>(
                            Messages.DR_WIZARD_CHECKING_POWER_STATE_IN_SOURCE_POOLS, checkGroup));
            }

            //Existing VMs and appliances checks
            checkGroup = new List<Check>();
            foreach (PoolMetadata poolMetadata in selectedPoolMetadata.Values)
            {
                foreach (VM_appliance vmAppliance in poolMetadata.VmAppliances.Values)
                {
                    PoolMetadata metadata = poolMetadata;
                    List<VM> vms = (from vmRef in vmAppliance.VMs
                                    where metadata.Vms.ContainsKey(vmRef)
                                    select metadata.Vms[vmRef]).ToList();
                    checkGroup.Add(new ExistingVmApplianceCheck(vmAppliance, vms, _pool));
                }
                
                foreach (VM vm in poolMetadata.Vms.Values)
                {
                    if (vm.IsAssignedToVapp())
                    {
                        //VM included in an appliance
                        continue;
                    }
                    checkGroup.Add(new ExistingVmCheck(vm, _pool));
                }
            }
            if (checkGroup.Count > 0)
                checks.Add(new KeyValuePair<string, List<Check>>(String.Format(Messages.DR_WIZARD_CHECKING_EXISTING_APPLIANCES_AND_VMS, _pool.Name()), checkGroup));

        
            //VM and appliance can be recovered checks
            checks.Add(new KeyValuePair<string, List<Check>>(String.Format(Messages.DR_WIZARD_CHECKING_VMS_CAN_BE_RECOVERED, _pool.Name()), new List<Check>()));
            checkGroup = checks[checks.Count - 1].Value;
            foreach (PoolMetadata poolMetadata in selectedPoolMetadata.Values)
            {
                foreach (VM_appliance vmAppliance in poolMetadata.VmAppliances.Values)
                {
                    checkGroup.Add(new AssertCanBeRecoveredCheck(vmAppliance, _pool, poolMetadata.Vdi));
                }    
                
                foreach (VM vm in poolMetadata.Vms.Values)
                {
                    if (vm.IsAssignedToVapp())
                    {
                        //VM included in an appliance
                        continue;
                    }
                    checkGroup.Add(new AssertCanBeRecoveredCheck(vm, _pool, poolMetadata.Vdi));
                }
            }
            return checks;
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Back)
            {
                _worker.CancelAsync();
                _worker = null;
            }
        }

        public override bool EnableNext()
        {
            int problemsCount = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                PreCheckItemRow preCheckRow = row as PreCheckItemRow;
                if (preCheckRow != null && preCheckRow.IsProblem)
                {
                    problemsCount++;
                }

            }

            if (_worker != null && _worker.IsBusy)
                labelPrecheckStatus.Text = Messages.DR_WIZARD_PRECHECKPAGE_STATUS_RUNNING;
            else
            {
                labelPrecheckStatus.Text = problemsCount > 0
                                               ? string.Format(Messages.DR_WIZARD_PRECHECKPAGE_STATUS_FAILURE, problemsCount)
                                               : Messages.DR_WIZARD_PRECHECKPAGE_STATUS_SUCCESS;
            }

            bool result = _worker != null && !_worker.IsBusy && problemsCount==0;
            panelErrorsFound.Visible = !result;
            labelContinue.Visible = result;

            return result;
        }

        public override void PageCancelled(ref bool cancel)
        {
            if (_worker != null)
                _worker.CancelAsync();
        }

        internal List<string> GetWarnings()
        {
            var warnings = new List<string>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var preCheckRow = row as PreCheckItemRow;
                if (preCheckRow != null && preCheckRow.IsWarning)
                {
                    warnings.Add(preCheckRow.Description);
                }
            }
            return warnings;
        }


        private void ExecuteSolution(PreCheckItemRow preCheckRow)
        {
            bool cancelled;
            AsyncAction action = preCheckRow.Problem.GetSolutionAction(out cancelled);
            if (action != null)
            {
                action.Completed += action_Completed;
                using (var progressDialog = new ActionProgressDialog(action, ProgressBarStyle.Blocks))
                    progressDialog.ShowDialog(this);
                if (action.Succeeded)
                {
                    var revertAction = preCheckRow.Problem.CreateUnwindChangesAction();
                    if (revertAction != null)
                        RevertActions.Add(revertAction);
                }
            }
        }

        private void action_Completed(ActionBase sender)
        {
            Thread.Sleep(1000);
            Program.Invoke(Program.MainWindow, RefreshRechecks);

            var drTaskCreateAction = sender as DrTaskCreateAction;
            if (drTaskCreateAction != null && drTaskCreateAction.Succeeded)
            {
                if (NewDrTaskIntroduced != null)
                    NewDrTaskIntroduced(drTaskCreateAction.Result);
                return;
            }

            var srIntroduceAction = sender as SrIntroduceAction;
            if (srIntroduceAction != null && srIntroduceAction.Succeeded)
            {
                if (SrIntroduced != null)
                    SrIntroduced(new XenRef<SR>(srIntroduceAction.Result));
            }
        }

        private void buttonReCheckProblems_Click(object sender, EventArgs e)
        {
            RefreshRechecks();
        }

        private void buttonResolveAll_Click(object sender, EventArgs e)
        {
            Dictionary<AsyncAction, AsyncAction> actions = new Dictionary<AsyncAction, AsyncAction>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                PreCheckItemRow preCheckRow = row as PreCheckItemRow;
                if (preCheckRow != null && preCheckRow.Problem != null)
                {
                    bool cancelled;
                    AsyncAction action = preCheckRow.Problem.GetSolutionAction(out cancelled);
                    if (action != null)
                    {
                        actions.Add(action, preCheckRow.Problem.CreateUnwindChangesAction());
                    }
                }
            }
            foreach (var asyncAction in actions.Keys)
            {
                using (var progressDialog = new ActionProgressDialog(asyncAction, ProgressBarStyle.Blocks))
                    progressDialog.ShowDialog(this);

                if (asyncAction.Succeeded && actions[asyncAction] != null)
                    RevertActions.Add(actions[asyncAction]);
                Program.Invoke(Program.MainWindow, RefreshRechecks);
            }
            Program.Invoke(Program.MainWindow, RefreshRechecks);
        }

        private void checkBoxViewPrecheckFailuresOnly_CheckedChanged(object sender, EventArgs e)
        {
            //RefreshRechecks();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var preCheckRow = row as PreCheckItemRow;
                if (preCheckRow != null && preCheckRow.IsProblem)
                    preCheckRow.Visible = true;
                else
                    row.Visible = !checkBoxViewPrecheckFailuresOnly.Checked;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            PreCheckItemRow preChecRow = dataGridView1.Rows[e.RowIndex] as PreCheckItemRow;
            if (preChecRow != null && e.ColumnIndex == 2)
            {
                ExecuteSolution(preChecRow);
                return;
            }
        } 

        #region PreCheckGridRow
        private abstract class PreCheckGridRow : DataGridViewRow
        {           
            protected DataGridViewImageCell _iconCell = new DataGridViewImageCell();
            protected DataGridViewTextBoxCell _descriptionCell = new DataGridViewTextBoxCell();
            protected DataGridViewCell _solutionCell = new DataGridViewLinkCell();

            protected PreCheckGridRow()
            {
                Cells.Add(_iconCell);
                Cells.Add(_descriptionCell);
                Cells.Add(_solutionCell);
            }
        }

        private class PreCheckHeaderRow : PreCheckGridRow
        {
            public PreCheckHeaderRow(string text)
            {
                _iconCell.Value = new Bitmap(1, 1);
                _descriptionCell.Value = text;
                _descriptionCell.Style.Font = new Font(Program.DefaultFont, FontStyle.Bold);
            }
        }

        private class PreCheckItemRow : PreCheckGridRow
        {
            private Problem _problem = null;
            private Check _check = null;
            public PreCheckItemRow(Problem problem)
            {
                _problem = problem;
                _check = problem.Check;
                UpdateRowFields();
            }

            public PreCheckItemRow(Check check)
            {
                _check = check;
                UpdateRowFields();
            }

            private void UpdateRowFields()
            {
                _iconCell.Value = Problem == null
                    ? Images.GetImage16For(Icons.Ok)
                    : Problem.Image;

                if (Problem != null)
                    _descriptionCell.Value = String.Format(Messages.DR_WIZARD_PRECHECKPAGE_PROBLEM, _check.Description, Problem.Description);
                else if (_check != null)
                {
                    _descriptionCell.Value = String.Format(Messages.DR_WIZARD_PRECHECKPAGE_OK, _check.Description);
                }
                _solutionCell.Value = Problem == null ? string.Empty : Problem.HelpMessage;
            }

            public Problem Problem
            {
                get
                {
                    return _problem;
                }
            }

            public string Description
            {
                get
                {
                    return _descriptionCell.Value as string;
                }
            }

            public bool IsProblem
            {
                get { return _problem != null && !(_problem is Warning); }
            }

            public bool IsWarning
            {
                get { return _problem != null && _problem is Warning; }
            }

            public override bool Equals(object obj)
            {
                PreCheckItemRow other = obj as PreCheckItemRow;
                if (other != null && other.Problem != null && _problem != null)
                    return _problem.CompareTo(other.Problem) == 0;
                return false;
            }

            public override int GetHashCode()
            {
                return (_problem != null ? _problem.GetHashCode() : 0);
            }
        }
        #endregion
    }

}

