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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Properties;
using XenAPI;
using XenAdmin.Dialogs;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Reboots the selected host. Shows a confirmation dialog.
    /// </summary>
    internal class RebootHostCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public RebootHostCommand()
        {
        }

        public RebootHostCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public RebootHostCommand(IMainWindow mainWindow, Host host, Control parent)
            : base(mainWindow, host)
        {
            SetParent(parent);
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<AsyncAction> actions = new List<AsyncAction>();
            foreach (Host host in selection.AsXenObjects<Host>(CanExecute))
            {
                MainWindowCommandInterface.CloseActiveWizards(host.Connection);
                RebootHostAction action = new RebootHostAction( host,AddHostToPoolCommand.NtolDialog);
                action.Completed += Program.MainWindow.action_Completed;
                actions.Add(action);
            }
            RunMultipleActions(actions, null, Messages.ACTION_HOSTS_REBOOTING, Messages.ACTION_HOSTS_REBOOTED, true);
        }

        private static bool CanExecute(Host host)
        {
            return host != null && host.IsLive();
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (!selection.AllItemsAre<Host>())
            {
                return false;
            }

            foreach (SelectedItem item in selection)
            {
                if (CanExecute((Host)item.XenObject))
                {
                    return true;
                }
            }
            return false;
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._001_Reboot_h32bit_16;
            }
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_REBOOT;
            }
        }

        protected override string ConfirmationDialogText
        {
            get
            {
                List<Host> hosts = GetSelection().AsXenObjects<Host>();

                bool hasRunningVMs = false;
                var hciHosts = new List<Host>();

                foreach (Host h in hosts)
                {
                    if (h.HasRunningVMs())
                        hasRunningVMs = true;

                    if (h.Connection.ResolveAll(h.resident_VMs).Exists(v => v.HciWarnBeforeShutdown()))
                        hciHosts.Add(h);
                }

                if (hciHosts.Count > 0)
                    return hciHosts.Count == 1
                        ? string.Format(Messages.CONFIRM_REBOOT_HCI_WARN_SERVER, hciHosts[0].Name())
                        : string.Format(Messages.CONFIRM_REBOOT_HCI_WARN_SERVERS, string.Join("\n", hciHosts.Select(h => h.Name())));
                
                if (hasRunningVMs)
                    return hosts.Count == 1
                        ? string.Format(Messages.CONFIRM_REBOOT_SERVER, hosts[0].Name())
                        : Messages.CONFIRM_REBOOT_SERVERS;
                
                return hosts.Count == 1
                    ? string.Format(Messages.CONFIRM_REBOOT_SERVER_NO_VMS, hosts[0].Name())
                    : Messages.CONFIRM_REBOOT_SERVERS_NO_VMS;
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
                SelectedItemCollection selection = GetSelection();
                if (selection.Count == 1)
                {
                    return Messages.CONFIRM_REBOOT_SERVER_TITLE;
                }
                return Messages.CONFIRM_REBOOT_SERVERS_TITLE;
            }
        }

        protected override CommandErrorDialog GetErrorDialogCore(IDictionary<IXenObject, string> cantExecuteReasons)
        {
            foreach (Host host in GetSelection().AsXenObjects<Host>())
            {
                if (!CanExecute(host) && host.IsLive())
                {
                    return new CommandErrorDialog(Messages.ERROR_DIALOG_FORCE_REBOOT_VM_TITLE, Messages.ERROR_DIALOG_FORCE_REBOOT_VM_TEXT, cantExecuteReasons);
                }
            }
            return null;
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            Host host = item as Host;
            if (host == null)
            {
                return base.GetCantExecuteReasonCore(item);
            }
            if (!host.IsLive())
            {
                return Messages.HOST_NOT_LIVE;
            }
            return base.GetCantExecuteReasonCore(item);
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.MAINWINDOW_REBOOT_HOST_CONTEXT_MENU;
            }
        }

        protected override string ConfirmationDialogYesButtonLabel
        {
            get
            {
                return Messages.CONFIRM_REBOOT_SERVER_YES_BUTTON_LABEL;
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
