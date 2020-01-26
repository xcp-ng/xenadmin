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
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAPI;

using XenAdmin.Actions.DR;

namespace XenAdmin.Wizards.DRWizards
{
    public partial class DRFailoverWizardAppliancesPage : XenTabPage
    {
        private Pool _pool;
        public Pool Pool
        {
            get { return _pool; }
            set
            {
                _pool = value;
                if (_pool != null)
                    labelPageDescription.Text = string.Format(labelPageDescription.Text, Pool.Name());
            }
        }

        public DRFailoverWizardAppliancesPage()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get { return Messages.DR_WIZARD_APPLIANCESPAGE_TEXT; }
        }

        public override string PageTitle
        {
            get
            {
                switch (WizardType)
                {
                    case DRWizardType.Failback:
                        return Messages.DR_WIZARD_APPLIANCESPAGE_TITLE_FAILBACK;
                    default:
                        return Messages.DR_WIZARD_APPLIANCESPAGE_TITLE_FAILOVER;
                } 
            }
        }

        public override string HelpID
        {
            get 
            {
                switch (WizardType)
                {
                    case DRWizardType.Failback:
                        return "Failback_vApps";
                    case DRWizardType.Dryrun:
                        return "Dryrun_vApps";
                    default:
                        return "Failover_vApps";
                }
            }
        }

        public override bool EnableNext()
        {
            int checkedItems = ApplianceTreeView.CheckedItems().Count;
            buttonClearAll.Enabled = checkedItems > 0;
            buttonSelectAll.Enabled = checkedItems < ApplianceTreeView.Items.Count;
            return checkedItems > 0;
        }

        private Dictionary<XenRef<VDI>, PoolMetadata> allPoolMetadata;
        public Dictionary<XenRef<VDI>, PoolMetadata> AllPoolMetadata { set { allPoolMetadata = value; } }

        private Dictionary<XenRef<VDI>, PoolMetadata> selectedPoolMetadata = new Dictionary<XenRef<VDI>, PoolMetadata>();

        public DRWizardType WizardType { private get; set; }

        public Dictionary<XenRef<VDI>, PoolMetadata> SelectedPoolMetadata
        {
            get { return selectedPoolMetadata; }
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction == PageLoadedDirection.Forward)
            {
                SetupLabels();
                PopulateTreeView();
            }
        }

        void SetupLabels()
        {
            switch (WizardType)
            {
                case DRWizardType.Failback:
                    labelPageDescription.Text = String.Format(Messages.DR_WIZARD_APPLIANCESPAGE_DESCRIPTION_FAILBACK,
                                                              Pool.Name());
                    radioButtonStart.Text = Messages.DR_WIZARD_APPLIANCESPAGE_FAILBACK_START;
                    radioButtonPaused.Text = Messages.DR_WIZARD_APPLIANCESPAGE_FAILBACK_STARTPAUSED;
                    radioButtonDoNotStart.Text = Messages.DR_WIZARD_APPLIANCESPAGE_FAILBACK_DONOTSTART;
                    break;
                default:
                    labelPageDescription.Text = Messages.DR_WIZARD_APPLIANCESPAGE_DESCRIPTION_FAILOVER;
                    radioButtonStart.Text = Messages.DR_WIZARD_APPLIANCESPAGE_FAILOVER_START;
                    radioButtonPaused.Text = Messages.DR_WIZARD_APPLIANCESPAGE_FAILOVER_STARTPAUSED;
                    radioButtonDoNotStart.Text = Messages.DR_WIZARD_APPLIANCESPAGE_FAILOVER_DONOTSTART;
                    break;
            }

            var dryrun = WizardType == DRWizardType.Dryrun;
            radioButtonPaused.Visible = dryrun;
            radioButtonPaused.Checked = dryrun;
            radioButtonStart.Visible = !dryrun;
        }

        private void PopulateTreeView(PoolMetadata poolMetadata, CustomTreeNode parentNode)
        {
            ApplianceTreeView.BeginUpdate();
            ApplianceTreeView.Cursor = Cursors.WaitCursor;
            try
            {
                if (poolMetadata.VmAppliances != null && poolMetadata.VmAppliances.Count > 0)
                {
                    foreach (var applianceItem in poolMetadata.VmAppliances)
                    {
                        ApplianceNode applianceNode = new ApplianceNode(applianceItem.Key, applianceItem.Value);
                        if (parentNode != null)
                        {
                            ApplianceTreeView.AddChildNode(parentNode, applianceNode);
                            applianceNode.Enabled = parentNode.Enabled;
                        }
                        else
                            ApplianceTreeView.AddNode(applianceNode);

                        foreach (XenRef<VM> vmRef in applianceItem.Value.VMs)
                        {
                            if (poolMetadata.Vms.ContainsKey(vmRef))
                            {
                                VmNode vmNode = new VmNode(vmRef, poolMetadata.Vms[vmRef], false);
                                ApplianceTreeView.AddChildNode(applianceNode, vmNode);
                                vmNode.Enabled = applianceNode.Enabled;
                            }
                        }
                        applianceNode.Expanded = false;
                    }
                }

                if (poolMetadata.Vms != null || poolMetadata.Vms.Count > 0)
                {
                    foreach (var vmItem in poolMetadata.Vms)
                    {
                        if (vmItem.Value.IsAssignedToVapp())
                        {
                            //VM included in an appliance
                            continue;
                        }
                        VmNode vmNode = new VmNode(vmItem.Key, vmItem.Value, true);
                        if (parentNode != null)
                        {
                            ApplianceTreeView.AddChildNode(parentNode, vmNode);
                            vmNode.Enabled = parentNode.Enabled;
                        }
                        else
                            ApplianceTreeView.AddNode(vmNode);
                    }
                }
            }
            finally
            {
                ApplianceTreeView.EndUpdate();
                ApplianceTreeView.Cursor = Cursors.Default;
            }
        }

        private void PopulateTreeView()
        {
            ApplianceTreeView.BeginUpdate();
            ApplianceTreeView.Cursor = Cursors.WaitCursor;
            try
            {
                ApplianceTreeView.ClearAllNodes();
                foreach (var vdiRef in allPoolMetadata.Keys)
                {
                    PoolMetadata poolMetadata = allPoolMetadata[vdiRef];
                    //if (!String.IsNullOrEmpty(poolMetadata.Pool.Name))
                    {
                        string description = poolMetadata.Pool.uuid == Pool.uuid ? Messages.DR_WIZARD_APPLIANCESPAGE_CURRENT_POOL : string.Empty;
                        PoolNode poolNode = new PoolNode(vdiRef, poolMetadata.Pool.Name(), description);
                        ApplianceTreeView.AddNode(poolNode);
                        poolNode.Enabled = (poolMetadata.Pool.uuid != Pool.uuid);
                        PopulateTreeView(poolMetadata, poolNode);
                    }
                }
            }
            finally
            {
                ApplianceTreeView.EndUpdate();
                ApplianceTreeView.Cursor = Cursors.Default;
            }
        }

        private void ApplianceTreeView_ItemCheckChanged(object sender, EventArgs e)
        {
            CustomTreeNode node = sender as CustomTreeNode;
        
            var poolNode = node.ParentNode as PoolNode;
            if (poolNode == null)
                return;

            XenRef<VDI> vdiRef = poolNode.VdiRef;

            // add pool if needed
            if (node.State == CheckState.Checked && !selectedPoolMetadata.ContainsKey(vdiRef))
            {
                selectedPoolMetadata.Add(vdiRef, new PoolMetadata(allPoolMetadata[vdiRef].Pool, allPoolMetadata[vdiRef].Vdi));
            }
            if (!selectedPoolMetadata.ContainsKey(vdiRef))
                return;

            DoOnNodeStateChanged(node, vdiRef);
            
            // remove pool if no appliance or vm selected
            if (selectedPoolMetadata[vdiRef].VmAppliances.Count == 0 && selectedPoolMetadata[vdiRef].Vms.Count == 0)
            {
                selectedPoolMetadata.Remove(vdiRef);
            }

            OnPageUpdated();
        }

        private void DoOnNodeStateChanged(CustomTreeNode node, XenRef<VDI> vdiRef)
        {
            var applianceNode = node as ApplianceNode;
            if (applianceNode != null)
            {
                switch (applianceNode.State)
                {
                    case CheckState.Checked:
                        if (!selectedPoolMetadata[vdiRef].VmAppliances.ContainsKey(applianceNode.ApplianceRef))
                            selectedPoolMetadata[vdiRef].VmAppliances.Add(applianceNode.ApplianceRef, applianceNode.Appliance);
                        break;
                    case CheckState.Unchecked:
                        if (selectedPoolMetadata[vdiRef].VmAppliances.ContainsKey(applianceNode.ApplianceRef))
                            selectedPoolMetadata[vdiRef].VmAppliances.Remove(applianceNode.ApplianceRef);
                        break;
                }

                foreach (var childNode in applianceNode.ChildNodes)
                {
                    childNode.State = applianceNode.State;
                    DoOnNodeStateChanged(childNode, vdiRef);
                }

                return;
            }

            var vmNode = node as VmNode;
            if (vmNode != null)
            {
                switch (vmNode.State)
                {
                    case CheckState.Checked:
                        if (!selectedPoolMetadata[vdiRef].Vms.ContainsKey(vmNode.VmRef))
                            selectedPoolMetadata[vdiRef].Vms.Add(vmNode.VmRef, vmNode.Vm);
                        break;
                    case CheckState.Unchecked:
                        if (selectedPoolMetadata[vdiRef].Vms.ContainsKey(vmNode.VmRef))
                            selectedPoolMetadata[vdiRef].Vms.Remove(vmNode.VmRef);
                        break;
                }
            }
        }

        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            foreach (CustomTreeNode node in ApplianceTreeView.Items)
            {
                if (!node.HideCheckbox)
                {
                    node.State = CheckState.Checked;
                    ApplianceTreeView_ItemCheckChanged(node, new EventArgs());
                }
            }
            ApplianceTreeView.Refresh();
        }

        private void buttonClearAll_Click(object sender, EventArgs e)
        {
            foreach (CustomTreeNode node in ApplianceTreeView.Items)
            {
                if (!node.HideCheckbox)
                {
                    node.State = CheckState.Unchecked;
                    ApplianceTreeView_ItemCheckChanged(node, new EventArgs());
                }
            }
            ApplianceTreeView.Refresh();
        }

        public StartActionAfterRecovery StartActionAfterRecovery
        {
            get
            {
                if (radioButtonStart.Checked)
                    return StartActionAfterRecovery.Start;
                if (radioButtonPaused.Checked)
                    return StartActionAfterRecovery.StartPaused;
                return StartActionAfterRecovery.None;
            }
        }

        public string GetSelectedPowerStateDescription()
        {
            switch (StartActionAfterRecovery)
            {
                case StartActionAfterRecovery.Start:
                    return WizardType == DRWizardType.Failback 
                        ? Messages.DR_WIZARD_APPLIANCESPAGE_FAILBACK_START_NOAMP
                        : Messages.DR_WIZARD_APPLIANCESPAGE_FAILOVER_START_NOAMP;
                case StartActionAfterRecovery.StartPaused:
                    return WizardType == DRWizardType.Failback
                        ? Messages.DR_WIZARD_APPLIANCESPAGE_FAILBACK_STARTPAUSED_NOAMP
                        : Messages.DR_WIZARD_APPLIANCESPAGE_FAILOVER_STARTPAUSED_NOAMP;
                default:
                    return WizardType == DRWizardType.Failback
                        ? Messages.DR_WIZARD_APPLIANCESPAGE_FAILBACK_DONOTSTART_NOAMP
                        : Messages.DR_WIZARD_APPLIANCESPAGE_FAILOVER_DONOTSTART_NOAMP;
            }
        }
    }

    class PoolNode : CustomTreeNode
    {
        public string PoolName; 
        public XenRef<VDI> VdiRef;
        private string description;

        public PoolNode(XenRef<VDI> vdiRef, string poolName, string description)
            : base(true)
        {
            PoolName = poolName;
            VdiRef = vdiRef;
            this.description = description;
            Update();
        }

        private void Update()
        {
            Text = PoolName;
            Image = Images.GetImage16For(Icons.Pool);

            Description = description;
            Enabled = true;
            HideCheckbox = true;
            Expanded = true;
        }
    }

    class ApplianceNode : CustomTreeNode
    {
        public XenRef<VM_appliance> ApplianceRef;
        public VM_appliance Appliance;

        public ApplianceNode(XenRef<VM_appliance> applianceRef, VM_appliance appliance)
            : base(true)
        {
            ApplianceRef = applianceRef;
            Appliance = appliance;
            Update();
        }

        private void Update()
        {
            Text = Appliance.Name();
            Image = Images.GetImage16For(Appliance);

            Description = "";
            Enabled = true;
            HideCheckbox = false;
        }
    }

    class VmNode : CustomTreeNode, IEquatable<VmNode>
    {
        public XenRef<VM> VmRef;
        public VM Vm;

        public VmNode(XenRef<VM> vmRef, VM vm, bool selectable)
            : base(selectable)
        {
            VmRef = vmRef;
            Vm = vm;
            Update();
        }

        private void Update()
        {
            Text = Vm.Name();
            Image = Images.GetImage16For(Icons.VM);
            Description = "";
            Enabled = true;
            HideCheckbox = !Selectable;
        }

        public bool Equals(VmNode other)
        {
            return VmRef == other.VmRef;
        }
    }

    public enum StartActionAfterRecovery { Start, StartPaused, None }
}
