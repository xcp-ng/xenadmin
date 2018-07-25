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
using System.Threading;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public abstract class RebootPlanAction : HostPlanAction
    {
        private bool _cancelled = false;

        protected RebootPlanAction(Host host)
            : base(host)
        {
        }

        public override void Cancel()
        {
            _cancelled = true;

            lock (this)
            {
                Monitor.PulseAll(this);
            }
        }

        private void RegisterConnectionLostEvent()
        {
            Connection.ConnectionLost -= connection_ConnectionLost;
            Connection.ConnectionLost += connection_ConnectionLost;
        }

        private void DeregisterConnectionLostEvent()
        {
            Connection.ConnectionLost -= connection_ConnectionLost;
        }

        protected void RestartAgent(ref Session session)
        {
            var hostObj = GetResolvedHost();
            AddProgressStep(string.Format(Messages.UPDATES_WIZARD_RESTARTING_AGENT, hostObj.Name()));
            WaitForReboot(ref session, Host.AgentStartTime, s => Host.async_restart_agent(s, HostXenRef.opaque_ref));
        }

        protected void RebootHost(ref Session session)
        {
            var hostObj = GetResolvedHost();
            AddProgressStep(string.Format(Messages.UPDATES_WIZARD_REBOOTING, hostObj.Name()));
            Connection.ExpectDisruption = true;
            try
            {
                WaitForReboot(ref session, Host.BootTime, s => Host.async_reboot(s, HostXenRef.opaque_ref));
                AddProgressStep(Messages.PLAN_ACTION_STATUS_RECONNECTING_STORAGE);
                foreach (var host in Connection.Cache.Hosts)
                    host.CheckAndPlugPBDs();  // Wait for PBDs to become plugged on all hosts
            }
            finally
            {
                Connection.ExpectDisruption = false;
            }
        }

        protected void WaitForReboot(ref Session session, Func<Session, string, double> metricDelegate, Action<Session> methodInvoker)
        {
            bool master = GetResolvedHost().IsMaster();

            _cancelled = false;
            double metric = metricDelegate(session, HostXenRef.opaque_ref);

            log.DebugFormat("{0}._WaitForReboot(master='{1}', metric='{2}')", GetType().Name, master, metric);

            PercentComplete = 10;

            RegisterConnectionLostEvent();
            try
            {
                methodInvoker(session);

                PercentComplete = 20;

                log.DebugFormat("{0}._WaitForReboot executed delegate...", GetType().Name);

                session = WaitForHostToStart(master, session, metricDelegate, metric);
            }
            finally
            {
                DeregisterConnectionLostEvent();
            }
        }

        private Session WaitForHostToStart(bool master, Session session, Func<Session, string, double> metricDelegate, double metric)
        {
            Connection.ExpectDisruption = true;

            try
            {
                if (master)
                {
                    Connection.SupressErrors = true;

                    //
                    // Wait for a dissconnection
                    //

                    WaitForDisconnection();

                    //
                    // Now, we need to wait for a reconnection
                    //
                    session = WaitReconnectToMaster(session);
                }

                PercentComplete = 60;

                // 
                // Now wait for boot time to be greater than it was before
                //
                WaitForBootTimeToBeGreaterThanBefore(master, session, metricDelegate, metric);

                log.DebugFormat("{0}._WaitForReboot done!", GetType().Name);
            }
            finally
            {
                Connection.SupressErrors = false;
            }
            return session;
        }

        private void WaitForDisconnection()
        {
            log.DebugFormat("{0}._WaitForReboot waiting for connection to go away...", GetType().Name);

            lock (this)
            {
                while (!lostConnection && !_cancelled)
                {
                    Monitor.Wait(this);
                }
            }

            log.DebugFormat("{0}._WaitForReboot connection went away...", GetType().Name);

            if (_cancelled)
                throw new CancelledException();

            PercentComplete = 40;

            Thread.Sleep(1000);
        }

        private void WaitForBootTimeToBeGreaterThanBefore(bool master, Session session, Func<Session, string, double> metricDelegate, double metric)
        {

            DateTime waitForMetric = DateTime.Now;
            int lastMetricDebug = 0;

            log.DebugFormat("{0}._WaitForReboot waiting for metric to update... (metric='{1}')", GetType().Name, metric);

            double currentMetric;
            do
            {
                currentMetric = metricDelegate(session, HostXenRef.opaque_ref);

                if (_cancelled)
                    throw new CancelledException();

                Thread.Sleep(1000);

                // Debug message once a minute

                int currMin = (int)((DateTime.Now - waitForMetric).TotalSeconds) / 60;
                if (currMin <= lastMetricDebug)
                    continue;

                lastMetricDebug = currMin;

                log.DebugFormat("{0}._WaitForReboot still waiting for metric after {1}... (old metric='{2}', current metric='{3}')",
                                GetType().Name, DateTime.Now - waitForMetric, metric, currentMetric);
            }
            while (metric == currentMetric);

            Thread.Sleep(1000);

            log.DebugFormat("{0}._WaitForReboot metric now up to date... (old metric='{1}', current metric='{2}')",
                            GetType().Name, metric, currentMetric);

            if (master)
            {
                //
                // Force a reconnect to prime the cache for the next actions
                //

                log.DebugFormat("{0}._WaitForReboot connecting up...", GetType().Name);

                Program.Invoke(Program.MainWindow, () => XenConnectionUI.BeginConnect(Connection, false, null, false));
            }
        }

        private Session WaitReconnectToMaster(Session session)
        {

            log.DebugFormat("{0}._WaitForReboot waiting for reconnect...", GetType().Name);

            DateTime waitForReconnect = DateTime.Now;
            int lastMinDebug = 0;

            do
            {
                if (_cancelled)
                    throw new CancelledException();

                try
                {
                    session = Connection.DuplicateSession();
                }
                catch (Exception e)
                {
                    session = null;
                    Thread.Sleep(5000);

                    // Debug message once a minute

                    int currMin = (int)((DateTime.Now - waitForReconnect).TotalSeconds) / 60;

                    if (currMin <= lastMinDebug)
                        continue;

                    lastMinDebug = currMin;

                    log.Debug(string.Format("{0}._WaitForReboot still waiting for connection after {1}...",
                        GetType().Name, DateTime.Now - waitForReconnect),e);
                }
            }
            while (session == null);

            log.DebugFormat("{0}._WaitForReboot reconnected after {1}...", GetType().Name, DateTime.Now - waitForReconnect);
            return session;
        }

        private bool lostConnection = false;

        private void connection_ConnectionLost(object sender, EventArgs e)
        {
            lock (this)
            {
                lostConnection = true;
                Monitor.PulseAll(this);
            }
        }
    }
}