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
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;
using XenAdmin.Dialogs;
using System.Text;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Shuts down the selected hosts. Shows a confirmation dialog.
    /// </summary>
    internal class ShutDownHostCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public ShutDownHostCommand()
        {
        }

        public ShutDownHostCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public ShutDownHostCommand(IMainWindow mainWindow, Host host, Control parent)
            : base(mainWindow, host)
        {
            SetParent(parent);
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<AsyncAction> actions = new List<AsyncAction>();
            foreach (Host host in selection.AsXenObjects<Host>(CanExecute))
            {
                this.MainWindowCommandInterface.CloseActiveWizards(host.Connection);
                ShutdownHostAction action = new ShutdownHostAction(host,AddHostToPoolCommand.NtolDialog);
                action.Completed += Program.MainWindow.action_Completed;
                actions.Add(action);
            }
            RunMultipleActions(actions, null, Messages.ACTION_HOSTS_SHUTTING_DOWN, Messages.ACTION_HOSTS_SHUTDOWN, true);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<Host>() && selection.AtLeastOneXenObjectCan<Host>(CanExecute);
        }

        private static bool CanExecute(Host host)
        {
            return host != null && host.IsLive() && !HelpersGUI.HasActiveHostAction(host) ;
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_SHUTDOWN;
            }
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._001_ShutDown_h32bit_16;
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
                    return Messages.CONFIRM_SHUTDOWN_SERVER_TITLE;
                }

                return Messages.CONFIRM_SHUTDOWN_SERVERS_TITLE;
            }
        }

        protected override string ConfirmationDialogText
        {
            get
            {
                List<Host> hosts = GetSelection().AsXenObjects<Host>();
                bool hasRunningVMs = false;
                var hciHosts = new List<Host>();
                var poolMasters = new List<Host>();

                foreach (Host h in hosts)
                {
                    if (h.HasRunningVMs())
                        hasRunningVMs = true;

                    if (h.Connection.ResolveAll(h.resident_VMs).Exists(v => v.HciWarnBeforeShutdown()))
                        hciHosts.Add(h);

                    if (Helpers.HostIsMaster(h) && h.Connection.Cache.HostCount > 1)
                        poolMasters.Add(h);
                }

                StringBuilder sb = new StringBuilder();
                string firstWarning;


                if (hciHosts.Count > 0)
                    firstWarning = hciHosts.Count == 1
                        ? string.Format(Messages.CONFIRM_SHUTDOWN_HCI_WARN_SERVER, hciHosts[0].Name())
                        : string.Format(Messages.CONFIRM_SHUTDOWN_HCI_WARN_SERVERS, string.Join("\n", hciHosts.Select(h => h.Name())));

                else if (hasRunningVMs)
                    firstWarning = hosts.Count == 1
                        ? string.Format(Messages.CONFIRM_SHUTDOWN_SERVER, hosts[0].Name())
                        : Messages.CONFIRM_SHUTDOWN_SERVERS;

                else firstWarning = hosts.Count == 1
                    ? string.Format(Messages.CONFIRM_SHUTDOWN_SERVER_NO_VMS, hosts[0].Name())
                    : Messages.CONFIRM_SHUTDOWN_SERVERS_NO_VMS;

                sb.Append(firstWarning);

                if (poolMasters.Count == 1)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendFormat(Messages.SHUT_DOWN_POOL_MASTER_SINGLE, poolMasters[0].Name());
                }
                else if (poolMasters.Count > 1)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendFormat(Messages.SHUT_DOWN_POOL_MASTER_MULTIPLE, string.Join(", ", poolMasters.Select(h => h.Name())));
                }
                return sb.ToString();
            }
        }

        protected override CommandErrorDialog GetErrorDialogCore(IDictionary<IXenObject, string> cantExecuteReasons)
        {
            foreach (Host host in GetSelection().AsXenObjects<Host>())
            {
                if (!CanExecute(host) && host.IsLive())
                {
                    return new CommandErrorDialog(Messages.ERROR_DIALOG_SHUTDOWN_HOST_TITLE, Messages.ERROR_DIALOG_SHUTDOWN_HOST_TEXT, cantExecuteReasons);
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
                return Messages.HOST_ALREADY_SHUT_DOWN;
            }
            else if (HelpersGUI.HasActiveHostAction(host))
            {
                return Messages.HOST_ACTION_IN_PROGRESS;
            }
            return base.GetCantExecuteReasonCore(item);
        }

        protected override string ConfirmationDialogYesButtonLabel
        {
            get
            {
                return Messages.CONFIRM_SHUTDOWN_SERVER_YES_BUTTON_LABEL;
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
