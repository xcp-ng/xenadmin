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
    /// A virtual GPU (vGPU)
    /// First published in XenServer 6.0.
    /// </summary>
    public partial class VGPU : XenObject<VGPU>
    {
        #region Constructors

        public VGPU()
        {
        }

        public VGPU(string uuid,
            XenRef<VM> VM,
            XenRef<GPU_group> GPU_group,
            string device,
            bool currently_attached,
            Dictionary<string, string> other_config,
            XenRef<VGPU_type> type,
            XenRef<PGPU> resident_on,
            XenRef<PGPU> scheduled_to_be_resident_on,
            Dictionary<string, string> compatibility_metadata,
            string extra_args,
            XenRef<PCI> PCI)
        {
            this.uuid = uuid;
            this.VM = VM;
            this.GPU_group = GPU_group;
            this.device = device;
            this.currently_attached = currently_attached;
            this.other_config = other_config;
            this.type = type;
            this.resident_on = resident_on;
            this.scheduled_to_be_resident_on = scheduled_to_be_resident_on;
            this.compatibility_metadata = compatibility_metadata;
            this.extra_args = extra_args;
            this.PCI = PCI;
        }

        /// <summary>
        /// Creates a new VGPU from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VGPU(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Creates a new VGPU from a Proxy_VGPU.
        /// </summary>
        /// <param name="proxy"></param>
        public VGPU(Proxy_VGPU proxy)
        {
            UpdateFrom(proxy);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VGPU.
        /// </summary>
        public override void UpdateFrom(VGPU update)
        {
            uuid = update.uuid;
            VM = update.VM;
            GPU_group = update.GPU_group;
            device = update.device;
            currently_attached = update.currently_attached;
            other_config = update.other_config;
            type = update.type;
            resident_on = update.resident_on;
            scheduled_to_be_resident_on = update.scheduled_to_be_resident_on;
            compatibility_metadata = update.compatibility_metadata;
            extra_args = update.extra_args;
            PCI = update.PCI;
        }

        internal void UpdateFrom(Proxy_VGPU proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            VM = proxy.VM == null ? null : XenRef<VM>.Create(proxy.VM);
            GPU_group = proxy.GPU_group == null ? null : XenRef<GPU_group>.Create(proxy.GPU_group);
            device = proxy.device == null ? null : proxy.device;
            currently_attached = (bool)proxy.currently_attached;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            type = proxy.type == null ? null : XenRef<VGPU_type>.Create(proxy.type);
            resident_on = proxy.resident_on == null ? null : XenRef<PGPU>.Create(proxy.resident_on);
            scheduled_to_be_resident_on = proxy.scheduled_to_be_resident_on == null ? null : XenRef<PGPU>.Create(proxy.scheduled_to_be_resident_on);
            compatibility_metadata = proxy.compatibility_metadata == null ? null : Maps.convert_from_proxy_string_string(proxy.compatibility_metadata);
            extra_args = proxy.extra_args == null ? null : proxy.extra_args;
            PCI = proxy.PCI == null ? null : XenRef<PCI>.Create(proxy.PCI);
        }

        public Proxy_VGPU ToProxy()
        {
            Proxy_VGPU result_ = new Proxy_VGPU();
            result_.uuid = uuid ?? "";
            result_.VM = VM ?? "";
            result_.GPU_group = GPU_group ?? "";
            result_.device = device ?? "";
            result_.currently_attached = currently_attached;
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.type = type ?? "";
            result_.resident_on = resident_on ?? "";
            result_.scheduled_to_be_resident_on = scheduled_to_be_resident_on ?? "";
            result_.compatibility_metadata = Maps.convert_to_proxy_string_string(compatibility_metadata);
            result_.extra_args = extra_args ?? "";
            result_.PCI = PCI ?? "";
            return result_;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this VGPU
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
            if (table.ContainsKey("GPU_group"))
                GPU_group = Marshalling.ParseRef<GPU_group>(table, "GPU_group");
            if (table.ContainsKey("device"))
                device = Marshalling.ParseString(table, "device");
            if (table.ContainsKey("currently_attached"))
                currently_attached = Marshalling.ParseBool(table, "currently_attached");
            if (table.ContainsKey("other_config"))
                other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("type"))
                type = Marshalling.ParseRef<VGPU_type>(table, "type");
            if (table.ContainsKey("resident_on"))
                resident_on = Marshalling.ParseRef<PGPU>(table, "resident_on");
            if (table.ContainsKey("scheduled_to_be_resident_on"))
                scheduled_to_be_resident_on = Marshalling.ParseRef<PGPU>(table, "scheduled_to_be_resident_on");
            if (table.ContainsKey("compatibility_metadata"))
                compatibility_metadata = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "compatibility_metadata"));
            if (table.ContainsKey("extra_args"))
                extra_args = Marshalling.ParseString(table, "extra_args");
            if (table.ContainsKey("PCI"))
                PCI = Marshalling.ParseRef<PCI>(table, "PCI");
        }

        public bool DeepEquals(VGPU other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._VM, other._VM) &&
                Helper.AreEqual2(this._GPU_group, other._GPU_group) &&
                Helper.AreEqual2(this._device, other._device) &&
                Helper.AreEqual2(this._currently_attached, other._currently_attached) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._type, other._type) &&
                Helper.AreEqual2(this._resident_on, other._resident_on) &&
                Helper.AreEqual2(this._scheduled_to_be_resident_on, other._scheduled_to_be_resident_on) &&
                Helper.AreEqual2(this._compatibility_metadata, other._compatibility_metadata) &&
                Helper.AreEqual2(this._extra_args, other._extra_args) &&
                Helper.AreEqual2(this._PCI, other._PCI);
        }

        internal static List<VGPU> ProxyArrayToObjectList(Proxy_VGPU[] input)
        {
            var result = new List<VGPU>();
            foreach (var item in input)
                result.Add(new VGPU(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, VGPU server)
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
                    VGPU.set_other_config(session, opaqueRef, _other_config);
                }
                if (!Helper.AreEqual2(_extra_args, server._extra_args))
                {
                    VGPU.set_extra_args(session, opaqueRef, _extra_args);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        public static VGPU get_record(Session session, string _vgpu)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_get_record(session.opaque_ref, _vgpu);
            else
                return new VGPU(session.proxy.vgpu_get_record(session.opaque_ref, _vgpu ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the VGPU instance with the specified UUID.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<VGPU> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<VGPU>.Create(session.proxy.vgpu_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        public static string get_uuid(Session session, string _vgpu)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_get_uuid(session.opaque_ref, _vgpu);
            else
                return session.proxy.vgpu_get_uuid(session.opaque_ref, _vgpu ?? "").parse();
        }

        /// <summary>
        /// Get the VM field of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        public static XenRef<VM> get_VM(Session session, string _vgpu)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_get_vm(session.opaque_ref, _vgpu);
            else
                return XenRef<VM>.Create(session.proxy.vgpu_get_vm(session.opaque_ref, _vgpu ?? "").parse());
        }

        /// <summary>
        /// Get the GPU_group field of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        public static XenRef<GPU_group> get_GPU_group(Session session, string _vgpu)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_get_gpu_group(session.opaque_ref, _vgpu);
            else
                return XenRef<GPU_group>.Create(session.proxy.vgpu_get_gpu_group(session.opaque_ref, _vgpu ?? "").parse());
        }

        /// <summary>
        /// Get the device field of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        public static string get_device(Session session, string _vgpu)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_get_device(session.opaque_ref, _vgpu);
            else
                return session.proxy.vgpu_get_device(session.opaque_ref, _vgpu ?? "").parse();
        }

        /// <summary>
        /// Get the currently_attached field of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        public static bool get_currently_attached(Session session, string _vgpu)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_get_currently_attached(session.opaque_ref, _vgpu);
            else
                return (bool)session.proxy.vgpu_get_currently_attached(session.opaque_ref, _vgpu ?? "").parse();
        }

        /// <summary>
        /// Get the other_config field of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        public static Dictionary<string, string> get_other_config(Session session, string _vgpu)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_get_other_config(session.opaque_ref, _vgpu);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vgpu_get_other_config(session.opaque_ref, _vgpu ?? "").parse());
        }

        /// <summary>
        /// Get the type field of the given VGPU.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        public static XenRef<VGPU_type> get_type(Session session, string _vgpu)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_get_type(session.opaque_ref, _vgpu);
            else
                return XenRef<VGPU_type>.Create(session.proxy.vgpu_get_type(session.opaque_ref, _vgpu ?? "").parse());
        }

        /// <summary>
        /// Get the resident_on field of the given VGPU.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        public static XenRef<PGPU> get_resident_on(Session session, string _vgpu)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_get_resident_on(session.opaque_ref, _vgpu);
            else
                return XenRef<PGPU>.Create(session.proxy.vgpu_get_resident_on(session.opaque_ref, _vgpu ?? "").parse());
        }

        /// <summary>
        /// Get the scheduled_to_be_resident_on field of the given VGPU.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        public static XenRef<PGPU> get_scheduled_to_be_resident_on(Session session, string _vgpu)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_get_scheduled_to_be_resident_on(session.opaque_ref, _vgpu);
            else
                return XenRef<PGPU>.Create(session.proxy.vgpu_get_scheduled_to_be_resident_on(session.opaque_ref, _vgpu ?? "").parse());
        }

        /// <summary>
        /// Get the compatibility_metadata field of the given VGPU.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        public static Dictionary<string, string> get_compatibility_metadata(Session session, string _vgpu)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_get_compatibility_metadata(session.opaque_ref, _vgpu);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vgpu_get_compatibility_metadata(session.opaque_ref, _vgpu ?? "").parse());
        }

        /// <summary>
        /// Get the extra_args field of the given VGPU.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        public static string get_extra_args(Session session, string _vgpu)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_get_extra_args(session.opaque_ref, _vgpu);
            else
                return session.proxy.vgpu_get_extra_args(session.opaque_ref, _vgpu ?? "").parse();
        }

        /// <summary>
        /// Get the PCI field of the given VGPU.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        public static XenRef<PCI> get_PCI(Session session, string _vgpu)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_get_pci(session.opaque_ref, _vgpu);
            else
                return XenRef<PCI>.Create(session.proxy.vgpu_get_pci(session.opaque_ref, _vgpu ?? "").parse());
        }

        /// <summary>
        /// Set the other_config field of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _vgpu, Dictionary<string, string> _other_config)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vgpu_set_other_config(session.opaque_ref, _vgpu, _other_config);
            else
                session.proxy.vgpu_set_other_config(session.opaque_ref, _vgpu ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _vgpu, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vgpu_add_to_other_config(session.opaque_ref, _vgpu, _key, _value);
            else
                session.proxy.vgpu_add_to_other_config(session.opaque_ref, _vgpu ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given VGPU.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _vgpu, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vgpu_remove_from_other_config(session.opaque_ref, _vgpu, _key);
            else
                session.proxy.vgpu_remove_from_other_config(session.opaque_ref, _vgpu ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// Set the extra_args field of the given VGPU.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <param name="_extra_args">New value to set</param>
        public static void set_extra_args(Session session, string _vgpu, string _extra_args)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vgpu_set_extra_args(session.opaque_ref, _vgpu, _extra_args);
            else
                session.proxy.vgpu_set_extra_args(session.opaque_ref, _vgpu ?? "", _extra_args ?? "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm"></param>
        /// <param name="_gpu_group"></param>
        /// <param name="_device"></param>
        /// <param name="_other_config"></param>
        public static XenRef<VGPU> create(Session session, string _vm, string _gpu_group, string _device, Dictionary<string, string> _other_config)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_create(session.opaque_ref, _vm, _gpu_group, _device, _other_config);
            else
                return XenRef<VGPU>.Create(session.proxy.vgpu_create(session.opaque_ref, _vm ?? "", _gpu_group ?? "", _device ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm"></param>
        /// <param name="_gpu_group"></param>
        /// <param name="_device"></param>
        /// <param name="_other_config"></param>
        public static XenRef<Task> async_create(Session session, string _vm, string _gpu_group, string _device, Dictionary<string, string> _other_config)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vgpu_create(session.opaque_ref, _vm, _gpu_group, _device, _other_config);
          else
              return XenRef<Task>.Create(session.proxy.async_vgpu_create(session.opaque_ref, _vm ?? "", _gpu_group ?? "", _device ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm"></param>
        /// <param name="_gpu_group"></param>
        /// <param name="_device"></param>
        /// <param name="_other_config"></param>
        /// <param name="_type"> First published in XenServer 6.2 SP1 Tech-Preview.</param>
        public static XenRef<VGPU> create(Session session, string _vm, string _gpu_group, string _device, Dictionary<string, string> _other_config, string _type)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_create(session.opaque_ref, _vm, _gpu_group, _device, _other_config, _type);
            else
                return XenRef<VGPU>.Create(session.proxy.vgpu_create(session.opaque_ref, _vm ?? "", _gpu_group ?? "", _device ?? "", Maps.convert_to_proxy_string_string(_other_config), _type ?? "").parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm"></param>
        /// <param name="_gpu_group"></param>
        /// <param name="_device"></param>
        /// <param name="_other_config"></param>
        /// <param name="_type"> First published in XenServer 6.2 SP1 Tech-Preview.</param>
        public static XenRef<Task> async_create(Session session, string _vm, string _gpu_group, string _device, Dictionary<string, string> _other_config, string _type)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vgpu_create(session.opaque_ref, _vm, _gpu_group, _device, _other_config, _type);
          else
              return XenRef<Task>.Create(session.proxy.async_vgpu_create(session.opaque_ref, _vm ?? "", _gpu_group ?? "", _device ?? "", Maps.convert_to_proxy_string_string(_other_config), _type ?? "").parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        public static void destroy(Session session, string _vgpu)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vgpu_destroy(session.opaque_ref, _vgpu);
            else
                session.proxy.vgpu_destroy(session.opaque_ref, _vgpu ?? "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        public static XenRef<Task> async_destroy(Session session, string _vgpu)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vgpu_destroy(session.opaque_ref, _vgpu);
          else
              return XenRef<Task>.Create(session.proxy.async_vgpu_destroy(session.opaque_ref, _vgpu ?? "").parse());
        }

        /// <summary>
        /// Return a list of all the VGPUs known to the system.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<VGPU>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_get_all(session.opaque_ref);
            else
                return XenRef<VGPU>.Create(session.proxy.vgpu_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the VGPU Records at once, in a single XML RPC call
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<VGPU>, VGPU> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vgpu_get_all_records(session.opaque_ref);
            else
                return XenRef<VGPU>.Create<Proxy_VGPU>(session.proxy.vgpu_get_all_records(session.opaque_ref).parse());
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
        /// VM that owns the vGPU
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
                    Changed = true;
                    NotifyPropertyChanged("VM");
                }
            }
        }
        private XenRef<VM> _VM = new XenRef<VM>(Helper.NullOpaqueRef);

        /// <summary>
        /// GPU group used by the vGPU
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<GPU_group>))]
        public virtual XenRef<GPU_group> GPU_group
        {
            get { return _GPU_group; }
            set
            {
                if (!Helper.AreEqual(value, _GPU_group))
                {
                    _GPU_group = value;
                    Changed = true;
                    NotifyPropertyChanged("GPU_group");
                }
            }
        }
        private XenRef<GPU_group> _GPU_group = new XenRef<GPU_group>(Helper.NullOpaqueRef);

        /// <summary>
        /// Order in which the devices are plugged into the VM
        /// </summary>
        public virtual string device
        {
            get { return _device; }
            set
            {
                if (!Helper.AreEqual(value, _device))
                {
                    _device = value;
                    Changed = true;
                    NotifyPropertyChanged("device");
                }
            }
        }
        private string _device = "0";

        /// <summary>
        /// Reflects whether the virtual device is currently connected to a physical device
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
        private bool _currently_attached = false;

        /// <summary>
        /// Additional configuration
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

        /// <summary>
        /// Preset type for this VGPU
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VGPU_type>))]
        public virtual XenRef<VGPU_type> type
        {
            get { return _type; }
            set
            {
                if (!Helper.AreEqual(value, _type))
                {
                    _type = value;
                    Changed = true;
                    NotifyPropertyChanged("type");
                }
            }
        }
        private XenRef<VGPU_type> _type = new XenRef<VGPU_type>("OpaqueRef:NULL");

        /// <summary>
        /// The PGPU on which this VGPU is running
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PGPU>))]
        public virtual XenRef<PGPU> resident_on
        {
            get { return _resident_on; }
            set
            {
                if (!Helper.AreEqual(value, _resident_on))
                {
                    _resident_on = value;
                    Changed = true;
                    NotifyPropertyChanged("resident_on");
                }
            }
        }
        private XenRef<PGPU> _resident_on = new XenRef<PGPU>("OpaqueRef:NULL");

        /// <summary>
        /// The PGPU on which this VGPU is scheduled to run
        /// First published in XenServer 7.0.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PGPU>))]
        public virtual XenRef<PGPU> scheduled_to_be_resident_on
        {
            get { return _scheduled_to_be_resident_on; }
            set
            {
                if (!Helper.AreEqual(value, _scheduled_to_be_resident_on))
                {
                    _scheduled_to_be_resident_on = value;
                    Changed = true;
                    NotifyPropertyChanged("scheduled_to_be_resident_on");
                }
            }
        }
        private XenRef<PGPU> _scheduled_to_be_resident_on = new XenRef<PGPU>("OpaqueRef:NULL");

        /// <summary>
        /// VGPU metadata to determine whether a VGPU can migrate between two PGPUs
        /// First published in XenServer 7.3.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> compatibility_metadata
        {
            get { return _compatibility_metadata; }
            set
            {
                if (!Helper.AreEqual(value, _compatibility_metadata))
                {
                    _compatibility_metadata = value;
                    Changed = true;
                    NotifyPropertyChanged("compatibility_metadata");
                }
            }
        }
        private Dictionary<string, string> _compatibility_metadata = new Dictionary<string, string>() {};

        /// <summary>
        /// Extra arguments for vGPU and passed to demu
        /// First published in Unreleased.
        /// </summary>
        public virtual string extra_args
        {
            get { return _extra_args; }
            set
            {
                if (!Helper.AreEqual(value, _extra_args))
                {
                    _extra_args = value;
                    Changed = true;
                    NotifyPropertyChanged("extra_args");
                }
            }
        }
        private string _extra_args = "";

        /// <summary>
        /// Device passed trough to VM, either as full device or SR-IOV virtual function
        /// First published in Unreleased.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PCI>))]
        public virtual XenRef<PCI> PCI
        {
            get { return _PCI; }
            set
            {
                if (!Helper.AreEqual(value, _PCI))
                {
                    _PCI = value;
                    Changed = true;
                    NotifyPropertyChanged("PCI");
                }
            }
        }
        private XenRef<PCI> _PCI = new XenRef<PCI>("OpaqueRef:NULL");
    }
}
