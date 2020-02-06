namespace XenAdmin.Controls.GPU
{
    partial class GpuRow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GpuRow));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.allowedTypesImageList = new System.Windows.Forms.ImageList(this.components);
            this.panelWithBorder = new XenAdmin.Controls.PanelWithBorder();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.allowedTypesLabel = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.pGpuLabel = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.pGpuPictureBox = new System.Windows.Forms.PictureBox();
            this.allowedTypesGrid = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.TextColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.shinyBarsContainerPanel = new System.Windows.Forms.TableLayoutPanel();
            this.editButton = new System.Windows.Forms.Button();
            this.multipleSelectionPanel = new System.Windows.Forms.Panel();
            this.clearAllButton = new System.Windows.Forms.Button();
            this.selectAllButton = new System.Windows.Forms.Button();
            this.panelWithBorder.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pGpuPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.allowedTypesGrid)).BeginInit();
            this.multipleSelectionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // allowedTypesImageList
            // 
            this.allowedTypesImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("allowedTypesImageList.ImageStream")));
            this.allowedTypesImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.allowedTypesImageList.Images.SetKeyName(0, "000_Abort_h32bit_16.png");
            this.allowedTypesImageList.Images.SetKeyName(1, "000_Tick_h32bit_16.png");
            // 
            // panelWithBorder
            // 
            resources.ApplyResources(this.panelWithBorder, "panelWithBorder");
            this.panelWithBorder.BackColor = System.Drawing.Color.Transparent;
            this.panelWithBorder.Controls.Add(this.tableLayoutPanel1);
            this.panelWithBorder.Controls.Add(this.multipleSelectionPanel);
            this.panelWithBorder.Name = "panelWithBorder";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.allowedTypesLabel, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.pGpuLabel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pGpuPictureBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.allowedTypesGrid, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.shinyBarsContainerPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.editButton, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // allowedTypesLabel
            // 
            resources.ApplyResources(this.allowedTypesLabel, "allowedTypesLabel");
            this.allowedTypesLabel.AutoEllipsis = true;
            this.allowedTypesLabel.BackColor = System.Drawing.Color.Transparent;
            this.allowedTypesLabel.Name = "allowedTypesLabel";
            // 
            // pGpuLabel
            // 
            resources.ApplyResources(this.pGpuLabel, "pGpuLabel");
            this.pGpuLabel.AutoEllipsis = true;
            this.pGpuLabel.BackColor = System.Drawing.Color.Transparent;
            this.pGpuLabel.Name = "pGpuLabel";
            // 
            // pGpuPictureBox
            // 
            this.pGpuPictureBox.Image = global::XenAdmin.Properties.Resources._000_GetMemoryInfo_h32bit_16;
            resources.ApplyResources(this.pGpuPictureBox, "pGpuPictureBox");
            this.pGpuPictureBox.Name = "pGpuPictureBox";
            this.pGpuPictureBox.TabStop = false;
            // 
            // allowedTypesGrid
            // 
            this.allowedTypesGrid.AllowUserToResizeColumns = false;
            this.allowedTypesGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.allowedTypesGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.allowedTypesGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.allowedTypesGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.allowedTypesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.allowedTypesGrid.ColumnHeadersVisible = false;
            this.allowedTypesGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ImageColumn,
            this.TextColumn});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.allowedTypesGrid.DefaultCellStyle = dataGridViewCellStyle1;
            this.allowedTypesGrid.GridColor = System.Drawing.SystemColors.Control;
            this.allowedTypesGrid.HideSelection = true;
            resources.ApplyResources(this.allowedTypesGrid, "allowedTypesGrid");
            this.allowedTypesGrid.Name = "allowedTypesGrid";
            this.allowedTypesGrid.ReadOnly = true;
            this.allowedTypesGrid.TabStop = false;
            // 
            // ImageColumn
            // 
            this.ImageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ImageColumn.Frozen = true;
            resources.ApplyResources(this.ImageColumn, "ImageColumn");
            this.ImageColumn.Name = "ImageColumn";
            this.ImageColumn.ReadOnly = true;
            // 
            // TextColumn
            // 
            this.TextColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TextColumn.FillWeight = 190F;
            this.TextColumn.Frozen = true;
            resources.ApplyResources(this.TextColumn, "TextColumn");
            this.TextColumn.Name = "TextColumn";
            this.TextColumn.ReadOnly = true;
            this.TextColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // shinyBarsContainerPanel
            // 
            resources.ApplyResources(this.shinyBarsContainerPanel, "shinyBarsContainerPanel");
            this.tableLayoutPanel1.SetColumnSpan(this.shinyBarsContainerPanel, 2);
            this.shinyBarsContainerPanel.Name = "shinyBarsContainerPanel";
            this.tableLayoutPanel1.SetRowSpan(this.shinyBarsContainerPanel, 2);
            // 
            // editButton
            // 
            resources.ApplyResources(this.editButton, "editButton");
            this.editButton.BackColor = System.Drawing.Color.Transparent;
            this.editButton.Name = "editButton";
            this.editButton.UseVisualStyleBackColor = false;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // multipleSelectionPanel
            // 
            this.multipleSelectionPanel.Controls.Add(this.clearAllButton);
            this.multipleSelectionPanel.Controls.Add(this.selectAllButton);
            resources.ApplyResources(this.multipleSelectionPanel, "multipleSelectionPanel");
            this.multipleSelectionPanel.Name = "multipleSelectionPanel";
            // 
            // clearAllButton
            // 
            resources.ApplyResources(this.clearAllButton, "clearAllButton");
            this.clearAllButton.Name = "clearAllButton";
            this.clearAllButton.UseVisualStyleBackColor = true;
            this.clearAllButton.Click += new System.EventHandler(this.clearAllButton_Click);
            // 
            // selectAllButton
            // 
            resources.ApplyResources(this.selectAllButton, "selectAllButton");
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.UseVisualStyleBackColor = true;
            this.selectAllButton.Click += new System.EventHandler(this.selectAllButton_Click);
            // 
            // GpuRow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panelWithBorder);
            this.Name = "GpuRow";
            this.panelWithBorder.ResumeLayout(false);
            this.panelWithBorder.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pGpuPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.allowedTypesGrid)).EndInit();
            this.multipleSelectionPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pGpuPictureBox;
        private XenAdmin.Controls.Common.AutoHeightLabel pGpuLabel;
        private System.Windows.Forms.ImageList allowedTypesImageList;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx allowedTypesGrid;
        private XenAdmin.Controls.Common.AutoHeightLabel allowedTypesLabel;
        private System.Windows.Forms.DataGridViewImageColumn ImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn TextColumn;
        private System.Windows.Forms.TableLayoutPanel shinyBarsContainerPanel;
        private System.Windows.Forms.Button clearAllButton;
        private System.Windows.Forms.Button selectAllButton;
        private PanelWithBorder panelWithBorder;
        protected System.Windows.Forms.Panel multipleSelectionPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button editButton;
    }
}
