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

using Newtonsoft.Json;


namespace XenAPI
{
    [JsonConverter(typeof(vm_operationsConverter))]
    public enum vm_operations
    {
        /// <summary>
        /// refers to the operation &quot;snapshot&quot;
        /// </summary>
        snapshot,
        /// <summary>
        /// refers to the operation &quot;clone&quot;
        /// </summary>
        clone,
        /// <summary>
        /// refers to the operation &quot;copy&quot;
        /// </summary>
        copy,
        /// <summary>
        /// refers to the operation &quot;create_template&quot;
        /// </summary>
        create_template,
        /// <summary>
        /// refers to the operation &quot;revert&quot;
        /// </summary>
        revert,
        /// <summary>
        /// refers to the operation &quot;checkpoint&quot;
        /// </summary>
        checkpoint,
        /// <summary>
        /// refers to the operation &quot;snapshot_with_quiesce&quot;
        /// </summary>
        snapshot_with_quiesce,
        /// <summary>
        /// refers to the operation &quot;provision&quot;
        /// </summary>
        provision,
        /// <summary>
        /// refers to the operation &quot;start&quot;
        /// </summary>
        start,
        /// <summary>
        /// refers to the operation &quot;start_on&quot;
        /// </summary>
        start_on,
        /// <summary>
        /// refers to the operation &quot;pause&quot;
        /// </summary>
        pause,
        /// <summary>
        /// refers to the operation &quot;unpause&quot;
        /// </summary>
        unpause,
        /// <summary>
        /// refers to the operation &quot;clean_shutdown&quot;
        /// </summary>
        clean_shutdown,
        /// <summary>
        /// refers to the operation &quot;clean_reboot&quot;
        /// </summary>
        clean_reboot,
        /// <summary>
        /// refers to the operation &quot;hard_shutdown&quot;
        /// </summary>
        hard_shutdown,
        /// <summary>
        /// refers to the operation &quot;power_state_reset&quot;
        /// </summary>
        power_state_reset,
        /// <summary>
        /// refers to the operation &quot;hard_reboot&quot;
        /// </summary>
        hard_reboot,
        /// <summary>
        /// refers to the operation &quot;suspend&quot;
        /// </summary>
        suspend,
        /// <summary>
        /// refers to the operation &quot;csvm&quot;
        /// </summary>
        csvm,
        /// <summary>
        /// refers to the operation &quot;resume&quot;
        /// </summary>
        resume,
        /// <summary>
        /// refers to the operation &quot;resume_on&quot;
        /// </summary>
        resume_on,
        /// <summary>
        /// refers to the operation &quot;pool_migrate&quot;
        /// </summary>
        pool_migrate,
        /// <summary>
        /// refers to the operation &quot;migrate_send&quot;
        /// </summary>
        migrate_send,
        /// <summary>
        /// refers to the operation &quot;get_boot_record&quot;
        /// </summary>
        get_boot_record,
        /// <summary>
        /// refers to the operation &quot;send_sysrq&quot;
        /// </summary>
        send_sysrq,
        /// <summary>
        /// refers to the operation &quot;send_trigger&quot;
        /// </summary>
        send_trigger,
        /// <summary>
        /// refers to the operation &quot;query_services&quot;
        /// </summary>
        query_services,
        /// <summary>
        /// refers to the operation &quot;shutdown&quot;
        /// </summary>
        shutdown,
        /// <summary>
        /// refers to the operation &quot;call_plugin&quot;
        /// </summary>
        call_plugin,
        /// <summary>
        /// Changing the memory settings
        /// </summary>
        changing_memory_live,
        /// <summary>
        /// Waiting for the memory settings to change
        /// </summary>
        awaiting_memory_live,
        /// <summary>
        /// Changing the memory dynamic range
        /// </summary>
        changing_dynamic_range,
        /// <summary>
        /// Changing the memory static range
        /// </summary>
        changing_static_range,
        /// <summary>
        /// Changing the memory limits
        /// </summary>
        changing_memory_limits,
        /// <summary>
        /// Changing the shadow memory for a halted VM.
        /// </summary>
        changing_shadow_memory,
        /// <summary>
        /// Changing the shadow memory for a running VM.
        /// </summary>
        changing_shadow_memory_live,
        /// <summary>
        /// Changing VCPU settings for a halted VM.
        /// </summary>
        changing_VCPUs,
        /// <summary>
        /// Changing VCPU settings for a running VM.
        /// </summary>
        changing_VCPUs_live,
        /// <summary>
        /// Changing NVRAM for a halted VM.
        /// </summary>
        changing_NVRAM,
        /// <summary>
        /// 
        /// </summary>
        assert_operation_valid,
        /// <summary>
        /// Add, remove, query or list data sources
        /// </summary>
        data_source_op,
        /// <summary>
        /// 
        /// </summary>
        update_allowed_operations,
        /// <summary>
        /// Turning this VM into a template
        /// </summary>
        make_into_template,
        /// <summary>
        /// importing a VM from a network stream
        /// </summary>
        import,
        /// <summary>
        /// exporting a VM to a network stream
        /// </summary>
        export,
        /// <summary>
        /// exporting VM metadata to a network stream
        /// </summary>
        metadata_export,
        /// <summary>
        /// Reverting the VM to a previous snapshotted state
        /// </summary>
        reverting,
        /// <summary>
        /// refers to the act of uninstalling the VM
        /// </summary>
        destroy,
        unknown
    }

    public static class vm_operations_helper
    {
        public static string ToString(vm_operations x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this vm_operations x)
        {
            switch (x)
            {
                case vm_operations.snapshot:
                    return "snapshot";
                case vm_operations.clone:
                    return "clone";
                case vm_operations.copy:
                    return "copy";
                case vm_operations.create_template:
                    return "create_template";
                case vm_operations.revert:
                    return "revert";
                case vm_operations.checkpoint:
                    return "checkpoint";
                case vm_operations.snapshot_with_quiesce:
                    return "snapshot_with_quiesce";
                case vm_operations.provision:
                    return "provision";
                case vm_operations.start:
                    return "start";
                case vm_operations.start_on:
                    return "start_on";
                case vm_operations.pause:
                    return "pause";
                case vm_operations.unpause:
                    return "unpause";
                case vm_operations.clean_shutdown:
                    return "clean_shutdown";
                case vm_operations.clean_reboot:
                    return "clean_reboot";
                case vm_operations.hard_shutdown:
                    return "hard_shutdown";
                case vm_operations.power_state_reset:
                    return "power_state_reset";
                case vm_operations.hard_reboot:
                    return "hard_reboot";
                case vm_operations.suspend:
                    return "suspend";
                case vm_operations.csvm:
                    return "csvm";
                case vm_operations.resume:
                    return "resume";
                case vm_operations.resume_on:
                    return "resume_on";
                case vm_operations.pool_migrate:
                    return "pool_migrate";
                case vm_operations.migrate_send:
                    return "migrate_send";
                case vm_operations.get_boot_record:
                    return "get_boot_record";
                case vm_operations.send_sysrq:
                    return "send_sysrq";
                case vm_operations.send_trigger:
                    return "send_trigger";
                case vm_operations.query_services:
                    return "query_services";
                case vm_operations.shutdown:
                    return "shutdown";
                case vm_operations.call_plugin:
                    return "call_plugin";
                case vm_operations.changing_memory_live:
                    return "changing_memory_live";
                case vm_operations.awaiting_memory_live:
                    return "awaiting_memory_live";
                case vm_operations.changing_dynamic_range:
                    return "changing_dynamic_range";
                case vm_operations.changing_static_range:
                    return "changing_static_range";
                case vm_operations.changing_memory_limits:
                    return "changing_memory_limits";
                case vm_operations.changing_shadow_memory:
                    return "changing_shadow_memory";
                case vm_operations.changing_shadow_memory_live:
                    return "changing_shadow_memory_live";
                case vm_operations.changing_VCPUs:
                    return "changing_VCPUs";
                case vm_operations.changing_VCPUs_live:
                    return "changing_VCPUs_live";
                case vm_operations.changing_NVRAM:
                    return "changing_NVRAM";
                case vm_operations.assert_operation_valid:
                    return "assert_operation_valid";
                case vm_operations.data_source_op:
                    return "data_source_op";
                case vm_operations.update_allowed_operations:
                    return "update_allowed_operations";
                case vm_operations.make_into_template:
                    return "make_into_template";
                case vm_operations.import:
                    return "import";
                case vm_operations.export:
                    return "export";
                case vm_operations.metadata_export:
                    return "metadata_export";
                case vm_operations.reverting:
                    return "reverting";
                case vm_operations.destroy:
                    return "destroy";
                default:
                    return "unknown";
            }
        }
    }

    internal class vm_operationsConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((vm_operations)value).StringOf());
        }
    }
}