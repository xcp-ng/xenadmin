namespace XenAdmin.Dialogs.Network
{
    partial class CertificateChangedDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CertificateChangedDialog));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BlurbLabel = new System.Windows.Forms.Label();
            this.AlwaysIgnoreCheckBox = new System.Windows.Forms.CheckBox();
            this.ViewCertificateButton = new System.Windows.Forms.Button();
            this.Blurb2Label = new System.Windows.Forms.Label();
            this.Blurb3Label = new System.Windows.Forms.Label();
            this.labelTrusted = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.Okbutton = new System.Windows.Forms.Button();
            this.Cancelbutton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
            this.pictureBox1.Name = "pictureBox1";
            this.tableLayoutPanel1.SetRowSpan(this.pictureBox1, 3);
            this.pictureBox1.TabStop = false;
            // 
            // BlurbLabel
            // 
            resources.ApplyResources(this.BlurbLabel, "BlurbLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.BlurbLabel, 2);
            this.BlurbLabel.Name = "BlurbLabel";
            // 
            // AlwaysIgnoreCheckBox
            // 
            resources.ApplyResources(this.AlwaysIgnoreCheckBox, "AlwaysIgnoreCheckBox");
            this.tableLayoutPanel1.SetColumnSpan(this.AlwaysIgnoreCheckBox, 2);
            this.AlwaysIgnoreCheckBox.Name = "AlwaysIgnoreCheckBox";
            this.AlwaysIgnoreCheckBox.UseVisualStyleBackColor = true;
            // 
            // ViewCertificateButton
            // 
            resources.ApplyResources(this.ViewCertificateButton, "ViewCertificateButton");
            this.ViewCertificateButton.Name = "ViewCertificateButton";
            this.ViewCertificateButton.UseVisualStyleBackColor = true;
            this.ViewCertificateButton.Click += new System.EventHandler(this.ViewCertificateButton_Click);
            // 
            // Blurb2Label
            // 
            resources.ApplyResources(this.Blurb2Label, "Blurb2Label");
            this.tableLayoutPanel1.SetColumnSpan(this.Blurb2Label, 2);
            this.Blurb2Label.Name = "Blurb2Label";
            // 
            // Blurb3Label
            // 
            resources.ApplyResources(this.Blurb3Label, "Blurb3Label");
            this.Blurb3Label.Name = "Blurb3Label";
            // 
            // labelTrusted
            // 
            resources.ApplyResources(this.labelTrusted, "labelTrusted");
            this.tableLayoutPanel1.SetColumnSpan(this.labelTrusted, 2);
            this.labelTrusted.Name = "labelTrusted";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.BlurbLabel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.AlwaysIgnoreCheckBox, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelTrusted, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.Blurb2Label, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.ViewCertificateButton, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.Blurb3Label, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 3);
            this.tableLayoutPanel2.Controls.Add(this.Okbutton, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.Cancelbutton, 2, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // Okbutton
            // 
            this.Okbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.Okbutton, "Okbutton");
            this.Okbutton.Name = "Okbutton";
            this.Okbutton.UseVisualStyleBackColor = true;
            this.Okbutton.Click += new System.EventHandler(this.Okbutton_Click);
            // 
            // Cancelbutton
            // 
            this.Cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.Cancelbutton, "Cancelbutton");
            this.Cancelbutton.Name = "Cancelbutton";
            this.Cancelbutton.UseVisualStyleBackColor = true;
            // 
            // CertificateChangedDialog
            // 
            this.AcceptButton = this.Okbutton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.Cancelbutton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CertificateChangedDialog";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label BlurbLabel;
        private System.Windows.Forms.CheckBox AlwaysIgnoreCheckBox;
        private System.Windows.Forms.Button ViewCertificateButton;
        private System.Windows.Forms.Label Blurb2Label;
        private System.Windows.Forms.Label Blurb3Label;
        private System.Windows.Forms.Label labelTrusted;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button Okbutton;
        private System.Windows.Forms.Button Cancelbutton;
    }
}