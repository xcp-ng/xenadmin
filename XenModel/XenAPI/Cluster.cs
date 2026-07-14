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
    /// Cluster-wide Cluster metadata
    /// First published in XenServer 7.6.
    /// </summary>
    public partial class Cluster : XenObject<Cluster>
    {
        #region Constructors

        public Cluster()
        {
        }

        public Cluster(string uuid,
            List<XenRef<Cluster_host>> cluster_hosts,
            string[] pending_forget,
            string cluster_token,
            string cluster_stack,
            long cluster_stack_version,
            bool is_quorate,
            long quorum,
            long live_hosts,
            long expected_hosts,
            List<cluster_operation> allowed_operations,
            Dictionary<string, cluster_operation> current_operations,
            bool pool_auto_join,
            double token_timeout,
            double token_timeout_coefficient,
            Dictionary<string, string> cluster_config,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.cluster_hosts = cluster_hosts;
            this.pending_forget = pending_forget;
            this.cluster_token = cluster_token;
            this.cluster_stack = cluster_stack;
            this.cluster_stack_version = cluster_stack_version;
            this.is_quorate = is_quorate;
            this.quorum = quorum;
            this.live_hosts = live_hosts;
            this.expected_hosts = expected_hosts;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.pool_auto_join = pool_auto_join;
            this.token_timeout = token_timeout;
            this.token_timeout_coefficient = token_timeout_coefficient;
            this.cluster_config = cluster_config;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Cluster from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Cluster(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Cluster.
        /// </summary>
        public override void UpdateFrom(Cluster record)
        {
            uuid = record.uuid;
            cluster_hosts = record.cluster_hosts;
            pending_forget = record.pending_forget;
            cluster_token = record.cluster_token;
            cluster_stack = record.cluster_stack;
            cluster_stack_version = record.cluster_stack_version;
            is_quorate = record.is_quorate;
            quorum = record.quorum;
            live_hosts = record.live_hosts;
            expected_hosts = record.expected_hosts;
            allowed_operations = record.allowed_operations;
            current_operations = record.current_operations;
            pool_auto_join = record.pool_auto_join;
            token_timeout = record.token_timeout;
            token_timeout_coefficient = record.token_timeout_coefficient;
            cluster_config = record.cluster_config;
            other_config = record.other_config;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Cluster
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("cluster_hosts"))
                cluster_hosts = Marshalling.ParseSetRef<Cluster_host>(table, "cluster_hosts");
            if (table.ContainsKey("pending_forget"))
                pending_forget = Marshalling.ParseStringArray(table, "pending_forget");
            if (table.ContainsKey("cluster_token"))
                cluster_token = Marshalling.ParseString(table, "cluster_token");
            if (table.ContainsKey("cluster_stack"))
                cluster_stack = Marshalling.ParseString(table, "cluster_stack");
            if (table.ContainsKey("cluster_stack_version"))
                cluster_stack_version = Marshalling.ParseLong(table, "cluster_stack_version");
            if (table.ContainsKey("is_quorate"))
                is_quorate = Marshalling.ParseBool(table, "is_quorate");
            if (table.ContainsKey("quorum"))
                quorum = Marshalling.ParseLong(table, "quorum");
            if (table.ContainsKey("live_hosts"))
                live_hosts = Marshalling.ParseLong(table, "live_hosts");
            if (table.ContainsKey("expected_hosts"))
                expected_hosts = Marshalling.ParseLong(table, "expected_hosts");
            if (table.ContainsKey("allowed_operations"))
                allowed_operations = Helper.StringArrayToEnumList<cluster_operation>(Marshalling.ParseStringArray(table, "allowed_operations"));
            if (table.ContainsKey("current_operations"))
                current_operations = Maps.ToDictionary_string_cluster_operation(Marshalling.ParseHashTable(table, "current_operations"));
            if (table.ContainsKey("pool_auto_join"))
                pool_auto_join = Marshalling.ParseBool(table, "pool_auto_join");
            if (table.ContainsKey("token_timeout"))
                token_timeout = Marshalling.ParseDouble(table, "token_timeout");
            if (table.ContainsKey("token_timeout_coefficient"))
                token_timeout_coefficient = Marshalling.ParseDouble(table, "token_timeout_coefficient");
            if (table.ContainsKey("cluster_config"))
                cluster_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "cluster_config"));
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Cluster other, bool ignoreCurrentOperations)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (!ignoreCurrentOperations && !Helper.AreEqual2(current_operations, other.current_operations))
                return false;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_cluster_hosts, other._cluster_hosts) &&
                Helper.AreEqual2(_pending_forget, other._pending_forget) &&
                Helper.AreEqual2(_cluster_token, other._cluster_token) &&
                Helper.AreEqual2(_cluster_stack, other._cluster_stack) &&
                Helper.AreEqual2(_cluster_stack_version, other._cluster_stack_version) &&
                Helper.AreEqual2(_is_quorate, other._is_quorate) &&
                Helper.AreEqual2(_quorum, other._quorum) &&
                Helper.AreEqual2(_live_hosts, other._live_hosts) &&
                Helper.AreEqual2(_expected_hosts, other._expected_hosts) &&
                Helper.AreEqual2(_allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(_pool_auto_join, other._pool_auto_join) &&
                Helper.AreEqual2(_token_timeout, other._token_timeout) &&
                Helper.AreEqual2(_token_timeout_coefficient, other._token_timeout_coefficient) &&
                Helper.AreEqual2(_cluster_config, other._cluster_config) &&
                Helper.AreEqual2(_other_config, other._other_config);
        }


        /// <summary>
        /// Get a record containing the current state of the given Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Cluster get_record(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_record(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get a reference to the Cluster instance with the specified UUID.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Cluster> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.cluster_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_uuid(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get the cluster_hosts field of the given Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Cluster_host>> get_cluster_hosts(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_cluster_hosts(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get the pending_forget field of the given Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_pending_forget(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_pending_forget(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get the cluster_token field of the given Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_cluster_token(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_cluster_token(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get the cluster_stack field of the given Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_cluster_stack(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_cluster_stack(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get the cluster_stack_version field of the given Cluster.
        /// Experimental. First published in 24.15.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_cluster_stack_version(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_cluster_stack_version(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get the is_quorate field of the given Cluster.
        /// Experimental. First published in 24.3.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_is_quorate(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_is_quorate(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get the quorum field of the given Cluster.
        /// Experimental. First published in 24.3.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_quorum(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_quorum(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get the live_hosts field of the given Cluster.
        /// Experimental. First published in 24.3.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_live_hosts(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_live_hosts(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get the expected_hosts field of the given Cluster.
        /// Experimental. First published in 25.17.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_expected_hosts(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_expected_hosts(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get the allowed_operations field of the given Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<cluster_operation> get_allowed_operations(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_allowed_operations(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get the current_operations field of the given Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, cluster_operation> get_current_operations(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_current_operations(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get the pool_auto_join field of the given Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_pool_auto_join(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_pool_auto_join(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get the token_timeout field of the given Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static double get_token_timeout(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_token_timeout(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get the token_timeout_coefficient field of the given Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static double get_token_timeout_coefficient(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_token_timeout_coefficient(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get the cluster_config field of the given Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_cluster_config(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_cluster_config(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Get the other_config field of the given Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_other_config(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Set the other_config field of the given Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_other_config(Session session, string _cluster, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.cluster_set_other_config(session.opaque_ref, _cluster, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_other_config(Session session, string _cluster, string _key, string _value)
        {
            session.JsonRpcClient.cluster_add_to_other_config(session.opaque_ref, _cluster, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given Cluster.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_other_config(Session session, string _cluster, string _key)
        {
            session.JsonRpcClient.cluster_remove_from_other_config(session.opaque_ref, _cluster, _key);
        }

        /// <summary>
        /// Creates a Cluster object and one Cluster_host object as its first member
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The PIF to connect the cluster's first cluster_host to</param>
        /// <param name="_cluster_stack">simply the string 'corosync'. No other cluster stacks are currently supported</param>
        /// <param name="_pool_auto_join">true if xapi is automatically joining new pool members to the cluster</param>
        /// <param name="_token_timeout">Corosync token timeout in seconds</param>
        /// <param name="_token_timeout_coefficient">Corosync token timeout coefficient in seconds</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Cluster> create(Session session, string _pif, string _cluster_stack, bool _pool_auto_join, double _token_timeout, double _token_timeout_coefficient)
        {
            return session.JsonRpcClient.cluster_create(session.opaque_ref, _pif, _cluster_stack, _pool_auto_join, _token_timeout, _token_timeout_coefficient);
        }

        /// <summary>
        /// Creates a Cluster object and one Cluster_host object as its first member
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">The PIF to connect the cluster's first cluster_host to</param>
        /// <param name="_cluster_stack">simply the string 'corosync'. No other cluster stacks are currently supported</param>
        /// <param name="_pool_auto_join">true if xapi is automatically joining new pool members to the cluster</param>
        /// <param name="_token_timeout">Corosync token timeout in seconds</param>
        /// <param name="_token_timeout_coefficient">Corosync token timeout coefficient in seconds</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, string _pif, string _cluster_stack, bool _pool_auto_join, double _token_timeout, double _token_timeout_coefficient)
        {
          return session.JsonRpcClient.async_cluster_create(session.opaque_ref, _pif, _cluster_stack, _pool_auto_join, _token_timeout, _token_timeout_coefficient);
        }

        /// <summary>
        /// Destroys a Cluster object and the one remaining Cluster_host member
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _cluster)
        {
            session.JsonRpcClient.cluster_destroy(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Destroys a Cluster object and the one remaining Cluster_host member
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _cluster)
        {
          return session.JsonRpcClient.async_cluster_destroy(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Returns the network used by the cluster for inter-host communication, i.e. the network shared by all cluster host PIFs
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Network> get_network(Session session, string _cluster)
        {
            return session.JsonRpcClient.cluster_get_network(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Returns the network used by the cluster for inter-host communication, i.e. the network shared by all cluster host PIFs
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_get_network(Session session, string _cluster)
        {
          return session.JsonRpcClient.async_cluster_get_network(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Attempt to create a Cluster from the entire pool
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">the single network on which corosync carries out its inter-host communications</param>
        /// <param name="_cluster_stack">simply the string 'corosync'. No other cluster stacks are currently supported</param>
        /// <param name="_token_timeout">Corosync token timeout in seconds</param>
        /// <param name="_token_timeout_coefficient">Corosync token timeout coefficient in seconds</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Cluster> pool_create(Session session, string _network, string _cluster_stack, double _token_timeout, double _token_timeout_coefficient)
        {
            return session.JsonRpcClient.cluster_pool_create(session.opaque_ref, _network, _cluster_stack, _token_timeout, _token_timeout_coefficient);
        }

        /// <summary>
        /// Attempt to create a Cluster from the entire pool
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">the single network on which corosync carries out its inter-host communications</param>
        /// <param name="_cluster_stack">simply the string 'corosync'. No other cluster stacks are currently supported</param>
        /// <param name="_token_timeout">Corosync token timeout in seconds</param>
        /// <param name="_token_timeout_coefficient">Corosync token timeout coefficient in seconds</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_pool_create(Session session, string _network, string _cluster_stack, double _token_timeout, double _token_timeout_coefficient)
        {
          return session.JsonRpcClient.async_cluster_pool_create(session.opaque_ref, _network, _cluster_stack, _token_timeout, _token_timeout_coefficient);
        }

        /// <summary>
        /// Attempt to force destroy the Cluster_host objects, and then destroy the Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void pool_force_destroy(Session session, string _cluster)
        {
            session.JsonRpcClient.cluster_pool_force_destroy(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Attempt to force destroy the Cluster_host objects, and then destroy the Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_pool_force_destroy(Session session, string _cluster)
        {
          return session.JsonRpcClient.async_cluster_pool_force_destroy(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Attempt to destroy the Cluster_host objects for all hosts in the pool and then destroy the Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void pool_destroy(Session session, string _cluster)
        {
            session.JsonRpcClient.cluster_pool_destroy(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Attempt to destroy the Cluster_host objects for all hosts in the pool and then destroy the Cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_pool_destroy(Session session, string _cluster)
        {
          return session.JsonRpcClient.async_cluster_pool_destroy(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Resynchronise the cluster_host objects across the pool. Creates them where they need creating and then plugs them
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void pool_resync(Session session, string _cluster)
        {
            session.JsonRpcClient.cluster_pool_resync(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Resynchronise the cluster_host objects across the pool. Creates them where they need creating and then plugs them
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_pool_resync(Session session, string _cluster)
        {
          return session.JsonRpcClient.async_cluster_pool_resync(session.opaque_ref, _cluster);
        }

        /// <summary>
        /// Return a list of all the Clusters known to the system.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Cluster>> get_all(Session session)
        {
            return session.JsonRpcClient.cluster_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of Cluster references to Cluster records for all Clusters known to the system.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Cluster>, Cluster> get_all_records(Session session)
        {
            return session.JsonRpcClient.cluster_get_all_records(session.opaque_ref);
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
        /// A list of the cluster_host objects associated with the Cluster
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Cluster_host>))]
        public virtual List<XenRef<Cluster_host>> cluster_hosts
        {
            get { return _cluster_hosts; }
            set
            {
                if (!Helper.AreEqual(value, _cluster_hosts))
                {
                    _cluster_hosts = value;
                    NotifyPropertyChanged("cluster_hosts");
                }
            }
        }
        private List<XenRef<Cluster_host>> _cluster_hosts = new List<XenRef<Cluster_host>>() {};

        /// <summary>
        /// Internal field used by Host.destroy to store the IP of cluster members marked as permanently dead but not yet removed
        /// </summary>
        public virtual string[] pending_forget
        {
            get { return _pending_forget; }
            set
            {
                if (!Helper.AreEqual(value, _pending_forget))
                {
                    _pending_forget = value;
                    NotifyPropertyChanged("pending_forget");
                }
            }
        }
        private string[] _pending_forget = {};

        /// <summary>
        /// The secret key used by xapi-clusterd when it talks to itself on other hosts
        /// </summary>
        public virtual string cluster_token
        {
            get { return _cluster_token; }
            set
            {
                if (!Helper.AreEqual(value, _cluster_token))
                {
                    _cluster_token = value;
                    NotifyPropertyChanged("cluster_token");
                }
            }
        }
        private string _cluster_token = "";

        /// <summary>
        /// Simply the string 'corosync'. No other cluster stacks are currently supported
        /// </summary>
        public virtual string cluster_stack
        {
            get { return _cluster_stack; }
            set
            {
                if (!Helper.AreEqual(value, _cluster_stack))
                {
                    _cluster_stack = value;
                    NotifyPropertyChanged("cluster_stack");
                }
            }
        }
        private string _cluster_stack = "corosync";

        /// <summary>
        /// Version of cluster stack, not writable via the API. Defaulting to 2 for backwards compatibility when upgrading from a cluster without this field, which means it is necessarily running version 2 of corosync, the only cluster stack supported so far.
        /// Experimental. First published in 24.15.0.
        /// </summary>
        public virtual long cluster_stack_version
        {
            get { return _cluster_stack_version; }
            set
            {
                if (!Helper.AreEqual(value, _cluster_stack_version))
                {
                    _cluster_stack_version = value;
                    NotifyPropertyChanged("cluster_stack_version");
                }
            }
        }
        private long _cluster_stack_version = 2;

        /// <summary>
        /// Whether the cluster stack thinks the cluster is quorate
        /// Experimental. First published in 24.3.0.
        /// </summary>
        public virtual bool is_quorate
        {
            get { return _is_quorate; }
            set
            {
                if (!Helper.AreEqual(value, _is_quorate))
                {
                    _is_quorate = value;
                    NotifyPropertyChanged("is_quorate");
                }
            }
        }
        private bool _is_quorate = false;

        /// <summary>
        /// Number of live hosts in order to be quorate
        /// Experimental. First published in 24.3.0.
        /// </summary>
        public virtual long quorum
        {
            get { return _quorum; }
            set
            {
                if (!Helper.AreEqual(value, _quorum))
                {
                    _quorum = value;
                    NotifyPropertyChanged("quorum");
                }
            }
        }
        private long _quorum = 0;

        /// <summary>
        /// Current number of live hosts, according to the cluster stack
        /// Experimental. First published in 24.3.0.
        /// </summary>
        public virtual long live_hosts
        {
            get { return _live_hosts; }
            set
            {
                if (!Helper.AreEqual(value, _live_hosts))
                {
                    _live_hosts = value;
                    NotifyPropertyChanged("live_hosts");
                }
            }
        }
        private long _live_hosts = 0;

        /// <summary>
        /// Total number of hosts expected by the cluster stack
        /// Experimental. First published in 25.17.0.
        /// </summary>
        public virtual long expected_hosts
        {
            get { return _expected_hosts; }
            set
            {
                if (!Helper.AreEqual(value, _expected_hosts))
                {
                    _expected_hosts = value;
                    NotifyPropertyChanged("expected_hosts");
                }
            }
        }
        private long _expected_hosts = 0;

        /// <summary>
        /// list of the operations allowed in this state. This list is advisory only and the server state may have changed by the time this field is read by a client.
        /// </summary>
        public virtual List<cluster_operation> allowed_operations
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
        private List<cluster_operation> _allowed_operations = new List<cluster_operation>() {};

        /// <summary>
        /// links each of the running tasks using this object (by reference) to a current_operation enum which describes the nature of the task.
        /// </summary>
        public virtual Dictionary<string, cluster_operation> current_operations
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
        private Dictionary<string, cluster_operation> _current_operations = new Dictionary<string, cluster_operation>() {};

        /// <summary>
        /// True if automatically joining new pool members to the cluster. This will be `true` in the first release
        /// </summary>
        public virtual bool pool_auto_join
        {
            get { return _pool_auto_join; }
            set
            {
                if (!Helper.AreEqual(value, _pool_auto_join))
                {
                    _pool_auto_join = value;
                    NotifyPropertyChanged("pool_auto_join");
                }
            }
        }
        private bool _pool_auto_join = true;

        /// <summary>
        /// The corosync token timeout in seconds
        /// </summary>
        public virtual double token_timeout
        {
            get { return _token_timeout; }
            set
            {
                if (!Helper.AreEqual(value, _token_timeout))
                {
                    _token_timeout = value;
                    NotifyPropertyChanged("token_timeout");
                }
            }
        }
        private double _token_timeout = 20.000;

        /// <summary>
        /// The corosync token timeout coefficient in seconds
        /// </summary>
        public virtual double token_timeout_coefficient
        {
            get { return _token_timeout_coefficient; }
            set
            {
                if (!Helper.AreEqual(value, _token_timeout_coefficient))
                {
                    _token_timeout_coefficient = value;
                    NotifyPropertyChanged("token_timeout_coefficient");
                }
            }
        }
        private double _token_timeout_coefficient = 1.000;

        /// <summary>
        /// Contains read-only settings for the cluster, such as timeouts and other options. It can only be set at cluster create time
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> cluster_config
        {
            get { return _cluster_config; }
            set
            {
                if (!Helper.AreEqual(value, _cluster_config))
                {
                    _cluster_config = value;
                    NotifyPropertyChanged("cluster_config");
                }
            }
        }
        private Dictionary<string, string> _cluster_config = new Dictionary<string, string>() {};

        /// <summary>
        /// Additional configuration
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
