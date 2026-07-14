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
    /// Cluster member metadata
    /// First published in XenServer 7.6.
    /// </summary>
    public partial class Cluster_host : XenObject<Cluster_host>
    {
        #region Constructors

        public Cluster_host()
        {
        }

        public Cluster_host(string uuid,
            XenRef<Cluster> cluster,
            XenRef<Host> host,
            bool enabled,
            XenRef<PIF> PIF,
            bool joined,
            bool live,
            DateTime last_update_live,
            List<cluster_host_operation> allowed_operations,
            Dictionary<string, cluster_host_operation> current_operations,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.cluster = cluster;
            this.host = host;
            this.enabled = enabled;
            this.PIF = PIF;
            this.joined = joined;
            this.live = live;
            this.last_update_live = last_update_live;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Cluster_host from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Cluster_host(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Cluster_host.
        /// </summary>
        public override void UpdateFrom(Cluster_host record)
        {
            uuid = record.uuid;
            cluster = record.cluster;
            host = record.host;
            enabled = record.enabled;
            PIF = record.PIF;
            joined = record.joined;
            live = record.live;
            last_update_live = record.last_update_live;
            allowed_operations = record.allowed_operations;
            current_operations = record.current_operations;
            other_config = record.other_config;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Cluster_host
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("cluster"))
                cluster = Marshalling.ParseRef<Cluster>(table, "cluster");
            if (table.ContainsKey("host"))
                host = Marshalling.ParseRef<Host>(table, "host");
            if (table.ContainsKey("enabled"))
                enabled = Marshalling.ParseBool(table, "enabled");
            if (table.ContainsKey("PIF"))
                PIF = Marshalling.ParseRef<PIF>(table, "PIF");
            if (table.ContainsKey("joined"))
                joined = Marshalling.ParseBool(table, "joined");
            if (table.ContainsKey("live"))
                live = Marshalling.ParseBool(table, "live");
            if (table.ContainsKey("last_update_live"))
                last_update_live = Marshalling.ParseDateTime(table, "last_update_live");
            if (table.ContainsKey("allowed_operations"))
                allowed_operations = Helper.StringArrayToEnumList<cluster_host_operation>(Marshalling.ParseStringArray(table, "allowed_operations"));
            if (table.ContainsKey("current_operations"))
                current_operations = Maps.ToDictionary_string_cluster_host_operation(Marshalling.ParseHashTable(table, "current_operations"));
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Cluster_host other, bool ignoreCurrentOperations)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (!ignoreCurrentOperations && !Helper.AreEqual2(current_operations, other.current_operations))
                return false;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_cluster, other._cluster) &&
                Helper.AreEqual2(_host, other._host) &&
                Helper.AreEqual2(_enabled, other._enabled) &&
                Helper.AreEqual2(_PIF, other._PIF) &&
                Helper.AreEqual2(_joined, other._joined) &&
                Helper.AreEqual2(_live, other._live) &&
                Helper.AreEqual2(_last_update_live, other._last_update_live) &&
                Helper.AreEqual2(_allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(_other_config, other._other_config);
        }


        /// <summary>
        /// Get a record containing the current state of the given Cluster_host.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Cluster_host get_record(Session session, string _cluster_host)
        {
            return session.JsonRpcClient.cluster_host_get_record(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Get a reference to the Cluster_host instance with the specified UUID.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Cluster_host> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.cluster_host_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given Cluster_host.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _cluster_host)
        {
            return session.JsonRpcClient.cluster_host_get_uuid(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Get the cluster field of the given Cluster_host.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Cluster> get_cluster(Session session, string _cluster_host)
        {
            return session.JsonRpcClient.cluster_host_get_cluster(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Get the host field of the given Cluster_host.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host> get_host(Session session, string _cluster_host)
        {
            return session.JsonRpcClient.cluster_host_get_host(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Get the enabled field of the given Cluster_host.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_enabled(Session session, string _cluster_host)
        {
            return session.JsonRpcClient.cluster_host_get_enabled(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Get the PIF field of the given Cluster_host.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PIF> get_PIF(Session session, string _cluster_host)
        {
            return session.JsonRpcClient.cluster_host_get_pif(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Get the joined field of the given Cluster_host.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_joined(Session session, string _cluster_host)
        {
            return session.JsonRpcClient.cluster_host_get_joined(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Get the live field of the given Cluster_host.
        /// Experimental. First published in 24.3.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_live(Session session, string _cluster_host)
        {
            return session.JsonRpcClient.cluster_host_get_live(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Get the last_update_live field of the given Cluster_host.
        /// Experimental. First published in 24.3.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_last_update_live(Session session, string _cluster_host)
        {
            return session.JsonRpcClient.cluster_host_get_last_update_live(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Get the allowed_operations field of the given Cluster_host.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<cluster_host_operation> get_allowed_operations(Session session, string _cluster_host)
        {
            return session.JsonRpcClient.cluster_host_get_allowed_operations(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Get the current_operations field of the given Cluster_host.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, cluster_host_operation> get_current_operations(Session session, string _cluster_host)
        {
            return session.JsonRpcClient.cluster_host_get_current_operations(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Get the other_config field of the given Cluster_host.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _cluster_host)
        {
            return session.JsonRpcClient.cluster_host_get_other_config(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Add a new host to an existing cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">Cluster to join</param>
        /// <param name="_host">new cluster member</param>
        /// <param name="_pif">Network interface to use for communication</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Cluster_host> create(Session session, string _cluster, string _host, string _pif)
        {
            return session.JsonRpcClient.cluster_host_create(session.opaque_ref, _cluster, _host, _pif);
        }

        /// <summary>
        /// Add a new host to an existing cluster.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">Cluster to join</param>
        /// <param name="_host">new cluster member</param>
        /// <param name="_pif">Network interface to use for communication</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, string _cluster, string _host, string _pif)
        {
          return session.JsonRpcClient.async_cluster_host_create(session.opaque_ref, _cluster, _host, _pif);
        }

        /// <summary>
        /// Remove the host from an existing cluster. This operation is allowed even if a cluster host is not enabled.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _cluster_host)
        {
            session.JsonRpcClient.cluster_host_destroy(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Remove the host from an existing cluster. This operation is allowed even if a cluster host is not enabled.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _cluster_host)
        {
          return session.JsonRpcClient.async_cluster_host_destroy(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Enable cluster membership for a disabled cluster host.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void enable(Session session, string _cluster_host)
        {
            session.JsonRpcClient.cluster_host_enable(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Enable cluster membership for a disabled cluster host.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_enable(Session session, string _cluster_host)
        {
          return session.JsonRpcClient.async_cluster_host_enable(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Remove a host from an existing cluster forcefully.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void force_destroy(Session session, string _cluster_host)
        {
            session.JsonRpcClient.cluster_host_force_destroy(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Remove a host from an existing cluster forcefully.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_force_destroy(Session session, string _cluster_host)
        {
          return session.JsonRpcClient.async_cluster_host_force_destroy(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Disable cluster membership for an enabled cluster host.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void disable(Session session, string _cluster_host)
        {
            session.JsonRpcClient.cluster_host_disable(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Disable cluster membership for an enabled cluster host.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_disable(Session session, string _cluster_host)
        {
          return session.JsonRpcClient.async_cluster_host_disable(session.opaque_ref, _cluster_host);
        }

        /// <summary>
        /// Return a list of all the Cluster_hosts known to the system.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Cluster_host>> get_all(Session session)
        {
            return session.JsonRpcClient.cluster_host_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of Cluster_host references to Cluster_host records for all Cluster_hosts known to the system.
        /// First published in XenServer 7.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Cluster_host>, Cluster_host> get_all_records(Session session)
        {
            return session.JsonRpcClient.cluster_host_get_all_records(session.opaque_ref);
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
        /// Reference to the Cluster object
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Cluster>))]
        public virtual XenRef<Cluster> cluster
        {
            get { return _cluster; }
            set
            {
                if (!Helper.AreEqual(value, _cluster))
                {
                    _cluster = value;
                    NotifyPropertyChanged("cluster");
                }
            }
        }
        private XenRef<Cluster> _cluster = new XenRef<Cluster>("OpaqueRef:NULL");

        /// <summary>
        /// Reference to the Host object
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
        private XenRef<Host> _host = new XenRef<Host>("OpaqueRef:NULL");

        /// <summary>
        /// Whether the cluster host believes that clustering should be enabled on this host. This field can be altered by calling the enable/disable message on a cluster host. Only enabled members run the underlying cluster stack. Disabled members are still considered a member of the cluster (see joined), and can be re-enabled by the user.
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
        /// Reference to the PIF object
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PIF>))]
        public virtual XenRef<PIF> PIF
        {
            get { return _PIF; }
            set
            {
                if (!Helper.AreEqual(value, _PIF))
                {
                    _PIF = value;
                    NotifyPropertyChanged("PIF");
                }
            }
        }
        private XenRef<PIF> _PIF = new XenRef<PIF>("OpaqueRef:NULL");

        /// <summary>
        /// Whether the cluster host has joined the cluster. Contrary to enabled, a host that is not joined is not considered a member of the cluster, and hence enable and disable operations cannot be performed on this host.
        /// </summary>
        public virtual bool joined
        {
            get { return _joined; }
            set
            {
                if (!Helper.AreEqual(value, _joined))
                {
                    _joined = value;
                    NotifyPropertyChanged("joined");
                }
            }
        }
        private bool _joined = true;

        /// <summary>
        /// Whether the underlying cluster stack thinks we are live. This field is set automatically based on updates from the cluster stack and cannot be altered by the user.
        /// Experimental. First published in 24.3.0.
        /// </summary>
        public virtual bool live
        {
            get { return _live; }
            set
            {
                if (!Helper.AreEqual(value, _live))
                {
                    _live = value;
                    NotifyPropertyChanged("live");
                }
            }
        }
        private bool _live = false;

        /// <summary>
        /// Time when the live field was last updated based on information from the cluster stack
        /// Experimental. First published in 24.3.0.
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime last_update_live
        {
            get { return _last_update_live; }
            set
            {
                if (!Helper.AreEqual(value, _last_update_live))
                {
                    _last_update_live = value;
                    NotifyPropertyChanged("last_update_live");
                }
            }
        }
        private DateTime _last_update_live = DateTime.ParseExact("19700101T00:00:00Z", "yyyyMMddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        /// <summary>
        /// list of the operations allowed in this state. This list is advisory only and the server state may have changed by the time this field is read by a client.
        /// </summary>
        public virtual List<cluster_host_operation> allowed_operations
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
        private List<cluster_host_operation> _allowed_operations = new List<cluster_host_operation>() {};

        /// <summary>
        /// links each of the running tasks using this object (by reference) to a current_operation enum which describes the nature of the task.
        /// </summary>
        public virtual Dictionary<string, cluster_host_operation> current_operations
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
        private Dictionary<string, cluster_host_operation> _current_operations = new Dictionary<string, cluster_host_operation>() {};

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
