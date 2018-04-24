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
    public class SetVMStartupOptionsAction : PureAsyncAction
    {
        private readonly Dictionary<VM, VMStartupOptions> settings = new Dictionary<VM, VMStartupOptions>();
        private readonly Pool pool;

        public SetVMStartupOptionsAction(IXenConnection connection, Dictionary<VM, VMStartupOptions> settings, bool suppressHistory)
            : base(connection, Messages.SETTING_VM_STARTUP_OPTIONS, suppressHistory)
        {
            System.Diagnostics.Trace.Assert(connection != null);
            System.Diagnostics.Trace.Assert(settings != null);

            this.settings = settings;
            this.pool = Helpers.GetPoolOfOne(this.Connection);
            if (this.pool == null)
                throw new Failure(Failure.INTERNAL_ERROR, Messages.POOL_GONE);
        }

        protected override void Run()
        {
            int i = 0;
            foreach (VM vm in settings.Keys)
            {
                Description = string.Format(Messages.SETTING_VM_STARTUP_OPTIONS_ON_X, Helpers.GetName(vm));

                VM.set_order(this.Session, vm.opaque_ref, settings[vm].Order);
                VM.set_start_delay(this.Session, vm.opaque_ref, settings[vm].StartDelay);

                PercentComplete = (int)(++i * (60.0 / settings.Count));
                if (Cancelling)
                    throw new CancelledException();
            }

            // Sync database to ensure new settings are saved to all hosts
            RelatedTask = XenAPI.Pool.async_sync_database(this.Session);
            PollToCompletion(60, 100);

            Description = Messages.COMPLETED;
        }
    }
}
