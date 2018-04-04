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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAPI;
using XenCenterLib;


namespace XenAdmin.Dialogs
{
    public partial class NetworkingPropertiesPage : UserControl, VerticalTabs.IVerticalTab
    {
        public enum Type { PRIMARY, PRIMARY_WITH_HA, SECONDARY };

        public bool Pool;
        public Type type;
        public int HostCount;
        public string Purpose;
        protected internal Rectangle DeleteIconBounds;

        public override string Text
        {
            get
            {
                return Purpose ?? String.Empty;
            }
            set
            {
                Purpose = value;
            }
        }

        private bool valid;
        public bool Valid
        {
            get { return valid; }
            set { if (valid != value) { valid = value; ValidChanged(this, null); } }
        }
        
        public event EventHandler ValidChanged;
        public event EventHandler DeleteButtonClicked;
        public event EventHandler NetworkComboBoxChanged;

        private Dictionary<XenAPI.Network, List<NetworkingPropertiesPage>> InUseMap;
        private XenAPI.Network ManagementNetwork;
        private string InUseWarning = null;
        private bool SquelchNetworkComboBoxChange = false;
        private bool TriggeringChange = false;

        public NetworkingPropertiesPage(Type type)
        {
            this.type = type;
            InitializeComponent();
        }

        public Image Image
        {
            get { return Images.GetImage16For(type == Type.SECONDARY ? Icons.PifSecondary : Icons.PifPrimary); }
        }

        public String SubText
        {
            get 
            {
                if (NetworkComboBox.SelectedItem == null)
                    return Messages.NONE;

                return String.Format(Messages.NETWORKING_PROPERTIES_SUBTEXT, 
                    NetworkComboBox.SelectedItem, DHCPIPRadioButton.Checked ? Messages.PIF_DHCP : Messages.PIF_STATIC); 
            }
        }

        private void SetDNSControlsVisible(bool value)
        {
            PreferredDNSTextBox.Visible = value;
            AlternateDNS1TextBox.Visible = value;
            AlternateDNS2TextBox.Visible = value;
            PreferredDNSLabel.Visible = value;
            AlternateDNS1Label.Visible = value;
            AlternateDNS2Label.Visible = value;
        }

        public void RefreshButtons()
        {
            XenAPI.Network network = (XenAPI.Network)NetworkComboBox.SelectedItem;
            string purpose = network == null ? null : FindOtherPurpose(network);

            InUseWarning =
                purpose == null || purpose == Purpose ?
                    null :
                type == Type.SECONDARY && network == ManagementNetwork ?
                    string.Format(Messages.NETWORKING_PROPERTIES_IN_USE_WARNING_MANAGEMENT, network.ToString()) :
                    string.Format(Messages.NETWORKING_PROPERTIES_IN_USE_WARNING, network.ToString(), purpose);

            PurposeLabel.Visible =
                PurposeTextBox.Visible =
                DeleteButton.Visible =
                (type == Type.SECONDARY);

            panelHAEnabledWarning.Visible =
                (type == Type.PRIMARY_WITH_HA);

            SetDNSControlsVisible(type != Type.SECONDARY);

            panelInUseWarning.Visible =
                InUseWarning != null;
            InUseWarningText.Text = InUseWarning;

            IpAddressSettingsLabel.Text = type == Type.SECONDARY
                                              ? Messages.NETWORKING_PROPERTIES_IP_SETTINGS
                                              : Messages.NETWORKING_PROPERTIES_IP_AND_DNS_SETTINGS;
            IPAddressLabel.Text = Pool ? Messages.IP_ADDRESS_RANGE_LABEL : Messages.IP_ADDRESS_LABEL;

            tableLayoutPanelStaticSettings.Enabled = FixedIPRadioButton.Checked;

            RangeEndLabel.Visible = Pool;

            Valid =
                InUseWarning == null &&
                NetworkComboBox.SelectedIndex != -1 &&
                (DHCPIPRadioButton.Checked ||
                 ((StringUtility.IsIPAddress(IPAddressTextBox.Text)) &&
                  StringUtility.IsValidNetmask(SubnetTextBox.Text) && IsOptionalIPAddress(GatewayTextBox.Text))) &&
                (type == Type.SECONDARY || ((IsOptionalIPAddress(PreferredDNSTextBox.Text) 
                                            && IsOptionalIPAddress(AlternateDNS1TextBox.Text) 
                                            && IsOptionalIPAddress(AlternateDNS2TextBox.Text))));

            // Grey out everything if HA is enabled: CA-24714
            if (type == Type.PRIMARY_WITH_HA)
            {
                foreach (Control c in Controls)
                    c.Enabled = false;
                haEnabledWarningIcon.Enabled =
                haEnabledRubric.Enabled =
                    true;
            }

            var pif = Tag as PIF;
            var existingCluster = network != null ? network.Connection.Cache.Clusters.FirstOrDefault() : null;

            if (pif != null && existingCluster != null && existingCluster.network.opaque_ref == network.opaque_ref)
            {
                Host host = network.Connection.Resolve(pif.host);
                    
                ClusteringEnabled = network.Connection.Cache.Cluster_hosts.Any(cluster =>
                    cluster.host.opaque_ref == pif.host.opaque_ref && cluster.enabled);

                if (ClusteringEnabled)
                {
                    if (host != null && host.enabled)
                    {
                        DisableControls(string.Format(Messages.CANNOT_CHANGE_IP_CLUSTERING_ENABLED, network.Name()));
                    }
                    else
                    {
                        DeleteButton.Enabled = false;
                        tableLayoutInfo.Visible = true;
                        labelWarning.Text = string.Format(Messages.CANNOT_REMOVE_IP_WHEN_CLUSTERING_ON_NETWORK, network.Name());
                    }
                }
            }
        }

        internal bool ClusteringEnabled { get; private set; }

        private string FindOtherPurpose(XenAPI.Network network)
        {
            if (InUseMap[network] != null)
            {
                List<NetworkingPropertiesPage> pages = InUseMap[network];
                foreach (NetworkingPropertiesPage page in pages)
                {
                    if (page != this)
                        return page.Purpose;
                }
            }
            return null;
        }

        private bool IsInRange(string s)
        {
            string[] bits = s.Split('.');
            return int.Parse(bits[3]) + HostCount <= 256;
        }

        private bool IsOptionalIPAddress(string s)
        {
            return s == "" || StringUtility.IsIPAddress(s);
        }

        private void SomethingChanged(object sender, EventArgs e)
        {
            RefreshButtons();
        }

        private void NetworkComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SquelchNetworkComboBoxChange)
                return;

            RefreshButtons();

            TriggeringChange = true;
            try
            {
                if (NetworkComboBoxChanged != null)
                    NetworkComboBoxChanged(this, e);
            }
            finally
            {
                TriggeringChange = false;
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (DeleteButtonClicked != null)
                DeleteButtonClicked(this, e);
        }

        private void NetworkComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;

            XenAPI.Network network =
                (XenAPI.Network)(e.Index == -1 ? NetworkComboBox.SelectedItem : NetworkComboBox.Items[e.Index]);

            if (network == null)
                return;

            string purpose = FindOtherPurpose(network);
            string label =
                purpose == null || purpose == Purpose ?
                    network.ToString() :
                    string.Format(Messages.NETWORK_IN_USE_BY, network.ToString(), purpose);

            Brush text_brush = (e.State & DrawItemState.Selected) > 0 ? SystemBrushes.HighlightText : SystemBrushes.ControlText;
            
            e.DrawBackground();
            g.DrawString(label, Program.DefaultFont, text_brush, new PointF(e.Bounds.X + 1, e.Bounds.Y + 1));
            e.DrawFocusRectangle();
        }

        internal void RefreshNetworkComboBox(Dictionary<XenAPI.Network, List<NetworkingPropertiesPage>> InUseMap, XenAPI.Network ManagementNetwork, bool AllowManagementOnVLAN)
        {
            this.InUseMap = InUseMap;
            this.ManagementNetwork = ManagementNetwork;

            XenAPI.Network selected = (XenAPI.Network)NetworkComboBox.SelectedItem;

            List<XenAPI.Network> networks = new List<XenAPI.Network>(InUseMap.Keys);
            networks.Sort();
            NetworkComboBox.Items.Clear();

            if (!AllowManagementOnVLAN && (type == Type.PRIMARY || type == Type.PRIMARY_WITH_HA))
                networks.RemoveAll(network=>network.IsVLAN());
        
            networks.RemoveAll(network => network.IsSriov());

            NetworkComboBox.Items.AddRange(networks.ToArray());

            SquelchNetworkComboBoxChange = true;
            try
            {
                NetworkComboBox.SelectedItem = selected;
            }
            finally
            {
                SquelchNetworkComboBoxChange = false;
            }

            if (!TriggeringChange)
            {
                RefreshButtons();
            }
        }

       
        internal void SelectFirstUnusedNetwork()
        {
            List<XenAPI.Network> networks = new List<XenAPI.Network>(InUseMap.Keys);
            networks.Sort();
            foreach (XenAPI.Network network in networks)
            {
                if (InUseMap[network] == null)
                {
                    NetworkComboBox.SelectedItem = network;
                    return;
                }
            }
        }

        internal void SelectName()
        {
            PurposeTextBox.Select();
            PurposeTextBox.SelectAll();
        }

        public bool NameValid
        {
            get
            {
                return _nameValid;
            }
        }

        private bool _nameValid = true;

        private void PurposeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (PurposeTextBox.Text == string.Empty)
                _nameValid = false;
            else
                _nameValid = true;
            if(ValidChanged!=null)
            ValidChanged(this, null);
        }

        private void IPAddressTextBox_TextChanged(object sender, EventArgs e)
        {
            RefreshButtons();
            if (Pool)
            {
                if (StringUtility.IsIPAddress(IPAddressTextBox.Text))
                {
                    string[] bits = IPAddressTextBox.Text.Split('.');
                    RangeEndLabel.Text = string.Format(Messages.IP_ADDRESS_RANGE_END, bits[0], bits[1], bits[2],
                                                       int.Parse(bits[3]) + HostCount - 1);
                }
            }
        }

        private void DisableControls(string message)
        {
            PurposeLabel.Enabled = PurposeTextBox.Enabled = Network2Label.Enabled = NetworkComboBox.Enabled = IpAddressSettingsLabel.Enabled =
                DHCPIPRadioButton.Enabled = FixedIPRadioButton.Enabled = tableLayoutPanelStaticSettings.Enabled = DeleteButton.Enabled = false;
            tableLayoutInfo.Visible = true;
            labelWarning.Text = message;
        }
    }
}
