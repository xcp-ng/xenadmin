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
using System.Xml;
using XenAdmin.Wizards.GenericPages;
using XenOvf.Definitions;
using XenAPI;
using XenOvf;


namespace XenAdmin.Wizards.ImportWizard
{
    class ImportSelectNetworkPage : SelectMultipleVMNetworkPage
    {
        /// <summary>
        /// Gets the page's title (headline)
        /// </summary>
        public override string PageTitle { get { return Messages.IMPORT_SELECT_NETWORK_PAGE_TITLE; } }

        /// <summary>
        /// Gets the page's label in the (left hand side) wizard progress panel
        /// </summary>
        public override string Text { get { return Messages.IMPORT_SELECT_NETWORK_PAGE_TEXT; } }

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        protected override bool AllowSriovNetwork(XenAPI.Network network, string sysId)
        {
            var vhs = OVF.FindVirtualHardwareSectionByAffinity(SelectedOvfEnvelope, sysId, "xen");
            var data = vhs.VirtualSystemOtherConfigurationData;

            foreach (var s in data)
            {
                if (s.Name == "recommendations")
                {
                    try
                    {
                        XmlDocument xdRecommendations = new XmlDocument();
                        xdRecommendations.LoadXml(s.Value.Value);

                        XmlNode xn = xdRecommendations.SelectSingleNode(@"restrictions/restriction[@field='allow-network-sriov']");
                        if (xn == null || xn.Attributes == null)
                            return false;
 
                        return Convert.ToInt32(xn.Attributes["value"].Value) != 0;
                    }
                    catch
                    {
                        return false;
                    }                    
                }
            }

            return false;
        }

        public EnvelopeType SelectedOvfEnvelope { private get; set; }

        public override string IntroductionText { get { return Messages.IMPORT_WIZARD_NETWORKING_INTRO; } }

        public override string TableIntroductionText { get { return Messages.IMPORT_WIZARD_VM_SELECTION_INTRODUCTION; } }

        public override NetworkResourceContainer NetworkData(string sysId)
        {
            return new OvfNetworkResourceContainer(sysId, SelectedOvfEnvelope);
        }
    }
}
