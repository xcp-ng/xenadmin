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
    public class WlbRetrieveVmRecommendationsAction: PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<VM> vms;
        private readonly Dictionary<VM, Dictionary<XenRef<Host>, string[]>> recommendations = new Dictionary<VM, Dictionary<XenRef<Host>, string[]>>();
        private bool isError;

        public WlbRetrieveVmRecommendationsAction(IXenConnection connection, List<VM> vms)
            : base(connection, Messages.WLB_RETRIEVING_VM_RECOMMENDATIONS, true)
        {
            this.vms = vms;
        }

        protected override void Run()
        {
            if (vms.Count == 0)
            {
                isError = true;
                return;
            }

            if (Helpers.WlbEnabled(vms[0].Connection))
            {
                try
                {
                    foreach (var vm in vms)
                    {
                        recommendations[vm] = VM.retrieve_wlb_recommendations(Session, vm.opaque_ref);
                    }
                }
                catch (Exception e)
                {
                    log.Error("Error getting WLB recommendations", e);
                    isError = true;
                }
            }
            else
            {
                isError = true;
            }
        }

        public Dictionary<VM, Dictionary<XenRef<Host>, string[]>> Recommendations
        {
            get { return isError ? null : recommendations; }
        }
    }
}
