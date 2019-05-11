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
using XenAdmin.Network;


namespace XenAPI
{
    partial class Task
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public delegate task_status_type TaskStatusOp(Session session, string task);
        public delegate double TaskProgressOp(Session session, string task);
        public delegate Task TaskGetRecordOp(Session session, string task);

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

                Session newSession;

                try
                {
                    // try to create a new TCP stream to use, as the other one has failed us
                    newSession = connection.DuplicateSession();
                    session = newSession;
                }
                catch
                {
                }

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
            return s == null ? null : new List<string>(s.Split(','));
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

        public override string Name()
        {
            return name_label.Replace("Async.", "");
        }

        public override string Description()
        {
            return name_description;
        }

        private static List<string> tasksToIgnore = new List<string>(new string[] { "SR.scan" });
        public bool IgnoreInCacheUpdate()
        {
            return tasksToIgnore.Contains(name_label);
        }
    }
}
