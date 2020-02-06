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
using System.Xml;
using XenOvf.Definitions;
using XenOvf.Utilities;

namespace XenOvf
{
    public partial class OVF
    {
        /// <summary>
        /// Binary flags describing things to validate
        /// </summary>
        [Flags]
        private enum ValidationFlags
        {
            /// <summary>
            /// No validation
            /// </summary>
            None = 0,
            /// <summary>
            /// Check the OVF Version (if present)
            /// </summary>
            Version = 1,
            /// <summary>
            /// Ensure the files listed in References.Files are present and accessible
            /// </summary>
            Files = 2,
            /// <summary>
            /// Ensure a CPU RASD is present
            /// </summary>
            Cpu = 4,
            /// <summary>
            /// Ensure a Memory RASD is present
            /// </summary>
            Memory = 8,
            /// <summary>
            /// Validate Network RASD is connected to a NETWORK
            /// </summary>
            Networks = 16,
            /// <summary>
            /// Ensure import can handle required RASDs and Sections
            /// </summary>
            Capability = 32,
            /// <summary>
            /// Validate the OVF XML against the Schema
            /// </summary>
            Schema = 64
        };


        public static bool Validate(Package package, out List<string> warnings)
        {
            warnings = new List<string>();
            var ovfEnv = package.OvfEnvelope;

            if (ovfEnv == null)
            {
                warnings.Add(Messages.VALIDATION_INVALID_OVF);
                return false;
            }

            log.InfoFormat("Started validation of package {0}", package.Name);

            var validationFlags = (ValidationFlags)Properties.Settings.Default.RequiredValidations;

            if (validationFlags.HasFlag(ValidationFlags.Version))
                ValidateVersion(ovfEnv, ref warnings);
            if (validationFlags.HasFlag(ValidationFlags.Schema))
                ValidateSchema(package.DescriptorXml, ref warnings);

            VirtualDiskDesc_Type[] disks = null;
            NetworkSection_TypeNetwork[] networks = null;

            foreach (var section in ovfEnv.Sections)
            {
                if (section is DiskSection_Type diskSection)
                    disks = diskSection.Disk;
                else if (section is NetworkSection_Type netSection)
                    networks = netSection.Network;
            }

            var systems = new Content_Type[0];
            switch (ovfEnv.Item)
            {
                case VirtualSystemCollection_Type vsCol:
                    systems = vsCol.Content;
                    break;
                case VirtualSystem_Type _:
                    systems = new [] { ovfEnv.Item };
                    break;
                default:
                    log.Error($"OVF envelope item type {ovfEnv.Item.GetType()} is not recognized.");
                    warnings.Add(string.Format(Messages.VALIDATION_INVALID_TYPE, ovfEnv.Item.GetType()));
                    break;
            }

            foreach (var system in systems)
            {
                foreach (var section in system.Items)
                {
                    if (!(section is VirtualHardwareSection_Type vhs))
                        continue;

                    var hardwareType = vhs.System?.VirtualSystemType?.Value;
                    if (hardwareType != null && !Properties.Settings.Default.knownVirtualSystemTypes.Contains(hardwareType))
                    {
                        log.Warn($"Found unexpected virtual hardware type {hardwareType}.");
                        warnings.Add(string.Format(Messages.VALIDATION_UNKNOWN_HARDWARE_TYPE, hardwareType));
                    }

                    RASD_Type[] rasds = vhs.Item;

                    if (validationFlags.HasFlag(ValidationFlags.Files) && !ValidateDisks(package, disks, rasds, ref warnings))
                        return false;

                    foreach (RASD_Type rasd in rasds)
                    {
                        if (rasd.ResourceType.Value == 3 && validationFlags.HasFlag(ValidationFlags.Cpu))
                            ValidateCpu(rasd, ref warnings);

                        if (rasd.ResourceType.Value == 4 && validationFlags.HasFlag(ValidationFlags.Memory))
                            ValidateMemory(rasd, ref warnings);

                        if (rasd.ResourceType.Value == 10 && validationFlags.HasFlag(ValidationFlags.Networks))
                            ValidateNetworks(rasd, networks, ref warnings);

                        if (validationFlags.HasFlag(ValidationFlags.Capability))
                            ValidateCapability(rasd, ref warnings);
                    }
                }
            }

            log.InfoFormat("Finished validation of package {0}", package.Name);
            return true;
        }

        
		private static void ValidateVersion(EnvelopeType ovfEnv, ref List<string> warnings)
        {
            log.Info("Validating OVF version");

            if (ovfEnv.version == null && ovfEnv.AnyAttr != null && ovfEnv.AnyAttr.Length > 0)
            {
                foreach (XmlAttribute attr in ovfEnv.AnyAttr)
                {
                    if (attr.Name.ToLower().Contains("version"))
                    {
                        ovfEnv.version = attr.Value;
                        break;
                    }
                }
            }

            if (ovfEnv.version == null)
            {
                log.Warn("OVF version not set, assuming 1.0.0");
                warnings.Add(string.Format(Messages.VALIDATION_VERSION_UNSET, ovfEnv.version));
                ovfEnv.version = "1.0.0";
            }
            else if (!Properties.Settings.Default.Versions.Contains(ovfEnv.version))
            {
                warnings.Add(string.Format(Messages.VALIDATION_VERSION_INVALID, ovfEnv.version));
                log.WarnFormat($"OVF version {ovfEnv.version} is not supported.");
            }
        }

        private static bool ValidateDisks(Package package, VirtualDiskDesc_Type[] disks, RASD_Type[] rasds, ref List<string> warnings)
        {
            log.Info("Validating disks");

            bool isValid = true;
            File_Type[] files = package.OvfEnvelope?.References?.File ?? new File_Type[0];

            foreach (File_Type file in files)
            {
				string ext = Path.GetExtension(file.href).ToLower();
				if (ext == Properties.Settings.Default.manifestFileExtension || ext == Properties.Settings.Default.certificateFileExtension)
					continue;

                if (package.HasFile(file.href))
                {
                    if (!Properties.Settings.Default.knownFileExtensions.ToLower().Contains(ext))
                    {
                        warnings.Add(string.Format(Messages.VALIDATION_FILE_UNSUPPORTED_EXTENSION, file.href));
                        continue;
                    }

                    bool validlink = false;
                    foreach (VirtualDiskDesc_Type disk in disks)
                    {
                        if (file.id == disk.fileRef)
                        {
                            foreach (RASD_Type rasd in rasds)
                            {
                                if (rasd.ResourceType.Value == 17 || rasd.ResourceType.Value == 19 ||
                                    rasd.ResourceType.Value == 20 || rasd.ResourceType.Value == 21)
                                {
                                    if (rasd.HostResource != null && rasd.HostResource.Length > 0)
                                    {
                                        if (rasd.HostResource[0].Value.Contains(disk.diskId))
                                        {
                                            validlink = true;
                                            break;
                                        }
                                    }
                                    else if (disk.diskId == rasd.InstanceID.Value)
                                    {
                                        validlink = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (!validlink)
                    {
                        log.WarnFormat("Disk linkage (file to RASD) does not exist for {0}", file.href);
                        warnings.Add(string.Format(Messages.VALIDATION_FILE_INVALID_LINKAGE, file.href));
                    }
                }
                else
                {
                    warnings.Add(string.Format(Messages.VALIDATION_FILE_NOT_FOUND, file.href));
                    log.Error($"Failed to find file {file.href} listed in the reference section.");
                    isValid = false;
                }
            }

            return isValid;
        }

        private static void ValidateCpu(RASD_Type rasd, ref List<string> warnings)
        {
            log.Info("Validating CPU");

            if (rasd.VirtualQuantity == null || rasd.VirtualQuantity.Value <= 0)
            {
                log.Warn("CPU invalid Virtual Quantity");
                warnings.Add(Messages.VALIDATION_CPU_INVALID_QUANTITY);
            }
            else if (rasd.Limit != null && rasd.VirtualQuantity.Value > rasd.Limit.Value)
            {
                log.WarnFormat("Processor quantity {0} exceeds the limit of {1}.", rasd.VirtualQuantity.Value, rasd.Limit.Value);
                warnings.Add(string.Format(Messages.VALIDATION_CPU_EXCEEDS_LIMIT, rasd.VirtualQuantity.Value, rasd.Limit.Value));
            }
            else if (rasd.InstanceID == null || rasd.InstanceID.Value.Length <= 0)
            {
                log.Info("CPU has an invalid InstanceID, creating a new one.");
                warnings.Add(Messages.VALIDATION_INVALID_INSTANCEID);//ovf is valid
                rasd.InstanceID = new cimString(Guid.NewGuid().ToString());
            }
        }

        private static void ValidateMemory(RASD_Type rasd, ref List<string> warnings)
        {
            log.Info("Validating Memory");

            if (rasd.VirtualQuantity == null || rasd.VirtualQuantity.Value <= 0)
            {
                log.Warn("Memory invalid Virtual Quantity");
                warnings.Add(Messages.VALIDATION_INVALID_MEMORY_QUANTITY);
            }
            else if (rasd.AllocationUnits == null || rasd.AllocationUnits.Value.Length <= 0)
            {
                log.Warn("Memory AllocationUnits not valid");
                warnings.Add(Messages.VALIDATION_INVALID_MEMORY_ALLOCATIONUNITS);
            }
            else if (rasd.InstanceID == null || rasd.InstanceID.Value.Length <= 0)
            {
                log.Info("Memory has an invalid InstanceID, creating a new one.");
                warnings.Add(Messages.VALIDATION_INVALID_INSTANCEID);
                rasd.InstanceID = new cimString(Guid.NewGuid().ToString());//ovf is valid
            }
        }

        private static void ValidateNetworks(RASD_Type rasd, NetworkSection_TypeNetwork[] networks, ref List<string> warnings)
        {
            log.Info("Validating Networks");

            if (rasd.InstanceID == null || rasd.InstanceID.Value.Length <= 0)
            {
                log.Info("Network has an invalid InstanceID, creating a new one.");
                warnings.Add(Messages.VALIDATION_INVALID_INSTANCEID);
                rasd.InstanceID = new cimString(Guid.NewGuid().ToString());
                return;
            }

            bool linkage = false;
            foreach (NetworkSection_TypeNetwork net in networks)
            {
                // TODO: this may only work for Citrix created VIFs; for others we may need to use a different key to validate linkage.

                if (rasd.Connection != null && rasd.Connection.Length > 0 && net.name == rasd.Connection[0].Value ||
                    net.Description.msgid == rasd.InstanceID.Value)
                {
                    linkage = true;
                }
            }

            if (!linkage)
            {
                log.Error(Messages.VALIDATION_NETWORK_NO_DEVICE);
                warnings.Add(Messages.VALIDATION_NETWORK_NO_DEVICE);
            }
        }

        private static void ValidateCapability(RASD_Type rasd, ref List<string> warnings)
        {
            log.Info("Validating Capabilities");

            switch (rasd.ResourceType.Value)
            {
                // CIM SCHEMA 2.19.0
                case 3:  // Processor
                case 4:  // Memory
                case 5:  // IDE Controller
                case 6:  // Parallel SCSI HBA
                case 7:  // FC HBA
                case 8:  // iSCSI HBA
                case 9:  // IB HCA
                case 10: // Ethernet Adapter
                case 15: // CD Drive
                case 16: // DVD Drive
                case 17: // Disk Drive
                case 19: // Storage Extent
                case 20: // Other storage Device
                case 21: // Serial Port  // Microsoft uses this for Hard Disk Image also, based on an OLDER schema
                    break;
                case 1:  // Other
                case 2:  // Computer System
                case 11: // Other Network Adapter
                case 12: // I/O Slot
                case 13: // I/O Device
                case 14: // Floppy Drive
                case 18: // Tape Drive
                case 22: // Parallel Port
                case 23: // USB Controller
                case 24: // Graphics Controller
                case 25: // IEEE 1394 Controller
                case 26: // Partitionable Unit
                case 27: // Base Partitionable Unit
                case 28: // Power
                case 29: // Cooling Capacity
                case 30: // Ethernet Switch Port
                case 31: // Logical Disk
                case 32: // Storage Volume
                case 33: // Ethernet Connection
                default:
                    if (rasd.required)
                    {
                        var message = string.Format(Messages.VALIDATION_REQUIRED_ELEMENT_NOT_RECOGNIZED, rasd.ResourceType.Value, rasd.ElementName.Value);
                        log.Error(message);
                        warnings.Add(message);
                    }
                    break;
            }
        }

        private static void ValidateSchema(string ovfContent, ref List<string> warnings)
        {
            log.Info("Validating OVF XML schema");

            try
            {
                Tools.ValidateXmlToSchema(ovfContent);
            }
            catch (Exception ex)
            {
                log.Warn("OVF descriptor does not comply with the OVF XML schema.", ex);
                warnings.Add(Messages.VALIDATION_SCHEMA_FAILED);
            }
        }
    }
}
