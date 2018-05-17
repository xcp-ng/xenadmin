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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Dialogs.OptionsPages;
using XenAdmin.Plugins;

namespace XenAdmin.Dialogs
{
    public partial class OptionsDialog : VerticallyTabbedDialog
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string ToolsOptionsLogHeader = "Tools Options Settings -";

        internal OptionsDialog(PluginManager pluginManager)
        {
            InitializeComponent();
            pluginOptionsPage1.Enabled = pluginManager.Enabled;
            pluginOptionsPage1.PluginManager = pluginManager;
            verticalTabs.SelectedItem = securityOptionsPage1;

            connectionOptionsPage1.OptionsDialog = this;
            if (!Application.RenderWithVisualStyles)
                ContentPanel.BackColor = SystemColors.Control;
            // call save serverlist on OK
            saveAndRestoreOptionsPage1.SaveAllAfter = true;

            if (Helpers.CommonCriteriaCertificationRelease)
                verticalTabs.Items.Remove(updatesOptionsPage1);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            foreach (IOptionsPage page in verticalTabs.Items)
            {
                page.Save();
            }

            Settings.TrySaveSettings();

            Log();

            Close();
        }

        public static void Log()
        {
            log.Info(ToolsOptionsLogHeader);

            ConnectionOptionsPage.Log();
            ConsolesOptionsPage.Log();
            SecurityOptionsPage.Log();
            if (!Helpers.CommonCriteriaCertificationRelease)
                UpdatesOptionsPage.Log();
            DisplayOptionsPage.Log();
            DisplayOptions2Page.Log();
            SaveAndRestoreOptionsPage.Log();
            PluginOptionsPage.Log();
            ConfirmationOptionsPage.Log();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void SelectConnectionOptionsPage()
        {
            SelectPage(connectionOptionsPage1);
        }
    }
}

