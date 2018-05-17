namespace XenAdmin.Dialogs.OptionsPages
{
    partial class DisplayOptions2Page
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisplayOptions2Page));
            this.ViewMemoryRelativeToEachOtherCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ViewMemoryRelativeToEachOtherCheckbox
            // 
            resources.ApplyResources(this.ViewMemoryRelativeToEachOtherCheckbox, "ViewMemoryRelativeToEachOtherCheckbox");
            this.ViewMemoryRelativeToEachOtherCheckbox.Name = "ViewMemoryRelativeToEachOtherCheckbox";
            this.ViewMemoryRelativeToEachOtherCheckbox.UseVisualStyleBackColor = true;
            // 
            // DisplayOptions2Page
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.ViewMemoryRelativeToEachOtherCheckbox);
            this.Name = "DisplayOptions2Page";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox ViewMemoryRelativeToEachOtherCheckbox;
    }
}
