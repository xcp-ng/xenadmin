/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
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
using Newtonsoft.Json;


namespace XenAPI
{
    /// <summary>
    /// A new piece of functionality
    /// First published in XenServer 7.2.
    /// </summary>
    public partial class Feature : XenObject<Feature>
    {
        #region Constructors

        public Feature()
        {
        }

        public Feature(string uuid,
            string name_label,
            string name_description,
            bool enabled,
            bool experimental,
            string version,
            XenRef<Host> host)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.enabled = enabled;
            this.experimental = experimental;
            this.version = version;
            this.host = host;
        }

        /// <summary>
        /// Creates a new Feature from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Feature(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Creates a new Feature from a Proxy_Feature.
        /// </summary>
        /// <param name="proxy"></param>
        public Feature(Proxy_Feature proxy)
        {
            UpdateFrom(proxy);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Feature.
        /// </summary>
        public override void UpdateFrom(Feature update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            enabled = update.enabled;
            experimental = update.experimental;
            version = update.version;
            host = update.host;
        }

        internal void UpdateFrom(Proxy_Feature proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            name_label = proxy.name_label == null ? null : proxy.name_label;
            name_description = proxy.name_description == null ? null : proxy.name_description;
            enabled = (bool)proxy.enabled;
            experimental = (bool)proxy.experimental;
            version = proxy.version == null ? null : proxy.version;
            host = proxy.host == null ? null : XenRef<Host>.Create(proxy.host);
        }

        public Proxy_Feature ToProxy()
        {
            Proxy_Feature result_ = new Proxy_Feature();
            result_.uuid = uuid ?? "";
            result_.name_label = name_label ?? "";
            result_.name_description = name_description ?? "";
            result_.enabled = enabled;
            result_.experimental = experimental;
            result_.version = version ?? "";
            result_.host = host ?? "";
            return result_;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Feature
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
            if (table.ContainsKey("experimental"))
                experimental = Marshalling.ParseBool(table, "experimental");
            if (table.ContainsKey("version"))
                version = Marshalling.ParseString(table, "version");
            if (table.ContainsKey("host"))
                host = Marshalling.ParseRef<Host>(table, "host");
        }

        public bool DeepEquals(Feature other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name_label, other._name_label) &&
                Helper.AreEqual2(this._name_description, other._name_description) &&
                Helper.AreEqual2(this._enabled, other._enabled) &&
                Helper.AreEqual2(this._experimental, other._experimental) &&
                Helper.AreEqual2(this._version, other._version) &&
                Helper.AreEqual2(this._host, other._host);
        }

        internal static List<Feature> ProxyArrayToObjectList(Proxy_Feature[] input)
        {
            var result = new List<Feature>();
            foreach (var item in input)
                result.Add(new Feature(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, Feature server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
              throw new InvalidOperationException("This type has no read/write properties");
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given Feature.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_feature">The opaque_ref of the given feature</param>
        public static Feature get_record(Session session, string _feature)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.feature_get_record(session.opaque_ref, _feature);
            else
                return new Feature(session.XmlRpcProxy.feature_get_record(session.opaque_ref, _feature ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the Feature instance with the specified UUID.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<Feature> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.feature_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<Feature>.Create(session.XmlRpcProxy.feature_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get all the Feature instances with the given label.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        public static List<XenRef<Feature>> get_by_name_label(Session session, string _label)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.feature_get_by_name_label(session.opaque_ref, _label);
            else
                return XenRef<Feature>.Create(session.XmlRpcProxy.feature_get_by_name_label(session.opaque_ref, _label ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given Feature.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_feature">The opaque_ref of the given feature</param>
        public static string get_uuid(Session session, string _feature)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.feature_get_uuid(session.opaque_ref, _feature);
            else
                return session.XmlRpcProxy.feature_get_uuid(session.opaque_ref, _feature ?? "").parse();
        }

        /// <summary>
        /// Get the name/label field of the given Feature.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_feature">The opaque_ref of the given feature</param>
        public static string get_name_label(Session session, string _feature)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.feature_get_name_label(session.opaque_ref, _feature);
            else
                return session.XmlRpcProxy.feature_get_name_label(session.opaque_ref, _feature ?? "").parse();
        }

        /// <summary>
        /// Get the name/description field of the given Feature.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_feature">The opaque_ref of the given feature</param>
        public static string get_name_description(Session session, string _feature)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.feature_get_name_description(session.opaque_ref, _feature);
            else
                return session.XmlRpcProxy.feature_get_name_description(session.opaque_ref, _feature ?? "").parse();
        }

        /// <summary>
        /// Get the enabled field of the given Feature.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_feature">The opaque_ref of the given feature</param>
        public static bool get_enabled(Session session, string _feature)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.feature_get_enabled(session.opaque_ref, _feature);
            else
                return (bool)session.XmlRpcProxy.feature_get_enabled(session.opaque_ref, _feature ?? "").parse();
        }

        /// <summary>
        /// Get the experimental field of the given Feature.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_feature">The opaque_ref of the given feature</param>
        public static bool get_experimental(Session session, string _feature)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.feature_get_experimental(session.opaque_ref, _feature);
            else
                return (bool)session.XmlRpcProxy.feature_get_experimental(session.opaque_ref, _feature ?? "").parse();
        }

        /// <summary>
        /// Get the version field of the given Feature.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_feature">The opaque_ref of the given feature</param>
        public static string get_version(Session session, string _feature)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.feature_get_version(session.opaque_ref, _feature);
            else
                return session.XmlRpcProxy.feature_get_version(session.opaque_ref, _feature ?? "").parse();
        }

        /// <summary>
        /// Get the host field of the given Feature.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_feature">The opaque_ref of the given feature</param>
        public static XenRef<Host> get_host(Session session, string _feature)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.feature_get_host(session.opaque_ref, _feature);
            else
                return XenRef<Host>.Create(session.XmlRpcProxy.feature_get_host(session.opaque_ref, _feature ?? "").parse());
        }

        /// <summary>
        /// Return a list of all the Features known to the system.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Feature>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.feature_get_all(session.opaque_ref);
            else
                return XenRef<Feature>.Create(session.XmlRpcProxy.feature_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the Feature Records at once, in a single XML RPC call
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Feature>, Feature> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.feature_get_all_records(session.opaque_ref);
            else
                return XenRef<Feature>.Create<Proxy_Feature>(session.XmlRpcProxy.feature_get_all_records(session.opaque_ref).parse());
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
        /// Indicates whether the feature is enabled
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

        /// <summary>
        /// Indicates whether the feature is experimental (as opposed to stable and fully supported)
        /// </summary>
        public virtual bool experimental
        {
            get { return _experimental; }
            set
            {
                if (!Helper.AreEqual(value, _experimental))
                {
                    _experimental = value;
                    NotifyPropertyChanged("experimental");
                }
            }
        }
        private bool _experimental = false;

        /// <summary>
        /// The version of this feature
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
        private string _version = "1.0";

        /// <summary>
        /// The host where this feature is available
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
    }
}
