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
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Network;


namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public class UnwindProblemsAction : PlanAction
    {
        private readonly List<Problem> _problems;
        private readonly IXenConnection _connection;

        public UnwindProblemsAction(List<Problem> problems, IXenConnection connection = null)
        {
            _problems = problems;
            _connection = connection;
        }

        protected override void _Run()
        {
            var msg = _connection == null
                ? Messages.REVERTING_RESOLVED_PRECHECKS
                : string.Format(Messages.REVERTING_RESOLVED_PRECHECKS_POOL, _connection.Name);

            AddProgressStep(msg);

            for (int i = 0; i < _problems.Count; i++)
            {
                var action = _problems[i].CreateUnwindChangesAction();
                if (action != null && action.Connection != null && action.Connection.IsConnected)
                    action.RunExternal(null);
                PercentComplete = i * 100 / _problems.Count;
            }
        }
    }
}