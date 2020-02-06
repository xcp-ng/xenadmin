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
using System.IO;
using System.Management;
using System.Reflection;
using System.Resources;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using XenOvf.Definitions;
using XenOvf.Utilities;
using XenCenterLib.Archive;
using XenCenterLib.Compression;

namespace XenOvf
{
    public partial class OVF
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Event Registration of changes in Ovf state.
        /// </summary>
        public static event Action<OvfEventArgs> Changed;

        /// <summary>
        /// Protected method call for eventing.
        /// </summary>
        /// <param name="e">OvfEventArgs</param>
        private static void OnChanged(OvfEventArgs e)
        {
            if (Changed != null)
            {
                Changed(e);
            }
        }

        private const long KB = 1024;
        private const long MB = (KB * 1024);
        private const long GB = (MB * 1024);

        private static bool _promptForEula = true;

		internal static ResourceManager _rm = new ResourceManager("XenOvf.Messages", Assembly.GetExecutingAssembly());
		internal static ResourceManager _ovfrm = new ResourceManager("XenOvf.Content", Assembly.GetExecutingAssembly());

        #region PUBLIC
        public OVF()
        {
            UnLoad();
        }

        #region PROPERTIES

        public static object AlgorithmMap(string key)
        {
            return Properties.Settings.Default[key];
        }

        public bool PromptForEula
        {
            get { return _promptForEula; }
            set { _promptForEula = value; }
        }
 
        #endregion

        #region LOAD OVF
        /// <summary>
        /// Load an OVF XML File into OVF class context
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static EnvelopeType Load(string filename)
        {
        	return Tools.LoadOvfXml(filename);
        }

        #endregion

        #region SAVE OVF

        public static void SaveAs(EnvelopeType ovfEnv, string filename)
        {
            SaveAs(ToXml(ovfEnv), filename);
        }

        public static void SaveAs(string OvfXml, string filename)
        {
            log.DebugFormat("OVF.SaveAs: {0}", filename);
            if (OvfXml == null)
            {
                log.Error("SaveAs: cannot save NULL string OvfXml");
                throw new ArgumentNullException();
            }
            if (filename == null)
            {
                log.Error("SaveAs: cannot save OvfXml. Filename was NULL");
                throw new ArgumentNullException();
            }

            string oldfile = string.Format(@"{0}_ovf.old", Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename)));
            try
            {
                if (File.Exists(filename))
                {
                    if (File.Exists(oldfile))
                    {
                        File.Delete(oldfile);
                    }
                    File.Move(filename, oldfile);
                }
            }
            catch (Exception ex)
            {
                log.Error("File handling error. ", ex);
            }

            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(OvfXml);
                sw.Flush();
            }

            if (File.Exists(oldfile))
                File.Delete(oldfile);

            log.Debug("OVF.SaveAs completed");
        }
        #endregion

        #region UNLOAD OVF
        /// <summary>
        /// Clears memory and resets to defaults.
        /// </summary>
        public void UnLoad()
        {
            mappings.Clear();
            Win32_ComputerSystem = null;
            Win32_Processor.Clear();
            Win32_CDROMDrive.Clear();
            Win32_DiskDrive.Clear();
            Win32_NetworkAdapter.Clear();
            Win32_IDEController.Clear();
            Win32_SCSIController.Clear();
            Win32_IDEControllerDevice.Clear();
            Win32_SCSIControllerDevice.Clear();
            Win32_DiskPartition.Clear();
            Win32_DiskDriveToDiskPartition.Clear();
        }
        #endregion

        #region CHECK FOR FILE(S) METHODs (and HAS methods)

		public static bool HasDeploymentOptions(EnvelopeType ovfObj)
        {
            DeploymentOptionSection_Type[] dos = FindSections<DeploymentOptionSection_Type>(ovfObj);
            if (dos != null && dos.Length > 0)
            {
                return true;
            }
            return false;
        }

		public static bool HasEula(EnvelopeType ovfObj)
        {
            EulaSection_Type[] eulas = FindSections<EulaSection_Type>(ovfObj);
            if (eulas != null && eulas.Length > 0)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region ADDs

		public string AddAnnotation(EnvelopeType ovfObj, string vsId, string info, string annotation)
        {
            return AddAnnotation(ovfObj, vsId, Properties.Settings.Default.Language, info, annotation);
        }

        public string AddAnnotation(EnvelopeType ovfObj, string vsId, string lang, string info, string annotation)
        {
			VirtualSystem_Type vs = FindVirtualSystemById(ovfObj, vsId);
            List<Section_Type> sections = new List<Section_Type>();
            sections.AddRange(vs.Items);

            AnnotationSection_Type annotate = new AnnotationSection_Type();

            annotate.Id = Guid.NewGuid().ToString();
			annotate.Info = new Msg_Type(AddToStringSection(ovfObj, lang, info), info);
			annotate.Annotation = new Msg_Type(AddToStringSection(ovfObj, lang, info), info);
            sections.Add(annotate);

            vs.Items = sections.ToArray();

            return annotate.Id;
        }

		public static string AddCDROM(EnvelopeType ovfObj, string vsId, string cdId, string caption, string description)
        {
            return AddCDROM(ovfObj, vsId, Properties.Settings.Default.Language, cdId, caption, description);
        }
        /// <summary>
        /// Add a CD/DVD Drive
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="cdId">InstanceID</param>
        /// <param name="caption">string short description</param>
        /// <param name="description">string longer description</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
                                                         Justification = "Logging mechanism")]
		public static string AddCDROM(EnvelopeType ovfObj, string vsId, string lang, string cdId, string caption, string description)
        {
            RASD_Type rasd = new RASD_Type();
            rasd.required = false;
            rasd.AllocationUnits = new cimString(_ovfrm.GetString("RASD_16_ALLOCATIONUNITS"));
            rasd.AutomaticAllocation = new cimBoolean();
            rasd.AutomaticAllocation.Value = true;
            rasd.ConsumerVisibility = new ConsumerVisibility();
            rasd.ConsumerVisibility.Value = 3; //From MS.
            rasd.Caption = new Caption(caption);
            rasd.Description = new cimString(description);
            rasd.ElementName = new cimString(_ovfrm.GetString("RASD_16_ELEMENTNAME"));
            rasd.InstanceID = new cimString(cdId);
            rasd.Limit = new cimUnsignedLong();
            rasd.Limit.Value = 1; // From MS;
            rasd.MappingBehavior = new MappingBehavior();
            rasd.MappingBehavior.Value = 0; // From MS.
            rasd.ResourceType = new ResourceType();
            rasd.ResourceType.Value = 16;  // DVD Drive
            rasd.VirtualQuantity = new cimUnsignedLong();
            rasd.VirtualQuantity.Value = 1;
            rasd.Weight = new cimUnsignedInt();
            rasd.Weight.Value = 0;  // From MS.

            AddRasdToAllVHS(ovfObj, vsId, rasd);

            log.Debug("OVF.AddCDDrive completed");
            return rasd.InstanceID.Value;
        }

		public void AddController(EnvelopeType ovfObj, string vsId, DeviceType type, string deviceId, int iteration)
        {
            AddController(ovfObj, vsId, Properties.Settings.Default.Language, type, deviceId, iteration);
        }

        /// <summary>
        /// Add a controller to the mix.
        /// </summary>
        /// <param name="ovfObj">object of type EnvelopeType</param>
        /// <param name="vsId">System Instance ID</param>
        /// <param name="type">Resource Type: 5 = IDE, 6 = SCSI</param>
        /// <param name="deviceId">String identifying the device to match to the controller</param>
        /// <param name="iteration">which controller 0 = first, 1, 2, 3... (per type)</param>
        /// <returns>InstanceID of Controller</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
                                                         Justification = "Logging mechanism")]
		public void AddController(EnvelopeType ovfObj, string vsId, string lang, DeviceType type, string deviceId, int iteration)
        {
            VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfObj, vsId);

            foreach (VirtualHardwareSection_Type vhs in vhsArray)
            {
                AddControllerToVHS(vhs, lang, type, deviceId, iteration);
            }

            log.Debug("OVF.AddController completed");
        }

        /// <summary>
        /// Add a controller to the mix.
        /// </summary>
        /// <param name="ovfObj">object of type EnvelopeType</param>
        /// <param name="vsId">System Instance ID</param>
        /// <param name="type">Resource Type: 5 = IDE, 6 = SCSI</param>
        /// <param name="deviceId">String identifying the device to match to the controller</param>
        /// <param name="iteration">which controller 0 = first, 1, 2, 3... (per type)</param>
        /// <returns>InstanceID of Controller</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
                                                         Justification = "Logging mechanism")]
        public void AddControllerToVHS(object vhsObj, string lang, DeviceType type, string deviceId, int iteration)
        {
            VirtualHardwareSection_Type vhs = (VirtualHardwareSection_Type)vhsObj;
            RASD_Type rasd = new RASD_Type();

            string controllername = _ovfrm.GetString("RASD_CONTROLLER_UNKNOWN");

            switch (type)
            {
                case DeviceType.IDE: { controllername = _ovfrm.GetString("RASD_CONTROLLER_IDE"); break; }
                case DeviceType.SCSI: { controllername = _ovfrm.GetString("RASD_CONTROLLER_SCSI"); break; }
                default: { controllername = _ovfrm.GetString("RASD_CONTROLLER_OTHER"); break; }
            }

            string caption = string.Format("{0} {1}", controllername, iteration);
            rasd.required = false;  // as default change to FALSE, if we make a connection, it'll change to true.
            rasd.Address = new cimString(Convert.ToString(iteration));
            rasd.AllocationUnits = new cimString("Controllers");
            rasd.Caption = new Caption(caption);
            rasd.ConsumerVisibility = new ConsumerVisibility();
            rasd.ConsumerVisibility.Value = 3;
            rasd.Description = new cimString(string.Format(_ovfrm.GetString("RASD_CONTROLLER_DESCRIPTION"), controllername));
            rasd.ElementName = new cimString(string.Format(_ovfrm.GetString("RASD_CONTROLLER_ELEMENTNAME"), controllername, iteration));
            rasd.InstanceID = new cimString(deviceId);
            if (type == DeviceType.SCSI)
            {
                rasd.ResourceSubType = new cimString(_ovfrm.GetString("RASD_CONTROLLER_SCSI_SUBTYPE"));
            }
            rasd.Limit = new cimUnsignedLong();
            rasd.Limit.Value = 2;
            rasd.ResourceType = new ResourceType();
            rasd.ResourceType.Value = (ushort)type;
            rasd.VirtualQuantity = new cimUnsignedLong();
            rasd.VirtualQuantity.Value = 1;
            rasd.Weight = new cimUnsignedInt();
            rasd.Weight.Value = 0;

            List<RASD_Type> rasds = new List<RASD_Type>();
            rasds.Add(rasd);

            if (vhs.Item != null && vhs.Item.Length > 0)
            {
                rasds.AddRange(vhs.Item);
            }

            vhs.Item = rasds.ToArray();

            log.Debug("OVF.AddController completed");
        }

		public string AddDeploymentOption(EnvelopeType ovfObj, string label, string description, bool isdefault)
        {
            return AddDeploymentOption(ovfObj, Properties.Settings.Default.Language, label, description, isdefault);
        }

		public string AddDeploymentOption(EnvelopeType env, string lang, string label, string description, bool isdefault)
        {
            DeploymentOptionSection_Type dos = null;
            List<Section_Type> sections = new List<Section_Type>();
            Section_Type[] sectionArray = null;

            if (env.Sections != null)
            {
                sectionArray = env.Sections;
            }

            foreach (Section_Type sect in sectionArray)
            {
                if (sect is DeploymentOptionSection_Type)
                {
                    dos = (DeploymentOptionSection_Type)sect;
                }
                else
                {
                    sections.Add(sect);
                }
            }
            if (dos == null)
            {
                dos = new DeploymentOptionSection_Type();
                dos.Id = Guid.NewGuid().ToString();
            }

            DeploymentOptionSection_TypeConfiguration conf = new DeploymentOptionSection_TypeConfiguration();
            conf.@default = isdefault;
			conf.Description = new Msg_Type(AddToStringSection(env, lang, description), description);
			conf.Label = new Msg_Type(AddToStringSection(env, lang, label), label);
            conf.id = Guid.NewGuid().ToString();

            List<DeploymentOptionSection_TypeConfiguration> confs = new List<DeploymentOptionSection_TypeConfiguration>();
            if (dos.Configuration != null && dos.Configuration.Length > 0)
            {
                confs.AddRange(dos.Configuration);
            }
            confs.Add(conf);
            dos.Configuration = confs.ToArray();
            sections.Add(dos);

            env.Sections = sections.ToArray();

            return conf.id;
        }

		public void AddDeviceToController(EnvelopeType ovfObj, string vsId, string deviceInstanceId, string controllerInstanceId, string AddressOnController)
        {
            AddDeviceToController(ovfObj, vsId, Properties.Settings.Default.Language, deviceInstanceId, controllerInstanceId, AddressOnController);
        }
        /// <summary>
        /// Connect a Disk (VHD) to a Controller ie: IDE or SCSI and where on controller it should exist.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">string Virtual System Identifier</param>
        /// <param name="deviceInstanceId">instance ID of device</param>
        /// <param name="controllerInstanceId">instance ID of controller</param>
        /// <param name="AddressOnController">where on controller 0...</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
                                                         Justification = "Logging mechanism")]
        public void AddDeviceToController(EnvelopeType ovfObj, string vsId, string lang, string deviceInstanceId, string controllerInstanceId, string AddressOnController)
        {
            VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfObj, vsId);

            foreach (VirtualHardwareSection_Type vhs in vhsArray)
            {
                foreach (RASD_Type rasd in vhs.Item)
                {
                    if (rasd.InstanceID.Value.Equals(deviceInstanceId))
                    {
                        rasd.Parent = new cimString(controllerInstanceId);
                        rasd.AddressOnParent = new cimString(AddressOnController);
                    }
                    else if (rasd.InstanceID.Value.Equals(controllerInstanceId))
                    {
                        rasd.required = true;
                    }
                }
            }
            log.Debug("OVF.AddDeviceToController completed");
            return;
        }

        public static void AddDisk(EnvelopeType ovfObj, string vsId, string diskId, string vhdFileName, bool bootable, string name, string description, ulong filesize, ulong capacity)
        {
            AddDisk(ovfObj, vsId, diskId, Properties.Settings.Default.Language, vhdFileName, bootable, name, description, filesize, capacity);
        }
        /// <summary>
        /// Add a VHD to the VM
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Which VM to apply to</param>
        /// <param name="diskId">The RASDs InstanceID</param>
        /// <param name="vhdFileName">File Name of VHD (needs to be in the same directory as OVF</param>
        /// <param name="bootable">Is this disk bootable</param>
        /// <param name="caption">Short string describing disk</param>
        /// <param name="description">Description of VHD</param>
        /// <param name="filesize">physical file size</param>
        /// <param name="capacity">capacity of disk</param>
        /// <param name="freespace">amount of free space</param>
        /// <param name="sysIdx">System Index in OVF to set memory value (0 = first VM)</param>
        /// <param name="idx">Section Index in Virtual Hardware Section (1 = VHS index)</param>
        /// <returns>Instance ID of Disk</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
                                                        Justification = "Logging mechanism")]
        public static void AddDisk(EnvelopeType ovfEnv, string vsId, string diskId, string lang, string vhdFileName, bool bootable, string name, string description, ulong filesize, ulong capacity)
        {
            List<File_Type> files = new List<File_Type>();
            List<Section_Type> sections = new List<Section_Type>();
            List<VirtualDiskDesc_Type> disks = new List<VirtualDiskDesc_Type>();
            DiskSection_Type disksection = null;

            if (ovfEnv.References.File != null && ovfEnv.References.File.Length > 0)
            {
                foreach (File_Type fi in ovfEnv.References.File)
                {
                    files.Add(fi);
                }
            }

            if (ovfEnv.Sections != null && ovfEnv.Sections.Length > 0)
            {
                foreach (Section_Type sect in ovfEnv.Sections)
                {
                    if (sect is DiskSection_Type)
                    {
                        DiskSection_Type ds = (DiskSection_Type)sect;
                        if (ds.Disk != null && ds.Disk.Length > 0)
                        {
                            foreach (VirtualDiskDesc_Type vd in ds.Disk)
                            {
                                disks.Add(vd);
                            }
                        }
                    }
                    else
                    {
                        sections.Add(sect);
                    }
                }
            }

            disksection = new DiskSection_Type();
            string info = _ovfrm.GetString("SECTION_DISK_INFO");
            disksection.Info = new Msg_Type(AddToStringSection(ovfEnv, lang, info), info);


            VirtualDiskDesc_Type vdisk = new VirtualDiskDesc_Type();
            File_Type filet = new File_Type();
            RASD_Type rasd = new RASD_Type();

            vdisk.capacity = Convert.ToString(capacity);
            vdisk.isBootable = bootable;
            vdisk.format = Properties.Settings.Default.winFileFormatURI;
            vdisk.fileRef = diskId;
            vdisk.diskId = vdisk.fileRef;
            disks.Add(vdisk);

            filet.id = vdisk.diskId;
            if (filesize > 0)
            {
                filet.size = filesize;
                filet.sizeSpecified = true;
            }
            filet.href = string.Format(Properties.Settings.Default.FileURI, vhdFileName);
            files.Add(filet);
            rasd.AllocationUnits = new cimString(_ovfrm.GetString("RASD_19_ALLOCATIONUNITS"));
            rasd.AutomaticAllocation = new cimBoolean();
            rasd.AutomaticAllocation.Value = true;
            // Other code depends on the value of caption.
            rasd.Caption = new Caption(_ovfrm.GetString("RASD_19_CAPTION"));
            rasd.ConsumerVisibility = new ConsumerVisibility();
            rasd.ConsumerVisibility.Value = 3; //From MS.

            rasd.Connection = new cimString[] { new cimString(diskId) };
            rasd.HostResource = new cimString[] { new cimString(string.Format(Properties.Settings.Default.hostresource, diskId)) };

            rasd.Description = new cimString(description);
            rasd.ElementName = new cimString(name);
            rasd.InstanceID = new cimString(diskId);
            rasd.Limit = new cimUnsignedLong();
            rasd.Limit.Value = 1; // From MS;
            rasd.MappingBehavior = new MappingBehavior();
            rasd.MappingBehavior.Value = 0; // From MS.
            rasd.ResourceSubType = new cimString(_ovfrm.GetString("RASD_19_RESOURCESUBTYPE"));
            rasd.ResourceType = new ResourceType();
            rasd.ResourceType.Value = 19;  // Hard Disk Image
            rasd.VirtualQuantity = new cimUnsignedLong();
            rasd.VirtualQuantity.Value = 1;
            rasd.Weight = new cimUnsignedInt();
            rasd.Weight.Value = 100;  // From MS.

            AddRasdToAllVHS(ovfEnv, vsId, rasd);

            disksection.Disk = disks.ToArray();
            sections.Add(disksection);

            ovfEnv.Sections = sections.ToArray();
            ovfEnv.References.File = files.ToArray();
            log.Debug("OVF.AddDisk completed");
        }

		public static string AddEula(EnvelopeType ovfObj, string eulafilename)
        {
            return AddEula(ovfObj, Properties.Settings.Default.Language, eulafilename);
        }
        /// <summary>
        /// Add a EULA to the OVF
        /// </summary>
		public static string AddEula(EnvelopeType ovfEnv, string lang, string eulafilename)
        {
            EulaSection_Type eulaSection = null;
            if (eulafilename != null)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(eulafilename, FileMode.Open, FileAccess.Read, FileShare.Read);
                    StreamReader sw = new StreamReader(fs);

                    eulaSection = new EulaSection_Type();
                    eulaSection.Id = Guid.NewGuid().ToString();
                    string info = _ovfrm.GetString("SECTION_EULA_INFO");
                    eulaSection.Info = new Msg_Type(AddToStringSection(ovfEnv, lang, info), info);
                    string agreement = sw.ReadToEnd();
                    eulaSection.License = new Msg_Type(AddToStringSection(ovfEnv, lang, agreement), agreement);
                }
                catch (Exception ex)
                {
                    eulaSection = null;
                    throw new Exception("Export Halted: Cannot read EULA", ex);
                }
                finally
                {
                    if (fs != null) { fs.Close(); }
                }
            }
            if (eulaSection != null)
            {
                List<Section_Type> sections = new List<Section_Type>();
                if (ovfEnv.Item != null)
                {
                    if (ovfEnv.Item.Items != null)
                    {
                        sections.AddRange(ovfEnv.Item.Items);
                    }
                }
                else
                {
                    throw new Exception("Cannot add EULA no VirtualSystem or VirtualSystemCollection available.");
                }
                sections.Add(eulaSection);
                ovfEnv.Item.Items = sections.ToArray();
            }
            return eulaSection.Id;
        }
        /// <summary>
        /// Add an external file
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="filename">Filename</param>
		public static void AddExternalFile(EnvelopeType ovfObj, string filename, string id)
        {
            File_Type ft = new File_Type();
            if (id == null || id.Length <= 0)
            {
                id = Guid.NewGuid().ToString();
            }
            ft.id = id;
            ft.href = Path.GetFileName(filename);
            List<File_Type> ftList = new List<File_Type>();
            if (ovfObj.References.File != null)
            {
                ftList.AddRange(ovfObj.References.File);
            }
            ftList.Add(ft);
            ovfObj.References.File = ftList.ToArray();
        }

		public void AddFileReference(EnvelopeType ovfObj, string filename, string id, ulong capacity, string format)
        {
            AddFileReference(ovfObj, Properties.Settings.Default.Language, filename, id, capacity, format);
        }
        /// <summary>
        /// Add an Disk Reference file
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="filename">Filename</param>
		public static void AddFileReference(EnvelopeType env, string lang, string filename, string id, ulong capacity, string format)
        {
            DiskSection_Type ds = null;
            List<VirtualDiskDesc_Type> vdisks = new List<VirtualDiskDesc_Type>();
            List<Section_Type> sections = new List<Section_Type>();
            if (env.Sections != null)
            {
                foreach (Section_Type section in env.Sections)
                {
                    if (section is DiskSection_Type)
                    {
                        ds = (DiskSection_Type)section;
                    }
                    else
                    {
                        sections.Add(section);
                    }
                }
            }

            if (ds == null)
            {
                ds = new DiskSection_Type();
                string info = _ovfrm.GetString("SECTION_DISK_INFO");
                ds.Info = new Msg_Type(AddToStringSection(env, lang, info), info);
            }

            VirtualDiskDesc_Type vdisk = new VirtualDiskDesc_Type();
            vdisk.format = format;
            vdisk.diskId = id;
            vdisk.fileRef = id;

            if (capacity != 0)
            {
                vdisk.capacity = Convert.ToString(capacity);
            }

            AddExternalFile(env, filename, id);

            if (ds.Disk != null)
            {
                foreach (VirtualDiskDesc_Type vd in ds.Disk)
                {
                    vdisks.Add(vd);
                }
            }

            vdisks.Add(vdisk);
            ds.Disk = vdisks.ToArray();
            sections.Add(ds);
            env.Sections = sections.ToArray();

        }

		public static InstallSection_Type AddInstallSection(EnvelopeType ovfObj, string vsId, ushort bootStopDelay, string lang, string info)
        {
            VirtualSystem_Type vSystem = FindVirtualSystemById(ovfObj, vsId);
            InstallSection_Type installSection = null;

            List<Section_Type> sections = new List<Section_Type>();
            foreach (Section_Type section in vSystem.Items)
            {
                if (section is InstallSection_Type)
                {
                    installSection = (InstallSection_Type)section;
                }
                else
                {
                    sections.Add(section);
                }
            }
            if (installSection == null)
            {
                installSection = new InstallSection_Type();
                installSection.Id = Guid.NewGuid().ToString();
            }
            installSection.initialBootStopDelay = bootStopDelay;
            installSection.Info = new Msg_Type(AddToStringSection(ovfObj, lang, info), info);
            sections.Add(installSection);
            vSystem.Items = sections.ToArray();
            return installSection;
        }

		public static void AddNetwork(EnvelopeType ovfObj, string vsId, string netId, string netName, string networkDescription, string macAddress)
        {
            AddNetwork(ovfObj, vsId, Properties.Settings.Default.Language, netId, netName, networkDescription, macAddress);
        }
        /// <summary>
        /// Add a Network to the VM
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="macAddress">null = unset, value sets MAC Address</param>
		public static void AddNetwork(EnvelopeType ovfEnv, string vsId, string lang, string netId, string netName, string networkDescription, string macAddress)
        {
            List<NetworkSection_TypeNetwork> ns = new List<NetworkSection_TypeNetwork>();
            NetworkSection_TypeNetwork attached = null;
            List<RASD_Type> rasds = new List<RASD_Type>();

            NetworkSection_Type netsection = new NetworkSection_Type();
            string info = _ovfrm.GetString("SECTION_NETWORK_INFO");
            netsection.Info = new Msg_Type(AddToStringSection(ovfEnv, lang, info), info);

            List<Section_Type> sections = new List<Section_Type>();

            if (ovfEnv.Sections != null && ovfEnv.Sections.Length > 0)
            {
                foreach (Section_Type sect in ovfEnv.Sections)
                {
                    if (sect is NetworkSection_Type)
                    {
                        netsection = (NetworkSection_Type)sect;
                    }
                    else
                    {
                        sections.Add(sect);
                    }
                }
            }

            if (netsection.Network != null && netsection.Network.Length > 0)
            {
                foreach (NetworkSection_TypeNetwork lns in netsection.Network)
                {
                    ns.Add(lns);
                    if (lns.name.Equals(netId))
                    {
                        attached = lns;
                    }
                }
            }

            RASD_Type rasd = new RASD_Type();
            if (!string.IsNullOrEmpty(macAddress))
            {
                rasd.Address = new cimString(macAddress);
            }
            rasd.AllocationUnits = new cimString(_ovfrm.GetString("RASD_10_ALLOCATIONUNITS"));
            rasd.AutomaticAllocation = new cimBoolean();
            rasd.AutomaticAllocation.Value = true;
            rasd.Caption = new Caption(_ovfrm.GetString("RASD_10_CAPTION"));
            rasd.ConsumerVisibility = new ConsumerVisibility();
            rasd.ConsumerVisibility.Value = 3;
            if (!string.IsNullOrEmpty(networkDescription))
            {
                rasd.Description = new cimString(networkDescription);
            }
            else
            {
                rasd.Description = new cimString(_ovfrm.GetString("RASD_10_DESCRIPTION"));
            }

			rasd.ElementName = new cimString(netName);
            rasd.InstanceID = new cimString(Guid.NewGuid().ToString());
            rasd.Connection = new cimString[1];
            rasd.Connection[0] = new cimString(netId);
            rasd.Limit = new cimUnsignedLong();
            rasd.Limit.Value = 1;
            rasd.MappingBehavior = new MappingBehavior();
            rasd.MappingBehavior.Value = 0;
            rasd.ResourceType = new ResourceType();
            rasd.ResourceType.Value = 10;
            rasd.VirtualQuantity = new cimUnsignedLong();
            rasd.VirtualQuantity.Value = 1;
            rasd.Weight = new cimUnsignedInt();
            rasd.Weight.Value = 0;

            if (attached == null)
            {
                attached = new NetworkSection_TypeNetwork();
                attached.name = netId;
                attached.Description = new Msg_Type(AddToStringSection(ovfEnv, lang, rasd.Description.Value), rasd.Description.Value);
                ns.Add(attached);
            }

            AddRasdToAllVHS(ovfEnv, vsId, rasd);

            netsection.Network = ns.ToArray();
            sections.Add(netsection);
            ovfEnv.Sections = sections.ToArray();
            log.Debug("OVF.AddNetwork completed");
        }

		public void AddOperatingSystemSection(EnvelopeType ovfObj, string vsId, string description, string osInfo)
        {
            AddOperatingSystemSection(ovfObj, vsId, Properties.Settings.Default.Language, description, osInfo, 0);
        }

		public static void AddOperatingSystemSection(EnvelopeType ovfObj, string vsId, string lang, string description, string osInfo)
        {
            AddOperatingSystemSection(ovfObj, vsId, lang, description, osInfo, 0);
        }
        /// <summary>
        /// Add the Operating System Section 
        /// </summary>
        /// <param name="ovfEnv">Ovf:EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="lang">Language</param>
        /// <param name="description">Description</param>
        /// <param name="osInfo">OS Information</param>
        /// <param name="osid">ushort identifying the OS from CIM_OperatingSystem ValueMap</param>
		public void AddOperatingSystemSection(EnvelopeType ovfObj, string vsId, string description, string osInfo, ushort osid)
        {
            AddOperatingSystemSection(ovfObj, vsId, Properties.Settings.Default.Language, description, osInfo, osid);
        }
        /// <summary>
        /// Add the Operating System Section
        /// </summary>
        /// <param name="ovfEnv">Ovf:EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="lang">Language</param>
        /// <param name="description">Description</param>
        /// <param name="osInfo">OS Information</param>
        /// <param name="osid">ushort identifying the OS from CIM_OperatingSystem ValueMap</param>
		public static void AddOperatingSystemSection(EnvelopeType ovfEnv, string vsId, string lang, string description, string osInfo, ushort osid)
        {
            OperatingSystemSection_Type oss = new OperatingSystemSection_Type();
            oss.id = osid;
            string info = null;
            if (!string.IsNullOrEmpty(description))
            {
                oss.Description = new Msg_Type(AddToStringSection(ovfEnv, lang, description), description);
            }
            else
            {
                info = _ovfrm.GetString("SECTION_OPERATINGSYSTEM_DESCRIPTION");
                oss.Description = new Msg_Type(AddToStringSection(ovfEnv, lang, info), info);
            }
            if (!string.IsNullOrEmpty(osInfo))
            {
                oss.Info = new Msg_Type(AddToStringSection(ovfEnv, lang, osInfo), osInfo);
            }
            else
            {
                info = _ovfrm.GetString("SECTION_OPERATINGSYSTEM_INFO");
                oss.Info = new Msg_Type(AddToStringSection(ovfEnv, lang, info), info);
            }

            if (ovfEnv.Item == null || ((VirtualSystemCollection_Type)ovfEnv.Item).Content == null)
            {
                throw new ArgumentNullException(Messages.FAILED_TO_ADD_OS_SECTION);
            }
            AddContent((VirtualSystemCollection_Type)ovfEnv.Item, vsId, oss);
            log.DebugFormat("OVF.AddOperatingSystemSection completed {0}", vsId);
        }

		public static string AddOtherSystemSettingData(EnvelopeType ovfObj, string vsId, string name, string value, string description, bool permitMultiple=false)
        {
            return AddOtherSystemSettingData(ovfObj, vsId, Properties.Settings.Default.Language, name, value, description, permitMultiple);
        }
        /// <summary>
        /// Add XEN Specific configuration Items.
        /// </summary>
        /// <param name="OvfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="name">Name of Parameter: is:  HVM-boot-policy (case sensitive)</param>
        /// <param name="value">value for the parameter</param>
        /// <param name="description">Description of parameter</param>
        /// <param name="permitMultiple">Whether the setting data permit multiple values, default false</param>
        public static string AddOtherSystemSettingData(EnvelopeType ovfObj, string vsId, string lang, string name, string value, string description, bool permitMultiple=false)
        {
            VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfObj, vsId);
            VirtualHardwareSection_Type vhs = null;
            foreach (VirtualHardwareSection_Type _vhs in vhsArray)
            {
                if (_vhs.System != null && _vhs.System.VirtualSystemType != null &&
                    !(string.IsNullOrEmpty(_vhs.System.VirtualSystemType.Value)) &&
                    (_vhs.System.VirtualSystemType.Value.ToLower().StartsWith("xen") ||
                     _vhs.System.VirtualSystemType.Value.ToLower().StartsWith("hvm")))
                {
                    vhs = _vhs;
                    break;
                }
            }

            if (vhs == null)
            {
                log.Warn("OVF.AddOtherSystemSettingData: could not find 'xen' or 'hvm' system type VHS, skipping.");
                return null;
            }

            List<Xen_ConfigurationSettingData_Type> xencfg = new List<Xen_ConfigurationSettingData_Type>();

            if (vhs.VirtualSystemOtherConfigurationData != null && vhs.VirtualSystemOtherConfigurationData.Length > 0)
            {
                foreach (Xen_ConfigurationSettingData_Type xencsd in vhs.VirtualSystemOtherConfigurationData)
                {
                    // If permitMultiple, the old items are kept, and the new item will be added later,
                    // otherwise, the old item is skipped here and will be overridden by the new one
                    if (permitMultiple || xencsd.Name.ToLower() != name.ToLower())
                    {
                        xencfg.Add(xencsd);
                    }
                }
            }

            Xen_ConfigurationSettingData_Type xenother = new Xen_ConfigurationSettingData_Type();
            xenother.id = Guid.NewGuid().ToString();
            xenother.Name = name;
            xenother.Value = new cimString(value);
            xenother.Info = new Msg_Type(AddToStringSection(ovfObj, lang, description), description);

            xencfg.Add(xenother);

            vhs.VirtualSystemOtherConfigurationData = xencfg.ToArray();
            log.Debug("OVF.AddOtherSystemSettingData completed");
            return xenother.id;
        }

        public static string AddPostInstallOperation(EnvelopeType ovfObj, string vsId, string lang, string message)
        {
            VirtualSystem_Type vSystem = FindVirtualSystemById(ovfObj, vsId);
            InstallSection_Type installSection = null;
            foreach (Section_Type sec in vSystem.Items)
            {
                if (sec is InstallSection_Type)
                {
                    installSection = (InstallSection_Type)sec;
                    break;
                }
            }

            if (installSection == null)
                installSection = AddInstallSection(ovfObj, vsId, 600, Properties.Settings.Default.Language, "ConfigureForXenServer");


            Xen_PostInstallOperation_Type XenPostInstall = new Xen_PostInstallOperation_Type();
            XenPostInstall.required = false;
            XenPostInstall.requiredSpecified = true;
            XenPostInstall.id = Guid.NewGuid().ToString();
            XenPostInstall.Info = new Msg_Type(AddToStringSection(ovfObj, lang, message), message);
            installSection.PostInstallOperations = XenPostInstall;

            return XenPostInstall.id;
        }

		public string AddProductSection(EnvelopeType ovfObj, string nameSpace, string info, string product, string vendor, string version, string producturl, string vendorurl)
        {
            return AddProductSection(ovfObj, Properties.Settings.Default.Language, nameSpace, info, product, vendor, version, producturl, vendorurl);
        }

		public string AddProductSection(EnvelopeType env, string lang, string nameSpace, string info, string product, string vendor, string version, string producturl, string vendorurl)
        {
            string psId = Guid.NewGuid().ToString();
            ProductSection_Type ps = new ProductSection_Type();
            ps.@class = nameSpace;
            ps.instance = psId;
            ps.Product = new Msg_Type(AddToStringSection(env, lang, product), product);
            ps.ProductUrl = new cimString(producturl);
            ps.Vendor = new Msg_Type(AddToStringSection(env, lang, vendor), vendor);
            ps.Version = new cimString(version);
            ps.VendorUrl = new cimString(vendorurl);

            List<Section_Type> sections = new List<Section_Type>();
            Section_Type[] sectionArray = null;
            if (env.Item != null)
            {
                if (env.Item is VirtualSystemCollection_Type)
                {
                    sectionArray = ((VirtualSystemCollection_Type)env.Item).Content[0].Items;
                }
                else
                {
                    sectionArray = env.Item.Items;
                }
            }
            else
            {
                sectionArray = env.Sections;
            }
            foreach (Section_Type section in sectionArray)
            {
                if (!(section is ProductSection_Type))
                {
                    sections.Add(section);
                }
            }
            sections.Add(ps);

            if (env.Item is VirtualSystemCollection_Type)
            {
                ((VirtualSystemCollection_Type)env.Item).Content[0].Items = sections.ToArray();
            }
            else
            {
                env.Item.Items = sections.ToArray();
            }

            return psId;
        }

		public string AddProductSectionProperty(EnvelopeType ovfObj, string psId, string category, string key, string type, bool userConfig, string label, string description)
        {
            return AddProductSectionProperty(ovfObj, psId, Properties.Settings.Default.Language, category, key, type, userConfig, label, description);
        }

		public string AddProductSectionProperty(EnvelopeType env, string psId, string lang, string category, string key, string type, bool userConfig, string label, string description)
        {
            Msg_Type msgCategory = new Msg_Type(AddToStringSection(env, lang, category), category);
            ProductSection_TypeProperty pst = new ProductSection_TypeProperty();
            pst.id = Guid.NewGuid().ToString();
            pst.key = key;
            pst.type = type;
            pst.userConfigurable = userConfig;
            pst.Label = new Msg_Type(AddToStringSection(env, lang, label), label);
            pst.Description = new Msg_Type(AddToStringSection(env, lang, description), description);

            ProductSection_Type productsection = null;
            List<Section_Type> sections = new List<Section_Type>();

            Section_Type[] sectionArray = null;
            if (env.Item is VirtualSystemCollection_Type)
            {
                sectionArray = ((VirtualSystemCollection_Type)env.Item).Content[0].Items;
            }
            else
            {
                sectionArray = env.Item.Items;
            }


            foreach (Section_Type section in sectionArray)
            {
                if (section is ProductSection_Type)
                {
                    productsection = (ProductSection_Type)section;
                    if (productsection.instance == psId)
                    {
                        break;
                    }
                    productsection = null;
                }
                sections.Add(section);
            }

            if (productsection == null)
                throw new InvalidDataException(string.Format(Messages.OVF_PRODUCT_SECTION_MISSING, psId));

            List<object> pstp = new List<object>();
            if (productsection.Items != null && productsection.Items.Length > 0)
            {
                pstp.AddRange(productsection.Items);
            }
            pstp.Add(msgCategory);
            pstp.Add(pst);

            productsection.Items = pstp.ToArray();

            return pst.id;
        }

		public static string AddRasd(EnvelopeType ovfObj, string vsId, object rasd)
        {
            return AddRasd(ovfObj, vsId, Properties.Settings.Default.Language, rasd);
        }
        /// <summary>
        /// Add a RASD to to the Virtual System
        /// will replace any existing RASD with the same InstanceID.
        /// </summary>
        /// <param name="ovfObj">object of type EnvelopeType</param>
        /// <param name="vsId">Virtual System ID (InstanceID)</param>
        /// <param name="rasd">object of type RASD_Type</param>
		public static string AddRasd(EnvelopeType ovfObj, string vsId, string lang, object rasd)
        {
            RASD_Type lRasd = (RASD_Type)rasd;
            AddRasdToAllVHS(ovfObj, vsId, lRasd);
            string elementname = _ovfrm.GetString("RASD_UNKNOWN_ELEMENTNAME");
            if (lRasd.ElementName != null && !string.IsNullOrEmpty(lRasd.ElementName.Value))
                elementname = lRasd.ElementName.Value;
            log.DebugFormat("OVF.AddRasd added: {0}:{1}", elementname, lRasd.InstanceID.Value);
            return lRasd.InstanceID.Value;
        }

		public string AddResourceAllocationSection(EnvelopeType ovfObj, string info, bool required, RASD_Type rasd)
        {
            return AddResourceAllocationSection(ovfObj, Properties.Settings.Default.Language, info, required, rasd);
        }

		public string AddResourceAllocationSection(EnvelopeType ovfObj, string lang, string info, bool required, RASD_Type rasd)
        {
            ResourceAllocationSection_Type rasSection = null;
            ResourceAllocationSection_Type[] rasArray = FindSections<ResourceAllocationSection_Type>(ovfObj.Item.Items);
            if (rasArray != null && rasArray.Length > 0)
            {
                rasSection = rasArray[0];
            }
            else
            {
                rasSection = new ResourceAllocationSection_Type();
                rasSection.Id = Guid.NewGuid().ToString();
                rasSection.Info = new Msg_Type(AddToStringSection(ovfObj, lang, info), info);
                rasSection.required = required;
                ovfObj.Item.Items = AddSection(ovfObj.Item.Items, rasSection);
            }

            List<RASD_Type> _rasds = new List<RASD_Type>();

            if (rasSection.Item != null && rasSection.Item.Length > 0)
            {
                _rasds.AddRange(rasSection.Item);
            }

            _rasds.Add(rasd);
            rasSection.Item = _rasds.ToArray();

            return rasSection.Id;
        }

        public static Section_Type[] AddSection(Section_Type[] sections, Section_Type section)
        {
            List<Section_Type> sectionList = new List<Section_Type>();
            if (sections != null)
                sectionList.AddRange(sections);
            sectionList.Add(section);
            return sectionList.ToArray();
        }

		public static string AddStartupSection(EnvelopeType env, bool required, string vsId, long order, long startdelay, long stopdelay)
        {
            return AddStartupSection(env, required, Properties.Settings.Default.Language, vsId, order, startdelay, stopdelay);
        }

		public static string AddStartupSection(EnvelopeType env, bool required, string lang, string vsId, long order, long startdelay, long stopdelay)
        {
            string info = _ovfrm.GetString("SECTION_STARTUP_INFO");
        	StartupSection_Type startupSection = new StartupSection_Type
        	                                     	{
        	                                     		Id = Guid.NewGuid().ToString(),
        	                                     		Info = new Msg_Type(AddToStringSection(env, lang, info), info),
														required = required
        	                                     	};

			var sections = new List<Section_Type>();

			if (env.Sections != null && env.Sections.Length > 0)
			{
				foreach (var sect in env.Sections)
				{
					if (sect is StartupSection_Type)
						startupSection = (StartupSection_Type)sect;
					else
						sections.Add(sect);
				}
			}

			//create new item
			var item = new StartupSection_TypeItem
			{
				id = vsId,
				order = order,
				startAction = "powerOn",
				startDelay = startdelay,
				stopAction = "powerOff",
				stopDelay = stopdelay,
				waitingForGuest = false,
			};

			var itemList = new List<StartupSection_TypeItem>();

			if (startupSection.Item != null)
				itemList.AddRange(startupSection.Item); //store existing items

			itemList.Add(item); //add newly created
			startupSection.Item = itemList.ToArray(); //update list

			sections.Add(startupSection);
			env.Sections = sections.ToArray();
			log.Debug("OVF.AddStartupOptions completed");

			return startupSection.Id;
        }

		public static string AddToStringSection(EnvelopeType ovfObj, string message)
        {
            return AddToStringSection(ovfObj, Properties.Settings.Default.Language, message);
        }
        /// <summary>
        /// Add a string to the string section.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="lang">language identifier ie: "en-US"</param>
        /// <param name="message">Text to put in string section</param>
        /// <returns>Identifier to section.</returns>
		public static string AddToStringSection(EnvelopeType env, string lang, string message)
        {
            if (lang == null)
                return null;

            // Only create the section if the language is different than the default.
            if (lang.ToLower() == Properties.Settings.Default.Language.ToLower())
            {
                return null;
            }
            string sId = Guid.NewGuid().ToString();
            List<Strings_Type> allStrings = new List<Strings_Type>();
            Strings_Type currentLanguage = null;

            if (env.Strings != null)
            {
                foreach (Strings_Type strtype in env.Strings)
                {
                    if (strtype.lang == lang)
                    {
                        currentLanguage = strtype;
                    }
                    else
                    {
                        allStrings.Add(strtype);
                    }
                }
            }
            if (currentLanguage == null)
            {
                currentLanguage = new Strings_Type();
                currentLanguage.lang = lang;
            }
            List<Strings_TypeMsg> msgList = new List<Strings_TypeMsg>();
            if (currentLanguage.Msg != null)
            {
                msgList.AddRange(currentLanguage.Msg);
            }

            Strings_TypeMsg strMsg = new Strings_TypeMsg();
            strMsg.msgid = sId;
            strMsg.Value = message;

            msgList.Add(strMsg);
            currentLanguage.Msg = msgList.ToArray();
            allStrings.Add(currentLanguage);
            env.Strings = allStrings.ToArray();
            return sId;
        }

        public static string AddVirtualHardwareSection(EnvelopeType ovfEnv, string vsId)
        {
            return AddVirtualHardwareSection(ovfEnv, vsId, Properties.Settings.Default.Language);
        }

        public static string AddVirtualHardwareSection(EnvelopeType ovfEnv, string vsId, string lang)
        {

            VirtualHardwareSection_Type vhs = new VirtualHardwareSection_Type();
            vhs.Id = Guid.NewGuid().ToString();
            vhs.Info = new Msg_Type(AddToStringSection(ovfEnv, lang, Properties.Settings.Default.vhsSettings), Properties.Settings.Default.vhsSettings);

            if (ovfEnv.Item == null || ((VirtualSystemCollection_Type)ovfEnv.Item).Content == null)
            {
                throw new ArgumentNullException(Messages.FAILED_TO_ADD_VIRTUAL_HARDWARE_SECTION);
            }
            AddContent((VirtualSystemCollection_Type)ovfEnv.Item, vsId, vhs);
            log.DebugFormat("OVF.AddVirtualHardwareSection completed {0}", vsId);
            return vhs.Id;
        }

        public VirtualHardwareSection_Type AddVHSforVMWare(EnvelopeType ovfEnv, string vsId, VirtualHardwareSection_Type vhsTemplate)
        {
            return AddVHSforVMWare(ovfEnv, vsId, vhsTemplate, Properties.Settings.Default.Language);
        }

        public VirtualHardwareSection_Type AddVHSforVMWare(EnvelopeType ovfEnv, string vsId, VirtualHardwareSection_Type vhsTemplate, string lang)
        {
            string vhsId = AddVirtualHardwareSection(ovfEnv, vsId, lang);
            AddVirtualSystemSettingData(ovfEnv,
                                        vsId,
                                        vhsId,
                                        vhsTemplate.System.ElementName.Value,
                                        (vhsTemplate.System.Caption != null) ? vhsTemplate.System.Caption.Value : vhsTemplate.System.ElementName.Value,
                                        (vhsTemplate.System.Description != null) ? vhsTemplate.System.Description.Value : vhsTemplate.System.ElementName.Value,
                                        Guid.NewGuid().ToString(),
                                        Properties.Settings.Default.vmwHardwareType);
            VirtualHardwareSection_Type vhs = FindVirtualHardwareSection(ovfEnv, vsId, vhsId);
            vhs.Item = vhsTemplate.Item;
            return vhs;
        }

		public static string AddVirtualSystem(EnvelopeType ovfObj, string ovfname)
        {
            return AddVirtualSystem(ovfObj, Properties.Settings.Default.Language, ovfname);
        }
        /// <summary>
        /// Add a Virtual System Section to OVF
        /// MUST be done at least once.
        /// </summary>
        /// <param name="ovfObj">object of type EnvelopeType</param>
        /// <param name="lang">Language</param>
        /// <param name="ovfname">Name of the OVF</param>
        /// <returns>InstanceId of Virtual System</returns>
        public static string AddVirtualSystem(EnvelopeType ovfObj, string lang, string ovfname)
        {

            VirtualSystem_Type vs = new VirtualSystem_Type();
            vs.id = Guid.NewGuid().ToString();
            string info = _ovfrm.GetString("VIRTUAL_SYSTEM_TYPE_INFO");
            vs.Info = new Msg_Type(AddToStringSection(ovfObj, lang, info), info);
            vs.Name = new Msg_Type[1] { new Msg_Type(AddToStringSection(ovfObj, lang, ovfname), ovfname) };

            AddVirtualSystem(ovfObj, vs);
            AddOperatingSystemSection(ovfObj, vs.id, lang, null, null);

            log.Debug("OVF.AddVirtualSystem(obj,lang,ovfname) completed");
            return vs.id;
        }

		public static void AddVirtualSystem(EnvelopeType ovfEnv, VirtualSystem_Type vs)
        {
            // Collect the current virtual systems so we don't lose any.
            List<VirtualSystem_Type> virtualsystems = new List<VirtualSystem_Type>();
            if ((VirtualSystem_Type[])((VirtualSystemCollection_Type)ovfEnv.Item).Content != null)
            {
                foreach (VirtualSystem_Type vsType in (VirtualSystem_Type[])((VirtualSystemCollection_Type)ovfEnv.Item).Content)
                {
                    virtualsystems.Add(vsType);
                }
            }
            virtualsystems.Add(vs);
            ((VirtualSystemCollection_Type)ovfEnv.Item).Content = virtualsystems.ToArray();
            log.Debug("OVF.AddVirtualSystem(obj, vs)");
        }

		public static void AddVirtualSystemSettingData(EnvelopeType ovfObj, string vsId, string vhsId, string name, string caption, string description, string identifier, string systemtype)
        {
            AddVirtualSystemSettingData(ovfObj, vsId, vhsId, Properties.Settings.Default.Language, name, caption, description, identifier, systemtype);
        }
        /// <summary>
        /// Set the Virtual System Setting Data
        /// Determine how a Hypervisor will create the VM
        /// SystemType:
        /// 301 = Generally all systems exported from Microsoft will get this value.
        /// hvm-3.0-unknown = This is a NON-Paravirtualized VM.
        /// xen-3.0-unknown = This is a PARAVIRTUALIZED VM.
        /// </summary>
        /// <param name="ovfObj">object of type EnvelopeType</param>
        /// <param name="vsId">string InstanceID of Virtual System</param>
        /// <param name="lang">Language of Textual data</param>
        /// <param name="name">string name of Virtual System</param>
        /// <param name="caption">string caption short description</param>
        /// <param name="description">string description longer description</param>
        /// <param name="identifier">string VirtualSystemIdentifier</param>
        /// <param name="systemtype">string Defines the system Type valid values: "301", "hvm-3.0-unknown", "xen-3.0-unknown"</param>
        public static void AddVirtualSystemSettingData(EnvelopeType ovfObj, string vsId, string vhsId, string lang, string name, string caption, string description, string identifier, string systemtype)
        {
			VirtualHardwareSection_Type vhs = FindVirtualHardwareSection(ovfObj, vsId, vhsId);
            VSSD_Type vssd = null;
            vssd = new VSSD_Type();
            vssd.Caption = new Caption(caption);
            vssd.ElementName = new cimString(name);
            vssd.InstanceID = new cimString(Guid.NewGuid().ToString());
            vssd.VirtualSystemIdentifier = new cimString(identifier);
            vssd.VirtualSystemType = new cimString(systemtype);
            if (!string.IsNullOrEmpty(description)) // optional
            {
                vssd.Description = new cimString(description);
            }

            vhs.System = vssd;
            log.Debug("OVF.AddVirtualSystemSettingData completed");
        }
        #endregion

        #region CREATEs

        public static EnvelopeType CreateOvfEnvelope(string vmName, ulong cpuCount, ulong memory,
            string bootParams, string platformSettings, ulong diskCapacity, bool isWim, ulong additionalSpace,
            string diskPath, ulong imageLength)
        {
            EnvelopeType env = CreateEnvelope(vmName);
            string systemID = AddVirtualSystem(env, vmName);

            string hdwareSectionId = AddVirtualHardwareSection(env, systemID);
            string guid = Guid.NewGuid().ToString();
            AddVirtualSystemSettingData(env, systemID, hdwareSectionId, env.Name, Messages.VIRTUAL_MACHINE, Messages.OVF_CREATED, guid, "hvm-3.0-unknown");

            AddOtherSystemSettingData(env, systemID, "HVM_boot_policy", Properties.Settings.Default.xenBootOptions, GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_2"));

            bootParams = Properties.Settings.Default.xenBootParams + bootParams;
            AddOtherSystemSettingData(env, systemID, "HVM_boot_params", bootParams, GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_6"));

            var platformSetting = Properties.Settings.Default.xenPlatformSetting + platformSettings;
            AddOtherSystemSettingData(env, systemID, "platform", platformSetting, GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_3"));

            SetCPUs(env, systemID, cpuCount);
            SetMemory(env, systemID, memory, "MB");

            string netId = Guid.NewGuid().ToString();
            AddNetwork(env, systemID, netId, string.Format(Messages.NETWORK_NAME, 0), Messages.OVF_NET_DESCRIPTION, null);

            string diskId = Guid.NewGuid().ToString();
            ulong capacity = diskCapacity;

            if (isWim)
                capacity += additionalSpace;

            AddDisk(env, systemID, diskId, diskPath, true, Messages.OVF_DISK_CAPTION, Messages.OVF_CREATED, imageLength, capacity);

            FinalizeEnvelope(env);
            return env;
        }

        public EnvelopeType Create(DiskInfo[] vhdExports, string pathToOvf, string ovfName)
        {
            return Create(vhdExports, pathToOvf, ovfName, Properties.Settings.Default.Language);
        }
        /// <summary>
        /// Create an OVF (xml string) from local system.
        /// *** Element[0] of VHDExports MUST be the BOOT Disk ***
        /// DiskInfo.VHDFileName == The name the VHD will have after export.
        /// DiskInfo.DriveId == "PHYSICALDRIVE0","PHYSICALDRIVE1"... 
        /// </summary>
        /// <param name="vhdExports">LIST of Names to the VHD Files to be created REQUIREMENTS: 1. Element[0] MUST be the boot device</param>
        /// <param name="pathToOvf"></param>
        /// <param name="lang"></param>
        /// <param name="ovfName">Name of the OVF Package</param>
        /// <returns>xml string representing the OVF</returns>
        public EnvelopeType Create(DiskInfo[] vhdExports, string pathToOvf, string ovfName, string lang)
        {
            return ConvertPhysicaltoOVF(vhdExports, pathToOvf, ovfName, lang);
        }

		public static EnvelopeType CreateEnvelope(string ovfName)
        {
            return CreateEnvelope(ovfName, Properties.Settings.Default.Language);
        }
        /// <summary>
        /// Create an Empty OVF Structure.
        /// (Marshal as IntPtr)
        /// </summary>
        /// <param name="ovfName">Name of the ovf</param>
        /// <returns>EnvelopeType as Object</returns>
		public static EnvelopeType CreateEnvelope(string ovfName, string lang)
        {
            EnvelopeType ovfEnv = new EnvelopeType();
            ovfEnv.Name = ovfName;
            ovfEnv.id = Guid.NewGuid().ToString();
            ovfEnv.lang = Properties.Settings.Default.Language;
            ovfEnv.version = Properties.Settings.Default.ovfversion;
            ovfEnv.References = new References_Type();
            ovfEnv.Sections = null;

            VirtualSystemCollection_Type vscontent = new VirtualSystemCollection_Type();
            string info = _ovfrm.GetString("VIRTUAL_SYSTEM_COLLECTION_TYPE_INFO");
            vscontent.Info = new Msg_Type(AddToStringSection(ovfEnv, lang, info), info);
            ovfEnv.Item = vscontent;
            vscontent.id = Guid.NewGuid().ToString();
            vscontent.Content = null;

            log.DebugFormat("OVF.CreateEnvelope {0} created {1}", ovfEnv.Name, ovfEnv.id);

            return ovfEnv;
        }
        /// <summary>
        /// Create an OVA (Tar file) from the OVF and associated files.
        /// </summary>
        /// <param name="pathToOvf">Absolute path to the OVF files</param>
        /// <param name="ovfFileName">OVF file name (file.ovf)</param>
        /// <param name="compress">Compress the OVA (NOT IMPLEMENTED)</param>
        /// <param name="cleanup">Remove source files after addition to archive. (true = delete, false = keep)</param>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public void CreateOva(string pathToOvf, string ovfFileName, bool compress, bool cleanup)
        {
            ConvertOVFtoOVA(pathToOvf, ovfFileName, compress, cleanup);
        }
        /// <summary>
        /// Create an OVA (Tar file) from the OVF and associated files.
        /// </summary>
        /// <param name="pathToOvf">Absolute path to the OVF files</param>
        /// <param name="ovfFileName">OVF file name (file.ovf)</param>
        /// <param name="compress">Compress the OVA (NOT IMPLEMENTED)</param>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public void CreateOva(string pathToOvf, string ovfFileName, bool compress)
        {
            ConvertOVFtoOVA(pathToOvf, ovfFileName, compress);
        }
        #endregion

        #region REMOVEs

		public void RemoveAnnotation(EnvelopeType env, string id)
        {
            env.Item.Items = RemoveSection(env.Item.Items, id);
        }

		public void RemoveCDROM(EnvelopeType ovfObj, string vsId, string cdromId)
        {
            RemoveRasd(ovfObj, vsId, cdromId);
        }

        public void RemoveController(EnvelopeType ovfObj, string vsId, string controllerId)
        {
            RemoveRasd(ovfObj, vsId, controllerId);
        }
        /// <summary>
        /// Remove information to the RASD.Connection[0].Value,  
        /// This field is used to define specifics for XenServer such as:
        /// device=[0..3] or [0..15]
        /// sr=[name] or [uuid]
        /// network=[name], [uuid], or [bridgename]
        /// vdi=[uuid]
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">VirtualSystem Id</param>
        /// <param name="rasdId">RASD Instance Id</param>
        /// <param name="prompt">prompt string: "sr=", "device=", "network=", "vdi="...</param>
        /// <param name="uuid">name or uuid</param>
		public RASD_Type RemoveConnectionInRASD(EnvelopeType ovfObj, string vsId, string rasdId, string prompt)
        {
            RASD_Type rasd = FindRasdById(ovfObj, vsId, rasdId);
            if (rasd.Connection != null && rasd.Connection.Length > 0 &&
                rasd.Connection[0] != null && !string.IsNullOrEmpty(rasd.Connection[0].Value))
            {
                if (rasd.Connection[0].Value.ToLower().Contains(prompt))
                {
                    StringBuilder sb = new StringBuilder();
                    string[] pairs = rasd.Connection[0].Value.Split(new char[] { ',' });
                    foreach (string pair in pairs)
                    {
                        if (!pair.Trim().ToLower().StartsWith(prompt))
                        {
                            sb.AppendFormat("{0},", pair);
                        }
                    }
                    rasd.Connection[0].Value = sb.ToString();
                }
            }
            log.DebugFormat("OVF.RemoveConnectionInRASD completed {0}", vsId);
            return rasd;
        }

		public void RemoveDeploymentOption(EnvelopeType ovfObj, string deploymentId)
        {
            ovfObj.Item.Items = RemoveSection(ovfObj.Item.Items, deploymentId);
        }

		public void RemoveDisk(EnvelopeType ovfObj, string vsId, string diskId)
        {
            RASD_Type rasd = FindRasdById(ovfObj, diskId);
            RemoveDisk(ovfObj, vsId, rasd);
        }

		public void RemoveDisk(EnvelopeType ovfObj, string vsId, RASD_Type diskrasd)
        {
            string diskReference = null;
            //
            // Use in order of priority
            // 1. HostResource should have binding's if exists.
            // 2. InstanceID referenced
            // 3. Connection maybe used by Microsoft (older CIM Spec)
            //
            if (Tools.ValidateProperty("HostResource", diskrasd))
            {
                diskReference = diskrasd.HostResource[0].Value;
            }
            else if (Tools.ValidateProperty("InstanceID", diskrasd))
            {
                diskReference = diskrasd.InstanceID.Value;
            }
            else if (Tools.ValidateProperty("Connection", diskrasd))
            {
                diskReference = diskrasd.Connection[0].Value;
            }

            if (diskReference == null)
            {
                throw new InvalidDataException("OVF.RemoveDisk: Cannot find reference to the disk file.");
            }

            RemoveFileReference(ovfObj, diskReference);

            RemoveRasd(ovfObj, vsId, diskrasd.InstanceID.Value);
        }

		public void RemoveExternalFile(EnvelopeType ovfObj, string id)
        {
            List<File_Type> file = new List<File_Type>();
            foreach (File_Type _file in ovfObj.References.File)
            {
                if (_file.id == id)
                    continue;
                file.Add(_file);
            }
            ovfObj.References.File = file.ToArray();
        }

		public void RemoveEula(EnvelopeType ovfEnv, string eulaId)
        {
            List<Section_Type> sections = new List<Section_Type>();

            foreach (Section_Type section in ovfEnv.Sections)
            {
                if (section is EulaSection_Type)
                {
                    if (((EulaSection_Type)section).Id == eulaId)
                        continue;
                }
                sections.Add(section);
            }
            ovfEnv.Sections = sections.ToArray();
        }

		public void RemoveFileReference(EnvelopeType ovfObj, string id)
        {
            DiskSection_Type[] disksections = FindSections<DiskSection_Type>(ovfObj.Sections);

            if (disksections == null)
            {
                throw new InvalidDataException(Messages.OVF_DISK_SECTION_MISSING);
            }

            List<VirtualDiskDesc_Type> vdisks = new List<VirtualDiskDesc_Type>();

            foreach (VirtualDiskDesc_Type vdisk in disksections[0].Disk)
            {
                if (vdisk.diskId == id)
                {
                    RemoveExternalFile(ovfObj, vdisk.fileRef);
                    continue;
                }
                vdisks.Add(vdisk);
            }
            disksections[0].Disk = vdisks.ToArray();
        }

		public void RemoveNetwork(EnvelopeType ovfObj, string vsId, string id)
        {
            RASD_Type[] netrasds = FindRasdByType(ovfObj, vsId, 10);
            NetworkSection_Type[] netSection = FindSections<NetworkSection_Type>(ovfObj.Sections);

            Dictionary<string, int> netmaps = new Dictionary<string, int>();

            foreach (NetworkSection_TypeNetwork net in netSection[0].Network)
            {
                netmaps.Add(net.Description.msgid, 0);
            }
            // count the references to each controller
            foreach (RASD_Type rasd in netrasds)
            {
                string key = rasd.Connection[0].Value;
                if (netmaps.ContainsKey(key))
                {
                    if (rasd.InstanceID.Value == id)
                    {
                        netmaps[key]--;
                    }
                    else
                    {
                        netmaps[key]++;
                    }
                }
            }
            // remove networks no longer referenced.
            List<NetworkSection_TypeNetwork> nets = new List<NetworkSection_TypeNetwork>();
            foreach (string key in netmaps.Keys)
            {
                if (netmaps[key] > 0)
                {
                    foreach (NetworkSection_TypeNetwork net in netSection[0].Network)
                    {
                        if (net.Description.msgid == key)
                        {
                            nets.Add(net);
                        }
                    }

                }
            }

            netSection[0].Network = nets.ToArray();
            RemoveRasd(ovfObj, vsId, id);
        }

		public void RemoveOtherSystemSettingData(EnvelopeType ovfObj, string vsId, string id)
        {
            VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfObj, vsId);
            List<Xen_ConfigurationSettingData_Type> ovsocd = new List<Xen_ConfigurationSettingData_Type>();
            foreach (VirtualHardwareSection_Type vhs in vhsArray)
            {
                if (vhs.VirtualSystemOtherConfigurationData != null && vhs.VirtualSystemOtherConfigurationData.Length > 0)
                {
                    foreach (Xen_ConfigurationSettingData_Type xenc in vhs.VirtualSystemOtherConfigurationData)
                    {
                        if (xenc.id == id)
                            continue;
                        ovsocd.Add(xenc);
                    }
                }
                vhs.VirtualSystemOtherConfigurationData = ovsocd.ToArray();
            }
        }

		public void RemovePostOperation(EnvelopeType ovfObj, string vsId, string installId, string id)
        {
            InstallSection_Type installSection = FindSection<InstallSection_Type>(ovfObj.Item.Items, installId);
            if (installSection.PostInstallOperations != null)
            {
                if (installSection.PostInstallOperations.id == id)
                {
                    installSection.PostInstallOperations = null;
                }
            }
        }

		public void RemoveProductSection(EnvelopeType ovfObj, string id)
        {
            ovfObj.Item.Items = RemoveSection(ovfObj.Item.Items, id);
        }

        public void RemoveProductSectionProperty(EnvelopeType ovfObj, string productId, string id)
        {
            ProductSection_Type[] ProductSections = FindSections<ProductSection_Type>(ovfObj.Item.Items);
            ProductSection_Type _productSection = null;
            foreach (ProductSection_Type ps in ProductSections)
            {
                if (ps.Id == productId)
                {
                    _productSection = ps;
                    break;
                }
            }

            if (_productSection == null)
                throw new InvalidDataException(string.Format(Messages.OVF_PRODUCT_SECTION_MISSING, productId));

            List<object> _properties = new List<object>();
            foreach (object property in _productSection.Items)
            {
                if (property is ProductSection_TypeProperty)
                {
                    if (((ProductSection_TypeProperty)property).id == id)
                        continue;
                }
                _properties.Add(property);
            }
            _productSection.Items = _properties.ToArray();
        }

		public void RemovePostOperationCommand(EnvelopeType ovfObj, string vsId, string installId, string operationId, string commandId)
        {
            InstallSection_Type installSection = FindSection<InstallSection_Type>(ovfObj.Item.Items, installId);
            if (installSection.PostInstallOperations != null)
            {
                if (installSection.PostInstallOperations.id == operationId)
                {
                    List<Xen_PostInstallOperationCommand_Type> xenpostcmd = new List<Xen_PostInstallOperationCommand_Type>();
                    foreach (Xen_PostInstallOperationCommand_Type xencmd in installSection.PostInstallOperations.PostInstallOperationCommand)
                    {
                        if (xencmd.id == commandId)
                            continue;
                        xenpostcmd.Add(xencmd);
                    }
                    installSection.PostInstallOperations.PostInstallOperationCommand = xenpostcmd.ToArray();
                }
            }

        }
        /// <summary>
        /// Remove ANY RASD by it's Instance ID.
        /// </summary>
		/// <param name="ovfEnv">EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier (InstanceID)</param>
        /// <param name="instanceID"></param>
		public void RemoveRasd(EnvelopeType ovfEnv, string vsId, string instanceID)
        {
            List<RASD_Type> rasds = new List<RASD_Type>();

            VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfEnv, vsId);
            foreach (VirtualHardwareSection_Type vhs in vhsArray)
            {
                if (vhs.Item != null && vhs.Item.Length > 0)
                {
                    foreach (RASD_Type _rasd in vhs.Item)
                    {
                        if (!_rasd.InstanceID.Equals(instanceID))
                        {
                            rasds.Add(_rasd);
                        }
                        else
                        {
                            string elementname = _ovfrm.GetString("RASD_UNKNOWN_ELEMENTNAME");
                            if (!string.IsNullOrEmpty(_rasd.ElementName.Value))
                                elementname = _rasd.ElementName.Value;
                            log.DebugFormat("OVF.RemoveRasd deleted: {0}:{1}", elementname, _rasd.InstanceID.Value);
                        }
                    }
                }
                vhs.Item = rasds.ToArray();
            }
            log.Debug("OVF.RemoveRasd completed");
        }

        public void RemoveResourceAllocationSection(EnvelopeType ovfObj, string id)
        {
            ovfObj.Item.Items = RemoveSection(ovfObj.Item.Items, id);
        }

        public Section_Type[] RemoveSection(Section_Type[] sections, string sectionId)
        {
            List<Section_Type> sects = new List<Section_Type>();
            foreach (Section_Type section in sections)
            {
                if (section.Id != sectionId)
                {
                    sects.Add(section);
                }
            }
            return sects.ToArray();
        }

		public void RemoveStartupSection(EnvelopeType ovfObj, string vsId, string id)
        {
            VirtualSystem_Type vs = FindVirtualSystemById(ovfObj, vsId);
            vs.Items = RemoveSection(vs.Items, id);
        }

		public void RemoveStartupSectionItem(EnvelopeType ovfObj, string vsId, string startupId, string id)
        {
            VirtualSystem_Type vs = FindVirtualSystemById(ovfObj, vsId);
            if (vs == null)
                throw new InvalidDataException(string.Format(Messages.OVF_VIRTUAL_SYSTEM_MISSING, vsId));
            StartupSection_Type sst = FindSection<StartupSection_Type>(vs.Items, startupId);
            if (sst == null)
                throw new InvalidDataException(Messages.OVF_STARTUP_SECTION_MISSING);

            List<StartupSection_TypeItem> ssti = new List<StartupSection_TypeItem>();
            foreach (StartupSection_TypeItem sitem in sst.Item)
            {
                if (sitem.id == id)
                    continue;
                ssti.Add(sitem);
            }
            sst.Item = ssti.ToArray();
        }

        public void RemoveStringSection(EnvelopeType ovfObj, string id)
        {
            ovfObj.Sections = RemoveSection(ovfObj.Sections, id);
        }

		public void RemoveVirtualSystem(EnvelopeType env, string id)
        {
			RASD_Type[] _disks = FindDiskRasds(env, id);

            foreach (RASD_Type _disk in _disks)
            {
				RemoveDisk(env, id, _disk);
            }

            VirtualSystem_Type vs = null;
            if (env.Item is VirtualSystem_Type)
            {
                vs = (VirtualSystem_Type)env.Item;
                env.Item = null;
            }
            else if (env.Item is VirtualSystemCollection_Type)
            {
                VirtualSystem_Type[] vsArray = (VirtualSystem_Type[])((VirtualSystemCollection_Type)env.Item).Content;
                List<VirtualSystem_Type> vsList = new List<VirtualSystem_Type>();
                foreach (VirtualSystem_Type _vs in vsArray)
                {
                    if (_vs.id == id)
                    {
                        vs = _vs;
                        continue;
                    }
                    vsList.Add(_vs);
                }
                ((VirtualSystemCollection_Type)env.Item).Content = vsList.ToArray();
            }

            if (vs == null)
                throw new InvalidDataException(string.Format(Messages.OVF_VIRTUAL_SYSTEM_MISSING, id));



        }
        #endregion

        #region UPDATEs
        /// <summary>
        /// Update a current annotation
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="annotationId">Identifier for Annotation</param>
        /// <param name="info">Info Text</param>
        /// <param name="annotation">Annotation</param>
		public void UpdateAnnotation(EnvelopeType ovfObj, string vsId, string annotationId, string info, string annotation)
        {
            UpdateAnnotation(ovfObj, vsId, annotationId, Properties.Settings.Default.Language, info, annotation);
        }
        /// <summary>
        /// Update a current annotation
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="annotationId">Identifier for Annotation</param>
        /// <param name="lang">Language</param>
        /// <param name="info">Info Text</param>
        /// <param name="annotation">Annotation</param>
		public void UpdateAnnotation(EnvelopeType ovfObj, string vsId, string annotationId, string lang, string info, string annotation)
        {
            VirtualSystem_Type vs = FindVirtualSystemById(ovfObj, vsId);

            foreach (Section_Type section in vs.Items)
            {
                if (section is AnnotationSection_Type)
                {
                    AnnotationSection_Type annot = (AnnotationSection_Type)section;
                    if (annot.Id == annotationId)
                    {
                        if (Tools.ValidateProperty("Info", annot))
                        {
                            UpdateStringSection(ovfObj, annot.Info.msgid, info);
                        }
                        else
                        {
                            annot.Info = new Msg_Type(AddToStringSection(ovfObj, lang, info), info);
                        }
                        if (Tools.ValidateProperty("Annotation", annot))
                        {
                            UpdateStringSection(ovfObj, annot.Info.msgid, annotation);
                        }
                        else
                        {
                            annot.Annotation = new Msg_Type(AddToStringSection(ovfObj, lang, annotation), annotation);
                        }
                    }
                }
            }
           log.Debug("UpdateAnnotation Exit");
        }
        /// <summary>
        /// Add a CD/DVD Drive
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="cdId">InstanceID</param>
        /// <param name="caption">string short description</param>
        /// <param name="description">string longer description</param>
		public void UpdateCDROM(EnvelopeType ovfObj, string vsId, string cdId, string caption, string description)
        {
            UpdateCDROM(ovfObj, vsId, Properties.Settings.Default.Language, cdId, caption, description);
        }
        /// <summary>
        /// Add a CD/DVD Drive
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="lang">Language</param>
        /// <param name="cdId">InstanceID</param>
        /// <param name="caption">string short description</param>
        /// <param name="description">string longer description</param>
		public void UpdateCDROM(EnvelopeType ovfObj, string vsId, string lang, string cdId, string caption, string description)
        {
            RASD_Type rasd = FindRasdById(ovfObj, cdId);
            if (rasd == null)
            {
                throw new InvalidDataException(Messages.OVF_CDROM_MISSING);
            }

            if (Tools.ValidateProperty("Caption", rasd))
            {
                rasd.Caption.Value = caption;
            }
            else
            {
                rasd.Caption = new Caption(caption);
            }

            if (Tools.ValidateProperty("Description", rasd))
            {
                rasd.Description.Value = description;
            }
            else
            {
                rasd.Description = new cimString(description);
            }

            log.Debug("OVF.UpdateCDROM Exit");
        }
        /// <summary>
        /// Add a controller to the mix.
        /// </summary>
        /// <param name="ovfObj">object of type EnvelopeType</param>
        /// <param name="vsId">System Instance ID</param>
        /// <param name="type">Resource Type: 5 = IDE, 6 = SCSI</param>
        /// <param name="deviceId">String identifying the device to match to the controller</param>
        /// <param name="iteration">which controller 0 = first, 1, 2, 3... (per type)</param>
        /// <returns>InstanceID of Controller</returns>
		public void UpdateController(EnvelopeType ovfObj, string vsId, DeviceType type, string deviceId, int iteration)
        {
            UpdateController(ovfObj, vsId, Properties.Settings.Default.Language, deviceId, type, iteration);
        }
        /// <summary>
        /// Add a controller to the mix.
        /// </summary>
        /// <param name="ovfObj">object of type EnvelopeType</param>
        /// <param name="vsId">System Instance ID</param>
        /// <param name="lang">Language</param>
        /// <param name="type">Resource Type: 5 = IDE, 6 = SCSI</param>
        /// <param name="deviceId">String identifying the device to match to the controller</param>
        /// <param name="iteration">which controller 0 = first, 1, 2, 3... (per type)</param>
        /// <returns>InstanceID of Controller</returns>
		public void UpdateController(EnvelopeType ovfObj, string vsId, string lang, string deviceId, DeviceType type, int iteration)
        {
            RASD_Type rasd = FindRasdById(ovfObj, deviceId);
            if (rasd == null)
            {
                throw new InvalidDataException(string.Format(Messages.OVF_CONTROLLER_MISSING, deviceId));
            }

            if (rasd.ResourceType.Value != (ushort)type)
            {
                rasd.ResourceType.Value = (ushort)type;
            }

            if (!Tools.ValidateProperty("Address", rasd))
            {
                rasd.Address = new cimString(Convert.ToString(iteration));
            }
            else
            {
                rasd.Address.Value = Convert.ToString(iteration);
            }

            log.Debug("OVF.UpdateController exit");
        }
        /// <summary>
        /// Update a defined Deployment Option
        /// </summary>
        /// <param name="ovfObj"></param>
        /// <param name="id">Deployment Option Section Type Configuration Identifier</param>
        /// <param name="label">Label</param>
        /// <param name="description">Description</param>
        /// <param name="isdefault">Is default deployment options</param>
		public void UpdateDeploymentOption(EnvelopeType ovfObj, string id, string label, string description, bool isdefault)
        {
            UpdateDeploymentOption(ovfObj, id, Properties.Settings.Default.Language, label, description, isdefault);
        }
        /// <summary>
        /// Update a defined Deployment Option
        /// </summary>
		/// <param name="env"></param>
        /// <param name="id">Deployment Option Section Type Configuration Identifier</param>
        /// <param name="lang">Language</param>
        /// <param name="label">Label</param>
        /// <param name="description">Description</param>
        /// <param name="isdefault">Is default deployment options</param>
		public void UpdateDeploymentOption(EnvelopeType env, string id, string lang, string label, string description, bool isdefault)
        {
            DeploymentOptionSection_Type dos = null;
            List<Section_Type> sections = new List<Section_Type>();
            foreach (Section_Type sect in env.Sections)
            {
                if (sect is DeploymentOptionSection_Type)
                {
                    dos = (DeploymentOptionSection_Type)sect;
                    break;
                }
            }

            if (dos == null)
            {
                throw new InvalidDataException(Messages.OVF_DEPLOYMENT_SECTION_MISSING);
            }

            DeploymentOptionSection_TypeConfiguration configItem = null;
            foreach (DeploymentOptionSection_TypeConfiguration dostc in dos.Configuration)
            {
                if (dostc.id == id)
                {
                    configItem = dostc;
                    break;
                }
            }

            if (configItem == null)
            {
                throw new InvalidDataException(Messages.OVF_DEPLOYMENT_CFG_SECTION_MISSING);
            }

            configItem.@default = isdefault;
            configItem.Description.Value = UpdateStringSection(env, configItem.Description.msgid, lang, description);
            configItem.Label.Value = UpdateStringSection(env, configItem.Label.msgid, lang, label);

            return;
        }
        /// <summary>
        /// Update DISK information by RASD InstanceID
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="instanceID">RASD InstanceID</param>
        /// <param name="description">Description of VHD</param>
        /// <param name="vhdFileName">File Name of VHD (needs to be in the same directory as OVF</param>
        /// <param name="filesize">physical file size</param>
        /// <param name="capacity">capacity of disk</param>
        /// <param name="freespace">amount of free space</param>
		public void UpdateDisk(EnvelopeType ovfObj, string vsId, string instanceID, string description, string vhdFileName, ulong filesize, ulong capacity, ulong freespace)
        {
            UpdateDisk(ovfObj, vsId, Properties.Settings.Default.Language, instanceID, description, vhdFileName, filesize, capacity, freespace);
        }
        /// <summary>
        /// Update DISK information by RASD InstanceID
        /// </summary>
		/// <param name="ovfEnv">EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="lang">Language</param>
        /// <param name="instanceID">RASD InstanceID</param>
        /// <param name="description">Description of VHD</param>
        /// <param name="vhdFileName">File Name of VHD (needs to be in the same directory as OVF</param>
        /// <param name="filesize">physical file size</param>
        /// <param name="capacity">capacity of disk</param>
        /// <param name="freespace">amount of free space</param>
        public void UpdateDisk(EnvelopeType ovfEnv, string vsId, string lang, string instanceID, string description, string vhdFileName, ulong filesize, ulong capacity, ulong freespace)
        {
            List<RASD_Type> rasdList = new List<RASD_Type>();
            File_Type file = null;
            VirtualDiskDesc_Type disk = null;

            VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfEnv, vsId);
            foreach (VirtualHardwareSection_Type vhs in vhsArray)
            {
                #region FIND RASD
                if (vhs.Item != null && vhs.Item.Length > 0)
                {
                    foreach (RASD_Type _rasd in vhs.Item)
                    {
                        if (_rasd.InstanceID.Value.Equals(instanceID))
                        {
                            if (_rasd.ResourceType.Value == 15 || _rasd.ResourceType.Value == 16)
                            {
                                log.Info("Found CD in connection, skipped");
                                break;
                            }
                            rasdList.Add(_rasd);
                            break;
                        }
                    }
                }
                #endregion
            }

            #region FIND VIRTUAL DISK DESCRIPTION
            if (rasdList.Count > 0 && ovfEnv.Sections != null && ovfEnv.Sections.Length > 0)
            {
                foreach (Section_Type sect in ovfEnv.Sections)
                {
                    if (sect is DiskSection_Type)
                    {
                        DiskSection_Type ds = (DiskSection_Type)sect;
                        if (ds.Disk != null && ds.Disk.Length > 0)
                        {
                            bool f = false;
                            foreach (VirtualDiskDesc_Type vd in ds.Disk)
                            {
                                foreach (RASD_Type rasd in rasdList)
                                {
                                    if (vd.diskId.Equals(rasd.InstanceID.Value))
                                    {
                                        disk = vd;
                                        f = true;
                                        break;
                                    }
                                }
                                if (f) break;
                            }
                        }
                    }
                }
            }
            #endregion

            #region FIND FILE DEFINITION
            if (rasdList.Count > 0 && disk != null && ovfEnv.References.File != null && ovfEnv.References.File.Length > 0)
            {
                foreach (File_Type fi in ovfEnv.References.File)
                {
                    if (fi.id.Equals(disk.fileRef))
                    {
                        file = fi;
                        break;
                    }
                }
            }
            #endregion

            if (rasdList.Count <= 0 || file == null || disk == null)
            {
                throw new ArgumentException(string.Format(Messages.OVF_RASD_MISSING, instanceID));
            }

            foreach (RASD_Type rasd in rasdList)
            {
                rasd.Description.Value = description;
            }

            disk.capacity = Convert.ToString(capacity);
            long popsize = (long)(capacity - freespace);
            if (capacity != 0 && freespace != 0 &&
                freespace < capacity && popsize > 0)
            {
                disk.populatedSize = popsize;
                disk.populatedSizeSpecified = true;
            }
            file.href = string.Format(Properties.Settings.Default.FileURI, vhdFileName);
            log.Debug("OVF.UpdateDisk.1 completed");
        }
        /// <summary>
        /// Update a EULA to the OVF
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="eulaId">Identifier</param>
        /// <param name="eulafilename">Filename</param>
		public void UpdateEula(EnvelopeType ovfObj, string eulaId, string eulafilename)
        {
            UpdateEula(ovfObj, eulaId, Properties.Settings.Default.Language, eulafilename);
        }
        /// <summary>
        /// Update a EULA to the OVF
        /// </summary>
        /// <param name="ovfEnv">EnvelopeType</param>
        /// <param name="eulaId">Identifier</param>
        /// <param name="lang">Language</param>
        /// <param name="eulafilename">Filename</param>
		public void UpdateEula(EnvelopeType ovfEnv, string eulaId, string lang, string eulafilename)
        {
            EulaSection_Type updateEula = FindSection<EulaSection_Type>(ovfEnv.Sections, eulaId);

            string agreement = null;
            if (eulafilename != null)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(eulafilename, FileMode.Open, FileAccess.Read, FileShare.Read);
                    StreamReader sw = new StreamReader(fs);
                    agreement = sw.ReadToEnd();
                }
                catch (Exception ex)
                {
                    throw new InvalidDataException(Messages.OVF_CANNOT_READ_EULA, ex);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }

            updateEula.License.Value = UpdateStringSection(ovfEnv, updateEula.License.msgid, lang, agreement);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetobj"></param>
        /// <param name="fieldname"></param>
        /// <param name="value"></param>
        public static void UpdateField(object targetobj, string fieldname, object value)
        {
            PropertyInfo p = targetobj.GetType().GetProperty(fieldname);
            if (p == null)
            {
                log.ErrorFormat("PROPERTY: {0}.{1} does not exist", targetobj.GetType().Name, fieldname);
            }
            else
            {
                MethodInfo[] methods = p.GetAccessors();
                Type valuetype = Type.GetType("cimString");
                MethodInfo set = null;
                foreach (MethodInfo m in methods)
                {
                    if (m.Name.StartsWith("get"))
                    {
                        valuetype = m.ReturnType;
                    }
                    if (m.Name.StartsWith("set"))
                    {
                        set = m;
                    }
                }

                Type[] types = null;
                object newvalue = null;
                if (!fieldname.Equals("Connection") &&
                    !fieldname.Equals("HostResource"))
                {
                    types = new Type[] { typeof(string) };
                    ConstructorInfo ci = valuetype.GetConstructor(types);
                    object[] arg = new object[] { value };
                    newvalue = ci.Invoke(arg);
                }
                else
                {
                    newvalue = new cimString[] { new cimString((string)value) };
                }
                object[] newvalues = new object[] { newvalue };

                if (set != null)
                {
                    set.Invoke(targetobj, newvalues);
                }
                else
                {
                    log.ErrorFormat("{0} has no set method, read only", p.Name);
                }
            }
            log.DebugFormat("OVF.UpdateField completed {0}", fieldname);
        }
        /// <summary>
        /// Update DISK information by RASD InstanceID
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="oldfilename">Old Filename</param>
        /// <param name="newfilename">New Filename</param>
		public static void UpdateFilename(EnvelopeType ovfEnv, string oldfilename, string newfilename)
        {
            #region FIND FILE DEFINITION
            if (ovfEnv.References != null && ovfEnv.References.File != null && ovfEnv.References.File.Length > 0)
            {
                foreach (File_Type fi in ovfEnv.References.File)
                {
                    if (fi.href.ToLower().Contains(oldfilename.ToLower()))
                    {
                        fi.href = fi.href.ToLower().Replace(oldfilename.ToLower(), newfilename);
                    }
                }
            }
            else
            {
                log.ErrorFormat("Cannot replace {0} with {1} because OVF does not contain a References Section.", oldfilename, newfilename);
                throw new ArgumentException(Messages.OVF_REFERENCE_SECTION_MISSING);
            }
            #endregion

            log.Debug("OVF.UpdateFilename completed");
        }

		public void UpdateFileSizes(EnvelopeType ovfEnv, string filename, long size, long capacity)
        {
            #region FIND FILE DEFINITION
            if (ovfEnv.References != null && ovfEnv.References.File != null && ovfEnv.References.File.Length > 0)
            {
                string fileRef = null;
                foreach (File_Type fi in ovfEnv.References.File)
                {
                    if (fi.href.ToLower().Contains(filename.ToLower()))
                    {
                        fi.size = (ulong)size;
                        fi.sizeSpecified = true;
                        fileRef = fi.id;
                        break;
                    }
                }
                DiskSection_Type[] sections = FindSections<DiskSection_Type>(ovfEnv.Sections);
                if (sections != null && sections.Length > 0)
                {
                    foreach (DiskSection_Type dsksection in sections)
                    {
                        bool found = false;
                        if (dsksection.Disk != null && dsksection.Disk.Length > 0)
                        {
                            foreach (VirtualDiskDesc_Type vdisk in dsksection.Disk)
                            {
                                if (vdisk.fileRef.Contains(fileRef))
                                {
                                    vdisk.capacity = Convert.ToString(capacity);
                                    vdisk.capacityAllocationUnits = _ovfrm.GetString("VIRTUAL_DISK_DESC_CAPACITYALLOCATIONUNITS");
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (found) { break; }
                    }
                }
            }
            else
            {
                throw new ArgumentException(Messages.OVF_REFERENCE_SECTION_MISSING);
            }
            #endregion

            log.Debug("OVF.UpdateFileSize completed");
        }
        /// <summary>
        /// The InstallSection indicates that the virtual machine needs to be booted once in order to install and or configure the guest software.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="insId">Install Section Identifier</param>
        /// <param name="bootStopDelay">Specifies a delay in seconds to wait for the virtual machine to power off.</param>
        /// <param name="info">Description</param>
		public void UpdateInstallSection(EnvelopeType ovfObj, string vsId, string insId, ushort bootStopDelay, string info)
        {
            UpdateInstallSection(ovfObj, vsId, insId, bootStopDelay, Properties.Settings.Default.Language, info);
        }
        /// <summary>
        /// The InstallSection indicates that the virtual machine needs to be booted once in order to install and or configure the guest software.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="insId">Install Section Identifier</param>
        /// <param name="bootStopDelay">Specifies a delay in seconds to wait for the virtual machine to power off.</param>
        /// <param name="lang">Language</param>
        /// <param name="info">Description</param>
		public void UpdateInstallSection(EnvelopeType ovfObj, string vsId, string insId, ushort bootStopDelay, string lang, string info)
        {
            VirtualSystem_Type vSystem = FindVirtualSystemById(ovfObj, vsId);
            InstallSection_Type installSection = FindSection<InstallSection_Type>(vSystem.Items, insId);

            UpdateStringSection(ovfObj, installSection.Info.msgid, info);
            installSection.Info.Value = info;
            installSection.initialBootStopDelay = bootStopDelay;
        }
        /// <summary>
        /// Change a Network to add/remove MacAddress
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="lang">Language</param>
        /// <param name="instanceID">InstanceID of Rasd</param>
        /// <param name="macAddress">MAC address or null (to clear)</param>
        public void UpdateNetwork(EnvelopeType ovfObj, string vsId, string instanceID, string macAddress)
        {
            UpdateNetwork(ovfObj, vsId, Properties.Settings.Default.Language, instanceID, macAddress);
        }
        /// <summary>
        /// Change a Network to add/remove MacAddress
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="lang">Language</param>
        /// <param name="instanceID">InstanceID of Rasd</param>
        /// <param name="macAddress">MAC address or null (to clear)</param>
		public void UpdateNetwork(EnvelopeType ovfObj, string vsId, string lang, string instanceID, string macAddress)
        {
            List<RASD_Type> rasdList = new List<RASD_Type>();

            VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfObj, vsId);

            foreach (VirtualHardwareSection_Type vhs in vhsArray)
            {
                if (vhs.Item != null && vhs.Item.Length > 0)
                {
                    foreach (RASD_Type _rasd in vhs.Item)
                    {
                        if (_rasd.InstanceID.Equals(instanceID))
                        {
                            rasdList.Add(_rasd);
                            break;
                        }
                    }
                }
            }

            foreach (RASD_Type rasd in rasdList)
            {
                if (!string.IsNullOrEmpty(macAddress))
                {
                    rasd.Address = new cimString(macAddress);
                }
                else
                {
                    rasd.Address = null;
                }
            }
            log.Debug("OVF.UpdateNetwork completed");
        }
        /// <summary>
        /// Update the Operating System Section
        /// (replaces any previous version)
        /// </summary>
        /// <param name="ovfEnv">Ovf:EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="description">Description</param>
        /// <param name="osInfo">OS Information</param>
        public void UpdateOperatingSystemSection(EnvelopeType ovfEnv, string vsId, string description, string osInfo)
        {
            UpdateOperatingSystemSection(ovfEnv, vsId, description, osInfo, 0);
        }
        /// <summary>
        /// Update the Operating System Section
        /// (replaces any previous version)
        /// </summary>
        /// <param name="ovfEnv">Ovf:EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="lang">Language</param>
        /// <param name="description">Description</param>
        /// <param name="osInfo">OS Information</param>
        /// <param name="osid">ushort identifying the OS from CIM_OperatingSystem ValueMap</param>
        public static void UpdateOperatingSystemSection(EnvelopeType ovfEnv, string vsId, string description, string osInfo, ushort osid)
        {
            VirtualSystem_Type vs = FindVirtualSystemById(ovfEnv, vsId);
            OperatingSystemSection_Type[] ossArray = FindSections<OperatingSystemSection_Type>(vs.Items);
            foreach (OperatingSystemSection_Type oss in ossArray)
            {
                oss.Description = new Msg_Type(AddToStringSection(ovfEnv, description), description);
                oss.Info = new Msg_Type(AddToStringSection(ovfEnv, osInfo), osInfo);
                oss.id = osid;
            }
        }
        /// <summary>
        /// Update the Operating System Section
        /// (replaces any previous version)
        /// </summary>
        /// <param name="ovfEnv">Ovf:EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="lang">Language</param>
        /// <param name="description">Description</param>
        /// <param name="osInfo">OS Information</param>
        /// <param name="osid">ushort identifying the OS from CIM_OperatingSystem ValueMap</param>
        public void UpdateOperatingSystemSection(EnvelopeType ovfEnv, string vsId, string lang, string description, string osInfo, ushort osid)
        {
            AddOperatingSystemSection(ovfEnv, vsId, lang, description, osInfo, osid);
        }
        /// <summary>
        /// Add XEN Specific configuration Items.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="ossdId">Identifier</param>
        /// <param name="name">Name of Parameter: is:  HVM-boot-policy (case sensitive)</param>
        /// <param name="value">value for the parameter</param>
        /// <param name="description">Description of parameter</param>
		public void UpdateOtherSystemSettingData(EnvelopeType ovfObj, string vsId, string ossdId, string name, string value, string description)
        {
            UpdateOtherSystemSettingData(ovfObj, vsId, ossdId, Properties.Settings.Default.Language, name, value, description);
        }
        /// <summary>
        /// Add XEN Specific configuration Items.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Identifier</param>
        /// <param name="ossdId">Identifier</param>
        /// <param name="lang">Language</param>
        /// <param name="name">Name of Parameter: is:  HVM-boot-policy (case sensitive)</param>
        /// <param name="value">value for the parameter</param>
        /// <param name="description">Description of parameter</param>
		public void UpdateOtherSystemSettingData(EnvelopeType ovfObj, string vsId, string ossdId, string lang, string name, string value, string description)
        {
            VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfObj, vsId);
            List<Xen_ConfigurationSettingData_Type> xencfgList = new List<Xen_ConfigurationSettingData_Type>();

            foreach (VirtualHardwareSection_Type vhs in vhsArray)
            {
                if (vhs.VirtualSystemOtherConfigurationData != null && vhs.VirtualSystemOtherConfigurationData.Length > 0)
                {
                    foreach (Xen_ConfigurationSettingData_Type xencsd in vhs.VirtualSystemOtherConfigurationData)
                    {
                        if (xencsd.id == ossdId)
                        {
                            xencfgList.Add(xencsd);
                            break;
                        }
                    }
                }
            }

            foreach (Xen_ConfigurationSettingData_Type xencfg in xencfgList)
            {
                xencfg.Name = name;
                xencfg.Value.Value = value;
                UpdateStringSection(ovfObj, xencfg.Info.msgid, description);
                xencfg.Info.Value = description;
            }
        }
        /// <summary>
        /// Citrix Extension: Define a Post Installation Operation
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="postId">Identifier</param>
        /// <param name="message">Free form message about post install</param>
		public void UpdatePostInstallOperation(EnvelopeType ovfObj, string vsId, string postId, string message)
        {
            UpdatePostInstallOperation(ovfObj, vsId, postId, Properties.Settings.Default.Language, message);
        }
        /// <summary>
        /// Citrix Extension: Define a Post Installation Operation
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="postId">Identifier</param>
        /// <param name="lang">Language</param>
        /// <param name="message">Free form message about post install</param>
		public void UpdatePostInstallOperation(EnvelopeType ovfObj, string vsId, string postId, string lang, string message)
        {
            VirtualSystem_Type vSystem = FindVirtualSystemById(ovfObj, vsId);
            Xen_PostInstallOperation_Type postOperation = null;
            InstallSection_Type[] installSections = FindSectionsByType<InstallSection_Type>(ovfObj, vsId);
            foreach (InstallSection_Type sec in installSections)
            {
                if (sec.PostInstallOperations.id == postId)
                {
                    postOperation = sec.PostInstallOperations;
                    break;
                }
            }

            if (postOperation == null)
                throw new InvalidDataException(Messages.OVF_POST_INSTALL_MISSING);

            postOperation.Info.Value = UpdateStringSection(ovfObj, postOperation.Info.msgid, lang, message);
        }
        /// <summary>
        /// Update a Post Install Operation Command
        /// This is a Citrix Extension to the InstallSection.
        /// Provides the ability to execute a series of command after the Initial Startup.
        /// In a specific case, the VM is imported then is booted from the ISO file to perform fixups upon the attached hard disk image.
        /// The vm will auto shutdown where these commands will then be executed which in this case is to disconnect the iso image and 
        /// reset the BIOS boot order to boot from the disk first.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="postId">PostInstallOperation Identifier</param>
        /// <param name="operId">Operation Identifier</param>
        /// <param name="order">Specifies the startup order using non-negative integer values.  The order of execution of the post action is the numerical ascending order of the values. Items with the same order identifier may be started up concurrently.</param>
        /// <param name="operation">Operation Name</param>
        /// <param name="value">Values for operation</param>
		public void UpdatePostInstallOperationCommand(EnvelopeType ovfObj, string vsId, string postId, string operId, uint order, string operation, string value)
        {
            UpdatePostInstallOperationCommand(ovfObj, vsId, Properties.Settings.Default.Language, postId, operId, order, operation, value);
        }
        /// <summary>
        /// Update a Post Install Operation Command
        /// This is a Citrix Extension to the InstallSection.
        /// Provides the ability to execute a series of command after the Initial Startup.
        /// In a specific case, the VM is imported then is booted from the ISO file to perform fixups upon the attached hard disk image.
        /// The vm will auto shutdown where these commands will then be executed which in this case is to disconnect the iso image and 
        /// reset the BIOS boot order to boot from the disk first.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="lang">Language</param>
        /// <param name="postId">PostInstallOperation Identifier</param>
        /// <param name="operId">Operation Identifier</param>
        /// <param name="order">Specifies the startup order using non-negative integer values.  The order of execution of the post action is the numerical ascending order of the values. Items with the same order identifier may be started up concurrently.</param>
        /// <param name="operation">Operation Name</param>
        /// <param name="value">Values for operation</param>
		public void UpdatePostInstallOperationCommand(EnvelopeType ovfObj, string vsId, string lang, string postId, string operId, uint order, string operation, string value)
        {
            VirtualSystem_Type vSystem = FindVirtualSystemById(ovfObj, vsId);
            Xen_PostInstallOperation_Type postOperation = null;
            Xen_PostInstallOperationCommand_Type postCommand = null;
            InstallSection_Type[] installSections = FindSectionsByType<InstallSection_Type>(ovfObj, vsId);

            foreach (InstallSection_Type sec in installSections)
            {
                if (sec.PostInstallOperations.id == postId)
                {
                    postOperation = sec.PostInstallOperations;
                    break;
                }
            }

            if (postOperation == null)
                throw new InvalidDataException(Messages.OVF_POST_INSTALL_MISSING);

            if (postOperation.PostInstallOperationCommand == null || postOperation.PostInstallOperationCommand.Length <= 0)
                throw new InvalidDataException(Messages.OVF_POST_INSTALL_OPERATION_MISSING);

            foreach (Xen_PostInstallOperationCommand_Type postCmd in postOperation.PostInstallOperationCommand)
            {
                if (postCmd.id == operId)
                {
                    postCommand = postCmd;
                    break;
                }
            }

            if (postCommand == null)
                throw new InvalidDataException(Messages.OVF_POST_INSTALL_OPERATION_MISSING);

            postCommand.Operation = operation;
            postCommand.Order = order;
            postCommand.Value = value;

        }
        /// <summary>
        /// Add a product section definition
        /// </summary>
        /// <param name="ovfObj">Envelope Type</param>
        /// <param name="prodId">Product Id</param>
        /// <param name="nameSpace">unique name space</param>
        /// <param name="info">Text</param>
        /// <param name="product">Product Description</param>
        /// <param name="vendor">Vendor name</param>
        /// <param name="version">Version</param>
        /// <param name="producturl">URL for Product</param>
        /// <param name="vendorurl">URL for Vendor</param>
		public void UpdateProductSection(EnvelopeType ovfObj, string prodId, string nameSpace, string info, string product, string vendor, string version, string producturl, string vendorurl)
        {
            UpdateProductSection(ovfObj, prodId, Properties.Settings.Default.Language, nameSpace, info, product, vendor, version, producturl, vendorurl);
        }
        /// <summary>
        /// Add a product section definition
        /// </summary>
        /// <param name="ovfObj">Envelope Type</param>
        /// <param name="prodId">Product Id</param>
        /// <param name="lang">Language ie: en-US</param>
        /// <param name="nameSpace">unique name space</param>
        /// <param name="info">Text</param>
        /// <param name="product">Product Description</param>
        /// <param name="vendor">Vendor name</param>
        /// <param name="version">Version</param>
        /// <param name="producturl">URL for Product</param>
        /// <param name="vendorurl">URL for Vendor</param>
		public void UpdateProductSection(EnvelopeType ovfObj, string prodId, string lang, string nameSpace, string info, string product, string vendor, string version, string producturl, string vendorurl)
        {
            ProductSection_Type productSection = FindSection<ProductSection_Type>(ovfObj.Sections, prodId);

            productSection.@class = nameSpace;
            productSection.Product.Value = UpdateStringSection(ovfObj, productSection.Product.msgid, lang, product);
            productSection.ProductUrl = new cimString(producturl);
            productSection.Vendor.Value = UpdateStringSection(ovfObj, productSection.Vendor.msgid, lang, vendor);
            productSection.Version = new cimString(version);
            productSection.VendorUrl = new cimString(vendorurl);
        }
        /// <summary>
        /// Add property to Product Section
        /// </summary>
        /// <param name="ovfObj">Envelope Type</param>
        /// <param name="psId">Product Section Identifier</param>
        /// <param name="propId">Property Identifier</param>
        /// <param name="category">A grouping mechanism for individual properties</param>
        /// <param name="key">unique reference</param>
        /// <param name="type">valid types: uint8, sint8, uint16, sint16, uint32, sint32, uint64, sint64, string, boolean, real32, real64</param>
        /// <param name="userConfig">Force the selection of specific value choices, default: false</param>
        /// <param name="label">label for config</param>
        /// <param name="description">description of configuration item</param>
		public void UpdateProductSectionProperty(EnvelopeType ovfObj, string psId, string propId, string category, string key, string type, bool userConfig, string label, string description)
        {
            UpdateProductSectionProperty(ovfObj, psId, propId, Properties.Settings.Default.Language, category, key, type, userConfig, label, description);
        }
        /// <summary>
        /// Add property to Product Section
        /// </summary>
        /// <param name="ovfObj">Envelope Type</param>
        /// <param name="psId">Product Section Identifier</param>
        /// <param name="propId">Property Identifier</param>
        /// <param name="lang">Language: en-US</param>
        /// <param name="category">A grouping mechanism for individual properties</param>
        /// <param name="key">unique reference</param>
        /// <param name="type">valid types: uint8, sint8, uint16, sint16, uint32, sint32, uint64, sint64, string, boolean, real32, real64</param>
        /// <param name="userConfig">Force the selection of specific value choices, default: false</param>
        /// <param name="label">label for config</param>
        /// <param name="description">description of configuration item</param>
		public void UpdateProductSectionProperty(EnvelopeType ovfObj, string psId, string propId, string lang, string category, string key, string type, bool userConfig, string label, string description)
        {

            ProductSection_Type productSection = FindSection<ProductSection_Type>(ovfObj.Sections, psId);

            if (productSection == null || productSection.Items == null)
                throw new InvalidDataException(string.Format(Messages.OVF_PRODUCT_SECTION_MISSING, psId));

            ProductSection_TypeProperty property = null;
            foreach (object obj in productSection.Items)
            {
                if (obj is ProductSection_TypeProperty)
                {
                    ProductSection_TypeProperty pstp = (ProductSection_TypeProperty)obj;
                    if (pstp.id == propId)
                    {
                        property = pstp;
                        break;
                    }
                }
            }

            if (property == null)
                throw new InvalidDataException(string.Format(Messages.OVF_PRODUCT_PROPERTY_MISSING, propId));

            property.key = key;
            property.type = type;
            property.Label.Value = UpdateStringSection(ovfObj, property.Label.msgid, lang, label);
            property.Description.Value = UpdateStringSection(ovfObj, property.Label.msgid, lang, description);

        }
        /// <summary>
        /// Add a specific RASD to the ResourceAllocationSection.
        /// </summary>
        /// <param name="ovfObj">Envelope</param>
        /// <param name="id">Id of ResourceAllocationSection</param>
        /// <param name="info">Free form text</param>
        /// <param name="required">defines if required, default false.</param>
        /// <param name="rasd">RASD_Type rasd to add.</param>
		public void UpdateResourceAllocationSection(EnvelopeType ovfObj, string id, string info, bool required, RASD_Type rasd)
        {
            UpdateResourceAllocationSection(ovfObj, id, Properties.Settings.Default.Language, info, required, rasd);
        }
        /// <summary>
        /// Add a specific RASD to the ResourceAllocationSection.
        /// </summary>
        /// <param name="ovfObj">Envelope</param>
        /// <param name="id">Id of ResourceAllocationSection</param>
        /// <param name="lang">Language</param>
        /// <param name="info">Free form text</param>
        /// <param name="required">defines if required, default false.</param>
        /// <param name="rasd">RASD_Type rasd to add.</param>
		public void UpdateResourceAllocationSection(EnvelopeType ovfObj, string id, string lang, string info, bool required, RASD_Type rasd)
        {
            ResourceAllocationSection_Type resource = FindSection<ResourceAllocationSection_Type>(ovfObj.Item.Items, id);

            if (resource != null)
            {
                List<RASD_Type> _rasds = new List<RASD_Type>();
                resource.Info.Value = UpdateStringSection(ovfObj, resource.Info.msgid, lang, info);
                resource.required = required;
                _rasds.Add(rasd);
                if (resource.Item != null && resource.Item.Length > 0)
                {
                    foreach (RASD_Type __rasd in resource.Item)
                    {
                        if (rasd.InstanceID.Value != __rasd.InstanceID.Value)
                        {
                            _rasds.Add(__rasd);
                        }
                    }
                }
                resource.Item = _rasds.ToArray();
            }
        }
        /// <summary>
        /// Update any field in a RASD
        /// </summary>
        /// <param name="ovfObj">OVF Envelope Object</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="rasdId">RASD.InstanceId</param>
        /// <param name="fieldname">Name of the field to update</param>
        /// <param name="value">value to set field to.</param>
		public void UpdateResourceAllocationSettingData(EnvelopeType ovfObj, string vsId, string rasdId, string fieldname, object value)
        {
            UpdateResourceAllocationSettingData(ovfObj, vsId, Properties.Settings.Default.Language, rasdId, fieldname, value);
        }
        /// <summary>
        /// Update any field in a RASD
        /// </summary>
        /// <param name="ovfObj">OVF Envelope Object</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="lang">Language</param>
        /// <param name="rasdId">RASD.InstanceId</param>
        /// <param name="fieldname">Name of the field to update</param>
        /// <param name="value">value to set field to.</param>
		public void UpdateResourceAllocationSettingData(EnvelopeType ovfObj, string vsId, string lang, string rasdId, string fieldname, object value)
        {
			VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfObj, vsId);
            foreach (VirtualHardwareSection_Type vhs in vhsArray)
            {
                if (vhs.Item != null && vhs.Item.Length > 0)
                {
                    foreach (RASD_Type rasd in vhs.Item)
                    {
                        if (rasd.InstanceID.Value.ToLower().Equals(rasdId.ToLower()))
                        {
                            UpdateField(rasd, fieldname, value);
                            break;
                        }
                    }
                }
            }
            log.Debug("OVF.UpdateResourceAllocationSettingData completed");
        }
        /// <summary>
        /// Startup Section wrapper.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="ssId">Startup Section Id</param>
        /// <param name="required">Specifies if this section is Required to be processed.</param>
        /// <param name="message">Free form message describing the startup section</param>
		public void UpdateStartupSection(EnvelopeType ovfObj, string vsId, string ssId, bool required, string message)
        {
            UpdateStartupSection(ovfObj, vsId, ssId, required, Properties.Settings.Default.Language, message);
        }
        /// <summary>
        /// Startup Section wrapper.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="ssId">Startup Section Id</param>
        /// <param name="required">Specifies if this section is Required to be processed.</param>
        /// <param name="lang">Language</param>
        /// <param name="message">Free form message describing the startup section</param>
		public void UpdateStartupSection(EnvelopeType ovfObj, string vsId, string ssId, bool required, string lang, string message)
        {
            VirtualSystem_Type vs = FindVirtualSystemById(ovfObj, vsId);
            StartupSection_Type ss = FindSection<StartupSection_Type>(vs.Items, ssId);

            if (ss == null)
                throw new InvalidDataException(Messages.OVF_STARTUP_SECTION_MISSING);

            ss.Info.Value = UpdateStringSection(ovfObj, ss.Info.msgid, lang, message);
            ss.required = required;
        }
        /// <summary>
        /// Set the startup section options.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="ssId">Startup Section Id</param>
        /// <param name="order">Specifies the startup order using non-negative integer values.  The order of execution of the start action is the numerical ascending order of the values. Items with the same order identifier may be started up concurrently.  The order of execution of the stop action is the numerical descending order of the values.</param>
        /// <param name="startdelay">Specifies a delay in seconds to wait until proceeding to the next order in the start sequence.  The default value is 0.</param>
        /// <param name="stopdelay">Specifies a delay in seconds to wait until proceeding to the previous order in the stop sequence. The default value is 0.</param>
        /// <param name="startaction">Specifies the start action to use.  Valid values are: PowerOn, None. The default value is PowerOn</param>
        /// <param name="stopaction">Specifies the start action to use.  Valid values are: PowerOff, guestShutdown, None. The default value is PowerOff</param>
        /// <param name="waitforguest">Enables the platform to resume the startup sequence after the guest software has reported it is ready.  The interpretation of this is deployment platform specific. The default value is FALSE.</param>
		public void UpdateStartupSectionItem(EnvelopeType ovfObj, string vsId, string ssId, ushort order, ushort startdelay, ushort stopdelay, string startaction, string stopaction, bool waitforguest)
        {
            UpdateStartupSectionItem(ovfObj, vsId, ssId, Properties.Settings.Default.Language, order, startdelay, stopdelay, startaction, stopaction, waitforguest);
        }
        /// <summary>
        /// Set the startup section options.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="ssId">Startup Section Id</param>
        /// <param name="lang">Language</param>
        /// <param name="order">Specifies the startup order using non-negative integer values.  The order of execution of the start action is the numerical ascending order of the values. Items with the same order identifier may be started up concurrently.  The order of execution of the stop action is the numerical descending order of the values.</param>
        /// <param name="startdelay">Specifies a delay in seconds to wait until proceeding to the next order in the start sequence.  The default value is 0.</param>
        /// <param name="stopdelay">Specifies a delay in seconds to wait until proceeding to the previous order in the stop sequence. The default value is 0.</param>
        /// <param name="startaction">Specifies the start action to use.  Valid values are: PowerOn, None. The default value is PowerOn</param>
        /// <param name="stopaction">Specifies the start action to use.  Valid values are: PowerOff, guestShutdown, None. The default value is PowerOff</param>
        /// <param name="waitforguest">Enables the platform to resume the startup sequence aftet the guest software has reported it is ready.  The interpretation of this is deployment platform specific. The default value is FALSE.</param>
		public void UpdateStartupSectionItem(EnvelopeType ovfObj, string vsId, string ssId, string lang, ushort order, ushort startdelay, ushort stopdelay, string startaction, string stopaction, bool waitforguest)
        {
            VirtualSystem_Type vs = FindVirtualSystemById(ovfObj, vsId);
            StartupSection_Type ss = FindSection<StartupSection_Type>(vs.Items, ssId);

            if (ss == null)
                throw new InvalidDataException(Messages.OVF_STARTUP_SECTION_MISSING);


            StartupSection_TypeItem sitem = null;

            if (ss.Item == null || ss.Item.Length <= 0)
                throw new InvalidDataException(Messages.OVF_STARTUP_SECTION_MISSING);

            foreach (StartupSection_TypeItem _item in ss.Item)
            {
                if (_item.id == ssId)
                {
                    sitem = _item;
                    break;
                }
            }

            if (sitem == null)
                throw new InvalidDataException(Messages.OVF_STARTUP_SECTION_MISSING);

            sitem.order = order;
            sitem.startAction = startaction;
            sitem.startDelay = startdelay;
            sitem.stopAction = stopaction;
            sitem.stopDelay = stopdelay;
            sitem.waitingForGuest = waitforguest;
        }
        /// <summary>
        /// Update a string to the string section.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="ssId">String Section Identifier</param>
        /// <param name="message">Text to put in string section</param>
        /// <returns>Identifier to section.</returns>
		public string UpdateStringSection(EnvelopeType ovfObj, string ssId, string message)
        {
            return UpdateStringSection(ovfObj, ssId, Properties.Settings.Default.Language, message);
        }
        /// <summary>
        /// Update a string to the string section.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="ssId">String Section Identifier</param>
        /// <param name="lang">language identifier ie: "en-US"</param>
        /// <param name="message">Text to put in string section</param>
        /// <returns>Identifier to section.</returns>
		public string UpdateStringSection(EnvelopeType ovfObj, string ssId, string lang, string message)
        {
            List<Strings_Type> allStrings = new List<Strings_Type>();
            Strings_Type currentLanguage = null;

			if (ovfObj.Strings != null)
            {
				foreach (Strings_Type strtype in ovfObj.Strings)
                {
                    if (strtype.lang == lang)
                    {
                        currentLanguage = strtype;
                    }
                    else
                    {
                        allStrings.Add(strtype);
                    }
                }
            }
            else
            {
                log.Info("No String Section, operation skipped");
                return message;
            }
            if (currentLanguage == null)
            {
                throw new InvalidDataException(string.Format(Messages.OVF_STRINGS_SECTION_MISSING, lang));
            }

            List<Strings_TypeMsg> msgList = new List<Strings_TypeMsg>();
            if (currentLanguage.Msg != null)
            {
                bool updated = false;
                foreach (Strings_TypeMsg strmsg in currentLanguage.Msg)
                {
                    if (strmsg.msgid == ssId)
                    {
                        strmsg.Value = message;
                        updated = true;
                        break;
                    }
                }

                if (!updated)
                {
                    throw new InvalidDataException(string.Format(Messages.OVF_STRINGS_SECTION_MISSING_MSG, ssId, lang));
                }
            }

            return message;
        }
        /// <summary>
        /// Add a Virtual System Section to OVF
        /// MUST be done at least once.
        /// </summary>
        /// <param name="ovfObj">object of type EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="name">Name of the OVF</param>
        /// <param name="info">information string</param>
        /// <returns>InstanceId of Virtual System</returns>
		public void UpdateVirtualSystem(EnvelopeType ovfObj, string vsId, string name, string info)
        {
            UpdateVirtualSystem(ovfObj, vsId, Properties.Settings.Default.Language, name, info);
        }
        /// <summary>
        /// Add a Virtual System Section to OVF
        /// MUST be done at least once.
        /// </summary>
        /// <param name="ovfObj">object of type EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="lang">Language</param>
        /// <param name="name">Name of the OVF</param>
        /// <param name="info">information string</param>
        /// <returns>InstanceId of Virtual System</returns>
		public void UpdateVirtualSystem(EnvelopeType ovfObj, string vsId, string lang, string name, string info)
        {
            VirtualSystem_Type vs = FindVirtualSystemById(ovfObj, vsId);

            vs.Info.Value = UpdateStringSection(ovfObj, vs.Info.msgid, info);
            if (vs.Name != null && vs.Name.Length > 0)
            {
                vs.Name[0].Value = UpdateStringSection(ovfObj, vs.Name[0].msgid, name);
            }
            else
            {
                vs.Name = new Msg_Type[1] { new Msg_Type(AddToStringSection(ovfObj, lang, name), name) };
            }
            log.Debug("OVF.UpdateVirtualSystem completed");
        }
        /// <summary>
        /// Update the name field, over write[0] if present add if not.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">virtual system identifier</param>
        /// <param name="name">name to give virtual system</param>
		public void UpdateVirtualSystemName(EnvelopeType ovfObj, string vsId, string name)
        {
            UpdateVirtualSystemName(ovfObj, vsId, Properties.Settings.Default.Language, name);
        }
        /// <summary>
        /// Update the name field, over write[0] if present add if not.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">virtual system identifier</param>
        /// <param name="lang"></param>
        /// <param name="name">name to give virtual system</param>
		public static void UpdateVirtualSystemName(EnvelopeType ovfObj, string vsId, string lang, string name)
        {
            foreach (VirtualSystem_Type vsType in (VirtualSystem_Type[])(((VirtualSystemCollection_Type)ovfObj.Item)).Content)
            {
                if (vsType.id.Equals(vsId))
                {
                    if (!Tools.ValidateProperty("Name", vsType))
                    {
						vsType.Name = new Msg_Type[1] { new Msg_Type(AddToStringSection(ovfObj, lang, name), name) };
                    }
                    break;
                }
            }
        }
        /// <summary>
        /// Helper Method: Update a field in the VirtualSystemSettingData (VSSD)
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="fieldname">Field name to update</param>
        /// <param name="value">value to set field</param>
		public void UpdateVirtualSystemSettingData(EnvelopeType ovfObj, string vsId, string fieldname, object value)
        {
            UpdateVirtualSystemSettingData(ovfObj, vsId, Properties.Settings.Default.Language, fieldname, value);
        }
        /// <summary>
        /// Helper Method: Update a field in the VirtualSystemSettingData (VSSD)
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="lang">Language of Info</param>
        /// <param name="fieldname">Field name to update</param>
        /// <param name="value">value to set field</param>
        public void UpdateVirtualSystemSettingData(EnvelopeType ovfObj, string vsId, string lang, string fieldname, object value)
        {
			VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfObj, vsId);
            foreach (VirtualHardwareSection_Type vhs in vhsArray)
            {
                if (vhs.System == null)
                {
                    vhs.System = new VSSD_Type();
                    vhs.System.InstanceID = new cimString(Guid.NewGuid().ToString());
                }
                UpdateField(vhs.System, fieldname, value);
            }
            log.Debug("OVF.UpdateResourceAllocationSettingData completed");
        }

		public void UpdateVirtualSystemSettingData(EnvelopeType ovfObj, string vsId, string vhsId, string lang, string fieldname, object value)
        {
            VirtualHardwareSection_Type vhs = FindVirtualHardwareSection(ovfObj, vsId, vhsId);
            if (vhs.System == null)
            {
                vhs.System = new VSSD_Type();
                vhs.System.InstanceID = new cimString(Guid.NewGuid().ToString());
            }
            UpdateField(vhs.System, fieldname, value);
            log.Debug("OVF.UpdateResourceAllocationSettingData completed");
        }
        #endregion

        #region FINDs
        /// <summary>
        /// Provided the full path / filename of an OVF return a list of 
        /// filenames/references contained with in the OVF.
        /// </summary>
        /// <param name="filename">fullpath/filename</param>
        /// <returns>array of filenames/references</returns>
        public string[] FindAttachedFiles(string filename)
        {
            return FindAttachedFiles(Load(filename));
        }
        /// <summary>
        /// Find all attached filenames.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <returns>string array of filename/references</returns>
		public string[] FindAttachedFiles(EnvelopeType env)
        {
            List<string> files = new List<string>();

            if (env.References != null && env.References.File != null && env.References.File.Length > 0)
            {
                foreach (File_Type filetype in env.References.File)
                {
                    files.Add(filetype.href);
                }
            }
            return files.ToArray();
        }
        /// <summary>
        /// Find the boot disk 
        /// </summary>
        /// <param name="ovfObj">Envelope Type</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <returns>the RASD of the BootDisk</returns>
		public RASD_Type FindCurrentBootDisk(EnvelopeType ovfObj, string vsId)
        {
            RASD_Type rasd = null;
            RASD_Type[] disks = FindDiskRasds(ovfObj, vsId);
            foreach (RASD_Type _rasd in disks)
            {
                if (IsDeviceBootable(ovfObj, vsId, _rasd))
                {
                    rasd = _rasd;
                    break;
                }
            }
            return rasd;
        }
        /// <summary>
        /// Scan the RASDs to find the next Available Xen device.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <returns>string containing the value of the next available device</returns>
		public string FindNextDeviceId(EnvelopeType ovfObj, string vsId)
        {
            RASD_Type[] disks = FindDiskRasds(ovfObj, vsId);

            int device = 0;

            foreach (RASD_Type _rasd in disks)
            {
                if (Tools.ValidateProperty("Connection", _rasd))
                {
                    if (_rasd.Connection[0].Value.ToLower().Contains("device="))
                    {
                        string[] values = _rasd.Connection[0].Value.Split(new char[] { ',' });
                        foreach (string _value in values)
                        {
                            if (_value.ToLower().StartsWith("device="))
                            {
                                int cDevice = Convert.ToInt32(((string[])_value.Split(new char[] { '=' }))[1]);
                                if (cDevice > device)
                                {
                                    device = cDevice;
                                }
                            }
                        }
                    }
                }
            }

            return Convert.ToString(++device);
        }
        /// <summary>
        /// Given a RASD find it's parent.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="rasd">RASD_Type to find parent</param>
        /// <returns>RASD of Parent</returns>
		public RASD_Type FindParentDevice(EnvelopeType ovfObj, string vsId, RASD_Type rasd)
        {
            RASD_Type _rasd = null;
            if (Tools.ValidateProperty("Parent", rasd))
            {
                string testId = null;
                if (rasd.Parent.Value.ToLower().Contains("msvm_resourceallocationsettingdata.instanceid="))
                {
                    testId = ((string[])rasd.Parent.Value.Split(new char[] { '=' }))[1].Replace("\"", "").Replace("\\", "");
                }
                else
                {
                    testId = rasd.Parent.Value;
                }
                RASD_Type[] _rasds = FindAllRasds(ovfObj, vsId);
                foreach (RASD_Type __rasd in _rasds)
                {
                    string instancestr = __rasd.InstanceID.Value.Replace("\\", "");
                    if (instancestr.Equals(testId))
                    {
                        _rasd = __rasd;
                        break;
                    }
                }
            }
            return _rasd;
        }
        /// <summary>
        /// Find ALL RASDs attached to a Virtual System.
        /// </summary>
        /// <param name="ovfObj">Envelope Type</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <returns>Array of RASDs or NULL</returns>
		public RASD_Type[] FindAllRasds(EnvelopeType ovfObj, string vsId)
        {
            List<RASD_Type> rasds = new List<RASD_Type>();
            VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfObj, vsId);

            foreach (VirtualHardwareSection_Type vhs in vhsArray)
            {
                rasds.AddRange(vhs.Item);
            }

            return rasds.ToArray();
        }
        /// <summary>
        /// Helper:  Find ALL disk RASDs Attached to a Virtual System.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id.</param>
        /// <returns>Array of RASDs or NULL</returns>
		public static RASD_Type[] FindDiskRasds(EnvelopeType ovfObj, string vsId)
        {
            List<RASD_Type> diskRasds = new List<RASD_Type>();
            VirtualSystem_Type vSystem = FindVirtualSystemById(ovfObj, vsId);

            if (vSystem == null)
            {
                throw new InvalidDataException(string.Format(Messages.OVF_VIRTUAL_SYSTEM_MISSING, vsId));
            }

            foreach (object item in vSystem.Items)
            {
                if (item is VirtualHardwareSection_Type)
                {
                    VirtualHardwareSection_Type vhs = (VirtualHardwareSection_Type)item;
                    foreach (RASD_Type rasd in vhs.Item)
                    {
                        if (IsDiskRasd(rasd))
                        {
                            diskRasds.Add(rasd);
                        }
                    }
                }
            }
            log.DebugFormat("OVF.FindDiskRasds completed, {0} found", diskRasds.Count);
            return diskRasds.ToArray();
        }
        /// <summary>
        /// Find a Systems RASDs by Resource Type.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="resourceType">Resource Type Value</param>
        /// <returns>Array of RASDs or NULL</returns>
		public static RASD_Type[] FindRasdByType(EnvelopeType ovfObj, string vsId, uint resourceType)
        {
            List<RASD_Type> returnRasds = new List<RASD_Type>();
            VirtualSystem_Type vSystem = FindVirtualSystemById(ovfObj, vsId);

            if (vSystem == null)
            {
                throw new InvalidDataException(string.Format(Messages.OVF_VIRTUAL_SYSTEM_MISSING, vsId));
            }

            foreach (object item in vSystem.Items)
            {
                if (item is VirtualHardwareSection_Type)
                {
                    returnRasds.AddRange(FindRasdByType(item, resourceType));
                }
            }
            log.Debug("OVF.FindRasdByType completed");
            return returnRasds.ToArray();
        }
        /// <summary>
        /// Find a RASD by it ResourceType value.
        /// </summary>
        /// <param name="vhsObj">VirtualHardwareSection_Type</param>
        /// <param name="resourceType">uint resource type value</param>
        /// <returns>RASD_Type[] an array of RASDs or NULL</returns>
        public static RASD_Type[] FindRasdByType(object vhsObj, uint resourceType)
        {
            List<RASD_Type> rasds = new List<RASD_Type>();
            if (!(vhsObj is VirtualHardwareSection_Type))
            {
                throw new InvalidCastException(Messages.OVF_VHS_MISSING);
            }
            VirtualHardwareSection_Type vhs = (VirtualHardwareSection_Type)vhsObj;
            foreach (RASD_Type _rasd in vhs.Item)
            {
                if (_rasd.ResourceType.Value == resourceType)
                {
                    rasds.Add(_rasd);
                }
            }
            log.DebugFormat("OVF.FindRasdByType completed, {0} found", rasds.Count);
            return rasds.ToArray();
        }
        /// <summary>
        /// Find a RASD in a Virtual System and InstanceId
        /// </summary>
        /// <param name="ovfObj">Envelope_Type</param>
        /// <param name="vsId">string representing the VirtualSystem_Type.id</param>
        /// <param name="instanceId">RASD_Type.InstanceId</param>
        /// <returns>RASD_Type or NULL if not found.</returns>
		public static RASD_Type FindRasdById(EnvelopeType ovfObj, string vsId, string instanceId)
        {
            RASD_Type returnRasd = null;
            VirtualSystem_Type vSystem = FindVirtualSystemById(ovfObj, vsId);

            if (vSystem == null)
            {
                throw new InvalidDataException(string.Format(Messages.OVF_VIRTUAL_SYSTEM_MISSING, vsId));
            }

			VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfObj, vSystem.id);

            foreach (VirtualHardwareSection_Type vhs in vhsArray)
            {
                returnRasd = FindRasdById(vhs, instanceId);
                if (returnRasd != null)
                    break;
            }

            log.Debug("OVF.FindRasdById completed");
            return returnRasd;
        }
        /// <summary>
        /// Find a RASD by it's InstanceId
        /// </summary>
        /// <param name="vhsObj">VirtualHardwareSection_Type</param>
        /// <param name="instanceId">string InstanceId of RASD</param>
        /// <returns>RASD_Type object that matches the InstanceId</returns>
        public static RASD_Type FindRasdById(object vhsObj, string instanceId)
        {
            if (!(vhsObj is VirtualHardwareSection_Type))
            {
                throw new InvalidCastException(Messages.OVF_VHS_MISSING);
            }
            VirtualHardwareSection_Type vhs = (VirtualHardwareSection_Type)vhsObj;
            foreach (RASD_Type _rasd in vhs.Item)
            {
                if (_rasd.InstanceID.Value.Equals(instanceId))
                {
                    log.Debug("OVF.FindRasdById completed, found");
                    return _rasd;
                }
            }
            log.Debug("OVF.FindRasdById completed, not found");
            return null;
        }
        /// <summary>
        /// Find the filename for the given RASD.  
        /// The RASD must Resource Type: 17, 19, 21 and be a hard disk image.
        /// </summary>
        /// <param name="ovfEnv">EnvelopeType</param>
        /// <param name="rasd">RASD_Type</param>
        /// <param name="compressed">GZip or BZip2</param>
        /// <returns>string: filename or NULL</returns>
        public static string FindRasdFileName(EnvelopeType ovfEnv, RASD_Type rasd, out string compressed)
        {
            //string filename = null;
            string diskReference = null;
            //
            // Use in order of priority
            // 1. HostResource should have binding's if exists.
            // 2. InstanceID referenced
            // 3. Connection maybe used by Microsoft (older CIM Spec)
            //
            if (Tools.ValidateProperty("HostResource", rasd))
            {
                diskReference = rasd.HostResource[0].Value;
            }
            else if (Tools.ValidateProperty("InstanceID", rasd))
            {
                diskReference = rasd.InstanceID.Value;
            }
            else if (Tools.ValidateProperty("Connection", rasd))
            {
                diskReference = rasd.Connection[0].Value;
            }


            if (diskReference == null)
            {
                compressed = "None";
                return null;
            }

            DiskSection_Type disksection = null;
            foreach (object section in ovfEnv.Sections)
            {
                if (section is DiskSection_Type)
                {
                    disksection = (DiskSection_Type)section;
                    break;
                }
            }

            if (disksection == null)
            {
                compressed = "None";
                return null;
            }


            File_Type fileReference = null;

            foreach (VirtualDiskDesc_Type vdisk in disksection.Disk)
            {
                if (diskReference.Contains(vdisk.diskId))
                {
                    foreach (File_Type filer in ovfEnv.References.File)
                    {
                        if (filer.id.Contains(vdisk.fileRef))
                        {
                            fileReference = filer;
                        }
                    }
                }
            }

            if (fileReference == null)
            {
                compressed = "None";
                return null;
            }

            if (fileReference.compression != null &&
                (fileReference.compression.ToUpper().Equals("GZIP") ||
                 fileReference.compression.ToUpper().Equals("BZIP2")))
            {
                compressed = fileReference.compression;
            }
            else
            {
                compressed = "None";
            }

            //
            // @DONE: What if the file is compressed? Need to handle.
            //
            // @DONE. here: This will break in case of: http:// and https://
            // and may break in case of file:// if it's on a different server.
            //
            //if (fileReference.href.Contains("/"))
            //{
            //    filename = fileReference.href.Substring(fileReference.href.LastIndexOf('/') + 1);
            //}
            //else
            //{
            //    filename = fileReference.href;
            //}


            // Let the actual import mechanism figure this out for late time decision on a per-file basis.
            return fileReference.href;
        }
        /// <summary>
        /// Find the localize string.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="msg">Msg_Type</param>
        /// <returns>Localized String</returns>
		public static string FindStringsMessage(EnvelopeType ovfObj, Msg_Type msg)
        {
            return FindStringsMessage(ovfObj, Properties.Settings.Default.Language, msg);
        }
        /// <summary>
        /// Find the localize string.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="lang">Language: ie: en-us</param>
        /// <param name="msg">Msg_Type</param>
        /// <returns>Localized String</returns>
		public static string FindStringsMessage(EnvelopeType ovfObj, string lang, Msg_Type msg)
        {
            string message = "";
            if (!string.IsNullOrEmpty(msg.msgid))
            {
                if (lang == null)
                {
                    lang = Properties.Settings.Default.Language;
                }
                message = FindStringsMessage(ovfObj, lang, msg.msgid);
            }
            if ((message == "" || message == "Empty") && !string.IsNullOrEmpty(msg.Value))
            {
                message = msg.Value;
            }
            return message;
        }
        /// <summary>
        /// Find the localize string, using default language
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="msgId">Identifier</param>
        /// <returns>Localized String</returns>
        public string FindStringsMessage(EnvelopeType ovfObj, string msgId)
        {
            return FindStringsMessage(ovfObj, Properties.Settings.Default.Language, msgId);
        }
        /// <summary>
        /// Find the localize string.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="lang">Language: ie: en-us</param>
        /// <param name="msgId">Identifier</param>
        /// <returns>Localized String</returns>
		public static string FindStringsMessage(EnvelopeType ovfObj, string lang, string msgId)
        {
            Strings_Type stringtype = null;
            string message = null;

			if (ovfObj.Strings != null && ovfObj.Strings.Length > 0)
            {
				foreach (Strings_Type strings in ovfObj.Strings)
                {
                    if (strings.lang == lang)
                    {
                        if (string.IsNullOrEmpty(strings.fileRef))
                        {
                            stringtype = strings;
                            break;
                        }
                        else
                        {
                            // External Bundle
                            File_Type filetype = FindFileReference(ovfObj, strings.fileRef);
                            if (!string.IsNullOrEmpty(filetype.href))
                            {
                                stringtype = Tools.Deserialize<Strings_Type>(Tools.LoadFile(filetype.href));
                                break;
                            }
                        }
                    }
                }
                if (stringtype != null)
                {
                    message = FindStringsMessage(stringtype, msgId);
                }
                if (string.IsNullOrEmpty(message))
                {
                    message = string.Format(Messages.OVF_STRINGS_SECTION_MISSING_MSG, msgId, lang);
                }
            }
            return message;
        }

        public static string FindStringsMessage(Strings_Type bundle, string messageId)
        {
            string message = null;
            foreach (Strings_TypeMsg msg in bundle.Msg)
            {

                if (msg.msgid == messageId)
                {
                    message = msg.Value;
                    break;
                }
            }
            return message;
        }
        /// <summary>
        /// Get a list of system ids in OVF Envelope
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <returns>string[] of virtual system ids</returns>
		public static string[] FindSystemIds(EnvelopeType ovfEnv)
        {
            List<string> systemIds = new List<string>();
            if (!(ovfEnv is EnvelopeType))
            {
                throw new InvalidDataException(Messages.OVF_ENVELOPE_IS_INVALID);
            }

            if (ovfEnv.Item is VirtualSystem_Type)
            {
                VirtualSystem_Type vstemp = (VirtualSystem_Type)ovfEnv.Item;
                ovfEnv.Item = new VirtualSystemCollection_Type();
                ((VirtualSystemCollection_Type)ovfEnv.Item).Content = new Content_Type[] { vstemp };
            }

            foreach (VirtualSystem_Type vSystem in ((VirtualSystemCollection_Type)ovfEnv.Item).Content)
            {
                systemIds.Add(vSystem.id);
            }
            log.DebugFormat("OVF.FindSystemIds completed, {0} found", systemIds.Count);
            return systemIds.ToArray();
        }
        /// <summary>
        // Find a name to use of a VM within an envelope that could have come from any hypervisor.
        // TODO: Consider refactoring this method because it is very similar to XenAdmin.Wizards.ImportWizard().
        /// </summary>
        /// <param name="ovfObj"></param>
        /// <param name="sysId"></param>
        /// <returns></returns>
		public static string FindSystemName(EnvelopeType ovfObj, string sysId)
        {
            VirtualSystem_Type vSystem = FindVirtualSystemById(ovfObj, sysId);

            // Use the given name if present and valid.
            // The given name is Envelope.VirtualSystem.Name specified in the OVF Specification 1.1 clause 7.2.
            // XenServer sets the name property.
            // vSphere 4.1 and Virtual Box 4.0.6 do not.
            if ((Tools.ValidateProperty("Name", vSystem)) && !String.IsNullOrEmpty(vSystem.Name[0].Value))
                return vSystem.Name[0].Value;

            // The VM wasn't given a name.
            // Build a list of choices from various properties.
            var choices = new List<string>();

            // VirtualSystem.id is next preference because vSphere and Virtual Box typically set this property to the VM name.
            if (!string.IsNullOrEmpty(vSystem.id))
                choices.Add(vSystem.id);

            // VirtualHardwareSection_Type.VirtualSystemIdentifier is next preference because Virtual Box will also set this property to the VM name.
			VirtualHardwareSection_Type vhs = FindVirtualHardwareSectionByAffinity(ovfObj, vSystem.id, "xen");

            if ((vhs != null) && (Tools.ValidateProperty("VirtualSystemIdentifier", vhs.System)))
                choices.Add(vhs.System.VirtualSystemIdentifier.Value);

            // Operating system description is next preference.
            OperatingSystemSection_Type[] oss = FindSections<OperatingSystemSection_Type>(vSystem.Items);

            if ((oss != null) && (Tools.ValidateProperty("VirtualSystemIdentifier", oss[0].Description.Value)))
                    choices.Add(oss[0].Description.Value);

            // Envelope name is the last preference for XenServer that can could be a path in some cases.
            // vSphere and Virtual Box usually don't set this property.
			choices.Add(Path.GetFileNameWithoutExtension(ovfObj.Name));

            // First choice is one that is not a GUID.
            foreach (var choice in choices)
            {
                if (!String.IsNullOrEmpty(choice) && !IsGUID(choice))
                    return choice;
            }

            // Second choice is the first GUID.
            foreach (var choice in choices)
            {
                if (!String.IsNullOrEmpty(choice))
                    return choice;
            }

            // Last resort is a new GUID.
            return Guid.NewGuid().ToString();
        }


        /// <summary>
        /// Locate VirtualSystem_Type give its ID.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">id string</param>
        /// <returns>VirtualSystem_Type</returns>
		public static VirtualSystem_Type FindVirtualSystemById(EnvelopeType env, string vsId)
        {
            VirtualSystem_Type vs = null;

            if (env.Item is VirtualSystem_Type)
            {
                vs = (VirtualSystem_Type)env.Item;
                if (!vs.id.Equals(vsId))
                {
                    vs = null;
                }
            }
            else if (env.Item is VirtualSystemCollection_Type)
            {
                VirtualSystemCollection_Type vsCollection = (VirtualSystemCollection_Type)env.Item;
                foreach (object content in vsCollection.Content)
                {
                    if (content is VirtualSystem_Type)
                    {
                        VirtualSystem_Type vsTemp = (VirtualSystem_Type)content;
                        if (vsTemp.id.Equals(vsId))
                        {
                            vs = vsTemp;
                            break;
                        }
                    }
                }
            }
            return vs;
        }

		public static File_Type FindFileReference(EnvelopeType ovfObj, string fileId)
        {
            foreach (File_Type file in ovfObj.References.File)
            {
                if (file.id.Contains(fileId))
                {
                    return file;
                }
            }
            return null;
        }

		public static File_Type FindFileReferenceByVDisk(EnvelopeType ovfObj, VirtualDiskDesc_Type disk)
        {
            if (disk == null) return null;
            return FindFileReference(ovfObj, disk.fileRef);
        }

		public static File_Type FindFileReferenceByRASD(EnvelopeType ovfObj, RASD_Type rasd)
        {
            File_Type fileRef = null;
            string hostresource = null;
            if (rasd.HostResource != null && rasd.HostResource.Length > 0)
            {
                foreach (cimString _hostresource in rasd.HostResource)
                {
                    if (!string.IsNullOrEmpty(_hostresource.Value))
                    {
                        hostresource = _hostresource.Value;
                    }
                }
            }

            if (string.IsNullOrEmpty(hostresource))
            {
                hostresource = rasd.InstanceID.Value;
            }

            fileRef = FindFileReferenceByVDisk(ovfObj, FindDiskReference(ovfObj, hostresource));

            return fileRef;
        }

		public static VirtualDiskDesc_Type FindDiskReferenceByFileId(EnvelopeType ovfObj, string fileId)
        {
            foreach (object obj in ovfObj.Sections)
            {
                if (obj is DiskSection_Type)
                {
                    DiskSection_Type ds = (DiskSection_Type)obj;
                    foreach (VirtualDiskDesc_Type vds in ds.Disk)
                    {
                        if (vds.fileRef.Contains(fileId))
                        {
                            return vds;
                        }
                    }
                }
            }
            return null;

        }

		public static VirtualDiskDesc_Type FindDiskReference(EnvelopeType ovfObj, string diskId)
        {
			foreach (object obj in ovfObj.Sections)
            {
                if (obj is DiskSection_Type)
                {
                    DiskSection_Type ds = (DiskSection_Type)obj;
                    foreach (VirtualDiskDesc_Type vds in ds.Disk)
                    {
                        if (diskId.Contains(vds.diskId))
                        {
                            return vds;
                        }
                    }
                }
            }
            return null;
        }

		public static VirtualDiskDesc_Type FindDiskReference(EnvelopeType ovfObj, RASD_Type rasd)
        {
            VirtualDiskDesc_Type vdRef = null;
            if (rasd.HostResource != null &&
                rasd.HostResource.Length > 0 &&
                !string.IsNullOrEmpty(rasd.HostResource[0].Value))
            {
                vdRef = FindDiskReference(ovfObj, rasd.HostResource[0].Value);
            }

            if (vdRef == null &&
                rasd.InstanceID != null &&
                !string.IsNullOrEmpty(rasd.InstanceID.Value))
            {
                vdRef = FindDiskReference(ovfObj, rasd.InstanceID.Value);
            }

            return vdRef;
        }

		public RASD_Type FindDiskRASDByDiskType(EnvelopeType ovfObj, VirtualDiskDesc_Type disk)
        {
            if (disk != null)
            {
                string[] vsIdArray = FindSystemIds(ovfObj);
                List<RASD_Type> rasdArray = new List<RASD_Type>();
                foreach (string vsId in vsIdArray)
                {
                    rasdArray.AddRange(FindDiskRasds(ovfObj, vsId));
                    rasdArray.AddRange(FindRasdByType(ovfObj, vsId, 15));
                    rasdArray.AddRange(FindRasdByType(ovfObj, vsId, 16));
                    foreach (RASD_Type _rasd in rasdArray)
                    {
                        if (_rasd.HostResource != null && _rasd.HostResource.Length > 0)
                        {
                            foreach (cimString hostresource in _rasd.HostResource)
                            {
                                if (!string.IsNullOrEmpty(hostresource.Value))
                                {
                                    if (hostresource.Value.Contains(disk.diskId))
                                    {
                                        return _rasd;
                                    }
                                }
                            }
                        }
                        else if (_rasd.InstanceID != null && !string.IsNullOrEmpty(_rasd.InstanceID.Value))
                        {
                            if (_rasd.InstanceID.Value.Contains(disk.diskId))
                            {
                                return _rasd;
                            }
                        }
                    }
                }
            }
            return null;
        }

		public RASD_Type FindDiskRASDByFileType(EnvelopeType ovfObj, File_Type file)
        {
            return FindDiskRASDByDiskType(ovfObj, FindDiskReferenceByFileId(ovfObj, file.id));
        }
        /// <summary>
        /// Find the disk sizes from an ova.xml (XenServer XVA)
        /// </summary>
        /// <param name="ovaxml">ova.xml string from a XenServer XVA export.</param>
        /// <returns>DiskInfo[] summarizing disks defined in the ova.xml</returns>
        public DiskInfo[] FindSizes(string ovaxml)
        {
            int iteration = 0;
            List<DiskInfo> di = new List<DiskInfo>();
            XenXva xenobj = Tools.DeserializeOvaXml(ovaxml);
            foreach (XenMember xm in xenobj.xenstruct.xenmember)
            {
                if (xm.xenname.ToLower().Equals("objects"))
                {
                    foreach (object obj in ((XenArray)xm.xenvalue).xendata.xenvalue)
                    {
                        if (obj is XenStruct)
                        {
                            bool AtVDI = false;
                            bool AtSR = false;
                            string reference = null;
                            foreach (XenMember xmm in ((XenStruct)obj).xenmember)
                            {
                                #region SET AREA
                                if (xmm.xenname.ToLower().Equals("class") &&
                                    xmm.xenvalue != null &&
                                    xmm.xenvalue is string &&
                                    ((string)xmm.xenvalue).Length > 0)
                                {
                                    if (((string)xmm.xenvalue).ToLower().Equals("vdi"))
                                    {
                                        AtVDI = true;
                                    }
                                    else if (((string)xmm.xenvalue).ToLower().Equals("sr"))
                                    {
                                        AtSR = true;
                                    }
                                    else
                                    {
                                        AtVDI = false;
                                        AtSR = false;
                                    }
                                }
                                #endregion

                                #region CHECK REFERENCE
                                if (xmm.xenname.ToLower().Equals("id") &&
                                   xmm.xenvalue != null &&
                                   xmm.xenvalue is string)
                                {
                                    reference = (string)xmm.xenvalue;
                                }
                                #endregion

                                #region GET DATA
                                if (xmm.xenname.ToLower().Equals("snapshot") &&
                                    xmm.xenvalue != null &&
                                    xmm.xenvalue is XenStruct)
                                {
                                    if (AtVDI)
                                    {
                                        DiskInfo diskDetails = new DiskInfo();
                                        diskDetails.DriveId = reference;
                                        foreach (XenMember xmmm in ((XenStruct)xmm.xenvalue).xenmember)
                                        {
                                            if (xmmm.xenname.ToLower().Equals("virtual_size"))
                                            {
                                                diskDetails.CapacitySize = (string)xmmm.xenvalue;
                                            }
                                            else if (xmmm.xenname.ToLower().Equals("physical_utilisation"))
                                            {
                                                diskDetails.PhysicalSize = (string)xmmm.xenvalue;
                                            }
                                            if (xmmm.xenname.ToLower().Equals("sr"))
                                            {
                                                string key = string.Format(@"{0}.{1}", (string)xmmm.xenvalue, iteration++);
                                                mappings.Add(key, reference);
                                            }
                                        }

                                        log.DebugFormat("OVF.FindSizes: Id {0} Size {1} Type {2}", diskDetails.DriveId, diskDetails.CapacitySize, (diskDetails.DiskType == 0) ? "HDD" : "CD/DVD/Other");
                                        di.Add(diskDetails);
                                        AtVDI = false;
                                    }
                                    if (AtSR)
                                    {
                                        foreach (XenMember xmmm in ((XenStruct)xmm.xenvalue).xenmember)
                                        {
                                            if (xmmm.xenname.ToLower().Equals("content_type"))
                                            {
                                                //if (mappings.ContainsKey(reference))
                                                //{
                                                foreach (DiskInfo details in di)
                                                {
                                                    //if (details.DriveId.ToLower().Equals(mappings[reference].ToLower()))
                                                    foreach (string key in mappings.Keys)
                                                    {
                                                        if (key.ToLower().StartsWith(reference.ToLower()))
                                                        {
                                                            if (details.DriveId.ToLower().Equals(mappings[key].ToLower()))
                                                            {
                                                                if (((string)xmmm.xenvalue).ToLower().Equals("iso"))
                                                                {
                                                                    details.DiskType = 1;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    //}
                                                }
                                            }
                                        }
                                        AtSR = false;
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
            }
            mappings.Clear();
            log.Debug("OVF.FindSizes completed");
            return di.ToArray();
        }
        /// <summary>
        /// Given a list of sections find one section of type T with ID = id.
        /// </summary>
        /// <typeparam name="T">Section Type: Type name to find</typeparam>
        /// <param name="sectObjs">Section_Type[] list of sections</param>
        /// <param name="id">Identifier</param>
        /// <returns>Section of type T</returns>
        public T FindSection<T>(Section_Type[] sectObjs, string id)
        {
            T section = default(T);

            foreach (object sect in sectObjs)
            {
                if (sect is T)
                {
                    if (((Section_Type)sect).Id == id)
                    {
                        section = (T)sect;
                        break;
                    }
                }
            }
            return section;
        }
        /// <summary>
        /// Find all sections of type T
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="ovfEnv">EnvelopeType</param>
        /// <returns>Array of T[]</returns>
        public static T[] FindSections<T>(EnvelopeType ovfEnv)
        {
            if (ovfEnv == null)
            {
                throw new NullReferenceException("OVF.FindSection: OVF Envelope is NULL");
            }
            List<T> sections = new List<T>();

            // Top level Sections
            if (ovfEnv.Sections != null && ovfEnv.Sections.Length > 0)
            {
                foreach (object section in ovfEnv.Sections)
                {
                    if (section is T)
                    {
                        sections.Add(((T)section));
                    }
                }
            }
            // Section in the FIRST Content_Type
            // If it is VirtualSystemCollection_Type  find all below...
            if (ovfEnv.Item is VirtualSystemCollection_Type)
            {
                VirtualSystemCollection_Type coll = (VirtualSystemCollection_Type)ovfEnv.Item;
                if (coll.Items != null && coll.Items.Length > 0)
                {
                    foreach (object section in coll.Items)
                    {
                        if (section is T)
                        {
                            sections.Add(((T)section));
                        }
                    }
                }
            }

            string[] vsIdArray = FindSystemIds(ovfEnv);
            foreach (string vsId in vsIdArray)
            {
                VirtualSystem_Type vs = FindVirtualSystemById(ovfEnv, vsId);
                if (vs != null)
                {
                    if (vs.Items != null && vs.Items.Length > 0)
                    {
                        foreach (object section in vs.Items)
                        {
                            if (section is T)
                            {
                                sections.Add(((T)section));
                            }
                        }
                    }
                }
            }

            return sections.ToArray();
        }
        /// <summary>
        /// Find all sections of type T
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="sectObjs">Array of Section_Type[]</param>
        /// <returns>Array of T[]</returns>
        public static T[] FindSections<T>(Section_Type[] sectObjs)
        {
            List<T> sections = new List<T>();

            foreach (object sect in sectObjs)
            {
                if (sect is T)
                {
                    sections.Add((T)sect);
                }
            }

            return sections.ToArray();
        }
        /// <summary>
        /// Given the Type find all sections in Virtual System that match that type.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <returns>Array of T"/></returns>
		public T[] FindSectionsByType<T>(EnvelopeType ovfObj, string vsId)
        {
            VirtualSystem_Type vSystem = FindVirtualSystemById(ovfObj, vsId);
            List<T> section = new List<T>();

            foreach (object sect in vSystem.Items)
            {
                if (sect is T)
                {
                    section.Add((T)sect);
                }
            }

            if (section == null || section.Count <= 0)
            {
                throw new InvalidDataException(string.Format(Messages.OVF_CANNOT_FIND_SECTION, typeof(T).ToString(), vsId ));
            }
            return section.ToArray();
        }

        public static VirtualHardwareSection_Type[] FindVirtualHardwareSection(EnvelopeType ovfEnv, string vsId)
        {
            List<VirtualHardwareSection_Type> vhs = new List<VirtualHardwareSection_Type>();
            Content_Type[] contentArray = null;
            if (ovfEnv.Item is VirtualSystemCollection_Type)
            {
                contentArray = (((VirtualSystemCollection_Type)ovfEnv.Item)).Content;
            }
            else if (ovfEnv.Item is VirtualSystem_Type)
            {
                contentArray = new Content_Type[] { (VirtualSystem_Type)(ovfEnv.Item) };
            }

            foreach (Content_Type vsType in contentArray)
            {
                if (vsType is VirtualSystem_Type)
                {
                    VirtualSystem_Type vst = (VirtualSystem_Type)vsType;
                    if (vst.id.Equals(vsId))
                    {
                        foreach (object obj in vst.Items)
                        {
                            if (obj is VirtualHardwareSection_Type)
                            {
                                vhs.Add((VirtualHardwareSection_Type)obj);
                            }
                        }
                        break;
                    }
                }
            }

            log.DebugFormat("OVF.FindVirtualHardwareSection {0} found.", vhs.Count);
            return vhs.ToArray();
        }

        public static VirtualHardwareSection_Type FindVirtualHardwareSection(EnvelopeType ovfEnv, string vsId, string vhsId)
        {

            VirtualHardwareSection_Type vhs = null;
            Content_Type[] vsList = (((VirtualSystemCollection_Type)ovfEnv.Item)).Content;
            foreach (Content_Type vsType in (Content_Type[])vsList)
            {
                if (vsType is VirtualSystem_Type)
                {
                    VirtualSystem_Type vst = (VirtualSystem_Type)vsType;
                    if (vst.id.Equals(vsId))
                    {
                        foreach (object obj in vst.Items)
                        {
                            if (obj is VirtualHardwareSection_Type)
                            {
                                VirtualHardwareSection_Type v = (VirtualHardwareSection_Type)obj;
                                if (v.Id.Equals(vhsId))
                                {
                                    return v;
                                }
                            }
                        }
                        break;
                    }
                }
            }

            log.DebugFormat("OVF.FindVirtualHardwareSection completed {0} ", vsId);
            return vhs;
        }

        public static VirtualHardwareSection_Type FindVirtualHardwareSectionByAffinity(EnvelopeType ovfEnv, string vsId, string typeAffinity)
        {
            VirtualHardwareSection_Type vhs = null;
            // @TODO: May have more than one VHS, need to select closest:
            // 1. xen-3.0-*
            // 2. hvm-3.0-*
            // 3. 301
            // 4. vmx-??
            VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfEnv, vsId);

            int priority = 99;
            foreach (VirtualHardwareSection_Type _vhs in vhsArray)
            {
                if (_vhs.System?.VirtualSystemType?.Value == null)
                {
                    vhs = _vhs;
                    log.Info("Import.Process: Found an Unknown Virtual Hardware Section (Rating: 5) [Unknown]. " +
                             "Results may vary depending on hard disk image format.");
                    priority = 5;
                }
                else if (_vhs.System.VirtualSystemType.Value.ToLower().StartsWith(typeAffinity))
                {
                    vhs = _vhs;
                    priority = 0;
                    log.InfoFormat("Import.Process: Found closest affinity Virtual Hardware Section (Rating: 0) [{0}]", _vhs.System.VirtualSystemType.Value);
                    break;
                }
                else if (_vhs.System.VirtualSystemType.Value.ToLower().StartsWith("xen") ||
                         _vhs.System.VirtualSystemType.Value.ToLower().EndsWith("xen:pv"))
                {
                    if (priority > 0)
                    {
                        vhs = _vhs;
                        priority = 1;
                        log.InfoFormat("Import.Process: Found a XEN PV'd Virtual Hardware Section (Rating: 1) [{0}]", _vhs.System.VirtualSystemType.Value);
                    }
                }
                else if (_vhs.System.VirtualSystemType.Value.ToLower().StartsWith("hvm") ||
                         _vhs.System.VirtualSystemType.Value.ToLower().EndsWith("xen:hvm"))
                {
                    if (priority >= 1)
                    {
                        vhs = _vhs;
                        priority = 2;
                        log.InfoFormat("Import.Process: Found a XEN Non-PV'd Virtual Hardware Section (Rating: 2) [{0}]", _vhs.System.VirtualSystemType.Value);
                    }
                }
                else if (_vhs.System.VirtualSystemType.Value.ToLower().StartsWith("301"))
                {
                    if (priority >= 3)
                    {
                        vhs = _vhs;
                        priority = 3;
                    }
                    log.InfoFormat("Import.Process: Found a Microsoft Virtual Hardware Section (Rating: 3) [{0}]", _vhs.System.VirtualSystemType.Value);
                }
                else if (_vhs.System.VirtualSystemType.Value.ToLower().StartsWith("vmx"))
                {
                    if (priority >= 4)
                    {
                        vhs = _vhs;
                        priority = 4;
                        log.InfoFormat("Import.Process: Found a VMWare Virtual Hardware Section (Rating: 4) [{0}]", _vhs.System.VirtualSystemType.Value);
                    }
                }
                else
                {
                    if (priority >= 5)
                    {
                        vhs = _vhs;
                        log.InfoFormat("Import.Process: Found an Unknown Virtual Hardware Section (Rating: 5) [{0}]. " +
                                       "Results may vary depending on hard disk image format.",
                            _vhs.System.VirtualSystemType.Value);
                        priority = 5;
                    }
                }
            }

            if (vhs == null)
            {
                log.Error("Import.Process: No VirtualHardwareSection_Type Exists");
                throw new InvalidDataException(Messages.OVF_VHS_MISSING);
            }
            return vhs;
        }
        #endregion

        #region SETs
        /// <summary>
        /// Set the count for the number of CPUs to assign to the virtual machine.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="cpucount">Number of CPUs</param>
		public static void SetCPUs(EnvelopeType ovfEnv, string vsId, UInt64 cpucount)
        {
            List<RASD_Type> rasds = new List<RASD_Type>();

            VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfEnv, vsId);
            foreach (VirtualHardwareSection_Type vhs in vhsArray)
            {
                if (vhs.Item != null && vhs.Item.Length > 0)
                {
                    foreach (RASD_Type _rasd in vhs.Item)
                    {
                        rasds.Add(_rasd);
                        if (_rasd.ResourceType.Value == 3)
                        {
                            _rasd.VirtualQuantity.Value = cpucount;
                            return;
                        }
                    }
                }

                RASD_Type rasd = new RASD_Type();
                rasd.AllocationUnits = new cimString(_ovfrm.GetString("RASD_3_ALLOCATIONUNITS"));
                rasd.AutomaticAllocation = new cimBoolean();
                rasd.AutomaticAllocation.Value = true;
                rasd.ConsumerVisibility = new ConsumerVisibility();
                rasd.ConsumerVisibility.Value = 0; //From MS.
                rasd.Caption = new Caption(_ovfrm.GetString("RASD_3_CAPTION"));
                rasd.Description = new cimString(_ovfrm.GetString("RASD_3_DESCRIPTION"));
                rasd.ElementName = new cimString(_ovfrm.GetString("RASD_3_ELEMENTNAME"));
                rasd.InstanceID = new cimString(Guid.NewGuid().ToString());
                rasd.Limit = new cimUnsignedLong();
                rasd.Limit.Value = 100000; // From MS;
                rasd.MappingBehavior = new MappingBehavior();
                rasd.MappingBehavior.Value = 0; // From MS.
                rasd.ResourceType = new ResourceType();
                rasd.ResourceType.Value = 3;  // CPU
                rasd.VirtualQuantity = new cimUnsignedLong();
                rasd.VirtualQuantity.Value = cpucount;
                rasd.Weight = new cimUnsignedInt();
                rasd.Weight.Value = 100;  // From MS.
                rasds.Add(rasd);

                vhs.Item = rasds.ToArray();
            }
            log.Debug("OVF.SetCPUs completed");
        }
        /// <summary>
        /// Set Memory Setting Information
        /// </summary>
		/// <param name="ovfEnv">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="memory">Amount of Memory in Bytes</param>
        /// <param name="unit">Unit of Measure, "MB"  (only current valid value)</param>
        public static void SetMemory(EnvelopeType ovfEnv, string vsId, ulong memory, string unit)
        {
           List<RASD_Type> rasds = new List<RASD_Type>();

            VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfEnv, vsId);

            switch (unit.ToLower())
            {
                case "kb": { unit = "byte * 2^10"; break; }
                case "mb": { unit = "byte * 2^20"; break; }
                case "gb": { unit = "byte * 2^30"; break; }
                default: { unit = "byte * 2^20"; break; }
            }

            foreach (VirtualHardwareSection_Type vhs in vhsArray)
            {
                if (vhs.Item != null && vhs.Item.Length > 0)
                {
                    foreach (RASD_Type _rasd in vhs.Item)
                    {
                        rasds.Add(_rasd);
                        // If Memory already exist, do not duplicate, just update.
                        if (_rasd.ResourceType.Value == 4)
                        {
                            _rasd.AllocationUnits.Value = unit;
                            _rasd.VirtualQuantity.Value = memory;
                            return;
                        }
                    }
                }
                RASD_Type rasd = new RASD_Type();
                rasd.AllocationUnits = new cimString(unit);
                rasd.AutomaticAllocation = new cimBoolean();
                rasd.AutomaticAllocation.Value = true;
                rasd.ConsumerVisibility = new ConsumerVisibility();
                rasd.ConsumerVisibility.Value = 2;
                rasd.Caption = new Caption(_ovfrm.GetString("RASD_4_CAPTION"));
                rasd.Description = new cimString(_ovfrm.GetString("RASD_4_DESCRIPTION"));
                rasd.ElementName = new cimString(_ovfrm.GetString("RASD_4_ELEMENTNAME"));
                rasd.InstanceID = new cimString(Guid.NewGuid().ToString());
                rasd.Limit = new cimUnsignedLong();
                rasd.Limit.Value = 128; // From MS;
                rasd.MappingBehavior = new MappingBehavior();
                rasd.MappingBehavior.Value = 0; // From MS.
                rasd.ResourceSubType = new cimString(_ovfrm.GetString("RASD_4_RESOURCESUBTYPE"));
                rasd.ResourceType = new ResourceType();
                rasd.ResourceType.Value = 4;  // Memory
                rasd.VirtualQuantity = new cimUnsignedLong();
                rasd.VirtualQuantity.Value = memory;  // should to round to human readable values. ( but is correct with taskmgr )
                rasd.Weight = new cimUnsignedInt();
                rasd.Weight.Value = 0;  // From MS.
                rasds.Add(rasd);

                vhs.Item = rasds.ToArray();
            }
            log.Debug("OVF.SetMemory completed");
        }
        /// <summary>
        /// Helper to set the target userdevice to connect the resources.
        /// </summary>
        /// <param name="ovfObj">Envelope Type</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="rasdId">RASD Instance Id</param>
        /// <param name="device">Device number</param>
		public static void SetTargetDeviceInRASD(EnvelopeType ovfObj, string vsId, string rasdId, string device)
        {
            short nbr = short.Parse(device);

            if (nbr < 0 || nbr > 15)
            {
                var message = string.Format(Messages.OVF_DEVICE_OUT_OF_RANGE, device);
                log.ErrorFormat("Device number {0} is less than zero or greater than 15.", device);
                throw new ArgumentOutOfRangeException(message);
            }

            RASD_Type rasd = SetConnectionInRASD(ovfObj, vsId, rasdId, Properties.Settings.Default.xenDeviceKey, device);
            if (rasd.AddressOnParent == null || rasd.AddressOnParent.Value == null)
            {
                rasd.AddressOnParent = new cimString();
            }
            rasd.AddressOnParent.Value = device;

            log.DebugFormat("OVF.SetTargetDeviceInRASD completed {0} device {1}", vsId, device);
        }
        /// <summary>
        /// Helper to add an ISO into the Envelope
        /// </summary>
        /// <param name="ovfObj">Envelope Type</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="rasdId">RASD Instance Id</param>
        /// <param name="sruuid">target SR name or uuid</param>
		public static void SetTargetISOSRInRASD(EnvelopeType ovfObj, string vsId, string rasdId, string sruuid)
        {
            SetConnectionInRASD(ovfObj, vsId, rasdId, Properties.Settings.Default.xenSRKey, sruuid);
            log.DebugFormat("OVF.SetTargetISOSRInRASD completed {0}", vsId);
        }
        /// <summary>
        /// Helper to set the destination Storage Repository for the VM drives.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">VirtualSystem Id</param>
        /// <param name="rasdId">RASD Instance Id</param>
        /// <param name="sruuid">SR name or uuid</param>
		public static void SetTargetSRInRASD(EnvelopeType ovfObj, string vsId, string rasdId, string sruuid)
        {
            SetConnectionInRASD(ovfObj, vsId, rasdId, Properties.Settings.Default.xenSRKey, sruuid);
            log.DebugFormat("OVF.SetTargetSRInRASD completed {0}", vsId);
        }

        /// <summary>
        /// Helper to set the destination VDI for the VM drives.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">VirtualSystem Id</param>
        /// <param name="rasdId">RASD Instance Id</param>
        /// <param name="vdiuuid">VDI uuid</param>
        public static void SetTargetVDIInRASD(EnvelopeType ovfObj, string vsId, string rasdId, string vdiuuid)
        {
            SetConnectionInRASD(ovfObj, vsId, rasdId, Properties.Settings.Default.xenVDIKey, vdiuuid);
            log.DebugFormat("OVF.SetTargetVDIInRASD completed {0}", vsId);
        }

        /// <summary>
        /// Helper to set the network identifier.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">VirtualSystem Id</param>
        /// <param name="rasdId">RASD Instance Id</param>
        /// <param name="netuuid">network identifier: uuid, name, bridgename</param>
		public static void SetTargetNetworkInRASD(EnvelopeType ovfObj, string vsId, string rasdId, string netuuid)
        {
            SetConnectionInRASD(ovfObj, vsId, rasdId, Properties.Settings.Default.xenNetworkKey, netuuid);
            log.DebugFormat("OVF.SetTargetNetworkInRASD completed {0}", vsId);
        }
        /// <summary>
        /// Add information to the RASD.Connection[0].Value,  This field is used to define
        /// specifics for XenServer such as:
        /// device=[0..3] or [0..15]
        /// sr=[name] or [uuid]
        /// network=[name], [uuid], or [bridgename]
        /// vdi=[uuid]
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">VirtualSystem Id</param>
        /// <param name="rasdId">RASD Instance Id</param>
        /// <param name="prompt">prompt string: "sr=", "device=", "network=", "vdi="...</param>
        /// <param name="uuid">name or uuid</param>
		public static RASD_Type SetConnectionInRASD(EnvelopeType ovfObj, string vsId, string rasdId, string prompt, string uuid)
        {
            RASD_Type rasd = FindRasdById(ovfObj, vsId, rasdId);

            if (rasd.Connection != null && rasd.Connection.Length > 0 &&
                rasd.Connection[0] != null && !string.IsNullOrEmpty(rasd.Connection[0].Value))
            {
                StringBuilder sb = new StringBuilder();
                if (rasd.Connection[0].Value.ToLower().Contains(prompt))
                {
                    string[] pairs = rasd.Connection[0].Value.Split(new char[] { ',' });
                    foreach (string pair in pairs)
                    {
                        if (!pair.Trim().ToLower().StartsWith(prompt))
                        {
                            sb.AppendFormat("{0},", pair);
                        }
                    }
                    sb.AppendFormat("{0}{1}", prompt, uuid);
                }
                else
                {
                    sb.AppendFormat("{0},{1}{2}", rasd.Connection[0].Value, prompt, uuid);
                }
                rasd.Connection[0].Value = sb.ToString();

            }
            else
            {
                rasd.Connection = new cimString[] { new cimString(string.Format("{0}{1}", prompt, uuid)) };
            }
            log.DebugFormat("OVF.SetConnectionInRASD completed {0}", vsId);
            return rasd;
        }

        /// <returns>string of the InstanceID of the CDROM RASD</returns>
        public static string SetRunOnceBootCDROMOSFixup(EnvelopeType ovfObj, string vsId, string ovfPath)
        {
            return SetRunOnceBootCDROM(ovfObj, vsId, ovfPath, Properties.Settings.Default.xenLinuxFixUpDisk);
        }
        /// <summary>
        /// Add an ISO as a run-once device
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="ovfPath">Path to ovf</param>
        /// <param name="isofilename">fullpath/filename of iso to attach</param>
        /// <returns>string of the InstanceID of the CDROM RASD</returns>
		public static string SetRunOnceBootCDROM(EnvelopeType ovfObj, string vsId, string ovfPath, string isofilename)
        {
            return SetRunOnceBootCDROM(ovfObj, vsId, Properties.Settings.Default.Language, ovfPath, isofilename);
        }
        /// <summary>
        /// Add an ISO as a run-once device
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
        /// <param name="vsId">Virtual System Id</param>
        /// <param name="lang">Language ie: en-us</param>
        /// <param name="ovfPath">Path to ovf</param>
        /// <param name="isofilename">fullpath/filename of iso to attach</param>
        /// <returns>string of the InstanceID of the CDROM RASD</returns>
		public static string SetRunOnceBootCDROM(EnvelopeType ovfObj, string vsId, string lang, string ovfPath, string isofilename)
        {
            //
            // @TODO Need to check if Fixup CD is already attached and just need to add to vsid.
            //
            List<RASD_Type> cdroms = new List<RASD_Type>();
            cdroms.AddRange(FindRasdByType(ovfObj, vsId, 16));  // DVD
            cdroms.AddRange(FindRasdByType(ovfObj, vsId, 15));  // CDROM

            string cdId = null;
            if (cdroms.Count <= 0)
            {
                cdId = Guid.NewGuid().ToString();
                AddCDROM(ovfObj, vsId, lang, cdId, _ovfrm.GetString("RASD_16_CAPTION"), _ovfrm.GetString("RASD_16_DESCRIPTION"));
                cdroms.AddRange(FindRasdByType(ovfObj, vsId, 16));
            }
            else
            {
                cdId = cdroms[0].InstanceID.Value;
            }
            //
            // Change Boot Order to: D
            //
            // Check to see if "VirtualSystemOtherConfigurationData is present if NOT generate for HVM.
            //
            VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfObj, vsId);
            foreach (VirtualHardwareSection_Type vhs in vhsArray)
            {
                if (vhs.System != null &&
                    vhs.System.VirtualSystemType != null &&
                    !string.IsNullOrEmpty(vhs.System.VirtualSystemType.Value))
                {
                    if (vhs.System.VirtualSystemType.Value.ToLower().StartsWith("xen"))
                    {
                        string ident = null;
                        if (vhs.System.ElementName != null && !string.IsNullOrEmpty(vhs.System.ElementName.Value))
                        {
                            ident = vhs.System.ElementName.Value;
                        }
                        else if (vhs.System.Caption != null && !string.IsNullOrEmpty(vhs.System.Caption.Value))
                        {
                            ident = vhs.System.Caption.Value;
                        }
                        else
                        {
                            ident = vhs.System.InstanceID.Value;
                        }
                        log.InfoFormat("Booting a Paravirtualized disk to CDROM is not valid, skipping: {0}", ident);
                        continue;
                    }
                }
                if (vhs.VirtualSystemOtherConfigurationData == null)
                {
                    AddOtherSystemSettingData(ovfObj, vsId, "HVM_boot_policy", Properties.Settings.Default.xenBootOptions, _ovfrm.GetString("OTHER_SYSTEM_SETTING_DESCRIPTION_2"));
                    AddOtherSystemSettingData(ovfObj, vsId, "HVM_boot_params", "dnc", _ovfrm.GetString("OTHER_SYSTEM_SETTING_DESCRIPTION_6"));
                    AddOtherSystemSettingData(ovfObj, vsId, "platform", Properties.Settings.Default.xenPlatformSetting, _ovfrm.GetString("OTHER_SYSTEM_SETTING_DESCRIPTION_3"));
                }
                else
                {
                    List<Xen_ConfigurationSettingData_Type> newCSD = new List<Xen_ConfigurationSettingData_Type>();
                    foreach (Xen_ConfigurationSettingData_Type csd in vhs.VirtualSystemOtherConfigurationData)
                    {
                        if (csd.Name.ToLower().Equals("hvm_boot_params"))
                        {
                            csd.Value.Value = "d";
                        }
                    }
                }
                // Add the fixup ISO file.
                UpdateField(cdroms[0], "HostResource", string.Format("ovf:/disk/{0}", cdId));
                AddFileReference(ovfObj, lang, isofilename, cdId, 0, Properties.Settings.Default.isoFileFormatURI);
                string destFile = Path.Combine(ovfPath, Path.GetFileName(isofilename));
                string srcFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), isofilename);

                // Indicate the need to configure a VM for XenServer during import with just the presence of a post install operation that is specific to XenServer.
                // This is an overly complicated flag but simplification would require much more refactoring at the time of this change.
                AddPostInstallOperation(ovfObj, vsId, lang, "ConfigureForXenServer");

                string xml = Tools.Serialize(ovfObj, typeof(EnvelopeType));

                if (File.Exists(srcFile))
                {
                    if (!File.Exists(destFile))
                    {
                        File.Copy(srcFile, destFile);
                    }
                }
                else
                {
                    log.WarnFormat("SetRunOnceBootCDROM: Missing ISO: {0} find and copy to: {1}", srcFile, destFile);
                }
            }
            return cdId;
        }
        #endregion

        #region MISC

        /// <summary>
        /// Convert OVF EnvelopeType to XML string
        /// </summary>
        /// <param name="ovfEnv">EnvelopeType object</param>
        /// <returns>string (xml)</returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static string ToXml(EnvelopeType ovfEnv)
        {
            if (ovfEnv == null)
                throw new NullReferenceException(Messages.OVF_ENVELOPE_IS_INVALID);

			return Serialize(ovfEnv);
        }

        /// <summary>
        /// Transform the OVF Object (EnvelopeType) to XML.
        /// </summary>
        /// <param name="ovf">OVF Object (EnvelopeType)</param>
        /// <returns>XML String</returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static string Serialize(object ovf)
        {
            if (ovf != null)
            {
                if (ovf is EnvelopeType)
                {
                    return Tools.Serialize(ovf, typeof(EnvelopeType), Tools.LoadNamespaces());
                }
                else
                {
                    throw new ArgumentException(Messages.OVF_ENVELOPE_IS_INVALID);
                }
            }
            else
            {
                throw new ArgumentNullException(Messages.OVF_ENVELOPE_IS_INVALID);
            }
        }

        public static string ExtractArchive(string pathToOva)
        {
            Stream inputStream = null;
            FileStream fsStream = null;
            ArchiveIterator tar = null;

            var dir = Path.GetDirectoryName(pathToOva);

            try
            {
                string ext = Path.GetExtension(pathToOva);

                if (ext.ToLower().EndsWith("gz") || ext.ToLower().EndsWith("bz2")) // need to decompress.
                {
                    log.Info("OVA is compressed, de-compression stream inserted");

                    var ovafilename = string.Format(@"{0}", Path.GetFileNameWithoutExtension(pathToOva));
                    string ovaext = Path.GetExtension(ovafilename);

                    if (ovaext.ToLower().EndsWith("ova"))
                    {
                        fsStream = new FileStream(Path.Combine(dir, ovafilename), FileMode.Open, FileAccess.Read, FileShare.Read);

                        var compType = Properties.Settings.Default.useGZip ? CompressionFactory.Type.Gz : CompressionFactory.Type.Bz2;
                        inputStream = CompressionFactory.Reader(compType, fsStream);
                    }
                    else
                    {
                        throw new ArgumentException(Messages.OVF_COMPRESSED_OVA_INVALID);
                    }
                }
                else
                {
                    inputStream = new FileStream(pathToOva, FileMode.Open, FileAccess.Read, FileShare.Read);
                }

                var temp = Path.Combine(dir, Path.GetRandomFileName());
                tar = ArchiveFactory.Reader(ArchiveFactory.Type.Tar, inputStream);
                tar.ExtractAllContents(temp);
                return temp;
            }
            finally
            {
                tar?.Dispose();
                inputStream?.Close();
                fsStream?.Close();
            }
        }

        /// <summary>
        /// Extract the OVF xml meta data.
        /// </summary>
        /// <param name="pathtoova"></param>
        /// <param name="ovafilename"></param>
        /// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidDataException"></exception>
		public static string ExtractOVFXml(string ovapath)
        {
        	FileStream fsStream = null;
        	Stream inputStream = null;
			try
			{
				#region DECOMPRESSION STREAM

				if (ovapath.ToLower().EndsWith("gz") || ovapath.ToLower().EndsWith("bz2")) // need to decompress.
				{
					log.Info("OVA is compressed, de-compression stream inserted");
					string ovaext = Path.GetExtension(Path.GetFileNameWithoutExtension(ovapath));

					if (ovaext.ToLower().EndsWith("ova"))
					{
						fsStream = new FileStream(ovapath, FileMode.Open, FileAccess.Read, FileShare.None);
						if (Properties.Settings.Default.useGZip)
                            inputStream = CompressionFactory.Reader(CompressionFactory.Type.Gz, fsStream);
						else
                            inputStream = CompressionFactory.Reader(CompressionFactory.Type.Bz2, fsStream);
					}
					else
					{
                        throw new ArgumentException(Messages.OVF_COMPRESSED_OVA_INVALID);
                    }
				}
				else
				{
					inputStream = new FileStream(ovapath, FileMode.Open, FileAccess.Read, FileShare.None);
				}

				#endregion

				using (ArchiveIterator tis = ArchiveFactory.Reader(ArchiveFactory.Type.Tar, inputStream))
				{
					var ovfxml = new StringBuilder();

					while (tis.HasNext())
					{
						if (tis.CurrentFileSize() == 0)
							throw new InvalidDataException(Messages.INVALID_DATA_IN_OVA);

						if (tis.CurrentFileName().EndsWith(Properties.Settings.Default.ovfFileExtension))
						{
                            using( MemoryStream ms = new MemoryStream() )
                            {
                                tis.ExtractCurrentFile( ms );
                                ovfxml.Append( Encoding.UTF8.GetString(ms.ToArray()));
                                return ovfxml.ToString();
                            }
						}
					}
				}
			}
			finally
			{
				if (inputStream != null)
					inputStream.Close();
				if (fsStream != null)
					fsStream.Close();
			}

            return null;
        }

        public Stream ExtractFileFromOva(string ovafilename, string extractfile)
        {
            MemoryStream ms = new MemoryStream();
            using (Stream inputStream = File.OpenRead(ovafilename))
            {
                using( ArchiveIterator it = ArchiveFactory.Reader(ArchiveFactory.Type.Tar, inputStream))
                {
                    while( it.HasNext() )
                    {
                        if (it.CurrentFileName().ToLower().EndsWith(extractfile.ToLower()))
                            it.ExtractCurrentFile( ms );
                    }
                }
            }
            ms.Position = 0;
            return ms;
        }
        /// <summary>
        /// Finalize the OVF to provide the 'roll up' information in the System Setting Data.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType</param>
		public static void FinalizeEnvelope(EnvelopeType ovfEnv)
        {
            // FIND ALL SYSTEMS IN HERE... loop through them.

            List<VirtualSystem_Type> systems = new List<VirtualSystem_Type>();

            if (ovfEnv.Item is VirtualSystemCollection_Type)
            {
                VirtualSystemCollection_Type vsc = (VirtualSystemCollection_Type)ovfEnv.Item;
                if (vsc.Content != null && vsc.Content.Length > 0)
                {
                    Content_Type[] contentArray = ((VirtualSystemCollection_Type)ovfEnv.Item).Content;
                    foreach (Content_Type content in contentArray)
                    {
                        if (content is VirtualSystem_Type)
                        {
                            systems.Add((VirtualSystem_Type)content);
                        }
                    }
                }
            }
            else if (ovfEnv.Item is VirtualSystem_Type)
            {
                systems.Add((VirtualSystem_Type)ovfEnv.Item);
            }

            if (systems.Count <= 0)
            {
                log.Error("Finalize Envelope FAILED, no Virtual Systems Defined.");
                return;
            }

            foreach (VirtualSystem_Type system in systems)
            {
                VirtualHardwareSection_Type[] vha = FindVirtualHardwareSection(ovfEnv, system.id);
                foreach (VirtualHardwareSection_Type vhs in vha)
                {
                    ulong cpuCount = 0;
                    ulong memCount = 0;
                    ulong ethCount = 0;
                    ulong dskCount = 0;

                    #region ROLL UP COUNTS
                    foreach (RASD_Type rasd in vhs.Item)
                    {
                        switch (rasd.ResourceType.Value)
                        {
                            case 3: //CPU
                                {
                                    cpuCount += rasd.VirtualQuantity.Value;
                                    break;
                                }
                            case 4: //Memory
                                {
                                    memCount += rasd.VirtualQuantity.Value;
                                    break;
                                }
                            case 10: // Ethernet
                                {
                                    ethCount++;
                                    break;
                                }
                            case 17: // Disks
                            case 19:
                            case 20:
                            case 21:
                                {
                                    bool continueWithDisk = false;
                                    if (rasd.Caption != null && rasd.Caption.Value != null)
                                    {
                                        continueWithDisk = IsDiskRasd(rasd);
                                    }
                                    else if (rasd.AllocationUnits != null &&
                                        rasd.AllocationUnits.Value != null &&
                                        rasd.AllocationUnits.Value.Contains(_ovfrm.GetString("RASD_19_ALLOCATIONUNITS")))
                                    {
                                        continueWithDisk = true;
                                    }
                                    if (continueWithDisk)
                                    {
                                        dskCount++;
                                    }
                                    break;
                                }
                        }

                    }
                    #endregion

                    vhs.Info.Value = string.Format(_ovfrm.GetString("VIRTUAL_HARDWARE_SECTION_INFO"), memCount, cpuCount, dskCount, ethCount);
                }
            }

            if (systems.Count == 1)
            {
                ovfEnv.Item = (Content_Type)systems[0];
            }
            log.Debug("OVF.FinalizeEnvelope completed.");
        }

        public static EnvelopeType Merge(List<EnvelopeType> ovfcollection, string ovfname)
        {
            EnvelopeType finalEnv = CreateEnvelope(ovfname);
			finalEnv.version = Properties.Settings.Default.ovfversion;
			finalEnv.Item = new VirtualSystemCollection_Type();
			finalEnv.Item.id = Guid.NewGuid().ToString();

            List<Section_Type> sections = new List<Section_Type>();
            List<File_Type> filetypes = new List<File_Type>();
            List<VirtualDiskDesc_Type> disks = new List<VirtualDiskDesc_Type>();
            List<VirtualSystem_Type> vsystem = new List<VirtualSystem_Type>();
            Dictionary<string, NetworkSection_TypeNetwork> networks = new Dictionary<string, NetworkSection_TypeNetwork>();
        	var startupOptions = new List<StartupSection_TypeItem>();

            #region COLLECT SECTIONS
            foreach (EnvelopeType curEnvelope in ovfcollection)
            {
                if (curEnvelope.References != null && curEnvelope.References.File != null)
                {
                    foreach (File_Type file in curEnvelope.References.File)
                    {
                        filetypes.Add(file);
                    }
                }
                if (curEnvelope.Sections != null)
                {
                    foreach (Section_Type section in curEnvelope.Sections)
                    {
                        if (section is DiskSection_Type)
                        {
                            DiskSection_Type ds = (DiskSection_Type)section;
                            if (ds.Disk != null)
                            {
                                foreach (VirtualDiskDesc_Type vd in ds.Disk)
                                {
                                    disks.Add(vd);
                                }
                            }
                        }
                        else if (section is NetworkSection_Type)
                        {
                            NetworkSection_Type ns = (NetworkSection_Type)section;
                            if (ns.Network != null)
                            {
                                foreach (NetworkSection_TypeNetwork net in ns.Network)
                                {
                                    if (!networks.ContainsKey(net.name))
                                    {
                                        networks.Add(net.name, net);
                                    }
                                }
                            }
                        }
						else if (section is StartupSection_Type)
						{
							StartupSection_Type startup = section as StartupSection_Type;
							if (startup.Item != null)
								startupOptions.AddRange(startup.Item);
						}
                    }
                }

                if (curEnvelope.Item != null)
                {
                    if (curEnvelope.Item is VirtualSystem_Type)
                    {
                        VirtualSystem_Type vs = (VirtualSystem_Type)curEnvelope.Item;
                        vsystem.Add(vs);
                    }
                    else if (curEnvelope.Item is VirtualSystemCollection_Type)
                    {
                        VirtualSystemCollection_Type vcol = (VirtualSystemCollection_Type)curEnvelope.Item;
                        vcol.id = Guid.NewGuid().ToString();
                        foreach (Content_Type entity in vcol.Content)
                        {
                            if (entity is VirtualSystem_Type)
                            {
                                VirtualSystem_Type vs = (VirtualSystem_Type)entity;
                                vsystem.Add(vs);
                            }
                        }
                    }
                }
            }
            #endregion

            if (disks.Count > 0)
            {
                DiskSection_Type disksection = new DiskSection_Type();
                string infons = _ovfrm.GetString("SECTION_DISK_INFO");
				disksection.Info = new Msg_Type(AddToStringSection(finalEnv, infons), infons);
                disksection.Disk = disks.ToArray();
                sections.Add(disksection);
            }

            if (networks.Count > 0)
            {
                NetworkSection_Type netsection = new NetworkSection_Type();
                string infons = _ovfrm.GetString("SECTION_NETWORK_INFO");
				netsection.Info = new Msg_Type(AddToStringSection(finalEnv, infons), infons);
                List<NetworkSection_TypeNetwork> nsList = new List<NetworkSection_TypeNetwork>();
                foreach (string key in networks.Keys)
                {
                    nsList.Add(networks[key]);
                }
                netsection.Network = nsList.ToArray();
                sections.Add(netsection);
            }

			if (startupOptions.Count > 0)
			{
				string info = _ovfrm.GetString("SECTION_STARTUP_INFO");
				var startupSection = new StartupSection_Type
				                     	{
											Info = new Msg_Type(AddToStringSection(finalEnv, info), info),
				                     		Item = startupOptions.ToArray()
				                     	};
				sections.Add(startupSection);
			}

			finalEnv.Name = ovfname;
			finalEnv.id = Guid.NewGuid().ToString();
			finalEnv.References = new References_Type();
			finalEnv.References.File = filetypes.ToArray();
			finalEnv.Sections = sections.ToArray();
			((VirtualSystemCollection_Type)finalEnv.Item).Content = vsystem.ToArray();
			FinalizeEnvelope(finalEnv);
			return finalEnv;
        }
        /// <summary>
        /// Take an OVF and make 2+ OVFs
        /// </summary>
        /// <param name="Envelope">OVF Envelope to split out.</param>
        /// <param name="ovfname">Name of the OVF Package</param>
        /// <param name="vsIdarrays">An array of (array of virtual system Id)s</param>
        /// <returns>Array of Envelope in order of vsId arrays.</returns>
        public static EnvelopeType[] Split(EnvelopeType Envelope, string ovfname, object[] vsIdarrays)
        {
            // EACH Array here is an Envelope.
            List<EnvelopeType> OVFArray = new List<EnvelopeType>();

            Section_Type[] vscSections = null;
            if (Envelope.Item is VirtualSystemCollection_Type)
            {
                vscSections = ((VirtualSystemCollection_Type)Envelope.Item).Items;
            }

            foreach (string[] vsIdArray in vsIdarrays)
            {
                EnvelopeType _env = CreateEnvelope(ovfname);
                _env.References = new References_Type();
                List<VirtualDiskDesc_Type> vdisks = new List<VirtualDiskDesc_Type>();
                List<File_Type> files = new List<File_Type>();

                foreach (Section_Type section in Envelope.Sections)
                {
                    if (!(section is DiskSection_Type))
                    {
                        _env.Sections = AddSection(_env.Sections, section);
                    }
                }

                foreach (string vsId in vsIdArray)
                {
                    VirtualSystem_Type vs = FindVirtualSystemById(Envelope, vsId);
                    OVF.AddVirtualSystem(_env, vs);

                    RASD_Type[] rasds = FindDiskRasds(Envelope, vsId);
                    foreach (RASD_Type rasd in rasds)
                    {
                        File_Type file = FindFileReferenceByRASD(Envelope, rasd);
                        if (file != null)
                        {
                            VirtualDiskDesc_Type vdisk = FindDiskReferenceByFileId(Envelope, file.id);
                            files.Add(file);
                            vdisks.Add(vdisk);
                        }
                    }
                    List<RASD_Type> cdroms = new List<RASD_Type>();
                    VirtualHardwareSection_Type vhs = FindVirtualHardwareSectionByAffinity(Envelope, vsId, "xen");
                    cdroms.AddRange(FindRasdByType(vhs, 15));
                    cdroms.AddRange(FindRasdByType(vhs, 16));

                    foreach (RASD_Type cd in cdroms)
                    {
                        File_Type file = FindFileReferenceByRASD(Envelope, cd);
                        if (file != null)
                        {
                            VirtualDiskDesc_Type vdisk = FindDiskReferenceByFileId(Envelope, file.id);
                            if (vdisk != null)
                                vdisks.Add(vdisk);
                            files.Add(file);
                        }
                    }
                }

                if (vscSections != null)
                {
                    if (_env.Item != null)
                    {
                        foreach (Section_Type vsc in vscSections)
                        {
                            _env.Item.Items = AddSection(_env.Item.Items, vsc);
                        }
                    }
                }

                _env.References.File = files.ToArray();
                DiskSection_Type ds = new DiskSection_Type();
                string infods = _ovfrm.GetString("SECTION_DISK_INFO");
                ds.Info = new Msg_Type(AddToStringSection(_env, infods), infods);
                ds.Disk = vdisks.ToArray();
                _env.Sections = AddSection(_env.Sections, ds);

                FinalizeEnvelope(_env);
                OVFArray.Add(_env);
            }

            return OVFArray.ToArray();
        }

        public static bool IsDiskRasd(RASD_Type rasd)
        {
            bool isdiskrasd = false;
            string[] rejectArray = _ovfrm.GetString("IS_DISK_RASD_FILTER").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (rasd.ResourceType.Value == 17 ||
                rasd.ResourceType.Value == 19 ||
                rasd.ResourceType.Value == 20 ||
                rasd.ResourceType.Value == 21)
            {
                if (rasd.Caption != null && !(string.IsNullOrEmpty(rasd.Caption.Value)))
                {
                    isdiskrasd = true;
                    foreach (string reject in rejectArray)
                    {
                        if (rasd.Caption.Value.ToUpper().Contains(reject.ToUpper()))
                        {
                            isdiskrasd = false;
                        }
                    }
                }
                else
                {
                    isdiskrasd = true;
                }
            }
            return isdiskrasd;
        }

        public static string GetContentMessage(string key)
        {
            return _ovfrm.GetString(key);
        }

        public static string GetISOFixupFileName()
        {
            return Path.GetFileName(Properties.Settings.Default.xenLinuxFixUpDisk);
        }

        public static string GetISOFixupPath()
        {
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(assemblyDir, Properties.Settings.Default.xenLinuxFixUpDisk);
        }
        /// <summary>
        /// convert to a UInt64 a allocation unit
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="allocationunits">Form: "byte * #^##" ie: "byte * 2^20"</param>
        /// <returns></returns>
        public static UInt64 ComputeCapacity(long capacity, string allocationunits)
        {
            double multiplier = 1.0;

            string[] allocunits = null;
            if (allocationunits != null)
            {
                allocunits = allocationunits.Split(new char[] { '*', '^' }, StringSplitOptions.RemoveEmptyEntries);
                if (allocunits.Length == 3)
                {
                    multiplier = Math.Pow(Convert.ToDouble(allocunits[1]), Convert.ToDouble(allocunits[2]));
                }
            }
            return Convert.ToUInt64(capacity * multiplier);
        }
        #endregion
        #endregion

        #region PRIVATE
        private static bool IsGUID(string expression)
        {
            if (expression != null)
            {
                Regex guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");

                return guidRegEx.IsMatch(expression);
            }
            return false;
        }
        private bool IsDeviceBootable(EnvelopeType ovfEnv, string vsId, RASD_Type rasd)
        {
            // This is a best guess algorithm. without opening the VHD itself, there is no guaranteed method
            // to delineate this, so we guess.
            // IF it's created by Kensho/XenConvert there will be a chance of having a clue.
            // Otherwise it'll be based upon 'order' and device 0 will win the bootable device.
            bool isBootable = true;
            VirtualDiskDesc_Type[] disks = null;

            foreach (Section_Type sect in ovfEnv.Sections)
            {
                if (sect is DiskSection_Type)
                {
                    disks = ((DiskSection_Type)sect).Disk;
                }
            }

            if (disks == null)
                return false;

            bool useHostResource = false;
            if (Tools.ValidateProperty("HostResource", rasd))
            {
                log.Debug("Using HostResource to find Disk");
                useHostResource = true;
            }
            else
            {
                log.Debug("Using InstanceID to find Disk");
            }

            foreach (VirtualDiskDesc_Type disk in disks)
            {
                if (useHostResource)
                {
                    if (rasd.HostResource[0].Value.Contains(disk.diskId))
                    {
                        isBootable = disk.isBootable;
                    }
                }
                else
                {
                    if (rasd.InstanceID.Value.Contains(disk.diskId))
                    {
                        isBootable = disk.isBootable;
                    }
                }
            }

            if (!isBootable) // Second chance.
            {
                if (Tools.ValidateProperty("Address", rasd))
                {
                    if ((rasd.ResourceType.Value == 21 ||
                        rasd.ResourceType.Value == 5) &&
                        rasd.Address.Value == "0")
                    {
                        isBootable = true;
                    }
                }

                if (Tools.ValidateProperty("AddressOnParent", rasd))
                {
                    if ((rasd.ResourceType.Value == 17 ||
                        rasd.ResourceType.Value == 19) &&
                        rasd.AddressOnParent.Value == "0")
                    {
                        isBootable = true;
                    }
                }

                if (Tools.ValidateProperty("Connection", rasd))
                {
                    if (rasd.Connection[0].Value.Contains("device=0"))
                    {
                        isBootable = true;
                    }
                }
                //
                // Absolute last change. this should find a disk 0
                // on IDE controller 0.
                //
                RASD_Type parent = FindParentDevice(ovfEnv, vsId, rasd);
                if (Tools.ValidateProperty("Address", parent))
                {
                    int address = 0;
                    address = Convert.ToInt32(parent.Address.Value);
                    if (parent.ResourceType.Value != 5)
                    {
                        RASD_Type controller = FindParentDevice(ovfEnv, vsId, parent);
                        if (Tools.ValidateProperty("Address", controller))
                        {
                            address += Convert.ToInt32(controller.Address.Value);
                        }
                    }
                    if (address == 0)
                    {
                        isBootable = true;
                    }
                }
            }

            return isBootable;
        }
        private static void AddContent(VirtualSystemCollection_Type systemColl, string vsId, object item)
        {
            AddContent(systemColl, vsId, Properties.Settings.Default.Language, item);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
                                                        Justification = "Logging mechanism")]
        private static void AddContent(VirtualSystemCollection_Type systemColl, string vsId, string lang, object item)
        {
            List<Section_Type> sections = new List<Section_Type>();
            foreach (Content_Type content in systemColl.Content)
            {
                if (content is VirtualSystem_Type)
                {
                    VirtualSystem_Type vsType = (VirtualSystem_Type)content;
                    if (vsType.id.Equals(vsId))
                    {
                        if (vsType.Items != null)
                        {
                            sections.AddRange(vsType.Items);
                        }
                        sections.Add((Section_Type)item);
                        vsType.Items = sections.ToArray();
                        break;
                    }
                }
            }
            log.DebugFormat("OVF.AddContent completed {0}", vsId);
        }
        private static void AddRasdToAllVHS(EnvelopeType ovfEnv, string vsId, RASD_Type rasd)
        {
            List<RASD_Type> rasds = new List<RASD_Type>();
            VirtualHardwareSection_Type[] vhsArray = FindVirtualHardwareSection(ovfEnv, vsId);
            rasds.Add(rasd);

            foreach (VirtualHardwareSection_Type vhs in vhsArray)
            {
                if (vhs.Item != null && vhs.Item.Length > 0)
                {
                    foreach (RASD_Type _rasd in vhs.Item)
                    {
                        if (!_rasd.InstanceID.Value.Equals(rasd.InstanceID.Value))
                        {
                            rasds.Add(_rasd);
                        }
                    }
                }
                vhs.Item = rasds.ToArray();
            }
        }
        private List<ManagementObject> FindDeviceReferences(string classname, string deviceId)
        {
            List<ManagementObject> References = new List<ManagementObject>();
            string query = string.Format("select * from {0}", classname);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject mgtobj in searcher.Get())
            {
                #region FIND BY PROPERTIES NOT EXPLICID
                string antecedent = null;
                foreach (PropertyData pd in mgtobj.Properties)
                {
                    if (pd.Name.ToLower().Equals("antecedent") && pd.Value != null)
                    {
                        antecedent = (string)pd.Value;
                    }
                }
                if (antecedent == null)
                {
                    log.Debug("PCI Association not available, continuing.");
                    continue;
                }
                #endregion
                if (antecedent.Replace(@"\", "").Contains(deviceId.Replace(@"\", "")))
                {
                    References.Add(mgtobj);
                }
            }
            log.DebugFormat("OVF.FindDeviceReferences completed {0} {1}", classname, deviceId);
            return References;
        }
        private bool IsBootDisk(string deviceID)
        {
            bool bootable = false;
            try
            {
                foreach (ManagementObject mo in Win32_DiskDriveToDiskPartition)
                {
                    #region FIND BY PROPERTIES NOT EXPLICID
                    string _antecedent = null;
                    string _dependent = null;
                    foreach (PropertyData pd in mo.Properties)
                    {
                        if (pd.Name.ToLower().Equals("antecedent") && pd.Value != null)
                        {
                            _antecedent = (string)pd.Value;
                        }
                        if (pd.Name.ToLower().Equals("dependent") && pd.Value != null)
                        {
                            _dependent = (string)pd.Value;
                        }
                    }
                    if (_antecedent == null || _dependent == null)
                    {
                        log.Debug("Win32_DiskDriveToDiskPartition Association not available, continuing.");
                        continue;
                    }
                    #endregion
                    string antecedent = _antecedent.Replace("\\\\", "\\");
                    if (antecedent.Contains(deviceID))
                    {
                        foreach (ManagementObject md in Win32_DiskPartition)
                        {
                            #region FIND BY PROPERTIES NOT EXPLICID
                            string _deviceid = null;
                            bool _bootable = false;
                            foreach (PropertyData pd in md.Properties)
                            {
                                if (pd.Name.ToLower().Equals("deviceid") && pd.Value != null)
                                {
                                    _deviceid = (string)pd.Value;
                                }
                                if (pd.Name.ToLower().Equals("bootable") && pd.Value != null)
                                {
                                    _bootable = (bool)pd.Value;
                                }
                            }
                            if (_deviceid == null)
                            {
                                log.Debug("Win32_DiskPartition DeviceID not available, continuing.");
                                continue;
                            }
                            #endregion
                            if (_dependent.Contains(_deviceid))
                            {
                                return _bootable;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("OVF.IsBootDisk failed, could not determine if bootable", ex);
            }
            log.DebugFormat("OVF.IsBootDisk completed Bootable = {0}", bootable);
            return bootable;
        }
        private bool FillEmptyRequiredFields(RASD_Type rasd)
        {
            bool success = true;
            if (rasd.ElementName == null)
            {
                rasd.ElementName = new cimString(Guid.NewGuid().ToString());
            }
            if (rasd.InstanceID == null)
            {
                rasd.InstanceID = new cimString(Guid.NewGuid().ToString());
            }
            if (rasd.ResourceType == null)
            {
                success = false;
            }
            return success;
        }
        private static int compareFileSizes(File_Type leftFile, File_Type rightFile)
        {
            string leftfilename = Path.GetFileName(leftFile.href.Substring(leftFile.href.LastIndexOf('/') + 1));
            string rightfilename = Path.GetFileName(rightFile.href.Substring(rightFile.href.LastIndexOf('/') + 1));
            FileInfo fl = new FileInfo(leftfilename);
            FileInfo fr = new FileInfo(rightfilename);
            return fl.Length.CompareTo(fr.Length);
        }
        #endregion
    }
}
