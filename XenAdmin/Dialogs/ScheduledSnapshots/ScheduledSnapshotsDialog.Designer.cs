namespace XenAdmin.Dialogs.ScheduledSnapshots
{
    partial class ScheduledSnapshotsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScheduledSnapshotsDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNew = new System.Windows.Forms.Button();
            this.buttonEnable = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonRunNow = new System.Windows.Forms.Button();
            this.labelPolicyTitle = new System.Windows.Forms.Label();
            this.buttonProperties = new System.Windows.Forms.Button();
            this.labelTopBlurb = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.deprecationBanner = new XenAdmin.Controls.DeprecationBanner();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.panelPolicies = new System.Windows.Forms.Panel();
            this.panelLoading = new System.Windows.Forms.Panel();
            this.labelLoading = new System.Windows.Forms.Label();
            this.pictureBoxSpinner = new System.Windows.Forms.PictureBox();
            this.dataGridViewPolicies = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnEnabled = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnVMs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnNextSnapshotTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnCorrespondingServerTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLastResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewRunHistory = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnExpand = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.labelHistory = new System.Windows.Forms.Label();
            this.labelShow = new System.Windows.Forms.Label();
            this.comboBoxTimeSpan = new System.Windows.Forms.ComboBox();
            this.ShowHideRunHistoryButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panelPolicies.SuspendLayout();
            this.panelLoading.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPolicies)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRunHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonNew
            // 
            resources.ApplyResources(this.buttonNew, "buttonNew");
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.UseVisualStyleBackColor = true;
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            // 
            // buttonEnable
            // 
            resources.ApplyResources(this.buttonEnable, "buttonEnable");
            this.buttonEnable.Name = "buttonEnable";
            this.buttonEnable.UseVisualStyleBackColor = true;
            this.buttonEnable.Click += new System.EventHandler(this.buttonEnable_Click);
            // 
            // buttonDelete
            // 
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonRunNow
            // 
            resources.ApplyResources(this.buttonRunNow, "buttonRunNow");
            this.buttonRunNow.Name = "buttonRunNow";
            this.buttonRunNow.UseVisualStyleBackColor = true;
            this.buttonRunNow.Click += new System.EventHandler(this.buttonRunNow_Click);
            // 
            // labelPolicyTitle
            // 
            resources.ApplyResources(this.labelPolicyTitle, "labelPolicyTitle");
            this.labelPolicyTitle.AutoEllipsis = true;
            this.tableLayoutPanel3.SetColumnSpan(this.labelPolicyTitle, 3);
            this.labelPolicyTitle.Name = "labelPolicyTitle";
            // 
            // buttonProperties
            // 
            resources.ApplyResources(this.buttonProperties, "buttonProperties");
            this.buttonProperties.Name = "buttonProperties";
            this.buttonProperties.UseVisualStyleBackColor = true;
            this.buttonProperties.Click += new System.EventHandler(this.buttonProperties_Click);
            // 
            // labelTopBlurb
            // 
            resources.ApplyResources(this.labelTopBlurb, "labelTopBlurb");
            this.labelTopBlurb.Name = "labelTopBlurb";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelTopBlurb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.deprecationBanner, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // deprecationBanner
            // 
            resources.ApplyResources(this.deprecationBanner, "deprecationBanner");
            this.deprecationBanner.BackColor = System.Drawing.Color.LightCoral;
            this.deprecationBanner.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.deprecationBanner.Name = "deprecationBanner";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.panelPolicies, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.labelPolicyTitle, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.buttonNew, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.buttonEnable, 2, 2);
            this.tableLayoutPanel3.Controls.Add(this.buttonRunNow, 2, 3);
            this.tableLayoutPanel3.Controls.Add(this.buttonDelete, 2, 4);
            this.tableLayoutPanel3.Controls.Add(this.buttonProperties, 2, 5);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // panelPolicies
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.panelPolicies, 2);
            this.panelPolicies.Controls.Add(this.panelLoading);
            this.panelPolicies.Controls.Add(this.dataGridViewPolicies);
            resources.ApplyResources(this.panelPolicies, "panelPolicies");
            this.panelPolicies.Name = "panelPolicies";
            this.tableLayoutPanel3.SetRowSpan(this.panelPolicies, 6);
            // 
            // panelLoading
            // 
            resources.ApplyResources(this.panelLoading, "panelLoading");
            this.panelLoading.BackColor = System.Drawing.SystemColors.Window;
            this.panelLoading.Controls.Add(this.labelLoading);
            this.panelLoading.Controls.Add(this.pictureBoxSpinner);
            this.panelLoading.Name = "panelLoading";
            // 
            // labelLoading
            // 
            resources.ApplyResources(this.labelLoading, "labelLoading");
            this.labelLoading.Name = "labelLoading";
            // 
            // pictureBoxSpinner
            // 
            resources.ApplyResources(this.pictureBoxSpinner, "pictureBoxSpinner");
            this.pictureBoxSpinner.Image = global::XenAdmin.Properties.Resources.ajax_loader;
            this.pictureBoxSpinner.Name = "pictureBoxSpinner";
            this.pictureBoxSpinner.TabStop = false;
            // 
            // dataGridViewPolicies
            // 
            this.dataGridViewPolicies.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridViewPolicies.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewPolicies.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewPolicies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewPolicies.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnName,
            this.ColumnEnabled,
            this.ColumnVMs,
            this.ColumnNextSnapshotTime,
            this.ColumnCorrespondingServerTime,
            this.ColumnLastResult});
            resources.ApplyResources(this.dataGridViewPolicies, "dataGridViewPolicies");
            this.dataGridViewPolicies.GridColor = System.Drawing.SystemColors.Control;
            this.dataGridViewPolicies.MultiSelect = true;
            this.dataGridViewPolicies.Name = "dataGridViewPolicies";
            this.dataGridViewPolicies.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewPolicies.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewPolicies.SelectionChanged += new System.EventHandler(this.dataGridViewPolicies_SelectionChanged);
            // 
            // ColumnName
            // 
            this.ColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnName, "ColumnName");
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            // 
            // ColumnEnabled
            // 
            this.ColumnEnabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnEnabled, "ColumnEnabled");
            this.ColumnEnabled.Name = "ColumnEnabled";
            this.ColumnEnabled.ReadOnly = true;
            this.ColumnEnabled.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ColumnVMs
            // 
            this.ColumnVMs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnVMs.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.ColumnVMs, "ColumnVMs");
            this.ColumnVMs.Name = "ColumnVMs";
            this.ColumnVMs.ReadOnly = true;
            // 
            // ColumnNextSnapshotTime
            // 
            this.ColumnNextSnapshotTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnNextSnapshotTime, "ColumnNextSnapshotTime");
            this.ColumnNextSnapshotTime.Name = "ColumnNextSnapshotTime";
            this.ColumnNextSnapshotTime.ReadOnly = true;
            // 
            // ColumnCorrespondingServerTime
            // 
            this.ColumnCorrespondingServerTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnCorrespondingServerTime, "ColumnCorrespondingServerTime");
            this.ColumnCorrespondingServerTime.Name = "ColumnCorrespondingServerTime";
            this.ColumnCorrespondingServerTime.ReadOnly = true;
            // 
            // ColumnLastResult
            // 
            this.ColumnLastResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnLastResult, "ColumnLastResult");
            this.ColumnLastResult.Name = "ColumnLastResult";
            this.ColumnLastResult.ReadOnly = true;
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.buttonCancel, 1, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel2, 0, 3);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel4, 0, 4);
            this.tableLayoutPanel5.Controls.Add(this.ShowHideRunHistoryButton, 0, 2);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.dataGridViewRunHistory, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelHistory, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelShow, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.comboBoxTimeSpan, 2, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // dataGridViewRunHistory
            // 
            this.dataGridViewRunHistory.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewRunHistory.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewRunHistory.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewRunHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewRunHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnExpand,
            this.ColumnResult,
            this.ColumnDateTime,
            this.ColumnDescription});
            this.tableLayoutPanel2.SetColumnSpan(this.dataGridViewRunHistory, 3);
            resources.ApplyResources(this.dataGridViewRunHistory, "dataGridViewRunHistory");
            this.dataGridViewRunHistory.GridColor = System.Drawing.SystemColors.Control;
            this.dataGridViewRunHistory.Name = "dataGridViewRunHistory";
            this.dataGridViewRunHistory.ReadOnly = true;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewRunHistory.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewRunHistory.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewRunHistory.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRunHistory_CellClick);
            // 
            // ColumnExpand
            // 
            this.ColumnExpand.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle3.NullValue = "System.Drawing.Bitmap";
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(5);
            this.ColumnExpand.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.ColumnExpand, "ColumnExpand");
            this.ColumnExpand.Name = "ColumnExpand";
            this.ColumnExpand.ReadOnly = true;
            // 
            // ColumnResult
            // 
            this.ColumnResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnResult.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.ColumnResult, "ColumnResult");
            this.ColumnResult.Name = "ColumnResult";
            this.ColumnResult.ReadOnly = true;
            // 
            // ColumnDateTime
            // 
            this.ColumnDateTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnDateTime, "ColumnDateTime");
            this.ColumnDateTime.Name = "ColumnDateTime";
            this.ColumnDateTime.ReadOnly = true;
            // 
            // ColumnDescription
            // 
            this.ColumnDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnDescription, "ColumnDescription");
            this.ColumnDescription.Name = "ColumnDescription";
            this.ColumnDescription.ReadOnly = true;
            // 
            // labelHistory
            // 
            resources.ApplyResources(this.labelHistory, "labelHistory");
            this.labelHistory.Name = "labelHistory";
            // 
            // labelShow
            // 
            resources.ApplyResources(this.labelShow, "labelShow");
            this.labelShow.Name = "labelShow";
            // 
            // comboBoxTimeSpan
            // 
            resources.ApplyResources(this.comboBoxTimeSpan, "comboBoxTimeSpan");
            this.comboBoxTimeSpan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTimeSpan.FormattingEnabled = true;
            this.comboBoxTimeSpan.Items.AddRange(new object[] {
            resources.GetString("comboBoxTimeSpan.Items"),
            resources.GetString("comboBoxTimeSpan.Items1"),
            resources.GetString("comboBoxTimeSpan.Items2")});
            this.comboBoxTimeSpan.Name = "comboBoxTimeSpan";
            this.comboBoxTimeSpan.SelectedIndexChanged += new System.EventHandler(this.comboBoxTimeSpan_SelectedIndexChanged);
            // 
            // ShowHideRunHistoryButton
            // 
            resources.ApplyResources(this.ShowHideRunHistoryButton, "ShowHideRunHistoryButton");
            this.ShowHideRunHistoryButton.Image = global::XenAdmin.Properties.Resources.PDChevronDown;
            this.ShowHideRunHistoryButton.Name = "ShowHideRunHistoryButton";
            this.ShowHideRunHistoryButton.UseVisualStyleBackColor = true;
            this.ShowHideRunHistoryButton.Click += new System.EventHandler(this.ShowHideRunHistoryButton_Click);
            // 
            // ScheduledSnapshotsDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tableLayoutPanel5);
            this.Name = "ScheduledSnapshotsDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.VMProtectionPoliciesDialog_FormClosed);
            this.Load += new System.EventHandler(this.VMProtectionPoliciesDialog_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.panelPolicies.ResumeLayout(false);
            this.panelLoading.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPolicies)).EndInit();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRunHistory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNew;
        private System.Windows.Forms.Button buttonEnable;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonRunNow;
        private System.Windows.Forms.Label labelPolicyTitle;
        private System.Windows.Forms.Button buttonProperties;
        private System.Windows.Forms.Label labelTopBlurb;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private XenAdmin.Controls.DeprecationBanner deprecationBanner;
        private System.Windows.Forms.Panel panelLoading;
        private System.Windows.Forms.PictureBox pictureBoxSpinner;
        private System.Windows.Forms.Label labelLoading;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewRunHistory;
        private System.Windows.Forms.DataGridViewImageColumn ColumnExpand;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnResult;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDescription;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label labelHistory;
        private System.Windows.Forms.Label labelShow;
        private System.Windows.Forms.ComboBox comboBoxTimeSpan;
        private System.Windows.Forms.Panel panelPolicies;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewPolicies;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnVMs;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnNextSnapshotTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCorrespondingServerTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLastResult;
        private System.Windows.Forms.Button ShowHideRunHistoryButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    }
}
