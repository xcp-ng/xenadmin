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
using System.Reflection;
using log4net;
using XenAdmin.Controls;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAPI;
using System.Linq;
using XenAdmin.Core;
using System.Text;
using XenAdmin.Diagnostics.Problems;


namespace XenAdmin.Wizards.PatchingWizard
{
    public enum Status { NotStarted, Started, Cancelled, Completed }
    public abstract partial class AutomatedUpdatesBasePage : XenTabPage
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool _thisPageIsCompleted = false;
        private bool _someWorkersFailed = false;

        public List<Problem> PrecheckProblemsActuallyResolved { private get; set; }
        public List<Pool> SelectedPools { private get; set; }
        public bool ApplyUpdatesToNewVersion { get; set; }
        public Status Status { get; private set; }

        private List<UpdateProgressBackgroundWorker> backgroundWorkers = new List<UpdateProgressBackgroundWorker>();
        private List<UpdateProgressBackgroundWorker> failedWorkers = new List<UpdateProgressBackgroundWorker>();

        private List<PoolPatchMapping> patchMappings = new List<PoolPatchMapping>();
        public Dictionary<XenServerPatch, string> AllDownloadedPatches = new Dictionary<XenServerPatch, string>();

        public AutomatedUpdatesBasePage()
        {
            InitializeComponent();
            panel1.Visible = false;
        }

        #region XenTabPage overrides
        public override bool EnablePrevious()
        {
            return false;
        }

        private bool _nextEnabled;
        public override bool EnableNext()
        {
            return _nextEnabled;
        }

        private bool _cancelEnabled = true;
        public override bool EnableCancel()
        {
            return _cancelEnabled;
        }

        public override void PageCancelled()
        {
            if (!_thisPageIsCompleted)
            {
                Status = Status.Cancelled;
                backgroundWorkers.ForEach(bgw => bgw.CancelAsync());
            }

            base.PageCancelled();
        }
        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (_thisPageIsCompleted)
                return;

            Status = Status.NotStarted;
            labelTitle.Text = BlurbText();

            if (!StartUpgradeWorkers())
            {
                Status = Status.Completed;
                _thisPageIsCompleted = true;
                _nextEnabled = true;
                OnPageUpdated();
            }
            else
            {
                Status = Status.Started;
            }
        }

        #endregion

        #region Virtual members

        protected abstract string BlurbText();
        protected abstract string SuccessMessageOnCompletion(bool multiplePools);
        protected abstract string FailureMessageOnCompletion(bool multiplePools);
        protected abstract string SuccessMessagePerPool();
        protected abstract string FailureMessagePerPool(bool multipleErrors);
        protected abstract string UserCancellationMessage();

        protected abstract List<HostPlan> GenerateHostPlans(Pool pool, out List<Host> applicableHosts);

        protected virtual bool SkipInitialPlanActions(Host host)
        {
            return false;
        }

        protected virtual void DoAfterInitialPlanActions(UpdateProgressBackgroundWorker bgw, Host host, List<Host> hosts) { }
        #endregion

        #region background workers
        private bool StartUpgradeWorkers()
        {
            //reset the background workers
            backgroundWorkers = new List<UpdateProgressBackgroundWorker>();
            bool atLeastOneWorkerStarted = false;
            
            foreach (var pool in SelectedPools)
            {
                List<Host> applicableHosts;
                var planActions = GenerateHostPlans(pool, out applicableHosts);

                var finalActions = new List<PlanAction>();

                //add a revert pre-check action for this pool
                var curPool = pool;
                var problemsToRevert = PrecheckProblemsActuallyResolved.Where(a =>
                    a.SolutionAction != null && Helpers.GetPoolOfOne(a.SolutionAction.Connection).Equals(curPool)).ToList();

                if (problemsToRevert.Count > 0)
                    finalActions.Add(new UnwindProblemsAction(problemsToRevert, pool.Connection));

                if (planActions.Count > 0)
                {
                    atLeastOneWorkerStarted = true;
                    StartNewWorker(pool.Name(), planActions, finalActions);
                }
            }

            return atLeastOneWorkerStarted;
        }

        private void StartNewWorker(string poolName, List<HostPlan> planActions, List<PlanAction> finalActions)
        {
            var bgw = new UpdateProgressBackgroundWorker(planActions, finalActions)
            {
                Name = poolName,
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            bgw.DoWork += WorkerDoWork;
            bgw.ProgressChanged += WorkerProgressChanged;
            bgw.RunWorkerCompleted += WorkerCompleted;
            backgroundWorkers.Add(bgw);
            bgw.RunWorkerAsync();
        }

        private void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var bgw = sender as UpdateProgressBackgroundWorker;
            if (bgw == null)
                return;

            if (!bgw.CancellationPending)
            {
                PlanAction action = (PlanAction)e.UserState;
                if (action != null)
                {
                    if (!action.IsComplete)
                    {
                        if (!bgw.InProgressActions.Contains(action))
                            bgw.InProgressActions.Add(action);
                    }
                    else
                    {
                        if (!bgw.DoneActions.Contains(action))
                            bgw.DoneActions.Add(action);
                        bgw.InProgressActions.Remove(action);

                        if (action.Error == null)
                        {
                            // remove the successful action from the cleanup actions (we are running the cleanup actions in case of failures or if the user cancelled the process, but we shouldn't re-run the actions that have already been run)
                            bgw.CleanupActions.Remove(action);
                        }
                        else
                        {
                            if (!failedWorkers.Contains(bgw))
                                failedWorkers.Add(bgw);
                        }
                    }
                }

                UpdateStatus();
            }
        }

        private void UpdateStatus()
        {
            var newVal = backgroundWorkers.Sum(b => b.PercentComplete) / backgroundWorkers.Count;
            if (newVal < 0)
                newVal = 0;
            else if (newVal > 100)
                newVal = 100;
            progressBar.Value = (int)newVal;

            var allsb = new StringBuilder();

            foreach (var bgw in backgroundWorkers)
            {
                int bgwErrorCount = 0;
                int bgwCancellationCount = 0;
                var sb = new StringBuilder();
                var errorSb = new StringBuilder();

                if (!String.IsNullOrEmpty(bgw.Name))
                    sb.AppendLine(string.Format("{0}:", bgw.Name));

                foreach (var pa in bgw.DoneActions)
                {
                    pa.ProgressHistory.ForEach(step => sb.AppendIndented(step).AppendLine());

                    if (pa.Error != null)
                    {
                        if (pa.Error is CancelledException)
                        {
                            bgwCancellationCount++;
                            continue;
                        }

                        var innerEx = pa.Error.InnerException as Failure;
                        errorSb.AppendLine(innerEx == null ? pa.Error.Message : innerEx.Message);
                        bgwErrorCount++;
                    }
                }

                foreach (var pa in bgw.InProgressActions)
                {
                    pa.ProgressHistory.ForEach(step => sb.AppendIndented(step).AppendLine());
                }

                sb.AppendLine();

                if (bgwCancellationCount > 0)
                {
                    sb.AppendIndented(UserCancellationMessage()).AppendLine();
                }
                else if (bgwErrorCount > 0)
                {
                    sb.AppendIndented(FailureMessagePerPool(bgwErrorCount > 1)).AppendLine();
                    sb.AppendIndented(errorSb);
                }
                else if (!bgw.IsBusy)
                {
                    sb.AppendIndented(SuccessMessagePerPool()).AppendLine();
                }

                sb.AppendLine();
                allsb.Append(sb);
            }

            textBoxLog.Text = allsb.ToString();
            textBoxLog.SelectionStart = textBoxLog.Text.Length;
            textBoxLog.ScrollToCaret();
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var bgw = sender as UpdateProgressBackgroundWorker;
            if (bgw == null)
                return;

            PlanAction action = null;

            try
            {
                foreach (var hp in bgw.HostPlans)
                {
                    var host = hp.Host;

                    // Step 1: InitialPlanActions  (e.g. upgrade the host in the RPU case)
                    bgw.ProgressIncrement = bgw.InitialActionsIncrement(hp);
                    if (!SkipInitialPlanActions(host))
                    {
                        var initialActions = hp.InitialPlanActions;

                        foreach (var a in initialActions)
                        {
                            action = a;
                            bgw.RunPlanAction(action, ref doWorkEventArgs);
                        }
                    }

                    DoAfterInitialPlanActions(bgw, host, bgw.HostPlans.Select(h => h.Host).ToList());

                    // Step 2: UpdatesPlanActions  (priority update action)
                    bgw.ProgressIncrement = bgw.UpdatesActionsIncrement(hp);
                    var planActions = hp.UpdatesPlanActions; 
                    foreach (var a in planActions)
                    {
                        action = a;
                        bgw.RunPlanAction(action, ref doWorkEventArgs);
                    }

                    // Step 3: DelayedActions
                    bgw.ProgressIncrement = bgw.DelayedActionsIncrement(hp);
                    // running delayed actions, but skipping the ones that should be skipped
                    var delayedActions = hp.DelayedPlanActions;
                    var restartActions = delayedActions.Where(a => a is RestartHostPlanAction).ToList();

                    foreach (var a in restartActions)
                    {
                        action = a;
                        bgw.RunPlanAction(action, ref doWorkEventArgs);
                    }

                    var otherActions = delayedActions.Where(a => !(a is RestartHostPlanAction)).ToList();

                    foreach (var a in otherActions)
                    {
                        action = a;

                        // any non-restart-alike delayed action needs to be run if:
                        // - this host is pre-Ely and there isn't any delayed restart plan action, or
                        // - this host is Ely or above and live patching must have succeeded or there isn't any delayed restart plan action
                        if (restartActions.Count <= 0 ||
                            (Helpers.ElyOrGreater(host) && host.Connection.TryResolveWithTimeout(new XenRef<Host>(host.opaque_ref)).updates_requiring_reboot.Count <= 0))
                        {
                            bgw.RunPlanAction(action, ref doWorkEventArgs);
                        }
                        else
                        {
                            //skip running it, but still need to report progress, mainly for the progress bar
                            bgw.RunPlanAction(action, ref doWorkEventArgs, true);
                        }
                    }
                }

                // Step 4: FinalActions (eg. revert pre-checks)
                bgw.ProgressIncrement = bgw.FinalActionsIncrement;
                foreach (var a in bgw.FinalActions)
                {
                    action = a;
                    bgw.RunPlanAction(action, ref doWorkEventArgs);
                }
            }
            catch (Exception e)
            {
                if (action.Error == null)
                    action.Error = new Exception(Messages.ERROR_UNKNOWN);

                log.ErrorFormat("Failed to carry out plan. {0} {1}", action.CurrentProgressStep, e);
                doWorkEventArgs.Result = new Exception(action.CurrentProgressStep, e);
            }
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Status = Status.Completed;
                panel1.Visible = true;
                labelError.Text = UserCancellationMessage();
                pictureBox1.Image = Images.StaticImages.cancelled_action_16;
                buttonRetry.Visible = false;
                _thisPageIsCompleted = true;
                _cancelEnabled = false;
                _nextEnabled = true;
            }
            else
            {
                var someWorkersCancelled = false;
                var bgw = sender as UpdateProgressBackgroundWorker;
                if (bgw != null)
                {
                    var workerSucceeded = true;

                    if (bgw.DoneActions.Any(a => a.Error != null && !(a.Error is CancelledException)))
                    {
                        workerSucceeded = false;
                        _someWorkersFailed = true;
                    }

                    if (bgw.DoneActions.Any(a => a.Error is CancelledException))
                    {
                        workerSucceeded = false;
                        someWorkersCancelled = true;
                    }

                    if (workerSucceeded)
                        bgw.PercentComplete = 100;
                }
                //if all finished
                if (backgroundWorkers.All(w => !w.IsBusy))
                {
                    Status = Status.Completed;
                    panel1.Visible = true;
                    if (_someWorkersFailed)
                    {
                        labelError.Text = FailureMessageOnCompletion(backgroundWorkers.Count > 1);
                        pictureBox1.Image = Images.StaticImages._000_error_h32bit_16;
                        buttonRetry.Visible = true;
                    }
                    else if (someWorkersCancelled)
                    {
                        labelError.Text = UserCancellationMessage();
                        pictureBox1.Image = Images.StaticImages.cancelled_action_16;
                        buttonRetry.Visible = false;
                    }
                    else
                    {
                        labelError.Text = SuccessMessageOnCompletion(backgroundWorkers.Count > 1);
                        pictureBox1.Image = Images.StaticImages._000_Tick_h32bit_16;
                        buttonRetry.Visible = false;
                    }

                    _thisPageIsCompleted = true;
                    _cancelEnabled = false;
                    _nextEnabled = true;
                }
            }

            UpdateStatus();
            OnPageUpdated();
        }

        private void RetryFailedActions()
        {
            _someWorkersFailed = false;
            panel1.Visible = false;

            var workers = new List<UpdateProgressBackgroundWorker>(failedWorkers);
            failedWorkers.Clear();

            foreach (var failedWorker in workers)
            {
                failedWorker.RunWorkerAsync();
            }

            _thisPageIsCompleted = false;
            _cancelEnabled = true;
            _nextEnabled = false;
            OnPageUpdated();
        }

        #endregion

        private void buttonRetry_Click(object sender, EventArgs e)
        {
            RetryFailedActions();
        }

        protected HostPlan GetUpdatePlanActionsForHost(Host host, List<Host> hosts, List<XenServerPatch> minimalPatches,
            List<XenServerPatch> uploadedPatches, KeyValuePair<XenServerPatch, string> patchFromDisk, bool repatriateVms = true)
        {
            var patchSequence = Updates.GetPatchSequenceForHost(host, minimalPatches);
            if (patchSequence == null)
                return new HostPlan(host, null, null, null);

            var planActionsPerHost = new List<PlanAction>();
            var delayedActionsPerHost = new List<PlanAction>();

            foreach (var patch in patchSequence)
            {
                if (!uploadedPatches.Contains(patch))
                {
                    planActionsPerHost.Add(new DownloadPatchPlanAction(host.Connection, patch, AllDownloadedPatches, patchFromDisk));
                    planActionsPerHost.Add(new UploadPatchToMasterPlanAction(host.Connection, patch, patchMappings, AllDownloadedPatches, patchFromDisk));
                    uploadedPatches.Add(patch);
                }

                planActionsPerHost.Add(new PatchPrecheckOnHostPlanAction(host.Connection, patch, host, patchMappings));
                planActionsPerHost.Add(new ApplyXenServerPatchPlanAction(host, patch, patchMappings));

                var action = patch.after_apply_guidance == after_apply_guidance.restartXAPI && delayedActionsPerHost.Any(a => a is RestartHostPlanAction)
                       ? new RestartHostPlanAction(host, host.GetRunningVMs(), true, true)
                       : GetAfterApplyGuidanceAction(host, patch.after_apply_guidance);

                if (action != null)
                {
                    if (patch.GuidanceMandatory)
                    {
                        planActionsPerHost.Add(action);
                        // remove all delayed actions of the same kind that has already been added
                        // (because this action is guidance-mandatory=true, therefore
                        // it will run immediately, making delayed ones obsolete)
                        delayedActionsPerHost.RemoveAll(a => action.GetType() == a.GetType());
                    }
                    else
                    {
                        // add the action if it's not already in the list
                        if (delayedActionsPerHost.All(a => a.GetType() != action.GetType()))
                            delayedActionsPerHost.Add(action);
                    }
                }

                var isLastHostInPool = hosts.IndexOf(host) == hosts.Count - 1;
                if (isLastHostInPool)
                {
                    // add cleanup action for current patch at the end of the update seuence for the last host in the pool
                    var master = Helpers.GetMaster(host.Connection);
                    planActionsPerHost.Add(new RemoveUpdateFileFromMasterPlanAction(master, patchMappings, patch));
                }
            }

            if (repatriateVms)
            {
                var lastRestart = delayedActionsPerHost.FindLast(a => a is RestartHostPlanAction)
                                  ?? planActionsPerHost.FindLast(a => a is RestartHostPlanAction);

                if (lastRestart != null)
                    ((RestartHostPlanAction) lastRestart).EnableOnly = false;
            }

            return new HostPlan(host, null, planActionsPerHost, delayedActionsPerHost);
        }

        private static PlanAction GetAfterApplyGuidanceAction(Host host, after_apply_guidance guidance)
        {
            switch (guidance)
            {
                case after_apply_guidance.restartHost:
                    return new RestartHostPlanAction(host, host.GetRunningVMs(), true);
                case after_apply_guidance.restartXAPI:
                    return new RestartAgentPlanAction(host);
                case after_apply_guidance.restartHVM:
                    return new RebootVMsPlanAction(host, host.GetRunningHvmVMs());
                case after_apply_guidance.restartPV:
                    return new RebootVMsPlanAction(host, host.GetRunningPvVMs());
                default:
                    return null;
            }
        }
    }
}
