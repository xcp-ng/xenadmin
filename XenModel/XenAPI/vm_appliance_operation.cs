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
    [JsonConverter(typeof(vm_appliance_operationConverter))]
    public enum vm_appliance_operation
    {
        /// <summary>
        /// Start
        /// </summary>
        start,
        /// <summary>
        /// Clean shutdown
        /// </summary>
        clean_shutdown,
        /// <summary>
        /// Hard shutdown
        /// </summary>
        hard_shutdown,
        /// <summary>
        /// Shutdown
        /// </summary>
        shutdown,
        unknown
    }

    public static class vm_appliance_operation_helper
    {
        public static string ToString(vm_appliance_operation x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this vm_appliance_operation x)
        {
            switch (x)
            {
                case vm_appliance_operation.start:
                    return "start";
                case vm_appliance_operation.clean_shutdown:
                    return "clean_shutdown";
                case vm_appliance_operation.hard_shutdown:
                    return "hard_shutdown";
                case vm_appliance_operation.shutdown:
                    return "shutdown";
                default:
                    return "unknown";
            }
        }
    }

    internal class vm_appliance_operationConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((vm_appliance_operation)value).StringOf());
        }
    }
}