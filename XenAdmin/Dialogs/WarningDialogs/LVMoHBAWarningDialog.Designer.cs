namespace XenAdmin.Dialogs.WarningDialogs
{
    partial class LVMoHBAWarningDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LVMoHBAWarningDialog));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxRepeat = new System.Windows.Forms.CheckBox();
            this.labelHeader = new System.Windows.Forms.Label();
            this.labelSelectOption = new System.Windows.Forms.Label();
            this.panelReattach = new XenAdmin.Controls.FlickerFreePanel();
            this.pictureBoxArrowReattach = new System.Windows.Forms.PictureBox();
            this.buttonReattach = new System.Windows.Forms.Button();
            this.labelReattachInfo = new System.Windows.Forms.Label();
            this.panelFormat = new XenAdmin.Controls.FlickerFreePanel();
            this.pictureBoxArrowFormat = new System.Windows.Forms.PictureBox();
            this.buttonFormat = new System.Windows.Forms.Button();
            this.labelFormatInfo = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.labelWarning = new System.Windows.Forms.Label();
            this.labelLUNDetails = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelSrDetails = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panelReattach.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArrowReattach)).BeginInit();
            this.panelFormat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArrowFormat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // checkBoxRepeat
            // 
            resources.ApplyResources(this.checkBoxRepeat, "checkBoxRepeat");
            this.checkBoxRepeat.Name = "checkBoxRepeat";
            this.checkBoxRepeat.UseVisualStyleBackColor = true;
            // 
            // labelHeader
            // 
            resources.ApplyResources(this.labelHeader, "labelHeader");
            this.labelHeader.Name = "labelHeader";
            // 
            // labelSelectOption
            // 
            resources.ApplyResources(this.labelSelectOption, "labelSelectOption");
            this.labelSelectOption.Name = "labelSelectOption";
            // 
            // panelReattach
            // 
            resources.ApplyResources(this.panelReattach, "panelReattach");
            this.panelReattach.BackColor = System.Drawing.SystemColors.Control;
            this.panelReattach.BorderColor = System.Drawing.Color.Black;
            this.panelReattach.BorderWidth = 1;
            this.panelReattach.Controls.Add(this.pictureBoxArrowReattach);
            this.panelReattach.Controls.Add(this.buttonReattach);
            this.panelReattach.Controls.Add(this.labelReattachInfo);
            this.panelReattach.Name = "panelReattach";
            this.panelReattach.Click += new System.EventHandler(this.panelReattach_Click);
            this.panelReattach.MouseEnter += new System.EventHandler(this.panelReattach_MouseEnter);
            // 
            // pictureBoxArrowReattach
            // 
            resources.ApplyResources(this.pictureBoxArrowReattach, "pictureBoxArrowReattach");
            this.pictureBoxArrowReattach.Image = global::XenAdmin.Properties.Resources._112_RightArrowLong_Blue_24x24_72;
            this.pictureBoxArrowReattach.Name = "pictureBoxArrowReattach";
            this.pictureBoxArrowReattach.TabStop = false;
            this.pictureBoxArrowReattach.Click += new System.EventHandler(this.panelReattach_Click);
            this.pictureBoxArrowReattach.MouseEnter += new System.EventHandler(this.panelReattach_MouseEnter);
            // 
            // buttonReattach
            // 
            resources.ApplyResources(this.buttonReattach, "buttonReattach");
            this.buttonReattach.FlatAppearance.BorderSize = 0;
            this.buttonReattach.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.Control;
            this.buttonReattach.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Control;
            this.buttonReattach.ForeColor = System.Drawing.Color.Navy;
            this.buttonReattach.Name = "buttonReattach";
            this.buttonReattach.UseVisualStyleBackColor = true;
            this.buttonReattach.Click += new System.EventHandler(this.panelReattach_Click);
            this.buttonReattach.MouseEnter += new System.EventHandler(this.panelReattach_MouseEnter);
            // 
            // labelReattachInfo
            // 
            resources.ApplyResources(this.labelReattachInfo, "labelReattachInfo");
            this.labelReattachInfo.Name = "labelReattachInfo";
            this.labelReattachInfo.Click += new System.EventHandler(this.panelReattach_Click);
            this.labelReattachInfo.MouseEnter += new System.EventHandler(this.panelReattach_MouseEnter);
            // 
            // panelFormat
            // 
            resources.ApplyResources(this.panelFormat, "panelFormat");
            this.panelFormat.BorderColor = System.Drawing.Color.Black;
            this.panelFormat.BorderWidth = 1;
            this.panelFormat.Controls.Add(this.pictureBoxArrowFormat);
            this.panelFormat.Controls.Add(this.buttonFormat);
            this.panelFormat.Controls.Add(this.labelFormatInfo);
            this.panelFormat.Name = "panelFormat";
            this.panelFormat.Click += new System.EventHandler(this.panelFormat_Click);
            this.panelFormat.MouseEnter += new System.EventHandler(this.panelFormat_MouseEnter);
            // 
            // pictureBoxArrowFormat
            // 
            resources.ApplyResources(this.pictureBoxArrowFormat, "pictureBoxArrowFormat");
            this.pictureBoxArrowFormat.Image = global::XenAdmin.Properties.Resources._112_RightArrowLong_Blue_24x24_72;
            this.pictureBoxArrowFormat.Name = "pictureBoxArrowFormat";
            this.pictureBoxArrowFormat.TabStop = false;
            this.pictureBoxArrowFormat.Click += new System.EventHandler(this.panelFormat_Click);
            this.pictureBoxArrowFormat.MouseEnter += new System.EventHandler(this.panelFormat_MouseEnter);
            // 
            // buttonFormat
            // 
            resources.ApplyResources(this.buttonFormat, "buttonFormat");
            this.buttonFormat.FlatAppearance.BorderSize = 0;
            this.buttonFormat.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.Control;
            this.buttonFormat.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Control;
            this.buttonFormat.ForeColor = System.Drawing.Color.Navy;
            this.buttonFormat.Name = "buttonFormat";
            this.buttonFormat.UseVisualStyleBackColor = true;
            this.buttonFormat.Click += new System.EventHandler(this.panelFormat_Click);
            this.buttonFormat.MouseEnter += new System.EventHandler(this.panelFormat_MouseEnter);
            // 
            // labelFormatInfo
            // 
            resources.ApplyResources(this.labelFormatInfo, "labelFormatInfo");
            this.labelFormatInfo.Name = "labelFormatInfo";
            this.labelFormatInfo.Click += new System.EventHandler(this.panelFormat_Click);
            this.labelFormatInfo.MouseEnter += new System.EventHandler(this.panelFormat_MouseEnter);
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // labelWarning
            // 
            resources.ApplyResources(this.labelWarning, "labelWarning");
            this.labelWarning.Name = "labelWarning";
            // 
            // labelLUNDetails
            // 
            resources.ApplyResources(this.labelLUNDetails, "labelLUNDetails");
            this.labelLUNDetails.Name = "labelLUNDetails";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelLUNDetails, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelHeader, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelSrDetails, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelSelectOption, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.panelReattach, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.panelFormat, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxRepeat, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 0, 8);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelSrDetails
            // 
            resources.ApplyResources(this.labelSrDetails, "labelSrDetails");
            this.labelSrDetails.Name = "labelSrDetails";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.pictureBox2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelWarning, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // LVMoHBAWarningDialog
            // 
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LVMoHBAWarningDialog";
            this.MouseEnter += new System.EventHandler(this.LVMoHBAWarningDialog_MouseEnter);
            this.panelReattach.ResumeLayout(false);
            this.panelReattach.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArrowReattach)).EndInit();
            this.panelFormat.ResumeLayout(false);
            this.panelFormat.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArrowFormat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxRepeat;
        private System.Windows.Forms.Label labelHeader;
        private System.Windows.Forms.Label labelSelectOption;
        private XenAdmin.Controls.FlickerFreePanel panelReattach;
        protected XenAdmin.Controls.FlickerFreePanel panelFormat;
        protected System.Windows.Forms.PictureBox pictureBox2;
        protected System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.PictureBox pictureBoxArrowReattach;
        private System.Windows.Forms.PictureBox pictureBoxArrowFormat;
        private System.Windows.Forms.Label labelReattachInfo;
        private System.Windows.Forms.Label labelFormatInfo;
        protected System.Windows.Forms.Label labelLUNDetails;
        private System.Windows.Forms.Button buttonReattach;
        private System.Windows.Forms.Button buttonFormat;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        protected System.Windows.Forms.Label labelSrDetails;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}
