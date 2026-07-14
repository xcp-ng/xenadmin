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
    /// a proxy connects a VM/VIF with a PVS site
    /// First published in XenServer 7.1.
    /// </summary>
    public partial class PVS_proxy : XenObject<PVS_proxy>
    {
        #region Constructors

        public PVS_proxy()
        {
        }

        public PVS_proxy(string uuid,
            XenRef<PVS_site> site,
            XenRef<VIF> VIF,
            bool currently_attached,
            pvs_proxy_status status)
        {
            this.uuid = uuid;
            this.site = site;
            this.VIF = VIF;
            this.currently_attached = currently_attached;
            this.status = status;
        }

        /// <summary>
        /// Creates a new PVS_proxy from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public PVS_proxy(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given PVS_proxy.
        /// </summary>
        public override void UpdateFrom(PVS_proxy record)
        {
            uuid = record.uuid;
            site = record.site;
            VIF = record.VIF;
            currently_attached = record.currently_attached;
            status = record.status;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this PVS_proxy
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("site"))
                site = Marshalling.ParseRef<PVS_site>(table, "site");
            if (table.ContainsKey("VIF"))
                VIF = Marshalling.ParseRef<VIF>(table, "VIF");
            if (table.ContainsKey("currently_attached"))
                currently_attached = Marshalling.ParseBool(table, "currently_attached");
            if (table.ContainsKey("status"))
                status = (pvs_proxy_status)Helper.EnumParseDefault(typeof(pvs_proxy_status), Marshalling.ParseString(table, "status"));
        }

        public bool DeepEquals(PVS_proxy other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_site, other._site) &&
                Helper.AreEqual2(_VIF, other._VIF) &&
                Helper.AreEqual2(_currently_attached, other._currently_attached) &&
                Helper.AreEqual2(_status, other._status);
        }


        /// <summary>
        /// Get a record containing the current state of the given PVS_proxy.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static PVS_proxy get_record(Session session, string _pvs_proxy)
        {
            return session.JsonRpcClient.pvs_proxy_get_record(session.opaque_ref, _pvs_proxy);
        }

        /// <summary>
        /// Get a reference to the PVS_proxy instance with the specified UUID.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PVS_proxy> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.pvs_proxy_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given PVS_proxy.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _pvs_proxy)
        {
            return session.JsonRpcClient.pvs_proxy_get_uuid(session.opaque_ref, _pvs_proxy);
        }

        /// <summary>
        /// Get the site field of the given PVS_proxy.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PVS_site> get_site(Session session, string _pvs_proxy)
        {
            return session.JsonRpcClient.pvs_proxy_get_site(session.opaque_ref, _pvs_proxy);
        }

        /// <summary>
        /// Get the VIF field of the given PVS_proxy.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VIF> get_VIF(Session session, string _pvs_proxy)
        {
            return session.JsonRpcClient.pvs_proxy_get_vif(session.opaque_ref, _pvs_proxy);
        }

        /// <summary>
        /// Get the currently_attached field of the given PVS_proxy.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_currently_attached(Session session, string _pvs_proxy)
        {
            return session.JsonRpcClient.pvs_proxy_get_currently_attached(session.opaque_ref, _pvs_proxy);
        }

        /// <summary>
        /// Get the status field of the given PVS_proxy.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static pvs_proxy_status get_status(Session session, string _pvs_proxy)
        {
            return session.JsonRpcClient.pvs_proxy_get_status(session.opaque_ref, _pvs_proxy);
        }

        /// <summary>
        /// Configure a VM/VIF to use a PVS proxy
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_site">PVS site that we proxy for</param>
        /// <param name="_vif">VIF for the VM that needs to be proxied</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<PVS_proxy> create(Session session, string _site, string _vif)
        {
            return session.JsonRpcClient.pvs_proxy_create(session.opaque_ref, _site, _vif);
        }

        /// <summary>
        /// Configure a VM/VIF to use a PVS proxy
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_site">PVS site that we proxy for</param>
        /// <param name="_vif">VIF for the VM that needs to be proxied</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, string _site, string _vif)
        {
          return session.JsonRpcClient.async_pvs_proxy_create(session.opaque_ref, _site, _vif);
        }

        /// <summary>
        /// remove (or switch off) a PVS proxy for this VM
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _pvs_proxy)
        {
            session.JsonRpcClient.pvs_proxy_destroy(session.opaque_ref, _pvs_proxy);
        }

        /// <summary>
        /// remove (or switch off) a PVS proxy for this VM
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _pvs_proxy)
        {
          return session.JsonRpcClient.async_pvs_proxy_destroy(session.opaque_ref, _pvs_proxy);
        }

        /// <summary>
        /// Return a list of all the PVS_proxys known to the system.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PVS_proxy>> get_all(Session session)
        {
            return session.JsonRpcClient.pvs_proxy_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of PVS_proxy references to PVS_proxy records for all PVS_proxys known to the system.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<PVS_proxy>, PVS_proxy> get_all_records(Session session)
        {
            return session.JsonRpcClient.pvs_proxy_get_all_records(session.opaque_ref);
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
        /// PVS site this proxy is part of
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

        /// <summary>
        /// VIF of the VM using the proxy
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VIF>))]
        public virtual XenRef<VIF> VIF
        {
            get { return _VIF; }
            set
            {
                if (!Helper.AreEqual(value, _VIF))
                {
                    _VIF = value;
                    NotifyPropertyChanged("VIF");
                }
            }
        }
        private XenRef<VIF> _VIF = new XenRef<VIF>("OpaqueRef:NULL");

        /// <summary>
        /// true = VM is currently proxied
        /// </summary>
        public virtual bool currently_attached
        {
            get { return _currently_attached; }
            set
            {
                if (!Helper.AreEqual(value, _currently_attached))
                {
                    _currently_attached = value;
                    NotifyPropertyChanged("currently_attached");
                }
            }
        }
        private bool _currently_attached = false;

        /// <summary>
        /// The run-time status of the proxy
        /// </summary>
        [JsonConverter(typeof(pvs_proxy_statusConverter))]
        public virtual pvs_proxy_status status
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
        private pvs_proxy_status _status = pvs_proxy_status.stopped;
    }
}
