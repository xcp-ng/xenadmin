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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public class SrRepairAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<Host> _hostList = new List<Host>();
        private Failure failure;
        private string failureDescription;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sr">Must not be null.</param>
        public SrRepairAction(IXenConnection connection, SR sr,bool isSharedAction)
            : base(connection, isSharedAction ? string.Format(Messages.ACTION_SR_SHARING, sr.NameWithoutHost()) : string.Format(Messages.ACTION_SR_REPAIRING, sr.NameWithoutHost()))
        {
            if (sr == null)
                throw new ArgumentNullException("sr");
            this.isSharedAction = isSharedAction;
            this.SR = sr;

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("pbd.plug");
            ApiMethodsToRoleCheck.Add("pbd.create");
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);
            #endregion
        }

        private readonly bool isSharedAction = false;

        protected override void Run()
        {
            log.Debug("Running SR repair");
            log.DebugFormat("SR='{0}'", SR.Name());

            //CA-176935, CA-173497 - we need to run Plug for the master first - creating a new list of hosts where the master is always first
            var allHosts = new List<Host>();
            
            var master = Helpers.GetMaster(Connection);
            if (master != null)
                allHosts.Add(master);
            
            foreach (var host in Connection.Cache.Hosts)
                if (!allHosts.Contains(host))
                    allHosts.Add(host);

            foreach (Host host in allHosts)
            {
                Host stoHost = SR.GetStorageHost();
                if (SR.shared
                    || (stoHost != null && host.opaque_ref == stoHost.opaque_ref))
                {
                    _hostList.Add(host);
                }
            }

            if (_hostList.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _hostList.Count; i++)
            {
                log.DebugFormat("_hostList[{0}]='{1}'", i, _hostList[i].Name());
            }

            int max = _hostList.Count * 2;
            int delta = 100 / max;

            try
            {
                if (SR.GetSRType(true) == SR.SRTypes.gfs2)
                {
                    var cluster = Connection.Cache.Clusters.FirstOrDefault();
                    if (cluster != null)
                    {
                        Cluster.pool_resync(Session, cluster.opaque_ref);
                    }
                }
            }

            catch (Exception e)
            {
                log.DebugFormat("Cluster pool resync failed with {0}", e.Message);
            }

            foreach (Host host in _hostList)
            {
                if (!host.HasPBDTo(SR) && SR.shared && SR.PBDs.Count > 0)
                {
                    PBD template = SR.Connection.Resolve(SR.PBDs[0]);
                    if (template != null)
                    {
                        this.Description = string.Format(Messages.ACTION_SR_REPAIR_CREATE_PBD, Helpers.GetName(host));
                        log.Debug(this.Description);

                        var newPbd = new PBD
                        {
                            currently_attached = false,
                            device_config = new Dictionary<string, string>(template.device_config),
                            SR = template.SR,
                            host = new XenRef<Host>(host.opaque_ref)
                        };

                        try
                        {
                            RelatedTask = XenAPI.PBD.async_create(this.Session, newPbd);

                            if (PercentComplete + delta <= 100)
                            {
                                PollToCompletion(PercentComplete, PercentComplete + delta);
                            }
                            else
                            {
                                PollToCompletion(PercentComplete, 100);
                                PercentComplete = 100;
                            }
                        }
                        catch (XenAPI.Failure f)
                        {
                            failure = f;
                            failureDescription = Description;
                        }
                    }
                }
                else
                {
                    PercentComplete += delta;
                }

                PBD thePBD = host.GetPBDTo(SR);
                if (thePBD != null && !thePBD.currently_attached)
                {
                    this.Description = string.Format(Messages.ACTION_SR_REPAIR_PLUGGING_PBD, Helpers.GetName(host));
                    log.Debug(this.Description);

                    try
                    {
                        RelatedTask = XenAPI.PBD.async_plug(this.Session, thePBD.opaque_ref);

                        if (PercentComplete + delta <= 100)
                        {
                            PollToCompletion(PercentComplete, PercentComplete + delta);
                        }
                        else
                        {
                            PollToCompletion(PercentComplete, 100);
                            PercentComplete = 100;
                        }
                    }
                    catch (XenAPI.Failure f)
                    {
                        failure = f;
                        failureDescription = Description;
                    }
                }
                else
                {
                    PercentComplete += delta;
                }
            }

            if (failure != null && failureDescription != null)
            {
                Description = failureDescription;
                throw failure;
            }

            // CA-14928: Sometimes we have too many PBDs if there has just been a host 
            // eject and the GC hasn't collected the PBD yet.
            //
            //if (SR.PBDs.Count != _hostList.Count && SR.shared && !SR.IsToolsSR)
            //{
            //    throw new Exception(Messages.ACTION_SR_REPAIR_FAILED);
            //}
            if (isSharedAction)
                Description = string.Format(Messages.ACTION_SR_SHARE_SUCCESSFUL, SR.NameWithoutHost());
            else
                Description = string.Format(Messages.ACTION_SR_REPAIR_SUCCESSFUL, SR.NameWithoutHost());
        }

        protected override void CancelRelatedTask()
        {
            this.Description = Messages.ACTION_SR_REPAIR_CANCELLED;

            base.CancelRelatedTask();
        }
    }
}
