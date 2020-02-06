namespace XenAdmin.Wizards.GenericPages
{
    partial class SelectMultipleVMDestinationPage
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
                UnregisterHandlers();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectMultipleVMDestinationPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_labelIntro = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.m_comboBoxConnection = new XenAdmin.Controls.EnableableComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.m_dataGridView = new System.Windows.Forms.DataGridView();
            this.m_colVmName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_colTarget = new XenAdmin.Controls.EnableableComboBoxColumn();
            this.tableLayoutPanelWarning = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelWarning = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).BeginInit();
            this.tableLayoutPanelWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.m_labelIntro, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.m_comboBoxConnection, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_dataGridView, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelWarning, 0, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // m_labelIntro
            // 
            resources.ApplyResources(this.m_labelIntro, "m_labelIntro");
            this.tableLayoutPanel1.SetColumnSpan(this.m_labelIntro, 2);
            this.m_labelIntro.Name = "m_labelIntro";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // m_comboBoxConnection
            // 
            resources.ApplyResources(this.m_comboBoxConnection, "m_comboBoxConnection");
            this.m_comboBoxConnection.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.m_comboBoxConnection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboBoxConnection.FormattingEnabled = true;
            this.m_comboBoxConnection.Name = "m_comboBoxConnection";
            this.m_comboBoxConnection.SelectedIndexChanged += new System.EventHandler(this.m_comboBoxConnection_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.Name = "label2";
            // 
            // m_dataGridView
            // 
            this.m_dataGridView.AllowUserToAddRows = false;
            this.m_dataGridView.AllowUserToDeleteRows = false;
            this.m_dataGridView.AllowUserToResizeColumns = false;
            this.m_dataGridView.AllowUserToResizeRows = false;
            this.m_dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.m_dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.m_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.m_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.m_colVmName,
            this.m_colTarget});
            this.tableLayoutPanel1.SetColumnSpan(this.m_dataGridView, 2);
            resources.ApplyResources(this.m_dataGridView, "m_dataGridView");
            this.m_dataGridView.Name = "m_dataGridView";
            this.m_dataGridView.RowHeadersVisible = false;
            this.m_dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_dataGridView_CellClick);
            this.m_dataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.m_dataGridView_CurrentCellDirtyStateChanged);
            // 
            // m_colVmName
            // 
            resources.ApplyResources(this.m_colVmName, "m_colVmName");
            this.m_colVmName.Name = "m_colVmName";
            this.m_colVmName.ReadOnly = true;
            // 
            // m_colTarget
            // 
            this.m_colTarget.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.m_colTarget, "m_colTarget");
            this.m_colTarget.Name = "m_colTarget";
            this.m_colTarget.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // tableLayoutPanelWarning
            // 
            resources.ApplyResources(this.tableLayoutPanelWarning, "tableLayoutPanelWarning");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanelWarning, 2);
            this.tableLayoutPanelWarning.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanelWarning.Controls.Add(this.labelWarning, 1, 0);
            this.tableLayoutPanelWarning.Name = "tableLayoutPanelWarning";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // labelWarning
            // 
            resources.ApplyResources(this.labelWarning, "labelWarning");
            this.labelWarning.Name = "labelWarning";
            // 
            // SelectMultipleVMDestinationPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SelectMultipleVMDestinationPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).EndInit();
            this.tableLayoutPanelWarning.ResumeLayout(false);
            this.tableLayoutPanelWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private XenAdmin.Controls.Common.AutoHeightLabel m_labelIntro;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView m_dataGridView;
		private System.Windows.Forms.Label label1;
        private XenAdmin.Controls.EnableableComboBox m_comboBoxConnection;
        private System.Windows.Forms.DataGridViewTextBoxColumn m_colVmName;
        private  XenAdmin.Controls.EnableableComboBoxColumn m_colTarget;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelWarning;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelWarning;
    }
}
