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
using System.Collections.Generic;

namespace XenAPI
{
    public partial class Relation
    {
        public readonly String field;
        public readonly String manyType;
        public readonly String manyField;

        public Relation(String field, String manyType, String manyField)
        {
            this.field = field;
            this.manyField = manyField;
            this.manyType = manyType;
        }

        public static Dictionary<Type, Relation[]> GetRelations()
        {
            Dictionary<Type, Relation[]> relations = new Dictionary<Type, Relation[]>();

            relations.Add(typeof(Pool), new Relation[] {
                new Relation("metadata_VDIs", "VDI", "metadata_of_pool"),
            });

            relations.Add(typeof(SR), new Relation[] {
                new Relation("VDIs", "VDI", "SR"),
                new Relation("PBDs", "PBD", "SR"),
            });

            relations.Add(typeof(GPU_group), new Relation[] {
                new Relation("VGPUs", "VGPU", "GPU_group"),
                new Relation("PGPUs", "PGPU", "GPU_group"),
            });

            relations.Add(typeof(USB_group), new Relation[] {
                new Relation("VUSBs", "VUSB", "USB_group"),
                new Relation("PUSBs", "PUSB", "USB_group"),
            });

            relations.Add(typeof(Subject), new Relation[] {
                new Relation("roles", "subject", "roles"),
            });

            relations.Add(typeof(Cluster), new Relation[] {
                new Relation("cluster_hosts", "Cluster_host", "cluster"),
            });

            relations.Add(typeof(Bond), new Relation[] {
                new Relation("slaves", "PIF", "bond_slave_of"),
            });

            relations.Add(typeof(Role), new Relation[] {
                new Relation("subroles", "role", "subroles"),
            });

            relations.Add(typeof(Pool_patch), new Relation[] {
                new Relation("host_patches", "host_patch", "pool_patch"),
            });

            relations.Add(typeof(Host_driver), new Relation[] {
                new Relation("variants", "Driver_variant", "driver"),
            });

            relations.Add(typeof(VM_appliance), new Relation[] {
                new Relation("VMs", "VM", "appliance"),
            });

            relations.Add(typeof(PIF), new Relation[] {
                new Relation("sriov_logical_PIF_of", "network_sriov", "logical_PIF"),
                new Relation("sriov_physical_PIF_of", "network_sriov", "physical_PIF"),
                new Relation("tunnel_transport_PIF_of", "tunnel", "transport_PIF"),
                new Relation("tunnel_access_PIF_of", "tunnel", "access_PIF"),
                new Relation("VLAN_slave_of", "VLAN", "tagged_PIF"),
                new Relation("bond_master_of", "Bond", "master"),
            });

            relations.Add(typeof(PVS_site), new Relation[] {
                new Relation("cache_storage", "PVS_cache_storage", "site"),
                new Relation("proxies", "PVS_proxy", "site"),
                new Relation("servers", "PVS_server", "site"),
            });

            relations.Add(typeof(DR_task), new Relation[] {
                new Relation("introduced_SRs", "SR", "introduced_by"),
            });

            relations.Add(typeof(Network), new Relation[] {
                new Relation("PIFs", "PIF", "network"),
                new Relation("VIFs", "VIF", "network"),
            });

            relations.Add(typeof(Task), new Relation[] {
                new Relation("subtasks", "task", "subtask_of"),
            });

            relations.Add(typeof(PGPU), new Relation[] {
                new Relation("resident_VGPUs", "VGPU", "resident_on"),
            });

            relations.Add(typeof(VGPU_type), new Relation[] {
                new Relation("enabled_on_GPU_groups", "GPU_group", "enabled_VGPU_types"),
                new Relation("supported_on_GPU_groups", "GPU_group", "supported_VGPU_types"),
                new Relation("enabled_on_PGPUs", "PGPU", "enabled_VGPU_types"),
                new Relation("supported_on_PGPUs", "PGPU", "supported_VGPU_types"),
                new Relation("VGPUs", "VGPU", "type"),
            });

            relations.Add(typeof(VDI), new Relation[] {
                new Relation("crash_dumps", "crashdump", "VDI"),
                new Relation("VBDs", "VBD", "VDI"),
                new Relation("snapshots", "VDI", "snapshot_of"),
            });

            relations.Add(typeof(VM_group), new Relation[] {
                new Relation("VMs", "VM", "groups"),
            });

            relations.Add(typeof(VMPP), new Relation[] {
                new Relation("VMs", "VM", "protection_policy"),
            });

            relations.Add(typeof(VM), new Relation[] {
                new Relation("VUSBs", "VUSB", "VM"),
                new Relation("VGPUs", "VGPU", "VM"),
                new Relation("consoles", "console", "VM"),
                new Relation("VTPMs", "VTPM", "VM"),
                new Relation("VIFs", "VIF", "VM"),
                new Relation("crash_dumps", "crashdump", "VM"),
                new Relation("VBDs", "VBD", "VM"),
                new Relation("children", "VM", "parent"),
                new Relation("snapshots", "VM", "snapshot_of"),
            });

            relations.Add(typeof(Pool_update), new Relation[] {
                new Relation("hosts", "host", "updates"),
            });

            relations.Add(typeof(Host), new Relation[] {
                new Relation("certificates", "Certificate", "host"),
                new Relation("features", "Feature", "host"),
                new Relation("PUSBs", "PUSB", "host"),
                new Relation("PGPUs", "PGPU", "host"),
                new Relation("PCIs", "PCI", "host"),
                new Relation("patches", "host_patch", "host"),
                new Relation("crashdumps", "host_crashdump", "host"),
                new Relation("host_CPUs", "host_cpu", "host"),
                new Relation("resident_VMs", "VM", "resident_on"),
                new Relation("PIFs", "PIF", "host"),
                new Relation("PBDs", "PBD", "host"),
            });

            relations.Add(typeof(VMSS), new Relation[] {
                new Relation("VMs", "VM", "snapshot_schedule"),
            });

            return relations;
       }
    }
}