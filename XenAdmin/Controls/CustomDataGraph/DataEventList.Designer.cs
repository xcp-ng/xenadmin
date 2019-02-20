namespace XenAdmin.Controls.CustomDataGraph
{
    partial class DataEventList
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
            this.components = new System.ComponentModel.Container();
            this.EventListToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // EventListToolTip
            // 
            this.EventListToolTip.AutomaticDelay = 0;
            this.EventListToolTip.UseAnimation = false;
            this.EventListToolTip.UseFading = false;
            // 
            // DataEventList
            // 
            this.ItemHeight = 20;
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.DataEventList_MouseDoubleClick);
            this.SelectedIndexChanged += new System.EventHandler(this.DataEventList_SelectedIndexChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip EventListToolTip;
    }
}
