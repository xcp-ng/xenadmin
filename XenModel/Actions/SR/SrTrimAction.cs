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
using System.Linq;
using System.Xml;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Actions
{
    public class SrTrimAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SrTrimAction(IXenConnection connection, SR sr)
            : base(connection, string.Format(Messages.ACTION_SR_TRIM_TITLE, sr.NameWithoutHost()), false)
        {
            SR = sr;
        }

        protected override void Run()
        {
            Description = Messages.ACTION_SR_TRIM_DESCRIPTION;

            var host = SR.GetFirstAttachedStorageHost();

            if (host == null)
            {
                log.WarnFormat("Plugin call trim.do_trim({0}) is not possible. Reason: Cannot reclaim freed space, because the SR is detached.", SR.uuid);
                Exception = new Exception(Messages.SR_TRIM_NO_STORAGE_HOST_ERROR);
                return;
            }

            var result = false;
            try
            {
                var args = new Dictionary<string, string> { { "sr_uuid", SR.uuid } };
                Result = Host.call_plugin(Session, host.opaque_ref, "trim", "do_trim", args);
                result = Result.ToLower() == "true";
            }
            catch (Failure failure)
            {
                log.WarnFormat("Plugin call trim.do_trim({0}) on {1} failed with {2}", SR.uuid, host.Name(),
                    failure.Message);
                throw;
            }

            if (result)
                Description = Messages.ACTION_SR_TRIM_DONE;
            else
            {
                log.WarnFormat("Plugin call trim.do_trim({0}) on {1} failed with {2}", SR.uuid, host.Name(), Result);
                var error = GetTrimError(Result);
                Exception = new Exception(error ?? Messages.ERROR_UNKNOWN);
            }
        }

        /* Example of trim output in case of exception:
            <trim_response>
                <key_value_pair>
                    <key>errcode</key>
                    <value>UnsupportedSRForTrim</value>
                </key_value_pair>
                <key_value_pair>
                    <key>errmsg</key>
                    <value>Trim on [some-uuid] not supported</value>
                </key_value_pair>
            </trim_response>
        */
        private static string GetTrimError(string xml)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                string errcode = null;
                string errmsg = null;

                var kvpNodes = doc.SelectNodes("/trim_response/key_value_pair");

                if (kvpNodes == null)
                    return null;

                foreach (XmlNode kvpNode in kvpNodes)
                {
                    var keyNode = kvpNode.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "key");
                    var valueNode = kvpNode.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "value");

                    if (keyNode != null && valueNode != null && keyNode.InnerText == "errcode")
                        errcode = valueNode.InnerText;

                    if (keyNode != null && valueNode != null && keyNode.InnerText == "errmsg")
                        errmsg = valueNode.InnerText;
                }

                return GetErrorMessageFromResources(errcode) ?? errmsg;
            }
            catch (Exception e)
            {
                log.DebugFormat("Exception parsing xml '{0}'", xml.Substring(0, 10000));
                log.Debug(e, e);

                return null;
            }
        }

        private static string GetErrorMessageFromResources(string errCode)
        {
            if (errCode == null)
                return null;
            return FriendlyNameManager.GetFriendlyName("Message.name-" + errCode);
        }
    }
}
