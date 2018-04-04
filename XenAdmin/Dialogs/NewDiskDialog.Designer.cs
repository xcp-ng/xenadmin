namespace XenAdmin.Dialogs
{
    partial class NewDiskDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewDiskDialog));
            this.DiskSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.SrListBox = new XenAdmin.Controls.SrPicker();
            this.GbLabel = new System.Windows.Forms.Label();
            this.CloseButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.DescriptionTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.comboBoxUnits = new System.Windows.Forms.ComboBox();
            this.labelError = new System.Windows.Forms.Label();
            this.pictureBoxError = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.DiskSizeNumericUpDown)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).BeginInit();
            this.SuspendLayout();
            // 
            // DiskSizeNumericUpDown
            // 
            resources.ApplyResources(this.DiskSizeNumericUpDown, "DiskSizeNumericUpDown");
            this.DiskSizeNumericUpDown.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.DiskSizeNumericUpDown.Name = "DiskSizeNumericUpDown";
            this.DiskSizeNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DiskSizeNumericUpDown.ValueChanged += new System.EventHandler(this.DiskSizeNumericUpDown_ValueChanged);
            this.DiskSizeNumericUpDown.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DiskSizeNumericUpDown_KeyUp);
            // 
            // SrListBox
            // 
            resources.ApplyResources(this.SrListBox, "SrListBox");
            this.tableLayoutPanel1.SetColumnSpan(this.SrListBox, 3);
            this.SrListBox.Connection = null;
            this.SrListBox.Name = "SrListBox";
            // 
            // GbLabel
            // 
            resources.ApplyResources(this.GbLabel, "GbLabel");
            this.GbLabel.Name = "GbLabel";
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // OkButton
            // 
            resources.ApplyResources(this.OkButton, "OkButton");
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OkButton.Name = "OkButton";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // NameTextBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.NameTextBox, 3);
            resources.ApplyResources(this.NameTextBox, "NameTextBox");
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.TextChanged += new System.EventHandler(this.NameTextBox_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // DescriptionTextBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.DescriptionTextBox, 3);
            resources.ApplyResources(this.DescriptionTextBox, "DescriptionTextBox");
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.TextChanged += new System.EventHandler(this.NameTextBox_TextChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.NameTextBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.DescriptionTextBox, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.SrListBox, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.DiskSizeNumericUpDown, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 9);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 3);
            this.panel1.Controls.Add(this.CloseButton);
            this.panel1.Controls.Add(this.OkButton);
            this.panel1.Name = "panel1";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.tableLayoutPanel1.SetColumnSpan(this.panel2, 2);
            this.panel2.Controls.Add(this.comboBoxUnits);
            this.panel2.Controls.Add(this.labelError);
            this.panel2.Controls.Add(this.pictureBoxError);
            this.panel2.Name = "panel2";
            // 
            // comboBoxUnits
            // 
            this.comboBoxUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxUnits, "comboBoxUnits");
            this.comboBoxUnits.FormattingEnabled = true;
            this.comboBoxUnits.Items.AddRange(new object[] {
            resources.GetString("comboBoxUnits.Items"),
            resources.GetString("comboBoxUnits.Items1")});
            this.comboBoxUnits.Name = "comboBoxUnits";
            // 
            // labelError
            // 
            resources.ApplyResources(this.labelError, "labelError");
            this.labelError.ForeColor = System.Drawing.Color.Red;
            this.labelError.Name = "labelError";
            // 
            // pictureBoxError
            // 
            resources.ApplyResources(this.pictureBoxError, "pictureBoxError");
            this.pictureBoxError.Name = "pictureBoxError";
            this.pictureBoxError.TabStop = false;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.tableLayoutPanel1.SetColumnSpan(this.label6, 4);
            this.label6.Name = "label6";
            // 
            // NewDiskDialog
            // 
            this.AcceptButton = this.OkButton;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.CloseButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.GbLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "NewDiskDialog";
            ((System.ComponentModel.ISupportInitialize)(this.DiskSizeNumericUpDown)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label GbLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.NumericUpDown DiskSizeNumericUpDown;
        public XenAdmin.Controls.SrPicker SrListBox;
        public System.Windows.Forms.Button CloseButton;
        public System.Windows.Forms.Button OkButton;
        public System.Windows.Forms.TextBox NameTextBox;
        public System.Windows.Forms.TextBox DescriptionTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelError;
        private System.Windows.Forms.PictureBox pictureBoxError;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxUnits;

    }
}
