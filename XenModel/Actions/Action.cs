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
using System.Text;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;

namespace XenAdmin.Actions
{
    public class ActionBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Title;

        /// <summary>
        /// The client-side, local time that this action started.
        /// </summary>
        public DateTime Started = DateTime.Now;

        /// <summary>
        /// The client-side, local time that this action finished.
        /// </summary>
        public DateTime Finished;

        // Used for pairing incoming MessageActions with the action that triggered them

        private Pool _pool;
        private Host _host;
        private VM _vM;
        private SR _sR;
        private VM _template;

        private IXenConnection connection;
        public virtual IXenConnection Connection
        {
            get{ return connection; }
            protected set { connection = value; }
        }

        /// <summary>
        /// Indicates whether XenCenter can exit safely while a task is still in
        /// progress. If it's not safe, the MainWindow will try to cancel the
        /// running task on exit. If it is safe, the task will be left to complete
        /// after XenCenter has exited.
        /// Use this for composite tasks when an action has more than one xapi call.
        /// </summary>
        private bool safeToExit = true;
        public bool SafeToExit
        {
            get { return safeToExit; }
            protected set { safeToExit = value; }
        }

        /// <summary>
        /// A list of opaque_refs, giving all the objects that this action applies to.
        /// Pool, Host, VM, SR, and Template will be added to this list automatically.
        /// </summary>
        public readonly List<string> AppliesTo = new List<string>();

        public Pool Pool
        {
            get
            {
                return _pool;
            }
            set
            {
                _pool = value;
                SetAppliesTo(Pool);
                if (Pool != null && Pool.Connection != null &&
                    Helpers.GetPool(Pool.Connection) == null &&
                    Pool.Connection.Cache.Hosts.Length == 1)
                {
                    SetAppliesTo(Pool.Connection.Cache.Hosts[0]);
                }
            }
        }

        public Host Host
        {
            get
            {
                return _host;
            }
            set
            {
                _host = value;
                SetAppliesTo(Host);
            }
        }

        public VM VM
        {
            get
            {
                return _vM;
            }
            set
            {
                _vM = value;

                if (value == null)
                    return;

                if (_vM.is_a_snapshot)
                {
                    VM parentVM = _vM.Connection.Resolve<VM>(_vM.snapshot_of);
                    SetAppliesTo(parentVM);
                }
                else
                    SetAppliesTo(VM);
            }
        }

        public SR SR
        {
            get
            {
                return _sR;
            }
            set
            {
                _sR = value;
                SetAppliesTo(SR);

                if (Host == null)
                    Host = SR.Home();
            }
        }

        public VM Template
        {
            get
            {
                return _template;
            }
            set
            {
                _template = value;
                SetAppliesTo(Template);
            }
        }

        protected void SetAppliesTo(IXenObject xo)
        {
            if (xo == null)
                return;

            if (xo is Pool)
            {
                Pool pool = (Pool)xo;
                AppliesTo.Add(pool.opaque_ref);
            }
            else if (xo is Host)
            {
                Host host = (Host)xo;
                SetAppliesTo(Helpers.GetPoolOfOne(host.Connection));
                AppliesTo.Add(host.opaque_ref);
            }
            else if (xo is VM)
            {
                VM vm = (VM)xo;
                SetAppliesTo(vm.Home());
                AppliesTo.Add(vm.opaque_ref);
            }
            else if (xo is SR)
            {
                SR sr = (SR)xo;
                SetAppliesTo(sr.Home());
                AppliesTo.Add(sr.opaque_ref);
            }
            else if (xo is VDI)
            {
                VDI vdi = (VDI)xo;
                SetAppliesTo(vdi.Connection.Resolve(vdi.SR));
            }
        }

        private string _description;
        private bool _isCompleted = false;
        private int _percentComplete = 0;
        private Exception _exception = null;

        #region Events
        public event Action<ActionBase> Changed;
        public event Action<ActionBase> Completed;
        public static event Action<ActionBase> NewAction;
        #endregion

        public bool LogDescriptionChanges = true;

        private bool showProgress = true;

        public bool ShowProgress
        {
            get
            {
                return showProgress;
            }
            set
            {
                showProgress = value;
            }
        }

        public ActionBase(string title, string description, bool suppressHistory)
            : this(title, description, suppressHistory, false)
        {

        }

        public ActionBase(string title, string description, bool suppressHistory, bool completeImmediately)
            : this(title, description, suppressHistory, completeImmediately, null)
        {}

        public ActionBase(string title, string description, bool suppressHistory, bool completeImmediately, string error)
        {
            Title = title;
            _description = description;
            log.Debug(_description);
            if (completeImmediately)
            {
                if (!string.IsNullOrEmpty(error))
                    _exception = new Exception(error);

                Finished = DateTime.Now;
                _percentComplete = 100;
                _isCompleted = true;
            }
            if (NewAction != null && !suppressHistory)
                NewAction(this);
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    if (LogDescriptionChanges)
                        log.Debug(_description);
                    OnChanged();
                }
            }
        }

        public bool IsCompleted
        {
            get { return _isCompleted; }
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnChanged();
                    OnCompleted();
                }
            }
        }

        public virtual int PercentComplete
        {
            get { return _percentComplete; }
            set
            {
                if (_percentComplete != value)
                {
                    System.Diagnostics.Debug.Assert(0 <= value && value <= 100, string.Format("value percent is {0}", value));

                    var percent = value;
                    if (percent < 0)
                        percent = 0;
                    else if (percent > 100)
                        percent = 100;

                    _percentComplete = percent;
                    OnChanged();
                }
            }
        }

        public bool SuppressProgressReport { get; set; }

        public void Tick(int percent, string description)
        {
            if (_percentComplete != percent || _description != description)
            {
                _description = description;

                if (percent < 0)
                    percent = 0;
                else if (percent > 100)
                    percent = 100;
                _percentComplete = percent;

                OnChanged();
            }
        }

        public bool Succeeded
        {
            get { return this.IsCompleted && this.Exception == null; }
        }

        public Exception Exception
        {
            get { return _exception; }
            protected set
            {
                _exception = value;
                OnChanged();
            }
        }

        public void SetObject(IXenObject model)
        {
            if (model is Pool)
                Pool = (Pool)model;
            else if (model is Host)
                Host = (Host)model;
            else if (model is VM)
                VM = (VM)model;
            else if (model is SR)
                SR = (SR)model;
            else if (model is VDI)
            {
                VDI vdi = (VDI)model;
                SetObject(vdi.Connection.Resolve(vdi.SR));
            }
            else if (model is VBD)
            {
                VBD vbd = (VBD)model;
                SetObject(vbd.Connection.Resolve(vbd.VM));
            }

        }

        public virtual bool CanCancel
        {
            get { return false; }
            protected set { throw new InvalidOperationException(); }
        }

        public virtual void Cancel()
        {
            throw new InvalidOperationException();
        }

        protected void OnChanged()
        {
            if (Changed != null)
                try
                {
                    if (!SuppressProgressReport)
                        Changed(this);
                }
                catch (Exception e)
                {
                    log.Debug(String.Format("Exception firing OnChanged for Action {0}", Title), e);
                    log.Debug(e, e);
                }
        }

        protected virtual void OnCompleted()
        {
            if (Completed != null)
            {
                try
                {
                    Completed(this);
                }
                catch (Exception ex)
                {
                    log.Debug(String.Format("Exception firing OnCompleted for Action {0}", Title), ex);
                    log.Debug(ex, ex);
                }
            }
        }

        protected void MarkCompleted()
        {
            MarkCompletedCore();
        }

        protected void MarkCompleted(Exception e)
        {
            log.Debug(e, e);
            Exception = e;
            MarkCompletedCore();
        }

        private void MarkCompletedCore()
        {
            Finished = DateTime.Now;
            PercentComplete = 100;
            IsCompleted = true;
        }

        #region Audit logging

        protected static readonly log4net.ILog AuditLog = log4net.LogManager.GetLogger("Audit");
        
        protected virtual void AuditLogStarted()
        {
            AuditLog.InfoFormat("Operation started: {0}", AuditDescription());
        }

        protected virtual void AuditLogSuccess()
        {
            AuditLog.InfoFormat("Operation success: {0}", AuditDescription());
        }

        protected virtual void AuditLogFailure()
        {
            AuditLog.WarnFormat("Operation failure: {0}", AuditDescription());
        }

        protected virtual void AuditLogCancelled()
        {
            AuditLog.InfoFormat("Operation cancelled: {0}", AuditDescription());
        }

        protected virtual string AuditDescription()
        {
            return string.Format("{0}: {1}: {2}{3}", GetType().Name,
                DescribeConnection(), DescribeObject(), Description);
        }

        protected virtual string DescribeConnection()
        {
            if(Connection == null)
                return "Connection unknown";

            return Helpers.GetName(Connection);
        }

        protected virtual string DescribeObject()
        {
            StringBuilder sb = new StringBuilder();
            if (VM != null)
            {
                sb.Append(DescribeVM(VM));
            }
            if (Pool != null)
            {
                sb.Append(DescribePool(Pool));
            }
            if (Host != null)
            {
                sb.Append(DescribeHost(Host));
            }
            return sb.ToString();
        }

        protected virtual string DescribeVM(VM v)
        {
            return string.Format("VM {0} ({1}): ", v.uuid, v.Name());
        }

        protected virtual string DescribePool(Pool p)
        {
            return string.Format("Pool {0} ({1}): ", p.uuid, p.Name());
        }

        protected virtual string DescribeHost(Host h)
        {
            return string.Format("Host {0} ({1}): ", h.uuid, h.Name());
        }
        #endregion
    }
}
