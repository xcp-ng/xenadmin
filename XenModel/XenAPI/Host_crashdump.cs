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
    /// Represents a host crash dump
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class Host_crashdump : XenObject<Host_crashdump>
    {
        #region Constructors

        public Host_crashdump()
        {
        }

        public Host_crashdump(string uuid,
            XenRef<Host> host,
            DateTime timestamp,
            long size,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.host = host;
            this.timestamp = timestamp;
            this.size = size;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Host_crashdump from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Host_crashdump(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Host_crashdump.
        /// </summary>
        public override void UpdateFrom(Host_crashdump record)
        {
            uuid = record.uuid;
            host = record.host;
            timestamp = record.timestamp;
            size = record.size;
            other_config = record.other_config;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Host_crashdump
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("host"))
                host = Marshalling.ParseRef<Host>(table, "host");
            if (table.ContainsKey("timestamp"))
                timestamp = Marshalling.ParseDateTime(table, "timestamp");
            if (table.ContainsKey("size"))
                size = Marshalling.ParseLong(table, "size");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Host_crashdump other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_host, other._host) &&
                Helper.AreEqual2(_timestamp, other._timestamp) &&
                Helper.AreEqual2(_size, other._size) &&
                Helper.AreEqual2(_other_config, other._other_config);
        }


        /// <summary>
        /// Get a record containing the current state of the given host_crashdump.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Host_crashdump get_record(Session session, string _host_crashdump)
        {
            return session.JsonRpcClient.host_crashdump_get_record(session.opaque_ref, _host_crashdump);
        }

        /// <summary>
        /// Get a reference to the host_crashdump instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host_crashdump> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.host_crashdump_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given host_crashdump.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _host_crashdump)
        {
            return session.JsonRpcClient.host_crashdump_get_uuid(session.opaque_ref, _host_crashdump);
        }

        /// <summary>
        /// Get the host field of the given host_crashdump.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host> get_host(Session session, string _host_crashdump)
        {
            return session.JsonRpcClient.host_crashdump_get_host(session.opaque_ref, _host_crashdump);
        }

        /// <summary>
        /// Get the timestamp field of the given host_crashdump.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_timestamp(Session session, string _host_crashdump)
        {
            return session.JsonRpcClient.host_crashdump_get_timestamp(session.opaque_ref, _host_crashdump);
        }

        /// <summary>
        /// Get the size field of the given host_crashdump.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_size(Session session, string _host_crashdump)
        {
            return session.JsonRpcClient.host_crashdump_get_size(session.opaque_ref, _host_crashdump);
        }

        /// <summary>
        /// Get the other_config field of the given host_crashdump.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _host_crashdump)
        {
            return session.JsonRpcClient.host_crashdump_get_other_config(session.opaque_ref, _host_crashdump);
        }

        /// <summary>
        /// Set the other_config field of the given host_crashdump.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_other_config(Session session, string _host_crashdump, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.host_crashdump_set_other_config(session.opaque_ref, _host_crashdump, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given host_crashdump.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_other_config(Session session, string _host_crashdump, string _key, string _value)
        {
            session.JsonRpcClient.host_crashdump_add_to_other_config(session.opaque_ref, _host_crashdump, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given host_crashdump.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_other_config(Session session, string _host_crashdump, string _key)
        {
            session.JsonRpcClient.host_crashdump_remove_from_other_config(session.opaque_ref, _host_crashdump, _key);
        }

        /// <summary>
        /// Destroy specified host crash dump, removing it from the disk.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _host_crashdump)
        {
            session.JsonRpcClient.host_crashdump_destroy(session.opaque_ref, _host_crashdump);
        }

        /// <summary>
        /// Destroy specified host crash dump, removing it from the disk.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _host_crashdump)
        {
          return session.JsonRpcClient.async_host_crashdump_destroy(session.opaque_ref, _host_crashdump);
        }

        /// <summary>
        /// Upload the specified host crash dump to a specified URL
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <param name="_url">The URL to upload to</param>
        /// <param name="_options">Extra configuration operations</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void upload(Session session, string _host_crashdump, string _url, Dictionary<string, string> _options)
        {
            session.JsonRpcClient.host_crashdump_upload(session.opaque_ref, _host_crashdump, _url, _options);
        }

        /// <summary>
        /// Upload the specified host crash dump to a specified URL
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <param name="_url">The URL to upload to</param>
        /// <param name="_options">Extra configuration operations</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_upload(Session session, string _host_crashdump, string _url, Dictionary<string, string> _options)
        {
          return session.JsonRpcClient.async_host_crashdump_upload(session.opaque_ref, _host_crashdump, _url, _options);
        }

        /// <summary>
        /// Return a list of all the host_crashdumps known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Host_crashdump>> get_all(Session session)
        {
            return session.JsonRpcClient.host_crashdump_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of host_crashdump references to host_crashdump records for all host_crashdumps known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Host_crashdump>, Host_crashdump> get_all_records(Session session)
        {
            return session.JsonRpcClient.host_crashdump_get_all_records(session.opaque_ref);
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
        /// Host the crashdump relates to
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
                    NotifyPropertyChanged("host");
                }
            }
        }
        private XenRef<Host> _host = new XenRef<Host>(Helper.NullOpaqueRef);

        /// <summary>
        /// Time the crash happened
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime timestamp
        {
            get { return _timestamp; }
            set
            {
                if (!Helper.AreEqual(value, _timestamp))
                {
                    _timestamp = value;
                    NotifyPropertyChanged("timestamp");
                }
            }
        }
        private DateTime _timestamp;

        /// <summary>
        /// Size of the crashdump
        /// </summary>
        public virtual long size
        {
            get { return _size; }
            set
            {
                if (!Helper.AreEqual(value, _size))
                {
                    _size = value;
                    NotifyPropertyChanged("size");
                }
            }
        }
        private long _size;

        /// <summary>
        /// additional configuration
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
                    NotifyPropertyChanged("other_config");
                }
            }
        }
        private Dictionary<string, string> _other_config = new Dictionary<string, string>() {};
    }
}
