namespace XenAdmin.TabPages
{
    partial class HistoryPage
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
            DeregisterEventHandlers();
            XenAdmin.Actions.ActionBase.NewAction -= Action_NewAction;

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStripTop = new System.Windows.Forms.ToolStrip();
            this.toolStripDdbFilterStatus = new XenAdmin.Controls.FilterStatusToolStripDropDownButton();
            this.toolStripDdbFilterLocation = new XenAdmin.Controls.FilterLocationToolStripDropDownButton();
            this.toolStripDdbFilterDates = new XenAdmin.Controls.FilterDatesToolStripDropDownButton();
            this.toolStripSplitButtonDismiss = new System.Windows.Forms.ToolStripSplitButton();
            this.tsmiDismissAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDismissSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLabelFiltersOnOff = new System.Windows.Forms.ToolStripLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnExpander = new System.Windows.Forms.DataGridViewImageColumn();
            this.columnStatus = new System.Windows.Forms.DataGridViewImageColumn();
            this.columnMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnActions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStripTop.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripTop
            // 
            resources.ApplyResources(this.toolStripTop, "toolStripTop");
            this.toolStripTop.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toolStripTop.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDdbFilterStatus,
            this.toolStripDdbFilterLocation,
            this.toolStripDdbFilterDates,
            this.toolStripSplitButtonDismiss,
            this.toolStripLabelFiltersOnOff});
            this.toolStripTop.Name = "toolStripTop";
            // 
            // toolStripDdbFilterStatus
            // 
            this.toolStripDdbFilterStatus.AutoToolTip = false;
            this.toolStripDdbFilterStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripDdbFilterStatus, "toolStripDdbFilterStatus");
            this.toolStripDdbFilterStatus.Name = "toolStripDdbFilterStatus";
            this.toolStripDdbFilterStatus.FilterChanged += new System.Action(this.toolStripDdbFilterStatus_FilterChanged);
            // 
            // toolStripDdbFilterLocation
            // 
            this.toolStripDdbFilterLocation.AutoToolTip = false;
            this.toolStripDdbFilterLocation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripDdbFilterLocation, "toolStripDdbFilterLocation");
            this.toolStripDdbFilterLocation.Name = "toolStripDdbFilterLocation";
            this.toolStripDdbFilterLocation.FilterChanged += new System.Action(this.toolStripDdbFilterLocation_FilterChanged);
            // 
            // toolStripDdbFilterDates
            // 
            this.toolStripDdbFilterDates.AutoToolTip = false;
            this.toolStripDdbFilterDates.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripDdbFilterDates, "toolStripDdbFilterDates");
            this.toolStripDdbFilterDates.Name = "toolStripDdbFilterDates";
            this.toolStripDdbFilterDates.FilterChanged += new System.Action(this.toolStripDdbFilterDates_FilterChanged);
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
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Gainsboro;
            this.tableLayoutPanel1.Controls.Add(this.dataGridView, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // dataGridView
            // 
            this.dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnExpander,
            this.columnStatus,
            this.columnMessage,
            this.columnLocation,
            this.columnDate,
            this.columnActions});
            resources.ApplyResources(this.dataGridView, "dataGridView");
            this.dataGridView.GridColor = System.Drawing.SystemColors.Control;
            this.dataGridView.MultiSelect = true;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // columnExpander
            // 
            this.columnExpander.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle1.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle1.NullValue")));
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.columnExpander.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.columnExpander, "columnExpander");
            this.columnExpander.Name = "columnExpander";
            // 
            // columnStatus
            // 
            this.columnStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle2.NullValue")));
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.columnStatus.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.columnStatus, "columnStatus");
            this.columnStatus.Name = "columnStatus";
            this.columnStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // columnMessage
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.columnMessage.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.columnMessage, "columnMessage");
            this.columnMessage.Name = "columnMessage";
            // 
            // columnLocation
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.columnLocation.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.columnLocation, "columnLocation");
            this.columnLocation.Name = "columnLocation";
            // 
            // columnDate
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.columnDate.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.columnDate, "columnDate");
            this.columnDate.Name = "columnDate";
            // 
            // columnActions
            // 
            this.columnActions.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnActions, "columnActions");
            this.columnActions.Name = "columnActions";
            this.columnActions.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.Gainsboro;
            this.tableLayoutPanel2.Controls.Add(this.toolStripTop, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // HistoryPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "HistoryPage";
            this.toolStripTop.ResumeLayout(false);
            this.toolStripTop.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripTop;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonDismiss;
        private System.Windows.Forms.ToolStripMenuItem tsmiDismissAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiDismissSelected;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridView;
        private XenAdmin.Controls.FilterStatusToolStripDropDownButton toolStripDdbFilterStatus;
        private XenAdmin.Controls.FilterLocationToolStripDropDownButton toolStripDdbFilterLocation;
        private XenAdmin.Controls.FilterDatesToolStripDropDownButton toolStripDdbFilterDates;
        private System.Windows.Forms.ToolStripLabel toolStripLabelFiltersOnOff;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridViewImageColumn columnExpander;
        private System.Windows.Forms.DataGridViewImageColumn columnStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnMessage;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnActions;
    }
}
