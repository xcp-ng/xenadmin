namespace XenAdmin.Wizards.NewPolicyWizard
{
    partial class DaysWeekCheckboxes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DaysWeekCheckboxes));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxMonday = new System.Windows.Forms.CheckBox();
            this.checkBoxTuesday = new System.Windows.Forms.CheckBox();
            this.checkBoxWednesday = new System.Windows.Forms.CheckBox();
            this.checkBoxThursday = new System.Windows.Forms.CheckBox();
            this.checkBoxFriday = new System.Windows.Forms.CheckBox();
            this.checkBoxSaturday = new System.Windows.Forms.CheckBox();
            this.checkBoxSunday = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.checkBoxMonday, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxTuesday, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxWednesday, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxThursday, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxFriday, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxSaturday, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxSunday, 2, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // checkBoxMonday
            // 
            resources.ApplyResources(this.checkBoxMonday, "checkBoxMonday");
            this.checkBoxMonday.Name = "checkBoxMonday";
            this.checkBoxMonday.UseVisualStyleBackColor = true;
            this.checkBoxMonday.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxTuesday
            // 
            resources.ApplyResources(this.checkBoxTuesday, "checkBoxTuesday");
            this.checkBoxTuesday.Name = "checkBoxTuesday";
            this.checkBoxTuesday.UseVisualStyleBackColor = true;
            this.checkBoxTuesday.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxWednesday
            // 
            resources.ApplyResources(this.checkBoxWednesday, "checkBoxWednesday");
            this.checkBoxWednesday.Name = "checkBoxWednesday";
            this.checkBoxWednesday.UseVisualStyleBackColor = true;
            this.checkBoxWednesday.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxThursday
            // 
            resources.ApplyResources(this.checkBoxThursday, "checkBoxThursday");
            this.checkBoxThursday.Name = "checkBoxThursday";
            this.checkBoxThursday.UseVisualStyleBackColor = true;
            this.checkBoxThursday.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxFriday
            // 
            resources.ApplyResources(this.checkBoxFriday, "checkBoxFriday");
            this.checkBoxFriday.Name = "checkBoxFriday";
            this.checkBoxFriday.UseVisualStyleBackColor = true;
            this.checkBoxFriday.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxSaturday
            // 
            resources.ApplyResources(this.checkBoxSaturday, "checkBoxSaturday");
            this.checkBoxSaturday.Name = "checkBoxSaturday";
            this.checkBoxSaturday.UseVisualStyleBackColor = true;
            this.checkBoxSaturday.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxSunday
            // 
            resources.ApplyResources(this.checkBoxSunday, "checkBoxSunday");
            this.checkBoxSunday.Name = "checkBoxSunday";
            this.checkBoxSunday.UseVisualStyleBackColor = true;
            this.checkBoxSunday.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // DaysWeekCheckboxes
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DaysWeekCheckboxes";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBoxSaturday;
        private System.Windows.Forms.CheckBox checkBoxFriday;
        private System.Windows.Forms.CheckBox checkBoxThursday;
        private System.Windows.Forms.CheckBox checkBoxSunday;
        private System.Windows.Forms.CheckBox checkBoxMonday;
        private System.Windows.Forms.CheckBox checkBoxTuesday;
        private System.Windows.Forms.CheckBox checkBoxWednesday;
    }
}
