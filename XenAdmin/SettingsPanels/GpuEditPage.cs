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
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;


namespace XenAdmin.SettingsPanels
{
    public partial class GpuEditPage : XenTabPage, IEditPage
    {
        public VM vm;
        private GpuTuple currentGpuTuple;
        private GPU_group[] gpu_groups;
        private bool gpusAvailable;

        public GpuEditPage()
        {
            InitializeComponent();
            imgRDP.Visible = labelRDP.Visible =
                imgNeedDriver.Visible = labelNeedDriver.Visible =
                imgNeedGpu.Visible = labelNeedGpu.Visible =
                imgStopVM.Visible = labelStopVM.Visible =
                imgHA.Visible = labelHA.Visible =
                    false;
        }

        public GPU_group GpuGroup
        {
            get
            {
                GpuTuple tuple = comboBoxGpus.SelectedItem as GpuTuple;
                return tuple == null ? null : tuple.GpuGroup;
            }
        }

        public VGPU_type VgpuType
        {
            get
            {
                GpuTuple tuple = comboBoxGpus.SelectedItem as GpuTuple;
                if (tuple == null || tuple.VgpuTypes == null || tuple.VgpuTypes.Length == 0)
                    return null;

                return tuple.VgpuTypes[0];
            }
        }

        public VM.HA_Restart_Priority SelectedPriority { private get; set; }

        #region IEditPage Members

        public AsyncAction SaveSettings()
        {
            return new GpuAssignAction(vm, GpuGroup, VgpuType);
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            Trace.Assert(clone is VM);  // only VMs should show this page
            Trace.Assert(!Helpers.FeatureForbidden(clone, Host.RestrictGpu));  // If license insufficient, we show upsell page instead

            vm = (VM)clone;
            SelectedPriority = vm.HARestartPriority();

            if (Connection == null) // on the PropertiesDialog
                Connection = vm.Connection;

            PopulatePage();
        }

        public bool ValidToSave
        {
            get { return true; }
        }

        public void ShowLocalValidationMessages()
        {
        }

        public void Cleanup()
        {
        }

        public bool HasChanged
        {
            get
            {
                var tuple = comboBoxGpus.SelectedItem as GpuTuple;
                if (tuple == null)
                    return false;
                return !tuple.Equals(currentGpuTuple);
            }
        }

        #region IVerticalTab Members

        public string SubText
        {
            get
            {
                string txt = Messages.GPU_UNAVAILABLE;

                if (gpusAvailable)
                {
                    var tuple = comboBoxGpus.SelectedItem as GpuTuple;
                    if (tuple != null)
                        txt = tuple.ToString();
                }

                return txt;
            }
        }

        public Image Image
        {
            get { return Resources._000_GetMemoryInfo_h32bit_16; }
        }

        #endregion

        #endregion

        #region XenTabPage overrides

        public override string Text
        {
            get { return Messages.GPU; }
        }

        public override string PageTitle
        {
            get { return Messages.NEWVMWIZARD_VGPUPAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return "GPU"; }
        }

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                var summ = new List<KeyValuePair<string, string>>();

                GpuTuple tuple = comboBoxGpus.SelectedItem as GpuTuple;
                if (tuple != null)
                    summ.Add(new KeyValuePair<string, string>(Messages.GPU, tuple.ToString()));

                return summ;
            }
        }
        
        public override void PopulatePage()
        {
            currentGpuTuple = new GpuTuple(null, null, null);

            if (vm.VGPUs.Count != 0)
            {
                VGPU vgpu = Connection.Resolve(vm.VGPUs[0]);
                if (vgpu != null)
                {
                    var vgpuGroup = Connection.Resolve(vgpu.GPU_group);

                    if (Helpers.FeatureForbidden(Connection, Host.RestrictVgpu) || !vm.CanHaveVGpu())
                    {
                        if (vgpuGroup.HasPassthrough())
                            currentGpuTuple = new GpuTuple(vgpuGroup, null, null); //GPU pass-through item
                    }
                    else
                    {
                        VGPU_type vgpuType = Connection.Resolve(vgpu.type);
                        currentGpuTuple = new GpuTuple(vgpuGroup, vgpuType, null);
                    }
                }
            }

            // vGPU was introduced in Clearwater SP1
            gpu_groups = !Helpers.ClearwaterSp1OrGreater(Connection) //We used to check host.RestrictVgpu here (instead of checking the API version); this is not correct anymore, because vGPU is a licensed feature.
                 ? Connection.Cache.GPU_groups
                 : Connection.Cache.GPU_groups.Where(g => g.PGPUs.Count > 0 && g.supported_VGPU_types.Count != 0).ToArray();
                   //not showing empty groups
            
            gpusAvailable = gpu_groups.Length > 0;

            if (gpusAvailable)
            {
                PopulateComboBox();
                ShowHideWarnings();
            }
            else
            {
                labelRubric.Text = Helpers.GetPool(Connection) == null
                                       ? Messages.GPU_RUBRIC_NO_GPUS_SERVER
                                       : Messages.GPU_RUBRIC_NO_GPUS_POOL;

                tableLayoutPanel1.Visible = false;
                warningsTable.Visible = false;
            }
        }

        public override void SelectDefaultControl()
        {
            if (comboBoxGpus.CanSelect)
                comboBoxGpus.Select();
        }

        #endregion

        private void PopulateComboBox()
        {
            var noneItem = new GpuTuple(null, null, null); // "None" item
            comboBoxGpus.Items.Add(noneItem);

            Array.Sort(gpu_groups);
            foreach (GPU_group gpu_group in gpu_groups)
            {
                if (Helpers.FeatureForbidden(Connection, Host.RestrictVgpu) || !vm.CanHaveVGpu())
                {
                    if (gpu_group.HasPassthrough())
                        comboBoxGpus.Items.Add(new GpuTuple(gpu_group, null, null));  //GPU pass-through item
                }
                else
                {
                    var enabledTypes = Connection.ResolveAll(gpu_group.enabled_VGPU_types);
                    var allTypes = Connection.ResolveAll(gpu_group.supported_VGPU_types);

                    var disabledTypes = allTypes.FindAll(t => !enabledTypes.Exists(e => e.opaque_ref == t.opaque_ref));

                    if (gpu_group.HasVGpu())
                    {
                        allTypes.Sort();
                        allTypes.Reverse();
                        comboBoxGpus.Items.Add(new GpuTuple(gpu_group, allTypes.ToArray())); // Group item
                    }

                    foreach (var vgpuType in allTypes)
                        comboBoxGpus.Items.Add(new GpuTuple(gpu_group, vgpuType, disabledTypes.ToArray())); // GPU_type item
                }
            }

            foreach (var item in comboBoxGpus.Items)
            {
                var tuple = item as GpuTuple;
                if (tuple == null)
                    continue;

                if (tuple.Equals(currentGpuTuple))
                {
                    comboBoxGpus.SelectedItem = item;
                    break;
                }
            }
            if (comboBoxGpus.SelectedItem == null)
                comboBoxGpus.SelectedItem = noneItem;
        }

        public void ShowHideWarnings()
        {
            if (!gpusAvailable)
                return;

            imgExperimental.Visible = labelExperimental.Visible =
                VgpuType != null && VgpuType.experimental;

            if (vm.power_state != vm_power_state.Halted)
            {
                imgRDP.Visible = labelRDP.Visible =
                imgNeedDriver.Visible = labelNeedDriver.Visible =
                imgNeedGpu.Visible = labelNeedGpu.Visible =
                imgHA.Visible = labelHA.Visible =
                    false;

                imgStopVM.Visible = labelStopVM.Visible = true;

                labelGpuType.Enabled = comboBoxGpus.Enabled = false;
                return;
            }

            Pool pool = Helpers.GetPool(Connection);
            if (pool != null && pool.ha_enabled && VM.HaPriorityIsRestart(Connection, SelectedPriority))
            {
                imgRDP.Visible = labelRDP.Visible =
                imgNeedDriver.Visible = labelNeedDriver.Visible =
                imgNeedGpu.Visible = labelNeedGpu.Visible =
                imgStopVM.Visible = labelStopVM.Visible =
                    false;

                imgHA.Visible = labelHA.Visible = true;

                labelGpuType.Enabled = comboBoxGpus.Enabled = false;
                return;
            }

            labelGpuType.Enabled = comboBoxGpus.Enabled = true;
            GpuTuple tuple = comboBoxGpus.SelectedItem as GpuTuple;

            imgStopVM.Visible = labelStopVM.Visible =
            imgHA.Visible = labelHA.Visible = false;

            imgRDP.Visible = labelRDP.Visible =
                HasChanged && currentGpuTuple.GpuGroup == null &&
                tuple != null && !tuple.IsFractionalVgpu;

            imgNeedGpu.Visible = labelNeedGpu.Visible =
            labelNeedDriver.Visible = imgNeedDriver.Visible =
                tuple != null && tuple.GpuGroup != null;
        }

        private void comboBoxGpus_SelectedIndexChanged(object sender, EventArgs e)
        {
            warningsTable.SuspendLayout();
            ShowHideWarnings();
            warningsTable.ResumeLayout();
        }

        private void warningsTable_SizeChanged(object sender, EventArgs e)
        {
            int[] columnsWidth = warningsTable.GetColumnWidths();
            int textColumnWidth = columnsWidth.Length > 1 ? columnsWidth[1] : 1;

            labelRDP.MaximumSize = labelStopVM.MaximumSize = new Size(textColumnWidth, 999);
        }
    }
}
