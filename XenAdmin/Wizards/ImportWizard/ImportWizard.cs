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
using System.Text.RegularExpressions;
using XenAdmin.Actions;
using XenAdmin.Actions.OVFActions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Mappings;
using XenAdmin.Network;
using XenAdmin.Wizards.GenericPages;
using XenAPI;
using System.Linq;

using XenOvf;
using XenOvf.Definitions;
using XenOvf.Utilities;
using Tuple = System.Collections.Generic.KeyValuePair<string, string>;

namespace XenAdmin.Wizards.ImportWizard
{
	internal partial class ImportWizard : XenWizardBase
	{
		#region Private fields
        private readonly ImportSelectStoragePage m_pageStorage;
        private readonly ImportSelectNetworkPage m_pageNetwork;
        private readonly ImportSelectHostPage m_pageHost;
        private readonly ImportSecurityPage m_pageSecurity;
        private readonly ImportEulaPage m_pageEula;
        private readonly ImportOptionsPage m_pageOptions;
        private readonly ImportFinishPage m_pageFinish;
        private readonly RBACWarningPage m_pageRbac;
        private readonly TvmIpPage m_pageTvmIp;
        private readonly ImageVMConfigPage m_pageVMconfig;
        private ImportSourcePage m_pageImportSource;
        private readonly StoragePickerPage m_pageXvaStorage;
        private readonly NetworkPickerPage m_pageXvaNetwork;
        private readonly GlobalSelectHost m_pageXvaHost;
        private readonly LunPerVdiImportPage lunPerVdiMappingPage;

        private IXenObject m_selectedObject;
		private Dictionary<string, VmMapping> m_vmMappings = new Dictionary<string, VmMapping>();
		/// <summary>
		/// Make this nullable and initialize with null so the pages are added correctly to the wizard progress the first time
		/// </summary>
		private ImportType? m_typeOfImport;
		private bool m_ignoreAffinitySet;
		private EnvelopeType m_envelopeFromVhd;
		#endregion

		public ImportWizard(IXenConnection con, IXenObject xenObject, string filename, bool ovfModeOnly)
			: base(con)
		{
			InitializeComponent();

		    m_pageStorage = new ImportSelectStoragePage();
		    m_pageNetwork = new ImportSelectNetworkPage();
		    m_pageHost = new ImportSelectHostPage();
		    m_pageSecurity = new ImportSecurityPage();
		    m_pageEula = new ImportEulaPage();
		    m_pageOptions = new ImportOptionsPage();
		    m_pageFinish = new ImportFinishPage();
		    m_pageRbac = new RBACWarningPage();
		    m_pageTvmIp = new TvmIpPage();
		    m_pageVMconfig = new ImageVMConfigPage();
		    m_pageImportSource = new ImportSourcePage();
		    m_pageXvaStorage = new StoragePickerPage();
		    m_pageXvaNetwork = new NetworkPickerPage();
		    m_pageXvaHost = new GlobalSelectHost();
            lunPerVdiMappingPage = new LunPerVdiImportPage { Connection = con };

			m_selectedObject = xenObject;
            m_pageTvmIp.IsExportMode = false;
			m_pageFinish.SummaryRetreiver = GetSummary;
			m_pageXvaStorage.ImportVmCompleted += m_pageXvaStorage_ImportVmCompleted;

			if (!string.IsNullOrEmpty(filename))
				m_pageImportSource.SetFileName(filename);

			m_pageImportSource.OvfModeOnly = ovfModeOnly;
            AddPages(m_pageImportSource, m_pageHost, m_pageStorage, m_pageNetwork, m_pageFinish);

            m_pageHost.ConnectionSelectionChanged += pageHost_ConnectionSelectionChanged;
            m_pageXvaHost.ConnectionSelectionChanged += pageHost_ConnectionSelectionChanged;
            ShowXenAppXenDesktopWarning(con);
		}

		#region Override (XenWizardBase) Methods

        protected override void FinishWizard()
		{
			switch (m_typeOfImport)
			{
				case ImportType.Xva:
					if (m_pageXvaStorage.ImportXvaAction != null)
						m_pageXvaStorage.ImportXvaAction.EndWizard(m_pageFinish.StartVmsAutomatically, m_pageXvaNetwork.VIFs);
					break;
				case ImportType.Ovf:
					(new ImportApplianceAction(m_pageHost.Connection,
											   m_pageImportSource.SelectedOvfEnvelope,
											   m_pageImportSource.SelectedOvfPackage,
					                           m_vmMappings,
					                           m_pageSecurity.VerifyManifest,
					                           m_pageSecurity.VerifySignature,
					                           m_pageSecurity.Password,
					                           m_pageOptions.RunFixups,
					                           m_pageOptions.SelectedIsoSR,
					                           m_pageTvmIp.NetworkUuid.Key,
					                           m_pageTvmIp.IsTvmIpStatic,
					                           m_pageTvmIp.TvmIpAddress,
					                           m_pageTvmIp.TvmSubnetMask,
					                           m_pageTvmIp.TvmGateway)).RunAsync();
					break;
				case ImportType.Vhd:
					(new ImportImageAction(m_pageHost.Connection,
										   m_envelopeFromVhd,
										   Path.GetDirectoryName(m_pageImportSource.FilePath),
					                       m_vmMappings,
					                       m_pageOptions.RunFixups,
					                       m_pageOptions.SelectedIsoSR,
					                       m_pageTvmIp.NetworkUuid.Key,
					                       m_pageTvmIp.IsTvmIpStatic,
					                       m_pageTvmIp.TvmIpAddress,
					                       m_pageTvmIp.TvmSubnetMask,
					                       m_pageTvmIp.TvmGateway)).RunAsync();
					break;
			}

			base.FinishWizard();
		}

		protected override void OnCancel()
		{
			base.OnCancel();

			if (m_pageXvaStorage.ImportXvaAction != null)
			{
				m_pageXvaStorage.ImportXvaAction.EndWizard(false, null);
				m_pageXvaStorage.ImportXvaAction.Cancel();
			}
		}

        protected override void UpdateWizardContent(XenTabPage page)
		{
			Type type = page.GetType();

			if (type == typeof(ImportSourcePage))
			{
				#region ImportSourcePage

				var oldTypeOfImport = m_typeOfImport;//store previous type
				m_typeOfImport = m_pageImportSource.TypeOfImport;
				var appliancePages = new XenTabPage[] {m_pageEula, m_pageHost, m_pageStorage, m_pageNetwork, m_pageSecurity, m_pageOptions, m_pageTvmIp};
                var imagePages = new XenTabPage[] { m_pageVMconfig, m_pageHost, m_pageStorage, m_pageNetwork, m_pageOptions, m_pageTvmIp };
                var xvaPages = new XenTabPage[] { m_pageXvaHost, m_pageXvaStorage, m_pageXvaNetwork };

				switch (m_typeOfImport)
				{
					case ImportType.Ovf:
						if (oldTypeOfImport != ImportType.Ovf)
						{
							Text = Messages.WIZARD_TEXT_IMPORT_OVF;
							pictureBoxWizard.Image = Properties.Resources._000_ImportVirtualAppliance_h32bit_32;
							m_pageFinish.ShowStartVmsGroupBox = false;
                            RemovePages(imagePages);
                            RemovePages(xvaPages);
                            AddAfterPage(m_pageImportSource, appliancePages);
						}

						m_pageEula.SelectedOvfEnvelope = m_pageImportSource.SelectedOvfEnvelope;
						m_pageSecurity.SelectedOvfPackage = m_pageImportSource.SelectedOvfPackage;

						CheckDisabledPages(m_pageEula, m_pageSecurity); //decide whether to disable these progress steps
						ResetVmMappings(m_pageImportSource.SelectedOvfEnvelope);
						m_pageHost.SelectedOvfEnvelope = m_pageImportSource.SelectedOvfEnvelope;
				        m_pageHost.SetDefaultTarget(m_pageHost.ChosenItem ?? m_selectedObject);
						m_pageHost.VmMappings = m_vmMappings;
						m_pageStorage.SelectedOvfEnvelope = m_pageImportSource.SelectedOvfEnvelope;
                        lunPerVdiMappingPage.SelectedOvfEnvelope = m_pageImportSource.SelectedOvfEnvelope;
						m_pageNetwork.SelectedOvfEnvelope = m_pageImportSource.SelectedOvfEnvelope;
                        
						NotifyNextPagesOfChange(m_pageEula, m_pageHost, m_pageStorage, m_pageNetwork, m_pageSecurity, m_pageOptions);
						break;
					case ImportType.Vhd:
						if (oldTypeOfImport != ImportType.Vhd)
						{
							Text = Messages.WIZARD_TEXT_IMPORT_VHD;
							pictureBoxWizard.Image = Properties.Resources._000_ImportVM_h32bit_32;
							m_pageFinish.ShowStartVmsGroupBox = false;
                            RemovePages(appliancePages);
                            RemovePages(xvaPages);
                            AddAfterPage(m_pageImportSource, imagePages);
						}
						m_pageVMconfig.IsWim = m_pageImportSource.IsWIM;
                        m_pageHost.SetDefaultTarget(m_pageHost.ChosenItem ?? m_selectedObject);
 						m_pageHost.SelectedOvfEnvelope = null;
						m_pageHost.VmMappings = m_vmMappings;
						NotifyNextPagesOfChange(m_pageVMconfig, m_pageHost, m_pageStorage, m_pageNetwork, m_pageOptions);
						break;
					case ImportType.Xva:
						if (oldTypeOfImport != ImportType.Xva)
						{
							Text = Messages.WIZARD_TEXT_IMPORT_XVA;
							pictureBoxWizard.Image = Properties.Resources._000_ImportVM_h32bit_32;
							m_pageFinish.ShowStartVmsGroupBox = true;
                            RemovePages(imagePages);
                            RemovePages(appliancePages);
                            AddAfterPage(m_pageImportSource, xvaPages);
						}
				        m_pageXvaHost.SetDefaultTarget(m_selectedObject);
						m_pageXvaStorage.FilePath = m_pageImportSource.FilePath;
						break;
				}

			    #endregion
			}
			else if (type == typeof(ImageVMConfigPage))
			{
				//then use it to create an ovf for the import
				m_envelopeFromVhd = InitialiseOvfEnvelope();
				m_pageStorage.SelectedOvfEnvelope = m_envelopeFromVhd;
			    lunPerVdiMappingPage.SelectedOvfEnvelope = m_envelopeFromVhd;
				m_pageNetwork.SelectedOvfEnvelope = m_envelopeFromVhd;
				ResetVmMappings(m_envelopeFromVhd);
				NotifyNextPagesOfChange(m_pageHost, m_pageStorage, m_pageNetwork);
			}
			else if (type == typeof(ImportSelectHostPage))
			{
                RemovePage(m_pageRbac);
				ConfigureRbacPage(m_pageHost.Connection);
				m_vmMappings = m_pageHost.VmMappings;
				m_pageStorage.VmMappings = m_vmMappings;
				m_pageStorage.Connection = m_pageHost.Connection;
				m_pageNetwork.Connection = m_pageHost.Connection;
				m_pageOptions.Connection = m_pageHost.Connection;
				m_pageTvmIp.Connection = m_pageHost.Connection;
				NotifyNextPagesOfChange(m_pageStorage, m_pageNetwork, m_pageOptions, m_pageTvmIp);
			}
			else if (type == typeof(ImportSelectStoragePage))
			{
			    RemovePage(lunPerVdiMappingPage);
			    lunPerVdiMappingPage.ClearPickerData();
				m_vmMappings = m_pageStorage.VmMappings;
				m_pageNetwork.VmMappings = m_vmMappings;
                lunPerVdiMappingPage.VmMappings = m_vmMappings;
                if (lunPerVdiMappingPage.IsAnyPickerDataMappable 
                    && lunPerVdiMappingPage.MapLunsToVdisRequired
                    && m_typeOfImport == ImportType.Ovf)
                    AddAfterPage(m_pageStorage, lunPerVdiMappingPage);
			}
            else if (type == typeof(LunPerVdiImportPage))
            {
                m_vmMappings = lunPerVdiMappingPage.VmMappings;
                m_pageNetwork.VmMappings = m_vmMappings;
            }
			else if (type == typeof(ImportSelectNetworkPage))
			{
				m_vmMappings = m_pageNetwork.VmMappings;
				m_pageOptions.VmMappings = m_vmMappings;
			}
			else if (type == typeof(GlobalSelectHost))
			{
				var con = m_pageXvaHost.SelectedHost == null ? m_pageXvaHost.SelectedConnection : m_pageXvaHost.SelectedHost.Connection;
                RemovePage(m_pageRbac);
				ConfigureRbacPage(con);

				m_pageXvaStorage.SetConnection(con);
				m_pageXvaStorage.SetTargetHost(m_ignoreAffinitySet ? null : m_pageXvaHost.SelectedHost);

				m_pageXvaNetwork.SetConnection(con);
				m_pageXvaNetwork.SetAffinity(m_pageXvaHost.SelectedHost);
				
				NotifyNextPagesOfChange(m_pageXvaStorage, m_pageXvaNetwork);
			}
			else if (type == typeof(StoragePickerPage))
			{
				m_pageFinish.ShowStartVmsGroupBox = m_pageXvaStorage.ImportedVm != null && !m_pageXvaStorage.ImportedVm.is_a_template;
				m_pageXvaNetwork.SetVm(m_pageXvaStorage.ImportedVm);
				NotifyNextPagesOfChange(m_pageXvaNetwork);
			}

			if (type != typeof(ImportFinishPage))
				NotifyNextPagesOfChange(m_pageFinish);
		}

        protected override string WizardPaneHelpID()
        {
            var curPageType = CurrentStepTabPage.GetType();

            if (curPageType == typeof(RBACWarningPage))
            {
                switch (m_typeOfImport)
                {
                    case ImportType.Ovf:
                        return FormatHelpId("RbacImportOvf");
                    case ImportType.Vhd:
                        return FormatHelpId("RbacImportImage");
                    case ImportType.Xva:
                        return FormatHelpId("RbacImportXva");
                }
            }
            if (curPageType == typeof(ImportSelectHostPage))
            {
                switch (m_typeOfImport)
                {
                    case ImportType.Ovf:
                        return FormatHelpId("SelectHostOvf");
                    case ImportType.Vhd:
                        return FormatHelpId("SelectHostImage");
                }
            }
            if (curPageType == typeof(ImportSelectStoragePage))
            {
                switch (m_typeOfImport)
                {
                    case ImportType.Ovf:
                        return FormatHelpId("SelectStorageOvf");
                    case ImportType.Vhd:
                        return FormatHelpId("SelectStorageImage");
                }
            }
            if (curPageType == typeof(ImportSelectNetworkPage))
            {
                switch (m_typeOfImport)
                {
                    case ImportType.Ovf:
                        return FormatHelpId("SelectNetworkOvf");
                    case ImportType.Vhd:
                        return FormatHelpId("SelectNetworkImage");
                }
            }
            if (curPageType == typeof(ImportOptionsPage))
            {
                switch (m_typeOfImport)
                {
                    case ImportType.Ovf:
                        return FormatHelpId("ImportOptionsOvf");
                    case ImportType.Vhd:
                        return FormatHelpId("ImportOptionsImage");
                }
            }
            if (curPageType == typeof(TvmIpPage))
            {
                switch (m_typeOfImport)
                {
                    case ImportType.Ovf:
                        return FormatHelpId("TvmIpImportOvf");
                    case ImportType.Vhd:
                        return FormatHelpId("TvmIpImportImage");
                }
            }
            if (curPageType == typeof(ImportFinishPage))
            {
                switch (m_typeOfImport)
                {
                    case ImportType.Ovf:
                        return FormatHelpId("ImportFinishOvf");
                    case ImportType.Vhd:
                        return FormatHelpId("ImportFinishImage");
                    case ImportType.Xva:
                        return FormatHelpId("ImportFinishXva");
                }
            }
            return base.WizardPaneHelpID();
        }

	    #endregion

		#region Private methods

        private void ConfigureRbacPage(IXenConnection selectedConnection)
		{
			if (selectedConnection == null || selectedConnection.Session.IsLocalSuperuser || Helpers.GetMaster(selectedConnection).external_auth_type == Auth.AUTH_TYPE_NONE)
				return;

			m_pageRbac.ClearPermissionChecks();
			m_ignoreAffinitySet = false;

			switch (m_typeOfImport)
			{
				case ImportType.Ovf:
				case ImportType.Vhd:
			        {
						var check = m_typeOfImport == ImportType.Ovf
						            	? new RBACWarningPage.WizardPermissionCheck(Messages.RBAC_WARNING_IMPORT_WIZARD_APPLIANCE) {Blocking = true}
						            	: new RBACWarningPage.WizardPermissionCheck(Messages.RBAC_WARNING_IMPORT_WIZARD_IMAGE) {Blocking = true};
						check.AddApiCheckRange(ApplianceAction.StaticRBACDependencies);
						m_pageRbac.AddPermissionChecks(selectedConnection, check);

                        AddAfterPage(m_pageHost, m_pageRbac);
					}
					break;
				case ImportType.Xva:
					{
						//Check to see if they can import VMs at all
						var importCheck = new RBACWarningPage.WizardPermissionCheck(Messages.RBAC_WARNING_IMPORT_WIZARD_XVA) {Blocking = true};
						importCheck.ApiCallsToCheck.AddRange(ImportVmAction.ConstantRBACRequirements);

						//Check to see if they can set the VM's affinity
						var affinityCheck = new RBACWarningPage.WizardPermissionCheck(Messages.RBAC_WARNING_IMPORT_WIZARD_AFFINITY);
						affinityCheck.ApiCallsToCheck.Add("vm.set_affinity");
						affinityCheck.WarningAction = new RBACWarningPage.PermissionCheckActionDelegate(delegate
						                                                                                	{
						                                                                                		//We cannot allow them to set the affinity, so we are only going
						                                                                                		//to offer them the choice of connection, not specific host
						                                                                                		m_ignoreAffinitySet = true;
						                                                                                	});
                        m_pageRbac.AddPermissionChecks(selectedConnection, importCheck, affinityCheck);
                        AddAfterPage(m_pageXvaHost, m_pageRbac);
					}
					break;
			}

			//set page Connection after the page has been added to the wizard
			//(because the Connection is reset when the page is added
			m_pageRbac.Connection = selectedConnection;
		}

        private void CheckDisabledPages(params XenTabPage[] pages)
		{
			foreach (var p in pages)
				p.CheckPageDisabled();
		}

		protected override IEnumerable<Tuple> GetSummary()
		{
			switch (m_typeOfImport)
			{
				case ImportType.Xva:
					return GetSummaryXva();
				case ImportType.Ovf:
					return GetSummaryOvf();
				case ImportType.Vhd:
					return GetSummaryVhd();
				default:
					return new List<Tuple>();
			}
		}

		private IEnumerable<Tuple> GetSummaryXva()
		{
			var temp = new List<Tuple>();
			temp.Add(new Tuple(Messages.FINISH_PAGE_VMNAME, m_pageXvaStorage.ImportedVm.Name()));
			temp.Add(new Tuple(Messages.FINISH_PAGE_TARGET, m_pageXvaHost.SelectedHost == null ? m_pageXvaHost.SelectedConnection.Name : m_pageXvaHost.SelectedHost.Name()));
			temp.Add(new Tuple(Messages.FINISH_PAGE_STORAGE, m_pageXvaStorage.SR.Name()));

			var con = m_pageXvaHost.SelectedHost == null ? m_pageXvaHost.SelectedConnection : m_pageXvaHost.SelectedHost.Connection;

			bool first = true;
			foreach (var vif in m_pageXvaNetwork.VIFs)
			{
				var netref = new XenRef<XenAPI.Network>(vif.network);
				var network = con.Resolve(netref);
				// CA-218956 - Expose HIMN when showing hidden objects
				if (network == null || (network.IsGuestInstallerNetwork() && !XenAdmin.Properties.Settings.Default.ShowHiddenVMs))
					continue;

				temp.Add(new Tuple(first ? Messages.FINISH_PAGE_NETWORK : "", network.Name()));
				first = false;
			}

			return temp;
		}

		private IEnumerable<Tuple> GetSummaryOvf()
		{
			var temp = new List<Tuple>();
			
			var appName = m_pageImportSource.SelectedOvfEnvelope.Name;
			if (string.IsNullOrEmpty(appName))
				appName = Path.GetFileNameWithoutExtension(m_pageImportSource.SelectedOvfPackage.PackageSourceFile);

			temp.Add(new Tuple(Messages.FINISH_PAGE_REVIEW_APPLIANCE, appName));
			temp.Add(new Tuple(Messages.FINISH_PAGE_VERIFY_MANIFEST, m_pageSecurity.VerifyManifest.ToYesNoStringI18n()));
			temp.Add(new Tuple(Messages.FINISH_PAGE_VERIFY_SIGNATURE, m_pageSecurity.VerifySignature.ToYesNoStringI18n()));

			temp.Add(new Tuple(Messages.FINISH_PAGE_RUN_FIXUPS, m_pageOptions.RunFixups.ToYesNoStringI18n()));
			if (m_pageOptions.RunFixups)
				temp.Add(new Tuple(Messages.FINISH_PAGE_ISOSR, m_pageOptions.SelectedIsoSR.Name()));

			temp.AddRange(GetVmMappingsSummary());
			temp.AddRange(GetTransferVmSummary());
			return temp;
		}

		private IEnumerable<Tuple> GetSummaryVhd()
		{
			var temp = new List<Tuple>();
			temp.Add(new Tuple(Messages.FINISH_PAGE_IMAGEPATH, m_pageImportSource.FilePath));
			temp.Add(new Tuple(Messages.FINISH_PAGE_VMNAME, m_pageVMconfig.VmName));
			temp.Add(new Tuple(Messages.FINISH_PAGE_CPUCOUNT, m_pageVMconfig.CpuCount.ToString()));
			temp.Add(new Tuple(Messages.FINISH_PAGE_MEMORY, string.Format(Messages.VAL_MB, m_pageVMconfig.Memory)));

			if (m_pageImportSource.IsWIM)
				temp.Add(new Tuple(Messages.FINISH_PAGE_ADDSPACE, Util.DiskSizeString(m_pageVMconfig.AdditionalSpace)));

			temp.Add(new Tuple(Messages.FINISH_PAGE_RUN_FIXUPS, m_pageOptions.RunFixups.ToYesNoStringI18n()));
			if (m_pageOptions.RunFixups)
				temp.Add(new Tuple(Messages.FINISH_PAGE_ISOSR, m_pageOptions.SelectedIsoSR.Name()));

			temp.AddRange(GetVmMappingsSummary());
			temp.AddRange(GetTransferVmSummary());
			return temp;
		}

		private IEnumerable<Tuple> GetVmMappingsSummary()
		{
			var temp = new List<Tuple>();

			foreach (var mapping in m_vmMappings.Values)
			{
				var targetLbl = m_vmMappings.Count == 1 ? Messages.FINISH_PAGE_TARGET : string.Format(Messages.FINISH_PAGE_TARGET_FOR_VM, mapping.VmNameLabel);
				var storageLbl = m_vmMappings.Count == 1 ? Messages.FINISH_PAGE_STORAGE : string.Format(Messages.FINISH_PAGE_STORAGE_FOR_VM, mapping.VmNameLabel);
				var networkLbl = m_vmMappings.Count == 1 ? Messages.FINISH_PAGE_NETWORK : string.Format(Messages.FINISH_PAGE_NETWORK_FOR_VM, mapping.VmNameLabel);

				temp.Add(new Tuple(targetLbl, mapping.TargetName));
				bool first = true;
				foreach (var sr in mapping.Storage)
				{
					temp.Add(new Tuple(first ? storageLbl : "", sr.Value.Name()));
					first = false;
				}

				first = true;
				foreach (var net in mapping.Networks)
				{
					temp.Add(new Tuple(first ? networkLbl : "", net.Value.Name()));
					first = false;
				}
			}
			return temp;
		}

		private IEnumerable<Tuple> GetTransferVmSummary()
		{
			var temp = new List<Tuple>();
			temp.Add(new Tuple(Messages.FINISH_PAGE_TVM_NETWORK, m_pageTvmIp.NetworkUuid.Value));

			if (m_pageTvmIp.IsTvmIpStatic)
			{
				temp.Add(new Tuple(Messages.FINISH_PAGE_TVM_IP, m_pageTvmIp.TvmIpAddress));
				temp.Add(new Tuple(Messages.FINISH_PAGE_TVM_MASK, m_pageTvmIp.TvmSubnetMask));
				temp.Add(new Tuple(Messages.FINISH_PAGE_TVM_GATEWAY, m_pageTvmIp.TvmGateway));
			}
			else
			{
				temp.Add(new Tuple(Messages.FINISH_PAGE_TVM_IP, Messages.FINISH_PAGE_TVM_DHCP));
			}
			return temp;
		}

		private void ResetVmMappings(EnvelopeType ovfEnvelope)
		{
			string[] sysIds = OVF.FindSystemIds(ovfEnvelope);
			m_vmMappings.Clear();

			foreach (string sysId in sysIds)
			{
				var vmMap = new VmMapping
				            	{
				            		VmNameLabel = (m_typeOfImport == ImportType.Ovf)
													? FindVMName(ovfEnvelope, sysId)
				            		              	: m_pageVMconfig.VmName //it should only iterate once
				            	};
				m_vmMappings.Add(sysId, vmMap);
			}
		}

		private bool IsGUID(string expression)
		{
			if (expression != null)
			{
				Regex guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");

				return guidRegEx.IsMatch(expression);
			}
			return false;
		}

        // Find a name to use of a VM within an envelope that could have come from any hypervisor.
        // TODO: Consider refactoring this method because it is very similar to OVF.FindSystemName().
		private string FindVMName(EnvelopeType ovfEnv, string systemid)
		{
			VirtualSystem_Type vSystem = OVF.FindVirtualSystemById(ovfEnv, systemid);

            // Use the given name if present and valid.
            // The given name is Envelope.VirtualSystem.Name specified in the OVF Specification 1.1 clause 7.2.
            // XenServer sets the name property.
            // vSphere 4.1 and Virtual Box 4.0.6 do not.
            if ((Tools.ValidateProperty("Name", vSystem)) && !String.IsNullOrEmpty(vSystem.Name[0].Value))
                    return vSystem.Name[0].Value;

            // The VM wasn't given a name.
            // Build a list of choices from various properties.
            var choices = new List<string>();

            // VirtualSystem.id is next preference because vSphere and Virtual Box typically set this property to the VM name.
            if (!string.IsNullOrEmpty(vSystem.id))
                choices.Add(vSystem.id);

            // VirtualHardwareSection_Type.VirtualSystemIdentifier is next preference because Virtual Box will also set this property to the VM name.
			VirtualHardwareSection_Type[] vhsList = OVF.FindVirtualHardwareSection(ovfEnv, systemid);

            foreach (VirtualHardwareSection_Type vhs in vhsList)
            {
                if (vhs == null || vhs.System == null)
                    continue;

                if (Tools.ValidateProperty("VirtualSystemIdentifier", vhs.System))
                    choices.Add(vhs.System.VirtualSystemIdentifier.Value);
            }

            // Operating system description is next preference.
			OperatingSystemSection_Type[] ossList = OVF.FindSections<OperatingSystemSection_Type>(vSystem.Items);

            foreach (OperatingSystemSection_Type oss in ossList)
            {
				if (Tools.ValidateProperty("Description", oss))
                    choices.Add(oss.Description.Value);
            }

            // Envelope name is the last preference for XenServer that can could be a path in some cases.
            // vSphere and Virtual Box usually don't set this property.
            choices.Add(Path.GetFileNameWithoutExtension(ovfEnv.Name));

            // Last preference is file name.
			choices.Add(Path.GetFileNameWithoutExtension(m_pageImportSource.SelectedOvfPackage.PackageSourceFile));

            // First choice is one that is not a GUID.
            foreach (var choice in choices)
            {
                if (!String.IsNullOrEmpty(choice) && !IsGUID(choice))
                    return choice;
            }

            // Second choice is the first GUID.
            foreach (var choice in choices)
            {
                if (!String.IsNullOrEmpty(choice))
                    return choice;
            }

            // Last resort is a new GUID.
            return Guid.NewGuid().ToString();
		}

		private EnvelopeType InitialiseOvfEnvelope()
		{
			EnvelopeType env = OVF.CreateEnvelope(m_pageVMconfig.VmName);

			string systemID = OVF.AddVirtualSystem(env, m_pageVMconfig.VmName);
			string hdwareSectionId = OVF.AddVirtualHardwareSection(env, systemID);
			string guid = Guid.NewGuid().ToString();
			OVF.AddVirtualSystemSettingData(env, systemID, hdwareSectionId, env.Name, Messages.VIRTUAL_MACHINE,
											Messages.OVF_CREATED, guid, "hvm-3.0-unknown");

			OVF.SetCPUs(env, systemID, m_pageVMconfig.CpuCount);
			OVF.SetMemory(env, systemID, m_pageVMconfig.Memory, "MB");

			string netId = Guid.NewGuid().ToString();
			OVF.AddNetwork(env, systemID, netId, string.Format(Messages.NETWORK_NAME, 0), Messages.OVF_NET_DESCRIPTION, null);

			string diskId = Guid.NewGuid().ToString();
			ulong capacity = m_pageImportSource.DiskCapacity;
			if (m_pageImportSource.IsWIM)
				capacity += m_pageVMconfig.AdditionalSpace;
			OVF.AddDisk(env, systemID, diskId, m_pageImportSource.FilePath, true, Messages.OVF_DISK_CAPTION,
						Messages.OVF_CREATED, m_pageImportSource.ImageLength, capacity);

			OVF.FinalizeEnvelope(env);
			return env;
		}

		#endregion

		private void m_pageXvaStorage_ImportVmCompleted()
		{
			Program.Invoke(this, () =>
			                     	{
			                     		if (CurrentStepTabPage.GetType() == typeof(StoragePickerPage))
			                     		{
											m_pageFinish.ShowStartVmsGroupBox = m_pageXvaStorage.ImportedVm != null && !m_pageXvaStorage.ImportedVm.is_a_template;
											m_pageXvaNetwork.SetVm(m_pageXvaStorage.ImportedVm);
											NotifyNextPagesOfChange(m_pageXvaNetwork);
											NextStep();
			                     		}	
			                     	});

		}

        private void ShowXenAppXenDesktopWarning(IXenConnection connection)
        {
            if (connection != null && connection.Cache.Hosts.Any(h => h.DesktopFeaturesEnabled() || h.DesktopPlusFeaturesEnabled() || h.DesktopCloudFeaturesEnabled()))
                ShowInformationMessage(Helpers.GetPool(connection) != null ? Messages.NEWVMWIZARD_XENAPP_XENDESKTOP_INFO_MESSAGE_POOL : Messages.NEWVMWIZARD_XENAPP_XENDESKTOP_INFO_MESSAGE_SERVER);
            else
                HideInformationMessage();
        }

        private void pageHost_ConnectionSelectionChanged(IXenConnection connection)
        {
            ShowXenAppXenDesktopWarning(connection);
        }

		#region Nested items

		/// <summary>
		/// Type of the object we want to import
		/// </summary>
		public enum ImportType
		{
			/// <summary>
			/// Exported VM or template; filetype *xva
			/// </summary>
			Xva,
			/// <summary>
			/// Appliance; filetypes *.ovf, *.ova, *.ova.gz
			/// </summary>
			Ovf,
			/// <summary>
			/// Virtual disk image; filetypes *.vhd, *.vmdk (CA-61385: remove ".vdi", ".wim" support for Boston)
			/// </summary>
			Vhd
		}

		#endregion
	}
}
