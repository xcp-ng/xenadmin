using XenAdmin.Commands;
using XenAPI;

namespace XenAdmin.TabPages
{
    partial class SrStoragePage
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SrStoragePage));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rescanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveVirtualDiskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteVirtualDiskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editVirtualDiskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.RemoveButtonContainer = new XenAdmin.Controls.ToolTipContainer();
            this.RemoveButton = new System.Windows.Forms.Button();
            this.EditButtonContainer = new XenAdmin.Controls.ToolTipContainer();
            this.EditButton = new System.Windows.Forms.Button();
            this.addVirtualDiskButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.toolTipContainerRescan = new XenAdmin.Controls.ToolTipContainer();
            this.buttonRescan = new System.Windows.Forms.Button();
            this.toolTipContainerMove = new XenAdmin.Controls.ToolTipContainer();
            this.buttonMove = new System.Windows.Forms.Button();
            this.dataGridViewVDIs = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnVolume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnVM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnCBT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pageContainerPanel.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.RemoveButtonContainer.SuspendLayout();
            this.EditButtonContainer.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.toolTipContainerRescan.SuspendLayout();
            this.toolTipContainerMove.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVDIs)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rescanToolStripMenuItem,
            this.addToolStripMenuItem,
            this.moveVirtualDiskToolStripMenuItem,
            this.deleteVirtualDiskToolStripMenuItem,
            this.toolStripSeparator1,
            this.editVirtualDiskToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // rescanToolStripMenuItem
            // 
            this.rescanToolStripMenuItem.Name = "rescanToolStripMenuItem";
            resources.ApplyResources(this.rescanToolStripMenuItem, "rescanToolStripMenuItem");
            this.rescanToolStripMenuItem.Click += new System.EventHandler(this.rescanToolStripMenuItem_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            resources.ApplyResources(this.addToolStripMenuItem, "addToolStripMenuItem");
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // moveVirtualDiskToolStripMenuItem
            // 
            this.moveVirtualDiskToolStripMenuItem.Name = "moveVirtualDiskToolStripMenuItem";
            resources.ApplyResources(this.moveVirtualDiskToolStripMenuItem, "moveVirtualDiskToolStripMenuItem");
            this.moveVirtualDiskToolStripMenuItem.Click += new System.EventHandler(this.moveVirtualDiskToolStripMenuItem_Click);
            // 
            // deleteVirtualDiskToolStripMenuItem
            // 
            this.deleteVirtualDiskToolStripMenuItem.Name = "deleteVirtualDiskToolStripMenuItem";
            resources.ApplyResources(this.deleteVirtualDiskToolStripMenuItem, "deleteVirtualDiskToolStripMenuItem");
            this.deleteVirtualDiskToolStripMenuItem.Click += new System.EventHandler(this.deleteVirtualDiskToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // editVirtualDiskToolStripMenuItem
            // 
            this.editVirtualDiskToolStripMenuItem.Image = global::XenAdmin.Properties.Resources.edit_16;
            resources.ApplyResources(this.editVirtualDiskToolStripMenuItem, "editVirtualDiskToolStripMenuItem");
            this.editVirtualDiskToolStripMenuItem.Name = "editVirtualDiskToolStripMenuItem";
            this.editVirtualDiskToolStripMenuItem.Click += new System.EventHandler(this.editVirtualDiskToolStripMenuItem_Click);
            // 
            // TitleLabel
            // 
            resources.ApplyResources(this.TitleLabel, "TitleLabel");
            this.TitleLabel.ForeColor = System.Drawing.Color.White;
            this.TitleLabel.Name = "TitleLabel";
            // 
            // RemoveButtonContainer
            // 
            this.RemoveButtonContainer.Controls.Add(this.RemoveButton);
            resources.ApplyResources(this.RemoveButtonContainer, "RemoveButtonContainer");
            this.RemoveButtonContainer.Name = "RemoveButtonContainer";
            // 
            // RemoveButton
            // 
            resources.ApplyResources(this.RemoveButton, "RemoveButton");
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // EditButtonContainer
            // 
            resources.ApplyResources(this.EditButtonContainer, "EditButtonContainer");
            this.EditButtonContainer.Controls.Add(this.EditButton);
            this.EditButtonContainer.Name = "EditButtonContainer";
            // 
            // EditButton
            // 
            resources.ApplyResources(this.EditButton, "EditButton");
            this.EditButton.Name = "EditButton";
            this.EditButton.UseVisualStyleBackColor = true;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // addVirtualDiskButton
            // 
            resources.ApplyResources(this.addVirtualDiskButton, "addVirtualDiskButton");
            this.addVirtualDiskButton.Name = "addVirtualDiskButton";
            this.addVirtualDiskButton.UseVisualStyleBackColor = true;
            this.addVirtualDiskButton.Click += new System.EventHandler(this.addVirtualDiskButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.toolTipContainerRescan);
            this.flowLayoutPanel1.Controls.Add(this.addVirtualDiskButton);
            this.flowLayoutPanel1.Controls.Add(this.EditButtonContainer);
            this.flowLayoutPanel1.Controls.Add(this.toolTipContainerMove);
            this.flowLayoutPanel1.Controls.Add(this.RemoveButtonContainer);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // toolTipContainerRescan
            // 
            this.toolTipContainerRescan.Controls.Add(this.buttonRescan);
            resources.ApplyResources(this.toolTipContainerRescan, "toolTipContainerRescan");
            this.toolTipContainerRescan.Name = "toolTipContainerRescan";
            // 
            // buttonRescan
            // 
            resources.ApplyResources(this.buttonRescan, "buttonRescan");
            this.buttonRescan.Name = "buttonRescan";
            this.buttonRescan.UseVisualStyleBackColor = true;
            this.buttonRescan.Click += new System.EventHandler(this.buttonRescan_Click);
            // 
            // toolTipContainerMove
            // 
            this.toolTipContainerMove.Controls.Add(this.buttonMove);
            resources.ApplyResources(this.toolTipContainerMove, "toolTipContainerMove");
            this.toolTipContainerMove.Name = "toolTipContainerMove";
            // 
            // buttonMove
            // 
            resources.ApplyResources(this.buttonMove, "buttonMove");
            this.buttonMove.Name = "buttonMove";
            this.buttonMove.UseVisualStyleBackColor = true;
            this.buttonMove.Click += new System.EventHandler(this.buttonMove_Click);
            // 
            // dataGridViewVDIs
            // 
            this.dataGridViewVDIs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewVDIs.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewVDIs.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewVDIs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewVDIs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnName,
            this.ColumnVolume,
            this.ColumnDesc,
            this.ColumnSize,
            this.ColumnVM,
            this.ColumnCBT});
            resources.ApplyResources(this.dataGridViewVDIs, "dataGridViewVDIs");
            this.dataGridViewVDIs.MultiSelect = true;
            this.dataGridViewVDIs.Name = "dataGridViewVDIs";
            this.dataGridViewVDIs.ReadOnly = true;
            this.dataGridViewVDIs.SelectionChanged += new System.EventHandler(this.dataGridViewVDIs_SelectedIndexChanged);
            this.dataGridViewVDIs.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.DataGridViewObject_SortCompare);
            this.dataGridViewVDIs.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridViewVDIs_KeyUp);
            this.dataGridViewVDIs.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGridViewVDIs_MouseUp);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewVDIs, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // ColumnName
            // 
            this.ColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnName, "ColumnName");
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            // 
            // ColumnVolume
            // 
            this.ColumnVolume.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnVolume, "ColumnVolume");
            this.ColumnVolume.Name = "ColumnVolume";
            this.ColumnVolume.ReadOnly = true;
            // 
            // ColumnDesc
            // 
            this.ColumnDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnDesc, "ColumnDesc");
            this.ColumnDesc.Name = "ColumnDesc";
            this.ColumnDesc.ReadOnly = true;
            // 
            // ColumnSize
            // 
            this.ColumnSize.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnSize, "ColumnSize");
            this.ColumnSize.Name = "ColumnSize";
            this.ColumnSize.ReadOnly = true;
            // 
            // ColumnVM
            // 
            this.ColumnVM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnVM, "ColumnVM");
            this.ColumnVM.Name = "ColumnVM";
            this.ColumnVM.ReadOnly = true;
            // 
            // ColumnCBT
            // 
            this.ColumnCBT.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnCBT, "ColumnCBT");
            this.ColumnCBT.Name = "ColumnCBT";
            this.ColumnCBT.ReadOnly = true;
            // 
            // SrStoragePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Name = "SrStoragePage";
            this.Controls.SetChildIndex(this.pageContainerPanel, 0);
            this.pageContainerPanel.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.RemoveButtonContainer.ResumeLayout(false);
            this.EditButtonContainer.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.toolTipContainerRescan.ResumeLayout(false);
            this.toolTipContainerMove.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVDIs)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editVirtualDiskToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteVirtualDiskToolStripMenuItem;
        private System.Windows.Forms.Button addVirtualDiskButton;
        private System.Windows.Forms.Label TitleLabel;
        private XenAdmin.Controls.ToolTipContainer EditButtonContainer;
        private System.Windows.Forms.Button EditButton;
        private XenAdmin.Controls.ToolTipContainer RemoveButtonContainer;
        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button buttonRescan;
        private XenAdmin.Controls.ToolTipContainer toolTipContainerRescan;
        private XenAdmin.Controls.ToolTipContainer toolTipContainerMove;
        private System.Windows.Forms.Button buttonMove;
        private System.Windows.Forms.ToolStripMenuItem moveVirtualDiskToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewVDIs;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStripMenuItem rescanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnVolume;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnVM;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCBT;

    }
}
