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

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;

namespace XenAdmin.SettingsPanels
{
    public partial class PoolGpuEditPage : UserControl, IEditPage
    {
        public Pool pool;
        private GPU_group[] gpu_groups;
        private allocation_algorithm currentAllocationAlgorithm;

        private bool showIntegratedGpu;
        private bool showPlacementPolicy;

        private Host host;
        private bool integratedGpuCurrentlyEnabled;
        private bool integratedGpuCurrentlyEnabledOnNextReboot;

        public PoolGpuEditPage()
        {
            InitializeComponent();
        }

        public allocation_algorithm AllocationAlgorithm
        {
            get
            {
                return radioButtonDepth.Checked ?
                    allocation_algorithm.depth_first :
                    radioButtonBreadth.Checked ? allocation_algorithm.breadth_first : allocation_algorithm.unknown;
            }
        }

        public bool EnableIntegratedGpuOnNextReboot
        {
            get
            {
                return radioButtonEnable.Checked;
            }
        }

        #region Implementation of IVerticalTab

        public override string Text
        {
            get { return Messages.GPU; }
        }

        public string SubText
        {
            get
            {
                var text = "";
                if (showPlacementPolicy)
                {
                    text = radioButtonDepth.Checked
                               ? Messages.GPU_PLACEMENT_POLICY_MAX_DENSITY
                               : radioButtonBreadth.Checked
                                     ? Messages.GPU_PLACEMENT_POLICY_MAX_PERFORMANCE
                                     : Messages.GPU_PLACEMENT_POLICY_MIXED;
                }
                if (showIntegratedGpu)
                {
                    var integratedGpuText = integratedGpuCurrentlyEnabled
                                                   ? Messages.INTEGRATED_GPU_PASSTHROUGH_ENABLED_SHORT
                                                   : Messages.INTEGRATED_GPU_PASSTHROUGH_DISABLED_SHORT;
                    text = showPlacementPolicy ? string.Join("; ", text, integratedGpuText) : integratedGpuText;
                }
                return text;
            }
        }

        public Image Image
        {
            get { return Resources._000_GetMemoryInfo_h32bit_16; }
        }

        #endregion

        #region Implementation of IEditPage

        public AsyncAction SaveSettings()
        {
            List<AsyncAction> actions = new List<AsyncAction>();

            if (HasPlacementPolicyChanged)
                actions.Add(new SetGpuPlacementPolicyAction(pool, AllocationAlgorithm));

            if (HasIntegratedGpuChanged && host != null)
                actions.Add(new UpdateIntegratedGpuPassthroughAction(host, EnableIntegratedGpuOnNextReboot, true));

            if (actions.Count == 0)
                return null;

            return actions.Count == 1 ? actions[0] : new MultipleAction(pool.Connection, "", "", "", actions, true);
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            Trace.Assert(clone is Pool || clone is Host);  // only Pools and Hosts should show this page

            pool = Helpers.GetPoolOfOne(clone.Connection);
            gpu_groups = pool.Connection.Cache.GPU_groups;

            bool isPoolOrStandaloneHost = clone is Pool || Helpers.GetPool(clone.Connection) == null;
            showPlacementPolicy = isPoolOrStandaloneHost && Helpers.VGpuCapability(clone.Connection);

            host = clone as Host; // can be null, if we are on a Pool
            showIntegratedGpu = host != null && host.CanEnableDisableIntegratedGpu();

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
            get { return HasPlacementPolicyChanged || HasIntegratedGpuChanged; }
        }

        #endregion

        private bool HasPlacementPolicyChanged
        {
            get
            {
                if (!showPlacementPolicy)
                    return false;

                var allocationAlgorithm = radioButtonDepth.Checked ? 
                    allocation_algorithm.depth_first : 
                    radioButtonBreadth.Checked ? allocation_algorithm.breadth_first : allocation_algorithm.unknown;

                return !allocationAlgorithm.Equals(currentAllocationAlgorithm);
            }
        }

        private bool HasIntegratedGpuChanged
        {
            get
            {
                if (!showIntegratedGpu)
                    return false;

                return EnableIntegratedGpuOnNextReboot != integratedGpuCurrentlyEnabledOnNextReboot;
            }
        }


        private void PopulatePage()
        {
            groupBoxPlacementPolicy.Visible = showPlacementPolicy;
            groupBoxIntedratedGpu.Visible = showIntegratedGpu;
            if (showPlacementPolicy)
                PopulatePlacementPolicyPanel();
            if (showIntegratedGpu)
                PopulateIntegratedGpuPanel();
        }

        private void PopulatePlacementPolicyPanel()
        {
            currentAllocationAlgorithm = allocation_algorithm.unknown;

            foreach (GPU_group gpu_group in gpu_groups)
            {
                if (gpu_group.allocation_algorithm != currentAllocationAlgorithm && currentAllocationAlgorithm != allocation_algorithm.unknown)
                {
                    // a mixture of allocation algorithms
                    currentAllocationAlgorithm = allocation_algorithm.unknown;
                    break;
                }
                currentAllocationAlgorithm = gpu_group.allocation_algorithm;
            }

            radioButtonDepth.Checked = currentAllocationAlgorithm == allocation_algorithm.depth_first;
            radioButtonBreadth.Checked = currentAllocationAlgorithm == allocation_algorithm.breadth_first;

            radioButtonMixture.Checked = currentAllocationAlgorithm == allocation_algorithm.unknown;
            radioButtonMixture.Visible = currentAllocationAlgorithm == allocation_algorithm.unknown;
            radioButtonMixture.Enabled = currentAllocationAlgorithm == allocation_algorithm.unknown;
        }

        private void PopulateIntegratedGpuPanel()
        {
            if (host == null)
                return;
            var currentHostDisplay = host.display;
            var systemDisplayDeviceGpu = host.SystemDisplayDevice();
            var currentPgpuDom0Access = systemDisplayDeviceGpu != null ? systemDisplayDeviceGpu.dom0_access : pgpu_dom0_access.unknown;

            bool hostCurrentlyEnabled = currentHostDisplay == host_display.enabled || currentHostDisplay == host_display.disable_on_reboot;
            bool hostEnabledOnNextReboot = currentHostDisplay == host_display.enabled || currentHostDisplay == host_display.enable_on_reboot;

            bool gpuCurrentlyEnabled = currentPgpuDom0Access == pgpu_dom0_access.enabled || currentPgpuDom0Access == pgpu_dom0_access.disable_on_reboot;
            bool gpuEnabledOnNextReboot = currentPgpuDom0Access == pgpu_dom0_access.enabled || currentPgpuDom0Access == pgpu_dom0_access.enable_on_reboot;

            integratedGpuCurrentlyEnabled = hostCurrentlyEnabled && gpuCurrentlyEnabled;
            integratedGpuCurrentlyEnabledOnNextReboot = hostEnabledOnNextReboot && gpuEnabledOnNextReboot;

            labelCurrentState.Text = (integratedGpuCurrentlyEnabled)
                                         ? Messages.INTEGRATED_GPU_PASSTHROUGH_ENABLED
                                         : Messages.INTEGRATED_GPU_PASSTHROUGH_DISABLED;

            radioButtonEnable.Checked = integratedGpuCurrentlyEnabledOnNextReboot;
            radioButtonDisable.Checked = !integratedGpuCurrentlyEnabledOnNextReboot;
        }
    }
}
