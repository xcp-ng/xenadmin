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
    [JsonConverter(typeof(pool_allowed_operationsConverter))]
    public enum pool_allowed_operations
    {
        /// <summary>
        /// Indicates this pool is in the process of enabling HA
        /// </summary>
        ha_enable,
        /// <summary>
        /// Indicates this pool is in the process of disabling HA
        /// </summary>
        ha_disable,
        /// <summary>
        /// Indicates this pool is in the process of creating a cluster
        /// </summary>
        cluster_create,
        /// <summary>
        /// Indicates this pool is in the process of changing master
        /// </summary>
        designate_new_master,
        /// <summary>
        /// Indicates this pool is in the process of configuring repositories
        /// </summary>
        configure_repositories,
        /// <summary>
        /// Indicates this pool is in the process of syncing updates
        /// </summary>
        sync_updates,
        /// <summary>
        /// Indicates this pool is in the process of syncing bundle
        /// </summary>
        sync_bundle,
        /// <summary>
        /// Indicates this pool is in the process of getting updates
        /// </summary>
        get_updates,
        /// <summary>
        /// Indicates this pool is in the process of applying updates
        /// </summary>
        apply_updates,
        /// <summary>
        /// Indicates this pool is in the process of enabling TLS verification
        /// </summary>
        tls_verification_enable,
        /// <summary>
        /// A certificate refresh and distribution is in progress
        /// </summary>
        cert_refresh,
        /// <summary>
        /// Indicates this pool is exchanging internal certificates with a new joiner
        /// </summary>
        exchange_certificates_on_join,
        /// <summary>
        /// Indicates this pool is exchanging ca certificates with a new joiner
        /// </summary>
        exchange_ca_certificates_on_join,
        /// <summary>
        /// Indicates the primary host is sending its certificates to another host
        /// </summary>
        copy_primary_host_certs,
        /// <summary>
        /// Ejection of a host from the pool is under way
        /// </summary>
        eject,
        /// <summary>
        /// Indicates this pool is exchanging CRLs with a new joiner
        /// </summary>
        exchange_crls_on_join,
        unknown
    }

    public static class pool_allowed_operations_helper
    {
        public static string ToString(pool_allowed_operations x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this pool_allowed_operations x)
        {
            switch (x)
            {
                case pool_allowed_operations.ha_enable:
                    return "ha_enable";
                case pool_allowed_operations.ha_disable:
                    return "ha_disable";
                case pool_allowed_operations.cluster_create:
                    return "cluster_create";
                case pool_allowed_operations.designate_new_master:
                    return "designate_new_master";
                case pool_allowed_operations.configure_repositories:
                    return "configure_repositories";
                case pool_allowed_operations.sync_updates:
                    return "sync_updates";
                case pool_allowed_operations.sync_bundle:
                    return "sync_bundle";
                case pool_allowed_operations.get_updates:
                    return "get_updates";
                case pool_allowed_operations.apply_updates:
                    return "apply_updates";
                case pool_allowed_operations.tls_verification_enable:
                    return "tls_verification_enable";
                case pool_allowed_operations.cert_refresh:
                    return "cert_refresh";
                case pool_allowed_operations.exchange_certificates_on_join:
                    return "exchange_certificates_on_join";
                case pool_allowed_operations.exchange_ca_certificates_on_join:
                    return "exchange_ca_certificates_on_join";
                case pool_allowed_operations.copy_primary_host_certs:
                    return "copy_primary_host_certs";
                case pool_allowed_operations.eject:
                    return "eject";
                case pool_allowed_operations.exchange_crls_on_join:
                    return "exchange_crls_on_join";
                default:
                    return "unknown";
            }
        }
    }

    internal class pool_allowed_operationsConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((pool_allowed_operations)value).StringOf());
        }
    }
}