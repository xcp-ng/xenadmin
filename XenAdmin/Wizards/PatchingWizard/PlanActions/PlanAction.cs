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
using System.Threading;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public abstract class PlanAction : IEquatable<PlanAction>
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int _percentComplete;
        public event Action<PlanAction> OnProgressChange;
        public Exception Error;
        protected bool Cancelling;
        private bool _running;
        private readonly Guid _actionId;

        private readonly object historyLock = new object();
        private readonly Stack<string> _progressHistory = new Stack<string>();

        public virtual string Title
        {
            get { return null; }
        }

        public virtual bool IsSkippable
        {
            get { return false; }
        }

        public virtual bool Skipping { private get; set; }

        protected virtual void DoOnSkip() { }

        public int PercentComplete
        {
            get
            {
                return _percentComplete;
            }
            protected set
            {
                _percentComplete = value;
                if (OnProgressChange != null)
                    OnProgressChange(this);
            }
        }

        public List<string> ProgressHistory
        {
            get
            {
                lock (historyLock)
                    return _progressHistory.Reverse().ToList();
            }
        }

        protected PlanAction()
        {
            _percentComplete = 0;
            _actionId = Guid.NewGuid();
        }

        protected abstract void _Run();

        public virtual void Run()
        {
            try
            {
                lock (historyLock)
                    _progressHistory.Clear();
                _running = true;
                PercentComplete = 0;
                if (Skipping)
                {
                    DoOnSkip();
                    Skipping = false;
                }
                else
                {
                    _Run();
                }
                AddProgressStep(null);
            }
            catch (CancelledException e)
            {
                ReplaceProgressStep(CurrentProgressStep + Messages.PLAN_ACTION_CANCELLED_BY_USER);
                Error = e;
                throw;
            }
            catch (Exception e)
            {
                if (e is Failure f && f.ErrorDescription != null && f.ErrorDescription.Count > 1 && f.ErrorDescription[1].Contains(FriendlyErrorNames.SR_BACKEND_FAILURE_432))
                {
                    // ignore this exception (CA-62989) in order to allow the Upgrade wizard to continue
                    // upgrading all the hosts in a pool. The detached SRs will be reported on Finish
                    log.Warn("There is a StorageLink Gateway SR that needs to be reattached.", f);
                }
                else
                {
                    var curStep = CurrentProgressStep;
                    if (!string.IsNullOrEmpty(curStep))
                        ReplaceProgressStep(CurrentProgressStep + Messages.PLAN_ACTION_ERROR);
                    Error = e;
                    throw;
                }
            }
            finally
            {
                _running = false;
                PercentComplete = 100;
            }
        }

        public string CurrentProgressStep
        {
            get
            {
                lock (historyLock)
                    return _progressHistory.Count > 0 ? _progressHistory.Peek() : string.Empty;
            }
        }

        protected void AddProgressStep(string step)
        {
            lock (historyLock)
            {
                if (_progressHistory.Count > 0)
                {
                    var popped = _progressHistory.Pop();
                    _progressHistory.Push(popped + Messages.PLAN_ACTION_DONE);
                }

                if (step != null)
                    _progressHistory.Push(step);

                if (OnProgressChange != null)
                    OnProgressChange(this);
            }
        }

        protected void ReplaceProgressStep(string step)
        {
            lock (historyLock)
            {
                if (_progressHistory.Count > 0)
                    _progressHistory.Pop();

                _progressHistory.Push(step);

                if (OnProgressChange != null)
                    OnProgressChange(this);
            }
        }

        protected string PollTaskForResultAndDestroy(IXenConnection connection, ref Session session, XenRef<Task> task)
        {
            return PollTaskForResultAndDestroy(connection, ref session, task, 0, 100);
        }

        protected string PollTaskForResultAndDestroy(IXenConnection connection, ref Session session, XenRef<Task> task, int min, int max)
        {
            try
            {
                return PollTaskForResult(connection, ref session, task,
                    progress => PercentComplete = progress, min, max);
            }
            finally
            {
                Task.destroy(session, task);
            }
        }

        protected static string PollTaskForResult(IXenConnection connection, ref Session session,
            XenRef<Task> task, Action<int> updateProgressDelegate)
        {
            return PollTaskForResult(connection, ref session, task, updateProgressDelegate, 0, 100);
        }

        protected static String PollTaskForResult(IXenConnection connection, ref Session session,
            XenRef<Task> task, Action<int> updateProgressDelegate, int min, int max)
        {
            Program.AssertOffEventThread();

            task_status_type status;
            int progress;

            do
            {
                status = (task_status_type)Task.DoWithSessionRetry(connection, ref session,
                                                                   (Task.TaskStatusOp)Task.get_status, task.opaque_ref);
                progress = min + (int)((max - min) * (double)Task.DoWithSessionRetry(connection, ref session,
                                                                                     (Task.TaskProgressOp)Task.get_progress, task.opaque_ref));

                updateProgressDelegate(progress);

                Thread.Sleep(100);
            }
            while (status == task_status_type.pending || status == task_status_type.cancelling);

            if (status == task_status_type.failure)
            {
                throw new Failure(Task.get_error_info(session, task));
            }
            else
            {
                return Task.get_result(session, task);
            }
        }

        public bool IsComplete
        {
            get
            {
                return (PercentComplete == 100 && !_running) || Error != null;
            }
        }

        public bool Equals(PlanAction other)
        {
            if (other == null)
                return false;

            return string.Equals(_actionId.ToString(), other._actionId.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public virtual void Cancel()
        {
            Cancelling = true;
        }
    }
}


