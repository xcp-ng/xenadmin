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

using System.Diagnostics;
using XenAdmin.Actions;
using XenAdmin.Commands;
using XenAdmin.Diagnostics.Checks;
using XenAPI;
using XenAdmin.Actions.VMActions;


namespace XenAdmin.Diagnostics.Problems.VMProblem
{
    public abstract class VMProblem : Problem
    {
        private VM _vm;
        protected Host ResidentOn;

        protected VMProblem(Check check, VM vm)
            : base(check)
        {
            _vm = vm;

            ResidentOn = VM.Connection.Resolve(VM.resident_on);
        }

        protected VM VM
        {
            get
            {
                return _vm;
            }
        }

        public override string HelpMessage
        {
            get
            {
                if (CanSuspendVM)
                    return Messages.SUSPEND_VM;
                else
                {
                    return Messages.SHUTDOWN_VM;
                }
            }
        }

        public sealed override string Title
        {
            get { return string.Format(Messages.PROBLEM_VMPROBLEM_TITLE, VM.uuid); }
        }

        protected AsyncAction SuspendVM()
        {
            return new VMSuspendAction(VM);
        }

        protected AsyncAction ShutdownVM()
        {
            return new VMHardShutdown(VM);
        }

        protected virtual bool CanSuspendVM
        {
            get
            {
                return VM.allowed_operations.Contains(vm_operations.suspend);
            }
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            if (_vm.Connection.Resolve(new XenRef<VM>(_vm.opaque_ref)) == null) // check if the vm is still in the cache
                return null;

            if (CanSuspendVM)
                return SuspendVM();
            return ShutdownVM();
        }

        public override AsyncAction CreateUnwindChangesAction()
        {
            if (_vm.Connection.Resolve(new XenRef<VM>(_vm.opaque_ref)) == null) // check if the vm is still in the cache
                return null;

            Debug.Assert(VM.power_state == vm_power_state.Halted ||
                         VM.power_state == vm_power_state.Suspended, "Expected VM to be suspended or shut down!");
            if(VM.power_state==vm_power_state.Halted)
                return new VMStartOnAction(VM, ResidentOn, VMOperationCommand.WarningDialogHAInvalidConfig, VMOperationCommand.StartDiagnosisForm);
            else
            {
                return new VMResumeAction(VM, VMOperationCommand.WarningDialogHAInvalidConfig,
                                          VMOperationCommand.StartDiagnosisForm);
            }
        }
    }
}
