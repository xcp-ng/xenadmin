namespace XenAdmin.Dialogs
{
    partial class ConversionVmSelectionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConversionVmSelectionDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewEx1 = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.columnImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.columnVm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnUuid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEx1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewEx1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.autoHeightLabel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonOk, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // dataGridViewEx1
            // 
            this.dataGridViewEx1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewEx1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewEx1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewEx1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnImage,
            this.columnVm,
            this.columnUuid});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridViewEx1, 3);
            resources.ApplyResources(this.dataGridViewEx1, "dataGridViewEx1");
            this.dataGridViewEx1.Name = "dataGridViewEx1";
            this.dataGridViewEx1.ReadOnly = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewEx1.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewEx1.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewEx1_CellMouseDoubleClick);
            this.dataGridViewEx1.SelectionChanged += new System.EventHandler(this.dataGridViewEx1_SelectionChanged);
            // 
            // columnImage
            // 
            this.columnImage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            resources.ApplyResources(this.columnImage, "columnImage");
            this.columnImage.Name = "columnImage";
            this.columnImage.ReadOnly = true;
            // 
            // columnVm
            // 
            resources.ApplyResources(this.columnVm, "columnVm");
            this.columnVm.Name = "columnVm";
            this.columnVm.ReadOnly = true;
            // 
            // columnUuid
            // 
            resources.ApplyResources(this.columnUuid, "columnUuid");
            this.columnUuid.Name = "columnUuid";
            this.columnUuid.ReadOnly = true;
            // 
            // autoHeightLabel1
            // 
            resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
            this.tableLayoutPanel1.SetColumnSpan(this.autoHeightLabel1, 3);
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // ConversionVmSelectionDialog
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ConversionVmSelectionDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEx1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Controls.DataGridViewEx.DataGridViewEx dataGridViewEx1;
        private System.Windows.Forms.DataGridViewImageColumn columnImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnVm;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnUuid;
        private Controls.Common.AutoHeightLabel autoHeightLabel1;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
    }
}

