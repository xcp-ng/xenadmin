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
    /// individual machine serving provisioning (block) data
    /// First published in XenServer 7.1.
    /// </summary>
    public partial class PVS_server : XenObject<PVS_server>
    {
        #region Constructors

        public PVS_server()
        {
        }

        public PVS_server(string uuid,
            string[] addresses,
            long first_port,
            long last_port,
            XenRef<PVS_site> site)
        {
            this.uuid = uuid;
            this.addresses = addresses;
            this.first_port = first_port;
            this.last_port = last_port;
            this.site = site;
        }

        /// <summary>
        /// Creates a new PVS_server from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public PVS_server(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given PVS_server.
        /// </summary>
        public override void UpdateFrom(PVS_server record)
        {
            uuid = record.uuid;
            addresses = record.addresses;
            first_port = record.first_port;
            last_port = record.last_port;
            site = record.site;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this PVS_server
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("addresses"))
                addresses = Marshalling.ParseStringArray(table, "addresses");
            if (table.ContainsKey("first_port"))
                first_port = Marshalling.ParseLong(table, "first_port");
            if (table.ContainsKey("last_port"))
                last_port = Marshalling.ParseLong(table, "last_port");
            if (table.ContainsKey("site"))
                site = Marshalling.ParseRef<PVS_site>(table, "site");
        }

        public bool DeepEquals(PVS_server other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_addresses, other._addresses) &&
                Helper.AreEqual2(_first_port, other._first_port) &&
                Helper.AreEqual2(_last_port, other._last_port) &&
                Helper.AreEqual2(_site, other._site);
        }


        /// <summary>
        /// Get a record containing the current state of the given PVS_server.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_server">The opaque_ref of the given pvs_server</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static PVS_server get_record(Session session, string _pvs_server)
        {
            return session.JsonRpcClient.pvs_server_get_record(session.opaque_ref, _pvs_server);
        }

        /// <summary>
        /// Get a reference to the PVS_server instance with the specified UUID.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PVS_server> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.pvs_server_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given PVS_server.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_server">The opaque_ref of the given pvs_server</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _pvs_server)
        {
            return session.JsonRpcClient.pvs_server_get_uuid(session.opaque_ref, _pvs_server);
        }

        /// <summary>
        /// Get the addresses field of the given PVS_server.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_server">The opaque_ref of the given pvs_server</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_addresses(Session session, string _pvs_server)
        {
            return session.JsonRpcClient.pvs_server_get_addresses(session.opaque_ref, _pvs_server);
        }

        /// <summary>
        /// Get the first_port field of the given PVS_server.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_server">The opaque_ref of the given pvs_server</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_first_port(Session session, string _pvs_server)
        {
            return session.JsonRpcClient.pvs_server_get_first_port(session.opaque_ref, _pvs_server);
        }

        /// <summary>
        /// Get the last_port field of the given PVS_server.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_server">The opaque_ref of the given pvs_server</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_last_port(Session session, string _pvs_server)
        {
            return session.JsonRpcClient.pvs_server_get_last_port(session.opaque_ref, _pvs_server);
        }

        /// <summary>
        /// Get the site field of the given PVS_server.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_server">The opaque_ref of the given pvs_server</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PVS_site> get_site(Session session, string _pvs_server)
        {
            return session.JsonRpcClient.pvs_server_get_site(session.opaque_ref, _pvs_server);
        }

        /// <summary>
        /// introduce new PVS server
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_addresses">IPv4/IPv6 addresses of the server</param>
        /// <param name="_first_port">first UDP port accepted by this server</param>
        /// <param name="_last_port">last UDP port accepted by this server</param>
        /// <param name="_site">PVS site this server is a part of</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<PVS_server> introduce(Session session, string[] _addresses, long _first_port, long _last_port, string _site)
        {
            return session.JsonRpcClient.pvs_server_introduce(session.opaque_ref, _addresses, _first_port, _last_port, _site);
        }

        /// <summary>
        /// introduce new PVS server
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_addresses">IPv4/IPv6 addresses of the server</param>
        /// <param name="_first_port">first UDP port accepted by this server</param>
        /// <param name="_last_port">last UDP port accepted by this server</param>
        /// <param name="_site">PVS site this server is a part of</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_introduce(Session session, string[] _addresses, long _first_port, long _last_port, string _site)
        {
          return session.JsonRpcClient.async_pvs_server_introduce(session.opaque_ref, _addresses, _first_port, _last_port, _site);
        }

        /// <summary>
        /// forget a PVS server
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_server">The opaque_ref of the given pvs_server</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void forget(Session session, string _pvs_server)
        {
            session.JsonRpcClient.pvs_server_forget(session.opaque_ref, _pvs_server);
        }

        /// <summary>
        /// forget a PVS server
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_server">The opaque_ref of the given pvs_server</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_forget(Session session, string _pvs_server)
        {
          return session.JsonRpcClient.async_pvs_server_forget(session.opaque_ref, _pvs_server);
        }

        /// <summary>
        /// Return a list of all the PVS_servers known to the system.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PVS_server>> get_all(Session session)
        {
            return session.JsonRpcClient.pvs_server_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of PVS_server references to PVS_server records for all PVS_servers known to the system.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<PVS_server>, PVS_server> get_all_records(Session session)
        {
            return session.JsonRpcClient.pvs_server_get_all_records(session.opaque_ref);
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
        /// IPv4 addresses of this server
        /// </summary>
        public virtual string[] addresses
        {
            get { return _addresses; }
            set
            {
                if (!Helper.AreEqual(value, _addresses))
                {
                    _addresses = value;
                    NotifyPropertyChanged("addresses");
                }
            }
        }
        private string[] _addresses = {};

        /// <summary>
        /// First UDP port accepted by this server
        /// </summary>
        public virtual long first_port
        {
            get { return _first_port; }
            set
            {
                if (!Helper.AreEqual(value, _first_port))
                {
                    _first_port = value;
                    NotifyPropertyChanged("first_port");
                }
            }
        }
        private long _first_port = 0;

        /// <summary>
        /// Last UDP port accepted by this server
        /// </summary>
        public virtual long last_port
        {
            get { return _last_port; }
            set
            {
                if (!Helper.AreEqual(value, _last_port))
                {
                    _last_port = value;
                    NotifyPropertyChanged("last_port");
                }
            }
        }
        private long _last_port = 0;

        /// <summary>
        /// PVS site this server is part of
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PVS_site>))]
        public virtual XenRef<PVS_site> site
        {
            get { return _site; }
            set
            {
                if (!Helper.AreEqual(value, _site))
                {
                    _site = value;
                    NotifyPropertyChanged("site");
                }
            }
        }
        private XenRef<PVS_site> _site = new XenRef<PVS_site>("OpaqueRef:NULL");
    }
}
