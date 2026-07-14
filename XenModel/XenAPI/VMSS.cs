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
    /// VM Snapshot Schedule
    /// First published in XenServer 7.2.
    /// </summary>
    public partial class VMSS : XenObject<VMSS>
    {
        #region Constructors

        public VMSS()
        {
        }

        public VMSS(string uuid,
            string name_label,
            string name_description,
            bool enabled,
            vmss_type type,
            long retained_snapshots,
            vmss_frequency frequency,
            Dictionary<string, string> schedule,
            DateTime last_run_time,
            List<XenRef<VM>> VMs)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.enabled = enabled;
            this.type = type;
            this.retained_snapshots = retained_snapshots;
            this.frequency = frequency;
            this.schedule = schedule;
            this.last_run_time = last_run_time;
            this.VMs = VMs;
        }

        /// <summary>
        /// Creates a new VMSS from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VMSS(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VMSS.
        /// </summary>
        public override void UpdateFrom(VMSS record)
        {
            uuid = record.uuid;
            name_label = record.name_label;
            name_description = record.name_description;
            enabled = record.enabled;
            type = record.type;
            retained_snapshots = record.retained_snapshots;
            frequency = record.frequency;
            schedule = record.schedule;
            last_run_time = record.last_run_time;
            VMs = record.VMs;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this VMSS
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
            if (table.ContainsKey("enabled"))
                enabled = Marshalling.ParseBool(table, "enabled");
            if (table.ContainsKey("type"))
                type = (vmss_type)Helper.EnumParseDefault(typeof(vmss_type), Marshalling.ParseString(table, "type"));
            if (table.ContainsKey("retained_snapshots"))
                retained_snapshots = Marshalling.ParseLong(table, "retained_snapshots");
            if (table.ContainsKey("frequency"))
                frequency = (vmss_frequency)Helper.EnumParseDefault(typeof(vmss_frequency), Marshalling.ParseString(table, "frequency"));
            if (table.ContainsKey("schedule"))
                schedule = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "schedule"));
            if (table.ContainsKey("last_run_time"))
                last_run_time = Marshalling.ParseDateTime(table, "last_run_time");
            if (table.ContainsKey("VMs"))
                VMs = Marshalling.ParseSetRef<VM>(table, "VMs");
        }

        public bool DeepEquals(VMSS other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name_label, other._name_label) &&
                Helper.AreEqual2(_name_description, other._name_description) &&
                Helper.AreEqual2(_enabled, other._enabled) &&
                Helper.AreEqual2(_type, other._type) &&
                Helper.AreEqual2(_retained_snapshots, other._retained_snapshots) &&
                Helper.AreEqual2(_frequency, other._frequency) &&
                Helper.AreEqual2(_schedule, other._schedule) &&
                Helper.AreEqual2(_last_run_time, other._last_run_time) &&
                Helper.AreEqual2(_VMs, other._VMs);
        }


        /// <summary>
        /// Get a record containing the current state of the given VMSS.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static VMSS get_record(Session session, string _vmss)
        {
            return session.JsonRpcClient.vmss_get_record(session.opaque_ref, _vmss);
        }

        /// <summary>
        /// Get a reference to the VMSS instance with the specified UUID.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VMSS> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.vmss_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Create a new VMSS instance, and return its handle.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<VMSS> create(Session session, VMSS _record)
        {
            return session.JsonRpcClient.vmss_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Create a new VMSS instance, and return its handle.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, VMSS _record)
        {
          return session.JsonRpcClient.async_vmss_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Destroy the specified VMSS instance.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _vmss)
        {
            session.JsonRpcClient.vmss_destroy(session.opaque_ref, _vmss);
        }

        /// <summary>
        /// Destroy the specified VMSS instance.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _vmss)
        {
          return session.JsonRpcClient.async_vmss_destroy(session.opaque_ref, _vmss);
        }

        /// <summary>
        /// Get all the VMSS instances with the given label.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VMSS>> get_by_name_label(Session session, string _label)
        {
            return session.JsonRpcClient.vmss_get_by_name_label(session.opaque_ref, _label);
        }

        /// <summary>
        /// Get the uuid field of the given VMSS.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _vmss)
        {
            return session.JsonRpcClient.vmss_get_uuid(session.opaque_ref, _vmss);
        }

        /// <summary>
        /// Get the name/label field of the given VMSS.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_label(Session session, string _vmss)
        {
            return session.JsonRpcClient.vmss_get_name_label(session.opaque_ref, _vmss);
        }

        /// <summary>
        /// Get the name/description field of the given VMSS.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_description(Session session, string _vmss)
        {
            return session.JsonRpcClient.vmss_get_name_description(session.opaque_ref, _vmss);
        }

        /// <summary>
        /// Get the enabled field of the given VMSS.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_enabled(Session session, string _vmss)
        {
            return session.JsonRpcClient.vmss_get_enabled(session.opaque_ref, _vmss);
        }

        /// <summary>
        /// Get the type field of the given VMSS.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static vmss_type get_type(Session session, string _vmss)
        {
            return session.JsonRpcClient.vmss_get_type(session.opaque_ref, _vmss);
        }

        /// <summary>
        /// Get the retained_snapshots field of the given VMSS.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_retained_snapshots(Session session, string _vmss)
        {
            return session.JsonRpcClient.vmss_get_retained_snapshots(session.opaque_ref, _vmss);
        }

        /// <summary>
        /// Get the frequency field of the given VMSS.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static vmss_frequency get_frequency(Session session, string _vmss)
        {
            return session.JsonRpcClient.vmss_get_frequency(session.opaque_ref, _vmss);
        }

        /// <summary>
        /// Get the schedule field of the given VMSS.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_schedule(Session session, string _vmss)
        {
            return session.JsonRpcClient.vmss_get_schedule(session.opaque_ref, _vmss);
        }

        /// <summary>
        /// Get the last_run_time field of the given VMSS.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_last_run_time(Session session, string _vmss)
        {
            return session.JsonRpcClient.vmss_get_last_run_time(session.opaque_ref, _vmss);
        }

        /// <summary>
        /// Get the VMs field of the given VMSS.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VM>> get_VMs(Session session, string _vmss)
        {
            return session.JsonRpcClient.vmss_get_vms(session.opaque_ref, _vmss);
        }

        /// <summary>
        /// Set the name/label field of the given VMSS.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_label">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_name_label(Session session, string _vmss, string _label)
        {
            session.JsonRpcClient.vmss_set_name_label(session.opaque_ref, _vmss, _label);
        }

        /// <summary>
        /// Set the name/description field of the given VMSS.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_description">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_name_description(Session session, string _vmss, string _description)
        {
            session.JsonRpcClient.vmss_set_name_description(session.opaque_ref, _vmss, _description);
        }

        /// <summary>
        /// Set the enabled field of the given VMSS.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_enabled">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_enabled(Session session, string _vmss, bool _enabled)
        {
            session.JsonRpcClient.vmss_set_enabled(session.opaque_ref, _vmss, _enabled);
        }

        /// <summary>
        /// This call executes the snapshot schedule immediately
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static string snapshot_now(Session session, string _vmss)
        {
            return session.JsonRpcClient.vmss_snapshot_now(session.opaque_ref, _vmss);
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">the value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_retained_snapshots(Session session, string _vmss, long _value)
        {
            session.JsonRpcClient.vmss_set_retained_snapshots(session.opaque_ref, _vmss, _value);
        }

        /// <summary>
        /// Set the value of the frequency field
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">the snapshot schedule frequency</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_frequency(Session session, string _vmss, vmss_frequency _value)
        {
            session.JsonRpcClient.vmss_set_frequency(session.opaque_ref, _vmss, _value);
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">the value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_schedule(Session session, string _vmss, Dictionary<string, string> _value)
        {
            session.JsonRpcClient.vmss_set_schedule(session.opaque_ref, _vmss, _value);
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_key">the key to add</param>
        /// <param name="_value">the value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_schedule(Session session, string _vmss, string _key, string _value)
        {
            session.JsonRpcClient.vmss_add_to_schedule(session.opaque_ref, _vmss, _key, _value);
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_key">the key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_schedule(Session session, string _vmss, string _key)
        {
            session.JsonRpcClient.vmss_remove_from_schedule(session.opaque_ref, _vmss, _key);
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">The time at which the schedule was last run. When the timezone is missing, UTC is assumed</param>
        /// <remarks>
        /// Minimum allowed role: Not Applicable
        /// </remarks>
        public static void set_last_run_time(Session session, string _vmss, DateTime _value)
        {
            session.JsonRpcClient.vmss_set_last_run_time(session.opaque_ref, _vmss, _value);
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">the snapshot schedule type</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_type(Session session, string _vmss, vmss_type _value)
        {
            session.JsonRpcClient.vmss_set_type(session.opaque_ref, _vmss, _value);
        }

        /// <summary>
        /// Return a list of all the VMSSs known to the system.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VMSS>> get_all(Session session)
        {
            return session.JsonRpcClient.vmss_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of VMSS references to VMSS records for all VMSSs known to the system.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<VMSS>, VMSS> get_all_records(Session session)
        {
            return session.JsonRpcClient.vmss_get_all_records(session.opaque_ref);
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
        /// enable or disable this snapshot schedule
        /// </summary>
        public virtual bool enabled
        {
            get { return _enabled; }
            set
            {
                if (!Helper.AreEqual(value, _enabled))
                {
                    _enabled = value;
                    NotifyPropertyChanged("enabled");
                }
            }
        }
        private bool _enabled = true;

        /// <summary>
        /// type of the snapshot schedule
        /// </summary>
        [JsonConverter(typeof(vmss_typeConverter))]
        public virtual vmss_type type
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
        private vmss_type _type;

        /// <summary>
        /// maximum number of snapshots that should be stored at any time
        /// </summary>
        public virtual long retained_snapshots
        {
            get { return _retained_snapshots; }
            set
            {
                if (!Helper.AreEqual(value, _retained_snapshots))
                {
                    _retained_snapshots = value;
                    NotifyPropertyChanged("retained_snapshots");
                }
            }
        }
        private long _retained_snapshots = 7;

        /// <summary>
        /// frequency of taking snapshot from snapshot schedule
        /// </summary>
        [JsonConverter(typeof(vmss_frequencyConverter))]
        public virtual vmss_frequency frequency
        {
            get { return _frequency; }
            set
            {
                if (!Helper.AreEqual(value, _frequency))
                {
                    _frequency = value;
                    NotifyPropertyChanged("frequency");
                }
            }
        }
        private vmss_frequency _frequency;

        /// <summary>
        /// schedule of the snapshot containing 'hour', 'min', 'days'. Date/time-related information is in Local Timezone
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> schedule
        {
            get { return _schedule; }
            set
            {
                if (!Helper.AreEqual(value, _schedule))
                {
                    _schedule = value;
                    NotifyPropertyChanged("schedule");
                }
            }
        }
        private Dictionary<string, string> _schedule = new Dictionary<string, string>() {};

        /// <summary>
        /// time of the last snapshot
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime last_run_time
        {
            get { return _last_run_time; }
            set
            {
                if (!Helper.AreEqual(value, _last_run_time))
                {
                    _last_run_time = value;
                    NotifyPropertyChanged("last_run_time");
                }
            }
        }
        private DateTime _last_run_time = DateTime.ParseExact("19700101T00:00:00Z", "yyyyMMddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        /// <summary>
        /// all VMs attached to this snapshot schedule
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VM>))]
        public virtual List<XenRef<VM>> VMs
        {
            get { return _VMs; }
            set
            {
                if (!Helper.AreEqual(value, _VMs))
                {
                    _VMs = value;
                    NotifyPropertyChanged("VMs");
                }
            }
        }
        private List<XenRef<VM>> _VMs = new List<XenRef<VM>>() {};
    }
}
