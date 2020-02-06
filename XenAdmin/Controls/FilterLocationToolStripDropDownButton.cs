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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Controls
{
    class FilterLocationToolStripDropDownButton : ToolStripDropDownButton
    {
        [Browsable(true)]
        public event Action FilterChanged;

        /// <summary>
        /// Maintain a list of all the objects we currently have events on for clearing out on rebuild
        /// </summary>
        private List<IXenConnection> connectionsWithEvents = new List<IXenConnection>();
        private List<Pool> poolsWithEvents = new List<Pool>();
        private List<Host> hostsWithEvents = new List<Host>();
        /// <summary>
        /// Store only host check states because the pools can be in an
        /// indeterminate state.
        /// </summary>
        private Dictionary<string, bool> HostCheckStates = new Dictionary<string, bool>();
        private readonly CollectionChangeEventHandler m_hostCollectionChangedWithInvoke;
        private bool inFilterListUpdate;
        private bool retryFilterListUpdate;

        private ToolStripMenuItem toolStripMenuItemAll;

        public FilterLocationToolStripDropDownButton()
        {
            ConnectionsManager.XenConnections.CollectionChanged += XenConnections_CollectionChanged;
            m_hostCollectionChangedWithInvoke = Program.ProgramInvokeHandler(Host_CollectionChanged);
        }

        protected override void Dispose(bool disposing)
        {
            DeregisterEvents();
            ConnectionsManager.XenConnections.CollectionChanged -= XenConnections_CollectionChanged;
            base.Dispose(disposing);
        }

        private void OnFilterChanged()
        {
            if (FilterChanged != null)
                FilterChanged();
        }

        /// <summary>
        /// Build the list of hosts to filter by for the first time and set all
        /// of them to be checked
        /// </summary>
        public void InitializeHostList()
        {
            foreach (IXenConnection c in ConnectionsManager.XenConnectionsCopy)
            {
                foreach (Host h in c.Cache.Hosts)
                    HostCheckStates[h.uuid] = true;
            }
        }

        public void RefreshLists()
        {
            BuildFilterList();
            OnFilterChanged();
        }

        public void BuildFilterList()
        {
            Program.AssertOnEventThread();

            if (inFilterListUpdate)
            {
                // queue up an update after the current one has finished,
                // in case the update has missed the relevant change
                retryFilterListUpdate = true;
                return;
            }

            inFilterListUpdate = true;

            try
            {
                DropDownItems.Clear();
                DeregisterEvents();
                RegisterEvents();

                foreach (IXenConnection c in ConnectionsManager.XenConnectionsCopy)
                {
                    Pool p = Helpers.GetPool(c);

                    if (p == null)// Stand alone host
                    {
                        foreach (Host h in c.Cache.Hosts)
                        {
                            var item = GenerateFilterItem(h, h.uuid);
                            item.Checked = HostCheckStates.ContainsKey(h.uuid);
                            DropDownItems.Add(item);
                            break;
                        }
                    }
                    else
                        DropDownItems.Add(GeneratePoolFilterItem(p));
                }

                if (DropDownItems.Count > 0)
                {
                    toolStripMenuItemAll = new ToolStripMenuItem
                        {
                            Text = Messages.FILTER_SHOW_ALL,
                            Enabled = FilterIsOn
                        };

                    DropDownItems.AddRange(new ToolStripItem[]
                        {
                            new ToolStripSeparator(),
                            toolStripMenuItemAll
                        });
                }

                Enabled = DropDownItems.Count > 0;
            }
            finally
            {
                inFilterListUpdate = false;
                if (retryFilterListUpdate)
                {
                    // there was a request to update while we were building,
                    // rebuild in case we missed something
                    retryFilterListUpdate = false;
                    BuildFilterList();
                }
            }
        }

        public bool HideByLocation(string hostUuid)
        {
            if (hostUuid == null)
                return false;

            if (HostCheckStates.ContainsKey(hostUuid) && !HostCheckStates[hostUuid])
                return true;

            return false;
        }

        public bool HideByLocation(List<string> hostUuids)
        {
            if (hostUuids.Count == 0)
                return false;

            return hostUuids.TrueForAll(uuid => HostCheckStates.ContainsKey(uuid) && !HostCheckStates[uuid]);
        }
        
        public bool FilterIsOn
        {
            get { return HostCheckStates.ContainsValue(false); }
        }

        private ToolStripMenuItem GeneratePoolFilterItem(Pool p)
        {
            List<ToolStripMenuItem> subItems = new List<ToolStripMenuItem>();

            foreach (Host h in p.Connection.Cache.Hosts)
            {
                var hostItem = GenerateFilterItem(h, h.uuid);
                hostItem.Checked = HostCheckStates.ContainsKey(h.uuid);
                subItems.Add(hostItem);
            }

            var poolItem = GenerateFilterItem(p, p.uuid);
            poolItem.DropDownItems.AddRange(subItems.ToArray());
            poolItem.CheckState = subItems.TrueForAll(item => item.Checked)
                                      ? CheckState.Checked
                                      : subItems.TrueForAll(item => !item.Checked)
                                            ? CheckState.Unchecked
                                            : CheckState.Indeterminate;
            poolItem.DropDownItemClicked += poolItem_DropDownItemClicked;
            return poolItem;
        }

        private ToolStripMenuItem GenerateFilterItem(IXenObject xenObject, string xenObjectUuid)
        {
            var item = new ToolStripMenuItem
                           {
                               Text = Helpers.GetName(xenObject),
                               Tag = xenObjectUuid
                           };
            return item;
        }

        private void RegisterEvents()
        {
            foreach (IXenConnection c in ConnectionsManager.XenConnectionsCopy)
            {
                c.ConnectionStateChanged += connection_ConnectionStateChanged;
                c.Cache.RegisterCollectionChanged<Host>(m_hostCollectionChangedWithInvoke);
                c.CachePopulated += connection_CachePopulated;
                connectionsWithEvents.Add(c);

                foreach (var pool in c.Cache.Pools)
                {
                    pool.PropertyChanged += pool_PropertyChanged;
                    poolsWithEvents.Add(pool);
                }

                foreach (Host host in c.Cache.Hosts)
                {
                    RegisterHostEvents(host);
                    hostsWithEvents.Add(host);
                }
            }
        }

        private void DeregisterEvents()
        {
            foreach (IXenConnection c in connectionsWithEvents)
            {
                c.ConnectionStateChanged -= connection_ConnectionStateChanged;
                c.Cache.DeregisterCollectionChanged<Host>(m_hostCollectionChangedWithInvoke);
                c.CachePopulated -= connection_CachePopulated;
            }

            foreach (var pool in poolsWithEvents)
                pool.PropertyChanged -= pool_PropertyChanged;

            foreach (Host h in hostsWithEvents)
                DeregisterHostEvents(h);
            
            connectionsWithEvents.Clear();
            poolsWithEvents.Clear();
            hostsWithEvents.Clear();
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

        private void connection_ConnectionStateChanged(IXenConnection conn)
        {
            Program.Invoke(Program.MainWindow, RefreshLists);
        }

        private void connection_CachePopulated(IXenConnection conn)
        {
            Program.Invoke(Program.MainWindow, RefreshLists);
        }

        private void XenConnections_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Add)
            {
                IXenConnection connection = e.Element as IXenConnection;

                foreach (Host host in connection.Cache.Hosts)
                    HostCheckStates[host.uuid] = true;
            }
        }

        private void Host_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Add)
                HostCheckStates[((Host)e.Element).uuid] = true;

            Program.Invoke(Parent, RefreshLists);
        }

        private void pool_PropertyChanged(object obj, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "other_config" || e.PropertyName == "name_label")
                Program.Invoke(Parent, RefreshLists);
        }

        private void hostMetrics_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "live")
                Program.Invoke(Parent, RefreshLists);
        }

        private void host_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label" || e.PropertyName == "metrics")
                Program.Invoke(Parent, RefreshLists);
        }

        protected override void OnDropDownItemClicked(ToolStripItemClickedEventArgs e)
        {
            //this method pertains to pool or stand alone host items
            base.OnDropDownItemClicked(e);
            HandleItemClicked(e.ClickedItem as ToolStripMenuItem);
        }

        private void poolItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            HandleItemClicked(e.ClickedItem as ToolStripMenuItem);
        }

        private void HandleItemClicked(ToolStripMenuItem item)
        {
            if (item == null)
                return;

            string uuid = (string)item.Tag;

            if (item.HasDropDownItems)
            {
                //this is a pool node

                item.CheckState = item.CheckState == CheckState.Checked
                                      ? CheckState.Unchecked
                                      : CheckState.Checked;

                foreach (ToolStripMenuItem child in item.DropDownItems)
                {
                    child.Checked = item.Checked;
                    string hostUuid = (string)child.Tag;
                    HostCheckStates[hostUuid] = child.Checked;
                }

                toolStripMenuItemAll.Enabled = FilterIsOn;
            }
            else if (item == toolStripMenuItemAll)
            {
                toolStripMenuItemAll.Enabled = false;
                InitializeHostList();
                BuildFilterList();
            }
            else
            {
                //this is a host node

                item.Checked = !item.Checked;
                HostCheckStates[uuid] = item.Checked;

                ToolStripMenuItem poolItem = item.OwnerItem as ToolStripMenuItem;

                if (poolItem != null)
                {
                    //this is not a standalone host; change the parent pool's check state

                    var itemArray = new ToolStripMenuItem[poolItem.DropDownItems.Count];
                    poolItem.DropDownItems.CopyTo(itemArray, 0);

                    poolItem.CheckState = Array.TrueForAll(itemArray, i => i.Checked)
                                              ? CheckState.Checked
                                              : Array.TrueForAll(itemArray, i => !i.Checked)
                                                    ? CheckState.Unchecked
                                                    : CheckState.Indeterminate;
                }

                toolStripMenuItemAll.Enabled = FilterIsOn;
            }

            OnFilterChanged();
        }
    }
}
