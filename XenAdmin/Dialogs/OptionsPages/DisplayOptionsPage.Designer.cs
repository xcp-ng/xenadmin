namespace XenAdmin.Dialogs.OptionsPages
{
    partial class DisplayOptionsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisplayOptionsPage));
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.GraphTypeGroupBox = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.GraphAreasRadioButton = new System.Windows.Forms.RadioButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.GraphLinesRadioButton = new System.Windows.Forms.RadioButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.TabGroupBox = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxStoreTab = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel3.SuspendLayout();
            this.GraphTypeGroupBox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.TabGroupBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.GraphTypeGroupBox, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.TabGroupBox, 0, 1);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // GraphTypeGroupBox
            // 
            resources.ApplyResources(this.GraphTypeGroupBox, "GraphTypeGroupBox");
            this.GraphTypeGroupBox.Controls.Add(this.tableLayoutPanel1);
            this.GraphTypeGroupBox.Name = "GraphTypeGroupBox";
            this.GraphTypeGroupBox.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.GraphAreasRadioButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox2, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.GraphLinesRadioButton, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // GraphAreasRadioButton
            // 
            resources.ApplyResources(this.GraphAreasRadioButton, "GraphAreasRadioButton");
            this.GraphAreasRadioButton.Name = "GraphAreasRadioButton";
            this.GraphAreasRadioButton.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Image = global::XenAdmin.Properties.Resources.graphline;
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // GraphLinesRadioButton
            // 
            resources.ApplyResources(this.GraphLinesRadioButton, "GraphLinesRadioButton");
            this.GraphLinesRadioButton.Checked = true;
            this.GraphLinesRadioButton.Name = "GraphLinesRadioButton";
            this.GraphLinesRadioButton.TabStop = true;
            this.GraphLinesRadioButton.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::XenAdmin.Properties.Resources.grapharea;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // TabGroupBox
            // 
            resources.ApplyResources(this.TabGroupBox, "TabGroupBox");
            this.TabGroupBox.Controls.Add(this.tableLayoutPanel2);
            this.TabGroupBox.Name = "TabGroupBox";
            this.TabGroupBox.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.checkBoxStoreTab, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // checkBoxStoreTab
            // 
            resources.ApplyResources(this.checkBoxStoreTab, "checkBoxStoreTab");
            this.checkBoxStoreTab.Checked = true;
            this.checkBoxStoreTab.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStoreTab.Name = "checkBoxStoreTab";
            this.checkBoxStoreTab.UseVisualStyleBackColor = true;
            // 
            // DisplayOptionsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "DisplayOptionsPage";
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.GraphTypeGroupBox.ResumeLayout(false);
            this.GraphTypeGroupBox.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.TabGroupBox.ResumeLayout(false);
            this.TabGroupBox.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private XenAdmin.Controls.DecentGroupBox GraphTypeGroupBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RadioButton GraphAreasRadioButton;
        private System.Windows.Forms.RadioButton GraphLinesRadioButton;
        private XenAdmin.Controls.Common.AutoHeightLabel label5;
        private Controls.DecentGroupBox TabGroupBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox checkBoxStoreTab;
    }
}
