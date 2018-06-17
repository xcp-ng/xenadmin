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
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Controls;



namespace XenAdmin.Wizards.NewNetworkWizard_Pages
{
    public partial class NetWTypeSelect : XenTabPage
    {
        public NetWTypeSelect()
        {
            InitializeComponent();
            this.rbtnExternalNetwork.Checked = true;
        }

        public override string Text { get { return Messages.NETW_TYPE_SELECT_TEXT; } }

        public override string PageTitle { get { return Messages.NETW_TYPE_SELECT_TITLE; } }

        public override void PopulatePage()
        {
            Update(Connection);
        }

        public NetworkTypes SelectedNetworkType
        {
            get
            {
                return rbtnBondedNetwork.Checked
                           ? NetworkTypes.Bonded
                           : rbtnExternalNetwork.Checked
                                 ? NetworkTypes.External
                                 : rbtnCHIN.Checked
                                       ? NetworkTypes.CHIN
                                       : rbtnSriov.Checked
                                           ? NetworkTypes.SRIOV
                                           : NetworkTypes.Internal;
            }
        }

        private void Update(IXenConnection connection)
        {
            Host master = Helpers.GetMaster(connection);
            if (master == null)
                return;

            Pool pool = Helpers.GetPoolOfOne(connection);
            labelCHIN.Visible = rbtnCHIN.Visible = !HiddenFeatures.CrossServerPrivateNetworkHidden;
            if (!pool.vSwitchController())
            {
                rbtnCHIN.Checked = false;
                rbtnCHIN.Enabled = labelCHIN.Enabled = false;

                labelWarningChinOption.Text = 
                    Helpers.FeatureForbidden(connection, Host.RestrictVSwitchController) ?
                    String.Format(Messages.FEATURE_DISABLED, Messages.CHIN) :
                    Messages.CHINS_NEED_VSWITCHCONTROLLER;

                iconWarningChinOption.Visible = labelWarningChinOption.Visible = !HiddenFeatures.CrossServerPrivateNetworkHidden;

                rbtnExternalNetwork.Checked = true;
            }
            else
            {
                rbtnCHIN.Enabled = labelCHIN.Enabled = true;
                iconWarningChinOption.Visible = labelWarningChinOption.Visible = false;
            }

            bool hasNicCanEnableSriov = pool.Connection.Cache.PIFs.Any(pif => pif.IsPhysical() && pif.SriovCapable() && !pif.IsSriovPhysicalPIF());
            bool sriovFeatureForbidden = Helpers.FeatureForbidden(connection, Host.RestrictSriovNetwork);

            if (!Helpers.KolkataOrGreater(pool.Connection))
            {
                iconWarningSriovOption.Visible = labelWarningSriovOption.Visible = false;
                rbtnSriov.Visible = labelSriov.Visible = false;
            }
            else if (Helpers.FeatureForbidden(pool.Connection, Host.SriovNetworkDisabled) ||
                sriovFeatureForbidden || !pool.HasSriovNic() || !hasNicCanEnableSriov)
            {
                rbtnSriov.Checked = false;
                rbtnSriov.Enabled = labelSriov.Enabled = false;

                labelWarningSriovOption.Text =
                    Helpers.FeatureForbidden(pool.Connection, Host.SriovNetworkDisabled)
                        ? String.Format(Messages.FEATURE_EXPERIMENTAL, Messages.NETWORK_SRIOV)
                        : sriovFeatureForbidden
                            ? String.Format(Messages.FEATURE_DISABLED, Messages.NETWORK_SRIOV)
                            : pool.HasSriovNic()
                                ? Messages.NICS_ARE_SRIOV_ENABLED
                                : Messages.SRIOV_NEED_NICSUPPORT;

                iconWarningSriovOption.Visible = labelWarningSriovOption.Visible = true;
            }
            else
            {
                rbtnSriov.Enabled = labelCHIN.Enabled = true;
                iconWarningSriovOption.Visible = labelWarningSriovOption.Visible = false;
            }
        }
    }
}
