namespace XenAdmin.SettingsPanels
{
    partial class HostAutostartEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HostAutostartEditPage));
            this.enableAutostart = new System.Windows.Forms.CheckBox();
            this.autostartMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // enableAutostart
            // 
            resources.ApplyResources(this.enableAutostart, "enableAutostart");
            this.enableAutostart.Name = "enableAutostart";
            this.enableAutostart.UseVisualStyleBackColor = true;
            // 
            // autostartMessage
            // 
            resources.ApplyResources(this.autostartMessage, "autostartMessage");
            this.autostartMessage.Name = "autostartMessage";
            // 
            // HostAutostartEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.autostartMessage);
            this.Controls.Add(this.enableAutostart);
            this.Name = "HostAutostartEditPage";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox enableAutostart;
        private System.Windows.Forms.Label autostartMessage;
    }
}
