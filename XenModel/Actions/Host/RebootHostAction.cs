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
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public class RebootHostAction : HostAbstractAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// For all but HostActionKind.Evacuate.
        /// 
        /// NOTE: when creating new HostActions, add Program.MainWindow.action_Completed to the completed event,
        /// and call Program.MainWindow.UpdateToolbars() after starting the action. This ensures the toolbar
        /// buttons are disabled while the action is in progress.
        /// </summary>
        public RebootHostAction(Host host, Func<HostAbstractAction, Pool, long, long, bool> acceptNTolChanges)
            : base(host.Connection, Messages.HOST_REBOOTING, Messages.WAITING, acceptNTolChanges, null)
        {
            if (host == null)
                throw new ArgumentNullException("host");
            this.Host = host;
            AddCommonAPIMethodsToRoleCheck();
            ApiMethodsToRoleCheck.Add("pool.ha_compute_hypothetical_max_host_failures_to_tolerate");
            ApiMethodsToRoleCheck.Add("pool.set_ha_host_failures_to_tolerate");

            ApiMethodsToRoleCheck.Add("vm.clean_shutdown");

            ApiMethodsToRoleCheck.Add("host.disable");
            ApiMethodsToRoleCheck.Add("host.enable");
            ApiMethodsToRoleCheck.Add("host.reboot");

        }

        protected override void Run()
        {
            bool wasEnabled = Host.enabled;
            this.Description = string.Format(Messages.ACTION_HOST_REBOOTING, Helpers.GetName(Host));

            MaybeReduceNtolBeforeOp(HostActionKind.Reboot);
            ShutdownVMs(true);

            try
            {
                RelatedTask = Host.async_reboot(Session, Host.opaque_ref);
                PollToCompletion(95, 100);
            }
            catch (Exception e)
            {
                log.Error("Exception rebooting host", e);
                try
                {
                    // Try to re-enable the host
                    if (wasEnabled)
                        Host.enable(Session, Host.opaque_ref);
                }
                catch (Exception e2)
                {
                    log.Error("Exception trying to re-enable host after error rebooting Host", e2);
                }

                var f = e as Failure;
                if (f != null && f.ErrorDescription != null && f.ErrorDescription.Count > 1 &&
                    f.ErrorDescription[0] == Failure.VM_FAILED_SHUTDOWN_ACKNOWLEDGMENT)
                {
                    var vm = Connection.Resolve(new XenRef<VM>(f.ErrorDescription[1]));
                    if (vm != null)
                    {
                        log.ErrorFormat("VM {0} (uuid {1}) did not acknowledge the need to shut down", vm.Name(), vm.uuid);
                        throw new Failure(string.Format(Messages.ACTION_REBOOT_HOST_VM_SHUTDOWN_ACK, vm.Name()));
                    }
                }
                throw;
            }

            // Close the IXenConnection if it is not to a pool, or is to the master of a pool
            if (Helpers.HostIsMaster(Host))
            {
                Host.Connection.Interrupt();
            }

            this.Description = string.Format(Messages.ACTION_HOST_REBOOTED, Helpers.GetName(Host));
        }

    }
}