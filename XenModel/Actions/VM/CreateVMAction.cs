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
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;
using System.Xml;


namespace XenAdmin.Actions.VMActions
{
    public enum InstallMethod
    {
        None,
        CD,
        Network
    }

    public enum BootMode { BIOS_BOOT, UEFI_BOOT, UEFI_SECURE_BOOT, NOT_AVAILABLE }

    public static class EnumExt
    {
        public static string StringOf(this BootMode x)
        {
            switch (x)
            {
                case BootMode.BIOS_BOOT:
                    return Messages.BIOS_BOOT;
                case BootMode.UEFI_BOOT:
                    return Messages.UEFI_BOOT;
                case BootMode.UEFI_SECURE_BOOT:
                    return Messages.UEFI_SECURE_BOOT;
                default:
                    return Messages.UNAVAILABLE;
            }
        }
    }

    public class CreateVMAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string NameLabel;
        private readonly string NameDescription;
        private readonly InstallMethod InsMethod;
        private readonly string PvArgs;
        private readonly VDI Cd;
        private readonly string Url;
        private readonly BootMode BootMode;
        private readonly Host HomeServer;
        private readonly long VcpusMax;
        private readonly long VcpusAtStartup;
        private readonly long MemoryDynamicMin, MemoryDynamicMax, MemoryStaticMax;
        private readonly List<DiskDescription> Disks;
        private readonly List<VIF> Vifs;
        private readonly bool StartAfter;
        private readonly Host CopyBiosStringsFrom;
        private readonly SR FullCopySR;
        private readonly GPU_group GpuGroup;
        private readonly VGPU_type VgpuType;
        private readonly long CoresPerSocket;
        private readonly string cloudConfigDriveTemplateText;
        private SR firstSR = null;

        private Action<VMStartAbstractAction, Failure> _startDiagnosisForm;
        private Action<VM, bool> _warningDialogHAInvalidConfig;

        private bool PointOfNoReturn;

        private bool assignOrRemoveVgpu;

        /// <summary>
        /// These are the RBAC dependencies that you always need to create a VM. Check CreateVMAction constructor for runtime dependent dependencies.
        /// </summary>
        public static RbacMethodList StaticRBACDependencies = new RbacMethodList(
            // provision VM
            "vm.provision",
            "vm.set_other_config",
            // set VM Params
            "vm.set_name_label",
            "vm.set_name_description",
            "vm.set_VCPUs_max",
            "vm.set_VCPUs_at_startup",
            // set VM Boot Params
            "vm.set_HVM_boot_params",
            "vm.set_PV_args",
            "vm.set_other_config",
            // Add CD Drive
            "vbd.eject",
            "vbd.insert",
            // Create CD Drive
            "vbd.create",
            // Add disks
            "vdi.destroy",
            "vdi.create",
            "vdi.set_sm_config",
            "vbd.create",
            "vbd.destroy",
            "vdi.copy",
            // Add networks
            "vif.create",
            "vm.set_platform"
        );

        public CreateVMAction(IXenConnection connection, VM template, Host copyBiosStringsFrom,
            string name, string description, InstallMethod installMethod,
            string pvArgs, VDI cd, string url, BootMode bootMode, Host homeServer, long vcpusMax, long vcpusAtStartup,
            long memoryDynamicMin, long memoryDynamicMax, long memoryStaticMax,
            List<DiskDescription> disks, SR fullCopySR, List<VIF> vifs, bool startAfter,
            Action<VM, bool> warningDialogHAInvalidConfig,
            Action<VMStartAbstractAction, Failure> startDiagnosisForm,
            GPU_group gpuGroup, VGPU_type vgpuType, bool modifyVgpuSettings, long coresPerSocket, string cloudConfigDriveTemplateText)
            : base(connection, string.Format(Messages.CREATE_VM, name),
            string.Format(Messages.CREATE_VM_FROM_TEMPLATE, name, Helpers.GetName(template)))
        {
            Template = template;
            CopyBiosStringsFrom = copyBiosStringsFrom;
            FullCopySR = fullCopySR;
            NameLabel = name;
            NameDescription = description;
            InsMethod = installMethod;
            PvArgs = pvArgs;
            Cd = cd;
            Url = url;
            BootMode = bootMode;
            HomeServer = homeServer;
            VcpusMax = vcpusMax;
            VcpusAtStartup = vcpusAtStartup;
            MemoryDynamicMin = memoryDynamicMin;
            MemoryDynamicMax = memoryDynamicMax;
            MemoryStaticMax = memoryStaticMax;
            Disks = disks;
            Vifs = vifs;
            StartAfter = startAfter;
            _warningDialogHAInvalidConfig = warningDialogHAInvalidConfig;
            _startDiagnosisForm = startDiagnosisForm;
            GpuGroup = gpuGroup;
            VgpuType = vgpuType;
            CoresPerSocket = coresPerSocket;
            this.cloudConfigDriveTemplateText = cloudConfigDriveTemplateText;

            Pool pool_of_one = Helpers.GetPoolOfOne(Connection);
            if (HomeServer != null || pool_of_one != null) // otherwise we have no where to put the action
                AppliesTo.Add(HomeServer != null ? HomeServer.opaque_ref : pool_of_one.opaque_ref);

            assignOrRemoveVgpu = GpuGroup != null && VgpuType != null || modifyVgpuSettings && Helpers.GpuCapability(Connection);

            #region RBAC Dependencies

            if (StartAfter)
                ApiMethodsToRoleCheck.Add("vm.start");
            if (HomeServerChanged())
                ApiMethodsToRoleCheck.Add("vm.set_affinity");
            if (Template.memory_dynamic_min != MemoryDynamicMin || Template.memory_dynamic_max != MemoryDynamicMax || Template.memory_static_max != MemoryStaticMax)
                ApiMethodsToRoleCheck.Add("vm.set_memory_limits");

            if (assignOrRemoveVgpu)
            {
                ApiMethodsToRoleCheck.Add("VGPU.destroy");
                ApiMethodsToRoleCheck.Add("VGPU.create");
            }

            ApiMethodsToRoleCheck.AddRange(StaticRBACDependencies);

            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);

            #endregion
        }

        protected override void Run()
        {
            if (FullCopySR != null)
            {
                // VM.copy is the best call to make if all target disks are on the same SR.
                // however, if the target disks are on the same SR as the source disks, then the user is 
                // given the choice of a fast-clone (VM.clone) or a full-copy (VM.copy) on the storage page of the wizard. If the
                // user chose a VM.clone, then FullCopySR will be null.

                RelatedTask = VM.async_copy(Session, Template.opaque_ref, HiddenVmName, FullCopySR.opaque_ref);
            }
            else
            {
                // if the target disks are on mixed storage or the user chose to a do a fast-clone on the storage
                // page then we end up here.

                RelatedTask = VM.async_clone(Session, Template.opaque_ref, HiddenVmName);
            }

            Description = string.Format(Messages.CLONING_TEMPLATE, Helpers.GetName(Template));
            PollToCompletion(0, 10);

            VM = Connection.WaitForCache(new XenRef<VM>(Result));

            ApplyRecommendationsForVendorDevice();
            CopyBiosStrings();
            SetXenCenterProperties();
            ProvisionVM();
            SetVMParams();
            SetVMBootParams();
            AddCdDrive();
            AddDisks();
            AddNetworks();
            XenAdminConfigManager.Provider.ShowObject(VM.opaque_ref);
            VM.IsBeingCreated = false;
            PointOfNoReturn = true;

            CloudCreateConfigDrive();
           
            AssignVgpu();

            if (StartAfter)
            {
                Description = Messages.STARTING_VM;
                var startAction = new VMStartAction(VM, _warningDialogHAInvalidConfig, _startDiagnosisForm);
                startAction.RunAsync();
            }

            Description = Messages.VM_SUCCESSFULLY_CREATED;
        }

        private void ApplyRecommendationsForVendorDevice()
        {
            var pool = Helpers.GetPoolOfOne(Connection);
            bool poolPolicyNoVendorDevice = pool == null || pool.policy_no_vendor_device;
            bool hasVendorDeviceRecommendation = Template.HasVendorDeviceRecommendation();

            if (hasVendorDeviceRecommendation && !poolPolicyNoVendorDevice && !Helpers.FeatureForbidden(VM, Host.RestrictVendorDevice))
            {
                log.DebugFormat("Recommendation (has-vendor-device = true) has been found on the template ({0}) and the host is licensed, so applying it on VM ({1}) being created.", Template.opaque_ref, VM.opaque_ref);
                VM.set_has_vendor_device(Connection.Session, VM.opaque_ref, true);
            }
            else
            {
                log.DebugFormat("Recommendation (has-vendor-device = true) has not been applied on the VM ({0}) being created.", VM.opaque_ref);

                if (!hasVendorDeviceRecommendation)
                    log.DebugFormat("Recommendation (has-vendor-device) is not set or false on the template ({0}).", Template.opaque_ref);

                if (poolPolicyNoVendorDevice)
                    log.DebugFormat("pool.policy_no_vendor_device returned {0}", poolPolicyNoVendorDevice);

                if (Helpers.FeatureForbidden(VM, Host.RestrictVendorDevice))
                    log.DebugFormat("Helpers.FeatureForbidden(VM, Host.RestrictVendorDevice) returned {0}", Helpers.FeatureForbidden(VM, Host.RestrictVendorDevice));
            }
        }

        private void CloudCreateConfigDrive()
        {
            if (Template.CanHaveCloudConfigDrive() && !string.IsNullOrEmpty(cloudConfigDriveTemplateText))
            {
                Description = Messages.CREATING_CLOUD_CONFIG_DRIVE; 

                var parameters = new Dictionary<string, string>();
                parameters.Add("vmuuid", VM.uuid);
                parameters.Add("sruuid", firstSR.uuid);
                parameters.Add("configuration", cloudConfigDriveTemplateText.Replace("\r\n", "\n"));

                var action = new ExecutePluginAction(Connection, HomeServer ?? Helpers.GetMaster(Connection),
                            "xscontainer",//plugin
                            "create_config_drive",//function
                            parameters,
                            true); //hidefromlogs

                action.RunExternal(Connection.Session);
                var result = action.Result.Replace("\n", Environment.NewLine);
            }
        }

        private void AssignVgpu()
        {
            if (assignOrRemoveVgpu)
            {
                var action = new GpuAssignAction(VM, GpuGroup, VgpuType);
                action.RunExternal(Session);
            }
        }

        private void CopyBiosStrings()
        {
            if (CopyBiosStringsFrom != null && Template.DefaultTemplate())
            {
                VM.copy_bios_strings(Session, this.VM.opaque_ref, CopyBiosStringsFrom.opaque_ref);
            }
        }

        private void SetXenCenterProperties()
        {
            VM.IsBeingCreated = true;
            XenAdminConfigManager.Provider.HideObject(VM.opaque_ref);
            AppliesTo.Add(VM.opaque_ref);
        }

        private void SetVMParams()
        {
            Description = Messages.SETTING_VM_PROPERTIES;
            XenAPI.VM.set_name_label(Session, VM.opaque_ref, NameLabel);
            XenAPI.VM.set_name_description(Session, VM.opaque_ref, NameDescription);
            ChangeVCPUSettingsAction vcpuAction = new ChangeVCPUSettingsAction(VM, VcpusMax, VcpusAtStartup);
            vcpuAction.RunExternal(Session);

            // set cores-per-socket
            Dictionary<string, string> platform = VM.platform == null ?
                            new Dictionary<string, string>() :
                            new Dictionary<string, string>(VM.platform);
            platform["cores-per-socket"] = CoresPerSocket.ToString();
            VM.set_platform(Session, VM.opaque_ref, platform);

            // Check these values have changed before setting them, as they are RBAC protected
            if (HomeServerChanged())
                XenAPI.VM.set_affinity(Session, VM.opaque_ref, HomeServer != null ? HomeServer.opaque_ref : Helper.NullOpaqueRef);

            if (Template.memory_dynamic_min != MemoryDynamicMin || Template.memory_dynamic_max != MemoryDynamicMax || Template.memory_static_max != MemoryStaticMax)
                XenAPI.VM.set_memory_limits(Session, VM.opaque_ref, Template.memory_static_min, MemoryStaticMax, MemoryDynamicMin, MemoryDynamicMax);
        }

        private bool HomeServerChanged()
        {
            if (HomeServer == null)
            {
                return Template.affinity.opaque_ref != Helper.NullOpaqueRef;
            }
            return HomeServer.opaque_ref != Template.affinity.opaque_ref;
        }

        private void SetVMBootParams()
        {
            if (Template.IsHVM() && (Disks.Count == 0 || InsMethod == InstallMethod.Network)) // CA-46213
            {
                // boot from network
                Dictionary<string, string> hvm_params = VM.HVM_boot_params;
                hvm_params["order"] = GetBootOrderNetworkFirst();
                XenAPI.VM.set_HVM_boot_params(Session, VM.opaque_ref, hvm_params);
            }
            else if (IsEli() && InsMethod == InstallMethod.Network)
            {
                Dictionary<string, string> other_config = VM.other_config;
                string normal_url = IsRhel() ? NormalizeRepoUrlForRHEL(Url) : Url;
                other_config["install-repository"] = normal_url;
                XenAPI.VM.set_other_config(Session, VM.opaque_ref, other_config);
            }
            else if (IsEli() && InsMethod == InstallMethod.CD)
            {
                Dictionary<string, string> other_config = VM.other_config;
                other_config["install-repository"] = "cdrom";
                XenAPI.VM.set_other_config(Session, VM.opaque_ref, other_config);
            }

            if (!Template.IsHVM())
            {
                XenAPI.VM.set_PV_args(Session, VM.opaque_ref, PvArgs);
            }
            else if (BootMode != BootMode.NOT_AVAILABLE)
            {
                var hvm_params = VM.HVM_boot_params;
                hvm_params["firmware"] = BootMode != BootMode.BIOS_BOOT ? "uefi" : "bios";
                XenAPI.VM.set_HVM_boot_params(Session, VM.opaque_ref, hvm_params);

                var platform = VM.platform;
                platform["secureboot"] = BootMode == BootMode.UEFI_SECURE_BOOT ? "true" : "false";
                XenAPI.VM.set_platform(Session, VM.opaque_ref, platform);
            }
        }

        private bool IsEli()
        {
            return !Template.IsHVM() && Template.PV_bootloader == "eliloader";
        }

        private bool IsRhel()
        {
            string distro = VM.InstallDistro();
            return distro == "rhel41" || distro == "rhel44" || distro == "rhlike";
        }

        private string NormalizeRepoUrlForRHEL(string url)
        {
            Uri uri = new Uri(url);
            return uri.Scheme == "nfs" ? string.Format("nfs:{0}:{1}", uri.Host, uri.PathAndQuery) : url;
        }

        private void ProvisionVM()
        {
            Description = Messages.PROVISIONING_VM;
            RewriteProvisionXML();
            RelatedTask = XenAPI.VM.async_provision(Session, VM.opaque_ref);

            PollToCompletion(10, 60);
        }

        private void RewriteProvisionXML()
        {
            XmlNode xml = VM.ProvisionXml();

            if (xml == null)
                return;

            // set the new vm's provision xml: remove "disks" entry, as we are going to explicitly create all the disks
            Dictionary<string, string> other_config = VM.other_config;
            other_config.Remove("disks");
            XenAPI.VM.set_other_config(Session, VM.opaque_ref, other_config);
        }

        private void AddCdDrive()
        {
            if (Helpers.CustomWithNoDVD(Template))
                return; // we have skipped the install media page because we are a cutom template with no cd drive - the user doesnt want a cd drive

            Description = Messages.CREATE_CD_DRIVE;
            VBD cd_drive = null;
            foreach (VBD vbd in Connection.ResolveAll(VM.VBDs))
            {
                if (vbd.type != vbd_type.CD)
                    continue;

                if ("0123".IndexOf(vbd.userdevice) < 0)  // userdevice is not 0, 1, 2 or 3: these are the valid positions for CD drives
                    continue;

                cd_drive = vbd;
                break;
            }

            if (cd_drive == null)
            {
                cd_drive = CreateCdDrive();
            }

            if (!cd_drive.empty)
            {
                RelatedTask = VBD.async_eject(Session, cd_drive.opaque_ref);
                PollToCompletion(65, 67);
            }

            if (InsMethod == InstallMethod.CD && Cd != null) // obviously dont insert the empty cd
            {
                RelatedTask = VBD.async_insert(Session, cd_drive.opaque_ref, Cd.opaque_ref);
                PollToCompletion(67, 70);
            }
        }

        private VBD CreateCdDrive()
        {
            List<string> devices = AllowedVBDs;
            if (devices.Count == 0)
                throw new Exception(Messages.NO_MORE_USERDEVICES);
            VBD vbd = new VBD
            {
                bootable = InsMethod == InstallMethod.CD,
                empty = true,
                unpluggable = true,
                mode = vbd_mode.RO,
                type = vbd_type.CD,
                userdevice = devices.Contains("3") ? "3" : devices[0],
                VM = new XenRef<VM>(VM.opaque_ref)
            };
            RelatedTask = VBD.async_create(Session, vbd);
            PollToCompletion(60, 65);

            return Connection.WaitForCache(new XenRef<VBD>(Result));
        }


        private void AddDisks()
        {
            Description = Messages.CREATING_DISKS;
            List<VBD> vbds = Connection.ResolveAll(VM.VBDs);

            bool firstDisk = true;
            string suspendSr = null;

            double progress = 70;
            double step = 20.0 / (double)Disks.Count;
            foreach (DiskDescription disk in Disks)
            {
                VBD vbd = GetDiskVBD(disk, vbds);
                VDI vdi = null;
                if (vbd != null)
                {
                    vdi = Connection.Resolve<VDI>(vbd.VDI);
                }
                if (!DiskOk(disk, vbd))
                {
                    if (vbd != null)
                        vdi = MoveDisk(disk, vbd, progress, step);
                    else
                        vdi = CreateDisk(disk, progress, step);
                }

                if (vdi == null)
                    continue;

                if (vdi.name_description != disk.Disk.name_description)
                    VDI.set_name_description(Session, vdi.opaque_ref, disk.Disk.name_description);
                if (vdi.name_label != disk.Disk.name_label)
                    VDI.set_name_label(Session, vdi.opaque_ref, disk.Disk.name_label);

                if (firstDisk)
                {
                    //use the first disk to set the VM.suspend_SR
                    SR vdiSR = Connection.Resolve(vdi.SR);
                    this.firstSR = vdiSR;
                    if(vdiSR != null && !vdiSR.HBALunPerVDI())
                        suspendSr = vdi.SR;
                    firstDisk = false;
                }

                progress += step;
            }

			VM.set_suspend_SR(Session, VM.opaque_ref, suspendSr);
        }

        private VBD GetDiskVBD(DiskDescription disk, List<VBD> vbds)
        {
            foreach (VBD vbd in vbds)
            {
                if (disk.Device.userdevice == vbd.userdevice)
                    return vbd;
            }
            return null;
        }

        private bool DiskOk(DiskDescription disk, VBD vbd)
        {
            if (vbd == null)
                return false;

            VDI vdi = Connection.Resolve(vbd.VDI);

            return vdi != null && disk.Disk.SR.opaque_ref == vdi.SR.opaque_ref;
        }

        private VDI MoveDisk(DiskDescription disk, VBD vbd, double progress, double step)
        {
            string old_vdi_ref = vbd.VDI.opaque_ref;

            RelatedTask = XenAPI.VDI.async_copy(Session, vbd.VDI.opaque_ref, disk.Disk.SR.opaque_ref);
            PollToCompletion(progress, progress + 0.25 * step);
            AddVMHint(Connection.WaitForCache(new XenRef<VDI>(Result)));


            VDI new_vdi = Connection.Resolve(new XenRef<VDI>(Result));

            RelatedTask = XenAPI.VBD.async_destroy(Session, vbd.opaque_ref);
            PollToCompletion(progress + 0.25 * step, progress + 0.5 * step);

            RelatedTask = XenAPI.VDI.async_destroy(Session, old_vdi_ref);
            PollToCompletion(progress + 0.5 * step, progress + 0.75 * step);

            CreateVbd(disk, new_vdi, progress + 0.75 * step, progress + step, IsDeviceAtPositionZero(disk));
            return new_vdi;
        }

        /// <summary>
        /// Helper: Check if the disk is at the zeroth position in the VBD list
        /// </summary>
        /// <param name="disk"></param>
        /// <returns></returns>
        private bool IsDeviceAtPositionZero(DiskDescription disk)
        {
            return disk.Device.userdevice == "0";
        }

        /// <summary>
        /// Create a VDI/disk. 
        /// If disk type is existing use the VDI in disk description 
        /// Otherwise create a new disk (provision it from the SR)
        /// </summary>
        /// <param name="disk"></param>
        /// <param name="progress"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private VDI CreateDisk(DiskDescription disk, double progress, double step)
        {
            VDI vdi;
            bool bootable = false;
            if(disk.Type == DiskDescription.DiskType.Existing)
                vdi = disk.Disk;
            else
            {
                vdi = CreateVdi(disk, progress, progress + 0.75 * step);
                bootable = IsDeviceAtPositionZero(disk) && InsMethod != InstallMethod.CD;
            }

            AddVMHint(vdi);
            CreateVbd(disk, vdi, progress + 0.75 * step, progress + step, bootable);
            return vdi;
        }

        private void AddVMHint(VDI vdi)
        {
            Dictionary<string, string> sm_config = VDI.get_sm_config(Session, vdi.opaque_ref);
            sm_config["vmhint"] = VM.opaque_ref;
            VDI.set_sm_config(Session, vdi.opaque_ref, sm_config);
        }

        private VDI CreateVdi(DiskDescription disk, double progress1, double progress2)
        {
            VDI vdi = new VDI();
            vdi.name_label = disk.Disk.name_label;
            vdi.name_description = disk.Disk.name_description;
            vdi.read_only = false;
            vdi.sharable = false;
            vdi.SR = disk.Disk.SR;
            vdi.type = disk.Disk.type;
            vdi.virtual_size = disk.Disk.virtual_size;
            vdi.sm_config = disk.Disk.sm_config;

            RelatedTask = XenAPI.VDI.async_create(Session, vdi);
            PollToCompletion(progress1, progress2);
            return Connection.WaitForCache(new XenRef<VDI>(Result));
        }

        /// <summary>
        /// Create a VBD
        /// 
        /// ** vbd.bootable **
        /// 1. Windows ignores bootable flag
        /// 2. Eliloader changes the device "0" to bootable when booting linux
        /// </summary>
        /// <param name="disk"></param>
        /// <param name="vdi"></param>
        /// <param name="progress1"></param>
        /// <param name="progress2"></param>
        /// <param name="bootable">Set VBD.bootable to this value - see comments above</param>
        private void CreateVbd(DiskDescription disk, VDI vdi, double progress1, double progress2, bool bootable)
        {
            List<string> devices = AllowedVBDs;
            if (devices.Count == 0)
                throw new Exception(Messages.NO_MORE_USERDEVICES);
            VBD vbd = new VBD();
            vbd.SetIsOwner(true);
            vbd.bootable = bootable;
            vbd.empty = false;
            vbd.unpluggable = true;
            vbd.mode = vbd_mode.RW;
            vbd.type = vbd_type.Disk;
            vbd.userdevice = devices.Contains(disk.Device.userdevice) ? disk.Device.userdevice : devices[0];
            vbd.device = "";
            vbd.VM = new XenRef<VM>(VM.opaque_ref);
            vbd.VDI = new XenRef<VDI>(vdi.opaque_ref);
            RelatedTask = VBD.async_create(Session, vbd);
            PollToCompletion(progress1, progress2);
            Connection.WaitForCache(new XenRef<VBD>(Result));
        }

        private void AddNetworks()
        {
            // first of all we need to clear any vifs that we have cloned from the template 
            double progress = 90;
            VIF vif;
            List<VIF> existingTemplateVifs = Connection.ResolveAll(VM.VIFs);
            double step = 5.0 / (double)existingTemplateVifs.Count;
            for (int i = 0; i < existingTemplateVifs.Count; i++)
            {
                vif = existingTemplateVifs[i];
                RelatedTask = XenAPI.VIF.async_destroy(Session, vif.opaque_ref);

                PollToCompletion(progress, progress + step);
                progress += step;
            }

            // then we add the ones the user has specified
            step = 5.0 / (double)Vifs.Count;
            for (int i = 0; i < Vifs.Count; i++)
            {
                vif = Vifs[i];
                List<string> devices = AllowedVIFs;
                VIF new_vif = new VIF();

                if (devices.Count < 1)
                {
                    // If we have assigned more VIFs than we have space for then don't try to create them
                    log.Warn("Tried to create more VIFs than the server allows. Ignoring remaining vifs");
                    return;
                }
                new_vif.device = devices.Contains(vif.device) ? vif.device : devices[0];
                new_vif.MAC = vif.MAC;
                new_vif.network = vif.network;
                new_vif.VM = new XenRef<VM>(VM.opaque_ref);
                new_vif.qos_algorithm_type = vif.qos_algorithm_type;
                new_vif.qos_algorithm_params = vif.qos_algorithm_params;
                RelatedTask = XenAPI.VIF.async_create(Session, new_vif);

                PollToCompletion(progress, progress + step);
                progress += step;

                Connection.WaitForCache(new XenRef<VIF>(Result));
            }
        }

        private string HiddenVmName
        {
            get
            {
                return Helpers.MakeHiddenName(NameLabel);
            }
        }

        private List<string> AllowedVBDs
        {
            get
            {
                return new List<String>(XenAPI.VM.get_allowed_VBD_devices(Session, VM.opaque_ref));
            }
        }

        private List<string> AllowedVIFs
        {
            get
            {
                return new List<String>(XenAPI.VM.get_allowed_VIF_devices(Session, VM.opaque_ref));
            }
        }

        protected override void CleanOnError()
        {
            if (VM != null && !PointOfNoReturn && Connection.IsConnected)
            {
                try
                {
                    VMDestroyAction.DestroyVM(Session, VM, true);
                }
                catch (Exception e)
                {
                    // if the clean up has failed for whatever reason we just log it and give up.
                    log.Error(e);
                }
            }
        }

        private string GetBootOrderNetworkFirst()
        {
            // add "n" at the beginning of the order string
            if (VM.HVM_boot_params.ContainsKey("order"))
            {
                string order = VM.HVM_boot_params["order"].ToLower();
                int i = order.IndexOf("n");
                switch (i)
                {
                    case -1: return order.Insert(0, "n");
                    case 0: return order;
                    default: return order.Remove(i, 1).Insert(0, "n");
                }
            }
            else
            {
                return "ncd";
            }
        }
    }
    public class DiskDescription
    {
        public VDI Disk;
        public VBD Device;
        public DiskType Type;
        public enum DiskType { New, Existing }

        public DiskDescription(){}

        public DiskDescription(VDI disk, VBD device)
        {
            Disk = disk;
            Device = device;
            Type = DiskType.New;
        }
    }
}
