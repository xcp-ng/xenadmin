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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Management;
using System.Reflection;
using System.Text;

using XenOvf.Definitions;
using XenOvf.Definitions.VMC;
using XenOvf.Utilities;
using XenCenterLib.Compression;
using XenCenterLib.Archive;

namespace XenOvf
{
    public partial class OVF
    {
        private ManagementObject Win32_ComputerSystem = null;
        private List<ManagementObject> Win32_Processor = new List<ManagementObject>();
        private List<ManagementObject> Win32_CDROMDrive = new List<ManagementObject>();
        private List<ManagementObject> Win32_DiskDrive = new List<ManagementObject>();
        private List<ManagementObject> Win32_NetworkAdapter = new List<ManagementObject>();
        private List<ManagementObject> Win32_IDEController = new List<ManagementObject>();
        private List<ManagementObject> Win32_SCSIController = new List<ManagementObject>();
        private List<ManagementObject> Win32_IDEControllerDevice = new List<ManagementObject>();
        private List<ManagementObject> Win32_SCSIControllerDevice = new List<ManagementObject>();
        private List<ManagementObject> Win32_DiskPartition = new List<ManagementObject>();
        private List<ManagementObject> Win32_DiskDriveToDiskPartition = new List<ManagementObject>();

        private Dictionary<string, string> mappings = new Dictionary<string, string>();

        #region CONVERSIONS

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
             Justification = "Tar Object uses close not Dispose, it cleans up all streams used.")]
        public static void ConvertOVFtoOVA(string pathToOvf, string ovfFileName, bool compress, bool cleanup = true)
        {
            // throws exception if any of the parameters is null (which we want)
            string fullOvfPath = Path.Combine(pathToOvf, ovfFileName);
            
            if (!File.Exists(fullOvfPath))
                throw new FileNotFoundException(string.Format(Messages.FILE_MISSING, fullOvfPath));

            // File Order is:
            // 1. OVF File
            // 2. Manifest (if exists)
            // 3. Signature File (if exists)
            // 4. All files listed in References.File.

            string origDir = "";
            try
            {
                origDir = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(pathToOvf);

                EnvelopeType ovfenv = Load(ovfFileName);
                File_Type[] files = ovfenv.References.File;

                string ovafilename = string.Format("{0}.ova", Path.GetFileNameWithoutExtension(ovfFileName));
                string manifestfile = string.Format("{0}.mf", Path.GetFileNameWithoutExtension(ovfFileName));
                string signaturefile = string.Format("{0}.cert", Path.GetFileNameWithoutExtension(ovfFileName));

                #region COMPRESSION STREAM

                Stream ovaStream;
                if (compress)
                {
                    if (Properties.Settings.Default.useGZip)
                    {
                        log.Info("OVF.ConvertOVFtoOVA GZIP compression stream inserted");
                        FileStream fsStream = new FileStream(ovafilename + ".gz", FileMode.CreateNew, FileAccess.Write, FileShare.None);
                        ovaStream = CompressionFactory.Writer(CompressionFactory.Type.Gz, fsStream);
                    }
                    else
                    {
                        log.Info("OVF.ConvertOVFtoOVA BZIP2 compression stream inserted");
                        FileStream fsStream = new FileStream(ovafilename + ".bz2", FileMode.CreateNew, FileAccess.Write, FileShare.None);
                        ovaStream = CompressionFactory.Writer(CompressionFactory.Type.Bz2, fsStream);
                    }
                }
                else
                {
                    ovaStream = new FileStream(ovafilename, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                }

                #endregion

                #region TAR

                using (var tar = ArchiveFactory.Writer(ArchiveFactory.Type.Tar, ovaStream))
                {
                    AddFileToArchiveWriter(tar, ovfFileName, cleanup);

                    if (File.Exists(manifestfile))
                    {
                        AddFileToArchiveWriter(tar, manifestfile, cleanup);
                       
                        // Cannot exist without manifest file.
                        if (File.Exists(signaturefile))
                            AddFileToArchiveWriter(tar, signaturefile, cleanup);
                    }

                    if (files != null)
                    {
                        foreach (File_Type file in files)
                            AddFileToArchiveWriter(tar, file.href, cleanup);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                log.ErrorFormat("{0} {1}", Messages.CONVERSION_FAILED, ex.Message);
                throw new Exception(Messages.CONVERSION_FAILED, ex);
            }
            finally
            {
                Directory.SetCurrentDirectory(origDir);
            }
            log.Debug("OVF.ConvertOVFtoOVA completed");
        }

        private static void AddFileToArchiveWriter(ArchiveWriter tar, string fileName, bool cleanup)
        {
            using (FileStream fs = File.OpenRead(fileName))
                tar.Add(fs, fileName);

            log.InfoFormat("Added file {0} to OVA archive", fileName);

            if (cleanup)
            {
                try
                {
                    File.Delete(fileName);
                }
                catch
                {
                }
            }
        }

        public EnvelopeType ConvertPhysicaltoOVF(DiskInfo[] vhdExports, string pathToOvf, string ovfName)
        {
            return ConvertPhysicaltoOVF(vhdExports, pathToOvf, ovfName, Properties.Settings.Default.Language);
        }

        public EnvelopeType ConvertPhysicaltoOVF(DiskInfo[] vhdExports, string pathToOvf, string ovfName, string lang)
        {
            CollectInformation();
            var env = CreateEnvelope(ovfName, lang);
            string vmUuid = AddVirtualSystem(env, lang, ovfName);
            string vhsId = AddVirtualHardwareSection(env, vmUuid, lang);
            AddVssd(env, vmUuid, vhsId);
            AddCPUs(env, vmUuid);
            AddMemory(env, vmUuid);
            AddNetworks(env, vmUuid, lang);
            CreateConnectedDevices(env, vmUuid, vhdExports);

            #region CREATE PVP ENTRIES

            foreach (DiskInfo di in vhdExports)
            {
                string pvpFilename = string.Format(@"{0}.pvp", Path.GetFileNameWithoutExtension(di.VhdFileName));
                string pvpPathWithFilename = Path.Combine(pathToOvf, pvpFilename);
                if (File.Exists(pvpPathWithFilename))
                {
                    AddExternalFile(env, pvpFilename, null);
                }
            }

            #endregion

            FinalizeEnvelope(env);
            log.DebugFormat("OVF.Create completed, {0}", ovfName);
            return env;
        }

        /// <summary>
        /// Converts an ova.xml (version 2) file from an XenServer Export *.xva into and OVF xml string.
        /// </summary>
        /// <param name="vhdExports">DiskInfo[] an array of disk names / identifiers</param>
        /// <param name="ovaxml">ova.xml data in a string</param>
        /// <param name="ovfFilePath">OVF file Path</param>
        /// <param name="ovfName">Name of the OVF</param>
        /// <returns>OVF XML String</returns>
        public string ConvertXVAtoOVF(DiskInfo[] vhdExports, string ovaxml, string ovfFilePath, string ovfName)
        {
            return ConvertXVAtoOVF(vhdExports, ovaxml, ovfFilePath, ovfName, Properties.Settings.Default.Language);
        }

        /// <summary>
        /// Converts an ova.xml (version 2) file from an XenServer Export *.xva into and OVF xml string.
        /// </summary>
        /// <param name="vhdExports">DiskInfo[] an array of disk names / identifiers</param>
        /// <param name="ovaxml">ova.xml data in a string</param>
        /// <param name="ovfFilePath">OVF file Path</param>
        /// <param name="ovfName">Name of the OVF</param>
        /// <param name="lang">Language to use</param>
        /// <returns>OVF XML String</returns>
        public string ConvertXVAtoOVF(DiskInfo[] vhdExports, string ovaxml, string ovfFilePath, string ovfName, string lang)
        {
            XenXva xenobj = Tools.DeserializeOvaXml(ovaxml);
            EnvelopeType env = ConvertFromXenOVA(xenobj, vhdExports, ovfFilePath, ovfName, lang);
            string xmlstring = Tools.Serialize(env, typeof(EnvelopeType), Tools.LoadNamespaces());
            log.Debug("OVF.ConvertXVAtoOVF completed");
            return xmlstring;
        }

        /// <summary>
        /// Comverts an ova.xml V 0.1 from a XenServer 3 export into an OVF Xml String
        /// </summary>
        /// <param name="vhdExports">DiskInfo[] an array of disk names / identifiers</param>
        /// <param name="ovaxmlFileName">ova xml file name</param>
        /// <param name="ovfName">OVF Name</param>
        /// <returns>OVF Xml String</returns>
        public string ConvertXVAv1toOVF(DiskInfo[] vhdExports, string ovaxmlFileName, string ovfName)
        {
            return ConvertXVAv1toOVF(vhdExports, ovaxmlFileName, ovfName, Properties.Settings.Default.Language);
        }

        /// <summary>
        /// Comverts an ova.xml V 0.1 from a XenServer 3 export into an OVF Xml String
        /// </summary>
        /// <param name="vhdExports">DiskInfo[] an array of disk names / identifiers</param>
        /// <param name="ovaxmlFileName">ova xml file name</param>
        /// <param name="ovfName">OVF Name</param>
        /// <param name="lang"></param>
        /// <returns>OVF Xml String</returns>
        public string ConvertXVAv1toOVF(DiskInfo[] vhdExports, string ovaxmlFileName, string ovfName, string lang)
        {
            XcAppliance xca = Tools.LoadOldOvaXml(ovaxmlFileName);
            EnvelopeType env = ConvertFromXenOVAv1(xca, vhdExports, ovfName, lang);
            return Tools.Serialize(env, typeof(EnvelopeType), Tools.LoadNamespaces());
        }

        /// <summary>
        /// Convert Virtual PC configuration file to an OVF Xml string.
        /// </summary>
        /// <param name="vpcFileName">filename</param>
        /// <param name="ovfName">ovf name</param>
        /// <returns>xml ovf string</returns>
        public string ConvertVPCtoOVF(string vpcFileName, string ovfName)
        {
            return ConvertVPCtoOVF(vpcFileName, ovfName, Properties.Settings.Default.Language);
        }

        /// <summary>
        /// Convert Virtual PC configuration file to an OVF Xml string.
        /// </summary>
        /// <param name="vpcFileName">filename</param>
        /// <param name="ovfName">ovf name</param>
        /// <param name="lang">language string</param>
        /// <returns>xml ovf string</returns>
        public string ConvertVPCtoOVF(string vpcFileName, string ovfName, string lang)
        {
            string vpcstring = Tools.LoadFile(vpcFileName);
            vpcstring = vpcstring.Replace("utf-16", "utf-8").Replace("UTF-16", "UTF-8"); // fails if we don't do this. (not nice need to figure real answer)
            Ms_Vmc_Type xca = Tools.Deserialize<Ms_Vmc_Type>(vpcstring);
            EnvelopeType env = ConvertFromVPCXml(xca, Path.GetFileNameWithoutExtension(vpcFileName), ovfName, lang);
            return Tools.Serialize(env, typeof(EnvelopeType), Tools.LoadNamespaces());
        }

        /// <summary>
        /// Convert Vmware meta data to OVF xml string
        /// </summary>
        /// <param name="vmxFileName">filename</param>
        /// <param name="ovfName">ovf name.</param>
        /// <returns>ovf xml string</returns>
        public string ConvertVMXtoOVF(string vmxFileName, string ovfName)
        {
            return ConvertVMXtoOVF(vmxFileName, ovfName, Properties.Settings.Default.Language);
        }

        /// <summary>
        /// Convert Vmware meta data to OVF xml string
        /// </summary>
        /// <param name="vmxFileName">filename</param>
        /// <param name="ovfName">ovf name.</param>
        /// <param name="lang">language</param>
        /// <returns>ovf xml string</returns>
        public string ConvertVMXtoOVF(string vmxFileName, string ovfName, string lang)
        {
            string vmxstring = Tools.LoadFile(vmxFileName);
            StringReader sr = new StringReader(vmxstring);
            string line = sr.ReadLine();
            while (true)
            {
                if (!string.IsNullOrEmpty(line) && line.Contains("="))
                {
                    string[] vmxpair = line.Split(new char[] {'='});
                    mappings.Add(vmxpair[0].Trim(), vmxpair[1].Replace("\"", "").Trim());
                }
                try
                {
                    line = sr.ReadLine();
                    if (line == null) break;
                }
                catch
                {
                    break;
                }

            }
            sr.Dispose();
            EnvelopeType env = ConvertFromVMXcfg(mappings, ovfName, lang);
            return Tools.Serialize(env, typeof(EnvelopeType), Tools.LoadNamespaces());
        }

        /// <summary>
        /// Convert Hyper-V Export CIM XML vm meta data to OVF xml
        /// </summary>
        /// <param name="hvxmlFileName">filename</param>
        /// <param name="ovfName">ovfname</param>
        /// <returns>ovf xml string</returns>
        public string ConvertHyperVtoOVF(string hvxmlFileName, string ovfName)
        {
            return ConvertHyperVtoOVF(hvxmlFileName, ovfName, Properties.Settings.Default.Language);
        }

        /// <summary>
        /// Convert Hyper-V Export CIM XML vm meta data to OVF xml
        /// </summary>
        /// <param name="hvxmlFileName">filename</param>
        /// <param name="ovfName">ovfname</param>
        /// <param name="lang">language</param>
        /// <returns>ovf xml string</returns>
        public string ConvertHyperVtoOVF(string hvxmlFileName, string ovfName, string lang)
        {
            string hvxml = Tools.LoadFile(hvxmlFileName);
            hvxml = hvxml.Replace("utf-16", "utf-8"); // fails if we don't do this.
            string xmlstring = null;
            Ms_Declarations_Type hvobj = Tools.Deserialize<Ms_Declarations_Type>(hvxml);
            if (hvobj != null &&
                hvobj.declgroups != null &&
                hvobj.declgroups.Count > 0 &&
                hvobj.declgroups[0].values != null &&
                hvobj.declgroups[0].values.Count > 0)
            {
                EnvelopeType env = ConvertFromHyperVXml(hvobj, ovfName, lang);
                xmlstring = Tools.Serialize(env, typeof(EnvelopeType), Tools.LoadNamespaces());
                log.Debug("XenOvf::ConvertHyperVtoOVF completed");
            }
            else
            {
                throw new InvalidDataException(Messages.CONVERSION_NO_DATA);
            }
            return xmlstring;
        }

        #endregion

        #region PRIVATE

        private EnvelopeType ConvertFromXenOVA(XenXva xenxva, DiskInfo[] vhdExports, string ovfFilePath, string ovfname, string lang)
        {
            mappings.Clear();
            EnvelopeType env = CreateEnvelope(ovfname, lang);
            string vsId = AddVirtualSystem(env, lang, ovfname);
            string vhsId = AddVirtualHardwareSection(env, vsId, lang);
            // Do this because it isn't defined yet this will allow for 
            // it to update in any order.
            UpdateVirtualSystemSettingData(env, vsId, "VirtualSystemType", "hvm");
            foreach (XenMember xm in xenxva.xenstruct.xenmember)
            {
                if (xm.xenname.ToLower().Equals("objects"))
                {
                    foreach (object obj in ((XenArray)xm.xenvalue).xendata.xenvalue)
                    {
                        if (obj is XenStruct)
                        {
                            bool AtVM = false;
                            bool AtVBD = false;
                            bool AtVIF = false;
                            bool AtNetwork = false;
                            bool AtVDI = false;
                            bool AtSR = false;
                            bool IsReferenced = false;
                            string reference = null;
                            foreach (XenMember xmm in ((XenStruct)obj).xenmember)
                            {
                                #region SET AREA

                                if (xmm.xenname.ToLower().Equals("class") &&
                                    xmm.xenvalue != null &&
                                    xmm.xenvalue is string &&
                                    ((string)xmm.xenvalue).Length > 0 &&
                                    ((string)xmm.xenvalue).ToLower().Equals("vm"))
                                {
                                    AtVM = true;
                                }
                                else if (xmm.xenname.ToLower().Equals("class") &&
                                         xmm.xenvalue != null &&
                                         xmm.xenvalue is string &&
                                         ((string)xmm.xenvalue).Length > 0 &&
                                         ((string)xmm.xenvalue).ToLower().Equals("vbd"))
                                {
                                    AtVBD = true;
                                }

                                else if (xmm.xenname.ToLower().Equals("class") &&
                                         xmm.xenvalue != null &&
                                         xmm.xenvalue is string &&
                                         ((string)xmm.xenvalue).Length > 0 &&
                                         ((string)xmm.xenvalue).ToLower().Equals("vif"))
                                {
                                    AtVIF = true;
                                }
                                else if (xmm.xenname.ToLower().Equals("class") &&
                                         xmm.xenvalue != null &&
                                         xmm.xenvalue is string &&
                                         ((string)xmm.xenvalue).Length > 0 &&
                                         ((string)xmm.xenvalue).ToLower().Equals("network"))
                                {
                                    AtNetwork = true;
                                }
                                else if (xmm.xenname.ToLower().Equals("class") &&
                                         xmm.xenvalue != null &&
                                         xmm.xenvalue is string &&
                                         ((string)xmm.xenvalue).Length > 0 &&
                                         ((string)xmm.xenvalue).ToLower().Equals("vdi"))
                                {
                                    AtVDI = true;
                                }
                                else if (xmm.xenname.ToLower().Equals("class") &&
                                         xmm.xenvalue != null &&
                                         xmm.xenvalue is string &&
                                         ((string)xmm.xenvalue).Length > 0 &&
                                         ((string)xmm.xenvalue).ToLower().Equals("sr"))
                                {
                                    AtSR = true;
                                }

                                #endregion

                                #region CHECK REFERENCE

                                if (xmm.xenname.ToLower().Equals("id") &&
                                    xmm.xenvalue != null &&
                                    xmm.xenvalue is string)
                                {
                                    string curid = (string)(xmm.xenvalue);
                                    if (mappings.ContainsKey(curid))
                                    {
                                        IsReferenced = true;
                                        reference = curid;
                                    }
                                    else
                                    {
                                        IsReferenced = false;
                                    }
                                }

                                #endregion

                                #region GET DATA

                                if (xmm.xenname.ToLower().Equals("snapshot") &&
                                    xmm.xenvalue != null &&
                                    xmm.xenvalue is XenStruct)
                                {
                                    if (AtVM)
                                    {
                                        TransformXvaOvf_VM(env, vsId, lang, (XenStruct)xmm.xenvalue);
                                        AtVM = false;
                                    }
                                    else if (AtVBD)
                                    {
                                        TransformXvaOvf_VBD(env, vsId, lang, (XenStruct)xmm.xenvalue);
                                        AtVBD = false;
                                    }
                                    else if (AtVIF)
                                    {
                                        TransformXvaOvf_VIF(env, vsId, lang, (XenStruct)xmm.xenvalue);
                                        AtVIF = false;
                                    }
                                    else if (AtNetwork && IsReferenced)
                                    {
                                        TransformXvaOvf_Network(env, vsId, lang, reference, (XenStruct)xmm.xenvalue);
                                        AtNetwork = false;
                                    }
                                    else if (AtVDI && IsReferenced)
                                    {
                                        DiskInfo diskDetails = null;
                                        foreach (DiskInfo di in vhdExports)
                                        {
                                            if (di.DriveId.ToLower().Equals(reference.ToLower()))
                                            {
                                                diskDetails = di;
                                                break;
                                            }
                                        }
                                        if (diskDetails != null)
                                        {
                                            TransformXvaOvf_VDI(env, vsId, lang, reference, diskDetails, (XenStruct)xmm.xenvalue);
                                            string pvpFilename = string.Format(@"{0}.pvp", Path.GetFileNameWithoutExtension(diskDetails.VhdFileName));
                                            string pvpFileWithPath = Path.Combine(ovfFilePath, pvpFilename);
                                            if (File.Exists(pvpFileWithPath))
                                            {
                                                AddExternalFile(env, pvpFilename, null);
                                            }
                                        }
                                        else
                                        {
                                            log.ErrorFormat("Could not find the details for disk, {0}", reference);
                                        }
                                        AtVDI = false;
                                    }
                                    else if (AtSR && IsReferenced)
                                    {
                                        TransformXvaOvf_SR(env, vsId, lang, (XenStruct)xmm.xenvalue);
                                        AtSR = false;
                                    }
                                    IsReferenced = false;
                                }

                                #endregion
                            }
                        }
                    }
                }
            }
            FinalizeEnvelope(env);
            log.DebugFormat("OVF.ConvertFromXenOVA completed {0}", ovfname);
            mappings.Clear();
            return env;
        }

        private EnvelopeType ConvertFromXenOVAv1(XcAppliance xenxva, DiskInfo[] vhdExports, string ovfname, string lang)
        {
            mappings.Clear();
            if (ovfname == null ||
                xenxva == null ||
                xenxva.vm == null ||
                xenxva.vm.config == null ||
                xenxva.vm.hacks == null ||
                xenxva.vm.vbd == null ||
                xenxva.vm.label == null ||
                xenxva.vm.label.Length <= 0 ||
                vhdExports == null ||
                vhdExports.Length <= 0)
                throw new NullReferenceException("Invalid/Null argument");

            EnvelopeType env = CreateEnvelope(ovfname, lang);
            string vsId = AddVirtualSystem(env, lang, ovfname);
            string vhsId = AddVirtualHardwareSection(env, vsId, lang);
            ulong mem = 0;
            if (xenxva.vm.config.memset != 0)
            {
                mem = xenxva.vm.config.memset/MB;
                SetMemory(env, vsId, mem, "MB");
            }
            if (xenxva.vm.config.vcpus != 0)
            {
                SetCPUs(env, vsId, xenxva.vm.config.vcpus);
            }
            if (xenxva.vm.hacks.isHVM)
            {
                AddVirtualSystemSettingData(env, vsId, vhsId, xenxva.vm.label.Trim(), xenxva.vm.label.Trim(), _ovfrm.GetString("CONVERT_XVA_VSSD_CAPTION"), Guid.NewGuid().ToString(), Properties.Settings.Default.xenDefaultVirtualSystemType);
                AddOtherSystemSettingData(env, vsId, lang, "HVM_boot_policy", Properties.Settings.Default.xenBootOptions, _ovfrm.GetString("XENSERVER_SPECIFIC_DESCRIPTION"));
                AddOtherSystemSettingData(env, vsId, lang, "HVM_boot_params", Properties.Settings.Default.xenBootParams, _ovfrm.GetString("XENSERVER_SPECIFIC_DESCRIPTION"));
                AddOtherSystemSettingData(env, vsId, lang, "platform", Properties.Settings.Default.xenPlatformSetting, _ovfrm.GetString("XENSERVER_PLATFORM_DESCRIPTION"));

            }
            else
            {
                AddVirtualSystemSettingData(env, vsId, vhsId, xenxva.vm.label.Trim(), xenxva.vm.label.Trim(), _ovfrm.GetString("CONVERT_XVA_VSSD_CAPTION"), Guid.NewGuid().ToString(), Properties.Settings.Default.xenDefaultPVVirtualSystemType);
                AddOtherSystemSettingData(env, vsId, lang, "PV_bootloader", Properties.Settings.Default.xenPVBootloader, _ovfrm.GetString("XENSERVER_SPECIFIC_DESCRIPTION"));
                AddOtherSystemSettingData(env, vsId, lang, "PV_kernel", Properties.Settings.Default.xenKernelOptions, _ovfrm.GetString("XENSERVER_SPECIFIC_DESCRIPTION"));
                AddOtherSystemSettingData(env, vsId, lang, "platform", Properties.Settings.Default.xenPVPlatformSetting, _ovfrm.GetString("XENSERVER_PLATFORM_DESCRIPTION"));
            }
            UpdateVirtualSystemName(env, vsId, lang, xenxva.vm.label.Trim());
            AddNetwork(env, vsId, lang, _ovfrm.GetString("RASD_10_CAPTION"), _ovfrm.GetString("RASD_10_DESCRIPTION"), null);
            string contollerId = Guid.NewGuid().ToString();
            AddController(env, vsId, DeviceType.IDE, contollerId, 0);
            int i = 0;
            foreach (DiskInfo di in vhdExports)
            {
                if (di.PhysicalSize == null) di.PhysicalSize = "0";
                if (di.CapacitySize == null) di.CapacitySize = "0";
                AddDisk(env, vsId, di.DriveId, lang, di.VhdFileName, true, _ovfrm.GetString("RASD_19_CAPTION"), _ovfrm.GetString("RASD_19_DESCRIPTION"), Convert.ToUInt64(di.PhysicalSize), Convert.ToUInt64(di.CapacitySize));
                AddDeviceToController(env, vsId, di.DriveId, contollerId, Convert.ToString(i++));
            }

            FinalizeEnvelope(env);
            log.DebugFormat("OVF.ConvertFromXenOVAv1 completed {0}", ovfname);
            mappings.Clear();
            return env;
        }

        private EnvelopeType ConvertFromHyperVXml(Ms_Declarations_Type hvobj, string ovfname, string lang)
        {
            EnvelopeType env = CreateEnvelope(ovfname, lang);
            string systemId = AddVirtualSystem(env, lang, ovfname);
            string vhsid = AddVirtualHardwareSection(env, systemId, lang);


            foreach (Ms_WrapperInstance_Type wrap in hvobj.declgroups[0].values)
            {
                RASD_Type rasd = new RASD_Type();
                switch (wrap.instance.className)
                {
                        #region CASE: Msvm_VirtualSystemSettingData

                    case "Msvm_VirtualSystemSettingData":
                    {
                        string ElementName = null;
                        string InstanceId = null;
                        string SystemName = null;
                        string VirtualSystemType = null;
                        foreach (Ms_Property_Base_Type prop in wrap.instance.Properties)
                        {
                            switch (prop.Name)
                            {
                                case "ElementName":
                                {
                                    ElementName = ((Ms_ParameterValue_Type)prop).Value;
                                    break;
                                }
                                case "InstanceID":
                                {
                                    InstanceId = ((Ms_ParameterValue_Type)prop).Value;
                                    break;
                                }
                                case "SystemName":
                                {
                                    SystemName = ((Ms_ParameterValue_Type)prop).Value;
                                    break;
                                }
                                case "VirtualSystemType":
                                {
                                    VirtualSystemType = ((Ms_ParameterValue_Type)prop).Value;
                                    break;
                                }
                            }
                        }
                        AddVirtualSystemSettingData(env, systemId, vhsid, ElementName, ElementName, ElementName, InstanceId, VirtualSystemType);
                        UpdateVirtualSystemName(env, systemId, lang, ElementName);
                        break;
                    }

                        #endregion

                        #region CASE: ResourceAllocationSettingData

                    case "Msvm_ProcessorSettingData":
                    case "Msvm_MemorySettingData":
                    case "Msvm_SyntheticEthernetPortSettingData":
                    case "Msvm_ResourceAllocationSettingData":
                    {
                        foreach (Ms_Property_Base_Type prop in wrap.instance.Properties)
                        {
                            if (prop is Ms_ParameterValue_Type)
                            {
                                if (((Ms_ParameterValue_Type)prop).Value == null ||
                                    ((Ms_ParameterValue_Type)prop).Value.Length <= 0)
                                {
                                    continue;
                                }
                            }
                            else if (prop is Ms_ParameterValueArray_Type)
                            {
                                if (((Ms_ParameterValueArray_Type)prop).Values == null ||
                                    ((Ms_ParameterValueArray_Type)prop).Values.Length <= 0)
                                {
                                    continue;
                                }
                            }

                            PropertyInfo[] properties = rasd.GetType().GetProperties();
                            foreach (PropertyInfo pi in properties)
                            {
                                if (pi.Name.ToLower().Equals(prop.Name.ToLower()))
                                {
                                    object newvalue = null;
                                    if (prop is Ms_ParameterValue_Type)
                                    {
                                        switch (prop.Type.ToLower())
                                        {
                                            case "string":
                                            {
                                                newvalue = new cimString((string)((Ms_ParameterValue_Type)prop).Value);
                                                break;
                                            }
                                            case "boolean":
                                            {
                                                newvalue = new cimBoolean();
                                                ((cimBoolean)newvalue).Value = Convert.ToBoolean(((Ms_ParameterValue_Type)prop).Value);
                                                break;
                                            }
                                            case "uint16":
                                            {
                                                newvalue = new cimUnsignedShort();
                                                ((cimUnsignedShort)newvalue).Value = Convert.ToUInt16(((Ms_ParameterValue_Type)prop).Value);
                                                break;
                                            }
                                            case "uint32":
                                            {
                                                newvalue = new cimUnsignedInt();
                                                ((cimUnsignedInt)newvalue).Value = Convert.ToUInt32(((Ms_ParameterValue_Type)prop).Value);
                                                break;
                                            }
                                            case "uint64":
                                            {
                                                newvalue = new cimUnsignedLong();
                                                ((cimUnsignedLong)newvalue).Value = Convert.ToUInt64(((Ms_ParameterValue_Type)prop).Value);
                                                break;
                                            }
                                        }
                                    }
                                    else if (prop is Ms_ParameterValueArray_Type)
                                    {
                                        switch (prop.Type.ToLower())
                                        {
                                            case "string":
                                            {
                                                List<cimString> sarray = new List<cimString>();
                                                foreach (Ms_ParameterValue_Type svalue in ((Ms_ParameterValueArray_Type)prop).Values)
                                                {
                                                    sarray.Add(new cimString(svalue.Value));
                                                }
                                                newvalue = sarray.ToArray();
                                                break;
                                            }
                                        }
                                    }

                                    object tmpobject = null;
                                    switch (pi.Name.ToLower())
                                    {
                                        case "caption":
                                        {
                                            newvalue = new Caption(((cimString)newvalue).Value);
                                            break;
                                        }
                                        case "changeabletype":
                                        {
                                            tmpobject = newvalue;
                                            newvalue = new ChangeableType();
                                            ((ChangeableType)newvalue).Value = ((cimUnsignedShort)tmpobject).Value;
                                            break;
                                        }
                                        case "consumervisibility":
                                        {
                                            tmpobject = newvalue;
                                            newvalue = new ConsumerVisibility();
                                            ((ConsumerVisibility)newvalue).Value = ((cimUnsignedShort)tmpobject).Value;
                                            break;
                                        }
                                        case "mappingbehavior":
                                        {
                                            tmpobject = newvalue;
                                            newvalue = new MappingBehavior();
                                            ((MappingBehavior)newvalue).Value = ((cimUnsignedShort)tmpobject).Value;
                                            break;
                                        }
                                        case "resourcetype":
                                        {
                                            tmpobject = newvalue;
                                            newvalue = new ResourceType();
                                            ((ResourceType)newvalue).Value = ((cimUnsignedShort)tmpobject).Value;
                                            break;
                                        }
                                        case "connection":
                                        case "hostresource":
                                        default:
                                        {
                                            break;
                                        }

                                    }
                                    pi.SetValue(rasd, newvalue, null);
                                }
                            }
                        }
                        if (rasd != null)
                        {
                            if (FillEmptyRequiredFields(rasd))
                            {
                                if (rasd.ResourceType.Value == 21 &&
                                    rasd.Caption.Value.ToLower().StartsWith(_ovfrm.GetString("RASD_19_CAPTION").ToLower()))
                                {
                                    string filename = Path.GetFileName(rasd.Connection[0].Value);
                                    AddFileReference(env, lang, filename, rasd.InstanceID.Value, 0, Properties.Settings.Default.winFileFormatURI);
                                    AddRasd(env, systemId, rasd);
                                }
                                else if (rasd.ResourceType.Value == 10)
                                {
                                    AddNetwork(env, systemId, lang, rasd.InstanceID.Value, rasd.Caption.Value, rasd.Address.Value);
                                }
                                else
                                {
                                    AddRasd(env, systemId, rasd);
                                }
                            }
                        }
                        break;
                    }

                        #endregion

                        #region CASE: Msvm_VLANEndpointSettingData

                    case "Msvm_VLANEndpointSettingData":

                        #endregion

                        #region CASE: SKIPPED / DEFAULT

                    case "Msvm_VirtualSystemExportSettingData":
                    case "Msvm_VirtualSystemGlobalSettingData":
                    case "Msvm_HeartbeatComponentSettingData":
                    case "Msvm_KvpExchangeComponentSettingData":
                    case "Msvm_ShutdownComponentSettingData":
                    case "Msvm_TimeSyncComponentSettingData":
                    case "Msvm_VssComponentSettingData":
                    case "Msvm_SwitchPort":
                    case "Msvm_VirtualSwitch":
                    default:
                    {
                        break;
                    }

                        #endregion
                }
            }

            FinalizeEnvelope(env);
            return env;
        }

        private EnvelopeType ConvertFromVPCXml(Ms_Vmc_Type vmcobj, string vpcName, string ovfname, string lang)
        {
            EnvelopeType env = CreateEnvelope(ovfname, lang);
            string vsId = AddVirtualSystem(env, lang, ovfname);
            string vhsId = AddVirtualHardwareSection(env, vsId, lang);
            SetMemory(env, vsId, Convert.ToUInt64(vmcobj.hardware.memory.ram_size.value), "MB");
            // TODO: See if this is set in the other versions somewhere.
            SetCPUs(env, vsId, (ulong)1);
            AddVirtualSystemSettingData(env, vsId, vhsId, vpcName, vpcName, _ovfrm.GetString("CONVERT_VPC_VSSD_CAPTION"), Guid.NewGuid().ToString(), Properties.Settings.Default.xenDefaultVirtualSystemType);
            AddOtherSystemSettingData(env, vsId, lang, "HVM_boot_policy", Properties.Settings.Default.xenBootOptions, _ovfrm.GetString("XENSERVER_SPECIFIC_DESCRIPTION"));
            AddOtherSystemSettingData(env, vsId, lang, "HVM_boot_params", Properties.Settings.Default.xenBootParams, _ovfrm.GetString("XENSERVER_SPECIFIC_DESCRIPTION"));
            AddOtherSystemSettingData(env, vsId, lang, "platform", Properties.Settings.Default.xenPlatformSetting, _ovfrm.GetString("XENSERVER_PLATFORM_DESCRIPTION"));

            UpdateVirtualSystemName(env, vsId, lang, vpcName);

            if (vmcobj.hardware.pci_bus.ethernet_adapter != null &&
                vmcobj.hardware.pci_bus.ethernet_adapter.ethernet_controller != null &&
                vmcobj.hardware.pci_bus.ethernet_adapter.ethernet_controller.Length > 0)
            {
                foreach (Ms_Ethernet_Controller_Type ec in vmcobj.hardware.pci_bus.ethernet_adapter.ethernet_controller)
                {
                    if (ec.virtual_network.id.value == null ||
                        ec.virtual_network.name.value == null)
                    {
                        continue;
                    }
                    AddNetwork(env, vsId, lang, ec.virtual_network.id.value, ec.virtual_network.name.value, ec.ethernet_card_address.value);
                }
            }
            foreach (Ms_Ide_Controller_Type ide in vmcobj.hardware.pci_bus.ide_adapter.ide_controller)
            {
                string ideId = Guid.NewGuid().ToString();
                AddController(env, vsId, DeviceType.IDE, ideId, Convert.ToInt32(ide.id));
                foreach (Ms_Location_Type loc in ide.location)
                {
                    string diskId = Guid.NewGuid().ToString();
                    switch (loc.drive_type.value)
                    {
                        case "0":
                        {
                            continue;
                        } // when 0 location appears empty
                        case "1": // when 1 VHD
                        {
                            AddDisk(env, vsId, lang, diskId, Path.GetFileName(loc.pathname.absolute.value), true, _ovfrm.GetString("RASD_19_CAPTION"), _ovfrm.GetString("RASD_19_DESCRIPTION"), 0, 0);
                            break;
                        }
                        case "2": // when 2 ISO
                        {
                            AddCDROM(env, vsId, diskId, _ovfrm.GetString("RASD_16_CAPTION"), _ovfrm.GetString("RASD_16_ELEMENTNAME"));
                            if (loc.pathname != null &&
                                loc.pathname.absolute != null &&
                                !string.IsNullOrEmpty(loc.pathname.absolute.value))
                            {
                                AddFileReference(env, lang, Path.GetFileName(loc.pathname.absolute.value), diskId, 0, Properties.Settings.Default.isoFileFormatURI);
                                UpdateResourceAllocationSettingData(env, vsId, diskId, "HostResource", string.Format(Properties.Settings.Default.hostresource, diskId));
                            }
                            break;
                        }
                    }
                    AddDeviceToController(env, vsId, diskId, ideId, loc.id);
                }
            }

            FinalizeEnvelope(env);
            log.DebugFormat("OVF.ConvertFromVPCXml completed {0}", ovfname);
            mappings.Clear();
            return env;
        }

        private EnvelopeType ConvertFromVMXcfg(Dictionary<string, string> vmxcfg, string ovfname, string lang)
        {
            EnvelopeType env = CreateEnvelope(ovfname, lang);
            string vsId = AddVirtualSystem(env, lang, ovfname);
            string vhsId = AddVirtualHardwareSection(env, vsId);
            int addedNetwork = 0;
            int addedIDE = 0;
            int addedSCSI = 0;
            string vmxName = ovfname.Replace("\"", "");

            // just in case these are in vmx file:
            if (!vmxcfg.ContainsKey("numvcpus"))
            {
                SetCPUs(env, vsId, 1);
            }
            else
            {
                SetCPUs(env, vsId, Convert.ToUInt64("numvcpus"));
            }
            if (!vmxcfg.ContainsKey("memsize"))
            {
                SetMemory(env, vsId, 512, "MB");
            }
            else
            {
                SetMemory(env, vsId, Convert.ToUInt64(vmxcfg["memsize"]), "MB");
            }

            foreach (string key in vmxcfg.Keys)
            {
                if (key.ToLower().StartsWith("ethernet"))
                {
                    char i = key[8];
                    int networkIdx = (int)i - 48;
                    if (networkIdx == addedNetwork)
                    {
                        string mac = null;
                        string cap = "Network";
                        string datakey = string.Format("ethernet{0}.generatedAddress", networkIdx);
                        string vdev = string.Format("ethernet{0}.virtualDev", networkIdx);
                        if (vmxcfg.ContainsKey(datakey))
                            mac = vmxcfg[datakey];
                        if (vmxcfg.ContainsKey(vdev))
                            cap = vmxcfg[vdev];
                        AddNetwork(env, vsId, lang, Guid.NewGuid().ToString(), cap, mac);
                        addedNetwork++;
                    }
                }
                else if (key.ToLower().StartsWith("displayname"))
                {
                    vmxName = vmxcfg[key];
                }
                else if (key.ToLower().StartsWith("ide"))
                {
                    char i = key[3];
                    int ideIdx = (int)i - 48;
                    if (ideIdx == addedIDE)
                    {
                        string[] parts = key.Split(new char[] {':', '.'});
                        int port = 0;
                        if (parts.Length == 3)
                        {
                            port = Convert.ToInt32(parts[1]);
                        }
                        string devId = Guid.NewGuid().ToString();
                        string diskId = Guid.NewGuid().ToString();
                        AddController(env, vsId, DeviceType.IDE, devId, ideIdx);
                        string filekey = string.Format("ide{0}:{1}.fileName", ideIdx, port);
                        string devicekey = string.Format("ide{0}:{1}.deviceType", ideIdx, port);
                        bool addDiskImage = false;
                        if (vmxcfg.ContainsKey(filekey))
                            addDiskImage = true;
                        if (vmxcfg.ContainsKey(devicekey))
                        {
                            if (vmxcfg[devicekey].ToLower().StartsWith("cdrom"))
                            {
                                AddCDROM(env, vsId, diskId, _ovfrm.GetString("RASD_16_CAPTION"), _ovfrm.GetString("RASD_16_ELEMENTNAME"));
                                if (addDiskImage)
                                {
                                    AddFileReference(env, lang, Path.GetFileName(vmxcfg[filekey]), diskId, 0, Properties.Settings.Default.isoFileFormatURI);
                                    UpdateResourceAllocationSettingData(env, vsId, diskId, "HostResource", string.Format(Properties.Settings.Default.hostresource, diskId));
                                }
                            }
                            else
                            {
                                if (addDiskImage)
                                {
                                    AddDisk(env, vsId, diskId, lang, Path.GetFileName(vmxcfg[filekey]), true, _ovfrm.GetString("RASD_19_CAPTION"), _ovfrm.GetString("RASD_19_DESCRIPTION"), 0, 0);
                                }
                            }
                        }
                        else
                        {
                            if (addDiskImage)
                            {
                                AddDisk(env, vsId, diskId, lang, Path.GetFileName(vmxcfg[filekey]), true, _ovfrm.GetString("RASD_19_CAPTION"), _ovfrm.GetString("RASD_19_DESCRIPTION"), 0, 0);
                            }
                        }

                        AddDeviceToController(env, vsId, diskId, devId, Convert.ToString(port));
                        addedIDE++;
                    }
                }
                else if (key.ToLower().StartsWith("scsi"))
                {
                    char i = key[4];
                    int scsiIdx = (int)i - 48;
                    if (scsiIdx == addedSCSI)
                    {
                        string[] parts = key.Split(new char[] {':', '.'});
                        int port = 0;
                        if (parts.Length == 3)
                        {
                            port = Convert.ToInt32(parts[1]);
                        }
                        else
                        {
                            continue;
                        }
                        string devId = Guid.NewGuid().ToString();
                        string diskId = Guid.NewGuid().ToString();
                        AddController(env, vsId, DeviceType.SCSI, devId, port);
                        string filekey = string.Format("scsi{0}:{1}.fileName", scsiIdx, port);
                        string devicekey = string.Format("scsi{0}:{1}.deviceType", scsiIdx, port);
                        bool addDiskImage = false;
                        if (vmxcfg.ContainsKey(filekey))
                            addDiskImage = true;
                        if (vmxcfg.ContainsKey(devicekey))
                        {
                            if (vmxcfg[devicekey].ToLower().StartsWith("cdrom"))
                            {
                                AddCDROM(env, vsId, diskId, _ovfrm.GetString("RASD_16_CAPTION"), _ovfrm.GetString("RASD_16_ELEMENTNAME"));
                                if (addDiskImage)
                                {
                                    AddFileReference(env, lang, Path.GetFileName(vmxcfg[filekey]), diskId, 0, Properties.Settings.Default.isoFileFormatURI);
                                    UpdateResourceAllocationSettingData(env, vsId, diskId, "HostResource", string.Format(Properties.Settings.Default.hostresource, diskId));
                                }
                            }
                            else
                            {
                                if (addDiskImage)
                                {
                                    AddDisk(env, vsId, diskId, lang, Path.GetFileName(vmxcfg[filekey]), true, _ovfrm.GetString("RASD_19_CAPTION"), _ovfrm.GetString("RASD_19_DESCRIPTION"), 0, 0);
                                }
                            }
                        }
                        else
                        {
                            if (addDiskImage)
                            {
                                AddDisk(env, vsId, diskId, lang, Path.GetFileName(vmxcfg[filekey]), true, _ovfrm.GetString("RASD_19_CAPTION"), _ovfrm.GetString("RASD_19_DESCRIPTION"), 0, 0);
                            }
                        }
                        AddDeviceToController(env, vsId, diskId, devId, Convert.ToString(port));
                        addedSCSI++;
                    }
                }
            }
            AddVirtualSystemSettingData(env, vsId, vhsId, vmxName, vmxName, _ovfrm.GetString("CONVERT_VMX_VSSD_CAPTION"), Guid.NewGuid().ToString(), Properties.Settings.Default.xenDefaultVirtualSystemType);
            AddOtherSystemSettingData(env, vsId, lang, "HVM_boot_policy", Properties.Settings.Default.xenBootOptions, _ovfrm.GetString("XENSERVER_SPECIFIC_DESCRIPTION"));
            AddOtherSystemSettingData(env, vsId, lang, "HVM_boot_params", Properties.Settings.Default.xenBootParams, _ovfrm.GetString("XENSERVER_SPECIFIC_DESCRIPTION"));
            AddOtherSystemSettingData(env, vsId, lang, "platform", Properties.Settings.Default.xenPlatformSetting, _ovfrm.GetString("XENSERVER_PLATFORM_DESCRIPTION"));
            FinalizeEnvelope(env);
            log.DebugFormat("OVF.ConvertFromVMXcfg completed {0}", ovfname);
            mappings.Clear();
            return env;
        }

        private void TransformXvaOvf_VM(EnvelopeType env, string vsId, string lang, XenStruct vmstruct)
        {
            int PVvalue = 0;
            foreach (XenMember n in vmstruct.xenmember)
            {
                //
                // Currently these are the only values we care about.
                //
                if (n.xenname.ToLower().Equals("uuid"))
                {
                    UpdateVirtualSystemSettingData(env, vsId, "VirtualSystemIdentifier", (string)n.xenvalue);
                }
                if (n.xenname.ToLower().Equals("name_label"))
                {
                    if (n.xenvalue != null && n.xenvalue is string && ((string)n.xenvalue).Length > 0)
                    {
                        UpdateVirtualSystemName(env, vsId, lang, (string)n.xenvalue);
                        UpdateVirtualSystemSettingData(env, vsId, "ElementName", (string)n.xenvalue);
                        UpdateVirtualSystemSettingData(env, vsId, "Caption", _ovfrm.GetString("CONVERT_APP_NAME"));
                    }
                }
                else if (n.xenname.ToLower().Equals("memory_static_max"))
                {
                    if (n.xenvalue != null && n.xenvalue is string && ((string)n.xenvalue).Length > 0)
                    {
                        UInt64 memory = Convert.ToUInt64(n.xenvalue)/MB;
                        SetMemory(env, vsId, memory, "MB");
                    }
                }
                else if (n.xenname.ToLower().Equals("vcpus_max"))
                {
                    if (n.xenvalue != null && n.xenvalue is string && ((string)n.xenvalue).Length > 0)
                    {
                        UInt64 cpus = Convert.ToUInt64(n.xenvalue);
                        SetCPUs(env, vsId, cpus);
                    }
                }
                else if (
                    n.xenname.ToLower().Equals("pv_bootloader") ||
                    n.xenname.ToLower().Equals("pv_kernel") ||
                    n.xenname.ToLower().Equals("pv_ramdisk") ||
                    n.xenname.ToLower().Equals("pv_args") ||
                    n.xenname.ToLower().Equals("pv_bootloader_args") ||
                    n.xenname.ToLower().Equals("pv_legacy_args") ||
                    n.xenname.ToLower().Equals("hvm_boot_policy") ||
                    n.xenname.ToLower().Equals("hvm_shadow_multiplier")
                )
                {
                    if (n.xenvalue != null && n.xenvalue is string && ((string)n.xenvalue).Length > 0)
                    {
                        if (n.xenname.ToLower().StartsWith("pv"))
                        {
                            PVvalue++;
                        }
                        if (n.xenname.ToLower().StartsWith("hvm_boot"))
                        {
                            PVvalue--;
                        }
                        AddOtherSystemSettingData(env, vsId, lang, n.xenname, (string)n.xenvalue, _ovfrm.GetString("XENSERVER_SPECIFIC_DESCRIPTION"));
                    }
                }
                else if (n.xenname.ToLower().Equals("hvm_boot_params"))
                {

                    if (n.xenvalue != null && n.xenvalue is XenStruct && ((XenStruct)n.xenvalue).xenmember != null)
                    {
                        string bootorder = "dc";
                        foreach (XenMember xm in ((XenStruct)n.xenvalue).xenmember)
                        {
                            PVvalue--;
                            if (xm.xenname.ToLower().Equals("order"))
                            {
                                if (xm.xenvalue != null && xm.xenvalue is string && ((string)xm.xenvalue).Length > 0)
                                {
                                    bootorder = (string)xm.xenvalue;
                                }
                            }
                        }
                        AddOtherSystemSettingData(env, vsId, lang, n.xenname, bootorder, _ovfrm.GetString("XENSERVER_SPECIFIC_DESCRIPTION"));
                    }
                }
                else if (n.xenname.ToLower().Equals("platform"))
                {
                    if (n.xenvalue != null && n.xenvalue is XenStruct && ((XenStruct)n.xenvalue).xenmember != null)
                    {
                        StringBuilder p = new StringBuilder();
                        foreach (XenMember xm in ((XenStruct)n.xenvalue).xenmember)
                        {
                            if (xm.xenvalue != null && xm.xenvalue is string && ((string)xm.xenvalue).Length > 0)
                            {
                                p.AppendFormat("{0}={1}; ", xm.xenname, (string)xm.xenvalue);
                            }
                        }
                        AddOtherSystemSettingData(env, vsId, lang, n.xenname, p.ToString().Trim(), _ovfrm.GetString("XENSERVER_PLATFORM_DESCRIPTION"));
                    }
                }
                else if (n.xenname.ToLower().Equals("other_config"))
                {
                    // Ignored, why you say? Because I haven't found one that is 'required' to reproduce the vm, yet.                            
                }
                else if (n.xenname.ToLower().Equals("domarch"))
                {
                    // Can't depend on a value here but we need to set it to work correctly.
                    if (n.xenvalue != null && n.xenvalue is string && ((string)n.xenvalue).Length > 0)
                    {
                        string svalue = (string)n.xenvalue;

                        if (svalue.Equals("hvm"))
                        {
                            PVvalue = -10;
                        }
                    }

                    if (PVvalue < 0)
                    {
                        UpdateVirtualSystemSettingData(env, vsId, "VirtualSystemType", "hvm-3.0-unknown");
                    }
                    else
                    {
                        UpdateVirtualSystemSettingData(env, vsId, "VirtualSystemType", "xen-3.0-unknown");
                    }
                }
            }
            log.DebugFormat("OVF.TransformXvaOvf_VM completed {0}", vsId);
        }

        private void TransformXvaOvf_VBD(EnvelopeType env, string vsId, string lang, XenStruct vmstruct)
        {

            string diskId = null;
            string vhdfilename = "";
            bool bootable = false;
            string caption = null;
            string description = null;
            ulong filesize = 0;
            ulong capacity = 0;
            string vdiuuid = null;

            bool isDisk = false;

            StringBuilder connection = new StringBuilder();
            foreach (XenMember n in vmstruct.xenmember)
            {
                //
                // Currently these are the only values we care about.
                //
                if (n.xenname.ToLower().Equals("uuid") &&
                    n.xenvalue != null &&
                    n.xenvalue is string &&
                    ((string)n.xenvalue).Length > 0)
                {
                    vdiuuid = (string)n.xenvalue;
                    if (!vdiuuid.ToLower().Contains("null"))
                    {
                        diskId = string.Format(@"Xen:{0}/{1}", vsId, vdiuuid);
                    }
                    else
                    {
                        vdiuuid = null;
                    }
                }
                else if (n.xenname.ToLower().Equals("vdi") &&
                         n.xenvalue != null &&
                         n.xenvalue is string &&
                         ((string)n.xenvalue).Length > 0)
                {
                    string vdiref = ((string)n.xenvalue);
                    if (!vdiref.ToLower().Contains("null"))
                    {
                        if (!mappings.ContainsKey(vdiref))
                        {
                            mappings.Add(vdiref, vdiuuid);
                        }
                    }
                }
                else if (n.xenname.ToLower().Equals("userdevice") &&
                         n.xenvalue != null &&
                         n.xenvalue is string &&
                         ((string)n.xenvalue).Length > 0)
                {
                    connection.AppendFormat("device={0},", ((string)n.xenvalue));
                }
                else if (n.xenname.ToLower().Equals("bootable") &&
                         n.xenvalue != null &&
                         n.xenvalue is string &&
                         ((string)n.xenvalue).Length > 0)
                {
                    connection.AppendFormat("bootable={0},", ((string)n.xenvalue));
                    bootable = Convert.ToBoolean(((string)n.xenvalue));
                }
                else if (n.xenname.ToLower().Equals("mode") &&
                         n.xenvalue != null &&
                         n.xenvalue is string &&
                         ((string)n.xenvalue).Length > 0)
                {
                    connection.AppendFormat("mode={0},", ((string)n.xenvalue));
                }
                else if (n.xenname.ToLower().Equals("type") &&
                         n.xenvalue != null &&
                         n.xenvalue is string &&
                         ((string)n.xenvalue).Length > 0)
                {
                    switch (((string)n.xenvalue).ToUpper())
                    {
                        case "CD":
                        {
                            isDisk = false;
                            caption = _ovfrm.GetString("RASD_16_CAPTION");
                            description = _ovfrm.GetString("RASD_16_DESCRIPTION");
                            break;
                        }
                        case "DISK":
                        {
                            caption = _ovfrm.GetString("RASD_19_CAPTION");
                            description = _ovfrm.GetString("RASD_19_DESCRIPTION");
                            isDisk = true;
                            break;
                        }
                    }
                }
            }
            if (vdiuuid != null)
                connection.AppendFormat("vdi={0}", vdiuuid);

            if (isDisk)
            {
                AddDisk(env, vsId, diskId, lang, vhdfilename, bootable, caption, description, filesize, capacity);
                UpdateResourceAllocationSettingData(env, vsId, diskId, "Connection", connection.ToString());
            }
            else
            {
                AddCDROM(env, vsId, diskId, caption, description);
            }
            log.DebugFormat("OVF.TransformXvaOvf_VBD completed {0}", vsId);
        }

        private void TransformXvaOvf_VIF(EnvelopeType env, string vsId, string lang, XenStruct vmstruct)
        {
            string vifuuid = null;
            string mac = null;

            foreach (XenMember n in vmstruct.xenmember)
            {
                //
                // Currently these are the only values we care about.
                //
                if (n.xenname.ToLower().Equals("uuid") &&
                    n.xenvalue != null &&
                    n.xenvalue is string &&
                    ((string)n.xenvalue).Length > 0)
                {
                    vifuuid = (string)n.xenvalue;
                    if (vifuuid.ToLower().Contains("null"))
                    {
                        vifuuid = null;
                    }
                }
                else if (n.xenname.ToLower().Equals("network") &&
                         n.xenvalue != null &&
                         n.xenvalue is string &&
                         ((string)n.xenvalue).Length > 0)
                {
                    string netref = ((string)n.xenvalue);
                    if (!netref.ToLower().Contains("null"))
                    {
                        if (!mappings.ContainsKey(netref))
                        {
                            mappings.Add(netref, vifuuid);
                        }
                    }
                }
                else if (n.xenname.ToLower().Equals("mac") &&
                         n.xenvalue != null &&
                         n.xenvalue is string &&
                         ((string)n.xenvalue).Length > 0)
                {
                    mac = ((string)n.xenvalue);
                }
            }
            AddNetwork(env, vsId, lang, vifuuid, null, mac);
            log.DebugFormat("OVF.TransformXvaOvf_VIF completed {0}", vsId);
        }

        private void TransformXvaOvf_Network(EnvelopeType env, string vsId, string lang, string refId, XenStruct vmstruct)
        {
            string networkName = null;

            foreach (XenMember n in vmstruct.xenmember)
            {
                //
                // Currently these are the only values we care about.
                //
                if (n.xenname.ToLower().Equals("name_label") &&
                    n.xenvalue != null &&
                    n.xenvalue is string &&
                    ((string)n.xenvalue).Length > 0)
                {
                    networkName = ((string)n.xenvalue);
                    break;
                }
            }

            foreach (Section_Type section in env.Sections)
            {
                if (section is NetworkSection_Type)
                {
                    NetworkSection_Type ns = (NetworkSection_Type)section;
                    foreach (NetworkSection_TypeNetwork net in ns.Network)
                    {
                        if (net.Description != null &&
                            net.Description.msgid != null &&
                            net.Description.msgid.Equals(mappings[refId]))
                        {
                            net.Description.Value = networkName;
                            break;
                        }
                    }
                    break;
                }
            }
            log.DebugFormat("OVF.TransformXvaOvf_Network completed {0}", vsId);
        }

        private void TransformXvaOvf_VDI(EnvelopeType env, string vsId, string lang, string refId, DiskInfo di, XenStruct vmstruct)
        {
            string instanceId = null;
            if (mappings.ContainsKey(refId))
            {
                instanceId = string.Format(@"Xen:{0}/{1}", vsId, mappings[refId]);
            }
            else
            {
                instanceId = string.Format(@"Xen:{0}/{1}", vsId, Guid.NewGuid().ToString());
            }
            string description = null;
            string vhdfilename = di.VhdFileName;
            ulong filesize = 0;
            ulong capacity = 0;
            ulong freespace = 0;
            foreach (XenMember n in vmstruct.xenmember)
            {
                if (n.xenname.ToLower().Equals("virtual_size"))
                {
                    capacity = Convert.ToUInt64(n.xenvalue);
                }
                else if (n.xenname.ToLower().Equals("physical_utilisation"))
                {
                    filesize = Convert.ToUInt64(n.xenvalue);
                }
            }
            freespace = capacity - filesize;
            UpdateDisk(env, vsId, instanceId, description, vhdfilename, filesize, capacity, freespace);
            log.DebugFormat("OVF.TransformXvaOvf_VDI completed {0}", vsId);
        }

        private void TransformXvaOvf_SR(EnvelopeType env, string vsId, string lang, XenStruct vmstruct)
        {
            //
            // Currently this is data for the Environment not necessarily part of the VM
            // even if the VM is in this SR it may not exist in the target server therefore
            // it is not required in the OVF.
            //
            log.DebugFormat("OVF.TransformXvaOvf_SR completed {0}", vsId);
        }


        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
             Justification = "Logging only usage")]
        private void CollectInformation()
        {
            Win32_ComputerSystem = null;
            Win32_Processor.Clear();
            Win32_CDROMDrive.Clear();
            Win32_DiskDrive.Clear();
            Win32_NetworkAdapter.Clear();
            Win32_IDEController.Clear();
            Win32_SCSIController.Clear();
            Win32_SCSIControllerDevice.Clear();
            Win32_IDEControllerDevice.Clear();
            Win32_DiskPartition.Clear();


            ManagementObjectSearcher searcher = null;

            #region Win32_ComputerSystem

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_ComputerSystem");
                foreach (ManagementObject mgtobj in searcher.Get())
                {
                    Win32_ComputerSystem = mgtobj; // only want one.
                    break;
                }
                log.Debug("OVF.CollectionInformation Win32_ComputerSystem.1");
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call to Win32_ComputerSystem failed. Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

            #region Win32_Processor

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_Processor");
                foreach (ManagementObject mgtobj in searcher.Get())
                {
                    Win32_Processor.Add(mgtobj); // only want one.
                    break;
                }
                log.DebugFormat("OVF.CollectionInformation Win32_Processor.{0}", Win32_Processor.Count);
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call to Win32_Processor failed. Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

            #region Win32_CDROMDrive

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_CDROMDrive");
                foreach (ManagementObject mgtobj in searcher.Get())
                {
                    Win32_CDROMDrive.Add(mgtobj);
                }
                log.DebugFormat("OVF.CollectionInformation Win32_CDROMDrive.{0}", Win32_CDROMDrive.Count);
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call to Win32_CDROMDrive failed. Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

            #region Win32_DiskDrive

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_DiskDrive");
                foreach (ManagementObject mgtobj in searcher.Get())
                {
                    Win32_DiskDrive.Add(mgtobj);
                }
                log.DebugFormat("OVF.CollectionInformation Win32_DiskDrive.{0}", Win32_DiskDrive.Count);
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call to Win32_CDROMDrive failed. Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

            #region Win32_NetworkAdapter

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_NetworkAdapter");
                foreach (ManagementObject mgtobj in searcher.Get())
                {
                    Win32_NetworkAdapter.Add(mgtobj);
                }
                log.DebugFormat("OVF.CollectionInformation Win32_NetworkAdapter.{0}", Win32_NetworkAdapter.Count);
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call to Win32_NetworkAdapter failed. Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

            #region Win32_IDEController

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_IDEController");
                foreach (ManagementObject mgtobj in searcher.Get())
                {
                    Win32_IDEController.Add(mgtobj);
                }
                log.DebugFormat("OVF.CollectionInformation Win32_IDEController.{0}", Win32_IDEController.Count);
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call for Win32_IDEController failed. Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

            #region Win32_SCSIController

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_SCSIController");
                foreach (ManagementObject mgtobj in searcher.Get())
                {
                    Win32_SCSIController.Add(mgtobj);
                }
                log.DebugFormat("OVF.CollectionInformation Win32_SCSIController.{0}", Win32_SCSIController.Count);
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call for Win32_SCSIController failed. Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

            #region Win32_DiskPartition

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_DiskPartition");
                foreach (ManagementObject mgtobj in searcher.Get())
                {
                    Win32_DiskPartition.Add(mgtobj);
                }
                log.DebugFormat("OVF.CollectionInformation Win32_DiskPartition.{0}", Win32_DiskPartition.Count);
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call for Win32_DiskPartition failed. Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

            #region Win32_DiskDriveToDiskPartition

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_DiskDriveToDiskPartition");
                foreach (ManagementObject mgtobj in searcher.Get())
                {
                    Win32_DiskDriveToDiskPartition.Add(mgtobj);
                }
                log.DebugFormat("OVF.CollectionInformation Win32_DiskDriveToDiskPartition.{0}", Win32_DiskDriveToDiskPartition.Count);
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call for Win32_DiskDriveToDiskPartition failed, Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

        }

        private void AddVssd(EnvelopeType ovfEnv, string vsId, string vhsId)
        {
            AddVssd(ovfEnv, vsId, vhsId, Properties.Settings.Default.Language);
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
             Justification = "Logging mechanism")]
        private void AddVssd(EnvelopeType ovfEnv, string vsId, string vhsId, string lang)
        {

            if (Win32_ComputerSystem != null)
            {
                #region FIND BY PROPERTIES NOT EXPLICID

                string name = "Generic Computer";
                string caption = "Generic Caption";
                string description = "Autogenerated OVF/OVA Package";

                foreach (PropertyData pd in Win32_ComputerSystem.Properties)
                {
                    if (pd.Name.ToLower().Equals("name") && pd.Value != null)
                    {
                        name = (string)pd.Value;
                    }
                    else if (pd.Name.ToLower().Equals("caption") && pd.Value != null)
                    {
                        caption = (string)pd.Value;
                    }
                    else if (pd.Name.ToLower().Equals("description") && pd.Value != null)
                    {
                        description = (string)pd.Value;
                    }
                }

                #endregion

                UpdateVirtualSystemName(ovfEnv, vsId, lang, name);
                AddVirtualSystemSettingData(ovfEnv,
                    vsId,
                    vhsId,
                    name,
                    caption,
                    description,
                    Guid.NewGuid().ToString(),
                    "301"); // 301 == Microsoft (source), hvm-3.0-unknown == (xen source) Microsoft && Linux NOT PV'd, xen-3.0-unknown == PV'd

            }
            else
            {
                Random rand = new Random();
                string name = string.Format(Messages.AUTOGENERATED, rand.Next());
                string caption = string.Format(Messages.UNKNOWN);
                string description = string.Format(Messages.UNKNOWN);
                UpdateVirtualSystem(ovfEnv, vsId, lang, name);
                AddVirtualSystemSettingData(ovfEnv,
                    vsId,
                    vhsId,
                    name,
                    caption,
                    description,
                    Guid.NewGuid().ToString(),
                    "301");
                log.Warn("System definition not available, created defaults");
            }
            log.Debug("OVF.AddVssd completed");
        }

        private void AddNetworks(EnvelopeType ovfEnv, string vsId)
        {
            AddNetworks(ovfEnv, vsId, Properties.Settings.Default.Language);
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
             Justification = "Logging mechanism")]
        private void AddNetworks(EnvelopeType ovfEnv, string vsId, string lang)
        {
            if (Win32_NetworkAdapter != null)
            {
                foreach (ManagementObject mo in Win32_NetworkAdapter)
                {
                    // Only get the physical adapters, not logical (which there are numerous)
                    //if ((bool)mo["PhysicalAdapter"])
                    bool addThisNetwork = false;
                    string macaddress = null;
                    string description = null;
                    //
                    // setPriority is used to determine the description
                    // 0 = unset
                    // 1 highest priority
                    // 2 next
                    // 3 next
                    // ...
                    //
                    int setPriority = 0;
                    foreach (PropertyData pd in mo.Properties)
                    {
                        if (pd.Name != null && pd.Name.Length > 0)
                        {
                            if (pd.Name.ToLower().Equals("macaddress") && pd.Value != null && ((string)pd.Value).Length > 0)
                            {
                                macaddress = (string)pd.Value;
                            }
                            else if (pd.Name.ToLower().Equals("netconnectionid") && pd.Value != null && ((string)pd.Value).Length > 0)
                            {
                                description = (string)pd.Value;
                                setPriority = 1;
                            }
                            else if (pd.Name.ToLower().Equals("name") && pd.Value != null && ((string)pd.Value).Length > 0)
                            {
                                if (setPriority == 0 || setPriority > 2)
                                {
                                    description = (string)pd.Value;
                                    setPriority = 2;
                                }
                            }
                            else if (pd.Name.ToLower().Equals("description") && pd.Value != null && ((string)pd.Value).Length > 0)
                            {
                                if (setPriority == 0 || setPriority > 3)
                                {
                                    description = (string)pd.Value;
                                    setPriority = 3;
                                }
                            }
                            // Below is trying to figure out if this is a Network Connection that
                            // is to be exported/defined.
                            // The issue is WMI has different value sets for different types of hardware
                            // such as hardware as we know it... pci ethernet
                            // or blade style, which WMI gives a different result.
                            // WAN/RAS connections.. etc.
                            // ANY one of the values can set this to true:
                            //      netconnectionstatus
                            //      pnpdeviceid
                            //      physicaladapter
                            //
                            else if (pd.Name.ToLower().Equals("netconnectionstatus") && pd.Value != null)
                            {
                                if ((UInt16)pd.Value == 0x2)
                                {
                                    addThisNetwork = true;
                                }
                            }
                            else if (pd.Name.ToLower().Equals("pnpdeviceid") && pd.Value != null && ((string)pd.Value).Length > 0)
                            {
                                if ((((string)pd.Value).ToLower().StartsWith("pci") || ((string)pd.Value).ToLower().StartsWith("scsi")))
                                {
                                    addThisNetwork = true;
                                }
                            }
                            else if (pd.Name.ToLower().Equals("physicaladapter") && pd.Value != null)
                            {
                                addThisNetwork = (bool)pd.Value;
                            }
                        }
                    }
                    if (addThisNetwork)
                    {
                        AddNetwork(ovfEnv, vsId, lang, Guid.NewGuid().ToString(), description, macaddress);
                    }
                }
            }
            else
            {
                log.Warn("No networks defined, If a network interface is required, the administrator will need to add it after import of OVF/OVA Package.");
            }
            log.DebugFormat("OVF.AddNetworks completed {0}", vsId);
        }

        private void AddCPUs(EnvelopeType ovfEnv, string vsId)
        {
            AddCPUs(ovfEnv, vsId, Properties.Settings.Default.Language);
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
             Justification = "Logging mechanism")]
        private void AddCPUs(EnvelopeType ovfEnv, string vsId, string lang)
        {
            UInt64 cpucount = 0;
            if (Win32_Processor != null && Win32_Processor.Count > 0)
            {
                foreach (ManagementObject mo in Win32_Processor)
                {
                    #region FIND BY PROPERTIES NOT EXPLICID

                    uint numberofcores = 1;

                    foreach (PropertyData pd in mo.Properties)
                    {
                        if (pd.Name.ToLower().Equals("numberofcores") && pd.Value != null)
                        {
                            numberofcores = (uint)pd.Value;
                        }
                    }

                    #endregion

                    cpucount += Convert.ToUInt64(numberofcores);
                }

                SetCPUs(ovfEnv, vsId, cpucount);
            }
            else
            {
                SetCPUs(ovfEnv, vsId, 1);
                log.Warn("OVF.AddCPUs, set using default (1) CPU");
            }
            log.DebugFormat("OVF.AddCPUs completed {0} cpus {1}", vsId, cpucount);
        }

        private void AddMemory(EnvelopeType ovfEnv, string vsId)
        {
            AddMemory(ovfEnv, vsId, Properties.Settings.Default.Language);
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
             Justification = "Logging mechanism")]
        private void AddMemory(EnvelopeType ovfEnv, string vsId, string lang)
        {
            ulong divisor = 1024*1024;
            ulong totalphysicalmemory = divisor*512; // 512MB Default Memory
            ulong memory = 0;

            if (Win32_ComputerSystem != null)
            {
                #region FIND BY PROPERTIES NOT EXPLICID

                foreach (PropertyData pd in Win32_ComputerSystem.Properties)
                {
                    if (pd.Name.ToLower().Equals("totalphysicalmemory") && pd.Value != null)
                    {
                        totalphysicalmemory = (ulong)pd.Value;
                        break;
                    }
                }

                #endregion

                memory = totalphysicalmemory/divisor;
            }
            else
            {
                log.Warn("OVF.AddMemory: could not determine system memory, defaulting to 512MB");
                memory = 512L;
            }
            SetMemory(ovfEnv, vsId, memory, "byte * 2^20");
            log.DebugFormat("OVF.AddMemory completed {0} memory {1} (byte * 2 ^ 20)", vsId, memory);
        }

        private void CreateConnectedDevices(EnvelopeType ovfEnv, string vsId, DiskInfo[] vhdExports)
        {
            CreateConnectedDevices(ovfEnv, vsId, Properties.Settings.Default.Language, vhdExports);
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
             Justification = "Logging mechanism")]
        private void CreateConnectedDevices(EnvelopeType ovfEnv, string vsId, string lang, DiskInfo[] vhdExports)
        {
            //VirtualHardwareSection_Type vhs = FindVirtualHardwareSection(ovfEnv, vsId);
            bool guessPosition = true;
            int i = 0;

            #region IDE

            if (Win32_IDEController != null && Win32_IDEController.Count > 0)
            {
                foreach (ManagementObject mo in Win32_IDEController)
                {
                    #region FIND BY PROPERTIES NOT EXPLICID

                    string deviceid = null;
                    foreach (PropertyData pd in mo.Properties)
                    {
                        if (pd.Name.ToLower().Equals("deviceid") && pd.Value != null)
                        {
                            deviceid = (string)pd.Value;
                        }
                    }

                    #endregion

                    if (deviceid == null)
                    {
                        log.Debug("No device id defined, continuing");
                        continue;
                    }
                    List<ManagementObject> ControllerAssociations = FindDeviceReferences("Win32_IDEControllerDevice", deviceid);
                    string controllerInstanceId = Guid.NewGuid().ToString();

                    AddController(ovfEnv, vsId, DeviceType.IDE, controllerInstanceId, i++);
                    foreach (ManagementObject ca in ControllerAssociations)
                    {
                        #region FIND BY PROPERTIES NOT EXPLICID

                        string _dependent = null;
                        foreach (PropertyData pd in ca.Properties)
                        {
                            if (pd.Name.ToLower().Equals("dependent") && pd.Value != null)
                            {
                                _dependent = (string)pd.Value;
                            }
                        }
                        if (_dependent == null)
                        {
                            log.Debug("PCI Association not available, continuing.");
                            continue;
                        }

                        #endregion

                        string[] dependent = _dependent.Split(new char[] {'='});
                        string dependentId = dependent[dependent.Length - 1].Replace("\"", "");
                        dependentId = dependentId.Replace(@"\", "");
                        string startswith = dependentId; //.Replace(@"\", "");

                        if (startswith.ToUpper().StartsWith(@"IDEDISK"))
                        {
                            log.Debug("OVF.CreateConnectedDevices Checking IDEDISK");
                            foreach (ManagementObject md in Win32_DiskDrive)
                            {
                                #region FIND BY PROPERTIES NOT EXPLICID

                                string _deviceid = null;
                                string _pnpdeviceid = null;
                                UInt64 _size = 0;
                                foreach (PropertyData pd in md.Properties)
                                {
                                    if (pd.Name.ToLower().Equals("deviceid") && pd.Value != null)
                                    {
                                        _deviceid = (string)pd.Value;
                                    }
                                    else if (pd.Name.ToLower().Equals("pnpdeviceid") && pd.Value != null)
                                    {
                                        _pnpdeviceid = (string)pd.Value;
                                    }
                                    else if (pd.Name.ToLower().Equals("size") && pd.Value != null)
                                    {
                                        _size = (UInt64)pd.Value;
                                    }

                                }

                                #endregion

                                _pnpdeviceid = _pnpdeviceid.Replace(@"\", "");
                                if (_pnpdeviceid.Equals(dependentId))
                                {
                                    foreach (DiskInfo di in vhdExports)
                                    {
                                        if (_deviceid.Contains(di.DriveId))
                                        {
                                            try
                                            {
                                                log.DebugFormat("OVF.CreateConnectedDevices: Dependent: {0}  Device: {1}", dependentId, _deviceid);
                                                string diskInstanceId = Guid.NewGuid().ToString();
                                                int lastAmp = dependentId.LastIndexOf('&');
                                                if (lastAmp < 0) lastAmp = 0;
                                                string[] tmp = dependentId.Substring(lastAmp + 1).Split(new char[] {'.'});
                                                string address = null;
                                                if (tmp.Length >= 2)
                                                {
                                                    address = tmp[1];
                                                }
                                                else
                                                {
                                                    address = (guessPosition) ? "0" : "1";
                                                    guessPosition = !guessPosition;
                                                }
                                                address = address.Replace("&", "_");
                                                bool bootable = IsBootDisk(di.DriveId);
                                                AddDisk(ovfEnv, vsId, diskInstanceId, lang, di.VhdFileName, bootable,
                                                    _ovfrm.GetString("RASD_19_CAPTION"),
                                                    _ovfrm.GetString("RASD_19_DESCRIPTION"),
                                                    Convert.ToUInt64(di.PhysicalSize), Convert.ToUInt64(di.CapacitySize));
                                                AddDeviceToController(ovfEnv, vsId, diskInstanceId, controllerInstanceId, address);
                                                di.Added = true;
                                                log.DebugFormat("OVF.CreateConnectedDevices: {0} ({1}) added to {2}", di.DriveId, di.VhdFileName, dependentId);
                                            }
                                            catch (Exception ex)
                                            {
                                                string msg = string.Format("{0} [{1}] controller connection could not be identified.", "IDEDISK", _pnpdeviceid);
                                                log.ErrorFormat("OVF.CreateConnectedDevices: {0}", msg);
                                                throw new Exception(msg, ex);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (startswith.ToUpper().StartsWith(@"IDECDROM"))
                        {
                            log.Debug("OVF.CreateConnectedDevices Checking IDECDROM");
                            foreach (ManagementObject md in Win32_CDROMDrive)
                            {
                                #region FIND BY PROPERTIES NOT EXPLICID

                                string _pnpdeviceid = null;
                                foreach (PropertyData pd in md.Properties)
                                {
                                    if (pd.Name.ToLower().Equals("pnpdeviceid") && pd.Value != null)
                                    {
                                        _pnpdeviceid = (string)pd.Value;
                                    }
                                }
                                if (_pnpdeviceid == null)
                                {
                                    log.Debug("PNPDeviceID not available, continuing.");
                                    continue;
                                }

                                #endregion

                                _pnpdeviceid = _pnpdeviceid.Replace(@"\", "");
                                if (_pnpdeviceid.Equals(dependentId))
                                {
                                    log.DebugFormat("OVF.CreateConnectedDevices: Dependent: {0}  Device: {1}", dependentId, _pnpdeviceid);
                                    try
                                    {
                                        string diskInstanceId = Guid.NewGuid().ToString();
                                        int lastAmp = dependentId.LastIndexOf('&');
                                        if (lastAmp < 0) lastAmp = 0;
                                        string[] tmp = dependentId.Substring(lastAmp + 1).Split(new char[] {'.'});
                                        //string[] tmp = dependentId.Split(new char[] { '.' });
                                        string address = tmp[1];
                                        int idetest = Convert.ToInt32(address);
                                        if (idetest != 0 && idetest != 1)
                                        {
                                            address = "0";
                                        }
                                        AddCDROM(ovfEnv, vsId, diskInstanceId, _ovfrm.GetString("RASD_16_CAPTION"), _ovfrm.GetString("RASD_16_ELEMENTNAME"));
                                        AddDeviceToController(ovfEnv, vsId, diskInstanceId, controllerInstanceId, address);
                                        log.DebugFormat("OVF.CreateConnectedDevices: CDROM added to {0}", dependentId);
                                    }
                                    catch
                                    {
                                        log.WarnFormat("OVF.CreateConnectedDevices: CDROM [{0}] controller connection could not be identified, skipped.", _pnpdeviceid);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                log.Info("OVF.CreateConnectedDevices NO IDE controllers detected.");
            }
            log.Debug("OVF.CreateConnectedDevices IDE Controllers completed.");

            #endregion

            #region SCSI

            if (Win32_SCSIController != null && Win32_SCSIController.Count > 0)
            {
                foreach (ManagementObject device in Win32_SCSIController)
                {
                    #region FIND BY PROPERTIES NOT EXPLICID

                    string _deviceid = null;
                    foreach (PropertyData pd in device.Properties)
                    {
                        if (pd.Name.ToLower().Equals("deviceid") && pd.Value != null)
                        {
                            _deviceid = (string)pd.Value;
                        }
                    }
                    if (_deviceid == null)
                    {
                        log.Debug("SCSI DeviceID not available, continuing.");
                        continue;
                    }

                    #endregion

                    List<ManagementObject> ControllerAssociations = FindDeviceReferences("Win32_SCSIControllerDevice", _deviceid);
                    string controllerInstanceId = Guid.NewGuid().ToString();

                    if (ControllerAssociations == null || ControllerAssociations.Count <= 0)
                    {
                        log.DebugFormat("No Controller associations for {0}", _deviceid);
                        continue;
                    }

                    AddController(ovfEnv, vsId, DeviceType.SCSI, controllerInstanceId, i++);
                    foreach (ManagementObject ca in ControllerAssociations)
                    {
                        #region FIND BY PROPERTIES NOT EXPLICID

                        string _dependent = null;
                        foreach (PropertyData pd in ca.Properties)
                        {
                            if (pd.Name.ToLower().Equals("dependent") && pd.Value != null)
                            {
                                _dependent = (string)pd.Value;
                            }
                        }
                        if (_dependent == null)
                        {
                            log.Debug("SCSI Association not available, continuing.");
                            continue;
                        }

                        #endregion

                        string[] dependent = _dependent.Split(new char[] {'='});
                        string dependentId = dependent[dependent.Length - 1].Replace("\"", "");
                        dependentId = dependentId.Replace(@"\", "");
                        string startswith = dependentId; //.Replace(@"\", "");

                        if (startswith.ToUpper().StartsWith(@"SCSIDISK"))
                        {
                            foreach (ManagementObject md in Win32_DiskDrive)
                            {
                                #region FIND BY PROPERTIES NOT EXPLICID

                                string __deviceid = null;
                                string __pnpdeviceid = null;
                                UInt32 __scsibus = 0;
                                UInt16 __scsilogicalunit = 0;
                                UInt16 __scsiport = 0;
                                UInt16 __scsitargetid = 0;
                                UInt64 __size = 0;
                                foreach (PropertyData pd in md.Properties)
                                {
                                    if (pd.Name.ToLower().Equals("deviceid") && pd.Value != null)
                                    {
                                        __deviceid = (string)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("pnpdeviceid") && pd.Value != null)
                                    {
                                        __pnpdeviceid = (string)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("scsibus") && pd.Value != null)
                                    {
                                        __scsibus = (UInt32)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("scsilogicalunit") && pd.Value != null)
                                    {
                                        __scsilogicalunit = (UInt16)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("scsiport") && pd.Value != null)
                                    {
                                        __scsiport = (UInt16)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("scsitargetid") && pd.Value != null)
                                    {
                                        __scsitargetid = (UInt16)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("size") && pd.Value != null)
                                    {
                                        __size = (UInt64)pd.Value;
                                    }
                                }
                                if (__deviceid == null)
                                {
                                    log.Debug("SCSI DeviceID not available, continuing.");
                                    continue;
                                }

                                #endregion

                                __pnpdeviceid = __pnpdeviceid.Replace(@"\", "");
                                if (__pnpdeviceid.Equals(dependentId))
                                {
                                    foreach (DiskInfo di in vhdExports)
                                    {
                                        if (__deviceid.Contains(di.DriveId))
                                        {
                                            string diskInstanceId = Guid.NewGuid().ToString();
                                            string _description = string.Format(_ovfrm.GetString("RASD_CONTROLLER_SCSI_DESCRIPTION"), __scsibus, __scsilogicalunit, __scsiport, __scsitargetid);
                                            bool bootable = IsBootDisk(di.DriveId);
                                            AddDisk(ovfEnv, vsId, diskInstanceId, lang, di.VhdFileName, bootable, _ovfrm.GetString("RASD_19_CAPTION"), _description, Convert.ToUInt64(di.PhysicalSize), Convert.ToUInt64(di.CapacitySize));
                                            AddDeviceToController(ovfEnv, vsId, diskInstanceId, controllerInstanceId, Convert.ToString(__scsiport));
                                            di.Added = true;
                                            log.DebugFormat("CreateConnectedDevices: {0} ({1}) added to {2}", di.DriveId, di.VhdFileName, dependentId);
                                        }
                                    }
                                }
                            }
                        }
                        else if (startswith.ToUpper().StartsWith(@"SCSICDROM"))
                        {
                            foreach (ManagementObject md in Win32_CDROMDrive)
                            {
                                #region FIND BY PROPERTIES NOT EXPLICID

                                string __deviceid = null;
                                string __pnpdeviceid = null;
                                UInt32 __scsibus = 0;
                                UInt16 __scsilogicalunit = 0;
                                UInt16 __scsiport = 0;
                                UInt16 __scsitargetid = 0;
                                foreach (PropertyData pd in md.Properties)
                                {
                                    if (pd.Name.ToLower().Equals("deviceid") && pd.Value != null)
                                    {
                                        __deviceid = (string)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("pnpdeviceid") && pd.Value != null)
                                    {
                                        __pnpdeviceid = (string)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("scsibus") && pd.Value != null)
                                    {
                                        __scsibus = (UInt32)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("scsilogicalunit") && pd.Value != null)
                                    {
                                        __scsilogicalunit = (UInt16)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("scsiport") && pd.Value != null)
                                    {
                                        __scsiport = (UInt16)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("scsitargetid") && pd.Value != null)
                                    {
                                        __scsitargetid = (UInt16)pd.Value;
                                    }
                                }
                                if (__deviceid == null)
                                {
                                    log.Debug("SCSI DeviceID not available, continuing.");
                                    continue;
                                }

                                #endregion

                                __pnpdeviceid = __pnpdeviceid.Replace(@"\", "");
                                if (__pnpdeviceid.Equals(dependentId))
                                {
                                    string diskInstanceId = Guid.NewGuid().ToString();
                                    string caption = string.Format(_ovfrm.GetString("RASD_CONTROLLER_SCSI_DESCRIPTION"), __scsibus, __scsilogicalunit, __scsiport, __scsitargetid);
                                    AddCDROM(ovfEnv, vsId, diskInstanceId, caption, _ovfrm.GetString("RASD_16_DESCRIPTION"));
                                    AddDeviceToController(ovfEnv, vsId, diskInstanceId, controllerInstanceId, Convert.ToString(__scsiport));
                                    log.DebugFormat("CreateConnectedDevices: CDROM added to {0}", dependentId);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                log.Info("OVF.CreateConnectedDevices no SCSI Controllers detected.");
            }
            log.DebugFormat("OVF.CreateConnectedDevices SCSI Controllers completed {0} ", vsId);

            #endregion

            #region OTHER CONTROLLER DISKS

            // These are disks that were not found on an IDE or SCSI, but exist and want to be exported.
            // these could be USB, 1394 etc.
            foreach (DiskInfo di in vhdExports)
            {
                if (!di.Added)
                {
                    UInt64 _size = 0;
                    string diskInstanceId = Guid.NewGuid().ToString();
                    string _deviceid = null;
                    string _mediatype = null;
                    foreach (ManagementObject md in Win32_DiskDrive)
                    {
                        #region FIND BY PROPERTIES NOT EXPLICID

                        foreach (PropertyData pd in md.Properties)
                        {
                            if (pd.Name.ToLower().Equals("deviceid") && pd.Value != null)
                            {
                                _deviceid = (string)pd.Value;
                            }
                            else if (pd.Name.ToLower().Equals("mediatype") && pd.Value != null)
                            {
                                _mediatype = (string)pd.Value;
                            }
                            else if (pd.Name.ToLower().Equals("size") && pd.Value != null)
                            {
                                _size = (UInt64)pd.Value;
                            }

                        }

                        #endregion
                    }

                    bool bootable = IsBootDisk(di.DriveId);
                    AddDisk(ovfEnv, vsId, diskInstanceId, lang, di.VhdFileName, bootable, _ovfrm.GetString("RASD_19_CAPTION"), _mediatype, Convert.ToUInt64(di.PhysicalSize), Convert.ToUInt64(di.CapacitySize));
                    di.Added = true;
                    log.DebugFormat("CreateConnectedDevices: {0} ({1}) added to {2}", di.DriveId, di.VhdFileName, _mediatype);
                }
            }
            log.DebugFormat("OVF.CreateConnectedDevices OTHER Controllers completed {0} ", vsId);

            #endregion

            #region CHECK ALL DISKS WERE DEFINED

            foreach (DiskInfo di in vhdExports)
            {
                if (!di.Added)
                {
                    AddDisk(ovfEnv, vsId, Guid.NewGuid().ToString(), lang, di.VhdFileName, false, _ovfrm.GetString("RASD_19_CAPTION"), _ovfrm.GetString("RASD_19_DESCRIPTION"), Convert.ToUInt64(di.PhysicalSize), Convert.ToUInt64(di.CapacitySize));
                    di.Added = true;
                    log.Warn("CreateConnectedDevices: MANUAL Update of OVF REQUIRED TO DEFINE: Disk Size and Capacity");
                    log.WarnFormat("CreateConnectedDevices: {0} ({1}) NOT FOUND, added as Unknown with 0 Size", di.DriveId, di.VhdFileName);
                }
            }

            #endregion

            log.DebugFormat("OVF.CreateConnectedDevices completed {0} ", vsId);
        }

        #endregion
    }
}
