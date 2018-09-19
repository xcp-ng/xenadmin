namespace XenAdmin.Controls.Ballooning
{
    partial class VMMemoryControlsAdvanced
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VMMemoryControlsAdvanced));
            this.spinnerPanel = new System.Windows.Forms.TableLayoutPanel();
            this.memorySpinnerDynMin = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.memorySpinnerDynMax = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.memorySpinnerStatMax = new XenAdmin.Controls.Ballooning.MemorySpinner();
            this.spinnerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // spinnerPanel
            // 
            resources.ApplyResources(this.spinnerPanel, "spinnerPanel");
            this.spinnerPanel.Controls.Add(this.memorySpinnerDynMin, 0, 0);
            this.spinnerPanel.Controls.Add(this.memorySpinnerDynMax, 0, 1);
            this.spinnerPanel.Controls.Add(this.memorySpinnerStatMax, 0, 2);
            this.spinnerPanel.Name = "spinnerPanel";
            // 
            // memorySpinnerDynMin
            // 
            resources.ApplyResources(this.memorySpinnerDynMin, "memorySpinnerDynMin");
            this.memorySpinnerDynMin.Increment = 0.1D;
            this.memorySpinnerDynMin.Name = "memorySpinnerDynMin";
            this.memorySpinnerDynMin.SpinnerValueChanged += new System.EventHandler(this.Spinners_ValueChanged);
            // 
            // memorySpinnerDynMax
            // 
            resources.ApplyResources(this.memorySpinnerDynMax, "memorySpinnerDynMax");
            this.memorySpinnerDynMax.Increment = 0.1D;
            this.memorySpinnerDynMax.Name = "memorySpinnerDynMax";
            this.memorySpinnerDynMax.SpinnerValueChanged += new System.EventHandler(this.Spinners_ValueChanged);
            // 
            // memorySpinnerStatMax
            // 
            resources.ApplyResources(this.memorySpinnerStatMax, "memorySpinnerStatMax");
            this.memorySpinnerStatMax.Increment = 0.1D;
            this.memorySpinnerStatMax.Name = "memorySpinnerStatMax";
            this.memorySpinnerStatMax.SpinnerValueChanged += new System.EventHandler(this.Spinners_ValueChanged);
            // 
            // VMMemoryControlsAdvanced
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.spinnerPanel);
            this.Name = "VMMemoryControlsAdvanced";
            this.spinnerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel spinnerPanel;
        private MemorySpinner memorySpinnerDynMin;
        private MemorySpinner memorySpinnerStatMax;
        private MemorySpinner memorySpinnerDynMax;
    }
}
