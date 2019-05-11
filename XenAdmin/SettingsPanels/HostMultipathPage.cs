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
using XenAPI;


namespace XenAdmin.SettingsPanels
{
    public partial class HostMultipathPage : UserControl, IEditPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Host host;

        public HostMultipathPage()
        {
            InitializeComponent();

            Text = Messages.MULTIPATHING;

            UpdateMaintenanceWarning();
        }

        private bool MaintenanceMode
        {
            get
            {
                if (host == null)
                    return true;

                Host_metrics metrics = host.Connection.Resolve(host.metrics);

                return host.MaintenanceMode() || (metrics != null && !metrics.live);
            }
        }

        private void multipathCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMaintenanceWarning();
        }

        private void UpdateMaintenanceWarning()
        {
            bool maintenance_mode = MaintenanceMode;

            maintenanceWarningImage.Visible = !maintenance_mode;
            maintenanceWarningLabel.Visible = !maintenance_mode;

            multipathCheckBox.Enabled = maintenance_mode;
        }

        #region IEditPage Members

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            host = clone as Host;
            UpdateMaintenanceWarning();
            Repopulate();
        }     

        public void Repopulate()
        {
            if (host == null)
                return;

            multipathCheckBox.Checked = host.MultipathEnabled();
        }

        public AsyncAction SaveSettings()
        {
            return new EditMultipathAction(host, multipathCheckBox.Checked, true);
        }

        public bool ValidToSave
        {
            get
            {
                return true;
            }
        }

        public void ShowLocalValidationMessages()
        {
        }

        public void Cleanup()
        {
        }

        public bool HasChanged
        {
            get
            {
                return multipathCheckBox.Checked != host.MultipathEnabled();
            }
        }

        public String SubText
        {
            get
            {
                return multipathCheckBox.Checked ? Messages.MULTIPATH_ACTIVE : Messages.MULTIPATH_NOT_ACTIVE;
            }
        }

        public Image Image
        {
            get
            {
                return Properties.Resources._000_Storage_h32bit_16;
            }
        }

        #endregion
    }
}
