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

using XenAPI;

namespace XenAdmin.Actions
{
    public class UpdateVIFAction : AsyncAction
    {
        private readonly VIF vif;
        private readonly VIF vifDescriptor;

        public UpdateVIFAction(VM vm, VIF vif, VIF vifDescriptor)
            : base(vm.Connection, string.Format(Messages.ACTION_VIF_UPDATING_TITLE, vif.NetworkName(), vm.Name()))
        {
            this.vif = vif;
            VM = vm;
            this.vifDescriptor = vifDescriptor;

            ApiMethodsToRoleCheck.AddRange(DeleteVIFAction.XmlRpcMethods);
            ApiMethodsToRoleCheck.AddRange(CreateVIFAction.XmlRpcMethods);
        }

        public bool RebootRequired { get; private set; }

        protected override void Run()
        {
            Description = Messages.ACTION_VIF_UPDATING;

            new DeleteVIFAction(vif, true).RunExternal(Session);

            var createAction = new CreateVIFAction(VM, vifDescriptor, true);
            createAction.RunExternal(Session);
            RebootRequired = createAction.RebootRequired;

            Description = Messages.ACTION_VIF_UPDATED;
        }
    }
}
