/* Copyright (c) Cloud Software Group, Inc. 
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

using DiscUtils;
using XenAdmin.Core;
using XenCenterLib.Compression;
using XenAPI;
using XenOvf;
using XenOvf.Definitions;
using XenOvf.Utilities;


namespace XenAdmin.Actions.OvfActions
{
    public partial class ImportApplianceAction
    {
        public static Regex VGPU_REGEX = new Regex("^GPU_types={(.*)};VGPU_type_vendor_name=(.*);VGPU_type_model_name=(.*);$");
        public static Regex PVS_SITE_REGEX = new Regex("^PVS_SITE={uuid=(.*)};$");

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected object Process(EnvelopeType ovfObj, string pathToOvf, string applianceName = null)
        {
            //Normalise the process
            if (ovfObj.Item is VirtualSystem_Type vstemp)
            {
				ovfObj.Item = new VirtualSystemCollection_Type();
				((VirtualSystemCollection_Type)ovfObj.Item).Content = new Content_Type[] { vstemp };
            }

			#region Create appliance

            CheckForCancellation();

            XenRef<VM_appliance> applRef = null;
			if (applianceName != null)
            {
                Description = string.Format(Messages.IMPORT_CREATING_APPLIANCE, applianceName);
                var vmAppliance = new VM_appliance {name_label = applianceName};
                var appliance = Connection.WaitForCache(VM_appliance.create(Connection.Session, vmAppliance));
                applRef = new XenRef<VM_appliance>(appliance.opaque_ref);
                log.Info($"Created appliance {applianceName} ({applRef.opaque_ref})");
			}

            StartupSection_TypeItem[] startupSections = null;
            if (ovfObj.Sections != null)
            {
                StartupSection_Type[] startUpArray = OVF.FindSections<StartupSection_Type>(ovfObj.Sections);
                if (startUpArray != null && startUpArray.Length > 0)
                    startupSections = startUpArray[0]?.Item;
            }

            #endregion

            var vmsToImport = ((VirtualSystemCollection_Type)ovfObj.Item).Content
                .Where(c => c is VirtualSystem_Type).Cast<VirtualSystem_Type>().ToList();

            XenRef<VM> vmRef = null;
            for (int i = 0; i < vmsToImport.Count; i++)
            {
                CheckForCancellation();

                int curVm = i;
                VirtualSystem_Type vSystem = vmsToImport[i];
                var vmName = OVF.FindSystemName(ovfObj, vSystem.id);
                VirtualHardwareSection_Type vhs = OVF.FindVirtualHardwareSectionByAffinity(ovfObj, vSystem.id, "xen");
                var vmStartupSection = startupSections?.FirstOrDefault(it => it.id == vSystem.id);

                vmRef = null;
                try
                {
                    vmRef = CreateVm(vmName, vhs, applRef, vmStartupSection);
                    SetDeviceConnections(vhs);
                    int vifDeviceIndex = 0;

                    for (int j = 0; j < vhs.Item.Length; j++)
                    {
                        CheckForCancellation();

                        int curVhs = j;
                        void UpdatePercentage(float x)
                        {
                            PercentComplete = (int)(20 + curVm * 80 / vmsToImport.Count + (curVhs + x) * 80 / (vmsToImport.Count * vhs.Item.Length));
                        }

                        AddResourceSettingData(ovfObj, vmRef, vhs.Item[j], pathToOvf, ref vifDeviceIndex, UpdatePercentage);
                        PercentComplete = 20 + i * 80 / vmsToImport.Count + (j + 1) * 80 / (vmsToImport.Count * vhs.Item.Length);
                    }

                    CreateVgpus(vhs, vmRef);
                    CreatePvsProxy(vhs, vmRef);

                    HandleInstallSection(Connection.Session, vmRef, vSystem);
                    VM.remove_from_other_config(Connection.Session, vmRef, "HideFromXenCenter");
                    PercentComplete = 20 + (i + 1) * 80 / vmsToImport.Count;
                }
                catch
                {
                    if (vmRef != null)
                    {
                        log.Info($"Import interrupted. Destroying VM {vmRef.opaque_ref}");
                        CleanUpVm(vmRef);
                    }

                    throw;
                }
            }

            if (applRef != null)
                return applRef;
            
            return vmRef;
        }

        private void HandleInstallSection(Session xenSession, XenRef<VM> vmRef, VirtualSystem_Type vSystem)
        {
            CheckForCancellation();

            InstallSection_Type[] installSections = OVF.FindSections<InstallSection_Type>(vSystem.Items);

            if (installSections == null || installSections.Length <= 0)
                return;
            
            Description = Messages.START_POST_INSTALL_INSTRUCTIONS;
            var installSection = installSections[0];

            // Configure for XenServer as requested by OVF.SetRunOnceBootCDROM() with the presence of a post install operation that is specific to XenServer.
            if (installSection.PostInstallOperations != null)
                ConfigureForXenServer(xenSession, vmRef);

            // Run the VM for the requested duration if this appliance had its own install section -- one not added to fixup for XenServer.
            if (installSection.Info == null ||
                installSection.Info != null && installSection.Info.Value.CompareTo("ConfigureForXenServer") != 0)
                InstallSectionStartVirtualMachine(xenSession, vmRef, installSection.initialBootStopDelay);
        }

        private static void ConfigureForXenServer(Session xenSession, XenRef<VM> vm)
        {
            // Ensure the new VM is down.
            if (VM.get_power_state(xenSession, vm) != vm_power_state.Halted)
                VM.hard_shutdown(xenSession, vm);

            while (VM.get_power_state(xenSession, vm) != vm_power_state.Halted)
                Thread.Sleep(1000);

            // Save its original memory configuration.
            long staticMemoryMin  = VM.get_memory_static_min(xenSession, vm);
            long staticMemoryMax  = VM.get_memory_static_max(xenSession, vm);
            long dynamicMemoryMin = VM.get_memory_dynamic_min(xenSession, vm);
            long dynamicMemoryMax = VM.get_memory_dynamic_max(xenSession, vm);

            // Minimize the memory capacity for the fixup OS.
            long fixupMemorySize = 256 * Util.BINARY_MEGA;

            VM.set_memory_limits(xenSession, vm, fixupMemorySize, fixupMemorySize, fixupMemorySize, fixupMemorySize);

            // Run the fixup OS on the VM.
            InstallSectionStartVirtualMachine(xenSession, vm, 600);

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

        private static void InstallSectionStartVirtualMachine(Session xenSession, XenRef<VM> vm, int initialBootStopDelayAsSeconds)
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
                Thread.Sleep(1000);

            // Let it run for the requested duration.
            int bootStopDelayAsMs = initialBootStopDelayAsSeconds * 1000;
            int msRunning = 0;

            while (VM.get_power_state(xenSession, vm) == vm_power_state.Running)
            {
                Thread.Sleep(1000);
                msRunning += 1000;

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

        private XenRef<VDI> ImportFile(string diskName, string pathToOvf, string filename, CompressionFactory.Type? compression, string sruuid, string description, string vdiuuid, bool isEncrypted, Action<float> updatePercentage)
        {
            if (filename == null)
                throw new InvalidDataException(Messages.ERROR_FILE_NAME_NULL);

            string filePath = Path.Combine(pathToOvf, filename);
            
            if (!File.Exists(filePath))
                throw new FileNotFoundException(string.Format(Messages.ERROR_FILE_NOT_FOUND, filename));

            string ext = Path.GetExtension(filename);
            string sourcefile = filePath;
            VirtualDisk vhdDisk = null;
            Stream dataStream = null;
            XenRef<VDI> vdiRef = null;

            try
            {
                #region ENCRYPTION

                if (isEncrypted)
                {
                    if (m_password == null)
                        throw new InvalidDataException(Messages.ERROR_NO_PASSWORD);

                    Description = string.Format(Messages.START_FILE_DECRYPTION, filename);
                    log.Debug($"Decrypting {filename} to temporary file enc_{filename}");
                    sourcefile = Path.Combine(pathToOvf, "enc_" + filename);
                    OVF.DecryptToTempFile(m_encryptionClass, filePath, m_encryptionVersion, m_password, sourcefile);
                }

                #endregion

                #region COMPRESSION

                if (compression.HasValue)
                {
                    Description = string.Format(Messages.START_FILE_EXPANSION, filename);
                    log.Debug($"Uncompressing {filename} to temporary file unc_{filename}");

                    // the compressed file will be replaced by the uncompressed, hence we need
                    // to use it with its disk extension (vmdk, vhd, etc.)
                    if (ext.ToLower().EndsWith(".gz"))
                    {
                        sourcefile = Path.Combine(pathToOvf, "unc_" + Path.GetFileNameWithoutExtension(filename));
                        ext = Path.GetExtension(sourcefile);
                    }

                    CompressionFactory.UncompressFile(filePath, sourcefile, compression.Value, CheckForCancellation);
                }

                #endregion

                #region OPEN DISK

                long dataLength;
                long virtualSize;
                string format = string.Empty;

                if (ext.ToLower().EndsWith("vhd"))
                {
                    vhdDisk = VirtualDisk.OpenDisk(sourcefile, FileAccess.Read);
                    virtualSize = vhdDisk.Capacity;
                    dataStream = File.OpenRead(sourcefile);
                    dataLength = dataStream.Length;
                    format = "&format=vhd";
                }
                else if (ext.ToLower().EndsWith("iso"))
                {
                    if (string.IsNullOrEmpty(sruuid))
                    {
                        log.Info($"Import of file {filename} was skipped");
                        return null;
                    }

                    dataStream = File.OpenRead(sourcefile);
                    dataLength = virtualSize = dataStream.Length;
                }
                else if (VirtualDiskManager.SupportedDiskFormats.Any(f => ext.ToLower().EndsWith(f.ToLower())))
                {
                    vhdDisk = VirtualDisk.OpenDisk(sourcefile, FileAccess.Read);
                    dataStream = vhdDisk.Content;
                    dataLength = virtualSize = vhdDisk.Capacity;
                }
                else
                {
                    throw new IOException(string.Format(Messages.UNSUPPORTED_FILE_TYPE, ext));
                }

                #endregion

                #region CREATE VDI IF SR HAS ENOUGH FREE SPACE

                //If no VDI uuid is provided create one, otherwise use the one provided as
                //the target for the import. Used for SRs such as Lun per VDI (PR-1544)

                Description = string.Format(Messages.IMPORT_VDI_PREPARE, filename);
                long freespace = 0;

                if (string.IsNullOrEmpty(vdiuuid))
                {
                    SR sr = Connection.Cache.SRs.FirstOrDefault(s => s.uuid == sruuid);
                    if (sr != null)
                        freespace = sr.physical_size - sr.physical_utilisation;

                    if (freespace < virtualSize)
                        throw new IOException(string.Format(Messages.SR_NOT_ENOUGH_SPACE,
                            sruuid, Util.DiskSizeString(virtualSize), filename));

                    VDI newVdi = new VDI
                    {
                        name_label = diskName,
                        name_description = description,
                        SR = new XenRef<SR>(sr == null ? Helper.NullOpaqueRef : sr.opaque_ref),//sr==null is unlikely
                        virtual_size = virtualSize,
                        type = vdi_type.user,
                        sharable = false,
                        read_only = false,
                        storage_lock = false,
                        managed = true,
                        is_a_snapshot = false
                    };

                    VDI vdi = Connection.WaitForCache(VDI.create(Connection.Session, newVdi));
                    if (vdi != null)
                    {
                        vdiuuid = vdi.uuid;
                        vdiRef = new XenRef<VDI>(vdi.opaque_ref);
                    }
                }
                else
                {
                    VDI vdi = Connection.Cache.VDIs.FirstOrDefault(v => v.uuid == vdiuuid);
                    if (vdi != null)
                    {
                        freespace = vdi.virtual_size;
                        vdiuuid = vdi.uuid;
                        vdiRef = new XenRef<VDI>(vdi.opaque_ref);
                    }

                    if (freespace < virtualSize)
                        throw new IOException(string.Format(Messages.VDI_NOT_ENOUGH_SPACE,
                            vdiuuid, Util.DiskSizeString(virtualSize), filename));
                }

                #endregion

                #region UPLOAD FILE

                var taskRef = Task.create(Connection.Session, "import_raw_vdi_task",
                    $"Importing disk {sourcefile} to VDI {vdiuuid}");

                var uriBuilder = new UriBuilder
                {
                    Scheme = Connection.UriScheme,
                    Host = Connection.Hostname,
                    Port = Connection.Port,
                    Path = "/import_raw_vdi",
                    Query = string.Format("session_id={0}&task_id={1}&vdi={2}{3}",
                        Connection.Session.opaque_ref, taskRef.opaque_ref, vdiuuid, format)
                };

                using (Stream outStream = HTTPHelper.PUT(uriBuilder.Uri, dataStream.Length, true))
                    HTTP.CopyStream(dataStream, outStream,
                        b =>
                        {
                            Description = string.Format(Messages.IMPORT_VDI, filename,
                                Util.DiskSizeString(b, 2, "F2"), Util.DiskSizeString(dataLength));

                            updatePercentage((float)b / dataLength);
                        },
                        () => Cancelling);

                #endregion

                return vdiRef;
            }
            catch (Exception e)
            {
                if (vdiRef != null)
                {
                    log.Info($"Import interrupted. Destroying VDI {vdiRef.opaque_ref}");
                    try
                    {
                        //need to wait for a bit until the VDI is released from the import task
                        Connection.WaitFor(() =>
                        {
                            var vdi = Connection.Resolve(vdiRef);
                            return vdi != null && vdi.allowed_operations.Contains(vdi_operations.destroy);
                        }, null);
                        VDI.destroy(Connection.Session, vdiRef);
                    }
                    catch
                    {
                        log.Error($"Failed to destroy VDI {vdiRef.opaque_ref} after interrupted import.");
                    }
                }

                if (e is HTTP.CancelledException)
                    throw new CancelledException();
                
                throw;
            }
            finally
            {
                dataStream?.Dispose();
                vhdDisk?.Dispose();

                try
                {
                    var sourcefileName = Path.GetFileName(sourcefile);
                    
                    if ((sourcefileName.StartsWith("enc_") || sourcefileName.StartsWith("unc_")) && File.Exists(sourcefile))
                        File.Delete(sourcefile);
                }
                catch
                {
                    //ignore errors
                }
            }
        }

        private XenRef<VM> CreateVm(string vmName, VirtualHardwareSection_Type system, XenRef<VM_appliance> applRef, StartupSection_TypeItem vmStartupSection)
        {
            Description = string.Format(Messages.IMPORT_CREATING_VM, vmName);
            string description = system.System?.Description?.Value ?? Messages.DEFAULT_IMPORT_DESCRIPTION;

            #region MEMORY

            ulong memorySize = 0;
            RASD_Type[] rasds = OVF.FindRasdByType(system, 4);

            if (rasds != null && rasds.Length > 0)
            {
                //The default memory unit is MB (2^20), however, the RASD may contain a different
                //one with format byte*memoryBase^memoryPower (byte being a literal string)

                double memoryBase = 2.0;
                double memoryPower = 20.0;

                foreach (RASD_Type rasd in rasds)
                {
                    if (rasd.AllocationUnits.Value.ToLower().StartsWith("byte"))
                    {
                        string[] a1 = rasd.AllocationUnits.Value.Split('*', '^');

                        if (a1.Length == 3)
                        {
                            if (!double.TryParse(a1[1].Trim(), out memoryBase))
                                memoryBase = 2.0;
                            if (!double.TryParse(a1[2].Trim(), out memoryPower))
                                memoryPower = 20.0;
                        }
                    }

                    double memoryMultiplier = Math.Pow(memoryBase, memoryPower);
                    memorySize += rasd.VirtualQuantity.Value * Convert.ToUInt64(memoryMultiplier);
                }
            }

            if (memorySize == 0)
                memorySize = VM.DEFAULT_MEM_MIN_IMG_IMPORT;
            else if (memorySize > long.MaxValue)
                memorySize = long.MaxValue;

            #endregion

            #region CPU COUNT

            ulong cpuCount = 0;
            ulong maxCpusCount = 0;
            rasds = OVF.FindRasdByType(system, 3);

            if (rasds != null && rasds.Length > 0)
            {
                //There may be more than one entries corresponding to CPUs
                //The VirtualQuantity in each one is Cores

                foreach (var rasd in rasds)
                {
                    cpuCount += rasd.VirtualQuantity.Value;
                    // CA-361078: Older versions of CHC/XC used to set the limit to 100,000 by default, and use 
                    // VirtualQuantity for max vCPUs.
                    // This way, we keep backwards compatibility with older OVFs.
                    var limit = rasd.Limit?.Value ?? 100_000;
                    maxCpusCount += limit >= 100_000 ? rasd.VirtualQuantity.Value : limit;
                }
                   
            }

            if (cpuCount < 1) //default minimum
                cpuCount = 1;
            else if (cpuCount > long.MaxValue) //unlikely, but better be safe
                cpuCount = long.MaxValue;

            if (maxCpusCount < 1)
                maxCpusCount = 1;
            else if (maxCpusCount > long.MaxValue)
                maxCpusCount = long.MaxValue;

            #endregion

            VM newVm = new VM
            {
                name_label = vmName ?? Messages.UNDEFINED_NAME_LABEL,
                name_description = description,
                user_version = 1,
                is_a_template = false,
                is_a_snapshot = false,
                memory_target = (long)memorySize,
                memory_static_max = (long)memorySize,
                memory_dynamic_max = (long)memorySize,
                memory_dynamic_min = (long)memorySize,
                memory_static_min = (long)memorySize,
                VCPUs_max = (long)maxCpusCount,
                VCPUs_at_startup = (long)cpuCount,
                actions_after_shutdown = on_normal_exit.destroy,
                actions_after_reboot = on_normal_exit.restart,
                actions_after_crash = on_crash_behaviour.restart,
                HVM_shadow_multiplier = 1.0,
                ha_always_run = false,
                other_config = new Dictionary<string, string> {{"HideFromXenCenter", "true"}}
            };

            //Note that the VM has to be created hidden.
            //We'll make it visible in the end, after all the setup is done

            #region XEN SPECIFIC CONFIGURATION INFORMATION

            if (system.VirtualSystemOtherConfigurationData == null || system.VirtualSystemOtherConfigurationData.Length <= 0)
            {
                // DEFAULT should work for all of HVM type or 301
                newVm.HVM_boot_policy = "BIOS order";
                newVm.HVM_boot_params = new Dictionary<string, string> {{"order", "dc"}};
                newVm.platform = new Dictionary<string, string>
                    {{"nx", "true"}, {"acpi", "true"}, {"apic", "true"}, {"pae", "true"}, {"stdvga", "0"}};
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
                            //In new OVFs the xcsd.Value.Value is a dictionary string like "key1=value1;key2=value2"
                            //However, we want to be backwards compatible with old OVFs where it was a plain string
                            newVm.HVM_boot_params = xcsdValue.IndexOf('=') > -1
                                ? xcsdValue.SplitToDictionary(';')
                                : new Dictionary<string, string> {{"order", xcsdValue}};
							break;
                        case "platform":
                            newVm.platform = xcsd.Value.Value.SplitToDictionary(';');
                            if (!newVm.platform.ContainsKey("nx"))
                                newVm.platform.Add("nx", "true");
                            break;
	                    case "nvram":
		                    newVm.NVRAM = xcsd.Value.Value.SplitToDictionary(';');
		                    break;
                        case "vgpu":
                            //Skip vGPUs here; we'll set them up after the VM is created
                            //because we need the VM's opaque_ref for them
                            break;
                        default:
                            hashtable.Add(key, xcsd.Value.Value);
                            break;
                    }
                }
                newVm.UpdateFrom(hashtable);
            }

            #endregion

            #region Set appliance

            if (applRef != null)
                newVm.appliance = applRef;

            if (vmStartupSection != null)
            {
                newVm.start_delay = vmStartupSection.startDelay;
                newVm.shutdown_delay = vmStartupSection.stopDelay;
                newVm.order = vmStartupSection.order;
            }

            #endregion

            #region set has_vendor_device

            var data = system.VirtualSystemOtherConfigurationData;
            var datum = data?.FirstOrDefault(s => s.Name == "VM_has_vendor_device");
            if (datum != null)
            {
                if (bool.TryParse(datum.Value.Value, out var hasVendorDevice) && hasVendorDevice)
                    newVm.has_vendor_device = hasVendorDevice;
            }

            #endregion

            var vm = Connection.WaitForCache(VM.create(Connection.Session, newVm));
            log.Info($"Created VM {vmName} ({vm.opaque_ref})");
            return new XenRef<VM>(vm.opaque_ref);
        }

        private void CreateVgpus(VirtualHardwareSection_Type system, XenRef<VM> vmRef)
        {
            CheckForCancellation();

            var data = system.VirtualSystemOtherConfigurationData;
            if (data == null)
                return;

            var datum = data.Where(s => s.Name == "vgpu");

            foreach (var item in datum)
            {
                Match m = VGPU_REGEX.Match(item.Value.Value);
                if (!m.Success)
                    continue;
                
                var types = m.Groups[1].Value.Split(';');

                var gpuGroup = Connection.Cache.GPU_groups.FirstOrDefault(g =>
                    g.supported_VGPU_types.Count > 0 &&
                    g.GPU_types.Length == types.Length &&
                    g.GPU_types.Intersect(types).Count() == types.Length);

                if (gpuGroup ==  null)
                    continue;

                string vendorName = m.Groups[2].Value;
                string modelName = m.Groups[3].Value;

                VGPU_type vgpuType = Connection.Cache.VGPU_types.FirstOrDefault(v =>
                    v.vendor_name == vendorName && v.model_name == modelName);

                if (vgpuType == null)
                    continue;

                var otherConfig = new Dictionary<string, string>();

                if (Helpers.FeatureForbidden(Connection, Host.RestrictVgpu))
                {
                    //If the current licence does not allow vGPU, we create one pass-through vGPU
                    //(default vGPU type) for the VM (multiple pass-through vGPUs are not supported yet)

                    var oneVgpu = Connection.WaitForCache(VGPU.create(Connection.Session, vmRef.opaque_ref, gpuGroup.opaque_ref, "0", otherConfig));
                    log.Info($"The host license does not support vGPU. Created one pass-through vGPU ({oneVgpu.opaque_ref}) for the VM");
                    break;
                }

                var vgpu = Connection.WaitForCache(VGPU.create(Connection.Session, vmRef.opaque_ref, gpuGroup.opaque_ref, "0", otherConfig, vgpuType.opaque_ref));
                log.Info($"Created vGPU {vgpu.opaque_ref}");
            }
        }

        private void CreatePvsProxy(VirtualHardwareSection_Type system, XenRef<VM> vmRef)
        {
            CheckForCancellation();

            var datum = system.VirtualSystemOtherConfigurationData?.FirstOrDefault(s => s.Name == "pvssite");
            if (datum == null)
                return;

            Match m = PVS_SITE_REGEX.Match(datum.Value.Value);
            if (!m.Success)
                return;
            
            var siteUuid = m.Groups[1].Value;
            var site = Connection.Cache.PVS_sites.FirstOrDefault(p => p.uuid == siteUuid);
            if (site == null)
                return;

            var vm = Connection.TryResolveWithTimeout(vmRef);
            if (vm == null)
                return;

            foreach (var vifRef in vm.VIFs)
            {
                var vif = Connection.TryResolveWithTimeout(vifRef);
                if (vif != null && vif.device.Equals("0"))
                {
                    var pvs = Connection.WaitForCache(PVS_proxy.create(Connection.Session, site.opaque_ref, vif.opaque_ref));
                    log.Info($"Created PVS_proxy {pvs.opaque_ref}");
                    break;
                }
            }
        }

        private void AddResourceSettingData(EnvelopeType ovfObj, XenRef<VM> vmRef, RASD_Type rasd, string pathToOvf, ref int vifDeviceIndex, Action<float> updatePercentage)
        {
            switch (rasd.ResourceType.Value)
            {
                case 10: // Network
                {
                    #region FIND NETWORK

                    XenRef<XenAPI.Network> net = null;
                    XenRef<XenAPI.Network> netDefault = null;

                    if (rasd.Connection != null && rasd.Connection.Length > 0 && !string.IsNullOrEmpty(rasd.Connection[0].Value))
                    {
                        // Ignore the NetworkSection/Network
                        // During Network Selection the UUID for Network was set in Connection Field

                        var dict = rasd.Connection[0].Value.SplitToDictionary(',');

                        if (!dict.TryGetValue("network", out string netuuid))
                            dict.TryGetValue("XenNetwork", out netuuid);

                        var networks = Connection.Cache.Networks;

                        foreach (XenAPI.Network network in networks)
                        {
                            if (net == null && netuuid != null &&
                                (netuuid.Equals(network.uuid) ||
                                 network.name_label.ToLower().Contains(netuuid) ||
                                 network.bridge.ToLower().Contains(netuuid)))
                                net = new XenRef<XenAPI.Network>(network.opaque_ref);

                            if (network.bridge.ToLower().Contains("xenbr0"))
                                netDefault = new XenRef<XenAPI.Network>(network.opaque_ref);
                        }

                        if (net == null)
                            net = netDefault;
                    }

                    #endregion

                    #region CREATE VIF

                    VIF newVif = new VIF
                    {
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
                        if (rasd.Address.Value != null && !rasd.Address.Value.Contains(":"))
                            newVif.MAC = string.Join(":", rasd.Address.Value.SplitInChunks(2));

                        if (string.IsNullOrEmpty(newVif.MAC))
                            newVif.MAC = rasd.Address.Value;
                    }

                    var vif = Connection.WaitForCache(VIF.create(Connection.Session, newVif));
                    log.Info($"Created VIF {vif.opaque_ref}");

                    #endregion

                    break;
                }
                case 15: // CD Drive
                case 16: // DVD Drive
                {
                    var vm = Connection.Resolve(vmRef);
                    if (vm != null)
                    {
                        foreach (var vbdRef in vm.VBDs)
                        {
                            var vbd = Connection.Resolve(vbdRef);
                            if (vbd != null && vbd.type == vbd_type.CD)
                                return; //currently XenServer allows only one CD/DVD drive per VM
                        }
                    }

                    #region CHECK FOR VDI TO UPLOAD

                    Description = Messages.CD_DVD_DEVICE_ATTACHED;

                    var filename = OVF.FindRasdFileName(ovfObj, rasd, out CompressionFactory.Type? compression);
                    var isoImage = Connection.Cache.VDIs.FirstOrDefault(v => v.name_label == filename);
                    XenRef<VDI> vdiRef = null;

                    if (isoImage != null)
                    {
                        vdiRef = new XenRef<VDI>(isoImage.opaque_ref);
                    }
                    else if (filename != null && !MetaDataOnly)
                    {
                        //find the ISO SR mapped in the OVF and import the ISO

                        string isoSrUuid = null;

                        if (rasd.Connection != null && rasd.Connection.Length > 0 && !string.IsNullOrEmpty(rasd.Connection[0].Value))
                        {
                            var dict = rasd.Connection[0].Value.SplitToDictionary(',');
                            dict.TryGetValue("sr", out string srId);
                            isoSrUuid = Connection.Cache.SRs.FirstOrDefault(s => s.uuid == srId || s.name_label == srId)?.uuid;
                        }

                        if (isoSrUuid != null)
                            vdiRef = ImportFile(filename, pathToOvf, filename,
                                compression, isoSrUuid, "", null, OVF.IsThisEncrypted(ovfObj, rasd), updatePercentage);
                    }

                    #endregion

                    #region CREATE VBD CONNECTION

                    var hashtable = new Hashtable();

                    if (rasd.Connection != null && rasd.Connection.Length > 0 && !string.IsNullOrEmpty(rasd.Connection[0].Value))
                    {
                        var dict = rasd.Connection[0].Value.SplitToDictionary(',');

                        foreach (var kvp in dict)
                        {
                            switch (kvp.Key)
                            {
                                case "sr":
                                case "vdi":
                                    continue;
                                case "device":
                                    hashtable.Add("userdevice", kvp.Value);
                                    break;
                                default:
                                    hashtable.Add(kvp.Key, kvp.Value);
                                    break;
                            }
                        }
                    }

                    VBD isoVbd = new VBD
                    {
                        VM = vmRef,
                        mode = vbd_mode.RO,
                        userdevice = "3",
                        type = vbd_type.CD,
                        storage_lock = false,
                        status_code = 0
                    };

                    isoVbd.UpdateFrom(hashtable);

                    if (vdiRef == null || Helper.IsNullOrEmptyOpaqueRef(vdiRef))
                    {
                        isoVbd.empty = true;
                    }
                    else
                    {
                        isoVbd.VDI = vdiRef;
                        isoVbd.empty = false;
                        isoVbd.bootable = true;
                        isoVbd.unpluggable = true;
                    }

                    isoVbd.userdevice = VerifyUserDevice(vmRef, isoVbd.userdevice);
                    isoVbd.other_config = new Dictionary<string, string> {{"owner", "true"}};

                    if (!isoVbd.userdevice.EndsWith("+"))
                    {
                        try
                        {
                            Connection.WaitForCache(VBD.create(Connection.Session, isoVbd));
                            log.Info("Created VBD for CD/DVD ROM");
                        }
                        catch (Exception ex)
                        {
                            log.Error("Failed to create VBD for CD/DVD ROM.", ex);
                        }
                    }
                    else
                    {
                        log.WarnFormat(
                            "Import:  ================== ATTENTION NEEDED =======================" +
                            "Import:  Could not determine appropriate number of device placement." +
                            "Import:  Please Start, Logon, Shut down VM ({0}), then manually attach" +
                            "Import:  disks with labels ending with \"+\" to the device number defined before the +." +
                            "Import:  ===========================================================", vmRef);
                    }

                    #endregion

                    break;
                }
                case 17: // Disk Drive
                case 19: // Storage Extent
                case 21: // Microsoft: Hard drive/Floppy/ISO
                {
                    #region ADD DISK

                    if (Tools.ValidateProperty("Caption", rasd) &&
                        (rasd.Caption.Value.ToUpper().Contains("COM") ||
                         rasd.Caption.Value.ToUpper().Contains("FLOPPY") ||
                         rasd.Caption.Value.ToUpper().Contains("ISO")))
                        break;

                    File_Type file = OVF.FindFileReferenceByRASD(ovfObj, rasd);
                    if (file == null)
                        break;

                    SetIfDeviceIsBootable(ovfObj, rasd);

                    var filename = OVF.FindRasdFileName(ovfObj, rasd, out CompressionFactory.Type? compression);
                    if (filename == null)
                    {
                        log.Warn($"No file available to import, skipping RASD {rasd.ResourceType.Value}: {rasd.InstanceID.Value}");
                        return;
                    }

                    if (Tools.ValidateProperty("Caption", rasd) &&
                        (rasd.Caption.Value.ToUpper().Contains("COM") ||
                         rasd.Caption.Value.ToUpper().Contains("FLOPPY") ||
                         rasd.Caption.Value.ToUpper().Contains("ISO")))
                    {
                        log.Info($"Resource {filename} is {rasd.Caption.Value}. Skipping import.");
                        return;
                    }

                    if (MetaDataOnly)
                    {
                        log.Info($"Importing metadata only. Skipping resource {filename}.");
                        return;
                    }

                    string sruuid = null;
                    string vdiuuid = null;
                    string userdeviceid = null;
                    bool isbootable = false;
                    vbd_mode mode = vbd_mode.RW;

                    #region PARSE CONNECTION

                    if (Tools.ValidateProperty("Connection", rasd) && !string.IsNullOrEmpty(rasd.Connection[0].Value))
                    {
                        var dict = rasd.Connection[0].Value.SplitToDictionary(',');

                        dict.TryGetValue("device", out userdeviceid);
                        dict.TryGetValue("vdi", out vdiuuid);

                        if (dict.TryGetValue("sr", out var srId))
                            sruuid = Connection.Cache.SRs.FirstOrDefault(s => s.uuid == srId || s.name_label == srId)?.uuid;

                        if (dict.TryGetValue("bootable", out var bootable))
                            isbootable = Convert.ToBoolean(bootable);

                        if (dict.TryGetValue("mode", out var theMode) && theMode == "r")
                            mode = vbd_mode.RO;
                    }

                    #endregion

                    #region FIND SR

                    if (sruuid == null && vdiuuid != null)
                    {
                        var mappedVdi = Connection.Cache.VDIs.FirstOrDefault(v => v.uuid == vdiuuid);
                        if (mappedVdi != null)
                            sruuid = Connection.Cache.SRs.FirstOrDefault(s => s.opaque_ref == mappedVdi.SR.opaque_ref)?.uuid;
                    }

                    if (sruuid == null)
                    {
                        var pool = Helpers.GetPoolOfOne(Connection);
                        if (pool != null)
                            sruuid = Connection.Resolve(pool.default_SR)?.uuid;
                    }

                    if (sruuid == null)
                        throw new InvalidDataException(Messages.ERROR_COULD_NOT_FIND_SR);

                    #endregion

                    #region IMPORT DISK

                    var vm = Connection.Resolve(vmRef);

                    string disklabel = rasd.ElementName?.Value;
                    if (string.IsNullOrEmpty(disklabel))
                        disklabel = $"{vm.Name()}_{userdeviceid}";

                    string description = rasd.Description?.Value ?? "";

                    XenRef<VDI> vdiRef = ImportFile(disklabel, pathToOvf, filename, compression, sruuid, description, vdiuuid, OVF.IsThisEncrypted(ovfObj, rasd), updatePercentage);

                    #endregion

                    #region CONNECT VBD

                    VBD vbd = new VBD
                    {
                        userdevice = VerifyUserDevice(vmRef, userdeviceid ?? "99"),
                        bootable = isbootable,
                        VDI = vdiRef,
                        mode = mode,
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
                            Connection.WaitForCache(VBD.create(Connection.Session, vbd));
                            log.Info($"Created VBD for disk {filename}.");
                        }
                        catch (Exception ex)
                        {
                            log.Error($"Failed to create VBD for disk {filename}." +
                                "Please attach disk manually.", ex);
                        }
                    }
                    else
                    {
                        log.WarnFormat(
                            "Import:  ================== ATTENTION NEEDED =======================" +
                            "Import:  Could not determine appropriate number for device placement." +
                            "Import:  Please Start, Logon, Shut down VM ({0}), then manually attach" +
                            "Import:  disks with labels with {0}_# that are not attached to {0}." +
                            "Import:  ===========================================================",
                            vm.Name());
                    }

                    #endregion

                    break;

                    #endregion
                }
            }
        }

        private string VerifyUserDevice(XenRef<VM> vmRef, string device)
        {
            log.DebugFormat("Import.VerifyUserDevice, checking device: {0} (99 = autoselect)", device);
            string usethisdevice = null;

            string[] allowedVBDs = VM.get_allowed_VBD_devices(Connection.Session, vmRef);

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
                if (device != null && !device.EndsWith("+"))
                    usethisdevice = device + "+";                
            }
            return usethisdevice;
        }

        private static void SetDeviceConnections(VirtualHardwareSection_Type vhs)
        {
            int[] connections = new int[16];

            var rasdList = new List<RASD_Type>(vhs.Item);
            rasdList.Sort(OVF.CompareControllerRasd);  // sorts based on ResourceType.Value

            // Xen does not support the different controller types,
            // therefore we must ensure via positional on controllers.
            // IDE - #1
            // SCSI - #2
            // IDE 0 Disk 0  Goes to Xen: userdevice=0
            // IDE 0 Disk 1  Goes to Xen: userdevice=1 
            // IDE 1 CD/DVD 0  Goes to Xen: userdevice=2
            // IDE 1 Disk 1  UnUsed ??????
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
                            List<RASD_Type> connectedrasds = OVF.FindConnectedItems(rasd.InstanceID.Value, vhs.Item, null);
                            
                            foreach (RASD_Type _rasd in connectedrasds)
                            {
                                int deviceoffset = _rasd.ResourceType.Value == 15 || _rasd.ResourceType.Value == 16 ? 2 : 0;

                                if (Tools.ValidateProperty("Connection", _rasd))
                                {
                                    if (!_rasd.Connection[0].Value.ToLower().Contains("device="))
                                    {
                                       _rasd.Connection[0].Value = string.Format("{0},device={1}", _rasd.Connection[0].Value, FindNextAvailable(deviceoffset, connections, 0));
                                    }
                                }
                                else
                                {
                                    _rasd.Connection = new[] { new cimString(string.Format("device={0}", FindNextAvailable(deviceoffset, connections, 0))) };
                                }
                            }
                            break;
                        }
                }
            }
        }

        private static int FindNextAvailable(int offset, int[] ids, int unusedkey)
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

        private static void SetIfDeviceIsBootable(EnvelopeType ovfEnv, RASD_Type rasd)
        {
            // This is a best guess algorithm.
            // Without opening the VHD itself, there is no guaranteed method to delineate this.
            // If it's created by Kensho/XenConvert there will be a chance of having a clue.
            // Otherwise it'll be based upon 'order' and device 0 will be the bootable device.

            if (!Tools.ValidateProperty("Connection", rasd))
                rasd.Connection = new[] {new cimString("")};

            if (rasd.Connection[0].Value.Contains("bootable"))
                return;

            bool isBootable = false;
            
            VirtualDiskDesc_Type[] disks = null;

            foreach (Section_Type sect in ovfEnv.Sections)
            {
                if (sect is DiskSection_Type diskSect)
                    disks = diskSect.Disk;
            }

            if (disks != null)
            {
                bool useHostResource = Tools.ValidateProperty("HostResource", rasd);
                var quantity = useHostResource ? rasd.HostResource[0].Value : rasd.InstanceID.Value;

                foreach (VirtualDiskDesc_Type disk in disks)
                {
                    if (quantity.Contains(disk.diskId))
                    {
                        isBootable = disk.isBootable;
                        break;
                    }
                }
            }

            if (!isBootable && Tools.ValidateProperty("Address", rasd) &&
                (rasd.ResourceType.Value == 21 || rasd.ResourceType.Value == 5) &&
                rasd.Address.Value == "0")
            {
                isBootable = true;
            }

            if (!isBootable && Tools.ValidateProperty("AddressOnParent", rasd) &&
                (rasd.ResourceType.Value == 17 || rasd.ResourceType.Value == 19) &&
                rasd.AddressOnParent.Value == "0")
            {
                isBootable = true;
            }

            if (!isBootable && rasd.Connection[0].Value.Contains("device=0"))
                isBootable = true;
            
            rasd.Connection[0].Value = $"{rasd.Connection[0].Value},bootable={isBootable}";
        }

        private void CleanUpVm(XenRef<VM> vmRef)
        {
            var vdiRefs = new List<XenRef<VDI>>();

            var vm = Connection.Resolve(vmRef);
            if (vm != null)
                foreach (var vbdRef in vm.VBDs)
                {
                    VBD vbd = Connection.Resolve(vbdRef);
                    if (vbd != null)
                    {
                        var vdi = Connection.Resolve(vbd.VDI);
                        if (vdi != null)
                            vdiRefs.Add(vbd.VDI);
                    }
                }

            try
            {
                VM.destroy(Connection.Session, vmRef);
            }
            catch
            {
                log.Error($"Failed to destroy VM {vmRef.opaque_ref} after interrupted import.");
            }

            foreach (var vdiRef in vdiRefs)
                try
                {
                    VDI.destroy(Connection.Session, vdiRef);
                }
                catch
                {
                    log.Error($"Failed to destroy VDI {vdiRef.opaque_ref} after interrupted import.");
                }
        }
    }
}
