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
using System.IO;
using XenAdmin.Core;
using XenAdmin.Mappings;
using XenAdmin.Network;

using XenAPI;
using XenOvf;
using XenOvf.Definitions;
using XenOvfTransport;

namespace XenAdmin.Actions.OVFActions
{
	public class ImportApplianceAction : ApplianceAction
	{
		#region Private fields

		private readonly Package m_package;
		private readonly Dictionary<string, VmMapping> m_vmMappings;
		private readonly bool m_verifyManifest;
		private readonly bool m_verifySignature;
		private readonly string m_password;
		private readonly bool m_runfixups;
		private readonly SR m_selectedIsoSr;
        private Import m_transportAction;

		#endregion

		public ImportApplianceAction(IXenConnection connection, Package package, Dictionary<string, VmMapping> vmMappings,
										bool verifyManifest, bool verifySignature, string password, bool runfixups, SR selectedIsoSr,
										string networkUuid, bool isTvmIpStatic, string tvmIpAddress, string tvmSubnetMask, string tvmGateway)
            : base(connection, string.Format(Messages.IMPORT_APPLIANCE, package.Name, Helpers.GetName(connection)),
                networkUuid, isTvmIpStatic, tvmIpAddress, tvmSubnetMask, tvmGateway)
		{
			m_package = package;
			m_vmMappings = vmMappings;
			m_verifyManifest = verifyManifest;
			m_verifySignature = verifySignature;
			m_password = password;
			m_runfixups = runfixups;
			m_selectedIsoSr = selectedIsoSr;
		}

        protected override XenOvfTransportBase TransportAction => m_transportAction;

		protected override void Run()
		{
		    base.Run();

			if (m_verifySignature)
			{
				Description = Messages.VERIFYING_SIGNATURE;

				try
				{
					// The appliance is known to have a signature and the user asked to verify it.
					m_package.VerifySignature();

					// If the appliance has a signature, then it has a manifest.
					// Always verify the manifest after verifying the signature.
					m_package.VerifyManifest();
				}
				catch (Exception e)
				{
					throw new Exception(String.Format(Messages.VERIFYING_SIGNATURE_ERROR, e.Message));
				}
			}
			else if (m_verifyManifest)
			{
				Description = Messages.VERIFYING_MANIFEST;

				try
				{
					// The appliance had a manifest without a signature and the user asked to verify it.
					// VerifyManifest() throws an exception when verification fails for any reason.
					m_package.VerifyManifest();
				}
				catch (Exception e)
				{
					throw new Exception(String.Format(Messages.VERIFYING_MANIFEST_ERROR, e.Message));
				}
			}

			PercentComplete = 20;
			Description = Messages.IMPORTING_VMS;

			var session = Connection.Session;
			var url = session.Url;
			Uri uri = new Uri(url);

			//create a copy of the OVF
			var envelopes = new List<EnvelopeType>();

			foreach (var vmMapping in m_vmMappings)
			{
				if (Cancelling)
					throw new CancelledException();

				string systemid = vmMapping.Key;
				var mapping = vmMapping.Value;
                EnvelopeType[] envs = OVF.Split(m_package.OvfEnvelope, "system", new object[] {new[] {systemid}});

				//storage
				foreach (var kvp in mapping.Storage)
					OVF.SetTargetSRInRASD(envs[0], systemid, kvp.Key, kvp.Value.uuid);

			    foreach (var kvp in mapping.StorageToAttach)
                    OVF.SetTargetVDIInRASD(envs[0], systemid, kvp.Key, kvp.Value.uuid);

				//network
				foreach (var kvp in mapping.Networks)
					OVF.SetTargetNetworkInRASD(envs[0], systemid, kvp.Key, kvp.Value.uuid);

				if (m_runfixups)
				{
					string cdId = OVF.SetRunOnceBootCDROMOSFixup(envs[0], systemid, Path.GetDirectoryName(m_package.PackageSourceFile));
					OVF.SetTargetISOSRInRASD(envs[0], systemid, cdId, m_selectedIsoSr.uuid);
				}

				envelopes.Add(envs[0]);
			}

            EnvelopeType env = OVF.Merge(envelopes, m_package.Name);
            m_package.ExtractToWorkingDir();

			try //importVM
            {
                m_transportAction = new Import(uri, session)
                {
                    ApplianceName = m_package.Name,
                    UpdateHandler = UpdateHandler,
                    Cancel = Cancelling //in case the Cancel button has already been pressed
                };
				m_transportAction.SetTvmNetwork(m_networkUuid, m_isTvmIpStatic, m_tvmIpAddress, m_tvmSubnetMask, m_tvmGateway);
                m_transportAction.Process(env, m_package.WorkingDir, m_password);
			}
			catch (OperationCanceledException)
			{
				throw new CancelledException();
			}
            finally
            {
                m_package.CleanUpWorkingDir();
            }

			PercentComplete = 100;
			Description = Messages.COMPLETED;
		}
    }
}
