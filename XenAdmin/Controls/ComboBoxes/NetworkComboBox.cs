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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Controls
{
    public class NetworkComboBox : LongStringComboBox
    {
        public NetworkComboBox()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public bool IncludePoolNameInComboBox { get; set; }
        public bool IncludeOnlyEnabledNetworksInComboBox { get; set; }
        public bool IncludeOnlyNetworksWithIPAddresses { get; set; }

        private IXenConnection PopulateConnection { get; set; }

        public void PopulateComboBox(IXenConnection connection, Func<NetworkComboBoxItem, bool> matchSelectionCriteria)
        {
            PopulateConnection = connection;
            Items.Clear();

            var pifArray = connection.Cache.PIFs;

            bool found = false;

            foreach (var pif in pifArray)
            {
                var curPif = pif;
                var count = (from NetworkComboBoxItem existingItem in Items
                             where existingItem.Network.opaque_ref == curPif.network.opaque_ref
                             select existingItem).Count();
                if (count > 0)
                    continue;

                
                var item = CreateNewItem(pif);

                if (IncludeOnlyEnabledNetworksInComboBox && !item.NetworkIsConnected)
                    continue;
                if (IncludeOnlyNetworksWithIPAddresses && !item.HasIPAddress)
                    continue;

                Items.Add(item);

                if (!found && matchSelectionCriteria(item))
                {
                    SelectedItem = item;
                    found = true;
                }
            }
        }


        private NetworkComboBoxItem CreateNewItem(PIF pif)
        {
            var network = PopulateConnection.Resolve(pif.network);

            return new NetworkComboBoxItem
                       {
                           IncludePoolNameInComboBox = IncludePoolNameInComboBox,
                           IsManagement = pif.management,
                           Network = network,
                           NetworkIsConnected = pif.LinkStatus() == PIF.LinkState.Connected,
                           HasIPAddress = pif.IsManagementInterface(false)
                       };
        }

        /// <summary>
        /// Get the uuid of the selected item along with its name in a pair
        /// </summary>
        public KeyValuePair<string, string> SelectedNetworkUuid
        {
            get
            {
                var selectedItem = (NetworkComboBoxItem)SelectedItem;
                return selectedItem == null
                        ? new KeyValuePair<string, string>(string.Empty, string.Empty)
                        : new KeyValuePair<string, string>(selectedItem.Network.uuid, selectedItem.Network.Name());
            }
        }
    }
}
