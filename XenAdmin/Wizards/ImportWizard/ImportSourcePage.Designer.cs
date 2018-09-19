namespace XenAdmin.Wizards.ImportWizard
{
    partial class ImportSourcePage
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
                if (_webClient != null)
                {
                    _webClient.DownloadFileCompleted -= webclient_DownloadFileCompleted;
                    _webClient.DownloadProgressChanged -= webclient_DownloadProgressChanged;
                }

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportSourcePage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblIntro = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.m_textBoxFile = new System.Windows.Forms.TextBox();
            this.m_buttonBrowse = new System.Windows.Forms.Button();
            this.m_ctrlError = new XenAdmin.Controls.Common.PasswordFailure();
            this.m_tlpEncryption = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.m_tlpError = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this._labelError = new System.Windows.Forms.Label();
            this.labelProgress = new System.Windows.Forms.Label();
            this._unzipWorker = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.m_tlpEncryption.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.m_tlpError.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.lblIntro, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.m_tlpEncryption, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.m_tlpError, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelProgress, 0, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // lblIntro
            // 
            resources.ApplyResources(this.lblIntro, "lblIntro");
            this.lblIntro.Name = "lblIntro";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.m_textBoxFile, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.m_buttonBrowse, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.m_ctrlError, 1, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // m_textBoxFile
            // 
            resources.ApplyResources(this.m_textBoxFile, "m_textBoxFile");
            this.m_textBoxFile.Name = "m_textBoxFile";
            this.m_textBoxFile.TextChanged += new System.EventHandler(this.m_textBoxImage_TextChanged);
            // 
            // m_buttonBrowse
            // 
            resources.ApplyResources(this.m_buttonBrowse, "m_buttonBrowse");
            this.m_buttonBrowse.Name = "m_buttonBrowse";
            this.m_buttonBrowse.UseVisualStyleBackColor = true;
            this.m_buttonBrowse.Click += new System.EventHandler(this.m_buttonBrowse_Click);
            // 
            // m_ctrlError
            // 
            resources.ApplyResources(this.m_ctrlError, "m_ctrlError");
            this.m_ctrlError.Name = "m_ctrlError";
            // 
            // m_tlpEncryption
            // 
            resources.ApplyResources(this.m_tlpEncryption, "m_tlpEncryption");
            this.m_tlpEncryption.Controls.Add(this.pictureBox1, 0, 0);
            this.m_tlpEncryption.Controls.Add(this.autoHeightLabel1, 1, 0);
            this.m_tlpEncryption.Name = "m_tlpEncryption";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources._000_WarningAlert_h32bit_32;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // autoHeightLabel1
            // 
            resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // m_tlpError
            // 
            resources.ApplyResources(this.m_tlpError, "m_tlpError");
            this.m_tlpError.Controls.Add(this.pictureBox2, 0, 0);
            this.m_tlpError.Controls.Add(this._labelError, 1, 0);
            this.m_tlpError.Name = "m_tlpError";
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // _labelError
            // 
            resources.ApplyResources(this._labelError, "_labelError");
            this._labelError.Name = "_labelError";
            // 
            // labelProgress
            // 
            resources.ApplyResources(this.labelProgress, "labelProgress");
            this.labelProgress.Name = "labelProgress";
            // 
            // _unzipWorker
            // 
            this._unzipWorker.WorkerReportsProgress = true;
            this._unzipWorker.WorkerSupportsCancellation = true;
            this._unzipWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._unzipWorker_DoWork);
            this._unzipWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this._unzipWorker_ProgressChanged);
            this._unzipWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._unzipWorker_RunWorkerCompleted);
            // 
            // ImportSourcePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ImportSourcePage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.m_tlpEncryption.ResumeLayout(false);
            this.m_tlpEncryption.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.m_tlpError.ResumeLayout(false);
            this.m_tlpError.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox m_textBoxFile;
		private System.Windows.Forms.Button m_buttonBrowse;
        private XenAdmin.Controls.Common.PasswordFailure m_ctrlError;
		private XenAdmin.Controls.Common.AutoHeightLabel lblIntro;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.TableLayoutPanel m_tlpEncryption;
		private System.Windows.Forms.PictureBox pictureBox1;
		private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TableLayoutPanel m_tlpError;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.ComponentModel.BackgroundWorker _unzipWorker;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.Label _labelError;

    }
}
