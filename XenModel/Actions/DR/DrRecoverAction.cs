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
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.DR
{
    public class DrRecoverAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Session MetadataSession;
        private readonly IXenObject xenObject;

        public DrRecoverAction(IXenConnection connection, IXenObject xenObject)
            : base(connection, xenObject is VM
                                   ? string.Format(Messages.ACTION_DR_RECOVER_VM_TITLE, xenObject.Name())
                                   : string.Format(Messages.ACTION_DR_RECOVER_APPLIANCE_TITLE, xenObject.Name()))
        {
            this.xenObject = xenObject;

            Pool = Helpers.GetPoolOfOne(connection);
            #region RBAC Dependencies
            if (xenObject is VM)
                ApiMethodsToRoleCheck.Add("VM.async_recover");
            if (xenObject is VM_appliance)
                ApiMethodsToRoleCheck.Add("VM_appliance.async_recover");
            #endregion

        }

        public IXenObject XenObject
        {
            get { return xenObject; }
        }

        protected override void Run()
        {
            Description = Messages.ACTION_DR_RECOVER_STATUS;
            if (MetadataSession != null)
            {
                if (xenObject is VM)
                    RelatedTask = VM.async_recover(MetadataSession, xenObject.opaque_ref, Session.opaque_ref, true);
                if (xenObject is VM_appliance)
                {
                    // if appliance already exists in target pool, it will be replaced during recovery and the uuid is preserved
                    RelatedTask = VM_appliance.async_recover(MetadataSession, xenObject.opaque_ref, Session.opaque_ref, true);
                }
                PollToCompletion();
            }
            else
            {
                log.DebugFormat("Metadata session is NULL. Cannot recover {0} to Pool {1}",
                                Helpers.GetName(xenObject), Helpers.GetName(Pool));
            }
            Description = String.Format(Messages.ACTION_DR_RECOVER_DONE, xenObject.Name());
        }
    }
}