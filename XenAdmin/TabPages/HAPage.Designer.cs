namespace XenAdmin.TabPages
{
    partial class HAPage
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
                ConnectionsManager.History.CollectionChanged -= History_CollectionChanged;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HAPage));
            this.label6 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxWarningTriangle = new System.Windows.Forms.PictureBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonConfigure = new System.Windows.Forms.Button();
            this.buttonDisableHa = new System.Windows.Forms.Button();
            this.tableLatencies = new System.Windows.Forms.TableLayoutPanel();
            this.customListPanel = new XenAdmin.Controls.CustomListPanel();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.pageContainerPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarningTriangle)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageContainerPanel
            // 
            this.pageContainerPanel.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.pageContainerPanel, "pageContainerPanel");
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLatencies, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.customListPanel, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.pictureBoxWarningTriangle, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelStatus, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // pictureBoxWarningTriangle
            // 
            this.pictureBoxWarningTriangle.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
            resources.ApplyResources(this.pictureBoxWarningTriangle, "pictureBoxWarningTriangle");
            this.pictureBoxWarningTriangle.Name = "pictureBoxWarningTriangle";
            this.pictureBoxWarningTriangle.TabStop = false;
            // 
            // labelStatus
            // 
            resources.ApplyResources(this.labelStatus, "labelStatus");
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.UseMnemonic = false;
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.buttonConfigure);
            this.flowLayoutPanel1.Controls.Add(this.buttonDisableHa);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // buttonConfigure
            // 
            resources.ApplyResources(this.buttonConfigure, "buttonConfigure");
            this.buttonConfigure.Name = "buttonConfigure";
            this.buttonConfigure.UseVisualStyleBackColor = true;
            this.buttonConfigure.Click += new System.EventHandler(this.buttonConfigure_Click);
            // 
            // buttonDisableHa
            // 
            resources.ApplyResources(this.buttonDisableHa, "buttonDisableHa");
            this.buttonDisableHa.Name = "buttonDisableHa";
            this.buttonDisableHa.UseVisualStyleBackColor = true;
            this.buttonDisableHa.Click += new System.EventHandler(this.buttonDisableHa_Click);
            // 
            // tableLatencies
            // 
            resources.ApplyResources(this.tableLatencies, "tableLatencies");
            this.tableLatencies.Name = "tableLatencies";
            // 
            // customListPanel
            // 
            this.customListPanel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.customListPanel, "customListPanel");
            this.customListPanel.Name = "customListPanel";
            // 
            // TitleLabel
            // 
            resources.ApplyResources(this.TitleLabel, "TitleLabel");
            this.TitleLabel.ForeColor = System.Drawing.Color.White;
            this.TitleLabel.Name = "TitleLabel";
            // 
            // HAPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Name = "HAPage";
            this.pageContainerPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarningTriangle)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.PictureBox pictureBoxWarningTriangle;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button buttonDisableHa;
        private System.Windows.Forms.Button buttonConfigure;
        private System.Windows.Forms.TableLayoutPanel tableLatencies;
        private XenAdmin.Controls.CustomListPanel customListPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}
