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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using XenAdmin.Properties;

namespace XenAdmin.Controls
{
    public class LicenseCheckableDataGridView : CheckableDataGridView.CheckableDataGridView, ILicenseCheckableDataGridViewView
    {
        public delegate void RefreshAllEvent(object sender, EventArgs e);
        public event RefreshAllEvent RefreshAll;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataGridViewImageColumn StatusImageColumn
        {
            get
            {
                Debug.Assert(_statusImageColumn != null);
                return _statusImageColumn;
            }
            set { _statusImageColumn = value; }
        }

        private DataGridViewImageColumn _statusImageColumn;

        private LicenseCheckableDataGridViewController LicenseController
        {
            get { return Controller as LicenseCheckableDataGridViewController; }
        }

        public LicenseCheckableDataGridView()
        {
            ColumnHeaderMouseClick += LicenseCheckableDataGridView_ColumnHeaderMouseClick;
        }

        private void LicenseCheckableDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;

            LicenseController.SortAndRefreshOnColumnClick(e.ColumnIndex);
        }

        public void SortAndRefresh()
        {
            LicenseController.SortAndRefresh(LicenseController.SortedColumn);
        }

        public void SetStatusIcon(int rowIndex, LicenseDataGridViewRow.Status status)
        {
            LicenseController.SetStatusIcon(rowIndex, status);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawStatusIcon(int rowIndex, LicenseDataGridViewRow.Status status)
        {
            Program.Invoke(Program.MainWindow, delegate
            {
                SuspendLayout();
                try
                {
                    if (StatusImageColumn == null)
                        return;

                    if (rowIndex < 0 || rowIndex >= Rows.Count)
                        return;

                    var r = Rows[rowIndex];
                    if (StatusImageColumn.Index >= r.Cells.Count)
                        return;

                    DataGridViewCell cell = new DataGridViewImageCell
                    {
                        ValueIsIcon = true,
                        ValueType = typeof(Bitmap),
                        Value = new Bitmap(1, 1)
                    };

                    if (status == LicenseDataGridViewRow.Status.Information)
                        cell.Value = Resources._000_Alert2_h32bit_16;
                    if (status == LicenseDataGridViewRow.Status.Warning)
                        cell.Value = Resources._000_error_h32bit_16;
                    if (status == LicenseDataGridViewRow.Status.Ok)
                        cell.Value = Resources._000_Tick_h32bit_16;

                    if (r.Cells[StatusImageColumn.Index] is DataGridViewImageCell)
                        r.Cells[StatusImageColumn.Index] = cell;
                }
                finally
                {
                    ResumeLayout();
                }
            });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void TriggerRefreshAllEvent()
        {
            if (RefreshAll != null)
                RefreshAll(this, EventArgs.Empty);
        }
    }
}
