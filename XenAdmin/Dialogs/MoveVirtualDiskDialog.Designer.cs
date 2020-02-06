namespace XenAdmin.Dialogs
{
    partial class MoveVirtualDiskDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MoveVirtualDiskDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.toolTipContainer2 = new XenAdmin.Controls.ToolTipContainer();
            this.buttonMove = new System.Windows.Forms.Button();
            this.srPicker1 = new XenAdmin.Controls.SrPicker();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelBlurb = new System.Windows.Forms.Label();
            this.toolTipContainer2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // toolTipContainer2
            // 
            resources.ApplyResources(this.toolTipContainer2, "toolTipContainer2");
            this.toolTipContainer2.Controls.Add(this.buttonMove);
            this.toolTipContainer2.Name = "toolTipContainer2";
            // 
            // buttonMove
            // 
            resources.ApplyResources(this.buttonMove, "buttonMove");
            this.buttonMove.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonMove.Name = "buttonMove";
            this.buttonMove.UseVisualStyleBackColor = true;
            this.buttonMove.Click += new System.EventHandler(this.buttonMove_Click);
            // 
            // srPicker1
            // 
            resources.ApplyResources(this.srPicker1, "srPicker1");
            this.tableLayoutPanel2.SetColumnSpan(this.srPicker1, 2);
            this.srPicker1.Connection = null;
            this.srPicker1.Name = "srPicker1";
            this.srPicker1.SelectedIndexChanged += new System.EventHandler(this.srPicker1_SelectedIndexChanged);
            this.srPicker1.DoubleClickOnRow += new System.EventHandler(this.srPicker1_DoubleClickOnRow);
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.toolTipContainer2, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.buttonCancel, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.srPicker1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelBlurb, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelBlurb
            // 
            resources.ApplyResources(this.labelBlurb, "labelBlurb");
            this.tableLayoutPanel2.SetColumnSpan(this.labelBlurb, 2);
            this.labelBlurb.Name = "labelBlurb";
            // 
            // MoveVirtualDiskDialog
            // 
            this.AcceptButton = this.buttonMove;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "MoveVirtualDiskDialog";
            this.toolTipContainer2.ResumeLayout(false);
            this.toolTipContainer2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private XenAdmin.Controls.ToolTipContainer toolTipContainer2;
        private System.Windows.Forms.Button buttonMove;
        private XenAdmin.Controls.SrPicker srPicker1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label labelBlurb;
        private System.Windows.Forms.Button buttonCancel;
    }
}