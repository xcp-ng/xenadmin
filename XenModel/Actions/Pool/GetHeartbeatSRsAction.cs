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
using XenAPI;


namespace XenAdmin.Actions
{
    public class GetHeartbeatSRsAction : AsyncAction
    {
        private readonly List<SRWrapper> _srs = new List<SRWrapper>();

        public GetHeartbeatSRsAction(Pool pool)
            : base(pool.Connection, string.Format(Messages.HA_SCANNING_SRS,
            Helpers.GetName(pool.Connection).Ellipsise(50)), null, true)
        {

        }

        public List<SRWrapper> SRs
        {
            get
            {
                return _srs;
            }
        }

        public override void RecomputeCanCancel()
        {
            // Do nothing
        }

        protected override void Run()
        {
            this.CanCancel = true;

            List<SR> SRs = new List<SR>(Connection.Cache.SRs);
            if (SRs.Count == 0)
                return;
            double increment = 100.0 / SRs.Count;

            foreach (SR sr in SRs)
            {
                try
                {
                    if (!sr.shared || sr.IsToolsSR())
                    {
                        // SR is unsuitable for heartbeating
                        continue;
                    }
                    RelatedTask = XenAPI.SR.async_assert_can_host_ha_statefile(this.Session, sr.opaque_ref);
                    this.PollToCompletion(this.PercentComplete, (int)(this.PercentComplete + increment));
                    _srs.Add(new SRWrapper(true, null, sr));

                    if (Cancelling)
                        throw new CancelledException();
                }
                catch (Failure f)
                {
                    string reason;
                    if (f.ErrorDescription.Count > 0 && f.ErrorDescription[0] == Failure.SR_HAS_NO_PBDS)
                    {
                        reason = Messages.SR_DETACHED;
                    }
                    else if (f.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                    {
                        // we just want to the first line, not the gorey details
                        reason = f.Message.Split('\n')[0];
                    }
                    else
                    {
                        reason = f.Message;
                    }
                    _srs.Add(new SRWrapper(false, reason, sr));
                }
            }
        }
    }

    public class SRWrapper
    {
        public readonly bool enabled;
        public readonly SR sr;
        public readonly string ReasonUnsuitable;

        internal SRWrapper(bool enabled, string reasonUnsuitable, SR sr)
        {
            this.enabled = enabled;
            this.sr = sr;
            this.ReasonUnsuitable = reasonUnsuitable;
        }
    }
}
