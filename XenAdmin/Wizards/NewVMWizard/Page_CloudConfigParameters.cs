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
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Controls;
using XenAdmin.Actions;
using XenAdmin.SettingsPanels;
using System.Drawing;


namespace XenAdmin.Wizards.NewVMWizard
{
    public partial class Page_CloudConfigParameters : XenTabPage, IEditPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private VM vmOrTemplate;
        private string existingConfig;

        public Host Affinity { get; set; }

        public Page_CloudConfigParameters()
        {
            InitializeComponent();
            warningsTable.Visible = false;
        }

        void ConfigDriveTemplateTextBox_TextChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        public override string Text
        {
            get { return Messages.NEWVMWIZARD_CLOUD_CONFIG_PARAMETERS_PAGE; }
        }

        public string SubText
        {
            get { return IncludeConfigDriveCheckBox.Checked ? Messages.VM_CLOUD_CONFIG_DRIVE_INCLUDED : Messages.VM_CLOUD_CONFIG_DRIVE_NOT_INCLUDED; }
        }

        public Image Image
        {
            get { return Properties.Resources.coreos_16; }
        }

        public override string PageTitle
        {
            get { return Messages.NEWVMWIZARD_CLOUD_CONFIG_PARAMETERS_PAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return "CloudConfigParameters"; } 
        }

        public override bool EnableNext()
        {
            return true;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (SelectedTemplate == vmOrTemplate)
                return;

            vmOrTemplate = SelectedTemplate;

            GetCloudConfigParameters();
            ShowHideButtonsAndWarnings(true);
        }

        public VM SelectedTemplate { private get; set; }


        public string ConfigDriveTemplateText
        {
            get
            {
                return IncludeConfigDriveCheckBox.Checked ? ConfigDriveTemplateTextBox.Text.Trim() : string.Empty;
            }
        }

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                List<KeyValuePair<string, string>> sum = new List<KeyValuePair<string, string>>();
                sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_CLOUD_CONFIG_PARAMETERS_PAGE, SubText));
                return sum;
            }
        }

        private void IncludeConfigDriveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ConfigDriveTemplateTextBox.Enabled = IncludeConfigDriveCheckBox.Checked;
        }
        
        private void GetCloudConfigParameters()
        {
            var configDrive = vmOrTemplate.CloudConfigDrive();
            GetCloudConfigParameters(configDrive);
        }

        private bool errorRetrievingConfigParameters;

        private void GetCloudConfigParameters(VDI configDrive)
        {
            var defaultConfig = configDrive == null;
            var parameters = new Dictionary<string, string>();
            if (defaultConfig)
                parameters.Add("templateuuid", vmOrTemplate.uuid);
            else
                parameters.Add("vdiuuid", configDrive.uuid);
            
            var action = new ExecutePluginAction(Connection, Affinity ?? Helpers.GetMaster(Connection),
                        "xscontainer",//plugin
                        defaultConfig ? "get_config_drive_default" : "get_config_drive_configuration",//function
                        parameters,
                        true); //hidefromlogs

            try
            {
                action.RunExternal(Connection.Session);
                var result = action.Result.Replace("\n", Environment.NewLine);
                ConfigDriveTemplateTextBox.Text = result;
                existingConfig = result;
                errorRetrievingConfigParameters = false;
            }
            catch (Exception)
            {
                log.Warn("Could not get the config drive parameters");
                errorRetrievingConfigParameters = true;
            }
        }

        private void GetDefaultParameters()
        {
            GetCloudConfigParameters(null);
        }

        private void ShowHideButtonsAndWarnings(bool inNewVmWizard)
        {
            // IncludeConfigDriveCheckBox and reloadDefaults only visible in the New VM Wizard
            IncludeConfigDriveCheckBox.Visible = reloadDefaults.Visible = inNewVmWizard;

            // for existing VMs, the cloud config cannot be edited on non-halted VMs or when we failed to retrive the existing cloud config parameters
            bool canEdit = inNewVmWizard ||
                           !errorRetrievingConfigParameters && (vmOrTemplate.is_a_template || vmOrTemplate.power_state == vm_power_state.Halted);

            ConfigDriveTemplateTextBox.ReadOnly = !canEdit;
            warningsTable.Visible = errorRetrievingConfigParameters || !canEdit;
            labelWarning.Text = errorRetrievingConfigParameters ? Messages.VM_CLOUD_CONFIG_DRIVE_UNAVAILABLE : Messages.VM_CLOUD_CONFIG_DRIVE_READONLY;
        }

        private void reloadDefaults_Click(object sender, EventArgs e)
        {
            GetDefaultParameters();
            if (errorRetrievingConfigParameters)
                using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Error, Messages.VM_CLOUD_CONFIG_DRIVE_CANNOT_RETRIEVE_DEFAULT)))
                {
                    dlg.ShowDialog(this);
                }
        }

        #region Implementation of IEditPage

        public AsyncAction SaveSettings()
        {
            var configDrive = vmOrTemplate.CloudConfigDrive();
            if (configDrive == null || string.IsNullOrEmpty(ConfigDriveTemplateText))
                return null;
            
            SR sr = vmOrTemplate.Connection.Resolve(configDrive.SR);
            if (sr == null)
                return null;

            var parameters = new Dictionary<string, string>();
            parameters.Add("vmuuid", vmOrTemplate.uuid);
            parameters.Add("sruuid", sr.uuid);
            parameters.Add("configuration", ConfigDriveTemplateText.Replace("\r\n", "\n"));

            return new ExecutePluginAction(Connection, vmOrTemplate.Home() ?? Helpers.GetMaster(Connection),
                                           "xscontainer", //plugin
                                           "create_config_drive", //function
                                           parameters,
                                           true); //hidefromlogs
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            vmOrTemplate = (VM)clone;

            if (Connection == null) // on the PropertiesDialog
                Connection = vmOrTemplate.Connection;

            GetCloudConfigParameters();
            ShowHideButtonsAndWarnings(false);
        }

        public bool ValidToSave
        {
            get { return true; }
        }

        public void ShowLocalValidationMessages()
        {
        }

        public void Cleanup()
        {
        }

        public bool HasChanged
        {
            get { return existingConfig != ConfigDriveTemplateText; }
        }

        #endregion
    }
}
