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

using XenAdmin.Actions;
using XenAdmin.Wlb;
using XenAPI;
using XenAdmin.Dialogs;


namespace XenAdmin.SettingsPanels
{
    public partial class WlbAutomationPage : UserControl, IEditPage
    {
        private WlbPoolConfiguration _poolConfiguration;
        private XenAdmin.Network.IXenConnection _connection;
        private bool _hasChanged = false;
        private bool _loading = false;
        private HashSet<string> _uuidSet = new HashSet<string>();

        private int[] minimumColumnWidths = {25, 50, 50, 50 };

        public WlbAutomationPage()
        {
            InitializeComponent();

            Text = Messages.WLB_AUTOMATION;
        }

        public WlbPoolConfiguration PoolConfiguration
        {
            set
            {
                if (null != value)
                {
                    _poolConfiguration = value;
                    InitializeControls();
                }
            }
        }

        public XenAdmin.Network.IXenConnection Connection
        {
            set
            {
                if (null != value)
                {
                    _connection = value;
                }
            }
        }

        private void InitializeControls()
        {
            _loading = true;

            // Set up the Automation checkboxes
            checkBoxUseAutomation.Checked = _poolConfiguration.AutoBalanceEnabled;
            checkBoxEnablePowerManagement.Checked = _poolConfiguration.PowerManagementEnabled;
            checkBoxEnablePowerManagement.Enabled = checkBoxUseAutomation.Checked;

            this.decentGroupBoxPowerManagementHosts.Enabled = true;

            // Set up the Power Management Host enlistment listview
            listViewExPowerManagementHosts.Columns.Clear();
            listViewExPowerManagementHosts.Columns.Add(String.Empty, 25);
            listViewExPowerManagementHosts.Columns.Add(Messages.WLB_HOST_SERVER, 220); // "Host Server"
            listViewExPowerManagementHosts.Columns.Add(Messages.WLB_POWERON_MODE, 95); // "PowerOn Mode"
            listViewExPowerManagementHosts.Columns.Add(Messages.WLB_LAST_POWERON_SUCCEEDED, 135); // "Tested"

            foreach (Host host in _connection.Cache.Hosts)
            {
                bool participatesInPowerManagement = false;
                string powerManagementTested = Messages.NO;

                if (_poolConfiguration.HostConfigurations.ContainsKey(host.uuid))
                {
                    participatesInPowerManagement = _poolConfiguration.HostConfigurations[host.uuid].ParticipatesInPowerManagement;
                    powerManagementTested = _poolConfiguration.HostConfigurations[host.uuid].LastPowerOnSucceeded ? Messages.YES : Messages.NO;
                }

                ListViewItem thisItem = new ListViewItem();
                thisItem.Tag = host;
                thisItem.Checked = participatesInPowerManagement;
                if (host.IsMaster())
                {
                    thisItem.SubItems.Add(string.Format("{0} ({1})", host.Name(), Messages.POOL_MASTER));
                }
                else
                {
                    thisItem.SubItems.Add(host.Name());
                }
                thisItem.SubItems.Add(GetHostPowerOnMode(host.power_on_mode));
                thisItem.SubItems.Add(powerManagementTested);
                listViewExPowerManagementHosts.Items.Add(thisItem);
            }

            labelNoHosts.Visible = (listViewExPowerManagementHosts.Items.Count == 0);
            _loading = false;
        }

        //Maintain minimum column widths
        private void listViewExPowerManagementHosts_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (e.ColumnIndex < minimumColumnWidths.Length)
            {
                if (listViewExPowerManagementHosts.Columns[e.ColumnIndex].Width < minimumColumnWidths[e.ColumnIndex])
                {
                    listViewExPowerManagementHosts.Columns[e.ColumnIndex].Width = minimumColumnWidths[e.ColumnIndex];
                }
            }
        }

        private string GetHostPowerOnMode(string internalMode)
        {
            string mode;

            switch (internalMode.ToLower())
            {
                case null:
                case "":
                    {
                        mode = Messages.PM_MODE_DISABLED;
                        break;
                    }
                case "wake-on-lan":
                    {
                        mode = Messages.PM_MODE_WOL;
                        break;
                    }
                case "ilo":
                    {
                        mode = Messages.PM_MODE_ILO;
                        break;
                    }
                case "drac":
                    {
                        mode = Messages.PM_MODE_DRAC;
                        break;
                    }
                default:
                    {
                        mode = Messages.PM_MODE_CUSTOM;
                        break;
                    }
            }

            return mode;
        }

        #region Control Event Handlers

        private void checkBoxUseAutomation_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                _hasChanged = true;
            }

            checkBoxEnablePowerManagement.Enabled = checkBoxUseAutomation.Checked;
        }

        private void checkBoxEnablePowerManagement_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                _hasChanged = true;
            }
        }

        private void listViewExPowerManagementHosts_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!_loading)
            {
                if (listViewExPowerManagementHosts.Items[e.Index].Tag is Host)
                {
                    Host host = (Host)listViewExPowerManagementHosts.Items[e.Index].Tag;
                    if (!HostCannotParticipateInPowerManagement(host))
                    {
                        if (e.NewValue == CheckState.Checked &&
                            _uuidSet.Contains(host.uuid) &&
                            (!_poolConfiguration.HostConfigurations.ContainsKey(host.uuid) ||
                             !_poolConfiguration.HostConfigurations[host.uuid].LastPowerOnSucceeded))
                        {
                            DialogResult dr;
                            using (var dlg = new ThreeButtonDialog(
                                new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.WLB_UNTESTED_HOST_WARNING, Messages.WLB_UNTESTED_HOST_CAPTION),
                                ThreeButtonDialog.ButtonYes,
                                ThreeButtonDialog.ButtonNo))
                            {
                                dr = dlg.ShowDialog();
                            }
                            if (dr == DialogResult.No)
                            {
                                e.NewValue = e.CurrentValue;
                            }
                            else
                            {
                                _hasChanged = true;
                            }
                        }
                        else
                        {
                            _uuidSet.Add(host.uuid);
                            _hasChanged = true;
                        }
                    }
                    else
                    {
                        e.NewValue = CheckState.Unchecked;
                    }
                }
            }
        }

        private void listViewExPowerManagementHosts_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listViewExPowerManagementHosts_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            Host host = (Host)e.Item.Tag;
            // disallow checking when the host is pool master or PowerOn is disabled (empty string)
            // we have to allow checking when PowerOn is null (unknown) to support PM on older versions of XS
            if (HostCannotParticipateInPowerManagement(host))
            {
                ControlPaint.DrawCheckBox(e.Graphics, e.Bounds.Left + 3, e.Bounds.Top + 1, 15, 15, ButtonState.Inactive);
            }
            else
            {
                if (e.Item.Checked)
                    ControlPaint.DrawCheckBox(e.Graphics, e.Bounds.Left + 3, e.Bounds.Top + 1, 15, 15, ButtonState.Checked);
                else
                    ControlPaint.DrawCheckBox(e.Graphics, e.Bounds.Left + 3, e.Bounds.Top + 1, 15, 15, ButtonState.Normal);
            }
        }

        private bool HostCannotParticipateInPowerManagement(Host host)
        {
            return host.IsMaster() || string.IsNullOrEmpty(host.power_on_mode);
        }

        private void listViewExPowerManagementHosts_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            // CA-59618 - Add a small buffer to the top of each row to prevent truncating larger character sets.
            using (SolidBrush brush = new SolidBrush(listViewExPowerManagementHosts.ForeColor))
            {
                Rectangle rect = new Rectangle(e.Bounds.Left, e.Bounds.Top + 1, e.Bounds.Width, e.Bounds.Height);
                StringFormat sf = new StringFormat(StringFormatFlags.LineLimit | StringFormatFlags.NoWrap);
                e.Graphics.DrawString(e.SubItem.Text, listViewExPowerManagementHosts.Font, brush, rect, sf);
            }
        }

        #endregion

        #region IEditPage Members

        public AsyncAction SaveSettings()
        {
            _poolConfiguration.AutoBalanceEnabled = checkBoxUseAutomation.Checked;
            if (checkBoxUseAutomation.Checked)
            {
                _poolConfiguration.PowerManagementEnabled = checkBoxEnablePowerManagement.Checked;
            }
            else
            {
                _poolConfiguration.PowerManagementEnabled = false;
            }

            foreach (ListViewItem listItem in listViewExPowerManagementHosts.Items)
            {
                Host host = (Host)listItem.Tag;
                WlbHostConfiguration hostConfiguration;
                if (!_poolConfiguration.HostConfigurations.TryGetValue(host.uuid, out hostConfiguration))
                {
                    hostConfiguration = new WlbHostConfiguration(host.uuid);
                    _poolConfiguration.HostConfigurations.Add(hostConfiguration.Uuid, hostConfiguration);
                }

                if ((listItem.Checked == true) && (!host.IsMaster()))
                {
                    hostConfiguration.ParticipatesInPowerManagement = true;
                }
                else
                {
                    hostConfiguration.ParticipatesInPowerManagement = false;
                }
            }

            return null;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            throw new NotImplementedException();
        }

        public bool ValidToSave => throw new NotImplementedException();

        public void ShowLocalValidationMessages()
        {
            throw new NotImplementedException();
        }

        public void Cleanup()
        {
            throw new NotImplementedException();
        }

        public bool HasChanged => _hasChanged;

        #endregion

        #region IVerticalTab Members


        public string SubText => Messages.WLB_AUTOMATION_SUBTEXT;

        public Image Image => Properties.Resources._000_EnablePowerControl_h32bit_16;

        #endregion

    }
}
