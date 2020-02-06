namespace XenAdmin.Dialogs
{
    partial class ExitMaintenanceModeDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExitMaintenanceModeDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanelBody = new System.Windows.Forms.TableLayoutPanel();
            this.labelBlurb = new System.Windows.Forms.Label();
            this.dataGridViewVms = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNo = new System.Windows.Forms.Button();
            this.buttonYes = new System.Windows.Forms.Button();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPicture = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnVm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnCurrentLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanelBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelBody
            // 
            resources.ApplyResources(this.tableLayoutPanelBody, "tableLayoutPanelBody");
            this.tableLayoutPanelBody.Controls.Add(this.labelBlurb, 1, 0);
            this.tableLayoutPanelBody.Controls.Add(this.dataGridViewVms, 1, 1);
            this.tableLayoutPanelBody.Controls.Add(this.flowLayoutPanel1, 1, 2);
            this.tableLayoutPanelBody.Name = "tableLayoutPanelBody";
            // 
            // labelBlurb
            // 
            resources.ApplyResources(this.labelBlurb, "labelBlurb");
            this.labelBlurb.Name = "labelBlurb";
            // 
            // dataGridViewVms
            // 
            this.dataGridViewVms.AllowUserToResizeColumns = false;
            this.dataGridViewVms.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewVms.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewVms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewVms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnPicture,
            this.ColumnVm,
            this.ColumnCurrentLocation});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewVms.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.dataGridViewVms, "dataGridViewVms");
            this.dataGridViewVms.HideSelection = true;
            this.dataGridViewVms.Name = "dataGridViewVms";
            this.dataGridViewVms.ReadOnly = true;
            this.dataGridViewVms.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.buttonCancel);
            this.flowLayoutPanel1.Controls.Add(this.buttonNo);
            this.flowLayoutPanel1.Controls.Add(this.buttonYes);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonNo
            // 
            resources.ApplyResources(this.buttonNo, "buttonNo");
            this.buttonNo.Name = "buttonNo";
            this.buttonNo.UseVisualStyleBackColor = true;
            this.buttonNo.Click += new System.EventHandler(this.buttonNo_Click);
            // 
            // buttonYes
            // 
            resources.ApplyResources(this.buttonYes, "buttonYes");
            this.buttonYes.Name = "buttonYes";
            this.buttonYes.UseVisualStyleBackColor = true;
            this.buttonYes.Click += new System.EventHandler(this.buttonYes_Click);
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewImageColumn1.HeaderText = global::XenAdmin.Messages.SOLUTION_UNKNOWN;
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.ReadOnly = true;
            this.dataGridViewImageColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            resources.ApplyResources(this.dataGridViewImageColumn1, "dataGridViewImageColumn1");
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.dataGridViewTextBoxColumn2, "dataGridViewTextBoxColumn2");
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColumnPicture
            // 
            this.ColumnPicture.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ColumnPicture.HeaderText = global::XenAdmin.Messages.SOLUTION_UNKNOWN;
            this.ColumnPicture.Name = "ColumnPicture";
            this.ColumnPicture.ReadOnly = true;
            this.ColumnPicture.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            resources.ApplyResources(this.ColumnPicture, "ColumnPicture");
            // 
            // ColumnVm
            // 
            this.ColumnVm.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ColumnVm, "ColumnVm");
            this.ColumnVm.Name = "ColumnVm";
            this.ColumnVm.ReadOnly = true;
            this.ColumnVm.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColumnCurrentLocation
            // 
            this.ColumnCurrentLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.ColumnCurrentLocation, "ColumnCurrentLocation");
            this.ColumnCurrentLocation.Name = "ColumnCurrentLocation";
            this.ColumnCurrentLocation.ReadOnly = true;
            this.ColumnCurrentLocation.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // RestoreVMsDialog
            // 
            this.AcceptButton = this.buttonYes;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tableLayoutPanelBody);
            this.Name = "RestoreVMsDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RestoreVMsDialog_FormClosing);
            this.tableLayoutPanelBody.ResumeLayout(false);
            this.tableLayoutPanelBody.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBody;
        private System.Windows.Forms.Label labelBlurb;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewVms;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button buttonNo;
        private System.Windows.Forms.Button buttonYes;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewImageColumn ColumnPicture;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnVm;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCurrentLocation;
    }
}