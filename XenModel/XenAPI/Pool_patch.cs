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
    /// Pool-wide patches
    /// First published in XenServer 4.1.
    /// </summary>
    public partial class Pool_patch : XenObject<Pool_patch>
    {
        #region Constructors

        public Pool_patch()
        {
        }

        public Pool_patch(string uuid,
            string name_label,
            string name_description,
            string version,
            long size,
            bool pool_applied,
            List<XenRef<Host_patch>> host_patches,
            List<after_apply_guidance> after_apply_guidance,
            XenRef<Pool_update> pool_update,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.version = version;
            this.size = size;
            this.pool_applied = pool_applied;
            this.host_patches = host_patches;
            this.after_apply_guidance = after_apply_guidance;
            this.pool_update = pool_update;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Pool_patch from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Pool_patch(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Pool_patch.
        /// </summary>
        public override void UpdateFrom(Pool_patch record)
        {
            uuid = record.uuid;
            name_label = record.name_label;
            name_description = record.name_description;
            version = record.version;
            size = record.size;
            pool_applied = record.pool_applied;
            host_patches = record.host_patches;
            after_apply_guidance = record.after_apply_guidance;
            pool_update = record.pool_update;
            other_config = record.other_config;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Pool_patch
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
            if (table.ContainsKey("version"))
                version = Marshalling.ParseString(table, "version");
            if (table.ContainsKey("size"))
                size = Marshalling.ParseLong(table, "size");
            if (table.ContainsKey("pool_applied"))
                pool_applied = Marshalling.ParseBool(table, "pool_applied");
            if (table.ContainsKey("host_patches"))
                host_patches = Marshalling.ParseSetRef<Host_patch>(table, "host_patches");
            if (table.ContainsKey("after_apply_guidance"))
                after_apply_guidance = Helper.StringArrayToEnumList<after_apply_guidance>(Marshalling.ParseStringArray(table, "after_apply_guidance"));
            if (table.ContainsKey("pool_update"))
                pool_update = Marshalling.ParseRef<Pool_update>(table, "pool_update");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Pool_patch other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name_label, other._name_label) &&
                Helper.AreEqual2(_name_description, other._name_description) &&
                Helper.AreEqual2(_version, other._version) &&
                Helper.AreEqual2(_size, other._size) &&
                Helper.AreEqual2(_pool_applied, other._pool_applied) &&
                Helper.AreEqual2(_host_patches, other._host_patches) &&
                Helper.AreEqual2(_after_apply_guidance, other._after_apply_guidance) &&
                Helper.AreEqual2(_pool_update, other._pool_update) &&
                Helper.AreEqual2(_other_config, other._other_config);
        }


        /// <summary>
        /// Get a record containing the current state of the given pool_patch.
        /// First published in XenServer 4.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Pool_patch get_record(Session session, string _pool_patch)
        {
            return session.JsonRpcClient.pool_patch_get_record(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Get a reference to the pool_patch instance with the specified UUID.
        /// First published in XenServer 4.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Pool_patch> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.pool_patch_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get all the pool_patch instances with the given label.
        /// First published in XenServer 4.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Pool_patch>> get_by_name_label(Session session, string _label)
        {
            return session.JsonRpcClient.pool_patch_get_by_name_label(session.opaque_ref, _label);
        }

        /// <summary>
        /// Get the uuid field of the given pool_patch.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _pool_patch)
        {
            return session.JsonRpcClient.pool_patch_get_uuid(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Get the name/label field of the given pool_patch.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_label(Session session, string _pool_patch)
        {
            return session.JsonRpcClient.pool_patch_get_name_label(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Get the name/description field of the given pool_patch.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_description(Session session, string _pool_patch)
        {
            return session.JsonRpcClient.pool_patch_get_name_description(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Get the version field of the given pool_patch.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_version(Session session, string _pool_patch)
        {
            return session.JsonRpcClient.pool_patch_get_version(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Get the size field of the given pool_patch.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_size(Session session, string _pool_patch)
        {
            return session.JsonRpcClient.pool_patch_get_size(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Get the pool_applied field of the given pool_patch.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_pool_applied(Session session, string _pool_patch)
        {
            return session.JsonRpcClient.pool_patch_get_pool_applied(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Get the host_patches field of the given pool_patch.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Host_patch>> get_host_patches(Session session, string _pool_patch)
        {
            return session.JsonRpcClient.pool_patch_get_host_patches(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Get the after_apply_guidance field of the given pool_patch.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<after_apply_guidance> get_after_apply_guidance(Session session, string _pool_patch)
        {
            return session.JsonRpcClient.pool_patch_get_after_apply_guidance(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Get the pool_update field of the given pool_patch.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Pool_update> get_pool_update(Session session, string _pool_patch)
        {
            return session.JsonRpcClient.pool_patch_get_pool_update(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Get the other_config field of the given pool_patch.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _pool_patch)
        {
            return session.JsonRpcClient.pool_patch_get_other_config(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Set the other_config field of the given pool_patch.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_other_config(Session session, string _pool_patch, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.pool_patch_set_other_config(session.opaque_ref, _pool_patch, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given pool_patch.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_other_config(Session session, string _pool_patch, string _key, string _value)
        {
            session.JsonRpcClient.pool_patch_add_to_other_config(session.opaque_ref, _pool_patch, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given pool_patch.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_other_config(Session session, string _pool_patch, string _key)
        {
            session.JsonRpcClient.pool_patch_remove_from_other_config(session.opaque_ref, _pool_patch, _key);
        }

        /// <summary>
        /// Apply the selected patch to a host and return its output
        /// First published in XenServer 4.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <param name="_host">The host to apply the patch too</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static string apply(Session session, string _pool_patch, string _host)
        {
            return session.JsonRpcClient.pool_patch_apply(session.opaque_ref, _pool_patch, _host);
        }

        /// <summary>
        /// Apply the selected patch to a host and return its output
        /// First published in XenServer 4.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <param name="_host">The host to apply the patch too</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_apply(Session session, string _pool_patch, string _host)
        {
          return session.JsonRpcClient.async_pool_patch_apply(session.opaque_ref, _pool_patch, _host);
        }

        /// <summary>
        /// Apply the selected patch to all hosts in the pool and return a map of host_ref -> patch output
        /// First published in XenServer 4.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void pool_apply(Session session, string _pool_patch)
        {
            session.JsonRpcClient.pool_patch_pool_apply(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Apply the selected patch to all hosts in the pool and return a map of host_ref -> patch output
        /// First published in XenServer 4.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_pool_apply(Session session, string _pool_patch)
        {
          return session.JsonRpcClient.async_pool_patch_pool_apply(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Execute the precheck stage of the selected patch on a host and return its output
        /// First published in XenServer 4.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <param name="_host">The host to run the prechecks on</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static string precheck(Session session, string _pool_patch, string _host)
        {
            return session.JsonRpcClient.pool_patch_precheck(session.opaque_ref, _pool_patch, _host);
        }

        /// <summary>
        /// Execute the precheck stage of the selected patch on a host and return its output
        /// First published in XenServer 4.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <param name="_host">The host to run the prechecks on</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_precheck(Session session, string _pool_patch, string _host)
        {
          return session.JsonRpcClient.async_pool_patch_precheck(session.opaque_ref, _pool_patch, _host);
        }

        /// <summary>
        /// Removes the patch's files from the server
        /// First published in XenServer 4.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void clean(Session session, string _pool_patch)
        {
            session.JsonRpcClient.pool_patch_clean(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Removes the patch's files from the server
        /// First published in XenServer 4.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_clean(Session session, string _pool_patch)
        {
          return session.JsonRpcClient.async_pool_patch_clean(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Removes the patch's files from all hosts in the pool, but does not remove the database entries
        /// First published in XenServer 6.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void pool_clean(Session session, string _pool_patch)
        {
            session.JsonRpcClient.pool_patch_pool_clean(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Removes the patch's files from all hosts in the pool, but does not remove the database entries
        /// First published in XenServer 6.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_pool_clean(Session session, string _pool_patch)
        {
          return session.JsonRpcClient.async_pool_patch_pool_clean(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Removes the patch's files from all hosts in the pool, and removes the database entries.  Only works on unapplied patches.
        /// First published in XenServer 4.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _pool_patch)
        {
            session.JsonRpcClient.pool_patch_destroy(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Removes the patch's files from all hosts in the pool, and removes the database entries.  Only works on unapplied patches.
        /// First published in XenServer 4.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _pool_patch)
        {
          return session.JsonRpcClient.async_pool_patch_destroy(session.opaque_ref, _pool_patch);
        }

        /// <summary>
        /// Removes the patch's files from the specified host
        /// First published in XenServer 6.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <param name="_host">The host on which to clean the patch</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void clean_on_host(Session session, string _pool_patch, string _host)
        {
            session.JsonRpcClient.pool_patch_clean_on_host(session.opaque_ref, _pool_patch, _host);
        }

        /// <summary>
        /// Removes the patch's files from the specified host
        /// First published in XenServer 6.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_patch">The opaque_ref of the given pool_patch</param>
        /// <param name="_host">The host on which to clean the patch</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_clean_on_host(Session session, string _pool_patch, string _host)
        {
          return session.JsonRpcClient.async_pool_patch_clean_on_host(session.opaque_ref, _pool_patch, _host);
        }

        /// <summary>
        /// Return a list of all the pool_patchs known to the system.
        /// First published in XenServer 4.1.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Pool_patch>> get_all(Session session)
        {
            return session.JsonRpcClient.pool_patch_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of pool_patch references to pool_patch records for all pool_patchs known to the system.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Pool_patch>, Pool_patch> get_all_records(Session session)
        {
            return session.JsonRpcClient.pool_patch_get_all_records(session.opaque_ref);
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
        /// Patch version number
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
        /// Size of the patch
        /// </summary>
        public virtual long size
        {
            get { return _size; }
            set
            {
                if (!Helper.AreEqual(value, _size))
                {
                    _size = value;
                    NotifyPropertyChanged("size");
                }
            }
        }
        private long _size = 0;

        /// <summary>
        /// This patch should be applied across the entire pool
        /// </summary>
        public virtual bool pool_applied
        {
            get { return _pool_applied; }
            set
            {
                if (!Helper.AreEqual(value, _pool_applied))
                {
                    _pool_applied = value;
                    NotifyPropertyChanged("pool_applied");
                }
            }
        }
        private bool _pool_applied = false;

        /// <summary>
        /// This hosts this patch is applied to.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Host_patch>))]
        public virtual List<XenRef<Host_patch>> host_patches
        {
            get { return _host_patches; }
            set
            {
                if (!Helper.AreEqual(value, _host_patches))
                {
                    _host_patches = value;
                    NotifyPropertyChanged("host_patches");
                }
            }
        }
        private List<XenRef<Host_patch>> _host_patches = new List<XenRef<Host_patch>>() {};

        /// <summary>
        /// What the client should do after this patch has been applied.
        /// </summary>
        public virtual List<after_apply_guidance> after_apply_guidance
        {
            get { return _after_apply_guidance; }
            set
            {
                if (!Helper.AreEqual(value, _after_apply_guidance))
                {
                    _after_apply_guidance = value;
                    NotifyPropertyChanged("after_apply_guidance");
                }
            }
        }
        private List<after_apply_guidance> _after_apply_guidance = new List<after_apply_guidance>() {};

        /// <summary>
        /// A reference to the associated pool_update object
        /// First published in XenServer 7.1.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Pool_update>))]
        public virtual XenRef<Pool_update> pool_update
        {
            get { return _pool_update; }
            set
            {
                if (!Helper.AreEqual(value, _pool_update))
                {
                    _pool_update = value;
                    NotifyPropertyChanged("pool_update");
                }
            }
        }
        private XenRef<Pool_update> _pool_update = new XenRef<Pool_update>("OpaqueRef:NULL");

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
    }
}
