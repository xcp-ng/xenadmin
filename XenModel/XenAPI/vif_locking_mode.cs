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
    [JsonConverter(typeof(vif_locking_modeConverter))]
    public enum vif_locking_mode
    {
        /// <summary>
        /// No specific configuration set - default network policy applies
        /// </summary>
        network_default,
        /// <summary>
        /// Only traffic to a specific MAC and a list of IPv4 or IPv6 addresses is permitted
        /// </summary>
        locked,
        /// <summary>
        /// All traffic is permitted
        /// </summary>
        unlocked,
        /// <summary>
        /// No traffic is permitted
        /// </summary>
        disabled,
        unknown
    }

    public static class vif_locking_mode_helper
    {
        public static string ToString(vif_locking_mode x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this vif_locking_mode x)
        {
            switch (x)
            {
                case vif_locking_mode.network_default:
                    return "network_default";
                case vif_locking_mode.locked:
                    return "locked";
                case vif_locking_mode.unlocked:
                    return "unlocked";
                case vif_locking_mode.disabled:
                    return "disabled";
                default:
                    return "unknown";
            }
        }
    }

    internal class vif_locking_modeConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((vif_locking_mode)value).StringOf());
        }
    }
}