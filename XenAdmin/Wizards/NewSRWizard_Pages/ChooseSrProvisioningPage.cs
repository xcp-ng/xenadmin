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

using System.ComponentModel;
using System.Linq;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Wizards.NewSRWizard_Pages
{
    public partial class ChooseSrProvisioningPage : XenTabPage
    {
        public ChooseSrProvisioningPage()
        {
            InitializeComponent();
            Cluster_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(Cluster_CollectionChanged);
        }

        private readonly CollectionChangeEventHandler Cluster_CollectionChangedWithInvoke;

        #region XenTabPage overrides

        public override string Text => Messages.PROVISIONING;

        public override string PageTitle => Messages.CHOOSE_SR_PROVISIONING_PAGE_TITLE;

        public override string HelpID => "Provisioning";

        #endregion

        public bool IsGfs2 => radioButtonGfs2.Checked;

        private void RefreshPage()
        {
            var clusteringEnabled = Connection.Cache.Clusters.Any();
            var restrictGfs2 = Helpers.FeatureForbidden(Connection, Host.RestrictCorosync);
            var gfs2Allowed = !restrictGfs2 && clusteringEnabled;

            radioButtonGfs2.Enabled = labelGFS2.Enabled = gfs2Allowed;

            if (!gfs2Allowed)
            {
                radioButtonLvm.Checked = true;
            }

            tableLayoutInfo.Visible = !gfs2Allowed;
            labelInfo.Text = restrictGfs2
                ? Messages.GFS2_INCORRECT_POOL_LICENSE
                : Messages.GFS2_REQUIRES_CLUSTERING_ENABLED;
            linkLabelPoolProperties.Visible = !clusteringEnabled && !restrictGfs2;   
            
            RefreshWarnings();
        }

        private void RefreshWarnings()
        {
            if (radioButtonGfs2.Checked)
            {
                bool disabledMultipathExists = false;

                foreach (Host host in Connection.Cache.Hosts)
                {
                    if (!host.MultipathEnabled())
                    {
                        disabledMultipathExists = true;
                        break;
                    }
                }

                tableLayoutWarning.Visible = disabledMultipathExists;
                labelWarning.Text = Connection.Cache.Hosts.Length > 1
                    ? Messages.CHOOSE_SR_PROVISIONING_PAGE_MULTIPATHING_MANY
                    : Messages.CHOOSE_SR_PROVISIONING_PAGE_MULTIPATHING_ONE;
            }
            else
            {
                tableLayoutWarning.Visible = false;
            }
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            RefreshPage();

            foreach (var host in Connection.Cache.Hosts)
                host.PropertyChanged += Host_PropertyChanged;

            Connection.Cache.RegisterCollectionChanged<Cluster>(Cluster_CollectionChangedWithInvoke);
            Connection.Cache.RegisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            foreach (var host in Connection.Cache.Hosts)
                host.PropertyChanged -= Host_PropertyChanged;

            Connection.Cache.DeregisterCollectionChanged<Cluster>(Cluster_CollectionChangedWithInvoke);
            Connection.Cache.DeregisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);
        }

        private void Cluster_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.AssertOnEventThread();
            RefreshPage();
        }

        private void Host_CollectionChangedWithInvoke(object sender, CollectionChangeEventArgs e)
        {
            Host host = e.Element as Host;
            if (host == null)
                return;

            if (e.Action == CollectionChangeAction.Add)
                host.PropertyChanged += Host_PropertyChanged;
            else if (e.Action == CollectionChangeAction.Remove)
                host.PropertyChanged -= Host_PropertyChanged;
        }

        private void Host_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "multipathing":
                    RefreshWarnings();
                    break;
            }
        }


        private void linkLabelPoolProperties_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            var pool = Helpers.GetPoolOfOne(Connection);

            if (pool == null)
                return;

            using (PropertiesDialog propertiesDialog = new PropertiesDialog(pool))
            {
                propertiesDialog.SelectClusteringEditPage();
                propertiesDialog.ShowDialog(this);
            }
        }

        private void radioButtonGfs2_CheckedChanged(object sender, System.EventArgs e)
        {
            if (radioButtonGfs2.Checked)
                RefreshWarnings();
        }

        private void radioButtonLvm_CheckedChanged(object sender, System.EventArgs e)
        {
            if (radioButtonLvm.Checked)
                RefreshWarnings();
        }
    }
}
