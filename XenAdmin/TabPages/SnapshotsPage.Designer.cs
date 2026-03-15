namespace XenAdmin.TabPages
{
    sealed partial class SnapshotsPage
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
                ConnectionsManager.History.CollectionChanged -= History_CollectionChanged;

                if(components != null)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SnapshotsPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.GeneralTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.contentTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panelPropertiesColumn = new System.Windows.Forms.Panel();
            this.panelVMPP = new System.Windows.Forms.Panel();
            this.linkLabelVMPPInfo = new System.Windows.Forms.LinkLabel();
            this.labelVMPPInfo = new System.Windows.Forms.Label();
            this.pictureBoxVMPPInfo = new System.Windows.Forms.PictureBox();
            this.propertiesGroupBox = new System.Windows.Forms.GroupBox();
            this.propertiesTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelSimpleSelection = new System.Windows.Forms.TableLayoutPanel();
            this.folderLabel = new System.Windows.Forms.Label();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.tagsLabel = new System.Windows.Forms.Label();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.sizeTitleLabel = new System.Windows.Forms.Label();
            this.folderTitleLabel = new System.Windows.Forms.Label();
            this.tagsTitleLabel = new System.Windows.Forms.Label();
            this.customFieldTitle1 = new System.Windows.Forms.Label();
            this.customFieldContent1 = new System.Windows.Forms.Label();
            this.customFieldTitle2 = new System.Windows.Forms.Label();
            this.customFieldContent2 = new System.Windows.Forms.Label();
            this.labelModeTitle = new System.Windows.Forms.Label();
            this.labelMode = new System.Windows.Forms.Label();
            this.propertiesButton = new System.Windows.Forms.Button();
            this.nameLabel = new System.Windows.Forms.Label();
            this.shadowPanel1 = new XenAdmin.Controls.ShadowPanel();
            this.screenshotPictureBox = new System.Windows.Forms.PictureBox();
            this.viewPanel = new System.Windows.Forms.Panel();
            this.snapshotTreeView = new XenAdmin.Controls.SnapshotTreeView(this.components);
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.newSnapshotButton = new System.Windows.Forms.Button();
            this.toolTipContainerRevertButton = new XenAdmin.Controls.ToolTipContainer();
            this.revertButton = new System.Windows.Forms.Button();
            this.buttonView = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.chevronButton1 = new XenAdmin.Controls.ChevronButton();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TakeSnapshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.revertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveVMToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.saveVMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separatorDeleteToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByCreatedOnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortBySizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.columnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnCreated = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnTags = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanelMultipleSelection = new System.Windows.Forms.TableLayoutPanel();
            this.multipleSelectionTags = new System.Windows.Forms.Label();
            this.multipleSelectionTotalSize = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.saveMenuStrip = new XenAdmin.Controls.NonReopeningContextMenuStrip(this.components);
            this.saveAsVMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveAsTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAsBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripView = new XenAdmin.Controls.NonReopeningContextMenuStrip(this.components);
            this.toolStripButtonListView = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonTreeView = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorView = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemScheduledSnapshots = new System.Windows.Forms.ToolStripMenuItem();
            this.pageContainerPanel.SuspendLayout();
            this.GeneralTableLayoutPanel.SuspendLayout();
            this.contentTableLayoutPanel.SuspendLayout();
            this.panelPropertiesColumn.SuspendLayout();
            this.panelVMPP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVMPPInfo)).BeginInit();
            this.propertiesGroupBox.SuspendLayout();
            this.propertiesTableLayoutPanel.SuspendLayout();
            this.tableLayoutPanelSimpleSelection.SuspendLayout();
            this.shadowPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.screenshotPictureBox)).BeginInit();
            this.viewPanel.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.toolTipContainerRevertButton.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.tableLayoutPanelMultipleSelection.SuspendLayout();
            this.saveMenuStrip.SuspendLayout();
            this.contextMenuStripView.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.GeneralTableLayoutPanel);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // GeneralTableLayoutPanel
            // 
            resources.ApplyResources(this.GeneralTableLayoutPanel, "GeneralTableLayoutPanel");
            this.GeneralTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.GeneralTableLayoutPanel.Controls.Add(this.contentTableLayoutPanel, 0, 1);
            this.GeneralTableLayoutPanel.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.GeneralTableLayoutPanel.Name = "GeneralTableLayoutPanel";
            // 
            // contentTableLayoutPanel
            // 
            resources.ApplyResources(this.contentTableLayoutPanel, "contentTableLayoutPanel");
            this.contentTableLayoutPanel.Controls.Add(this.panelPropertiesColumn, 1, 0);
            this.contentTableLayoutPanel.Controls.Add(this.viewPanel, 0, 0);
            this.contentTableLayoutPanel.Name = "contentTableLayoutPanel";
            // 
            // panelPropertiesColumn
            // 
            this.panelPropertiesColumn.Controls.Add(this.panelVMPP);
            this.panelPropertiesColumn.Controls.Add(this.propertiesGroupBox);
            resources.ApplyResources(this.panelPropertiesColumn, "panelPropertiesColumn");
            this.panelPropertiesColumn.Name = "panelPropertiesColumn";
            // 
            // panelVMPP
            // 
            resources.ApplyResources(this.panelVMPP, "panelVMPP");
            this.panelVMPP.Controls.Add(this.linkLabelVMPPInfo);
            this.panelVMPP.Controls.Add(this.labelVMPPInfo);
            this.panelVMPP.Controls.Add(this.pictureBoxVMPPInfo);
            this.panelVMPP.Name = "panelVMPP";
            // 
            // linkLabelVMPPInfo
            // 
            resources.ApplyResources(this.linkLabelVMPPInfo, "linkLabelVMPPInfo");
            this.linkLabelVMPPInfo.Name = "linkLabelVMPPInfo";
            this.linkLabelVMPPInfo.TabStop = true;
            this.linkLabelVMPPInfo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelVMPPInfo_Click);
            // 
            // labelVMPPInfo
            // 
            resources.ApplyResources(this.labelVMPPInfo, "labelVMPPInfo");
            this.labelVMPPInfo.AutoEllipsis = true;
            this.labelVMPPInfo.Name = "labelVMPPInfo";
            // 
            // pictureBoxVMPPInfo
            // 
            this.pictureBoxVMPPInfo.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.pictureBoxVMPPInfo, "pictureBoxVMPPInfo");
            this.pictureBoxVMPPInfo.Name = "pictureBoxVMPPInfo";
            this.pictureBoxVMPPInfo.TabStop = false;
            // 
            // propertiesGroupBox
            // 
            resources.ApplyResources(this.propertiesGroupBox, "propertiesGroupBox");
            this.propertiesGroupBox.Controls.Add(this.propertiesTableLayoutPanel);
            this.propertiesGroupBox.Name = "propertiesGroupBox";
            this.propertiesGroupBox.TabStop = false;
            // 
            // propertiesTableLayoutPanel
            // 
            resources.ApplyResources(this.propertiesTableLayoutPanel, "propertiesTableLayoutPanel");
            this.propertiesTableLayoutPanel.Controls.Add(this.tableLayoutPanelSimpleSelection, 0, 2);
            this.propertiesTableLayoutPanel.Controls.Add(this.propertiesButton, 0, 3);
            this.propertiesTableLayoutPanel.Controls.Add(this.nameLabel, 0, 1);
            this.propertiesTableLayoutPanel.Controls.Add(this.shadowPanel1, 0, 0);
            this.propertiesTableLayoutPanel.Name = "propertiesTableLayoutPanel";
            // 
            // tableLayoutPanelSimpleSelection
            // 
            resources.ApplyResources(this.tableLayoutPanelSimpleSelection, "tableLayoutPanelSimpleSelection");
            this.tableLayoutPanelSimpleSelection.Controls.Add(this.folderLabel, 1, 4);
            this.tableLayoutPanelSimpleSelection.Controls.Add(this.sizeLabel, 1, 2);
            this.tableLayoutPanelSimpleSelection.Controls.Add(this.tagsLabel, 1, 3);
            this.tableLayoutPanelSimpleSelection.Controls.Add(this.descriptionLabel, 1, 0);
            this.tableLayoutPanelSimpleSelection.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanelSimpleSelection.Controls.Add(this.sizeTitleLabel, 0, 2);
            this.tableLayoutPanelSimpleSelection.Controls.Add(this.folderTitleLabel, 0, 4);
            this.tableLayoutPanelSimpleSelection.Controls.Add(this.tagsTitleLabel, 0, 3);
            this.tableLayoutPanelSimpleSelection.Controls.Add(this.customFieldTitle1, 0, 5);
            this.tableLayoutPanelSimpleSelection.Controls.Add(this.customFieldContent1, 1, 5);
            this.tableLayoutPanelSimpleSelection.Controls.Add(this.customFieldTitle2, 0, 6);
            this.tableLayoutPanelSimpleSelection.Controls.Add(this.customFieldContent2, 1, 6);
            this.tableLayoutPanelSimpleSelection.Controls.Add(this.labelModeTitle, 0, 1);
            this.tableLayoutPanelSimpleSelection.Controls.Add(this.labelMode, 1, 1);
            this.tableLayoutPanelSimpleSelection.Name = "tableLayoutPanelSimpleSelection";
            // 
            // folderLabel
            // 
            this.folderLabel.AutoEllipsis = true;
            resources.ApplyResources(this.folderLabel, "folderLabel");
            this.folderLabel.Name = "folderLabel";
            this.folderLabel.UseMnemonic = false;
            // 
            // sizeLabel
            // 
            resources.ApplyResources(this.sizeLabel, "sizeLabel");
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.UseMnemonic = false;
            // 
            // tagsLabel
            // 
            this.tagsLabel.AutoEllipsis = true;
            resources.ApplyResources(this.tagsLabel, "tagsLabel");
            this.tagsLabel.Name = "tagsLabel";
            this.tagsLabel.UseMnemonic = false;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoEllipsis = true;
            resources.ApplyResources(this.descriptionLabel, "descriptionLabel");
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.UseMnemonic = false;
            // 
            // label1
            // 
            this.label1.AutoEllipsis = true;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.label1.UseMnemonic = false;
            // 
            // sizeTitleLabel
            // 
            resources.ApplyResources(this.sizeTitleLabel, "sizeTitleLabel");
            this.sizeTitleLabel.Name = "sizeTitleLabel";
            this.sizeTitleLabel.UseMnemonic = false;
            // 
            // folderTitleLabel
            // 
            resources.ApplyResources(this.folderTitleLabel, "folderTitleLabel");
            this.folderTitleLabel.Name = "folderTitleLabel";
            this.folderTitleLabel.UseMnemonic = false;
            // 
            // tagsTitleLabel
            // 
            resources.ApplyResources(this.tagsTitleLabel, "tagsTitleLabel");
            this.tagsTitleLabel.Name = "tagsTitleLabel";
            this.tagsTitleLabel.UseMnemonic = false;
            // 
            // customFieldTitle1
            // 
            this.customFieldTitle1.AutoEllipsis = true;
            resources.ApplyResources(this.customFieldTitle1, "customFieldTitle1");
            this.customFieldTitle1.Name = "customFieldTitle1";
            this.customFieldTitle1.UseMnemonic = false;
            // 
            // customFieldContent1
            // 
            this.customFieldContent1.AutoEllipsis = true;
            resources.ApplyResources(this.customFieldContent1, "customFieldContent1");
            this.customFieldContent1.Name = "customFieldContent1";
            this.customFieldContent1.UseMnemonic = false;
            // 
            // customFieldTitle2
            // 
            this.customFieldTitle2.AutoEllipsis = true;
            resources.ApplyResources(this.customFieldTitle2, "customFieldTitle2");
            this.customFieldTitle2.Name = "customFieldTitle2";
            this.customFieldTitle2.UseMnemonic = false;
            // 
            // customFieldContent2
            // 
            this.customFieldContent2.AutoEllipsis = true;
            resources.ApplyResources(this.customFieldContent2, "customFieldContent2");
            this.customFieldContent2.Name = "customFieldContent2";
            this.customFieldContent2.UseMnemonic = false;
            // 
            // labelModeTitle
            // 
            resources.ApplyResources(this.labelModeTitle, "labelModeTitle");
            this.labelModeTitle.Name = "labelModeTitle";
            this.labelModeTitle.UseMnemonic = false;
            // 
            // labelMode
            // 
            resources.ApplyResources(this.labelMode, "labelMode");
            this.labelMode.Name = "labelMode";
            // 
            // propertiesButton
            // 
            resources.ApplyResources(this.propertiesButton, "propertiesButton");
            this.propertiesButton.Name = "propertiesButton";
            this.propertiesButton.UseVisualStyleBackColor = true;
            this.propertiesButton.Click += new System.EventHandler(this.propertiesButton_Click);
            // 
            // nameLabel
            // 
            this.nameLabel.AutoEllipsis = true;
            resources.ApplyResources(this.nameLabel, "nameLabel");
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.UseMnemonic = false;
            // 
            // shadowPanel1
            // 
            resources.ApplyResources(this.shadowPanel1, "shadowPanel1");
            this.shadowPanel1.BorderColor = System.Drawing.Color.Empty;
            this.shadowPanel1.Controls.Add(this.screenshotPictureBox);
            this.shadowPanel1.Name = "shadowPanel1";
            this.shadowPanel1.PanelColor = System.Drawing.Color.Empty;
            // 
            // screenshotPictureBox
            // 
            this.screenshotPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.screenshotPictureBox, "screenshotPictureBox");
            this.screenshotPictureBox.Name = "screenshotPictureBox";
            this.screenshotPictureBox.TabStop = false;
            this.screenshotPictureBox.Click += new System.EventHandler(this.screenshotPictureBox_Click);
            // 
            // viewPanel
            // 
            this.viewPanel.Controls.Add(this.snapshotTreeView);
            resources.ApplyResources(this.viewPanel, "viewPanel");
            this.viewPanel.Name = "viewPanel";
            // 
            // snapshotTreeView
            // 
            resources.ApplyResources(this.snapshotTreeView, "snapshotTreeView");
            this.snapshotTreeView.AllowDrop = true;
            this.snapshotTreeView.AutoArrange = false;
            this.snapshotTreeView.BackgroundImageTiled = true;
            this.snapshotTreeView.GridLines = true;
            this.snapshotTreeView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.snapshotTreeView.HGap = 42;
            this.snapshotTreeView.HideSelection = false;
            this.snapshotTreeView.LinkLineColor = System.Drawing.SystemColors.ActiveBorder;
            this.snapshotTreeView.Name = "snapshotTreeView";
            this.snapshotTreeView.OwnerDraw = true;
            this.snapshotTreeView.ShowGroups = false;
            this.snapshotTreeView.ShowItemToolTips = true;
            this.snapshotTreeView.TileSize = new System.Drawing.Size(80, 60);
            this.snapshotTreeView.UseCompatibleStateImageBehavior = false;
            this.snapshotTreeView.VGap = 50;
            this.snapshotTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.snapshotTreeView_ItemDrag);
            this.snapshotTreeView.SelectedIndexChanged += new System.EventHandler(this.view_SelectionChanged);
            this.snapshotTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.snapshotTreeView_DragDrop);
            this.snapshotTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.snapshotTreeView_DragEnter);
            this.snapshotTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.snapshotTreeView_DragOver);
            this.snapshotTreeView.Enter += new System.EventHandler(this.snapshotTreeView_Enter);
            this.snapshotTreeView.Leave += new System.EventHandler(this.snapshotTreeView_Leave);
            this.snapshotTreeView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.snapshotTreeView1_MouseDoubleClick);
            this.snapshotTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.snapshotTreeView_MouseClick);
            this.snapshotTreeView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.snapshotTreeView_MouseMove);
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.newSnapshotButton, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.toolTipContainerRevertButton, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonView, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.saveButton, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.deleteButton, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.chevronButton1, 6, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // newSnapshotButton
            // 
            resources.ApplyResources(this.newSnapshotButton, "newSnapshotButton");
            this.newSnapshotButton.Name = "newSnapshotButton";
            this.newSnapshotButton.UseVisualStyleBackColor = true;
            this.newSnapshotButton.Click += new System.EventHandler(this.takeSnapshotToolStripButton_Click);
            // 
            // toolTipContainerRevertButton
            // 
            this.toolTipContainerRevertButton.Controls.Add(this.revertButton);
            resources.ApplyResources(this.toolTipContainerRevertButton, "toolTipContainerRevertButton");
            this.toolTipContainerRevertButton.Name = "toolTipContainerRevertButton";
            this.toolTipContainerRevertButton.TabStop = true;
            // 
            // revertButton
            // 
            resources.ApplyResources(this.revertButton, "revertButton");
            this.revertButton.Name = "revertButton";
            this.revertButton.UseVisualStyleBackColor = true;
            this.revertButton.Click += new System.EventHandler(this.revertButton_Click);
            // 
            // buttonView
            // 
            this.buttonView.Image = global::XenAdmin.Properties.Resources.expanded_triangle;
            resources.ApplyResources(this.buttonView, "buttonView");
            this.buttonView.Name = "buttonView";
            this.buttonView.UseVisualStyleBackColor = true;
            this.buttonView.Click += new System.EventHandler(this.button1_Click);
            // 
            // saveButton
            // 
            this.saveButton.Image = global::XenAdmin.Properties.Resources.expanded_triangle;
            resources.ApplyResources(this.saveButton, "saveButton");
            this.saveButton.Name = "saveButton";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // deleteButton
            // 
            resources.ApplyResources(this.deleteButton, "deleteButton");
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // chevronButton1
            // 
            resources.ApplyResources(this.chevronButton1, "chevronButton1");
            this.chevronButton1.Cursor = System.Windows.Forms.Cursors.Default;
            this.chevronButton1.Image = ((System.Drawing.Image)(resources.GetObject("chevronButton1.Image")));
            this.chevronButton1.Name = "chevronButton1";
            this.chevronButton1.ButtonClick += new System.EventHandler(this.chevronButton1_ButtonClick);
            this.chevronButton1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chevronButton1_KeyDown);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TakeSnapshotToolStripMenuItem,
            this.revertToolStripMenuItem,
            this.saveVMToolStripSeparator,
            this.saveVMToolStripMenuItem,
            this.saveTemplateToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.separatorDeleteToolStripSeparator,
            this.viewToolStripMenuItem,
            this.sortByToolStripMenuItem,
            this.sortToolStripSeparator,
            this.deleteToolStripMenuItem,
            this.propertiesToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // TakeSnapshotToolStripMenuItem
            // 
            this.TakeSnapshotToolStripMenuItem.Name = "TakeSnapshotToolStripMenuItem";
            resources.ApplyResources(this.TakeSnapshotToolStripMenuItem, "TakeSnapshotToolStripMenuItem");
            this.TakeSnapshotToolStripMenuItem.Click += new System.EventHandler(this.takeSnapshotToolStripButton_Click);
            // 
            // revertToolStripMenuItem
            // 
            this.revertToolStripMenuItem.Name = "revertToolStripMenuItem";
            resources.ApplyResources(this.revertToolStripMenuItem, "revertToolStripMenuItem");
            this.revertToolStripMenuItem.Click += new System.EventHandler(this.restoreToolStripButton_Click);
            // 
            // saveVMToolStripSeparator
            // 
            this.saveVMToolStripSeparator.Name = "saveVMToolStripSeparator";
            resources.ApplyResources(this.saveVMToolStripSeparator, "saveVMToolStripSeparator");
            // 
            // saveVMToolStripMenuItem
            // 
            this.saveVMToolStripMenuItem.Name = "saveVMToolStripMenuItem";
            resources.ApplyResources(this.saveVMToolStripMenuItem, "saveVMToolStripMenuItem");
            this.saveVMToolStripMenuItem.Click += new System.EventHandler(this.saveAsAVirtualMachineToolStripMenuItem_Click);
            // 
            // saveTemplateToolStripMenuItem
            // 
            this.saveTemplateToolStripMenuItem.Name = "saveTemplateToolStripMenuItem";
            resources.ApplyResources(this.saveTemplateToolStripMenuItem, "saveTemplateToolStripMenuItem");
            this.saveTemplateToolStripMenuItem.Click += new System.EventHandler(this.saveAsATemplateToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            resources.ApplyResources(this.exportToolStripMenuItem, "exportToolStripMenuItem");
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportSnapshotToolStripMenuItem_Click);
            // 
            // separatorDeleteToolStripSeparator
            // 
            this.separatorDeleteToolStripSeparator.Name = "separatorDeleteToolStripSeparator";
            resources.ApplyResources(this.separatorDeleteToolStripSeparator, "separatorDeleteToolStripSeparator");
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // sortByToolStripMenuItem
            // 
            this.sortByToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortByNameToolStripMenuItem,
            this.sortByCreatedOnToolStripMenuItem,
            this.sortBySizeToolStripMenuItem,
            this.sortByTypeToolStripMenuItem});
            this.sortByToolStripMenuItem.Name = "sortByToolStripMenuItem";
            resources.ApplyResources(this.sortByToolStripMenuItem, "sortByToolStripMenuItem");
            // 
            // sortByNameToolStripMenuItem
            // 
            this.sortByNameToolStripMenuItem.Name = "sortByNameToolStripMenuItem";
            resources.ApplyResources(this.sortByNameToolStripMenuItem, "sortByNameToolStripMenuItem");
            this.sortByNameToolStripMenuItem.Click += new System.EventHandler(this.sortByNameToolStripMenuItem_Click);
            // 
            // sortByCreatedOnToolStripMenuItem
            // 
            this.sortByCreatedOnToolStripMenuItem.Name = "sortByCreatedOnToolStripMenuItem";
            resources.ApplyResources(this.sortByCreatedOnToolStripMenuItem, "sortByCreatedOnToolStripMenuItem");
            this.sortByCreatedOnToolStripMenuItem.Click += new System.EventHandler(this.sortByCreatedOnToolStripMenuItem_Click);
            // 
            // sortBySizeToolStripMenuItem
            // 
            this.sortBySizeToolStripMenuItem.Name = "sortBySizeToolStripMenuItem";
            resources.ApplyResources(this.sortBySizeToolStripMenuItem, "sortBySizeToolStripMenuItem");
            this.sortBySizeToolStripMenuItem.Click += new System.EventHandler(this.sortBySizeToolStripMenuItem_Click);
            // 
            // sortByTypeToolStripMenuItem
            // 
            this.sortByTypeToolStripMenuItem.Name = "sortByTypeToolStripMenuItem";
            resources.ApplyResources(this.sortByTypeToolStripMenuItem, "sortByTypeToolStripMenuItem");
            this.sortByTypeToolStripMenuItem.Click += new System.EventHandler(this.sortByTypeToolStripMenuItem_Click);
            // 
            // sortToolStripSeparator
            // 
            this.sortToolStripSeparator.Name = "sortToolStripSeparator";
            resources.ApplyResources(this.sortToolStripSeparator, "sortToolStripSeparator");
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripButton_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            resources.ApplyResources(this.propertiesToolStripMenuItem, "propertiesToolStripMenuItem");
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesButton_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToOrderColumns = true;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.ControlDarkDark;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnType,
            this.columnName,
            this.columnCreated,
            this.columnSize,
            this.columnTags,
            this.ColumnDescription});
            resources.ApplyResources(this.dataGridView, "dataGridView");
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.SelectionChanged += new System.EventHandler(this.view_SelectionChanged);
            this.dataGridView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView_MouseClick);
            // 
            // columnType
            // 
            this.columnType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            resources.ApplyResources(this.columnType, "columnType");
            this.columnType.Name = "columnType";
            this.columnType.ReadOnly = true;
            this.columnType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // columnName
            // 
            this.columnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            resources.ApplyResources(this.columnName, "columnName");
            this.columnName.Name = "columnName";
            this.columnName.ReadOnly = true;
            this.columnName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // columnCreated
            // 
            this.columnCreated.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            resources.ApplyResources(this.columnCreated, "columnCreated");
            this.columnCreated.Name = "columnCreated";
            this.columnCreated.ReadOnly = true;
            this.columnCreated.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // columnSize
            // 
            this.columnSize.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.columnSize.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.columnSize, "columnSize");
            this.columnSize.Name = "columnSize";
            this.columnSize.ReadOnly = true;
            // 
            // columnTags
            // 
            this.columnTags.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            resources.ApplyResources(this.columnTags, "columnTags");
            this.columnTags.Name = "columnTags";
            this.columnTags.ReadOnly = true;
            this.columnTags.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnDescription
            // 
            this.ColumnDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnDescription, "ColumnDescription");
            this.ColumnDescription.Name = "ColumnDescription";
            this.ColumnDescription.ReadOnly = true;
            this.ColumnDescription.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dateLabel
            // 
            resources.ApplyResources(this.dateLabel, "dateLabel");
            this.dateLabel.Name = "dateLabel";
            // 
            // tableLayoutPanelMultipleSelection
            // 
            resources.ApplyResources(this.tableLayoutPanelMultipleSelection, "tableLayoutPanelMultipleSelection");
            this.tableLayoutPanelMultipleSelection.Controls.Add(this.multipleSelectionTags, 1, 1);
            this.tableLayoutPanelMultipleSelection.Controls.Add(this.multipleSelectionTotalSize, 1, 0);
            this.tableLayoutPanelMultipleSelection.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanelMultipleSelection.Controls.Add(this.label7, 0, 1);
            this.tableLayoutPanelMultipleSelection.Name = "tableLayoutPanelMultipleSelection";
            // 
            // multipleSelectionTags
            // 
            resources.ApplyResources(this.multipleSelectionTags, "multipleSelectionTags");
            this.multipleSelectionTags.Name = "multipleSelectionTags";
            // 
            // multipleSelectionTotalSize
            // 
            resources.ApplyResources(this.multipleSelectionTotalSize, "multipleSelectionTotalSize");
            this.multipleSelectionTotalSize.Name = "multipleSelectionTotalSize";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // saveMenuStrip
            // 
            this.saveMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.saveMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAsVMToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveAsTemplateToolStripMenuItem,
            this.exportAsBackupToolStripMenuItem});
            this.saveMenuStrip.Name = "saveMenuStrip";
            resources.ApplyResources(this.saveMenuStrip, "saveMenuStrip");
            // 
            // saveAsVMToolStripMenuItem
            // 
            this.saveAsVMToolStripMenuItem.Name = "saveAsVMToolStripMenuItem";
            resources.ApplyResources(this.saveAsVMToolStripMenuItem, "saveAsVMToolStripMenuItem");
            this.saveAsVMToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // saveAsTemplateToolStripMenuItem
            // 
            this.saveAsTemplateToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.saveAsTemplateToolStripMenuItem.Name = "saveAsTemplateToolStripMenuItem";
            resources.ApplyResources(this.saveAsTemplateToolStripMenuItem, "saveAsTemplateToolStripMenuItem");
            this.saveAsTemplateToolStripMenuItem.Click += new System.EventHandler(this.saveAsTemplateToolStripMenuItem_Click);
            // 
            // exportAsBackupToolStripMenuItem
            // 
            this.exportAsBackupToolStripMenuItem.Name = "exportAsBackupToolStripMenuItem";
            resources.ApplyResources(this.exportAsBackupToolStripMenuItem, "exportAsBackupToolStripMenuItem");
            this.exportAsBackupToolStripMenuItem.Click += new System.EventHandler(this.exportAsBackupToolStripMenuItem_Click);
            // 
            // contextMenuStripView
            // 
            this.contextMenuStripView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonListView,
            this.toolStripButtonTreeView,
            this.toolStripSeparatorView,
            this.toolStripMenuItemScheduledSnapshots});
            this.contextMenuStripView.Name = "contextMenuStripView";
            resources.ApplyResources(this.contextMenuStripView, "contextMenuStripView");
            // 
            // toolStripButtonListView
            // 
            this.toolStripButtonListView.Image = global::XenAdmin.Properties.Resources._000_ViewModeList_h32bit_16;
            this.toolStripButtonListView.Name = "toolStripButtonListView";
            resources.ApplyResources(this.toolStripButtonListView, "toolStripButtonListView");
            this.toolStripButtonListView.Click += new System.EventHandler(this.gridToolStripMenuItem_Click);
            // 
            // toolStripButtonTreeView
            // 
            this.toolStripButtonTreeView.Image = global::XenAdmin.Properties.Resources._000_ViewModeTree_h32bit_16;
            this.toolStripButtonTreeView.Name = "toolStripButtonTreeView";
            resources.ApplyResources(this.toolStripButtonTreeView, "toolStripButtonTreeView");
            this.toolStripButtonTreeView.Click += new System.EventHandler(this.treeToolStripMenuItem_Click);
            // 
            // toolStripSeparatorView
            // 
            this.toolStripSeparatorView.Name = "toolStripSeparatorView";
            resources.ApplyResources(this.toolStripSeparatorView, "toolStripSeparatorView");
            // 
            // toolStripMenuItemScheduledSnapshots
            // 
            this.toolStripMenuItemScheduledSnapshots.Name = "toolStripMenuItemScheduledSnapshots";
            resources.ApplyResources(this.toolStripMenuItemScheduledSnapshots, "toolStripMenuItemScheduledSnapshots");
            this.toolStripMenuItemScheduledSnapshots.Click += new System.EventHandler(this.toolStripMenuItemScheduledSnapshots_Click);
            // 
            // SnapshotsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.DoubleBuffered = true;
            this.Name = "SnapshotsPage";
            this.Controls.SetChildIndex(this.pageContainerPanel, 0);
            this.pageContainerPanel.ResumeLayout(false);
            this.pageContainerPanel.PerformLayout();
            this.GeneralTableLayoutPanel.ResumeLayout(false);
            this.GeneralTableLayoutPanel.PerformLayout();
            this.contentTableLayoutPanel.ResumeLayout(false);
            this.panelPropertiesColumn.ResumeLayout(false);
            this.panelPropertiesColumn.PerformLayout();
            this.panelVMPP.ResumeLayout(false);
            this.panelVMPP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVMPPInfo)).EndInit();
            this.propertiesGroupBox.ResumeLayout(false);
            this.propertiesGroupBox.PerformLayout();
            this.propertiesTableLayoutPanel.ResumeLayout(false);
            this.propertiesTableLayoutPanel.PerformLayout();
            this.tableLayoutPanelSimpleSelection.ResumeLayout(false);
            this.tableLayoutPanelSimpleSelection.PerformLayout();
            this.shadowPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.screenshotPictureBox)).EndInit();
            this.viewPanel.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.toolTipContainerRevertButton.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.tableLayoutPanelMultipleSelection.ResumeLayout(false);
            this.saveMenuStrip.ResumeLayout(false);
            this.contextMenuStripView.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TableLayoutPanel GeneralTableLayoutPanel;
        private XenAdmin.Controls.SnapshotTreeView snapshotTreeView;
        private System.Windows.Forms.Panel viewPanel;
        private System.Windows.Forms.GroupBox propertiesGroupBox;
        private System.Windows.Forms.TableLayoutPanel propertiesTableLayoutPanel;
        private System.Windows.Forms.Label folderTitleLabel;
        private System.Windows.Forms.Label tagsTitleLabel;
        private System.Windows.Forms.Label sizeTitleLabel;
        private System.Windows.Forms.Label dateLabel;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label sizeLabel;
        private System.Windows.Forms.Button propertiesButton;
        private System.Windows.Forms.Label folderLabel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem revertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TakeSnapshotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator separatorDeleteToolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator saveVMToolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem saveVMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveTemplateToolStripMenuItem;
        private System.Windows.Forms.Label tagsLabel;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortByToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator sortToolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem sortByNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortByCreatedOnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortBySizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortByTypeToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox screenshotPictureBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSimpleSelection;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMultipleSelection;
        private System.Windows.Forms.Label multipleSelectionTags;
        private System.Windows.Forms.Label multipleSelectionTotalSize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private XenAdmin.Controls.ShadowPanel shadowPanel1;
        private System.Windows.Forms.TableLayoutPanel contentTableLayoutPanel;
        private XenAdmin.Controls.NonReopeningContextMenuStrip saveMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem saveAsTemplateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsVMToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ToolStripMenuItem exportAsBackupToolStripMenuItem;
        private System.Windows.Forms.Button newSnapshotButton;
        private System.Windows.Forms.Button revertButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Label customFieldTitle1;
        private System.Windows.Forms.Label customFieldContent1;
        private System.Windows.Forms.Label customFieldTitle2;
        private System.Windows.Forms.Label customFieldContent2;
        private XenAdmin.Controls.ToolTipContainer toolTipContainerRevertButton;
        private System.Windows.Forms.Label labelModeTitle;
        private System.Windows.Forms.Label labelMode;
        private System.Windows.Forms.Panel panelPropertiesColumn;
        private System.Windows.Forms.Panel panelVMPP;
        private System.Windows.Forms.Label labelVMPPInfo;
        private System.Windows.Forms.PictureBox pictureBoxVMPPInfo;
        private System.Windows.Forms.LinkLabel linkLabelVMPPInfo;
        private System.Windows.Forms.Button buttonView;
        private XenAdmin.Controls.NonReopeningContextMenuStrip contextMenuStripView;
        private System.Windows.Forms.ToolStripMenuItem toolStripButtonListView;
        private System.Windows.Forms.ToolStripMenuItem toolStripButtonTreeView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorView;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemScheduledSnapshots;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private XenAdmin.Controls.ChevronButton chevronButton1;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnType;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnCreated;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnTags;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDescription;
    }
}
