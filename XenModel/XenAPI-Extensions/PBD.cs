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
using System.Text.RegularExpressions;
using System.Threading;
using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Network;


namespace XenAPI
{
    partial class PBD
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static bool WaitForPlug(Session session, string pbdOpaqueRef)
        {
            int timeout = 120; //Wait 2 min for PBD to plug

            while (timeout > 0)
            {
                if (PBD.get_currently_attached(session, pbdOpaqueRef))
                    return true;
                Thread.Sleep(1000);
                timeout--;
            }

            return false;
        }

        public static void CheckPlugPBDsForVMs(IXenConnection connection, List<XenRef<VM>> vmRefs, bool ignoreFailure = false)
        {
            var pbds = new List<PBD>();

            foreach (XenRef<VM> vmRef in vmRefs)
            {
                VM vm = connection.TryResolveWithTimeout(vmRef);
                
                foreach (var vbdRef in vm.VBDs)
                {
                    var vbd = connection.Resolve(vbdRef);
                    if (vbd == null)
                        continue;

                    VDI vdi = connection.Resolve(vbd.VDI);
                    if (vdi == null)
                        continue;

                    SR sr = connection.Resolve(vdi.SR);
                    if (sr == null)
                        continue;

                    foreach (var pbdRef in sr.PBDs)
                    {
                        var pbd = connection.Resolve(pbdRef);
                        if (pbd != null && !pbds.Contains(pbd))
                            pbds.Add(pbd);
                    }
                }
            }

            foreach (PBD pbd in pbds)
            {
                Session session = pbd.Connection.DuplicateSession();

                log.DebugFormat("Waiting for PBDs {0} to become plugged", pbd.Name());
                // Wait 2 min for PBD to become plugged
                if (WaitForPlug(session, pbd.opaque_ref))
                    continue;

                // if it's still unplugged, try plugging it - this will probably
                // fail, but at least we'll get a better error message.
                try
                {
                    log.DebugFormat("Plugging PBD {0}", pbd.Name());
                    plug(session, pbd.opaque_ref);
                }
                catch (Exception e)
                {
                    log.Debug(string.Format("Error plugging PBD {0}", pbd.Name()), e);

                    if (!ignoreFailure)
                        throw;
                }
            }
        }

        public bool MultipathActive()
        {

            return BoolKey(other_config, "multipathed");
        }

        /// <summary>
        /// The number of iSCSI sessions in use, as advertised by the SR backend, or -1 if the value is missing.
        /// </summary>
        public int ISCSISessions()
        {
            return IntKey(other_config, "iscsi_sessions", -1);
        }

        private static readonly Regex multipathCountRegex = new Regex(@"\[(\d+)L?,\s(\d+)L?");

        public static bool ParsePathCounts(String input, out int currentPaths, out int maxPaths)
        {
            currentPaths = 0;
            maxPaths = 0;

            Match match = multipathCountRegex.Match(input);

            return match.Success && match.Groups.Count == 3 &&
                int.TryParse(match.Groups[1].Value, out currentPaths) &&
                int.TryParse(match.Groups[2].Value, out maxPaths);
        }

        /// <summary>
        /// The status of the PBD as a friendly string (unplugged, host down, connected)
        /// </summary>
        public string StatusString()
        {
            if (!currently_attached)
                return Messages.REPAIR_SR_DIALOG_UNPLUGGED;

            Host h = Connection.Resolve(host);
            if (h == null)
                return Messages.REPAIR_SR_DIALOG_UNPLUGGED;

            if (!h.IsLive())
                return Messages.HOST_NOT_LIVE;

            return Messages.CONNECTED;
        }

        public StorageLinkCredentials GetStorageLinkCredentials()
        {
            var deviceConfig = new Dictionary<string, string>(device_config);
            
            SR sr = Connection.Resolve<SR>(SR);
            
            if (sr != null && sr.type == "cslg")
            {
                string host, username, passwordSecret;

                deviceConfig.TryGetValue("target", out host);
                deviceConfig.TryGetValue("username", out username);
                deviceConfig.TryGetValue("password_secret", out passwordSecret);

                return new StorageLinkCredentials(Connection, host, username, null, passwordSecret);
            }
            return null;
        }
    }
}
