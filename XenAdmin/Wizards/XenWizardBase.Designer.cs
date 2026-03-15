namespace XenAdmin.Wizards
{
    partial class XenWizardBase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XenWizardBase));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelTop = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxWizard = new System.Windows.Forms.PictureBox();
            this.labelWizard = new System.Windows.Forms.Label();
            this.XSHelpButton = new XenAdmin.Controls.HelpButton();
            this.wizardProgress = new XenAdmin.Wizards.WizardProgress();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.xenTabControlBody = new XenAdmin.Controls.XenTabControl();
            this.deprecationBanner = new XenAdmin.Controls.DeprecationBanner();
            this.buttonPrevious = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanelNavigation = new System.Windows.Forms.TableLayoutPanel();
            this.buttonNext = new System.Windows.Forms.Button();
            this.panelGeneralInformationMessage = new System.Windows.Forms.Panel();
            this.labelGeneralInformationMessage = new System.Windows.Forms.Label();
            this.pictureBoxGeneralInformationMessage = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWizard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XSHelpButton)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanelNavigation.SuspendLayout();
            this.panelGeneralInformationMessage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGeneralInformationMessage)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panelTop, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.wizardProgress, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(251)))), ((int)(((byte)(251)))));
            resources.ApplyResources(this.panelTop, "panelTop");
            this.tableLayoutPanel1.SetColumnSpan(this.panelTop, 2);
            this.panelTop.Controls.Add(this.pictureBoxWizard, 0, 0);
            this.panelTop.Controls.Add(this.labelWizard, 1, 0);
            this.panelTop.Controls.Add(this.XSHelpButton, 2, 0);
            this.panelTop.Name = "panelTop";
            // 
            // pictureBoxWizard
            // 
            this.pictureBoxWizard.Image = global::XenAdmin.Properties.Resources._000_CreateVM_h32bit_32;
            resources.ApplyResources(this.pictureBoxWizard, "pictureBoxWizard");
            this.pictureBoxWizard.Name = "pictureBoxWizard";
            this.pictureBoxWizard.TabStop = false;
            // 
            // labelWizard
            // 
            this.labelWizard.AutoEllipsis = true;
            resources.ApplyResources(this.labelWizard, "labelWizard");
            this.labelWizard.ForeColor = System.Drawing.Color.Black;
            this.labelWizard.Name = "labelWizard";
            // 
            // wizardProgress
            // 
            resources.ApplyResources(this.wizardProgress, "wizardProgress");
            this.wizardProgress.Name = "wizardProgress";
            this.wizardProgress.TabStop = false;
            this.wizardProgress.LeavingStep += new System.EventHandler<XenAdmin.Wizards.WizardProgressEventArgs>(this.WizardProgress_LeavingStep);
            this.wizardProgress.EnteringStep += new System.EventHandler<XenAdmin.Wizards.WizardProgressEventArgs>(this.WizardProgress_EnteringStep);
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.xenTabControlBody, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.deprecationBanner, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // xenTabControlBody
            // 
            resources.ApplyResources(this.xenTabControlBody, "xenTabControlBody");
            this.xenTabControlBody.Name = "xenTabControlBody";
            this.xenTabControlBody.SelectedIndex = -1;
            this.xenTabControlBody.SelectedTab = null;
            this.xenTabControlBody.TabStop = false;
            // 
            // deprecationBanner
            // 
            resources.ApplyResources(this.deprecationBanner, "deprecationBanner");
            this.deprecationBanner.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.deprecationBanner.Name = "deprecationBanner";
            // 
            // buttonPrevious
            // 
            resources.ApplyResources(this.buttonPrevious, "buttonPrevious");
            this.buttonPrevious.Name = "buttonPrevious";
            this.buttonPrevious.UseVisualStyleBackColor = true;
            this.buttonPrevious.Click += new System.EventHandler(this.buttonPrevious_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // tableLayoutPanelNavigation
            // 
            resources.ApplyResources(this.tableLayoutPanelNavigation, "tableLayoutPanelNavigation");
            this.tableLayoutPanelNavigation.Controls.Add(this.buttonPrevious, 2, 0);
            this.tableLayoutPanelNavigation.Controls.Add(this.buttonNext, 3, 0);
            this.tableLayoutPanelNavigation.Controls.Add(this.buttonCancel, 4, 0);
            this.tableLayoutPanelNavigation.Controls.Add(this.panelGeneralInformationMessage, 0, 0);
            this.tableLayoutPanelNavigation.Name = "tableLayoutPanelNavigation";
            // 
            // buttonNext
            // 
            resources.ApplyResources(this.buttonNext, "buttonNext");
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // panelGeneralInformationMessage
            // 
            this.tableLayoutPanelNavigation.SetColumnSpan(this.panelGeneralInformationMessage, 2);
            this.panelGeneralInformationMessage.Controls.Add(this.labelGeneralInformationMessage);
            this.panelGeneralInformationMessage.Controls.Add(this.pictureBoxGeneralInformationMessage);
            resources.ApplyResources(this.panelGeneralInformationMessage, "panelGeneralInformationMessage");
            this.panelGeneralInformationMessage.Name = "panelGeneralInformationMessage";
            // 
            // labelGeneralInformationMessage
            // 
            resources.ApplyResources(this.labelGeneralInformationMessage, "labelGeneralInformationMessage");
            this.labelGeneralInformationMessage.Name = "labelGeneralInformationMessage";
            // 
            // pictureBoxGeneralInformationMessage
            // 
            this.pictureBoxGeneralInformationMessage.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            resources.ApplyResources(this.pictureBoxGeneralInformationMessage, "pictureBoxGeneralInformationMessage");
            this.pictureBoxGeneralInformationMessage.Name = "pictureBoxGeneralInformationMessage";
            this.pictureBoxGeneralInformationMessage.TabStop = false;
            // 
            // XenWizardBase
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.tableLayoutPanelNavigation);
            this.DoubleBuffered = true;
            this.Icon = global::XenAdmin.Properties.Resources.AppIcon;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "XenWizardBase";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.XenWizardBase_FormClosing);
            this.Load += new System.EventHandler(this.XenWizardBase_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.XenWizardBase_KeyPress);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWizard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.XSHelpButton)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanelNavigation.ResumeLayout(false);
            this.tableLayoutPanelNavigation.PerformLayout();
            this.panelGeneralInformationMessage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGeneralInformationMessage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelWizard;
        protected System.Windows.Forms.PictureBox pictureBoxWizard;
        private XenAdmin.Controls.XenTabControl xenTabControlBody;
        protected XenAdmin.Controls.HelpButton XSHelpButton;
        private System.Windows.Forms.Button buttonPrevious;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelNavigation;
        private System.Windows.Forms.Button buttonNext;
        private WizardProgress wizardProgress;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private XenAdmin.Controls.DeprecationBanner deprecationBanner;
        private System.Windows.Forms.TableLayoutPanel panelTop;
        private System.Windows.Forms.Panel panelGeneralInformationMessage;
        private System.Windows.Forms.Label labelGeneralInformationMessage;
        private System.Windows.Forms.PictureBox pictureBoxGeneralInformationMessage;
    }
}