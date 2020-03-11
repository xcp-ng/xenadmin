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
using XenAdmin.Controls;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using XenAPI;
using System.IO;
using XenAdmin.Dialogs;
using System.Drawing;
using XenAdmin.Alerts;
using System.Linq;
using System.Xml;
using DiscUtils.Iso9660;


namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_SelectPatchPage : XenTabPage
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool CheckForUpdatesInProgress;
        public XenServerPatchAlert UpdateAlertFromWeb;
        public XenServerPatchAlert AlertFromFileOnDisk;
        public bool FileFromDiskHasUpdateXml { get; private set; }
        private bool firstLoad = true;
        private string unzippedUpdateFilePath;

        public PatchingWizard_SelectPatchPage()
        {
            InitializeComponent();
            tableLayoutPanelSpinner.Visible = false;

            labelWithAutomatedUpdates.Visible =
                automatedUpdatesOptionLabel.Visible = AutomatedUpdatesRadioButton.Visible = false;
            downloadUpdateRadioButton.Checked = true;
        }

        private void CheckForUpdates_CheckForUpdatesStarted()
        {
            Program.Invoke(Program.MainWindow, StartCheckForUpdates);
        }

        private void Updates_RestoreDismissedUpdatesStarted()
        {
            Program.Invoke(Program.MainWindow, StartCheckForUpdates);
        }

        private void StartCheckForUpdates()
        {
            if (CheckForUpdatesInProgress)
                return;

            CheckForUpdatesInProgress = true;
            dataGridViewPatches.Rows.Clear();
            tableLayoutPanelSpinner.Visible = true;
            RestoreDismUpdatesButton.Enabled = false;
            RefreshListButton.Enabled = false;
            OnPageUpdated();
        }

        private void CheckForUpdates_CheckForUpdatesCompleted(bool succeeded, string errorMessage)
        {
            Program.Invoke(Program.MainWindow, delegate
            {
                tableLayoutPanelSpinner.Visible = false;
                PopulatePatchesBox();
                RefreshListButton.Enabled = true;
                RestoreDismUpdatesButton.Enabled = true;
                CheckForUpdatesInProgress = false;
                OnPageUpdated();
            });
        }

        public void SelectDownloadAlert(XenServerPatchAlert alert)
        {
            downloadUpdateRadioButton.Checked = true;
            foreach (PatchGridViewRow row in dataGridViewPatches.Rows)
            {
                if (row.UpdateAlert.Equals(alert))
                {
                    row.Selected = true;
                }
            }
        }

        public override string Text
        {
            get { return Messages.PATCHINGWIZARD_SELECTPATCHPAGE_TEXT; }
        }

        public override string PageTitle
        {
            get { return Messages.PATCHINGWIZARD_SELECTPATCHPAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return "SelectUpdate"; }
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            Updates.CheckForUpdatesStarted += CheckForUpdates_CheckForUpdatesStarted;
            Updates.CheckForUpdatesCompleted += CheckForUpdates_CheckForUpdatesCompleted;
            Updates.RestoreDismissedUpdatesStarted += Updates_RestoreDismissedUpdatesStarted;

            if (direction == PageLoadedDirection.Forward)
            {
                //if any connected host is licensed for automated updates
                bool automatedUpdatesPossible =
                    ConnectionsManager.XenConnectionsCopy.Any(
                        c => c != null && c.Cache.Hosts.Any(h => !Host.RestrictBatchHotfixApply(h)));

                labelWithAutomatedUpdates.Visible =
                    automatedUpdatesOptionLabel.Visible = AutomatedUpdatesRadioButton.Visible = automatedUpdatesPossible;
                labelWithoutAutomatedUpdates.Visible = !automatedUpdatesPossible;

                if (firstLoad)
                {
                    AutomatedUpdatesRadioButton.Checked = automatedUpdatesPossible;
                    downloadUpdateRadioButton.Checked = !automatedUpdatesPossible;
                }
                else if (!automatedUpdatesPossible && AutomatedUpdatesRadioButton.Checked)
                {
                    downloadUpdateRadioButton.Checked = true;
                }

                Updates.CheckServerPatches();
                PopulatePatchesBox();
                OnPageUpdated();
            }

            firstLoad = false;
        }

        private bool IsInAutomatedUpdatesMode
        {
            get { return AutomatedUpdatesRadioButton.Visible && AutomatedUpdatesRadioButton.Checked; }
        }

        public WizardMode WizardMode
        {
            get
            {
                if (AutomatedUpdatesRadioButton.Visible && AutomatedUpdatesRadioButton.Checked)
                    return WizardMode.AutomatedUpdates;
                var updateAlert = downloadUpdateRadioButton.Checked
                    ? UpdateAlertFromWeb
                    : selectFromDiskRadioButton.Checked
                        ? AlertFromFileOnDisk
                        : null;
                if (updateAlert != null && updateAlert.NewServerVersion != null)
                    return WizardMode.NewVersion;
                return WizardMode.SingleUpdate;
            }
        }
        public KeyValuePair<XenServerPatch, string> PatchFromDisk { get; private set; }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Forward)
            {
                if (IsInAutomatedUpdatesMode)
                {
                    var succeed = Updates.CheckForUpdatesSync(this.Parent);
                    cancel = !succeed;
                }
                else if (downloadUpdateRadioButton.Checked)
                {
                    UpdateAlertFromWeb = dataGridViewPatches.SelectedRows.Count > 0
                        ? ((PatchGridViewRow) dataGridViewPatches.SelectedRows[0]).UpdateAlert
                        : null;

                    var distinctHosts = UpdateAlertFromWeb != null ? UpdateAlertFromWeb.DistinctHosts : null;
                    SelectedUpdateType = distinctHosts != null && distinctHosts.Any(Helpers.ElyOrGreater)
                        ? UpdateType.ISO
                        : UpdateType.Legacy;

                    AlertFromFileOnDisk = null;
                    FileFromDiskHasUpdateXml = false;
                    unzippedUpdateFilePath = null;
                    SelectedPatchFilePath = null;
                    PatchFromDisk = new KeyValuePair<XenServerPatch, string>(null, null);
                }
                else
                {
                    UpdateAlertFromWeb = null;
                    SelectedPatchFilePath = null;

                    if (!WizardHelpers.IsValidFile(FilePath, out var pathFailure))
                        using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(
                            SystemIcons.Error, pathFailure, Messages.UPDATES)))
                        {
                            cancel = true;
                            dlg.ShowDialog();
                            return;
                        }

                    SelectedPatchFilePath = FilePath;

                    if (Path.GetExtension(FilePath).ToLowerInvariant().Equals(".zip"))
                    {
                        //check if we are installing the update the user sees in the textbox
                        if (unzippedUpdateFilePath == null || !File.Exists(unzippedUpdateFilePath) ||
                            Path.GetFileNameWithoutExtension(unzippedUpdateFilePath) != Path.GetFileNameWithoutExtension(FilePath))
                        {
                            unzippedUpdateFilePath = WizardHelpers.ExtractUpdate(FilePath, this);
                        }

                        if (!WizardHelpers.IsValidFile(unzippedUpdateFilePath, out var zipFailure))
                        {
                            using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(
                                SystemIcons.Error, zipFailure, Messages.UPDATES)))
                            {
                                cancel = true;
                                dlg.ShowDialog();
                                return;
                            }
                        }
                        
                        if (!unzippedFiles.Contains(unzippedUpdateFilePath))
                            unzippedFiles.Add(unzippedUpdateFilePath);

                        SelectedPatchFilePath = unzippedUpdateFilePath;
                    }
                    else
                        unzippedUpdateFilePath = null;                  

                    if (SelectedPatchFilePath.EndsWith("." + BrandManager.ExtensionUpdate))
                        SelectedUpdateType = UpdateType.Legacy;
                    else if (SelectedPatchFilePath.EndsWith("." + InvisibleMessages.ISO_UPDATE))
                        SelectedUpdateType = UpdateType.ISO;

                    AlertFromFileOnDisk = GetAlertFromFile(SelectedPatchFilePath, out var hasUpdateXml);
                    FileFromDiskHasUpdateXml = hasUpdateXml;
                    PatchFromDisk = AlertFromFileOnDisk == null
                        ? new KeyValuePair<XenServerPatch, string>(null, null)
                        : new KeyValuePair<XenServerPatch, string>(AlertFromFileOnDisk.Patch, SelectedPatchFilePath);
                }
            }

            if (!cancel) //unsubscribe only if we are really leaving this page
            {
                Updates.RestoreDismissedUpdatesStarted -= Updates_RestoreDismissedUpdatesStarted;
                Updates.CheckForUpdatesStarted -= CheckForUpdates_CheckForUpdatesStarted;
                Updates.CheckForUpdatesCompleted -= CheckForUpdates_CheckForUpdatesCompleted;
            }
        }

        private XenServerPatchAlert GetAlertFromFile(string fileName, out bool hasUpdateXml)
        {
            var alertFromIso = GetAlertFromIsoFile(fileName, out hasUpdateXml);
            if (alertFromIso != null)
                return alertFromIso;

            // couldn't find an alert from the information in the iso file, try matching by name
            return Updates.FindPatchAlertByName(Path.GetFileNameWithoutExtension(fileName));
        }

        private XenServerPatchAlert GetAlertFromIsoFile(string fileName, out bool hasUpdateXml)
        {
            hasUpdateXml = false;

            if (!fileName.EndsWith(InvisibleMessages.ISO_UPDATE))
                return null;

            var xmlDoc = new XmlDocument();

            try
            {
                using (var isoStream = File.OpenRead(fileName))
                {
                    var cd = new CDReader(isoStream, true);
                    if (cd.Exists("Update.xml"))
                    {
                        using (var fileStream = cd.OpenFile("Update.xml", FileMode.Open))
                        {
                            xmlDoc.Load(fileStream);
                            hasUpdateXml = true;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error("Exception while reading the update data from the iso file:", exception);
            }

            var elements = xmlDoc.GetElementsByTagName("update");
            var update = elements.Count > 0 ? elements[0] : null;

            if (update == null || update.Attributes == null)
                return null;

            var uuid = update.Attributes["uuid"];
            return uuid != null ? Updates.FindPatchAlertByUuid(uuid.Value) : null;
        }

        private void PopulatePatchesBox()
        {
            try
            {
                var updates = Updates.UpdateAlerts.ToList();

                if (dataGridViewPatches.SortedColumn != null)
                {
                    if (dataGridViewPatches.SortedColumn.Index == ColumnUpdate.Index)
                        updates.Sort(Alert.CompareOnTitle);
                    else if (dataGridViewPatches.SortedColumn.Index == ColumnDate.Index)
                        updates.Sort(Alert.CompareOnDate);
                    else if (dataGridViewPatches.SortedColumn.Index == ColumnDescription.Index)
                        updates.Sort(Alert.CompareOnDescription);

                    if (dataGridViewPatches.SortOrder == SortOrder.Descending)
                        updates.Reverse();
                }
                else
                {
                    updates.Sort(new NewVersionPriorityAlertComparer());
                }

                dataGridViewPatches.SuspendLayout();
                dataGridViewPatches.Rows.Clear();

                foreach (Alert alert in updates)
                {
                    if (!(alert is XenServerPatchAlert patchAlert))
                        continue;

                    PatchGridViewRow row = new PatchGridViewRow(patchAlert);
                    if (!dataGridViewPatches.Rows.Contains(row))
                    {
                        dataGridViewPatches.Rows.Add(row);

                        if (patchAlert.RequiredXenCenterVersion != null)
                        {
                            row.Enabled = false;
                            row.SetToolTip(string.Format(Messages.UPDATES_WIZARD_NEWER_XENCENTER_REQUIRED,
                                patchAlert.RequiredXenCenterVersion.Version));
                        }
                    }
                }
            }
            finally
            {
                dataGridViewPatches.ResumeLayout();
            }
        }

        public override void PageCancelled(ref bool cancel)
        {
            Updates.RestoreDismissedUpdatesStarted -= Updates_RestoreDismissedUpdatesStarted;
            Updates.CheckForUpdatesStarted -= CheckForUpdates_CheckForUpdatesStarted;
            Updates.CheckForUpdatesCompleted -= CheckForUpdates_CheckForUpdatesCompleted;
        }

        public override bool EnableNext()
        {
            if (CheckForUpdatesInProgress)
            {
                return false;
            }

            if (IsInAutomatedUpdatesMode)
            {
                return true;
            }

            if (downloadUpdateRadioButton.Checked)
            {
                if (dataGridViewPatches.SelectedRows.Count == 1)
                {
                    DataGridViewExRow row = (DataGridViewExRow) dataGridViewPatches.SelectedRows[0];
                    if (row.Enabled)
                    {
                        return true;
                    }
                }
            }
            else if (selectFromDiskRadioButton.Checked)
            {
                if (WizardHelpers.IsValidFile(FilePath, out _))
                    return true;
            }

            return false;
        }

        public override bool EnablePrevious()
        {
            return !CheckForUpdatesInProgress;
        }

        /// <summary>
        /// List to store unzipped files to be removed later by PatchingWizard
        /// </summary>
        private readonly List<string> unzippedFiles = new List<string>();

        public List<string> UnzippedUpdateFiles
        {
            get { return unzippedFiles; }
        }

        public string FilePath
        {
            get { return fileNameTextBox.Text; }
            set { fileNameTextBox.Text = value; }
        }

        public UpdateType SelectedUpdateType { get; set; }

        public string SelectedPatchFilePath { get; set; }

        #region DataGridView

        private void dataGridViewPatches_SelectionChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        private void dataGridViewPatches_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            Alert alert1 = ((PatchGridViewRow) dataGridViewPatches.Rows[e.RowIndex1]).UpdateAlert;
            Alert alert2 = ((PatchGridViewRow) dataGridViewPatches.Rows[e.RowIndex2]).UpdateAlert;

            if (e.Column.Index == ColumnDate.Index)
            {
                e.SortResult = DateTime.Compare(alert1.Timestamp, alert2.Timestamp);
                e.Handled = true;
            }
        }

        private void dataGridViewPatches_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // The click is on a column header
            if (e.RowIndex == -1)
            {
                return;
            }
            PatchGridViewRow row = dataGridViewPatches.Rows[e.RowIndex] as PatchGridViewRow;
            if (row != null && e.ColumnIndex == 3)
            {
                row.UpdateAlert.FixLinkAction();
            }
        }

        private void dataGridViewPatches_Enter(object sender, EventArgs e)
        {
            downloadUpdateRadioButton.Checked = true;
            OnPageUpdated();
        }

        private class PatchGridViewRow : DataGridViewExRow, IEquatable<PatchGridViewRow>
        {
            private readonly XenServerPatchAlert _alert;

            private DataGridViewTextBoxCell _nameCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _descriptionCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _dateCell = new DataGridViewTextBoxCell();
            private DataGridViewLinkCell _webPageCell = new DataGridViewLinkCell();

            public PatchGridViewRow(XenServerPatchAlert alert)
            {
                _alert = alert;
                Cells.AddRange(_nameCell, _descriptionCell, _dateCell, _webPageCell);

                _nameCell.Value = String.Format(alert.Name);
                _descriptionCell.Value = String.Format(alert.Description);
                _dateCell.Value = HelpersGUI.DateTimeToString(alert.Timestamp.ToLocalTime(), Messages.DATEFORMAT_DMY,
                    true);
                _webPageCell.Value = Messages.PATCHING_WIZARD_WEBPAGE_CELL;
            }

            public XenServerPatchAlert UpdateAlert
            {
                get { return _alert; }
            }

            public bool Equals(PatchGridViewRow other)
            {
                if (other != null && other.UpdateAlert != null && UpdateAlert != null && UpdateAlert.uuid == other.UpdateAlert.uuid)
                    return true;
                return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is PatchGridViewRow)
                    return this.Equals((PatchGridViewRow) obj);
                return false;
            }

            public void SetToolTip(string toolTip)
            {
                foreach (var c in Cells)
                {
                    if (c is DataGridViewLinkCell)
                        continue;

                    var cell = c as DataGridViewCell;
                    if (c != null)
                        ((DataGridViewCell) c).ToolTipText = toolTip;
                }
            }
        }

        #endregion


        #region Buttons

        private void RestoreDismUpdatesButton_Click(object sender, EventArgs e)
        {
            dataGridViewPatches.Focus();
            Updates.RestoreDismissedUpdates();
        }

        private void RefreshListButton_Click(object sender, EventArgs e)
        {
            dataGridViewPatches.Focus();
            Updates.CheckForUpdates(true);
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            selectFromDiskRadioButton.Checked = true;
            var suppPack = WizardHelpers.GetSuppPackFromDisk(this);
            if (!string.IsNullOrEmpty(suppPack))
                FilePath = suppPack;
            OnPageUpdated();
        }

        #endregion


        #region TextBox

        private void fileNameTextBox_Enter(object sender, EventArgs e)
        {
            selectFromDiskRadioButton.Checked = true;
        }

        private void fileNameTextBox_TextChanged(object sender, EventArgs e)
        {
            selectFromDiskRadioButton.Checked = true;
            OnPageUpdated();
        }

        #endregion


        #region RadioButtons

        private void AutomaticRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        private void downloadUpdateRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            dataGridViewPatches.HideSelection = !downloadUpdateRadioButton.Checked;
            if (downloadUpdateRadioButton.Checked)
                dataGridViewPatches.Focus();
            OnPageUpdated();
        }

        private void selectFromDiskRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        private void AutomaticRadioButton_TabStopChanged(object sender, EventArgs e)
        {
            if (!AutomatedUpdatesRadioButton.TabStop)
                AutomatedUpdatesRadioButton.TabStop = true;
        }

        private void downloadUpdateRadioButton_TabStopChanged(object sender, EventArgs e)
        {
            if (!downloadUpdateRadioButton.TabStop)
                downloadUpdateRadioButton.TabStop = true;
        }

        private void selectFromDiskRadioButton_TabStopChanged(object sender, EventArgs e)
        {
            if (!selectFromDiskRadioButton.TabStop)
                selectFromDiskRadioButton.TabStop = true;
        }

        #endregion
    }        

    public enum UpdateType { Legacy, ISO }
}
