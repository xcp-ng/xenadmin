using System.Windows.Forms;

namespace XenAdmin.Controls
{
    partial class SnapshotTreeView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SnapshotTreeView));
            this.SuspendLayout();
            // 
            // SnapshotTreeView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoArrange = false;
            this.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.OwnerDraw = true;
            this.ShowGroups = false;
            this.TileSize = new System.Drawing.Size(80, 60);
            this.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.SnapshotTreeView_DrawItem);
            this.ResumeLayout(false);

        }



        #endregion


    }
}