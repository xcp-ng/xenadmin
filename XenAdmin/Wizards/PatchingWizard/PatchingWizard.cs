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
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Dialogs;
using XenAPI;
using System.Linq;
using System.IO;
using XenAdmin.Alerts;
using XenAdmin.Core;
using XenAdmin.Wizards.PatchingWizard.PlanActions;

namespace XenAdmin.Wizards.PatchingWizard
{
    public enum WizardMode { SingleUpdate, AutomatedUpdates, NewVersion }
    
    /// <summary>
    /// Remember that equals for patches don't work across connections because 
    /// we are not allow to override equals. YOU SHOULD NOT USE ANY OPERATION THAT IMPLIES CALL EQUALS OF Pool_patch or Host_patch
    /// You should do it manually or use delegates.
    /// </summary>
    public partial class PatchingWizard : UpdateUpgradeWizard
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PatchingWizard_PatchingPage PatchingWizard_PatchingPage;
        private readonly PatchingWizard_SelectPatchPage PatchingWizard_SelectPatchPage;
        private readonly PatchingWizard_ModePage PatchingWizard_ModePage;
        private readonly PatchingWizard_SelectServers PatchingWizard_SelectServers;
        private readonly PatchingWizard_UploadPage PatchingWizard_UploadPage;
        private readonly PatchingWizard_PrecheckPage PatchingWizard_PrecheckPage;
        private readonly PatchingWizard_FirstPage PatchingWizard_FirstPage;
        private readonly PatchingWizard_AutomatedUpdatesPage PatchingWizard_AutomatedUpdatesPage;

        public PatchingWizard()
        {
            InitializeComponent();

            PatchingWizard_PatchingPage = new PatchingWizard_PatchingPage();
            PatchingWizard_SelectPatchPage = new PatchingWizard_SelectPatchPage();
            PatchingWizard_ModePage = new PatchingWizard_ModePage();
            PatchingWizard_SelectServers = new PatchingWizard_SelectServers();
            PatchingWizard_UploadPage = new PatchingWizard_UploadPage();
            PatchingWizard_PrecheckPage = new PatchingWizard_PrecheckPage();
            PatchingWizard_FirstPage = new PatchingWizard_FirstPage();
            PatchingWizard_AutomatedUpdatesPage = new PatchingWizard_AutomatedUpdatesPage();

            AddPage(PatchingWizard_FirstPage);
            AddPage(PatchingWizard_SelectPatchPage);
            AddPage(PatchingWizard_SelectServers);
            AddPage(PatchingWizard_UploadPage);
            AddPage(PatchingWizard_PrecheckPage);
            AddPage(PatchingWizard_ModePage);
            AddPage(PatchingWizard_PatchingPage);
        }

        public void AddAlert(XenServerPatchAlert alert)
        {
            PatchingWizard_SelectPatchPage.SelectDownloadAlert(alert);
            PatchingWizard_SelectPatchPage.UpdateAlertFromWeb = alert;
            PatchingWizard_SelectServers.UpdateAlertFromWeb = alert;
            PatchingWizard_PrecheckPage.UpdateAlert = alert;
            PatchingWizard_UploadPage.SelectedUpdateAlert = alert;
        }

        public void AddFile(string path)
        {
            PatchingWizard_SelectPatchPage.FilePath = path;
        }

        public void SelectServers(List<Host> selectedServers)
        {
            PatchingWizard_SelectServers.SelectServers(selectedServers);
            PatchingWizard_SelectServers.DisableUnselectedServers();
        }

        protected override void UpdateWizardContent(XenTabPage senderPage)
        {
            var prevPageType = senderPage.GetType();

            if (prevPageType == typeof(PatchingWizard_SelectPatchPage))
            {
                var wizardMode = PatchingWizard_SelectPatchPage.WizardMode;
                var wizardIsInAutomatedUpdatesMode = wizardMode == WizardMode.AutomatedUpdates;

                var updateType = wizardIsInAutomatedUpdatesMode ? UpdateType.Legacy : PatchingWizard_SelectPatchPage.SelectedUpdateType;
                var selectedPatchFilePath = wizardIsInAutomatedUpdatesMode ? null : PatchingWizard_SelectPatchPage.SelectedPatchFilePath;
                var alertFromWeb = wizardIsInAutomatedUpdatesMode ? null : PatchingWizard_SelectPatchPage.UpdateAlertFromWeb;
                var alertFromFileOnDisk = wizardIsInAutomatedUpdatesMode ? null : PatchingWizard_SelectPatchPage.AlertFromFileOnDisk;
                var fileFromDiskHasUpdateXml = !wizardIsInAutomatedUpdatesMode && PatchingWizard_SelectPatchPage.FileFromDiskHasUpdateXml;

                PatchingWizard_SelectServers.WizardMode = wizardMode;
                PatchingWizard_SelectServers.SelectedUpdateType = updateType;
                PatchingWizard_SelectServers.UpdateAlertFromWeb = alertFromWeb;
                PatchingWizard_SelectServers.AlertFromFileOnDisk = alertFromFileOnDisk;
                PatchingWizard_SelectServers.FileFromDiskHasUpdateXml = fileFromDiskHasUpdateXml;

                RemovePage(PatchingWizard_UploadPage);
                RemovePage(PatchingWizard_ModePage);
                RemovePage(PatchingWizard_PatchingPage);
                RemovePage(PatchingWizard_AutomatedUpdatesPage);
                if (wizardMode == WizardMode.SingleUpdate)
                {
                    AddAfterPage(PatchingWizard_SelectServers, PatchingWizard_UploadPage);
                    AddAfterPage(PatchingWizard_PrecheckPage, PatchingWizard_ModePage);
                    AddAfterPage(PatchingWizard_ModePage, PatchingWizard_PatchingPage);
                }
                else // AutomatedUpdates or NewVersion
                {
                    AddAfterPage(PatchingWizard_PrecheckPage, PatchingWizard_AutomatedUpdatesPage);
                }

                PatchingWizard_UploadPage.SelectedUpdateType = updateType;
                PatchingWizard_UploadPage.SelectedPatchFilePath = selectedPatchFilePath;
                PatchingWizard_UploadPage.SelectedUpdateAlert = alertFromWeb ?? alertFromFileOnDisk;
                PatchingWizard_UploadPage.PatchFromDisk = PatchingWizard_SelectPatchPage.PatchFromDisk;

                PatchingWizard_ModePage.SelectedUpdateType = updateType;

                PatchingWizard_PrecheckPage.WizardMode = wizardMode;
                PatchingWizard_PrecheckPage.PoolUpdate = null; //reset the PoolUpdate property; it will be updated on leaving the Upload page, if this page is visible
                PatchingWizard_PrecheckPage.UpdateAlert = alertFromWeb ?? alertFromFileOnDisk;

                PatchingWizard_AutomatedUpdatesPage.WizardMode = wizardMode;
                PatchingWizard_AutomatedUpdatesPage.UpdateAlert = alertFromWeb ?? alertFromFileOnDisk;
                PatchingWizard_AutomatedUpdatesPage.PatchFromDisk = PatchingWizard_SelectPatchPage.PatchFromDisk;

                PatchingWizard_PatchingPage.SelectedUpdateType = updateType;
                PatchingWizard_PatchingPage.SelectedPatchFilePatch = selectedPatchFilePath;
            }
            else if (prevPageType == typeof(PatchingWizard_SelectServers))
            {
                var selectedServers = PatchingWizard_SelectServers.SelectedServers;
                var selectedPools = PatchingWizard_SelectServers.SelectedPools;
                var applyUpdatesToNewVersion = PatchingWizard_SelectServers.ApplyUpdatesToNewVersion;

                PatchingWizard_PrecheckPage.SelectedServers = selectedServers;
                PatchingWizard_PrecheckPage.ApplyUpdatesToNewVersion = applyUpdatesToNewVersion;

                PatchingWizard_ModePage.SelectedPools = selectedPools;
                PatchingWizard_ModePage.SelectedServers = selectedServers;

                PatchingWizard_PatchingPage.SelectedServers = selectedServers;
                PatchingWizard_PatchingPage.SelectedPools = selectedPools;

                PatchingWizard_UploadPage.SelectedServers = selectedServers;
                PatchingWizard_UploadPage.SelectedPools = selectedPools;

                PatchingWizard_AutomatedUpdatesPage.SelectedPools = selectedPools;
                PatchingWizard_AutomatedUpdatesPage.ApplyUpdatesToNewVersion = applyUpdatesToNewVersion;
            }
            else if (prevPageType == typeof(PatchingWizard_UploadPage))
            {
                var patch = PatchingWizard_UploadPage.Patch;
                var update = PatchingWizard_UploadPage.PoolUpdate;
                var suppPackVdis = PatchingWizard_UploadPage.SuppPackVdis;

                PatchingWizard_PrecheckPage.Patch = patch;
                PatchingWizard_PrecheckPage.PoolUpdate = update;

                var srsWithUploadedUpdates = new Dictionary<Pool_update, Dictionary<Host, SR>>();
                foreach (var mapping in PatchingWizard_UploadPage.PatchMappings)
                {
                    if (mapping is PoolUpdateMapping pum)
                        srsWithUploadedUpdates[pum.Pool_update] = pum.SrsWithUploadedUpdatesPerHost;
                    else if (mapping is SuppPackMapping spm && spm.Pool_update != null)
                        srsWithUploadedUpdates[spm.Pool_update] = spm.SrsWithUploadedUpdatesPerHost;
                }
                PatchingWizard_PrecheckPage.SrUploadedUpdates = srsWithUploadedUpdates;

                PatchingWizard_ModePage.Patch = patch;
                PatchingWizard_ModePage.PoolUpdate = update;

                PatchingWizard_PatchingPage.Patch = patch;
                PatchingWizard_PatchingPage.PoolUpdate = update;
                PatchingWizard_PatchingPage.SuppPackVdis = suppPackVdis;
            }
            else if (prevPageType == typeof(PatchingWizard_ModePage))
            {
                PatchingWizard_PatchingPage.ManualTextInstructions = PatchingWizard_ModePage.ManualTextInstructions;
                PatchingWizard_PatchingPage.IsAutomaticMode = PatchingWizard_ModePage.IsAutomaticMode;
                PatchingWizard_PatchingPage.RemoveUpdateFile = PatchingWizard_ModePage.RemoveUpdateFile;
            }
            else if (prevPageType == typeof(PatchingWizard_PrecheckPage))
            {
                PatchingWizard_PatchingPage.PrecheckProblemsActuallyResolved = PatchingWizard_PrecheckPage.PrecheckProblemsActuallyResolved;
                PatchingWizard_PatchingPage.LivePatchCodesByHost = PatchingWizard_PrecheckPage.LivePatchCodesByHost;
                PatchingWizard_ModePage.LivePatchCodesByHost = PatchingWizard_PrecheckPage.LivePatchCodesByHost;
                PatchingWizard_AutomatedUpdatesPage.PrecheckProblemsActuallyResolved = PatchingWizard_PrecheckPage.PrecheckProblemsActuallyResolved;
            }
        }
        
        protected override void OnCancel(ref bool cancel)
        {
            base.OnCancel(ref cancel);

            if (cancel)
                return;

            RunMultipleActions(Messages.REVERT_WIZARD_CHANGES, Messages.REVERTING_WIZARD_CHANGES,
                Messages.REVERTED_WIZARD_CHANGES,
                GetUnwindChangesActions(PatchingWizard_PrecheckPage.PrecheckProblemsActuallyResolved));

            CleanUploadedPatches(true);
            RemoveDownloadedPatches();
        }
        
        protected override void FinishWizard()
        {
            CleanUploadedPatches();
            RemoveDownloadedPatches();
            Updates.CheckServerPatches();
            base.FinishWizard();
        }

        private void CleanUploadedPatches(bool forceCleanSelectedPatch = false)
        {
            var list = new List<AsyncAction>();

            foreach (var mapping in PatchingWizard_UploadPage.PatchMappings)
            {
                Pool_patch patch = null;
                if (mapping is PoolPatchMapping patchMapping)
                    patch = patchMapping.Pool_patch;
                else if (mapping is OtherLegacyMapping legacyMapping)
                    patch = legacyMapping.Pool_patch;

                if (patch != null)
                {
                    // exclude the selected patch; either the user wants to keep it or it has already been cleared in the patching page
                    if (PatchingWizard_UploadPage.Patch == null ||
                        !string.Equals(patch.uuid, PatchingWizard_UploadPage.Patch.uuid, StringComparison.OrdinalIgnoreCase) ||
                        forceCleanSelectedPatch)
                    {
                        var action = GetCleanActionForPoolPatch(patch);
                        if (action != null)
                            list.Add(action);
                    }
                    continue;
                }
                
                if (mapping is PoolUpdateMapping updateMapping)
                {
                    var action = GetCleanActionForPoolUpdate(updateMapping.Pool_update);
                    if (action != null)
                        list.Add(action);
                    continue;
                }
                
                if (mapping is SuppPackMapping suppPackMapping)
                {
                    if (suppPackMapping.Pool_update!= null)
                    {
                        var action = GetCleanActionForPoolUpdate(suppPackMapping.Pool_update);
                        if (action != null)
                            list.Add(action);
                    }                        
                    else
                        list.AddRange(GetRemoveVdiActions(suppPackMapping.SuppPackVdis.Values.ToList()));
                }
            }

            RunMultipleActions(Messages.PATCHINGWIZARD_REMOVE_UPDATES, Messages.PATCHINGWIZARD_REMOVING_UPDATES, Messages.PATCHINGWIZARD_REMOVED_UPDATES, list);
        }

        private AsyncAction GetCleanActionForPoolPatch(Pool_patch patch)
        {
            if (patch == null || patch.Connection == null || !patch.Connection.IsConnected)
                return null;

            if (patch.HostsAppliedTo().Count == 0)
                return new RemovePatchAction(patch);

            return new DelegatedAsyncAction(patch.Connection, Messages.REMOVE_PATCH, "", "", session => Pool_patch.async_pool_clean(session, patch.opaque_ref));
        }

        private AsyncAction GetCleanActionForPoolUpdate(Pool_update update)
        {
            if (update == null || update.Connection == null || !update.Connection.IsConnected)
                return null;

            return new DelegatedAsyncAction(update.Connection, Messages.REMOVE_PATCH, "", "", session =>
            {
                try
                {
                    Pool_update.pool_clean(session, update.opaque_ref);
                    if (!update.AppliedOnHosts().Any())
                        Pool_update.destroy(session, update.opaque_ref);
                }
                catch (Failure f)
                {
                    log.Error("Clean up failed", f);
                }
            });
        }

        private List<AsyncAction> GetRemoveVdiActions(List<VDI> vdisToRemove)
        {
            var list = new List<AsyncAction>();

            if (vdisToRemove != null)
                foreach (var vdi in vdisToRemove)
                {
                    if (vdi.Connection != null && vdi.Connection.IsConnected)
                        list.Add(new DestroyDiskAction(vdi));
                }

            return list;
        }

        private void RemoveDownloadedPatches()
        {
            List<string> listOfDownloadedFiles = new List<string>();

            listOfDownloadedFiles.AddRange(PatchingWizard_AutomatedUpdatesPage.AllDownloadedPatches.Values); // AutomatedUpdates or NewVersion
            listOfDownloadedFiles.AddRange(PatchingWizard_UploadPage.AllDownloadedPatches.Values); //SingleUpdate
            listOfDownloadedFiles.AddRange(PatchingWizard_SelectPatchPage.UnzippedUpdateFiles);

            foreach (string downloadedPatch in listOfDownloadedFiles)
            {
                try
                {
                    if (File.Exists(downloadedPatch))
                    {
                        File.Delete(downloadedPatch);
                    }
                }
                catch
                {  
                    log.DebugFormat("Could not remove downloaded patch {0} ", downloadedPatch);
                }
            }           
        }

        private void RunMultipleActions(string title, string startDescription, string endDescription,
            List<AsyncAction> subActions)
        {
            if (subActions != null && subActions.Count > 0)
            {
                using (MultipleAction multipleAction = new MultipleAction(xenConnection, title, startDescription,
                    endDescription, subActions, false, true))
                {
                    using (var dialog = new ActionProgressDialog(multipleAction, ProgressBarStyle.Blocks))
                        dialog.ShowDialog(Program.MainWindow);
                }
            }
        }
    }
}
