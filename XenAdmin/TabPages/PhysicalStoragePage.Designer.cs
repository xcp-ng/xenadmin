namespace XenAdmin.TabPages
{
    partial class PhysicalStoragePage
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
            Host = null;
            Connection = null;

            if (disposing)
            {
                if (selectionManager != null)
                    selectionManager.Dispose();
                if (components != null)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhysicalStoragePage));
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.labelNetworkheadings = new System.Windows.Forms.Label();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridViewSr = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.columnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnShared = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnUsage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnVirtAlloc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.newSRButton = new XenAdmin.Commands.CommandButton();
            this.trimButtonContainer = new XenAdmin.Controls.ToolTipContainer();
            this.trimButton = new XenAdmin.Commands.CommandButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonProperties = new System.Windows.Forms.Button();
            this.pageContainerPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSr)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.trimButtonContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.panel1);
            this.pageContainerPanel.Controls.Add(this.labelNetworkheadings);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // labelNetworkheadings
            // 
            resources.ApplyResources(this.labelNetworkheadings, "labelNetworkheadings");
            this.labelNetworkheadings.Name = "labelNetworkheadings";
            // 
            // TitleLabel
            // 
            resources.ApplyResources(this.TitleLabel, "TitleLabel");
            this.TitleLabel.ForeColor = System.Drawing.Color.White;
            this.TitleLabel.Name = "TitleLabel";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.dataGridViewSr);
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Name = "panel1";
            // 
            // dataGridViewSr
            // 
            this.dataGridViewSr.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewSr.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewSr.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewSr.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnImage,
            this.columnName,
            this.columnDescription,
            this.columnType,
            this.columnShared,
            this.columnUsage,
            this.columnSize,
            this.columnVirtAlloc});
            resources.ApplyResources(this.dataGridViewSr, "dataGridViewSr");
            this.dataGridViewSr.Name = "dataGridViewSr";
            this.dataGridViewSr.SelectionChanged += new System.EventHandler(this.dataGridViewSrs_SelectedIndexChanged);
            this.dataGridViewSr.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGridViewSr_SortCompare);
            this.dataGridViewSr.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGridViewSr_MouseUp);
            // 
            // columnImage
            // 
            this.columnImage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnImage, "columnImage");
            this.columnImage.Name = "columnImage";
            this.columnImage.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // columnName
            // 
            this.columnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.columnName, "columnName");
            this.columnName.Name = "columnName";
            // 
            // columnDescription
            // 
            this.columnDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.columnDescription, "columnDescription");
            this.columnDescription.Name = "columnDescription";
            // 
            // columnType
            // 
            this.columnType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnType, "columnType");
            this.columnType.Name = "columnType";
            // 
            // columnShared
            // 
            this.columnShared.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnShared, "columnShared");
            this.columnShared.Name = "columnShared";
            // 
            // columnUsage
            // 
            this.columnUsage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnUsage, "columnUsage");
            this.columnUsage.Name = "columnUsage";
            // 
            // columnSize
            // 
            this.columnSize.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnSize, "columnSize");
            this.columnSize.Name = "columnSize";
            // 
            // columnVirtAlloc
            // 
            this.columnVirtAlloc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnVirtAlloc, "columnVirtAlloc");
            this.columnVirtAlloc.Name = "columnVirtAlloc";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.newSRButton);
            this.flowLayoutPanel1.Controls.Add(this.trimButtonContainer);
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Controls.Add(this.buttonProperties);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // newSRButton
            // 
            this.newSRButton.Command = new XenAdmin.Commands.NewSRCommand();
            resources.ApplyResources(this.newSRButton, "newSRButton");
            this.newSRButton.Name = "newSRButton";
            this.newSRButton.UseVisualStyleBackColor = true;
            // 
            // trimButtonContainer
            // 
            this.trimButtonContainer.Controls.Add(this.trimButton);
            resources.ApplyResources(this.trimButtonContainer, "trimButtonContainer");
            this.trimButtonContainer.Name = "trimButtonContainer";
            // 
            // trimButton
            // 
            this.trimButton.Command = new XenAdmin.Commands.TrimSRCommand();
            resources.ApplyResources(this.trimButton, "trimButton");
            this.trimButton.Name = "trimButton";
            this.trimButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // buttonProperties
            // 
            resources.ApplyResources(this.buttonProperties, "buttonProperties");
            this.buttonProperties.Name = "buttonProperties";
            this.buttonProperties.UseVisualStyleBackColor = true;
            this.buttonProperties.Click += new System.EventHandler(this.buttonProperties_Click);
            // 
            // PhysicalStoragePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Name = "PhysicalStoragePage";
            this.Controls.SetChildIndex(this.pageContainerPanel, 0);
            this.pageContainerPanel.ResumeLayout(false);
            this.pageContainerPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSr)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.trimButtonContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.Label labelNetworkheadings;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonProperties;
        private System.Windows.Forms.GroupBox groupBox1;
        private XenAdmin.Commands.CommandButton newSRButton;
        private XenAdmin.Commands.CommandButton trimButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Controls.ToolTipContainer trimButtonContainer;
        private Controls.DataGridViewEx.DataGridViewEx dataGridViewSr;
        private System.Windows.Forms.DataGridViewImageColumn columnImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnType;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnShared;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnUsage;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnVirtAlloc;
    }
}
