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
using System.ComponentModel;
using System.Linq;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard
{
    public class UpdateProgressBackgroundWorker : BackgroundWorker
    {
        public List<HostPlan> HostPlans { get; private set; }
        public List<PlanAction> FinalActions { get; private set; }
        public List<PlanAction> CleanupActions { get; private set; }
        public readonly List<PlanAction> DoneActions = new List<PlanAction>();
        public readonly List<PlanAction> InProgressActions = new List<PlanAction>();
        public Pool Pool { get; private set; }
        public string Name { get; private set; }
        
        private double _percentComplete;
        private readonly object _percentLock = new object();

        public double PercentComplete
        {
            get
            {
                lock (_percentLock)
                    return _percentComplete;
            }
            set
            {
                lock (_percentLock)
                    _percentComplete = value;
            }
        }

        public double ProgressIncrement { get; set; }

        public UpdateProgressBackgroundWorker(Pool pool, List<HostPlan> planActions, List<PlanAction> finalActions)
        {
            Pool = pool;
            Name = pool.Name();
            HostPlans = planActions ?? new List<HostPlan>();
            FinalActions = finalActions ?? new List<PlanAction>();
            CleanupActions = FinalActions.Where(a => a is RemoveUpdateFileFromMasterPlanAction).ToList();
        }

        public void RunPlanAction(PlanAction action, ref DoWorkEventArgs e, bool skip = false)
        {
            if (CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            // if this action is completed successfully, do not run it again
            if (DoneActions.Contains(action) && action.Error == null)
                return;

            // if we retry a failed action, we need to firstly remove it from DoneActions and reset its Error
            DoneActions.Remove(action);
            action.Error = null;

            if (skip)
            {
                //skip running it, but still need to report progress, mainly for the progress bar
                PercentComplete += ProgressIncrement * 100;
                ReportProgress((int)PercentComplete, action);
            }
            else
            {
                action.OnProgressChange += action_OnProgressChange;
                action.Run();
                action.OnProgressChange -= action_OnProgressChange;
            }
        }

        private void action_OnProgressChange(PlanAction planAction)
        {
            PercentComplete += planAction.IsComplete ? ProgressIncrement * 100 : 0;
            ReportProgress((int)PercentComplete, planAction);
        }

        public new void CancelAsync()
        {
            if (HostPlans != null)
                HostPlans.ForEach(ha =>
                {
                    var cur = ha;
                    cur.InitialPlanActions.ForEach(pa =>
                    {
                        if (!pa.IsComplete)
                            pa.Cancel();
                    });

                    cur.UpdatesPlanActions.ForEach(pa =>
                    {
                        if (!pa.IsComplete)
                            pa.Cancel();
                    });
                });

            base.CancelAsync();
        }

        private double FinalActionsPercentage
        {
            get
            {
                if (FinalActions == null || FinalActions.Count < 1)
                    return 0;
                return 0.4;
            }
        }

        private double HostPlansPercentage
        {
            get
            {
                if (HostPlans == null || HostPlans.Count < 1)
                    return 0;
                return 1 - FinalActionsPercentage;
            }
        }

        public double FinalActionsIncrement
        {
            get
            {
                if (FinalActions == null || FinalActions.Count < 1)
                    return 0;
                return FinalActionsPercentage / FinalActions.Count;
            }
        }

        public double HostPlansIncrement
        {
            get
            {
                if (HostPlans == null || HostPlans.Count < 1)
                    return 0;
                return HostPlansPercentage / HostPlans.Count;
            }
        }

        public double InitialActionsIncrement(HostPlan hp)
        {
            return hp.InitialActionsIncrement * HostPlansIncrement;
        }

        public double UpdatesActionsIncrement(HostPlan hp)
        {
            return hp.UpdatesActionsIncrement * HostPlansIncrement;
        }

        public double DelayedActionsIncrement(HostPlan hp)
        {
            return hp.DelayedActionsIncrement * HostPlansIncrement;
        }
    }

    public class HostPlan
    {
        public Host Host;
        public readonly List<PlanAction> InitialPlanActions;
        public readonly List<PlanAction> UpdatesPlanActions;
        public readonly List<PlanAction> DelayedPlanActions;

        public HostPlan(Host host, List<PlanAction> initialActions, List<PlanAction> updateActions, List<PlanAction> delayedActions)
        {
            Host = host;
            InitialPlanActions = initialActions ?? new List<PlanAction>();
            UpdatesPlanActions = updateActions ?? new List<PlanAction>();
            DelayedPlanActions = delayedActions ?? new List<PlanAction>();
        }

        private double InitialActionsPercentage
        {
            get
            {
                if (InitialPlanActions == null || InitialPlanActions.Count < 1)
                    return 0;
                return 0.3;
            }
        }

        private double UpdatesActionsPercentage
        {
            get
            {
                if (UpdatesPlanActions == null || UpdatesPlanActions.Count < 1)
                    return 0;
                return 0.9 - InitialActionsPercentage;
            }
        }

        private double DelayedActionsPercentage
        {
            get
            {
                if (DelayedPlanActions == null || DelayedPlanActions.Count < 1)
                    return 0;
                return 1 - InitialActionsPercentage - UpdatesActionsPercentage;
            }
        }

        public double InitialActionsIncrement
        {
            get
            {
                if (InitialPlanActions == null || InitialPlanActions.Count < 1)
                    return 0;
                return InitialActionsPercentage / InitialPlanActions.Count;
            }
        }

        public double UpdatesActionsIncrement
        {
            get
            {
                if (UpdatesPlanActions == null || UpdatesPlanActions.Count < 1)
                    return 0;
                return UpdatesActionsPercentage / UpdatesPlanActions.Count;
            }
        }

        public double DelayedActionsIncrement
        {
            get
            {
                if (DelayedPlanActions == null || DelayedPlanActions.Count < 1)
                    return 0;
                return DelayedActionsPercentage / DelayedPlanActions.Count;
            }
        }
    }
}
