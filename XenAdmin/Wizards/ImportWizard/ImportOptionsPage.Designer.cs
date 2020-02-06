namespace XenAdmin.Wizards.ImportWizard
{
    partial class ImportOptionsPage
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
                DeregisterEvents();
                if (components != null)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportOptionsPage));
            this.m_ctrlError = new XenAdmin.Controls.Common.PasswordFailure();
            this.m_labelIntro = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.m_radioButtonDontRunOSFixups = new System.Windows.Forms.RadioButton();
            this.m_labelDontRunOSFixups = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_radioButtonRunOSFixups = new System.Windows.Forms.RadioButton();
            this.m_labelRunOSFixups = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.m_labelLocationFixupISO = new System.Windows.Forms.Label();
            this.m_comboBoxISOLibraries = new XenAdmin.Controls.LongStringComboBox();
            this.m_pictureBoxInfo = new System.Windows.Forms.PictureBox();
            this.m_labelFixupISOInfo = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBoxInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // m_ctrlError
            // 
            resources.ApplyResources(this.m_ctrlError, "m_ctrlError");
            this.tableLayoutPanel2.SetColumnSpan(this.m_ctrlError, 2);
            this.m_ctrlError.Name = "m_ctrlError";
            // 
            // m_labelIntro
            // 
            resources.ApplyResources(this.m_labelIntro, "m_labelIntro");
            this.tableLayoutPanel2.SetColumnSpan(this.m_labelIntro, 4);
            this.m_labelIntro.Name = "m_labelIntro";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.m_labelIntro, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.m_radioButtonDontRunOSFixups, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.m_labelDontRunOSFixups, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.m_radioButtonRunOSFixups, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.m_labelRunOSFixups, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.m_labelLocationFixupISO, 1, 6);
            this.tableLayoutPanel2.Controls.Add(this.m_comboBoxISOLibraries, 2, 6);
            this.tableLayoutPanel2.Controls.Add(this.m_pictureBoxInfo, 2, 7);
            this.tableLayoutPanel2.Controls.Add(this.m_labelFixupISOInfo, 3, 7);
            this.tableLayoutPanel2.Controls.Add(this.m_ctrlError, 2, 8);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // m_radioButtonDontRunOSFixups
            // 
            resources.ApplyResources(this.m_radioButtonDontRunOSFixups, "m_radioButtonDontRunOSFixups");
            this.tableLayoutPanel2.SetColumnSpan(this.m_radioButtonDontRunOSFixups, 4);
            this.m_radioButtonDontRunOSFixups.Name = "m_radioButtonDontRunOSFixups";
            this.m_radioButtonDontRunOSFixups.UseVisualStyleBackColor = true;
            this.m_radioButtonDontRunOSFixups.CheckedChanged += new System.EventHandler(this.m_radioButtonDontRunOSFixups_CheckedChanged);
            // 
            // m_labelDontRunOSFixups
            // 
            resources.ApplyResources(this.m_labelDontRunOSFixups, "m_labelDontRunOSFixups");
            this.tableLayoutPanel2.SetColumnSpan(this.m_labelDontRunOSFixups, 3);
            this.m_labelDontRunOSFixups.Name = "m_labelDontRunOSFixups";
            // 
            // m_radioButtonRunOSFixups
            // 
            resources.ApplyResources(this.m_radioButtonRunOSFixups, "m_radioButtonRunOSFixups");
            this.tableLayoutPanel2.SetColumnSpan(this.m_radioButtonRunOSFixups, 4);
            this.m_radioButtonRunOSFixups.Name = "m_radioButtonRunOSFixups";
            this.m_radioButtonRunOSFixups.UseVisualStyleBackColor = true;
            this.m_radioButtonRunOSFixups.CheckedChanged += new System.EventHandler(this.m_radioButtonRunOSFixups_CheckedChanged);
            // 
            // m_labelRunOSFixups
            // 
            resources.ApplyResources(this.m_labelRunOSFixups, "m_labelRunOSFixups");
            this.tableLayoutPanel2.SetColumnSpan(this.m_labelRunOSFixups, 3);
            this.m_labelRunOSFixups.Name = "m_labelRunOSFixups";
            // 
            // m_labelLocationFixupISO
            // 
            resources.ApplyResources(this.m_labelLocationFixupISO, "m_labelLocationFixupISO");
            this.m_labelLocationFixupISO.Name = "m_labelLocationFixupISO";
            // 
            // m_comboBoxISOLibraries
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.m_comboBoxISOLibraries, 2);
            this.m_comboBoxISOLibraries.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboBoxISOLibraries.FormattingEnabled = true;
            resources.ApplyResources(this.m_comboBoxISOLibraries, "m_comboBoxISOLibraries");
            this.m_comboBoxISOLibraries.Name = "m_comboBoxISOLibraries";
            this.m_comboBoxISOLibraries.SelectedIndexChanged += new System.EventHandler(this.m_comboBoxISOLibraries_SelectedIndexChanged);
            // 
            // m_pictureBoxInfo
            // 
            this.m_pictureBoxInfo.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.m_pictureBoxInfo, "m_pictureBoxInfo");
            this.m_pictureBoxInfo.Name = "m_pictureBoxInfo";
            this.m_pictureBoxInfo.TabStop = false;
            // 
            // m_labelFixupISOInfo
            // 
            resources.ApplyResources(this.m_labelFixupISOInfo, "m_labelFixupISOInfo");
            this.m_labelFixupISOInfo.Name = "m_labelFixupISOInfo";
            // 
            // ImportOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "ImportOptionsPage";
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBoxInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.Common.AutoHeightLabel m_labelIntro;
        private XenAdmin.Controls.Common.PasswordFailure m_ctrlError;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.RadioButton m_radioButtonDontRunOSFixups;
        private XenAdmin.Controls.Common.AutoHeightLabel m_labelDontRunOSFixups;
        private System.Windows.Forms.RadioButton m_radioButtonRunOSFixups;
        private XenAdmin.Controls.Common.AutoHeightLabel m_labelRunOSFixups;
        private System.Windows.Forms.Label m_labelLocationFixupISO;
        private XenAdmin.Controls.LongStringComboBox m_comboBoxISOLibraries;
        private System.Windows.Forms.PictureBox m_pictureBoxInfo;
        private XenAdmin.Controls.Common.AutoHeightLabel m_labelFixupISOInfo;
    }
}
