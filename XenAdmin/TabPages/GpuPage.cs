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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.Common;
using XenAdmin.Controls.GPU;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.TabPages
{
    public partial class GpuPage : BaseTabPage
    {
        private const int ROW_GAP = 5;

        public GpuPage()
        {
            InitializeComponent();
            Text = Messages.GPU;
            PGPU_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(PGPU_CollectionChanged);
        }

        private readonly CollectionChangeEventHandler PGPU_CollectionChangedWithInvoke;

        public override string HelpID => "TabPageGPU";

        private IXenObject xenObject;
        List<PGPU> pGPUs = new List<PGPU>();

        /// <summary>
        /// The object that the panel is displaying GPU info for. 
        /// </summary>
        public IXenObject XenObject
        {
            set
            {
                System.Diagnostics.Trace.Assert(value is Pool || value is Host);
                xenObject = value;

                Rebuild();
            }
        }

        private bool _rebuilding;

        private void Rebuild()
        {
            Program.AssertOnEventThread();
            if (!Visible)
                return;
            _rebuilding = true;
            pageContainerPanel.SuspendLayout();

            // Store a list of the current controls. We remove them at the end because it makes less flicker that way.
            List<Control> oldControls = new List<Control>(pageContainerPanel.Controls.Count);
            foreach (Control c in pageContainerPanel.Controls)
            {
                oldControls.Add(c);
            }

            // Group pGPUs with the same settings
            Dictionary<GpuSettings, List<PGPU>> settingsToPGPUs = new Dictionary<GpuSettings, List<PGPU>>();  // all PGPUs with a particular setting
            List<GpuSettings> listSettings = new List<GpuSettings>();  // also make a list of GpuSettings to preserve the order

            pGPUs.Clear();

            var allPgpus = xenObject.Connection.Cache.PGPUs;
            pGPUs.AddRange(from pGpu in allPgpus
                           let host = xenObject.Connection.Resolve(pGpu.host)
                           where pGpu.supported_VGPU_types.Count > 0 && (xenObject is Pool || xenObject == host)
                           orderby host, pGpu.Name() ascending
                           select pGpu
                );

            foreach (PGPU pGpu in pGPUs)
            {
                RegisterPgpuHandlers(pGpu);

                var enabledTypes = pGpu.Connection.ResolveAll(pGpu.enabled_VGPU_types);
                var supportedTypes = pGpu.Connection.ResolveAll(pGpu.supported_VGPU_types);

                var newSettings = new GpuSettings(enabledTypes.ToArray(), supportedTypes.ToArray(), pGpu.Name());
                
                var existingSettings = settingsToPGPUs.Keys.FirstOrDefault(ss => ss.Equals(newSettings));

                if (existingSettings == null) // we've not seen these settings on another pGPU
                {
                    settingsToPGPUs.Add(newSettings, new List<PGPU>());
                    listSettings.Add(newSettings);
                    existingSettings = newSettings;
                }
                settingsToPGPUs[existingSettings].Add(pGpu);
            }

            int initScroll = pageContainerPanel.VerticalScroll.Value;
            int top = pageContainerPanel.Padding.Top - initScroll;

            if (Helpers.VGpuCapability(xenObject.Connection))
                AddRowToPanel(CreateGpuPlacementPolicyPanel(), ref top);

            foreach (GpuSettings settings in listSettings)
            {
                AddRowToPanel(new GpuRow(xenObject, settingsToPGPUs[settings]), ref top);
            }

            if (listSettings.Count == 0)
                AddRowToPanel(CreateNoGpuPanel(), ref top);

            // Remove old controls
            foreach (Control c in oldControls)
            {
                pageContainerPanel.Controls.Remove(c);
                int scroll = initScroll;
                if (scroll > pageContainerPanel.VerticalScroll.Maximum)
                    scroll = pageContainerPanel.VerticalScroll.Maximum;
                pageContainerPanel.VerticalScroll.Value = scroll; 
                c.Dispose();
            }
            _rebuilding = false;
            pageContainerPanel.ResumeLayout();
            ReLayout();
        }

        private GpuPlacementPolicyPanel CreateGpuPlacementPolicyPanel()
        {
            return new GpuPlacementPolicyPanel
                       {
                           MinimumSize = new System.Drawing.Size(393, 35),
                           Name = "gpuPlacementPolicyPanel1",
                           XenObject = xenObject
                       };
        }

        private AutoHeightLabel CreateNoGpuPanel()
        {
            return new AutoHeightLabel
            {
                Name = "noGpuPanel1",
                Text = xenObject is Pool ? Messages.NO_GPU_IN_POOL : Messages.NO_GPU_ON_HOST
            };
        }

        private void ReLayout()
        {
            Program.AssertOnEventThread();
            if (_rebuilding)
                return;

            int initScroll = pageContainerPanel.VerticalScroll.Value;
            int top = pageContainerPanel.Padding.Top - initScroll;
            foreach (Control row in pageContainerPanel.Controls)
            {
                row.Top = top;
                top += row.Height + ROW_GAP;
            }
        }

        private void AddRowToPanel(Control row, ref int top)
        {
            row.Top = top;
            row.Left = pageContainerPanel.Padding.Left - pageContainerPanel.HorizontalScroll.Value;
            SetRowWidth(row);
            row.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            top += row.Height + ROW_GAP;
            row.Resize += row_Resize;
            pageContainerPanel.Controls.Add(row);
        }

        private GpuRow FindRow(PGPU pgpu)
        {
            return pgpu != null ? pageContainerPanel.Controls.OfType<GpuRow>().FirstOrDefault(row => row.PGPUs.Contains(pgpu)) : null;
        }

        private void pageContainerPanel_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control row in pageContainerPanel.Controls)
                SetRowWidth(row);
        }

        void row_Resize(object sender, EventArgs e)
        {
            ReLayout();
        }

        private void SetRowWidth(Control row)
        {
            row.Width = pageContainerPanel.Width - pageContainerPanel.Padding.Left - 25;  // It won't drop below row.MinimumSize.Width though
        }

        private void PGPU_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Remove)
            {
                PGPU pgpu = e.Element as PGPU;
                UnregisterPgpuHandlers(pgpu);
            }
            XenObject = xenObject;
        }

        private void pgpu_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "resident_VGPUs")
            {
                var pgpu = sender as PGPU;
                var gpuRow = FindRow(pgpu);
                if (gpuRow != null) 
                    gpuRow.RefreshGpu(pgpu);
            }

            if (e.PropertyName == "enabled_VGPU_types" || e.PropertyName == "supported_VGPU_types")
            {
                Rebuild();
            }
        }

        private void RegisterPgpuHandlers(PGPU pgpu)
        {
            pgpu.PropertyChanged -= pgpu_PropertyChanged;
            pgpu.PropertyChanged += pgpu_PropertyChanged;
        }

        private void UnregisterPgpuHandlers(PGPU pgpu)
        {
            pgpu.PropertyChanged -= pgpu_PropertyChanged;
        }

        private void RegisterHandlers()
        {
            if (xenObject == null)
                return;

            xenObject.Connection.Cache.DeregisterCollectionChanged<PGPU>(PGPU_CollectionChangedWithInvoke);
            xenObject.Connection.Cache.RegisterCollectionChanged<PGPU>(PGPU_CollectionChangedWithInvoke);

            foreach (PGPU pgpu in xenObject.Connection.Cache.PGPUs)
            {
                UnregisterPgpuHandlers(pgpu);
                RegisterPgpuHandlers(pgpu);
            }
        }

        private void GpuPage_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                RegisterHandlers();
            else
                UnregisterHandlers();
        }

        private void UnregisterHandlers()
        {
            if (xenObject == null)
                return;

            xenObject.Connection.Cache.DeregisterCollectionChanged<PGPU>(PGPU_CollectionChangedWithInvoke);

            foreach (PGPU pgpu in xenObject.Connection.Cache.PGPUs)
            {
                UnregisterPgpuHandlers(pgpu);
            }
        }

        public override void PageHidden()
        {
            UnregisterHandlers();

            var gpuPlacementPolicyPanel = pageContainerPanel.Controls.OfType<GpuPlacementPolicyPanel>().FirstOrDefault();
            if (gpuPlacementPolicyPanel != null)
                gpuPlacementPolicyPanel.UnregisterHandlers();
        }


        internal class GpuSettings : IEquatable<GpuSettings>
        {
            public readonly VGPU_type[] EnabledVgpuTypes;
            public readonly VGPU_type[] SupportedVgpuTypes;
            public readonly string GpuName;

            public GpuSettings(VGPU_type[] enabledVgpuTypes, VGPU_type[] supportedVgpuTypes, string name)
            {
                EnabledVgpuTypes = enabledVgpuTypes;
                Array.Sort(EnabledVgpuTypes);

                SupportedVgpuTypes = supportedVgpuTypes;
                Array.Sort(SupportedVgpuTypes);

                GpuName = name;
            }

            private static bool EqualArrays(VGPU_type[] x, VGPU_type[] y)
            {
                if ((x == null || x.Length == 0) &&
                    (y == null || y.Length == 0))
                    return true;

                if ((x == null || x.Length == 0) ||
                    (y == null || y.Length == 0))
                    return false;

                if (x.Length != y.Length)
                    return false;

                for (int i = 0; i < x.Length; i++)
                {
                    if (!x[i].Equals(y[i]))
                        return false;
                }

                return true;
            }


            public bool Equals(GpuSettings other)
            {
                return GpuName.Equals(other.GpuName) && EqualArrays(EnabledVgpuTypes, other.EnabledVgpuTypes)
                    && EqualArrays(SupportedVgpuTypes, other.SupportedVgpuTypes);
            }


            public override string ToString()
            {
                return string.Join(",", EnabledVgpuTypes.Select(t => t.model_name).ToArray());
            }

        }
    }
}
