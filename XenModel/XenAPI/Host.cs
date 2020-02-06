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
            string uefi_certificates)
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

        /// <summary>
        /// Creates a new Host from a Proxy_Host.
        /// </summary>
        /// <param name="proxy"></param>
        public Host(Proxy_Host proxy)
        {
            UpdateFrom(proxy);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Host.
        /// </summary>
        public override void UpdateFrom(Host update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            memory_overhead = update.memory_overhead;
            allowed_operations = update.allowed_operations;
            current_operations = update.current_operations;
            API_version_major = update.API_version_major;
            API_version_minor = update.API_version_minor;
            API_version_vendor = update.API_version_vendor;
            API_version_vendor_implementation = update.API_version_vendor_implementation;
            enabled = update.enabled;
            software_version = update.software_version;
            other_config = update.other_config;
            capabilities = update.capabilities;
            cpu_configuration = update.cpu_configuration;
            sched_policy = update.sched_policy;
            supported_bootloaders = update.supported_bootloaders;
            resident_VMs = update.resident_VMs;
            logging = update.logging;
            PIFs = update.PIFs;
            suspend_image_sr = update.suspend_image_sr;
            crash_dump_sr = update.crash_dump_sr;
            crashdumps = update.crashdumps;
            patches = update.patches;
            updates = update.updates;
            PBDs = update.PBDs;
            host_CPUs = update.host_CPUs;
            cpu_info = update.cpu_info;
            hostname = update.hostname;
            address = update.address;
            metrics = update.metrics;
            license_params = update.license_params;
            ha_statefiles = update.ha_statefiles;
            ha_network_peers = update.ha_network_peers;
            blobs = update.blobs;
            tags = update.tags;
            external_auth_type = update.external_auth_type;
            external_auth_service_name = update.external_auth_service_name;
            external_auth_configuration = update.external_auth_configuration;
            edition = update.edition;
            license_server = update.license_server;
            bios_strings = update.bios_strings;
            power_on_mode = update.power_on_mode;
            power_on_config = update.power_on_config;
            local_cache_sr = update.local_cache_sr;
            chipset_info = update.chipset_info;
            PCIs = update.PCIs;
            PGPUs = update.PGPUs;
            PUSBs = update.PUSBs;
            ssl_legacy = update.ssl_legacy;
            guest_VCPUs_params = update.guest_VCPUs_params;
            display = update.display;
            virtual_hardware_platform_versions = update.virtual_hardware_platform_versions;
            control_domain = update.control_domain;
            updates_requiring_reboot = update.updates_requiring_reboot;
            features = update.features;
            iscsi_iqn = update.iscsi_iqn;
            multipathing = update.multipathing;
            uefi_certificates = update.uefi_certificates;
        }

        internal void UpdateFrom(Proxy_Host proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            name_label = proxy.name_label == null ? null : proxy.name_label;
            name_description = proxy.name_description == null ? null : proxy.name_description;
            memory_overhead = proxy.memory_overhead == null ? 0 : long.Parse(proxy.memory_overhead);
            allowed_operations = proxy.allowed_operations == null ? null : Helper.StringArrayToEnumList<host_allowed_operations>(proxy.allowed_operations);
            current_operations = proxy.current_operations == null ? null : Maps.convert_from_proxy_string_host_allowed_operations(proxy.current_operations);
            API_version_major = proxy.API_version_major == null ? 0 : long.Parse(proxy.API_version_major);
            API_version_minor = proxy.API_version_minor == null ? 0 : long.Parse(proxy.API_version_minor);
            API_version_vendor = proxy.API_version_vendor == null ? null : proxy.API_version_vendor;
            API_version_vendor_implementation = proxy.API_version_vendor_implementation == null ? null : Maps.convert_from_proxy_string_string(proxy.API_version_vendor_implementation);
            enabled = (bool)proxy.enabled;
            software_version = proxy.software_version == null ? null : Maps.convert_from_proxy_string_string(proxy.software_version);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            capabilities = proxy.capabilities == null ? new string[] {} : (string [])proxy.capabilities;
            cpu_configuration = proxy.cpu_configuration == null ? null : Maps.convert_from_proxy_string_string(proxy.cpu_configuration);
            sched_policy = proxy.sched_policy == null ? null : proxy.sched_policy;
            supported_bootloaders = proxy.supported_bootloaders == null ? new string[] {} : (string [])proxy.supported_bootloaders;
            resident_VMs = proxy.resident_VMs == null ? null : XenRef<VM>.Create(proxy.resident_VMs);
            logging = proxy.logging == null ? null : Maps.convert_from_proxy_string_string(proxy.logging);
            PIFs = proxy.PIFs == null ? null : XenRef<PIF>.Create(proxy.PIFs);
            suspend_image_sr = proxy.suspend_image_sr == null ? null : XenRef<SR>.Create(proxy.suspend_image_sr);
            crash_dump_sr = proxy.crash_dump_sr == null ? null : XenRef<SR>.Create(proxy.crash_dump_sr);
            crashdumps = proxy.crashdumps == null ? null : XenRef<Host_crashdump>.Create(proxy.crashdumps);
            patches = proxy.patches == null ? null : XenRef<Host_patch>.Create(proxy.patches);
            updates = proxy.updates == null ? null : XenRef<Pool_update>.Create(proxy.updates);
            PBDs = proxy.PBDs == null ? null : XenRef<PBD>.Create(proxy.PBDs);
            host_CPUs = proxy.host_CPUs == null ? null : XenRef<Host_cpu>.Create(proxy.host_CPUs);
            cpu_info = proxy.cpu_info == null ? null : Maps.convert_from_proxy_string_string(proxy.cpu_info);
            hostname = proxy.hostname == null ? null : proxy.hostname;
            address = proxy.address == null ? null : proxy.address;
            metrics = proxy.metrics == null ? null : XenRef<Host_metrics>.Create(proxy.metrics);
            license_params = proxy.license_params == null ? null : Maps.convert_from_proxy_string_string(proxy.license_params);
            ha_statefiles = proxy.ha_statefiles == null ? new string[] {} : (string [])proxy.ha_statefiles;
            ha_network_peers = proxy.ha_network_peers == null ? new string[] {} : (string [])proxy.ha_network_peers;
            blobs = proxy.blobs == null ? null : Maps.convert_from_proxy_string_XenRefBlob(proxy.blobs);
            tags = proxy.tags == null ? new string[] {} : (string [])proxy.tags;
            external_auth_type = proxy.external_auth_type == null ? null : proxy.external_auth_type;
            external_auth_service_name = proxy.external_auth_service_name == null ? null : proxy.external_auth_service_name;
            external_auth_configuration = proxy.external_auth_configuration == null ? null : Maps.convert_from_proxy_string_string(proxy.external_auth_configuration);
            edition = proxy.edition == null ? null : proxy.edition;
            license_server = proxy.license_server == null ? null : Maps.convert_from_proxy_string_string(proxy.license_server);
            bios_strings = proxy.bios_strings == null ? null : Maps.convert_from_proxy_string_string(proxy.bios_strings);
            power_on_mode = proxy.power_on_mode == null ? null : proxy.power_on_mode;
            power_on_config = proxy.power_on_config == null ? null : Maps.convert_from_proxy_string_string(proxy.power_on_config);
            local_cache_sr = proxy.local_cache_sr == null ? null : XenRef<SR>.Create(proxy.local_cache_sr);
            chipset_info = proxy.chipset_info == null ? null : Maps.convert_from_proxy_string_string(proxy.chipset_info);
            PCIs = proxy.PCIs == null ? null : XenRef<PCI>.Create(proxy.PCIs);
            PGPUs = proxy.PGPUs == null ? null : XenRef<PGPU>.Create(proxy.PGPUs);
            PUSBs = proxy.PUSBs == null ? null : XenRef<PUSB>.Create(proxy.PUSBs);
            ssl_legacy = (bool)proxy.ssl_legacy;
            guest_VCPUs_params = proxy.guest_VCPUs_params == null ? null : Maps.convert_from_proxy_string_string(proxy.guest_VCPUs_params);
            display = proxy.display == null ? (host_display) 0 : (host_display)Helper.EnumParseDefault(typeof(host_display), (string)proxy.display);
            virtual_hardware_platform_versions = proxy.virtual_hardware_platform_versions == null ? null : Helper.StringArrayToLongArray(proxy.virtual_hardware_platform_versions);
            control_domain = proxy.control_domain == null ? null : XenRef<VM>.Create(proxy.control_domain);
            updates_requiring_reboot = proxy.updates_requiring_reboot == null ? null : XenRef<Pool_update>.Create(proxy.updates_requiring_reboot);
            features = proxy.features == null ? null : XenRef<Feature>.Create(proxy.features);
            iscsi_iqn = proxy.iscsi_iqn == null ? null : proxy.iscsi_iqn;
            multipathing = (bool)proxy.multipathing;
            uefi_certificates = proxy.uefi_certificates == null ? null : proxy.uefi_certificates;
        }

        public Proxy_Host ToProxy()
        {
            Proxy_Host result_ = new Proxy_Host();
            result_.uuid = uuid ?? "";
            result_.name_label = name_label ?? "";
            result_.name_description = name_description ?? "";
            result_.memory_overhead = memory_overhead.ToString();
            result_.allowed_operations = allowed_operations == null ? new string[] {} : Helper.ObjectListToStringArray(allowed_operations);
            result_.current_operations = Maps.convert_to_proxy_string_host_allowed_operations(current_operations);
            result_.API_version_major = API_version_major.ToString();
            result_.API_version_minor = API_version_minor.ToString();
            result_.API_version_vendor = API_version_vendor ?? "";
            result_.API_version_vendor_implementation = Maps.convert_to_proxy_string_string(API_version_vendor_implementation);
            result_.enabled = enabled;
            result_.software_version = Maps.convert_to_proxy_string_string(software_version);
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.capabilities = capabilities;
            result_.cpu_configuration = Maps.convert_to_proxy_string_string(cpu_configuration);
            result_.sched_policy = sched_policy ?? "";
            result_.supported_bootloaders = supported_bootloaders;
            result_.resident_VMs = resident_VMs == null ? new string[] {} : Helper.RefListToStringArray(resident_VMs);
            result_.logging = Maps.convert_to_proxy_string_string(logging);
            result_.PIFs = PIFs == null ? new string[] {} : Helper.RefListToStringArray(PIFs);
            result_.suspend_image_sr = suspend_image_sr ?? "";
            result_.crash_dump_sr = crash_dump_sr ?? "";
            result_.crashdumps = crashdumps == null ? new string[] {} : Helper.RefListToStringArray(crashdumps);
            result_.patches = patches == null ? new string[] {} : Helper.RefListToStringArray(patches);
            result_.updates = updates == null ? new string[] {} : Helper.RefListToStringArray(updates);
            result_.PBDs = PBDs == null ? new string[] {} : Helper.RefListToStringArray(PBDs);
            result_.host_CPUs = host_CPUs == null ? new string[] {} : Helper.RefListToStringArray(host_CPUs);
            result_.cpu_info = Maps.convert_to_proxy_string_string(cpu_info);
            result_.hostname = hostname ?? "";
            result_.address = address ?? "";
            result_.metrics = metrics ?? "";
            result_.license_params = Maps.convert_to_proxy_string_string(license_params);
            result_.ha_statefiles = ha_statefiles;
            result_.ha_network_peers = ha_network_peers;
            result_.blobs = Maps.convert_to_proxy_string_XenRefBlob(blobs);
            result_.tags = tags;
            result_.external_auth_type = external_auth_type ?? "";
            result_.external_auth_service_name = external_auth_service_name ?? "";
            result_.external_auth_configuration = Maps.convert_to_proxy_string_string(external_auth_configuration);
            result_.edition = edition ?? "";
            result_.license_server = Maps.convert_to_proxy_string_string(license_server);
            result_.bios_strings = Maps.convert_to_proxy_string_string(bios_strings);
            result_.power_on_mode = power_on_mode ?? "";
            result_.power_on_config = Maps.convert_to_proxy_string_string(power_on_config);
            result_.local_cache_sr = local_cache_sr ?? "";
            result_.chipset_info = Maps.convert_to_proxy_string_string(chipset_info);
            result_.PCIs = PCIs == null ? new string[] {} : Helper.RefListToStringArray(PCIs);
            result_.PGPUs = PGPUs == null ? new string[] {} : Helper.RefListToStringArray(PGPUs);
            result_.PUSBs = PUSBs == null ? new string[] {} : Helper.RefListToStringArray(PUSBs);
            result_.ssl_legacy = ssl_legacy;
            result_.guest_VCPUs_params = Maps.convert_to_proxy_string_string(guest_VCPUs_params);
            result_.display = host_display_helper.ToString(display);
            result_.virtual_hardware_platform_versions = virtual_hardware_platform_versions == null ? new string[] {} : Helper.LongArrayToStringArray(virtual_hardware_platform_versions);
            result_.control_domain = control_domain ?? "";
            result_.updates_requiring_reboot = updates_requiring_reboot == null ? new string[] {} : Helper.RefListToStringArray(updates_requiring_reboot);
            result_.features = features == null ? new string[] {} : Helper.RefListToStringArray(features);
            result_.iscsi_iqn = iscsi_iqn ?? "";
            result_.multipathing = multipathing;
            result_.uefi_certificates = uefi_certificates ?? "";
            return result_;
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
                current_operations = Maps.convert_from_proxy_string_host_allowed_operations(Marshalling.ParseHashTable(table, "current_operations"));
            if (table.ContainsKey("API_version_major"))
                API_version_major = Marshalling.ParseLong(table, "API_version_major");
            if (table.ContainsKey("API_version_minor"))
                API_version_minor = Marshalling.ParseLong(table, "API_version_minor");
            if (table.ContainsKey("API_version_vendor"))
                API_version_vendor = Marshalling.ParseString(table, "API_version_vendor");
            if (table.ContainsKey("API_version_vendor_implementation"))
                API_version_vendor_implementation = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "API_version_vendor_implementation"));
            if (table.ContainsKey("enabled"))
                enabled = Marshalling.ParseBool(table, "enabled");
            if (table.ContainsKey("software_version"))
                software_version = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "software_version"));
            if (table.ContainsKey("other_config"))
                other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("capabilities"))
                capabilities = Marshalling.ParseStringArray(table, "capabilities");
            if (table.ContainsKey("cpu_configuration"))
                cpu_configuration = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "cpu_configuration"));
            if (table.ContainsKey("sched_policy"))
                sched_policy = Marshalling.ParseString(table, "sched_policy");
            if (table.ContainsKey("supported_bootloaders"))
                supported_bootloaders = Marshalling.ParseStringArray(table, "supported_bootloaders");
            if (table.ContainsKey("resident_VMs"))
                resident_VMs = Marshalling.ParseSetRef<VM>(table, "resident_VMs");
            if (table.ContainsKey("logging"))
                logging = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "logging"));
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
                cpu_info = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "cpu_info"));
            if (table.ContainsKey("hostname"))
                hostname = Marshalling.ParseString(table, "hostname");
            if (table.ContainsKey("address"))
                address = Marshalling.ParseString(table, "address");
            if (table.ContainsKey("metrics"))
                metrics = Marshalling.ParseRef<Host_metrics>(table, "metrics");
            if (table.ContainsKey("license_params"))
                license_params = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "license_params"));
            if (table.ContainsKey("ha_statefiles"))
                ha_statefiles = Marshalling.ParseStringArray(table, "ha_statefiles");
            if (table.ContainsKey("ha_network_peers"))
                ha_network_peers = Marshalling.ParseStringArray(table, "ha_network_peers");
            if (table.ContainsKey("blobs"))
                blobs = Maps.convert_from_proxy_string_XenRefBlob(Marshalling.ParseHashTable(table, "blobs"));
            if (table.ContainsKey("tags"))
                tags = Marshalling.ParseStringArray(table, "tags");
            if (table.ContainsKey("external_auth_type"))
                external_auth_type = Marshalling.ParseString(table, "external_auth_type");
            if (table.ContainsKey("external_auth_service_name"))
                external_auth_service_name = Marshalling.ParseString(table, "external_auth_service_name");
            if (table.ContainsKey("external_auth_configuration"))
                external_auth_configuration = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "external_auth_configuration"));
            if (table.ContainsKey("edition"))
                edition = Marshalling.ParseString(table, "edition");
            if (table.ContainsKey("license_server"))
                license_server = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "license_server"));
            if (table.ContainsKey("bios_strings"))
                bios_strings = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "bios_strings"));
            if (table.ContainsKey("power_on_mode"))
                power_on_mode = Marshalling.ParseString(table, "power_on_mode");
            if (table.ContainsKey("power_on_config"))
                power_on_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "power_on_config"));
            if (table.ContainsKey("local_cache_sr"))
                local_cache_sr = Marshalling.ParseRef<SR>(table, "local_cache_sr");
            if (table.ContainsKey("chipset_info"))
                chipset_info = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "chipset_info"));
            if (table.ContainsKey("PCIs"))
                PCIs = Marshalling.ParseSetRef<PCI>(table, "PCIs");
            if (table.ContainsKey("PGPUs"))
                PGPUs = Marshalling.ParseSetRef<PGPU>(table, "PGPUs");
            if (table.ContainsKey("PUSBs"))
                PUSBs = Marshalling.ParseSetRef<PUSB>(table, "PUSBs");
            if (table.ContainsKey("ssl_legacy"))
                ssl_legacy = Marshalling.ParseBool(table, "ssl_legacy");
            if (table.ContainsKey("guest_VCPUs_params"))
                guest_VCPUs_params = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "guest_VCPUs_params"));
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
        }

        public bool DeepEquals(Host other, bool ignoreCurrentOperations)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (!ignoreCurrentOperations && !Helper.AreEqual2(this.current_operations, other.current_operations))
                return false;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name_label, other._name_label) &&
                Helper.AreEqual2(this._name_description, other._name_description) &&
                Helper.AreEqual2(this._memory_overhead, other._memory_overhead) &&
                Helper.AreEqual2(this._allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(this._API_version_major, other._API_version_major) &&
                Helper.AreEqual2(this._API_version_minor, other._API_version_minor) &&
                Helper.AreEqual2(this._API_version_vendor, other._API_version_vendor) &&
                Helper.AreEqual2(this._API_version_vendor_implementation, other._API_version_vendor_implementation) &&
                Helper.AreEqual2(this._enabled, other._enabled) &&
                Helper.AreEqual2(this._software_version, other._software_version) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._capabilities, other._capabilities) &&
                Helper.AreEqual2(this._cpu_configuration, other._cpu_configuration) &&
                Helper.AreEqual2(this._sched_policy, other._sched_policy) &&
                Helper.AreEqual2(this._supported_bootloaders, other._supported_bootloaders) &&
                Helper.AreEqual2(this._resident_VMs, other._resident_VMs) &&
                Helper.AreEqual2(this._logging, other._logging) &&
                Helper.AreEqual2(this._PIFs, other._PIFs) &&
                Helper.AreEqual2(this._suspend_image_sr, other._suspend_image_sr) &&
                Helper.AreEqual2(this._crash_dump_sr, other._crash_dump_sr) &&
                Helper.AreEqual2(this._crashdumps, other._crashdumps) &&
                Helper.AreEqual2(this._patches, other._patches) &&
                Helper.AreEqual2(this._updates, other._updates) &&
                Helper.AreEqual2(this._PBDs, other._PBDs) &&
                Helper.AreEqual2(this._host_CPUs, other._host_CPUs) &&
                Helper.AreEqual2(this._cpu_info, other._cpu_info) &&
                Helper.AreEqual2(this._hostname, other._hostname) &&
                Helper.AreEqual2(this._address, other._address) &&
                Helper.AreEqual2(this._metrics, other._metrics) &&
                Helper.AreEqual2(this._license_params, other._license_params) &&
                Helper.AreEqual2(this._ha_statefiles, other._ha_statefiles) &&
                Helper.AreEqual2(this._ha_network_peers, other._ha_network_peers) &&
                Helper.AreEqual2(this._blobs, other._blobs) &&
                Helper.AreEqual2(this._tags, other._tags) &&
                Helper.AreEqual2(this._external_auth_type, other._external_auth_type) &&
                Helper.AreEqual2(this._external_auth_service_name, other._external_auth_service_name) &&
                Helper.AreEqual2(this._external_auth_configuration, other._external_auth_configuration) &&
                Helper.AreEqual2(this._edition, other._edition) &&
                Helper.AreEqual2(this._license_server, other._license_server) &&
                Helper.AreEqual2(this._bios_strings, other._bios_strings) &&
                Helper.AreEqual2(this._power_on_mode, other._power_on_mode) &&
                Helper.AreEqual2(this._power_on_config, other._power_on_config) &&
                Helper.AreEqual2(this._local_cache_sr, other._local_cache_sr) &&
                Helper.AreEqual2(this._chipset_info, other._chipset_info) &&
                Helper.AreEqual2(this._PCIs, other._PCIs) &&
                Helper.AreEqual2(this._PGPUs, other._PGPUs) &&
                Helper.AreEqual2(this._PUSBs, other._PUSBs) &&
                Helper.AreEqual2(this._ssl_legacy, other._ssl_legacy) &&
                Helper.AreEqual2(this._guest_VCPUs_params, other._guest_VCPUs_params) &&
                Helper.AreEqual2(this._display, other._display) &&
                Helper.AreEqual2(this._virtual_hardware_platform_versions, other._virtual_hardware_platform_versions) &&
                Helper.AreEqual2(this._control_domain, other._control_domain) &&
                Helper.AreEqual2(this._updates_requiring_reboot, other._updates_requiring_reboot) &&
                Helper.AreEqual2(this._features, other._features) &&
                Helper.AreEqual2(this._iscsi_iqn, other._iscsi_iqn) &&
                Helper.AreEqual2(this._multipathing, other._multipathing) &&
                Helper.AreEqual2(this._uefi_certificates, other._uefi_certificates);
        }

        internal static List<Host> ProxyArrayToObjectList(Proxy_Host[] input)
        {
            var result = new List<Host>();
            foreach (var item in input)
                result.Add(new Host(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, Host server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
                if (!Helper.AreEqual2(_name_label, server._name_label))
                {
                    Host.set_name_label(session, opaqueRef, _name_label);
                }
                if (!Helper.AreEqual2(_name_description, server._name_description))
                {
                    Host.set_name_description(session, opaqueRef, _name_description);
                }
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    Host.set_other_config(session, opaqueRef, _other_config);
                }
                if (!Helper.AreEqual2(_logging, server._logging))
                {
                    Host.set_logging(session, opaqueRef, _logging);
                }
                if (!Helper.AreEqual2(_suspend_image_sr, server._suspend_image_sr))
                {
                    Host.set_suspend_image_sr(session, opaqueRef, _suspend_image_sr);
                }
                if (!Helper.AreEqual2(_crash_dump_sr, server._crash_dump_sr))
                {
                    Host.set_crash_dump_sr(session, opaqueRef, _crash_dump_sr);
                }
                if (!Helper.AreEqual2(_hostname, server._hostname))
                {
                    Host.set_hostname(session, opaqueRef, _hostname);
                }
                if (!Helper.AreEqual2(_address, server._address))
                {
                    Host.set_address(session, opaqueRef, _address);
                }
                if (!Helper.AreEqual2(_tags, server._tags))
                {
                    Host.set_tags(session, opaqueRef, _tags);
                }
                if (!Helper.AreEqual2(_license_server, server._license_server))
                {
                    Host.set_license_server(session, opaqueRef, _license_server);
                }
                if (!Helper.AreEqual2(_guest_VCPUs_params, server._guest_VCPUs_params))
                {
                    Host.set_guest_VCPUs_params(session, opaqueRef, _guest_VCPUs_params);
                }
                if (!Helper.AreEqual2(_display, server._display))
                {
                    Host.set_display(session, opaqueRef, _display);
                }
                if (!Helper.AreEqual2(_ssl_legacy, server._ssl_legacy))
                {
                    Host.set_ssl_legacy(session, opaqueRef, _ssl_legacy);
                }
                if (!Helper.AreEqual2(_iscsi_iqn, server._iscsi_iqn))
                {
                    Host.set_iscsi_iqn(session, opaqueRef, _iscsi_iqn);
                }
                if (!Helper.AreEqual2(_multipathing, server._multipathing))
                {
                    Host.set_multipathing(session, opaqueRef, _multipathing);
                }
                if (!Helper.AreEqual2(_uefi_certificates, server._uefi_certificates))
                {
                    Host.set_uefi_certificates(session, opaqueRef, _uefi_certificates);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Host get_record(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_record(session.opaque_ref, _host);
            else
                return new Host(session.proxy.host_get_record(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the host instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<Host> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<Host>.Create(session.proxy.host_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get all the host instances with the given label.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        public static List<XenRef<Host>> get_by_name_label(Session session, string _label)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_by_name_label(session.opaque_ref, _label);
            else
                return XenRef<Host>.Create(session.proxy.host_get_by_name_label(session.opaque_ref, _label ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string get_uuid(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_uuid(session.opaque_ref, _host);
            else
                return session.proxy.host_get_uuid(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the name/label field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string get_name_label(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_name_label(session.opaque_ref, _host);
            else
                return session.proxy.host_get_name_label(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the name/description field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string get_name_description(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_name_description(session.opaque_ref, _host);
            else
                return session.proxy.host_get_name_description(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the memory/overhead field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static long get_memory_overhead(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_memory_overhead(session.opaque_ref, _host);
            else
                return long.Parse(session.proxy.host_get_memory_overhead(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the allowed_operations field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static List<host_allowed_operations> get_allowed_operations(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_allowed_operations(session.opaque_ref, _host);
            else
                return Helper.StringArrayToEnumList<host_allowed_operations>(session.proxy.host_get_allowed_operations(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the current_operations field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<string, host_allowed_operations> get_current_operations(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_current_operations(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_string_host_allowed_operations(session.proxy.host_get_current_operations(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the API_version/major field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static long get_API_version_major(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_api_version_major(session.opaque_ref, _host);
            else
                return long.Parse(session.proxy.host_get_api_version_major(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the API_version/minor field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static long get_API_version_minor(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_api_version_minor(session.opaque_ref, _host);
            else
                return long.Parse(session.proxy.host_get_api_version_minor(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the API_version/vendor field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string get_API_version_vendor(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_api_version_vendor(session.opaque_ref, _host);
            else
                return session.proxy.host_get_api_version_vendor(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the API_version/vendor_implementation field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<string, string> get_API_version_vendor_implementation(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_api_version_vendor_implementation(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.host_get_api_version_vendor_implementation(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the enabled field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static bool get_enabled(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_enabled(session.opaque_ref, _host);
            else
                return (bool)session.proxy.host_get_enabled(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the software_version field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<string, string> get_software_version(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_software_version(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.host_get_software_version(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the other_config field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<string, string> get_other_config(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_other_config(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.host_get_other_config(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the capabilities field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string[] get_capabilities(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_capabilities(session.opaque_ref, _host);
            else
                return (string [])session.proxy.host_get_capabilities(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the cpu_configuration field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<string, string> get_cpu_configuration(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_cpu_configuration(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.host_get_cpu_configuration(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the sched_policy field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string get_sched_policy(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_sched_policy(session.opaque_ref, _host);
            else
                return session.proxy.host_get_sched_policy(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the supported_bootloaders field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string[] get_supported_bootloaders(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_supported_bootloaders(session.opaque_ref, _host);
            else
                return (string [])session.proxy.host_get_supported_bootloaders(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the resident_VMs field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static List<XenRef<VM>> get_resident_VMs(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_resident_vms(session.opaque_ref, _host);
            else
                return XenRef<VM>.Create(session.proxy.host_get_resident_vms(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the logging field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<string, string> get_logging(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_logging(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.host_get_logging(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the PIFs field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static List<XenRef<PIF>> get_PIFs(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_pifs(session.opaque_ref, _host);
            else
                return XenRef<PIF>.Create(session.proxy.host_get_pifs(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the suspend_image_sr field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<SR> get_suspend_image_sr(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_suspend_image_sr(session.opaque_ref, _host);
            else
                return XenRef<SR>.Create(session.proxy.host_get_suspend_image_sr(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the crash_dump_sr field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<SR> get_crash_dump_sr(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_crash_dump_sr(session.opaque_ref, _host);
            else
                return XenRef<SR>.Create(session.proxy.host_get_crash_dump_sr(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the crashdumps field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static List<XenRef<Host_crashdump>> get_crashdumps(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_crashdumps(session.opaque_ref, _host);
            else
                return XenRef<Host_crashdump>.Create(session.proxy.host_get_crashdumps(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the patches field of the given host.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        [Deprecated("XenServer 7.1")]
        public static List<XenRef<Host_patch>> get_patches(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_patches(session.opaque_ref, _host);
            else
                return XenRef<Host_patch>.Create(session.proxy.host_get_patches(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the updates field of the given host.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static List<XenRef<Pool_update>> get_updates(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_updates(session.opaque_ref, _host);
            else
                return XenRef<Pool_update>.Create(session.proxy.host_get_updates(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the PBDs field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static List<XenRef<PBD>> get_PBDs(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_pbds(session.opaque_ref, _host);
            else
                return XenRef<PBD>.Create(session.proxy.host_get_pbds(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the host_CPUs field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static List<XenRef<Host_cpu>> get_host_CPUs(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_host_cpus(session.opaque_ref, _host);
            else
                return XenRef<Host_cpu>.Create(session.proxy.host_get_host_cpus(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the cpu_info field of the given host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<string, string> get_cpu_info(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_cpu_info(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.host_get_cpu_info(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the hostname field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string get_hostname(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_hostname(session.opaque_ref, _host);
            else
                return session.proxy.host_get_hostname(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the address field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string get_address(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_address(session.opaque_ref, _host);
            else
                return session.proxy.host_get_address(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the metrics field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Host_metrics> get_metrics(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_metrics(session.opaque_ref, _host);
            else
                return XenRef<Host_metrics>.Create(session.proxy.host_get_metrics(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the license_params field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<string, string> get_license_params(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_license_params(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.host_get_license_params(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the ha_statefiles field of the given host.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string[] get_ha_statefiles(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_ha_statefiles(session.opaque_ref, _host);
            else
                return (string [])session.proxy.host_get_ha_statefiles(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the ha_network_peers field of the given host.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string[] get_ha_network_peers(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_ha_network_peers(session.opaque_ref, _host);
            else
                return (string [])session.proxy.host_get_ha_network_peers(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the blobs field of the given host.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<string, XenRef<Blob>> get_blobs(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_blobs(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_string_XenRefBlob(session.proxy.host_get_blobs(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the tags field of the given host.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string[] get_tags(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_tags(session.opaque_ref, _host);
            else
                return (string [])session.proxy.host_get_tags(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the external_auth_type field of the given host.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string get_external_auth_type(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_external_auth_type(session.opaque_ref, _host);
            else
                return session.proxy.host_get_external_auth_type(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the external_auth_service_name field of the given host.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string get_external_auth_service_name(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_external_auth_service_name(session.opaque_ref, _host);
            else
                return session.proxy.host_get_external_auth_service_name(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the external_auth_configuration field of the given host.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<string, string> get_external_auth_configuration(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_external_auth_configuration(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.host_get_external_auth_configuration(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the edition field of the given host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string get_edition(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_edition(session.opaque_ref, _host);
            else
                return session.proxy.host_get_edition(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the license_server field of the given host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<string, string> get_license_server(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_license_server(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.host_get_license_server(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the bios_strings field of the given host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<string, string> get_bios_strings(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_bios_strings(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.host_get_bios_strings(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the power_on_mode field of the given host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string get_power_on_mode(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_power_on_mode(session.opaque_ref, _host);
            else
                return session.proxy.host_get_power_on_mode(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the power_on_config field of the given host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<string, string> get_power_on_config(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_power_on_config(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.host_get_power_on_config(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the local_cache_sr field of the given host.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<SR> get_local_cache_sr(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_local_cache_sr(session.opaque_ref, _host);
            else
                return XenRef<SR>.Create(session.proxy.host_get_local_cache_sr(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the chipset_info field of the given host.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<string, string> get_chipset_info(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_chipset_info(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.host_get_chipset_info(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the PCIs field of the given host.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static List<XenRef<PCI>> get_PCIs(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_pcis(session.opaque_ref, _host);
            else
                return XenRef<PCI>.Create(session.proxy.host_get_pcis(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the PGPUs field of the given host.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static List<XenRef<PGPU>> get_PGPUs(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_pgpus(session.opaque_ref, _host);
            else
                return XenRef<PGPU>.Create(session.proxy.host_get_pgpus(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the PUSBs field of the given host.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static List<XenRef<PUSB>> get_PUSBs(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_pusbs(session.opaque_ref, _host);
            else
                return XenRef<PUSB>.Create(session.proxy.host_get_pusbs(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the ssl_legacy field of the given host.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static bool get_ssl_legacy(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_ssl_legacy(session.opaque_ref, _host);
            else
                return (bool)session.proxy.host_get_ssl_legacy(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the guest_VCPUs_params field of the given host.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<string, string> get_guest_VCPUs_params(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_guest_vcpus_params(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.host_get_guest_vcpus_params(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the display field of the given host.
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static host_display get_display(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_display(session.opaque_ref, _host);
            else
                return (host_display)Helper.EnumParseDefault(typeof(host_display), (string)session.proxy.host_get_display(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the virtual_hardware_platform_versions field of the given host.
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static long[] get_virtual_hardware_platform_versions(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_virtual_hardware_platform_versions(session.opaque_ref, _host);
            else
                return Helper.StringArrayToLongArray(session.proxy.host_get_virtual_hardware_platform_versions(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the control_domain field of the given host.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<VM> get_control_domain(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_control_domain(session.opaque_ref, _host);
            else
                return XenRef<VM>.Create(session.proxy.host_get_control_domain(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the updates_requiring_reboot field of the given host.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static List<XenRef<Pool_update>> get_updates_requiring_reboot(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_updates_requiring_reboot(session.opaque_ref, _host);
            else
                return XenRef<Pool_update>.Create(session.proxy.host_get_updates_requiring_reboot(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the features field of the given host.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static List<XenRef<Feature>> get_features(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_features(session.opaque_ref, _host);
            else
                return XenRef<Feature>.Create(session.proxy.host_get_features(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the iscsi_iqn field of the given host.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string get_iscsi_iqn(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_iscsi_iqn(session.opaque_ref, _host);
            else
                return session.proxy.host_get_iscsi_iqn(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the multipathing field of the given host.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static bool get_multipathing(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_multipathing(session.opaque_ref, _host);
            else
                return (bool)session.proxy.host_get_multipathing(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the uefi_certificates field of the given host.
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string get_uefi_certificates(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_uefi_certificates(session.opaque_ref, _host);
            else
                return session.proxy.host_get_uefi_certificates(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Set the name/label field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_label">New value to set</param>
        public static void set_name_label(Session session, string _host, string _label)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_name_label(session.opaque_ref, _host, _label);
            else
                session.proxy.host_set_name_label(session.opaque_ref, _host ?? "", _label ?? "").parse();
        }

        /// <summary>
        /// Set the name/description field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_description">New value to set</param>
        public static void set_name_description(Session session, string _host, string _description)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_name_description(session.opaque_ref, _host, _description);
            else
                session.proxy.host_set_name_description(session.opaque_ref, _host ?? "", _description ?? "").parse();
        }

        /// <summary>
        /// Set the other_config field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _host, Dictionary<string, string> _other_config)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_other_config(session.opaque_ref, _host, _other_config);
            else
                session.proxy.host_set_other_config(session.opaque_ref, _host ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _host, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_add_to_other_config(session.opaque_ref, _host, _key, _value);
            else
                session.proxy.host_add_to_other_config(session.opaque_ref, _host ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given host.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _host, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_remove_from_other_config(session.opaque_ref, _host, _key);
            else
                session.proxy.host_remove_from_other_config(session.opaque_ref, _host ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// Set the logging field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_logging">New value to set</param>
        public static void set_logging(Session session, string _host, Dictionary<string, string> _logging)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_logging(session.opaque_ref, _host, _logging);
            else
                session.proxy.host_set_logging(session.opaque_ref, _host ?? "", Maps.convert_to_proxy_string_string(_logging)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the logging field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_logging(Session session, string _host, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_add_to_logging(session.opaque_ref, _host, _key, _value);
            else
                session.proxy.host_add_to_logging(session.opaque_ref, _host ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the logging field of the given host.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_logging(Session session, string _host, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_remove_from_logging(session.opaque_ref, _host, _key);
            else
                session.proxy.host_remove_from_logging(session.opaque_ref, _host ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// Set the suspend_image_sr field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_suspend_image_sr">New value to set</param>
        public static void set_suspend_image_sr(Session session, string _host, string _suspend_image_sr)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_suspend_image_sr(session.opaque_ref, _host, _suspend_image_sr);
            else
                session.proxy.host_set_suspend_image_sr(session.opaque_ref, _host ?? "", _suspend_image_sr ?? "").parse();
        }

        /// <summary>
        /// Set the crash_dump_sr field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_crash_dump_sr">New value to set</param>
        public static void set_crash_dump_sr(Session session, string _host, string _crash_dump_sr)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_crash_dump_sr(session.opaque_ref, _host, _crash_dump_sr);
            else
                session.proxy.host_set_crash_dump_sr(session.opaque_ref, _host ?? "", _crash_dump_sr ?? "").parse();
        }

        /// <summary>
        /// Set the hostname field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_hostname">New value to set</param>
        public static void set_hostname(Session session, string _host, string _hostname)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_hostname(session.opaque_ref, _host, _hostname);
            else
                session.proxy.host_set_hostname(session.opaque_ref, _host ?? "", _hostname ?? "").parse();
        }

        /// <summary>
        /// Set the address field of the given host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_address">New value to set</param>
        public static void set_address(Session session, string _host, string _address)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_address(session.opaque_ref, _host, _address);
            else
                session.proxy.host_set_address(session.opaque_ref, _host ?? "", _address ?? "").parse();
        }

        /// <summary>
        /// Set the tags field of the given host.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_tags">New value to set</param>
        public static void set_tags(Session session, string _host, string[] _tags)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_tags(session.opaque_ref, _host, _tags);
            else
                session.proxy.host_set_tags(session.opaque_ref, _host ?? "", _tags).parse();
        }

        /// <summary>
        /// Add the given value to the tags field of the given host.  If the value is already in that Set, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">New value to add</param>
        public static void add_tags(Session session, string _host, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_add_tags(session.opaque_ref, _host, _value);
            else
                session.proxy.host_add_tags(session.opaque_ref, _host ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given value from the tags field of the given host.  If the value is not in that Set, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">Value to remove</param>
        public static void remove_tags(Session session, string _host, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_remove_tags(session.opaque_ref, _host, _value);
            else
                session.proxy.host_remove_tags(session.opaque_ref, _host ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Set the license_server field of the given host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_license_server">New value to set</param>
        public static void set_license_server(Session session, string _host, Dictionary<string, string> _license_server)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_license_server(session.opaque_ref, _host, _license_server);
            else
                session.proxy.host_set_license_server(session.opaque_ref, _host ?? "", Maps.convert_to_proxy_string_string(_license_server)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the license_server field of the given host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_license_server(Session session, string _host, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_add_to_license_server(session.opaque_ref, _host, _key, _value);
            else
                session.proxy.host_add_to_license_server(session.opaque_ref, _host ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the license_server field of the given host.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_license_server(Session session, string _host, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_remove_from_license_server(session.opaque_ref, _host, _key);
            else
                session.proxy.host_remove_from_license_server(session.opaque_ref, _host ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// Set the guest_VCPUs_params field of the given host.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_guest_vcpus_params">New value to set</param>
        public static void set_guest_VCPUs_params(Session session, string _host, Dictionary<string, string> _guest_vcpus_params)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_guest_vcpus_params(session.opaque_ref, _host, _guest_vcpus_params);
            else
                session.proxy.host_set_guest_vcpus_params(session.opaque_ref, _host ?? "", Maps.convert_to_proxy_string_string(_guest_vcpus_params)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the guest_VCPUs_params field of the given host.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_guest_VCPUs_params(Session session, string _host, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_add_to_guest_vcpus_params(session.opaque_ref, _host, _key, _value);
            else
                session.proxy.host_add_to_guest_vcpus_params(session.opaque_ref, _host ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the guest_VCPUs_params field of the given host.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_guest_VCPUs_params(Session session, string _host, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_remove_from_guest_vcpus_params(session.opaque_ref, _host, _key);
            else
                session.proxy.host_remove_from_guest_vcpus_params(session.opaque_ref, _host ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// Set the display field of the given host.
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_display">New value to set</param>
        public static void set_display(Session session, string _host, host_display _display)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_display(session.opaque_ref, _host, _display);
            else
                session.proxy.host_set_display(session.opaque_ref, _host ?? "", host_display_helper.ToString(_display)).parse();
        }

        /// <summary>
        /// Puts the host into a state in which no new VMs can be started. Currently active VMs on the host continue to execute.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static void disable(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_disable(session.opaque_ref, _host);
            else
                session.proxy.host_disable(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Puts the host into a state in which no new VMs can be started. Currently active VMs on the host continue to execute.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_disable(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_disable(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_disable(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Puts the host into a state in which new VMs can be started.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static void enable(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_enable(session.opaque_ref, _host);
            else
                session.proxy.host_enable(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Puts the host into a state in which new VMs can be started.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_enable(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_enable(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_enable(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Shutdown the host. (This function can only be called if there are no currently running VMs on the host and it is disabled.)
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static void shutdown(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_shutdown(session.opaque_ref, _host);
            else
                session.proxy.host_shutdown(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Shutdown the host. (This function can only be called if there are no currently running VMs on the host and it is disabled.)
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_shutdown(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_shutdown(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_shutdown(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Reboot the host. (This function can only be called if there are no currently running VMs on the host and it is disabled.)
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static void reboot(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_reboot(session.opaque_ref, _host);
            else
                session.proxy.host_reboot(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Reboot the host. (This function can only be called if there are no currently running VMs on the host and it is disabled.)
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_reboot(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_reboot(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_reboot(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the host xen dmesg.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string dmesg(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_dmesg(session.opaque_ref, _host);
            else
                return session.proxy.host_dmesg(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the host xen dmesg.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_dmesg(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_dmesg(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_dmesg(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the host xen dmesg, and clear the buffer.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string dmesg_clear(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_dmesg_clear(session.opaque_ref, _host);
            else
                return session.proxy.host_dmesg_clear(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the host xen dmesg, and clear the buffer.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_dmesg_clear(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_dmesg_clear(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_dmesg_clear(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the host's log file
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string get_log(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_log(session.opaque_ref, _host);
            else
                return session.proxy.host_get_log(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the host's log file
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_get_log(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_get_log(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_get_log(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Inject the given string as debugging keys into Xen
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_keys">The keys to send</param>
        public static void send_debug_keys(Session session, string _host, string _keys)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_send_debug_keys(session.opaque_ref, _host, _keys);
            else
                session.proxy.host_send_debug_keys(session.opaque_ref, _host ?? "", _keys ?? "").parse();
        }

        /// <summary>
        /// Inject the given string as debugging keys into Xen
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_keys">The keys to send</param>
        public static XenRef<Task> async_send_debug_keys(Session session, string _host, string _keys)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_send_debug_keys(session.opaque_ref, _host, _keys);
          else
              return XenRef<Task>.Create(session.proxy.async_host_send_debug_keys(session.opaque_ref, _host ?? "", _keys ?? "").parse());
        }

        /// <summary>
        /// Run xen-bugtool --yestoall and upload the output to support
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_url">The URL to upload to</param>
        /// <param name="_options">Extra configuration operations</param>
        public static void bugreport_upload(Session session, string _host, string _url, Dictionary<string, string> _options)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_bugreport_upload(session.opaque_ref, _host, _url, _options);
            else
                session.proxy.host_bugreport_upload(session.opaque_ref, _host ?? "", _url ?? "", Maps.convert_to_proxy_string_string(_options)).parse();
        }

        /// <summary>
        /// Run xen-bugtool --yestoall and upload the output to support
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_url">The URL to upload to</param>
        /// <param name="_options">Extra configuration operations</param>
        public static XenRef<Task> async_bugreport_upload(Session session, string _host, string _url, Dictionary<string, string> _options)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_bugreport_upload(session.opaque_ref, _host, _url, _options);
          else
              return XenRef<Task>.Create(session.proxy.async_host_bugreport_upload(session.opaque_ref, _host ?? "", _url ?? "", Maps.convert_to_proxy_string_string(_options)).parse());
        }

        /// <summary>
        /// List all supported methods
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static string[] list_methods(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_list_methods(session.opaque_ref);
            else
                return (string [])session.proxy.host_list_methods(session.opaque_ref).parse();
        }

        /// <summary>
        /// Apply a new license to a host
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_contents">The contents of the license file, base64 encoded</param>
        public static void license_apply(Session session, string _host, string _contents)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_license_apply(session.opaque_ref, _host, _contents);
            else
                session.proxy.host_license_apply(session.opaque_ref, _host ?? "", _contents ?? "").parse();
        }

        /// <summary>
        /// Apply a new license to a host
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_contents">The contents of the license file, base64 encoded</param>
        public static XenRef<Task> async_license_apply(Session session, string _host, string _contents)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_license_apply(session.opaque_ref, _host, _contents);
          else
              return XenRef<Task>.Create(session.proxy.async_host_license_apply(session.opaque_ref, _host ?? "", _contents ?? "").parse());
        }

        /// <summary>
        /// Apply a new license to a host
        /// First published in XenServer 6.5 SP1 Hotfix 31.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_contents">The contents of the license file, base64 encoded</param>
        public static void license_add(Session session, string _host, string _contents)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_license_add(session.opaque_ref, _host, _contents);
            else
                session.proxy.host_license_add(session.opaque_ref, _host ?? "", _contents ?? "").parse();
        }

        /// <summary>
        /// Apply a new license to a host
        /// First published in XenServer 6.5 SP1 Hotfix 31.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_contents">The contents of the license file, base64 encoded</param>
        public static XenRef<Task> async_license_add(Session session, string _host, string _contents)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_license_add(session.opaque_ref, _host, _contents);
          else
              return XenRef<Task>.Create(session.proxy.async_host_license_add(session.opaque_ref, _host ?? "", _contents ?? "").parse());
        }

        /// <summary>
        /// Remove any license file from the specified host, and switch that host to the unlicensed edition
        /// First published in XenServer 6.5 SP1 Hotfix 31.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static void license_remove(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_license_remove(session.opaque_ref, _host);
            else
                session.proxy.host_license_remove(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Remove any license file from the specified host, and switch that host to the unlicensed edition
        /// First published in XenServer 6.5 SP1 Hotfix 31.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_license_remove(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_license_remove(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_license_remove(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Destroy specified host record in database
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static void destroy(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_destroy(session.opaque_ref, _host);
            else
                session.proxy.host_destroy(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Destroy specified host record in database
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_destroy(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_destroy(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_destroy(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Attempt to power-on the host (if the capability exists).
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static void power_on(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_power_on(session.opaque_ref, _host);
            else
                session.proxy.host_power_on(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Attempt to power-on the host (if the capability exists).
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_power_on(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_power_on(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_power_on(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// This call disables HA on the local host. This should only be used with extreme care.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_soft">Disable HA temporarily, revert upon host reboot or further changes, idempotent First published in XenServer 7.1.</param>
        public static void emergency_ha_disable(Session session, bool _soft)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_emergency_ha_disable(session.opaque_ref, _soft);
            else
                session.proxy.host_emergency_ha_disable(session.opaque_ref, _soft).parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static List<Data_source> get_data_sources(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_data_sources(session.opaque_ref, _host);
            else
                return Data_source.ProxyArrayToObjectList(session.proxy.host_get_data_sources(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Start recording the specified data source
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_data_source">The data source to record</param>
        public static void record_data_source(Session session, string _host, string _data_source)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_record_data_source(session.opaque_ref, _host, _data_source);
            else
                session.proxy.host_record_data_source(session.opaque_ref, _host ?? "", _data_source ?? "").parse();
        }

        /// <summary>
        /// Query the latest value of the specified data source
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_data_source">The data source to query</param>
        public static double query_data_source(Session session, string _host, string _data_source)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_query_data_source(session.opaque_ref, _host, _data_source);
            else
                return Convert.ToDouble(session.proxy.host_query_data_source(session.opaque_ref, _host ?? "", _data_source ?? "").parse());
        }

        /// <summary>
        /// Forget the recorded statistics related to the specified data source
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_data_source">The data source whose archives are to be forgotten</param>
        public static void forget_data_source_archives(Session session, string _host, string _data_source)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_forget_data_source_archives(session.opaque_ref, _host, _data_source);
            else
                session.proxy.host_forget_data_source_archives(session.opaque_ref, _host ?? "", _data_source ?? "").parse();
        }

        /// <summary>
        /// Check this host can be evacuated.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static void assert_can_evacuate(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_assert_can_evacuate(session.opaque_ref, _host);
            else
                session.proxy.host_assert_can_evacuate(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Check this host can be evacuated.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_assert_can_evacuate(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_assert_can_evacuate(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_assert_can_evacuate(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Return a set of VMs which prevent the host being evacuated, with per-VM error codes
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<XenRef<VM>, string[]> get_vms_which_prevent_evacuation(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_vms_which_prevent_evacuation(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_XenRefVM_string_array(session.proxy.host_get_vms_which_prevent_evacuation(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Return a set of VMs which prevent the host being evacuated, with per-VM error codes
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_get_vms_which_prevent_evacuation(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_get_vms_which_prevent_evacuation(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_get_vms_which_prevent_evacuation(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Return a set of VMs which are not co-operating with the host's memory control system
        /// First published in XenServer 5.6.
        /// Deprecated since XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        [Deprecated("XenServer 6.1")]
        public static List<XenRef<VM>> get_uncooperative_resident_VMs(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_uncooperative_resident_vms(session.opaque_ref, _host);
            else
                return XenRef<VM>.Create(session.proxy.host_get_uncooperative_resident_vms(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Return a set of VMs which are not co-operating with the host's memory control system
        /// First published in XenServer 5.6.
        /// Deprecated since XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        [Deprecated("XenServer 6.1")]
        public static XenRef<Task> async_get_uncooperative_resident_VMs(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_get_uncooperative_resident_vms(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_get_uncooperative_resident_vms(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Migrate all VMs off of this host, where possible.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static void evacuate(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_evacuate(session.opaque_ref, _host);
            else
                session.proxy.host_evacuate(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Migrate all VMs off of this host, where possible.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_evacuate(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_evacuate(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_evacuate(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Re-configure syslog logging
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static void syslog_reconfigure(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_syslog_reconfigure(session.opaque_ref, _host);
            else
                session.proxy.host_syslog_reconfigure(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Re-configure syslog logging
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_syslog_reconfigure(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_syslog_reconfigure(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_syslog_reconfigure(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Reconfigure the management network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">reference to a PIF object corresponding to the management interface</param>
        public static void management_reconfigure(Session session, string _pif)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_management_reconfigure(session.opaque_ref, _pif);
            else
                session.proxy.host_management_reconfigure(session.opaque_ref, _pif ?? "").parse();
        }

        /// <summary>
        /// Reconfigure the management network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">reference to a PIF object corresponding to the management interface</param>
        public static XenRef<Task> async_management_reconfigure(Session session, string _pif)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_management_reconfigure(session.opaque_ref, _pif);
          else
              return XenRef<Task>.Create(session.proxy.async_host_management_reconfigure(session.opaque_ref, _pif ?? "").parse());
        }

        /// <summary>
        /// Reconfigure the management network interface. Should only be used if Host.management_reconfigure is impossible because the network configuration is broken.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_interface">name of the interface to use as a management interface</param>
        public static void local_management_reconfigure(Session session, string _interface)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_local_management_reconfigure(session.opaque_ref, _interface);
            else
                session.proxy.host_local_management_reconfigure(session.opaque_ref, _interface ?? "").parse();
        }

        /// <summary>
        /// Disable the management network interface
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        public static void management_disable(Session session)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_management_disable(session.opaque_ref);
            else
                session.proxy.host_management_disable(session.opaque_ref).parse();
        }

        /// <summary>
        /// Returns the management interface for the specified host
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<PIF> get_management_interface(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_management_interface(session.opaque_ref, _host);
            else
                return XenRef<PIF>.Create(session.proxy.host_get_management_interface(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Returns the management interface for the specified host
        /// Experimental. First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_get_management_interface(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_get_management_interface(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_get_management_interface(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string get_system_status_capabilities(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_system_status_capabilities(session.opaque_ref, _host);
            else
                return session.proxy.host_get_system_status_capabilities(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Restarts the agent after a 10 second pause. WARNING: this is a dangerous operation. Any operations in progress will be aborted, and unrecoverable data loss may occur. The caller is responsible for ensuring that there are no operations in progress when this method is called.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static void restart_agent(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_restart_agent(session.opaque_ref, _host);
            else
                session.proxy.host_restart_agent(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Restarts the agent after a 10 second pause. WARNING: this is a dangerous operation. Any operations in progress will be aborted, and unrecoverable data loss may occur. The caller is responsible for ensuring that there are no operations in progress when this method is called.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_restart_agent(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_restart_agent(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_restart_agent(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Shuts the agent down after a 10 second pause. WARNING: this is a dangerous operation. Any operations in progress will be aborted, and unrecoverable data loss may occur. The caller is responsible for ensuring that there are no operations in progress when this method is called.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static void shutdown_agent(Session session)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_shutdown_agent(session.opaque_ref);
            else
                session.proxy.host_shutdown_agent(session.opaque_ref).parse();
        }

        /// <summary>
        /// Sets the host name to the specified string.  Both the API and lower-level system hostname are changed immediately.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_hostname">The new host name</param>
        public static void set_hostname_live(Session session, string _host, string _hostname)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_hostname_live(session.opaque_ref, _host, _hostname);
            else
                session.proxy.host_set_hostname_live(session.opaque_ref, _host ?? "", _hostname ?? "").parse();
        }

        /// <summary>
        /// Computes the amount of free memory on the host.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static long compute_free_memory(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_compute_free_memory(session.opaque_ref, _host);
            else
                return long.Parse(session.proxy.host_compute_free_memory(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Computes the amount of free memory on the host.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_compute_free_memory(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_compute_free_memory(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_compute_free_memory(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Computes the virtualization memory overhead of a host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static long compute_memory_overhead(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_compute_memory_overhead(session.opaque_ref, _host);
            else
                return long.Parse(session.proxy.host_compute_memory_overhead(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Computes the virtualization memory overhead of a host.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_compute_memory_overhead(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_compute_memory_overhead(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_compute_memory_overhead(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// This causes the synchronisation of the non-database data (messages, RRDs and so on) stored on the master to be synchronised with the host
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static void sync_data(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_sync_data(session.opaque_ref, _host);
            else
                session.proxy.host_sync_data(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// This causes the RRDs to be backed up to the master
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_delay">Delay in seconds from when the call is received to perform the backup</param>
        public static void backup_rrds(Session session, string _host, double _delay)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_backup_rrds(session.opaque_ref, _host, _delay);
            else
                session.proxy.host_backup_rrds(session.opaque_ref, _host ?? "", _delay).parse();
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this host
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        public static XenRef<Blob> create_new_blob(Session session, string _host, string _name, string _mime_type)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_create_new_blob(session.opaque_ref, _host, _name, _mime_type);
            else
                return XenRef<Blob>.Create(session.proxy.host_create_new_blob(session.opaque_ref, _host ?? "", _name ?? "", _mime_type ?? "").parse());
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this host
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        public static XenRef<Task> async_create_new_blob(Session session, string _host, string _name, string _mime_type)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_create_new_blob(session.opaque_ref, _host, _name, _mime_type);
          else
              return XenRef<Task>.Create(session.proxy.async_host_create_new_blob(session.opaque_ref, _host ?? "", _name ?? "", _mime_type ?? "").parse());
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
        public static XenRef<Blob> create_new_blob(Session session, string _host, string _name, string _mime_type, bool _public)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_create_new_blob(session.opaque_ref, _host, _name, _mime_type, _public);
            else
                return XenRef<Blob>.Create(session.proxy.host_create_new_blob(session.opaque_ref, _host ?? "", _name ?? "", _mime_type ?? "", _public).parse());
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
        public static XenRef<Task> async_create_new_blob(Session session, string _host, string _name, string _mime_type, bool _public)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_create_new_blob(session.opaque_ref, _host, _name, _mime_type, _public);
          else
              return XenRef<Task>.Create(session.proxy.async_host_create_new_blob(session.opaque_ref, _host ?? "", _name ?? "", _mime_type ?? "", _public).parse());
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
        public static string call_plugin(Session session, string _host, string _plugin, string _fn, Dictionary<string, string> _args)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_call_plugin(session.opaque_ref, _host, _plugin, _fn, _args);
            else
                return session.proxy.host_call_plugin(session.opaque_ref, _host ?? "", _plugin ?? "", _fn ?? "", Maps.convert_to_proxy_string_string(_args)).parse();
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
        public static XenRef<Task> async_call_plugin(Session session, string _host, string _plugin, string _fn, Dictionary<string, string> _args)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_call_plugin(session.opaque_ref, _host, _plugin, _fn, _args);
          else
              return XenRef<Task>.Create(session.proxy.async_host_call_plugin(session.opaque_ref, _host ?? "", _plugin ?? "", _fn ?? "", Maps.convert_to_proxy_string_string(_args)).parse());
        }

        /// <summary>
        /// Return true if the extension is available on the host
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_name">The name of the API call</param>
        public static bool has_extension(Session session, string _host, string _name)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_has_extension(session.opaque_ref, _host, _name);
            else
                return (bool)session.proxy.host_has_extension(session.opaque_ref, _host ?? "", _name ?? "").parse();
        }

        /// <summary>
        /// Return true if the extension is available on the host
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_name">The name of the API call</param>
        public static XenRef<Task> async_has_extension(Session session, string _host, string _name)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_has_extension(session.opaque_ref, _host, _name);
          else
              return XenRef<Task>.Create(session.proxy.async_host_has_extension(session.opaque_ref, _host ?? "", _name ?? "").parse());
        }

        /// <summary>
        /// Call an API extension on this host
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_call">Rpc call for the extension</param>
        public static string call_extension(Session session, string _host, string _call)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_call_extension(session.opaque_ref, _host, _call);
            else
                return session.proxy.host_call_extension(session.opaque_ref, _host ?? "", _call ?? "").parse();
        }

        /// <summary>
        /// This call queries the host's clock for the current time
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static DateTime get_servertime(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_servertime(session.opaque_ref, _host);
            else
                return session.proxy.host_get_servertime(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// This call queries the host's clock for the current time in the host's local timezone
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static DateTime get_server_localtime(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_server_localtime(session.opaque_ref, _host);
            else
                return session.proxy.host_get_server_localtime(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// This call enables external authentication on a host
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_config">A list of key-values containing the configuration data</param>
        /// <param name="_service_name">The name of the service</param>
        /// <param name="_auth_type">The type of authentication (e.g. AD for Active Directory)</param>
        public static void enable_external_auth(Session session, string _host, Dictionary<string, string> _config, string _service_name, string _auth_type)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_enable_external_auth(session.opaque_ref, _host, _config, _service_name, _auth_type);
            else
                session.proxy.host_enable_external_auth(session.opaque_ref, _host ?? "", Maps.convert_to_proxy_string_string(_config), _service_name ?? "", _auth_type ?? "").parse();
        }

        /// <summary>
        /// This call disables external authentication on the local host
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_config">Optional parameters as a list of key-values containing the configuration data</param>
        public static void disable_external_auth(Session session, string _host, Dictionary<string, string> _config)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_disable_external_auth(session.opaque_ref, _host, _config);
            else
                session.proxy.host_disable_external_auth(session.opaque_ref, _host ?? "", Maps.convert_to_proxy_string_string(_config)).parse();
        }

        /// <summary>
        /// Retrieves recommended host migrations to perform when evacuating the host from the wlb server. If a VM cannot be migrated from the host the reason is listed instead of a recommendation.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static Dictionary<XenRef<VM>, string[]> retrieve_wlb_evacuate_recommendations(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_retrieve_wlb_evacuate_recommendations(session.opaque_ref, _host);
            else
                return Maps.convert_from_proxy_XenRefVM_string_array(session.proxy.host_retrieve_wlb_evacuate_recommendations(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Retrieves recommended host migrations to perform when evacuating the host from the wlb server. If a VM cannot be migrated from the host the reason is listed instead of a recommendation.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_retrieve_wlb_evacuate_recommendations(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_retrieve_wlb_evacuate_recommendations(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_retrieve_wlb_evacuate_recommendations(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Get the installed server public TLS certificate.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static string get_server_certificate(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_server_certificate(session.opaque_ref, _host);
            else
                return session.proxy.host_get_server_certificate(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Get the installed server public TLS certificate.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_get_server_certificate(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_get_server_certificate(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_get_server_certificate(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Change to another edition, or reactivate the current edition after a license has expired. This may be subject to the successful checkout of an appropriate license.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_edition">The requested edition</param>
        public static void apply_edition(Session session, string _host, string _edition)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_apply_edition(session.opaque_ref, _host, _edition);
            else
                session.proxy.host_apply_edition(session.opaque_ref, _host ?? "", _edition ?? "").parse();
        }

        /// <summary>
        /// Change to another edition, or reactivate the current edition after a license has expired. This may be subject to the successful checkout of an appropriate license.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_edition">The requested edition</param>
        /// <param name="_force">Update the license params even if the apply call fails First published in XenServer 6.2.</param>
        public static void apply_edition(Session session, string _host, string _edition, bool _force)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_apply_edition(session.opaque_ref, _host, _edition, _force);
            else
                session.proxy.host_apply_edition(session.opaque_ref, _host ?? "", _edition ?? "", _force).parse();
        }

        /// <summary>
        /// Refresh the list of installed Supplemental Packs.
        /// First published in XenServer 5.6.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        [Deprecated("XenServer 7.1")]
        public static void refresh_pack_info(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_refresh_pack_info(session.opaque_ref, _host);
            else
                session.proxy.host_refresh_pack_info(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Refresh the list of installed Supplemental Packs.
        /// First published in XenServer 5.6.
        /// Deprecated since XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        [Deprecated("XenServer 7.1")]
        public static XenRef<Task> async_refresh_pack_info(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_refresh_pack_info(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_refresh_pack_info(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Set the power-on-mode, host, user and password 
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_power_on_mode">power-on-mode can be empty,iLO,wake-on-lan, DRAC or other</param>
        /// <param name="_power_on_config">Power on config</param>
        public static void set_power_on_mode(Session session, string _host, string _power_on_mode, Dictionary<string, string> _power_on_config)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_power_on_mode(session.opaque_ref, _host, _power_on_mode, _power_on_config);
            else
                session.proxy.host_set_power_on_mode(session.opaque_ref, _host ?? "", _power_on_mode ?? "", Maps.convert_to_proxy_string_string(_power_on_config)).parse();
        }

        /// <summary>
        /// Set the power-on-mode, host, user and password 
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_power_on_mode">power-on-mode can be empty,iLO,wake-on-lan, DRAC or other</param>
        /// <param name="_power_on_config">Power on config</param>
        public static XenRef<Task> async_set_power_on_mode(Session session, string _host, string _power_on_mode, Dictionary<string, string> _power_on_config)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_set_power_on_mode(session.opaque_ref, _host, _power_on_mode, _power_on_config);
          else
              return XenRef<Task>.Create(session.proxy.async_host_set_power_on_mode(session.opaque_ref, _host ?? "", _power_on_mode ?? "", Maps.convert_to_proxy_string_string(_power_on_config)).parse());
        }

        /// <summary>
        /// Set the CPU features to be used after a reboot, if the given features string is valid.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_features">The features string (32 hexadecimal digits)</param>
        public static void set_cpu_features(Session session, string _host, string _features)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_cpu_features(session.opaque_ref, _host, _features);
            else
                session.proxy.host_set_cpu_features(session.opaque_ref, _host ?? "", _features ?? "").parse();
        }

        /// <summary>
        /// Remove the feature mask, such that after a reboot all features of the CPU are enabled.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static void reset_cpu_features(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_reset_cpu_features(session.opaque_ref, _host);
            else
                session.proxy.host_reset_cpu_features(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Enable the use of a local SR for caching purposes
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_sr">The SR to use as a local cache</param>
        public static void enable_local_storage_caching(Session session, string _host, string _sr)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_enable_local_storage_caching(session.opaque_ref, _host, _sr);
            else
                session.proxy.host_enable_local_storage_caching(session.opaque_ref, _host ?? "", _sr ?? "").parse();
        }

        /// <summary>
        /// Disable the use of a local SR for caching purposes
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static void disable_local_storage_caching(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_disable_local_storage_caching(session.opaque_ref, _host);
            else
                session.proxy.host_disable_local_storage_caching(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Prepare to receive a VM, returning a token which can be passed to VM.migrate.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_network">The network through which migration traffic should be received.</param>
        /// <param name="_options">Extra configuration operations</param>
        public static Dictionary<string, string> migrate_receive(Session session, string _host, string _network, Dictionary<string, string> _options)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_migrate_receive(session.opaque_ref, _host, _network, _options);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.host_migrate_receive(session.opaque_ref, _host ?? "", _network ?? "", Maps.convert_to_proxy_string_string(_options)).parse());
        }

        /// <summary>
        /// Prepare to receive a VM, returning a token which can be passed to VM.migrate.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_network">The network through which migration traffic should be received.</param>
        /// <param name="_options">Extra configuration operations</param>
        public static XenRef<Task> async_migrate_receive(Session session, string _host, string _network, Dictionary<string, string> _options)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_migrate_receive(session.opaque_ref, _host, _network, _options);
          else
              return XenRef<Task>.Create(session.proxy.async_host_migrate_receive(session.opaque_ref, _host ?? "", _network ?? "", Maps.convert_to_proxy_string_string(_options)).parse());
        }

        /// <summary>
        /// Declare that a host is dead. This is a dangerous operation, and should only be called if the administrator is absolutely sure the host is definitely dead
        /// First published in XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static void declare_dead(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_declare_dead(session.opaque_ref, _host);
            else
                session.proxy.host_declare_dead(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Declare that a host is dead. This is a dangerous operation, and should only be called if the administrator is absolutely sure the host is definitely dead
        /// First published in XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_declare_dead(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_declare_dead(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_declare_dead(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Enable console output to the physical display device next time this host boots
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static host_display enable_display(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_enable_display(session.opaque_ref, _host);
            else
                return (host_display)Helper.EnumParseDefault(typeof(host_display), (string)session.proxy.host_enable_display(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Enable console output to the physical display device next time this host boots
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_enable_display(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_enable_display(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_enable_display(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Disable console output to the physical display device next time this host boots
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static host_display disable_display(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_disable_display(session.opaque_ref, _host);
            else
                return (host_display)Helper.EnumParseDefault(typeof(host_display), (string)session.proxy.host_disable_display(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Disable console output to the physical display device next time this host boots
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        public static XenRef<Task> async_disable_display(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_disable_display(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_host_disable_display(session.opaque_ref, _host ?? "").parse());
        }

        /// <summary>
        /// Enable/disable SSLv3 for interoperability with older server versions. When this is set to a different value, the host immediately restarts its SSL/TLS listening service; typically this takes less than a second but existing connections to it will be broken. API login sessions will remain valid.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">True to allow SSLv3 and ciphersuites as used in old XenServer versions</param>
        public static void set_ssl_legacy(Session session, string _host, bool _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_ssl_legacy(session.opaque_ref, _host, _value);
            else
                session.proxy.host_set_ssl_legacy(session.opaque_ref, _host ?? "", _value).parse();
        }

        /// <summary>
        /// Enable/disable SSLv3 for interoperability with older server versions. When this is set to a different value, the host immediately restarts its SSL/TLS listening service; typically this takes less than a second but existing connections to it will be broken. API login sessions will remain valid.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">True to allow SSLv3 and ciphersuites as used in old XenServer versions</param>
        public static XenRef<Task> async_set_ssl_legacy(Session session, string _host, bool _value)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_set_ssl_legacy(session.opaque_ref, _host, _value);
          else
              return XenRef<Task>.Create(session.proxy.async_host_set_ssl_legacy(session.opaque_ref, _host ?? "", _value).parse());
        }

        /// <summary>
        /// Sets the initiator IQN for the host
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The value to which the IQN should be set</param>
        public static void set_iscsi_iqn(Session session, string _host, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_iscsi_iqn(session.opaque_ref, _host, _value);
            else
                session.proxy.host_set_iscsi_iqn(session.opaque_ref, _host ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Sets the initiator IQN for the host
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The value to which the IQN should be set</param>
        public static XenRef<Task> async_set_iscsi_iqn(Session session, string _host, string _value)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_set_iscsi_iqn(session.opaque_ref, _host, _value);
          else
              return XenRef<Task>.Create(session.proxy.async_host_set_iscsi_iqn(session.opaque_ref, _host ?? "", _value ?? "").parse());
        }

        /// <summary>
        /// Specifies whether multipathing is enabled
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">Whether multipathing should be enabled</param>
        public static void set_multipathing(Session session, string _host, bool _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_multipathing(session.opaque_ref, _host, _value);
            else
                session.proxy.host_set_multipathing(session.opaque_ref, _host ?? "", _value).parse();
        }

        /// <summary>
        /// Specifies whether multipathing is enabled
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">Whether multipathing should be enabled</param>
        public static XenRef<Task> async_set_multipathing(Session session, string _host, bool _value)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_set_multipathing(session.opaque_ref, _host, _value);
          else
              return XenRef<Task>.Create(session.proxy.async_host_set_multipathing(session.opaque_ref, _host ?? "", _value).parse());
        }

        /// <summary>
        /// Sets the UEFI certificates on a host
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The certificates to apply to a host</param>
        public static void set_uefi_certificates(Session session, string _host, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.host_set_uefi_certificates(session.opaque_ref, _host, _value);
            else
                session.proxy.host_set_uefi_certificates(session.opaque_ref, _host ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Sets the UEFI certificates on a host
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The opaque_ref of the given host</param>
        /// <param name="_value">The certificates to apply to a host</param>
        public static XenRef<Task> async_set_uefi_certificates(Session session, string _host, string _value)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_host_set_uefi_certificates(session.opaque_ref, _host, _value);
          else
              return XenRef<Task>.Create(session.proxy.async_host_set_uefi_certificates(session.opaque_ref, _host ?? "", _value ?? "").parse());
        }

        /// <summary>
        /// Return a list of all the hosts known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Host>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_all(session.opaque_ref);
            else
                return XenRef<Host>.Create(session.proxy.host_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the host Records at once, in a single XML RPC call
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Host>, Host> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.host_get_all_records(session.opaque_ref);
            else
                return XenRef<Host>.Create<Proxy_Host>(session.proxy.host_get_all_records(session.opaque_ref).parse());
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
                    NotifyPropertyChanged("multipathing");
                }
            }
        }
        private bool _multipathing = false;

        /// <summary>
        /// The UEFI certificates allowing Secure Boot
        /// First published in Unreleased.
        /// </summary>
        public virtual string uefi_certificates
        {
            get { return _uefi_certificates; }
            set
            {
                if (!Helper.AreEqual(value, _uefi_certificates))
                {
                    _uefi_certificates = value;
                    Changed = true;
                    NotifyPropertyChanged("uefi_certificates");
                }
            }
        }
        private string _uefi_certificates = "";
    }
}
