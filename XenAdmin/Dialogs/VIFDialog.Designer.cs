namespace XenAdmin.Dialogs
{
    partial class VIFDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VIFDialog));
            this.Cancelbutton = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.promptTextBoxMac = new System.Windows.Forms.TextBox();
            this.radioButtonAutogenerate = new System.Windows.Forms.RadioButton();
            this.radioButtonMac = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanelBody = new System.Windows.Forms.TableLayoutPanel();
            this.labelBlurb = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelNetwork = new System.Windows.Forms.Label();
            this.comboBoxNetwork = new System.Windows.Forms.ComboBox();
            this.labelMAC = new System.Windows.Forms.Label();
            this.tableLayoutPanelMAC = new System.Windows.Forms.TableLayoutPanel();
            this.labelQoS = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.labelQoSUnits = new System.Windows.Forms.Label();
            this.promptTextBoxQoS = new System.Windows.Forms.TextBox();
            this.checkboxQoS = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanelInfo = new System.Windows.Forms.TableLayoutPanel();
            this.labelInfo = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanelError = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxError = new System.Windows.Forms.PictureBox();
            this.labelError = new System.Windows.Forms.Label();
            this.tableLayoutpanelButtons = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelBody.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelMAC.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanelInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanelError.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).BeginInit();
            this.tableLayoutpanelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // Cancelbutton
            // 
            this.Cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.Cancelbutton, "Cancelbutton");
            this.Cancelbutton.Name = "Cancelbutton";
            this.Cancelbutton.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // promptTextBoxMac
            // 
            resources.ApplyResources(this.promptTextBoxMac, "promptTextBoxMac");
            this.promptTextBoxMac.Name = "promptTextBoxMac";
            this.promptTextBoxMac.TextChanged += new System.EventHandler(this.promptTextBoxMac_TextChanged);
            this.promptTextBoxMac.Enter += new System.EventHandler(this.promptTextBoxMac_Enter);
            // 
            // radioButtonAutogenerate
            // 
            resources.ApplyResources(this.radioButtonAutogenerate, "radioButtonAutogenerate");
            this.tableLayoutPanelMAC.SetColumnSpan(this.radioButtonAutogenerate, 2);
            this.radioButtonAutogenerate.Name = "radioButtonAutogenerate";
            this.radioButtonAutogenerate.CheckedChanged += new System.EventHandler(this.radioButtonAutogenerate_CheckedChanged);
            // 
            // radioButtonMac
            // 
            resources.ApplyResources(this.radioButtonMac, "radioButtonMac");
            this.radioButtonMac.Name = "radioButtonMac";
            this.radioButtonMac.UseVisualStyleBackColor = true;
            this.radioButtonMac.CheckedChanged += new System.EventHandler(this.radioButtonMac_CheckedChanged);
            // 
            // tableLayoutPanelBody
            // 
            resources.ApplyResources(this.tableLayoutPanelBody, "tableLayoutPanelBody");
            this.tableLayoutPanelBody.Controls.Add(this.labelBlurb, 0, 0);
            this.tableLayoutPanelBody.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tableLayoutPanelBody.Controls.Add(this.labelMAC, 0, 2);
            this.tableLayoutPanelBody.Controls.Add(this.tableLayoutPanelMAC, 0, 3);
            this.tableLayoutPanelBody.Controls.Add(this.labelQoS, 0, 4);
            this.tableLayoutPanelBody.Controls.Add(this.tableLayoutPanel3, 0, 5);
            this.tableLayoutPanelBody.Controls.Add(this.tableLayoutPanelInfo, 0, 6);
            this.tableLayoutPanelBody.Controls.Add(this.tableLayoutPanelError, 0, 7);
            this.tableLayoutPanelBody.Controls.Add(this.tableLayoutpanelButtons, 0, 8);
            this.tableLayoutPanelBody.Name = "tableLayoutPanelBody";
            // 
            // labelBlurb
            // 
            resources.ApplyResources(this.labelBlurb, "labelBlurb");
            this.labelBlurb.Name = "labelBlurb";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelNetwork, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxNetwork, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelNetwork
            // 
            resources.ApplyResources(this.labelNetwork, "labelNetwork");
            this.labelNetwork.Name = "labelNetwork";
            // 
            // comboBoxNetwork
            // 
            resources.ApplyResources(this.comboBoxNetwork, "comboBoxNetwork");
            this.comboBoxNetwork.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNetwork.FormattingEnabled = true;
            this.comboBoxNetwork.Name = "comboBoxNetwork";
            this.comboBoxNetwork.SelectedIndexChanged += new System.EventHandler(this.comboBoxNetwork_SelectedIndexChanged);
            // 
            // labelMAC
            // 
            resources.ApplyResources(this.labelMAC, "labelMAC");
            this.labelMAC.Name = "labelMAC";
            // 
            // tableLayoutPanelMAC
            // 
            resources.ApplyResources(this.tableLayoutPanelMAC, "tableLayoutPanelMAC");
            this.tableLayoutPanelMAC.Controls.Add(this.radioButtonAutogenerate, 0, 0);
            this.tableLayoutPanelMAC.Controls.Add(this.radioButtonMac, 0, 1);
            this.tableLayoutPanelMAC.Controls.Add(this.promptTextBoxMac, 1, 1);
            this.tableLayoutPanelMAC.Name = "tableLayoutPanelMAC";
            // 
            // labelQoS
            // 
            resources.ApplyResources(this.labelQoS, "labelQoS");
            this.labelQoS.Name = "labelQoS";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.labelQoSUnits, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.promptTextBoxQoS, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.checkboxQoS, 0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // labelQoSUnits
            // 
            resources.ApplyResources(this.labelQoSUnits, "labelQoSUnits");
            this.labelQoSUnits.Name = "labelQoSUnits";
            // 
            // promptTextBoxQoS
            // 
            resources.ApplyResources(this.promptTextBoxQoS, "promptTextBoxQoS");
            this.promptTextBoxQoS.Name = "promptTextBoxQoS";
            this.promptTextBoxQoS.TextChanged += new System.EventHandler(this.promptTextBoxQoS_TextChanged);
            this.promptTextBoxQoS.Enter += new System.EventHandler(this.promptTextBoxQoS_Enter);
            // 
            // checkboxQoS
            // 
            resources.ApplyResources(this.checkboxQoS, "checkboxQoS");
            this.checkboxQoS.Name = "checkboxQoS";
            this.checkboxQoS.UseVisualStyleBackColor = true;
            this.checkboxQoS.CheckedChanged += new System.EventHandler(this.checkboxQoS_CheckedChanged);
            // 
            // tableLayoutPanelInfo
            // 
            resources.ApplyResources(this.tableLayoutPanelInfo, "tableLayoutPanelInfo");
            this.tableLayoutPanelInfo.Controls.Add(this.labelInfo, 1, 0);
            this.tableLayoutPanelInfo.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanelInfo.Name = "tableLayoutPanelInfo";
            // 
            // labelInfo
            // 
            resources.ApplyResources(this.labelInfo, "labelInfo");
            this.labelInfo.Name = "labelInfo";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // tableLayoutPanelError
            // 
            resources.ApplyResources(this.tableLayoutPanelError, "tableLayoutPanelError");
            this.tableLayoutPanelError.Controls.Add(this.pictureBoxError, 0, 0);
            this.tableLayoutPanelError.Controls.Add(this.labelError, 1, 0);
            this.tableLayoutPanelError.Name = "tableLayoutPanelError";
            // 
            // pictureBoxError
            // 
            resources.ApplyResources(this.pictureBoxError, "pictureBoxError");
            this.pictureBoxError.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            this.pictureBoxError.Name = "pictureBoxError";
            this.pictureBoxError.TabStop = false;
            // 
            // labelError
            // 
            resources.ApplyResources(this.labelError, "labelError");
            this.labelError.Name = "labelError";
            // 
            // tableLayoutpanelButtons
            // 
            resources.ApplyResources(this.tableLayoutpanelButtons, "tableLayoutpanelButtons");
            this.tableLayoutpanelButtons.Controls.Add(this.buttonOk, 0, 0);
            this.tableLayoutpanelButtons.Controls.Add(this.Cancelbutton, 1, 0);
            this.tableLayoutpanelButtons.Name = "tableLayoutpanelButtons";
            // 
            // VIFDialog
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.Cancelbutton;
            this.Controls.Add(this.tableLayoutPanelBody);
            this.Name = "VIFDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VIFDialog_FormClosing);
            this.tableLayoutPanelBody.ResumeLayout(false);
            this.tableLayoutPanelBody.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanelMAC.ResumeLayout(false);
            this.tableLayoutPanelMAC.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanelInfo.ResumeLayout(false);
            this.tableLayoutPanelInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanelError.ResumeLayout(false);
            this.tableLayoutPanelError.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).EndInit();
            this.tableLayoutpanelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Cancelbutton;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.TextBox promptTextBoxMac;
        private System.Windows.Forms.RadioButton radioButtonAutogenerate;
        private System.Windows.Forms.RadioButton radioButtonMac;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBody;
        private System.Windows.Forms.Label labelBlurb;
        private System.Windows.Forms.Label labelMAC;
        private System.Windows.Forms.Label labelQoS;
        private System.Windows.Forms.Label labelQoSUnits;
        private System.Windows.Forms.CheckBox checkboxQoS;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMAC;
        private System.Windows.Forms.TextBox promptTextBoxQoS;
        private System.Windows.Forms.TableLayoutPanel tableLayoutpanelButtons;
        private System.Windows.Forms.Label labelNetwork;
        private System.Windows.Forms.ComboBox comboBoxNetwork;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelInfo;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelError;
        private System.Windows.Forms.PictureBox pictureBoxError;
        private System.Windows.Forms.Label labelError;
    }
}