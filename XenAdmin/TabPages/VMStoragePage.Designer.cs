namespace XenAdmin.TabPages
{
    partial class VMStoragePage
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
            UnregisterHandlers();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VMStoragePage));
            this.AddButton = new System.Windows.Forms.Button();
            this.EditButton = new System.Windows.Forms.Button();
            this.AttachButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.DeactivateButtonContainer = new XenAdmin.Controls.ToolTipContainer();
            this.DeactivateButton = new System.Windows.Forms.Button();
            this.MoveButtonContainer = new XenAdmin.Controls.ToolTipContainer();
            this.MoveButton = new System.Windows.Forms.Button();
            this.DetachButtonContainer = new XenAdmin.Controls.ToolTipContainer();
            this.DetachButton = new System.Windows.Forms.Button();
            this.DeleteButtonContainer = new XenAdmin.Controls.ToolTipContainer();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAttach = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDeactivate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDetach = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewStorage = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnDevicePosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSRVolume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnReadOnly = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPriority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnActive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDevicePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.multipleDvdIsoList1 = new XenAdmin.Controls.MultipleDvdIsoList();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pageContainerPanel.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.DeactivateButtonContainer.SuspendLayout();
            this.MoveButtonContainer.SuspendLayout();
            this.DetachButtonContainer.SuspendLayout();
            this.DeleteButtonContainer.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStorage)).BeginInit();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // AddButton
            // 
            resources.ApplyResources(this.AddButton, "AddButton");
            this.AddButton.Name = "AddButton";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // EditButton
            // 
            resources.ApplyResources(this.EditButton, "EditButton");
            this.EditButton.Name = "EditButton";
            this.EditButton.UseVisualStyleBackColor = true;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // AttachButton
            // 
            resources.ApplyResources(this.AttachButton, "AttachButton");
            this.AttachButton.Name = "AttachButton";
            this.AttachButton.UseVisualStyleBackColor = true;
            this.AttachButton.Click += new System.EventHandler(this.AttachButton_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.AddButton);
            this.flowLayoutPanel1.Controls.Add(this.AttachButton);
            this.flowLayoutPanel1.Controls.Add(this.DeactivateButtonContainer);
            this.flowLayoutPanel1.Controls.Add(this.MoveButtonContainer);
            this.flowLayoutPanel1.Controls.Add(this.DetachButtonContainer);
            this.flowLayoutPanel1.Controls.Add(this.DeleteButtonContainer);
            this.flowLayoutPanel1.Controls.Add(this.EditButton);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // DeactivateButtonContainer
            // 
            this.DeactivateButtonContainer.Controls.Add(this.DeactivateButton);
            resources.ApplyResources(this.DeactivateButtonContainer, "DeactivateButtonContainer");
            this.DeactivateButtonContainer.Name = "DeactivateButtonContainer";
            // 
            // DeactivateButton
            // 
            resources.ApplyResources(this.DeactivateButton, "DeactivateButton");
            this.DeactivateButton.Name = "DeactivateButton";
            this.DeactivateButton.UseVisualStyleBackColor = true;
            this.DeactivateButton.Click += new System.EventHandler(this.DeactivateButton_Click);
            // 
            // MoveButtonContainer
            // 
            this.MoveButtonContainer.Controls.Add(this.MoveButton);
            resources.ApplyResources(this.MoveButtonContainer, "MoveButtonContainer");
            this.MoveButtonContainer.Name = "MoveButtonContainer";
            // 
            // MoveButton
            // 
            resources.ApplyResources(this.MoveButton, "MoveButton");
            this.MoveButton.Name = "MoveButton";
            this.MoveButton.UseVisualStyleBackColor = true;
            this.MoveButton.Click += new System.EventHandler(this.MoveButton_Click);
            // 
            // DetachButtonContainer
            // 
            this.DetachButtonContainer.Controls.Add(this.DetachButton);
            resources.ApplyResources(this.DetachButtonContainer, "DetachButtonContainer");
            this.DetachButtonContainer.Name = "DetachButtonContainer";
            // 
            // DetachButton
            // 
            resources.ApplyResources(this.DetachButton, "DetachButton");
            this.DetachButton.Name = "DetachButton";
            this.DetachButton.UseVisualStyleBackColor = true;
            this.DetachButton.Click += new System.EventHandler(this.DetachButton_Click);
            // 
            // DeleteButtonContainer
            // 
            this.DeleteButtonContainer.Controls.Add(this.DeleteButton);
            resources.ApplyResources(this.DeleteButtonContainer, "DeleteButtonContainer");
            this.DeleteButtonContainer.Name = "DeleteButtonContainer";
            // 
            // DeleteButton
            // 
            resources.ApplyResources(this.DeleteButton, "DeleteButton");
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteDriveButton_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAdd,
            this.toolStripMenuItemAttach,
            this.toolStripMenuItemDeactivate,
            this.toolStripMenuItemMove,
            this.toolStripMenuItemDelete,
            this.toolStripMenuItemDetach,
            this.toolStripSeparator1,
            this.toolStripMenuItemProperties});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // toolStripMenuItemAdd
            // 
            this.toolStripMenuItemAdd.Name = "toolStripMenuItemAdd";
            resources.ApplyResources(this.toolStripMenuItemAdd, "toolStripMenuItemAdd");
            this.toolStripMenuItemAdd.Click += new System.EventHandler(this.toolStripMenuItemAdd_Click);
            // 
            // toolStripMenuItemAttach
            // 
            this.toolStripMenuItemAttach.Name = "toolStripMenuItemAttach";
            resources.ApplyResources(this.toolStripMenuItemAttach, "toolStripMenuItemAttach");
            this.toolStripMenuItemAttach.Click += new System.EventHandler(this.toolStripMenuItemAttach_Click);
            // 
            // toolStripMenuItemDeactivate
            // 
            this.toolStripMenuItemDeactivate.Name = "toolStripMenuItemDeactivate";
            resources.ApplyResources(this.toolStripMenuItemDeactivate, "toolStripMenuItemDeactivate");
            this.toolStripMenuItemDeactivate.Click += new System.EventHandler(this.toolStripMenuItemDeactivate_Click);
            // 
            // toolStripMenuItemMove
            // 
            this.toolStripMenuItemMove.Name = "toolStripMenuItemMove";
            resources.ApplyResources(this.toolStripMenuItemMove, "toolStripMenuItemMove");
            this.toolStripMenuItemMove.Click += new System.EventHandler(this.toolStripMenuItemMove_Click);
            // 
            // toolStripMenuItemDelete
            // 
            this.toolStripMenuItemDelete.Name = "toolStripMenuItemDelete";
            resources.ApplyResources(this.toolStripMenuItemDelete, "toolStripMenuItemDelete");
            this.toolStripMenuItemDelete.Click += new System.EventHandler(this.toolStripMenuItemDelete_Click);
            // 
            // toolStripMenuItemDetach
            // 
            this.toolStripMenuItemDetach.Name = "toolStripMenuItemDetach";
            resources.ApplyResources(this.toolStripMenuItemDetach, "toolStripMenuItemDetach");
            this.toolStripMenuItemDetach.Click += new System.EventHandler(this.toolStripMenuItemDetach_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripMenuItemProperties
            // 
            this.toolStripMenuItemProperties.Image = global::XenAdmin.Properties.Resources.edit_16;
            resources.ApplyResources(this.toolStripMenuItemProperties, "toolStripMenuItemProperties");
            this.toolStripMenuItemProperties.Name = "toolStripMenuItemProperties";
            this.toolStripMenuItemProperties.Click += new System.EventHandler(this.toolStripMenuItemProperties_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewStorage, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.multipleDvdIsoList1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // dataGridViewStorage
            // 
            this.dataGridViewStorage.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewStorage.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewStorage.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewStorage.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnDevicePosition,
            this.ColumnName,
            this.ColumnDesc,
            this.ColumnSR,
            this.ColumnSRVolume,
            this.ColumnSize,
            this.ColumnReadOnly,
            this.ColumnPriority,
            this.ColumnActive,
            this.ColumnDevicePath});
            resources.ApplyResources(this.dataGridViewStorage, "dataGridViewStorage");
            this.dataGridViewStorage.MultiSelect = true;
            this.dataGridViewStorage.Name = "dataGridViewStorage";
            this.dataGridViewStorage.ReadOnly = true;
            this.dataGridViewStorage.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewStorage_CellMouseDoubleClick);
            this.dataGridViewStorage.SelectionChanged += new System.EventHandler(this.dataGridViewStorage_SelectedIndexChanged);
            this.dataGridViewStorage.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGridViewStorage_SortCompare);
            this.dataGridViewStorage.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridViewStorage_KeyUp);
            this.dataGridViewStorage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGridViewStorage_MouseUp);
            // 
            // ColumnDevicePosition
            // 
            this.ColumnDevicePosition.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnDevicePosition, "ColumnDevicePosition");
            this.ColumnDevicePosition.Name = "ColumnDevicePosition";
            this.ColumnDevicePosition.ReadOnly = true;
            // 
            // ColumnName
            // 
            this.ColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnName, "ColumnName");
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            // 
            // ColumnDesc
            // 
            this.ColumnDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnDesc, "ColumnDesc");
            this.ColumnDesc.Name = "ColumnDesc";
            this.ColumnDesc.ReadOnly = true;
            // 
            // ColumnSR
            // 
            this.ColumnSR.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnSR, "ColumnSR");
            this.ColumnSR.Name = "ColumnSR";
            this.ColumnSR.ReadOnly = true;
            // 
            // ColumnSRVolume
            // 
            this.ColumnSRVolume.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnSRVolume, "ColumnSRVolume");
            this.ColumnSRVolume.Name = "ColumnSRVolume";
            this.ColumnSRVolume.ReadOnly = true;
            // 
            // ColumnSize
            // 
            this.ColumnSize.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnSize, "ColumnSize");
            this.ColumnSize.Name = "ColumnSize";
            this.ColumnSize.ReadOnly = true;
            // 
            // ColumnReadOnly
            // 
            this.ColumnReadOnly.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnReadOnly, "ColumnReadOnly");
            this.ColumnReadOnly.Name = "ColumnReadOnly";
            this.ColumnReadOnly.ReadOnly = true;
            // 
            // ColumnPriority
            // 
            this.ColumnPriority.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnPriority, "ColumnPriority");
            this.ColumnPriority.Name = "ColumnPriority";
            this.ColumnPriority.ReadOnly = true;
            // 
            // ColumnActive
            // 
            this.ColumnActive.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnActive, "ColumnActive");
            this.ColumnActive.Name = "ColumnActive";
            this.ColumnActive.ReadOnly = true;
            // 
            // ColumnDevicePath
            // 
            this.ColumnDevicePath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnDevicePath, "ColumnDevicePath");
            this.ColumnDevicePath.Name = "ColumnDevicePath";
            this.ColumnDevicePath.ReadOnly = true;
            // 
            // multipleDvdIsoList1
            // 
            resources.ApplyResources(this.multipleDvdIsoList1, "multipleDvdIsoList1");
            this.multipleDvdIsoList1.LabelNewCdForeColor = System.Drawing.SystemColors.HotTrack;
            this.multipleDvdIsoList1.LabelSingleDvdForeColor = System.Drawing.SystemColors.ControlText;
            this.multipleDvdIsoList1.LinkLabelLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.multipleDvdIsoList1.Name = "multipleDvdIsoList1";
            this.multipleDvdIsoList1.VM = null;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn2, "dataGridViewTextBoxColumn2");
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn3, "dataGridViewTextBoxColumn3");
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.dataGridViewTextBoxColumn4, "dataGridViewTextBoxColumn4");
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn5, "dataGridViewTextBoxColumn5");
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn6, "dataGridViewTextBoxColumn6");
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn7, "dataGridViewTextBoxColumn7");
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.dataGridViewTextBoxColumn8, "dataGridViewTextBoxColumn8");
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn9, "dataGridViewTextBoxColumn9");
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn10, "dataGridViewTextBoxColumn10");
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.ReadOnly = true;
            // 
            // VMStoragePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Name = "VMStoragePage";
            this.Controls.SetChildIndex(this.pageContainerPanel, 0);
            this.pageContainerPanel.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.DeactivateButtonContainer.ResumeLayout(false);
            this.MoveButtonContainer.ResumeLayout(false);
            this.DetachButtonContainer.ResumeLayout(false);
            this.DeleteButtonContainer.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStorage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button AttachButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button EditButton;
        private System.Windows.Forms.Button DetachButton;
        private System.Windows.Forms.Button DeleteButton;
        private XenAdmin.Controls.ToolTipContainer DetachButtonContainer;
        private XenAdmin.Controls.ToolTipContainer DeleteButtonContainer;
        private XenAdmin.Controls.ToolTipContainer DeactivateButtonContainer;
        private System.Windows.Forms.Button DeactivateButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private XenAdmin.Controls.MultipleDvdIsoList multipleDvdIsoList1;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewStorage;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private XenAdmin.Controls.ToolTipContainer MoveButtonContainer;
        private System.Windows.Forms.Button MoveButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAdd;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAttach;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDeactivate;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMove;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemProperties;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDelete;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDetach;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDevicePosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSR;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSRVolume;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnReadOnly;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPriority;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnActive;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDevicePath;
    }
}
