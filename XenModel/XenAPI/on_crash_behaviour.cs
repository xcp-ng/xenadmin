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
    [JsonConverter(typeof(on_crash_behaviourConverter))]
    public enum on_crash_behaviour
    {
        /// <summary>
        /// destroy the VM state
        /// </summary>
        destroy,
        /// <summary>
        /// record a coredump and then destroy the VM state
        /// </summary>
        coredump_and_destroy,
        /// <summary>
        /// restart the VM
        /// </summary>
        restart,
        /// <summary>
        /// record a coredump and then restart the VM
        /// </summary>
        coredump_and_restart,
        /// <summary>
        /// leave the crashed VM paused
        /// </summary>
        preserve,
        /// <summary>
        /// rename the crashed VM and start a new copy
        /// </summary>
        rename_restart,
        unknown
    }

    public static class on_crash_behaviour_helper
    {
        public static string ToString(on_crash_behaviour x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this on_crash_behaviour x)
        {
            switch (x)
            {
                case on_crash_behaviour.destroy:
                    return "destroy";
                case on_crash_behaviour.coredump_and_destroy:
                    return "coredump_and_destroy";
                case on_crash_behaviour.restart:
                    return "restart";
                case on_crash_behaviour.coredump_and_restart:
                    return "coredump_and_restart";
                case on_crash_behaviour.preserve:
                    return "preserve";
                case on_crash_behaviour.rename_restart:
                    return "rename_restart";
                default:
                    return "unknown";
            }
        }
    }

    internal class on_crash_behaviourConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((on_crash_behaviour)value).StringOf());
        }
    }
}