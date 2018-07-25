using XenAdmin.Wizards.PatchingWizard;

namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    partial class RollingUpgradeWizardUpgradeModePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RollingUpgradeWizardUpgradeModePage));
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonManual = new System.Windows.Forms.RadioButton();
            this.radioButtonAutomatic = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.applyUpdatesLabel = new System.Windows.Forms.Label();
            this.applyUpdatesCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // radioButtonManual
            // 
            resources.ApplyResources(this.radioButtonManual, "radioButtonManual");
            this.radioButtonManual.Name = "radioButtonManual";
            this.radioButtonManual.UseVisualStyleBackColor = true;
            // 
            // radioButtonAutomatic
            // 
            resources.ApplyResources(this.radioButtonAutomatic, "radioButtonAutomatic");
            this.radioButtonAutomatic.Checked = true;
            this.radioButtonAutomatic.Name = "radioButtonAutomatic";
            this.radioButtonAutomatic.TabStop = true;
            this.radioButtonAutomatic.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.applyUpdatesLabel, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.applyUpdatesCheckBox, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonAutomatic, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonManual, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 7);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // applyUpdatesLabel
            // 
            resources.ApplyResources(this.applyUpdatesLabel, "applyUpdatesLabel");
            this.applyUpdatesLabel.Name = "applyUpdatesLabel";
            // 
            // applyUpdatesCheckBox
            // 
            resources.ApplyResources(this.applyUpdatesCheckBox, "applyUpdatesCheckBox");
            this.applyUpdatesCheckBox.Checked = true;
            this.applyUpdatesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel1.SetColumnSpan(this.applyUpdatesCheckBox, 2);
            this.applyUpdatesCheckBox.Name = "applyUpdatesCheckBox";
            this.applyUpdatesCheckBox.UseVisualStyleBackColor = true;
            // 
            // RollingUpgradeWizardUpgradeModePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RollingUpgradeWizardUpgradeModePage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonManual;
        private System.Windows.Forms.RadioButton radioButtonAutomatic;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox applyUpdatesCheckBox;
        private System.Windows.Forms.Label applyUpdatesLabel;
    }
}
