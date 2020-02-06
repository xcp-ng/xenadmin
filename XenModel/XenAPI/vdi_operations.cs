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
    [JsonConverter(typeof(vdi_operationsConverter))]
    public enum vdi_operations
    {
        /// <summary>
        /// Cloning the VDI
        /// </summary>
        clone,
        /// <summary>
        /// Copying the VDI
        /// </summary>
        copy,
        /// <summary>
        /// Resizing the VDI
        /// </summary>
        resize,
        /// <summary>
        /// Resizing the VDI which may or may not be online
        /// </summary>
        resize_online,
        /// <summary>
        /// Snapshotting the VDI
        /// </summary>
        snapshot,
        /// <summary>
        /// Mirroring the VDI
        /// </summary>
        mirror,
        /// <summary>
        /// Destroying the VDI
        /// </summary>
        destroy,
        /// <summary>
        /// Forget about the VDI
        /// </summary>
        forget,
        /// <summary>
        /// Refreshing the fields of the VDI
        /// </summary>
        update,
        /// <summary>
        /// Forcibly unlocking the VDI
        /// </summary>
        force_unlock,
        /// <summary>
        /// Generating static configuration
        /// </summary>
        generate_config,
        /// <summary>
        /// Enabling changed block tracking for a VDI
        /// </summary>
        enable_cbt,
        /// <summary>
        /// Disabling changed block tracking for a VDI
        /// </summary>
        disable_cbt,
        /// <summary>
        /// Deleting the data of the VDI
        /// </summary>
        data_destroy,
        /// <summary>
        /// Exporting a bitmap that shows the changed blocks between two VDIs
        /// </summary>
        list_changed_blocks,
        /// <summary>
        /// Setting the on_boot field of the VDI
        /// </summary>
        set_on_boot,
        /// <summary>
        /// Operations on this VDI are temporarily blocked
        /// </summary>
        blocked,
        unknown
    }

    public static class vdi_operations_helper
    {
        public static string ToString(vdi_operations x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this vdi_operations x)
        {
            switch (x)
            {
                case vdi_operations.clone:
                    return "clone";
                case vdi_operations.copy:
                    return "copy";
                case vdi_operations.resize:
                    return "resize";
                case vdi_operations.resize_online:
                    return "resize_online";
                case vdi_operations.snapshot:
                    return "snapshot";
                case vdi_operations.mirror:
                    return "mirror";
                case vdi_operations.destroy:
                    return "destroy";
                case vdi_operations.forget:
                    return "forget";
                case vdi_operations.update:
                    return "update";
                case vdi_operations.force_unlock:
                    return "force_unlock";
                case vdi_operations.generate_config:
                    return "generate_config";
                case vdi_operations.enable_cbt:
                    return "enable_cbt";
                case vdi_operations.disable_cbt:
                    return "disable_cbt";
                case vdi_operations.data_destroy:
                    return "data_destroy";
                case vdi_operations.list_changed_blocks:
                    return "list_changed_blocks";
                case vdi_operations.set_on_boot:
                    return "set_on_boot";
                case vdi_operations.blocked:
                    return "blocked";
                default:
                    return "unknown";
            }
        }
    }

    internal class vdi_operationsConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((vdi_operations)value).StringOf());
        }
    }
}