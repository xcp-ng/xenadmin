namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{
    partial class IntraPoolCopyPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IntraPoolCopyPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.DescriptionTextBox = new System.Windows.Forms.TextBox();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanelSrPicker = new System.Windows.Forms.TableLayoutPanel();
            this.srPicker1 = new XenAdmin.Controls.SrPicker();
            this.labelSrHint = new System.Windows.Forms.Label();
            this.toolTipContainer1 = new XenAdmin.Controls.ToolTipContainer();
            this.FastClonePanel = new System.Windows.Forms.Panel();
            this.CloneRadioButton = new System.Windows.Forms.RadioButton();
            this.FastCloneDescription = new System.Windows.Forms.Label();
            this.CopyRadioButton = new System.Windows.Forms.RadioButton();
            this.labelRubric = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanelSrPicker.SuspendLayout();
            this.toolTipContainer1.SuspendLayout();
            this.FastClonePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.DescriptionTextBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.NameTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelRubric, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // DescriptionTextBox
            // 
            resources.ApplyResources(this.DescriptionTextBox, "DescriptionTextBox");
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.TextChanged += new System.EventHandler(this.NameTextBox_TextChanged);
            // 
            // NameTextBox
            // 
            resources.ApplyResources(this.NameTextBox, "NameTextBox");
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.TextChanged += new System.EventHandler(this.NameTextBox_TextChanged);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 2);
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
            // srPicker1
            // 
            resources.ApplyResources(this.srPicker1, "srPicker1");
            this.srPicker1.Connection = null;
            this.srPicker1.Name = "srPicker1";
            this.srPicker1.SelectedIndexChanged += new System.EventHandler(this.srPicker1_SelectedIndexChanged);
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
            // CloneRadioButton
            // 
            resources.ApplyResources(this.CloneRadioButton, "CloneRadioButton");
            this.CloneRadioButton.Checked = true;
            this.CloneRadioButton.Name = "CloneRadioButton";
            this.CloneRadioButton.TabStop = true;
            this.CloneRadioButton.UseVisualStyleBackColor = true;
            this.CloneRadioButton.CheckedChanged += new System.EventHandler(this.CloneRadioButton_CheckedChanged);
            // 
            // FastCloneDescription
            // 
            resources.ApplyResources(this.FastCloneDescription, "FastCloneDescription");
            this.FastCloneDescription.AutoEllipsis = true;
            this.FastCloneDescription.Name = "FastCloneDescription";
            // 
            // CopyRadioButton
            // 
            resources.ApplyResources(this.CopyRadioButton, "CopyRadioButton");
            this.CopyRadioButton.Name = "CopyRadioButton";
            this.CopyRadioButton.UseVisualStyleBackColor = true;
            this.CopyRadioButton.CheckedChanged += new System.EventHandler(this.CopyRadioButton_CheckedChanged);
            // 
            // labelRubric
            // 
            resources.ApplyResources(this.labelRubric, "labelRubric");
            this.tableLayoutPanel1.SetColumnSpan(this.labelRubric, 2);
            this.labelRubric.Name = "labelRubric";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // IntraPoolCopyPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "IntraPoolCopyPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanelSrPicker.ResumeLayout(false);
            this.tableLayoutPanelSrPicker.PerformLayout();
            this.toolTipContainer1.ResumeLayout(false);
            this.FastClonePanel.ResumeLayout(false);
            this.FastClonePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelRubric;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Controls.DecentGroupBox groupBox1;
        private Controls.ToolTipContainer toolTipContainer1;
        private System.Windows.Forms.Panel FastClonePanel;
        private System.Windows.Forms.RadioButton CloneRadioButton;
        private System.Windows.Forms.Label FastCloneDescription;
        private System.Windows.Forms.RadioButton CopyRadioButton;
        private Controls.SrPicker srPicker1;
        private System.Windows.Forms.TextBox DescriptionTextBox;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSrPicker;
        private System.Windows.Forms.Label labelSrHint;
    }
}

