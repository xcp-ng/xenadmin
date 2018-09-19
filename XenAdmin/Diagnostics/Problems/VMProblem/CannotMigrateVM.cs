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
using XenAdmin.Diagnostics.Checks;
using XenAPI;


namespace XenAdmin.Diagnostics.Problems.VMProblem
{
    public class CannotMigrateVM : VMProblem
    {
        private readonly CannotMigrateVMReason reason;

        public enum CannotMigrateVMReason { Unknown, LicenseRestriction, CannotMigrateVm, CannotMigrateVmNoTools, CannotMigrateVmNoGpu, LacksFeatureSuspend, HasPCIAttached, OperationInProgress }

        public CannotMigrateVM(Check check, VM vm, CannotMigrateVMReason licenseRestriction = CannotMigrateVMReason.Unknown)
            : base(check, vm) 
        {
            this.reason = licenseRestriction;
        }

        public override string Description
        {
            get 
            {
                string descriptionFormat;

                switch (reason)
                {
                    case CannotMigrateVMReason.CannotMigrateVm:
                        descriptionFormat = Messages.UPDATES_WIZARD_CANNOT_MIGRATE_VM;
                        break;

                    case CannotMigrateVMReason.CannotMigrateVmNoTools:
                        descriptionFormat = Messages.UPDATES_WIZARD_CANNOT_MIGRATE_VM_NO_TOOLS;
                        break;

                    case CannotMigrateVMReason.CannotMigrateVmNoGpu:
                        descriptionFormat = Messages.UPDATES_WIZARD_CANNOT_MIGRATE_VM_NO_GPU;
                        break;

                    case CannotMigrateVMReason.LacksFeatureSuspend:
                        descriptionFormat = Messages.UPDATES_WIZARD_CANNOT_MIGRATE_VM_SUSPEND_REASON;
                        break;

                    case CannotMigrateVMReason.LicenseRestriction :
                        descriptionFormat = Messages.UPDATES_WIZARD_CANNOT_MIGRATE_VM_LICENSE_REASON;
                        break;

                    case CannotMigrateVMReason.HasPCIAttached:
                        descriptionFormat = Messages.UPDATES_WIZARD_CANNOT_MIGRATE_VM_PCI_REASON;
                        break;

                    case CannotMigrateVMReason.OperationInProgress:
                        descriptionFormat = Messages.UPDATES_WIZARD_CANNOT_MIGRATE_VM_OPERATION_IN_PROGRESS;
                        break;

                    default:
                        descriptionFormat = Messages.UPDATES_WIZARD_CANNOT_MIGRATE_VM_UNKNOWN_REASON;
                        break;
                }

                return String.Format(descriptionFormat, ServerName, VM.Name()); 
            }
        }
    }
}
