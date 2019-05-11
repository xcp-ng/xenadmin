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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAPI;
using XenAdmin.Dialogs;


namespace XenAdmin.Commands
{
    /// <summary>
    /// An interface used to hide the SetSelection and SetMainWindow methods on the <see cref="Command"/> class. 
    /// These methods are used only by the Command framework and are potentially confusing if seen by Command consumers.
    /// </summary>
    internal interface ICommand
    {
        void SetSelection(IEnumerable<SelectedItem> selection);
        void SetMainWindow(IMainWindow mainWindow);
    }

    /// <summary>
    /// A class which represents a user's task in XenCenter. Classes derived from <see cref="Command"/> can be easily
    /// assigned to menu items and toolbar buttons.
    /// </summary>
    [TypeConverter(typeof(CommandConverter))]
    internal abstract class Command : ICommand
    {
        private SelectedItemCollection _selection = new SelectedItemCollection();
        private IMainWindow _mainWindow;
        private Control _parent;

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        protected Command()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="mainWindow">The application main window.</param>
        protected Command(IMainWindow mainWindow)
        {
            Util.ThrowIfParameterNull(mainWindow, "mainWindow");
            _mainWindow = mainWindow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="mainWindow">The application main window.</param>
        /// <param name="selection">The selection context for the Command.</param>
        protected Command(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : this(mainWindow)
        {
            Util.ThrowIfParameterNull(selection, "selection");
            _selection = new SelectedItemCollection(selection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="mainWindow">The application main window.</param>
        /// <param name="selection">The selection context for the Command.</param>
        protected Command(IMainWindow mainWindow, SelectedItem selection)
            : this(mainWindow, new [] { selection })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="mainWindow">The application main window.</param>
        /// <param name="xenObject">The selection context for the Command.</param>
        protected Command(IMainWindow mainWindow, IXenObject xenObject)
            : this(mainWindow, new SelectedItem(xenObject))
        {
        }

        /// <summary>
        /// Gets a list of <see cref="SelectedItem"/>s from the specified <see cref="IXenObject"/>s.
        /// </summary>
        protected static IEnumerable<SelectedItem> ConvertToSelection<T>(IEnumerable<T> xenObjects) where T : IXenObject
        {
            Util.ThrowIfParameterNull(xenObjects, "selection");
            List<SelectedItem> selection = new List<SelectedItem>();
            foreach (T xenObject in xenObjects)
            {
                selection.Add(new SelectedItem(xenObject));
            }
            return selection;
        }

        /// <summary>
        /// Gets the current selection context for the Command.
        /// </summary>
        public SelectedItemCollection GetSelection()
        {
            return _selection;
        }

        /// <summary>
        /// Determines whether this instance can execute with the current selection context.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute; otherwise, <c>false</c>.
        /// </returns>
        public bool CanExecute()
        {
            return MainWindowCommandInterface != null && CanExecuteCore(GetSelection());
        }

        /// <summary>
        /// Determines whether this instance can execute with the current selection context.
        /// </summary>
        /// <param name="selection">The selection context.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can execute with the specified selection; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanExecuteCore(SelectedItemCollection selection)
        {
            return true;
        }

        /// <summary>
        /// Executes this Command on the current selection.
        /// </summary>
        public void Execute()
        {
            if (Confirm())
            {
                var cantExecuteReasons = GetCantExecuteReasons();
                var errorDialog = cantExecuteReasons.Count > 0 ? GetErrorDialogCore(cantExecuteReasons) : null;

                ExecuteCore(GetSelection());

                if (errorDialog != null)
                {
                    errorDialog.ShowDialog(Parent);
                }
            }
        }

        /// <summary>
        /// Executes this Command.
        /// </summary>
        /// <param name="selection">The selection the Command should operate on.</param>
        protected virtual void ExecuteCore(SelectedItemCollection selection)
        {
        }

        /// <summary>
        /// Gets the text for a menu item which launches this Command.
        /// </summary>
        public virtual string MenuText
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the text for a context menu item which launches this Command.
        /// </summary>
        public virtual string ContextMenuText
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the image for a menu item which launches this Command.
        /// </summary>
        public virtual Image MenuImage
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the image for a context menu item which launches this Command.
        /// </summary>
        public virtual Image ContextMenuImage
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the text for the toolbar button which launches this Command.
        /// </summary>
        public virtual string ToolBarText
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the image for a toolbar button which launches this Command.
        /// </summary>
        public virtual Image ToolBarImage
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the text for a button which launches this Command.
        /// </summary>
        public virtual string ButtonText
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the tool tip text. By default this is the can't execute reason if execution is not possible and
        /// blank if it can. Override EnabledToolTipText to provide a descriptive tooltip when the command is enabled.
        /// </summary>
        public virtual string ToolTipText
        {
            get 
            {
                if (CanExecute())
                    return EnabledToolTipText;

                return DisabledToolTipText;  
            }
        }

        /// <summary>
        /// Gets the tool tip text when the command is not able to run. CantExectuteReason for single items,
        /// null for multiple.
        /// </summary>
        protected virtual string DisabledToolTipText
        {
            get 
            {
                var reasons = GetCantExecuteReasons();
                // It's necessary to double check that we have one reason which matches up with a single selection
                // as CanExecuteCore and GetCantExecuteReasons aren't required to match up.
                if (reasons.Count == 1 && GetSelection().Count == 1)
                {
                    foreach (string s in reasons.Values)
                    {
                        if (s.Equals(Messages.UNKNOWN))
                        {
                            //This is the default, and not a useful tooltip
                            return null;
                        }
                        return s;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the tool tip text when the command is able to run. Null by default.
        /// </summary>
        protected virtual string EnabledToolTipText
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the shortcut key display string. This is only used if this Command is used on the main menu.
        /// </summary>
        public virtual string ShortcutKeyDisplayString
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the shortcut keys. This is only used if this Command is used on the main menu.
        /// </summary>
        public virtual Keys ShortcutKeys
        {
            get { return Keys.None; }
        }

        /// <summary>
        /// Gets a value indicating whether a confirmation dialog should be shown.
        /// </summary>
        protected virtual bool ConfirmationRequired
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the confirmation dialog title. The default for this is Messages.MESSAGEBOX_CONFIRM.
        /// </summary>
        protected virtual string ConfirmationDialogTitle
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the confirmation dialog text.
        /// </summary>
        protected virtual string ConfirmationDialogText
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the help id for the confirmatin dialog.
        /// </summary>
        protected virtual string ConfirmationDialogHelpId
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the confirmation dialog's "Yes" button label.
        /// </summary>
        protected virtual string ConfirmationDialogYesButtonLabel
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the confirmation dialog's "No" button label.
        /// </summary>
        protected virtual string ConfirmationDialogNoButtonLabel
        {
            get { return null; }
        }

        /// <summary>
        /// Gets a value indicating whether the "No" button should be selected when a confirmation dialog is displayed.
        /// </summary>
        protected virtual bool ConfirmationDialogNoButtonSelected
        {
            get { return false; }
        }

        /// <summary>
        /// Shows a confirmation dialog.
        /// </summary>
        /// <returns>true if the user clicked Yes.</returns>
        protected bool ShowConfirmationDialog()
        {
            ThreeButtonDialog.TBDButton buttonYes = ThreeButtonDialog.ButtonYes;
            if (!string.IsNullOrEmpty(ConfirmationDialogYesButtonLabel))
                buttonYes.label = ConfirmationDialogYesButtonLabel;
            
            ThreeButtonDialog.TBDButton buttonNo = ThreeButtonDialog.ButtonNo;
            if (!string.IsNullOrEmpty(ConfirmationDialogNoButtonLabel))
                buttonNo.label = ConfirmationDialogNoButtonLabel;
            if (ConfirmationDialogNoButtonSelected)
                buttonNo.selected = true;

            return MainWindow.Confirm(null, Parent, ConfirmationDialogTitle ?? Messages.XENCENTER,
                ConfirmationDialogHelpId, buttonYes, buttonNo, ConfirmationDialogText);
        }

        /// <summary>
        /// Shows a confirmation dialog if required.
        /// </summary>
        /// <returns>True if the operation should proceed.</returns>
        protected virtual bool Confirm()
        {
            return !ConfirmationRequired || ShowConfirmationDialog();
        }

        /// <summary>
        /// Gets all of the reasons that items in the selection can't execute.
        /// </summary>
        /// <returns>A dictionary of reasons keyed by the item name.</returns>
        public Dictionary<IXenObject, string> GetCantExecuteReasons()
        {
            var cantExecuteReasons = new Dictionary<IXenObject, string>();

            foreach (SelectedItem item in GetSelection())
            {
                if (item == null || item.XenObject == null)
                    continue;
                if (MainWindowCommandInterface != null && CanExecuteCore(new SelectedItemCollection(item)))
                    continue;

                string reason = GetCantExecuteReasonCore(item.XenObject);
                if (reason != null)
                    cantExecuteReasons.Add(item.XenObject, reason);
            }

            return cantExecuteReasons;
        }


        /// <summary>
        /// Gets the reason that the specified item from the selection cant execute. This is displayed in the error dialog.
        /// The default is "Unknown".
        /// </summary>
        protected virtual string GetCantExecuteReasonCore(IXenObject item)
        {
            return Messages.UNKNOWN;
        }

        /// <summary>
        /// Gets the error dialog to be displayed if one or more items in the selection couldn't be executed. Returns null by
        /// default i.e. An error dialog isn't displayed by default.
        /// </summary>
        /// <param name="cantExecuteReasons">The reasons for why the items couldn't execute.</param>
        protected virtual CommandErrorDialog GetErrorDialogCore(IDictionary<IXenObject, string> cantExecuteReasons)
        {
            return null;
        }

        /// <summary>
        /// Gets the main window to be used by the Command.
        /// </summary>
        public IMainWindow MainWindowCommandInterface
        {
            get { return _mainWindow; }
        }

        /// <summary>
        /// Sets the parent for any dialogs. If not called, then the main window is used.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public void SetParent(Control parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Gets the parent for any dialogs. If SetParent() hasn't been called then the MainWindow is returned.
        /// </summary>
        public Control Parent
        {
            get
            {
                return _parent ?? _mainWindow.Form;
            }
        }
        
        /// <summary>
        /// Runs the specified <see cref="AsyncAction"/>s such that they are synchronous per connection but asynchronous across connections.
        /// </summary>
        /// <param name="endDescription"></param>
        /// <param name="runActionsInParallel">Whether the actions should be executed simultaneously</param>
        /// <param name="actions"></param>
        /// <param name="title"></param>
        /// <param name="startDescription"></param>
        public void RunMultipleActions(IEnumerable<AsyncAction> actions, string title, string startDescription, string endDescription, bool runActionsInParallel)
        {
            MultipleActionLauncher launcher = new MultipleActionLauncher(actions, title, startDescription, endDescription, runActionsInParallel);
            launcher.Run();
        }

        #region ICommand Members

        /// <summary>
        /// Sets the selection context for the Command. This is hidden as it is only for use by the Command framework. 
        /// </summary>
        void ICommand.SetSelection(IEnumerable<SelectedItem> selection)
        {
            Util.ThrowIfParameterNull(selection, "selection");
            _selection = new SelectedItemCollection(selection);
        }

        /// <summary>
        /// Sets the main window for the Command. This is hidden as it is only for use by the Command framework.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        void ICommand.SetMainWindow(IMainWindow mainWindow)
        {
            Util.ThrowIfParameterNull(mainWindow, "mainWindow");
            _mainWindow = mainWindow;
        }

        #endregion
    }
}
