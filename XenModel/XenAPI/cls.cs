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
    [JsonConverter(typeof(clsConverter))]
    public enum cls
    {
        /// <summary>
        /// VM
        /// </summary>
        VM,
        /// <summary>
        /// Host
        /// </summary>
        Host,
        /// <summary>
        /// SR
        /// </summary>
        SR,
        /// <summary>
        /// Pool
        /// </summary>
        Pool,
        /// <summary>
        /// VMPP
        /// </summary>
        VMPP,
        /// <summary>
        /// VMSS
        /// </summary>
        VMSS,
        /// <summary>
        /// PVS_proxy
        /// </summary>
        PVS_proxy,
        /// <summary>
        /// VDI
        /// </summary>
        VDI,
        unknown
    }

    public static class cls_helper
    {
        public static string ToString(cls x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this cls x)
        {
            switch (x)
            {
                case cls.VM:
                    return "VM";
                case cls.Host:
                    return "Host";
                case cls.SR:
                    return "SR";
                case cls.Pool:
                    return "Pool";
                case cls.VMPP:
                    return "VMPP";
                case cls.VMSS:
                    return "VMSS";
                case cls.PVS_proxy:
                    return "PVS_proxy";
                case cls.VDI:
                    return "VDI";
                default:
                    return "unknown";
            }
        }
    }

    internal class clsConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((cls)value).StringOf());
        }
    }
}