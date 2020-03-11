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
    /// A virtual TPM device
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class VTPM : XenObject<VTPM>
    {
        #region Constructors

        public VTPM()
        {
        }

        public VTPM(string uuid,
            XenRef<VM> VM,
            XenRef<VM> backend)
        {
            this.uuid = uuid;
            this.VM = VM;
            this.backend = backend;
        }

        /// <summary>
        /// Creates a new VTPM from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VTPM(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Creates a new VTPM from a Proxy_VTPM.
        /// </summary>
        /// <param name="proxy"></param>
        public VTPM(Proxy_VTPM proxy)
        {
            UpdateFrom(proxy);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VTPM.
        /// </summary>
        public override void UpdateFrom(VTPM update)
        {
            uuid = update.uuid;
            VM = update.VM;
            backend = update.backend;
        }

        internal void UpdateFrom(Proxy_VTPM proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            VM = proxy.VM == null ? null : XenRef<VM>.Create(proxy.VM);
            backend = proxy.backend == null ? null : XenRef<VM>.Create(proxy.backend);
        }

        public Proxy_VTPM ToProxy()
        {
            Proxy_VTPM result_ = new Proxy_VTPM();
            result_.uuid = uuid ?? "";
            result_.VM = VM ?? "";
            result_.backend = backend ?? "";
            return result_;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this VTPM
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("VM"))
                VM = Marshalling.ParseRef<VM>(table, "VM");
            if (table.ContainsKey("backend"))
                backend = Marshalling.ParseRef<VM>(table, "backend");
        }

        public bool DeepEquals(VTPM other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._VM, other._VM) &&
                Helper.AreEqual2(this._backend, other._backend);
        }

        internal static List<VTPM> ProxyArrayToObjectList(Proxy_VTPM[] input)
        {
            var result = new List<VTPM>();
            foreach (var item in input)
                result.Add(new VTPM(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, VTPM server)
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
        /// Get a record containing the current state of the given VTPM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static VTPM get_record(Session session, string _vtpm)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_get_record(session.opaque_ref, _vtpm);
            else
                return new VTPM(session.XmlRpcProxy.vtpm_get_record(session.opaque_ref, _vtpm ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the VTPM instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<VTPM> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<VTPM>.Create(session.XmlRpcProxy.vtpm_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Create a new VTPM instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<VTPM> create(Session session, VTPM _record)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_create(session.opaque_ref, _record);
            else
                return XenRef<VTPM>.Create(session.XmlRpcProxy.vtpm_create(session.opaque_ref, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Create a new VTPM instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<Task> async_create(Session session, VTPM _record)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vtpm_create(session.opaque_ref, _record);
          else
              return XenRef<Task>.Create(session.XmlRpcProxy.async_vtpm_create(session.opaque_ref, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Destroy the specified VTPM instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static void destroy(Session session, string _vtpm)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vtpm_destroy(session.opaque_ref, _vtpm);
            else
                session.XmlRpcProxy.vtpm_destroy(session.opaque_ref, _vtpm ?? "").parse();
        }

        /// <summary>
        /// Destroy the specified VTPM instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static XenRef<Task> async_destroy(Session session, string _vtpm)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vtpm_destroy(session.opaque_ref, _vtpm);
          else
              return XenRef<Task>.Create(session.XmlRpcProxy.async_vtpm_destroy(session.opaque_ref, _vtpm ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given VTPM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static string get_uuid(Session session, string _vtpm)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_get_uuid(session.opaque_ref, _vtpm);
            else
                return session.XmlRpcProxy.vtpm_get_uuid(session.opaque_ref, _vtpm ?? "").parse();
        }

        /// <summary>
        /// Get the VM field of the given VTPM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static XenRef<VM> get_VM(Session session, string _vtpm)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_get_vm(session.opaque_ref, _vtpm);
            else
                return XenRef<VM>.Create(session.XmlRpcProxy.vtpm_get_vm(session.opaque_ref, _vtpm ?? "").parse());
        }

        /// <summary>
        /// Get the backend field of the given VTPM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vtpm">The opaque_ref of the given vtpm</param>
        public static XenRef<VM> get_backend(Session session, string _vtpm)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vtpm_get_backend(session.opaque_ref, _vtpm);
            else
                return XenRef<VM>.Create(session.XmlRpcProxy.vtpm_get_backend(session.opaque_ref, _vtpm ?? "").parse());
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
        /// the virtual machine
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VM>))]
        public virtual XenRef<VM> VM
        {
            get { return _VM; }
            set
            {
                if (!Helper.AreEqual(value, _VM))
                {
                    _VM = value;
                    NotifyPropertyChanged("VM");
                }
            }
        }
        private XenRef<VM> _VM = new XenRef<VM>(Helper.NullOpaqueRef);

        /// <summary>
        /// the domain where the backend is located
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VM>))]
        public virtual XenRef<VM> backend
        {
            get { return _backend; }
            set
            {
                if (!Helper.AreEqual(value, _backend))
                {
                    _backend = value;
                    NotifyPropertyChanged("backend");
                }
            }
        }
        private XenRef<VM> _backend = new XenRef<VM>(Helper.NullOpaqueRef);
    }
}
