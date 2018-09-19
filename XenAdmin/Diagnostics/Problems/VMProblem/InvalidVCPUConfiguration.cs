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
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using System.Windows.Forms;


namespace XenAdmin.Diagnostics.Problems.VMProblem
{
    public class InvalidVCPUConfiguration : VMProblem
    {
        public InvalidVCPUConfiguration(Check check, VM vm)
            : base(check, vm) { }

        public override string Description
        {
            get { return String.Format(Messages.UPGRADEWIZARD_PROBLEM_INVALID_VCPU_SETTINGS, ServerName, VM.Name()); }
        }

        public override string HelpMessage
        {
            get { return Messages.UPGRADEWIZARD_PROBLEM_INVALID_VCPU_SETTINGS_HELPMESSAGE; }
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            Program.AssertOnEventThread();
            cancelled = false;

            AsyncAction action = null;

            using (PropertiesDialog propertiesDialog = new PropertiesDialog(VM))
            {
                propertiesDialog.SelectVMCPUEditPage();

                propertiesDialog.FormClosing += (s, ee) =>
                {
                    if (propertiesDialog.DialogResult == DialogResult.Yes && ee.Action != null)
                    {
                        ee.StartAction = false;
                        action = ee.Action;
                    }
                };
                
                propertiesDialog.ShowDialog(Program.MainWindow);
                if (propertiesDialog.DialogResult != DialogResult.Yes || action == null)
                    cancelled = true;
            }

            return action;
        }

        public override AsyncAction CreateUnwindChangesAction()
        {
            return null;
        }
    }
}

