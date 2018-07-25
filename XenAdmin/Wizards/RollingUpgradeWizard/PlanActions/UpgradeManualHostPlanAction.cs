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

using XenAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using Timer = System.Timers.Timer;


namespace XenAdmin.Wizards.RollingUpgradeWizard.PlanActions
{
    public abstract class UpgradeHostPlanAction : RebootPlanAction
    {
        protected bool rebooting;
        private readonly Timer timer;
        protected readonly Control invokingControl;
        
        protected UpgradeHostPlanAction(Host host, Control invokingControl)
            : base(host)
        {
            timer = new Timer { Interval = 20 * 60000, AutoReset = true };
            timer.Elapsed += timer_Elapsed;
            this.invokingControl = invokingControl;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!rebooting)
                return;

            Program.Invoke(invokingControl, () =>
            {
                using (var dialog = new NonModalThreeButtonDialog(SystemIcons.Exclamation,
                    Messages.ROLLING_UPGRADE_TIMEOUT, Messages.ROLLING_POOL_UPGRADE,
                    Messages.KEEP_WAITING_BUTTON_LABEL, Messages.CANCEL))
                {
                    if (dialog.ShowDialog(invokingControl) != DialogResult.OK)
                        Cancel();
                }
            });
        }

        protected void Upgrade(ref Session session)
        {
            try
            {
                var hostObj = GetResolvedHost();

                if (hostObj.enabled)
                {
                    log.DebugFormat("Disabling host {0}", hostObj.Name());
                    AddProgressStep(string.Format(Messages.UPDATES_WIZARD_ENTERING_MAINTENANCE_MODE, hostObj.Name()));
                    Host.disable(session, HostXenRef.opaque_ref);
                }

                timer.Start();
                rebooting = true;

                log.DebugFormat("Upgrading host {0}", hostObj.Name());
                AddProgressStep(string.Format(Messages.PLAN_ACTION_STATUS_INSTALLING_XENSERVER, hostObj.Name()));

                log.DebugFormat("Waiting for host {0} to reboot", hostObj.Name());
                WaitForReboot(ref session, Host.BootTime, s => Host.async_reboot(s, HostXenRef.opaque_ref));

                AddProgressStep(Messages.PLAN_ACTION_STATUS_RECONNECTING_STORAGE);
                foreach (var host in Connection.Cache.Hosts)
                    host.CheckAndPlugPBDs(); // Wait for PBDs to become plugged on all hosts

                rebooting = false;
                log.DebugFormat("Host {0} rebooted", hostObj.Name());
            }
            finally
            {
                Connection.ExpectDisruption = false;
                timer.Stop();
            }
        }
    }

    public class UpgradeManualHostPlanAction : UpgradeHostPlanAction
    {
        public UpgradeManualHostPlanAction(Host host, Control invokingControl)
            : base(host, invokingControl)
        {
        }

        protected override void RunWithSession(ref Session session)
        {
            //Show dialog prepare host boot from CD or PXE boot and click OK to reboot

            Program.Invoke(invokingControl, () =>
            {
                using (var dialog = new NonModalThreeButtonDialog(SystemIcons.Information,
                    string.Format(Messages.ROLLING_UPGRADE_REBOOT_MESSAGE, GetResolvedHost().Name()),
                    Messages.ROLLING_POOL_UPGRADE, Messages.REBOOT, Messages.SKIP_SERVER))
                {
                    if (dialog.ShowDialog(invokingControl) != DialogResult.OK) // Cancel or Unknown
                    {
                        if (GetResolvedHost().IsMaster())
                        {
                            Error = new ApplicationException(Messages.EXCEPTION_USER_CANCELLED_MASTER);
                            throw Error;
                        }

                        Error = new CancelledException();
                    }
                }
            });

            if (Error != null)
                return;

            var hostObj = GetResolvedHost();
            string beforeRebootProductVersion = hostObj.LongProductVersion();
            string hostName = hostObj.Name();

            do
            {
                if (Cancelling)
                    break;

                Upgrade(ref session);

                hostObj = GetResolvedHost();
                if (Helpers.SameServerVersion(hostObj, beforeRebootProductVersion))
                {
                    var obj = hostObj;
                    Program.Invoke(invokingControl, () =>
                    {
                        using (var dialog = new NonModalThreeButtonDialog(SystemIcons.Exclamation,
                            string.Format(Messages.ROLLING_UPGRADE_REBOOT_AGAIN_MESSAGE, hostName),
                            Messages.ROLLING_POOL_UPGRADE,
                            Messages.REBOOT_AGAIN_BUTTON_LABEL, Messages.SKIP_SERVER))
                        {
                            if (dialog.ShowDialog(invokingControl) == DialogResult.OK)
                                return;
                            
                            if (obj.IsMaster())
                            {
                                Error = new Exception(Messages.HOST_REBOOTED_SAME_VERSION);
                                throw Error;
                            }
                            Error = new CancelledException();
                            Cancel();
                        }
                    });
                }
                else
                {
                    break;
                }
            } while (true);
        }
    }
}
