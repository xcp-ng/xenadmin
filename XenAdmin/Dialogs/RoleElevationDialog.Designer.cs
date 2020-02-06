namespace XenAdmin.Dialogs
{
    partial class RoleElevationDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoleElevationDialog));
            this.labelBlurb = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelCurrentRoleValue = new System.Windows.Forms.Label();
            this.labelCurrentRole = new System.Windows.Forms.Label();
            this.labelCurrentUserValue = new System.Windows.Forms.Label();
            this.labelCurrentUser = new System.Windows.Forms.Label();
            this.labelRequiredRole = new System.Windows.Forms.Label();
            this.labelRequiredRoleValue = new System.Windows.Forms.Label();
            this.TextBoxUsername = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.labelUserName = new System.Windows.Forms.Label();
            this.buttonAuthorize = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelCurrentAction = new System.Windows.Forms.Label();
            this.labelCurrentActionValue = new System.Windows.Forms.Label();
            this.labelServer = new System.Windows.Forms.Label();
            this.labelServerValue = new System.Windows.Forms.Label();
            this.divider = new System.Windows.Forms.GroupBox();
            this.labelBlurb2 = new System.Windows.Forms.Label();
            this.TextBoxPassword = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelBlurb
            // 
            resources.ApplyResources(this.labelBlurb, "labelBlurb");
            this.tableLayoutPanel1.SetColumnSpan(this.labelBlurb, 2);
            this.labelBlurb.Name = "labelBlurb";
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelCurrentRoleValue
            // 
            resources.ApplyResources(this.labelCurrentRoleValue, "labelCurrentRoleValue");
            this.labelCurrentRoleValue.Name = "labelCurrentRoleValue";
            // 
            // labelCurrentRole
            // 
            resources.ApplyResources(this.labelCurrentRole, "labelCurrentRole");
            this.labelCurrentRole.Name = "labelCurrentRole";
            // 
            // labelCurrentUserValue
            // 
            resources.ApplyResources(this.labelCurrentUserValue, "labelCurrentUserValue");
            this.labelCurrentUserValue.Name = "labelCurrentUserValue";
            // 
            // labelCurrentUser
            // 
            resources.ApplyResources(this.labelCurrentUser, "labelCurrentUser");
            this.labelCurrentUser.Name = "labelCurrentUser";
            // 
            // labelRequiredRole
            // 
            resources.ApplyResources(this.labelRequiredRole, "labelRequiredRole");
            this.labelRequiredRole.Name = "labelRequiredRole";
            // 
            // labelRequiredRoleValue
            // 
            resources.ApplyResources(this.labelRequiredRoleValue, "labelRequiredRoleValue");
            this.labelRequiredRoleValue.Name = "labelRequiredRoleValue";
            // 
            // TextBoxUsername
            // 
            resources.ApplyResources(this.TextBoxUsername, "TextBoxUsername");
            this.TextBoxUsername.Name = "TextBoxUsername";
            this.TextBoxUsername.TextChanged += new System.EventHandler(this.TextBoxUsername_TextChanged);
            // 
            // labelPassword
            // 
            resources.ApplyResources(this.labelPassword, "labelPassword");
            this.labelPassword.Name = "labelPassword";
            // 
            // labelUserName
            // 
            resources.ApplyResources(this.labelUserName, "labelUserName");
            this.labelUserName.Name = "labelUserName";
            // 
            // buttonAuthorize
            // 
            resources.ApplyResources(this.buttonAuthorize, "buttonAuthorize");
            this.buttonAuthorize.Name = "buttonAuthorize";
            this.buttonAuthorize.UseVisualStyleBackColor = true;
            this.buttonAuthorize.Click += new System.EventHandler(this.buttonAuthorize_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelBlurb, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelCurrentAction, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelCurrentActionValue, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelServer, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelServerValue, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelCurrentUser, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelCurrentUserValue, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelCurrentRole, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelCurrentRoleValue, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.divider, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelBlurb2, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelRequiredRole, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelRequiredRoleValue, 3, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelUserName, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.TextBoxUsername, 2, 8);
            this.tableLayoutPanel1.Controls.Add(this.labelPassword, 1, 9);
            this.tableLayoutPanel1.Controls.Add(this.TextBoxPassword, 2, 9);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 2, 10);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
            this.pictureBox1.Name = "pictureBox1";
            this.tableLayoutPanel1.SetRowSpan(this.pictureBox1, 2);
            this.pictureBox1.TabStop = false;
            // 
            // labelCurrentAction
            // 
            resources.ApplyResources(this.labelCurrentAction, "labelCurrentAction");
            this.labelCurrentAction.Name = "labelCurrentAction";
            // 
            // labelCurrentActionValue
            // 
            resources.ApplyResources(this.labelCurrentActionValue, "labelCurrentActionValue");
            this.labelCurrentActionValue.Name = "labelCurrentActionValue";
            // 
            // labelServer
            // 
            resources.ApplyResources(this.labelServer, "labelServer");
            this.labelServer.Name = "labelServer";
            // 
            // labelServerValue
            // 
            resources.ApplyResources(this.labelServerValue, "labelServerValue");
            this.labelServerValue.Name = "labelServerValue";
            // 
            // divider
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.divider, 2);
            resources.ApplyResources(this.divider, "divider");
            this.divider.Name = "divider";
            this.divider.TabStop = false;
            // 
            // labelBlurb2
            // 
            resources.ApplyResources(this.labelBlurb2, "labelBlurb2");
            this.tableLayoutPanel1.SetColumnSpan(this.labelBlurb2, 2);
            this.labelBlurb2.Name = "labelBlurb2";
            // 
            // TextBoxPassword
            // 
            resources.ApplyResources(this.TextBoxPassword, "TextBoxPassword");
            this.TextBoxPassword.Name = "TextBoxPassword";
            this.TextBoxPassword.UseSystemPasswordChar = true;
            this.TextBoxPassword.TextChanged += new System.EventHandler(this.TextBoxPassword_TextChanged);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.buttonAuthorize);
            this.panel1.Name = "panel1";
            // 
            // RoleElevationDialog
            // 
            this.AcceptButton = this.buttonAuthorize;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RoleElevationDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelBlurb;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelCurrentRoleValue;
        private System.Windows.Forms.Label labelCurrentRole;
        private System.Windows.Forms.Label labelCurrentUserValue;
        private System.Windows.Forms.Label labelCurrentUser;
        private System.Windows.Forms.Label labelRequiredRole;
        private System.Windows.Forms.Label labelRequiredRoleValue;
        private System.Windows.Forms.TextBox TextBoxUsername;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Label labelUserName;
        private System.Windows.Forms.Button buttonAuthorize;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox TextBoxPassword;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelBlurb2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox divider;
        private System.Windows.Forms.Label labelCurrentActionValue;
        private System.Windows.Forms.Label labelCurrentAction;
        private System.Windows.Forms.Label labelServer;
        private System.Windows.Forms.Label labelServerValue;
    }
}