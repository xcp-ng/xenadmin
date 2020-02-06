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
    [JsonConverter(typeof(host_displayConverter))]
    public enum host_display
    {
        /// <summary>
        /// This host is outputting its console to a physical display device
        /// </summary>
        enabled,
        /// <summary>
        /// The host will stop outputting its console to a physical display device on next boot
        /// </summary>
        disable_on_reboot,
        /// <summary>
        /// This host is not outputting its console to a physical display device
        /// </summary>
        disabled,
        /// <summary>
        /// The host will start outputting its console to a physical display device on next boot
        /// </summary>
        enable_on_reboot,
        unknown
    }

    public static class host_display_helper
    {
        public static string ToString(host_display x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this host_display x)
        {
            switch (x)
            {
                case host_display.enabled:
                    return "enabled";
                case host_display.disable_on_reboot:
                    return "disable_on_reboot";
                case host_display.disabled:
                    return "disabled";
                case host_display.enable_on_reboot:
                    return "enable_on_reboot";
                default:
                    return "unknown";
            }
        }
    }

    internal class host_displayConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((host_display)value).StringOf());
        }
    }
}