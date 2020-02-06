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
    [JsonConverter(typeof(cluster_operationConverter))]
    public enum cluster_operation
    {
        /// <summary>
        /// adding a new member to the cluster
        /// </summary>
        add,
        /// <summary>
        /// removing a member from the cluster
        /// </summary>
        remove,
        /// <summary>
        /// enabling any cluster member
        /// </summary>
        enable,
        /// <summary>
        /// disabling any cluster member
        /// </summary>
        disable,
        /// <summary>
        /// completely destroying a cluster
        /// </summary>
        destroy,
        unknown
    }

    public static class cluster_operation_helper
    {
        public static string ToString(cluster_operation x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this cluster_operation x)
        {
            switch (x)
            {
                case cluster_operation.add:
                    return "add";
                case cluster_operation.remove:
                    return "remove";
                case cluster_operation.enable:
                    return "enable";
                case cluster_operation.disable:
                    return "disable";
                case cluster_operation.destroy:
                    return "destroy";
                default:
                    return "unknown";
            }
        }
    }

    internal class cluster_operationConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((cluster_operation)value).StringOf());
        }
    }
}