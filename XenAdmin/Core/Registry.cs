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


namespace XenAdmin.Core
{
    public static class Registry
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal static bool AllowCredentialSave => ReadBool(ALLOW_CREDENTIAL_SAVE, true);

        internal static bool DisablePlugins => ReadBool(DISABLE_PLUGINS, false);

        internal static bool ForceSystemFonts => ReadBool(FORCE_SYSTEM_FONTS, false);

        internal static bool DontSudo => ReadBool(DONT_SUDO, false);

        public static SSLCertificateTypes SSLCertificateTypes
        {
            get
            {
                string v = ReadString(SSL_CERTIFICATES_KEY);

                if (v != null && v.ToUpperInvariant() == SSL_CERTIFICATES_CHANGED_ONLY)
                    return SSLCertificateTypes.Changed;

                if (v != null && v.ToUpperInvariant() == SSL_CERTIFICATES_ALL)
                    return SSLCertificateTypes.All;

                return SSLCertificateTypes.None;
            }
        }

        public static void AssertPowerShellInstalled()
        {
            string v = ReadRegistryValue(Microsoft.Win32.Registry.LocalMachine, PowerShellKey, PowerShellStamp);

            if (!int.TryParse(v, out var result) || result != 1)
                throw new I18NException(I18NExceptionType.PowerShellNotPresent);
        }

        public static void AssertPowerShellExecutionPolicyNonRestricted()
        {
            var val = ReadRegistryValue(Microsoft.Win32.Registry.LocalMachine, PSExecutionPolicyKey, PSExecutionPolicyName) ??
                      ReadRegistryValue(Microsoft.Win32.Registry.CurrentUser, PSExecutionPolicyKey, PSExecutionPolicyName);

            if (val == "Restricted")
                throw new I18NException(I18NExceptionType.PowerShellExecutionPolicyRestricted);
        }

        /// <summary>
        /// Reads a bool value key under HKEY_LOCAL_MACHINE\XENCENTER_LOCAL_KEYS
        /// </summary>
        /// <returns>False if the value is "false" (case insensitive), true if the key is present but not
        /// "false", defaultVal otherwise.</returns>
        private static bool ReadBool(string k, bool defaultVal)
        {
            var v = ReadRegistryValue(Microsoft.Win32.Registry.LocalMachine, XENCENTER_LOCAL_KEYS, k);
            return v == null ? defaultVal : v.ToUpperInvariant() != "FALSE";
        }

        /// <summary>
        /// Reads a string value k under HKEY_LOCAL_MACHINE\XENCENTER_LOCAL_KEYS
        /// </summary>
        private static string ReadString(string k)
        {
            return ReadRegistryValue(Microsoft.Win32.Registry.LocalMachine, XENCENTER_LOCAL_KEYS, k);
        }

        private static string ReadRegistryValue(RegistryKey baseKey, string subKey, string k)
        {
            if (baseKey == null)
                return null;

            try
            {
                using (var masterKey = baseKey.OpenSubKey(subKey))
                {
                    if (masterKey == null)
                        return null;

                    var v = masterKey.GetValue(k);
                    if (v != null)
                        return v.ToString();
                }
            }
            catch (Exception e)
            {
                log.Debug($"Failed to read {baseKey.Name}\\{subKey}\\{k} from registry; assuming NULL.", e);
            }

            return null;
        }

        private static string ReadRegistryValue(RegistryHive hKey, string subKey, string k, RegistryView rView = RegistryView.Default)
        {
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(hKey, rView))
                    return ReadRegistryValue(baseKey, subKey, k);
            }
            catch (Exception e)
            {
                log.Debug($"Failed to read {hKey}\\{subKey}\\{k} from registry view {rView}; assuming NULL.", e);
            }

            return null;
        }

        /// <summary>
        /// Reads a string value k under XENCENTER_LOCAL_KEYS, targeting the 32-bit
        /// registry view, and trying CurrentUser first and then LocalMachine
        /// </summary>
        private static string ReadInstalledKey(string k)
        {
            return ReadRegistryValue(RegistryHive.CurrentUser, XENCENTER_LOCAL_KEYS, k, RegistryView.Registry32) ??
                   ReadRegistryValue(RegistryHive.LocalMachine, XENCENTER_LOCAL_KEYS, k, RegistryView.Registry32);
        }

        public static string HealthCheckIdentityTokenDomainName => ReadString(HEALTH_CHECK_IDENTITY_TOKEN_DOMAIN_NAME);

        public static string HealthCheckUploadTokenDomainName => ReadString(HEALTH_CHECK_UPLOAD_TOKEN_DOMAIN_NAME);

        public static string HealthCheckUploadGrantTokenDomainName => ReadString(HEALTH_CHECK_UPLOAD_GRANT_TOKEN_DOMAIN_NAME);

        public static string HealthCheckUploadDomainName => ReadString(HEALTH_CHECK_UPLOAD_DOMAIN_NAME);

        public static string HealthCheckDiagnosticDomainName => ReadString(HEALTH_CHECK_DIAGNOSTIC_DOMAIN_NAME);

        public static string HealthCheckProductKey => ReadString(HEALTH_CHECK_PRODUCT_KEY);

        public static string HiddenFeatures => ReadInstalledKey(HIDDEN_FEATURES);

        public static string AdditionalFeatures => ReadInstalledKey(ADDITIONAL_FEATURES);

        public static string CustomUpdatesXmlLocation => ReadString(CUSTOM_UPDATES_XML_LOCATION);

        public static string CustomHelpUrl => ReadString(HELP_URL_OVERRIDE);

        private const string SSL_CERTIFICATES_CHANGED_ONLY = "CHANGED";
        private const string SSL_CERTIFICATES_ALL = "ALL";
        private const string SSL_CERTIFICATES_KEY = "ForceSSLCertificates";
        private const string ALLOW_CREDENTIAL_SAVE = "AllowCredentialSave";
        private const string FORCE_SYSTEM_FONTS = "ForceSystemFonts";
        private const string DISABLE_PLUGINS = "DisablePlugins";
        private const string DONT_SUDO = "DontSudo";
        private const string XENCENTER_LOCAL_KEYS = @"SOFTWARE\" + Branding.COMPANY_NAME_SHORT + @"\" + Branding.BRAND_CONSOLE;
        private const string PSExecutionPolicyKey = @"Software\Microsoft\PowerShell\1\ShellIds\Microsoft.PowerShell";
        private const string PSExecutionPolicyName = "ExecutionPolicy";
        private const string PowerShellKey = @"Software\Microsoft\PowerShell\1";
        private const string PowerShellStamp = "Install";
        private const string HEALTH_CHECK_IDENTITY_TOKEN_DOMAIN_NAME = "HealthCheckIdentityTokenDomainName";
        private const string HEALTH_CHECK_UPLOAD_TOKEN_DOMAIN_NAME = "HealthCheckUploadTokenDomainName";
        private const string HEALTH_CHECK_UPLOAD_GRANT_TOKEN_DOMAIN_NAME = "HealthCheckUploadGrantTokenDomainName";
        private const string HEALTH_CHECK_UPLOAD_DOMAIN_NAME = "HealthCheckUploadDomainName";
        private const string HEALTH_CHECK_DIAGNOSTIC_DOMAIN_NAME = "HealthCheckDiagnosticDomainName";
        private const string HEALTH_CHECK_PRODUCT_KEY = "HealthCheckProductKey";
        private const string HIDDEN_FEATURES = "HiddenFeatures";
        private const string ADDITIONAL_FEATURES = "AdditionalFeatures";
        private const string CUSTOM_UPDATES_XML_LOCATION = "CheckForUpdatesXmlLocationOverride";
        private const string HELP_URL_OVERRIDE = "HelpUrlOverride";
    }

    public enum SSLCertificateTypes { None, Changed, All }
}
