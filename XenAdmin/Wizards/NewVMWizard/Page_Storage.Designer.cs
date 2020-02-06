namespace XenAdmin.Wizards.NewVMWizard
{
    partial class Page_Storage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Page_Storage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.CloneCheckBox = new System.Windows.Forms.CheckBox();
            this.DisksGridView = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.ImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SrColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SharedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.EditButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.DisklessVMRadioButton = new System.Windows.Forms.RadioButton();
            this.DisksRadioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.DisksGridView)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CloneCheckBox
            // 
            resources.ApplyResources(this.CloneCheckBox, "CloneCheckBox");
            this.tableLayoutPanel1.SetColumnSpan(this.CloneCheckBox, 2);
            this.CloneCheckBox.Name = "CloneCheckBox";
            this.CloneCheckBox.UseVisualStyleBackColor = true;
            // 
            // DisksGridView
            // 
            this.DisksGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.DisksGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.DisksGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.DisksGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ImageColumn,
            this.NameColumn,
            this.SrColumn,
            this.SizeColumn,
            this.SharedColumn});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DisksGridView.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.DisksGridView, "DisksGridView");
            this.DisksGridView.Name = "DisksGridView";
            this.tableLayoutPanel1.SetRowSpan(this.DisksGridView, 3);
            this.DisksGridView.SelectionChanged += new System.EventHandler(this.DisksGridView_SelectionChanged);
            // 
            // ImageColumn
            // 
            this.ImageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ImageColumn, "ImageColumn");
            this.ImageColumn.Name = "ImageColumn";
            this.ImageColumn.ReadOnly = true;
            // 
            // NameColumn
            // 
            this.NameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NameColumn.FillWeight = 40F;
            resources.ApplyResources(this.NameColumn, "NameColumn");
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.ReadOnly = true;
            // 
            // SrColumn
            // 
            this.SrColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SrColumn.FillWeight = 60F;
            resources.ApplyResources(this.SrColumn, "SrColumn");
            this.SrColumn.Name = "SrColumn";
            this.SrColumn.ReadOnly = true;
            // 
            // SizeColumn
            // 
            this.SizeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.SizeColumn, "SizeColumn");
            this.SizeColumn.Name = "SizeColumn";
            this.SizeColumn.ReadOnly = true;
            // 
            // SharedColumn
            // 
            this.SharedColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.SharedColumn, "SharedColumn");
            this.SharedColumn.Name = "SharedColumn";
            this.SharedColumn.ReadOnly = true;
            // 
            // DeleteButton
            // 
            resources.ApplyResources(this.DeleteButton, "DeleteButton");
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // EditButton
            // 
            resources.ApplyResources(this.EditButton, "EditButton");
            this.EditButton.Name = "EditButton";
            this.EditButton.UseVisualStyleBackColor = true;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // AddButton
            // 
            resources.ApplyResources(this.AddButton, "AddButton");
            this.AddButton.Name = "AddButton";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // DisklessVMRadioButton
            // 
            resources.ApplyResources(this.DisklessVMRadioButton, "DisklessVMRadioButton");
            this.tableLayoutPanel1.SetColumnSpan(this.DisklessVMRadioButton, 3);
            this.DisklessVMRadioButton.Name = "DisklessVMRadioButton";
            this.DisklessVMRadioButton.TabStop = true;
            this.DisklessVMRadioButton.UseVisualStyleBackColor = true;
            this.DisklessVMRadioButton.CheckedChanged += new System.EventHandler(this.DisklessVMRadioButton_CheckedChanged);
            // 
            // DisksRadioButton
            // 
            resources.ApplyResources(this.DisksRadioButton, "DisksRadioButton");
            this.tableLayoutPanel1.SetColumnSpan(this.DisksRadioButton, 3);
            this.DisksRadioButton.Name = "DisksRadioButton";
            this.DisksRadioButton.TabStop = true;
            this.DisksRadioButton.UseVisualStyleBackColor = true;
            this.DisksRadioButton.CheckedChanged += new System.EventHandler(this.DisksRadioButton_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
            this.label1.Name = "label1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.CloneCheckBox, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.DeleteButton, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.DisksGridView, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.EditButton, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.DisksRadioButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.AddButton, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.DisklessVMRadioButton, 0, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // Page_Storage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Page_Storage";
            ((System.ComponentModel.ISupportInitialize)(this.DisksGridView)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton DisksRadioButton;
        private System.Windows.Forms.RadioButton DisklessVMRadioButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button EditButton;
        private System.Windows.Forms.Button DeleteButton;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx DisksGridView;
        private System.Windows.Forms.CheckBox CloneCheckBox;
        private System.Windows.Forms.DataGridViewImageColumn ImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SrColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SizeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SharedColumn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
