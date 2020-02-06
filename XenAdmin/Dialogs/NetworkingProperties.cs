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
using XenAdmin.Controls;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Actions;
using XenAdmin.Help;


namespace XenAdmin.Dialogs
{
    public partial class NetworkingProperties : VerticallyTabbedDialog
    {
        private Host Host;
        private Pool Pool;
        
        private string ObjectName;

        private List<PIF> ShownPIFs;
        private List<PIF> AllPIFs;
        private bool AllowManagementOnVLAN;

        public NetworkingProperties()
        {
            InitializeComponent();
        }

        public NetworkingProperties(Host host, PIF selectedPIF)
            :base(host.Connection)
        {
            InitializeComponent();
            Host = host;
            Pool = null;
            ObjectName = Helpers.GetName(host);
            AllowManagementOnVLAN = !Helpers.FeatureForbidden(connection, Host.RestrictManagementOnVLAN);

            BlurbLabel.Text = string.Format(Messages.NETWORKING_PROPERTIES_BLURB_HOST, ObjectName);

            Configure(selectedPIF);
        }



        public NetworkingProperties(Pool pool, PIF selectedPIF)
            : base(pool.Connection)
        {
            InitializeComponent();
            Pool = pool;
            ObjectName = Helpers.GetName(Pool);
            AllowManagementOnVLAN = !Helpers.FeatureForbidden(connection, Host.RestrictManagementOnVLAN);

            Host =connection.Resolve(pool.master);
            if (Host == null)
                throw new Failure(Failure.INTERNAL_ERROR, "Could not resolve master");

            BlurbLabel.Text = string.Format(Messages.NETWORKING_PROPERTIES_BLURB_POOL, ObjectName);

            Configure(selectedPIF);
        }


        private void Configure(PIF selectedPIF)
        {
            Text = string.Format(Messages.NETWORKING_PROPERTIES_TITLE, ObjectName);

            ShownPIFs = GetKnownPIFs(false);
            AllPIFs = GetKnownPIFs(true);

            PIF management_pif =
                AllPIFs.Find(
                    delegate(PIF pif) { return pif.management; });

            if (management_pif == null)
            {
                // Cache has not been populated yet, or is being cleared.
                return;
            }

            bool ha = (Pool != null && Pool.ha_enabled);  // CA-24714
            NetworkingPropertiesPage management_page = new NetworkingPropertiesPage(ha ? NetworkingPropertiesPage.Type.PRIMARY_WITH_HA : NetworkingPropertiesPage.Type.PRIMARY);
            management_page.Purpose = Messages.MANAGEMENT;
            AddTabContents(management_page);
            management_page.Tag = management_pif;

            foreach (PIF pif in ShownPIFs)
            {
                if (pif.opaque_ref != management_pif.opaque_ref && pif.IsSecondaryManagementInterface(true))
                {
                    MakeSecondaryPage(pif, Purpose(pif));
                }
            }

            Dictionary<XenAPI.Network, List<NetworkingPropertiesPage>> inusemap =
                MakeProposedInUseMap();
            foreach (NetworkingPropertiesPage page in verticalTabs.Items)
            {
                page.Pool = Pool != null;
                RefreshNetworkComboBox(inusemap, page);
            }

            foreach (NetworkingPropertiesPage page in verticalTabs.Items)
            {
                ConfigurePage(page, page.Tag as PIF);
            }
            if (selectedPIF != null)
            {
                foreach (NetworkingPropertiesPage item in verticalTabs.Items)
                {
                    if (item.Tag == selectedPIF)
                    {
                        verticalTabs.SelectedItem = item;
                         break;
                    }
                }
            }
            else
                verticalTabs.SelectedIndex = 0;
            ResizeVerticalTabs(verticalTabs.Items.Count);
            verticalTabs.AdjustItemTextBounds = GetItemTextBounds;
        }

        private void ResizeVerticalTabs(int itemCount)
        {
            int maxHeight = splitContainer.Panel1.Height - AddButton.Height;
            verticalTabs.Height = Math.Min(maxHeight, itemCount * verticalTabs.ItemHeight);
            AddButton.Top = verticalTabs.Top + verticalTabs.Height;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pif">May be null, in which case the page is configured with reasonable defaults.</param>
        private void ConfigurePage(NetworkingPropertiesPage page, PIF pif)
        {
            if (pif == null)
            {
                page.SelectFirstUnusedNetwork();

                page.SubnetTextBox.Text = "";
                page.GatewayTextBox.Text = "";
                page.IPAddressTextBox.Text = "";

                page.DHCPIPRadioButton.Checked = true;
            }
            else
            {
                page.NetworkComboBox.SelectedItem = connection.Resolve(pif.network);

                page.SubnetTextBox.Text = pif.netmask;
                page.GatewayTextBox.Text = pif.gateway;
                page.IPAddressTextBox.Text = pif.IP;

                page.DHCPIPRadioButton.Checked = pif.ip_configuration_mode == ip_configuration_mode.DHCP;
            }

            string[] addresses = pif == null ? new string[] { "" } : pif.DNS.Split(',');
            page.PreferredDNSTextBox.Text = addresses[0];
            page.AlternateDNS1TextBox.Text = addresses.Length == 1 ? "" : addresses[1];
            page.AlternateDNS2TextBox.Text = addresses.Length <= 2 ? "" : addresses[2];
        }

        private void RefreshNetworkComboBox(Dictionary<XenAPI.Network, List<NetworkingPropertiesPage>> InUseMap, NetworkingPropertiesPage page)
        {
            page.RefreshNetworkComboBox(InUseMap, ManagementNetwork(), AllowManagementOnVLAN);
        }

        private List<PIF> GetKnownPIFs(bool include_invisible)
        {
            List <PIF> result = new List<PIF>();
            foreach (XenAPI.Network network in connection.Cache.Networks)
            {
                if (network.Show(include_invisible || Properties.Settings.Default.ShowHiddenVMs))
                {
                    PIF pif = FindPIFForThisHost(network.PIFs);
                    if (pif != null)
                        result.Add(pif);
                }
            }
            return result;
        }

        private Dictionary<XenAPI.Network, List<NetworkingPropertiesPage>> MakeProposedInUseMap()
        {
            Dictionary<XenAPI.Network, List<NetworkingPropertiesPage>> inusemap =
                new Dictionary<XenAPI.Network, List<NetworkingPropertiesPage>>();

            foreach (NetworkingPropertiesPage page in verticalTabs.Items)
            {
                XenAPI.Network network = (XenAPI.Network)page.NetworkComboBox.SelectedItem;

                if (network == null)
                {
                    PIF pif = page.Tag as PIF;
                    if (pif == null)
                        continue;
                    network = pif.Connection.Resolve(pif.network);
                    if (network == null)
                        continue;
                }

                if (network.PIFs.Count == 0)
                    continue;

                if (!inusemap.ContainsKey(network))
                    inusemap[network] = new List<NetworkingPropertiesPage>();
                inusemap[network].Add(page);
            }

            foreach (XenAPI.Network network in connection.Cache.Networks)
            {
                if (network.Show(Properties.Settings.Default.ShowHiddenVMs))
                {
                    if (connection.ResolveAll(network.PIFs).Find(p => !p.IsTunnelAccessPIF()) == null)  // no PIFs, or all the PIFs are tunnel access PIFs so the network is a CHIN
                        continue;
                    PIF pif = FindPIFForThisHost(network.PIFs);
                    if (pif != null && pif.IsInUseBondSlave())
                        continue;
                    if (!inusemap.ContainsKey(network))
                        inusemap[network] = null;
                }
            }

            return inusemap;
        }

        private void AddTabContents(NetworkingPropertiesPage prop_page)
        {
            prop_page.HostCount = connection.Cache.HostCount;
            prop_page.ValidChanged += new EventHandler(prop_page_ValidChanged);
            prop_page.DeleteButtonClicked += new EventHandler(prop_page_DeleteButtonClicked);
            prop_page.NetworkComboBoxChanged += new EventHandler(prop_page_NetworkComboBoxChanged);
            prop_page.Parent = ContentPanel;
            prop_page.Dock = DockStyle.Fill;

            verticalTabs.Items.Add(prop_page);

            RefreshButtons();
        }

        void RefreshNetworkComboBoxes()
        {
            Dictionary<XenAPI.Network, List<NetworkingPropertiesPage>> inusemap =
                MakeProposedInUseMap();

            foreach (NetworkingPropertiesPage page in verticalTabs.Items)
            {
                RefreshNetworkComboBox(inusemap, page);
            }
        }

        private NetworkingPropertiesPage FindPage(string purpose)
        {
            foreach (NetworkingPropertiesPage page in verticalTabs.Items)
                if (page.Purpose == purpose)
                    return page;

            System.Diagnostics.Trace.Assert(false);
            return null;
        }

        void RefreshButtons()
        {
            foreach (NetworkingPropertiesPage page in verticalTabs.Items)
            {
                    page.RefreshButtons();
            }

            okButton.Enabled = AllPagesValid();
            AddButton.Enabled = ShownPIFs.Count > verticalTabs.Items.Count;
        }

        private bool AllPagesValid()
        {
            foreach (NetworkingPropertiesPage page in verticalTabs.Items)
                if (!page.Valid||!page.NameValid)
                    return false;
            
            return true;
       }

        private void AddButton_Click(object sender, EventArgs e)
        {
            ResizeVerticalTabs(verticalTabs.Items.Count + 1);
            verticalTabs.SelectedItem = MakeSecondaryPage(null, AuxTabName());

            NetworkingPropertiesPage page = verticalTabs.SelectedItem as NetworkingPropertiesPage;
            RefreshNetworkComboBox(MakeProposedInUseMap(), page);
            ConfigurePage(page, null);
        }

        void prop_page_NetworkComboBoxChanged(object sender, EventArgs e)
        {
            RefreshNetworkComboBoxes();
        }

        void prop_page_DeleteButtonClicked(object sender, EventArgs e)
        {
            NetworkingPropertiesPage prop_page = (NetworkingPropertiesPage)sender;
            int selectedIndex = verticalTabs.SelectedIndex;
            verticalTabs.Items.Remove(prop_page);
            verticalTabs.SelectedIndex = selectedIndex < verticalTabs.Items.Count - 1 ? selectedIndex : verticalTabs.Items.Count - 1;
            ContentPanel.Controls.Remove(prop_page);
            RefreshNetworkComboBoxes();
            RefreshButtons();
            ResizeVerticalTabs(verticalTabs.Items.Count);
        }

        void prop_page_ValidChanged(object sender, EventArgs e)
        {
            RefreshButtons();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pif">May be null, in which case the page is configured with reasonable defaults.</param>
        /// <param name="purpose"></param>
        /// <returns></returns>
        private NetworkingPropertiesPage MakeSecondaryPage(PIF pif, string purpose)
        {
            NetworkingPropertiesPage prop_page = MakeSecondaryPage(purpose);
            if (pif != null)
                prop_page.Tag = pif;

            if (pif == null)
                prop_page.SelectName();

            return prop_page;
        }

        private NetworkingPropertiesPage MakeSecondaryPage(string purpose)
        {
            NetworkingPropertiesPage prop_page = new NetworkingPropertiesPage(NetworkingPropertiesPage.Type.SECONDARY);
            prop_page.Pool = Pool != null;
            prop_page.Purpose = purpose;
            prop_page.PurposeTextBox.Text = purpose;
            prop_page.RefreshButtons();

            prop_page.PurposeTextBox.TextChanged +=
                (EventHandler)delegate(object sender, EventArgs e)
                {
                    prop_page.Text = prop_page.PurposeTextBox.Text;
                };

            AddTabContents(prop_page);

            return prop_page;
        }

        private string Purpose(PIF pif)
        {
            string purpose = pif.GetManagementPurpose();
            return string.IsNullOrEmpty(purpose) ? Messages.NETWORKING_PROPERTIES_PURPOSE_UNKNOWN : purpose;
        }

        private string AuxTabName()
        {
            List<string> tabnames = GetTabNames();
            return Helpers.MakeUniqueNameFromPattern(Messages.NETWORKING_PROPERTIES_AUX_TAB_NAME, tabnames, 1);
        }

        private List<string> GetTabNames()
        {
            List<string> result = new List<string>();
            foreach (NetworkingPropertiesPage page in verticalTabs.Items)
            {
                result.Add(page.Text);
            }
            return result;
        }

        private bool IPAddressSettingsChanged(PIF pif1, PIF pif2)
        {
            return !pif1.IP.Equals(pif2.IP) ||
                   !pif1.netmask.Equals(pif2.netmask) ||
                   !pif1.gateway.Equals(pif2.gateway);
        }

        private bool GetManagementInterfaceIPChanged(PIF oldManagement, PIF newManagement)
        {
            if (oldManagement == null || newManagement == null)
                throw new Failure(Failure.INTERNAL_ERROR, "Management interface is null.");

            if (oldManagement.ip_configuration_mode == ip_configuration_mode.DHCP)
            {
                if (newManagement.ip_configuration_mode == ip_configuration_mode.DHCP)
                    return !oldManagement.Equals(newManagement);

                if (newManagement.ip_configuration_mode == ip_configuration_mode.Static)
                    return IPAddressSettingsChanged(oldManagement, newManagement);
            }
            else if (oldManagement.ip_configuration_mode == ip_configuration_mode.Static)
            {
                if (newManagement.ip_configuration_mode == ip_configuration_mode.Static)
                    return IPAddressSettingsChanged(oldManagement, newManagement);

                if (newManagement.ip_configuration_mode == ip_configuration_mode.DHCP)
                    return true;
            }

            throw new Failure(Failure.INTERNAL_ERROR, "Unexpected IP configuration mode.");
        }

        private void AcceptBtn_Click(object sender, EventArgs e)
        {
            List<PIF> newPIFs = new List<PIF>();
            List<PIF> newNamePIFs = new List<PIF>();
            List<PIF> downPIFs = new List<PIF>();

            foreach (PIF pif in AllPIFs)
            {
                if (pif.IsManagementInterface(XenAdmin.Properties.Settings.Default.ShowHiddenVMs))
                    downPIFs.Add(pif);
            }

            foreach (NetworkingPropertiesPage page in verticalTabs.Items)
            {
                try
                {
                    CollateChanges(page, page.Tag as PIF, newPIFs, newNamePIFs);
                }
                catch (Failure)
                {
                    using (var dlg = new ThreeButtonDialog(
                       new ThreeButtonDialog.Details(
                           SystemIcons.Warning,
                           Messages.NETWORK_RECONFIG_CONNECTION_LOST,
                           Messages.XENCENTER)))
                    {
                        dlg.ShowDialog(this);
                    }
                    this.Close();
                    return;
                }
            }

            bool displayWarning = false;
            bool managementInterfaceIPChanged = false;
            PIF down_management = downPIFs.Find(PIFIsManagement);
            PIF new_management = newPIFs.Find(PIFIsManagement);
            if (down_management != null)
            {
                if (new_management == null)
                    throw new Failure(Failure.INTERNAL_ERROR, "Bringing down the management interface without bringing another one up is impossible!");

                managementInterfaceIPChanged = GetManagementInterfaceIPChanged(down_management, new_management);
                displayWarning = managementInterfaceIPChanged ||
                                 (down_management.uuid != new_management.uuid ||
                                  down_management.ip_configuration_mode != new_management.ip_configuration_mode);

                if (down_management.Equals(new_management))
                {
                    // Management interface has not changed
                    down_management = null;
                }
            }

            // Any PIFs that are in downPIFs but also in newPIFs need to be removed from the former.
            // downPIFs should contain all those that we no longer want to keep up.
            downPIFs.RemoveAll(p => PIFContains(newPIFs, p));

            // Remove any PIFs that haven't changed -- there's nothing to do for these ones.  They are in this
            // list originally so that they can be used as a filter against downPIFs.
            newPIFs.RemoveAll(p => !p.Changed);

            newNamePIFs.RemoveAll(p => !p.Changed);

            if (newNamePIFs.Count > 0)
            {
                new SetSecondaryManagementPurposeAction(connection, Pool, newNamePIFs).RunAsync();
            }

            if (newPIFs.Count > 0 || downPIFs.Count > 0)
            {
                if (displayWarning)
                {
                    string title = Pool == null ? Messages.NETWORKING_PROPERTIES_WARNING_CHANGING_MANAGEMENT_HOST 
                        : Messages.NETWORKING_PROPERTIES_WARNING_CHANGING_MANAGEMENT_POOL;

                    DialogResult dialogResult;
                    using (var dlg = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(SystemIcons.Warning, title),
                            "NetworkingPropertiesPMIWarning",
                            new ThreeButtonDialog.TBDButton(Messages.NETWORKING_PROPERTIES_CHANGING_MANAGEMENT_CONTINUE, DialogResult.OK),
                            ThreeButtonDialog.ButtonCancel))
                    {
                        dialogResult = dlg.ShowDialog(this);
                    }
                    if (DialogResult.OK != dialogResult)
                    {
                        DialogResult = System.Windows.Forms.DialogResult.None;
                        return;
                    }
                }

                if (down_management == null)
                {
                    // We're actually just changing the IP address, not moving the management interface, so we just pass it in through newPIFs.
                    new_management = null;
                }
                else
                {
                    // We're switching the management interface over, so remove the old one from downPIFs -- it will be special-cased through
                    // down_management and new_management.
                    downPIFs.Remove(down_management);
                }

                // Reverse the lists so that the management interface is always last to be done.  If something's going to go wrong, then we'd like
                // the management interface to be the one that we don't break.
                newPIFs.Reverse();
                downPIFs.Reverse();

                new ChangeNetworkingAction(connection, Pool, Host, newPIFs, downPIFs, new_management, down_management,
                                           managementInterfaceIPChanged).RunAsync();
            }

            Close();
        }

        private bool PIFContains(List<PIF> l, PIF p)
        {
            foreach (PIF p2 in l)
            {
                if (p2.uuid == p.uuid)
                    return true;
            }
            return false;
        }

        private bool PIFIsManagement(PIF p)
        {
            return p.management;
        }

        /// <summary>
        /// Will throw an exception if the network has gone away.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="oldPIF"></param>
        /// <param name="newPIFs"></param>
        /// <param name="newNamePIFs"></param>
        private void CollateChanges(NetworkingPropertiesPage page, PIF oldPIF, List<PIF> newPIFs, List<PIF> newNamePIFs)
        {
            bool changed = false;

            if (oldPIF == null)
            {
                // This tab is newly created.

                XenAPI.Network network = (XenAPI.Network)page.NetworkComboBox.SelectedItem;
                oldPIF = FindPIFForThisHost(network.PIFs);
                if (oldPIF == null)
                    throw new Failure(Failure.INTERNAL_ERROR, "Network has gone away");
                changed = true;
            }
            else
            {
                // This tab was populated when this dialog was launched.

                XenAPI.Network network = connection.Resolve(oldPIF.network);

                if ((XenAPI.Network)page.NetworkComboBox.SelectedItem != network)
                {
                    // The user has changed the network, so find the one we're using now.
                    XenAPI.Network new_network = (XenAPI.Network)page.NetworkComboBox.SelectedItem;
                    oldPIF = FindPIFForThisHost(new_network.PIFs);
                    if (oldPIF == null)
                        throw new Failure(Failure.INTERNAL_ERROR, "Network has gone away");
                    changed = true;
                }
            }

            PIF newPIF = (PIF)oldPIF.Clone();
            newPIF.Changed = changed;
            PIF newNamePIF = (PIF)oldPIF.Clone();
            newNamePIF.Changed = false;

            if (page.DHCPIPRadioButton.Checked)
            {
                newPIF.ip_configuration_mode = ip_configuration_mode.DHCP;
            }
            else
            {
                newPIF.ip_configuration_mode = ip_configuration_mode.Static;
                newPIF.netmask = page.SubnetTextBox.Text;
                newPIF.gateway = page.GatewayTextBox.Text;
                newPIF.IP = page.IPAddressTextBox.Text;
                List<string> dns = new List<string>();
                if (page.PreferredDNSTextBox.Text.Length > 0)
                    dns.Add(page.PreferredDNSTextBox.Text);
                if (page.AlternateDNS1TextBox.Text.Length > 0)
                    dns.Add(page.AlternateDNS1TextBox.Text);
                if (page.AlternateDNS2TextBox.Text.Length > 0)
                    dns.Add(page.AlternateDNS2TextBox.Text);

                newPIF.DNS = string.Join(",", dns.ToArray());
            }

            newPIF.management = page.type != NetworkingPropertiesPage.Type.SECONDARY;

            if (page.type == NetworkingPropertiesPage.Type.SECONDARY)
            {
                if (newPIF.Changed)
                    newPIF.SetManagementPurspose(page.PurposeTextBox.Text);
                else
                    newNamePIF.SetManagementPurspose(page.PurposeTextBox.Text);
            }

            newPIFs.Add(newPIF);
            newNamePIFs.Add(newNamePIF);
        }

        private PIF FindPIFForThisHost(List<XenRef<PIF>> pifs)
        {
            foreach (PIF pif in connection.ResolveAll(pifs))
            {
                if (pif.host.opaque_ref == Host.opaque_ref)
                    return pif;
            }
            return null;
        }

        internal void DedicateNewNIC()
        {
            if (AddButton.Enabled)
                AddButton_Click(null, null);
        }

        private XenAPI.Network ManagementNetwork()
        {
            return (XenAPI.Network)((NetworkingPropertiesPage)verticalTabs.Items[0]).NetworkComboBox.SelectedItem;
        }

        private void NetworkingProperties_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            HelpManager.Launch("NetworkingProperties");
        }

        private void NetworkingProperties_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            HelpManager.Launch("NetworkingProperties");
        }

        public void SelectPIF(PIF pif)
        {
            
        }

        private void splitContainer_Panel1_Resize(object sender, EventArgs e)
        {
            ResizeVerticalTabs(verticalTabs.Items.Count);
        }

        protected Rectangle GetItemTextBounds(Rectangle itemBounds)
        {
            return new Rectangle(itemBounds.X, itemBounds.Y, itemBounds.Width - 20, itemBounds.Height);
        }

        private void verticalTabs_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= verticalTabs.Items.Count)
                return;

            NetworkingPropertiesPage page = verticalTabs.Items[e.Index] as NetworkingPropertiesPage;
            if (page == null|| page.type != NetworkingPropertiesPage.Type.SECONDARY || page.ClusteringEnabled)
                return;

            Graphics g = e.Graphics;
            Rectangle b = e.Bounds;

            // draw Delete icon
            Image deleteIcon = Properties.Resources._000_Abort_h32bit_16;
            if (deleteIcon != null)
            {
                page.DeleteIconBounds = new Rectangle(b.Right - deleteIcon.Width - ((32 - deleteIcon.Width) / 2),
                    b.Y + ((32 - deleteIcon.Height) / 2), deleteIcon.Width, deleteIcon.Height);
                g.DrawImage(deleteIcon, page.DeleteIconBounds);

            }
        }

        private bool MouseIsOnDeleteIcon(Point mouseLocation)
        {
            int pageIndex = verticalTabs.IndexFromPoint(mouseLocation);
            if (pageIndex < 0)
                return false;

            NetworkingPropertiesPage page = verticalTabs.Items[pageIndex] as NetworkingPropertiesPage;
            if (page == null)
                return false;

            var bounds = page.DeleteIconBounds;
            return bounds.Contains(mouseLocation);
        }

        private void verticalTabs_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseIsOnDeleteIcon(e.Location))
                ShowTooltip(e.Location);
            else
                HideTooltip();
        }

        private void verticalTabs_MouseClick(object sender, MouseEventArgs e)
        {
            int pageIndex = verticalTabs.IndexFromPoint(e.Location);
            if (pageIndex < 0 || !MouseIsOnDeleteIcon(e.Location))
                return;

            NetworkingPropertiesPage page = verticalTabs.Items[pageIndex] as NetworkingPropertiesPage;
            if (page != null)
            {
                prop_page_DeleteButtonClicked(page, new EventArgs());
                HideTooltip();
            }
        }

        private void linkLabelTellMeMore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpManager.Launch("NetworkingProperties");
        }

        private readonly ToolTip toolTipRemove = new ToolTip();
        private bool tooltipVisible;

        private void ShowTooltip(Point location)
        {
            if (!tooltipVisible)
            {
                toolTipRemove.Show(Messages.NETWORKING_PROPERTIES_REMOVE_TOOLTIP, verticalTabs, location.X, location.Y + 20);
                tooltipVisible = true;
                Cursor = Cursors.Hand;
            }
        }

        private void HideTooltip()
        {
            toolTipRemove.Hide(verticalTabs);
            tooltipVisible = false;
            Cursor = Cursors.Default;
        }

        protected override string GetTabTitle(VerticalTabs.IVerticalTab verticalTab)
        {
            NetworkingPropertiesPage page = verticalTab as NetworkingPropertiesPage;
            if (page != null)
            {
                return page.type == NetworkingPropertiesPage.Type.SECONDARY ? page.Text : Messages.NETWORKING_PROPERTIES_TAB_TITLE_PRIMARY;
            }
            return base.GetTabTitle(verticalTab);
        }
    }
}
