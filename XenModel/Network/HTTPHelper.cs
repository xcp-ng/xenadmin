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
using System.IO;
using System.Net;
using XenAdmin;
using XenCenterLib;
using XenAdmin.Network;
using XenAdmin.Actions;


namespace XenAPI
{

    public class HTTPHelper
    {
        public enum ProxyStyle
        {
            // Note that these numbers make it into user settings files, so need to be preserved.
            DirectConnection = 0,
            SystemProxy = 1,
            SpecifiedProxy = 2
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        //
        // The following functions do puts and gets to and from the filesystem
        // updating Actions etc where appropriate.
        //

        /// <summary>
        /// HTTP PUT file from path to HTTP action f, updating action with progress every 500ms.
        /// </summary>
        /// <param name="action">Action on which to update the progress</param>
         /// <param name="timeout">Whether to apply a timeout</param>
       /// <param name="path">path of file to put</param>
        public static String Put(AsyncAction action, bool timeout, string path, string hostname, Delegate f, params object[] p)
        {
            return Put(action, XenAdminConfigManager.Provider.GetProxyTimeout(timeout), path, hostname, f, p);
        }

        /// <summary>
        /// HTTP PUT file from path to HTTP action f, updating action with progress every 500ms.
        /// </summary>
        /// <param name="action">Action on which to update the progress</param>
        /// <param name="timeout">Timeout value in ms</param>
        /// <param name="path">path of file to put</param>
        public static String Put(AsyncAction action, int timeout, string path, string hostname, Delegate f, params object[] p)
        {
            Session session = action.Session;
            action.RelatedTask = XenAPI.Task.create(session, "uploadTask", hostname);

            try
            {
                HTTP.UpdateProgressDelegate progressDelegate = delegate(int percent)
                {
                    action.Tick(percent, action.Description);
                };

                return Put(progressDelegate, action.GetCancelling, timeout, action.Connection,
                    action.RelatedTask, ref session, path, hostname, f, p);
            }
            finally
            {
                action.Session = session;
                Task.destroy(session, action.RelatedTask);
            }
        }

        /// <summary>
        /// A general HttpPut method, with delegates for progress and cancelling
        /// </summary>
        /// <param name="progressDelegate">Delegate called periodically (500ms) with percent complete</param>
        /// <param name="cancellingDelegate">Delegate called periodically to see if need to cancel</param>
        /// <param name="timeout">Whether to apply a timeout</param>
        /// <param name="task">The task used to track this http get</param>
        /// <param name="session">Session used to make api calls (may be replaced if logged out, hence ref)</param>
        /// <param name="path">Path to file to put</param>
        /// <returns>The result of the task passed in</returns>
        public static String Put(XenAPI.HTTP.UpdateProgressDelegate progressDelegate, HTTP.FuncBool cancellingDelegate, bool timeout,
            IXenConnection connection, XenRef<Task> task, ref Session session, string path,
            string hostname, Delegate f, params object[] p)
        {
            return Put(progressDelegate, cancellingDelegate, XenAdminConfigManager.Provider.GetProxyTimeout(timeout),
                       connection, task, ref session, path, hostname, f, p);
        }

        /// <summary>
        /// A general HttpPut method, with delegates for progress and cancelling
        /// </summary>
        /// <param name="progressDelegate">Delegate called periodically (500ms) with percent complete</param>
        /// <param name="cancellingDelegate">Delegate called periodically to see if need to cancel</param>
        /// <param name="timeout">Timeout value in ms</param>
        /// <param name="task">The task used to track this http get</param>
        /// <param name="session">Session used to make api calls (may be replaced if logged out, hence ref)</param>
        /// <param name="path">Path to file to put</param>
        /// <returns>The result of the task passed in</returns>
        public static String Put(XenAPI.HTTP.UpdateProgressDelegate progressDelegate, HTTP.FuncBool cancellingDelegate, int timeout,
            IXenConnection connection, XenRef<Task> task, ref Session session, string path,
            string hostname, Delegate f, params object[] p)
        {
            log.DebugFormat("HTTP PUTTING file from {0} to {1}", path, hostname);

            HTTP.FuncBool cancellingDelegate2 = (HTTP.FuncBool)delegate()
            {
                return (XenAdminConfigManager.Provider.ForcedExiting ||
                    cancellingDelegate != null && cancellingDelegate());
            };

            try
            {
                List<object> args = new List<object>();
                args.Add(progressDelegate);
                args.Add(cancellingDelegate2);
                args.Add(timeout);
                args.Add(hostname);
                args.Add(XenAdminConfigManager.Provider.GetProxyFromSettings(connection));
                args.Add(path);
                args.Add(task.opaque_ref);  // task_id
                args.AddRange(p);
                f.DynamicInvoke(args.ToArray());
            }
            catch (Exception e)
            {
                log.Debug($"Caught exception doing HTTP PUT from {path} to {hostname}", e);
                PollTaskForResult(connection, ref session, cancellingDelegate2, task, true);
                if (e is CancelledException || e.InnerException is CancelledException)
                    throw new XenAdmin.CancelledException();
                else
                    throw;
            }

            return PollTaskForResult(connection, ref session, cancellingDelegate2, task);
        }

        /// <summary>
        /// HTTP GET file from HTTP action f, saving it to path (via a temporary file).
        /// </summary>
        /// <param name="action">Action on which to update the progress</param>
        /// <param name="timeout">Whether to apply a timeout</param>
        /// <param name="path">Path to save file to.</param>
        /// <returns>Result of the task used to GET the file</returns>
        public static String Get(AsyncAction action, bool timeout, string path, string hostname, Delegate f, params object[] p)
        {
            return Get(action, timeout, null, path, hostname, f, p);
        }

        /// <summary>
        /// HTTP GET file from HTTP action f, saving it to path (via a temporary file).
        /// </summary>
        /// <param name="action">Action on which to update the progress</param>
        /// <param name="timeout">Whether to apply a timeout</param>
        /// <param name="dataRxDelegate">Delegate called every 500ms with the data transferred</param>
        /// <param name="path">Path to save file to.</param>
        /// <returns>Result of the task used to GET the file</returns>
        public static String Get(AsyncAction action, bool timeout, HTTP.DataCopiedDelegate dataRxDelegate, string path, string hostname, Delegate f, params object[] p)
        {
            Session session = action.Session;
            action.RelatedTask = XenAPI.Task.create(session, "downloadTask", hostname);

            try
            {
                HTTP.UpdateProgressDelegate progressDelegate = delegate(int percent)
                    {
                        action.Tick(percent, action.Description);
                    };

                return HTTPHelper.Get(progressDelegate, action.GetCancelling, timeout, dataRxDelegate, action.Connection,
                    action.RelatedTask, ref session, path, hostname, f, p);
            }
            finally
            {
                action.Session = session;
                Task.destroy(session, action.RelatedTask);
            }
        }

        public static String Get(HTTP.UpdateProgressDelegate progressDelegate, HTTP.FuncBool cancellingDelegate, bool timeout,
            HTTP.DataCopiedDelegate dataRxDelegate, IXenConnection connection, XenRef<Task> task, ref Session session, string path,
            string hostname, Delegate f, params object[] p)
        {
            log.DebugFormat("HTTP GETTING file from {0} to {1}", hostname, path);

            // Cannot use ref param in anonymous method, so save it here and restore it later
            Session _session = session;

            HTTP.DataCopiedDelegate dataCopiedDelegate = delegate(long bytes)
            {
                if (progressDelegate != null)
                {
                    int progress = (int)(100 * (double)Task.DoWithSessionRetry(connection, ref _session,
                        (Task.TaskProgressOp)Task.get_progress, task.opaque_ref));

                    progressDelegate(progress);
                }

                if (dataRxDelegate != null)
                    dataRxDelegate(bytes);
            };

            HTTP.FuncBool cancellingDelegate2 = (HTTP.FuncBool)delegate()
            {
                return (XenAdminConfigManager.Provider.ForcedExiting ||
                    cancellingDelegate != null && cancellingDelegate());
            };

            try
            {
                List<object> args = new List<object>();
                args.Add(dataCopiedDelegate);
                args.Add(cancellingDelegate2);
                args.Add(XenAdminConfigManager.Provider.GetProxyTimeout(timeout));
                args.Add(hostname);
                args.Add(XenAdminConfigManager.Provider.GetProxyFromSettings(connection));
                args.Add(path);
                args.Add(task.opaque_ref);  // task_id
                args.AddRange(p);
                f.DynamicInvoke(args.ToArray());
            }
            catch (Exception e)
            {
                log.Debug($"Caught exception doing HTTP GET from {hostname} to {path}", e);

                if (e is WebException && e.InnerException is IOException && Win32.GetHResult(e.InnerException as IOException) == Win32.ERROR_DISK_FULL)
                    throw e.InnerException;
                else if (e is CancelledException || e.InnerException is CancelledException)
                    throw new XenAdmin.CancelledException();
                else if (e.InnerException.Message == "Received error code HTTP/1.1 403 Forbidden\r\n from the server")
                {
                    // RBAC Failure
                    List<Role> roles = connection.Session.Roles;
                    roles.Sort();
                    throw new Exception(String.Format(Messages.RBAC_HTTP_FAILURE, roles[0].FriendlyName()), e);
                }
                else
                    throw e.InnerException;
            }

            return PollTaskForResult(connection, ref session, cancellingDelegate2, task);
        }

        private const int POLL_FOR_TASK_RESULT_TIMEOUT = 2 * 60 * 1000; // 2 minutes
        private const int POLL_FOR_TASK_RESULT_SLEEP_INTERVAL = 500;
        
        private static String PollTaskForResult(IXenConnection connection, ref Session session,
            HTTP.FuncBool cancellingDelegate, XenRef<Task> task, bool timeout = false)
        {
            //Program.AssertOffEventThread();

            task_status_type status;
            int tries = POLL_FOR_TASK_RESULT_TIMEOUT / POLL_FOR_TASK_RESULT_SLEEP_INTERVAL;
            do
            {
                if (cancellingDelegate != null && cancellingDelegate())
                    throw new XenAdmin.CancelledException();

                System.Threading.Thread.Sleep(POLL_FOR_TASK_RESULT_SLEEP_INTERVAL);
                tries--;

                status = (task_status_type)Task.DoWithSessionRetry(connection, ref session,
                    (Task.TaskStatusOp)Task.get_status, task.opaque_ref);
            }
            while ((status == task_status_type.pending || status == task_status_type.cancelling) && (!timeout || tries > 0));

            if (cancellingDelegate != null && cancellingDelegate())
                throw new XenAdmin.CancelledException();

            if (status == task_status_type.failure)
            {
                throw new Failure(Task.get_error_info(session, task));
            }
            else
            {
                return Task.get_result(session, task);
            }
        }

        public static Stream CONNECT(Uri uri, IXenConnection connection, string session, bool timeout)
        {
            return HTTP.CONNECT(uri, XenAdminConfigManager.Provider.GetProxyFromSettings(connection), session, XenAdminConfigManager.Provider.GetProxyTimeout(timeout));
        }

        public static Stream PUT(Uri uri, long ContentLength, bool timeout)
        {
            return HTTP.PUT(uri, XenAdminConfigManager.Provider.GetProxyFromSettings(null), ContentLength, XenAdminConfigManager.Provider.GetProxyTimeout(timeout));
        }

        public static Stream GET(Uri uri, IXenConnection connection, bool timeout)
        {
            return HTTP.GET(uri, XenAdminConfigManager.Provider.GetProxyFromSettings(connection, true), XenAdminConfigManager.Provider.GetProxyTimeout(timeout));
        }
    }
}
