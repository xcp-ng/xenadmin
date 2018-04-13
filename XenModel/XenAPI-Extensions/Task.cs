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
using System.Net;
using System.Reflection;
using System.Threading;
using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Network;


namespace XenAPI
{
    partial class Task
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public delegate task_status_type TaskStatusOp(Session session, string task);
        public delegate double TaskProgressOp(Session session, string task);
        public delegate Task TaskGetRecordOp(Session session, string task);

        private static readonly string XapiExportPrefix = "Export of VM: ";

        private static readonly Dictionary<string, string> Names = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> Titles = new Dictionary<string, string>();
        static Task()
        {
            Names["VM.clean_reboot"] = Messages.ACTION_VM_REBOOTING;
            Names["VM.clean_shutdown"] = Messages.ACTION_VM_SHUTTING_DOWN;
            Names["VM.clone"] = Messages.ACTION_VM_COPYING;
            // Export is dealt with specially.
            // Names["VM.export"] = ...;
            Names["VM.hard_reboot"] = Messages.ACTION_VM_REBOOTING;
            Names["VM.hard_shutdown"] = Messages.ACTION_VM_SHUTTING_DOWN;
            Names["VM.migrate_send"] = Messages.ACTION_VM_MIGRATING;
            Names["VM.pool_migrate"] = Messages.ACTION_VM_MIGRATING;
            Names["VM.resume"] = Messages.ACTION_VM_RESUMING;
            Names["VM.resume_on"] = Messages.ACTION_VM_RESUMING;
            Names["VM.start"] = Messages.ACTION_VM_STARTING;
            Names["VM.start_on"] = Messages.ACTION_VM_STARTING;
            Names["VM.suspend"] = Messages.ACTION_VM_SUSPENDING;
            Names["VM.checkpoint"] = Messages.SNAPSHOTTING;
            Names["VM.snapshot"] = Messages.SNAPSHOTTING;

            Names["VM import"] = Messages.IMPORTING;
            Names["Import of Zurich/Geneva style XVA"] = Messages.IMPORTING;

            Titles["VM.clean_reboot"] = Messages.ACTION_VM_REBOOTING_TITLE;
            Titles["VM.clean_shutdown"] = Messages.ACTION_VM_SHUTTING_DOWN_TITLE;
            Titles["VM.clone"] = Messages.ACTION_VM_COPYING_TITLE_MEDDLING;
            // Export is dealt with specially.
            // Titles["VM.export"] = ...;
            Titles["VM.hard_reboot"] = Messages.ACTION_VM_REBOOTING_TITLE;
            Titles["VM.hard_shutdown"] = Messages.ACTION_VM_SHUTTING_DOWN_TITLE;
            Titles["VM.migrate_send"] = Messages.ACTION_VM_MIGRATING_RESIDENT;
            Titles["VM.pool_migrate"] = Messages.ACTION_VM_MIGRATING_RESIDENT;
            Titles["VM.resume"] = Messages.ACTION_VM_RESUMING_ON_TITLE;
            Titles["VM.resume_on"] = Messages.ACTION_VM_RESUMING_ON_TITLE;
            Titles["VM.start"] = Messages.ACTION_VM_STARTING_ON_TITLE;
            Titles["VM.start_on"] = Messages.ACTION_VM_STARTING_ON_TITLE;
            Titles["VM.suspend"] = Messages.ACTION_VM_SUSPENDING_TITLE;
            Titles["VM.checkpoint"] = Messages.ACTION_VM_SNAPSHOT_TITLE;
            Titles["VM.snapshot"] = Messages.ACTION_VM_SNAPSHOT_TITLE;

            Titles["VM import"] = Messages.IMPORTVM_TITLE;
            Titles["Import of Zurich/Geneva style XVA"] = Messages.IMPORTVM_TITLE;
        }

        /// <summary>
        /// Try and run the delegate.
        /// If it fails with a web exception or invalid session, try again.
        /// Only retry 60 times. 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="session"></param>
        /// <param name="f"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Object DoWithSessionRetry(IXenConnection connection, ref Session session, Delegate f, params object[] p)
        {
            int retries = 60;

            while (true)
            {
                try
                {
                    object[] ps = new object[p.Length + 1];

                    ps[0] = session;

                    for (int i = 0; i < p.Length; i++)
                    {
                        ps[i + 1] = p[i];
                    }

                    try
                    {
                        return f.DynamicInvoke(ps);
                    }
                    catch (TargetInvocationException exn)
                    {
                        throw exn.InnerException;
                    }
                }
                catch (WebException we)
                {
                    log.ErrorFormat("WebException in DoWithSessionRetry, retry {0}", retries);
                    log.Error(we, we);

                    if (retries <= 0)
                        throw;
                }
                catch (Failure failure)
                {
                    log.ErrorFormat("Failure in DoWithSessionRetry, retry {0}", retries);
                    log.Error(failure, failure);

                    if (retries <= 0)
                        throw;

                    if (failure.ErrorDescription.Count < 1 || failure.ErrorDescription[0] != XenAPI.Failure.SESSION_INVALID)
                        throw;
                }

                Session newSession = connection.DuplicateSession();

                try
                {
                    // Try and logout the old session using the new session
                    newSession.logout(session.opaque_ref);
                }
                catch
                {
                }

                session = newSession;

                retries--;

                Thread.Sleep(connection.ExpectDisruption ? 500 : 100);
            }
        }



        /// <summary>
        /// A list of OpaqueRefs of objects to which the current task applies.  This is set
        /// by one XenCenter instance, and picked up by other ones.
        /// </summary>
        public List<string> AppliesTo()
        {
            string s = Get(other_config, "applies_to");
            return s == null ? ExportAppliesTo() : new List<string>(s.Split(','));
        }

        private List<string> ExportAppliesTo()
        {
            VM vm = ExportAppliesToVM();
            if (vm == null)
            {
                return null;
            }
            else
            {
                List<string> result = new List<string>(1);
                result.Add(vm.opaque_ref);
                return result;
            }
        }

        /// <returns>The VM that this Task applies to, or null if we don't know, or it's not an export.</returns>
        private VM ExportAppliesToVM()
        {
            if (name_label.StartsWith(XapiExportPrefix))
            {
                if (Connection == null)
                    return null;
                string uuid = name_label.Substring(XapiExportPrefix.Length);
                return Connection.Cache.Find_By_Uuid<VM>(uuid);
            }
            else
            {
                return null;
            }
        }

        public static void SetAppliesTo(Session session, string _task, List<string> applies_to)
        {
            try
            {
                remove_from_other_config(session, _task, "applies_to"); 
                add_to_other_config(session, _task, "applies_to", string.Join(",", applies_to.ToArray()));
            }
            catch (XenAPI.Failure f)
            {
                if (f.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                {
                    // Read only user without task.other_config rights - just ignore this request
                    return;
                }
                throw f;
            }
        }

        public string XenCenterUUID()
        {
            return Get(other_config, "XenCenterUUID");
        }

        public static void SetXenCenterUUID(Session session, string _task, string uuid)
        {
            try
            {
                remove_from_other_config(session, _task, "XenCenterUUID");
                add_to_other_config(session, _task, "XenCenterUUID", uuid);
            }
            catch (XenAPI.Failure f)
            {
                if (f.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                {
                    // Read only user without task.other_config rights - just ignore this request
                    return;
                }
                throw f;
            }
        }

        public static void RemoveXenCenterUUID(Session session, string task)
        {
            if(session == null || String.IsNullOrEmpty(task))
                return;

            try
            {
                remove_from_other_config(session, task, "XenCenterUUID");
            }
            catch (XenAPI.Failure f)
            {
                if (f.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                {
                    // Read only user without task.other_config rights - just ignore this request
                    return;
                }
                throw;
            }
        }

        /// <summary>
        /// True if the task should be completely hidden (i.e. Name == null).
        /// </summary>
        public override bool IsHidden()
        {
            return Name() == null;
        }

        /// <summary>
        /// The friendly name to use for this task, or null
        /// if this task is to be completely hidden (e.g. SR.scan).
        /// </summary>
        public override string Name()
        {
            string nl = name_label.Replace("Async.", "");
            return nl.StartsWith(XapiExportPrefix)
                ? ExportName()
                : Names.ContainsKey(nl)
                    ? Names[nl]
                    : other_config.ContainsKey("ShowInXenCenter")
                        ? name_label
                        : null;
        }

        public override string Description()
        {
            return name_description;
        }

        private string ExportName()
        {
            VM vm = ExportAppliesToVM();
            return
                vm == null ?
                    Messages.EXPORTING :
                    string.Format(Messages.ACTION_EXPORT_TASK_NAME, vm.name_label);
        }

        /// <summary>
        /// The title to use for this task, or null
        /// if this task is to be completely hidden (e.g. SR.scan).
        /// </summary>
        public string Title()
        {
            string nl = name_label.Replace("Async.", "");
            if (nl.StartsWith(XapiExportPrefix))
                return ExportName();
            if (AppliesTo() != null && Titles.ContainsKey(nl))
            {
                try
                {
                    return string.Format(Titles[nl], (object[])GetAppliesToNames().ToArray());
                }
                catch
                {
                } // can crash if GetAppliesToNames() can't find enough names: CA-29493
            }
            if (Names.ContainsKey(nl))
                return Names[nl];
            return null;
        }

        private List<string> GetAppliesToNames()
        {
            
            VM vm = null;
            Host host1 = null;
            Host host2 = null;
            var appliesTo = AppliesTo();
            foreach (string r in appliesTo)
            {
                VM _vm = Connection.Resolve(new XenRef<VM>(r));
                if (_vm != null)
                {
                    vm = _vm;
                    continue;
                }

                Host _host = Connection.Resolve(new XenRef<Host>(r));
                if (_host != null)
                {
                    if (host1 == null)
                        host1 = _host;
                    else
                        host2 = _host;
                    continue;
                }
            }

            List<string> result = new List<string>();

            if (vm != null)
                result.Add(vm.name_label);
            if (host1 != null)
                result.Add(host1.name_label);
            if (host2 != null)
                result.Add(host2.name_label);

            return result;
        }

        private static List<string> tasksToIgnore = new List<string>(new string[] { "SR.scan" });
        public bool IgnoreInCacheUpdate()
        {
            return tasksToIgnore.Contains(name_label);
        }
    }
}
