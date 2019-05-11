namespace XenAdmin.SettingsPanels
{
    partial class LogDestinationEditPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogDestinationEditPage));
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.ServerLabel = new System.Windows.Forms.Label();
            this.ServerTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxRemote = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel2.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // ServerLabel
            // 
            resources.ApplyResources(this.ServerLabel, "ServerLabel");
            this.ServerLabel.Name = "ServerLabel";
            // 
            // ServerTextBox
            // 
            resources.ApplyResources(this.ServerTextBox, "ServerTextBox");
            this.ServerTextBox.Name = "ServerTextBox";
            this.ServerTextBox.TextChanged += new System.EventHandler(this.ServerTextBox_TextChanged);
            this.ServerTextBox.Enter += new System.EventHandler(this.ServerTextBox_Enter);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxRemote, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ServerLabel, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.ServerTextBox, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.tableLayoutPanel1.SetColumnSpan(this.label3, 3);
            this.label3.Name = "label3";
            // 
            // checkBoxRemote
            // 
            resources.ApplyResources(this.checkBoxRemote, "checkBoxRemote");
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxRemote, 3);
            this.checkBoxRemote.Name = "checkBoxRemote";
            this.checkBoxRemote.UseVisualStyleBackColor = true;
            this.checkBoxRemote.CheckedChanged += new System.EventHandler(this.checkBoxRemote_CheckedChanged);
            // 
            // LogDestinationEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "LogDestinationEditPage";
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label ServerLabel;
        private System.Windows.Forms.TextBox ServerTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxRemote;
    }
}
