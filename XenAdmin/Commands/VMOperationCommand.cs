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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Actions.VMActions;


namespace XenAdmin.Commands
{
    /// <summary>
    /// This is the base Command for the start-on, resume-on and migrate Commands.
    /// </summary>
    internal abstract class VMOperationCommand : Command
    {
        private readonly vm_operations _operation;

        public VMOperationCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection, vm_operations operation)
            : base(mainWindow, selection)
        {
            _operation = operation;
            AssertOperationIsSupported();
        }

        public VMOperationCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
            _operation = vm_operations.unknown;
        }

        private void AssertOperationIsSupported()
        {
            if (_operation != vm_operations.start_on && 
                _operation != vm_operations.resume_on && 
                _operation != vm_operations.pool_migrate)
            {
                throw new NotSupportedException("Invalid operation, not supported");
            }
        }

        public virtual Image SecondImage
        {
            get
            {
                return null;
            }
        }

        public virtual double StarRating
        {
            get
            {
                return 0;
            }
        }

        protected vm_operations Operation
        {
            get
            {
                return _operation;
            }
        }

        /// <summary>
        /// Gets the host which should be used for the start-on, resume-on or migrate for the specified VM.
        /// </summary>
        protected abstract Host GetHost(VM vm);

        protected sealed override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<VM>() && selection.AtLeastOneXenObjectCan<VM>(CanExecute);
        }

        /// <summary>
        /// Determines whether the specified VM can be executed (i.e. resumed-on, started-on or migrated.)
        /// </summary>
        protected abstract bool CanExecute(VM vm);

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            AssertOperationAllowsExecution();

            string title = Messages.ACTION_VMS_RESUMING_ON_TITLE;
            string startDescription = Messages.ACTION_VMS_RESUMING_ON_TITLE;
            string endDescription = Messages.ACTION_VM_RESUMED;

            List<AsyncAction> actions = new List<AsyncAction>();
            if (_operation == vm_operations.pool_migrate)
            {
                
                title = Messages.ACTION_VMS_MIGRATING_TITLE;
                startDescription = Messages.ACTION_VMS_MIGRATING_TITLE;
                endDescription = Messages.ACTION_VM_MIGRATED;
                foreach (VM vm in selection.AsXenObjects<VM>(CanExecute))
                {
                    this.MainWindowCommandInterface.CloseActiveWizards(vm);
                    Host host = GetHost(vm);
                    actions.Add(new VMMigrateAction(vm, host));
                }
            }
            else if (_operation == vm_operations.start_on)
            {
                title = Messages.ACTION_VMS_STARTING_ON_TITLE;
                startDescription = Messages.ACTION_VMS_STARTING_ON_TITLE;
                endDescription = Messages.ACTION_VM_STARTED;
                foreach (VM vm in selection.AsXenObjects<VM>(CanExecute))
                {
                    Host host = GetHost(vm);
                    actions.Add(new VMStartOnAction(vm, host,WarningDialogHAInvalidConfig, StartDiagnosisForm));
                }
            }
            else if (_operation == vm_operations.resume_on)
            {
                title = Messages.ACTION_VMS_RESUMING_ON_TITLE;
                startDescription = Messages.ACTION_VMS_RESUMING_ON_TITLE;
                endDescription = Messages.ACTION_VM_RESUMED;
                foreach (VM vm in selection.AsXenObjects<VM>(CanExecute))
                {
                    Host host = GetHost(vm);
                    actions.Add(new VMResumeOnAction(vm, host, WarningDialogHAInvalidConfig, StartDiagnosisForm));
                }
            }

            RunMultipleActions(actions, title, startDescription, endDescription, true);
        }

        private void AssertOperationAllowsExecution()
        {
            if(_operation == vm_operations.unknown)
                throw new NotSupportedException("VM operation unknown is not supported");
        }

        protected string ErrorDialogTitle
        {
            get
            {
                if (_operation == vm_operations.pool_migrate)
                {
                    return Messages.ERROR_DIALOG_MIGRATE_TITLE;
                }
                else if (_operation == vm_operations.start_on)
                {
                    return Messages.ERROR_DIALOG_START_ON_TITLE;
                }
                return Messages.ERROR_DIALOG_RESUME_ON_TITLE;
            }
        }

        protected string ErrorDialogText
        {
            get
            {
                if (_operation == vm_operations.pool_migrate)
                {
                    return Messages.ERROR_DIALOG_MIGRATE_TEXT;
                }
                else if (_operation == vm_operations.start_on)
                {
                    return Messages.ERROR_DIALOG_START_ON_TEXT;
                }
                return Messages.ERROR_DIALOG_RESUME_ON_TEXT;
            }
        }

        public static void WarningDialogHAInvalidConfig(VM vm, bool isStart)
        {
            Program.Invoke(Program.MainWindow, () =>
            {
                DialogResult dialogResult;
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning,
                        String.Format(isStart ? Messages.HA_INVALID_CONFIG_START : Messages.HA_INVALID_CONFIG_RESUME,
                            Helpers.GetName(vm).Ellipsise(500)),
                        Messages.HIGH_AVAILABILITY),
                    ThreeButtonDialog.ButtonOK,
                    ThreeButtonDialog.ButtonCancel))
                {
                    dialogResult = dlg.ShowDialog(Program.MainWindow);
                }
                if (dialogResult == DialogResult.Cancel)
                {
                    throw new CancelledException();
                }
            });
        }


        public static void StartDiagnosisForm(VM vm, bool isStart)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                string title = Messages.ERROR_DIALOG_START_VM_TITLE;
                string text = string.Format(Messages.ERROR_DIALOG_START_VM_TEXT, vm);

                if (Win32Window.GetWindowWithText(title) != null)
                {
                    // don't bother showing this if there's one already up.
                    return;
                }

                IXenConnection connection = vm.Connection;
                Session session = connection.DuplicateSession();
                if (session != null)
                {
                    Dictionary<Host, string> reasons = new Dictionary<Host, string>();

                    foreach (Host host in connection.Cache.Hosts)
                    {
                        reasons[host] = string.Empty;
                        if (!isStart && VMOperationHostCommand.VmCpuFeaturesIncompatibleWithHost(host, vm))
                        {
                            reasons[host] = FriendlyErrorNames.VM_INCOMPATIBLE_WITH_THIS_HOST;
                            continue;
                        }
                        try
                        {
                            VM.assert_can_boot_here(session, vm.opaque_ref, host.opaque_ref);
                        }
                        catch (Failure failure)
                        {
                            reasons[host] = failure.Message;
                        }
                    }

                    Program.Invoke(Program.MainWindow, () => CommandErrorDialog.Create<Host>(title, text, reasons).ShowDialog(Program.MainWindow));
                }
            });
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// In the case there being nowhere to start/resume the VM (NO_HOSTS_AVAILABLE), shows the reason why the VM could not be started
        /// on each host. If the start failed due to HA_OPERATION_WOULD_BREAK_FAILOVER_PLAN, offers to decrement ntol and try the operation
        /// again.
        /// </summary>
        public static void StartDiagnosisForm(VMStartAbstractAction VMStartAction , Failure failure)
        {

            if (failure.ErrorDescription[0] == Failure.NO_HOSTS_AVAILABLE)
            {
                // Show a dialog displaying why the VM couldn't be started on each host
                StartDiagnosisForm(VMStartAction.VM, VMStartAction.IsStart);
            }
            else if (failure.ErrorDescription[0] == Failure.HA_OPERATION_WOULD_BREAK_FAILOVER_PLAN)
            {
                // The action was blocked by HA because it would reduce the number of tolerable server failures.
                // With the user's consent, we'll reduce the number of configured failures to tolerate and try again.
                Pool pool = Helpers.GetPool(VMStartAction.VM.Connection);
                if (pool == null)
                {
                    log.ErrorFormat("Could not get pool for VM {0} in StartDiagnosisForm()", Helpers.GetName(VMStartAction.VM));
                    return;
                }

                long ntol = pool.ha_host_failures_to_tolerate;
                long newNtol = Math.Min(pool.ha_plan_exists_for - 1, ntol - 1);
                if (newNtol <= 0)
                {
                    // We would need to basically turn HA off to start this VM
                    string msg = String.Format(VMStartAction.IsStart ? Messages.HA_VM_START_NTOL_ZERO : Messages.HA_VM_RESUME_NTOL_ZERO,
                        Helpers.GetName(pool).Ellipsise(100),
                        Helpers.GetName(VMStartAction.VM).Ellipsise(100));
                    Program.Invoke(Program.MainWindow, delegate()
                    {
                        using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Warning, msg, Messages.HIGH_AVAILABILITY)))
                        {
                            dlg.ShowDialog(Program.MainWindow);
                        }
                    });
                }
                else
                {
                    // Show 'reduce ntol?' dialog
                    string msg = String.Format(VMStartAction.IsStart ? Messages.HA_VM_START_NTOL_DROP : Messages.HA_VM_RESUME_NTOL_DROP,
                        Helpers.GetName(pool).Ellipsise(100), ntol,
                        Helpers.GetName(VMStartAction.VM).Ellipsise(100), newNtol);

                    Program.Invoke(Program.MainWindow, delegate()
                    {
                        DialogResult r;
                        using (var dlg = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(SystemIcons.Warning, msg, Messages.HIGH_AVAILABILITY),
                            ThreeButtonDialog.ButtonYes,
                            new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, ThreeButtonDialog.ButtonType.CANCEL, true)))
                        {
                            r = dlg.ShowDialog(Program.MainWindow);
                        }

                        if (r == DialogResult.Yes)
                        {
                            DelegatedAsyncAction action = new DelegatedAsyncAction(VMStartAction.VM.Connection, Messages.HA_LOWERING_NTOL, null, null,
                                delegate(Session session)
                                {
                                    // Set new ntol, then retry action
                                    XenAPI.Pool.set_ha_host_failures_to_tolerate(session, pool.opaque_ref, newNtol);
                                    // ntol set succeeded, start new action
                                    VMStartAction.Clone().RunAsync();
                                });
                            action.RunAsync();
                        }
                    });
                }
            }
        }
    }
}
