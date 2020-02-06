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
    [JsonConverter(typeof(update_after_apply_guidanceConverter))]
    public enum update_after_apply_guidance
    {
        /// <summary>
        /// This update requires HVM guests to be restarted once applied.
        /// </summary>
        restartHVM,
        /// <summary>
        /// This update requires PV guests to be restarted once applied.
        /// </summary>
        restartPV,
        /// <summary>
        /// This update requires the host to be restarted once applied.
        /// </summary>
        restartHost,
        /// <summary>
        /// This update requires XAPI to be restarted once applied.
        /// </summary>
        restartXAPI,
        unknown
    }

    public static class update_after_apply_guidance_helper
    {
        public static string ToString(update_after_apply_guidance x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this update_after_apply_guidance x)
        {
            switch (x)
            {
                case update_after_apply_guidance.restartHVM:
                    return "restartHVM";
                case update_after_apply_guidance.restartPV:
                    return "restartPV";
                case update_after_apply_guidance.restartHost:
                    return "restartHost";
                case update_after_apply_guidance.restartXAPI:
                    return "restartXAPI";
                default:
                    return "unknown";
            }
        }
    }

    internal class update_after_apply_guidanceConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((update_after_apply_guidance)value).StringOf());
        }
    }
}