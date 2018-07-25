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

using XenAdmin.Diagnostics.Checks;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Actions;


namespace XenAdmin.Diagnostics.Problems.VMProblem
{
    public class LocalCD : VMProblem
    {
        private VDI LoadedCD;

        public LocalCD(Check check, VM vm)
            : base(check, vm) { }

        public override string Description
        {
            get { return string.Format(Messages.UPDATES_WIZARD_LOCAL_CD, VM.Name().Ellipsise(25)); }
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            VBD cddrive = VM.FindVMCDROM();

            if (cddrive != null)
            {
                LoadedCD = VM.Connection.Resolve(cddrive.VDI);
            }

            return new ChangeVMISOAction(VM.Connection, VM, null, cddrive);
        }

        public override AsyncAction CreateUnwindChangesAction()
        {
            return null;

        }

        public override string HelpMessage
        {
            get { return Messages.EJECT_CD; }
        }

    }
}
