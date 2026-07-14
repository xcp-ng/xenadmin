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
    /// A virtual block device
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class VBD : XenObject<VBD>
    {
        #region Constructors

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
        /// Creates a new VBD from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VBD(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VBD.
        /// </summary>
        public override void UpdateFrom(VBD record)
        {
            uuid = record.uuid;
            allowed_operations = record.allowed_operations;
            current_operations = record.current_operations;
            VM = record.VM;
            VDI = record.VDI;
            device = record.device;
            userdevice = record.userdevice;
            bootable = record.bootable;
            mode = record.mode;
            type = record.type;
            unpluggable = record.unpluggable;
            storage_lock = record.storage_lock;
            empty = record.empty;
            other_config = record.other_config;
            currently_attached = record.currently_attached;
            status_code = record.status_code;
            status_detail = record.status_detail;
            runtime_properties = record.runtime_properties;
            qos_algorithm_type = record.qos_algorithm_type;
            qos_algorithm_params = record.qos_algorithm_params;
            qos_supported_algorithms = record.qos_supported_algorithms;
            metrics = record.metrics;
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
                current_operations = Maps.ToDictionary_string_vbd_operations(Marshalling.ParseHashTable(table, "current_operations"));
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
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("currently_attached"))
                currently_attached = Marshalling.ParseBool(table, "currently_attached");
            if (table.ContainsKey("status_code"))
                status_code = Marshalling.ParseLong(table, "status_code");
            if (table.ContainsKey("status_detail"))
                status_detail = Marshalling.ParseString(table, "status_detail");
            if (table.ContainsKey("runtime_properties"))
                runtime_properties = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "runtime_properties"));
            if (table.ContainsKey("qos_algorithm_type"))
                qos_algorithm_type = Marshalling.ParseString(table, "qos_algorithm_type");
            if (table.ContainsKey("qos_algorithm_params"))
                qos_algorithm_params = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "qos_algorithm_params"));
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

            if (!ignoreCurrentOperations && !Helper.AreEqual2(current_operations, other.current_operations))
                return false;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(_VM, other._VM) &&
                Helper.AreEqual2(_VDI, other._VDI) &&
                Helper.AreEqual2(_device, other._device) &&
                Helper.AreEqual2(_userdevice, other._userdevice) &&
                Helper.AreEqual2(_bootable, other._bootable) &&
                Helper.AreEqual2(_mode, other._mode) &&
                Helper.AreEqual2(_type, other._type) &&
                Helper.AreEqual2(_unpluggable, other._unpluggable) &&
                Helper.AreEqual2(_storage_lock, other._storage_lock) &&
                Helper.AreEqual2(_empty, other._empty) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_currently_attached, other._currently_attached) &&
                Helper.AreEqual2(_status_code, other._status_code) &&
                Helper.AreEqual2(_status_detail, other._status_detail) &&
                Helper.AreEqual2(_runtime_properties, other._runtime_properties) &&
                Helper.AreEqual2(_qos_algorithm_type, other._qos_algorithm_type) &&
                Helper.AreEqual2(_qos_algorithm_params, other._qos_algorithm_params) &&
                Helper.AreEqual2(_qos_supported_algorithms, other._qos_supported_algorithms) &&
                Helper.AreEqual2(_metrics, other._metrics);
        }


        /// <summary>
        /// Get a record containing the current state of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static VBD get_record(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_record(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get a reference to the VBD instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VBD> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.vbd_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Create a new VBD instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<VBD> create(Session session, VBD _record)
        {
            return session.JsonRpcClient.vbd_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Create a new VBD instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_create(Session session, VBD _record)
        {
          return session.JsonRpcClient.async_vbd_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Destroy the specified VBD instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void destroy(Session session, string _vbd)
        {
            session.JsonRpcClient.vbd_destroy(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Destroy the specified VBD instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _vbd)
        {
          return session.JsonRpcClient.async_vbd_destroy(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the uuid field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_uuid(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the allowed_operations field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<vbd_operations> get_allowed_operations(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_allowed_operations(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the current_operations field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, vbd_operations> get_current_operations(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_current_operations(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the VM field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VM> get_VM(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_vm(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the VDI field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VDI> get_VDI(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_vdi(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the device field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_device(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_device(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the userdevice field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_userdevice(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_userdevice(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the bootable field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_bootable(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_bootable(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the mode field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static vbd_mode get_mode(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_mode(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the type field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static vbd_type get_type(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_type(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the unpluggable field of the given VBD.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_unpluggable(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_unpluggable(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the storage_lock field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_storage_lock(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_storage_lock(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the empty field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_empty(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_empty(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the other_config field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_other_config(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the currently_attached field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_currently_attached(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_currently_attached(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the status_code field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_status_code(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_status_code(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the status_detail field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_status_detail(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_status_detail(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the runtime_properties field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_runtime_properties(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_runtime_properties(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the qos/algorithm_type field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_qos_algorithm_type(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_qos_algorithm_type(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the qos/algorithm_params field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_qos_algorithm_params(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_qos_algorithm_params(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the qos/supported_algorithms field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_qos_supported_algorithms(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_qos_supported_algorithms(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Get the metrics field of the given VBD.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        [Deprecated("XenServer 6.1")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VBD_metrics> get_metrics(Session session, string _vbd)
        {
            return session.JsonRpcClient.vbd_get_metrics(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Set the userdevice field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_userdevice">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_userdevice(Session session, string _vbd, string _userdevice)
        {
            session.JsonRpcClient.vbd_set_userdevice(session.opaque_ref, _vbd, _userdevice);
        }

        /// <summary>
        /// Set the bootable field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_bootable">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_bootable(Session session, string _vbd, bool _bootable)
        {
            session.JsonRpcClient.vbd_set_bootable(session.opaque_ref, _vbd, _bootable);
        }

        /// <summary>
        /// Set the type field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_type">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_type(Session session, string _vbd, vbd_type _type)
        {
            session.JsonRpcClient.vbd_set_type(session.opaque_ref, _vbd, _type);
        }

        /// <summary>
        /// Set the unpluggable field of the given VBD.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_unpluggable">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_unpluggable(Session session, string _vbd, bool _unpluggable)
        {
            session.JsonRpcClient.vbd_set_unpluggable(session.opaque_ref, _vbd, _unpluggable);
        }

        /// <summary>
        /// Set the other_config field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_other_config(Session session, string _vbd, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.vbd_set_other_config(session.opaque_ref, _vbd, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void add_to_other_config(Session session, string _vbd, string _key, string _value)
        {
            session.JsonRpcClient.vbd_add_to_other_config(session.opaque_ref, _vbd, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given VBD.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void remove_from_other_config(Session session, string _vbd, string _key)
        {
            session.JsonRpcClient.vbd_remove_from_other_config(session.opaque_ref, _vbd, _key);
        }

        /// <summary>
        /// Set the qos/algorithm_type field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_algorithm_type">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_qos_algorithm_type(Session session, string _vbd, string _algorithm_type)
        {
            session.JsonRpcClient.vbd_set_qos_algorithm_type(session.opaque_ref, _vbd, _algorithm_type);
        }

        /// <summary>
        /// Set the qos/algorithm_params field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_algorithm_params">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_qos_algorithm_params(Session session, string _vbd, Dictionary<string, string> _algorithm_params)
        {
            session.JsonRpcClient.vbd_set_qos_algorithm_params(session.opaque_ref, _vbd, _algorithm_params);
        }

        /// <summary>
        /// Add the given key-value pair to the qos/algorithm_params field of the given VBD.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void add_to_qos_algorithm_params(Session session, string _vbd, string _key, string _value)
        {
            session.JsonRpcClient.vbd_add_to_qos_algorithm_params(session.opaque_ref, _vbd, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the qos/algorithm_params field of the given VBD.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void remove_from_qos_algorithm_params(Session session, string _vbd, string _key)
        {
            session.JsonRpcClient.vbd_remove_from_qos_algorithm_params(session.opaque_ref, _vbd, _key);
        }

        /// <summary>
        /// Remove the media from the device and leave it empty
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void eject(Session session, string _vbd)
        {
            session.JsonRpcClient.vbd_eject(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Remove the media from the device and leave it empty
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static XenRef<Task> async_eject(Session session, string _vbd)
        {
          return session.JsonRpcClient.async_vbd_eject(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Insert new media into the device
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_vdi">The new VDI to 'insert'</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void insert(Session session, string _vbd, string _vdi)
        {
            session.JsonRpcClient.vbd_insert(session.opaque_ref, _vbd, _vdi);
        }

        /// <summary>
        /// Insert new media into the device
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_vdi">The new VDI to 'insert'</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static XenRef<Task> async_insert(Session session, string _vbd, string _vdi)
        {
          return session.JsonRpcClient.async_vbd_insert(session.opaque_ref, _vbd, _vdi);
        }

        /// <summary>
        /// Hotplug the specified VBD, dynamically attaching it to the running VM
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void plug(Session session, string _vbd)
        {
            session.JsonRpcClient.vbd_plug(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Hotplug the specified VBD, dynamically attaching it to the running VM
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_plug(Session session, string _vbd)
        {
          return session.JsonRpcClient.async_vbd_plug(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Hot-unplug the specified VBD, dynamically unattaching it from the running VM
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void unplug(Session session, string _vbd)
        {
            session.JsonRpcClient.vbd_unplug(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Hot-unplug the specified VBD, dynamically unattaching it from the running VM
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_unplug(Session session, string _vbd)
        {
          return session.JsonRpcClient.async_vbd_unplug(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Forcibly unplug the specified VBD
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void unplug_force(Session session, string _vbd)
        {
            session.JsonRpcClient.vbd_unplug_force(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Forcibly unplug the specified VBD
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_unplug_force(Session session, string _vbd)
        {
          return session.JsonRpcClient.async_vbd_unplug_force(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Throws an error if this VBD could not be attached to this VM if the VM were running. Intended for debugging.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void assert_attachable(Session session, string _vbd)
        {
            session.JsonRpcClient.vbd_assert_attachable(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Throws an error if this VBD could not be attached to this VM if the VM were running. Intended for debugging.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_assert_attachable(Session session, string _vbd)
        {
          return session.JsonRpcClient.async_vbd_assert_attachable(session.opaque_ref, _vbd);
        }

        /// <summary>
        /// Sets the mode of the VBD. The power_state of the VM must be halted.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_value">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_mode(Session session, string _vbd, vbd_mode _value)
        {
            session.JsonRpcClient.vbd_set_mode(session.opaque_ref, _vbd, _value);
        }

        /// <summary>
        /// Sets the mode of the VBD. The power_state of the VM must be halted.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vbd">The opaque_ref of the given vbd</param>
        /// <param name="_value">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_set_mode(Session session, string _vbd, vbd_mode _value)
        {
          return session.JsonRpcClient.async_vbd_set_mode(session.opaque_ref, _vbd, _value);
        }

        /// <summary>
        /// Return a list of all the VBDs known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VBD>> get_all(Session session)
        {
            return session.JsonRpcClient.vbd_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of VBD references to VBD records for all VBDs known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<VBD>, VBD> get_all_records(Session session)
        {
            return session.JsonRpcClient.vbd_get_all_records(session.opaque_ref);
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
                    NotifyPropertyChanged("VDI");
                }
            }
        }
        private XenRef<VDI> _VDI = new XenRef<VDI>(Helper.NullOpaqueRef);

        /// <summary>
        /// device seen by the guest, for example, hda1
        /// </summary>
        public virtual string device
        {
            get { return _device; }
            set
            {
                if (!Helper.AreEqual(value, _device))
                {
                    _device = value;
                    NotifyPropertyChanged("device");
                }
            }
        }
        private string _device = "";

        /// <summary>
        /// user-friendly device name, for example, 0, 1, 2, etc.
        /// </summary>
        public virtual string userdevice
        {
            get { return _userdevice; }
            set
            {
                if (!Helper.AreEqual(value, _userdevice))
                {
                    _userdevice = value;
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
                    NotifyPropertyChanged("mode");
                }
            }
        }
        private vbd_mode _mode;

        /// <summary>
        /// how the VBD will appear to the guest (for example, disk or CD)
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
                    NotifyPropertyChanged("currently_attached");
                }
            }
        }
        private bool _currently_attached = false;

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
                    NotifyPropertyChanged("metrics");
                }
            }
        }
        private XenRef<VBD_metrics> _metrics = new XenRef<VBD_metrics>("OpaqueRef:NULL");
    }
}
