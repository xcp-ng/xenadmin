namespace XenAdmin.Wizards.GenericPages
{
    partial class SelectMultipleVMNetworkPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectMultipleVMNetworkPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_labelIntro = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.m_dataGridView = new System.Windows.Forms.DataGridView();
            this.m_colVmNetwork = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_colTargetNet = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.m_buttonRefresh = new System.Windows.Forms.Button();
            this.tableLayoutPanelError = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxError = new System.Windows.Forms.PictureBox();
            this.labelError = new System.Windows.Forms.Label();
            this.m_checkBoxMac = new System.Windows.Forms.CheckBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).BeginInit();
            this.tableLayoutPanelError.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.m_labelIntro, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.m_dataGridView, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_buttonRefresh, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelError, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.m_checkBoxMac, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // m_labelIntro
            // 
            resources.ApplyResources(this.m_labelIntro, "m_labelIntro");
            this.tableLayoutPanel1.SetColumnSpan(this.m_labelIntro, 2);
            this.m_labelIntro.Name = "m_labelIntro";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.m_dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.m_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.m_colVmNetwork,
            this.m_colTargetNet});
            this.tableLayoutPanel1.SetColumnSpan(this.m_dataGridView, 2);
            resources.ApplyResources(this.m_dataGridView, "m_dataGridView");
            this.m_dataGridView.Name = "m_dataGridView";
            this.m_dataGridView.RowHeadersVisible = false;
            this.m_dataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_dataGridView_CellEnter);
            this.m_dataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.m_dataGridView_CurrentCellDirtyStateChanged);
            // 
            // m_colVmNetwork
            // 
            resources.ApplyResources(this.m_colVmNetwork, "m_colVmNetwork");
            this.m_colVmNetwork.Name = "m_colVmNetwork";
            this.m_colVmNetwork.ReadOnly = true;
            // 
            // m_colTargetNet
            // 
            this.m_colTargetNet.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.m_colTargetNet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.m_colTargetNet, "m_colTargetNet");
            this.m_colTargetNet.Name = "m_colTargetNet";
            // 
            // m_buttonRefresh
            // 
            resources.ApplyResources(this.m_buttonRefresh, "m_buttonRefresh");
            this.m_buttonRefresh.Name = "m_buttonRefresh";
            this.m_buttonRefresh.UseVisualStyleBackColor = true;
            this.m_buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // tableLayoutPanelError
            // 
            resources.ApplyResources(this.tableLayoutPanelError, "tableLayoutPanelError");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanelError, 2);
            this.tableLayoutPanelError.Controls.Add(this.pictureBoxError, 0, 0);
            this.tableLayoutPanelError.Controls.Add(this.labelError, 1, 0);
            this.tableLayoutPanelError.Name = "tableLayoutPanelError";
            // 
            // pictureBoxError
            // 
            resources.ApplyResources(this.pictureBoxError, "pictureBoxError");
            this.pictureBoxError.Name = "pictureBoxError";
            this.pictureBoxError.TabStop = false;
            // 
            // labelError
            // 
            resources.ApplyResources(this.labelError, "labelError");
            this.labelError.Name = "labelError";
            // 
            // m_checkBoxMac
            // 
            resources.ApplyResources(this.m_checkBoxMac, "m_checkBoxMac");
            this.m_checkBoxMac.Name = "m_checkBoxMac";
            this.m_checkBoxMac.UseVisualStyleBackColor = true;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // SelectMultipleVMNetworkPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SelectMultipleVMNetworkPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).EndInit();
            this.tableLayoutPanelError.ResumeLayout(false);
            this.tableLayoutPanelError.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.Common.AutoHeightLabel m_labelIntro;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView m_dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn m_colVmNetwork;
        private System.Windows.Forms.DataGridViewComboBoxColumn m_colTargetNet;
        private System.Windows.Forms.CheckBox m_checkBoxMac;
        private System.Windows.Forms.Button m_buttonRefresh;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelError;
        private System.Windows.Forms.PictureBox pictureBoxError;
        private System.Windows.Forms.Label labelError;
    }
}
