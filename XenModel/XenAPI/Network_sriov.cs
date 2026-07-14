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
    /// network-sriov which connects logical pif and physical pif
    /// First published in XenServer 7.5.
    /// </summary>
    public partial class Network_sriov : XenObject<Network_sriov>
    {
        #region Constructors

        public Network_sriov()
        {
        }

        public Network_sriov(string uuid,
            XenRef<PIF> physical_PIF,
            XenRef<PIF> logical_PIF,
            bool requires_reboot,
            sriov_configuration_mode configuration_mode)
        {
            this.uuid = uuid;
            this.physical_PIF = physical_PIF;
            this.logical_PIF = logical_PIF;
            this.requires_reboot = requires_reboot;
            this.configuration_mode = configuration_mode;
        }

        /// <summary>
        /// Creates a new Network_sriov from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Network_sriov(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Network_sriov.
        /// </summary>
        public override void UpdateFrom(Network_sriov record)
        {
            uuid = record.uuid;
            physical_PIF = record.physical_PIF;
            logical_PIF = record.logical_PIF;
            requires_reboot = record.requires_reboot;
            configuration_mode = record.configuration_mode;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Network_sriov
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("physical_PIF"))
                physical_PIF = Marshalling.ParseRef<PIF>(table, "physical_PIF");
            if (table.ContainsKey("logical_PIF"))
                logical_PIF = Marshalling.ParseRef<PIF>(table, "logical_PIF");
            if (table.ContainsKey("requires_reboot"))
                requires_reboot = Marshalling.ParseBool(table, "requires_reboot");
            if (table.ContainsKey("configuration_mode"))
                configuration_mode = (sriov_configuration_mode)Helper.EnumParseDefault(typeof(sriov_configuration_mode), Marshalling.ParseString(table, "configuration_mode"));
        }

        public bool DeepEquals(Network_sriov other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_physical_PIF, other._physical_PIF) &&
                Helper.AreEqual2(_logical_PIF, other._logical_PIF) &&
                Helper.AreEqual2(_requires_reboot, other._requires_reboot) &&
                Helper.AreEqual2(_configuration_mode, other._configuration_mode);
        }


        /// <summary>
        /// Get a record containing the current state of the given network_sriov.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Network_sriov get_record(Session session, string _network_sriov)
        {
            return session.JsonRpcClient.network_sriov_get_record(session.opaque_ref, _network_sriov);
        }

        /// <summary>
        /// Get a reference to the network_sriov instance with the specified UUID.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Network_sriov> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.network_sriov_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given network_sriov.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _network_sriov)
        {
            return session.JsonRpcClient.network_sriov_get_uuid(session.opaque_ref, _network_sriov);
        }

        /// <summary>
        /// Get the physical_PIF field of the given network_sriov.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PIF> get_physical_PIF(Session session, string _network_sriov)
        {
            return session.JsonRpcClient.network_sriov_get_physical_pif(session.opaque_ref, _network_sriov);
        }

        /// <summary>
        /// Get the logical_PIF field of the given network_sriov.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PIF> get_logical_PIF(Session session, string _network_sriov)
        {
            return session.JsonRpcClient.network_sriov_get_logical_pif(session.opaque_ref, _network_sriov);
        }

        /// <summary>
        /// Get the requires_reboot field of the given network_sriov.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_requires_reboot(Session session, string _network_sriov)
        {
            return session.JsonRpcClient.network_sriov_get_requires_reboot(session.opaque_ref, _network_sriov);
        }

        /// <summary>
        /// Get the configuration_mode field of the given network_sriov.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static sriov_configuration_mode get_configuration_mode(Session session, string _network_sriov)
        {
            return session.JsonRpcClient.network_sriov_get_configuration_mode(session.opaque_ref, _network_sriov);
        }

        /// <summary>
        /// Enable SR-IOV on the specific PIF. It will create a network-sriov based on the specific PIF and automatically create a logical PIF to connect the specific network.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">PIF on which to enable SR-IOV</param>
        /// <param name="_network">Network to connect SR-IOV virtual functions with VM VIFs</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Network_sriov> create(Session session, string _pif, string _network)
        {
            return session.JsonRpcClient.network_sriov_create(session.opaque_ref, _pif, _network);
        }

        /// <summary>
        /// Enable SR-IOV on the specific PIF. It will create a network-sriov based on the specific PIF and automatically create a logical PIF to connect the specific network.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">PIF on which to enable SR-IOV</param>
        /// <param name="_network">Network to connect SR-IOV virtual functions with VM VIFs</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, string _pif, string _network)
        {
          return session.JsonRpcClient.async_network_sriov_create(session.opaque_ref, _pif, _network);
        }

        /// <summary>
        /// Disable SR-IOV on the specific PIF. It will destroy the network-sriov and the logical PIF accordingly.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _network_sriov)
        {
            session.JsonRpcClient.network_sriov_destroy(session.opaque_ref, _network_sriov);
        }

        /// <summary>
        /// Disable SR-IOV on the specific PIF. It will destroy the network-sriov and the logical PIF accordingly.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _network_sriov)
        {
          return session.JsonRpcClient.async_network_sriov_destroy(session.opaque_ref, _network_sriov);
        }

        /// <summary>
        /// Get the number of free SR-IOV VFs on the associated PIF
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_remaining_capacity(Session session, string _network_sriov)
        {
            return session.JsonRpcClient.network_sriov_get_remaining_capacity(session.opaque_ref, _network_sriov);
        }

        /// <summary>
        /// Get the number of free SR-IOV VFs on the associated PIF
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network_sriov">The opaque_ref of the given network_sriov</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_get_remaining_capacity(Session session, string _network_sriov)
        {
          return session.JsonRpcClient.async_network_sriov_get_remaining_capacity(session.opaque_ref, _network_sriov);
        }

        /// <summary>
        /// Return a list of all the network_sriovs known to the system.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Network_sriov>> get_all(Session session)
        {
            return session.JsonRpcClient.network_sriov_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of network_sriov references to network_sriov records for all network_sriovs known to the system.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Network_sriov>, Network_sriov> get_all_records(Session session)
        {
            return session.JsonRpcClient.network_sriov_get_all_records(session.opaque_ref);
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
        /// The PIF that has SR-IOV enabled
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PIF>))]
        public virtual XenRef<PIF> physical_PIF
        {
            get { return _physical_PIF; }
            set
            {
                if (!Helper.AreEqual(value, _physical_PIF))
                {
                    _physical_PIF = value;
                    NotifyPropertyChanged("physical_PIF");
                }
            }
        }
        private XenRef<PIF> _physical_PIF = new XenRef<PIF>(Helper.NullOpaqueRef);

        /// <summary>
        /// The logical PIF to connect to the SR-IOV network after enable SR-IOV on the physical PIF
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PIF>))]
        public virtual XenRef<PIF> logical_PIF
        {
            get { return _logical_PIF; }
            set
            {
                if (!Helper.AreEqual(value, _logical_PIF))
                {
                    _logical_PIF = value;
                    NotifyPropertyChanged("logical_PIF");
                }
            }
        }
        private XenRef<PIF> _logical_PIF = new XenRef<PIF>(Helper.NullOpaqueRef);

        /// <summary>
        /// Indicates whether the host need to be rebooted before SR-IOV is enabled on the physical PIF
        /// </summary>
        public virtual bool requires_reboot
        {
            get { return _requires_reboot; }
            set
            {
                if (!Helper.AreEqual(value, _requires_reboot))
                {
                    _requires_reboot = value;
                    NotifyPropertyChanged("requires_reboot");
                }
            }
        }
        private bool _requires_reboot = false;

        /// <summary>
        /// The mode for configure network sriov
        /// </summary>
        [JsonConverter(typeof(sriov_configuration_modeConverter))]
        public virtual sriov_configuration_mode configuration_mode
        {
            get { return _configuration_mode; }
            set
            {
                if (!Helper.AreEqual(value, _configuration_mode))
                {
                    _configuration_mode = value;
                    NotifyPropertyChanged("configuration_mode");
                }
            }
        }
        private sriov_configuration_mode _configuration_mode = sriov_configuration_mode.unknown;
    }
}
