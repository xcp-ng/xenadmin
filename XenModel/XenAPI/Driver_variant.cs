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
    /// UNSUPPORTED. Variant of a host driver
    /// First published in .
    /// </summary>
    public partial class Driver_variant : XenObject<Driver_variant>
    {
        #region Constructors

        public Driver_variant()
        {
        }

        public Driver_variant(string uuid,
            string name,
            XenRef<Host_driver> driver,
            string version,
            bool hardware_present,
            double priority,
            string status)
        {
            this.uuid = uuid;
            this.name = name;
            this.driver = driver;
            this.version = version;
            this.hardware_present = hardware_present;
            this.priority = priority;
            this.status = status;
        }

        /// <summary>
        /// Creates a new Driver_variant from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Driver_variant(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Driver_variant.
        /// </summary>
        public override void UpdateFrom(Driver_variant record)
        {
            uuid = record.uuid;
            name = record.name;
            driver = record.driver;
            version = record.version;
            hardware_present = record.hardware_present;
            priority = record.priority;
            status = record.status;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Driver_variant
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("name"))
                name = Marshalling.ParseString(table, "name");
            if (table.ContainsKey("driver"))
                driver = Marshalling.ParseRef<Host_driver>(table, "driver");
            if (table.ContainsKey("version"))
                version = Marshalling.ParseString(table, "version");
            if (table.ContainsKey("hardware_present"))
                hardware_present = Marshalling.ParseBool(table, "hardware_present");
            if (table.ContainsKey("priority"))
                priority = Marshalling.ParseDouble(table, "priority");
            if (table.ContainsKey("status"))
                status = Marshalling.ParseString(table, "status");
        }

        public bool DeepEquals(Driver_variant other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name, other._name) &&
                Helper.AreEqual2(_driver, other._driver) &&
                Helper.AreEqual2(_version, other._version) &&
                Helper.AreEqual2(_hardware_present, other._hardware_present) &&
                Helper.AreEqual2(_priority, other._priority) &&
                Helper.AreEqual2(_status, other._status);
        }


        /// <summary>
        /// Get a record containing the current state of the given Driver_variant.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_driver_variant">The opaque_ref of the given driver_variant</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Driver_variant get_record(Session session, string _driver_variant)
        {
            return session.JsonRpcClient.driver_variant_get_record(session.opaque_ref, _driver_variant);
        }

        /// <summary>
        /// Get a reference to the Driver_variant instance with the specified UUID.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Driver_variant> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.driver_variant_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given Driver_variant.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_driver_variant">The opaque_ref of the given driver_variant</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _driver_variant)
        {
            return session.JsonRpcClient.driver_variant_get_uuid(session.opaque_ref, _driver_variant);
        }

        /// <summary>
        /// Get the name field of the given Driver_variant.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_driver_variant">The opaque_ref of the given driver_variant</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name(Session session, string _driver_variant)
        {
            return session.JsonRpcClient.driver_variant_get_name(session.opaque_ref, _driver_variant);
        }

        /// <summary>
        /// Get the driver field of the given Driver_variant.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_driver_variant">The opaque_ref of the given driver_variant</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host_driver> get_driver(Session session, string _driver_variant)
        {
            return session.JsonRpcClient.driver_variant_get_driver(session.opaque_ref, _driver_variant);
        }

        /// <summary>
        /// Get the version field of the given Driver_variant.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_driver_variant">The opaque_ref of the given driver_variant</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_version(Session session, string _driver_variant)
        {
            return session.JsonRpcClient.driver_variant_get_version(session.opaque_ref, _driver_variant);
        }

        /// <summary>
        /// Get the hardware_present field of the given Driver_variant.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_driver_variant">The opaque_ref of the given driver_variant</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_hardware_present(Session session, string _driver_variant)
        {
            return session.JsonRpcClient.driver_variant_get_hardware_present(session.opaque_ref, _driver_variant);
        }

        /// <summary>
        /// Get the priority field of the given Driver_variant.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_driver_variant">The opaque_ref of the given driver_variant</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static double get_priority(Session session, string _driver_variant)
        {
            return session.JsonRpcClient.driver_variant_get_priority(session.opaque_ref, _driver_variant);
        }

        /// <summary>
        /// Get the status field of the given Driver_variant.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_driver_variant">The opaque_ref of the given driver_variant</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_status(Session session, string _driver_variant)
        {
            return session.JsonRpcClient.driver_variant_get_status(session.opaque_ref, _driver_variant);
        }

        /// <summary>
        /// UNSUPPORTED Select this variant of a driver to become active after reboot or immediately if currently no version is active
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_driver_variant">The opaque_ref of the given driver_variant</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void select(Session session, string _driver_variant)
        {
            session.JsonRpcClient.driver_variant_select(session.opaque_ref, _driver_variant);
        }

        /// <summary>
        /// UNSUPPORTED Select this variant of a driver to become active after reboot or immediately if currently no version is active
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_driver_variant">The opaque_ref of the given driver_variant</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_select(Session session, string _driver_variant)
        {
          return session.JsonRpcClient.async_driver_variant_select(session.opaque_ref, _driver_variant);
        }

        /// <summary>
        /// Return a list of all the Driver_variants known to the system.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Driver_variant>> get_all(Session session)
        {
            return session.JsonRpcClient.driver_variant_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of Driver_variant references to Driver_variant records for all Driver_variants known to the system.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Driver_variant>, Driver_variant> get_all_records(Session session)
        {
            return session.JsonRpcClient.driver_variant_get_all_records(session.opaque_ref);
        }

        /// <summary>
        /// Unique identifier/object reference
        /// Experimental. First published in 25.2.0.
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
        /// Name identifying the driver variant within the driver
        /// Experimental. First published in 25.2.0.
        /// </summary>
        public virtual string name
        {
            get { return _name; }
            set
            {
                if (!Helper.AreEqual(value, _name))
                {
                    _name = value;
                    NotifyPropertyChanged("name");
                }
            }
        }
        private string _name = "";

        /// <summary>
        /// Driver this variant is a part of
        /// Experimental. First published in 25.2.0.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Host_driver>))]
        public virtual XenRef<Host_driver> driver
        {
            get { return _driver; }
            set
            {
                if (!Helper.AreEqual(value, _driver))
                {
                    _driver = value;
                    NotifyPropertyChanged("driver");
                }
            }
        }
        private XenRef<Host_driver> _driver = new XenRef<Host_driver>(Helper.NullOpaqueRef);

        /// <summary>
        /// Unique version of this driver variant
        /// Experimental. First published in 25.2.0.
        /// </summary>
        public virtual string version
        {
            get { return _version; }
            set
            {
                if (!Helper.AreEqual(value, _version))
                {
                    _version = value;
                    NotifyPropertyChanged("version");
                }
            }
        }
        private string _version = "";

        /// <summary>
        /// True if the hardware for this variant is present on the host
        /// Experimental. First published in 25.2.0.
        /// </summary>
        public virtual bool hardware_present
        {
            get { return _hardware_present; }
            set
            {
                if (!Helper.AreEqual(value, _hardware_present))
                {
                    _hardware_present = value;
                    NotifyPropertyChanged("hardware_present");
                }
            }
        }
        private bool _hardware_present;

        /// <summary>
        /// Priority; this needs an explanation how this is ordered
        /// Experimental. First published in 25.2.0.
        /// </summary>
        public virtual double priority
        {
            get { return _priority; }
            set
            {
                if (!Helper.AreEqual(value, _priority))
                {
                    _priority = value;
                    NotifyPropertyChanged("priority");
                }
            }
        }
        private double _priority;

        /// <summary>
        /// Development and release status of this variant, like 'alpha'
        /// Experimental. First published in 25.2.0.
        /// </summary>
        public virtual string status
        {
            get { return _status; }
            set
            {
                if (!Helper.AreEqual(value, _status))
                {
                    _status = value;
                    NotifyPropertyChanged("status");
                }
            }
        }
        private string _status = "";
    }
}
