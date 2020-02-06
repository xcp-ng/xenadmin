namespace XenAdmin.SettingsPanels
{
    partial class VDISizeLocationPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VDISizeLocationPage));
            this.SizeLabel = new System.Windows.Forms.Label();
            this.locationLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanelInfo = new System.Windows.Forms.TableLayoutPanel();
            this.labelShutDownWarning = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelLocationValueRO = new System.Windows.Forms.Label();
            this.diskSpinner1 = new XenAdmin.Controls.DiskSpinner();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // SizeLabel
            // 
            resources.ApplyResources(this.SizeLabel, "SizeLabel");
            this.SizeLabel.Name = "SizeLabel";
            // 
            // locationLabel
            // 
            resources.ApplyResources(this.locationLabel, "locationLabel");
            this.locationLabel.Name = "locationLabel";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.SizeLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.locationLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelInfo, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelLocationValueRO, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.diskSpinner1, 1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // tableLayoutPanelInfo
            // 
            resources.ApplyResources(this.tableLayoutPanelInfo, "tableLayoutPanelInfo");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanelInfo, 2);
            this.tableLayoutPanelInfo.Controls.Add(this.labelShutDownWarning, 1, 0);
            this.tableLayoutPanelInfo.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanelInfo.Name = "tableLayoutPanelInfo";
            // 
            // labelShutDownWarning
            // 
            resources.ApplyResources(this.labelShutDownWarning, "labelShutDownWarning");
            this.labelShutDownWarning.Name = "labelShutDownWarning";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // labelLocationValueRO
            // 
            resources.ApplyResources(this.labelLocationValueRO, "labelLocationValueRO");
            this.labelLocationValueRO.Name = "labelLocationValueRO";
            // 
            // diskSpinner1
            // 
            resources.ApplyResources(this.diskSpinner1, "diskSpinner1");
            this.diskSpinner1.Name = "diskSpinner1";
            this.diskSpinner1.SelectedSizeChanged += new System.Action(this.diskSpinner1_SelectedSizeChanged);
            // 
            // VDISizeLocationPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "VDISizeLocationPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanelInfo.ResumeLayout(false);
            this.tableLayoutPanelInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label SizeLabel;
        private System.Windows.Forms.Label locationLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelShutDownWarning;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelLocationValueRO;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelInfo;
        private Controls.DiskSpinner diskSpinner1;
    }
}
