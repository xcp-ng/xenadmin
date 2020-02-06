namespace XenAdmin.Wizards.ImportWizard
{
    partial class ImportSecurityPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportSecurityPage));
            this.m_labelVerify = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_checkBoxVerify = new System.Windows.Forms.CheckBox();
            this.lblSecurityIntro = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_linkCertificate = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_ctrlError = new XenAdmin.Controls.Common.PasswordFailure();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_labelVerify
            // 
            resources.ApplyResources(this.m_labelVerify, "m_labelVerify");
            this.tableLayoutPanel1.SetColumnSpan(this.m_labelVerify, 2);
            this.m_labelVerify.Name = "m_labelVerify";
            // 
            // m_checkBoxVerify
            // 
            resources.ApplyResources(this.m_checkBoxVerify, "m_checkBoxVerify");
            this.m_checkBoxVerify.Name = "m_checkBoxVerify";
            this.m_checkBoxVerify.UseVisualStyleBackColor = true;
            this.m_checkBoxVerify.CheckedChanged += new System.EventHandler(this.m_checkBoxVerify_CheckedChanged);
            // 
            // lblSecurityIntro
            // 
            resources.ApplyResources(this.lblSecurityIntro, "lblSecurityIntro");
            this.tableLayoutPanel1.SetColumnSpan(this.lblSecurityIntro, 2);
            this.lblSecurityIntro.Name = "lblSecurityIntro";
            // 
            // m_linkCertificate
            // 
            resources.ApplyResources(this.m_linkCertificate, "m_linkCertificate");
            this.m_linkCertificate.Name = "m_linkCertificate";
            this.m_linkCertificate.TabStop = true;
            this.m_linkCertificate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.m_linkCertificate_LinkClicked);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.m_linkCertificate, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_checkBoxVerify, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_labelVerify, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblSecurityIntro, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_ctrlError, 0, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // m_ctrlError
            // 
            resources.ApplyResources(this.m_ctrlError, "m_ctrlError");
            this.tableLayoutPanel1.SetColumnSpan(this.m_ctrlError, 2);
            this.m_ctrlError.Name = "m_ctrlError";
            // 
            // ImportSecurityPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ImportSecurityPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

		private XenAdmin.Controls.Common.AutoHeightLabel m_labelVerify;
		private System.Windows.Forms.CheckBox m_checkBoxVerify;
		private XenAdmin.Controls.Common.AutoHeightLabel lblSecurityIntro;
        private System.Windows.Forms.LinkLabel m_linkCertificate;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.Common.PasswordFailure m_ctrlError;
    }
}
