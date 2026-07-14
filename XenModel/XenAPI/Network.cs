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
    /// A virtual network
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class Network : XenObject<Network>
    {
        #region Constructors

        public Network()
        {
        }

        public Network(string uuid,
            string name_label,
            string name_description,
            List<network_operations> allowed_operations,
            Dictionary<string, network_operations> current_operations,
            List<XenRef<VIF>> VIFs,
            List<XenRef<PIF>> PIFs,
            long MTU,
            Dictionary<string, string> other_config,
            string bridge,
            bool managed,
            Dictionary<string, XenRef<Blob>> blobs,
            string[] tags,
            network_default_locking_mode default_locking_mode,
            Dictionary<XenRef<VIF>, string> assigned_ips,
            List<network_purpose> purpose)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.VIFs = VIFs;
            this.PIFs = PIFs;
            this.MTU = MTU;
            this.other_config = other_config;
            this.bridge = bridge;
            this.managed = managed;
            this.blobs = blobs;
            this.tags = tags;
            this.default_locking_mode = default_locking_mode;
            this.assigned_ips = assigned_ips;
            this.purpose = purpose;
        }

        /// <summary>
        /// Creates a new Network from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Network(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Network.
        /// </summary>
        public override void UpdateFrom(Network record)
        {
            uuid = record.uuid;
            name_label = record.name_label;
            name_description = record.name_description;
            allowed_operations = record.allowed_operations;
            current_operations = record.current_operations;
            VIFs = record.VIFs;
            PIFs = record.PIFs;
            MTU = record.MTU;
            other_config = record.other_config;
            bridge = record.bridge;
            managed = record.managed;
            blobs = record.blobs;
            tags = record.tags;
            default_locking_mode = record.default_locking_mode;
            assigned_ips = record.assigned_ips;
            purpose = record.purpose;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Network
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
            if (table.ContainsKey("allowed_operations"))
                allowed_operations = Helper.StringArrayToEnumList<network_operations>(Marshalling.ParseStringArray(table, "allowed_operations"));
            if (table.ContainsKey("current_operations"))
                current_operations = Maps.ToDictionary_string_network_operations(Marshalling.ParseHashTable(table, "current_operations"));
            if (table.ContainsKey("VIFs"))
                VIFs = Marshalling.ParseSetRef<VIF>(table, "VIFs");
            if (table.ContainsKey("PIFs"))
                PIFs = Marshalling.ParseSetRef<PIF>(table, "PIFs");
            if (table.ContainsKey("MTU"))
                MTU = Marshalling.ParseLong(table, "MTU");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("bridge"))
                bridge = Marshalling.ParseString(table, "bridge");
            if (table.ContainsKey("managed"))
                managed = Marshalling.ParseBool(table, "managed");
            if (table.ContainsKey("blobs"))
                blobs = Maps.ToDictionary_string_XenRefBlob(Marshalling.ParseHashTable(table, "blobs"));
            if (table.ContainsKey("tags"))
                tags = Marshalling.ParseStringArray(table, "tags");
            if (table.ContainsKey("default_locking_mode"))
                default_locking_mode = (network_default_locking_mode)Helper.EnumParseDefault(typeof(network_default_locking_mode), Marshalling.ParseString(table, "default_locking_mode"));
            if (table.ContainsKey("assigned_ips"))
                assigned_ips = Maps.ToDictionary_XenRefVIF_string(Marshalling.ParseHashTable(table, "assigned_ips"));
            if (table.ContainsKey("purpose"))
                purpose = Helper.StringArrayToEnumList<network_purpose>(Marshalling.ParseStringArray(table, "purpose"));
        }

        public bool DeepEquals(Network other, bool ignoreCurrentOperations)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (!ignoreCurrentOperations && !Helper.AreEqual2(current_operations, other.current_operations))
                return false;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name_label, other._name_label) &&
                Helper.AreEqual2(_name_description, other._name_description) &&
                Helper.AreEqual2(_allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(_VIFs, other._VIFs) &&
                Helper.AreEqual2(_PIFs, other._PIFs) &&
                Helper.AreEqual2(_MTU, other._MTU) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_bridge, other._bridge) &&
                Helper.AreEqual2(_managed, other._managed) &&
                Helper.AreEqual2(_blobs, other._blobs) &&
                Helper.AreEqual2(_tags, other._tags) &&
                Helper.AreEqual2(_default_locking_mode, other._default_locking_mode) &&
                Helper.AreEqual2(_assigned_ips, other._assigned_ips) &&
                Helper.AreEqual2(_purpose, other._purpose);
        }


        /// <summary>
        /// Get a record containing the current state of the given network.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Network get_record(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_record(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get a reference to the network instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Network> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.network_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Create a new network instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Network> create(Session session, Network _record)
        {
            return session.JsonRpcClient.network_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Create a new network instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_create(Session session, Network _record)
        {
          return session.JsonRpcClient.async_network_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Destroy the specified network instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void destroy(Session session, string _network)
        {
            session.JsonRpcClient.network_destroy(session.opaque_ref, _network);
        }

        /// <summary>
        /// Destroy the specified network instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _network)
        {
          return session.JsonRpcClient.async_network_destroy(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get all the network instances with the given label.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Network>> get_by_name_label(Session session, string _label)
        {
            return session.JsonRpcClient.network_get_by_name_label(session.opaque_ref, _label);
        }

        /// <summary>
        /// Get the uuid field of the given network.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_uuid(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get the name/label field of the given network.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_label(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_name_label(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get the name/description field of the given network.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_description(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_name_description(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get the allowed_operations field of the given network.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<network_operations> get_allowed_operations(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_allowed_operations(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get the current_operations field of the given network.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, network_operations> get_current_operations(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_current_operations(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get the VIFs field of the given network.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VIF>> get_VIFs(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_vifs(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get the PIFs field of the given network.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PIF>> get_PIFs(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_pifs(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get the MTU field of the given network.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_MTU(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_mtu(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get the other_config field of the given network.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_other_config(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get the bridge field of the given network.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_bridge(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_bridge(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get the managed field of the given network.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_managed(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_managed(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get the blobs field of the given network.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, XenRef<Blob>> get_blobs(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_blobs(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get the tags field of the given network.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_tags(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_tags(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get the default_locking_mode field of the given network.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static network_default_locking_mode get_default_locking_mode(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_default_locking_mode(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get the assigned_ips field of the given network.
        /// First published in XenServer 6.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<VIF>, string> get_assigned_ips(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_assigned_ips(session.opaque_ref, _network);
        }

        /// <summary>
        /// Get the purpose field of the given network.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<network_purpose> get_purpose(Session session, string _network)
        {
            return session.JsonRpcClient.network_get_purpose(session.opaque_ref, _network);
        }

        /// <summary>
        /// Set the name/label field of the given network.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_label">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_name_label(Session session, string _network, string _label)
        {
            session.JsonRpcClient.network_set_name_label(session.opaque_ref, _network, _label);
        }

        /// <summary>
        /// Set the name/description field of the given network.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_description">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_name_description(Session session, string _network, string _description)
        {
            session.JsonRpcClient.network_set_name_description(session.opaque_ref, _network, _description);
        }

        /// <summary>
        /// Set the MTU field of the given network.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_mtu">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_MTU(Session session, string _network, long _mtu)
        {
            session.JsonRpcClient.network_set_mtu(session.opaque_ref, _network, _mtu);
        }

        /// <summary>
        /// Set the other_config field of the given network.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_other_config(Session session, string _network, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.network_set_other_config(session.opaque_ref, _network, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given network.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_other_config(Session session, string _network, string _key, string _value)
        {
            session.JsonRpcClient.network_add_to_other_config(session.opaque_ref, _network, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given network.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_other_config(Session session, string _network, string _key)
        {
            session.JsonRpcClient.network_remove_from_other_config(session.opaque_ref, _network, _key);
        }

        /// <summary>
        /// Set the tags field of the given network.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_tags">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void set_tags(Session session, string _network, string[] _tags)
        {
            session.JsonRpcClient.network_set_tags(session.opaque_ref, _network, _tags);
        }

        /// <summary>
        /// Add the given value to the tags field of the given network.  If the value is already in that Set, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_value">New value to add</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void add_tags(Session session, string _network, string _value)
        {
            session.JsonRpcClient.network_add_tags(session.opaque_ref, _network, _value);
        }

        /// <summary>
        /// Remove the given value from the tags field of the given network.  If the value is not in that Set, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_value">Value to remove</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void remove_tags(Session session, string _network, string _value)
        {
            session.JsonRpcClient.network_remove_tags(session.opaque_ref, _network, _value);
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this pool
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Blob> create_new_blob(Session session, string _network, string _name, string _mime_type)
        {
            return session.JsonRpcClient.network_create_new_blob(session.opaque_ref, _network, _name, _mime_type);
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this pool
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create_new_blob(Session session, string _network, string _name, string _mime_type)
        {
          return session.JsonRpcClient.async_network_create_new_blob(session.opaque_ref, _network, _name, _mime_type);
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this pool
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <param name="_public">True if the blob should be publicly available First published in XenServer 6.1.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Blob> create_new_blob(Session session, string _network, string _name, string _mime_type, bool _public)
        {
            return session.JsonRpcClient.network_create_new_blob(session.opaque_ref, _network, _name, _mime_type, _public);
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this pool
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <param name="_public">True if the blob should be publicly available First published in XenServer 6.1.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create_new_blob(Session session, string _network, string _name, string _mime_type, bool _public)
        {
          return session.JsonRpcClient.async_network_create_new_blob(session.opaque_ref, _network, _name, _mime_type, _public);
        }

        /// <summary>
        /// Set the default locking mode for VIFs attached to this network
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_value">The default locking mode for VIFs attached to this network.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_default_locking_mode(Session session, string _network, network_default_locking_mode _value)
        {
            session.JsonRpcClient.network_set_default_locking_mode(session.opaque_ref, _network, _value);
        }

        /// <summary>
        /// Set the default locking mode for VIFs attached to this network
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_value">The default locking mode for VIFs attached to this network.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_default_locking_mode(Session session, string _network, network_default_locking_mode _value)
        {
          return session.JsonRpcClient.async_network_set_default_locking_mode(session.opaque_ref, _network, _value);
        }

        /// <summary>
        /// Give a network a new purpose (if not present already)
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_value">The purpose to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void add_purpose(Session session, string _network, network_purpose _value)
        {
            session.JsonRpcClient.network_add_purpose(session.opaque_ref, _network, _value);
        }

        /// <summary>
        /// Give a network a new purpose (if not present already)
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_value">The purpose to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_add_purpose(Session session, string _network, network_purpose _value)
        {
          return session.JsonRpcClient.async_network_add_purpose(session.opaque_ref, _network, _value);
        }

        /// <summary>
        /// Remove a purpose from a network (if present)
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_value">The purpose to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void remove_purpose(Session session, string _network, network_purpose _value)
        {
            session.JsonRpcClient.network_remove_purpose(session.opaque_ref, _network, _value);
        }

        /// <summary>
        /// Remove a purpose from a network (if present)
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The opaque_ref of the given network</param>
        /// <param name="_value">The purpose to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_remove_purpose(Session session, string _network, network_purpose _value)
        {
          return session.JsonRpcClient.async_network_remove_purpose(session.opaque_ref, _network, _value);
        }

        /// <summary>
        /// Return a list of all the networks known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Network>> get_all(Session session)
        {
            return session.JsonRpcClient.network_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of network references to network records for all networks known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Network>, Network> get_all_records(Session session)
        {
            return session.JsonRpcClient.network_get_all_records(session.opaque_ref);
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
        /// list of the operations allowed in this state. This list is advisory only and the server state may have changed by the time this field is read by a client.
        /// </summary>
        public virtual List<network_operations> allowed_operations
        {
            get { return _allowed_operations; }
            set
            {
                if (!Helper.AreEqual(value, _allowed_operations))
                {
                    _allowed_operations = value;
                    NotifyPropertyChanged("allowed_operations");
                }
            }
        }
        private List<network_operations> _allowed_operations = new List<network_operations>() {};

        /// <summary>
        /// links each of the running tasks using this object (by reference) to a current_operation enum which describes the nature of the task.
        /// </summary>
        public virtual Dictionary<string, network_operations> current_operations
        {
            get { return _current_operations; }
            set
            {
                if (!Helper.AreEqual(value, _current_operations))
                {
                    _current_operations = value;
                    NotifyPropertyChanged("current_operations");
                }
            }
        }
        private Dictionary<string, network_operations> _current_operations = new Dictionary<string, network_operations>() {};

        /// <summary>
        /// list of connected vifs
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VIF>))]
        public virtual List<XenRef<VIF>> VIFs
        {
            get { return _VIFs; }
            set
            {
                if (!Helper.AreEqual(value, _VIFs))
                {
                    _VIFs = value;
                    NotifyPropertyChanged("VIFs");
                }
            }
        }
        private List<XenRef<VIF>> _VIFs = new List<XenRef<VIF>>() {};

        /// <summary>
        /// list of connected pifs
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<PIF>))]
        public virtual List<XenRef<PIF>> PIFs
        {
            get { return _PIFs; }
            set
            {
                if (!Helper.AreEqual(value, _PIFs))
                {
                    _PIFs = value;
                    NotifyPropertyChanged("PIFs");
                }
            }
        }
        private List<XenRef<PIF>> _PIFs = new List<XenRef<PIF>>() {};

        /// <summary>
        /// MTU in octets
        /// First published in XenServer 5.6.
        /// </summary>
        public virtual long MTU
        {
            get { return _MTU; }
            set
            {
                if (!Helper.AreEqual(value, _MTU))
                {
                    _MTU = value;
                    NotifyPropertyChanged("MTU");
                }
            }
        }
        private long _MTU = 1500;

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
        /// name of the bridge corresponding to this network on the local host
        /// </summary>
        public virtual string bridge
        {
            get { return _bridge; }
            set
            {
                if (!Helper.AreEqual(value, _bridge))
                {
                    _bridge = value;
                    NotifyPropertyChanged("bridge");
                }
            }
        }
        private string _bridge = "";

        /// <summary>
        /// true if the bridge is managed by xapi
        /// First published in XenServer 7.2.
        /// </summary>
        public virtual bool managed
        {
            get { return _managed; }
            set
            {
                if (!Helper.AreEqual(value, _managed))
                {
                    _managed = value;
                    NotifyPropertyChanged("managed");
                }
            }
        }
        private bool _managed = true;

        /// <summary>
        /// Binary blobs associated with this network
        /// First published in XenServer 5.0.
        /// </summary>
        [JsonConverter(typeof(StringXenRefMapConverter<Blob>))]
        public virtual Dictionary<string, XenRef<Blob>> blobs
        {
            get { return _blobs; }
            set
            {
                if (!Helper.AreEqual(value, _blobs))
                {
                    _blobs = value;
                    NotifyPropertyChanged("blobs");
                }
            }
        }
        private Dictionary<string, XenRef<Blob>> _blobs = new Dictionary<string, XenRef<Blob>>() {};

        /// <summary>
        /// user-specified tags for categorization purposes
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual string[] tags
        {
            get { return _tags; }
            set
            {
                if (!Helper.AreEqual(value, _tags))
                {
                    _tags = value;
                    NotifyPropertyChanged("tags");
                }
            }
        }
        private string[] _tags = {};

        /// <summary>
        /// The network will use this value to determine the behaviour of all VIFs where locking_mode = default
        /// First published in XenServer 6.1.
        /// </summary>
        [JsonConverter(typeof(network_default_locking_modeConverter))]
        public virtual network_default_locking_mode default_locking_mode
        {
            get { return _default_locking_mode; }
            set
            {
                if (!Helper.AreEqual(value, _default_locking_mode))
                {
                    _default_locking_mode = value;
                    NotifyPropertyChanged("default_locking_mode");
                }
            }
        }
        private network_default_locking_mode _default_locking_mode = network_default_locking_mode.unlocked;

        /// <summary>
        /// The IP addresses assigned to VIFs on networks that have active xapi-managed DHCP
        /// First published in XenServer 6.5.
        /// </summary>
        [JsonConverter(typeof(XenRefStringMapConverter<VIF>))]
        public virtual Dictionary<XenRef<VIF>, string> assigned_ips
        {
            get { return _assigned_ips; }
            set
            {
                if (!Helper.AreEqual(value, _assigned_ips))
                {
                    _assigned_ips = value;
                    NotifyPropertyChanged("assigned_ips");
                }
            }
        }
        private Dictionary<XenRef<VIF>, string> _assigned_ips = new Dictionary<XenRef<VIF>, string>() {};

        /// <summary>
        /// Set of purposes for which the server will use this network
        /// First published in XenServer 7.3.
        /// </summary>
        public virtual List<network_purpose> purpose
        {
            get { return _purpose; }
            set
            {
                if (!Helper.AreEqual(value, _purpose))
                {
                    _purpose = value;
                    NotifyPropertyChanged("purpose");
                }
            }
        }
        private List<network_purpose> _purpose = new List<network_purpose>() {};
    }
}
