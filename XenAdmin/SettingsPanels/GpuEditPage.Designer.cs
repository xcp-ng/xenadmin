namespace XenAdmin.SettingsPanels
{
    partial class GpuEditPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GpuEditPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.warningsTable = new System.Windows.Forms.TableLayoutPanel();
            this.labelMulti = new System.Windows.Forms.Label();
            this.imgMulti = new System.Windows.Forms.PictureBox();
            this.imgRDP = new System.Windows.Forms.PictureBox();
            this.labelRDP = new System.Windows.Forms.Label();
            this.imgNeedDriver = new System.Windows.Forms.PictureBox();
            this.labelNeedDriver = new System.Windows.Forms.Label();
            this.imgNeedGpu = new System.Windows.Forms.PictureBox();
            this.labelNeedGpu = new System.Windows.Forms.Label();
            this.imgStopVM = new System.Windows.Forms.PictureBox();
            this.labelStopVM = new System.Windows.Forms.Label();
            this.imgHA = new System.Windows.Forms.PictureBox();
            this.labelHA = new System.Windows.Forms.Label();
            this.imgExperimental = new System.Windows.Forms.PictureBox();
            this.labelExperimental = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.addButton = new System.Windows.Forms.Button();
            this.labelRubric = new System.Windows.Forms.Label();
            this.gpuGrid = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.deleteButton = new System.Windows.Forms.Button();
            this.DeviceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VGPUsPerGPUColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxResolutionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxDisplaysColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VideoRAMColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.warningsTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgMulti)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgRDP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgNeedDriver)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgNeedGpu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgStopVM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgExperimental)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gpuGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // warningsTable
            // 
            resources.ApplyResources(this.warningsTable, "warningsTable");
            this.tableLayoutPanel1.SetColumnSpan(this.warningsTable, 2);
            this.warningsTable.Controls.Add(this.labelMulti, 1, 6);
            this.warningsTable.Controls.Add(this.imgMulti, 0, 6);
            this.warningsTable.Controls.Add(this.imgRDP, 0, 1);
            this.warningsTable.Controls.Add(this.labelRDP, 1, 1);
            this.warningsTable.Controls.Add(this.imgNeedDriver, 0, 2);
            this.warningsTable.Controls.Add(this.labelNeedDriver, 1, 2);
            this.warningsTable.Controls.Add(this.imgNeedGpu, 0, 3);
            this.warningsTable.Controls.Add(this.labelNeedGpu, 1, 3);
            this.warningsTable.Controls.Add(this.imgStopVM, 0, 4);
            this.warningsTable.Controls.Add(this.labelStopVM, 1, 4);
            this.warningsTable.Controls.Add(this.imgHA, 0, 5);
            this.warningsTable.Controls.Add(this.labelHA, 1, 5);
            this.warningsTable.Controls.Add(this.imgExperimental, 0, 0);
            this.warningsTable.Controls.Add(this.labelExperimental, 1, 0);
            this.warningsTable.Name = "warningsTable";
            this.warningsTable.SizeChanged += new System.EventHandler(this.warningsTable_SizeChanged);
            // 
            // labelMulti
            // 
            resources.ApplyResources(this.labelMulti, "labelMulti");
            this.labelMulti.Name = "labelMulti";
            // 
            // imgMulti
            // 
            resources.ApplyResources(this.imgMulti, "imgMulti");
            this.imgMulti.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.imgMulti.Name = "imgMulti";
            this.imgMulti.TabStop = false;
            // 
            // imgRDP
            // 
            resources.ApplyResources(this.imgRDP, "imgRDP");
            this.imgRDP.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            this.imgRDP.Name = "imgRDP";
            this.imgRDP.TabStop = false;
            // 
            // labelRDP
            // 
            resources.ApplyResources(this.labelRDP, "labelRDP");
            this.labelRDP.Name = "labelRDP";
            // 
            // imgNeedDriver
            // 
            this.imgNeedDriver.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.imgNeedDriver, "imgNeedDriver");
            this.imgNeedDriver.Name = "imgNeedDriver";
            this.imgNeedDriver.TabStop = false;
            // 
            // labelNeedDriver
            // 
            resources.ApplyResources(this.labelNeedDriver, "labelNeedDriver");
            this.labelNeedDriver.Name = "labelNeedDriver";
            // 
            // imgNeedGpu
            // 
            resources.ApplyResources(this.imgNeedGpu, "imgNeedGpu");
            this.imgNeedGpu.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.imgNeedGpu.Name = "imgNeedGpu";
            this.imgNeedGpu.TabStop = false;
            // 
            // labelNeedGpu
            // 
            resources.ApplyResources(this.labelNeedGpu, "labelNeedGpu");
            this.labelNeedGpu.Name = "labelNeedGpu";
            // 
            // imgStopVM
            // 
            resources.ApplyResources(this.imgStopVM, "imgStopVM");
            this.imgStopVM.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.imgStopVM.Name = "imgStopVM";
            this.imgStopVM.TabStop = false;
            // 
            // labelStopVM
            // 
            resources.ApplyResources(this.labelStopVM, "labelStopVM");
            this.labelStopVM.Name = "labelStopVM";
            // 
            // imgHA
            // 
            resources.ApplyResources(this.imgHA, "imgHA");
            this.imgHA.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.imgHA.Name = "imgHA";
            this.imgHA.TabStop = false;
            // 
            // labelHA
            // 
            resources.ApplyResources(this.labelHA, "labelHA");
            this.labelHA.Name = "labelHA";
            // 
            // imgExperimental
            // 
            this.imgExperimental.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.imgExperimental, "imgExperimental");
            this.imgExperimental.Name = "imgExperimental";
            this.imgExperimental.TabStop = false;
            // 
            // labelExperimental
            // 
            resources.ApplyResources(this.labelExperimental, "labelExperimental");
            this.labelExperimental.Name = "labelExperimental";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.addButton, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelRubric, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gpuGrid, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.warningsTable, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.deleteButton, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // addButton
            // 
            resources.ApplyResources(this.addButton, "addButton");
            this.addButton.Name = "addButton";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // labelRubric
            // 
            resources.ApplyResources(this.labelRubric, "labelRubric");
            this.tableLayoutPanel1.SetColumnSpan(this.labelRubric, 2);
            this.labelRubric.Name = "labelRubric";
            // 
            // gpuGrid
            // 
            this.gpuGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gpuGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gpuGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.gpuGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gpuGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DeviceColumn,
            this.NameColumn,
            this.VGPUsPerGPUColumn,
            this.MaxResolutionColumn,
            this.MaxDisplaysColumn,
            this.VideoRAMColumn});
            resources.ApplyResources(this.gpuGrid, "gpuGrid");
            this.gpuGrid.Name = "gpuGrid";
            this.gpuGrid.ReadOnly = true;
            this.gpuGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.tableLayoutPanel1.SetRowSpan(this.gpuGrid, 2);
            // 
            // deleteButton
            // 
            resources.ApplyResources(this.deleteButton, "deleteButton");
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // DeviceColumn
            // 
            this.DeviceColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.DeviceColumn, "DeviceColumn");
            this.DeviceColumn.Name = "DeviceColumn";
            this.DeviceColumn.ReadOnly = true;
            this.DeviceColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // NameColumn
            // 
            this.NameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.NameColumn, "NameColumn");
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.ReadOnly = true;
            this.NameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // VGPUsPerGPUColumn
            // 
            this.VGPUsPerGPUColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.VGPUsPerGPUColumn.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.VGPUsPerGPUColumn, "VGPUsPerGPUColumn");
            this.VGPUsPerGPUColumn.Name = "VGPUsPerGPUColumn";
            this.VGPUsPerGPUColumn.ReadOnly = true;
            this.VGPUsPerGPUColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.VGPUsPerGPUColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // MaxResolutionColumn
            // 
            this.MaxResolutionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.MaxResolutionColumn.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.MaxResolutionColumn, "MaxResolutionColumn");
            this.MaxResolutionColumn.Name = "MaxResolutionColumn";
            this.MaxResolutionColumn.ReadOnly = true;
            this.MaxResolutionColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.MaxResolutionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // MaxDisplaysColumn
            // 
            this.MaxDisplaysColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.MaxDisplaysColumn.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.MaxDisplaysColumn, "MaxDisplaysColumn");
            this.MaxDisplaysColumn.Name = "MaxDisplaysColumn";
            this.MaxDisplaysColumn.ReadOnly = true;
            this.MaxDisplaysColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.MaxDisplaysColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // VideoRAMColumn
            // 
            this.VideoRAMColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.VideoRAMColumn.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.VideoRAMColumn, "VideoRAMColumn");
            this.VideoRAMColumn.Name = "VideoRAMColumn";
            this.VideoRAMColumn.ReadOnly = true;
            this.VideoRAMColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.VideoRAMColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // GpuEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "GpuEditPage";
            this.warningsTable.ResumeLayout(false);
            this.warningsTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgMulti)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgRDP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgNeedDriver)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgNeedGpu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgStopVM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgExperimental)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gpuGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel warningsTable;
        private System.Windows.Forms.PictureBox imgRDP;
        private System.Windows.Forms.Label labelRDP;
        private System.Windows.Forms.PictureBox imgStopVM;
        private System.Windows.Forms.Label labelStopVM;
        private System.Windows.Forms.PictureBox imgNeedGpu;
        private System.Windows.Forms.Label labelNeedGpu;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox imgNeedDriver;
        private System.Windows.Forms.Label labelNeedDriver;
        private System.Windows.Forms.PictureBox imgHA;
        private System.Windows.Forms.Label labelHA;
        private System.Windows.Forms.PictureBox imgExperimental;
        private System.Windows.Forms.Label labelExperimental;
        private System.Windows.Forms.Label labelRubric;
        private Controls.DataGridViewEx.DataGridViewEx gpuGrid;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Label labelMulti;
        private System.Windows.Forms.PictureBox imgMulti;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeviceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn VGPUsPerGPUColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxResolutionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxDisplaysColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn VideoRAMColumn;
    }
}
