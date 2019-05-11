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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Commands;

namespace XenAdmin.Dialogs
{
    /// <summary>
    /// A confirmation dialog for Commands. Primarily used for displaying the subset of items from the multiple-selection
    /// which are not going to be actioned by the Command.
    /// </summary>
    internal partial class CommandErrorDialog : XenDialogBase
    {
        public enum DialogMode { Close, OKCancel };

        public DialogMode Mode { get; private set; }

        private ListSortDirection m_direction;
        private DataGridViewColumn m_oldSortedColumn;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandErrorDialog"/> class.
        /// </summary>
        /// <param name="title">The title for the confirmation dialog.</param>
        /// <param name="text">The text for the confirmation dialog.</param>
        /// <param name="cantExecuteReasons">A dictionary of names of the objects which will be ignored with associated reasons.</param>
        /// <param name="mode">Whether the dialog should show a Close button, or OK and Cancel buttons.</param>
        public CommandErrorDialog(string title, string text,
            IDictionary<IXenObject, string> cantExecuteReasons, DialogMode mode = DialogMode.Close)
        {
            Util.ThrowIfParameterNull(cantExecuteReasons, "cantExecuteReasons");
            Util.ThrowIfParameterNull(title, "title");
            Util.ThrowIfParameterNull(text, "text");

            InitializeComponent();

            Text = title;
            lblText.Text = text;
            Mode = mode;

            btnCancel.Visible = mode == DialogMode.OKCancel;
            btnOK.Visible = mode == DialogMode.OKCancel;
            btnClose.Visible = mode == DialogMode.Close;

            foreach (var xenObject in cantExecuteReasons.Keys)
            {
                DataGridViewRow row = new DataGridViewRow {Tag = xenObject };
                row.Cells.AddRange(
                    new DataGridViewImageCell {Value = Images.GetImage16For(xenObject) },
                    new DataGridViewTextBoxCell {Value = xenObject.ToString()},
                    new DataGridViewTextBoxCell {Value = cantExecuteReasons[xenObject] });
                m_dataGridView.Rows.Add(row);
            }

            m_direction = ListSortDirection.Ascending;
            m_oldSortedColumn = colName;
            m_dataGridView.Sort(new RowComparer(colName.Index, m_direction));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void m_dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < colName.Index || e.ColumnIndex > colReason.Index)
                return;

            var newColumn = m_dataGridView.Columns[e.ColumnIndex];

            if (m_oldSortedColumn == null)
            {
                // the DataGridView is not currently sorted
                m_direction = ListSortDirection.Ascending;
                m_oldSortedColumn = newColumn;
            }
            else
            {
                if (m_oldSortedColumn == newColumn)
                {
                    // Sort the same column again, reversing the SortOrder.
                    m_direction = (m_direction == ListSortDirection.Ascending)
                                    ? ListSortDirection.Descending
                                    : ListSortDirection.Ascending;
                }
                else
                {
                    // Sort a new column and remove the SortGlyph from the old column
                    m_direction = ListSortDirection.Ascending;
                    m_oldSortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                    m_oldSortedColumn = newColumn;
                }
            }

            m_dataGridView.Sort(new RowComparer(newColumn.Index, m_direction));
            newColumn.HeaderCell.SortGlyphDirection = m_direction == ListSortDirection.Ascending
                                                          ? SortOrder.Ascending
                                                          : SortOrder.Descending;
        }

        private class RowComparer : IComparer
        {
            private readonly int m_columnIndex;
            private readonly ListSortDirection m_direction;
            private const int COL_NAME = 1;
            private const int COL_REASON = 2;

            public RowComparer(int columnIndex, ListSortDirection direction)
            {
                m_columnIndex = columnIndex;
                m_direction = direction;
            }

            public int Compare(object x, object y)
            {
                var row1 = (DataGridViewRow)x;
                var row2 = (DataGridViewRow)y;

                int result = 0;

                if (m_columnIndex == COL_NAME)
                    result = ((IXenObject)row1.Tag).CompareTo(row2.Tag);
                else if (m_columnIndex == COL_REASON)
                    result = string.Compare(row1.Cells[COL_REASON].Value.ToString(), row2.Cells[COL_REASON].Value.ToString());

                if (m_direction == ListSortDirection.Descending)
                    return -1 * result;

                return result;
            }
        }
    }
}