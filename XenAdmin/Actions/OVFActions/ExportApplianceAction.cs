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
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

using XenOvf;
using XenOvf.Definitions;
using XenOvfTransport;

namespace XenAdmin.Actions.OVFActions
{
	public class ExportApplianceAction : ApplianceAction
	{
		#region Private fields

		private readonly string m_applianceDirectory;
		private readonly string m_applianceFileName;
		private readonly List<VM> m_vmsToExport;
		private readonly IEnumerable<string> m_eulas;
		private readonly bool m_signAppliance;
		private readonly bool m_createManifest;
		private readonly X509Certificate2 m_certificate;
		private readonly bool m_encryptFiles;
		private readonly string m_encryptPassword;
		private readonly bool m_createOVA;
		private readonly bool m_compressOVFfiles;
        private readonly bool m_shouldVerify;
		private OvfCompressor m_compressor;
        private Export m_transportAction;

		#endregion

		public ExportApplianceAction(IXenConnection connection, string applianceDirectory, string applianceFileName, List<VM> vmsToExport,
										IEnumerable<string> eulas, bool signAppliance, bool createManifest, X509Certificate2 certificate,
										bool encryptFiles, string encryptPassword, bool createOVA, bool compressOVFfiles,
										string networkUuid, bool isTvmIpStatic, string tvmIpAddress, string tvmSubnetMask, string tvmGateway, bool shouldVerify)
			: base(connection,
                string.Format(createOVA ? Messages.EXPORT_OVA_PACKAGE : Messages.EXPORT_OVF_PACKAGE, applianceFileName, Helpers.GetName(connection)),
                networkUuid, isTvmIpStatic, tvmIpAddress, tvmSubnetMask, tvmGateway)
		{
			m_applianceDirectory = applianceDirectory;
			m_applianceFileName = applianceFileName;
			m_vmsToExport = vmsToExport;
			m_eulas = eulas;
			m_signAppliance = signAppliance;
			m_createManifest = createManifest;
			m_certificate = certificate;
			m_encryptFiles = encryptFiles;
			m_encryptPassword = encryptPassword;
			m_createOVA = createOVA;
			m_compressOVFfiles = compressOVFfiles;
            m_shouldVerify = shouldVerify;

			if (m_vmsToExport.Count == 1)
				VM = m_vmsToExport.First();
		}

        protected override XenOvfTransportBase TransportAction => m_transportAction;

		protected override void Run()
		{
            base.Run();
		    
			var session = Connection.Session;

			var url = session.Url;
			Uri uri = new Uri(url);

			var appFolder = Path.Combine(m_applianceDirectory, m_applianceFileName);
			var appFile = string.Format("{0}.ovf", m_applianceFileName);

			if (!Directory.Exists(appFolder))
				Directory.CreateDirectory(appFolder);
			PercentComplete = 5;

			Description = Messages.EXPORTING_VMS;
			EnvelopeType env;
			try
			{
                m_transportAction = new Export(uri, session)
				                    	{
				                    		UpdateHandler = UpdateHandler,
				                    		ShouldVerifyDisks = m_shouldVerify,
				                    		Cancel = Cancelling //in case the Cancel button has already been pressed
				                    	};
				m_transportAction.SetTvmNetwork(m_networkUuid, m_isTvmIpStatic, m_tvmIpAddress, m_tvmSubnetMask, m_tvmGateway);
                env = m_transportAction.Process(appFolder, m_applianceFileName, (from VM vm in m_vmsToExport select vm.uuid).ToArray());
				PercentComplete = 60;
			}
			catch (OperationCanceledException)
			{
				throw new CancelledException();
			}

			foreach (var eula in m_eulas)
			{
				if (Cancelling)
					throw new CancelledException();
				Description = Messages.ADDING_EULAS;
				OVF.AddEula(env, eula);
			}

			if (Cancelling)
				throw new CancelledException();

			var ovfPath = Path.Combine(appFolder, appFile);
			Description = String.Format(Messages.CREATING_FILE, appFile);
			OVF.SaveAs(env, ovfPath);
			PercentComplete = 70;
			
			if (Cancelling)
					throw new CancelledException();

			if (m_signAppliance)
			{	
				Description = Messages.SIGNING_APPLIANCE;
				OVF.Sign(m_certificate, appFolder, appFile);
			}
			else if (m_createManifest)
			{
				Description = Messages.CREATING_MANIFEST;
				OVF.Manifest(appFolder, appFile);
			}

			PercentComplete = 90;
			
			if (Cancelling)
					throw new CancelledException();

			if (m_createOVA)
			{
				Description = String.Format(Messages.CREATING_FILE, String.Format("{0}.ova", m_applianceFileName));
				OVF.ConvertOVFtoOVA(appFolder, appFile, m_compressOVFfiles);
			}
			else if (m_compressOVFfiles)
			{
				Description = Messages.COMPRESSING_FILES;
				m_compressor = new OvfCompressor { CancelCompression = Cancelling }; //in case the Cancel button has already been pressed}

				try
				{
					m_compressor.CompressOvfFiles(ovfPath, "GZip");
				}
				catch (OperationCanceledException)
				{
					throw new CancelledException();
				}
			}

			PercentComplete = 100;
			Description = Messages.COMPLETED;
		}

		protected override void CancelRelatedTask()
		{
			base.CancelRelatedTask();

			if (m_compressor != null)
				m_compressor.CancelCompression = true;
		}
	}
}
