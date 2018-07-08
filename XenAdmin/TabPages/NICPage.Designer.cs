namespace XenAdmin.TabPages
{
    partial class NICPage
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
            // Deregister listeners.
            Host = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NICPage));
            this.label6 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.CreateBondButton = new System.Windows.Forms.Button();
            this.DeleteBondButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonRescan = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ColumnNIC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnMAC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLinkStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSpeed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDuplex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnVendorName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDeviceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBusPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnFCoECapable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSriovCapable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pageContainerPanel.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.panel1);
            this.pageContainerPanel.Controls.Add(this.label1);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // CreateBondButton
            // 
            resources.ApplyResources(this.CreateBondButton, "CreateBondButton");
            this.CreateBondButton.Name = "CreateBondButton";
            this.CreateBondButton.UseVisualStyleBackColor = true;
            this.CreateBondButton.Click += new System.EventHandler(this.CreateBondButton_Click);
            // 
            // DeleteBondButton
            // 
            resources.ApplyResources(this.DeleteBondButton, "DeleteBondButton");
            this.DeleteBondButton.Name = "DeleteBondButton";
            this.DeleteBondButton.UseVisualStyleBackColor = true;
            this.DeleteBondButton.Click += new System.EventHandler(this.DeleteBondButton_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.CreateBondButton);
            this.flowLayoutPanel1.Controls.Add(this.DeleteBondButton);
            this.flowLayoutPanel1.Controls.Add(this.buttonRescan);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // buttonRescan
            // 
            resources.ApplyResources(this.buttonRescan, "buttonRescan");
            this.buttonRescan.Name = "buttonRescan";
            this.buttonRescan.UseVisualStyleBackColor = true;
            this.buttonRescan.Click += new System.EventHandler(this.buttonRescan_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
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
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Name = "panel1";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnNIC,
            this.ColumnMAC,
            this.ColumnLinkStatus,
            this.ColumnSpeed,
            this.ColumnDuplex,
            this.ColumnVendorName,
            this.ColumnDeviceName,
            this.ColumnBusPath,
            this.ColumnFCoECapable,
            this.ColumnSriovCapable});
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.datagridview_SelectedIndexChanged);
            this.dataGridView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // ColumnNIC
            // 
            resources.ApplyResources(this.ColumnNIC, "ColumnNIC");
            this.ColumnNIC.Name = "ColumnNIC";
            this.ColumnNIC.ReadOnly = true;
            // 
            // ColumnMAC
            // 
            resources.ApplyResources(this.ColumnMAC, "ColumnMAC");
            this.ColumnMAC.Name = "ColumnMAC";
            this.ColumnMAC.ReadOnly = true;
            // 
            // ColumnLinkStatus
            // 
            resources.ApplyResources(this.ColumnLinkStatus, "ColumnLinkStatus");
            this.ColumnLinkStatus.Name = "ColumnLinkStatus";
            this.ColumnLinkStatus.ReadOnly = true;
            // 
            // ColumnSpeed
            // 
            resources.ApplyResources(this.ColumnSpeed, "ColumnSpeed");
            this.ColumnSpeed.Name = "ColumnSpeed";
            this.ColumnSpeed.ReadOnly = true;
            // 
            // ColumnDuplex
            // 
            resources.ApplyResources(this.ColumnDuplex, "ColumnDuplex");
            this.ColumnDuplex.Name = "ColumnDuplex";
            this.ColumnDuplex.ReadOnly = true;
            // 
            // ColumnVendorName
            // 
            resources.ApplyResources(this.ColumnVendorName, "ColumnVendorName");
            this.ColumnVendorName.Name = "ColumnVendorName";
            this.ColumnVendorName.ReadOnly = true;
            // 
            // ColumnDeviceName
            // 
            this.ColumnDeviceName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnDeviceName, "ColumnDeviceName");
            this.ColumnDeviceName.Name = "ColumnDeviceName";
            this.ColumnDeviceName.ReadOnly = true;
            this.ColumnDeviceName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColumnBusPath
            // 
            resources.ApplyResources(this.ColumnBusPath, "ColumnBusPath");
            this.ColumnBusPath.Name = "ColumnBusPath";
            this.ColumnBusPath.ReadOnly = true;
            // 
            // ColumnFCoECapable
            // 
            resources.ApplyResources(this.ColumnFCoECapable, "ColumnFCoECapable");
            this.ColumnFCoECapable.Name = "ColumnFCoECapable";
            this.ColumnFCoECapable.ReadOnly = true;
            // 
            // ColumnSriovCapable
            // 
            this.ColumnSriovCapable.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnSriovCapable, "ColumnSriovCapable");
            this.ColumnSriovCapable.Name = "ColumnSriovCapable";
            this.ColumnSriovCapable.ReadOnly = true;
            this.ColumnSriovCapable.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // NICPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Name = "NICPage";
            this.Controls.SetChildIndex(this.pageContainerPanel, 0);
            this.pageContainerPanel.ResumeLayout(false);
            this.pageContainerPanel.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Button CreateBondButton;
        private System.Windows.Forms.Button DeleteBondButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonRescan;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnNIC;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnMAC;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLinkStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSpeed;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDuplex;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnVendorName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDeviceName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnBusPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFCoECapable;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSriovCapable;
    }
}
