using XenAPI;

namespace XenAdmin.Wizards.NewPolicyWizard
{
    partial class NewPolicySnapshotTypePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewPolicySnapshotTypePage));
            this.radioButtonDiskAndMemory = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelWarning = new System.Windows.Forms.Label();
            this.pictureBoxWarning = new System.Windows.Forms.PictureBox();
            this.pictureBoxVSS = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButtonDiskOnly = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.quiesceCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelVss = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanelCheckpoint = new System.Windows.Forms.TableLayoutPanel();
            this.checkpointInfoPictureBox = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVSS)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanelVss.SuspendLayout();
            this.tableLayoutPanelCheckpoint.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkpointInfoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // radioButtonDiskAndMemory
            // 
            resources.ApplyResources(this.radioButtonDiskAndMemory, "radioButtonDiskAndMemory");
            this.tableLayoutPanel2.SetColumnSpan(this.radioButtonDiskAndMemory, 2);
            this.radioButtonDiskAndMemory.Name = "radioButtonDiskAndMemory";
            this.radioButtonDiskAndMemory.UseVisualStyleBackColor = true;
            this.radioButtonDiskAndMemory.CheckedChanged += new System.EventHandler(this.radioButtonDiskAndMemory_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelWarning, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxWarning, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelWarning
            // 
            resources.ApplyResources(this.labelWarning, "labelWarning");
            this.labelWarning.Name = "labelWarning";
            // 
            // pictureBoxWarning
            // 
            this.pictureBoxWarning.Image = global::XenAdmin.Properties.Resources._000_Alert2_h32bit_16;
            resources.ApplyResources(this.pictureBoxWarning, "pictureBoxWarning");
            this.pictureBoxWarning.Name = "pictureBoxWarning";
            this.pictureBoxWarning.TabStop = false;
            // 
            // pictureBoxVSS
            // 
            this.pictureBoxVSS.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.pictureBoxVSS, "pictureBoxVSS");
            this.pictureBoxVSS.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBoxVSS.Name = "pictureBoxVSS";
            this.pictureBoxVSS.TabStop = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // radioButtonDiskOnly
            // 
            resources.ApplyResources(this.radioButtonDiskOnly, "radioButtonDiskOnly");
            this.radioButtonDiskOnly.Checked = true;
            this.tableLayoutPanel2.SetColumnSpan(this.radioButtonDiskOnly, 2);
            this.radioButtonDiskOnly.Name = "radioButtonDiskOnly";
            this.radioButtonDiskOnly.TabStop = true;
            this.radioButtonDiskOnly.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel2.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // quiesceCheckBox
            // 
            resources.ApplyResources(this.quiesceCheckBox, "quiesceCheckBox");
            this.quiesceCheckBox.Name = "quiesceCheckBox";
            this.quiesceCheckBox.UseVisualStyleBackColor = true;
            this.quiesceCheckBox.CheckedChanged += new System.EventHandler(this.quiesceCheckBox_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.label3, 1, 6);
            this.tableLayoutPanel2.Controls.Add(this.radioButtonDiskOnly, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 1, 8);
            this.tableLayoutPanel2.Controls.Add(this.radioButtonDiskAndMemory, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.label2, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.quiesceCheckBox, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanelVss, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanelCheckpoint, 1, 7);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // tableLayoutPanelVss
            // 
            resources.ApplyResources(this.tableLayoutPanelVss, "tableLayoutPanelVss");
            this.tableLayoutPanelVss.Controls.Add(this.pictureBoxVSS, 0, 0);
            this.tableLayoutPanelVss.Controls.Add(this.label4, 1, 0);
            this.tableLayoutPanelVss.Name = "tableLayoutPanelVss";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // tableLayoutPanelCheckpoint
            // 
            resources.ApplyResources(this.tableLayoutPanelCheckpoint, "tableLayoutPanelCheckpoint");
            this.tableLayoutPanelCheckpoint.Controls.Add(this.checkpointInfoPictureBox, 0, 0);
            this.tableLayoutPanelCheckpoint.Controls.Add(this.label5, 1, 0);
            this.tableLayoutPanelCheckpoint.Name = "tableLayoutPanelCheckpoint";
            // 
            // checkpointInfoPictureBox
            // 
            this.checkpointInfoPictureBox.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.checkpointInfoPictureBox, "checkpointInfoPictureBox");
            this.checkpointInfoPictureBox.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.checkpointInfoPictureBox.Name = "checkpointInfoPictureBox";
            this.checkpointInfoPictureBox.TabStop = false;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // NewPolicySnapshotTypePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "NewPolicySnapshotTypePage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVSS)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanelVss.ResumeLayout(false);
            this.tableLayoutPanelVss.PerformLayout();
            this.tableLayoutPanelCheckpoint.ResumeLayout(false);
            this.tableLayoutPanelCheckpoint.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkpointInfoPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonDiskAndMemory;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBoxWarning;
        private System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.PictureBox pictureBoxVSS;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.RadioButton radioButtonDiskOnly;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox quiesceCheckBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelVss;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelCheckpoint;
        private System.Windows.Forms.PictureBox checkpointInfoPictureBox;
        private System.Windows.Forms.Label label5;
    }
}
