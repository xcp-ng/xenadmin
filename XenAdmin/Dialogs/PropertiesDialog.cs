/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Wizards.NewVMWizard;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.SettingsPanels;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Wizards.NewPolicyWizard;
using XenAdmin.Wizards.NewVMApplianceWizard;
using XenAdmin.Wizards.GenericPages;
using System.Linq;

namespace XenAdmin.Dialogs
{
    public partial class PropertiesDialog : VerticallyTabbedDialog
    {
        #region Tabs
        private CpuMemoryEditPage VCpuMemoryEditPage;
        private HostMultipathPage hostMultipathPage1;
        private CustomFieldsDisplayPage CustomFieldsEditPage;
        private LogDestinationEditPage LogDestinationEditPage;
        private HomeServerEditPage HomeServerPage;
        private BootOptionsEditPage StartupOptionsEditPage;
        private VMAdvancedEditPage VMAdvancedEditPage;
        private PerfmonAlertEditPage PerfmonAlertEditPage;
        private EditNetworkPage editNetworkPage;
        private VDISizeLocationPage vdiSizeLocation;
        private VMHAEditPage VMHAEditPage;
        private GeneralEditPage GeneralEditPage;
        private PerfmonAlertOptionsPage PerfmonAlertOptionsEditPage;
        private HostPowerONEditPage HostPowerONEditPage;
        private HostAutostartEditPage HostAutostartEditPage;
        private NewPolicySnapshotFrequencyPage newPolicySnapshotFrequencyPage1;
        private NewPolicySnapshotTypePage newPolicyVMSSTypePage1;
        private NewVMGroupVMsPage<VMSS> newVMSSVMsPage1;
        private NewVMGroupVMsPage<VM_appliance> newVMApplianceVMsPage1;
        private NewVMApplianceVMOrderAndDelaysPage newVmApplianceVmOrderAndDelaysPage1;
        private GpuEditPage GpuEditPage;
        private PoolGpuEditPage PoolGpuEditPage;
        private VMEnlightenmentEditPage VMEnlightenmentEditPage;
        private Page_CloudConfigParameters CloudConfigParametersPage;
        private SecurityEditPage SecurityEditPage;
        private LivePatchingEditPage LivePatchingEditPage;
        private USBEditPage usbEditPage;
        private NetworkOptionsEditPage NetworkOptionsEditPage;
        private ClusteringEditPage ClusteringEditPage;
        private SrReadCachingEditPage SrReadCachingEditPage;
        private PoolAdvancedEditPage _poolAdvancedEditPage;
        private NRPEEditPage NRPEEditPage;
        #endregion

        private readonly IXenObject _xenObjectBefore, _xenObjectCopy;
        private AsyncAction _action;
        private bool _startAction = true;
        private System.Timers.Timer timer = new System.Timers.Timer();

        public new event EventHandler<PropertiesDialogClosingEventArgs> FormClosing;

        public PropertiesDialog(IXenObject xenObject)
            : base(xenObject.Connection)
        {
            // xenObject must not be null. If this occurs, we shouldn't have offered Properties in the UI.
            Debug.Assert(xenObject != null, "XenObject is null");

            InitializeComponent();

            _xenObjectBefore = xenObject;
            _xenObjectCopy = xenObject.Clone();

            Name = string.Format("Edit{0}GeneralSettingsDialog", xenObject.GetType().Name);
            Text = string.Format(Messages.PROPERTIES_DIALOG_TITLE, Helpers.GetName(xenObject));

            if (!Application.RenderWithVisualStyles)
                ContentPanel.BackColor = SystemColors.Control;

            Build();
        }

        private void Build()
        {
            var pool = Helpers.GetPoolOfOne(connection);

            bool isHost = _xenObjectCopy is Host;
            bool isVm = _xenObjectCopy is VM vm && !vm.is_a_snapshot;
            bool isSr = _xenObjectCopy is SR;
            bool isPool = _xenObjectCopy is Pool;
            bool isVdi = _xenObjectCopy is VDI;
            bool isNetwork = _xenObjectCopy is XenAPI.Network;
            bool isPoolOrStandalone = isPool || (isHost && Helpers.GetPool(_xenObjectCopy.Connection) == null);
            bool isVmAppliance = _xenObjectCopy is VM_appliance;
            bool isVmss = _xenObjectCopy is VMSS;

            ContentPanel.SuspendLayout();
            verticalTabs.BeginUpdate();

            try
            {
                verticalTabs.Items.Clear();

                ShowTab(GeneralEditPage = new GeneralEditPage());

                if (!isVmAppliance && !isVmss)
                    ShowTab(CustomFieldsEditPage = new CustomFieldsDisplayPage { AutoScroll = true });

                if (isVm)
                {
                    ShowTab(VCpuMemoryEditPage = new CpuMemoryEditPage());
                    ShowTab(StartupOptionsEditPage = new BootOptionsEditPage());
                    VMHAEditPage = new VMHAEditPage();
                    VMHAEditPage.Populated += EditPage_Populated;
                    ShowTab(VMHAEditPage);
                }

                if (isVm || isHost || isSr)
                {
                    ShowTab(PerfmonAlertEditPage = new PerfmonAlertEditPage { AutoScroll = true });
                }

                if (isPoolOrStandalone)
                {
                    ShowTab(PerfmonAlertOptionsEditPage = new PerfmonAlertOptionsPage());
                }

                if (isHost)
                {
                    ShowTab(hostMultipathPage1 = new HostMultipathPage());
                    ShowTab(LogDestinationEditPage = new LogDestinationEditPage());
                }

                if (isHost || isPool)
                    ShowTab(HostPowerONEditPage = new HostPowerONEditPage());

                if (isHost)
                    ShowTab(HostAutostartEditPage = new HostAutostartEditPage());

                if ((isPoolOrStandalone && Helpers.VGpuCapability(_xenObjectCopy.Connection))
                    || (isHost && ((Host)_xenObjectCopy).CanEnableDisableIntegratedGpu()))
                {
                    ShowTab(PoolGpuEditPage = new PoolGpuEditPage());
                }

                if (isPoolOrStandalone && !Helpers.FeatureForbidden(_xenObjectCopy.Connection, Host.RestrictSslLegacySwitch) && !Helpers.StockholmOrGreater(connection))
                    ShowTab(SecurityEditPage = new SecurityEditPage());

                if (isPoolOrStandalone && !Helpers.FeatureForbidden(_xenObjectCopy.Connection, Host.RestrictLivePatching) && !Helpers.CloudOrGreater(connection))
                    ShowTab(LivePatchingEditPage = new LivePatchingEditPage());

                if (isPoolOrStandalone && !Helpers.FeatureForbidden(_xenObjectCopy.Connection, Host.RestrictIGMPSnooping) && Helpers.GetCoordinator(pool).vSwitchNetworkBackend())
                    ShowTab(NetworkOptionsEditPage = new NetworkOptionsEditPage());

                if (isPoolOrStandalone && !Helpers.FeatureForbidden(_xenObjectCopy.Connection, Host.RestrictCorosync))
                    ShowTab(ClusteringEditPage = new ClusteringEditPage());

                if (isPool && Helpers.CloudOrGreater(_xenObjectCopy.Connection) && Helpers.XapiEqualOrGreater_22_33_0(_xenObjectCopy.Connection))
                    ShowTab(_poolAdvancedEditPage = new PoolAdvancedEditPage());

                if (isNetwork)
                    ShowTab(editNetworkPage = new EditNetworkPage());

                if (isVm)
                {
                    var theVm = (VM)_xenObjectCopy;

                    if (!Helpers.WlbEnabledAndConfigured(_xenObjectCopy.Connection))
                        ShowTab(HomeServerPage = new HomeServerEditPage());

                    if (theVm.CanHaveGpu())
                    {
                        if (Helpers.GpusAvailable(connection))
                            ShowTab(GpuEditPage = new GpuEditPage());
                    }

                    if (theVm.IsHVM())
                    {
                        if (!theVm.is_a_template && !Helpers.FeatureForbidden(_xenObjectCopy, Host.RestrictUsbPassthrough) &&
                            pool.Connection.Cache.Hosts.Any(host => host.PUSBs.Count > 0))
                        {
                            usbEditPage = new USBEditPage();
                            usbEditPage.Populated += EditPage_Populated;
                            ShowTab(usbEditPage);
                        }

                        ShowTab(VMAdvancedEditPage = new VMAdvancedEditPage());
                    }

                    if (Helpers.ContainerCapability(_xenObjectCopy.Connection))
                    {
                        if (theVm.CanBeEnlightened())
                            ShowTab(VMEnlightenmentEditPage = new VMEnlightenmentEditPage());

                        if (theVm.CanHaveCloudConfigDrive())
                            ShowTab(CloudConfigParametersPage = new Page_CloudConfigParameters());
                    }
                }

                if (isVmss)
                {
                    ShowTab(newVMSSVMsPage1 = new NewVMGroupVMsPage<VMSS> { Pool = pool });
                    ShowTab(newPolicyVMSSTypePage1 = new NewPolicySnapshotTypePage());
                    newPolicySnapshotFrequencyPage1 = new NewPolicySnapshotFrequencyPage { Connection = pool.Connection };
                    newPolicySnapshotFrequencyPage1.Populated += EditPage_Populated;
                    ShowTab(newPolicySnapshotFrequencyPage1);
                }

                if (isVmAppliance)
                {
                    ShowTab(newVMApplianceVMsPage1 = new NewVMGroupVMsPage<VM_appliance> { Pool = pool });
                    ShowTab(newVmApplianceVmOrderAndDelaysPage1 = new NewVMApplianceVMOrderAndDelaysPage { Pool = pool });
                }

                if (isSr && ((SR)_xenObjectCopy).SupportsReadCaching() && !Helpers.FeatureForbidden(_xenObjectCopy, Host.RestrictReadCaching))
                    ShowTab(SrReadCachingEditPage = new SrReadCachingEditPage());

                if (isVdi)
                {
                    ShowTab(vdiSizeLocation = new VDISizeLocationPage());

                    VDI vdi = _xenObjectCopy as VDI;

                    List<VBDEditPage> vbdEditPages = new List<VBDEditPage>();

                    foreach (VBD vbd in vdi.Connection.ResolveAll(vdi.VBDs))
                    {
                        VBDEditPage editPage = new VBDEditPage();

                        editPage.SetXenObjects(null, vbd);
                        vbdEditPages.Add(editPage);
                        ShowTab(editPage);
                    }

                    if (vbdEditPages.Count <= 0)
                        return;

                    using (var dialog = new ActionProgressDialog(
                               new DelegatedAsyncAction(vdi.Connection, Messages.DEVICE_POSITION_SCANNING,
                                   Messages.DEVICE_POSITION_SCANNING, Messages.DEVICE_POSITION_SCANNED,
                                   delegate (Session session)
                                   {
                                       foreach (VBDEditPage page in vbdEditPages)
                                           page.UpdateDevicePositions(session);
                                   }),
                               ProgressBarStyle.Continuous))
                    {
                        dialog.ShowCancel = true;
                        dialog.ShowDialog(Program.MainWindow);
                    }
                }
                if (isPoolOrStandalone && Helpers.XapiEqualOrGreater_23_27_0(connection)
                    && (connection.Session.IsLocalSuperuser || connection.Session.Roles.Any(r => r.name_label == Role.MR_ROLE_POOL_ADMIN)))
                {
                    NRPEEditPage = new NRPEEditPage();
                    ShowTab(NRPEEditPage);
                }
            }
            finally
            {
                ContentPanel.ResumeLayout();
                verticalTabs.EndUpdate();
                verticalTabs.SelectedIndex = 0;
            }
        }

        private void ShowTab(IEditPage editPage)
        {
            var pageAsControl = (Control)editPage;
            ContentPanel.Controls.Add(pageAsControl);
            pageAsControl.BackColor = Color.Transparent;
            pageAsControl.Dock = DockStyle.Fill;

            editPage.SetXenObjects(_xenObjectBefore, _xenObjectCopy);
            verticalTabs.Items.Add(editPage);
        }

        private void EditPage_Populated()
        {
            verticalTabs.Refresh();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var changedPageExists = false;

            foreach (IEditPage editPage in verticalTabs.Items)
            {
                if (!editPage.ValidToSave)
                {
                    SelectPage(editPage);

                    editPage.ShowLocalValidationMessages();
                    DialogResult = DialogResult.None;
                    return;
                }

                if (editPage.HasChanged)
                    changedPageExists = true;
            }

            if (!changedPageExists)
            {
                Close();
                return;
            }

            // Yes, save to the LocalXenObject.
            List<AsyncAction> actions = SaveSettings();

            Program.Invoke(Program.MainWindow.GeneralPage, Program.MainWindow.GeneralPage.UpdateButtons);

            // Add a save changes on the beginning of the actions to enact the alterations that were just changes to the xenObjectCopy.
            // Must come first because some pages' SaveChanges() rely on modifying the object via the xenObjectCopy before their actions are run.

            int index = 0;
            if (_xenObjectBefore is VMSS vmss && vmss.type != vmss_type.snapshot_with_quiesce)
                index = actions.Count;

            actions.Insert(index, new SaveChangesAction(_xenObjectCopy, true, _xenObjectBefore));

            var objName = Helpers.GetName(_xenObjectBefore).Ellipsise(50);
            _action = new MultipleAction(
                connection,
                string.Format(Messages.UPDATE_PROPERTIES, objName),
                Messages.UPDATING_PROPERTIES,
                string.Format(Messages.UPDATED_PROPERTIES, objName),
                actions);

            _action.SetObject(_xenObjectCopy);

            _action.Completed += action_Completed;
            Close();

            if (_startAction)
            {
                _xenObjectBefore.Locked = true;
                _action.RunAsync();
            }
        }

        private void action_Completed(ActionBase sender)
        {
            _xenObjectBefore.Locked = false;
            Program.Invoke(Program.MainWindow.GeneralPage, Program.MainWindow.GeneralPage.UpdateButtons);
        }

        /// <summary>
        /// Iterates through all of the tab pages, saving changes to their cloned XenObjects,
        /// and accumulating and returning their Actions for further processing.
        /// </summary>
        private List<AsyncAction> SaveSettings()
        {
            List<AsyncAction> actions = new List<AsyncAction>();
            AsyncAction finalAction = null;

            foreach (IEditPage editPage in verticalTabs.Items)
            {
                if (!editPage.HasChanged)
                    continue;

                AsyncAction action = editPage.SaveSettings();
                if (action == null)
                    continue;

                if (action is SetSslLegacyAction)
                    finalAction = action;  // annoying special case: SetSslLegacyAction must be last because it will disrupt the connection and we may lose later actions
                else
                    actions.Add(action);
            }

            if (finalAction != null)
                actions.Add(finalAction);
            return actions;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        protected sealed override void OnFormClosing(FormClosingEventArgs e)
        {
            foreach (IEditPage editPage in verticalTabs.Items)
            {
                editPage.Cleanup();
            }

            var args = new PropertiesDialogClosingEventArgs(_action, _startAction);

            OnFormClosing(args);

            _startAction = args.StartAction;
        }

        protected virtual void OnFormClosing(PropertiesDialogClosingEventArgs e)
        {
            var handler = FormClosing;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void verticalTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var snapshotTypePage = verticalTabs.SelectedItem as NewPolicySnapshotTypePage;
            if (snapshotTypePage != null)
            {
                newPolicyVMSSTypePage1.ToggleQuiesceCheckBox(newVMSSVMsPage1.SelectedVMs);
                return;
            }

            var vmApplianceStartOrderPage = verticalTabs.SelectedItem as NewVMApplianceVMOrderAndDelaysPage;
            if (vmApplianceStartOrderPage != null && newVMApplianceVMsPage1 != null)
            {
                vmApplianceStartOrderPage.SetSelectedVMs(newVMApplianceVMsPage1.SelectedVMs);
                return;
            }

            if (verticalTabs.SelectedItem == VMHAEditPage)
            {
                VMHAEditPage.StartNtolUpdate();
                if (GpuEditPage != null)
                {
                    VMHAEditPage.VGpus = GpuEditPage.VGpus;
                    VMHAEditPage.RefillPrioritiesComboBox();
                }
                return;
            }

            if (verticalTabs.SelectedItem == GpuEditPage && VMHAEditPage != null)
            {
                GpuEditPage.SelectedPriority = VMHAEditPage.SelectedPriority;
                GpuEditPage.ShowHideWarnings();
                return;
            }

            if (verticalTabs.SelectedItem == usbEditPage && VMHAEditPage != null)
            {
                usbEditPage.SelectedPriority = VMHAEditPage.SelectedPriority;
                usbEditPage.ShowHideWarnings();
                return;
            }

            if (verticalTabs.SelectedItem == HostPowerONEditPage)
            {
                HostPowerONEditPage.LoadPowerOnMode();
                return;
            }

            HideToolTips();
        }

        private void HideToolTips()
        {
            foreach (IEditPage page in verticalTabs.Items)
            {
                page.HideLocalValidationMessages();
            }
        }

        private void PropertiesDialog_Move(object sender, EventArgs e)
        {
            HideToolTips();
        }

        private void PropertiesDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer.Stop();
        }

        #region Select page methods

        public void EditName()
        {
            SelectPage(GeneralEditPage);
            GeneralEditPage.SelectName();
        }

        public void EditDescription()
        {
            SelectPage(GeneralEditPage);
            GeneralEditPage.SelectDescription();
        }

        public void EditIqn()
        {
            SelectPage(GeneralEditPage);
            GeneralEditPage.SelectIqn();
        }

        public void SelectCustomFieldsEditPage()
        {
            SelectPage(CustomFieldsEditPage);
        }

        public void SelectPoolGpuEditPage()
        {
            SelectPage(PoolGpuEditPage);
        }

        public void SelectPerfmonAlertEditPage()
        {
            SelectPage(PerfmonAlertEditPage);
        }

        public void SelectStartupOptionsEditPage()
        {
            SelectPage(StartupOptionsEditPage);
        }

        public void SelectHomeServerEditPage()
        {
            SelectPage(HomeServerPage);
        }

        public void SelectLogDestinationEditPage()
        {
            SelectPage(LogDestinationEditPage);
        }

        public void SelectVMHAEditPage()
        {
            SelectPage(VMHAEditPage);
        }

        public void SelectVMCPUEditPage()
        {
            SelectPage(VCpuMemoryEditPage);
        }

        public void SelectVdiSizeLocationPage()
        {
            SelectPage(vdiSizeLocation);
        }

        public void SelectClusteringEditPage()
        {
            SelectPage(ClusteringEditPage);
        }

        #endregion
    }

    public class PropertiesDialogClosingEventArgs : EventArgs
    {
        public AsyncAction Action { get; private set; }
        public bool StartAction { get; set; }
        public PropertiesDialogClosingEventArgs(AsyncAction action, bool startAction)
        {
            StartAction = startAction;
            Action = action;
        }
    }
}

