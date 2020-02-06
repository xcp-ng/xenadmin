namespace XenAdmin.Dialogs
{
    partial class ConversionDialog
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
            if (disposing && (components != null))
            {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConversionDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewConversions = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnVm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSourceServer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnStartTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnFinishTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnStatus = new System.Windows.Forms.DataGridViewImageColumn();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewDetails = new System.Windows.Forms.DataGridView();
            this.columnKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.labelDetails = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextItemRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.contextItemFetchLogs = new System.Windows.Forms.ToolStripMenuItem();
            this.contextItemRetry = new System.Windows.Forms.ToolStripMenuItem();
            this.contextItemCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanelTop = new System.Windows.Forms.TableLayoutPanel();
            this.toolStripTop = new XenAdmin.Controls.ToolStripEx();
            this.toolStripButtonNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRetry = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDdbFilterStatus = new XenAdmin.Controls.FilterStatusToolStripDropDownButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSplitButtonRefresh = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripMenuItemRefreshSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRefreshAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonClear = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSplitButtonLogs = new System.Windows.Forms.ToolStripSplitButton();
            this.menuItemFetchSelectedLog = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFetchAllLogs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabelFiltersOnOff = new System.Windows.Forms.ToolStripLabel();
            this.timerVpx = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLinkLabel = new XenAdmin.Dialogs.ConversionDialog.ActionableLinkLabel();
            this.tableLayoutMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConversions)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDetails)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPanelTop.SuspendLayout();
            this.toolStripTop.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutMain
            // 
            resources.ApplyResources(this.tableLayoutMain, "tableLayoutMain");
            this.tableLayoutMain.Controls.Add(this.dataGridViewConversions, 0, 0);
            this.tableLayoutMain.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutMain.Controls.Add(this.buttonClose, 1, 1);
            this.tableLayoutMain.Name = "tableLayoutMain";
            // 
            // dataGridViewConversions
            // 
            this.dataGridViewConversions.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewConversions.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewConversions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewConversions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnVm,
            this.ColumnSourceServer,
            this.ColumnStartTime,
            this.ColumnFinishTime,
            this.ColumnStatus});
            resources.ApplyResources(this.dataGridViewConversions, "dataGridViewConversions");
            this.dataGridViewConversions.Name = "dataGridViewConversions";
            this.dataGridViewConversions.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewConversions.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewConversions.SelectionChanged += new System.EventHandler(this.dataGridViewConversions_SelectionChanged);
            this.dataGridViewConversions.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGridViewConversions_SortCompare);
            this.dataGridViewConversions.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridViewConversions_KeyUp);
            this.dataGridViewConversions.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGridViewConversions_MouseUp);
            // 
            // ColumnVm
            // 
            resources.ApplyResources(this.ColumnVm, "ColumnVm");
            this.ColumnVm.Name = "ColumnVm";
            this.ColumnVm.ReadOnly = true;
            // 
            // ColumnSourceServer
            // 
            resources.ApplyResources(this.ColumnSourceServer, "ColumnSourceServer");
            this.ColumnSourceServer.Name = "ColumnSourceServer";
            this.ColumnSourceServer.ReadOnly = true;
            // 
            // ColumnStartTime
            // 
            resources.ApplyResources(this.ColumnStartTime, "ColumnStartTime");
            this.ColumnStartTime.Name = "ColumnStartTime";
            this.ColumnStartTime.ReadOnly = true;
            // 
            // ColumnFinishTime
            // 
            resources.ApplyResources(this.ColumnFinishTime, "ColumnFinishTime");
            this.ColumnFinishTime.Name = "ColumnFinishTime";
            this.ColumnFinishTime.ReadOnly = true;
            // 
            // ColumnStatus
            // 
            this.ColumnStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.NullValue = "System.Drawing.Bitmap";
            this.ColumnStatus.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.ColumnStatus, "ColumnStatus");
            this.ColumnStatus.Name = "ColumnStatus";
            this.ColumnStatus.ReadOnly = true;
            this.ColumnStatus.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.dataGridViewDetails, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.labelDetails, 0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // dataGridViewDetails
            // 
            this.dataGridViewDetails.AllowUserToAddRows = false;
            this.dataGridViewDetails.AllowUserToDeleteRows = false;
            this.dataGridViewDetails.AllowUserToResizeColumns = false;
            this.dataGridViewDetails.AllowUserToResizeRows = false;
            this.dataGridViewDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            this.dataGridViewDetails.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridViewDetails.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridViewDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewDetails.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewDetails.ColumnHeadersVisible = false;
            this.dataGridViewDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnKey,
            this.columnValue});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(3);
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewDetails.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.dataGridViewDetails, "dataGridViewDetails");
            this.dataGridViewDetails.MultiSelect = false;
            this.dataGridViewDetails.Name = "dataGridViewDetails";
            this.dataGridViewDetails.ReadOnly = true;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewDetails.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewDetails.RowHeadersVisible = false;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            this.dataGridViewDetails.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // columnKey
            // 
            resources.ApplyResources(this.columnKey, "columnKey");
            this.columnKey.Name = "columnKey";
            this.columnKey.ReadOnly = true;
            // 
            // columnValue
            // 
            this.columnValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.columnValue.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.columnValue, "columnValue");
            this.columnValue.Name = "columnValue";
            this.columnValue.ReadOnly = true;
            // 
            // labelDetails
            // 
            resources.ApplyResources(this.labelDetails, "labelDetails");
            this.labelDetails.Name = "labelDetails";
            // 
            // buttonClose
            // 
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextItemRefresh,
            this.contextItemFetchLogs,
            this.contextItemRetry,
            this.contextItemCancel});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // contextItemRefresh
            // 
            this.contextItemRefresh.Name = "contextItemRefresh";
            resources.ApplyResources(this.contextItemRefresh, "contextItemRefresh");
            this.contextItemRefresh.Click += new System.EventHandler(this.toolStripMenuItemRefreshSelected_Click);
            // 
            // contextItemFetchLogs
            // 
            this.contextItemFetchLogs.Name = "contextItemFetchLogs";
            resources.ApplyResources(this.contextItemFetchLogs, "contextItemFetchLogs");
            this.contextItemFetchLogs.Click += new System.EventHandler(this.menuItemFetchSelectedLog_Click);
            // 
            // contextItemRetry
            // 
            this.contextItemRetry.Name = "contextItemRetry";
            resources.ApplyResources(this.contextItemRetry, "contextItemRetry");
            this.contextItemRetry.Click += new System.EventHandler(this.toolStripButtonRetry_Click);
            // 
            // contextItemCancel
            // 
            this.contextItemCancel.Name = "contextItemCancel";
            resources.ApplyResources(this.contextItemCancel, "contextItemCancel");
            this.contextItemCancel.Click += new System.EventHandler(this.toolStripButtonCancel_Click);
            // 
            // tableLayoutPanelTop
            // 
            resources.ApplyResources(this.tableLayoutPanelTop, "tableLayoutPanelTop");
            this.tableLayoutPanelTop.BackColor = System.Drawing.Color.Gainsboro;
            this.tableLayoutPanelTop.Controls.Add(this.toolStripTop, 0, 0);
            this.tableLayoutPanelTop.Name = "tableLayoutPanelTop";
            // 
            // toolStripTop
            // 
            resources.ApplyResources(this.toolStripTop, "toolStripTop");
            this.toolStripTop.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.toolStripTop.ClickThrough = true;
            this.toolStripTop.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNew,
            this.toolStripSeparator1,
            this.toolStripButtonCancel,
            this.toolStripButtonRetry,
            this.toolStripSeparator2,
            this.toolStripDdbFilterStatus,
            this.toolStripSeparator3,
            this.toolStripSplitButtonRefresh,
            this.toolStripButtonClear,
            this.toolStripSeparator4,
            this.toolStripSplitButtonLogs,
            this.toolStripButtonExport,
            this.toolStripLabelFiltersOnOff});
            this.toolStripTop.Name = "toolStripTop";
            // 
            // toolStripButtonNew
            // 
            this.toolStripButtonNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonNew, "toolStripButtonNew");
            this.toolStripButtonNew.Name = "toolStripButtonNew";
            this.toolStripButtonNew.Click += new System.EventHandler(this.toolStripButtonNew_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonCancel
            // 
            this.toolStripButtonCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonCancel, "toolStripButtonCancel");
            this.toolStripButtonCancel.Name = "toolStripButtonCancel";
            this.toolStripButtonCancel.Click += new System.EventHandler(this.toolStripButtonCancel_Click);
            // 
            // toolStripButtonRetry
            // 
            this.toolStripButtonRetry.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonRetry, "toolStripButtonRetry");
            this.toolStripButtonRetry.Name = "toolStripButtonRetry";
            this.toolStripButtonRetry.Click += new System.EventHandler(this.toolStripButtonRetry_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripDdbFilterStatus
            // 
            this.toolStripDdbFilterStatus.AutoToolTip = false;
            this.toolStripDdbFilterStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripDdbFilterStatus, "toolStripDdbFilterStatus");
            this.toolStripDdbFilterStatus.Name = "toolStripDdbFilterStatus";
            this.toolStripDdbFilterStatus.FilterChanged += new System.Action(this.toolStripDdbFilterStatus_FilterChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // toolStripSplitButtonRefresh
            // 
            this.toolStripSplitButtonRefresh.AutoToolTip = false;
            this.toolStripSplitButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButtonRefresh.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemRefreshSelected,
            this.toolStripMenuItemRefreshAll});
            resources.ApplyResources(this.toolStripSplitButtonRefresh, "toolStripSplitButtonRefresh");
            this.toolStripSplitButtonRefresh.Name = "toolStripSplitButtonRefresh";
            this.toolStripSplitButtonRefresh.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStripSplitButtonRefresh_DropDownItemClicked);
            // 
            // toolStripMenuItemRefreshSelected
            // 
            this.toolStripMenuItemRefreshSelected.Name = "toolStripMenuItemRefreshSelected";
            resources.ApplyResources(this.toolStripMenuItemRefreshSelected, "toolStripMenuItemRefreshSelected");
            this.toolStripMenuItemRefreshSelected.Click += new System.EventHandler(this.toolStripMenuItemRefreshSelected_Click);
            // 
            // toolStripMenuItemRefreshAll
            // 
            this.toolStripMenuItemRefreshAll.Name = "toolStripMenuItemRefreshAll";
            resources.ApplyResources(this.toolStripMenuItemRefreshAll, "toolStripMenuItemRefreshAll");
            this.toolStripMenuItemRefreshAll.Click += new System.EventHandler(this.toolStripMenuItemRefreshAll_Click);
            // 
            // toolStripButtonClear
            // 
            this.toolStripButtonClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonClear, "toolStripButtonClear");
            this.toolStripButtonClear.Name = "toolStripButtonClear";
            this.toolStripButtonClear.Click += new System.EventHandler(this.toolStripButtonClear_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // toolStripSplitButtonLogs
            // 
            this.toolStripSplitButtonLogs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButtonLogs.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFetchSelectedLog,
            this.menuItemFetchAllLogs});
            resources.ApplyResources(this.toolStripSplitButtonLogs, "toolStripSplitButtonLogs");
            this.toolStripSplitButtonLogs.Name = "toolStripSplitButtonLogs";
            this.toolStripSplitButtonLogs.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStripSplitButtonLogs_DropDownItemClicked);
            // 
            // menuItemFetchSelectedLog
            // 
            this.menuItemFetchSelectedLog.Name = "menuItemFetchSelectedLog";
            resources.ApplyResources(this.menuItemFetchSelectedLog, "menuItemFetchSelectedLog");
            this.menuItemFetchSelectedLog.Click += new System.EventHandler(this.menuItemFetchSelectedLog_Click);
            // 
            // menuItemFetchAllLogs
            // 
            this.menuItemFetchAllLogs.Name = "menuItemFetchAllLogs";
            resources.ApplyResources(this.menuItemFetchAllLogs, "menuItemFetchAllLogs");
            this.menuItemFetchAllLogs.Click += new System.EventHandler(this.menuItemFetchAllLogs_Click);
            // 
            // toolStripButtonExport
            // 
            this.toolStripButtonExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonExport, "toolStripButtonExport");
            this.toolStripButtonExport.Name = "toolStripButtonExport";
            this.toolStripButtonExport.Click += new System.EventHandler(this.toolStripButtonExport_Click);
            // 
            // toolStripLabelFiltersOnOff
            // 
            this.toolStripLabelFiltersOnOff.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.toolStripLabelFiltersOnOff, "toolStripLabelFiltersOnOff");
            this.toolStripLabelFiltersOnOff.Name = "toolStripLabelFiltersOnOff";
            // 
            // timerVpx
            // 
            this.timerVpx.Interval = 1000;
            this.timerVpx.Tick += new System.EventHandler(this.timerVpx_Tick);
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.statusLinkLabel});
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.TabStop = true;
            // 
            // statusLabel
            // 
            resources.ApplyResources(this.statusLabel, "statusLabel");
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            // 
            // statusLinkLabel
            // 
            this.statusLinkLabel.ActiveLinkColor = System.Drawing.Color.Blue;
            resources.ApplyResources(this.statusLinkLabel, "statusLinkLabel");
            this.statusLinkLabel.IsLink = true;
            this.statusLinkLabel.LinkColor = System.Drawing.Color.Blue;
            this.statusLinkLabel.Name = "statusLinkLabel";
            this.statusLinkLabel.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.statusLinkLabel.VisitedLinkColor = System.Drawing.Color.Blue;
            // 
            // ConversionDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonClose;
            this.Controls.Add(this.tableLayoutMain);
            this.Controls.Add(this.tableLayoutPanelTop);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "ConversionDialog";
            this.tableLayoutMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConversions)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDetails)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPanelTop.ResumeLayout(false);
            this.toolStripTop.ResumeLayout(false);
            this.toolStripTop.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTop;
        private Controls.ToolStripEx toolStripTop;
        private System.Windows.Forms.ToolStripButton toolStripButtonNew;
        private System.Windows.Forms.ToolStripButton toolStripButtonRetry;
        private System.Windows.Forms.ToolStripButton toolStripButtonCancel;
        private System.Windows.Forms.ToolStripButton toolStripButtonExport;
        private System.Windows.Forms.ToolStripButton toolStripButtonClear;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private Controls.FilterStatusToolStripDropDownButton toolStripDdbFilterStatus;
        private System.Windows.Forms.ToolStripLabel toolStripLabelFiltersOnOff;
        private System.Windows.Forms.DataGridView dataGridViewDetails;
        private System.Windows.Forms.Label labelDetails;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private Controls.DataGridViewEx.DataGridViewEx dataGridViewConversions;
        private System.Windows.Forms.Timer timerVpx;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonRefresh;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRefreshAll;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRefreshSelected;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private ActionableLinkLabel statusLinkLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnValue;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonLogs;
        private System.Windows.Forms.ToolStripMenuItem menuItemFetchSelectedLog;
        private System.Windows.Forms.ToolStripMenuItem menuItemFetchAllLogs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnVm;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSourceServer;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnStartTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFinishTime;
        private System.Windows.Forms.DataGridViewImageColumn ColumnStatus;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem contextItemRefresh;
        private System.Windows.Forms.ToolStripMenuItem contextItemFetchLogs;
        private System.Windows.Forms.ToolStripMenuItem contextItemRetry;
        private System.Windows.Forms.ToolStripMenuItem contextItemCancel;
    }
}

