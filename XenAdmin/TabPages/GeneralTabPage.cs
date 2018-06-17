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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.CustomFields;
using XenAdmin.Dialogs;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAdmin.SettingsPanels;
using XenAPI;
using XenCenterLib;

namespace XenAdmin.TabPages
{
    public partial class GeneralTabPage : BaseTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Dictionary<Type, List<PDSection>> _expandedSections = new Dictionary<Type, List<PDSection>>();

        /// <summary>
        /// Set when we need to do a rebuild, but we are not visible, to queue up a rebuild.
        /// </summary>
        private bool refreshNeeded = false;

        private LicenseStatus licenseStatus;

        private List<PDSection> sections;

        public LicenseManagerLauncher LicenseLauncher { private get; set; }

        public GeneralTabPage()
        {
            InitializeComponent();

            VM_guest_metrics_CollectionChangedWithInvoke =
                Program.ProgramInvokeHandler(VM_guest_metrics_CollectionChanged);
            OtherConfigAndTagsWatcher.TagsChanged += OtherConfigAndTagsWatcher_TagsChanged;
            sections = new List<PDSection>();
            foreach (Control control in panel2.Controls)
            {
                Panel p = control as Panel;
                if (p == null)
                    continue;

                foreach (Control c in p.Controls)
                {
                    PDSection s = c as PDSection;
                    if (s == null)
                        continue;
                    sections.Add(s);
                    s.MaximumSize = new Size(900, 9999999);
                    s.fixFirstColumnWidth(150);
                    s.contentChangedSelection += s_contentChangedSelection;
                    s.contentReceivedFocus += s_contentReceivedFocus;
                }
            }            
        }

        private void licenseStatus_ItemUpdated(object sender, EventArgs e)
        {
            if (pdSectionLicense == null || licenseStatus == null)
                return;

            GeneralTabLicenseStatusStringifier ss = new GeneralTabLicenseStatusStringifier(licenseStatus);
            Program.Invoke(Program.MainWindow, () => 
            {
                pdSectionLicense.UpdateEntryValueWithKey(
                    FriendlyName("host.license_params-expiry"),
                    ss.ExpiryDate, 
                    ss.ShowExpiryDate);
            });

            Program.Invoke(Program.MainWindow, () =>
            {
                pdSectionLicense.UpdateEntryValueWithKey(
                    Messages.LICENSE_STATUS,
                    ss.ExpiryStatus,
                    true);
            });

            Pool p = xenObject as Pool;
            if (p != null)
                Program.Invoke(Program.MainWindow, () =>
                {
                    var additionalString = PoolAdditionalLicenseString();
                    pdSectionGeneral.UpdateEntryValueWithKey(
                        Messages.POOL_LICENSE,
                        additionalString != string.Empty
                            ? string.Format(Messages.MAINWINDOW_CONTEXT_REASON, p.LicenseString(), additionalString)
                            : p.LicenseString(),
                        true);
                });
        }

        void s_contentReceivedFocus(PDSection s)
        {
            scrollToSelectionIfNeeded(s);
        }

        void s_contentChangedSelection(PDSection s)
        {
            scrollToSelectionIfNeeded(s);
        }

        private void scrollToSelectionIfNeeded(PDSection s)
        {
            if (s.HasNoSelection())
                return;

            Rectangle selectedRowBounds = s.SelectedRowBounds;

            // translate to the coordinates of the pdsection container panel (the one added for padding purposes)
            selectedRowBounds.Offset(s.Parent.Location);

            // Top edge visible?
            if (panel2.ClientRectangle.Height - selectedRowBounds.Top > 0 && selectedRowBounds.Top > 0)
            {
                // Bottom edge visible?
                if (panel2.ClientRectangle.Height - selectedRowBounds.Bottom > 0 && selectedRowBounds.Bottom > 0)
                {
                    // The entire selected row is in view, no need to move 
                    return;
                }
            }

            panel2.ForceScrollTo(s);
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            new PropertiesCommand(Program.MainWindow, xenObject).Execute();
        }

        private IXenObject xenObject;
        public IXenObject XenObject
        {
            set
            {
                if (value == null)
                    return;

                RegisterLicenseStatusUpdater(value);

                if (xenObject != value)
                {
                    UnregisterHandlers();

                    xenObject = value;
                    RegisterHandlers();
                    BuildList();
                    List<PDSection> listPDSections = null;
                    if (_expandedSections.TryGetValue(xenObject.GetType(), out listPDSections))
                        ResetExpandState(listPDSections);
                    else
                        ResetExpandState();
                }
                else
                {
                    BuildList();
                }
            }
        }

        private void RegisterLicenseStatusUpdater(IXenObject xenObject)
        {
            if (licenseStatus != null)
                licenseStatus.ItemUpdated -= licenseStatus_ItemUpdated;

            if (xenObject.Connection != null && !xenObject.Connection.IsConnected)
                return;

            if (xenObject is Host || xenObject is Pool)
            {
                licenseStatus = new LicenseStatus(xenObject);
                licenseStatus.ItemUpdated += licenseStatus_ItemUpdated;
                licenseStatus.BeginUpdate();
            }
        }

        void s_ExpandedEventHandler(PDSection pdSection)
        {
            if (pdSection != null)
            {
                //Add to the expandedSections
                List<PDSection> listSections;
                if (_expandedSections.TryGetValue(xenObject.GetType(), out listSections))
                {
                    if (!listSections.Contains(pdSection) && pdSection.IsExpanded)
                        listSections.Add(pdSection);
                    else if (!pdSection.IsExpanded)
                        listSections.Remove(pdSection);
                }
                else if (pdSection.IsExpanded)
                {
                    List<PDSection> list = new List<PDSection>();
                    list.Add(pdSection);
                    _expandedSections.Add(xenObject.GetType(), list);
                }
            }
            SetStatesOfExpandingLinks();
        }

        private void ResetExpandState()
        {
            panel2.SuspendLayout();
            foreach (PDSection s in sections)
            {
                s.Contract();
            }
            pdSectionGeneral.Expand();
            panel2.ResumeLayout();
        }
        private void ResetExpandState(List<PDSection> expandedSections)
        {
            panel2.SuspendLayout();
            foreach (PDSection s in sections)
            {
                if (expandedSections.Contains(s))
                    s.Expand();
                else
                    s.Contract();
            }
            pdSectionGeneral.Expand();
            panel2.ResumeLayout();
        }

        private void UnregisterHandlers()
        {
            if (xenObject != null)
                xenObject.PropertyChanged -= PropertyChanged;

            if (xenObject is Host)
            {
                Host host = xenObject as Host;

                Host_metrics metric = xenObject.Connection.Resolve<Host_metrics>(host.metrics);
                if (metric != null)
                    metric.PropertyChanged -= PropertyChanged;
            }
            else if (xenObject is VM)
            {
                VM vm = xenObject as VM;

                VM_metrics metric = vm.Connection.Resolve(vm.metrics);
                if (metric != null)
                    metric.PropertyChanged -= PropertyChanged;

                VM_guest_metrics guestmetric = xenObject.Connection.Resolve(vm.guest_metrics);
                if (guestmetric != null)
                    guestmetric.PropertyChanged -= PropertyChanged;

                vm.Connection.Cache.DeregisterCollectionChanged<VM_guest_metrics>(VM_guest_metrics_CollectionChangedWithInvoke);
            }
            else if (xenObject is SR)
            {
                SR sr = xenObject as SR;

                foreach (PBD pbd in sr.Connection.ResolveAll(sr.PBDs))
                {
                    pbd.PropertyChanged -= PropertyChanged;
                }
            }
            else if (xenObject is Pool)
            {
                xenObject.Connection.Cache.DeregisterBatchCollectionChanged<Pool_patch>(Pool_patch_BatchCollectionChanged);
                xenObject.Connection.Cache.DeregisterBatchCollectionChanged<Pool_update>(Pool_update_BatchCollectionChanged);
            }
        }

        void VM_guest_metrics_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (!this.Visible)
                return;
            // Required to refresh the panel when the vm boots so we show the correct pv driver state and version
            // Note this does NOT get called every 2s, just when the vm power state changes (hopefully!)
            BuildList();
        }

        private readonly CollectionChangeEventHandler VM_guest_metrics_CollectionChangedWithInvoke;
        private void RegisterHandlers()
        {
            if (xenObject != null)
                xenObject.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);

            if (xenObject is Host)
            {
                Host host = xenObject as Host;
                Host_metrics metric = xenObject.Connection.Resolve(host.metrics);
                if (metric != null)
                    metric.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);
            }
            else if (xenObject is VM)
            {
                VM vm = xenObject as VM;

                VM_metrics metric = vm.Connection.Resolve(vm.metrics);
                if (metric != null)
                    metric.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);

                VM_guest_metrics guestmetric = xenObject.Connection.Resolve(vm.guest_metrics);
                if (guestmetric != null)
                    guestmetric.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);

                xenObject.Connection.Cache.RegisterCollectionChanged<VM_guest_metrics>(VM_guest_metrics_CollectionChangedWithInvoke);
            }
            else if (xenObject is Pool)
            {
                xenObject.Connection.Cache.RegisterBatchCollectionChanged<Pool_patch>(Pool_patch_BatchCollectionChanged);
                xenObject.Connection.Cache.RegisterBatchCollectionChanged<Pool_update>(Pool_update_BatchCollectionChanged);
            }
        }

        void Pool_patch_BatchCollectionChanged(object sender, EventArgs e)
        {
            Program.BeginInvoke(this, BuildList);
        }

        void Pool_update_BatchCollectionChanged(object sender, EventArgs e)
        {
            Program.BeginInvoke(this, BuildList);
        }

        void OtherConfigAndTagsWatcher_TagsChanged()
        {
            BuildList();
        }

        // We queue up a rebuild if we are not shown but the contents becomes out of date, this just fires off the rebuild
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible && refreshNeeded)
            {
                BuildList();
                refreshNeeded = false;
            }
            base.OnVisibleChanged(e);
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "state" ||
                e.PropertyName == "last_updated")
            {
                return;
            }

            Program.Invoke(this, delegate
            {
                if (e.PropertyName == "PBDs")
                {
                    SR sr = xenObject as SR;
                    if (sr == null)
                        return;

                    foreach (PBD pbd in xenObject.Connection.ResolveAll(sr.PBDs))
                    {
                        pbd.PropertyChanged -= PropertyChanged;
                        pbd.PropertyChanged += PropertyChanged;
                    }

                    BuildList();
                }
                else
                {
                    // Atm we are rebuilding on almost any property changed event. 
                    // As long as we are just clearing and readding the rows in the PDSections this seems to be super quick. 
                    // If it gets slower we should update specific boxes for specific property changes.
                    if (licenseStatus != null && licenseStatus.Updated)
                        licenseStatus.BeginUpdate();
                    BuildList();
                    EnableDisableEdit();
                }
            });
        }

        public void EnableDisableEdit()
        {
            buttonProperties.Enabled = xenObject != null && !xenObject.Locked && xenObject.Connection != null && xenObject.Connection.IsConnected;

            //keeping it separate
            if (xenObject is DockerContainer)
            {
                buttonProperties.Enabled = false;

                DockerContainer container = (DockerContainer)xenObject;
                buttonViewConsole.Visible = true;
                buttonViewLog.Visible = true;

                // Grey out the buttons if the Container management VM is Windows.
                // For Linux VM, enable the buttons only when the docker is running.
                if (container.Parent.IsWindows())
                {
                    buttonViewConsole.Enabled = false;
                    buttonViewLog.Enabled = false;
                }
                else
                {
                    buttonViewConsole.Enabled = buttonViewLog.Enabled = container.power_state == vm_power_state.Running;
                }
            }
            else
            {
                buttonViewConsole.Visible = false;
                buttonViewLog.Visible = false;
            }

        }

        public void BuildList()
        {
            //Program.AssertOnEventThread();

            if (!this.Visible)
            {
                refreshNeeded = true;
                return;
            }
            if (xenObject == null)
                return;

            if (xenObject is Host && !xenObject.Connection.IsConnected)
                base.Text = Messages.CONNECTION_GENERAL_TAB_TITLE;
            else if (xenObject is Host)
            {
                base.Text = Messages.HOST_GENERAL_TAB_TITLE;
            }
                
                
            else if (xenObject is VM)
            {
                VM vm = (VM)xenObject;
                if (vm.is_a_snapshot)
                    base.Text = Messages.SNAPSHOT_GENERAL_TAB_TITLE;
                else if (vm.is_a_template)
                    base.Text = Messages.TEMPLATE_GENERAL_TAB_TITLE;
                else
                    base.Text = Messages.VM_GENERAL_TAB_TITLE;
            }
            else if (xenObject is SR)
                base.Text = Messages.SR_GENERAL_TAB_TITLE;
            else if (xenObject is Pool)
                base.Text = Messages.POOL_GENERAL_TAB_TITLE;
            else if (xenObject is DockerContainer)
                base.Text = Messages.CONTAINER_GENERAL_TAB_TITLE;

            panel2.SuspendLayout();
            // Clear all the data from the sections (visible and non visible)
            foreach (PDSection s in sections)
            {
                s.PauseLayout();
                s.ClearData();
            }
            // Generate the content of each box, each method performs a cast and only populates if XenObject is the relevant type
           
            if (xenObject is Host && (xenObject.Connection == null || !xenObject.Connection.IsConnected))
            {
                generateDisconnectedHostBox();
            }
            else if (xenObject is DockerContainer)
            {
                generateDockerContainerGeneralBox();
            }
            else
            {
                generateGeneralBox();
                generateCustomFieldsBox();
                generateInterfaceBox();
                generateMemoryBox();
                generateVersionBox();
                generateLicenseBox();
                generateCPUBox();
                generateHostPatchesBox();
                generateBootBox();
                generateHABox();
                generateStatusBox();
                generateMultipathBox();
                generatePoolPatchesBox();
                generateMultipathBootBox();
                generateVCPUsBox();
                generateDockerInfoBox();
                generateReadCachingBox();
            }

            // hide all the sections which haven't been populated, those that have make sure are visible
            foreach (PDSection s in sections)
            {
                if (s.IsEmpty())
                {
                    s.Parent.Visible = false;
                }
                else
                {
                    s.Parent.Visible = true;
                    if (s.ContainsFocus)
                        s.RestoreSelection();
                }
                s.StartLayout();
            }
            panel2.ResumeLayout();
            EnableDisableEdit();
        }

        private void generateInterfaceBox()
        {
            Host Host = xenObject as Host;
            Pool Pool = xenObject as Pool;
            if (Host != null)
            {
                fillInterfacesForHost(Host, false);
            }
            else if (Pool != null)
            {
                // Here we tell fillInterfacesForHost to prefix each entry with the hosts name label, so we know which entry belongs to which host
                // and also to better preserve uniqueness for keys in the PDSection

                foreach (Host h in Pool.Connection.Cache.Hosts)
                    fillInterfacesForHost(h, true);
            }
        }

        private void fillInterfacesForHost(Host Host, bool includeHostSuffix)
        {
            PDSection s = pdSectionManagementInterfaces;

            ToolStripMenuItem editValue = new ToolStripMenuItem(Messages.EDIT) { Image = Properties.Resources.edit_16 };
            editValue.Click += delegate
                {
                    NetworkingProperties p = new NetworkingProperties(Host, null);
                    p.ShowDialog(Program.MainWindow);
                };

            if (!string.IsNullOrEmpty(Host.hostname))
            {
                if (!includeHostSuffix)
                    s.AddEntry(FriendlyName("host.hostname"), Host.hostname, editValue);
                else
                    s.AddEntry(
                        string.Format(Messages.PROPERTY_ON_OBJECT, FriendlyName("host.hostname"), Helpers.GetName(Host)),
                        Host.hostname,
                        editValue);
            }
            foreach (PIF pif in xenObject.Connection.ResolveAll<PIF>(Host.PIFs))
            {
                if (pif.management)
                {
                    if (!includeHostSuffix)
                        s.AddEntry(Messages.MANAGEMENT_INTERFACE, pif.FriendlyIPAddress(), editValue);
                    else
                        s.AddEntry(
                            string.Format(Messages.PROPERTY_ON_OBJECT, Messages.MANAGEMENT_INTERFACE, Helpers.GetName(Host)),
                            pif.FriendlyIPAddress(),
                            editValue);
                }
            }

            foreach (PIF pif in xenObject.Connection.ResolveAll<PIF>(Host.PIFs))
            {
                if (pif.IsSecondaryManagementInterface(Properties.Settings.Default.ShowHiddenVMs))
                {
                    if (!includeHostSuffix)
                        s.AddEntry(pif.GetManagementPurpose().Ellipsise(30), pif.FriendlyIPAddress(), editValue);
                    else
                        s.AddEntry(
                            string.Format(Messages.PROPERTY_ON_OBJECT, pif.GetManagementPurpose().Ellipsise(30), Helpers.GetName(Host)),
                            pif.FriendlyIPAddress(),
                            editValue);
                }
            }
        }

        private void generateCustomFieldsBox()
        {
            List<CustomField> customFields = CustomFieldsManager.CustomFieldValues(xenObject);
            if (customFields.Count <= 0)
                return;

            PDSection s = pdSectionCustomFields;

            foreach (CustomField customField in customFields)
            {
                ToolStripMenuItem editValue = new ToolStripMenuItem(Messages.EDIT){Image= Properties.Resources.edit_16};
                editValue.Click += delegate
                    {
                        using (PropertiesDialog dialog = new PropertiesDialog(xenObject))
                        {
                            dialog.SelectCustomFieldsEditPage();
                            dialog.ShowDialog();
                        }
                    };

                var menuItems = new[] { editValue };
                CustomFieldWrapper cfWrapper = new CustomFieldWrapper(xenObject, customField.Definition);

                s.AddEntry(customField.Definition.Name.Ellipsise(30), cfWrapper.ToString(), menuItems, customField.Definition.Name);
            }
        }

        private void generatePoolPatchesBox()
        {
            Pool pool = xenObject as Pool;
            if (pool == null)
                return;

            PDSection s = pdSectionUpdates;

            List<KeyValuePair<String, String>> messages = CheckPoolUpdate(pool);
            if (messages.Count > 0)
            {
                foreach (KeyValuePair<String, String> kvp in messages)
                {
                    s.AddEntry(kvp.Key, kvp.Value);
                }
            }
            Host master = Helpers.GetMaster(xenObject.Connection);
            if (master == null)
                return;

            var poolAppPatches = poolAppliedPatches();
            if (!string.IsNullOrEmpty(poolAppPatches))
            {
                s.AddEntry(FriendlyName("Pool_patch.fully_applied"), poolAppPatches);
            }

            var poolPartPatches = poolPartialPatches();
            if (!string.IsNullOrEmpty(poolPartPatches))
            {
                CommandToolStripMenuItem applypatch = new CommandToolStripMenuItem(
                    new InstallNewUpdateCommand(Program.MainWindow), true);
                var menuItems = new[] { applypatch };

                s.AddEntry(FriendlyName("Pool_patch.partially_applied"), poolPartPatches, menuItems, Color.Red);
            }
        }

        private void generateHostPatchesBox()
        {
            Host host = xenObject as Host;
            if (host == null)
                return;

            PDSection s = pdSectionUpdates;
            List<KeyValuePair<String, String>> messages;

            bool elyOrGreater = Helpers.ElyOrGreater(host);

            if (elyOrGreater)
            {
                // As of Ely we use host.updates_requiring_reboot to generate the list of reboot required messages
                messages = CheckHostUpdatesRequiringReboot(host);
            }
            else
            {
                // For older versions no change to how messages are generated
                messages = CheckServerUpdates(host);
            }

            if (messages.Count > 0)
            {
                foreach (KeyValuePair<String, String> kvp in messages)
                {
                    s.AddEntry(kvp.Key, kvp.Value);
                }
            }

            var appliedPatchesList = Helpers.HostAppliedPatchesList(host);
            var appliedPatches = string.Join(Environment.NewLine, appliedPatchesList.ToArray());
            if (!string.IsNullOrEmpty(appliedPatches))
            {
                s.AddEntry(FriendlyName("Pool_patch.applied"), appliedPatches);
            }

            var recommendedPatches = RecommendedPatchesForHost(host);
            if (!string.IsNullOrEmpty(recommendedPatches))
            {
                s.AddEntry(FriendlyName("Pool_patch.required-updates"), recommendedPatches);
            }

            if (!elyOrGreater)
            {
                // add supplemental packs
                var suppPacks = hostInstalledSuppPacks(host);
                if (!string.IsNullOrEmpty(suppPacks))
                {
                    s.AddEntry(FriendlyName("Supplemental_packs.installed"), suppPacks);
                }
            }
        }

        private void generateHABox()
        {
            VM vm = xenObject as VM;
            if (vm == null)
                return;

            Pool pool = Helpers.GetPoolOfOne(xenObject.Connection);
            if (pool == null || !pool.ha_enabled)
                return;

            PDSection s = pdSectionHighAvailability;

            s.AddEntry(FriendlyName("VM.ha_restart_priority"), Helpers.RestartPriorityI18n(vm.HARestartPriority()),
                new PropertiesToolStripMenuItem(new VmEditHaCommand(Program.MainWindow, xenObject)));
        }

        private void generateStatusBox()
        {
            SR sr = xenObject as SR;
            if (sr == null)
                return;

            PDSection s = pdSectionStatus;

            bool broken = sr.IsBroken() || !sr.MultipathAOK();
            bool detached = !sr.HasPBDs();

            ToolStripMenuItem repair = new ToolStripMenuItem
                {
                    Text = Messages.GENERAL_SR_CONTEXT_REPAIR,
                    Image = Properties.Resources._000_StorageBroken_h32bit_16
                };
            repair.Click += delegate
                {
                    Program.MainWindow.ShowPerConnectionWizard(xenObject.Connection, new RepairSRDialog(sr));
                };

            var menuItems = new[] { repair };

            if (broken && !detached)
                s.AddEntry(FriendlyName("SR.state"), sr.StatusString(), menuItems);
            else
                s.AddEntry(FriendlyName("SR.state"), sr.StatusString());

            foreach (Host host in xenObject.Connection.Cache.Hosts)
            {
                PBD pbdToSR = null;
                foreach (PBD pbd in xenObject.Connection.ResolveAll(host.PBDs))
                {
                    if (pbd.SR.opaque_ref != xenObject.opaque_ref)
                        continue;

                    pbdToSR = pbd;
                    break;
                }
                if (pbdToSR == null)
                {
                    if (!sr.shared)
                        continue;

                    if (!detached)
                        s.AddEntry("  " + Helpers.GetName(host).Ellipsise(30),
                            Messages.REPAIR_SR_DIALOG_CONNECTION_MISSING, menuItems, Color.Red);
                    else
                        s.AddEntry("  " + Helpers.GetName(host).Ellipsise(30),
                            Messages.REPAIR_SR_DIALOG_CONNECTION_MISSING, Color.Red);

                    continue;
                }

                pbdToSR.PropertyChanged -= new PropertyChangedEventHandler(PropertyChanged);
                pbdToSR.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);

                if (!pbdToSR.currently_attached)
                {
                    if (!detached)
                        s.AddEntry(Helpers.GetName(host).Ellipsise(30), pbdToSR.StatusString(), menuItems, Color.Red);
                    else
                        s.AddEntry(Helpers.GetName(host).Ellipsise(30), pbdToSR.StatusString(), Color.Red);
                }
                else
                {
                    s.AddEntry(Helpers.GetName(host).Ellipsise(30), pbdToSR.StatusString());
                }
            }
        }

        private void generateMultipathBox()
        {
            SR sr = xenObject as SR;
            if (sr == null)
                return;

            PDSection s = pdSectionMultipathing;

            if (!sr.MultipathCapable())
            {
                s.AddEntry(Messages.MULTIPATH_CAPABLE, Messages.NO);
                return;
            }

            if (sr.LunPerVDI())
            {
                Dictionary<VM, Dictionary<VDI, String>>
                    pathStatus = sr.GetMultiPathStatusLunPerVDI();

                foreach (Host host in xenObject.Connection.Cache.Hosts)
                {
                    PBD pbd = sr.GetPBDFor(host);
                    if (pbd == null || !pbd.MultipathActive())
                    {
                        s.AddEntry(host.Name(), Messages.MULTIPATH_NOT_ACTIVE);
                        continue;
                    }

                    s.AddEntry(host.Name(), Messages.MULTIPATH_ACTIVE);
                    foreach (KeyValuePair<VM, Dictionary<VDI, String>> kvp in pathStatus)
                    {
                        VM vm = kvp.Key;
                        if (vm.resident_on == null ||
                            vm.resident_on.opaque_ref != host.opaque_ref)
                            continue;

                        bool renderOnOneLine = false;
                        int lastMax = -1;
                        int lastCurrent = -1;

                        foreach (KeyValuePair<VDI, String> kvp2 in kvp.Value)
                        {
                            int current;
                            int max;
                            if (!PBD.ParsePathCounts(kvp2.Value, out current, out max))
                                continue;

                            if (!renderOnOneLine)
                            {
                                lastMax = max;
                                lastCurrent = current;
                                renderOnOneLine = true;
                                continue;
                            }

                            if (lastMax == max && lastCurrent == current)
                                continue;

                            renderOnOneLine = false;
                            break;
                        }

                        if (renderOnOneLine)
                        {
                            AddMultipathLine(s, String.Format("    {0}", vm.Name()),
                                             lastCurrent, lastMax, pbd.ISCSISessions());
                        }
                        else
                        {
                            s.AddEntry(String.Format("    {0}", vm.Name()), "");

                            foreach (KeyValuePair<VDI, String> kvp2 in kvp.Value)
                            {
                                int current;
                                int max;
                                if (!PBD.ParsePathCounts(kvp2.Value, out current, out max))
                                    continue;

                                AddMultipathLine(s, String.Format("        {0}", kvp2.Key.Name()),
                                                current, max, pbd.ISCSISessions());
                            }
                        }
                    }
                }
            }
            else
            {
                Dictionary<PBD, String> pathStatus = sr.GetMultiPathStatusLunPerSR();

                foreach (Host host in xenObject.Connection.Cache.Hosts)
                {
                    PBD pbd = sr.GetPBDFor(host);
                    if (pbd == null || !pathStatus.ContainsKey(pbd))
                    {
                        s.AddEntry(host.Name(),
                            pbd != null && pbd.MultipathActive()
                                ? Messages.MULTIPATH_ACTIVE
                                : Messages.MULTIPATH_NOT_ACTIVE);
                        continue;
                    }

                    String status = pathStatus[pbd];

                    int current;
                    int max;
                    PBD.ParsePathCounts(status, out current, out max); //Guaranteed to work if PBD is in pathStatus

                    AddMultipathLine(s, host.Name(), current, max, pbd.ISCSISessions());
                }
            }
        }

        private void AddMultipathLine(PDSection s, String title, int current, int max, int iscsiSessions)
        {
            bool bad = current < max || (iscsiSessions != -1 && max < iscsiSessions);
            string row = string.Format(Messages.MULTIPATH_STATUS, current, max);
            if (iscsiSessions != -1)
                row += string.Format(Messages.MULTIPATH_STATUS_ISCSI_SESSIONS, iscsiSessions);

            if (bad)
                s.AddEntry(title, row, Color.Red);
            else
                s.AddEntry(title, row);
        }

        private void generateMultipathBootBox()
        {
            Host host = xenObject as Host;
            if (host == null)
                return;

            int current, max;
            if (!host.GetBootPathCounts(out current, out max))
                return;

            PDSection s = pdSectionMultipathBoot;
            string text = string.Format(Messages.MULTIPATH_STATUS, current, max);
            bool bad = current < max;
            if (bad)
                s.AddEntry(Messages.STATUS, text, Color.Red);
            else
                s.AddEntry(Messages.STATUS, text);
        }

        private void generateBootBox()
        {
            VM vm = xenObject as VM;
            if (vm == null)
                return;

            PDSection s = pdSectionBootOptions;

        	if (vm.IsHVM())
            {	
                s.AddEntry(FriendlyName("VM.BootOrder"), HVMBootOrder(vm),
                   new PropertiesToolStripMenuItem(new VmEditStartupOptionsCommand(Program.MainWindow, vm)));
            }
            else
            {
                s.AddEntry(FriendlyName("VM.PV_args"), vm.PV_args,
                    new PropertiesToolStripMenuItem(new VmEditStartupOptionsCommand(Program.MainWindow, vm)));
            }
        }

        private void generateLicenseBox()
        {
            Host host = xenObject as Host;
            if (host == null)
                return;

            PDSection s = pdSectionLicense;

            if (host.license_params == null)
                return;

            Dictionary<string, string> info = new Dictionary<string, string>(host.license_params);

            // This field is now supressed as it has no meaning under the current license scheme, and was never
            // enforced anyway.
            info.Remove("sockets");

            // Remove "expiry" field for "basic" license
            if (!string.IsNullOrEmpty(host.edition) && host.edition == "basic")
                info.Remove("expiry");

            if (info.ContainsKey("expiry"))
            {
                ToolStripMenuItem editItem = new ToolStripMenuItem(Messages.LAUNCH_LICENSE_MANAGER);
                editItem.Click += delegate
                    {
                        if (LicenseLauncher != null)
                        {
                            LicenseLauncher.Parent = Program.MainWindow;
                            LicenseLauncher.LaunchIfRequired(false, ConnectionsManager.XenConnections);
                        }
                    };

                GeneralTabLicenseStatusStringifier ss = new GeneralTabLicenseStatusStringifier(licenseStatus);
                s.AddEntry(Messages.LICENSE_STATUS, ss.ExpiryStatus, editItem);
                s.AddEntry(FriendlyName("host.license_params-expiry"), ss.ExpiryDate, editItem, ss.ShowExpiryDate);
                info.Remove("expiry");
            }

            if (!string.IsNullOrEmpty(host.edition))
            {
                s.AddEntry(FriendlyName("host.edition"), Helpers.GetFriendlyLicenseName(host));
            }

            s.AddEntry(Messages.NUMBER_OF_SOCKETS, host.CpuSockets().ToString());

            if (host.license_server.ContainsKey("address"))
            {
                var licenseServerAddress = host.license_server["address"].Trim();
                if (licenseServerAddress == "" || licenseServerAddress.ToLower() == "localhost")
                    s.AddEntry(FriendlyName(String.Format("host.license_server-address")), host.license_server["address"]);
                else
                {
                    var openUrl = new ToolStripMenuItem(Messages.LICENSE_SERVER_WEB_CONSOLE_GOTO);
                    openUrl.Click += (sender, args) => Program.OpenURL(string.Format(Messages.LICENSE_SERVER_WEB_CONSOLE_FORMAT, licenseServerAddress, Host.LicenseServerWebConsolePort));
                    s.AddEntryLink(FriendlyName(String.Format("host.license_server-address")),
                                   host.license_server["address"],
                                   new[] {openUrl},
                                   openUrl.PerformClick);
                }
            }
            if (host.license_server.ContainsKey("port"))
            {
                s.AddEntry(FriendlyName(String.Format("host.license_server-port")), host.license_server["port"]);
            }

            foreach (string key in new string[] { "productcode", "serialnumber" })
            {
                if (info.ContainsKey(key))
                {
                    string row_name = string.Format("host.license_params-{0}", key);
                    string k = key;
                    if (host.license_params[k] != string.Empty)
                        s.AddEntry(FriendlyName(row_name), host.license_params[k]);
                    info.Remove(key);
                }
            }

            string restrictions = Helpers.GetHostRestrictions(host);
            if (restrictions != "")
            {
                s.AddEntry(Messages.RESTRICTIONS, restrictions);
            }
        }

        private void generateVersionBox()
        {
            Host host = xenObject as Host;

            if (host == null || host.software_version == null)
                return;

            if (host.software_version.ContainsKey("date"))
                pdSectionVersion.AddEntry(Messages.SOFTWARE_VERSION_DATE, host.software_version["date"]);
            if (!Helpers.ElyOrGreater(host) && host.software_version.ContainsKey("build_number"))
                pdSectionVersion.AddEntry(Messages.SOFTWARE_VERSION_BUILD_NUMBER, host.software_version["build_number"]);
            if (host.software_version.ContainsKey("product_version"))
                pdSectionVersion.AddEntry(Messages.SOFTWARE_VERSION_PRODUCT_VERSION, host.ProductVersionText());
            if (host.software_version.ContainsKey("dbv"))
                pdSectionVersion.AddEntry("DBV", host.software_version["dbv"]);
        }

        private void generateCPUBox()
        {
            Host host = xenObject as Host;
            if (host == null)
                return;

            PDSection s = pdSectionCPU;

            SortedDictionary<long, Host_cpu> d = new SortedDictionary<long, Host_cpu>();
            foreach (Host_cpu cpu in xenObject.Connection.ResolveAll(host.host_CPUs))
            {
                d.Add(cpu.number, cpu);
            }

            bool cpusIdentical = CPUsIdentical(d.Values);

            foreach (Host_cpu cpu in d.Values)
            {
                String label = String.Format(Messages.GENERAL_DETAILS_CPU_NUMBER,
                    cpusIdentical && d.Values.Count > 1 ? String.Format("0 - {0}", d.Values.Count - 1)
                        : cpu.number.ToString());

                s.AddEntry(label, Helpers.GetCPUProperties(cpu));
                if (cpusIdentical)
                    break;
            }
        }

        private void generateVCPUsBox()
        {
            VM vm = xenObject as VM;
            if (vm == null)
                return;

            PDSection s = pdSectionVCPUs; 
            
            s.AddEntry(FriendlyName("VM.VCPUs"), vm.VCPUs_at_startup.ToString());
            if (vm.VCPUs_at_startup != vm.VCPUs_max || vm.SupportsVcpuHotplug())
                s.AddEntry(FriendlyName("VM.MaxVCPUs"), vm.VCPUs_max.ToString());
            s.AddEntry(FriendlyName("VM.Topology"), vm.Topology());
        }

        private void generateDisconnectedHostBox()
        {
            IXenConnection conn = xenObject.Connection;

            PDSection s = pdSectionGeneral;

            string name = Helpers.GetName(xenObject);
            s.AddEntry(FriendlyName("host.name_label"), name);
            if (conn != null && conn.Hostname != name)
                s.AddEntry(FriendlyName("host.address"), conn.Hostname);

            if (conn != null && conn.PoolMembers.Count > 1)
                s.AddEntry(FriendlyName("host.pool_members"), string.Join(", ", conn.PoolMembers.ToArray()));

        }

        private void generateGeneralBox()
        {
            PDSection s = pdSectionGeneral;

            s.AddEntry(FriendlyName("host.name_label"), Helpers.GetName(xenObject),
                new PropertiesToolStripMenuItem(new PropertiesCommand(Program.MainWindow, xenObject)));

            VM vm = xenObject as VM;
            if (vm == null || vm.DescriptionType() != VM.VmDescriptionType.None)
            {
                s.AddEntry(FriendlyName("host.name_description"), xenObject.Description(),
                            new PropertiesToolStripMenuItem(new DescriptionPropertiesCommand(Program.MainWindow, xenObject)));
            }

            GenTagRow(s);
            GenFolderRow(s);

            Host host = xenObject as Host;
            if (host != null)
            {
                if (Helpers.GetPool(xenObject.Connection) != null)
                    s.AddEntry(Messages.POOL_MASTER, host.IsMaster() ? Messages.YES : Messages.NO);

                if (!host.IsLive())
                {
                    s.AddEntry(FriendlyName("host.enabled"), Messages.HOST_NOT_LIVE, Color.Red);
                }
                else if (!host.enabled)
                {
                    var item = new ToolStripMenuItem(Messages.EXIT_MAINTENANCE_MODE);
                    item.Click += delegate
                        {
                            new HostMaintenanceModeCommand(Program.MainWindow, host,
                                                           HostMaintenanceModeCommandParameter.Exit).Execute();
                        };
                    s.AddEntry(FriendlyName("host.enabled"),
                               host.MaintenanceMode() ? Messages.HOST_IN_MAINTENANCE_MODE : Messages.DISABLED,
                               new[] { item },
                               Color.Red);
                }
                else
                {
                    var item = new ToolStripMenuItem(Messages.ENTER_MAINTENANCE_MODE);
                    item.Click += delegate
                        {
                            new HostMaintenanceModeCommand(Program.MainWindow, host,
                                HostMaintenanceModeCommandParameter.Enter).Execute();
                        };
                    s.AddEntry(FriendlyName("host.enabled"), Messages.YES, item);
                }

                s.AddEntry(FriendlyName("host.iscsi_iqn"), host.GetIscsiIqn(),
                    new PropertiesToolStripMenuItem(new IqnPropertiesCommand(Program.MainWindow, xenObject)));
                s.AddEntry(FriendlyName("host.log_destination"), host.GetSysLogDestination() ?? Messages.HOST_LOG_DESTINATION_LOCAL,
                   new PropertiesToolStripMenuItem(new HostEditLogDestinationCommand(Program.MainWindow, xenObject)));

                PrettyTimeSpan uptime = host.Uptime();
                PrettyTimeSpan agentUptime = host.AgentUptime();
                s.AddEntry(FriendlyName("host.uptime"), uptime == null ? "" : uptime.ToString());
                s.AddEntry(FriendlyName("host.agentUptime"), agentUptime == null ? "" : agentUptime.ToString());

                if (host.external_auth_type == Auth.AUTH_TYPE_AD)
                    s.AddEntry(FriendlyName("host.external_auth_service_name"), host.external_auth_service_name);
            }

            if (vm != null)
            {
                s.AddEntry(FriendlyName("VM.OSName"), vm.GetOSName());

                s.AddEntry(FriendlyName("VM.OperatingMode"), vm.IsHVM() ? Messages.VM_OPERATING_MODE_HVM : Messages.VM_OPERATING_MODE_PV);

                if (!vm.DefaultTemplate())
                {
                    s.AddEntry(Messages.BIOS_STRINGS_COPIED, vm.BiosStringsCopied() ? Messages.YES : Messages.NO);
                }

				if (vm.Connection != null)
				{
					var appl = vm.Connection.Resolve(vm.appliance);
					if (appl != null)
					{
                        var applProperties = new ToolStripMenuItem(Messages.VM_APPLIANCE_PROPERTIES);
					    applProperties.Click +=
					        (sender, e) =>
					            {
					                using (PropertiesDialog propertiesDialog = new PropertiesDialog(appl))
					                    propertiesDialog.ShowDialog(this);
					            };

						s.AddEntryLink(Messages.VM_APPLIANCE, appl.Name(), new[] { applProperties },
									   () =>
									   {
										   using (PropertiesDialog propertiesDialog = new PropertiesDialog(appl))
											   propertiesDialog.ShowDialog(this);
									   });
					}
				}

            	if (vm.is_a_snapshot)
                {
                    VM snapshotOf = vm.Connection.Resolve(vm.snapshot_of);
                    s.AddEntry(Messages.SNAPSHOT_OF, snapshotOf == null ? string.Empty : snapshotOf.Name());
                    s.AddEntry(Messages.CREATION_TIME, HelpersGUI.DateTimeToString(vm.snapshot_time.ToLocalTime() + vm.Connection.ServerTimeOffset, Messages.DATEFORMAT_DMY_HMS, true));
                }

                if (!vm.is_a_template)
                {
                    GenerateVirtualisationStatusForGeneralBox(s, vm);

                    var runningTime = vm.RunningTime();
                    if (runningTime != null)
                        s.AddEntry(FriendlyName("VM.uptime"), runningTime.ToString());

                    if (vm.IsP2V())
                    {
                        s.AddEntry(FriendlyName("VM.P2V_SourceMachine"), vm.P2V_SourceMachine());
                        s.AddEntry(FriendlyName("VM.P2V_ImportDate"), HelpersGUI.DateTimeToString(vm.P2V_ImportDate().ToLocalTime(), Messages.DATEFORMAT_DMY_HMS, true));
                    }

                    // Dont show if WLB is enabled.
                    if (VMCanChooseHomeServer(vm))
                    {
                        s.AddEntry(FriendlyName("VM.affinity"), vm.AffinityServerString(),
                            new PropertiesToolStripMenuItem(new VmEditHomeServerCommand(Program.MainWindow, xenObject)));
                    }
                }
            }

            SR sr = xenObject as SR;
            if (sr != null)
            {
                s.AddEntry(Messages.TYPE, sr.FriendlyTypeName());

                if (sr.content_type != SR.Content_Type_ISO && sr.GetSRType(false) != SR.SRTypes.udev)
                    s.AddEntry(FriendlyName("SR.size"), sr.SizeString());

                if (sr.GetScsiID() != null)
                    s.AddEntry(FriendlyName("SR.scsiid"), sr.GetScsiID() ?? Messages.UNKNOWN);

                // if in folder-view or if looking at SR on storagelink then display
                // location here
                if (Program.MainWindow.SelectionManager.Selection.HostAncestor == null && Program.MainWindow.SelectionManager.Selection.PoolAncestor == null)
                {
                    IXenObject belongsTo = Helpers.GetPool(sr.Connection);

                    if (belongsTo != null)
                    {
                        s.AddEntry(Messages.POOL, Helpers.GetName(belongsTo));
                    }
                    else
                    {
                        belongsTo = Helpers.GetMaster(sr.Connection);

                        if (belongsTo != null)
                        {
                            s.AddEntry(Messages.SERVER, Helpers.GetName(belongsTo));
                        }
                    }
                }
            }

            Pool p = xenObject as Pool;
            if (p != null)
            {
                var additionalString = PoolAdditionalLicenseString();
                s.AddEntry(Messages.POOL_LICENSE,
                    additionalString != string.Empty
                        ? string.Format(Messages.MAINWINDOW_CONTEXT_REASON, p.LicenseString(), additionalString)
                        : p.LicenseString());
                s.AddEntry(Messages.NUMBER_OF_SOCKETS, p.CpuSockets().ToString());

                var master = p.Connection.Resolve(p.master);
                if (master != null)
                {
                    if (p.IsPoolFullyUpgraded())
                    {
                        s.AddEntry(Messages.SOFTWARE_VERSION_PRODUCT_VERSION, master.ProductVersionText());
                    }
                    else
                    {
                        var cmd = new RollingUpgradeCommand(Program.MainWindow);
                        var runRpuWizard = new ToolStripMenuItem(Messages.ROLLING_POOL_UPGRADE_ELLIPSIS,
                            null,
                            (sender, args) => cmd.Execute());

                        s.AddEntryLink(Messages.SOFTWARE_VERSION_PRODUCT_VERSION,
                            string.Format(Messages.POOL_VERSIONS_LINK_TEXT, master.ProductVersionText()),
                            new[] {runRpuWizard},
                            cmd);
                    }
                }
            }

            VDI vdi = xenObject as VDI;
            if (vdi != null)
            {
                s.AddEntry(Messages.SIZE, vdi.SizeText(),
                    new PropertiesToolStripMenuItem(new VdiEditSizeLocationCommand(Program.MainWindow, xenObject)));

                SR vdiSr = vdi.Connection.Resolve(vdi.SR);
                if (vdiSr != null && !vdiSr.IsToolsSR())
                    s.AddEntry(Messages.DATATYPE_STORAGE, vdiSr.NameWithLocation());

                string vdiVms = vdi.VMsOfVDI();
                if (!string.IsNullOrEmpty(vdiVms))
                    s.AddEntry(Messages.VIRTUAL_MACHINE, vdiVms);
            }

            s.AddEntry(FriendlyName("host.uuid"), GetUUID(xenObject));
        }

        private string PoolAdditionalLicenseString()
        {
            if (licenseStatus.CurrentState == LicenseStatus.HostState.Expired)
                return Messages.LICENSE_EXPIRED;
            else if (licenseStatus.CurrentState == LicenseStatus.HostState.Free)
                return Messages.LICENSE_UNLICENSED;
            else   
                return string.Empty;
        }

        private static void GenerateVirtualisationStatusForGeneralBox(PDSection s, VM vm)
        {
            if (vm != null && vm.Connection != null)
            {
                //For Dundee or higher Windows VMs
                if (Helpers.DundeeOrGreater(vm.Connection) && vm.IsWindows())
                {
                    var gm = vm.Connection.Resolve(vm.guest_metrics);

                    bool isIoOptimized = gm != null && gm.PV_drivers_detected;
                    bool isManagementAgentInstalled = vm.GetVirtualisationStatus().HasFlag(VM.VirtualisationStatus.MANAGEMENT_INSTALLED);
                    bool canInstallIoDriversAndManagementAgent = InstallToolsCommand.CanExecute(vm) && !isIoOptimized;
                    bool canInstallManagementAgentOnly = InstallToolsCommand.CanExecute(vm) && isIoOptimized && !isManagementAgentInstalled;
                    //canInstallIoDriversOnly is missing - management agent communicates with XS using the I/O drivers

                    var sb = new StringBuilder();

                    if (vm.power_state == vm_power_state.Running)
                    {
                        if (vm.GetVirtualisationStatus().HasFlag(XenAPI.VM.VirtualisationStatus.UNKNOWN))
                        {
                            sb.AppendLine(vm.VirtualisationStatusString());
                        }
                        else
                        {
                            //Row 1 : I/O Drivers
                            if (isIoOptimized)
                            {
                                sb.Append(Messages.VIRTUALIZATION_STATE_VM_IO_OPTIMIZED);
                            }
                            else
                            {
                                sb.Append(Messages.VIRTUALIZATION_STATE_VM_IO_NOT_OPTIMIZED);
                            }

                            sb.Append(Environment.NewLine);

                            //Row 2: Management Agent
                            if (isManagementAgentInstalled)
                            {
                                sb.Append(Messages.VIRTUALIZATION_STATE_VM_MANAGEMENT_AGENT_INSTALLED);
                            }
                            else
                            {
                                sb.Append(Messages.VIRTUALIZATION_STATE_VM_MANAGEMENT_AGENT_NOT_INSTALLED);
                            }
                            sb.Append(Environment.NewLine);
                        }
                    }

                    //Row 3 : vendor device - Windows Update
                    if(!HiddenFeatures.WindowsUpdateHidden)
                        sb.Append(vm.has_vendor_device ? Messages.VIRTUALIZATION_STATE_VM_RECEIVING_UPDATES : Messages.VIRTUALIZATION_STATE_VM_NOT_RECEIVING_UPDATES);

                    // displaying Row1 - Row3
                    s.AddEntry(FriendlyName("VM.VirtualizationState"), sb.ToString());

                    if (vm.power_state == vm_power_state.Running)
                    {
                        //Row 4: Install Tools
                        string installMessage = string.Empty;
                        if (canInstallIoDriversAndManagementAgent)
                        {
                            installMessage = Messages.VIRTUALIZATION_STATE_VM_INSTALL_IO_DRIVERS_AND_MANAGEMENT_AGENT;
                        }
                        else if (canInstallManagementAgentOnly)
                        {
                            installMessage = Messages.VIRTUALIZATION_STATE_VM_INSTALL_MANAGEMENT_AGENT;
                        }

                        if (!string.IsNullOrEmpty(installMessage))
                        {
                            var installtools = new ToolStripMenuItem(installMessage);
                            installtools.Click += delegate
                            {
                                new InstallToolsCommand(Program.MainWindow, vm).Execute();
                            };
                            s.AddEntryLink(string.Empty, installMessage,
                                new[] { installtools },
                                new InstallToolsCommand(Program.MainWindow, vm));
                        }
                    }
                }

                //for everything else (All VMs on pre-Dundee hosts & All non-Windows VMs on any host)
                else if (vm.power_state == vm_power_state.Running)
                {
                    var status = vm.GetVirtualisationStatus();
                    if (status == 0 || status.HasFlag(XenAPI.VM.VirtualisationStatus.PV_DRIVERS_OUT_OF_DATE))
                    {
                        if (InstallToolsCommand.CanExecute(vm))
                        {
                            var installtools = new ToolStripMenuItem(Messages.INSTALL_XENSERVER_TOOLS_DOTS);
                            installtools.Click += delegate
                            {
                                new InstallToolsCommand(Program.MainWindow, vm).Execute();
                            };
                            s.AddEntryLink(FriendlyName("VM.VirtualizationState"), vm.VirtualisationStatusString(),
                                new[] { installtools },
                                new InstallToolsCommand(Program.MainWindow, vm));
                        }
                        else
                        {
                            s.AddEntry(FriendlyName("VM.VirtualizationState"), vm.VirtualisationStatusString(), Color.Red);
                        }

                    }
                    else
                    {
                        s.AddEntry(FriendlyName("VM.VirtualizationState"), vm.VirtualisationStatusString());
                    }
                }
            }
        }

        private void generateDockerContainerGeneralBox()
        {
            if (xenObject is DockerContainer)
            {
                PDSection s = pdSectionGeneral;
                DockerContainer dockerContainer = (DockerContainer)xenObject;
                s.AddEntry(Messages.NAME, dockerContainer.Name().Length != 0 ? dockerContainer.Name() : Messages.NONE);
                s.AddEntry(Messages.STATUS, dockerContainer.status.Length != 0 ? dockerContainer.status : Messages.NONE);
                try
                {
                    DateTime created = Util.FromUnixTime(double.Parse(dockerContainer.created)).ToLocalTime();
                    s.AddEntry(Messages.CONTAINER_CREATED, HelpersGUI.DateTimeToString(created, Messages.DATEFORMAT_DMY_HMS, true));
                }
                catch
                {
                    s.AddEntry(Messages.CONTAINER_CREATED, dockerContainer.created.Length != 0 ? dockerContainer.created : Messages.NONE);
                }
                s.AddEntry(Messages.CONTAINER_IMAGE, dockerContainer.image.Length != 0 ? dockerContainer.image : Messages.NONE);
                s.AddEntry(Messages.CONTAINER, dockerContainer.container.Length != 0 ? dockerContainer.container : Messages.NONE);
                s.AddEntry(Messages.CONTAINER_COMMAND, dockerContainer.command.Length != 0 ? dockerContainer.command : Messages.NONE);
                var ports = dockerContainer.PortList.Select(p => p.Description);
                if (ports.Count() > 0)
                {
                    s.AddEntry(Messages.CONTAINER_PORTS, string.Join(Environment.NewLine, ports));
                }
                s.AddEntry(Messages.UUID, dockerContainer.uuid.Length != 0 ? dockerContainer.uuid : Messages.NONE);
            }
        }

        private void generateReadCachingBox()
        {
            VM vm = xenObject as VM;
            if (vm == null || !vm.IsRunning() || !Helpers.CreamOrGreater(vm.Connection))
                return;

            PDSection s = pdSectionReadCaching;

            var pvsProxy = vm.PvsProxy();
            if (pvsProxy != null)
                s.AddEntry(FriendlyName("VM.pvs_read_caching_status"), pvs_proxy_status_extensions.ToFriendlyString(pvsProxy.status));
            else if (vm.ReadCachingEnabled())
            {
                s.AddEntry(FriendlyName("VM.read_caching_status"), Messages.VM_READ_CACHING_ENABLED);
                var vdiList = vm.ReadCachingVDIs().Select(vdi => vdi.NameWithLocation()).ToArray();
                s.AddEntry(FriendlyName("VM.read_caching_disks"), string.Join("\n", vdiList));
            }
            else
            {
                s.AddEntry(FriendlyName("VM.read_caching_status"), Messages.VM_READ_CACHING_DISABLED);
                var reason = vm.ReadCachingDisabledReason();
                if (reason != null)
                    s.AddEntry(FriendlyName("VM.read_caching_reason"), reason);
            }
        }

        private static bool VMCanChooseHomeServer(VM vm)
        {
            if (vm != null && !vm.is_a_template)
            {
                String ChangeHomeReason = vm.IsOnSharedStorage();

                return !Helpers.WlbEnabledAndConfigured(vm.Connection) &&
                    (String.IsNullOrEmpty(ChangeHomeReason) || vm.HasNoDisksAndNoLocalCD());
            }
            return false;
        }


        private void GenTagRow(PDSection s)
        {
            string[] tags = Tags.GetTags(xenObject);
            
            if (tags != null && tags.Length > 0)
            {
                ToolStripMenuItem goToTag = new ToolStripMenuItem(Messages.VIEW_TAG_MENU_OPTION);

                foreach (string tag in tags)
                {
                    var item = new ToolStripMenuItem(tag.Ellipsise(30));
                    item.Click += delegate { Program.MainWindow.SearchForTag(tag); };
                    goToTag.DropDownItems.Add(item);
                }

                s.AddEntry(Messages.TAGS, TagsString(), new[] { goToTag, new PropertiesToolStripMenuItem(new PropertiesCommand(Program.MainWindow, xenObject)) });
                return;
            }

            s.AddEntry(Messages.TAGS, Messages.NONE, new PropertiesToolStripMenuItem(new PropertiesCommand(Program.MainWindow, xenObject)));
        }

        private string TagsString()
        {
            string[] tags = Tags.GetTags(xenObject);
            if (tags == null || tags.Length == 0)
                return Messages.NONE;

            return string.Join(", ", tags.OrderBy(s => s).ToArray());
        }

        private void GenFolderRow(PDSection s)
        {
            List<ToolStripMenuItem> menuItems = new List<ToolStripMenuItem>();
            if (xenObject.Path != "")
            {
                var item = new ToolStripMenuItem(Messages.VIEW_FOLDER_MENU_OPTION);
                item.Click += delegate { Program.MainWindow.SearchForFolder(xenObject.Path); };
                menuItems.Add(item);
            }
            menuItems.Add(new PropertiesToolStripMenuItem(new PropertiesCommand(Program.MainWindow, xenObject)));
            s.AddEntry(
                Messages.FOLDER,
                new FolderListItem(xenObject.Path, FolderListItem.AllowSearch.None, false),
                menuItems
                );
        }

        private void generateMemoryBox()
        {
            Host host = xenObject as Host;
            if (host == null)
                return;

            PDSection s = pdSectionMemory;

      
            s.AddEntry(FriendlyName("host.ServerMemory"), host.HostMemoryString());
            s.AddEntry(FriendlyName("host.VMMemory"), host.ResidentVMMemoryUsageString());
            s.AddEntry(FriendlyName("host.XenMemory"), host.XenMemoryString());
            
        }

        private void addStringEntry(PDSection s, string key, string value)
        {
            s.AddEntry(key, string.IsNullOrEmpty(value) ? Messages.NONE : value);
        }

        private void generateDockerInfoBox()
        {
            VM vm = xenObject as VM;
            if (vm == null)
                return;

            VM_Docker_Info info = vm.DockerInfo();
            if (info == null)
                return;

            VM_Docker_Version version = vm.DockerVersion();
            if (version == null)
                return;

            PDSection s = pdSectionDockerInfo;

            addStringEntry(s, Messages.DOCKER_INFO_API_VERSION, version.ApiVersion);
            addStringEntry(s, Messages.DOCKER_INFO_VERSION, version.Version);
            addStringEntry(s, Messages.DOCKER_INFO_GIT_COMMIT, version.GitCommit);
            addStringEntry(s, Messages.DOCKER_INFO_DRIVER, info.Driver);
            addStringEntry(s, Messages.DOCKER_INFO_INDEX_SERVER_ADDRESS, info.IndexServerAddress);
            addStringEntry(s, Messages.DOCKER_INFO_EXECUTION_DRIVER, info.ExecutionDriver);
            addStringEntry(s, Messages.DOCKER_INFO_IPV4_FORWARDING, info.IPv4Forwarding);
        }

        private bool CPUsIdentical(IEnumerable<Host_cpu> cpus)
        {
            String cpuText = null;
            foreach (Host_cpu cpu in cpus)
            {
                if (cpuText == null)
                {
                    cpuText = Helpers.GetCPUProperties(cpu);
                    continue;
                }
                if (Helpers.GetCPUProperties(cpu) != cpuText)
                    return false;
            }
            return true;
        }

        private string RecommendedPatchesForHost(Host host)
        {
            var result = new List<string>();
            var recommendedPatches = Updates.RecommendedPatchesForHost(host);

            if (recommendedPatches == null)
                return String.Empty;

            foreach (var patch in recommendedPatches)
                result.Add(patch.Name);

            return string.Join(Environment.NewLine, result.ToArray());
        }

        private string hostUnappliedPatches(Host host)
        {
            List<string> result = new List<string>();

            foreach (Pool_patch patch in Pool_patch.GetAllThatApply(host, ConnectionsManager.XenConnectionsCopy))
            {
                if (!patch.AppliedTo(ConnectionsManager.XenConnectionsCopy).Contains(new XenRef<Host>(xenObject.opaque_ref)))
                    result.Add(patch.Name());
            }

            result.Sort(StringUtility.NaturalCompare);
            return string.Join("\n", result.ToArray());
        }

        private string hostInstalledSuppPacks(Host host)
        {
            var result = host.SuppPacks().Select(suppPack => suppPack.LongDescription).ToList();
            result.Sort(StringUtility.NaturalCompare);
            return string.Join("\n", result.ToArray());
        }

        #region VM delegates

        private static string HVMBootOrder(VM vm)
        {
            var order = vm.GetBootOrder().ToUpper().Union(new[] { 'D', 'C', 'N' });
            return string.Join("\n", order.Select(c => new BootDevice(c).ToString()).ToArray());
        }

        #endregion

        #region Pool delegates

        private string poolAppliedPatches()
        {
            return 
                Helpers.ElyOrGreater(xenObject.Connection)
                ? poolUpdateString(update => update.AppliedOnHosts().Count == xenObject.Connection.Cache.HostCount)
                : poolPatchString(patch => patch.host_patches.Count == xenObject.Connection.Cache.HostCount);
        }

        private string poolPartialPatches()
        {
            return Helpers.ElyOrGreater(xenObject.Connection)
                ? poolUpdateString(update =>
                {
                    var appliedOnHosts = update.AppliedOnHosts();
                    return appliedOnHosts.Count > 0 && appliedOnHosts.Count != xenObject.Connection.Cache.HostCount;
                })
                : poolPatchString(patch => patch.host_patches.Count > 0 && patch.host_patches.Count != xenObject.Connection.Cache.HostCount);
        }

        private string poolPatchString(Predicate<Pool_patch> predicate)
        {
            Pool_patch[] patches = xenObject.Connection.Cache.Pool_patches;

            List<String> output = new List<String>();

            foreach (Pool_patch patch in patches)
                if (predicate(patch))
                    output.Add(patch.name_label);

            output.Sort(StringUtility.NaturalCompare);

            return String.Join(", ", output.ToArray());
        }

        private string poolUpdateString(Predicate<Pool_update> predicate)
        {
            Pool_update[] updates = xenObject.Connection.Cache.Pool_updates;

            List<String> output = new List<String>();

            foreach (var update in updates)
                if (predicate(update))
                    output.Add(Helpers.UpdatesFriendlyNameAndVersion(update));

            output.Sort(StringUtility.NaturalCompare);

            return String.Join(", ", output.ToArray());
        }

        #endregion

        /// <summary>
        /// Checks for reboot warnings on all hosts in the pool and returns them as a list
        /// </summary>
        /// <param name="pool"></param>
        /// <returns></returns>
        private List<KeyValuePair<String, String>> CheckPoolUpdate(Pool pool)
        {
            List<KeyValuePair<String, String>> warnings = new List<KeyValuePair<string, string>>();

            if (Helpers.ElyOrGreater(pool.Connection))
            {
                // As of Ely we use CheckHostUpdatesRequiringReboot to get reboot messages for a host
                foreach (Host host in xenObject.Connection.Cache.Hosts)
                {
                    warnings.AddRange(CheckHostUpdatesRequiringReboot(host));
                }
            }
            else
            {
                // Earlier versions use the old server updates method
                foreach (Host host in xenObject.Connection.Cache.Hosts)
                {
                    warnings.AddRange(CheckServerUpdates(host));
                }
            }
            return warnings;
        }

        /// <summary>
        /// Checks the server has been restarted after any patches that require a restart were applied and returns a list of reboot warnings
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        private List<KeyValuePair<String, String>> CheckServerUpdates(Host host)
        {
            List<Pool_patch> patches = host.AppliedPatches();
            List<KeyValuePair<String, String>> warnings = new List<KeyValuePair<String, String>>();
            double bootTime = host.BootTime();
            double agentStart = host.AgentStartTime();

            if (bootTime == 0.0 || agentStart == 0.0)
                return warnings;

            foreach (Pool_patch patch in patches)
            {
                double applyTime = Util.ToUnixTime(patch.AppliedOn(host));

                if (patch.after_apply_guidance.Contains(after_apply_guidance.restartHost) && applyTime > bootTime
                    || patch.after_apply_guidance.Contains(after_apply_guidance.restartXAPI) && applyTime > agentStart)
                {
                    warnings.Add(CreateWarningRow(host, patch));
                }
            }
            return warnings;
        }

        /// <summary>
        /// Creates a list of warnings for updates that require the host to be rebooted
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        private List<KeyValuePair<String, String>> CheckHostUpdatesRequiringReboot(Host host)
        {
            var warnings = new List<KeyValuePair<String, String>>();
            
            // Updates that require host restart
            var updateRefs = host.updates_requiring_reboot;
            foreach (var updateRef in updateRefs)
            {
                var update = host.Connection.Resolve(updateRef);
                warnings.Add(CreateWarningRow(host, update));
            }

            // For Toolstack restart, legacy code has to be used to determine this - pool_patches are still populated for backward compatibility
            List<Pool_patch> patches = host.AppliedPatches();
            double bootTime = host.BootTime();
            double agentStart = host.AgentStartTime();

            if (bootTime == 0.0 || agentStart == 0.0)
                return warnings;

            foreach (Pool_patch patch in patches)
            {
                double applyTime = Util.ToUnixTime(patch.AppliedOn(host));

                if (patch.after_apply_guidance.Contains(after_apply_guidance.restartXAPI)
                    && applyTime > agentStart)
                {
                    warnings.Add(CreateWarningRow(host, patch));
                }
            }

            return warnings;
        }

        private KeyValuePair<string, string> CreateWarningRow(Host host, Pool_patch patch)
        {
            var key = String.Format(Messages.GENERAL_PANEL_UPDATE_KEY, patch.Name(), host.Name());
            string value = string.Empty;

            if (patch.after_apply_guidance.Contains(after_apply_guidance.restartHost))
            {
                value = string.Format(Messages.GENERAL_PANEL_UPDATE_REBOOT_WARNING, host.Name(), patch.Name());
            }
            else if (patch.after_apply_guidance.Contains(after_apply_guidance.restartXAPI))
            {
                value = string.Format(Messages.GENERAL_PANEL_UPDATE_RESTART_TOOLSTACK_WARNING, host.Name(), patch.Name());
            }
            
            return new KeyValuePair<string, string>(key, value);
        }

        private KeyValuePair<string, string> CreateWarningRow(Host host, Pool_update update)
        {
            var key = String.Format(Messages.GENERAL_PANEL_UPDATE_KEY, Helpers.UpdatesFriendlyName(update.Name()), host.Name());
            var value = string.Format(Messages.GENERAL_PANEL_UPDATE_REBOOT_WARNING, host.Name(), Helpers.UpdatesFriendlyName(update.Name()));

            return new KeyValuePair<string, string>(key, value);
        }
 
        private static string GetUUID(IXenObject o)
        {
            return o.Get("uuid") as String;
        }

        private static string FriendlyName(string propertyName)
        {
            return Core.PropertyManager.GetFriendlyName(string.Format("Label-{0}", propertyName)) ?? propertyName;
        }

        private void linkLabelExpand_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (PDSection s in sections)
            {
                if (!s.Parent.Visible)
                    continue;

                s.DisableFocusEvent = true;
                s.Expand();
                s.DisableFocusEvent = false;
            }

            linkLabelCollapse.Focus();
        }

        private void linkLabelCollapse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
            foreach (PDSection s in sections)
            {
                if (!s.Parent.Visible)
                    continue;

                s.DisableFocusEvent = true;
                s.Contract();
                s.DisableFocusEvent = false;
            }

            linkLabelExpand.Focus();
        }

        private void SetStatesOfExpandingLinks()
        {
            var sectionsVisible = sections.Where(section => section.Parent.Visible);
            bool anyExpanded = sectionsVisible.Any(s => s.IsExpanded);
            bool anyCollapsed = sectionsVisible.Any(s => !s.IsExpanded);
            linkLabelExpand.Enabled = anyCollapsed;
            linkLabelCollapse.Enabled = anyExpanded;
        }

        private void buttonViewConsole_Click(object sender, EventArgs e)
        {
            if (xenObject is DockerContainer)
            {
                DockerContainer dockerContainer = (DockerContainer)xenObject;
                string vmIp = dockerContainer.Parent.IPAddressForSSH();
                //Set command 'docker attach' to attach to the container.
                string dockerCmd = "env docker attach --sig-proxy=false " + dockerContainer.uuid;
                startPutty(dockerCmd, vmIp);
            }
        }

        private void buttonViewLog_Click(object sender, EventArgs e)
        {
            if (xenObject is DockerContainer)
            {
                DockerContainer dockerContainer = (DockerContainer)xenObject;
                string vmIp = dockerContainer.Parent.IPAddressForSSH();
                //Set command 'docker logs' to retrieve the logs of the container.
                string dockerCmd = "env docker logs --tail=50 --follow --timestamps " + dockerContainer.uuid;
                startPutty(dockerCmd, vmIp);
            }
        }

        private void startPutty(string command, string ipAddr)
        {
            try
            {
                //Write docker command to a temp file.
                string cmdFile = Path.Combine(Path.GetTempPath(), "ContainerManagementCommand.txt");
                File.WriteAllText(cmdFile, command);

                //Invoke Putty, SSH to VM and execute docker command.
                var puttyPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "putty.exe");
                string args = "-m " + cmdFile + " -t " + ipAddr;

                //Specify the key for SSH connection.
                var keyFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "ContainerManagement.ppk");
                if (File.Exists(keyFile))
                {
                    args = args + " -i " + keyFile;
                }
                var startInfo = new ProcessStartInfo(puttyPath, args);

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                log.Error("Error starting PuTTY.", ex);
                using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Error, Messages.ERROR_PUTTY_LAUNCHING, Messages.XENCENTER)))
                {
                    dlg.ShowDialog(Parent);
                }
            }
        }
    }
}
