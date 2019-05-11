namespace XenAdmin.Controls.MainWindowControls
{
    partial class NavigationPane
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NavigationPane));
            this.splitContainer1 = new XenAdmin.Controls.Common.SmoothSplitContainer();
            this.navigationView = new XenAdmin.Controls.MainWindowControls.NavigationView();
            this.notificationsView = new XenAdmin.Controls.MainWindowControls.NotificationsView();
            this.toolStripBig = new XenAdmin.Controls.MainWindowControls.NavigationToolStripBig();
            this.buttonInfraBig = new XenAdmin.Controls.MainWindowControls.NavigationButtonBig();
            this.buttonObjectsBig = new XenAdmin.Controls.MainWindowControls.NavigationButtonBig();
            this.buttonOrganizationBig = new XenAdmin.Controls.MainWindowControls.NavigationDropDownButtonBig();
            this.toolStripMenuItemTags = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFolders = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFields = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemVapps = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonSearchesBig = new XenAdmin.Controls.MainWindowControls.NavigationDropDownButtonBig();
            this.buttonNotifyBig = new XenAdmin.Controls.MainWindowControls.NotificationButtonBig();
            this.toolStripSmall = new XenAdmin.Controls.MainWindowControls.NavigationToolStripSmall();
            this.buttonNotifySmall = new XenAdmin.Controls.MainWindowControls.NotificationButtonSmall();
            this.buttonSearchesSmall = new XenAdmin.Controls.MainWindowControls.NavigationDropDownButtonSmall();
            this.buttonOrganizationSmall = new XenAdmin.Controls.MainWindowControls.NavigationDropDownButtonSmall();
            this.buttonObjectsSmall = new XenAdmin.Controls.MainWindowControls.NavigationButtonSmall();
            this.buttonInfraSmall = new XenAdmin.Controls.MainWindowControls.NavigationButtonSmall();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStripBig.SuspendLayout();
            this.toolStripSmall.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.navigationView);
            this.splitContainer1.Panel1.Controls.Add(this.notificationsView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.toolStripBig);
            // 
            // navigationView
            // 
            resources.ApplyResources(this.navigationView, "navigationView");
            this.navigationView.Name = "navigationView";
            this.navigationView.DragDropCommandActivated += new System.Action<string>(this.navigationView_DragDropCommandActivated);
            this.navigationView.TreeViewSelectionChanged += new System.Action(this.navigationView_TreeViewSelectionChanged);
            this.navigationView.TreeNodeBeforeSelected += new System.Action(this.navigationView_TreeNodeBeforeSelected);
            this.navigationView.TreeNodeClicked += new System.Action(this.navigationView_TreeNodeClicked);
            this.navigationView.TreeNodeRightClicked += new System.Action(this.navigationView_TreeNodeRightClicked);
            this.navigationView.TreeViewRefreshed += new System.Action(this.navigationView_TreeViewRefreshed);
            this.navigationView.TreeViewRefreshSuspended += new System.Action(this.navigationView_TreeViewRefreshSuspended);
            this.navigationView.TreeViewRefreshResumed += new System.Action(this.navigationView_TreeViewRefreshResumed);
            // 
            // notificationsView
            // 
            resources.ApplyResources(this.notificationsView, "notificationsView");
            this.notificationsView.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.notificationsView.Name = "notificationsView";
            this.notificationsView.NotificationsSubModeChanged += new System.Action<XenAdmin.Controls.MainWindowControls.NotificationsSubModeItem>(this.notificationsView_NotificationsSubModeChanged);
            // 
            // toolStripBig
            // 
            resources.ApplyResources(this.toolStripBig, "toolStripBig");
            this.toolStripBig.ClickThrough = true;
            this.toolStripBig.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripBig.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonInfraBig,
            this.buttonObjectsBig,
            this.buttonOrganizationBig,
            this.buttonSearchesBig,
            this.buttonNotifyBig});
            this.toolStripBig.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStripBig.Name = "toolStripBig";
            this.toolStripBig.LayoutCompleted += new System.EventHandler(this.toolStripBig_LayoutCompleted);
            // 
            // buttonInfraBig
            // 
            this.buttonInfraBig.Image = global::XenAdmin.Properties.Resources.infra_view_24;
            resources.ApplyResources(this.buttonInfraBig, "buttonInfraBig");
            this.buttonInfraBig.Name = "buttonInfraBig";
            this.buttonInfraBig.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonInfraBig.PairedItem = null;
            // 
            // buttonObjectsBig
            // 
            this.buttonObjectsBig.Image = global::XenAdmin.Properties.Resources.objects_24;
            resources.ApplyResources(this.buttonObjectsBig, "buttonObjectsBig");
            this.buttonObjectsBig.Name = "buttonObjectsBig";
            this.buttonObjectsBig.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonObjectsBig.PairedItem = null;
            // 
            // buttonOrganizationBig
            // 
            this.buttonOrganizationBig.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemTags,
            this.toolStripMenuItemFolders,
            this.toolStripMenuItemFields,
            this.toolStripMenuItemVapps});
            this.buttonOrganizationBig.Image = global::XenAdmin.Properties.Resources.org_view_24;
            resources.ApplyResources(this.buttonOrganizationBig, "buttonOrganizationBig");
            this.buttonOrganizationBig.Name = "buttonOrganizationBig";
            this.buttonOrganizationBig.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonOrganizationBig.PairedItem = null;
            // 
            // toolStripMenuItemTags
            // 
            this.toolStripMenuItemTags.Image = global::XenAdmin.Properties.Resources._000_Tag_h32bit_16;
            this.toolStripMenuItemTags.Name = "toolStripMenuItemTags";
            resources.ApplyResources(this.toolStripMenuItemTags, "toolStripMenuItemTags");
            // 
            // toolStripMenuItemFolders
            // 
            this.toolStripMenuItemFolders.Image = global::XenAdmin.Properties.Resources._000_Folder_open_h32bit_16;
            this.toolStripMenuItemFolders.Name = "toolStripMenuItemFolders";
            resources.ApplyResources(this.toolStripMenuItemFolders, "toolStripMenuItemFolders");
            // 
            // toolStripMenuItemFields
            // 
            this.toolStripMenuItemFields.Image = global::XenAdmin.Properties.Resources._000_Fields_h32bit_16;
            this.toolStripMenuItemFields.Name = "toolStripMenuItemFields";
            resources.ApplyResources(this.toolStripMenuItemFields, "toolStripMenuItemFields");
            // 
            // toolStripMenuItemVapps
            // 
            this.toolStripMenuItemVapps.Image = global::XenAdmin.Properties.Resources._000_VirtualAppliance_h32bit_16;
            this.toolStripMenuItemVapps.Name = "toolStripMenuItemVapps";
            resources.ApplyResources(this.toolStripMenuItemVapps, "toolStripMenuItemVapps");
            // 
            // buttonSearchesBig
            // 
            this.buttonSearchesBig.Image = global::XenAdmin.Properties.Resources.saved_searches_24;
            resources.ApplyResources(this.buttonSearchesBig, "buttonSearchesBig");
            this.buttonSearchesBig.Name = "buttonSearchesBig";
            this.buttonSearchesBig.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonSearchesBig.PairedItem = null;
            // 
            // buttonNotifyBig
            // 
            this.buttonNotifyBig.Image = global::XenAdmin.Properties.Resources.notif_none_24;
            resources.ApplyResources(this.buttonNotifyBig, "buttonNotifyBig");
            this.buttonNotifyBig.Name = "buttonNotifyBig";
            this.buttonNotifyBig.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonNotifyBig.PairedItem = null;
            this.buttonNotifyBig.UnreadEntries = 0;
            // 
            // toolStripSmall
            // 
            this.toolStripSmall.CanOverflow = false;
            this.toolStripSmall.ClickThrough = true;
            resources.ApplyResources(this.toolStripSmall, "toolStripSmall");
            this.toolStripSmall.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripSmall.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonNotifySmall,
            this.buttonSearchesSmall,
            this.buttonOrganizationSmall,
            this.buttonObjectsSmall,
            this.buttonInfraSmall});
            this.toolStripSmall.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStripSmall.Name = "toolStripSmall";
            // 
            // buttonNotifySmall
            // 
            this.buttonNotifySmall.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buttonNotifySmall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonNotifySmall.Image = global::XenAdmin.Properties.Resources.notif_none_16;
            resources.ApplyResources(this.buttonNotifySmall, "buttonNotifySmall");
            this.buttonNotifySmall.Name = "buttonNotifySmall";
            this.buttonNotifySmall.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonNotifySmall.PairedItem = null;
            this.buttonNotifySmall.UnreadEntries = 0;
            this.buttonNotifySmall.Image = global::XenAdmin.Properties.Resources.notif_none_16;
            // 
            // buttonSearchesSmall
            // 
            this.buttonSearchesSmall.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buttonSearchesSmall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSearchesSmall.Image = global::XenAdmin.Properties.Resources.saved_searches_16;
            resources.ApplyResources(this.buttonSearchesSmall, "buttonSearchesSmall");
            this.buttonSearchesSmall.Name = "buttonSearchesSmall";
            this.buttonSearchesSmall.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonSearchesSmall.PairedItem = null;
            // 
            // buttonOrganizationSmall
            // 
            this.buttonOrganizationSmall.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buttonOrganizationSmall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonOrganizationSmall.Image = global::XenAdmin.Properties.Resources.org_view_16;
            resources.ApplyResources(this.buttonOrganizationSmall, "buttonOrganizationSmall");
            this.buttonOrganizationSmall.Name = "buttonOrganizationSmall";
            this.buttonOrganizationSmall.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonOrganizationSmall.PairedItem = null;
            // 
            // buttonObjectsSmall
            // 
            this.buttonObjectsSmall.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buttonObjectsSmall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonObjectsSmall.Image = global::XenAdmin.Properties.Resources.objects_16;
            resources.ApplyResources(this.buttonObjectsSmall, "buttonObjectsSmall");
            this.buttonObjectsSmall.Name = "buttonObjectsSmall";
            this.buttonObjectsSmall.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonObjectsSmall.PairedItem = null;
            // 
            // buttonInfraSmall
            // 
            this.buttonInfraSmall.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buttonInfraSmall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonInfraSmall.Image = global::XenAdmin.Properties.Resources.infra_view_16;
            resources.ApplyResources(this.buttonInfraSmall, "buttonInfraSmall");
            this.buttonInfraSmall.Name = "buttonInfraSmall";
            this.buttonInfraSmall.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.buttonInfraSmall.PairedItem = null;
            // 
            // NavigationPane
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStripSmall);
            this.Name = "NavigationPane";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStripBig.ResumeLayout(false);
            this.toolStripBig.PerformLayout();
            this.toolStripSmall.ResumeLayout(false);
            this.toolStripSmall.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private XenAdmin.Controls.MainWindowControls.NavigationToolStripSmall toolStripSmall;
        private XenAdmin.Controls.MainWindowControls.NavigationButtonSmall buttonInfraSmall;
        private XenAdmin.Controls.MainWindowControls.NavigationButtonSmall buttonObjectsSmall;
        private XenAdmin.Controls.MainWindowControls.NavigationDropDownButtonSmall buttonOrganizationSmall;
        private XenAdmin.Controls.MainWindowControls.NavigationDropDownButtonSmall buttonSearchesSmall;
        private XenAdmin.Controls.MainWindowControls.NotificationButtonSmall buttonNotifySmall;
        private XenAdmin.Controls.MainWindowControls.NavigationToolStripBig toolStripBig;
        private XenAdmin.Controls.MainWindowControls.NavigationButtonBig buttonInfraBig;
        private XenAdmin.Controls.MainWindowControls.NavigationButtonBig buttonObjectsBig;
        private XenAdmin.Controls.MainWindowControls.NavigationDropDownButtonBig buttonOrganizationBig;
        private XenAdmin.Controls.MainWindowControls.NavigationDropDownButtonBig buttonSearchesBig;
        private XenAdmin.Controls.MainWindowControls.NotificationButtonBig buttonNotifyBig;
        private XenAdmin.Controls.Common.SmoothSplitContainer splitContainer1;
        private NavigationView navigationView;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemTags;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFolders;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFields;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemVapps;
        private NotificationsView notificationsView;
    }
}
