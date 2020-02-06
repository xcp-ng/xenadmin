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
    [JsonConverter(typeof(vgpu_type_implementationConverter))]
    public enum vgpu_type_implementation
    {
        /// <summary>
        /// Pass through an entire physical GPU to a guest
        /// </summary>
        passthrough,
        /// <summary>
        /// vGPU using NVIDIA hardware
        /// </summary>
        nvidia,
        /// <summary>
        /// vGPU using NVIDIA hardware with SR-IOV
        /// </summary>
        nvidia_sriov,
        /// <summary>
        /// vGPU using Intel GVT-g
        /// </summary>
        gvt_g,
        /// <summary>
        /// vGPU using AMD MxGPU
        /// </summary>
        mxgpu,
        unknown
    }

    public static class vgpu_type_implementation_helper
    {
        public static string ToString(vgpu_type_implementation x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this vgpu_type_implementation x)
        {
            switch (x)
            {
                case vgpu_type_implementation.passthrough:
                    return "passthrough";
                case vgpu_type_implementation.nvidia:
                    return "nvidia";
                case vgpu_type_implementation.nvidia_sriov:
                    return "nvidia_sriov";
                case vgpu_type_implementation.gvt_g:
                    return "gvt_g";
                case vgpu_type_implementation.mxgpu:
                    return "mxgpu";
                default:
                    return "unknown";
            }
        }
    }

    internal class vgpu_type_implementationConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((vgpu_type_implementation)value).StringOf());
        }
    }
}