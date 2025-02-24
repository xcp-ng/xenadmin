namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    partial class LocalStorageFrontend
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocalStorageFrontend));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButtonExt = new System.Windows.Forms.RadioButton();
            this.radioButtonLVM = new System.Windows.Forms.RadioButton();
            this.textBoxDevicePath = new System.Windows.Forms.TextBox();
            this.radioButtonXFS = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonExt, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonLVM, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBoxDevicePath, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonXFS, 1, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // radioButtonExt
            // 
            resources.ApplyResources(this.radioButtonExt, "radioButtonExt");
            this.radioButtonExt.Checked = true;
            this.radioButtonExt.Name = "radioButtonExt";
            this.radioButtonExt.TabStop = true;
            this.radioButtonExt.UseVisualStyleBackColor = true;
            this.radioButtonExt.CheckedChanged += new System.EventHandler(this.radioButtonExt4_CheckedChanged);
            // 
            // radioButtonLVM
            // 
            resources.ApplyResources(this.radioButtonLVM, "radioButtonLVM");
            this.radioButtonLVM.Name = "radioButtonLVM";
            this.radioButtonLVM.UseVisualStyleBackColor = true;
            this.radioButtonLVM.CheckedChanged += new System.EventHandler(this.radioButtonExt4_CheckedChanged);
            // 
            // textBoxDevicePath
            // 
            resources.ApplyResources(this.textBoxDevicePath, "textBoxDevicePath");
            this.textBoxDevicePath.Name = "textBoxDevicePath";
            this.textBoxDevicePath.TextChanged += new System.EventHandler(this.textBoxDevicePath_TextChanged);
            // 
            // radioButtonXFS
            // 
            resources.ApplyResources(this.radioButtonXFS, "radioButtonXFS");
            this.radioButtonXFS.Name = "radioButtonXFS";
            this.radioButtonXFS.UseVisualStyleBackColor = true;
            this.radioButtonXFS.CheckedChanged += new System.EventHandler(this.radioButtonExt4_CheckedChanged);
            // 
            // LocalStorageFrontend
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LocalStorageFrontend";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButtonExt;
        private System.Windows.Forms.TextBox textBoxDevicePath;
        private System.Windows.Forms.RadioButton radioButtonXFS;
        private System.Windows.Forms.RadioButton radioButtonLVM;
    }
}
