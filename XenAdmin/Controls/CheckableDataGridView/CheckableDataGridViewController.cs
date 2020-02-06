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
using XenCenterLib;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Controls.CheckableDataGridView
{
    public class CheckableDataGridViewController : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public enum SortDirection
        {
            None,
            Ascending,
            Descending
        }

        public ICheckableDataGridViewView View { protected get; set; }

        public int CheckboxIndex{get { return 0; } }

        public CheckableDataGridViewController(){}

        public CheckableDataGridViewController(ICheckableDataGridViewView view)
        {
            View = view;
        }

        protected readonly List<CheckableDataGridViewRow> storedRows = new List<CheckableDataGridViewRow>();

        public void ClearAllRows()
        {
            Program.AssertOnEventThread(); 
            storedRows.ForEach(r => r.Dispose());
            storedRows.Clear();

            View.DrawAllRowsAsCleared();
        }

        private IXenObject GetXenObject(CheckableDataGridViewRow row)
        {
            return row.XenObject is Pool ? Helpers.GetMaster(row.XenObject.Connection) : row.XenObject;
        }

        public void AddRows(List<CheckableDataGridViewRow> rows)
        {
            foreach (CheckableDataGridViewRow cRow in rows)
            {
                Program.AssertOnEventThread();
                storedRows.Add(cRow);
                cRow.CellDataUpdated += cRow_CellDataUpdated;
                IXenObject xenObject = GetXenObject(cRow);
                if (xenObject != null && xenObject.Connection != null)
                    xenObject.Connection.Cache.RegisterBatchCollectionChanged<Host>(HostBatchCollectionChanged);

                View.DrawRow(cRow);
            }

            rows.ForEach(r=>r.BeginCellUpdate());
        }

        void HostBatchCollectionChanged(object sender, EventArgs e)
        {
            ChangeableDictionary<XenRef<Host>, Host> d = sender as ChangeableDictionary<XenRef<Host>, Host>;
            if (d == null) return;

            foreach (var host in d.Values)
            {
                var row = storedRows.FirstOrDefault(r => GetXenObject(r) == host);
                if (row != null)
                    RedrawRow(row);
            }
        }

        private void cRow_CellDataUpdated(CheckableDataGridViewRow row)
        {
            UpdateRow(row, true);
        }

        public List<CheckableDataGridViewRow> CheckedRows
        {
            get { return storedRows.ToList().Where(r => r.Checked).ToList(); }
        }

        public void HighlightOnlyRow(int rowIndex)
        {
            if(rowIndex < 0 )
                return;
            storedRows.ForEach(r=>r.Highlighted = false);
            storedRows[rowIndex].Highlighted = true;
            UpdateRow(storedRows[rowIndex], false);
            View.DrawRowAsHighlighted(storedRows[rowIndex].Highlighted, rowIndex);
        }

        public void HighlightOnlyRow(CheckableDataGridViewRow row)
        {
            int rowMatch = storedRows.FindIndex(r => r == row);
            HighlightOnlyRow(rowMatch);
        }

        public CheckableDataGridViewRow GetRow(int rowIndex)
        {
            return storedRows[rowIndex];
        }

        public List<IXenObject> StoredXenObjects
        {
            get
            {
                return storedRows.ConvertAll(r => r.XenObject);
            }
        }

        public int SortedColumn { get; protected set; }
        public SortDirection CurrentSortDirection { get; private set; }
        protected void SetNextSortDirection(int columnIndex)
        {
            if(columnIndex != SortedColumn)
            {
                View.DrawColumnGlyph(SortDirection.None, SortedColumn);
                SortedColumn = columnIndex;
                CurrentSortDirection = SortDirection.None;
                return;
            }

            switch(CurrentSortDirection)
            {
                case SortDirection.None:
                    CurrentSortDirection = SortDirection.Ascending;
                    break;
                case SortDirection.Ascending:
                    CurrentSortDirection = SortDirection.Descending;
                    break;
                case SortDirection.Descending:
                    CurrentSortDirection = SortDirection.None;
                    break;
            }

            View.DrawColumnGlyph(CurrentSortDirection, SortedColumn);
        }

        private void RedrawRow(CheckableDataGridViewRow row)
        {
            //Should start the cell update => cell updated event => UpdateRow => trigger row updated event
            CheckableDataGridViewRow cRow = storedRows.FirstOrDefault(r => r == row);
            cRow.BeginCellUpdate();

            if(cRow.Checked)
                ToggleRowChecked(cRow.Index);
        }

        /// <summary>
        /// Triggers row update event
        /// Pass in the replacement row
        /// </summary>
        /// <param name="toUpdate">Replacement Row</param>
        /// <param name="refreshGrid"></param>
        private void UpdateRow(CheckableDataGridViewRow toUpdate, bool refreshGrid)
        {
            if (toUpdate == null)
                return;

            int indexToUpdate;

            Program.AssertOnEventThread();
           
            indexToUpdate = ReplaceStoredRow(toUpdate);

            if (indexToUpdate >= storedRows.Count || indexToUpdate < 0)
            {
                log.DebugFormat("Could not update row '{0}'; Stored rows contain '{1}' items", indexToUpdate,
                                storedRows.Count);
                return;
            }

            View.DrawUpdatedRow(storedRows[indexToUpdate].CellText, storedRows[indexToUpdate].CellDataLoaded,
                                storedRows[indexToUpdate].Disabled, indexToUpdate);

            View.TriggerRowUpdatedEvent(indexToUpdate, refreshGrid);
        }

        private int ReplaceStoredRow(CheckableDataGridViewRow toUpdate)
        {
            if (storedRows == null)
                return -1;
                
            CheckableDataGridViewRow lookupRow = storedRows.FirstOrDefault(r => r == toUpdate);
            if(lookupRow == null)
                return -1;

            int indexToUpdate = lookupRow.Index;
            if (indexToUpdate >= storedRows.Count || indexToUpdate < 0)
            {
                log.DebugFormat("Unexpected index in ReplaceStoredRow row '{0}'; Stored rows contain '{1}' items", indexToUpdate, storedRows.Count);
                return -1;
            }
            storedRows.Remove(storedRows[indexToUpdate]);
            storedRows.Insert(indexToUpdate, toUpdate); 
            return indexToUpdate;
        }

        private void ToggleDisableOtherRowsInContext(int checkedRowIndex)
        {
            if (storedRows.All(r => !r.Checked))
                ToggleDisableIfLastStanding(checkedRowIndex);
        }

        private void ToggleDisableIfLastStanding(int checkedRowIndex)
        {
            foreach (CheckableDataGridViewRow otherRow in storedRows.ToList())
            {
                if (otherRow.LockDisabledState)
                    continue;

                if (otherRow.Index != checkedRowIndex && otherRow.Disabled)
                {
                    otherRow.Disabled = !otherRow.Disabled;
                    View.DrawRowAsDisabled(otherRow.Disabled, otherRow.Index);
                }
            }
        }

        public void SetRowDisabledWithReason(int rowIndex, string information, bool disabled)
        {
            if(rowIndex < 0 || rowIndex >= storedRows.Count)
                return;

            Program.AssertOnEventThread();

            storedRows[rowIndex].DisabledReason = information;
            storedRows[rowIndex].Disabled = disabled;
            storedRows[rowIndex].LockDisabledState = disabled;
            
            View.DrawSetRowInformation(rowIndex, information);
            View.DrawRowAsDisabled(disabled, rowIndex);
            View.DrawRowAsLocked(disabled, rowIndex);
        }

        public void ToggleRowChecked(int rowIndex)
        {
  
            if(rowIndex >= storedRows.Count || rowIndex < 0)
            {
                log.DebugFormat("Could not toggle row '{0}'; Stored rows contain '{1}' items", rowIndex, storedRows.Count);
                return;
            }

            if(storedRows[rowIndex].LockDisabledState)
                return;
            
            if(storedRows[rowIndex].Disabled)
            {
                storedRows[rowIndex].Checked = false;
                View.DrawRowAsChecked(storedRows[rowIndex].Checked, rowIndex);
                return;
            }
                

            if (!storedRows[rowIndex].XenObject.Connection.IsConnected)
            {
                storedRows[rowIndex].Checked = false;
                View.DrawRowAsChecked(storedRows[rowIndex].Checked, rowIndex);
                ToggleDisableOtherRowsInContext(rowIndex);
                SetRowDisabledWithReason(rowIndex, Messages.POOL_OR_HOST_IS_NOT_CONNECTED, true);
                return;
            }

            storedRows[rowIndex].Checked = !storedRows[rowIndex].Checked;
            View.DrawRowAsChecked(storedRows[rowIndex].Checked, rowIndex);
            ToggleDisableOtherRowsInContext(rowIndex);
            View.TriggerRowCheckedEvent(rowIndex);
        }

        public void ToggleRowsChecked(List<CheckableDataGridViewRow> rows)
        {
            foreach (CheckableDataGridViewRow row in rows)
            {
                CheckableDataGridViewRow row1 = row;
                int rowMatch = storedRows.FindIndex(r=>r==row1);
                if(rowMatch < 0)
                    return;

                ToggleRowChecked(rowMatch);
            }
        }


        public void ToggleRowHighlighted(CheckableDataGridViewRow row)
        {
            int rowMatch = storedRows.FindIndex(r => r == row);
            if (rowMatch < 0)
                return;

            ToggleRowHighlighted(rowMatch);
        }

        public void ToggleRowHighlighted(int rowIndex)
        {
            if(rowIndex < storedRows.Count)
            {
                storedRows[rowIndex].Highlighted = !storedRows[rowIndex].Highlighted;
                View.DrawRowAsHighlighted(storedRows[rowIndex].Highlighted, rowIndex);
            }
        }

        public void CellContentClicked(int columnIndex, int rowIndex, bool senderRowDisabled)
        {
            if(rowIndex < 0 || columnIndex < 0)
                return;

            if (columnIndex == CheckboxIndex && !senderRowDisabled)
            {
                ToggleRowChecked(rowIndex);
            }
            HighlightOnlyRow(rowIndex);
        }

        #region IDisposable Members
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed;
        public void Dispose(bool disposing)
        {
            if(!disposed)
            {
                foreach (CheckableDataGridViewRow row in storedRows)
                {
                    row.CellDataUpdated -= cRow_CellDataUpdated;
                    IXenObject xenObject = GetXenObject(row);
                    if (xenObject != null && xenObject.Connection != null)
                        xenObject.Connection.Cache.DeregisterBatchCollectionChanged<Host>(HostBatchCollectionChanged);
                }

                if(disposing)
                {
                    //Managed objects to be disposed here
                }
                disposed = true;
            }
        }

        #endregion
    }
}
