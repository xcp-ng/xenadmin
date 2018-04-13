/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 *   1) Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *
 *   2) Redistributions in binary form must reproduce the above
 *      copyright notice, this list of conditions and the following
 *      disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;


namespace XenAPI
{
    /// <summary>
    /// A physical network interface (note separate VLANs are represented as several PIFs)
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class PIF : XenObject<PIF>
    {
        public PIF()
        {
        }

        public PIF(string uuid,
            string device,
            XenRef<Network> network,
            XenRef<Host> host,
            string MAC,
            long MTU,
            long VLAN,
            XenRef<PIF_metrics> metrics,
            bool physical,
            bool currently_attached,
            ip_configuration_mode ip_configuration_mode,
            string IP,
            string netmask,
            string gateway,
            string DNS,
            XenRef<Bond> bond_slave_of,
            List<XenRef<Bond>> bond_master_of,
            XenRef<VLAN> VLAN_master_of,
            List<XenRef<VLAN>> VLAN_slave_of,
            bool management,
            Dictionary<string, string> other_config,
            bool disallow_unplug,
            List<XenRef<Tunnel>> tunnel_access_PIF_of,
            List<XenRef<Tunnel>> tunnel_transport_PIF_of,
            ipv6_configuration_mode ipv6_configuration_mode,
            string[] IPv6,
            string ipv6_gateway,
            primary_address_type primary_address_type,
            bool managed,
            Dictionary<string, string> properties,
            string[] capabilities,
            pif_igmp_status igmp_snooping_status,
            List<XenRef<Network_sriov>> sriov_physical_PIF_of,
            List<XenRef<Network_sriov>> sriov_logical_PIF_of,
            XenRef<PCI> PCI)
        {
            this.uuid = uuid;
            this.device = device;
            this.network = network;
            this.host = host;
            this.MAC = MAC;
            this.MTU = MTU;
            this.VLAN = VLAN;
            this.metrics = metrics;
            this.physical = physical;
            this.currently_attached = currently_attached;
            this.ip_configuration_mode = ip_configuration_mode;
            this.IP = IP;
            this.netmask = netmask;
            this.gateway = gateway;
            this.DNS = DNS;
            this.bond_slave_of = bond_slave_of;
            this.bond_master_of = bond_master_of;
            this.VLAN_master_of = VLAN_master_of;
            this.VLAN_slave_of = VLAN_slave_of;
            this.management = management;
            this.other_config = other_config;
            this.disallow_unplug = disallow_unplug;
            this.tunnel_access_PIF_of = tunnel_access_PIF_of;
            this.tunnel_transport_PIF_of = tunnel_transport_PIF_of;
            this.ipv6_configuration_mode = ipv6_configuration_mode;
            this.IPv6 = IPv6;
            this.ipv6_gateway = ipv6_gateway;
            this.primary_address_type = primary_address_type;
            this.managed = managed;
            this.properties = properties;
            this.capabilities = capabilities;
            this.igmp_snooping_status = igmp_snooping_status;
            this.sriov_physical_PIF_of = sriov_physical_PIF_of;
            this.sriov_logical_PIF_of = sriov_logical_PIF_of;
            this.PCI = PCI;
        }

        /// <summary>
        /// Creates a new PIF from a Proxy_PIF.
        /// </summary>
        /// <param name="proxy"></param>
        public PIF(Proxy_PIF proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given PIF.
        /// </summary>
        public override void UpdateFrom(PIF update)
        {
            uuid = update.uuid;
            device = update.device;
            network = update.network;
            host = update.host;
            MAC = update.MAC;
            MTU = update.MTU;
            VLAN = update.VLAN;
            metrics = update.metrics;
            physical = update.physical;
            currently_attached = update.currently_attached;
            ip_configuration_mode = update.ip_configuration_mode;
            IP = update.IP;
            netmask = update.netmask;
            gateway = update.gateway;
            DNS = update.DNS;
            bond_slave_of = update.bond_slave_of;
            bond_master_of = update.bond_master_of;
            VLAN_master_of = update.VLAN_master_of;
            VLAN_slave_of = update.VLAN_slave_of;
            management = update.management;
            other_config = update.other_config;
            disallow_unplug = update.disallow_unplug;
            tunnel_access_PIF_of = update.tunnel_access_PIF_of;
            tunnel_transport_PIF_of = update.tunnel_transport_PIF_of;
            ipv6_configuration_mode = update.ipv6_configuration_mode;
            IPv6 = update.IPv6;
            ipv6_gateway = update.ipv6_gateway;
            primary_address_type = update.primary_address_type;
            managed = update.managed;
            properties = update.properties;
            capabilities = update.capabilities;
            igmp_snooping_status = update.igmp_snooping_status;
            sriov_physical_PIF_of = update.sriov_physical_PIF_of;
            sriov_logical_PIF_of = update.sriov_logical_PIF_of;
            PCI = update.PCI;
        }

        internal void UpdateFromProxy(Proxy_PIF proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            device = proxy.device == null ? null : proxy.device;
            network = proxy.network == null ? null : XenRef<Network>.Create(proxy.network);
            host = proxy.host == null ? null : XenRef<Host>.Create(proxy.host);
            MAC = proxy.MAC == null ? null : proxy.MAC;
            MTU = proxy.MTU == null ? 0 : long.Parse(proxy.MTU);
            VLAN = proxy.VLAN == null ? 0 : long.Parse(proxy.VLAN);
            metrics = proxy.metrics == null ? null : XenRef<PIF_metrics>.Create(proxy.metrics);
            physical = (bool)proxy.physical;
            currently_attached = (bool)proxy.currently_attached;
            ip_configuration_mode = proxy.ip_configuration_mode == null ? (ip_configuration_mode) 0 : (ip_configuration_mode)Helper.EnumParseDefault(typeof(ip_configuration_mode), (string)proxy.ip_configuration_mode);
            IP = proxy.IP == null ? null : proxy.IP;
            netmask = proxy.netmask == null ? null : proxy.netmask;
            gateway = proxy.gateway == null ? null : proxy.gateway;
            DNS = proxy.DNS == null ? null : proxy.DNS;
            bond_slave_of = proxy.bond_slave_of == null ? null : XenRef<Bond>.Create(proxy.bond_slave_of);
            bond_master_of = proxy.bond_master_of == null ? null : XenRef<Bond>.Create(proxy.bond_master_of);
            VLAN_master_of = proxy.VLAN_master_of == null ? null : XenRef<VLAN>.Create(proxy.VLAN_master_of);
            VLAN_slave_of = proxy.VLAN_slave_of == null ? null : XenRef<VLAN>.Create(proxy.VLAN_slave_of);
            management = (bool)proxy.management;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            disallow_unplug = (bool)proxy.disallow_unplug;
            tunnel_access_PIF_of = proxy.tunnel_access_PIF_of == null ? null : XenRef<Tunnel>.Create(proxy.tunnel_access_PIF_of);
            tunnel_transport_PIF_of = proxy.tunnel_transport_PIF_of == null ? null : XenRef<Tunnel>.Create(proxy.tunnel_transport_PIF_of);
            ipv6_configuration_mode = proxy.ipv6_configuration_mode == null ? (ipv6_configuration_mode) 0 : (ipv6_configuration_mode)Helper.EnumParseDefault(typeof(ipv6_configuration_mode), (string)proxy.ipv6_configuration_mode);
            IPv6 = proxy.IPv6 == null ? new string[] {} : (string [])proxy.IPv6;
            ipv6_gateway = proxy.ipv6_gateway == null ? null : proxy.ipv6_gateway;
            primary_address_type = proxy.primary_address_type == null ? (primary_address_type) 0 : (primary_address_type)Helper.EnumParseDefault(typeof(primary_address_type), (string)proxy.primary_address_type);
            managed = (bool)proxy.managed;
            properties = proxy.properties == null ? null : Maps.convert_from_proxy_string_string(proxy.properties);
            capabilities = proxy.capabilities == null ? new string[] {} : (string [])proxy.capabilities;
            igmp_snooping_status = proxy.igmp_snooping_status == null ? (pif_igmp_status) 0 : (pif_igmp_status)Helper.EnumParseDefault(typeof(pif_igmp_status), (string)proxy.igmp_snooping_status);
            sriov_physical_PIF_of = proxy.sriov_physical_PIF_of == null ? null : XenRef<Network_sriov>.Create(proxy.sriov_physical_PIF_of);
            sriov_logical_PIF_of = proxy.sriov_logical_PIF_of == null ? null : XenRef<Network_sriov>.Create(proxy.sriov_logical_PIF_of);
            PCI = proxy.PCI == null ? null : XenRef<PCI>.Create(proxy.PCI);
        }

        public Proxy_PIF ToProxy()
        {
            Proxy_PIF result_ = new Proxy_PIF();
            result_.uuid = uuid ?? "";
            result_.device = device ?? "";
            result_.network = network ?? "";
            result_.host = host ?? "";
            result_.MAC = MAC ?? "";
            result_.MTU = MTU.ToString();
            result_.VLAN = VLAN.ToString();
            result_.metrics = metrics ?? "";
            result_.physical = physical;
            result_.currently_attached = currently_attached;
            result_.ip_configuration_mode = ip_configuration_mode_helper.ToString(ip_configuration_mode);
            result_.IP = IP ?? "";
            result_.netmask = netmask ?? "";
            result_.gateway = gateway ?? "";
            result_.DNS = DNS ?? "";
            result_.bond_slave_of = bond_slave_of ?? "";
            result_.bond_master_of = bond_master_of == null ? new string[] {} : Helper.RefListToStringArray(bond_master_of);
            result_.VLAN_master_of = VLAN_master_of ?? "";
            result_.VLAN_slave_of = VLAN_slave_of == null ? new string[] {} : Helper.RefListToStringArray(VLAN_slave_of);
            result_.management = management;
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.disallow_unplug = disallow_unplug;
            result_.tunnel_access_PIF_of = tunnel_access_PIF_of == null ? new string[] {} : Helper.RefListToStringArray(tunnel_access_PIF_of);
            result_.tunnel_transport_PIF_of = tunnel_transport_PIF_of == null ? new string[] {} : Helper.RefListToStringArray(tunnel_transport_PIF_of);
            result_.ipv6_configuration_mode = ipv6_configuration_mode_helper.ToString(ipv6_configuration_mode);
            result_.IPv6 = IPv6;
            result_.ipv6_gateway = ipv6_gateway ?? "";
            result_.primary_address_type = primary_address_type_helper.ToString(primary_address_type);
            result_.managed = managed;
            result_.properties = Maps.convert_to_proxy_string_string(properties);
            result_.capabilities = capabilities;
            result_.igmp_snooping_status = pif_igmp_status_helper.ToString(igmp_snooping_status);
            result_.sriov_physical_PIF_of = sriov_physical_PIF_of == null ? new string[] {} : Helper.RefListToStringArray(sriov_physical_PIF_of);
            result_.sriov_logical_PIF_of = sriov_logical_PIF_of == null ? new string[] {} : Helper.RefListToStringArray(sriov_logical_PIF_of);
            result_.PCI = PCI ?? "";
            return result_;
        }

        /// <summary>
        /// Creates a new PIF from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public PIF(Hashtable table) : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this PIF
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("device"))
                device = Marshalling.ParseString(table, "device");
            if (table.ContainsKey("network"))
                network = Marshalling.ParseRef<Network>(table, "network");
            if (table.ContainsKey("host"))
                host = Marshalling.ParseRef<Host>(table, "host");
            if (table.ContainsKey("MAC"))
                MAC = Marshalling.ParseString(table, "MAC");
            if (table.ContainsKey("MTU"))
                MTU = Marshalling.ParseLong(table, "MTU");
            if (table.ContainsKey("VLAN"))
                VLAN = Marshalling.ParseLong(table, "VLAN");
            if (table.ContainsKey("metrics"))
                metrics = Marshalling.ParseRef<PIF_metrics>(table, "metrics");
            if (table.ContainsKey("physical"))
                physical = Marshalling.ParseBool(table, "physical");
            if (table.ContainsKey("currently_attached"))
                currently_attached = Marshalling.ParseBool(table, "currently_attached");
            if (table.ContainsKey("ip_configuration_mode"))
                ip_configuration_mode = (ip_configuration_mode)Helper.EnumParseDefault(typeof(ip_configuration_mode), Marshalling.ParseString(table, "ip_configuration_mode"));
            if (table.ContainsKey("IP"))
                IP = Marshalling.ParseString(table, "IP");
            if (table.ContainsKey("netmask"))
                netmask = Marshalling.ParseString(table, "netmask");
            if (table.ContainsKey("gateway"))
                gateway = Marshalling.ParseString(table, "gateway");
            if (table.ContainsKey("DNS"))
                DNS = Marshalling.ParseString(table, "DNS");
            if (table.ContainsKey("bond_slave_of"))
                bond_slave_of = Marshalling.ParseRef<Bond>(table, "bond_slave_of");
            if (table.ContainsKey("bond_master_of"))
                bond_master_of = Marshalling.ParseSetRef<Bond>(table, "bond_master_of");
            if (table.ContainsKey("VLAN_master_of"))
                VLAN_master_of = Marshalling.ParseRef<VLAN>(table, "VLAN_master_of");
            if (table.ContainsKey("VLAN_slave_of"))
                VLAN_slave_of = Marshalling.ParseSetRef<VLAN>(table, "VLAN_slave_of");
            if (table.ContainsKey("management"))
                management = Marshalling.ParseBool(table, "management");
            if (table.ContainsKey("other_config"))
                other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("disallow_unplug"))
                disallow_unplug = Marshalling.ParseBool(table, "disallow_unplug");
            if (table.ContainsKey("tunnel_access_PIF_of"))
                tunnel_access_PIF_of = Marshalling.ParseSetRef<Tunnel>(table, "tunnel_access_PIF_of");
            if (table.ContainsKey("tunnel_transport_PIF_of"))
                tunnel_transport_PIF_of = Marshalling.ParseSetRef<Tunnel>(table, "tunnel_transport_PIF_of");
            if (table.ContainsKey("ipv6_configuration_mode"))
                ipv6_configuration_mode = (ipv6_configuration_mode)Helper.EnumParseDefault(typeof(ipv6_configuration_mode), Marshalling.ParseString(table, "ipv6_configuration_mode"));
            if (table.ContainsKey("IPv6"))
                IPv6 = Marshalling.ParseStringArray(table, "IPv6");
            if (table.ContainsKey("ipv6_gateway"))
                ipv6_gateway = Marshalling.ParseString(table, "ipv6_gateway");
            if (table.ContainsKey("primary_address_type"))
                primary_address_type = (primary_address_type)Helper.EnumParseDefault(typeof(primary_address_type), Marshalling.ParseString(table, "primary_address_type"));
            if (table.ContainsKey("managed"))
                managed = Marshalling.ParseBool(table, "managed");
            if (table.ContainsKey("properties"))
                properties = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "properties"));
            if (table.ContainsKey("capabilities"))
                capabilities = Marshalling.ParseStringArray(table, "capabilities");
            if (table.ContainsKey("igmp_snooping_status"))
                igmp_snooping_status = (pif_igmp_status)Helper.EnumParseDefault(typeof(pif_igmp_status), Marshalling.ParseString(table, "igmp_snooping_status"));
            if (table.ContainsKey("sriov_physical_PIF_of"))
                sriov_physical_PIF_of = Marshalling.ParseSetRef<Network_sriov>(table, "sriov_physical_PIF_of");
            if (table.ContainsKey("sriov_logical_PIF_of"))
                sriov_logical_PIF_of = Marshalling.ParseSetRef<Network_sriov>(table, "sriov_logical_PIF_of");
            if (table.ContainsKey("PCI"))
                PCI = Marshalling.ParseRef<PCI>(table, "PCI");
        }

        public bool DeepEquals(PIF other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._device, other._device) &&
                Helper.AreEqual2(this._network, other._network) &&
                Helper.AreEqual2(this._host, other._host) &&
                Helper.AreEqual2(this._MAC, other._MAC) &&
                Helper.AreEqual2(this._MTU, other._MTU) &&
                Helper.AreEqual2(this._VLAN, other._VLAN) &&
                Helper.AreEqual2(this._metrics, other._metrics) &&
                Helper.AreEqual2(this._physical, other._physical) &&
                Helper.AreEqual2(this._currently_attached, other._currently_attached) &&
                Helper.AreEqual2(this._ip_configuration_mode, other._ip_configuration_mode) &&
                Helper.AreEqual2(this._IP, other._IP) &&
                Helper.AreEqual2(this._netmask, other._netmask) &&
                Helper.AreEqual2(this._gateway, other._gateway) &&
                Helper.AreEqual2(this._DNS, other._DNS) &&
                Helper.AreEqual2(this._bond_slave_of, other._bond_slave_of) &&
                Helper.AreEqual2(this._bond_master_of, other._bond_master_of) &&
                Helper.AreEqual2(this._VLAN_master_of, other._VLAN_master_of) &&
                Helper.AreEqual2(this._VLAN_slave_of, other._VLAN_slave_of) &&
                Helper.AreEqual2(this._management, other._management) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._disallow_unplug, other._disallow_unplug) &&
                Helper.AreEqual2(this._tunnel_access_PIF_of, other._tunnel_access_PIF_of) &&
                Helper.AreEqual2(this._tunnel_transport_PIF_of, other._tunnel_transport_PIF_of) &&
                Helper.AreEqual2(this._ipv6_configuration_mode, other._ipv6_configuration_mode) &&
                Helper.AreEqual2(this._IPv6, other._IPv6) &&
                Helper.AreEqual2(this._ipv6_gateway, other._ipv6_gateway) &&
                Helper.AreEqual2(this._primary_address_type, other._primary_address_type) &&
                Helper.AreEqual2(this._managed, other._managed) &&
                Helper.AreEqual2(this._properties, other._properties) &&
                Helper.AreEqual2(this._capabilities, other._capabilities) &&
                Helper.AreEqual2(this._igmp_snooping_status, other._igmp_snooping_status) &&
                Helper.AreEqual2(this._sriov_physical_PIF_of, other._sriov_physical_PIF_of) &&
                Helper.AreEqual2(this._sriov_logical_PIF_of, other._sriov_logical_PIF_of) &&
                Helper.AreEqual2(this._PCI, other._PCI);
        }

        internal static List<PIF> ProxyArrayToObjectList(Proxy_PIF[] input)
        {
            var result = new List<PIF>();
            foreach (var item in input)
                result.Add(new PIF(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, PIF server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    PIF.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static PIF get_record(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_record(session.opaque_ref, _pif);
            else
                return new PIF(session.proxy.pif_get_record(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the PIF instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<PIF> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<PIF>.Create(session.proxy.pif_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string get_uuid(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_uuid(session.opaque_ref, _pif);
            else
                return session.proxy.pif_get_uuid(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Get the device field of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string get_device(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_device(session.opaque_ref, _pif);
            else
                return session.proxy.pif_get_device(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Get the network field of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<Network> get_network(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_network(session.opaque_ref, _pif);
            else
                return XenRef<Network>.Create(session.proxy.pif_get_network(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the host field of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<Host> get_host(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_host(session.opaque_ref, _pif);
            else
                return XenRef<Host>.Create(session.proxy.pif_get_host(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the MAC field of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string get_MAC(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_mac(session.opaque_ref, _pif);
            else
                return session.proxy.pif_get_mac(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Get the MTU field of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static long get_MTU(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_mtu(session.opaque_ref, _pif);
            else
                return long.Parse(session.proxy.pif_get_mtu(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the VLAN field of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static long get_VLAN(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_vlan(session.opaque_ref, _pif);
            else
                return long.Parse(session.proxy.pif_get_vlan(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the metrics field of the given PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<PIF_metrics> get_metrics(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_metrics(session.opaque_ref, _pif);
            else
                return XenRef<PIF_metrics>.Create(session.proxy.pif_get_metrics(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the physical field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static bool get_physical(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_physical(session.opaque_ref, _pif);
            else
                return (bool)session.proxy.pif_get_physical(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Get the currently_attached field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static bool get_currently_attached(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_currently_attached(session.opaque_ref, _pif);
            else
                return (bool)session.proxy.pif_get_currently_attached(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Get the ip_configuration_mode field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static ip_configuration_mode get_ip_configuration_mode(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_ip_configuration_mode(session.opaque_ref, _pif);
            else
                return (ip_configuration_mode)Helper.EnumParseDefault(typeof(ip_configuration_mode), (string)session.proxy.pif_get_ip_configuration_mode(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the IP field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string get_IP(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_ip(session.opaque_ref, _pif);
            else
                return session.proxy.pif_get_ip(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Get the netmask field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string get_netmask(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_netmask(session.opaque_ref, _pif);
            else
                return session.proxy.pif_get_netmask(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Get the gateway field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string get_gateway(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_gateway(session.opaque_ref, _pif);
            else
                return session.proxy.pif_get_gateway(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Get the DNS field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string get_DNS(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_dns(session.opaque_ref, _pif);
            else
                return session.proxy.pif_get_dns(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Get the bond_slave_of field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<Bond> get_bond_slave_of(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_bond_slave_of(session.opaque_ref, _pif);
            else
                return XenRef<Bond>.Create(session.proxy.pif_get_bond_slave_of(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the bond_master_of field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static List<XenRef<Bond>> get_bond_master_of(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_bond_master_of(session.opaque_ref, _pif);
            else
                return XenRef<Bond>.Create(session.proxy.pif_get_bond_master_of(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the VLAN_master_of field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<VLAN> get_VLAN_master_of(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_vlan_master_of(session.opaque_ref, _pif);
            else
                return XenRef<VLAN>.Create(session.proxy.pif_get_vlan_master_of(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the VLAN_slave_of field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static List<XenRef<VLAN>> get_VLAN_slave_of(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_vlan_slave_of(session.opaque_ref, _pif);
            else
                return XenRef<VLAN>.Create(session.proxy.pif_get_vlan_slave_of(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the management field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static bool get_management(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_management(session.opaque_ref, _pif);
            else
                return (bool)session.proxy.pif_get_management(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Get the other_config field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static Dictionary<string, string> get_other_config(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_other_config(session.opaque_ref, _pif);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.pif_get_other_config(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the disallow_unplug field of the given PIF.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static bool get_disallow_unplug(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_disallow_unplug(session.opaque_ref, _pif);
            else
                return (bool)session.proxy.pif_get_disallow_unplug(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Get the tunnel_access_PIF_of field of the given PIF.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static List<XenRef<Tunnel>> get_tunnel_access_PIF_of(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_tunnel_access_pif_of(session.opaque_ref, _pif);
            else
                return XenRef<Tunnel>.Create(session.proxy.pif_get_tunnel_access_pif_of(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the tunnel_transport_PIF_of field of the given PIF.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static List<XenRef<Tunnel>> get_tunnel_transport_PIF_of(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_tunnel_transport_pif_of(session.opaque_ref, _pif);
            else
                return XenRef<Tunnel>.Create(session.proxy.pif_get_tunnel_transport_pif_of(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the ipv6_configuration_mode field of the given PIF.
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static ipv6_configuration_mode get_ipv6_configuration_mode(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_ipv6_configuration_mode(session.opaque_ref, _pif);
            else
                return (ipv6_configuration_mode)Helper.EnumParseDefault(typeof(ipv6_configuration_mode), (string)session.proxy.pif_get_ipv6_configuration_mode(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the IPv6 field of the given PIF.
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string[] get_IPv6(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_ipv6(session.opaque_ref, _pif);
            else
                return (string [])session.proxy.pif_get_ipv6(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Get the ipv6_gateway field of the given PIF.
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string get_ipv6_gateway(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_ipv6_gateway(session.opaque_ref, _pif);
            else
                return session.proxy.pif_get_ipv6_gateway(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Get the primary_address_type field of the given PIF.
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static primary_address_type get_primary_address_type(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_primary_address_type(session.opaque_ref, _pif);
            else
                return (primary_address_type)Helper.EnumParseDefault(typeof(primary_address_type), (string)session.proxy.pif_get_primary_address_type(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the managed field of the given PIF.
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static bool get_managed(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_managed(session.opaque_ref, _pif);
            else
                return (bool)session.proxy.pif_get_managed(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Get the properties field of the given PIF.
        /// First published in XenServer 6.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static Dictionary<string, string> get_properties(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_properties(session.opaque_ref, _pif);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.pif_get_properties(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the capabilities field of the given PIF.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static string[] get_capabilities(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_capabilities(session.opaque_ref, _pif);
            else
                return (string [])session.proxy.pif_get_capabilities(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Get the igmp_snooping_status field of the given PIF.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static pif_igmp_status get_igmp_snooping_status(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_igmp_snooping_status(session.opaque_ref, _pif);
            else
                return (pif_igmp_status)Helper.EnumParseDefault(typeof(pif_igmp_status), (string)session.proxy.pif_get_igmp_snooping_status(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the sriov_physical_PIF_of field of the given PIF.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static List<XenRef<Network_sriov>> get_sriov_physical_PIF_of(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_sriov_physical_pif_of(session.opaque_ref, _pif);
            else
                return XenRef<Network_sriov>.Create(session.proxy.pif_get_sriov_physical_pif_of(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the sriov_logical_PIF_of field of the given PIF.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static List<XenRef<Network_sriov>> get_sriov_logical_PIF_of(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_sriov_logical_pif_of(session.opaque_ref, _pif);
            else
                return XenRef<Network_sriov>.Create(session.proxy.pif_get_sriov_logical_pif_of(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Get the PCI field of the given PIF.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<PCI> get_PCI(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_pci(session.opaque_ref, _pif);
            else
                return XenRef<PCI>.Create(session.proxy.pif_get_pci(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Set the other_config field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _pif, Dictionary<string, string> _other_config)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pif_set_other_config(session.opaque_ref, _pif, _other_config);
            else
                session.proxy.pif_set_other_config(session.opaque_ref, _pif ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given PIF.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _pif, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pif_add_to_other_config(session.opaque_ref, _pif, _key, _value);
            else
                session.proxy.pif_add_to_other_config(session.opaque_ref, _pif ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given PIF.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _pif, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pif_remove_from_other_config(session.opaque_ref, _pif, _key);
            else
                session.proxy.pif_remove_from_other_config(session.opaque_ref, _pif ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// Create a VLAN interface from an existing physical interface. This call is deprecated: use VLAN.create instead
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device">physical interface on which to create the VLAN interface</param>
        /// <param name="_network">network to which this interface should be connected</param>
        /// <param name="_host">physical machine to which this PIF is connected</param>
        /// <param name="_vlan">VLAN tag for the new interface</param>
        [Deprecated("XenServer 4.1")]
        public static XenRef<PIF> create_VLAN(Session session, string _device, string _network, string _host, long _vlan)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_create_vlan(session.opaque_ref, _device, _network, _host, _vlan);
            else
                return XenRef<PIF>.Create(session.proxy.pif_create_vlan(session.opaque_ref, _device ?? "", _network ?? "", _host ?? "", _vlan.ToString()).parse());
        }

        /// <summary>
        /// Create a VLAN interface from an existing physical interface. This call is deprecated: use VLAN.create instead
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device">physical interface on which to create the VLAN interface</param>
        /// <param name="_network">network to which this interface should be connected</param>
        /// <param name="_host">physical machine to which this PIF is connected</param>
        /// <param name="_vlan">VLAN tag for the new interface</param>
        [Deprecated("XenServer 4.1")]
        public static XenRef<Task> async_create_VLAN(Session session, string _device, string _network, string _host, long _vlan)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_create_vlan(session.opaque_ref, _device, _network, _host, _vlan);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_create_vlan(session.opaque_ref, _device ?? "", _network ?? "", _host ?? "", _vlan.ToString()).parse());
        }

        /// <summary>
        /// Destroy the PIF object (provided it is a VLAN interface). This call is deprecated: use VLAN.destroy or Bond.destroy instead
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        [Deprecated("XenServer 4.1")]
        public static void destroy(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pif_destroy(session.opaque_ref, _pif);
            else
                session.proxy.pif_destroy(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Destroy the PIF object (provided it is a VLAN interface). This call is deprecated: use VLAN.destroy or Bond.destroy instead
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        [Deprecated("XenServer 4.1")]
        public static XenRef<Task> async_destroy(Session session, string _pif)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_destroy(session.opaque_ref, _pif);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_destroy(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Reconfigure the IP address settings for this interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_mode">whether to use dynamic/static/no-assignment</param>
        /// <param name="_ip">the new IP address</param>
        /// <param name="_netmask">the new netmask</param>
        /// <param name="_gateway">the new gateway</param>
        /// <param name="_dns">the new DNS settings</param>
        public static void reconfigure_ip(Session session, string _pif, ip_configuration_mode _mode, string _ip, string _netmask, string _gateway, string _dns)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pif_reconfigure_ip(session.opaque_ref, _pif, _mode, _ip, _netmask, _gateway, _dns);
            else
                session.proxy.pif_reconfigure_ip(session.opaque_ref, _pif ?? "", ip_configuration_mode_helper.ToString(_mode), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "").parse();
        }

        /// <summary>
        /// Reconfigure the IP address settings for this interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_mode">whether to use dynamic/static/no-assignment</param>
        /// <param name="_ip">the new IP address</param>
        /// <param name="_netmask">the new netmask</param>
        /// <param name="_gateway">the new gateway</param>
        /// <param name="_dns">the new DNS settings</param>
        public static XenRef<Task> async_reconfigure_ip(Session session, string _pif, ip_configuration_mode _mode, string _ip, string _netmask, string _gateway, string _dns)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_reconfigure_ip(session.opaque_ref, _pif, _mode, _ip, _netmask, _gateway, _dns);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_reconfigure_ip(session.opaque_ref, _pif ?? "", ip_configuration_mode_helper.ToString(_mode), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "").parse());
        }

        /// <summary>
        /// Reconfigure the IPv6 address settings for this interface
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_mode">whether to use dynamic/static/no-assignment</param>
        /// <param name="_ipv6">the new IPv6 address (in &lt;addr&gt;/&lt;prefix length&gt; format)</param>
        /// <param name="_gateway">the new gateway</param>
        /// <param name="_dns">the new DNS settings</param>
        public static void reconfigure_ipv6(Session session, string _pif, ipv6_configuration_mode _mode, string _ipv6, string _gateway, string _dns)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pif_reconfigure_ipv6(session.opaque_ref, _pif, _mode, _ipv6, _gateway, _dns);
            else
                session.proxy.pif_reconfigure_ipv6(session.opaque_ref, _pif ?? "", ipv6_configuration_mode_helper.ToString(_mode), _ipv6 ?? "", _gateway ?? "", _dns ?? "").parse();
        }

        /// <summary>
        /// Reconfigure the IPv6 address settings for this interface
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_mode">whether to use dynamic/static/no-assignment</param>
        /// <param name="_ipv6">the new IPv6 address (in &lt;addr&gt;/&lt;prefix length&gt; format)</param>
        /// <param name="_gateway">the new gateway</param>
        /// <param name="_dns">the new DNS settings</param>
        public static XenRef<Task> async_reconfigure_ipv6(Session session, string _pif, ipv6_configuration_mode _mode, string _ipv6, string _gateway, string _dns)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_reconfigure_ipv6(session.opaque_ref, _pif, _mode, _ipv6, _gateway, _dns);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_reconfigure_ipv6(session.opaque_ref, _pif ?? "", ipv6_configuration_mode_helper.ToString(_mode), _ipv6 ?? "", _gateway ?? "", _dns ?? "").parse());
        }

        /// <summary>
        /// Change the primary address type used by this PIF
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_primary_address_type">Whether to prefer IPv4 or IPv6 connections</param>
        public static void set_primary_address_type(Session session, string _pif, primary_address_type _primary_address_type)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pif_set_primary_address_type(session.opaque_ref, _pif, _primary_address_type);
            else
                session.proxy.pif_set_primary_address_type(session.opaque_ref, _pif ?? "", primary_address_type_helper.ToString(_primary_address_type)).parse();
        }

        /// <summary>
        /// Change the primary address type used by this PIF
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_primary_address_type">Whether to prefer IPv4 or IPv6 connections</param>
        public static XenRef<Task> async_set_primary_address_type(Session session, string _pif, primary_address_type _primary_address_type)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_set_primary_address_type(session.opaque_ref, _pif, _primary_address_type);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_set_primary_address_type(session.opaque_ref, _pif ?? "", primary_address_type_helper.ToString(_primary_address_type)).parse());
        }

        /// <summary>
        /// Scan for physical interfaces on a host and create PIF objects to represent them
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host on which to scan</param>
        public static void scan(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pif_scan(session.opaque_ref, _host);
            else
                session.proxy.pif_scan(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Scan for physical interfaces on a host and create PIF objects to represent them
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host on which to scan</param>
        public static XenRef<Task> async_scan(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_scan(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_scan(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Create a PIF object matching a particular network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host on which the interface exists</param>
        /// <param name="_mac">The MAC address of the interface</param>
        /// <param name="_device">The device name to use for the interface</param>
        public static XenRef<PIF> introduce(Session session, string _host, string _mac, string _device)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_introduce(session.opaque_ref, _host, _mac, _device);
            else
                return XenRef<PIF>.Create(session.proxy.pif_introduce(session.opaque_ref, _host ?? "", _mac ?? "", _device ?? "").parse());
        }

        /// <summary>
        /// Create a PIF object matching a particular network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host on which the interface exists</param>
        /// <param name="_mac">The MAC address of the interface</param>
        /// <param name="_device">The device name to use for the interface</param>
        public static XenRef<Task> async_introduce(Session session, string _host, string _mac, string _device)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_introduce(session.opaque_ref, _host, _mac, _device);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_introduce(session.opaque_ref, _host ?? "", _mac ?? "", _device ?? "").parse());
        }

        /// <summary>
        /// Create a PIF object matching a particular network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host on which the interface exists</param>
        /// <param name="_mac">The MAC address of the interface</param>
        /// <param name="_device">The device name to use for the interface</param>
        /// <param name="_managed">Indicates whether the interface is managed by xapi (defaults to "true") First published in XenServer 6.2 SP1.</param>
        public static XenRef<PIF> introduce(Session session, string _host, string _mac, string _device, bool _managed)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_introduce(session.opaque_ref, _host, _mac, _device, _managed);
            else
                return XenRef<PIF>.Create(session.proxy.pif_introduce(session.opaque_ref, _host ?? "", _mac ?? "", _device ?? "", _managed).parse());
        }

        /// <summary>
        /// Create a PIF object matching a particular network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host on which the interface exists</param>
        /// <param name="_mac">The MAC address of the interface</param>
        /// <param name="_device">The device name to use for the interface</param>
        /// <param name="_managed">Indicates whether the interface is managed by xapi (defaults to "true") First published in XenServer 6.2 SP1.</param>
        public static XenRef<Task> async_introduce(Session session, string _host, string _mac, string _device, bool _managed)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_introduce(session.opaque_ref, _host, _mac, _device, _managed);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_introduce(session.opaque_ref, _host ?? "", _mac ?? "", _device ?? "", _managed).parse());
        }

        /// <summary>
        /// Destroy the PIF object matching a particular network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static void forget(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pif_forget(session.opaque_ref, _pif);
            else
                session.proxy.pif_forget(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Destroy the PIF object matching a particular network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<Task> async_forget(Session session, string _pif)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_forget(session.opaque_ref, _pif);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_forget(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Attempt to bring down a physical interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static void unplug(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pif_unplug(session.opaque_ref, _pif);
            else
                session.proxy.pif_unplug(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Attempt to bring down a physical interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<Task> async_unplug(Session session, string _pif)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_unplug(session.opaque_ref, _pif);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_unplug(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Set whether unplugging the PIF is allowed
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_value">New value to set</param>
        public static void set_disallow_unplug(Session session, string _pif, bool _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pif_set_disallow_unplug(session.opaque_ref, _pif, _value);
            else
                session.proxy.pif_set_disallow_unplug(session.opaque_ref, _pif ?? "", _value).parse();
        }

        /// <summary>
        /// Set whether unplugging the PIF is allowed
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_value">New value to set</param>
        public static XenRef<Task> async_set_disallow_unplug(Session session, string _pif, bool _value)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_set_disallow_unplug(session.opaque_ref, _pif, _value);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_set_disallow_unplug(session.opaque_ref, _pif ?? "", _value).parse());
        }

        /// <summary>
        /// Attempt to bring up a physical interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static void plug(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pif_plug(session.opaque_ref, _pif);
            else
                session.proxy.pif_plug(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Attempt to bring up a physical interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<Task> async_plug(Session session, string _pif)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_plug(session.opaque_ref, _pif);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_plug(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Create a new PIF record in the database only
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device"></param>
        /// <param name="_network"></param>
        /// <param name="_host"></param>
        /// <param name="_mac"></param>
        /// <param name="_mtu"></param>
        /// <param name="_vlan"></param>
        /// <param name="_physical"></param>
        /// <param name="_ip_configuration_mode"></param>
        /// <param name="_ip"></param>
        /// <param name="_netmask"></param>
        /// <param name="_gateway"></param>
        /// <param name="_dns"></param>
        /// <param name="_bond_slave_of"></param>
        /// <param name="_vlan_master_of"></param>
        /// <param name="_management"></param>
        /// <param name="_other_config"></param>
        /// <param name="_disallow_unplug"></param>
        public static XenRef<PIF> db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_db_introduce(session.opaque_ref, _device, _network, _host, _mac, _mtu, _vlan, _physical, _ip_configuration_mode, _ip, _netmask, _gateway, _dns, _bond_slave_of, _vlan_master_of, _management, _other_config, _disallow_unplug);
            else
                return XenRef<PIF>.Create(session.proxy.pif_db_introduce(session.opaque_ref, _device ?? "", _network ?? "", _host ?? "", _mac ?? "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "", _bond_slave_of ?? "", _vlan_master_of ?? "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug).parse());
        }

        /// <summary>
        /// Create a new PIF record in the database only
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device"></param>
        /// <param name="_network"></param>
        /// <param name="_host"></param>
        /// <param name="_mac"></param>
        /// <param name="_mtu"></param>
        /// <param name="_vlan"></param>
        /// <param name="_physical"></param>
        /// <param name="_ip_configuration_mode"></param>
        /// <param name="_ip"></param>
        /// <param name="_netmask"></param>
        /// <param name="_gateway"></param>
        /// <param name="_dns"></param>
        /// <param name="_bond_slave_of"></param>
        /// <param name="_vlan_master_of"></param>
        /// <param name="_management"></param>
        /// <param name="_other_config"></param>
        /// <param name="_disallow_unplug"></param>
        public static XenRef<Task> async_db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_db_introduce(session.opaque_ref, _device, _network, _host, _mac, _mtu, _vlan, _physical, _ip_configuration_mode, _ip, _netmask, _gateway, _dns, _bond_slave_of, _vlan_master_of, _management, _other_config, _disallow_unplug);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_db_introduce(session.opaque_ref, _device ?? "", _network ?? "", _host ?? "", _mac ?? "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "", _bond_slave_of ?? "", _vlan_master_of ?? "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug).parse());
        }

        /// <summary>
        /// Create a new PIF record in the database only
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device"></param>
        /// <param name="_network"></param>
        /// <param name="_host"></param>
        /// <param name="_mac"></param>
        /// <param name="_mtu"></param>
        /// <param name="_vlan"></param>
        /// <param name="_physical"></param>
        /// <param name="_ip_configuration_mode"></param>
        /// <param name="_ip"></param>
        /// <param name="_netmask"></param>
        /// <param name="_gateway"></param>
        /// <param name="_dns"></param>
        /// <param name="_bond_slave_of"></param>
        /// <param name="_vlan_master_of"></param>
        /// <param name="_management"></param>
        /// <param name="_other_config"></param>
        /// <param name="_disallow_unplug"></param>
        /// <param name="_ipv6_configuration_mode"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6_gateway"> First published in XenServer 6.0.</param>
        /// <param name="_primary_address_type"> First published in XenServer 6.0.</param>
        public static XenRef<PIF> db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_db_introduce(session.opaque_ref, _device, _network, _host, _mac, _mtu, _vlan, _physical, _ip_configuration_mode, _ip, _netmask, _gateway, _dns, _bond_slave_of, _vlan_master_of, _management, _other_config, _disallow_unplug, _ipv6_configuration_mode, _ipv6, _ipv6_gateway, _primary_address_type);
            else
                return XenRef<PIF>.Create(session.proxy.pif_db_introduce(session.opaque_ref, _device ?? "", _network ?? "", _host ?? "", _mac ?? "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "", _bond_slave_of ?? "", _vlan_master_of ?? "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug, ipv6_configuration_mode_helper.ToString(_ipv6_configuration_mode), _ipv6, _ipv6_gateway ?? "", primary_address_type_helper.ToString(_primary_address_type)).parse());
        }

        /// <summary>
        /// Create a new PIF record in the database only
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device"></param>
        /// <param name="_network"></param>
        /// <param name="_host"></param>
        /// <param name="_mac"></param>
        /// <param name="_mtu"></param>
        /// <param name="_vlan"></param>
        /// <param name="_physical"></param>
        /// <param name="_ip_configuration_mode"></param>
        /// <param name="_ip"></param>
        /// <param name="_netmask"></param>
        /// <param name="_gateway"></param>
        /// <param name="_dns"></param>
        /// <param name="_bond_slave_of"></param>
        /// <param name="_vlan_master_of"></param>
        /// <param name="_management"></param>
        /// <param name="_other_config"></param>
        /// <param name="_disallow_unplug"></param>
        /// <param name="_ipv6_configuration_mode"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6_gateway"> First published in XenServer 6.0.</param>
        /// <param name="_primary_address_type"> First published in XenServer 6.0.</param>
        public static XenRef<Task> async_db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_db_introduce(session.opaque_ref, _device, _network, _host, _mac, _mtu, _vlan, _physical, _ip_configuration_mode, _ip, _netmask, _gateway, _dns, _bond_slave_of, _vlan_master_of, _management, _other_config, _disallow_unplug, _ipv6_configuration_mode, _ipv6, _ipv6_gateway, _primary_address_type);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_db_introduce(session.opaque_ref, _device ?? "", _network ?? "", _host ?? "", _mac ?? "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "", _bond_slave_of ?? "", _vlan_master_of ?? "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug, ipv6_configuration_mode_helper.ToString(_ipv6_configuration_mode), _ipv6, _ipv6_gateway ?? "", primary_address_type_helper.ToString(_primary_address_type)).parse());
        }

        /// <summary>
        /// Create a new PIF record in the database only
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device"></param>
        /// <param name="_network"></param>
        /// <param name="_host"></param>
        /// <param name="_mac"></param>
        /// <param name="_mtu"></param>
        /// <param name="_vlan"></param>
        /// <param name="_physical"></param>
        /// <param name="_ip_configuration_mode"></param>
        /// <param name="_ip"></param>
        /// <param name="_netmask"></param>
        /// <param name="_gateway"></param>
        /// <param name="_dns"></param>
        /// <param name="_bond_slave_of"></param>
        /// <param name="_vlan_master_of"></param>
        /// <param name="_management"></param>
        /// <param name="_other_config"></param>
        /// <param name="_disallow_unplug"></param>
        /// <param name="_ipv6_configuration_mode"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6_gateway"> First published in XenServer 6.0.</param>
        /// <param name="_primary_address_type"> First published in XenServer 6.0.</param>
        /// <param name="_managed"> First published in XenServer 6.2 SP1.</param>
        public static XenRef<PIF> db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type, bool _managed)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_db_introduce(session.opaque_ref, _device, _network, _host, _mac, _mtu, _vlan, _physical, _ip_configuration_mode, _ip, _netmask, _gateway, _dns, _bond_slave_of, _vlan_master_of, _management, _other_config, _disallow_unplug, _ipv6_configuration_mode, _ipv6, _ipv6_gateway, _primary_address_type, _managed);
            else
                return XenRef<PIF>.Create(session.proxy.pif_db_introduce(session.opaque_ref, _device ?? "", _network ?? "", _host ?? "", _mac ?? "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "", _bond_slave_of ?? "", _vlan_master_of ?? "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug, ipv6_configuration_mode_helper.ToString(_ipv6_configuration_mode), _ipv6, _ipv6_gateway ?? "", primary_address_type_helper.ToString(_primary_address_type), _managed).parse());
        }

        /// <summary>
        /// Create a new PIF record in the database only
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device"></param>
        /// <param name="_network"></param>
        /// <param name="_host"></param>
        /// <param name="_mac"></param>
        /// <param name="_mtu"></param>
        /// <param name="_vlan"></param>
        /// <param name="_physical"></param>
        /// <param name="_ip_configuration_mode"></param>
        /// <param name="_ip"></param>
        /// <param name="_netmask"></param>
        /// <param name="_gateway"></param>
        /// <param name="_dns"></param>
        /// <param name="_bond_slave_of"></param>
        /// <param name="_vlan_master_of"></param>
        /// <param name="_management"></param>
        /// <param name="_other_config"></param>
        /// <param name="_disallow_unplug"></param>
        /// <param name="_ipv6_configuration_mode"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6_gateway"> First published in XenServer 6.0.</param>
        /// <param name="_primary_address_type"> First published in XenServer 6.0.</param>
        /// <param name="_managed"> First published in XenServer 6.2 SP1.</param>
        public static XenRef<Task> async_db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type, bool _managed)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_db_introduce(session.opaque_ref, _device, _network, _host, _mac, _mtu, _vlan, _physical, _ip_configuration_mode, _ip, _netmask, _gateway, _dns, _bond_slave_of, _vlan_master_of, _management, _other_config, _disallow_unplug, _ipv6_configuration_mode, _ipv6, _ipv6_gateway, _primary_address_type, _managed);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_db_introduce(session.opaque_ref, _device ?? "", _network ?? "", _host ?? "", _mac ?? "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "", _bond_slave_of ?? "", _vlan_master_of ?? "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug, ipv6_configuration_mode_helper.ToString(_ipv6_configuration_mode), _ipv6, _ipv6_gateway ?? "", primary_address_type_helper.ToString(_primary_address_type), _managed).parse());
        }

        /// <summary>
        /// Create a new PIF record in the database only
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device"></param>
        /// <param name="_network"></param>
        /// <param name="_host"></param>
        /// <param name="_mac"></param>
        /// <param name="_mtu"></param>
        /// <param name="_vlan"></param>
        /// <param name="_physical"></param>
        /// <param name="_ip_configuration_mode"></param>
        /// <param name="_ip"></param>
        /// <param name="_netmask"></param>
        /// <param name="_gateway"></param>
        /// <param name="_dns"></param>
        /// <param name="_bond_slave_of"></param>
        /// <param name="_vlan_master_of"></param>
        /// <param name="_management"></param>
        /// <param name="_other_config"></param>
        /// <param name="_disallow_unplug"></param>
        /// <param name="_ipv6_configuration_mode"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6_gateway"> First published in XenServer 6.0.</param>
        /// <param name="_primary_address_type"> First published in XenServer 6.0.</param>
        /// <param name="_managed"> First published in XenServer 6.2 SP1.</param>
        /// <param name="_properties"> First published in XenServer 6.5.</param>
        public static XenRef<PIF> db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type, bool _managed, Dictionary<string, string> _properties)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_db_introduce(session.opaque_ref, _device, _network, _host, _mac, _mtu, _vlan, _physical, _ip_configuration_mode, _ip, _netmask, _gateway, _dns, _bond_slave_of, _vlan_master_of, _management, _other_config, _disallow_unplug, _ipv6_configuration_mode, _ipv6, _ipv6_gateway, _primary_address_type, _managed, _properties);
            else
                return XenRef<PIF>.Create(session.proxy.pif_db_introduce(session.opaque_ref, _device ?? "", _network ?? "", _host ?? "", _mac ?? "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "", _bond_slave_of ?? "", _vlan_master_of ?? "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug, ipv6_configuration_mode_helper.ToString(_ipv6_configuration_mode), _ipv6, _ipv6_gateway ?? "", primary_address_type_helper.ToString(_primary_address_type), _managed, Maps.convert_to_proxy_string_string(_properties)).parse());
        }

        /// <summary>
        /// Create a new PIF record in the database only
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device"></param>
        /// <param name="_network"></param>
        /// <param name="_host"></param>
        /// <param name="_mac"></param>
        /// <param name="_mtu"></param>
        /// <param name="_vlan"></param>
        /// <param name="_physical"></param>
        /// <param name="_ip_configuration_mode"></param>
        /// <param name="_ip"></param>
        /// <param name="_netmask"></param>
        /// <param name="_gateway"></param>
        /// <param name="_dns"></param>
        /// <param name="_bond_slave_of"></param>
        /// <param name="_vlan_master_of"></param>
        /// <param name="_management"></param>
        /// <param name="_other_config"></param>
        /// <param name="_disallow_unplug"></param>
        /// <param name="_ipv6_configuration_mode"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6"> First published in XenServer 6.0.</param>
        /// <param name="_ipv6_gateway"> First published in XenServer 6.0.</param>
        /// <param name="_primary_address_type"> First published in XenServer 6.0.</param>
        /// <param name="_managed"> First published in XenServer 6.2 SP1.</param>
        /// <param name="_properties"> First published in XenServer 6.5.</param>
        public static XenRef<Task> async_db_introduce(Session session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type, bool _managed, Dictionary<string, string> _properties)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_db_introduce(session.opaque_ref, _device, _network, _host, _mac, _mtu, _vlan, _physical, _ip_configuration_mode, _ip, _netmask, _gateway, _dns, _bond_slave_of, _vlan_master_of, _management, _other_config, _disallow_unplug, _ipv6_configuration_mode, _ipv6, _ipv6_gateway, _primary_address_type, _managed, _properties);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_db_introduce(session.opaque_ref, _device ?? "", _network ?? "", _host ?? "", _mac ?? "", _mtu.ToString(), _vlan.ToString(), _physical, ip_configuration_mode_helper.ToString(_ip_configuration_mode), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "", _bond_slave_of ?? "", _vlan_master_of ?? "", _management, Maps.convert_to_proxy_string_string(_other_config), _disallow_unplug, ipv6_configuration_mode_helper.ToString(_ipv6_configuration_mode), _ipv6, _ipv6_gateway ?? "", primary_address_type_helper.ToString(_primary_address_type), _managed, Maps.convert_to_proxy_string_string(_properties)).parse());
        }

        /// <summary>
        /// Destroy a PIF database record.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static void db_forget(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pif_db_forget(session.opaque_ref, _pif);
            else
                session.proxy.pif_db_forget(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Destroy a PIF database record.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        public static XenRef<Task> async_db_forget(Session session, string _pif)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_db_forget(session.opaque_ref, _pif);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_db_forget(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Set the value of a property of the PIF
        /// First published in XenServer 6.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_name">The property name</param>
        /// <param name="_value">The property value</param>
        public static void set_property(Session session, string _pif, string _name, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pif_set_property(session.opaque_ref, _pif, _name, _value);
            else
                session.proxy.pif_set_property(session.opaque_ref, _pif ?? "", _name ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Set the value of a property of the PIF
        /// First published in XenServer 6.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The opaque_ref of the given pif</param>
        /// <param name="_name">The property name</param>
        /// <param name="_value">The property value</param>
        public static XenRef<Task> async_set_property(Session session, string _pif, string _name, string _value)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pif_set_property(session.opaque_ref, _pif, _name, _value);
          else
              return XenRef<Task>.Create(session.proxy.async_pif_set_property(session.opaque_ref, _pif ?? "", _name ?? "", _value ?? "").parse());
        }

        /// <summary>
        /// Return a list of all the PIFs known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<PIF>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_all(session.opaque_ref);
            else
                return XenRef<PIF>.Create(session.proxy.pif_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the PIF Records at once, in a single XML RPC call
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<PIF>, PIF> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pif_get_all_records(session.opaque_ref);
            else
                return XenRef<PIF>.Create<Proxy_PIF>(session.proxy.pif_get_all_records(session.opaque_ref).parse());
        }

        /// <summary>
        /// Unique identifier/object reference
        /// </summary>
        public virtual string uuid
        {
            get { return _uuid; }
            set
            {
                if (!Helper.AreEqual(value, _uuid))
                {
                    _uuid = value;
                    Changed = true;
                    NotifyPropertyChanged("uuid");
                }
            }
        }
        private string _uuid = "";

        /// <summary>
        /// machine-readable name of the interface (e.g. eth0)
        /// </summary>
        public virtual string device
        {
            get { return _device; }
            set
            {
                if (!Helper.AreEqual(value, _device))
                {
                    _device = value;
                    Changed = true;
                    NotifyPropertyChanged("device");
                }
            }
        }
        private string _device = "";

        /// <summary>
        /// virtual network to which this pif is connected
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Network>))]
        public virtual XenRef<Network> network
        {
            get { return _network; }
            set
            {
                if (!Helper.AreEqual(value, _network))
                {
                    _network = value;
                    Changed = true;
                    NotifyPropertyChanged("network");
                }
            }
        }
        private XenRef<Network> _network = new XenRef<Network>(Helper.NullOpaqueRef);

        /// <summary>
        /// physical machine to which this pif is connected
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Host>))]
        public virtual XenRef<Host> host
        {
            get { return _host; }
            set
            {
                if (!Helper.AreEqual(value, _host))
                {
                    _host = value;
                    Changed = true;
                    NotifyPropertyChanged("host");
                }
            }
        }
        private XenRef<Host> _host = new XenRef<Host>(Helper.NullOpaqueRef);

        /// <summary>
        /// ethernet MAC address of physical interface
        /// </summary>
        public virtual string MAC
        {
            get { return _MAC; }
            set
            {
                if (!Helper.AreEqual(value, _MAC))
                {
                    _MAC = value;
                    Changed = true;
                    NotifyPropertyChanged("MAC");
                }
            }
        }
        private string _MAC = "";

        /// <summary>
        /// MTU in octets
        /// </summary>
        public virtual long MTU
        {
            get { return _MTU; }
            set
            {
                if (!Helper.AreEqual(value, _MTU))
                {
                    _MTU = value;
                    Changed = true;
                    NotifyPropertyChanged("MTU");
                }
            }
        }
        private long _MTU;

        /// <summary>
        /// VLAN tag for all traffic passing through this interface
        /// </summary>
        public virtual long VLAN
        {
            get { return _VLAN; }
            set
            {
                if (!Helper.AreEqual(value, _VLAN))
                {
                    _VLAN = value;
                    Changed = true;
                    NotifyPropertyChanged("VLAN");
                }
            }
        }
        private long _VLAN;

        /// <summary>
        /// metrics associated with this PIF
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PIF_metrics>))]
        public virtual XenRef<PIF_metrics> metrics
        {
            get { return _metrics; }
            set
            {
                if (!Helper.AreEqual(value, _metrics))
                {
                    _metrics = value;
                    Changed = true;
                    NotifyPropertyChanged("metrics");
                }
            }
        }
        private XenRef<PIF_metrics> _metrics = new XenRef<PIF_metrics>(Helper.NullOpaqueRef);

        /// <summary>
        /// true if this represents a physical network interface
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual bool physical
        {
            get { return _physical; }
            set
            {
                if (!Helper.AreEqual(value, _physical))
                {
                    _physical = value;
                    Changed = true;
                    NotifyPropertyChanged("physical");
                }
            }
        }
        private bool _physical = false;

        /// <summary>
        /// true if this interface is online
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual bool currently_attached
        {
            get { return _currently_attached; }
            set
            {
                if (!Helper.AreEqual(value, _currently_attached))
                {
                    _currently_attached = value;
                    Changed = true;
                    NotifyPropertyChanged("currently_attached");
                }
            }
        }
        private bool _currently_attached = true;

        /// <summary>
        /// Sets if and how this interface gets an IP address
        /// First published in XenServer 4.1.
        /// </summary>
        [JsonConverter(typeof(ip_configuration_modeConverter))]
        public virtual ip_configuration_mode ip_configuration_mode
        {
            get { return _ip_configuration_mode; }
            set
            {
                if (!Helper.AreEqual(value, _ip_configuration_mode))
                {
                    _ip_configuration_mode = value;
                    Changed = true;
                    NotifyPropertyChanged("ip_configuration_mode");
                }
            }
        }
        private ip_configuration_mode _ip_configuration_mode = ip_configuration_mode.None;

        /// <summary>
        /// IP address
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual string IP
        {
            get { return _IP; }
            set
            {
                if (!Helper.AreEqual(value, _IP))
                {
                    _IP = value;
                    Changed = true;
                    NotifyPropertyChanged("IP");
                }
            }
        }
        private string _IP = "";

        /// <summary>
        /// IP netmask
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual string netmask
        {
            get { return _netmask; }
            set
            {
                if (!Helper.AreEqual(value, _netmask))
                {
                    _netmask = value;
                    Changed = true;
                    NotifyPropertyChanged("netmask");
                }
            }
        }
        private string _netmask = "";

        /// <summary>
        /// IP gateway
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual string gateway
        {
            get { return _gateway; }
            set
            {
                if (!Helper.AreEqual(value, _gateway))
                {
                    _gateway = value;
                    Changed = true;
                    NotifyPropertyChanged("gateway");
                }
            }
        }
        private string _gateway = "";

        /// <summary>
        /// IP address of DNS servers to use
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual string DNS
        {
            get { return _DNS; }
            set
            {
                if (!Helper.AreEqual(value, _DNS))
                {
                    _DNS = value;
                    Changed = true;
                    NotifyPropertyChanged("DNS");
                }
            }
        }
        private string _DNS = "";

        /// <summary>
        /// Indicates which bond this interface is part of
        /// First published in XenServer 4.1.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Bond>))]
        public virtual XenRef<Bond> bond_slave_of
        {
            get { return _bond_slave_of; }
            set
            {
                if (!Helper.AreEqual(value, _bond_slave_of))
                {
                    _bond_slave_of = value;
                    Changed = true;
                    NotifyPropertyChanged("bond_slave_of");
                }
            }
        }
        private XenRef<Bond> _bond_slave_of = new XenRef<Bond>(Helper.NullOpaqueRef);

        /// <summary>
        /// Indicates this PIF represents the results of a bond
        /// First published in XenServer 4.1.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Bond>))]
        public virtual List<XenRef<Bond>> bond_master_of
        {
            get { return _bond_master_of; }
            set
            {
                if (!Helper.AreEqual(value, _bond_master_of))
                {
                    _bond_master_of = value;
                    Changed = true;
                    NotifyPropertyChanged("bond_master_of");
                }
            }
        }
        private List<XenRef<Bond>> _bond_master_of = new List<XenRef<Bond>>() {};

        /// <summary>
        /// Indicates wich VLAN this interface receives untagged traffic from
        /// First published in XenServer 4.1.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VLAN>))]
        public virtual XenRef<VLAN> VLAN_master_of
        {
            get { return _VLAN_master_of; }
            set
            {
                if (!Helper.AreEqual(value, _VLAN_master_of))
                {
                    _VLAN_master_of = value;
                    Changed = true;
                    NotifyPropertyChanged("VLAN_master_of");
                }
            }
        }
        private XenRef<VLAN> _VLAN_master_of = new XenRef<VLAN>(Helper.NullOpaqueRef);

        /// <summary>
        /// Indicates which VLANs this interface transmits tagged traffic to
        /// First published in XenServer 4.1.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VLAN>))]
        public virtual List<XenRef<VLAN>> VLAN_slave_of
        {
            get { return _VLAN_slave_of; }
            set
            {
                if (!Helper.AreEqual(value, _VLAN_slave_of))
                {
                    _VLAN_slave_of = value;
                    Changed = true;
                    NotifyPropertyChanged("VLAN_slave_of");
                }
            }
        }
        private List<XenRef<VLAN>> _VLAN_slave_of = new List<XenRef<VLAN>>() {};

        /// <summary>
        /// Indicates whether the control software is listening for connections on this interface
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual bool management
        {
            get { return _management; }
            set
            {
                if (!Helper.AreEqual(value, _management))
                {
                    _management = value;
                    Changed = true;
                    NotifyPropertyChanged("management");
                }
            }
        }
        private bool _management = false;

        /// <summary>
        /// Additional configuration
        /// First published in XenServer 4.1.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> other_config
        {
            get { return _other_config; }
            set
            {
                if (!Helper.AreEqual(value, _other_config))
                {
                    _other_config = value;
                    Changed = true;
                    NotifyPropertyChanged("other_config");
                }
            }
        }
        private Dictionary<string, string> _other_config = new Dictionary<string, string>() {};

        /// <summary>
        /// Prevent this PIF from being unplugged; set this to notify the management tool-stack that the PIF has a special use and should not be unplugged under any circumstances (e.g. because you're running storage traffic over it)
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual bool disallow_unplug
        {
            get { return _disallow_unplug; }
            set
            {
                if (!Helper.AreEqual(value, _disallow_unplug))
                {
                    _disallow_unplug = value;
                    Changed = true;
                    NotifyPropertyChanged("disallow_unplug");
                }
            }
        }
        private bool _disallow_unplug = false;

        /// <summary>
        /// Indicates to which tunnel this PIF gives access
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Tunnel>))]
        public virtual List<XenRef<Tunnel>> tunnel_access_PIF_of
        {
            get { return _tunnel_access_PIF_of; }
            set
            {
                if (!Helper.AreEqual(value, _tunnel_access_PIF_of))
                {
                    _tunnel_access_PIF_of = value;
                    Changed = true;
                    NotifyPropertyChanged("tunnel_access_PIF_of");
                }
            }
        }
        private List<XenRef<Tunnel>> _tunnel_access_PIF_of = new List<XenRef<Tunnel>>() {};

        /// <summary>
        /// Indicates to which tunnel this PIF provides transport
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Tunnel>))]
        public virtual List<XenRef<Tunnel>> tunnel_transport_PIF_of
        {
            get { return _tunnel_transport_PIF_of; }
            set
            {
                if (!Helper.AreEqual(value, _tunnel_transport_PIF_of))
                {
                    _tunnel_transport_PIF_of = value;
                    Changed = true;
                    NotifyPropertyChanged("tunnel_transport_PIF_of");
                }
            }
        }
        private List<XenRef<Tunnel>> _tunnel_transport_PIF_of = new List<XenRef<Tunnel>>() {};

        /// <summary>
        /// Sets if and how this interface gets an IPv6 address
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        [JsonConverter(typeof(ipv6_configuration_modeConverter))]
        public virtual ipv6_configuration_mode ipv6_configuration_mode
        {
            get { return _ipv6_configuration_mode; }
            set
            {
                if (!Helper.AreEqual(value, _ipv6_configuration_mode))
                {
                    _ipv6_configuration_mode = value;
                    Changed = true;
                    NotifyPropertyChanged("ipv6_configuration_mode");
                }
            }
        }
        private ipv6_configuration_mode _ipv6_configuration_mode = ipv6_configuration_mode.None;

        /// <summary>
        /// IPv6 address
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        public virtual string[] IPv6
        {
            get { return _IPv6; }
            set
            {
                if (!Helper.AreEqual(value, _IPv6))
                {
                    _IPv6 = value;
                    Changed = true;
                    NotifyPropertyChanged("IPv6");
                }
            }
        }
        private string[] _IPv6 = {};

        /// <summary>
        /// IPv6 gateway
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        public virtual string ipv6_gateway
        {
            get { return _ipv6_gateway; }
            set
            {
                if (!Helper.AreEqual(value, _ipv6_gateway))
                {
                    _ipv6_gateway = value;
                    Changed = true;
                    NotifyPropertyChanged("ipv6_gateway");
                }
            }
        }
        private string _ipv6_gateway = "";

        /// <summary>
        /// Which protocol should define the primary address of this interface
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        [JsonConverter(typeof(primary_address_typeConverter))]
        public virtual primary_address_type primary_address_type
        {
            get { return _primary_address_type; }
            set
            {
                if (!Helper.AreEqual(value, _primary_address_type))
                {
                    _primary_address_type = value;
                    Changed = true;
                    NotifyPropertyChanged("primary_address_type");
                }
            }
        }
        private primary_address_type _primary_address_type = primary_address_type.IPv4;

        /// <summary>
        /// Indicates whether the interface is managed by xapi. If it is not, then xapi will not configure the interface, the commands PIF.plug/unplug/reconfigure_ip(v6) can not be used, nor can the interface be bonded or have VLANs based on top through xapi.
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        public virtual bool managed
        {
            get { return _managed; }
            set
            {
                if (!Helper.AreEqual(value, _managed))
                {
                    _managed = value;
                    Changed = true;
                    NotifyPropertyChanged("managed");
                }
            }
        }
        private bool _managed = true;

        /// <summary>
        /// Additional configuration properties for the interface.
        /// First published in XenServer 6.5.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> properties
        {
            get { return _properties; }
            set
            {
                if (!Helper.AreEqual(value, _properties))
                {
                    _properties = value;
                    Changed = true;
                    NotifyPropertyChanged("properties");
                }
            }
        }
        private Dictionary<string, string> _properties = new Dictionary<string, string>() {};

        /// <summary>
        /// Additional capabilities on the interface.
        /// First published in XenServer 7.0.
        /// </summary>
        public virtual string[] capabilities
        {
            get { return _capabilities; }
            set
            {
                if (!Helper.AreEqual(value, _capabilities))
                {
                    _capabilities = value;
                    Changed = true;
                    NotifyPropertyChanged("capabilities");
                }
            }
        }
        private string[] _capabilities = {};

        /// <summary>
        /// The IGMP snooping status of the corresponding network bridge
        /// First published in XenServer 7.3.
        /// </summary>
        [JsonConverter(typeof(pif_igmp_statusConverter))]
        public virtual pif_igmp_status igmp_snooping_status
        {
            get { return _igmp_snooping_status; }
            set
            {
                if (!Helper.AreEqual(value, _igmp_snooping_status))
                {
                    _igmp_snooping_status = value;
                    Changed = true;
                    NotifyPropertyChanged("igmp_snooping_status");
                }
            }
        }
        private pif_igmp_status _igmp_snooping_status = pif_igmp_status.unknown;

        /// <summary>
        /// Indicates which network_sriov this interface is physical of
        /// First published in Unreleased.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Network_sriov>))]
        public virtual List<XenRef<Network_sriov>> sriov_physical_PIF_of
        {
            get { return _sriov_physical_PIF_of; }
            set
            {
                if (!Helper.AreEqual(value, _sriov_physical_PIF_of))
                {
                    _sriov_physical_PIF_of = value;
                    Changed = true;
                    NotifyPropertyChanged("sriov_physical_PIF_of");
                }
            }
        }
        private List<XenRef<Network_sriov>> _sriov_physical_PIF_of = new List<XenRef<Network_sriov>>() {};

        /// <summary>
        /// Indicates which network_sriov this interface is logical of
        /// First published in Unreleased.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Network_sriov>))]
        public virtual List<XenRef<Network_sriov>> sriov_logical_PIF_of
        {
            get { return _sriov_logical_PIF_of; }
            set
            {
                if (!Helper.AreEqual(value, _sriov_logical_PIF_of))
                {
                    _sriov_logical_PIF_of = value;
                    Changed = true;
                    NotifyPropertyChanged("sriov_logical_PIF_of");
                }
            }
        }
        private List<XenRef<Network_sriov>> _sriov_logical_PIF_of = new List<XenRef<Network_sriov>>() {};

        /// <summary>
        /// Link to underlying PCI device
        /// First published in Unreleased.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PCI>))]
        public virtual XenRef<PCI> PCI
        {
            get { return _PCI; }
            set
            {
                if (!Helper.AreEqual(value, _PCI))
                {
                    _PCI = value;
                    Changed = true;
                    NotifyPropertyChanged("PCI");
                }
            }
        }
        private XenRef<PCI> _PCI = new XenRef<PCI>("OpaqueRef:NULL");
    }
}
