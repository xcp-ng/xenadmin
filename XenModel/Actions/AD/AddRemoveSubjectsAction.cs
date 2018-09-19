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
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public class AddRemoveSubjectsAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<string> subjectNamesToAdd;
        private readonly List<Subject> subjectsToRemove;

        /// <summary>
        /// Progress through this action
        /// </summary>
        private int stepsComplete = 0;

        private int steps;

        /// <summary>
        /// If we remove a subject from the server we are currently using we need to log it out by hand. This value indicates whether we need to perform this aciton or not.
        /// </summary>
        private bool logoutSession = false;

        /// <summary>
        /// Successfully resolved ids we will create subjects out of
        /// </summary>
        private List<string> sidsToAdd;

        public delegate void NameResolvedEventHandler(object sender, string enteredName, string resolvedName, string sid, Exception exception);
        /// <summary>
        /// Fired for each event after resolution is attempted. If succeeded exception is null, and we pass the full resolved name and sid.
        /// </summary>
        public event NameResolvedEventHandler NameResolveComplete;

        public delegate void AllNamesResolvedEventHandler();
        /// <summary>
        /// Fired when all resolutions have finished.
        /// </summary>
        public event AllNamesResolvedEventHandler AllResolveComplete;

        public delegate void SubjectAddedEventHandler(object sender, Subject subject, Exception exception);
        /// <summary>
        /// Event for when a single subject add has completed. If succeeded exception is null.
        /// </summary>
        public event SubjectAddedEventHandler SubjectAddComplete;

        public delegate void SubjectRemovedEventHandler(object sender, string sid, Exception exception);
        /// <summary>
        /// Event for when a single subject remove has completed. If succeeded exception is null.
        /// </summary>
        public event SubjectRemovedEventHandler SubjectRemoveComplete;


        public AddRemoveSubjectsAction(IXenConnection connection, List<string> subjectNamesToAdd, List<Subject> subjectsToRemove)
            : base(connection, 
                string.Format(Messages.AD_ADDING_REMOVING_ON, Helpers.GetName(connection).Ellipsise(50)), 
                Messages.AD_ADDING_REMOVING, false)
        {
            var pool = Helpers.GetPool(connection);
            if (pool != null)
                Pool = pool;
            else
                Host = Helpers.GetMaster(connection);
            this.subjectNamesToAdd = subjectNamesToAdd;
            this.subjectsToRemove = subjectsToRemove;

#region RBAC checks

            if (subjectNamesToAdd != null && subjectNamesToAdd.Count > 0)
            {
                ApiMethodsToRoleCheck.Add("subject.create");
            }
            if (subjectsToRemove != null && subjectsToRemove.Count > 0)
            {
                ApiMethodsToRoleCheck.Add("session.logout_subject_identifier");
                ApiMethodsToRoleCheck.Add("subject.destroy");
            }
#endregion

        }

        protected override void Run()
        {
            // clear out dupes
            Dictionary<string, object> sNames = new Dictionary<string, object>();
            List<string> uniqueList = new List<string>();
            foreach (string name in subjectNamesToAdd)
            {
                if (sNames.ContainsKey(name))
                    continue;

                sNames.Add(name, null);
                uniqueList.Add(name);
            }
            subjectNamesToAdd = uniqueList;

            // for each entry to add we must resolve it, then add it. Then we do the removes.
            steps = subjectNamesToAdd.Count * 2 + subjectsToRemove.Count;
            sidsToAdd = new List<string>();

            resolveSubjects();
            addResolvedSubjects();
            removeSubjects();

            // We have only only kept track of the latest exception no matter how many occurred - refer the user to the logs for full info.
            Description = Exception == null ? Messages.COMPLETED : Messages.COMPLETED_WITH_ERRORS;

            if (logoutSession)
                Connection.Logout();
        }

        private void resolveSubjects()
        {
            Exception e = null;
            string resolvedName = "";
            string sid = "";
            log.DebugFormat("Resolving AD entries on pool '{0}'", Helpers.GetName(Connection).Ellipsise(50));
            foreach (string name in subjectNamesToAdd)
            {
                try
                {
                    sid = Auth.get_subject_identifier(Session, name);
                    sidsToAdd.Add(sid);
                    if (!Auth.get_subject_information_from_identifier(Session, sid).TryGetValue(Subject.SUBJECT_NAME_KEY, out resolvedName))
                        resolvedName = Messages.UNKNOWN_AD_USER;  
                }
                catch (Failure f)
                {
                    if (f.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                        Failure.ParseRBACFailure(f,Connection,Session);

                    Exception = f;
                    log.Warn(string.Format("Exception resolving AD user {0}", name), f);
                    e = f;
                }
                finally
                {
                    if (NameResolveComplete != null)
                    {
                        NameResolveComplete(this, name, resolvedName, sid, e);
                    }
                    e = null;
                    sid = resolvedName = "";
                }
                stepsComplete++;
                PercentComplete = (100 * stepsComplete) / steps;
            }

            if (AllResolveComplete != null)
            {
                AllResolveComplete();
            }
        }

        private void addResolvedSubjects()
        {
            Exception e = null;
            Subject subject = null;
            log.DebugFormat("Adding {0} new subjects on pool '{1}'", sidsToAdd.Count, Helpers.GetName(Connection).Ellipsise(50));
            foreach (string sid in sidsToAdd)
            {
                try
                {
                    // We pass this object back even if it fails so we know who we are talking about
                    subject = new Subject();
                    subject.subject_identifier = sid;
                    subject.other_config = Auth.get_subject_information_from_identifier(Session, sid);

                    // Check that this subject doesn't already exist
                    foreach (Subject s in Connection.Cache.Subjects)
                    {
                        if (s.subject_identifier == sid)
                        {
                            log.WarnFormat("A Subject with sid {0} already exists on the server.", sid);
                            string subjectName = (subject.DisplayName ?? subject.SubjectName ?? "").Ellipsise(50);
                            throw new Exception(String.Format(Messages.AD_USER_ALREADY_HAS_ACCESS, subjectName));
                        }
                    }
                    
                    XenAPI.Subject.create(Session, subject);
                }
                catch (Exception ex)
                {
                    Failure f = ex as Failure;
                    if (f != null && f.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                        Failure.ParseRBACFailure(f,Connection,Session);

                    Exception = ex;
                    log.Warn("Exception adding AD user to subject list", ex);
                    e = ex;
                }
                finally
                {
                    if (SubjectAddComplete != null)
                    {
                        SubjectAddComplete(this, subject, e);
                    }
                    e = null;
                    subject = null;
                }
                stepsComplete++;
                PercentComplete = (100 * stepsComplete) / steps;
            }
        }

        private void removeSubjects()
        {
            Exception e = null;
            log.DebugFormat("Removing {0} existing subjects on pool '{1}'", subjectsToRemove.Count, Helpers.GetName(Connection).Ellipsise(50));
            string selfSid = Connection.Session.IsLocalSuperuser || Connection.Session.Subject == null
                ? ""
                : Connection.Resolve(Connection.Session.Subject).subject_identifier;

            foreach (Subject subject in subjectsToRemove)
            {
                string sid = subject.subject_identifier;
                try
                {
                    if (!Connection.Session.IsLocalSuperuser && selfSid == sid)
                    {
                        // Committing suicide. We will log ourselves out later.
                        logoutSession = true;
                    }
                    else
                    {
                        Session.logout_subject_identifier(Session, sid);
                    }
                    XenAPI.Subject.destroy(Session, subject.opaque_ref);
                    // We look at the session subject as this is the authority under which we are connected. 
                    // (deliberate use of the original session for subject analysis... the sudo session is not the one we want to interrogate
                    
                }
                catch (Exception ex)
                {
                    Failure f = ex as Failure;
                    if (f != null && f.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                        Failure.ParseRBACFailure(f, Connection, Session);

                    Exception = ex;
                    log.Warn("Exception removing AD user to subject list", ex);
                    e = ex;
                }
                finally
                {
                    if (SubjectRemoveComplete != null)
                    {
                        SubjectRemoveComplete(this, sid, e);
                    }
                    e = null;
                }
                stepsComplete++;
                PercentComplete = (100 * stepsComplete) / steps;
            }
        }
    }
}
