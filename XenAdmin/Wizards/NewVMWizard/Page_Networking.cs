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
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Controls;
using XenAdmin.Dialogs;
using System.Drawing;


namespace XenAdmin.Wizards.NewVMWizard
{
    public partial class Page_Networking : XenTabPage
    {
        /// <summary>
        /// This value is then the lowest possible upper bound for the number of networks on a new vm created from a default template. We only do this for default templates, as custom templates can be tweaked (install tools) for higher values. (See CA-31800)
        /// </summary>
        private static int MAX_NETWORKS_FOR_DEFAULT_TEMPLATES = 4;
        private VM Template;
        private string VmName;

        public Page_Networking()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get { return Messages.NEWVMWIZARD_NETWORKINGPAGE_NAME; }
        }

        public override string PageTitle
        {
            get { return Messages.NEWVMWIZARD_NETWORKINGPAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return "Networking"; }
        }

        public override bool EnableNext()
        {
            return true;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            VM template = SelectedTemplate;
            string vmName = SelectedName;

            if (Template == template && VmName == vmName)
                return;

            Template = template;
            VmName = vmName;

            panelDefaultTemplateInfo.Visible = Template.DefaultTemplate();

            BoxTitle.Text = string.Format(Messages.NEWVMWIZARD_NETWORKINGPAGE_VIFSON, VmName);//CA-56794 Helpers.TrimStringIfRequired(VmName, 50));

            LoadNetworks();

            if (NetworksGridView.Rows.Count > 0)
                NetworksGridView.Rows[0].Selected = true;

            UpdateEnablement();
        }

        public override void SelectDefaultControl()
        {
            NetworksGridView.Select();
        }

        public VM SelectedTemplate { private get; set; }
        public string SelectedName { private get; set; }

        private void UpdateEnablement()
        {
            // limiting the number of Vifs allowed to 4 on creation for default templates
            if (NetworksGridView.Rows.Count > Template.MaxVIFsAllowed())
            {
                toolTipContainerAddButton.SetToolTip(Messages.TOOLTIP_MAX_NETWORKS_FROM_TEMPLATE);
                AddButton.Enabled = false;
            }
            else if (Template.DefaultTemplate() && NetworksGridView.Rows.Count >= MAX_NETWORKS_FOR_DEFAULT_TEMPLATES)
            {
                toolTipContainerAddButton.SetToolTip(string.Format(Messages.TOOLTIP_MAX_NETWORKS_FROM_DEFAULT_TEMPLATE, MAX_NETWORKS_FOR_DEFAULT_TEMPLATES));
                AddButton.Enabled = false;
            }
            else
            {
                AddButton.Enabled = true;
                toolTipContainerAddButton.RemoveAll();
            }
            PropertiesButton.Enabled = NetworksGridView.SelectedRows.Count > 0;
            DeleteButton.Enabled = NetworksGridView.SelectedRows.Count > 0;

            OnPageUpdated();
        }

        private void LoadNetworks()
        {
            NetworksGridView.Rows.Clear();

            if (Template.DefaultTemplate())
            {
                // we add all default networks
                List<XenAPI.Network> networks = new List<XenAPI.Network>(Connection.Cache.Networks);
                networks.Sort();
                foreach (XenAPI.Network network in networks)
                {
                    // CA-218956 - Expose HIMN when showing hidden objects
                    // HIMN shouldn't be autoplugged
                    if (network.IsGuestInstallerNetwork() ||
                        !network.GetAutoPlug() || !network.Show(Properties.Settings.Default.ShowHiddenVMs) || network.IsSlave() || (network.IsSriov() && !Template.HasSriovRecommendation()))
                        continue;

                    if (NetworksGridView.Rows.Count < MAX_NETWORKS_FOR_DEFAULT_TEMPLATES)
                        NetworksGridView.Rows.Add(new NetworkListViewItem(Connection, network, NetworksGridView.Rows.Count));
                    else
                        break;
                }
            }
            else
            {
                // we add all the templates networks
                List<VIF> vifs = Connection.ResolveAll(Template.VIFs);
                vifs.Sort();
                foreach (VIF vif in vifs)
                    NetworksGridView.Rows.Add(new NetworkListViewItem(Connection, vif, NetworksGridView.Rows.Count, false));
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            VIF newVif;
            using (var dialog = new VIFDialog(Connection, null, NetworksGridView.Rows.Count, Template.HasSriovRecommendation()))
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                newVif = dialog.NewVif();
            }

            NetworksGridView.Rows.Add(new NetworkListViewItem(Connection, newVif, NetworksGridView.Rows.Count, true));
            UpdateEnablement();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (NetworksGridView.SelectedRows.Count <= 0)
                return;

            NetworksGridView.Rows.Remove(NetworksGridView.SelectedRows[0]);
            UpdateEnablement();
        }

        private void PropertiesButton_Click(object sender, EventArgs e)
        {
            if (NetworksGridView.SelectedRows.Count <= 0)
                return;

            NetworkListViewItem selectedItem = ((NetworkListViewItem)NetworksGridView.SelectedRows[0]);

            VIF newVif;
            using (var dialog = new VIFDialog(Connection, selectedItem.Vif, selectedItem.Index, Template.HasSriovRecommendation()))
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                newVif = dialog.NewVif();
            }

            selectedItem.Vif = newVif;
            selectedItem.UpdateDetails();

            UpdateEnablement();
        }

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                List<KeyValuePair<string, string>> sum = new List<KeyValuePair<string, string>>();
                int i = 0;
                foreach (VIF v in SelectedVifs)
                {
                    sum.Add(new KeyValuePair<string, string>(string.Format(Messages.NEWVMWIZARD_NETWORKINGPAGE_VIF, i), v.NetworkName()));
                    i++;
                }
                return sum;
            }
        }

        public List<VIF> SelectedVifs
        {
            get
            {
                List<VIF> vifs = new List<VIF>();
                foreach (NetworkListViewItem vif in NetworksGridView.Rows)
                {
                    vifs.Add(vif.Vif);
                }
                return vifs;
            }
        }
    }

    public class NetworkListViewItem : DataGridViewRow
    {
        public VIF Vif;

        private DataGridViewImageCell ImageCell;
        private DataGridViewTextBoxCell MacCell;
        private DataGridViewTextBoxCell NetworkCell;

        public NetworkListViewItem(IXenConnection connection, VIF vif, int index, bool keepMac)
        {
            Vif = new VIF();
            Vif.Connection = connection;

            Vif.device = vif.device;
            Vif.MAC = keepMac ? vif.MAC : "";
            Vif.network = vif.network;
            Vif.qos_algorithm_type = vif.qos_algorithm_type;
            Vif.qos_algorithm_params = vif.qos_algorithm_params;

            CreateCells();
        }

        public NetworkListViewItem(IXenConnection connection, XenAPI.Network network, int index)
        {
            Vif = new VIF();
            Vif.Connection = connection;

            Vif.device = index.ToString();
            Vif.network = new XenRef<XenAPI.Network>(network.opaque_ref);

            CreateCells();
        }

        private void CreateCells()
        {
            ImageCell = new DataGridViewImageCell(false);
            ImageCell.ValueType = typeof (Image);
            MacCell = new DataGridViewTextBoxCell();
            NetworkCell = new DataGridViewTextBoxCell();

            Cells.AddRange(new DataGridViewCell[] { ImageCell, MacCell, NetworkCell });

            UpdateDetails();
        }

        public void UpdateDetails()
        {
            ImageCell.Value = Properties.Resources._000_Network_h32bit_16;
            MacCell.Value = string.IsNullOrEmpty(Vif.MAC) ? Messages.NEWVMWIZARD_NETWORKINGPAGE_AUTOGEN : Vif.MAC;
            NetworkCell.Value = Helpers.GetName(Vif.Connection.Resolve(Vif.network));
        }
    }
}
