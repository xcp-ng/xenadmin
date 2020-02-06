namespace XenAdmin.Dialogs
{
    partial class NewDiskDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewDiskDialog));
            this.SrListBox = new XenAdmin.Controls.SrPicker();
            this.CloseButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.DescriptionTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.diskSpinner1 = new XenAdmin.Controls.DiskSpinner();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SrListBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.SrListBox, 3);
            this.SrListBox.Connection = null;
            resources.ApplyResources(this.SrListBox, "SrListBox");
            this.SrListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.SrListBox.Name = "SrListBox";
            this.SrListBox.NodeIndent = 3;
            this.SrListBox.RootAlwaysExpanded = false;
            this.SrListBox.ShowCheckboxes = false;
            this.SrListBox.ShowDescription = true;
            this.SrListBox.ShowImages = true;
            this.SrListBox.ShowRootLines = true;
            this.SrListBox.SelectedIndexChanged += new System.EventHandler(this.srListBox_SelectedIndexChanged);
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // OkButton
            // 
            resources.ApplyResources(this.OkButton, "OkButton");
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OkButton.Name = "OkButton";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // NameTextBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.NameTextBox, 3);
            resources.ApplyResources(this.NameTextBox, "NameTextBox");
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.TextChanged += new System.EventHandler(this.NameTextBox_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // DescriptionTextBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.DescriptionTextBox, 3);
            resources.ApplyResources(this.DescriptionTextBox, "DescriptionTextBox");
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.TextChanged += new System.EventHandler(this.NameTextBox_TextChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.CloseButton, 3, 6);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.NameTextBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.DescriptionTextBox, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.SrListBox, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.OkButton, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.diskSpinner1, 1, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.tableLayoutPanel1.SetColumnSpan(this.label6, 4);
            this.label6.Name = "label6";
            // 
            // diskSpinner1
            // 
            resources.ApplyResources(this.diskSpinner1, "diskSpinner1");
            this.tableLayoutPanel1.SetColumnSpan(this.diskSpinner1, 3);
            this.diskSpinner1.Name = "diskSpinner1";
            this.diskSpinner1.SelectedSizeChanged += new System.Action(this.diskSpinner1_SelectedSizeChanged);
            // 
            // NewDiskDialog
            // 
            this.AcceptButton = this.OkButton;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.CloseButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "NewDiskDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private XenAdmin.Controls.SrPicker SrListBox;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.TextBox DescriptionTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private Controls.DiskSpinner diskSpinner1;
    }
}
