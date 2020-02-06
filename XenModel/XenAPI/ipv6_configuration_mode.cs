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
    [JsonConverter(typeof(ipv6_configuration_modeConverter))]
    public enum ipv6_configuration_mode
    {
        /// <summary>
        /// Do not acquire an IPv6 address
        /// </summary>
        None,
        /// <summary>
        /// Acquire an IPv6 address by DHCP
        /// </summary>
        DHCP,
        /// <summary>
        /// Static IPv6 address configuration
        /// </summary>
        Static,
        /// <summary>
        /// Router assigned prefix delegation IPv6 allocation
        /// </summary>
        Autoconf,
        unknown
    }

    public static class ipv6_configuration_mode_helper
    {
        public static string ToString(ipv6_configuration_mode x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this ipv6_configuration_mode x)
        {
            switch (x)
            {
                case ipv6_configuration_mode.None:
                    return "None";
                case ipv6_configuration_mode.DHCP:
                    return "DHCP";
                case ipv6_configuration_mode.Static:
                    return "Static";
                case ipv6_configuration_mode.Autoconf:
                    return "Autoconf";
                default:
                    return "unknown";
            }
        }
    }

    internal class ipv6_configuration_modeConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((ipv6_configuration_mode)value).StringOf());
        }
    }
}