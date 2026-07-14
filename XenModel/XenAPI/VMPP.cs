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
    /// VM Protection Policy
    /// First published in XenServer 5.6 FP1.
    /// </summary>
    public partial class VMPP : XenObject<VMPP>
    {
        #region Constructors

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
        /// Creates a new VMPP from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VMPP(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VMPP.
        /// </summary>
        public override void UpdateFrom(VMPP record)
        {
            uuid = record.uuid;
            name_label = record.name_label;
            name_description = record.name_description;
            is_policy_enabled = record.is_policy_enabled;
            backup_type = record.backup_type;
            backup_retention_value = record.backup_retention_value;
            backup_frequency = record.backup_frequency;
            backup_schedule = record.backup_schedule;
            is_backup_running = record.is_backup_running;
            backup_last_run_time = record.backup_last_run_time;
            archive_target_type = record.archive_target_type;
            archive_target_config = record.archive_target_config;
            archive_frequency = record.archive_frequency;
            archive_schedule = record.archive_schedule;
            is_archive_running = record.is_archive_running;
            archive_last_run_time = record.archive_last_run_time;
            VMs = record.VMs;
            is_alarm_enabled = record.is_alarm_enabled;
            alarm_config = record.alarm_config;
            recent_alerts = record.recent_alerts;
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
                backup_schedule = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "backup_schedule"));
            if (table.ContainsKey("is_backup_running"))
                is_backup_running = Marshalling.ParseBool(table, "is_backup_running");
            if (table.ContainsKey("backup_last_run_time"))
                backup_last_run_time = Marshalling.ParseDateTime(table, "backup_last_run_time");
            if (table.ContainsKey("archive_target_type"))
                archive_target_type = (vmpp_archive_target_type)Helper.EnumParseDefault(typeof(vmpp_archive_target_type), Marshalling.ParseString(table, "archive_target_type"));
            if (table.ContainsKey("archive_target_config"))
                archive_target_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "archive_target_config"));
            if (table.ContainsKey("archive_frequency"))
                archive_frequency = (vmpp_archive_frequency)Helper.EnumParseDefault(typeof(vmpp_archive_frequency), Marshalling.ParseString(table, "archive_frequency"));
            if (table.ContainsKey("archive_schedule"))
                archive_schedule = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "archive_schedule"));
            if (table.ContainsKey("is_archive_running"))
                is_archive_running = Marshalling.ParseBool(table, "is_archive_running");
            if (table.ContainsKey("archive_last_run_time"))
                archive_last_run_time = Marshalling.ParseDateTime(table, "archive_last_run_time");
            if (table.ContainsKey("VMs"))
                VMs = Marshalling.ParseSetRef<VM>(table, "VMs");
            if (table.ContainsKey("is_alarm_enabled"))
                is_alarm_enabled = Marshalling.ParseBool(table, "is_alarm_enabled");
            if (table.ContainsKey("alarm_config"))
                alarm_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "alarm_config"));
            if (table.ContainsKey("recent_alerts"))
                recent_alerts = Marshalling.ParseStringArray(table, "recent_alerts");
        }

        public bool DeepEquals(VMPP other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name_label, other._name_label) &&
                Helper.AreEqual2(_name_description, other._name_description) &&
                Helper.AreEqual2(_is_policy_enabled, other._is_policy_enabled) &&
                Helper.AreEqual2(_backup_type, other._backup_type) &&
                Helper.AreEqual2(_backup_retention_value, other._backup_retention_value) &&
                Helper.AreEqual2(_backup_frequency, other._backup_frequency) &&
                Helper.AreEqual2(_backup_schedule, other._backup_schedule) &&
                Helper.AreEqual2(_is_backup_running, other._is_backup_running) &&
                Helper.AreEqual2(_backup_last_run_time, other._backup_last_run_time) &&
                Helper.AreEqual2(_archive_target_type, other._archive_target_type) &&
                Helper.AreEqual2(_archive_target_config, other._archive_target_config) &&
                Helper.AreEqual2(_archive_frequency, other._archive_frequency) &&
                Helper.AreEqual2(_archive_schedule, other._archive_schedule) &&
                Helper.AreEqual2(_is_archive_running, other._is_archive_running) &&
                Helper.AreEqual2(_archive_last_run_time, other._archive_last_run_time) &&
                Helper.AreEqual2(_VMs, other._VMs) &&
                Helper.AreEqual2(_is_alarm_enabled, other._is_alarm_enabled) &&
                Helper.AreEqual2(_alarm_config, other._alarm_config) &&
                Helper.AreEqual2(_recent_alerts, other._recent_alerts);
        }


        /// <summary>
        /// Get a record containing the current state of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static VMPP get_record(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_record(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get a reference to the VMPP instance with the specified UUID.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VMPP> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.vmpp_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Create a new VMPP instance, and return its handle.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<VMPP> create(Session session, VMPP _record)
        {
            return session.JsonRpcClient.vmpp_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Create a new VMPP instance, and return its handle.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, VMPP _record)
        {
          return session.JsonRpcClient.async_vmpp_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Destroy the specified VMPP instance.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _vmpp)
        {
            session.JsonRpcClient.vmpp_destroy(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Destroy the specified VMPP instance.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _vmpp)
        {
          return session.JsonRpcClient.async_vmpp_destroy(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get all the VMPP instances with the given label.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VMPP>> get_by_name_label(Session session, string _label)
        {
            return session.JsonRpcClient.vmpp_get_by_name_label(session.opaque_ref, _label);
        }

        /// <summary>
        /// Get the uuid field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_uuid(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the name/label field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_label(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_name_label(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the name/description field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_description(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_name_description(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the is_policy_enabled field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_is_policy_enabled(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_is_policy_enabled(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the backup_type field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static vmpp_backup_type get_backup_type(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_backup_type(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the backup_retention_value field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_backup_retention_value(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_backup_retention_value(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the backup_frequency field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static vmpp_backup_frequency get_backup_frequency(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_backup_frequency(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the backup_schedule field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_backup_schedule(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_backup_schedule(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the is_backup_running field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_is_backup_running(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_is_backup_running(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the backup_last_run_time field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_backup_last_run_time(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_backup_last_run_time(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the archive_target_type field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static vmpp_archive_target_type get_archive_target_type(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_archive_target_type(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the archive_target_config field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_archive_target_config(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_archive_target_config(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the archive_frequency field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static vmpp_archive_frequency get_archive_frequency(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_archive_frequency(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the archive_schedule field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_archive_schedule(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_archive_schedule(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the is_archive_running field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_is_archive_running(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_is_archive_running(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the archive_last_run_time field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_archive_last_run_time(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_archive_last_run_time(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the VMs field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VM>> get_VMs(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_vms(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the is_alarm_enabled field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_is_alarm_enabled(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_is_alarm_enabled(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the alarm_config field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_alarm_config(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_alarm_config(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Get the recent_alerts field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_recent_alerts(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_get_recent_alerts(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// Set the name/label field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_label">New value to set</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_name_label(Session session, string _vmpp, string _label)
        {
            session.JsonRpcClient.vmpp_set_name_label(session.opaque_ref, _vmpp, _label);
        }

        /// <summary>
        /// Set the name/description field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_description">New value to set</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_name_description(Session session, string _vmpp, string _description)
        {
            session.JsonRpcClient.vmpp_set_name_description(session.opaque_ref, _vmpp, _description);
        }

        /// <summary>
        /// Set the is_policy_enabled field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_is_policy_enabled">New value to set</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_is_policy_enabled(Session session, string _vmpp, bool _is_policy_enabled)
        {
            session.JsonRpcClient.vmpp_set_is_policy_enabled(session.opaque_ref, _vmpp, _is_policy_enabled);
        }

        /// <summary>
        /// Set the backup_type field of the given VMPP.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_backup_type">New value to set</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_backup_type(Session session, string _vmpp, vmpp_backup_type _backup_type)
        {
            session.JsonRpcClient.vmpp_set_backup_type(session.opaque_ref, _vmpp, _backup_type);
        }

        /// <summary>
        /// This call executes the protection policy immediately
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static string protect_now(Session session, string _vmpp)
        {
            return session.JsonRpcClient.vmpp_protect_now(session.opaque_ref, _vmpp);
        }

        /// <summary>
        /// This call archives the snapshot provided as a parameter
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_snapshot">The snapshot to archive</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static string archive_now(Session session, string _snapshot)
        {
            return session.JsonRpcClient.vmpp_archive_now(session.opaque_ref, _snapshot);
        }

        /// <summary>
        /// This call fetches a history of alerts for a given protection policy
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_hours_from_now">how many hours in the past the oldest record to fetch is</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static string[] get_alerts(Session session, string _vmpp, long _hours_from_now)
        {
            return session.JsonRpcClient.vmpp_get_alerts(session.opaque_ref, _vmpp, _hours_from_now);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the value to set</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_backup_retention_value(Session session, string _vmpp, long _value)
        {
            session.JsonRpcClient.vmpp_set_backup_retention_value(session.opaque_ref, _vmpp, _value);
        }

        /// <summary>
        /// Set the value of the backup_frequency field
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the backup frequency</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_backup_frequency(Session session, string _vmpp, vmpp_backup_frequency _value)
        {
            session.JsonRpcClient.vmpp_set_backup_frequency(session.opaque_ref, _vmpp, _value);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the value to set</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_backup_schedule(Session session, string _vmpp, Dictionary<string, string> _value)
        {
            session.JsonRpcClient.vmpp_set_backup_schedule(session.opaque_ref, _vmpp, _value);
        }

        /// <summary>
        /// Set the value of the archive_frequency field
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the archive frequency</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_archive_frequency(Session session, string _vmpp, vmpp_archive_frequency _value)
        {
            session.JsonRpcClient.vmpp_set_archive_frequency(session.opaque_ref, _vmpp, _value);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the value to set</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_archive_schedule(Session session, string _vmpp, Dictionary<string, string> _value)
        {
            session.JsonRpcClient.vmpp_set_archive_schedule(session.opaque_ref, _vmpp, _value);
        }

        /// <summary>
        /// Set the value of the archive_target_config_type field
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the archive target config type</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_archive_target_type(Session session, string _vmpp, vmpp_archive_target_type _value)
        {
            session.JsonRpcClient.vmpp_set_archive_target_type(session.opaque_ref, _vmpp, _value);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the value to set</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_archive_target_config(Session session, string _vmpp, Dictionary<string, string> _value)
        {
            session.JsonRpcClient.vmpp_set_archive_target_config(session.opaque_ref, _vmpp, _value);
        }

        /// <summary>
        /// Set the value of the is_alarm_enabled field
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">true if alarm is enabled for this policy</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_is_alarm_enabled(Session session, string _vmpp, bool _value)
        {
            session.JsonRpcClient.vmpp_set_is_alarm_enabled(session.opaque_ref, _vmpp, _value);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">the value to set</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_alarm_config(Session session, string _vmpp, Dictionary<string, string> _value)
        {
            session.JsonRpcClient.vmpp_set_alarm_config(session.opaque_ref, _vmpp, _value);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_key">the key to add</param>
        /// <param name="_value">the value to add</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_backup_schedule(Session session, string _vmpp, string _key, string _value)
        {
            session.JsonRpcClient.vmpp_add_to_backup_schedule(session.opaque_ref, _vmpp, _key, _value);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_key">the key to add</param>
        /// <param name="_value">the value to add</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_archive_target_config(Session session, string _vmpp, string _key, string _value)
        {
            session.JsonRpcClient.vmpp_add_to_archive_target_config(session.opaque_ref, _vmpp, _key, _value);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_key">the key to add</param>
        /// <param name="_value">the value to add</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_archive_schedule(Session session, string _vmpp, string _key, string _value)
        {
            session.JsonRpcClient.vmpp_add_to_archive_schedule(session.opaque_ref, _vmpp, _key, _value);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_key">the key to add</param>
        /// <param name="_value">the value to add</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_alarm_config(Session session, string _vmpp, string _key, string _value)
        {
            session.JsonRpcClient.vmpp_add_to_alarm_config(session.opaque_ref, _vmpp, _key, _value);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_key">the key to remove</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_backup_schedule(Session session, string _vmpp, string _key)
        {
            session.JsonRpcClient.vmpp_remove_from_backup_schedule(session.opaque_ref, _vmpp, _key);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_key">the key to remove</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_archive_target_config(Session session, string _vmpp, string _key)
        {
            session.JsonRpcClient.vmpp_remove_from_archive_target_config(session.opaque_ref, _vmpp, _key);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_key">the key to remove</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_archive_schedule(Session session, string _vmpp, string _key)
        {
            session.JsonRpcClient.vmpp_remove_from_archive_schedule(session.opaque_ref, _vmpp, _key);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_key">the key to remove</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_alarm_config(Session session, string _vmpp, string _key)
        {
            session.JsonRpcClient.vmpp_remove_from_alarm_config(session.opaque_ref, _vmpp, _key);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">The time at which the last backup was done. When the timezone is missing, UTC is assumed</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: Not Applicable
        /// </remarks>
        public static void set_backup_last_run_time(Session session, string _vmpp, DateTime _value)
        {
            session.JsonRpcClient.vmpp_set_backup_last_run_time(session.opaque_ref, _vmpp, _value);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmpp">The opaque_ref of the given vmpp</param>
        /// <param name="_value">The time at which the last archive was created. When the timezone is missing, UTC is assumed</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: Not Applicable
        /// </remarks>
        public static void set_archive_last_run_time(Session session, string _vmpp, DateTime _value)
        {
            session.JsonRpcClient.vmpp_set_archive_last_run_time(session.opaque_ref, _vmpp, _value);
        }

        /// <summary>
        /// Return a list of all the VMPPs known to the system.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VMPP>> get_all(Session session)
        {
            return session.JsonRpcClient.vmpp_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of VMPP references to VMPP records for all VMPPs known to the system.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<VMPP>, VMPP> get_all_records(Session session)
        {
            return session.JsonRpcClient.vmpp_get_all_records(session.opaque_ref);
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
                    NotifyPropertyChanged("recent_alerts");
                }
            }
        }
        private string[] _recent_alerts = {};
    }
}
