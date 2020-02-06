namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{
    partial class CrossPoolMigrateTransferNetworkPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CrossPoolMigrateTransferNetworkPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.networkComboBox = new XenAdmin.Controls.NetworkComboBox();
            this.blurbText = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.blurbText, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.networkComboBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // networkComboBox
            // 
            resources.ApplyResources(this.networkComboBox, "networkComboBox");
            this.networkComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.networkComboBox.FormattingEnabled = true;
            this.networkComboBox.IncludeOnlyEnabledNetworksInComboBox = false;
            this.networkComboBox.IncludeOnlyNetworksWithIPAddresses = false;
            this.networkComboBox.IncludePoolNameInComboBox = false;
            this.networkComboBox.Name = "networkComboBox";
            // 
            // blurbText
            // 
            resources.ApplyResources(this.blurbText, "blurbText");
            this.tableLayoutPanel1.SetColumnSpan(this.blurbText, 2);
            this.blurbText.Name = "blurbText";
            // 
            // CrossPoolMigrateTransferNetworkPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CrossPoolMigrateTransferNetworkPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.NetworkComboBox networkComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label blurbText;
    }
}

