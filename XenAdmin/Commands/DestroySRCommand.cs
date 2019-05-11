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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Dialogs;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Destroys the selected SRs. Shows a confirmation dialog.
    /// </summary>
    internal class DestroySRCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public DestroySRCommand()
        {
        }

        public DestroySRCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public DestroySRCommand(IMainWindow mainWindow, SR selection)
            : base(mainWindow, selection)
        {
        }

        public DestroySRCommand(IMainWindow mainWindow, IEnumerable<SR> selection)
            : base(mainWindow, ConvertToSelection(selection))
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<AsyncAction> actions = new List<AsyncAction>();
            foreach (SR sr in selection.AsXenObjects<SR>(CanExecute))
            {
                actions.Add(new SrAction(SrActionKind.Destroy, sr));
            }
            RunMultipleActions(actions, Messages.ACTION_SRS_DESTROYING, string.Empty, string.Empty, true);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<SR>() && selection.AtLeastOneXenObjectCan<SR>(CanExecute);
        }

        private static bool CanExecute(SR sr)
        {
            return sr != null && !sr.HasRunningVMs() && sr.CanCreateWithXenCenter()
                && sr.allowed_operations.Contains(storage_operations.destroy) && !HelpersGUI.GetActionInProgress(sr);
        }

        /// <summary>
        /// Gets the text for a menu item which launches this Command.
        /// </summary>
        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_DESTROY_SR;
            }
        }

        protected override bool ConfirmationRequired
        {
            get
            {
                return true;
            }
        }

        protected override string ConfirmationDialogText
        {
            get
            {
                List<SR> srs = GetSelection().AsXenObjects<SR>();
                if (srs.Count == 1)
                {
                    return string.Format(Messages.MESSAGEBOX_DESTROY_SR_CONTINUE, srs[0].Name());
                }

                return Messages.MESSAGEBOX_DESTROY_SRS_CONTINUE;
            }
        }

        protected override string ConfirmationDialogTitle
        {
            get
            {
                if (GetSelection().Count == 1)
                {
                    return Messages.MESSAGEBOX_DESTROY_SR_CONTINUE_TITLE;
                }
                return Messages.MESSAGEBOX_DESTROY_SRS_CONTINUE_TITLE;
            }
        }

        protected override CommandErrorDialog GetErrorDialogCore(IDictionary<IXenObject, string> cantExecuteReasons)
        {
            return new CommandErrorDialog(Messages.ERROR_DIALOG_DESTROY_SR_TITLE, Messages.ERROR_DIALOG_DESTROY_SR_TEXT, cantExecuteReasons);
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            SR sr = item as SR;
            if (sr == null)
            {
                return base.GetCantExecuteReasonCore(item);
            }
            if (!sr.HasPBDs())
            {
                return Messages.SR_DETACHED;
            }
            else if (sr.HasRunningVMs())
            {
                return Messages.SR_HAS_RUNNING_VMS;
            }
            else if (!sr.CanCreateWithXenCenter())
            {
                return Messages.SR_CANNOT_BE_DESTROYED_WITH_XC;
            }
            else if (HelpersGUI.GetActionInProgress(sr))
            {
                return Messages.SR_ACTION_IN_PROGRESS;
            }
            return base.GetCantExecuteReasonCore(item);
        }

        protected override string ConfirmationDialogYesButtonLabel
        {
            get
            {
                return Messages.MESSAGEBOX_DESTROY_SR_YES_BUTTON_LABEL;
            }
        }

        protected override bool ConfirmationDialogNoButtonSelected
        {
            get
            {
                return true;
            }
        }
    }
}
