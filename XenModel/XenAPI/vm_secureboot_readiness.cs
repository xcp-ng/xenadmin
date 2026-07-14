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
    [JsonConverter(typeof(vm_secureboot_readinessConverter))]
    public enum vm_secureboot_readiness
    {
        /// <summary>
        /// VM&apos;s firmware is not UEFI
        /// </summary>
        not_supported,
        /// <summary>
        /// Secureboot is disabled on this VM
        /// </summary>
        disabled,
        /// <summary>
        /// Secured boot is enabled on this VM and its NVRAM.EFI-variables are empty
        /// </summary>
        first_boot,
        /// <summary>
        /// Secured boot is enabled on this VM and PK, KEK, db and dbx are defined in its EFI variables
        /// </summary>
        ready,
        /// <summary>
        /// Secured boot is enabled on this VM and PK, KEK, db but not dbx are defined in its EFI variables
        /// </summary>
        ready_no_dbx,
        /// <summary>
        /// Secured boot is enabled on this VM and PK is not defined in its EFI variables
        /// </summary>
        setup_mode,
        /// <summary>
        /// Secured boot is enabled on this VM and the certificates defined in its EFI variables are incomplete
        /// </summary>
        certs_incomplete,
        unknown
    }

    public static class vm_secureboot_readiness_helper
    {
        public static string ToString(vm_secureboot_readiness x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this vm_secureboot_readiness x)
        {
            switch (x)
            {
                case vm_secureboot_readiness.not_supported:
                    return "not_supported";
                case vm_secureboot_readiness.disabled:
                    return "disabled";
                case vm_secureboot_readiness.first_boot:
                    return "first_boot";
                case vm_secureboot_readiness.ready:
                    return "ready";
                case vm_secureboot_readiness.ready_no_dbx:
                    return "ready_no_dbx";
                case vm_secureboot_readiness.setup_mode:
                    return "setup_mode";
                case vm_secureboot_readiness.certs_incomplete:
                    return "certs_incomplete";
                default:
                    return "unknown";
            }
        }
    }

    internal class vm_secureboot_readinessConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((vm_secureboot_readiness)value).StringOf());
        }
    }
}