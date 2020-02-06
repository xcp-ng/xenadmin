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
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.PoolProblem;
using XenAPI;
using System.Linq;

namespace XenAdmin.Diagnostics.Checks.DR
{
    public class AssertCanBeRecoveredCheck : PoolCheck
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Session MetadataSession;
        private readonly IXenObject xenObject;
        public VDI Vdi;
        public AssertCanBeRecoveredCheck(IXenObject xenObject, Pool pool, VDI vdi)
            : base(pool)
        {
            this.xenObject = xenObject;
            Vdi = vdi;
        }

        private SR RetrieveSR(XenRef<SR> xenRefSr)
        {
            return SR.get_record(MetadataSession, xenRefSr);
        }

        private List<SR> GetRequiredSRs(IXenObject xenObject)
        {
            List<XenRef<SR>> xenRefSRs = new List<XenRef<SR>>();
            
            if (xenObject is VM)
                xenRefSRs = VM.get_SRs_required_for_recovery(MetadataSession, xenObject.opaque_ref, Pool.Connection.Session.opaque_ref);
            if (xenObject is VM_appliance)
                xenRefSRs = VM_appliance.get_SRs_required_for_recovery(MetadataSession, xenObject.opaque_ref, Pool.Connection.Session.opaque_ref);

            if (xenRefSRs != null && xenRefSRs.Count > 0)
                return xenRefSRs.Select(item => RetrieveSR(item)).ToList();

            return null;
        }

        private bool FibreChannelSR(SR sr)
        {
            return sr.GetSRType(true) == SR.SRTypes.lvmohba;
        }

        private List<SRDeviceConfig> GetFCSRDeviceConfigList(List<SR> requiredSRs)
        {
            List<SRDeviceConfig> srDeviceConfigList = new List<SRDeviceConfig>();
            
            List<SR> fcSRList = requiredSRs.FindAll(FibreChannelSR);
            foreach (SR fcSR in fcSRList)
            {
                Dictionary<string, string> dconf = null;
                if (fcSR.PBDs.Count > 0)
                    dconf = PBD.get_device_config(MetadataSession, fcSR.PBDs[0].opaque_ref);

                if (dconf != null)
                    srDeviceConfigList.Add(new SRDeviceConfig(fcSR, dconf));
            }
            
            return srDeviceConfigList;
        }

        protected override Problem RunCheck()
        {
            if (MetadataSession == null)
                return null;

            try
            {
                if (xenObject is VM)
                    VM.assert_can_be_recovered(MetadataSession, xenObject.opaque_ref, Pool.Connection.Session.opaque_ref);
                if (xenObject is VM_appliance)
                    VM_appliance.assert_can_be_recovered(MetadataSession, xenObject.opaque_ref,
                                                         Pool.Connection.Session.opaque_ref);
            }
            catch (Failure f)
            {
                if (f.ErrorDescription.Count > 2 && f.ErrorDescription[0] == Failure.VM_REQUIRES_SR)
                {
                    List<SR> requiredSRs = GetRequiredSRs(xenObject) ?? new List<SR>();

                    SR sr = RetrieveSR(new XenRef<SR>(f.ErrorDescription[2]));
                    if (!requiredSRs.Contains(sr))
                        requiredSRs.Add(sr);

                    //search for local SRs
                    SR localSR = requiredSRs.Find(item => !item.shared);
                    if (localSR != null)
                    {
                        // there is local SR which means the VM cannot be recovered
                        return new MissingSRProblem(this, Pool, localSR, null);
                    }

                    //search for FibreChannel SRs
                    List<SRDeviceConfig> srDeviceConfigList = GetFCSRDeviceConfigList(requiredSRs);
                    if (srDeviceConfigList.Count == 0)
                        return new MissingSRProblem(this, Pool, requiredSRs[0], null);

                    if (srDeviceConfigList.Count == 1)
                        return new MissingSRProblem(this, Pool, srDeviceConfigList[0].SR,
                                                    srDeviceConfigList[0].DeviceConfig);
                    
                    return new MissingMultipleFCSRsProblem(this, Pool, srDeviceConfigList);                   
                }
            }
            catch (Exception e)
            {
                log.Error($"There was an error calling assert_can_be_recovered for object {xenObject.Name()}.", e);
            }
            return null;
        }

        public override string Description
        {
            get
            {
                return String.Format(xenObject is VM 
                    ? Messages.DR_WIZARD_VM_CHECK_DESCRIPTION
                    : Messages.DR_WIZARD_APPLIANCE_CHECK_DESCRIPTION, xenObject.Name()); 
            }
        }
    }
}
