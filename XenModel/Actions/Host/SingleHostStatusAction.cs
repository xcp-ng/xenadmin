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
using XenAdmin.Core;
using XenAPI;
using XenCenterLib;
using XenCenterLib.Archive;

namespace XenAdmin.Actions
{
    public class SingleHostStatusAction :  StatusReportAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly Host host;
        private readonly string[] capabilityKeys;
        private readonly long size;
        private readonly string[] RBAC_FAIL_STRINGS = {"HTTP", "403", "Forbidden"};

    public SingleHostStatusAction(Host host, long size, List<string> capabilityKeys, string path, string time)
            : base(host.Connection, string.Format(Messages.ACTION_SYSTEM_STATUS_COMPILING, Helpers.GetName(host)), path, time)
        {
            this.host = host;
            this.capabilityKeys = capabilityKeys.ToArray();
            this.size = size;
        }

        public long DataTransferred;

        protected override void Run()
        {
            Status = ReportStatus.compiling;

            string hostname = Helpers.GetName(host);
            hostname = TarSanitization.SanitizeTarPathMember(hostname);
            if (hostname.Length > 20)
                hostname = hostname.Truncate(20);

            string filename = string.Format("{1}\\{2}-bugtool-{0}.tar", hostname, filePath, timeString);

            string entries_string = string.Join(",", capabilityKeys);

            log.DebugFormat("Getting system status for {0} on {1}", entries_string, hostname);

            try
            {
                if (Session == null)
                    throw new Exception(Messages.CONNECTION_IO_EXCEPTION);

                HTTPHelper.Get(this, false, dataRxDelegate, filename, host.address,
                    (HTTP_actions.get_ssss)HTTP_actions.get_system_status,
                    Session.opaque_ref, entries_string, "tar");

                log.DebugFormat("Getting system status from {0} successful", hostname);

                Status = ReportStatus.succeeded;
                Description = Messages.COMPLETED;
                PercentComplete = 100;
            }
            catch (HTTP.CancelledException)
            {
                throw new CancelledException();
            }
            catch (CancelledException ce)
            {
                log.Info("Getting system status cancelled");
                Status = ReportStatus.cancelled;
                Error = ce;
                Description = Messages.ACTION_SYSTEM_STATUS_CANCELLED;
                throw;
            }
            catch (Exception e)
            {
                log.Error(string.Format("Getting system status from {0} failed", hostname), e);

                Status = ReportStatus.failed;
                Error = e;

                if (Win32.GetHResult(e) == Win32.ERROR_DISK_FULL)
                {
                    Description = Messages.ACTION_SYSTEM_STATUS_DISK_FULL;
                    return;
                }

                if (!string.IsNullOrEmpty(Error.Message) && RBAC_FAIL_STRINGS.All(s => Error.Message.Contains(s)))
                {
                    var roles = Host.Connection.Session.Roles;
                    roles.Sort();
                    Description = string.Format(Messages.BUGTOOL_RBAC_FAILURE, roles[0].FriendlyName());
                    return;
                }

                Description = Messages.BUGTOOL_REPORTSTATUS_FAILED;
            }
        }

        private void dataRxDelegate(long rxd)
        {
            Status = ReportStatus.downloading;
            DataTransferred = rxd;

            if (Cancelling)
                throw new CancelledException();

            //although the parent class's PercentComplete setter does correct out of range values,
            //it throws an assertion to catch potentially erroneous percentage calculations;
            //here, however, the error is expected because the MaxSize is only an estimate,
            //hence the assertion is pointless, hence correct the value before assigning it to PercentComplete 

            PercentComplete = rxd < size ? (int)(rxd * 100L / size) : 100;
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = !Cancelling;
        }
    }
}
