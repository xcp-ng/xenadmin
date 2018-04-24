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
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAPI;

namespace XenAdmin.Actions.Wlb
{
    public class WlbRetrieveRecommendationAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Dictionary<XenRef<VM>, string[]> WLBOptPoolRecommendations = new Dictionary<XenRef<VM>, string[]>();

        public WlbRetrieveRecommendationAction(Pool pool)
            : base(pool.Connection, string.Format(Messages.WLB_RETRIEVING_RECOMMENDATIONS, pool.Name()), true)
        {
            if (pool == null)
                throw new ArgumentNullException("pool");

            this.Pool = pool;

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("pool.retrieve_wlb_recommendations");
            #endregion
        }

        protected override void Run()
        {
            try
            {
                log.DebugFormat("Retrieving recommendations for pool {0}", Pool.Name());
                this.WLBOptPoolRecommendations = Pool.retrieve_wlb_recommendations(Session);
                log.DebugFormat("Success retrieving recommendations for pool {0}", Pool.Name());

                //Retrieving the recommendations was successful, so update the WlbServerState to report the current state
                //  This is here in case there was a previous communication error which has been fixed.
                if (WlbServerState.GetState(Pool) == WlbServerState.ServerState.ConnectionError || WlbServerState.GetState(Pool) == WlbServerState.ServerState.Unknown)
                {
                    if (Helpers.WlbEnabled(Pool.Connection))
                    {
                        WlbServerState.SetState(Pool, WlbServerState.ServerState.Enabled);
                    }
                    else
                    {
                        WlbServerState.SetState(Pool, WlbServerState.ServerState.Disabled);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is Failure)
                {
                    WlbServerState.SetState(Pool, WlbServerState.ServerState.ConnectionError, (Failure)ex);
                }
                log.Error(ex, ex);
            }
        }


    }
}
