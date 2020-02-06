namespace XenAdmin.Dialogs
{
    partial class CopyVMDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CopyVMDialog));
            this.srPicker1 = new XenAdmin.Controls.SrPicker();
            this.CloseButton = new System.Windows.Forms.Button();
            this.MoveButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.CloneRadioButton = new System.Windows.Forms.RadioButton();
            this.CopyRadioButton = new System.Windows.Forms.RadioButton();
            this.DescriptionTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.FastCloneDescription = new System.Windows.Forms.Label();
            this.groupBox1 = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanelSrPicker = new System.Windows.Forms.TableLayoutPanel();
            this.labelSrHint = new System.Windows.Forms.Label();
            this.toolTipContainer1 = new XenAdmin.Controls.ToolTipContainer();
            this.FastClonePanel = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanelSrPicker.SuspendLayout();
            this.toolTipContainer1.SuspendLayout();
            this.FastClonePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // srPicker1
            // 
            resources.ApplyResources(this.srPicker1, "srPicker1");
            this.srPicker1.Connection = null;
            this.srPicker1.Name = "srPicker1";
            this.srPicker1.SelectedIndexChanged += new System.EventHandler(this.srPicker1_SelectedIndexChanged);
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // MoveButton
            // 
            resources.ApplyResources(this.MoveButton, "MoveButton");
            this.MoveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.MoveButton.Name = "MoveButton";
            this.MoveButton.UseVisualStyleBackColor = true;
            this.MoveButton.Click += new System.EventHandler(this.MoveButton_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // NameTextBox
            // 
            resources.ApplyResources(this.NameTextBox, "NameTextBox");
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.TextChanged += new System.EventHandler(this.NameTextBox_TextChanged);
            // 
            // CloneRadioButton
            // 
            resources.ApplyResources(this.CloneRadioButton, "CloneRadioButton");
            this.CloneRadioButton.Checked = true;
            this.CloneRadioButton.Name = "CloneRadioButton";
            this.CloneRadioButton.TabStop = true;
            this.CloneRadioButton.UseVisualStyleBackColor = true;
            this.CloneRadioButton.CheckedChanged += new System.EventHandler(this.CloneRadioButton_CheckedChanged);
            // 
            // CopyRadioButton
            // 
            resources.ApplyResources(this.CopyRadioButton, "CopyRadioButton");
            this.CopyRadioButton.Name = "CopyRadioButton";
            this.CopyRadioButton.UseVisualStyleBackColor = true;
            this.CopyRadioButton.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // DescriptionTextBox
            // 
            resources.ApplyResources(this.DescriptionTextBox, "DescriptionTextBox");
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.TextChanged += new System.EventHandler(this.NameTextBox_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // FastCloneDescription
            // 
            resources.ApplyResources(this.FastCloneDescription, "FastCloneDescription");
            this.FastCloneDescription.AutoEllipsis = true;
            this.FastCloneDescription.Name = "FastCloneDescription";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.tableLayoutPanelSrPicker);
            this.groupBox1.Controls.Add(this.toolTipContainer1);
            this.groupBox1.Controls.Add(this.CopyRadioButton);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanelSrPicker
            // 
            resources.ApplyResources(this.tableLayoutPanelSrPicker, "tableLayoutPanelSrPicker");
            this.tableLayoutPanelSrPicker.Controls.Add(this.srPicker1, 0, 1);
            this.tableLayoutPanelSrPicker.Controls.Add(this.labelSrHint, 0, 0);
            this.tableLayoutPanelSrPicker.Name = "tableLayoutPanelSrPicker";
            // 
            // labelSrHint
            // 
            resources.ApplyResources(this.labelSrHint, "labelSrHint");
            this.labelSrHint.Name = "labelSrHint";
            // 
            // toolTipContainer1
            // 
            resources.ApplyResources(this.toolTipContainer1, "toolTipContainer1");
            this.toolTipContainer1.Controls.Add(this.FastClonePanel);
            this.toolTipContainer1.Name = "toolTipContainer1";
            // 
            // FastClonePanel
            // 
            this.FastClonePanel.Controls.Add(this.CloneRadioButton);
            this.FastClonePanel.Controls.Add(this.FastCloneDescription);
            resources.ApplyResources(this.FastClonePanel, "FastClonePanel");
            this.FastClonePanel.Name = "FastClonePanel";
            // 
            // CopyVMDialog
            // 
            this.AcceptButton = this.MoveButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.CloseButton;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.DescriptionTextBox);
            this.Controls.Add(this.NameTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.MoveButton);
            this.Controls.Add(this.CloseButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "CopyVMDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Shown += new System.EventHandler(this.CopyVMDialog_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanelSrPicker.ResumeLayout(false);
            this.tableLayoutPanelSrPicker.PerformLayout();
            this.toolTipContainer1.ResumeLayout(false);
            this.FastClonePanel.ResumeLayout(false);
            this.FastClonePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.SrPicker srPicker1;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button MoveButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.RadioButton CloneRadioButton;
        private System.Windows.Forms.RadioButton CopyRadioButton;
        private System.Windows.Forms.TextBox DescriptionTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label FastCloneDescription;
        private XenAdmin.Controls.DecentGroupBox groupBox1;
        private XenAdmin.Controls.ToolTipContainer toolTipContainer1;
        private System.Windows.Forms.Panel FastClonePanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSrPicker;
        private System.Windows.Forms.Label labelSrHint;
    }
}
