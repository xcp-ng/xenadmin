namespace XenAdmin.TabPages
{
    partial class AdPage
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
            ConnectionsManager.History.CollectionChanged -= new System.ComponentModel.CollectionChangeEventHandler(History_CollectionChanged);
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.contextMenuStripADBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemLogout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemChangeRoles = new System.Windows.Forms.ToolStripMenuItem();
            this.GridViewSubjectList = new System.Windows.Forms.DataGridView();
            this.ColumnExpand = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnTypeImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnSubject = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnRoles = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LabelGridViewDisabled = new System.Windows.Forms.Label();
            this.ButtonRemove = new System.Windows.Forms.Button();
            this.tTipRemoveButton = new XenAdmin.Controls.ToolTipContainer();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.labelBlurb = new System.Windows.Forms.Label();
            this.buttonJoinLeave = new System.Windows.Forms.Button();
            this.tTipChangeRole = new XenAdmin.Controls.ToolTipContainer();
            this.ButtonChangeRoles = new System.Windows.Forms.Button();
            this.tTipLogoutButton = new XenAdmin.Controls.ToolTipContainer();
            this.ButtonLogout = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.pageContainerPanel.SuspendLayout();
            this.contextMenuStripADBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridViewSubjectList)).BeginInit();
            this.tTipRemoveButton.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tTipChangeRole.SuspendLayout();
            this.tTipLogoutButton.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.panel1);
            this.pageContainerPanel.Controls.Add(this.label1);
            this.pageContainerPanel.Controls.Add(this.tableLayoutPanel2);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // contextMenuStripADBox
            // 
            this.contextMenuStripADBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.toolStripMenuItemRemove,
            this.toolStripSeparator1,
            this.toolStripMenuItemLogout,
            this.toolStripMenuItemChangeRoles});
            this.contextMenuStripADBox.Name = "contextMenuStripADBox";
            resources.ApplyResources(this.contextMenuStripADBox, "contextMenuStripADBox");
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            resources.ApplyResources(this.addToolStripMenuItem, "addToolStripMenuItem");
            this.addToolStripMenuItem.Click += new System.EventHandler(this.buttonResolve_Click);
            // 
            // toolStripMenuItemRemove
            // 
            this.toolStripMenuItemRemove.Name = "toolStripMenuItemRemove";
            resources.ApplyResources(this.toolStripMenuItemRemove, "toolStripMenuItemRemove");
            this.toolStripMenuItemRemove.Click += new System.EventHandler(this.ButtonRemove_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripMenuItemLogout
            // 
            this.toolStripMenuItemLogout.Name = "toolStripMenuItemLogout";
            resources.ApplyResources(this.toolStripMenuItemLogout, "toolStripMenuItemLogout");
            this.toolStripMenuItemLogout.Click += new System.EventHandler(this.ButtonLogout_Click);
            // 
            // toolStripMenuItemChangeRoles
            // 
            this.toolStripMenuItemChangeRoles.Name = "toolStripMenuItemChangeRoles";
            resources.ApplyResources(this.toolStripMenuItemChangeRoles, "toolStripMenuItemChangeRoles");
            this.toolStripMenuItemChangeRoles.Click += new System.EventHandler(this.ButtonChangeRoles_Click);
            // 
            // GridViewSubjectList
            // 
            this.GridViewSubjectList.AllowUserToAddRows = false;
            this.GridViewSubjectList.AllowUserToDeleteRows = false;
            this.GridViewSubjectList.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            this.GridViewSubjectList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.GridViewSubjectList.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.GridViewSubjectList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.GridViewSubjectList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            resources.ApplyResources(this.GridViewSubjectList, "GridViewSubjectList");
            this.GridViewSubjectList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.GridViewSubjectList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnExpand,
            this.ColumnTypeImage,
            this.ColumnSubject,
            this.ColumnRoles,
            this.ColumnStatus});
            this.GridViewSubjectList.ContextMenuStrip = this.contextMenuStripADBox;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(1);
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.GridViewSubjectList.DefaultCellStyle = dataGridViewCellStyle6;
            this.GridViewSubjectList.Name = "GridViewSubjectList";
            this.GridViewSubjectList.RowHeadersVisible = false;
            this.GridViewSubjectList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GridViewSubjectList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridViewSubjectList_CellClick);
            this.GridViewSubjectList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridViewSubjectList_CellDoubleClick);
            this.GridViewSubjectList.SelectionChanged += new System.EventHandler(this.GridViewSubjectList_SelectionChanged);
            this.GridViewSubjectList.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.GridViewSubjectList_SortCompare);
            // 
            // ColumnExpand
            // 
            this.ColumnExpand.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.ColumnExpand.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.ColumnExpand, "ColumnExpand");
            this.ColumnExpand.Name = "ColumnExpand";
            this.ColumnExpand.ReadOnly = true;
            this.ColumnExpand.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColumnTypeImage
            // 
            this.ColumnTypeImage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.ColumnTypeImage.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.ColumnTypeImage, "ColumnTypeImage");
            this.ColumnTypeImage.Name = "ColumnTypeImage";
            this.ColumnTypeImage.ReadOnly = true;
            this.ColumnTypeImage.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnTypeImage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ColumnSubject
            // 
            this.ColumnSubject.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnSubject.DefaultCellStyle = dataGridViewCellStyle4;
            this.ColumnSubject.FillWeight = 200F;
            resources.ApplyResources(this.ColumnSubject, "ColumnSubject");
            this.ColumnSubject.Name = "ColumnSubject";
            this.ColumnSubject.ReadOnly = true;
            // 
            // ColumnRoles
            // 
            this.ColumnRoles.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnRoles.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.ColumnRoles, "ColumnRoles");
            this.ColumnRoles.Name = "ColumnRoles";
            this.ColumnRoles.ReadOnly = true;
            this.ColumnRoles.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColumnStatus
            // 
            this.ColumnStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnStatus, "ColumnStatus");
            this.ColumnStatus.Name = "ColumnStatus";
            this.ColumnStatus.ReadOnly = true;
            this.ColumnStatus.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // LabelGridViewDisabled
            // 
            resources.ApplyResources(this.LabelGridViewDisabled, "LabelGridViewDisabled");
            this.LabelGridViewDisabled.BackColor = System.Drawing.SystemColors.Menu;
            this.LabelGridViewDisabled.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.LabelGridViewDisabled.Name = "LabelGridViewDisabled";
            // 
            // ButtonRemove
            // 
            resources.ApplyResources(this.ButtonRemove, "ButtonRemove");
            this.ButtonRemove.Name = "ButtonRemove";
            this.ButtonRemove.UseVisualStyleBackColor = true;
            this.ButtonRemove.Click += new System.EventHandler(this.ButtonRemove_Click);
            // 
            // tTipRemoveButton
            // 
            this.tTipRemoveButton.Controls.Add(this.ButtonRemove);
            resources.ApplyResources(this.tTipRemoveButton, "tTipRemoveButton");
            this.tTipRemoveButton.Name = "tTipRemoveButton";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.labelBlurb, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonJoinLeave, 0, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // labelBlurb
            // 
            resources.ApplyResources(this.labelBlurb, "labelBlurb");
            this.labelBlurb.Name = "labelBlurb";
            this.labelBlurb.UseMnemonic = false;
            // 
            // buttonJoinLeave
            // 
            resources.ApplyResources(this.buttonJoinLeave, "buttonJoinLeave");
            this.buttonJoinLeave.Name = "buttonJoinLeave";
            this.buttonJoinLeave.UseVisualStyleBackColor = true;
            this.buttonJoinLeave.Click += new System.EventHandler(this.buttonJoinLeave_Click);
            // 
            // tTipChangeRole
            // 
            this.tTipChangeRole.Controls.Add(this.ButtonChangeRoles);
            resources.ApplyResources(this.tTipChangeRole, "tTipChangeRole");
            this.tTipChangeRole.Name = "tTipChangeRole";
            // 
            // ButtonChangeRoles
            // 
            resources.ApplyResources(this.ButtonChangeRoles, "ButtonChangeRoles");
            this.ButtonChangeRoles.Name = "ButtonChangeRoles";
            this.ButtonChangeRoles.UseVisualStyleBackColor = true;
            this.ButtonChangeRoles.Click += new System.EventHandler(this.ButtonChangeRoles_Click);
            // 
            // tTipLogoutButton
            // 
            this.tTipLogoutButton.Controls.Add(this.ButtonLogout);
            resources.ApplyResources(this.tTipLogoutButton, "tTipLogoutButton");
            this.tTipLogoutButton.Name = "tTipLogoutButton";
            // 
            // ButtonLogout
            // 
            resources.ApplyResources(this.ButtonLogout, "ButtonLogout");
            this.ButtonLogout.Name = "ButtonLogout";
            this.ButtonLogout.UseVisualStyleBackColor = true;
            this.ButtonLogout.Click += new System.EventHandler(this.ButtonLogout_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.buttonAdd);
            this.flowLayoutPanel1.Controls.Add(this.tTipRemoveButton);
            this.flowLayoutPanel1.Controls.Add(this.tTipLogoutButton);
            this.flowLayoutPanel1.Controls.Add(this.tTipChangeRole);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // buttonAdd
            // 
            resources.ApplyResources(this.buttonAdd, "buttonAdd");
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonResolve_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.GridViewSubjectList);
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.LabelGridViewDisabled);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // AdPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Name = "AdPage";
            this.Controls.SetChildIndex(this.pageContainerPanel, 0);
            this.pageContainerPanel.ResumeLayout(false);
            this.pageContainerPanel.PerformLayout();
            this.contextMenuStripADBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GridViewSubjectList)).EndInit();
            this.tTipRemoveButton.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tTipChangeRole.ResumeLayout(false);
            this.tTipLogoutButton.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.ToolTipContainer tTipChangeRole;
        private System.Windows.Forms.Button ButtonChangeRoles;
        private XenAdmin.Controls.ToolTipContainer tTipLogoutButton;
        private System.Windows.Forms.Button ButtonLogout;
        private XenAdmin.Controls.ToolTipContainer tTipRemoveButton;
        private System.Windows.Forms.Button ButtonRemove;
        private System.Windows.Forms.Label LabelGridViewDisabled;
        private System.Windows.Forms.DataGridView GridViewSubjectList;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripADBox;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemChangeRoles;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLogout;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRemove;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label labelBlurb;
        private System.Windows.Forms.Button buttonJoinLeave;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridViewImageColumn ColumnExpand;
        private System.Windows.Forms.DataGridViewImageColumn ColumnTypeImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSubject;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnRoles;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnStatus;
    }
}
