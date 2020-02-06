namespace XenAdmin.Wizards.ImportWizard
{
	partial class ImportFinishPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportFinishPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_checkBoxStartVms = new System.Windows.Forms.CheckBox();
            this.m_labelIntro = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_dataGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.m_checkBoxStartVms, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_labelIntro, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_dataGridView, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // m_checkBoxStartVms
            // 
            resources.ApplyResources(this.m_checkBoxStartVms, "m_checkBoxStartVms");
            this.m_checkBoxStartVms.Checked = true;
            this.m_checkBoxStartVms.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_checkBoxStartVms.Name = "m_checkBoxStartVms";
            this.m_checkBoxStartVms.UseVisualStyleBackColor = true;
            // 
            // m_labelIntro
            // 
            resources.ApplyResources(this.m_labelIntro, "m_labelIntro");
            this.m_labelIntro.Name = "m_labelIntro";
            // 
            // m_dataGridView
            // 
            this.m_dataGridView.AllowUserToAddRows = false;
            this.m_dataGridView.AllowUserToDeleteRows = false;
            this.m_dataGridView.AllowUserToResizeColumns = false;
            this.m_dataGridView.AllowUserToResizeRows = false;
            this.m_dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.m_dataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.m_dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.m_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dataGridView.ColumnHeadersVisible = false;
            this.m_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            resources.ApplyResources(this.m_dataGridView, "m_dataGridView");
            this.m_dataGridView.MultiSelect = false;
            this.m_dataGridView.Name = "m_dataGridView";
            this.m_dataGridView.ReadOnly = true;
            this.m_dataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            this.m_dataGridView.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.m_dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // Column1
            // 
            resources.ApplyResources(this.Column1, "Column1");
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.Column2, "Column2");
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // ImportFinishPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ImportFinishPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox m_checkBoxStartVms;
		private XenAdmin.Controls.Common.AutoHeightLabel m_labelIntro;
		private System.Windows.Forms.DataGridView m_dataGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column2;

    }
}
