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
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public class AddRemoveRolesAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<Role> toAdd;
        private readonly List<Role> toRemove;
        private readonly Subject subject;

        public AddRemoveRolesAction(IXenConnection connection, Subject subject, List<Role> toAdd, List<Role> toRemove)
            : base(connection,
                string.Format(Messages.AD_ADDING_REMOVING_ROLES_ON, (subject.DisplayName ?? subject.SubjectName ?? subject.subject_identifier).Ellipsise(50)),
                Messages.AD_ADDING_REMOVING_ROLES,
                false)
        {
            var pool = Helpers.GetPool(connection);
            if (pool != null)
                Pool = pool;
            else
                Host = Helpers.GetMaster(connection);
            this.toAdd = toAdd;
            this.toRemove = toRemove;
            this.subject = subject;
        }

        protected override void Run()
        {
            int count = toAdd.Count + toRemove.Count;
            int done = 0;
            log.DebugFormat("Adding {0} roles on subject '{1}'", toAdd.Count, (subject.DisplayName ?? subject.SubjectName ?? subject.subject_identifier).Ellipsise(50));
            foreach (Role r in toAdd)
            {
                Subject.add_to_roles(Session, subject.opaque_ref, r.opaque_ref);
                done++;
                PercentComplete = (100 * done) / count;  
            }

            log.DebugFormat("Removing {0} roles on subject '{1}'", toRemove.Count, (subject.DisplayName ?? subject.SubjectName ?? subject.subject_identifier).Ellipsise(50));
            foreach (Role r in toRemove)
            {
                Subject.remove_from_roles(Session, subject.opaque_ref, r.opaque_ref);
                done++;
                PercentComplete = (100 * done) / count;
            }
            Description = Messages.COMPLETED;
        }
    }
}
