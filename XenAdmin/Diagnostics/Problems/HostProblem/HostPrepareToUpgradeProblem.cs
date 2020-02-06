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


using System.Drawing;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Diagnostics.Problems.HostProblem
{
    class HostPrepareToUpgradeProblem : HostProblem
    {
        private readonly string _shortMessage;
        private readonly string _longMessage;

        public HostPrepareToUpgradeProblem(Check check, Host host, string friendlyErrorKey = null)
            : base(check, host)
        {
            InitialiseMessagesFromErrorKey(friendlyErrorKey, out _shortMessage, out _longMessage);
        }

        public override bool IsFixable => false;

        public override string Description => _shortMessage;
       
        public override string HelpMessage => Messages.MORE_INFO;

        protected override Actions.AsyncAction CreateAction(out bool cancelled)
        {
            Program.Invoke(Program.MainWindow, () =>
            {
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Information, _longMessage)))
                {
                    dlg.ShowDialog();
                }
            });

            cancelled = true;
            return null;
        }

        private void InitialiseMessagesFromErrorKey(string errorKey, out string shortMessage, out string longMessage)
        {
            longMessage = Messages.PROBLEM_PREPARE_TO_UPGRADE;

            if (string.IsNullOrEmpty(errorKey))
            {
                shortMessage = string.Format(Messages.INSTALL_FILES_CANNOT_BE_FOUND, ServerName);
                return;
            }

            if (errorKey.StartsWith("REPO_SERVER_ERROR_"))
                errorKey = "REPO_SERVER_ERROR_5XX";

            var friendlyError = FriendlyNameManager.GetFriendlyName($"PREPARE_HOST_UPGRADE_{errorKey}");

            if (string.IsNullOrEmpty(friendlyError))
            {
                shortMessage = string.Format(Messages.INSTALL_FILES_CANNOT_BE_FOUND, ServerName);
                return;
            }

            shortMessage = $"{ServerName}: {friendlyError}";
        }
    }
}
