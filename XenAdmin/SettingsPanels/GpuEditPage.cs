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
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Properties;
using XenAPI;


namespace XenAdmin.SettingsPanels
{
    public partial class GpuEditPage : XenTabPage, IEditPage
    {
        public VM vm;
        private List<VGPU> currentGpus = new List<VGPU>();
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

        public List<VGPU> VGpus
        {
            get
            {
                return gpuGrid.Rows.Cast<object>().Select(row => row as VGpuDetailRow)
                    .Select(vGpuRow => vGpuRow?.VGpu).Where(vGpu => vGpu != null).ToList();
            }
        }

        public VM.HA_Restart_Priority SelectedPriority { private get; set; }

        #region IEditPage Members

        public AsyncAction SaveSettings()
        {
            return new GpuAssignAction(vm, VGpus);
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

        public bool ValidToSave => true;

        public void ShowLocalValidationMessages()
        {
        }

        public void Cleanup()
        {
        }

        public bool HasChanged => !VGpus.SequenceEqual(currentGpus);
        
        #region IVerticalTab Members

        public string SubText
        {
            get
            {
                string txt = Messages.UNAVAILABLE;

                if (gpusAvailable)
                {
                    var vGpus = VGpus;
                    txt = vGpus.Count > 0 ? string.Join(",", vGpus.Select(v => v.VGpuTypeDescription())) : Messages.GPU_NONE;
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

        public override string Text => Messages.GPU;

        public override string PageTitle => Messages.NEWVMWIZARD_VGPUPAGE_TITLE;

        public override string HelpID => "GPU";

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                var summ = new List<KeyValuePair<string, string>>();

                if (gpuGrid.Rows.Count > 0)
                    summ.Add(new KeyValuePair<string, string>(Messages.GPU, SubText));

                return summ;
            }
        }
        
        public override void PopulatePage()
        {
            currentGpus.Clear();

            gpu_groups = Connection.Cache.GPU_groups.Where(g => g.PGPUs.Count > 0 && g.supported_VGPU_types.Count != 0).ToArray();
            //not showing empty groups

            gpusAvailable = gpu_groups.Length > 0;

            if (gpusAvailable)
            {
                PopulateGrid();
                ShowHideWarnings();
            }
            else
            {
                labelRubric.Text = Helpers.GetPool(Connection) == null
                                       ? Messages.GPU_RUBRIC_NO_GPUS_SERVER
                                       : Messages.GPU_RUBRIC_NO_GPUS_POOL;

                gpuGrid.Visible = addButton.Visible = deleteButton.Visible = false;
                warningsTable.Visible = false;
            }
        }

        public override void SelectDefaultControl()
        {
            if (addButton.CanSelect)
                addButton.Select();
        }

        #endregion

        private void PopulateGrid()
        {
            gpuGrid.SuspendLayout();
            gpuGrid.Rows.Clear();
            gpuGrid.Cursor = Cursors.WaitCursor;
            try
            {
                foreach(var vGpuRef in vm.VGPUs)
                {
                    var vgpu = Connection.Resolve(vGpuRef);
                    if (vgpu != null)
                    {
                        currentGpus.Add(vgpu);
                        gpuGrid.Rows.Add(new VGpuDetailRow(vgpu));
                    }
                }

                DeviceColumn.Visible = vm.VGPUs.Count > 0 && Helpers.QuebecOrGreater(Connection);
            }
            finally
            {
                gpuGrid.ResumeLayout();
                gpuGrid.Cursor = Cursors.Default;
            }
        }

        public void ShowHideWarnings()
        {
            if (!gpusAvailable)
                return;

            var vGpus = VGpus;

            imgExperimental.Visible = labelExperimental.Visible =
                vGpus.Count > 0 && vGpus.Any(v => v.IsExperimental());

            if (vm.power_state != vm_power_state.Halted)
            {
                imgRDP.Visible = labelRDP.Visible =
                imgNeedDriver.Visible = labelNeedDriver.Visible =
                imgNeedGpu.Visible = labelNeedGpu.Visible =
                imgHA.Visible = labelHA.Visible =
                    false;

                imgStopVM.Visible = labelStopVM.Visible = true;

                addButton.Enabled = deleteButton.Enabled = false;
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

                addButton.Enabled = deleteButton.Enabled = false;
                return;
            }

            var multipleVgpuSupport = vGpus.All(v => { var x = Connection.Resolve(v.type); return x != null && x.compatible_types_in_vm.Count > 0; });
            addButton.Enabled = multipleVgpuSupport;
            deleteButton.Enabled = gpuGrid.SelectedRows.Count > 0;

            imgMulti.Visible = labelMulti.Visible = vGpus.Count > 0;
            if (vGpus.Count > 0)
                labelMulti.Text = multipleVgpuSupport ? Messages.NEWVMWIZARD_VGPUPAGE_MULTIPLE_VGPU_INFO : Messages.NEWVMWIZARD_VGPUPAGE_SINGLE_VGPU_INFO;

            imgStopVM.Visible = labelStopVM.Visible =
            imgHA.Visible = labelHA.Visible = false;

            imgRDP.Visible = labelRDP.Visible =
                HasChanged && currentGpus.Count == 0 &&
                vGpus.Count > 0 && vGpus.Any(v => v.IsPassthrough());

            imgNeedGpu.Visible = labelNeedGpu.Visible =
                labelNeedDriver.Visible = imgNeedDriver.Visible = vGpus.Count > 0;
        }

        private void warningsTable_SizeChanged(object sender, EventArgs e)
        {
            int[] columnsWidth = warningsTable.GetColumnWidths();
            int textColumnWidth = columnsWidth.Length > 1 ? columnsWidth[1] : 1;

            labelRDP.MaximumSize = labelStopVM.MaximumSize = new Size(textColumnWidth, 999);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new AddVGPUDialog(vm, VGpus))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var tuple = dialog.SelectedTuple;
                    if (tuple == null)
                        return;

                    VGPU_type type = tuple.VgpuTypes[0];
                    // Set vGPU device to null. vGPU creation will check whether a valid device is assigned
                    // to the vGPU, if not then 0 is used to declare the first availabe slot in VM
                    var vGpu = new VGPU();
                    vGpu.device = null;
                    vGpu.GPU_group = new XenRef<GPU_group>(tuple.GpuGroup.opaque_ref);
                    vGpu.type = new XenRef<VGPU_type>(type.opaque_ref);
                    vGpu.Connection = vm.Connection;
                    gpuGrid.Rows.Add(new VGpuDetailRow(vGpu));
                    warningsTable.SuspendLayout();
                    ShowHideWarnings();
                    warningsTable.ResumeLayout();
                }
            } 
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (gpuGrid.SelectedRows.Count == 0)
                return;

            gpuGrid.Rows.Remove(gpuGrid.SelectedRows[0]);
            warningsTable.SuspendLayout();
            ShowHideWarnings();
            warningsTable.ResumeLayout();
        }

    }

    public class VGpuDetailRow : DataGridViewExRow
    {
        private readonly DataGridViewTextBoxCell deviceColumn = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell nameColumn = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell vGpusPerGpuColumn = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell maxResolutionColumn = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell maxDisplaysColumn = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell videoRamColumn = new DataGridViewTextBoxCell();
        // Xapi reserves device numbers [0,20] for vGPU for backwards compatibility.
        // The guest PCI bus slots lie within [11,31], hence XenCenter needs to
        // shift the device number by 11 in order to show the actual PCI slot number used in the VM
        private const int VGPU_PCI_SLOT_NUMBER_SHIFT = 11;

        public VGPU VGpu { get; }

        public VGpuDetailRow(VGPU vGpu)
        {
            VGpu = vGpu;

            SetCells();
            Cells.AddRange(deviceColumn, nameColumn, vGpusPerGpuColumn, maxResolutionColumn, maxDisplaysColumn, videoRamColumn);
        }

        private void SetCells()
        {
            if (int.TryParse(VGpu.device, out var device))
                deviceColumn.Value = Helpers.QuebecOrGreater(VGpu.Connection) ? device + VGPU_PCI_SLOT_NUMBER_SHIFT : device;
            else
                deviceColumn.Value = Messages.HYPHEN;

            var vGpuType = VGpu.Connection.Resolve(VGpu.type);
            var gpuGroup = VGpu.Connection.Resolve(VGpu.GPU_group);

            bool isPassThru = vGpuType.IsPassthrough();

            nameColumn.Value = isPassThru 
                ? gpuGroup.HasVGpu() ? Messages.VGPU_PASSTHRU_TOSTRING : gpuGroup.Name()
                : vGpuType.model_name;

            if (!isPassThru)
                vGpusPerGpuColumn.Value = vGpuType.Capacity();
            else
                vGpusPerGpuColumn.Value = string.Empty;

            if (!isPassThru)
            {
                var maxRes = vGpuType.MaxResolution();
                maxResolutionColumn.Value = maxRes == "0x0" || String.IsNullOrEmpty(maxRes) ? "" : maxRes;
            }

            if (!isPassThru)
                maxDisplaysColumn.Value = vGpuType.max_heads < 1 ? "" : String.Format("{0}", vGpuType.max_heads);
            else
                maxDisplaysColumn.Value = string.Empty;

            videoRamColumn.Value = vGpuType.framebuffer_size != 0 ? Util.MemorySizeStringSuitableUnits(vGpuType.framebuffer_size, true) : string.Empty;
        }
    }
}
