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
using System.ComponentModel;
using System.Windows.Forms;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Controls
{
    public partial class PoolHostPicker : CustomTreeView
    {
        public EventHandler<SelectedItemEventArgs> SelectedItemChanged;
        public bool SupressErrors = false;

        public PoolHostPicker()
        {
            InitializeComponent();
            CollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged);
            ShowCheckboxes = false;
            ShowDescription = true;
            ShowImages = true;

            buildList();

            ConnectionsManager.XenConnections.CollectionChanged += CollectionChanged;
        }

		public override int ItemHeight { get { return 18; } }

        private CollectionChangeEventHandler CollectionChangedWithInvoke;
        void CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.BeginInvoke(this, buildList);
        }

        void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "name_label" || e.PropertyName == "metrics" || e.PropertyName == "enabled" || e.PropertyName == "live" || e.PropertyName == "patches")
                Program.Invoke(this, buildList);
        }

        public void buildList()
        {
            Program.AssertOnEventThread();

            Host selectedhost = null;
            IXenConnection selectedconnection = null;
            if (SelectedItem != null)
            {
                if (SelectedItem is HostItem)
                {
                    selectedhost = (SelectedItem as HostItem).TheHost;
                }
                else if (SelectedItem is PoolItem)
                {
                    selectedconnection = (SelectedItem as PoolItem).Connection;
                }
            }
            BeginUpdate();
            try
            {
                ClearAllNodes();
                foreach (IXenConnection xc in ConnectionsManager.XenConnectionsCopy)
                {
                    if (Helpers.GetPool(xc) != null)
                    {
                        PoolItem item = new PoolItem(xc);
                        if (SupressErrors)
                        {
                            item.Enabled = false;
                            item.Description = "";
                        }
                        AddNode(item);
                        foreach (Host host in xc.Cache.Hosts)
                        {
                            HostItem item2 = new HostItem(host);
                            if (SupressErrors)
                            {
                                item2.Enabled = host.IsLive();
                                item2.Description = "";
                            }
                            AddChildNode(item, item2);
                            host.PropertyChanged -= PropertyChanged;
                            host.PropertyChanged += PropertyChanged;

                        }
                    }
                    else if (xc.IsConnected)
                    {
                        Host host = Helpers.GetMaster(xc);
                        if (host != null)
                        {
                            HostItem item = new HostItem(host);
                            if (SupressErrors)
                            {
                                item.Enabled = host.IsLive();
                                item.Description = "";
                            }
                            AddNode(item);
                            host.PropertyChanged -= PropertyChanged;
                            host.PropertyChanged += PropertyChanged;
                        }
                        else
                        {
                            PoolItem item = new PoolItem(xc);
                            if (SupressErrors)
                            {
                                item.Enabled = false;
                                item.Description = "";
                            }
                            AddNode(item);
                        }
                    }
                    else
                    {
                        PoolItem item = new PoolItem(xc);
                        if (SupressErrors)
                        {
                            item.Enabled = false;
                            item.Description = "";
                        }
                        AddNode(item);
                    }

                    Pool pool = Helpers.GetPoolOfOne(xc);
                    if (pool != null)
                    {
                        pool.PropertyChanged -= PropertyChanged;
                        pool.PropertyChanged += PropertyChanged;
                    }

                    xc.ConnectionStateChanged -= xc_ConnectionStateChanged;
                    xc.ConnectionStateChanged += xc_ConnectionStateChanged;
                    xc.CachePopulated -= xc_CachePopulated;
                    xc.CachePopulated += xc_CachePopulated;
                    xc.Cache.RegisterCollectionChanged<Host>(CollectionChangedWithInvoke);
                }
            }
            finally
            {
                EndUpdate();
                if (selectedhost != null)
                    SelectHost(selectedhost);
                else if (selectedconnection != null)
                    SelectConnection(selectedconnection);
                else if (SelectedItemChanged != null)
                    SelectedItemChanged(null, new SelectedItemEventArgs(false));
            }
        }

        void xc_CachePopulated(IXenConnection conn)
        {
            Program.Invoke(this, buildList);
        }

        void xc_ConnectionStateChanged(IXenConnection conn)
        {
            Program.Invoke(this, buildList);
        }

        private void UnregisterHandlers()
        {
            ConnectionsManager.XenConnections.CollectionChanged -= CollectionChanged;
            foreach (IXenConnection xc in ConnectionsManager.XenConnectionsCopy)
            {
                Pool pool = Helpers.GetPoolOfOne(xc);
                if (pool != null)
                    pool.PropertyChanged -= PropertyChanged;

                foreach (Host host in xc.Cache.Hosts)
                    host.PropertyChanged -= PropertyChanged;

                xc.ConnectionStateChanged -= xc_ConnectionStateChanged;
                xc.CachePopulated -= xc_CachePopulated;
                xc.Cache.DeregisterCollectionChanged<Host>(CollectionChangedWithInvoke);
            }
        } 

        private CustomTreeNode lastSelected;
        public bool AllowPoolSelect = true;

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (SelectedItem is CustomTreeNode)
            {
                CustomTreeNode item = SelectedItem as CustomTreeNode;
                if (!item.Enabled)
                {
                    SelectedItem = lastSelected;
                }
                if (!AllowPoolSelect && item is PoolItem)
                {
                    SelectedItem = lastSelected;
                }
            }

            lastSelected = SelectedItem as CustomTreeNode;

            base.OnSelectedIndexChanged(e);
            if(SelectedItemChanged != null)
                SelectedItemChanged(null,new SelectedItemEventArgs((SelectedItem is PoolItem || SelectedItem is HostItem) && (SelectedItem as CustomTreeNode).Enabled));
        }

        private bool SelectNextEnabledNode(CustomTreeNode currentNode, bool searchForward)
        {
            CustomTreeNode nextEnabledNode = GetNextEnabledNode(currentNode, searchForward);
            if (nextEnabledNode != null)
            {
                SelectedItem = nextEnabledNode;
                return true;
            }
            return false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            var node = SelectedItem as CustomTreeNode;
            if (node != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                        {
                            e.Handled = SelectNextEnabledNode(node, true);
                            break;
                        }
                    case Keys.Up:
                        {
                            e.Handled = SelectNextEnabledNode(node, false);
                            break;
                        }
                }
            }
            base.OnKeyDown(e);
        }

        public IXenConnection ChosenConnection
        {
            get
            {
                if (SelectedItem == null || SelectedItem is HostItem || !(SelectedItem as CustomTreeNode).Enabled)
                    return null;
                return (SelectedItem as PoolItem).Connection;
            }
        }

        public Host ChosenHost
        {
            get
            {
                if (SelectedItem == null || SelectedItem is PoolItem || !(SelectedItem as CustomTreeNode).Enabled)
                    return null;
                return (SelectedItem as HostItem).TheHost;
            }
        }

        public void SelectHost(Host host)
        {
            if (host == null)
            {
                return;
            }
            foreach (CustomTreeNode item in Items)
            {
                if (TryToSelectHost(item, host))
                    return;

                if (item is PoolItem)
                {
                    foreach (CustomTreeNode childItem in item.ChildNodes)
                    {
                        if (TryToSelectHost(childItem, host))
                            return;
                    }
                }
            }
            OnSelectedIndexChanged(null);
        }

        /// <summary>
        /// Tries to select the node if it is a host item. If it is a host item, but is disabled, selects its parent instead.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="host"></param>
        /// <returns>True if successful</returns>
        private bool TryToSelectHost(CustomTreeNode item, Host host)
        {
            if (item is HostItem)
            {
                HostItem hostitem = item as HostItem;
                if (hostitem.TheHost.opaque_ref == host.opaque_ref)
                {
                    if (hostitem.Enabled)
                    {
                        SelectedItem = hostitem;
                        return true;
                    }
                    else if (hostitem.ParentNode is PoolItem)
                    {
                        SelectConnection(host.Connection);
                        return true;
                    }
                }
            }
            return false;
        }

        public void SelectConnection(IXenConnection xenConnection)
        {
            foreach (CustomTreeNode item in Items)
            {
                if (item is PoolItem)
                {
                    PoolItem poolitem = item as PoolItem;
                    if (poolitem.Connection.Equals(xenConnection))
                    {
                        SelectedItem = poolitem;
                        return;
                    }
                }
            }
            OnSelectedIndexChanged(null);
        }

        internal void SelectFirstThing()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] is CustomTreeNode && (Items[i] as CustomTreeNode).Enabled)
                {
                    if (!AllowPoolSelect && Items[i] is PoolItem)
                        continue;
                    SelectedIndex = i;
                    return;
                }
            }
        }
    }

    public class PoolItem : CustomTreeNode
    {
        public IXenConnection Connection;
        public PoolItem(IXenConnection xc)
        {
            Connection = xc;
            Update();
        }

        public void Update()
        {
            this.Image = Images.GetImage16For(Connection);
            this.Text = Helpers.GetName(Connection);
            this.Enabled = Connection.IsConnected && (Helpers.GetPool(Connection) == null || Helpers.HasFullyConnectedSharedStorage(Connection));
            if (Enabled)
                this.Description = "";
            else if (!Connection.IsConnected)
                this.Description = Messages.DISCONNECTED;
            else
                this.Description = Messages.POOL_HAS_NO_SHARED_STORAGE;

        }

        protected override int SameLevelSortOrder(CustomTreeNode other)
        {
            if (Enabled && !other.Enabled)
                return -1;
            else if (!Enabled && other.Enabled)
                return 1;
            else
                return base.SameLevelSortOrder(other);
        }
    }

    public class HostItem : CustomTreeNode
    {
        public Host TheHost;

        public HostItem(Host host)
        {
            TheHost = host;
            Update();
        }

        public void Update()
        {
            this.Image = Images.GetImage16For(TheHost);
            this.Text = Helpers.GetName(TheHost);
            bool isLiveHost = TheHost.IsLive();
            this.Enabled = isLiveHost && CanCreateVMsWithAffinityTo(TheHost);
            if (Enabled)
                this.Description = "";
            else if (!isLiveHost)
                this.Description = Messages.HOST_NOT_LIVE;
            else
                this.Description = Messages.HOST_SEES_NO_STORAGE;
        }

        protected override int SameLevelSortOrder(CustomTreeNode other)
        {
            if (Enabled && !other.Enabled)
                return -1;
            else if (!Enabled && other.Enabled)
                return 1;
            else if (Enabled && other.Enabled && other is HostItem)
                return TheHost.CompareTo(((HostItem)other).TheHost);
            else
                return base.SameLevelSortOrder(other);
        }

        private static bool CanCreateVMsWithAffinityTo(Host TheHost)
        {
            if (Helpers.HasFullyConnectedSharedStorage(TheHost.Connection))
                return true;
            else
            {
                foreach (SR sr in TheHost.Connection.Cache.SRs)
                {
                    if (sr.CanBeSeenFrom(TheHost) && sr.CanCreateVmOn())
                        return true;
                }
            }
            return false;
        }
    }

    public class SelectedItemEventArgs : EventArgs
    {
        public bool SomethingSelected;

        public SelectedItemEventArgs(bool notnull)
        {
            SomethingSelected = notnull;
        }
    }
}
