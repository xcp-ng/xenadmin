namespace XenAdmin.Wizards.ConversionWizard
{
    partial class VmSelectionPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VmSelectionPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewVms = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnChecked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnVm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnOs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDiskSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.buttonClearAll = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.tableLayoutPanelError = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxError = new System.Windows.Forms.PictureBox();
            this.labelError = new System.Windows.Forms.Label();
            this._backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanelError.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewVms, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelError, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // dataGridViewVms
            // 
            this.dataGridViewVms.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewVms.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewVms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewVms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnChecked,
            this.columnVm,
            this.columnOs,
            this.columnDiskSize});
            resources.ApplyResources(this.dataGridViewVms, "dataGridViewVms");
            this.dataGridViewVms.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.dataGridViewVms.MultiSelect = true;
            this.dataGridViewVms.Name = "dataGridViewVms";
            this.dataGridViewVms.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewVms_CellValueChanged);
            this.dataGridViewVms.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridViewVms_CurrentCellDirtyStateChanged);
            // 
            // columnChecked
            // 
            this.columnChecked.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.columnChecked, "columnChecked");
            this.columnChecked.Name = "columnChecked";
            // 
            // columnVm
            // 
            resources.ApplyResources(this.columnVm, "columnVm");
            this.columnVm.Name = "columnVm";
            this.columnVm.ReadOnly = true;
            // 
            // columnOs
            // 
            resources.ApplyResources(this.columnOs, "columnOs");
            this.columnOs.Name = "columnOs";
            this.columnOs.ReadOnly = true;
            // 
            // columnDiskSize
            // 
            this.columnDiskSize.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.columnDiskSize.FillWeight = 40F;
            resources.ApplyResources(this.columnDiskSize, "columnDiskSize");
            this.columnDiskSize.Name = "columnDiskSize";
            this.columnDiskSize.ReadOnly = true;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.buttonSelectAll, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.buttonClearAll, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.buttonRefresh, 3, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // buttonSelectAll
            // 
            resources.ApplyResources(this.buttonSelectAll, "buttonSelectAll");
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            this.buttonSelectAll.Click += new System.EventHandler(this.buttonSelectAll_Click);
            // 
            // buttonClearAll
            // 
            resources.ApplyResources(this.buttonClearAll, "buttonClearAll");
            this.buttonClearAll.Name = "buttonClearAll";
            this.buttonClearAll.UseVisualStyleBackColor = true;
            this.buttonClearAll.Click += new System.EventHandler(this.buttonClearAll_Click);
            // 
            // buttonRefresh
            // 
            resources.ApplyResources(this.buttonRefresh, "buttonRefresh");
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // tableLayoutPanelError
            // 
            resources.ApplyResources(this.tableLayoutPanelError, "tableLayoutPanelError");
            this.tableLayoutPanelError.Controls.Add(this.pictureBoxError, 0, 0);
            this.tableLayoutPanelError.Controls.Add(this.labelError, 1, 0);
            this.tableLayoutPanelError.Name = "tableLayoutPanelError";
            // 
            // pictureBoxError
            // 
            resources.ApplyResources(this.pictureBoxError, "pictureBoxError");
            this.pictureBoxError.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            this.pictureBoxError.Name = "pictureBoxError";
            this.pictureBoxError.TabStop = false;
            // 
            // labelError
            // 
            resources.ApplyResources(this.labelError, "labelError");
            this.labelError.Name = "labelError";
            // 
            // _backgroundWorker
            // 
            this._backgroundWorker.WorkerSupportsCancellation = true;
            this._backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._backgroundWorker_DoWork);
            this._backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._backgroundWorker_RunWorkerCompleted);
            // 
            // VmSelectionPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "VmSelectionPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanelError.ResumeLayout(false);
            this.tableLayoutPanelError.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelError;
        private System.Windows.Forms.PictureBox pictureBoxError;
        private System.Windows.Forms.Label labelError;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button buttonSelectAll;
        private System.Windows.Forms.Button buttonClearAll;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Label label1;
        private Controls.DataGridViewEx.DataGridViewEx dataGridViewVms;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnChecked;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnVm;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnOs;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDiskSize;
        private System.ComponentModel.BackgroundWorker _backgroundWorker;
    }
}
