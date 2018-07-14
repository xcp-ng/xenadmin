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
using System.Text;
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard.Filters
{
    public class CrossPoolMigrateCanMigrateFilter : ReasoningFilter
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly WizardMode _wizardMode;
        private string disableReason = string.Empty;
        private readonly List<VM> preSelectedVMs;
        private IDictionary<string, IDictionary<string, string>> cache;
        private bool canceled = false;
        private static readonly Object cacheLock = new Object();

        public CrossPoolMigrateCanMigrateFilter(IXenObject itemAddedToComboBox, List<VM> preSelectedVMs, WizardMode wizardMode, IDictionary<string, IDictionary<string, string>> cache = null)
            : base(itemAddedToComboBox)
        {
            _wizardMode = wizardMode;
            if (cache == null)
                this.cache = new Dictionary<string, IDictionary<string, string>>();
            else
                this.cache = cache;

            if (preSelectedVMs == null)
                throw new ArgumentNullException("Pre-selected VMs are null");
            this.preSelectedVMs = preSelectedVMs;
        }

        public override void Cancel()
        {
            canceled = true;
        }

        public override bool FailureFound
        {
            get
            {
                log.InfoFormat("Asserting can migrate to {0}...", ItemToFilterOn);

                Pool targetPool;
                List<Host> targets = CollateHosts(out targetPool);
                var excludedHosts = new List<string>();

                foreach (Host host in targets)
                {
                    var targetSrs = host.Connection.Cache.SRs.Where(sr => sr.SupportsStorageMigration()).ToList();
                    var targetNetwork = GetANetwork(host);

                    foreach (VM vm in preSelectedVMs)
                    {
                        if (canceled)
                            return excludedHosts.Count == targets.Count;

                        // obtain the cache data for a vm
                        IDictionary<string, string> vmCache;
                        lock (cacheLock)
                        {
                            if (!cache.ContainsKey(vm.opaque_ref))
                            {
                                cache.Add(vm.opaque_ref, new Dictionary<string, string>());
                            }
                            vmCache = cache[vm.opaque_ref];
                        }

                        try
                        {
                            //CA-220218: for intra-pool motion of halted VMs we do a move, so no need to assert we can migrate
                            Pool vmPool = Helpers.GetPoolOfOne(vm.Connection);
                            if (_wizardMode == WizardMode.Move && vmPool != null && targetPool != null && vmPool.opaque_ref == targetPool.opaque_ref)
                                continue;
                            
                            //Skip the resident host as there's a filter for it and 
                            //if not then you could exclude intrapool migration
                            //CA-205799: do not offer the host the VM is currently on
                            Host homeHost = vm.Home();
                            if (homeHost != null && homeHost.opaque_ref == host.opaque_ref)
                            {
                                if (!excludedHosts.Contains(host.opaque_ref))
                                    excludedHosts.Add(host.opaque_ref);
                                continue;
                            }

                            if (vmCache.ContainsKey(host.opaque_ref))
                            {
                                disableReason = vmCache[host.opaque_ref];
                                if (!string.IsNullOrEmpty(disableReason) && !excludedHosts.Contains(host.opaque_ref))
                                {
                                    excludedHosts.Add(host.opaque_ref);
                                }
                                continue;
                            }

                            //if pool_migrate can be done, then we will allow it in the wizard, even if storage migration is not allowed (i.e. users can use the wizard to live-migrate a VM inside the pool)
                            if (_wizardMode == WizardMode.Migrate && vmPool != null && targetPool != null && vmPool.opaque_ref == targetPool.opaque_ref)
                            {
                                var reason = VMOperationHostCommand.GetVmCannotBootOnHostReason(vm, host, vm.Connection.Session, vm_operations.pool_migrate);
                                if (string.IsNullOrEmpty(reason))
                                {
                                    lock (cacheLock)
                                    {
                                        vmCache.Add(host.opaque_ref, reason);
                                    }
                                    continue;
                                }
                            }

                            PIF managementPif = host.Connection.Cache.PIFs.First(p => p.management);
                            XenAPI.Network network = host.Connection.Cache.Resolve(managementPif.network);

                            Session session = host.Connection.DuplicateSession();
                            Dictionary<string, string> receiveMapping = Host.migrate_receive(session, host.opaque_ref, network.opaque_ref, new Dictionary<string, string>());
                            VM.assert_can_migrate(vm.Connection.Session,
                                                  vm.opaque_ref,
                                                  receiveMapping,
                                                  true,
                                                  GetVdiMap(vm, targetSrs),
                                                  vm.Connection == host.Connection ? new Dictionary<XenRef<VIF>, XenRef<XenAPI.Network>>() : GetVifMap(vm, targetNetwork),
                                                  //TODO: Implement force=true as general option
                                                  new Dictionary<string, string>(){{ "force", "true" }});
                            lock (cacheLock)
                            {
                                vmCache.Add(host.opaque_ref, string.Empty);
                            }
                        }
                        catch (Failure failure)
                        {
                            if (failure.ErrorDescription.Count > 0 && failure.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                                disableReason = failure.Message.Split('\n')[0].TrimEnd('\r'); // we want the first line only
                            else
                                disableReason = failure.Message;

                            lock (cacheLock)
                            {
                                vmCache.Add(host.opaque_ref, disableReason.Clone().ToString());
                            }

                            log.ErrorFormat("VM: {0}, Host: {1} - Reason: {2};", vm.opaque_ref, host.opaque_ref, failure.Message);
                            
                            if (!excludedHosts.Contains(host.opaque_ref))
                                excludedHosts.Add(host.opaque_ref);
                        }
                    }
                }

                return excludedHosts.Count == targets.Count;
            }
        }

        private List<Host> CollateHosts(out Pool thePool)
        {
            thePool = null;

            List<Host> target = new List<Host>();
            if (ItemToFilterOn is Host)
            {
                target.Add(ItemToFilterOn as Host);
                thePool = Helpers.GetPoolOfOne(ItemToFilterOn.Connection);
            }

            if (ItemToFilterOn is Pool)
            {
                Pool pool = ItemToFilterOn as Pool;
                target.AddRange(pool.Connection.Cache.Hosts);
                thePool = pool;
            }
            return target;
        }

        public override string Reason
        {
            get { return disableReason; }
        }

        private Dictionary<XenRef<VDI>, XenRef<SR>> GetVdiMap(VM vm, List<SR> targetSrs)
        {
            var vdiMap = new Dictionary<XenRef<VDI>, XenRef<SR>>();

            foreach (var vbdRef in vm.VBDs)
            {
                VBD vbd = vm.Connection.Resolve(vbdRef);
                if (vbd != null)
                {
                    VDI vdi = vm.Connection.Resolve(vbd.VDI);
                    if (vdi != null)
                    {
                        SR sr = vm.Connection.Resolve(vdi.SR);
                        if (sr != null && sr.GetSRType(true) != SR.SRTypes.iso)
                        {
                            // CA-220218: select a storage other than the VDI's current storage to ensure that
                            // both source and target SRs will be checked to see if they support migration
                            // (when sourceSR == targetSR, the server side skips the check)
                            var targetSr = targetSrs.FirstOrDefault(s => s.opaque_ref != sr.opaque_ref);
                            if (targetSr != null)
                                vdiMap.Add(new XenRef<VDI>(vdi.opaque_ref), new XenRef<SR>(targetSr));
                        }
                    }
                }
            }

            return vdiMap;
        }

        private XenAPI.Network GetANetwork(Host host)
        {
            return host.Connection.Cache.Networks.FirstOrDefault(network => host.CanSeeNetwork(network));
        }

        private Dictionary<XenRef<VIF>, XenRef<XenAPI.Network>> GetVifMap(VM vm, XenAPI.Network targetNetwork)
        {
            var vifMap = new Dictionary<XenRef<VIF>, XenRef<XenAPI.Network>>();

            if (targetNetwork != null)
            {
                List<VIF> vifs = vm.Connection.ResolveAll(vm.VIFs);

                foreach (var vif in vifs)
                {
                    vifMap.Add(new XenRef<VIF>(vif.opaque_ref), new XenRef<XenAPI.Network>(targetNetwork));
                }
            }
            return vifMap;
        }
    }
}
