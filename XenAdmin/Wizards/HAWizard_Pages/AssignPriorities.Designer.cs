using XenAdmin.Controls;
namespace XenAdmin.Wizards.HAWizard_Pages
{
    partial class AssignPriorities
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssignPriorities));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxStatus = new System.Windows.Forms.PictureBox();
            this.labelHaStatus = new System.Windows.Forms.Label();
            this.labelProtectionLevel = new System.Windows.Forms.Label();
            this.m_dropDownButtonRestartPriority = new XenAdmin.Controls.DropDownButton();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.labelStartOrder = new System.Windows.Forms.Label();
            this.nudOrder = new System.Windows.Forms.NumericUpDown();
            this.labelStartDelay = new System.Windows.Forms.Label();
            this.nudStartDelay = new System.Windows.Forms.NumericUpDown();
            this.labelStartDelayUnits = new System.Windows.Forms.Label();
            this.linkLabelTellMeMore = new System.Windows.Forms.LinkLabel();
            this.haNtolIndicator = new XenAdmin.Controls.HaNtolIndicator();
            this.dataGridViewVms = new XenAdmin.Controls.DataGridViewEx.DataGridViewEx();
            this.colImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.colVm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRestartPriority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStartOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDelay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAgile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxStatus, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelHaStatus, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelProtectionLevel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_dropDownButtonRestartPriority, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelStartOrder, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.nudOrder, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelStartDelay, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.nudStartDelay, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelStartDelayUnits, 4, 4);
            this.tableLayoutPanel1.Controls.Add(this.linkLabelTellMeMore, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.haNtolIndicator, 5, 2);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewVms, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // pictureBoxStatus
            // 
            this.pictureBoxStatus.Image = global::XenAdmin.Properties.Resources._000_Tick_h32bit_16;
            resources.ApplyResources(this.pictureBoxStatus, "pictureBoxStatus");
            this.pictureBoxStatus.Name = "pictureBoxStatus";
            this.pictureBoxStatus.TabStop = false;
            // 
            // labelHaStatus
            // 
            resources.ApplyResources(this.labelHaStatus, "labelHaStatus");
            this.tableLayoutPanel1.SetColumnSpan(this.labelHaStatus, 5);
            this.labelHaStatus.Name = "labelHaStatus";
            // 
            // labelProtectionLevel
            // 
            resources.ApplyResources(this.labelProtectionLevel, "labelProtectionLevel");
            this.tableLayoutPanel1.SetColumnSpan(this.labelProtectionLevel, 2);
            this.labelProtectionLevel.Name = "labelProtectionLevel";
            // 
            // m_dropDownButtonRestartPriority
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.m_dropDownButtonRestartPriority, 2);
            this.m_dropDownButtonRestartPriority.ContextMenuStrip = this.contextMenuStrip;
            resources.ApplyResources(this.m_dropDownButtonRestartPriority, "m_dropDownButtonRestartPriority");
            this.m_dropDownButtonRestartPriority.Name = "m_dropDownButtonRestartPriority";
            this.m_dropDownButtonRestartPriority.UseVisualStyleBackColor = true;
            this.m_dropDownButtonRestartPriority.Click += new System.EventHandler(this.m_dropDownButtonRestartPriority_Click);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            // 
            // labelStartOrder
            // 
            resources.ApplyResources(this.labelStartOrder, "labelStartOrder");
            this.tableLayoutPanel1.SetColumnSpan(this.labelStartOrder, 3);
            this.labelStartOrder.Name = "labelStartOrder";
            // 
            // nudOrder
            // 
            resources.ApplyResources(this.nudOrder, "nudOrder");
            this.nudOrder.Name = "nudOrder";
            this.nudOrder.ValueChanged += new System.EventHandler(this.nudOrder_ValueChanged);
            // 
            // labelStartDelay
            // 
            resources.ApplyResources(this.labelStartDelay, "labelStartDelay");
            this.tableLayoutPanel1.SetColumnSpan(this.labelStartDelay, 3);
            this.labelStartDelay.Name = "labelStartDelay";
            // 
            // nudStartDelay
            // 
            resources.ApplyResources(this.nudStartDelay, "nudStartDelay");
            this.nudStartDelay.Name = "nudStartDelay";
            this.nudStartDelay.ValueChanged += new System.EventHandler(this.nudStartDelay_ValueChanged);
            // 
            // labelStartDelayUnits
            // 
            resources.ApplyResources(this.labelStartDelayUnits, "labelStartDelayUnits");
            this.labelStartDelayUnits.Name = "labelStartDelayUnits";
            // 
            // haNtolIndicator
            // 
            resources.ApplyResources(this.haNtolIndicator, "haNtolIndicator");
            this.haNtolIndicator.Name = "haNtolIndicator";
            this.tableLayoutPanel1.SetRowSpan(this.haNtolIndicator, 4);
            // 
            // dataGridViewVms
            // 
            this.dataGridViewVms.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewVms.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewVms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewVms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colImage,
            this.colVm,
            this.colRestartPriority,
            this.colStartOrder,
            this.colDelay,
            this.colAgile});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridViewVms, 6);
            resources.ApplyResources(this.dataGridViewVms, "dataGridViewVms");
            this.dataGridViewVms.MultiSelect = true;
            this.dataGridViewVms.Name = "dataGridViewVms";
            this.dataGridViewVms.SelectionChanged += new System.EventHandler(this.dataGridViewVms_SelectionChanged);
            this.dataGridViewVms.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewVms_KeyDown);
            // 
            // colImage
            // 
            this.colImage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.colImage, "colImage");
            this.colImage.Name = "colImage";
            // 
            // colVm
            // 
            this.colVm.FillWeight = 123.8597F;
            resources.ApplyResources(this.colVm, "colVm");
            this.colVm.Name = "colVm";
            // 
            // colRestartPriority
            // 
            this.colRestartPriority.FillWeight = 142.0455F;
            resources.ApplyResources(this.colRestartPriority, "colRestartPriority");
            this.colRestartPriority.Name = "colRestartPriority";
            // 
            // colStartOrder
            // 
            this.colStartOrder.FillWeight = 78.03161F;
            resources.ApplyResources(this.colStartOrder, "colStartOrder");
            this.colStartOrder.Name = "colStartOrder";
            // 
            // colDelay
            // 
            this.colDelay.FillWeight = 78.03161F;
            resources.ApplyResources(this.colDelay, "colDelay");
            this.colDelay.Name = "colDelay";
            // 
            // colAgile
            // 
            this.colAgile.FillWeight = 78.03161F;
            resources.ApplyResources(this.colAgile, "colAgile");
            this.colAgile.Name = "colAgile";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // bgWorker
            // 
            this.bgWorker.WorkerSupportsCancellation = true;
            this.bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_DoWork);
            this.bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_RunWorkerCompleted);
            // 
            // AssignPriorities
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "AssignPriorities";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVms)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelHaStatus;
        private System.Windows.Forms.PictureBox pictureBoxStatus;
        private HaNtolIndicator haNtolIndicator;
        private System.Windows.Forms.Label labelStartDelayUnits;
        private System.Windows.Forms.NumericUpDown nudStartDelay;
        private System.Windows.Forms.Label labelStartDelay;
        private System.Windows.Forms.Label labelStartOrder;
        private System.Windows.Forms.NumericUpDown nudOrder;
        private DropDownButton m_dropDownButtonRestartPriority;
        private System.Windows.Forms.Label labelProtectionLevel;
        private System.Windows.Forms.LinkLabel linkLabelTellMeMore;
        private XenAdmin.Controls.DataGridViewEx.DataGridViewEx dataGridViewVms;
        private System.Windows.Forms.DataGridViewImageColumn colImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVm;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRestartPriority;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStartOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDelay;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAgile;
        private System.ComponentModel.BackgroundWorker bgWorker;


    }
}
