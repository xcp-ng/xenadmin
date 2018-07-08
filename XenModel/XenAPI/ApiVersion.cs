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

using System;
using System.Collections;
using System.Collections.Generic;


namespace XenAPI
{
    public enum API_Version
    {
        API_1_1 = 1, //XenServer 4.0 (rio)
        API_1_2 = 2, //XenServer 4.1 (miami)
        API_1_3 = 3, //XenServer 5.0 (orlando)
        API_1_4 = 4, //Unreleased ()
        API_1_5 = 5, //XenServer 5.0 update 3 ()
        API_1_6 = 6, //XenServer 5.5 (george)
        API_1_7 = 7, //XenServer 5.6 (midnight-ride)
        API_1_8 = 8, //XenServer 5.6 FP1 (cowley)
        API_1_9 = 9, //XenServer 6.0 (boston)
        API_1_10 = 10, //XenServer 6.1 (tampa)
        API_2_0 = 11, //XenServer 6.2 (clearwater)
        API_2_1 = 12, //XenServer 6.2 SP1 (vgpu-productisation)
        API_2_2 = 13, //XenServer 6.2 SP1 Hotfix 4 (clearwater-felton)
        API_2_3 = 14, //XenServer 6.5 (creedence)
        API_2_4 = 15, //XenServer 6.5 SP1 (cream)
        API_2_5 = 16, //XenServer 7.0 (dundee)
        API_2_6 = 17, //XenServer 7.1 (ely)
        API_2_7 = 18, //XenServer 7.2 (falcon)
        API_2_8 = 19, //XenServer 7.3 (inverness)
        API_2_9 = 20, //XenServer 7.4 (jura)
        API_2_10 = 21, //XenServer 7.5 (kolkata)
        API_2_11 = 22, //Unreleased (lima)
        LATEST = 22,
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
                default:
                    return "Unknown";
            }
        }

        public static API_Version GetAPIVersion(long major, long minor)
        {
            try
            {
                return (API_Version)Enum.Parse(typeof(API_Version),
                    string.Format("API_{0}_{1}", major, minor));
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
                if (tokens.Length == 2 && int.TryParse(tokens[0], out major) && int.TryParse(tokens[1], out minor))
                {
                    return GetAPIVersion(major, minor);
                }
            }
            return API_Version.UNKNOWN;
        }

        /// <summary>
        /// Return a positive number if the given session's API version is greater than the given
        /// API_version, negative if it is less, and 0 if they are equal.
        /// </summary>
        internal static int APIVersionCompare(Session session, API_Version v)
        {
            return (int)session.APIVersion - (int)v;
        }

        /// <summary>
        /// Return true if the given session's API version is greater than or equal to the given
        /// API_version.
        /// </summary>
        internal static bool APIVersionMeets(Session session, API_Version v)
        {
            return APIVersionCompare(session, v) >= 0;
        }
    }
}
