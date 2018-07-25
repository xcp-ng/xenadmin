using XenAdmin.Controls.DataGridViewEx;

namespace XenAdmin.Wizards.PatchingWizard
{
    partial class PatchingWizard_PrecheckPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatchingWizard_PrecheckPage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.labelPrechecksFirstLine = new System.Windows.Forms.Label();
            this.dataGridView1 = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ColumnState = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSolution = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonReCheckProblems = new System.Windows.Forms.Button();
            this.buttonResolveAll = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.labelIssues = new System.Windows.Forms.Label();
            this.pictureBoxIssues = new System.Windows.Forms.PictureBox();
            this.checkBoxViewPrecheckFailuresOnly = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelProgress = new System.Windows.Forms.Label();
            this._worker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIssues)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelPrechecksFirstLine
            // 
            resources.ApplyResources(this.labelPrechecksFirstLine, "labelPrechecksFirstLine");
            this.labelPrechecksFirstLine.AutoEllipsis = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelPrechecksFirstLine, 4);
            this.labelPrechecksFirstLine.Name = "labelPrechecksFirstLine";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToResizeColumns = false;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ColumnHeadersVisible = false;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnState,
            this.ColumnDescription,
            this.ColumnSolution});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridView1, 4);
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.HideSelection = true;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseMove);
            this.dataGridView1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dataGridView1_KeyPress);
            // 
            // ColumnState
            // 
            this.ColumnState.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.NullValue = "System.Drawing.Bitmap";
            this.ColumnState.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.ColumnState, "ColumnState");
            this.ColumnState.Name = "ColumnState";
            this.ColumnState.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnState.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ColumnDescription
            // 
            this.ColumnDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnDescription.FillWeight = 137.8531F;
            resources.ApplyResources(this.ColumnDescription, "ColumnDescription");
            this.ColumnDescription.Name = "ColumnDescription";
            // 
            // ColumnSolution
            // 
            this.ColumnSolution.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnSolution.FillWeight = 40F;
            resources.ApplyResources(this.ColumnSolution, "ColumnSolution");
            this.ColumnSolution.Name = "ColumnSolution";
            // 
            // buttonReCheckProblems
            // 
            resources.ApplyResources(this.buttonReCheckProblems, "buttonReCheckProblems");
            this.buttonReCheckProblems.Name = "buttonReCheckProblems";
            this.buttonReCheckProblems.UseVisualStyleBackColor = true;
            this.buttonReCheckProblems.Click += new System.EventHandler(this.buttonReCheckProblems_Click);
            // 
            // buttonResolveAll
            // 
            resources.ApplyResources(this.buttonResolveAll, "buttonResolveAll");
            this.buttonResolveAll.Name = "buttonResolveAll";
            this.buttonResolveAll.UseVisualStyleBackColor = true;
            this.buttonResolveAll.Click += new System.EventHandler(this.buttonResolveAll_Click);
            // 
            // progressBar1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.progressBar1, 4);
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // labelIssues
            // 
            this.labelIssues.AutoEllipsis = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelIssues, 3);
            resources.ApplyResources(this.labelIssues, "labelIssues");
            this.labelIssues.Name = "labelIssues";
            // 
            // pictureBoxIssues
            // 
            this.pictureBoxIssues.Image = global::XenAdmin.Properties.Resources._000_Abort_h32bit_16;
            resources.ApplyResources(this.pictureBoxIssues, "pictureBoxIssues");
            this.pictureBoxIssues.Name = "pictureBoxIssues";
            this.pictureBoxIssues.TabStop = false;
            // 
            // checkBoxViewPrecheckFailuresOnly
            // 
            resources.ApplyResources(this.checkBoxViewPrecheckFailuresOnly, "checkBoxViewPrecheckFailuresOnly");
            this.checkBoxViewPrecheckFailuresOnly.Checked = true;
            this.checkBoxViewPrecheckFailuresOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxViewPrecheckFailuresOnly, 2);
            this.checkBoxViewPrecheckFailuresOnly.Name = "checkBoxViewPrecheckFailuresOnly";
            this.checkBoxViewPrecheckFailuresOnly.UseVisualStyleBackColor = true;
            this.checkBoxViewPrecheckFailuresOnly.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelPrechecksFirstLine, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelProgress, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonResolveAll, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxViewPrecheckFailuresOnly, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonReCheckProblems, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxIssues, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelIssues, 1, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelProgress
            // 
            this.labelProgress.AutoEllipsis = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelProgress, 4);
            resources.ApplyResources(this.labelProgress, "labelProgress");
            this.labelProgress.Name = "labelProgress";
            // 
            // _worker
            // 
            this._worker.WorkerReportsProgress = true;
            this._worker.WorkerSupportsCancellation = true;
            this._worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            this._worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            this._worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._worker_RunWorkerCompleted);
            // 
            // PatchingWizard_PrecheckPage
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this, "$this");
            this.Name = "PatchingWizard_PrecheckPage";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIssues)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.Label labelPrechecksFirstLine;
        private DataGridViewEx dataGridView1;
        private System.Windows.Forms.Button buttonReCheckProblems;
        private System.Windows.Forms.Button buttonResolveAll;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label labelIssues;
        private System.Windows.Forms.PictureBox pictureBoxIssues;
        private System.Windows.Forms.CheckBox checkBoxViewPrecheckFailuresOnly;
        private System.Windows.Forms.DataGridViewImageColumn ColumnState;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSolution;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelProgress;
        private System.ComponentModel.BackgroundWorker _worker;

    }
}
