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
    [JsonConverter(typeof(pvs_proxy_statusConverter))]
    public enum pvs_proxy_status
    {
        /// <summary>
        /// The proxy is not currently running
        /// </summary>
        stopped,
        /// <summary>
        /// The proxy is setup but has not yet cached anything
        /// </summary>
        initialised,
        /// <summary>
        /// The proxy is currently caching data
        /// </summary>
        caching,
        /// <summary>
        /// The PVS device is configured to use an incompatible write-cache mode
        /// </summary>
        incompatible_write_cache_mode,
        /// <summary>
        /// The PVS protocol in use is not compatible with the PVS proxy
        /// </summary>
        incompatible_protocol_version,
        unknown
    }

    public static class pvs_proxy_status_helper
    {
        public static string ToString(pvs_proxy_status x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this pvs_proxy_status x)
        {
            switch (x)
            {
                case pvs_proxy_status.stopped:
                    return "stopped";
                case pvs_proxy_status.initialised:
                    return "initialised";
                case pvs_proxy_status.caching:
                    return "caching";
                case pvs_proxy_status.incompatible_write_cache_mode:
                    return "incompatible_write_cache_mode";
                case pvs_proxy_status.incompatible_protocol_version:
                    return "incompatible_protocol_version";
                default:
                    return "unknown";
            }
        }
    }

    internal class pvs_proxy_statusConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((pvs_proxy_status)value).StringOf());
        }
    }
}