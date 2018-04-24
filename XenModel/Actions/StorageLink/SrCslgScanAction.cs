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
using System.Xml;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions
{
    /// <summary>
    /// Used for scanning for CSLG servers
    /// </summary>
    public abstract class SrCslgScanAction : PureAsyncAction
    {
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _passwordSecret;

        /// <summary>
        /// Initializes a new instance of the <see cref="SrCslgScanAction"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="username">The username.</param>
        /// <param name="passwordSecret">The password secret.</param>
        protected SrCslgScanAction(IXenConnection connection, string hostname, string username, string passwordSecret)
            : base(connection, string.Format(Messages.ACTION_SR_SCAN_NAME_CSLG, hostname), string.Format(Messages.ACTION_SR_SCAN_NAME_CSLG, hostname), true)
        {
            Util.ThrowIfStringParameterNullOrEmpty(hostname, "hostname");
            Util.ThrowIfStringParameterNullOrEmpty(username, "username");

            _hostname = hostname;
            _username = username;
            _passwordSecret = passwordSecret;
        }

        protected void RunProbe(Dictionary<string, string> dconf)
        {
            // CA-40132 create a new secret based on the secret that was passed in to the constructor. This is required so that the
            // server doesn't associate the secret with a particular SR and then delete it when the SR is detached.         
            string newPasswordSecret = Secret.CreateSecret(Session, _passwordSecret);

            dconf["password_secret"] = newPasswordSecret;
            try
            {
                RelatedTask = XenAPI.SR.async_probe(Session, Helpers.GetMaster(Connection).opaque_ref, dconf, XenAPI.SR.SRTypes.cslg.ToString(), new Dictionary<String, String>());
                PollToCompletion();
            }
            finally
            {
                // now destroy the secret.
                if (!string.IsNullOrEmpty(newPasswordSecret))
                {
                    string newSecretRef = Secret.get_by_uuid(Session, newPasswordSecret);
                    Secret.destroy(Session, newSecretRef);
                }
            }
        }



        protected Dictionary<string, string> GetAuthenticationDeviceConfig()
        {
            Dictionary<String, String> dconf = new Dictionary<String, String>();
            dconf.Add("target", _hostname);
            dconf.Add("username", _username);
            dconf.Add("password_secret", _passwordSecret);

            return dconf;
        }

        protected string GetXmlNodeInnerText(XmlNode node, string xPath)
        {
            Util.ThrowIfParameterNull(node, "node");
            Util.ThrowIfStringParameterNullOrEmpty(xPath, "xPath");

            XmlNodeList nodes = node.SelectNodes(xPath);

            if (nodes == null || nodes.Count == 0)
            {
                throw new InvalidOperationException(string.Format(Messages.CANNOT_FIND_NODE, xPath));
            }

            return nodes[0].InnerText;
        }

        protected List<string> GetXmlChildNodeInnerTexts(XmlNode node, string xPath)
        {
            Util.ThrowIfParameterNull(node, "node");
            Util.ThrowIfStringParameterNullOrEmpty(xPath, "xPath");

            List<string> output = new List<string>();
            XmlNodeList nodes = node.SelectNodes(xPath);

            if (nodes == null)
            {
                throw new InvalidOperationException(string.Format(Messages.CANNOT_FIND_NODE, xPath));
            }

            foreach (XmlNode n in nodes)
            {
                output.Add(n.InnerText);
            }

            return output;
        }



    }
}
