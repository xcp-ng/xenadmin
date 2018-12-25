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
using XenAdmin.Wizards.GenericPages;
using XenAPI;
using XenOvf;
using XenOvf.Definitions;

namespace XenAdmin.Wizards.ImportWizard
{
    public class OvfStorageResourceContainer : StorageResourceContainer
    {
        private readonly RASD_Type[] rasdArray;
        private readonly EnvelopeType selectedOvfEnvelope;
        private int currentLoc;

        public OvfStorageResourceContainer( EnvelopeType selectedOvfEnvelope, string sysId)
        {
            rasdArray = OVF.FindDiskRasds(selectedOvfEnvelope, sysId);
            this.selectedOvfEnvelope = selectedOvfEnvelope;
        }

        public override IStorageResource Next()
        {
            IStorageResource res = new OvfStorageResource(rasdArray[currentLoc], selectedOvfEnvelope);
            currentLoc++;
            return res;
        }

        public override bool IsNext
        {
            get { return currentLoc < rasdArray.Length; }
        }
    }

    public class OvfStorageResource : IStorageResource
    {
        private readonly RASD_Type rasd;
        private readonly EnvelopeType envelope;
        private readonly File_Type file ;

        public OvfStorageResource(RASD_Type type, EnvelopeType envelopeType)
        {
            rasd = type;
            envelope = envelopeType;
            file = OVF.FindFileReferenceByRASD(envelope, rasd);
        }
        public string DiskLabel
        {
            get
            {
                return rasd.ElementName == null ? null : rasd.ElementName.Value;
            }
        }

        public object Tag
        {
            get
            {
                return rasd.InstanceID == null ? null : rasd.InstanceID.Value;
            }
        }

        public bool SRTypeInvalid
        {
            get
            {
                ushort rasdtype = rasd.ResourceType.Value;
                return (rasdtype == 15 || rasdtype == 16);
            }
        }

        public bool CanCalculateDiskCapacity
        {
            get { return file != null; }
        }

        public ulong RequiredDiskCapacity
        {
            get
            {
                VirtualDiskDesc_Type disk = OVF.FindDiskReference(envelope, rasd);

                if (disk != null && !string.IsNullOrEmpty(disk.capacity))
                {
                    return Convert.ToUInt64(OVF.ComputeCapacity(Convert.ToInt64(disk.capacity), disk.capacityAllocationUnits));
                }
                else if (file != null)
                {
                    return file.size;
                }
                else 
                {
                    return 0;
                }
            }
        }

		public XenRef<SR> SR
		{
			get { return null; }
		}
	}
}
