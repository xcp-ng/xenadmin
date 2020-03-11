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
using XenAdmin.Core;
using XenAdmin.Wizards;
using XenAPI;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Shows the New Sr wizard in re-attach mode.
    /// </summary>
    internal class ReattachSRCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public ReattachSRCommand()
        {
        }

        public ReattachSRCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public ReattachSRCommand(IMainWindow mainWindow, SR sr)
            : base(mainWindow, sr)
        {
        }

        private void Execute(SR sr)
        {
            if (CanReattachSR(sr))
                MainWindowCommandInterface.ShowPerConnectionWizard(sr.Connection, new NewSRWizard(sr.Connection, sr));
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Execute((SR)selection[0].XenObject);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.Count == 1 && CanReattachSR(selection[0].XenObject as SR);
        }

        private static bool CanReattachSR(SR sr)
        {
            return sr != null
                && !sr.HasPBDs()
                && !HelpersGUI.GetActionInProgress(sr)
                && !(sr.type == "cslg" && Helpers.FeatureForbidden(sr.Connection, Host.RestrictStorageChoices))
                && (SM.GetByType(sr.Connection, sr.type) != null);
        }

        public override string MenuText => Messages.MAINWINDOW_REATTACH_SR;
    }
}
