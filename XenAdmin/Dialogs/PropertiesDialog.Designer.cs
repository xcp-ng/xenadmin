namespace XenAdmin.Dialogs
{
    partial class PropertiesDialog
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
            if (disposing)
            {
                if (VMHAEditPage != null)
                    VMHAEditPage.Populated -= this.EditPage_Populated;
                if (usbEditPage != null)
                    usbEditPage.Populated -= this.EditPage_Populated;
                if (newPolicySnapshotFrequencyPage1 != null)
                    newPolicySnapshotFrequencyPage1.Populated -= EditPage_Populated;

                if (components != null)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesDialog));
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
            // 
            // verticalTabs
            // 
            resources.ApplyResources(this.verticalTabs, "verticalTabs");
            this.verticalTabs.SelectedIndexChanged += new System.EventHandler(this.verticalTabs_SelectedIndexChanged);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // splitContainer
            // 
            resources.ApplyResources(this.splitContainer, "splitContainer");
            // 
            // blueBorder
            // 
            resources.ApplyResources(this.blueBorder, "blueBorder");
            // 
            // PropertiesDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "PropertiesDialog";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PropertiesDialog_FormClosed);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.blueBorder.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        
    }
}
