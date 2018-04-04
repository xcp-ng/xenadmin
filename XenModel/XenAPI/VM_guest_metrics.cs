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
    /// The metrics reported by the guest (as opposed to inferred from outside)
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class VM_guest_metrics : XenObject<VM_guest_metrics>
    {
        public VM_guest_metrics()
        {
        }

        public VM_guest_metrics(string uuid,
            Dictionary<string, string> os_version,
            Dictionary<string, string> PV_drivers_version,
            bool PV_drivers_up_to_date,
            Dictionary<string, string> memory,
            Dictionary<string, string> disks,
            Dictionary<string, string> networks,
            Dictionary<string, string> other,
            DateTime last_updated,
            Dictionary<string, string> other_config,
            bool live,
            tristate_type can_use_hotplug_vbd,
            tristate_type can_use_hotplug_vif,
            bool PV_drivers_detected)
        {
            this.uuid = uuid;
            this.os_version = os_version;
            this.PV_drivers_version = PV_drivers_version;
            this.PV_drivers_up_to_date = PV_drivers_up_to_date;
            this.memory = memory;
            this.disks = disks;
            this.networks = networks;
            this.other = other;
            this.last_updated = last_updated;
            this.other_config = other_config;
            this.live = live;
            this.can_use_hotplug_vbd = can_use_hotplug_vbd;
            this.can_use_hotplug_vif = can_use_hotplug_vif;
            this.PV_drivers_detected = PV_drivers_detected;
        }

        /// <summary>
        /// Creates a new VM_guest_metrics from a Proxy_VM_guest_metrics.
        /// </summary>
        /// <param name="proxy"></param>
        public VM_guest_metrics(Proxy_VM_guest_metrics proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VM_guest_metrics.
        /// </summary>
        public override void UpdateFrom(VM_guest_metrics update)
        {
            uuid = update.uuid;
            os_version = update.os_version;
            PV_drivers_version = update.PV_drivers_version;
            PV_drivers_up_to_date = update.PV_drivers_up_to_date;
            memory = update.memory;
            disks = update.disks;
            networks = update.networks;
            other = update.other;
            last_updated = update.last_updated;
            other_config = update.other_config;
            live = update.live;
            can_use_hotplug_vbd = update.can_use_hotplug_vbd;
            can_use_hotplug_vif = update.can_use_hotplug_vif;
            PV_drivers_detected = update.PV_drivers_detected;
        }

        internal void UpdateFromProxy(Proxy_VM_guest_metrics proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            os_version = proxy.os_version == null ? null : Maps.convert_from_proxy_string_string(proxy.os_version);
            PV_drivers_version = proxy.PV_drivers_version == null ? null : Maps.convert_from_proxy_string_string(proxy.PV_drivers_version);
            PV_drivers_up_to_date = (bool)proxy.PV_drivers_up_to_date;
            memory = proxy.memory == null ? null : Maps.convert_from_proxy_string_string(proxy.memory);
            disks = proxy.disks == null ? null : Maps.convert_from_proxy_string_string(proxy.disks);
            networks = proxy.networks == null ? null : Maps.convert_from_proxy_string_string(proxy.networks);
            other = proxy.other == null ? null : Maps.convert_from_proxy_string_string(proxy.other);
            last_updated = proxy.last_updated;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            live = (bool)proxy.live;
            can_use_hotplug_vbd = proxy.can_use_hotplug_vbd == null ? (tristate_type) 0 : (tristate_type)Helper.EnumParseDefault(typeof(tristate_type), (string)proxy.can_use_hotplug_vbd);
            can_use_hotplug_vif = proxy.can_use_hotplug_vif == null ? (tristate_type) 0 : (tristate_type)Helper.EnumParseDefault(typeof(tristate_type), (string)proxy.can_use_hotplug_vif);
            PV_drivers_detected = (bool)proxy.PV_drivers_detected;
        }

        public Proxy_VM_guest_metrics ToProxy()
        {
            Proxy_VM_guest_metrics result_ = new Proxy_VM_guest_metrics();
            result_.uuid = uuid ?? "";
            result_.os_version = Maps.convert_to_proxy_string_string(os_version);
            result_.PV_drivers_version = Maps.convert_to_proxy_string_string(PV_drivers_version);
            result_.PV_drivers_up_to_date = PV_drivers_up_to_date;
            result_.memory = Maps.convert_to_proxy_string_string(memory);
            result_.disks = Maps.convert_to_proxy_string_string(disks);
            result_.networks = Maps.convert_to_proxy_string_string(networks);
            result_.other = Maps.convert_to_proxy_string_string(other);
            result_.last_updated = last_updated;
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.live = live;
            result_.can_use_hotplug_vbd = tristate_type_helper.ToString(can_use_hotplug_vbd);
            result_.can_use_hotplug_vif = tristate_type_helper.ToString(can_use_hotplug_vif);
            result_.PV_drivers_detected = PV_drivers_detected;
            return result_;
        }

        /// <summary>
        /// Creates a new VM_guest_metrics from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VM_guest_metrics(Hashtable table) : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this VM_guest_metrics
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("os_version"))
                os_version = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "os_version"));
            if (table.ContainsKey("PV_drivers_version"))
                PV_drivers_version = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "PV_drivers_version"));
            if (table.ContainsKey("PV_drivers_up_to_date"))
                PV_drivers_up_to_date = Marshalling.ParseBool(table, "PV_drivers_up_to_date");
            if (table.ContainsKey("memory"))
                memory = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "memory"));
            if (table.ContainsKey("disks"))
                disks = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "disks"));
            if (table.ContainsKey("networks"))
                networks = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "networks"));
            if (table.ContainsKey("other"))
                other = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other"));
            if (table.ContainsKey("last_updated"))
                last_updated = Marshalling.ParseDateTime(table, "last_updated");
            if (table.ContainsKey("other_config"))
                other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("live"))
                live = Marshalling.ParseBool(table, "live");
            if (table.ContainsKey("can_use_hotplug_vbd"))
                can_use_hotplug_vbd = (tristate_type)Helper.EnumParseDefault(typeof(tristate_type), Marshalling.ParseString(table, "can_use_hotplug_vbd"));
            if (table.ContainsKey("can_use_hotplug_vif"))
                can_use_hotplug_vif = (tristate_type)Helper.EnumParseDefault(typeof(tristate_type), Marshalling.ParseString(table, "can_use_hotplug_vif"));
            if (table.ContainsKey("PV_drivers_detected"))
                PV_drivers_detected = Marshalling.ParseBool(table, "PV_drivers_detected");
        }

        public bool DeepEquals(VM_guest_metrics other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._os_version, other._os_version) &&
                Helper.AreEqual2(this._PV_drivers_version, other._PV_drivers_version) &&
                Helper.AreEqual2(this._PV_drivers_up_to_date, other._PV_drivers_up_to_date) &&
                Helper.AreEqual2(this._memory, other._memory) &&
                Helper.AreEqual2(this._disks, other._disks) &&
                Helper.AreEqual2(this._networks, other._networks) &&
                Helper.AreEqual2(this._other, other._other) &&
                Helper.AreEqual2(this._last_updated, other._last_updated) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._live, other._live) &&
                Helper.AreEqual2(this._can_use_hotplug_vbd, other._can_use_hotplug_vbd) &&
                Helper.AreEqual2(this._can_use_hotplug_vif, other._can_use_hotplug_vif) &&
                Helper.AreEqual2(this._PV_drivers_detected, other._PV_drivers_detected);
        }

        internal static List<VM_guest_metrics> ProxyArrayToObjectList(Proxy_VM_guest_metrics[] input)
        {
            var result = new List<VM_guest_metrics>();
            foreach (var item in input)
                result.Add(new VM_guest_metrics(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, VM_guest_metrics server)
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
                    VM_guest_metrics.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        public static VM_guest_metrics get_record(Session session, string _vm_guest_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_record(session.opaque_ref, _vm_guest_metrics);
            else
                return new VM_guest_metrics((Proxy_VM_guest_metrics)session.proxy.vm_guest_metrics_get_record(session.opaque_ref, _vm_guest_metrics ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the VM_guest_metrics instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<VM_guest_metrics> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<VM_guest_metrics>.Create(session.proxy.vm_guest_metrics_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        public static string get_uuid(Session session, string _vm_guest_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_uuid(session.opaque_ref, _vm_guest_metrics);
            else
                return session.proxy.vm_guest_metrics_get_uuid(session.opaque_ref, _vm_guest_metrics ?? "").parse();
        }

        /// <summary>
        /// Get the os_version field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        public static Dictionary<string, string> get_os_version(Session session, string _vm_guest_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_os_version(session.opaque_ref, _vm_guest_metrics);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vm_guest_metrics_get_os_version(session.opaque_ref, _vm_guest_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the PV_drivers_version field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        public static Dictionary<string, string> get_PV_drivers_version(Session session, string _vm_guest_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_pv_drivers_version(session.opaque_ref, _vm_guest_metrics);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vm_guest_metrics_get_pv_drivers_version(session.opaque_ref, _vm_guest_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the PV_drivers_up_to_date field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        [Deprecated("XenServer 7.0")]
        public static bool get_PV_drivers_up_to_date(Session session, string _vm_guest_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_pv_drivers_up_to_date(session.opaque_ref, _vm_guest_metrics);
            else
                return (bool)session.proxy.vm_guest_metrics_get_pv_drivers_up_to_date(session.opaque_ref, _vm_guest_metrics ?? "").parse();
        }

        /// <summary>
        /// Get the memory field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        public static Dictionary<string, string> get_memory(Session session, string _vm_guest_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_memory(session.opaque_ref, _vm_guest_metrics);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vm_guest_metrics_get_memory(session.opaque_ref, _vm_guest_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the disks field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        public static Dictionary<string, string> get_disks(Session session, string _vm_guest_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_disks(session.opaque_ref, _vm_guest_metrics);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vm_guest_metrics_get_disks(session.opaque_ref, _vm_guest_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the networks field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        public static Dictionary<string, string> get_networks(Session session, string _vm_guest_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_networks(session.opaque_ref, _vm_guest_metrics);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vm_guest_metrics_get_networks(session.opaque_ref, _vm_guest_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the other field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        public static Dictionary<string, string> get_other(Session session, string _vm_guest_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_other(session.opaque_ref, _vm_guest_metrics);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vm_guest_metrics_get_other(session.opaque_ref, _vm_guest_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the last_updated field of the given VM_guest_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        public static DateTime get_last_updated(Session session, string _vm_guest_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_last_updated(session.opaque_ref, _vm_guest_metrics);
            else
                return session.proxy.vm_guest_metrics_get_last_updated(session.opaque_ref, _vm_guest_metrics ?? "").parse();
        }

        /// <summary>
        /// Get the other_config field of the given VM_guest_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        public static Dictionary<string, string> get_other_config(Session session, string _vm_guest_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_other_config(session.opaque_ref, _vm_guest_metrics);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vm_guest_metrics_get_other_config(session.opaque_ref, _vm_guest_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the live field of the given VM_guest_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        public static bool get_live(Session session, string _vm_guest_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_live(session.opaque_ref, _vm_guest_metrics);
            else
                return (bool)session.proxy.vm_guest_metrics_get_live(session.opaque_ref, _vm_guest_metrics ?? "").parse();
        }

        /// <summary>
        /// Get the can_use_hotplug_vbd field of the given VM_guest_metrics.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        public static tristate_type get_can_use_hotplug_vbd(Session session, string _vm_guest_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_can_use_hotplug_vbd(session.opaque_ref, _vm_guest_metrics);
            else
                return (tristate_type)Helper.EnumParseDefault(typeof(tristate_type), (string)session.proxy.vm_guest_metrics_get_can_use_hotplug_vbd(session.opaque_ref, _vm_guest_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the can_use_hotplug_vif field of the given VM_guest_metrics.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        public static tristate_type get_can_use_hotplug_vif(Session session, string _vm_guest_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_can_use_hotplug_vif(session.opaque_ref, _vm_guest_metrics);
            else
                return (tristate_type)Helper.EnumParseDefault(typeof(tristate_type), (string)session.proxy.vm_guest_metrics_get_can_use_hotplug_vif(session.opaque_ref, _vm_guest_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the PV_drivers_detected field of the given VM_guest_metrics.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        public static bool get_PV_drivers_detected(Session session, string _vm_guest_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_pv_drivers_detected(session.opaque_ref, _vm_guest_metrics);
            else
                return (bool)session.proxy.vm_guest_metrics_get_pv_drivers_detected(session.opaque_ref, _vm_guest_metrics ?? "").parse();
        }

        /// <summary>
        /// Set the other_config field of the given VM_guest_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _vm_guest_metrics, Dictionary<string, string> _other_config)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vm_guest_metrics_set_other_config(session.opaque_ref, _vm_guest_metrics, _other_config);
            else
                session.proxy.vm_guest_metrics_set_other_config(session.opaque_ref, _vm_guest_metrics ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given VM_guest_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _vm_guest_metrics, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vm_guest_metrics_add_to_other_config(session.opaque_ref, _vm_guest_metrics, _key, _value);
            else
                session.proxy.vm_guest_metrics_add_to_other_config(session.opaque_ref, _vm_guest_metrics ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given VM_guest_metrics.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_guest_metrics">The opaque_ref of the given vm_guest_metrics</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _vm_guest_metrics, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vm_guest_metrics_remove_from_other_config(session.opaque_ref, _vm_guest_metrics, _key);
            else
                session.proxy.vm_guest_metrics_remove_from_other_config(session.opaque_ref, _vm_guest_metrics ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// Return a list of all the VM_guest_metrics instances known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<VM_guest_metrics>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_all(session.opaque_ref);
            else
                return XenRef<VM_guest_metrics>.Create(session.proxy.vm_guest_metrics_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the VM_guest_metrics Records at once, in a single XML RPC call
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<VM_guest_metrics>, VM_guest_metrics> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_guest_metrics_get_all_records(session.opaque_ref);
            else
                return XenRef<VM_guest_metrics>.Create<Proxy_VM_guest_metrics>(session.proxy.vm_guest_metrics_get_all_records(session.opaque_ref).parse());
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
        /// version of the OS
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> os_version
        {
            get { return _os_version; }
            set
            {
                if (!Helper.AreEqual(value, _os_version))
                {
                    _os_version = value;
                    Changed = true;
                    NotifyPropertyChanged("os_version");
                }
            }
        }
        private Dictionary<string, string> _os_version = new Dictionary<string, string>() {};

        /// <summary>
        /// version of the PV drivers
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> PV_drivers_version
        {
            get { return _PV_drivers_version; }
            set
            {
                if (!Helper.AreEqual(value, _PV_drivers_version))
                {
                    _PV_drivers_version = value;
                    Changed = true;
                    NotifyPropertyChanged("PV_drivers_version");
                }
            }
        }
        private Dictionary<string, string> _PV_drivers_version = new Dictionary<string, string>() {};

        /// <summary>
        /// Logically equivalent to PV_drivers_detected
        /// </summary>
        public virtual bool PV_drivers_up_to_date
        {
            get { return _PV_drivers_up_to_date; }
            set
            {
                if (!Helper.AreEqual(value, _PV_drivers_up_to_date))
                {
                    _PV_drivers_up_to_date = value;
                    Changed = true;
                    NotifyPropertyChanged("PV_drivers_up_to_date");
                }
            }
        }
        private bool _PV_drivers_up_to_date;

        /// <summary>
        /// This field exists but has no data. Use the memory and memory_internal_free RRD data-sources instead.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> memory
        {
            get { return _memory; }
            set
            {
                if (!Helper.AreEqual(value, _memory))
                {
                    _memory = value;
                    Changed = true;
                    NotifyPropertyChanged("memory");
                }
            }
        }
        private Dictionary<string, string> _memory = new Dictionary<string, string>() {};

        /// <summary>
        /// This field exists but has no data.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> disks
        {
            get { return _disks; }
            set
            {
                if (!Helper.AreEqual(value, _disks))
                {
                    _disks = value;
                    Changed = true;
                    NotifyPropertyChanged("disks");
                }
            }
        }
        private Dictionary<string, string> _disks = new Dictionary<string, string>() {};

        /// <summary>
        /// network configuration
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> networks
        {
            get { return _networks; }
            set
            {
                if (!Helper.AreEqual(value, _networks))
                {
                    _networks = value;
                    Changed = true;
                    NotifyPropertyChanged("networks");
                }
            }
        }
        private Dictionary<string, string> _networks = new Dictionary<string, string>() {};

        /// <summary>
        /// anything else
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> other
        {
            get { return _other; }
            set
            {
                if (!Helper.AreEqual(value, _other))
                {
                    _other = value;
                    Changed = true;
                    NotifyPropertyChanged("other");
                }
            }
        }
        private Dictionary<string, string> _other = new Dictionary<string, string>() {};

        /// <summary>
        /// Time at which this information was last updated
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime last_updated
        {
            get { return _last_updated; }
            set
            {
                if (!Helper.AreEqual(value, _last_updated))
                {
                    _last_updated = value;
                    Changed = true;
                    NotifyPropertyChanged("last_updated");
                }
            }
        }
        private DateTime _last_updated;

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
                    Changed = true;
                    NotifyPropertyChanged("other_config");
                }
            }
        }
        private Dictionary<string, string> _other_config = new Dictionary<string, string>() {};

        /// <summary>
        /// True if the guest is sending heartbeat messages via the guest agent
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual bool live
        {
            get { return _live; }
            set
            {
                if (!Helper.AreEqual(value, _live))
                {
                    _live = value;
                    Changed = true;
                    NotifyPropertyChanged("live");
                }
            }
        }
        private bool _live = false;

        /// <summary>
        /// The guest's statement of whether it supports VBD hotplug, i.e. whether it is capable of responding immediately to instantiation of a new VBD by bringing online a new PV block device. If the guest states that it is not capable, then the VBD plug and unplug operations will not be allowed while the guest is running.
        /// First published in XenServer 7.0.
        /// </summary>
        [JsonConverter(typeof(tristate_typeConverter))]
        public virtual tristate_type can_use_hotplug_vbd
        {
            get { return _can_use_hotplug_vbd; }
            set
            {
                if (!Helper.AreEqual(value, _can_use_hotplug_vbd))
                {
                    _can_use_hotplug_vbd = value;
                    Changed = true;
                    NotifyPropertyChanged("can_use_hotplug_vbd");
                }
            }
        }
        private tristate_type _can_use_hotplug_vbd = tristate_type.unspecified;

        /// <summary>
        /// The guest's statement of whether it supports VIF hotplug, i.e. whether it is capable of responding immediately to instantiation of a new VIF by bringing online a new PV network device. If the guest states that it is not capable, then the VIF plug and unplug operations will not be allowed while the guest is running.
        /// First published in XenServer 7.0.
        /// </summary>
        [JsonConverter(typeof(tristate_typeConverter))]
        public virtual tristate_type can_use_hotplug_vif
        {
            get { return _can_use_hotplug_vif; }
            set
            {
                if (!Helper.AreEqual(value, _can_use_hotplug_vif))
                {
                    _can_use_hotplug_vif = value;
                    Changed = true;
                    NotifyPropertyChanged("can_use_hotplug_vif");
                }
            }
        }
        private tristate_type _can_use_hotplug_vif = tristate_type.unspecified;

        /// <summary>
        /// At least one of the guest's devices has successfully connected to the backend.
        /// First published in XenServer 7.0.
        /// </summary>
        public virtual bool PV_drivers_detected
        {
            get { return _PV_drivers_detected; }
            set
            {
                if (!Helper.AreEqual(value, _PV_drivers_detected))
                {
                    _PV_drivers_detected = value;
                    Changed = true;
                    NotifyPropertyChanged("PV_drivers_detected");
                }
            }
        }
        private bool _PV_drivers_detected = false;
    }
}
