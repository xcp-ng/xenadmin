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
    /// A physical host
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class Host : XenObject<Host>
    {
        #region Constructors

        public Host()
        {
        }

        public Host(string uuid,
            string name_label,
            string name_description,
            long memory_overhead,
            List<host_allowed_operations> allowed_operations,
            Dictionary<string, host_allowed_operations> current_operations,
            long API_version_major,
            long API_version_minor,
            string API_version_vendor,
            Dictionary<string, string> API_version_vendor_implementation,
            bool enabled,
            Dictionary<string, string> software_version,
            Dictionary<string, string> other_config,
            string[] capabilities,
            Dictionary<string, string> cpu_configuration,
            string sched_policy,
            string[] supported_bootloaders,
            List<XenRef<VM>> resident_VMs,
            Dictionary<string, string> logging,
            List<XenRef<PIF>> PIFs,
            XenRef<SR> suspend_image_sr,
            XenRef<SR> crash_dump_sr,
            List<XenRef<Host_crashdump>> crashdumps,
            List<XenRef<Host_patch>> patches,
            List<XenRef<Pool_update>> updates,
            List<XenRef<PBD>> PBDs,
            List<XenRef<Host_cpu>> host_CPUs,
            Dictionary<string, string> cpu_info,
            string hostname,
            string address,
            XenRef<Host_metrics> metrics,
            Dictionary<string, string> license_params,
            string[] ha_statefiles,
            string[] ha_network_peers,
            Dictionary<string, XenRef<Blob>> blobs,
            string[] tags,
            string external_auth_type,
            string external_auth_service_name,
            Dictionary<string, string> external_auth_configuration,
            string edition,
            Dictionary<string, string> license_server,
            Dictionary<string, string> bios_strings,
            string power_on_mode,
            Dictionary<string, string> power_on_config,
            XenRef<SR> local_cache_sr,
            Dictionary<string, string> chipset_info,
            List<XenRef<PCI>> PCIs,
            List<XenRef<PGPU>> PGPUs,
            List<XenRef<PUSB>> PUSBs,
            bool ssl_legacy,
            Dictionary<string, string> guest_VCPUs_params,
            host_display display,
            long[] virtual_hardware_platform_versions,
            XenRef<VM> control_domain,
            List<XenRef<Pool_update>> updates_requiring_reboot,
            List<XenRef<Feature>> features,
            string iscsi_iqn,
            bool multipathing,
            string uefi_certificates,
            List<XenRef<Certificate>> certificates,
            string[] editions,
            List<update_guidances> pending_guidances,
            bool tls_verification_enabled,
            DateTime last_software_update,
            bool https_only,
            latest_synced_updates_applied_state latest_synced_updates_applied,
            host_numa_affinity_policy numa_affinity_policy,
            List<update_guidances> pending_guidances_recommended,
            List<update_guidances> pending_guidances_full,
            string last_update_hash,
            bool ssh_enabled,
            long ssh_enabled_timeout,
            DateTime ssh_expiry,
            long console_idle_timeout,
            bool ssh_auto_mode,
            string max_cstate,
            bool secure_boot,
            host_ntp_mode ntp_mode,
            string[] ntp_custom_servers,
            string timezone)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.memory_overhead = memory_overhead;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.API_version_major = API_version_major;
            this.API_version_minor = API_version_minor;
            this.API_version_vendor = API_version_vendor;
            this.API_version_vendor_implementation = API_version_vendor_implementation;
            this.enabled = enabled;
            this.software_version = software_version;
            this.other_config = other_config;
            this.capabilities = capabilities;
            this.cpu_configuration = cpu_configuration;
            this.sched_policy = sched_policy;
            this.supported_bootloaders = supported_bootloaders;
            this.resident_VMs = resident_VMs;
            this.logging = logging;
            this.PIFs = PIFs;
            this.suspend_image_sr = suspend_image_sr;
            this.crash_dump_sr = crash_dump_sr;
            this.crashdumps = crashdumps;
            this.patches = patches;
            this.updates = updates;
            this.PBDs = PBDs;
            this.host_CPUs = host_CPUs;
            this.cpu_info = cpu_info;
            this.hostname = hostname;
            this.address = address;
            this.metrics = metrics;
            this.license_params = license_params;
            this.ha_statefiles = ha_statefiles;
            this.ha_network_peers = ha_network_peers;
            this.blobs = blobs;
            this.tags = tags;
            this.external_auth_type = external_auth_type;
            this.external_auth_service_name = external_auth_service_name;
            this.external_auth_configuration = external_auth_configuration;
            this.edition = edition;
            this.license_server = license_server;
            this.bios_strings = bios_strings;
            this.power_on_mode = power_on_mode;
            this.power_on_config = power_on_config;
            this.local_cache_sr = local_cache_sr;
            this.chipset_info = chipset_info;
            this.PCIs = PCIs;
            this.PGPUs = PGPUs;
            this.PUSBs = PUSBs;
            this.ssl_legacy = ssl_legacy;
            this.guest_VCPUs_params = guest_VCPUs_params;
            this.display = display;
            this.virtual_hardware_platform_versions = virtual_hardware_platform_versions;
            this.control_domain = control_domain;
            this.updates_requiring_reboot = updates_requiring_reboot;
            this.features = features;
            this.iscsi_iqn = iscsi_iqn;
            this.multipathing = multipathing;
            this.uefi_certificates = uefi_certificates;
            this.certificates = certificates;
            this.editions = editions;
            this.pending_guidances = pending_guidances;
            this.tls_verification_enabled = tls_verification_enabled;
            this.last_software_update = last_software_update;
            this.https_only = https_only;
            this.latest_synced_updates_applied = latest_synced_updates_applied;
            this.numa_affinity_policy = numa_affinity_policy;
            this.pending_guidances_recommended = pending_guidances_recommended;
            this.pending_guidances_full = pending_guidances_full;
            this.last_update_hash = last_update_hash;
            this.ssh_enabled = ssh_enabled;
            this.ssh_enabled_timeout = ssh_enabled_timeout;
            this.ssh_expiry = ssh_expiry;
            this.console_idle_timeout = console_idle_timeout;
            this.ssh_auto_mode = ssh_auto_mode;
            this.max_cstate = max_cstate;
            this.secure_boot = secure_boot;
            this.ntp_mode = ntp_mode;
            this.ntp_custom_servers = ntp_custom_servers;
            this.timezone = timezone;
        }

        /// <summary>
        /// Creates a new Host from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Host(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Host.
        /// </summary>
        public override void UpdateFrom(Host record)
        {
            uuid = record.uuid;
            name_label = record.name_label;
            name_description = record.name_description;
            memory_overhead = record.memory_overhead;
            allowed_operations = record.allowed_operations;
            current_operations = record.current_operations;
            API_version_major = record.API_version_major;
            API_version_minor = record.API_version_minor;
            API_version_vendor = record.API_version_vendor;
            API_version_vendor_implementation = record.API_version_vendor_implementation;
            enabled = record.enabled;
            software_version = record.software_version;
            other_config = record.other_config;
            capabilities = record.capabilities;
            cpu_configuration = record.cpu_configuration;
            sched_policy = record.sched_policy;
            supported_bootloaders = record.supported_bootloaders;
            resident_VMs = record.resident_VMs;
            logging = record.logging;
            PIFs = record.PIFs;
            suspend_image_sr = record.suspend_image_sr;
            crash_dump_sr = record.crash_dump_sr;
            crashdumps = record.crashdumps;
            patches = record.patches;
            updates = record.updates;
            PBDs = record.PBDs;
            host_CPUs = record.host_CPUs;
            cpu_info = record.cpu_info;
            hostname = record.hostname;
            address = record.address;
            metrics = record.metrics;
            license_params = record.license_params;
            ha_statefiles = record.ha_statefiles;
            ha_network_peers = record.ha_network_peers;
            blobs = record.blobs;
            tags = record.tags;
            external_auth_type = record.external_auth_type;
            external_auth_service_name = record.external_auth_service_name;
            external_auth_configuration = record.external_auth_configuration;
            edition = record.edition;
            license_server = record.license_server;
            bios_strings = record.bios_strings;
            power_on_mode = record.power_on_mode;
            power_on_config = record.power_on_config;
            local_cache_sr = record.local_cache_sr;
            chipset_info = record.chipset_info;
            PCIs = record.PCIs;
            PGPUs = record.PGPUs;
            PUSBs = record.PUSBs;
            ssl_legacy = record.ssl_legacy;
            guest_VCPUs_params = record.guest_VCPUs_params;
            display = record.display;
            virtual_hardware_platform_versions = record.virtual_hardware_platform_versions;
            control_domain = record.control_domain;
            updates_requiring_reboot = record.updates_requiring_reboot;
            features = record.features;
            iscsi_iqn = record.iscsi_iqn;
            multipathing = record.multipathing;
            uefi_certificates = record.uefi_certificates;
            certificates = record.certificates;
            editions = record.editions;
            pending_guidances = record.pending_guidances;
            tls_verification_enabled = record.tls_verification_enabled;
            last_software_update = record.last_software_update;
            https_only = record.https_only;
            latest_synced_updates_applied = record.latest_synced_updates_applied;
            numa_affinity_policy = record.numa_affinity_policy;
            pending_guidances_recommended = record.pending_guidances_recommended;
            pending_guidances_full = record.pending_guidances_full;
            last_update_hash = record.last_update_hash;
            ssh_enabled = record.ssh_enabled;
            ssh_enabled_timeout = record.ssh_enabled_timeout;
            ssh_expiry = record.ssh_expiry;
            console_idle_timeout = record.console_idle_timeout;
            ssh_auto_mode = record.ssh_auto_mode;
            max_cstate = record.max_cstate;
            secure_boot = record.secure_boot;
            ntp_mode = record.ntp_mode;
            ntp_custom_servers = record.ntp_custom_servers;
            timezone = record.timezone;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Host
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
            if (table.ContainsKey("memory_overhead"))
                memory_overhead = Marshalling.ParseLong(table, "memory_overhead");
            if (table.ContainsKey("allowed_operations"))
                allowed_operations = Helper.StringArrayToEnumList<host_allowed_operations>(Marshalling.ParseStringArray(table, "allowed_operations"));
            if (table.ContainsKey("current_operations"))
                current_operations = Maps.ToDictionary_string_host_allowed_operations(Marshalling.ParseHashTable(table, "current_operations"));
            if (table.ContainsKey("API_version_major"))
                API_version_major = Marshalling.ParseLong(table, "API_version_major");
            if (table.ContainsKey("API_version_minor"))
                API_version_minor = Marshalling.ParseLong(table, "API_version_minor");
            if (table.ContainsKey("API_version_vendor"))
                API_version_vendor = Marshalling.ParseString(table, "API_version_vendor");
            if (table.ContainsKey("API_version_vendor_implementation"))
                API_version_vendor_implementation = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "API_version_vendor_implementation"));
            if (table.ContainsKey("enabled"))
                enabled = Marshalling.ParseBool(table, "enabled");
            if (table.ContainsKey("software_version"))
                software_version = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "software_version"));
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("capabilities"))
                capabilities = Marshalling.ParseStringArray(table, "capabilities");
            if (table.ContainsKey("cpu_configuration"))
                cpu_configuration = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "cpu_configuration"));
            if (table.ContainsKey("sched_policy"))
                sched_policy = Marshalling.ParseString(table, "sched_policy");
            if (table.ContainsKey("supported_bootloaders"))
                supported_bootloaders = Marshalling.ParseStringArray(table, "supported_bootloaders");
            if (table.ContainsKey("resident_VMs"))
                resident_VMs = Marshalling.ParseSetRef<VM>(table, "resident_VMs");
            if (table.ContainsKey("logging"))
                logging = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "logging"));
            if (table.ContainsKey("PIFs"))
                PIFs = Marshalling.ParseSetRef<PIF>(table, "PIFs");
            if (table.ContainsKey("suspend_image_sr"))
                suspend_image_sr = Marshalling.ParseRef<SR>(table, "suspend_image_sr");
            if (table.ContainsKey("crash_dump_sr"))
                crash_dump_sr = Marshalling.ParseRef<SR>(table, "crash_dump_sr");
            if (table.ContainsKey("crashdumps"))
                crashdumps = Marshalling.ParseSetRef<Host_crashdump>(table, "crashdumps");
            if (table.ContainsKey("patches"))
                patches = Marshalling.ParseSetRef<Host_patch>(table, "patches");
            if (table.ContainsKey("updates"))
                updates = Marshalling.ParseSetRef<Pool_update>(table, "updates");
            if (table.ContainsKey("PBDs"))
                PBDs = Marshalling.ParseSetRef<PBD>(table, "PBDs");
            if (table.ContainsKey("host_CPUs"))
                host_CPUs = Marshalling.ParseSetRef<Host_cpu>(table, "host_CPUs");
            if (table.ContainsKey("cpu_info"))
                cpu_info = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "cpu_info"));
            if (table.ContainsKey("hostname"))
                hostname = Marshalling.ParseString(table, "hostname");
            if (table.ContainsKey("address"))
                address = Marshalling.ParseString(table, "address");
            if (table.ContainsKey("metrics"))
                metrics = Marshalling.ParseRef<Host_metrics>(table, "metrics");
            if (table.ContainsKey("license_params"))
                license_params = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "license_params"));
            if (table.ContainsKey("ha_statefiles"))
                ha_statefiles = Marshalling.ParseStringArray(table, "ha_statefiles");
            if (table.ContainsKey("ha_network_peers"))
                ha_network_peers = Marshalling.ParseStringArray(table, "ha_network_peers");
            if (table.ContainsKey("blobs"))
                blobs = Maps.ToDictionary_string_XenRefBlob(Marshalling.ParseHashTable(table, "blobs"));
            if (table.ContainsKey("tags"))
                tags = Marshalling.ParseStringArray(table, "tags");
            if (table.ContainsKey("external_auth_type"))
                external_auth_type = Marshalling.ParseString(table, "external_auth_type");
            if (table.ContainsKey("external_auth_service_name"))
                external_auth_service_name = Marshalling.ParseString(table, "external_auth_service_name");
            if (table.ContainsKey("external_auth_configuration"))
                external_auth_configuration = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "external_auth_configuration"));
            if (table.ContainsKey("edition"))
                edition = Marshalling.ParseString(table, "edition");
            if (table.ContainsKey("license_server"))
                license_server = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "license_server"));
            if (table.ContainsKey("bios_strings"))
                bios_strings = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "bios_strings"));
            if (table.ContainsKey("power_on_mode"))
                power_on_mode = Marshalling.ParseString(table, "power_on_mode");
            if (table.ContainsKey("power_on_config"))
                power_on_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "power_on_config"));
            if (table.ContainsKey("local_cache_sr"))
                local_cache_sr = Marshalling.ParseRef<SR>(table, "local_cache_sr");
            if (table.ContainsKey("chipset_info"))
                chipset_info = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "chipset_info"));
            if (table.ContainsKey("PCIs"))
                PCIs = Marshalling.ParseSetRef<PCI>(table, "PCIs");
            if (table.ContainsKey("PGPUs"))
                PGPUs = Marshalling.ParseSetRef<PGPU>(table, "PGPUs");
            if (table.ContainsKey("PUSBs"))
                PUSBs = Marshalling.ParseSetRef<PUSB>(table, "PUSBs");
            if (table.ContainsKey("ssl_legacy"))
                ssl_legacy = Marshalling.ParseBool(table, "ssl_legacy");
            if (table.ContainsKey("guest_VCPUs_params"))
                guest_VCPUs_params = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "guest_VCPUs_params"));
            if (table.ContainsKey("display"))
                display = (host_display)Helper.EnumParseDefault(typeof(host_display), Marshalling.ParseString(table, "display"));
            if (table.ContainsKey("virtual_hardware_platform_versions"))
                virtual_hardware_platform_versions = Marshalling.ParseLongArray(table, "virtual_hardware_platform_versions");
            if (table.ContainsKey("control_domain"))
                control_domain = Marshalling.ParseRef<VM>(table, "control_domain");
            if (table.ContainsKey("updates_requiring_reboot"))
                updates_requiring_reboot = Marshalling.ParseSetRef<Pool_update>(table, "updates_requiring_reboot");
            if (table.ContainsKey("features"))
                features = Marshalling.ParseSetRef<Feature>(table, "features");
            if (table.ContainsKey("iscsi_iqn"))
                iscsi_iqn = Marshalling.ParseString(table, "iscsi_iqn");
            if (table.ContainsKey("multipathing"))
                multipathing = Marshalling.ParseBool(table, "multipathing");
            if (table.ContainsKey("uefi_certificates"))
                uefi_certificates = Marshalling.ParseString(table, "uefi_certificates");
            if (table.ContainsKey("certificates"))
                certificates = Marshalling.ParseSetRef<Certificate>(table, "certificates");
            if (table.ContainsKey("editions"))
                editions = Marshalling.ParseStringArray(table, "editions");
            if (table.ContainsKey("pending_guidances"))
                pending_guidances = Helper.StringArrayToEnumList<update_guidances>(Marshalling.ParseStringArray(table, "pending_guidances"));
            if (table.ContainsKey("tls_verification_enabled"))
                tls_verification_enabled = Marshalling.ParseBool(table, "tls_verification_enabled");
            if (table.ContainsKey("last_software_update"))
                last_software_update = Marshalling.ParseDateTime(table, "last_software_update");
            if (table.ContainsKey("https_only"))
                https_only = Marshalling.ParseBool(table, "https_only");
            if (table.ContainsKey("latest_synced_updates_applied"))
                latest_synced_updates_applied = (latest_synced_updates_applied_state)Helper.EnumParseDefault(typeof(latest_synced_updates_applied_state), Marshalling.ParseString(table, "latest_synced_updates_applied"));
            if (table.ContainsKey("numa_affinity_policy"))
                numa_affinity_policy = (host_numa_affinity_policy)Helper.EnumParseDefault(typeof(host_numa_affinity_policy), Marshalling.ParseString(table, "numa_affinity_policy"));
            if (table.ContainsKey("pending_guidances_recommended"))
                pending_guidances_recommended = Helper.StringArrayToEnumList<update_guidances>(Marshalling.ParseStringArray(table, "pending_guidances_recommended"));
            if (table.ContainsKey("pending_guidances_full"))
                pending_guidances_full = Helper.StringArrayToEnumList<update_guidances>(Marshalling.ParseStringArray(table, "pending_guidances_full"));
            if (table.ContainsKey("last_update_hash"))
                last_update_hash = Marshalling.ParseString(table, "last_update_hash");
            if (table.ContainsKey("ssh_enabled"))
                ssh_enabled = Marshalling.ParseBool(table, "ssh_enabled");
            if (table.ContainsKey("ssh_enabled_timeout"))
                ssh_enabled_timeout = Marshalling.ParseLong(table, "ssh_enabled_timeout");
            if (table.ContainsKey("ssh_expiry"))
                ssh_expiry = Marshalling.ParseDateTime(table, "ssh_expiry");
            if (table.ContainsKey("console_idle_timeout"))
                console_idle_timeout = Marshalling.ParseLong(table, "console_idle_timeout");
            if (table.ContainsKey("ssh_auto_mode"))
                ssh_auto_mode = Marshalling.ParseBool(table, "ssh_auto_mode");
            if (table.ContainsKey("max_cstate"))
                max_cstate = Marshalling.ParseString(table, "max_cstate");
            if (table.ContainsKey("secure_boot"))
                secure_boot = Marshalling.ParseBool(table, "secure_boot");
            if (table.ContainsKey("ntp_mode"))
                ntp_mode = (host_ntp_mode)Helper.EnumParseDefault(typeof(host_ntp_mode), Marshalling.ParseString(table, "ntp_mode"));
            if (table.ContainsKey("ntp_custom_servers"))
                ntp_custom_servers = Marshalling.ParseStringArray(table, "ntp_custom_servers");
            if (table.ContainsKey("timezone"))
                timezone = Marshalling.ParseString(table, "timezone");
        }

        public bool DeepEquals(Host other, bool ignoreCurrentOperations)
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
                Helper.AreEqual2(_memory_overhead, other._memory_overhead) &&
                Helper.AreEqual2(_allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(_API_version_major, other._API_version_major) &&
                Helper.AreEqual2(_API_version_minor, other._API_version_minor) &&
                Helper.AreEqual2(_API_version_vendor, other._API_version_vendor) &&
                Helper.AreEqual2(_API_version_vendor_implementation, other._API_version_vendor_implementation) &&
                Helper.AreEqual2(_enabled, other._enabled) &&
                Helper.AreEqual2(_software_version, other._software_version) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_capabilities, other._capabilities) &&
                Helper.AreEqual2(_cpu_configuration, other._cpu_configuration) &&
                Helper.AreEqual2(_sched_policy, other._sched_policy) &&
                Helper.AreEqual2(_supported_bootloaders, other._supported_bootloaders) &&
                Helper.AreEqual2(_resident_VMs, other._resident_VMs) &&
                Helper.AreEqual2(_logging, other._logging) &&
                Helper.AreEqual2(_PIFs, other._PIFs) &&
                Helper.AreEqual2(_suspend_image_sr, other._suspend_image_sr) &&
                Helper.AreEqual2(_crash_dump_sr, other._crash_dump_sr) &&
                Helper.AreEqual2(_crashdumps, other._crashdumps) &&
                Helper.AreEqual2(_patches, other._patches) &&
                Helper.AreEqual2(_updates, other._updates) &&
                Helper.AreEqual2(_PBDs, other._PBDs) &&
                Helper.AreEqual2(_host_CPUs, other._host_CPUs) &&
                Helper.AreEqual2(_cpu_info, other._cpu_info) &&
                Helper.AreEqual2(_hostname, other._hostname) &&
                Helper.AreEqual2(_address, other._address) &&
                Helper.AreEqual2(_metrics, other._metrics) &&
                Helper.AreEqual2(_license_params, other._license_params) &&
                Helper.AreEqual2(_ha_statefiles, other._ha_statefiles) &&
                Helper.AreEqual2(_ha_network_peers, other._ha_network_peers) &&
                Helper.AreEqual2(_blobs, other._blobs) &&
                Helper.AreEqual2(_tags, other._tags) &&
                Helper.AreEqual2(_external_auth_type, other._external_auth_type) &&
                Helper.AreEqual2(_external_auth_service_name, other._external_auth_service_name) &&
                Helper.AreEqual2(_external_auth_configuration, other._external_auth_configuration) &&
                Helper.AreEqual2(_edition, other._edition) &&
                Helper.AreEqual2(_license_server, other._license_server) &&
                Helper.AreEqual2(_bios_strings, other._bios_strings) &&
                Helper.AreEqual2(_power_on_mode, other._power_on_mode) &&
                Helper.AreEqual2(_power_on_config, other._power_on_config) &&
                Helper.AreEqual2(_local_cache_sr, other._local_cache_sr) &&
                Helper.AreEqual2(_chipset_info, other._chipset_info) &&
                Helper.AreEqual2(_PCIs, other._PCIs) &&
                Helper.AreEqual2(_PGPUs, other._PGPUs) &&
                Helper.AreEqual2(_PUSBs, other._PUSBs) &&
                Helper.AreEqual2(_ssl_legacy, other._ssl_legacy) &&
                Helper.AreEqual2(_guest_VCPUs_params, other._guest_VCPUs_params) &&
                Helper.AreEqual2(_display, other._display) &&
                Helper.AreEqual2(_virtual_hardware_platform_versions, other._virtual_hardware_platform_versions) &&
                Helper.AreEqual2(_control_domain, other._control_domain) &&
                Helper.AreEqual2(_updates_requiring_reboot, other._updates_requiring_reboot) &&
                Helper.AreEqual2(_features, other._features) &&
                Helper.AreEqual2(_iscsi_iqn, other._iscsi_iqn) &&
                Helper.AreEqual2(_multipathing, other._multipathing) &&
                Helper.AreEqual2(_uefi_certificates, other._uefi_certificates) &&
                Helper.AreEqual2(_certificates, other._certificates) &&
                Helper.AreEqual2(_editions, other._editions) &&
                Helper.AreEqual2(_pending_guidances, other._pending_guidances) &&
                Helper.AreEqual2(_tls_verification_enabled, other._tls_verification_enabled) &&
                Helper.AreEqual2(_last_software_update, other._last_software_update) &&
                Helper.AreEqual2(_https_only, other._https_only) &&
                Helper.AreEqual2(_latest_synced_updates_applied, other._latest_synced_updates_applied) &&
                Helper.AreEqual2(_numa_affinity_policy, other._numa_affinity_policy) &&
                Helper.AreEqual2(_pending_guidances_recommended, other._pending_guidances_recommended) &&
                Helper.AreEqual2(_pending_guidances_full, other._pending_guidances_full) &&
                Helper.AreEqual2(_last_update_hash, other._last_update_hash) &&
                Helper.AreEqual2(_ssh_enabled, other._ssh_enabled) &&
                Helper.AreEqual2(_ssh_enabled_timeout, other._ssh_enabled_timeout) &&
                Helper.AreEqual2(_ssh_expiry, other._ssh_expiry) &&
                Helper.AreEqual2(_console_idle_timeout, other._console_idle_timeout) &&
                Helper.AreEqual2(_ssh_auto_mode, other._ssh_auto_mode) &&
                Helper.AreEqual2(_max_cstate, other._max_cstate) &&
                Helper.AreEqual2(_secure_boot, other._secure_boot) &&
                Helper.AreEqual2(_ntp_mode, other._ntp_mode) &&
                Helper.AreEqual2(_ntp_custom_servers, other._ntp_custom_servers) &&
                Helper.AreEqual2(_timezone, other._timezone);
        }


        /// <summary>
        /// Get a record containing the current state of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Host get_record(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_record(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get a reference to the host instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.host_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get all the host instances with the given label.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Host>> get_by_name_label(Session session, string _label)
        {
            return session.JsonRpcClient.host_get_by_name_label(session.opaque_ref, _label);
        }

        /// <summary>
        /// Get the uuid field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_uuid(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the name/label field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_label(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_name_label(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the name/description field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_description(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_name_description(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the memory/overhead field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_memory_overhead(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_memory_overhead(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the allowed_operations field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<host_allowed_operations> get_allowed_operations(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_allowed_operations(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the current_operations field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, host_allowed_operations> get_current_operations(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_current_operations(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the API_version/major field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_API_version_major(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_api_version_major(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the API_version/minor field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_API_version_minor(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_api_version_minor(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the API_version/vendor field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_API_version_vendor(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_api_version_vendor(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the API_version/vendor_implementation field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_API_version_vendor_implementation(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_api_version_vendor_implementation(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the enabled field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_enabled(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_enabled(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the software_version field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_software_version(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_software_version(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the other_config field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_other_config(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the capabilities field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_capabilities(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_capabilities(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the cpu_configuration field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_cpu_configuration(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_cpu_configuration(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the sched_policy field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_sched_policy(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_sched_policy(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the supported_bootloaders field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_supported_bootloaders(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_supported_bootloaders(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the resident_VMs field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VM>> get_resident_VMs(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_resident_vms(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the logging field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_logging(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_logging(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the PIFs field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PIF>> get_PIFs(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_pifs(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the suspend_image_sr field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<SR> get_suspend_image_sr(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_suspend_image_sr(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the crash_dump_sr field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<SR> get_crash_dump_sr(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_crash_dump_sr(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the crashdumps field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Host_crashdump>> get_crashdumps(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_crashdumps(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the patches field of the given host.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Host_patch>> get_patches(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_patches(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the updates field of the given host.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Pool_update>> get_updates(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_updates(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the PBDs field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PBD>> get_PBDs(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_pbds(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the host_CPUs field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Host_cpu>> get_host_CPUs(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_host_cpus(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the cpu_info field of the given host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_cpu_info(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_cpu_info(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the hostname field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_hostname(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_hostname(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the address field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_address(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_address(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the metrics field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host_metrics> get_metrics(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_metrics(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the license_params field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_license_params(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_license_params(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the ha_statefiles field of the given host.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_ha_statefiles(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_ha_statefiles(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the ha_network_peers field of the given host.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_ha_network_peers(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_ha_network_peers(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the blobs field of the given host.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, XenRef<Blob>> get_blobs(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_blobs(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the tags field of the given host.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_tags(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_tags(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the external_auth_type field of the given host.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_external_auth_type(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_external_auth_type(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the external_auth_service_name field of the given host.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_external_auth_service_name(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_external_auth_service_name(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the external_auth_configuration field of the given host.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_external_auth_configuration(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_external_auth_configuration(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the edition field of the given host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_edition(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_edition(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the license_server field of the given host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_license_server(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_license_server(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the bios_strings field of the given host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_bios_strings(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_bios_strings(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the power_on_mode field of the given host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_power_on_mode(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_power_on_mode(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the power_on_config field of the given host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_power_on_config(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_power_on_config(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the local_cache_sr field of the given host.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<SR> get_local_cache_sr(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_local_cache_sr(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the chipset_info field of the given host.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_chipset_info(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_chipset_info(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the PCIs field of the given host.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PCI>> get_PCIs(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_pcis(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the PGPUs field of the given host.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PGPU>> get_PGPUs(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_pgpus(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the PUSBs field of the given host.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PUSB>> get_PUSBs(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_pusbs(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the ssl_legacy field of the given host.
        /// First published in XenServer 7.0.
        /// Deprecated since Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        [Deprecated("Citrix Hypervisor 8.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_ssl_legacy(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_ssl_legacy(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the guest_VCPUs_params field of the given host.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_guest_VCPUs_params(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_guest_vcpus_params(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the display field of the given host.
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static host_display get_display(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_display(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the virtual_hardware_platform_versions field of the given host.
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long[] get_virtual_hardware_platform_versions(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_virtual_hardware_platform_versions(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the control_domain field of the given host.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VM> get_control_domain(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_control_domain(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the updates_requiring_reboot field of the given host.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Pool_update>> get_updates_requiring_reboot(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_updates_requiring_reboot(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the features field of the given host.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Feature>> get_features(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_features(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the iscsi_iqn field of the given host.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_iscsi_iqn(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_iscsi_iqn(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the multipathing field of the given host.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_multipathing(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_multipathing(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the uefi_certificates field of the given host.
        /// First published in Citrix Hypervisor 8.1.
        /// Deprecated since 22.16.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        [Deprecated("22.16.0")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uefi_certificates(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_uefi_certificates(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the certificates field of the given host.
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Certificate>> get_certificates(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_certificates(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the editions field of the given host.
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_editions(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_editions(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the pending_guidances field of the given host.
        /// First published in 1.303.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<update_guidances> get_pending_guidances(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_pending_guidances(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the tls_verification_enabled field of the given host.
        /// First published in 1.313.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_tls_verification_enabled(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_tls_verification_enabled(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the last_software_update field of the given host.
        /// Experimental. First published in 22.20.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_last_software_update(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_last_software_update(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the https_only field of the given host.
        /// Experimental. First published in 22.27.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_https_only(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_https_only(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the latest_synced_updates_applied field of the given host.
        /// Experimental. First published in 23.18.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static latest_synced_updates_applied_state get_latest_synced_updates_applied(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_latest_synced_updates_applied(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the numa_affinity_policy field of the given host.
        /// Experimental. First published in 24.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static host_numa_affinity_policy get_numa_affinity_policy(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_numa_affinity_policy(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the pending_guidances_recommended field of the given host.
        /// Experimental. First published in 24.10.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<update_guidances> get_pending_guidances_recommended(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_pending_guidances_recommended(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the pending_guidances_full field of the given host.
        /// Experimental. First published in 24.10.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<update_guidances> get_pending_guidances_full(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_pending_guidances_full(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the last_update_hash field of the given host.
        /// Experimental. First published in 24.10.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_last_update_hash(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_last_update_hash(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the ssh_enabled field of the given host.
        /// Experimental. First published in 25.21.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_ssh_enabled(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_ssh_enabled(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the ssh_enabled_timeout field of the given host.
        /// Experimental. First published in 25.21.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_ssh_enabled_timeout(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_ssh_enabled_timeout(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the ssh_expiry field of the given host.
        /// Experimental. First published in 25.21.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_ssh_expiry(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_ssh_expiry(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the console_idle_timeout field of the given host.
        /// Experimental. First published in 25.21.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_console_idle_timeout(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_console_idle_timeout(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the ssh_auto_mode field of the given host.
        /// Experimental. First published in 25.27.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_ssh_auto_mode(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_ssh_auto_mode(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the max_cstate field of the given host.
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_max_cstate(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_max_cstate(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the secure_boot field of the given host.
        /// Experimental. First published in 25.31.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_secure_boot(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_secure_boot(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the ntp_mode field of the given host.
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static host_ntp_mode get_ntp_mode(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_ntp_mode(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the ntp_custom_servers field of the given host.
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_ntp_custom_servers(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_ntp_custom_servers(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the timezone field of the given host.
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_timezone(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_timezone(session.opaque_ref, _host);
        }

        /// <summary>
        /// Set the name/label field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_label">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_name_label(Session session, string _host, string _label)
        {
            session.JsonRpcClient.host_set_name_label(session.opaque_ref, _host, _label);
        }

        /// <summary>
        /// Set the name/description field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_description">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_name_description(Session session, string _host, string _description)
        {
            session.JsonRpcClient.host_set_name_description(session.opaque_ref, _host, _description);
        }

        /// <summary>
        /// Set the other_config field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_other_config(Session session, string _host, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.host_set_other_config(session.opaque_ref, _host, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_other_config(Session session, string _host, string _key, string _value)
        {
            session.JsonRpcClient.host_add_to_other_config(session.opaque_ref, _host, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given host.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_other_config(Session session, string _host, string _key)
        {
            session.JsonRpcClient.host_remove_from_other_config(session.opaque_ref, _host, _key);
        }

        /// <summary>
        /// Set the logging field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_logging">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_logging(Session session, string _host, Dictionary<string, string> _logging)
        {
            session.JsonRpcClient.host_set_logging(session.opaque_ref, _host, _logging);
        }

        /// <summary>
        /// Add the given key-value pair to the logging field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_logging(Session session, string _host, string _key, string _value)
        {
            session.JsonRpcClient.host_add_to_logging(session.opaque_ref, _host, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the logging field of the given host.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_logging(Session session, string _host, string _key)
        {
            session.JsonRpcClient.host_remove_from_logging(session.opaque_ref, _host, _key);
        }

        /// <summary>
        /// Set the suspend_image_sr field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_suspend_image_sr">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_suspend_image_sr(Session session, string _host, string _suspend_image_sr)
        {
            session.JsonRpcClient.host_set_suspend_image_sr(session.opaque_ref, _host, _suspend_image_sr);
        }

        /// <summary>
        /// Set the crash_dump_sr field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_crash_dump_sr">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_crash_dump_sr(Session session, string _host, string _crash_dump_sr)
        {
            session.JsonRpcClient.host_set_crash_dump_sr(session.opaque_ref, _host, _crash_dump_sr);
        }

        /// <summary>
        /// Set the hostname field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_hostname">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_hostname(Session session, string _host, string _hostname)
        {
            session.JsonRpcClient.host_set_hostname(session.opaque_ref, _host, _hostname);
        }

        /// <summary>
        /// Set the address field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_address">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_address(Session session, string _host, string _address)
        {
            session.JsonRpcClient.host_set_address(session.opaque_ref, _host, _address);
        }

        /// <summary>
        /// Set the tags field of the given host.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_tags">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void set_tags(Session session, string _host, string[] _tags)
        {
            session.JsonRpcClient.host_set_tags(session.opaque_ref, _host, _tags);
        }

        /// <summary>
        /// Add the given value to the tags field of the given host.  If the value is already in that Set, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">New value to add</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void add_tags(Session session, string _host, string _value)
        {
            session.JsonRpcClient.host_add_tags(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Remove the given value from the tags field of the given host.  If the value is not in that Set, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">Value to remove</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void remove_tags(Session session, string _host, string _value)
        {
            session.JsonRpcClient.host_remove_tags(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Set the license_server field of the given host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_license_server">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_license_server(Session session, string _host, Dictionary<string, string> _license_server)
        {
            session.JsonRpcClient.host_set_license_server(session.opaque_ref, _host, _license_server);
        }

        /// <summary>
        /// Add the given key-value pair to the license_server field of the given host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_license_server(Session session, string _host, string _key, string _value)
        {
            session.JsonRpcClient.host_add_to_license_server(session.opaque_ref, _host, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the license_server field of the given host.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_license_server(Session session, string _host, string _key)
        {
            session.JsonRpcClient.host_remove_from_license_server(session.opaque_ref, _host, _key);
        }

        /// <summary>
        /// Set the guest_VCPUs_params field of the given host.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_guest_vcpus_params">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_guest_VCPUs_params(Session session, string _host, Dictionary<string, string> _guest_vcpus_params)
        {
            session.JsonRpcClient.host_set_guest_vcpus_params(session.opaque_ref, _host, _guest_vcpus_params);
        }

        /// <summary>
        /// Add the given key-value pair to the guest_VCPUs_params field of the given host.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_guest_VCPUs_params(Session session, string _host, string _key, string _value)
        {
            session.JsonRpcClient.host_add_to_guest_vcpus_params(session.opaque_ref, _host, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the guest_VCPUs_params field of the given host.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_guest_VCPUs_params(Session session, string _host, string _key)
        {
            session.JsonRpcClient.host_remove_from_guest_vcpus_params(session.opaque_ref, _host, _key);
        }

        /// <summary>
        /// Set the display field of the given host.
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_display">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_display(Session session, string _host, host_display _display)
        {
            session.JsonRpcClient.host_set_display(session.opaque_ref, _host, _display);
        }

        /// <summary>
        /// Puts the host into a state in which no new VMs can be started. Currently active VMs on the host continue to execute.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void disable(Session session, string _host)
        {
            session.JsonRpcClient.host_disable(session.opaque_ref, _host);
        }

        /// <summary>
        /// Puts the host into a state in which no new VMs can be started. Currently active VMs on the host continue to execute.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_disable(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_disable(session.opaque_ref, _host);
        }

        /// <summary>
        /// Puts the host into a state in which no new VMs can be started. Currently active VMs on the host continue to execute.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_auto_enable">If true (default), the host will be re-enabled after a toolstack restart automatically. If false, the host will be disabled indefinitely, across toolstack restarts and host reboots, until re-enabled explicitly with Host.enable. First published in 25.31.0.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void disable(Session session, string _host, bool _auto_enable)
        {
            session.JsonRpcClient.host_disable(session.opaque_ref, _host, _auto_enable);
        }

        /// <summary>
        /// Puts the host into a state in which no new VMs can be started. Currently active VMs on the host continue to execute.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_auto_enable">If true (default), the host will be re-enabled after a toolstack restart automatically. If false, the host will be disabled indefinitely, across toolstack restarts and host reboots, until re-enabled explicitly with Host.enable. First published in 25.31.0.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_disable(Session session, string _host, bool _auto_enable)
        {
          return session.JsonRpcClient.async_host_disable(session.opaque_ref, _host, _auto_enable);
        }

        /// <summary>
        /// Puts the host into a state in which new VMs can be started.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void enable(Session session, string _host)
        {
            session.JsonRpcClient.host_enable(session.opaque_ref, _host);
        }

        /// <summary>
        /// Puts the host into a state in which new VMs can be started.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_enable(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_enable(session.opaque_ref, _host);
        }

        /// <summary>
        /// Shutdown the host. (This function can only be called if there are no currently running VMs on the host and it is disabled.)
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void shutdown(Session session, string _host)
        {
            session.JsonRpcClient.host_shutdown(session.opaque_ref, _host);
        }

        /// <summary>
        /// Shutdown the host. (This function can only be called if there are no currently running VMs on the host and it is disabled.)
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_shutdown(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_shutdown(session.opaque_ref, _host);
        }

        /// <summary>
        /// Reboot the host. (This function can only be called if there are no currently running VMs on the host and it is disabled.)
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void reboot(Session session, string _host)
        {
            session.JsonRpcClient.host_reboot(session.opaque_ref, _host);
        }

        /// <summary>
        /// Reboot the host. (This function can only be called if there are no currently running VMs on the host and it is disabled.)
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_reboot(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_reboot(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the host xen dmesg.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static string dmesg(Session session, string _host)
        {
            return session.JsonRpcClient.host_dmesg(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the host xen dmesg.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_dmesg(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_dmesg(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the host xen dmesg, and clear the buffer.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static string dmesg_clear(Session session, string _host)
        {
            return session.JsonRpcClient.host_dmesg_clear(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the host xen dmesg, and clear the buffer.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_dmesg_clear(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_dmesg_clear(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the host's log file
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_log(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_log(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the host's log file
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_get_log(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_get_log(session.opaque_ref, _host);
        }

        /// <summary>
        /// Inject the given string as debugging keys into Xen
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_keys">The keys to send</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void send_debug_keys(Session session, string _host, string _keys)
        {
            session.JsonRpcClient.host_send_debug_keys(session.opaque_ref, _host, _keys);
        }

        /// <summary>
        /// Inject the given string as debugging keys into Xen
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_keys">The keys to send</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_send_debug_keys(Session session, string _host, string _keys)
        {
          return session.JsonRpcClient.async_host_send_debug_keys(session.opaque_ref, _host, _keys);
        }

        /// <summary>
        /// Run xen-bugtool --yestoall and upload the output to support
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_url">The URL to upload to</param>
        /// <param name="_options">Extra configuration operations</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void bugreport_upload(Session session, string _host, string _url, Dictionary<string, string> _options)
        {
            session.JsonRpcClient.host_bugreport_upload(session.opaque_ref, _host, _url, _options);
        }

        /// <summary>
        /// Run xen-bugtool --yestoall and upload the output to support
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_url">The URL to upload to</param>
        /// <param name="_options">Extra configuration operations</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_bugreport_upload(Session session, string _host, string _url, Dictionary<string, string> _options)
        {
          return session.JsonRpcClient.async_host_bugreport_upload(session.opaque_ref, _host, _url, _options);
        }

        /// <summary>
        /// List all supported methods
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] list_methods(Session session)
        {
            return session.JsonRpcClient.host_list_methods(session.opaque_ref);
        }

        /// <summary>
        /// Apply a new license to a host
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_contents">The contents of the license file, base64 encoded</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void license_apply(Session session, string _host, string _contents)
        {
            session.JsonRpcClient.host_license_apply(session.opaque_ref, _host, _contents);
        }

        /// <summary>
        /// Apply a new license to a host
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_contents">The contents of the license file, base64 encoded</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_license_apply(Session session, string _host, string _contents)
        {
          return session.JsonRpcClient.async_host_license_apply(session.opaque_ref, _host, _contents);
        }

        /// <summary>
        /// Apply a new license to a host
        /// First published in XenServer 6.5 SP1 Hotfix 31.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_contents">The contents of the license file, base64 encoded</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void license_add(Session session, string _host, string _contents)
        {
            session.JsonRpcClient.host_license_add(session.opaque_ref, _host, _contents);
        }

        /// <summary>
        /// Apply a new license to a host
        /// First published in XenServer 6.5 SP1 Hotfix 31.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_contents">The contents of the license file, base64 encoded</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_license_add(Session session, string _host, string _contents)
        {
          return session.JsonRpcClient.async_host_license_add(session.opaque_ref, _host, _contents);
        }

        /// <summary>
        /// Remove any license file from the specified host, and switch that host to the unlicensed edition
        /// First published in XenServer 6.5 SP1 Hotfix 31.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void license_remove(Session session, string _host)
        {
            session.JsonRpcClient.host_license_remove(session.opaque_ref, _host);
        }

        /// <summary>
        /// Remove any license file from the specified host, and switch that host to the unlicensed edition
        /// First published in XenServer 6.5 SP1 Hotfix 31.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_license_remove(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_license_remove(session.opaque_ref, _host);
        }

        /// <summary>
        /// Destroy specified host record in database
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _host)
        {
            session.JsonRpcClient.host_destroy(session.opaque_ref, _host);
        }

        /// <summary>
        /// Destroy specified host record in database
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_destroy(session.opaque_ref, _host);
        }

        /// <summary>
        /// Attempt to power-on the host (if the capability exists).
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void power_on(Session session, string _host)
        {
            session.JsonRpcClient.host_power_on(session.opaque_ref, _host);
        }

        /// <summary>
        /// Attempt to power-on the host (if the capability exists).
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_power_on(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_power_on(session.opaque_ref, _host);
        }

        /// <summary>
        /// This call disables HA on the local host. This should only be used with extreme care.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_soft">Disable HA temporarily, revert upon host reboot or further changes, idempotent First published in XenServer 7.1.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void emergency_ha_disable(Session session, bool _soft)
        {
            session.JsonRpcClient.host_emergency_ha_disable(session.opaque_ref, _soft);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<Data_source> get_data_sources(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_data_sources(session.opaque_ref, _host);
        }

        /// <summary>
        /// Start recording the specified data source
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_data_source">The data source to record</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void record_data_source(Session session, string _host, string _data_source)
        {
            session.JsonRpcClient.host_record_data_source(session.opaque_ref, _host, _data_source);
        }

        /// <summary>
        /// Query the latest value of the specified data source
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_data_source">The data source to query</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static double query_data_source(Session session, string _host, string _data_source)
        {
            return session.JsonRpcClient.host_query_data_source(session.opaque_ref, _host, _data_source);
        }

        /// <summary>
        /// Forget the recorded statistics related to the specified data source
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_data_source">The data source whose archives are to be forgotten</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void forget_data_source_archives(Session session, string _host, string _data_source)
        {
            session.JsonRpcClient.host_forget_data_source_archives(session.opaque_ref, _host, _data_source);
        }

        /// <summary>
        /// Check this host can be evacuated.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void assert_can_evacuate(Session session, string _host)
        {
            session.JsonRpcClient.host_assert_can_evacuate(session.opaque_ref, _host);
        }

        /// <summary>
        /// Check this host can be evacuated.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_assert_can_evacuate(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_assert_can_evacuate(session.opaque_ref, _host);
        }

        /// <summary>
        /// Return a set of VMs which prevent the host being evacuated, with per-VM error codes
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<VM>, string[]> get_vms_which_prevent_evacuation(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_vms_which_prevent_evacuation(session.opaque_ref, _host);
        }

        /// <summary>
        /// Return a set of VMs which prevent the host being evacuated, with per-VM error codes
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_get_vms_which_prevent_evacuation(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_get_vms_which_prevent_evacuation(session.opaque_ref, _host);
        }

        /// <summary>
        /// Return a set of VMs which are not co-operating with the host's memory control system
        /// First published in XenServer 5.6.
        /// Deprecated since XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        [Deprecated("XenServer 6.1")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VM>> get_uncooperative_resident_VMs(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_uncooperative_resident_vms(session.opaque_ref, _host);
        }

        /// <summary>
        /// Return a set of VMs which are not co-operating with the host's memory control system
        /// First published in XenServer 5.6.
        /// Deprecated since XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        [Deprecated("XenServer 6.1")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_get_uncooperative_resident_VMs(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_get_uncooperative_resident_vms(session.opaque_ref, _host);
        }

        /// <summary>
        /// Migrate all VMs off of this host, where possible.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void evacuate(Session session, string _host)
        {
            session.JsonRpcClient.host_evacuate(session.opaque_ref, _host);
        }

        /// <summary>
        /// Migrate all VMs off of this host, where possible.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_evacuate(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_evacuate(session.opaque_ref, _host);
        }

        /// <summary>
        /// Migrate all VMs off of this host, where possible.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_network">Optional preferred network for migration First published in 1.297.0.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void evacuate(Session session, string _host, string _network)
        {
            session.JsonRpcClient.host_evacuate(session.opaque_ref, _host, _network);
        }

        /// <summary>
        /// Migrate all VMs off of this host, where possible.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_network">Optional preferred network for migration First published in 1.297.0.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_evacuate(Session session, string _host, string _network)
        {
          return session.JsonRpcClient.async_host_evacuate(session.opaque_ref, _host, _network);
        }

        /// <summary>
        /// Migrate all VMs off of this host, where possible.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_network">Optional preferred network for migration First published in 1.297.0.</param>
        /// <param name="_evacuate_batch_size">The maximum number of VMs to be migrated per batch 0 will use the value `evacuation-batch-size` defined in xapi.conf First published in 23.27.0.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void evacuate(Session session, string _host, string _network, long _evacuate_batch_size)
        {
            session.JsonRpcClient.host_evacuate(session.opaque_ref, _host, _network, _evacuate_batch_size);
        }

        /// <summary>
        /// Migrate all VMs off of this host, where possible.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_network">Optional preferred network for migration First published in 1.297.0.</param>
        /// <param name="_evacuate_batch_size">The maximum number of VMs to be migrated per batch 0 will use the value `evacuation-batch-size` defined in xapi.conf First published in 23.27.0.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_evacuate(Session session, string _host, string _network, long _evacuate_batch_size)
        {
          return session.JsonRpcClient.async_host_evacuate(session.opaque_ref, _host, _network, _evacuate_batch_size);
        }

        /// <summary>
        /// Re-configure syslog logging
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void syslog_reconfigure(Session session, string _host)
        {
            session.JsonRpcClient.host_syslog_reconfigure(session.opaque_ref, _host);
        }

        /// <summary>
        /// Re-configure syslog logging
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_syslog_reconfigure(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_syslog_reconfigure(session.opaque_ref, _host);
        }

        /// <summary>
        /// Reconfigure the management network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">reference to a PIF object corresponding to the management interface</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void management_reconfigure(Session session, string _pif)
        {
            session.JsonRpcClient.host_management_reconfigure(session.opaque_ref, _pif);
        }

        /// <summary>
        /// Reconfigure the management network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">reference to a PIF object corresponding to the management interface</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_management_reconfigure(Session session, string _pif)
        {
          return session.JsonRpcClient.async_host_management_reconfigure(session.opaque_ref, _pif);
        }

        /// <summary>
        /// Reconfigure the management network interface. Should only be used if Host.management_reconfigure is impossible because the network configuration is broken.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_interface">name of the interface to use as a management interface</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void local_management_reconfigure(Session session, string _interface)
        {
            session.JsonRpcClient.host_local_management_reconfigure(session.opaque_ref, _interface);
        }

        /// <summary>
        /// Disable the management network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void management_disable(Session session)
        {
            session.JsonRpcClient.host_management_disable(session.opaque_ref);
        }

        /// <summary>
        /// Returns the management interface for the specified host
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<PIF> get_management_interface(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_management_interface(session.opaque_ref, _host);
        }

        /// <summary>
        /// Returns the management interface for the specified host
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_get_management_interface(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_get_management_interface(session.opaque_ref, _host);
        }

        /// <summary>
        /// 
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_system_status_capabilities(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_system_status_capabilities(session.opaque_ref, _host);
        }

        /// <summary>
        /// Restarts the agent after a 10 second pause. WARNING: this is a dangerous operation. Any operations in progress will be aborted, and unrecoverable data loss may occur. The caller is responsible for ensuring that there are no operations in progress when this method is called.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void restart_agent(Session session, string _host)
        {
            session.JsonRpcClient.host_restart_agent(session.opaque_ref, _host);
        }

        /// <summary>
        /// Restarts the agent after a 10 second pause. WARNING: this is a dangerous operation. Any operations in progress will be aborted, and unrecoverable data loss may occur. The caller is responsible for ensuring that there are no operations in progress when this method is called.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_restart_agent(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_restart_agent(session.opaque_ref, _host);
        }

        /// <summary>
        /// Shuts the agent down after a 10 second pause. WARNING: this is a dangerous operation. Any operations in progress will be aborted, and unrecoverable data loss may occur. The caller is responsible for ensuring that there are no operations in progress when this method is called.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void shutdown_agent(Session session)
        {
            session.JsonRpcClient.host_shutdown_agent(session.opaque_ref);
        }

        /// <summary>
        /// Sets the host name to the specified string.  Both the API and lower-level system hostname are changed immediately.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_hostname">The new host name</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_hostname_live(Session session, string _host, string _hostname)
        {
            session.JsonRpcClient.host_set_hostname_live(session.opaque_ref, _host, _hostname);
        }

        /// <summary>
        /// Computes the amount of free memory on the host.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long compute_free_memory(Session session, string _host)
        {
            return session.JsonRpcClient.host_compute_free_memory(session.opaque_ref, _host);
        }

        /// <summary>
        /// Computes the amount of free memory on the host.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_compute_free_memory(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_compute_free_memory(session.opaque_ref, _host);
        }

        /// <summary>
        /// Computes the virtualization memory overhead of a host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long compute_memory_overhead(Session session, string _host)
        {
            return session.JsonRpcClient.host_compute_memory_overhead(session.opaque_ref, _host);
        }

        /// <summary>
        /// Computes the virtualization memory overhead of a host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_compute_memory_overhead(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_compute_memory_overhead(session.opaque_ref, _host);
        }

        /// <summary>
        /// This causes the synchronisation of the non-database data (messages, RRDs and so on) stored on the master to be synchronised with the host
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void sync_data(Session session, string _host)
        {
            session.JsonRpcClient.host_sync_data(session.opaque_ref, _host);
        }

        /// <summary>
        /// This causes the RRDs to be backed up to the master
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_delay">Delay in seconds from when the call is received to perform the backup</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void backup_rrds(Session session, string _host, double _delay)
        {
            session.JsonRpcClient.host_backup_rrds(session.opaque_ref, _host, _delay);
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this host
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Blob> create_new_blob(Session session, string _host, string _name, string _mime_type)
        {
            return session.JsonRpcClient.host_create_new_blob(session.opaque_ref, _host, _name, _mime_type);
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this host
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create_new_blob(Session session, string _host, string _name, string _mime_type)
        {
          return session.JsonRpcClient.async_host_create_new_blob(session.opaque_ref, _host, _name, _mime_type);
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this host
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <param name="_public">True if the blob should be publicly available First published in XenServer 6.1.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Blob> create_new_blob(Session session, string _host, string _name, string _mime_type, bool _public)
        {
            return session.JsonRpcClient.host_create_new_blob(session.opaque_ref, _host, _name, _mime_type, _public);
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this host
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <param name="_public">True if the blob should be publicly available First published in XenServer 6.1.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create_new_blob(Session session, string _host, string _name, string _mime_type, bool _public)
        {
          return session.JsonRpcClient.async_host_create_new_blob(session.opaque_ref, _host, _name, _mime_type, _public);
        }

        /// <summary>
        /// Call an API plugin on this host
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_plugin">The name of the plugin</param>
        /// <param name="_fn">The name of the function within the plugin</param>
        /// <param name="_args">Arguments for the function</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static string call_plugin(Session session, string _host, string _plugin, string _fn, Dictionary<string, string> _args)
        {
            return session.JsonRpcClient.host_call_plugin(session.opaque_ref, _host, _plugin, _fn, _args);
        }

        /// <summary>
        /// Call an API plugin on this host
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_plugin">The name of the plugin</param>
        /// <param name="_fn">The name of the function within the plugin</param>
        /// <param name="_args">Arguments for the function</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_call_plugin(Session session, string _host, string _plugin, string _fn, Dictionary<string, string> _args)
        {
          return session.JsonRpcClient.async_host_call_plugin(session.opaque_ref, _host, _plugin, _fn, _args);
        }

        /// <summary>
        /// Return true if the extension is available on the host
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_name">The name of the API call</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static bool has_extension(Session session, string _host, string _name)
        {
            return session.JsonRpcClient.host_has_extension(session.opaque_ref, _host, _name);
        }

        /// <summary>
        /// Return true if the extension is available on the host
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_name">The name of the API call</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_has_extension(Session session, string _host, string _name)
        {
          return session.JsonRpcClient.async_host_has_extension(session.opaque_ref, _host, _name);
        }

        /// <summary>
        /// Call an API extension on this host
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_call">Rpc call for the extension</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static string call_extension(Session session, string _host, string _call)
        {
            return session.JsonRpcClient.host_call_extension(session.opaque_ref, _host, _call);
        }

        /// <summary>
        /// This call queries the host's clock for the current time
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_servertime(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_servertime(session.opaque_ref, _host);
        }

        /// <summary>
        /// This call queries the host's clock for the current time in the host's local timezone
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_server_localtime(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_server_localtime(session.opaque_ref, _host);
        }

        /// <summary>
        /// This call enables external authentication on a host
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_config">A list of key-values containing the configuration data</param>
        /// <param name="_service_name">The name of the service</param>
        /// <param name="_auth_type">The type of authentication (for example, AD for Active Directory)</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void enable_external_auth(Session session, string _host, Dictionary<string, string> _config, string _service_name, string _auth_type)
        {
            session.JsonRpcClient.host_enable_external_auth(session.opaque_ref, _host, _config, _service_name, _auth_type);
        }

        /// <summary>
        /// This call disables external authentication on the local host
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_config">Optional parameters as a list of key-values containing the configuration data</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void disable_external_auth(Session session, string _host, Dictionary<string, string> _config)
        {
            session.JsonRpcClient.host_disable_external_auth(session.opaque_ref, _host, _config);
        }

        /// <summary>
        /// This call disables external authentication on the local host
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_config">Optional parameters as a list of key-values containing the configuration data</param>
        /// <param name="_force">Disable external auth even when not enabled First published in 26.2.0-next.</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void disable_external_auth(Session session, string _host, Dictionary<string, string> _config, bool _force)
        {
            session.JsonRpcClient.host_disable_external_auth(session.opaque_ref, _host, _config, _force);
        }

        /// <summary>
        /// This call enables or disables LDAPS for external authentication on the host
        /// First published in 26.16.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_ldaps">Whether to enable or disable LDAPS</param>
        /// <param name="_force">Force the operation even if already in the desired state</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void external_auth_set_ldaps(Session session, string _host, bool _ldaps, bool _force)
        {
            session.JsonRpcClient.host_external_auth_set_ldaps(session.opaque_ref, _host, _ldaps, _force);
        }

        /// <summary>
        /// Retrieves recommended host migrations to perform when evacuating the host from the wlb server. If a VM cannot be migrated from the host the reason is listed instead of a recommendation.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<VM>, string[]> retrieve_wlb_evacuate_recommendations(Session session, string _host)
        {
            return session.JsonRpcClient.host_retrieve_wlb_evacuate_recommendations(session.opaque_ref, _host);
        }

        /// <summary>
        /// Retrieves recommended host migrations to perform when evacuating the host from the wlb server. If a VM cannot be migrated from the host the reason is listed instead of a recommendation.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_retrieve_wlb_evacuate_recommendations(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_retrieve_wlb_evacuate_recommendations(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the installed server public TLS certificate.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_server_certificate(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_server_certificate(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the installed server public TLS certificate.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_get_server_certificate(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_get_server_certificate(session.opaque_ref, _host);
        }

        /// <summary>
        /// Replace the internal self-signed host certficate with a new one.
        /// First published in 1.307.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void refresh_server_certificate(Session session, string _host)
        {
            session.JsonRpcClient.host_refresh_server_certificate(session.opaque_ref, _host);
        }

        /// <summary>
        /// Replace the internal self-signed host certficate with a new one.
        /// First published in 1.307.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_refresh_server_certificate(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_refresh_server_certificate(session.opaque_ref, _host);
        }

        /// <summary>
        /// Install the TLS server certificate.
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_certificate">The server certificate, in PEM form</param>
        /// <param name="_private_key">The unencrypted private key used to sign the certificate, in PKCS#8 form</param>
        /// <param name="_certificate_chain">The certificate chain, in PEM form</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void install_server_certificate(Session session, string _host, string _certificate, string _private_key, string _certificate_chain)
        {
            session.JsonRpcClient.host_install_server_certificate(session.opaque_ref, _host, _certificate, _private_key, _certificate_chain);
        }

        /// <summary>
        /// Install the TLS server certificate.
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_certificate">The server certificate, in PEM form</param>
        /// <param name="_private_key">The unencrypted private key used to sign the certificate, in PKCS#8 form</param>
        /// <param name="_certificate_chain">The certificate chain, in PEM form</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_install_server_certificate(Session session, string _host, string _certificate, string _private_key, string _certificate_chain)
        {
          return session.JsonRpcClient.async_host_install_server_certificate(session.opaque_ref, _host, _certificate, _private_key, _certificate_chain);
        }

        /// <summary>
        /// Delete the current TLS server certificate and replace by a new, self-signed one. This should only be used with extreme care.
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: Not Applicable
        /// </remarks>
        public static void emergency_reset_server_certificate(Session session)
        {
            session.JsonRpcClient.host_emergency_reset_server_certificate(session.opaque_ref);
        }

        /// <summary>
        /// Delete the current TLS server certificate and replace by a new, self-signed one. This should only be used with extreme care.
        /// First published in 1.290.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void reset_server_certificate(Session session, string _host)
        {
            session.JsonRpcClient.host_reset_server_certificate(session.opaque_ref, _host);
        }

        /// <summary>
        /// Delete the current TLS server certificate and replace by a new, self-signed one. This should only be used with extreme care.
        /// First published in 1.290.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_reset_server_certificate(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_reset_server_certificate(session.opaque_ref, _host);
        }

        /// <summary>
        /// Change to another edition, or reactivate the current edition after a license has expired. This may be subject to the successful checkout of an appropriate license.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_edition">The requested edition</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void apply_edition(Session session, string _host, string _edition)
        {
            session.JsonRpcClient.host_apply_edition(session.opaque_ref, _host, _edition);
        }

        /// <summary>
        /// Change to another edition, or reactivate the current edition after a license has expired. This may be subject to the successful checkout of an appropriate license.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_edition">The requested edition</param>
        /// <param name="_force">Update the license params even if the apply call fails First published in XenServer 6.2.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void apply_edition(Session session, string _host, string _edition, bool _force)
        {
            session.JsonRpcClient.host_apply_edition(session.opaque_ref, _host, _edition, _force);
        }

        /// <summary>
        /// Refresh the list of installed Supplemental Packs.
        /// First published in XenServer 5.6.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void refresh_pack_info(Session session, string _host)
        {
            session.JsonRpcClient.host_refresh_pack_info(session.opaque_ref, _host);
        }

        /// <summary>
        /// Refresh the list of installed Supplemental Packs.
        /// First published in XenServer 5.6.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        [Deprecated("XenServer 7.1")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_refresh_pack_info(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_refresh_pack_info(session.opaque_ref, _host);
        }

        /// <summary>
        /// Set the power-on-mode, host, user and password
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_power_on_mode">power-on-mode can be empty, wake-on-lan, IPMI or other</param>
        /// <param name="_power_on_config">Power on config</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_power_on_mode(Session session, string _host, string _power_on_mode, Dictionary<string, string> _power_on_config)
        {
            session.JsonRpcClient.host_set_power_on_mode(session.opaque_ref, _host, _power_on_mode, _power_on_config);
        }

        /// <summary>
        /// Set the power-on-mode, host, user and password
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_power_on_mode">power-on-mode can be empty, wake-on-lan, IPMI or other</param>
        /// <param name="_power_on_config">Power on config</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_power_on_mode(Session session, string _host, string _power_on_mode, Dictionary<string, string> _power_on_config)
        {
          return session.JsonRpcClient.async_host_set_power_on_mode(session.opaque_ref, _host, _power_on_mode, _power_on_config);
        }

        /// <summary>
        /// Set the CPU features to be used after a reboot, if the given features string is valid.
        /// First published in XenServer 5.6.
        /// Deprecated since XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_features">The features string (32 hexadecimal digits)</param>
        [Deprecated("XenServer 7.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_cpu_features(Session session, string _host, string _features)
        {
            session.JsonRpcClient.host_set_cpu_features(session.opaque_ref, _host, _features);
        }

        /// <summary>
        /// Remove the feature mask, such that after a reboot all features of the CPU are enabled.
        /// First published in XenServer 5.6.
        /// Deprecated since XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        [Deprecated("XenServer 7.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void reset_cpu_features(Session session, string _host)
        {
            session.JsonRpcClient.host_reset_cpu_features(session.opaque_ref, _host);
        }

        /// <summary>
        /// Enable the use of a local SR for caching purposes
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_sr">The SR to use as a local cache</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void enable_local_storage_caching(Session session, string _host, string _sr)
        {
            session.JsonRpcClient.host_enable_local_storage_caching(session.opaque_ref, _host, _sr);
        }

        /// <summary>
        /// Disable the use of a local SR for caching purposes
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void disable_local_storage_caching(Session session, string _host)
        {
            session.JsonRpcClient.host_disable_local_storage_caching(session.opaque_ref, _host);
        }

        /// <summary>
        /// Prepare to receive a VM, returning a token which can be passed to VM.migrate.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_network">The network through which migration traffic should be received.</param>
        /// <param name="_options">Extra configuration operations</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static Dictionary<string, string> migrate_receive(Session session, string _host, string _network, Dictionary<string, string> _options)
        {
            return session.JsonRpcClient.host_migrate_receive(session.opaque_ref, _host, _network, _options);
        }

        /// <summary>
        /// Prepare to receive a VM, returning a token which can be passed to VM.migrate.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_network">The network through which migration traffic should be received.</param>
        /// <param name="_options">Extra configuration operations</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_migrate_receive(Session session, string _host, string _network, Dictionary<string, string> _options)
        {
          return session.JsonRpcClient.async_host_migrate_receive(session.opaque_ref, _host, _network, _options);
        }

        /// <summary>
        /// Declare that a host is dead. This is a dangerous operation, and should only be called if the administrator is absolutely sure the host is definitely dead
        /// First published in XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void declare_dead(Session session, string _host)
        {
            session.JsonRpcClient.host_declare_dead(session.opaque_ref, _host);
        }

        /// <summary>
        /// Declare that a host is dead. This is a dangerous operation, and should only be called if the administrator is absolutely sure the host is definitely dead
        /// First published in XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_declare_dead(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_declare_dead(session.opaque_ref, _host);
        }

        /// <summary>
        /// Enable console output to the physical display device next time this host boots
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static host_display enable_display(Session session, string _host)
        {
            return session.JsonRpcClient.host_enable_display(session.opaque_ref, _host);
        }

        /// <summary>
        /// Enable console output to the physical display device next time this host boots
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_enable_display(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_enable_display(session.opaque_ref, _host);
        }

        /// <summary>
        /// Disable console output to the physical display device next time this host boots
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static host_display disable_display(Session session, string _host)
        {
            return session.JsonRpcClient.host_disable_display(session.opaque_ref, _host);
        }

        /// <summary>
        /// Disable console output to the physical display device next time this host boots
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_disable_display(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_disable_display(session.opaque_ref, _host);
        }

        /// <summary>
        /// Enable/disable SSLv3 for interoperability with older server versions. When this is set to a different value, the host immediately restarts its SSL/TLS listening service; typically this takes less than a second but existing connections to it will be broken. API login sessions will remain valid.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">True to allow SSLv3 and ciphersuites as used in old XenServer versions</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_ssl_legacy(Session session, string _host, bool _value)
        {
            session.JsonRpcClient.host_set_ssl_legacy(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Enable/disable SSLv3 for interoperability with older server versions. When this is set to a different value, the host immediately restarts its SSL/TLS listening service; typically this takes less than a second but existing connections to it will be broken. API login sessions will remain valid.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">True to allow SSLv3 and ciphersuites as used in old XenServer versions</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_ssl_legacy(Session session, string _host, bool _value)
        {
          return session.JsonRpcClient.async_host_set_ssl_legacy(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Sets the initiator IQN for the host
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The value to which the IQN should be set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_iscsi_iqn(Session session, string _host, string _value)
        {
            session.JsonRpcClient.host_set_iscsi_iqn(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Sets the initiator IQN for the host
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The value to which the IQN should be set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_iscsi_iqn(Session session, string _host, string _value)
        {
          return session.JsonRpcClient.async_host_set_iscsi_iqn(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Specifies whether multipathing is enabled
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">Whether multipathing should be enabled</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_multipathing(Session session, string _host, bool _value)
        {
            session.JsonRpcClient.host_set_multipathing(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Specifies whether multipathing is enabled
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">Whether multipathing should be enabled</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_multipathing(Session session, string _host, bool _value)
        {
          return session.JsonRpcClient.async_host_set_multipathing(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Sets the UEFI certificates on a host
        /// First published in Citrix Hypervisor 8.1.
        /// Deprecated since 22.16.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The certificates to apply to a host</param>
        [Deprecated("22.16.0")]
        /// <remarks>
        /// Minimum allowed role: Not Applicable
        /// </remarks>
        public static void set_uefi_certificates(Session session, string _host, string _value)
        {
            session.JsonRpcClient.host_set_uefi_certificates(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Sets the UEFI certificates on a host
        /// First published in Citrix Hypervisor 8.1.
        /// Deprecated since 22.16.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The certificates to apply to a host</param>
        [Deprecated("22.16.0")]
        /// <remarks>
        /// Minimum allowed role: Not Applicable
        /// </remarks>
        public static XenRef<Task> async_set_uefi_certificates(Session session, string _host, string _value)
        {
          return session.JsonRpcClient.async_host_set_uefi_certificates(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Sets xen's sched-gran on a host. See: https://xenbits.xen.org/docs/unstable/misc/xen-command-line.html#sched-gran-x86
        /// First published in 1.271.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The sched-gran to apply to a host</param>
        /// <remarks>
        /// Minimum allowed role: Not Applicable
        /// </remarks>
        public static void set_sched_gran(Session session, string _host, host_sched_gran _value)
        {
            session.JsonRpcClient.host_set_sched_gran(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Sets xen's sched-gran on a host. See: https://xenbits.xen.org/docs/unstable/misc/xen-command-line.html#sched-gran-x86
        /// First published in 1.271.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The sched-gran to apply to a host</param>
        /// <remarks>
        /// Minimum allowed role: Not Applicable
        /// </remarks>
        public static XenRef<Task> async_set_sched_gran(Session session, string _host, host_sched_gran _value)
        {
          return session.JsonRpcClient.async_host_set_sched_gran(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Gets xen's sched-gran on a host
        /// First published in 1.271.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: Not Applicable
        /// </remarks>
        public static host_sched_gran get_sched_gran(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_sched_gran(session.opaque_ref, _host);
        }

        /// <summary>
        /// Gets xen's sched-gran on a host
        /// First published in 1.271.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: Not Applicable
        /// </remarks>
        public static XenRef<Task> async_get_sched_gran(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_get_sched_gran(session.opaque_ref, _host);
        }

        /// <summary>
        /// Set VM placement NUMA affinity policy
        /// Experimental. First published in 24.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The NUMA affinity policy to apply to a host</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_numa_affinity_policy(Session session, string _host, host_numa_affinity_policy _value)
        {
            session.JsonRpcClient.host_set_numa_affinity_policy(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Set VM placement NUMA affinity policy
        /// Experimental. First published in 24.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The NUMA affinity policy to apply to a host</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_numa_affinity_policy(Session session, string _host, host_numa_affinity_policy _value)
        {
          return session.JsonRpcClient.async_host_set_numa_affinity_policy(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Disable TLS verification for this host only
        /// First published in 1.290.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: Not Applicable
        /// </remarks>
        public static void emergency_disable_tls_verification(Session session)
        {
            session.JsonRpcClient.host_emergency_disable_tls_verification(session.opaque_ref);
        }

        /// <summary>
        /// Reenable TLS verification for this host only, and only after it was emergency disabled
        /// First published in 1.298.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: Not Applicable
        /// </remarks>
        public static void emergency_reenable_tls_verification(Session session)
        {
            session.JsonRpcClient.host_emergency_reenable_tls_verification(session.opaque_ref);
        }

        /// <summary>
        /// apply updates from current enabled repository on a host
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_hash">The hash of updateinfo to be applied which is returned by previous pool.sync_udpates</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static string[][] apply_updates(Session session, string _host, string _hash)
        {
            return session.JsonRpcClient.host_apply_updates(session.opaque_ref, _host, _hash);
        }

        /// <summary>
        /// apply updates from current enabled repository on a host
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_hash">The hash of updateinfo to be applied which is returned by previous pool.sync_udpates</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_apply_updates(Session session, string _host, string _hash)
        {
          return session.JsonRpcClient.async_host_apply_updates(session.opaque_ref, _host, _hash);
        }

        /// <summary>
        /// Scan the host and update its driver information.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void rescan_drivers(Session session, string _host)
        {
            session.JsonRpcClient.host_rescan_drivers(session.opaque_ref, _host);
        }

        /// <summary>
        /// Scan the host and update its driver information.
        /// Experimental. First published in 25.2.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_rescan_drivers(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_rescan_drivers(session.opaque_ref, _host);
        }

        /// <summary>
        /// updates the host firewall to open or close port 80 depending on the value
        /// Experimental. First published in 22.27.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">true - http port 80 will be blocked, false - http port 80 will be open</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_https_only(Session session, string _host, bool _value)
        {
            session.JsonRpcClient.host_set_https_only(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// updates the host firewall to open or close port 80 depending on the value
        /// Experimental. First published in 22.27.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">true - http port 80 will be blocked, false - http port 80 will be open</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_https_only(Session session, string _host, bool _value)
        {
          return session.JsonRpcClient.async_host_set_https_only(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// apply all recommended guidances both on the host and on all HVM VMs on the host after updates are applied on the host
        /// Experimental. First published in 23.18.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void apply_recommended_guidances(Session session, string _host)
        {
            session.JsonRpcClient.host_apply_recommended_guidances(session.opaque_ref, _host);
        }

        /// <summary>
        /// apply all recommended guidances both on the host and on all HVM VMs on the host after updates are applied on the host
        /// Experimental. First published in 23.18.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_apply_recommended_guidances(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_apply_recommended_guidances(session.opaque_ref, _host);
        }

        /// <summary>
        /// Clear the pending mandatory guidance on this host
        /// Experimental. First published in 24.10.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: Not Applicable
        /// </remarks>
        public static void emergency_clear_mandatory_guidance(Session session)
        {
            session.JsonRpcClient.host_emergency_clear_mandatory_guidance(session.opaque_ref);
        }

        /// <summary>
        /// Enable SSH access on the host. It will start the service sshd only if it is not running. It will also enable the service sshd only if it is not enabled. A newly joined host in the pool or an ejected host from the pool would keep the original status.
        /// Experimental. First published in 25.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void enable_ssh(Session session, string _host)
        {
            session.JsonRpcClient.host_enable_ssh(session.opaque_ref, _host);
        }

        /// <summary>
        /// Enable SSH access on the host. It will start the service sshd only if it is not running. It will also enable the service sshd only if it is not enabled. A newly joined host in the pool or an ejected host from the pool would keep the original status.
        /// Experimental. First published in 25.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_enable_ssh(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_enable_ssh(session.opaque_ref, _host);
        }

        /// <summary>
        /// Disable SSH access on the host. It will stop the service sshd only if it is running. It will also disable the service sshd only if it is enabled. A newly joined host in the pool or an ejected host from the pool would keep the original status.
        /// Experimental. First published in 25.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void disable_ssh(Session session, string _host)
        {
            session.JsonRpcClient.host_disable_ssh(session.opaque_ref, _host);
        }

        /// <summary>
        /// Disable SSH access on the host. It will stop the service sshd only if it is running. It will also disable the service sshd only if it is enabled. A newly joined host in the pool or an ejected host from the pool would keep the original status.
        /// Experimental. First published in 25.13.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_disable_ssh(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_disable_ssh(session.opaque_ref, _host);
        }

        /// <summary>
        /// Set the SSH service enabled timeout for the host
        /// Experimental. First published in 25.21.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The SSH enabled timeout in seconds (0 means no timeout, max 2 days)</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_ssh_enabled_timeout(Session session, string _host, long _value)
        {
            session.JsonRpcClient.host_set_ssh_enabled_timeout(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Set the SSH service enabled timeout for the host
        /// Experimental. First published in 25.21.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The SSH enabled timeout in seconds (0 means no timeout, max 2 days)</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_ssh_enabled_timeout(Session session, string _host, long _value)
        {
          return session.JsonRpcClient.async_host_set_ssh_enabled_timeout(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Set the console idle timeout for the host
        /// Experimental. First published in 25.21.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The console idle timeout in seconds</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_console_idle_timeout(Session session, string _host, long _value)
        {
            session.JsonRpcClient.host_set_console_idle_timeout(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Set the console idle timeout for the host
        /// Experimental. First published in 25.21.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The console idle timeout in seconds</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_console_idle_timeout(Session session, string _host, long _value)
        {
          return session.JsonRpcClient.async_host_set_console_idle_timeout(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Set the SSH auto mode for the host
        /// Experimental. First published in 25.27.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The SSH auto mode for the host. When true, the SSH port is closed by default and it's open only in case the API is unavailable. When false, the SSH port is always open.</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_ssh_auto_mode(Session session, string _host, bool _value)
        {
            session.JsonRpcClient.host_set_ssh_auto_mode(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Set the SSH auto mode for the host
        /// Experimental. First published in 25.27.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The SSH auto mode for the host. When true, the SSH port is closed by default and it's open only in case the API is unavailable. When false, the SSH port is always open.</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_ssh_auto_mode(Session session, string _host, bool _value)
        {
          return session.JsonRpcClient.async_host_set_ssh_auto_mode(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Get the (name, version) list of tracked user agents on this host. If different versions of the same name are seen, keep the last-seen version. The oldest entry will be removed if reach the max num. Note that the list is cleared after host/XAPI restart
        /// Experimental. First published in 25.34.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_tracked_user_agents(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_tracked_user_agents(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the (name, version) list of tracked user agents on this host. If different versions of the same name are seen, keep the last-seen version. The oldest entry will be removed if reach the max num. Note that the list is cleared after host/XAPI restart
        /// Experimental. First published in 25.34.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_get_tracked_user_agents(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_get_tracked_user_agents(session.opaque_ref, _host);
        }

        /// <summary>
        /// Sets xen's max-cstate on a host. See: https://xenbits.xen.org/docs/unstable/misc/xen-command-line.html#max_cstate-x86. "" means unlimited; "N" means limit to CN; "N,M" means limit to CN with max sub cstate M. Note: Only C0, C1, unlimited are supported currently.
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The max_cstate to apply to a host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_max_cstate(Session session, string _host, string _value)
        {
            session.JsonRpcClient.host_set_max_cstate(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Sets xen's max-cstate on a host. See: https://xenbits.xen.org/docs/unstable/misc/xen-command-line.html#max_cstate-x86. "" means unlimited; "N" means limit to CN; "N,M" means limit to CN with max sub cstate M. Note: Only C0, C1, unlimited are supported currently.
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The max_cstate to apply to a host</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_max_cstate(Session session, string _host, string _value)
        {
          return session.JsonRpcClient.async_host_set_max_cstate(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Set the NTP mode for the host
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The NTP mode to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_ntp_mode(Session session, string _host, host_ntp_mode _value)
        {
            session.JsonRpcClient.host_set_ntp_mode(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Set the NTP mode for the host
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The NTP mode to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_ntp_mode(Session session, string _host, host_ntp_mode _value)
        {
          return session.JsonRpcClient.async_host_set_ntp_mode(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Set the custom NTP servers for the host
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The set of custom NTP servers to configure</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_ntp_custom_servers(Session session, string _host, string[] _value)
        {
            session.JsonRpcClient.host_set_ntp_custom_servers(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Set the custom NTP servers for the host
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The set of custom NTP servers to configure</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_ntp_custom_servers(Session session, string _host, string[] _value)
        {
          return session.JsonRpcClient.async_host_set_ntp_custom_servers(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Get the NTP servers status on the host
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_ntp_servers_status(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_ntp_servers_status(session.opaque_ref, _host);
        }

        /// <summary>
        /// Get the NTP servers status on the host
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_get_ntp_servers_status(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_get_ntp_servers_status(session.opaque_ref, _host);
        }

        /// <summary>
        /// Set the host's timezone.
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The time zone identifier as defined in the IANA Time Zone Database</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_timezone(Session session, string _host, string _value)
        {
            session.JsonRpcClient.host_set_timezone(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Set the host's timezone.
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The time zone identifier as defined in the IANA Time Zone Database</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_timezone(Session session, string _host, string _value)
        {
          return session.JsonRpcClient.async_host_set_timezone(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// List all available timezones on the host.
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] list_timezones(Session session, string _host)
        {
            return session.JsonRpcClient.host_list_timezones(session.opaque_ref, _host);
        }

        /// <summary>
        /// List all available timezones on the host.
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_list_timezones(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_list_timezones(session.opaque_ref, _host);
        }

        /// <summary>
        /// Returns true if the system clock on the host is synchronized with the NTP servers.
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_ntp_synchronized(Session session, string _host)
        {
            return session.JsonRpcClient.host_get_ntp_synchronized(session.opaque_ref, _host);
        }

        /// <summary>
        /// Returns true if the system clock on the host is synchronized with the NTP servers.
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_get_ntp_synchronized(Session session, string _host)
        {
          return session.JsonRpcClient.async_host_get_ntp_synchronized(session.opaque_ref, _host);
        }

        /// <summary>
        /// Set the host's system clock when NTP is disabled.
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">A date/time to be set. When a timezone offset is missing, UTC is assumed.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_servertime(Session session, string _host, DateTime _value)
        {
            session.JsonRpcClient.host_set_servertime(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Set the host's system clock when NTP is disabled.
        /// Experimental. First published in 26.0.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">A date/time to be set. When a timezone offset is missing, UTC is assumed.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_servertime(Session session, string _host, DateTime _value)
        {
          return session.JsonRpcClient.async_host_set_servertime(session.opaque_ref, _host, _value);
        }

        /// <summary>
        /// Return a list of all the hosts known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Host>> get_all(Session session)
        {
            return session.JsonRpcClient.host_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of host references to host records for all hosts known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Host>, Host> get_all_records(Session session)
        {
            return session.JsonRpcClient.host_get_all_records(session.opaque_ref);
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
        /// Virtualization memory overhead (bytes).
        /// </summary>
        public virtual long memory_overhead
        {
            get { return _memory_overhead; }
            set
            {
                if (!Helper.AreEqual(value, _memory_overhead))
                {
                    _memory_overhead = value;
                    NotifyPropertyChanged("memory_overhead");
                }
            }
        }
        private long _memory_overhead = 0;

        /// <summary>
        /// list of the operations allowed in this state. This list is advisory only and the server state may have changed by the time this field is read by a client.
        /// </summary>
        public virtual List<host_allowed_operations> allowed_operations
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
        private List<host_allowed_operations> _allowed_operations = new List<host_allowed_operations>() {};

        /// <summary>
        /// links each of the running tasks using this object (by reference) to a current_operation enum which describes the nature of the task.
        /// </summary>
        public virtual Dictionary<string, host_allowed_operations> current_operations
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
        private Dictionary<string, host_allowed_operations> _current_operations = new Dictionary<string, host_allowed_operations>() {};

        /// <summary>
        /// major version number
        /// </summary>
        public virtual long API_version_major
        {
            get { return _API_version_major; }
            set
            {
                if (!Helper.AreEqual(value, _API_version_major))
                {
                    _API_version_major = value;
                    NotifyPropertyChanged("API_version_major");
                }
            }
        }
        private long _API_version_major;

        /// <summary>
        /// minor version number
        /// </summary>
        public virtual long API_version_minor
        {
            get { return _API_version_minor; }
            set
            {
                if (!Helper.AreEqual(value, _API_version_minor))
                {
                    _API_version_minor = value;
                    NotifyPropertyChanged("API_version_minor");
                }
            }
        }
        private long _API_version_minor;

        /// <summary>
        /// identification of vendor
        /// </summary>
        public virtual string API_version_vendor
        {
            get { return _API_version_vendor; }
            set
            {
                if (!Helper.AreEqual(value, _API_version_vendor))
                {
                    _API_version_vendor = value;
                    NotifyPropertyChanged("API_version_vendor");
                }
            }
        }
        private string _API_version_vendor = "";

        /// <summary>
        /// details of vendor implementation
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> API_version_vendor_implementation
        {
            get { return _API_version_vendor_implementation; }
            set
            {
                if (!Helper.AreEqual(value, _API_version_vendor_implementation))
                {
                    _API_version_vendor_implementation = value;
                    NotifyPropertyChanged("API_version_vendor_implementation");
                }
            }
        }
        private Dictionary<string, string> _API_version_vendor_implementation = new Dictionary<string, string>() {};

        /// <summary>
        /// True if the host is currently enabled
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
        private bool _enabled;

        /// <summary>
        /// version strings
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> software_version
        {
            get { return _software_version; }
            set
            {
                if (!Helper.AreEqual(value, _software_version))
                {
                    _software_version = value;
                    NotifyPropertyChanged("software_version");
                }
            }
        }
        private Dictionary<string, string> _software_version = new Dictionary<string, string>() {};

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
        /// Xen capabilities
        /// </summary>
        public virtual string[] capabilities
        {
            get { return _capabilities; }
            set
            {
                if (!Helper.AreEqual(value, _capabilities))
                {
                    _capabilities = value;
                    NotifyPropertyChanged("capabilities");
                }
            }
        }
        private string[] _capabilities = {};

        /// <summary>
        /// The CPU configuration on this host.  May contain keys such as "nr_nodes", "sockets_per_node", "cores_per_socket", or "threads_per_core"
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> cpu_configuration
        {
            get { return _cpu_configuration; }
            set
            {
                if (!Helper.AreEqual(value, _cpu_configuration))
                {
                    _cpu_configuration = value;
                    NotifyPropertyChanged("cpu_configuration");
                }
            }
        }
        private Dictionary<string, string> _cpu_configuration = new Dictionary<string, string>() {};

        /// <summary>
        /// Scheduler policy currently in force on this host
        /// </summary>
        public virtual string sched_policy
        {
            get { return _sched_policy; }
            set
            {
                if (!Helper.AreEqual(value, _sched_policy))
                {
                    _sched_policy = value;
                    NotifyPropertyChanged("sched_policy");
                }
            }
        }
        private string _sched_policy = "";

        /// <summary>
        /// a list of the bootloaders installed on the machine
        /// </summary>
        public virtual string[] supported_bootloaders
        {
            get { return _supported_bootloaders; }
            set
            {
                if (!Helper.AreEqual(value, _supported_bootloaders))
                {
                    _supported_bootloaders = value;
                    NotifyPropertyChanged("supported_bootloaders");
                }
            }
        }
        private string[] _supported_bootloaders = {};

        /// <summary>
        /// list of VMs currently resident on host
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VM>))]
        public virtual List<XenRef<VM>> resident_VMs
        {
            get { return _resident_VMs; }
            set
            {
                if (!Helper.AreEqual(value, _resident_VMs))
                {
                    _resident_VMs = value;
                    NotifyPropertyChanged("resident_VMs");
                }
            }
        }
        private List<XenRef<VM>> _resident_VMs = new List<XenRef<VM>>() {};

        /// <summary>
        /// logging configuration
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> logging
        {
            get { return _logging; }
            set
            {
                if (!Helper.AreEqual(value, _logging))
                {
                    _logging = value;
                    NotifyPropertyChanged("logging");
                }
            }
        }
        private Dictionary<string, string> _logging = new Dictionary<string, string>() {};

        /// <summary>
        /// physical network interfaces
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<PIF>))]
        public virtual List<XenRef<PIF>> PIFs
        {
            get { return _PIFs; }
            set
            {
                if (!Helper.AreEqual(value, _PIFs))
                {
                    _PIFs = value;
                    NotifyPropertyChanged("PIFs");
                }
            }
        }
        private List<XenRef<PIF>> _PIFs = new List<XenRef<PIF>>() {};

        /// <summary>
        /// The SR in which VDIs for suspend images are created
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<SR>))]
        public virtual XenRef<SR> suspend_image_sr
        {
            get { return _suspend_image_sr; }
            set
            {
                if (!Helper.AreEqual(value, _suspend_image_sr))
                {
                    _suspend_image_sr = value;
                    NotifyPropertyChanged("suspend_image_sr");
                }
            }
        }
        private XenRef<SR> _suspend_image_sr = new XenRef<SR>(Helper.NullOpaqueRef);

        /// <summary>
        /// The SR in which VDIs for crash dumps are created
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<SR>))]
        public virtual XenRef<SR> crash_dump_sr
        {
            get { return _crash_dump_sr; }
            set
            {
                if (!Helper.AreEqual(value, _crash_dump_sr))
                {
                    _crash_dump_sr = value;
                    NotifyPropertyChanged("crash_dump_sr");
                }
            }
        }
        private XenRef<SR> _crash_dump_sr = new XenRef<SR>(Helper.NullOpaqueRef);

        /// <summary>
        /// Set of host crash dumps
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Host_crashdump>))]
        public virtual List<XenRef<Host_crashdump>> crashdumps
        {
            get { return _crashdumps; }
            set
            {
                if (!Helper.AreEqual(value, _crashdumps))
                {
                    _crashdumps = value;
                    NotifyPropertyChanged("crashdumps");
                }
            }
        }
        private List<XenRef<Host_crashdump>> _crashdumps = new List<XenRef<Host_crashdump>>() {};

        /// <summary>
        /// Set of host patches
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Host_patch>))]
        public virtual List<XenRef<Host_patch>> patches
        {
            get { return _patches; }
            set
            {
                if (!Helper.AreEqual(value, _patches))
                {
                    _patches = value;
                    NotifyPropertyChanged("patches");
                }
            }
        }
        private List<XenRef<Host_patch>> _patches = new List<XenRef<Host_patch>>() {};

        /// <summary>
        /// Set of updates
        /// First published in XenServer 7.1.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Pool_update>))]
        public virtual List<XenRef<Pool_update>> updates
        {
            get { return _updates; }
            set
            {
                if (!Helper.AreEqual(value, _updates))
                {
                    _updates = value;
                    NotifyPropertyChanged("updates");
                }
            }
        }
        private List<XenRef<Pool_update>> _updates = new List<XenRef<Pool_update>>() {};

        /// <summary>
        /// physical blockdevices
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<PBD>))]
        public virtual List<XenRef<PBD>> PBDs
        {
            get { return _PBDs; }
            set
            {
                if (!Helper.AreEqual(value, _PBDs))
                {
                    _PBDs = value;
                    NotifyPropertyChanged("PBDs");
                }
            }
        }
        private List<XenRef<PBD>> _PBDs = new List<XenRef<PBD>>() {};

        /// <summary>
        /// The physical CPUs on this host
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Host_cpu>))]
        public virtual List<XenRef<Host_cpu>> host_CPUs
        {
            get { return _host_CPUs; }
            set
            {
                if (!Helper.AreEqual(value, _host_CPUs))
                {
                    _host_CPUs = value;
                    NotifyPropertyChanged("host_CPUs");
                }
            }
        }
        private List<XenRef<Host_cpu>> _host_CPUs = new List<XenRef<Host_cpu>>() {};

        /// <summary>
        /// Details about the physical CPUs on this host
        /// First published in XenServer 5.6.
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
        /// The hostname of this host
        /// </summary>
        public virtual string hostname
        {
            get { return _hostname; }
            set
            {
                if (!Helper.AreEqual(value, _hostname))
                {
                    _hostname = value;
                    NotifyPropertyChanged("hostname");
                }
            }
        }
        private string _hostname = "";

        /// <summary>
        /// The address by which this host can be contacted from any other host in the pool
        /// </summary>
        public virtual string address
        {
            get { return _address; }
            set
            {
                if (!Helper.AreEqual(value, _address))
                {
                    _address = value;
                    NotifyPropertyChanged("address");
                }
            }
        }
        private string _address = "";

        /// <summary>
        /// metrics associated with this host
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Host_metrics>))]
        public virtual XenRef<Host_metrics> metrics
        {
            get { return _metrics; }
            set
            {
                if (!Helper.AreEqual(value, _metrics))
                {
                    _metrics = value;
                    NotifyPropertyChanged("metrics");
                }
            }
        }
        private XenRef<Host_metrics> _metrics = new XenRef<Host_metrics>(Helper.NullOpaqueRef);

        /// <summary>
        /// State of the current license
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> license_params
        {
            get { return _license_params; }
            set
            {
                if (!Helper.AreEqual(value, _license_params))
                {
                    _license_params = value;
                    NotifyPropertyChanged("license_params");
                }
            }
        }
        private Dictionary<string, string> _license_params = new Dictionary<string, string>() {};

        /// <summary>
        /// The set of statefiles accessible from this host
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
        /// The set of hosts visible via the network from this host
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual string[] ha_network_peers
        {
            get { return _ha_network_peers; }
            set
            {
                if (!Helper.AreEqual(value, _ha_network_peers))
                {
                    _ha_network_peers = value;
                    NotifyPropertyChanged("ha_network_peers");
                }
            }
        }
        private string[] _ha_network_peers = {};

        /// <summary>
        /// Binary blobs associated with this host
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
        /// type of external authentication service configured; empty if none configured.
        /// First published in XenServer 5.5.
        /// </summary>
        public virtual string external_auth_type
        {
            get { return _external_auth_type; }
            set
            {
                if (!Helper.AreEqual(value, _external_auth_type))
                {
                    _external_auth_type = value;
                    NotifyPropertyChanged("external_auth_type");
                }
            }
        }
        private string _external_auth_type = "";

        /// <summary>
        /// name of external authentication service configured; empty if none configured.
        /// First published in XenServer 5.5.
        /// </summary>
        public virtual string external_auth_service_name
        {
            get { return _external_auth_service_name; }
            set
            {
                if (!Helper.AreEqual(value, _external_auth_service_name))
                {
                    _external_auth_service_name = value;
                    NotifyPropertyChanged("external_auth_service_name");
                }
            }
        }
        private string _external_auth_service_name = "";

        /// <summary>
        /// configuration specific to external authentication service
        /// First published in XenServer 5.5.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> external_auth_configuration
        {
            get { return _external_auth_configuration; }
            set
            {
                if (!Helper.AreEqual(value, _external_auth_configuration))
                {
                    _external_auth_configuration = value;
                    NotifyPropertyChanged("external_auth_configuration");
                }
            }
        }
        private Dictionary<string, string> _external_auth_configuration = new Dictionary<string, string>() {};

        /// <summary>
        /// Product edition
        /// First published in XenServer 5.6.
        /// </summary>
        public virtual string edition
        {
            get { return _edition; }
            set
            {
                if (!Helper.AreEqual(value, _edition))
                {
                    _edition = value;
                    NotifyPropertyChanged("edition");
                }
            }
        }
        private string _edition = "";

        /// <summary>
        /// Contact information of the license server
        /// First published in XenServer 5.6.
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
        private Dictionary<string, string> _license_server = new Dictionary<string, string>() {{"address", "localhost"}, {"port", "27000"}};

        /// <summary>
        /// BIOS strings
        /// First published in XenServer 5.6.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> bios_strings
        {
            get { return _bios_strings; }
            set
            {
                if (!Helper.AreEqual(value, _bios_strings))
                {
                    _bios_strings = value;
                    NotifyPropertyChanged("bios_strings");
                }
            }
        }
        private Dictionary<string, string> _bios_strings = new Dictionary<string, string>() {};

        /// <summary>
        /// The power on mode
        /// First published in XenServer 5.6.
        /// </summary>
        public virtual string power_on_mode
        {
            get { return _power_on_mode; }
            set
            {
                if (!Helper.AreEqual(value, _power_on_mode))
                {
                    _power_on_mode = value;
                    NotifyPropertyChanged("power_on_mode");
                }
            }
        }
        private string _power_on_mode = "";

        /// <summary>
        /// The power on config
        /// First published in XenServer 5.6.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> power_on_config
        {
            get { return _power_on_config; }
            set
            {
                if (!Helper.AreEqual(value, _power_on_config))
                {
                    _power_on_config = value;
                    NotifyPropertyChanged("power_on_config");
                }
            }
        }
        private Dictionary<string, string> _power_on_config = new Dictionary<string, string>() {};

        /// <summary>
        /// The SR that is used as a local cache
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<SR>))]
        public virtual XenRef<SR> local_cache_sr
        {
            get { return _local_cache_sr; }
            set
            {
                if (!Helper.AreEqual(value, _local_cache_sr))
                {
                    _local_cache_sr = value;
                    NotifyPropertyChanged("local_cache_sr");
                }
            }
        }
        private XenRef<SR> _local_cache_sr = new XenRef<SR>("OpaqueRef:NULL");

        /// <summary>
        /// Information about chipset features
        /// First published in XenServer 6.0.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> chipset_info
        {
            get { return _chipset_info; }
            set
            {
                if (!Helper.AreEqual(value, _chipset_info))
                {
                    _chipset_info = value;
                    NotifyPropertyChanged("chipset_info");
                }
            }
        }
        private Dictionary<string, string> _chipset_info = new Dictionary<string, string>() {};

        /// <summary>
        /// List of PCI devices in the host
        /// First published in XenServer 6.0.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<PCI>))]
        public virtual List<XenRef<PCI>> PCIs
        {
            get { return _PCIs; }
            set
            {
                if (!Helper.AreEqual(value, _PCIs))
                {
                    _PCIs = value;
                    NotifyPropertyChanged("PCIs");
                }
            }
        }
        private List<XenRef<PCI>> _PCIs = new List<XenRef<PCI>>() {};

        /// <summary>
        /// List of physical GPUs in the host
        /// First published in XenServer 6.0.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<PGPU>))]
        public virtual List<XenRef<PGPU>> PGPUs
        {
            get { return _PGPUs; }
            set
            {
                if (!Helper.AreEqual(value, _PGPUs))
                {
                    _PGPUs = value;
                    NotifyPropertyChanged("PGPUs");
                }
            }
        }
        private List<XenRef<PGPU>> _PGPUs = new List<XenRef<PGPU>>() {};

        /// <summary>
        /// List of physical USBs in the host
        /// First published in XenServer 7.3.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<PUSB>))]
        public virtual List<XenRef<PUSB>> PUSBs
        {
            get { return _PUSBs; }
            set
            {
                if (!Helper.AreEqual(value, _PUSBs))
                {
                    _PUSBs = value;
                    NotifyPropertyChanged("PUSBs");
                }
            }
        }
        private List<XenRef<PUSB>> _PUSBs = new List<XenRef<PUSB>>() {};

        /// <summary>
        /// Allow SSLv3 protocol and ciphersuites as used by older server versions. This controls both incoming and outgoing connections. When this is set to a different value, the host immediately restarts its SSL/TLS listening service; typically this takes less than a second but existing connections to it will be broken. API login sessions will remain valid.
        /// First published in XenServer 7.0.
        /// </summary>
        public virtual bool ssl_legacy
        {
            get { return _ssl_legacy; }
            set
            {
                if (!Helper.AreEqual(value, _ssl_legacy))
                {
                    _ssl_legacy = value;
                    NotifyPropertyChanged("ssl_legacy");
                }
            }
        }
        private bool _ssl_legacy = true;

        /// <summary>
        /// VCPUs params to apply to all resident guests
        /// First published in XenServer 6.1.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> guest_VCPUs_params
        {
            get { return _guest_VCPUs_params; }
            set
            {
                if (!Helper.AreEqual(value, _guest_VCPUs_params))
                {
                    _guest_VCPUs_params = value;
                    NotifyPropertyChanged("guest_VCPUs_params");
                }
            }
        }
        private Dictionary<string, string> _guest_VCPUs_params = new Dictionary<string, string>() {};

        /// <summary>
        /// indicates whether the host is configured to output its console to a physical display device
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        [JsonConverter(typeof(host_displayConverter))]
        public virtual host_display display
        {
            get { return _display; }
            set
            {
                if (!Helper.AreEqual(value, _display))
                {
                    _display = value;
                    NotifyPropertyChanged("display");
                }
            }
        }
        private host_display _display = host_display.enabled;

        /// <summary>
        /// The set of versions of the virtual hardware platform that the host can offer to its guests
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        public virtual long[] virtual_hardware_platform_versions
        {
            get { return _virtual_hardware_platform_versions; }
            set
            {
                if (!Helper.AreEqual(value, _virtual_hardware_platform_versions))
                {
                    _virtual_hardware_platform_versions = value;
                    NotifyPropertyChanged("virtual_hardware_platform_versions");
                }
            }
        }
        private long[] _virtual_hardware_platform_versions = {0};

        /// <summary>
        /// The control domain (domain 0)
        /// First published in XenServer 7.1.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VM>))]
        public virtual XenRef<VM> control_domain
        {
            get { return _control_domain; }
            set
            {
                if (!Helper.AreEqual(value, _control_domain))
                {
                    _control_domain = value;
                    NotifyPropertyChanged("control_domain");
                }
            }
        }
        private XenRef<VM> _control_domain = new XenRef<VM>("OpaqueRef:NULL");

        /// <summary>
        /// List of updates which require reboot
        /// First published in XenServer 7.1.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Pool_update>))]
        public virtual List<XenRef<Pool_update>> updates_requiring_reboot
        {
            get { return _updates_requiring_reboot; }
            set
            {
                if (!Helper.AreEqual(value, _updates_requiring_reboot))
                {
                    _updates_requiring_reboot = value;
                    NotifyPropertyChanged("updates_requiring_reboot");
                }
            }
        }
        private List<XenRef<Pool_update>> _updates_requiring_reboot = new List<XenRef<Pool_update>>() {};

        /// <summary>
        /// List of features available on this host
        /// First published in XenServer 7.2.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Feature>))]
        public virtual List<XenRef<Feature>> features
        {
            get { return _features; }
            set
            {
                if (!Helper.AreEqual(value, _features))
                {
                    _features = value;
                    NotifyPropertyChanged("features");
                }
            }
        }
        private List<XenRef<Feature>> _features = new List<XenRef<Feature>>() {};

        /// <summary>
        /// The initiator IQN for the host
        /// First published in XenServer 7.5.
        /// </summary>
        public virtual string iscsi_iqn
        {
            get { return _iscsi_iqn; }
            set
            {
                if (!Helper.AreEqual(value, _iscsi_iqn))
                {
                    _iscsi_iqn = value;
                    NotifyPropertyChanged("iscsi_iqn");
                }
            }
        }
        private string _iscsi_iqn = "";

        /// <summary>
        /// Specifies whether multipathing is enabled
        /// First published in XenServer 7.5.
        /// </summary>
        public virtual bool multipathing
        {
            get { return _multipathing; }
            set
            {
                if (!Helper.AreEqual(value, _multipathing))
                {
                    _multipathing = value;
                    NotifyPropertyChanged("multipathing");
                }
            }
        }
        private bool _multipathing = false;

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
        /// List of certificates installed in the host
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Certificate>))]
        public virtual List<XenRef<Certificate>> certificates
        {
            get { return _certificates; }
            set
            {
                if (!Helper.AreEqual(value, _certificates))
                {
                    _certificates = value;
                    NotifyPropertyChanged("certificates");
                }
            }
        }
        private List<XenRef<Certificate>> _certificates = new List<XenRef<Certificate>>() {};

        /// <summary>
        /// List of all available product editions
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        public virtual string[] editions
        {
            get { return _editions; }
            set
            {
                if (!Helper.AreEqual(value, _editions))
                {
                    _editions = value;
                    NotifyPropertyChanged("editions");
                }
            }
        }
        private string[] _editions = {};

        /// <summary>
        /// The set of pending mandatory guidances after applying updates, which must be applied, otherwise there may be, for example, VM failures
        /// First published in 1.303.0.
        /// </summary>
        public virtual List<update_guidances> pending_guidances
        {
            get { return _pending_guidances; }
            set
            {
                if (!Helper.AreEqual(value, _pending_guidances))
                {
                    _pending_guidances = value;
                    NotifyPropertyChanged("pending_guidances");
                }
            }
        }
        private List<update_guidances> _pending_guidances = new List<update_guidances>() {};

        /// <summary>
        /// True if this host has TLS verifcation enabled
        /// First published in 1.313.0.
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
        /// Date and time when the last software update was applied
        /// Experimental. First published in 22.20.0.
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime last_software_update
        {
            get { return _last_software_update; }
            set
            {
                if (!Helper.AreEqual(value, _last_software_update))
                {
                    _last_software_update = value;
                    NotifyPropertyChanged("last_software_update");
                }
            }
        }
        private DateTime _last_software_update = DateTime.ParseExact("19700101T00:00:00Z", "yyyyMMddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        /// <summary>
        /// Reflects whether port 80 is open (false) or not (true)
        /// Experimental. First published in 22.27.0.
        /// </summary>
        public virtual bool https_only
        {
            get { return _https_only; }
            set
            {
                if (!Helper.AreEqual(value, _https_only))
                {
                    _https_only = value;
                    NotifyPropertyChanged("https_only");
                }
            }
        }
        private bool _https_only = false;

        /// <summary>
        /// Default as 'unknown', 'yes' if the host is up to date with updates synced from remote CDN, otherwise 'no'
        /// Experimental. First published in 23.18.0.
        /// </summary>
        [JsonConverter(typeof(latest_synced_updates_applied_stateConverter))]
        public virtual latest_synced_updates_applied_state latest_synced_updates_applied
        {
            get { return _latest_synced_updates_applied; }
            set
            {
                if (!Helper.AreEqual(value, _latest_synced_updates_applied))
                {
                    _latest_synced_updates_applied = value;
                    NotifyPropertyChanged("latest_synced_updates_applied");
                }
            }
        }
        private latest_synced_updates_applied_state _latest_synced_updates_applied = latest_synced_updates_applied_state.unknown;

        /// <summary>
        /// NUMA-aware VM memory and vCPU placement policy
        /// Experimental. First published in 24.0.0.
        /// </summary>
        [JsonConverter(typeof(host_numa_affinity_policyConverter))]
        public virtual host_numa_affinity_policy numa_affinity_policy
        {
            get { return _numa_affinity_policy; }
            set
            {
                if (!Helper.AreEqual(value, _numa_affinity_policy))
                {
                    _numa_affinity_policy = value;
                    NotifyPropertyChanged("numa_affinity_policy");
                }
            }
        }
        private host_numa_affinity_policy _numa_affinity_policy = host_numa_affinity_policy.default_policy;

        /// <summary>
        /// The set of pending recommended guidances after applying updates, which most users should follow to make the updates effective, but if not followed, will not cause a failure
        /// Experimental. First published in 24.10.0.
        /// </summary>
        public virtual List<update_guidances> pending_guidances_recommended
        {
            get { return _pending_guidances_recommended; }
            set
            {
                if (!Helper.AreEqual(value, _pending_guidances_recommended))
                {
                    _pending_guidances_recommended = value;
                    NotifyPropertyChanged("pending_guidances_recommended");
                }
            }
        }
        private List<update_guidances> _pending_guidances_recommended = new List<update_guidances>() {};

        /// <summary>
        /// The set of pending full guidances after applying updates, which a user should follow to make some updates, for example, specific hardware drivers or CPU features, fully effective, but the 'average user' doesn't need to
        /// Experimental. First published in 24.10.0.
        /// </summary>
        public virtual List<update_guidances> pending_guidances_full
        {
            get { return _pending_guidances_full; }
            set
            {
                if (!Helper.AreEqual(value, _pending_guidances_full))
                {
                    _pending_guidances_full = value;
                    NotifyPropertyChanged("pending_guidances_full");
                }
            }
        }
        private List<update_guidances> _pending_guidances_full = new List<update_guidances>() {};

        /// <summary>
        /// The SHA256 checksum of updateinfo of the most recently applied update on the host
        /// Experimental. First published in 24.10.0.
        /// </summary>
        public virtual string last_update_hash
        {
            get { return _last_update_hash; }
            set
            {
                if (!Helper.AreEqual(value, _last_update_hash))
                {
                    _last_update_hash = value;
                    NotifyPropertyChanged("last_update_hash");
                }
            }
        }
        private string _last_update_hash = "";

        /// <summary>
        /// True if SSH access is enabled for the host
        /// Experimental. First published in 25.21.0.
        /// </summary>
        public virtual bool ssh_enabled
        {
            get { return _ssh_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _ssh_enabled))
                {
                    _ssh_enabled = value;
                    NotifyPropertyChanged("ssh_enabled");
                }
            }
        }
        private bool _ssh_enabled = true;

        /// <summary>
        /// The timeout in seconds after which SSH access will be automatically disabled (0 means never), this setting will be applied every time the SSH is enabled by XAPI
        /// Experimental. First published in 25.21.0.
        /// </summary>
        public virtual long ssh_enabled_timeout
        {
            get { return _ssh_enabled_timeout; }
            set
            {
                if (!Helper.AreEqual(value, _ssh_enabled_timeout))
                {
                    _ssh_enabled_timeout = value;
                    NotifyPropertyChanged("ssh_enabled_timeout");
                }
            }
        }
        private long _ssh_enabled_timeout = 0;

        /// <summary>
        /// The time in UTC after which the SSH access will be automatically disabled
        /// Experimental. First published in 25.21.0.
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime ssh_expiry
        {
            get { return _ssh_expiry; }
            set
            {
                if (!Helper.AreEqual(value, _ssh_expiry))
                {
                    _ssh_expiry = value;
                    NotifyPropertyChanged("ssh_expiry");
                }
            }
        }
        private DateTime _ssh_expiry = DateTime.ParseExact("19700101T00:00:00Z", "yyyyMMddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        /// <summary>
        /// The timeout in seconds after which idle console will be automatically terminated (0 means never)
        /// Experimental. First published in 25.21.0.
        /// </summary>
        public virtual long console_idle_timeout
        {
            get { return _console_idle_timeout; }
            set
            {
                if (!Helper.AreEqual(value, _console_idle_timeout))
                {
                    _console_idle_timeout = value;
                    NotifyPropertyChanged("console_idle_timeout");
                }
            }
        }
        private long _console_idle_timeout = 0;

        /// <summary>
        /// Reflects whether SSH auto mode is enabled for the host
        /// Experimental. First published in 25.27.0.
        /// </summary>
        public virtual bool ssh_auto_mode
        {
            get { return _ssh_auto_mode; }
            set
            {
                if (!Helper.AreEqual(value, _ssh_auto_mode))
                {
                    _ssh_auto_mode = value;
                    NotifyPropertyChanged("ssh_auto_mode");
                }
            }
        }
        private bool _ssh_auto_mode = false;

        /// <summary>
        /// The maximum C-state that the host is allowed to enter, "" means unlimited; "N" means limit to CN; "N,M" means limit to CN with max sub cstate M.
        /// Experimental. First published in 26.0.0.
        /// </summary>
        public virtual string max_cstate
        {
            get { return _max_cstate; }
            set
            {
                if (!Helper.AreEqual(value, _max_cstate))
                {
                    _max_cstate = value;
                    NotifyPropertyChanged("max_cstate");
                }
            }
        }
        private string _max_cstate = "";

        /// <summary>
        /// Whether the host has booted in secure boot mode
        /// Experimental. First published in 25.31.0.
        /// </summary>
        public virtual bool secure_boot
        {
            get { return _secure_boot; }
            set
            {
                if (!Helper.AreEqual(value, _secure_boot))
                {
                    _secure_boot = value;
                    NotifyPropertyChanged("secure_boot");
                }
            }
        }
        private bool _secure_boot = false;

        /// <summary>
        /// Indicates NTP servers are assigned by DHCP, or configured by user, or the factory servers, or NTP is disabled
        /// Experimental. First published in 26.0.0.
        /// </summary>
        [JsonConverter(typeof(host_ntp_modeConverter))]
        public virtual host_ntp_mode ntp_mode
        {
            get { return _ntp_mode; }
            set
            {
                if (!Helper.AreEqual(value, _ntp_mode))
                {
                    _ntp_mode = value;
                    NotifyPropertyChanged("ntp_mode");
                }
            }
        }
        private host_ntp_mode _ntp_mode = host_ntp_mode.Factory;

        /// <summary>
        /// Custom NTP servers configured by users, used in Custom NTP mode
        /// Experimental. First published in 26.0.0.
        /// </summary>
        public virtual string[] ntp_custom_servers
        {
            get { return _ntp_custom_servers; }
            set
            {
                if (!Helper.AreEqual(value, _ntp_custom_servers))
                {
                    _ntp_custom_servers = value;
                    NotifyPropertyChanged("ntp_custom_servers");
                }
            }
        }
        private string[] _ntp_custom_servers = {};

        /// <summary>
        /// The time zone identifier as defined in the IANA Time Zone Database
        /// Experimental. First published in 26.0.0.
        /// </summary>
        public virtual string timezone
        {
            get { return _timezone; }
            set
            {
                if (!Helper.AreEqual(value, _timezone))
                {
                    _timezone = value;
                    NotifyPropertyChanged("timezone");
                }
            }
        }
        private string _timezone = "UTC";
    }
}
