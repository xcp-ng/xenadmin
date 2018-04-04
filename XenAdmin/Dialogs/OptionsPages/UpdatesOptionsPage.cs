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
using XenAdmin.Properties;
using XenAdmin.Core;


namespace XenAdmin.Dialogs.OptionsPages
{
    public partial class UpdatesOptionsPage : UserControl, IOptionsPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public UpdatesOptionsPage()
        {
            InitializeComponent();

            build();
        }

        private void build()
        {
            // XenCenter updates
            AllowXenCenterUpdatesCheckBox.Checked = Properties.Settings.Default.AllowXenCenterUpdates;

            // XenServer updates
            AllowXenServerPatchesCheckBox.Checked = Properties.Settings.Default.AllowPatchesUpdates;
            AllowXenServerUpdatesCheckBox.Checked = Properties.Settings.Default.AllowXenServerUpdates;
        }

        public static void Log()
        {
            // XenCenter updates
            log.Info("=== AllowXenCenterUpdates: " + Properties.Settings.Default.AllowXenCenterUpdates.ToString());

            // XenServer updates
            log.Info("=== AllowPatchesUpdates: " + Properties.Settings.Default.AllowPatchesUpdates.ToString());
            log.Info("=== AllowXenServerUpdates: " + Properties.Settings.Default.AllowXenServerUpdates.ToString());
        }

        #region IOptionsPage Members

        public void Save()
        {
            bool refreshUpdatesTab = IsCheckForUpdatesRequired();

            // XenCenter updates
            if (AllowXenCenterUpdatesCheckBox.Checked != Properties.Settings.Default.AllowXenCenterUpdates)
                Properties.Settings.Default.AllowXenCenterUpdates = AllowXenCenterUpdatesCheckBox.Checked;

            // XenServer updates
            if (AllowXenServerPatchesCheckBox.Checked != Properties.Settings.Default.AllowPatchesUpdates)
                Properties.Settings.Default.AllowPatchesUpdates = AllowXenServerPatchesCheckBox.Checked;
            if (AllowXenServerUpdatesCheckBox.Checked != Properties.Settings.Default.AllowXenServerUpdates)
                Properties.Settings.Default.AllowXenServerUpdates = AllowXenServerUpdatesCheckBox.Checked;

            if(refreshUpdatesTab)
            {
                Updates.CheckForUpdates(false, true);
            }
        }

        /// <summary>
        /// Returns true if at least one box has been changed in Updates Options. Otherwise returns false.
        /// </summary>
        /// <returns></returns>
        private bool IsCheckForUpdatesRequired()
        {
            return (AllowXenCenterUpdatesCheckBox.Checked != Properties.Settings.Default.AllowXenCenterUpdates) ||
                   (AllowXenServerPatchesCheckBox.Checked != Properties.Settings.Default.AllowPatchesUpdates) ||
                   (AllowXenServerUpdatesCheckBox.Checked != Properties.Settings.Default.AllowXenServerUpdates);
        }

        #endregion

        #region IVerticalTab Members

        public override string Text
        {
            get { return Messages.UPDATES; }
        }

        public string SubText
        {
            get { return Messages.UPDATES_DESC; }
        }

        public Image Image
        {
            get { return Resources._000_Patch_h32bit_16; }
        }

        #endregion
    }
}
