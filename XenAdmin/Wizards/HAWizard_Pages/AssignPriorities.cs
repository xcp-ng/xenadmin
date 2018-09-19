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
using System.Linq;
using System.Windows.Forms;

using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Properties;
using XenAPI;


namespace XenAdmin.Wizards.HAWizard_Pages
{
    public partial class AssignPriorities : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IXenConnection connection;

        private readonly CollectionChangeEventHandler VM_CollectionChangedWithInvoke;
        private readonly List<VM> _vmsQueuedForUpdate = new List<VM>();

        /// <summary>
        /// May not be set to null.
        /// </summary>
        public new IXenConnection Connection
        {
            private get { return connection; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                if (connection != null)
                    DeregisterEvents();

                connection = value;
                RegisterEvents();
            }
        }

        internal void PopulatePageControls()
        {
            Debug.Assert(connection != null, "Connection is null; set it to non-null before calling this method.");

            UpdateMenuItems();
            PopulateVMs();
            haNtolIndicator.Connection = Connection;
            haNtolIndicator.Settings = getCurrentSettings();
            StartNtolUpdate();
        }

        private void UpdateMenuItems()
        {
            List<VM.HA_Restart_Priority> restartPriorities = VM.GetAvailableRestartPriorities(connection);

            //When this line: m_dropDownButtonRestartPriority.ContextMenuStrip = this.contextMenuStrip
            //was called in the designer a "dummy" item was added to the contextMenuStrip by the m_dropDownButtonRestartPriority,
            //however here it is cleared so no need to remove it explicitly later when contextMenuStrip.Items is called
            contextMenuStrip.Items.Clear();

            foreach (var restartPriority in restartPriorities)
            {
                var menuItem = contextMenuStrip.Items.Add(Helpers.RestartPriorityI18n(restartPriority));
                menuItem.Tag = restartPriority;
                menuItem.Click += priority_Click;
            }
        }

        /// <summary>
        /// Called when the current IXenConnection's VM dictionary changes.
        /// </summary>
        private void VM_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.AssertOnEventThread();

            var vm = e.Element as VM;
            if (vm == null)
                return;

            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    vm.PropertyChanged -= vm_PropertyChanged;
                    vm.PropertyChanged += vm_PropertyChanged;
                    AddVmRow(vm);
                    break;
                case CollectionChangeAction.Remove:
                    vm.PropertyChanged -= vm_PropertyChanged;
                    RemoveVmRow(vm);
                    break;
            }
        }

        /// <summary>
        /// The current (uncommitted) VM restart priorities.
        /// </summary>
        public Dictionary<VM, VM.HA_Restart_Priority> CurrentSettings
        {
            get { return haNtolIndicator.Settings; }
        }

        /// <summary>
        /// The Ntol from the HaNtolIndicator control contained within.
        /// </summary>
        public long Ntol
        {
            get
            {
                return haNtolIndicator.Ntol;
            }
        }

        /// <summary>
        /// Whether the user has made any changes to VM restart priorities on the server.
        /// </summary>
        public bool ChangesMade { get; private set; }

        public AssignPriorities()
        {
            InitializeComponent();
            VM_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(VM_CollectionChanged);

            haNtolIndicator.UpdateInProgressChanged += haNtolIndicator_UpdateInProgressChanged;

            nudStartDelay.Maximum = long.MaxValue;
            nudOrder.Maximum = long.MaxValue;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            DeregisterEvents();

            if (disposing && (components != null))
            {
                haNtolIndicator.UpdateInProgressChanged -= haNtolIndicator_UpdateInProgressChanged;
                components.Dispose();
            }

            StopNtolUpdate();

            base.Dispose(disposing);
        }

        private void RegisterEvents()
        {
            // Add listeners
            connection.Cache.RegisterCollectionChanged<VM>(VM_CollectionChangedWithInvoke);

            foreach (VM vm in connection.Cache.VMs)
            {
                vm.PropertyChanged -= vm_PropertyChanged;
                vm.PropertyChanged += vm_PropertyChanged;
            }
        }

        private void DeregisterEvents()
        {
            // Remove listeners
            connection.Cache.DeregisterCollectionChanged<VM>(VM_CollectionChangedWithInvoke);

            foreach (VM vm in connection.Cache.VMs)
                vm.PropertyChanged -= vm_PropertyChanged;
        }

        /// <summary>
        /// Sets all agile VMs to state 'Protected' and all non-agile VMs to 'Restart if possible'.
        /// Important: call before setting the Connection property.
        /// </summary>
        internal bool ProtectVmsByDefault { get; set; }

        private void StartNtolUpdate()
        {
            haNtolIndicator.StartNtolUpdate();
        }

        internal void StopNtolUpdate()
        {
            haNtolIndicator.StopNtolUpdate();
        }

        /// <summary>
        /// Must be called on the event thread.
        /// </summary>
        private void PopulateVMs()
        {
            Program.AssertOnEventThread();

            try
            {
                dataGridViewVms.SuspendLayout();
                dataGridViewVms.Rows.Clear();

                var newRows = new List<DataGridViewRow>();
                var vms = connection.Cache.VMs.Where(v => v.HaCanProtect(Properties.Settings.Default.ShowHiddenVMs));

                // see if HA is being activated for the first time.
                bool firstTime = IsHaActivatedFirstTime(vms);

                foreach (VM vm in connection.Cache.VMs)
                {
                    if (!vm.HaCanProtect(Properties.Settings.Default.ShowHiddenVMs))
                        continue;

                    // Create a new row for this VM.
                    // The priority for this row is either initially null (which means 'fill in the restart priority with
                    // Protected/Restart if possible when we've determined if the VM is agile'), or its current restart priority.
                    // The first case is for the HA wizard when priorities are being configured for the first time,
                    // the second is for the edit dialog, when HA is already enabled.
                    
                    VM.HA_Restart_Priority? priority = firstTime ? (VM.HA_Restart_Priority?)null : vm.HARestartPriority();
                    var row = new VmWithSettingsRow(vm, priority);
                    newRows.Add(row);
                }

                dataGridViewVms.Rows.AddRange(newRows.ToArray());
                var addedVms = from row in dataGridViewVms.Rows.Cast<VmWithSettingsRow>() select row.Vm;
                UpdateVMsAgility(addedVms.ToList());
            }
            finally
            {
                dataGridViewVms.ResumeLayout();
            }
        }

        /// <summary>
        /// Starts a new background thread that updates the displayed agility status for each VM.
        /// </summary>
        private void UpdateVMsAgility(List<VM> vms)
        {
            if (bgWorker.IsBusy)
            {
                foreach (var vm in vms)
                {
                    if (!_vmsQueuedForUpdate.Contains(vm))
                        _vmsQueuedForUpdate.Add(vm);
                }
                return;
            }

            Debug.Assert(connection != null, "Connection is null; set it to non-null before calling this method.");
            if (vms.Count > 0)
                bgWorker.RunWorkerAsync(vms);
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var vms = e.Argument as IEnumerable<VM>;
            Debug.Assert(vms != null);

            Session session = connection.DuplicateSession();
            var results = new Dictionary<VM, string>();

            foreach (VM vm in vms)
            {
                try
                {
                    VM.assert_agile(session, vm.opaque_ref);
                    results[vm] = null;
                }
                catch (Failure failure)//The VM wasn't agile
                {
                    results[vm] = failure.ErrorDescription[0];
                }
            }

            e.Result = results;
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                log.Error(e.Error);
                return;
            }

            var result = e.Result as Dictionary<VM, string>;
            Debug.Assert(result != null);

            foreach (var pair in result)
            {
                VM vm = pair.Key;
                string nonAgileReason = pair.Value;
                bool isNowAgile = nonAgileReason == null;

                //worker started on main thread => this event handler will be on
                //main thread too, so no need to invoke

                VmWithSettingsRow row = findItemFromVM(vm);
                if (row == null)
                    return;

                // We previously had no restart priority assigned => protectVmsByDefault.
                // Now we know whether the VM is agile, so we can assign it the highest protection available.
                if (row.RestartPriority == null)
                {
                    Debug.Assert(ProtectVmsByDefault);

                    var priority = isNowAgile ? VM.HaHighestProtectionAvailable(connection) : VM.HA_Restart_Priority.BestEffort;

                    row.UpdateRestartPriority(priority);
                    haNtolIndicator.Settings = getCurrentSettings();
                }
                else if (!isNowAgile
                        && row.RestartPriority != VM.HA_Restart_Priority.BestEffort
                        && row.RestartPriority != VM.HA_Restart_Priority.DoNotRestart)
                {
                    row.UpdateRestartPriority(VM.HA_Restart_Priority.BestEffort);
                    haNtolIndicator.Settings = getCurrentSettings();
                }

                row.UpdateAgile(isNowAgile);
                row.FriendlyNonAgileReason = nonAgileReason;
            }
            updateButtons();

            if (_vmsQueuedForUpdate.Count > 0)
            {
                var vms = new List<VM>(_vmsQueuedForUpdate);
                _vmsQueuedForUpdate.Clear();
                UpdateVMsAgility(vms);
            }
        }
        
        private bool IsHaActivatedFirstTime(IEnumerable<VM> vms)
        {
            return ProtectVmsByDefault && vms.All(v => string.IsNullOrEmpty(v.ha_restart_priority));
        }

        private void AddVmRow(VM vm)
        {
            Program.AssertOnEventThread();

            if (!vm.HaCanProtect(Properties.Settings.Default.ShowHiddenVMs))
                return;

            // see if HA is being activated for the first time
            var vms = connection.Cache.VMs.Where(v => v.HaCanProtect(Properties.Settings.Default.ShowHiddenVMs));
            bool firstTime = IsHaActivatedFirstTime(vms);

            VM.HA_Restart_Priority? priority = firstTime ? (VM.HA_Restart_Priority?)null : vm.HARestartPriority();
            var row = new VmWithSettingsRow(vm, priority);
            dataGridViewVms.Rows.Add(row);
            UpdateVMsAgility(new List<VM> {vm});
        }

        private void RemoveVmRow(VM vm)
        {
            Program.AssertOnEventThread();

            var row = findItemFromVM(vm);
            if (row != null)
                dataGridViewVms.Rows.Remove(row);
        }

        /// <summary>
        /// Finds the ListViewItem for the given VM. Will return null if no corresponding item could be found.
        /// Must be called on the event thread.
        /// </summary>
        private VmWithSettingsRow findItemFromVM(VM vm)
        {
            Program.AssertOnEventThread();
            return dataGridViewVms.Rows.Cast<VmWithSettingsRow>().FirstOrDefault(r => r.Vm == vm);
        }

        private void vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = sender as VM;
            if (vm == null)
                return;

            Program.Invoke(this, () =>
            {
                var row = findItemFromVM(vm);

                try
                {
                    dataGridViewVms.SuspendLayout();

                    if (row == null)
                        AddVmRow(vm);
                    else
                    {
                        row.UpdateVm(vm);
                        row.SetAgileCalculating();
                        UpdateVMsAgility(new List<VM> {vm});
                    }
                }
                finally
                {
                    dataGridViewVms.ResumeLayout();
                }
            });
        }

        private void haNtolIndicator_UpdateInProgressChanged(object sender, EventArgs e)
        {
            // Signal to anyone who cares that this page is good to continue, or not.
            OnPageUpdated();

            if (!haNtolIndicator.UpdateInProgress)
            {
                pictureBoxStatus.Visible = true;

                if (haNtolIndicator.Ntol == -1)
                {
                    labelHaStatus.Text = Messages.HA_UNABLE_TO_CALCULATE_MESSAGE;
                    labelHaStatus.ForeColor = Color.Red;
                    pictureBoxStatus.Image = Resources._000_Alert2_h32bit_16;
                    return;
                }

                if (haNtolIndicator.Overcommitted || haNtolIndicator.Ntol == 0)
                {
                    labelHaStatus.Text = Messages.HA_OVERCOMMIT_MESSAGE;
                    labelHaStatus.ForeColor = Color.Red;
                    pictureBoxStatus.Image = Resources._000_Alert2_h32bit_16;
                    return;
                }

                labelHaStatus.Text = string.Format(Messages.HA_GUARANTEED_MESSAGE, haNtolIndicator.NtolMax);
                labelHaStatus.ForeColor = SystemColors.ControlText;
                pictureBoxStatus.Image = Resources._000_Tick_h32bit_16;
            }
        }

        private void dataGridViewVms_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                try
                {
                    dataGridViewVms.SuspendLayout();
                    dataGridViewVms.SelectAll();
                }
                finally
                {
                    dataGridViewVms.ResumeLayout();
                }
            }
        }

        /// <summary>
        /// Called when the user clicks one of the restart priorities in the context menu.
        /// </summary>
        private void priority_Click(object sender, EventArgs e)
        {
            var menuitem = (ToolStripMenuItem)sender;
            VM.HA_Restart_Priority pri = (VM.HA_Restart_Priority)menuitem.Tag;

            bool changesMade = false;
            foreach (var row in dataGridViewVms.SelectedRows.Cast<VmWithSettingsRow>())
            {             
                if (row.RestartPriority != pri)
                {
                    changesMade = true;
                    row.UpdateRestartPriority(pri);
                }
            }

            if (changesMade)
            {
                ChangesMade = true;
                haNtolIndicator.Settings = getCurrentSettings();
                // Will trigger a ntol update under this revised plan
                updateButtons();
            }
        }

        /// <summary>
        /// Gets the current (uncommitted) VM restart priorities. Must be called on the GUI thread.
        /// </summary>
        private Dictionary<VM, VM.HA_Restart_Priority> getCurrentSettings()
        {
            Program.AssertOnEventThread();
            Dictionary<VM, VM.HA_Restart_Priority> result = new Dictionary<VM, VM.HA_Restart_Priority>();

            foreach (var row in dataGridViewVms.Rows.Cast<VmWithSettingsRow>())
            {
                // If the restart priority is null, it means we don't know if the VM is agile yet, and we have
                // protectVmsByDefault == true.
                result[row.Vm] = row.RestartPriority ?? VM.HA_Restart_Priority.BestEffort;
            }
            return result;
        }

        private void dataGridViewVms_SelectionChanged(object sender, EventArgs e)
        {
            updateButtons();
        }

        private void m_dropDownButtonRestartPriority_Click(object sender, EventArgs e)
        {
            var selectedRows = dataGridViewVms.SelectedRows.Cast<VmWithSettingsRow>().ToList();

            foreach (ToolStripMenuItem item in contextMenuStrip.Items)
            {
                var itemRestartPriority = ((VM.HA_Restart_Priority)item.Tag);
                item.Checked = selectedRows.Count > 0 && selectedRows.All(s => s.RestartPriority == itemRestartPriority);
            }
        }

        private void updateButtons()
        {
            Program.AssertOnEventThread();

            var selectedRows = dataGridViewVms.SelectedRows.Cast<VmWithSettingsRow>().ToList();

            if (dataGridViewVms.SelectedRows.Count == 0)
            {
                m_dropDownButtonRestartPriority.Enabled = false;
                m_dropDownButtonRestartPriority.Text = "";
                nudOrder.Enabled = nudStartDelay.Enabled = false;
                nudOrder.Text = nudStartDelay.Text = "";
                return;
            }

            // if there is a VM with null priority in the selection disable all buttons. We are waiting on the background thread 
            // to see if the VM is agile before giving it a starting value
            if (selectedRows.Any(r => r.RestartPriority == null))
            {
                m_dropDownButtonRestartPriority.Enabled = true;
                m_dropDownButtonRestartPriority.Text = "";
                nudOrder.Enabled = nudStartDelay.Enabled = true;
                nudOrder.Text = nudStartDelay.Text = "";
                return;
            }

            m_dropDownButtonRestartPriority.Enabled = true;
            
            //now set the drop down button text

            bool allSamePriority = false;

            foreach (ToolStripMenuItem item in contextMenuStrip.Items)
            {
                VM.HA_Restart_Priority itemRestartPriority = (VM.HA_Restart_Priority)item.Tag;

                if (selectedRows.All(r => r.RestartPriority == itemRestartPriority))
                {
                    allSamePriority = true;
                    m_dropDownButtonRestartPriority.Text = Helpers.RestartPriorityI18n(itemRestartPriority);
                    break;
                }
            }

            if (!allSamePriority && dataGridViewVms.SelectedRows.Count > 1)
            {
                m_dropDownButtonRestartPriority.Text = Messages.HA_ASSIGN_PRIORITIES_MIXED_PROTECTION_LEVELS;
            }

            // set the order and delay NUDs 
            nudOrder.Enabled = nudStartDelay.Enabled = true;

            var orderDistList = (from row in selectedRows select row.StartOrder).Distinct().ToList();
            nudOrder.Text = orderDistList.Count == 1 ? orderDistList.ElementAt(0).ToString() : "";

            var delayDistList = (from row in selectedRows select row.StartDelay).Distinct().ToList();
            nudStartDelay.Text = delayDistList.Count == 1 ? delayDistList.ElementAt(0).ToString() : "";

            // check that all the VMs selected in the list are agile and make sure the protect button is disabled with the relevant reason
            VmWithSettingsRow nonAgileRow = selectedRows.FirstOrDefault(r => !r.IsAgile);

            // Now set the buttons and tooltips)
            foreach (ToolStripMenuItem menuItem in contextMenuStrip.Items)
            {
                var priority = (VM.HA_Restart_Priority)menuItem.Tag;

                if (VM.HaPriorityIsRestart(connection, priority))
                {
                    menuItem.Enabled = (nonAgileRow == null);
                    menuItem.ToolTipText = (nonAgileRow == null)
                                               ? ""
                                               : nonAgileRow.FriendlyNonAgileReason;
                }
                else
                {
                    menuItem.Enabled = true;
                }
            }
        }

        private void nudOrder_ValueChanged(object sender, EventArgs e)
        {
            long newValue = (long)nudOrder.Value;
            bool changesMade = false;

            foreach (var row in dataGridViewVms.SelectedRows.Cast<VmWithSettingsRow>())
            {
                if (row.StartOrder != newValue)
                {
                    changesMade = true;
                    row.UpdateStartOrder(newValue);
                }
            }

            if (changesMade)
            {
                ChangesMade = true;
                updateButtons();
            }
        }

        private void nudStartDelay_ValueChanged(object sender, EventArgs e)
        {
            long newValue = (long)nudStartDelay.Value;
            bool changesMade = false;

            foreach (var row in dataGridViewVms.SelectedRows.Cast<VmWithSettingsRow>())
            {
                if (row.StartDelay != newValue)
                {
                    changesMade = true;
                    row.UpdateStartDelay(newValue);
                }
            }

            if (changesMade)
            {
                ChangesMade = true;
                updateButtons();
            }
        }

        /// <summary>
        /// Gets the changed (uncommitted) VM startup options. Must be called on the GUI thread.
        /// </summary>
        /// <returns></returns>
        public Dictionary<VM, VMStartupOptions> GetChangedStartupOptions()
        {
            Program.AssertOnEventThread();
            Dictionary<VM, VMStartupOptions> result = new Dictionary<VM, VMStartupOptions>();
            
            foreach (var curRow in dataGridViewVms.Rows.Cast<VmWithSettingsRow>())
            {
                if (curRow.HasChanged())
                    result[curRow.Vm] = new VMStartupOptions(curRow.StartOrder,
                                                             curRow.StartDelay,
                                                             curRow.RestartPriority ?? VM.HA_Restart_Priority.BestEffort);
            }
            return result;
        }

        private void linkLabelTellMeMore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Help.HelpManager.Launch("HaNtolZero");
        }

        #region XentabPage overrides

        public override string Text
        {
            get { return Messages.HAWIZARD_ASSIGNPRIORITIESPAGE_TEXT; }
        }

        public override string PageTitle
        {
            get 
            {
                return Messages.HAWIZARD_ASSIGNPRIORITIESPAGE_TITLE;
            }
        }

        public override bool EnableNext()
        {
            return !haNtolIndicator.UpdateInProgress && Ntol >= 0 && !haNtolIndicator.Overcommitted;
        }

        public override void PageCancelled()
        {
            StopNtolUpdate();
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            PopulatePageControls();
        }

        public override void SelectDefaultControl()
        {
            dataGridViewVms.Select();
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            StopNtolUpdate();
        }

        #endregion

        #region Nested classes

        private class VmWithSettingsRow : DataGridViewRow
        {
            private readonly DataGridViewImageCell cellImage;
            private readonly DataGridViewTextBoxCell cellVm;
            private readonly DataGridViewTextBoxCell cellRestartPriority;
            private readonly DataGridViewTextBoxCell cellStartOrder;
            private readonly DataGridViewTextBoxCell cellDelay;
            private readonly DataGridViewTextBoxCell cellAgile;
            private string _nonAgileReason = string.Empty;

            public VM Vm { get; private set; }
            public long StartDelay { get; private set; }
            public long StartOrder { get; private set; }
            public VM.HA_Restart_Priority? RestartPriority { get; private set; }
            public bool IsAgile { get; private set; }

            /// <summary>
            /// Returns a short version of the agility violation for the error type stored in NonAgileReason.
            /// Returns empty string if there is no error.
            /// For a more detailed, technical description use the error translation in FriendlyErrorNames.resx
            /// </summary>
            public string FriendlyNonAgileReason
            {
                get { return _nonAgileReason; }
                set
                {
                    var msg = string.Empty;
                    switch (value)
                    {
                        case null:
                            break;
                        case "HA_CONSTRAINT_VIOLATION_SR_NOT_SHARED":
                            msg = Messages.NOT_AGILE_SR_NOT_SHARED;
                            break;
                        case "HA_CONSTRAINT_VIOLATION_NETWORK_NOT_SHARED":
                            msg = Messages.NOT_AGILE_NETWORK_NOT_SHARED;
                            break;
                        case "VM_HAS_VGPU":
                            msg = Messages.NOT_AGILE_VM_HAS_VGPU;
                            break;
                        case "VM_HAS_VUSB":
                            msg = Messages.NOT_AGILE_VM_HAS_VUSB;
                            break;
                        case "VM_HAS_SRIOV_VIF":
                            msg = Messages.NOT_AGILE_VM_HAS_SRIOV_VIF;
                            break;
                        default:
                            // We shouldn't really be here unless we have not iterated through all the return errors from vm.assert_agile
                            msg = Messages.NOT_AGILE_UNKOWN;
                            break;
                    }

                    _nonAgileReason = msg;
                    cellAgile.ToolTipText = msg;
                }
            }

            public VmWithSettingsRow(VM vm, VM.HA_Restart_Priority? priority)
            {
                cellImage = new DataGridViewImageCell {ValueType = typeof(Image)};
                cellVm = new DataGridViewTextBoxCell();
                cellRestartPriority = new DataGridViewTextBoxCell();
                cellStartOrder = new DataGridViewTextBoxCell();
                cellDelay = new DataGridViewTextBoxCell();
                cellAgile = new DataGridViewTextBoxCell();
                Cells.AddRange(cellImage, cellVm, cellRestartPriority, cellStartOrder, cellDelay, cellAgile);

                UpdateVm(vm);
                UpdateRestartPriority(priority);
                UpdateStartOrder(vm.order);
                UpdateStartDelay(vm.start_delay);
                SetAgileCalculating();
            }

            public void UpdateVm(VM vm)
            {
                Vm = vm;
                cellImage.Value = Images.GetImage16For(vm);
                cellVm.Value = vm.Name();
            }

            public void UpdateStartDelay(long startDelay)
            {
                StartDelay = startDelay;
                cellDelay.Value = string.Format(Messages.TIME_SECONDS, startDelay);
            }

            public void UpdateStartOrder(long startOrder)
            {
                StartOrder = startOrder;
                cellStartOrder.Value = startOrder.ToString();
            }

            public void UpdateRestartPriority(VM.HA_Restart_Priority? restartPriority)
            {
                RestartPriority = restartPriority;
                cellRestartPriority.Value = Helpers.RestartPriorityI18n(restartPriority);
            }

            public void UpdateAgile(bool isAgile)
            {
                IsAgile = isAgile;
                cellAgile.Value = isAgile.ToYesNoStringI18n();
            }

            public void SetAgileCalculating()
            {
                cellAgile.Value = Messages.HA_CALCULATING_AGILITY;
            }

            public bool HasChanged()
            {
                return Vm.order != StartOrder || Vm.start_delay != StartDelay || Vm.HARestartPriority() != RestartPriority;
            }
        }

        #endregion
    }
}
