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
using XenAdmin.Diagnostics.Checks;
using XenAPI;
using XenAdmin.Actions;
using System.Collections.Generic;


namespace XenAdmin.Diagnostics.Problems.PoolProblem
{
    class HAEnabledProblem : PoolProblem
    {
        private readonly long FailuresToTolerate;
        private readonly List<SR> HeartbeatSrs = new List<SR>();

        public HAEnabledProblem(Check check, Pool pool)
            : base(check, pool)
        {
            FailuresToTolerate = pool.ha_host_failures_to_tolerate;
            HeartbeatSrs = pool.GetHAHeartbeatSRs();
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            return new DisableHAAction(Pool);
        }

        public override AsyncAction CreateUnwindChangesAction()
        {
            return new EnableHAAction(Pool, null, HeartbeatSrs, FailuresToTolerate);
        }

        public override string Description
        {
            get 
            {
                return String.Format(Messages.UPDATES_WIZARD_HA_ON_DESCRIPTION, Pool);
            }
        }

        public override string HelpMessage
        {
            get
            {
                return Messages.TURN_HA_OFF;
            }
        }
    }

    class HAEnabledWarning : Warning
    {
        private readonly Pool pool;
        private readonly Host host;

        public HAEnabledWarning(Check check, Pool pool, Host host)
            : base(check)
        {
            this.pool = pool;
            this.host = host;
        }

        public override string Title
        {
            get { return Check.Description; }
        }

        public override string Description
        {
            get
            {
                return string.Format(Messages.UPDATES_WIZARD_HA_ON_WARNING, host, pool);
            }
        }
    }
}
