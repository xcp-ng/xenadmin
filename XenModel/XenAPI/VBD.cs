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
    /// A virtual block device
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class VBD : XenObject<VBD>
    {
        public VBD()
        {
        }

        public VBD(string uuid,
            List<vbd_operations> allowed_operations,
            Dictionary<string, vbd_operations> current_operations,
            XenRef<VM> VM,
            XenRef<VDI> VDI,
            string device,
            string userdevice,
            bool bootable,
            vbd_mode mode,
            vbd_type type,
            bool unpluggable,
            bool storage_lock,
            bool empty,
            Dictionary<string, string> other_config,
            bool currently_attached,
            long status_code,
            string status_detail,
            Dictionary<string, string> runtime_properties,
            string qos_algorithm_type,
            Dictionary<string, string> qos_algorithm_params,
            string[] qos_supported_algorithms,
            XenRef<VBD_metrics> metrics)
        {
            this.uuid = uuid;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.VM = VM;
            this.VDI = VDI;
            this.device = device;
            this.userdevice = userdevice;
            this.bootable = bootable;
            this.mode = mode;
            this.type = type;
            this.unpluggable = unpluggable;
            this.storage_lock = storage_lock;
            this.empty = empty;
            this.other_config = other_config;
            this.currently_attached = currently_attached;
            this.status_code = status_code;
            this.status_detail = status_detail;
            this.runtime_properties = runtime_properties;
            this.qos_algorithm_type = qos_algorithm_type;
            this.qos_algorithm_params = qos_algorithm_params;
            this.qos_supported_algorithms = qos_supported_algorithms;
            this.metrics = metrics;
        }

        /// <summary>
        /// Creates a new VBD from a Proxy_VBD.
        /// </summary>
        /// <param name="proxy"></param>
        public VBD(Proxy_VBD proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VBD.
        /// </summary>
        public override void UpdateFrom(VBD update)
        {
            uuid = update.uuid;
            allowed_operations = update.allowed_operations;
            current_operations = update.current_operations;
            VM = update.VM;
            VDI = update.VDI;
            device = update.device;
            userdevice = update.userdevice;
            bootable = update.bootable;
            mode = update.mode;
            type = update.type;
            unpluggable = update.unpluggable;
            storage_lock = update.storage_lock;
            empty = update.empty;
            other_config = update.other_config;
            currently_attached = update.currently_attached;
            status_code = update.status_code;
            status_detail = update.status_detail;
            runtime_properties = update.runtime_properties;
            qos_algorithm_type = update.qos_algorithm_type;
            qos_algorithm_params = update.qos_algorithm_params;
            qos_supported_algorithms = update.qos_supported_algorithms;
            metrics = update.metrics;
        }

        internal void UpdateFromProxy(Proxy_VBD proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            allowed_operations = proxy.allowed_operations == null ? null : Helper.StringArrayToEnumList<vbd_operations>(proxy.allowed_operations);
            current_operations = proxy.current_operations == null ? null : Maps.convert_from_proxy_string_vbd_operations(proxy.current_operations);
            VM = proxy.VM == null ? null : XenRef<VM>.Create(proxy.VM);
            VDI = proxy.VDI == null ? null : XenRef<VDI>.Create(proxy.VDI);
            device = proxy.device == null ? null : proxy.device;
            userdevice = proxy.userdevice == null ? null : proxy.userdevice;
            bootable = (bool)proxy.bootable;
            mode = proxy.mode == null ? (vbd_mode) 0 : (vbd_mode)Helper.EnumParseDefault(typeof(vbd_mode), (string)proxy.mode);
            type = proxy.type == null ? (vbd_type) 0 : (vbd_type)Helper.EnumParseDefault(typeof(vbd_type), (string)proxy.type);
            unpluggable = (bool)proxy.unpluggable;
            storage_lock = (bool)proxy.storage_lock;
            empty = (bool)proxy.empty;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            currently_attached = (bool)proxy.currently_attached;
            status_code = proxy.status_code == null ? 0 : long.Parse(proxy.status_code);
            status_detail = proxy.status_detail == null ? null : proxy.status_detail;
            runtime_properties = proxy.runtime_properties == null ? null : Maps.convert_from_proxy_string_string(proxy.runtime_properties);
            qos_algorithm_type = proxy.qos_algorithm_type == null ? null : proxy.qos_algorithm_type;
            qos_algorithm_params = proxy.qos_algorithm_params == null ? null : Maps.convert_from_proxy_string_string(proxy.qos_algorithm_params);
            qos_supported_algorithms = proxy.qos_supported_algorithms == null ? new string[] {} : (string [])proxy.qos_supported_algorithms;
            metrics = proxy.metrics == null ? null : XenRef<VBD_metrics>.Create(proxy.metrics);
        }

        public Proxy_VBD ToProxy()
        {
            Proxy_VBD result_ = new Proxy_VBD();
            result_.uuid = uuid ?? "";
            result_.allowed_operations = allowed_operations == null ? new string[] {} : Helper.ObjectListToStringArray(allowed_operations);
            result_.current_operations = Maps.convert_to_proxy_string_vbd_operations(current_operations);
            result_.VM = VM ?? "";
            result_.VDI = VDI ?? "";
            result_.device = device ?? "";
            result_.userdevice = userdevice ?? "";
            result_.bootable = bootable;
            result_.mode = vbd_mode_helper.ToString(mode);
            result_.type = vbd_type_helper.ToString(type);
            result_.unpluggable = unpluggable;
            result_.storage_lock = storage_lock;
            result_.empty = empty;
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.currently_attached = currently_attached;
            result_.status_code = status_code.ToString();
            result_.status_detail = status_detail ?? "";
            result_.runtime_properties = Maps.convert_to_proxy_string_string(runtime_properties);
            result_.qos_algorithm_type = qos_algorithm_type ?? "";
            result_.qos_algorithm_params = Maps.convert_to_proxy_string_string(qos_algorithm_params);
            result_.qos_supported_algorithms = qos_supported_algorithms;
            result_.metrics = metrics ?? "";
            return result_;
        }

        /// <summary>
        /// Creates a new VBD from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VBD(Hashtable table) : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this VBD
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("allowed_operations"))
                allowed_operations = Helper.StringArrayToEnumList<vbd_operations>(Marshalling.ParseStringArray(table, "allowed_operations"));
            if (table.ContainsKey("current_operations"))
                current_operations = Maps.convert_from_proxy_string_vbd_operations(Marshalling.ParseHashTable(table, "current_operations"));
            if (table.ContainsKey("VM"))
                VM = Marshalling.ParseRef<VM>(table, "VM");
            if (table.ContainsKey("VDI"))
                VDI = Marshalling.ParseRef<VDI>(table, "VDI");
            if (table.ContainsKey("device"))
                device = Marshalling.ParseString(table, "device");
            if (table.ContainsKey("userdevice"))
                userdevice = Marshalling.ParseString(table, "userdevice");
            if (table.ContainsKey("bootable"))
                bootable = Marshalling.ParseBool(table, "bootable");
            if (table.ContainsKey("mode"))
                mode = (vbd_mode)Helper.EnumParseDefault(typeof(vbd_mode), Marshalling.ParseString(table, "mode"));
            if (table.ContainsKey("type"))
                type = (vbd_type)Helper.EnumParseDefault(typeof(vbd_type), Marshalling.ParseString(table, "type"));
            if (table.ContainsKey("unpluggable"))
                unpluggable = Marshalling.ParseBool(table, "unpluggable");
            if (table.ContainsKey("storage_lock"))
                storage_lock = Marshalling.ParseBool(table, "storage_lock");
            if (table.ContainsKey("empty"))
                empty = Marshalling.ParseBool(table, "empty");
            if (table.ContainsKey("other_config"))
                other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("currently_attached"))
                currently_attached = Marshalling.ParseBool(table, "currently_attached");
            if (table.ContainsKey("status_code"))
                status_code = Marshalling.ParseLong(table, "status_code");
            if (table.ContainsKey("status_detail"))
                status_detail = Marshalling.ParseString(table, "status_detail");
            if (table.ContainsKey("runtime_properties"))
                runtime_properties = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "runtime_properties"));
            if (table.ContainsKey("qos_algorithm_type"))
                qos_algorithm_type = Marshalling.ParseString(table, "qos_algorithm_type");
            if (table.ContainsKey("qos_algorithm_params"))
                qos_algorithm_params = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "qos_algorithm_params"));
            if (table.ContainsKey("qos_supported_algorithms"))
                qos_supported_algorithms = Marshalling.ParseStringArray(table, "qos_supported_algorithms");
            if (table.ContainsKey("metrics"))
                metrics = Marshalling.ParseRef<VBD_metrics>(table, "metrics");
        }

        public bool DeepEquals(VBD other, bool ignoreCurrentOperations)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (!ignoreCurrentOperations && !Helper.AreEqual2(this.current_operations, other.current_operations))
                return false;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(this._VM, other._VM) &&
                Helper.AreEqual2(this._VDI, other._VDI) &&
                Helper.AreEqual2(this._device, other._device) &&
                Helper.AreEqual2(this._userdevice, other._userdevice) &&
                Helper.AreEqual2(this._bootable, other._bootable) &&
                Helper.AreEqual2(this._mode, other._mode) &&
                Helper.AreEqual2(this._type, other._type) &&
                Helper.AreEqual2(this._unpluggable, other._unpluggable) &&
                Helper.AreEqual2(this._storage_lock, other._storage_lock) &&
                Helper.AreEqual2(this._empty, other._empty) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._currently_attached, other._currently_attached) &&
                Helper.AreEqual2(this._status_code, other._status_code) &&
                Helper.AreEqual2(this._status_detail, other._status_detail) &&
                Helper.AreEqual2(this._runtime_properties, other._runtime_properties) &&
                Helper.AreEqual2(this._qos_algorithm_type, other._qos_algorithm_type) &&
                Helper.AreEqual2(this._qos_algorithm_params, other._qos_algorithm_params) &&
                Helper.AreEqual2(this._qos_supported_algorithms, other._qos_supported_algorithms) &&
                Helper.AreEqual2(this._metrics, other._metrics);
        }

        internal static List<VBD> ProxyArrayToObjectList(Proxy_VBD[] input)
        {
            var result = new List<VBD>();
            foreach (var item in input)
                result.Add(new VBD(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, VBD server)
        {
            if (opaqueRef == null)
            {
                var reference = create(session, this);
                return reference == null ? null : reference.opaque_ref;
            }
            else
            {
                if (!Helper.AreEqual2(_userdevice, server._userdevice))
                {
                    VBD.set_userdevice(session, opaqueRef, _userdevice);
                }
                if (!Helper.AreEqual2(_bootable, server._bootable))
                {
                    VBD.set_bootable(session, opaqueRef, _bootable);
                }
                if (!Helper.AreEqual2(_type, server._type))
                {
                    VBD.set_type(session, opaqueRef, _type);
                }
                if (!Helper.AreEqual2(_unpluggable, server._unpluggable))
                {
                    VBD.set_unpluggable(session, opaqueRef, _unpluggable);
                }
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    VBD.set_other_config(session, opaqueRef, _other_config);
                }
                if (!Helper.AreEqual2(_qos_algorithm_type, server._qos_algorithm_type))
                {
                    VBD.set_qos_algorithm_type(session, opaqueRef, _qos_algorithm_type);
                }
                if (!Helper.AreEqual2(_qos_algorithm_params, server._qos_algorithm_params))
                {
                    VBD.set_qos_algorithm_params(session, opaqueRef, _qos_algorithm_params);
                }
                if (!Helper.AreEqual2(_mode, server._mode))
                {
                    VBD.set_mode(session, opaqueRef, _mode);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static VBD get_record(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_record(session.opaque_ref, _vbd);
            else
                return new VBD((Proxy_VBD)session.proxy.vbd_get_record(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the VBD instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<VBD> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<VBD>.Create(session.proxy.vbd_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Create a new VBD instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<VBD> create(Session session, VBD _record)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_create(session.opaque_ref, _record);
            else
                return XenRef<VBD>.Create(session.proxy.vbd_create(session.opaque_ref, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Create a new VBD instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<Task> async_create(Session session, VBD _record)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vbd_create(session.opaque_ref, _record);
          else
              return XenRef<Task>.Create(session.proxy.async_vbd_create(session.opaque_ref, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Destroy the specified VBD instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static void destroy(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_destroy(session.opaque_ref, _vbd);
            else
                session.proxy.vbd_destroy(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Destroy the specified VBD instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static XenRef<Task> async_destroy(Session session, string _vbd)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vbd_destroy(session.opaque_ref, _vbd);
          else
              return XenRef<Task>.Create(session.proxy.async_vbd_destroy(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static string get_uuid(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_uuid(session.opaque_ref, _vbd);
            else
                return session.proxy.vbd_get_uuid(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Get the allowed_operations field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static List<vbd_operations> get_allowed_operations(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_allowed_operations(session.opaque_ref, _vbd);
            else
                return Helper.StringArrayToEnumList<vbd_operations>(session.proxy.vbd_get_allowed_operations(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Get the current_operations field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static Dictionary<string, vbd_operations> get_current_operations(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_current_operations(session.opaque_ref, _vbd);
            else
                return Maps.convert_from_proxy_string_vbd_operations(session.proxy.vbd_get_current_operations(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Get the VM field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static XenRef<VM> get_VM(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_vm(session.opaque_ref, _vbd);
            else
                return XenRef<VM>.Create(session.proxy.vbd_get_vm(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Get the VDI field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static XenRef<VDI> get_VDI(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_vdi(session.opaque_ref, _vbd);
            else
                return XenRef<VDI>.Create(session.proxy.vbd_get_vdi(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Get the device field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static string get_device(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_device(session.opaque_ref, _vbd);
            else
                return session.proxy.vbd_get_device(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Get the userdevice field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static string get_userdevice(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_userdevice(session.opaque_ref, _vbd);
            else
                return session.proxy.vbd_get_userdevice(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Get the bootable field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static bool get_bootable(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_bootable(session.opaque_ref, _vbd);
            else
                return (bool)session.proxy.vbd_get_bootable(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Get the mode field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static vbd_mode get_mode(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_mode(session.opaque_ref, _vbd);
            else
                return (vbd_mode)Helper.EnumParseDefault(typeof(vbd_mode), (string)session.proxy.vbd_get_mode(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Get the type field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static vbd_type get_type(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_type(session.opaque_ref, _vbd);
            else
                return (vbd_type)Helper.EnumParseDefault(typeof(vbd_type), (string)session.proxy.vbd_get_type(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Get the unpluggable field of the given VBD.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static bool get_unpluggable(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_unpluggable(session.opaque_ref, _vbd);
            else
                return (bool)session.proxy.vbd_get_unpluggable(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Get the storage_lock field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static bool get_storage_lock(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_storage_lock(session.opaque_ref, _vbd);
            else
                return (bool)session.proxy.vbd_get_storage_lock(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Get the empty field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static bool get_empty(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_empty(session.opaque_ref, _vbd);
            else
                return (bool)session.proxy.vbd_get_empty(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Get the other_config field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static Dictionary<string, string> get_other_config(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_other_config(session.opaque_ref, _vbd);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vbd_get_other_config(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Get the currently_attached field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static bool get_currently_attached(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_currently_attached(session.opaque_ref, _vbd);
            else
                return (bool)session.proxy.vbd_get_currently_attached(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Get the status_code field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static long get_status_code(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_status_code(session.opaque_ref, _vbd);
            else
                return long.Parse(session.proxy.vbd_get_status_code(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Get the status_detail field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static string get_status_detail(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_status_detail(session.opaque_ref, _vbd);
            else
                return session.proxy.vbd_get_status_detail(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Get the runtime_properties field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static Dictionary<string, string> get_runtime_properties(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_runtime_properties(session.opaque_ref, _vbd);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vbd_get_runtime_properties(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Get the qos/algorithm_type field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static string get_qos_algorithm_type(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_qos_algorithm_type(session.opaque_ref, _vbd);
            else
                return session.proxy.vbd_get_qos_algorithm_type(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Get the qos/algorithm_params field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static Dictionary<string, string> get_qos_algorithm_params(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_qos_algorithm_params(session.opaque_ref, _vbd);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vbd_get_qos_algorithm_params(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Get the qos/supported_algorithms field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static string[] get_qos_supported_algorithms(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_qos_supported_algorithms(session.opaque_ref, _vbd);
            else
                return (string [])session.proxy.vbd_get_qos_supported_algorithms(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Get the metrics field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static XenRef<VBD_metrics> get_metrics(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_metrics(session.opaque_ref, _vbd);
            else
                return XenRef<VBD_metrics>.Create(session.proxy.vbd_get_metrics(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Set the userdevice field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_userdevice">New value to set</param>
        public static void set_userdevice(Session session, string _vbd, string _userdevice)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_set_userdevice(session.opaque_ref, _vbd, _userdevice);
            else
                session.proxy.vbd_set_userdevice(session.opaque_ref, _vbd ?? "", _userdevice ?? "").parse();
        }

        /// <summary>
        /// Set the bootable field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_bootable">New value to set</param>
        public static void set_bootable(Session session, string _vbd, bool _bootable)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_set_bootable(session.opaque_ref, _vbd, _bootable);
            else
                session.proxy.vbd_set_bootable(session.opaque_ref, _vbd ?? "", _bootable).parse();
        }

        /// <summary>
        /// Set the type field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_type">New value to set</param>
        public static void set_type(Session session, string _vbd, vbd_type _type)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_set_type(session.opaque_ref, _vbd, _type);
            else
                session.proxy.vbd_set_type(session.opaque_ref, _vbd ?? "", vbd_type_helper.ToString(_type)).parse();
        }

        /// <summary>
        /// Set the unpluggable field of the given VBD.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_unpluggable">New value to set</param>
        public static void set_unpluggable(Session session, string _vbd, bool _unpluggable)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_set_unpluggable(session.opaque_ref, _vbd, _unpluggable);
            else
                session.proxy.vbd_set_unpluggable(session.opaque_ref, _vbd ?? "", _unpluggable).parse();
        }

        /// <summary>
        /// Set the other_config field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _vbd, Dictionary<string, string> _other_config)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_set_other_config(session.opaque_ref, _vbd, _other_config);
            else
                session.proxy.vbd_set_other_config(session.opaque_ref, _vbd ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _vbd, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_add_to_other_config(session.opaque_ref, _vbd, _key, _value);
            else
                session.proxy.vbd_add_to_other_config(session.opaque_ref, _vbd ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given VBD.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _vbd, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_remove_from_other_config(session.opaque_ref, _vbd, _key);
            else
                session.proxy.vbd_remove_from_other_config(session.opaque_ref, _vbd ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// Set the qos/algorithm_type field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_algorithm_type">New value to set</param>
        public static void set_qos_algorithm_type(Session session, string _vbd, string _algorithm_type)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_set_qos_algorithm_type(session.opaque_ref, _vbd, _algorithm_type);
            else
                session.proxy.vbd_set_qos_algorithm_type(session.opaque_ref, _vbd ?? "", _algorithm_type ?? "").parse();
        }

        /// <summary>
        /// Set the qos/algorithm_params field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_algorithm_params">New value to set</param>
        public static void set_qos_algorithm_params(Session session, string _vbd, Dictionary<string, string> _algorithm_params)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_set_qos_algorithm_params(session.opaque_ref, _vbd, _algorithm_params);
            else
                session.proxy.vbd_set_qos_algorithm_params(session.opaque_ref, _vbd ?? "", Maps.convert_to_proxy_string_string(_algorithm_params)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the qos/algorithm_params field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_qos_algorithm_params(Session session, string _vbd, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_add_to_qos_algorithm_params(session.opaque_ref, _vbd, _key, _value);
            else
                session.proxy.vbd_add_to_qos_algorithm_params(session.opaque_ref, _vbd ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the qos/algorithm_params field of the given VBD.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_qos_algorithm_params(Session session, string _vbd, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_remove_from_qos_algorithm_params(session.opaque_ref, _vbd, _key);
            else
                session.proxy.vbd_remove_from_qos_algorithm_params(session.opaque_ref, _vbd ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// Remove the media from the device and leave it empty
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static void eject(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_eject(session.opaque_ref, _vbd);
            else
                session.proxy.vbd_eject(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Remove the media from the device and leave it empty
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static XenRef<Task> async_eject(Session session, string _vbd)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vbd_eject(session.opaque_ref, _vbd);
          else
              return XenRef<Task>.Create(session.proxy.async_vbd_eject(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Insert new media into the device
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_vdi">The new VDI to 'insert'</param>
        public static void insert(Session session, string _vbd, string _vdi)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_insert(session.opaque_ref, _vbd, _vdi);
            else
                session.proxy.vbd_insert(session.opaque_ref, _vbd ?? "", _vdi ?? "").parse();
        }

        /// <summary>
        /// Insert new media into the device
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_vdi">The new VDI to 'insert'</param>
        public static XenRef<Task> async_insert(Session session, string _vbd, string _vdi)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vbd_insert(session.opaque_ref, _vbd, _vdi);
          else
              return XenRef<Task>.Create(session.proxy.async_vbd_insert(session.opaque_ref, _vbd ?? "", _vdi ?? "").parse());
        }

        /// <summary>
        /// Hotplug the specified VBD, dynamically attaching it to the running VM
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static void plug(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_plug(session.opaque_ref, _vbd);
            else
                session.proxy.vbd_plug(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Hotplug the specified VBD, dynamically attaching it to the running VM
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static XenRef<Task> async_plug(Session session, string _vbd)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vbd_plug(session.opaque_ref, _vbd);
          else
              return XenRef<Task>.Create(session.proxy.async_vbd_plug(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Hot-unplug the specified VBD, dynamically unattaching it from the running VM
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static void unplug(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_unplug(session.opaque_ref, _vbd);
            else
                session.proxy.vbd_unplug(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Hot-unplug the specified VBD, dynamically unattaching it from the running VM
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static XenRef<Task> async_unplug(Session session, string _vbd)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vbd_unplug(session.opaque_ref, _vbd);
          else
              return XenRef<Task>.Create(session.proxy.async_vbd_unplug(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Forcibly unplug the specified VBD
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static void unplug_force(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_unplug_force(session.opaque_ref, _vbd);
            else
                session.proxy.vbd_unplug_force(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Forcibly unplug the specified VBD
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static XenRef<Task> async_unplug_force(Session session, string _vbd)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vbd_unplug_force(session.opaque_ref, _vbd);
          else
              return XenRef<Task>.Create(session.proxy.async_vbd_unplug_force(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Throws an error if this VBD could not be attached to this VM if the VM were running. Intended for debugging.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static void assert_attachable(Session session, string _vbd)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_assert_attachable(session.opaque_ref, _vbd);
            else
                session.proxy.vbd_assert_attachable(session.opaque_ref, _vbd ?? "").parse();
        }

        /// <summary>
        /// Throws an error if this VBD could not be attached to this VM if the VM were running. Intended for debugging.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        public static XenRef<Task> async_assert_attachable(Session session, string _vbd)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vbd_assert_attachable(session.opaque_ref, _vbd);
          else
              return XenRef<Task>.Create(session.proxy.async_vbd_assert_attachable(session.opaque_ref, _vbd ?? "").parse());
        }

        /// <summary>
        /// Sets the mode of the VBD. The power_state of the VM must be halted.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_value">New value to set</param>
        public static void set_mode(Session session, string _vbd, vbd_mode _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vbd_set_mode(session.opaque_ref, _vbd, _value);
            else
                session.proxy.vbd_set_mode(session.opaque_ref, _vbd ?? "", vbd_mode_helper.ToString(_value)).parse();
        }

        /// <summary>
        /// Sets the mode of the VBD. The power_state of the VM must be halted.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_value">New value to set</param>
        public static XenRef<Task> async_set_mode(Session session, string _vbd, vbd_mode _value)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vbd_set_mode(session.opaque_ref, _vbd, _value);
          else
              return XenRef<Task>.Create(session.proxy.async_vbd_set_mode(session.opaque_ref, _vbd ?? "", vbd_mode_helper.ToString(_value)).parse());
        }

        /// <summary>
        /// Return a list of all the VBDs known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<VBD>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_all(session.opaque_ref);
            else
                return XenRef<VBD>.Create(session.proxy.vbd_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the VBD Records at once, in a single XML RPC call
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<VBD>, VBD> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vbd_get_all_records(session.opaque_ref);
            else
                return XenRef<VBD>.Create<Proxy_VBD>(session.proxy.vbd_get_all_records(session.opaque_ref).parse());
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
        /// list of the operations allowed in this state. This list is advisory only and the server state may have changed by the time this field is read by a client.
        /// </summary>
        public virtual List<vbd_operations> allowed_operations
        {
            get { return _allowed_operations; }
            set
            {
                if (!Helper.AreEqual(value, _allowed_operations))
                {
                    _allowed_operations = value;
                    Changed = true;
                    NotifyPropertyChanged("allowed_operations");
                }
            }
        }
        private List<vbd_operations> _allowed_operations = new List<vbd_operations>() {};

        /// <summary>
        /// links each of the running tasks using this object (by reference) to a current_operation enum which describes the nature of the task.
        /// </summary>
        public virtual Dictionary<string, vbd_operations> current_operations
        {
            get { return _current_operations; }
            set
            {
                if (!Helper.AreEqual(value, _current_operations))
                {
                    _current_operations = value;
                    Changed = true;
                    NotifyPropertyChanged("current_operations");
                }
            }
        }
        private Dictionary<string, vbd_operations> _current_operations = new Dictionary<string, vbd_operations>() {};

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
                    Changed = true;
                    NotifyPropertyChanged("VM");
                }
            }
        }
        private XenRef<VM> _VM = new XenRef<VM>(Helper.NullOpaqueRef);

        /// <summary>
        /// the virtual disk
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
        private XenRef<VDI> _VDI = new XenRef<VDI>(Helper.NullOpaqueRef);

        /// <summary>
        /// device seen by the guest e.g. hda1
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
        private string _device = "";

        /// <summary>
        /// user-friendly device name e.g. 0,1,2,etc.
        /// </summary>
        public virtual string userdevice
        {
            get { return _userdevice; }
            set
            {
                if (!Helper.AreEqual(value, _userdevice))
                {
                    _userdevice = value;
                    Changed = true;
                    NotifyPropertyChanged("userdevice");
                }
            }
        }
        private string _userdevice = "";

        /// <summary>
        /// true if this VBD is bootable
        /// </summary>
        public virtual bool bootable
        {
            get { return _bootable; }
            set
            {
                if (!Helper.AreEqual(value, _bootable))
                {
                    _bootable = value;
                    Changed = true;
                    NotifyPropertyChanged("bootable");
                }
            }
        }
        private bool _bootable;

        /// <summary>
        /// the mode the VBD should be mounted with
        /// </summary>
        [JsonConverter(typeof(vbd_modeConverter))]
        public virtual vbd_mode mode
        {
            get { return _mode; }
            set
            {
                if (!Helper.AreEqual(value, _mode))
                {
                    _mode = value;
                    Changed = true;
                    NotifyPropertyChanged("mode");
                }
            }
        }
        private vbd_mode _mode;

        /// <summary>
        /// how the VBD will appear to the guest (e.g. disk or CD)
        /// </summary>
        [JsonConverter(typeof(vbd_typeConverter))]
        public virtual vbd_type type
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
        private vbd_type _type;

        /// <summary>
        /// true if this VBD will support hot-unplug
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual bool unpluggable
        {
            get { return _unpluggable; }
            set
            {
                if (!Helper.AreEqual(value, _unpluggable))
                {
                    _unpluggable = value;
                    Changed = true;
                    NotifyPropertyChanged("unpluggable");
                }
            }
        }
        private bool _unpluggable = true;

        /// <summary>
        /// true if a storage level lock was acquired
        /// </summary>
        public virtual bool storage_lock
        {
            get { return _storage_lock; }
            set
            {
                if (!Helper.AreEqual(value, _storage_lock))
                {
                    _storage_lock = value;
                    Changed = true;
                    NotifyPropertyChanged("storage_lock");
                }
            }
        }
        private bool _storage_lock;

        /// <summary>
        /// if true this represents an empty drive
        /// </summary>
        public virtual bool empty
        {
            get { return _empty; }
            set
            {
                if (!Helper.AreEqual(value, _empty))
                {
                    _empty = value;
                    Changed = true;
                    NotifyPropertyChanged("empty");
                }
            }
        }
        private bool _empty;

        /// <summary>
        /// additional configuration
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
        /// is the device currently attached (erased on reboot)
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
        /// error/success code associated with last attach-operation (erased on reboot)
        /// </summary>
        public virtual long status_code
        {
            get { return _status_code; }
            set
            {
                if (!Helper.AreEqual(value, _status_code))
                {
                    _status_code = value;
                    Changed = true;
                    NotifyPropertyChanged("status_code");
                }
            }
        }
        private long _status_code;

        /// <summary>
        /// error/success information associated with last attach-operation status (erased on reboot)
        /// </summary>
        public virtual string status_detail
        {
            get { return _status_detail; }
            set
            {
                if (!Helper.AreEqual(value, _status_detail))
                {
                    _status_detail = value;
                    Changed = true;
                    NotifyPropertyChanged("status_detail");
                }
            }
        }
        private string _status_detail = "";

        /// <summary>
        /// Device runtime properties
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> runtime_properties
        {
            get { return _runtime_properties; }
            set
            {
                if (!Helper.AreEqual(value, _runtime_properties))
                {
                    _runtime_properties = value;
                    Changed = true;
                    NotifyPropertyChanged("runtime_properties");
                }
            }
        }
        private Dictionary<string, string> _runtime_properties = new Dictionary<string, string>() {};

        /// <summary>
        /// QoS algorithm to use
        /// </summary>
        public virtual string qos_algorithm_type
        {
            get { return _qos_algorithm_type; }
            set
            {
                if (!Helper.AreEqual(value, _qos_algorithm_type))
                {
                    _qos_algorithm_type = value;
                    Changed = true;
                    NotifyPropertyChanged("qos_algorithm_type");
                }
            }
        }
        private string _qos_algorithm_type = "";

        /// <summary>
        /// parameters for chosen QoS algorithm
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> qos_algorithm_params
        {
            get { return _qos_algorithm_params; }
            set
            {
                if (!Helper.AreEqual(value, _qos_algorithm_params))
                {
                    _qos_algorithm_params = value;
                    Changed = true;
                    NotifyPropertyChanged("qos_algorithm_params");
                }
            }
        }
        private Dictionary<string, string> _qos_algorithm_params = new Dictionary<string, string>() {};

        /// <summary>
        /// supported QoS algorithms for this VBD
        /// </summary>
        public virtual string[] qos_supported_algorithms
        {
            get { return _qos_supported_algorithms; }
            set
            {
                if (!Helper.AreEqual(value, _qos_supported_algorithms))
                {
                    _qos_supported_algorithms = value;
                    Changed = true;
                    NotifyPropertyChanged("qos_supported_algorithms");
                }
            }
        }
        private string[] _qos_supported_algorithms = {};

        /// <summary>
        /// metrics associated with this VBD
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VBD_metrics>))]
        public virtual XenRef<VBD_metrics> metrics
        {
            get { return _metrics; }
            set
            {
                if (!Helper.AreEqual(value, _metrics))
                {
                    _metrics = value;
                    Changed = true;
                    NotifyPropertyChanged("metrics");
                }
            }
        }
        private XenRef<VBD_metrics> _metrics = new XenRef<VBD_metrics>(Helper.NullOpaqueRef);
    }
}
