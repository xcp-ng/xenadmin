namespace XenAdmin.Dialogs
{
    partial class AssignLicenseDialog
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
                licenseServerNameTextBox.TextChanged -= licenseServerPortTextBox_TextChanged;
                licenseServerPortTextBox.TextChanged -= licenseServerNameTextBox_TextChanged;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssignLicenseDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.mainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.licenseServerLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.licenseServerNameLabel = new System.Windows.Forms.Label();
            this.licenseServerNameTextBox = new System.Windows.Forms.TextBox();
            this.colonLabel = new System.Windows.Forms.Label();
            this.licenseServerPortTextBox = new System.Windows.Forms.TextBox();
            this.mainLabel = new System.Windows.Forms.Label();
            this.editionsGroupBox = new System.Windows.Forms.GroupBox();
            this.editionLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.perSocketRadioButton = new System.Windows.Forms.RadioButton();
            this.xenDesktopEnterpriseRadioButton = new System.Windows.Forms.RadioButton();
            this.enterprisePerSocketRadioButton = new System.Windows.Forms.RadioButton();
            this.enterprisePerUserRadioButton = new System.Windows.Forms.RadioButton();
            this.desktopPlusRadioButton = new System.Windows.Forms.RadioButton();
            this.desktopRadioButton = new System.Windows.Forms.RadioButton();
            this.desktopCloudRadioButton = new System.Windows.Forms.RadioButton();
            this.standardPerSocketRadioButton = new System.Windows.Forms.RadioButton();
            this.buttonsLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.mainLayoutPanel.SuspendLayout();
            this.licenseServerLayoutPanel.SuspendLayout();
            this.editionsGroupBox.SuspendLayout();
            this.editionLayoutPanel.SuspendLayout();
            this.buttonsLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // mainLayoutPanel
            // 
            resources.ApplyResources(this.mainLayoutPanel, "mainLayoutPanel");
            this.mainLayoutPanel.Controls.Add(this.licenseServerLayoutPanel, 0, 1);
            this.mainLayoutPanel.Controls.Add(this.mainLabel, 0, 0);
            this.mainLayoutPanel.Controls.Add(this.editionsGroupBox, 0, 2);
            this.mainLayoutPanel.Controls.Add(this.buttonsLayoutPanel, 0, 3);
            this.mainLayoutPanel.Name = "mainLayoutPanel";
            // 
            // licenseServerLayoutPanel
            // 
            resources.ApplyResources(this.licenseServerLayoutPanel, "licenseServerLayoutPanel");
            this.licenseServerLayoutPanel.Controls.Add(this.licenseServerNameLabel, 0, 0);
            this.licenseServerLayoutPanel.Controls.Add(this.licenseServerNameTextBox, 1, 0);
            this.licenseServerLayoutPanel.Controls.Add(this.colonLabel, 2, 0);
            this.licenseServerLayoutPanel.Controls.Add(this.licenseServerPortTextBox, 3, 0);
            this.licenseServerLayoutPanel.Name = "licenseServerLayoutPanel";
            // 
            // licenseServerNameLabel
            // 
            resources.ApplyResources(this.licenseServerNameLabel, "licenseServerNameLabel");
            this.licenseServerNameLabel.Name = "licenseServerNameLabel";
            // 
            // licenseServerNameTextBox
            // 
            resources.ApplyResources(this.licenseServerNameTextBox, "licenseServerNameTextBox");
            this.licenseServerNameTextBox.Name = "licenseServerNameTextBox";
            // 
            // colonLabel
            // 
            resources.ApplyResources(this.colonLabel, "colonLabel");
            this.colonLabel.Name = "colonLabel";
            // 
            // licenseServerPortTextBox
            // 
            resources.ApplyResources(this.licenseServerPortTextBox, "licenseServerPortTextBox");
            this.licenseServerPortTextBox.Name = "licenseServerPortTextBox";
            // 
            // mainLabel
            // 
            resources.ApplyResources(this.mainLabel, "mainLabel");
            this.mainLabel.Name = "mainLabel";
            // 
            // editionsGroupBox
            // 
            resources.ApplyResources(this.editionsGroupBox, "editionsGroupBox");
            this.editionsGroupBox.Controls.Add(this.editionLayoutPanel);
            this.editionsGroupBox.Name = "editionsGroupBox";
            this.editionsGroupBox.TabStop = false;
            // 
            // editionLayoutPanel
            // 
            resources.ApplyResources(this.editionLayoutPanel, "editionLayoutPanel");
            this.editionLayoutPanel.Controls.Add(this.perSocketRadioButton);
            this.editionLayoutPanel.Controls.Add(this.xenDesktopEnterpriseRadioButton);
            this.editionLayoutPanel.Controls.Add(this.enterprisePerSocketRadioButton);
            this.editionLayoutPanel.Controls.Add(this.enterprisePerUserRadioButton);
            this.editionLayoutPanel.Controls.Add(this.desktopPlusRadioButton);
            this.editionLayoutPanel.Controls.Add(this.desktopRadioButton);
            this.editionLayoutPanel.Controls.Add(this.desktopCloudRadioButton);
            this.editionLayoutPanel.Controls.Add(this.standardPerSocketRadioButton);
            this.editionLayoutPanel.Name = "editionLayoutPanel";
            // 
            // perSocketRadioButton
            // 
            resources.ApplyResources(this.perSocketRadioButton, "perSocketRadioButton");
            this.perSocketRadioButton.Checked = true;
            this.perSocketRadioButton.Name = "perSocketRadioButton";
            this.perSocketRadioButton.TabStop = true;
            this.perSocketRadioButton.UseVisualStyleBackColor = true;
            // 
            // xenDesktopEnterpriseRadioButton
            // 
            resources.ApplyResources(this.xenDesktopEnterpriseRadioButton, "xenDesktopEnterpriseRadioButton");
            this.xenDesktopEnterpriseRadioButton.Name = "xenDesktopEnterpriseRadioButton";
            this.xenDesktopEnterpriseRadioButton.UseVisualStyleBackColor = true;
            // 
            // enterprisePerSocketRadioButton
            // 
            resources.ApplyResources(this.enterprisePerSocketRadioButton, "enterprisePerSocketRadioButton");
            this.enterprisePerSocketRadioButton.Name = "enterprisePerSocketRadioButton";
            this.enterprisePerSocketRadioButton.UseVisualStyleBackColor = true;
            // 
            // enterprisePerUserRadioButton
            // 
            resources.ApplyResources(this.enterprisePerUserRadioButton, "enterprisePerUserRadioButton");
            this.enterprisePerUserRadioButton.Name = "enterprisePerUserRadioButton";
            this.enterprisePerUserRadioButton.UseVisualStyleBackColor = true;
            // 
            // desktopPlusRadioButton
            // 
            resources.ApplyResources(this.desktopPlusRadioButton, "desktopPlusRadioButton");
            this.desktopPlusRadioButton.Name = "desktopPlusRadioButton";
            this.desktopPlusRadioButton.UseVisualStyleBackColor = true;
            // 
            // desktopRadioButton
            // 
            resources.ApplyResources(this.desktopRadioButton, "desktopRadioButton");
            this.desktopRadioButton.Name = "desktopRadioButton";
            this.desktopRadioButton.UseVisualStyleBackColor = true;
            // 
            // desktopCloudRadioButton
            // 
            resources.ApplyResources(this.desktopCloudRadioButton, "desktopCloudRadioButton");
            this.desktopCloudRadioButton.Name = "desktopCloudRadioButton";
            this.desktopCloudRadioButton.UseVisualStyleBackColor = true;
            // 
            // standardPerSocketRadioButton
            // 
            resources.ApplyResources(this.standardPerSocketRadioButton, "standardPerSocketRadioButton");
            this.standardPerSocketRadioButton.Name = "standardPerSocketRadioButton";
            this.standardPerSocketRadioButton.UseVisualStyleBackColor = true;
            // 
            // buttonsLayoutPanel
            // 
            resources.ApplyResources(this.buttonsLayoutPanel, "buttonsLayoutPanel");
            this.buttonsLayoutPanel.Controls.Add(this.okButton);
            this.buttonsLayoutPanel.Controls.Add(this.cancelButton);
            this.buttonsLayoutPanel.Name = "buttonsLayoutPanel";
            // 
            // AssignLicenseDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.mainLayoutPanel);
            this.Name = "AssignLicenseDialog";
            this.Shown += new System.EventHandler(this.AssignLicenseDialog_Shown);
            this.mainLayoutPanel.ResumeLayout(false);
            this.mainLayoutPanel.PerformLayout();
            this.licenseServerLayoutPanel.ResumeLayout(false);
            this.licenseServerLayoutPanel.PerformLayout();
            this.editionsGroupBox.ResumeLayout(false);
            this.editionsGroupBox.PerformLayout();
            this.editionLayoutPanel.ResumeLayout(false);
            this.editionLayoutPanel.PerformLayout();
            this.buttonsLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TableLayoutPanel mainLayoutPanel;
        private System.Windows.Forms.Label mainLabel;
        private System.Windows.Forms.FlowLayoutPanel buttonsLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel licenseServerLayoutPanel;
        private System.Windows.Forms.Label licenseServerNameLabel;
        private System.Windows.Forms.TextBox licenseServerNameTextBox;
        private System.Windows.Forms.TextBox licenseServerPortTextBox;
        private System.Windows.Forms.Label colonLabel;
        private System.Windows.Forms.GroupBox editionsGroupBox;
        private System.Windows.Forms.FlowLayoutPanel editionLayoutPanel;
        private System.Windows.Forms.RadioButton perSocketRadioButton;
        private System.Windows.Forms.RadioButton xenDesktopEnterpriseRadioButton;
        private System.Windows.Forms.RadioButton enterprisePerSocketRadioButton;
        private System.Windows.Forms.RadioButton enterprisePerUserRadioButton;
        private System.Windows.Forms.RadioButton desktopRadioButton;
        private System.Windows.Forms.RadioButton standardPerSocketRadioButton;
        private System.Windows.Forms.RadioButton desktopPlusRadioButton;
        private System.Windows.Forms.RadioButton desktopCloudRadioButton;
    }
}