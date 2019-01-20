/* Original work Copyright (c) Citrix Systems, Inc.
 * Modified work Copyright XCP-ng project 
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
using XenAdmin.Core;

namespace XenAdmin.Actions.VMActions
{
    public abstract class VMUnPauseAction : PureAsyncAction
    {
        protected VMUnPauseAction(VM vm,string title)
            : base(vm.Connection, title)
        {
            //fixme: add new message to Messages
            //this.Description = Messages.ACTION_PREPARING;
            this.Description = "Unpause VM";
            this.VM = vm;
            this.Host = vm.Home();
            this.Pool = Core.Helpers.GetPool(vm.Connection);
        }
    }

    public class VMUnPause : VMUnPauseAction
    {
        public VMUnPause(VM vm)
            : base(vm, string.Format("Unpause VM", vm.Name(), vm.Home() == null ? Helpers.GetName(vm.Connection) : vm.Home().Name()))
        {
        }

        protected override void Run()
        {
            //fixme: add new message to Messages
            //this.Description = Messages.ACTION_VM_UNPAUSING;
            this.Description = "unpause VM";

            RelatedTask = VM.async_unpause(Session, VM.opaque_ref);
            PollToCompletion(0, 100);
            //fixme: add new message to Messages
            //this.Description = Messages.ACTION_VM_UNPAUSED;
            this.Description = "VM unpaused";
        }
    }

}
