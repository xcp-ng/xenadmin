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

using System;

namespace XenAPI
{
    public enum API_Version
    {
        /// <summary>XenServer 4.0 (rio)</summary>
        API_1_1 = 1,
        /// <summary>XenServer 4.1 (miami)</summary>
        API_1_2 = 2,
        /// <summary>XenServer 5.0 (orlando)</summary>
        API_1_3 = 3,
        /// <summary>Unreleased ()</summary>
        API_1_4 = 4,
        /// <summary>XenServer 5.0 update 3 ()</summary>
        API_1_5 = 5,
        /// <summary>XenServer 5.5 (george)</summary>
        API_1_6 = 6,
        /// <summary>XenServer 5.6 (midnight-ride)</summary>
        API_1_7 = 7,
        /// <summary>XenServer 5.6 FP1 (cowley)</summary>
        API_1_8 = 8,
        /// <summary>XenServer 6.0 (boston)</summary>
        API_1_9 = 9,
        /// <summary>XenServer 6.1 (tampa)</summary>
        API_1_10 = 10,
        /// <summary>XenServer 6.2 (clearwater)</summary>
        API_2_0 = 11,
        /// <summary>XenServer 6.2 SP1 (vgpu-productisation)</summary>
        API_2_1 = 12,
        /// <summary>XenServer 6.2 SP1 Hotfix 4 (clearwater-felton)</summary>
        API_2_2 = 13,
        /// <summary>XenServer 6.5 (creedence)</summary>
        API_2_3 = 14,
        /// <summary>XenServer 6.5 SP1 (cream)</summary>
        API_2_4 = 15,
        /// <summary>XenServer 7.0 (dundee)</summary>
        API_2_5 = 16,
        /// <summary>XenServer 7.1 (ely)</summary>
        API_2_6 = 17,
        /// <summary>XenServer 7.2 (falcon)</summary>
        API_2_7 = 18,
        /// <summary>XenServer 7.3 (inverness)</summary>
        API_2_8 = 19,
        /// <summary>XenServer 7.4 (jura)</summary>
        API_2_9 = 20,
        /// <summary>XenServer 7.5 (kolkata)</summary>
        API_2_10 = 21,
        /// <summary>XenServer 7.6 (lima)</summary>
        API_2_11 = 22,
        /// <summary>Citrix Hypervisor 8.0 (naples)</summary>
        API_2_12 = 23,
        /// <summary>Unreleased (oslo)</summary>
        API_2_13 = 24,
        /// <summary>Citrix Hypervisor 8.1 (quebec)</summary>
        API_2_14 = 25,
        /// <summary>Citrix Hypervisor 8.2 (stockholm)</summary>
        API_2_15 = 26,
        /// <summary>XenServer 8 Preview (nile-preview)</summary>
        API_2_20 = 27,
        /// <summary>XenServer 8 (nile)</summary>
        API_2_21 = 28,
        /// <summary>XenServer 9 (orinoco)</summary>
        API_2_23 = 29,
        LATEST = 29,
        UNKNOWN = 99
    }

    public static partial class Helper
    {
        public static string APIVersionString(API_Version v)
        {
            switch (v)
            {
                case API_Version.API_1_1:
                    return "1.1";
                case API_Version.API_1_2:
                    return "1.2";
                case API_Version.API_1_3:
                    return "1.3";
                case API_Version.API_1_4:
                    return "1.4";
                case API_Version.API_1_5:
                    return "1.5";
                case API_Version.API_1_6:
                    return "1.6";
                case API_Version.API_1_7:
                    return "1.7";
                case API_Version.API_1_8:
                    return "1.8";
                case API_Version.API_1_9:
                    return "1.9";
                case API_Version.API_1_10:
                    return "1.10";
                case API_Version.API_2_0:
                    return "2.0";
                case API_Version.API_2_1:
                    return "2.1";
                case API_Version.API_2_2:
                    return "2.2";
                case API_Version.API_2_3:
                    return "2.3";
                case API_Version.API_2_4:
                    return "2.4";
                case API_Version.API_2_5:
                    return "2.5";
                case API_Version.API_2_6:
                    return "2.6";
                case API_Version.API_2_7:
                    return "2.7";
                case API_Version.API_2_8:
                    return "2.8";
                case API_Version.API_2_9:
                    return "2.9";
                case API_Version.API_2_10:
                    return "2.10";
                case API_Version.API_2_11:
                    return "2.11";
                case API_Version.API_2_12:
                    return "2.12";
                case API_Version.API_2_13:
                    return "2.13";
                case API_Version.API_2_14:
                    return "2.14";
                case API_Version.API_2_15:
                    return "2.15";
                case API_Version.API_2_20:
                    return "2.20";
                case API_Version.API_2_21:
                    return "2.21";
                case API_Version.API_2_23:
                    return "2.23";
                default:
                    return "Unknown";
            }
        }

        public static API_Version GetAPIVersion(long major, long minor)
        {
            try
            {
                return (API_Version)Enum.Parse(typeof(API_Version), $"API_{major}_{minor}");
            }
            catch (ArgumentException)
            {
                return API_Version.UNKNOWN;
            }
        }

        /// <summary>
        /// Converts the string representation of an API version number to its API_Version equivalent.
        /// This function assumes that API version numbers are of form a.b
        /// </summary>
        public static API_Version GetAPIVersion(string version)
        {
            if (version != null)
            {
                string[] tokens = version.Split('.');
                int major, minor;
                if (tokens.Length == 2 &&
                    int.TryParse(tokens[0], out major) &&
                    int.TryParse(tokens[1], out minor))
                {
                    return GetAPIVersion(major, minor);
                }
            }
            return API_Version.UNKNOWN;
        }
    }
}