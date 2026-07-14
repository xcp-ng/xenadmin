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
    /// machines serving blocks of data for provisioning VMs
    /// First published in XenServer 7.1.
    /// </summary>
    public partial class PVS_site : XenObject<PVS_site>
    {
        #region Constructors

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
        /// Creates a new PVS_site from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public PVS_site(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given PVS_site.
        /// </summary>
        public override void UpdateFrom(PVS_site record)
        {
            uuid = record.uuid;
            name_label = record.name_label;
            name_description = record.name_description;
            PVS_uuid = record.PVS_uuid;
            cache_storage = record.cache_storage;
            servers = record.servers;
            proxies = record.proxies;
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

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name_label, other._name_label) &&
                Helper.AreEqual2(_name_description, other._name_description) &&
                Helper.AreEqual2(_PVS_uuid, other._PVS_uuid) &&
                Helper.AreEqual2(_cache_storage, other._cache_storage) &&
                Helper.AreEqual2(_servers, other._servers) &&
                Helper.AreEqual2(_proxies, other._proxies);
        }


        /// <summary>
        /// Get a record containing the current state of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static PVS_site get_record(Session session, string _pvs_site)
        {
            return session.JsonRpcClient.pvs_site_get_record(session.opaque_ref, _pvs_site);
        }

        /// <summary>
        /// Get a reference to the PVS_site instance with the specified UUID.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PVS_site> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.pvs_site_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get all the PVS_site instances with the given label.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PVS_site>> get_by_name_label(Session session, string _label)
        {
            return session.JsonRpcClient.pvs_site_get_by_name_label(session.opaque_ref, _label);
        }

        /// <summary>
        /// Get the uuid field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _pvs_site)
        {
            return session.JsonRpcClient.pvs_site_get_uuid(session.opaque_ref, _pvs_site);
        }

        /// <summary>
        /// Get the name/label field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_label(Session session, string _pvs_site)
        {
            return session.JsonRpcClient.pvs_site_get_name_label(session.opaque_ref, _pvs_site);
        }

        /// <summary>
        /// Get the name/description field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_description(Session session, string _pvs_site)
        {
            return session.JsonRpcClient.pvs_site_get_name_description(session.opaque_ref, _pvs_site);
        }

        /// <summary>
        /// Get the PVS_uuid field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_PVS_uuid(Session session, string _pvs_site)
        {
            return session.JsonRpcClient.pvs_site_get_pvs_uuid(session.opaque_ref, _pvs_site);
        }

        /// <summary>
        /// Get the cache_storage field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PVS_cache_storage>> get_cache_storage(Session session, string _pvs_site)
        {
            return session.JsonRpcClient.pvs_site_get_cache_storage(session.opaque_ref, _pvs_site);
        }

        /// <summary>
        /// Get the servers field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PVS_server>> get_servers(Session session, string _pvs_site)
        {
            return session.JsonRpcClient.pvs_site_get_servers(session.opaque_ref, _pvs_site);
        }

        /// <summary>
        /// Get the proxies field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PVS_proxy>> get_proxies(Session session, string _pvs_site)
        {
            return session.JsonRpcClient.pvs_site_get_proxies(session.opaque_ref, _pvs_site);
        }

        /// <summary>
        /// Set the name/label field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <param name="_label">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_name_label(Session session, string _pvs_site, string _label)
        {
            session.JsonRpcClient.pvs_site_set_name_label(session.opaque_ref, _pvs_site, _label);
        }

        /// <summary>
        /// Set the name/description field of the given PVS_site.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <param name="_description">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_name_description(Session session, string _pvs_site, string _description)
        {
            session.JsonRpcClient.pvs_site_set_name_description(session.opaque_ref, _pvs_site, _description);
        }

        /// <summary>
        /// Introduce new PVS site
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name_label">name of the PVS site</param>
        /// <param name="_name_description">description of the PVS site</param>
        /// <param name="_pvs_uuid">unique identifier of the PVS site</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<PVS_site> introduce(Session session, string _name_label, string _name_description, string _pvs_uuid)
        {
            return session.JsonRpcClient.pvs_site_introduce(session.opaque_ref, _name_label, _name_description, _pvs_uuid);
        }

        /// <summary>
        /// Introduce new PVS site
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name_label">name of the PVS site</param>
        /// <param name="_name_description">description of the PVS site</param>
        /// <param name="_pvs_uuid">unique identifier of the PVS site</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_introduce(Session session, string _name_label, string _name_description, string _pvs_uuid)
        {
          return session.JsonRpcClient.async_pvs_site_introduce(session.opaque_ref, _name_label, _name_description, _pvs_uuid);
        }

        /// <summary>
        /// Remove a site's meta data
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void forget(Session session, string _pvs_site)
        {
            session.JsonRpcClient.pvs_site_forget(session.opaque_ref, _pvs_site);
        }

        /// <summary>
        /// Remove a site's meta data
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_forget(Session session, string _pvs_site)
        {
          return session.JsonRpcClient.async_pvs_site_forget(session.opaque_ref, _pvs_site);
        }

        /// <summary>
        /// Update the PVS UUID of the PVS site
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <param name="_value">PVS UUID to be used</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_PVS_uuid(Session session, string _pvs_site, string _value)
        {
            session.JsonRpcClient.pvs_site_set_pvs_uuid(session.opaque_ref, _pvs_site, _value);
        }

        /// <summary>
        /// Update the PVS UUID of the PVS site
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_site">The opaque_ref of the given pvs_site</param>
        /// <param name="_value">PVS UUID to be used</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_PVS_uuid(Session session, string _pvs_site, string _value)
        {
          return session.JsonRpcClient.async_pvs_site_set_pvs_uuid(session.opaque_ref, _pvs_site, _value);
        }

        /// <summary>
        /// Return a list of all the PVS_sites known to the system.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PVS_site>> get_all(Session session)
        {
            return session.JsonRpcClient.pvs_site_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of PVS_site references to PVS_site records for all PVS_sites known to the system.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<PVS_site>, PVS_site> get_all_records(Session session)
        {
            return session.JsonRpcClient.pvs_site_get_all_records(session.opaque_ref);
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
                    NotifyPropertyChanged("proxies");
                }
            }
        }
        private List<XenRef<PVS_proxy>> _proxies = new List<XenRef<PVS_proxy>>() {};
    }
}
