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
using XenAPI;


namespace XenAdmin.Actions
{
    public class CreateCdDriveAction : AsyncAction
    {
        /// <summary>
        /// Subscribe to this even unless installing tools
        /// </summary>
        public event Action<string> ShowUserInstruction;

        public CreateCdDriveAction(VM vm)
            : base(vm.Connection, string.Format(Messages.NEW_DVD_DRIVE_CREATE_TITLE, vm.Name()))
        {
            VM = vm;

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("vm.assert_agile");
            ApiMethodsToRoleCheck.AddRange(XenAPI.Role.CommonSessionApiList);
            #endregion
        }

        protected override void Run()
        {
            VBD cdrom = VM.FindVMCDROM();
            if (cdrom == null)
            {
                Description = Messages.NEW_DVD_DRIVE_CREATING;
                // could not find a cd, try and make one

                if (VM.VBDs.Count >= VM.MaxVBDsAllowed())
                {
                    throw new Exception(Messages.CDDRIVE_MAX_ALLOWED_VBDS);
                }

                List<String> allowedDevices = new List<String>(XenAPI.VM.get_allowed_VBD_devices(Session, VM.opaque_ref));

                if (allowedDevices == null || allowedDevices.Count == 0)
                {
                    throw new Exception(Messages.CDDRIVE_MAX_ALLOWED_VBDS);
                }

                XenAPI.VBD cdDrive = new XenAPI.VBD
                {
                    VM = new XenAPI.XenRef<XenAPI.VM>(VM.opaque_ref),
                    bootable = false,
                    device = "",
                    userdevice = allowedDevices.Contains("3") ? "3" : allowedDevices[0],
                    empty = true,
                    type = XenAPI.vbd_type.CD,
                    mode = XenAPI.vbd_mode.RO
                };

                var cdCreate = new VbdSaveAndPlugAction(VM, cdDrive, Messages.DVD_DRIVE, Session, true);
                cdCreate.ShowUserInstruction += msg => ShowUserInstruction?.Invoke(msg);
                cdCreate.RunExternal(Session);
                Description = Messages.NEW_DVD_DRIVE_DONE;
            }
        }
    }
}
