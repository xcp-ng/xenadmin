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
using System.Windows.Forms;
using XenAdmin.Commands;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Controls;

using XenAdmin.Actions;

namespace XenAdmin.Dialogs
{
    public partial class NewPoolDialog : XenDialogBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<ConnectionWrapperWithMoreStuff> connections = new List<ConnectionWrapperWithMoreStuff>();
        private List<IXenConnection> newConnections = new List<IXenConnection>();

        public NewPoolDialog(Host host) 
        {
            InitializeComponent();
            customTreeViewServers.NodeIndent = 3;
            customTreeViewServers.ShowCheckboxes = true;
            customTreeViewServers.ShowDescription = true;
            customTreeViewServers.ShowImages = false;
            getAllCurrentConnections();
            ConnectionsManager.XenConnections.CollectionChanged += XenConnections_CollectionChanged;
            poolName = string.Empty; //forcing user to enter something before the Next button is enabled
            selectMaster(host);
            updateButtons();
        }

        public NewPoolDialog(List<Host> hosts)
            : this(hosts.Count >= 1 ? hosts[0] : null)
        {
            selectSlaves(hosts);
        }

        private enum InvalidReasons { NONE, EMPTY_POOL_NAME, NO_MASTER, MAX_POOL_SIZE_EXCEEDED };
        private InvalidReasons validToClose
        {
            get
            {
                if (poolName == "")
                    return InvalidReasons.EMPTY_POOL_NAME;

                if (comboBoxServers.SelectedIndex < 0)
                    return InvalidReasons.NO_MASTER;

                if (comboBoxServers.Items.Count <= 0)
                    return InvalidReasons.NO_MASTER;

                Host master = getMaster();
                if (master != null)
                {
                    List<Host> slaves = getSlaves();
                    if (PoolJoinRules.WillExceedPoolMaxSize(master.Connection, slaves.Count))
                        return InvalidReasons.MAX_POOL_SIZE_EXCEEDED;
                }

                return InvalidReasons.NONE;
            }
        }

        private string poolName
        {
            get { return textBoxName.Text.Trim(); }
            set { textBoxName.Text = value; }
        }

        private string poolDescription
        {
            get { return textBoxDescription.Text; }
        }

        private void updateButtons()
        {
            switch (validToClose)
            {
                case InvalidReasons.NONE:
                    buttonCreate.Enabled = true;
                    toolTipContainerCreate.RemoveAll();
                    break;
                case InvalidReasons.NO_MASTER:
                    buttonCreate.Enabled = false;
                    toolTipContainerCreate.SetToolTip(Messages.NO_ELIGIBLE_MASTER);
                    break;
                case InvalidReasons.EMPTY_POOL_NAME:
                    buttonCreate.Enabled = false;
                    toolTipContainerCreate.SetToolTip(Messages.POOL_NAME_EMPTY);
                    break;
                case InvalidReasons.MAX_POOL_SIZE_EXCEEDED:
                    buttonCreate.Enabled = false;
                    toolTipContainerCreate.SetToolTip(Messages.NEWPOOL_WILL_EXCEED_POOL_MAX_SIZE);
                    break;
            }
        }

        private void createPool()
        {
            try
            {
                Host master = getMaster();
                if (master == null)
                {
                    log.Error("Disconnected during create pool");
                    return;
                }
                List<Host> slaves = getSlaves();
                // Check supp packs and warn
                List<string> badSuppPacks = PoolJoinRules.HomogeneousSuppPacksDiffering(slaves, master);
                if (!HelpersGUI.GetPermissionFor(badSuppPacks, sp => true,
                    Messages.NEW_POOL_SUPP_PACK, Messages.NEW_POOL_SUPP_PACKS, false, "PoolJoinSuppPacks"))
                {
                    return;
                }

                // Are there any hosts which are forbidden from masking their CPUs for licensing reasons?
                // If so, we need to show upsell.
                if (null != slaves.Find(host =>
                    !PoolJoinRules.CompatibleCPUs(host, master, false) &&
                    Helpers.FeatureForbidden(host, Host.RestrictCpuMasking) &&
                    !PoolJoinRules.FreeHostPaidMaster(host, master, false)))  // in this case we can upgrade the license and then mask the CPU
                {
                    using (var dlg = new UpsellDialog(HiddenFeatures.LinkLabelHidden ? Messages.UPSELL_BLURB_CPUMASKING : Messages.UPSELL_BLURB_CPUMASKING + Messages.UPSELL_BLURB_TRIAL,
                                                        InvisibleMessages.UPSELL_LEARNMOREURL_TRIAL))
                        dlg.ShowDialog(this);
                    return;
                }

                if (!HelpersGUI.GetPermissionFor(slaves, host => PoolJoinRules.FreeHostPaidMaster(host, master, false),
                    Messages.NEW_POOL_LICENSE_MESSAGE, Messages.NEW_POOL_LICENSE_MESSAGE_MULTIPLE, true, "PoolJoinRelicensing")
                    ||
                    !HelpersGUI.GetPermissionFor(slaves, host => !PoolJoinRules.CompatibleCPUs(host, master, false),
                    Messages.NEW_POOL_CPU_MASKING_MESSAGE, Messages.NEW_POOL_CPU_MASKING_MESSAGE_MULTIPLE, true, "PoolJoinCpuMasking")
                    ||
                    !HelpersGUI.GetPermissionFor(slaves, host => !PoolJoinRules.CompatibleAdConfig(host, master, false),
                    Messages.NEW_POOL_AD_MESSAGE, Messages.NEW_POOL_AD_MESSAGE_MULTIPLE, true, "PoolJoinAdConfiguring")
                    ||
                    !HelpersGUI.GetPermissionForCpuFeatureLevelling(slaves, Helpers.GetPoolOfOne(master.Connection)))
                {
                    return;
                }

                log.DebugFormat("Creating new pool {0} ({1}) with master {2}", poolName, poolDescription, Helpers.GetName(master));
                foreach (Host slave in slaves)
                    log.DebugFormat("  Slave {0}", Helpers.GetName(slave));

                new CreatePoolAction(master, slaves, poolName, poolDescription, AddHostToPoolCommand.GetAdPrompt, 
                    AddHostToPoolCommand.NtolDialog, ApplyLicenseEditionCommand.ShowLicensingFailureDialog).RunAsync();
            }
            catch (System.Net.WebException exn)
            {
                log.Debug(exn, exn);
            }
        }

        private void addConnectionsToListBox()
        {
            ConnectionWrapperWithMoreStuff master = (comboBoxServers.Items.Count > 0 ? (ConnectionWrapperWithMoreStuff)comboBoxServers.SelectedItem : null);
            foreach (ConnectionWrapperWithMoreStuff c in connections)
            {
                c.TheMaster = master;
                c.Refresh();
            }
            customTreeViewServers.BeginUpdate();
            try
            {
                customTreeViewServers.ClearAllNodes();
                foreach (ConnectionWrapperWithMoreStuff c in connections)
                {
                    customTreeViewServers.AddNode(c);
                }
                customTreeViewServers.Resort();
            }
            finally
            {
                customTreeViewServers.EndUpdate();
                customTreeViewServers.Invalidate();
            }
        }

        private void getAllCurrentConnections()
        {
            List<IXenConnection> toRemove = new List<IXenConnection>();
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (connection != null&&connection.IsConnected)
                {
                    bool contains = false;
                    foreach (ConnectionWrapperWithMoreStuff cw in connections)
                    {
                        if (cw.Connection == connection)
                        {
                            contains = true;
                            break;
                        }
                    }
                    if (!contains)
                    {
                        ConnectionWrapperWithMoreStuff c = new ConnectionWrapperWithMoreStuff(connection);
                        connections.Add(c);
                        connection.CachePopulated -= Connection_CachePopulated;
                        connection.CachePopulated += Connection_CachePopulated;
                        connection.ConnectionStateChanged -= connection_ConnectionStateChanged;
                        connection.ConnectionStateChanged += connection_ConnectionStateChanged;
                        if (newConnections.Contains(connection))
                        {
                            c.State = CheckState.Checked;
                            toRemove.Add(connection);
                        }
                    }
                }
            }
            foreach (IXenConnection connection in toRemove)
            {
                newConnections.Remove(connection);
            }
            connections.RemoveAll((Predicate<ConnectionWrapperWithMoreStuff>)delegate(ConnectionWrapperWithMoreStuff c)
            {
                return !ConnectionsManager.XenConnectionsCopy.Contains(c.Connection);
            });
            addConnectionsToComboBox();
        }

        private void addConnectionsToComboBox()
        {
            comboBoxServers.Items.Clear();
            ConnectionWrapperWithMoreStuff master = null;
            foreach (ConnectionWrapperWithMoreStuff wrappedConnection in connections)
            {
                wrappedConnection.Refresh();
                if (wrappedConnection.CanBeMaster)
                {
                    comboBoxServers.Items.Add(wrappedConnection);
                    if (wrappedConnection.WillBeMaster)
                        master = wrappedConnection;
                }
            }
            if (master != null)
            {
                comboBoxServers.SelectedItem = master;
            }
            else if (comboBoxServers.Items.Count > 0)
            {
                ConnectionWrapperWithMoreStuff defaultMaster = comboBoxServers.Items[0] as ConnectionWrapperWithMoreStuff;
                setAsMaster(defaultMaster);
                comboBoxServers.SelectedItem = defaultMaster;
            }
            addConnectionsToListBox();
            updateButtons();
        }

        private void selectMaster(Host master)
        {
            if (master == null || Helpers.GetPool(master.Connection) != null)
                return;
            foreach (ConnectionWrapperWithMoreStuff c in connections)
            {
                if (c.WillBeMaster)
                    c.State = CheckState.Unchecked;
            }
            foreach (ConnectionWrapperWithMoreStuff c in connections)
            {
                if (c.Connection == master.Connection)
                {
                    if (c.CanBeMaster)
                        setAsMaster(c);
                    return;
                }
            }
        }

        private void selectSlaves(List<Host> slaves)
        {
            foreach (ConnectionWrapperWithMoreStuff c in connections)
            {
                if (c.AllowedAsSlave &&
                    slaves.Find(slave => (c.Connection == slave.Connection)) != null)  // one of the slaves is the connection we're looking at
                {
                    c.State = CheckState.Checked;
                }
            }
        }

        private void setAsMaster(ConnectionWrapperWithMoreStuff defaultMaster)
        {
            foreach (ConnectionWrapperWithMoreStuff connectionWrapper in connections)
                connectionWrapper.TheMaster = defaultMaster;

            comboBoxServers.SelectedItem = defaultMaster;
            addConnectionsToListBox();
        }

        private Host getMaster()
        {
            ConnectionWrapperWithMoreStuff connectionToMaster = null;
            foreach (ConnectionWrapperWithMoreStuff connection in connections)
            {
                if (connection.WillBeMaster)
                {
                    connectionToMaster = connection;
                    break;
                }
            }
            return connectionToMaster != null ? Helpers.GetMaster(connectionToMaster.Connection) : null;
        }

        private List<Host> getSlaves()
        {
            List<Host> slaveConnections = new List<Host>();
            foreach (ConnectionWrapperWithMoreStuff connection in connections)
            {
                if (connection.State == CheckState.Checked && !connection.WillBeMaster && connection.AllowedAsSlave)
                {
                    slaveConnections.Add(Helpers.GetMaster(connection.Connection));
                }
            }
            return slaveConnections;
        }

        private void comboBoxServers_SelectionChangeCommitted(object sender, EventArgs e)
        {
            setAsMaster(comboBoxServers.SelectedItem as ConnectionWrapperWithMoreStuff);
        }

        private void buttonAddNewServer_Click(object sender, EventArgs e)
        {
            IXenConnection newConnection = new XenConnection();
            newConnections.Add(newConnection);
            new AddServerTask(this, newConnection, null).Start();
        }

        private void comboBoxServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateButtons();
        }

        private void customTreeViewServers_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Space || !(customTreeViewServers.SelectedItem is CustomTreeNode))
                return;
            CustomTreeNode node = (CustomTreeNode)customTreeViewServers.SelectedItem;
            if (!node.Enabled)
                return;
            node.State = node.State == CheckState.Unchecked ? CheckState.Checked : CheckState.Unchecked;
            customTreeViewServers.Refresh();
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            updateButtons();
        }

        void XenConnections_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.BeginInvoke(this, getAllCurrentConnections);
        }

        void Connection_CachePopulated(IXenConnection conn)
        {
            Program.BeginInvoke(this, addConnectionsToComboBox);
        }

        void connection_ConnectionStateChanged(IXenConnection conn)
        {
            Program.BeginInvoke(this, addConnectionsToComboBox);
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            createPool();
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void customTreeViewServers_ItemCheckChanged(object sender, EventArgs e)
        {
            updateButtons();
        }
    }
}
