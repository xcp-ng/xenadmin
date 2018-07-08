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
using System.Collections.Generic;
using System.Text;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Actions;
using XenAPI;


namespace XenAdmin.Alerts
{
    public class XenServerVersionAlert : XenServerUpdateAlert
    {
        public XenServerVersion Version;

        public XenServerVersionAlert(XenServerVersion version)
        {
            Version = version;
            RequiredXenCenterVersion = Updates.GetRequiredXenCenterVersion(Version);
            _timestamp = version.TimeStamp;
        }

        public override string WebPageLabel
        {
            get { return Version.Url; }
        }

        public override string Name
        {
            get { return Version.Name; }
        }

        public override string Title
        {
            get { return string.Format(Messages.DOWLOAD_LATEST_XS_TITLE, Version.Name); }
        }

        public override string Description
        {
            get { return string.Format(Messages.DOWNLOAD_LATEST_XS_BODY, Version.Name); }
        }

        public override string FixLinkText
        {
            get
            {
                return Messages.ALERT_NEW_VERSION_DOWNLOAD;
            }
        }

        public override AlertPriority Priority
        {
            get { return AlertPriority.Priority5; }
        }

        public override Action FixLinkAction
        {
            get { return () => Program.OpenURL(Version.Url); }
        }

        public override string HelpID
        {
            get
            {
                return "XenServerUpdateAlert";
            }
        }

        protected override bool IsDismissed(IXenConnection connection)
        {
            Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
                return false;

            Dictionary<string, string> other_config = pool.other_config;

            if (other_config.ContainsKey(IgnoreServerAction.LAST_SEEN_SERVER_VERSION_KEY))
            {
                List<string> current = new List<string>(other_config[IgnoreServerAction.LAST_SEEN_SERVER_VERSION_KEY].Split(','));
                if (current.Contains(Version.VersionAndOEM))
                    return true;
            }
            return false;
        }

        public override bool Equals(Alert other)
        {
            if (other is XenServerVersionAlert)
            {
                return Version.VersionAndOEM == ((XenServerVersionAlert)other).Version.VersionAndOEM;
            }
            return base.Equals(other);
        }
    }
}
