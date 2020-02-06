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
using System.Text;
using System.Threading;
using DiscUtils;
using XenOvf;
using XenOvf.Definitions;
using XenOvf.Utilities;
using XenAPI;
using System.Linq;


namespace XenOvfTransport
{
    public class Export : XenOvfTransportBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const long KB = 1024;
        private const long MB = (KB * 1024);
        private const long GB = (MB * 1024);
        private const long MEMMIN = 128 * MB;

        private List<XenRef<VDI>> _vdiRefs = new List<XenRef<VDI>>();

        public Export(Uri xenserver, Session session)
            : base(xenserver, session)
        {
        }

        public EnvelopeType Process(string targetPath, string ovfname, string[] vmUuid)
        {
            List<EnvelopeType> envList = new List<EnvelopeType>();

            foreach (string vmuuid in vmUuid)
                envList.Add(_export(XenSession, targetPath, ovfname, vmuuid));

            EnvelopeType ovfEnv = OVF.Merge(envList, ovfname);

			if (AutoSave)
            {
                string ovffilename = Path.Combine(targetPath, string.Format(@"{0}.ovf", ovfname));
                OVF.SaveAs(ovfEnv, ovffilename);
            }
        	OnUpdate(new XenOvfTransportEventArgs(TransportStep.Export, Messages.COMPLETED_EXPORT));
            return ovfEnv;
        }

		public bool AutoSave { get; set; }

        public bool ShouldVerifyDisks { get; set; }

		public bool MetaDataOnly { get; set; }

        private EnvelopeType _export(Session xenSession, string targetPath, string ovfname, string vmUuid)
        {
        	EnvelopeType ovfEnv;

            try
            {
                log.DebugFormat("Export: {0}, {1}", ovfname, targetPath);

                #region GET VM Reference
                XenRef<VM> vmRef = null;

                try
                {
                    vmRef = VM.get_by_uuid(xenSession, vmUuid);
                }
                catch
                {
                    log.WarnFormat("VM not found as uuid: {0}, trying as name-label", vmUuid);
                    vmRef = null;
                }
                if (vmRef == null)
                {
					try
					{
						List<XenRef<VM>> vmRefs = VM.get_by_name_label(xenSession, vmUuid);
						vmRef = vmRefs[0];
                        log.DebugFormat("{0} VM(s) found by label {1}", vmRefs.Count, vmUuid);
                        if (vmRefs.Count > 1)
                            log.WarnFormat("Only exporting FIRST VM with name {0}", vmUuid);
					}
					catch
					{
						log.ErrorFormat("Failed to find VM {0}.", vmUuid);
						throw;
					}
                }
                #endregion

                VM vm = VM.get_record(xenSession, vmRef);

                if (vm.power_state != vm_power_state.Halted && vm.power_state != vm_power_state.Suspended)
                {
                	var message = string.Format(Messages.ERROR_VM_NOT_HALTED, vm.Name());
                	OnUpdate(new XenOvfTransportEventArgs(TransportStep.Export, message));
                    log.Info($"VM {vm.Name()} ({vmRef.opaque_ref}) is neither halted nor suspended.");
                    throw new Exception(message);
                }

                #region CREATE ENVELOPE / ADD VIRTUAL SYSTEM
                ovfEnv = OVF.CreateEnvelope(ovfname);
                string vsId = OVF.AddVirtualSystem(ovfEnv, vm.name_label);
                string vhsId = OVF.AddVirtualHardwareSection(ovfEnv, vsId);
                #endregion

                #region TRY TO ID OS
                XenRef<VM_guest_metrics> vmgmRef = VM.get_guest_metrics(xenSession, vmRef);
				if (!vmgmRef.opaque_ref.ToUpper().Contains("NULL"))
                {
                    VM_guest_metrics vmgm = VM_guest_metrics.get_record(xenSession, vmgmRef);
                    //VM_metrics vmm = VM_metrics.get_record(xenSession, VM.get_metrics(xenSession, vmRef));
                    if (vmgm.os_version != null && vmgm.os_version.Count > 0)
                    {
                        foreach (string key in vmgm.os_version.Keys)
                        {
                            if (key.ToLower().Equals("name"))
                            {
                                ushort osid = ValueMaps.OperatingSystem(vmgm.os_version[key]);
                                if (osid == 0xFFFF) { osid = 1; } // change to OTHER since search failed.
                                string version = OVF.GetContentMessage("SECTION_OPERATINGSYSTEM_INFO");
                                if (vmgm.os_version.ContainsKey("major") &&
                                    vmgm.os_version.ContainsKey("minor"))
                                {
                                    version = string.Format(OVF.GetContentMessage("SECTION_OPERATINGSYSTEM_VERSION"), vmgm.os_version["major"], vmgm.os_version["minor"]);
                                }
                                string osname = (vmgm.os_version[key].Split(new [] { '|' }))[0];
                                OVF.UpdateOperatingSystemSection(ovfEnv, vsId, osname, version, osid);
                                break;
                            }
                        }
                    }
                }
                #endregion

                #region ADD VSSD
                // IS PV'd? for VirtualSystemType identification.
                string typeformat = @"{0}-{1}-{2}";
                string vmtype = string.Format(typeformat, "hvm", "3.0", "unknown");
                if (vm.HVM_boot_policy != null && vm.HVM_boot_policy == Properties.Settings.Default.xenBootOptions)
                {
                    if (!string.IsNullOrEmpty(vm.domarch))
                    {
                        vmtype = string.Format(typeformat, vm.domarch, "3.0", "unknown");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(vm.domarch))
                    {
                        vmtype = string.Format(typeformat, "xen", "3.0", vm.domarch);
                    }
                    else
                    {
                        vmtype = string.Format(typeformat, "xen", "3.0", "unknown");
                    }
                }
                OVF.AddVirtualSystemSettingData(ovfEnv, vsId, vhsId, vm.name_label, OVF.GetContentMessage("VSSD_CAPTION"), vm.name_description, Guid.NewGuid().ToString(), vmtype);
                #endregion

                #region ADD CPUS
                OVF.SetCPUs(ovfEnv, vsId, (ulong)vm.VCPUs_max);
                #endregion

                #region ADD MEMORY
                OVF.SetMemory(ovfEnv, vsId, (ulong)(vm.memory_dynamic_max / MB), "MB");
                #endregion

                #region ADD NETWORKS
                List<XenRef<VIF>> vifs = VM.get_VIFs(xenSession, vmRef);
                foreach (XenRef<VIF> vifref in vifs)
                {
                    VIF vif = VIF.get_record(xenSession, vifref);
                    XenRef<Network> netRef = vif.network;
                    Network net = Network.get_record(xenSession, netRef);

                    // Why is the following call reference using name_label where other references use uuid?
                    OVF.AddNetwork(ovfEnv, vsId, net.uuid, net.name_label, net.name_description, vif.MAC);
                }
                #endregion

				#region SET STARTUP OPTIONS
				OVF.AddStartupSection(ovfEnv, true, vsId, vm.order, vm.start_delay, vm.shutdown_delay);
				#endregion

				#region GET AND EXPORT DISKS using iSCSI
				List<XenRef<VBD>> vbdlist = VM.get_VBDs(xenSession, vmRef);
                _vdiRefs.Clear();

                int diskIndex = 0;

                foreach (XenRef<VBD> vbdref in vbdlist)
                {
                    VBD vbd = VBD.get_record(xenSession, vbdref);

                    if (vbd.type == vbd_type.CD)
                    {
                        string rasdid = OVF.AddCDROM(ovfEnv, vsId, vbd.uuid, OVF.GetContentMessage("RASD_16_CAPTION"), OVF.GetContentMessage("RASD_16_DESCRIPTION"));
                        OVF.SetTargetDeviceInRASD(ovfEnv, vsId, rasdid, vbd.userdevice);
                    }
                    else
                    {
                        try
                        {
                            XenRef<VDI> vdi = VBD.get_VDI(xenSession, vbdref);
                            if (vdi != null && !string.IsNullOrEmpty(vdi.opaque_ref) && !(vdi.opaque_ref.ToLower().Contains("null")))
                            {
                                _vdiRefs.Add(vdi);
                                VDI lVdi = VDI.get_record(xenSession, vdi);
                                string destinationFilename = Path.Combine(targetPath, string.Format(@"{0}.vhd", lVdi.uuid));
                                string diskid = Guid.NewGuid().ToString();

                                string diskName = lVdi.name_label;

                                if (diskName == null)
                                    diskName = string.Format("{0} {1}", OVF.GetContentMessage("RASD_19_CAPTION"), diskIndex);

                                OVF.AddDisk(ovfEnv, vsId, diskid, Path.GetFileName(destinationFilename), vbd.bootable, diskName, lVdi.name_description, (ulong)lVdi.physical_utilisation, (ulong)lVdi.virtual_size);
                                OVF.SetTargetDeviceInRASD(ovfEnv, vsId, diskid, vbd.userdevice);

                                diskIndex++;
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Info($"Export: VBD {vbdref} Skipped.", ex);
                        }
                    }
                }
                #endregion

				if (!MetaDataOnly)
                {
                    _copydisks(ovfEnv, ovfname, targetPath);
                }

				#region ADD XEN SPECIFICS

				var _params = vm.HVM_boot_params;
				if (_params != null && _params.Count > 0)
                {
                    OVF.AddOtherSystemSettingData(ovfEnv, vsId, "HVM_boot_params", string.Join(";", _params.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_6"));
                }
                if (!string.IsNullOrEmpty(vm.HVM_boot_policy))
                {
                    OVF.AddOtherSystemSettingData(ovfEnv, vsId, "HVM_boot_policy", vm.HVM_boot_policy, OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_2"));
                }
                if (vm.HVM_shadow_multiplier != 1.0)
                {
					OVF.AddOtherSystemSettingData(ovfEnv, vsId, "HVM_shadow_multiplier", Convert.ToString(vm.HVM_shadow_multiplier), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }
	            var platform = vm.platform;
				if (platform != null && platform.Count > 0)
                {
					OVF.AddOtherSystemSettingData(ovfEnv, vsId, "platform", string.Join(";", platform.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_3"));
                }
				var nvram = vm.NVRAM;
                if (nvram != null && nvram.Count > 0)
				{
                    OVF.AddOtherSystemSettingData(ovfEnv, vsId, "NVRAM", string.Join(";", nvram.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_7"));
                }
                if (!string.IsNullOrEmpty(vm.PV_args))
                {
					OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_args", vm.PV_args, OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }
                if (!string.IsNullOrEmpty(vm.PV_bootloader))
                {
					OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_bootloader", vm.PV_bootloader, OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }
                if (!string.IsNullOrEmpty(vm.PV_bootloader_args))
                {
					OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_bootloader_args", vm.PV_bootloader_args, OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }
                if (!string.IsNullOrEmpty(vm.PV_kernel))
                {
					OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_kernel", vm.PV_kernel, OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }
                if (!string.IsNullOrEmpty(vm.PV_legacy_args))
                {
					OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_legacy_args", vm.PV_legacy_args, OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }
                if (!string.IsNullOrEmpty(vm.PV_ramdisk))
                {
					OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_ramdisk", vm.PV_ramdisk, OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }

                if (vm.hardware_platform_version >= 0)
                {
                    OVF.AddOtherSystemSettingData(ovfEnv, vsId, "hardware_platform_version", vm.hardware_platform_version.ToString(), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }

                if (!string.IsNullOrEmpty(vm.recommendations))
                {
                    OVF.AddOtherSystemSettingData(ovfEnv, vsId, "recommendations", vm.recommendations.ToString(), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }

                if (vm.has_vendor_device)
                {
                    //serialise it with a different name to avoid it being deserialised automatically and getting the wrong type
                    OVF.AddOtherSystemSettingData(ovfEnv, vsId, "VM_has_vendor_device", vm.has_vendor_device.ToString(), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }

                foreach(XenRef<VGPU> gpuRef in vm.VGPUs)
                {
                    VGPU vgpu = VGPU.get_record(xenSession, gpuRef);

                    if (vgpu != null)
                    {
                        var vgpuGroup = GPU_group.get_record(xenSession, vgpu.GPU_group);
                        var vgpuType = VGPU_type.get_record(xenSession, vgpu.type);

                        var sb = new StringBuilder();
                        sb.AppendFormat("GPU_types={{{0}}};",
                                        vgpuGroup.GPU_types == null || vgpuGroup.GPU_types.Length < 1
                                            ? ""
                                            : string.Join(";", vgpuGroup.GPU_types));
                        sb.AppendFormat("VGPU_type_vendor_name={0};", vgpuType.vendor_name ?? "");
                        sb.AppendFormat("VGPU_type_model_name={0};", vgpuType.model_name ?? "");
                        OVF.AddOtherSystemSettingData(ovfEnv, vsId, "vgpu", sb.ToString(), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_4"), true);
                    }
                }

                string pvsSiteUuid = string.Empty;
                var allProxies = xenSession.Connection.Cache.PVS_proxies;                

                foreach (var p in allProxies.Where(p => p != null && p.VIF != null))
                {
                    var vif = xenSession.Connection.Resolve(p.VIF);
                    if (vif != null)
                    {
                        var vmFromVif = xenSession.Connection.Resolve(vif.VM);
                        if (vmFromVif != null && vmFromVif.uuid == vm.uuid)
                        {
                            var pvsSite = xenSession.Connection.Resolve(p.site);
                            if (pvsSite != null)
                            {
                                pvsSiteUuid = pvsSite.uuid;
                            }

                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(pvsSiteUuid))
                {
                    var sb = new StringBuilder();
                    sb.AppendFormat("PVS_SITE={{{0}}};", string.Format("uuid={0}", pvsSiteUuid));

                    OVF.AddOtherSystemSettingData(ovfEnv, vsId, "pvssite", sb.ToString(), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_5"));
                }
                #endregion

                OVF.FinalizeEnvelope(ovfEnv);
            }
            catch (Exception ex)
            {
				if (ex is OperationCanceledException)
					throw;
                log.Error("Export failed", ex);
                throw new Exception(Messages.ERROR_EXPORT_FAILED, ex);
            }
            return ovfEnv;
        }

        private void _copydisks(EnvelopeType ovfEnv, string label, string targetPath)
        {
        	m_iscsi = new iSCSI
        	          	{
        	          		UpdateHandler = iscsi_UpdateHandler,
        	          		Cancel = Cancel//in case it has already been cancelled
        	          	};
			m_iscsi.ConfigureTvmNetwork(m_networkUuid, m_isTvmIpStatic, m_tvmIpAddress, m_tvmSubnetMask, m_tvmGateway);

            try
            {
                foreach (XenRef<VDI> vdiuuid in _vdiRefs)
                {
                    string uuid = "";
                    string destinationFilename = "";

                    try
                    {
                        uuid = VDI.get_uuid(XenSession, vdiuuid);
                        destinationFilename = Path.Combine(targetPath, string.Format(@"{0}.vhd", uuid));
                        
                        if (File.Exists(destinationFilename))
                        {
                            destinationFilename = Path.Combine(targetPath, string.Format(@"{0}_{1}.vhd", uuid, Thread.CurrentThread.ManagedThreadId));
                            OVF.UpdateFilename(ovfEnv, string.Format(@"{0}.vhd", uuid), string.Format(@"{0}_{1}.vhd", uuid, Thread.CurrentThread.ManagedThreadId));
                            log.InfoFormat("{0}: VHD Name collision, renamed {1}.vhd to {1}_{2}.vhd",
                                           label, uuid, Thread.CurrentThread.ManagedThreadId);
                        }

                        OnUpdate(new XenOvfTransportEventArgs(TransportStep.Export, string.Format(Messages.FILES_TRANSPORT_SETUP, uuid + ".vhd")));
						
                        using (Stream source = m_iscsi.Connect(XenSession, uuid, true))
                        {
                            OnUpdate(new XenOvfTransportEventArgs(TransportStep.Export, ""));
                            using (FileStream fs = new FileStream(destinationFilename, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
                            {
                                // Create a geometry to give to DiscUtils.Vhd.Disk.InitializeDynamic() just so it doesn't override capacity
                                // when initializing the .
                                DiscUtils.Geometry geometry = DiscUtils.Geometry.FromCapacity(source.Length);

                                using (DiscUtils.Vhd.Disk destination = DiscUtils.Vhd.Disk.InitializeDynamic(fs, Ownership.None, source.Length, geometry))
                                {
									m_iscsi.Copy(source, destination.Content, Path.GetFileName(destinationFilename), ShouldVerifyDisks);
                                }
                            }
                        }

                        if (ShouldVerifyDisks)
                        {
                            using (var target = new DiscUtils.Vhd.Disk(destinationFilename, FileAccess.Read))
                            {
								m_iscsi.Verify(target.Content, destinationFilename);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
						if (ex is OperationCanceledException)
							throw;
                        var msg = string.Format(Messages.ISCSI_COPY_ERROR, destinationFilename);
                        log.Error($"Failed to transfer virtual disk {destinationFilename}", ex);
                        throw new Exception(msg, ex);
                    }
                    finally
                    {
                        OnUpdate(new XenOvfTransportEventArgs(TransportStep.Export, string.Format(Messages.FILES_TRANSPORT_CLEANUP, uuid + ".vhd")));
						m_iscsi.Disconnect(XenSession);
                    }
                }
            }
            finally
            {
                _vdiRefs.Clear();
            }
        }

		private void iscsi_UpdateHandler(XenOvfTransportEventArgs e)
		{
			OnUpdate(e);
		}
    }
}
