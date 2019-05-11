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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;
using XenAdmin.Dialogs;

using XenAdmin.Actions.HostActions;

namespace XenAdmin.Commands
{
    /// <summary>
    /// Powers on the selected hosts.
    /// </summary>
    internal class PowerOnHostCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public PowerOnHostCommand()
        {
        }

        public PowerOnHostCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public PowerOnHostCommand(IMainWindow mainWindow, Host host)
            : base(mainWindow, host)
        {
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_POWER_ON;
            }
        }

        public override string ToolBarText
        {
            get
            {
                return Messages.MAINWINDOW_POWER_ON_NO_AMP;
            }
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._001_PowerOn_h32bit_16;
            }
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<AsyncAction> actions = new List<AsyncAction>();
            foreach (Host host in selection.AsXenObjects<Host>(CanExecute))
            {
                var action = new HostPowerOnAction( host);
                action.Completed += Program.MainWindow.action_Completed;
                actions.Add(action);
            }
            RunMultipleActions(actions, null, Messages.ACTION_HOST_STARTING, Messages.ACTION_HOST_STARTED, true);
        }

        private static bool CanExecute(Host host)
        {
            return host != null
                && !host.IsLive()
                && host.allowed_operations != null && host.allowed_operations.Contains(host_allowed_operations.power_on)
                && !HelpersGUI.HasActiveHostAction(host)
                && host.power_on_mode != "";
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<Host>() && selection.AtLeastOneXenObjectCan<Host>(CanExecute);
        }

        protected override CommandErrorDialog GetErrorDialogCore(IDictionary<IXenObject, string> cantExecuteReasons)
        {
            foreach (Host host in GetSelection().AsXenObjects<Host>())
            {
                if (!CanExecute(host) && !host.IsLive())
                {
                    return new CommandErrorDialog(Messages.ERROR_DIALOG_POWER_ON_HOST_TITLE, Messages.ERROR_DIALOG_POWER_ON_HOST_TEXT, cantExecuteReasons);
                }
            }
            return null;
        }

        public override Keys ShortcutKeys
        {
            get
            {
                return Keys.Control | Keys.B;
            }
        }

        public override string ShortcutKeyDisplayString
        {
            get
            {
                return Messages.MAINWINDOW_CTRL_B;
            }
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            Host host = item as Host;
            if (host == null)
            {
                return base.GetCantExecuteReasonCore(item);
            }
            if (host.IsLive())
            {
                return Messages.HOST_ALREADY_POWERED_ON;
            }
            else if (host.power_on_mode == "")
            {
                return Messages.HOST_POWER_ON_MODE_NOT_SET;
            }
            return base.GetCantExecuteReasonCore(item);
        }


        public override string ContextMenuText
        {
            get
            {
                return Messages.MAINWINDOW_POWER_ON_CONTEXT_MENU;
            }
        }
    }
}
