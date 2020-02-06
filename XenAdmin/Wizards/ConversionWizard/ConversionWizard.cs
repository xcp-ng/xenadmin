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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.XCM;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Mappings;
using Tuple = System.Collections.Generic.KeyValuePair<string, string>;


namespace XenAdmin.Wizards.ConversionWizard
{
    public class ConversionWizard : XenWizardBase
    {
        private readonly CredentialsPage pageCredentials;
        private readonly VmSelectionPage pageVmSelection;
        private readonly SrSelectionPage pageSrSelection;
        private readonly ConversionNetworkPage pageNetworkOptions;
        private readonly SummaryPage pageSummary;

        private readonly ConversionClient _conversionClient;

        private Dictionary<string, VmMapping> _vmMappings = new Dictionary<string, VmMapping>();

        public ConversionWizard(ConversionClient client)
            : base(client.Connection)
        {
            pictureBoxWizard.Image = Images.StaticImages.ConversionManager_32;
           
            _conversionClient = client;

            pageCredentials = new CredentialsPage {ConversionClient = _conversionClient};
            pageVmSelection = new VmSelectionPage {ConversionClient = _conversionClient};
            pageSrSelection = new SrSelectionPage {ConversionClient = _conversionClient};
            pageNetworkOptions = new ConversionNetworkPage {ConversionClient = _conversionClient, VmMappings = _vmMappings};
            pageSummary = new SummaryPage {SummaryRetreiver = GetSummary};

            AddPages(pageCredentials, pageVmSelection, pageSrSelection, pageNetworkOptions, pageSummary);
        }

        public List<ConversionConfig> ConversionConfigs { get; private set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = Messages.CONVERSION_WIZARD_TEXT;
        }

        protected override void UpdateWizardContent(XenTabPage page)
        {
            if (page == pageCredentials)
            {
                pageVmSelection.VMwareVMs = pageCredentials.VMwareVMs;
                pageVmSelection.VmwareCredInfo = pageCredentials.VmwareCredInfo;
                pageNetworkOptions.VmwareCredInfo = pageCredentials.VmwareCredInfo;
            }
            else if (page == pageVmSelection)
            {
                pageSrSelection.SelectedVms = pageVmSelection.SelectedVms;
            }
        }

        protected override void FinishWizard()
        {
            ConversionConfigs = new List<ConversionConfig>();

            foreach (var vm in pageVmSelection.SelectedVms)
            {
                ConversionConfigs.Add(new ConversionConfig
                {
                    SourceVmName = vm.Name,
                    SourceVmUUID = vm.UUID,
                    PreserveMAC = pageNetworkOptions.PreserveMAC,
                    NetworkMappings = ConversionConfig.DictionaryToStruct(pageNetworkOptions.RawMappings),
                    SourceServer = pageCredentials.VmwareCredInfo,
                    StorageMapping = new StorageMapping {SRuuid = pageSrSelection.SelectedSR.uuid}
                });
            }

            base.FinishWizard();
        }

        protected override IEnumerable<Tuple> GetSummary()
        {
            return new List<Tuple>
            {
                new Tuple(Messages.CONVERSION_SUMMARY_DATA_VMWARE_SERVER, pageCredentials.VmWareServer),
                new Tuple(Messages.CONVERSION_SUMMARY_DATA_VMS,
                    string.Join(",", pageVmSelection.SelectedVms.Select(vm => vm.Name))),
                new Tuple(Messages.CONVERSION_SUMMARY_DATA_SR, pageSrSelection.SelectedSR.Name()),
                new Tuple(Messages.CONVERSION_SUMMARY_DATA_NETWORK, string.Join("\r\n",
                    from string key in pageNetworkOptions.RawMappings.Keys
                    select $"{key}: {pageNetworkOptions.RawMappings[key]}")),
                new Tuple(Messages.CONVERSION_SUMMARY_DATA_MAC, pageNetworkOptions.PreserveMAC.ToYesNoStringI18n())
            };
        }
    }
}
