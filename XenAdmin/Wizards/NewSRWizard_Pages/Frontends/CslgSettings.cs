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
using System.Collections.ObjectModel;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.StorageLinkAPI;
using XenAPI;
using System.Drawing;

namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class CslgSettings : XenTabPage
    {
        private const string CHAPUSER = "chapuser";
        private const string CHAPPASSWORD = "chappassword";
        private const string STORAGE_POOL_ID = "storagePoolId";
        private const string PROTOCOL = "protocol";
        private const string PROVISION_OPTIONS = "provision-options";
        private const string PROVISION_TYPE = "provision-type";
        private const string RAID_TYPE = "raid-type";

        public CslgSettings()
        {
            InitializeComponent();
        }

        private void UpdateStoragePoolSpaceLabel()
        {
            CslgStoragePool pool = comboBoxStoragePool.SelectedItem as CslgStoragePool;
            labelProgress.Visible = simpleProgressBar1.Visible = pool != null && pool.StorageLinkPool != null;

            if (pool != null && pool.StorageLinkPool != null)
            {
                string capacityText = Util.DiskSizeString(pool.StorageLinkPool.Capacity * 1024L * 1024L);
                string freeSpaceText = Util.DiskSizeString(pool.StorageLinkPool.FreeSpace * 1024L * 1024L);
                string spaceText = string.Format(Messages.STORAGELINK_POOL_DISK_SPACE, freeSpaceText, capacityText);

                double capacity = Math.Max(1, (double)pool.StorageLinkPool.Capacity);
                double freeSpace = (double)pool.StorageLinkPool.FreeSpace;
                double usedSpacePerc = (capacity - freeSpace) / capacity;

                simpleProgressBar1.Progress = usedSpacePerc;
                labelProgress.Text = spaceText;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateStoragePoolSpaceLabel();
        }

        #region Event handlers

        private void comboBoxStoragePools_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxRaidType.Items.Clear();
            comboBoxProvisioningType.Items.Clear();

            if (comboBoxStoragePool.Items.Count > 0)
            {
                CslgStoragePool pool = comboBoxStoragePool.SelectedItem as CslgStoragePool;

                if (pool != null)
                {
                    foreach (string raidType in pool.RaidTypes)
                        comboBoxRaidType.Items.Add(raidType);

                    if (pool.RaidTypes.Count > 0)
                        comboBoxRaidType.SelectedIndex = 0;

                    foreach (CslgParameter provisioningType in pool.ProvisioningTypes)
                        comboBoxProvisioningType.Items.Add(provisioningType);

                    if (pool.ProvisioningTypes.Count > 0)
                        comboBoxProvisioningType.SelectedIndex = 0;
                }
            }

            OnPageUpdated();
            UpdateStoragePoolSpaceLabel();
        }

        private void checkBoxShowAll_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxStoragePool.Items.Clear();

            foreach (CslgStoragePool pool in StoragePools)
            {
                if (checkBoxShowAll.Checked || !pool.HasParent)
                    comboBoxStoragePool.Items.Add(pool);
            }

            if (comboBoxStoragePool.Items.Count > 0)
                comboBoxStoragePool.SelectedIndex = 0;
        }

        private void checkBoxUseChap_CheckedChanged(object sender, EventArgs e)
        {
            labelChapSecret.Enabled = checkBoxUseChap.Checked;
            labelChapUser.Enabled = checkBoxUseChap.Checked;
            textBoxChapSecret.Enabled = checkBoxUseChap.Checked;
            textBoxChapUser.Enabled = checkBoxUseChap.Checked;
        }

        private void comboBoxStoragePool_DrawItem(object sender, DrawItemEventArgs e)
        {
            const int progBarWidth = 130;
            const int progBarHeight = 11;

            if (e.Index >= 0)
            {
                CslgStoragePool item = (CslgStoragePool)comboBoxStoragePool.Items[e.Index];

                e.DrawBackground();

                bool isEdit = (e.State & DrawItemState.ComboBoxEdit) != 0;
                Rectangle textBounds = new Rectangle(e.Bounds.X, e.Bounds.Y - 1, e.Bounds.Width - (isEdit ? 0 : progBarWidth + 5), e.Bounds.Height);
                Drawing.DrawText(e.Graphics, item.ToString(), e.Font, textBounds, e.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                if (item.StorageLinkPool != null && !isEdit)
                {
                    string capacityText = Util.DiskSizeString(item.StorageLinkPool.Capacity * 1024L * 1024L);
                    string freeSpaceText = Util.DiskSizeString(item.StorageLinkPool.FreeSpace * 1024L * 1024L);
                    string spaceText = string.Format(Messages.STORAGELINK_POOL_DISK_SPACE, freeSpaceText, capacityText);

                    double capacity = Math.Max(1, (double)item.StorageLinkPool.Capacity);
                    double freeSpace = (double)item.StorageLinkPool.FreeSpace;
                    double usedSpacePerc = (capacity - freeSpace) / capacity;

                    Rectangle progBarBounds = new Rectangle(e.Bounds.Right - progBarWidth - 3, e.Bounds.Bottom - progBarHeight - 6, progBarWidth, progBarHeight);
                    Drawing.DrawSimpleProgressBar(e.Graphics, progBarBounds, usedSpacePerc, XenAdmin.Core.Drawing.SimpleProgressBarColor.Blue);

                    // draw free space info
                    textBounds = new Rectangle(e.Bounds.Left + 2, e.Bounds.Top + 2, e.Bounds.Width, e.Bounds.Height);
                    Drawing.DrawText(e.Graphics, spaceText, e.Font, textBounds, e.ForeColor, TextFormatFlags.Right | TextFormatFlags.EndEllipsis);

                    if (e.Index < comboBoxStoragePool.Items.Count - 1)
                    {
                        //draw grey line underneath for all items except last
                        e.Graphics.DrawLine(SystemPens.ControlLight, new Point(e.Bounds.Left, e.Bounds.Bottom - 1), new Point(e.Bounds.Right, e.Bounds.Bottom - 1));
                    }
                }

                if ((e.State & DrawItemState.NoFocusRect) == 0)
                {
                    // draw focus rect if required.
                    e.DrawFocusRectangle();
                }
            }
        }

        private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            if (e.Index >= 0)
            {
                e.DrawBackground();

                Drawing.DrawText(e.Graphics, comboBox.Items[e.Index].ToString(), e.Font, e.Bounds, e.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                if ((e.State & DrawItemState.NoFocusRect) == 0)
                {
                    e.DrawFocusRectangle();
                }
            }
        }

        private void comboBoxStoragePool_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            int rightColWidth = 130;
            object item = comboBoxStoragePool.Items[e.Index];

            int width = TextRenderer.MeasureText(item.ToString(), comboBoxStoragePool.Font).Width + rightColWidth;

            e.ItemWidth = width;
            e.ItemHeight = 33;
        }

        private void comboBoxStoragePool_SizeChanged(object sender, EventArgs e)
        {
            if (comboBoxStoragePool.Width > 0)
                comboBoxStoragePool.DropDownWidth = comboBoxStoragePool.Width;
        }

        #endregion

        #region XenTabPage overrides

        private void SetCHAPAuthenticationControls(bool visible)
        {
            checkBoxUseChap.Visible = visible;
            labelChapUser.Visible = textBoxChapUser.Visible = visible;
            labelChapSecret.Visible = textBoxChapSecret.Visible = visible;
            if (!visible)
            {
                checkBoxUseChap.Checked = false;
                textBoxChapUser.Clear();
                textBoxChapSecret.Clear();
            }
        }

        public override void PopulatePage()
        {
            comboBoxStoragePool.Items.Clear();

            foreach (CslgStoragePool pool in StoragePools)
            {
                if (checkBoxShowAll.Checked || !pool.HasParent)
                    comboBoxStoragePool.Items.Add(pool);
            }

            if (comboBoxStoragePool.Items.Count > 0 && comboBoxStoragePool.SelectedIndex < 0)
                comboBoxStoragePool.SelectedIndex = 0;

            comboBoxProtocol.Items.Clear();
            comboBoxProvisionOptions.Items.Clear();

            if (SystemStorage != null)
            {
                checkBoxShowAll.Visible = true;

                foreach (CslgParameter protocol in SystemStorage.Protocols)
                    comboBoxProtocol.Items.Add(protocol);

                if (comboBoxProtocol.Items.Count > 0)
                    comboBoxProtocol.SelectedIndex = 0;

                foreach (CslgParameter provisioningOption in SystemStorage.ProvisioningOptions)
                    comboBoxProvisionOptions.Items.Add(provisioningOption);

                if (comboBoxProvisionOptions.Items.Count > 0)
                    comboBoxProvisionOptions.SelectedIndex = 0;

                SetCHAPAuthenticationControls(SystemStorage.SupportsCHAP);
            }
            else
            {
                SetCHAPAuthenticationControls(false);
            }
        }

        public override string PageTitle { get { return Messages.NEWSR_CSLG_SETTINGS_PAGE_TITLE; } }

        public override string Text { get { return Messages.SETTINGS; } }

        public override string HelpID { get { return "SL_SLServer_options"; } }

        public override bool EnableNext()
        {
            return comboBoxStoragePool.SelectedItem is CslgStoragePool;
        }

        #endregion

        #region Accessors

        public ReadOnlyCollection<CslgStoragePool> StoragePools { private get; set; }

        public StorageLinkAdapterBoston SelectedStorageAdapter { private get; set; }

        public CslgSystemStorage SystemStorage { private get; set; }

        public StorageLinkCredentials StorageLinkCredentials { private get; set; }

        public Dictionary<string, string> DeviceConfigParts
        {
            get
            {
                var dconf = new Dictionary<string, string>();

                if (checkBoxUseChap.Checked && !string.IsNullOrEmpty(textBoxChapUser.Text) && !string.IsNullOrEmpty(textBoxChapSecret.Text))
                {
                    dconf[CHAPUSER] = textBoxChapUser.Text;
                    dconf[CHAPPASSWORD] = textBoxChapSecret.Text;
                }

                CslgStoragePool pool = comboBoxStoragePool.SelectedItem as CslgStoragePool;
                if (pool != null)
                    dconf[STORAGE_POOL_ID] = pool.StoragePoolId;

                CslgParameter protocol = comboBoxProtocol.SelectedItem as CslgParameter;
                if (protocol != null && protocol.Name != null)
                    dconf[PROTOCOL] = protocol.Name;

                CslgParameter provisioningOptions = comboBoxProvisionOptions.SelectedItem as CslgParameter;
                if (provisioningOptions != null && provisioningOptions.Name != null)
                    dconf[PROVISION_OPTIONS] = provisioningOptions.Name;

                CslgParameter provisioningType = comboBoxProvisioningType.SelectedItem as CslgParameter;
                if (provisioningType != null && provisioningType.Name != null)
                    dconf[PROVISION_TYPE] = provisioningType.Name;

                string raidType = comboBoxRaidType.SelectedItem as string;
                if (raidType != null)
                    dconf[RAID_TYPE] = raidType;

                return dconf;
            }
        }

        public string SrDescription
        {
            get { return String.Format("{0} ({1})", SystemStorage, comboBoxStoragePool.SelectedItem); }
        }

        #endregion
    }
}
