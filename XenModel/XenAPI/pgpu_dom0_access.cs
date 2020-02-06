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
    [JsonConverter(typeof(pgpu_dom0_accessConverter))]
    public enum pgpu_dom0_access
    {
        /// <summary>
        /// dom0 can access this device as normal
        /// </summary>
        enabled,
        /// <summary>
        /// On host reboot dom0 will be blocked from accessing this device
        /// </summary>
        disable_on_reboot,
        /// <summary>
        /// dom0 cannot access this device
        /// </summary>
        disabled,
        /// <summary>
        /// On host reboot dom0 will be allowed to access this device
        /// </summary>
        enable_on_reboot,
        unknown
    }

    public static class pgpu_dom0_access_helper
    {
        public static string ToString(pgpu_dom0_access x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this pgpu_dom0_access x)
        {
            switch (x)
            {
                case pgpu_dom0_access.enabled:
                    return "enabled";
                case pgpu_dom0_access.disable_on_reboot:
                    return "disable_on_reboot";
                case pgpu_dom0_access.disabled:
                    return "disabled";
                case pgpu_dom0_access.enable_on_reboot:
                    return "enable_on_reboot";
                default:
                    return "unknown";
            }
        }
    }

    internal class pgpu_dom0_accessConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((pgpu_dom0_access)value).StringOf());
        }
    }
}