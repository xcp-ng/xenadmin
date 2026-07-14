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
    [JsonConverter(typeof(vm_secureboot_certificates_stateConverter))]
    public enum vm_secureboot_certificates_state
    {
        /// <summary>
        /// The VM&apos;s certificates do not need to be updated (including the case where Secure Boot does not apply to this VM, e.g. BIOS VM).
        /// </summary>
        ok,
        /// <summary>
        /// The Secure Boot certificates are due to expire or have already expired.
        /// </summary>
        update_available,
        /// <summary>
        /// An update of the certificates will be triggered whenever the VM boots. This includes VM.start, VM.reboot and a guest-triggered reboot.
        /// </summary>
        update_on_boot,
        unknown
    }

    public static class vm_secureboot_certificates_state_helper
    {
        public static string ToString(vm_secureboot_certificates_state x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this vm_secureboot_certificates_state x)
        {
            switch (x)
            {
                case vm_secureboot_certificates_state.ok:
                    return "ok";
                case vm_secureboot_certificates_state.update_available:
                    return "update_available";
                case vm_secureboot_certificates_state.update_on_boot:
                    return "update_on_boot";
                default:
                    return "unknown";
            }
        }
    }

    internal class vm_secureboot_certificates_stateConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((vm_secureboot_certificates_state)value).StringOf());
        }
    }
}