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
using System.Text;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public class RestartHostPlanAction : RebootPlanAction
    {
        private readonly List<XenRef<VM>> _vms;
        private readonly bool _enableOnly;
        private readonly bool _restartAgentFallback;

        public RestartHostPlanAction(Host host, List<XenRef<VM>> vms, bool enableOnly = false, bool restartAgentFallback = false)
            : base(host, string.Empty)
        {
            _vms = vms;
            Visible = false;
            _enableOnly = enableOnly;
            _restartAgentFallback = restartAgentFallback;
        }

        protected override void RunWithSession(ref Session session)
        {
            var hostObj = GetResolvedHost();

            if (Helpers.ElyOrGreater(hostObj))
            {
                log.DebugFormat("Checking host.patches_requiring_reboot now on '{0}'.", hostObj);

                if (hostObj.updates_requiring_reboot.Count > 0)
                {
                    log.DebugFormat("Found {0} patches requiring reboot (live patching failed)."
                                    + "Initiating evacuate-reboot-bringbabiesback process.",
                        hostObj.updates_requiring_reboot.Count);
                }
                else if (_restartAgentFallback)
                {
                    log.Debug("Live patching succeeded. Restarting agent.");
                    Visible = true;
                    Title = ProgressDescription = string.Format(Messages.UPDATES_WIZARD_RESTARTING_AGENT, hostObj.Name());
                    WaitForReboot(ref session, Host.AgentStartTime, s => Host.async_restart_agent(s, HostXenRef.opaque_ref));
                    return;
                }
                else
                {
                    log.Debug("Did not find patches requiring reboot (live patching succeeded)."
                              + " Skipping scheduled restart.");
                    return;
                }
            }
            Visible = true;
            var sb = new StringBuilder();

            sb.Append(string.Format(Messages.PLANACTION_VMS_MIGRATING, hostObj.Name()));
            ProgressDescription = sb.ToString();

            EvacuateHost(ref session);

            sb.AppendLine(Messages.DONE);
            sb.Append(string.Format(Messages.UPDATES_WIZARD_REBOOTING, hostObj.Name()));
            ProgressDescription = sb.ToString();

            RebootHost(ref session);

            sb.AppendLine(Messages.DONE);
            sb.Append(string.Format(Messages.UPDATES_WIZARD_EXITING_MAINTENANCE_MODE, hostObj.Name()));
            ProgressDescription = sb.ToString();

            BringBabiesBack(ref session, _vms, _enableOnly);
        }
    }
}
