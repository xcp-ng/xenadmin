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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using DiscUtils;
using DiscUtils.BootConfig;
using DiscUtils.Ntfs;
using DiscUtils.Partitions;
using DiscUtils.Registry;
using DiscUtils.Wim;
using XenAdmin.Core;
using XenAPI;
using XenOvf;
using XenOvf.Definitions;
using XenOvf.Definitions.XENC;
using XenOvf.Definitions.VMC;
using XenCenterLib.Compression;

using XenOvf.Utilities;

namespace XenOvfTransport
{
    public class Import : XenOvfTransportBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const long KB = 1024;
        private const long MB = (KB * 1024);
        private const long GB = (MB * 1024);
        private const long STATMEMMIN = 16 * MB;

        public enum TransferType 
        { 
            UploadRawVDI = 0,
            iSCSI = 1,
            SFTP = 2,
            Skip = 3,
            Unknown = 4 
        }

        public enum TransferMethod 
        {
            Image = 0,
            Files = 1,
            Unknown = 2
        }

		private string downloadupdatemsg;
        private string EncryptionClass;
        private int EncryptionKeySize;
        private List<XenRef<VM>> P2VVMServerList = new List<XenRef<VM>>();
        private XenRef<SR> DefaultSRUUID = null;
        private readonly Http http;
        private VirtualDisk vhdDisk = null;
        private WimFile wimDisk = null;
        private ulong wimFileCount = 0;
        private ulong wimFileIndex = 0;
        private ulong AdditionalSpace = 20 * GB;
        private int vifDeviceIndex = 0;  // used to count number of Networks attached and to be used as the device number.
        private string _currentfilename = null;
        private Exception _downloadexception = null;
        private ulong _filedownloadsize = 0;
        private int xvadisk = 0;
        private AutoResetEvent uridownloadcomplete = new AutoResetEvent(false);

        public string ApplianceName { get; set; }

        public bool MetaDataOnly { get; set; }

		#region Constructors

        public Import(Uri xenserver, Session session)
            : base(xenserver, session)
        {
            http = new Http { UpdateHandler = http_UpdateHandler };
        }

		#endregion

		#region PUBLIC

        public void Process(EnvelopeType ovfObj, string pathToOvf, string passcode)
        {
            var xenSession = XenSession;
            if (xenSession == null)
                throw new InvalidOperationException(Messages.ERROR_NOT_CONNECTED);

            string ovfname = Guid.NewGuid().ToString();

            vifDeviceIndex = 0;
            string encryptionVersion = null;

            #region CHECK ENCRYPTION
            if (OVF.HasEncryption(ovfObj))
            {
                if (passcode == null)
                {
                	throw new InvalidDataException(Messages.ERROR_NO_PASSWORD);
                }
                string fileuuids = null;
                SecuritySection_Type[] securitysection = OVF.FindSections<SecuritySection_Type>((ovfObj).Sections);
                if (securitysection != null && securitysection.Length >= 0)
                {
                    foreach (Security_Type securitytype in securitysection[0].Security)
                    {
                        if (securitytype.ReferenceList.Items != null && securitytype.ReferenceList.Items.Length > 0)
                        {
                            foreach (XenOvf.Definitions.XENC.ReferenceType dataref in securitytype.ReferenceList.Items)
                            {
                                if (dataref is DataReference)
                                {
                                    fileuuids += ":" + ((DataReference)dataref).ValueType;
                                }
                            }
                        }
                        if (securitytype.EncryptionMethod != null && securitytype.EncryptionMethod.Algorithm != null)
                        {
                            string algoname = (securitytype.EncryptionMethod.Algorithm.Split('#'))[1].ToLower().Replace('-', '_');
                            object x = OVF.AlgorithmMap(algoname);
                            if (x != null)
                            {
                                EncryptionClass = (string)x;
                                EncryptionKeySize = Convert.ToInt32(securitytype.EncryptionMethod.KeySize);
                            }
                        }

                        if (!string.IsNullOrEmpty(securitytype.version))
                        {
                            encryptionVersion = securitytype.version;                                
                        }
                    }
                }
            }
            #endregion

            #region FIND DEFAULT SR 
            Dictionary<XenRef<Pool>, Pool> pools = Pool.get_all_records(xenSession);
            foreach (XenRef<Pool> pool in pools.Keys)
            {
                DefaultSRUUID = pools[pool].default_SR;
                break;
            }
            #endregion

            //
            // So the process is the same below, change this
            //
            if (ovfObj.Item is VirtualSystem_Type)
            {
				VirtualSystem_Type vstemp = (VirtualSystem_Type)ovfObj.Item;
				ovfObj.Item = new VirtualSystemCollection_Type();
				((VirtualSystemCollection_Type)ovfObj.Item).Content = new Content_Type[] { vstemp };
            }

			#region Create appliance

        	XenRef<VM_appliance> applRef = null;
			if (ApplianceName != null)
			{
				var vmAppliance = new VM_appliance {name_label = ApplianceName, Connection = xenSession.Connection};
				applRef = VM_appliance.create(xenSession, vmAppliance);
			}

        	#endregion

			foreach (VirtualSystem_Type vSystem in ((VirtualSystemCollection_Type)ovfObj.Item).Content)
            {
               //FIND/SET THE NAME OF THE VM
				ovfname = OVF.FindSystemName(ovfObj, vSystem.id);
                log.DebugFormat("Import: {0}, {1}", ovfname, pathToOvf);

				VirtualHardwareSection_Type vhs = OVF.FindVirtualHardwareSectionByAffinity(ovfObj, vSystem.id, "xen");

                XenRef<VM> vmRef = DefineSystem(xenSession, vhs, ovfname);
                if (vmRef == null)
                {
                    log.Error("Failed to create a VM");
                    throw new Exception(Messages.ERROR_CREATE_VM_FAILED);
				}

				HideSystem(xenSession, vmRef);
                log.DebugFormat("OVF.Import.Process: DefineSystem completed ({0})", VM.get_name_label(xenSession, vmRef));
				
				#region Set appliance
				if (applRef != null)
					VM.set_appliance(xenSession, vmRef.opaque_ref, applRef.opaque_ref);

				if (ovfObj.Sections != null)
				{
					StartupSection_Type[] startUpArray = OVF.FindSections<StartupSection_Type>(ovfObj.Sections);
					if (startUpArray != null && startUpArray.Length > 0)
					{
						var startupSection = startUpArray[0];
						var itemList = startupSection.Item;

						if (itemList != null)
						{
							var item = itemList.FirstOrDefault(it => it.id == vSystem.id);

							if (item != null)
							{
								VM.set_start_delay(xenSession, vmRef.opaque_ref, item.startDelay);
								VM.set_shutdown_delay(xenSession, vmRef.opaque_ref, item.stopDelay);
								VM.set_order(xenSession, vmRef.opaque_ref, item.order);
							}
						}
					}
				}

            	#endregion

                #region set has_vendor_device

                if (Helpers.DundeeOrGreater(xenSession.Connection))
                {
                    var data = vhs.VirtualSystemOtherConfigurationData;
                    if (data != null)
                    {
                        var datum = data.FirstOrDefault(s => s.Name == "VM_has_vendor_device");
                        if (datum != null)
                        {
                            bool hasVendorDevice;
                            if (bool.TryParse(datum.Value.Value, out hasVendorDevice) && hasVendorDevice)
                                VM.set_has_vendor_device(xenSession, vmRef.opaque_ref, hasVendorDevice);
                        }
                    }
                }

                #endregion

                #region Set vgpu
                
                List<Tuple<GPU_group, VGPU_type> >vgpus = FindGpuGroupAndVgpuType(xenSession, vhs);
                foreach (var item in vgpus)
                {
                    GPU_group gpuGroup = item.Item1;
                    VGPU_type vgpuType = item.Item2;
                    if (gpuGroup != null)
                    {
                        var other_config = new Dictionary<string, string>();

                        if (Helpers.FeatureForbidden(xenSession, Host.RestrictVgpu))
                        {
                            // The host does not support vGPU feature, so we create passthrough (default vGPU type) vGPU type
                            // for the VM. However, passthrough vGPU type does not support multiple at this moment, so we only
                            // create one vGPU for the VM and ignore the others. The limitation need to be released if multiple
                            // passthrough can be supported
                            log.Debug("The host license does not support vGPU, create one passthrough vGPU for the VM");
                            VGPU.create(xenSession, vmRef.opaque_ref, gpuGroup.opaque_ref, "0", other_config);
                            break;
                        }
                        else if (vgpuType != null)
                            VGPU.create(xenSession, vmRef.opaque_ref, gpuGroup.opaque_ref, "0", other_config, vgpuType.opaque_ref);
                    }
                }

                #endregion

                SetDeviceConnections(ovfObj, vhs);
                try
                {
                    foreach (RASD_Type rasd in vhs.Item)
                    {
                        string thisPassCode = null;
                        // Check to see if THIS rasd is encrypted, if so, set the passcode.
                        if (OVF.IsThisEncrypted(ovfObj, rasd))
                            thisPassCode = passcode;

                        string compression = "None";
                        if (rasd.ResourceType.Value == 17 || rasd.ResourceType.Value == 19 || rasd.ResourceType.Value == 21)
                        {
							bool skip = Tools.ValidateProperty("Caption", rasd) &&
								 (
								  rasd.Caption.Value.ToUpper().Contains("COM") ||
								  rasd.Caption.Value.ToUpper().Contains("FLOPPY") ||
								  rasd.Caption.Value.ToUpper().Contains("ISO")
								 );

                            if (!skip)
                            {
                                File_Type file = OVF.FindFileReferenceByRASD(ovfObj, rasd);
								if (file == null)
									continue;
                                
								if (IsKnownURIType(file.href))
                                    _filedownloadsize = file.size;

                                VirtualDiskDesc_Type vdisk = OVF.FindDiskReference(ovfObj, rasd);
								SetIfDeviceIsBootable(ovfObj, rasd);
                                AdditionalSpace = OVF.ComputeCapacity(Convert.ToInt64(vdisk.capacity), vdisk.capacityAllocationUnits);  // used in Wim file imports only.
								AddResourceSettingData(xenSession, vmRef, rasd, pathToOvf, OVF.FindRasdFileName(ovfObj, rasd, out compression), compression, encryptionVersion, thisPassCode);
                            }
                        }
                        else
                        {
							AddResourceSettingData(xenSession, vmRef, rasd, pathToOvf, OVF.FindRasdFileName(ovfObj, rasd, out compression), compression, encryptionVersion, thisPassCode);
                        }
                    }

                    InstallSection_Type[] installSection = OVF.FindSections<InstallSection_Type>(vSystem.Items);

                    if (installSection != null && installSection.Length == 1)
                    {
                        OnUpdate(new XenOvfTransportEventArgs(TransportStep.Import, Messages.START_POST_INSTALL_INSTRUCTIONS));
                        OnUpdate(new XenOvfTransportEventArgs(TransportStep.Import, Messages.START_POST_INSTALL_INSTRUCTIONS));
                        HandleInstallSection(xenSession, vmRef, installSection[0]);
                    }
                    ShowSystem(xenSession, vmRef);

                    #region PVS Proxy
                    var site = FindPvsSite(xenSession, vhs);

                    if (site != null)
                    {
                        var vm =  xenSession.Connection.Resolve(vmRef);
                        if (vm != null)
                        {
                            var vifs = xenSession.Connection.ResolveAll(vm.VIFs);
                            var firstVif = vifs.FirstOrDefault(v => v.device.Equals("0"));

                            if (firstVif != null)
                            {
                                var foundSite = PVS_site.get_by_uuid(xenSession, site.uuid);

                                if (foundSite != null)
                                {
                                    PVS_proxy.create(xenSession, foundSite.opaque_ref, firstVif.opaque_ref);
                                }
                            }
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
					if (ex is OperationCanceledException)
						throw;
                    log.Error("Import failed", ex);
                    throw new Exception(Messages.ERROR_IMPORT_FAILED, ex);
                }
            }

            OnUpdate(new XenOvfTransportEventArgs(TransportStep.Import, Messages.COMPLETED_IMPORT));
        }

    	#endregion

        #region PRIVATE
		
		private void http_UpdateHandler(XenOvfTransportEventArgs e)
		{
			OnUpdate(e);
		}

        private void HandleInstallSection(Session xenSession, XenRef<VM> vm, InstallSection_Type installsection)
        {
            // Configure for XenServer as requested by OVF.SetRunOnceBootCDROM() with the presence of a post install operation that is specific to XenServer.
            if (installsection.PostInstallOperations != null)
                ConfigureForXenServer(xenSession, vm);

            // Run the VM for the requested duration if this appliance had its own install section -- one not added to fixup for XenServer.
            if (installsection.Info == null ||
                installsection.Info != null && installsection.Info.Value.CompareTo("ConfigureForXenServer") != 0)
                InstallSectionStartVirtualMachine(xenSession, vm, installsection.initialBootStopDelay);
        }

        private void ConfigureForXenServer(Session xenSession, XenRef<VM> vm)
        {
            // Ensure the new VM is down.
            if (VM.get_power_state(xenSession, vm) != vm_power_state.Halted)
                VM.hard_shutdown(xenSession, vm);

            while (VM.get_power_state(xenSession, vm) != vm_power_state.Halted)
                Thread.Sleep(Properties.Settings.Default.FixupPollTimeAsMs);

            // Save its original memory configuration.
            long staticMemoryMin  = VM.get_memory_static_min(xenSession, vm);
            long staticMemoryMax  = VM.get_memory_static_max(xenSession, vm);
            long dynamicMemoryMin = VM.get_memory_dynamic_min(xenSession, vm);
            long dynamicMemoryMax = VM.get_memory_dynamic_max(xenSession, vm);

            // Minimize the memory capacity for the fixup OS.
            long fixupMemorySize = Properties.Settings.Default.FixupOsMemorySizeAsMB * MB;

            VM.set_memory_limits(xenSession, vm, fixupMemorySize, fixupMemorySize, fixupMemorySize, fixupMemorySize);

            // Run the fixup OS on the VM.
            InstallSectionStartVirtualMachine(xenSession, vm, Properties.Settings.Default.FixupDurationAsSeconds);

            // Restore the original memory configuration.
            VM.set_memory_limits(xenSession, vm, staticMemoryMin, staticMemoryMax, dynamicMemoryMin, dynamicMemoryMax);

            // Eject the fixupOS CD.
            List<XenRef<VBD>> vbdList = VM.get_VBDs(xenSession, vm);
            foreach (XenRef<VBD> vbd in vbdList)
            {
                if (VBD.get_type(xenSession, vbd) == vbd_type.CD)
                    VBD.eject(xenSession, vbd);

                // Note that the original code did not destroy the VBD representing the CD drive.
            }

            // Restore the boot order.
            Dictionary<string, string> bootParameters = new Dictionary<string, string>();
            bootParameters.Add("order", "cnd");
            VM.set_HVM_boot_params(xenSession, vm, bootParameters);
        }


        private void InstallSectionStartVirtualMachine(Session xenSession, XenRef<VM> vm, int initialBootStopDelayAsSeconds)
        {
            log.InfoFormat("Running fixup on VM with opaque_ref {0}", vm.opaque_ref);

            // Start the VM.
            if (VM.get_power_state(xenSession, vm) != vm_power_state.Running)
                VM.start(xenSession, vm, false, true);

            // Finish early if requested to stop on its own.
            if (initialBootStopDelayAsSeconds == 0)
                return;

            // Wait for it to start.
            while (VM.get_power_state(xenSession, vm) != vm_power_state.Running)
                Thread.Sleep(Properties.Settings.Default.FixupPollTimeAsMs);

            // Let it run for the requested duration.
            int bootStopDelayAsMs = initialBootStopDelayAsSeconds * 1000;
            int msRunning = 0;

            while (VM.get_power_state(xenSession, vm) == vm_power_state.Running)
            {
                Thread.Sleep(Properties.Settings.Default.FixupPollTimeAsMs);
                msRunning += Properties.Settings.Default.FixupPollTimeAsMs;

                if (msRunning > bootStopDelayAsMs)
                    break;
            }

            // Ensure it is off.
            if (VM.get_power_state(xenSession, vm) == vm_power_state.Halted)
                return;

            try
            {
                VM.hard_shutdown(xenSession, vm);
            }
            catch (Exception e)
            {
                log.InfoFormat("Unable to hard-shutdown VM {0}. Will ignore error: {1}", vm.opaque_ref, e.Message);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
                                                         Justification = "Logging mechanism")]
        internal List<XenRef<VDI>> ImportFile(Session xenSession, string vmname, string pathToOvf, string filename, string compression, string version, string passcode, string sruuid, string description, string vdiuuid)
        {
            List<XenRef<VDI>> vdiRef = new List<XenRef<VDI>>();

            // Get the disk transport method from the configuration in XenOvfTransport.Properties.Settings.TransferType.
            TransferType useTransport = (TransferType)Enum.Parse(typeof(TransferType), Properties.Settings.Default.TransferType, true);
            TransferMethod useTransferMethod = TransferMethod.Image;
            string sourcefile = filename;
            string encryptfilename = null;
            string uncompressedfilename = null;
            string StartPath = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(pathToOvf);
            Stream dataStream = null;
            long dataCapacity = 0;

            #region SET UP TRANSPORT
            if (filename != null)
            {
                if (IsKnownURIType(filename))
                {
                    Uri fileUri = new Uri(filename);
                    filename = DownloadFileAsync(fileUri, 0);
                    sourcefile = filename;
                }

                if (File.Exists(filename))
                {
                    string ext = Path.GetExtension(filename);

                    try
                    {
                        encryptfilename = "enc_" + filename;
                        uncompressedfilename = "unc_" + filename;
                        // OK.. lets see is the file encrypted?
                        #region ENCRYPTION
                        if (passcode != null)
                        {
                            var statusMessage = string.Format(Messages.START_FILE_DECRYPTION, filename);
                            OnUpdate(new XenOvfTransportEventArgs(TransportStep.Security, statusMessage));
                            log.Debug($"Decrypting {filename}");
                            OVF.DecryptToTempFile(EncryptionClass, filename, version, passcode, encryptfilename);
                            sourcefile = encryptfilename;
                            statusMessage += Messages.COMPLETE;
                            OnUpdate(new XenOvfTransportEventArgs(TransportStep.Security, statusMessage));
                        }
                        #endregion 

                        #region COMPRESSION
                        // Identity == no compression, it is meant when a URL is used to identify the compression during transport.
                        if (compression != null && 
                            !compression.ToLower().Equals("none") && 
                            !compression.ToLower().Equals("identity"))
                        {
                            // gz is the only understood 'compressed' format, strip it..
                            // the OVF is marked with "compressed=gzip" therefor it will get decompress
                            // correctly and use with its disk extension (vmdk/vhd/vdi)...
                            if (ext.ToLower().EndsWith(".gz"))
                            {
                                string newfilename = Path.GetFileNameWithoutExtension(uncompressedfilename);
                                uncompressedfilename = newfilename;
                                ext = Path.GetExtension(uncompressedfilename);
                            }
                            var statusMessage = string.Format(Messages.START_FILE_EXPANSION, filename);
                            OnUpdate(new XenOvfTransportEventArgs(TransportStep.Compression, statusMessage));
                        	var ovfCompressor = new OvfCompressor();
							ovfCompressor.UncompressFile(sourcefile, uncompressedfilename, compression);
                            if (File.Exists(encryptfilename)) { File.Delete(encryptfilename); }
                            sourcefile = uncompressedfilename;
                            statusMessage += Messages.COMPLETE;
                            OnUpdate(new XenOvfTransportEventArgs(TransportStep.Compression, statusMessage));
                        }
                        #endregion

                        #region DISK SELECTION
                        bool knownDisk = false;
                        foreach (string diskext in VirtualDisk.SupportedDiskFormats)
                        {
                            if (ext.ToLower().Contains(diskext.ToLower()))
                            {
                                knownDisk = true;
                                break;
                            }
                        }
                        if (knownDisk)
                        {
                            log.DebugFormat("Found file {0} using {1} Stream", filename, ext);
                            vhdDisk = VirtualDisk.OpenDisk(sourcefile, FileAccess.Read);
                            dataStream = vhdDisk.Content;
                            dataCapacity = vhdDisk.Capacity;
                        }
                        else if (ext.ToLower().Contains("wim"))
                        {
                            log.WarnFormat("EXPERIMENTAL CODE: Found file {0} using WIM file structure", filename);
                            wimDisk = new DiscUtils.Wim.WimFile(new FileStream(sourcefile, FileMode.Open, FileAccess.Read));
                            //wimFS = wimDisk.GetImage(wimDisk.BootImage);

                            dataStream = null;

                            string manifest = wimDisk.Manifest;
                            Wim_Manifest wimManifest = Tools.Deserialize<Wim_Manifest>(manifest);
                            ulong imagesize = wimManifest.Image[wimDisk.BootImage].TotalBytes; // <----<<< Image data size
                            wimFileCount = wimManifest.Image[wimDisk.BootImage].FileCount;
                            dataCapacity = (long)(imagesize + AdditionalSpace);
                            useTransferMethod = TransferMethod.Files;
                        }
                        else if (ext.ToLower().Contains("xva"))
                        {
                            log.WarnFormat("EXPERIMENTAL CODE: Found file {0} using XVA Stream (DISK {1} is being imported).", filename, xvadisk);
                            DiscUtils.Xva.VirtualMachine vm = new DiscUtils.Xva.VirtualMachine(new FileStream(sourcefile, FileMode.Open, FileAccess.Read));
                            int i = 0;
                            foreach (DiscUtils.Xva.Disk d in vm.Disks)
                            {
                                if (i == xvadisk)
                                {
                                    vhdDisk = d;
                                    break;
                                }
                            }
                            dataStream = vhdDisk.Content;
                            dataCapacity = vhdDisk.Capacity;
                        }
                        else if (ext.ToLower().EndsWith("iso"))
                        {
                            if (string.IsNullOrEmpty(sruuid))
                            {
                                useTransport = TransferType.Skip;
                            }
                            else
                            {
                                //DiscUtils.Iso9660.CDReader cdr = new DiscUtils.Iso9660.CDReader(File.OpenRead(filename), true);
                                dataStream = File.OpenRead(filename);
                                dataCapacity = dataStream.Length + (512 * KB);  // Xen does 512KB rounding this is to ensure it doesn't round down below size.
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        log.Error("Failed to open virtual disk", ex);
                        throw new Exception(Messages.ISCSI_ERROR_CANNOT_OPEN_DISK, ex);
                    }
                }
                else
                {
                    throw new FileNotFoundException(string.Format(Messages.FILE_MISSING, filename));
                }
            }
            else
            {
                log.Error("The file to import was not provided");
                throw new InvalidDataException(Messages.ERROR_FILE_NAME_NULL);
            }
            #endregion
                        
            try
            {
                #region SEE IF TARGET SR HAS ENOUGH SPACE
                if (useTransport == TransferType.UploadRawVDI || useTransport == TransferType.iSCSI)
                {
                    long freespace;
                    string contenttype = string.Empty;
                    if(vdiuuid != null)
                    {
                        XenRef<VDI> vdiLookup = VDI.get_by_uuid(xenSession, vdiuuid);
                        freespace = VDI.get_virtual_size(xenSession, vdiLookup);
                    }
                    else
                    {
                        XenRef<SR> srRef = SR.get_by_uuid(xenSession, sruuid);
                        long size = SR.get_physical_size(xenSession, srRef);
                        long usage = SR.get_physical_utilisation(xenSession, srRef);
                        contenttype = SR.get_content_type(xenSession, srRef);
                        freespace = size - usage;
                    }

                    if (freespace <= dataCapacity)
                    {
                        log.Error($"SR {sruuid} does not have {vhdDisk.Capacity} bytes of free space to import virtual disk {filename}.");
                        string message = string.Format(Messages.NOT_ENOUGH_SPACE_IN_SR, sruuid, Convert.ToString(vhdDisk.Capacity), filename);
                        throw new IOException(message);
                    }
                }
                #endregion

                #region UPLOAD FILE
                switch (useTransport)
                {
                    case TransferType.UploadRawVDI:
                        {
                            vdiRef.Add(UploadRawVDI(xenSession, sruuid, vmname, dataStream, dataCapacity, description));
                            break;
                        }
                    case TransferType.iSCSI:
                        {
                            if (useTransferMethod == TransferMethod.Image)
                            {
                                vdiRef.Add(UploadiSCSI(xenSession, sruuid, vmname, dataStream, dataCapacity, description, vdiuuid));
                            }
                            else
                            {
                                for (int i = 0; i < wimDisk.ImageCount; i++)
                                {
                                    Wim_Manifest wimManifest = Tools.Deserialize<Wim_Manifest>(wimDisk.Manifest);
                                    wimFileCount = wimManifest.Image[i].FileCount;
                                    int wimArch = wimManifest.Image[i].Windows.Architecture;
                                    vdiRef.Add(UploadiSCSIbyWimFile(xenSession, sruuid, vmname, wimDisk, i, dataCapacity, wimFileCount, wimArch, ""));
                                }
                            }
                            break;
                        }
                    case TransferType.Skip:
                        {
                            log.Info("ImportFile: Upload Skipped");
                            break;
                        }
                    default:
                		{
                            log.Error($"Unsupported transfer type {useTransport.ToString()}");
                            throw new InvalidDataException(Messages.UNSUPPORTED_TRANSPORT);
                        }
                }
                #endregion
            }
            catch (Exception ex)
            {
				if (ex is OperationCanceledException)
					throw;
				throw new Exception(Messages.FILE_TRANSPORT_FAILED, ex);
            }
            finally
            {
                if (vhdDisk != null)
                {
                    vhdDisk.Dispose();
                    vhdDisk = null;
                }
                if (wimDisk != null)
                {
                    wimDisk = null;
                }

                try
                {
                    if (File.Exists(encryptfilename))
                        File.Delete(encryptfilename);

                    if (File.Exists(uncompressedfilename))
                        File.Delete(uncompressedfilename);
                }
                catch
                {
                    //ignore errors
                }

                Directory.SetCurrentDirectory(StartPath);
            }


            log.DebugFormat("OVF.Import.ImportFile leave: created {0} VDIs", vdiRef.Count);
            return vdiRef;
        }

        private XenRef<VDI> UploadiSCSI(Session xenSession, string sruuid, string label, Stream filestream, long capacity, string description, string vdiuuid)
        {
            log.Debug($"OVF.Import.UploadiSCSI SRUUID: {sruuid}, Label: {label}, Capacity: {capacity}");

            XenRef<VDI> vdiRef = null;

            //If no VDI uuid is provided create a VDI, otherwise use the one provided as
            //the target for the import. Used for SRs such as Lun per VDI (PR-1544)
            if(String.IsNullOrEmpty(vdiuuid))
            {
                vdiRef = CreateVDI(xenSession, sruuid, label, capacity, description);
                vdiuuid = VDI.get_uuid(xenSession, vdiRef);
            }
            else
            {
                vdiRef = new XenRef<VDI>(VDI.get_by_uuid(xenSession, vdiuuid));
            }
                

            #region UPLOAD iSCSI STREAM
            OnUpdate(new XenOvfTransportEventArgs(TransportStep.Import, string.Format(Messages.FILES_TRANSPORT_SETUP, _currentfilename)));
        	m_iscsi = new iSCSI
        	        	{
        	        		UpdateHandler = iscsi_UpdateHandler,
        	        		Cancel = Cancel //in case it has already been cancelled
        	        	};
			m_iscsi.ConfigureTvmNetwork(m_networkUuid, m_isTvmIpStatic, m_tvmIpAddress, m_tvmSubnetMask, m_tvmGateway);
            try
            {
                using (Stream iSCSIStream = m_iscsi.Connect(xenSession, vdiuuid, false))
                {
					m_iscsi.Copy(filestream, iSCSIStream, label, false);
                    iSCSIStream.Flush();
                }
            }
            catch (Exception ex)
            {
				if (ex is OperationCanceledException)
					throw;
                log.Error("Failed to import a virtual disk over iSCSI. ", ex);
                vdiRef = null;
                throw new Exception(Messages.ERROR_ISCSI_UPLOAD_FAILED, ex);
            }
            finally
            {
                OnUpdate(new XenOvfTransportEventArgs(TransportStep.Import, string.Format(Messages.FILES_TRANSPORT_CLEANUP,_currentfilename)));
				m_iscsi.Disconnect(xenSession);
            }
            #endregion

            log.Debug("OVF.Import.UploadiSCSI Leave");
            return vdiRef;
        }

        private XenRef<VDI> UploadiSCSIbyWimFile(Session xenSession, string sruuid, string label, WimFile wimDisk, int imageindex, long capacity, ulong wimFileCount, int arch, string description)
        {
            log.Debug($"OVF.Import.UploadiSCSIbyWimFile SRUUID: {sruuid}, Label: {label}, ImageIndex: {imageindex}, Capacity: {capacity}");

            string vdilabel = string.Format("{0}{1}", label, imageindex);
            XenRef<VDI> vdiRef = CreateVDI(xenSession, sruuid, vdilabel, capacity, description);

            byte[] mbr = null;
            byte[] boot = null;
            //byte[] bcd = null;
            //byte[] bootmgr = null;

            string vhdfile = FindReferenceVHD(Properties.Settings.Default.ReferenceVHDName);
            if (File.Exists(vhdfile))
            {
                mbr = ExtractMBRFromVHD(vhdfile);
                boot = ExtractBootFromVHD(vhdfile);
                //bcd = ExtractBCDFromVHD(vhdfile, arch);
                //bootmgr = ExtractBootmgrFromVHD(vhdfile, arch);
            }
            else
            {
                log.WarnFormat("Reference VHD not found [{0}]", Properties.Settings.Default.ReferenceVHDName);
            }

            #region UPLOAD iSCSI STREAM
            OnUpdate(new XenOvfTransportEventArgs(TransportStep.Import, string.Format(Messages.FILES_TRANSPORT_SETUP, _currentfilename)));
			m_iscsi = new iSCSI
			{
				UpdateHandler = iscsi_UpdateHandler,
				Cancel = Cancel //in case it has already been cancelled
			};
			m_iscsi.ConfigureTvmNetwork(m_networkUuid, m_isTvmIpStatic, m_tvmIpAddress, m_tvmSubnetMask, m_tvmGateway);
            try
            {
                wimFileIndex = 0;
                string vdiuuid = VDI.get_uuid(xenSession, vdiRef);
				Stream iSCSIStream = m_iscsi.Connect(xenSession, vdiuuid, false);
                WimFileSystem w = wimDisk.GetImage(imageindex);

                if (mbr != null)
                {
					m_iscsi.ScsiDisk.SetMasterBootRecord(mbr);
                }
                else
                {
                    log.WarnFormat("System will not be bootable, cannot find [{0}] to extract master boot record.", vhdfile);
                    OnUpdate(new XenOvfTransportEventArgs(TransportStep.Import, Messages.WARNING_TARGET_NOT_BOOTABLE));
                }
				m_iscsi.ScsiDisk.Signature = new Random().Next();

				BiosPartitionTable table = BiosPartitionTable.Initialize(m_iscsi.ScsiDisk, WellKnownPartitionType.WindowsNtfs);
				VolumeManager volmgr = new VolumeManager(m_iscsi.ScsiDisk);

                NtfsFileSystem ntfs = null;
                if (wimDisk.BootImage == imageindex && boot != null)
                {
                    table.SetActivePartition(0);
                    ntfs = NtfsFileSystem.Format(volmgr.GetLogicalVolumes()[0], "New Volume", boot);
                }
                else
                {
                    ntfs = NtfsFileSystem.Format(volmgr.GetLogicalVolumes()[0], "New Volume");
                }

                //AddBCD(ntfs, bcd);
                //AddBootMgr(ntfs, bootmgr);  // If it's not there it'll be created if it is it will not.. not below filecopy will overwrite if one it exists.

				FileCopy(m_iscsi, w.Root.GetFiles(), w, ntfs);
				FileCopy(m_iscsi, w.Root.GetDirectories(), w, ntfs);

                FixBCD(ntfs, volmgr);

                ntfs.Dispose();

            }
            catch (Exception ex)
            {
				if (ex is OperationCanceledException)
					throw;
                log.Error("Failed to import a virtual disk over iSCSI.", ex);
                vdiRef = null;
                throw new Exception(Messages.ERROR_ISCSI_UPLOAD_FAILED, ex);
            }
            finally
            {
                OnUpdate(new XenOvfTransportEventArgs(TransportStep.Import, string.Format(Messages.FILES_TRANSPORT_CLEANUP, _currentfilename)));
				m_iscsi.Disconnect(xenSession);
            }
            #endregion

            log.Debug("OVF.Import.UploadiSCSIbyWimFile Leave");
            return vdiRef;
        }

		private void iscsi_UpdateHandler(XenOvfTransportEventArgs e)
		{
			OnUpdate(e);
		}

        private XenRef<VDI> CreateVDI(Session xenSession, string sruuid, string label, long capacity, string description)
        {
            VDI vdi = new VDI
            {
                uuid = Guid.NewGuid().ToString(),
                name_label = label,
                name_description = description,
                SR = sruuid.ToLower().StartsWith("opaque") ? new XenRef<SR>(sruuid) : SR.get_by_uuid(xenSession, sruuid),
                virtual_size = capacity,
                physical_utilisation = capacity,
                type = vdi_type.user,
                sharable = false,
                read_only = false,
                storage_lock = false,
                managed = true,
                is_a_snapshot = false
            };

            try
            {
                // Note that XenServer will round the capacity up to the nearest multiple of a 2 MB block.
                var vdiRef = VDI.create(xenSession, vdi);
                log.Debug("Import.CeateVDI::VDI Created");
                return vdiRef;
            }
            catch (Exception ex)
            {
                log.Error("Failed to create VDI. ", ex);
                throw new Exception(Messages.ERROR_CANNOT_CREATE_VDI, ex);
            }
        }

        private void FileCopy(iSCSI iscsi, DiscDirectoryInfo[] DirInfos, WimFileSystem w, NtfsFileSystem ntfs)
        {
            foreach (DiscDirectoryInfo dir in DirInfos)
            {
                if (IsExcluded(dir.FullName))
                {
                    log.InfoFormat("Directory Skip {0}", dir.FullName);
                    continue;
                }
                FileAttributes attr = dir.Attributes;
                if ((dir.Attributes & FileAttributes.ReparsePoint) == 0)
                {
                    ntfs.CreateDirectory(dir.FullName);
                    if ((attr & FileAttributes.Temporary) == FileAttributes.Temporary)
                        attr = attr & ~FileAttributes.Temporary;
                    if ((attr & FileAttributes.Offline) == FileAttributes.Offline)
                        attr = attr & ~FileAttributes.Offline;
                    ntfs.SetAttributes(dir.FullName, attr);

                    FileCopy(iscsi, dir.GetDirectories(), w, ntfs);
                    FileCopy(iscsi, dir.GetFiles(), w, ntfs);
                }
                else
                {
                    log.InfoFormat("Directory ReparsePoint {0}", dir.FullName);
                    ReparsePoint rp = w.GetReparsePoint(dir.FullName);
                    ntfs.CreateDirectory(dir.FullName);
                    ntfs.SetReparsePoint(dir.FullName, rp);
                }
            }
        }

        private void FileCopy(iSCSI iscsi, DiscFileInfo[] FileInfos, WimFileSystem w, NtfsFileSystem ntfs)
        {
            foreach (DiscFileInfo file in FileInfos)
            {
                if (IsExcluded(file.FullName))
                {
                    log.InfoFormat("File Skip {0}", file.FullName);
                    continue;
                }
                FileAttributes attr = file.Attributes;
                if ((attr & FileAttributes.ReparsePoint) == 0)
                {
                    using (Stream source = w.OpenFile(file.FullName, FileMode.Open, FileAccess.Read))
                    {
                        using (Stream destin = ntfs.OpenFile(file.FullName, FileMode.Create, FileAccess.Write))
                        {
                            iscsi.WimCopy(source, destin, Path.GetFileName(file.FullName), false, wimFileIndex++, wimFileCount);
                        }
                    }

                    if ((attr & FileAttributes.Temporary) == FileAttributes.Temporary)
                        attr = attr & ~FileAttributes.Temporary;
                    if ((attr & FileAttributes.Offline) == FileAttributes.Offline)
                        attr = attr & ~FileAttributes.Offline;
                    ntfs.SetAttributes(file.FullName, attr);
                }
                else
                {
                    log.InfoFormat("Reparse Point: {0}", file.FullName);
                    ReparsePoint rp = w.GetReparsePoint(file.FullName);
                    ntfs.SetReparsePoint(file.FullName, rp);
                }
            }
        }

        private bool IsExcluded(string path)
        {
            string pathUpper = path.ToUpperInvariant();

            foreach(string excluded in Properties.Settings.Default.EXCLUDED_FILES_COPY)
            {
                if (pathUpper == excluded.ToUpperInvariant())
                {
                    return true;
                }
            }

            return false;
        }

        private XenRef<VDI> UploadRawVDI(Session xenSession, string sruuid, string label, Stream filestream, long capacity, string description)
        {
            log.Debug($"OVF.Import.UploadRawVDI SRUUID: {sruuid}, Label: {label}, Capacity: {capacity}");

            #region CREATE A VDI

            VDI vdi = new VDI
            {
                uuid = Guid.NewGuid().ToString(),
                name_label = label,
                name_description = description,
                SR = sruuid.ToLower().StartsWith("opaque") ? new XenRef<SR>(sruuid) : SR.get_by_uuid(xenSession, sruuid),
                // Add 2MB, VDIs appear to round down making it too small.
                virtual_size = capacity + 2*MB,
                physical_utilisation = capacity + 2*MB,
                type = vdi_type.user,
                sharable = false,
                read_only = false,
                storage_lock = false,
                managed = true,
                is_a_snapshot = false
            };

            XenRef<VDI> vdiRef = null;
            try
            {
                vdiRef = VDI.create(xenSession, vdi);
                log.Debug("Import.UploadRawVDI::VDI Created");
            }
            catch (Exception ex)
            {
                log.Error("Failed to create VDI", ex);
                throw new Exception(Messages.ERROR_CANNOT_CREATE_VDI, ex);
            }

            #endregion

            #region UPLOAD HTTP STREAM
            XenRef<Task> taskRef = Task.create(xenSession, "UpdateStream", "UpdateStream");
            string p2VUri = string.Format("/import_raw_vdi?session_id={0}&task_id={1}&vdi={2}", xenSession.opaque_ref, taskRef.opaque_ref, vdiRef.opaque_ref);
            NameValueCollection headers = new NameValueCollection();
            headers.Add("Content-Length", Convert.ToString(capacity));
            headers.Add("Content-Type", "application/octet-stream");
            headers.Add("Expect", "100-continue");
            headers.Add("Accept", "*/*");
            headers.Add("Connection", "close");
            headers.Add("User-Agent", "XenP2VClient/1.5");
            try
            {
                http.Put(filestream, _uri, p2VUri, headers, 0, capacity, false);
            }
            catch (Exception ex)
            {
                log.Error("Failed to import a virtual disk over HTTP. ", ex);
                if (vdiRef != null)
                {
                    try
                    {
                        VDI.destroy(xenSession, vdi.opaque_ref);
                    }
                    catch (Exception e)
                    {
                        log.Error("Failed to remove a virtual disk image (VDI). ", e);
                        throw new Exception(Messages.ERROR_REMOVE_VDI_FAILED, e);
                    }
                }
                vdiRef = null;
                throw new Exception(Messages.ERROR_HTTP_UPLOAD_FAILED, ex);
            }
            log.Debug("Import.UploadRawVDI::http.put complete");
            #endregion
            
            // give it time to catch up.
            Thread.Sleep(new TimeSpan(0, 0, 5));
            log.Debug("OVF.UploadRawVDI Leave");
            return vdiRef;

        }

        private XenRef<VM> DefineSystem(Session xenSession, VirtualHardwareSection_Type system, string ovfName)
        {
            // Set Defaults:
            Random rand = new Random();
            string vM_name_label = null;
            if (ovfName != null)
            {
                vM_name_label = ovfName;
            }

            string vmUuid = Guid.NewGuid().ToString();
            string systemType = "301";
            ulong memorySize = 512 * MB;  // default size.
            string description = Messages.DEFAULT_IMPORT_DESCRIPTION;

            if (system.System == null)
            {
                log.Debug("OVF.Import.DefineSystem: System VSSD is empty, guessing. HVM style");
                if (vM_name_label == null)
                {
                	vM_name_label = Messages.UNDEFINED_NAME_LABEL;
                }
                vmUuid = Guid.NewGuid().ToString();
                systemType = "hvm-3.0-unknown";
            }
            else
            {                
                if (vM_name_label == null)
                {
                	vM_name_label = Messages.UNDEFINED_NAME_LABEL;
                }
                vmUuid = Guid.NewGuid().ToString();

                if (system.System.VirtualSystemType != null && !string.IsNullOrEmpty(system.System.VirtualSystemType.Value))
                {
                    systemType = system.System.VirtualSystemType.Value;
                }

                if (system.System.Description != null)
                    description = system.System.Description.Value;

            }

            #region MEMORY
            // Get Memory, huh? what? oh.. ya..
            RASD_Type[] rasds = OVF.FindRasdByType(system, 4);
            if (rasds != null && rasds.Length > 0)
            {
                // hopefully only one. but if more ... then deal with it.
                memorySize = 0;
                // These are Default to MB... if other ensure RASD is correct.
                double memoryPower = 20.0;
                double memoryRaise = 2.0;
               
                foreach (RASD_Type rasd in rasds)
                {
                    if (rasd.AllocationUnits.Value.ToLower().StartsWith("bytes"))
                    {
                        // Format:  Bytes * 2 ^ 20
                        string[] a1 = rasd.AllocationUnits.Value.Split('*', '^');
                        if (a1.Length == 3)
                        {
                            memoryRaise = Convert.ToDouble(a1[1]);
                            memoryPower = Convert.ToDouble(a1[2]);
                        }
                    }
                    double memoryMultiplier = Math.Pow(memoryRaise,memoryPower);
                    memorySize += rasd.VirtualQuantity.Value * Convert.ToUInt64(memoryMultiplier);
                }
            }
            #endregion

            #region CPU COUNT
            // Get Memory, huh? what? oh.. ya..
            rasds = OVF.FindRasdByType(system, 3);
            ulong CpuCount = 1;
            if (rasds != null && rasds.Length > 0)
            {
                //
                // Ok this can have more than one CPU and cores each so...
                // Normally each entry is a CPU, the VirtualQuatity is Cores.
                //
                CpuCount = 0;
                foreach (RASD_Type rasd in rasds)
                {
                    CpuCount += rasd.VirtualQuantity.Value;
                }
            }
            #endregion

            var longMemorySize = memorySize > long.MaxValue ? 0 : (long)memorySize;
            var longCpuCount = CpuCount > long.MaxValue ? 0 : (long)CpuCount;

            VM newVm = new VM
            {
                uuid = vmUuid,
                name_label = vM_name_label,
                name_description = description,
                user_version = 1,
                is_a_template = false,
                is_a_snapshot = false,
                memory_target = longMemorySize,
                memory_static_max = longMemorySize,
                memory_dynamic_max = longMemorySize,
                memory_dynamic_min = longMemorySize,
                memory_static_min = STATMEMMIN,
                VCPUs_max = longCpuCount,
                VCPUs_at_startup = longCpuCount,
                actions_after_shutdown = on_normal_exit.destroy,
                actions_after_reboot = on_normal_exit.restart,
                actions_after_crash = on_crash_behaviour.restart,
                HVM_shadow_multiplier = 1.0,
                ha_always_run = false
            };

            #region XEN SPECIFIC CONFIGURATION INFORMATION

            if (system.VirtualSystemOtherConfigurationData == null || system.VirtualSystemOtherConfigurationData.Length <= 0)
            {
                // DEFAULT should work for all of HVM type or 301
                newVm.HVM_boot_policy = Properties.Settings.Default.xenBootOptions;
                newVm.HVM_boot_params = SplitStringIntoDictionary(Properties.Settings.Default.xenBootParams);
				newVm.platform = MakePlatformHash(Properties.Settings.Default.xenPlatformSetting);
            }
            else
            {
                var hashtable = new Hashtable();
                foreach (Xen_ConfigurationSettingData_Type xcsd in system.VirtualSystemOtherConfigurationData)
                {
                    string key = xcsd.Name.Replace('-', '_');
                    switch (key.ToLower())
                    {
                        case "hvm_boot_params":
	                        var xcsdValue = xcsd.Value.Value;
							// Backward Compatibility
							// Before this change, the string in xcsd.Value.Value is just a plain string.
							// And now we would like to change it to support a dictionary string like "key1=value1;key2=value2"
							// However, this logic should also work if a plain string is passed in from an old OVF file 
							newVm.HVM_boot_params = xcsdValue.IndexOf('=') > -1 ? SplitStringIntoDictionary(xcsdValue) : new Dictionary<string, string> { { "order", xcsdValue } };
							break;
                        case "platform":
                            newVm.platform = MakePlatformHash(xcsd.Value.Value);
                            break;
	                    case "nvram":
		                    newVm.NVRAM = SplitStringIntoDictionary(xcsd.Value.Value);
		                    break;
                        case "vgpu":
                            // Skip vGPUs here and do not put vGPUs into the hashtable,
                            // as the vGPUs are updated in #region Set vgpu
                            break;
                        default:
                            hashtable.Add(key, xcsd.Value.Value);
                            break;
                    }
                }
                newVm.UpdateFrom(hashtable);
            }

            #endregion

            try
            {
                return VM.create(xenSession, newVm);                
            }
            catch (Exception ex)
            {
                log.Error("Failed to create a virtual machine (VM).", ex);
                throw new Exception(Messages.ERROR_CREATE_VM_FAILED, ex);
            }
        }

        public static Regex VGPU_REGEX = new Regex("^GPU_types={(.*)};VGPU_type_vendor_name=(.*);VGPU_type_model_name=(.*);$");

        public static Regex PVS_SITE_REGEX = new Regex("^PVS_SITE={uuid=(.*)};$");

        private List<Tuple<GPU_group, VGPU_type>> FindGpuGroupAndVgpuType(Session xenSession, VirtualHardwareSection_Type system)
        {
            List<Tuple<GPU_group, VGPU_type>> vgpus = new List<Tuple<GPU_group, VGPU_type>>();

            var data = system.VirtualSystemOtherConfigurationData;
            if (data == null)
                return vgpus;

            var datum = data.Where(s => s.Name == "vgpu");

            foreach (var item in datum)
            {
                Match m = VGPU_REGEX.Match(item.Value.Value);
                if (!m.Success)
                    continue;
                var types = m.Groups[1].Value.Split(';');

                var gpuGroups = GPU_group.get_all_records(xenSession);
                var gpuKvp = gpuGroups.FirstOrDefault(g =>
                    g.Value.supported_VGPU_types.Count > 0 &&
                    g.Value.GPU_types.Length == types.Length &&
                    g.Value.GPU_types.Intersect(types).Count() == types.Length);

                if (gpuKvp.Equals(default(KeyValuePair<XenRef<GPU_group>, GPU_group>)))
                    continue;

                var gpuGroup = gpuKvp.Value;
                VGPU_type vgpuType = null;

                gpuGroup.opaque_ref = gpuKvp.Key.opaque_ref;

                string vendorName = m.Groups[2].Value;
                string modelName = m.Groups[3].Value;

                var vgpuTypes = VGPU_type.get_all_records(xenSession);
                var vgpuKey = vgpuTypes.FirstOrDefault(v =>
                    v.Value.vendor_name == vendorName && v.Value.model_name == modelName);

                if (!vgpuKey.Equals(default(KeyValuePair<XenRef<VGPU_type>, VGPU_type>)))
                {
                    vgpuType = vgpuKey.Value;
                    vgpuType.opaque_ref = vgpuKey.Key.opaque_ref;
                }
                vgpus.Add(new Tuple<GPU_group, VGPU_type>(gpuGroup, vgpuType));
            }
            return vgpus;
        }

        private PVS_site FindPvsSite(Session xenSession, VirtualHardwareSection_Type system)
        {
            var data = system.VirtualSystemOtherConfigurationData;
            if (data == null)
                return null;

            var datum = data.FirstOrDefault(s => s.Name == "pvssite");
            if (datum == null)
                return null;

            Match m = PVS_SITE_REGEX.Match(datum.Value.Value);
            if (!m.Success)
                return null;

            var siteUuid = m.Groups[1].Value;

            var allSites = PVS_site.get_all_records(xenSession);

            var site = allSites.Select(kvp => kvp.Value).FirstOrDefault(p => p.uuid == siteUuid);

            return site;
        }

        private void RemoveSystem(Session xenSession, XenRef<VM> vm)
        {
            try
            {
                VM.destroy(xenSession, vm);
            }
            catch (Exception ex)
            {
                log.Error("Failed to remove a virtual machine (VM). ", ex);
                throw new Exception(Messages.ERROR_REMOVE_VM_FAILED, ex);
            }
        }

        private static Dictionary<string, string> SplitStringIntoDictionary(string inputStr)
        {
			/* Comment for the usage o.Split(new[] { '=' }, 2)
			 * The second parameter "2" is used to handle the case when the delimiter '=' appears in the content of the string
			 *
			 * For example, inputStr = "EFI-variables-backend=xapidb;EFI-variabbles-on-boot=reset;EFI-variables=dGVzdA=="
			 * Notice there are 2 extra '=' in the last section
			 * The expected output of that section should be { 'EFI-variables':'dGVzdA==' }
			 * But if we do not the second parameter "2" in the Split function, the actual output will be { 'EFI-variables':'' }
			 */
			return inputStr.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Split(new[] { '=' }, 2))
				.ToDictionary(o => o.FirstOrDefault(), o => o.LastOrDefault());
        }

        private Dictionary<string, string> MakePlatformHash(string platform)
        {
            var hPlatform = SplitStringIntoDictionary(platform);

            // Handle the case when NX isn't in the platform string.
            if (!hPlatform.ContainsKey("nx"))
                hPlatform.Add("nx", "true");

            return hPlatform;
        }

        private void AddResourceSettingData(Session xenSession, XenRef<VM> vmRef, RASD_Type rasd, string pathToOvf, string filename, string compression, string version, string passcode)
        {
            switch (rasd.ResourceType.Value)
            {
                case 3: // Processor: Already set in DefineSystem
                case 4: // Memory: Already set in DefineSystem
                case 5: // Internal Disk Controller of one type or another.
                case 6:
                case 7:
                case 8:
                case 9:
                    {
                        // For Xen really nothing to do here, does not support the different
                        // controller types, therefore we must ensure
                        // via positional on controllers.
                        // IDE - #1
                        // SCSI - #2
                        // IDE 0 Disk  0 Goes to Xen: userdevice=0
                        // IDE 0 Disk  1 Goes to Xen: userdevice=1 
                        // IDE 1 Disk  0 Goes to Xen: userdevice=2
                        // IDE 1 CDDVD 1 Goes to Xen: userdevice=3
                        // SCSI 0 Disk 0 Goes to Xen: userdevice=4
                        // SCSI 0 Disk 1 Goes to Xen: userdevice=5
                        // and so forth.
                        break;
                    }
                case 10: // Network
                    {
                        XenRef<Network> net = null;
                        XenRef<Network> netDefault = null;
                        string netuuid = null;

                        #region SELECT NETWORK
                        Dictionary<XenRef<Network>, Network> networks = Network.get_all_records(xenSession);
                        if (rasd.Connection != null && rasd.Connection.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(rasd.Connection[0].Value))
                            {
                                // Ignore the NetworkSection/Network
                                // During Network Selection the UUID for Network was set in Connection Field
                                // Makes data self contained here.

                                if (rasd.Connection[0].Value.Contains(Properties.Settings.Default.xenNetworkKey) ||
                                    rasd.Connection[0].Value.Contains(Properties.Settings.Default.xenNetworkUuidKey))
                                {
                                    string[] s = rasd.Connection[0].Value.Split(',');
                                    for (int i = 0; i < s.Length; i++)
                                    {
                                        if (s[i].StartsWith(Properties.Settings.Default.xenNetworkKey) ||
                                            s[i].StartsWith(Properties.Settings.Default.xenNetworkUuidKey))
                                        {
                                            string[] s1 = s[i].Split('=');
                                            netuuid = s1[1];
                                        }
                                    }
                                }
                                foreach (XenRef<Network> netRef in networks.Keys)
                                {
                                    // if its a UUID and we find it... use it..
                                    if (net == null && netuuid != null && 
                                        netuuid.Equals(networks[netRef].uuid))
                                    {
                                        net = netRef;
                                    }
                                    // Ok second is to match it as a NAME_LABEL
                                    else if (net == null && netuuid != null && 
                                        networks[netRef].name_label.ToLower().Contains(netuuid))
                                    {
                                        net = netRef;
                                    }
                                    // hhmm neither... is it a BRIDGE name?
                                    else if (net == null && netuuid != null && 
                                        networks[netRef].bridge.ToLower().Contains(netuuid))
                                    {
                                        net = netRef;
                                    }
                                    // ok find the default.
                                    if (networks[netRef].bridge.ToLower().Contains(Properties.Settings.Default.xenDefaultNetwork))
                                    {
                                        netDefault = netRef;
                                    }
                                }
                                if (net == null)
                                {
                                    net = netDefault;
                                }
                            }
                        }
                        #endregion

                        #region ATTACH NETWORK TO VM

                        VIF vif = new VIF
                        {
                            uuid = Guid.NewGuid().ToString(),
                            allowed_operations = new List<vif_operations> {vif_operations.attach},
                            device = Convert.ToString(vifDeviceIndex++),
                            network = net,
                            VM = vmRef,
                            MTU = 1500,
                            locking_mode = vif_locking_mode.network_default
                        };

                        // This is MAC address if available use it.
                        // needs to be in form:  00:00:00:00:00:00
                        if (Tools.ValidateProperty("Address", rasd))
                        {
                            StringBuilder networkAddress = new StringBuilder();
                            if (!rasd.Address.Value.Contains(":"))
                            {
                                for (int i = 0; i < rasd.Address.Value.Length; i++)
                                {
                                    if ((i > 0) && (i % 2) == 0)
                                    {
                                        networkAddress.Append(":");
                                    }
                                    networkAddress.Append(rasd.Address.Value[i]);
                                }
                            }
                            if (networkAddress.Length == 0)
                            {
                                networkAddress.Append(rasd.Address.Value);
                            }
                            vif.MAC = networkAddress.ToString();
                        }

                        try
                        {
                            xenSession.Connection.WaitForCache(VIF.create(xenSession, vif));
                        }
                        catch (Exception ex)
                        {
                            log.Error("Failed to create a virtual network interface (VIF). ", ex);
                            throw new Exception(Messages.ERROR_CREATE_VIF_FAILED, ex);
                        }
                        #endregion
                        log.Debug("OVF.Import.AddResourceSettingData: Network Added");

                        break;
                    }
                case 15: // CD Drive
                case 16: // DVD Drive
                    {
                        // We always attach as "EMPTY".
                        // Currently Xen Server can only have ONE CD, so we must 
                        // Skip the others.
                        // If it's not necessary.. skip it.

                        #region Attach DVD to VM
                        bool SkipCD = false;
                        List<XenRef<VBD>> vbds = VM.get_VBDs(xenSession, vmRef);
                        foreach (XenRef<VBD> vbd in vbds)
                        {
                            vbd_type vbdType = VBD.get_type(xenSession, vbd);
                            if (vbdType == vbd_type.CD)
                            {
                                SkipCD = true;
                                break;
                            }
                        }

                        if (!SkipCD)
                        {
                            List<XenRef<VDI>> vdiRefs = new List<XenRef<VDI>>();
                            if (filename != null)
                            {
                                #region FIND THE ISO SR MAPPED IN THE OVF
                                string isoUuid = null;
                                if (rasd.Connection != null && rasd.Connection.Length > 0)
                                {
                                    if (rasd.Connection[0].Value.ToLower().Contains("sr="))
                                    {
                                        string[] vpairs = rasd.Connection[0].Value.Split(',');
                                        foreach (string vset in vpairs)
                                        {
                                            if (vset.ToLower().StartsWith("sr="))
                                            {
                                                var srToFind = vset.Substring(vset.LastIndexOf('=') + 1);

                                                try
                                                {
                                                    XenRef<SR> srRef = SR.get_by_uuid(xenSession, srToFind);
                                                    if (srRef != null && srRef != Helper.NullOpaqueRef)
                                                    {
                                                        isoUuid = srToFind;
                                                        break;
                                                    }
                                                }
                                                catch
                                                {
                                                    log.Debug("Import.AddResourceSettingData: iso sr uuid not found, trying name_label");
                                                }

                                                try
                                                {
                                                    var srRecords = SR.get_all_records(xenSession);

                                                    isoUuid = (from SR sr in srRecords.Values
                                                        where sr.name_label == srToFind
                                                        select sr.uuid).FirstOrDefault();
                                                }
                                                catch
                                                {
                                                    log.Debug("Import.AddResourceSettingData: iso sr uuid not found, looking for vdi...");
                                                }

                                                break;
                                            }
                                        }
                                    }
                                }
                                #endregion

                                // VDI trumps SR
                                List<XenRef<VDI>> isoVDIlist = VDI.get_by_name_label(xenSession, filename);
                                if (isoVDIlist.Count > 0)
                                {
                                    vdiRefs.Add(isoVDIlist[0]);
                                }
                                else
                                {
                                    #region LAST CHANCE USE XENTOOLS ISO SR
                                    if (isoUuid == null)
                                    {
                                        Dictionary<XenRef<SR>, SR> srDictionary = SR.get_all_records(xenSession);
                                        foreach (XenRef<SR> key in srDictionary.Keys)
                                        {
                                            if (srDictionary[key].content_type.ToLower() == "iso" && srDictionary[key].type.ToLower() == "iso")
                                            {
                                                if (srDictionary[key].name_label.ToLower().Equals(Properties.Settings.Default.xenTools.ToLower()))
                                                {
                                                    isoUuid = srDictionary[key].uuid;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region DO IMPORT ISO FILE
                                    if (isoUuid != null && !MetaDataOnly)
                                    {
                                        _currentfilename = filename;
                                        try
                                        {
                                            vdiRefs = ImportFile(xenSession, filename, pathToOvf, filename, compression, version, passcode, isoUuid, "", null);
                                        }
                                        catch (Exception ex)
                                        {
                                            if (ex is OperationCanceledException)
                                                throw;
                                            var msg = string.Format(Messages.ERROR_ADDRESOURCESETTINGDATA_FAILED, Messages.ISO);
                                            log.Error("Failed to add resource ISO.", ex);
                                            throw new Exception(msg, ex);
                                        }
                                        finally
                                        {
                                            if (vdiRefs == null || vdiRefs.Count <= 0)
                                            {
                                                log.ErrorFormat("Failed to import virtual disk from file {0} to storage repository {1}.", filename, isoUuid);
                                                RemoveSystem(xenSession, vmRef);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                vdiRefs.Add(XenRef<VDI>.Create(string.Empty));
                            }

                            #region CREATE VBD CONNECTION
                         
                            foreach (XenRef<VDI> currentVDI in vdiRefs)
                            {
                                var hashtable = new Hashtable();

                                if (rasd.Connection != null && rasd.Connection.Length > 0)
                                {
                                    string[] valuepairs = rasd.Connection[0].Value.Split(',');

                                    foreach (string valuepair in valuepairs)
                                    {
                                        string[] namevalue = valuepair.Split('=');
                                        string name = namevalue[0].ToLower();

                                        switch (name)
                                        {
                                            case "sr":
                                            case "vdi":
                                                continue;
                                            case "device":
                                                hashtable.Add("userdevice", namevalue[1]);
                                                break;
                                            default:
                                                hashtable.Add(namevalue[0], namevalue[1]);
                                                break;
                                        }
                                    }
                                }

                                VBD vbd = new VBD
                                {
                                    VM = vmRef,
                                    mode = vbd_mode.RO,
                                    userdevice = "3",
                                    type = vbd_type.CD,
                                    storage_lock = false,
                                    status_code = 0
                                };

                                vbd.UpdateFrom(hashtable);

                                if (currentVDI != null && !string.IsNullOrEmpty(currentVDI.opaque_ref))
                                {
                                    vbd.VDI = currentVDI;
                                    vbd.empty = false;
                                    vbd.bootable = true;
                                    vbd.unpluggable = true;
                                }
                                else
                                {
                                    vbd.empty = true;
                                }

                                vbd.userdevice = VerifyUserDevice(xenSession, vmRef, vbd.userdevice);
                                vbd.other_config = new Dictionary<string, string> { { "owner", "true" } };

                                if (!vbd.userdevice.EndsWith("+"))
                                {
                                    try
                                    {
                                        VBD.create(xenSession, vbd);
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error("Import.AddResourceSettingData: ", ex);
                                    }
                                }
                                else
                                {
                                    log.WarnFormat(
                                        "Import:  ================== ATTENTION NEEDED =======================" +
                                        "Import:  Could not determine appropriate number of device placement." +
                                        "Import:  Please Start, Logon, Shut down, System ({0})" +
                                        "Import:  Then attach disks with labels ending with \"+\" to the device number defined before the +." +
                                        "Import:  ===========================================================", vmRef);

                                    OnUpdate(new XenOvfTransportEventArgs(TransportStep.Import, Messages.WARNING_ADMIN_REQUIRED));
                                }
                            }
                            #endregion

                        }
                        #endregion
                        OnUpdate(new XenOvfTransportEventArgs(TransportStep.CdDvdDrive,
							string.Format(Messages.DEVICE_ATTACHED, Messages.CD_DVD_DEVICE)));
                        log.Debug("Import.AddResourceSettingData: CD/DVD ROM Added");

                        break;
                    }
                case 17: // Disk Drive
                case 19: // Storage Extent
                case 21: // Microsoft: Harddisk/Floppy/ISO
                    {
                        #region ADD DISK
                        if (filename == null) // NO disk is available, why import RASD?
                        {
                            log.WarnFormat("No file available to import, skipping: RASD{0}: {1}", rasd.ResourceType.Value, rasd.InstanceID.Value);
                            break;
                        }
                        string sruuid = null;
                        string vdiuuid = null;
                        string userdeviceid = null;
                        string vmName = VM.get_name_label(xenSession, vmRef);
                        bool isbootable = false;
                        vbd_mode mode = vbd_mode.RW;

                        bool importThisRasd = true;
                        if (Tools.ValidateProperty("Caption", rasd)) // rasd.Caption != null && rasd.Caption.Value != null && rasd.Caption.Value.Length > 0)
                        {
                            if (
                                rasd.Caption.Value.ToUpper().Contains("COM") ||
                                rasd.Caption.Value.ToUpper().Contains("FLOPPY") ||
                                rasd.Caption.Value.ToUpper().Contains("ISO")
                                )
                            {
                                importThisRasd = false;
                            }
                        }

                        if (importThisRasd)
                        {
                            #region IMPORT DISKS
                            if (!MetaDataOnly)
                            {
                                _currentfilename = filename;

                                List<XenRef<VDI>> vdiRef = null;

                                #region PARSE CONNECTION
                                if (Tools.ValidateProperty("Connection", rasd))
                                {
                                    string[] s = rasd.Connection[0].Value.Split('=', ',');
                                    for (int i = 0; i < s.Length; i++)
                                    {
                                        string checkme = s[i].ToLower().Trim();
                                        switch (checkme)
                                        {
                                            case "device":
                                                {
                                                    userdeviceid = s[++i];
                                                    break;
                                                }
                                            case "bootable":
                                                {
                                                    isbootable = Convert.ToBoolean(s[++i]);
                                                    break;
                                                }
                                            case "mode":
                                                {
                                                    if (s[++i].Equals("r"))
                                                    {
                                                        mode = vbd_mode.RO;
                                                    }
                                                    break;
                                                }
                                            case "vdi":
                                                {
                                                    vdiuuid = s[++i];
                                                    break;
                                                }
                                            case "sr":
                                                {
                                                    sruuid = s[++i];
                                                    break;
                                                }
                                        }
                                    }
                                }
                                #endregion

                                #region VERIFY SR UUID
                                if (!string.IsNullOrEmpty(sruuid))
                                {
                                    XenRef<SR> srref = null;
                                    try
                                    {
                                        srref = SR.get_by_uuid(xenSession, sruuid);
                                    }
                                    catch
                                    {
                                        log.Debug("Import.AddResourceSettingData: SR missing... still looking..");
                                    }
                                    if (srref == null)
                                    {
                                        List<XenRef<SR>> srlist = null;
                                        try
                                        {
                                            srlist = SR.get_by_name_label(xenSession, sruuid);
                                        }
                                        catch
                                        {
                                            log.Debug("Import.AddResourceSettingData: SR missing... still looking..");
                                        }
                                        if (srlist != null && srlist.Count > 0)
                                        {
                                            sruuid = SR.get_uuid(xenSession, srlist[0]);
                                        }
                                    }
                                }
                                else
                                {
                                    sruuid = null;
                                }
                                #endregion

                                #region LAST CHANGE TO FIND SR
                                if (sruuid == null)
                                {
                                    if (DefaultSRUUID == null)
                                    {
                                        log.Error("The SR was not found and a default was not assigned.");
                                        throw new InvalidDataException(Messages.ERROR_COULD_NOT_FIND_SR);
                                    }

                                    Dictionary<XenRef<SR>, SR> srDict = SR.get_all_records(xenSession);
                                    if (vdiuuid != null)
                                    {
                                        //Try and get the SR that belongs to the VDI attached
                                        XenRef<VDI> tempVDI = VDI.get_by_uuid(xenSession, vdiuuid);
                                        if (tempVDI == null)
                                        {
                                            log.Error("The SR was not found and a default was not assigned.");
                                            throw new InvalidDataException(Messages.ERROR_COULD_NOT_FIND_SR);
                                        }

                                        XenRef<SR> tempSR = VDI.get_SR(xenSession, tempVDI.opaque_ref);
                                        sruuid = srDict[tempSR].uuid;
                                    }
                                    else
                                        sruuid = srDict[DefaultSRUUID].uuid;
                                }
                                #endregion

                                try
                                {
                                    string disklabel = string.Format("{0}_{1}", vmName, userdeviceid);

                                    if ((rasd.ElementName != null) && (!string.IsNullOrEmpty(rasd.ElementName.Value)))
                                        disklabel = rasd.ElementName.Value;

                                    string description = "";

                                    if ((rasd.Description != null) && (!string.IsNullOrEmpty(rasd.Description.Value)))
                                        description = rasd.Description.Value;

                                    vdiRef = ImportFile(xenSession, disklabel, pathToOvf, filename, compression, version, passcode, sruuid, description, vdiuuid);
                                }
                                catch (Exception ex)
                                {
									if (ex is OperationCanceledException)
										throw;
									var msg = string.Format(Messages.ERROR_ADDRESOURCESETTINGDATA_FAILED, Messages.DISK_DEVICE);
                                    log.Error("Failed to add resource Hard Disk Image.", ex);
                                    throw new InvalidDataException(msg, ex);
                                }
                                finally
                                {
                                    if (vdiRef == null)
                                    {
                                    	log.ErrorFormat("Failed to import virtual disk from file {0} to storage repository {1}.", filename, sruuid);
                                        RemoveSystem(xenSession, vmRef);
                                    }
                                }

                                log.DebugFormat("Import.AddResourceSettingData counts {0} VDIs", vdiRef.Count);


                                foreach (XenRef<VDI> currentVDI in vdiRef)
                                {
                                    VBD vbd = new VBD
                                    {
                                        userdevice = VerifyUserDevice(xenSession, vmRef, userdeviceid ?? "99"),
                                        bootable = isbootable,
                                        VDI = currentVDI,
                                        mode = mode,
                                        uuid = Guid.NewGuid().ToString(),
                                        VM = vmRef,
                                        empty = false,
                                        type = vbd_type.Disk,
                                        currently_attached = false,
                                        storage_lock = false,
                                        status_code = 0,
                                        // below other_config keys XS to delete the disk along with the VM.
                                        other_config = new Dictionary<string, string> {{"owner", "true"}}
                                    };

                                    if (!vbd.userdevice.EndsWith("+"))
                                    {
                                        try
                                        {
                                            VBD.create(xenSession, vbd);
                                        }
                                        catch (Exception ex)
                                        {
                                            log.Error("Failed to create a virtual block device (VBD).", ex);
                                            throw new Exception(Messages.ERROR_CREATE_VBD_FAILED, ex);
                                        }
                                    }
                                    else
                                    {
                                        log.WarnFormat(
                                            "Import:  ================== ATTENTION NEEDED =======================" +
                                            "Import:  Could not determine appropriate number for device placement." +
                                            "Import:  Please Start, Logon, Shut down, System ({0})" +
                                            "Import:  Then manually attach disks with labels with {0}_# that are not attached to {0}" +
                                            "Import:  ===========================================================",
                                            vmName);
                                        OnUpdate(new XenOvfTransportEventArgs(TransportStep.Import, Messages.WARNING_ADMIN_REQUIRED));
                                    }
                                }
                            }
                            else
                            {
                                log.InfoFormat("Import: FILE SKIPPED (METADATA ONLY SELECTED)  {0}", _currentfilename);
                            }
                            #endregion
                        }
                        log.Debug("Import.AddResourceSettingData: Hard Disk Image Added");
                        break;
                        #endregion
                    }
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
                                                         Justification = "Logging mechanism")]
        private string VerifyUserDevice(Session xenSession, XenRef<VM> vmRef, string device)
        {
            log.DebugFormat("Import.VerifyUserDevice, checking device: {0} (99 = autoselect)", device);
            string usethisdevice = null;

            string[] allowedVBDs = VM.get_allowed_VBD_devices(xenSession, vmRef);

            if (allowedVBDs == null || allowedVBDs.Length <= 0)
            {
                log.ErrorFormat("OVF.VerifyUserDevice: No more available devices, cannot add device: {0}", device);
                return device + "+";
            }

            if (!string.IsNullOrEmpty(device) && !device.StartsWith("99"))
            {
                foreach (string allowedvbd in allowedVBDs)
                {
                    if (device.ToLower() == allowedvbd.ToLower())
                    {
                        usethisdevice = device;
                        log.DebugFormat("Import.VerifyUserDevice, device: {0} will be used.", device);
                        break;
                    }
                }
            }
            else
            {
                usethisdevice = allowedVBDs[0];
                log.DebugFormat("Import.VerifyUserDevice, device [{0}] is not available, setting to: [{1}]", device, usethisdevice);
            }

            if (usethisdevice == null)
            {
                if (!device.EndsWith("+"))
                    usethisdevice = device + "+";                
            }
            return usethisdevice;
        }
        private void SetDeviceConnections(EnvelopeType ovfEnv, VirtualHardwareSection_Type vhs)
        {
            int[] connections = new int[16];
            int deviceoffset = 0;
            List<RASD_Type> rasdList = new List<RASD_Type>();
            
            rasdList.AddRange(vhs.Item);
            rasdList.Sort(compareControllerRasd);  // sorts based on ResourceType.Value

            // For Xen really nothing to do here, does not support the different
            // controller types, therefore we must ensure
            // via positional on controllers.
            // IDE - #1
            // SCSI - #2
            // IDE 0 Disk 0  Goes to Xen: userdevice=0
            // IDE 0 Disk 1  Goes to Xen: userdevice=1 
            // IDE 1 CD/DVD 0  Goes to Xen: userdevice=2
            // IDE 1 Disk 1  UnUsed
            // SCSI 0 Disk 0 Goes to Xen: userdevice=3
            // SCSI 0 Disk 1 Goes to Xen: userdevice=4
            // and so forth.

            foreach (RASD_Type rasd in rasdList)
            {
                switch (rasd.ResourceType.Value)
                {
                    case 5:  // IDE Controller #1
                    case 6:  // Parallel SCSI HBA #2
                    case 7:  // FC HBA #3
                    case 8:  // iSCSI HBA #4
                    case 9:  // IB HCA #5
                        {
                            List<RASD_Type> connectedrasds = FindConnectedItems(rasd.InstanceID.Value, vhs.Item, null);
                            foreach (RASD_Type _rasd in connectedrasds)
                            {
                                //if (_rasd.Connection != null &&
                                //    _rasd.Connection.Length > 0 &&
                                //    _rasd.Connection[0] != null &&
                                //    _rasd.Connection[0].Value != null &&
                                //    _rasd.Connection[0].Value.Length > 0)
                                if (_rasd.ResourceType.Value == 15 || _rasd.ResourceType.Value == 16)
                                {
                                    deviceoffset = 2;
                                }
                                else
                                {
                                    deviceoffset = 0;
                                }
                                if (Tools.ValidateProperty("Connection", _rasd))
                                {
                                    if (!_rasd.Connection[0].Value.ToLower().Contains("device="))
                                    {
                                       _rasd.Connection[0].Value = string.Format("{0},device={1}", _rasd.Connection[0].Value, FindNextAvailable(deviceoffset, connections, 0));
                                    }
                                }
                                else
                                {
                                    _rasd.Connection = new cimString[] { new cimString(string.Format("device={0}", FindNextAvailable(deviceoffset, connections, 0))) };
                                }
                            }
                            break;
                        }
                }
            }
        }
        private int FindNextAvailable(int offset, int[] ids, int unusedkey)
        {
            int available = 0;
            for (int i = offset; i < ids.Length; i++)
            {
                if (ids[i] == unusedkey)
                {
                    ids[i] = 1;
                    available = i;
                    break;
                }
            }
            return available;
        }
        private List<RASD_Type> FindConnectedItems(string instanceId, RASD_Type[] rasds, string value22)
        {
            List<RASD_Type> connectedRasds = new List<RASD_Type>();
            foreach (RASD_Type rasd in rasds)
            {
                if (rasd.Parent != null && !string.IsNullOrEmpty(rasd.Parent.Value) )
                {
                    string parent = rasd.Parent.Value.Replace(@"\", "");
                    string instance = instanceId.Replace(@"\", "");
                    if (parent.Contains(instance))
                    {
                        switch (rasd.ResourceType.Value)
                        {
                            case 15:
                            case 16:
                                {
                                    connectedRasds.Add(rasd);
                                    break;
                                }
                            case 22: // Check to see if it's Microsoft Synthetic Disk Drive
                                {
                                    if (Tools.ValidateProperty("ResourceSubType", rasd) && 
                                        rasd.ResourceSubType.Value.ToLower().Contains("synthetic")
                                        )
                                    {
                                        connectedRasds.AddRange(FindConnectedItems(rasd.InstanceID.Value, rasds, rasd.Address.Value));
                                    }
                                    break;
                                }
                            case 17: // VMware Hard Disk
                            case 19: // XenServer/XenConvert Storage Extent
                            case 21: // Microsoft Hard Disk Image
                                {
                                    if ((Tools.ValidateProperty("ElementName", rasd) && rasd.ElementName.Value.ToLower().Contains("hard disk")) ||
                                        (Tools.ValidateProperty("Caption", rasd) && rasd.Caption.Value.ToLower().Contains("hard disk")) ||
                                        (Tools.ValidateProperty("Caption", rasd) && rasd.Caption.Value.ToLower().StartsWith("disk"))
                                        )
                                    {
                                        if (value22 != null)
                                        {
                                            rasd.Address = new cimString(value22);
                                        }
                                        if (!connectedRasds.Contains(rasd))
                                            connectedRasds.Add(rasd);
                                    }
                                    break;
                                }
                        }
                    }
                }
            }
            Comparison<RASD_Type> diskcomparison = new Comparison<RASD_Type>(compareConnectedDisks);
            connectedRasds.Sort(diskcomparison);
            return connectedRasds;
        }
        private void SetIfDeviceIsBootable(EnvelopeType ovfEnv, RASD_Type rasd)
        {
            // This is a best guess algorithm. without opening the VHD itself, there is no guarrenteed method
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
                return;

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
            
            foreach(VirtualDiskDesc_Type disk in disks)
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
                if (!rasd.Connection[0].Value.Contains("bootable"))
                {
                    rasd.Connection[0].Value = string.Format("{0},bootable={1}", rasd.Connection[0].Value, isBootable);
                }
            }
            else
            {
                rasd.Connection = new cimString[] { new cimString(string.Format("bootable={0}", isBootable)) };
            }
        }
        private XenRef<VDI> CheckForISOVDI(Session xenSession, string filename)
        {
            XenRef<VDI> vdiref = null;

            Dictionary<XenRef<VDI>, VDI> vdidict = VDI.get_all_records(xenSession);

            foreach (XenRef<VDI> key in vdidict.Keys)
            {
                if (vdidict[key].name_label.ToLower().Equals(filename.ToLower()))
                {
                    vdiref = key;
                    break;
                }
            }

            return vdiref;
        }

        public bool IsKnownURIType(string filename)
        {
            string expression = Properties.Settings.Default.uriRegex;
            RegexStringValidator rsv = new RegexStringValidator(expression);
            if (rsv.CanValidate(filename.GetType()))
            {
                try
                {
                    rsv.Validate(filename);
                    log.InfoFormat("Import.isURI: File {0} is in URI format.", filename);
                    return true;
                }
                catch
                {
                    log.InfoFormat("Import.isURI: File {0} is not in URI format.", filename);
                }
            }
            return false;
        }
        
        private void HideSystem(Session xenSession, XenRef<VM> vmRef)
        {
            VM.add_to_other_config(xenSession, vmRef, "HideFromXenCenter", "true");
        }
        private void ShowSystem(Session xenSession, XenRef<VM> vmRef)
        {
            VM.remove_from_other_config(xenSession, vmRef, "HideFromXenCenter");
        }
        
        private byte[] ExtractMBRFromVHD(string referenceVHD)
        {
            byte[] mbr = null;
            using (VirtualDisk vhdx = DiscUtils.Vhd.Disk.OpenDisk(referenceVHD, FileAccess.Read))
            {
                mbr = vhdx.GetMasterBootRecord();
            }
            return mbr;
        }
        private byte[] ExtractBootFromVHD(string referenceVHD)
        {
            return ExtractFileFromVHD(@"$Boot", referenceVHD);
        }
        private byte[] ExtractBCDFromVHD(string referenceVHD, int arch)
        {
            string architecture = "x86";
            if (arch == 0) { architecture = "x86"; }
            else if (arch == 9) { architecture = "x64"; }
            string filename = string.Format("{0}{1}", Path.DirectorySeparatorChar, Path.Combine(architecture, "BCD"));
            return ExtractFileFromVHD(filename, referenceVHD);
        }
        private byte[] ExtractBootmgrFromVHD(string referenceVHD, int arch)
        {
            string architecture = "x86";
            if (arch == 0)  { architecture = "x86"; }
            else if (arch == 9) { architecture = "x64"; }
            string filename = string.Format("{0}{1}", Path.DirectorySeparatorChar, Path.Combine(architecture, "bootmgr"));
            return ExtractFileFromVHD(filename, referenceVHD);
        }
        private byte[] ExtractFileFromVHD(string filename, string referenceVHD)
        {
            byte[] file = null;
            using (VirtualDisk vhdx = DiscUtils.Vhd.Disk.OpenDisk(referenceVHD, FileAccess.Read))
            {
                NtfsFileSystem vhdbNtfs = new NtfsFileSystem(vhdx.Partitions[0].Open());
                using (Stream bootStream = vhdbNtfs.OpenFile(filename, FileMode.Open, FileAccess.Read))
                {
                    file = new byte[bootStream.Length];
                    int totalRead = 0;
                    while (totalRead < file.Length)
                    {
                        totalRead += bootStream.Read(file, totalRead, file.Length - totalRead);
                    }
                }
            }
            return file;
        }


        private void AddBCD(NtfsFileSystem ntfs, byte[] BCD)
        {
            if (!ntfs.DirectoryExists(@"\boot"))
            {
                ntfs.CreateDirectory(@"\boot");
            }
            if (!ntfs.FileExists(@"\boot\BCD"))
            {
                using (Stream bcdStream = ntfs.OpenFile(@"\boot\BCD", FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    bcdStream.Write(BCD, 0, BCD.Length);
                }
            }
        }
        private void AddBootMgr(NtfsFileSystem ntfs, byte[] BootMgr)
        {
            if (!ntfs.FileExists(@"\bootmgr"))
            {
                using (Stream bcdStream = ntfs.OpenFile(@"\bootmgr", FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    bcdStream.Write(BootMgr, 0, BootMgr.Length);
                }
            }
        }

        private void FixBCD(NtfsFileSystem ntfs, VolumeManager volMgr)
        {
            if (ntfs.FileExists(@"\boot\BCD"))
            {
                // Force all boot entries in the BCD to point to the newly created NTFS partition - does _not_ cope with
                // complex multi-volume / multi-boot scenarios at all.
                using (Stream bcdStream = ntfs.OpenFile(@"\boot\BCD", FileMode.Open, FileAccess.ReadWrite))
                {
                    using (RegistryHive hive = new RegistryHive(bcdStream))
                    {
                        Store store = new Store(hive.Root);
                        foreach (var obj in store.Objects)
                        {
                            foreach (var elem in obj.Elements)
                            {
                                if (elem.Format == DiscUtils.BootConfig.ElementFormat.Device)
                                {
                                    elem.Value = DiscUtils.BootConfig.ElementValue.ForDevice(elem.Value.ParentObject, volMgr.GetPhysicalVolumes()[0]);
                                }
                            }
                        }
                    }
                }
            }
        }
        private string FindReferenceVHD(string referenceVHD)
        {
            string bzip2ext = Properties.Settings.Default.bzip2ext;
            string reffilename = Path.GetFileName(referenceVHD);
            string refVHD = null;
            string appdata = null;
            string datapath = null;

            appdata = System.Environment.GetEnvironmentVariable("APPDATA");
            if (string.IsNullOrEmpty(appdata))
            {
                appdata = System.Environment.GetEnvironmentVariable("ProgramData");
            }

            if (string.IsNullOrEmpty(appdata))
            {
                appdata = Path.Combine("C:", "Temp");
            }

            datapath = Path.Combine(appdata, Path.Combine("citrix", "temp"));

            if (!Directory.Exists(datapath))
                Directory.CreateDirectory(datapath);

            refVHD = Path.Combine(datapath, reffilename);
            if (!File.Exists(refVHD))
            {

                string apath = Assembly.GetExecutingAssembly().Location;
                string assempath = Path.GetDirectoryName(apath);
                while (assempath.Length > 3)
                {
                    string testPath = Path.Combine(assempath, referenceVHD);
                    if (File.Exists(testPath))
                    {
                        refVHD = testPath;
                        break;
                    }
                    else if (File.Exists(testPath + bzip2ext))
                    {
                        refVHD = testPath + bzip2ext;
                        break;
                    }
                    assempath = Path.GetDirectoryName(assempath);
                }

                if (Path.GetExtension(refVHD) == bzip2ext)
                {
                    string outfile = Path.Combine(datapath, Path.GetFileNameWithoutExtension(refVHD));
                    if (!File.Exists(outfile))
                    {
                        try
                        {
                            using (CompressionStream bzos = CompressionFactory.Writer(CompressionFactory.Type.Bz2, File.OpenWrite(outfile)))
                            {
                                bzos.BufferedWrite(File.OpenRead(refVHD));
                            }
                        }
                        finally { }
                    }
                    refVHD = outfile;
                    log.Info("A Compressed Reference VHD was found and uncompressed.");
                }
            }
            log.InfoFormat("Reference VHD: {0}", refVHD);
            return refVHD;
        }

    	public string DownloadFileAsync(Uri filetodownload, ulong totalsize)
        {
            log.InfoFormat("DownloadFileAsync: {0}", filetodownload);
            _downloadexception = null;
            string tmpfilename = filetodownload.AbsolutePath.Substring(filetodownload.AbsolutePath.LastIndexOf('/') + 1);
            if (!File.Exists(tmpfilename))
            {
                downloadupdatemsg = string.Format(Messages.ISCSI_COPY_PROGRESS, tmpfilename);
                _downloadexception = null;
                OnUpdate(new XenOvfTransportEventArgs(TransportStep.Download, downloadupdatemsg, 0, _filedownloadsize));
                WebClient wc = new WebClient();
                wc.Proxy = XenAdmin.XenAdminConfigManager.Provider.GetProxyFromSettings(null, false);
                wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(filetodownload, tmpfilename);
                uridownloadcomplete.WaitOne();
                if (_downloadexception != null)
                {
                    if (!Path.GetExtension(tmpfilename).Equals(".pvp"))  // don't worry bout pvp files, we don't use them.
                        throw _downloadexception;
                }
                OnUpdate(new XenOvfTransportEventArgs(TransportStep.Download, downloadupdatemsg, _filedownloadsize, _filedownloadsize));
            }
            return tmpfilename;
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            OnUpdate(new XenOvfTransportEventArgs(TransportStep.Download, downloadupdatemsg, (ulong)e.BytesReceived, (ulong)_filedownloadsize));
        }
        private void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                log.Error("DownloadFileAsync failure.", e.Error);
                _downloadexception = e.Error;
            }
            log.Info("DownloadFileAsync: completed");
            uridownloadcomplete.Set();
        }

        private static int comparePostInstallCommand(Xen_PostInstallOperationCommand_Type postOpLeft, Xen_PostInstallOperationCommand_Type postOpRight)
        {
            return postOpLeft.Order.CompareTo(postOpRight.Order);
        }
        private static int compareControllerRasd(RASD_Type rasd1, RASD_Type rasd2)
        {
            if (rasd1.ResourceType.Value >= 5 &&
                rasd1.ResourceType.Value <= 9 &&
                rasd2.ResourceType.Value >= 5 &&
                rasd2.ResourceType.Value <= 9 && 
                rasd1.Address != null &&
                rasd1.Address.Value != null &&
                rasd2.Address != null &&
                rasd2.Address.Value != null)
            {
                ushort address1 = Convert.ToUInt16(rasd1.Address.Value);
                ushort address2 = Convert.ToUInt16(rasd2.Address.Value);
                int left = (rasd1.ResourceType.Value * 10) + address1;
                int right = (rasd2.ResourceType.Value * 10) + address2;
                return (left).CompareTo(right);
            }
            else
            {
                return rasd1.ResourceType.Value.CompareTo(rasd2.ResourceType.Value);
            }
        }
        private static int compareConnectedDisks(RASD_Type rasd1, RASD_Type rasd2)
        {
            if (rasd1.AddressOnParent != null &&
                rasd1.AddressOnParent.Value != null &&
                rasd2.AddressOnParent != null &&
                rasd2.AddressOnParent.Value != null)
            {

                return (rasd1.AddressOnParent.Value).CompareTo(rasd2.AddressOnParent.Value);
            }
            else
            {
                if (rasd1.Address != null &&
                    rasd1.Address.Value != null &&
                    rasd2.Address != null &&
                    rasd2.Address.Value != null)
                {
                    ushort address1 = Convert.ToUInt16(rasd1.Address.Value);
                    ushort address2 = Convert.ToUInt16(rasd2.Address.Value);
                    return (address1).CompareTo(address2);
                }
                throw new ArgumentNullException("Cannot compare null values");
            }
        }
        #endregion

    }
}
