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
using System.Net;
using System.Net.Sockets;
using System.Xml;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using System.Globalization;
using XenCenterLib;

namespace XenAdmin.Actions
{
    /// <summary>
    /// Ask the server for a list of IQNs on a particular iSCSI target.
    /// </summary>
    public class ISCSIPopulateIQNsAction : AsyncAction
    {
        protected readonly string targetHost;
        protected readonly UInt16 targetPort;
        protected readonly string chapUsername;
        protected readonly string chapPassword;

        protected IScsiIqnInfo[] _iqns;
        /// <summary>
        /// Will be null if the scan has not yet successfully returned.
        /// </summary>
        public IScsiIqnInfo[] IQNs
        {
            get
            {
                return _iqns;
            }
        }

        public ISCSIPopulateIQNsAction(IXenConnection connection, string targetHost,
            UInt16 targetPort, string chapUsername, string chapPassword)
            : base(connection, string.Format(Messages.ACTION_ISCSI_IQN_SCANNING, targetHost), null, true)
        {
            this.targetHost = targetHost;
            this.targetPort = targetPort;
            this.chapUsername = chapUsername;
            this.chapPassword = chapPassword;
        }

        public class NoIQNsFoundException : Exception
        {
            private readonly string host;

            public NoIQNsFoundException(string host)
            {
                this.host = host;
            }

            public override string Message
            {
                get
                {
                    return String.Format(Messages.NEWSR_NO_IQNS_FOUND, host);
                }
            }
        }

        private const string UriPrefix = "http://";

        private string ParseIPAddress(string address)
        {
            Uri url;
            if (Uri.TryCreate(string.Format("{0}{1}", UriPrefix, address), UriKind.Absolute, out url))
            {
                IPAddress ip;
                if (IPAddress.TryParse(url.Host, out ip))
                {
                    if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                        return "[" + ip + "]";
                    return ip.ToString();
                }
            }
            return address;
        }

        protected override void Run()
        {
            Pool pool = Helpers.GetPoolOfOne(Connection);
            if (pool == null)
                throw new Failure(Failure.INTERNAL_ERROR, Messages.POOL_GONE);

            Dictionary<string, string> settings = new Dictionary<string, string>();
            settings["target"] = targetHost;
            settings["port"] = targetPort.ToString(CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(chapUsername))
            {
                settings["chapuser"] = chapUsername;
                settings["chappassword"] = chapPassword;
            }
            
            try
            {
                // Perform a create with some missing params: should fail with the error
                // containing the list of SRs on the filer.
                RelatedTask = SR.async_create(Session, pool.master,
                     settings, 0, Helpers.GuiTempObjectPrefix, Messages.ISCSI_SHOULD_NO_BE_CREATED,
                     SR.SRTypes.lvmoiscsi.ToString(), "user", true, new Dictionary<string, string>());
                this.PollToCompletion();

                // Create should always fail and never get here
                throw new InvalidOperationException(Messages.ISCSI_FAIL);
            }
            catch (Failure exn)
            {
                if (exn.ErrorDescription.Count < 1)
                    throw new BadServerResponse(targetHost);

                // We expect an SR_BACKEND_FAILURE_96 error, with a message from
                // xapi, stdout, and then stderr.
                // stderr will be an XML-encoded description of the iSCSI IQNs.
                if (exn.ErrorDescription[0] != "SR_BACKEND_FAILURE_96")
                    throw;

                // We want a custom error if the server returns no aggregates.
                if (exn.ErrorDescription.Count < 4 || exn.ErrorDescription[3].Length == 0)
                    throw new NoIQNsFoundException(targetHost);

                XmlDocument doc = new XmlDocument();
                List<IScsiIqnInfo> results = new List<IScsiIqnInfo>();
                try
                {
                    doc.LoadXml(exn.ErrorDescription[3]);
                    foreach (XmlNode targetListNode in doc.GetElementsByTagName("iscsi-target-iqns"))
                    {
                        foreach (XmlNode targetNode in targetListNode.ChildNodes)
                        {
                            int index = -1;
                            string address = null;
                            UInt16 port = Util.DEFAULT_ISCSI_PORT;
                            string targetIQN = null;

                            foreach (XmlNode infoNode in targetNode.ChildNodes)
                            {
                                if (infoNode.Name.ToLowerInvariant() == "index")
                                {
                                    index = int.Parse(infoNode.InnerText, CultureInfo.InvariantCulture);
                                }
                                else if (infoNode.Name.ToLowerInvariant() == "ipaddress")
                                {
                                    string addr = infoNode.InnerText.Trim();
                                    address = ParseIPAddress(addr);
                                }
                                else if (infoNode.Name.ToLowerInvariant() == "port")
                                {
                                    port = UInt16.Parse(infoNode.InnerText, CultureInfo.InvariantCulture);
                                }
                                else if (infoNode.Name.ToLowerInvariant() == "targetiqn")
                                {
                                    targetIQN = infoNode.InnerText.Trim();
                                }
                            }
                            results.Add(new IScsiIqnInfo(index, targetIQN, address, port));
                        }
                    }
                    results.Sort();
                    _iqns = results.ToArray();
                }
                catch
                {
                    throw new BadServerResponse(targetHost);
                }

                if (_iqns.Length < 1)
                    throw new NoIQNsFoundException(targetHost);
            }
        }
    }

    public class Gfs2PopulateIQNsAction : ISCSIPopulateIQNsAction
    {
        public Gfs2PopulateIQNsAction(IXenConnection connection, string targetHost,
            UInt16 targetPort, string chapUsername, string chapPassword)
            : base(connection, targetHost, targetPort, chapUsername, chapPassword)
        { }

        protected override void Run()
        {
            Pool pool = Helpers.GetPoolOfOne(Connection);
            if (pool == null)
                throw new Failure(Failure.INTERNAL_ERROR, Messages.POOL_GONE);
            var deviceConfig = new Dictionary<string, string>();
            deviceConfig["provider"] = "iscsi";
            deviceConfig["ips"] = targetHost;
            deviceConfig["port"] = targetPort.ToString(CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(chapUsername))
            {
                deviceConfig["chapuser"] = chapUsername;
                deviceConfig["chappassword"] = chapPassword;
            }

            var probeResults = SR.probe_ext(Session, pool.master.opaque_ref,
                     deviceConfig, SR.SRTypes.gfs2.ToString(), new Dictionary<string, string>());

            var results = new List<IScsiIqnInfo>();
            var index = -1;
            foreach (var probeResult in probeResults)
            {
                index++;
                var address = probeResult.configuration.ContainsKey("ips") ? probeResult.configuration["ips"] : "";
                UInt16 port;
                if (!probeResult.configuration.ContainsKey("port") || !UInt16.TryParse(probeResult.configuration["port"], out port))
                    port = Util.DEFAULT_ISCSI_PORT;
                var targetIQN = probeResult.configuration.ContainsKey("iqns") ? probeResult.configuration["iqns"] : "";

                results.Add(new IScsiIqnInfo(index, targetIQN, address, port));
            }
            results.Sort();
            _iqns = results.ToArray();

            if (_iqns.Length < 1)
                throw new NoIQNsFoundException(targetHost);
        }
    }
    
    public struct IScsiIqnInfo : IComparable<IScsiIqnInfo>, IEquatable<IScsiIqnInfo>
    {
        public readonly int Index;
        /// <summary>
        /// May be null.
        /// </summary>
        public readonly string TargetIQN;
        /// <summary>
        /// May be null.
        /// </summary>
        public readonly string IpAddress;
        public readonly UInt16 Port;

        public IScsiIqnInfo(int index, string targetIQN, string ipAddress, UInt16 port)
        {
            Index = index;
            TargetIQN = targetIQN;
            IpAddress = ipAddress;
            Port = port;
        }

        public int CompareTo(IScsiIqnInfo other)
        {
            // Special case: * goes at the end
            if (TargetIQN == "*" && other.TargetIQN != "*")
                return 1;

            if (other.TargetIQN == "*" && TargetIQN != "*")
                return -1;

            // Sort by the TargetIQN (not the Index: see CA-40066)
            return StringUtility.NaturalCompare(TargetIQN, other.TargetIQN);
        }

        public bool Equals(IScsiIqnInfo other)
        {
            return this.Index == other.Index && this.TargetIQN == other.TargetIQN
            && this.IpAddress == other.IpAddress && this.Port == other.Port;
        }
    }
}
