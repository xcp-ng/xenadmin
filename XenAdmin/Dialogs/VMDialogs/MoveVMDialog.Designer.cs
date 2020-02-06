namespace XenAdmin.Dialogs.VMDialogs
{
    partial class MoveVMDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MoveVMDialog));
            this.srPicker1 = new XenAdmin.Controls.SrPicker();
            this.buttonMove = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTipContainer1 = new XenAdmin.Controls.ToolTipContainer();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolTipContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // srPicker1
            // 
            resources.ApplyResources(this.srPicker1, "srPicker1");
            this.srPicker1.Connection = null;
            this.srPicker1.Name = "srPicker1";
            this.srPicker1.SelectedIndexChanged += new System.EventHandler(this.srPicker1_SelectedIndexChanged);
            // 
            // buttonMove
            // 
            resources.ApplyResources(this.buttonMove, "buttonMove");
            this.buttonMove.Name = "buttonMove";
            this.buttonMove.UseVisualStyleBackColor = true;
            this.buttonMove.Click += new System.EventHandler(this.buttonMove_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.srPicker1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // toolTipContainer1
            // 
            resources.ApplyResources(this.toolTipContainer1, "toolTipContainer1");
            this.toolTipContainer1.Controls.Add(this.buttonMove);
            this.toolTipContainer1.Name = "toolTipContainer1";
            // 
            // MoveVMDialog
            // 
            this.AcceptButton = this.buttonMove;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.toolTipContainer1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.buttonCancel);
            this.Name = "MoveVMDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.toolTipContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private XenAdmin.Controls.SrPicker srPicker1;
        private System.Windows.Forms.Button buttonMove;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private XenAdmin.Controls.ToolTipContainer toolTipContainer1;
    }
}