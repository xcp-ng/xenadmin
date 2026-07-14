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
    [JsonConverter(typeof(host_numa_affinity_policyConverter))]
    public enum host_numa_affinity_policy
    {
        /// <summary>
        /// VMs are spread across all available NUMA nodes
        /// </summary>
        any,
        /// <summary>
        /// VMs are placed on the smallest number of NUMA nodes that they fit using soft-pinning, but the policy doesn&apos;t guarantee a balanced placement, falling back to the &apos;any&apos; policy.
        /// </summary>
        best_effort,
        /// <summary>
        /// Use the NUMA affinity policy that is the default for the current version
        /// </summary>
        default_policy,
        unknown
    }

    public static class host_numa_affinity_policy_helper
    {
        public static string ToString(host_numa_affinity_policy x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this host_numa_affinity_policy x)
        {
            switch (x)
            {
                case host_numa_affinity_policy.any:
                    return "any";
                case host_numa_affinity_policy.best_effort:
                    return "best_effort";
                case host_numa_affinity_policy.default_policy:
                    return "default_policy";
                default:
                    return "unknown";
            }
        }
    }

    internal class host_numa_affinity_policyConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((host_numa_affinity_policy)value).StringOf());
        }
    }
}