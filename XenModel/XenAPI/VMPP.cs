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
    /// VM Protection Policy
    /// First published in XenServer 5.6 FP1.
    /// </summary>
    public partial class VMPP : XenObject<VMPP>
    {
        public VMPP()
        {
        }

        public VMPP(string uuid,
            string name_label,
            string name_description,
            bool is_policy_enabled,
            vmpp_backup_type backup_type,
            long backup_retention_value,
            vmpp_backup_frequency backup_frequency,
            Dictionary<string, string> backup_schedule,
            bool is_backup_running,
            DateTime backup_last_run_time,
            vmpp_archive_target_type archive_target_type,
            Dictionary<string, string> archive_target_config,
            vmpp_archive_frequency archive_frequency,
            Dictionary<string, string> archive_schedule,
            bool is_archive_running,
            DateTime archive_last_run_time,
            List<XenRef<VM>> VMs,
            bool is_alarm_enabled,
            Dictionary<string, string> alarm_config,
            string[] recent_alerts)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.is_policy_enabled = is_policy_enabled;
            this.backup_type = backup_type;
            this.backup_retention_value = backup_retention_value;
            this.backup_frequency = backup_frequency;
            this.backup_schedule = backup_schedule;
            this.is_backup_running = is_backup_running;
            this.backup_last_run_time = backup_last_run_time;
            this.archive_target_type = archive_target_type;
            this.archive_target_config = archive_target_config;
            this.archive_frequency = archive_frequency;
            this.archive_schedule = archive_schedule;
            this.is_archive_running = is_archive_running;
            this.archive_last_run_time = archive_last_run_time;
            this.VMs = VMs;
            this.is_alarm_enabled = is_alarm_enabled;
            this.alarm_config = alarm_config;
            this.recent_alerts = recent_alerts;
        }

        /// <summary>
        /// Creates a new VMPP from a Proxy_VMPP.
        /// </summary>
        /// <param name="proxy"></param>
        public VMPP(Proxy_VMPP proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VMPP.
        /// </summary>
        public override void UpdateFrom(VMPP update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            is_policy_enabled = update.is_policy_enabled;
            backup_type = update.backup_type;
            backup_retention_value = update.backup_retention_value;
            backup_frequency = update.backup_frequency;
            backup_schedule = update.backup_schedule;
            is_backup_running = update.is_backup_running;
            backup_last_run_time = update.backup_last_run_time;
            archive_target_type = update.archive_target_type;
            archive_target_config = update.archive_target_config;
            archive_frequency = update.archive_frequency;
            archive_schedule = update.archive_schedule;
            is_archive_running = update.is_archive_running;
            archive_last_run_time = update.archive_last_run_time;
            VMs = update.VMs;
            is_alarm_enabled = update.is_alarm_enabled;
            alarm_config = update.alarm_config;
            recent_alerts = update.recent_alerts;
        }

        internal void UpdateFromProxy(Proxy_VMPP proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            name_label = proxy.name_label == null ? null : proxy.name_label;
            name_description = proxy.name_description == null ? null : proxy.name_description;
            is_policy_enabled = (bool)proxy.is_policy_enabled;
            backup_type = proxy.backup_type == null ? (vmpp_backup_type) 0 : (vmpp_backup_type)Helper.EnumParseDefault(typeof(vmpp_backup_type), (string)proxy.backup_type);
            backup_retention_value = proxy.backup_retention_value == null ? 0 : long.Parse(proxy.backup_retention_value);
            backup_frequency = proxy.backup_frequency == null ? (vmpp_backup_frequency) 0 : (vmpp_backup_frequency)Helper.EnumParseDefault(typeof(vmpp_backup_frequency), (string)proxy.backup_frequency);
            backup_schedule = proxy.backup_schedule == null ? null : Maps.convert_from_proxy_string_string(proxy.backup_schedule);
            is_backup_running = (bool)proxy.is_backup_running;
            backup_last_run_time = proxy.backup_last_run_time;
            archive_target_type = proxy.archive_target_type == null ? (vmpp_archive_target_type) 0 : (vmpp_archive_target_type)Helper.EnumParseDefault(typeof(vmpp_archive_target_type), (string)proxy.archive_target_type);
            archive_target_config = proxy.archive_target_config == null ? null : Maps.convert_from_proxy_string_string(proxy.archive_target_config);
            archive_frequency = proxy.archive_frequency == null ? (vmpp_archive_frequency) 0 : (vmpp_archive_frequency)Helper.EnumParseDefault(typeof(vmpp_archive_frequency), (string)proxy.archive_frequency);
            archive_schedule = proxy.archive_schedule == null ? null : Maps.convert_from_proxy_string_string(proxy.archive_schedule);
            is_archive_running = (bool)proxy.is_archive_running;
            archive_last_run_time = proxy.archive_last_run_time;
            VMs = proxy.VMs == null ? null : XenRef<VM>.Create(proxy.VMs);
            is_alarm_enabled = (bool)proxy.is_alarm_enabled;
            alarm_config = proxy.alarm_config == null ? null : Maps.convert_from_proxy_string_string(proxy.alarm_config);
            recent_alerts = proxy.recent_alerts == null ? new string[] {} : (string [])proxy.recent_alerts;
        }

        public Proxy_VMPP ToProxy()
        {
            Proxy_VMPP result_ = new Proxy_VMPP();
            result_.uuid = uuid ?? "";
            result_.name_label = name_label ?? "";
            result_.name_description = name_description ?? "";
            result_.is_policy_enabled = is_policy_enabled;
            result_.backup_type = vmpp_backup_type_helper.ToString(backup_type);
            result_.backup_retention_value = backup_retention_value.ToString();
            result_.backup_frequency = vmpp_backup_frequency_helper.ToString(backup_frequency);
            result_.backup_schedule = Maps.convert_to_proxy_string_string(backup_schedule);
            result_.is_backup_running = is_backup_running;
            result_.backup_last_run_time = backup_last_run_time;
            result_.archive_target_type = vmpp_archive_target_type_helper.ToString(archive_target_type);
            result_.archive_target_config = Maps.convert_to_proxy_string_string(archive_target_config);
            result_.archive_frequency = vmpp_archive_frequency_helper.ToString(archive_frequency);
            result_.archive_schedule = Maps.convert_to_proxy_string_string(archive_schedule);
            result_.is_archive_running = is_archive_running;
            result_.archive_last_run_time = archive_last_run_time;
            result_.VMs = VMs == null ? new string[] {} : Helper.RefListToStringArray(VMs);
            result_.is_alarm_enabled = is_alarm_enabled;
            result_.alarm_config = Maps.convert_to_proxy_string_string(alarm_config);
            result_.recent_alerts = recent_alerts;
            return result_;
        }

        /// <summary>
        /// Creates a new VMPP from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VMPP(Hashtable table) : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this VMPP
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
            if (table.ContainsKey("is_policy_enabled"))
                is_policy_enabled = Marshalling.ParseBool(table, "is_policy_enabled");
            if (table.ContainsKey("backup_type"))
                backup_type = (vmpp_backup_type)Helper.EnumParseDefault(typeof(vmpp_backup_type), Marshalling.ParseString(table, "backup_type"));
            if (table.ContainsKey("backup_retention_value"))
                backup_retention_value = Marshalling.ParseLong(table, "backup_retention_value");
            if (table.ContainsKey("backup_frequency"))
                backup_frequency = (vmpp_backup_frequency)Helper.EnumParseDefault(typeof(vmpp_backup_frequency), Marshalling.ParseString(table, "backup_frequency"));
            if (table.ContainsKey("backup_schedule"))
                backup_schedule = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "backup_schedule"));
            if (table.ContainsKey("is_backup_running"))
                is_backup_running = Marshalling.ParseBool(table, "is_backup_running");
            if (table.ContainsKey("backup_last_run_time"))
                backup_last_run_time = Marshalling.ParseDateTime(table, "backup_last_run_time");
            if (table.ContainsKey("archive_target_type"))
                archive_target_type = (vmpp_archive_target_type)Helper.EnumParseDefault(typeof(vmpp_archive_target_type), Marshalling.ParseString(table, "archive_target_type"));
            if (table.ContainsKey("archive_target_config"))
                archive_target_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "archive_target_config"));
            if (table.ContainsKey("archive_frequency"))
                archive_frequency = (vmpp_archive_frequency)Helper.EnumParseDefault(typeof(vmpp_archive_frequency), Marshalling.ParseString(table, "archive_frequency"));
            if (table.ContainsKey("archive_schedule"))
                archive_schedule = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "archive_schedule"));
            if (table.ContainsKey("is_archive_running"))
                is_archive_running = Marshalling.ParseBool(table, "is_archive_running");
            if (table.ContainsKey("archive_last_run_time"))
                archive_last_run_time = Marshalling.ParseDateTime(table, "archive_last_run_time");
            if (table.ContainsKey("VMs"))
                VMs = Marshalling.ParseSetRef<VM>(table, "VMs");
            if (table.ContainsKey("is_alarm_enabled"))
                is_alarm_enabled = Marshalling.ParseBool(table, "is_alarm_enabled");
            if (table.ContainsKey("alarm_config"))
                alarm_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "alarm_config"));
            if (table.ContainsKey("recent_alerts"))
                recent_alerts = Marshalling.ParseStringArray(table, "recent_alerts");
        }

        public bool DeepEquals(VMPP other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name_label, other._name_label) &&
                Helper.AreEqual2(this._name_description, other._name_description) &&
                Helper.AreEqual2(this._is_policy_enabled, other._is_policy_enabled) &&
                Helper.AreEqual2(this._backup_type, other._backup_type) &&
                Helper.AreEqual2(this._backup_retention_value, other._backup_retention_value) &&
                Helper.AreEqual2(this._backup_frequency, other._backup_frequency) &&
                Helper.AreEqual2(this._backup_schedule, other._backup_schedule) &&
                Helper.AreEqual2(this._is_backup_running, other._is_backup_running) &&
                Helper.AreEqual2(this._backup_last_run_time, other._backup_last_run_time) &&
                Helper.AreEqual2(this._archive_target_type, other._archive_target_type) &&
                Helper.AreEqual2(this._archive_target_config, other._archive_target_config) &&
                Helper.AreEqual2(this._archive_frequency, other._archive_frequency) &&
                Helper.AreEqual2(this._archive_schedule, other._archive_schedule) &&
                Helper.AreEqual2(this._is_archive_running, other._is_archive_running) &&
                Helper.AreEqual2(this._archive_last_run_time, other._archive_last_run_time) &&
                Helper.AreEqual2(this._VMs, other._VMs) &&
                Helper.AreEqual2(this._is_alarm_enabled, other._is_alarm_enabled) &&
                Helper.AreEqual2(this._alarm_config, other._alarm_config) &&
                Helper.AreEqual2(this._recent_alerts, other._recent_alerts);
        }

        internal static List<VMPP> ProxyArrayToObjectList(Proxy_VMPP[] input)
        {
            var result = new List<VMPP>();
            foreach (var item in input)
                result.Add(new VMPP(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, VMPP server)
        {
            if (opaqueRef == null)
            {
                var reference = create(session, this);
                return reference == null ? null : reference.opaque_ref;
            }
            else
            {
                if (!Helper.AreEqual2(_name_label, server._name_label))
                {
                    VMPP.set_name_label(session, opaqueRef, _name_label);
                }
                if (!Helper.AreEqual2(_name_description, server._name_description))
                {
                    VMPP.set_name_description(session, opaqueRef, _name_description);
                }
                if (!Helper.AreEqual2(_is_policy_enabled, server._is_policy_enabled))
                {
                    VMPP.set_is_policy_enabled(session, opaqueRef, _is_policy_enabled);
                }
                if (!Helper.AreEqual2(_backup_type, server._backup_type))
                {
                    VMPP.set_backup_type(session, opaqueRef, _backup_type);
                }
                if (!Helper.AreEqual2(_backup_retention_value, server._backup_retention_value))
                {
                    VMPP.set_backup_retention_value(session, opaqueRef, _backup_retention_value);
                }
                if (!Helper.AreEqual2(_backup_frequency, server._backup_frequency))
                {
                    VMPP.set_backup_frequency(session, opaqueRef, _backup_frequency);
                }
                if (!Helper.AreEqual2(_backup_schedule, server._backup_schedule))
                {
                    VMPP.set_backup_schedule(session, opaqueRef, _backup_schedule);
                }
                if (!Helper.AreEqual2(_archive_target_type, server._archive_target_type))
                {
                    VMPP.set_archive_target_type(session, opaqueRef, _archive_target_type);
                }
                if (!Helper.AreEqual2(_archive_target_config, server._archive_target_config))
                {
                    VMPP.set_archive_target_config(session, opaqueRef, _archive_target_config);
                }
                if (!Helper.AreEqual2(_archive_frequency, server._archive_frequency))
                {
                    VMPP.set_archive_frequency(session, opaqueRef, _archive_frequency);
                }
                if (!Helper.AreEqual2(_archive_schedule, server._archive_schedule))
                {
                    VMPP.set_archive_schedule(session, opaqueRef, _archive_schedule);
                }
                if (!Helper.AreEqual2(_is_alarm_enabled, server._is_alarm_enabled))
                {
                    VMPP.set_is_alarm_enabled(session, opaqueRef, _is_alarm_enabled);
                }
                if (!Helper.AreEqual2(_alarm_config, server._alarm_config))
                {
                    VMPP.set_alarm_config(session, opaqueRef, _alarm_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static VMPP get_record(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_record(session.opaque_ref, _vmpp);
            else
                return new VMPP(session.proxy.vmpp_get_record(session.opaque_ref, _vmpp ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the VMPP instance with the specified UUID.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<VMPP> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<VMPP>.Create(session.proxy.vmpp_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Create a new VMPP instance, and return its handle.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<VMPP> create(Session session, VMPP _record)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_create(session.opaque_ref, _record);
            else
                return XenRef<VMPP>.Create(session.proxy.vmpp_create(session.opaque_ref, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Create a new VMPP instance, and return its handle.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<Task> async_create(Session session, VMPP _record)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vmpp_create(session.opaque_ref, _record);
          else
              return XenRef<Task>.Create(session.proxy.async_vmpp_create(session.opaque_ref, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Destroy the specified VMPP instance.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static void destroy(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_destroy(session.opaque_ref, _vmpp);
            else
                session.proxy.vmpp_destroy(session.opaque_ref, _vmpp ?? "").parse();
        }

        /// <summary>
        /// Destroy the specified VMPP instance.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static XenRef<Task> async_destroy(Session session, string _vmpp)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vmpp_destroy(session.opaque_ref, _vmpp);
          else
              return XenRef<Task>.Create(session.proxy.async_vmpp_destroy(session.opaque_ref, _vmpp ?? "").parse());
        }

        /// <summary>
        /// Get all the VMPP instances with the given label.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        public static List<XenRef<VMPP>> get_by_name_label(Session session, string _label)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_by_name_label(session.opaque_ref, _label);
            else
                return XenRef<VMPP>.Create(session.proxy.vmpp_get_by_name_label(session.opaque_ref, _label ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static string get_uuid(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_uuid(session.opaque_ref, _vmpp);
            else
                return session.proxy.vmpp_get_uuid(session.opaque_ref, _vmpp ?? "").parse();
        }

        /// <summary>
        /// Get the name/label field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static string get_name_label(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_name_label(session.opaque_ref, _vmpp);
            else
                return session.proxy.vmpp_get_name_label(session.opaque_ref, _vmpp ?? "").parse();
        }

        /// <summary>
        /// Get the name/description field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static string get_name_description(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_name_description(session.opaque_ref, _vmpp);
            else
                return session.proxy.vmpp_get_name_description(session.opaque_ref, _vmpp ?? "").parse();
        }

        /// <summary>
        /// Get the is_policy_enabled field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static bool get_is_policy_enabled(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_is_policy_enabled(session.opaque_ref, _vmpp);
            else
                return (bool)session.proxy.vmpp_get_is_policy_enabled(session.opaque_ref, _vmpp ?? "").parse();
        }

        /// <summary>
        /// Get the backup_type field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static vmpp_backup_type get_backup_type(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_backup_type(session.opaque_ref, _vmpp);
            else
                return (vmpp_backup_type)Helper.EnumParseDefault(typeof(vmpp_backup_type), (string)session.proxy.vmpp_get_backup_type(session.opaque_ref, _vmpp ?? "").parse());
        }

        /// <summary>
        /// Get the backup_retention_value field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static long get_backup_retention_value(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_backup_retention_value(session.opaque_ref, _vmpp);
            else
                return long.Parse(session.proxy.vmpp_get_backup_retention_value(session.opaque_ref, _vmpp ?? "").parse());
        }

        /// <summary>
        /// Get the backup_frequency field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static vmpp_backup_frequency get_backup_frequency(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_backup_frequency(session.opaque_ref, _vmpp);
            else
                return (vmpp_backup_frequency)Helper.EnumParseDefault(typeof(vmpp_backup_frequency), (string)session.proxy.vmpp_get_backup_frequency(session.opaque_ref, _vmpp ?? "").parse());
        }

        /// <summary>
        /// Get the backup_schedule field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static Dictionary<string, string> get_backup_schedule(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_backup_schedule(session.opaque_ref, _vmpp);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vmpp_get_backup_schedule(session.opaque_ref, _vmpp ?? "").parse());
        }

        /// <summary>
        /// Get the is_backup_running field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static bool get_is_backup_running(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_is_backup_running(session.opaque_ref, _vmpp);
            else
                return (bool)session.proxy.vmpp_get_is_backup_running(session.opaque_ref, _vmpp ?? "").parse();
        }

        /// <summary>
        /// Get the backup_last_run_time field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static DateTime get_backup_last_run_time(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_backup_last_run_time(session.opaque_ref, _vmpp);
            else
                return session.proxy.vmpp_get_backup_last_run_time(session.opaque_ref, _vmpp ?? "").parse();
        }

        /// <summary>
        /// Get the archive_target_type field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static vmpp_archive_target_type get_archive_target_type(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_archive_target_type(session.opaque_ref, _vmpp);
            else
                return (vmpp_archive_target_type)Helper.EnumParseDefault(typeof(vmpp_archive_target_type), (string)session.proxy.vmpp_get_archive_target_type(session.opaque_ref, _vmpp ?? "").parse());
        }

        /// <summary>
        /// Get the archive_target_config field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static Dictionary<string, string> get_archive_target_config(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_archive_target_config(session.opaque_ref, _vmpp);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vmpp_get_archive_target_config(session.opaque_ref, _vmpp ?? "").parse());
        }

        /// <summary>
        /// Get the archive_frequency field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static vmpp_archive_frequency get_archive_frequency(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_archive_frequency(session.opaque_ref, _vmpp);
            else
                return (vmpp_archive_frequency)Helper.EnumParseDefault(typeof(vmpp_archive_frequency), (string)session.proxy.vmpp_get_archive_frequency(session.opaque_ref, _vmpp ?? "").parse());
        }

        /// <summary>
        /// Get the archive_schedule field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static Dictionary<string, string> get_archive_schedule(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_archive_schedule(session.opaque_ref, _vmpp);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vmpp_get_archive_schedule(session.opaque_ref, _vmpp ?? "").parse());
        }

        /// <summary>
        /// Get the is_archive_running field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static bool get_is_archive_running(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_is_archive_running(session.opaque_ref, _vmpp);
            else
                return (bool)session.proxy.vmpp_get_is_archive_running(session.opaque_ref, _vmpp ?? "").parse();
        }

        /// <summary>
        /// Get the archive_last_run_time field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static DateTime get_archive_last_run_time(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_archive_last_run_time(session.opaque_ref, _vmpp);
            else
                return session.proxy.vmpp_get_archive_last_run_time(session.opaque_ref, _vmpp ?? "").parse();
        }

        /// <summary>
        /// Get the VMs field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static List<XenRef<VM>> get_VMs(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_vms(session.opaque_ref, _vmpp);
            else
                return XenRef<VM>.Create(session.proxy.vmpp_get_vms(session.opaque_ref, _vmpp ?? "").parse());
        }

        /// <summary>
        /// Get the is_alarm_enabled field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static bool get_is_alarm_enabled(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_is_alarm_enabled(session.opaque_ref, _vmpp);
            else
                return (bool)session.proxy.vmpp_get_is_alarm_enabled(session.opaque_ref, _vmpp ?? "").parse();
        }

        /// <summary>
        /// Get the alarm_config field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static Dictionary<string, string> get_alarm_config(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_alarm_config(session.opaque_ref, _vmpp);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vmpp_get_alarm_config(session.opaque_ref, _vmpp ?? "").parse());
        }

        /// <summary>
        /// Get the recent_alerts field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static string[] get_recent_alerts(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_recent_alerts(session.opaque_ref, _vmpp);
            else
                return (string [])session.proxy.vmpp_get_recent_alerts(session.opaque_ref, _vmpp ?? "").parse();
        }

        /// <summary>
        /// Set the name/label field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_label">New value to set</param>
        public static void set_name_label(Session session, string _vmpp, string _label)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_set_name_label(session.opaque_ref, _vmpp, _label);
            else
                session.proxy.vmpp_set_name_label(session.opaque_ref, _vmpp ?? "", _label ?? "").parse();
        }

        /// <summary>
        /// Set the name/description field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_description">New value to set</param>
        public static void set_name_description(Session session, string _vmpp, string _description)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_set_name_description(session.opaque_ref, _vmpp, _description);
            else
                session.proxy.vmpp_set_name_description(session.opaque_ref, _vmpp ?? "", _description ?? "").parse();
        }

        /// <summary>
        /// Set the is_policy_enabled field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_is_policy_enabled">New value to set</param>
        public static void set_is_policy_enabled(Session session, string _vmpp, bool _is_policy_enabled)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_set_is_policy_enabled(session.opaque_ref, _vmpp, _is_policy_enabled);
            else
                session.proxy.vmpp_set_is_policy_enabled(session.opaque_ref, _vmpp ?? "", _is_policy_enabled).parse();
        }

        /// <summary>
        /// Set the backup_type field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_backup_type">New value to set</param>
        public static void set_backup_type(Session session, string _vmpp, vmpp_backup_type _backup_type)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_set_backup_type(session.opaque_ref, _vmpp, _backup_type);
            else
                session.proxy.vmpp_set_backup_type(session.opaque_ref, _vmpp ?? "", vmpp_backup_type_helper.ToString(_backup_type)).parse();
        }

        /// <summary>
        /// This call executes the protection policy immediately
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        public static string protect_now(Session session, string _vmpp)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_protect_now(session.opaque_ref, _vmpp);
            else
                return session.proxy.vmpp_protect_now(session.opaque_ref, _vmpp ?? "").parse();
        }

        /// <summary>
        /// This call archives the snapshot provided as a parameter
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_snapshot">The snapshot to archive</param>
        public static string archive_now(Session session, string _snapshot)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_archive_now(session.opaque_ref, _snapshot);
            else
                return session.proxy.vmpp_archive_now(session.opaque_ref, _snapshot ?? "").parse();
        }

        /// <summary>
        /// This call fetches a history of alerts for a given protection policy
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_hours_from_now">how many hours in the past the oldest record to fetch is</param>
        public static string[] get_alerts(Session session, string _vmpp, long _hours_from_now)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_alerts(session.opaque_ref, _vmpp, _hours_from_now);
            else
                return (string [])session.proxy.vmpp_get_alerts(session.opaque_ref, _vmpp ?? "", _hours_from_now.ToString()).parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the value to set</param>
        public static void set_backup_retention_value(Session session, string _vmpp, long _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_set_backup_retention_value(session.opaque_ref, _vmpp, _value);
            else
                session.proxy.vmpp_set_backup_retention_value(session.opaque_ref, _vmpp ?? "", _value.ToString()).parse();
        }

        /// <summary>
        /// Set the value of the backup_frequency field
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the backup frequency</param>
        public static void set_backup_frequency(Session session, string _vmpp, vmpp_backup_frequency _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_set_backup_frequency(session.opaque_ref, _vmpp, _value);
            else
                session.proxy.vmpp_set_backup_frequency(session.opaque_ref, _vmpp ?? "", vmpp_backup_frequency_helper.ToString(_value)).parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the value to set</param>
        public static void set_backup_schedule(Session session, string _vmpp, Dictionary<string, string> _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_set_backup_schedule(session.opaque_ref, _vmpp, _value);
            else
                session.proxy.vmpp_set_backup_schedule(session.opaque_ref, _vmpp ?? "", Maps.convert_to_proxy_string_string(_value)).parse();
        }

        /// <summary>
        /// Set the value of the archive_frequency field
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the archive frequency</param>
        public static void set_archive_frequency(Session session, string _vmpp, vmpp_archive_frequency _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_set_archive_frequency(session.opaque_ref, _vmpp, _value);
            else
                session.proxy.vmpp_set_archive_frequency(session.opaque_ref, _vmpp ?? "", vmpp_archive_frequency_helper.ToString(_value)).parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the value to set</param>
        public static void set_archive_schedule(Session session, string _vmpp, Dictionary<string, string> _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_set_archive_schedule(session.opaque_ref, _vmpp, _value);
            else
                session.proxy.vmpp_set_archive_schedule(session.opaque_ref, _vmpp ?? "", Maps.convert_to_proxy_string_string(_value)).parse();
        }

        /// <summary>
        /// Set the value of the archive_target_config_type field
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the archive target config type</param>
        public static void set_archive_target_type(Session session, string _vmpp, vmpp_archive_target_type _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_set_archive_target_type(session.opaque_ref, _vmpp, _value);
            else
                session.proxy.vmpp_set_archive_target_type(session.opaque_ref, _vmpp ?? "", vmpp_archive_target_type_helper.ToString(_value)).parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the value to set</param>
        public static void set_archive_target_config(Session session, string _vmpp, Dictionary<string, string> _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_set_archive_target_config(session.opaque_ref, _vmpp, _value);
            else
                session.proxy.vmpp_set_archive_target_config(session.opaque_ref, _vmpp ?? "", Maps.convert_to_proxy_string_string(_value)).parse();
        }

        /// <summary>
        /// Set the value of the is_alarm_enabled field
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">true if alarm is enabled for this policy</param>
        public static void set_is_alarm_enabled(Session session, string _vmpp, bool _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_set_is_alarm_enabled(session.opaque_ref, _vmpp, _value);
            else
                session.proxy.vmpp_set_is_alarm_enabled(session.opaque_ref, _vmpp ?? "", _value).parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the value to set</param>
        public static void set_alarm_config(Session session, string _vmpp, Dictionary<string, string> _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_set_alarm_config(session.opaque_ref, _vmpp, _value);
            else
                session.proxy.vmpp_set_alarm_config(session.opaque_ref, _vmpp ?? "", Maps.convert_to_proxy_string_string(_value)).parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_key">the key to add</param>
        /// <param name="_value">the value to add</param>
        public static void add_to_backup_schedule(Session session, string _vmpp, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_add_to_backup_schedule(session.opaque_ref, _vmpp, _key, _value);
            else
                session.proxy.vmpp_add_to_backup_schedule(session.opaque_ref, _vmpp ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_key">the key to add</param>
        /// <param name="_value">the value to add</param>
        public static void add_to_archive_target_config(Session session, string _vmpp, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_add_to_archive_target_config(session.opaque_ref, _vmpp, _key, _value);
            else
                session.proxy.vmpp_add_to_archive_target_config(session.opaque_ref, _vmpp ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_key">the key to add</param>
        /// <param name="_value">the value to add</param>
        public static void add_to_archive_schedule(Session session, string _vmpp, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_add_to_archive_schedule(session.opaque_ref, _vmpp, _key, _value);
            else
                session.proxy.vmpp_add_to_archive_schedule(session.opaque_ref, _vmpp ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_key">the key to add</param>
        /// <param name="_value">the value to add</param>
        public static void add_to_alarm_config(Session session, string _vmpp, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_add_to_alarm_config(session.opaque_ref, _vmpp, _key, _value);
            else
                session.proxy.vmpp_add_to_alarm_config(session.opaque_ref, _vmpp ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_key">the key to remove</param>
        public static void remove_from_backup_schedule(Session session, string _vmpp, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_remove_from_backup_schedule(session.opaque_ref, _vmpp, _key);
            else
                session.proxy.vmpp_remove_from_backup_schedule(session.opaque_ref, _vmpp ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_key">the key to remove</param>
        public static void remove_from_archive_target_config(Session session, string _vmpp, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_remove_from_archive_target_config(session.opaque_ref, _vmpp, _key);
            else
                session.proxy.vmpp_remove_from_archive_target_config(session.opaque_ref, _vmpp ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_key">the key to remove</param>
        public static void remove_from_archive_schedule(Session session, string _vmpp, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_remove_from_archive_schedule(session.opaque_ref, _vmpp, _key);
            else
                session.proxy.vmpp_remove_from_archive_schedule(session.opaque_ref, _vmpp ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_key">the key to remove</param>
        public static void remove_from_alarm_config(Session session, string _vmpp, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_remove_from_alarm_config(session.opaque_ref, _vmpp, _key);
            else
                session.proxy.vmpp_remove_from_alarm_config(session.opaque_ref, _vmpp ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the value to set</param>
        public static void set_backup_last_run_time(Session session, string _vmpp, DateTime _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_set_backup_last_run_time(session.opaque_ref, _vmpp, _value);
            else
                session.proxy.vmpp_set_backup_last_run_time(session.opaque_ref, _vmpp ?? "", _value).parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the value to set</param>
        public static void set_archive_last_run_time(Session session, string _vmpp, DateTime _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vmpp_set_archive_last_run_time(session.opaque_ref, _vmpp, _value);
            else
                session.proxy.vmpp_set_archive_last_run_time(session.opaque_ref, _vmpp ?? "", _value).parse();
        }

        /// <summary>
        /// Return a list of all the VMPPs known to the system.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<VMPP>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_all(session.opaque_ref);
            else
                return XenRef<VMPP>.Create(session.proxy.vmpp_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the VMPP Records at once, in a single XML RPC call
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<VMPP>, VMPP> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vmpp_get_all_records(session.opaque_ref);
            else
                return XenRef<VMPP>.Create<Proxy_VMPP>(session.proxy.vmpp_get_all_records(session.opaque_ref).parse());
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
                    Changed = true;
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
                    Changed = true;
                    NotifyPropertyChanged("name_description");
                }
            }
        }
        private string _name_description = "";

        /// <summary>
        /// enable or disable this policy
        /// </summary>
        public virtual bool is_policy_enabled
        {
            get { return _is_policy_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _is_policy_enabled))
                {
                    _is_policy_enabled = value;
                    Changed = true;
                    NotifyPropertyChanged("is_policy_enabled");
                }
            }
        }
        private bool _is_policy_enabled = true;

        /// <summary>
        /// type of the backup sub-policy
        /// </summary>
        [JsonConverter(typeof(vmpp_backup_typeConverter))]
        public virtual vmpp_backup_type backup_type
        {
            get { return _backup_type; }
            set
            {
                if (!Helper.AreEqual(value, _backup_type))
                {
                    _backup_type = value;
                    Changed = true;
                    NotifyPropertyChanged("backup_type");
                }
            }
        }
        private vmpp_backup_type _backup_type = vmpp_backup_type.snapshot;

        /// <summary>
        /// maximum number of backups that should be stored at any time
        /// </summary>
        public virtual long backup_retention_value
        {
            get { return _backup_retention_value; }
            set
            {
                if (!Helper.AreEqual(value, _backup_retention_value))
                {
                    _backup_retention_value = value;
                    Changed = true;
                    NotifyPropertyChanged("backup_retention_value");
                }
            }
        }
        private long _backup_retention_value = 7;

        /// <summary>
        /// frequency of the backup schedule
        /// </summary>
        [JsonConverter(typeof(vmpp_backup_frequencyConverter))]
        public virtual vmpp_backup_frequency backup_frequency
        {
            get { return _backup_frequency; }
            set
            {
                if (!Helper.AreEqual(value, _backup_frequency))
                {
                    _backup_frequency = value;
                    Changed = true;
                    NotifyPropertyChanged("backup_frequency");
                }
            }
        }
        private vmpp_backup_frequency _backup_frequency = vmpp_backup_frequency.daily;

        /// <summary>
        /// schedule of the backup containing 'hour', 'min', 'days'. Date/time-related information is in Local Timezone
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> backup_schedule
        {
            get { return _backup_schedule; }
            set
            {
                if (!Helper.AreEqual(value, _backup_schedule))
                {
                    _backup_schedule = value;
                    Changed = true;
                    NotifyPropertyChanged("backup_schedule");
                }
            }
        }
        private Dictionary<string, string> _backup_schedule = new Dictionary<string, string>() {};

        /// <summary>
        /// true if this protection policy's backup is running
        /// </summary>
        public virtual bool is_backup_running
        {
            get { return _is_backup_running; }
            set
            {
                if (!Helper.AreEqual(value, _is_backup_running))
                {
                    _is_backup_running = value;
                    Changed = true;
                    NotifyPropertyChanged("is_backup_running");
                }
            }
        }
        private bool _is_backup_running;

        /// <summary>
        /// time of the last backup
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime backup_last_run_time
        {
            get { return _backup_last_run_time; }
            set
            {
                if (!Helper.AreEqual(value, _backup_last_run_time))
                {
                    _backup_last_run_time = value;
                    Changed = true;
                    NotifyPropertyChanged("backup_last_run_time");
                }
            }
        }
        private DateTime _backup_last_run_time = DateTime.ParseExact("19700101T00:00:00Z", "yyyyMMddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        /// <summary>
        /// type of the archive target config
        /// </summary>
        [JsonConverter(typeof(vmpp_archive_target_typeConverter))]
        public virtual vmpp_archive_target_type archive_target_type
        {
            get { return _archive_target_type; }
            set
            {
                if (!Helper.AreEqual(value, _archive_target_type))
                {
                    _archive_target_type = value;
                    Changed = true;
                    NotifyPropertyChanged("archive_target_type");
                }
            }
        }
        private vmpp_archive_target_type _archive_target_type = vmpp_archive_target_type.none;

        /// <summary>
        /// configuration for the archive, including its 'location', 'username', 'password'
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> archive_target_config
        {
            get { return _archive_target_config; }
            set
            {
                if (!Helper.AreEqual(value, _archive_target_config))
                {
                    _archive_target_config = value;
                    Changed = true;
                    NotifyPropertyChanged("archive_target_config");
                }
            }
        }
        private Dictionary<string, string> _archive_target_config = new Dictionary<string, string>() {};

        /// <summary>
        /// frequency of the archive schedule
        /// </summary>
        [JsonConverter(typeof(vmpp_archive_frequencyConverter))]
        public virtual vmpp_archive_frequency archive_frequency
        {
            get { return _archive_frequency; }
            set
            {
                if (!Helper.AreEqual(value, _archive_frequency))
                {
                    _archive_frequency = value;
                    Changed = true;
                    NotifyPropertyChanged("archive_frequency");
                }
            }
        }
        private vmpp_archive_frequency _archive_frequency = vmpp_archive_frequency.never;

        /// <summary>
        /// schedule of the archive containing 'hour', 'min', 'days'. Date/time-related information is in Local Timezone
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> archive_schedule
        {
            get { return _archive_schedule; }
            set
            {
                if (!Helper.AreEqual(value, _archive_schedule))
                {
                    _archive_schedule = value;
                    Changed = true;
                    NotifyPropertyChanged("archive_schedule");
                }
            }
        }
        private Dictionary<string, string> _archive_schedule = new Dictionary<string, string>() {};

        /// <summary>
        /// true if this protection policy's archive is running
        /// </summary>
        public virtual bool is_archive_running
        {
            get { return _is_archive_running; }
            set
            {
                if (!Helper.AreEqual(value, _is_archive_running))
                {
                    _is_archive_running = value;
                    Changed = true;
                    NotifyPropertyChanged("is_archive_running");
                }
            }
        }
        private bool _is_archive_running;

        /// <summary>
        /// time of the last archive
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime archive_last_run_time
        {
            get { return _archive_last_run_time; }
            set
            {
                if (!Helper.AreEqual(value, _archive_last_run_time))
                {
                    _archive_last_run_time = value;
                    Changed = true;
                    NotifyPropertyChanged("archive_last_run_time");
                }
            }
        }
        private DateTime _archive_last_run_time = DateTime.ParseExact("19700101T00:00:00Z", "yyyyMMddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        /// <summary>
        /// all VMs attached to this protection policy
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VM>))]
        public virtual List<XenRef<VM>> VMs
        {
            get { return _VMs; }
            set
            {
                if (!Helper.AreEqual(value, _VMs))
                {
                    _VMs = value;
                    Changed = true;
                    NotifyPropertyChanged("VMs");
                }
            }
        }
        private List<XenRef<VM>> _VMs = new List<XenRef<VM>>() {};

        /// <summary>
        /// true if alarm is enabled for this policy
        /// </summary>
        public virtual bool is_alarm_enabled
        {
            get { return _is_alarm_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _is_alarm_enabled))
                {
                    _is_alarm_enabled = value;
                    Changed = true;
                    NotifyPropertyChanged("is_alarm_enabled");
                }
            }
        }
        private bool _is_alarm_enabled = false;

        /// <summary>
        /// configuration for the alarm
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> alarm_config
        {
            get { return _alarm_config; }
            set
            {
                if (!Helper.AreEqual(value, _alarm_config))
                {
                    _alarm_config = value;
                    Changed = true;
                    NotifyPropertyChanged("alarm_config");
                }
            }
        }
        private Dictionary<string, string> _alarm_config = new Dictionary<string, string>() {};

        /// <summary>
        /// recent alerts
        /// </summary>
        public virtual string[] recent_alerts
        {
            get { return _recent_alerts; }
            set
            {
                if (!Helper.AreEqual(value, _recent_alerts))
                {
                    _recent_alerts = value;
                    Changed = true;
                    NotifyPropertyChanged("recent_alerts");
                }
            }
        }
        private string[] _recent_alerts = {};
    }
}
