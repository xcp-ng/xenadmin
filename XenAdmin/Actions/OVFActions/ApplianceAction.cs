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

using System.Threading;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenOvfTransport;

namespace XenAdmin.Actions.OVFActions
{
	public abstract class ApplianceAction : AsyncAction
	{
		protected string m_networkUuid;
		protected bool m_isTvmIpStatic;
		protected string m_tvmIpAddress;
		protected string m_tvmSubnetMask;
		protected string m_tvmGateway;

        protected abstract XenOvfTransportBase TransportAction { get; }

        private const int SLEEP_TIME = 900;
        private const int MAX_ITERATIONS = 60 * 60 * 24 / SLEEP_TIME * 1000; //iterations in 24h

		/// <summary>
		/// RBAC dependencies needed to import appliance/export an appliance/import disk image.
		/// </summary>
		public static RbacMethodList StaticRBACDependencies = new RbacMethodList("VM.add_to_other_config",
																				 "VM.create",
																				 "VM.destroy",
																				 "VM.hard_shutdown",
																				 "VM.remove_from_other_config",
																				 "VM.set_HVM_boot_params",
																				 "VM.start",
																				 "VDI.create",
																				 "VDI.destroy",
																				 "VBD.create",
																				 "VBD.eject",
																				 "VIF.create",
																				 "Host.call_plugin");

		protected ApplianceAction(IXenConnection connection, string title,
			string networkUuid, bool isTvmIpStatic, string tvmIpAddress, string tvmSubnetMask, string tvmGateway)
			: base(connection, title)
		{
			m_networkUuid = networkUuid;
			m_isTvmIpStatic = isTvmIpStatic;
			m_tvmIpAddress = tvmIpAddress;
			m_tvmSubnetMask = tvmSubnetMask;
			m_tvmGateway = tvmGateway;

			Pool pool = Helpers.GetPool(connection);
			if (pool != null)
				Pool = pool;
			else
				Host = Helpers.GetMaster(connection);
		}

	    protected override void Run()
	    {
	        SafeToExit = false; 
            InitialiseTicker();
	    }

		protected void UpdateHandler(XenOvfTransportEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Message))
				Description = e.Message;
		}

		public override void RecomputeCanCancel()
		{
			CanCancel = true;
		}

		protected override void CancelRelatedTask()
		{
			Description = Messages.CANCELING;

            if (TransportAction != null)
                TransportAction.Cancel = true;
		}

	    private void InitialiseTicker()
	    {
	        System.Threading.Tasks.Task.Run(() => TickUntilCompletion());
	    }

	    private void TickUntilCompletion()
	    {
	        int i = 0;
	        while (!IsCompleted && ++i<MAX_ITERATIONS)
	        {
	            OnChanged();
                Thread.Sleep(SLEEP_TIME);
	        }
	    }


	}
}
