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


namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    public partial class RollingUpgradeWizardUpgradeModePage : XenTabPage
    {
        public RollingUpgradeWizardUpgradeModePage()
        {
            InitializeComponent();
        }
        
        #region XenTabPage overrides
        public override string Text
        {
            get { return Messages.ROLLING_UPGRADE_TITLE_MODE; }
        }

        public override string PageTitle
        {
            get
            {
                return Messages.ROLLING_UPGRADE_MODE_PAGE; 
            }
        }

        public override string HelpID
        {
            get { return "Upgrademode"; }
        }

        public override string NextText(bool isLastPage)
        {
            return Messages.RUN_PRECHECKS_WITH_ACCESS_KEY;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            var licensedPoolCount = 0;
            var poolCount = 0;
            foreach (Host master in SelectedMasters)
            {
                var hosts = master.Connection.Cache.Hosts;

                if (hosts.Length == 0)
                    continue;

                poolCount++;
                var automatedUpdatesRestricted = hosts.Any(h => Helpers.DundeeOrGreater(h) && Host.RestrictBatchHotfixApply(h)); //if any host is not licensed for automated updates
                if (!automatedUpdatesRestricted)
                    licensedPoolCount++;
            }

            if (licensedPoolCount > 0) // at least one pool licensed for automated updates 
            {
                applyUpdatesCheckBox.Visible = applyUpdatesLabel.Visible = true;
                applyUpdatesCheckBox.Text = poolCount == licensedPoolCount
                    ? Messages.PATCHINGWIZARD_SELECTSERVERPAGE_APPLY_UPDATES
                    : Messages.PATCHINGWIZARD_SELECTSERVERPAGE_APPLY_UPDATES_MIXED;
            }
            else  // all pools unlicensed
            {
                applyUpdatesCheckBox.Visible = applyUpdatesLabel.Visible = false;
            }
        }
        #endregion

        #region Accessors
        public IEnumerable<Host> SelectedMasters { private get; set; }

        public bool ManualModeSelected
        {
            get { return radioButtonManual.Checked; }
        }

        public bool ApplyUpdatesToNewVersion
        {
            get
            {
                return applyUpdatesCheckBox.Visible && applyUpdatesCheckBox.Checked;
            }
        }
        #endregion
    }
}
