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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;
using System.ComponentModel;

namespace XenAdmin.Controls.DataGridViewEx
{
    /// <summary>
    /// Template-esque base class for displaying Pool-orientated view in a collapsable tree
    /// 
    /// Pools are given with hosts in a collapsible tree via. tick boxes. Unpooled hosts remain alone.
    /// Class contains hooks for additional columns/information which may be added as DataGridViewTextBoxCells to 
    /// this view.
    /// 
    /// Defaults are expansion cell, checkbox cell and the name which is the pool or in the case of a 
    /// standalone server, the server name
    /// </summary>
    public abstract class PoolHostDataGridViewOneCheckboxRow : CollapsingPoolHostDataGridViewRow
    {
        private class DataGridViewNameCell : DataGridViewTextBoxCell
        {
            protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
            {
                PoolHostDataGridViewOneCheckboxRow row = (PoolHostDataGridViewOneCheckboxRow)this.OwningRow;
                if (value is Host || value is Pool)
                {
                    Image hostIcon = Images.GetImage16For(value as IXenObject);
                    var iconIndent = row.IsCheckable ? 0 : 16;
                    var textIndent = iconIndent + 16;
                    base.Paint(graphics, clipBounds,
                               new Rectangle(cellBounds.X + textIndent, cellBounds.Y, cellBounds.Width - textIndent,
                                             cellBounds.Height), rowIndex, cellState, value, formattedValue,
                               errorText, cellStyle, advancedBorderStyle, paintParts);
                    if ((cellState & DataGridViewElementStates.Selected) != 0 && row.Enabled )
                    {
                        using (var brush = new SolidBrush(DataGridView.DefaultCellStyle.SelectionBackColor))
                            graphics.FillRectangle(
                                brush, cellBounds.X, cellBounds.Y, hostIcon.Width + iconIndent, cellBounds.Height);
                    }
                    else
                    {
                        //Background behind the host icon
                        using (var brush = new SolidBrush(this.DataGridView.DefaultCellStyle.BackColor))
                            graphics.FillRectangle(brush,
                                                   cellBounds.X, cellBounds.Y, hostIcon.Width + iconIndent, cellBounds.Height);
                    }

                    if (row.Enabled)
                        graphics.DrawImage(hostIcon, cellBounds.X + iconIndent, cellBounds.Y + 3, hostIcon.Width, hostIcon.Height);
                    else
                        graphics.DrawImage(hostIcon,
                                           new Rectangle(cellBounds.X + iconIndent, cellBounds.Y + 3, hostIcon.Width, hostIcon.Height),
                                           0, 0, hostIcon.Width, hostIcon.Height, GraphicsUnit.Pixel,
                                           Drawing.GreyScaleAttributes);
                }
                else
                {
                    base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
                }
            }
        }
        
        /// <summary>
        /// If the row is either a pool row or else a stanadlone host row
        /// </summary>
        public override bool IsCheckable
        {
            get { return !(IsAHostRow && HasPool); }
        }

        private bool checkBoxVisible = true;
        public bool CheckBoxVisible
        {
            get { return checkBoxVisible; }
        }

        protected override void SetStandaloneStatus(bool isStandalone)
        {
            if (IsAHostRow)
            {
                Checked = CheckState.Unchecked;
                checkBoxVisible = isStandalone;
                _hasPool = !isStandalone;
            }
        }    

        public override bool Equals(object obj)
        {
            if( obj != null && obj.GetType() == GetType() )
            {
                PoolHostDataGridViewOneCheckboxRow castRow = (PoolHostDataGridViewOneCheckboxRow) obj;
                return _nameCell.Value.ToString().Equals(castRow._nameCell.Value.ToString());
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public CheckState Checked
        {
            set
            { 
                _poolCheckBoxCell.Value = value;
            }
            get
            {
                return (CheckState)_poolCheckBoxCell.Value;
            }
        }

        protected PoolHostDataGridViewOneCheckboxRow(){}

        protected PoolHostDataGridViewOneCheckboxRow(Pool pool)
            : base(pool)
        {
            SetupCells();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host">underlying host</param>
        /// <param name="hasPool">should the host row be considered as a member of the pool or a standalone host</param>
        protected PoolHostDataGridViewOneCheckboxRow(Host host, bool hasPool)
            : base(host, hasPool)
        {
            SetupCells();
        }

        protected virtual void SetupAdditionalDetailsColumns() { }

        private void SetupCells()
        {
           _poolCheckBoxCell = new DataGridViewCheckBoxCell 
            { 
                ThreeState = true,
                TrueValue = CheckState.Checked, 
                FalseValue = CheckState.Unchecked, 
                IndeterminateValue = CheckState.Indeterminate,
            }; 

            _expansionCell = new DataGridViewImageCell();
            _nameCell = new DataGridViewNameCell();

            Cells.AddRange(_expansionCell, _poolCheckBoxCell, _nameCell);

            SetupAdditionalDetailsColumns();

            UpdateDetails();
        }

        protected void UpdateDetails()
        {
            if (Tag is Pool)
            {
                Pool pool = (Pool)Tag;
                Host master = pool.Connection.Resolve(pool.master);
                if( _poolCheckBoxCell.Value == null )
                    Checked = CheckState.Unchecked;
                SetCollapseIcon();
                _nameCell.Value = pool;
                UpdateAdditionalDetailsForPool(pool, master);

            }
            else if (Tag is Host)
            {
                Host host = (Host)Tag;
                if (_poolCheckBoxCell.Value == null)
                    Checked = CheckState.Unchecked;
                SetExpandIcon();
                _nameCell.Value = host;
                UpdateAdditionalDetailsForHost(host);

            }
        }

        protected virtual void UpdateAdditionalDetailsForPool(Pool pool, Host master) { }

        protected virtual void UpdateAdditionalDetailsForHost(Host host) { }
    }
}
