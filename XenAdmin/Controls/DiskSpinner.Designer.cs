namespace XenAdmin.Controls
{
    partial class DiskSpinner
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
                DiskSizeNumericUpDown.TextChanged -= DiskSizeNumericUpDown_TextChanged;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiskSpinner));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelError = new System.Windows.Forms.TableLayoutPanel();
            this.labelError = new System.Windows.Forms.Label();
            this.pictureBoxError = new System.Windows.Forms.PictureBox();
            this.comboBoxUnits = new System.Windows.Forms.ComboBox();
            this.DiskSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelError.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiskSizeNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelError, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxUnits, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.DiskSizeNumericUpDown, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanelError
            // 
            resources.ApplyResources(this.tableLayoutPanelError, "tableLayoutPanelError");
            this.tableLayoutPanelError.Controls.Add(this.labelError, 1, 0);
            this.tableLayoutPanelError.Controls.Add(this.pictureBoxError, 0, 0);
            this.tableLayoutPanelError.Name = "tableLayoutPanelError";
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
            // comboBoxUnits
            // 
            this.comboBoxUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxUnits, "comboBoxUnits");
            this.comboBoxUnits.FormattingEnabled = true;
            this.comboBoxUnits.Name = "comboBoxUnits";
            this.comboBoxUnits.SelectedIndexChanged += new System.EventHandler(this.comboBoxUnits_SelectedIndexChanged);
            // 
            // DiskSizeNumericUpDown
            // 
            resources.ApplyResources(this.DiskSizeNumericUpDown, "DiskSizeNumericUpDown");
            this.DiskSizeNumericUpDown.Name = "DiskSizeNumericUpDown";
            this.DiskSizeNumericUpDown.ValueChanged += new System.EventHandler(this.DiskSizeNumericUpDown_ValueChanged);
            this.DiskSizeNumericUpDown.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DiskSizeNumericUpDown_KeyUp);
            // 
            // DiskSpinner
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DiskSpinner";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanelError.ResumeLayout(false);
            this.tableLayoutPanelError.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiskSizeNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.NumericUpDown DiskSizeNumericUpDown;
        private System.Windows.Forms.ComboBox comboBoxUnits;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelError;
        private System.Windows.Forms.Label labelError;
        private System.Windows.Forms.PictureBox pictureBoxError;
    }
}
