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
    /// A user or group that can log in xapi
    /// First published in XenServer 5.5.
    /// </summary>
    public partial class Subject : XenObject<Subject>
    {
        #region Constructors

        public Subject()
        {
        }

        public Subject(string uuid,
            string subject_identifier,
            Dictionary<string, string> other_config,
            List<XenRef<Role>> roles)
        {
            this.uuid = uuid;
            this.subject_identifier = subject_identifier;
            this.other_config = other_config;
            this.roles = roles;
        }

        /// <summary>
        /// Creates a new Subject from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Subject(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Creates a new Subject from a Proxy_Subject.
        /// </summary>
        /// <param name="proxy"></param>
        public Subject(Proxy_Subject proxy)
        {
            UpdateFrom(proxy);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Subject.
        /// </summary>
        public override void UpdateFrom(Subject update)
        {
            uuid = update.uuid;
            subject_identifier = update.subject_identifier;
            other_config = update.other_config;
            roles = update.roles;
        }

        internal void UpdateFrom(Proxy_Subject proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            subject_identifier = proxy.subject_identifier == null ? null : proxy.subject_identifier;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            roles = proxy.roles == null ? null : XenRef<Role>.Create(proxy.roles);
        }

        public Proxy_Subject ToProxy()
        {
            Proxy_Subject result_ = new Proxy_Subject();
            result_.uuid = uuid ?? "";
            result_.subject_identifier = subject_identifier ?? "";
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.roles = roles == null ? new string[] {} : Helper.RefListToStringArray(roles);
            return result_;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Subject
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("subject_identifier"))
                subject_identifier = Marshalling.ParseString(table, "subject_identifier");
            if (table.ContainsKey("other_config"))
                other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("roles"))
                roles = Marshalling.ParseSetRef<Role>(table, "roles");
        }

        public bool DeepEquals(Subject other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._subject_identifier, other._subject_identifier) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._roles, other._roles);
        }

        internal static List<Subject> ProxyArrayToObjectList(Proxy_Subject[] input)
        {
            var result = new List<Subject>();
            foreach (var item in input)
                result.Add(new Subject(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, Subject server)
        {
            if (opaqueRef == null)
            {
                var reference = create(session, this);
                return reference == null ? null : reference.opaque_ref;
            }
            else
            {
              throw new InvalidOperationException("This type has no read/write properties");
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given subject.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        public static Subject get_record(Session session, string _subject)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.subject_get_record(session.opaque_ref, _subject);
            else
                return new Subject(session.XmlRpcProxy.subject_get_record(session.opaque_ref, _subject ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the subject instance with the specified UUID.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<Subject> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.subject_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<Subject>.Create(session.XmlRpcProxy.subject_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Create a new subject instance, and return its handle.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<Subject> create(Session session, Subject _record)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.subject_create(session.opaque_ref, _record);
            else
                return XenRef<Subject>.Create(session.XmlRpcProxy.subject_create(session.opaque_ref, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Create a new subject instance, and return its handle.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<Task> async_create(Session session, Subject _record)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_subject_create(session.opaque_ref, _record);
          else
              return XenRef<Task>.Create(session.XmlRpcProxy.async_subject_create(session.opaque_ref, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Destroy the specified subject instance.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        public static void destroy(Session session, string _subject)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.subject_destroy(session.opaque_ref, _subject);
            else
                session.XmlRpcProxy.subject_destroy(session.opaque_ref, _subject ?? "").parse();
        }

        /// <summary>
        /// Destroy the specified subject instance.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        public static XenRef<Task> async_destroy(Session session, string _subject)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_subject_destroy(session.opaque_ref, _subject);
          else
              return XenRef<Task>.Create(session.XmlRpcProxy.async_subject_destroy(session.opaque_ref, _subject ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given subject.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        public static string get_uuid(Session session, string _subject)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.subject_get_uuid(session.opaque_ref, _subject);
            else
                return session.XmlRpcProxy.subject_get_uuid(session.opaque_ref, _subject ?? "").parse();
        }

        /// <summary>
        /// Get the subject_identifier field of the given subject.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        public static string get_subject_identifier(Session session, string _subject)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.subject_get_subject_identifier(session.opaque_ref, _subject);
            else
                return session.XmlRpcProxy.subject_get_subject_identifier(session.opaque_ref, _subject ?? "").parse();
        }

        /// <summary>
        /// Get the other_config field of the given subject.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        public static Dictionary<string, string> get_other_config(Session session, string _subject)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.subject_get_other_config(session.opaque_ref, _subject);
            else
                return Maps.convert_from_proxy_string_string(session.XmlRpcProxy.subject_get_other_config(session.opaque_ref, _subject ?? "").parse());
        }

        /// <summary>
        /// Get the roles field of the given subject.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        public static List<XenRef<Role>> get_roles(Session session, string _subject)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.subject_get_roles(session.opaque_ref, _subject);
            else
                return XenRef<Role>.Create(session.XmlRpcProxy.subject_get_roles(session.opaque_ref, _subject ?? "").parse());
        }

        /// <summary>
        /// This call adds a new role to a subject
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        /// <param name="_role">The unique role reference</param>
        public static void add_to_roles(Session session, string _subject, string _role)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.subject_add_to_roles(session.opaque_ref, _subject, _role);
            else
                session.XmlRpcProxy.subject_add_to_roles(session.opaque_ref, _subject ?? "", _role ?? "").parse();
        }

        /// <summary>
        /// This call removes a role from a subject
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        /// <param name="_role">The unique role reference in the subject's roles field</param>
        public static void remove_from_roles(Session session, string _subject, string _role)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.subject_remove_from_roles(session.opaque_ref, _subject, _role);
            else
                session.XmlRpcProxy.subject_remove_from_roles(session.opaque_ref, _subject ?? "", _role ?? "").parse();
        }

        /// <summary>
        /// This call returns a list of permission names given a subject
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        public static string[] get_permissions_name_label(Session session, string _subject)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.subject_get_permissions_name_label(session.opaque_ref, _subject);
            else
                return (string [])session.XmlRpcProxy.subject_get_permissions_name_label(session.opaque_ref, _subject ?? "").parse();
        }

        /// <summary>
        /// Return a list of all the subjects known to the system.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Subject>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.subject_get_all(session.opaque_ref);
            else
                return XenRef<Subject>.Create(session.XmlRpcProxy.subject_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the subject Records at once, in a single XML RPC call
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Subject>, Subject> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.subject_get_all_records(session.opaque_ref);
            else
                return XenRef<Subject>.Create<Proxy_Subject>(session.XmlRpcProxy.subject_get_all_records(session.opaque_ref).parse());
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
        /// the subject identifier, unique in the external directory service
        /// </summary>
        public virtual string subject_identifier
        {
            get { return _subject_identifier; }
            set
            {
                if (!Helper.AreEqual(value, _subject_identifier))
                {
                    _subject_identifier = value;
                    NotifyPropertyChanged("subject_identifier");
                }
            }
        }
        private string _subject_identifier = "";

        /// <summary>
        /// additional configuration
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
        /// the roles associated with this subject
        /// First published in XenServer 5.6.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Role>))]
        public virtual List<XenRef<Role>> roles
        {
            get { return _roles; }
            set
            {
                if (!Helper.AreEqual(value, _roles))
                {
                    _roles = value;
                    NotifyPropertyChanged("roles");
                }
            }
        }
        private List<XenRef<Role>> _roles = new List<XenRef<Role>>() {new XenRef<Role>("OpaqueRef:0165f154-ba3e-034e-6b27-5d271af109ba")};
    }
}
