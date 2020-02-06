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
    [JsonConverter(typeof(host_allowed_operationsConverter))]
    public enum host_allowed_operations
    {
        /// <summary>
        /// Indicates this host is able to provision another VM
        /// </summary>
        provision,
        /// <summary>
        /// Indicates this host is evacuating
        /// </summary>
        evacuate,
        /// <summary>
        /// Indicates this host is in the process of shutting itself down
        /// </summary>
        shutdown,
        /// <summary>
        /// Indicates this host is in the process of rebooting
        /// </summary>
        reboot,
        /// <summary>
        /// Indicates this host is in the process of being powered on
        /// </summary>
        power_on,
        /// <summary>
        /// This host is starting a VM
        /// </summary>
        vm_start,
        /// <summary>
        /// This host is resuming a VM
        /// </summary>
        vm_resume,
        /// <summary>
        /// This host is the migration target of a VM
        /// </summary>
        vm_migrate,
        unknown
    }

    public static class host_allowed_operations_helper
    {
        public static string ToString(host_allowed_operations x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this host_allowed_operations x)
        {
            switch (x)
            {
                case host_allowed_operations.provision:
                    return "provision";
                case host_allowed_operations.evacuate:
                    return "evacuate";
                case host_allowed_operations.shutdown:
                    return "shutdown";
                case host_allowed_operations.reboot:
                    return "reboot";
                case host_allowed_operations.power_on:
                    return "power_on";
                case host_allowed_operations.vm_start:
                    return "vm_start";
                case host_allowed_operations.vm_resume:
                    return "vm_resume";
                case host_allowed_operations.vm_migrate:
                    return "vm_migrate";
                default:
                    return "unknown";
            }
        }
    }

    internal class host_allowed_operationsConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((host_allowed_operations)value).StringOf());
        }
    }
}