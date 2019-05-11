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
using System.Linq;
using XenAdmin.Wizards.GenericPages;
using XenAPI;
using XenOvf.Definitions;

namespace XenAdmin.Wizards.ImportWizard.Filters
{
    public class HardwareCompatibilityFilter : ReasoningFilter
    {
        private List<Xen_ConfigurationSettingData_Type> _hardwarePlatformSettings = new List<Xen_ConfigurationSettingData_Type>();
        private List<Host> _hosts = new List<Host>();

        public HardwareCompatibilityFilter(IXenObject itemAddedToComboBox, List<Xen_ConfigurationSettingData_Type> hardwarePlatformSettings)
            : base(itemAddedToComboBox)
        {
            _hardwarePlatformSettings = hardwarePlatformSettings;

            if (itemAddedToComboBox is Host host)
                _hosts.Add(host);

            if (itemAddedToComboBox is Pool pool)
                _hosts.AddRange(pool.Connection.Cache.Hosts);
        }
    
        public override bool FailureFoundFor(IXenObject itemToFilterOn)
        {
            foreach (var setting in _hardwarePlatformSettings)
            {
                long hardwarePlatformVersion;
                if (!long.TryParse(setting.Value.Value, out hardwarePlatformVersion))
                    continue;

                if (_hosts.Count > 0)
                {
                    if (_hosts.Any(h => !h.virtual_hardware_platform_versions.Contains(hardwarePlatformVersion)))
                        return true;
                }
                else
                {
                    if (hardwarePlatformVersion > 0)
                        return true;
                }
            }

            return false;
        }

        public override string Reason => Messages.CPM_FAILURE_REASON_HARDWARE_PLATFORM;
    }
}