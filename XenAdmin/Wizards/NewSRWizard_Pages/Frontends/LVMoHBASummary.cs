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
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using XenAdmin.Properties;
using System.Linq;

namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class LVMoHBASummary : XenTabPage
    {
        public LVMoHBASummary()
        {
            InitializeComponent();
            SuccessfullyCreatedSRs = new List<SrDescriptor>();
            FailedToCreateSRs = new List<SrDescriptor>();
        }

        #region XenTabPage overrides

        public override string PageTitle
        {
            get
            {
                return SuccessfullyCreatedSRs.Count + FailedToCreateSRs.Count == 1
                           ? Messages.NEWSR_LVMOHBA_SUMMARY_SINGLE_TITLE
                           : Messages.NEWSR_LVMOHBA_SUMMARY_MULTIPLE_TITLE;
            }
        }

        public override string Text { get { return Messages.NEWSR_LVMOHBA_SUMMARY_TEXT; } }

        public override string HelpID { get { return "Summary_HBA"; } }

        public override bool EnableCancel()
        {
            return false;
        }

        public override bool EnablePrevious()
        {
            return false;
        }

        public override void  PopulatePage()
        {
            PopulateGrid(false);
            PopulateGrid(true);

            foreach (var row in dataGridViewSummary.Rows.OfType<LVMoHBASummaryHeaderGridViewRow>())
            {
                ExpandCollapseRow(row.Index);
            }
        }

        #endregion

        #region Accessors

        public List<SrDescriptor> SuccessfullyCreatedSRs { get; set; }
        public List<SrDescriptor> FailedToCreateSRs { get; set; }

        #endregion

        # region Private methods

        private void PopulateGrid(bool success)
        {
            List<SrDescriptor> srDescriptors = success ? SuccessfullyCreatedSRs : FailedToCreateSRs;
            if (srDescriptors.Count == 0)
                return;

            dataGridViewSummary.Rows.Add(new LVMoHBASummaryHeaderGridViewRow(success, srDescriptors.Count));
            foreach (var descriptor in srDescriptors)
            {
                DataGridViewRow row = new LVMoHBASrDescriptorGridViewRow(descriptor) { Visible = false };
                dataGridViewSummary.Rows.Add(row);
            }
        }

        private void ExpandCollapseRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dataGridViewSummary.Rows.Count)
                return;

            var row = dataGridViewSummary.Rows[rowIndex] as LVMoHBASummaryHeaderGridViewRow;
            if (row != null)
            {
                row.ToggleExpanded();
                for (int i = row.Index + 1; i < dataGridViewSummary.Rows.Count; i++)
                {
                    if (dataGridViewSummary.Rows[i] is LVMoHBASummaryHeaderGridViewRow)
                        break;

                    var detailsRow = dataGridViewSummary.Rows[i] as LVMoHBASrDescriptorGridViewRow;
                    if (detailsRow != null)
                        detailsRow.Visible = !detailsRow.Visible;
                }
            }
        }

        #endregion

        #region Event handlers

        private const int expansionColumnIndex = 0;

        private void dataGridViewSummary_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == expansionColumnIndex)
                ExpandCollapseRow(e.RowIndex);
        }

        #endregion

        #region Nested classes

        private abstract class LVMoHBASummaryGridViewRow : DataGridViewExRow
        {
            protected DataGridViewImageCell imageCell;
            protected DataGridViewTextBoxCell detailsCell;

            protected void SetupCells()
            {
                imageCell = GetImageCell();
                detailsCell = GetDetailsCell();

                Cells.Add(imageCell);
                Cells.Add(detailsCell);
            }

            protected abstract DataGridViewImageCell GetImageCell();
            protected abstract DataGridViewTextBoxCell GetDetailsCell();
        }

        private class LVMoHBASummaryHeaderGridViewRow : LVMoHBASummaryGridViewRow
        {
            private readonly bool success;
            private readonly int count;
            private bool expanded;
            private readonly Image icon;

            public LVMoHBASummaryHeaderGridViewRow(bool success, int count)
            {
                this.success = success;
                this.count = count;
                icon = success ? Resources._000_Tick_h32bit_16 : Resources._000_Abort_h32bit_16;
                SetupCells();
            }

            private string GetFailureText()
            {
                return count == 1
                           ? Messages.NEWSR_LVMOHBA_SUMMARY_FAILURE_SINGLE
                           : string.Format(Messages.NEWSR_LVMOHBA_SUMMARY_FAILURE_MULTIPLE, count);
            }

            private string GetSuccessText()
            {
                return count == 1
                           ? Messages.NEWSR_LVMOHBA_SUMMARY_SUCCESS_SINGLE
                           : string.Format(Messages.NEWSR_LVMOHBA_SUMMARY_SUCCESS_MULTIPLE, count);
            }

            protected override DataGridViewImageCell GetImageCell()
            {
                return new DataGridViewImageCell {Value = Resources.contracted_triangle};
            }

            protected override DataGridViewTextBoxCell GetDetailsCell()
            {
                return new DataGridViewTextBoxCell {Value = success ? GetSuccessText() : GetFailureText()};
            }

            public void ToggleExpanded()
            {
                expanded = !expanded;
                imageCell.Value = expanded ? Resources.expanded_triangle : Resources.contracted_triangle;
            }

            protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow)
            {
                base.Paint(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow);

                int iconOffset = imageCell.OwningColumn.Width;
                string text = detailsCell.Value as string;
                if (!string.IsNullOrEmpty(text))
                    iconOffset += Drawing.MeasureText(graphics, text, DataGridView.Font).Width + 4;

                graphics.DrawImage(icon, rowBounds.Left + iconOffset, rowBounds.Top + 1, icon.Width, icon.Height);
            }
        }

        private class LVMoHBASrDescriptorGridViewRow : LVMoHBASummaryGridViewRow
        {
            private SrDescriptor srDescriptor;

            public LVMoHBASrDescriptorGridViewRow(SrDescriptor srDescriptor)
            {
                this.srDescriptor = srDescriptor;
                SetupCells();
            }

            protected override DataGridViewImageCell GetImageCell()
            {
                return new DataGridViewImageCell { Value = new Bitmap(1, 1) };
            }

            protected override DataGridViewTextBoxCell GetDetailsCell()
            {
                var descriptor = srDescriptor as FibreChannelDescriptor;
                string text = descriptor != null
                    ? string.Format(Messages.NEWSR_LVMOHBA_SUMMARY_DETAILS_CELL_TEXT,
                        descriptor.Name,
                        Util.DiskSizeString(descriptor.Device.Size),
                        descriptor.Device.Serial)
                    : srDescriptor.Name;

                return new DataGridViewTextBoxCell
                {
                    Value = text,
                    Style = {Padding = new Padding(10, 0, 0, 0)}
                };
            }
        }

        #endregion
    }
}
