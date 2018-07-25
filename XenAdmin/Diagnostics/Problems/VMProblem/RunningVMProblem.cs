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
using XenAdmin.Actions;
using XenAdmin.Actions.VMActions;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAPI;


namespace XenAdmin.Diagnostics.Problems.VMProblem
{
    public class RunningVMProblem: VMProblem
    {
        private readonly bool hardShutdown;

        public RunningVMProblem(Check check, VM vm, bool hardShutdown)
            : base(check, vm)
        {
            this.hardShutdown = hardShutdown;
        }

        public override string Description
        {
            get
            {
                return String.Format(Messages.DR_WIZARD_PROBLEM_RUNNING_VM, Helpers.GetPoolOfOne(VM.Connection).Name()); 
            } 
        }

        public override string HelpMessage
        {
            get { return Messages.DR_WIZARD_PROBLEM_RUNNING_VM_HELPMESSAGE; } 
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            Program.AssertOnEventThread();
            cancelled = false;

            if (hardShutdown)
                return new VMHardShutdown(VM);

            if (VM.allowed_operations.Contains(vm_operations.clean_shutdown))
                return new VMCleanShutdown(VM);
            return new VMHardShutdown(VM);
        }

        public override AsyncAction CreateUnwindChangesAction()
        {
            return null;
        }
    }
}