namespace XenAdmin.SettingsPanels
{
    partial class BootOptionsEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BootOptionsEditPage));
            this.m_textBoxOsParams = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_autoHeightLabelHvm = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_autoHeightLabelNonHvm = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_tlpHvm = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.m_buttonUp = new System.Windows.Forms.Button();
            this.m_buttonDown = new System.Windows.Forms.Button();
            this.m_checkedListBox = new System.Windows.Forms.CheckedListBox();
            this.m_tlpNonHvm = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.m_comboBoxBootDevice = new System.Windows.Forms.ComboBox();
            this.m_checkBoxAutoBoot = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.m_tlpHvm.SuspendLayout();
            this.m_tlpNonHvm.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_textBoxOsParams
            // 
            resources.ApplyResources(this.m_textBoxOsParams, "m_textBoxOsParams");
            this.m_textBoxOsParams.Name = "m_textBoxOsParams";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.m_autoHeightLabelHvm, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_autoHeightLabelNonHvm, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.m_tlpHvm, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.m_tlpNonHvm, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.m_checkBoxAutoBoot, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // m_autoHeightLabelHvm
            // 
            resources.ApplyResources(this.m_autoHeightLabelHvm, "m_autoHeightLabelHvm");
            this.tableLayoutPanel1.SetColumnSpan(this.m_autoHeightLabelHvm, 4);
            this.m_autoHeightLabelHvm.Name = "m_autoHeightLabelHvm";
            // 
            // m_autoHeightLabelNonHvm
            // 
            resources.ApplyResources(this.m_autoHeightLabelNonHvm, "m_autoHeightLabelNonHvm");
            this.tableLayoutPanel1.SetColumnSpan(this.m_autoHeightLabelNonHvm, 3);
            this.m_autoHeightLabelNonHvm.Name = "m_autoHeightLabelNonHvm";
            // 
            // m_tlpHvm
            // 
            resources.ApplyResources(this.m_tlpHvm, "m_tlpHvm");
            this.m_tlpHvm.Controls.Add(this.label1, 0, 0);
            this.m_tlpHvm.Controls.Add(this.m_buttonUp, 2, 0);
            this.m_tlpHvm.Controls.Add(this.m_buttonDown, 2, 1);
            this.m_tlpHvm.Controls.Add(this.m_checkedListBox, 1, 0);
            this.m_tlpHvm.Name = "m_tlpHvm";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // m_buttonUp
            // 
            resources.ApplyResources(this.m_buttonUp, "m_buttonUp");
            this.m_buttonUp.Name = "m_buttonUp";
            this.m_buttonUp.UseVisualStyleBackColor = true;
            this.m_buttonUp.Click += new System.EventHandler(this.m_buttonUp_Click);
            // 
            // m_buttonDown
            // 
            resources.ApplyResources(this.m_buttonDown, "m_buttonDown");
            this.m_buttonDown.Name = "m_buttonDown";
            this.m_buttonDown.UseVisualStyleBackColor = true;
            this.m_buttonDown.Click += new System.EventHandler(this.m_buttonDown_Click);
            // 
            // m_checkedListBox
            // 
            resources.ApplyResources(this.m_checkedListBox, "m_checkedListBox");
            this.m_checkedListBox.FormattingEnabled = true;
            this.m_checkedListBox.Name = "m_checkedListBox";
            this.m_tlpHvm.SetRowSpan(this.m_checkedListBox, 3);
            this.m_checkedListBox.SelectedIndexChanged += new System.EventHandler(this.m_checkedListBox_SelectedIndexChanged);
            // 
            // m_tlpNonHvm
            // 
            resources.ApplyResources(this.m_tlpNonHvm, "m_tlpNonHvm");
            this.m_tlpNonHvm.Controls.Add(this.label3, 0, 2);
            this.m_tlpNonHvm.Controls.Add(this.m_textBoxOsParams, 1, 2);
            this.m_tlpNonHvm.Controls.Add(this.label2, 0, 0);
            this.m_tlpNonHvm.Controls.Add(this.m_comboBoxBootDevice, 1, 0);
            this.m_tlpNonHvm.Name = "m_tlpNonHvm";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // m_comboBoxBootDevice
            // 
            resources.ApplyResources(this.m_comboBoxBootDevice, "m_comboBoxBootDevice");
            this.m_comboBoxBootDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboBoxBootDevice.FormattingEnabled = true;
            this.m_comboBoxBootDevice.Name = "m_comboBoxBootDevice";
            this.m_comboBoxBootDevice.SelectedIndexChanged += new System.EventHandler(this.m_comboBoxBootDevice_SelectedIndexChanged);
            // 
            // m_checkBoxAutoBoot
            // 
            resources.ApplyResources(this.m_checkBoxAutoBoot, "m_checkBoxAutoBoot");
            this.m_checkBoxAutoBoot.Name = "m_checkBoxAutoBoot";
            this.m_checkBoxAutoBoot.UseVisualStyleBackColor = true;
            // 
            // BootOptionsEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "BootOptionsEditPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.m_tlpHvm.ResumeLayout(false);
            this.m_tlpHvm.PerformLayout();
            this.m_tlpNonHvm.ResumeLayout(false);
            this.m_tlpNonHvm.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox m_textBoxOsParams;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel m_tlpHvm;
		private System.Windows.Forms.TableLayoutPanel m_tlpNonHvm;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox m_comboBoxBootDevice;
		private XenAdmin.Controls.Common.AutoHeightLabel m_autoHeightLabelHvm;
		private XenAdmin.Controls.Common.AutoHeightLabel m_autoHeightLabelNonHvm;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button m_buttonUp;
		private System.Windows.Forms.Button m_buttonDown;
        private System.Windows.Forms.CheckedListBox m_checkedListBox;
        private System.Windows.Forms.CheckBox m_checkBoxAutoBoot;
    }
}
