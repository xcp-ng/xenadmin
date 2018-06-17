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
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Controls.CheckableDataGridView
{
    /// <summary>
    /// NOTE: Ensure your first column is a checkbox column
    /// </summary>
    public class CheckableDataGridView : DataGridViewEx.DataGridViewEx, ICheckableDataGridViewView
    {
        public class CheckableDataGridViewRowEventArgs : EventArgs
        {
            public int RowIndex { get; set; }
            public bool RefreshGrid { get; set; }
        }

        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string RowUpdatedEventKey = "CheckableDataGridViewRowUpdatedEventKey";
        private const string RowCheckedEventKey = "CheckableDataGridViewRowCheckedEventKey";

        public delegate void RowUpdatedEvent(object sender, CheckableDataGridViewRowEventArgs e);
        public event RowUpdatedEvent RowUpdated
        {
            add{ Events.AddHandler(RowUpdatedEventKey, value); }
            remove{ Events.RemoveHandler(RowUpdatedEventKey, value); }
        }

        public delegate void RowCheckedEvent(object sender, CheckableDataGridViewRowEventArgs e);
        public event RowCheckedEvent RowChecked
        {
            add { Events.AddHandler(RowCheckedEventKey, value); }
            remove { Events.RemoveHandler(RowCheckedEventKey, value); }
        }
                
        public virtual void LoadView()
        {
            CellClick += CheckableDataGridView_CellContentClick;
            DataError += CheckableDataGridView_DataError;
        }

        private void CheckableDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            log.Error(e.Exception.Message,e.Exception);
        }

        public void ClearAllGridRows()
        {
            Controller.ClearAllRows();
        }

        private void CheckableDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex < 0)
                return;

            CheckableDataGridView senderGrid = sender as CheckableDataGridView;
            if (senderGrid == null)
                return;
            CheckableDataGridViewRow senderRow = senderGrid.Rows[e.RowIndex] as CheckableDataGridViewRow;
            if (senderRow == null)
                return;

            Controller.CellContentClicked(e.ColumnIndex, e.RowIndex, senderRow.Disabled);
        }

        public void SetRowInformation(int rowIndex, string information, bool disabled)
        {
            Controller.SetRowDisabledWithReason(rowIndex, information, disabled);
        }

        public void AddRows(List<CheckableDataGridViewRow> objects)
        {
            Controller.AddRows(objects);
        }

        public void CheckRows(List<CheckableDataGridViewRow> objects)
        {
           Controller.ToggleRowsChecked(objects);
        }

        public CheckableDataGridViewRow GetCheckableRow(int rowIndex)
        {
            return Controller.GetRow(rowIndex);
        }

        public List<CheckableDataGridViewRow> CheckedRows
        {
            get { return Controller.CheckedRows; }
        }

        public void HighlightRow(CheckableDataGridViewRow row)
        {
            Controller.HighlightOnlyRow(row);
        }

        public List<IXenObject> StoredXenObjects
        {
            get { return Controller.StoredXenObjects; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawAllRowsAsCleared()
        {
            SuspendLayout();
            try
            {
                Rows.Clear();
            }
            finally
            {
                ResumeLayout();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawAllRowsAsClearedMW()
        {
            Program.Invoke(Program.MainWindow, DrawAllRowsAsCleared);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawColumnGlyph(CheckableDataGridViewController.SortDirection sortDirection, int columnNumber)
        {
            if (columnNumber < 0 || columnNumber >= Columns.Count)
                return;

            if(Columns[columnNumber].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;

            if (sortDirection == CheckableDataGridViewController.SortDirection.Ascending)
                Columns[columnNumber].HeaderCell.SortGlyphDirection = SortOrder.Ascending;

            if (sortDirection == CheckableDataGridViewController.SortDirection.Descending)
                Columns[columnNumber].HeaderCell.SortGlyphDirection = SortOrder.Descending;

            if (sortDirection == CheckableDataGridViewController.SortDirection.None) 
                Columns[columnNumber].HeaderCell.SortGlyphDirection = SortOrder.None;
        }

        #region IDisposible
        private bool disposed;
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!disposed)
                {
                    foreach (CheckableDataGridViewRow row in Rows)
                    {
                        if(row != null)
                            row.Dispose();
                    }
                    Controller.Dispose();
                }
                Rows.Clear();
                disposed = true;
            }
            base.Dispose(disposing);
        } 
        #endregion

        #region ICheckableDataGridViewView Members
        [EditorBrowsable(EditorBrowsableState.Never)]
        public CheckableDataGridViewController Controller { set; protected get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawRow(CheckableDataGridViewRow gvRow)
        {
            SuspendLayout();
            try
            {
                CheckableDataGridViewRow row = gvRow;
                if(Rows.Contains(row))
                    Rows.Remove(row);
                row.Cells.Clear();
                DataGridViewCheckBoxCell cbCell = new DataGridViewCheckBoxCell { Value = gvRow.Checked };
                row.Cells.Add(cbCell);
                foreach (object cellValue in gvRow.CellText)
                {
                    DataGridViewCellStyle style = row.Disabled ? RowStyle(true) : UpdatingRowStyle(row.CellDataLoaded);
                    row.Cells.Add(GetCell(style, cellValue));
                }

                Rows.Add(row);
            }
            finally
            {
                ResumeLayout();
            }
        }

        private DataGridViewCell GetCell(DataGridViewCellStyle style, object cellValue)
        {

            if (cellValue is Bitmap)
                return new DataGridViewImageCell { Value = cellValue, ValueType = cellValue.GetType(), Style = style };

            return new DataGridViewTextBoxCell { Value = cellValue, ValueType = cellValue.GetType(), Style = style };
        }

        private DataGridViewCellStyle UpdatingRowStyle(bool cellDataLoaded)
        {
            return RowStyle(!cellDataLoaded);
        }

        private DataGridViewCellStyle RowStyle(bool greyStyle)
        {
            if (greyStyle)
                return new DataGridViewCellStyle
                {
                    ForeColor = Color.Gray,
                    Font = Program.DefaultFontItalic
                };
            return new DataGridViewCellStyle
            {
                ForeColor = Color.Black,
                Font = Program.DefaultFont
            };
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawRowAsDisabled(bool disabledStatus, int rowIndex)
        {
            SuspendLayout();
            try
            {
                if (rowIndex < 0 || rowIndex >= Rows.Count)
                    return;

                DataGridViewRow row = Rows[rowIndex];
                DataGridViewCheckBoxCell checkBoxCell = row.Cells[Controller.CheckboxIndex] as DataGridViewCheckBoxCell;

                if (checkBoxCell == null)
                    return;

                checkBoxCell.ReadOnly = disabledStatus;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style = RowStyle(disabledStatus);
                } 
            }
            finally
            {
                ResumeLayout();    
            }
            

        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawRowAsHighlighted(bool highlightStatus, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= Rows.Count)
                return;

            CurrentCell = null;
            // Set CurrentCell to first visible cell
            for (int i = 0; i < Rows[rowIndex].Cells.Count; i++)
            {
                if (Rows[rowIndex].Cells[i].Visible)
                {
                    CurrentCell = Rows[rowIndex].Cells[i];
                    break;
                }
            } 
            Rows[rowIndex].Selected = highlightStatus;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawRowAsChecked(bool checkStatus, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= Rows.Count)
                return;

            DataGridViewRow row = Rows[rowIndex];
            DataGridViewCheckBoxCell checkBoxCell = row.Cells[Controller.CheckboxIndex] as DataGridViewCheckBoxCell;

            if (checkBoxCell != null)
            {
                checkBoxCell.Value = checkStatus;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawRowAsLocked(bool lockStatus, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= Rows.Count)
                return;

            CheckableDataGridViewRow row = Rows[rowIndex] as CheckableDataGridViewRow;

            if (row != null)
            {
                row.LockDisabledState = lockStatus;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SuspendDrawing()
        {
            Program.Invoke(this, () => HelpersGUI.SuspendDrawing(this) );
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResumeDrawing()
        {
            Program.Invoke(this, () =>
                                     {
                                         HelpersGUI.ResumeDrawing(this);
                                         Refresh();
                                     });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void TriggerRowUpdatedEvent(int rowUpdated, bool refreshGrid)
        {
            RowUpdatedEvent handler = Events[RowUpdatedEventKey] as RowUpdatedEvent;
            if (handler != null)
                handler.Invoke(this, new CheckableDataGridViewRowEventArgs { RowIndex = rowUpdated, RefreshGrid = refreshGrid });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawUpdatedRow(Queue<object> textToUse, bool cellDataLoaded, bool rowDisabled, int rowIndex)
        {
            Program.Invoke(this, delegate
                                     {
                                         try{
                                            
                                             SuspendLayout();
                                             if(rowIndex < 0 || rowIndex >= Rows.Count)
                                             {
                                                 log.DebugFormat("Could not update row {0} of {1}, so skipping update", rowIndex, Rows.Count);
                                                 return;
                                             }
                                             DataGridViewRow rowToUpdate = Rows[rowIndex];
                                             for (int i = 0; i < rowToUpdate.Cells.Count; i++ )
                                             {
                                                 if (i == Controller.CheckboxIndex)
                                                     continue;
                                                 
                                                 rowToUpdate.Cells[i].Value = textToUse.Dequeue();
                                                 rowToUpdate.Cells[i].Style = rowDisabled ? RowStyle(true) : UpdatingRowStyle(cellDataLoaded);
                                             }
                                         }finally
                                         {
                                             ResumeLayout();
                                         }
                                     });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void TriggerRowCheckedEvent(int rowChecked)
        {
            RowCheckedEvent handler = Events[RowCheckedEventKey] as RowCheckedEvent;
            if (handler != null)
                handler.Invoke(this, new CheckableDataGridViewRowEventArgs { RowIndex = rowChecked });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawRowMW(CheckableDataGridViewRow row)
        {
            Program.Invoke(Program.MainWindow, ()=>DrawRow(row));
        }

        public void DrawRowAsHighlightedMW(bool highlightStatus, int rowIndex)
        {
            Program.Invoke(Program.MainWindow, () => DrawRowAsHighlighted(highlightStatus, rowIndex));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawSetRowInformation(int rowIndex, string information)
        {
            if (rowIndex < 0 || rowIndex >= Rows.Count)
                return;

            CheckableDataGridViewRow row = Rows[rowIndex] as CheckableDataGridViewRow;
            if (row != null)
                row.DisabledReason = information;
        }


        #endregion
    }
}
