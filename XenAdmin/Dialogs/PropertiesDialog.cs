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
        private CPUMemoryEditPage VCpuMemoryEditPage;
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
        private UpsellPage PerfmonAlertUpsellEditPage;
        private UpsellPage PerfmonAlertOptionsUpsellEditPage;
        private PerfmonAlertOptionsPage PerfmonAlertOptionsEditPage;
        private HostPowerONEditPage HostPowerONEditPage;
        private PoolPowerONEditPage PoolPowerONEditPage;
        private NewPolicySnapshotFrequencyPage newPolicySnapshotFrequencyPage1;
        private NewPolicySnapshotTypePage newPolicyVMSSTypePage1;
        private NewVMGroupVMsPage<VMSS> newVMSSVMsPage1;
        private NewVMGroupVMsPage<VM_appliance> newVMApplianceVMsPage1;
        private NewVMApplianceVMOrderAndDelaysPage newVmApplianceVmOrderAndDelaysPage1;
        private UpsellPage GpuUpsellEditPage;
        private GpuEditPage GpuEditPage;
        private PoolGpuEditPage PoolGpuEditPage;
        private VMEnlightenmentEditPage VMEnlightenmentEditPage;
        private Page_CloudConfigParameters CloudConfigParametersPage;
        private SecurityEditPage SecurityEditPage;
        private LivePatchingEditPage LivePatchingEditPage;
        private USBEditPage usbEditPage;
        private NetworkOptionsEditPage NetworkOptionsEditPage;
        private ClusteringEditPage ClusteringEditPage;
        #endregion

        private IXenObject xenObject, xenObjectBefore, xenObjectCopy;
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

            this.xenObject = xenObject;
            xenObjectBefore = xenObject.Clone();
            xenObjectCopy = xenObject.Clone();

            Name = String.Format("Edit{0}GeneralSettingsDialog", xenObject.GetType().Name);
            Text = String.Format(Messages.PROPERTIES_DIALOG_TITLE, Helpers.GetName(xenObject));

            if (!Application.RenderWithVisualStyles)
                ContentPanel.BackColor = SystemColors.Control;

            Build();
        }

        private void Build()
        {
            var pool = Helpers.GetPoolOfOne(connection);

            bool is_host = xenObjectCopy is Host;
            bool is_vm = xenObjectCopy is VM && !((VM)xenObjectCopy).is_a_snapshot;
            bool is_sr = xenObjectCopy is SR;

            bool is_pool = xenObjectCopy is Pool;
            bool is_vdi = xenObjectCopy is VDI;
            bool is_network = xenObjectCopy is XenAPI.Network;

            bool is_hvm = is_vm && ((VM)xenObjectCopy).IsHVM();
            bool is_template = is_vm && ((VM)xenObjectCopy).is_a_template;
            bool is_in_pool = Helpers.GetPool(xenObjectCopy.Connection) != null;

            bool is_pool_or_standalone = is_pool || (is_host && !is_in_pool);

            bool wlb_enabled = (Helpers.WlbEnabledAndConfigured(xenObjectCopy.Connection));

            bool is_VM_appliance = xenObjectCopy is VM_appliance;

            bool is_VMSS = xenObjectCopy is VMSS;

            ContentPanel.SuspendLayout();
            verticalTabs.BeginUpdate();

            try
            {
                verticalTabs.Items.Clear();

                ShowTab(GeneralEditPage = new GeneralEditPage());

                if (!is_VM_appliance)
                    ShowTab(CustomFieldsEditPage = new CustomFieldsDisplayPage {AutoScroll = true});

                if (is_vm)
                {
                    ShowTab(VCpuMemoryEditPage = new CPUMemoryEditPage());
                    ShowTab(StartupOptionsEditPage = new BootOptionsEditPage());
                    VMHAEditPage = new VMHAEditPage();
                    VMHAEditPage.Populated += EditPage_Populated;
                    ShowTab(VMHAEditPage);
                }

                if (is_vm || is_host || is_sr)
                {
                    if (Helpers.FeatureForbidden(xenObjectCopy, Host.RestrictAlerts))
                    {
                        PerfmonAlertUpsellEditPage = new UpsellPage {Image = Properties.Resources._000_Alert2_h32bit_16, Text = Messages.ALERTS};
                        PerfmonAlertUpsellEditPage.SetAllTexts(HiddenFeatures.LinkLabelHidden ? Messages.UPSELL_BLURB_ALERTS : Messages.UPSELL_BLURB_ALERTS + Messages.UPSELL_BLURB_TRIAL, 
                                                                InvisibleMessages.UPSELL_LEARNMOREURL_TRIAL);
                        ShowTab(PerfmonAlertUpsellEditPage);
                    }
                    else
                    {
                        ShowTab(PerfmonAlertEditPage = new PerfmonAlertEditPage {AutoScroll = true});
                    }
                }

                if (is_pool_or_standalone)
                {
                    if (Helpers.FeatureForbidden(xenObjectCopy, Host.RestrictAlerts))
                    {
                        PerfmonAlertOptionsUpsellEditPage = new UpsellPage {Image = Properties.Resources._000_Email_h32bit_16, Text = Messages.EMAIL_OPTIONS};
                        PerfmonAlertOptionsUpsellEditPage.SetAllTexts(HiddenFeatures.LinkLabelHidden ? Messages.UPSELL_BLURB_ALERTS : Messages.UPSELL_BLURB_ALERTS + Messages.UPSELL_BLURB_TRIAL,
                                                                        InvisibleMessages.UPSELL_LEARNMOREURL_TRIAL);
                        ShowTab(PerfmonAlertOptionsUpsellEditPage);
                    }
                    else
                    {
                        ShowTab(PerfmonAlertOptionsEditPage = new PerfmonAlertOptionsPage());
                    }
                }

                if (is_host)
                {
                    ShowTab(hostMultipathPage1 = new HostMultipathPage());
                    ShowTab(HostPowerONEditPage = new HostPowerONEditPage());
                    ShowTab(LogDestinationEditPage = new LogDestinationEditPage());
                }
                
                if (is_pool)
                    ShowTab(PoolPowerONEditPage = new PoolPowerONEditPage());

                if ((is_pool_or_standalone && Helpers.VGpuCapability(xenObjectCopy.Connection))
                    || (is_host && ((Host)xenObjectCopy).CanEnableDisableIntegratedGpu()))
                {
                    ShowTab(PoolGpuEditPage = new PoolGpuEditPage());
                }

                if (is_pool_or_standalone && !Helpers.FeatureForbidden(xenObject.Connection, Host.RestrictSslLegacySwitch))
                    ShowTab(SecurityEditPage = new SecurityEditPage());

                if (is_pool_or_standalone && !Helpers.FeatureForbidden(xenObject.Connection, Host.RestrictLivePatching))
                    ShowTab(LivePatchingEditPage = new LivePatchingEditPage());

                if (is_pool_or_standalone && !Helpers.FeatureForbidden(xenObject.Connection, Host.RestrictIGMPSnooping) && Helpers.GetMaster(pool).vSwitchNetworkBackend())
                    ShowTab(NetworkOptionsEditPage = new NetworkOptionsEditPage());

                if (is_pool_or_standalone && !Helpers.FeatureForbidden(xenObject.Connection, Host.RestrictCorosync))
                    ShowTab(ClusteringEditPage = new ClusteringEditPage());

                if (is_network)
                    ShowTab(editNetworkPage = new EditNetworkPage());

                if (is_vm && !wlb_enabled)
                    ShowTab(HomeServerPage = new HomeServerEditPage());

                if (is_vm && ((VM)xenObjectCopy).CanHaveGpu())
                {
                    if (Helpers.FeatureForbidden(xenObjectCopy, Host.RestrictGpu))
                    {
                        GpuUpsellEditPage = new UpsellPage { Image = Properties.Resources._000_GetMemoryInfo_h32bit_16, Text = Messages.GPU };
                        GpuUpsellEditPage.SetAllTexts(HiddenFeatures.LinkLabelHidden ? Messages.UPSELL_BLURB_GPU : Messages.UPSELL_BLURB_GPU + Messages.UPSELL_BLURB_TRIAL,
                                                        InvisibleMessages.UPSELL_LEARNMOREURL_TRIAL);
                        ShowTab(GpuUpsellEditPage);
                    }
                    else
                    {
                        ShowTab(GpuEditPage = new GpuEditPage());
                    }
                }

                if (is_hvm && !is_template && !Helpers.FeatureForbidden(xenObjectCopy, Host.RestrictUsbPassthrough) &&
                    pool.Connection.Cache.Hosts.Any(host => host.PUSBs.Count > 0))
                {
                    usbEditPage = new USBEditPage();
                    usbEditPage.Populated += EditPage_Populated;
                    ShowTab(usbEditPage);
                }

                if (is_hvm)
                {
                    ShowTab(VMAdvancedEditPage = new VMAdvancedEditPage());
                }

                if (is_vm && Helpers.ContainerCapability(xenObject.Connection) && ((VM)xenObjectCopy).CanBeEnlightened())
                    ShowTab(VMEnlightenmentEditPage = new VMEnlightenmentEditPage());

                if (is_vm && Helpers.ContainerCapability(xenObject.Connection) && ((VM)xenObjectCopy).CanHaveCloudConfigDrive())
                    ShowTab(CloudConfigParametersPage = new Page_CloudConfigParameters());

                if(is_VMSS)
                {
                    ShowTab(newVMSSVMsPage1 = new NewVMGroupVMsPage<VMSS> {Pool = pool});
                    ShowTab(newPolicyVMSSTypePage1 = new NewPolicySnapshotTypePage());
                    newPolicySnapshotFrequencyPage1 = new NewPolicySnapshotFrequencyPage {Connection = pool.Connection};
                    newPolicySnapshotFrequencyPage1.Populated += EditPage_Populated;
                    ShowTab(newPolicySnapshotFrequencyPage1);
                }

                if (is_VM_appliance)
                {
                    ShowTab(newVMApplianceVMsPage1 = new NewVMGroupVMsPage<VM_appliance> { Pool = pool });
                    ShowTab(newVmApplianceVmOrderAndDelaysPage1 = new NewVMApplianceVMOrderAndDelaysPage { Pool = pool });
                }

                //
                // Now add one tab per VBD (for VDIs only)
                //

                if (!is_vdi)
                    return;

                ShowTab(vdiSizeLocation = new VDISizeLocationPage());

                VDI vdi = xenObjectCopy as VDI;

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
                        delegate(Session session)
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

            editPage.SetXenObjects(xenObject, xenObjectCopy);
            verticalTabs.Items.Add(editPage);
        }

        private void EditPage_Populated()
        {
            verticalTabs.Refresh();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Have any of the fields in the tab pages changed?
            if (!HasChanged)
            {
                Close();
                return;
            }

            if (!ValidToSave)
            {
                // Keep dialog open and allow user to correct the error as
                // indicated by the balloon tooltip.
                DialogResult = DialogResult.None;
                return;
            }

            // Yes, save to the LocalXenObject.
            List<AsyncAction> actions = SaveSettings();

            Program.Invoke(Program.MainWindow.GeneralPage, Program.MainWindow.GeneralPage.EnableDisableEdit);

            // Add a save changes on the beginning of the actions to enact the alterations that were just changes to the xenObjectCopy.
            // Must come first because some pages' SaveChanges() rely on modifying the object via the xenObjectCopy before their actions are run.

            if(xenObjectBefore is VMSS)
            {
                XenAPI.VMSS VMSSObj = xenObjectBefore as XenAPI.VMSS;
                if (VMSSObj.type == vmss_type.snapshot_with_quiesce)
                {
                    actions.Insert(0, new SaveChangesAction(xenObjectCopy, xenObjectBefore, true));
                }
                else
                {
                    actions.Insert(actions.Count, new SaveChangesAction(xenObjectCopy, xenObjectBefore, true));
                }
            }
            else
            {
                actions.Insert(0, new SaveChangesAction(xenObjectCopy, xenObjectBefore, true)); 
            }

            var objName = Helpers.GetName(xenObject).Ellipsise(50);
            _action = new MultipleAction(
                connection,
                string.Format(Messages.UPDATE_PROPERTIES, objName),
                Messages.UPDATING_PROPERTIES,
                string.Format(Messages.UPDATED_PROPERTIES, objName),
                actions);

            _action.SetObject(xenObjectCopy);
            
            _action.Completed += action_Completed;
            Close();

            if (_startAction)
            {
                _action.RunAsync();
            }
        }

        private void action_Completed(ActionBase sender)
        {
            Program.Invoke(Program.MainWindow.GeneralPage, Program.MainWindow.GeneralPage.EnableDisableEdit);
        }

        /*
         * Iterates through all of the tab pages checking for changes and
         * return the status.
         */
        private bool HasChanged
        {
            get
            {
                foreach (IEditPage editPage in verticalTabs.Items)
                    if (editPage.HasChanged)
                        return true;

                return false;
            }
        }

        /*
         * Iterate through all tab pages looking for local validation errors.  If
         * we encounter a local validation error on a TabPage, then make the TabPage
         * the selected, and have the inner control show one or more balloon tips.  Keep
         * the dialog open.
         */
        private bool ValidToSave
        {
            get
            {
                foreach (IEditPage editPage in verticalTabs.Items)
                    if (!editPage.ValidToSave)
                    {
                        SelectPage(editPage);

                        // Show local validation balloon message for this tab page.
                        editPage.ShowLocalValidationMessages();

                        return false;
                    }

                return true;
            }
        }

        /* 
         * Iterates through all of the tab pages, saving changes to their cloned XenObjects,
         * and accumulating and returning their Actions for further processing.
         */
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
                    VMHAEditPage.GpuGroup = GpuEditPage.GpuGroup;
                    VMHAEditPage.VgpuType = GpuEditPage.VgpuType;
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

