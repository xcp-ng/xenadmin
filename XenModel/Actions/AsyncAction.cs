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
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public abstract class AsyncAction : CancellingAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Func<List<Role>, IXenConnection, string, SudoElevationResult> sudoDialog = XenAdminConfigManager.Provider.SudoDialogDelegate;

        private string _result;

        protected AsyncAction(IXenConnection connection, string title, string description, bool suppressHistory)
            : base(title, description, suppressHistory)
        {
            this.Connection = connection;
            Pool = Helpers.GetPoolOfOne(connection);
        }

        protected AsyncAction(IXenConnection connection, string title, string description)
            : this(connection, title, description, false)
        {
        }

        protected AsyncAction(IXenConnection connection, string title)
            : this(connection, title, "", false)
        {
        }

        protected AsyncAction(IXenConnection connection, string title, bool suppress_history)
            : this(connection, title, "", suppress_history)
        {
        }

        public string Result
        {
            get
            {
                if (Exception != null)
                {
                    throw Exception;
                }
                else
                {
                    return _result;
                }
            }
            set { _result = value; }
        }

        /// <summary>
        /// If you want the action to run a pre-check that the current user can perform all the necessary api calls, list them under this field.
        /// If empty, then no checks will be run.
        /// </summary>
        protected RbacMethodList ApiMethodsToRoleCheck = new RbacMethodList();
        public virtual RbacMethodList GetApiMethodsToRoleCheck
        {
            get { return ApiMethodsToRoleCheck; }
        }

        /// <summary>
        /// If the sudoUsername and sudoPassword fields are both not null, then the action will use these credentials when making new sessions.
        /// </summary>
        public string sudoUsername;
        /// <summary>
        /// If the sudoUsername and sudoPassword fields are both not null, then the action will use these credentials when making new sessions.
        /// </summary>
        public string sudoPassword;

        /// <summary>
        /// Checks to see if we are using elevated credentials for this action. Returns a session using them if they exist, otherwise
        /// using the basic credentials of the IXenConnection. Important - will throw exceptions similar to connection.NewSession
        /// </summary>
        /// <returns></returns>
        public override Session NewSession()
        {
            if (Connection == null)
                return null;

            if (String.IsNullOrEmpty(sudoPassword) || String.IsNullOrEmpty(sudoUsername))
                return Connection.DuplicateSession();

            return Connection.ElevatedSession(sudoUsername, sudoPassword);
        }

        /// <summary>
        /// Used for cross connection actions (e.g adding a host to a pool, we need to get a session from the connection we are joining)
        /// Checks to see if we are using elevated credentials for this action. Returns a session using them if they exist, otherwise
        /// using the basic credentials of the _supplied_ IXenConnection. Important - will throw exceptions similar to connection.NewSession
        /// </summary>
        /// <param name="xc"></param>
        /// <returns></returns>
        public Session NewSession(IXenConnection xc)
        {
            if (Connection == null)
                return null;

            if (String.IsNullOrEmpty(sudoPassword) || String.IsNullOrEmpty(sudoUsername))
                return xc.DuplicateSession();

            return xc.ElevatedSession(sudoUsername, sudoPassword);
        }



        
        public static bool ForcedExiting
        {
            get { return XenAdminConfigManager.Provider.ForcedExiting; }
        }

        /// <summary>
        /// Prepare the action's task for exit by removing the XenCenterUUID.
        /// A call here just before exit will mean that the task will get picked 
        /// up as a meddling action on restart of xencenter, and thus reappear in the log.
        /// </summary>
        public void PrepareForLogReloadAfterRestart()
        {
            try
            {
                Task.RemoveXenCenterUUID(Session, RelatedTask.opaque_ref);
            }
            catch(KeyNotFoundException)
            {
                log.Debug("Removing XenCenterUUID failed - KeyNotFound");
            }
            catch(NullReferenceException)
            {
                log.Debug("Removing XenCenterUUID failed - NullReference");
            }
            catch (WebException)
            {
                log.Debug("Removing XenCenterUUID failed - Could not connect through http");
            }
        }

        public virtual void RunAsync()
        {
            AuditLogStarted();
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(RunWorkerThread), null);
        }

        /// <summary>
        /// Use this function to run this action non-async, but do the appropriate tidy up code.
        /// If a session is passed in, which it always should be if called from another Action,
        /// use that session for the action: it is then the responsibility of the calling function
        /// to make sure the session has the appropriate privileges and tidy it up afterwards.
        /// </summary>
        public void RunExternal(Session session)
        {
            RunWorkerThread(session);
            if (Exception != null)
                throw Exception;
        }

        protected abstract void Run();


        /// <param name="o">A session to use for this action: if null, construct a new session and sudo it if necessary</param>
        private void RunWorkerThread(object o)
        {
            StartedRunning = true;
            if (Cancelled)  // already cancelled before it's started
                return;

            try
            {
                // Check that the current user credentials are enough to complete the api calls in this action (if specified)
                if (o != null)
                    Session = (Session)o;
                else
                    SetSessionByRole();
                Run();
                AuditLogSuccess();
                MarkCompleted();
            }
            catch (CancelledException e)
            {
                Cancelled = true;
                AuditLogCancelled();
                MarkCompleted(e);
            }
            catch (Exception e)
            {
                Failure f = e as Failure;
                if (f != null && Connection != null && f.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                {
                    Failure.ParseRBACFailure(f, Connection, Session ?? Connection.Session);
                }
                log.Error(e);
                AuditLogFailure();
                MarkCompleted(e);
            }
            finally
            {
                Clean();

                if (Exception != null)
                    CleanOnError();

                if (o == null && Session != null && Session.IsElevatedSession)
                {
                    // The session is a new, sudo-ed session: we need to log these ones out
                    try
                    {
                        Session.logout();
                    }
                    catch (Failure f)
                    {
                        log.Debug("Session.logout() failed. ", f);
                    }
                }

                Session = null;
                LogoutCancelSession();
            }
        }

        public virtual void DestroyTask()
        {
            //Program.AssertOffEventThread();

            if (Session == null || string.IsNullOrEmpty(Session.opaque_ref) || RelatedTask == null)
                return;

            try
            {
                PerformSilentTaskOp(delegate() { XenAPI.Task.destroy(Session, RelatedTask); });
            }
            finally
            {
                RelatedTask = null;
            }
        }

        public void PollToCompletion(int start, int finish)
        {
            new TaskPoller(this, start, finish).PollToCompletion();
        }

        public void PollToCompletion(double start, double finish)
        {
            PollToCompletion((int)start, (int)finish);
        }

        /// <summary>
        /// Equivalent to PollToCompletion(0, 100).
        /// </summary>
        public void PollToCompletion()
        {
            PollToCompletion(0, 100);
        }

        /// <summary>
        /// If the action has detailed the Api calls that it will make then find the set of roles that can complete the entire action. If the
        /// current user's role is not on that list then show the Role Elevation Dialog to give them the oppertunity to change to a new user.
        /// </summary>
        private void SetSessionByRole()
        {
            if (Connection == null
                || Connection.Session == null
                || Session != null) // We have been pre-seeded with a Session to use
                return;

            RbacMethodList rbacMethodList;

            if (Connection.Session.IsLocalSuperuser || XenAdminConfigManager.Provider.DontSudo)  // don't need / want to sudo
                rbacMethodList = new RbacMethodList();
            else
                rbacMethodList = GetApiMethodsToRoleCheck;

            if (rbacMethodList.Count == 0)
            {
                Session = NewSession();
                return;
            }

            List<Role> rolesAbleToCompleteAction;
            bool ableToCompleteAction = Role.CanPerform(rbacMethodList, Connection, out rolesAbleToCompleteAction);

            log.DebugFormat("Roles able to complete action: {0}", Role.FriendlyCSVRoleList(rolesAbleToCompleteAction));
            log.DebugFormat("Subject {0} has roles: {1}", Connection.Session.UserLogName(), Role.FriendlyCSVRoleList(Connection.Session.Roles));

            if (ableToCompleteAction)
            {
                log.Debug("Subject authorized to complete action");
                Session = Connection.Session;
                return;
            }

            log.Debug("Subject not authorized to complete action, showing sudo dialog");
            var result = sudoDialog(rolesAbleToCompleteAction, Connection, Title);
            if (result.Result)
            {
                sudoUsername = result.ElevatedUsername;
                sudoPassword = result.ElevatedPassword;
                Session = result.ElevatedSession;
                return;
            }
            else
            {
                log.Debug("User cancelled sudo dialog, cancelling action");
                throw new CancelledException();
            }
        }

        public class SudoElevationResult
        {
            public readonly string ElevatedUsername;
            public readonly string ElevatedPassword;
            public readonly Session ElevatedSession;
            public readonly bool Result;

            public SudoElevationResult(bool result, string user, string password, Session session)
            {
                Result = result;
                ElevatedUsername = user;
                ElevatedPassword = password;
                ElevatedSession = session;
            }
        }

        protected static void BestEffort(ref Exception caught, bool expectDisruption, Action func)
        {
            try
            {
                func();
            }
            catch (Exception exn)
            {
                if (expectDisruption &&
                    exn is WebException && ((WebException)exn).Status == WebExceptionStatus.KeepAliveFailure)  // ignore keep-alive failures if disruption is expected
                {
                    return;
                }

                log.Error(exn, exn);
                if (caught == null)
                {
                    caught = exn;
                }
            }
        }

        protected void BestEffort(ref Exception caught, Action func)
        {
            BestEffort(ref caught, Connection != null && Connection.ExpectDisruption, func);
        }

        protected void AddCommonAPIMethodsToRoleCheck()
        {
            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);
        }
    }
}
