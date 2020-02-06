/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Commands;


namespace XenAdmin.Wizards.BugToolWizardFiles
{
    public partial class GenericSelectHostsPage : XenTabPage
    {
        private readonly CollectionChangeEventHandler Host_CollectionChangedWithInvoke;
        private bool inupdate;

        public GenericSelectHostsPage()
        {
            InitializeComponent();

            buildList();
            Host_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(Host_CollectionChanged);
            HostListTreeView.ShowImages = true;
            HostListTreeView.ShowCheckboxes = true;
            HostListTreeView.ShowDescription = true;
            HostListTreeView.ItemCheckChanged += HostListTreeView_ItemCheckChanged;

            ConnectionsManager.XenConnections.CollectionChanged += XenConnections_CollectionChanged;
        }

        #region XenTabPage overrides

        public override string HelpID { get { return "SelectServers"; } }

        public override string Text { get { return Messages.BUGTOOL_PAGE_SERVERS_TEXT; } }

        public override string PageTitle { get { return Messages.BUGTOOL_PAGE_SERVERS_TITLE; } }

        public override bool EnableNext()
        {
            return HostListTreeView.CheckedItems().Count > 0;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            EnableDisableButtons();
        }

        #endregion

        public void SelectHosts(List<IXenObject> selectedObjects)
        {
            if (selectedObjects.Count == 0 || HostListTreeView.Items.Count == 0)
                return;

            HostListTreeView.BeginUpdate();
            HostListTreeView.ClearSelected();

            for (int index = 0; index < HostListTreeView.Items.Count; index++)
            {
                var node = HostListTreeView.Items[index] as HostCustomTreeNode;
                if (node == null)
                    continue;

                IXenConnection con = node.Tag as IXenConnection;
                if (con != null)
                {
                    var pool = Helpers.GetPool(con);
                    if (pool == null)
                    {
                        Host master = Helpers.GetMaster(con);
                        if (master != null && selectedObjects.Contains(master))
                            node.State = CheckState.Checked;
                    }
                    else
                    {
                        if (selectedObjects.Contains(pool))
                        {
                            foreach (var subnode in node.ChildNodes)
                                subnode.State = CheckState.Checked;
                        }
                    }
                    continue;
                }

                var host = node.Tag as Host;
                if (host != null && selectedObjects.Contains(host))
                    node.State = CheckState.Checked;
            }

            //focus on first checked item so the user can find it in a long list
            foreach (var node in HostListTreeView.CheckedItems())
            {
                HostListTreeView.SelectedItems.Add(node);
                break;
            }

            HostListTreeView.EndUpdate();
        }

        public List<Host> SelectedHosts
        {
            get
            {
                List<Host> hosts = new List<Host>();
                foreach (HostCustomTreeNode node in HostListTreeView.CheckedItems())
                {
                    IXenConnection c = node.Tag as IXenConnection;
                    if (c != null)
                    {
                        if (Helpers.GetPool(c) == null)
                        {
                            Host master = Helpers.GetMaster(c);
                            if (master != null)
                                hosts.Add(master);
                        }
                        continue;
                    }

                    Host host = node.Tag as Host;
                    if (host != null)
                        hosts.Add(host);
                }

                return hosts;
            }
        }

        private void buildList()
        {
            Program.AssertOnEventThread();

            if (inupdate)
                return;
            
            inupdate = true;
            HostListTreeView.BeginUpdate();
           
            try
            {
                // Save old checked states to preserve across update
                var oldCheckStates = new Dictionary<string, CheckState>();
                foreach (HostCustomTreeNode node in HostListTreeView.Items)
                {
                    if (node.HostOrPoolUuid != null)
                        oldCheckStates.Add(node.HostOrPoolUuid, node.State);
                }

                HostListTreeView.ClearAllNodes();
                DeregisterEvents();

                foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                {
                    connection.ConnectionStateChanged += connection_ConnectionStateChanged;
                    connection.Cache.RegisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);
                    connection.CachePopulated += connection_CachePopulated;

                    if (!connection.IsConnected)
                        continue;  // don't show disconnected connections CA-60514

                    HostCustomTreeNode node = new HostCustomTreeNode(true)
                                                  {
                                                      Text = Helpers.GetName(connection),
                                                      Tag = connection,
                                                      Enabled = true,
                                                      Description = "",
                                                      State = CheckState.Unchecked,
                                                      Image = Images.GetImage16For(connection)
                                                  };

                    HostListTreeView.AddNode(node);

                    // Save uuid of pool to this node
                    Pool pool = Helpers.GetPoolOfOne(connection);
                    if (pool == null)
                        continue;

                    pool.PropertyChanged += pool_PropertyChanged;
                    
                    node.HostOrPoolUuid = pool.uuid;
                    node.State = (oldCheckStates.ContainsKey(node.HostOrPoolUuid) && node.Enabled) ?
                        oldCheckStates[node.HostOrPoolUuid] :
                        CheckState.Unchecked;
                  
                    if (Helpers.GetPool(connection) != null)
                    {
                        node.Image = Images.GetImage16For(pool);

                        foreach (Host host in connection.Cache.Hosts)
                        {
                            bool isHostLive = host.IsLive();
                            HostCustomTreeNode childnode = new HostCustomTreeNode(true)
                                                               {
                                                                   Text = Helpers.GetName(host),
                                                                   Tag = host,
                                                                   HostOrPoolUuid = host.uuid,
                                                                   Enabled = isHostLive,
                                                                   Description = isHostLive ? "" : Messages.HOST_NOT_LIVE,
                                                                   Image = Images.GetImage16For(host)
                                                               };

                            childnode.State = (oldCheckStates.ContainsKey(childnode.HostOrPoolUuid) && childnode.Enabled) ?
                                oldCheckStates[childnode.HostOrPoolUuid] :
                                CheckState.Unchecked;

                            HostListTreeView.AddChildNode(node, childnode);
  
                            RegisterHostEvents(host);
                        }
                    }
                    else
                    {
                        Host master = Helpers.GetMaster(connection);
                        if (master != null)
                        {
                            bool isMasterLive = master.IsLive();
                            node.Enabled = isMasterLive;
                            node.Description = isMasterLive ? "" : Messages.HOST_NOT_LIVE;
                            node.Image = Images.GetImage16For(master);

                            RegisterHostEvents(master);
                        }
                    }
                }
            }
            finally
            {
                inupdate = false;
                HostListTreeView.EndUpdate();
                EnableDisableButtons();
            }
        }

        private void DeregisterEvents()
        {
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                connection.ConnectionStateChanged -= connection_ConnectionStateChanged;
                connection.Cache.DeregisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);
                connection.CachePopulated -= connection_CachePopulated;

                Pool pool = Helpers.GetPoolOfOne(connection);
                if (pool == null)
                    continue;

                pool.PropertyChanged -= pool_PropertyChanged;

                if (Helpers.GetPool(connection) != null)
                {
                    foreach (Host host in connection.Cache.Hosts)
                        DeregisterHostEvents(host);
                }
                else
                {
                    Host master = Helpers.GetMaster(connection);
                    if (master != null)
                        DeregisterHostEvents(master);
                }
            }
        }

        private void RegisterHostEvents(Host host)
        {
            Host_metrics metrics = host.Connection.Resolve(host.metrics);
            if (metrics != null)
                metrics.PropertyChanged += hostMetrics_PropertyChanged;
            host.PropertyChanged += host_PropertyChanged;
        }

        private void DeregisterHostEvents(Host host)
        {
            Host_metrics metrics = host.Connection.Resolve(host.metrics);
            if (metrics != null)
                metrics.PropertyChanged -= hostMetrics_PropertyChanged;
            host.PropertyChanged -= host_PropertyChanged;
        }

        private void connection_CachePopulated(IXenConnection conn)
        {
            Program.Invoke(this, buildList);
        }

        private void Host_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            buildList();
        }

        private void pool_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label" || e.PropertyName == "master")
            {
                Program.Invoke(this, buildList);
            }
        }

        private void hostMetrics_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "live")
            {
                Program.Invoke(this, buildList);
            }
        }

        private void host_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label" || e.PropertyName == "enabled" || e.PropertyName == "patches" || e.PropertyName == "metrics")
            {
                Program.Invoke(this, buildList);
            }
        }

        private void connection_ConnectionStateChanged(IXenConnection conn)
        {
            Program.Invoke(this, (CollectionChangeEventHandler)XenConnections_CollectionChanged, this, null);
        }

        private void XenConnections_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.BeginInvoke(Program.MainWindow,buildList);
        }

        private void EnableDisableButtons()
        {
            if (!Visible)
                return; // CA-28387

            OnPageUpdated();

            SelectAllButton.Enabled = HostListTreeView.CheckableItems().Count != 0;
            SelectNoneButton.Enabled = HostListTreeView.CheckedItems().Count > 0;
        }

        #region Control event handlers

        private void SelectNoneButton_Click(object sender, EventArgs e)
        {
            HostListTreeView.SecretNode.State = CheckState.Unchecked;
            HostListTreeView.Refresh();
            EnableDisableButtons();
        }

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            HostListTreeView.SecretNode.State = CheckState.Checked;
            HostListTreeView.Refresh();
            EnableDisableButtons();
        }

        private void connectbutton_Click(object sender, EventArgs e)
        {
            new AddHostCommand(Program.MainWindow, ParentForm).Execute();
        }

        private void HostListTreeView_ItemCheckChanged(object sender, EventArgs e)
        {
            EnableDisableButtons();
        }

        #endregion
    }

    public class HostCustomTreeNode : CustomTreeNode
    {
        public string HostOrPoolUuid;

        public HostCustomTreeNode(bool selectable)
            : base(selectable)
        {}

        // Fixed for CA-57131.
        // Though it would be better to use the usual search mechanism for building
        // the tree, so that the order is absolutely guaranteed to be the same.
        protected override int SameLevelSortOrder(CustomTreeNode other)
        {
            if (other.Tag is IXenConnection && this.Tag is IXenConnection)
            {
                return (this.Tag as IXenConnection).CompareTo(other.Tag as IXenConnection);
            }
            else if (other.Tag is IXenConnection)
            {
                return -1;
            }
            else if (this.Tag is IXenConnection)
            {
                return 1;
            }
            else if (this.Tag is Host && other.Tag is Host)
            {
                return (this.Tag as Host).CompareTo(other.Tag as Host);
            }
            else
            {
                return base.SameLevelSortOrder(other);
            }
        }
    }
}
