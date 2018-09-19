namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class LVMoISCSI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LVMoISCSI));
            this.lunInUseLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.iSCSITargetGroupBox = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.labelIscsiIQN = new System.Windows.Forms.Label();
            this.comboBoxIscsiIqns = new System.Windows.Forms.ComboBox();
            this.comboBoxIscsiLuns = new System.Windows.Forms.ComboBox();
            this.targetLunLabel = new System.Windows.Forms.Label();
            this.errorLabelAtTargetLUN = new System.Windows.Forms.Label();
            this.errorIconAtTargetLUN = new System.Windows.Forms.PictureBox();
            this.spinnerIconAtTargetIqn = new XenAdmin.Controls.SpinnerIcon();
            this.spinnerIconAtTargetLun = new XenAdmin.Controls.SpinnerIcon();
            this.errorLabelAtCHAPPassword = new System.Windows.Forms.Label();
            this.errorIconAtCHAPPassword = new System.Windows.Forms.PictureBox();
            this.errorIconAtHostOrIP = new System.Windows.Forms.PictureBox();
            this.scanTargetHostButton = new System.Windows.Forms.Button();
            this.labelIscsiTargetHost = new System.Windows.Forms.Label();
            this.textBoxIscsiHost = new System.Windows.Forms.TextBox();
            this.labelColon = new System.Windows.Forms.Label();
            this.textBoxIscsiPort = new System.Windows.Forms.TextBox();
            this.IScsiChapUserTextBox = new System.Windows.Forms.TextBox();
            this.IScsiChapSecretTextBox = new System.Windows.Forms.TextBox();
            this.labelCHAPuser = new System.Windows.Forms.Label();
            this.IScsiChapSecretLabel = new System.Windows.Forms.Label();
            this.IscsiUseChapCheckBox = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.errorLabelAtHostname = new System.Windows.Forms.Label();
            this.spinnerIconAtScanTargetHostButton = new XenAdmin.Controls.SpinnerIcon();
            this.toolTipContainerIQNscan = new XenAdmin.Controls.ToolTipContainer();
            this.tableLayoutPanel1.SuspendLayout();
            this.iSCSITargetGroupBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtTargetLUN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtTargetIqn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtTargetLun)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtCHAPPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtHostOrIP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtScanTargetHostButton)).BeginInit();
            this.SuspendLayout();
            // 
            // lunInUseLabel
            // 
            resources.ApplyResources(this.lunInUseLabel, "lunInUseLabel");
            this.lunInUseLabel.Name = "lunInUseLabel";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.iSCSITargetGroupBox, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.errorLabelAtCHAPPassword, 3, 6);
            this.tableLayoutPanel1.Controls.Add(this.errorIconAtCHAPPassword, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.errorIconAtHostOrIP, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.scanTargetHostButton, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelIscsiTargetHost, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxIscsiHost, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelColon, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxIscsiPort, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.IScsiChapUserTextBox, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.IScsiChapSecretTextBox, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelCHAPuser, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.IScsiChapSecretLabel, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.IscsiUseChapCheckBox, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.errorLabelAtHostname, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.spinnerIconAtScanTargetHostButton, 2, 7);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // iSCSITargetGroupBox
            // 
            resources.ApplyResources(this.iSCSITargetGroupBox, "iSCSITargetGroupBox");
            this.tableLayoutPanel1.SetColumnSpan(this.iSCSITargetGroupBox, 6);
            this.iSCSITargetGroupBox.Controls.Add(this.tableLayoutPanel2);
            this.iSCSITargetGroupBox.Name = "iSCSITargetGroupBox";
            this.iSCSITargetGroupBox.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.labelIscsiIQN, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.comboBoxIscsiIqns, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.comboBoxIscsiLuns, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.targetLunLabel, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.errorLabelAtTargetLUN, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.errorIconAtTargetLUN, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.spinnerIconAtTargetIqn, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.spinnerIconAtTargetLun, 3, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // labelIscsiIQN
            // 
            resources.ApplyResources(this.labelIscsiIQN, "labelIscsiIQN");
            this.labelIscsiIQN.BackColor = System.Drawing.Color.Transparent;
            this.labelIscsiIQN.Name = "labelIscsiIQN";
            // 
            // comboBoxIscsiIqns
            // 
            resources.ApplyResources(this.comboBoxIscsiIqns, "comboBoxIscsiIqns");
            this.tableLayoutPanel2.SetColumnSpan(this.comboBoxIscsiIqns, 2);
            this.comboBoxIscsiIqns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIscsiIqns.FormattingEnabled = true;
            this.comboBoxIscsiIqns.Name = "comboBoxIscsiIqns";
            this.comboBoxIscsiIqns.SelectedIndexChanged += new System.EventHandler(this.IScsiTargetIqnComboBox_SelectedIndexChanged);
            // 
            // comboBoxIscsiLuns
            // 
            resources.ApplyResources(this.comboBoxIscsiLuns, "comboBoxIscsiLuns");
            this.tableLayoutPanel2.SetColumnSpan(this.comboBoxIscsiLuns, 2);
            this.comboBoxIscsiLuns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIscsiLuns.FormattingEnabled = true;
            this.comboBoxIscsiLuns.Name = "comboBoxIscsiLuns";
            this.comboBoxIscsiLuns.SelectedIndexChanged += new System.EventHandler(this.comboBoxIscsiLuns_SelectedIndexChanged);
            // 
            // targetLunLabel
            // 
            resources.ApplyResources(this.targetLunLabel, "targetLunLabel");
            this.targetLunLabel.Name = "targetLunLabel";
            // 
            // errorLabelAtTargetLUN
            // 
            resources.ApplyResources(this.errorLabelAtTargetLUN, "errorLabelAtTargetLUN");
            this.errorLabelAtTargetLUN.AutoEllipsis = true;
            this.tableLayoutPanel2.SetColumnSpan(this.errorLabelAtTargetLUN, 2);
            this.errorLabelAtTargetLUN.ForeColor = System.Drawing.Color.Red;
            this.errorLabelAtTargetLUN.Name = "errorLabelAtTargetLUN";
            // 
            // errorIconAtTargetLUN
            // 
            resources.ApplyResources(this.errorIconAtTargetLUN, "errorIconAtTargetLUN");
            this.errorIconAtTargetLUN.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            this.errorIconAtTargetLUN.Name = "errorIconAtTargetLUN";
            this.errorIconAtTargetLUN.TabStop = false;
            // 
            // spinnerIconAtTargetIqn
            // 
            resources.ApplyResources(this.spinnerIconAtTargetIqn, "spinnerIconAtTargetIqn");
            this.spinnerIconAtTargetIqn.Name = "spinnerIconAtTargetIqn";
            this.spinnerIconAtTargetIqn.SucceededImage = global::XenAdmin.Properties.Resources._000_Tick_h32bit_16;
            this.spinnerIconAtTargetIqn.TabStop = false;
            // 
            // spinnerIconAtTargetLun
            // 
            resources.ApplyResources(this.spinnerIconAtTargetLun, "spinnerIconAtTargetLun");
            this.spinnerIconAtTargetLun.Name = "spinnerIconAtTargetLun";
            this.spinnerIconAtTargetLun.SucceededImage = global::XenAdmin.Properties.Resources._000_Tick_h32bit_16;
            this.spinnerIconAtTargetLun.TabStop = false;
            // 
            // errorLabelAtCHAPPassword
            // 
            resources.ApplyResources(this.errorLabelAtCHAPPassword, "errorLabelAtCHAPPassword");
            this.errorLabelAtCHAPPassword.AutoEllipsis = true;
            this.tableLayoutPanel1.SetColumnSpan(this.errorLabelAtCHAPPassword, 3);
            this.errorLabelAtCHAPPassword.ForeColor = System.Drawing.Color.Red;
            this.errorLabelAtCHAPPassword.Name = "errorLabelAtCHAPPassword";
            // 
            // errorIconAtCHAPPassword
            // 
            resources.ApplyResources(this.errorIconAtCHAPPassword, "errorIconAtCHAPPassword");
            this.errorIconAtCHAPPassword.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            this.errorIconAtCHAPPassword.Name = "errorIconAtCHAPPassword";
            this.errorIconAtCHAPPassword.TabStop = false;
            // 
            // errorIconAtHostOrIP
            // 
            resources.ApplyResources(this.errorIconAtHostOrIP, "errorIconAtHostOrIP");
            this.errorIconAtHostOrIP.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            this.errorIconAtHostOrIP.Name = "errorIconAtHostOrIP";
            this.errorIconAtHostOrIP.TabStop = false;
            // 
            // scanTargetHostButton
            // 
            resources.ApplyResources(this.scanTargetHostButton, "scanTargetHostButton");
            this.tableLayoutPanel1.SetColumnSpan(this.scanTargetHostButton, 2);
            this.scanTargetHostButton.Name = "scanTargetHostButton";
            this.scanTargetHostButton.Click += new System.EventHandler(this.scanTargetHostButton_Click);
            // 
            // labelIscsiTargetHost
            // 
            resources.ApplyResources(this.labelIscsiTargetHost, "labelIscsiTargetHost");
            this.labelIscsiTargetHost.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.SetColumnSpan(this.labelIscsiTargetHost, 2);
            this.labelIscsiTargetHost.Name = "labelIscsiTargetHost";
            // 
            // textBoxIscsiHost
            // 
            resources.ApplyResources(this.textBoxIscsiHost, "textBoxIscsiHost");
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxIscsiHost, 2);
            this.textBoxIscsiHost.Name = "textBoxIscsiHost";
            this.textBoxIscsiHost.TextChanged += new System.EventHandler(this.textBoxIscsiHost_TextChanged);
            // 
            // labelColon
            // 
            resources.ApplyResources(this.labelColon, "labelColon");
            this.labelColon.Name = "labelColon";
            // 
            // textBoxIscsiPort
            // 
            resources.ApplyResources(this.textBoxIscsiPort, "textBoxIscsiPort");
            this.textBoxIscsiPort.Name = "textBoxIscsiPort";
            this.textBoxIscsiPort.TextChanged += new System.EventHandler(this.textBoxIscsiHost_TextChanged);
            // 
            // IScsiChapUserTextBox
            // 
            this.IScsiChapUserTextBox.AllowDrop = true;
            resources.ApplyResources(this.IScsiChapUserTextBox, "IScsiChapUserTextBox");
            this.tableLayoutPanel1.SetColumnSpan(this.IScsiChapUserTextBox, 2);
            this.IScsiChapUserTextBox.Name = "IScsiChapUserTextBox";
            this.IScsiChapUserTextBox.TextChanged += new System.EventHandler(this.ChapSettings_Changed);
            // 
            // IScsiChapSecretTextBox
            // 
            resources.ApplyResources(this.IScsiChapSecretTextBox, "IScsiChapSecretTextBox");
            this.tableLayoutPanel1.SetColumnSpan(this.IScsiChapSecretTextBox, 2);
            this.IScsiChapSecretTextBox.Name = "IScsiChapSecretTextBox";
            this.IScsiChapSecretTextBox.UseSystemPasswordChar = true;
            this.IScsiChapSecretTextBox.TextChanged += new System.EventHandler(this.ChapSettings_Changed);
            // 
            // labelCHAPuser
            // 
            resources.ApplyResources(this.labelCHAPuser, "labelCHAPuser");
            this.labelCHAPuser.BackColor = System.Drawing.Color.Transparent;
            this.labelCHAPuser.Name = "labelCHAPuser";
            // 
            // IScsiChapSecretLabel
            // 
            resources.ApplyResources(this.IScsiChapSecretLabel, "IScsiChapSecretLabel");
            this.IScsiChapSecretLabel.BackColor = System.Drawing.Color.Transparent;
            this.IScsiChapSecretLabel.Name = "IScsiChapSecretLabel";
            // 
            // IscsiUseChapCheckBox
            // 
            resources.ApplyResources(this.IscsiUseChapCheckBox, "IscsiUseChapCheckBox");
            this.tableLayoutPanel1.SetColumnSpan(this.IscsiUseChapCheckBox, 6);
            this.IscsiUseChapCheckBox.Name = "IscsiUseChapCheckBox";
            this.IscsiUseChapCheckBox.UseVisualStyleBackColor = true;
            this.IscsiUseChapCheckBox.CheckedChanged += new System.EventHandler(this.IscsiUseChapCheckBox_CheckedChanged);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.tableLayoutPanel1.SetColumnSpan(this.label11, 6);
            this.label11.Name = "label11";
            // 
            // errorLabelAtHostname
            // 
            resources.ApplyResources(this.errorLabelAtHostname, "errorLabelAtHostname");
            this.errorLabelAtHostname.AutoEllipsis = true;
            this.tableLayoutPanel1.SetColumnSpan(this.errorLabelAtHostname, 3);
            this.errorLabelAtHostname.ForeColor = System.Drawing.Color.Red;
            this.errorLabelAtHostname.Name = "errorLabelAtHostname";
            // 
            // spinnerIconAtScanTargetHostButton
            // 
            resources.ApplyResources(this.spinnerIconAtScanTargetHostButton, "spinnerIconAtScanTargetHostButton");
            this.spinnerIconAtScanTargetHostButton.Name = "spinnerIconAtScanTargetHostButton";
            this.spinnerIconAtScanTargetHostButton.SucceededImage = global::XenAdmin.Properties.Resources._000_Tick_h32bit_16;
            this.spinnerIconAtScanTargetHostButton.TabStop = false;
            // 
            // toolTipContainerIQNscan
            // 
            resources.ApplyResources(this.toolTipContainerIQNscan, "toolTipContainerIQNscan");
            this.toolTipContainerIQNscan.Name = "toolTipContainerIQNscan";
            // 
            // LVMoISCSI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolTipContainerIQNscan);
            this.Name = "LVMoISCSI";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.iSCSITargetGroupBox.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtTargetLUN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtTargetIqn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtTargetLun)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtCHAPPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorIconAtHostOrIP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIconAtScanTargetHostButton)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxIscsiPort;
        private System.Windows.Forms.Label labelColon;
        private XenAdmin.Controls.ToolTipContainer toolTipContainerIQNscan;
        private System.Windows.Forms.Button scanTargetHostButton;
        private System.Windows.Forms.CheckBox IscsiUseChapCheckBox;
        private System.Windows.Forms.ComboBox comboBoxIscsiIqns;
        private System.Windows.Forms.ComboBox comboBoxIscsiLuns;
        private System.Windows.Forms.Label errorLabelAtHostname;
        private System.Windows.Forms.Label lunInUseLabel;
        private System.Windows.Forms.Label targetLunLabel;
        private System.Windows.Forms.Label IScsiChapSecretLabel;
        private System.Windows.Forms.TextBox IScsiChapSecretTextBox;
        private System.Windows.Forms.Label labelCHAPuser;
        private System.Windows.Forms.TextBox IScsiChapUserTextBox;
        private System.Windows.Forms.Label labelIscsiTargetHost;
        private System.Windows.Forms.Label labelIscsiIQN;
        private System.Windows.Forms.TextBox textBoxIscsiHost;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private XenAdmin.Controls.DecentGroupBox iSCSITargetGroupBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.PictureBox errorIconAtTargetLUN;
        private System.Windows.Forms.PictureBox errorIconAtHostOrIP;
        private System.Windows.Forms.PictureBox errorIconAtCHAPPassword;
        private System.Windows.Forms.Label errorLabelAtCHAPPassword;
        private System.Windows.Forms.Label errorLabelAtTargetLUN;
        private XenAdmin.Controls.SpinnerIcon spinnerIconAtTargetIqn;
        private XenAdmin.Controls.SpinnerIcon spinnerIconAtTargetLun;
        private XenAdmin.Controls.SpinnerIcon spinnerIconAtScanTargetHostButton;
    }
}
