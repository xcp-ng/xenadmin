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
    /// The metrics associated with a VM
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class VM_metrics : XenObject<VM_metrics>
    {
        #region Constructors

        public VM_metrics()
        {
        }

        public VM_metrics(string uuid,
            long memory_actual,
            long VCPUs_number,
            Dictionary<long, double> VCPUs_utilisation,
            Dictionary<long, long> VCPUs_CPU,
            Dictionary<string, string> VCPUs_params,
            Dictionary<long, string[]> VCPUs_flags,
            string[] state,
            DateTime start_time,
            DateTime install_time,
            DateTime last_updated,
            Dictionary<string, string> other_config,
            bool hvm,
            bool nested_virt,
            bool nomigrate,
            domain_type current_domain_type,
            bool numa_optimised,
            long numa_nodes,
            Dictionary<long, long> numa_node_memory)
        {
            this.uuid = uuid;
            this.memory_actual = memory_actual;
            this.VCPUs_number = VCPUs_number;
            this.VCPUs_utilisation = VCPUs_utilisation;
            this.VCPUs_CPU = VCPUs_CPU;
            this.VCPUs_params = VCPUs_params;
            this.VCPUs_flags = VCPUs_flags;
            this.state = state;
            this.start_time = start_time;
            this.install_time = install_time;
            this.last_updated = last_updated;
            this.other_config = other_config;
            this.hvm = hvm;
            this.nested_virt = nested_virt;
            this.nomigrate = nomigrate;
            this.current_domain_type = current_domain_type;
            this.numa_optimised = numa_optimised;
            this.numa_nodes = numa_nodes;
            this.numa_node_memory = numa_node_memory;
        }

        /// <summary>
        /// Creates a new VM_metrics from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VM_metrics(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VM_metrics.
        /// </summary>
        public override void UpdateFrom(VM_metrics record)
        {
            uuid = record.uuid;
            memory_actual = record.memory_actual;
            VCPUs_number = record.VCPUs_number;
            VCPUs_utilisation = record.VCPUs_utilisation;
            VCPUs_CPU = record.VCPUs_CPU;
            VCPUs_params = record.VCPUs_params;
            VCPUs_flags = record.VCPUs_flags;
            state = record.state;
            start_time = record.start_time;
            install_time = record.install_time;
            last_updated = record.last_updated;
            other_config = record.other_config;
            hvm = record.hvm;
            nested_virt = record.nested_virt;
            nomigrate = record.nomigrate;
            current_domain_type = record.current_domain_type;
            numa_optimised = record.numa_optimised;
            numa_nodes = record.numa_nodes;
            numa_node_memory = record.numa_node_memory;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this VM_metrics
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("memory_actual"))
                memory_actual = Marshalling.ParseLong(table, "memory_actual");
            if (table.ContainsKey("VCPUs_number"))
                VCPUs_number = Marshalling.ParseLong(table, "VCPUs_number");
            if (table.ContainsKey("VCPUs_utilisation"))
                VCPUs_utilisation = Maps.ToDictionary_long_double(Marshalling.ParseHashTable(table, "VCPUs_utilisation"));
            if (table.ContainsKey("VCPUs_CPU"))
                VCPUs_CPU = Maps.ToDictionary_long_long(Marshalling.ParseHashTable(table, "VCPUs_CPU"));
            if (table.ContainsKey("VCPUs_params"))
                VCPUs_params = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "VCPUs_params"));
            if (table.ContainsKey("VCPUs_flags"))
                VCPUs_flags = Maps.ToDictionary_long_string_array(Marshalling.ParseHashTable(table, "VCPUs_flags"));
            if (table.ContainsKey("state"))
                state = Marshalling.ParseStringArray(table, "state");
            if (table.ContainsKey("start_time"))
                start_time = Marshalling.ParseDateTime(table, "start_time");
            if (table.ContainsKey("install_time"))
                install_time = Marshalling.ParseDateTime(table, "install_time");
            if (table.ContainsKey("last_updated"))
                last_updated = Marshalling.ParseDateTime(table, "last_updated");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("hvm"))
                hvm = Marshalling.ParseBool(table, "hvm");
            if (table.ContainsKey("nested_virt"))
                nested_virt = Marshalling.ParseBool(table, "nested_virt");
            if (table.ContainsKey("nomigrate"))
                nomigrate = Marshalling.ParseBool(table, "nomigrate");
            if (table.ContainsKey("current_domain_type"))
                current_domain_type = (domain_type)Helper.EnumParseDefault(typeof(domain_type), Marshalling.ParseString(table, "current_domain_type"));
            if (table.ContainsKey("numa_optimised"))
                numa_optimised = Marshalling.ParseBool(table, "numa_optimised");
            if (table.ContainsKey("numa_nodes"))
                numa_nodes = Marshalling.ParseLong(table, "numa_nodes");
            if (table.ContainsKey("numa_node_memory"))
                numa_node_memory = Maps.ToDictionary_long_long(Marshalling.ParseHashTable(table, "numa_node_memory"));
        }

        public bool DeepEquals(VM_metrics other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_memory_actual, other._memory_actual) &&
                Helper.AreEqual2(_VCPUs_number, other._VCPUs_number) &&
                Helper.AreEqual2(_VCPUs_utilisation, other._VCPUs_utilisation) &&
                Helper.AreEqual2(_VCPUs_CPU, other._VCPUs_CPU) &&
                Helper.AreEqual2(_VCPUs_params, other._VCPUs_params) &&
                Helper.AreEqual2(_VCPUs_flags, other._VCPUs_flags) &&
                Helper.AreEqual2(_state, other._state) &&
                Helper.AreEqual2(_start_time, other._start_time) &&
                Helper.AreEqual2(_install_time, other._install_time) &&
                Helper.AreEqual2(_last_updated, other._last_updated) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_hvm, other._hvm) &&
                Helper.AreEqual2(_nested_virt, other._nested_virt) &&
                Helper.AreEqual2(_nomigrate, other._nomigrate) &&
                Helper.AreEqual2(_current_domain_type, other._current_domain_type) &&
                Helper.AreEqual2(_numa_optimised, other._numa_optimised) &&
                Helper.AreEqual2(_numa_nodes, other._numa_nodes) &&
                Helper.AreEqual2(_numa_node_memory, other._numa_node_memory);
        }


        /// <summary>
        /// Get a record containing the current state of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static VM_metrics get_record(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_record(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get a reference to the VM_metrics instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VM_metrics> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.vm_metrics_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_uuid(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the memory/actual field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_memory_actual(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_memory_actual(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the VCPUs/number field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_VCPUs_number(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_vcpus_number(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the VCPUs/utilisation field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        [Deprecated("XenServer 6.1")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<long, double> get_VCPUs_utilisation(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_vcpus_utilisation(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the VCPUs/CPU field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<long, long> get_VCPUs_CPU(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_vcpus_cpu(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the VCPUs/params field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_VCPUs_params(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_vcpus_params(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the VCPUs/flags field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<long, string[]> get_VCPUs_flags(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_vcpus_flags(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the state field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_state(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_state(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the start_time field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_start_time(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_start_time(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the install_time field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_install_time(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_install_time(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the last_updated field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_last_updated(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_last_updated(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the other_config field of the given VM_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_other_config(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the hvm field of the given VM_metrics.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_hvm(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_hvm(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the nested_virt field of the given VM_metrics.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_nested_virt(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_nested_virt(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the nomigrate field of the given VM_metrics.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_nomigrate(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_nomigrate(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the current_domain_type field of the given VM_metrics.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static domain_type get_current_domain_type(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_current_domain_type(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the numa_optimised field of the given VM_metrics.
        /// Experimental. First published in 26.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_numa_optimised(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_numa_optimised(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the numa_nodes field of the given VM_metrics.
        /// Experimental. First published in 26.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_numa_nodes(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_numa_nodes(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Get the numa_node_memory field of the given VM_metrics.
        /// Experimental. First published in 26.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<long, long> get_numa_node_memory(Session session, string _vm_metrics)
        {
            return session.JsonRpcClient.vm_metrics_get_numa_node_memory(session.opaque_ref, _vm_metrics);
        }

        /// <summary>
        /// Set the other_config field of the given VM_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_other_config(Session session, string _vm_metrics, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.vm_metrics_set_other_config(session.opaque_ref, _vm_metrics, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given VM_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void add_to_other_config(Session session, string _vm_metrics, string _key, string _value)
        {
            session.JsonRpcClient.vm_metrics_add_to_other_config(session.opaque_ref, _vm_metrics, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given VM_metrics.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void remove_from_other_config(Session session, string _vm_metrics, string _key)
        {
            session.JsonRpcClient.vm_metrics_remove_from_other_config(session.opaque_ref, _vm_metrics, _key);
        }

        /// <summary>
        /// Return a list of all the VM_metrics instances known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VM_metrics>> get_all(Session session)
        {
            return session.JsonRpcClient.vm_metrics_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of VM_metrics references to VM_metrics records for all VM_metrics instances known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<VM_metrics>, VM_metrics> get_all_records(Session session)
        {
            return session.JsonRpcClient.vm_metrics_get_all_records(session.opaque_ref);
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
        /// Guest's actual memory (bytes)
        /// </summary>
        public virtual long memory_actual
        {
            get { return _memory_actual; }
            set
            {
                if (!Helper.AreEqual(value, _memory_actual))
                {
                    _memory_actual = value;
                    NotifyPropertyChanged("memory_actual");
                }
            }
        }
        private long _memory_actual;

        /// <summary>
        /// Current number of VCPUs
        /// </summary>
        public virtual long VCPUs_number
        {
            get { return _VCPUs_number; }
            set
            {
                if (!Helper.AreEqual(value, _VCPUs_number))
                {
                    _VCPUs_number = value;
                    NotifyPropertyChanged("VCPUs_number");
                }
            }
        }
        private long _VCPUs_number;

        /// <summary>
        /// Utilisation for all of guest's current VCPUs
        /// </summary>
        public virtual Dictionary<long, double> VCPUs_utilisation
        {
            get { return _VCPUs_utilisation; }
            set
            {
                if (!Helper.AreEqual(value, _VCPUs_utilisation))
                {
                    _VCPUs_utilisation = value;
                    NotifyPropertyChanged("VCPUs_utilisation");
                }
            }
        }
        private Dictionary<long, double> _VCPUs_utilisation = new Dictionary<long, double>() {};

        /// <summary>
        /// VCPU to PCPU map
        /// </summary>
        public virtual Dictionary<long, long> VCPUs_CPU
        {
            get { return _VCPUs_CPU; }
            set
            {
                if (!Helper.AreEqual(value, _VCPUs_CPU))
                {
                    _VCPUs_CPU = value;
                    NotifyPropertyChanged("VCPUs_CPU");
                }
            }
        }
        private Dictionary<long, long> _VCPUs_CPU = new Dictionary<long, long>() {};

        /// <summary>
        /// The live equivalent to VM.VCPUs_params
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> VCPUs_params
        {
            get { return _VCPUs_params; }
            set
            {
                if (!Helper.AreEqual(value, _VCPUs_params))
                {
                    _VCPUs_params = value;
                    NotifyPropertyChanged("VCPUs_params");
                }
            }
        }
        private Dictionary<string, string> _VCPUs_params = new Dictionary<string, string>() {};

        /// <summary>
        /// CPU flags (blocked,online,running)
        /// </summary>
        public virtual Dictionary<long, string[]> VCPUs_flags
        {
            get { return _VCPUs_flags; }
            set
            {
                if (!Helper.AreEqual(value, _VCPUs_flags))
                {
                    _VCPUs_flags = value;
                    NotifyPropertyChanged("VCPUs_flags");
                }
            }
        }
        private Dictionary<long, string[]> _VCPUs_flags = new Dictionary<long, string[]>() {};

        /// <summary>
        /// The state of the guest, eg blocked, dying etc
        /// </summary>
        public virtual string[] state
        {
            get { return _state; }
            set
            {
                if (!Helper.AreEqual(value, _state))
                {
                    _state = value;
                    NotifyPropertyChanged("state");
                }
            }
        }
        private string[] _state = {};

        /// <summary>
        /// Time at which this VM was last booted
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime start_time
        {
            get { return _start_time; }
            set
            {
                if (!Helper.AreEqual(value, _start_time))
                {
                    _start_time = value;
                    NotifyPropertyChanged("start_time");
                }
            }
        }
        private DateTime _start_time;

        /// <summary>
        /// Time at which the VM was installed
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime install_time
        {
            get { return _install_time; }
            set
            {
                if (!Helper.AreEqual(value, _install_time))
                {
                    _install_time = value;
                    NotifyPropertyChanged("install_time");
                }
            }
        }
        private DateTime _install_time;

        /// <summary>
        /// Time at which this information was last updated
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime last_updated
        {
            get { return _last_updated; }
            set
            {
                if (!Helper.AreEqual(value, _last_updated))
                {
                    _last_updated = value;
                    NotifyPropertyChanged("last_updated");
                }
            }
        }
        private DateTime _last_updated;

        /// <summary>
        /// additional configuration
        /// First published in XenServer 5.0.
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
        /// hardware virtual machine
        /// First published in XenServer 7.1.
        /// </summary>
        public virtual bool hvm
        {
            get { return _hvm; }
            set
            {
                if (!Helper.AreEqual(value, _hvm))
                {
                    _hvm = value;
                    NotifyPropertyChanged("hvm");
                }
            }
        }
        private bool _hvm = false;

        /// <summary>
        /// VM supports nested virtualisation
        /// First published in XenServer 7.1.
        /// </summary>
        public virtual bool nested_virt
        {
            get { return _nested_virt; }
            set
            {
                if (!Helper.AreEqual(value, _nested_virt))
                {
                    _nested_virt = value;
                    NotifyPropertyChanged("nested_virt");
                }
            }
        }
        private bool _nested_virt = false;

        /// <summary>
        /// VM is immobile and can't migrate between hosts
        /// First published in XenServer 7.1.
        /// </summary>
        public virtual bool nomigrate
        {
            get { return _nomigrate; }
            set
            {
                if (!Helper.AreEqual(value, _nomigrate))
                {
                    _nomigrate = value;
                    NotifyPropertyChanged("nomigrate");
                }
            }
        }
        private bool _nomigrate = false;

        /// <summary>
        /// The current domain type of the VM (for running,suspended, or paused VMs). The last-known domain type for halted VMs.
        /// First published in XenServer 7.5.
        /// </summary>
        [JsonConverter(typeof(domain_typeConverter))]
        public virtual domain_type current_domain_type
        {
            get { return _current_domain_type; }
            set
            {
                if (!Helper.AreEqual(value, _current_domain_type))
                {
                    _current_domain_type = value;
                    NotifyPropertyChanged("current_domain_type");
                }
            }
        }
        private domain_type _current_domain_type = domain_type.unspecified;

        /// <summary>
        /// If the VM is optimised for NUMA
        /// Experimental. First published in 26.2.0.
        /// </summary>
        public virtual bool numa_optimised
        {
            get { return _numa_optimised; }
            set
            {
                if (!Helper.AreEqual(value, _numa_optimised))
                {
                    _numa_optimised = value;
                    NotifyPropertyChanged("numa_optimised");
                }
            }
        }
        private bool _numa_optimised = false;

        /// <summary>
        /// number of NUMA nodes of the host the VM is using
        /// Experimental. First published in 26.2.0.
        /// </summary>
        public virtual long numa_nodes
        {
            get { return _numa_nodes; }
            set
            {
                if (!Helper.AreEqual(value, _numa_nodes))
                {
                    _numa_nodes = value;
                    NotifyPropertyChanged("numa_nodes");
                }
            }
        }
        private long _numa_nodes = 0;

        /// <summary>
        /// mapping a NUMA node (int) to an amount of memory (bytes) in that node.
        /// Experimental. First published in 26.2.0.
        /// </summary>
        public virtual Dictionary<long, long> numa_node_memory
        {
            get { return _numa_node_memory; }
            set
            {
                if (!Helper.AreEqual(value, _numa_node_memory))
                {
                    _numa_node_memory = value;
                    NotifyPropertyChanged("numa_node_memory");
                }
            }
        }
        private Dictionary<long, long> _numa_node_memory = new Dictionary<long, long>() {};
    }
}
