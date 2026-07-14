/*
 * Copyright (c) Cloud Software Group, Inc.
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
using System.Linq;
using Newtonsoft.Json;


namespace XenAPI
{
    /// <summary>
    /// A tunnel for network traffic
    /// First published in XenServer 5.6 FP1.
    /// </summary>
    public partial class Tunnel : XenObject<Tunnel>
    {
        #region Constructors

        public Tunnel()
        {
        }

        public Tunnel(string uuid,
            XenRef<PIF> access_PIF,
            XenRef<PIF> transport_PIF,
            Dictionary<string, string> status,
            Dictionary<string, string> other_config,
            tunnel_protocol protocol)
        {
            this.uuid = uuid;
            this.access_PIF = access_PIF;
            this.transport_PIF = transport_PIF;
            this.status = status;
            this.other_config = other_config;
            this.protocol = protocol;
        }

        /// <summary>
        /// Creates a new Tunnel from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Tunnel(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Tunnel.
        /// </summary>
        public override void UpdateFrom(Tunnel record)
        {
            uuid = record.uuid;
            access_PIF = record.access_PIF;
            transport_PIF = record.transport_PIF;
            status = record.status;
            other_config = record.other_config;
            protocol = record.protocol;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Tunnel
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("access_PIF"))
                access_PIF = Marshalling.ParseRef<PIF>(table, "access_PIF");
            if (table.ContainsKey("transport_PIF"))
                transport_PIF = Marshalling.ParseRef<PIF>(table, "transport_PIF");
            if (table.ContainsKey("status"))
                status = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "status"));
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("protocol"))
                protocol = (tunnel_protocol)Helper.EnumParseDefault(typeof(tunnel_protocol), Marshalling.ParseString(table, "protocol"));
        }

        public bool DeepEquals(Tunnel other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_access_PIF, other._access_PIF) &&
                Helper.AreEqual2(_transport_PIF, other._transport_PIF) &&
                Helper.AreEqual2(_status, other._status) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_protocol, other._protocol);
        }


        /// <summary>
        /// Get a record containing the current state of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Tunnel get_record(Session session, string _tunnel)
        {
            return session.JsonRpcClient.tunnel_get_record(session.opaque_ref, _tunnel);
        }

        /// <summary>
        /// Get a reference to the tunnel instance with the specified UUID.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Tunnel> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.tunnel_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _tunnel)
        {
            return session.JsonRpcClient.tunnel_get_uuid(session.opaque_ref, _tunnel);
        }

        /// <summary>
        /// Get the access_PIF field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PIF> get_access_PIF(Session session, string _tunnel)
        {
            return session.JsonRpcClient.tunnel_get_access_pif(session.opaque_ref, _tunnel);
        }

        /// <summary>
        /// Get the transport_PIF field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PIF> get_transport_PIF(Session session, string _tunnel)
        {
            return session.JsonRpcClient.tunnel_get_transport_pif(session.opaque_ref, _tunnel);
        }

        /// <summary>
        /// Get the status field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_status(Session session, string _tunnel)
        {
            return session.JsonRpcClient.tunnel_get_status(session.opaque_ref, _tunnel);
        }

        /// <summary>
        /// Get the other_config field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _tunnel)
        {
            return session.JsonRpcClient.tunnel_get_other_config(session.opaque_ref, _tunnel);
        }

        /// <summary>
        /// Get the protocol field of the given tunnel.
        /// First published in 1.250.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static tunnel_protocol get_protocol(Session session, string _tunnel)
        {
            return session.JsonRpcClient.tunnel_get_protocol(session.opaque_ref, _tunnel);
        }

        /// <summary>
        /// Set the status field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <param name="_status">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_status(Session session, string _tunnel, Dictionary<string, string> _status)
        {
            session.JsonRpcClient.tunnel_set_status(session.opaque_ref, _tunnel, _status);
        }

        /// <summary>
        /// Add the given key-value pair to the status field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_status(Session session, string _tunnel, string _key, string _value)
        {
            session.JsonRpcClient.tunnel_add_to_status(session.opaque_ref, _tunnel, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the status field of the given tunnel.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_status(Session session, string _tunnel, string _key)
        {
            session.JsonRpcClient.tunnel_remove_from_status(session.opaque_ref, _tunnel, _key);
        }

        /// <summary>
        /// Set the other_config field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_other_config(Session session, string _tunnel, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.tunnel_set_other_config(session.opaque_ref, _tunnel, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given tunnel.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_other_config(Session session, string _tunnel, string _key, string _value)
        {
            session.JsonRpcClient.tunnel_add_to_other_config(session.opaque_ref, _tunnel, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given tunnel.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_other_config(Session session, string _tunnel, string _key)
        {
            session.JsonRpcClient.tunnel_remove_from_other_config(session.opaque_ref, _tunnel, _key);
        }

        /// <summary>
        /// Set the protocol field of the given tunnel.
        /// First published in 1.250.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <param name="_protocol">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_protocol(Session session, string _tunnel, tunnel_protocol _protocol)
        {
            session.JsonRpcClient.tunnel_set_protocol(session.opaque_ref, _tunnel, _protocol);
        }

        /// <summary>
        /// Create a tunnel
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_transport_pif">PIF which receives the tagged traffic First published in XenServer 7.0.</param>
        /// <param name="_network">Network to receive the tunnelled traffic First published in XenServer 7.0.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Tunnel> create(Session session, string _transport_pif, string _network)
        {
            return session.JsonRpcClient.tunnel_create(session.opaque_ref, _transport_pif, _network);
        }

        /// <summary>
        /// Create a tunnel
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_transport_pif">PIF which receives the tagged traffic First published in XenServer 7.0.</param>
        /// <param name="_network">Network to receive the tunnelled traffic First published in XenServer 7.0.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, string _transport_pif, string _network)
        {
          return session.JsonRpcClient.async_tunnel_create(session.opaque_ref, _transport_pif, _network);
        }

        /// <summary>
        /// Create a tunnel
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_transport_pif">PIF which receives the tagged traffic First published in XenServer 7.0.</param>
        /// <param name="_network">Network to receive the tunnelled traffic First published in XenServer 7.0.</param>
        /// <param name="_protocol">Protocol used for the tunnel (GRE or VxLAN) First published in 1.250.0.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Tunnel> create(Session session, string _transport_pif, string _network, tunnel_protocol _protocol)
        {
            return session.JsonRpcClient.tunnel_create(session.opaque_ref, _transport_pif, _network, _protocol);
        }

        /// <summary>
        /// Create a tunnel
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_transport_pif">PIF which receives the tagged traffic First published in XenServer 7.0.</param>
        /// <param name="_network">Network to receive the tunnelled traffic First published in XenServer 7.0.</param>
        /// <param name="_protocol">Protocol used for the tunnel (GRE or VxLAN) First published in 1.250.0.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, string _transport_pif, string _network, tunnel_protocol _protocol)
        {
          return session.JsonRpcClient.async_tunnel_create(session.opaque_ref, _transport_pif, _network, _protocol);
        }

        /// <summary>
        /// Destroy a tunnel
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _tunnel)
        {
            session.JsonRpcClient.tunnel_destroy(session.opaque_ref, _tunnel);
        }

        /// <summary>
        /// Destroy a tunnel
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tunnel">The opaque_ref of the given tunnel</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _tunnel)
        {
          return session.JsonRpcClient.async_tunnel_destroy(session.opaque_ref, _tunnel);
        }

        /// <summary>
        /// Return a list of all the tunnels known to the system.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Tunnel>> get_all(Session session)
        {
            return session.JsonRpcClient.tunnel_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of tunnel references to tunnel records for all tunnels known to the system.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Tunnel>, Tunnel> get_all_records(Session session)
        {
            return session.JsonRpcClient.tunnel_get_all_records(session.opaque_ref);
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
                    NotifyPropertyChanged("uuid");
                }
            }
        }
        private string _uuid = "";

        /// <summary>
        /// The interface through which the tunnel is accessed
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PIF>))]
        public virtual XenRef<PIF> access_PIF
        {
            get { return _access_PIF; }
            set
            {
                if (!Helper.AreEqual(value, _access_PIF))
                {
                    _access_PIF = value;
                    NotifyPropertyChanged("access_PIF");
                }
            }
        }
        private XenRef<PIF> _access_PIF = new XenRef<PIF>(Helper.NullOpaqueRef);

        /// <summary>
        /// The interface used by the tunnel
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PIF>))]
        public virtual XenRef<PIF> transport_PIF
        {
            get { return _transport_PIF; }
            set
            {
                if (!Helper.AreEqual(value, _transport_PIF))
                {
                    _transport_PIF = value;
                    NotifyPropertyChanged("transport_PIF");
                }
            }
        }
        private XenRef<PIF> _transport_PIF = new XenRef<PIF>(Helper.NullOpaqueRef);

        /// <summary>
        /// Status information about the tunnel
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> status
        {
            get { return _status; }
            set
            {
                if (!Helper.AreEqual(value, _status))
                {
                    _status = value;
                    NotifyPropertyChanged("status");
                }
            }
        }
        private Dictionary<string, string> _status = new Dictionary<string, string>() {{"active", "false"}};

        /// <summary>
        /// Additional configuration
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
                    NotifyPropertyChanged("other_config");
                }
            }
        }
        private Dictionary<string, string> _other_config = new Dictionary<string, string>() {};

        /// <summary>
        /// The protocol used for tunneling (either GRE or VxLAN)
        /// First published in 1.250.0.
        /// </summary>
        [JsonConverter(typeof(tunnel_protocolConverter))]
        public virtual tunnel_protocol protocol
        {
            get { return _protocol; }
            set
            {
                if (!Helper.AreEqual(value, _protocol))
                {
                    _protocol = value;
                    NotifyPropertyChanged("protocol");
                }
            }
        }
        private tunnel_protocol _protocol = tunnel_protocol.gre;
    }
}
