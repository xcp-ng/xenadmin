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

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using XenAPI;


namespace XenAdmin.Actions
{
    public class FibreChannelProbeAction : PureAsyncAction
    {
        private readonly SR.SRTypes srType;

        public List<FibreChannelDevice> FibreChannelDevices;

        public FibreChannelProbeAction(Host master, SR.SRTypes srType = SR.SRTypes.lvmohba)
            : base(master.Connection, string.Format(Messages.PROBING_HBA_TITLE, master.Name()), null, true)
        {
            Host = master;
            this.srType = srType;
        }

        protected override void Run()
        {
            Description = Messages.PROBING_HBA;
            if (srType != SR.SRTypes.gfs2)
            {
                try
                {
                    Result = SR.probe(Session, Host.opaque_ref, new Dictionary<string, string>(), srType.ToString(), new Dictionary<string, string>());
                }
                catch (Failure f)
                {
                    if (f.ErrorDescription[0] == "SR_BACKEND_FAILURE_90" 
                     || f.ErrorDescription[0] == "SR_BACKEND_FAILURE_107")
                        Result = f.ErrorDescription[3];
                    else
                        throw;
                }

                FibreChannelDevices = ProcessXml(Result);
            }
            else
            {
                var deviceConfig = new Dictionary<string, string>();
                deviceConfig["provider"] = "hba";
                var result = SR.probe_ext(Session, Host.opaque_ref, deviceConfig, srType.ToString(), new Dictionary<string, string>());

                var list = new List<FibreChannelDevice>();
                foreach (var r in result)
                {
                    var dict = new Dictionary<string, string>(r.configuration);
                    r.extra_info.ToList().ForEach(kvp => dict.Add(kvp.Key, kvp.Value));
                    list.Add(new FibreChannelDevice(dict));
                }
                FibreChannelDevices = list;
            }
            Description = Messages.PROBED_HBA;
        }


        private static List<FibreChannelDevice> ProcessXml(string p)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(p);

                var devices = new List<FibreChannelDevice>();

                var blockDevices = doc.GetElementsByTagName("BlockDevice");

                foreach (XmlNode device in blockDevices)
                {
                    var properties = new Dictionary<string, string>();

                    foreach (XmlNode node in device.ChildNodes)
                        properties.Add(node.Name.ToLowerInvariant(), node.InnerText.Trim());

                    devices.Add(new FibreChannelDevice(properties));
                }

                return devices;
            }
            catch (XmlException e)
            {
                throw new Failure(Messages.FIBRECHANNEL_XML_ERROR, e);
            }
        }
    }
}
