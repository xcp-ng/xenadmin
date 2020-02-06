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

using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.Wlb
{
    public class DisableWLBAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _deconfigure = false;
        private static string OPTIMIZINGPOOL = "wlb_optimizing_pool";

        public DisableWLBAction(Pool pool, bool deconfigure)
            : base(pool.Connection, string.Format(Messages.DISABLING_WLB_ON, Helpers.GetName(pool).Ellipsise(50)), Messages.DISABLING_WLB, false)
        {
            this.Pool = pool;
            this._deconfigure = deconfigure;

            if (deconfigure)
            {
                this.Title = String.Format(Messages.DECONFIGURING_WLB_ON, Helpers.GetName(pool).Ellipsise(50));
                this.Description = Messages.DECONFIGURING_WLB;
            }

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("pool.set_wlb_enabled");
            ApiMethodsToRoleCheck.Add("pool.deconfigure_wlb");
            #endregion
        }

        protected override void Run()
        {
            if (_deconfigure)
            {
                try
                {
                    if (!Helpers.WlbEnabled(Pool.Connection))
                    {
                        log.Debug("Resuming WLB (prior to disconnecting) for pool " + Pool.Name());
                        XenAPI.Pool.set_wlb_enabled(this.Session, Pool.opaque_ref, true);
                        log.Debug("Success resuming WLB on pool " + Pool.Name());
                    }
                    log.Debug("Disconnecting Workload Balancing from pool " + Pool.Name() + " and removing all pool data");
                    XenAPI.Pool.deconfigure_wlb(this.Session);
                    log.Debug("Success disconnecting Workload Balancing on pool " + Pool.Name());
                    this.Description = Messages.COMPLETED;

                    WlbServerState.SetState(this.Session, Pool, WlbServerState.ServerState.NotConfigured);
                }
                catch (Exception ex)
                {
                    //Force disabling of WLB
                    XenAPI.Pool.set_wlb_enabled(this.Session, Pool.opaque_ref, false);
                    WlbServerState.SetState(this.Session, Pool, WlbServerState.ServerState.ConnectionError, (Failure)ex);
                    log.Debug($"Disconnecting  Workload Balancing failed on pool {Pool.Name()}. Workload Balancing has been paused.", ex);
                    throw new Exception(string.Format(Messages.ACTION_WLB_DECONFIGURE_FAILED, Pool.Name(), ex.Message));
                }
                finally
                {
                    //Clear the Optimizing Pool flag in case it was left behind
                    Helpers.SetOtherConfig(this.Session, this.Pool, OPTIMIZINGPOOL, string.Empty);
                }
            }
            else
            {
                try
                {
                    log.Debug("Pausing Workload Balancing on pool " + Pool.Name());
                    XenAPI.Pool.set_wlb_enabled(this.Session, Pool.opaque_ref, false); 
                    log.Debug("Success pausing Workload Balancing on pool " + Pool.Name());
                    this.Description = Messages.COMPLETED;
                    WlbServerState.SetState(this.Session, Pool, WlbServerState.ServerState.Disabled);
                }
                catch (Exception ex)
                {
                    if (ex is Failure)
                    {
                        WlbServerState.SetState(this.Session, Pool, WlbServerState.ServerState.ConnectionError, (Failure)ex);
                    }
                    throw ex;
                }
                finally
                {
                    //Clear the Optimizing Pool flag in case it was left behind
                    Helpers.SetOtherConfig(this.Session, this.Pool, OPTIMIZINGPOOL, string.Empty);
                }
            }

        }

    }
}
