using XenAdmin.Controls.DataGridViewEx;

namespace XenAdmin.Wizards.PatchingWizard
{
    partial class PatchingWizard_SelectPatchPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatchingWizard_SelectPatchPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelWithoutAutomatedUpdates = new System.Windows.Forms.Label();
            this.labelWithAutomatedUpdates = new System.Windows.Forms.Label();
            this.AutomatedUpdatesRadioButton = new System.Windows.Forms.RadioButton();
            this.automatedUpdatesOptionLabel = new System.Windows.Forms.Label();
            this.downloadUpdateRadioButton = new System.Windows.Forms.RadioButton();
            this.RefreshListButton = new System.Windows.Forms.Button();
            this.RestoreDismUpdatesButton = new System.Windows.Forms.Button();
            this.selectFromDiskRadioButton = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.fileNameTextBox = new System.Windows.Forms.TextBox();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanelSpinner = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewPatches = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnUpdate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.webPageColumn = new System.Windows.Forms.DataGridViewLinkColumn();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanelSpinner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPatches)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelWithoutAutomatedUpdates, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelWithAutomatedUpdates, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.AutomatedUpdatesRadioButton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.automatedUpdatesOptionLabel, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.downloadUpdateRadioButton, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.RefreshListButton, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.RestoreDismUpdatesButton, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.selectFromDiskRadioButton, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelWithoutAutomatedUpdates
            // 
            resources.ApplyResources(this.labelWithoutAutomatedUpdates, "labelWithoutAutomatedUpdates");
            this.tableLayoutPanel1.SetColumnSpan(this.labelWithoutAutomatedUpdates, 3);
            this.labelWithoutAutomatedUpdates.Name = "labelWithoutAutomatedUpdates";
            // 
            // labelWithAutomatedUpdates
            // 
            resources.ApplyResources(this.labelWithAutomatedUpdates, "labelWithAutomatedUpdates");
            this.tableLayoutPanel1.SetColumnSpan(this.labelWithAutomatedUpdates, 3);
            this.labelWithAutomatedUpdates.Name = "labelWithAutomatedUpdates";
            // 
            // AutomatedUpdatesRadioButton
            // 
            resources.ApplyResources(this.AutomatedUpdatesRadioButton, "AutomatedUpdatesRadioButton");
            this.AutomatedUpdatesRadioButton.Checked = true;
            this.tableLayoutPanel1.SetColumnSpan(this.AutomatedUpdatesRadioButton, 3);
            this.AutomatedUpdatesRadioButton.Name = "AutomatedUpdatesRadioButton";
            this.AutomatedUpdatesRadioButton.TabStop = true;
            this.AutomatedUpdatesRadioButton.UseVisualStyleBackColor = true;
            this.AutomatedUpdatesRadioButton.CheckedChanged += new System.EventHandler(this.AutomaticRadioButton_CheckedChanged);
            this.AutomatedUpdatesRadioButton.TabStopChanged += new System.EventHandler(this.AutomaticRadioButton_TabStopChanged);
            // 
            // automatedUpdatesOptionLabel
            // 
            resources.ApplyResources(this.automatedUpdatesOptionLabel, "automatedUpdatesOptionLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.automatedUpdatesOptionLabel, 2);
            this.automatedUpdatesOptionLabel.Name = "automatedUpdatesOptionLabel";
            // 
            // downloadUpdateRadioButton
            // 
            resources.ApplyResources(this.downloadUpdateRadioButton, "downloadUpdateRadioButton");
            this.tableLayoutPanel1.SetColumnSpan(this.downloadUpdateRadioButton, 3);
            this.downloadUpdateRadioButton.Name = "downloadUpdateRadioButton";
            this.downloadUpdateRadioButton.UseVisualStyleBackColor = true;
            this.downloadUpdateRadioButton.CheckedChanged += new System.EventHandler(this.downloadUpdateRadioButton_CheckedChanged);
            this.downloadUpdateRadioButton.TabStopChanged += new System.EventHandler(this.downloadUpdateRadioButton_TabStopChanged);
            // 
            // RefreshListButton
            // 
            resources.ApplyResources(this.RefreshListButton, "RefreshListButton");
            this.RefreshListButton.Name = "RefreshListButton";
            this.RefreshListButton.UseVisualStyleBackColor = true;
            this.RefreshListButton.Click += new System.EventHandler(this.RefreshListButton_Click);
            // 
            // RestoreDismUpdatesButton
            // 
            resources.ApplyResources(this.RestoreDismUpdatesButton, "RestoreDismUpdatesButton");
            this.RestoreDismUpdatesButton.Name = "RestoreDismUpdatesButton";
            this.RestoreDismUpdatesButton.UseVisualStyleBackColor = true;
            this.RestoreDismUpdatesButton.Click += new System.EventHandler(this.RestoreDismUpdatesButton_Click);
            // 
            // selectFromDiskRadioButton
            // 
            resources.ApplyResources(this.selectFromDiskRadioButton, "selectFromDiskRadioButton");
            this.tableLayoutPanel1.SetColumnSpan(this.selectFromDiskRadioButton, 3);
            this.selectFromDiskRadioButton.Name = "selectFromDiskRadioButton";
            this.selectFromDiskRadioButton.UseVisualStyleBackColor = true;
            this.selectFromDiskRadioButton.CheckedChanged += new System.EventHandler(this.selectFromDiskRadioButton_CheckedChanged);
            this.selectFromDiskRadioButton.TabStopChanged += new System.EventHandler(this.selectFromDiskRadioButton_TabStopChanged);
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 2);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.fileNameTextBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.BrowseButton, 2, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // fileNameTextBox
            // 
            resources.ApplyResources(this.fileNameTextBox, "fileNameTextBox");
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.TextChanged += new System.EventHandler(this.fileNameTextBox_TextChanged);
            this.fileNameTextBox.Enter += new System.EventHandler(this.fileNameTextBox_Enter);
            // 
            // BrowseButton
            // 
            resources.ApplyResources(this.BrowseButton, "BrowseButton");
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // panel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.tableLayoutPanelSpinner);
            this.panel1.Controls.Add(this.dataGridViewPatches);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // tableLayoutPanelSpinner
            // 
            resources.ApplyResources(this.tableLayoutPanelSpinner, "tableLayoutPanelSpinner");
            this.tableLayoutPanelSpinner.BackColor = System.Drawing.SystemColors.Window;
            this.tableLayoutPanelSpinner.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanelSpinner.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanelSpinner.Name = "tableLayoutPanelSpinner";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources.ajax_loader;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.SystemColors.Window;
            this.label1.Name = "label1";
            // 
            // dataGridViewPatches
            // 
            this.dataGridViewPatches.AllowUserToResizeColumns = false;
            this.dataGridViewPatches.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewPatches.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewPatches.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewPatches.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewPatches.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnUpdate,
            this.ColumnDescription,
            this.ColumnDate,
            this.webPageColumn});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewPatches.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.dataGridViewPatches, "dataGridViewPatches");
            this.dataGridViewPatches.HideSelection = true;
            this.dataGridViewPatches.Name = "dataGridViewPatches";
            this.dataGridViewPatches.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewPatches.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewPatches.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridViewPatches.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewPatches.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewPatches_CellContentClick);
            this.dataGridViewPatches.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewPatches_CellMouseClick);
            this.dataGridViewPatches.SelectionChanged += new System.EventHandler(this.dataGridViewPatches_SelectionChanged);
            this.dataGridViewPatches.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGridViewPatches_SortCompare);
            this.dataGridViewPatches.Enter += new System.EventHandler(this.dataGridViewPatches_Enter);
            // 
            // ColumnUpdate
            // 
            this.ColumnUpdate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnUpdate.DefaultCellStyle = dataGridViewCellStyle1;
            this.ColumnUpdate.FillWeight = 76.67365F;
            resources.ApplyResources(this.ColumnUpdate, "ColumnUpdate");
            this.ColumnUpdate.Name = "ColumnUpdate";
            this.ColumnUpdate.ReadOnly = true;
            // 
            // ColumnDescription
            // 
            this.ColumnDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnDescription.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnDescription.FillWeight = 172.4619F;
            resources.ApplyResources(this.ColumnDescription, "ColumnDescription");
            this.ColumnDescription.Name = "ColumnDescription";
            this.ColumnDescription.ReadOnly = true;
            // 
            // ColumnDate
            // 
            this.ColumnDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnDate.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColumnDate.FillWeight = 80F;
            resources.ApplyResources(this.ColumnDate, "ColumnDate");
            this.ColumnDate.Name = "ColumnDate";
            this.ColumnDate.ReadOnly = true;
            // 
            // webPageColumn
            // 
            this.webPageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.webPageColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.webPageColumn.FillWeight = 60F;
            resources.ApplyResources(this.webPageColumn, "webPageColumn");
            this.webPageColumn.Name = "webPageColumn";
            this.webPageColumn.ReadOnly = true;
            this.webPageColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // PatchingWizard_SelectPatchPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PatchingWizard_SelectPatchPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanelSpinner.ResumeLayout(false);
            this.tableLayoutPanelSpinner.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPatches)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BrowseButton;
        private DataGridViewEx dataGridViewPatches;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox fileNameTextBox;
        private System.Windows.Forms.RadioButton selectFromDiskRadioButton;
        private System.Windows.Forms.Button RefreshListButton;
        private System.Windows.Forms.RadioButton downloadUpdateRadioButton;
        private System.Windows.Forms.Label labelWithAutomatedUpdates;
        private System.Windows.Forms.Button RestoreDismUpdatesButton;
        private System.Windows.Forms.Label automatedUpdatesOptionLabel;
        private System.Windows.Forms.RadioButton AutomatedUpdatesRadioButton;
        private System.Windows.Forms.Label labelWithoutAutomatedUpdates;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSpinner;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnUpdate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDate;
        private System.Windows.Forms.DataGridViewLinkColumn webPageColumn;
    }
}
