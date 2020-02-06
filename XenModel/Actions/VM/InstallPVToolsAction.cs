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
using System.Linq;
using XenAPI;


namespace XenAdmin.Actions
{
    /// <summary>
    /// This action installs the PV tools on a selected windows VM.
    /// </summary>
    public class InstallPVToolsAction : AsyncAction
    {
        private readonly bool _searchHiddenIsOs;

        public InstallPVToolsAction(VM vm, bool searchHiddenISOs)
            : base(vm.Connection, string.Format(Messages.INSTALLTOOLS_TITLE, vm.Name()))
        {
            VM = vm;
            _searchHiddenIsOs = searchHiddenISOs;

            #region RBAC Dependencies
            foreach (SR sr in VM.Connection.Cache.SRs)
            {
                if (sr.IsToolsSR() && sr.IsBroken())
                {
                    ApiMethodsToRoleCheck.Add("pbd.plug");
                    ApiMethodsToRoleCheck.Add("pbd.create");
                    break;
                }
            }
            ApiMethodsToRoleCheck.Add("vbd.eject");
            ApiMethodsToRoleCheck.Add("vbd.insert");
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);
            #endregion
        }

        protected override void Run()
        {
            // check the xstools sr is present, if not try and repair it
            foreach(SR sr in VM.Connection.Cache.SRs)
            {
                if (sr.IsToolsSR() && sr.IsBroken())
                {
                    try
                    {
                        SrRepairAction action = new SrRepairAction(sr.Connection, sr, false);
                        action.RunExternal(Session);
                    }
                    catch (Failure f)
                    {
                        throw new Failure(Messages.XS_TOOLS_SR_NOT_FOUND, f);
                    }
                }

            }

            // Check the version (if any) of the PV tools already on this host...
            VM_guest_metrics guestMetrics = Connection.Resolve(VM.guest_metrics);
            if (guestMetrics != null && !VM.HasNewVirtualisationStates() && guestMetrics.PV_drivers_installed() && guestMetrics.PV_drivers_up_to_date)
            {
                this.Description = Messages.INSTALLTOOLS_EXIST;
                return;
            }

            // Check that the VM has a cd-rom...
            XenAPI.VBD cdrom = VM.FindVMCDROM();
            if (cdrom == null)
            {
                throw new Failure(Messages.INSTALLTOOLS_COULDNOTFIND_CD);
            }

            // Find the tools ISO...
            XenAPI.VDI winIso = findWinISO(_searchHiddenIsOs);
            if (winIso == null)
            {
                throw new Failure(Messages.INSTALLTOOLS_COULDNOTFIND_WIN);
            }

            Description = Messages.INSTALLTOOLS_STARTING;

            // Eject anything that's currently in the cd-rom...
            if (!cdrom.empty)
            {
                XenAPI.VBD.eject(Session, cdrom.opaque_ref);
            }

            // Insert the tools ISO...
            XenAPI.VBD.insert(Session, cdrom.opaque_ref, winIso.opaque_ref);

            // done here; installation continues on the VM
            Description = Messages.INSTALLTOOLS_DONE;
        }

        // Find the windows ISO disc by scanning the SRs - will return
        // null if the disc could not be found...
        private XenAPI.VDI findWinISO(bool searchHiddenISOs)
        {
            foreach (SR sr in Connection.Cache.SRs)
            {
                if (XenAPI.SR.SRTypes.iso.ToString() == sr.content_type)
                {
                    var vdis = Connection.ResolveAllShownXenModelObjects(sr.VDIs, searchHiddenISOs);
                    var vdi = vdis.FirstOrDefault(v => v.IsToolsIso());
                    if (vdi != null)
                        return vdi;
                }
            }

            return null;
        }
    }
}
