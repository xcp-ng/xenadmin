namespace XenAdmin.Controls.Ballooning
{
    partial class VMMemoryControlsNoEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VMMemoryControlsNoEdit));
            this.iconBoxDynMin = new System.Windows.Forms.PictureBox();
            this.iconBoxDynMax = new System.Windows.Forms.PictureBox();
            this.labelDynMin = new System.Windows.Forms.Label();
            this.labelDynMax = new System.Windows.Forms.Label();
            this.labelStatMax = new System.Windows.Forms.Label();
            this.valueDynMin = new System.Windows.Forms.Label();
            this.valueDynMax = new System.Windows.Forms.Label();
            this.valueStatMax = new System.Windows.Forms.Label();
            this.editButton = new System.Windows.Forms.Button();
            this.vmShinyBar = new XenAdmin.Controls.Ballooning.VMShinyBar();
            ((System.ComponentModel.ISupportInitialize)(this.iconBoxDynMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconBoxDynMax)).BeginInit();
            this.SuspendLayout();
            // 
            // iconBoxDynMin
            // 
            resources.ApplyResources(this.iconBoxDynMin, "iconBoxDynMin");
            this.iconBoxDynMin.BackColor = System.Drawing.Color.Transparent;
            this.iconBoxDynMin.Image = global::XenAdmin.Properties.Resources.memory_dynmin_slider_noedit_small;
            this.iconBoxDynMin.Name = "iconBoxDynMin";
            this.iconBoxDynMin.TabStop = false;
            // 
            // iconBoxDynMax
            // 
            resources.ApplyResources(this.iconBoxDynMax, "iconBoxDynMax");
            this.iconBoxDynMax.BackColor = System.Drawing.Color.Transparent;
            this.iconBoxDynMax.Image = global::XenAdmin.Properties.Resources.memory_dynmax_slider_noedit_small;
            this.iconBoxDynMax.Name = "iconBoxDynMax";
            this.iconBoxDynMax.TabStop = false;
            // 
            // labelDynMin
            // 
            resources.ApplyResources(this.labelDynMin, "labelDynMin");
            this.labelDynMin.BackColor = System.Drawing.Color.Transparent;
            this.labelDynMin.Name = "labelDynMin";
            // 
            // labelDynMax
            // 
            resources.ApplyResources(this.labelDynMax, "labelDynMax");
            this.labelDynMax.BackColor = System.Drawing.Color.Transparent;
            this.labelDynMax.Name = "labelDynMax";
            // 
            // labelStatMax
            // 
            resources.ApplyResources(this.labelStatMax, "labelStatMax");
            this.labelStatMax.BackColor = System.Drawing.Color.Transparent;
            this.labelStatMax.Name = "labelStatMax";
            // 
            // valueDynMin
            // 
            resources.ApplyResources(this.valueDynMin, "valueDynMin");
            this.valueDynMin.BackColor = System.Drawing.Color.Transparent;
            this.valueDynMin.Name = "valueDynMin";
            // 
            // valueDynMax
            // 
            resources.ApplyResources(this.valueDynMax, "valueDynMax");
            this.valueDynMax.BackColor = System.Drawing.Color.Transparent;
            this.valueDynMax.Name = "valueDynMax";
            // 
            // valueStatMax
            // 
            resources.ApplyResources(this.valueStatMax, "valueStatMax");
            this.valueStatMax.BackColor = System.Drawing.Color.Transparent;
            this.valueStatMax.Name = "valueStatMax";
            // 
            // editButton
            // 
            resources.ApplyResources(this.editButton, "editButton");
            this.editButton.Name = "editButton";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // vmShinyBar
            // 
            resources.ApplyResources(this.vmShinyBar, "vmShinyBar");
            this.vmShinyBar.Name = "vmShinyBar";
            // 
            // VMMemoryControlsNoEdit
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.valueStatMax);
            this.Controls.Add(this.valueDynMax);
            this.Controls.Add(this.valueDynMin);
            this.Controls.Add(this.labelStatMax);
            this.Controls.Add(this.labelDynMax);
            this.Controls.Add(this.labelDynMin);
            this.Controls.Add(this.iconBoxDynMax);
            this.Controls.Add(this.iconBoxDynMin);
            this.Controls.Add(this.vmShinyBar);
            this.Name = "VMMemoryControlsNoEdit";
            ((System.ComponentModel.ISupportInitialize)(this.iconBoxDynMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconBoxDynMax)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private VMShinyBar vmShinyBar;
        private System.Windows.Forms.PictureBox iconBoxDynMin;
        private System.Windows.Forms.PictureBox iconBoxDynMax;
        private System.Windows.Forms.Label labelDynMin;
        private System.Windows.Forms.Label labelDynMax;
        private System.Windows.Forms.Label labelStatMax;
        private System.Windows.Forms.Label valueDynMin;
        private System.Windows.Forms.Label valueDynMax;
        private System.Windows.Forms.Label valueStatMax;
        private System.Windows.Forms.Button editButton;
    }
}
