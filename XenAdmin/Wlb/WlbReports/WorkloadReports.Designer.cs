namespace XenAdmin
{
    partial class WorkloadReports
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkloadReports));
            this.splitContainerLeftPane = new System.Windows.Forms.SplitContainer();
            this.treeViewReportList = new System.Windows.Forms.TreeView();
            this.lblReports = new System.Windows.Forms.Label();
            this.treeViewSubscriptionList = new System.Windows.Forms.TreeView();
            this.lblSubscriptions = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.wlbReportView1 = new XenAdmin.Controls.Wlb.WlbReportView();
            this.subscriptionView1 = new XenAdmin.Controls.Wlb.WlbReportSubscriptionView();
            this.contextMenuReports = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.runReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerLeftPane.Panel1.SuspendLayout();
            this.splitContainerLeftPane.Panel2.SuspendLayout();
            this.splitContainerLeftPane.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuReports.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerLeftPane
            // 
            resources.ApplyResources(this.splitContainerLeftPane, "splitContainerLeftPane");
            this.splitContainerLeftPane.Name = "splitContainerLeftPane";
            // 
            // splitContainerLeftPane.Panel1
            // 
            this.splitContainerLeftPane.Panel1.Controls.Add(this.treeViewReportList);
            this.splitContainerLeftPane.Panel1.Controls.Add(this.lblReports);
            resources.ApplyResources(this.splitContainerLeftPane.Panel1, "splitContainerLeftPane.Panel1");
            // 
            // splitContainerLeftPane.Panel2
            // 
            this.splitContainerLeftPane.Panel2.Controls.Add(this.treeViewSubscriptionList);
            this.splitContainerLeftPane.Panel2.Controls.Add(this.lblSubscriptions);
            resources.ApplyResources(this.splitContainerLeftPane.Panel2, "splitContainerLeftPane.Panel2");
            // 
            // treeViewReportList
            // 
            resources.ApplyResources(this.treeViewReportList, "treeViewReportList");
            this.treeViewReportList.HideSelection = false;
            this.treeViewReportList.ItemHeight = 20;
            this.treeViewReportList.Name = "treeViewReportList";
            this.treeViewReportList.ShowNodeToolTips = true;
            this.treeViewReportList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewReportList_AfterSelect);
            // 
            // lblReports
            // 
            resources.ApplyResources(this.lblReports, "lblReports");
            this.lblReports.Name = "lblReports";
            // 
            // treeViewSubscriptionList
            // 
            resources.ApplyResources(this.treeViewSubscriptionList, "treeViewSubscriptionList");
            this.treeViewSubscriptionList.HideSelection = false;
            this.treeViewSubscriptionList.ItemHeight = 20;
            this.treeViewSubscriptionList.Name = "treeViewSubscriptionList";
            this.treeViewSubscriptionList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSubscriptionList_AfterSelect);
            // 
            // lblSubscriptions
            // 
            resources.ApplyResources(this.lblSubscriptions, "lblSubscriptions");
            this.lblSubscriptions.Name = "lblSubscriptions";
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainerLeftPane);
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.wlbReportView1);
            this.splitContainer1.Panel2.Controls.Add(this.subscriptionView1);
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            // 
            // wlbReportView1
            // 
            resources.ApplyResources(this.wlbReportView1, "wlbReportView1");
            this.wlbReportView1.Hosts = null;
            this.wlbReportView1.MinimumSize = new System.Drawing.Size(350, 350);
            this.wlbReportView1.IsCreedenceOrLater = false;
            this.wlbReportView1.Name = "wlbReportView1";
            this.wlbReportView1.Pool = null;
            this.wlbReportView1.ResetReportViewer = false;
            this.wlbReportView1.ViewerLocalReport = null;
            this.wlbReportView1.ViewerReportInfo = null;
            this.wlbReportView1.Close += new System.EventHandler(this.wlbReportView1_Close);
            this.wlbReportView1.ReportDrilledThrough += new Microsoft.Reporting.WinForms.DrillthroughEventHandler(this.wlbReportView1_ReportDrilledThrough);
            this.wlbReportView1.PoolConnectionLost += new System.EventHandler(this.wlbReportView1_PoolConnectionLost);
            // 
            // subscriptionView1
            // 
            resources.ApplyResources(this.subscriptionView1, "subscriptionView1");
            this.subscriptionView1.BackColor = System.Drawing.SystemColors.Control;
            this.subscriptionView1.MinimumSize = new System.Drawing.Size(671, 278);
            this.subscriptionView1.Name = "subscriptionView1";
            this.subscriptionView1.Pool = null;
            this.subscriptionView1.Close += new System.EventHandler(this.wlbReportView1_Close);
            this.subscriptionView1.PoolConnectionLost += new System.EventHandler(this.wlbReportView1_PoolConnectionLost);
            // 
            // contextMenuReports
            // 
            this.contextMenuReports.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runReportToolStripMenuItem});
            this.contextMenuReports.Name = "contextMenuReports";
            resources.ApplyResources(this.contextMenuReports, "contextMenuReports");
            // 
            // runReportToolStripMenuItem
            // 
            this.runReportToolStripMenuItem.Name = "runReportToolStripMenuItem";
            resources.ApplyResources(this.runReportToolStripMenuItem, "runReportToolStripMenuItem");
            // 
            // WorkloadReports
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.HelpButton = true;
            this.Icon = global::XenAdmin.Properties.Resources.AppIcon;
            this.Name = "WorkloadReports";
            this.Load += new System.EventHandler(this.ReportForm_Load);
            this.Shown += new System.EventHandler(this.WlbReportWindow_Shown);
            this.splitContainerLeftPane.Panel1.ResumeLayout(false);
            this.splitContainerLeftPane.Panel1.PerformLayout();
            this.splitContainerLeftPane.Panel2.ResumeLayout(false);
            this.splitContainerLeftPane.Panel2.PerformLayout();
            this.splitContainerLeftPane.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuReports.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainerLeftPane;
        private System.Windows.Forms.TreeView treeViewReportList;
        private System.Windows.Forms.Label lblReports;
        private System.Windows.Forms.TreeView treeViewSubscriptionList;
        private System.Windows.Forms.Label lblSubscriptions;
        private XenAdmin.Controls.Wlb.WlbReportView wlbReportView1;
        private XenAdmin.Controls.Wlb.WlbReportSubscriptionView subscriptionView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuReports;
        private System.Windows.Forms.ToolStripMenuItem runReportToolStripMenuItem;
    }
}
