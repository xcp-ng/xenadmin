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
using System.Diagnostics;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Mappings;
using XenAdmin.Network;

using XenAPI;
using XenOvf;
using XenOvf.Definitions;
using XenOvfTransport;

namespace XenAdmin.Actions.OVFActions
{
	public class ImportImageAction : ApplianceAction
	{
		#region Private fields

		private readonly EnvelopeType m_ovfEnvelope;
		private readonly Dictionary<string, VmMapping> m_vmMappings;
		private readonly bool m_runfixups;
		private readonly SR m_selectedIsoSr;
		private readonly string m_directory;
        private Import m_transportAction;

		#endregion

		public ImportImageAction(IXenConnection connection, EnvelopeType ovfEnv, string directory, Dictionary<string, VmMapping> vmMappings, bool runfixups, SR selectedIsoSr,
									string networkUuid, bool isTvmIpStatic, string tvmIpAddress, string tvmSubnetMask, string tvmGateway)
			: base(connection, string.Format(Messages.IMPORT_DISK_IMAGE, ovfEnv.Name, Helpers.GetName(connection)),
                networkUuid, isTvmIpStatic, tvmIpAddress, tvmSubnetMask, tvmGateway)
		{
			m_ovfEnvelope = ovfEnv;
			m_directory = directory;
			m_vmMappings = vmMappings;
			m_runfixups = runfixups;
			m_selectedIsoSr = selectedIsoSr;
		}

        protected override XenOvfTransportBase TransportAction => m_transportAction;

		protected override void Run()
		{
		    base.Run();

			Debug.Assert(m_vmMappings.Count == 1, "There is one VM mapping");

			string systemid = m_vmMappings.Keys.ElementAt(0);
			var mapping = m_vmMappings.Values.ElementAt(0);

			var session = Connection.Session;
			var url = session.Url;
			Uri uri = new Uri(url);

			PercentComplete = 20;
			Description = Messages.IMPORTING_DISK_IMAGE;

			//create a copy of the ovf envelope
			EnvelopeType[] envs = OVF.Split(m_ovfEnvelope, "system", new object[] {new[] {systemid}});
			EnvelopeType curEnv = envs[0];

			//storage
			foreach (var kvp in mapping.Storage)
				OVF.SetTargetSRInRASD(curEnv, systemid, kvp.Key, kvp.Value.uuid);

			//network
			foreach (var kvp in mapping.Networks)
				OVF.SetTargetNetworkInRASD(curEnv, systemid, kvp.Key, kvp.Value.uuid);

			if (m_runfixups)
			{
				string cdId = OVF.SetRunOnceBootCDROMOSFixup(curEnv, systemid, m_directory);
				OVF.SetTargetISOSRInRASD(curEnv, systemid, cdId, m_selectedIsoSr.uuid);
			}

			try //importVM
			{
				m_transportAction = new Import(uri, session)
				                    	{
				                    		UpdateHandler = UpdateHandler,
											Cancel = Cancelling //in case the Cancel button has already been pressed
				                    	};
				m_transportAction.SetTvmNetwork(m_networkUuid, m_isTvmIpStatic, m_tvmIpAddress, m_tvmSubnetMask, m_tvmGateway);
                m_transportAction.Process(curEnv, m_directory, null);

				PercentComplete = 100;
				Description = Messages.COMPLETED;
			}
			catch (OperationCanceledException)
			{
				throw new CancelledException();
			}
		}
	}
}
