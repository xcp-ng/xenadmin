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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Dialogs;
using XenAPI;

using XenAdmin.Actions.DR;

namespace XenAdmin.Diagnostics.Problems.VMProblem
{
    public class ExistingVmProblem: VMProblem
    {
        public ExistingVmProblem(Check check, VM vm)
            : base(check, vm)
        {
        }

        public override string Description
        {
            get
            {
                return String.Format(Messages.DR_WIZARD_PROBLEM_EXISTING_VM, Helpers.GetPoolOfOne(VM.Connection).Name()); 
            } 
        }

        public override string HelpMessage
        {
            get { return Messages.DR_WIZARD_PROBLEM_EXISTING_VM_HELPMESSAGE; } 
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            Program.AssertOnEventThread();

            DialogResult dialogResult;
            using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, String.Format(Messages.CONFIRM_DELETE_VM, VM.Name(), VM.Connection.Name), Messages.ACTION_SHUTDOWN_AND_DESTROY_VM_TITLE),
                    ThreeButtonDialog.ButtonYes,
                    ThreeButtonDialog.ButtonNo))
            {
                dialogResult = dlg.ShowDialog();
            }
            if (dialogResult == DialogResult.Yes)
            {
                cancelled = false;
                List<VM> vms = new List<VM> { VM };
                return new ShutdownAndDestroyVMsAction(VM.Connection, vms);
            }

            cancelled = true;
            return null;
        }

        public override AsyncAction CreateUnwindChangesAction()
        {
            return null;
        }
    }
}
