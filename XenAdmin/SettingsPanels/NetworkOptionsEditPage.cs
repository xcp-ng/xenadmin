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

using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.SettingsPanels
{
    public partial class NetworkOptionsEditPage : UserControl, IEditPage
    {
        private Pool pool;

        public NetworkOptionsEditPage()
        {
            InitializeComponent();
            Text = Messages.NETWORK_OPTIONS;
        }

        #region IEditPage Members

        public AsyncAction SaveSettings()
        {
            bool now_enabled = radioButtonEnable.Checked;

            string title = null;
            string description = null;

            if (now_enabled)
            {
                title = Messages.ACTION_ENABLE_IGMP_SNOOPING;
                description = Messages.ACTION_DISABLE_IGMP_SNOOPING;
            }
            else
            {
                title = Messages.ACTION_DISABLE_IGMP_SNOOPING;
                description = Messages.ACTION_DISABLE_IGMP_SNOOPING;
            }

            return new DelegatedAsyncAction(pool.Connection, title, description, null,
               delegate(Session session) { Pool.set_igmp_snooping_enabled(session, pool.opaque_ref, radioButtonEnable.Checked); }, true);
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            pool = Helpers.GetPoolOfOne(clone.Connection);  // clone could be a pool or a host

            if (pool != null && pool.igmp_snooping_enabled)
            {
                radioButtonEnable.Checked = true;
            }
            else
            {
                radioButtonDisable.Checked = true;
            }
        }

        public bool ValidToSave
        {
            get { return true; }
        }

        public void ShowLocalValidationMessages()
        { }

        public void Cleanup()
        { }

        public bool HasChanged
        {
            // Server flag is for enabled, so compare to enable radio
            get { return pool != null && (pool.igmp_snooping_enabled != radioButtonEnable.Checked); }
        }

        #endregion

        #region IVerticalTab Members

        public string SubText
        {
            get
            {
                return radioButtonEnable.Checked
                            ? Messages.NETWORKOPTIONSEDITPAGE_SUBTEXT_IGMP_SNOOPING_ENABLED
                            : Messages.NETWORKOPTIONSEDITPAGE_SUBTEXT_IGMP_SNOOPING_DISABLED;
            }
        }

        public Image Image
        {
            get { return Properties.Resources._000_Network_h32bit_16; }
        }

        #endregion
    }
}
