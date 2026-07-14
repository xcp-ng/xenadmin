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
    /// Describes an observer which will control observability activity in the Toolstack
    /// First published in .
    /// </summary>
    public partial class Observer : XenObject<Observer>
    {
        #region Constructors

        public Observer()
        {
        }

        public Observer(string uuid,
            string name_label,
            string name_description,
            List<XenRef<Host>> hosts,
            Dictionary<string, string> attributes,
            string[] endpoints,
            string[] components,
            bool enabled)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.hosts = hosts;
            this.attributes = attributes;
            this.endpoints = endpoints;
            this.components = components;
            this.enabled = enabled;
        }

        /// <summary>
        /// Creates a new Observer from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Observer(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Observer.
        /// </summary>
        public override void UpdateFrom(Observer record)
        {
            uuid = record.uuid;
            name_label = record.name_label;
            name_description = record.name_description;
            hosts = record.hosts;
            attributes = record.attributes;
            endpoints = record.endpoints;
            components = record.components;
            enabled = record.enabled;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Observer
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
            if (table.ContainsKey("hosts"))
                hosts = Marshalling.ParseSetRef<Host>(table, "hosts");
            if (table.ContainsKey("attributes"))
                attributes = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "attributes"));
            if (table.ContainsKey("endpoints"))
                endpoints = Marshalling.ParseStringArray(table, "endpoints");
            if (table.ContainsKey("components"))
                components = Marshalling.ParseStringArray(table, "components");
            if (table.ContainsKey("enabled"))
                enabled = Marshalling.ParseBool(table, "enabled");
        }

        public bool DeepEquals(Observer other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name_label, other._name_label) &&
                Helper.AreEqual2(_name_description, other._name_description) &&
                Helper.AreEqual2(_hosts, other._hosts) &&
                Helper.AreEqual2(_attributes, other._attributes) &&
                Helper.AreEqual2(_endpoints, other._endpoints) &&
                Helper.AreEqual2(_components, other._components) &&
                Helper.AreEqual2(_enabled, other._enabled);
        }


        /// <summary>
        /// Get a record containing the current state of the given Observer.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Observer get_record(Session session, string _observer)
        {
            return session.JsonRpcClient.observer_get_record(session.opaque_ref, _observer);
        }

        /// <summary>
        /// Get a reference to the Observer instance with the specified UUID.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Observer> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.observer_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Create a new Observer instance, and return its handle.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Observer> create(Session session, Observer _record)
        {
            return session.JsonRpcClient.observer_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Create a new Observer instance, and return its handle.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_create(Session session, Observer _record)
        {
          return session.JsonRpcClient.async_observer_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Destroy the specified Observer instance.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void destroy(Session session, string _observer)
        {
            session.JsonRpcClient.observer_destroy(session.opaque_ref, _observer);
        }

        /// <summary>
        /// Destroy the specified Observer instance.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _observer)
        {
          return session.JsonRpcClient.async_observer_destroy(session.opaque_ref, _observer);
        }

        /// <summary>
        /// Get all the Observer instances with the given label.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Observer>> get_by_name_label(Session session, string _label)
        {
            return session.JsonRpcClient.observer_get_by_name_label(session.opaque_ref, _label);
        }

        /// <summary>
        /// Get the uuid field of the given Observer.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _observer)
        {
            return session.JsonRpcClient.observer_get_uuid(session.opaque_ref, _observer);
        }

        /// <summary>
        /// Get the name/label field of the given Observer.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_label(Session session, string _observer)
        {
            return session.JsonRpcClient.observer_get_name_label(session.opaque_ref, _observer);
        }

        /// <summary>
        /// Get the name/description field of the given Observer.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_description(Session session, string _observer)
        {
            return session.JsonRpcClient.observer_get_name_description(session.opaque_ref, _observer);
        }

        /// <summary>
        /// Get the hosts field of the given Observer.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Host>> get_hosts(Session session, string _observer)
        {
            return session.JsonRpcClient.observer_get_hosts(session.opaque_ref, _observer);
        }

        /// <summary>
        /// Get the attributes field of the given Observer.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_attributes(Session session, string _observer)
        {
            return session.JsonRpcClient.observer_get_attributes(session.opaque_ref, _observer);
        }

        /// <summary>
        /// Get the endpoints field of the given Observer.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_endpoints(Session session, string _observer)
        {
            return session.JsonRpcClient.observer_get_endpoints(session.opaque_ref, _observer);
        }

        /// <summary>
        /// Get the components field of the given Observer.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_components(Session session, string _observer)
        {
            return session.JsonRpcClient.observer_get_components(session.opaque_ref, _observer);
        }

        /// <summary>
        /// Get the enabled field of the given Observer.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_enabled(Session session, string _observer)
        {
            return session.JsonRpcClient.observer_get_enabled(session.opaque_ref, _observer);
        }

        /// <summary>
        /// Set the name/label field of the given Observer.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <param name="_label">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_name_label(Session session, string _observer, string _label)
        {
            session.JsonRpcClient.observer_set_name_label(session.opaque_ref, _observer, _label);
        }

        /// <summary>
        /// Set the name/description field of the given Observer.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <param name="_description">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_name_description(Session session, string _observer, string _description)
        {
            session.JsonRpcClient.observer_set_name_description(session.opaque_ref, _observer, _description);
        }

        /// <summary>
        /// Sets the hosts that the observer is to be registered on
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <param name="_value">Hosts the observer is registered on</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_hosts(Session session, string _observer, List<XenRef<Host>> _value)
        {
            session.JsonRpcClient.observer_set_hosts(session.opaque_ref, _observer, _value);
        }

        /// <summary>
        /// Sets the hosts that the observer is to be registered on
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <param name="_value">Hosts the observer is registered on</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_hosts(Session session, string _observer, List<XenRef<Host>> _value)
        {
          return session.JsonRpcClient.async_observer_set_hosts(session.opaque_ref, _observer, _value);
        }

        /// <summary>
        /// Enable / disable this observer which will stop the observer from producing observability information
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <param name="_value">If the observer is to be enabled (true) or disabled (false)</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_enabled(Session session, string _observer, bool _value)
        {
            session.JsonRpcClient.observer_set_enabled(session.opaque_ref, _observer, _value);
        }

        /// <summary>
        /// Enable / disable this observer which will stop the observer from producing observability information
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <param name="_value">If the observer is to be enabled (true) or disabled (false)</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_enabled(Session session, string _observer, bool _value)
        {
          return session.JsonRpcClient.async_observer_set_enabled(session.opaque_ref, _observer, _value);
        }

        /// <summary>
        /// Set the attributes of an observer. These are used to emit metadata by the observer
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <param name="_value">The attributes that the observer emits as part of the data</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_attributes(Session session, string _observer, Dictionary<string, string> _value)
        {
            session.JsonRpcClient.observer_set_attributes(session.opaque_ref, _observer, _value);
        }

        /// <summary>
        /// Set the attributes of an observer. These are used to emit metadata by the observer
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <param name="_value">The attributes that the observer emits as part of the data</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_attributes(Session session, string _observer, Dictionary<string, string> _value)
        {
          return session.JsonRpcClient.async_observer_set_attributes(session.opaque_ref, _observer, _value);
        }

        /// <summary>
        /// Set the file/HTTP endpoints the observer sends data to
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <param name="_value">The endpoints that the observer will export data to. A URL or the string 'bugtool'. This can refer to an enpoint to the local file system</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_endpoints(Session session, string _observer, string[] _value)
        {
            session.JsonRpcClient.observer_set_endpoints(session.opaque_ref, _observer, _value);
        }

        /// <summary>
        /// Set the file/HTTP endpoints the observer sends data to
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <param name="_value">The endpoints that the observer will export data to. A URL or the string 'bugtool'. This can refer to an enpoint to the local file system</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_endpoints(Session session, string _observer, string[] _value)
        {
          return session.JsonRpcClient.async_observer_set_endpoints(session.opaque_ref, _observer, _value);
        }

        /// <summary>
        /// Set the components on which the observer will broadcast to. i.e. xapi, xenopsd, networkd, etc.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <param name="_value">The components the observer will broadcast to</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_components(Session session, string _observer, string[] _value)
        {
            session.JsonRpcClient.observer_set_components(session.opaque_ref, _observer, _value);
        }

        /// <summary>
        /// Set the components on which the observer will broadcast to. i.e. xapi, xenopsd, networkd, etc.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_observer">The opaque_ref of the given observer</param>
        /// <param name="_value">The components the observer will broadcast to</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_components(Session session, string _observer, string[] _value)
        {
          return session.JsonRpcClient.async_observer_set_components(session.opaque_ref, _observer, _value);
        }

        /// <summary>
        /// Return a list of all the Observers known to the system.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Observer>> get_all(Session session)
        {
            return session.JsonRpcClient.observer_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of Observer references to Observer records for all Observers known to the system.
        /// Experimental. First published in 23.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Observer>, Observer> get_all_records(Session session)
        {
            return session.JsonRpcClient.observer_get_all_records(session.opaque_ref);
        }

        /// <summary>
        /// Unique identifier/object reference
        /// Experimental. First published in 23.14.0.
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
        /// First published in XenServer 4.0.
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
        /// First published in XenServer 4.0.
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
        /// The list of hosts the observer is active on. An empty list means all hosts
        /// Experimental. First published in 23.14.0.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Host>))]
        public virtual List<XenRef<Host>> hosts
        {
            get { return _hosts; }
            set
            {
                if (!Helper.AreEqual(value, _hosts))
                {
                    _hosts = value;
                    NotifyPropertyChanged("hosts");
                }
            }
        }
        private List<XenRef<Host>> _hosts = new List<XenRef<Host>>() {};

        /// <summary>
        /// Attributes that observer will add to the data they produce
        /// Experimental. First published in 23.14.0.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> attributes
        {
            get { return _attributes; }
            set
            {
                if (!Helper.AreEqual(value, _attributes))
                {
                    _attributes = value;
                    NotifyPropertyChanged("attributes");
                }
            }
        }
        private Dictionary<string, string> _attributes = new Dictionary<string, string>() {};

        /// <summary>
        /// The list of endpoints where data is exported to. Each endpoint is a URL or the string 'bugtool' refering to the internal logs
        /// Experimental. First published in 23.14.0.
        /// </summary>
        public virtual string[] endpoints
        {
            get { return _endpoints; }
            set
            {
                if (!Helper.AreEqual(value, _endpoints))
                {
                    _endpoints = value;
                    NotifyPropertyChanged("endpoints");
                }
            }
        }
        private string[] _endpoints = {};

        /// <summary>
        /// The list of xenserver components the observer will broadcast. An empty list means all components
        /// Experimental. First published in 23.14.0.
        /// </summary>
        public virtual string[] components
        {
            get { return _components; }
            set
            {
                if (!Helper.AreEqual(value, _components))
                {
                    _components = value;
                    NotifyPropertyChanged("components");
                }
            }
        }
        private string[] _components = {};

        /// <summary>
        /// This denotes if the observer is enabled. true if it is enabled and false if it is disabled
        /// Experimental. First published in 23.14.0.
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
        private bool _enabled = false;
    }
}
