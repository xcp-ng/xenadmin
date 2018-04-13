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
    /// network-sriov which connects logical pif and physical pif
    /// First published in Unreleased.
    /// </summary>
    public partial class Network_sriov : XenObject<Network_sriov>
    {
        public Network_sriov()
        {
        }

        public Network_sriov(string uuid,
            XenRef<PIF> physical_PIF,
            XenRef<PIF> logical_PIF,
            bool requires_reboot,
            sriov_configuration_mode configuration_mode)
        {
            this.uuid = uuid;
            this.physical_PIF = physical_PIF;
            this.logical_PIF = logical_PIF;
            this.requires_reboot = requires_reboot;
            this.configuration_mode = configuration_mode;
        }

        /// <summary>
        /// Creates a new Network_sriov from a Proxy_Network_sriov.
        /// </summary>
        /// <param name="proxy"></param>
        public Network_sriov(Proxy_Network_sriov proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Network_sriov.
        /// </summary>
        public override void UpdateFrom(Network_sriov update)
        {
            uuid = update.uuid;
            physical_PIF = update.physical_PIF;
            logical_PIF = update.logical_PIF;
            requires_reboot = update.requires_reboot;
            configuration_mode = update.configuration_mode;
        }

        internal void UpdateFromProxy(Proxy_Network_sriov proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            physical_PIF = proxy.physical_PIF == null ? null : XenRef<PIF>.Create(proxy.physical_PIF);
            logical_PIF = proxy.logical_PIF == null ? null : XenRef<PIF>.Create(proxy.logical_PIF);
            requires_reboot = (bool)proxy.requires_reboot;
            configuration_mode = proxy.configuration_mode == null ? (sriov_configuration_mode) 0 : (sriov_configuration_mode)Helper.EnumParseDefault(typeof(sriov_configuration_mode), (string)proxy.configuration_mode);
        }

        public Proxy_Network_sriov ToProxy()
        {
            Proxy_Network_sriov result_ = new Proxy_Network_sriov();
            result_.uuid = uuid ?? "";
            result_.physical_PIF = physical_PIF ?? "";
            result_.logical_PIF = logical_PIF ?? "";
            result_.requires_reboot = requires_reboot;
            result_.configuration_mode = sriov_configuration_mode_helper.ToString(configuration_mode);
            return result_;
        }

        /// <summary>
        /// Creates a new Network_sriov from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Network_sriov(Hashtable table) : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Network_sriov
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("physical_PIF"))
                physical_PIF = Marshalling.ParseRef<PIF>(table, "physical_PIF");
            if (table.ContainsKey("logical_PIF"))
                logical_PIF = Marshalling.ParseRef<PIF>(table, "logical_PIF");
            if (table.ContainsKey("requires_reboot"))
                requires_reboot = Marshalling.ParseBool(table, "requires_reboot");
            if (table.ContainsKey("configuration_mode"))
                configuration_mode = (sriov_configuration_mode)Helper.EnumParseDefault(typeof(sriov_configuration_mode), Marshalling.ParseString(table, "configuration_mode"));
        }

        public bool DeepEquals(Network_sriov other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._physical_PIF, other._physical_PIF) &&
                Helper.AreEqual2(this._logical_PIF, other._logical_PIF) &&
                Helper.AreEqual2(this._requires_reboot, other._requires_reboot) &&
                Helper.AreEqual2(this._configuration_mode, other._configuration_mode);
        }

        internal static List<Network_sriov> ProxyArrayToObjectList(Proxy_Network_sriov[] input)
        {
            var result = new List<Network_sriov>();
            foreach (var item in input)
                result.Add(new Network_sriov(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, Network_sriov server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
              throw new InvalidOperationException("This type has no read/write properties");
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given network_sriov.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        public static Network_sriov get_record(Session session, string _network_sriov)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.network_sriov_get_record(session.opaque_ref, _network_sriov);
            else
                return new Network_sriov(session.proxy.network_sriov_get_record(session.opaque_ref, _network_sriov ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the network_sriov instance with the specified UUID.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<Network_sriov> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.network_sriov_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<Network_sriov>.Create(session.proxy.network_sriov_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given network_sriov.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        public static string get_uuid(Session session, string _network_sriov)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.network_sriov_get_uuid(session.opaque_ref, _network_sriov);
            else
                return session.proxy.network_sriov_get_uuid(session.opaque_ref, _network_sriov ?? "").parse();
        }

        /// <summary>
        /// Get the physical_PIF field of the given network_sriov.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        public static XenRef<PIF> get_physical_PIF(Session session, string _network_sriov)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.network_sriov_get_physical_pif(session.opaque_ref, _network_sriov);
            else
                return XenRef<PIF>.Create(session.proxy.network_sriov_get_physical_pif(session.opaque_ref, _network_sriov ?? "").parse());
        }

        /// <summary>
        /// Get the logical_PIF field of the given network_sriov.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        public static XenRef<PIF> get_logical_PIF(Session session, string _network_sriov)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.network_sriov_get_logical_pif(session.opaque_ref, _network_sriov);
            else
                return XenRef<PIF>.Create(session.proxy.network_sriov_get_logical_pif(session.opaque_ref, _network_sriov ?? "").parse());
        }

        /// <summary>
        /// Get the requires_reboot field of the given network_sriov.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        public static bool get_requires_reboot(Session session, string _network_sriov)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.network_sriov_get_requires_reboot(session.opaque_ref, _network_sriov);
            else
                return (bool)session.proxy.network_sriov_get_requires_reboot(session.opaque_ref, _network_sriov ?? "").parse();
        }

        /// <summary>
        /// Get the configuration_mode field of the given network_sriov.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        public static sriov_configuration_mode get_configuration_mode(Session session, string _network_sriov)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.network_sriov_get_configuration_mode(session.opaque_ref, _network_sriov);
            else
                return (sriov_configuration_mode)Helper.EnumParseDefault(typeof(sriov_configuration_mode), (string)session.proxy.network_sriov_get_configuration_mode(session.opaque_ref, _network_sriov ?? "").parse());
        }

        /// <summary>
        /// Enable SR-IOV on the specific PIF. It will create a network-sriov based on the specific PIF and automatically create a logical PIF to connect the specific network.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">PIF on which to enable SR-IOV</param>
        /// <param name="_network">Network to connect SR-IOV virtual functions with VM VIFs</param>
        public static XenRef<Network_sriov> create(Session session, string _pif, string _network)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.network_sriov_create(session.opaque_ref, _pif, _network);
            else
                return XenRef<Network_sriov>.Create(session.proxy.network_sriov_create(session.opaque_ref, _pif ?? "", _network ?? "").parse());
        }

        /// <summary>
        /// Enable SR-IOV on the specific PIF. It will create a network-sriov based on the specific PIF and automatically create a logical PIF to connect the specific network.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">PIF on which to enable SR-IOV</param>
        /// <param name="_network">Network to connect SR-IOV virtual functions with VM VIFs</param>
        public static XenRef<Task> async_create(Session session, string _pif, string _network)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_network_sriov_create(session.opaque_ref, _pif, _network);
          else
              return XenRef<Task>.Create(session.proxy.async_network_sriov_create(session.opaque_ref, _pif ?? "", _network ?? "").parse());
        }

        /// <summary>
        /// Disable SR-IOV on the specific PIF. It will destroy the network-sriov and the logical PIF accordingly.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        public static void destroy(Session session, string _network_sriov)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.network_sriov_destroy(session.opaque_ref, _network_sriov);
            else
                session.proxy.network_sriov_destroy(session.opaque_ref, _network_sriov ?? "").parse();
        }

        /// <summary>
        /// Disable SR-IOV on the specific PIF. It will destroy the network-sriov and the logical PIF accordingly.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        public static XenRef<Task> async_destroy(Session session, string _network_sriov)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_network_sriov_destroy(session.opaque_ref, _network_sriov);
          else
              return XenRef<Task>.Create(session.proxy.async_network_sriov_destroy(session.opaque_ref, _network_sriov ?? "").parse());
        }

        /// <summary>
        /// Get the number of free SR-IOV VFs on the associated PIF
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        public static long get_remaining_capacity(Session session, string _network_sriov)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.network_sriov_get_remaining_capacity(session.opaque_ref, _network_sriov);
            else
                return long.Parse(session.proxy.network_sriov_get_remaining_capacity(session.opaque_ref, _network_sriov ?? "").parse());
        }

        /// <summary>
        /// Get the number of free SR-IOV VFs on the associated PIF
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        public static XenRef<Task> async_get_remaining_capacity(Session session, string _network_sriov)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_network_sriov_get_remaining_capacity(session.opaque_ref, _network_sriov);
          else
              return XenRef<Task>.Create(session.proxy.async_network_sriov_get_remaining_capacity(session.opaque_ref, _network_sriov ?? "").parse());
        }

        /// <summary>
        /// Return a list of all the network_sriovs known to the system.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Network_sriov>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.network_sriov_get_all(session.opaque_ref);
            else
                return XenRef<Network_sriov>.Create(session.proxy.network_sriov_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the network_sriov Records at once, in a single XML RPC call
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Network_sriov>, Network_sriov> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.network_sriov_get_all_records(session.opaque_ref);
            else
                return XenRef<Network_sriov>.Create<Proxy_Network_sriov>(session.proxy.network_sriov_get_all_records(session.opaque_ref).parse());
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
        /// The PIF that has SR-IOV enabled
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PIF>))]
        public virtual XenRef<PIF> physical_PIF
        {
            get { return _physical_PIF; }
            set
            {
                if (!Helper.AreEqual(value, _physical_PIF))
                {
                    _physical_PIF = value;
                    Changed = true;
                    NotifyPropertyChanged("physical_PIF");
                }
            }
        }
        private XenRef<PIF> _physical_PIF = new XenRef<PIF>(Helper.NullOpaqueRef);

        /// <summary>
        /// The logical PIF to connect to the SR-IOV network after enable SR-IOV on the physical PIF
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PIF>))]
        public virtual XenRef<PIF> logical_PIF
        {
            get { return _logical_PIF; }
            set
            {
                if (!Helper.AreEqual(value, _logical_PIF))
                {
                    _logical_PIF = value;
                    Changed = true;
                    NotifyPropertyChanged("logical_PIF");
                }
            }
        }
        private XenRef<PIF> _logical_PIF = new XenRef<PIF>(Helper.NullOpaqueRef);

        /// <summary>
        /// Indicates whether the host need to be rebooted before SR-IOV is enabled on the physical PIF
        /// </summary>
        public virtual bool requires_reboot
        {
            get { return _requires_reboot; }
            set
            {
                if (!Helper.AreEqual(value, _requires_reboot))
                {
                    _requires_reboot = value;
                    Changed = true;
                    NotifyPropertyChanged("requires_reboot");
                }
            }
        }
        private bool _requires_reboot = false;

        /// <summary>
        /// The mode for configure network sriov
        /// </summary>
        [JsonConverter(typeof(sriov_configuration_modeConverter))]
        public virtual sriov_configuration_mode configuration_mode
        {
            get { return _configuration_mode; }
            set
            {
                if (!Helper.AreEqual(value, _configuration_mode))
                {
                    _configuration_mode = value;
                    Changed = true;
                    NotifyPropertyChanged("configuration_mode");
                }
            }
        }
        private sriov_configuration_mode _configuration_mode = sriov_configuration_mode.unknown;
    }
}
