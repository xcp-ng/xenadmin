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
    /// Represents a host crash dump
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class Host_crashdump : XenObject<Host_crashdump>
    {
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
        /// Creates a new Host_crashdump from a Proxy_Host_crashdump.
        /// </summary>
        /// <param name="proxy"></param>
        public Host_crashdump(Proxy_Host_crashdump proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Host_crashdump.
        /// </summary>
        public override void UpdateFrom(Host_crashdump update)
        {
            uuid = update.uuid;
            host = update.host;
            timestamp = update.timestamp;
            size = update.size;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_Host_crashdump proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            host = proxy.host == null ? null : XenRef<Host>.Create(proxy.host);
            timestamp = proxy.timestamp;
            size = proxy.size == null ? 0 : long.Parse(proxy.size);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_Host_crashdump ToProxy()
        {
            Proxy_Host_crashdump result_ = new Proxy_Host_crashdump();
            result_.uuid = uuid ?? "";
            result_.host = host ?? "";
            result_.timestamp = timestamp;
            result_.size = size.ToString();
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new Host_crashdump from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Host_crashdump(Hashtable table) : this()
        {
            UpdateFrom(table);
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
                other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Host_crashdump other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._host, other._host) &&
                Helper.AreEqual2(this._timestamp, other._timestamp) &&
                Helper.AreEqual2(this._size, other._size) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        internal static List<Host_crashdump> ProxyArrayToObjectList(Proxy_Host_crashdump[] input)
        {
            var result = new List<Host_crashdump>();
            foreach (var item in input)
                result.Add(new Host_crashdump(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, Host_crashdump server)
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
                    Host_crashdump.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given host_crashdump.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        public static Host_crashdump get_record(Session session, string _host_crashdump)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_crashdump_get_record(session.opaque_ref, _host_crashdump);
            else
                return new Host_crashdump(session.proxy.host_crashdump_get_record(session.opaque_ref, _host_crashdump ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the host_crashdump instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<Host_crashdump> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_crashdump_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<Host_crashdump>.Create(session.proxy.host_crashdump_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given host_crashdump.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        public static string get_uuid(Session session, string _host_crashdump)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_crashdump_get_uuid(session.opaque_ref, _host_crashdump);
            else
                return session.proxy.host_crashdump_get_uuid(session.opaque_ref, _host_crashdump ?? "").parse();
        }

        /// <summary>
        /// Get the host field of the given host_crashdump.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        public static XenRef<Host> get_host(Session session, string _host_crashdump)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_crashdump_get_host(session.opaque_ref, _host_crashdump);
            else
                return XenRef<Host>.Create(session.proxy.host_crashdump_get_host(session.opaque_ref, _host_crashdump ?? "").parse());
        }

        /// <summary>
        /// Get the timestamp field of the given host_crashdump.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        public static DateTime get_timestamp(Session session, string _host_crashdump)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_crashdump_get_timestamp(session.opaque_ref, _host_crashdump);
            else
                return session.proxy.host_crashdump_get_timestamp(session.opaque_ref, _host_crashdump ?? "").parse();
        }

        /// <summary>
        /// Get the size field of the given host_crashdump.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        public static long get_size(Session session, string _host_crashdump)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_crashdump_get_size(session.opaque_ref, _host_crashdump);
            else
                return long.Parse(session.proxy.host_crashdump_get_size(session.opaque_ref, _host_crashdump ?? "").parse());
        }

        /// <summary>
        /// Get the other_config field of the given host_crashdump.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        public static Dictionary<string, string> get_other_config(Session session, string _host_crashdump)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_crashdump_get_other_config(session.opaque_ref, _host_crashdump);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.host_crashdump_get_other_config(session.opaque_ref, _host_crashdump ?? "").parse());
        }

        /// <summary>
        /// Set the other_config field of the given host_crashdump.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _host_crashdump, Dictionary<string, string> _other_config)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_crashdump_set_other_config(session.opaque_ref, _host_crashdump, _other_config);
            else
                session.proxy.host_crashdump_set_other_config(session.opaque_ref, _host_crashdump ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given host_crashdump.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _host_crashdump, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_crashdump_add_to_other_config(session.opaque_ref, _host_crashdump, _key, _value);
            else
                session.proxy.host_crashdump_add_to_other_config(session.opaque_ref, _host_crashdump ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given host_crashdump.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _host_crashdump, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_crashdump_remove_from_other_config(session.opaque_ref, _host_crashdump, _key);
            else
                session.proxy.host_crashdump_remove_from_other_config(session.opaque_ref, _host_crashdump ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// Destroy specified host crash dump, removing it from the disk.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        public static void destroy(Session session, string _host_crashdump)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_crashdump_destroy(session.opaque_ref, _host_crashdump);
            else
                session.proxy.host_crashdump_destroy(session.opaque_ref, _host_crashdump ?? "").parse();
        }

        /// <summary>
        /// Destroy specified host crash dump, removing it from the disk.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        public static XenRef<Task> async_destroy(Session session, string _host_crashdump)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_crashdump_destroy(session.opaque_ref, _host_crashdump);
          else
              return XenRef<Task>.Create(session.proxy.async_host_crashdump_destroy(session.opaque_ref, _host_crashdump ?? "").parse());
        }

        /// <summary>
        /// Upload the specified host crash dump to a specified URL
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <param name="_url">The URL to upload to</param>
        /// <param name="_options">Extra configuration operations</param>
        public static void upload(Session session, string _host_crashdump, string _url, Dictionary<string, string> _options)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_crashdump_upload(session.opaque_ref, _host_crashdump, _url, _options);
            else
                session.proxy.host_crashdump_upload(session.opaque_ref, _host_crashdump ?? "", _url ?? "", Maps.convert_to_proxy_string_string(_options)).parse();
        }

        /// <summary>
        /// Upload the specified host crash dump to a specified URL
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_crashdump">The opaque_ref of the given host_crashdump</param>
        /// <param name="_url">The URL to upload to</param>
        /// <param name="_options">Extra configuration operations</param>
        public static XenRef<Task> async_upload(Session session, string _host_crashdump, string _url, Dictionary<string, string> _options)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_crashdump_upload(session.opaque_ref, _host_crashdump, _url, _options);
          else
              return XenRef<Task>.Create(session.proxy.async_host_crashdump_upload(session.opaque_ref, _host_crashdump ?? "", _url ?? "", Maps.convert_to_proxy_string_string(_options)).parse());
        }

        /// <summary>
        /// Return a list of all the host_crashdumps known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Host_crashdump>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_crashdump_get_all(session.opaque_ref);
            else
                return XenRef<Host_crashdump>.Create(session.proxy.host_crashdump_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the host_crashdump Records at once, in a single XML RPC call
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Host_crashdump>, Host_crashdump> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_crashdump_get_all_records(session.opaque_ref);
            else
                return XenRef<Host_crashdump>.Create<Proxy_Host_crashdump>(session.proxy.host_crashdump_get_all_records(session.opaque_ref).parse());
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
                    NotifyPropertyChanged("other_config");
                }
            }
        }
        private Dictionary<string, string> _other_config = new Dictionary<string, string>() {};
    }
}
