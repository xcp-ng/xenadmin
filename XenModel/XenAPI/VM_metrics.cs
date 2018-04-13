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


namespace XenAPI
{
    /// <summary>
    /// The metrics associated with a VM
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class VM_metrics : XenObject<VM_metrics>
    {
        public VM_metrics()
        {
        }

        public VM_metrics(string uuid,
            long memory_actual,
            long VCPUs_number,
            Dictionary<long, double> VCPUs_utilisation,
            Dictionary<long, long> VCPUs_CPU,
            Dictionary<string, string> VCPUs_params,
            Dictionary<long, string[]> VCPUs_flags,
            string[] state,
            DateTime start_time,
            DateTime install_time,
            DateTime last_updated,
            Dictionary<string, string> other_config,
            bool hvm,
            bool nested_virt,
            bool nomigrate,
            domain_type current_domain_type)
        {
            this.uuid = uuid;
            this.memory_actual = memory_actual;
            this.VCPUs_number = VCPUs_number;
            this.VCPUs_utilisation = VCPUs_utilisation;
            this.VCPUs_CPU = VCPUs_CPU;
            this.VCPUs_params = VCPUs_params;
            this.VCPUs_flags = VCPUs_flags;
            this.state = state;
            this.start_time = start_time;
            this.install_time = install_time;
            this.last_updated = last_updated;
            this.other_config = other_config;
            this.hvm = hvm;
            this.nested_virt = nested_virt;
            this.nomigrate = nomigrate;
            this.current_domain_type = current_domain_type;
        }

        /// <summary>
        /// Creates a new VM_metrics from a Proxy_VM_metrics.
        /// </summary>
        /// <param name="proxy"></param>
        public VM_metrics(Proxy_VM_metrics proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VM_metrics.
        /// </summary>
        public override void UpdateFrom(VM_metrics update)
        {
            uuid = update.uuid;
            memory_actual = update.memory_actual;
            VCPUs_number = update.VCPUs_number;
            VCPUs_utilisation = update.VCPUs_utilisation;
            VCPUs_CPU = update.VCPUs_CPU;
            VCPUs_params = update.VCPUs_params;
            VCPUs_flags = update.VCPUs_flags;
            state = update.state;
            start_time = update.start_time;
            install_time = update.install_time;
            last_updated = update.last_updated;
            other_config = update.other_config;
            hvm = update.hvm;
            nested_virt = update.nested_virt;
            nomigrate = update.nomigrate;
            current_domain_type = update.current_domain_type;
        }

        internal void UpdateFromProxy(Proxy_VM_metrics proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            memory_actual = proxy.memory_actual == null ? 0 : long.Parse(proxy.memory_actual);
            VCPUs_number = proxy.VCPUs_number == null ? 0 : long.Parse(proxy.VCPUs_number);
            VCPUs_utilisation = proxy.VCPUs_utilisation == null ? null : Maps.convert_from_proxy_long_double(proxy.VCPUs_utilisation);
            VCPUs_CPU = proxy.VCPUs_CPU == null ? null : Maps.convert_from_proxy_long_long(proxy.VCPUs_CPU);
            VCPUs_params = proxy.VCPUs_params == null ? null : Maps.convert_from_proxy_string_string(proxy.VCPUs_params);
            VCPUs_flags = proxy.VCPUs_flags == null ? null : Maps.convert_from_proxy_long_string_array(proxy.VCPUs_flags);
            state = proxy.state == null ? new string[] {} : (string [])proxy.state;
            start_time = proxy.start_time;
            install_time = proxy.install_time;
            last_updated = proxy.last_updated;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            hvm = (bool)proxy.hvm;
            nested_virt = (bool)proxy.nested_virt;
            nomigrate = (bool)proxy.nomigrate;
            current_domain_type = proxy.current_domain_type == null ? (domain_type) 0 : (domain_type)Helper.EnumParseDefault(typeof(domain_type), (string)proxy.current_domain_type);
        }

        public Proxy_VM_metrics ToProxy()
        {
            Proxy_VM_metrics result_ = new Proxy_VM_metrics();
            result_.uuid = uuid ?? "";
            result_.memory_actual = memory_actual.ToString();
            result_.VCPUs_number = VCPUs_number.ToString();
            result_.VCPUs_utilisation = Maps.convert_to_proxy_long_double(VCPUs_utilisation);
            result_.VCPUs_CPU = Maps.convert_to_proxy_long_long(VCPUs_CPU);
            result_.VCPUs_params = Maps.convert_to_proxy_string_string(VCPUs_params);
            result_.VCPUs_flags = Maps.convert_to_proxy_long_string_array(VCPUs_flags);
            result_.state = state;
            result_.start_time = start_time;
            result_.install_time = install_time;
            result_.last_updated = last_updated;
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.hvm = hvm;
            result_.nested_virt = nested_virt;
            result_.nomigrate = nomigrate;
            result_.current_domain_type = domain_type_helper.ToString(current_domain_type);
            return result_;
        }

        /// <summary>
        /// Creates a new VM_metrics from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VM_metrics(Hashtable table) : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this VM_metrics
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("memory_actual"))
                memory_actual = Marshalling.ParseLong(table, "memory_actual");
            if (table.ContainsKey("VCPUs_number"))
                VCPUs_number = Marshalling.ParseLong(table, "VCPUs_number");
            if (table.ContainsKey("VCPUs_utilisation"))
                VCPUs_utilisation = Maps.convert_from_proxy_long_double(Marshalling.ParseHashTable(table, "VCPUs_utilisation"));
            if (table.ContainsKey("VCPUs_CPU"))
                VCPUs_CPU = Maps.convert_from_proxy_long_long(Marshalling.ParseHashTable(table, "VCPUs_CPU"));
            if (table.ContainsKey("VCPUs_params"))
                VCPUs_params = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "VCPUs_params"));
            if (table.ContainsKey("VCPUs_flags"))
                VCPUs_flags = Maps.convert_from_proxy_long_string_array(Marshalling.ParseHashTable(table, "VCPUs_flags"));
            if (table.ContainsKey("state"))
                state = Marshalling.ParseStringArray(table, "state");
            if (table.ContainsKey("start_time"))
                start_time = Marshalling.ParseDateTime(table, "start_time");
            if (table.ContainsKey("install_time"))
                install_time = Marshalling.ParseDateTime(table, "install_time");
            if (table.ContainsKey("last_updated"))
                last_updated = Marshalling.ParseDateTime(table, "last_updated");
            if (table.ContainsKey("other_config"))
                other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("hvm"))
                hvm = Marshalling.ParseBool(table, "hvm");
            if (table.ContainsKey("nested_virt"))
                nested_virt = Marshalling.ParseBool(table, "nested_virt");
            if (table.ContainsKey("nomigrate"))
                nomigrate = Marshalling.ParseBool(table, "nomigrate");
            if (table.ContainsKey("current_domain_type"))
                current_domain_type = (domain_type)Helper.EnumParseDefault(typeof(domain_type), Marshalling.ParseString(table, "current_domain_type"));
        }

        public bool DeepEquals(VM_metrics other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._memory_actual, other._memory_actual) &&
                Helper.AreEqual2(this._VCPUs_number, other._VCPUs_number) &&
                Helper.AreEqual2(this._VCPUs_utilisation, other._VCPUs_utilisation) &&
                Helper.AreEqual2(this._VCPUs_CPU, other._VCPUs_CPU) &&
                Helper.AreEqual2(this._VCPUs_params, other._VCPUs_params) &&
                Helper.AreEqual2(this._VCPUs_flags, other._VCPUs_flags) &&
                Helper.AreEqual2(this._state, other._state) &&
                Helper.AreEqual2(this._start_time, other._start_time) &&
                Helper.AreEqual2(this._install_time, other._install_time) &&
                Helper.AreEqual2(this._last_updated, other._last_updated) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._hvm, other._hvm) &&
                Helper.AreEqual2(this._nested_virt, other._nested_virt) &&
                Helper.AreEqual2(this._nomigrate, other._nomigrate) &&
                Helper.AreEqual2(this._current_domain_type, other._current_domain_type);
        }

        internal static List<VM_metrics> ProxyArrayToObjectList(Proxy_VM_metrics[] input)
        {
            var result = new List<VM_metrics>();
            foreach (var item in input)
                result.Add(new VM_metrics(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, VM_metrics server)
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
                    VM_metrics.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static VM_metrics get_record(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_record(session.opaque_ref, _vm_metrics);
            else
                return new VM_metrics(session.proxy.vm_metrics_get_record(session.opaque_ref, _vm_metrics ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the VM_metrics instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<VM_metrics> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<VM_metrics>.Create(session.proxy.vm_metrics_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static string get_uuid(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_uuid(session.opaque_ref, _vm_metrics);
            else
                return session.proxy.vm_metrics_get_uuid(session.opaque_ref, _vm_metrics ?? "").parse();
        }

        /// <summary>
        /// Get the memory/actual field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static long get_memory_actual(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_memory_actual(session.opaque_ref, _vm_metrics);
            else
                return long.Parse(session.proxy.vm_metrics_get_memory_actual(session.opaque_ref, _vm_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the VCPUs/number field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static long get_VCPUs_number(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_vcpus_number(session.opaque_ref, _vm_metrics);
            else
                return long.Parse(session.proxy.vm_metrics_get_vcpus_number(session.opaque_ref, _vm_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the VCPUs/utilisation field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static Dictionary<long, double> get_VCPUs_utilisation(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_vcpus_utilisation(session.opaque_ref, _vm_metrics);
            else
                return Maps.convert_from_proxy_long_double(session.proxy.vm_metrics_get_vcpus_utilisation(session.opaque_ref, _vm_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the VCPUs/CPU field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static Dictionary<long, long> get_VCPUs_CPU(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_vcpus_cpu(session.opaque_ref, _vm_metrics);
            else
                return Maps.convert_from_proxy_long_long(session.proxy.vm_metrics_get_vcpus_cpu(session.opaque_ref, _vm_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the VCPUs/params field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static Dictionary<string, string> get_VCPUs_params(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_vcpus_params(session.opaque_ref, _vm_metrics);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vm_metrics_get_vcpus_params(session.opaque_ref, _vm_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the VCPUs/flags field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static Dictionary<long, string[]> get_VCPUs_flags(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_vcpus_flags(session.opaque_ref, _vm_metrics);
            else
                return Maps.convert_from_proxy_long_string_array(session.proxy.vm_metrics_get_vcpus_flags(session.opaque_ref, _vm_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the state field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static string[] get_state(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_state(session.opaque_ref, _vm_metrics);
            else
                return (string [])session.proxy.vm_metrics_get_state(session.opaque_ref, _vm_metrics ?? "").parse();
        }

        /// <summary>
        /// Get the start_time field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static DateTime get_start_time(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_start_time(session.opaque_ref, _vm_metrics);
            else
                return session.proxy.vm_metrics_get_start_time(session.opaque_ref, _vm_metrics ?? "").parse();
        }

        /// <summary>
        /// Get the install_time field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static DateTime get_install_time(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_install_time(session.opaque_ref, _vm_metrics);
            else
                return session.proxy.vm_metrics_get_install_time(session.opaque_ref, _vm_metrics ?? "").parse();
        }

        /// <summary>
        /// Get the last_updated field of the given VM_metrics.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static DateTime get_last_updated(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_last_updated(session.opaque_ref, _vm_metrics);
            else
                return session.proxy.vm_metrics_get_last_updated(session.opaque_ref, _vm_metrics ?? "").parse();
        }

        /// <summary>
        /// Get the other_config field of the given VM_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static Dictionary<string, string> get_other_config(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_other_config(session.opaque_ref, _vm_metrics);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vm_metrics_get_other_config(session.opaque_ref, _vm_metrics ?? "").parse());
        }

        /// <summary>
        /// Get the hvm field of the given VM_metrics.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static bool get_hvm(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_hvm(session.opaque_ref, _vm_metrics);
            else
                return (bool)session.proxy.vm_metrics_get_hvm(session.opaque_ref, _vm_metrics ?? "").parse();
        }

        /// <summary>
        /// Get the nested_virt field of the given VM_metrics.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static bool get_nested_virt(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_nested_virt(session.opaque_ref, _vm_metrics);
            else
                return (bool)session.proxy.vm_metrics_get_nested_virt(session.opaque_ref, _vm_metrics ?? "").parse();
        }

        /// <summary>
        /// Get the nomigrate field of the given VM_metrics.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static bool get_nomigrate(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_nomigrate(session.opaque_ref, _vm_metrics);
            else
                return (bool)session.proxy.vm_metrics_get_nomigrate(session.opaque_ref, _vm_metrics ?? "").parse();
        }

        /// <summary>
        /// Get the current_domain_type field of the given VM_metrics.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        public static domain_type get_current_domain_type(Session session, string _vm_metrics)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_current_domain_type(session.opaque_ref, _vm_metrics);
            else
                return (domain_type)Helper.EnumParseDefault(typeof(domain_type), (string)session.proxy.vm_metrics_get_current_domain_type(session.opaque_ref, _vm_metrics ?? "").parse());
        }

        /// <summary>
        /// Set the other_config field of the given VM_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _vm_metrics, Dictionary<string, string> _other_config)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vm_metrics_set_other_config(session.opaque_ref, _vm_metrics, _other_config);
            else
                session.proxy.vm_metrics_set_other_config(session.opaque_ref, _vm_metrics ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given VM_metrics.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _vm_metrics, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vm_metrics_add_to_other_config(session.opaque_ref, _vm_metrics, _key, _value);
            else
                session.proxy.vm_metrics_add_to_other_config(session.opaque_ref, _vm_metrics ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given VM_metrics.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_metrics">The opaque_ref of the given vm_metrics</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _vm_metrics, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vm_metrics_remove_from_other_config(session.opaque_ref, _vm_metrics, _key);
            else
                session.proxy.vm_metrics_remove_from_other_config(session.opaque_ref, _vm_metrics ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// Return a list of all the VM_metrics instances known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<VM_metrics>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_all(session.opaque_ref);
            else
                return XenRef<VM_metrics>.Create(session.proxy.vm_metrics_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the VM_metrics Records at once, in a single XML RPC call
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<VM_metrics>, VM_metrics> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vm_metrics_get_all_records(session.opaque_ref);
            else
                return XenRef<VM_metrics>.Create<Proxy_VM_metrics>(session.proxy.vm_metrics_get_all_records(session.opaque_ref).parse());
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
        /// Guest's actual memory (bytes)
        /// </summary>
        public virtual long memory_actual
        {
            get { return _memory_actual; }
            set
            {
                if (!Helper.AreEqual(value, _memory_actual))
                {
                    _memory_actual = value;
                    Changed = true;
                    NotifyPropertyChanged("memory_actual");
                }
            }
        }
        private long _memory_actual;

        /// <summary>
        /// Current number of VCPUs
        /// </summary>
        public virtual long VCPUs_number
        {
            get { return _VCPUs_number; }
            set
            {
                if (!Helper.AreEqual(value, _VCPUs_number))
                {
                    _VCPUs_number = value;
                    Changed = true;
                    NotifyPropertyChanged("VCPUs_number");
                }
            }
        }
        private long _VCPUs_number;

        /// <summary>
        /// Utilisation for all of guest's current VCPUs
        /// </summary>
        public virtual Dictionary<long, double> VCPUs_utilisation
        {
            get { return _VCPUs_utilisation; }
            set
            {
                if (!Helper.AreEqual(value, _VCPUs_utilisation))
                {
                    _VCPUs_utilisation = value;
                    Changed = true;
                    NotifyPropertyChanged("VCPUs_utilisation");
                }
            }
        }
        private Dictionary<long, double> _VCPUs_utilisation = new Dictionary<long, double>() {};

        /// <summary>
        /// VCPU to PCPU map
        /// </summary>
        public virtual Dictionary<long, long> VCPUs_CPU
        {
            get { return _VCPUs_CPU; }
            set
            {
                if (!Helper.AreEqual(value, _VCPUs_CPU))
                {
                    _VCPUs_CPU = value;
                    Changed = true;
                    NotifyPropertyChanged("VCPUs_CPU");
                }
            }
        }
        private Dictionary<long, long> _VCPUs_CPU = new Dictionary<long, long>() {};

        /// <summary>
        /// The live equivalent to VM.VCPUs_params
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> VCPUs_params
        {
            get { return _VCPUs_params; }
            set
            {
                if (!Helper.AreEqual(value, _VCPUs_params))
                {
                    _VCPUs_params = value;
                    Changed = true;
                    NotifyPropertyChanged("VCPUs_params");
                }
            }
        }
        private Dictionary<string, string> _VCPUs_params = new Dictionary<string, string>() {};

        /// <summary>
        /// CPU flags (blocked,online,running)
        /// </summary>
        public virtual Dictionary<long, string[]> VCPUs_flags
        {
            get { return _VCPUs_flags; }
            set
            {
                if (!Helper.AreEqual(value, _VCPUs_flags))
                {
                    _VCPUs_flags = value;
                    Changed = true;
                    NotifyPropertyChanged("VCPUs_flags");
                }
            }
        }
        private Dictionary<long, string[]> _VCPUs_flags = new Dictionary<long, string[]>() {};

        /// <summary>
        /// The state of the guest, eg blocked, dying etc
        /// </summary>
        public virtual string[] state
        {
            get { return _state; }
            set
            {
                if (!Helper.AreEqual(value, _state))
                {
                    _state = value;
                    Changed = true;
                    NotifyPropertyChanged("state");
                }
            }
        }
        private string[] _state = {};

        /// <summary>
        /// Time at which this VM was last booted
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime start_time
        {
            get { return _start_time; }
            set
            {
                if (!Helper.AreEqual(value, _start_time))
                {
                    _start_time = value;
                    Changed = true;
                    NotifyPropertyChanged("start_time");
                }
            }
        }
        private DateTime _start_time;

        /// <summary>
        /// Time at which the VM was installed
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime install_time
        {
            get { return _install_time; }
            set
            {
                if (!Helper.AreEqual(value, _install_time))
                {
                    _install_time = value;
                    Changed = true;
                    NotifyPropertyChanged("install_time");
                }
            }
        }
        private DateTime _install_time;

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
        /// hardware virtual machine
        /// First published in XenServer 7.1.
        /// </summary>
        public virtual bool hvm
        {
            get { return _hvm; }
            set
            {
                if (!Helper.AreEqual(value, _hvm))
                {
                    _hvm = value;
                    Changed = true;
                    NotifyPropertyChanged("hvm");
                }
            }
        }
        private bool _hvm = false;

        /// <summary>
        /// VM supports nested virtualisation
        /// First published in XenServer 7.1.
        /// </summary>
        public virtual bool nested_virt
        {
            get { return _nested_virt; }
            set
            {
                if (!Helper.AreEqual(value, _nested_virt))
                {
                    _nested_virt = value;
                    Changed = true;
                    NotifyPropertyChanged("nested_virt");
                }
            }
        }
        private bool _nested_virt = false;

        /// <summary>
        /// VM is immobile and can't migrate between hosts
        /// First published in XenServer 7.1.
        /// </summary>
        public virtual bool nomigrate
        {
            get { return _nomigrate; }
            set
            {
                if (!Helper.AreEqual(value, _nomigrate))
                {
                    _nomigrate = value;
                    Changed = true;
                    NotifyPropertyChanged("nomigrate");
                }
            }
        }
        private bool _nomigrate = false;

        /// <summary>
        /// The current domain type of the VM (for running,suspended, or paused VMs). The last-known domain type for halted VMs.
        /// First published in Unreleased.
        /// </summary>
        [JsonConverter(typeof(domain_typeConverter))]
        public virtual domain_type current_domain_type
        {
            get { return _current_domain_type; }
            set
            {
                if (!Helper.AreEqual(value, _current_domain_type))
                {
                    _current_domain_type = value;
                    Changed = true;
                    NotifyPropertyChanged("current_domain_type");
                }
            }
        }
        private domain_type _current_domain_type = domain_type.unspecified;
    }
}
