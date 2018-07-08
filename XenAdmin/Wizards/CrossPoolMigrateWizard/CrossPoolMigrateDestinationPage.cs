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
using XenAdmin.Network;
using XenAdmin.Wizards.CrossPoolMigrateWizard.Filters;
using XenAdmin.Wizards.GenericPages;
using XenAPI;
using System.Linq;
using XenAdmin.Controls;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{
    internal class CrossPoolMigrateDestinationPage : SelectMultipleVMDestinationPage
    {
        private List<VM> selectedVMs;
        private WizardMode wizardMode;
        // A 2-level cache to store the result of CrossPoolMigrateCanMigrateFilter.
        // Cache structure is like: <vm-ref, <host-ref, fault-reason>>.
        private IDictionary<string, IDictionary<string, string>> migrateFilterCache = 
            new Dictionary<string, IDictionary<string, string>>();


        public CrossPoolMigrateDestinationPage()
            : this(null, WizardMode.Migrate, null)
        {
        }

        public CrossPoolMigrateDestinationPage(List<VM> selectedVMs, WizardMode wizardMode, List<IXenConnection> ignoredConnections)
        {
            this.selectedVMs = selectedVMs;
            this.wizardMode = wizardMode;
            this.ignoredConnections = ignoredConnections ?? new List<IXenConnection>();

            InitializeText();
        }

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        /// <summary>
        /// Gets the value by which the help files section for this page is identified
        /// </summary>
        public override string HelpID { get { return wizardMode == WizardMode.Copy ? "DestinationCopyMode" : "Destination"; } }

        /// <summary>
        /// Gets the page's title (headline)
        /// </summary>
        public override string PageTitle { get { return Messages.CPM_WIZARD_DESTINATION_TITLE; } }

        /// <summary>
        /// Gets the page's label in the (left hand side) wizard progress panel
        /// </summary>
        public override string Text { get { return Messages.CPM_WIZARD_DESTINATION_TAB_TITLE; } }

        private bool TemplatesOnly { get { return selectedVMs != null && selectedVMs.All(vm => vm.is_a_template); } }

        protected override string InstructionText 
        {
            get
            {
                if (TemplatesOnly)
                {
                    if (selectedVMs != null && selectedVMs.Count == 1)
                    {
                        return Messages.CPM_WIZARD_DESTINATION_INSTRUCTIONS_COPY_TEMPLATE_SINGLE;
                    }
                    else
                    {
                        return Messages.CPM_WIZARD_DESTINATION_INSTRUCTIONS_COPY_TEMPLATE;
                    }
                }
                else
                {
                    if (selectedVMs != null && selectedVMs.Count == 1)
                    {
                        if (wizardMode == WizardMode.Copy)
                            return Messages.CPM_WIZARD_DESTINATION_INSTRUCTIONS_COPY_SINGLE;

                        if (wizardMode == WizardMode.Move)
                            return Messages.CPM_WIZARD_DESTINATION_INSTRUCTIONS_MOVE_SINGLE;

                        return Messages.CPM_WIZARD_DESTINATION_INSTRUCTIONS_MIGRATE_SINGLE;
                    }

                    if (wizardMode == WizardMode.Copy)
                        return Messages.CPM_WIZARD_DESTINATION_INSTRUCTIONS_COPY;

                    if (wizardMode == WizardMode.Move)
                        return Messages.CPM_WIZARD_DESTINATION_INSTRUCTIONS_MOVE;

                    return Messages.CPM_WIZARD_DESTINATION_INSTRUCTIONS_MIGRATE;
                    
                }
            }
        }

        protected override string TargetServerText { get { return Messages.CPM_WIZARD_DESTINATION_DESTINATION; } }

        protected override string TargetServerSelectionIntroText { get { return Messages.CPM_WIZARD_DESTINATION_TABLE_INTRO; } }

        protected override DelayLoadingOptionComboBoxItem CreateDelayLoadingOptionComboBoxItem(IXenObject xenItem)
        {
            var filters = new List<ReasoningFilter>
            {
                new ResidentHostIsSameAsSelectionFilter(xenItem, selectedVMs),
                new CrossPoolMigrateCanMigrateFilter(xenItem, selectedVMs, wizardMode, migrateFilterCache),
                new WlbEnabledFilter(xenItem, selectedVMs)
            };
            return new DelayLoadingOptionComboBoxItem(xenItem, filters);
        }

        protected override List<ReasoningFilter> CreateTargetServerFilterList(IEnableableXenObjectComboBoxItem selectedItem, List<string> vmOpaqueRefs)
        {
            var filters = new List<ReasoningFilter>();

            if(selectedItem != null && vmOpaqueRefs != null && selectedVMs != null)
            {
                List<VM> vmList = new List<VM>();
                foreach (string opaqueRef in vmOpaqueRefs)
                    vmList.Add(selectedVMs.Find(vm => vm.opaque_ref == opaqueRef));

                filters.Add(new ResidentHostIsSameAsSelectionFilter(selectedItem.Item, vmList));
                filters.Add(new CrossPoolMigrateCanMigrateFilter(selectedItem.Item, vmList, wizardMode, migrateFilterCache));
                filters.Add(new WlbEnabledFilter(selectedItem.Item, vmList));
            } 

            return filters;
        }

        protected override bool PerformCheck()
        {
            if (ChosenItem != null && (ChosenItem.Connection == null || !ChosenItem.Connection.IsConnected))
            {
                CrossPoolMigrateWizard.ShowWarningMessageBox(Messages.CPM_WIZARD_ERROR_TARGET_DISCONNECTED);
                return false;
            }

            if (selectedVMs == null || selectedVMs.Count == 0 || Connection == null
                || selectedVMs.Any(vm => Connection.Resolve(new XenRef<VM>(vm)) == null))
            {
                CrossPoolMigrateWizard.ShowWarningMessageBox(Messages.CPM_WIZARD_VM_MISSING_ERROR);
                return false;
            }

            return true;
        }

        protected override string VmColumnHeaderText
        {
            get
            {
                return TemplatesOnly ? Messages.TEMPLATE : Messages.VM;
            }
        }

        protected override string TargetColumnHeaderText
        {
            get
            {
                return Messages.TARGET_SERVER;
            }
        }
    }
}
