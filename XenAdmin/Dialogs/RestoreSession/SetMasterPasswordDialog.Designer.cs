namespace XenAdmin.Dialogs.RestoreSession
{
    partial class SetMasterPasswordDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetMasterPasswordDialog));
            this.noteLabel = new System.Windows.Forms.Label();
            this.reEnterMasterLabel = new System.Windows.Forms.Label();
            this.masterLabel = new System.Windows.Forms.Label();
            this.reEnterMasterTextBox = new System.Windows.Forms.TextBox();
            this.masterTextBox = new System.Windows.Forms.TextBox();
            this.masterBlurbLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.newPasswordError = new XenAdmin.Controls.Common.PasswordFailure();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // noteLabel
            // 
            resources.ApplyResources(this.noteLabel, "noteLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.noteLabel, 2);
            this.noteLabel.Name = "noteLabel";
            // 
            // reEnterMasterLabel
            // 
            resources.ApplyResources(this.reEnterMasterLabel, "reEnterMasterLabel");
            this.reEnterMasterLabel.Name = "reEnterMasterLabel";
            // 
            // masterLabel
            // 
            resources.ApplyResources(this.masterLabel, "masterLabel");
            this.masterLabel.Name = "masterLabel";
            // 
            // reEnterMasterTextBox
            // 
            resources.ApplyResources(this.reEnterMasterTextBox, "reEnterMasterTextBox");
            this.reEnterMasterTextBox.Name = "reEnterMasterTextBox";
            this.reEnterMasterTextBox.UseSystemPasswordChar = true;
            this.reEnterMasterTextBox.TextChanged += new System.EventHandler(this.reEnterMasterTextBox_TextChanged);
            // 
            // masterTextBox
            // 
            resources.ApplyResources(this.masterTextBox, "masterTextBox");
            this.masterTextBox.Name = "masterTextBox";
            this.masterTextBox.UseSystemPasswordChar = true;
            this.masterTextBox.TextChanged += new System.EventHandler(this.masterTextBox_TextChanged);
            // 
            // masterBlurbLabel
            // 
            resources.ApplyResources(this.masterBlurbLabel, "masterBlurbLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.masterBlurbLabel, 2);
            this.masterBlurbLabel.Name = "masterBlurbLabel";
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // newPasswordError
            // 
            resources.ApplyResources(this.newPasswordError, "newPasswordError");
            this.newPasswordError.Name = "newPasswordError";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.masterLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.newPasswordError, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.masterTextBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.noteLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.reEnterMasterTextBox, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.reEnterMasterLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.masterBlurbLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.cancelButton);
            this.flowLayoutPanel1.Controls.Add(this.okButton);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // SetMasterPasswordDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SetMasterPasswordDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label noteLabel;
        private System.Windows.Forms.Label reEnterMasterLabel;
        private System.Windows.Forms.Label masterLabel;
        private System.Windows.Forms.TextBox reEnterMasterTextBox;
        private System.Windows.Forms.TextBox masterTextBox;
        private System.Windows.Forms.Label masterBlurbLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private XenAdmin.Controls.Common.PasswordFailure newPasswordError;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}