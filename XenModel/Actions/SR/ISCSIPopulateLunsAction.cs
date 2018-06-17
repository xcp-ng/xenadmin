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
using System.Globalization;
using System.Xml;

using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    /// <summary>
    /// Asks the server for a list of LUNs on a particular iSCSI target.
    /// </summary>
    public class ISCSIPopulateLunsAction : AsyncAction
    {
        protected readonly string targetHost;
        protected readonly UInt16 targetPort;
        protected readonly string targetIQN;
        protected readonly string chapUsername;
        protected readonly string chapPassword;

        protected ISCSIInfo[] _luns;
        /// <summary>
        /// Will be null if the scan has not yet successfully returned.
        /// </summary>
        public ISCSIInfo[] LUNs
        {
            get
            {
                return _luns;
            }
        }

        public ISCSIPopulateLunsAction(IXenConnection connection, string targetHost,
            UInt16 targetPort, string targetIQN, string chapUsername, string chapPassword)
            : base(connection, string.Format(Messages.ACTION_ISCSI_LUN_SCANNING, targetHost))
        {
            this.targetHost = targetHost;
            this.targetPort = targetPort;
            this.targetIQN = targetIQN;
            this.chapUsername = chapUsername;
            this.chapPassword = chapPassword;
        }

        public class NoLUNsFoundException : Exception
        {
            private string host;

            public NoLUNsFoundException(string host)
            {
                this.host = host;
            }

            public override string Message
            {
                get
                {
                    return string.Format(Messages.NEWSR_NO_LUNS_FOUND, host);
                }
            }
        }

        protected override void Run()
        {
            Pool pool = Helpers.GetPoolOfOne(Connection);
            if (pool == null)
                throw new Failure(Failure.INTERNAL_ERROR, Messages.POOL_GONE);

            Dictionary<string, string> settings = new Dictionary<string, string>();

            settings["target"] = targetHost;
            settings["port"] = targetPort.ToString(CultureInfo.InvariantCulture);
            settings["targetIQN"] = targetIQN;
            if (!string.IsNullOrEmpty(this.chapUsername))
            {
                settings["chapuser"] = this.chapUsername;
                settings["chappassword"] = this.chapPassword;
            }

            try
            {
                RelatedTask = SR.async_create(Session, pool.master, settings, 0, 
                    Helpers.GuiTempObjectPrefix, Messages.ISCSI_SHOULD_NO_BE_CREATED,
                    SR.SRTypes.lvmoiscsi.ToString(), "user", true, new Dictionary<string, string>());
                this.PollToCompletion();
                throw new InvalidOperationException(Messages.ISCSI_FAIL);
            }
            catch (Failure exn)
            {
                if (exn.ErrorDescription.Count < 1)
                    throw new BadServerResponse(targetHost);

                // We expect an SR_BACKEND_FAILURE_96 error, with a message from
                // xapi, stdout, and then stderr.
                // stderr will be an XML-encoded description of the iSCSI IQNs.
                if (exn.ErrorDescription[0] != "SR_BACKEND_FAILURE_107" && exn.ErrorDescription[0] != "SR_BACKEND_FAILURE_87")
                    throw;

                // We want a custom error if the server returns no aggregates.
                if (exn.ErrorDescription.Count < 4 || exn.ErrorDescription[3].Length == 0)
                    throw new NoLUNsFoundException(targetHost);


                try
                {
                    List<ISCSIInfo> results = new List<ISCSIInfo>();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(exn.ErrorDescription[3]);
                    foreach (XmlNode target_node in doc.GetElementsByTagName("iscsi-target"))
                    {
                        foreach (XmlNode lun_node in target_node.ChildNodes)
                        {
                            int lunid = -1;
                            string scsiid = "";
                            string vendor = "";
                            string serial = "";
                            long size = -1;
                            foreach (XmlNode n in lun_node.ChildNodes)
                            {
                                if (n.Name.ToLowerInvariant() == "lunid")
                                {
                                    lunid = int.Parse(n.InnerText.Trim(), CultureInfo.InvariantCulture);
                                }
                                else if (n.Name.ToLowerInvariant() == "vendor")
                                {
                                    vendor = n.InnerText.Trim();
                                }
                                else if (n.Name.ToLowerInvariant() == "serial")
                                {
                                    serial = n.InnerText.Trim();
                                }
                                else if (n.Name.ToLowerInvariant() == "size")
                                {
                                    long.TryParse(n.InnerText.Trim(), out size);
                                }
                                else if (n.Name.ToLowerInvariant() == "scsiid")
                                {
                                    scsiid = n.InnerText.Trim();
                                }
                            }
                            results.Add(new ISCSIInfo(scsiid,lunid, vendor, serial, size));
                        }
                    }
                    results.Sort();
                    _luns = results.ToArray();
                }
                catch
                {
                    throw new BadServerResponse(targetHost);
                }

                if (_luns.Length < 1)
                    throw new NoLUNsFoundException(targetHost);
            }
        }
    }

    public class Gfs2PopulateLunsAction : ISCSIPopulateLunsAction
    {
        public Gfs2PopulateLunsAction(IXenConnection connection, string targetHost,
            UInt16 targetPort, string targetIQN, string chapUsername, string chapPassword)
            : base(connection, targetHost, targetPort, targetIQN, chapUsername, chapPassword)
        { }

        protected override void Run()
        {
            Pool pool = Helpers.GetPoolOfOne(Connection);
            if (pool == null)
                throw new Failure(Failure.INTERNAL_ERROR, Messages.POOL_GONE);

            var deviceConfig = new Dictionary<string, string>();
            deviceConfig["provider"] = "iscsi";
            deviceConfig["target"] = targetHost;
            deviceConfig["port"] = targetPort.ToString(CultureInfo.InvariantCulture);
            deviceConfig["targetIQN"] = targetIQN;
            if (!string.IsNullOrEmpty(chapUsername))
            {
                deviceConfig["chapuser"] = chapUsername;
                deviceConfig["chappassword"] = chapPassword;
            }

            var probeResults = SR.probe_ext(Session, pool.master.opaque_ref,
                     deviceConfig, SR.SRTypes.gfs2.ToString().ToLowerInvariant(), new Dictionary<string, string>());

            var results = new List<ISCSIInfo>();
            foreach (var probeResult in probeResults)
            {
                int lunid;
                if (!probeResult.extra_info.ContainsKey("LUNid") || !int.TryParse(probeResult.extra_info["LUNid"], out lunid))
                    lunid = -1;
                var vendor = probeResult.extra_info.ContainsKey("vendor") ? probeResult.extra_info["vendor"] : "";
                var serial = probeResult.extra_info.ContainsKey("serial") ? probeResult.extra_info["serial"] : "";
                long size;
                if (!probeResult.extra_info.ContainsKey("size") || !long.TryParse(probeResult.extra_info["size"], out size))
                    size = -1;
                var scsiid = probeResult.configuration.ContainsKey("SCSIid") ? probeResult.configuration["SCSIid"] : "";

                results.Add(new ISCSIInfo(scsiid, lunid, vendor, serial, size));
            }
            results.Sort();
            _luns = results.ToArray();

            if (_luns.Length < 1)
                throw new NoLUNsFoundException(targetHost);
        }
    }

    public struct ISCSIInfo : IComparable<ISCSIInfo>
    {
        /// <summary>
        /// May be -1 when running against earlier Rio builds -- should always be set to a valid LUN ID otherwise.
        /// </summary>
        public int LunID;

        /// <summary>
        /// New identifier -- dont use LinID
        /// </summary>
        public string ScsiID;

        /// <summary>
        /// May be the empty string if no vendor field was supplied.
        /// </summary>
        public string Vendor;

        /// <summary>
        /// May be the empty string if no serial number was supplied.
        /// </summary>
        public string Serial;

        /// <summary>
        /// May be -1 if no size was supplied.
        /// </summary>
        public long Size;

        internal ISCSIInfo(string scsiid, int lunid, string vendor, string serial, long size)
        {
            ScsiID = scsiid;
            LunID = lunid;
            Vendor = vendor;
            Serial = serial;
            Size = size;
        }

        public int CompareTo(ISCSIInfo other)
        {
            return LunID - other.LunID;
        }
    }
}
