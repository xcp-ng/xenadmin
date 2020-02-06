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
    [JsonConverter(typeof(vmpp_archive_frequencyConverter))]
    public enum vmpp_archive_frequency
    {
        /// <summary>
        /// Never archive
        /// </summary>
        never,
        /// <summary>
        /// Archive after backup
        /// </summary>
        always_after_backup,
        /// <summary>
        /// Daily archives
        /// </summary>
        daily,
        /// <summary>
        /// Weekly backups
        /// </summary>
        weekly,
        unknown
    }

    public static class vmpp_archive_frequency_helper
    {
        public static string ToString(vmpp_archive_frequency x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this vmpp_archive_frequency x)
        {
            switch (x)
            {
                case vmpp_archive_frequency.never:
                    return "never";
                case vmpp_archive_frequency.always_after_backup:
                    return "always_after_backup";
                case vmpp_archive_frequency.daily:
                    return "daily";
                case vmpp_archive_frequency.weekly:
                    return "weekly";
                default:
                    return "unknown";
            }
        }
    }

    internal class vmpp_archive_frequencyConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((vmpp_archive_frequency)value).StringOf());
        }
    }
}