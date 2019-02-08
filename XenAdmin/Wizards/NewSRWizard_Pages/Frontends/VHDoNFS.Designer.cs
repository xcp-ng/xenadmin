namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class VHDoNFS
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VHDoNFS));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonNfsReattach = new System.Windows.Forms.RadioButton();
            this.radioButtonNfsNew = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.nfsVersionLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.NfsServerPathTextBox = new System.Windows.Forms.TextBox();
            this.labelAdvancedOptions = new System.Windows.Forms.Label();
            this.serverOptionsTextBox = new System.Windows.Forms.TextBox();
            this.NfsScanButton = new System.Windows.Forms.Button();
            this.nfsVersionSelectorTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.nfsVersion3RadioButton = new System.Windows.Forms.RadioButton();
            this.nfsVersion4RadioButton = new System.Windows.Forms.RadioButton();
            this.nfsVersion41RadioButton = new System.Windows.Forms.RadioButton();
            this.listBoxNfsSRs = new XenAdmin.Controls.SRListBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.nfsVersionSelectorTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.nfsVersionLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.NfsServerPathTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelAdvancedOptions, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.serverOptionsTextBox, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonNfsNew, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonNfsReattach, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.listBoxNfsSRs, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.NfsScanButton, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.nfsVersionSelectorTableLayoutPanel, 1, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // radioButtonNfsReattach
            // 
            resources.ApplyResources(this.radioButtonNfsReattach, "radioButtonNfsReattach");
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonNfsReattach, 3);
            this.radioButtonNfsReattach.Name = "radioButtonNfsReattach";
            this.radioButtonNfsReattach.UseVisualStyleBackColor = true;
            this.radioButtonNfsReattach.CheckedChanged += new System.EventHandler(this.radioButtonNfsReattach_CheckedChanged);
            // 
            // radioButtonNfsNew
            // 
            resources.ApplyResources(this.radioButtonNfsNew, "radioButtonNfsNew");
            this.radioButtonNfsNew.Checked = true;
            this.tableLayoutPanel1.SetColumnSpan(this.radioButtonNfsNew, 2);
            this.radioButtonNfsNew.Name = "radioButtonNfsNew";
            this.radioButtonNfsNew.TabStop = true;
            this.radioButtonNfsNew.UseVisualStyleBackColor = true;
            this.radioButtonNfsNew.CheckedChanged += new System.EventHandler(this.radioButtonNfsNew_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label2.Name = "label2";
            // 
            // nfsVersionLabel
            // 
            resources.ApplyResources(this.nfsVersionLabel, "nfsVersionLabel");
            this.nfsVersionLabel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.nfsVersionLabel.Name = "nfsVersionLabel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
            this.label1.Name = "label1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label3.Name = "label3";
            // 
            // NfsServerPathTextBox
            // 
            resources.ApplyResources(this.NfsServerPathTextBox, "NfsServerPathTextBox");
            this.NfsServerPathTextBox.Name = "NfsServerPathTextBox";
            this.NfsServerPathTextBox.TextChanged += new System.EventHandler(this.NfsServerPathTextBox_TextChanged);
            // 
            // labelAdvancedOptions
            // 
            resources.ApplyResources(this.labelAdvancedOptions, "labelAdvancedOptions");
            this.labelAdvancedOptions.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelAdvancedOptions.Name = "labelAdvancedOptions";
            // 
            // serverOptionsTextBox
            // 
            resources.ApplyResources(this.serverOptionsTextBox, "serverOptionsTextBox");
            this.serverOptionsTextBox.Name = "serverOptionsTextBox";
            // 
            // NfsScanButton
            // 
            resources.ApplyResources(this.NfsScanButton, "NfsScanButton");
            this.NfsScanButton.Name = "NfsScanButton";
            this.NfsScanButton.UseVisualStyleBackColor = true;
            this.NfsScanButton.Click += new System.EventHandler(this.buttonNfsScan_Click);
            // 
            // nfsVersionSelectorTableLayoutPanel
            // 
            resources.ApplyResources(this.nfsVersionSelectorTableLayoutPanel, "nfsVersionSelectorTableLayoutPanel");
            this.tableLayoutPanel1.SetColumnSpan(this.nfsVersionSelectorTableLayoutPanel, 2);
            this.nfsVersionSelectorTableLayoutPanel.Controls.Add(this.nfsVersion3RadioButton, 0, 0);
            this.nfsVersionSelectorTableLayoutPanel.Controls.Add(this.nfsVersion4RadioButton, 0, 1);
            this.nfsVersionSelectorTableLayoutPanel.Controls.Add(this.nfsVersion41RadioButton, 0, 2);
            this.nfsVersionSelectorTableLayoutPanel.Name = "nfsVersionSelectorTableLayoutPanel";
            // 
            // nfsVersion3RadioButton
            // 
            resources.ApplyResources(this.nfsVersion3RadioButton, "nfsVersion3RadioButton");
            this.nfsVersion3RadioButton.Checked = true;
            this.nfsVersion3RadioButton.Name = "nfsVersion3RadioButton";
            this.nfsVersion3RadioButton.TabStop = true;
            this.nfsVersion3RadioButton.UseVisualStyleBackColor = true;
            // 
            // nfsVersion4RadioButton
            // 
            resources.ApplyResources(this.nfsVersion4RadioButton, "nfsVersion4RadioButton");
            this.nfsVersion4RadioButton.Name = "nfsVersion4RadioButton";
            this.nfsVersion4RadioButton.UseVisualStyleBackColor = true;
            // 
            // nfsVersion41RadioButton
            // 
            resources.ApplyResources(this.nfsVersion41RadioButton, "nfsVersion41RadioButton");
            this.nfsVersion41RadioButton.Name = "nfsVersion41RadioButton";
            this.nfsVersion41RadioButton.UseVisualStyleBackColor = true;
            // 
            // listBoxNfsSRs
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.listBoxNfsSRs, 3);
            resources.ApplyResources(this.listBoxNfsSRs, "listBoxNfsSRs");
            this.listBoxNfsSRs.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxNfsSRs.Name = "listBoxNfsSRs";
            this.listBoxNfsSRs.Sorted = true;
            this.listBoxNfsSRs.SelectedIndexChanged += new System.EventHandler(this.listBoxNfsSRs_SelectedIndexChanged);
            // 
            // VHDoNFS
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "VHDoNFS";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.nfsVersionSelectorTableLayoutPanel.ResumeLayout(false);
            this.nfsVersionSelectorTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonNfsReattach;
        private XenAdmin.Controls.SRListBox listBoxNfsSRs;
       private System.Windows.Forms.RadioButton radioButtonNfsNew;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox serverOptionsTextBox;
        private System.Windows.Forms.Label labelAdvancedOptions;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox NfsServerPathTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label nfsVersionLabel;
        private System.Windows.Forms.TableLayoutPanel nfsVersionSelectorTableLayoutPanel;
        private System.Windows.Forms.RadioButton nfsVersion3RadioButton;
        private System.Windows.Forms.RadioButton nfsVersion4RadioButton;
        private System.Windows.Forms.RadioButton nfsVersion41RadioButton;
        private System.Windows.Forms.Button NfsScanButton;
    }
}
