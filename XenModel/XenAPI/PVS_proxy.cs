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

        /// <summary>
        /// Creates a new PVS_proxy from a Proxy_PVS_proxy.
        /// </summary>
        /// <param name="proxy"></param>
        public PVS_proxy(Proxy_PVS_proxy proxy)
        {
            UpdateFrom(proxy);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given PVS_proxy.
        /// </summary>
        public override void UpdateFrom(PVS_proxy update)
        {
            uuid = update.uuid;
            site = update.site;
            VIF = update.VIF;
            currently_attached = update.currently_attached;
            status = update.status;
        }

        internal void UpdateFrom(Proxy_PVS_proxy proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            site = proxy.site == null ? null : XenRef<PVS_site>.Create(proxy.site);
            VIF = proxy.VIF == null ? null : XenRef<VIF>.Create(proxy.VIF);
            currently_attached = (bool)proxy.currently_attached;
            status = proxy.status == null ? (pvs_proxy_status) 0 : (pvs_proxy_status)Helper.EnumParseDefault(typeof(pvs_proxy_status), (string)proxy.status);
        }

        public Proxy_PVS_proxy ToProxy()
        {
            Proxy_PVS_proxy result_ = new Proxy_PVS_proxy();
            result_.uuid = uuid ?? "";
            result_.site = site ?? "";
            result_.VIF = VIF ?? "";
            result_.currently_attached = currently_attached;
            result_.status = pvs_proxy_status_helper.ToString(status);
            return result_;
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

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._site, other._site) &&
                Helper.AreEqual2(this._VIF, other._VIF) &&
                Helper.AreEqual2(this._currently_attached, other._currently_attached) &&
                Helper.AreEqual2(this._status, other._status);
        }

        internal static List<PVS_proxy> ProxyArrayToObjectList(Proxy_PVS_proxy[] input)
        {
            var result = new List<PVS_proxy>();
            foreach (var item in input)
                result.Add(new PVS_proxy(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, PVS_proxy server)
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
        /// Get a record containing the current state of the given PVS_proxy.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static PVS_proxy get_record(Session session, string _pvs_proxy)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_proxy_get_record(session.opaque_ref, _pvs_proxy);
            else
                return new PVS_proxy(session.XmlRpcProxy.pvs_proxy_get_record(session.opaque_ref, _pvs_proxy ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the PVS_proxy instance with the specified UUID.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<PVS_proxy> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_proxy_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<PVS_proxy>.Create(session.XmlRpcProxy.pvs_proxy_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given PVS_proxy.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static string get_uuid(Session session, string _pvs_proxy)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_proxy_get_uuid(session.opaque_ref, _pvs_proxy);
            else
                return session.XmlRpcProxy.pvs_proxy_get_uuid(session.opaque_ref, _pvs_proxy ?? "").parse();
        }

        /// <summary>
        /// Get the site field of the given PVS_proxy.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static XenRef<PVS_site> get_site(Session session, string _pvs_proxy)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_proxy_get_site(session.opaque_ref, _pvs_proxy);
            else
                return XenRef<PVS_site>.Create(session.XmlRpcProxy.pvs_proxy_get_site(session.opaque_ref, _pvs_proxy ?? "").parse());
        }

        /// <summary>
        /// Get the VIF field of the given PVS_proxy.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static XenRef<VIF> get_VIF(Session session, string _pvs_proxy)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_proxy_get_vif(session.opaque_ref, _pvs_proxy);
            else
                return XenRef<VIF>.Create(session.XmlRpcProxy.pvs_proxy_get_vif(session.opaque_ref, _pvs_proxy ?? "").parse());
        }

        /// <summary>
        /// Get the currently_attached field of the given PVS_proxy.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static bool get_currently_attached(Session session, string _pvs_proxy)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_proxy_get_currently_attached(session.opaque_ref, _pvs_proxy);
            else
                return (bool)session.XmlRpcProxy.pvs_proxy_get_currently_attached(session.opaque_ref, _pvs_proxy ?? "").parse();
        }

        /// <summary>
        /// Get the status field of the given PVS_proxy.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static pvs_proxy_status get_status(Session session, string _pvs_proxy)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_proxy_get_status(session.opaque_ref, _pvs_proxy);
            else
                return (pvs_proxy_status)Helper.EnumParseDefault(typeof(pvs_proxy_status), (string)session.XmlRpcProxy.pvs_proxy_get_status(session.opaque_ref, _pvs_proxy ?? "").parse());
        }

        /// <summary>
        /// Configure a VM/VIF to use a PVS proxy
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_site">PVS site that we proxy for</param>
        /// <param name="_vif">VIF for the VM that needs to be proxied</param>
        public static XenRef<PVS_proxy> create(Session session, string _site, string _vif)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_proxy_create(session.opaque_ref, _site, _vif);
            else
                return XenRef<PVS_proxy>.Create(session.XmlRpcProxy.pvs_proxy_create(session.opaque_ref, _site ?? "", _vif ?? "").parse());
        }

        /// <summary>
        /// Configure a VM/VIF to use a PVS proxy
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_site">PVS site that we proxy for</param>
        /// <param name="_vif">VIF for the VM that needs to be proxied</param>
        public static XenRef<Task> async_create(Session session, string _site, string _vif)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pvs_proxy_create(session.opaque_ref, _site, _vif);
          else
              return XenRef<Task>.Create(session.XmlRpcProxy.async_pvs_proxy_create(session.opaque_ref, _site ?? "", _vif ?? "").parse());
        }

        /// <summary>
        /// remove (or switch off) a PVS proxy for this VM
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static void destroy(Session session, string _pvs_proxy)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pvs_proxy_destroy(session.opaque_ref, _pvs_proxy);
            else
                session.XmlRpcProxy.pvs_proxy_destroy(session.opaque_ref, _pvs_proxy ?? "").parse();
        }

        /// <summary>
        /// remove (or switch off) a PVS proxy for this VM
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_proxy">The opaque_ref of the given pvs_proxy</param>
        public static XenRef<Task> async_destroy(Session session, string _pvs_proxy)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pvs_proxy_destroy(session.opaque_ref, _pvs_proxy);
          else
              return XenRef<Task>.Create(session.XmlRpcProxy.async_pvs_proxy_destroy(session.opaque_ref, _pvs_proxy ?? "").parse());
        }

        /// <summary>
        /// Return a list of all the PVS_proxys known to the system.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<PVS_proxy>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_proxy_get_all(session.opaque_ref);
            else
                return XenRef<PVS_proxy>.Create(session.XmlRpcProxy.pvs_proxy_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the PVS_proxy Records at once, in a single XML RPC call
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<PVS_proxy>, PVS_proxy> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_proxy_get_all_records(session.opaque_ref);
            else
                return XenRef<PVS_proxy>.Create<Proxy_PVS_proxy>(session.XmlRpcProxy.pvs_proxy_get_all_records(session.opaque_ref).parse());
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
