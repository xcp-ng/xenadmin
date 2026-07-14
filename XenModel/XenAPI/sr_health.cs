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
    [JsonConverter(typeof(sr_healthConverter))]
    public enum sr_health
    {
        /// <summary>
        /// Storage is fully available
        /// </summary>
        healthy,
        /// <summary>
        /// Storage is busy recovering, for example, rebuilding mirrors.
        /// </summary>
        recovering,
        /// <summary>
        /// Storage is unreachable but may be recoverable with admin intervention
        /// </summary>
        unreachable,
        /// <summary>
        /// Storage is unavailable, a host reboot will be required
        /// </summary>
        unavailable,
        unknown
    }

    public static class sr_health_helper
    {
        public static string ToString(sr_health x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this sr_health x)
        {
            switch (x)
            {
                case sr_health.healthy:
                    return "healthy";
                case sr_health.recovering:
                    return "recovering";
                case sr_health.unreachable:
                    return "unreachable";
                case sr_health.unavailable:
                    return "unavailable";
                default:
                    return "unknown";
            }
        }
    }

    internal class sr_healthConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((sr_health)value).StringOf());
        }
    }
}