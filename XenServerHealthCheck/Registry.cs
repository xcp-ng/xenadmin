/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using Microsoft.Win32;

namespace XenServerHealthCheck
{
    public class Registry
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Reads a key from XENCENTER_LOCAL_KEYS\k.
        /// </summary>
        private static string ReadKey(string k)
        {
            try
            {
                RegistryKey masterKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(HEALTH_CHECK_LOCAL_KEYS);
                if (masterKey == null)
                    return null;

                try
                {
                    var v = masterKey.GetValue(k);
                    return (v != null) ? v.ToString() : null;
                }
                finally
                {
                    masterKey.Close();
                }
            }
            catch (Exception e)
            {
                log.Debug($"Failed to read {HEALTH_CHECK_LOCAL_KEYS}\\{k} from registry", e);
                return null;
            }
        }

        /// <summary>
        /// Read a true/false key from XENCENTER_LOCAL_KEYS\k.
        /// </summary>
        /// <returns>False if the value is "false" (case insensitive), true if the key is present but not
        /// "false", def otherwise.</returns>
        private static Int32 ReadLong(string k)
        {
            try
            {
                RegistryKey masterKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(HEALTH_CHECK_LOCAL_KEYS);
                if (masterKey == null)
                    return 0;

                try
                {
                    object v = masterKey.GetValue(k);
                    if (v == null)
                        return 0;
                    else
                        return Convert.ToInt32(v);
                }
                finally
                {
                    masterKey.Close();
                }
            }
            catch (Exception e)
            {
                log.Debug($"Failed to read {HEALTH_CHECK_LOCAL_KEYS}\\{k} from registry.", e);
                return 0;
            }
        }

        public static string HealthCheckUploadDomainName
        {
            get 
            { 
                string domain_name = ReadKey(HEALTH_CHECK_UPLOAD_DOMAIN_NAME);
                if (string.IsNullOrEmpty(domain_name))
                    return "https://rttf.citrix.com/feeds/api/";
                return domain_name;
            }
        }

        public static Int32 HealthCheckTimeInterval
        {
            get 
            {
                Int32 interval = ReadLong(HEALTH_CHECK_TIME_INTERVAL);
                if(interval == 0)
                    interval = 30;
                return interval;
            }
        }

        private const string HEALTH_CHECK_LOCAL_KEYS = @"SOFTWARE\"+ Branding.COMPANY_NAME_SHORT +@"\XenHealthCheck";
        private const string HEALTH_CHECK_UPLOAD_DOMAIN_NAME = "HealthCheckUploadDomainName";
        private const string HEALTH_CHECK_TIME_INTERVAL = "HealthCheckTimeIntervalInMinutes";
        
    }

    public enum SSLCertificateTypes { None, Changed, All }
}
