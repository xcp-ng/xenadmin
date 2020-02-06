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
using System.Text;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Core;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using System.Collections.ObjectModel;
using XenAdmin.Network;
using System.Drawing;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Installs XenServer Tools on the selected VM.
    /// </summary>
    internal class InstallToolsCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public InstallToolsCommand()
        {
        }

        public InstallToolsCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public InstallToolsCommand(IMainWindow mainWindow, IEnumerable<VM> vms)
            : this(mainWindow, ConvertToSelection(vms))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallToolsCommand"/> class.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        /// <param name="vm">The vm.</param>
        /// <param name="parent">The parent window for any UI.</param>
        public InstallToolsCommand(IMainWindow mainWindow, VM vm, Control parent)
            : base(mainWindow, vm)
        {
            SetParent(parent);
        }

        public InstallToolsCommand(IMainWindow mainWindow, VM vm)
            : this(mainWindow, vm, null)
        {
        }

        /// <summary>
        /// Installs tools on the single VM selected.
        /// </summary>
        /// <returns>The <see cref="AsyncAction"/> for the Command.</returns>
        public AsyncAction ExecuteGetAction()
        {
            SelectedItemCollection selection = GetSelection();

            if (selection.Count > 1)
            {
                throw new InvalidOperationException("This method can only be used with a single VM selected.");
            }

            if (selection.ContainsOneItemOfType<VM>())
            {
                VM vm = (VM)selection[0].XenObject;

                if (CanExecute(vm))
                {
                    return SingleVMExecute(vm);
                }
            }

            return null;
        }

        /// <summary>
        /// Attempts to install tools on the vm
        /// </summary>
        /// <param name="vm"></param>
        /// <returns>null if user cancels or an AsyncAction. This is either the InstallPVToolsAction or the CreateCdDriveAction if the VM needed a DVD drive.</returns>
        private AsyncAction SingleVMExecute(VM vm)
        {
            if (vm.FindVMCDROM() == null)
            {
                DialogResult dialogResult;
                using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(
                            null,
                            Messages.NEW_DVD_DRIVE_REQUIRED,
                            Messages.XENCENTER),
                        ThreeButtonDialog.ButtonYes,
                        ThreeButtonDialog.ButtonNo))
                {
                    dialogResult = dlg.ShowDialog(Parent);
                }
                if (dialogResult == DialogResult.Yes)
                {
                    //do not register the event ShowUserInstruction; we show explicitly a message afterwards
                    var createDriveAction = new CreateCdDriveAction(vm);

                    using (var dlg = new ActionProgressDialog(createDriveAction, ProgressBarStyle.Marquee))
                    {
                        dlg.ShowDialog(Parent);
                    }

                    if (createDriveAction.Succeeded)
                    {
                        ShowMustRebootBox();
                    }
                    return createDriveAction;
                }
            }
            else
            {
                DialogResult dr = new InstallToolsWarningDialog(vm.Connection).ShowDialog(Parent);
                if (dr == DialogResult.Yes)
                {
                    var installToolsAction = new InstallPVToolsAction(vm, Properties.Settings.Default.ShowHiddenVMs);
                    installToolsAction.Completed += InstallToolsActionCompleted;

                    installToolsAction.RunAsync();
                    return installToolsAction;
                }
            }
            return null;
        }

        /// <summary>
        /// Attempts to install tools on several VMs
        /// </summary>
        /// <param name="vms"></param>
        /// <returns>Whether the action was launched (i.e., the user didn't Cancel)</returns>
        private bool MultipleVMExecute(List<VM> vms)
        {
            bool newDvdDrivesRequired = false;
            foreach (VM vm in vms)
            {
                if (CanExecute(vm) && vm.FindVMCDROM() == null)
                {
                    newDvdDrivesRequired = true;
                    break;
                }
            }

            if (newDvdDrivesRequired)
            {
                DialogResult dialogResult;
                using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.NEW_DVD_DRIVES_REQUIRED, Messages.XENCENTER),
                    ThreeButtonDialog.ButtonYes,
                    ThreeButtonDialog.ButtonNo))
                {
                    dialogResult = dlg.ShowDialog(Parent);
                }
                if (dialogResult == DialogResult.Yes)
                {
                    foreach (VM vm in vms)
                    {
                        if (CanExecute(vm) && vm.FindVMCDROM() == null)
                        {
                            //do not register the event ShowUserInstruction; we show explicitly a message afterwards
                            var createDriveAction = new CreateCdDriveAction(vm);

                            using (var dlg = new ActionProgressDialog(createDriveAction, ProgressBarStyle.Marquee))
                            {
                                dlg.ShowDialog(Parent);
                            }
                        }
                    }
                    ShowMustRebootBox();
                    return true;
                }
            }
            else
            {
                List<IXenConnection> vmConnections = new List<IXenConnection>();
                foreach (VM vm in vms)
                    vmConnections.Add(vm.Connection);

                if (new InstallToolsWarningDialog(null, true, vmConnections).ShowDialog(Parent) == DialogResult.Yes)
                {
                    foreach (VM vm in vms)
                    {
                        var installToolsAction = new InstallPVToolsAction(vm, Properties.Settings.Default.ShowHiddenVMs);

                        if (vms.IndexOf(vm) == 0)
                        {
                            installToolsAction.Completed += FirstInstallToolsActionCompleted;
                        }
                        else
                        {
                            installToolsAction.Completed += InstallToolsActionCompleted;
                        }
                        installToolsAction.RunAsync();
                    }
                    return true;
                }
            }
            return false;
        }

        private void FirstInstallToolsActionCompleted(ActionBase sender)
        {
            InstallPVToolsAction action = (InstallPVToolsAction)sender;

            MainWindowCommandInterface.Invoke(delegate
            {
                // Try to select the VM on which the PV tools are to be installed
                if (MainWindowCommandInterface.SelectObjectInTree(action.VM))
                {
                    // Switch to the VM's console tab
                    MainWindowCommandInterface.SwitchToTab(MainWindow.Tab.Console);
                }
            });
        }

        private void InstallToolsActionCompleted(ActionBase sender)
        {
            InstallPVToolsAction action = (InstallPVToolsAction)sender;
           
            MainWindowCommandInterface.Invoke(delegate
            {
                Program.MainWindow.SelectObject(action.VM);
                Program.MainWindow.TheTabControl.SelectedTab = Program.MainWindow.TabPageConsole;
            });
        }

        private void ShowMustRebootBox()
        {
            if (!MainWindowCommandInterface.RunInAutomatedTestMode)
            {
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Information, Messages.NEW_DVD_DRIVE_REBOOT_TOOLS)))
                {
                    dlg.ShowDialog(Parent);
                }
            }
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<VM> vms = selection.AsXenObjects<VM>(CanExecute);

            if (vms.Count == 1)
                SingleVMExecute(vms[0]);
            else
                MultipleVMExecute(vms);
        }

        public bool ConfirmAndExecute()
        {
            List<VM> vms = GetSelection().AsXenObjects<VM>(CanExecute);

            if (vms.Count == 0)
                return true;

            if (vms.Count == 1)
                return (SingleVMExecute(vms[0]) != null);
            
            return MultipleVMExecute(vms);
        }

        public static bool CanExecute(VM vm)
        {
            var virtualisationStatus = vm.GetVirtualisationStatus();

            return vm != null && !vm.is_a_template && !vm.Locked &&
                !virtualisationStatus.HasFlag(VM.VirtualisationStatus.UNKNOWN) &&
                (!virtualisationStatus.HasFlag(VM.VirtualisationStatus.IO_DRIVERS_INSTALLED) || !virtualisationStatus.HasFlag(VM.VirtualisationStatus.MANAGEMENT_INSTALLED)) &&
                vm.power_state == vm_power_state.Running && !ResidentHostIsOlderThanMaster(vm)
                && CanViewVMConsole(vm.Connection);
        }

        private static bool ResidentHostIsOlderThanMaster(VM vm)
        {
            var vmHome = vm.Home();
            return vmHome != null && Helpers.IsOlderThanMaster(vmHome);
        }

        public static bool CanExecuteAll(List<VM> vms)
        {
            foreach (VM vm in vms)
            {
                if (!CanExecute(vm))
                    return false;
            }
            return true;
        }

        private static bool CanViewVMConsole(XenAdmin.Network.IXenConnection xenConnection)
        {
            if (xenConnection.Session == null)
                return false;

            RbacMethodList r = new RbacMethodList("http/connect_console");
            if (Role.CanPerform(r, xenConnection, false))
                return true;

            return false;
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<VM>() && selection.AtLeastOneXenObjectCan<VM>(CanExecute);
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_INSTALL_TOOLS;
            }
        }
    }
}
