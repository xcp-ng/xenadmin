namespace XenAdmin.Wizards.ImportWizard
{
	partial class StoragePickerPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StoragePickerPage));
            this.m_srPicker = new XenAdmin.Controls.SrPicker();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelSrHint = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_srPicker
            // 
            this.m_srPicker.Connection = null;
            resources.ApplyResources(this.m_srPicker, "m_srPicker");
            this.m_srPicker.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.m_srPicker.Name = "m_srPicker";
            this.m_srPicker.NodeIndent = 3;
            this.m_srPicker.RootAlwaysExpanded = false;
            this.m_srPicker.ShowCheckboxes = false;
            this.m_srPicker.ShowDescription = true;
            this.m_srPicker.ShowImages = true;
            this.m_srPicker.ShowRootLines = true;
            this.m_srPicker.SelectedIndexChanged += new System.EventHandler(this.srPicker1_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelSrHint, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_srPicker, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelSrHint
            // 
            resources.ApplyResources(this.labelSrHint, "labelSrHint");
            this.labelSrHint.Name = "labelSrHint";
            // 
            // StoragePickerPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "StoragePickerPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

        private XenAdmin.Controls.SrPicker m_srPicker;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelSrHint;
	}
}
