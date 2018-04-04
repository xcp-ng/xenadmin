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
using Newtonsoft.Json.Converters;


namespace XenAPI
{
    /// <summary>
    /// Describes the storage that is available to a PVS site for caching purposes
    /// First published in XenServer 7.1.
    /// </summary>
    public partial class PVS_cache_storage : XenObject<PVS_cache_storage>
    {
        public PVS_cache_storage()
        {
        }

        public PVS_cache_storage(string uuid,
            XenRef<Host> host,
            XenRef<SR> SR,
            XenRef<PVS_site> site,
            long size,
            XenRef<VDI> VDI)
        {
            this.uuid = uuid;
            this.host = host;
            this.SR = SR;
            this.site = site;
            this.size = size;
            this.VDI = VDI;
        }

        /// <summary>
        /// Creates a new PVS_cache_storage from a Proxy_PVS_cache_storage.
        /// </summary>
        /// <param name="proxy"></param>
        public PVS_cache_storage(Proxy_PVS_cache_storage proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given PVS_cache_storage.
        /// </summary>
        public override void UpdateFrom(PVS_cache_storage update)
        {
            uuid = update.uuid;
            host = update.host;
            SR = update.SR;
            site = update.site;
            size = update.size;
            VDI = update.VDI;
        }

        internal void UpdateFromProxy(Proxy_PVS_cache_storage proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            host = proxy.host == null ? null : XenRef<Host>.Create(proxy.host);
            SR = proxy.SR == null ? null : XenRef<SR>.Create(proxy.SR);
            site = proxy.site == null ? null : XenRef<PVS_site>.Create(proxy.site);
            size = proxy.size == null ? 0 : long.Parse(proxy.size);
            VDI = proxy.VDI == null ? null : XenRef<VDI>.Create(proxy.VDI);
        }

        public Proxy_PVS_cache_storage ToProxy()
        {
            Proxy_PVS_cache_storage result_ = new Proxy_PVS_cache_storage();
            result_.uuid = uuid ?? "";
            result_.host = host ?? "";
            result_.SR = SR ?? "";
            result_.site = site ?? "";
            result_.size = size.ToString();
            result_.VDI = VDI ?? "";
            return result_;
        }

        /// <summary>
        /// Creates a new PVS_cache_storage from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public PVS_cache_storage(Hashtable table) : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this PVS_cache_storage
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
            if (table.ContainsKey("site"))
                site = Marshalling.ParseRef<PVS_site>(table, "site");
            if (table.ContainsKey("size"))
                size = Marshalling.ParseLong(table, "size");
            if (table.ContainsKey("VDI"))
                VDI = Marshalling.ParseRef<VDI>(table, "VDI");
        }

        public bool DeepEquals(PVS_cache_storage other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._host, other._host) &&
                Helper.AreEqual2(this._SR, other._SR) &&
                Helper.AreEqual2(this._site, other._site) &&
                Helper.AreEqual2(this._size, other._size) &&
                Helper.AreEqual2(this._VDI, other._VDI);
        }

        internal static List<PVS_cache_storage> ProxyArrayToObjectList(Proxy_PVS_cache_storage[] input)
        {
            var result = new List<PVS_cache_storage>();
            foreach (var item in input)
                result.Add(new PVS_cache_storage(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, PVS_cache_storage server)
        {
            if (opaqueRef == null)
            {
                var reference = create(session, this);
                return reference == null ? null : reference.opaque_ref;
            }
            else
            {
              throw new InvalidOperationException("This type has no read/write properties");
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given PVS_cache_storage.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static PVS_cache_storage get_record(Session session, string _pvs_cache_storage)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_cache_storage_get_record(session.opaque_ref, _pvs_cache_storage);
            else
                return new PVS_cache_storage((Proxy_PVS_cache_storage)session.proxy.pvs_cache_storage_get_record(session.opaque_ref, _pvs_cache_storage ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the PVS_cache_storage instance with the specified UUID.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<PVS_cache_storage> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_cache_storage_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<PVS_cache_storage>.Create(session.proxy.pvs_cache_storage_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Create a new PVS_cache_storage instance, and return its handle.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<PVS_cache_storage> create(Session session, PVS_cache_storage _record)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_cache_storage_create(session.opaque_ref, _record);
            else
                return XenRef<PVS_cache_storage>.Create(session.proxy.pvs_cache_storage_create(session.opaque_ref, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Create a new PVS_cache_storage instance, and return its handle.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<Task> async_create(Session session, PVS_cache_storage _record)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pvs_cache_storage_create(session.opaque_ref, _record);
          else
              return XenRef<Task>.Create(session.proxy.async_pvs_cache_storage_create(session.opaque_ref, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Destroy the specified PVS_cache_storage instance.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static void destroy(Session session, string _pvs_cache_storage)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.pvs_cache_storage_destroy(session.opaque_ref, _pvs_cache_storage);
            else
                session.proxy.pvs_cache_storage_destroy(session.opaque_ref, _pvs_cache_storage ?? "").parse();
        }

        /// <summary>
        /// Destroy the specified PVS_cache_storage instance.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static XenRef<Task> async_destroy(Session session, string _pvs_cache_storage)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_pvs_cache_storage_destroy(session.opaque_ref, _pvs_cache_storage);
          else
              return XenRef<Task>.Create(session.proxy.async_pvs_cache_storage_destroy(session.opaque_ref, _pvs_cache_storage ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given PVS_cache_storage.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static string get_uuid(Session session, string _pvs_cache_storage)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_cache_storage_get_uuid(session.opaque_ref, _pvs_cache_storage);
            else
                return session.proxy.pvs_cache_storage_get_uuid(session.opaque_ref, _pvs_cache_storage ?? "").parse();
        }

        /// <summary>
        /// Get the host field of the given PVS_cache_storage.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static XenRef<Host> get_host(Session session, string _pvs_cache_storage)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_cache_storage_get_host(session.opaque_ref, _pvs_cache_storage);
            else
                return XenRef<Host>.Create(session.proxy.pvs_cache_storage_get_host(session.opaque_ref, _pvs_cache_storage ?? "").parse());
        }

        /// <summary>
        /// Get the SR field of the given PVS_cache_storage.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static XenRef<SR> get_SR(Session session, string _pvs_cache_storage)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_cache_storage_get_sr(session.opaque_ref, _pvs_cache_storage);
            else
                return XenRef<SR>.Create(session.proxy.pvs_cache_storage_get_sr(session.opaque_ref, _pvs_cache_storage ?? "").parse());
        }

        /// <summary>
        /// Get the site field of the given PVS_cache_storage.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static XenRef<PVS_site> get_site(Session session, string _pvs_cache_storage)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_cache_storage_get_site(session.opaque_ref, _pvs_cache_storage);
            else
                return XenRef<PVS_site>.Create(session.proxy.pvs_cache_storage_get_site(session.opaque_ref, _pvs_cache_storage ?? "").parse());
        }

        /// <summary>
        /// Get the size field of the given PVS_cache_storage.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static long get_size(Session session, string _pvs_cache_storage)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_cache_storage_get_size(session.opaque_ref, _pvs_cache_storage);
            else
                return long.Parse(session.proxy.pvs_cache_storage_get_size(session.opaque_ref, _pvs_cache_storage ?? "").parse());
        }

        /// <summary>
        /// Get the VDI field of the given PVS_cache_storage.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pvs_cache_storage">The opaque_ref of the given pvs_cache_storage</param>
        public static XenRef<VDI> get_VDI(Session session, string _pvs_cache_storage)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_cache_storage_get_vdi(session.opaque_ref, _pvs_cache_storage);
            else
                return XenRef<VDI>.Create(session.proxy.pvs_cache_storage_get_vdi(session.opaque_ref, _pvs_cache_storage ?? "").parse());
        }

        /// <summary>
        /// Return a list of all the PVS_cache_storages known to the system.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<PVS_cache_storage>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_cache_storage_get_all(session.opaque_ref);
            else
                return XenRef<PVS_cache_storage>.Create(session.proxy.pvs_cache_storage_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the PVS_cache_storage Records at once, in a single XML RPC call
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<PVS_cache_storage>, PVS_cache_storage> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.pvs_cache_storage_get_all_records(session.opaque_ref);
            else
                return XenRef<PVS_cache_storage>.Create<Proxy_PVS_cache_storage>(session.proxy.pvs_cache_storage_get_all_records(session.opaque_ref).parse());
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
        /// The host on which this object defines PVS cache storage
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
        private XenRef<Host> _host = new XenRef<Host>("OpaqueRef:NULL");

        /// <summary>
        /// SR providing storage for the PVS cache
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
        private XenRef<SR> _SR = new XenRef<SR>("OpaqueRef:NULL");

        /// <summary>
        /// The PVS_site for which this object defines the storage
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
                    Changed = true;
                    NotifyPropertyChanged("site");
                }
            }
        }
        private XenRef<PVS_site> _site = new XenRef<PVS_site>("OpaqueRef:NULL");

        /// <summary>
        /// The size of the cache VDI (in bytes)
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
        private long _size = 21474836480;

        /// <summary>
        /// The VDI used for caching
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VDI>))]
        public virtual XenRef<VDI> VDI
        {
            get { return _VDI; }
            set
            {
                if (!Helper.AreEqual(value, _VDI))
                {
                    _VDI = value;
                    Changed = true;
                    NotifyPropertyChanged("VDI");
                }
            }
        }
        private XenRef<VDI> _VDI = new XenRef<VDI>("OpaqueRef:NULL");
    }
}
