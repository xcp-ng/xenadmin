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

using System.Linq;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Network;
using XenAdmin.Actions.VMActions;
using XenAdmin.SettingsPanels;

using XenAPI;
using System.Collections.Generic;
using XenAdmin.Wizards.GenericPages;
using XenAdmin.Core;

using XenAdmin.Actions;
using System.Windows.Forms;

namespace XenAdmin.Wizards.NewVMWizard
{
    public partial class NewVMWizard : XenWizardBase
    {
        private readonly Page_Template page_1_Template;
        private readonly Page_CopyBiosStrings page_1b_BiosLocking;
        private readonly Page_Name page_2_Name;
        private readonly Page_InstallationMedia page_3_InstallationMedia;
        private readonly Page_HomeServer page_4_HomeServer;
        private readonly Page_CpuMem page_5_CpuMem;
        private readonly Page_Storage page_6_Storage;
        private readonly Page_Networking page_7_Networking;
        private readonly Page_Finish page_8_Finish;
        private readonly RBACWarningPage page_RbacWarning;
        private readonly LunPerVdiNewVMMappingPage page_6b_LunPerVdi;
        private readonly GpuEditPage pageVgpu;
        private readonly Page_CloudConfigParameters page_CloudConfigParameters;

        private Host m_affinity;
        private bool BlockAffinitySelection = false;
        private bool gpuCapability;

        public AsyncAction Action;

        public NewVMWizard(IXenConnection connection, VM template, Host affinity)
            : base(connection)
        {
            InitializeComponent();

            page_1_Template = new Page_Template();
            page_1b_BiosLocking = new Page_CopyBiosStrings();
            page_2_Name = new Page_Name();
            page_3_InstallationMedia = new Page_InstallationMedia();
            page_4_HomeServer = new Page_HomeServer();
            page_5_CpuMem = new Page_CpuMem();
            page_6_Storage = new Page_Storage();
            page_7_Networking = new Page_Networking();
            page_8_Finish = new Page_Finish();
            page_RbacWarning = new RBACWarningPage();
            page_6b_LunPerVdi = new LunPerVdiNewVMMappingPage { Connection = xenConnection };
            pageVgpu = new GpuEditPage();
            page_CloudConfigParameters = new Page_CloudConfigParameters();

            #region RBAC Warning Page Checks
            if (connection.Session.IsLocalSuperuser || Helpers.GetMaster(connection).external_auth_type == Auth.AUTH_TYPE_NONE)
            {
                //page_RbacWarning.DisableStep = true;
            }
            else
            {
                // Check to see if they can even create a VM
                var createCheck = new RBACWarningPage.WizardPermissionCheck(Messages.RBAC_WARNING_VM_WIZARD_BLOCK);
                foreach (RbacMethod method in CreateVMAction.StaticRBACDependencies)
                    createCheck.AddApiCheck(method);
                createCheck.Blocking = true;

                // Check to see if they can set memory values
                var memCheck = new RBACWarningPage.WizardPermissionCheck(Messages.RBAC_WARNING_VM_WIZARD_MEM);
                memCheck.AddApiCheck("vm.set_memory_limits");
                memCheck.WarningAction = new RBACWarningPage.PermissionCheckActionDelegate(delegate()
                {
                    // no point letting them continue
                    page_5_CpuMem.DisableMemoryControls();
                });


                // Check to see if they can set the VM's affinity
                var affinityCheck = new RBACWarningPage.WizardPermissionCheck(Messages.RBAC_WARNING_VM_WIZARD_AFFINITY);
                affinityCheck.ApiCallsToCheck.Add("vm.set_affinity");
                affinityCheck.WarningAction = new RBACWarningPage.PermissionCheckActionDelegate(delegate()
                {
                    page_4_HomeServer.DisableStep = true;
                    BlockAffinitySelection = true;
                    Program.Invoke(this, RefreshProgress);
                });

                page_RbacWarning.AddPermissionChecks(xenConnection, createCheck, affinityCheck, memCheck);

                if (Helpers.GpuCapability(xenConnection))
                {
                    var vgpuCheck = new RBACWarningPage.WizardPermissionCheck(Messages.RBAC_WARNING_VM_WIZARD_GPU);
                    vgpuCheck.ApiCallsToCheck.Add("vgpu.create");
                    vgpuCheck.WarningAction = new RBACWarningPage.PermissionCheckActionDelegate(() =>
                    {
                        pageVgpu.DisableStep = true;
                        Program.Invoke(this, RefreshProgress);
                    });

                    page_RbacWarning.AddPermissionChecks(xenConnection, vgpuCheck);
                }

                AddPage(page_RbacWarning, 0);
            }
            #endregion

            page_8_Finish.SummaryRetreiver = GetSummary;

            AddPages(page_1_Template, page_2_Name, page_3_InstallationMedia, page_4_HomeServer,
                     page_5_CpuMem, page_6_Storage, page_7_Networking, page_8_Finish);

            m_affinity = affinity;
            page_1_Template.SelectedTemplate = template;
            page_1b_BiosLocking.Affinity = affinity;
            page_3_InstallationMedia.Affinity = affinity;
            page_4_HomeServer.Affinity = affinity;
            page_6_Storage.Affinity = affinity;

            ShowXenAppXenDesktopWarning(connection);
        }

        protected override void FinishWizard()
        {
            Action = new CreateVMAction(xenConnection,
                                        page_1_Template.SelectedTemplate,
                                        page_1_Template.CopyBiosStrings
                                            ? page_1b_BiosLocking.CopyBiosStringsFrom
                                            : null,
                                        page_2_Name.SelectedName,
                                        page_2_Name.SelectedDescription,
                                        page_3_InstallationMedia.SelectedInstallMethod,
                                        page_3_InstallationMedia.SelectedPvArgs,
                                        page_3_InstallationMedia.SelectedCD,
                                        page_3_InstallationMedia.SelectedUrl,
                                        page_3_InstallationMedia.SelectedBootMode,
                                        m_affinity,
                                        page_5_CpuMem.SelectedVcpusMax,
                                        page_5_CpuMem.SelectedVcpusAtStartup,
                                        (long)page_5_CpuMem.SelectedMemoryDynamicMin,
                                        (long)page_5_CpuMem.SelectedMemoryDynamicMax,
                                        (long)page_5_CpuMem.SelectedMemoryStaticMax,
                                        page_6b_LunPerVdi.MapLunsToVdisRequired
                                            ? page_6b_LunPerVdi.MappedDisks
                                            : page_6_Storage.SelectedDisks,
                                        page_6_Storage.FullCopySR,
                                        page_7_Networking.SelectedVifs,
                                        page_8_Finish.StartImmediately,
                                        VMOperationCommand.WarningDialogHAInvalidConfig,
                                        VMOperationCommand.StartDiagnosisForm,
                                        gpuCapability ? pageVgpu.VGpus : null,
                                        pageVgpu.HasChanged,
                                        page_5_CpuMem.SelectedCoresPerSocket,
                                        page_CloudConfigParameters.ConfigDriveTemplateText);

            Action.RunAsync();

            base.FinishWizard();
        }

        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            if (page_CloudConfigParameters != null && page_CloudConfigParameters.ActiveControl is TextBox && e.KeyChar == (char)Keys.Enter)
                return;

            base.OnKeyPress(e);
        }
        protected override void UpdateWizardContent(XenTabPage senderPage)
        {
            var prevPageType = senderPage.GetType();
                      
            if (prevPageType == typeof(Page_Template))
            {
                var selectedTemplate = page_1_Template.SelectedTemplate;

                page_1b_BiosLocking.SelectedTemplate = selectedTemplate;
                page_2_Name.SelectedTemplate = selectedTemplate;
                page_3_InstallationMedia.SelectedTemplate = selectedTemplate;
                page_4_HomeServer.SelectedTemplate = selectedTemplate;
                page_5_CpuMem.SelectedTemplate = selectedTemplate;
                pageVgpu.vm = selectedTemplate;
                page_6_Storage.Template = selectedTemplate;
                page_7_Networking.SelectedTemplate = selectedTemplate;
                page_CloudConfigParameters.Affinity = m_affinity;
                page_CloudConfigParameters.SelectedTemplate = selectedTemplate;


                RemovePage(pageVgpu);
                gpuCapability = Helpers.GpuCapability(xenConnection) && selectedTemplate.CanHaveGpu();
                if (gpuCapability)
                    AddAfterPage(page_5_CpuMem, pageVgpu);

                RemovePage(page_1b_BiosLocking);

                if (page_1_Template.CopyBiosStrings)
                {
                    // insert after template page
                    AddAfterPage(page_1_Template, page_1b_BiosLocking);

                    page_4_HomeServer.DisableStep = selectedTemplate.DefaultTemplate();
                }
                else
                {
                    if (!BlockAffinitySelection)
                        page_4_HomeServer.DisableStep = false;
                }

                // if custom template has no cd drive (must have been removed via cli) don't add one
                var noInstallMedia = Helpers.CustomWithNoDVD(selectedTemplate);

                if (selectedTemplate != null && selectedTemplate.DefaultTemplate() && string.IsNullOrEmpty(selectedTemplate.InstallMethods()))
                    noInstallMedia = true;

                page_3_InstallationMedia.ShowInstallationMedia = !noInstallMedia;

                // The user cannot set their own affinity, use the one off the template
                if (BlockAffinitySelection)
                    m_affinity = xenConnection.Resolve(selectedTemplate.affinity);

                RemovePage(page_CloudConfigParameters);
                if (selectedTemplate != null && selectedTemplate.CanHaveCloudConfigDrive() && Helpers.ContainerCapability(xenConnection))
                {
                    AddAfterPage(page_6_Storage, page_CloudConfigParameters);
                }
            }
            else if (prevPageType == typeof(Page_CopyBiosStrings))
            {
                if (page_1_Template.CopyBiosStrings && page_1_Template.SelectedTemplate.DefaultTemplate())
                {
                    m_affinity = page_1b_BiosLocking.CopyBiosStringsFrom;
                    page_4_HomeServer.Affinity = m_affinity;
                    page_6_Storage.Affinity = m_affinity;
                }
            }
            else if (prevPageType == typeof(Page_Name))
            {
                var selectedName = page_2_Name.SelectedName;
                page_6_Storage.SelectedName = selectedName;
                page_7_Networking.SelectedName = selectedName;
            }
            else if (prevPageType == typeof(Page_InstallationMedia))
            {
                var selectedInstallMethod = page_3_InstallationMedia.SelectedInstallMethod;
                page_4_HomeServer.SelectedCD = page_3_InstallationMedia.SelectedCD;
                page_4_HomeServer.SelectedInstallMethod = selectedInstallMethod;
                page_6_Storage.SelectedInstallMethod = selectedInstallMethod;
            }
            else if (prevPageType == typeof(Page_HomeServer))
            {
                if (!page_4_HomeServer.DisableStep)
                {
                    m_affinity = page_4_HomeServer.SelectedHomeServer;
                    page_6_Storage.Affinity = m_affinity;
                    page_CloudConfigParameters.Affinity = m_affinity;
                }
            }
            else if (prevPageType == typeof(Page_Storage))
            {
                RemovePage(page_6b_LunPerVdi);
                List<SR> srs = page_6_Storage.SelectedDisks.ConvertAll(d => xenConnection.Resolve(d.Disk.SR));
                if (srs.Any(sr => sr.HBALunPerVDI()))
                {
                    page_6b_LunPerVdi.DisksToMap = page_6_Storage.SelectedDisks;
                    AddAfterPage(page_6_Storage, page_6b_LunPerVdi);
                }
            }
        }

        protected override string WizardPaneHelpID()
        {
            return CurrentStepTabPage is RBACWarningPage ? FormatHelpId("Rbac") : base.WizardPaneHelpID();
        }

        private void ShowXenAppXenDesktopWarning(IXenConnection connection)
        {
            if (connection != null && connection.Cache.Hosts.Any(h => h.DesktopFeaturesEnabled() || h.DesktopPlusFeaturesEnabled() || h.DesktopCloudFeaturesEnabled()))
                ShowInformationMessage(Helpers.GetPool(connection) != null ? Messages.NEWVMWIZARD_XENAPP_XENDESKTOP_INFO_MESSAGE_POOL : Messages.NEWVMWIZARD_XENAPP_XENDESKTOP_INFO_MESSAGE_SERVER);
            else
                HideInformationMessage();
        }
    }
}
