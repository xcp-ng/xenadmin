namespace XenAdmin.Wizards.NewPolicyWizard
{
    partial class NewPolicySnapshotFrequencyPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewPolicySnapshotFrequencyPage));
            this.radioButtonHourly = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.dateTimePickerDaily = new XenAdmin.Controls.DateTimeMinutes15();
            this.comboBoxMin = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.daysWeekCheckboxes = new XenAdmin.Wizards.NewPolicyWizard.DaysWeekCheckboxes();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimePickerWeekly = new XenAdmin.Controls.DateTimeMinutes15();
            this.radioButtonWeekly = new System.Windows.Forms.RadioButton();
            this.radioButtonDaily = new System.Windows.Forms.RadioButton();
            this.numericUpDownRetention = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.sectionLabelSchedule = new XenAdmin.Controls.SectionHeaderLabel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label9 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panelHourly = new System.Windows.Forms.TableLayoutPanel();
            this.panelDaily = new System.Windows.Forms.TableLayoutPanel();
            this.panelWeekly = new System.Windows.Forms.TableLayoutPanel();
            this.MainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.autoHeightLabel1 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.sectionLabelNumber = new XenAdmin.Controls.SectionHeaderLabel();
            this.autoHeightLabel2 = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.LoadingBox = new System.Windows.Forms.TableLayoutPanel();
            this.labelProgress = new System.Windows.Forms.Label();
            this.spinnerIcon1 = new XenAdmin.Controls.SpinnerIcon();
            this.labelServerNextRun = new System.Windows.Forms.Label();
            this.labelClientNextRun = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRetention)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panelHourly.SuspendLayout();
            this.panelDaily.SuspendLayout();
            this.panelWeekly.SuspendLayout();
            this.MainTableLayoutPanel.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.LoadingBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIcon1)).BeginInit();
            this.SuspendLayout();
            // 
            // radioButtonHourly
            // 
            resources.ApplyResources(this.radioButtonHourly, "radioButtonHourly");
            this.radioButtonHourly.Name = "radioButtonHourly";
            this.radioButtonHourly.UseVisualStyleBackColor = true;
            this.radioButtonHourly.CheckedChanged += new System.EventHandler(this.radioButtonHourly_CheckedChanged);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // dateTimePickerDaily
            // 
            resources.ApplyResources(this.dateTimePickerDaily, "dateTimePickerDaily");
            this.dateTimePickerDaily.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerDaily.Name = "dateTimePickerDaily";
            this.dateTimePickerDaily.ShowUpDown = true;
            this.dateTimePickerDaily.Value = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            this.dateTimePickerDaily.ValueChanged += new System.EventHandler(this.dateTimePickerDaily_ValueChanged);
            // 
            // comboBoxMin
            // 
            this.comboBoxMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMin.FormattingEnabled = true;
            this.comboBoxMin.Items.AddRange(new object[] {
            resources.GetString("comboBoxMin.Items"),
            resources.GetString("comboBoxMin.Items1"),
            resources.GetString("comboBoxMin.Items2"),
            resources.GetString("comboBoxMin.Items3")});
            resources.ApplyResources(this.comboBoxMin, "comboBoxMin");
            this.comboBoxMin.Name = "comboBoxMin";
            this.comboBoxMin.SelectedIndexChanged += new System.EventHandler(this.comboBoxMin_SelectedIndexChanged);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // daysWeekCheckboxes
            // 
            resources.ApplyResources(this.daysWeekCheckboxes, "daysWeekCheckboxes");
            this.panelWeekly.SetColumnSpan(this.daysWeekCheckboxes, 2);
            this.daysWeekCheckboxes.Name = "daysWeekCheckboxes";
            this.daysWeekCheckboxes.CheckBoxChanged += new System.EventHandler(this.daysWeekCheckboxes_CheckBoxChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // dateTimePickerWeekly
            // 
            resources.ApplyResources(this.dateTimePickerWeekly, "dateTimePickerWeekly");
            this.dateTimePickerWeekly.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerWeekly.Name = "dateTimePickerWeekly";
            this.dateTimePickerWeekly.ShowUpDown = true;
            this.dateTimePickerWeekly.Value = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            this.dateTimePickerWeekly.ValueChanged += new System.EventHandler(this.dateTimePickerWeekly_ValueChanged);
            // 
            // radioButtonWeekly
            // 
            resources.ApplyResources(this.radioButtonWeekly, "radioButtonWeekly");
            this.radioButtonWeekly.Name = "radioButtonWeekly";
            this.radioButtonWeekly.UseVisualStyleBackColor = true;
            this.radioButtonWeekly.CheckedChanged += new System.EventHandler(this.radioButtonWeekly_CheckedChanged);
            // 
            // radioButtonDaily
            // 
            resources.ApplyResources(this.radioButtonDaily, "radioButtonDaily");
            this.radioButtonDaily.Name = "radioButtonDaily";
            this.radioButtonDaily.UseVisualStyleBackColor = true;
            this.radioButtonDaily.CheckedChanged += new System.EventHandler(this.radioButtonDaily_CheckedChanged);
            // 
            // numericUpDownRetention
            // 
            resources.ApplyResources(this.numericUpDownRetention, "numericUpDownRetention");
            this.numericUpDownRetention.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownRetention.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownRetention.Name = "numericUpDownRetention";
            this.numericUpDownRetention.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownRetention.ValueChanged += new System.EventHandler(this.numericUpDownRetention_ValueChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.sectionLabelSchedule, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelClientNextRun, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelServerNextRun, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // sectionLabelSchedule
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.sectionLabelSchedule, 2);
            resources.ApplyResources(this.sectionLabelSchedule, "sectionLabelSchedule");
            this.sectionLabelSchedule.LineColor = System.Drawing.SystemColors.Window;
            this.sectionLabelSchedule.LineLocation = XenAdmin.Controls.SectionHeaderLabel.VerticalAlignment.Middle;
            this.sectionLabelSchedule.Name = "sectionLabelSchedule";
            this.sectionLabelSchedule.TabStop = false;
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.radioButtonHourly, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.radioButtonDaily, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.radioButtonWeekly, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.label9, 1, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label9.Name = "label9";
            this.tableLayoutPanel4.SetRowSpan(this.label9, 3);
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.panelHourly);
            this.flowLayoutPanel1.Controls.Add(this.panelDaily);
            this.flowLayoutPanel1.Controls.Add(this.panelWeekly);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // panelHourly
            // 
            resources.ApplyResources(this.panelHourly, "panelHourly");
            this.panelHourly.Controls.Add(this.label6, 0, 0);
            this.panelHourly.Controls.Add(this.comboBoxMin, 1, 0);
            this.panelHourly.Controls.Add(this.label7, 2, 0);
            this.panelHourly.Name = "panelHourly";
            // 
            // panelDaily
            // 
            resources.ApplyResources(this.panelDaily, "panelDaily");
            this.panelDaily.Controls.Add(this.label8, 0, 0);
            this.panelDaily.Controls.Add(this.dateTimePickerDaily, 1, 0);
            this.panelDaily.Name = "panelDaily";
            // 
            // panelWeekly
            // 
            resources.ApplyResources(this.panelWeekly, "panelWeekly");
            this.panelWeekly.Controls.Add(this.label4, 0, 0);
            this.panelWeekly.Controls.Add(this.dateTimePickerWeekly, 1, 0);
            this.panelWeekly.Controls.Add(this.daysWeekCheckboxes, 0, 1);
            this.panelWeekly.Name = "panelWeekly";
            // 
            // MainTableLayoutPanel
            // 
            resources.ApplyResources(this.MainTableLayoutPanel, "MainTableLayoutPanel");
            this.MainTableLayoutPanel.Controls.Add(this.autoHeightLabel1, 0, 0);
            this.MainTableLayoutPanel.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.MainTableLayoutPanel.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.MainTableLayoutPanel.Name = "MainTableLayoutPanel";
            // 
            // autoHeightLabel1
            // 
            resources.ApplyResources(this.autoHeightLabel1, "autoHeightLabel1");
            this.autoHeightLabel1.Name = "autoHeightLabel1";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.sectionLabelNumber, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.autoHeightLabel2, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.numericUpDownRetention, 1, 2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // sectionLabelNumber
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.sectionLabelNumber, 2);
            resources.ApplyResources(this.sectionLabelNumber, "sectionLabelNumber");
            this.sectionLabelNumber.LineColor = System.Drawing.SystemColors.Window;
            this.sectionLabelNumber.LineLocation = XenAdmin.Controls.SectionHeaderLabel.VerticalAlignment.Middle;
            this.sectionLabelNumber.Name = "sectionLabelNumber";
            this.sectionLabelNumber.TabStop = false;
            // 
            // autoHeightLabel2
            // 
            resources.ApplyResources(this.autoHeightLabel2, "autoHeightLabel2");
            this.tableLayoutPanel3.SetColumnSpan(this.autoHeightLabel2, 2);
            this.autoHeightLabel2.Name = "autoHeightLabel2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.label1.Name = "label1";
            // 
            // LoadingBox
            // 
            resources.ApplyResources(this.LoadingBox, "LoadingBox");
            this.LoadingBox.Controls.Add(this.labelProgress, 1, 0);
            this.LoadingBox.Controls.Add(this.spinnerIcon1, 0, 0);
            this.LoadingBox.Name = "LoadingBox";
            // 
            // labelProgress
            // 
            resources.ApplyResources(this.labelProgress, "labelProgress");
            this.labelProgress.Name = "labelProgress";
            // 
            // spinnerIcon1
            // 
            resources.ApplyResources(this.spinnerIcon1, "spinnerIcon1");
            this.spinnerIcon1.Name = "spinnerIcon1";
            this.spinnerIcon1.TabStop = false;
            // 
            // labelServerNextRun
            // 
            resources.ApplyResources(this.labelServerNextRun, "labelServerNextRun");
            this.tableLayoutPanel1.SetColumnSpan(this.labelServerNextRun, 2);
            this.labelServerNextRun.Name = "labelServerNextRun";
            // 
            // labelClientNextRun
            // 
            resources.ApplyResources(this.labelClientNextRun, "labelClientNextRun");
            this.tableLayoutPanel1.SetColumnSpan(this.labelClientNextRun, 2);
            this.labelClientNextRun.Name = "labelClientNextRun";
            // 
            // NewPolicySnapshotFrequencyPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.LoadingBox);
            this.Controls.Add(this.MainTableLayoutPanel);
            this.Name = "NewPolicySnapshotFrequencyPage";
            this.ParentChanged += new System.EventHandler(this.NewPolicySnapshotFrequencyPage_ParentChanged);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRetention)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panelHourly.ResumeLayout(false);
            this.panelHourly.PerformLayout();
            this.panelDaily.ResumeLayout(false);
            this.panelDaily.PerformLayout();
            this.panelWeekly.ResumeLayout(false);
            this.panelWeekly.PerformLayout();
            this.MainTableLayoutPanel.ResumeLayout(false);
            this.MainTableLayoutPanel.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.LoadingBox.ResumeLayout(false);
            this.LoadingBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerIcon1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonHourly;
        private System.Windows.Forms.RadioButton radioButtonWeekly;
        private System.Windows.Forms.RadioButton radioButtonDaily;
        private System.Windows.Forms.NumericUpDown numericUpDownRetention;
        private System.Windows.Forms.Label label2;
        private XenAdmin.Controls.DateTimeMinutes15 dateTimePickerDaily;
        private XenAdmin.Controls.DateTimeMinutes15 dateTimePickerWeekly;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxMin;
        private DaysWeekCheckboxes daysWeekCheckboxes;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel MainTableLayoutPanel;
        private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel1;
        private XenAdmin.Controls.SectionHeaderLabel sectionLabelSchedule;
        private XenAdmin.Controls.SectionHeaderLabel sectionLabelNumber;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private XenAdmin.Controls.Common.AutoHeightLabel autoHeightLabel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel panelWeekly;
        private System.Windows.Forms.TableLayoutPanel panelHourly;
        private System.Windows.Forms.TableLayoutPanel panelDaily;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel LoadingBox;
        private System.Windows.Forms.Label labelProgress;
        private Controls.SpinnerIcon spinnerIcon1;
        private System.Windows.Forms.Label labelClientNextRun;
        private System.Windows.Forms.Label labelServerNextRun;
    }
}
