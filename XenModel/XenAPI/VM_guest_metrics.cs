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
    /// The metrics reported by the guest (as opposed to inferred from outside)
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class VM_guest_metrics : XenObject<VM_guest_metrics>
    {
        #region Constructors

        public VM_guest_metrics()
        {
        }

        public VM_guest_metrics(string uuid,
            Dictionary<string, string> os_version,
            Dictionary<string, string> netbios_name,
            Dictionary<string, string> PV_drivers_version,
            bool PV_drivers_up_to_date,
            Dictionary<string, string> memory,
            Dictionary<string, string> disks,
            Dictionary<string, string> networks,
            Dictionary<string, string> other,
            DateTime last_updated,
            Dictionary<string, string> other_config,
            bool live,
            tristate_type can_use_hotplug_vbd,
            tristate_type can_use_hotplug_vif,
            bool PV_drivers_detected,
            Dictionary<string, string> services)
        {
            this.uuid = uuid;
            this.os_version = os_version;
            this.netbios_name = netbios_name;
            this.PV_drivers_version = PV_drivers_version;
            this.PV_drivers_up_to_date = PV_drivers_up_to_date;
            this.memory = memory;
            this.disks = disks;
            this.networks = networks;
            this.other = other;
            this.last_updated = last_updated;
            this.other_config = other_config;
            this.live = live;
            this.can_use_hotplug_vbd = can_use_hotplug_vbd;
            this.can_use_hotplug_vif = can_use_hotplug_vif;
            this.PV_drivers_detected = PV_drivers_detected;
            this.services = services;
        }

        /// <summary>
        /// Creates a new VM_guest_metrics from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VM_guest_metrics(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VM_guest_metrics.
        /// </summary>
        public override void UpdateFrom(VM_guest_metrics record)
        {
            uuid = record.uuid;
            os_version = record.os_version;
            netbios_name = record.netbios_name;
            PV_drivers_version = record.PV_drivers_version;
            PV_drivers_up_to_date = record.PV_drivers_up_to_date;
            memory = record.memory;
            disks = record.disks;
            networks = record.networks;
            other = record.other;
            last_updated = record.last_updated;
            other_config = record.other_config;
            live = record.live;
            can_use_hotplug_vbd = record.can_use_hotplug_vbd;
            can_use_hotplug_vif = record.can_use_hotplug_vif;
            PV_drivers_detected = record.PV_drivers_detected;
            services = record.services;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this VM_guest_metrics
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("os_version"))
                os_version = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "os_version"));
            if (table.ContainsKey("netbios_name"))
                netbios_name = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "netbios_name"));
            if (table.ContainsKey("PV_drivers_version"))
                PV_drivers_version = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "PV_drivers_version"));
            if (table.ContainsKey("PV_drivers_up_to_date"))
                PV_drivers_up_to_date = Marshalling.ParseBool(table, "PV_drivers_up_to_date");
            if (table.ContainsKey("memory"))
                memory = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "memory"));
            if (table.ContainsKey("disks"))
                disks = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "disks"));
            if (table.ContainsKey("networks"))
                networks = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "networks"));
            if (table.ContainsKey("other"))
                other = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other"));
            if (table.ContainsKey("last_updated"))
                last_updated = Marshalling.ParseDateTime(table, "last_updated");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("live"))
                live = Marshalling.ParseBool(table, "live");
            if (table.ContainsKey("can_use_hotplug_vbd"))
                can_use_hotplug_vbd = (tristate_type)Helper.EnumParseDefault(typeof(tristate_type), Marshalling.ParseString(table, "can_use_hotplug_vbd"));
            if (table.ContainsKey("can_use_hotplug_vif"))
                can_use_hotplug_vif = (tristate_type)Helper.EnumParseDefault(typeof(tristate_type), Marshalling.ParseString(table, "can_use_hotplug_vif"));
            if (table.ContainsKey("PV_drivers_detected"))
                PV_drivers_detected = Marshalling.ParseBool(table, "PV_drivers_detected");
            if (table.ContainsKey("services"))
                services = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "services"));
        }

        public bool DeepEquals(VM_guest_metrics other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_os_version, other._os_version) &&
                Helper.AreEqual2(_netbios_name, other._netbios_name) &&
                Helper.AreEqual2(_PV_drivers_version, other._PV_drivers_version) &&
                Helper.AreEqual2(_PV_drivers_up_to_date, other._PV_drivers_up_to_date) &&
                Helper.AreEqual2(_memory, other._memory) &&
                Helper.AreEqual2(_disks, other._disks) &&
                Helper.AreEqual2(_networks, other._networks) &&
                Helper.AreEqual2(_other, other._other) &&
                Helper.AreEqual2(_last_updated, other._last_updated) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_live, other._live) &&
                Helper.AreEqual2(_can_use_hotplug_vbd, other._can_use_hotplug_vbd) &&
                Helper.AreEqual2(_can_use_hotplug_vif, other._can_use_hotplug_vif) &&
                Helper.AreEqual2(_PV_drivers_detected, other._PV_drivers_detected) &&
                Helper.AreEqual2(_services, other._services);
        }


        /// <summary>
        /// Get a record containing the current state of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static VM_guest_metrics get_record(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_record(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Get a reference to the VM_guest_metrics instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VM_guest_metrics> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_uuid(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Get the os_version field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_os_version(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_os_version(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Get the netbios_name field of the given VM_guest_metrics.
        /// Experimental. First published in 24.28.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_netbios_name(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_netbios_name(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Get the PV_drivers_version field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_PV_drivers_version(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_pv_drivers_version(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Get the PV_drivers_up_to_date field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        [Deprecated("XenServer 7.0")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_PV_drivers_up_to_date(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_pv_drivers_up_to_date(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Get the memory field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        [Deprecated("XenServer 5.5")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_memory(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_memory(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Get the disks field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        [Deprecated("XenServer 5.0")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_disks(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_disks(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Get the networks field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_networks(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_networks(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Get the other field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_other(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Get the last_updated field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_last_updated(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_last_updated(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Get the other_config field of the given VM_guest_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_other_config(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Get the live field of the given VM_guest_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_live(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_live(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Get the can_use_hotplug_vbd field of the given VM_guest_metrics.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static tristate_type get_can_use_hotplug_vbd(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_can_use_hotplug_vbd(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Get the can_use_hotplug_vif field of the given VM_guest_metrics.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static tristate_type get_can_use_hotplug_vif(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_can_use_hotplug_vif(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Get the PV_drivers_detected field of the given VM_guest_metrics.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_PV_drivers_detected(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_pv_drivers_detected(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Get the services field of the given VM_guest_metrics.
        /// Experimental. First published in 25.15.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_services(Session session, string _vm_guest_metrics)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_services(session.opaque_ref, _vm_guest_metrics);
        }

        /// <summary>
        /// Set the other_config field of the given VM_guest_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_other_config(Session session, string _vm_guest_metrics, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.vm_guest_metrics_set_other_config(session.opaque_ref, _vm_guest_metrics, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given VM_guest_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void add_to_other_config(Session session, string _vm_guest_metrics, string _key, string _value)
        {
            session.JsonRpcClient.vm_guest_metrics_add_to_other_config(session.opaque_ref, _vm_guest_metrics, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given VM_guest_metrics.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void remove_from_other_config(Session session, string _vm_guest_metrics, string _key)
        {
            session.JsonRpcClient.vm_guest_metrics_remove_from_other_config(session.opaque_ref, _vm_guest_metrics, _key);
        }

        /// <summary>
        /// Return a list of all the VM_guest_metrics instances known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VM_guest_metrics>> get_all(Session session)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of VM_guest_metrics references to VM_guest_metrics records for all VM_guest_metrics instances known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<VM_guest_metrics>, VM_guest_metrics> get_all_records(Session session)
        {
            return session.JsonRpcClient.vm_guest_metrics_get_all_records(session.opaque_ref);
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
        /// version of the OS
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> os_version
        {
            get { return _os_version; }
            set
            {
                if (!Helper.AreEqual(value, _os_version))
                {
                    _os_version = value;
                    NotifyPropertyChanged("os_version");
                }
            }
        }
        private Dictionary<string, string> _os_version = new Dictionary<string, string>() {};

        /// <summary>
        /// The NETBIOS name of the machine
        /// Experimental. First published in 24.28.0.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> netbios_name
        {
            get { return _netbios_name; }
            set
            {
                if (!Helper.AreEqual(value, _netbios_name))
                {
                    _netbios_name = value;
                    NotifyPropertyChanged("netbios_name");
                }
            }
        }
        private Dictionary<string, string> _netbios_name = new Dictionary<string, string>() {};

        /// <summary>
        /// version of the PV drivers
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> PV_drivers_version
        {
            get { return _PV_drivers_version; }
            set
            {
                if (!Helper.AreEqual(value, _PV_drivers_version))
                {
                    _PV_drivers_version = value;
                    NotifyPropertyChanged("PV_drivers_version");
                }
            }
        }
        private Dictionary<string, string> _PV_drivers_version = new Dictionary<string, string>() {};

        /// <summary>
        /// Logically equivalent to PV_drivers_detected
        /// </summary>
        public virtual bool PV_drivers_up_to_date
        {
            get { return _PV_drivers_up_to_date; }
            set
            {
                if (!Helper.AreEqual(value, _PV_drivers_up_to_date))
                {
                    _PV_drivers_up_to_date = value;
                    NotifyPropertyChanged("PV_drivers_up_to_date");
                }
            }
        }
        private bool _PV_drivers_up_to_date;

        /// <summary>
        /// This field exists but has no data. Use the memory and memory_internal_free RRD data-sources instead.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> memory
        {
            get { return _memory; }
            set
            {
                if (!Helper.AreEqual(value, _memory))
                {
                    _memory = value;
                    NotifyPropertyChanged("memory");
                }
            }
        }
        private Dictionary<string, string> _memory = new Dictionary<string, string>() {};

        /// <summary>
        /// This field exists but has no data.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> disks
        {
            get { return _disks; }
            set
            {
                if (!Helper.AreEqual(value, _disks))
                {
                    _disks = value;
                    NotifyPropertyChanged("disks");
                }
            }
        }
        private Dictionary<string, string> _disks = new Dictionary<string, string>() {};

        /// <summary>
        /// network configuration
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> networks
        {
            get { return _networks; }
            set
            {
                if (!Helper.AreEqual(value, _networks))
                {
                    _networks = value;
                    NotifyPropertyChanged("networks");
                }
            }
        }
        private Dictionary<string, string> _networks = new Dictionary<string, string>() {};

        /// <summary>
        /// anything else
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> other
        {
            get { return _other; }
            set
            {
                if (!Helper.AreEqual(value, _other))
                {
                    _other = value;
                    NotifyPropertyChanged("other");
                }
            }
        }
        private Dictionary<string, string> _other = new Dictionary<string, string>() {};

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
        /// True if the guest is sending heartbeat messages via the guest agent
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual bool live
        {
            get { return _live; }
            set
            {
                if (!Helper.AreEqual(value, _live))
                {
                    _live = value;
                    NotifyPropertyChanged("live");
                }
            }
        }
        private bool _live = false;

        /// <summary>
        /// The guest's statement of whether it supports VBD hotplug, i.e. whether it is capable of responding immediately to instantiation of a new VBD by bringing online a new PV block device. If the guest states that it is not capable, then the VBD plug and unplug operations will not be allowed while the guest is running.
        /// First published in XenServer 7.0.
        /// </summary>
        [JsonConverter(typeof(tristate_typeConverter))]
        public virtual tristate_type can_use_hotplug_vbd
        {
            get { return _can_use_hotplug_vbd; }
            set
            {
                if (!Helper.AreEqual(value, _can_use_hotplug_vbd))
                {
                    _can_use_hotplug_vbd = value;
                    NotifyPropertyChanged("can_use_hotplug_vbd");
                }
            }
        }
        private tristate_type _can_use_hotplug_vbd = tristate_type.unspecified;

        /// <summary>
        /// The guest's statement of whether it supports VIF hotplug, i.e. whether it is capable of responding immediately to instantiation of a new VIF by bringing online a new PV network device. If the guest states that it is not capable, then the VIF plug and unplug operations will not be allowed while the guest is running.
        /// First published in XenServer 7.0.
        /// </summary>
        [JsonConverter(typeof(tristate_typeConverter))]
        public virtual tristate_type can_use_hotplug_vif
        {
            get { return _can_use_hotplug_vif; }
            set
            {
                if (!Helper.AreEqual(value, _can_use_hotplug_vif))
                {
                    _can_use_hotplug_vif = value;
                    NotifyPropertyChanged("can_use_hotplug_vif");
                }
            }
        }
        private tristate_type _can_use_hotplug_vif = tristate_type.unspecified;

        /// <summary>
        /// At least one of the guest's devices has successfully connected to the backend.
        /// First published in XenServer 7.0.
        /// </summary>
        public virtual bool PV_drivers_detected
        {
            get { return _PV_drivers_detected; }
            set
            {
                if (!Helper.AreEqual(value, _PV_drivers_detected))
                {
                    _PV_drivers_detected = value;
                    NotifyPropertyChanged("PV_drivers_detected");
                }
            }
        }
        private bool _PV_drivers_detected = false;

        /// <summary>
        /// The guest's services data.
        /// Experimental. First published in 25.15.0.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> services
        {
            get { return _services; }
            set
            {
                if (!Helper.AreEqual(value, _services))
                {
                    _services = value;
                    NotifyPropertyChanged("services");
                }
            }
        }
        private Dictionary<string, string> _services = new Dictionary<string, string>() {};
    }
}
