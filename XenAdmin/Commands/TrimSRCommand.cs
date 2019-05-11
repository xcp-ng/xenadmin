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

using System.Collections.Generic;
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Commands
{
    internal class TrimSRCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public TrimSRCommand()
        {
        }

        public TrimSRCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public TrimSRCommand(IMainWindow mainWindow, SR sr)
            : base(mainWindow, sr)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            var actions = new List<AsyncAction>();
            foreach (SR sr in selection.AsXenObjects<SR>(CanExecute))
            {
                actions.Add(new SrTrimAction(sr.Connection, sr));
            }
            RunMultipleActions(actions, null, Messages.ACTION_SR_TRIM_DESCRIPTION, Messages.ACTION_SR_TRIM_DONE, true);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<SR>() && selection.AtLeastOneXenObjectCan<SR>(CanExecute);
        }

        private static bool CanExecute(SR sr)
        {
            return sr != null && sr.SupportsTrim() && sr.GetFirstAttachedStorageHost() != null;
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_TRIM_SR;
            }
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            SR sr = item as SR;
            if (sr != null && !sr.SupportsTrim())
            {
                return Messages.TOOLTIP_SR_TRIM_UNSUPPORTED;
            }
            return base.GetCantExecuteReasonCore(item);
        }

        /// <summary>
        /// Gets the tool tip text when the command is not able to run. 
        /// If multiple items and Trim is not supported on all, then return this reason.
        /// Otherwise, the default behaviour: CantExectuteReason for single items, null for multiple.
        /// </summary>
        protected override string DisabledToolTipText
        {
            get
            {
                var selection = GetSelection();
                var allUnsuported = selection.Count > 1 && selection.Select(item => item.XenObject as SR).All(sr => sr != null && !sr.SupportsTrim());
                return allUnsuported ? Messages.TOOLTIP_SR_TRIM_UNSUPPORTED_MULTIPLE : base.DisabledToolTipText;
            }
        }

        protected override bool ConfirmationRequired
        {
            get
            {
                return true;
            }
        }

        protected override string ConfirmationDialogTitle
        {
            get
            {
                return Messages.CONFIRM_TRIM_SR_TITLE;
            }
        }

        protected override string ConfirmationDialogText
        {
            get
            {
                return Messages.CONFIRM_TRIM_SR;
            }
        }
    }
}
