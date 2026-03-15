/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Drawing;
using System.Windows.Forms;

namespace XenAdmin.Controls.DataGridViewEx
{
    [ToolboxBitmap(typeof(DataGridView))]
    public partial class DataGridViewEx : DataGridView
    {
        private DataGridViewCellStyle EnabledStyle;
        private DataGridViewCellStyle EnabledHiddenStyle;
        private DataGridViewCellStyle EnabledUnfocusedStyle;
        private DataGridViewCellStyle DisabledStyle;
        private DataGridViewCellStyle DisabledRowStyle;
        private DataGridViewCellStyle DisabledHiddenStyle;
        private bool hideSelection = false;

        public DataGridViewEx()
        {
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToResizeRows = false;
            this.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.MultiSelect = false;
            this.RowHeadersVisible = false;
            this.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ShowCellErrors = false;
            this.ShowEditingIcon = false;
            this.ShowRowErrors = false;
            this.StandardTab = true;
            this.DoubleBuffered = true;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            InitializeComponent();
            RefreshTheme();
        }

        private void SetUpDataGridViewExCellStyles()
        {
            EnabledStyle = new DataGridViewCellStyle();
            EnabledHiddenStyle = new DataGridViewCellStyle();
            EnabledUnfocusedStyle = new DataGridViewCellStyle();
            DisabledStyle = new DataGridViewCellStyle();
            DisabledHiddenStyle = new DataGridViewCellStyle();
            DisabledRowStyle = new DataGridViewCellStyle();

            if (Type.GetType("Mono.Runtime") != null)
            {
                // These fonts are null by default
                // Under Mono some functions produce exceptions due to that
                // So set them to some value

                var CellFont = new Font(Program.DefaultFont.FontFamily, Program.DefaultFont.Size - 1f);

                EnabledStyle.Font = CellFont;
                EnabledHiddenStyle.Font = CellFont;
                DisabledStyle.Font = CellFont;
                DisabledHiddenStyle.Font = CellFont;
                DisabledRowStyle.Font = CellFont;
                EnabledUnfocusedStyle.Font = CellFont;
            }

            EnabledStyle.BackColor = ThemeBackgroundColor;
            EnabledStyle.ForeColor = SystemColors.ControlText;
            EnabledStyle.SelectionBackColor = SystemColors.Highlight;
            EnabledStyle.SelectionForeColor = SystemColors.HighlightText;

            EnabledHiddenStyle.BackColor = ThemeBackgroundColor;
            EnabledHiddenStyle.ForeColor = SystemColors.ControlText;
            EnabledHiddenStyle.SelectionBackColor = ThemeBackgroundColor;
            EnabledHiddenStyle.SelectionForeColor = SystemColors.ControlText;

            //EnabledUnfocusedStyle.BackColor = ThemeBackgroundColor;
            //EnabledUnfocusedStyle.ForeColor = SystemColors.ControlText;
            //EnabledUnfocusedStyle.SelectionBackColor = ThemeBackgroundColor;
            //EnabledUnfocusedStyle.SelectionForeColor = SystemColors.ControlText;

            DisabledStyle.BackColor = ThemeBackgroundColor;
            DisabledStyle.ForeColor = SystemColors.GrayText;
            DisabledStyle.SelectionBackColor = SystemColors.ControlDark;
            DisabledStyle.SelectionForeColor = SystemColors.ControlText;

            DisabledHiddenStyle.BackColor = ThemeBackgroundColor;
            DisabledHiddenStyle.ForeColor = SystemColors.GrayText;
            DisabledHiddenStyle.SelectionBackColor = ThemeBackgroundColor;
            DisabledHiddenStyle.SelectionForeColor = SystemColors.GrayText;

            DisabledRowStyle.BackColor = ThemeBackgroundColor;
            DisabledRowStyle.ForeColor = SystemColors.GrayText;
            DisabledRowStyle.SelectionBackColor = ThemeBackgroundColor;
            DisabledRowStyle.SelectionForeColor = SystemColors.GrayText;
        }

        private void RefreshTheme()
        {
            SetUpDataGridViewExCellStyles();
            DefaultCellStyle = GetCellStyle(Enabled, HideSelection, Focused);
            this.BackgroundColor = ThemeBackgroundColor;
            this.GridColor = ThemeBackgroundColor;
        }

        public Color ThemeBackgroundColor
        {
            get
            {
                return (Application.RenderWithVisualStyles || !adjustColorsForClassic) ? System.Drawing.SystemColors.Window : System.Drawing.SystemColors.Control;
            }
        }

        private bool adjustColorsForClassic = true;
        [DefaultValue(true)]
        public bool AdjustColorsForClassic
        {
            get
            {
                return adjustColorsForClassic;
            }
            set
            {
                adjustColorsForClassic = value;
                RefreshTheme();
            }
        }

        [DefaultValue(false)]
        public new bool AllowUserToAddRows
        {
            get
            {
                return base.AllowUserToAddRows;
            }
            set
            {
                base.AllowUserToAddRows = value;
            }
        }

        [DefaultValue(false)]
        public new bool AllowUserToDeleteRows
        {
            get
            {
                return base.AllowUserToDeleteRows;
            }
            set
            {
                base.AllowUserToDeleteRows = value;
            }
        }

        [DefaultValue(false)]
        public new bool AllowUserToResizeRows
        {
            get
            {
                return base.AllowUserToResizeRows;
            }
            set
            {
                base.AllowUserToResizeRows = value;
            }
        }

        [DefaultValue(DataGridViewAutoSizeColumnsMode.Fill)]
        public new DataGridViewAutoSizeColumnsMode AutoSizeColumnsMode
        {
            get
            {
                return base.AutoSizeColumnsMode;
            }
            set
            {
                base.AutoSizeColumnsMode = value;
            }
        }

        [DefaultValue(BorderStyle.Fixed3D)]
        public new BorderStyle BorderStyle
        {
            get
            {
                return base.BorderStyle;
            }
            set
            {
                base.BorderStyle = value;
            }
        }

        [DefaultValue(DataGridViewEditMode.EditProgrammatically)]
        public new DataGridViewEditMode EditMode
        {
            get
            {
                return base.EditMode;
            }
            set
            {
                base.EditMode = value;
            }
        }

        [DefaultValue(typeof(Color), "Window")]
        public new Color GridColor
        {
            get
            {
                return base.GridColor;
            }
            set
            {
                base.GridColor = value;
            }
        }

        [DefaultValue(false)]
        public new bool MultiSelect
        {
            get
            {
                return base.MultiSelect;
            }
            set
            {
                base.MultiSelect = value;
            }
        }

        [DefaultValue(false)]
        public new bool RowHeadersVisible
        {
            get
            {
                return base.RowHeadersVisible;
            }
            set
            {
                base.RowHeadersVisible = value;
            }
        }

        [DefaultValue(DataGridViewSelectionMode.FullRowSelect)]
        public new DataGridViewSelectionMode SelectionMode
        {
            get
            {
                return base.SelectionMode;
            }
            set
            {
                base.SelectionMode = value;
            }
        }

        [DefaultValue(false)]
        public new bool ShowCellErrors
        {
            get
            {
                return base.ShowCellErrors;
            }
            set
            {
                base.ShowCellErrors = value;
            }
        }

        [DefaultValue(false)]
        public new bool ShowEditingIcon
        {
            get
            {
                return base.ShowEditingIcon;
            }
            set
            {
                base.ShowEditingIcon = value;
            }
        }

        [DefaultValue(false)]
        public new bool ShowRowErrors
        {
            get
            {
                return base.ShowRowErrors;
            }
            set
            {
                base.ShowRowErrors = value;
            }
        }

        [DefaultValue(true)]
        public new bool StandardTab
        {
            get
            {
                return base.StandardTab;
            }
            set
            {
                base.StandardTab = value;
            }
        }

        public new bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
                
                DefaultCellStyle = GetCellStyle(value, HideSelection, Focused);
                BackgroundColor = value ? ThemeBackgroundColor : SystemColors.Control;
                GridColor = value ? ThemeBackgroundColor : SystemColors.Control;
                ColumnHeadersDefaultCellStyle = value ? EnabledStyle : DisabledStyle;

                foreach(DataGridViewRow r in Rows)
                {
                    DataGridViewExRow row = r as DataGridViewExRow;

                    if (row == null)
                        continue;

                    row.UpdateDefaultCellStyle();
                }
            }
        }
     
        /// <summary>
        /// Finds the distance from the top of the control to the top of the given row index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int RowDepth(int index)
        {
            int output = 0;
            for (int i = 0; i < index; i++)
            {
                output += Rows[i].Height;
            }
            return output;
        }

        public DataGridViewCellStyle GetCellStyle(bool enabled, bool hide, bool focused)
        {
            if (enabled)
                if (hide)
                    return EnabledHiddenStyle;
                else if (focused)
                    return EnabledStyle;
                else
                    return EnabledUnfocusedStyle;
            else
                return hide ? DisabledHiddenStyle : DisabledStyle;
        }

        public DataGridViewCellStyle GetRowCellStyle(bool enabled, bool gridviewEnabled)
        {
            if(gridviewEnabled)
                return enabled ? EnabledStyle : DisabledRowStyle;
            else
                return enabled ? DisabledStyle : DisabledHiddenStyle;
        }

        [DefaultValue(false)]
        public bool HideSelection
        {
            get
            {
                return hideSelection;
            }
            set
            {
                DefaultCellStyle = GetCellStyle(Enabled, value, Focused);
                hideSelection = value;
            }
        }

        public int HorizontalScrollBarHeight
        {
            get { return HorizontalScrollBar.Height; }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            DefaultCellStyle = GetCellStyle(Enabled, HideSelection, true);
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            DefaultCellStyle = GetCellStyle(Enabled, HideSelection, false);
            base.OnLostFocus(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
    }
}
