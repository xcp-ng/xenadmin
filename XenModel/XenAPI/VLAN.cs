/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace XenAPI
{
    /// <summary>
    /// A VLAN mux/demux
    /// First published in XenServer 4.1.
    /// </summary>
    public partial class VLAN : XenObject<VLAN>
    {
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
        /// Creates a new VLAN from a Proxy_VLAN.
        /// </summary>
        /// <param name="proxy"></param>
        public VLAN(Proxy_VLAN proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VLAN.
        /// </summary>
        public override void UpdateFrom(VLAN update)
        {
            uuid = update.uuid;
            tagged_PIF = update.tagged_PIF;
            untagged_PIF = update.untagged_PIF;
            tag = update.tag;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_VLAN proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            tagged_PIF = proxy.tagged_PIF == null ? null : XenRef<PIF>.Create(proxy.tagged_PIF);
            untagged_PIF = proxy.untagged_PIF == null ? null : XenRef<PIF>.Create(proxy.untagged_PIF);
            tag = proxy.tag == null ? 0 : long.Parse(proxy.tag);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_VLAN ToProxy()
        {
            Proxy_VLAN result_ = new Proxy_VLAN();
            result_.uuid = uuid ?? "";
            result_.tagged_PIF = tagged_PIF ?? "";
            result_.untagged_PIF = untagged_PIF ?? "";
            result_.tag = tag.ToString();
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new VLAN from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VLAN(Hashtable table) : this()
        {
            UpdateFrom(table);
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
                other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(VLAN other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._tagged_PIF, other._tagged_PIF) &&
                Helper.AreEqual2(this._untagged_PIF, other._untagged_PIF) &&
                Helper.AreEqual2(this._tag, other._tag) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        internal static List<VLAN> ProxyArrayToObjectList(Proxy_VLAN[] input)
        {
            var result = new List<VLAN>();
            foreach (var item in input)
                result.Add(new VLAN(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, VLAN server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    VLAN.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given VLAN.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        public static VLAN get_record(Session session, string _vlan)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vlan_get_record(session.opaque_ref, _vlan);
            else
                return new VLAN((Proxy_VLAN)session.proxy.vlan_get_record(session.opaque_ref, _vlan ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the VLAN instance with the specified UUID.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<VLAN> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vlan_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<VLAN>.Create(session.proxy.vlan_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given VLAN.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        public static string get_uuid(Session session, string _vlan)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vlan_get_uuid(session.opaque_ref, _vlan);
            else
                return session.proxy.vlan_get_uuid(session.opaque_ref, _vlan ?? "").parse();
        }

        /// <summary>
        /// Get the tagged_PIF field of the given VLAN.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        public static XenRef<PIF> get_tagged_PIF(Session session, string _vlan)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vlan_get_tagged_pif(session.opaque_ref, _vlan);
            else
                return XenRef<PIF>.Create(session.proxy.vlan_get_tagged_pif(session.opaque_ref, _vlan ?? "").parse());
        }

        /// <summary>
        /// Get the untagged_PIF field of the given VLAN.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        public static XenRef<PIF> get_untagged_PIF(Session session, string _vlan)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vlan_get_untagged_pif(session.opaque_ref, _vlan);
            else
                return XenRef<PIF>.Create(session.proxy.vlan_get_untagged_pif(session.opaque_ref, _vlan ?? "").parse());
        }

        /// <summary>
        /// Get the tag field of the given VLAN.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        public static long get_tag(Session session, string _vlan)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vlan_get_tag(session.opaque_ref, _vlan);
            else
                return long.Parse(session.proxy.vlan_get_tag(session.opaque_ref, _vlan ?? "").parse());
        }

        /// <summary>
        /// Get the other_config field of the given VLAN.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        public static Dictionary<string, string> get_other_config(Session session, string _vlan)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vlan_get_other_config(session.opaque_ref, _vlan);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vlan_get_other_config(session.opaque_ref, _vlan ?? "").parse());
        }

        /// <summary>
        /// Set the other_config field of the given VLAN.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _vlan, Dictionary<string, string> _other_config)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vlan_set_other_config(session.opaque_ref, _vlan, _other_config);
            else
                session.proxy.vlan_set_other_config(session.opaque_ref, _vlan ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given VLAN.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _vlan, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vlan_add_to_other_config(session.opaque_ref, _vlan, _key, _value);
            else
                session.proxy.vlan_add_to_other_config(session.opaque_ref, _vlan ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given VLAN.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _vlan, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vlan_remove_from_other_config(session.opaque_ref, _vlan, _key);
            else
                session.proxy.vlan_remove_from_other_config(session.opaque_ref, _vlan ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// Create a VLAN mux/demuxer
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tagged_pif">PIF which receives the tagged traffic</param>
        /// <param name="_tag">VLAN tag to use</param>
        /// <param name="_network">Network to receive the untagged traffic</param>
        public static XenRef<VLAN> create(Session session, string _tagged_pif, long _tag, string _network)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vlan_create(session.opaque_ref, _tagged_pif, _tag, _network);
            else
                return XenRef<VLAN>.Create(session.proxy.vlan_create(session.opaque_ref, _tagged_pif ?? "", _tag.ToString(), _network ?? "").parse());
        }

        /// <summary>
        /// Create a VLAN mux/demuxer
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_tagged_pif">PIF which receives the tagged traffic</param>
        /// <param name="_tag">VLAN tag to use</param>
        /// <param name="_network">Network to receive the untagged traffic</param>
        public static XenRef<Task> async_create(Session session, string _tagged_pif, long _tag, string _network)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vlan_create(session.opaque_ref, _tagged_pif, _tag, _network);
          else
              return XenRef<Task>.Create(session.proxy.async_vlan_create(session.opaque_ref, _tagged_pif ?? "", _tag.ToString(), _network ?? "").parse());
        }

        /// <summary>
        /// Destroy a VLAN mux/demuxer
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        public static void destroy(Session session, string _vlan)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vlan_destroy(session.opaque_ref, _vlan);
            else
                session.proxy.vlan_destroy(session.opaque_ref, _vlan ?? "").parse();
        }

        /// <summary>
        /// Destroy a VLAN mux/demuxer
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vlan">The opaque_ref of the given vlan</param>
        public static XenRef<Task> async_destroy(Session session, string _vlan)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vlan_destroy(session.opaque_ref, _vlan);
          else
              return XenRef<Task>.Create(session.proxy.async_vlan_destroy(session.opaque_ref, _vlan ?? "").parse());
        }

        /// <summary>
        /// Return a list of all the VLANs known to the system.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<VLAN>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vlan_get_all(session.opaque_ref);
            else
                return XenRef<VLAN>.Create(session.proxy.vlan_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the VLAN Records at once, in a single XML RPC call
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<VLAN>, VLAN> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vlan_get_all_records(session.opaque_ref);
            else
                return XenRef<VLAN>.Create<Proxy_VLAN>(session.proxy.vlan_get_all_records(session.opaque_ref).parse());
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
                    NotifyPropertyChanged("other_config");
                }
            }
        }
        private Dictionary<string, string> _other_config = new Dictionary<string, string>() {};
    }
}
