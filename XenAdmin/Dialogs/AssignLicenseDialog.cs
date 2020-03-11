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
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public partial class AssignLicenseDialog : XenDialogBase
    {
        private readonly List<IXenObject> xos;
        private readonly Host.Edition currentEdition;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignLicenseDialog"/> class.
        /// </summary>
        public AssignLicenseDialog(IEnumerable<IXenObject> xos, String firstHost, String firstPort, Host.Edition firstEdition)
        {
            Util.ThrowIfEnumerableParameterNullOrEmpty(xos, "XenObjects");
            this.xos = new List<IXenObject>(xos);
            this.currentEdition = firstEdition;
            InitializeComponent();
            licenseServerNameTextBox.TextChanged += licenseServerPortTextBox_TextChanged;
            licenseServerPortTextBox.TextChanged += licenseServerNameTextBox_TextChanged;
            LoadLicenseOptions();
            UpdateButtonEnablement();

            // if all the hosts have the same license server details then populate the textboxes.
            List<Host> hosts = CreateHostsList();

            if (hosts[0].license_server.ContainsKey("address") && hosts[0].license_server.ContainsKey("port") &&
                hosts[0].license_server["address"] != "localhost")
            {
                if (hosts.TrueForAll(delegate(Host h)
                {
                    return h.license_server.ContainsKey("address") &&
                        h.license_server.ContainsKey("port") &&
                        h.license_server["address"] == hosts[0].license_server["address"] &&
                        h.license_server["port"] == hosts[0].license_server["port"];
                }))
                {
                    licenseServerPortTextBox.Text = hosts[0].license_server["port"];
                    licenseServerNameTextBox.Text = hosts[0].license_server["address"];
                }
            }
            else if ((!String.IsNullOrEmpty(firstHost)) && (!String.IsNullOrEmpty(firstPort)))
            {
                licenseServerPortTextBox.Text = firstPort;
                licenseServerNameTextBox.Text = firstHost;
            }
        }

        private List<Host> CreateHostsList()
        {
            List<Host> hosts = new List<Host>();
            foreach (IXenObject xenObject in xos)
            {
                if(xenObject is Pool)
                {
                    Pool pool = xenObject as Pool;
                    hosts.Add(xenObject.Connection.Resolve(pool.master));
                }
                if(xenObject is Host)
                    hosts.Add(xenObject as Host);
            }
            return hosts;
        }

        private void LoadLicenseOptions()
        {
            if (xos.TrueForAll(x => Helpers.NaplesOrGreater(x.Connection)))
            {
                perSocketRadioButton.Visible = false;
                xenDesktopEnterpriseRadioButton.Visible = false;
                enterprisePerSocketRadioButton.Checked = true;
                enterprisePerSocketRadioButton.Text = String.Format(Messages.LICENSE_EDITION_ENTERPRISE_PERSOCKET, 
                    xos.Sum(x => x.Connection.Cache.Hosts.Sum(h => h.CpuSockets()))); 
                enterprisePerUserRadioButton.Text = Messages.LICENSE_EDITION_ENTERPRISE_PERUSER;
                desktopPlusRadioButton.Text = Messages.LICENSE_EDITION_DESKTOP_PLUS;
                desktopRadioButton.Text = Messages.LICENSE_EDITION_DESKTOP;
                desktopCloudRadioButton.Visible = true;
                desktopCloudRadioButton.Text = Messages.LICENSE_EDITION_DESKTOP_CLOUD;
                standardPerSocketRadioButton.Text = String.Format(Messages.LICENSE_EDITION_STANDARD_PERSOCKET,
                    xos.Sum(x => x.Connection.Cache.Hosts.Sum(h => h.CpuSockets())));
            }
            else
            {
                perSocketRadioButton.Visible = false;
                xenDesktopEnterpriseRadioButton.Visible = false;
                enterprisePerSocketRadioButton.Checked = true;
                enterprisePerSocketRadioButton.Text = string.Format(Messages.LICENSE_EDITION_ENTERPRISE_PERSOCKET_LEGACY, 
                    xos.Sum(x => x.Connection.Cache.Hosts.Sum(h => h.CpuSockets())), BrandManager.LegacyProduct);
                enterprisePerUserRadioButton.Text = string.Format(Messages.LICENSE_EDITION_ENTERPRISE_PERUSER_LEGACY, BrandManager.LegacyProduct);
                desktopPlusRadioButton.Text = Messages.LICENSE_EDITION_DESKTOP_PLUS_LEGACY;
                desktopRadioButton.Text = Messages.LICENSE_EDITION_DESKTOP_LEGACY;
                desktopCloudRadioButton.Visible = xos.TrueForAll(x => Helpers.JuraOrGreater(x.Connection) || Helpers.HavanaOrGreater(x.Connection));
                desktopCloudRadioButton.Text = Messages.LICENSE_EDITION_DESKTOP_CLOUD_LEGACY;
                standardPerSocketRadioButton.Text = string.Format(Messages.LICENSE_EDITION_STANDARD_PERSOCKET_LEGACY,
                    xos.Sum(x => x.Connection.Cache.Hosts.Sum(h => h.CpuSockets())), BrandManager.LegacyProduct);
            }
        }

        private static void CheckRadioButtonIfVisible(RadioButton radioButton)
        {
            if (radioButton.Visible)
                radioButton.Checked = true;
        }

        private void CheckCurrentEdition(Host.Edition currentEdition)
        {
            switch (currentEdition)
            {
                case Host.Edition.XenDesktop:
                    CheckRadioButtonIfVisible(xenDesktopEnterpriseRadioButton);
                    break;
                case Host.Edition.PerSocket:
                    CheckRadioButtonIfVisible(perSocketRadioButton);
                    break;
                case Host.Edition.EnterprisePerSocket:
                    CheckRadioButtonIfVisible(enterprisePerSocketRadioButton);
                    break;
                case Host.Edition.EnterprisePerUser:
                    CheckRadioButtonIfVisible(enterprisePerUserRadioButton);
                    break;
                case Host.Edition.StandardPerSocket:
                    CheckRadioButtonIfVisible(standardPerSocketRadioButton);
                    break;
                case Host.Edition.Desktop:
                    CheckRadioButtonIfVisible(desktopRadioButton);
                    break;
                case Host.Edition.DesktopPlus:
                    CheckRadioButtonIfVisible(desktopPlusRadioButton);
                    break;
                case Host.Edition.DesktopCloud:
                    CheckRadioButtonIfVisible(desktopCloudRadioButton);
                    break;
            }
        }

        private Host.Edition GetCheckedEdition()
        {
            if (xenDesktopEnterpriseRadioButton.Checked)
                return Host.Edition.XenDesktop;

            if (perSocketRadioButton.Checked)
                return Host.Edition.PerSocket;

            if (enterprisePerSocketRadioButton.Checked)
                return Host.Edition.EnterprisePerSocket;

            if (enterprisePerUserRadioButton.Checked)
                return Host.Edition.EnterprisePerUser;

            if (desktopRadioButton.Checked)
                return Host.Edition.Desktop;

            if (desktopPlusRadioButton.Checked)
                return Host.Edition.DesktopPlus;

            if (desktopCloudRadioButton.Checked)
                return Host.Edition.DesktopCloud;

            return Host.Edition.StandardPerSocket;
        }

        private void okButton_Click(object sender, EventArgs e)
        {

            ApplyLicenseEditionCommand command = new ApplyLicenseEditionCommand(Program.MainWindow, xos, GetCheckedEdition(), licenseServerNameTextBox.Text, licenseServerPortTextBox.Text, this);

            command.Succedded += delegate
                                     {
                                         Program.Invoke(this, () =>
                                            {
                                                DialogResult = DialogResult.OK;
                                                Close();
                                            });
                                     };
            command.Execute();
        }

        private void licenseServerPortTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateButtonEnablement();
        }

        private void licenseServerNameTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateButtonEnablement();
        }

        private void UpdateButtonEnablement()
        {
            okButton.Enabled = licenseServerPortTextBox.Text.Length > 0 && licenseServerNameTextBox.Text.Length > 0 && Util.IsValidPort(licenseServerPortTextBox.Text);
        }

        private void AssignLicenseDialog_Shown(object sender, EventArgs e)
        {
            CheckCurrentEdition(currentEdition);
        }
    }
}