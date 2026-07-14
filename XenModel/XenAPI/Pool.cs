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
    /// Pool-wide information
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class Pool : XenObject<Pool>
    {
        #region Constructors

        public Pool()
        {
        }

        public Pool(string uuid,
            string name_label,
            string name_description,
            XenRef<Host> master,
            XenRef<SR> default_SR,
            XenRef<SR> suspend_image_SR,
            XenRef<SR> crash_dump_SR,
            Dictionary<string, string> other_config,
            bool ha_enabled,
            Dictionary<string, string> ha_configuration,
            string[] ha_statefiles,
            long ha_host_failures_to_tolerate,
            long ha_plan_exists_for,
            bool ha_allow_overcommit,
            bool ha_overcommitted,
            Dictionary<string, XenRef<Blob>> blobs,
            string[] tags,
            Dictionary<string, string> gui_config,
            Dictionary<string, string> health_check_config,
            string wlb_url,
            string wlb_username,
            bool wlb_enabled,
            bool wlb_verify_cert,
            bool redo_log_enabled,
            XenRef<VDI> redo_log_vdi,
            string vswitch_controller,
            Dictionary<string, string> restrictions,
            List<XenRef<VDI>> metadata_VDIs,
            string ha_cluster_stack,
            List<pool_allowed_operations> allowed_operations,
            Dictionary<string, pool_allowed_operations> current_operations,
            Dictionary<string, string> guest_agent_config,
            Dictionary<string, string> cpu_info,
            bool policy_no_vendor_device,
            bool live_patching_disabled,
            bool igmp_snooping_enabled,
            string uefi_certificates,
            string custom_uefi_certificates,
            bool is_psr_pending,
            bool tls_verification_enabled,
            List<XenRef<Repository>> repositories,
            bool client_certificate_auth_enabled,
            string client_certificate_auth_name,
            string repository_proxy_url,
            string repository_proxy_username,
            XenRef<Secret> repository_proxy_password,
            bool migration_compression,
            bool coordinator_bias,
            long local_auth_max_threads,
            long ext_auth_max_threads,
            bool ext_auth_cache_enabled,
            long ext_auth_cache_size,
            long ext_auth_cache_expiry,
            XenRef<Secret> telemetry_uuid,
            telemetry_frequency telemetry_frequency,
            DateTime telemetry_next_collection,
            DateTime last_update_sync,
            update_sync_frequency update_sync_frequency,
            long update_sync_day,
            bool update_sync_enabled,
            Dictionary<string, string> recommendations,
            Dictionary<string, string> license_server,
            bool ha_reboot_vm_on_internal_shutdown,
            bool limit_console_sessions,
            long vm_console_idle_timeout,
            bool auto_update_vm_secureboot_certificates)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.master = master;
            this.default_SR = default_SR;
            this.suspend_image_SR = suspend_image_SR;
            this.crash_dump_SR = crash_dump_SR;
            this.other_config = other_config;
            this.ha_enabled = ha_enabled;
            this.ha_configuration = ha_configuration;
            this.ha_statefiles = ha_statefiles;
            this.ha_host_failures_to_tolerate = ha_host_failures_to_tolerate;
            this.ha_plan_exists_for = ha_plan_exists_for;
            this.ha_allow_overcommit = ha_allow_overcommit;
            this.ha_overcommitted = ha_overcommitted;
            this.blobs = blobs;
            this.tags = tags;
            this.gui_config = gui_config;
            this.health_check_config = health_check_config;
            this.wlb_url = wlb_url;
            this.wlb_username = wlb_username;
            this.wlb_enabled = wlb_enabled;
            this.wlb_verify_cert = wlb_verify_cert;
            this.redo_log_enabled = redo_log_enabled;
            this.redo_log_vdi = redo_log_vdi;
            this.vswitch_controller = vswitch_controller;
            this.restrictions = restrictions;
            this.metadata_VDIs = metadata_VDIs;
            this.ha_cluster_stack = ha_cluster_stack;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.guest_agent_config = guest_agent_config;
            this.cpu_info = cpu_info;
            this.policy_no_vendor_device = policy_no_vendor_device;
            this.live_patching_disabled = live_patching_disabled;
            this.igmp_snooping_enabled = igmp_snooping_enabled;
            this.uefi_certificates = uefi_certificates;
            this.custom_uefi_certificates = custom_uefi_certificates;
            this.is_psr_pending = is_psr_pending;
            this.tls_verification_enabled = tls_verification_enabled;
            this.repositories = repositories;
            this.client_certificate_auth_enabled = client_certificate_auth_enabled;
            this.client_certificate_auth_name = client_certificate_auth_name;
            this.repository_proxy_url = repository_proxy_url;
            this.repository_proxy_username = repository_proxy_username;
            this.repository_proxy_password = repository_proxy_password;
            this.migration_compression = migration_compression;
            this.coordinator_bias = coordinator_bias;
            this.local_auth_max_threads = local_auth_max_threads;
            this.ext_auth_max_threads = ext_auth_max_threads;
            this.ext_auth_cache_enabled = ext_auth_cache_enabled;
            this.ext_auth_cache_size = ext_auth_cache_size;
            this.ext_auth_cache_expiry = ext_auth_cache_expiry;
            this.telemetry_uuid = telemetry_uuid;
            this.telemetry_frequency = telemetry_frequency;
            this.telemetry_next_collection = telemetry_next_collection;
            this.last_update_sync = last_update_sync;
            this.update_sync_frequency = update_sync_frequency;
            this.update_sync_day = update_sync_day;
            this.update_sync_enabled = update_sync_enabled;
            this.recommendations = recommendations;
            this.license_server = license_server;
            this.ha_reboot_vm_on_internal_shutdown = ha_reboot_vm_on_internal_shutdown;
            this.limit_console_sessions = limit_console_sessions;
            this.vm_console_idle_timeout = vm_console_idle_timeout;
            this.auto_update_vm_secureboot_certificates = auto_update_vm_secureboot_certificates;
        }

        /// <summary>
        /// Creates a new Pool from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Pool(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Pool.
        /// </summary>
        public override void UpdateFrom(Pool record)
        {
            uuid = record.uuid;
            name_label = record.name_label;
            name_description = record.name_description;
            master = record.master;
            default_SR = record.default_SR;
            suspend_image_SR = record.suspend_image_SR;
            crash_dump_SR = record.crash_dump_SR;
            other_config = record.other_config;
            ha_enabled = record.ha_enabled;
            ha_configuration = record.ha_configuration;
            ha_statefiles = record.ha_statefiles;
            ha_host_failures_to_tolerate = record.ha_host_failures_to_tolerate;
            ha_plan_exists_for = record.ha_plan_exists_for;
            ha_allow_overcommit = record.ha_allow_overcommit;
            ha_overcommitted = record.ha_overcommitted;
            blobs = record.blobs;
            tags = record.tags;
            gui_config = record.gui_config;
            health_check_config = record.health_check_config;
            wlb_url = record.wlb_url;
            wlb_username = record.wlb_username;
            wlb_enabled = record.wlb_enabled;
            wlb_verify_cert = record.wlb_verify_cert;
            redo_log_enabled = record.redo_log_enabled;
            redo_log_vdi = record.redo_log_vdi;
            vswitch_controller = record.vswitch_controller;
            restrictions = record.restrictions;
            metadata_VDIs = record.metadata_VDIs;
            ha_cluster_stack = record.ha_cluster_stack;
            allowed_operations = record.allowed_operations;
            current_operations = record.current_operations;
            guest_agent_config = record.guest_agent_config;
            cpu_info = record.cpu_info;
            policy_no_vendor_device = record.policy_no_vendor_device;
            live_patching_disabled = record.live_patching_disabled;
            igmp_snooping_enabled = record.igmp_snooping_enabled;
            uefi_certificates = record.uefi_certificates;
            custom_uefi_certificates = record.custom_uefi_certificates;
            is_psr_pending = record.is_psr_pending;
            tls_verification_enabled = record.tls_verification_enabled;
            repositories = record.repositories;
            client_certificate_auth_enabled = record.client_certificate_auth_enabled;
            client_certificate_auth_name = record.client_certificate_auth_name;
            repository_proxy_url = record.repository_proxy_url;
            repository_proxy_username = record.repository_proxy_username;
            repository_proxy_password = record.repository_proxy_password;
            migration_compression = record.migration_compression;
            coordinator_bias = record.coordinator_bias;
            local_auth_max_threads = record.local_auth_max_threads;
            ext_auth_max_threads = record.ext_auth_max_threads;
            ext_auth_cache_enabled = record.ext_auth_cache_enabled;
            ext_auth_cache_size = record.ext_auth_cache_size;
            ext_auth_cache_expiry = record.ext_auth_cache_expiry;
            telemetry_uuid = record.telemetry_uuid;
            telemetry_frequency = record.telemetry_frequency;
            telemetry_next_collection = record.telemetry_next_collection;
            last_update_sync = record.last_update_sync;
            update_sync_frequency = record.update_sync_frequency;
            update_sync_day = record.update_sync_day;
            update_sync_enabled = record.update_sync_enabled;
            recommendations = record.recommendations;
            license_server = record.license_server;
            ha_reboot_vm_on_internal_shutdown = record.ha_reboot_vm_on_internal_shutdown;
            limit_console_sessions = record.limit_console_sessions;
            vm_console_idle_timeout = record.vm_console_idle_timeout;
            auto_update_vm_secureboot_certificates = record.auto_update_vm_secureboot_certificates;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Pool
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
            if (table.ContainsKey("master"))
                master = Marshalling.ParseRef<Host>(table, "master");
            if (table.ContainsKey("default_SR"))
                default_SR = Marshalling.ParseRef<SR>(table, "default_SR");
            if (table.ContainsKey("suspend_image_SR"))
                suspend_image_SR = Marshalling.ParseRef<SR>(table, "suspend_image_SR");
            if (table.ContainsKey("crash_dump_SR"))
                crash_dump_SR = Marshalling.ParseRef<SR>(table, "crash_dump_SR");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("ha_enabled"))
                ha_enabled = Marshalling.ParseBool(table, "ha_enabled");
            if (table.ContainsKey("ha_configuration"))
                ha_configuration = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "ha_configuration"));
            if (table.ContainsKey("ha_statefiles"))
                ha_statefiles = Marshalling.ParseStringArray(table, "ha_statefiles");
            if (table.ContainsKey("ha_host_failures_to_tolerate"))
                ha_host_failures_to_tolerate = Marshalling.ParseLong(table, "ha_host_failures_to_tolerate");
            if (table.ContainsKey("ha_plan_exists_for"))
                ha_plan_exists_for = Marshalling.ParseLong(table, "ha_plan_exists_for");
            if (table.ContainsKey("ha_allow_overcommit"))
                ha_allow_overcommit = Marshalling.ParseBool(table, "ha_allow_overcommit");
            if (table.ContainsKey("ha_overcommitted"))
                ha_overcommitted = Marshalling.ParseBool(table, "ha_overcommitted");
            if (table.ContainsKey("blobs"))
                blobs = Maps.ToDictionary_string_XenRefBlob(Marshalling.ParseHashTable(table, "blobs"));
            if (table.ContainsKey("tags"))
                tags = Marshalling.ParseStringArray(table, "tags");
            if (table.ContainsKey("gui_config"))
                gui_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "gui_config"));
            if (table.ContainsKey("health_check_config"))
                health_check_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "health_check_config"));
            if (table.ContainsKey("wlb_url"))
                wlb_url = Marshalling.ParseString(table, "wlb_url");
            if (table.ContainsKey("wlb_username"))
                wlb_username = Marshalling.ParseString(table, "wlb_username");
            if (table.ContainsKey("wlb_enabled"))
                wlb_enabled = Marshalling.ParseBool(table, "wlb_enabled");
            if (table.ContainsKey("wlb_verify_cert"))
                wlb_verify_cert = Marshalling.ParseBool(table, "wlb_verify_cert");
            if (table.ContainsKey("redo_log_enabled"))
                redo_log_enabled = Marshalling.ParseBool(table, "redo_log_enabled");
            if (table.ContainsKey("redo_log_vdi"))
                redo_log_vdi = Marshalling.ParseRef<VDI>(table, "redo_log_vdi");
            if (table.ContainsKey("vswitch_controller"))
                vswitch_controller = Marshalling.ParseString(table, "vswitch_controller");
            if (table.ContainsKey("restrictions"))
                restrictions = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "restrictions"));
            if (table.ContainsKey("metadata_VDIs"))
                metadata_VDIs = Marshalling.ParseSetRef<VDI>(table, "metadata_VDIs");
            if (table.ContainsKey("ha_cluster_stack"))
                ha_cluster_stack = Marshalling.ParseString(table, "ha_cluster_stack");
            if (table.ContainsKey("allowed_operations"))
                allowed_operations = Helper.StringArrayToEnumList<pool_allowed_operations>(Marshalling.ParseStringArray(table, "allowed_operations"));
            if (table.ContainsKey("current_operations"))
                current_operations = Maps.ToDictionary_string_pool_allowed_operations(Marshalling.ParseHashTable(table, "current_operations"));
            if (table.ContainsKey("guest_agent_config"))
                guest_agent_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "guest_agent_config"));
            if (table.ContainsKey("cpu_info"))
                cpu_info = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "cpu_info"));
            if (table.ContainsKey("policy_no_vendor_device"))
                policy_no_vendor_device = Marshalling.ParseBool(table, "policy_no_vendor_device");
            if (table.ContainsKey("live_patching_disabled"))
                live_patching_disabled = Marshalling.ParseBool(table, "live_patching_disabled");
            if (table.ContainsKey("igmp_snooping_enabled"))
                igmp_snooping_enabled = Marshalling.ParseBool(table, "igmp_snooping_enabled");
            if (table.ContainsKey("uefi_certificates"))
                uefi_certificates = Marshalling.ParseString(table, "uefi_certificates");
            if (table.ContainsKey("custom_uefi_certificates"))
                custom_uefi_certificates = Marshalling.ParseString(table, "custom_uefi_certificates");
            if (table.ContainsKey("is_psr_pending"))
                is_psr_pending = Marshalling.ParseBool(table, "is_psr_pending");
            if (table.ContainsKey("tls_verification_enabled"))
                tls_verification_enabled = Marshalling.ParseBool(table, "tls_verification_enabled");
            if (table.ContainsKey("repositories"))
                repositories = Marshalling.ParseSetRef<Repository>(table, "repositories");
            if (table.ContainsKey("client_certificate_auth_enabled"))
                client_certificate_auth_enabled = Marshalling.ParseBool(table, "client_certificate_auth_enabled");
            if (table.ContainsKey("client_certificate_auth_name"))
                client_certificate_auth_name = Marshalling.ParseString(table, "client_certificate_auth_name");
            if (table.ContainsKey("repository_proxy_url"))
                repository_proxy_url = Marshalling.ParseString(table, "repository_proxy_url");
            if (table.ContainsKey("repository_proxy_username"))
                repository_proxy_username = Marshalling.ParseString(table, "repository_proxy_username");
            if (table.ContainsKey("repository_proxy_password"))
                repository_proxy_password = Marshalling.ParseRef<Secret>(table, "repository_proxy_password");
            if (table.ContainsKey("migration_compression"))
                migration_compression = Marshalling.ParseBool(table, "migration_compression");
            if (table.ContainsKey("coordinator_bias"))
                coordinator_bias = Marshalling.ParseBool(table, "coordinator_bias");
            if (table.ContainsKey("local_auth_max_threads"))
                local_auth_max_threads = Marshalling.ParseLong(table, "local_auth_max_threads");
            if (table.ContainsKey("ext_auth_max_threads"))
                ext_auth_max_threads = Marshalling.ParseLong(table, "ext_auth_max_threads");
            if (table.ContainsKey("ext_auth_cache_enabled"))
                ext_auth_cache_enabled = Marshalling.ParseBool(table, "ext_auth_cache_enabled");
            if (table.ContainsKey("ext_auth_cache_size"))
                ext_auth_cache_size = Marshalling.ParseLong(table, "ext_auth_cache_size");
            if (table.ContainsKey("ext_auth_cache_expiry"))
                ext_auth_cache_expiry = Marshalling.ParseLong(table, "ext_auth_cache_expiry");
            if (table.ContainsKey("telemetry_uuid"))
                telemetry_uuid = Marshalling.ParseRef<Secret>(table, "telemetry_uuid");
            if (table.ContainsKey("telemetry_frequency"))
                telemetry_frequency = (telemetry_frequency)Helper.EnumParseDefault(typeof(telemetry_frequency), Marshalling.ParseString(table, "telemetry_frequency"));
            if (table.ContainsKey("telemetry_next_collection"))
                telemetry_next_collection = Marshalling.ParseDateTime(table, "telemetry_next_collection");
            if (table.ContainsKey("last_update_sync"))
                last_update_sync = Marshalling.ParseDateTime(table, "last_update_sync");
            if (table.ContainsKey("update_sync_frequency"))
                update_sync_frequency = (update_sync_frequency)Helper.EnumParseDefault(typeof(update_sync_frequency), Marshalling.ParseString(table, "update_sync_frequency"));
            if (table.ContainsKey("update_sync_day"))
                update_sync_day = Marshalling.ParseLong(table, "update_sync_day");
            if (table.ContainsKey("update_sync_enabled"))
                update_sync_enabled = Marshalling.ParseBool(table, "update_sync_enabled");
            if (table.ContainsKey("recommendations"))
                recommendations = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "recommendations"));
            if (table.ContainsKey("license_server"))
                license_server = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "license_server"));
            if (table.ContainsKey("ha_reboot_vm_on_internal_shutdown"))
                ha_reboot_vm_on_internal_shutdown = Marshalling.ParseBool(table, "ha_reboot_vm_on_internal_shutdown");
            if (table.ContainsKey("limit_console_sessions"))
                limit_console_sessions = Marshalling.ParseBool(table, "limit_console_sessions");
            if (table.ContainsKey("vm_console_idle_timeout"))
                vm_console_idle_timeout = Marshalling.ParseLong(table, "vm_console_idle_timeout");
            if (table.ContainsKey("auto_update_vm_secureboot_certificates"))
                auto_update_vm_secureboot_certificates = Marshalling.ParseBool(table, "auto_update_vm_secureboot_certificates");
        }

        public bool DeepEquals(Pool other, bool ignoreCurrentOperations)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (!ignoreCurrentOperations && !Helper.AreEqual2(current_operations, other.current_operations))
                return false;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name_label, other._name_label) &&
                Helper.AreEqual2(_name_description, other._name_description) &&
                Helper.AreEqual2(_master, other._master) &&
                Helper.AreEqual2(_default_SR, other._default_SR) &&
                Helper.AreEqual2(_suspend_image_SR, other._suspend_image_SR) &&
                Helper.AreEqual2(_crash_dump_SR, other._crash_dump_SR) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_ha_enabled, other._ha_enabled) &&
                Helper.AreEqual2(_ha_configuration, other._ha_configuration) &&
                Helper.AreEqual2(_ha_statefiles, other._ha_statefiles) &&
                Helper.AreEqual2(_ha_host_failures_to_tolerate, other._ha_host_failures_to_tolerate) &&
                Helper.AreEqual2(_ha_plan_exists_for, other._ha_plan_exists_for) &&
                Helper.AreEqual2(_ha_allow_overcommit, other._ha_allow_overcommit) &&
                Helper.AreEqual2(_ha_overcommitted, other._ha_overcommitted) &&
                Helper.AreEqual2(_blobs, other._blobs) &&
                Helper.AreEqual2(_tags, other._tags) &&
                Helper.AreEqual2(_gui_config, other._gui_config) &&
                Helper.AreEqual2(_health_check_config, other._health_check_config) &&
                Helper.AreEqual2(_wlb_url, other._wlb_url) &&
                Helper.AreEqual2(_wlb_username, other._wlb_username) &&
                Helper.AreEqual2(_wlb_enabled, other._wlb_enabled) &&
                Helper.AreEqual2(_wlb_verify_cert, other._wlb_verify_cert) &&
                Helper.AreEqual2(_redo_log_enabled, other._redo_log_enabled) &&
                Helper.AreEqual2(_redo_log_vdi, other._redo_log_vdi) &&
                Helper.AreEqual2(_vswitch_controller, other._vswitch_controller) &&
                Helper.AreEqual2(_restrictions, other._restrictions) &&
                Helper.AreEqual2(_metadata_VDIs, other._metadata_VDIs) &&
                Helper.AreEqual2(_ha_cluster_stack, other._ha_cluster_stack) &&
                Helper.AreEqual2(_allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(_guest_agent_config, other._guest_agent_config) &&
                Helper.AreEqual2(_cpu_info, other._cpu_info) &&
                Helper.AreEqual2(_policy_no_vendor_device, other._policy_no_vendor_device) &&
                Helper.AreEqual2(_live_patching_disabled, other._live_patching_disabled) &&
                Helper.AreEqual2(_igmp_snooping_enabled, other._igmp_snooping_enabled) &&
                Helper.AreEqual2(_uefi_certificates, other._uefi_certificates) &&
                Helper.AreEqual2(_custom_uefi_certificates, other._custom_uefi_certificates) &&
                Helper.AreEqual2(_is_psr_pending, other._is_psr_pending) &&
                Helper.AreEqual2(_tls_verification_enabled, other._tls_verification_enabled) &&
                Helper.AreEqual2(_repositories, other._repositories) &&
                Helper.AreEqual2(_client_certificate_auth_enabled, other._client_certificate_auth_enabled) &&
                Helper.AreEqual2(_client_certificate_auth_name, other._client_certificate_auth_name) &&
                Helper.AreEqual2(_repository_proxy_url, other._repository_proxy_url) &&
                Helper.AreEqual2(_repository_proxy_username, other._repository_proxy_username) &&
                Helper.AreEqual2(_repository_proxy_password, other._repository_proxy_password) &&
                Helper.AreEqual2(_migration_compression, other._migration_compression) &&
                Helper.AreEqual2(_coordinator_bias, other._coordinator_bias) &&
                Helper.AreEqual2(_local_auth_max_threads, other._local_auth_max_threads) &&
                Helper.AreEqual2(_ext_auth_max_threads, other._ext_auth_max_threads) &&
                Helper.AreEqual2(_ext_auth_cache_enabled, other._ext_auth_cache_enabled) &&
                Helper.AreEqual2(_ext_auth_cache_size, other._ext_auth_cache_size) &&
                Helper.AreEqual2(_ext_auth_cache_expiry, other._ext_auth_cache_expiry) &&
                Helper.AreEqual2(_telemetry_uuid, other._telemetry_uuid) &&
                Helper.AreEqual2(_telemetry_frequency, other._telemetry_frequency) &&
                Helper.AreEqual2(_telemetry_next_collection, other._telemetry_next_collection) &&
                Helper.AreEqual2(_last_update_sync, other._last_update_sync) &&
                Helper.AreEqual2(_update_sync_frequency, other._update_sync_frequency) &&
                Helper.AreEqual2(_update_sync_day, other._update_sync_day) &&
                Helper.AreEqual2(_update_sync_enabled, other._update_sync_enabled) &&
                Helper.AreEqual2(_recommendations, other._recommendations) &&
                Helper.AreEqual2(_license_server, other._license_server) &&
                Helper.AreEqual2(_ha_reboot_vm_on_internal_shutdown, other._ha_reboot_vm_on_internal_shutdown) &&
                Helper.AreEqual2(_limit_console_sessions, other._limit_console_sessions) &&
                Helper.AreEqual2(_vm_console_idle_timeout, other._vm_console_idle_timeout) &&
                Helper.AreEqual2(_auto_update_vm_secureboot_certificates, other._auto_update_vm_secureboot_certificates);
        }


        /// <summary>
        /// Get a record containing the current state of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Pool get_record(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_record(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get a reference to the pool instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Pool> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.pool_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_uuid(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the name_label field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_label(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_name_label(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the name_description field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_description(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_name_description(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the master field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host> get_master(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_master(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the default_SR field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<SR> get_default_SR(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_default_sr(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the suspend_image_SR field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<SR> get_suspend_image_SR(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_suspend_image_sr(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the crash_dump_SR field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<SR> get_crash_dump_SR(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_crash_dump_sr(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the other_config field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_other_config(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the ha_enabled field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_ha_enabled(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_ha_enabled(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the ha_configuration field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_ha_configuration(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_ha_configuration(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the ha_statefiles field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_ha_statefiles(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_ha_statefiles(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the ha_host_failures_to_tolerate field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_ha_host_failures_to_tolerate(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_ha_host_failures_to_tolerate(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the ha_plan_exists_for field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_ha_plan_exists_for(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_ha_plan_exists_for(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the ha_allow_overcommit field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_ha_allow_overcommit(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_ha_allow_overcommit(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the ha_overcommitted field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_ha_overcommitted(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_ha_overcommitted(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the blobs field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, XenRef<Blob>> get_blobs(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_blobs(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the tags field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_tags(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_tags(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the gui_config field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_gui_config(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_gui_config(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the health_check_config field of the given pool.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_health_check_config(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_health_check_config(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the wlb_url field of the given pool.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_wlb_url(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_wlb_url(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the wlb_username field of the given pool.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_wlb_username(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_wlb_username(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the wlb_enabled field of the given pool.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_wlb_enabled(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_wlb_enabled(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the wlb_verify_cert field of the given pool.
        /// First published in XenServer 5.5.
        /// Deprecated since 1.290.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        [Deprecated("1.290.0")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_wlb_verify_cert(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_wlb_verify_cert(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the redo_log_enabled field of the given pool.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_redo_log_enabled(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_redo_log_enabled(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the redo_log_vdi field of the given pool.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VDI> get_redo_log_vdi(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_redo_log_vdi(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the vswitch_controller field of the given pool.
        /// First published in XenServer 5.6.
        /// Deprecated since XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        [Deprecated("XenServer 7.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_vswitch_controller(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_vswitch_controller(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the restrictions field of the given pool.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_restrictions(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_restrictions(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the metadata_VDIs field of the given pool.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VDI>> get_metadata_VDIs(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_metadata_vdis(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the ha_cluster_stack field of the given pool.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_ha_cluster_stack(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_ha_cluster_stack(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the allowed_operations field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<pool_allowed_operations> get_allowed_operations(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_allowed_operations(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the current_operations field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, pool_allowed_operations> get_current_operations(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_current_operations(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the guest_agent_config field of the given pool.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_guest_agent_config(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_guest_agent_config(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the cpu_info field of the given pool.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_cpu_info(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_cpu_info(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the policy_no_vendor_device field of the given pool.
        /// First published in XenServer 7.0.
        /// Deprecated since 24.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        [Deprecated("24.14.0")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_policy_no_vendor_device(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_policy_no_vendor_device(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the live_patching_disabled field of the given pool.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_live_patching_disabled(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_live_patching_disabled(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the igmp_snooping_enabled field of the given pool.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_igmp_snooping_enabled(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_igmp_snooping_enabled(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the uefi_certificates field of the given pool.
        /// First published in Citrix Hypervisor 8.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uefi_certificates(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_uefi_certificates(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the custom_uefi_certificates field of the given pool.
        /// Experimental. First published in 24.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_custom_uefi_certificates(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_custom_uefi_certificates(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the is_psr_pending field of the given pool.
        /// First published in Citrix Hypervisor 8.2 Hotfix 2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_is_psr_pending(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_is_psr_pending(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the tls_verification_enabled field of the given pool.
        /// First published in 1.290.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_tls_verification_enabled(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_tls_verification_enabled(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the repositories field of the given pool.
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Repository>> get_repositories(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_repositories(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the client_certificate_auth_enabled field of the given pool.
        /// First published in 1.318.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_client_certificate_auth_enabled(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_client_certificate_auth_enabled(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the client_certificate_auth_name field of the given pool.
        /// First published in 1.318.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_client_certificate_auth_name(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_client_certificate_auth_name(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the repository_proxy_url field of the given pool.
        /// First published in 21.3.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_repository_proxy_url(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_repository_proxy_url(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the repository_proxy_username field of the given pool.
        /// First published in 21.3.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_repository_proxy_username(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_repository_proxy_username(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the repository_proxy_password field of the given pool.
        /// First published in 21.3.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Secret> get_repository_proxy_password(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_repository_proxy_password(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the migration_compression field of the given pool.
        /// Experimental. First published in 22.33.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_migration_compression(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_migration_compression(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the coordinator_bias field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_coordinator_bias(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_coordinator_bias(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the local_auth_max_threads field of the given pool.
        /// Experimental. First published in 23.27.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_local_auth_max_threads(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_local_auth_max_threads(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the ext_auth_max_threads field of the given pool.
        /// Experimental. First published in 23.27.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_ext_auth_max_threads(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_ext_auth_max_threads(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the ext_auth_cache_enabled field of the given pool.
        /// Experimental. First published in 24.31.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_ext_auth_cache_enabled(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_ext_auth_cache_enabled(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the ext_auth_cache_size field of the given pool.
        /// Experimental. First published in 24.31.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_ext_auth_cache_size(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_ext_auth_cache_size(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the ext_auth_cache_expiry field of the given pool.
        /// Experimental. First published in 24.31.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_ext_auth_cache_expiry(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_ext_auth_cache_expiry(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the telemetry_uuid field of the given pool.
        /// Experimental. First published in 23.9.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Secret> get_telemetry_uuid(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_telemetry_uuid(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the telemetry_frequency field of the given pool.
        /// Experimental. First published in 23.9.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static telemetry_frequency get_telemetry_frequency(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_telemetry_frequency(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the telemetry_next_collection field of the given pool.
        /// Experimental. First published in 23.9.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_telemetry_next_collection(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_telemetry_next_collection(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the last_update_sync field of the given pool.
        /// Experimental. First published in 23.18.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_last_update_sync(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_last_update_sync(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the update_sync_frequency field of the given pool.
        /// Experimental. First published in 23.18.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static update_sync_frequency get_update_sync_frequency(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_update_sync_frequency(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the update_sync_day field of the given pool.
        /// Experimental. First published in 23.18.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_update_sync_day(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_update_sync_day(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the update_sync_enabled field of the given pool.
        /// Experimental. First published in 23.18.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_update_sync_enabled(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_update_sync_enabled(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the recommendations field of the given pool.
        /// Experimental. First published in 24.19.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_recommendations(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_recommendations(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the license_server field of the given pool.
        /// Experimental. First published in 25.6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_license_server(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_license_server(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the ha_reboot_vm_on_internal_shutdown field of the given pool.
        /// Experimental. First published in 25.16.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_ha_reboot_vm_on_internal_shutdown(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_ha_reboot_vm_on_internal_shutdown(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the limit_console_sessions field of the given pool.
        /// Experimental. First published in 26.1.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_limit_console_sessions(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_limit_console_sessions(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the vm_console_idle_timeout field of the given pool.
        /// Experimental. First published in 26.1.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_vm_console_idle_timeout(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_vm_console_idle_timeout(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Get the auto_update_vm_secureboot_certificates field of the given pool.
        /// Experimental. First published in 26.16.0-next.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_auto_update_vm_secureboot_certificates(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_auto_update_vm_secureboot_certificates(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Set the name_label field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name_label">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_name_label(Session session, string _pool, string _name_label)
        {
            session.JsonRpcClient.pool_set_name_label(session.opaque_ref, _pool, _name_label);
        }

        /// <summary>
        /// Set the name_description field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name_description">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_name_description(Session session, string _pool, string _name_description)
        {
            session.JsonRpcClient.pool_set_name_description(session.opaque_ref, _pool, _name_description);
        }

        /// <summary>
        /// Set the default_SR field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_default_sr">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_default_SR(Session session, string _pool, string _default_sr)
        {
            session.JsonRpcClient.pool_set_default_sr(session.opaque_ref, _pool, _default_sr);
        }

        /// <summary>
        /// Set the suspend_image_SR field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_suspend_image_sr">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_suspend_image_SR(Session session, string _pool, string _suspend_image_sr)
        {
            session.JsonRpcClient.pool_set_suspend_image_sr(session.opaque_ref, _pool, _suspend_image_sr);
        }

        /// <summary>
        /// Set the crash_dump_SR field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_crash_dump_sr">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_crash_dump_SR(Session session, string _pool, string _crash_dump_sr)
        {
            session.JsonRpcClient.pool_set_crash_dump_sr(session.opaque_ref, _pool, _crash_dump_sr);
        }

        /// <summary>
        /// Set the other_config field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_other_config(Session session, string _pool, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.pool_set_other_config(session.opaque_ref, _pool, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_other_config(Session session, string _pool, string _key, string _value)
        {
            session.JsonRpcClient.pool_add_to_other_config(session.opaque_ref, _pool, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given pool.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_other_config(Session session, string _pool, string _key)
        {
            session.JsonRpcClient.pool_remove_from_other_config(session.opaque_ref, _pool, _key);
        }

        /// <summary>
        /// Set the ha_allow_overcommit field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_ha_allow_overcommit">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_ha_allow_overcommit(Session session, string _pool, bool _ha_allow_overcommit)
        {
            session.JsonRpcClient.pool_set_ha_allow_overcommit(session.opaque_ref, _pool, _ha_allow_overcommit);
        }

        /// <summary>
        /// Set the tags field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_tags">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void set_tags(Session session, string _pool, string[] _tags)
        {
            session.JsonRpcClient.pool_set_tags(session.opaque_ref, _pool, _tags);
        }

        /// <summary>
        /// Add the given value to the tags field of the given pool.  If the value is already in that Set, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">New value to add</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void add_tags(Session session, string _pool, string _value)
        {
            session.JsonRpcClient.pool_add_tags(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Remove the given value from the tags field of the given pool.  If the value is not in that Set, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">Value to remove</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void remove_tags(Session session, string _pool, string _value)
        {
            session.JsonRpcClient.pool_remove_tags(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Set the gui_config field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_gui_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void set_gui_config(Session session, string _pool, Dictionary<string, string> _gui_config)
        {
            session.JsonRpcClient.pool_set_gui_config(session.opaque_ref, _pool, _gui_config);
        }

        /// <summary>
        /// Add the given key-value pair to the gui_config field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void add_to_gui_config(Session session, string _pool, string _key, string _value)
        {
            session.JsonRpcClient.pool_add_to_gui_config(session.opaque_ref, _pool, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the gui_config field of the given pool.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void remove_from_gui_config(Session session, string _pool, string _key)
        {
            session.JsonRpcClient.pool_remove_from_gui_config(session.opaque_ref, _pool, _key);
        }

        /// <summary>
        /// Set the health_check_config field of the given pool.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_health_check_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_health_check_config(Session session, string _pool, Dictionary<string, string> _health_check_config)
        {
            session.JsonRpcClient.pool_set_health_check_config(session.opaque_ref, _pool, _health_check_config);
        }

        /// <summary>
        /// Add the given key-value pair to the health_check_config field of the given pool.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_health_check_config(Session session, string _pool, string _key, string _value)
        {
            session.JsonRpcClient.pool_add_to_health_check_config(session.opaque_ref, _pool, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the health_check_config field of the given pool.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_health_check_config(Session session, string _pool, string _key)
        {
            session.JsonRpcClient.pool_remove_from_health_check_config(session.opaque_ref, _pool, _key);
        }

        /// <summary>
        /// Set the wlb_enabled field of the given pool.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_wlb_enabled">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_wlb_enabled(Session session, string _pool, bool _wlb_enabled)
        {
            session.JsonRpcClient.pool_set_wlb_enabled(session.opaque_ref, _pool, _wlb_enabled);
        }

        /// <summary>
        /// Set the wlb_verify_cert field of the given pool.
        /// First published in XenServer 5.5.
        /// Deprecated since 1.290.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_wlb_verify_cert">New value to set</param>
        [Deprecated("1.290.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_wlb_verify_cert(Session session, string _pool, bool _wlb_verify_cert)
        {
            session.JsonRpcClient.pool_set_wlb_verify_cert(session.opaque_ref, _pool, _wlb_verify_cert);
        }

        /// <summary>
        /// Set the policy_no_vendor_device field of the given pool.
        /// First published in XenServer 7.0.
        /// Deprecated since 24.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_policy_no_vendor_device">New value to set</param>
        [Deprecated("24.14.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_policy_no_vendor_device(Session session, string _pool, bool _policy_no_vendor_device)
        {
            session.JsonRpcClient.pool_set_policy_no_vendor_device(session.opaque_ref, _pool, _policy_no_vendor_device);
        }

        /// <summary>
        /// Set the live_patching_disabled field of the given pool.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_live_patching_disabled">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_live_patching_disabled(Session session, string _pool, bool _live_patching_disabled)
        {
            session.JsonRpcClient.pool_set_live_patching_disabled(session.opaque_ref, _pool, _live_patching_disabled);
        }

        /// <summary>
        /// Set the is_psr_pending field of the given pool.
        /// First published in Citrix Hypervisor 8.2 Hotfix 2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_is_psr_pending">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_is_psr_pending(Session session, string _pool, bool _is_psr_pending)
        {
            session.JsonRpcClient.pool_set_is_psr_pending(session.opaque_ref, _pool, _is_psr_pending);
        }

        /// <summary>
        /// Set the migration_compression field of the given pool.
        /// Experimental. First published in 22.33.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_migration_compression">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_migration_compression(Session session, string _pool, bool _migration_compression)
        {
            session.JsonRpcClient.pool_set_migration_compression(session.opaque_ref, _pool, _migration_compression);
        }

        /// <summary>
        /// Set the coordinator_bias field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_coordinator_bias">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_coordinator_bias(Session session, string _pool, bool _coordinator_bias)
        {
            session.JsonRpcClient.pool_set_coordinator_bias(session.opaque_ref, _pool, _coordinator_bias);
        }

        /// <summary>
        /// Set the license_server field of the given pool.
        /// Experimental. First published in 25.6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_license_server">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_license_server(Session session, string _pool, Dictionary<string, string> _license_server)
        {
            session.JsonRpcClient.pool_set_license_server(session.opaque_ref, _pool, _license_server);
        }

        /// <summary>
        /// Add the given key-value pair to the license_server field of the given pool.
        /// Experimental. First published in 25.6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_license_server(Session session, string _pool, string _key, string _value)
        {
            session.JsonRpcClient.pool_add_to_license_server(session.opaque_ref, _pool, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the license_server field of the given pool.  If the key is not in that Map, then do nothing.
        /// Experimental. First published in 25.6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_license_server(Session session, string _pool, string _key)
        {
            session.JsonRpcClient.pool_remove_from_license_server(session.opaque_ref, _pool, _key);
        }

        /// <summary>
        /// Set the ha_reboot_vm_on_internal_shutdown field of the given pool.
        /// Experimental. First published in 25.16.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_ha_reboot_vm_on_internal_shutdown">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_ha_reboot_vm_on_internal_shutdown(Session session, string _pool, bool _ha_reboot_vm_on_internal_shutdown)
        {
            session.JsonRpcClient.pool_set_ha_reboot_vm_on_internal_shutdown(session.opaque_ref, _pool, _ha_reboot_vm_on_internal_shutdown);
        }

        /// <summary>
        /// Set the limit_console_sessions field of the given pool.
        /// Experimental. First published in 26.1.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_limit_console_sessions">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_limit_console_sessions(Session session, string _pool, bool _limit_console_sessions)
        {
            session.JsonRpcClient.pool_set_limit_console_sessions(session.opaque_ref, _pool, _limit_console_sessions);
        }

        /// <summary>
        /// Set the vm_console_idle_timeout field of the given pool.
        /// Experimental. First published in 26.1.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_vm_console_idle_timeout">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_vm_console_idle_timeout(Session session, string _pool, long _vm_console_idle_timeout)
        {
            session.JsonRpcClient.pool_set_vm_console_idle_timeout(session.opaque_ref, _pool, _vm_console_idle_timeout);
        }

        /// <summary>
        /// Set the auto_update_vm_secureboot_certificates field of the given pool.
        /// Experimental. First published in 26.16.0-next.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_auto_update_vm_secureboot_certificates">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_auto_update_vm_secureboot_certificates(Session session, string _pool, bool _auto_update_vm_secureboot_certificates)
        {
            session.JsonRpcClient.pool_set_auto_update_vm_secureboot_certificates(session.opaque_ref, _pool, _auto_update_vm_secureboot_certificates);
        }

        /// <summary>
        /// Instruct host to join a new pool
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_master_address">The hostname of the master of the pool to join</param>
        /// <param name="_master_username">The username of the master (for initial authentication)</param>
        /// <param name="_master_password">The password for the master (for initial authentication)</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void join(Session session, string _master_address, string _master_username, string _master_password)
        {
            session.JsonRpcClient.pool_join(session.opaque_ref, _master_address, _master_username, _master_password);
        }

        /// <summary>
        /// Instruct host to join a new pool
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_master_address">The hostname of the master of the pool to join</param>
        /// <param name="_master_username">The username of the master (for initial authentication)</param>
        /// <param name="_master_password">The password for the master (for initial authentication)</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_join(Session session, string _master_address, string _master_username, string _master_password)
        {
          return session.JsonRpcClient.async_pool_join(session.opaque_ref, _master_address, _master_username, _master_password);
        }

        /// <summary>
        /// Instruct host to join a new pool
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_master_address">The hostname of the master of the pool to join</param>
        /// <param name="_master_username">The username of the master (for initial authentication)</param>
        /// <param name="_master_password">The password for the master (for initial authentication)</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void join_force(Session session, string _master_address, string _master_username, string _master_password)
        {
            session.JsonRpcClient.pool_join_force(session.opaque_ref, _master_address, _master_username, _master_password);
        }

        /// <summary>
        /// Instruct host to join a new pool
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_master_address">The hostname of the master of the pool to join</param>
        /// <param name="_master_username">The username of the master (for initial authentication)</param>
        /// <param name="_master_password">The password for the master (for initial authentication)</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_join_force(Session session, string _master_address, string _master_username, string _master_password)
        {
          return session.JsonRpcClient.async_pool_join_force(session.opaque_ref, _master_address, _master_username, _master_password);
        }

        /// <summary>
        /// Instruct a pool master to eject a host from the pool
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host to eject</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void eject(Session session, string _host)
        {
            session.JsonRpcClient.pool_eject(session.opaque_ref, _host);
        }

        /// <summary>
        /// Instruct a pool master to eject a host from the pool
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host to eject</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_eject(Session session, string _host)
        {
          return session.JsonRpcClient.async_pool_eject(session.opaque_ref, _host);
        }

        /// <summary>
        /// Instruct host that's currently a slave to transition to being master
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void emergency_transition_to_master(Session session)
        {
            session.JsonRpcClient.pool_emergency_transition_to_master(session.opaque_ref);
        }

        /// <summary>
        /// Instruct a slave already in a pool that the master has changed
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_master_address">The hostname of the master</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void emergency_reset_master(Session session, string _master_address)
        {
            session.JsonRpcClient.pool_emergency_reset_master(session.opaque_ref, _master_address);
        }

        /// <summary>
        /// Instruct a pool master, M, to try and contact its slaves and, if slaves are in emergency mode, reset their master address to M.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static List<XenRef<Host>> recover_slaves(Session session)
        {
            return session.JsonRpcClient.pool_recover_slaves(session.opaque_ref);
        }

        /// <summary>
        /// Instruct a pool master, M, to try and contact its slaves and, if slaves are in emergency mode, reset their master address to M.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_recover_slaves(Session session)
        {
          return session.JsonRpcClient.async_pool_recover_slaves(session.opaque_ref);
        }

        /// <summary>
        /// Create PIFs, mapping a network to the same physical interface/VLAN on each host. This call is deprecated: use Pool.create_VLAN_from_PIF instead.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device">physical interface on which to create the VLAN interface</param>
        /// <param name="_network">network to which this interface should be connected</param>
        /// <param name="_vlan">VLAN tag for the new interface</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static List<XenRef<PIF>> create_VLAN(Session session, string _device, string _network, long _vlan)
        {
            return session.JsonRpcClient.pool_create_vlan(session.opaque_ref, _device, _network, _vlan);
        }

        /// <summary>
        /// Create PIFs, mapping a network to the same physical interface/VLAN on each host. This call is deprecated: use Pool.create_VLAN_from_PIF instead.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device">physical interface on which to create the VLAN interface</param>
        /// <param name="_network">network to which this interface should be connected</param>
        /// <param name="_vlan">VLAN tag for the new interface</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create_VLAN(Session session, string _device, string _network, long _vlan)
        {
          return session.JsonRpcClient.async_pool_create_vlan(session.opaque_ref, _device, _network, _vlan);
        }

        /// <summary>
        /// Reconfigure the management network interface for all Hosts in the Pool
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The network</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void management_reconfigure(Session session, string _network)
        {
            session.JsonRpcClient.pool_management_reconfigure(session.opaque_ref, _network);
        }

        /// <summary>
        /// Reconfigure the management network interface for all Hosts in the Pool
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">The network</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_management_reconfigure(Session session, string _network)
        {
          return session.JsonRpcClient.async_pool_management_reconfigure(session.opaque_ref, _network);
        }

        /// <summary>
        /// Create a pool-wide VLAN by taking the PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">physical interface on any particular host, that identifies the PIF on which to create the (pool-wide) VLAN interface</param>
        /// <param name="_network">network to which this interface should be connected</param>
        /// <param name="_vlan">VLAN tag for the new interface</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static List<XenRef<PIF>> create_VLAN_from_PIF(Session session, string _pif, string _network, long _vlan)
        {
            return session.JsonRpcClient.pool_create_vlan_from_pif(session.opaque_ref, _pif, _network, _vlan);
        }

        /// <summary>
        /// Create a pool-wide VLAN by taking the PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">physical interface on any particular host, that identifies the PIF on which to create the (pool-wide) VLAN interface</param>
        /// <param name="_network">network to which this interface should be connected</param>
        /// <param name="_vlan">VLAN tag for the new interface</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create_VLAN_from_PIF(Session session, string _pif, string _network, long _vlan)
        {
          return session.JsonRpcClient.async_pool_create_vlan_from_pif(session.opaque_ref, _pif, _network, _vlan);
        }

        /// <summary>
        /// Turn on High Availability mode
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_heartbeat_srs">Set of SRs to use for storage heartbeating</param>
        /// <param name="_configuration">Detailed HA configuration to apply</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void enable_ha(Session session, List<XenRef<SR>> _heartbeat_srs, Dictionary<string, string> _configuration)
        {
            session.JsonRpcClient.pool_enable_ha(session.opaque_ref, _heartbeat_srs, _configuration);
        }

        /// <summary>
        /// Turn on High Availability mode
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_heartbeat_srs">Set of SRs to use for storage heartbeating</param>
        /// <param name="_configuration">Detailed HA configuration to apply</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_enable_ha(Session session, List<XenRef<SR>> _heartbeat_srs, Dictionary<string, string> _configuration)
        {
          return session.JsonRpcClient.async_pool_enable_ha(session.opaque_ref, _heartbeat_srs, _configuration);
        }

        /// <summary>
        /// Turn off High Availability mode
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void disable_ha(Session session)
        {
            session.JsonRpcClient.pool_disable_ha(session.opaque_ref);
        }

        /// <summary>
        /// Turn off High Availability mode
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_disable_ha(Session session)
        {
          return session.JsonRpcClient.async_pool_disable_ha(session.opaque_ref);
        }

        /// <summary>
        /// Forcibly synchronise the database now
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void sync_database(Session session)
        {
            session.JsonRpcClient.pool_sync_database(session.opaque_ref);
        }

        /// <summary>
        /// Forcibly synchronise the database now
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_sync_database(Session session)
        {
          return session.JsonRpcClient.async_pool_sync_database(session.opaque_ref);
        }

        /// <summary>
        /// Perform an orderly handover of the role of master to the referenced host.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host who should become the new master</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void designate_new_master(Session session, string _host)
        {
            session.JsonRpcClient.pool_designate_new_master(session.opaque_ref, _host);
        }

        /// <summary>
        /// Perform an orderly handover of the role of master to the referenced host.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host who should become the new master</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_designate_new_master(Session session, string _host)
        {
          return session.JsonRpcClient.async_pool_designate_new_master(session.opaque_ref, _host);
        }

        /// <summary>
        /// When this call returns the VM restart logic will not run for the requested number of seconds. If the argument is zero then the restart thread is immediately unblocked
        /// First published in XenServer 5.0 Update 1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_seconds">The number of seconds to block the restart thread for</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void ha_prevent_restarts_for(Session session, long _seconds)
        {
            session.JsonRpcClient.pool_ha_prevent_restarts_for(session.opaque_ref, _seconds);
        }

        /// <summary>
        /// Returns true if a VM failover plan exists for up to 'n' host failures
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_n">The number of host failures to plan for</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static bool ha_failover_plan_exists(Session session, long _n)
        {
            return session.JsonRpcClient.pool_ha_failover_plan_exists(session.opaque_ref, _n);
        }

        /// <summary>
        /// Returns the maximum number of host failures we could tolerate before we would be unable to restart configured VMs
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static long ha_compute_max_host_failures_to_tolerate(Session session)
        {
            return session.JsonRpcClient.pool_ha_compute_max_host_failures_to_tolerate(session.opaque_ref);
        }

        /// <summary>
        /// Returns the maximum number of host failures we could tolerate before we would be unable to restart the provided VMs
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_configuration">Map of protected VM reference to restart priority</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long ha_compute_hypothetical_max_host_failures_to_tolerate(Session session, Dictionary<XenRef<VM>, string> _configuration)
        {
            return session.JsonRpcClient.pool_ha_compute_hypothetical_max_host_failures_to_tolerate(session.opaque_ref, _configuration);
        }

        /// <summary>
        /// Return a VM failover plan assuming a given subset of hosts fail
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_failed_hosts">The set of hosts to assume have failed</param>
        /// <param name="_failed_vms">The set of VMs to restart</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static Dictionary<XenRef<VM>, Dictionary<string, string>> ha_compute_vm_failover_plan(Session session, List<XenRef<Host>> _failed_hosts, List<XenRef<VM>> _failed_vms)
        {
            return session.JsonRpcClient.pool_ha_compute_vm_failover_plan(session.opaque_ref, _failed_hosts, _failed_vms);
        }

        /// <summary>
        /// Set the maximum number of host failures to consider in the HA VM restart planner
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">New number of host failures to consider</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_ha_host_failures_to_tolerate(Session session, string _pool, long _value)
        {
            session.JsonRpcClient.pool_set_ha_host_failures_to_tolerate(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Set the maximum number of host failures to consider in the HA VM restart planner
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">New number of host failures to consider</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_ha_host_failures_to_tolerate(Session session, string _pool, long _value)
        {
          return session.JsonRpcClient.async_pool_set_ha_host_failures_to_tolerate(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this pool
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Blob> create_new_blob(Session session, string _pool, string _name, string _mime_type)
        {
            return session.JsonRpcClient.pool_create_new_blob(session.opaque_ref, _pool, _name, _mime_type);
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this pool
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create_new_blob(Session session, string _pool, string _name, string _mime_type)
        {
          return session.JsonRpcClient.async_pool_create_new_blob(session.opaque_ref, _pool, _name, _mime_type);
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this pool
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <param name="_public">True if the blob should be publicly available First published in XenServer 6.1.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Blob> create_new_blob(Session session, string _pool, string _name, string _mime_type, bool _public)
        {
            return session.JsonRpcClient.pool_create_new_blob(session.opaque_ref, _pool, _name, _mime_type, _public);
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this pool
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <param name="_public">True if the blob should be publicly available First published in XenServer 6.1.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create_new_blob(Session session, string _pool, string _name, string _mime_type, bool _public)
        {
          return session.JsonRpcClient.async_pool_create_new_blob(session.opaque_ref, _pool, _name, _mime_type, _public);
        }

        /// <summary>
        /// This call enables external authentication on all the hosts of the pool
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_config">A list of key-values containing the configuration data</param>
        /// <param name="_service_name">The name of the service</param>
        /// <param name="_auth_type">The type of authentication (for example, AD for Active Directory)</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void enable_external_auth(Session session, string _pool, Dictionary<string, string> _config, string _service_name, string _auth_type)
        {
            session.JsonRpcClient.pool_enable_external_auth(session.opaque_ref, _pool, _config, _service_name, _auth_type);
        }

        /// <summary>
        /// This call disables external authentication on all the hosts of the pool
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_config">Optional parameters as a list of key-values containing the configuration data</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void disable_external_auth(Session session, string _pool, Dictionary<string, string> _config)
        {
            session.JsonRpcClient.pool_disable_external_auth(session.opaque_ref, _pool, _config);
        }

        /// <summary>
        /// This call enables or disables LDAPS for external authentication on all hosts in the pool
        /// First published in 26.16.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_ldaps">Whether to enable or disable LDAPS</param>
        /// <param name="_force">Force the operation even if already in the desired state</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void external_auth_set_ldaps(Session session, string _pool, bool _ldaps, bool _force)
        {
            session.JsonRpcClient.pool_external_auth_set_ldaps(session.opaque_ref, _pool, _ldaps, _force);
        }

        /// <summary>
        /// This call asynchronously detects if the external authentication configuration in any slave is different from that in the master and raises appropriate alerts
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void detect_nonhomogeneous_external_auth(Session session, string _pool)
        {
            session.JsonRpcClient.pool_detect_nonhomogeneous_external_auth(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Initializes workload balancing monitoring on this pool with the specified wlb server
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_wlb_url">The ip address and port to use when accessing the wlb server</param>
        /// <param name="_wlb_username">The username used to authenticate with the wlb server</param>
        /// <param name="_wlb_password">The password used to authenticate with the wlb server</param>
        /// <param name="_xenserver_username">The username used by the wlb server to authenticate with the xenserver</param>
        /// <param name="_xenserver_password">The password used by the wlb server to authenticate with the xenserver</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void initialize_wlb(Session session, string _wlb_url, string _wlb_username, string _wlb_password, string _xenserver_username, string _xenserver_password)
        {
            session.JsonRpcClient.pool_initialize_wlb(session.opaque_ref, _wlb_url, _wlb_username, _wlb_password, _xenserver_username, _xenserver_password);
        }

        /// <summary>
        /// Initializes workload balancing monitoring on this pool with the specified wlb server
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_wlb_url">The ip address and port to use when accessing the wlb server</param>
        /// <param name="_wlb_username">The username used to authenticate with the wlb server</param>
        /// <param name="_wlb_password">The password used to authenticate with the wlb server</param>
        /// <param name="_xenserver_username">The username used by the wlb server to authenticate with the xenserver</param>
        /// <param name="_xenserver_password">The password used by the wlb server to authenticate with the xenserver</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_initialize_wlb(Session session, string _wlb_url, string _wlb_username, string _wlb_password, string _xenserver_username, string _xenserver_password)
        {
          return session.JsonRpcClient.async_pool_initialize_wlb(session.opaque_ref, _wlb_url, _wlb_username, _wlb_password, _xenserver_username, _xenserver_password);
        }

        /// <summary>
        /// Permanently deconfigures workload balancing monitoring on this pool
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void deconfigure_wlb(Session session)
        {
            session.JsonRpcClient.pool_deconfigure_wlb(session.opaque_ref);
        }

        /// <summary>
        /// Permanently deconfigures workload balancing monitoring on this pool
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_deconfigure_wlb(Session session)
        {
          return session.JsonRpcClient.async_pool_deconfigure_wlb(session.opaque_ref);
        }

        /// <summary>
        /// Sets the pool optimization criteria for the workload balancing server
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_config">The configuration to use in optimizing this pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void send_wlb_configuration(Session session, Dictionary<string, string> _config)
        {
            session.JsonRpcClient.pool_send_wlb_configuration(session.opaque_ref, _config);
        }

        /// <summary>
        /// Sets the pool optimization criteria for the workload balancing server
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_config">The configuration to use in optimizing this pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_send_wlb_configuration(Session session, Dictionary<string, string> _config)
        {
          return session.JsonRpcClient.async_pool_send_wlb_configuration(session.opaque_ref, _config);
        }

        /// <summary>
        /// Retrieves the pool optimization criteria from the workload balancing server
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> retrieve_wlb_configuration(Session session)
        {
            return session.JsonRpcClient.pool_retrieve_wlb_configuration(session.opaque_ref);
        }

        /// <summary>
        /// Retrieves the pool optimization criteria from the workload balancing server
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_retrieve_wlb_configuration(Session session)
        {
          return session.JsonRpcClient.async_pool_retrieve_wlb_configuration(session.opaque_ref);
        }

        /// <summary>
        /// Retrieves vm migrate recommendations for the pool from the workload balancing server
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<VM>, string[]> retrieve_wlb_recommendations(Session session)
        {
            return session.JsonRpcClient.pool_retrieve_wlb_recommendations(session.opaque_ref);
        }

        /// <summary>
        /// Retrieves vm migrate recommendations for the pool from the workload balancing server
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_retrieve_wlb_recommendations(Session session)
        {
          return session.JsonRpcClient.async_pool_retrieve_wlb_recommendations(session.opaque_ref);
        }

        /// <summary>
        /// Send the given body to the given host and port, using HTTPS, and print the response.  This is used for debugging the SSL layer.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host"></param>
        /// <param name="_port"></param>
        /// <param name="_body"></param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static string send_test_post(Session session, string _host, long _port, string _body)
        {
            return session.JsonRpcClient.pool_send_test_post(session.opaque_ref, _host, _port, _body);
        }

        /// <summary>
        /// Send the given body to the given host and port, using HTTPS, and print the response.  This is used for debugging the SSL layer.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host"></param>
        /// <param name="_port"></param>
        /// <param name="_body"></param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_send_test_post(Session session, string _host, long _port, string _body)
        {
          return session.JsonRpcClient.async_pool_send_test_post(session.opaque_ref, _host, _port, _body);
        }

        /// <summary>
        /// Install a TLS CA certificate, pool-wide.
        /// First published in XenServer 5.5.
        /// Deprecated since 1.290.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">A name to give the certificate</param>
        /// <param name="_cert">The certificate in PEM format</param>
        [Deprecated("1.290.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void certificate_install(Session session, string _name, string _cert)
        {
            session.JsonRpcClient.pool_certificate_install(session.opaque_ref, _name, _cert);
        }

        /// <summary>
        /// Install a TLS CA certificate, pool-wide.
        /// First published in XenServer 5.5.
        /// Deprecated since 1.290.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">A name to give the certificate</param>
        /// <param name="_cert">The certificate in PEM format</param>
        [Deprecated("1.290.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_certificate_install(Session session, string _name, string _cert)
        {
          return session.JsonRpcClient.async_pool_certificate_install(session.opaque_ref, _name, _cert);
        }

        /// <summary>
        /// Remove a pool-wide TLS CA certificate.
        /// First published in XenServer 5.5.
        /// Deprecated since 1.290.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">The certificate name</param>
        [Deprecated("1.290.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void certificate_uninstall(Session session, string _name)
        {
            session.JsonRpcClient.pool_certificate_uninstall(session.opaque_ref, _name);
        }

        /// <summary>
        /// Remove a pool-wide TLS CA certificate.
        /// First published in XenServer 5.5.
        /// Deprecated since 1.290.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">The certificate name</param>
        [Deprecated("1.290.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_certificate_uninstall(Session session, string _name)
        {
          return session.JsonRpcClient.async_pool_certificate_uninstall(session.opaque_ref, _name);
        }

        /// <summary>
        /// List the names of all installed TLS CA certificates.
        /// First published in XenServer 5.5.
        /// Deprecated since 1.290.0.
        /// </summary>
        /// <param name="session">The session</param>
        [Deprecated("1.290.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static string[] certificate_list(Session session)
        {
            return session.JsonRpcClient.pool_certificate_list(session.opaque_ref);
        }

        /// <summary>
        /// List the names of all installed TLS CA certificates.
        /// First published in XenServer 5.5.
        /// Deprecated since 1.290.0.
        /// </summary>
        /// <param name="session">The session</param>
        [Deprecated("1.290.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_certificate_list(Session session)
        {
          return session.JsonRpcClient.async_pool_certificate_list(session.opaque_ref);
        }

        /// <summary>
        /// Install a TLS CA certificate, pool-wide.
        /// First published in 1.290.0.
        /// Deprecated since 26.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">A name to give the certificate</param>
        /// <param name="_cert">The certificate in PEM format</param>
        [Deprecated("26.13.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void install_ca_certificate(Session session, string _name, string _cert)
        {
            session.JsonRpcClient.pool_install_ca_certificate(session.opaque_ref, _name, _cert);
        }

        /// <summary>
        /// Install a TLS CA certificate, pool-wide.
        /// First published in 1.290.0.
        /// Deprecated since 26.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">A name to give the certificate</param>
        /// <param name="_cert">The certificate in PEM format</param>
        [Deprecated("26.13.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_install_ca_certificate(Session session, string _name, string _cert)
        {
          return session.JsonRpcClient.async_pool_install_ca_certificate(session.opaque_ref, _name, _cert);
        }

        /// <summary>
        /// Remove a pool-wide TLS CA certificate.
        /// First published in 1.290.0.
        /// Deprecated since 26.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">The certificate name</param>
        [Deprecated("26.13.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void uninstall_ca_certificate(Session session, string _name)
        {
            session.JsonRpcClient.pool_uninstall_ca_certificate(session.opaque_ref, _name);
        }

        /// <summary>
        /// Remove a pool-wide TLS CA certificate.
        /// First published in 1.290.0.
        /// Deprecated since 26.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">The certificate name</param>
        [Deprecated("26.13.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_uninstall_ca_certificate(Session session, string _name)
        {
          return session.JsonRpcClient.async_pool_uninstall_ca_certificate(session.opaque_ref, _name);
        }

        /// <summary>
        /// Remove a pool-wide TLS CA certificate.
        /// First published in 1.290.0.
        /// Deprecated since 26.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">The certificate name</param>
        /// <param name="_force">Remove the DB entry even if the file is non-existent First published in 24.35.0.</param>
        [Deprecated("26.13.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void uninstall_ca_certificate(Session session, string _name, bool _force)
        {
            session.JsonRpcClient.pool_uninstall_ca_certificate(session.opaque_ref, _name, _force);
        }

        /// <summary>
        /// Remove a pool-wide TLS CA certificate.
        /// First published in 1.290.0.
        /// Deprecated since 26.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">The certificate name</param>
        /// <param name="_force">Remove the DB entry even if the file is non-existent First published in 24.35.0.</param>
        [Deprecated("26.13.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_uninstall_ca_certificate(Session session, string _name, bool _force)
        {
          return session.JsonRpcClient.async_pool_uninstall_ca_certificate(session.opaque_ref, _name, _force);
        }

        /// <summary>
        /// Install a TLS CA-issued Certificate Revocation List, pool-wide.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">A name to give the CRL</param>
        /// <param name="_cert">The CRL</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void crl_install(Session session, string _name, string _cert)
        {
            session.JsonRpcClient.pool_crl_install(session.opaque_ref, _name, _cert);
        }

        /// <summary>
        /// Install a TLS CA-issued Certificate Revocation List, pool-wide.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">A name to give the CRL</param>
        /// <param name="_cert">The CRL</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_crl_install(Session session, string _name, string _cert)
        {
          return session.JsonRpcClient.async_pool_crl_install(session.opaque_ref, _name, _cert);
        }

        /// <summary>
        /// Remove a pool-wide TLS CA-issued Certificate Revocation List.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">The CRL name</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void crl_uninstall(Session session, string _name)
        {
            session.JsonRpcClient.pool_crl_uninstall(session.opaque_ref, _name);
        }

        /// <summary>
        /// Remove a pool-wide TLS CA-issued Certificate Revocation List.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">The CRL name</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_crl_uninstall(Session session, string _name)
        {
          return session.JsonRpcClient.async_pool_crl_uninstall(session.opaque_ref, _name);
        }

        /// <summary>
        /// List the names of all installed TLS CA-issued Certificate Revocation Lists.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static string[] crl_list(Session session)
        {
            return session.JsonRpcClient.pool_crl_list(session.opaque_ref);
        }

        /// <summary>
        /// List the names of all installed TLS CA-issued Certificate Revocation Lists.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_crl_list(Session session)
        {
          return session.JsonRpcClient.async_pool_crl_list(session.opaque_ref);
        }

        /// <summary>
        /// Copy the TLS CA certificates and CRLs of the master to all slaves.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void certificate_sync(Session session)
        {
            session.JsonRpcClient.pool_certificate_sync(session.opaque_ref);
        }

        /// <summary>
        /// Copy the TLS CA certificates and CRLs of the master to all slaves.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_certificate_sync(Session session)
        {
          return session.JsonRpcClient.async_pool_certificate_sync(session.opaque_ref);
        }

        /// <summary>
        /// Enable TLS server certificate verification
        /// First published in 1.290.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void enable_tls_verification(Session session)
        {
            session.JsonRpcClient.pool_enable_tls_verification(session.opaque_ref);
        }

        /// <summary>
        /// Enable the redo log on the given SR and start using it, unless HA is enabled.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sr">SR to hold the redo log.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void enable_redo_log(Session session, string _sr)
        {
            session.JsonRpcClient.pool_enable_redo_log(session.opaque_ref, _sr);
        }

        /// <summary>
        /// Enable the redo log on the given SR and start using it, unless HA is enabled.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sr">SR to hold the redo log.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_enable_redo_log(Session session, string _sr)
        {
          return session.JsonRpcClient.async_pool_enable_redo_log(session.opaque_ref, _sr);
        }

        /// <summary>
        /// Disable the redo log if in use, unless HA is enabled.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void disable_redo_log(Session session)
        {
            session.JsonRpcClient.pool_disable_redo_log(session.opaque_ref);
        }

        /// <summary>
        /// Disable the redo log if in use, unless HA is enabled.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_disable_redo_log(Session session)
        {
          return session.JsonRpcClient.async_pool_disable_redo_log(session.opaque_ref);
        }

        /// <summary>
        /// Set the IP address of the vswitch controller.
        /// First published in XenServer 5.6.
        /// Deprecated since XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_address">IP address of the vswitch controller.</param>
        [Deprecated("XenServer 7.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_vswitch_controller(Session session, string _address)
        {
            session.JsonRpcClient.pool_set_vswitch_controller(session.opaque_ref, _address);
        }

        /// <summary>
        /// Set the IP address of the vswitch controller.
        /// First published in XenServer 5.6.
        /// Deprecated since XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_address">IP address of the vswitch controller.</param>
        [Deprecated("XenServer 7.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_vswitch_controller(Session session, string _address)
        {
          return session.JsonRpcClient.async_pool_set_vswitch_controller(session.opaque_ref, _address);
        }

        /// <summary>
        /// This call tests if a location is valid
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_config">Location config settings to test</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static string test_archive_target(Session session, string _pool, Dictionary<string, string> _config)
        {
            return session.JsonRpcClient.pool_test_archive_target(session.opaque_ref, _pool, _config);
        }

        /// <summary>
        /// This call attempts to enable pool-wide local storage caching
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void enable_local_storage_caching(Session session, string _pool)
        {
            session.JsonRpcClient.pool_enable_local_storage_caching(session.opaque_ref, _pool);
        }

        /// <summary>
        /// This call attempts to enable pool-wide local storage caching
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_enable_local_storage_caching(Session session, string _pool)
        {
          return session.JsonRpcClient.async_pool_enable_local_storage_caching(session.opaque_ref, _pool);
        }

        /// <summary>
        /// This call disables pool-wide local storage caching
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void disable_local_storage_caching(Session session, string _pool)
        {
            session.JsonRpcClient.pool_disable_local_storage_caching(session.opaque_ref, _pool);
        }

        /// <summary>
        /// This call disables pool-wide local storage caching
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_disable_local_storage_caching(Session session, string _pool)
        {
          return session.JsonRpcClient.async_pool_disable_local_storage_caching(session.opaque_ref, _pool);
        }

        /// <summary>
        /// This call returns the license state for the pool
        /// First published in XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_license_state(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_license_state(session.opaque_ref, _pool);
        }

        /// <summary>
        /// This call returns the license state for the pool
        /// First published in XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_get_license_state(Session session, string _pool)
        {
          return session.JsonRpcClient.async_pool_get_license_state(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Apply an edition to all hosts in the pool
        /// First published in XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_edition">The requested edition</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void apply_edition(Session session, string _pool, string _edition)
        {
            session.JsonRpcClient.pool_apply_edition(session.opaque_ref, _pool, _edition);
        }

        /// <summary>
        /// Apply an edition to all hosts in the pool
        /// First published in XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_edition">The requested edition</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_apply_edition(Session session, string _pool, string _edition)
        {
          return session.JsonRpcClient.async_pool_apply_edition(session.opaque_ref, _pool, _edition);
        }

        /// <summary>
        /// Sets ssl_legacy true on each host, pool-master last. See Host.ssl_legacy and Host.set_ssl_legacy.
        /// First published in XenServer 7.0.
        /// Deprecated since XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        [Deprecated("XenServer 7.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void enable_ssl_legacy(Session session, string _pool)
        {
            session.JsonRpcClient.pool_enable_ssl_legacy(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Sets ssl_legacy true on each host, pool-master last. See Host.ssl_legacy and Host.set_ssl_legacy.
        /// First published in XenServer 7.0.
        /// Deprecated since XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        [Deprecated("XenServer 7.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_enable_ssl_legacy(Session session, string _pool)
        {
          return session.JsonRpcClient.async_pool_enable_ssl_legacy(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Sets ssl_legacy false on each host, pool-master last. See Host.ssl_legacy and Host.set_ssl_legacy.
        /// First published in XenServer 7.0.
        /// Deprecated since Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        [Deprecated("Citrix Hypervisor 8.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void disable_ssl_legacy(Session session, string _pool)
        {
            session.JsonRpcClient.pool_disable_ssl_legacy(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Sets ssl_legacy false on each host, pool-master last. See Host.ssl_legacy and Host.set_ssl_legacy.
        /// First published in XenServer 7.0.
        /// Deprecated since Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        [Deprecated("Citrix Hypervisor 8.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_disable_ssl_legacy(Session session, string _pool)
        {
          return session.JsonRpcClient.async_pool_disable_ssl_legacy(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Enable or disable IGMP Snooping on the pool.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">Enable or disable IGMP Snooping on the pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_igmp_snooping_enabled(Session session, string _pool, bool _value)
        {
            session.JsonRpcClient.pool_set_igmp_snooping_enabled(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Enable or disable IGMP Snooping on the pool.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">Enable or disable IGMP Snooping on the pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_igmp_snooping_enabled(Session session, string _pool, bool _value)
        {
          return session.JsonRpcClient.async_pool_set_igmp_snooping_enabled(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Return true if the extension is available on the pool
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name">The name of the API call</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static bool has_extension(Session session, string _pool, string _name)
        {
            return session.JsonRpcClient.pool_has_extension(session.opaque_ref, _pool, _name);
        }

        /// <summary>
        /// Return true if the extension is available on the pool
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name">The name of the API call</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_has_extension(Session session, string _pool, string _name)
        {
          return session.JsonRpcClient.async_pool_has_extension(session.opaque_ref, _pool, _name);
        }

        /// <summary>
        /// Add a key-value pair to the pool-wide guest agent configuration
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">The key to add</param>
        /// <param name="_value">The value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void add_to_guest_agent_config(Session session, string _pool, string _key, string _value)
        {
            session.JsonRpcClient.pool_add_to_guest_agent_config(session.opaque_ref, _pool, _key, _value);
        }

        /// <summary>
        /// Add a key-value pair to the pool-wide guest agent configuration
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">The key to add</param>
        /// <param name="_value">The value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_add_to_guest_agent_config(Session session, string _pool, string _key, string _value)
        {
          return session.JsonRpcClient.async_pool_add_to_guest_agent_config(session.opaque_ref, _pool, _key, _value);
        }

        /// <summary>
        /// Remove a key-value pair from the pool-wide guest agent configuration
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">The key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void remove_from_guest_agent_config(Session session, string _pool, string _key)
        {
            session.JsonRpcClient.pool_remove_from_guest_agent_config(session.opaque_ref, _pool, _key);
        }

        /// <summary>
        /// Remove a key-value pair from the pool-wide guest agent configuration
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">The key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_remove_from_guest_agent_config(Session session, string _pool, string _key)
        {
          return session.JsonRpcClient.async_pool_remove_from_guest_agent_config(session.opaque_ref, _pool, _key);
        }

        /// <summary>
        /// 
        /// First published in Citrix Hypervisor 8.2 Hotfix 2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void rotate_secret(Session session)
        {
            session.JsonRpcClient.pool_rotate_secret(session.opaque_ref);
        }

        /// <summary>
        /// 
        /// First published in Citrix Hypervisor 8.2 Hotfix 2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_rotate_secret(Session session)
        {
          return session.JsonRpcClient.async_pool_rotate_secret(session.opaque_ref);
        }

        /// <summary>
        /// Set enabled set of repositories
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The set of repositories to be enabled</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_repositories(Session session, string _pool, List<XenRef<Repository>> _value)
        {
            session.JsonRpcClient.pool_set_repositories(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Set enabled set of repositories
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The set of repositories to be enabled</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_repositories(Session session, string _pool, List<XenRef<Repository>> _value)
        {
          return session.JsonRpcClient.async_pool_set_repositories(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Add a repository to the enabled set
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The repository to be added to the enabled set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_repository(Session session, string _pool, string _value)
        {
            session.JsonRpcClient.pool_add_repository(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Add a repository to the enabled set
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The repository to be added to the enabled set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_add_repository(Session session, string _pool, string _value)
        {
          return session.JsonRpcClient.async_pool_add_repository(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Remove a repository from the enabled set
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The repository to be removed</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_repository(Session session, string _pool, string _value)
        {
            session.JsonRpcClient.pool_remove_repository(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Remove a repository from the enabled set
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The repository to be removed</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_remove_repository(Session session, string _pool, string _value)
        {
          return session.JsonRpcClient.async_pool_remove_repository(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Sync with the enabled repository
        /// First published in 1.329.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_force">If true local mirroring repo will be removed before syncing</param>
        /// <param name="_token">The token for repository client authentication</param>
        /// <param name="_token_id">The ID of the token</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static string sync_updates(Session session, string _pool, bool _force, string _token, string _token_id)
        {
            return session.JsonRpcClient.pool_sync_updates(session.opaque_ref, _pool, _force, _token, _token_id);
        }

        /// <summary>
        /// Sync with the enabled repository
        /// First published in 1.329.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_force">If true local mirroring repo will be removed before syncing</param>
        /// <param name="_token">The token for repository client authentication</param>
        /// <param name="_token_id">The ID of the token</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_sync_updates(Session session, string _pool, bool _force, string _token, string _token_id)
        {
          return session.JsonRpcClient.async_pool_sync_updates(session.opaque_ref, _pool, _force, _token, _token_id);
        }

        /// <summary>
        /// Sync with the enabled repository
        /// First published in 1.329.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_force">If true local mirroring repo will be removed before syncing</param>
        /// <param name="_token">The token for repository client authentication</param>
        /// <param name="_token_id">The ID of the token</param>
        /// <param name="_username">The username of the remote pool First published in 25.7.0.</param>
        /// <param name="_password">The password of the remote pool First published in 25.7.0.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static string sync_updates(Session session, string _pool, bool _force, string _token, string _token_id, string _username, string _password)
        {
            return session.JsonRpcClient.pool_sync_updates(session.opaque_ref, _pool, _force, _token, _token_id, _username, _password);
        }

        /// <summary>
        /// Sync with the enabled repository
        /// First published in 1.329.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_force">If true local mirroring repo will be removed before syncing</param>
        /// <param name="_token">The token for repository client authentication</param>
        /// <param name="_token_id">The ID of the token</param>
        /// <param name="_username">The username of the remote pool First published in 25.7.0.</param>
        /// <param name="_password">The password of the remote pool First published in 25.7.0.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_sync_updates(Session session, string _pool, bool _force, string _token, string _token_id, string _username, string _password)
        {
          return session.JsonRpcClient.async_pool_sync_updates(session.opaque_ref, _pool, _force, _token, _token_id, _username, _password);
        }

        /// <summary>
        /// Check if the pool is ready to be updated. If not, report the reasons.
        /// First published in 1.304.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_requires_reboot">Assume that the update will require host reboots</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static string[][] check_update_readiness(Session session, string _pool, bool _requires_reboot)
        {
            return session.JsonRpcClient.pool_check_update_readiness(session.opaque_ref, _pool, _requires_reboot);
        }

        /// <summary>
        /// Check if the pool is ready to be updated. If not, report the reasons.
        /// First published in 1.304.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_requires_reboot">Assume that the update will require host reboots</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_check_update_readiness(Session session, string _pool, bool _requires_reboot)
        {
          return session.JsonRpcClient.async_pool_check_update_readiness(session.opaque_ref, _pool, _requires_reboot);
        }

        /// <summary>
        /// Enable client certificate authentication on the pool
        /// First published in 1.318.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name">The name (CN/SAN) that an incoming client certificate must have to allow authentication</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void enable_client_certificate_auth(Session session, string _pool, string _name)
        {
            session.JsonRpcClient.pool_enable_client_certificate_auth(session.opaque_ref, _pool, _name);
        }

        /// <summary>
        /// Enable client certificate authentication on the pool
        /// First published in 1.318.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name">The name (CN/SAN) that an incoming client certificate must have to allow authentication</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_enable_client_certificate_auth(Session session, string _pool, string _name)
        {
          return session.JsonRpcClient.async_pool_enable_client_certificate_auth(session.opaque_ref, _pool, _name);
        }

        /// <summary>
        /// Disable client certificate authentication on the pool
        /// First published in 1.318.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void disable_client_certificate_auth(Session session, string _pool)
        {
            session.JsonRpcClient.pool_disable_client_certificate_auth(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Disable client certificate authentication on the pool
        /// First published in 1.318.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_disable_client_certificate_auth(Session session, string _pool)
        {
          return session.JsonRpcClient.async_pool_disable_client_certificate_auth(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Configure proxy for RPM package repositories.
        /// First published in 21.3.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_url">The URL of the proxy server</param>
        /// <param name="_username">The username used to authenticate with the proxy server</param>
        /// <param name="_password">The password used to authenticate with the proxy server</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void configure_repository_proxy(Session session, string _pool, string _url, string _username, string _password)
        {
            session.JsonRpcClient.pool_configure_repository_proxy(session.opaque_ref, _pool, _url, _username, _password);
        }

        /// <summary>
        /// Configure proxy for RPM package repositories.
        /// First published in 21.3.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_url">The URL of the proxy server</param>
        /// <param name="_username">The username used to authenticate with the proxy server</param>
        /// <param name="_password">The password used to authenticate with the proxy server</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_configure_repository_proxy(Session session, string _pool, string _url, string _username, string _password)
        {
          return session.JsonRpcClient.async_pool_configure_repository_proxy(session.opaque_ref, _pool, _url, _username, _password);
        }

        /// <summary>
        /// Disable the proxy for RPM package repositories.
        /// First published in 21.4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void disable_repository_proxy(Session session, string _pool)
        {
            session.JsonRpcClient.pool_disable_repository_proxy(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Disable the proxy for RPM package repositories.
        /// First published in 21.4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_disable_repository_proxy(Session session, string _pool)
        {
          return session.JsonRpcClient.async_pool_disable_repository_proxy(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Set the UEFI certificates for a pool and all its hosts. Deprecated: use set_custom_uefi_certificates instead
        /// First published in 22.16.0.
        /// Deprecated since 24.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The certificates to apply to the pool and its hosts</param>
        [Deprecated("24.0.0")]
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_uefi_certificates(Session session, string _pool, string _value)
        {
            session.JsonRpcClient.pool_set_uefi_certificates(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Set the UEFI certificates for a pool and all its hosts. Deprecated: use set_custom_uefi_certificates instead
        /// First published in 22.16.0.
        /// Deprecated since 24.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The certificates to apply to the pool and its hosts</param>
        [Deprecated("24.0.0")]
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_uefi_certificates(Session session, string _pool, string _value)
        {
          return session.JsonRpcClient.async_pool_set_uefi_certificates(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Set custom UEFI certificates for a pool and all its hosts. Need `allow-custom-uefi-certs` set to true in conf. If empty: default back to Pool.uefi_certificates
        /// Experimental. First published in 24.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The certificates to apply to the pool and its hosts</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_custom_uefi_certificates(Session session, string _pool, string _value)
        {
            session.JsonRpcClient.pool_set_custom_uefi_certificates(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Set custom UEFI certificates for a pool and all its hosts. Need `allow-custom-uefi-certs` set to true in conf. If empty: default back to Pool.uefi_certificates
        /// Experimental. First published in 24.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The certificates to apply to the pool and its hosts</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_custom_uefi_certificates(Session session, string _pool, string _value)
        {
          return session.JsonRpcClient.async_pool_set_custom_uefi_certificates(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// updates all the host firewalls in the pool to open or close port 80 depending on the value
        /// Experimental. First published in 22.27.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">true - http port 80 will be blocked, false - http port 80 will be open for the hosts in the pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_https_only(Session session, string _pool, bool _value)
        {
            session.JsonRpcClient.pool_set_https_only(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// updates all the host firewalls in the pool to open or close port 80 depending on the value
        /// Experimental. First published in 22.27.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">true - http port 80 will be blocked, false - http port 80 will be open for the hosts in the pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_https_only(Session session, string _pool, bool _value)
        {
          return session.JsonRpcClient.async_pool_set_https_only(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Set the timestamp for the next telemetry data collection.
        /// Experimental. First published in 23.9.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The earliest timestamp (in UTC) when the next round of telemetry collection can be carried out.</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_telemetry_next_collection(Session session, string _pool, DateTime _value)
        {
            session.JsonRpcClient.pool_set_telemetry_next_collection(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Set the timestamp for the next telemetry data collection.
        /// Experimental. First published in 23.9.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The earliest timestamp (in UTC) when the next round of telemetry collection can be carried out.</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_telemetry_next_collection(Session session, string _pool, DateTime _value)
        {
          return session.JsonRpcClient.async_pool_set_telemetry_next_collection(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Assign a new UUID to telemetry data.
        /// Experimental. First published in 23.9.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void reset_telemetry_uuid(Session session, string _pool)
        {
            session.JsonRpcClient.pool_reset_telemetry_uuid(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Assign a new UUID to telemetry data.
        /// Experimental. First published in 23.9.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_reset_telemetry_uuid(Session session, string _pool)
        {
          return session.JsonRpcClient.async_pool_reset_telemetry_uuid(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Configure periodic update synchronization to synchronize updates from a remote CDN
        /// Experimental. First published in 23.18.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_update_sync_frequency">The frequency at which updates are synchronized from a remote CDN: daily or weekly.</param>
        /// <param name="_update_sync_day">The day of the week the update synchronization will happen, based on pool's local timezone. Valid values are 0 to 6, 0 being Sunday. For 'daily' schedule, the value is ignored.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void configure_update_sync(Session session, string _pool, update_sync_frequency _update_sync_frequency, long _update_sync_day)
        {
            session.JsonRpcClient.pool_configure_update_sync(session.opaque_ref, _pool, _update_sync_frequency, _update_sync_day);
        }

        /// <summary>
        /// Configure periodic update synchronization to synchronize updates from a remote CDN
        /// Experimental. First published in 23.18.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_update_sync_frequency">The frequency at which updates are synchronized from a remote CDN: daily or weekly.</param>
        /// <param name="_update_sync_day">The day of the week the update synchronization will happen, based on pool's local timezone. Valid values are 0 to 6, 0 being Sunday. For 'daily' schedule, the value is ignored.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_configure_update_sync(Session session, string _pool, update_sync_frequency _update_sync_frequency, long _update_sync_day)
        {
          return session.JsonRpcClient.async_pool_configure_update_sync(session.opaque_ref, _pool, _update_sync_frequency, _update_sync_day);
        }

        /// <summary>
        /// enable or disable periodic update synchronization depending on the value
        /// Experimental. First published in 23.18.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">true - enable periodic update synchronization, false - disable it</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_update_sync_enabled(Session session, string _pool, bool _value)
        {
            session.JsonRpcClient.pool_set_update_sync_enabled(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// enable or disable periodic update synchronization depending on the value
        /// Experimental. First published in 23.18.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">true - enable periodic update synchronization, false - disable it</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_update_sync_enabled(Session session, string _pool, bool _value)
        {
          return session.JsonRpcClient.async_pool_set_update_sync_enabled(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// 
        /// Experimental. First published in 23.27.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The new maximum</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_local_auth_max_threads(Session session, string _pool, long _value)
        {
            session.JsonRpcClient.pool_set_local_auth_max_threads(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// 
        /// Experimental. First published in 23.27.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The new maximum</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_ext_auth_max_threads(Session session, string _pool, long _value)
        {
            session.JsonRpcClient.pool_set_ext_auth_max_threads(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// 
        /// Experimental. First published in 24.31.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">Specifies whether caching is enabled for external authentication</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_ext_auth_cache_enabled(Session session, string _pool, bool _value)
        {
            session.JsonRpcClient.pool_set_ext_auth_cache_enabled(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// 
        /// Experimental. First published in 24.31.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">Specifies whether caching is enabled for external authentication</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_ext_auth_cache_enabled(Session session, string _pool, bool _value)
        {
          return session.JsonRpcClient.async_pool_set_ext_auth_cache_enabled(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// 
        /// Experimental. First published in 24.31.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The capacity of the external authentication cache</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_ext_auth_cache_size(Session session, string _pool, long _value)
        {
            session.JsonRpcClient.pool_set_ext_auth_cache_size(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// 
        /// Experimental. First published in 24.31.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The capacity of the external authentication cache</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_ext_auth_cache_size(Session session, string _pool, long _value)
        {
          return session.JsonRpcClient.async_pool_set_ext_auth_cache_size(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// 
        /// Experimental. First published in 24.31.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The expiry time of entries in the external authentication cache (in seconds - 300 seconds, i.e. 5 minutes, is the default value)</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_ext_auth_cache_expiry(Session session, string _pool, long _value)
        {
            session.JsonRpcClient.pool_set_ext_auth_cache_expiry(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// 
        /// Experimental. First published in 24.31.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The expiry time of entries in the external authentication cache (in seconds - 300 seconds, i.e. 5 minutes, is the default value)</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_ext_auth_cache_expiry(Session session, string _pool, long _value)
        {
          return session.JsonRpcClient.async_pool_set_ext_auth_cache_expiry(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// 
        /// Experimental. First published in 24.17.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static pool_guest_secureboot_readiness get_guest_secureboot_readiness(Session session, string _pool)
        {
            return session.JsonRpcClient.pool_get_guest_secureboot_readiness(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Enable SSH access on all hosts in the pool. It's a helper which calls host.enable_ssh for all the hosts in the pool.
        /// Experimental. First published in 25.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void enable_ssh(Session session, string _pool)
        {
            session.JsonRpcClient.pool_enable_ssh(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Enable SSH access on all hosts in the pool. It's a helper which calls host.enable_ssh for all the hosts in the pool.
        /// Experimental. First published in 25.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_enable_ssh(Session session, string _pool)
        {
          return session.JsonRpcClient.async_pool_enable_ssh(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Disable SSH access on all hosts in the pool. It's a helper which calls host.disable_ssh for all the hosts in the pool.
        /// Experimental. First published in 25.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void disable_ssh(Session session, string _pool)
        {
            session.JsonRpcClient.pool_disable_ssh(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Disable SSH access on all hosts in the pool. It's a helper which calls host.disable_ssh for all the hosts in the pool.
        /// Experimental. First published in 25.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_disable_ssh(Session session, string _pool)
        {
          return session.JsonRpcClient.async_pool_disable_ssh(session.opaque_ref, _pool);
        }

        /// <summary>
        /// Set the SSH enabled timeout for all hosts in the pool
        /// Experimental. First published in 25.21.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The SSH enabled timeout in seconds. (0 means no timeout, max 2 days)</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_ssh_enabled_timeout(Session session, string _pool, long _value)
        {
            session.JsonRpcClient.pool_set_ssh_enabled_timeout(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Set the SSH enabled timeout for all hosts in the pool
        /// Experimental. First published in 25.21.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The SSH enabled timeout in seconds. (0 means no timeout, max 2 days)</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_ssh_enabled_timeout(Session session, string _pool, long _value)
        {
          return session.JsonRpcClient.async_pool_set_ssh_enabled_timeout(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Set the console idle timeout for all hosts in the pool
        /// Experimental. First published in 25.21.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The idle SSH/VNC session timeout in seconds. A value of 0 means no timeout.</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_console_idle_timeout(Session session, string _pool, long _value)
        {
            session.JsonRpcClient.pool_set_console_idle_timeout(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Set the console idle timeout for all hosts in the pool
        /// Experimental. First published in 25.21.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The idle SSH/VNC session timeout in seconds. A value of 0 means no timeout.</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_console_idle_timeout(Session session, string _pool, long _value)
        {
          return session.JsonRpcClient.async_pool_set_console_idle_timeout(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Set the SSH auto mode for all hosts in the pool
        /// Experimental. First published in 25.27.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The SSH auto mode for all hosts in the pool. When true, the SSH port is closed by default and it's open only in case the API is unavailable. When false, the SSH port is always open.</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_ssh_auto_mode(Session session, string _pool, bool _value)
        {
            session.JsonRpcClient.pool_set_ssh_auto_mode(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Set the SSH auto mode for all hosts in the pool
        /// Experimental. First published in 25.27.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">The SSH auto mode for all hosts in the pool. When true, the SSH port is closed by default and it's open only in case the API is unavailable. When false, the SSH port is always open.</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_ssh_auto_mode(Session session, string _pool, bool _value)
        {
          return session.JsonRpcClient.async_pool_set_ssh_auto_mode(session.opaque_ref, _pool, _value);
        }

        /// <summary>
        /// Install a trusted TLS certificate, pool-wide.
        /// Experimental. First published in 26.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_ca">The trusted certificate is a root CA certificate used to verify a chain (true), or a leaf certificate used for certificate pinning (false)</param>
        /// <param name="_cert">The certificate in PEM format</param>
        /// <param name="_purpose">The purpose of the certificate</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void install_trusted_certificate(Session session, string _pool, bool _ca, string _cert, List<certificate_purpose> _purpose)
        {
            session.JsonRpcClient.pool_install_trusted_certificate(session.opaque_ref, _pool, _ca, _cert, _purpose);
        }

        /// <summary>
        /// Install a trusted TLS certificate, pool-wide.
        /// Experimental. First published in 26.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_ca">The trusted certificate is a root CA certificate used to verify a chain (true), or a leaf certificate used for certificate pinning (false)</param>
        /// <param name="_cert">The certificate in PEM format</param>
        /// <param name="_purpose">The purpose of the certificate</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_install_trusted_certificate(Session session, string _pool, bool _ca, string _cert, List<certificate_purpose> _purpose)
        {
          return session.JsonRpcClient.async_pool_install_trusted_certificate(session.opaque_ref, _pool, _ca, _cert, _purpose);
        }

        /// <summary>
        /// Uninstall a trusted TLS certificate, pool-wide.
        /// Experimental. First published in 26.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_certificate">The reference of the trusted certificate to be uninstalled</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void uninstall_trusted_certificate(Session session, string _pool, string _certificate)
        {
            session.JsonRpcClient.pool_uninstall_trusted_certificate(session.opaque_ref, _pool, _certificate);
        }

        /// <summary>
        /// Uninstall a trusted TLS certificate, pool-wide.
        /// Experimental. First published in 26.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_certificate">The reference of the trusted certificate to be uninstalled</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_uninstall_trusted_certificate(Session session, string _pool, string _certificate)
        {
          return session.JsonRpcClient.async_pool_uninstall_trusted_certificate(session.opaque_ref, _pool, _certificate);
        }

        /// <summary>
        /// Download trusted TLS certificates from a remote pool and install them in this pool. Certificates already present locally (matched by fingerprint and purpose) are skipped.
        /// Experimental. First published in 26.16.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_remote_pool">The hostname or IP address of the coordinator of the remote pool from which the certificates are downloaded</param>
        /// <param name="_remote_session">A session obtained from the remote pool, used to authenticate the download</param>
        /// <param name="_remote_certificate">The PEM-encoded TLS certificate of the remote pool's coordinator, used to verify the TLS connection to the remote pool.</param>
        /// <param name="_ca">true for 'ca' or false for 'pinned'</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static List<XenRef<Certificate>> sync_trusted_certificates_from(Session session, string _pool, string _remote_pool, string _remote_session, string _remote_certificate, bool _ca)
        {
            return session.JsonRpcClient.pool_sync_trusted_certificates_from(session.opaque_ref, _pool, _remote_pool, _remote_session, _remote_certificate, _ca);
        }

        /// <summary>
        /// Download trusted TLS certificates from a remote pool and install them in this pool. Certificates already present locally (matched by fingerprint and purpose) are skipped.
        /// Experimental. First published in 26.16.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_remote_pool">The hostname or IP address of the coordinator of the remote pool from which the certificates are downloaded</param>
        /// <param name="_remote_session">A session obtained from the remote pool, used to authenticate the download</param>
        /// <param name="_remote_certificate">The PEM-encoded TLS certificate of the remote pool's coordinator, used to verify the TLS connection to the remote pool.</param>
        /// <param name="_ca">true for 'ca' or false for 'pinned'</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_sync_trusted_certificates_from(Session session, string _pool, string _remote_pool, string _remote_session, string _remote_certificate, bool _ca)
        {
          return session.JsonRpcClient.async_pool_sync_trusted_certificates_from(session.opaque_ref, _pool, _remote_pool, _remote_session, _remote_certificate, _ca);
        }

        /// <summary>
        /// Return a list of all the pools known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Pool>> get_all(Session session)
        {
            return session.JsonRpcClient.pool_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of pool references to pool records for all pools known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Pool>, Pool> get_all_records(Session session)
        {
            return session.JsonRpcClient.pool_get_all_records(session.opaque_ref);
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
        /// Short name
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
        /// Description
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
        /// The host that is pool master
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Host>))]
        public virtual XenRef<Host> master
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
        private XenRef<Host> _master = new XenRef<Host>(Helper.NullOpaqueRef);

        /// <summary>
        /// Default SR for VDIs
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<SR>))]
        public virtual XenRef<SR> default_SR
        {
            get { return _default_SR; }
            set
            {
                if (!Helper.AreEqual(value, _default_SR))
                {
                    _default_SR = value;
                    NotifyPropertyChanged("default_SR");
                }
            }
        }
        private XenRef<SR> _default_SR = new XenRef<SR>(Helper.NullOpaqueRef);

        /// <summary>
        /// The SR in which VDIs for suspend images are created
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<SR>))]
        public virtual XenRef<SR> suspend_image_SR
        {
            get { return _suspend_image_SR; }
            set
            {
                if (!Helper.AreEqual(value, _suspend_image_SR))
                {
                    _suspend_image_SR = value;
                    NotifyPropertyChanged("suspend_image_SR");
                }
            }
        }
        private XenRef<SR> _suspend_image_SR = new XenRef<SR>(Helper.NullOpaqueRef);

        /// <summary>
        /// The SR in which VDIs for crash dumps are created
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<SR>))]
        public virtual XenRef<SR> crash_dump_SR
        {
            get { return _crash_dump_SR; }
            set
            {
                if (!Helper.AreEqual(value, _crash_dump_SR))
                {
                    _crash_dump_SR = value;
                    NotifyPropertyChanged("crash_dump_SR");
                }
            }
        }
        private XenRef<SR> _crash_dump_SR = new XenRef<SR>(Helper.NullOpaqueRef);

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
        /// true if HA is enabled on the pool, false otherwise
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual bool ha_enabled
        {
            get { return _ha_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _ha_enabled))
                {
                    _ha_enabled = value;
                    NotifyPropertyChanged("ha_enabled");
                }
            }
        }
        private bool _ha_enabled = false;

        /// <summary>
        /// The current HA configuration
        /// First published in XenServer 5.0.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> ha_configuration
        {
            get { return _ha_configuration; }
            set
            {
                if (!Helper.AreEqual(value, _ha_configuration))
                {
                    _ha_configuration = value;
                    NotifyPropertyChanged("ha_configuration");
                }
            }
        }
        private Dictionary<string, string> _ha_configuration = new Dictionary<string, string>() {};

        /// <summary>
        /// HA statefile VDIs in use
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual string[] ha_statefiles
        {
            get { return _ha_statefiles; }
            set
            {
                if (!Helper.AreEqual(value, _ha_statefiles))
                {
                    _ha_statefiles = value;
                    NotifyPropertyChanged("ha_statefiles");
                }
            }
        }
        private string[] _ha_statefiles = {};

        /// <summary>
        /// Number of host failures to tolerate before the Pool is declared to be overcommitted
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual long ha_host_failures_to_tolerate
        {
            get { return _ha_host_failures_to_tolerate; }
            set
            {
                if (!Helper.AreEqual(value, _ha_host_failures_to_tolerate))
                {
                    _ha_host_failures_to_tolerate = value;
                    NotifyPropertyChanged("ha_host_failures_to_tolerate");
                }
            }
        }
        private long _ha_host_failures_to_tolerate = 0;

        /// <summary>
        /// Number of future host failures we have managed to find a plan for. Once this reaches zero any future host failures will cause the failure of protected VMs.
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual long ha_plan_exists_for
        {
            get { return _ha_plan_exists_for; }
            set
            {
                if (!Helper.AreEqual(value, _ha_plan_exists_for))
                {
                    _ha_plan_exists_for = value;
                    NotifyPropertyChanged("ha_plan_exists_for");
                }
            }
        }
        private long _ha_plan_exists_for = 0;

        /// <summary>
        /// If set to false then operations which would cause the Pool to become overcommitted will be blocked.
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual bool ha_allow_overcommit
        {
            get { return _ha_allow_overcommit; }
            set
            {
                if (!Helper.AreEqual(value, _ha_allow_overcommit))
                {
                    _ha_allow_overcommit = value;
                    NotifyPropertyChanged("ha_allow_overcommit");
                }
            }
        }
        private bool _ha_allow_overcommit = false;

        /// <summary>
        /// True if the Pool is considered to be overcommitted i.e. if there exist insufficient physical resources to tolerate the configured number of host failures
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual bool ha_overcommitted
        {
            get { return _ha_overcommitted; }
            set
            {
                if (!Helper.AreEqual(value, _ha_overcommitted))
                {
                    _ha_overcommitted = value;
                    NotifyPropertyChanged("ha_overcommitted");
                }
            }
        }
        private bool _ha_overcommitted = false;

        /// <summary>
        /// Binary blobs associated with this pool
        /// First published in XenServer 5.0.
        /// </summary>
        [JsonConverter(typeof(StringXenRefMapConverter<Blob>))]
        public virtual Dictionary<string, XenRef<Blob>> blobs
        {
            get { return _blobs; }
            set
            {
                if (!Helper.AreEqual(value, _blobs))
                {
                    _blobs = value;
                    NotifyPropertyChanged("blobs");
                }
            }
        }
        private Dictionary<string, XenRef<Blob>> _blobs = new Dictionary<string, XenRef<Blob>>() {};

        /// <summary>
        /// user-specified tags for categorization purposes
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual string[] tags
        {
            get { return _tags; }
            set
            {
                if (!Helper.AreEqual(value, _tags))
                {
                    _tags = value;
                    NotifyPropertyChanged("tags");
                }
            }
        }
        private string[] _tags = {};

        /// <summary>
        /// gui-specific configuration for pool
        /// First published in XenServer 5.0.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> gui_config
        {
            get { return _gui_config; }
            set
            {
                if (!Helper.AreEqual(value, _gui_config))
                {
                    _gui_config = value;
                    NotifyPropertyChanged("gui_config");
                }
            }
        }
        private Dictionary<string, string> _gui_config = new Dictionary<string, string>() {};

        /// <summary>
        /// Configuration for the automatic health check feature
        /// First published in XenServer 7.0.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> health_check_config
        {
            get { return _health_check_config; }
            set
            {
                if (!Helper.AreEqual(value, _health_check_config))
                {
                    _health_check_config = value;
                    NotifyPropertyChanged("health_check_config");
                }
            }
        }
        private Dictionary<string, string> _health_check_config = new Dictionary<string, string>() {};

        /// <summary>
        /// Url for the configured workload balancing host
        /// First published in XenServer 5.5.
        /// </summary>
        public virtual string wlb_url
        {
            get { return _wlb_url; }
            set
            {
                if (!Helper.AreEqual(value, _wlb_url))
                {
                    _wlb_url = value;
                    NotifyPropertyChanged("wlb_url");
                }
            }
        }
        private string _wlb_url = "";

        /// <summary>
        /// Username for accessing the workload balancing host
        /// First published in XenServer 5.5.
        /// </summary>
        public virtual string wlb_username
        {
            get { return _wlb_username; }
            set
            {
                if (!Helper.AreEqual(value, _wlb_username))
                {
                    _wlb_username = value;
                    NotifyPropertyChanged("wlb_username");
                }
            }
        }
        private string _wlb_username = "";

        /// <summary>
        /// true if workload balancing is enabled on the pool, false otherwise
        /// First published in XenServer 5.5.
        /// </summary>
        public virtual bool wlb_enabled
        {
            get { return _wlb_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _wlb_enabled))
                {
                    _wlb_enabled = value;
                    NotifyPropertyChanged("wlb_enabled");
                }
            }
        }
        private bool _wlb_enabled = false;

        /// <summary>
        /// true if communication with the WLB server should enforce TLS certificate verification.
        /// First published in XenServer 5.5.
        /// </summary>
        public virtual bool wlb_verify_cert
        {
            get { return _wlb_verify_cert; }
            set
            {
                if (!Helper.AreEqual(value, _wlb_verify_cert))
                {
                    _wlb_verify_cert = value;
                    NotifyPropertyChanged("wlb_verify_cert");
                }
            }
        }
        private bool _wlb_verify_cert = false;

        /// <summary>
        /// true a redo-log is to be used other than when HA is enabled, false otherwise
        /// First published in XenServer 5.6.
        /// </summary>
        public virtual bool redo_log_enabled
        {
            get { return _redo_log_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _redo_log_enabled))
                {
                    _redo_log_enabled = value;
                    NotifyPropertyChanged("redo_log_enabled");
                }
            }
        }
        private bool _redo_log_enabled = false;

        /// <summary>
        /// indicates the VDI to use for the redo-log other than when HA is enabled
        /// First published in XenServer 5.6.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VDI>))]
        public virtual XenRef<VDI> redo_log_vdi
        {
            get { return _redo_log_vdi; }
            set
            {
                if (!Helper.AreEqual(value, _redo_log_vdi))
                {
                    _redo_log_vdi = value;
                    NotifyPropertyChanged("redo_log_vdi");
                }
            }
        }
        private XenRef<VDI> _redo_log_vdi = new XenRef<VDI>("OpaqueRef:NULL");

        /// <summary>
        /// address of the vswitch controller
        /// First published in XenServer 5.6.
        /// </summary>
        public virtual string vswitch_controller
        {
            get { return _vswitch_controller; }
            set
            {
                if (!Helper.AreEqual(value, _vswitch_controller))
                {
                    _vswitch_controller = value;
                    NotifyPropertyChanged("vswitch_controller");
                }
            }
        }
        private string _vswitch_controller = "";

        /// <summary>
        /// Pool-wide restrictions currently in effect
        /// First published in XenServer 5.6.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> restrictions
        {
            get { return _restrictions; }
            set
            {
                if (!Helper.AreEqual(value, _restrictions))
                {
                    _restrictions = value;
                    NotifyPropertyChanged("restrictions");
                }
            }
        }
        private Dictionary<string, string> _restrictions = new Dictionary<string, string>() {};

        /// <summary>
        /// The set of currently known metadata VDIs for this pool
        /// First published in XenServer 6.0.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VDI>))]
        public virtual List<XenRef<VDI>> metadata_VDIs
        {
            get { return _metadata_VDIs; }
            set
            {
                if (!Helper.AreEqual(value, _metadata_VDIs))
                {
                    _metadata_VDIs = value;
                    NotifyPropertyChanged("metadata_VDIs");
                }
            }
        }
        private List<XenRef<VDI>> _metadata_VDIs = new List<XenRef<VDI>>() {};

        /// <summary>
        /// The HA cluster stack that is currently in use. Only valid when HA is enabled.
        /// First published in XenServer 7.0.
        /// </summary>
        public virtual string ha_cluster_stack
        {
            get { return _ha_cluster_stack; }
            set
            {
                if (!Helper.AreEqual(value, _ha_cluster_stack))
                {
                    _ha_cluster_stack = value;
                    NotifyPropertyChanged("ha_cluster_stack");
                }
            }
        }
        private string _ha_cluster_stack = "";

        /// <summary>
        /// list of the operations allowed in this state. This list is advisory only and the server state may have changed by the time this field is read by a client.
        /// </summary>
        public virtual List<pool_allowed_operations> allowed_operations
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
        private List<pool_allowed_operations> _allowed_operations = new List<pool_allowed_operations>() {};

        /// <summary>
        /// links each of the running tasks using this object (by reference) to a current_operation enum which describes the nature of the task.
        /// </summary>
        public virtual Dictionary<string, pool_allowed_operations> current_operations
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
        private Dictionary<string, pool_allowed_operations> _current_operations = new Dictionary<string, pool_allowed_operations>() {};

        /// <summary>
        /// Pool-wide guest agent configuration information
        /// First published in XenServer 7.0.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> guest_agent_config
        {
            get { return _guest_agent_config; }
            set
            {
                if (!Helper.AreEqual(value, _guest_agent_config))
                {
                    _guest_agent_config = value;
                    NotifyPropertyChanged("guest_agent_config");
                }
            }
        }
        private Dictionary<string, string> _guest_agent_config = new Dictionary<string, string>() {};

        /// <summary>
        /// Details about the physical CPUs on the pool
        /// First published in XenServer 7.0.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> cpu_info
        {
            get { return _cpu_info; }
            set
            {
                if (!Helper.AreEqual(value, _cpu_info))
                {
                    _cpu_info = value;
                    NotifyPropertyChanged("cpu_info");
                }
            }
        }
        private Dictionary<string, string> _cpu_info = new Dictionary<string, string>() {};

        /// <summary>
        /// This field was consulted when VM.create did not specify a value for 'has_vendor_device'; VM.create now uses a simple default and no longer consults this value.
        /// First published in XenServer 7.0.
        /// </summary>
        public virtual bool policy_no_vendor_device
        {
            get { return _policy_no_vendor_device; }
            set
            {
                if (!Helper.AreEqual(value, _policy_no_vendor_device))
                {
                    _policy_no_vendor_device = value;
                    NotifyPropertyChanged("policy_no_vendor_device");
                }
            }
        }
        private bool _policy_no_vendor_device = false;

        /// <summary>
        /// The pool-wide flag to show if the live patching feauture is disabled or not.
        /// First published in XenServer 7.1.
        /// </summary>
        public virtual bool live_patching_disabled
        {
            get { return _live_patching_disabled; }
            set
            {
                if (!Helper.AreEqual(value, _live_patching_disabled))
                {
                    _live_patching_disabled = value;
                    NotifyPropertyChanged("live_patching_disabled");
                }
            }
        }
        private bool _live_patching_disabled = false;

        /// <summary>
        /// true if IGMP snooping is enabled in the pool, false otherwise.
        /// First published in XenServer 7.3.
        /// </summary>
        public virtual bool igmp_snooping_enabled
        {
            get { return _igmp_snooping_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _igmp_snooping_enabled))
                {
                    _igmp_snooping_enabled = value;
                    NotifyPropertyChanged("igmp_snooping_enabled");
                }
            }
        }
        private bool _igmp_snooping_enabled = false;

        /// <summary>
        /// The UEFI certificates allowing Secure Boot
        /// First published in Citrix Hypervisor 8.1.
        /// </summary>
        public virtual string uefi_certificates
        {
            get { return _uefi_certificates; }
            set
            {
                if (!Helper.AreEqual(value, _uefi_certificates))
                {
                    _uefi_certificates = value;
                    NotifyPropertyChanged("uefi_certificates");
                }
            }
        }
        private string _uefi_certificates = "";

        /// <summary>
        /// Custom UEFI certificates allowing Secure Boot
        /// Experimental. First published in 24.0.0.
        /// </summary>
        public virtual string custom_uefi_certificates
        {
            get { return _custom_uefi_certificates; }
            set
            {
                if (!Helper.AreEqual(value, _custom_uefi_certificates))
                {
                    _custom_uefi_certificates = value;
                    NotifyPropertyChanged("custom_uefi_certificates");
                }
            }
        }
        private string _custom_uefi_certificates = "";

        /// <summary>
        /// True if either a PSR is running or we are waiting for a PSR to be re-run
        /// First published in Citrix Hypervisor 8.2 Hotfix 2.
        /// </summary>
        public virtual bool is_psr_pending
        {
            get { return _is_psr_pending; }
            set
            {
                if (!Helper.AreEqual(value, _is_psr_pending))
                {
                    _is_psr_pending = value;
                    NotifyPropertyChanged("is_psr_pending");
                }
            }
        }
        private bool _is_psr_pending = false;

        /// <summary>
        /// True iff TLS certificate verification is enabled
        /// First published in 1.290.0.
        /// </summary>
        public virtual bool tls_verification_enabled
        {
            get { return _tls_verification_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _tls_verification_enabled))
                {
                    _tls_verification_enabled = value;
                    NotifyPropertyChanged("tls_verification_enabled");
                }
            }
        }
        private bool _tls_verification_enabled = false;

        /// <summary>
        /// The set of currently enabled repositories
        /// First published in 1.301.0.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Repository>))]
        public virtual List<XenRef<Repository>> repositories
        {
            get { return _repositories; }
            set
            {
                if (!Helper.AreEqual(value, _repositories))
                {
                    _repositories = value;
                    NotifyPropertyChanged("repositories");
                }
            }
        }
        private List<XenRef<Repository>> _repositories = new List<XenRef<Repository>>() {};

        /// <summary>
        /// True if authentication by TLS client certificates is enabled
        /// First published in 1.318.0.
        /// </summary>
        public virtual bool client_certificate_auth_enabled
        {
            get { return _client_certificate_auth_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _client_certificate_auth_enabled))
                {
                    _client_certificate_auth_enabled = value;
                    NotifyPropertyChanged("client_certificate_auth_enabled");
                }
            }
        }
        private bool _client_certificate_auth_enabled = false;

        /// <summary>
        /// The name (CN/SAN) that an incoming client certificate must have to allow authentication
        /// First published in 1.318.0.
        /// </summary>
        public virtual string client_certificate_auth_name
        {
            get { return _client_certificate_auth_name; }
            set
            {
                if (!Helper.AreEqual(value, _client_certificate_auth_name))
                {
                    _client_certificate_auth_name = value;
                    NotifyPropertyChanged("client_certificate_auth_name");
                }
            }
        }
        private string _client_certificate_auth_name = "";

        /// <summary>
        /// Url of the proxy used in syncing with the enabled repositories
        /// First published in 21.3.0.
        /// </summary>
        public virtual string repository_proxy_url
        {
            get { return _repository_proxy_url; }
            set
            {
                if (!Helper.AreEqual(value, _repository_proxy_url))
                {
                    _repository_proxy_url = value;
                    NotifyPropertyChanged("repository_proxy_url");
                }
            }
        }
        private string _repository_proxy_url = "";

        /// <summary>
        /// Username for the authentication of the proxy used in syncing with the enabled repositories
        /// First published in 21.3.0.
        /// </summary>
        public virtual string repository_proxy_username
        {
            get { return _repository_proxy_username; }
            set
            {
                if (!Helper.AreEqual(value, _repository_proxy_username))
                {
                    _repository_proxy_username = value;
                    NotifyPropertyChanged("repository_proxy_username");
                }
            }
        }
        private string _repository_proxy_username = "";

        /// <summary>
        /// Password for the authentication of the proxy used in syncing with the enabled repositories
        /// First published in 21.3.0.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Secret>))]
        public virtual XenRef<Secret> repository_proxy_password
        {
            get { return _repository_proxy_password; }
            set
            {
                if (!Helper.AreEqual(value, _repository_proxy_password))
                {
                    _repository_proxy_password = value;
                    NotifyPropertyChanged("repository_proxy_password");
                }
            }
        }
        private XenRef<Secret> _repository_proxy_password = new XenRef<Secret>("OpaqueRef:NULL");

        /// <summary>
        /// Default behaviour during migration, True if stream compression should be used
        /// Experimental. First published in 22.33.0.
        /// </summary>
        public virtual bool migration_compression
        {
            get { return _migration_compression; }
            set
            {
                if (!Helper.AreEqual(value, _migration_compression))
                {
                    _migration_compression = value;
                    NotifyPropertyChanged("migration_compression");
                }
            }
        }
        private bool _migration_compression = false;

        /// <summary>
        /// true if bias against pool master when scheduling vms is enabled, false otherwise
        /// </summary>
        public virtual bool coordinator_bias
        {
            get { return _coordinator_bias; }
            set
            {
                if (!Helper.AreEqual(value, _coordinator_bias))
                {
                    _coordinator_bias = value;
                    NotifyPropertyChanged("coordinator_bias");
                }
            }
        }
        private bool _coordinator_bias = true;

        /// <summary>
        /// Maximum number of threads to use for PAM authentication
        /// Experimental. First published in 23.27.0.
        /// </summary>
        public virtual long local_auth_max_threads
        {
            get { return _local_auth_max_threads; }
            set
            {
                if (!Helper.AreEqual(value, _local_auth_max_threads))
                {
                    _local_auth_max_threads = value;
                    NotifyPropertyChanged("local_auth_max_threads");
                }
            }
        }
        private long _local_auth_max_threads = 8;

        /// <summary>
        /// Maximum number of threads to use for external (AD) authentication
        /// Experimental. First published in 23.27.0.
        /// </summary>
        public virtual long ext_auth_max_threads
        {
            get { return _ext_auth_max_threads; }
            set
            {
                if (!Helper.AreEqual(value, _ext_auth_max_threads))
                {
                    _ext_auth_max_threads = value;
                    NotifyPropertyChanged("ext_auth_max_threads");
                }
            }
        }
        private long _ext_auth_max_threads = 1;

        /// <summary>
        /// Specifies whether external authentication caching is enabled for this pool or not
        /// Experimental. First published in 24.31.0.
        /// </summary>
        public virtual bool ext_auth_cache_enabled
        {
            get { return _ext_auth_cache_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _ext_auth_cache_enabled))
                {
                    _ext_auth_cache_enabled = value;
                    NotifyPropertyChanged("ext_auth_cache_enabled");
                }
            }
        }
        private bool _ext_auth_cache_enabled = true;

        /// <summary>
        /// Maximum capacity of external authentication cache
        /// Experimental. First published in 24.31.0.
        /// </summary>
        public virtual long ext_auth_cache_size
        {
            get { return _ext_auth_cache_size; }
            set
            {
                if (!Helper.AreEqual(value, _ext_auth_cache_size))
                {
                    _ext_auth_cache_size = value;
                    NotifyPropertyChanged("ext_auth_cache_size");
                }
            }
        }
        private long _ext_auth_cache_size = 50;

        /// <summary>
        /// Specifies how long external authentication entries should be cached for (seconds)
        /// Experimental. First published in 24.31.0.
        /// </summary>
        public virtual long ext_auth_cache_expiry
        {
            get { return _ext_auth_cache_expiry; }
            set
            {
                if (!Helper.AreEqual(value, _ext_auth_cache_expiry))
                {
                    _ext_auth_cache_expiry = value;
                    NotifyPropertyChanged("ext_auth_cache_expiry");
                }
            }
        }
        private long _ext_auth_cache_expiry = 300;

        /// <summary>
        /// The UUID of the pool for identification of telemetry data
        /// Experimental. First published in 23.9.0.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Secret>))]
        public virtual XenRef<Secret> telemetry_uuid
        {
            get { return _telemetry_uuid; }
            set
            {
                if (!Helper.AreEqual(value, _telemetry_uuid))
                {
                    _telemetry_uuid = value;
                    NotifyPropertyChanged("telemetry_uuid");
                }
            }
        }
        private XenRef<Secret> _telemetry_uuid = new XenRef<Secret>("OpaqueRef:NULL");

        /// <summary>
        /// How often the telemetry collection will be carried out
        /// Experimental. First published in 23.9.0.
        /// </summary>
        [JsonConverter(typeof(telemetry_frequencyConverter))]
        public virtual telemetry_frequency telemetry_frequency
        {
            get { return _telemetry_frequency; }
            set
            {
                if (!Helper.AreEqual(value, _telemetry_frequency))
                {
                    _telemetry_frequency = value;
                    NotifyPropertyChanged("telemetry_frequency");
                }
            }
        }
        private telemetry_frequency _telemetry_frequency = telemetry_frequency.weekly;

        /// <summary>
        /// The earliest timestamp (in UTC) when the next round of telemetry collection can be carried out
        /// Experimental. First published in 23.9.0.
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime telemetry_next_collection
        {
            get { return _telemetry_next_collection; }
            set
            {
                if (!Helper.AreEqual(value, _telemetry_next_collection))
                {
                    _telemetry_next_collection = value;
                    NotifyPropertyChanged("telemetry_next_collection");
                }
            }
        }
        private DateTime _telemetry_next_collection = DateTime.ParseExact("19700101T00:00:00Z", "yyyyMMddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        /// <summary>
        /// time of the last update sychronization
        /// Experimental. First published in 23.18.0.
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime last_update_sync
        {
            get { return _last_update_sync; }
            set
            {
                if (!Helper.AreEqual(value, _last_update_sync))
                {
                    _last_update_sync = value;
                    NotifyPropertyChanged("last_update_sync");
                }
            }
        }
        private DateTime _last_update_sync = DateTime.ParseExact("19700101T00:00:00Z", "yyyyMMddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        /// <summary>
        /// The frequency at which updates are synchronized from a remote CDN: daily or weekly.
        /// Experimental. First published in 23.18.0.
        /// </summary>
        [JsonConverter(typeof(update_sync_frequencyConverter))]
        public virtual update_sync_frequency update_sync_frequency
        {
            get { return _update_sync_frequency; }
            set
            {
                if (!Helper.AreEqual(value, _update_sync_frequency))
                {
                    _update_sync_frequency = value;
                    NotifyPropertyChanged("update_sync_frequency");
                }
            }
        }
        private update_sync_frequency _update_sync_frequency = update_sync_frequency.weekly;

        /// <summary>
        /// The day of the week the update synchronizations will be scheduled, based on pool's local timezone. Ignored when update_sync_frequency is daily
        /// Experimental. First published in 23.18.0.
        /// </summary>
        public virtual long update_sync_day
        {
            get { return _update_sync_day; }
            set
            {
                if (!Helper.AreEqual(value, _update_sync_day))
                {
                    _update_sync_day = value;
                    NotifyPropertyChanged("update_sync_day");
                }
            }
        }
        private long _update_sync_day = 0;

        /// <summary>
        /// Whether periodic update synchronization is enabled or not
        /// Experimental. First published in 23.18.0.
        /// </summary>
        public virtual bool update_sync_enabled
        {
            get { return _update_sync_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _update_sync_enabled))
                {
                    _update_sync_enabled = value;
                    NotifyPropertyChanged("update_sync_enabled");
                }
            }
        }
        private bool _update_sync_enabled = false;

        /// <summary>
        /// The recommended pool properties for clients to respect for optimal performance. For example, max-vm-group=5
        /// Experimental. First published in 24.19.1.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> recommendations
        {
            get { return _recommendations; }
            set
            {
                if (!Helper.AreEqual(value, _recommendations))
                {
                    _recommendations = value;
                    NotifyPropertyChanged("recommendations");
                }
            }
        }
        private Dictionary<string, string> _recommendations = new Dictionary<string, string>() {};

        /// <summary>
        /// Licensing data shared within the whole pool
        /// Experimental. First published in 25.6.0.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> license_server
        {
            get { return _license_server; }
            set
            {
                if (!Helper.AreEqual(value, _license_server))
                {
                    _license_server = value;
                    NotifyPropertyChanged("license_server");
                }
            }
        }
        private Dictionary<string, string> _license_server = new Dictionary<string, string>() {};

        /// <summary>
        /// Indicates whether an HA-protected VM that is shut down from inside (not through the API) should be automatically rebooted when HA is enabled
        /// Experimental. First published in 25.16.0.
        /// </summary>
        public virtual bool ha_reboot_vm_on_internal_shutdown
        {
            get { return _ha_reboot_vm_on_internal_shutdown; }
            set
            {
                if (!Helper.AreEqual(value, _ha_reboot_vm_on_internal_shutdown))
                {
                    _ha_reboot_vm_on_internal_shutdown = value;
                    NotifyPropertyChanged("ha_reboot_vm_on_internal_shutdown");
                }
            }
        }
        private bool _ha_reboot_vm_on_internal_shutdown = true;

        /// <summary>
        /// When true, only one console connection per VM/host in the pool is accepted. Otherwise every connection for a VM/host's console is accepted. Note: when true, connection attempts via websocket will be rejected.
        /// Experimental. First published in 26.1.0.
        /// </summary>
        public virtual bool limit_console_sessions
        {
            get { return _limit_console_sessions; }
            set
            {
                if (!Helper.AreEqual(value, _limit_console_sessions))
                {
                    _limit_console_sessions = value;
                    NotifyPropertyChanged("limit_console_sessions");
                }
            }
        }
        private bool _limit_console_sessions = false;

        /// <summary>
        /// The maximum time (in seconds) that a VM's console can be idle before it is automatically disconnected. The default value 0 means never timeout. This setting applies only to VM consoles; for host consoles, use the separate parameter 'host.console_idle_timeout'.
        /// Experimental. First published in 26.1.0.
        /// </summary>
        public virtual long vm_console_idle_timeout
        {
            get { return _vm_console_idle_timeout; }
            set
            {
                if (!Helper.AreEqual(value, _vm_console_idle_timeout))
                {
                    _vm_console_idle_timeout = value;
                    NotifyPropertyChanged("vm_console_idle_timeout");
                }
            }
        }
        private long _vm_console_idle_timeout = 0;

        /// <summary>
        /// When true, at VM.create time the pool automatically marks a newly created VM whose Secure Boot certificates are due to expire by setting VM.secureboot_certificates_state to update_on_boot (the same effect as VM.update_secureboot_certificates_on_boot), so the certificates are updated on the VM's next boot. This applies to VMs created by VM.create, including those created by operations that internally call it: the HTTP(s) PUT /import and /import_metadata calls and disaster recovery (VM.recover and VM_appliance.recover); it does not apply to VM.clone or VM.copy, which inherit the source VM's state.
        /// Experimental. First published in 26.16.0-next.
        /// </summary>
        public virtual bool auto_update_vm_secureboot_certificates
        {
            get { return _auto_update_vm_secureboot_certificates; }
            set
            {
                if (!Helper.AreEqual(value, _auto_update_vm_secureboot_certificates))
                {
                    _auto_update_vm_secureboot_certificates = value;
                    NotifyPropertyChanged("auto_update_vm_secureboot_certificates");
                }
            }
        }
        private bool _auto_update_vm_secureboot_certificates = false;
    }
}
