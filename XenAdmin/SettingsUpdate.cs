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
using System.Reflection;
using XenAdmin;
using System.IO;

public static class SettingsUpdate
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    // The path of the user.config files looks something like this:
    // <Profile Directory>\<Company Name>\<App Name>_<Evidence Type>_<Evidence Hash>\<Version>\user.config

    /// <summary>
    /// Looks for a config file from a previous installation of the application and updates the settings from it.
    /// </summary>
    public static void Update()
    {
        try
        {
            Assembly a = Assembly.GetExecutingAssembly();
            Version appVersion = a.GetName().Version;

            // get previous config file by enumerating through all the folders in <Profile Directory>\<Company Name> 
            // to find a previous user.config file

            var currentConfigFolder = new DirectoryInfo(Settings.GetUserConfigPath()).Parent;

            if (currentConfigFolder == null)
                return;

            var appDomainName = AppDomain.CurrentDomain.FriendlyName;
            var companyFolder = currentConfigFolder.Parent != null ? currentConfigFolder.Parent.Parent : null;

            if (companyFolder == null)
                return;

            FileInfo previousConfig = null;
            Version previousVersion = null;

            foreach (var subDir in companyFolder.GetDirectories("*" + appDomainName + "*", SearchOption.AllDirectories))
            {
                foreach (var file in subDir.GetFiles("user.config", SearchOption.AllDirectories))
                {
                    var configFolderName = Path.GetFileName(Path.GetDirectoryName(file.FullName));
                    if (configFolderName != null)
                    {
                        var configVersion = new Version(configFolderName);

                        if ((configVersion <= appVersion) && (previousVersion == null || configVersion > previousVersion))
                        {
                            previousVersion = configVersion;
                            previousConfig = file;
                        }
                    }
                }
            }
            
            if (previousConfig != null)
            {
                // copy previous config file to current config location
                var destinationFile = Path.GetDirectoryName(currentConfigFolder.FullName);

                destinationFile = Path.Combine(destinationFile, previousVersion.ToString());

                if (!Directory.Exists(destinationFile))
                    Directory.CreateDirectory(destinationFile);

                destinationFile = Path.Combine(destinationFile, previousConfig.Name);

                File.Copy(previousConfig.FullName, destinationFile);

                // upgrade settings
                XenAdmin.Properties.Settings.Default.Upgrade();
            }
        }
        catch (Exception ex)
        {
            log.Debug("Exception while updating settings.", ex);
        }
    }
}
