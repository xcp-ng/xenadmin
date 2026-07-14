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
    /// A virtual machine (or 'guest').
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class VM : XenObject<VM>
    {
        #region Constructors

        public VM()
        {
        }

        public VM(string uuid,
            List<vm_operations> allowed_operations,
            Dictionary<string, vm_operations> current_operations,
            string name_label,
            string name_description,
            vm_power_state power_state,
            long user_version,
            bool is_a_template,
            bool is_default_template,
            XenRef<VDI> suspend_VDI,
            XenRef<Host> resident_on,
            XenRef<Host> scheduled_to_be_resident_on,
            XenRef<Host> affinity,
            long memory_overhead,
            long memory_target,
            long memory_static_max,
            long memory_dynamic_max,
            long memory_dynamic_min,
            long memory_static_min,
            Dictionary<string, string> VCPUs_params,
            long VCPUs_max,
            long VCPUs_at_startup,
            on_softreboot_behavior actions_after_softreboot,
            on_normal_exit actions_after_shutdown,
            on_normal_exit actions_after_reboot,
            on_crash_behaviour actions_after_crash,
            List<XenRef<Console>> consoles,
            List<XenRef<VIF>> VIFs,
            List<XenRef<VBD>> VBDs,
            List<XenRef<VUSB>> VUSBs,
            List<XenRef<Crashdump>> crash_dumps,
            List<XenRef<VTPM>> VTPMs,
            string PV_bootloader,
            string PV_kernel,
            string PV_ramdisk,
            string PV_args,
            string PV_bootloader_args,
            string PV_legacy_args,
            string HVM_boot_policy,
            Dictionary<string, string> HVM_boot_params,
            double HVM_shadow_multiplier,
            Dictionary<string, string> platform,
            string PCI_bus,
            Dictionary<string, string> other_config,
            long domid,
            string domarch,
            Dictionary<string, string> last_boot_CPU_flags,
            bool is_control_domain,
            XenRef<VM_metrics> metrics,
            XenRef<VM_guest_metrics> guest_metrics,
            string last_booted_record,
            string recommendations,
            Dictionary<string, string> xenstore_data,
            bool ha_always_run,
            string ha_restart_priority,
            bool is_a_snapshot,
            XenRef<VM> snapshot_of,
            List<XenRef<VM>> snapshots,
            DateTime snapshot_time,
            string transportable_snapshot_id,
            Dictionary<string, XenRef<Blob>> blobs,
            string[] tags,
            Dictionary<vm_operations, string> blocked_operations,
            Dictionary<string, string> snapshot_info,
            string snapshot_metadata,
            XenRef<VM> parent,
            List<XenRef<VM>> children,
            Dictionary<string, string> bios_strings,
            XenRef<VMPP> protection_policy,
            bool is_snapshot_from_vmpp,
            XenRef<VMSS> snapshot_schedule,
            bool is_vmss_snapshot,
            XenRef<VM_appliance> appliance,
            long start_delay,
            long shutdown_delay,
            long order,
            List<XenRef<VGPU>> VGPUs,
            List<XenRef<PCI>> attached_PCIs,
            XenRef<SR> suspend_SR,
            long version,
            string generation_id,
            long hardware_platform_version,
            bool has_vendor_device,
            bool requires_reboot,
            string reference_label,
            domain_type domain_type,
            Dictionary<string, string> NVRAM,
            List<update_guidances> pending_guidances,
            List<update_guidances> pending_guidances_recommended,
            List<update_guidances> pending_guidances_full,
            List<XenRef<VM_group>> groups,
            vm_secureboot_certificates_state secureboot_certificates_state)
        {
            this.uuid = uuid;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.name_label = name_label;
            this.name_description = name_description;
            this.power_state = power_state;
            this.user_version = user_version;
            this.is_a_template = is_a_template;
            this.is_default_template = is_default_template;
            this.suspend_VDI = suspend_VDI;
            this.resident_on = resident_on;
            this.scheduled_to_be_resident_on = scheduled_to_be_resident_on;
            this.affinity = affinity;
            this.memory_overhead = memory_overhead;
            this.memory_target = memory_target;
            this.memory_static_max = memory_static_max;
            this.memory_dynamic_max = memory_dynamic_max;
            this.memory_dynamic_min = memory_dynamic_min;
            this.memory_static_min = memory_static_min;
            this.VCPUs_params = VCPUs_params;
            this.VCPUs_max = VCPUs_max;
            this.VCPUs_at_startup = VCPUs_at_startup;
            this.actions_after_softreboot = actions_after_softreboot;
            this.actions_after_shutdown = actions_after_shutdown;
            this.actions_after_reboot = actions_after_reboot;
            this.actions_after_crash = actions_after_crash;
            this.consoles = consoles;
            this.VIFs = VIFs;
            this.VBDs = VBDs;
            this.VUSBs = VUSBs;
            this.crash_dumps = crash_dumps;
            this.VTPMs = VTPMs;
            this.PV_bootloader = PV_bootloader;
            this.PV_kernel = PV_kernel;
            this.PV_ramdisk = PV_ramdisk;
            this.PV_args = PV_args;
            this.PV_bootloader_args = PV_bootloader_args;
            this.PV_legacy_args = PV_legacy_args;
            this.HVM_boot_policy = HVM_boot_policy;
            this.HVM_boot_params = HVM_boot_params;
            this.HVM_shadow_multiplier = HVM_shadow_multiplier;
            this.platform = platform;
            this.PCI_bus = PCI_bus;
            this.other_config = other_config;
            this.domid = domid;
            this.domarch = domarch;
            this.last_boot_CPU_flags = last_boot_CPU_flags;
            this.is_control_domain = is_control_domain;
            this.metrics = metrics;
            this.guest_metrics = guest_metrics;
            this.last_booted_record = last_booted_record;
            this.recommendations = recommendations;
            this.xenstore_data = xenstore_data;
            this.ha_always_run = ha_always_run;
            this.ha_restart_priority = ha_restart_priority;
            this.is_a_snapshot = is_a_snapshot;
            this.snapshot_of = snapshot_of;
            this.snapshots = snapshots;
            this.snapshot_time = snapshot_time;
            this.transportable_snapshot_id = transportable_snapshot_id;
            this.blobs = blobs;
            this.tags = tags;
            this.blocked_operations = blocked_operations;
            this.snapshot_info = snapshot_info;
            this.snapshot_metadata = snapshot_metadata;
            this.parent = parent;
            this.children = children;
            this.bios_strings = bios_strings;
            this.protection_policy = protection_policy;
            this.is_snapshot_from_vmpp = is_snapshot_from_vmpp;
            this.snapshot_schedule = snapshot_schedule;
            this.is_vmss_snapshot = is_vmss_snapshot;
            this.appliance = appliance;
            this.start_delay = start_delay;
            this.shutdown_delay = shutdown_delay;
            this.order = order;
            this.VGPUs = VGPUs;
            this.attached_PCIs = attached_PCIs;
            this.suspend_SR = suspend_SR;
            this.version = version;
            this.generation_id = generation_id;
            this.hardware_platform_version = hardware_platform_version;
            this.has_vendor_device = has_vendor_device;
            this.requires_reboot = requires_reboot;
            this.reference_label = reference_label;
            this.domain_type = domain_type;
            this.NVRAM = NVRAM;
            this.pending_guidances = pending_guidances;
            this.pending_guidances_recommended = pending_guidances_recommended;
            this.pending_guidances_full = pending_guidances_full;
            this.groups = groups;
            this.secureboot_certificates_state = secureboot_certificates_state;
        }

        /// <summary>
        /// Creates a new VM from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VM(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VM.
        /// </summary>
        public override void UpdateFrom(VM record)
        {
            uuid = record.uuid;
            allowed_operations = record.allowed_operations;
            current_operations = record.current_operations;
            name_label = record.name_label;
            name_description = record.name_description;
            power_state = record.power_state;
            user_version = record.user_version;
            is_a_template = record.is_a_template;
            is_default_template = record.is_default_template;
            suspend_VDI = record.suspend_VDI;
            resident_on = record.resident_on;
            scheduled_to_be_resident_on = record.scheduled_to_be_resident_on;
            affinity = record.affinity;
            memory_overhead = record.memory_overhead;
            memory_target = record.memory_target;
            memory_static_max = record.memory_static_max;
            memory_dynamic_max = record.memory_dynamic_max;
            memory_dynamic_min = record.memory_dynamic_min;
            memory_static_min = record.memory_static_min;
            VCPUs_params = record.VCPUs_params;
            VCPUs_max = record.VCPUs_max;
            VCPUs_at_startup = record.VCPUs_at_startup;
            actions_after_softreboot = record.actions_after_softreboot;
            actions_after_shutdown = record.actions_after_shutdown;
            actions_after_reboot = record.actions_after_reboot;
            actions_after_crash = record.actions_after_crash;
            consoles = record.consoles;
            VIFs = record.VIFs;
            VBDs = record.VBDs;
            VUSBs = record.VUSBs;
            crash_dumps = record.crash_dumps;
            VTPMs = record.VTPMs;
            PV_bootloader = record.PV_bootloader;
            PV_kernel = record.PV_kernel;
            PV_ramdisk = record.PV_ramdisk;
            PV_args = record.PV_args;
            PV_bootloader_args = record.PV_bootloader_args;
            PV_legacy_args = record.PV_legacy_args;
            HVM_boot_policy = record.HVM_boot_policy;
            HVM_boot_params = record.HVM_boot_params;
            HVM_shadow_multiplier = record.HVM_shadow_multiplier;
            platform = record.platform;
            PCI_bus = record.PCI_bus;
            other_config = record.other_config;
            domid = record.domid;
            domarch = record.domarch;
            last_boot_CPU_flags = record.last_boot_CPU_flags;
            is_control_domain = record.is_control_domain;
            metrics = record.metrics;
            guest_metrics = record.guest_metrics;
            last_booted_record = record.last_booted_record;
            recommendations = record.recommendations;
            xenstore_data = record.xenstore_data;
            ha_always_run = record.ha_always_run;
            ha_restart_priority = record.ha_restart_priority;
            is_a_snapshot = record.is_a_snapshot;
            snapshot_of = record.snapshot_of;
            snapshots = record.snapshots;
            snapshot_time = record.snapshot_time;
            transportable_snapshot_id = record.transportable_snapshot_id;
            blobs = record.blobs;
            tags = record.tags;
            blocked_operations = record.blocked_operations;
            snapshot_info = record.snapshot_info;
            snapshot_metadata = record.snapshot_metadata;
            parent = record.parent;
            children = record.children;
            bios_strings = record.bios_strings;
            protection_policy = record.protection_policy;
            is_snapshot_from_vmpp = record.is_snapshot_from_vmpp;
            snapshot_schedule = record.snapshot_schedule;
            is_vmss_snapshot = record.is_vmss_snapshot;
            appliance = record.appliance;
            start_delay = record.start_delay;
            shutdown_delay = record.shutdown_delay;
            order = record.order;
            VGPUs = record.VGPUs;
            attached_PCIs = record.attached_PCIs;
            suspend_SR = record.suspend_SR;
            version = record.version;
            generation_id = record.generation_id;
            hardware_platform_version = record.hardware_platform_version;
            has_vendor_device = record.has_vendor_device;
            requires_reboot = record.requires_reboot;
            reference_label = record.reference_label;
            domain_type = record.domain_type;
            NVRAM = record.NVRAM;
            pending_guidances = record.pending_guidances;
            pending_guidances_recommended = record.pending_guidances_recommended;
            pending_guidances_full = record.pending_guidances_full;
            groups = record.groups;
            secureboot_certificates_state = record.secureboot_certificates_state;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this VM
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("allowed_operations"))
                allowed_operations = Helper.StringArrayToEnumList<vm_operations>(Marshalling.ParseStringArray(table, "allowed_operations"));
            if (table.ContainsKey("current_operations"))
                current_operations = Maps.ToDictionary_string_vm_operations(Marshalling.ParseHashTable(table, "current_operations"));
            if (table.ContainsKey("name_label"))
                name_label = Marshalling.ParseString(table, "name_label");
            if (table.ContainsKey("name_description"))
                name_description = Marshalling.ParseString(table, "name_description");
            if (table.ContainsKey("power_state"))
                power_state = (vm_power_state)Helper.EnumParseDefault(typeof(vm_power_state), Marshalling.ParseString(table, "power_state"));
            if (table.ContainsKey("user_version"))
                user_version = Marshalling.ParseLong(table, "user_version");
            if (table.ContainsKey("is_a_template"))
                is_a_template = Marshalling.ParseBool(table, "is_a_template");
            if (table.ContainsKey("is_default_template"))
                is_default_template = Marshalling.ParseBool(table, "is_default_template");
            if (table.ContainsKey("suspend_VDI"))
                suspend_VDI = Marshalling.ParseRef<VDI>(table, "suspend_VDI");
            if (table.ContainsKey("resident_on"))
                resident_on = Marshalling.ParseRef<Host>(table, "resident_on");
            if (table.ContainsKey("scheduled_to_be_resident_on"))
                scheduled_to_be_resident_on = Marshalling.ParseRef<Host>(table, "scheduled_to_be_resident_on");
            if (table.ContainsKey("affinity"))
                affinity = Marshalling.ParseRef<Host>(table, "affinity");
            if (table.ContainsKey("memory_overhead"))
                memory_overhead = Marshalling.ParseLong(table, "memory_overhead");
            if (table.ContainsKey("memory_target"))
                memory_target = Marshalling.ParseLong(table, "memory_target");
            if (table.ContainsKey("memory_static_max"))
                memory_static_max = Marshalling.ParseLong(table, "memory_static_max");
            if (table.ContainsKey("memory_dynamic_max"))
                memory_dynamic_max = Marshalling.ParseLong(table, "memory_dynamic_max");
            if (table.ContainsKey("memory_dynamic_min"))
                memory_dynamic_min = Marshalling.ParseLong(table, "memory_dynamic_min");
            if (table.ContainsKey("memory_static_min"))
                memory_static_min = Marshalling.ParseLong(table, "memory_static_min");
            if (table.ContainsKey("VCPUs_params"))
                VCPUs_params = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "VCPUs_params"));
            if (table.ContainsKey("VCPUs_max"))
                VCPUs_max = Marshalling.ParseLong(table, "VCPUs_max");
            if (table.ContainsKey("VCPUs_at_startup"))
                VCPUs_at_startup = Marshalling.ParseLong(table, "VCPUs_at_startup");
            if (table.ContainsKey("actions_after_softreboot"))
                actions_after_softreboot = (on_softreboot_behavior)Helper.EnumParseDefault(typeof(on_softreboot_behavior), Marshalling.ParseString(table, "actions_after_softreboot"));
            if (table.ContainsKey("actions_after_shutdown"))
                actions_after_shutdown = (on_normal_exit)Helper.EnumParseDefault(typeof(on_normal_exit), Marshalling.ParseString(table, "actions_after_shutdown"));
            if (table.ContainsKey("actions_after_reboot"))
                actions_after_reboot = (on_normal_exit)Helper.EnumParseDefault(typeof(on_normal_exit), Marshalling.ParseString(table, "actions_after_reboot"));
            if (table.ContainsKey("actions_after_crash"))
                actions_after_crash = (on_crash_behaviour)Helper.EnumParseDefault(typeof(on_crash_behaviour), Marshalling.ParseString(table, "actions_after_crash"));
            if (table.ContainsKey("consoles"))
                consoles = Marshalling.ParseSetRef<Console>(table, "consoles");
            if (table.ContainsKey("VIFs"))
                VIFs = Marshalling.ParseSetRef<VIF>(table, "VIFs");
            if (table.ContainsKey("VBDs"))
                VBDs = Marshalling.ParseSetRef<VBD>(table, "VBDs");
            if (table.ContainsKey("VUSBs"))
                VUSBs = Marshalling.ParseSetRef<VUSB>(table, "VUSBs");
            if (table.ContainsKey("crash_dumps"))
                crash_dumps = Marshalling.ParseSetRef<Crashdump>(table, "crash_dumps");
            if (table.ContainsKey("VTPMs"))
                VTPMs = Marshalling.ParseSetRef<VTPM>(table, "VTPMs");
            if (table.ContainsKey("PV_bootloader"))
                PV_bootloader = Marshalling.ParseString(table, "PV_bootloader");
            if (table.ContainsKey("PV_kernel"))
                PV_kernel = Marshalling.ParseString(table, "PV_kernel");
            if (table.ContainsKey("PV_ramdisk"))
                PV_ramdisk = Marshalling.ParseString(table, "PV_ramdisk");
            if (table.ContainsKey("PV_args"))
                PV_args = Marshalling.ParseString(table, "PV_args");
            if (table.ContainsKey("PV_bootloader_args"))
                PV_bootloader_args = Marshalling.ParseString(table, "PV_bootloader_args");
            if (table.ContainsKey("PV_legacy_args"))
                PV_legacy_args = Marshalling.ParseString(table, "PV_legacy_args");
            if (table.ContainsKey("HVM_boot_policy"))
                HVM_boot_policy = Marshalling.ParseString(table, "HVM_boot_policy");
            if (table.ContainsKey("HVM_boot_params"))
                HVM_boot_params = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "HVM_boot_params"));
            if (table.ContainsKey("HVM_shadow_multiplier"))
                HVM_shadow_multiplier = Marshalling.ParseDouble(table, "HVM_shadow_multiplier");
            if (table.ContainsKey("platform"))
                platform = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "platform"));
            if (table.ContainsKey("PCI_bus"))
                PCI_bus = Marshalling.ParseString(table, "PCI_bus");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("domid"))
                domid = Marshalling.ParseLong(table, "domid");
            if (table.ContainsKey("domarch"))
                domarch = Marshalling.ParseString(table, "domarch");
            if (table.ContainsKey("last_boot_CPU_flags"))
                last_boot_CPU_flags = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "last_boot_CPU_flags"));
            if (table.ContainsKey("is_control_domain"))
                is_control_domain = Marshalling.ParseBool(table, "is_control_domain");
            if (table.ContainsKey("metrics"))
                metrics = Marshalling.ParseRef<VM_metrics>(table, "metrics");
            if (table.ContainsKey("guest_metrics"))
                guest_metrics = Marshalling.ParseRef<VM_guest_metrics>(table, "guest_metrics");
            if (table.ContainsKey("last_booted_record"))
                last_booted_record = Marshalling.ParseString(table, "last_booted_record");
            if (table.ContainsKey("recommendations"))
                recommendations = Marshalling.ParseString(table, "recommendations");
            if (table.ContainsKey("xenstore_data"))
                xenstore_data = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "xenstore_data"));
            if (table.ContainsKey("ha_always_run"))
                ha_always_run = Marshalling.ParseBool(table, "ha_always_run");
            if (table.ContainsKey("ha_restart_priority"))
                ha_restart_priority = Marshalling.ParseString(table, "ha_restart_priority");
            if (table.ContainsKey("is_a_snapshot"))
                is_a_snapshot = Marshalling.ParseBool(table, "is_a_snapshot");
            if (table.ContainsKey("snapshot_of"))
                snapshot_of = Marshalling.ParseRef<VM>(table, "snapshot_of");
            if (table.ContainsKey("snapshots"))
                snapshots = Marshalling.ParseSetRef<VM>(table, "snapshots");
            if (table.ContainsKey("snapshot_time"))
                snapshot_time = Marshalling.ParseDateTime(table, "snapshot_time");
            if (table.ContainsKey("transportable_snapshot_id"))
                transportable_snapshot_id = Marshalling.ParseString(table, "transportable_snapshot_id");
            if (table.ContainsKey("blobs"))
                blobs = Maps.ToDictionary_string_XenRefBlob(Marshalling.ParseHashTable(table, "blobs"));
            if (table.ContainsKey("tags"))
                tags = Marshalling.ParseStringArray(table, "tags");
            if (table.ContainsKey("blocked_operations"))
                blocked_operations = Maps.ToDictionary_vm_operations_string(Marshalling.ParseHashTable(table, "blocked_operations"));
            if (table.ContainsKey("snapshot_info"))
                snapshot_info = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "snapshot_info"));
            if (table.ContainsKey("snapshot_metadata"))
                snapshot_metadata = Marshalling.ParseString(table, "snapshot_metadata");
            if (table.ContainsKey("parent"))
                parent = Marshalling.ParseRef<VM>(table, "parent");
            if (table.ContainsKey("children"))
                children = Marshalling.ParseSetRef<VM>(table, "children");
            if (table.ContainsKey("bios_strings"))
                bios_strings = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "bios_strings"));
            if (table.ContainsKey("protection_policy"))
                protection_policy = Marshalling.ParseRef<VMPP>(table, "protection_policy");
            if (table.ContainsKey("is_snapshot_from_vmpp"))
                is_snapshot_from_vmpp = Marshalling.ParseBool(table, "is_snapshot_from_vmpp");
            if (table.ContainsKey("snapshot_schedule"))
                snapshot_schedule = Marshalling.ParseRef<VMSS>(table, "snapshot_schedule");
            if (table.ContainsKey("is_vmss_snapshot"))
                is_vmss_snapshot = Marshalling.ParseBool(table, "is_vmss_snapshot");
            if (table.ContainsKey("appliance"))
                appliance = Marshalling.ParseRef<VM_appliance>(table, "appliance");
            if (table.ContainsKey("start_delay"))
                start_delay = Marshalling.ParseLong(table, "start_delay");
            if (table.ContainsKey("shutdown_delay"))
                shutdown_delay = Marshalling.ParseLong(table, "shutdown_delay");
            if (table.ContainsKey("order"))
                order = Marshalling.ParseLong(table, "order");
            if (table.ContainsKey("VGPUs"))
                VGPUs = Marshalling.ParseSetRef<VGPU>(table, "VGPUs");
            if (table.ContainsKey("attached_PCIs"))
                attached_PCIs = Marshalling.ParseSetRef<PCI>(table, "attached_PCIs");
            if (table.ContainsKey("suspend_SR"))
                suspend_SR = Marshalling.ParseRef<SR>(table, "suspend_SR");
            if (table.ContainsKey("version"))
                version = Marshalling.ParseLong(table, "version");
            if (table.ContainsKey("generation_id"))
                generation_id = Marshalling.ParseString(table, "generation_id");
            if (table.ContainsKey("hardware_platform_version"))
                hardware_platform_version = Marshalling.ParseLong(table, "hardware_platform_version");
            if (table.ContainsKey("has_vendor_device"))
                has_vendor_device = Marshalling.ParseBool(table, "has_vendor_device");
            if (table.ContainsKey("requires_reboot"))
                requires_reboot = Marshalling.ParseBool(table, "requires_reboot");
            if (table.ContainsKey("reference_label"))
                reference_label = Marshalling.ParseString(table, "reference_label");
            if (table.ContainsKey("domain_type"))
                domain_type = (domain_type)Helper.EnumParseDefault(typeof(domain_type), Marshalling.ParseString(table, "domain_type"));
            if (table.ContainsKey("NVRAM"))
                NVRAM = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "NVRAM"));
            if (table.ContainsKey("pending_guidances"))
                pending_guidances = Helper.StringArrayToEnumList<update_guidances>(Marshalling.ParseStringArray(table, "pending_guidances"));
            if (table.ContainsKey("pending_guidances_recommended"))
                pending_guidances_recommended = Helper.StringArrayToEnumList<update_guidances>(Marshalling.ParseStringArray(table, "pending_guidances_recommended"));
            if (table.ContainsKey("pending_guidances_full"))
                pending_guidances_full = Helper.StringArrayToEnumList<update_guidances>(Marshalling.ParseStringArray(table, "pending_guidances_full"));
            if (table.ContainsKey("groups"))
                groups = Marshalling.ParseSetRef<VM_group>(table, "groups");
            if (table.ContainsKey("secureboot_certificates_state"))
                secureboot_certificates_state = (vm_secureboot_certificates_state)Helper.EnumParseDefault(typeof(vm_secureboot_certificates_state), Marshalling.ParseString(table, "secureboot_certificates_state"));
        }

        public bool DeepEquals(VM other, bool ignoreCurrentOperations)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (!ignoreCurrentOperations && !Helper.AreEqual2(current_operations, other.current_operations))
                return false;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(_name_label, other._name_label) &&
                Helper.AreEqual2(_name_description, other._name_description) &&
                Helper.AreEqual2(_power_state, other._power_state) &&
                Helper.AreEqual2(_user_version, other._user_version) &&
                Helper.AreEqual2(_is_a_template, other._is_a_template) &&
                Helper.AreEqual2(_is_default_template, other._is_default_template) &&
                Helper.AreEqual2(_suspend_VDI, other._suspend_VDI) &&
                Helper.AreEqual2(_resident_on, other._resident_on) &&
                Helper.AreEqual2(_scheduled_to_be_resident_on, other._scheduled_to_be_resident_on) &&
                Helper.AreEqual2(_affinity, other._affinity) &&
                Helper.AreEqual2(_memory_overhead, other._memory_overhead) &&
                Helper.AreEqual2(_memory_target, other._memory_target) &&
                Helper.AreEqual2(_memory_static_max, other._memory_static_max) &&
                Helper.AreEqual2(_memory_dynamic_max, other._memory_dynamic_max) &&
                Helper.AreEqual2(_memory_dynamic_min, other._memory_dynamic_min) &&
                Helper.AreEqual2(_memory_static_min, other._memory_static_min) &&
                Helper.AreEqual2(_VCPUs_params, other._VCPUs_params) &&
                Helper.AreEqual2(_VCPUs_max, other._VCPUs_max) &&
                Helper.AreEqual2(_VCPUs_at_startup, other._VCPUs_at_startup) &&
                Helper.AreEqual2(_actions_after_softreboot, other._actions_after_softreboot) &&
                Helper.AreEqual2(_actions_after_shutdown, other._actions_after_shutdown) &&
                Helper.AreEqual2(_actions_after_reboot, other._actions_after_reboot) &&
                Helper.AreEqual2(_actions_after_crash, other._actions_after_crash) &&
                Helper.AreEqual2(_consoles, other._consoles) &&
                Helper.AreEqual2(_VIFs, other._VIFs) &&
                Helper.AreEqual2(_VBDs, other._VBDs) &&
                Helper.AreEqual2(_VUSBs, other._VUSBs) &&
                Helper.AreEqual2(_crash_dumps, other._crash_dumps) &&
                Helper.AreEqual2(_VTPMs, other._VTPMs) &&
                Helper.AreEqual2(_PV_bootloader, other._PV_bootloader) &&
                Helper.AreEqual2(_PV_kernel, other._PV_kernel) &&
                Helper.AreEqual2(_PV_ramdisk, other._PV_ramdisk) &&
                Helper.AreEqual2(_PV_args, other._PV_args) &&
                Helper.AreEqual2(_PV_bootloader_args, other._PV_bootloader_args) &&
                Helper.AreEqual2(_PV_legacy_args, other._PV_legacy_args) &&
                Helper.AreEqual2(_HVM_boot_policy, other._HVM_boot_policy) &&
                Helper.AreEqual2(_HVM_boot_params, other._HVM_boot_params) &&
                Helper.AreEqual2(_HVM_shadow_multiplier, other._HVM_shadow_multiplier) &&
                Helper.AreEqual2(_platform, other._platform) &&
                Helper.AreEqual2(_PCI_bus, other._PCI_bus) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_domid, other._domid) &&
                Helper.AreEqual2(_domarch, other._domarch) &&
                Helper.AreEqual2(_last_boot_CPU_flags, other._last_boot_CPU_flags) &&
                Helper.AreEqual2(_is_control_domain, other._is_control_domain) &&
                Helper.AreEqual2(_metrics, other._metrics) &&
                Helper.AreEqual2(_guest_metrics, other._guest_metrics) &&
                Helper.AreEqual2(_last_booted_record, other._last_booted_record) &&
                Helper.AreEqual2(_recommendations, other._recommendations) &&
                Helper.AreEqual2(_xenstore_data, other._xenstore_data) &&
                Helper.AreEqual2(_ha_always_run, other._ha_always_run) &&
                Helper.AreEqual2(_ha_restart_priority, other._ha_restart_priority) &&
                Helper.AreEqual2(_is_a_snapshot, other._is_a_snapshot) &&
                Helper.AreEqual2(_snapshot_of, other._snapshot_of) &&
                Helper.AreEqual2(_snapshots, other._snapshots) &&
                Helper.AreEqual2(_snapshot_time, other._snapshot_time) &&
                Helper.AreEqual2(_transportable_snapshot_id, other._transportable_snapshot_id) &&
                Helper.AreEqual2(_blobs, other._blobs) &&
                Helper.AreEqual2(_tags, other._tags) &&
                Helper.AreEqual2(_blocked_operations, other._blocked_operations) &&
                Helper.AreEqual2(_snapshot_info, other._snapshot_info) &&
                Helper.AreEqual2(_snapshot_metadata, other._snapshot_metadata) &&
                Helper.AreEqual2(_parent, other._parent) &&
                Helper.AreEqual2(_children, other._children) &&
                Helper.AreEqual2(_bios_strings, other._bios_strings) &&
                Helper.AreEqual2(_protection_policy, other._protection_policy) &&
                Helper.AreEqual2(_is_snapshot_from_vmpp, other._is_snapshot_from_vmpp) &&
                Helper.AreEqual2(_snapshot_schedule, other._snapshot_schedule) &&
                Helper.AreEqual2(_is_vmss_snapshot, other._is_vmss_snapshot) &&
                Helper.AreEqual2(_appliance, other._appliance) &&
                Helper.AreEqual2(_start_delay, other._start_delay) &&
                Helper.AreEqual2(_shutdown_delay, other._shutdown_delay) &&
                Helper.AreEqual2(_order, other._order) &&
                Helper.AreEqual2(_VGPUs, other._VGPUs) &&
                Helper.AreEqual2(_attached_PCIs, other._attached_PCIs) &&
                Helper.AreEqual2(_suspend_SR, other._suspend_SR) &&
                Helper.AreEqual2(_version, other._version) &&
                Helper.AreEqual2(_generation_id, other._generation_id) &&
                Helper.AreEqual2(_hardware_platform_version, other._hardware_platform_version) &&
                Helper.AreEqual2(_has_vendor_device, other._has_vendor_device) &&
                Helper.AreEqual2(_requires_reboot, other._requires_reboot) &&
                Helper.AreEqual2(_reference_label, other._reference_label) &&
                Helper.AreEqual2(_domain_type, other._domain_type) &&
                Helper.AreEqual2(_NVRAM, other._NVRAM) &&
                Helper.AreEqual2(_pending_guidances, other._pending_guidances) &&
                Helper.AreEqual2(_pending_guidances_recommended, other._pending_guidances_recommended) &&
                Helper.AreEqual2(_pending_guidances_full, other._pending_guidances_full) &&
                Helper.AreEqual2(_groups, other._groups) &&
                Helper.AreEqual2(_secureboot_certificates_state, other._secureboot_certificates_state);
        }


        /// <summary>
        /// Get a record containing the current state of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static VM get_record(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_record(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get a reference to the VM instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VM> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.vm_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// NOT RECOMMENDED! VM.clone or VM.copy (or VM.import) is a better choice in almost all situations. The standard way to obtain a new VM is to call VM.clone on a template VM, then call VM.provision on the new clone. Caution: if VM.create is used and then the new VM is attached to a virtual disc that has an operating system already installed, then there is no guarantee that the operating system will boot and run. Any software that calls VM.create on a future version of this API may fail or give unexpected results. For example this could happen if an additional parameter were added to VM.create. VM.create is intended only for use in the automatic creation of the system VM templates. It creates a new VM instance, and returns its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<VM> create(Session session, VM _record)
        {
            return session.JsonRpcClient.vm_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// NOT RECOMMENDED! VM.clone or VM.copy (or VM.import) is a better choice in almost all situations. The standard way to obtain a new VM is to call VM.clone on a template VM, then call VM.provision on the new clone. Caution: if VM.create is used and then the new VM is attached to a virtual disc that has an operating system already installed, then there is no guarantee that the operating system will boot and run. Any software that calls VM.create on a future version of this API may fail or give unexpected results. For example this could happen if an additional parameter were added to VM.create. VM.create is intended only for use in the automatic creation of the system VM templates. It creates a new VM instance, and returns its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_create(Session session, VM _record)
        {
          return session.JsonRpcClient.async_vm_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Destroy the specified VM.  The VM is completely removed from the system.  This function can only be called when the VM is in the Halted State.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void destroy(Session session, string _vm)
        {
            session.JsonRpcClient.vm_destroy(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Destroy the specified VM.  The VM is completely removed from the system.  This function can only be called when the VM is in the Halted State.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_destroy(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get all the VM instances with the given label.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VM>> get_by_name_label(Session session, string _label)
        {
            return session.JsonRpcClient.vm_get_by_name_label(session.opaque_ref, _label);
        }

        /// <summary>
        /// Get the uuid field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_uuid(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the allowed_operations field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<vm_operations> get_allowed_operations(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_allowed_operations(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the current_operations field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, vm_operations> get_current_operations(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_current_operations(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the name/label field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_label(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_name_label(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the name/description field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_description(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_name_description(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the power_state field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static vm_power_state get_power_state(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_power_state(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the user_version field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_user_version(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_user_version(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the is_a_template field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_is_a_template(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_is_a_template(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the is_default_template field of the given VM.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_is_default_template(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_is_default_template(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the suspend_VDI field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VDI> get_suspend_VDI(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_suspend_vdi(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the resident_on field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host> get_resident_on(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_resident_on(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the scheduled_to_be_resident_on field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host> get_scheduled_to_be_resident_on(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_scheduled_to_be_resident_on(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the affinity field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host> get_affinity(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_affinity(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the memory/overhead field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_memory_overhead(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_memory_overhead(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the memory/target field of the given VM.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        [Deprecated("XenServer 5.6")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_memory_target(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_memory_target(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the memory/static_max field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_memory_static_max(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_memory_static_max(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the memory/dynamic_max field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_memory_dynamic_max(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_memory_dynamic_max(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the memory/dynamic_min field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_memory_dynamic_min(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_memory_dynamic_min(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the memory/static_min field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_memory_static_min(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_memory_static_min(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the VCPUs/params field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_VCPUs_params(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_vcpus_params(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the VCPUs/max field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_VCPUs_max(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_vcpus_max(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the VCPUs/at_startup field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_VCPUs_at_startup(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_vcpus_at_startup(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the actions/after_softreboot field of the given VM.
        /// Experimental. First published in 23.1.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static on_softreboot_behavior get_actions_after_softreboot(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_actions_after_softreboot(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the actions/after_shutdown field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static on_normal_exit get_actions_after_shutdown(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_actions_after_shutdown(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the actions/after_reboot field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static on_normal_exit get_actions_after_reboot(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_actions_after_reboot(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the actions/after_crash field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static on_crash_behaviour get_actions_after_crash(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_actions_after_crash(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the consoles field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Console>> get_consoles(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_consoles(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the VIFs field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VIF>> get_VIFs(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_vifs(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the VBDs field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VBD>> get_VBDs(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_vbds(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the VUSBs field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VUSB>> get_VUSBs(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_vusbs(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the crash_dumps field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Crashdump>> get_crash_dumps(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_crash_dumps(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the VTPMs field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VTPM>> get_VTPMs(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_vtpms(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the PV/bootloader field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_PV_bootloader(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_pv_bootloader(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the PV/kernel field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_PV_kernel(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_pv_kernel(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the PV/ramdisk field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_PV_ramdisk(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_pv_ramdisk(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the PV/args field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_PV_args(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_pv_args(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the PV/bootloader_args field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_PV_bootloader_args(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_pv_bootloader_args(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the PV/legacy_args field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_PV_legacy_args(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_pv_legacy_args(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the HVM/boot_policy field of the given VM.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        [Deprecated("XenServer 7.5")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_HVM_boot_policy(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_hvm_boot_policy(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the HVM/boot_params field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_HVM_boot_params(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_hvm_boot_params(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the HVM/shadow_multiplier field of the given VM.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static double get_HVM_shadow_multiplier(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_hvm_shadow_multiplier(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the platform field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_platform(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_platform(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the PCI_bus field of the given VM.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        [Deprecated("XenServer 6.0")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_PCI_bus(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_pci_bus(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the other_config field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_other_config(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the domid field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_domid(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_domid(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the domarch field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_domarch(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_domarch(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the last_boot_CPU_flags field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_last_boot_CPU_flags(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_last_boot_cpu_flags(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the is_control_domain field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_is_control_domain(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_is_control_domain(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the metrics field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VM_metrics> get_metrics(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_metrics(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the guest_metrics field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VM_guest_metrics> get_guest_metrics(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_guest_metrics(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the last_booted_record field of the given VM.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_last_booted_record(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_last_booted_record(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the recommendations field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_recommendations(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_recommendations(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the xenstore_data field of the given VM.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_xenstore_data(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_xenstore_data(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the ha_always_run field of the given VM.
        /// First published in XenServer 5.0.
        /// Deprecated since XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        [Deprecated("XenServer 6.0")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_ha_always_run(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_ha_always_run(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the ha_restart_priority field of the given VM.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_ha_restart_priority(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_ha_restart_priority(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the is_a_snapshot field of the given VM.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_is_a_snapshot(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_is_a_snapshot(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the snapshot_of field of the given VM.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VM> get_snapshot_of(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_snapshot_of(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the snapshots field of the given VM.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VM>> get_snapshots(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_snapshots(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the snapshot_time field of the given VM.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_snapshot_time(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_snapshot_time(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the transportable_snapshot_id field of the given VM.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_transportable_snapshot_id(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_transportable_snapshot_id(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the blobs field of the given VM.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, XenRef<Blob>> get_blobs(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_blobs(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the tags field of the given VM.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_tags(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_tags(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the blocked_operations field of the given VM.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<vm_operations, string> get_blocked_operations(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_blocked_operations(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the snapshot_info field of the given VM.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_snapshot_info(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_snapshot_info(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the snapshot_metadata field of the given VM.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_snapshot_metadata(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_snapshot_metadata(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the parent field of the given VM.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VM> get_parent(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_parent(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the children field of the given VM.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VM>> get_children(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_children(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the bios_strings field of the given VM.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_bios_strings(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_bios_strings(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the protection_policy field of the given VM.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VMPP> get_protection_policy(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_protection_policy(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the is_snapshot_from_vmpp field of the given VM.
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_is_snapshot_from_vmpp(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_is_snapshot_from_vmpp(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the snapshot_schedule field of the given VM.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VMSS> get_snapshot_schedule(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_snapshot_schedule(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the is_vmss_snapshot field of the given VM.
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_is_vmss_snapshot(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_is_vmss_snapshot(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the appliance field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VM_appliance> get_appliance(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_appliance(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the start_delay field of the given VM.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_start_delay(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_start_delay(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the shutdown_delay field of the given VM.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_shutdown_delay(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_shutdown_delay(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the order field of the given VM.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_order(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_order(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the VGPUs field of the given VM.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VGPU>> get_VGPUs(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_vgpus(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the attached_PCIs field of the given VM.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PCI>> get_attached_PCIs(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_attached_pcis(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the suspend_SR field of the given VM.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<SR> get_suspend_SR(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_suspend_sr(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the version field of the given VM.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_version(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_version(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the generation_id field of the given VM.
        /// First published in XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_generation_id(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_generation_id(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the hardware_platform_version field of the given VM.
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_hardware_platform_version(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_hardware_platform_version(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the has_vendor_device field of the given VM.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_has_vendor_device(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_has_vendor_device(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the requires_reboot field of the given VM.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_requires_reboot(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_requires_reboot(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the reference_label field of the given VM.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_reference_label(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_reference_label(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the domain_type field of the given VM.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static domain_type get_domain_type(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_domain_type(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the NVRAM field of the given VM.
        /// First published in Citrix Hypervisor 8.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_NVRAM(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_nvram(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the pending_guidances field of the given VM.
        /// First published in 1.303.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<update_guidances> get_pending_guidances(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_pending_guidances(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the pending_guidances_recommended field of the given VM.
        /// Experimental. First published in 24.10.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<update_guidances> get_pending_guidances_recommended(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_pending_guidances_recommended(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the pending_guidances_full field of the given VM.
        /// Experimental. First published in 24.10.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<update_guidances> get_pending_guidances_full(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_pending_guidances_full(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the groups field of the given VM.
        /// Experimental. First published in 24.19.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VM_group>> get_groups(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_groups(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Get the secureboot_certificates_state field of the given VM.
        /// Experimental. First published in 26.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static vm_secureboot_certificates_state get_secureboot_certificates_state(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_secureboot_certificates_state(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Set the name/label field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_label">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_name_label(Session session, string _vm, string _label)
        {
            session.JsonRpcClient.vm_set_name_label(session.opaque_ref, _vm, _label);
        }

        /// <summary>
        /// Set the name/description field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_description">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_name_description(Session session, string _vm, string _description)
        {
            session.JsonRpcClient.vm_set_name_description(session.opaque_ref, _vm, _description);
        }

        /// <summary>
        /// Set the user_version field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_user_version">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_user_version(Session session, string _vm, long _user_version)
        {
            session.JsonRpcClient.vm_set_user_version(session.opaque_ref, _vm, _user_version);
        }

        /// <summary>
        /// Set the is_a_template field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_is_a_template">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_is_a_template(Session session, string _vm, bool _is_a_template)
        {
            session.JsonRpcClient.vm_set_is_a_template(session.opaque_ref, _vm, _is_a_template);
        }

        /// <summary>
        /// Set the affinity field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_affinity">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void set_affinity(Session session, string _vm, string _affinity)
        {
            session.JsonRpcClient.vm_set_affinity(session.opaque_ref, _vm, _affinity);
        }

        /// <summary>
        /// Set the VCPUs/params field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_params">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_VCPUs_params(Session session, string _vm, Dictionary<string, string> _params)
        {
            session.JsonRpcClient.vm_set_vcpus_params(session.opaque_ref, _vm, _params);
        }

        /// <summary>
        /// Add the given key-value pair to the VCPUs/params field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void add_to_VCPUs_params(Session session, string _vm, string _key, string _value)
        {
            session.JsonRpcClient.vm_add_to_vcpus_params(session.opaque_ref, _vm, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the VCPUs/params field of the given VM.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void remove_from_VCPUs_params(Session session, string _vm, string _key)
        {
            session.JsonRpcClient.vm_remove_from_vcpus_params(session.opaque_ref, _vm, _key);
        }

        /// <summary>
        /// Set the actions/after_softreboot field of the given VM.
        /// Experimental. First published in 23.1.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_after_softreboot">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_actions_after_softreboot(Session session, string _vm, on_softreboot_behavior _after_softreboot)
        {
            session.JsonRpcClient.vm_set_actions_after_softreboot(session.opaque_ref, _vm, _after_softreboot);
        }

        /// <summary>
        /// Set the actions/after_shutdown field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_after_shutdown">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_actions_after_shutdown(Session session, string _vm, on_normal_exit _after_shutdown)
        {
            session.JsonRpcClient.vm_set_actions_after_shutdown(session.opaque_ref, _vm, _after_shutdown);
        }

        /// <summary>
        /// Set the actions/after_reboot field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_after_reboot">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_actions_after_reboot(Session session, string _vm, on_normal_exit _after_reboot)
        {
            session.JsonRpcClient.vm_set_actions_after_reboot(session.opaque_ref, _vm, _after_reboot);
        }

        /// <summary>
        /// Set the PV/bootloader field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_bootloader">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_PV_bootloader(Session session, string _vm, string _bootloader)
        {
            session.JsonRpcClient.vm_set_pv_bootloader(session.opaque_ref, _vm, _bootloader);
        }

        /// <summary>
        /// Set the PV/kernel field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_kernel">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_PV_kernel(Session session, string _vm, string _kernel)
        {
            session.JsonRpcClient.vm_set_pv_kernel(session.opaque_ref, _vm, _kernel);
        }

        /// <summary>
        /// Set the PV/ramdisk field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_ramdisk">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_PV_ramdisk(Session session, string _vm, string _ramdisk)
        {
            session.JsonRpcClient.vm_set_pv_ramdisk(session.opaque_ref, _vm, _ramdisk);
        }

        /// <summary>
        /// Set the PV/args field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_args">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_PV_args(Session session, string _vm, string _args)
        {
            session.JsonRpcClient.vm_set_pv_args(session.opaque_ref, _vm, _args);
        }

        /// <summary>
        /// Set the PV/bootloader_args field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_bootloader_args">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_PV_bootloader_args(Session session, string _vm, string _bootloader_args)
        {
            session.JsonRpcClient.vm_set_pv_bootloader_args(session.opaque_ref, _vm, _bootloader_args);
        }

        /// <summary>
        /// Set the PV/legacy_args field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_legacy_args">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_PV_legacy_args(Session session, string _vm, string _legacy_args)
        {
            session.JsonRpcClient.vm_set_pv_legacy_args(session.opaque_ref, _vm, _legacy_args);
        }

        /// <summary>
        /// Set the HVM/boot_params field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_boot_params">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_HVM_boot_params(Session session, string _vm, Dictionary<string, string> _boot_params)
        {
            session.JsonRpcClient.vm_set_hvm_boot_params(session.opaque_ref, _vm, _boot_params);
        }

        /// <summary>
        /// Add the given key-value pair to the HVM/boot_params field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void add_to_HVM_boot_params(Session session, string _vm, string _key, string _value)
        {
            session.JsonRpcClient.vm_add_to_hvm_boot_params(session.opaque_ref, _vm, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the HVM/boot_params field of the given VM.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void remove_from_HVM_boot_params(Session session, string _vm, string _key)
        {
            session.JsonRpcClient.vm_remove_from_hvm_boot_params(session.opaque_ref, _vm, _key);
        }

        /// <summary>
        /// Set the PCI_bus field of the given VM.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_pci_bus">New value to set</param>
        [Deprecated("XenServer 6.0")]
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_PCI_bus(Session session, string _vm, string _pci_bus)
        {
            session.JsonRpcClient.vm_set_pci_bus(session.opaque_ref, _vm, _pci_bus);
        }

        /// <summary>
        /// Set the recommendations field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_recommendations">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_recommendations(Session session, string _vm, string _recommendations)
        {
            session.JsonRpcClient.vm_set_recommendations(session.opaque_ref, _vm, _recommendations);
        }

        /// <summary>
        /// Set the xenstore_data field of the given VM.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_xenstore_data">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_xenstore_data(Session session, string _vm, Dictionary<string, string> _xenstore_data)
        {
            session.JsonRpcClient.vm_set_xenstore_data(session.opaque_ref, _vm, _xenstore_data);
        }

        /// <summary>
        /// Add the given key-value pair to the xenstore_data field of the given VM.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void add_to_xenstore_data(Session session, string _vm, string _key, string _value)
        {
            session.JsonRpcClient.vm_add_to_xenstore_data(session.opaque_ref, _vm, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the xenstore_data field of the given VM.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void remove_from_xenstore_data(Session session, string _vm, string _key)
        {
            session.JsonRpcClient.vm_remove_from_xenstore_data(session.opaque_ref, _vm, _key);
        }

        /// <summary>
        /// Set the tags field of the given VM.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_tags">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void set_tags(Session session, string _vm, string[] _tags)
        {
            session.JsonRpcClient.vm_set_tags(session.opaque_ref, _vm, _tags);
        }

        /// <summary>
        /// Add the given value to the tags field of the given VM.  If the value is already in that Set, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">New value to add</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void add_tags(Session session, string _vm, string _value)
        {
            session.JsonRpcClient.vm_add_tags(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Remove the given value from the tags field of the given VM.  If the value is not in that Set, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">Value to remove</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void remove_tags(Session session, string _vm, string _value)
        {
            session.JsonRpcClient.vm_remove_tags(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set the suspend_SR field of the given VM.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_suspend_sr">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_suspend_SR(Session session, string _vm, string _suspend_sr)
        {
            session.JsonRpcClient.vm_set_suspend_sr(session.opaque_ref, _vm, _suspend_sr);
        }

        /// <summary>
        /// Set the hardware_platform_version field of the given VM.
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_hardware_platform_version">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_hardware_platform_version(Session session, string _vm, long _hardware_platform_version)
        {
            session.JsonRpcClient.vm_set_hardware_platform_version(session.opaque_ref, _vm, _hardware_platform_version);
        }

        /// <summary>
        /// Snapshots the specified VM, making a new VM. Snapshot automatically exploits the capabilities of the underlying storage repository in which the VM's disk images are stored (for example, Copy on Write).
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_new_name">The name of the snapshotted VM</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<VM> snapshot(Session session, string _vm, string _new_name)
        {
            return session.JsonRpcClient.vm_snapshot(session.opaque_ref, _vm, _new_name);
        }

        /// <summary>
        /// Snapshots the specified VM, making a new VM. Snapshot automatically exploits the capabilities of the underlying storage repository in which the VM's disk images are stored (for example, Copy on Write).
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_new_name">The name of the snapshotted VM</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_snapshot(Session session, string _vm, string _new_name)
        {
          return session.JsonRpcClient.async_vm_snapshot(session.opaque_ref, _vm, _new_name);
        }

        /// <summary>
        /// Snapshots the specified VM, making a new VM. Snapshot automatically exploits the capabilities of the underlying storage repository in which the VM's disk images are stored (for example, Copy on Write).
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_new_name">The name of the snapshotted VM</param>
        /// <param name="_ignore_vdis">A list of VDIs to ignore for the snapshot First published in 21.4.0.</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<VM> snapshot(Session session, string _vm, string _new_name, List<XenRef<VDI>> _ignore_vdis)
        {
            return session.JsonRpcClient.vm_snapshot(session.opaque_ref, _vm, _new_name, _ignore_vdis);
        }

        /// <summary>
        /// Snapshots the specified VM, making a new VM. Snapshot automatically exploits the capabilities of the underlying storage repository in which the VM's disk images are stored (for example, Copy on Write).
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_new_name">The name of the snapshotted VM</param>
        /// <param name="_ignore_vdis">A list of VDIs to ignore for the snapshot First published in 21.4.0.</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_snapshot(Session session, string _vm, string _new_name, List<XenRef<VDI>> _ignore_vdis)
        {
          return session.JsonRpcClient.async_vm_snapshot(session.opaque_ref, _vm, _new_name, _ignore_vdis);
        }

        /// <summary>
        /// Snapshots the specified VM with quiesce, making a new VM. Snapshot automatically exploits the capabilities of the underlying storage repository in which the VM's disk images are stored (for example, Copy on Write).
        /// First published in XenServer 5.0.
        /// Deprecated since Citrix Hypervisor 8.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_new_name">The name of the snapshotted VM</param>
        [Deprecated("Citrix Hypervisor 8.1")]
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<VM> snapshot_with_quiesce(Session session, string _vm, string _new_name)
        {
            return session.JsonRpcClient.vm_snapshot_with_quiesce(session.opaque_ref, _vm, _new_name);
        }

        /// <summary>
        /// Snapshots the specified VM with quiesce, making a new VM. Snapshot automatically exploits the capabilities of the underlying storage repository in which the VM's disk images are stored (for example, Copy on Write).
        /// First published in XenServer 5.0.
        /// Deprecated since Citrix Hypervisor 8.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_new_name">The name of the snapshotted VM</param>
        [Deprecated("Citrix Hypervisor 8.1")]
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_snapshot_with_quiesce(Session session, string _vm, string _new_name)
        {
          return session.JsonRpcClient.async_vm_snapshot_with_quiesce(session.opaque_ref, _vm, _new_name);
        }

        /// <summary>
        /// Clones the specified VM, making a new VM. Clone automatically exploits the capabilities of the underlying storage repository in which the VM's disk images are stored (for example, Copy on Write). This function can only be called when the VM is in the Halted State.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_new_name">The name of the cloned VM</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<VM> clone(Session session, string _vm, string _new_name)
        {
            return session.JsonRpcClient.vm_clone(session.opaque_ref, _vm, _new_name);
        }

        /// <summary>
        /// Clones the specified VM, making a new VM. Clone automatically exploits the capabilities of the underlying storage repository in which the VM's disk images are stored (for example, Copy on Write). This function can only be called when the VM is in the Halted State.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_new_name">The name of the cloned VM</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_clone(Session session, string _vm, string _new_name)
        {
          return session.JsonRpcClient.async_vm_clone(session.opaque_ref, _vm, _new_name);
        }

        /// <summary>
        /// Copied the specified VM, making a new VM. Unlike clone, copy does not exploits the capabilities of the underlying storage repository in which the VM's disk images are stored. Instead, copy guarantees that the disk images of the newly created VM will be 'full disks' - i.e. not part of a CoW chain.  This function can only be called when the VM is in the Halted State.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_new_name">The name of the copied VM</param>
        /// <param name="_sr">An SR to copy all the VM's disks into (if an invalid reference then it uses the existing SRs)</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<VM> copy(Session session, string _vm, string _new_name, string _sr)
        {
            return session.JsonRpcClient.vm_copy(session.opaque_ref, _vm, _new_name, _sr);
        }

        /// <summary>
        /// Copied the specified VM, making a new VM. Unlike clone, copy does not exploits the capabilities of the underlying storage repository in which the VM's disk images are stored. Instead, copy guarantees that the disk images of the newly created VM will be 'full disks' - i.e. not part of a CoW chain.  This function can only be called when the VM is in the Halted State.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_new_name">The name of the copied VM</param>
        /// <param name="_sr">An SR to copy all the VM's disks into (if an invalid reference then it uses the existing SRs)</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_copy(Session session, string _vm, string _new_name, string _sr)
        {
          return session.JsonRpcClient.async_vm_copy(session.opaque_ref, _vm, _new_name, _sr);
        }

        /// <summary>
        /// Reverts the specified VM to a previous state.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given snapshotted state</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void revert(Session session, string _vm)
        {
            session.JsonRpcClient.vm_revert(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Reverts the specified VM to a previous state.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given snapshotted state</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_revert(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_revert(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Checkpoints the specified VM, making a new VM. Checkpoint automatically exploits the capabilities of the underlying storage repository in which the VM's disk images are stored (for example, Copy on Write) and saves the memory image as well.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_new_name">The name of the checkpointed VM</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<VM> checkpoint(Session session, string _vm, string _new_name)
        {
            return session.JsonRpcClient.vm_checkpoint(session.opaque_ref, _vm, _new_name);
        }

        /// <summary>
        /// Checkpoints the specified VM, making a new VM. Checkpoint automatically exploits the capabilities of the underlying storage repository in which the VM's disk images are stored (for example, Copy on Write) and saves the memory image as well.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_new_name">The name of the checkpointed VM</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_checkpoint(Session session, string _vm, string _new_name)
        {
          return session.JsonRpcClient.async_vm_checkpoint(session.opaque_ref, _vm, _new_name);
        }

        /// <summary>
        /// Inspects the disk configuration contained within the VM's other_config, creates VDIs and VBDs and then executes any applicable post-install script.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void provision(Session session, string _vm)
        {
            session.JsonRpcClient.vm_provision(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Inspects the disk configuration contained within the VM's other_config, creates VDIs and VBDs and then executes any applicable post-install script.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_provision(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_provision(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Start the specified VM.  This function can only be called with the VM is in the Halted State.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_start_paused">Instantiate VM in paused state if set to true.</param>
        /// <param name="_force">Attempt to force the VM to start. If this flag is false then the VM may fail pre-boot safety checks (for example, if the CPU the VM last booted on looks substantially different to the current one)</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void start(Session session, string _vm, bool _start_paused, bool _force)
        {
            session.JsonRpcClient.vm_start(session.opaque_ref, _vm, _start_paused, _force);
        }

        /// <summary>
        /// Start the specified VM.  This function can only be called with the VM is in the Halted State.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_start_paused">Instantiate VM in paused state if set to true.</param>
        /// <param name="_force">Attempt to force the VM to start. If this flag is false then the VM may fail pre-boot safety checks (for example, if the CPU the VM last booted on looks substantially different to the current one)</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static XenRef<Task> async_start(Session session, string _vm, bool _start_paused, bool _force)
        {
          return session.JsonRpcClient.async_vm_start(session.opaque_ref, _vm, _start_paused, _force);
        }

        /// <summary>
        /// Start the specified VM on a particular host.  This function can only be called with the VM is in the Halted State.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_host">The Host on which to start the VM</param>
        /// <param name="_start_paused">Instantiate VM in paused state if set to true.</param>
        /// <param name="_force">Attempt to force the VM to start. If this flag is false then the VM may fail pre-boot safety checks (for example, if the CPU the VM last booted on looks substantially different to the current one)</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void start_on(Session session, string _vm, string _host, bool _start_paused, bool _force)
        {
            session.JsonRpcClient.vm_start_on(session.opaque_ref, _vm, _host, _start_paused, _force);
        }

        /// <summary>
        /// Start the specified VM on a particular host.  This function can only be called with the VM is in the Halted State.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_host">The Host on which to start the VM</param>
        /// <param name="_start_paused">Instantiate VM in paused state if set to true.</param>
        /// <param name="_force">Attempt to force the VM to start. If this flag is false then the VM may fail pre-boot safety checks (for example, if the CPU the VM last booted on looks substantially different to the current one)</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_start_on(Session session, string _vm, string _host, bool _start_paused, bool _force)
        {
          return session.JsonRpcClient.async_vm_start_on(session.opaque_ref, _vm, _host, _start_paused, _force);
        }

        /// <summary>
        /// Pause the specified VM. This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void pause(Session session, string _vm)
        {
            session.JsonRpcClient.vm_pause(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Pause the specified VM. This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static XenRef<Task> async_pause(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_pause(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Resume the specified VM. This can only be called when the specified VM is in the Paused state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void unpause(Session session, string _vm)
        {
            session.JsonRpcClient.vm_unpause(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Resume the specified VM. This can only be called when the specified VM is in the Paused state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static XenRef<Task> async_unpause(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_unpause(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Attempt to cleanly shutdown the specified VM. (Note: this may not be supported, for example, if a guest agent is not installed). This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void clean_shutdown(Session session, string _vm)
        {
            session.JsonRpcClient.vm_clean_shutdown(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Attempt to cleanly shutdown the specified VM. (Note: this may not be supported, for example, if a guest agent is not installed). This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static XenRef<Task> async_clean_shutdown(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_clean_shutdown(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Attempts to first clean shutdown a VM and if it should fail then perform a hard shutdown on it.
        /// First published in XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void shutdown(Session session, string _vm)
        {
            session.JsonRpcClient.vm_shutdown(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Attempts to first clean shutdown a VM and if it should fail then perform a hard shutdown on it.
        /// First published in XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static XenRef<Task> async_shutdown(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_shutdown(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Attempt to cleanly shutdown the specified VM (Note: this may not be supported, for example, if a guest agent is not installed). This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void clean_reboot(Session session, string _vm)
        {
            session.JsonRpcClient.vm_clean_reboot(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Attempt to cleanly shutdown the specified VM (Note: this may not be supported, for example, if a guest agent is not installed). This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static XenRef<Task> async_clean_reboot(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_clean_reboot(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Stop executing the specified VM without attempting a clean shutdown.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void hard_shutdown(Session session, string _vm)
        {
            session.JsonRpcClient.vm_hard_shutdown(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Stop executing the specified VM without attempting a clean shutdown.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static XenRef<Task> async_hard_shutdown(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_hard_shutdown(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Reset the power-state of the VM to halted in the database only. (Used to recover from slave failures in pooling scenarios by resetting the power-states of VMs running on dead slaves to halted.) This is a potentially dangerous operation; use with care.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void power_state_reset(Session session, string _vm)
        {
            session.JsonRpcClient.vm_power_state_reset(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Reset the power-state of the VM to halted in the database only. (Used to recover from slave failures in pooling scenarios by resetting the power-states of VMs running on dead slaves to halted.) This is a potentially dangerous operation; use with care.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_power_state_reset(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_power_state_reset(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Stop executing the specified VM without attempting a clean shutdown and immediately restart the VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void hard_reboot(Session session, string _vm)
        {
            session.JsonRpcClient.vm_hard_reboot(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Stop executing the specified VM without attempting a clean shutdown and immediately restart the VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static XenRef<Task> async_hard_reboot(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_hard_reboot(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Suspend the specified VM to disk.  This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void suspend(Session session, string _vm)
        {
            session.JsonRpcClient.vm_suspend(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Suspend the specified VM to disk.  This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static XenRef<Task> async_suspend(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_suspend(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Awaken the specified VM and resume it.  This can only be called when the specified VM is in the Suspended state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_start_paused">Resume VM in paused state if set to true.</param>
        /// <param name="_force">Attempt to force the VM to resume. If this flag is false then the VM may fail pre-resume safety checks (for example, if the CPU the VM was running on looks substantially different to the current one)</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static void resume(Session session, string _vm, bool _start_paused, bool _force)
        {
            session.JsonRpcClient.vm_resume(session.opaque_ref, _vm, _start_paused, _force);
        }

        /// <summary>
        /// Awaken the specified VM and resume it.  This can only be called when the specified VM is in the Suspended state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_start_paused">Resume VM in paused state if set to true.</param>
        /// <param name="_force">Attempt to force the VM to resume. If this flag is false then the VM may fail pre-resume safety checks (for example, if the CPU the VM was running on looks substantially different to the current one)</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static XenRef<Task> async_resume(Session session, string _vm, bool _start_paused, bool _force)
        {
          return session.JsonRpcClient.async_vm_resume(session.opaque_ref, _vm, _start_paused, _force);
        }

        /// <summary>
        /// Awaken the specified VM and resume it on a particular Host.  This can only be called when the specified VM is in the Suspended state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_host">The Host on which to resume the VM</param>
        /// <param name="_start_paused">Resume VM in paused state if set to true.</param>
        /// <param name="_force">Attempt to force the VM to resume. If this flag is false then the VM may fail pre-resume safety checks (for example, if the CPU the VM was running on looks substantially different to the current one)</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void resume_on(Session session, string _vm, string _host, bool _start_paused, bool _force)
        {
            session.JsonRpcClient.vm_resume_on(session.opaque_ref, _vm, _host, _start_paused, _force);
        }

        /// <summary>
        /// Awaken the specified VM and resume it on a particular Host.  This can only be called when the specified VM is in the Suspended state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_host">The Host on which to resume the VM</param>
        /// <param name="_start_paused">Resume VM in paused state if set to true.</param>
        /// <param name="_force">Attempt to force the VM to resume. If this flag is false then the VM may fail pre-resume safety checks (for example, if the CPU the VM was running on looks substantially different to the current one)</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_resume_on(Session session, string _vm, string _host, bool _start_paused, bool _force)
        {
          return session.JsonRpcClient.async_vm_resume_on(session.opaque_ref, _vm, _host, _start_paused, _force);
        }

        /// <summary>
        /// Migrate a VM to another Host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_host">The target host</param>
        /// <param name="_options">Extra configuration operations: force, live, copy, compress. Each is a boolean option, taking 'true' or 'false' as a value. Option 'compress' controls the use of stream compression during migration.</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void pool_migrate(Session session, string _vm, string _host, Dictionary<string, string> _options)
        {
            session.JsonRpcClient.vm_pool_migrate(session.opaque_ref, _vm, _host, _options);
        }

        /// <summary>
        /// Migrate a VM to another Host.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_host">The target host</param>
        /// <param name="_options">Extra configuration operations: force, live, copy, compress. Each is a boolean option, taking 'true' or 'false' as a value. Option 'compress' controls the use of stream compression during migration.</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_pool_migrate(Session session, string _vm, string _host, Dictionary<string, string> _options)
        {
          return session.JsonRpcClient.async_vm_pool_migrate(session.opaque_ref, _vm, _host, _options);
        }

        /// <summary>
        /// Set the number of VCPUs for a running VM
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_nvcpu">The number of VCPUs</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_VCPUs_number_live(Session session, string _vm, long _nvcpu)
        {
            session.JsonRpcClient.vm_set_vcpus_number_live(session.opaque_ref, _vm, _nvcpu);
        }

        /// <summary>
        /// Set the number of VCPUs for a running VM
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_nvcpu">The number of VCPUs</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_set_VCPUs_number_live(Session session, string _vm, long _nvcpu)
        {
          return session.JsonRpcClient.async_vm_set_vcpus_number_live(session.opaque_ref, _vm, _nvcpu);
        }

        /// <summary>
        /// Add the given key-value pair to VM.VCPUs_params, and apply that value on the running VM
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">The key</param>
        /// <param name="_value">The value</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void add_to_VCPUs_params_live(Session session, string _vm, string _key, string _value)
        {
            session.JsonRpcClient.vm_add_to_vcpus_params_live(session.opaque_ref, _vm, _key, _value);
        }

        /// <summary>
        /// Add the given key-value pair to VM.VCPUs_params, and apply that value on the running VM
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">The key</param>
        /// <param name="_value">The value</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_add_to_VCPUs_params_live(Session session, string _vm, string _key, string _value)
        {
          return session.JsonRpcClient.async_vm_add_to_vcpus_params_live(session.opaque_ref, _vm, _key, _value);
        }

        /// <summary>
        /// 
        /// First published in Citrix Hypervisor 8.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The value</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_NVRAM(Session session, string _vm, Dictionary<string, string> _value)
        {
            session.JsonRpcClient.vm_set_nvram(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// 
        /// First published in Citrix Hypervisor 8.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">The key</param>
        /// <param name="_value">The value</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void add_to_NVRAM(Session session, string _vm, string _key, string _value)
        {
            session.JsonRpcClient.vm_add_to_nvram(session.opaque_ref, _vm, _key, _value);
        }

        /// <summary>
        /// 
        /// First published in Citrix Hypervisor 8.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">The key</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void remove_from_NVRAM(Session session, string _vm, string _key)
        {
            session.JsonRpcClient.vm_remove_from_nvram(session.opaque_ref, _vm, _key);
        }

        /// <summary>
        /// Set the value of the ha_restart_priority field
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The value</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_ha_restart_priority(Session session, string _vm, string _value)
        {
            session.JsonRpcClient.vm_set_ha_restart_priority(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set the value of the ha_always_run
        /// First published in XenServer 5.0.
        /// Deprecated since XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The value</param>
        [Deprecated("XenServer 6.0")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_ha_always_run(Session session, string _vm, bool _value)
        {
            session.JsonRpcClient.vm_set_ha_always_run(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Computes the virtualization memory overhead of a VM.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long compute_memory_overhead(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_compute_memory_overhead(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Computes the virtualization memory overhead of a VM.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_compute_memory_overhead(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_compute_memory_overhead(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Set the value of the memory_dynamic_max field
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The new value of memory_dynamic_max</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void set_memory_dynamic_max(Session session, string _vm, long _value)
        {
            session.JsonRpcClient.vm_set_memory_dynamic_max(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set the value of the memory_dynamic_min field
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The new value of memory_dynamic_min</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void set_memory_dynamic_min(Session session, string _vm, long _value)
        {
            session.JsonRpcClient.vm_set_memory_dynamic_min(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set the minimum and maximum amounts of physical memory the VM is allowed to use.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_min">The new minimum value</param>
        /// <param name="_max">The new maximum value</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void set_memory_dynamic_range(Session session, string _vm, long _min, long _max)
        {
            session.JsonRpcClient.vm_set_memory_dynamic_range(session.opaque_ref, _vm, _min, _max);
        }

        /// <summary>
        /// Set the minimum and maximum amounts of physical memory the VM is allowed to use.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_min">The new minimum value</param>
        /// <param name="_max">The new maximum value</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_set_memory_dynamic_range(Session session, string _vm, long _min, long _max)
        {
          return session.JsonRpcClient.async_vm_set_memory_dynamic_range(session.opaque_ref, _vm, _min, _max);
        }

        /// <summary>
        /// Set the value of the memory_static_max field
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The new value of memory_static_max</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void set_memory_static_max(Session session, string _vm, long _value)
        {
            session.JsonRpcClient.vm_set_memory_static_max(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set the value of the memory_static_min field
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The new value of memory_static_min</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void set_memory_static_min(Session session, string _vm, long _value)
        {
            session.JsonRpcClient.vm_set_memory_static_min(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set the static (ie boot-time) range of virtual memory that the VM is allowed to use.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_min">The new minimum value</param>
        /// <param name="_max">The new maximum value</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void set_memory_static_range(Session session, string _vm, long _min, long _max)
        {
            session.JsonRpcClient.vm_set_memory_static_range(session.opaque_ref, _vm, _min, _max);
        }

        /// <summary>
        /// Set the static (ie boot-time) range of virtual memory that the VM is allowed to use.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_min">The new minimum value</param>
        /// <param name="_max">The new maximum value</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_set_memory_static_range(Session session, string _vm, long _min, long _max)
        {
          return session.JsonRpcClient.async_vm_set_memory_static_range(session.opaque_ref, _vm, _min, _max);
        }

        /// <summary>
        /// Set the memory limits of this VM.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_static_min">The new value of memory_static_min.</param>
        /// <param name="_static_max">The new value of memory_static_max.</param>
        /// <param name="_dynamic_min">The new value of memory_dynamic_min.</param>
        /// <param name="_dynamic_max">The new value of memory_dynamic_max.</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void set_memory_limits(Session session, string _vm, long _static_min, long _static_max, long _dynamic_min, long _dynamic_max)
        {
            session.JsonRpcClient.vm_set_memory_limits(session.opaque_ref, _vm, _static_min, _static_max, _dynamic_min, _dynamic_max);
        }

        /// <summary>
        /// Set the memory limits of this VM.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_static_min">The new value of memory_static_min.</param>
        /// <param name="_static_max">The new value of memory_static_max.</param>
        /// <param name="_dynamic_min">The new value of memory_dynamic_min.</param>
        /// <param name="_dynamic_max">The new value of memory_dynamic_max.</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_set_memory_limits(Session session, string _vm, long _static_min, long _static_max, long _dynamic_min, long _dynamic_max)
        {
          return session.JsonRpcClient.async_vm_set_memory_limits(session.opaque_ref, _vm, _static_min, _static_max, _dynamic_min, _dynamic_max);
        }

        /// <summary>
        /// Set the memory allocation of this VM. Sets all of memory_static_max, memory_dynamic_min, and memory_dynamic_max to the given value, and leaves memory_static_min untouched.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The new memory allocation (bytes).</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void set_memory(Session session, string _vm, long _value)
        {
            session.JsonRpcClient.vm_set_memory(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set the memory allocation of this VM. Sets all of memory_static_max, memory_dynamic_min, and memory_dynamic_max to the given value, and leaves memory_static_min untouched.
        /// First published in XenServer 7.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The new memory allocation (bytes).</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_set_memory(Session session, string _vm, long _value)
        {
          return session.JsonRpcClient.async_vm_set_memory(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set the memory target for a running VM
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_target">The target in bytes</param>
        [Deprecated("XenServer 5.6")]
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void set_memory_target_live(Session session, string _vm, long _target)
        {
            session.JsonRpcClient.vm_set_memory_target_live(session.opaque_ref, _vm, _target);
        }

        /// <summary>
        /// Set the memory target for a running VM
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_target">The target in bytes</param>
        [Deprecated("XenServer 5.6")]
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_set_memory_target_live(Session session, string _vm, long _target)
        {
          return session.JsonRpcClient.async_vm_set_memory_target_live(session.opaque_ref, _vm, _target);
        }

        /// <summary>
        /// Wait for a running VM to reach its current memory target
        /// First published in XenServer 5.0.
        /// Deprecated since XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        [Deprecated("XenServer 5.6")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static void wait_memory_target_live(Session session, string _vm)
        {
            session.JsonRpcClient.vm_wait_memory_target_live(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Wait for a running VM to reach its current memory target
        /// First published in XenServer 5.0.
        /// Deprecated since XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        [Deprecated("XenServer 5.6")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_wait_memory_target_live(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_wait_memory_target_live(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Return true if the VM is currently 'co-operative' i.e. is expected to reach a balloon target and actually has done
        /// First published in XenServer 5.6.
        /// Deprecated since XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        [Deprecated("XenServer 6.1")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_cooperative(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_cooperative(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Return true if the VM is currently 'co-operative' i.e. is expected to reach a balloon target and actually has done
        /// First published in XenServer 5.6.
        /// Deprecated since XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        [Deprecated("XenServer 6.1")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_get_cooperative(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_get_cooperative(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Set the shadow memory multiplier on a halted VM
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The new shadow memory multiplier to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void set_HVM_shadow_multiplier(Session session, string _vm, double _value)
        {
            session.JsonRpcClient.vm_set_hvm_shadow_multiplier(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set the shadow memory multiplier on a running VM
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_multiplier">The new shadow memory multiplier to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void set_shadow_multiplier_live(Session session, string _vm, double _multiplier)
        {
            session.JsonRpcClient.vm_set_shadow_multiplier_live(session.opaque_ref, _vm, _multiplier);
        }

        /// <summary>
        /// Set the shadow memory multiplier on a running VM
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_multiplier">The new shadow memory multiplier to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_set_shadow_multiplier_live(Session session, string _vm, double _multiplier)
        {
          return session.JsonRpcClient.async_vm_set_shadow_multiplier_live(session.opaque_ref, _vm, _multiplier);
        }

        /// <summary>
        /// Set the maximum number of VCPUs for a halted VM
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The new maximum number of VCPUs</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_VCPUs_max(Session session, string _vm, long _value)
        {
            session.JsonRpcClient.vm_set_vcpus_max(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set the number of startup VCPUs for a halted VM
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The new maximum number of VCPUs</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_VCPUs_at_startup(Session session, string _vm, long _value)
        {
            session.JsonRpcClient.vm_set_vcpus_at_startup(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Send the given key as a sysrq to this VM.  The key is specified as a single character (a String of length 1).  This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">The key to send</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void send_sysrq(Session session, string _vm, string _key)
        {
            session.JsonRpcClient.vm_send_sysrq(session.opaque_ref, _vm, _key);
        }

        /// <summary>
        /// Send the given key as a sysrq to this VM.  The key is specified as a single character (a String of length 1).  This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">The key to send</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_send_sysrq(Session session, string _vm, string _key)
        {
          return session.JsonRpcClient.async_vm_send_sysrq(session.opaque_ref, _vm, _key);
        }

        /// <summary>
        /// Send the named trigger to this VM.  This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_trigger">The trigger to send</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void send_trigger(Session session, string _vm, string _trigger)
        {
            session.JsonRpcClient.vm_send_trigger(session.opaque_ref, _vm, _trigger);
        }

        /// <summary>
        /// Send the named trigger to this VM.  This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_trigger">The trigger to send</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_send_trigger(Session session, string _vm, string _trigger)
        {
          return session.JsonRpcClient.async_vm_send_trigger(session.opaque_ref, _vm, _trigger);
        }

        /// <summary>
        /// Returns the maximum amount of guest memory which will fit, together with overheads, in the supplied amount of physical memory. If 'exact' is true then an exact calculation is performed using the VM's current settings. If 'exact' is false then a more conservative approximation is used
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_total">Total amount of physical RAM to fit within</param>
        /// <param name="_approximate">If false the limit is calculated with the guest's current exact configuration. Otherwise a more approximate calculation is performed</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long maximise_memory(Session session, string _vm, long _total, bool _approximate)
        {
            return session.JsonRpcClient.vm_maximise_memory(session.opaque_ref, _vm, _total, _approximate);
        }

        /// <summary>
        /// Returns the maximum amount of guest memory which will fit, together with overheads, in the supplied amount of physical memory. If 'exact' is true then an exact calculation is performed using the VM's current settings. If 'exact' is false then a more conservative approximation is used
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_total">Total amount of physical RAM to fit within</param>
        /// <param name="_approximate">If false the limit is calculated with the guest's current exact configuration. Otherwise a more approximate calculation is performed</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_maximise_memory(Session session, string _vm, long _total, bool _approximate)
        {
          return session.JsonRpcClient.async_vm_maximise_memory(session.opaque_ref, _vm, _total, _approximate);
        }

        /// <summary>
        /// Migrate the VM to another host.  This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_dest">The result of a Host.migrate_receive call.</param>
        /// <param name="_live">Live migration</param>
        /// <param name="_vdi_map">Map of source VDI to destination SR</param>
        /// <param name="_vif_map">Map of source VIF to destination network</param>
        /// <param name="_options">Other parameters</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<VM> migrate_send(Session session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options)
        {
            return session.JsonRpcClient.vm_migrate_send(session.opaque_ref, _vm, _dest, _live, _vdi_map, _vif_map, _options);
        }

        /// <summary>
        /// Migrate the VM to another host.  This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_dest">The result of a Host.migrate_receive call.</param>
        /// <param name="_live">Live migration</param>
        /// <param name="_vdi_map">Map of source VDI to destination SR</param>
        /// <param name="_vif_map">Map of source VIF to destination network</param>
        /// <param name="_options">Other parameters</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_migrate_send(Session session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options)
        {
          return session.JsonRpcClient.async_vm_migrate_send(session.opaque_ref, _vm, _dest, _live, _vdi_map, _vif_map, _options);
        }

        /// <summary>
        /// Migrate the VM to another host.  This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_dest">The result of a Host.migrate_receive call.</param>
        /// <param name="_live">Live migration</param>
        /// <param name="_vdi_map">Map of source VDI to destination SR</param>
        /// <param name="_vif_map">Map of source VIF to destination network</param>
        /// <param name="_options">Other parameters</param>
        /// <param name="_vgpu_map">Map of source vGPU to destination GPU group First published in XenServer 7.3.</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<VM> migrate_send(Session session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options, Dictionary<XenRef<VGPU>, XenRef<GPU_group>> _vgpu_map)
        {
            return session.JsonRpcClient.vm_migrate_send(session.opaque_ref, _vm, _dest, _live, _vdi_map, _vif_map, _options, _vgpu_map);
        }

        /// <summary>
        /// Migrate the VM to another host.  This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_dest">The result of a Host.migrate_receive call.</param>
        /// <param name="_live">Live migration</param>
        /// <param name="_vdi_map">Map of source VDI to destination SR</param>
        /// <param name="_vif_map">Map of source VIF to destination network</param>
        /// <param name="_options">Other parameters</param>
        /// <param name="_vgpu_map">Map of source vGPU to destination GPU group First published in XenServer 7.3.</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_migrate_send(Session session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options, Dictionary<XenRef<VGPU>, XenRef<GPU_group>> _vgpu_map)
        {
          return session.JsonRpcClient.async_vm_migrate_send(session.opaque_ref, _vm, _dest, _live, _vdi_map, _vif_map, _options, _vgpu_map);
        }

        /// <summary>
        /// Migrate the VM to another host.  This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_dest">The result of a Host.migrate_receive call.</param>
        /// <param name="_live">Live migration</param>
        /// <param name="_vdi_map">Map of source VDI to destination SR</param>
        /// <param name="_vif_map">Map of source VIF to destination network</param>
        /// <param name="_options">Other parameters</param>
        /// <param name="_vgpu_map">Map of source vGPU to destination GPU group First published in XenServer 7.3.</param>
        /// <param name="_vdi_format_map">Map of source VDI to it's expected type on destination First published in 26.14.0-next.</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<VM> migrate_send(Session session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options, Dictionary<XenRef<VGPU>, XenRef<GPU_group>> _vgpu_map, Dictionary<XenRef<VDI>, string> _vdi_format_map)
        {
            return session.JsonRpcClient.vm_migrate_send(session.opaque_ref, _vm, _dest, _live, _vdi_map, _vif_map, _options, _vgpu_map, _vdi_format_map);
        }

        /// <summary>
        /// Migrate the VM to another host.  This can only be called when the specified VM is in the Running state.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_dest">The result of a Host.migrate_receive call.</param>
        /// <param name="_live">Live migration</param>
        /// <param name="_vdi_map">Map of source VDI to destination SR</param>
        /// <param name="_vif_map">Map of source VIF to destination network</param>
        /// <param name="_options">Other parameters</param>
        /// <param name="_vgpu_map">Map of source vGPU to destination GPU group First published in XenServer 7.3.</param>
        /// <param name="_vdi_format_map">Map of source VDI to it's expected type on destination First published in 26.14.0-next.</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_migrate_send(Session session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options, Dictionary<XenRef<VGPU>, XenRef<GPU_group>> _vgpu_map, Dictionary<XenRef<VDI>, string> _vdi_format_map)
        {
          return session.JsonRpcClient.async_vm_migrate_send(session.opaque_ref, _vm, _dest, _live, _vdi_map, _vif_map, _options, _vgpu_map, _vdi_format_map);
        }

        /// <summary>
        /// Assert whether a VM can be migrated to the specified destination.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_dest">The result of a Host.migrate_receive call.</param>
        /// <param name="_live">Live migration</param>
        /// <param name="_vdi_map">Map of source VDI to destination SR</param>
        /// <param name="_vif_map">Map of source VIF to destination network</param>
        /// <param name="_options">Other parameters</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void assert_can_migrate(Session session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options)
        {
            session.JsonRpcClient.vm_assert_can_migrate(session.opaque_ref, _vm, _dest, _live, _vdi_map, _vif_map, _options);
        }

        /// <summary>
        /// Assert whether a VM can be migrated to the specified destination.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_dest">The result of a Host.migrate_receive call.</param>
        /// <param name="_live">Live migration</param>
        /// <param name="_vdi_map">Map of source VDI to destination SR</param>
        /// <param name="_vif_map">Map of source VIF to destination network</param>
        /// <param name="_options">Other parameters</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_assert_can_migrate(Session session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options)
        {
          return session.JsonRpcClient.async_vm_assert_can_migrate(session.opaque_ref, _vm, _dest, _live, _vdi_map, _vif_map, _options);
        }

        /// <summary>
        /// Assert whether a VM can be migrated to the specified destination.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_dest">The result of a Host.migrate_receive call.</param>
        /// <param name="_live">Live migration</param>
        /// <param name="_vdi_map">Map of source VDI to destination SR</param>
        /// <param name="_vif_map">Map of source VIF to destination network</param>
        /// <param name="_options">Other parameters</param>
        /// <param name="_vgpu_map">Map of source vGPU to destination GPU group First published in XenServer 7.3.</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void assert_can_migrate(Session session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options, Dictionary<XenRef<VGPU>, XenRef<GPU_group>> _vgpu_map)
        {
            session.JsonRpcClient.vm_assert_can_migrate(session.opaque_ref, _vm, _dest, _live, _vdi_map, _vif_map, _options, _vgpu_map);
        }

        /// <summary>
        /// Assert whether a VM can be migrated to the specified destination.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_dest">The result of a Host.migrate_receive call.</param>
        /// <param name="_live">Live migration</param>
        /// <param name="_vdi_map">Map of source VDI to destination SR</param>
        /// <param name="_vif_map">Map of source VIF to destination network</param>
        /// <param name="_options">Other parameters</param>
        /// <param name="_vgpu_map">Map of source vGPU to destination GPU group First published in XenServer 7.3.</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_assert_can_migrate(Session session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options, Dictionary<XenRef<VGPU>, XenRef<GPU_group>> _vgpu_map)
        {
          return session.JsonRpcClient.async_vm_assert_can_migrate(session.opaque_ref, _vm, _dest, _live, _vdi_map, _vif_map, _options, _vgpu_map);
        }

        /// <summary>
        /// Returns a record describing the VM's dynamic state, initialised when the VM boots and updated to reflect runtime configuration changes, for example CPU hotplug
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        [Deprecated("XenServer 7.3")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static VM get_boot_record(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_boot_record(session.opaque_ref, _vm);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<Data_source> get_data_sources(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_data_sources(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Start recording the specified data source
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_data_source">The data source to record</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void record_data_source(Session session, string _vm, string _data_source)
        {
            session.JsonRpcClient.vm_record_data_source(session.opaque_ref, _vm, _data_source);
        }

        /// <summary>
        /// Query the latest value of the specified data source
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_data_source">The data source to query</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static double query_data_source(Session session, string _vm, string _data_source)
        {
            return session.JsonRpcClient.vm_query_data_source(session.opaque_ref, _vm, _data_source);
        }

        /// <summary>
        /// Forget the recorded statistics related to the specified data source
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_data_source">The data source whose archives are to be forgotten</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void forget_data_source_archives(Session session, string _vm, string _data_source)
        {
            session.JsonRpcClient.vm_forget_data_source_archives(session.opaque_ref, _vm, _data_source);
        }

        /// <summary>
        /// Check to see whether this operation is acceptable in the current state of the system, raising an error if the operation is invalid for some reason
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_op">proposed operation</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static void assert_operation_valid(Session session, string _vm, vm_operations _op)
        {
            session.JsonRpcClient.vm_assert_operation_valid(session.opaque_ref, _vm, _op);
        }

        /// <summary>
        /// Check to see whether this operation is acceptable in the current state of the system, raising an error if the operation is invalid for some reason
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_op">proposed operation</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_assert_operation_valid(Session session, string _vm, vm_operations _op)
        {
          return session.JsonRpcClient.async_vm_assert_operation_valid(session.opaque_ref, _vm, _op);
        }

        /// <summary>
        /// Recomputes the list of acceptable operations
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void update_allowed_operations(Session session, string _vm)
        {
            session.JsonRpcClient.vm_update_allowed_operations(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Recomputes the list of acceptable operations
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_update_allowed_operations(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_update_allowed_operations(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Returns a list of the allowed values that a VBD device field can take
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_allowed_VBD_devices(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_allowed_vbd_devices(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Returns a list of the allowed values that a VIF device field can take
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_allowed_VIF_devices(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_allowed_vif_devices(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Return the list of hosts on which this VM may run.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Host>> get_possible_hosts(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_possible_hosts(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Return the list of hosts on which this VM may run.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_get_possible_hosts(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_get_possible_hosts(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Returns an error if the VM could not boot on this host for some reason
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_host">The host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static void assert_can_boot_here(Session session, string _vm, string _host)
        {
            session.JsonRpcClient.vm_assert_can_boot_here(session.opaque_ref, _vm, _host);
        }

        /// <summary>
        /// Returns an error if the VM could not boot on this host for some reason
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_host">The host</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_assert_can_boot_here(Session session, string _vm, string _host)
        {
          return session.JsonRpcClient.async_vm_assert_can_boot_here(session.opaque_ref, _vm, _host);
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this VM
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Blob> create_new_blob(Session session, string _vm, string _name, string _mime_type)
        {
            return session.JsonRpcClient.vm_create_new_blob(session.opaque_ref, _vm, _name, _mime_type);
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this VM
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_create_new_blob(Session session, string _vm, string _name, string _mime_type)
        {
          return session.JsonRpcClient.async_vm_create_new_blob(session.opaque_ref, _vm, _name, _mime_type);
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this VM
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <param name="_public">True if the blob should be publicly available First published in XenServer 6.1.</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Blob> create_new_blob(Session session, string _vm, string _name, string _mime_type, bool _public)
        {
            return session.JsonRpcClient.vm_create_new_blob(session.opaque_ref, _vm, _name, _mime_type, _public);
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this VM
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <param name="_public">True if the blob should be publicly available First published in XenServer 6.1.</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_create_new_blob(Session session, string _vm, string _name, string _mime_type, bool _public)
        {
          return session.JsonRpcClient.async_vm_create_new_blob(session.opaque_ref, _vm, _name, _mime_type, _public);
        }

        /// <summary>
        /// Returns an error if the VM is not considered agile, for example, because it is tied to a resource local to a host
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static void assert_agile(Session session, string _vm)
        {
            session.JsonRpcClient.vm_assert_agile(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Returns an error if the VM is not considered agile, for example, because it is tied to a resource local to a host
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_assert_agile(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_assert_agile(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Returns mapping of hosts to ratings, indicating the suitability of starting the VM at that location according to wlb. Rating is replaced with an error if the VM cannot boot there.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Host>, string[]> retrieve_wlb_recommendations(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_retrieve_wlb_recommendations(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Returns mapping of hosts to ratings, indicating the suitability of starting the VM at that location according to wlb. Rating is replaced with an error if the VM cannot boot there.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_retrieve_wlb_recommendations(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_retrieve_wlb_recommendations(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Set custom BIOS strings to this VM. VM will be given a default set of BIOS strings, only some of which can be overridden by the supplied values. Allowed keys are: 'bios-vendor', 'bios-version', 'system-manufacturer', 'system-product-name', 'system-version', 'system-serial-number', 'enclosure-asset-tag', 'baseboard-manufacturer', 'baseboard-product-name', 'baseboard-version', 'baseboard-serial-number', 'baseboard-asset-tag', 'baseboard-location-in-chassis', 'enclosure-asset-tag'
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The custom BIOS strings as a list of key-value pairs</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_bios_strings(Session session, string _vm, Dictionary<string, string> _value)
        {
            session.JsonRpcClient.vm_set_bios_strings(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set custom BIOS strings to this VM. VM will be given a default set of BIOS strings, only some of which can be overridden by the supplied values. Allowed keys are: 'bios-vendor', 'bios-version', 'system-manufacturer', 'system-product-name', 'system-version', 'system-serial-number', 'enclosure-asset-tag', 'baseboard-manufacturer', 'baseboard-product-name', 'baseboard-version', 'baseboard-serial-number', 'baseboard-asset-tag', 'baseboard-location-in-chassis', 'enclosure-asset-tag'
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The custom BIOS strings as a list of key-value pairs</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_set_bios_strings(Session session, string _vm, Dictionary<string, string> _value)
        {
          return session.JsonRpcClient.async_vm_set_bios_strings(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Copy the BIOS strings from the given host to this VM
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_host">The host to copy the BIOS strings from</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void copy_bios_strings(Session session, string _vm, string _host)
        {
            session.JsonRpcClient.vm_copy_bios_strings(session.opaque_ref, _vm, _host);
        }

        /// <summary>
        /// Copy the BIOS strings from the given host to this VM
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_host">The host to copy the BIOS strings from</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_copy_bios_strings(Session session, string _vm, string _host)
        {
          return session.JsonRpcClient.async_vm_copy_bios_strings(session.opaque_ref, _vm, _host);
        }

        /// <summary>
        /// Set the value of the protection_policy field
        /// First published in XenServer 5.6 FP1.
        /// Deprecated since XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The value</param>
        [Deprecated("XenServer 6.2")]
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_protection_policy(Session session, string _vm, string _value)
        {
            session.JsonRpcClient.vm_set_protection_policy(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set the value of the snapshot schedule field
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The value</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_snapshot_schedule(Session session, string _vm, string _value)
        {
            session.JsonRpcClient.vm_set_snapshot_schedule(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set this VM's start delay in seconds
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">This VM's start delay in seconds</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_start_delay(Session session, string _vm, long _value)
        {
            session.JsonRpcClient.vm_set_start_delay(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set this VM's start delay in seconds
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">This VM's start delay in seconds</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_start_delay(Session session, string _vm, long _value)
        {
          return session.JsonRpcClient.async_vm_set_start_delay(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set this VM's shutdown delay in seconds
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">This VM's shutdown delay in seconds</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_shutdown_delay(Session session, string _vm, long _value)
        {
            session.JsonRpcClient.vm_set_shutdown_delay(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set this VM's shutdown delay in seconds
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">This VM's shutdown delay in seconds</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_shutdown_delay(Session session, string _vm, long _value)
        {
          return session.JsonRpcClient.async_vm_set_shutdown_delay(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set this VM's boot order
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">This VM's boot order</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_order(Session session, string _vm, long _value)
        {
            session.JsonRpcClient.vm_set_order(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set this VM's boot order
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">This VM's boot order</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_order(Session session, string _vm, long _value)
        {
          return session.JsonRpcClient.async_vm_set_order(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set this VM's suspend VDI, which must be indentical to its current one
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The suspend VDI uuid</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_suspend_VDI(Session session, string _vm, string _value)
        {
            session.JsonRpcClient.vm_set_suspend_vdi(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set this VM's suspend VDI, which must be indentical to its current one
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The suspend VDI uuid</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_suspend_VDI(Session session, string _vm, string _value)
        {
          return session.JsonRpcClient.async_vm_set_suspend_vdi(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Assert whether all SRs required to recover this VM are available.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_session_to">The session to which the VM is to be recovered.</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static void assert_can_be_recovered(Session session, string _vm, string _session_to)
        {
            session.JsonRpcClient.vm_assert_can_be_recovered(session.opaque_ref, _vm, _session_to);
        }

        /// <summary>
        /// Assert whether all SRs required to recover this VM are available.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_session_to">The session to which the VM is to be recovered.</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_assert_can_be_recovered(Session session, string _vm, string _session_to)
        {
          return session.JsonRpcClient.async_vm_assert_can_be_recovered(session.opaque_ref, _vm, _session_to);
        }

        /// <summary>
        /// List all the SR's that are required for the VM to be recovered
        /// First published in XenServer 6.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_session_to">The session to which the SRs of the VM have to be recovered.</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<SR>> get_SRs_required_for_recovery(Session session, string _vm, string _session_to)
        {
            return session.JsonRpcClient.vm_get_srs_required_for_recovery(session.opaque_ref, _vm, _session_to);
        }

        /// <summary>
        /// List all the SR's that are required for the VM to be recovered
        /// First published in XenServer 6.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_session_to">The session to which the SRs of the VM have to be recovered.</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_get_SRs_required_for_recovery(Session session, string _vm, string _session_to)
        {
          return session.JsonRpcClient.async_vm_get_srs_required_for_recovery(session.opaque_ref, _vm, _session_to);
        }

        /// <summary>
        /// Recover the VM
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_session_to">The session to which the VM is to be recovered.</param>
        /// <param name="_force">Whether the VM should replace newer versions of itself.</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static void recover(Session session, string _vm, string _session_to, bool _force)
        {
            session.JsonRpcClient.vm_recover(session.opaque_ref, _vm, _session_to, _force);
        }

        /// <summary>
        /// Recover the VM
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_session_to">The session to which the VM is to be recovered.</param>
        /// <param name="_force">Whether the VM should replace newer versions of itself.</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_recover(Session session, string _vm, string _session_to, bool _force)
        {
          return session.JsonRpcClient.async_vm_recover(session.opaque_ref, _vm, _session_to, _force);
        }

        /// <summary>
        /// Import using a conversion service.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_type">Type of the conversion</param>
        /// <param name="_username">Admin username on the host</param>
        /// <param name="_password">Password on the host</param>
        /// <param name="_sr">The destination SR</param>
        /// <param name="_remote_config">Remote configuration options</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void import_convert(Session session, string _type, string _username, string _password, string _sr, Dictionary<string, string> _remote_config)
        {
            session.JsonRpcClient.vm_import_convert(session.opaque_ref, _type, _username, _password, _sr, _remote_config);
        }

        /// <summary>
        /// Import using a conversion service.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_type">Type of the conversion</param>
        /// <param name="_username">Admin username on the host</param>
        /// <param name="_password">Password on the host</param>
        /// <param name="_sr">The destination SR</param>
        /// <param name="_remote_config">Remote configuration options</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_import_convert(Session session, string _type, string _username, string _password, string _sr, Dictionary<string, string> _remote_config)
        {
          return session.JsonRpcClient.async_vm_import_convert(session.opaque_ref, _type, _username, _password, _sr, _remote_config);
        }

        /// <summary>
        /// Assign this VM to an appliance.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The appliance to which this VM should be assigned.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_appliance(Session session, string _vm, string _value)
        {
            session.JsonRpcClient.vm_set_appliance(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Assign this VM to an appliance.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The appliance to which this VM should be assigned.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_appliance(Session session, string _vm, string _value)
        {
          return session.JsonRpcClient.async_vm_set_appliance(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Associate this VM with VM groups.
        /// Experimental. First published in 24.19.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The VM groups to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_groups(Session session, string _vm, List<XenRef<VM_group>> _value)
        {
            session.JsonRpcClient.vm_set_groups(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Associate this VM with VM groups.
        /// Experimental. First published in 24.19.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The VM groups to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_set_groups(Session session, string _vm, List<XenRef<VM_group>> _value)
        {
          return session.JsonRpcClient.async_vm_set_groups(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Query the system services advertised by this VM and register them. This can only be applied to a system domain.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static Dictionary<string, string> query_services(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_query_services(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Query the system services advertised by this VM and register them. This can only be applied to a system domain.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_query_services(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_query_services(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Call an API plugin on this vm
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_plugin">The name of the plugin</param>
        /// <param name="_fn">The name of the function within the plugin</param>
        /// <param name="_args">Arguments for the function</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static string call_plugin(Session session, string _vm, string _plugin, string _fn, Dictionary<string, string> _args)
        {
            return session.JsonRpcClient.vm_call_plugin(session.opaque_ref, _vm, _plugin, _fn, _args);
        }

        /// <summary>
        /// Call an API plugin on this vm
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_plugin">The name of the plugin</param>
        /// <param name="_fn">The name of the function within the plugin</param>
        /// <param name="_args">Arguments for the function</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static XenRef<Task> async_call_plugin(Session session, string _vm, string _plugin, string _fn, Dictionary<string, string> _args)
        {
          return session.JsonRpcClient.async_vm_call_plugin(session.opaque_ref, _vm, _plugin, _fn, _args);
        }

        /// <summary>
        /// Call an API plugin on the host where this vm resides
        /// Experimental. First published in 25.22.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_plugin">The name of the plugin</param>
        /// <param name="_fn">The name of the function within the plugin</param>
        /// <param name="_args">Arguments for the function</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static string call_host_plugin(Session session, string _vm, string _plugin, string _fn, Dictionary<string, string> _args)
        {
            return session.JsonRpcClient.vm_call_host_plugin(session.opaque_ref, _vm, _plugin, _fn, _args);
        }

        /// <summary>
        /// Call an API plugin on the host where this vm resides
        /// Experimental. First published in 25.22.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_plugin">The name of the plugin</param>
        /// <param name="_fn">The name of the function within the plugin</param>
        /// <param name="_args">Arguments for the function</param>
        /// <remarks>
        /// Minimum allowed role: vm-operator
        /// </remarks>
        public static XenRef<Task> async_call_host_plugin(Session session, string _vm, string _plugin, string _fn, Dictionary<string, string> _args)
        {
          return session.JsonRpcClient.async_vm_call_host_plugin(session.opaque_ref, _vm, _plugin, _fn, _args);
        }

        /// <summary>
        /// Controls whether, when the VM starts in HVM mode, its virtual hardware will include the emulated PCI device for which drivers may be available through Windows Update. Usually this should never be changed on a VM on which Windows has been installed: changing it on such a VM is likely to lead to a crash on next start.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">True to provide the vendor PCI device.</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_has_vendor_device(Session session, string _vm, bool _value)
        {
            session.JsonRpcClient.vm_set_has_vendor_device(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Controls whether, when the VM starts in HVM mode, its virtual hardware will include the emulated PCI device for which drivers may be available through Windows Update. Usually this should never be changed on a VM on which Windows has been installed: changing it on such a VM is likely to lead to a crash on next start.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">True to provide the vendor PCI device.</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_set_has_vendor_device(Session session, string _vm, bool _value)
        {
          return session.JsonRpcClient.async_vm_set_has_vendor_device(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Import an XVA from a URI
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_url">The URL of the XVA file</param>
        /// <param name="_sr">The destination SR for the disks</param>
        /// <param name="_full_restore">Perform a full restore</param>
        /// <param name="_force">Force the import</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static List<XenRef<VM>> import(Session session, string _url, string _sr, bool _full_restore, bool _force)
        {
            return session.JsonRpcClient.vm_import(session.opaque_ref, _url, _sr, _full_restore, _force);
        }

        /// <summary>
        /// Import an XVA from a URI
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_url">The URL of the XVA file</param>
        /// <param name="_sr">The destination SR for the disks</param>
        /// <param name="_full_restore">Perform a full restore</param>
        /// <param name="_force">Force the import</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_import(Session session, string _url, string _sr, bool _full_restore, bool _force)
        {
          return session.JsonRpcClient.async_vm_import(session.opaque_ref, _url, _sr, _full_restore, _force);
        }

        /// <summary>
        /// Sets the actions_after_crash parameter
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The new value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_actions_after_crash(Session session, string _vm, on_crash_behaviour _value)
        {
            session.JsonRpcClient.vm_set_actions_after_crash(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Sets the actions_after_crash parameter
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The new value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_set_actions_after_crash(Session session, string _vm, on_crash_behaviour _value)
        {
          return session.JsonRpcClient.async_vm_set_actions_after_crash(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set the VM.domain_type field of the given VM, which will take effect when it is next started
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The new domain type</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_domain_type(Session session, string _vm, domain_type _value)
        {
            session.JsonRpcClient.vm_set_domain_type(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Set the VM.HVM_boot_policy field of the given VM, which will take effect when it is next started
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">The new HVM boot policy</param>
        [Deprecated("XenServer 7.5")]
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_HVM_boot_policy(Session session, string _vm, string _value)
        {
            session.JsonRpcClient.vm_set_hvm_boot_policy(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Restart device models of the VM
        /// Experimental. First published in 23.30.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static void restart_device_models(Session session, string _vm)
        {
            session.JsonRpcClient.vm_restart_device_models(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Restart device models of the VM
        /// Experimental. First published in 23.30.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: vm-power-admin
        /// </remarks>
        public static XenRef<Task> async_restart_device_models(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_restart_device_models(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Set the UEFI mode of a VM
        /// Experimental. First published in 24.17.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_mode">The UEFI mode to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static string set_uefi_mode(Session session, string _vm, vm_uefi_mode _mode)
        {
            return session.JsonRpcClient.vm_set_uefi_mode(session.opaque_ref, _vm, _mode);
        }

        /// <summary>
        /// Set the UEFI mode of a VM
        /// Experimental. First published in 24.17.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_mode">The UEFI mode to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_uefi_mode(Session session, string _vm, vm_uefi_mode _mode)
        {
          return session.JsonRpcClient.async_vm_set_uefi_mode(session.opaque_ref, _vm, _mode);
        }

        /// <summary>
        /// Return the secureboot readiness of the VM
        /// Experimental. First published in 24.17.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static vm_secureboot_readiness get_secureboot_readiness(Session session, string _vm)
        {
            return session.JsonRpcClient.vm_get_secureboot_readiness(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Return the secureboot readiness of the VM
        /// Experimental. First published in 24.17.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_get_secureboot_readiness(Session session, string _vm)
        {
          return session.JsonRpcClient.async_vm_get_secureboot_readiness(session.opaque_ref, _vm);
        }

        /// <summary>
        /// Mark or unmark secure boot certificate update on VM boot
        /// Experimental. First published in 26.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_mark">If true: mark certificates for update on next boot. If false: remove the mark</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void update_secureboot_certificates_on_boot(Session session, string _vm, bool _mark)
        {
            session.JsonRpcClient.vm_update_secureboot_certificates_on_boot(session.opaque_ref, _vm, _mark);
        }

        /// <summary>
        /// Mark or unmark secure boot certificate update on VM boot
        /// Experimental. First published in 26.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_mark">If true: mark certificates for update on next boot. If false: remove the mark</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_update_secureboot_certificates_on_boot(Session session, string _vm, bool _mark)
        {
          return session.JsonRpcClient.async_vm_update_secureboot_certificates_on_boot(session.opaque_ref, _vm, _mark);
        }

        /// <summary>
        /// Update list of operations which have been explicitly blocked and an error code
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">Blocked operations</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_blocked_operations(Session session, string _vm, Dictionary<vm_operations, string> _value)
        {
            session.JsonRpcClient.vm_set_blocked_operations(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Update list of operations which have been explicitly blocked and an error code
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">Blocked operations</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_set_blocked_operations(Session session, string _vm, Dictionary<vm_operations, string> _value)
        {
          return session.JsonRpcClient.async_vm_set_blocked_operations(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Update list of operations which have been explicitly blocked and an error code
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">Blocked operation</param>
        /// <param name="_value">Error code</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void add_to_blocked_operations(Session session, string _vm, vm_operations _key, string _value)
        {
            session.JsonRpcClient.vm_add_to_blocked_operations(session.opaque_ref, _vm, _key, _value);
        }

        /// <summary>
        /// Update list of operations which have been explicitly blocked and an error code
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">Blocked operation</param>
        /// <param name="_value">Error code</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_add_to_blocked_operations(Session session, string _vm, vm_operations _key, string _value)
        {
          return session.JsonRpcClient.async_vm_add_to_blocked_operations(session.opaque_ref, _vm, _key, _value);
        }

        /// <summary>
        /// Update list of operations which have been explicitly blocked and an error code
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">Blocked operation</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void remove_from_blocked_operations(Session session, string _vm, vm_operations _key)
        {
            session.JsonRpcClient.vm_remove_from_blocked_operations(session.opaque_ref, _vm, _key);
        }

        /// <summary>
        /// Update list of operations which have been explicitly blocked and an error code
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">Blocked operation</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_remove_from_blocked_operations(Session session, string _vm, vm_operations _key)
        {
          return session.JsonRpcClient.async_vm_remove_from_blocked_operations(session.opaque_ref, _vm, _key);
        }

        /// <summary>
        /// Pass unattend.xml to Windows sysprep and wait for the VM to shut down as part of a reboot.
        /// Experimental. First published in 25.24.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_unattend">XML content passed to sysprep</param>
        /// <param name="_timeout">timeout in seconds for expected reboot</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void sysprep(Session session, string _vm, string _unattend, double _timeout)
        {
            session.JsonRpcClient.vm_sysprep(session.opaque_ref, _vm, _unattend, _timeout);
        }

        /// <summary>
        /// Pass unattend.xml to Windows sysprep and wait for the VM to shut down as part of a reboot.
        /// Experimental. First published in 25.24.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_unattend">XML content passed to sysprep</param>
        /// <param name="_timeout">timeout in seconds for expected reboot</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_sysprep(Session session, string _vm, string _unattend, double _timeout)
        {
          return session.JsonRpcClient.async_vm_sysprep(session.opaque_ref, _vm, _unattend, _timeout);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void add_to_other_config(Session session, string _vm, string _key, string _value)
        {
            session.JsonRpcClient.vm_add_to_other_config(session.opaque_ref, _vm, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given VM. If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">Key of entry to remove</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void remove_from_other_config(Session session, string _vm, string _key)
        {
            session.JsonRpcClient.vm_remove_from_other_config(session.opaque_ref, _vm, _key);
        }

        /// <summary>
        /// Set the other_config field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_other_config(Session session, string _vm, Dictionary<string, string> _value)
        {
            session.JsonRpcClient.vm_set_other_config(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Add the given key-value pair to the platform field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void add_to_platform(Session session, string _vm, string _key, string _value)
        {
            session.JsonRpcClient.vm_add_to_platform(session.opaque_ref, _vm, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the platform field of the given VM. If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_key">Key of entry to remove</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void remove_from_platform(Session session, string _vm, string _key)
        {
            session.JsonRpcClient.vm_remove_from_platform(session.opaque_ref, _vm, _key);
        }

        /// <summary>
        /// Set the platform field of the given VM.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The opaque_ref of the given vm</param>
        /// <param name="_value">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_platform(Session session, string _vm, Dictionary<string, string> _value)
        {
            session.JsonRpcClient.vm_set_platform(session.opaque_ref, _vm, _value);
        }

        /// <summary>
        /// Return a list of all the VMs known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VM>> get_all(Session session)
        {
            return session.JsonRpcClient.vm_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of VM references to VM records for all VMs known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<VM>, VM> get_all_records(Session session)
        {
            return session.JsonRpcClient.vm_get_all_records(session.opaque_ref);
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
        /// list of the operations allowed in this state. This list is advisory only and the server state may have changed by the time this field is read by a client.
        /// </summary>
        public virtual List<vm_operations> allowed_operations
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
        private List<vm_operations> _allowed_operations = new List<vm_operations>() {};

        /// <summary>
        /// links each of the running tasks using this object (by reference) to a current_operation enum which describes the nature of the task.
        /// </summary>
        public virtual Dictionary<string, vm_operations> current_operations
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
        private Dictionary<string, vm_operations> _current_operations = new Dictionary<string, vm_operations>() {};

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
        /// Current power state of the machine
        /// </summary>
        [JsonConverter(typeof(vm_power_stateConverter))]
        public virtual vm_power_state power_state
        {
            get { return _power_state; }
            set
            {
                if (!Helper.AreEqual(value, _power_state))
                {
                    _power_state = value;
                    NotifyPropertyChanged("power_state");
                }
            }
        }
        private vm_power_state _power_state = vm_power_state.Halted;

        /// <summary>
        /// Creators of VMs and templates may store version information here.
        /// </summary>
        public virtual long user_version
        {
            get { return _user_version; }
            set
            {
                if (!Helper.AreEqual(value, _user_version))
                {
                    _user_version = value;
                    NotifyPropertyChanged("user_version");
                }
            }
        }
        private long _user_version;

        /// <summary>
        /// true if this is a template. Template VMs can never be started, they are used only for cloning other VMs
        /// </summary>
        public virtual bool is_a_template
        {
            get { return _is_a_template; }
            set
            {
                if (!Helper.AreEqual(value, _is_a_template))
                {
                    _is_a_template = value;
                    NotifyPropertyChanged("is_a_template");
                }
            }
        }
        private bool _is_a_template;

        /// <summary>
        /// true if this is a default template. Default template VMs can never be started or migrated, they are used only for cloning other VMs
        /// First published in XenServer 7.2.
        /// </summary>
        public virtual bool is_default_template
        {
            get { return _is_default_template; }
            set
            {
                if (!Helper.AreEqual(value, _is_default_template))
                {
                    _is_default_template = value;
                    NotifyPropertyChanged("is_default_template");
                }
            }
        }
        private bool _is_default_template = false;

        /// <summary>
        /// The VDI that a suspend image is stored on. (Only has meaning if VM is currently suspended)
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VDI>))]
        public virtual XenRef<VDI> suspend_VDI
        {
            get { return _suspend_VDI; }
            set
            {
                if (!Helper.AreEqual(value, _suspend_VDI))
                {
                    _suspend_VDI = value;
                    NotifyPropertyChanged("suspend_VDI");
                }
            }
        }
        private XenRef<VDI> _suspend_VDI = new XenRef<VDI>("OpaqueRef:NULL");

        /// <summary>
        /// the host the VM is currently resident on
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Host>))]
        public virtual XenRef<Host> resident_on
        {
            get { return _resident_on; }
            set
            {
                if (!Helper.AreEqual(value, _resident_on))
                {
                    _resident_on = value;
                    NotifyPropertyChanged("resident_on");
                }
            }
        }
        private XenRef<Host> _resident_on = new XenRef<Host>(Helper.NullOpaqueRef);

        /// <summary>
        /// the host on which the VM is due to be started/resumed/migrated. This acts as a memory reservation indicator
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Host>))]
        public virtual XenRef<Host> scheduled_to_be_resident_on
        {
            get { return _scheduled_to_be_resident_on; }
            set
            {
                if (!Helper.AreEqual(value, _scheduled_to_be_resident_on))
                {
                    _scheduled_to_be_resident_on = value;
                    NotifyPropertyChanged("scheduled_to_be_resident_on");
                }
            }
        }
        private XenRef<Host> _scheduled_to_be_resident_on = new XenRef<Host>("OpaqueRef:NULL");

        /// <summary>
        /// A host which the VM has some affinity for (or NULL). This is used as a hint to the start call when it decides where to run the VM. Resource constraints may cause the VM to be started elsewhere.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Host>))]
        public virtual XenRef<Host> affinity
        {
            get { return _affinity; }
            set
            {
                if (!Helper.AreEqual(value, _affinity))
                {
                    _affinity = value;
                    NotifyPropertyChanged("affinity");
                }
            }
        }
        private XenRef<Host> _affinity = new XenRef<Host>(Helper.NullOpaqueRef);

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
        /// Dynamically-set memory target (bytes). The value of this field indicates the current target for memory available to this VM.
        /// </summary>
        public virtual long memory_target
        {
            get { return _memory_target; }
            set
            {
                if (!Helper.AreEqual(value, _memory_target))
                {
                    _memory_target = value;
                    NotifyPropertyChanged("memory_target");
                }
            }
        }
        private long _memory_target = 0;

        /// <summary>
        /// Statically-set (i.e. absolute) maximum (bytes). The value of this field at VM start time acts as a hard limit of the amount of memory a guest can use. New values only take effect on reboot.
        /// </summary>
        public virtual long memory_static_max
        {
            get { return _memory_static_max; }
            set
            {
                if (!Helper.AreEqual(value, _memory_static_max))
                {
                    _memory_static_max = value;
                    NotifyPropertyChanged("memory_static_max");
                }
            }
        }
        private long _memory_static_max;

        /// <summary>
        /// Dynamic maximum (bytes)
        /// </summary>
        public virtual long memory_dynamic_max
        {
            get { return _memory_dynamic_max; }
            set
            {
                if (!Helper.AreEqual(value, _memory_dynamic_max))
                {
                    _memory_dynamic_max = value;
                    NotifyPropertyChanged("memory_dynamic_max");
                }
            }
        }
        private long _memory_dynamic_max;

        /// <summary>
        /// Dynamic minimum (bytes)
        /// </summary>
        public virtual long memory_dynamic_min
        {
            get { return _memory_dynamic_min; }
            set
            {
                if (!Helper.AreEqual(value, _memory_dynamic_min))
                {
                    _memory_dynamic_min = value;
                    NotifyPropertyChanged("memory_dynamic_min");
                }
            }
        }
        private long _memory_dynamic_min;

        /// <summary>
        /// Statically-set (i.e. absolute) mininum (bytes). The value of this field indicates the least amount of memory this VM can boot with without crashing.
        /// </summary>
        public virtual long memory_static_min
        {
            get { return _memory_static_min; }
            set
            {
                if (!Helper.AreEqual(value, _memory_static_min))
                {
                    _memory_static_min = value;
                    NotifyPropertyChanged("memory_static_min");
                }
            }
        }
        private long _memory_static_min;

        /// <summary>
        /// configuration parameters for the selected VCPU policy
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
                    NotifyPropertyChanged("VCPUs_params");
                }
            }
        }
        private Dictionary<string, string> _VCPUs_params = new Dictionary<string, string>() {};

        /// <summary>
        /// Max number of VCPUs
        /// </summary>
        public virtual long VCPUs_max
        {
            get { return _VCPUs_max; }
            set
            {
                if (!Helper.AreEqual(value, _VCPUs_max))
                {
                    _VCPUs_max = value;
                    NotifyPropertyChanged("VCPUs_max");
                }
            }
        }
        private long _VCPUs_max;

        /// <summary>
        /// Boot number of VCPUs
        /// </summary>
        public virtual long VCPUs_at_startup
        {
            get { return _VCPUs_at_startup; }
            set
            {
                if (!Helper.AreEqual(value, _VCPUs_at_startup))
                {
                    _VCPUs_at_startup = value;
                    NotifyPropertyChanged("VCPUs_at_startup");
                }
            }
        }
        private long _VCPUs_at_startup;

        /// <summary>
        /// action to take after soft reboot
        /// Experimental. First published in 23.1.0.
        /// </summary>
        [JsonConverter(typeof(on_softreboot_behaviorConverter))]
        public virtual on_softreboot_behavior actions_after_softreboot
        {
            get { return _actions_after_softreboot; }
            set
            {
                if (!Helper.AreEqual(value, _actions_after_softreboot))
                {
                    _actions_after_softreboot = value;
                    NotifyPropertyChanged("actions_after_softreboot");
                }
            }
        }
        private on_softreboot_behavior _actions_after_softreboot = on_softreboot_behavior.soft_reboot;

        /// <summary>
        /// action to take after the guest has shutdown itself
        /// </summary>
        [JsonConverter(typeof(on_normal_exitConverter))]
        public virtual on_normal_exit actions_after_shutdown
        {
            get { return _actions_after_shutdown; }
            set
            {
                if (!Helper.AreEqual(value, _actions_after_shutdown))
                {
                    _actions_after_shutdown = value;
                    NotifyPropertyChanged("actions_after_shutdown");
                }
            }
        }
        private on_normal_exit _actions_after_shutdown;

        /// <summary>
        /// action to take after the guest has rebooted itself
        /// </summary>
        [JsonConverter(typeof(on_normal_exitConverter))]
        public virtual on_normal_exit actions_after_reboot
        {
            get { return _actions_after_reboot; }
            set
            {
                if (!Helper.AreEqual(value, _actions_after_reboot))
                {
                    _actions_after_reboot = value;
                    NotifyPropertyChanged("actions_after_reboot");
                }
            }
        }
        private on_normal_exit _actions_after_reboot;

        /// <summary>
        /// action to take if the guest crashes
        /// </summary>
        [JsonConverter(typeof(on_crash_behaviourConverter))]
        public virtual on_crash_behaviour actions_after_crash
        {
            get { return _actions_after_crash; }
            set
            {
                if (!Helper.AreEqual(value, _actions_after_crash))
                {
                    _actions_after_crash = value;
                    NotifyPropertyChanged("actions_after_crash");
                }
            }
        }
        private on_crash_behaviour _actions_after_crash;

        /// <summary>
        /// virtual console devices
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Console>))]
        public virtual List<XenRef<Console>> consoles
        {
            get { return _consoles; }
            set
            {
                if (!Helper.AreEqual(value, _consoles))
                {
                    _consoles = value;
                    NotifyPropertyChanged("consoles");
                }
            }
        }
        private List<XenRef<Console>> _consoles = new List<XenRef<Console>>() {};

        /// <summary>
        /// virtual network interfaces
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VIF>))]
        public virtual List<XenRef<VIF>> VIFs
        {
            get { return _VIFs; }
            set
            {
                if (!Helper.AreEqual(value, _VIFs))
                {
                    _VIFs = value;
                    NotifyPropertyChanged("VIFs");
                }
            }
        }
        private List<XenRef<VIF>> _VIFs = new List<XenRef<VIF>>() {};

        /// <summary>
        /// virtual block devices
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VBD>))]
        public virtual List<XenRef<VBD>> VBDs
        {
            get { return _VBDs; }
            set
            {
                if (!Helper.AreEqual(value, _VBDs))
                {
                    _VBDs = value;
                    NotifyPropertyChanged("VBDs");
                }
            }
        }
        private List<XenRef<VBD>> _VBDs = new List<XenRef<VBD>>() {};

        /// <summary>
        /// virtual usb devices
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VUSB>))]
        public virtual List<XenRef<VUSB>> VUSBs
        {
            get { return _VUSBs; }
            set
            {
                if (!Helper.AreEqual(value, _VUSBs))
                {
                    _VUSBs = value;
                    NotifyPropertyChanged("VUSBs");
                }
            }
        }
        private List<XenRef<VUSB>> _VUSBs = new List<XenRef<VUSB>>() {};

        /// <summary>
        /// crash dumps associated with this VM
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Crashdump>))]
        public virtual List<XenRef<Crashdump>> crash_dumps
        {
            get { return _crash_dumps; }
            set
            {
                if (!Helper.AreEqual(value, _crash_dumps))
                {
                    _crash_dumps = value;
                    NotifyPropertyChanged("crash_dumps");
                }
            }
        }
        private List<XenRef<Crashdump>> _crash_dumps = new List<XenRef<Crashdump>>() {};

        /// <summary>
        /// virtual TPMs
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VTPM>))]
        public virtual List<XenRef<VTPM>> VTPMs
        {
            get { return _VTPMs; }
            set
            {
                if (!Helper.AreEqual(value, _VTPMs))
                {
                    _VTPMs = value;
                    NotifyPropertyChanged("VTPMs");
                }
            }
        }
        private List<XenRef<VTPM>> _VTPMs = new List<XenRef<VTPM>>() {};

        /// <summary>
        /// name of or path to bootloader
        /// </summary>
        public virtual string PV_bootloader
        {
            get { return _PV_bootloader; }
            set
            {
                if (!Helper.AreEqual(value, _PV_bootloader))
                {
                    _PV_bootloader = value;
                    NotifyPropertyChanged("PV_bootloader");
                }
            }
        }
        private string _PV_bootloader = "";

        /// <summary>
        /// path to the kernel
        /// </summary>
        public virtual string PV_kernel
        {
            get { return _PV_kernel; }
            set
            {
                if (!Helper.AreEqual(value, _PV_kernel))
                {
                    _PV_kernel = value;
                    NotifyPropertyChanged("PV_kernel");
                }
            }
        }
        private string _PV_kernel = "";

        /// <summary>
        /// path to the initrd
        /// </summary>
        public virtual string PV_ramdisk
        {
            get { return _PV_ramdisk; }
            set
            {
                if (!Helper.AreEqual(value, _PV_ramdisk))
                {
                    _PV_ramdisk = value;
                    NotifyPropertyChanged("PV_ramdisk");
                }
            }
        }
        private string _PV_ramdisk = "";

        /// <summary>
        /// kernel command-line arguments
        /// </summary>
        public virtual string PV_args
        {
            get { return _PV_args; }
            set
            {
                if (!Helper.AreEqual(value, _PV_args))
                {
                    _PV_args = value;
                    NotifyPropertyChanged("PV_args");
                }
            }
        }
        private string _PV_args = "";

        /// <summary>
        /// miscellaneous arguments for the bootloader
        /// </summary>
        public virtual string PV_bootloader_args
        {
            get { return _PV_bootloader_args; }
            set
            {
                if (!Helper.AreEqual(value, _PV_bootloader_args))
                {
                    _PV_bootloader_args = value;
                    NotifyPropertyChanged("PV_bootloader_args");
                }
            }
        }
        private string _PV_bootloader_args = "";

        /// <summary>
        /// to make Zurich guests boot
        /// </summary>
        public virtual string PV_legacy_args
        {
            get { return _PV_legacy_args; }
            set
            {
                if (!Helper.AreEqual(value, _PV_legacy_args))
                {
                    _PV_legacy_args = value;
                    NotifyPropertyChanged("PV_legacy_args");
                }
            }
        }
        private string _PV_legacy_args = "";

        /// <summary>
        /// HVM boot policy
        /// </summary>
        public virtual string HVM_boot_policy
        {
            get { return _HVM_boot_policy; }
            set
            {
                if (!Helper.AreEqual(value, _HVM_boot_policy))
                {
                    _HVM_boot_policy = value;
                    NotifyPropertyChanged("HVM_boot_policy");
                }
            }
        }
        private string _HVM_boot_policy = "";

        /// <summary>
        /// HVM boot params
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> HVM_boot_params
        {
            get { return _HVM_boot_params; }
            set
            {
                if (!Helper.AreEqual(value, _HVM_boot_params))
                {
                    _HVM_boot_params = value;
                    NotifyPropertyChanged("HVM_boot_params");
                }
            }
        }
        private Dictionary<string, string> _HVM_boot_params = new Dictionary<string, string>() {};

        /// <summary>
        /// multiplier applied to the amount of shadow that will be made available to the guest
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual double HVM_shadow_multiplier
        {
            get { return _HVM_shadow_multiplier; }
            set
            {
                if (!Helper.AreEqual(value, _HVM_shadow_multiplier))
                {
                    _HVM_shadow_multiplier = value;
                    NotifyPropertyChanged("HVM_shadow_multiplier");
                }
            }
        }
        private double _HVM_shadow_multiplier = 1.000;

        /// <summary>
        /// platform-specific configuration
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> platform
        {
            get { return _platform; }
            set
            {
                if (!Helper.AreEqual(value, _platform))
                {
                    _platform = value;
                    NotifyPropertyChanged("platform");
                }
            }
        }
        private Dictionary<string, string> _platform = new Dictionary<string, string>() {};

        /// <summary>
        /// PCI bus path for pass-through devices
        /// </summary>
        public virtual string PCI_bus
        {
            get { return _PCI_bus; }
            set
            {
                if (!Helper.AreEqual(value, _PCI_bus))
                {
                    _PCI_bus = value;
                    NotifyPropertyChanged("PCI_bus");
                }
            }
        }
        private string _PCI_bus = "";

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
        /// domain ID (if available, -1 otherwise)
        /// </summary>
        public virtual long domid
        {
            get { return _domid; }
            set
            {
                if (!Helper.AreEqual(value, _domid))
                {
                    _domid = value;
                    NotifyPropertyChanged("domid");
                }
            }
        }
        private long _domid;

        /// <summary>
        /// Domain architecture (if available, null string otherwise)
        /// </summary>
        public virtual string domarch
        {
            get { return _domarch; }
            set
            {
                if (!Helper.AreEqual(value, _domarch))
                {
                    _domarch = value;
                    NotifyPropertyChanged("domarch");
                }
            }
        }
        private string _domarch = "";

        /// <summary>
        /// describes the CPU flags on which the VM was last booted
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> last_boot_CPU_flags
        {
            get { return _last_boot_CPU_flags; }
            set
            {
                if (!Helper.AreEqual(value, _last_boot_CPU_flags))
                {
                    _last_boot_CPU_flags = value;
                    NotifyPropertyChanged("last_boot_CPU_flags");
                }
            }
        }
        private Dictionary<string, string> _last_boot_CPU_flags = new Dictionary<string, string>() {};

        /// <summary>
        /// true if this is a control domain (domain 0 or a driver domain)
        /// </summary>
        public virtual bool is_control_domain
        {
            get { return _is_control_domain; }
            set
            {
                if (!Helper.AreEqual(value, _is_control_domain))
                {
                    _is_control_domain = value;
                    NotifyPropertyChanged("is_control_domain");
                }
            }
        }
        private bool _is_control_domain;

        /// <summary>
        /// metrics associated with this VM
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VM_metrics>))]
        public virtual XenRef<VM_metrics> metrics
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
        private XenRef<VM_metrics> _metrics = new XenRef<VM_metrics>(Helper.NullOpaqueRef);

        /// <summary>
        /// metrics associated with the running guest
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VM_guest_metrics>))]
        public virtual XenRef<VM_guest_metrics> guest_metrics
        {
            get { return _guest_metrics; }
            set
            {
                if (!Helper.AreEqual(value, _guest_metrics))
                {
                    _guest_metrics = value;
                    NotifyPropertyChanged("guest_metrics");
                }
            }
        }
        private XenRef<VM_guest_metrics> _guest_metrics = new XenRef<VM_guest_metrics>(Helper.NullOpaqueRef);

        /// <summary>
        /// marshalled value containing VM record at time of last boot
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual string last_booted_record
        {
            get { return _last_booted_record; }
            set
            {
                if (!Helper.AreEqual(value, _last_booted_record))
                {
                    _last_booted_record = value;
                    NotifyPropertyChanged("last_booted_record");
                }
            }
        }
        private string _last_booted_record = "";

        /// <summary>
        /// An XML specification of recommended values and ranges for properties of this VM
        /// </summary>
        public virtual string recommendations
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
        private string _recommendations = "";

        /// <summary>
        /// data to be inserted into the xenstore tree (/local/domain/&lt;domid&gt;/vm-data) after the VM is created.
        /// First published in XenServer 4.1.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> xenstore_data
        {
            get { return _xenstore_data; }
            set
            {
                if (!Helper.AreEqual(value, _xenstore_data))
                {
                    _xenstore_data = value;
                    NotifyPropertyChanged("xenstore_data");
                }
            }
        }
        private Dictionary<string, string> _xenstore_data = new Dictionary<string, string>() {};

        /// <summary>
        /// if true then the system will attempt to keep the VM running as much as possible.
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual bool ha_always_run
        {
            get { return _ha_always_run; }
            set
            {
                if (!Helper.AreEqual(value, _ha_always_run))
                {
                    _ha_always_run = value;
                    NotifyPropertyChanged("ha_always_run");
                }
            }
        }
        private bool _ha_always_run = false;

        /// <summary>
        /// has possible values: "best-effort" meaning "try to restart this VM if possible but don't consider the Pool to be overcommitted if this is not possible"; "restart" meaning "this VM should be restarted"; "" meaning "do not try to restart this VM"
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual string ha_restart_priority
        {
            get { return _ha_restart_priority; }
            set
            {
                if (!Helper.AreEqual(value, _ha_restart_priority))
                {
                    _ha_restart_priority = value;
                    NotifyPropertyChanged("ha_restart_priority");
                }
            }
        }
        private string _ha_restart_priority = "";

        /// <summary>
        /// true if this is a snapshot. Snapshotted VMs can never be started, they are used only for cloning other VMs
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual bool is_a_snapshot
        {
            get { return _is_a_snapshot; }
            set
            {
                if (!Helper.AreEqual(value, _is_a_snapshot))
                {
                    _is_a_snapshot = value;
                    NotifyPropertyChanged("is_a_snapshot");
                }
            }
        }
        private bool _is_a_snapshot = false;

        /// <summary>
        /// Ref pointing to the VM this snapshot is of.
        /// First published in XenServer 5.0.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VM>))]
        public virtual XenRef<VM> snapshot_of
        {
            get { return _snapshot_of; }
            set
            {
                if (!Helper.AreEqual(value, _snapshot_of))
                {
                    _snapshot_of = value;
                    NotifyPropertyChanged("snapshot_of");
                }
            }
        }
        private XenRef<VM> _snapshot_of = new XenRef<VM>(Helper.NullOpaqueRef);

        /// <summary>
        /// List pointing to all the VM snapshots.
        /// First published in XenServer 5.0.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VM>))]
        public virtual List<XenRef<VM>> snapshots
        {
            get { return _snapshots; }
            set
            {
                if (!Helper.AreEqual(value, _snapshots))
                {
                    _snapshots = value;
                    NotifyPropertyChanged("snapshots");
                }
            }
        }
        private List<XenRef<VM>> _snapshots = new List<XenRef<VM>>() {};

        /// <summary>
        /// Date/time when this snapshot was created.
        /// First published in XenServer 5.0.
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime snapshot_time
        {
            get { return _snapshot_time; }
            set
            {
                if (!Helper.AreEqual(value, _snapshot_time))
                {
                    _snapshot_time = value;
                    NotifyPropertyChanged("snapshot_time");
                }
            }
        }
        private DateTime _snapshot_time = DateTime.ParseExact("19700101T00:00:00Z", "yyyyMMddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        /// <summary>
        /// Transportable ID of the snapshot VM
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual string transportable_snapshot_id
        {
            get { return _transportable_snapshot_id; }
            set
            {
                if (!Helper.AreEqual(value, _transportable_snapshot_id))
                {
                    _transportable_snapshot_id = value;
                    NotifyPropertyChanged("transportable_snapshot_id");
                }
            }
        }
        private string _transportable_snapshot_id = "";

        /// <summary>
        /// Binary blobs associated with this VM
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
        /// List of operations which have been explicitly blocked and an error code
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual Dictionary<vm_operations, string> blocked_operations
        {
            get { return _blocked_operations; }
            set
            {
                if (!Helper.AreEqual(value, _blocked_operations))
                {
                    _blocked_operations = value;
                    NotifyPropertyChanged("blocked_operations");
                }
            }
        }
        private Dictionary<vm_operations, string> _blocked_operations = new Dictionary<vm_operations, string>() {};

        /// <summary>
        /// Human-readable information concerning this snapshot
        /// First published in XenServer 5.6.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> snapshot_info
        {
            get { return _snapshot_info; }
            set
            {
                if (!Helper.AreEqual(value, _snapshot_info))
                {
                    _snapshot_info = value;
                    NotifyPropertyChanged("snapshot_info");
                }
            }
        }
        private Dictionary<string, string> _snapshot_info = new Dictionary<string, string>() {};

        /// <summary>
        /// Encoded information about the VM's metadata this is a snapshot of
        /// First published in XenServer 5.6.
        /// </summary>
        public virtual string snapshot_metadata
        {
            get { return _snapshot_metadata; }
            set
            {
                if (!Helper.AreEqual(value, _snapshot_metadata))
                {
                    _snapshot_metadata = value;
                    NotifyPropertyChanged("snapshot_metadata");
                }
            }
        }
        private string _snapshot_metadata = "";

        /// <summary>
        /// Ref pointing to the parent of this VM
        /// First published in XenServer 5.6.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VM>))]
        public virtual XenRef<VM> parent
        {
            get { return _parent; }
            set
            {
                if (!Helper.AreEqual(value, _parent))
                {
                    _parent = value;
                    NotifyPropertyChanged("parent");
                }
            }
        }
        private XenRef<VM> _parent = new XenRef<VM>(Helper.NullOpaqueRef);

        /// <summary>
        /// List pointing to all the children of this VM
        /// First published in XenServer 5.6.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VM>))]
        public virtual List<XenRef<VM>> children
        {
            get { return _children; }
            set
            {
                if (!Helper.AreEqual(value, _children))
                {
                    _children = value;
                    NotifyPropertyChanged("children");
                }
            }
        }
        private List<XenRef<VM>> _children = new List<XenRef<VM>>() {};

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
        /// Ref pointing to a protection policy for this VM
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VMPP>))]
        public virtual XenRef<VMPP> protection_policy
        {
            get { return _protection_policy; }
            set
            {
                if (!Helper.AreEqual(value, _protection_policy))
                {
                    _protection_policy = value;
                    NotifyPropertyChanged("protection_policy");
                }
            }
        }
        private XenRef<VMPP> _protection_policy = new XenRef<VMPP>("OpaqueRef:NULL");

        /// <summary>
        /// true if this snapshot was created by the protection policy
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        public virtual bool is_snapshot_from_vmpp
        {
            get { return _is_snapshot_from_vmpp; }
            set
            {
                if (!Helper.AreEqual(value, _is_snapshot_from_vmpp))
                {
                    _is_snapshot_from_vmpp = value;
                    NotifyPropertyChanged("is_snapshot_from_vmpp");
                }
            }
        }
        private bool _is_snapshot_from_vmpp = false;

        /// <summary>
        /// Ref pointing to a snapshot schedule for this VM
        /// First published in XenServer 7.2.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VMSS>))]
        public virtual XenRef<VMSS> snapshot_schedule
        {
            get { return _snapshot_schedule; }
            set
            {
                if (!Helper.AreEqual(value, _snapshot_schedule))
                {
                    _snapshot_schedule = value;
                    NotifyPropertyChanged("snapshot_schedule");
                }
            }
        }
        private XenRef<VMSS> _snapshot_schedule = new XenRef<VMSS>("OpaqueRef:NULL");

        /// <summary>
        /// true if this snapshot was created by the snapshot schedule
        /// First published in XenServer 7.2.
        /// </summary>
        public virtual bool is_vmss_snapshot
        {
            get { return _is_vmss_snapshot; }
            set
            {
                if (!Helper.AreEqual(value, _is_vmss_snapshot))
                {
                    _is_vmss_snapshot = value;
                    NotifyPropertyChanged("is_vmss_snapshot");
                }
            }
        }
        private bool _is_vmss_snapshot = false;

        /// <summary>
        /// the appliance to which this VM belongs
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VM_appliance>))]
        public virtual XenRef<VM_appliance> appliance
        {
            get { return _appliance; }
            set
            {
                if (!Helper.AreEqual(value, _appliance))
                {
                    _appliance = value;
                    NotifyPropertyChanged("appliance");
                }
            }
        }
        private XenRef<VM_appliance> _appliance = new XenRef<VM_appliance>("OpaqueRef:NULL");

        /// <summary>
        /// The delay to wait before proceeding to the next order in the startup sequence (seconds)
        /// First published in XenServer 6.0.
        /// </summary>
        public virtual long start_delay
        {
            get { return _start_delay; }
            set
            {
                if (!Helper.AreEqual(value, _start_delay))
                {
                    _start_delay = value;
                    NotifyPropertyChanged("start_delay");
                }
            }
        }
        private long _start_delay = 0;

        /// <summary>
        /// The delay to wait before proceeding to the next order in the shutdown sequence (seconds)
        /// First published in XenServer 6.0.
        /// </summary>
        public virtual long shutdown_delay
        {
            get { return _shutdown_delay; }
            set
            {
                if (!Helper.AreEqual(value, _shutdown_delay))
                {
                    _shutdown_delay = value;
                    NotifyPropertyChanged("shutdown_delay");
                }
            }
        }
        private long _shutdown_delay = 0;

        /// <summary>
        /// The point in the startup or shutdown sequence at which this VM will be started
        /// First published in XenServer 6.0.
        /// </summary>
        public virtual long order
        {
            get { return _order; }
            set
            {
                if (!Helper.AreEqual(value, _order))
                {
                    _order = value;
                    NotifyPropertyChanged("order");
                }
            }
        }
        private long _order = 0;

        /// <summary>
        /// Virtual GPUs
        /// First published in XenServer 6.0.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VGPU>))]
        public virtual List<XenRef<VGPU>> VGPUs
        {
            get { return _VGPUs; }
            set
            {
                if (!Helper.AreEqual(value, _VGPUs))
                {
                    _VGPUs = value;
                    NotifyPropertyChanged("VGPUs");
                }
            }
        }
        private List<XenRef<VGPU>> _VGPUs = new List<XenRef<VGPU>>() {};

        /// <summary>
        /// Currently passed-through PCI devices
        /// First published in XenServer 6.0.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<PCI>))]
        public virtual List<XenRef<PCI>> attached_PCIs
        {
            get { return _attached_PCIs; }
            set
            {
                if (!Helper.AreEqual(value, _attached_PCIs))
                {
                    _attached_PCIs = value;
                    NotifyPropertyChanged("attached_PCIs");
                }
            }
        }
        private List<XenRef<PCI>> _attached_PCIs = new List<XenRef<PCI>>() {};

        /// <summary>
        /// The SR on which a suspend image is stored
        /// First published in XenServer 6.0.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<SR>))]
        public virtual XenRef<SR> suspend_SR
        {
            get { return _suspend_SR; }
            set
            {
                if (!Helper.AreEqual(value, _suspend_SR))
                {
                    _suspend_SR = value;
                    NotifyPropertyChanged("suspend_SR");
                }
            }
        }
        private XenRef<SR> _suspend_SR = new XenRef<SR>("OpaqueRef:NULL");

        /// <summary>
        /// The number of times this VM has been recovered
        /// First published in XenServer 6.0.
        /// </summary>
        public virtual long version
        {
            get { return _version; }
            set
            {
                if (!Helper.AreEqual(value, _version))
                {
                    _version = value;
                    NotifyPropertyChanged("version");
                }
            }
        }
        private long _version = 0;

        /// <summary>
        /// Generation ID of the VM
        /// First published in XenServer 6.2.
        /// </summary>
        public virtual string generation_id
        {
            get { return _generation_id; }
            set
            {
                if (!Helper.AreEqual(value, _generation_id))
                {
                    _generation_id = value;
                    NotifyPropertyChanged("generation_id");
                }
            }
        }
        private string _generation_id = "0:0";

        /// <summary>
        /// The host virtual hardware platform version the VM can run on
        /// First published in XenServer 6.5 SP1.
        /// </summary>
        public virtual long hardware_platform_version
        {
            get { return _hardware_platform_version; }
            set
            {
                if (!Helper.AreEqual(value, _hardware_platform_version))
                {
                    _hardware_platform_version = value;
                    NotifyPropertyChanged("hardware_platform_version");
                }
            }
        }
        private long _hardware_platform_version = 0;

        /// <summary>
        /// When an HVM guest starts, this controls the presence of the emulated C000 PCI device which triggers Windows Update to fetch or update PV drivers.
        /// First published in XenServer 7.0.
        /// </summary>
        public virtual bool has_vendor_device
        {
            get { return _has_vendor_device; }
            set
            {
                if (!Helper.AreEqual(value, _has_vendor_device))
                {
                    _has_vendor_device = value;
                    NotifyPropertyChanged("has_vendor_device");
                }
            }
        }
        private bool _has_vendor_device = true;

        /// <summary>
        /// Indicates whether a VM requires a reboot in order to update its configuration, for example, its memory allocation.
        /// First published in XenServer 7.1.
        /// </summary>
        public virtual bool requires_reboot
        {
            get { return _requires_reboot; }
            set
            {
                if (!Helper.AreEqual(value, _requires_reboot))
                {
                    _requires_reboot = value;
                    NotifyPropertyChanged("requires_reboot");
                }
            }
        }
        private bool _requires_reboot = false;

        /// <summary>
        /// Textual reference to the template used to create a VM. This can be used by clients in need of an immutable reference to the template since the latter's uuid and name_label may change, for example, after a package installation or upgrade.
        /// First published in XenServer 7.1.
        /// </summary>
        public virtual string reference_label
        {
            get { return _reference_label; }
            set
            {
                if (!Helper.AreEqual(value, _reference_label))
                {
                    _reference_label = value;
                    NotifyPropertyChanged("reference_label");
                }
            }
        }
        private string _reference_label = "";

        /// <summary>
        /// The type of domain that will be created when the VM is started
        /// First published in XenServer 7.5.
        /// </summary>
        [JsonConverter(typeof(domain_typeConverter))]
        public virtual domain_type domain_type
        {
            get { return _domain_type; }
            set
            {
                if (!Helper.AreEqual(value, _domain_type))
                {
                    _domain_type = value;
                    NotifyPropertyChanged("domain_type");
                }
            }
        }
        private domain_type _domain_type = domain_type.unspecified;

        /// <summary>
        /// initial value for guest NVRAM (containing UEFI variables, etc). Cannot be changed while the VM is running
        /// First published in Citrix Hypervisor 8.0.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> NVRAM
        {
            get { return _NVRAM; }
            set
            {
                if (!Helper.AreEqual(value, _NVRAM))
                {
                    _NVRAM = value;
                    NotifyPropertyChanged("NVRAM");
                }
            }
        }
        private Dictionary<string, string> _NVRAM = new Dictionary<string, string>() {};

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
        /// VM groups associated with the VM
        /// Experimental. First published in 24.19.1.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VM_group>))]
        public virtual List<XenRef<VM_group>> groups
        {
            get { return _groups; }
            set
            {
                if (!Helper.AreEqual(value, _groups))
                {
                    _groups = value;
                    NotifyPropertyChanged("groups");
                }
            }
        }
        private List<XenRef<VM_group>> _groups = new List<XenRef<VM_group>>() {};

        /// <summary>
        /// The state of the Secure Boot certificates, showing whether an update is available, already scheduled, or not needed.
        /// Experimental. First published in 26.14.0.
        /// </summary>
        [JsonConverter(typeof(vm_secureboot_certificates_stateConverter))]
        public virtual vm_secureboot_certificates_state secureboot_certificates_state
        {
            get { return _secureboot_certificates_state; }
            set
            {
                if (!Helper.AreEqual(value, _secureboot_certificates_state))
                {
                    _secureboot_certificates_state = value;
                    NotifyPropertyChanged("secureboot_certificates_state");
                }
            }
        }
        private vm_secureboot_certificates_state _secureboot_certificates_state = vm_secureboot_certificates_state.ok;
    }
}
