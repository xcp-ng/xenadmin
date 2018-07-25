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
using System.Drawing;
using System.Linq;
using System.Threading;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using System.Windows.Forms;
using XenAdmin.Wizards.PatchingWizard;
using XenAPI;

namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    public partial class RollingUpgradeWizard : UpdateUpgradeWizard
    {
        private readonly RollingUpgradeUpgradePage RollingUpgradeUpgradePage;
        private readonly RollingUpgradeWizardSelectPool RollingUpgradeWizardSelectPool;
        private readonly RollingUpgradeWizardPrecheckPage RollingUpgradeWizardPrecheckPage;
        private readonly RollingUpgradeWizardFirstPage RollingUpgradeWizardFirstPage;
        private readonly RollingUpgradeWizardInstallMethodPage RollingUpgradeWizardInstallMethodPage;
        private readonly RollingUpgradeWizardUpgradeModePage RollingUpgradeWizardUpgradeModePage;
        private readonly RollingUpgradeReadyToUpgradePage RollingUpgradeReadyToUpgradePage;

        public RollingUpgradeWizard()
        {
            InitializeComponent();

            RollingUpgradeUpgradePage = new RollingUpgradeUpgradePage();
            RollingUpgradeWizardSelectPool = new RollingUpgradeWizardSelectPool();
            RollingUpgradeWizardPrecheckPage = new RollingUpgradeWizardPrecheckPage();
            RollingUpgradeWizardFirstPage = new RollingUpgradeWizardFirstPage();
            RollingUpgradeWizardInstallMethodPage = new RollingUpgradeWizardInstallMethodPage();
            RollingUpgradeWizardUpgradeModePage = new RollingUpgradeWizardUpgradeModePage();
            RollingUpgradeReadyToUpgradePage = new RollingUpgradeReadyToUpgradePage();

            AddPage(RollingUpgradeWizardFirstPage);
            AddPage(RollingUpgradeWizardSelectPool);
            AddPage(RollingUpgradeWizardUpgradeModePage);
            AddPage(RollingUpgradeWizardPrecheckPage);
            //Here has to be inserted the installer location page if automatic
            AddPage(RollingUpgradeReadyToUpgradePage);
            AddPage(RollingUpgradeUpgradePage);
        }

        protected override void FinishWizard()
        {
            var brokenSRs = RollingUpgradeWizardSelectPool.SelectedMasters
                .Any(master => master != null && master.Connection.Cache.SRs.Any(sr => sr.IsBroken(true)));
            if(brokenSRs)
            {
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning,
                        Messages.BROKEN_SRS_AFTER_UPGRADE)))
                {
                    dlg.ShowDialog(Program.MainWindow);
                }
            }
            base.FinishWizard();
        }

        protected override void UpdateWizardContent(XenTabPage senderPage)
        {
            var prevPageType = senderPage.GetType();

            if (prevPageType == typeof(RollingUpgradeWizardSelectPool))
            {
                var selectedMasters = RollingUpgradeWizardSelectPool.SelectedMasters;
                RollingUpgradeWizardUpgradeModePage.SelectedMasters = selectedMasters;
                RollingUpgradeWizardPrecheckPage.SelectedMasters = selectedMasters;
                RollingUpgradeWizardInstallMethodPage.SelectedMasters = selectedMasters;
                RollingUpgradeReadyToUpgradePage.SelectedMasters = selectedMasters;

                var selectedPools = new List<Pool>();
                foreach (var master in selectedMasters)
                {
                    var pool = Helpers.GetPoolOfOne(master.Connection);
                    if (pool != null && !selectedPools.Contains(pool))
                        selectedPools.Add(pool);
                }

                RollingUpgradeUpgradePage.SelectedPools = selectedPools;
            }
            else if (prevPageType == typeof(RollingUpgradeWizardUpgradeModePage))
            {
                var manualModeSelected = RollingUpgradeWizardUpgradeModePage.ManualModeSelected;
                
                RemovePageAt(4);
                if (manualModeSelected)
                    AddPage(RollingUpgradeReadyToUpgradePage, 4);
                else
                    AddPage(RollingUpgradeWizardInstallMethodPage, 4);

                RollingUpgradeWizardPrecheckPage.ManualUpgrade = manualModeSelected;
                RollingUpgradeUpgradePage.ManualModeSelected = manualModeSelected;

                var applyUpdatesToNewVersion = RollingUpgradeWizardUpgradeModePage.ApplyUpdatesToNewVersion;
                RollingUpgradeWizardPrecheckPage.ApplyUpdatesToNewVersion = applyUpdatesToNewVersion;
                RollingUpgradeUpgradePage.ApplyUpdatesToNewVersion = applyUpdatesToNewVersion;
            }
            else if (prevPageType == typeof(RollingUpgradeWizardInstallMethodPage))
                RollingUpgradeUpgradePage.InstallMethodConfig = RollingUpgradeWizardInstallMethodPage.InstallMethodConfig;
            else if (prevPageType == typeof(RollingUpgradeWizardPrecheckPage))
                RollingUpgradeUpgradePage.PrecheckProblemsActuallyResolved = RollingUpgradeWizardPrecheckPage.PrecheckProblemsActuallyResolved;
        }

        protected override void OnShown(System.EventArgs e)
        {
            base.OnShown(e);

            if (Properties.Settings.Default.RollingUpgradeWizardShowFirstPage)
                NextStep();
        }

        private void RevertResolvedPreChecks()
        {
            var subActions = GetUnwindChangesActions(RollingUpgradeWizardPrecheckPage.PrecheckProblemsActuallyResolved);
            if (subActions.Count > 0)
            {
                using (MultipleAction multipleAction = new MultipleAction(xenConnection, Messages.REVERT_WIZARD_CHANGES,
                                                                          Messages.REVERTING_WIZARD_CHANGES,
                                                                          Messages.REVERTED_WIZARD_CHANGES,
                                                                          subActions, false, true))
                {
                    using (var dialog = new ActionProgressDialog(multipleAction, ProgressBarStyle.Blocks))
                        dialog.ShowDialog(Program.MainWindow);
                }
            }
        }

        protected override void OnCancel()
        {
            if (RollingUpgradeUpgradePage.Status == Status.NotStarted)
                RevertResolvedPreChecks();

            base.OnCancel();
        }

        private void ShowCanBeResumedInfo()
        {
            using (var dialog = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Information,
                                                                Messages.ROLLING_UPGRADE_CAN_RESUME_UPGRADE,
                                                                Messages.ROLLING_POOL_UPGRADE)))
            {
                dialog.ShowDialog(Program.MainWindow);
            }
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);

            if (RollingUpgradeUpgradePage.Status == Status.Cancelled)
                ThreadPool.QueueUserWorkItem(o => Program.Invoke(Program.MainWindow, ShowCanBeResumedInfo));
        }
    }
}