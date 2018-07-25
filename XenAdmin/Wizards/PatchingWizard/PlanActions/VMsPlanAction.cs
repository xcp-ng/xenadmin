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
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public abstract class VMsPlanAction : PlanActionWithSession
    {
        private readonly List<XenRef<VM>> _vms;

        protected VMsPlanAction(List<XenRef<VM>> vms, IXenConnection connection)
            : base(connection)
        {
            this._vms = vms;
        }

        protected abstract XenRef<Task> DoPerVM(Session session, XenRef<VM> vmRef);

        protected override void RunWithSession(ref Session session)
        {
            PBD.CheckPlugPBDsForVMs(Connection, _vms);

            int vmCount = _vms.Count;

            for (int i = 0; i < _vms.Count; i++)
            {
                var vmRef = _vms[i];
                XenRef<Task> task = DoPerVM(session, vmRef);

                try
                {
                    var j = i;
                    PollTaskForResult(Connection, ref session, task,
                        progress => PercentComplete = (progress + 100 * j) / vmCount);
                }
                finally
                {
                    Task.destroy(session, task);
                }
            }
        }
    }
}