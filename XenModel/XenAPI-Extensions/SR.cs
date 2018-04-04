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
using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Network;

namespace XenAPI
{
    public partial class SR : IComparable<SR>, IEquatable<SR>
    {
        /// <summary>
        /// The SR types. Note that the names of these enum members correspond exactly to the SR type string as
        /// recognised by the server. If you add anything here, check whether it should also be added to
        /// SR.CanCreateWithXenCenter. Also add it to XenAPI/FriendlyNames.resx under
        /// Label-SR.SRTypes-*. Don't change the lower-casing!
        /// </summary>
        public enum SRTypes
        {
            local, ext, lvmoiscsi, iso, nfs, lvm, netapp, udev, lvmofc,
            lvmohba, egenera, egeneracd, dummy, unknown, equal, cslg, shm,
            iscsi,
            ebs, rawhba,
            smb, lvmofcoe, gfs2,
            nutanix, nutanixiso, 
            tmpfs
        }

        public const string Content_Type_ISO = "iso";
        public const string SM_Config_Type_CD = "cd";

        private const string XenServer_Tools_Label = "XenServer Tools";

        public override string ToString()
        {
            return Name();
        }

        /// <returns>A friendly name for the SR.</returns>
        public override string Name()
        {
            return I18N("name_label", name_label, true);
        }

        /// <summary>
        /// Get the given SR's home, i.e. the host under which we are going to display it.  May return null, if this SR should live
        /// at the pool level.
        /// </summary>
        public Host Home()
        {
            if (shared || PBDs.Count != 1)
                return null;

            PBD pbd = Connection.Resolve(PBDs[0]);
            if (pbd == null)
                return null;

            return Connection.Resolve(pbd.host);
        }

        public string NameWithoutHost()
        {
            return I18N("name_label", name_label, false);
        }

        /// <returns>A friendly description for the SR.</returns>
        public override string Description()
        {
            return I18N("name_description", name_description, true);
        }

        public bool Physical()
        {
            SRTypes typ = GetSRType(false);
            return typ == SRTypes.local || (typ == SRTypes.udev && SMConfigType() == SM_Config_Type_CD);
        }

        public string SMConfigType()
        {
            return Get(sm_config, "type");
        }

        public bool IsToolsSR()
        {
            return name_label == SR.XenServer_Tools_Label || is_tools_sr;
        }

        public string FriendlyTypeName()
        {
            return getFriendlyTypeName(GetSRType(false));
        }

        /// <summary>
        /// A friendly (internationalized) name for the SR type.
        /// </summary>
        public static string getFriendlyTypeName(SRTypes srType)
        {
            return PropertyManager.GetFriendlyName(string.Format("Label-SR.SRTypes-{0}", srType.ToString()),
                                                       "Label-SR.SRTypes-unknown");
        }

        /// <summary>
        /// Use this instead of type
        /// </summary>
        public SRTypes GetSRType(bool ignoreCase)
        {
            try
            {
                return (SRTypes)Enum.Parse(typeof(SRTypes), type, ignoreCase);
            }
            catch
            {
                return SRTypes.unknown;
            }
        }

        public SM GetSM()
        {
            return SM.GetByType(Connection, GetSRType(true).ToString());
        }

        public string ConfigType()
        {
           return Get(sm_config, "type");
        }

        private string I18N(string field_name, string field_value, bool with_host)
        {
            if (!other_config.ContainsKey("i18n-key"))
            {
                return field_value;
            }

            string i18n_key = other_config["i18n-key"];

            string original_value_key = "i18n-original-value-" + field_name;
            string original_value =
                other_config.ContainsKey(original_value_key) ? other_config[original_value_key] : "";

            if (original_value != field_value)
                return field_value;

            string hostname = with_host ? GetHostName() : null;
            if (hostname == null)
            {
                string pattern = PropertyManager.GetFriendlyName(string.Format("SR.{0}-{1}", field_name, i18n_key));
                return pattern == null ? field_value : pattern;
            }
            else
            {
                string pattern = PropertyManager.GetFriendlyName(string.Format("SR.{0}-{1}-host", field_name, i18n_key));
                return pattern == null ? field_value : string.Format(pattern, hostname);
            }
        }

        /// <returns>The name of the host to which this SR is attached, or null if the storage is shared
        /// or unattached.</returns>
        private string GetHostName()
        {
            if (Connection == null)
                return null;
            Host host = GetStorageHost();
            return host == null ? null : host.Name();
        }


        /// <returns>The host to which the given SR belongs, or null if the SR is shared or completely disconnected.</returns>
        public Host GetStorageHost()
        {
            if (shared || PBDs.Count != 1)
                return null;

            PBD pbd = Connection.Resolve(PBDs[0]);
            return pbd == null ? null : Connection.Resolve(pbd.host);
        }

        /// <summary>
        /// Iterating through the PBDs, this will return the storage host of the first PBD that is currently_attached.
        /// This will return null if there are no PBDs or none of them is currently_attached
        /// </summary>
        /// <returns></returns>
        public Host GetFirstAttachedStorageHost()
        {
            if (PBDs.Count == 0)
                return null;

            var currentlyAttachedPBDs = PBDs.Select(pbdref => Connection.Resolve(pbdref)).Where(p => p != null && p.currently_attached);

            if (currentlyAttachedPBDs.FirstOrDefault() != null)
                return currentlyAttachedPBDs.Select(p => Connection.Resolve(p.host)).Where(h => h != null).FirstOrDefault();

            return null;
        }

        public bool IsDetachable()
        {
            return !IsDetached() && !HasRunningVMs() && CanCreateWithXenCenter();
        }

        /// <summary>
        /// Can create with XC, or is citrix storage link gateway. Special case alert!
        /// </summary>
        public bool CanCreateWithXenCenter()
        {
            SRTypes type = GetSRType(false);
            return type == SRTypes.iso
                || type == SRTypes.lvmoiscsi
                || type == SRTypes.nfs
                || type == SRTypes.equal
                || type == SRTypes.netapp
                || type == SRTypes.lvmohba
                || type == SRTypes.cslg
                || type == SRTypes.smb
                || type == SRTypes.lvmofcoe
                || type == SRTypes.gfs2;
        }

        public bool IsLocalSR()
        {
            SRTypes typ = GetSRType(false);
            return typ == SRTypes.local
                   || typ == SRTypes.ext
                   || typ == SRTypes.lvm
                   || typ == SRTypes.udev
                   || typ == SRTypes.egeneracd
                   || typ == SRTypes.dummy;
        }

        public bool ShowForgetWarning()
        {
            return GetSRType(false) != SRTypes.iso;
        }

        /// <summary>
        /// Internal helper function. True if all the PBDs for this SR are currently_attached.
        /// </summary>
        /// <returns></returns>
        private bool AllPBDsAttached()
        {
            return Connection.ResolveAll(this.PBDs).All(pbd => pbd.currently_attached);
        }

        /// <summary>
        /// Internal helper function. True if any of the PBDs for this SR is currently_attached.
        /// </summary>
        /// <returns></returns>
        private bool AnyPBDAttached()
        {
            return Connection.ResolveAll(this.PBDs).Any(pbd => pbd.currently_attached);
        }

        /// <summary>
        /// Returns true if there are any Running or Suspended VMs attached to VDIs on this SR.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public bool HasRunningVMs()
        {
            foreach (VDI vdi in Connection.ResolveAll(VDIs))
            {
                foreach (VBD vbd in Connection.ResolveAll(vdi.VBDs))
                {
                    VM vm = Connection.Resolve(vbd.VM);
                    if (vm == null)
                        continue;
                    // PR-1223: ignore control domain VM on metadata VDIs, so that the SR can be detached if there are no other running VMs
                    if (vdi.type == vdi_type.metadata && vm.is_control_domain)
                        continue;
                    if (vm.power_state == vm_power_state.Running)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// If host is non-null, return whether this storage can be seen from the given host.
        /// If host is null, return whether the storage is shared, with a PBD for each host and at least one PBD plugged.
        /// (See CA-36285 for why this is the right test when looking for SRs on which to create a new VM).
        /// </summary>
        public virtual bool CanBeSeenFrom(Host host)
        {
            if (host == null)
            {
                return shared && Connection != null && !IsBroken(false) && AnyPBDAttached();
            }

            foreach (PBD pbd in host.Connection.ResolveAll(PBDs))
                if (pbd.currently_attached && pbd.host.opaque_ref == host.opaque_ref)
                    return true;

            return false;
        }

        public PBD GetPBDFor(Host host)
        {
            foreach (PBD pbd in host.Connection.ResolveAll(PBDs))
                if (pbd.host.opaque_ref == host.opaque_ref)
                    return pbd;

            return null;
        }

        /// <summary>
        /// True if there is less than 0.5GB free. Always false for dummy and ebs SRs.
        /// </summary>
        public bool IsFull()
        {
            SRTypes t = GetSRType(false);
            return t != SRTypes.dummy && t != SRTypes.ebs && FreeSpace() < XenAdmin.Util.BINARY_GIGA/2;
        }

        public virtual long FreeSpace()
        {
            return physical_size - physical_utilisation;
        }

        public virtual bool ShowInVDISRList(bool showHiddenVMs)
        {
            if (content_type == Content_Type_ISO)
                return false;
            return Show(showHiddenVMs);

        }

        public bool IsDetached()
        {
            // SR is detached when it has no PBDs or when all its PBDs are unplugged
            return !HasPBDs() || !AnyPBDAttached();
        }

        public bool HasPBDs()
        {
            // CA-15188: Show SRs with no PBDs on Orlando, as pool-eject bug has been fixed.
            // SRs are detached if they have no PBDs

            return PBDs.Count > 0;
        }

        public override bool Show(bool showHiddenVMs)
        {
            if (name_label.StartsWith(Helpers.GuiTempObjectPrefix))
                return false;

            SRTypes srType = GetSRType(false);

            // CA-15012 - dont show cd drives of type local on miami (if dont get destroyed properly on upgrade)
            if (srType == SRTypes.local)
                return false;

            // Hide Memory SR
            if (srType == SRTypes.tmpfs)
                return false;

            if (showHiddenVMs)
                return true;

			//CP-2458: hide SRs that were introduced by a DR_task
			if (introduced_by != null && introduced_by.opaque_ref != Helper.NullOpaqueRef)
				return false;

            return !IsHidden();
        }

        public override bool IsHidden()
        {
            return BoolKey(other_config, HIDE_FROM_XENCENTER);
        }

        /// <summary>
        /// The SR is broken when it has the wrong number of PBDs, or (optionally) not all the PBDs are attached.
        /// </summary>
        /// <param name="checkAttached">Whether to check that all the PBDs are attached</param>
        /// <returns></returns>
        public virtual bool IsBroken(bool checkAttached)
        {
            if (PBDs == null || PBDs.Count == 0 ||
                checkAttached && !AllPBDsAttached())
            {
                return true;
            }
            Pool pool = Helpers.GetPoolOfOne(Connection);
            if (pool == null || !shared)
            {
                if (PBDs.Count != 1)
                {
                    // There should be exactly one PBD, since this is a non-shared SR
                    return true;
                }
            }
            else
            {
                if (PBDs.Count != Connection.Cache.HostCount)
                {
                    // There isn't a PBD for each host
                    return true;
                }
            }
            return false;
        }

        public bool IsBroken()
        {
            return IsBroken(true);
        }

        public static bool IsDefaultSr(SR sr)
        {
            Pool pool = Helpers.GetPoolOfOne(sr.Connection);
            return pool != null && pool.default_SR != null && pool.default_SR.opaque_ref == sr.opaque_ref;
        }

        /// <summary>
        /// Returns true if a new VM may be created on this SR: the SR supports VDI_CREATE, has the right number of PBDs, and is not full.
        /// </summary>
        /// <param name="myConnection">The IXenConnection whose cache this XenObject belongs to. May not be null.</param>
        /// <returns></returns>
        public bool CanCreateVmOn()
        {
            System.Diagnostics.Trace.Assert(Connection != null, "Connection must not be null");

            return SupportsVdiCreate() && !IsBroken(false) && !IsFull();
        }


        /// <summary>
        /// Whether the underlying SR backend supports VDI_CREATE. Will return true even if the SR is full.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public virtual bool SupportsVdiCreate()
        {
            // ISO SRs are deemed not to support VDI create in the GUI, even though the back end
            // knows that they do. See CA-40119.
            if (content_type == SR.Content_Type_ISO)
                return false;

            // Memory SRs should not support VDI create in the GUI
            if (GetSRType(false) == SR.SRTypes.tmpfs)
                return false;

            SM sm = SM.GetByType(Connection, type);
            return sm != null && -1 != Array.IndexOf(sm.capabilities, "VDI_CREATE");
        }

        public static List<SRInfo> ParseSRList(List<Probe_result> probeExtResult)
        {
            List<SRInfo> results = new List<SRInfo>();
            foreach (var probeResult in probeExtResult.Where(p => p.sr != null))
            {
                string uuid = probeResult.sr.uuid;
                long size = probeResult.sr.total_space;
                string aggr = "";
                string name_label = probeResult.sr.name_label;
                string name_description = probeResult.sr.name_description;
                bool pool_metadata_detected = false;

                results.Add(new SRInfo(uuid, size, aggr, name_label, name_description, pool_metadata_detected));
            }
            return results;
        }

        /// <summary>
        /// Parses an XML list of SRs (as returned by the SR.probe() call) into a list of SRInfos.
        /// </summary>
        public static List<SRInfo> ParseSRListXML(string xml)
        {
            List<SRInfo> results = new List<SRInfo>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            // If we've got this from an async task result, then it will be wrapped
            // in a <value> element.  Parse the contents instead.
            foreach (XmlNode node in doc.GetElementsByTagName("value"))
            {
                xml = node.InnerText;
                doc = new XmlDocument();
                doc.LoadXml(xml);
                break;
            }

            foreach (XmlNode node in doc.GetElementsByTagName("SR"))
            {
                string uuid = "";
                long size = 0;
                string aggr = "";
                string name_label = "";
                string name_description = "";
                bool pool_metadata_detected = false;

                foreach (XmlNode info in node.ChildNodes)
                {
                    if (info.Name.ToLowerInvariant() == "uuid")
                    {
                        uuid = info.InnerText.Trim();
                    }
                    else if (info.Name.ToLowerInvariant() == "size")
                    {
                        size = long.Parse(info.InnerText.Trim());
                    }
                    else if (info.Name.ToLowerInvariant() == "aggregate")
                    {
                        aggr = info.InnerText.Trim();
                    } 
                    else if (info.Name.ToLowerInvariant() == "name_label")
                    {
                        name_label = info.InnerText.Trim();
                    } 
                    else if (info.Name.ToLowerInvariant() == "name_description")
                    {
                        name_description = info.InnerText.Trim();
                    } 
                    else if (info.Name.ToLowerInvariant() == "pool_metadata_detected")
                    {
                        bool.TryParse(info.InnerText.Trim(), out pool_metadata_detected);
                    } 
                }
                results.Add(new SRInfo(uuid, size, aggr, name_label, name_description, pool_metadata_detected));
                /*if (aggr != "")
                    results.Add(new SRInfo(uuid, size, aggr));
                else
                    results.Add(new SRInfo(uuid, size));*/
            }
            return results;
        }

        public static List<string> ParseSupportedVersionsListXML(string xml)
        {
            var supportedVersionsResult = new List<string>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            // If we've got this from an async task result, then it will be wrapped
            // in a <value> element.  Parse the contents instead.
            foreach (XmlNode node in doc.GetElementsByTagName("value"))
            {
                xml = node.InnerText;
                doc = new XmlDocument();
                doc.LoadXml(xml);
                break;
            }


            foreach (XmlNode node in doc.GetElementsByTagName("SupportedVersions"))
            {
                foreach (XmlNode info in node.ChildNodes)
                {
                    if (info.Name.ToLowerInvariant() == "version")
                    {
                        supportedVersionsResult.Add(info.InnerText.Trim());
                    }
                }
            }

            return supportedVersionsResult;
        }

        public String GetScsiID()
        {
            foreach (PBD pbd in Connection.ResolveAll(PBDs))
            {
                if (!pbd.device_config.ContainsKey("SCSIid"))
                    continue;

                return pbd.device_config["SCSIid"];
            }

            if (!sm_config.ContainsKey("devserial"))
                return null;

            String SCSIid = sm_config["devserial"];

            if (SCSIid.StartsWith("scsi-"))
                SCSIid = SCSIid.Remove(0, 5);

            // CA-22352: SCSI IDs on the general panel for a NetApp SR have a trailing comma
            SCSIid = SCSIid.TrimEnd(new char[] { ',' });

            return SCSIid;
        }

        /// <summary>
        /// New Lun Per VDI mode (cf. LunPerVDI method) using the hba encapsulating type
        /// </summary>
        public virtual bool HBALunPerVDI()
        {
            return GetSRType(true) == SRTypes.rawhba;
        }

        /// <summary>
        /// Legacy LunPerVDI mode - for old servers that this was set up on
        /// This is no longer an option (2012) for newer servers but we need to keep this
        /// </summary>
        public bool LunPerVDI()
        {
            // Look for the mapping from scsi id -> vdi uuid 
            // in sm-config.  

            foreach (String key in sm_config.Keys)
                if (key.Contains("LUNperVDI") || key.StartsWith("scsi-"))
                    return true;

            return false;
        }

        private const String MPATH = "mpath";

        public Dictionary<VM, Dictionary<VDI, String>> GetMultiPathStatusLunPerVDI()
        {
            Dictionary<VM, Dictionary<VDI, String>> result =
                new Dictionary<VM, Dictionary<VDI, String>>();

            if (Connection == null)
                return result;

            foreach (PBD pbd in Connection.ResolveAll(PBDs))
            {
                if (!pbd.MultipathActive())
                    continue;

                foreach (KeyValuePair<String, String> kvp in pbd.other_config)
                {
                    if (!kvp.Key.StartsWith(MPATH))
                        continue;

                    int current;
                    int max;
                    if (!PBD.ParsePathCounts(kvp.Value, out current, out max))
                        continue;

                    String scsiIdKey = String.Format("scsi-{0}", kvp.Key.Substring(MPATH.Length + 1));
                    if (!sm_config.ContainsKey(scsiIdKey))
                        continue;

                    String vdiUUID = sm_config[scsiIdKey];
                    VDI vdi = null;

                    foreach (VDI candidate in Connection.ResolveAll(VDIs))
                    {
                        if (candidate.uuid != vdiUUID)
                            continue;

                        vdi = candidate;
                        break;
                    }

                    if (vdi == null)
                        continue;

                    foreach (VBD vbd in Connection.ResolveAll(vdi.VBDs))
                    {
                        VM vm = Connection.Resolve(vbd.VM);
                        if (vm == null)
                            continue;

                        if (vm.power_state != vm_power_state.Running)
                            continue;

                        if (!result.ContainsKey(vm))
                            result[vm] = new Dictionary<VDI, String>();

                        result[vm][vdi] = kvp.Value;
                    }
                }
            }

            return result;
        }

        public Dictionary<PBD, String> GetMultiPathStatusLunPerSR()
        {
            Dictionary<PBD, String> result =
                new Dictionary<PBD, String>();


            if (Connection == null)
                return result;

            foreach (PBD pbd in Connection.ResolveAll(PBDs))
            {
                if (!pbd.MultipathActive())
                    continue;

                String status = String.Empty;

                foreach (KeyValuePair<String, String> kvp in pbd.other_config)
                {
                    if (!kvp.Key.StartsWith(MPATH))
                        continue;

                    status = kvp.Value;
                    break;
                }

                int current;
                int max;
                if (!PBD.ParsePathCounts(status, out current, out max))
                    continue;

                result[pbd] = status;
            }

            return result;
        }

        public bool MultipathAOK()
        {
            if (!MultipathCapable())
                return true;

            if (LunPerVDI())
            {
                Dictionary<VM, Dictionary<VDI, String>>
                    multipathStatus = GetMultiPathStatusLunPerVDI();

                foreach (VM vm in multipathStatus.Keys)
                    foreach (VDI vdi in multipathStatus[vm].Keys)
                        if (!CheckMultipathString(multipathStatus[vm][vdi]))
                            return false;
            }
            else
            {
                Dictionary<PBD, String> multipathStatus =
                    GetMultiPathStatusLunPerSR();

                foreach (PBD pbd in multipathStatus.Keys)
                    if (pbd.MultipathActive() && !CheckMultipathString(multipathStatus[pbd]))
                        return false;
            }

            return true;
        }

        public override string NameWithLocation()
        {
            //return only the Name for local SRs
            if (Connection != null && !shared)
            {
                return Name();
            }

            return base.NameWithLocation();
        }

        internal override string LocationString()
        {
            var home = Home();
            return home != null ? home.LocationString() : base.LocationString();
        }

        private bool CheckMultipathString(String status)
        {
            int current;
            int max;
            if (!PBD.ParsePathCounts(status, out current, out max))
                return true;

            return current >= max;
        }

        public Dictionary<String, String> GetDeviceConfig(IXenConnection connection)
        {
            foreach (PBD pbd in connection.ResolveAll(PBDs))
                return pbd.device_config;

            return null;
        }

        public class SRInfo : IComparable<SRInfo>, IEquatable<SRInfo>
        {
            public readonly string UUID;
            public readonly long Size;
            public readonly string Aggr;
            public string Name;
            public string Description;
            public readonly bool PoolMetadataDetected;

            public SRInfo(string uuid)
                : this(uuid, 0, "", "", "", false)
            {
            }

            public SRInfo(string uuid, long size)
                : this(uuid, size, "", "", "", false)
            {
            }

            public SRInfo(string uuid, long size, string aggr)
                : this(uuid, size, aggr, "", "", false)
            {
            }

            public SRInfo(string uuid, long size, string aggr, string name, string description, bool poolMetadataDetected)
            {
                UUID = uuid;
                Size = size;
                Aggr = aggr;
                Name = name;
                Description = description;
                PoolMetadataDetected = poolMetadataDetected;
            }

            public int CompareTo(SRInfo other)
            {
                return (this.UUID.CompareTo(other.UUID));
            }

            public bool Equals(SRInfo other)
            {
                return this.CompareTo(other) == 0;
            }

            public override string ToString()
            {
                return UUID;
            }
        }

        public bool MultipathCapable()
        {
            SM relatedSm = GetSM();
            bool isAnSmCapability = relatedSm != null && relatedSm.MultipathEnabled();
            bool isInSmConfig = BoolKey(sm_config, "multipathable");
            return isInSmConfig || isAnSmCapability;
        }

        public string Target()
        {
            SR sr = Connection.Resolve(new XenRef<SR>(this.opaque_ref));
            if (sr == null)
                return String.Empty;

            foreach (PBD pbd in sr.Connection.ResolveAll(sr.PBDs))
            {
                SRTypes type = sr.GetSRType(false);
                if ((type == SR.SRTypes.netapp || type == SR.SRTypes.lvmoiscsi || type == SR.SRTypes.equal) && pbd.device_config.ContainsKey("target")) // netapp or iscsi
                {
                    return pbd.device_config["target"];
                }
                else if (type == SR.SRTypes.iso && pbd.device_config.ContainsKey("location")) // cifs or nfs iso
                {
                    String target = Helpers.HostnameFromLocation(pbd.device_config["location"]); // has form //ip_address/path
                    if (String.IsNullOrEmpty(target))
                        continue;

                    return target;
                }
                else if (type == SR.SRTypes.nfs && pbd.device_config.ContainsKey("server"))
                {
                    return pbd.device_config["server"];
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// The amount of memory used as compared to the available and allocated amounts as a friendly string
        /// </summary>
        public String SizeString()
        {
            return string.Format(Messages.SR_SIZE_USED,
                Util.DiskSizeString(physical_utilisation),
                Util.DiskSizeString(physical_size),
                Util.DiskSizeString(virtual_allocation));
        }

        /// <summary>
        /// A friendly string indicating whether the sr is detached/broken/multipath failing/needs upgrade/ok
        /// </summary>
        public String StatusString()
        {
            if (!HasPBDs())
                return Messages.DETACHED;

            if (IsDetached() || IsBroken())
                return Messages.GENERAL_SR_STATE_BROKEN;

            if (!MultipathAOK())
                return Messages.GENERAL_MULTIPATH_FAILURE;

            return Messages.GENERAL_STATE_OK;
        }

        /// <summary>
        /// Returns true when there is a pbd containing adapterid else false
        /// </summary>
        public bool CanRepairAfterUpgradeFromLegacySL()
        {
            if (type == "cslg")
            {
                var pbds = Connection.ResolveAll(PBDs);
                if (pbds != null)
                {
                    return pbds.Any(pbd => pbd.device_config.ContainsKey("adapterid"));
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Whether SR supports database replication.
        /// </summary>
        public static bool SupportsDatabaseReplication(IXenConnection connection, SR sr)
        {
            try
            {
                assert_supports_database_replication(connection.Session, sr.opaque_ref);
                return true;
            }
            catch (Failure)
            {
                return false;
            }
        }

        /// <summary>
        /// Is an iSL type or legacy iSl adpater type
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public static bool IsIslOrIslLegacy(SR sr)
        {
            SRTypes currentType = sr.GetSRType(true);
            return currentType == SRTypes.cslg || currentType == SRTypes.equal || currentType == SRTypes.netapp;
        }

        /// <summary>
        /// Whether the underlying SR backend supports SR_TRIM
        /// </summary>
        /// <returns></returns>
        public bool SupportsTrim()
        {
            System.Diagnostics.Trace.Assert(Connection != null, "Connection must not be null");

            SM sm = SM.GetByType(Connection, type);
            return sm != null && sm.features != null && sm.features.ContainsKey("SR_TRIM");
        }

        public long PercentageCommitted()
        {
            return (long)Math.Round(virtual_allocation/(double)physical_size*100.0);
        }

        #region IEquatable<SR> Members

        /// <summary>
        /// Indicates whether the current object is equal to the specified object. This calls the implementation from XenObject.
        /// This implementation is required for ToStringWrapper.
        /// </summary>
        public bool Equals(SR other)
        {
            return base.Equals(other);
        }

        #endregion
    }
}
