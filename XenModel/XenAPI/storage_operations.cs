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
    [JsonConverter(typeof(storage_operationsConverter))]
    public enum storage_operations
    {
        /// <summary>
        /// Scanning backends for new or deleted VDIs
        /// </summary>
        scan,
        /// <summary>
        /// Destroying the SR
        /// </summary>
        destroy,
        /// <summary>
        /// Forgetting about SR
        /// </summary>
        forget,
        /// <summary>
        /// Plugging a PBD into this SR
        /// </summary>
        plug,
        /// <summary>
        /// Unplugging a PBD from this SR
        /// </summary>
        unplug,
        /// <summary>
        /// Refresh the fields on the SR
        /// </summary>
        update,
        /// <summary>
        /// Creating a new VDI
        /// </summary>
        vdi_create,
        /// <summary>
        /// Introducing a new VDI
        /// </summary>
        vdi_introduce,
        /// <summary>
        /// Destroying a VDI
        /// </summary>
        vdi_destroy,
        /// <summary>
        /// Resizing a VDI
        /// </summary>
        vdi_resize,
        /// <summary>
        /// Cloneing a VDI
        /// </summary>
        vdi_clone,
        /// <summary>
        /// Snapshotting a VDI
        /// </summary>
        vdi_snapshot,
        /// <summary>
        /// Mirroring a VDI
        /// </summary>
        vdi_mirror,
        /// <summary>
        /// Enabling changed block tracking for a VDI
        /// </summary>
        vdi_enable_cbt,
        /// <summary>
        /// Disabling changed block tracking for a VDI
        /// </summary>
        vdi_disable_cbt,
        /// <summary>
        /// Deleting the data of the VDI
        /// </summary>
        vdi_data_destroy,
        /// <summary>
        /// Exporting a bitmap that shows the changed blocks between two VDIs
        /// </summary>
        vdi_list_changed_blocks,
        /// <summary>
        /// Setting the on_boot field of the VDI
        /// </summary>
        vdi_set_on_boot,
        /// <summary>
        /// Creating a PBD for this SR
        /// </summary>
        pbd_create,
        /// <summary>
        /// Destroying one of this SR&apos;s PBDs
        /// </summary>
        pbd_destroy,
        unknown
    }

    public static class storage_operations_helper
    {
        public static string ToString(storage_operations x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this storage_operations x)
        {
            switch (x)
            {
                case storage_operations.scan:
                    return "scan";
                case storage_operations.destroy:
                    return "destroy";
                case storage_operations.forget:
                    return "forget";
                case storage_operations.plug:
                    return "plug";
                case storage_operations.unplug:
                    return "unplug";
                case storage_operations.update:
                    return "update";
                case storage_operations.vdi_create:
                    return "vdi_create";
                case storage_operations.vdi_introduce:
                    return "vdi_introduce";
                case storage_operations.vdi_destroy:
                    return "vdi_destroy";
                case storage_operations.vdi_resize:
                    return "vdi_resize";
                case storage_operations.vdi_clone:
                    return "vdi_clone";
                case storage_operations.vdi_snapshot:
                    return "vdi_snapshot";
                case storage_operations.vdi_mirror:
                    return "vdi_mirror";
                case storage_operations.vdi_enable_cbt:
                    return "vdi_enable_cbt";
                case storage_operations.vdi_disable_cbt:
                    return "vdi_disable_cbt";
                case storage_operations.vdi_data_destroy:
                    return "vdi_data_destroy";
                case storage_operations.vdi_list_changed_blocks:
                    return "vdi_list_changed_blocks";
                case storage_operations.vdi_set_on_boot:
                    return "vdi_set_on_boot";
                case storage_operations.pbd_create:
                    return "pbd_create";
                case storage_operations.pbd_destroy:
                    return "pbd_destroy";
                default:
                    return "unknown";
            }
        }
    }

    internal class storage_operationsConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((storage_operations)value).StringOf());
        }
    }
}