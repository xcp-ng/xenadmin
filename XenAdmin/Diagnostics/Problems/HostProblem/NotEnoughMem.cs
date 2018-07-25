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

using System.Windows.Forms;
using XenAdmin.Commands;
using XenAdmin.Diagnostics.Checks;
using XenAPI;
using XenAdmin.Dialogs.VMDialogs;
using XenAdmin.Actions;
using XenAdmin.Actions.VMActions;
using System.Collections.Generic;


namespace XenAdmin.Diagnostics.Problems.HostProblem
{
    public class NotEnoughMem : HostProblem
    {
        private List<VM> VmsToSuspend = new List<VM>();
        private List<VM> VmsToShutdown = new List<VM>();

        public NotEnoughMem(Check check, Host host)
            : base(check,  host)
        {
        }

        public override string Description
        {
            get { return string.Format(Messages.UPDATES_WIZARD_NO_MEMORY, ServerName); }
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            using (var dlg = new SelectVMsToSuspendDialog(Server))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    cancelled = false;

                    VmsToSuspend = dlg.SelectedVMsToSuspend;
                    VmsToShutdown = dlg.SelectedVMsToShutdown;

                    return new SuspendAndShutdownVMsAction(Server.Connection, Server, VmsToSuspend, VmsToShutdown);
                }
            }

            cancelled = true;
            return null;
        }

        public override string HelpMessage
        {
            get { return Messages.SUSPEND_VMS; }
        }

        public override AsyncAction CreateUnwindChangesAction()
        {
            return new ResumeAndStartVMsAction(Server.Connection, Server, VmsToSuspend, VmsToShutdown,VMOperationCommand.WarningDialogHAInvalidConfig,VMOperationCommand.StartDiagnosisForm);
        }


    }
}
