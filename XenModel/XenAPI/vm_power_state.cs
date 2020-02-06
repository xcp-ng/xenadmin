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
    [JsonConverter(typeof(vm_power_stateConverter))]
    public enum vm_power_state
    {
        /// <summary>
        /// VM is offline and not using any resources
        /// </summary>
        Halted,
        /// <summary>
        /// All resources have been allocated but the VM itself is paused and its vCPUs are not running
        /// </summary>
        Paused,
        /// <summary>
        /// Running
        /// </summary>
        Running,
        /// <summary>
        /// VM state has been saved to disk and it is nolonger running. Note that disks remain in-use while the VM is suspended.
        /// </summary>
        Suspended,
        unknown
    }

    public static class vm_power_state_helper
    {
        public static string ToString(vm_power_state x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this vm_power_state x)
        {
            switch (x)
            {
                case vm_power_state.Halted:
                    return "Halted";
                case vm_power_state.Paused:
                    return "Paused";
                case vm_power_state.Running:
                    return "Running";
                case vm_power_state.Suspended:
                    return "Suspended";
                default:
                    return "unknown";
            }
        }
    }

    internal class vm_power_stateConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((vm_power_state)value).StringOf());
        }
    }
}