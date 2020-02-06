namespace XenAdmin.Dialogs
{
    partial class VmSnapshotDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VmSnapshotDialog));
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.labelCheckpointInfo = new System.Windows.Forms.Label();
            this.labelQuiesceInfo = new System.Windows.Forms.Label();
            this.labelSnapshotInfo = new System.Windows.Forms.Label();
            this.diskRadioButton = new System.Windows.Forms.RadioButton();
            this.quiesceCheckBox = new System.Windows.Forms.CheckBox();
            this.memoryRadioButton = new System.Windows.Forms.RadioButton();
            this.pictureBoxSnapshotsInfo = new System.Windows.Forms.PictureBox();
            this.CheckpointInfoPictureBox = new System.Windows.Forms.PictureBox();
            this.pictureBoxQuiesceInfo = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSnapshotsInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckpointInfoPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxQuiesceInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // labelName
            // 
            resources.ApplyResources(this.labelName, "labelName");
            this.labelName.Name = "labelName";
            // 
            // textBoxName
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxName, 2);
            resources.ApplyResources(this.textBoxName, "textBoxName");
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBoxDescription
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxDescription, 2);
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.Name = "textBoxDescription";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.buttonOk, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBoxName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxDescription, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 3);
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.labelCheckpointInfo, 2, 5);
            this.tableLayoutPanel2.Controls.Add(this.labelQuiesceInfo, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.labelSnapshotInfo, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.diskRadioButton, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.quiesceCheckBox, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.memoryRadioButton, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.pictureBoxSnapshotsInfo, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.CheckpointInfoPictureBox, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.pictureBoxQuiesceInfo, 1, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // labelCheckpointInfo
            // 
            resources.ApplyResources(this.labelCheckpointInfo, "labelCheckpointInfo");
            this.labelCheckpointInfo.Name = "labelCheckpointInfo";
            // 
            // labelQuiesceInfo
            // 
            resources.ApplyResources(this.labelQuiesceInfo, "labelQuiesceInfo");
            this.labelQuiesceInfo.Name = "labelQuiesceInfo";
            // 
            // labelSnapshotInfo
            // 
            resources.ApplyResources(this.labelSnapshotInfo, "labelSnapshotInfo");
            this.labelSnapshotInfo.Name = "labelSnapshotInfo";
            // 
            // diskRadioButton
            // 
            resources.ApplyResources(this.diskRadioButton, "diskRadioButton");
            this.diskRadioButton.Checked = true;
            this.tableLayoutPanel2.SetColumnSpan(this.diskRadioButton, 3);
            this.diskRadioButton.Name = "diskRadioButton";
            this.diskRadioButton.TabStop = true;
            this.diskRadioButton.UseVisualStyleBackColor = true;
            this.diskRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // quiesceCheckBox
            // 
            resources.ApplyResources(this.quiesceCheckBox, "quiesceCheckBox");
            this.tableLayoutPanel2.SetColumnSpan(this.quiesceCheckBox, 2);
            this.quiesceCheckBox.Name = "quiesceCheckBox";
            this.quiesceCheckBox.UseVisualStyleBackColor = true;
            this.quiesceCheckBox.CheckedChanged += new System.EventHandler(this.quiesceCheckBox_CheckedChanged);
            // 
            // memoryRadioButton
            // 
            resources.ApplyResources(this.memoryRadioButton, "memoryRadioButton");
            this.tableLayoutPanel2.SetColumnSpan(this.memoryRadioButton, 3);
            this.memoryRadioButton.Name = "memoryRadioButton";
            this.memoryRadioButton.TabStop = true;
            this.memoryRadioButton.UseVisualStyleBackColor = true;
            this.memoryRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // pictureBoxSnapshotsInfo
            // 
            this.pictureBoxSnapshotsInfo.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.pictureBoxSnapshotsInfo, "pictureBoxSnapshotsInfo");
            this.pictureBoxSnapshotsInfo.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBoxSnapshotsInfo.Name = "pictureBoxSnapshotsInfo";
            this.pictureBoxSnapshotsInfo.TabStop = false;
            // 
            // CheckpointInfoPictureBox
            // 
            this.CheckpointInfoPictureBox.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.CheckpointInfoPictureBox, "CheckpointInfoPictureBox");
            this.CheckpointInfoPictureBox.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.CheckpointInfoPictureBox.Name = "CheckpointInfoPictureBox";
            this.CheckpointInfoPictureBox.TabStop = false;
            // 
            // pictureBoxQuiesceInfo
            // 
            this.pictureBoxQuiesceInfo.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.pictureBoxQuiesceInfo, "pictureBoxQuiesceInfo");
            this.pictureBoxQuiesceInfo.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBoxQuiesceInfo.Name = "pictureBoxQuiesceInfo";
            this.pictureBoxQuiesceInfo.TabStop = false;
            // 
            // VmSnapshotDialog
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "VmSnapshotDialog";
            this.Load += new System.EventHandler(this.VmSnapshotDialog_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSnapshotsInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckpointInfoPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxQuiesceInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton memoryRadioButton;
        private System.Windows.Forms.PictureBox CheckpointInfoPictureBox;
        private System.Windows.Forms.CheckBox quiesceCheckBox;
        private System.Windows.Forms.PictureBox pictureBoxQuiesceInfo;
        private System.Windows.Forms.RadioButton diskRadioButton;
        private System.Windows.Forms.PictureBox pictureBoxSnapshotsInfo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label labelCheckpointInfo;
        private System.Windows.Forms.Label labelQuiesceInfo;
        private System.Windows.Forms.Label labelSnapshotInfo;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
    }
}