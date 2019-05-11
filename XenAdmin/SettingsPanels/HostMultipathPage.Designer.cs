namespace XenAdmin.SettingsPanels
{
    partial class HostMultipathPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HostMultipathPage));
            this.multipathCheckBox = new System.Windows.Forms.CheckBox();
            this.maintenanceWarningImage = new System.Windows.Forms.PictureBox();
            this.maintenanceWarningLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.maintenanceWarningImage)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // multipathCheckBox
            // 
            resources.ApplyResources(this.multipathCheckBox, "multipathCheckBox");
            this.tableLayoutPanel1.SetColumnSpan(this.multipathCheckBox, 2);
            this.multipathCheckBox.Name = "multipathCheckBox";
            this.multipathCheckBox.UseVisualStyleBackColor = true;
            this.multipathCheckBox.CheckedChanged += new System.EventHandler(this.multipathCheckBox_CheckedChanged);
            // 
            // maintenanceWarningImage
            // 
            this.maintenanceWarningImage.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.maintenanceWarningImage, "maintenanceWarningImage");
            this.maintenanceWarningImage.Name = "maintenanceWarningImage";
            this.maintenanceWarningImage.TabStop = false;
            // 
            // maintenanceWarningLabel
            // 
            resources.ApplyResources(this.maintenanceWarningLabel, "maintenanceWarningLabel");
            this.maintenanceWarningLabel.Name = "maintenanceWarningLabel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.maintenanceWarningImage, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.maintenanceWarningLabel, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.multipathCheckBox, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // HostMultipathPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "HostMultipathPage";
            ((System.ComponentModel.ISupportInitialize)(this.maintenanceWarningImage)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox multipathCheckBox;
        private System.Windows.Forms.PictureBox maintenanceWarningImage;
        private System.Windows.Forms.Label maintenanceWarningLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
