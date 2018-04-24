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
using XenAdmin.Actions;
using XenAdmin.Actions.Wlb;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAPI;


namespace XenAdmin.Actions.HostActions
{
    public class HostPowerOnAction : AsyncAction
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public HostPowerOnAction(XenAPI.Host host)
            : base(host.Connection, Messages.HOST_POWER_ON)
        {
            Host = host;
            AddCommonAPIMethodsToRoleCheck();

            ApiMethodsToRoleCheck.Add("pool.send_wlb_configuration");
            ApiMethodsToRoleCheck.Add("host.power_on");
        }

        protected override void Run()
        {
            bool succeeded = false;
            string name = Helpers.GetName(Host);
            XenAPI.Host master = Helpers.GetMaster(Connection);
            AppliesTo.Add(master.opaque_ref);
            Title = string.Format(Messages.ACTION_HOST_START_TITLE, name);
            Description = Messages.ACTION_HOST_STARTING;
            try
            {
                XenAPI.Host.power_on(Session, Host.opaque_ref);
                Description = Messages.ACTION_HOST_STARTED;
                succeeded = true;

                /* WLB: Below code doesn't work, becasue RelatedTask is not set. 
                 *      Need to explore other option when enabling set poweron task value for wlb reporting
                if (Helpers.IsWLBEnabled(this.Connection)
                    && Host.other_config.ContainsKey(WlbOptimizePoolAction.OPTIMIZINGPOOL))
                {
                    // set host poweroff task key values for wlb reporting purpose
                    Task.add_to_other_config(this.Session, this.RelatedTask.opaque_ref, "wlb_advised", Host.other_config[WlbOptimizePoolAction.OPTIMIZINGPOOL]);
                    Task.add_to_other_config(this.Session, this.RelatedTask.opaque_ref, "wlb_action", "host_poweron");
                    Task.add_to_other_config(this.Session, this.RelatedTask.opaque_ref, "wlb_action_obj_ref", Host.opaque_ref);
                    Task.add_to_other_config(this.Session, this.RelatedTask.opaque_ref, "wlb_action_obj_type", "host");
                }
                */
            }
            catch (Exception e)
            {
                Failure f = e as Failure;
                if (f != null)
                {
                    string msg = f.ErrorDescription.Count > 2 ? Messages.ResourceManager.GetString(f.ErrorDescription[2]) : null;
                    if (msg != null)
                        throw new Exception(msg);
                    else
                    {
                        throw new Exception(string.Format(Messages.POWER_ON_REQUEST_FAILED, this.Host));
                    }
                }
                throw;
            }
            finally
            {
                if (Helpers.WlbConfigured(this.Connection) && Helpers.WlbEnabledAndConfigured(this.Connection))
                {
                    UpdateHostLastPowerOnSucceeded(succeeded, Host);
                }
            }
        }

        /// <summary>
        /// Attempts to set the LastPowerOnSucceeded flag in the WLB Host configuration
        /// </summary>
        private void UpdateHostLastPowerOnSucceeded(bool succeeded, XenAPI.Host host)
        {
            try
            {
                //Helpers.SetOtherConfig(this.Host.Connection.Session, this.Host, "LastPowerOnsucceeded", successful.ToString());
                WlbHostConfiguration hostConfig = new WlbHostConfiguration(host.uuid);
                hostConfig.LastPowerOnSucceeded = succeeded;
                if (!succeeded)
                {
                    hostConfig.ParticipatesInPowerManagement = false;
                }
                XenAPI.Pool pool = Helpers.GetPoolOfOne(host.Connection);
                if (null != pool)
                {
                    SendWlbConfigurationAction action = new SendWlbConfigurationAction(pool, hostConfig.ToDictionary(), SendWlbConfigurationKind.SetHostConfiguration);
                    action.RunExternal(Session);
                }
                else
                {
                    throw new Failure(Failure.INTERNAL_ERROR, Messages.POOL_GONE);
                }
            }
            catch (Exception ex)
            {
                log.Error("Unable to set the host's LastPowerOnSucceeded status.", ex);
            }
        }
    }
}
