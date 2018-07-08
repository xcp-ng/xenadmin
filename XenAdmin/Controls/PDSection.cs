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
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using XenAdmin.Properties;
using System.Linq;
using XenAdmin.Commands;


namespace XenAdmin.Controls
{
    public partial class PDSection : UserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Private fields

        /// <summary>
        /// Used to persist selection over a repaint, we set this when we clear the datagridview
        /// </summary>
        private string previousSelectionKey;

        private bool inLayout;

        /// <summary>
        /// Whether the mouse is over the chevron
        /// </summary>
        private bool chevronHot;

        #endregion

        #region Delegates and Events

        /// <summary>
        /// Event for when the datagridview received focus
        /// </summary>
        public event Action<PDSection> contentReceivedFocus;

        /// <summary>
        /// Event for when the datagridview receives focus
        /// </summary>
        public event Action<PDSection> contentChangedSelection;

        public event Action<PDSection> ExpandedChanged;

        #endregion

        public PDSection()
        {
            InitializeComponent();
            SetDefaultValues();
            Contract();
            MinimumSize = new Size(0, Height);
            dataGridViewEx1.LostFocus += dataGridViewEx1_LostFocus;
            dataGridViewEx1.GotFocus += dataGridViewEx1_GotFocus;

            if (!Application.RenderWithVisualStyles)
            {
                panel1.BackColor = SystemColors.Control;
                this.BackColor = SystemColors.ControlDark;
            }
        }

        #region Accessors

        [Browsable(false)]
        public bool DisableFocusEvent { private get; set; }

        [DefaultValue("Title")]
        [Browsable(true)]
        [Localizable(true)]
        public string SectionTitle
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }

        [Browsable(false)]
        public bool IsEmpty()
        {
            return dataGridViewEx1.Rows.Count == 0;
        }

        [DefaultValue(true)]
        [Browsable(false)]
        public bool IsExpanded { get; private set; }

        [Browsable(false)]
        public bool HasNoSelection()
        {
            return dataGridViewEx1.SelectedRows.Count <= 0;
        }

        [Browsable(false)]
        public Rectangle SelectedRowBounds
        {
            get
            {
                if (HasNoSelection())
                    return new Rectangle(0, 0, 0, 0);

                int x = dataGridViewEx1.Location.X;
                int y = dataGridViewEx1.RowDepth(dataGridViewEx1.SelectedRows[0].Index) + dataGridViewEx1.Location.Y;
                int w = dataGridViewEx1.Width;
                int h = dataGridViewEx1.SelectedRows[0].Height;
                return new Rectangle(x, y, w, h);
            }
        }

        [Browsable(false)]
        public bool ShowCellToolTips
        {
            get { return dataGridViewEx1.ShowCellToolTips; }
            set { dataGridViewEx1.ShowCellToolTips = value; }
        }

        #endregion
       
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            // For keyboard access
            if (DisableFocusEvent)
                return;

            if (!IsExpanded)
                Expand();
        }

        #region Control Event Handlers

        private void dataGridViewEx1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                ExecuteCellCommandOrAction(dataGridViewEx1.Rows[e.RowIndex].Cells[e.ColumnIndex]);
        }

        private void dataGridViewEx1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                ExecuteCellCommandOrAction(dataGridViewEx1.CurrentCell);
        }

        private void dataGridViewEx1_LostFocus(object sender, EventArgs e)
        {
            dataGridViewEx1.HideSelection = true;
        }

        private void dataGridViewEx1_GotFocus(object sender, EventArgs e)
        {
            dataGridViewEx1.HideSelection = false;
            if (contentReceivedFocus != null)
                contentReceivedFocus(this);
        }

        private void dataGridViewEx1_SelectionChanged(object sender, EventArgs e)
        {
            if (inLayout)
                return;

            if (contentChangedSelection != null)
                contentChangedSelection(this);
        }

        private void dataGridViewEx1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            DataGridView.HitTestInfo i = dataGridViewEx1.HitTest(e.X, e.Y);

            if (i.RowIndex < 0 || i.RowIndex > dataGridViewEx1.RowCount)
                return;
            if (i.ColumnIndex < 0 || i.ColumnIndex > dataGridViewEx1.ColumnCount)
                return;

            dataGridViewEx1.Focus();
            var row = dataGridViewEx1.Rows[i.RowIndex];
            row.Selected = true;
            
            contextMenuStrip1.Items.Clear();
            contextMenuStrip1.Items.Add(copyToolStripMenuItem);

            var menuItems = row.Tag as IEnumerable<ToolStripMenuItem>;
            if (menuItems != null)
            {
                contextMenuStrip1.Items.Add(new ToolStripSeparator());
                contextMenuStrip1.Items.AddRange(menuItems.ToArray());
            }
            contextMenuStrip1.Show(dataGridViewEx1, dataGridViewEx1.PointToClient(MousePosition));
        }

        
        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(dataGridViewEx1.SelectedRows[0].Cells[1].Value);
        }


        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (IsExpanded)
                Contract();
            else
            {
                Expand();
                dataGridViewEx1.Focus();
            }
        }
        
        private void chevron_Click(object sender, EventArgs e)
        {
            if (IsExpanded)
                Contract();
            else
            {
                Expand();
                dataGridViewEx1.Focus();
            }
        }

        private void chevron_MouseEnter(object sender, EventArgs e)
        {
            chevronHot = true;
            RefreshChevron();
        }

        private void chevron_MouseLeave(object sender, EventArgs e)
        {
            chevronHot = false;
            RefreshChevron();
        }
 
        #endregion

        #region Private Methods

        private void SetDefaultValues()
        {
            SectionTitle = Messages.PDSECTION_TITLE;
            IsExpanded = true;
        }

        private void ExecuteCellCommandOrAction(DataGridViewCell cell)
        {
            if (cell == null)
                return;

            var command = cell.Tag as Command;
            if (command != null)
                command.Execute();

            var action = cell.Tag as Action;
            if (action != null)
                action.Invoke();
        }

        private void RefreshHeight()
        {
            if (IsExpanded)
            {
                int newHeight = dataGridViewEx1.Rows.GetRowsHeight(DataGridViewElementStates.Visible);

                int valueColWidth = dataGridViewEx1.Width - dataGridViewEx1.Columns[KeyColumn.Index].Width;
                int preferredValueColWidth =
                    dataGridViewEx1.Columns[ValueColumn.Index].GetPreferredWidth(
                        DataGridViewAutoSizeColumnMode.AllCells, true);

                int horizontalScrollBarHeight = preferredValueColWidth - 1 >= valueColWidth
                                                    ? dataGridViewEx1.HorizontalScrollBarHeight
                                                    : 0;
                
                // 3px added so we have a border (one at top of control, one between title and grid, one at bottom)
                Height = panel1.Height + newHeight + horizontalScrollBarHeight + 3;
                // this correction is needed because the anchor property of the grid drags it to fill the space we want to be a border
                // when we go from contracted (2 borders) to expanded (3 borders)
            }
            else
            {
                Height = panel1.Height + 2;
            }
        }
        
        private void RefreshChevron()
        {
            if (IsExpanded)
            {
                chevron.Image = chevronHot ? Resources.PDChevronUpOver : Resources.PDChevronUp;
            }
            else
            {
                chevron.Image = chevronHot ? Resources.PDChevronDownOver : Resources.PDChevronDown;
            }
        }

        private void AddRow(DataGridViewRow r)
        {
            dataGridViewEx1.Rows.Add(r);
            if (inLayout)
                return;

            RefreshHeight();
        }

        private void ToggleExpandedState(bool expand)
        {
            if (IsExpanded == expand)
                return;

            IsExpanded = expand;
            dataGridViewEx1.Visible = expand;
            RefreshHeight();
            RefreshChevron();

            if (ExpandedChanged != null)
                ExpandedChanged(this);
        }

        #endregion

        public void Contract()
        {
            ToggleExpandedState(false);
        }

        public void Expand()
        {
            ValueColumn.MinimumWidth = 5;
            HelpersGUI.ResizeGridViewColumnToAllCells(ValueColumn);
            ToggleExpandedState(true);
        }

        private DataGridViewExRow CreateRow(string Key, string Value)
        {
            if (!String.IsNullOrEmpty(Key))
                Key += Messages.GENERAL_PAGE_KVP_SEPARATOR;
            DataGridViewExRow r = new DataGridViewExRow();
            r.CreateCells(dataGridViewEx1);
            r.Cells[0].Value = Key;
            r.Cells[1].Value = Value;
            return r;
        }
        public void AddEntry(string Key, string Value)
        {
            var r = CreateRow(Key, Value);
            AddRow(r);
        }

        public void AddEntry(string Key, string Value, ToolStripMenuItem contextMenuItem)
        {
            AddEntry(Key, Value, new[] { contextMenuItem });
        }

        public void AddEntry(string Key, string Value, IEnumerable<ToolStripMenuItem> contextMenuItems)
        {
            var r = CreateRow(Key, Value);
            r.Tag = contextMenuItems;
            AddRow(r);
        }
        
        public void AddEntry(string Key, string Value, IEnumerable<ToolStripMenuItem> contextMenuItems, string toolTipText)
        {
            AddEntry(Key, Value, contextMenuItems);
            if (toolTipText != Key)
                dataGridViewEx1.Rows[dataGridViewEx1.RowCount - 1].Cells[0].ToolTipText = toolTipText;
        }

        public void AddEntry(string Key, string Value, Color fontColor)
        {
            var r = CreateRow(Key, Value);
            r.Cells[1].Style.ForeColor = fontColor;
            AddRow(r);
            dataGridViewEx1.DefaultCellStyle = new DataGridViewCellStyle();
        }

        public void AddEntry(string Key, string Value, IEnumerable<ToolStripMenuItem> contextMenuItems, Color fontColor)
        {
            var r = CreateRow(Key, Value);
            r.Cells[1].Style.ForeColor = fontColor;
            r.Tag = contextMenuItems;
            AddRow(r);
            dataGridViewEx1.DefaultCellStyle = new DataGridViewCellStyle();
        }

        public void AddEntry(string Key, FolderListItem Value, IEnumerable<ToolStripMenuItem> contextMenuItems)
        {
            // Special case for folders: CA-33311

            if (!String.IsNullOrEmpty(Key))
                Key += Messages.GENERAL_PAGE_KVP_SEPARATOR;
            DataGridViewExRow r = new DataGridViewExRow();
            r.Cells.Add(new DataGridViewTextBoxCell());
            r.Cells[0].Value = Key;
            r.Cells.Add(new FolderCell(Value));
            r.Tag = contextMenuItems;
            AddRow(r);
        }

        internal void AddEntryLink(string Key, string Value, IEnumerable<ToolStripMenuItem> contextMenuItems, Command command)
        {
            if (!String.IsNullOrEmpty(Key))
                Key += Messages.GENERAL_PAGE_KVP_SEPARATOR;
            DataGridViewExRow r = new DataGridViewExRow();
            r.CreateCells(dataGridViewEx1);
            r.Cells[0].Value = Key;
            r.Cells[1] = new DataGridViewLinkCell();
            r.Cells[1].Value = Value;
            r.Cells[1].Tag = command;
            r.Tag = contextMenuItems;
            AddRow(r);
        }

        internal void AddEntryLink(string Key, string Value, IEnumerable<ToolStripMenuItem> contextMenuItems, Action action)
        {
            if (!String.IsNullOrEmpty(Key))
                Key += Messages.GENERAL_PAGE_KVP_SEPARATOR;
            DataGridViewExRow r = new DataGridViewExRow();
            r.CreateCells(dataGridViewEx1);
            r.Cells[0].Value = Key;
            r.Cells[1] = new DataGridViewLinkCell();
            r.Cells[1].Value = Value;
            r.Cells[1].Tag = action;
            r.Tag = contextMenuItems;
            AddRow(r);
        }

        public void AddEntry(string Key, string Value, ToolStripMenuItem contextMenuItem, bool visible)
        {
            AddEntry(Key, Value, new[] { contextMenuItem }, visible);
        }

        public void AddEntry(string Key, string Value, IEnumerable<ToolStripMenuItem> contextMenuItems, bool visible)
        {
            var r = CreateRow(Key, Value);
            r.Tag = contextMenuItems;
            r.Visible = visible;
            AddRow(r);
        }

        public void UpdateEntryValueWithKey(string Key, string newValue, bool visible)
        {
            List<DataGridViewExRow> matchingRows = (from DataGridViewExRow row in dataGridViewEx1.Rows
                                                   where row.Cells[0].Value.ToString().Contains(Key)
                                                   select row).ToList();

            if (matchingRows.Count != 1 || matchingRows[0].Cells.Count < 2)
                return;

            matchingRows[0].Cells[1].Value = newValue;

            if (matchingRows[0].Visible != visible)
            {
                matchingRows[0].Visible = visible;
                RefreshHeight();
            }
        }

        internal void fixFirstColumnWidth(int width)
        {
            dataGridViewEx1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewEx1.Columns[0].Width = width;
        }

        /// <summary>
        /// Clears the rows in the data grid view. If performing a repaint, you can attempt to restore the previous selection with a call to RestoreSelection()
        /// </summary>
        public void ClearData()
        {
            if (dataGridViewEx1.SelectedRows.Count > 0)
                previousSelectionKey = dataGridViewEx1.SelectedRows[0].Cells[0].Value.ToString();

            dataGridViewEx1.Rows.Clear();
        }

        /// <summary>
        /// Restores selection to the row stored from the last ClearData() call
        /// </summary>
        public void RestoreSelection()
        {
            if (string.IsNullOrEmpty(previousSelectionKey))
                return;

            foreach (DataGridViewRow r in dataGridViewEx1.Rows)
            {
                if (r.Cells[0].Value.ToString() == previousSelectionKey)
                {
                    r.Cells[0].Selected = true;
                    return;
                }
            }
        }

        public void PauseLayout()
        {
            inLayout = true;
        }

        public void StartLayout()
        {
            inLayout = false;
            RefreshHeight();
        }
    }
}
