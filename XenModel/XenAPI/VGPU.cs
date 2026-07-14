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

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VGPU.
        /// </summary>
        public override void UpdateFrom(VGPU record)
        {
            uuid = record.uuid;
            VM = record.VM;
            GPU_group = record.GPU_group;
            device = record.device;
            currently_attached = record.currently_attached;
            other_config = record.other_config;
            type = record.type;
            resident_on = record.resident_on;
            scheduled_to_be_resident_on = record.scheduled_to_be_resident_on;
            compatibility_metadata = record.compatibility_metadata;
            extra_args = record.extra_args;
            PCI = record.PCI;
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
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("type"))
                type = Marshalling.ParseRef<VGPU_type>(table, "type");
            if (table.ContainsKey("resident_on"))
                resident_on = Marshalling.ParseRef<PGPU>(table, "resident_on");
            if (table.ContainsKey("scheduled_to_be_resident_on"))
                scheduled_to_be_resident_on = Marshalling.ParseRef<PGPU>(table, "scheduled_to_be_resident_on");
            if (table.ContainsKey("compatibility_metadata"))
                compatibility_metadata = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "compatibility_metadata"));
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

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_VM, other._VM) &&
                Helper.AreEqual2(_GPU_group, other._GPU_group) &&
                Helper.AreEqual2(_device, other._device) &&
                Helper.AreEqual2(_currently_attached, other._currently_attached) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_type, other._type) &&
                Helper.AreEqual2(_resident_on, other._resident_on) &&
                Helper.AreEqual2(_scheduled_to_be_resident_on, other._scheduled_to_be_resident_on) &&
                Helper.AreEqual2(_compatibility_metadata, other._compatibility_metadata) &&
                Helper.AreEqual2(_extra_args, other._extra_args) &&
                Helper.AreEqual2(_PCI, other._PCI);
        }


        /// <summary>
        /// Get a record containing the current state of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static VGPU get_record(Session session, string _vgpu)
        {
            return session.JsonRpcClient.vgpu_get_record(session.opaque_ref, _vgpu);
        }

        /// <summary>
        /// Get a reference to the VGPU instance with the specified UUID.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VGPU> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.vgpu_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _vgpu)
        {
            return session.JsonRpcClient.vgpu_get_uuid(session.opaque_ref, _vgpu);
        }

        /// <summary>
        /// Get the VM field of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VM> get_VM(Session session, string _vgpu)
        {
            return session.JsonRpcClient.vgpu_get_vm(session.opaque_ref, _vgpu);
        }

        /// <summary>
        /// Get the GPU_group field of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<GPU_group> get_GPU_group(Session session, string _vgpu)
        {
            return session.JsonRpcClient.vgpu_get_gpu_group(session.opaque_ref, _vgpu);
        }

        /// <summary>
        /// Get the device field of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_device(Session session, string _vgpu)
        {
            return session.JsonRpcClient.vgpu_get_device(session.opaque_ref, _vgpu);
        }

        /// <summary>
        /// Get the currently_attached field of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_currently_attached(Session session, string _vgpu)
        {
            return session.JsonRpcClient.vgpu_get_currently_attached(session.opaque_ref, _vgpu);
        }

        /// <summary>
        /// Get the other_config field of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _vgpu)
        {
            return session.JsonRpcClient.vgpu_get_other_config(session.opaque_ref, _vgpu);
        }

        /// <summary>
        /// Get the type field of the given VGPU.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VGPU_type> get_type(Session session, string _vgpu)
        {
            return session.JsonRpcClient.vgpu_get_type(session.opaque_ref, _vgpu);
        }

        /// <summary>
        /// Get the resident_on field of the given VGPU.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PGPU> get_resident_on(Session session, string _vgpu)
        {
            return session.JsonRpcClient.vgpu_get_resident_on(session.opaque_ref, _vgpu);
        }

        /// <summary>
        /// Get the scheduled_to_be_resident_on field of the given VGPU.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PGPU> get_scheduled_to_be_resident_on(Session session, string _vgpu)
        {
            return session.JsonRpcClient.vgpu_get_scheduled_to_be_resident_on(session.opaque_ref, _vgpu);
        }

        /// <summary>
        /// Get the compatibility_metadata field of the given VGPU.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_compatibility_metadata(Session session, string _vgpu)
        {
            return session.JsonRpcClient.vgpu_get_compatibility_metadata(session.opaque_ref, _vgpu);
        }

        /// <summary>
        /// Get the extra_args field of the given VGPU.
        /// First published in Citrix Hypervisor 8.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_extra_args(Session session, string _vgpu)
        {
            return session.JsonRpcClient.vgpu_get_extra_args(session.opaque_ref, _vgpu);
        }

        /// <summary>
        /// Get the PCI field of the given VGPU.
        /// First published in Citrix Hypervisor 8.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PCI> get_PCI(Session session, string _vgpu)
        {
            return session.JsonRpcClient.vgpu_get_pci(session.opaque_ref, _vgpu);
        }

        /// <summary>
        /// Set the other_config field of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_other_config(Session session, string _vgpu, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.vgpu_set_other_config(session.opaque_ref, _vgpu, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given VGPU.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_other_config(Session session, string _vgpu, string _key, string _value)
        {
            session.JsonRpcClient.vgpu_add_to_other_config(session.opaque_ref, _vgpu, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given VGPU.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_other_config(Session session, string _vgpu, string _key)
        {
            session.JsonRpcClient.vgpu_remove_from_other_config(session.opaque_ref, _vgpu, _key);
        }

        /// <summary>
        /// Set the extra_args field of the given VGPU.
        /// First published in Citrix Hypervisor 8.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <param name="_extra_args">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_extra_args(Session session, string _vgpu, string _extra_args)
        {
            session.JsonRpcClient.vgpu_set_extra_args(session.opaque_ref, _vgpu, _extra_args);
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
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<VGPU> create(Session session, string _vm, string _gpu_group, string _device, Dictionary<string, string> _other_config)
        {
            return session.JsonRpcClient.vgpu_create(session.opaque_ref, _vm, _gpu_group, _device, _other_config);
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
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, string _vm, string _gpu_group, string _device, Dictionary<string, string> _other_config)
        {
          return session.JsonRpcClient.async_vgpu_create(session.opaque_ref, _vm, _gpu_group, _device, _other_config);
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
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<VGPU> create(Session session, string _vm, string _gpu_group, string _device, Dictionary<string, string> _other_config, string _type)
        {
            return session.JsonRpcClient.vgpu_create(session.opaque_ref, _vm, _gpu_group, _device, _other_config, _type);
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
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, string _vm, string _gpu_group, string _device, Dictionary<string, string> _other_config, string _type)
        {
          return session.JsonRpcClient.async_vgpu_create(session.opaque_ref, _vm, _gpu_group, _device, _other_config, _type);
        }

        /// <summary>
        /// 
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _vgpu)
        {
            session.JsonRpcClient.vgpu_destroy(session.opaque_ref, _vgpu);
        }

        /// <summary>
        /// 
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu">The opaque_ref of the given vgpu</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _vgpu)
        {
          return session.JsonRpcClient.async_vgpu_destroy(session.opaque_ref, _vgpu);
        }

        /// <summary>
        /// Return a list of all the VGPUs known to the system.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VGPU>> get_all(Session session)
        {
            return session.JsonRpcClient.vgpu_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of VGPU references to VGPU records for all VGPUs known to the system.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<VGPU>, VGPU> get_all_records(Session session)
        {
            return session.JsonRpcClient.vgpu_get_all_records(session.opaque_ref);
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
                    NotifyPropertyChanged("compatibility_metadata");
                }
            }
        }
        private Dictionary<string, string> _compatibility_metadata = new Dictionary<string, string>() {};

        /// <summary>
        /// Extra arguments for vGPU and passed to demu
        /// First published in Citrix Hypervisor 8.1.
        /// </summary>
        public virtual string extra_args
        {
            get { return _extra_args; }
            set
            {
                if (!Helper.AreEqual(value, _extra_args))
                {
                    _extra_args = value;
                    NotifyPropertyChanged("extra_args");
                }
            }
        }
        private string _extra_args = "";

        /// <summary>
        /// Device passed trough to VM, either as full device or SR-IOV virtual function
        /// First published in Citrix Hypervisor 8.1.
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
                    NotifyPropertyChanged("PCI");
                }
            }
        }
        private XenRef<PCI> _PCI = new XenRef<PCI>("OpaqueRef:NULL");
    }
}
