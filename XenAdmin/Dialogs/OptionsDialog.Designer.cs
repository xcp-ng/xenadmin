namespace XenAdmin.Dialogs
{
    partial class OptionsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsDialog));
            this.connectionOptionsPage1 = new XenAdmin.Dialogs.OptionsPages.ConnectionOptionsPage();
            this.consolesOptionsPage1 = new XenAdmin.Dialogs.OptionsPages.ConsolesOptionsPage();
            this.graphsOptionsPage1 = new XenAdmin.Dialogs.OptionsPages.DisplayOptionsPage();
            this.graphsOptions2Page1 = new XenAdmin.Dialogs.OptionsPages.DisplayOptions2Page();
            this.updatesOptionsPage1 = new XenAdmin.Dialogs.OptionsPages.UpdatesOptionsPage();
            this.securityOptionsPage1 = new XenAdmin.Dialogs.OptionsPages.SecurityOptionsPage();
            this.saveAndRestoreOptionsPage1 = new XenAdmin.Dialogs.OptionsPages.SaveAndRestoreOptionsPage();
            this.pluginOptionsPage1 = new XenAdmin.Dialogs.OptionsPages.PluginOptionsPage();
            this.confirmationOptionsPage1 = new XenAdmin.Dialogs.OptionsPages.ConfirmationOptionsPage();
            this.ContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.blueBorder.SuspendLayout();
            this.SuspendLayout();
            // 
            // ContentPanel
            // 
            resources.ApplyResources(this.ContentPanel, "ContentPanel");
            this.ContentPanel.Controls.Add(this.confirmationOptionsPage1);
            this.ContentPanel.Controls.Add(this.pluginOptionsPage1);
            this.ContentPanel.Controls.Add(this.saveAndRestoreOptionsPage1);
            this.ContentPanel.Controls.Add(this.securityOptionsPage1);
            this.ContentPanel.Controls.Add(this.updatesOptionsPage1);
            this.ContentPanel.Controls.Add(this.graphsOptionsPage1);
            this.ContentPanel.Controls.Add(this.graphsOptions2Page1);
            this.ContentPanel.Controls.Add(this.consolesOptionsPage1);
            this.ContentPanel.Controls.Add(this.connectionOptionsPage1);
            // 
            // verticalTabs
            // 
            resources.ApplyResources(this.verticalTabs, "verticalTabs");
            this.verticalTabs.Items.AddRange(new object[] {
            this.securityOptionsPage1,
            this.updatesOptionsPage1,
            this.graphsOptionsPage1,
            this.graphsOptions2Page1,
            this.consolesOptionsPage1,
            this.connectionOptionsPage1,
            this.saveAndRestoreOptionsPage1,
            this.pluginOptionsPage1,
            this.confirmationOptionsPage1});
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // splitContainer
            // 
            resources.ApplyResources(this.splitContainer, "splitContainer");
            // 
            // blueBorder
            // 
            resources.ApplyResources(this.blueBorder, "blueBorder");
            // 
            // connectionOptionsPage1
            // 
            resources.ApplyResources(this.connectionOptionsPage1, "connectionOptionsPage1");
            this.connectionOptionsPage1.Name = "connectionOptionsPage1";
            // 
            // consolesOptionsPage1
            // 
            resources.ApplyResources(this.consolesOptionsPage1, "consolesOptionsPage1");
            this.consolesOptionsPage1.Name = "consolesOptionsPage1";
            // 
            // graphsOptionsPage1
            // 
            resources.ApplyResources(this.graphsOptionsPage1, "graphsOptionsPage1");
            this.graphsOptionsPage1.Name = "graphsOptionsPage1";
            // 
            // graphsOptions2Page1
            // 
            resources.ApplyResources(this.graphsOptions2Page1, "graphsOptions2Page1");
            this.graphsOptions2Page1.Name = "graphsOptions2Page1";
            // 
            // updatesOptionsPage1
            // 
            resources.ApplyResources(this.updatesOptionsPage1, "updatesOptionsPage1");
            this.updatesOptionsPage1.Name = "updatesOptionsPage1";
            // 
            // securityOptionsPage1
            // 
            resources.ApplyResources(this.securityOptionsPage1, "securityOptionsPage1");
            this.securityOptionsPage1.Name = "securityOptionsPage1";
            // 
            // saveAndRestoreOptionsPage1
            // 
            resources.ApplyResources(this.saveAndRestoreOptionsPage1, "saveAndRestoreOptionsPage1");
            this.saveAndRestoreOptionsPage1.Name = "saveAndRestoreOptionsPage1";
            // 
            // pluginOptionsPage1
            // 
            resources.ApplyResources(this.pluginOptionsPage1, "pluginOptionsPage1");
            this.pluginOptionsPage1.Name = "pluginOptionsPage1";
            // 
            // confirmationOptionsPage1
            // 
            resources.ApplyResources(this.confirmationOptionsPage1, "confirmationOptionsPage1");
            this.confirmationOptionsPage1.Name = "confirmationOptionsPage1";
            // 
            // OptionsDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "OptionsDialog";
            this.ContentPanel.ResumeLayout(false);
            this.ContentPanel.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.blueBorder.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private XenAdmin.Dialogs.OptionsPages.ConnectionOptionsPage connectionOptionsPage1;
        private XenAdmin.Dialogs.OptionsPages.ConsolesOptionsPage consolesOptionsPage1;
        private XenAdmin.Dialogs.OptionsPages.UpdatesOptionsPage updatesOptionsPage1;
        private XenAdmin.Dialogs.OptionsPages.DisplayOptionsPage graphsOptionsPage1;
        private XenAdmin.Dialogs.OptionsPages.DisplayOptions2Page graphsOptions2Page1;
        private XenAdmin.Dialogs.OptionsPages.SecurityOptionsPage securityOptionsPage1;
        private XenAdmin.Dialogs.OptionsPages.SaveAndRestoreOptionsPage saveAndRestoreOptionsPage1;
        private XenAdmin.Dialogs.OptionsPages.PluginOptionsPage pluginOptionsPage1;
        private OptionsPages.ConfirmationOptionsPage confirmationOptionsPage1;
    }
}
