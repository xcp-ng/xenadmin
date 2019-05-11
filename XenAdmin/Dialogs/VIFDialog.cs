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
using System.Windows.Forms;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;
using System.Linq;


namespace XenAdmin.Dialogs
{
    public partial class VIFDialog : XenDialogBase
    {
        private readonly VIF ExistingVif;
        private readonly int Device;
        private readonly bool allowSriov;

        public VIFDialog(IXenConnection connection, VIF existingVif, int device, bool allowSriov = false)
            : base(connection)
        {
            InitializeComponent();
            CueBannersManager.SetWatermark(promptTextBoxMac, "aa:bb:cc:dd:ee:ff");

            ExistingVif = existingVif;
            Device = device;
            this.allowSriov = allowSriov;

            if (ExistingVif != null)
            {
                Text = Messages.VIRTUAL_INTERFACE_PROPERTIES;
                buttonOk.Text = Messages.OK;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadNetworks();
            LoadDetails();
            ValidateInput();
        }

        private void LoadDetails()
        {
            // Check if vSwitch Controller is configured for the pool (CA-46299)
            Pool pool = Helpers.GetPoolOfOne(connection);
            var vSwitchController = pool != null && pool.vSwitchController();

            if (vSwitchController) 
            {
                tableLayoutPanel3.Enabled = checkboxQoS.Enabled = checkboxQoS.Checked = false;
                tableLayoutPanelInfo.Visible = true;
            }
            else
            {
                if (ExistingVif == null)
                {
                    promptTextBoxQoS.Text = "";
                    checkboxQoS.Checked = false;
                }
                else
                {
                    promptTextBoxQoS.Text = ExistingVif.LimitString();
                    checkboxQoS.Checked = ExistingVif.qos_algorithm_type == VIF.RATE_LIMIT_QOS_VALUE;
                }

                tableLayoutPanel3.Enabled = checkboxQoS.Enabled = true;
                tableLayoutPanelInfo.Visible = false;
            }

            if (ExistingVif == null)
            {
                radioButtonAutogenerate.Checked = true;
                return;
            }

            foreach (NetworkComboBoxItem item in comboBoxNetwork.Items)
            {
                if (item.Network != null && item.Network.opaque_ref == ExistingVif.network.opaque_ref)
                    comboBoxNetwork.SelectedItem = item;
            }

            promptTextBoxMac.Text = ExistingVif.MAC;

            if (!string.IsNullOrEmpty(ExistingVif.MAC))
                radioButtonMac.Checked = true;
            else
                radioButtonAutogenerate.Checked = true;
        }

        private void ValidateInput()
        {
            string error;

            if (!IsValidNetwork(out error) || !IsValidMAc(out error) || !IsValidQoSLimit(out error))
            {
                buttonOk.Enabled = false;

                if (string.IsNullOrEmpty(error))
                {
                    tableLayoutPanelError.Visible = false;
                }
                else
                {
                    tableLayoutPanelError.Visible = true;
                    pictureBoxError.Image = Images.StaticImages._000_error_h32bit_16;
                    labelError.Text = error;
                }
            }
            else
            {
                buttonOk.Enabled = true;
                tableLayoutPanelError.Visible = false;
            }
        }

        private bool IsValidNetwork(out string error)
        {
            error = Messages.SELECT_NETWORK_TOOLTIP;
            return comboBoxNetwork.SelectedItem is NetworkComboBoxItem item && item.Network != null;
        }

        private bool IsValidMAc(out string error)
        {
            error = null;

            if (!radioButtonMac.Checked)
                return true;

            if (string.IsNullOrWhiteSpace(promptTextBoxMac.Text))
                return false;

            error = Messages.MAC_INVALID;
            return Helpers.IsValidMAC(promptTextBoxMac.Text);
        }

        private bool IsValidQoSLimit(out string error)
        {
            error = null;

            if (!checkboxQoS.Checked)
                return true;

            if (string.IsNullOrWhiteSpace(promptTextBoxQoS.Text))
                return false;

            error = Messages.ENTER_VALID_QOS;

            if (int.TryParse(promptTextBoxQoS.Text, out var result))
                return result > 0;

            return false;
        }

        private void LoadNetworks()
        {
            List<XenAPI.Network> networks = new List<XenAPI.Network>(connection.Cache.Networks);
            networks.Sort();
            foreach (XenAPI.Network network in networks)
            {
                if (!network.Show(Properties.Settings.Default.ShowHiddenVMs) || network.IsSlave() || (network.IsSriov() && !allowSriov))
                    continue;

                comboBoxNetwork.Items.Add(new NetworkComboBoxItem(network));
            }

            if (comboBoxNetwork.Items.Count == 0)
            {
                comboBoxNetwork.Items.Add(new NetworkComboBoxItem(null));
            }
            comboBoxNetwork.SelectedIndex = 0;
        }

        private XenAPI.Network SelectedNetwork
        {
            get
            {
                return ((NetworkComboBoxItem)comboBoxNetwork.SelectedItem).Network;
            }
        }

        private string SelectedMac
        {
            get
            {
                return radioButtonAutogenerate.Checked ? "" : promptTextBoxMac.Text;
            }
        }

        public VIF NewVif()
        {
            var vif = new VIF();
            vif.Connection = connection;
            vif.network = new XenRef<XenAPI.Network>(SelectedNetwork.opaque_ref);
            vif.MAC = SelectedMac;
            vif.device = Device.ToString();

            if (checkboxQoS.Checked)
                vif.qos_algorithm_type = VIF.RATE_LIMIT_QOS_VALUE;

            // preserve this param even if we have decided not to turn on qos
            if (!string.IsNullOrWhiteSpace(promptTextBoxQoS.Text))
                vif.qos_algorithm_params = new Dictionary<string, string> {{VIF.KBPS_QOS_PARAMS_KEY, promptTextBoxQoS.Text}};

            return vif;
        }

        /// <summary>
        /// Retrieves the new settings as a vif object. You will need to set the VM field to use these settings in a vif action
        /// </summary>
        /// <returns></returns>
        public VIF GetNewSettings()
        {
            var newVif = new VIF();
            if (ExistingVif != null)
                newVif.UpdateFrom(ExistingVif);

            newVif.network = new XenRef<XenAPI.Network>(SelectedNetwork.opaque_ref);
            newVif.MAC = SelectedMac;
            newVif.device = Device.ToString();

            if (checkboxQoS.Checked)
            {
                newVif.qos_algorithm_type = VIF.RATE_LIMIT_QOS_VALUE;
            }
            else if (ExistingVif != null && ExistingVif.qos_algorithm_type == VIF.RATE_LIMIT_QOS_VALUE)
            {
                newVif.qos_algorithm_type = "";
            }
            // else ... we leave it alone. Currently we only deal with "ratelimit" and "", don't overwrite the field if it's something else

            // preserve this param even if we turn off qos
            if (!string.IsNullOrEmpty(promptTextBoxQoS.Text))
                newVif.qos_algorithm_params = new Dictionary<string, string> { { VIF.KBPS_QOS_PARAMS_KEY, promptTextBoxQoS.Text } };

            return newVif;
        }

        private bool MACAddressHasChanged()
        {
            if (ExistingVif == null)
                return true;
            return promptTextBoxMac.Text != ExistingVif.MAC;
        }

        private bool ChangesHaveBeenMade
        {
            get
            {
                if (ExistingVif == null)
                    return true;

                if (ExistingVif.network.opaque_ref != SelectedNetwork.opaque_ref)
                    return true;

                if (ExistingVif.MAC != SelectedMac)
                    return true;

                if (ExistingVif.device != Device.ToString())
                    return true;

                if (ExistingVif.qos_algorithm_type == VIF.RATE_LIMIT_QOS_VALUE)
                {
                    if (!checkboxQoS.Checked)
                        return true;
                    if (ExistingVif.qos_algorithm_params[VIF.KBPS_QOS_PARAMS_KEY] != promptTextBoxQoS.Text)
                        return true;
                }
                else if (string.IsNullOrEmpty(ExistingVif.qos_algorithm_type))
                {
                    if (checkboxQoS.Checked)
                        return true;
                }

                return false;
            }
        }

        #region Control event handlers

        private void VIFDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.Cancel)
                return;

            if (MACAddressHasChanged())
                foreach (var xenConnection in ConnectionsManager.XenConnectionsCopy.Where(c => c.IsConnected))
                {
                    foreach (VIF vif in xenConnection.Cache.VIFs)
                    {
                        var vm = xenConnection.Resolve(vif.VM);
                        if (vif != ExistingVif && vif.MAC == SelectedMac && vm != null && vm.is_a_real_vm())
                        {
                            using (var dlg = new ThreeButtonDialog(
                                new ThreeButtonDialog.Details(SystemIcons.Warning,
                                    string.Format(Messages.PROBLEM_MAC_ADDRESS_IS_DUPLICATE, SelectedMac, vm.NameWithLocation()),
                                    Messages.PROBLEM_MAC_ADDRESS_IS_DUPLICATE_TITLE),
                                new ThreeButtonDialog.TBDButton(Messages.YES_BUTTON_CAPTION, DialogResult.Yes),
                                new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, ThreeButtonDialog.ButtonType.CANCEL, true)))
                            {
                                e.Cancel = dlg.ShowDialog(this) == DialogResult.No;
                                return;
                            }
                        }
                    }
                }

            if (!ChangesHaveBeenMade)
                DialogResult = DialogResult.Cancel;
        }

        private void comboBoxNetwork_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateInput();
        }

        private void radioButtonAutogenerate_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAutogenerate.Checked)
                ValidateInput();
        }

        private void radioButtonMac_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonMac.Checked)
                ValidateInput();
        }

        private void promptTextBoxMac_Enter(object sender, EventArgs e)
        {
            radioButtonMac.Checked = true;
            ValidateInput();
        }

        private void promptTextBoxMac_TextChanged(object sender, EventArgs e)
        {
            ValidateInput();
        }

        private void checkboxQoS_CheckedChanged(object sender, EventArgs e)
        {
            ValidateInput();
        }

        private void promptTextBoxQoS_TextChanged(object sender, EventArgs e)
        {
            ValidateInput();
        }

        private void promptTextBoxQoS_Enter(object sender, EventArgs e)
        {
            checkboxQoS.Checked = true;
            ValidateInput();
        }       

        #endregion

        internal override string HelpName
        {
            get
            {
                if (ExistingVif != null)
                    return "EditVmNetworkSettingsDialog";
                else
                    return "VIFDialog";
            }
        }
    }

    public class NetworkComboBoxItem : IEquatable<NetworkComboBoxItem>
    {
        public XenAPI.Network Network;

        public NetworkComboBoxItem(XenAPI.Network network)
        {
            Network = network;
        }

        public override string ToString()
        {
            return Network == null ? Messages.NONE : Helpers.GetName(Network);
        }

        public bool Equals(NetworkComboBoxItem other)
        {
            if (Network == null)
                return other.Network == null;
            return Network.Equals(other.Network);
        }
    }
}