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
    /// A group of compatible GPUs across the resource pool
    /// First published in XenServer 6.0.
    /// </summary>
    public partial class GPU_group : XenObject<GPU_group>
    {
        #region Constructors

        public GPU_group()
        {
        }

        public GPU_group(string uuid,
            string name_label,
            string name_description,
            List<XenRef<PGPU>> PGPUs,
            List<XenRef<VGPU>> VGPUs,
            string[] GPU_types,
            Dictionary<string, string> other_config,
            allocation_algorithm allocation_algorithm,
            List<XenRef<VGPU_type>> supported_VGPU_types,
            List<XenRef<VGPU_type>> enabled_VGPU_types)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.PGPUs = PGPUs;
            this.VGPUs = VGPUs;
            this.GPU_types = GPU_types;
            this.other_config = other_config;
            this.allocation_algorithm = allocation_algorithm;
            this.supported_VGPU_types = supported_VGPU_types;
            this.enabled_VGPU_types = enabled_VGPU_types;
        }

        /// <summary>
        /// Creates a new GPU_group from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public GPU_group(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given GPU_group.
        /// </summary>
        public override void UpdateFrom(GPU_group record)
        {
            uuid = record.uuid;
            name_label = record.name_label;
            name_description = record.name_description;
            PGPUs = record.PGPUs;
            VGPUs = record.VGPUs;
            GPU_types = record.GPU_types;
            other_config = record.other_config;
            allocation_algorithm = record.allocation_algorithm;
            supported_VGPU_types = record.supported_VGPU_types;
            enabled_VGPU_types = record.enabled_VGPU_types;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this GPU_group
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
            if (table.ContainsKey("PGPUs"))
                PGPUs = Marshalling.ParseSetRef<PGPU>(table, "PGPUs");
            if (table.ContainsKey("VGPUs"))
                VGPUs = Marshalling.ParseSetRef<VGPU>(table, "VGPUs");
            if (table.ContainsKey("GPU_types"))
                GPU_types = Marshalling.ParseStringArray(table, "GPU_types");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("allocation_algorithm"))
                allocation_algorithm = (allocation_algorithm)Helper.EnumParseDefault(typeof(allocation_algorithm), Marshalling.ParseString(table, "allocation_algorithm"));
            if (table.ContainsKey("supported_VGPU_types"))
                supported_VGPU_types = Marshalling.ParseSetRef<VGPU_type>(table, "supported_VGPU_types");
            if (table.ContainsKey("enabled_VGPU_types"))
                enabled_VGPU_types = Marshalling.ParseSetRef<VGPU_type>(table, "enabled_VGPU_types");
        }

        public bool DeepEquals(GPU_group other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name_label, other._name_label) &&
                Helper.AreEqual2(_name_description, other._name_description) &&
                Helper.AreEqual2(_PGPUs, other._PGPUs) &&
                Helper.AreEqual2(_VGPUs, other._VGPUs) &&
                Helper.AreEqual2(_GPU_types, other._GPU_types) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_allocation_algorithm, other._allocation_algorithm) &&
                Helper.AreEqual2(_supported_VGPU_types, other._supported_VGPU_types) &&
                Helper.AreEqual2(_enabled_VGPU_types, other._enabled_VGPU_types);
        }


        /// <summary>
        /// Get a record containing the current state of the given GPU_group.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static GPU_group get_record(Session session, string _gpu_group)
        {
            return session.JsonRpcClient.gpu_group_get_record(session.opaque_ref, _gpu_group);
        }

        /// <summary>
        /// Get a reference to the GPU_group instance with the specified UUID.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<GPU_group> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.gpu_group_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get all the GPU_group instances with the given label.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<GPU_group>> get_by_name_label(Session session, string _label)
        {
            return session.JsonRpcClient.gpu_group_get_by_name_label(session.opaque_ref, _label);
        }

        /// <summary>
        /// Get the uuid field of the given GPU_group.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _gpu_group)
        {
            return session.JsonRpcClient.gpu_group_get_uuid(session.opaque_ref, _gpu_group);
        }

        /// <summary>
        /// Get the name/label field of the given GPU_group.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_label(Session session, string _gpu_group)
        {
            return session.JsonRpcClient.gpu_group_get_name_label(session.opaque_ref, _gpu_group);
        }

        /// <summary>
        /// Get the name/description field of the given GPU_group.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_description(Session session, string _gpu_group)
        {
            return session.JsonRpcClient.gpu_group_get_name_description(session.opaque_ref, _gpu_group);
        }

        /// <summary>
        /// Get the PGPUs field of the given GPU_group.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PGPU>> get_PGPUs(Session session, string _gpu_group)
        {
            return session.JsonRpcClient.gpu_group_get_pgpus(session.opaque_ref, _gpu_group);
        }

        /// <summary>
        /// Get the VGPUs field of the given GPU_group.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VGPU>> get_VGPUs(Session session, string _gpu_group)
        {
            return session.JsonRpcClient.gpu_group_get_vgpus(session.opaque_ref, _gpu_group);
        }

        /// <summary>
        /// Get the GPU_types field of the given GPU_group.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_GPU_types(Session session, string _gpu_group)
        {
            return session.JsonRpcClient.gpu_group_get_gpu_types(session.opaque_ref, _gpu_group);
        }

        /// <summary>
        /// Get the other_config field of the given GPU_group.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _gpu_group)
        {
            return session.JsonRpcClient.gpu_group_get_other_config(session.opaque_ref, _gpu_group);
        }

        /// <summary>
        /// Get the allocation_algorithm field of the given GPU_group.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static allocation_algorithm get_allocation_algorithm(Session session, string _gpu_group)
        {
            return session.JsonRpcClient.gpu_group_get_allocation_algorithm(session.opaque_ref, _gpu_group);
        }

        /// <summary>
        /// Get the supported_VGPU_types field of the given GPU_group.
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VGPU_type>> get_supported_VGPU_types(Session session, string _gpu_group)
        {
            return session.JsonRpcClient.gpu_group_get_supported_vgpu_types(session.opaque_ref, _gpu_group);
        }

        /// <summary>
        /// Get the enabled_VGPU_types field of the given GPU_group.
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VGPU_type>> get_enabled_VGPU_types(Session session, string _gpu_group)
        {
            return session.JsonRpcClient.gpu_group_get_enabled_vgpu_types(session.opaque_ref, _gpu_group);
        }

        /// <summary>
        /// Set the name/label field of the given GPU_group.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <param name="_label">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_name_label(Session session, string _gpu_group, string _label)
        {
            session.JsonRpcClient.gpu_group_set_name_label(session.opaque_ref, _gpu_group, _label);
        }

        /// <summary>
        /// Set the name/description field of the given GPU_group.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <param name="_description">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_name_description(Session session, string _gpu_group, string _description)
        {
            session.JsonRpcClient.gpu_group_set_name_description(session.opaque_ref, _gpu_group, _description);
        }

        /// <summary>
        /// Set the other_config field of the given GPU_group.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_other_config(Session session, string _gpu_group, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.gpu_group_set_other_config(session.opaque_ref, _gpu_group, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given GPU_group.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_other_config(Session session, string _gpu_group, string _key, string _value)
        {
            session.JsonRpcClient.gpu_group_add_to_other_config(session.opaque_ref, _gpu_group, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given GPU_group.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_other_config(Session session, string _gpu_group, string _key)
        {
            session.JsonRpcClient.gpu_group_remove_from_other_config(session.opaque_ref, _gpu_group, _key);
        }

        /// <summary>
        /// Set the allocation_algorithm field of the given GPU_group.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <param name="_allocation_algorithm">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_allocation_algorithm(Session session, string _gpu_group, allocation_algorithm _allocation_algorithm)
        {
            session.JsonRpcClient.gpu_group_set_allocation_algorithm(session.opaque_ref, _gpu_group, _allocation_algorithm);
        }

        /// <summary>
        /// 
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name_label"></param>
        /// <param name="_name_description"></param>
        /// <param name="_other_config"></param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<GPU_group> create(Session session, string _name_label, string _name_description, Dictionary<string, string> _other_config)
        {
            return session.JsonRpcClient.gpu_group_create(session.opaque_ref, _name_label, _name_description, _other_config);
        }

        /// <summary>
        /// 
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name_label"></param>
        /// <param name="_name_description"></param>
        /// <param name="_other_config"></param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, string _name_label, string _name_description, Dictionary<string, string> _other_config)
        {
          return session.JsonRpcClient.async_gpu_group_create(session.opaque_ref, _name_label, _name_description, _other_config);
        }

        /// <summary>
        /// 
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _gpu_group)
        {
            session.JsonRpcClient.gpu_group_destroy(session.opaque_ref, _gpu_group);
        }

        /// <summary>
        /// 
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _gpu_group)
        {
          return session.JsonRpcClient.async_gpu_group_destroy(session.opaque_ref, _gpu_group);
        }

        /// <summary>
        /// 
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <param name="_vgpu_type">The VGPU_type for which the remaining capacity will be calculated</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_remaining_capacity(Session session, string _gpu_group, string _vgpu_type)
        {
            return session.JsonRpcClient.gpu_group_get_remaining_capacity(session.opaque_ref, _gpu_group, _vgpu_type);
        }

        /// <summary>
        /// 
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_gpu_group">The opaque_ref of the given gpu_group</param>
        /// <param name="_vgpu_type">The VGPU_type for which the remaining capacity will be calculated</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_get_remaining_capacity(Session session, string _gpu_group, string _vgpu_type)
        {
          return session.JsonRpcClient.async_gpu_group_get_remaining_capacity(session.opaque_ref, _gpu_group, _vgpu_type);
        }

        /// <summary>
        /// Return a list of all the GPU_groups known to the system.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<GPU_group>> get_all(Session session)
        {
            return session.JsonRpcClient.gpu_group_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of GPU_group references to GPU_group records for all GPU_groups known to the system.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<GPU_group>, GPU_group> get_all_records(Session session)
        {
            return session.JsonRpcClient.gpu_group_get_all_records(session.opaque_ref);
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
        /// List of pGPUs in the group
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<PGPU>))]
        public virtual List<XenRef<PGPU>> PGPUs
        {
            get { return _PGPUs; }
            set
            {
                if (!Helper.AreEqual(value, _PGPUs))
                {
                    _PGPUs = value;
                    NotifyPropertyChanged("PGPUs");
                }
            }
        }
        private List<XenRef<PGPU>> _PGPUs = new List<XenRef<PGPU>>() {};

        /// <summary>
        /// List of vGPUs using the group
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VGPU>))]
        public virtual List<XenRef<VGPU>> VGPUs
        {
            get { return _VGPUs; }
            set
            {
                if (!Helper.AreEqual(value, _VGPUs))
                {
                    _VGPUs = value;
                    NotifyPropertyChanged("VGPUs");
                }
            }
        }
        private List<XenRef<VGPU>> _VGPUs = new List<XenRef<VGPU>>() {};

        /// <summary>
        /// List of GPU types (vendor+device ID) that can be in this group
        /// </summary>
        public virtual string[] GPU_types
        {
            get { return _GPU_types; }
            set
            {
                if (!Helper.AreEqual(value, _GPU_types))
                {
                    _GPU_types = value;
                    NotifyPropertyChanged("GPU_types");
                }
            }
        }
        private string[] _GPU_types = {};

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
        /// Current allocation of vGPUs to pGPUs for this group
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        [JsonConverter(typeof(allocation_algorithmConverter))]
        public virtual allocation_algorithm allocation_algorithm
        {
            get { return _allocation_algorithm; }
            set
            {
                if (!Helper.AreEqual(value, _allocation_algorithm))
                {
                    _allocation_algorithm = value;
                    NotifyPropertyChanged("allocation_algorithm");
                }
            }
        }
        private allocation_algorithm _allocation_algorithm = allocation_algorithm.depth_first;

        /// <summary>
        /// vGPU types supported on at least one of the pGPUs in this group
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VGPU_type>))]
        public virtual List<XenRef<VGPU_type>> supported_VGPU_types
        {
            get { return _supported_VGPU_types; }
            set
            {
                if (!Helper.AreEqual(value, _supported_VGPU_types))
                {
                    _supported_VGPU_types = value;
                    NotifyPropertyChanged("supported_VGPU_types");
                }
            }
        }
        private List<XenRef<VGPU_type>> _supported_VGPU_types = new List<XenRef<VGPU_type>>() {};

        /// <summary>
        /// vGPU types supported on at least one of the pGPUs in this group
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VGPU_type>))]
        public virtual List<XenRef<VGPU_type>> enabled_VGPU_types
        {
            get { return _enabled_VGPU_types; }
            set
            {
                if (!Helper.AreEqual(value, _enabled_VGPU_types))
                {
                    _enabled_VGPU_types = value;
                    NotifyPropertyChanged("enabled_VGPU_types");
                }
            }
        }
        private List<XenRef<VGPU_type>> _enabled_VGPU_types = new List<XenRef<VGPU_type>>() {};
    }
}
