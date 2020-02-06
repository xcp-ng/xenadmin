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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;
using XenCenterLib;

namespace XenAdmin.TabPages
{
    internal partial class HAPage : BaseTabPage
    {
        private Pool pool;

        private IXenObject xenObject;

        private bool restartHBInitializationTimer;

        private bool initializationDelayElapsed;

        private System.Timers.Timer initializationDelayTimer;

        private const int HB_INITIALIZATION_DELAY = 30000;

        private readonly CollectionChangeEventHandler Host_CollectionChangedWithInvoke;
        /// <summary>
        /// The object that the panel is displaying HA info for. Must be set on the event thread.
        /// </summary>
        public IXenObject XenObject
        {
            set
            {
                Program.AssertOnEventThread();

                UnregisterHandlers();

                xenObject = value;
                pool = xenObject as Pool;
                if (pool != null)
                {
                    pool.PropertyChanged += pool_PropertyChanged;
                    foreach (var host in pool.Connection.Cache.Hosts)
                    {
                        host.PropertyChanged += host_PropertyChanged;
                    }
                    pool.Connection.Cache.RegisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);
                }

                Rebuild();
            }
        }

        public override string HelpID => "TabPageHA";

        public HAPage()
        {
            InitializeComponent();

            customListPanel.ContextMenuRequest += GP_ContextShow;
            Host_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(Host_CollectionChanged);
            ConnectionsManager.History.CollectionChanged += History_CollectionChanged;
            base.Text = Messages.HIGH_AVAILABILITY;
            restartHBInitializationTimer = true;
        }

        private void History_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            //Program.AssertOnEventThread();
            Program.BeginInvoke(Program.MainWindow, () =>
            {
                if (e.Action == CollectionChangeAction.Add &&
                    (e.Element is EnableHAAction || e.Element is DisableHAAction))
                {
                    AsyncAction action = (AsyncAction)e.Element;
                    action.Changed += action_Changed;
                    if (xenObject != null && xenObject.Connection == action.Connection)
                        Rebuild();
                }
            });
        }

        private void action_Changed(ActionBase sender)
        {
            // This seems to be called off the event thread
            AsyncAction action = (AsyncAction)sender;
            if (action.IsCompleted)
            {
                action.Changed -= action_Changed;
                Program.Invoke(Program.MainWindow, delegate()
                {
                    if (xenObject != null && xenObject.Connection == action.Connection)
                    {
                        Rebuild();
                    }
                });
            }
        }

        /// <summary>
        /// Registered on the connection's Host collection when the HAPage is showing a pool.
        /// Means we respond to hosts joining/leaving the pool.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Host_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    ((Host)e.Element).PropertyChanged += host_PropertyChanged;
                    break;
                case CollectionChangeAction.Remove:
                    ((Host)e.Element).PropertyChanged -= host_PropertyChanged;
                    break;
                case CollectionChangeAction.Refresh:
                    // As of writing, ChangeableDictionary never raises this kind of event
                    throw new NotImplementedException();
            }
        }

        private void UnregisterHandlers()
        {
            if (pool == null) 
                return;
            pool.PropertyChanged -= pool_PropertyChanged;
            foreach (var host in pool.Connection.Cache.Hosts)
            {
                host.PropertyChanged -= host_PropertyChanged;
            }
            pool.Connection.Cache.DeregisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);
        }

        public override void PageHidden()
        {
            UnregisterHandlers();
        }

        /// <summary>
        /// Rebuilds the panel contents. Must be called on the event thread.
        /// </summary>
        private void Rebuild()
        {
            Program.AssertOnEventThread();
            if (!this.Visible)
                return;
            customListPanel.BeginUpdate();
            try
            {
                customListPanel.ClearRows();
                tableLatencies.Controls.Clear();

                if (xenObject == null)
                    return;

                AsyncAction action = HelpersGUI.FindActiveHaAction(xenObject.Connection);
                if (action != null)
                {
                    // There is an EnableHAAction or DisableHAAction in progress relating to this connection.
                    // Show some text and disable editing
                    buttonConfigure.Enabled = false;
                    buttonDisableHa.Enabled = false;

                    pictureBoxWarningTriangle.Visible = false;
                    labelStatus.Text = String.Format(action is EnableHAAction ? Messages.HA_PAGE_ENABLING : Messages.HA_PAGE_DISABLING,
                        Helpers.GetName(xenObject.Connection));
                }
                else
                {
                    //CA-250234 no need to rebuild HA page if we lost connection to the pool 
                    if (pool.Connection == null || pool.Connection.Session == null || !pool.Connection.IsConnected)
                        return;

                    // Generate the tab contents depending on what XenObject we're displaying

                    if (pool.ha_enabled)
                    {
                        if (PassedRbacChecks())
                        {
                            bool haRestricted = Helpers.FeatureForbidden(pool, Host.RestrictHA);

                            buttonConfigure.Visible = !haRestricted;
                            buttonConfigure.Enabled = !haRestricted;
                            buttonDisableHa.Visible = true;
                            buttonDisableHa.Enabled = true;

                            pictureBoxWarningTriangle.Visible = false;
                            labelStatus.Text = string.Format(haRestricted ? Messages.HA_TAB_CONFIGURED_UNLICENSED : Messages.HA_TAB_CONFIGURED_BLURB, Helpers.GetName(pool).Ellipsise(30));
                        }
                        else
                        {
                            buttonConfigure.Visible = false;
                            buttonDisableHa.Visible = false;

                            pictureBoxWarningTriangle.Visible = true;
                            labelStatus.Text = String.Format(Messages.RBAC_HA_TAB_WARNING,
                                Role.FriendlyCSVRoleList(Role.ValidRoleList(HA_PERMISSION_CHECKS, pool.Connection)),
                                Role.FriendlyCSVRoleList(pool.Connection.Session.Roles));
                        }
                    }
                    else
                    {
                        buttonConfigure.Visible = true;
                        buttonConfigure.Enabled = true;
                        buttonDisableHa.Visible = false;

                        pictureBoxWarningTriangle.Visible = false;
                        labelStatus.Text = String.Format(Messages.HAPANEL_BLURB, Helpers.GetName(pool).Ellipsise(30));
                    }

                    var sr = xenObject as SR;
                    if (sr != null)
                    {
                        // Currently unused
                        generateSRHABox(sr);
                    }
                    else if (xenObject is Pool)
                    {
                        generatePoolHABox(pool);
                        tableLayoutPanel1.AutoScrollMinSize = new Size(0, tableLatencies.Height + customListPanel.Height);
                    }
                }
            }
            finally
            {
                customListPanel.EndUpdate();
            }
        }

        private RbacMethodList HA_PERMISSION_CHECKS = new RbacMethodList(
            "pool.set_ha_host_failures_to_tolerate",
            "pool.sync_database",
            "vm.set_ha_restart_priority",
            "pool.ha_compute_hypothetical_max_host_failures_to_tolerate"
        );

        private bool PassedRbacChecks()
        {
            return Role.CanPerform(HA_PERMISSION_CHECKS, pool.Connection);
        }

        private void pool_PropertyChanged(object sender1, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ha_enabled" || e.PropertyName == "ha_host_failures_to_tolerate"
                || e.PropertyName == "ha_overcommitted" || e.PropertyName == "ha_plan_exists_for"
                || e.PropertyName == "name_label")
            {
                Rebuild();
            }
        }

        private void host_PropertyChanged(object sender1, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label" || e.PropertyName == "ha_statefiles"
                || e.PropertyName == "ha_network_peers")
            {
                Rebuild();
            }
        }

        private string getPoolHAStatus(Pool pool)
        {
            return pool.ha_enabled ? Messages.YES : Messages.NO;
        }

        private void generatePoolHABox(Pool pool)
        {
            if (!pool.ha_enabled)
            {
                restartHBInitializationTimer = true;
                return;
            }

            if (restartHBInitializationTimer)
            {
                restartHBInitializationTimer = false;
                SetNetworkHBInitDelay();
            }
            
            // 'High Availability' heading
            CustomListRow header = CreateHeader(Messages.HA_CONFIGURATION_TITLE);
            customListPanel.AddRow(header);
            AddRow(header, GetFriendlyName("pool.ha_enabled"), pool, getPoolHAStatus, false);
            {
                // ntol row. May be red and bold.
                bool redBold = pool.ha_host_failures_to_tolerate == 0;

                CustomListRow newChild = CreateNewRow(Messages.HA_CONFIGURED_CAPACITY, new ToStringWrapper<Pool>(pool, getNtol), redBold);
                header.AddChild(newChild);

                if (redBold)
                {
                    newChild.Items[1].ForeColor = Color.Red;
                    ToolStripMenuItem editHa = new ToolStripMenuItem(Messages.CONFIGURE_HA_ELLIPSIS);
                    editHa.Click += delegate { EditHA(pool); };
                    newChild.MenuItems.Add(editHa);
                    newChild.DefaultMenuItem = editHa;
                }
                else
                {
                    newChild.Items[1].ForeColor = BaseTabPage.ItemValueForeColor;
                }
            }

            {
                // plan_exists_for row needs some special work: the text may be red and bold
                bool redBold = haStatusRed(pool);
                CustomListRow newChild = CreateNewRow(Messages.HA_CURRENT_CAPACITY, new ToStringWrapper<Pool>(pool, getPlanExistsFor), redBold);
                header.AddChild(newChild);

                if (redBold)
                {
                    newChild.Items[1].ForeColor = Color.Red;
                    ToolStripMenuItem editHa = new ToolStripMenuItem(Messages.CONFIGURE_HA_ELLIPSIS);
                    editHa.Click += delegate { EditHA(pool); };
                    newChild.MenuItems.Add(editHa);
                    newChild.DefaultMenuItem = editHa;
                }
                else
                {
                    newChild.Items[1].ForeColor = BaseTabPage.ItemValueForeColor;
                }
            }
            AddBottomSpacing(header);

            // 'Heartbeating status' heading
            header = CreateHeader(Messages.HEARTBEATING_STATUS);
            customListPanel.AddRow(header);

            // Now build the heartbeat target health table

            List<SR> heartbeatSRs = pool.GetHAHeartbeatSRs();
            // Sort heartbeat SRs using NaturalCompare
            heartbeatSRs.Sort((Comparison<SR>)delegate(SR a, SR b)
            {
                return StringUtility.NaturalCompare(a.Name(), b.Name());
            });

            List<Host> members = new List<Host>(pool.Connection.Cache.Hosts);
            // Sort pool members using NaturalCompare
            members.Sort((Comparison<Host>)delegate(Host a, Host b)
            {
                return StringUtility.NaturalCompare(a.Name(), b.Name());
            });
            int numCols = 1 + 2 + (2 * heartbeatSRs.Count); // Hostnames col, then 2 each for each HB target (network + SRs)
            int numRows = 1 + members.Count;

            // Create rows and cols
            tableLatencies.SuspendLayout();
            tableLatencies.ColumnCount = numCols;
            tableLatencies.ColumnStyles.Clear();
            for (int i = 0; i < numCols; i++)
            {
                tableLatencies.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            }
            tableLatencies.RowCount = numRows;
            tableLatencies.RowStyles.Clear();
            for (int i = 0; i < numRows; i++)
            {
                tableLatencies.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            {
                // Network icon
                PictureBox p = new PictureBox();
                p.Image = Images.GetImage16For(Icons.Network);
                p.SizeMode = PictureBoxSizeMode.AutoSize;
                p.Padding = new Padding(0);
                tableLatencies.Controls.Add(p);
                tableLatencies.SetCellPosition(p, new TableLayoutPanelCellPosition(1, 0));
                // Network text
                Label l = new Label();
                l.Padding = new Padding(0, 5, 5, 5);
                l.Font = BaseTabPage.ItemLabelFont;
                l.ForeColor = BaseTabPage.ItemLabelForeColor;
                l.AutoSize = true;
                l.Text = Messages.NETWORK;
                tableLatencies.Controls.Add(l);
                tableLatencies.SetCellPosition(l, new TableLayoutPanelCellPosition(2, 0));
            }

            for (int i = 0; i < heartbeatSRs.Count; i++)
            {
                // SR icon
                PictureBox p = new PictureBox();
                p.Image = Images.GetImage16For(Images.GetIconFor(heartbeatSRs[i]));
                p.SizeMode = PictureBoxSizeMode.AutoSize;
                p.Padding = new Padding(0);
                tableLatencies.Controls.Add(p);
                tableLatencies.SetCellPosition(p, new TableLayoutPanelCellPosition((2 * i) + 3, 0));
                // SR name
                Label l = new Label();
                l.Padding = new Padding(0, 5, 5, 5);
                l.Font = BaseTabPage.ItemLabelFont;
                l.ForeColor = BaseTabPage.ItemLabelForeColor;
                l.AutoSize = false;
                l.Size = new Size(200, 25);
                l.AutoEllipsis = true;
                l.Text = heartbeatSRs[i].Name();
                tableLatencies.Controls.Add(l);
                tableLatencies.SetCellPosition(l, new TableLayoutPanelCellPosition((2 * i) + 4, 0));
            }

            for (int i = 0; i < members.Count; i++)
            {
                // Server name label
                Label l = new Label();
                l.Padding = new Padding(5);
                l.Font = BaseTabPage.ItemLabelFont;
                l.ForeColor = BaseTabPage.ItemLabelForeColor;
                l.AutoSize = true;
                l.Text = members[i].Name().Ellipsise(30);
                tableLatencies.Controls.Add(l);
                tableLatencies.SetCellPosition(l, new TableLayoutPanelCellPosition(0, i + 1));

                // Network HB status
                l = new Label();
                l.Padding = new Padding(0, 5, 0, 5);
                l.Font = BaseTabPage.ItemValueFont;
                l.AutoSize = true;
                l.ForeColor = (members[i].ha_network_peers.Length == members.Count && initializationDelayElapsed) ? Color.Green : BaseTabPage.ItemValueForeColor;

                if (initializationDelayElapsed)
                {
                    if (members[i].ha_network_peers.Length == 0)
                    {
                        l.Text = Messages.HA_HEARTBEAT_UNHEALTHY;
                    }
                    else if (members[i].ha_network_peers.Length == members.Count)
                    {
                        l.Text = Messages.HA_HEARTBEAT_HEALTHY;
                    }
                    else
                    {
                        l.Text = String.Format(Messages.HA_HEARTBEAT_SERVERS, members[i].ha_network_peers.Length, members.Count);
                    }
                }                
                else
                {
                    l.Text = Messages.HA_HEARTBEAT_SERVERS_INITIALISING;
                }

                tableLatencies.Controls.Add(l);
                tableLatencies.SetCellPosition(l, new TableLayoutPanelCellPosition(1, i + 1));
                tableLatencies.SetColumnSpan(l, 2);

                // For each heartbeat SR, show health from this host's perspective
                for (int j = 0; j < heartbeatSRs.Count; j++)
                {
                    l = new Label();
                    l.Padding = new Padding(0, 5, 0, 5);
                    l.Font = BaseTabPage.ItemValueFont;
                    l.ForeColor = BaseTabPage.ItemValueForeColor;
                    l.AutoSize = true;
                    l.Text = Messages.HA_HEARTBEAT_UNHEALTHY;
                    foreach (string opaqueRef in members[i].ha_statefiles)
                    {
                        XenRef<VDI> vdiRef = new XenRef<VDI>(opaqueRef);
                        VDI vdi = pool.Connection.Resolve(vdiRef);
                        if (vdi == null)
                            continue;
                        SR sr = pool.Connection.Resolve(vdi.SR);
                        if (sr == null)
                            continue;
                        if (sr == heartbeatSRs[j])
                        {
                            l.ForeColor = Color.Green;
                            l.Text = Messages.HA_HEARTBEAT_HEALTHY;
                            break;
                        }
                    }
                    tableLatencies.Controls.Add(l);
                    tableLatencies.SetCellPosition(l, new TableLayoutPanelCellPosition((2 * j) + 2, i + 1));
                    tableLatencies.SetColumnSpan(l, 2);
                }
            }

            tableLatencies.ResumeLayout();
            tableLatencies.PerformLayout();
        }

        private static string getNtol(Pool pool)
        {
            long ntol = pool.ha_host_failures_to_tolerate;
            if (ntol > 0)
            {
                return ntol.ToString();
            }
            else
            {
                return Messages.HA_NTOL_ZERO_HAPAGE;
            }
        }

        private static string getPlanExistsFor(Pool pool)
        {
            long plan = pool.ha_plan_exists_for;
            long ntol = pool.ha_host_failures_to_tolerate;
            return plan == ntol ? plan.ToString() : String.Format(Messages.HA_OVERCOMMITTED_NTOL, plan);
        }

        private static bool haStatusRed(Pool pool)
        {
            return pool.ha_host_failures_to_tolerate > pool.ha_plan_exists_for;
        }

        private void generateSRHABox(SR sr)
        {
            CustomListRow header = CreateHeader(Messages.HA_CONFIGURATION_TITLE);
            customListPanel.AddRow(header);

            // We could do something here, e.g.
            // HB SR for pool: xyz
            //     latency to h1: 123
            //     latency to h2: 456
            // HB SR for pool: abc
            //     latency to j1: 123
            //     latency to j2: 456

            AddBottomSpacing(header);
        }

        private void GP_ContextShow(object sender, ListPanelItemClickedEventArgs e)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            foreach (ToolStripMenuItem item in e.Item.Row.MenuItems)
            {
                menu.Items.Add(item);
            }
            ToolStripMenuItem copyItem = new ToolStripMenuItem(Messages.COPY) { Image = Properties.Resources.copy_16 };
            copyItem.Click += delegate
                {
                    String text = Helpers.ToWindowsLineEndings(e.Item.Tag != null ? e.Item.Tag.ToString() : e.Item.Text);
                    Clip.SetClipboardText(text);
                };
            menu.Items.Add(copyItem);
            menu.Show(this, PointToClient(MousePosition));
        }

        private static CustomListRow CreateHeader(string text)
        {
            return new CustomListRow(text, BaseTabPage.HeaderBackColor, BaseTabPage.HeaderForeColor, BaseTabPage.HeaderBorderColor, Program.DefaultFontHeader);
        }

        private static CustomListRow AddRow<T>(CustomListRow parent, string key, T obj, ToStringDelegate<T> del, bool valueBold) where T : IEquatable<T>
        {
            CustomListRow newChild = CreateNewRow(key + ": ", new ToStringWrapper<T>(obj, del), valueBold);
            parent.AddChild(newChild);
            return newChild;
        }

        private static CustomListRow CreateNewRow<T>(string key, ToStringWrapper<T> value, bool valueBold) where T : IEquatable<T>
        {
            CustomListItem Label = new CustomListItem(key, BaseTabPage.ItemLabelFont, BaseTabPage.ItemLabelForeColor);
            Label.Anchor = AnchorStyles.Top;
            Label.itemBorder.Bottom = BaseTabPage.ITEM_SPACING;
            CustomListItem Value = new CustomListItem(value, valueBold ? BaseTabPage.ItemValueFontBold : BaseTabPage.ItemValueFont, BaseTabPage.ItemValueForeColor);
            Value.Anchor = AnchorStyles.Top;
            Value.itemBorder.Bottom = BaseTabPage.ITEM_SPACING;
            return new CustomListRow(BaseTabPage.ItemBackColor, Label, Value);
        }

        private static void AddBottomSpacing(CustomListRow header)
        {
            if (header.Children.Count != 0)
            {
                header.AddChild(new CustomListRow(header.BackColor, 5));
            }
        }

        private static string GetFriendlyName(string propertyName)
        {
            return FriendlyNameManager.GetFriendlyName(string.Format("Label-{0}", propertyName));
        }

        private void buttonConfigure_Click(object sender, EventArgs e)
        {
            if (pool == null)
                return;
            EditHA(pool);
        }

        private void buttonDisableHa_Click(object sender, EventArgs e)
        {
            if (pool == null)
                return;

            if (!pool.ha_enabled)
            {
                // Start HA wizard
                EditHA(pool);
                return;
            }

            if (pool.ha_statefiles.All(sf => pool.Connection.Resolve(new XenRef<VDI>(sf)) == null)) //empty gives true, which is correct
            {
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(
                        SystemIcons.Error,
                        string.Format(Messages.HA_DISABLE_NO_STATEFILE, Helpers.GetName(pool).Ellipsise(30)),
                        Messages.DISABLE_HA),
                    "HADisable",
                    ThreeButtonDialog.ButtonOK))
                {
                    dlg.ShowDialog(this);
                    return;
                }
            }

            // Confirm the user wants to disable HA
            using (var dlg = new ThreeButtonDialog(
                new ThreeButtonDialog.Details(
                    null,
                    string.Format(Messages.HA_DISABLE_QUERY, Helpers.GetName(pool).Ellipsise(30)),
                    Messages.DISABLE_HA),
                "HADisable",
                ThreeButtonDialog.ButtonYes,
                ThreeButtonDialog.ButtonNo))
            {
                if (dlg.ShowDialog(this) != DialogResult.Yes)
                    return;
            }

            DisableHAAction action = new DisableHAAction(pool);
            // We will need to re-enable buttons when the action completes
            action.Completed += Program.MainWindow.action_Completed;
            action.RunAsync();
        }

        /// <param name="pool">Must not be null.</param>
        internal static void EditHA(Pool pool)
        {
            var cmd = new HACommand(Program.MainWindow, pool);
            if (cmd.CanExecute())
                cmd.Execute();
        }

        private void SetNetworkHBInitDelay()
        {
            initializationDelayElapsed = false;

            //30 second delay to allow network HB status to initialize  
            initializationDelayTimer = new System.Timers.Timer(HB_INITIALIZATION_DELAY);
            initializationDelayTimer.Elapsed += HeartbeatInitialization_TimeElapsed;
            initializationDelayTimer.AutoReset = false;
            initializationDelayTimer.Enabled = true;
        }

        private void HeartbeatInitialization_TimeElapsed(Object source, System.Timers.ElapsedEventArgs e)
        {
            initializationDelayElapsed = true;
            Program.Invoke(Program.MainWindow, Rebuild);
        }
    }
}
