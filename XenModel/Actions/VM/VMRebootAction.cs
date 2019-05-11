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
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions.VMActions
{
    public abstract class VMRebootAction : PureAsyncAction
    {

        public VMRebootAction(VM vm)
            : base(vm.Connection, string.Format(Messages.ACTION_VM_REBOOTING_ON_TITLE, vm.Name(), vm.Home() == null ? Helpers.GetName(vm.Connection) : vm.Home().Name()))
        {
            this.Description = Messages.ACTION_PREPARING;
            this.VM = vm;
            this.Host = vm.Home();
            this.Pool = Core.Helpers.GetPool(vm.Connection);
        }
    }

    public class VMCleanReboot : VMRebootAction
    {
        public VMCleanReboot(VM vm)
            : base(vm)
        {
        }

        protected override void Run()
        {
            this.Description = Messages.ACTION_VM_REBOOTING;
            RelatedTask = XenAPI.VM.async_clean_reboot(Session, VM.opaque_ref);
            PollToCompletion();
            this.Description = Messages.ACTION_VM_REBOOTED;
        }
    }

    public class VMHardReboot : VMRebootAction
    {
        public VMHardReboot(VM vm)
            : base(vm)
        {
        }

        protected override void Run()
        {
            this.Description = Messages.ACTION_VM_REBOOTING;
            RelatedTask = XenAPI.VM.async_hard_reboot(Session, VM.opaque_ref);
            PollToCompletion();
            this.Description = Messages.ACTION_VM_REBOOTED;
        }
    }
}
