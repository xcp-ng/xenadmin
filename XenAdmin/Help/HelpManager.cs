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
using System.Net;
using System.Resources;
using System.Threading;
using XenAdmin.Core;


namespace XenAdmin.Help
{
    class HelpManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly ResourceManager resources;
        private static readonly string HelpUrl = Registry.CustomHelpUrl;
        private static readonly string HelpQuery = string.Empty;
        private static volatile string _helpVersion;

        private static readonly object _helpLock = new object();

        static HelpManager()
        {
            resources = new ResourceManager("XenAdmin.Help.HelpManager", typeof(HelpManager).Assembly);

            if (string.IsNullOrEmpty(HelpUrl))
            {
                HelpUrl = InvisibleMessages.HELP_URL;

                HelpQuery = string.Format(InvisibleMessages.HELP_URL_QUERY,
                    $"{Branding.XENCENTER_VERSION}.{Program.Version.Revision}".Replace('.', '_'),
                    Messages.XENCENTER);
            }
        }

        internal static void SetHelpVersion()
        {
            ThreadPool.QueueUserWorkItem(obj =>
            {
                try
                {
                    var version = Program.Version;
                    var helpVersion = $"{version.Major}-{version.Minor}/";
                    var request = WebRequest.Create(HelpUrl + helpVersion + "index.html");
                    request.Method = "HEAD";

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        if (response != null && response.StatusCode == HttpStatusCode.OK)
                            _helpVersion = helpVersion;
                        else
                            _helpVersion = "current-release/";
                    }
                }
                catch
                {
                    _helpVersion = "current-release/";
                }
                finally
                {
                    lock (_helpLock)
                        Monitor.PulseAll(_helpLock);
                }
            });
        }

        internal static bool TryGetTopicId(string pageRef, out string topicId)
        {
            topicId = null;
            if (pageRef == null)
                return false;

            topicId = resources.GetString(pageRef);
            return topicId != null;
        }

        public static void Launch(string pageRef)
        {
            TryGetTopicId(pageRef, out string topicId);

            if (pageRef == null)
                log.WarnFormat("Attempted to launch help window with null help pageRef");
            else if (topicId == null)
                log.WarnFormat("Failed to find help topic ID for {0}", pageRef);
            else
                log.DebugFormat("Found help topic ID {0} for {1}", topicId, pageRef);

            if (string.IsNullOrEmpty(_helpVersion))
                lock (_helpLock)
                    Monitor.Wait(_helpLock);

            var helpTopicUrl = HelpUrl + _helpVersion + $"{topicId ?? "index"}.html" + HelpQuery;
            Program.OpenURL(helpTopicUrl.ToLowerInvariant());

            // record help usage
            Properties.Settings.Default.HelpLastUsed = DateTime.UtcNow.ToString("u");
            Settings.TrySaveSettings();
        }
    }

    internal interface IFormWithHelp
    {
        bool HasHelp();
    }

    internal interface IControlWithHelp
    {
        string HelpID { get; }
    }
}
