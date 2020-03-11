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
using System.Windows.Forms;
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAPI;
using System.Drawing;
using XenAdmin.Actions.VMActions;

using XenAdmin.Actions.HostActions;

namespace XenAdmin.Actions.Wlb
{
    class WlbOptimizePoolAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string optId = String.Empty;
        private readonly Dictionary<VM, WlbOptimizationRecommendation> vmOptList = new Dictionary<VM, WlbOptimizationRecommendation>();
        
        private bool moveToNext = false;
        private string hostActionError = String.Empty;

        public WlbOptimizePoolAction(Pool pool, Dictionary<VM, WlbOptimizationRecommendation> vmOptLst, string optId)
            : base(pool.Connection, string.Format(Messages.WLB_OPTIMIZING_POOL, Helpers.GetName(pool).Ellipsise(50)))
        {
            if (pool == null)
                throw new ArgumentNullException("pool");
            if (vmOptLst == null)
                throw new ArgumentNullException("vmOptLst");

            this.Pool = pool;
            this.vmOptList = vmOptLst;
            this.optId = optId;

            #region RBAC Dependencies
            // HA adjustments
            ApiMethodsToRoleCheck.Add("pool.sync_database");
            ApiMethodsToRoleCheck.Add("pool.set_ha_host_failures_to_tolerate");
            ApiMethodsToRoleCheck.Add("vm.set_ha_restart_priority");

            ApiMethodsToRoleCheck.Add("vm.assert_can_boot_here");
            ApiMethodsToRoleCheck.Add("vm.assert_agile");
            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);
            #endregion
        }

        protected override void Run()
        {
            if (vmOptList.Count == 0)
            {
                log.ErrorFormat("{0} has no VMs need to be optimized", Helpers.GetName(Pool));
                return;
            }
            
            this.Description = Messages.ACTION_WLB_POOL_OPTIMIZING;

            try
            {
                log.Debug("Optimizing " + Pool.Name());

                // for checking whether to display recommendations on optimize pool listview
                Helpers.SetOtherConfig(this.Session, this.Pool,WlbOptimizationRecommendation.OPTIMIZINGPOOL, Messages.IN_PROGRESS);
                int start = 0;
                int each = 90 / vmOptList.Count;

                foreach (KeyValuePair<VM, WlbOptimizationRecommendation> vmItem in vmOptList)
                {
                        VM vm = vmItem.Key;

                        if (vmItem.Key.is_control_domain)
                        {
                            log.Debug(vmItem.Value.powerOperation + " " + Helpers.GetName(vmItem.Value.toHost));
                            Host fromHost = vmItem.Value.fromHost;
                            Helpers.SetOtherConfig(fromHost.Connection.Session, fromHost,WlbOptimizationRecommendation.OPTIMIZINGPOOL, vmItem.Value.recId.ToString());

                            AsyncAction hostAction = null;
                            int waitingInterval = 10 * 1000; // default to 10s 

                            if (vmItem.Value.fromHost.IsLive())
                            {
                                hostAction = new ShutdownHostAction(fromHost, AddHostToPoolCommand.NtolDialog);
                            }
                            else
                            {
                                hostAction = new HostPowerOnAction(fromHost);
                                waitingInterval = 30 * 1000; // wait for 30s
                            }

                            hostAction.Completed += HostAction_Completed;
                            hostAction.RunAsync();

                            while (!moveToNext)
                            {
                                if (!String.IsNullOrEmpty(hostActionError))
                                {
                                    throw new Exception(hostActionError);
                                }
                                else
                                {
                                    //wait
                                    System.Threading.Thread.Sleep(waitingInterval);
                                }
                            }
                        }
                        else
                        {
                            log.Debug("Migrating VM " + vm.Name());
                            Host toHost = vmItem.Value.toHost;

                            try
                            {

                                // check if there is a conflict with HA, start optimize if we can
                                RelocateVmWithHa(this, vm, toHost, start, start + each, vmItem.Value.recId);
                            }
                            catch (Failure f)
                            {
                                // prompt to user if ha notl can be raised, if yes, continue
                                long newNtol;
                                if (RaiseHANotl(vm, f, out newNtol))
                                {
                                    DelegatedAsyncAction action = new DelegatedAsyncAction(vm.Connection, Messages.HA_LOWERING_NTOL, null, null,
                                        delegate(Session session)
                                    {
                                        // Set new ntol, then retry action
                                        XenAPI.Pool.set_ha_host_failures_to_tolerate(session, this.Pool.opaque_ref, newNtol);
                                        // ntol set succeeded, start new action
                                        Program.Invoke(Program.MainWindow, () => XenDialogBase.CloseAll(vm));
                                        new VMMigrateAction(vm,toHost).RunAsync();
                                    });
                                    action.RunAsync();
                                }
                                else
                                {
                                    Helpers.SetOtherConfig(this.Session, this.Pool, WlbOptimizationRecommendation.OPTIMIZINGPOOL, Messages.WLB_OPT_FAILED);
                                    this.Description = Messages.WLB_OPT_FAILED;
                                    throw;
                                }
                            }
                        }
                        start += each;

                        // stop if user cancels optimize pool
                        if (Cancelling)
                            throw new CancelledException();
                    }
                    this.Description = Messages.WLB_OPT_OK_NOTICE_TEXT;
                    Helpers.SetOtherConfig(this.Session, this.Pool, WlbOptimizationRecommendation.OPTIMIZINGPOOL, optId);
                    log.Debug("Completed optimizing " + Pool.Name());
                }
            catch (Failure ex)
            {
                Helpers.SetOtherConfig(this.Session, this.Pool, WlbOptimizationRecommendation.OPTIMIZINGPOOL, optId);
                WlbServerState.SetState(Pool, WlbServerState.ServerState.ConnectionError, ex);
                throw;
            }
            catch (CancelledException)
            {
                Helpers.SetOtherConfig(this.Session, this.Pool, WlbOptimizationRecommendation.OPTIMIZINGPOOL, optId);
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex, ex);
                this.Description = Messages.WLB_OPT_FAILED;
                Helpers.SetOtherConfig(this.Session, this.Pool, WlbOptimizationRecommendation.OPTIMIZINGPOOL, optId);
                log.Debug("Optimizing " + Pool.Name() + " is failed");
            }
            this.PercentComplete = 100;
        }

         
        private void HostAction_Completed(ActionBase sender)
        {
            AsyncAction action = (AsyncAction)sender;
            if (action.IsCompleted)
            {
                action.Completed -= HostAction_Completed;

                if (action is ShutdownHostAction || action is HostPowerOnAction)
                {
                    if (action.Description == Messages.ACTION_HOST_STARTED || action.Description == String.Format(Messages.ACTION_HOST_SHUTDOWN, Helpers.GetName(action.Host)))
                    {
                        moveToNext = true;
                    }
                    if ((action.Exception != null && !String.IsNullOrEmpty(action.Exception.Message)) || action.Description == String.Format(Messages.ACTION_HOST_START_FAILED, Helpers.GetName(action.Host)))
                    {
                        hostActionError = action.Exception == null ? action.Description : action.Exception.Message;
                    }
                }
            }
        }


        private void RelocateVmWithHa(AsyncAction action, VM vm, Host host, int start, int end, int recommendationId)
        {
            bool setDoNotRestart = false;

            if (vm.HaPriorityIsRestart())
            {
                try
                {
                    XenAPI.VM.assert_agile(action.Session, vm.opaque_ref);
                }
                catch (Failure)
                {
                    // VM is not agile, but it is 'Protected' by HA. This is an inconsistent state (see CA-20820).
                    // Tell the user the VM will be started without HA protection.
                    Program.Invoke(Program.MainWindow, delegate()
                    {
                        using (var dlg = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(
                                SystemIcons.Warning,
                                String.Format(Messages.HA_INVALID_CONFIG_RESUME, Helpers.GetName(vm).Ellipsise(500)),
                                Messages.HIGH_AVAILABILITY)))
                        {
                            dlg.ShowDialog(Program.MainWindow);
                        }
                    });

                    // Set the VM to 'Do not restart'.
                    XenAPI.VM.set_ha_restart_priority(action.Session, vm.opaque_ref, XenAPI.VM.RESTART_PRIORITY_DO_NOT_RESTART);
                    setDoNotRestart = true;
                }
            }

            if (!setDoNotRestart && vm.HasSavedRestartPriority())
            {
                // If HA is turned on, setting ha_always_run will cause the VM to be started by itself
                // but when the VM fails to start we want to know why, so we do a VM.start here too.
                // This will fail with a power state exception if HA has already started the VM - but in
                // that case we don't care, since the VM successfully started already.
                SetHaProtection(true, action, vm, start, end);
                try
                {
                    DoAction(action, vm, host, start, end, recommendationId);
                }
                catch (Failure f)
                {
                    if (f.ErrorDescription.Count == 4 && f.ErrorDescription[0] == Failure.VM_BAD_POWER_STATE && f.ErrorDescription[3] == "running")
                    {
                        // The VM started successfully via HA
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                // HA off: just do a regular start
                DoAction(action, vm, host, start, end, recommendationId);
            }
        }

        /// <summary>
        /// In the case there was nowhere to start/resume the VM (NO_HOSTS_AVAILABLE), shows the reason why the VM could not be started
        /// on each host. If the start failed due to HA_OPERATION_WOULD_BREAK_FAILOVER_PLAN, offers to decrement ntol and try the operation
        /// again.
        /// </summary>
        /// <param name="vm">vm</param>
        /// <param name="failure">failure, xapi exception</param>
        /// <param name="newNtol"></param>
        //private static bool StartDiagnosisForm(XenObject<VM> vm, Failure failure, string recommendationId)
        private bool RaiseHANotl(VM vm, Failure failure, out long newNtol)
        {
            bool error = false;
            newNtol = 0;

            if (failure.ErrorDescription[0] == Failure.NO_HOSTS_AVAILABLE)
            {
                VMOperationCommand.StartDiagnosisForm(vm, vm.power_state == vm_power_state.Halted);
            }
            else if (failure.ErrorDescription[0] == Failure.HA_OPERATION_WOULD_BREAK_FAILOVER_PLAN)
            {
                // The action was blocked by HA because it would reduce the number of tolerable server failures.
                // With the user's consent, we'll reduce the number of configured failures to tolerate and try again.
                if (this.Pool == null)
                {
                    log.ErrorFormat("Could not get pool for VM {0} in StartDiagnosisForm()", Helpers.GetName(vm));
                    return error;
                }

                long ntol = this.Pool.ha_host_failures_to_tolerate;
                newNtol = Math.Min(this.Pool.ha_plan_exists_for - 1, ntol - 1);
                if (newNtol <= 0)
                {
                    // We would need to basically turn HA off to start this VM
                    string msg = String.Format(Messages.HA_OPT_VM_RELOCATE_NTOL_ZERO,
                        Helpers.GetName(this.Pool).Ellipsise(100),
                        Helpers.GetName(vm).Ellipsise(100));
                    Program.Invoke(Program.MainWindow, delegate()
                    {
                        using (var dlg = new ThreeButtonDialog(
                           new ThreeButtonDialog.Details(
                               SystemIcons.Warning,
                               msg,
                               Messages.HIGH_AVAILABILITY)))
                        {
                            dlg.ShowDialog(Program.MainWindow);
                        }
                    });
                }
                else
                {
                    // Show 'reduce ntol?' dialog
                    string msg = String.Format(Messages.HA_OPT_DISABLE_NTOL_DROP,
                        Helpers.GetName(this.Pool).Ellipsise(100), ntol,
                        Helpers.GetName(vm).Ellipsise(100), newNtol);

                    Program.Invoke(Program.MainWindow, delegate()
                    {
                        using (var dlg = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(
                               SystemIcons.Warning,
                               msg,
                               Messages.HIGH_AVAILABILITY),
                            ThreeButtonDialog.ButtonYes,
                            new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, ThreeButtonDialog.ButtonType.CANCEL, true)))
                        {
                            DialogResult r = dlg.ShowDialog(Program.MainWindow);
                            if (r != DialogResult.Yes)
                            {
                                error = true;
                            }
                        }
                    });
                }
            }
            return error;
        }

        /// <summary>
        /// Migrate vm and log recommendation id for reporting purpose
        /// </summary>
        /// <param name="action">AsyncAction</param>
        /// <param name="vm">VM that needs to be migrated</param>
        /// <param name="host">Host that vm will migrate to</param>
        /// <param name="start">progress bar start point</param>
        /// <param name="end">progress bar end point</param>
        /// <param name="recommendationId">recommendation id</param>
        private static void DoAction(AsyncAction action, VM vm, Host host, int start, int end, int recommendationId)
        {
            action.RelatedTask = XenAPI.VM.async_live_migrate(action.Session, vm.opaque_ref, host.opaque_ref);

            if (recommendationId != 0)
            {
                // set vm migrate task key values for wlb reporting purpose
                Task.add_to_other_config(action.Session, action.RelatedTask.opaque_ref, "wlb_advised", recommendationId.ToString());
                Task.add_to_other_config(action.Session, action.RelatedTask.opaque_ref, "wlb_action", "vm_migrate");
                Task.add_to_other_config(action.Session, action.RelatedTask.opaque_ref, "wlb_action_obj_ref", vm.opaque_ref);
                Task.add_to_other_config(action.Session, action.RelatedTask.opaque_ref, "wlb_action_obj_type", "vm");
            }
            action.PollToCompletion(start, end);
        }

        /// <summary>
        /// Enables or disables HA protection for a VM (VM.ha_always_run). Also does a pool.sync_database afterwards.
        /// Does nothing if the server is a build before ha_always_run was introduced.
        /// May throw a XenAPI.Failure.
        /// </summary>
        /// <param name="protect">true if vm is protected</param>
        /// <param name="action">AsyncAction</param>
        /// <param name="vm">vm</param>
        /// <param name="start">progress bar start point</param>
        /// <param name="end">progress bar end point</param>
        private static void SetHaProtection(bool protect, AsyncAction action, VM vm, int start, int end)
        {
        	// Do database sync. Helps to ensure that the change persists over master failover.
            action.RelatedTask = XenAPI.Pool.async_sync_database(action.Session);
            action.PollToCompletion(start, end);
        }

        /*
        /// <summary>
        /// offer to bump ntol back up, if we can
        /// Asks the user if they want to increase ntol (since hypothetical_max might have gone up).
        /// </summary>
        // not in use at the moment, keep it for now
        //private void BumpNtol()
        //{
        //    if (this.Pool!= null && this.Pool.ha_enabled)
        //    {
        //        Dictionary<XenRef<VM>, string> config = Helpers.GetVmHaRestartPrioritiesForApi(Helpers.GetVmHaRestartPriorities(this.Pool.Connection));
        //        long max = XenAPI.Pool.ha_compute_hypothetical_max_host_failures_to_tolerate(this.Session, config);
        //        long currentNtol = this.Pool.ha_host_failures_to_tolerate;

        //        if (currentNtol < max)
        //        {
        //            bool doit = false;

        //            Program.Invoke(Program.MainWindow, delegate()
        //            {
        //                string poolName = Helpers.TrimStringIfRequired(Helpers.GetName(this.Pool), 500);
        //                string msg = String.Format(Messages.HA_OPT_ENABLE_NTOL_RAISE_QUERY, poolName, currentNtol, max);

        //                if (new ThreeButtonDialog(
        //                        new ThreeButtonDialog.Details(null, msg, Messages.HIGH_AVAILABILITY),
        //                        ThreeButtonDialog.ButtonYes,
        //                        new ThreeButtonDialog.TBDButton(Messages.NO, DialogResult.No, ThreeButtonDialog.ButtonType.CANCEL, true)).ShowDialog(Program.MainWindow) == DialogResult.Yes)                                         
        //                {
        //                    doit = true;
        //                }
        //            });

        //            if (doit)
        //            {
        //                XenAPI.Pool.set_ha_host_failures_to_tolerate(this.Session, this.Pool.opaque_ref, max);
        //            }
        //        }
        //    }
        //}
        */
    }
}
