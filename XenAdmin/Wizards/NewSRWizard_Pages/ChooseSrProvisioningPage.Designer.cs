namespace XenAdmin.Wizards.NewSRWizard_Pages
{
    partial class ChooseSrProvisioningPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseSrProvisioningPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonGfs2 = new System.Windows.Forms.RadioButton();
            this.labelGFS2 = new System.Windows.Forms.Label();
            this.radioButtonLvm = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutInfo = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxInfo = new System.Windows.Forms.PictureBox();
            this.labelInfo = new System.Windows.Forms.Label();
            this.linkLabelPoolProperties = new System.Windows.Forms.LinkLabel();
            this.tableLayoutWarning = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxWarning = new System.Windows.Forms.PictureBox();
            this.labelWarning = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo)).BeginInit();
            this.tableLayoutWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarning)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonGfs2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelGFS2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonLvm, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutInfo, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutWarning, 0, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // radioButtonGfs2
            // 
            resources.ApplyResources(this.radioButtonGfs2, "radioButtonGfs2");
            this.radioButtonGfs2.Checked = true;
            this.radioButtonGfs2.Name = "radioButtonGfs2";
            this.radioButtonGfs2.TabStop = true;
            this.radioButtonGfs2.UseVisualStyleBackColor = true;
            this.radioButtonGfs2.CheckedChanged += new System.EventHandler(this.radioButtonGfs2_CheckedChanged);
            // 
            // labelGFS2
            // 
            resources.ApplyResources(this.labelGFS2, "labelGFS2");
            this.labelGFS2.Name = "labelGFS2";
            // 
            // radioButtonLvm
            // 
            resources.ApplyResources(this.radioButtonLvm, "radioButtonLvm");
            this.radioButtonLvm.Name = "radioButtonLvm";
            this.radioButtonLvm.UseVisualStyleBackColor = true;
            this.radioButtonLvm.CheckedChanged += new System.EventHandler(this.radioButtonLvm_CheckedChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // tableLayoutInfo
            // 
            resources.ApplyResources(this.tableLayoutInfo, "tableLayoutInfo");
            this.tableLayoutInfo.Controls.Add(this.pictureBoxInfo, 0, 0);
            this.tableLayoutInfo.Controls.Add(this.labelInfo, 1, 0);
            this.tableLayoutInfo.Controls.Add(this.linkLabelPoolProperties, 2, 0);
            this.tableLayoutInfo.Name = "tableLayoutInfo";
            // 
            // pictureBoxInfo
            // 
            this.pictureBoxInfo.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.pictureBoxInfo, "pictureBoxInfo");
            this.pictureBoxInfo.Name = "pictureBoxInfo";
            this.pictureBoxInfo.TabStop = false;
            // 
            // labelInfo
            // 
            resources.ApplyResources(this.labelInfo, "labelInfo");
            this.labelInfo.Name = "labelInfo";
            // 
            // linkLabelPoolProperties
            // 
            resources.ApplyResources(this.linkLabelPoolProperties, "linkLabelPoolProperties");
            this.linkLabelPoolProperties.Name = "linkLabelPoolProperties";
            this.linkLabelPoolProperties.TabStop = true;
            this.linkLabelPoolProperties.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelPoolProperties_LinkClicked);
            // 
            // tableLayoutWarning
            // 
            resources.ApplyResources(this.tableLayoutWarning, "tableLayoutWarning");
            this.tableLayoutWarning.Controls.Add(this.pictureBoxWarning, 0, 0);
            this.tableLayoutWarning.Controls.Add(this.labelWarning, 1, 0);
            this.tableLayoutWarning.Name = "tableLayoutWarning";
            // 
            // pictureBoxWarning
            // 
            this.pictureBoxWarning.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
            resources.ApplyResources(this.pictureBoxWarning, "pictureBoxWarning");
            this.pictureBoxWarning.Name = "pictureBoxWarning";
            this.pictureBoxWarning.TabStop = false;
            // 
            // labelWarning
            // 
            resources.ApplyResources(this.labelWarning, "labelWarning");
            this.labelWarning.Name = "labelWarning";
            // 
            // ChooseSrProvisioningPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ChooseSrProvisioningPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutInfo.ResumeLayout(false);
            this.tableLayoutInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo)).EndInit();
            this.tableLayoutWarning.ResumeLayout(false);
            this.tableLayoutWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarning)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonGfs2;
        private System.Windows.Forms.Label labelGFS2;
        private System.Windows.Forms.RadioButton radioButtonLvm;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBoxInfo;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutInfo;
        private System.Windows.Forms.LinkLabel linkLabelPoolProperties;
        private System.Windows.Forms.TableLayoutPanel tableLayoutWarning;
        private System.Windows.Forms.PictureBox pictureBoxWarning;
        private System.Windows.Forms.Label labelWarning;
    }
}
