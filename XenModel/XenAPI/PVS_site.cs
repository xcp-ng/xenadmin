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
    /// machines serving blocks of data for provisioning VMs
    /// First published in XenServer 7.1.
    /// </summary>
    public partial class PVS_site : XenObject<PVS_site>
    {
        public PVS_site()
        {
        }

        public PVS_site(string uuid,
            string name_label,
            string name_description,
            string PVS_uuid,
            List<XenRef<PVS_cache_storage>> cache_storage,
            List<XenRef<PVS_server>> servers,
            List<XenRef<PVS_proxy>> proxies)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.PVS_uuid = PVS_uuid;
            this.cache_storage = cache_storage;
            this.servers = servers;
            this.proxies = proxies;
        }

        /// <summary>
        /// Creates a new PVS_site from a Proxy_PVS_site.
        /// </summary>
        /// <param name="proxy"></param>
        public PVS_site(Proxy_PVS_site proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given PVS_site.
        /// </summary>
        public override void UpdateFrom(PVS_site update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            PVS_uuid = update.PVS_uuid;
            cache_storage = update.cache_storage;
            servers = update.servers;
            proxies = update.proxies;
        }

        internal void UpdateFromProxy(Proxy_PVS_site proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            name_label = proxy.name_label == null ? null : proxy.name_label;
            name_description = proxy.name_description == null ? null : proxy.name_description;
            PVS_uuid = proxy.PVS_uuid == null ? null : proxy.PVS_uuid;
            cache_storage = proxy.cache_storage == null ? null : XenRef<PVS_cache_storage>.Create(proxy.cache_storage);
            servers = proxy.servers == null ? null : XenRef<PVS_server>.Create(proxy.servers);
            proxies = proxy.proxies == null ? null : XenRef<PVS_proxy>.Create(proxy.proxies);
        }

        public Proxy_PVS_site ToProxy()
        {
            Proxy_PVS_site result_ = new Proxy_PVS_site();
            result_.uuid = uuid ?? "";
            result_.name_label = name_label ?? "";
            result_.name_description = name_description ?? "";
            result_.PVS_uuid = PVS_uuid ?? "";
            result_.cache_storage = cache_storage == null ? new string[] {} : Helper.RefListToStringArray(cache_storage);
            result_.servers = servers == null ? new string[] {} : Helper.RefListToStringArray(servers);
            result_.proxies = proxies == null ? new string[] {} : Helper.RefListToStringArray(proxies);
            return result_;
        }

        /// <summary>
        /// Creates a new PVS_site from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public PVS_site(Hashtable table) : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this PVS_site
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("name_label"))
                name_label = Marshalling.ParseString(table, "name_label");
            if (table.ContainsKey("name_description"))
                name_description = Marshalling.ParseString(table, "name_description");
            if (table.ContainsKey("PVS_uuid"))
                PVS_uuid = Marshalling.ParseString(table, "PVS_uuid");
            if (table.ContainsKey("cache_storage"))
                cache_storage = Marshalling.ParseSetRef<PVS_cache_storage>(table, "cache_storage");
            if (table.ContainsKey("servers"))
                servers = Marshalling.ParseSetRef<PVS_server>(table, "servers");
            if (table.ContainsKey("proxies"))
                proxies = Marshalling.ParseSetRef<PVS_proxy>(table, "proxies");
        }

        public bool DeepEquals(PVS_site other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name_label, other._name_label) &&
                Helper.AreEqual2(this._name_description, other._name_description) &&
                Helper.AreEqual2(this._PVS_uuid, other._PVS_uuid) &&
                Helper.AreEqual2(this._cache_storage, other._cache_storage) &&
                Helper.AreEqual2(this._servers, other._servers) &&
                Helper.AreEqual2(this._proxies, other._proxies);
        }

        internal static List<PVS_site> ProxyArrayToObjectList(Proxy_PVS_site[] input)
        {
            var result = new List<PVS_site>();
            foreach (var item in input)
                result.Add(new PVS_site(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, PVS_site server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
                if (!Helper.AreEqual2(_name_label, server._name_label))
                {
                    PVS_site.set_name_label(session, opaqueRef, _name_label);
                }
                if (!Helper.AreEqual2(_name_description, server._name_description))
                {
                    PVS_site.set_name_description(session, opaqueRef, _name_description);
                }
                if (!Helper.AreEqual2(_PVS_uuid, server._PVS_uuid))
                {
                    PVS_site.set_PVS_uuid(session, opaqueRef, _PVS_uuid);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static PVS_site get_record(Session session, string _pvs_site)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_site_get_record(session.opaque_ref, _pvs_site);
            else
                return new PVS_site(session.proxy.pvs_site_get_record(session.opaque_ref, _pvs_site ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the PVS_site instance with the specified UUID.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<PVS_site> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_site_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<PVS_site>.Create(session.proxy.pvs_site_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get all the PVS_site instances with the given label.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        public static List<XenRef<PVS_site>> get_by_name_label(Session session, string _label)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_site_get_by_name_label(session.opaque_ref, _label);
            else
                return XenRef<PVS_site>.Create(session.proxy.pvs_site_get_by_name_label(session.opaque_ref, _label ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static string get_uuid(Session session, string _pvs_site)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_site_get_uuid(session.opaque_ref, _pvs_site);
            else
                return session.proxy.pvs_site_get_uuid(session.opaque_ref, _pvs_site ?? "").parse();
        }

        /// <summary>
        /// Get the name/label field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static string get_name_label(Session session, string _pvs_site)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_site_get_name_label(session.opaque_ref, _pvs_site);
            else
                return session.proxy.pvs_site_get_name_label(session.opaque_ref, _pvs_site ?? "").parse();
        }

        /// <summary>
        /// Get the name/description field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static string get_name_description(Session session, string _pvs_site)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_site_get_name_description(session.opaque_ref, _pvs_site);
            else
                return session.proxy.pvs_site_get_name_description(session.opaque_ref, _pvs_site ?? "").parse();
        }

        /// <summary>
        /// Get the PVS_uuid field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static string get_PVS_uuid(Session session, string _pvs_site)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_site_get_pvs_uuid(session.opaque_ref, _pvs_site);
            else
                return session.proxy.pvs_site_get_pvs_uuid(session.opaque_ref, _pvs_site ?? "").parse();
        }

        /// <summary>
        /// Get the cache_storage field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static List<XenRef<PVS_cache_storage>> get_cache_storage(Session session, string _pvs_site)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_site_get_cache_storage(session.opaque_ref, _pvs_site);
            else
                return XenRef<PVS_cache_storage>.Create(session.proxy.pvs_site_get_cache_storage(session.opaque_ref, _pvs_site ?? "").parse());
        }

        /// <summary>
        /// Get the servers field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static List<XenRef<PVS_server>> get_servers(Session session, string _pvs_site)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_site_get_servers(session.opaque_ref, _pvs_site);
            else
                return XenRef<PVS_server>.Create(session.proxy.pvs_site_get_servers(session.opaque_ref, _pvs_site ?? "").parse());
        }

        /// <summary>
        /// Get the proxies field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static List<XenRef<PVS_proxy>> get_proxies(Session session, string _pvs_site)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_site_get_proxies(session.opaque_ref, _pvs_site);
            else
                return XenRef<PVS_proxy>.Create(session.proxy.pvs_site_get_proxies(session.opaque_ref, _pvs_site ?? "").parse());
        }

        /// <summary>
        /// Set the name/label field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <param name="_label">New value to set</param>
        public static void set_name_label(Session session, string _pvs_site, string _label)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pvs_site_set_name_label(session.opaque_ref, _pvs_site, _label);
            else
                session.proxy.pvs_site_set_name_label(session.opaque_ref, _pvs_site ?? "", _label ?? "").parse();
        }

        /// <summary>
        /// Set the name/description field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <param name="_description">New value to set</param>
        public static void set_name_description(Session session, string _pvs_site, string _description)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pvs_site_set_name_description(session.opaque_ref, _pvs_site, _description);
            else
                session.proxy.pvs_site_set_name_description(session.opaque_ref, _pvs_site ?? "", _description ?? "").parse();
        }

        /// <summary>
        /// Introduce new PVS site
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name_label">name of the PVS site</param>
        /// <param name="_name_description">description of the PVS site</param>
        /// <param name="_pvs_uuid">unique identifier of the PVS site</param>
        public static XenRef<PVS_site> introduce(Session session, string _name_label, string _name_description, string _pvs_uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_site_introduce(session.opaque_ref, _name_label, _name_description, _pvs_uuid);
            else
                return XenRef<PVS_site>.Create(session.proxy.pvs_site_introduce(session.opaque_ref, _name_label ?? "", _name_description ?? "", _pvs_uuid ?? "").parse());
        }

        /// <summary>
        /// Introduce new PVS site
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name_label">name of the PVS site</param>
        /// <param name="_name_description">description of the PVS site</param>
        /// <param name="_pvs_uuid">unique identifier of the PVS site</param>
        public static XenRef<Task> async_introduce(Session session, string _name_label, string _name_description, string _pvs_uuid)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pvs_site_introduce(session.opaque_ref, _name_label, _name_description, _pvs_uuid);
          else
              return XenRef<Task>.Create(session.proxy.async_pvs_site_introduce(session.opaque_ref, _name_label ?? "", _name_description ?? "", _pvs_uuid ?? "").parse());
        }

        /// <summary>
        /// Remove a site's meta data
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static void forget(Session session, string _pvs_site)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pvs_site_forget(session.opaque_ref, _pvs_site);
            else
                session.proxy.pvs_site_forget(session.opaque_ref, _pvs_site ?? "").parse();
        }

        /// <summary>
        /// Remove a site's meta data
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        public static XenRef<Task> async_forget(Session session, string _pvs_site)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pvs_site_forget(session.opaque_ref, _pvs_site);
          else
              return XenRef<Task>.Create(session.proxy.async_pvs_site_forget(session.opaque_ref, _pvs_site ?? "").parse());
        }

        /// <summary>
        /// Update the PVS UUID of the PVS site
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <param name="_value">PVS UUID to be used</param>
        public static void set_PVS_uuid(Session session, string _pvs_site, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pvs_site_set_pvs_uuid(session.opaque_ref, _pvs_site, _value);
            else
                session.proxy.pvs_site_set_pvs_uuid(session.opaque_ref, _pvs_site ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Update the PVS UUID of the PVS site
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <param name="_value">PVS UUID to be used</param>
        public static XenRef<Task> async_set_PVS_uuid(Session session, string _pvs_site, string _value)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pvs_site_set_pvs_uuid(session.opaque_ref, _pvs_site, _value);
          else
              return XenRef<Task>.Create(session.proxy.async_pvs_site_set_pvs_uuid(session.opaque_ref, _pvs_site ?? "", _value ?? "").parse());
        }

        /// <summary>
        /// Return a list of all the PVS_sites known to the system.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<PVS_site>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_site_get_all(session.opaque_ref);
            else
                return XenRef<PVS_site>.Create(session.proxy.pvs_site_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the PVS_site Records at once, in a single XML RPC call
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<PVS_site>, PVS_site> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_site_get_all_records(session.opaque_ref);
            else
                return XenRef<PVS_site>.Create<Proxy_PVS_site>(session.proxy.pvs_site_get_all_records(session.opaque_ref).parse());
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
        /// a human-readable name
        /// </summary>
        public virtual string name_label
        {
            get { return _name_label; }
            set
            {
                if (!Helper.AreEqual(value, _name_label))
                {
                    _name_label = value;
                    Changed = true;
                    NotifyPropertyChanged("name_label");
                }
            }
        }
        private string _name_label = "";

        /// <summary>
        /// a notes field containing human-readable description
        /// </summary>
        public virtual string name_description
        {
            get { return _name_description; }
            set
            {
                if (!Helper.AreEqual(value, _name_description))
                {
                    _name_description = value;
                    Changed = true;
                    NotifyPropertyChanged("name_description");
                }
            }
        }
        private string _name_description = "";

        /// <summary>
        /// Unique identifier of the PVS site, as configured in PVS
        /// </summary>
        public virtual string PVS_uuid
        {
            get { return _PVS_uuid; }
            set
            {
                if (!Helper.AreEqual(value, _PVS_uuid))
                {
                    _PVS_uuid = value;
                    Changed = true;
                    NotifyPropertyChanged("PVS_uuid");
                }
            }
        }
        private string _PVS_uuid = "";

        /// <summary>
        /// The SR used by PVS proxy for the cache
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<PVS_cache_storage>))]
        public virtual List<XenRef<PVS_cache_storage>> cache_storage
        {
            get { return _cache_storage; }
            set
            {
                if (!Helper.AreEqual(value, _cache_storage))
                {
                    _cache_storage = value;
                    Changed = true;
                    NotifyPropertyChanged("cache_storage");
                }
            }
        }
        private List<XenRef<PVS_cache_storage>> _cache_storage = new List<XenRef<PVS_cache_storage>>() {};

        /// <summary>
        /// The set of PVS servers in the site
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<PVS_server>))]
        public virtual List<XenRef<PVS_server>> servers
        {
            get { return _servers; }
            set
            {
                if (!Helper.AreEqual(value, _servers))
                {
                    _servers = value;
                    Changed = true;
                    NotifyPropertyChanged("servers");
                }
            }
        }
        private List<XenRef<PVS_server>> _servers = new List<XenRef<PVS_server>>() {};

        /// <summary>
        /// The set of proxies associated with the site
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<PVS_proxy>))]
        public virtual List<XenRef<PVS_proxy>> proxies
        {
            get { return _proxies; }
            set
            {
                if (!Helper.AreEqual(value, _proxies))
                {
                    _proxies = value;
                    Changed = true;
                    NotifyPropertyChanged("proxies");
                }
            }
        }
        private List<XenRef<PVS_proxy>> _proxies = new List<XenRef<PVS_proxy>>() {};
    }
}
