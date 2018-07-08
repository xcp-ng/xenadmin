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
using XenAdmin;
using XenAdmin.Core;


namespace XenAPI
{
    public partial class PIF : IComparable<PIF>
    {
        public override string Name()
        {
            if (IsPhysical())
            {
                bool bond;
                string number = NICIdentifier(out bond);
                return string.Format(bond ? Messages.PIF_BOND : Messages.PIF_NIC, number);
            }
            else if (IsTunnelAccessPIF())
            {
                // In the case of tunnel access PIFs, use the name of the corresponding transport PIF (CA-63296)
                Tunnel tunnel = Connection.Resolve(tunnel_access_PIF_of[0]);
                PIF transport_pif = Connection.Resolve(tunnel.transport_PIF);
                return transport_pif.Name();
            }

            else if(IsSriovLogicalPIF())
            {
                if (Connection == null)
                    return "";
                Network_sriov network_s = Connection.Resolve(sriov_logical_PIF_of[0]);
                if (network_s == null)
                    return "";
                PIF pif = Connection.Resolve(network_s.physical_PIF);
                if (pif == null)
                    return "";
                return pif.Name();
            }
            else
            {
                if (Connection == null)
                    return "";
                VLAN vlan = Connection.Resolve(VLAN_master_of);
                if (vlan == null)
                    return "";
                PIF slave = Connection.Resolve(vlan.tagged_PIF);
                if (slave == null)
                    return "";
                return slave.Name();
            }
        }

        public PIF_metrics PIFMetrics()
        {
            return metrics == null || Connection == null ? null : Connection.Resolve(metrics);
        }

        /// <summary>
        /// This is the name of the secondary management interface
        /// </summary>
        public string GetManagementPurpose()
        {
            return Get(other_config, "management_purpose");
        }

        public void SetManagementPurspose(string value)
        {
            other_config = SetDictionaryKey(other_config, "management_purpose", value);
        }

        internal string NICIdentifier(out bool is_bond)
        {
            if (bond_master_of.Count == 0)
            {
                is_bond = false;
                return device.Replace("eth", "");
            }
            else
            {
                is_bond = true;

                if (Connection == null)
                    return device;

                List<PIF> slaves = new List<PIF>();
                foreach (Bond bond in Connection.ResolveAll(bond_master_of))
                {
                    slaves.AddRange(Connection.ResolveAll(bond.slaves));
                }
                return BondSuffix(slaves);
            }
        }

        public static string BondSuffix(List<PIF> slaves)
        {
            List<string> ids = new List<string>();
            foreach (PIF slave in slaves)
            {
                ids.Add(slave.device.Replace("eth", ""));
            }
            ids.Sort();
            return string.Join("+", ids.ToArray());
        }

        public override string ToString()
        {
            return Name();
        }

        public bool IsTunnelAccessPIF()
        {
            return tunnel_access_PIF_of != null && tunnel_access_PIF_of.Count != 0;
        }

        public bool IsPhysical()
        {
            return VLAN == -1 && !IsTunnelAccessPIF() && !IsSriovLogicalPIF();
        }

        public override int CompareTo(PIF other)
        {
            int result = this.management.CompareTo(other.management);
            if (result != 0)
                return -result;  // management PIF first

            return base.CompareTo(other);
        }

        public override bool Show(bool showHiddenVMs)
        {
            return IsManaged() && (showHiddenVMs || !IsHidden());
        }

        public override bool IsHidden()
        {
            return BoolKey(other_config, HIDE_FROM_XENCENTER);
        }

        /// <summary>
        /// Indicates whether the interface is managed by xapi.
        /// Note that this is the same as PIF.managed property for Clearwater SP1 and later hosts.
        /// And it is always true for older hosts, where the managed property is not available.
        /// </summary>
        public bool IsManaged()
        {
            return Helpers.ClearwaterSp1OrGreater(Connection) ? managed : true;
        }

        // Whether this PIF is a management interface in the XenCenter sense.
        // Note that this is not the same as PIF.management, which only includes
        // the primary management interface. We also want all interfaces with an
        // IP address.
        public bool IsManagementInterface(bool showHiddenVMs)
        {
            return IsPrimaryManagementInterface() || IsSecondaryManagementInterface(showHiddenVMs);
        }

        public bool IsPrimaryManagementInterface()
        {
            if (!management)
                return false;

            Network nw = Connection.Resolve(network);
            return nw != null && !nw.IsGuestInstallerNetwork();
        }

        // I lied slightly above. A secondary management interface in Boston and greater
        // is one with an IP address. In Cowley and earlier, it's one where disallow_unplug
        // is set. If the interface was configured through XenCenter, both of these things
        // will be true, but if it was configured on the command line, they may not be.
        // See CA-56611 for a discussion of this.
        public bool IsSecondaryManagementInterface(bool showHiddenVMs)
        {
            if (management)
                return false;

            bool criterion = (ip_configuration_mode != ip_configuration_mode.None && ip_configuration_mode != ip_configuration_mode.unknown);
            if (!criterion)
                return false;

            Network nw = Connection.Resolve(network);
            return nw != null && nw.Show(showHiddenVMs);
        }

        public string ManagementInterfaceNameOrUnknown()
        {
            if (management)
                return Messages.MANAGEMENT;

            var managementPurpose = GetManagementPurpose();
            return string.IsNullOrEmpty(managementPurpose) ? Messages.NETWORKING_PROPERTIES_PURPOSE_UNKNOWN : managementPurpose;
        }

        public bool IsBondSlave()
        {
            return BondSlaveOf() != null;
        }

        /// <summary>
        /// Whether this is a bond slave, and the bond master is plugged.
        /// </summary>
        public bool IsInUseBondSlave()
        {
            Bond bond = BondSlaveOf();
            if (bond == null)
                return false;
            PIF master = bond.Connection.Resolve(bond.master);
            if (master == null)
                return false;
            return master.currently_attached;
        }

        public bool IsUsedByClustering()
        {
            return Connection.Cache.Cluster_hosts.Any(cluster => cluster.PIF.opaque_ref == opaque_ref);
        }

        /// <summary>
        /// Returns the Bond of which this PIF is a slave, or null if it is not so.
        /// </summary>
        public Bond BondSlaveOf()
        {
            return Connection == null ? null : Connection.Resolve(bond_slave_of);
        }

        /// <summary>
        /// Returns the Bond of which this PIF is a master, or null if it is not so.
        /// </summary>
        public Bond BondMasterOf()
        {
            return Connection == null || bond_master_of.Count == 0 ? null : Connection.Resolve(bond_master_of[0]);
        }

        public string Speed()
        {
            if (Connection == null)
                return Messages.HYPHEN;
            PIF_metrics pifMetrics = Connection.Resolve(this.metrics);
            if (pifMetrics == null)
                return Messages.HYPHEN;
            return string.Format(Messages.NICPANEL_BIT_RATE, pifMetrics.speed);
        }

        public bool IsBondNIC()
        {
            return bond_master_of.Count > 0;
        }

        public string Duplex()
        {
            if (Connection == null)
                return Messages.HYPHEN;
            PIF_metrics pifMetrics = Connection.Resolve(this.metrics);
            if (pifMetrics == null)
                return Messages.HYPHEN;
            return pifMetrics.duplex ? Messages.NICPANEL_FULL_DUPLEX : Messages.NICPANEL_HALF_DUPLEX;
        }

        public bool Carrier()
        {
            if (Connection == null)
                return false;
            PIF_metrics pifMetrics = Connection.Resolve(this.metrics);
            if (pifMetrics == null)
                return false;
            return pifMetrics.carrier;
        }

        /// <summary>
        /// Returns either the IP address of the PIF, DHCP or Unknown as appropriate
        /// </summary>
        public string FriendlyIPAddress()
        {
            if (!string.IsNullOrEmpty(IP))
                return IP;
            switch (ip_configuration_mode)
            {
                case ip_configuration_mode.DHCP:
                    return Messages.PIF_DHCP;
                default:
                    return Messages.PIF_UNKNOWN;
            }
        }

        public string LinkStatusString()
        {
            var linkStatus = LinkStatus();
            switch (linkStatus)
            {
                case LinkState.Connected:
                    return Messages.CONNECTED;
                case LinkState.Disconnected:
                    return Messages.DISCONNECTED;
                case LinkState.Unknown:
                    return Messages.UNKNOWN;
            }
            return "-";
        }

        public enum LinkState { Unknown, Connected, Disconnected };

        public LinkState LinkStatus()
        {
            if (IsTunnelAccessPIF())
            {
                Tunnel tunnel = Connection.Resolve(tunnel_access_PIF_of[0]); // can only ever be the access PIF of one tunnel
                Dictionary<string, string> status = (tunnel == null ? null : tunnel.status);
                return (status != null && status.ContainsKey("active") && status["active"] == "true"
                    ? LinkState.Connected : LinkState.Disconnected);
            }

            //if (!pif.IsPhysical && !poolwide)
            //    return Messages.SPACED_HYPHEN;

            PIF_metrics pifMetrics = PIFMetrics();

            // LinkStatus of SR-IOV network and VLAN on SR-IOV network
            if (IsSriovLogicalPIF() || VLAN >= 0)
            {
                var network_sriov = NetworkSriov();
                if (network_sriov == null && VLAN < 0)
                {
                    return LinkState.Disconnected;
                }
                else if (network_sriov != null)
                {
                    Network_sriov network_s = Connection.Resolve(network_sriov);
                    if (pifMetrics == null)
                        return LinkState.Unknown;
                    else if (!pifMetrics.carrier || (network_s == null || network_s.configuration_mode == sriov_configuration_mode.unknown || network_s.requires_reboot == true ))
                        return LinkState.Disconnected;
                }
            }

            return pifMetrics == null
                ? LinkState.Unknown
                : pifMetrics.carrier ? LinkState.Connected : LinkState.Disconnected;
        }

        public string IpConfigurationModeString()
        {
            switch (ip_configuration_mode)
            {
                case ip_configuration_mode.None:
                    return Messages.PIF_NONE;
                case ip_configuration_mode.DHCP:
                    return Messages.PIF_DHCP;
                case ip_configuration_mode.Static:
                    return Messages.PIF_STATIC;
                default:
                    return Messages.PIF_UNKNOWN;
            }
        }

        public bool FCoECapable()
        {
            return capabilities.Any(capability => capability == "fcoe");
        }

        public bool IsSriovLogicalPIF()
        {
            return sriov_logical_PIF_of != null && sriov_logical_PIF_of.Count != 0;
        }

        public bool IsSriovPhysicalPIF()
        {
            return sriov_physical_PIF_of != null && sriov_physical_PIF_of.Count != 0;
        }

        public bool SriovCapable()
        {
            return capabilities.Any(capability => capability == "sriov");
        }

        public XenRef<Network_sriov> NetworkSriov()
        {
            if (IsSriovLogicalPIF())
                return sriov_logical_PIF_of[0];

            if (VLAN < 0)
                return null;
            var vlan = Connection.Resolve(VLAN_master_of);
            if (vlan == null)
                return null;
            var taggedPif = Connection.Resolve(vlan.tagged_PIF);
            return taggedPif != null && taggedPif.IsSriovLogicalPIF() ? taggedPif.sriov_logical_PIF_of[0] : null;
        }
    }
}
