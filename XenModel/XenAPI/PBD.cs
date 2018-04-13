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
    /// The physical block devices through which hosts access SRs
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class PBD : XenObject<PBD>
    {
        public PBD()
        {
        }

        public PBD(string uuid,
            XenRef<Host> host,
            XenRef<SR> SR,
            Dictionary<string, string> device_config,
            bool currently_attached,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.host = host;
            this.SR = SR;
            this.device_config = device_config;
            this.currently_attached = currently_attached;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new PBD from a Proxy_PBD.
        /// </summary>
        /// <param name="proxy"></param>
        public PBD(Proxy_PBD proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given PBD.
        /// </summary>
        public override void UpdateFrom(PBD update)
        {
            uuid = update.uuid;
            host = update.host;
            SR = update.SR;
            device_config = update.device_config;
            currently_attached = update.currently_attached;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_PBD proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            host = proxy.host == null ? null : XenRef<Host>.Create(proxy.host);
            SR = proxy.SR == null ? null : XenRef<SR>.Create(proxy.SR);
            device_config = proxy.device_config == null ? null : Maps.convert_from_proxy_string_string(proxy.device_config);
            currently_attached = (bool)proxy.currently_attached;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_PBD ToProxy()
        {
            Proxy_PBD result_ = new Proxy_PBD();
            result_.uuid = uuid ?? "";
            result_.host = host ?? "";
            result_.SR = SR ?? "";
            result_.device_config = Maps.convert_to_proxy_string_string(device_config);
            result_.currently_attached = currently_attached;
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new PBD from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public PBD(Hashtable table) : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this PBD
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
            if (table.ContainsKey("SR"))
                SR = Marshalling.ParseRef<SR>(table, "SR");
            if (table.ContainsKey("device_config"))
                device_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "device_config"));
            if (table.ContainsKey("currently_attached"))
                currently_attached = Marshalling.ParseBool(table, "currently_attached");
            if (table.ContainsKey("other_config"))
                other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(PBD other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._host, other._host) &&
                Helper.AreEqual2(this._SR, other._SR) &&
                Helper.AreEqual2(this._device_config, other._device_config) &&
                Helper.AreEqual2(this._currently_attached, other._currently_attached) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        internal static List<PBD> ProxyArrayToObjectList(Proxy_PBD[] input)
        {
            var result = new List<PBD>();
            foreach (var item in input)
                result.Add(new PBD(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, PBD server)
        {
            if (opaqueRef == null)
            {
                var reference = create(session, this);
                return reference == null ? null : reference.opaque_ref;
            }
            else
            {
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    PBD.set_other_config(session, opaqueRef, _other_config);
                }
                if (!Helper.AreEqual2(_device_config, server._device_config))
                {
                    PBD.set_device_config(session, opaqueRef, _device_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given PBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        public static PBD get_record(Session session, string _pbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pbd_get_record(session.opaque_ref, _pbd);
            else
                return new PBD(session.proxy.pbd_get_record(session.opaque_ref, _pbd ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the PBD instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<PBD> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pbd_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<PBD>.Create(session.proxy.pbd_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Create a new PBD instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<PBD> create(Session session, PBD _record)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pbd_create(session.opaque_ref, _record);
            else
                return XenRef<PBD>.Create(session.proxy.pbd_create(session.opaque_ref, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Create a new PBD instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<Task> async_create(Session session, PBD _record)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pbd_create(session.opaque_ref, _record);
          else
              return XenRef<Task>.Create(session.proxy.async_pbd_create(session.opaque_ref, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Destroy the specified PBD instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        public static void destroy(Session session, string _pbd)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pbd_destroy(session.opaque_ref, _pbd);
            else
                session.proxy.pbd_destroy(session.opaque_ref, _pbd ?? "").parse();
        }

        /// <summary>
        /// Destroy the specified PBD instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        public static XenRef<Task> async_destroy(Session session, string _pbd)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pbd_destroy(session.opaque_ref, _pbd);
          else
              return XenRef<Task>.Create(session.proxy.async_pbd_destroy(session.opaque_ref, _pbd ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given PBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        public static string get_uuid(Session session, string _pbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pbd_get_uuid(session.opaque_ref, _pbd);
            else
                return session.proxy.pbd_get_uuid(session.opaque_ref, _pbd ?? "").parse();
        }

        /// <summary>
        /// Get the host field of the given PBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        public static XenRef<Host> get_host(Session session, string _pbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pbd_get_host(session.opaque_ref, _pbd);
            else
                return XenRef<Host>.Create(session.proxy.pbd_get_host(session.opaque_ref, _pbd ?? "").parse());
        }

        /// <summary>
        /// Get the SR field of the given PBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        public static XenRef<SR> get_SR(Session session, string _pbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pbd_get_sr(session.opaque_ref, _pbd);
            else
                return XenRef<SR>.Create(session.proxy.pbd_get_sr(session.opaque_ref, _pbd ?? "").parse());
        }

        /// <summary>
        /// Get the device_config field of the given PBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        public static Dictionary<string, string> get_device_config(Session session, string _pbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pbd_get_device_config(session.opaque_ref, _pbd);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.pbd_get_device_config(session.opaque_ref, _pbd ?? "").parse());
        }

        /// <summary>
        /// Get the currently_attached field of the given PBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        public static bool get_currently_attached(Session session, string _pbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pbd_get_currently_attached(session.opaque_ref, _pbd);
            else
                return (bool)session.proxy.pbd_get_currently_attached(session.opaque_ref, _pbd ?? "").parse();
        }

        /// <summary>
        /// Get the other_config field of the given PBD.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        public static Dictionary<string, string> get_other_config(Session session, string _pbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pbd_get_other_config(session.opaque_ref, _pbd);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.pbd_get_other_config(session.opaque_ref, _pbd ?? "").parse());
        }

        /// <summary>
        /// Set the other_config field of the given PBD.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _pbd, Dictionary<string, string> _other_config)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pbd_set_other_config(session.opaque_ref, _pbd, _other_config);
            else
                session.proxy.pbd_set_other_config(session.opaque_ref, _pbd ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given PBD.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _pbd, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pbd_add_to_other_config(session.opaque_ref, _pbd, _key, _value);
            else
                session.proxy.pbd_add_to_other_config(session.opaque_ref, _pbd ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given PBD.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _pbd, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pbd_remove_from_other_config(session.opaque_ref, _pbd, _key);
            else
                session.proxy.pbd_remove_from_other_config(session.opaque_ref, _pbd ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// Activate the specified PBD, causing the referenced SR to be attached and scanned
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        public static void plug(Session session, string _pbd)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pbd_plug(session.opaque_ref, _pbd);
            else
                session.proxy.pbd_plug(session.opaque_ref, _pbd ?? "").parse();
        }

        /// <summary>
        /// Activate the specified PBD, causing the referenced SR to be attached and scanned
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        public static XenRef<Task> async_plug(Session session, string _pbd)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pbd_plug(session.opaque_ref, _pbd);
          else
              return XenRef<Task>.Create(session.proxy.async_pbd_plug(session.opaque_ref, _pbd ?? "").parse());
        }

        /// <summary>
        /// Deactivate the specified PBD, causing the referenced SR to be detached and nolonger scanned
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        public static void unplug(Session session, string _pbd)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pbd_unplug(session.opaque_ref, _pbd);
            else
                session.proxy.pbd_unplug(session.opaque_ref, _pbd ?? "").parse();
        }

        /// <summary>
        /// Deactivate the specified PBD, causing the referenced SR to be detached and nolonger scanned
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        public static XenRef<Task> async_unplug(Session session, string _pbd)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pbd_unplug(session.opaque_ref, _pbd);
          else
              return XenRef<Task>.Create(session.proxy.async_pbd_unplug(session.opaque_ref, _pbd ?? "").parse());
        }

        /// <summary>
        /// Sets the PBD's device_config field
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        /// <param name="_value">The new value of the PBD's device_config</param>
        public static void set_device_config(Session session, string _pbd, Dictionary<string, string> _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pbd_set_device_config(session.opaque_ref, _pbd, _value);
            else
                session.proxy.pbd_set_device_config(session.opaque_ref, _pbd ?? "", Maps.convert_to_proxy_string_string(_value)).parse();
        }

        /// <summary>
        /// Sets the PBD's device_config field
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pbd">The opaque_ref of the given pbd</param>
        /// <param name="_value">The new value of the PBD's device_config</param>
        public static XenRef<Task> async_set_device_config(Session session, string _pbd, Dictionary<string, string> _value)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pbd_set_device_config(session.opaque_ref, _pbd, _value);
          else
              return XenRef<Task>.Create(session.proxy.async_pbd_set_device_config(session.opaque_ref, _pbd ?? "", Maps.convert_to_proxy_string_string(_value)).parse());
        }

        /// <summary>
        /// Return a list of all the PBDs known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<PBD>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pbd_get_all(session.opaque_ref);
            else
                return XenRef<PBD>.Create(session.proxy.pbd_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the PBD Records at once, in a single XML RPC call
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<PBD>, PBD> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pbd_get_all_records(session.opaque_ref);
            else
                return XenRef<PBD>.Create<Proxy_PBD>(session.proxy.pbd_get_all_records(session.opaque_ref).parse());
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
        /// physical machine on which the pbd is available
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
        /// the storage repository that the pbd realises
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<SR>))]
        public virtual XenRef<SR> SR
        {
            get { return _SR; }
            set
            {
                if (!Helper.AreEqual(value, _SR))
                {
                    _SR = value;
                    Changed = true;
                    NotifyPropertyChanged("SR");
                }
            }
        }
        private XenRef<SR> _SR = new XenRef<SR>(Helper.NullOpaqueRef);

        /// <summary>
        /// a config string to string map that is provided to the host's SR-backend-driver
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> device_config
        {
            get { return _device_config; }
            set
            {
                if (!Helper.AreEqual(value, _device_config))
                {
                    _device_config = value;
                    Changed = true;
                    NotifyPropertyChanged("device_config");
                }
            }
        }
        private Dictionary<string, string> _device_config = new Dictionary<string, string>() {};

        /// <summary>
        /// is the SR currently attached on this host?
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
        private bool _currently_attached;

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
