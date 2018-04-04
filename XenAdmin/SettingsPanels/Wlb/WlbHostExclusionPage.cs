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
using System.Data;
using System.Text;
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.Wlb;
using XenAPI;


namespace XenAdmin.SettingsPanels
{
    public partial class WlbHostExclusionPage : UserControl, IEditPage
    {
        private WlbPoolConfiguration _poolConfiguration;
        private XenAdmin.Network.IXenConnection _connection;
        private bool _hasChanged = false;
        private bool _loading = false;

        private int[] minimumColumnWidths = { 25, 50 };

        //private Pool _pool;

        public WlbHostExclusionPage()
        {
            InitializeComponent();

            listViewExExcludedHostsSorter = new XenAdmin.Controls.ListViewColumnSorter();
            this.listViewExExcludedHosts.ListViewItemSorter = listViewExExcludedHostsSorter;

            Text = Messages.WLB_HOST_EXCLUSION;
        }

        public WlbPoolConfiguration PoolConfiguration
        {
            set
            {
                if (null != value)
                {
                    _poolConfiguration = value;
                    if (null != _connection)
                    {
                        InitializeControls();
                    }
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
                    if (null != _poolConfiguration)
                    {
                        InitializeControls();
                    }
                }
            }
        }

        private void InitializeControls()
        {
            _loading = true;

            listViewExExcludedHosts.Columns.Clear();
            listViewExExcludedHosts.Columns.Add(String.Empty);
            listViewExExcludedHosts.Columns.Add(Messages.WLB_HOST_SERVER); // "Host Server"
            listViewExExcludedHosts.Columns[0].Width = 25;
            listViewExExcludedHosts.Columns[1].Width = 425;

            foreach (Host host in _connection.Cache.Hosts)
            {
                bool hostExcluded = false;

                if (_poolConfiguration.HostConfigurations.ContainsKey(host.uuid))
                {
                    hostExcluded = _poolConfiguration.HostConfigurations[host.uuid].Excluded;
                }
                ListViewItem thisItem = new ListViewItem();
                thisItem.SubItems.Add(host.Name());
                listViewExExcludedHosts.Items.Add(thisItem);
                thisItem.Tag = host;
                thisItem.Checked = hostExcluded;     
            }

            listViewExExcludedHostsSorter.SortColumn = 1;
            listViewExExcludedHostsSorter.Order = SortOrder.Ascending;
            listViewExExcludedHosts.Sort();

            _loading = false;
        }

        private void listViewExExcludedHosts_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!_loading)
            {
                _hasChanged = true;
            }
        }

        private void listViewExExcludedHosts_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (e.ColumnIndex < minimumColumnWidths.Length)
            {
                if (listViewExExcludedHosts.Columns[e.ColumnIndex].Width < minimumColumnWidths[e.ColumnIndex])
                {
                    listViewExExcludedHosts.Columns[e.ColumnIndex].Width = minimumColumnWidths[e.ColumnIndex];
                }
            }
        }

        #region IEditPage Members

        public AsyncAction SaveSettings()
        {
            foreach (ListViewItem listItem in listViewExExcludedHosts.Items)
            {
                Host host = (Host)listItem.Tag;
                WlbHostConfiguration hostConfiguration;
                if (!_poolConfiguration.HostConfigurations.TryGetValue(host.uuid, out hostConfiguration))
                {
                    hostConfiguration = new WlbHostConfiguration(host.uuid);
                    _poolConfiguration.HostConfigurations.Add(hostConfiguration.Uuid, hostConfiguration);
                }
                hostConfiguration.Excluded = listItem.Checked;
            }
            return null;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool ValidToSave
        {
            get { return true; }
        }

        public void ShowLocalValidationMessages()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Cleanup()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool HasChanged
        {
            get { return _hasChanged; }
        }

        #endregion

        #region IVerticalTab Members


        public string SubText
        {
            get { return Messages.WLB_HOST_EXCLUSION; }
        }

        public Image Image
        {
            get { return Properties.Resources._000_ExcludeHost_h32bit_16; }
        }

        #endregion

    }
}
