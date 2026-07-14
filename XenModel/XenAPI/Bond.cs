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
    /// A Network bond that combines physical network interfaces, also known as link aggregation
    /// First published in XenServer 4.1.
    /// </summary>
    public partial class Bond : XenObject<Bond>
    {
        #region Constructors

        public Bond()
        {
        }

        public Bond(string uuid,
            XenRef<PIF> master,
            List<XenRef<PIF>> slaves,
            Dictionary<string, string> other_config,
            XenRef<PIF> primary_slave,
            bond_mode mode,
            Dictionary<string, string> properties,
            long links_up,
            bool auto_update_mac)
        {
            this.uuid = uuid;
            this.master = master;
            this.slaves = slaves;
            this.other_config = other_config;
            this.primary_slave = primary_slave;
            this.mode = mode;
            this.properties = properties;
            this.links_up = links_up;
            this.auto_update_mac = auto_update_mac;
        }

        /// <summary>
        /// Creates a new Bond from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Bond(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Bond.
        /// </summary>
        public override void UpdateFrom(Bond record)
        {
            uuid = record.uuid;
            master = record.master;
            slaves = record.slaves;
            other_config = record.other_config;
            primary_slave = record.primary_slave;
            mode = record.mode;
            properties = record.properties;
            links_up = record.links_up;
            auto_update_mac = record.auto_update_mac;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Bond
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("master"))
                master = Marshalling.ParseRef<PIF>(table, "master");
            if (table.ContainsKey("slaves"))
                slaves = Marshalling.ParseSetRef<PIF>(table, "slaves");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("primary_slave"))
                primary_slave = Marshalling.ParseRef<PIF>(table, "primary_slave");
            if (table.ContainsKey("mode"))
                mode = (bond_mode)Helper.EnumParseDefault(typeof(bond_mode), Marshalling.ParseString(table, "mode"));
            if (table.ContainsKey("properties"))
                properties = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "properties"));
            if (table.ContainsKey("links_up"))
                links_up = Marshalling.ParseLong(table, "links_up");
            if (table.ContainsKey("auto_update_mac"))
                auto_update_mac = Marshalling.ParseBool(table, "auto_update_mac");
        }

        public bool DeepEquals(Bond other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_master, other._master) &&
                Helper.AreEqual2(_slaves, other._slaves) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_primary_slave, other._primary_slave) &&
                Helper.AreEqual2(_mode, other._mode) &&
                Helper.AreEqual2(_properties, other._properties) &&
                Helper.AreEqual2(_links_up, other._links_up) &&
                Helper.AreEqual2(_auto_update_mac, other._auto_update_mac);
        }


        /// <summary>
        /// Get a record containing the current state of the given Bond.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Bond get_record(Session session, string _bond)
        {
            return session.JsonRpcClient.bond_get_record(session.opaque_ref, _bond);
        }

        /// <summary>
        /// Get a reference to the Bond instance with the specified UUID.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Bond> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.bond_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given Bond.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _bond)
        {
            return session.JsonRpcClient.bond_get_uuid(session.opaque_ref, _bond);
        }

        /// <summary>
        /// Get the master field of the given Bond.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PIF> get_master(Session session, string _bond)
        {
            return session.JsonRpcClient.bond_get_master(session.opaque_ref, _bond);
        }

        /// <summary>
        /// Get the slaves field of the given Bond.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PIF>> get_slaves(Session session, string _bond)
        {
            return session.JsonRpcClient.bond_get_slaves(session.opaque_ref, _bond);
        }

        /// <summary>
        /// Get the other_config field of the given Bond.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _bond)
        {
            return session.JsonRpcClient.bond_get_other_config(session.opaque_ref, _bond);
        }

        /// <summary>
        /// Get the primary_slave field of the given Bond.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PIF> get_primary_slave(Session session, string _bond)
        {
            return session.JsonRpcClient.bond_get_primary_slave(session.opaque_ref, _bond);
        }

        /// <summary>
        /// Get the mode field of the given Bond.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bond_mode get_mode(Session session, string _bond)
        {
            return session.JsonRpcClient.bond_get_mode(session.opaque_ref, _bond);
        }

        /// <summary>
        /// Get the properties field of the given Bond.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_properties(Session session, string _bond)
        {
            return session.JsonRpcClient.bond_get_properties(session.opaque_ref, _bond);
        }

        /// <summary>
        /// Get the links_up field of the given Bond.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_links_up(Session session, string _bond)
        {
            return session.JsonRpcClient.bond_get_links_up(session.opaque_ref, _bond);
        }

        /// <summary>
        /// Get the auto_update_mac field of the given Bond.
        /// First published in Citrix Hypervisor 8.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_auto_update_mac(Session session, string _bond)
        {
            return session.JsonRpcClient.bond_get_auto_update_mac(session.opaque_ref, _bond);
        }

        /// <summary>
        /// Set the other_config field of the given Bond.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_other_config(Session session, string _bond, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.bond_set_other_config(session.opaque_ref, _bond, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given Bond.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_other_config(Session session, string _bond, string _key, string _value)
        {
            session.JsonRpcClient.bond_add_to_other_config(session.opaque_ref, _bond, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given Bond.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_other_config(Session session, string _bond, string _key)
        {
            session.JsonRpcClient.bond_remove_from_other_config(session.opaque_ref, _bond, _key);
        }

        /// <summary>
        /// Create an interface bond
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">Network to add the bonded PIF to</param>
        /// <param name="_members">PIFs to add to this bond</param>
        /// <param name="_mac">The MAC address to use on the bond itself. If this parameter is the empty string then the bond will inherit its MAC address from the primary slave.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Bond> create(Session session, string _network, List<XenRef<PIF>> _members, string _mac)
        {
            return session.JsonRpcClient.bond_create(session.opaque_ref, _network, _members, _mac);
        }

        /// <summary>
        /// Create an interface bond
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">Network to add the bonded PIF to</param>
        /// <param name="_members">PIFs to add to this bond</param>
        /// <param name="_mac">The MAC address to use on the bond itself. If this parameter is the empty string then the bond will inherit its MAC address from the primary slave.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, string _network, List<XenRef<PIF>> _members, string _mac)
        {
          return session.JsonRpcClient.async_bond_create(session.opaque_ref, _network, _members, _mac);
        }

        /// <summary>
        /// Create an interface bond
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">Network to add the bonded PIF to</param>
        /// <param name="_members">PIFs to add to this bond</param>
        /// <param name="_mac">The MAC address to use on the bond itself. If this parameter is the empty string then the bond will inherit its MAC address from the primary slave.</param>
        /// <param name="_mode">Bonding mode to use for the new bond First published in XenServer 6.0.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Bond> create(Session session, string _network, List<XenRef<PIF>> _members, string _mac, bond_mode _mode)
        {
            return session.JsonRpcClient.bond_create(session.opaque_ref, _network, _members, _mac, _mode);
        }

        /// <summary>
        /// Create an interface bond
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">Network to add the bonded PIF to</param>
        /// <param name="_members">PIFs to add to this bond</param>
        /// <param name="_mac">The MAC address to use on the bond itself. If this parameter is the empty string then the bond will inherit its MAC address from the primary slave.</param>
        /// <param name="_mode">Bonding mode to use for the new bond First published in XenServer 6.0.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, string _network, List<XenRef<PIF>> _members, string _mac, bond_mode _mode)
        {
          return session.JsonRpcClient.async_bond_create(session.opaque_ref, _network, _members, _mac, _mode);
        }

        /// <summary>
        /// Create an interface bond
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">Network to add the bonded PIF to</param>
        /// <param name="_members">PIFs to add to this bond</param>
        /// <param name="_mac">The MAC address to use on the bond itself. If this parameter is the empty string then the bond will inherit its MAC address from the primary slave.</param>
        /// <param name="_mode">Bonding mode to use for the new bond First published in XenServer 6.0.</param>
        /// <param name="_properties">Additional configuration parameters specific to the bond mode First published in XenServer 6.1.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Bond> create(Session session, string _network, List<XenRef<PIF>> _members, string _mac, bond_mode _mode, Dictionary<string, string> _properties)
        {
            return session.JsonRpcClient.bond_create(session.opaque_ref, _network, _members, _mac, _mode, _properties);
        }

        /// <summary>
        /// Create an interface bond
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">Network to add the bonded PIF to</param>
        /// <param name="_members">PIFs to add to this bond</param>
        /// <param name="_mac">The MAC address to use on the bond itself. If this parameter is the empty string then the bond will inherit its MAC address from the primary slave.</param>
        /// <param name="_mode">Bonding mode to use for the new bond First published in XenServer 6.0.</param>
        /// <param name="_properties">Additional configuration parameters specific to the bond mode First published in XenServer 6.1.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, string _network, List<XenRef<PIF>> _members, string _mac, bond_mode _mode, Dictionary<string, string> _properties)
        {
          return session.JsonRpcClient.async_bond_create(session.opaque_ref, _network, _members, _mac, _mode, _properties);
        }

        /// <summary>
        /// Destroy an interface bond
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _bond)
        {
            session.JsonRpcClient.bond_destroy(session.opaque_ref, _bond);
        }

        /// <summary>
        /// Destroy an interface bond
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _bond)
        {
          return session.JsonRpcClient.async_bond_destroy(session.opaque_ref, _bond);
        }

        /// <summary>
        /// Change the bond mode
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <param name="_value">The new bond mode</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_mode(Session session, string _bond, bond_mode _value)
        {
            session.JsonRpcClient.bond_set_mode(session.opaque_ref, _bond, _value);
        }

        /// <summary>
        /// Change the bond mode
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <param name="_value">The new bond mode</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_mode(Session session, string _bond, bond_mode _value)
        {
          return session.JsonRpcClient.async_bond_set_mode(session.opaque_ref, _bond, _value);
        }

        /// <summary>
        /// Set the value of a property of the bond
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <param name="_name">The property name</param>
        /// <param name="_value">The property value</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_property(Session session, string _bond, string _name, string _value)
        {
            session.JsonRpcClient.bond_set_property(session.opaque_ref, _bond, _name, _value);
        }

        /// <summary>
        /// Set the value of a property of the bond
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_bond">The opaque_ref of the given bond</param>
        /// <param name="_name">The property name</param>
        /// <param name="_value">The property value</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_property(Session session, string _bond, string _name, string _value)
        {
          return session.JsonRpcClient.async_bond_set_property(session.opaque_ref, _bond, _name, _value);
        }

        /// <summary>
        /// Return a list of all the Bonds known to the system.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Bond>> get_all(Session session)
        {
            return session.JsonRpcClient.bond_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of Bond references to Bond records for all Bonds known to the system.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Bond>, Bond> get_all_records(Session session)
        {
            return session.JsonRpcClient.bond_get_all_records(session.opaque_ref);
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
        /// The bonded interface
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PIF>))]
        public virtual XenRef<PIF> master
        {
            get { return _master; }
            set
            {
                if (!Helper.AreEqual(value, _master))
                {
                    _master = value;
                    NotifyPropertyChanged("master");
                }
            }
        }
        private XenRef<PIF> _master = new XenRef<PIF>(Helper.NullOpaqueRef);

        /// <summary>
        /// The interfaces which are part of this bond
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<PIF>))]
        public virtual List<XenRef<PIF>> slaves
        {
            get { return _slaves; }
            set
            {
                if (!Helper.AreEqual(value, _slaves))
                {
                    _slaves = value;
                    NotifyPropertyChanged("slaves");
                }
            }
        }
        private List<XenRef<PIF>> _slaves = new List<XenRef<PIF>>() {};

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
        /// The PIF of which the IP configuration and MAC were copied to the bond, and which will receive all configuration/VLANs/VIFs on the bond if the bond is destroyed
        /// First published in XenServer 6.0.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PIF>))]
        public virtual XenRef<PIF> primary_slave
        {
            get { return _primary_slave; }
            set
            {
                if (!Helper.AreEqual(value, _primary_slave))
                {
                    _primary_slave = value;
                    NotifyPropertyChanged("primary_slave");
                }
            }
        }
        private XenRef<PIF> _primary_slave = new XenRef<PIF>("OpaqueRef:NULL");

        /// <summary>
        /// The algorithm used to distribute traffic among the bonded NICs
        /// First published in XenServer 6.0.
        /// </summary>
        [JsonConverter(typeof(bond_modeConverter))]
        public virtual bond_mode mode
        {
            get { return _mode; }
            set
            {
                if (!Helper.AreEqual(value, _mode))
                {
                    _mode = value;
                    NotifyPropertyChanged("mode");
                }
            }
        }
        private bond_mode _mode = bond_mode.balance_slb;

        /// <summary>
        /// Additional configuration properties specific to the bond mode.
        /// First published in XenServer 6.1.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> properties
        {
            get { return _properties; }
            set
            {
                if (!Helper.AreEqual(value, _properties))
                {
                    _properties = value;
                    NotifyPropertyChanged("properties");
                }
            }
        }
        private Dictionary<string, string> _properties = new Dictionary<string, string>() {};

        /// <summary>
        /// Number of links up in this bond
        /// First published in XenServer 6.1.
        /// </summary>
        public virtual long links_up
        {
            get { return _links_up; }
            set
            {
                if (!Helper.AreEqual(value, _links_up))
                {
                    _links_up = value;
                    NotifyPropertyChanged("links_up");
                }
            }
        }
        private long _links_up = 0;

        /// <summary>
        /// true if the MAC was taken from the primary slave when the bond was created, and false if the client specified the MAC
        /// First published in Citrix Hypervisor 8.1.
        /// </summary>
        public virtual bool auto_update_mac
        {
            get { return _auto_update_mac; }
            set
            {
                if (!Helper.AreEqual(value, _auto_update_mac))
                {
                    _auto_update_mac = value;
                    NotifyPropertyChanged("auto_update_mac");
                }
            }
        }
        private bool _auto_update_mac = true;
    }
}
