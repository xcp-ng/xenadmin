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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Dialogs;


namespace XenAdmin.SettingsPanels
{
    public partial class CPUMemoryEditPage : UserControl, IEditPage
    {
        private VM vm;
        bool ShowMemory = false;       // If this VM has DMC, we don't show the memory controls on this page.

        private bool _ValidToSave = true;
        private decimal _OrigMemory;
        private long _OrigVCPUs;
        private long _OrigVCPUsMax;
        private long _OrigVCPUsAtStartup;
        private decimal _OrigVCPUWeight;
        private decimal _CurrentVCPUWeight;
        private bool isVcpuHotplugSupported;

        // Please note that the comboBoxVCPUs control can represent two different VM properties, depending whether the VM supports vCPU hotplug or not: 
        // If vCPU hotplug is supported, comboBoxVCPUs represents the maximum number of vCPUs (VCPUs_max). And the initial number of vCPUs is represented in comboBoxInitialVCPUs (which is only visible in this case) 
        // If vCPU hotplug is not supported, comboBoxVCPUs represents the initial number of vCPUs (VCPUs_at_startup). In this case we will also set the VM property VCPUs_max to the same value.
        // We use the _OrigVCPUs variable to store the original value that populates this combo box (VCPUs_max if hotplug is allowed, otherwise VCPUs_at_startup)

        private ChangeMemorySettingsAction memoryAction;
        public bool ValidToSave
        {
            get
            {
                if (!_ValidToSave)
                    return false;

                // Also confirm whether the user wants to save memory changes.
                // If not, don't close the properties dialog.
                if (HasMemoryChanged)
                {
                    long mem = Convert.ToInt64(this.nudMemory.Value * Util.BINARY_MEGA);
                    memoryAction = BallooningDialog.ConfirmAndReturnAction(Program.MainWindow, vm, mem, mem, mem, (long)vm.memory_static_max, false);
                    if (memoryAction == null)
                        return false;
                }

                return true;
            }
        }

        public CPUMemoryEditPage()
        {
            InitializeComponent();

            Text = Messages.CPU_AND_MEMORY;

            transparentTrackBar1.Scroll += new EventHandler(tbPriority_Scroll);

            this.nudMemory.TextChanged += new EventHandler(nudMemory_TextChanged);
            this.nudMemory.LostFocus += new EventHandler(nudMemory_LostFocus);
        }

        public Image Image
        {
            get
            {
                return Properties.Resources._000_CPU_h32bit_16;
            }
        }

        void nudMemory_LostFocus(object sender, EventArgs e)
        {
            ValidateNud(nudMemory, (decimal)vm.memory_static_max / Util.BINARY_MEGA);
        }

        private void ValidateNud(NumericUpDown nud, Decimal defaultValue)
        {
            if (!String.IsNullOrEmpty(nud.Text.Trim()))
                return;

            nud.Value = defaultValue >= nud.Minimum && defaultValue <= nud.Maximum ?
                defaultValue : nud.Maximum;

            nud.Text = nud.Value.ToString();
        }

        void nudMemory_TextChanged(object sender, EventArgs e)
        {
            decimal val;
            if (decimal.TryParse(nudMemory.Text, out val))
            {
                if (val >= nudMemory.Minimum && val <= nudMemory.Maximum)
                    nudMemory_ValueChanged(null, null);
                else if (val > nudMemory.Maximum)
                    ShowMemError(true, false);
                else
                    ShowMemError(false, false);
            }
            if (this.nudMemory.Text == "")
            {
                _ValidToSave = false;
            }
            else
            {
                _ValidToSave = true;
            }
        }

        private void tbPriority_Scroll(object sender, EventArgs e)
        {
            _CurrentVCPUWeight = Convert.ToDecimal(Math.Pow(4.0d, Convert.ToDouble(transparentTrackBar1.Value)));
            if (transparentTrackBar1.Value == transparentTrackBar1.Max)
                _CurrentVCPUWeight--;
        }

        /// <summary>
        /// Must be a VM.
        /// </summary>
        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            vm = (VM)clone;
            ShowMemory = Helpers.FeatureForbidden(vm, Host.RestrictDMC);
            Repopulate();
        }

        public void Repopulate()
        {
            VM vm = this.vm;

            Text = ShowMemory ? Messages.CPU_AND_MEMORY : Messages.CPU;
            if (!ShowMemory)
                lblMemory.Visible = panel2.Visible = MemWarningLabel.Visible = false;
            else if (vm.power_state != vm_power_state.Halted && vm.power_state != vm_power_state.Running)
            {
                panel2.Enabled = false;
                MemWarningLabel.Text = Messages.MEM_NOT_WHEN_SUSPENDED;
                MemWarningLabel.ForeColor = SystemColors.ControlText;
                MemWarningLabel.Visible = true;
            }

            // Since updates come in dribs and drabs, avoid error if new max and min arrive
            // out of sync and maximum < minimum.
            if (vm.memory_dynamic_max >= vm.memory_dynamic_min &&
                vm.memory_static_max >= vm.memory_static_min)
            {
                decimal min = Convert.ToDecimal(vm.memory_static_min / Util.BINARY_MEGA);
                decimal max = Convert.ToDecimal(vm.MaxMemAllowed() / Util.BINARY_MEGA);
                decimal value = Convert.ToDecimal(vm.memory_static_max / Util.BINARY_MEGA);
                // Avoid setting the range to exclude the current value: CA-40041
                if (value > max)
                    max = value;
                if (value < min)
                    min = value;
                this.nudMemory.Minimum = min;
                this.nudMemory.Maximum = max;
                this.nudMemory.Text = (this.nudMemory.Value = value).ToString();
            }

            Host currentHost = Helpers.GetMaster(this.vm.Connection);
            if (currentHost != null)
            {
                // Show the performance warning about vCPUs > pCPUs.
                // Don't show if the VM isn't running, since we don't know which server it will
                // run on (and so can't count the number of pCPUs).
                if ( vm.power_state == vm_power_state.Running
                    && vm.VCPUs_at_startup > currentHost.host_CPUs.Count
                    && !vm.GetIgnoreExcessiveVcpus())
                {
                    lblVcpuWarning.Visible = true;
                    this.tableLayoutPanel1.RowStyles[1].SizeType = SizeType.Absolute;
                    this.tableLayoutPanel1.RowStyles[1].Height = 30;
                }
                else
                {
                    lblVcpuWarning.Visible = false;
                }
            }
            else
            {
                lblVcpuWarning.Visible = false;
            }

            isVcpuHotplugSupported = vm.SupportsVcpuHotplug();

            label1.Text = GetRubric();

            _OrigMemory = nudMemory.Value;
            _OrigVCPUsMax = vm.VCPUs_max > 0 ? vm.VCPUs_max : 1;
            _OrigVCPUsAtStartup = vm.VCPUs_at_startup > 0 ? vm.VCPUs_at_startup : 1;
            _OrigVCPUWeight = _CurrentVCPUWeight;
            _OrigVCPUs = isVcpuHotplugSupported ? _OrigVCPUsMax : _OrigVCPUsAtStartup;
            _prevVCPUsMax = _OrigVCPUsMax;  // we use variable in RefreshCurrentVCPUs for checking if VcpusAtStartup and VcpusMax were equal before VcpusMax changed

            _CurrentVCPUWeight = Convert.ToDecimal(vm.GetVcpuWeight());

            InitializeVcpuControls();
            
            _ValidToSave = true;
        }

        private void InitializeVcpuControls()
        {
            lblVCPUs.Text = isVcpuHotplugSupported
                ? Messages.VM_CPUMEMPAGE_MAX_VCPUS_LABEL
                : Messages.VM_CPUMEMPAGE_VCPUS_LABEL;

            labelInitialVCPUs.Text = vm.power_state == vm_power_state.Halted
                ? Messages.VM_CPUMEMPAGE_INITIAL_VCPUS_LABEL
                : Messages.VM_CPUMEMPAGE_CURRENT_VCPUS_LABEL;
            
            labelInitialVCPUs.Visible = comboBoxInitialVCPUs.Visible = isVcpuHotplugSupported;
            comboBoxInitialVCPUs.Enabled = isVcpuHotplugSupported &&
                                           (vm.power_state == vm_power_state.Halted ||
                                            vm.power_state == vm_power_state.Running);

            comboBoxVCPUs.Enabled = comboBoxTopology.Enabled = vm.power_state == vm_power_state.Halted;

            comboBoxTopology.Populate(vm.VCPUs_at_startup, vm.VCPUs_max, vm.GetCoresPerSocket(), vm.MaxCoresPerSocket());

            // CA-12941 
            // We set a sensible maximum based on the template, but if the user sets something higher 
            // from the CLI then use that as the maximum.
            var maxAllowed = vm.MaxVCPUsAllowed();
            long maxVCPUs = maxAllowed < _OrigVCPUs ? _OrigVCPUs : maxAllowed;
            PopulateVCPUs(maxVCPUs, _OrigVCPUs);

            if (isVcpuHotplugSupported)
                PopulateVCPUsAtStartup(_OrigVCPUsMax, _OrigVCPUsAtStartup);

            transparentTrackBar1.Value = Convert.ToInt32(Math.Log(Convert.ToDouble(vm.GetVcpuWeight())) / Math.Log(4.0d));
            panel1.Enabled = vm.power_state == vm_power_state.Halted;
        }

        private void PopulateVCPUComboBox(ComboBox comboBox, long min, long max, long currentValue, Predicate<long> isValid)
        {
            comboBox.BeginUpdate();
            comboBox.Items.Clear();
            for (long i = min; i <= max; ++i)
            {
                if (i == currentValue || isValid(i))
                    comboBox.Items.Add(i);
            }
            if (currentValue > max)
                comboBox.Items.Add(currentValue);
            comboBox.SelectedItem = currentValue;
            comboBox.EndUpdate();
        }

        private void PopulateVCPUs(long maxVCPUs, long currentVCPUs)
        {
            PopulateVCPUComboBox(comboBoxVCPUs, 1, maxVCPUs, currentVCPUs, i => comboBoxTopology.IsValidVCPU(i));
        }

        private void PopulateVCPUsAtStartup(long max, long currentValue)
        {
            long min = vm.power_state == vm_power_state.Halted ? 1 : _OrigVCPUsAtStartup;
            PopulateVCPUComboBox(comboBoxInitialVCPUs, min, max, currentValue, i => true);
        }

        private string GetRubric()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Messages.VM_CPUMEMPAGE_RUBRIC);
            // add hotplug text
            if (isVcpuHotplugSupported)
                sb.Append(Messages.VM_CPUMEMPAGE_RUBRIC_HOTPLUG);
            // add power state warning
            if (vm.power_state != vm_power_state.Halted)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(isVcpuHotplugSupported ? Messages.VM_CPUMEMPAGE_MAX_VCPUS_READONLY : Messages.VCPU_ONLY_WHEN_HALTED);
            }
            // add power state warning for Current number of vCPUs
            if (isVcpuHotplugSupported && vm.power_state != vm_power_state.Halted && vm.power_state != vm_power_state.Running)
            {
                sb.Append(Messages.VM_CPUMEMPAGE_CURRENT_VCPUS_READONLY);
            }
            return sb.ToString();
        }

        public bool HasChanged
        {
            get { return HasVCPUChanged || HasMemoryChanged || HasTopologyChanged || HasVCPUsAtStartupChanged || HasVCPUWeightChanged; }
        }

        private bool HasMemoryChanged
        {
            get
            {
                return _OrigMemory != nudMemory.Value;
            }
        }

        private bool HasVCPUChanged
        {
            get
            {
                return _OrigVCPUs != (long)comboBoxVCPUs.SelectedItem;
            }
        }

        private bool HasVCPUWeightChanged
        {
            get
            {
                return _OrigVCPUWeight != _CurrentVCPUWeight;
            }
        }

        private bool HasVCPUsAtStartupChanged
        {
            get
            {
                return isVcpuHotplugSupported && _OrigVCPUsAtStartup != (long)comboBoxInitialVCPUs.SelectedItem;
            }
        }

        private bool HasTopologyChanged
        {
            get
            {
                return vm.GetCoresPerSocket() != comboBoxTopology.CoresPerSocket;
            }
        }

        private long SelectedVcpusMax
        {
            get
            {
                return (long)comboBoxVCPUs.SelectedItem;
            }
        }

        private long SelectedVcpusAtStartup
        {
            get
            {
                return isVcpuHotplugSupported ? (long)comboBoxInitialVCPUs.SelectedItem : (long)comboBoxVCPUs.SelectedItem;
            }
        }

        public AsyncAction SaveSettings()
        {
            List<AsyncAction> actions = new List<AsyncAction>();

            if (HasVCPUWeightChanged)
            {
                vm.SetVcpuWeight(Convert.ToInt32(_CurrentVCPUWeight));
            }

            if (HasVCPUChanged || HasVCPUsAtStartupChanged)
            {
                actions.Add(new ChangeVCPUSettingsAction(vm, SelectedVcpusMax, SelectedVcpusAtStartup));
            }

            if (HasTopologyChanged)
            {
                vm.SetCoresPerSocket(comboBoxTopology.CoresPerSocket);
            }
			
			if (HasMemoryChanged)
            {
                actions.Add(memoryAction);  // Calculated in ValidToSave
            }

            if (actions.Count == 0)
                return null;
            else if (actions.Count == 1)
                return actions[0];
            else
            {
                MultipleAction multipleAction = new MultipleAction(vm.Connection, "", "", "", actions, true);
                return multipleAction;
            }
        }

        /** Show local validation balloon tooltips */
        public void ShowLocalValidationMessages() { }

        /** Unregister listeners, dispose balloon tooltips, etc. */
        public void Cleanup() { }

        /// <summary>
        /// Shows the warning dialog about vCPUs > pCPUs.
        /// </summary>
        private void lblVcpuWarning_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (vm == null)
            {
                System.Diagnostics.Trace.Assert(false, "Selected object should be a vm");
            }
            else
            {
                new Dialogs.VcpuWarningDialog(vm).ShowDialog();
                this.Refresh();
            }
        }

        private void nudMemory_ValueChanged(object sender, EventArgs e)
        {
            ShowMemError(false, true);
        }

        private void ShowMemError(bool showAlways, bool testValue)
        {
            if (vm == null || !ShowMemory)
                return;

            Host selectedAffinity = vm.Connection.Resolve<Host>(vm.power_state == vm_power_state.Running ? vm.resident_on : vm.affinity);
            if (selectedAffinity != null)
            {
                Host_metrics host_metrics = vm.Connection.Resolve<Host_metrics>(selectedAffinity.metrics);
                if ((showAlways || (testValue && (host_metrics != null && (double)host_metrics.memory_total < (double)nudMemory.Value * (double)Util.BINARY_MEGA))))
                {
                    MemWarningLabel.Visible = true;
                }
                else
                {
                    MemWarningLabel.Visible = false;
                }
            }
        }

        private void comboBoxVCPUs_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowVcpuError(false, true);
            comboBoxTopology.Update((long)comboBoxVCPUs.SelectedItem);
            ValidateVCPUSettings();
            RefreshCurrentVCPUs();
        }

        private void ShowVcpuError(bool showAlways, bool testValue)
        {
            if (vm == null || !comboBoxVCPUs.Enabled)
                return;
            Host selectedAffinity = vm.Home();
            if (selectedAffinity == null && vm.Connection.Cache.Hosts.Length == 1)
                selectedAffinity = vm.Connection.Cache.Hosts[0];

            if (showAlways || (testValue && (selectedAffinity != null && selectedAffinity.host_CPUs.Count < (long)comboBoxVCPUs.SelectedItem)))
            {
                VCPUWarningLabel.Visible = true;
            }
            else
            {
                VCPUWarningLabel.Visible = false;
            }
        }

        private long _prevVCPUsMax;

        private void RefreshCurrentVCPUs()
        {
            // refresh comboBoxInitialVCPUs if it's visible and populated
            if (comboBoxInitialVCPUs.Visible && comboBoxInitialVCPUs.Items.Count > 0)
            {
                // VcpusAtStartup is always <= VcpusMax
                // So if VcpusMax is decreased below VcpusAtStartup, then VcpusAtStartup is decreased to that number too
                // If VcpusAtStartup and VcpusMax are equal, and VcpusMax is changed, then VcpusAtStartup is changed to match
                // But if the numbers are unequal, and VcpusMax is changed but is still higher than VcpusAtStartup, then VcpusAtStartup is unchanged
                var newValue = SelectedVcpusAtStartup;
               
                if (SelectedVcpusMax < SelectedVcpusAtStartup)
                    newValue = SelectedVcpusMax;
                else if (SelectedVcpusAtStartup == _prevVCPUsMax && SelectedVcpusMax != _prevVCPUsMax)
                    newValue = SelectedVcpusMax;

                PopulateVCPUsAtStartup(SelectedVcpusMax, newValue);
                _prevVCPUsMax = SelectedVcpusMax;
            }
        }

        public String SubText
        {
            get
            {
                return ShowMemory ?
                    String.Format(Messages.CPU_AND_MEMORY_SUB, SelectedVcpusAtStartup, nudMemory.Value) :
                    String.Format(Messages.CPU_SUB, SelectedVcpusAtStartup);
            }
        }
        
        private void comboBoxTopology_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateVCPUSettings();
        }

        private void ValidateVCPUSettings()
        {
            if (comboBoxVCPUs.SelectedItem != null)
                labelInvalidVCPUWarning.Text = VM.ValidVCPUConfiguration((long)comboBoxVCPUs.SelectedItem, comboBoxTopology.CoresPerSocket);
        }
    }
}
