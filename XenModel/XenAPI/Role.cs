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
    /// A set of permissions associated with a subject
    /// First published in XenServer 5.6.
    /// </summary>
    public partial class Role : XenObject<Role>
    {
        #region Constructors

        public Role()
        {
        }

        public Role(string uuid,
            string name_label,
            string name_description,
            List<XenRef<Role>> subroles,
            bool is_internal)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.subroles = subroles;
            this.is_internal = is_internal;
        }

        /// <summary>
        /// Creates a new Role from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Role(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Role.
        /// </summary>
        public override void UpdateFrom(Role record)
        {
            uuid = record.uuid;
            name_label = record.name_label;
            name_description = record.name_description;
            subroles = record.subroles;
            is_internal = record.is_internal;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Role
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
            if (table.ContainsKey("subroles"))
                subroles = Marshalling.ParseSetRef<Role>(table, "subroles");
            if (table.ContainsKey("is_internal"))
                is_internal = Marshalling.ParseBool(table, "is_internal");
        }

        public bool DeepEquals(Role other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name_label, other._name_label) &&
                Helper.AreEqual2(_name_description, other._name_description) &&
                Helper.AreEqual2(_subroles, other._subroles) &&
                Helper.AreEqual2(_is_internal, other._is_internal);
        }


        /// <summary>
        /// Get a record containing the current state of the given role.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_role">The opaque_ref of the given role</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Role get_record(Session session, string _role)
        {
            return session.JsonRpcClient.role_get_record(session.opaque_ref, _role);
        }

        /// <summary>
        /// Get a reference to the role instance with the specified UUID.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Role> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.role_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get all the role instances with the given label.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Role>> get_by_name_label(Session session, string _label)
        {
            return session.JsonRpcClient.role_get_by_name_label(session.opaque_ref, _label);
        }

        /// <summary>
        /// Get the uuid field of the given role.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_role">The opaque_ref of the given role</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _role)
        {
            return session.JsonRpcClient.role_get_uuid(session.opaque_ref, _role);
        }

        /// <summary>
        /// Get the name/label field of the given role.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_role">The opaque_ref of the given role</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_label(Session session, string _role)
        {
            return session.JsonRpcClient.role_get_name_label(session.opaque_ref, _role);
        }

        /// <summary>
        /// Get the name/description field of the given role.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_role">The opaque_ref of the given role</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_description(Session session, string _role)
        {
            return session.JsonRpcClient.role_get_name_description(session.opaque_ref, _role);
        }

        /// <summary>
        /// Get the subroles field of the given role.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_role">The opaque_ref of the given role</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Role>> get_subroles(Session session, string _role)
        {
            return session.JsonRpcClient.role_get_subroles(session.opaque_ref, _role);
        }

        /// <summary>
        /// Get the is_internal field of the given role.
        /// First published in 22.5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_role">The opaque_ref of the given role</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_is_internal(Session session, string _role)
        {
            return session.JsonRpcClient.role_get_is_internal(session.opaque_ref, _role);
        }

        /// <summary>
        /// This call returns a list of permissions given a role
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_role">The opaque_ref of the given role</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Role>> get_permissions(Session session, string _role)
        {
            return session.JsonRpcClient.role_get_permissions(session.opaque_ref, _role);
        }

        /// <summary>
        /// This call returns a list of permission names given a role
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_role">The opaque_ref of the given role</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_permissions_name_label(Session session, string _role)
        {
            return session.JsonRpcClient.role_get_permissions_name_label(session.opaque_ref, _role);
        }

        /// <summary>
        /// This call returns a list of roles given a permission
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_role">The opaque_ref of the given permission</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Role>> get_by_permission(Session session, string _role)
        {
            return session.JsonRpcClient.role_get_by_permission(session.opaque_ref, _role);
        }

        /// <summary>
        /// This call returns a list of roles given a permission name
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">The short friendly name of the role</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Role>> get_by_permission_name_label(Session session, string _label)
        {
            return session.JsonRpcClient.role_get_by_permission_name_label(session.opaque_ref, _label);
        }

        /// <summary>
        /// Return a list of all the roles known to the system.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Role>> get_all(Session session)
        {
            return session.JsonRpcClient.role_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of role references to role records for all roles known to the system.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Role>, Role> get_all_records(Session session)
        {
            return session.JsonRpcClient.role_get_all_records(session.opaque_ref);
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
        /// a short user-friendly name for the role
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
        /// what this role is for
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
        /// a list of pointers to other roles or permissions
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Role>))]
        public virtual List<XenRef<Role>> subroles
        {
            get { return _subroles; }
            set
            {
                if (!Helper.AreEqual(value, _subroles))
                {
                    _subroles = value;
                    NotifyPropertyChanged("subroles");
                }
            }
        }
        private List<XenRef<Role>> _subroles = new List<XenRef<Role>>() {};

        /// <summary>
        /// Indicates whether the role is only to be assigned internally by xapi, or can be used by clients
        /// First published in 22.5.0.
        /// </summary>
        public virtual bool is_internal
        {
            get { return _is_internal; }
            set
            {
                if (!Helper.AreEqual(value, _is_internal))
                {
                    _is_internal = value;
                    NotifyPropertyChanged("is_internal");
                }
            }
        }
        private bool _is_internal = false;
    }
}
