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
    /// Describes the SDN controller that is to connect with the pool
    /// First published in XenServer 7.2.
    /// </summary>
    public partial class SDN_controller : XenObject<SDN_controller>
    {
        #region Constructors

        public SDN_controller()
        {
        }

        public SDN_controller(string uuid,
            sdn_controller_protocol protocol,
            string address,
            long port)
        {
            this.uuid = uuid;
            this.protocol = protocol;
            this.address = address;
            this.port = port;
        }

        /// <summary>
        /// Creates a new SDN_controller from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public SDN_controller(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given SDN_controller.
        /// </summary>
        public override void UpdateFrom(SDN_controller record)
        {
            uuid = record.uuid;
            protocol = record.protocol;
            address = record.address;
            port = record.port;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this SDN_controller
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("protocol"))
                protocol = (sdn_controller_protocol)Helper.EnumParseDefault(typeof(sdn_controller_protocol), Marshalling.ParseString(table, "protocol"));
            if (table.ContainsKey("address"))
                address = Marshalling.ParseString(table, "address");
            if (table.ContainsKey("port"))
                port = Marshalling.ParseLong(table, "port");
        }

        public bool DeepEquals(SDN_controller other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_protocol, other._protocol) &&
                Helper.AreEqual2(_address, other._address) &&
                Helper.AreEqual2(_port, other._port);
        }


        /// <summary>
        /// Get a record containing the current state of the given SDN_controller.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sdn_controller">The opaque_ref of the given sdn_controller</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static SDN_controller get_record(Session session, string _sdn_controller)
        {
            return session.JsonRpcClient.sdn_controller_get_record(session.opaque_ref, _sdn_controller);
        }

        /// <summary>
        /// Get a reference to the SDN_controller instance with the specified UUID.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<SDN_controller> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.sdn_controller_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given SDN_controller.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sdn_controller">The opaque_ref of the given sdn_controller</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _sdn_controller)
        {
            return session.JsonRpcClient.sdn_controller_get_uuid(session.opaque_ref, _sdn_controller);
        }

        /// <summary>
        /// Get the protocol field of the given SDN_controller.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sdn_controller">The opaque_ref of the given sdn_controller</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static sdn_controller_protocol get_protocol(Session session, string _sdn_controller)
        {
            return session.JsonRpcClient.sdn_controller_get_protocol(session.opaque_ref, _sdn_controller);
        }

        /// <summary>
        /// Get the address field of the given SDN_controller.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sdn_controller">The opaque_ref of the given sdn_controller</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_address(Session session, string _sdn_controller)
        {
            return session.JsonRpcClient.sdn_controller_get_address(session.opaque_ref, _sdn_controller);
        }

        /// <summary>
        /// Get the port field of the given SDN_controller.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sdn_controller">The opaque_ref of the given sdn_controller</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_port(Session session, string _sdn_controller)
        {
            return session.JsonRpcClient.sdn_controller_get_port(session.opaque_ref, _sdn_controller);
        }

        /// <summary>
        /// Introduce an SDN controller to the pool.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_protocol">Protocol to connect with the controller.</param>
        /// <param name="_address">IP address of the controller.</param>
        /// <param name="_port">TCP port of the controller.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<SDN_controller> introduce(Session session, sdn_controller_protocol _protocol, string _address, long _port)
        {
            return session.JsonRpcClient.sdn_controller_introduce(session.opaque_ref, _protocol, _address, _port);
        }

        /// <summary>
        /// Introduce an SDN controller to the pool.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_protocol">Protocol to connect with the controller.</param>
        /// <param name="_address">IP address of the controller.</param>
        /// <param name="_port">TCP port of the controller.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_introduce(Session session, sdn_controller_protocol _protocol, string _address, long _port)
        {
          return session.JsonRpcClient.async_sdn_controller_introduce(session.opaque_ref, _protocol, _address, _port);
        }

        /// <summary>
        /// Remove the OVS manager of the pool and destroy the db record.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sdn_controller">The opaque_ref of the given sdn_controller</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void forget(Session session, string _sdn_controller)
        {
            session.JsonRpcClient.sdn_controller_forget(session.opaque_ref, _sdn_controller);
        }

        /// <summary>
        /// Remove the OVS manager of the pool and destroy the db record.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sdn_controller">The opaque_ref of the given sdn_controller</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_forget(Session session, string _sdn_controller)
        {
          return session.JsonRpcClient.async_sdn_controller_forget(session.opaque_ref, _sdn_controller);
        }

        /// <summary>
        /// Return a list of all the SDN_controllers known to the system.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<SDN_controller>> get_all(Session session)
        {
            return session.JsonRpcClient.sdn_controller_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of SDN_controller references to SDN_controller records for all SDN_controllers known to the system.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<SDN_controller>, SDN_controller> get_all_records(Session session)
        {
            return session.JsonRpcClient.sdn_controller_get_all_records(session.opaque_ref);
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
        /// Protocol to connect with SDN controller
        /// </summary>
        [JsonConverter(typeof(sdn_controller_protocolConverter))]
        public virtual sdn_controller_protocol protocol
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
        private sdn_controller_protocol _protocol = sdn_controller_protocol.ssl;

        /// <summary>
        /// IP address of the controller
        /// </summary>
        public virtual string address
        {
            get { return _address; }
            set
            {
                if (!Helper.AreEqual(value, _address))
                {
                    _address = value;
                    NotifyPropertyChanged("address");
                }
            }
        }
        private string _address = "";

        /// <summary>
        /// TCP port of the controller
        /// </summary>
        public virtual long port
        {
            get { return _port; }
            set
            {
                if (!Helper.AreEqual(value, _port))
                {
                    _port = value;
                    NotifyPropertyChanged("port");
                }
            }
        }
        private long _port = 0;
    }
}
