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
using XenAdmin.Controls;
using System.Linq;
using XenAPI;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{
    public partial class CrossPoolMigrateTransferNetworkPage : XenTabPage
    {
        private List<VM> selectedVMs;
        private readonly bool templatesOnly = false;
        private readonly WizardMode wizardMode;

        public CrossPoolMigrateTransferNetworkPage(List<VM> selectedVMs, bool templatesOnly, WizardMode wizardMode)
        {
            this.selectedVMs = selectedVMs;
            this.templatesOnly = templatesOnly;
            this.wizardMode = wizardMode;

            InitializeComponent();
            InitializeCustomPageElements();
        }

        private void InitializeCustomPageElements()
        {
            blurbText.Text = templatesOnly ? Messages.CPS_WIZARD_MIGRATION_PAGE_TITLE_TEMPLATE : Messages.CPS_WIZARD_MIGRATION_PAGE_TITLE_VM;

            networkComboBox.IncludePoolNameInComboBox = true;
            networkComboBox.IncludeOnlyEnabledNetworksInComboBox = true;
            networkComboBox.IncludeOnlyNetworksWithIPAddresses = true;
        }

        private bool m_buttonNextEnabled;
        private bool m_buttonPreviousEnabled;

        #region Base class (XenTabPage) overrides

        /// <summary>
        /// Gets the page's title (headline)
        /// </summary>
        public override string PageTitle { get { return Messages.CPM_WIZARD_SELECT_TRANSFER_NETWORK_TITLE; } }

        /// <summary>
        /// Gets the page's label in the (left hand side) wizard progress panel
        /// </summary>
        public override string Text { get { return Messages.CPM_WIZARD_SELECT_TRANSFER_NETWORK_PAGE_TEXT; } }

        /// <summary>
        /// Gets the value by which the help files section for this page is identified
        /// </summary>
        public override string HelpID { get { return wizardMode == WizardMode.Copy ? "TransferNetworkCopyMode" : "TransferNetwork"; } }

        protected override bool ImplementsIsDirty()
        {
            return false;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            SetButtonsEnabled(true);
        }

        public override void PopulatePage()
        {
            networkComboBox.PopulateComboBox(Connection, item => !item.IsManagement);

            if (networkComboBox.SelectedItem == null)
                networkComboBox.SelectedItem = networkComboBox.Items.Cast<NetworkComboBoxItem>().FirstOrDefault(item => item.IsManagement);
        }

        public KeyValuePair<string, string> NetworkUuid
        {
            get { return networkComboBox.SelectedNetworkUuid; }
        }

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

        public override bool EnablePrevious()
        {
            return m_buttonPreviousEnabled;
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (!CrossPoolMigrateWizard.AllVMsAvailable(selectedVMs))
            {
                cancel = true;
                SetButtonsEnabled(false);
            }
        }
        #endregion

        protected void SetButtonsEnabled(bool enabled)
        {
            m_buttonNextEnabled = enabled;
            m_buttonPreviousEnabled = enabled;
            OnPageUpdated();
        }
    }
}
