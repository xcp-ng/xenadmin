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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.SettingsPanels
{
    public partial class ClusteringEditPage : UserControl, IEditPage
    {
        private Pool pool;
        private bool clusteringEnabled;
        private XenRef<XenAPI.Network> commonNetwork;
        private readonly ToolTip SelectNetworkToolTip;

        public ClusteringEditPage()
        {
            InitializeComponent();
            Text = Messages.CLUSTERING;
            tableLayoutPanelNetworkWarning.Visible = false;
            SelectNetworkToolTip = new ToolTip
                {
                    IsBalloon = true,
                    ToolTipIcon = ToolTipIcon.Warning,
                    ToolTipTitle = Messages.MUST_SELECT_NETWORK
                };
        }

        public string SubText
        {
            get { return CheckBoxEnableClustering.Checked ? Messages.ENABLED : Messages.DISABLED; } 
        }

        public Image Image
        {
            get { return Properties.Resources._000_Storage_h32bit_16; }
        }

        public AsyncAction SaveSettings()
        {
            if (CheckBoxEnableClustering.Checked)
            {
                var network = ((NetworkComboBoxItem)comboBoxNetwork.SelectedItem).Network;
                if (network != null)
                {
                    return new EnableClusteringAction(pool, network);
                }
            }
            else
            {
                return new DisableClusteringAction(pool);
            }
            return null;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            pool = Helpers.GetPoolOfOne(clone.Connection);
            var existingCluster = pool.Connection.Cache.Clusters.FirstOrDefault();
            clusteringEnabled = existingCluster != null;
            labelWarning.Visible = pictureBoxInfo1.Visible = false;
            LoadNetworks(existingCluster);
            SetPage();
        }

        public bool ValidToSave
        {
            get
            {
                return clusteringEnabled || comboBoxNetwork.SelectedItem != null || !CheckBoxEnableClustering.Checked;
            }
        }

        public void ShowLocalValidationMessages()
        {
            if (!ValidToSave)
            {
                HelpersGUI.ShowBalloonMessage(comboBoxNetwork, SelectNetworkToolTip);
            }
        }

        public void Cleanup()
        {
            
        }

        public bool HasChanged
        {
            get
            {
                return clusteringEnabled != CheckBoxEnableClustering.Checked;
            }
        }

        #region PrivateMethods
        private void LoadNetworks(Cluster cluster)
        {
            comboBoxNetwork.IncludeOnlyEnabledNetworksInComboBox = false;
            comboBoxNetwork.IncludeOnlyNetworksWithIPAddresses = true;

            if (cluster == null)
            {
                comboBoxNetwork.PopulateComboBox(pool.Connection, item => false);
                if (comboBoxNetwork.Items.Count == 0)
                    DisableControls(Messages.GFS2_NO_NETWORK);
            }
            else
            {
                DelegatedAsyncAction action = new DelegatedAsyncAction(pool.Connection,
                    string.Empty, string.Empty, string.Empty,
                    delegate(Session session)
                    {
                        commonNetwork = Cluster.get_network(session, cluster.opaque_ref);
                    },
                    true);

                action.Completed += action_Completed;

                action.RunAsync();
            }
        }

        private void action_Completed(ActionBase sender)
        {
            var action = (AsyncAction) sender;
            if (!action.Succeeded)
                commonNetwork = null;

            Program.Invoke(ParentForm, delegate
            {
                comboBoxNetwork.PopulateComboBox(pool.Connection, item => commonNetwork != null && item.Network.opaque_ref.Equals(commonNetwork.opaque_ref));
                tableLayoutPanelNetworkWarning.Visible = commonNetwork == null || comboBoxNetwork.SelectedItem == null;
            });
        }

        private void SetPage()
        {
            CheckBoxEnableClustering.Checked = clusteringEnabled;

            if (clusteringEnabled)
            {
                var gfs2Attached = pool.Connection.Cache.SRs.Any(sr => sr.GetSRType(true) == SR.SRTypes.gfs2 && !sr.IsDetached());
                if (gfs2Attached)
                    DisableControls(Messages.GFS2_SR_ATTACHED);
                else
                    comboBoxNetwork.Enabled = labelNetwork.Enabled = false;
            }
            else if (pool.ha_enabled)
                DisableControls(Messages.GFS2_HA_ENABLED);

            labelHostCountWarning.Visible = pool.Connection.Cache.HostCount < 3;
        }

        private void DisableControls(string message)
        {
            comboBoxNetwork.Enabled = labelNetwork.Enabled = CheckBoxEnableClustering.Enabled = false;
            labelWarning.Visible = pictureBoxInfo1.Visible = true;
            labelWarning.Text = message;
        }

        #endregion 

        private void comboBoxNetwork_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!clusteringEnabled)
                CheckBoxEnableClustering.Checked = comboBoxNetwork.SelectedItem != null;
        }
    }
}
