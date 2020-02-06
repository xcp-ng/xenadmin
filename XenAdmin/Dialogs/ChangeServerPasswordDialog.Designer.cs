namespace XenAdmin.Dialogs
{
    partial class ChangeServerPasswordDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeServerPasswordDialog));
            this.OldPasswordLabel = new System.Windows.Forms.Label();
            this.oldPassBox = new System.Windows.Forms.TextBox();
            this.NewPasswordLabel = new System.Windows.Forms.Label();
            this.newPassBox = new System.Windows.Forms.TextBox();
            this.confirmBox = new System.Windows.Forms.TextBox();
            this.RetypeNewPasswordLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ServerNameLabel = new System.Windows.Forms.Label();
            this.currentPasswordError = new XenAdmin.Controls.Common.PasswordFailure();
            this.newPasswordError = new XenAdmin.Controls.Common.PasswordFailure();
            this.noteLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OldPasswordLabel
            // 
            resources.ApplyResources(this.OldPasswordLabel, "OldPasswordLabel");
            this.OldPasswordLabel.Name = "OldPasswordLabel";
            // 
            // oldPassBox
            // 
            resources.ApplyResources(this.oldPassBox, "oldPassBox");
            this.tableLayoutPanel1.SetColumnSpan(this.oldPassBox, 2);
            this.oldPassBox.Name = "oldPassBox";
            this.oldPassBox.UseSystemPasswordChar = true;
            this.oldPassBox.TextChanged += new System.EventHandler(this.oldPassBox_TextChanged);
            // 
            // NewPasswordLabel
            // 
            resources.ApplyResources(this.NewPasswordLabel, "NewPasswordLabel");
            this.NewPasswordLabel.Name = "NewPasswordLabel";
            // 
            // newPassBox
            // 
            resources.ApplyResources(this.newPassBox, "newPassBox");
            this.tableLayoutPanel1.SetColumnSpan(this.newPassBox, 2);
            this.newPassBox.Name = "newPassBox";
            this.newPassBox.UseSystemPasswordChar = true;
            this.newPassBox.TextChanged += new System.EventHandler(this.newPassBox_TextChanged);
            // 
            // confirmBox
            // 
            resources.ApplyResources(this.confirmBox, "confirmBox");
            this.tableLayoutPanel1.SetColumnSpan(this.confirmBox, 2);
            this.confirmBox.Name = "confirmBox";
            this.confirmBox.UseSystemPasswordChar = true;
            this.confirmBox.TextChanged += new System.EventHandler(this.confirmBox_TextChanged);
            // 
            // RetypeNewPasswordLabel
            // 
            resources.ApplyResources(this.RetypeNewPasswordLabel, "RetypeNewPasswordLabel");
            this.RetypeNewPasswordLabel.Name = "RetypeNewPasswordLabel";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.ServerNameLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.OldPasswordLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.oldPassBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.NewPasswordLabel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.newPassBox, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.RetypeNewPasswordLabel, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.confirmBox, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.okButton, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.cancelButton, 2, 7);
            this.tableLayoutPanel1.Controls.Add(this.currentPasswordError, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.newPasswordError, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.noteLabel, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // ServerNameLabel
            // 
            resources.ApplyResources(this.ServerNameLabel, "ServerNameLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.ServerNameLabel, 3);
            this.ServerNameLabel.Name = "ServerNameLabel";
            // 
            // currentPasswordError
            // 
            resources.ApplyResources(this.currentPasswordError, "currentPasswordError");
            this.tableLayoutPanel1.SetColumnSpan(this.currentPasswordError, 2);
            this.currentPasswordError.Name = "currentPasswordError";
            // 
            // newPasswordError
            // 
            resources.ApplyResources(this.newPasswordError, "newPasswordError");
            this.tableLayoutPanel1.SetColumnSpan(this.newPasswordError, 2);
            this.newPasswordError.Name = "newPasswordError";
            // 
            // noteLabel
            // 
            resources.ApplyResources(this.noteLabel, "noteLabel");
            this.tableLayoutPanel1.SetColumnSpan(this.noteLabel, 3);
            this.noteLabel.Name = "noteLabel";
            // 
            // ChangeServerPasswordDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ChangeServerPasswordDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label OldPasswordLabel;
        private System.Windows.Forms.TextBox oldPassBox;
        private System.Windows.Forms.Label NewPasswordLabel;
        private System.Windows.Forms.TextBox newPassBox;
        private System.Windows.Forms.TextBox confirmBox;
        private System.Windows.Forms.Label RetypeNewPasswordLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label ServerNameLabel;
        private XenAdmin.Controls.Common.PasswordFailure currentPasswordError;
        private XenAdmin.Controls.Common.PasswordFailure newPasswordError;
        private System.Windows.Forms.Label noteLabel;
    }
}