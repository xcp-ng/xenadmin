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
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public partial class AddVGPUDialog : XenDialogBase
    {
        private VM _vm;
        private List<VGPU> existingVGpus;
        public GpuTuple SelectedTuple { private set; get; }

        public AddVGPUDialog(VM vm, List<VGPU> VGpus)
        {
            _vm = vm;
            existingVGpus = VGpus;
            InitializeComponent();
            PopulateComboBox();
        }

        private void PopulateComboBox()
        {
            GPU_group[] gpu_groups = _vm.Connection.Cache.GPU_groups.Where(g => g.PGPUs.Count > 0 && g.supported_VGPU_types.Count != 0).ToArray();
            Array.Sort(gpu_groups);
            foreach (GPU_group gpu_group in gpu_groups)
            {
                if (Helpers.FeatureForbidden(_vm.Connection, Host.RestrictVgpu) || !_vm.CanHaveVGpu())
                {
                    if (gpu_group.HasPassthrough())
                        comboBoxTypes.Items.Add(new GpuTuple(gpu_group, null, null));  //GPU pass-through item
                }
                else
                {
                    HashSet<XenRef<VGPU_type>> commonTypesSet = new HashSet<XenRef<VGPU_type>>(gpu_group.supported_VGPU_types);

                    foreach (var existingVgpu in existingVGpus)
                    {
                        var existingType = _vm.Connection.Resolve(existingVgpu.type);
                        if (existingType != null && existingType.compatible_types_in_vm != null)
                        {
                            var compatibleTypesSet = new HashSet<XenRef<VGPU_type>>(existingType.compatible_types_in_vm);
                            commonTypesSet.IntersectWith(compatibleTypesSet);
                        }
                    }

                    var commonTypes = _vm.Connection.ResolveAll(commonTypesSet);
                    commonTypes.Sort();
                    commonTypes.Reverse();

                    if (gpu_group.HasVGpu() && commonTypes.Count > 0)
                        comboBoxTypes.Items.Add(
                            new GpuTuple(gpu_group, _vm.Connection.ResolveAll(gpu_group.supported_VGPU_types).ToArray())); // Group item

                    var disabledTypes = _vm.Connection.ResolveAll(gpu_group.supported_VGPU_types.Except(gpu_group.enabled_VGPU_types));

                    foreach (var vgpuType in commonTypes)
                        comboBoxTypes.Items.Add(new GpuTuple(gpu_group, vgpuType, disabledTypes.ToArray())); // GPU_type item

                    if (commonTypes.Count == 1 && !disabledTypes.Contains(commonTypes[0]))
                        comboBoxTypes.SelectedItem = comboBoxTypes.Items[0];
                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            GpuTuple tuple = comboBoxTypes.SelectedItem as GpuTuple;
            if (tuple == null || tuple.VgpuTypes == null || tuple.VgpuTypes.Length == 0)
                SelectedTuple = null;
            else
                SelectedTuple = tuple;
        }

        private void comboBoxTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonAdd.Enabled = true;
        }
    }
}
