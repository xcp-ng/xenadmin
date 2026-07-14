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

using Newtonsoft.Json;


namespace XenAPI
{
    [JsonConverter(typeof(update_guidancesConverter))]
    public enum update_guidances
    {
        /// <summary>
        /// Indicates the updated host should reboot as soon as possible
        /// </summary>
        reboot_host,
        /// <summary>
        /// Indicates the updated host should reboot as soon as possible since one or more livepatch(es) failed to be applied.
        /// </summary>
        reboot_host_on_livepatch_failure,
        /// <summary>
        /// Indicates the updated host should reboot as soon as possible since one or more kernel livepatch(es) failed to be applied.
        /// </summary>
        reboot_host_on_kernel_livepatch_failure,
        /// <summary>
        /// Indicates the updated host should reboot as soon as possible since one or more xen livepatch(es) failed to be applied.
        /// </summary>
        reboot_host_on_xen_livepatch_failure,
        /// <summary>
        /// Indicates the Toolstack running on the updated host should restart as soon as possible
        /// </summary>
        restart_toolstack,
        /// <summary>
        /// Indicates the device model of a running VM should restart as soon as possible
        /// </summary>
        restart_device_model,
        /// <summary>
        /// Indicates the VM should restart as soon as possible
        /// </summary>
        restart_vm,
        unknown
    }

    public static class update_guidances_helper
    {
        public static string ToString(update_guidances x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this update_guidances x)
        {
            switch (x)
            {
                case update_guidances.reboot_host:
                    return "reboot_host";
                case update_guidances.reboot_host_on_livepatch_failure:
                    return "reboot_host_on_livepatch_failure";
                case update_guidances.reboot_host_on_kernel_livepatch_failure:
                    return "reboot_host_on_kernel_livepatch_failure";
                case update_guidances.reboot_host_on_xen_livepatch_failure:
                    return "reboot_host_on_xen_livepatch_failure";
                case update_guidances.restart_toolstack:
                    return "restart_toolstack";
                case update_guidances.restart_device_model:
                    return "restart_device_model";
                case update_guidances.restart_vm:
                    return "restart_vm";
                default:
                    return "unknown";
            }
        }
    }

    internal class update_guidancesConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((update_guidances)value).StringOf());
        }
    }
}