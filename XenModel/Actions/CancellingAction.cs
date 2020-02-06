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
using System.Threading;
using XenAdmin.Network;
using XenAPI;
using System.Collections.Generic;
using System.Net;
using CookComputing.XmlRpc;
using System.Reflection;

namespace XenAdmin.Actions
{
    public class CancellingAction : ActionBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Session _cancel_session = null;
        private Session _session;

        /// <summary>
        /// Whether, the last time we checked on the server, this task could be cancelled.  This is a cached
        /// value, so may be stale.
        /// </summary>
        private volatile bool can_cancel = false;

        /// <summary>
        /// Whether this operation is being cancelled.  This takes precedence over can_cancel.
        /// </summary>
        private volatile bool cancelling = false;

        /// <summary>
        /// Whether this operation was cancelled or not.
        /// </summary>
        private volatile bool cancelled = false;

        protected CancellingAction(string title, string description, bool suppressHistory) 
            : base(title, description, suppressHistory)
        {
        }

        protected CancellingAction(string title, string description, bool suppressHistory, bool completeImmediately)
            : base(title, description, suppressHistory, completeImmediately)
        {
        }

        private XenRef<Task> _relatedTask;

        private readonly object connectionLock = new object();

        public sealed override IXenConnection Connection
        {
            protected set
            {
                // lock any changes to the connection
                // This only affect actions which run across multiple connections
                lock (connectionLock)
                {
                    base.Connection = value;
                }
            }
        }

        public Session Session
        {
            get { return _session; }
            set { _session = value; }
        }

        
        private delegate void SetXenCenterUUIDDelegate(Session session, string _task, string uuid);
        private delegate void SetAppliesToDelegate(Session session, string _task, List<string> applies_to);

        /// <summary>
        /// The XenAPI.Task object (if any) that corresponds to this action.
        /// </summary>
        public XenRef<Task> RelatedTask
        {
            get { return _relatedTask; }
            set
            {
                //Program.AssertOffEventThread();
                _relatedTask = value;
                if (_relatedTask != null && _session != null)
                {
                    DoWithSessionRetry(ref _session, (SetXenCenterUUIDDelegate)Task.SetXenCenterUUID, _relatedTask.opaque_ref, XenAdminConfigManager.Provider.XenCenterUUID);
                    DoWithSessionRetry(ref _session, (SetAppliesToDelegate)Task.SetAppliesTo, _relatedTask.opaque_ref, AppliesTo);
                    RecomputeCanCancel();
                }
            }
        }

        public override sealed bool CanCancel
        {
            get { return !cancelling && can_cancel; }
            protected set
            {
                if (can_cancel != value)
                {
                    can_cancel = value;
                    OnChanged();
                }
            }
        }

        public bool Cancelling
        {
            get { return cancelling; }
            protected set { cancelling = value; }
        }

        // You can't refer to property getters in order to treat them as a delegate, so this is
        // a substitute.
        public bool GetCancelling()
        {
            return cancelling;
        }

        public bool Cancelled
        {
            get { return cancelled; }
            protected set { cancelled = value; }
        }

        /// <summary>
        /// Check again whether this task may be cancelled.  Must be called from off the event thread,
        /// whereas CanCancel is called on it.
        /// </summary>
        public virtual void RecomputeCanCancel()
        {
            try
            {
                XenRef<Task> task = _relatedTask;
                if (task == null)
                {
                    can_cancel = false;
                    return;
                }

                Session local_session = GetCancelSession();
                if (local_session == null || string.IsNullOrEmpty(local_session.opaque_ref))
                {
                    can_cancel = false;
                    return;
                }
                CanCancel = Task.get_allowed_operations(local_session, task.opaque_ref).Contains(task_allowed_operations.cancel);
            }
            catch (Exception exn)
            {
                log.Error("Error recomputing whether the task can be cancelled.", exn);
                LogoutCancelSession();
                can_cancel = false;
            }
        }

        /// <summary>
        /// Will return null if Connection is null.
        /// </summary>
        /// <returns></returns>
        protected Session GetCancelSession()
        {
            lock (connectionLock)
            {
                if (Connection == null || !Connection.IsConnected)
                    return null;

                if (_cancel_session == null)
                {
                    if (_session == null)
                    {
                        _cancel_session = Connection.DuplicateSession(XenAdminConfigManager.Provider.ConnectionTimeout);
                    }
                    else if (_session.Url == null)  // DbProxy
                    {
                        return null;
                    }
                    else
                    {
                        _cancel_session = XenAdminConfigManager.Provider.CreateActionSession(_session, Connection);
                    }
                }

                return _cancel_session;
            }
        }

        protected void LogoutCancelSession()
        {
            lock (connectionLock)
            {
                _cancel_session = null;
            }
        }

        /// <summary>
        /// Cancels this action.
        /// 
        /// 1. Must be called on the event thread.
        /// 2. Will return if Cancelling = true
        /// 3. Runs RecomputeCanCancel() on a bg thread, then if CanCancel == true, sets Cancelling to true and runs Cancel_()
        /// </summary>
        public override sealed void Cancel()
        {
            log.Debug("Cancel() was called. Attempting to cancel action");

            // We can always cancel before the action starts running
            lock (_startedRunningLock)
            {
                if (!_startedRunning)
                {
                    cancelled = true;
                    new Thread(delegate()
                    {
                        AuditLogCancelled();
                        MarkCompleted(new CancelledException());
                        Clean();
                        CleanOnError();
                    }).Start();
                    return;
                }
            }

            lock (_cancellinglock)
            {
                if (Cancelling)
                    return;

                Cancelling = true;
            }

            Thread t = new Thread(DoCancel);
            t.Name = string.Format("Cancelling task {0}", Title);
            t.IsBackground = true;
            t.Priority = ThreadPriority.Lowest;
            t.Start();
        }

        private void DoCancel()
        {
            RecomputeCanCancel();
            if (can_cancel)
            {
                try
                {
                    CancelRelatedTask();
                }
                catch (Exception e)
                {
                    log.Debug($"Exception when cancelling action {Description}.", e);
                    LogoutCancelSession();
                    Cancelling = false;
                }
            }
        }

        /// <summary>
        /// Called by Cancel on a background thread, to do any heavy lifting.
        /// Creates a new Session and cancels RelatedTask.
        /// 
        ///   * Only called if RecomputeCanCancel sets CanCancel to true;
        ///   * Cancelling set to true before this call, but after call to RecomputeCanCancel and CanCancel.
        ///   * DO NOT CHECK CANCANCEL HERE, you will only be called with it as true, but it may change after.
        ///
        /// </summary>
        protected virtual void CancelRelatedTask()
        {
            PerformSilentTaskOp(delegate()
            {
                // Create a new session since this.Session may be in-use by the thread pool thread, and, in particular, TaskPoller
                Session local_session = GetCancelSession();

                XenRef<Task> r = RelatedTask;
                if (r != null)
                {
                    XenAPI.Task.cancel(local_session, r);
                }
            });
        }

        protected void PerformSilentTaskOp(Action f)
        {
            if (_relatedTask == null)
                return;
            try
            {
                f();
            }
            catch (XenAPI.Failure exn)
            {
                if (exn.ErrorDescription.Count > 1 &&
                    exn.ErrorDescription[0] == XenAPI.Failure.HANDLE_INVALID &&
                    exn.ErrorDescription[1] == "task")
                {
                    log.Debug("Task related failure (invalid handle)", exn);
                    // The task has disappeared.
                    _relatedTask = null;
                }
                else
                {
                    log.Error("Task related failure.", exn);
                    // Ignore, and hope that this isn't a problem.
                }
            }
            catch (Exception exn)
            {
                log.Error("Task related exception.", exn);
                // Ignore, and hope that this isn't a problem.
            }
        }

        private readonly object _cancellinglock = new object();

        // _startedRunningLock controls the case that two threads try and Cancel and Run an action
        // at the same time: one of them has to win and prevent the other.
        private object _startedRunningLock = new object();
        
        private bool _startedRunning = false;
        public bool StartedRunning
        {
            get { return _startedRunning; }
            protected set
            {
                lock(_startedRunningLock)
                {
                    _startedRunning = value;
                }
            }
        }
        

        /// <summary>
        /// If there has been an exception this code will always execute after the action has finished, use for tidyup
        /// </summary>
        protected virtual void CleanOnError() { }

        /// <summary>
        /// This code will always execute after the action has finished, use for tidyup
        /// </summary>
        protected virtual void Clean() { }

        public virtual Session NewSession()
        {
            if (Connection == null)
                return null;
            return Connection.DuplicateSession();
        }

        /// <summary>
        /// Overload for use by actions, using elevated credentials on the retry, if implemented in NewSession().
        /// Try and run the delegate.
        /// If it fails with a web exception or invalid session, try again.
        /// Only retry 60 times. 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="f"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public object DoWithSessionRetry(ref Session session, Delegate f, params object[] p)
        {
            int retries = 60;

            while (true)
            {
                try
                {
                    object[] ps = new object[p.Length + 1];

                    ps[0] = session;

                    for (int i = 0; i < p.Length; i++)
                    {
                        ps[i + 1] = p[i];
                    }

                    try
                    {
                        return f.DynamicInvoke(ps);
                    }
                    catch (TargetInvocationException exn)
                    {
                        throw exn.InnerException;
                    }
                }
                catch (XmlRpcNullParameterException xmlExcept)
                {
                    log.Error($"XmlRpcNullParameterException in DoWithSessionRetry, retry {retries}.", xmlExcept);
                    throw new Exception(Messages.INVALID_SESSION);
                }
                catch (XmlRpcIllFormedXmlException xmlRpcIllFormedXmlException)
                {
                    log.Error($"XmlRpcIllFormedXmlException in DoWithSessionRetry, retry {retries}.", xmlRpcIllFormedXmlException);

                    if (!Connection.ExpectDisruption || retries <= 0)
                        throw;
                }
                catch (WebException we)
                {
                    log.Error($"WebException in DoWithSessionRetry, retry {retries}.", we);

                    if (retries <= 0)
                        throw;
                }
                catch (Failure failure)
                {
                    log.Error($"Failure in DoWithSessionRetry, retry {retries}.", failure);

                    if (retries <= 0)
                        throw;

                    if (failure.ErrorDescription.Count < 1 || failure.ErrorDescription[0] != XenAPI.Failure.SESSION_INVALID)
                        throw;
                }

                Session newSession;

                try
                {
                    // try to create a new TCP stream to use, as the other one has failed us
                    newSession = NewSession();
                    session = newSession;
                }
                catch (DisconnectionException e)
                {
                    if (!Connection.ExpectDisruption)
                    {
                        //this was not expected, throw the d/c exception
                        throw e;
                    }
                    // We are expecting disruption on this connection. We need to wait for the hearbeat to recover.
                    // Though after 60 retries we will give up in the previous try catch block
                }
                catch
                {
                    // do nothing
                }



                retries--;

                Thread.Sleep(Connection.ExpectDisruption ? 500 : 100);
            }
        }
    }
}
