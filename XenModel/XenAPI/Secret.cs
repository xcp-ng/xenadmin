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
    /// A secret
    /// First published in XenServer 5.6.
    /// </summary>
    public partial class Secret : XenObject<Secret>
    {
        #region Constructors

        public Secret()
        {
        }

        public Secret(string uuid,
            string value,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.value = value;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Secret from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Secret(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Secret.
        /// </summary>
        public override void UpdateFrom(Secret record)
        {
            uuid = record.uuid;
            value = record.value;
            other_config = record.other_config;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Secret
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("value"))
                value = Marshalling.ParseString(table, "value");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Secret other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_value, other._value) &&
                Helper.AreEqual2(_other_config, other._other_config);
        }


        /// <summary>
        /// Get a record containing the current state of the given secret.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static Secret get_record(Session session, string _secret)
        {
            return session.JsonRpcClient.secret_get_record(session.opaque_ref, _secret);
        }

        /// <summary>
        /// Get a reference to the secret instance with the specified UUID.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Secret> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.secret_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Create a new secret instance, and return its handle.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Secret> create(Session session, Secret _record)
        {
            return session.JsonRpcClient.secret_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Create a new secret instance, and return its handle.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, Secret _record)
        {
          return session.JsonRpcClient.async_secret_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Destroy the specified secret instance.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _secret)
        {
            session.JsonRpcClient.secret_destroy(session.opaque_ref, _secret);
        }

        /// <summary>
        /// Destroy the specified secret instance.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _secret)
        {
          return session.JsonRpcClient.async_secret_destroy(session.opaque_ref, _secret);
        }

        /// <summary>
        /// Get the uuid field of the given secret.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static string get_uuid(Session session, string _secret)
        {
            return session.JsonRpcClient.secret_get_uuid(session.opaque_ref, _secret);
        }

        /// <summary>
        /// Get the value field of the given secret.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static string get_value(Session session, string _secret)
        {
            return session.JsonRpcClient.secret_get_value(session.opaque_ref, _secret);
        }

        /// <summary>
        /// Get the other_config field of the given secret.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _secret)
        {
            return session.JsonRpcClient.secret_get_other_config(session.opaque_ref, _secret);
        }

        /// <summary>
        /// Set the value field of the given secret.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        /// <param name="_value">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_value(Session session, string _secret, string _value)
        {
            session.JsonRpcClient.secret_set_value(session.opaque_ref, _secret, _value);
        }

        /// <summary>
        /// Set the other_config field of the given secret.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_other_config(Session session, string _secret, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.secret_set_other_config(session.opaque_ref, _secret, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given secret.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_other_config(Session session, string _secret, string _key, string _value)
        {
            session.JsonRpcClient.secret_add_to_other_config(session.opaque_ref, _secret, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given secret.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_other_config(Session session, string _secret, string _key)
        {
            session.JsonRpcClient.secret_remove_from_other_config(session.opaque_ref, _secret, _key);
        }

        /// <summary>
        /// Return a list of all the secrets known to the system.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static List<XenRef<Secret>> get_all(Session session)
        {
            return session.JsonRpcClient.secret_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of secret references to secret records for all secrets known to the system.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static Dictionary<XenRef<Secret>, Secret> get_all_records(Session session)
        {
            return session.JsonRpcClient.secret_get_all_records(session.opaque_ref);
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
        /// the secret
        /// </summary>
        public virtual string value
        {
            get { return _value; }
            set
            {
                if (!Helper.AreEqual(value, _value))
                {
                    _value = value;
                    NotifyPropertyChanged("value");
                }
            }
        }
        private string _value = "";

        /// <summary>
        /// other_config
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
