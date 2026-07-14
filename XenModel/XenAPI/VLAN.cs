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
    /// A VLAN mux/demux
    /// First published in XenServer 4.1.
    /// </summary>
    public partial class VLAN : XenObject<VLAN>
    {
        #region Constructors

        public VLAN()
        {
        }

        public VLAN(string uuid,
            XenRef<PIF> tagged_PIF,
            XenRef<PIF> untagged_PIF,
            long tag,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.tagged_PIF = tagged_PIF;
            this.untagged_PIF = untagged_PIF;
            this.tag = tag;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new VLAN from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VLAN(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VLAN.
        /// </summary>
        public override void UpdateFrom(VLAN record)
        {
            uuid = record.uuid;
            tagged_PIF = record.tagged_PIF;
            untagged_PIF = record.untagged_PIF;
            tag = record.tag;
            other_config = record.other_config;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this VLAN
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("tagged_PIF"))
                tagged_PIF = Marshalling.ParseRef<PIF>(table, "tagged_PIF");
            if (table.ContainsKey("untagged_PIF"))
                untagged_PIF = Marshalling.ParseRef<PIF>(table, "untagged_PIF");
            if (table.ContainsKey("tag"))
                tag = Marshalling.ParseLong(table, "tag");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(VLAN other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_tagged_PIF, other._tagged_PIF) &&
                Helper.AreEqual2(_untagged_PIF, other._untagged_PIF) &&
                Helper.AreEqual2(_tag, other._tag) &&
                Helper.AreEqual2(_other_config, other._other_config);
        }


        /// <summary>
        /// Get a record containing the current state of the given VLAN.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static VLAN get_record(Session session, string _vlan)
        {
            return session.JsonRpcClient.vlan_get_record(session.opaque_ref, _vlan);
        }

        /// <summary>
        /// Get a reference to the VLAN instance with the specified UUID.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VLAN> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.vlan_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given VLAN.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _vlan)
        {
            return session.JsonRpcClient.vlan_get_uuid(session.opaque_ref, _vlan);
        }

        /// <summary>
        /// Get the tagged_PIF field of the given VLAN.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PIF> get_tagged_PIF(Session session, string _vlan)
        {
            return session.JsonRpcClient.vlan_get_tagged_pif(session.opaque_ref, _vlan);
        }

        /// <summary>
        /// Get the untagged_PIF field of the given VLAN.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PIF> get_untagged_PIF(Session session, string _vlan)
        {
            return session.JsonRpcClient.vlan_get_untagged_pif(session.opaque_ref, _vlan);
        }

        /// <summary>
        /// Get the tag field of the given VLAN.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_tag(Session session, string _vlan)
        {
            return session.JsonRpcClient.vlan_get_tag(session.opaque_ref, _vlan);
        }

        /// <summary>
        /// Get the other_config field of the given VLAN.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _vlan)
        {
            return session.JsonRpcClient.vlan_get_other_config(session.opaque_ref, _vlan);
        }

        /// <summary>
        /// Set the other_config field of the given VLAN.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_other_config(Session session, string _vlan, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.vlan_set_other_config(session.opaque_ref, _vlan, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given VLAN.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_other_config(Session session, string _vlan, string _key, string _value)
        {
            session.JsonRpcClient.vlan_add_to_other_config(session.opaque_ref, _vlan, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given VLAN.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_other_config(Session session, string _vlan, string _key)
        {
            session.JsonRpcClient.vlan_remove_from_other_config(session.opaque_ref, _vlan, _key);
        }

        /// <summary>
        /// Create a VLAN mux/demuxer
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tagged_pif">PIF which receives the tagged traffic</param>
        /// <param name="_tag">VLAN tag to use</param>
        /// <param name="_network">Network to receive the untagged traffic</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<VLAN> create(Session session, string _tagged_pif, long _tag, string _network)
        {
            return session.JsonRpcClient.vlan_create(session.opaque_ref, _tagged_pif, _tag, _network);
        }

        /// <summary>
        /// Create a VLAN mux/demuxer
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tagged_pif">PIF which receives the tagged traffic</param>
        /// <param name="_tag">VLAN tag to use</param>
        /// <param name="_network">Network to receive the untagged traffic</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, string _tagged_pif, long _tag, string _network)
        {
          return session.JsonRpcClient.async_vlan_create(session.opaque_ref, _tagged_pif, _tag, _network);
        }

        /// <summary>
        /// Destroy a VLAN mux/demuxer
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _vlan)
        {
            session.JsonRpcClient.vlan_destroy(session.opaque_ref, _vlan);
        }

        /// <summary>
        /// Destroy a VLAN mux/demuxer
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _vlan)
        {
          return session.JsonRpcClient.async_vlan_destroy(session.opaque_ref, _vlan);
        }

        /// <summary>
        /// Return a list of all the VLANs known to the system.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VLAN>> get_all(Session session)
        {
            return session.JsonRpcClient.vlan_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of VLAN references to VLAN records for all VLANs known to the system.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<VLAN>, VLAN> get_all_records(Session session)
        {
            return session.JsonRpcClient.vlan_get_all_records(session.opaque_ref);
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
        /// interface on which traffic is tagged
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PIF>))]
        public virtual XenRef<PIF> tagged_PIF
        {
            get { return _tagged_PIF; }
            set
            {
                if (!Helper.AreEqual(value, _tagged_PIF))
                {
                    _tagged_PIF = value;
                    NotifyPropertyChanged("tagged_PIF");
                }
            }
        }
        private XenRef<PIF> _tagged_PIF = new XenRef<PIF>(Helper.NullOpaqueRef);

        /// <summary>
        /// interface on which traffic is untagged
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<PIF>))]
        public virtual XenRef<PIF> untagged_PIF
        {
            get { return _untagged_PIF; }
            set
            {
                if (!Helper.AreEqual(value, _untagged_PIF))
                {
                    _untagged_PIF = value;
                    NotifyPropertyChanged("untagged_PIF");
                }
            }
        }
        private XenRef<PIF> _untagged_PIF = new XenRef<PIF>(Helper.NullOpaqueRef);

        /// <summary>
        /// VLAN tag in use
        /// </summary>
        public virtual long tag
        {
            get { return _tag; }
            set
            {
                if (!Helper.AreEqual(value, _tag))
                {
                    _tag = value;
                    NotifyPropertyChanged("tag");
                }
            }
        }
        private long _tag = -1;

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
