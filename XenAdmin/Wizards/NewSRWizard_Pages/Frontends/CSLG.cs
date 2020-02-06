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
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.StorageLinkAPI;
using XenAPI;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using XenAdmin.Properties;

namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class CSLG : XenTabPage
    {
        #region Private fields
        private SR _srToReattach;
        private bool _disasterRecoveryTask;
        private int _storageSystemComboLastSelectedIndex = -1;

        private const string ADAPTER_ID = "adapterid";
        private const string STORAGE_SYSTEM_ID = "storageSystemId";
        #endregion

        public CSLG()
        {
            InitializeComponent();
        }

        #region Accessors

        public StorageLinkAdapterBoston SelectedStorageAdapter { get { return comboBoxStorageSystem.SelectedItem as StorageLinkAdapterBoston; } }

        public CslgSystemStorage SelectedStorageSystem { get { return comboBoxStorageSystem.SelectedItem as CslgSystemStorage; } }

        public ReadOnlyCollection<CslgStoragePool> StoragePools { get; private set; }

        public bool NetAppSelected
        {
            get
            {
                var item = comboBoxStorageSystem.SelectedItem as string;
                return item != null && item == Messages.CSLG_NETAPP_DIRECT;
            }
        }

        public bool DellSelected
        {
            get
            {
                var item = comboBoxStorageSystem.SelectedItem as string;
                return item != null && item == Messages.CSLG_DELL_DIRECT;
            }
        }

        public Dictionary<string, string> DeviceConfigParts
        {
            get
            {
                var dconf = new Dictionary<string, string>();
                var adapter = comboBoxStorageSystem.SelectedItem as StorageLinkAdapterBoston;
                if (adapter != null)
                    dconf[ADAPTER_ID] = adapter.Id;
                return dconf;
            }
        }

        public SrWizardType SrWizardType
        {
            set
            {
                _srToReattach = value.SrToReattach;
                _disasterRecoveryTask = value.DisasterRecoveryTask;
            }
        }

        #endregion

        private static List<StorageLinkCredentials> GetAllValidStorageLinkCreds()
        {
            var credsList = new List<StorageLinkCredentials>();

            foreach (IXenConnection c in ConnectionsManager.XenConnectionsCopy.FindAll(c => c.IsConnected && !Helpers.FeatureForbidden(c, Host.RestrictStorageChoices)))
            {
                var p = Helpers.GetPoolOfOne(c);
                credsList.AddRange(Array.ConvertAll(p.Connection.Cache.PBDs, pbd => pbd.GetStorageLinkCredentials()));
            }
            credsList.RemoveAll(cc => cc == null || !cc.IsValid);
            return credsList;
        }

        /// <summary>
        /// Performs a scan of the CSLG host specified in the textboxes. 
        /// </summary>
        /// <returns>True, if the scan succeeded, otherwise False.</returns>
        public bool PerformStorageSystemScan()
        {
            var items = new List<object>();

            if (Connection.IsConnected && !Helpers.FeatureForbidden(Connection, Host.RestrictStorageChoices))
            {
                if (_srToReattach == null || _srToReattach.type == "cslg")
                {
                    var action = new SrCslgAdaptersScanAction(Connection);
                    using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                    {
                        // never show the error message if it fails.
                        action.Completed += s =>
                        {
                            if (!action.Succeeded)
                            {
                                Program.Invoke(dialog, dialog.Close);
                            }
                        };

                        dialog.ShowDialog(this);
                    }
                    if (action.Succeeded)
                    {
                        var adapters = action.GetAdapters();
                        items.AddRange(Util.PopulateList<object>(adapters));
                        items.Sort((x, y) => x.ToString().CompareTo(y.ToString()));
                    }
                    else
                        return false;
                }
            }

            bool bostonHasDell = false;
            bool bostonHasNetapp = false;
            if (items != null)
            {
                bostonHasDell = (items.Find(item => ((StorageLinkAdapterBoston)item).Id == "DELL_EQUALLOGIC") != null);
                bostonHasNetapp = (items.Find(item => ((StorageLinkAdapterBoston)item).Id == "NETAPP") != null);
            }

            comboBoxStorageSystem.Items.Clear();

            if (_srToReattach != null)
            {
                if (_srToReattach.type == "equal")
                {
                    // a direct-connect Equallogic is being reattached. Only add this item.
                    comboBoxStorageSystem.Items.Add(Messages.CSLG_DELL_DIRECT);
                }
                else if (_srToReattach.type == "netapp")
                {
                    // a direct-connect NetApp is being reattached. Only add this item.
                    comboBoxStorageSystem.Items.Add(Messages.CSLG_NETAPP_DIRECT);
                }
            }
            else
            {
                // a pool or host was selected in the mainwindow tree when the wizard was launched.
                if (!bostonHasDell || !bostonHasNetapp)
                    comboBoxStorageSystem.Items.Add(new NonSelectableComboBoxItem(Messages.CSLG_DIRECT_CONNECTION, true));

                if (!bostonHasDell)
                    comboBoxStorageSystem.Items.Add(Messages.CSLG_DELL_DIRECT);
                if (!bostonHasNetapp)
                    comboBoxStorageSystem.Items.Add(Messages.CSLG_NETAPP_DIRECT);
            }

            if (items != null && items.Count > 0)
            {
                if (!bostonHasDell || !bostonHasNetapp)
                    comboBoxStorageSystem.Items.Add(new NonSelectableComboBoxItem(Messages.CSLG_STORAGELINK_ADAPTERS, true));
                comboBoxStorageSystem.Items.AddRange(items.ToArray());
            }

            if (comboBoxStorageSystem.SelectedIndex < 0 && comboBoxStorageSystem.Items.Count > 0)
            {
                // select the first selectable item if nothing's already been selected.
                comboBoxStorageSystem.SelectedItem = Util.PopulateList<object>(comboBoxStorageSystem.Items).Find(s => !(s is NonSelectableComboBoxItem));
                if (_srToReattach != null && _srToReattach.type == "cslg")
                {
                    comboBoxStorageSystem.SelectedItem =
                        Util.PopulateList<object>(comboBoxStorageSystem.Items).Find(s =>
                        {
                            var bostonadapter = s as StorageLinkAdapterBoston;
                            if (bostonadapter != null)
                            {
                                // sm_config["md_svid"] looks like "DELL__EQUALLOGIC__{GUID}"
                                // bostonadapter.Id looks like "DELL_EQUALLOGIC"
                                // Additionally, EMC__CLARIION is always SMI-S (CA-72968)
                                if (_srToReattach.sm_config.ContainsKey("md_svid"))
                                {
                                    var svid = _srToReattach.sm_config["md_svid"];
                                    if (svid.Replace("__", "_").StartsWith(bostonadapter.Id))
                                        return true;
                                    if (bostonadapter.Id == "SMIS_STORAGE_SYSTEM" && svid.StartsWith("EMC__CLARIION"))
                                        return true;
                                    return false;
                                }
                            }
                            return false;
                        });
                }
            }
            return true;
        }

        #region XenTabPage overrides

        public override void PopulatePage()
        {
            labelAdapter.Visible = true;
            labelStorageSystem.Text = Messages.CSLG_STORAGEADAPTER;
        }

        public override string Text
        {
            get { return Messages.STORAGE_ADAPTER; }
        }

        public override string PageTitle
        {
            get { return Messages.NEWSR_CSLG_ADAPTER_PAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return "SL_System"; }
        }

        public override bool EnableNext()
        {
            return comboBoxStorageSystem.SelectedItem != null &&
                   !(comboBoxStorageSystem.SelectedItem is NonSelectableComboBoxItem);
        }

        public override bool EnablePrevious()
        {
            if (_disasterRecoveryTask && _srToReattach == null)
                return false;

            return true;
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

        #region Event handlers

        private void comboBoxStorageSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxStorageSystem.SelectedItem is NonSelectableComboBoxItem)
            {
                if (comboBoxStorageSystem.SelectedIndex < comboBoxStorageSystem.Items.Count - 1 &&
                    (_storageSystemComboLastSelectedIndex < comboBoxStorageSystem.SelectedIndex || comboBoxStorageSystem.SelectedIndex == 0))
                {
                    _storageSystemComboLastSelectedIndex = comboBoxStorageSystem.SelectedIndex;
                    comboBoxStorageSystem.SelectedIndex++;
                }
                else
                {
                    _storageSystemComboLastSelectedIndex = comboBoxStorageSystem.SelectedIndex;
                    comboBoxStorageSystem.SelectedIndex--;
                }
            }
            else
            {
                _storageSystemComboLastSelectedIndex = comboBoxStorageSystem.SelectedIndex;
            }

            OnPageUpdated();
        }

        private void comboBoxStorageSystem_DrawItem(object sender, DrawItemEventArgs e)
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
    }
}
