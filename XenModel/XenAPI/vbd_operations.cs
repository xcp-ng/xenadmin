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
    [JsonConverter(typeof(vbd_operationsConverter))]
    public enum vbd_operations
    {
        /// <summary>
        /// Attempting to attach this VBD to a VM
        /// </summary>
        attach,
        /// <summary>
        /// Attempting to eject the media from this VBD
        /// </summary>
        eject,
        /// <summary>
        /// Attempting to insert new media into this VBD
        /// </summary>
        insert,
        /// <summary>
        /// Attempting to hotplug this VBD
        /// </summary>
        plug,
        /// <summary>
        /// Attempting to hot unplug this VBD
        /// </summary>
        unplug,
        /// <summary>
        /// Attempting to forcibly unplug this VBD
        /// </summary>
        unplug_force,
        /// <summary>
        /// Attempting to pause a block device backend
        /// </summary>
        pause,
        /// <summary>
        /// Attempting to unpause a block device backend
        /// </summary>
        unpause,
        unknown
    }

    public static class vbd_operations_helper
    {
        public static string ToString(vbd_operations x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this vbd_operations x)
        {
            switch (x)
            {
                case vbd_operations.attach:
                    return "attach";
                case vbd_operations.eject:
                    return "eject";
                case vbd_operations.insert:
                    return "insert";
                case vbd_operations.plug:
                    return "plug";
                case vbd_operations.unplug:
                    return "unplug";
                case vbd_operations.unplug_force:
                    return "unplug_force";
                case vbd_operations.pause:
                    return "pause";
                case vbd_operations.unpause:
                    return "unpause";
                default:
                    return "unknown";
            }
        }
    }

    internal class vbd_operationsConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((vbd_operations)value).StringOf());
        }
    }
}