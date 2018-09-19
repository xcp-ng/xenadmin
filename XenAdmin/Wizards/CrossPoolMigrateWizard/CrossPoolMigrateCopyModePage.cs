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
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{
    public partial class CrossPoolMigrateCopyModePage : XenTabPage
    {
        private List<VM> selectedVMs;

        public CrossPoolMigrateCopyModePage(List<VM> selectedVMs)
        {
            this.selectedVMs = selectedVMs; 
            InitializeComponent();

            if (selectedVMs != null && selectedVMs.Count == 1 && selectedVMs[0] != null && selectedVMs[0].is_a_template)
                labelRubric.Text = Messages.COPY_VM_WIZARD_RUBRIC_TEMPLATE;
            else
                labelRubric.Text = Messages.COPY_VM_WIZARD_RUBRIC_VM;
        }

        private bool _buttonNextEnabled;

        #region Base class (XenTabPage) overrides

        /// <summary>
        /// Gets the page's title (headline)
        /// </summary>
        public override string PageTitle { get { return Messages.CPM_WIZARD_COPY_MODE_TITLE; } }

        /// <summary>
        /// Gets the page's label in the (left hand side) wizard progress panel
        /// </summary>
        public override string Text { get { return Messages.CPM_WIZARD_COPY_MODE_TAB_TITLE;  } }

        /// <summary>
        /// Gets the value by which the help files section for this page is identified
        /// </summary>
        public override string HelpID { get { return "CopyMode"; } }

        protected override bool ImplementsIsDirty()
        {
            return false;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            var crossPoolRestricted = Helpers.FeatureForbidden(Connection, Host.RestrictCrossPoolMigrate);
            warningsTable.Visible = crossPoolRestricted;
            crossPoolRadioButton.Enabled = crossPoolDescriptionLabel.Enabled = !crossPoolRestricted;
            SetButtonsEnabled(true);
        }

        public override void PopulatePage()
        {
            SetButtonsEnabled(true);
        }

        public bool IntraPoolCopySelected
        {
            get { return intraPoolRadioButton.Checked; }
        }

        public override bool EnableNext()
        {
            return _buttonNextEnabled;
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
            _buttonNextEnabled = enabled;
            OnPageUpdated();
        }
    }
}
