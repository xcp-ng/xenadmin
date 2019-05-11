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
    public class CreateVIFAction : AsyncAction
    {
        private VIF _vifDescriptor;

        public CreateVIFAction(VM vm, VIF vifDescriptor, bool suppressHistory = false)
            : base(vm.Connection, string.Format(Messages.ACTION_VIF_CREATING_TITLE, vm.Name()), suppressHistory)
        {
            _vifDescriptor = vifDescriptor;
            VM = vm;

            foreach (var method in XmlRpcMethods)
                ApiMethodsToRoleCheck.Add(method);
        }

        /// <summary>
        /// A List of XML RPC methods used by this class
        /// </summary>
        public static readonly string[] XmlRpcMethods = {"Async_VIF.create", "VIF.plug"};

        public bool RebootRequired { get; private set; }

        protected override void Run()
        {
            Description = Messages.ACTION_VIF_CREATING;

            RelatedTask = VIF.async_create(Session, _vifDescriptor);
            PollToCompletion();

            if (VM.power_state == vm_power_state.Running)
            {
                string newVifRef = Result; //the result from async_create

                if (VIF.get_allowed_operations(Session, newVifRef).Contains(vif_operations.plug))
                    VIF.plug(Session, newVifRef); // try hot-plugging
                else
                    RebootRequired = true;
            }

            Description = Messages.ACTION_VIF_CREATED;
        }
    }
}
