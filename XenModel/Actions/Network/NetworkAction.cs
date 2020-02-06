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
using System.Text;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public class NetworkAction : AsyncAction
    {
        private enum network_actions { create, destroy, update };

        private XenAPI.Network networkClone;
        private XenAPI.Network networkServerObject;
        private readonly network_actions actionType;
        private readonly PIF pif;
        private readonly long vlan;
        private readonly bool external;
        private readonly bool changePIFs;

        private List<PIF> PIFs;

        /// <summary>
        /// Create an external (VLAN) network.
        /// </summary>
        /// <param name="network">A new Network instance describing the changes to be made on the server side.</param>
        /// <param name="pif">The PIF representing the physical NIC from which we're basing our new VLAN.</param>
        /// <param name="vlan">The new VLAN tag.</param>
        public NetworkAction(IXenConnection connection, XenAPI.Network network, PIF pif, long vlan)
            : base(connection, string.Format(Messages.NETWORK_ACTION_CREATING_NETWORK_TITLE, network.Name(),
            Helpers.GetName(connection)))
        {
            actionType = network_actions.create;
            this.networkClone = network;
            this.pif = pif;
            this.vlan = vlan;
            this.external = true;
            this.PIFs = null;

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("network.create");
            ApiMethodsToRoleCheck.Add("pool.create_VLAN_from_PIF");
            #endregion

            init();
        }

        /// <summary>
        /// Create or destroy a private network.
        /// </summary>
        public NetworkAction(IXenConnection connection, XenAPI.Network network, bool create)
            : base(connection,
                   string.Format(create ? Messages.NETWORK_ACTION_CREATING_NETWORK_TITLE : Messages.NETWORK_ACTION_REMOVING_NETWORK_TITLE,
                                 network.Name(), Helpers.GetName(connection)))
        {
            this.networkClone = network;
            this.external = false;

            if (create)
            {
                #region RBAC Dependencies

                ApiMethodsToRoleCheck.Add("network.create");

                #endregion
                actionType = network_actions.create;
                PIFs = null;
            }
            else
            {
                PIFs = Connection.ResolveAll(network.PIFs);
                #region RBAC Dependencies
                ApiMethodsToRoleCheck.Add("network.destroy");
                if (PIFs.Find(p => p.IsTunnelAccessPIF()) != null)
                    ApiMethodsToRoleCheck.Add("tunnel.destroy");
                if (PIFs.Find(p => !p.IsTunnelAccessPIF() && p.physical) != null)
                    ApiMethodsToRoleCheck.Add("pif.forget");  // actually, we should have at most one of tunnel.destroy and pif.forget
                if (PIFs.Find(p => !p.IsTunnelAccessPIF() && !p.physical) != null)
                    ApiMethodsToRoleCheck.Add("vlan.destroy");  // same here, shouldn't be both virtual and physcial really
                #endregion
                actionType = network_actions.destroy;
            }

            init();
        }

        /// <summary>
        /// Update a network.
        /// </summary>
        /// <param name="network">The modified network that we're going to save to the server.</param>
        /// <param name="changePIFs">True if we're going to create or destroy PIFs (i.e. change a private network to a
        /// VLAN, or vice versa.</param>
        /// <param name="external">Whether the new network is external i.e. a VLAN.</param>
        /// <param name="pif">The PIF representing the physical NIC from which we're basing our new VLAN.
        /// Null iff changePIFs is false or external is false.</param>
        /// <param name="vlan">The new VLAN tag.  Ignored iff changePIFs is false or external is false.</param>
        /// <param name="suppressHistory"></param>
        public NetworkAction(IXenConnection connection, XenAPI.Network network,
            bool changePIFs, bool external, PIF pif, long vlan, bool suppressHistory)
            : base(connection, string.Format(Messages.NETWORK_ACTION_UPDATING_NETWORK_TITLE,
            network.Name(), Helpers.GetName(connection)), suppressHistory)
        {
            actionType = network_actions.update;
            this.networkClone = network;
            this.changePIFs = changePIFs;
            this.external = external;
            this.pif = pif;
            this.vlan = vlan;
            PIFs = changePIFs ? Connection.ResolveAll(network.PIFs) : null;

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("network.set_name_label");
            ApiMethodsToRoleCheck.Add("network.set_other_config");
            ApiMethodsToRoleCheck.Add("network.add_to_other_config");
            ApiMethodsToRoleCheck.Add("network.remove_from_other_config");
            ApiMethodsToRoleCheck.Add("network.set_tags");
            ApiMethodsToRoleCheck.Add("network.add_tags");
            ApiMethodsToRoleCheck.Add("network.remove_tags");
            if (changePIFs)
            {
                ApiMethodsToRoleCheck.Add("pif.destroy");
                if (external)
                    ApiMethodsToRoleCheck.Add("pool.create_VLAN_from_PIF");
            }

            #endregion

            init();
        }

        private void init()
        {
            SetAppliesTo(Connection);
            if (networkClone.opaque_ref != null)
            {
                // we are updating an existing object, and so should lock it
                networkServerObject = Connection.Resolve<XenAPI.Network>(new XenRef<XenAPI.Network>(networkClone.opaque_ref));
                networkServerObject.Locked = true;
            }
            if (PIFs == null) return;
            foreach (PIF pif in PIFs)
                pif.Locked = true;
        }

        private void SetAppliesTo(IXenConnection connection)
        {
            foreach (Pool pool in connection.Cache.Pools)
            {
                AppliesTo.Add(pool.opaque_ref);
            }
            foreach (Host host in connection.Cache.Hosts)
            {
                AppliesTo.Add(host.opaque_ref);
            }
        }

        private void destroyPIFs()
        {
            foreach (PIF pif in PIFs)
            {
                if (pif.IsTunnelAccessPIF())
                {
                    // A tunnel access PIF is destroyed by destroying its tunnel.
                    // (Actually each network will have either all tunnel access PIFs (if
                    // it is a CHIN) or all regular PIFs (if it isn't), but we don't use
                    // that: we do it PIF-by-PIF).
                    foreach (Tunnel tunnel in Connection.ResolveAll(pif.tunnel_access_PIF_of))  // actually there will only ever be one
                        Tunnel.destroy(Session, tunnel.opaque_ref);
                }
                else
                {
                    if (!pif.physical)
                    {
                        if(pif.VLAN != -1)
                        {
                            VLAN.destroy(Session, pif.VLAN_master_of);
                        }
                        
                        if(pif.IsSriovLogicalPIF())
                        {
                            Network_sriov.destroy(Session, pif.sriov_logical_PIF_of[0]);
                        }
                    }
                    else
                    {
                        // do we ever destroy physical pifs anyway? not sure this is something we need but here for completeness
                        PIF.forget(Session, pif.opaque_ref);
                    }
                    
                }
            }
            PIFs = null;
        }

        protected override void Run()
        {
            switch (actionType)
            {
                case network_actions.destroy:
                    Description = Messages.NETWORK_ACTION_REMOVING_NETWORK;
                    destroyPIFs();
                    XenAPI.Network.destroy(Session, networkClone.opaque_ref);
                    networkClone = null;
                    Description = Messages.NETWORK_ACTION_NETWORK_REMOVED;
                    break;

                case network_actions.update:
                    Description = Messages.NETWORK_ACTION_UPDATING_NETWORK;

                    if (changePIFs)
                    {
                        if (external)
                        {
                            //Before we do a destroy check the vlan tag is not in use on this network
                            foreach (PIF p in Connection.Cache.PIFs)
                            {
                                if (p.VLAN == vlan && p.device == pif.device)
                                    throw new Exception(FriendlyErrorNames.PIF_VLAN_EXISTS);
                            }
                        }
                        destroyPIFs();
                        if (external)
                            CreateVLAN(networkClone.opaque_ref);
                    }

                    Description = Messages.NETWORK_ACTION_NETWORK_UPDATED;
                    break;

                case network_actions.create:
                    Description = Messages.NETWORK_ACTION_CREATING_NETWORK;

                    XenRef<XenAPI.Network> networkRef = XenAPI.Network.create(Session, networkClone);
                    if (external)
                        CreateVLAN(networkRef.opaque_ref);

                    Description = Messages.NETWORK_ACTION_NETWORK_CREATED;
                    break;
            }
        }

        private void CreateVLAN(string network_ref)
        {
            Host host = Helpers.GetMaster(Connection);
            if (host == null)
                throw new Failure(Failure.INTERNAL_ERROR, Messages.POOL_MASTER_GONE);
            Pool.create_VLAN_from_PIF(Session, pif.opaque_ref, network_ref, vlan);
        }

        protected override void Clean()
        {
            if (networkServerObject != null)
                networkServerObject.Locked = false;
            if (PIFs == null) return;
            foreach (PIF pif in PIFs)
                pif.Locked = false;
        }
    }
}
