namespace XenAdmin.TabPages
{
    partial class AlertSummaryPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DeregisterEventHandlers();

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlertSummaryPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.LabelCappingEntries = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1 = new XenAdmin.Controls.ToolStripEx();
            this.toolStripDropDownSeveritiesFilter = new XenAdmin.Controls.FilterSeveritiesToolStripDropDownButton();
            this.toolStripDropDownButtonServerFilter = new XenAdmin.Controls.FilterLocationToolStripDropDownButton();
            this.toolStripDropDownButtonDateFilter = new XenAdmin.Controls.FilterDatesToolStripDropDownButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonExportAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSplitButtonDismiss = new System.Windows.Forms.ToolStripSplitButton();
            this.tsmiDismissAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDismissSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLabelFiltersOnOff = new System.Windows.Forms.ToolStripLabel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.GridViewAlerts = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnExpand = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnSeverity = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnActions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridViewAlerts)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // LabelCappingEntries
            // 
            resources.ApplyResources(this.LabelCappingEntries, "LabelCappingEntries");
            this.LabelCappingEntries.ForeColor = System.Drawing.SystemColors.ControlText;
            this.LabelCappingEntries.Name = "LabelCappingEntries";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Gainsboro;
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toolStrip1.ClickThrough = true;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownSeveritiesFilter,
            this.toolStripDropDownButtonServerFilter,
            this.toolStripDropDownButtonDateFilter,
            this.toolStripSeparator3,
            this.toolStripButtonRefresh,
            this.toolStripSeparator1,
            this.toolStripButtonExportAll,
            this.toolStripSplitButtonDismiss,
            this.toolStripLabelFiltersOnOff});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripDropDownSeveritiesFilter
            // 
            this.toolStripDropDownSeveritiesFilter.AutoToolTip = false;
            resources.ApplyResources(this.toolStripDropDownSeveritiesFilter, "toolStripDropDownSeveritiesFilter");
            this.toolStripDropDownSeveritiesFilter.Name = "toolStripDropDownSeveritiesFilter";
            this.toolStripDropDownSeveritiesFilter.FilterChanged += new System.Action(this.toolStripDropDownSeveritiesFilter_FilterChanged);
            // 
            // toolStripDropDownButtonServerFilter
            // 
            this.toolStripDropDownButtonServerFilter.AutoToolTip = false;
            resources.ApplyResources(this.toolStripDropDownButtonServerFilter, "toolStripDropDownButtonServerFilter");
            this.toolStripDropDownButtonServerFilter.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
            this.toolStripDropDownButtonServerFilter.Name = "toolStripDropDownButtonServerFilter";
            this.toolStripDropDownButtonServerFilter.FilterChanged += new System.Action(this.toolStripDropDownButtonServerFilter_FilterChanged);
            // 
            // toolStripDropDownButtonDateFilter
            // 
            this.toolStripDropDownButtonDateFilter.AutoToolTip = false;
            this.toolStripDropDownButtonDateFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripDropDownButtonDateFilter, "toolStripDropDownButtonDateFilter");
            this.toolStripDropDownButtonDateFilter.Name = "toolStripDropDownButtonDateFilter";
            this.toolStripDropDownButtonDateFilter.FilterChanged += new System.Action(this.toolStripDropDownButtonDateFilter_FilterChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.AutoToolTip = false;
            resources.ApplyResources(this.toolStripButtonRefresh, "toolStripButtonRefresh");
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonExportAll
            // 
            this.toolStripButtonExportAll.AutoToolTip = false;
            resources.ApplyResources(this.toolStripButtonExportAll, "toolStripButtonExportAll");
            this.toolStripButtonExportAll.Name = "toolStripButtonExportAll";
            this.toolStripButtonExportAll.Click += new System.EventHandler(this.toolStripButtonExportAll_Click);
            // 
            // toolStripSplitButtonDismiss
            // 
            this.toolStripSplitButtonDismiss.AutoToolTip = false;
            this.toolStripSplitButtonDismiss.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButtonDismiss.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDismissAll,
            this.tsmiDismissSelected});
            resources.ApplyResources(this.toolStripSplitButtonDismiss, "toolStripSplitButtonDismiss");
            this.toolStripSplitButtonDismiss.Name = "toolStripSplitButtonDismiss";
            this.toolStripSplitButtonDismiss.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStripSplitButtonDismiss_DropDownItemClicked);
            // 
            // tsmiDismissAll
            // 
            this.tsmiDismissAll.Name = "tsmiDismissAll";
            resources.ApplyResources(this.tsmiDismissAll, "tsmiDismissAll");
            this.tsmiDismissAll.Click += new System.EventHandler(this.tsmiDismissAll_Click);
            // 
            // tsmiDismissSelected
            // 
            this.tsmiDismissSelected.Name = "tsmiDismissSelected";
            resources.ApplyResources(this.tsmiDismissSelected, "tsmiDismissSelected");
            this.tsmiDismissSelected.Click += new System.EventHandler(this.tsmiDismissSelected_Click);
            // 
            // toolStripLabelFiltersOnOff
            // 
            this.toolStripLabelFiltersOnOff.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.toolStripLabelFiltersOnOff, "toolStripLabelFiltersOnOff");
            this.toolStripLabelFiltersOnOff.Name = "toolStripLabelFiltersOnOff";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.LabelCappingEntries, 1, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // GridViewAlerts
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            this.GridViewAlerts.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.GridViewAlerts, "GridViewAlerts");
            this.GridViewAlerts.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.GridViewAlerts.BackgroundColor = System.Drawing.SystemColors.Window;
            this.GridViewAlerts.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.GridViewAlerts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.GridViewAlerts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnExpand,
            this.ColumnSeverity,
            this.ColumnMessage,
            this.ColumnLocation,
            this.ColumnDate,
            this.ColumnActions});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.GridViewAlerts.DefaultCellStyle = dataGridViewCellStyle7;
            this.GridViewAlerts.GridColor = System.Drawing.SystemColors.ControlDark;
            this.GridViewAlerts.MultiSelect = true;
            this.GridViewAlerts.Name = "GridViewAlerts";
            this.GridViewAlerts.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridViewAlerts_CellClick);
            this.GridViewAlerts.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridViewAlerts_CellDoubleClick);
            this.GridViewAlerts.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.GridViewAlerts_ColumnHeaderMouseClick);
            this.GridViewAlerts.SelectionChanged += new System.EventHandler(this.GridViewAlerts_SelectionChanged);
            this.GridViewAlerts.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.GridViewAlerts_SortCompare);
            this.GridViewAlerts.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GridViewAlerts_KeyDown);
            // 
            // ColumnExpand
            // 
            this.ColumnExpand.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle2.NullValue = null;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.ColumnExpand.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.ColumnExpand, "ColumnExpand");
            this.ColumnExpand.Name = "ColumnExpand";
            this.ColumnExpand.ReadOnly = true;
            this.ColumnExpand.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColumnSeverity
            // 
            this.ColumnSeverity.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle3.NullValue = null;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.ColumnSeverity.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.ColumnSeverity, "ColumnSeverity");
            this.ColumnSeverity.Name = "ColumnSeverity";
            this.ColumnSeverity.ReadOnly = true;
            this.ColumnSeverity.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnSeverity.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ColumnMessage
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnMessage.DefaultCellStyle = dataGridViewCellStyle4;
            this.ColumnMessage.FillWeight = 200F;
            resources.ApplyResources(this.ColumnMessage, "ColumnMessage");
            this.ColumnMessage.Name = "ColumnMessage";
            this.ColumnMessage.ReadOnly = true;
            // 
            // ColumnLocation
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnLocation.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.ColumnLocation, "ColumnLocation");
            this.ColumnLocation.Name = "ColumnLocation";
            this.ColumnLocation.ReadOnly = true;
            // 
            // ColumnDate
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.ColumnDate.DefaultCellStyle = dataGridViewCellStyle6;
            resources.ApplyResources(this.ColumnDate, "ColumnDate");
            this.ColumnDate.Name = "ColumnDate";
            this.ColumnDate.ReadOnly = true;
            this.ColumnDate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColumnActions
            // 
            this.ColumnActions.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnActions, "ColumnActions");
            this.ColumnActions.Name = "ColumnActions";
            this.ColumnActions.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnActions.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // AlertSummaryPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.GridViewAlerts);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "AlertSummaryPage";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridViewAlerts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx GridViewAlerts;
        private System.Windows.Forms.Label LabelCappingEntries;
        private XenAdmin.Controls.ToolStripEx toolStrip1;
        private XenAdmin.Controls.FilterDatesToolStripDropDownButton toolStripDropDownButtonDateFilter;
        private System.Windows.Forms.ToolStripButton toolStripButtonExportAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private XenAdmin.Controls.FilterLocationToolStripDropDownButton toolStripDropDownButtonServerFilter;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private XenAdmin.Controls.FilterSeveritiesToolStripDropDownButton toolStripDropDownSeveritiesFilter;
        private System.Windows.Forms.ToolStripLabel toolStripLabelFiltersOnOff;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonDismiss;
        private System.Windows.Forms.ToolStripMenuItem tsmiDismissAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiDismissSelected;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.DataGridViewImageColumn ColumnExpand;
        private System.Windows.Forms.DataGridViewImageColumn ColumnSeverity;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnMessage;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnActions;
    }
}