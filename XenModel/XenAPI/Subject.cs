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

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Subject.
        /// </summary>
        public override void UpdateFrom(Subject record)
        {
            uuid = record.uuid;
            subject_identifier = record.subject_identifier;
            other_config = record.other_config;
            roles = record.roles;
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
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("roles"))
                roles = Marshalling.ParseSetRef<Role>(table, "roles");
        }

        public bool DeepEquals(Subject other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_subject_identifier, other._subject_identifier) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_roles, other._roles);
        }


        /// <summary>
        /// Get a record containing the current state of the given subject.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Subject get_record(Session session, string _subject)
        {
            return session.JsonRpcClient.subject_get_record(session.opaque_ref, _subject);
        }

        /// <summary>
        /// Get a reference to the subject instance with the specified UUID.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Subject> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.subject_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Create a new subject instance, and return its handle.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Subject> create(Session session, Subject _record)
        {
            return session.JsonRpcClient.subject_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Create a new subject instance, and return its handle.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_create(Session session, Subject _record)
        {
          return session.JsonRpcClient.async_subject_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Destroy the specified subject instance.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void destroy(Session session, string _subject)
        {
            session.JsonRpcClient.subject_destroy(session.opaque_ref, _subject);
        }

        /// <summary>
        /// Destroy the specified subject instance.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _subject)
        {
          return session.JsonRpcClient.async_subject_destroy(session.opaque_ref, _subject);
        }

        /// <summary>
        /// Get the uuid field of the given subject.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _subject)
        {
            return session.JsonRpcClient.subject_get_uuid(session.opaque_ref, _subject);
        }

        /// <summary>
        /// Get the subject_identifier field of the given subject.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_subject_identifier(Session session, string _subject)
        {
            return session.JsonRpcClient.subject_get_subject_identifier(session.opaque_ref, _subject);
        }

        /// <summary>
        /// Get the other_config field of the given subject.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _subject)
        {
            return session.JsonRpcClient.subject_get_other_config(session.opaque_ref, _subject);
        }

        /// <summary>
        /// Get the roles field of the given subject.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Role>> get_roles(Session session, string _subject)
        {
            return session.JsonRpcClient.subject_get_roles(session.opaque_ref, _subject);
        }

        /// <summary>
        /// This call adds a new role to a subject
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        /// <param name="_role">The unique role reference</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void add_to_roles(Session session, string _subject, string _role)
        {
            session.JsonRpcClient.subject_add_to_roles(session.opaque_ref, _subject, _role);
        }

        /// <summary>
        /// This call removes a role from a subject
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        /// <param name="_role">The unique role reference in the subject's roles field</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void remove_from_roles(Session session, string _subject, string _role)
        {
            session.JsonRpcClient.subject_remove_from_roles(session.opaque_ref, _subject, _role);
        }

        /// <summary>
        /// This call returns a list of permission names given a subject
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject">The opaque_ref of the given subject</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_permissions_name_label(Session session, string _subject)
        {
            return session.JsonRpcClient.subject_get_permissions_name_label(session.opaque_ref, _subject);
        }

        /// <summary>
        /// Return a list of all the subjects known to the system.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Subject>> get_all(Session session)
        {
            return session.JsonRpcClient.subject_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of subject references to subject records for all subjects known to the system.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Subject>, Subject> get_all_records(Session session)
        {
            return session.JsonRpcClient.subject_get_all_records(session.opaque_ref);
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
