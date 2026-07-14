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
    /// A storage manager plugin
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class SM : XenObject<SM>
    {
        #region Constructors

        public SM()
        {
        }

        public SM(string uuid,
            string name_label,
            string name_description,
            string type,
            string vendor,
            string copyright,
            string version,
            string required_api_version,
            Dictionary<string, string> configuration,
            string[] capabilities,
            Dictionary<string, long> features,
            Dictionary<string, string> other_config,
            string driver_filename,
            string[] required_cluster_stack,
            List<image_format_type> supported_image_formats)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.type = type;
            this.vendor = vendor;
            this.copyright = copyright;
            this.version = version;
            this.required_api_version = required_api_version;
            this.configuration = configuration;
            this.capabilities = capabilities;
            this.features = features;
            this.other_config = other_config;
            this.driver_filename = driver_filename;
            this.required_cluster_stack = required_cluster_stack;
            this.supported_image_formats = supported_image_formats;
        }

        /// <summary>
        /// Creates a new SM from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public SM(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given SM.
        /// </summary>
        public override void UpdateFrom(SM record)
        {
            uuid = record.uuid;
            name_label = record.name_label;
            name_description = record.name_description;
            type = record.type;
            vendor = record.vendor;
            copyright = record.copyright;
            version = record.version;
            required_api_version = record.required_api_version;
            configuration = record.configuration;
            capabilities = record.capabilities;
            features = record.features;
            other_config = record.other_config;
            driver_filename = record.driver_filename;
            required_cluster_stack = record.required_cluster_stack;
            supported_image_formats = record.supported_image_formats;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this SM
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
            if (table.ContainsKey("type"))
                type = Marshalling.ParseString(table, "type");
            if (table.ContainsKey("vendor"))
                vendor = Marshalling.ParseString(table, "vendor");
            if (table.ContainsKey("copyright"))
                copyright = Marshalling.ParseString(table, "copyright");
            if (table.ContainsKey("version"))
                version = Marshalling.ParseString(table, "version");
            if (table.ContainsKey("required_api_version"))
                required_api_version = Marshalling.ParseString(table, "required_api_version");
            if (table.ContainsKey("configuration"))
                configuration = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "configuration"));
            if (table.ContainsKey("capabilities"))
                capabilities = Marshalling.ParseStringArray(table, "capabilities");
            if (table.ContainsKey("features"))
                features = Maps.ToDictionary_string_long(Marshalling.ParseHashTable(table, "features"));
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("driver_filename"))
                driver_filename = Marshalling.ParseString(table, "driver_filename");
            if (table.ContainsKey("required_cluster_stack"))
                required_cluster_stack = Marshalling.ParseStringArray(table, "required_cluster_stack");
            if (table.ContainsKey("supported_image_formats"))
                supported_image_formats = Helper.StringArrayToEnumList<image_format_type>(Marshalling.ParseStringArray(table, "supported_image_formats"));
        }

        public bool DeepEquals(SM other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name_label, other._name_label) &&
                Helper.AreEqual2(_name_description, other._name_description) &&
                Helper.AreEqual2(_type, other._type) &&
                Helper.AreEqual2(_vendor, other._vendor) &&
                Helper.AreEqual2(_copyright, other._copyright) &&
                Helper.AreEqual2(_version, other._version) &&
                Helper.AreEqual2(_required_api_version, other._required_api_version) &&
                Helper.AreEqual2(_configuration, other._configuration) &&
                Helper.AreEqual2(_capabilities, other._capabilities) &&
                Helper.AreEqual2(_features, other._features) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_driver_filename, other._driver_filename) &&
                Helper.AreEqual2(_required_cluster_stack, other._required_cluster_stack) &&
                Helper.AreEqual2(_supported_image_formats, other._supported_image_formats);
        }


        /// <summary>
        /// Get a record containing the current state of the given SM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static SM get_record(Session session, string _sm)
        {
            return session.JsonRpcClient.sm_get_record(session.opaque_ref, _sm);
        }

        /// <summary>
        /// Get a reference to the SM instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<SM> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.sm_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get all the SM instances with the given label.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<SM>> get_by_name_label(Session session, string _label)
        {
            return session.JsonRpcClient.sm_get_by_name_label(session.opaque_ref, _label);
        }

        /// <summary>
        /// Get the uuid field of the given SM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _sm)
        {
            return session.JsonRpcClient.sm_get_uuid(session.opaque_ref, _sm);
        }

        /// <summary>
        /// Get the name/label field of the given SM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_label(Session session, string _sm)
        {
            return session.JsonRpcClient.sm_get_name_label(session.opaque_ref, _sm);
        }

        /// <summary>
        /// Get the name/description field of the given SM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_description(Session session, string _sm)
        {
            return session.JsonRpcClient.sm_get_name_description(session.opaque_ref, _sm);
        }

        /// <summary>
        /// Get the type field of the given SM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_type(Session session, string _sm)
        {
            return session.JsonRpcClient.sm_get_type(session.opaque_ref, _sm);
        }

        /// <summary>
        /// Get the vendor field of the given SM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_vendor(Session session, string _sm)
        {
            return session.JsonRpcClient.sm_get_vendor(session.opaque_ref, _sm);
        }

        /// <summary>
        /// Get the copyright field of the given SM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_copyright(Session session, string _sm)
        {
            return session.JsonRpcClient.sm_get_copyright(session.opaque_ref, _sm);
        }

        /// <summary>
        /// Get the version field of the given SM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_version(Session session, string _sm)
        {
            return session.JsonRpcClient.sm_get_version(session.opaque_ref, _sm);
        }

        /// <summary>
        /// Get the required_api_version field of the given SM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_required_api_version(Session session, string _sm)
        {
            return session.JsonRpcClient.sm_get_required_api_version(session.opaque_ref, _sm);
        }

        /// <summary>
        /// Get the configuration field of the given SM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_configuration(Session session, string _sm)
        {
            return session.JsonRpcClient.sm_get_configuration(session.opaque_ref, _sm);
        }

        /// <summary>
        /// Get the capabilities field of the given SM.
        /// First published in XenServer 4.1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_capabilities(Session session, string _sm)
        {
            return session.JsonRpcClient.sm_get_capabilities(session.opaque_ref, _sm);
        }

        /// <summary>
        /// Get the features field of the given SM.
        /// First published in XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, long> get_features(Session session, string _sm)
        {
            return session.JsonRpcClient.sm_get_features(session.opaque_ref, _sm);
        }

        /// <summary>
        /// Get the other_config field of the given SM.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _sm)
        {
            return session.JsonRpcClient.sm_get_other_config(session.opaque_ref, _sm);
        }

        /// <summary>
        /// Get the driver_filename field of the given SM.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_driver_filename(Session session, string _sm)
        {
            return session.JsonRpcClient.sm_get_driver_filename(session.opaque_ref, _sm);
        }

        /// <summary>
        /// Get the required_cluster_stack field of the given SM.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_required_cluster_stack(Session session, string _sm)
        {
            return session.JsonRpcClient.sm_get_required_cluster_stack(session.opaque_ref, _sm);
        }

        /// <summary>
        /// Get the supported_image_formats field of the given SM.
        /// Experimental. First published in 26.15.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<image_format_type> get_supported_image_formats(Session session, string _sm)
        {
            return session.JsonRpcClient.sm_get_supported_image_formats(session.opaque_ref, _sm);
        }

        /// <summary>
        /// Set the other_config field of the given SM.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_other_config(Session session, string _sm, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.sm_set_other_config(session.opaque_ref, _sm, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given SM.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_other_config(Session session, string _sm, string _key, string _value)
        {
            session.JsonRpcClient.sm_add_to_other_config(session.opaque_ref, _sm, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given SM.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sm">The opaque_ref of the given sm</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_other_config(Session session, string _sm, string _key)
        {
            session.JsonRpcClient.sm_remove_from_other_config(session.opaque_ref, _sm, _key);
        }

        /// <summary>
        /// Return a list of all the SMs known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<SM>> get_all(Session session)
        {
            return session.JsonRpcClient.sm_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of SM references to SM records for all SMs known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<SM>, SM> get_all_records(Session session)
        {
            return session.JsonRpcClient.sm_get_all_records(session.opaque_ref);
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
        /// SR.type
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
        /// Vendor who created this plugin
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
        /// Entity which owns the copyright of this plugin
        /// </summary>
        public virtual string copyright
        {
            get { return _copyright; }
            set
            {
                if (!Helper.AreEqual(value, _copyright))
                {
                    _copyright = value;
                    NotifyPropertyChanged("copyright");
                }
            }
        }
        private string _copyright = "";

        /// <summary>
        /// Version of the plugin
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
        /// Minimum SM API version required on the server
        /// </summary>
        public virtual string required_api_version
        {
            get { return _required_api_version; }
            set
            {
                if (!Helper.AreEqual(value, _required_api_version))
                {
                    _required_api_version = value;
                    NotifyPropertyChanged("required_api_version");
                }
            }
        }
        private string _required_api_version = "";

        /// <summary>
        /// names and descriptions of device config keys
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> configuration
        {
            get { return _configuration; }
            set
            {
                if (!Helper.AreEqual(value, _configuration))
                {
                    _configuration = value;
                    NotifyPropertyChanged("configuration");
                }
            }
        }
        private Dictionary<string, string> _configuration = new Dictionary<string, string>() {};

        /// <summary>
        /// capabilities of the SM plugin
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual string[] capabilities
        {
            get { return _capabilities; }
            set
            {
                if (!Helper.AreEqual(value, _capabilities))
                {
                    _capabilities = value;
                    NotifyPropertyChanged("capabilities");
                }
            }
        }
        private string[] _capabilities = {};

        /// <summary>
        /// capabilities of the SM plugin, with capability version numbers
        /// First published in XenServer 6.2.
        /// </summary>
        public virtual Dictionary<string, long> features
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
        private Dictionary<string, long> _features = new Dictionary<string, long>() {};

        /// <summary>
        /// additional configuration
        /// First published in XenServer 4.1.
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
        /// filename of the storage driver
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual string driver_filename
        {
            get { return _driver_filename; }
            set
            {
                if (!Helper.AreEqual(value, _driver_filename))
                {
                    _driver_filename = value;
                    NotifyPropertyChanged("driver_filename");
                }
            }
        }
        private string _driver_filename = "";

        /// <summary>
        /// The storage plugin requires that one of these cluster stacks is configured and running.
        /// First published in XenServer 7.0.
        /// </summary>
        public virtual string[] required_cluster_stack
        {
            get { return _required_cluster_stack; }
            set
            {
                if (!Helper.AreEqual(value, _required_cluster_stack))
                {
                    _required_cluster_stack = value;
                    NotifyPropertyChanged("required_cluster_stack");
                }
            }
        }
        private string[] _required_cluster_stack = {};

        /// <summary>
        /// Image formats supported by the SR: raw, vhd, qcow2
        /// Experimental. First published in 26.15.0.
        /// </summary>
        public virtual List<image_format_type> supported_image_formats
        {
            get { return _supported_image_formats; }
            set
            {
                if (!Helper.AreEqual(value, _supported_image_formats))
                {
                    _supported_image_formats = value;
                    NotifyPropertyChanged("supported_image_formats");
                }
            }
        }
        private List<image_format_type> _supported_image_formats = new List<image_format_type>() {};
    }
}
