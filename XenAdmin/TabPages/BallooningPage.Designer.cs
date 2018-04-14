namespace XenAdmin.TabPages
{
    partial class BallooningPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BallooningPage));
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            this.pageContainerPanel.SizeChanged += new System.EventHandler(this.pageContainerPanel_SizeChanged);
            // 
            // BallooningPage
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "BallooningPage";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
