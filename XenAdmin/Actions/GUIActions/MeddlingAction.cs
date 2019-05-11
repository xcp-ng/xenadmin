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
using System.Linq;
using System.Threading;
using XenAPI;
using XenAdmin.Core;

namespace XenAdmin.Actions.GUIActions
{
    /// <summary>
    /// A "meddling" Action is one being performed by someone else; in other words,
    /// they are ones that we've inferred by the presence of task instances on the pool.
    /// </summary>
    public class MeddlingAction : CancellingAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Heuristic to determine whether a new task was created by a client
        /// aware of our task.AppliesTo scheme, or by some other client.
        /// </summary>
        private static readonly TimeSpan awareClientHeuristic = TimeSpan.FromSeconds(5);

        private static readonly string XapiExportTaskPrefix = "Export of VM: ";
        private static readonly string XapiImportTaskName = "VM import";

        private static readonly List<vm_operations> RecognisedVmOperations = new List<vm_operations>
        {
            vm_operations.clean_reboot,
            vm_operations.clean_shutdown,
            vm_operations.clone,
            vm_operations.hard_reboot,
            vm_operations.hard_shutdown,
            vm_operations.migrate_send,
            vm_operations.pool_migrate,
            vm_operations.resume,
            vm_operations.resume_on,
            vm_operations.start,
            vm_operations.start_on,
            vm_operations.suspend,
            vm_operations.checkpoint,
            vm_operations.snapshot,
            vm_operations.export,
            vm_operations.import
        };

        private vm_operations vmOperation;

        public MeddlingAction(Task task)
            : base(task.Name(), task.Description(), false, false)
        {
            RelatedTask = new XenRef<Task>(task.opaque_ref);

            Host = task.Connection.Resolve(task.resident_on) ?? Helpers.GetMaster(task.Connection);

            Started = (task.created + task.Connection.ServerTimeOffset).ToLocalTime();
            SetAppliesToData(task);
            Connection = task.Connection;
            VM = GetVm(task);
            vmOperation = GetVmOperation(task);
            UpdateActionTitleAndDescription(task);
            Update(task, false);
        }

        public void Update(Task task, bool deleting)
        {
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 if (!deleting)
                                                 {
                                                     RecomputeCanCancel();
                                                 }

                                                 if (task == null || IsCompleted)
                                                     return;

                                                 log.DebugFormat("Updating task {0} : {1} - {2}", task.opaque_ref,
                                                                 task.created, task.finished);
                                                 
                                                 int percentComplete = task.progress < 0 ? 0 : (int) (100.0*task.progress);
                                                 if (PercentComplete < percentComplete)
                                                    PercentComplete = percentComplete;

                                                 SetFatalErrorData(task);

                                                 DetermineIfTaskIsComplete(task, deleting);

                                                 if (IsCompleted)
                                                     Description = Messages.COMPLETED;

                                                 DestroyUnwantedOperations(task);

                                                 if (deleting)
                                                     LogoutCancelSession();
                                             });
        }

        private void DetermineIfTaskIsComplete(Task task, bool deleting)
        {
            if (task.finished.Year > 1970)
            {
                DateTime t = task.finished + task.Connection.ServerTimeOffset;
                Finished = t.ToLocalTime();
                IsCompleted = true;
            }
            else if (deleting)
            {
                Finished = DateTime.Now;
                IsCompleted = true;
            }
            else
            {
                StartedRunning = true;
            }
        }

        private void SetFatalErrorData(Task task)
        {
            string[] err = task.error_info;
            if (err != null && err.Length > 0)
                Exception = new Failure(err);
            else if (task.status == task_status_type.cancelled)
                Exception = new CancelledException();
        }

        private void SetAppliesToData(Task task)
        {
            List<string> applies_to = task.AppliesTo();
            if (applies_to != null)
            {
                AppliesTo.AddRange(applies_to);
            }
            else
            {
                // A non-aware client has created this task.  We'll create a new action for this, and place it under
                // the task.resident_on host, or if that doesn't resolve, the pool master.
                Host host = task.Connection.Resolve(task.resident_on) ?? Helpers.GetMaster(task.Connection);
                if (host != null)
                    AppliesTo.Add(host.opaque_ref);
            }
        }

        private VM GetVm(Task task)
        {
            // try to find the VM in AppliesTo
            foreach (string r in AppliesTo)
            {
                VM vm = task.Connection.Resolve(new XenRef<VM>(r));
                if (vm != null)
                    return vm;
            }

            // try to find a VM in the cache which has this task in its current_operations
            return task.Connection.Cache.VMs.FirstOrDefault(vm => vm.current_operations.Keys.Contains(task.opaque_ref));
        }

        private static vm_operations GetVmOperation(Task task)
        {
            string nl = task.name_label.Replace("Async.", "");
            if (nl.StartsWith("VM."))
            {
                nl = nl.Replace("VM.", "");
                if (Enum.TryParse(nl, out vm_operations vmOperation) && RecognisedVmOperations.Contains(vmOperation))
                {
                    return vmOperation;
                }
            }
            else
            {
                // other tasks, e.g. export, import
                if (task.name_label == XapiImportTaskName)
                    return vm_operations.import;
                if (task.name_label.StartsWith(XapiExportTaskPrefix))
                    return vm_operations.export;
            }

            return vm_operations.unknown;
        }

        private void UpdateActionTitleAndDescription(Task task)
        {
            Host host1 = null;
            Host host2 = null;
            var appliesTo = task.AppliesTo();
            if (appliesTo != null)
            {
                foreach (string r in appliesTo)
                {
                    var host = Connection.Resolve(new XenRef<Host>(r));
                    if (host == null)
                        continue;
                    if (host1 == null)
                        host1 = host;
                    else
                        host2 = host;
                }
            }
            else
            {
                host1 = task.Connection.Resolve(task.resident_on) ?? Helpers.GetMaster(task.Connection);
            }

            List<string> names = new List<string>();

            if (VM != null)
                names.Add(VM.name_label);
            if (host1 != null)
                names.Add(host1.name_label);
            if (host2 != null)
                names.Add(host2.name_label);

            string titleFormat;
            switch (vmOperation)
            {
                case vm_operations.clean_reboot:
                case vm_operations.hard_reboot:
                    titleFormat = names.Count > 1 ? Messages.ACTION_VM_REBOOTING_ON_TITLE : Messages.ACTION_VM_REBOOTING_TITLE;
                    Description = Messages.ACTION_VM_REBOOTING;
                    break;
                case vm_operations.clean_shutdown:
                case vm_operations.hard_shutdown:
                    titleFormat = names.Count > 1 ? Messages.ACTION_VM_SHUTTING_DOWN_ON_TITLE : Messages.ACTION_VM_SHUTTING_DOWN_TITLE;
                    Description = Messages.ACTION_VM_SHUTTING_DOWN;
                    break;
                case vm_operations.clone:
                    titleFormat = Messages.ACTION_VM_COPYING_TITLE_MEDDLING;
                    Description = Messages.ACTION_VM_COPYING;
                    break;
                case vm_operations.migrate_send:
                case vm_operations.pool_migrate:
                    titleFormat = names.Count > 2 ? Messages.ACTION_VM_MIGRATING_RESIDENT : Messages.ACTION_VM_MIGRATING_TITLE;
                    Description = Messages.ACTION_VM_MIGRATING;
                    break;
                case vm_operations.resume:
                case vm_operations.resume_on:
                    titleFormat = names.Count > 1 ? Messages.ACTION_VM_RESUMING_ON_TITLE : Messages.ACTION_VM_RESUMING_TITLE;
                    Description = Messages.ACTION_VM_RESUMING;
                    break;
                case vm_operations.start:
                case vm_operations.start_on:
                    titleFormat = names.Count > 1 ? Messages.ACTION_VM_STARTING_ON_TITLE : Messages.ACTION_VM_STARTING_TITLE;
                    Description = Messages.ACTION_VM_STARTING;
                    break;
                case vm_operations.suspend:
                    titleFormat =Messages.ACTION_VM_SUSPENDING_TITLE;
                    Description = Messages.ACTION_VM_SUSPENDING;
                    break;
                case vm_operations.checkpoint:
                case vm_operations.snapshot:
                    titleFormat = Messages.ACTION_VM_SNAPSHOT_TITLE;
                    Description = Messages.SNAPSHOTTING;
                    break;
                case vm_operations.export:
                    titleFormat = Messages.ACTION_EXPORT_TASK_NAME;
                    Description = Messages.ACTION_EXPORT_DESCRIPTION_IN_PROGRESS;
                    break;
                case vm_operations.import:
                    titleFormat = VM == null ? Messages.IMPORTING : names.Count > 1 
                        ? Messages.ACTION_IMPORT_VM_TO_HOST_TITLE 
                        : Messages.ACTION_IMPORT_VM_TITLE;
                    Description = Messages.IMPORTING;
                    break;
                default:
                    titleFormat = task.name_label;
                    Description = task.name_description;
                    break;
            }

            try
            {
                Title = string.Format(titleFormat, names.ToArray());
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                Title = Description ?? task.name_label;
            }
        }

        /// <summary>
        /// This is one of our tasks, or it's a sub-task of something else, or it corresponds
        /// to an operation we don't care to recognize. We're going to do no more with it.
        /// </summary>
        public static bool IsTaskUnwanted(Task task)
        {
            return task.XenCenterUUID() == Program.XenCenterUUID ||
                   task.Connection.Resolve(task.subtask_of) != null ||
                   GetVmOperation(task) == vm_operations.unknown;
        }

        /// <summary>
        /// Decides whether a MeddlingAction can be created for a given task.
        /// If AppliesTo is set, then the client that created this task knows about our scheme for passing
        /// info between clients. 
        /// Otherwise, we give the client a window (awareClientHeuristic) to set this field before we decide
        /// that it's a non-aware client.
        /// </summary>
        public static bool IsTaskSuitable(Task task)
        {
            return task.AppliesTo() != null ||
                   task.created + task.Connection.ServerTimeOffset < DateTime.UtcNow - awareClientHeuristic;
        }

        private void DestroyUnwantedOperations(Task task)
        {
            string[] err = task.error_info;
            if (task.Name() == "SR.create" && err != null && err.Length > 0 && err[0] == Failure.SR_BACKEND_FAILURE_107)
            {
                // This isn't an SR create at all, it is a scan for LUNs. Hide it, since the 'error' info contains loads of XML,
                // and is not useful. We don't know this until the error occurs though. Destroy the MeddlingAction.
                task.PropertyChanged -= MeddlingActionManager.Task_PropertyChanged;
                ConnectionsManager.History.Remove(this);
            }
        }
    }
}
