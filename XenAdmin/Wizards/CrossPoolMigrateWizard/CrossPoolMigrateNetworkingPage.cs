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
using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{
    class CrossPoolMigrateNetworkingPage : SelectMultipleVMNetworkPage
    {
        private readonly bool templatesOnly = false;
        private readonly WizardMode wizardMode;

        public CrossPoolMigrateNetworkingPage(bool templatesOnly, WizardMode wizardMode)
        {
            this.templatesOnly = templatesOnly;
            this.wizardMode = wizardMode;

            InitializeText();
        }
        
        /// <summary>
        /// Gets the page's title (headline)
        /// </summary>
        public override string PageTitle { get { return Messages.CPM_WIZARD_SELECT_NETWORK_PAGE_TITLE; } }

        /// <summary>
        /// Gets the page's label in the (left hand side) wizard progress panel
        /// </summary>
        public override string Text { get { return Messages.CPM_WIZARD_SELECT_NETWORK_PAGE_TEXT; } }

        public override string IntroductionText
        {
            get 
            {
                if (templatesOnly)
                {
                    return 
                        VmMappings != null && VmMappings.Count > 1 ? Messages.CPM_WIZARD_NETWORKING_INTRO_TEMPLATE : Messages.CPM_WIZARD_NETWORKING_INTRO_TEMPLATE_SINGLE;
                }
                else
                {
                    return 
                        VmMappings != null && VmMappings.Count > 1 ? Messages.CPM_WIZARD_NETWORKING_INTRO : Messages.CPM_WIZARD_NETWORKING_INTRO_SINGLE;
                }
            }
        }

        public override string TableIntroductionText { get { return Messages.CPM_WIZARD_VM_SELECTION_INTRODUCTION; } }

        /// <summary>
        /// Gets the value by which the help files section for this page is identified
        /// </summary>
        public override string HelpID { get { return wizardMode == WizardMode.Copy ? "NetworkingCopyMode" : "Networking"; } }

        public override NetworkResourceContainer NetworkData(string sysId)
        {
            VM vm = Connection.Resolve(new XenRef<VM>(sysId));

            if(vm == null)
                return null;

            List<VIF> vifs = Connection.ResolveAll(vm.VIFs);
            return new CrossPoolMigrationNetworkResourceContainer(vifs);
        }

        protected override bool AllowSriovNetwork(XenAPI.Network network, string sysId)
        {
            VM vm = Connection.Resolve(new XenRef<VM>(sysId));
            if (vm == null)
                return false;

            if (vm.power_state != vm_power_state.Halted)
                return false;

            return vm.HasSriovRecommendation();
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (!CrossPoolMigrateWizard.AllVMsAvailable(VmMappings, Connection))
            {
                cancel = true;
                SetButtonNextEnabled(false);
                SetButtonPreviousEnabled(false);
            }
        }

        protected override string NetworkColumnHeaderText
        {
            get
            {
                return templatesOnly ? Messages.CPS_WIZARD_NETWORKING_NETWORK_COLUMN_TEMPLATE : Messages.CPS_WIZARD_NETWORKING_NETWORK_COLUMN_VM;
            }
        }
    }
}
