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

using XenAPI;
using XenAdmin.Core;
using XenAdmin.Model;
using XenAdmin.Network;


namespace XenAdmin.Actions
{
    public class CreateBondAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private class NewBond
        {
            internal NewBond(Bond bond, PIF master, List<PIF> slaves)
            {
                this.bond = bond;
                this.master = master;
                this.slaves = slaves;
            }

            internal Bond bond;
            internal PIF master;
            internal List<PIF> slaves;
        }

        private readonly string name_label;
        private readonly bool autoplug;
        private readonly long mtu;
        private readonly bond_mode bondMode;
        private readonly Dictionary<Host, List<PIF>> PIFs = new Dictionary<Host, List<PIF>>();
        private readonly Host Master;
        private readonly Bond.hashing_algoritm hashingAlgoritm;

        /// <param name="name_label">The name for the new network.</param>
        /// <param name="PIFs_on_master">The PIFs on the master representing the physical NICs that are to be bonded together.</param>
        /// <param name="autoplug">Whether the new network is marked AutoPlug.</param>
        /// <param name="mtu">The MTU for the Bond, ignored for pre cowley</param>
        /// <param name="bondMode">The bond mode, ignored for pre-Boston</param>
        public CreateBondAction(IXenConnection connection, string name_label, List<PIF> PIFs_on_master, bool autoplug, long mtu, bond_mode bondMode,
            Bond.hashing_algoritm hashingAlgoritm)
            : base(connection, string.Format(Messages.ACTION_CREATE_BOND_TITLE, name_label),
                   string.Format(Messages.ACTION_CREATE_BOND_DESCRIPTION, name_label))
        {
            this.name_label = name_label;
            this.autoplug = autoplug;
            this.mtu = mtu;
            this.bondMode = bondMode;
            this.hashingAlgoritm = hashingAlgoritm;

            Pool = Helpers.GetPoolOfOne(Connection);
            if (Pool == null)
                throw new Failure(Failure.INTERNAL_ERROR, Messages.POOL_GONE);

            Master = Connection.Resolve(Pool.master);
            if (Master == null)
                throw new Failure(Failure.INTERNAL_ERROR, Messages.POOL_MASTER_GONE);

            foreach (Host host in Connection.Cache.Hosts)
                AppliesTo.Add(host.opaque_ref);

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("host.management_reconfigure");
            ApiMethodsToRoleCheck.Add("network.create");
            ApiMethodsToRoleCheck.Add("network.destroy");
            ApiMethodsToRoleCheck.Add("network.remove_from_other_config");
            ApiMethodsToRoleCheck.Add("pif.reconfigure_ip");
            ApiMethodsToRoleCheck.Add("pif.plug");
            ApiMethodsToRoleCheck.Add("bond.create");
            ApiMethodsToRoleCheck.Add("bond.destroy");
            ApiMethodsToRoleCheck.AddRange(XenAPI.Role.CommonSessionApiList);
            ApiMethodsToRoleCheck.AddRange(XenAPI.Role.CommonTaskApiList);
            #endregion

            PIFs = NetworkingHelper.PIFsOnAllHosts(PIFs_on_master);
            // these locks will be cleared in clean()
            foreach (List<PIF> pifs in PIFs.Values)
            {
                foreach (PIF pif in pifs)
                {
                    pif.Locked = true;
                }
            }
        }

        private List<NewBond> new_bonds;
        private XenAPI.Network network;
        protected override void Run()
        {
            // the network lock and the connection.expectDisruption will be cleared in clean()

            new_bonds = new List<NewBond>();
            network = null;
            Connection.ExpectDisruption = true;
            int inc = 100 / (Connection.Cache.HostCount * 2 + 1);
            string network_ref = CreateNetwork(0, inc);

            try
            {
                network = Connection.WaitForCache(new XenRef<XenAPI.Network>(network_ref));
                network.Locked = true;
                XenAPI.Network.remove_from_other_config(Session, network_ref, XenAPI.Network.CREATE_IN_PROGRESS);

                int lo = inc;
                foreach (Host host in GetHostsMasterLast())
                {
                    List<PIF> pifs = PIFs[host].FindAll(x => x.physical).ToList();

                    List<XenRef<PIF>> pif_refs = new List<XenRef<PIF>>();
                    foreach (PIF pif in pifs)
                    {
                        pif_refs.Add(new XenRef<PIF>(pif.opaque_ref));
                    }

                    log.DebugFormat("Creating bond on {0} with {1} PIFs...", Helpers.GetName(host), pifs.Count);

                    Dictionary<string, string> bondProperties = new Dictionary<string, string>();
                    if (bondMode == bond_mode.lacp)
                        bondProperties.Add("hashing_algorithm", Bond.HashingAlgoritmToString(hashingAlgoritm));

                    RelatedTask = Bond.async_create(Session, network_ref, pif_refs, "", bondMode, bondProperties);

                    PollToCompletion(lo, lo + inc);
                    lo += inc;
                    log.DebugFormat("Creating bond on {0} done: bond is {1}.", Helpers.GetName(host), Result);

                    Bond new_bond = Connection.WaitForCache(new XenRef<Bond>(Result));
                    if (new_bond == null)
                        throw new Failure(Failure.INTERNAL_ERROR, Messages.BOND_GONE);

                    PIF new_master = Connection.Resolve(new_bond.master);
                    if (new_master == null)
                        throw new Failure(Failure.INTERNAL_ERROR, Messages.BOND_MASTER_GONE);

                    new_bonds.Add(new NewBond(new_bond, new_master, pifs));

                    new_bond.Locked = true;
                    new_master.Locked = true;
                }

                foreach (NewBond new_bond in new_bonds)
                {
                    lo += inc;
                    ReconfigureManagementInterfaces(new_bond.slaves, new_bond.master, lo);
                }
            }
            catch (Exception)
            {
                foreach (NewBond new_bond in new_bonds)
                {
                    RevertManagementInterfaces(new_bond);
                    DestroyBond(new_bond.bond);
                }

                DestroyNetwork(network_ref);
                throw;
            }

            Description = string.Format(Messages.ACTION_CREATE_BOND_DONE, name_label);
        }

        private List<Host> GetHostsMasterLast()
        {
            List<Host> result = new List<Host>(Connection.Cache.Hosts);
            result.Remove(Master);
            result.Add(Master);
            return result;
        }

        /// <summary>
        /// Nothrow guarantee.
        /// </summary>
        /// <param name="network"></param>
        /// <param name="new_bonds"></param>
        private void UnlockAll(XenAPI.Network network, List<NewBond> new_bonds)
        {
            if (network != null)
            {
                network.Locked = false;
                foreach (NewBond newbond in new_bonds)
                {
                    newbond.master.Locked = false;
                    newbond.bond.Locked = false;
                }
            }
            foreach (Host host in PIFs.Keys)
            {
                foreach (PIF pif in PIFs[host])
                {
                    pif.Locked = false;
                }
            }
        }

        private void ReconfigureManagementInterfaces(List<PIF> slaves, PIF new_master, int hi)
        {
            int lo = PercentComplete;
            int inc = (hi - lo) / slaves.Count;
            foreach (PIF pif in slaves)
            {
                lo += inc;
                NetworkingActionHelpers.MoveManagementInterfaceName(this, pif, new_master);
            }
        }

        /// <summary>
        /// Nothrow guarantee.
        /// </summary>
        /// <param name="slaves"></param>
        /// <param name="master"></param>
        private void RevertManagementInterfaces(NewBond new_bond)
        {
            List<PIF> slaves = new_bond.slaves;
            PIF master = new_bond.master;
            Bond bond = new_bond.bond;

            PIF slave = Connection.Resolve(bond.primary_slave);

            if (slave == null)
            {
                if (slaves.Count == 0)
                    return;
                slave = slaves[0];
            }

            try
            {
                NetworkingActionHelpers.MoveManagementInterfaceName(this, master, slave);
            }
            catch (Exception exn)
            {
                log.Warn(exn, exn);
            }
        }

        string CreateNetwork(int lo, int hi)
        {
            log.DebugFormat("Creating network {0}...", name_label);

            XenAPI.Network network = new XenAPI.Network();
            network.name_label = name_label;
            network.SetAutoPlug(autoplug);
            network.MTU = mtu;
            if (network.other_config == null)
                network.other_config = new Dictionary<string, string>();
            network.other_config[XenAPI.Network.CREATE_IN_PROGRESS] = "true";
            network.managed = true;

            RelatedTask = XenAPI.Network.async_create(Session, network);
            PollToCompletion(lo, hi);

            log.DebugFormat("Created network {0} as {1}.", name_label, Result);

            return Result;
        }

        /// <summary>
        /// Nothrow guarantee.
        /// </summary>
        /// <param name="network"></param>
        private void DestroyNetwork(string network)
        {
            log.DebugFormat("Destroying network {0} as part of cleanup...", network);

            try
            {
                XenAPI.Network.destroy(Session, network);
            }
            catch (Exception exn)
            {
                log.Warn(exn, exn);
            }

            log.DebugFormat("Destroying network {0} as part of cleanup done.", network);
        }

        /// <summary>
        /// Nothrow guarantee.
        /// </summary>
        /// <param name="bond"></param>
        private void DestroyBond(Bond bond)
        {
            log.DebugFormat("Destroying bond {0} as part of cleanup...", bond.uuid);

            try
            {
                XenAPI.Bond.destroy(Session, bond.opaque_ref);
            }
            catch (Exception exn)
            {
                log.Warn(exn, exn);
            }

            log.DebugFormat("Destroying bond {0} as part of cleanup done.", bond.uuid);
        }

        protected override void Clean()
        {
            Connection.ExpectDisruption = false;
            UnlockAll(network, new_bonds);
        }
    }
}
