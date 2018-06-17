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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.CheckableDataGridView;
using XenAdmin.Controls.SummaryPanel;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public partial class LicenseManager : XenDialogBase, ILicenseManagerView
    {
        public LicenseManager(LicenseManagerController lmcontroller)
        {
            InitializeComponent();
            checkableDataGridView.StatusImageColumn = statusImageColumn;
            SetupControllers(lmcontroller);
        }

        private void SetupControllers(LicenseManagerController lmcontroller)
        {
            checkableDataGridView.Controller = new LicenseCheckableDataGridViewController(checkableDataGridView);
            summaryPanel.Controller = new SummaryPanelController(summaryPanel);
            checkableDataGridView.LoadView();
            Controller = lmcontroller;
            Controller.View = this;
            downloadLicenseServerLink.Visible = checkBoxColumn.Visible = !Controller.ReadOnlyView;
        }

        private void LoadView(List<IXenObject> itemsToShow, List<IXenObject> selectedItems)
        {
            //Grid
            checkableDataGridView.SelectionChanged += checkableDataGridView_SelectionChanged;
            checkableDataGridView.RowUpdated += checkableDataGridView_RowUpdated;
            checkableDataGridView.RowChecked += checkableDataGridView_RowChecked;
            checkableDataGridView.RefreshAll += checkableDataGridView_RefreshAll;
            
            //Buttons
            cancelButton.Click += closeButton_Click;
            releaseLicenseButton.Click += releaseLicenseButton_Click;
            assignLicenceButton.Click += assignLicenceButton_Click;
            downloadLicenseServerLink.LinkClicked += downloadLicenseServerLink_LinkClicked;
            
            //Controllers
            Controller.PopulateGrid(itemsToShow, selectedItems);

        }

        void checkableDataGridView_RowChecked(object sender, CheckableDataGridView.CheckableDataGridViewRowEventArgs e)
        {
            Controller.UpdateButtonEnablement();
        }

        private void assignLicenceButton_Click(object sender, EventArgs e)
        {
            Controller.AssignLicense(checkableDataGridView.CheckedRows);
        }

        private void downloadLicenseServerLink_LinkClicked(object sender, EventArgs e)
        {
            Controller.DownloadLicenseManager();
        }

        private void releaseLicenseButton_Click(object sender, EventArgs e)
        {
            Controller.ReleaseLicenses(checkableDataGridView.CheckedRows);
        }

        private void checkableDataGridView_RowUpdated(object sender, CheckableDataGridView.CheckableDataGridViewRowEventArgs e)
        {
            LicenseCheckableDataGridView senderGrid = sender as LicenseCheckableDataGridView;
            if (senderGrid == null || e.RowIndex >= senderGrid.Rows.Count || e.RowIndex < 0)
                return;

            LicenseDataGridViewRow lRow = senderGrid.Rows[e.RowIndex] as LicenseDataGridViewRow;
            if (lRow == null)
                return;

            Controller.SetStatusIcon(e.RowIndex, lRow.RowStatus);

            if (!e.RefreshGrid && senderGrid.SelectedRows.Count > 0 && senderGrid.SelectedRows[0].Index == e.RowIndex)
            {
                Controller.SummariseSelectedRow(checkableDataGridView.GetCheckableRow(e.RowIndex));
            }

            if (e.RefreshGrid)
                senderGrid.SortAndRefresh();
        }

        private void checkableDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            CheckableDataGridView senderGrid = sender as CheckableDataGridView;
            if(senderGrid == null || senderGrid.SelectedRows.Count<1)
                return;

            Controller.SummariseSelectedRow(checkableDataGridView.GetCheckableRow(senderGrid.SelectedRows[0].Index));
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        public DialogResult ShowDialog(IWin32Window parent, List<IXenObject> itemsToShow, List<IXenObject> selectedItems)
        {
            LoadView(itemsToShow, selectedItems);
            return ShowDialog(parent);
        }

        public void RefreshView(List<IXenObject> itemsToShow, List<IXenObject> selectedItems)
        {
            Controller.Repopulate(itemsToShow, selectedItems);
        }

        void checkableDataGridView_RefreshAll(object sender, EventArgs eventArgs)
        {
            Program.Invoke(this, Controller.Repopulate);
        }

        #region ILicenseManagerView Members
        [EditorBrowsable(EditorBrowsableState.Never)]
        public LicenseManagerController Controller { set; private get; } 

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawRowStatusIcon(int rowIndex, LicenseDataGridViewRow.Status rowStatus)
        {
            checkableDataGridView.SetStatusIcon(rowIndex, rowStatus);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<CheckableDataGridViewRow> GetCheckedRows
        {
            get { return checkableDataGridView.CheckedRows; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawRowsInGrid(List<CheckableDataGridViewRow> itemsToShow)
        {
            checkableDataGridView.AddRows(itemsToShow);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawSelectedRowsAsChecked(List<CheckableDataGridViewRow> rows)
        {
            checkableDataGridView.CheckRows(rows);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawSummaryForHighlightedRow(CheckableDataGridViewRow row, SummaryTextComponent summaryComponent, Action runOnUrlClick)
        {
            Program.Invoke(this, delegate
                                     {
                                         LicenseDataGridViewRow lRow = row as LicenseDataGridViewRow;
                                         if(lRow == null || lRow.XenObject == null)
                                             return;

                                         summaryPanel.Title = lRow.XenObject.Name();
                                         summaryPanel.HelperUrl = Messages.LICENSE_MANAGER_BUY_LICENSE_LINK_TEXT;
                                         summaryPanel.HelperUrlVisible = lRow.HelperUrlRequired && !Controller.ReadOnlyView;
                                         summaryPanel.WarningVisible = lRow.WarningRequired;
                                         summaryPanel.WarningText = lRow.WarningText;
                                         summaryPanel.SummaryText = summaryComponent;
                                         switch (lRow.RowStatus)
                                         {
                                             case LicenseDataGridViewRow.Status.Information:
                                                 summaryPanel.WarningIcon = Resources._000_Alert2_h32bit_16;
                                                 break;
                                             case LicenseDataGridViewRow.Status.Warning:
                                                 summaryPanel.WarningIcon = Resources._000_error_h32bit_16;
                                                 break;
                                             default:
                                                 summaryPanel.WarningIcon = Resources._000_Tick_h32bit_16;
                                                 break;
                                         }
                                         summaryPanel.InformationVisible = false;
                                         summaryPanel.RunOnUrlClick = runOnUrlClick;
                                     });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawHighlightedRow(CheckableDataGridViewRow row)
        {
            checkableDataGridView.HighlightRow(row);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawAssignButtonAsDisabled(bool isDisabled)
        {
            assignLicenceButton.Enabled = !isDisabled;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawReleaseButtonAsDisabled(bool isDisabled)
        {
            releaseLicenseButton.Enabled = !isDisabled;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ClearAllGridRows()
        {
            checkableDataGridView.ClearAllGridRows();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawSummaryInformation(string info, bool show)
        {
            Program.Invoke(Program.MainWindow, delegate
                                                   {
                                                       summaryPanel.InformationText = info;
                                                       summaryPanel.InformationVisible = show;
                                                   });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetRowDisabledRowInfo(int rowIndex, string info, bool disabled)
        {
            checkableDataGridView.SetRowInformation(rowIndex, info, disabled);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawViewAsReadOnly(bool isReadOnly)
        {
            if (isReadOnly)
            {
                assignLicenceButton.Hide();
                releaseLicenseButton.Hide();
            }
            else
            {
                assignLicenceButton.Show();
                releaseLicenseButton.Show();
            }
        }

        #endregion
    }
}
