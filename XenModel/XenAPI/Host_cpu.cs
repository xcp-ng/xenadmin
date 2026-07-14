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
    /// A physical CPU
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class Host_cpu : XenObject<Host_cpu>
    {
        #region Constructors

        public Host_cpu()
        {
        }

        public Host_cpu(string uuid,
            XenRef<Host> host,
            long number,
            string vendor,
            long speed,
            string modelname,
            long family,
            long model,
            string stepping,
            string flags,
            string features,
            double utilisation,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.host = host;
            this.number = number;
            this.vendor = vendor;
            this.speed = speed;
            this.modelname = modelname;
            this.family = family;
            this.model = model;
            this.stepping = stepping;
            this.flags = flags;
            this.features = features;
            this.utilisation = utilisation;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Host_cpu from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Host_cpu(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Host_cpu.
        /// </summary>
        public override void UpdateFrom(Host_cpu record)
        {
            uuid = record.uuid;
            host = record.host;
            number = record.number;
            vendor = record.vendor;
            speed = record.speed;
            modelname = record.modelname;
            family = record.family;
            model = record.model;
            stepping = record.stepping;
            flags = record.flags;
            features = record.features;
            utilisation = record.utilisation;
            other_config = record.other_config;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Host_cpu
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
            if (table.ContainsKey("number"))
                number = Marshalling.ParseLong(table, "number");
            if (table.ContainsKey("vendor"))
                vendor = Marshalling.ParseString(table, "vendor");
            if (table.ContainsKey("speed"))
                speed = Marshalling.ParseLong(table, "speed");
            if (table.ContainsKey("modelname"))
                modelname = Marshalling.ParseString(table, "modelname");
            if (table.ContainsKey("family"))
                family = Marshalling.ParseLong(table, "family");
            if (table.ContainsKey("model"))
                model = Marshalling.ParseLong(table, "model");
            if (table.ContainsKey("stepping"))
                stepping = Marshalling.ParseString(table, "stepping");
            if (table.ContainsKey("flags"))
                flags = Marshalling.ParseString(table, "flags");
            if (table.ContainsKey("features"))
                features = Marshalling.ParseString(table, "features");
            if (table.ContainsKey("utilisation"))
                utilisation = Marshalling.ParseDouble(table, "utilisation");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Host_cpu other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_host, other._host) &&
                Helper.AreEqual2(_number, other._number) &&
                Helper.AreEqual2(_vendor, other._vendor) &&
                Helper.AreEqual2(_speed, other._speed) &&
                Helper.AreEqual2(_modelname, other._modelname) &&
                Helper.AreEqual2(_family, other._family) &&
                Helper.AreEqual2(_model, other._model) &&
                Helper.AreEqual2(_stepping, other._stepping) &&
                Helper.AreEqual2(_flags, other._flags) &&
                Helper.AreEqual2(_features, other._features) &&
                Helper.AreEqual2(_utilisation, other._utilisation) &&
                Helper.AreEqual2(_other_config, other._other_config);
        }


        /// <summary>
        /// Get a record containing the current state of the given host_cpu.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        [Deprecated("XenServer 5.6")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Host_cpu get_record(Session session, string _host_cpu)
        {
            return session.JsonRpcClient.host_cpu_get_record(session.opaque_ref, _host_cpu);
        }

        /// <summary>
        /// Get a reference to the host_cpu instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        [Deprecated("XenServer 5.6")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host_cpu> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.host_cpu_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _host_cpu)
        {
            return session.JsonRpcClient.host_cpu_get_uuid(session.opaque_ref, _host_cpu);
        }

        /// <summary>
        /// Get the host field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host> get_host(Session session, string _host_cpu)
        {
            return session.JsonRpcClient.host_cpu_get_host(session.opaque_ref, _host_cpu);
        }

        /// <summary>
        /// Get the number field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_number(Session session, string _host_cpu)
        {
            return session.JsonRpcClient.host_cpu_get_number(session.opaque_ref, _host_cpu);
        }

        /// <summary>
        /// Get the vendor field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_vendor(Session session, string _host_cpu)
        {
            return session.JsonRpcClient.host_cpu_get_vendor(session.opaque_ref, _host_cpu);
        }

        /// <summary>
        /// Get the speed field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_speed(Session session, string _host_cpu)
        {
            return session.JsonRpcClient.host_cpu_get_speed(session.opaque_ref, _host_cpu);
        }

        /// <summary>
        /// Get the modelname field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_modelname(Session session, string _host_cpu)
        {
            return session.JsonRpcClient.host_cpu_get_modelname(session.opaque_ref, _host_cpu);
        }

        /// <summary>
        /// Get the family field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_family(Session session, string _host_cpu)
        {
            return session.JsonRpcClient.host_cpu_get_family(session.opaque_ref, _host_cpu);
        }

        /// <summary>
        /// Get the model field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_model(Session session, string _host_cpu)
        {
            return session.JsonRpcClient.host_cpu_get_model(session.opaque_ref, _host_cpu);
        }

        /// <summary>
        /// Get the stepping field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_stepping(Session session, string _host_cpu)
        {
            return session.JsonRpcClient.host_cpu_get_stepping(session.opaque_ref, _host_cpu);
        }

        /// <summary>
        /// Get the flags field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_flags(Session session, string _host_cpu)
        {
            return session.JsonRpcClient.host_cpu_get_flags(session.opaque_ref, _host_cpu);
        }

        /// <summary>
        /// Get the features field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_features(Session session, string _host_cpu)
        {
            return session.JsonRpcClient.host_cpu_get_features(session.opaque_ref, _host_cpu);
        }

        /// <summary>
        /// Get the utilisation field of the given host_cpu.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static double get_utilisation(Session session, string _host_cpu)
        {
            return session.JsonRpcClient.host_cpu_get_utilisation(session.opaque_ref, _host_cpu);
        }

        /// <summary>
        /// Get the other_config field of the given host_cpu.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _host_cpu)
        {
            return session.JsonRpcClient.host_cpu_get_other_config(session.opaque_ref, _host_cpu);
        }

        /// <summary>
        /// Set the other_config field of the given host_cpu.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_other_config(Session session, string _host_cpu, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.host_cpu_set_other_config(session.opaque_ref, _host_cpu, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given host_cpu.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_other_config(Session session, string _host_cpu, string _key, string _value)
        {
            session.JsonRpcClient.host_cpu_add_to_other_config(session.opaque_ref, _host_cpu, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given host_cpu.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_cpu">The opaque_ref of the given host_cpu</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_other_config(Session session, string _host_cpu, string _key)
        {
            session.JsonRpcClient.host_cpu_remove_from_other_config(session.opaque_ref, _host_cpu, _key);
        }

        /// <summary>
        /// Return a list of all the host_cpus known to the system.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        [Deprecated("XenServer 5.6")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Host_cpu>> get_all(Session session)
        {
            return session.JsonRpcClient.host_cpu_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of host_cpu references to host_cpu records for all host_cpus known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Host_cpu>, Host_cpu> get_all_records(Session session)
        {
            return session.JsonRpcClient.host_cpu_get_all_records(session.opaque_ref);
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
        /// the host the CPU is in
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
                    NotifyPropertyChanged("host");
                }
            }
        }
        private XenRef<Host> _host = new XenRef<Host>(Helper.NullOpaqueRef);

        /// <summary>
        /// the number of the physical CPU within the host
        /// </summary>
        public virtual long number
        {
            get { return _number; }
            set
            {
                if (!Helper.AreEqual(value, _number))
                {
                    _number = value;
                    NotifyPropertyChanged("number");
                }
            }
        }
        private long _number;

        /// <summary>
        /// the vendor of the physical CPU
        /// </summary>
        public virtual string vendor
        {
            get { return _vendor; }
            set
            {
                if (!Helper.AreEqual(value, _vendor))
                {
                    _vendor = value;
                    NotifyPropertyChanged("vendor");
                }
            }
        }
        private string _vendor = "";

        /// <summary>
        /// the speed of the physical CPU
        /// </summary>
        public virtual long speed
        {
            get { return _speed; }
            set
            {
                if (!Helper.AreEqual(value, _speed))
                {
                    _speed = value;
                    NotifyPropertyChanged("speed");
                }
            }
        }
        private long _speed;

        /// <summary>
        /// the model name of the physical CPU
        /// </summary>
        public virtual string modelname
        {
            get { return _modelname; }
            set
            {
                if (!Helper.AreEqual(value, _modelname))
                {
                    _modelname = value;
                    NotifyPropertyChanged("modelname");
                }
            }
        }
        private string _modelname = "";

        /// <summary>
        /// the family (number) of the physical CPU
        /// </summary>
        public virtual long family
        {
            get { return _family; }
            set
            {
                if (!Helper.AreEqual(value, _family))
                {
                    _family = value;
                    NotifyPropertyChanged("family");
                }
            }
        }
        private long _family;

        /// <summary>
        /// the model number of the physical CPU
        /// </summary>
        public virtual long model
        {
            get { return _model; }
            set
            {
                if (!Helper.AreEqual(value, _model))
                {
                    _model = value;
                    NotifyPropertyChanged("model");
                }
            }
        }
        private long _model;

        /// <summary>
        /// the stepping of the physical CPU
        /// </summary>
        public virtual string stepping
        {
            get { return _stepping; }
            set
            {
                if (!Helper.AreEqual(value, _stepping))
                {
                    _stepping = value;
                    NotifyPropertyChanged("stepping");
                }
            }
        }
        private string _stepping = "";

        /// <summary>
        /// the flags of the physical CPU (a decoded version of the features field)
        /// </summary>
        public virtual string flags
        {
            get { return _flags; }
            set
            {
                if (!Helper.AreEqual(value, _flags))
                {
                    _flags = value;
                    NotifyPropertyChanged("flags");
                }
            }
        }
        private string _flags = "";

        /// <summary>
        /// the physical CPU feature bitmap
        /// </summary>
        public virtual string features
        {
            get { return _features; }
            set
            {
                if (!Helper.AreEqual(value, _features))
                {
                    _features = value;
                    NotifyPropertyChanged("features");
                }
            }
        }
        private string _features = "";

        /// <summary>
        /// the current CPU utilisation
        /// </summary>
        public virtual double utilisation
        {
            get { return _utilisation; }
            set
            {
                if (!Helper.AreEqual(value, _utilisation))
                {
                    _utilisation = value;
                    NotifyPropertyChanged("utilisation");
                }
            }
        }
        private double _utilisation;

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
    }
}
