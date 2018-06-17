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
using XenAdmin.Actions;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Wizards.NewSRWizard_Pages
{
    public class SrDescriptor
    {
        public SrDescriptor()
        {
            DeviceConfig = new Dictionary<string, string>();
            SMConfig = new Dictionary<string, string>();
        }

        public Dictionary<string, string> DeviceConfig { get; set; }
        public Dictionary<string, string> SMConfig { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string UUID { get; set; }
    }

    public abstract class FibreChannelDescriptor : SrDescriptor
    {
        protected FibreChannelDescriptor(FibreChannelDevice device, string descrFormat)
        {
            Device = device;
            Description = string.Format(descrFormat, device.Vendor, device.Serial);
        }

        public abstract SR.SRTypes SrType { get; }

        public FibreChannelDevice Device { get; private set; }
    }

    public class LvmOhbaSrDescriptor : FibreChannelDescriptor
    {
        public LvmOhbaSrDescriptor(FibreChannelDevice device)
            : base(device, Messages.NEWSR_LVMOHBA_DESCRIPTION)
        {
            DeviceConfig["SCSIid"] = device.SCSIid;
        }

        public override SR.SRTypes SrType
        {
            get { return SR.SRTypes.lvmohba; }
        }
    }

    public class FcoeSrDescriptor : FibreChannelDescriptor
    {
        public FcoeSrDescriptor(FibreChannelDevice device)
            : base(device, Messages.NEWSR_LVMOFCOE_DESCRIPTION)
        {
            DeviceConfig["SCSIid"] = device.SCSIid;
            DeviceConfig["path"] = device.Path;
        }

        public override SR.SRTypes SrType
        {
            get { return SR.SRTypes.lvmofcoe; }
        }
    }

    public class Gfs2HbaSrDescriptor : FibreChannelDescriptor
    {
        public Gfs2HbaSrDescriptor(FibreChannelDevice device)
            : base(device, Messages.NEWSR_LVMOHBA_DESCRIPTION)
        {
            DeviceConfig["provider"] = "hba";
            DeviceConfig["SCSIid"] = device.SCSIid;
        }

        public override SR.SRTypes SrType
        {
            get { return SR.SRTypes.gfs2; }
        }
    }

    public class Gfs2FcoeSrDescriptor : FibreChannelDescriptor
    {
        public Gfs2FcoeSrDescriptor(FibreChannelDevice device)
            : base(device, Messages.NEWSR_LVMOFCOE_DESCRIPTION)
        {
            DeviceConfig["provider"] = "fcoe";
            DeviceConfig["SCSIid"] = device.SCSIid;
            DeviceConfig["path"] = device.Path;
        }

        public override SR.SRTypes SrType
        {
            get { return SR.SRTypes.gfs2; }
        }
    }

    public abstract class SrWizardType
    {
        protected SrWizardType()
        {
            SrDescriptors = new List<SrDescriptor>();
        }

        public virtual IEnumerable<string> Errors { get { return new string[] { }; } }

        /// <summary>
        /// Floodgate: Whether this SR is unable to be created in the free version.
        /// </summary>
        public abstract bool IsEnhancedSR { get; }
        /// <summary>
        /// CA-16955: New SR wizard could show blurb for each backend type
        /// </summary>
        public abstract string FrontendBlurb { get; }
        public abstract string FrontendTypeName { get; }
        public abstract SR.SRTypes Type { get; }
        public abstract string ContentType { get; }
        public abstract bool ShowIntroducePrompt { get; }
        public abstract bool ShowReattachWarning { get; }
        public abstract bool AllowToCreateNewSr { get; set; }
        public virtual bool IsGfs2 { get { return false; } set { } }

        public string SrName
        {
            get { return SrDescriptors.Count > 0 ? SrDescriptors[0].Name : null; }
            set
            {
                if (SrDescriptors.Count == 0)
                    SrDescriptors.Add(new SrDescriptor());
                SrDescriptors[0].Name = value;
            }
        }
        public string UUID
        {
            get { return SrDescriptors.Count > 0 ? SrDescriptors[0].UUID : null; }
            set
            {
                if (SrDescriptors.Count == 0)
                    SrDescriptors.Add(new SrDescriptor());
                SrDescriptors[0].UUID = value;
            }
        }
        public Dictionary<String, String> DeviceConfig
        {
            get { return SrDescriptors.Count > 0 ? SrDescriptors[0].DeviceConfig : null; }
            set
            {
                if (SrDescriptors.Count == 0)
                    SrDescriptors.Add(new SrDescriptor());
                SrDescriptors[0].DeviceConfig = value;
            }
        }
        public Dictionary<String, String> SMConfig
        {
            get { return SrDescriptors.Count > 0 ? SrDescriptors[0].SMConfig : null; }
            set
            {
                if (SrDescriptors.Count == 0)
                    SrDescriptors.Add(new SrDescriptor());
                SrDescriptors[0].SMConfig = value;
            }
        }
        public string Description
        {
            get { return SrDescriptors.Count > 0 ? SrDescriptors[0].Description : null; }
            set
            {
                if (SrDescriptors.Count == 0)
                    SrDescriptors.Add(new SrDescriptor());
                SrDescriptors[0].Description = value;
            }
        }
        public bool DisasterRecoveryTask { get; set; }
        public bool AutoDescriptionRequired { get; set; }

        public List<SrDescriptor> SrDescriptors { get; set; }

        private SR _srToReattach;
        public SR SrToReattach
        {
            get { return _srToReattach; }
            set
            {
                if (_srToReattach == value)
                    return;

                _srToReattach = value;
                
                if (value != null)
                {
                    SrName = _srToReattach.Name();
                    Description = _srToReattach.Description();
                    UUID = _srToReattach.uuid;
                }
            }
        }

        public virtual void ResetSrName(IXenConnection connection)
        {
            SrName = SrWizardHelpers.DefaultSRName(String.Format(Messages.SRWIZARD_STORAGE_NAME, SR.getFriendlyTypeName(Type)), connection);
        }

        public virtual IEnumerable<SR.SRTypes> PossibleTypes { get { return new SR.SRTypes[] { Type }; } }
    }

    public class SrWizardType_CifsIso : SrWizardType
    {
        public override IEnumerable<string> Errors
        {
            get { return new[] { Failure.SR_BACKEND_FAILURE_225, Failure.SR_BACKEND_FAILURE_140, Failure.SR_BACKEND_FAILURE_222 }; }
        }

        public override bool IsEnhancedSR { get { return false; } }
        public override string FrontendBlurb { get { return Messages.NEWSR_CIFS_ISO_BLURB; } }
        public override string FrontendTypeName { get { return Messages.NEWSR_CIFS_ISO_TYPE_NAME; } }
        public override SR.SRTypes Type { get { return SR.SRTypes.iso; } }
        public override string ContentType { get { return SR.Content_Type_ISO; } }
        public override bool ShowIntroducePrompt { get { return false; } }
        public override bool ShowReattachWarning { get { return false; } }
        public override bool AllowToCreateNewSr { get { return true; } set { } }

        public override void ResetSrName(IXenConnection connection)
        {
            SrName = SrWizardHelpers.DefaultSRName(Messages.SRWIZARD_CIFS_LIBRARY, connection);
        }
    }

    public class SrWizardType_Iscsi : SrWizardType
    {
        public override bool IsEnhancedSR { get { return false; } }
        public override string FrontendBlurb { get { return Messages.NEWSR_LVMOISCSI_BLURB; } }
        public override string FrontendTypeName { get { return Messages.NEWSR_LVMOISCSI_TYPE_NAME; } }
        public override SR.SRTypes Type { get { return IsGfs2 ? SR.SRTypes.gfs2 : SR.SRTypes.lvmoiscsi; } }
        public override string ContentType { get { return ""; } }
        public override bool ShowIntroducePrompt { get { return false; } }
        public override bool ShowReattachWarning { get { return true; } }
        public override bool AllowToCreateNewSr { get; set; }
        public override bool IsGfs2 { get; set; }

        public override void ResetSrName(IXenConnection connection)
        {
            SrName = SrWizardHelpers.DefaultSRName(Messages.SRWIZARD_ISCSI_STORAGE, connection);
        }

        public override IEnumerable<SR.SRTypes> PossibleTypes { get { return new SR.SRTypes[] { SR.SRTypes.lvmoiscsi, SR.SRTypes.gfs2 }; } }
    }

    public class SrWizardType_Hba : SrWizardType
    {
        public override bool IsEnhancedSR { get { return false; } }
        public override string FrontendBlurb { get { return Messages.NEWSR_LVMOHBA_BLURB; } }
        public override string FrontendTypeName { get { return Messages.NEWSR_LVMOHBA_TYPE_NAME; } }
        public override SR.SRTypes Type { get { return IsGfs2 ? SR.SRTypes.gfs2 : SR.SRTypes.lvmohba; } }
        public override string ContentType { get { return ""; } }
        public override bool ShowIntroducePrompt { get { return false; } }
        public override bool ShowReattachWarning { get { return true; } }
        public override bool AllowToCreateNewSr { get; set; }
        public override bool IsGfs2 { get; set; }

        public override void ResetSrName(IXenConnection connection)
        {
            SrName = SrWizardHelpers.DefaultSRName(Messages.NEWSR_HBA_DEFAULT_NAME, connection);
        }

        public override IEnumerable<SR.SRTypes> PossibleTypes { get { return new SR.SRTypes[] { SR.SRTypes.lvmohba, SR.SRTypes.gfs2 }; } }
    }

    public class SrWizardType_VhdoNfs : SrWizardType
    {
        public override IEnumerable<string> Errors
        {
            get { return new[] { Failure.SR_BACKEND_FAILURE_72, Failure.SR_BACKEND_FAILURE_73, Failure.SR_BACKEND_FAILURE_140 }; }
        }

        public override bool IsEnhancedSR { get { return false; } }
        public override string FrontendBlurb { get { return Messages.NEWSR_VHDONFS_BLURB; } }
        public override string FrontendTypeName { get { return Messages.NEWSR_VHDONFS_TYPE_NAME; } }
        public override SR.SRTypes Type { get { return SR.SRTypes.nfs; } }
        public override string ContentType { get { return ""; } }
        public override bool ShowIntroducePrompt { get { return true; } }
        public override bool ShowReattachWarning { get { return true; } }
        public override bool AllowToCreateNewSr { get; set; }

        public override void ResetSrName(IXenConnection connection)
        {
            SrName = SrWizardHelpers.DefaultSRName(Messages.SRWIZARD_NFS_STORAGE, connection);
        }
    }

    public class SrWizardType_NfsIso : SrWizardType
    {
        public override IEnumerable<string> Errors
        {
            get { return new[] { Failure.SR_BACKEND_FAILURE_72, Failure.SR_BACKEND_FAILURE_140, Failure.SR_BACKEND_FAILURE_222 }; }
        }

        public override bool IsEnhancedSR { get { return false; } }
        public override string FrontendBlurb { get { return Messages.NEWSR_NFS_ISO_BLURB; } }
        public override string FrontendTypeName { get { return Messages.NEWSR_NFS_ISO_TYPE_NAME; } }
        public override SR.SRTypes Type { get { return SR.SRTypes.iso; } }
        public override string ContentType { get { return SR.Content_Type_ISO; } }
        public override bool ShowIntroducePrompt { get { return false; } }
        public override bool ShowReattachWarning { get { return false; } }
        public override bool AllowToCreateNewSr { get { return true; } set { } }

        public override void ResetSrName(IXenConnection connection)
        {
            SrName = SrWizardHelpers.DefaultSRName(Messages.SRWIZARD_NFS_LIBRARY, connection);
        }
    }

    public class SrWizardType_Cifs : SrWizardType
    {
        public override IEnumerable<string> Errors
        {
            get { return new[] { Failure.SR_BACKEND_FAILURE_111, Failure.SR_BACKEND_FAILURE_112, Failure.SR_BACKEND_FAILURE_113, Failure.SR_BACKEND_FAILURE_114, Failure.SR_BACKEND_FAILURE_454 }; }
        }

        public override bool IsEnhancedSR { get { return false; } }
        public override string FrontendBlurb { get { return Messages.NEWSR_CIFS_BLURB; } }
        public override string FrontendTypeName { get { return Messages.NEWSR_CIFS_TYPE_NAME; } }
        public override SR.SRTypes Type { get { return SR.SRTypes.smb; } }
        public override string ContentType { get { return ""; } }
        public override bool ShowIntroducePrompt { get { return false; } }
        public override bool ShowReattachWarning { get { return false; } }
        public override bool AllowToCreateNewSr { get { return true; } set { } }

        public override void ResetSrName(IXenConnection connection)
        {
            SrName = SrWizardHelpers.DefaultSRName(Messages.SRWIZARD_CIFS_STORAGE, connection);
        }
    }

    public class SrWizardType_Cslg : SrWizardType
    {
        public override bool IsEnhancedSR { get { return true; } }
        public override string FrontendBlurb { get { return Messages.NEWSR_CSLG_BLURB; } }
        public override string FrontendTypeName { get { return Messages.NEWSR_CSLG_TYPE_NAME; } }
        public override SR.SRTypes Type { get { return SR.SRTypes.cslg; } }
        public override string ContentType { get { return ""; } }
        public override bool ShowIntroducePrompt { get { return true; } }
        public override bool ShowReattachWarning { get { return true; } }
        public override bool AllowToCreateNewSr { get { return true; } set { } }

        public SrWizardType_NetApp ToNetApp()
        {
            var netApp = new SrWizardType_NetApp
                             {
                                 SrName = SrName,
                                 UUID = UUID,
                                 DeviceConfig = DeviceConfig,
                                 Description = Description,
                                 DisasterRecoveryTask = DisasterRecoveryTask,
                                 AutoDescriptionRequired = AutoDescriptionRequired,
                                 SrToReattach = SrToReattach,
                                 AllowToCreateNewSr = AllowToCreateNewSr
                             };
            return netApp;
        }

        public SrWizardType_EqualLogic ToEqualLogic()
        {
            var equal = new SrWizardType_EqualLogic
                            {
                                SrName = SrName,
                                UUID = UUID,
                                DeviceConfig = DeviceConfig,
                                Description = Description,
                                DisasterRecoveryTask = DisasterRecoveryTask,
                                AutoDescriptionRequired = AutoDescriptionRequired,
                                SrToReattach = SrToReattach,
                                AllowToCreateNewSr = AllowToCreateNewSr
                            };
            return equal;
        }
    }

    public class SrWizardType_NetApp : SrWizardType
    {
        public override bool IsEnhancedSR { get { return true; } }
        public override string FrontendBlurb { get { return Messages.NEWSR_NETAPP_BLURB; } }
        public override string FrontendTypeName { get { return SR.getFriendlyTypeName(Type); } }
        public override SR.SRTypes Type { get { return SR.SRTypes.netapp; } }
        public override string ContentType { get { return ""; } }
        public override bool ShowIntroducePrompt { get { return true; } }
        public override bool ShowReattachWarning { get { return true; } }
        public override bool AllowToCreateNewSr { get; set; }
    }

    public class SrWizardType_EqualLogic : SrWizardType
    {
        public override bool IsEnhancedSR { get { return true; } }
        public override string FrontendBlurb { get { return Messages.NEWSR_EQUAL_LOGIC_BLURB; } }
        public override string FrontendTypeName { get { return SR.getFriendlyTypeName(Type); } }
        public override SR.SRTypes Type { get { return SR.SRTypes.equal; } }
        public override string ContentType { get { return ""; } }
        public override bool ShowIntroducePrompt { get { return true; } }
        public override bool ShowReattachWarning { get { return true; } }
        public override bool AllowToCreateNewSr { get; set; }
    }

    public class SrWizardType_Fcoe : SrWizardType
    {
        public override bool IsEnhancedSR { get { return false; } }
        public override string FrontendBlurb { get { return Messages.NEWSR_LVMOFCOE_BLURB; } }
        public override string FrontendTypeName { get { return Messages.NEWSR_LVMOFCOE_TYPE_NAME; } }
        public override SR.SRTypes Type { get { return IsGfs2 ? SR.SRTypes.gfs2 : SR.SRTypes.lvmofcoe; } }
        public override string ContentType { get { return ""; } }
        public override bool ShowIntroducePrompt { get { return false; } }
        public override bool ShowReattachWarning { get { return true; } }
        public override bool AllowToCreateNewSr { get; set; }
        public override bool IsGfs2 { get; set; }

        public override void ResetSrName(IXenConnection connection)
        {
            SrName = SrWizardHelpers.DefaultSRName(Messages.NEWSR_FCOE_DEFAULT_NAME, connection);
        }

        public override IEnumerable<SR.SRTypes> PossibleTypes { get { return new SR.SRTypes[] { SR.SRTypes.lvmofcoe, SR.SRTypes.gfs2 }; } }
    }
}
