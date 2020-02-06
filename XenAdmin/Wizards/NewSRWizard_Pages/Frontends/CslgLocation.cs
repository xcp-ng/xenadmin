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
using XenAdmin.Dialogs;
using XenAdmin.StorageLinkAPI;
using XenAPI;
using System.Drawing;
using XenAdmin.Properties;

namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class CslgLocation : XenTabPage
    {
        private const string TARGET = "target";
        private const string USERNAME = "username";
        private const string PASSWORD = "password";
        private const string STORAGE_SYSTEM_ID = "storageSystemId";

        public CslgLocation()
        {
            InitializeComponent();
        }

        #region Accessors

        public StorageLinkAdapterBoston SelectedStorageAdapter { private get; set; }

        public ReadOnlyCollection<CslgStoragePool> StoragePools { get; private set; }

        public CslgSystemStorage SystemStorage { get { return comboBoxStorageSystemBoston.SelectedItem as CslgSystemStorage; } }

        public StorageLinkCredentials StorageLinkCredentials
        {
            get
            {
                return new StorageLinkCredentials(Connection, textBoxTarget.Text, textBoxUsername.Text,
                                                  textBoxPassword.Text, textBoxPassword.Text);
            }
        }

        public Dictionary<string, string> DeviceConfigParts
        {
            get
            {
                var dconf = new Dictionary<string, string>();
                dconf[TARGET] = textBoxTarget.Text;
                dconf[USERNAME] = textBoxUsername.Text;
                dconf[PASSWORD] = textBoxPassword.Text;
                dconf[STORAGE_SYSTEM_ID] = SystemStorage.StorageSystemId;
                return dconf;
            }
        }

        #endregion

        #region XenTabPage overrides

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Forward)
                cancel = !PerformStoragePoolScan();
        }

        public override string Text { get { return Messages.STORAGE_SYSTEM; } }

        public override string PageTitle { get { return Messages.NEWSR_CSLG_PAGE_TITLE; } }

        public override string HelpID { get { return "SL_SLServer_location"; } }

        public override bool EnableNext()
        {
            return comboBoxStorageSystemBoston.SelectedIndex > 0;
        }

        public override void PopulatePage()
        {
            textBoxTarget.Text = textBoxUsername.Text = textBoxPassword.Text = string.Empty;
            comboBoxStorageSystemBoston.Items.Clear();
            UpdateStorageSystemControls();
        }

        #endregion

        private bool PerformStoragePoolScan()
        {
            var credentials = StorageLinkCredentials;

            var scanAction = new SrCslgStoragePoolScanAction(Connection, credentials.Host, credentials.Username, credentials.PasswordSecret,
                                                             SystemStorage.StorageSystemId, SelectedStorageAdapter.Id);

            using (var dialog = new ActionProgressDialog(scanAction, ProgressBarStyle.Marquee))
                dialog.ShowDialog(this);

            if (scanAction.Succeeded)
                StoragePools = scanAction.CslgStoragePools;

            return scanAction.Succeeded;
        }

        private void UpdateStorageSystemControls()
        {
            buttonDiscoverBostonSS.Enabled = !string.IsNullOrEmpty(textBoxUsername.Text) && !string.IsNullOrEmpty(textBoxTarget.Text);
            comboBoxStorageSystemBoston.Enabled = false;
            comboBoxStorageSystemBoston.Items.Clear();
        }

        #region Event handlers

        private void buttonDiscoverBostonSS_Click(object sender, EventArgs e)
        {
            SrCslgStorageSystemScanAction action = new SrCslgStorageSystemScanAction(Connection, SelectedStorageAdapter.Id, textBoxTarget.Text, textBoxUsername.Text, textBoxPassword.Text);
            using (var dlg = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                dlg.ShowDialog(this);

            var items = new List<object>();
            if (action.Succeeded)
            {
                var storages = action.CslgSystemStorages;
                items.AddRange(Util.PopulateList<object>(storages));
                items.Sort((x, y) => x.ToString().CompareTo(y.ToString()));
            }

            comboBoxStorageSystemBoston.Items.Clear();
            if (items.Count > 0)
            {
                comboBoxStorageSystemBoston.Enabled = true;
                comboBoxStorageSystemBoston.Items.Add(new NonSelectableComboBoxItem(string.Format(Messages.CSLG_STORAGELINK_SERVER, textBoxTarget.Text), true));
                comboBoxStorageSystemBoston.Items.AddRange(items.ToArray());
                // select the first selectable item
                comboBoxStorageSystemBoston.SelectedItem = comboBoxStorageSystemBoston.Items[1];
            }
            else
                comboBoxStorageSystemBoston.Enabled = false;
        }

        private void textBoxTarget_TextChanged(object sender, EventArgs e)
        {
            UpdateStorageSystemControls();
            OnPageUpdated();
        }

        private void comboBoxStorageSystemBoston_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxStorageSystemBoston.SelectedIndex == 0)
                comboBoxStorageSystemBoston.SelectedIndex = 1;
            OnPageUpdated();
        }

        private void comboBoxStorageSystemBoston_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (e.Index > -1)
            {
                var item = comboBox.Items[e.Index];
                var storageSystem = item as CslgSystemStorage;
                var nonSelectable = item as NonSelectableComboBoxItem;
                bool edit = (e.State & DrawItemState.ComboBoxEdit) != 0;
                const int imageWidth = 16;
                int indent = edit ? 0 : (imageWidth / 2);

                e.DrawBackground();

                var textRect = new Rectangle(e.Bounds.X + indent + imageWidth, e.Bounds.Y, e.Bounds.Width - imageWidth - indent, e.Bounds.Height);

                if (storageSystem != null)
                {
                    e.Graphics.DrawImageUnscaled(Resources.sl_system_16, new Point(e.Bounds.X + indent, e.Bounds.Y + 1));
                    Drawing.DrawText(e.Graphics, item.ToString(), e.Font, textRect, e.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }
                else if (nonSelectable != null)
                {
                    // bold headings
                    Drawing.DrawText(e.Graphics, item.ToString(), new Font(e.Font, FontStyle.Bold), e.Bounds, e.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }
                else
                {
                    // dell and netapp direction connection.
                    textRect = new Rectangle(e.Bounds.X + indent, e.Bounds.Y, e.Bounds.Width - indent, e.Bounds.Height);
                    Drawing.DrawText(e.Graphics, item.ToString(), e.Font, textRect, e.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }

                if ((e.State & DrawItemState.NoFocusRect) == 0)
                {
                    e.DrawFocusRectangle();
                }
            }
        }

        #endregion

        #region NonSelectableComboBoxItem class

        private class NonSelectableComboBoxItem
        {
            public string Text { get; private set; }
            public bool Bold { get; private set; }

            public NonSelectableComboBoxItem(string text, bool bold)
            {
                Text = text;
                Bold = bold;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        #endregion
    }
}
