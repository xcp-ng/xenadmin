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
    /// A user of the system
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class User : XenObject<User>
    {
        #region Constructors

        public User()
        {
        }

        public User(string uuid,
            string short_name,
            string fullname,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.short_name = short_name;
            this.fullname = fullname;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new User from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public User(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given User.
        /// </summary>
        public override void UpdateFrom(User record)
        {
            uuid = record.uuid;
            short_name = record.short_name;
            fullname = record.fullname;
            other_config = record.other_config;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this User
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("short_name"))
                short_name = Marshalling.ParseString(table, "short_name");
            if (table.ContainsKey("fullname"))
                fullname = Marshalling.ParseString(table, "fullname");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(User other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_short_name, other._short_name) &&
                Helper.AreEqual2(_fullname, other._fullname) &&
                Helper.AreEqual2(_other_config, other._other_config);
        }


        /// <summary>
        /// Get a record containing the current state of the given user.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        [Deprecated("XenServer 5.5")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static User get_record(Session session, string _user)
        {
            return session.JsonRpcClient.user_get_record(session.opaque_ref, _user);
        }

        /// <summary>
        /// Get a reference to the user instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        [Deprecated("XenServer 5.5")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<User> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.user_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Create a new user instance, and return its handle.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        [Deprecated("XenServer 5.5")]
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<User> create(Session session, User _record)
        {
            return session.JsonRpcClient.user_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Create a new user instance, and return its handle.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        [Deprecated("XenServer 5.5")]
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_create(Session session, User _record)
        {
          return session.JsonRpcClient.async_user_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Destroy the specified user instance.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        [Deprecated("XenServer 5.5")]
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void destroy(Session session, string _user)
        {
            session.JsonRpcClient.user_destroy(session.opaque_ref, _user);
        }

        /// <summary>
        /// Destroy the specified user instance.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        [Deprecated("XenServer 5.5")]
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _user)
        {
          return session.JsonRpcClient.async_user_destroy(session.opaque_ref, _user);
        }

        /// <summary>
        /// Get the uuid field of the given user.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _user)
        {
            return session.JsonRpcClient.user_get_uuid(session.opaque_ref, _user);
        }

        /// <summary>
        /// Get the short_name field of the given user.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_short_name(Session session, string _user)
        {
            return session.JsonRpcClient.user_get_short_name(session.opaque_ref, _user);
        }

        /// <summary>
        /// Get the fullname field of the given user.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_fullname(Session session, string _user)
        {
            return session.JsonRpcClient.user_get_fullname(session.opaque_ref, _user);
        }

        /// <summary>
        /// Get the other_config field of the given user.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _user)
        {
            return session.JsonRpcClient.user_get_other_config(session.opaque_ref, _user);
        }

        /// <summary>
        /// Set the fullname field of the given user.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        /// <param name="_fullname">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_fullname(Session session, string _user, string _fullname)
        {
            session.JsonRpcClient.user_set_fullname(session.opaque_ref, _user, _fullname);
        }

        /// <summary>
        /// Set the other_config field of the given user.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_other_config(Session session, string _user, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.user_set_other_config(session.opaque_ref, _user, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given user.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void add_to_other_config(Session session, string _user, string _key, string _value)
        {
            session.JsonRpcClient.user_add_to_other_config(session.opaque_ref, _user, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given user.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void remove_from_other_config(Session session, string _user, string _key)
        {
            session.JsonRpcClient.user_remove_from_other_config(session.opaque_ref, _user, _key);
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
        /// short name (for example, userid)
        /// </summary>
        public virtual string short_name
        {
            get { return _short_name; }
            set
            {
                if (!Helper.AreEqual(value, _short_name))
                {
                    _short_name = value;
                    NotifyPropertyChanged("short_name");
                }
            }
        }
        private string _short_name = "";

        /// <summary>
        /// full name
        /// </summary>
        public virtual string fullname
        {
            get { return _fullname; }
            set
            {
                if (!Helper.AreEqual(value, _fullname))
                {
                    _fullname = value;
                    NotifyPropertyChanged("fullname");
                }
            }
        }
        private string _fullname = "";

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
