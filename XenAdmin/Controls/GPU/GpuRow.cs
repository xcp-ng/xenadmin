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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;

namespace XenAdmin.Controls.GPU
{
    public partial class GpuRow : UserControl
    {
        public GpuRow()
        {
            InitializeComponent();
        }

        public GpuRow(IXenObject xenObject, List<PGPU> pGpuList)
            : this()
        {
            this.xenObject = xenObject;
            pGpuLabel.Text = pGpuList[0].Name();
            RepopulateAllowedTypes(pGpuList[0]);
            vGpuCapability = !Helpers.FeatureForbidden(xenObject, Host.RestrictVgpu) && pGpuList[0].HasVGpu();
            Rebuild(pGpuList);
            SetupPage();
        }

        private readonly IXenObject xenObject;

        private readonly Dictionary<PGPU, CheckBox> pGpus = new Dictionary<PGPU, CheckBox>();

        private readonly bool vGpuCapability;

        private void Rebuild(List<PGPU> pGpuList)
        {
            Program.AssertOnEventThread();
            if (!Visible)
                return;

            shinyBarsContainerPanel.SuspendLayout();
            
            // Store a list of the current controls. We remove them at the end because it makes less flicker that way.
            var oldControls = new List<Control>(shinyBarsContainerPanel.Controls.Count);
            oldControls.AddRange(shinyBarsContainerPanel.Controls.Cast<Control>());

            bool showingHostLabel = (xenObject is Pool) && xenObject.Connection.Cache.Hosts.Length > 1;

            int index = 1;
            XenRef<Host> hostRef = null;

            foreach (PGPU pgpu in pGpuList)
            {
                var host = xenObject.Connection.Resolve(pgpu.host);

                // add host label if needed
                if (showingHostLabel && (hostRef == null || pgpu.host.opaque_ref != hostRef.opaque_ref))
                {
                    AddHostLabel(new Label { Text = String.Format(Messages.GPU_ON_HOST_LABEL, host.Name())}, ref index);
                    hostRef = pgpu.host;
                }

                // add pGPU shiny bar
                var gpuShinyBar = new GpuShinyBar();
                var checkBox = (pGpuList.Count > 1 && vGpuCapability) ? new CheckBox() : null; 
                AddShinyBar(gpuShinyBar, checkBox, ref index);
                gpuShinyBar.Initialize(pgpu);

                pGpus.Add(pgpu, checkBox);
            }    
            
            // Remove old controls
            foreach (Control c in oldControls)
            {
                shinyBarsContainerPanel.Controls.Remove(c);
                c.Dispose();
            }
            shinyBarsContainerPanel.ResumeLayout();
            ReLayout();
        }

        private void AddShinyBar(UserControl shinyBar, CheckBox checkBox, ref int index)
        {
            if (checkBox != null)
            {
                shinyBarsContainerPanel.Controls.Add(checkBox, 0, index);
                checkBox.Dock = DockStyle.Top;
                checkBox.Margin = new Padding(6, 32, 0, 0);
                checkBox.CheckedChanged += CheckedChanged;
                checkBox.Checked = true;
            }

            shinyBarsContainerPanel.Controls.Add(shinyBar, 1, index);
            shinyBar.Dock = DockStyle.Fill;

            index++;
        }

        private void AddHostLabel(Label label, ref int index)
        {
            shinyBarsContainerPanel.Controls.Add(label, 0, index);
            shinyBarsContainerPanel.SetColumnSpan(label, 2); 
            label.Dock = DockStyle.Fill;
            index++;
        }

        private void ReLayout()
        {
            Program.AssertOnEventThread();
            Height = tableLayoutPanel1.Height + 2;
        }

        private void SetupPage()
        {
            multipleSelectionPanel.Visible = (pGpus.Count > 1) && vGpuCapability;
            editButton.Text = (pGpus.Count > 1) ? Messages.GPU_EDIT_ALLOWED_TYPES_MULTIPLE : Messages.GPU_EDIT_ALLOWED_TYPES_SINGLE;
            editButton.Visible = vGpuCapability;
        }

        private GpuShinyBar FindGpuShinyBar(PGPU pGpu)
        {
            return pGpu != null
                       ? shinyBarsContainerPanel.Controls.OfType<GpuShinyBar>().FirstOrDefault(
                           shinyBar => shinyBar.PGPU == pGpu)
                       : null;
        }

        public void RefreshGpu(PGPU pGpu)
        {
            Program.AssertOnEventThread();
            if (!Visible)
                return;

            GpuShinyBar gpuShinyBar = FindGpuShinyBar(pGpu);
            if (gpuShinyBar != null)
            {
                gpuShinyBar.Initialize(pGpu);
                gpuShinyBar.Refresh();
            }
        }

        public IEnumerable<PGPU> PGPUs
        {
            get
            {
                return pGpus.Keys;
            }
        }

        private void selectAllButton_Click(object sender, EventArgs e)
        {
            foreach (var checkBox in pGpus.Values.Where(checkBox => checkBox != null))
            {
                checkBox.Checked = true;
            }
        }

        private void clearAllButton_Click(object sender, EventArgs e)
        {
            foreach (var checkBox in pGpus.Values.Where(checkBox => checkBox != null))
            {
                checkBox.Checked = false;
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (pGpus.Count == 0)
                return;

            var selectedPGPUs = pGpus.Count > 1
                ? (from kvp in pGpus where kvp.Value != null && kvp.Value.Checked select kvp.Key).ToList()
                : new List<PGPU> {pGpus.Keys.ElementAt(0)};

            using (var dlog = new GpuConfiguration(selectedPGPUs))
                dlog.ShowDialog(Program.MainWindow);
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            editButton.Enabled = clearAllButton.Enabled =
                pGpus.Values.Any(checkBox => checkBox != null && checkBox.Checked);
            selectAllButton.Enabled =
                pGpus.Values.Any(checkBox => checkBox != null && !checkBox.Checked);
        }

        #region Allowed vGpu types
        private void RepopulateAllowedTypes(PGPU pGpu)
        {
            allowedTypesGrid.SuspendLayout();
            allowedTypesGrid.Rows.Clear();
            allowedTypesGrid.Cursor = Cursors.WaitCursor;
            try
            {
                allowedTypesGrid.Rows.Clear();
                if (pGpu.supported_VGPU_types != null && pGpu.supported_VGPU_types.Count > 0)
                {
                    allowedTypesGrid.Rows.AddRange((from vGpuTypeRef in pGpu.supported_VGPU_types
                                                    let vGpuType = pGpu.Connection.Resolve(vGpuTypeRef)
                                                    let enabledType = pGpu.enabled_VGPU_types.Contains(vGpuTypeRef)
                                                    orderby vGpuType descending
                                                    select new VGpuTypeRow(vGpuType, enabledType)).ToArray());
                }
                allowedTypesGrid.Height = allowedTypesGrid.Rows[0].Height * (allowedTypesGrid.RowCount);
            }
            finally
            {
                allowedTypesGrid.ResumeLayout();
                allowedTypesGrid.Cursor = Cursors.Default;
            }
        }
        #endregion
    }

    class VGpuTypeRow : DataGridViewExRow
    {
        public VGPU_type VGpuType;
        public bool EnabledType;

        private readonly DataGridViewExImageCell ImageCell = new DataGridViewExImageCell();
        private readonly DataGridViewTextBoxCell NameCell = new DataGridViewTextBoxCell();

        public VGpuTypeRow(VGPU_type vGpuType, bool enabledType)
        {
            VGpuType = vGpuType;
            EnabledType = enabledType;

            Cells.AddRange(ImageCell,
                           NameCell);

            UpdateDetails();
        }

        public void UpdateDetails()
        {
            ImageCell.Value = EnabledType ? Resources._000_Tick_h32bit_16 : Resources._000_Abort_h32bit_16;
            NameCell.Value = VGpuType.Name();
        }
    }
}
