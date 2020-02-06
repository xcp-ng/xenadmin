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

using System.Collections;
using System.Collections.Generic;
using XenAdmin.Controls.CheckableDataGridView;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.LicenseManagerLicenseRowComparers;

namespace XenAdmin.Controls
{
    public class LicenseCheckableDataGridViewController : CheckableDataGridViewController
    {
        private const int DefaultSortIndex = 4;

        public LicenseCheckableDataGridViewController()
        {
            SortedColumn = DefaultSortIndex;
        }

        public LicenseCheckableDataGridViewController(ILicenseCheckableDataGridViewView view) : base(view)
        {
        }

        private ILicenseCheckableDataGridViewView LicenseView
        {
            get { return View as ILicenseCheckableDataGridViewView; }
        }

        public void SetStatusIcon(int rowIndex, LicenseDataGridViewRow.Status status)
        {
            LicenseView.DrawStatusIcon(rowIndex, status);
        }

        /// <summary>
        /// Sorts and refreshes without cycling the sort mode
        /// </summary>
        public void SortAndRefresh(int columnIndex)
        {
            SortAndRefresh(columnIndex, false);
        }

        /// <summary>
        /// Sorts and refreshes after cycling the sort mode
        /// </summary>
        public void SortAndRefreshOnColumnClick(int columnIndex)
        {
            SortAndRefresh(columnIndex, true);
        }

        /// <summary>
        /// General method for sort and refresh
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="cycleSortOrder">If true then the sort order will be cycled to the next mdoe before sorting</param>
        private void SortAndRefresh(int columnIndex, bool cycleSortOrder)
        {
            IComparer<CheckableDataGridViewRow> comparer = ComparerForColumn(columnIndex);

            if (cycleSortOrder)
                SetNextSortDirection(columnIndex);

            if (CurrentSortDirection == SortDirection.None)
                comparer = ComparerForColumn(DefaultSortIndex);

            if (comparer == null || columnIndex < 0)
                return;

            Program.AssertOnEventThread();

            View.SuspendDrawing();
            try
            {
                View.DrawAllRowsAsClearedMW();
                storedRows.Sort(comparer);

                if(CurrentSortDirection == SortDirection.Descending)
                    storedRows.Reverse();

                foreach (CheckableDataGridViewRow row in storedRows)
                {
                    View.DrawRowMW(row);
                    LicenseDataGridViewRow lRow = row as LicenseDataGridViewRow;
                    if (lRow == null)
                        continue;
                    LicenseView.DrawStatusIcon(row.Index, lRow.RowStatus);
                    if (row.Highlighted)
                        View.DrawRowAsHighlightedMW(true, row.Index);
                }
            }
            finally
            {
                View.ResumeDrawing();
            }
        }

        private IComparer<CheckableDataGridViewRow> ComparerForColumn(int index)
        {
            switch (index)
            {
                case 1:
                    return new NameColumnComparer();
                case 2:
                    return new ProductColumnComparer();
                case 4: 
                   return new ExpiryComparer();
                default:
                    return null;
            }
        }
    }
}
