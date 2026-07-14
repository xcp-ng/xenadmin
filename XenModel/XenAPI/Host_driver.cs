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
    /// UNSUPPORTED. A multi-version driver on a host
    /// First published in .
    /// </summary>
    public partial class Host_driver : XenObject<Host_driver>
    {
        #region Constructors

        public Host_driver()
        {
        }

        public Host_driver(string uuid,
            XenRef<Host> host,
            string name,
            string friendly_name,
            List<XenRef<Driver_variant>> variants,
            XenRef<Driver_variant> active_variant,
            XenRef<Driver_variant> selected_variant,
            string type,
            string description,
            string info)
        {
            this.uuid = uuid;
            this.host = host;
            this.name = name;
            this.friendly_name = friendly_name;
            this.variants = variants;
            this.active_variant = active_variant;
            this.selected_variant = selected_variant;
            this.type = type;
            this.description = description;
            this.info = info;
        }

        /// <summary>
        /// Creates a new Host_driver from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Host_driver(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Host_driver.
        /// </summary>
        public override void UpdateFrom(Host_driver record)
        {
            uuid = record.uuid;
            host = record.host;
            name = record.name;
            friendly_name = record.friendly_name;
            variants = record.variants;
            active_variant = record.active_variant;
            selected_variant = record.selected_variant;
            type = record.type;
            description = record.description;
            info = record.info;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Host_driver
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
            if (table.ContainsKey("name"))
                name = Marshalling.ParseString(table, "name");
            if (table.ContainsKey("friendly_name"))
                friendly_name = Marshalling.ParseString(table, "friendly_name");
            if (table.ContainsKey("variants"))
                variants = Marshalling.ParseSetRef<Driver_variant>(table, "variants");
            if (table.ContainsKey("active_variant"))
                active_variant = Marshalling.ParseRef<Driver_variant>(table, "active_variant");
            if (table.ContainsKey("selected_variant"))
                selected_variant = Marshalling.ParseRef<Driver_variant>(table, "selected_variant");
            if (table.ContainsKey("type"))
                type = Marshalling.ParseString(table, "type");
            if (table.ContainsKey("description"))
                description = Marshalling.ParseString(table, "description");
            if (table.ContainsKey("info"))
                info = Marshalling.ParseString(table, "info");
        }

        public bool DeepEquals(Host_driver other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_host, other._host) &&
                Helper.AreEqual2(_name, other._name) &&
                Helper.AreEqual2(_friendly_name, other._friendly_name) &&
                Helper.AreEqual2(_variants, other._variants) &&
                Helper.AreEqual2(_active_variant, other._active_variant) &&
                Helper.AreEqual2(_selected_variant, other._selected_variant) &&
                Helper.AreEqual2(_type, other._type) &&
                Helper.AreEqual2(_description, other._description) &&
                Helper.AreEqual2(_info, other._info);
        }


        /// <summary>
        /// Get a record containing the current state of the given Host_driver.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_driver">The opaque_ref of the given host_driver</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Host_driver get_record(Session session, string _host_driver)
        {
            return session.JsonRpcClient.host_driver_get_record(session.opaque_ref, _host_driver);
        }

        /// <summary>
        /// Get a reference to the Host_driver instance with the specified UUID.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host_driver> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.host_driver_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given Host_driver.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_driver">The opaque_ref of the given host_driver</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _host_driver)
        {
            return session.JsonRpcClient.host_driver_get_uuid(session.opaque_ref, _host_driver);
        }

        /// <summary>
        /// Get the host field of the given Host_driver.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_driver">The opaque_ref of the given host_driver</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host> get_host(Session session, string _host_driver)
        {
            return session.JsonRpcClient.host_driver_get_host(session.opaque_ref, _host_driver);
        }

        /// <summary>
        /// Get the name field of the given Host_driver.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_driver">The opaque_ref of the given host_driver</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name(Session session, string _host_driver)
        {
            return session.JsonRpcClient.host_driver_get_name(session.opaque_ref, _host_driver);
        }

        /// <summary>
        /// Get the friendly_name field of the given Host_driver.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_driver">The opaque_ref of the given host_driver</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_friendly_name(Session session, string _host_driver)
        {
            return session.JsonRpcClient.host_driver_get_friendly_name(session.opaque_ref, _host_driver);
        }

        /// <summary>
        /// Get the variants field of the given Host_driver.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_driver">The opaque_ref of the given host_driver</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Driver_variant>> get_variants(Session session, string _host_driver)
        {
            return session.JsonRpcClient.host_driver_get_variants(session.opaque_ref, _host_driver);
        }

        /// <summary>
        /// Get the active_variant field of the given Host_driver.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_driver">The opaque_ref of the given host_driver</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Driver_variant> get_active_variant(Session session, string _host_driver)
        {
            return session.JsonRpcClient.host_driver_get_active_variant(session.opaque_ref, _host_driver);
        }

        /// <summary>
        /// Get the selected_variant field of the given Host_driver.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_driver">The opaque_ref of the given host_driver</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Driver_variant> get_selected_variant(Session session, string _host_driver)
        {
            return session.JsonRpcClient.host_driver_get_selected_variant(session.opaque_ref, _host_driver);
        }

        /// <summary>
        /// Get the type field of the given Host_driver.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_driver">The opaque_ref of the given host_driver</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_type(Session session, string _host_driver)
        {
            return session.JsonRpcClient.host_driver_get_type(session.opaque_ref, _host_driver);
        }

        /// <summary>
        /// Get the description field of the given Host_driver.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_driver">The opaque_ref of the given host_driver</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_description(Session session, string _host_driver)
        {
            return session.JsonRpcClient.host_driver_get_description(session.opaque_ref, _host_driver);
        }

        /// <summary>
        /// Get the info field of the given Host_driver.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_driver">The opaque_ref of the given host_driver</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_info(Session session, string _host_driver)
        {
            return session.JsonRpcClient.host_driver_get_info(session.opaque_ref, _host_driver);
        }

        /// <summary>
        /// UNSUPPORTED. Select a variant of the driver to become active after reboot or immediately if currently no version is active
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_driver">The opaque_ref of the given host_driver</param>
        /// <param name="_variant">Driver version to become active (after reboot).</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void select(Session session, string _host_driver, string _variant)
        {
            session.JsonRpcClient.host_driver_select(session.opaque_ref, _host_driver, _variant);
        }

        /// <summary>
        /// UNSUPPORTED. Select a variant of the driver to become active after reboot or immediately if currently no version is active
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_driver">The opaque_ref of the given host_driver</param>
        /// <param name="_variant">Driver version to become active (after reboot).</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_select(Session session, string _host_driver, string _variant)
        {
          return session.JsonRpcClient.async_host_driver_select(session.opaque_ref, _host_driver, _variant);
        }

        /// <summary>
        /// UNSUPPORTED. Deselect the currently active variant of this driver after reboot. No action will be taken if no variant is currently active.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_driver">The opaque_ref of the given host_driver</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void deselect(Session session, string _host_driver)
        {
            session.JsonRpcClient.host_driver_deselect(session.opaque_ref, _host_driver);
        }

        /// <summary>
        /// UNSUPPORTED. Deselect the currently active variant of this driver after reboot. No action will be taken if no variant is currently active.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_driver">The opaque_ref of the given host_driver</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_deselect(Session session, string _host_driver)
        {
          return session.JsonRpcClient.async_host_driver_deselect(session.opaque_ref, _host_driver);
        }

        /// <summary>
        /// UNSUPPORTED. Re-scan a host's drivers and update information about them. This is mostly  for trouble shooting.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">Update driver information of this host.</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void rescan(Session session, string _host)
        {
            session.JsonRpcClient.host_driver_rescan(session.opaque_ref, _host);
        }

        /// <summary>
        /// UNSUPPORTED. Re-scan a host's drivers and update information about them. This is mostly  for trouble shooting.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">Update driver information of this host.</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_rescan(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_driver_rescan(session.opaque_ref, _host);
        }

        /// <summary>
        /// Return a list of all the Host_drivers known to the system.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Host_driver>> get_all(Session session)
        {
            return session.JsonRpcClient.host_driver_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of Host_driver references to Host_driver records for all Host_drivers known to the system.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Host_driver>, Host_driver> get_all_records(Session session)
        {
            return session.JsonRpcClient.host_driver_get_all_records(session.opaque_ref);
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
        /// Host where this driver is installed
        /// Experimental. First published in 25.2.0.
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
        /// Name identifying the driver uniquely
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
        /// Descriptive name, not used for identification
        /// Experimental. First published in 25.2.0.
        /// </summary>
        public virtual string friendly_name
        {
            get { return _friendly_name; }
            set
            {
                if (!Helper.AreEqual(value, _friendly_name))
                {
                    _friendly_name = value;
                    NotifyPropertyChanged("friendly_name");
                }
            }
        }
        private string _friendly_name = "";

        /// <summary>
        /// Variants of this driver available for selection
        /// Experimental. First published in 25.2.0.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Driver_variant>))]
        public virtual List<XenRef<Driver_variant>> variants
        {
            get { return _variants; }
            set
            {
                if (!Helper.AreEqual(value, _variants))
                {
                    _variants = value;
                    NotifyPropertyChanged("variants");
                }
            }
        }
        private List<XenRef<Driver_variant>> _variants = new List<XenRef<Driver_variant>>() {};

        /// <summary>
        /// Currently active variant of this driver, if any, or Null.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Driver_variant>))]
        public virtual XenRef<Driver_variant> active_variant
        {
            get { return _active_variant; }
            set
            {
                if (!Helper.AreEqual(value, _active_variant))
                {
                    _active_variant = value;
                    NotifyPropertyChanged("active_variant");
                }
            }
        }
        private XenRef<Driver_variant> _active_variant = new XenRef<Driver_variant>(Helper.NullOpaqueRef);

        /// <summary>
        /// Variant (if any) selected to become active after reboot. Or Null
        /// Experimental. First published in 25.2.0.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Driver_variant>))]
        public virtual XenRef<Driver_variant> selected_variant
        {
            get { return _selected_variant; }
            set
            {
                if (!Helper.AreEqual(value, _selected_variant))
                {
                    _selected_variant = value;
                    NotifyPropertyChanged("selected_variant");
                }
            }
        }
        private XenRef<Driver_variant> _selected_variant = new XenRef<Driver_variant>(Helper.NullOpaqueRef);

        /// <summary>
        /// Device type this driver supports, like network or storage
        /// Experimental. First published in 25.2.0.
        /// </summary>
        public virtual string type
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
        private string _type = "";

        /// <summary>
        /// Description of the driver
        /// Experimental. First published in 25.2.0.
        /// </summary>
        public virtual string description
        {
            get { return _description; }
            set
            {
                if (!Helper.AreEqual(value, _description))
                {
                    _description = value;
                    NotifyPropertyChanged("description");
                }
            }
        }
        private string _description = "";

        /// <summary>
        /// Information about the driver
        /// Experimental. First published in 25.2.0.
        /// </summary>
        public virtual string info
        {
            get { return _info; }
            set
            {
                if (!Helper.AreEqual(value, _info))
                {
                    _info = value;
                    NotifyPropertyChanged("info");
                }
            }
        }
        private string _info = "";
    }
}
