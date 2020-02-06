namespace XenAdmin.Wizards.ConversionWizard
{
    partial class SrSelectionPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SrSelectionPage));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.SRLabel = new System.Windows.Forms.Label();
            this.comboBoxSr = new XenAdmin.Controls.EnableableComboBox();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.tableLayoutPanelError = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxError = new System.Windows.Forms.PictureBox();
            this.labelError = new System.Windows.Forms.Label();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelError.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.SRLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxSr, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonRefresh, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelError, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.chart1, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
            this.label1.Name = "label1";
            // 
            // SRLabel
            // 
            resources.ApplyResources(this.SRLabel, "SRLabel");
            this.SRLabel.Name = "SRLabel";
            // 
            // comboBoxSr
            // 
            resources.ApplyResources(this.comboBoxSr, "comboBoxSr");
            this.comboBoxSr.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxSr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSr.FormattingEnabled = true;
            this.comboBoxSr.Name = "comboBoxSr";
            this.comboBoxSr.SelectedIndexChanged += new System.EventHandler(this.comboBoxSr_SelectedIndexChanged);
            // 
            // buttonRefresh
            // 
            resources.ApplyResources(this.buttonRefresh, "buttonRefresh");
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // tableLayoutPanelError
            // 
            resources.ApplyResources(this.tableLayoutPanelError, "tableLayoutPanelError");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanelError, 2);
            this.tableLayoutPanelError.Controls.Add(this.pictureBoxError, 0, 0);
            this.tableLayoutPanelError.Controls.Add(this.labelError, 1, 0);
            this.tableLayoutPanelError.Name = "tableLayoutPanelError";
            // 
            // pictureBoxError
            // 
            this.pictureBoxError.Image = global::XenAdmin.Properties.Resources._000_error_h32bit_16;
            resources.ApplyResources(this.pictureBoxError, "pictureBoxError");
            this.pictureBoxError.Name = "pictureBoxError";
            this.pictureBoxError.TabStop = false;
            // 
            // labelError
            // 
            resources.ApplyResources(this.labelError, "labelError");
            this.labelError.Name = "labelError";
            // 
            // chart1
            // 
            this.chart1.BackColor = System.Drawing.SystemColors.Control;
            this.chart1.BackImageTransparentColor = System.Drawing.SystemColors.Control;
            this.chart1.BackSecondaryColor = System.Drawing.SystemColors.Control;
            this.chart1.BorderlineColor = System.Drawing.SystemColors.Control;
            chartArea1.Area3DStyle.Enable3D = true;
            chartArea1.BackColor = System.Drawing.SystemColors.Control;
            chartArea1.BackSecondaryColor = System.Drawing.SystemColors.Control;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.tableLayoutPanel1.SetColumnSpan(this.chart1, 3);
            resources.ApplyResources(this.chart1, "chart1");
            legend1.BackColor = System.Drawing.SystemColors.Control;
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series1.Color = System.Drawing.SystemColors.Control;
            series1.CustomProperties = "PieLabelStyle=Outside, PieStartAngle=90";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // SrSelectionPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SrSelectionPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanelError.ResumeLayout(false);
            this.tableLayoutPanelError.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label SRLabel;
        private XenAdmin.Controls.EnableableComboBox comboBoxSr;
        private System.Windows.Forms.Label labelError;
        private System.Windows.Forms.PictureBox pictureBoxError;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelError;
        private System.Windows.Forms.Button buttonRefresh;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}
