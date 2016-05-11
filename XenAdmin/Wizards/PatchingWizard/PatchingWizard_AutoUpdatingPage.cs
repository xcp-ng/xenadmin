﻿/* Copyright (c) Citrix Systems Inc. 
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
using System.IO;
using System.Reflection;
using System.Threading;
using log4net;
using XenAdmin.Controls;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Dialogs;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAPI;
using XenAdmin.Actions;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Network;
using System.Text;

namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_AutoUpdatingPage : XenTabPage
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public XenAdmin.Core.Updates.UpgradeSequence UpgradeSequences { get; set; }
        private bool _thisPageHasBeenCompleted = false;

        public List<Problem> ProblemsResolvedPreCheck { private get; set; }

        public List<Host> SelectedMasters { private get; set; }

        private List<PoolPatchMapping> patchMappings = new List<PoolPatchMapping>();
        private Dictionary<XenServerPatch, string> AllDownloadedPatches = new Dictionary<XenServerPatch, string>();

        private List<BackgroundWorker> backgroundWorkers = new List<BackgroundWorker>();

        private List<UpgradeProgressDescriptor> upgradeProgressDescriptors = new List<UpgradeProgressDescriptor>();


        public PatchingWizard_AutoUpdatingPage()
        {
            InitializeComponent();
        }

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

        public override bool EnablePrevious()
        {
            return false;
        }

        private bool _nextEnabled;
        public override bool EnableNext()
        {
            return _nextEnabled;
        }

        private bool _cancelEnabled;
        public override bool EnableCancel()
        {
            return _cancelEnabled;
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);

            if (_thisPageHasBeenCompleted)
            {
                actionsWorker = null;
                return;
            }

            Dictionary<Host, List<PlanAction>> planActionsPerPool = new Dictionary<Host, List<PlanAction>>();

            foreach (var master in SelectedMasters)
            {
                Dictionary<Host, List<PlanAction>> planActionsByHost = new Dictionary<Host, List<PlanAction>>();
                Dictionary<Host, List<PlanAction>> delayedActionsByHost = new Dictionary<Host, List<PlanAction>>();

                foreach (var host in master.Connection.Cache.Hosts)
                {
                    planActionsByHost.Add(host, new List<PlanAction>());
                    delayedActionsByHost.Add(host, new List<PlanAction>());
                }

                //var master = Helpers.GetMaster(pool.Connection);
                var hosts = master.Connection.Cache.Hosts;

                var us = Updates.GetUpgradeSequence(master.Connection);

                foreach (var patch in us.UniquePatches)
                {
                    var hostsToApply = us.Where(u => u.Value.Contains(patch)).Select(u => u.Key).ToList();

                    planActionsByHost[master].Add(new DownloadPatchPlanAction(master.Connection, patch, patchMappings, AllDownloadedPatches));
                    planActionsByHost[master].Add(new UploadPatchToMasterPlanAction(master.Connection, patch, patchMappings, AllDownloadedPatches));
                    planActionsByHost[master].Add(new PatchPrechecksOnMultipleHostsPlanAction(master.Connection, patch, hostsToApply, patchMappings));

                    if (hostsToApply.Contains(master))
                    {
                        planActionsByHost[master].Add(new ApplyXenServerPatchPlanAction(master, patch, patchMappings));
                        planActionsByHost[master].AddRange(GetMandatoryActionListForPatch(delayedActionsByHost[master], master, patch));
                        UpdateDelayedAfterPatchGuidanceActionListForHost(delayedActionsByHost[master], master, patch);
                    }

                    foreach (var host in hostsToApply)
                    {
                        if (host != master)
                            planActionsByHost[host].Add(new ApplyXenServerPatchPlanAction(host, patch, patchMappings));
                            planActionsByHost[host].AddRange(GetMandatoryActionListForPatch(delayedActionsByHost[host], host, patch));
                            UpdateDelayedAfterPatchGuidanceActionListForHost(delayedActionsByHost[host], host, patch);
                    }

                    // now add all non-delayed actions to the pool action list

                    foreach (var kvp in planActionsByHost)
                    {
                        if (!planActionsPerPool.ContainsKey(master))
                        {
                            planActionsPerPool.Add(master, new List<PlanAction>());
                        }

                        planActionsPerPool[master].AddRange(kvp.Value);
                        kvp.Value.Clear();
                    }

                    //clean up master at the end:
                    planActionsPerPool[master].Add(new RemoveUpdateFileFromMasterPlanAction(master, patchMappings, patch));

                }//patch

                var delayedActions = new List<PlanAction>();
                //add all delayed actions to the end of the actions, per host
                foreach (var kvp in delayedActionsByHost)
                {
                    delayedActions.AddRange(kvp.Value);
                }

                var upd = new UpgradeProgressDescriptor(master, planActionsPerPool[master], delayedActions);
                upgradeProgressDescriptors.Add(upd);

            } //foreach in SelectedMasters


            //var planActions = new List<PlanAction>();

            //actions list for serial execution
            foreach (var desc in upgradeProgressDescriptors)
            {
                //planActions.AddRange(actionListPerPool.Value);

                var bgw = new BackgroundWorker();
                backgroundWorkers.Add(bgw);

                bgw.DoWork += new DoWorkEventHandler(PatchingWizardAutomaticPatchWork);
                bgw.WorkerReportsProgress = true;
                bgw.ProgressChanged += new ProgressChangedEventHandler(actionsWorker_ProgressChanged);
                bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(actionsWorker_RunWorkerCompleted);
                bgw.WorkerSupportsCancellation = true;
                bgw.RunWorkerAsync(desc);
            }

            //planActions.Add(new UnwindProblemsAction(ProblemsResolvedPreCheck));

            //actionsWorker = new BackgroundWorker();
            //actionsWorker.DoWork += new DoWorkEventHandler(PatchingWizardAutomaticPatchWork);
            //actionsWorker.WorkerReportsProgress = true;
            //actionsWorker.ProgressChanged += new ProgressChangedEventHandler(actionsWorker_ProgressChanged);
            //actionsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(actionsWorker_RunWorkerCompleted);
            //actionsWorker.WorkerSupportsCancellation = true;
            //actionsWorker.RunWorkerAsync(planActions);
        }

        #region automatic_mode

        private List<PlanAction> doneActions = new List<PlanAction>();
        private List<PlanAction> inProgressActions = new List<PlanAction>();

        private void actionsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var actionsWorker = sender as BackgroundWorker;

            if (!actionsWorker.CancellationPending)
            {
                PlanAction action = (PlanAction)e.UserState;
                if (e.ProgressPercentage == 0)
                {
                    inProgressActions.Add(action);
                }
                else
                {
                    doneActions.Add(action);
                    inProgressActions.Remove(action);

                    progressBar.Value += (int)((float)e.ProgressPercentage / (float)backgroundWorkers.Count); //extend with error handling related numbers
                }

                var sb = new StringBuilder();

                foreach (var pa in doneActions)
                {
                    sb.Append(pa);
                    sb.AppendLine(Messages.DONE);
                }

                foreach (var pa in inProgressActions)
                {
                    sb.Append(pa);
                    sb.AppendLine();
                }

                textBoxLog.Text = sb.ToString();
            }
        }

        private void PatchingWizardAutomaticPatchWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var actionsWorker = sender as BackgroundWorker;

            UpgradeProgressDescriptor descriptor = (UpgradeProgressDescriptor)doWorkEventArgs.Argument;

            var actionList = descriptor.planActions.Concat(descriptor.delayedActions).ToList();

            foreach (PlanAction action in actionList)
            {
                try
                {
                    if (actionsWorker.CancellationPending)
                    {
                        //descriptor.InProgressAction = null;
                        doWorkEventArgs.Cancel = true;
                        return;
                    }
                    //descriptor.InProgressAction = action;

                    actionsWorker.ReportProgress(0, action);
                    action.Run();
                    Thread.Sleep(1000);

                    //descriptor.doneActions.Add(action);

                    actionsWorker.ReportProgress((int)((1.0 / (double)actionList.Count) * 100), action);
                }
                catch (Exception e)
                {
                    //descriptor.FailedWithExceptionAction = action;

                    log.Error("Failed to carry out plan.", e);
                    log.Debug(actionList);
                    doWorkEventArgs.Result = new Exception(action.Title, e);
                    break;
                }
                finally
                {
                    //descriptor.InProgressAction = null;
                }
            }
        }

        private void actionsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                Exception exception = e.Result as Exception;
                if (exception != null)
                {
                    FinishedWithErrors(exception);
                    
                }
                else
                {
                    FinishedSuccessfully();
                }

                progressBar.Value = 100;
                _cancelEnabled = false;
                _nextEnabled = true;
            }
            OnPageUpdated();

            _thisPageHasBeenCompleted = true;
        }

        private void UpdateDelayedAfterPatchGuidanceActionListForHost(List<PlanAction> delayedGuidances, Host host, XenServerPatch patch)
        {
            List<PlanAction> actions = GetAfterApplyGuidanceActionsForPatch(host, patch);

            if (!patch.GuidanceMandatory)
            {
                //not mandatory, so these actions will have to be run later
                //add the ones that are not yet there
                foreach (var ap in actions)
                    if (!delayedGuidances.Any(da => da.GetType() == ap.GetType()))
                    {
                        //special rules
                        //do not add restartXAPI if there is already a restartHost on the list
                        if (delayedGuidances.Any(da => da is RebootHostPlanAction) && ap is RestartAgentPlanAction)
                            continue;

                        delayedGuidances.Add(ap);
                    }
            }
            else
            {
                //remove delayed action of the same kind for this host (because it is mandatory and will run immediately)
                delayedGuidances.RemoveAll(dg => actions.Any(ma => ma.GetType() == dg.GetType())); 

                //if it is a restart, clean delayed list
                if (patch.after_apply_guidance == after_apply_guidance.restartHost)
                    delayedGuidances.Clear();
                
            }
        }

        private static List<PlanAction> GetAfterApplyGuidanceActionsForPatch(Host host, XenServerPatch patch)
        {
            List<PlanAction> actions = new List<PlanAction>();

            List<XenRef<VM>> runningVMs = RunningVMs(host);

            if (patch.after_apply_guidance == after_apply_guidance.restartHost)
            {
                actions.Add(new EvacuateHostPlanAction(host));
                actions.Add(new RebootHostPlanAction(host));
                actions.Add(new BringBabiesBackAction(runningVMs, host, false));
            }

            if (patch.after_apply_guidance == after_apply_guidance.restartXAPI)
            {
                actions.Add(new RestartAgentPlanAction(host));
            }

            if (patch.after_apply_guidance == after_apply_guidance.restartHVM)
            {
                actions.Add(new RebootVMsPlanAction(host, RunningHvmVMs(host)));
            }

            if (patch.after_apply_guidance == after_apply_guidance.restartPV)
            {
                actions.Add(new RebootVMsPlanAction(host, RunningPvVMs(host)));
            }

            return actions;
        }

        private List<PlanAction> GetMandatoryActionListForPatch(List<PlanAction> delayedGuidances, Host host, XenServerPatch patch)
        {
            var actions = new List<PlanAction>();

            if (!patch.GuidanceMandatory)
                return actions;

            actions = GetAfterApplyGuidanceActionsForPatch(host, patch);

            return actions;
        }

        private static List<XenRef<VM>> RunningHvmVMs(Host host)
        {
            List<XenRef<VM>> vms = new List<XenRef<VM>>();
            foreach (VM vm in host.Connection.ResolveAll(host.resident_VMs))
            {
                if (!vm.IsHVM || !vm.is_a_real_vm)
                    continue;
                vms.Add(new XenRef<VM>(vm.opaque_ref));
            }
            return vms;
        }

        private static List<XenRef<VM>> RunningPvVMs(Host host)
        {
            List<XenRef<VM>> vms = new List<XenRef<VM>>();
            foreach (VM vm in host.Connection.ResolveAll(host.resident_VMs))
            {
                if (vm.IsHVM || !vm.is_a_real_vm)
                    continue;
                vms.Add(new XenRef<VM>(vm.opaque_ref));
            }
            return vms;
        }

        private static List<XenRef<VM>> RunningVMs(Host host)
        {
            List<XenRef<VM>> vms = new List<XenRef<VM>>();
            foreach (VM vm in host.Connection.ResolveAll(host.resident_VMs))
            {
                if (!vm.is_a_real_vm)
                    continue;

                vms.Add(new XenRef<VM>(vm.opaque_ref));
            }
            return vms;
        }


        #endregion

        private void FinishedWithErrors(Exception exception)
        {
            //labelTitle.Text = string.Format(Messages.UPDATE_WAS_NOT_COMPLETED, GetUpdateName());

            string errorMessage = null;

            if (exception != null && exception.InnerException != null && exception.InnerException is Failure)
            {
                var innerEx = exception.InnerException as Failure;
                errorMessage = innerEx.Message;

                if (innerEx.ErrorDescription != null && innerEx.ErrorDescription.Count > 0)
                    log.Error(string.Concat(innerEx.ErrorDescription.ToArray()));
            }

            labelError.Text = errorMessage ?? string.Format(Messages.PATCHING_WIZARD_ERROR, exception.Message);

            log.ErrorFormat("Error message displayed: {0}", labelError.Text);

            pictureBox1.Image = SystemIcons.Error.ToBitmap();
            if (exception.InnerException is SupplementalPackInstallFailedException)
            {
                errorLinkLabel.Visible = true;
                errorLinkLabel.Tag = exception.InnerException;
            }
            panel1.Visible = true;
        }

        private void FinishedSuccessfully()
        {
            labelTitle.Text = Messages.PATCHINGWIZARD_UPDATES_DONE_AUTOMATIC_MODE;
            pictureBox1.Image = null;
            labelError.Text = Messages.CLOSE_WIZARD_CLICK_FINISH;
        }
    }
}

