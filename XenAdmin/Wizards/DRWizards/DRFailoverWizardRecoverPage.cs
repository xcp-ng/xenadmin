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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using XenAdmin.Actions;
using XenAdmin.Actions.DR;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Properties;
using XenAPI;


namespace XenAdmin.Wizards.DRWizards
{
    public partial class DRFailoverWizardRecoverPage : XenTabPage
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event Action ReportStarted;
        public event Action<AsyncAction> ReportActionResultGot;
        public event Action<string, int, bool> ReportLineGot;

        public DRFailoverWizardRecoverPage()
        {
            InitializeComponent();
        }

        public override string PageTitle
        {
            get
            {
                switch (WizardType)
                {
                    case DRWizardType.Failback:
                        return String.Format(Messages.DR_WIZARD_RECOVERPAGE_TITLE_FAILBACK, Connection.Name);
                    case DRWizardType.Dryrun:
                        return String.Format(Messages.DR_WIZARD_RECOVERPAGE_TITLE_DRYRUN, Connection.Name);
                    default:
                        return Messages.DR_WIZARD_RECOVERPAGE_TITLE_FAILOVER;
                } 
            }
        }

        public override string Text
        {
            get { return Messages.DR_WIZARD_RECOVERPAGE_TEXT; }
        }

        public override string HelpID
        {
            get 
            {
                switch (WizardType)
                {
                    case DRWizardType.Failback:
                        return "Failback_Recover";
                    case DRWizardType.Dryrun:
                        return "Dryrun_Recover";
                    default:
                        return "Failover_Recover";
                }
            }
        }

        private static Bitmap animatedImage = Resources.ajax_loader;

        private void onFrameChanged(object sender, EventArgs e)
        {
            try
            {
                ImageAnimator.UpdateFrames();
                Program.BeginInvoke(dataGridView1, () => dataGridView1.InvalidateColumn(1));
            }
            catch (Exception)
            {
            }
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            ImageAnimator.StopAnimate(animatedImage, onFrameChanged);
            if (direction == PageLoadedDirection.Back)
            {
                actions.Clear();
            }
        }

        public override bool EnableNext()
        {
            SetupLabels();
            return (progressBar1.Value == 100);
        }

        public override bool EnablePrevious()
        {
            return false;
        }

        private void SetupLabels()
        {
            switch (WizardType)
            {
                case DRWizardType.Failback:
                    labelTitle.Text = progressBar1.Value == 100 
                        ? String.Format(Messages.DR_WIZARD_RECOVERPAGE_COMPLETE_FAILBACK, Connection.Name) 
                        : Messages.DR_WIZARD_RECOVERPAGE_IN_PROGRESS_FAILBACK; 
                    labelContinue.Text = Messages.DR_WIZARD_RECOVERPAGE_CONTINUE_FAILBACK;
                    break;
                case DRWizardType.Dryrun:
                    labelTitle.Text = progressBar1.Value == 100
                        ? String.Format(Messages.DR_WIZARD_RECOVERPAGE_COMPLETE_DRYRUN, Connection.Name)  
                        : Messages.DR_WIZARD_RECOVERPAGE_IN_PROGRESS_DRYRUN;
                    labelContinue.Text = Messages.DR_WIZARD_RECOVERPAGE_CONTINUE_DRYRUN;
                    break;
                default:
                    labelTitle.Text = progressBar1.Value == 100 
                        ? Messages.DR_WIZARD_RECOVERPAGE_COMPLETE_FAILOVER 
                        : Messages.DR_WIZARD_RECOVERPAGE_IN_PROGRESS_FAILOVER;
                    labelContinue.Text = Messages.DR_WIZARD_RECOVERPAGE_CONTINUE_FAILOVER;
                    break;
            }
            labelContinue.Visible = progressBar1.Value == 100;
        }

        public DRWizardType WizardType { private get; set; }

        private Dictionary<VDI, List<AsyncAction>> actions = new Dictionary<VDI, List<AsyncAction>>();
        private int objectsToBeRecovered;

        private StartActionAfterRecovery startActionAfterRecovery;
        public StartActionAfterRecovery StartActionAfterRecovery { set { startActionAfterRecovery = value; } }

        public Dictionary<XenRef<VDI>, PoolMetadata> SelectedPoolMetadata { private get; set; }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction == PageLoadedDirection.Back || actions.Count > 0)
                return;

            ImageAnimator.Animate(animatedImage, onFrameChanged);

            if (ReportStarted != null)
                ReportStarted();
            
            OnPageUpdated();

            dataGridView1.Rows.Clear();
            actions.Clear();
            objectsToBeRecovered = 0;

            // add "recovery" tasks
            foreach (var poolMetadata in SelectedPoolMetadata.Values)
            {
                actions.Add(poolMetadata.Vdi, CreateSubActionsFor(poolMetadata));
            }

            // add a row for "Start VMs and Appliances" task, if required

            if (startActionAfterRecovery != StartActionAfterRecovery.None)
                dataGridView1.Rows.Add(new DataGridViewRowRecover(Messages.ACTION_START_VMS_AND_APPLIANCES_TITLE));

            labelOverallProgress.Text = string.Format(Messages.DR_WIZARD_RECOVERPAGE_OVERALL_PROGRESS, 0, dataGridView1.Rows.Count);

            RecoverNextPool();
        }

        List<AsyncAction> CreateSubActionsFor(PoolMetadata poolMetadata)
        {
            log.DebugFormat("Generating recovery actions from pool {0} (VDI {1})", poolMetadata.Pool.Name(), poolMetadata.Vdi.Name());
                
            List<AsyncAction> subActions = new List<AsyncAction>();

            VdiOpenDatabaseAction openDatabaseAction = new VdiOpenDatabaseAction(Connection, poolMetadata.Vdi);
            openDatabaseAction.Completed += OpenDatabaseActionCompleted;

            subActions.Add(openDatabaseAction);

            foreach (var vmAppliance in poolMetadata.VmAppliances.Values)
            {
                DrRecoverAction drRecoverAction = new DrRecoverAction(Connection, vmAppliance);
                drRecoverAction.Completed += SingleRecoverActionCompleted;
                drRecoverAction.Changed += SingleRecoverActionChanged;
                subActions.Add(drRecoverAction);
                dataGridView1.Rows.Add(new DataGridViewRowRecover(vmAppliance));
                objectsToBeRecovered++;
            }

            foreach (var vm in poolMetadata.Vms.Values)
            {
                if (vm.IsAssignedToVapp())
                {
                    //VM included in an appliance
                    continue;
                }
                DrRecoverAction drRecoverAction = new DrRecoverAction(Connection, vm);
                drRecoverAction.Completed += SingleRecoverActionCompleted;
                drRecoverAction.Changed += SingleRecoverActionChanged;
                subActions.Add(drRecoverAction);
                dataGridView1.Rows.Add(new DataGridViewRowRecover(vm));
                objectsToBeRecovered++;
            }
            log.DebugFormat("Done - {0} actions generated", subActions.Count);
            
            return subActions;
        }

        internal List<string> RecoveredVmsUuids = new List<string>();
        internal List<string> RecoveredVmAppliancesUuids = new List<string>();

        private void SingleRecoverActionCompleted(ActionBase sender)
        {
            DrRecoverAction senderAction = (DrRecoverAction)sender;
            senderAction.Completed -= SingleRecoverActionCompleted;
            senderAction.Changed -= SingleRecoverActionChanged;

            objectsToBeRecovered--;

            Program.BeginInvoke(this, () =>
            {
                progressBar1.Value = progressBar1.Value < 100
                                        ? progressBar1.Value + (100 / dataGridView1.Rows.Count)
                                        : 100;
                var row = FindRow(senderAction.XenObject);
                if (row != null)
                {
                    if (senderAction.Succeeded)
                        row.UpdateStatus(RecoverState.Recovered, Messages.DR_WIZARD_RECOVERPAGE_STATUS_COMPLETED);
                    else
                        row.UpdateStatus(RecoverState.Error, Messages.DR_WIZARD_RECOVERPAGE_STATUS_FAILED, senderAction.Exception.Message);
                    labelOverallProgress.Text = string.Format(Messages.DR_WIZARD_RECOVERPAGE_OVERALL_PROGRESS,
                                                              row.Index + 1, dataGridView1.Rows.Count);
                }
            });

            if (senderAction.Succeeded)
            {
                if (senderAction.XenObject is VM)
                    RecoveredVmsUuids.Add((senderAction.XenObject as VM).uuid);
                else if (senderAction.XenObject is VM_appliance)
                    RecoveredVmAppliancesUuids.Add((senderAction.XenObject as VM_appliance).uuid);
            }

            if (ReportActionResultGot != null)
                ReportActionResultGot(senderAction);
        }

        private void SingleRecoverActionChanged(ActionBase sender)
        {
            DrRecoverAction senderAction = (DrRecoverAction)sender;

            if (senderAction.IsCompleted)
                return;

            Program.BeginInvoke(this, () =>
            {
                var row = FindRow(senderAction.XenObject);
                if (row != null && !senderAction.IsCompleted)
                    row.UpdateStatus(RecoverState.Recovering, Messages.DR_WIZARD_RECOVERPAGE_STATUS_WORKING, senderAction.Title);
            });
        }


        private MultipleAction multipleRecoverAction;
        private Session metadataSession;

        private void OpenDatabaseActionCompleted(ActionBase sender)
        {
            VdiOpenDatabaseAction senderAction = (VdiOpenDatabaseAction)sender;
            senderAction.Completed -= OpenDatabaseActionCompleted;

            log.DebugFormat("Metadata database open ({0}). Now start recovering", senderAction.Vdi.Name());

            metadataSession = senderAction.MetadataSession;
            if (metadataSession == null)
                return;

            // assign metadata session to all recover actions
            List<AsyncAction> recoverSubActions = new List<AsyncAction>();
            foreach (var action in actions[senderAction.Vdi])
            {
                if (action is DrRecoverAction)
                {
                    ((DrRecoverAction)action).MetadataSession = metadataSession;
                    recoverSubActions.Add(action);
                }
            }

            multipleRecoverAction = new MultipleAction(Connection,
                String.Format(Messages.DR_WIZARD_RECOVERPAGE_RECOVER_FROM, senderAction.Vdi.Name()),
                String.Format(Messages.DR_WIZARD_RECOVERPAGE_RECOVERING_FROM, senderAction.Vdi.Name()), 
                Messages.COMPLETED, 
                recoverSubActions);
            multipleRecoverAction.Completed += MultipleRecoverActionCompleted;
            multipleRecoverAction.RunAsync();
        }

        private void MultipleRecoverActionCompleted(ActionBase sender)
        {
            MultipleAction senderAction = (MultipleAction)sender;
            senderAction.Completed -= MultipleRecoverActionCompleted;

            log.Debug("Finished recovery. Close metadata database");
            // logout from metadata session
            metadataSession.logout();

            Program.BeginInvoke(this, () =>
            {
                if (objectsToBeRecovered == 0) // finished recovering, now start VMs, if required
                {
                    switch (startActionAfterRecovery)
                    {
                        case StartActionAfterRecovery.Start :
                            StartRecoveredVMs(false);
                            break;
                        case StartActionAfterRecovery.StartPaused:
                            StartRecoveredVMs(true);
                            break;
                        default:
                            progressBar1.Value = 100;
                            OnPageUpdated();
                            if (ReportLineGot != null)
                                ReportLineGot(labelTitle.Text, 0, true);
                            break;
                    }
                }
                else // recover next pool
                    RecoverNextPool();
            });
        }

        private void RecoverNextPool()
        {
            foreach (var actionList in actions.Values)
            {
                bool startRecovery = false;
                foreach (var action in actionList)
                {
                    VdiOpenDatabaseAction openDatabaseAction = action as VdiOpenDatabaseAction;
                    if (openDatabaseAction != null && !openDatabaseAction.IsCompleted)
                    {
                        startRecovery = true;
                        log.DebugFormat("Open metadata database ({0})", openDatabaseAction.Vdi.Name());
                        openDatabaseAction.RunAsync();
                        break;
                    }
                }
                if (startRecovery)
                    break; // start recovery of first "unrecovered" pool (unrecovered = !openDatabaseAction.IsCompleted)
            }
        }

        private DataGridViewRowRecover FindRow(IXenObject xenObject)
        {
            foreach (DataGridViewRowRecover row in dataGridView1.Rows)
            {
                if (row.XenObject == xenObject)
                    return row;
            }
            throw new Exception("Row not found");
        }

        private void StartRecoveredVMs(bool paused)
        {
            List<VM> vmsToStart = new List<VM>();
            foreach (var uuid in RecoveredVmsUuids)
            {
                foreach (VM vm in Connection.Cache.VMs)
                {
                    if (vm.uuid == uuid)
                        vmsToStart.Add(vm);
                }
            }

            List<VM_appliance> vmAppliancesToStart = new List<VM_appliance>();
            foreach (var uuid in RecoveredVmAppliancesUuids)
            {
                foreach (VM_appliance vmAppliance in Connection.Cache.VM_appliances)
                {
                    if (vmAppliance.uuid == uuid)
                        vmAppliancesToStart.Add(vmAppliance);
                }
            }

            var action = new StartVMsAndAppliancesAction(Connection, vmsToStart, vmAppliancesToStart, VMOperationCommand.WarningDialogHAInvalidConfig, VMOperationCommand.StartDiagnosisForm, paused);
            if (action != null)
            {
                action.Completed += StartVMsActionCompleted;
                action.Changed += StartVMsActionChanged;
                action.RunAsync();
            }
        }

        private void StartVMsActionChanged(ActionBase sender)
        {
            StartVMsAndAppliancesAction senderAction = (StartVMsAndAppliancesAction)sender;

            if (senderAction.IsCompleted)
                return;

            Program.BeginInvoke(this, () =>
            {
              var row = dataGridView1.Rows[dataGridView1.RowCount - 1] as DataGridViewRowRecover; //last row is "Start VMs and Appliances" row
              if (row != null && !senderAction.IsCompleted)
                  row.UpdateStatus(RecoverState.Recovering, Messages.DR_WIZARD_RECOVERPAGE_STATUS_WORKING, senderAction.Description);
            });
        }

        private void StartVMsActionCompleted(ActionBase sender)
        {
            StartVMsAndAppliancesAction senderAction = (StartVMsAndAppliancesAction)sender;
            senderAction.Completed -= StartVMsActionCompleted;
            senderAction.Changed -= StartVMsActionChanged;

            if (ReportActionResultGot != null)
                ReportActionResultGot(senderAction);

            log.Debug("Finished starting VMs and appliances");

            Program.BeginInvoke(this, () =>
            {
                progressBar1.Value = 100;
                var row = dataGridView1.Rows[dataGridView1.RowCount - 1] as DataGridViewRowRecover; //last row is "Start VMs and Appliances" row
                if (row != null)
                {
                    if (senderAction.Succeeded)
                        row.UpdateStatus(RecoverState.Recovered, Messages.DR_WIZARD_RECOVERPAGE_STATUS_COMPLETED);
                    else
                        row.UpdateStatus(RecoverState.Error, Messages.DR_WIZARD_RECOVERPAGE_STATUS_FAILED, senderAction.Exception.Message);
                    labelOverallProgress.Text = string.Format(Messages.DR_WIZARD_RECOVERPAGE_OVERALL_PROGRESS,
                                                              row.Index + 1, dataGridView1.Rows.Count);
                }
                OnPageUpdated();
                if (ReportLineGot != null)
                    ReportLineGot(labelTitle.Text, 0, true);
            });
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.ViewLogFiles();
        }

        public class DataGridViewRowRecover : DataGridViewRow
        {
            public readonly IXenObject XenObject; // it can be VM or VM_appliance
            private DataGridViewImageCell imageCell = new DataGridViewImageCell();
            private DataGridViewTextBoxCell taskCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell statusCell = new DataGridViewTextBoxCell();

            private DataGridViewRowRecover()
            {
                this.Cells.Add(taskCell);
                this.Cells.Add(imageCell);
                this.Cells.Add(statusCell);
            }

            public DataGridViewRowRecover(IXenObject xenObject)
                : this()
            {
                XenObject = xenObject;
                taskCell.Value = XenObject is VM
                                     ? string.Format(Messages.ACTION_DR_RECOVER_VM_TITLE, XenObject.Name())
                                     : string.Format(Messages.ACTION_DR_RECOVER_APPLIANCE_TITLE, XenObject.Name());
                UpdateStatus(RecoverState.NotRecovered, Messages.DR_WIZARD_RECOVERPAGE_STATUS_PENDING);
            }

            public DataGridViewRowRecover(string title)
                : this()
            {
                taskCell.Value = title;
                UpdateStatus(RecoverState.NotRecovered, Messages.DR_WIZARD_RECOVERPAGE_STATUS_PENDING);
            }

            public void UpdateStatus(RecoverState state, string value)
            {
                UpdateStatus(state, value, "");
            }

            public void UpdateStatus(RecoverState state, string value, string toolTipText)
            {
                switch (state)
                {
                    case RecoverState.Recovered:
                        imageCell.Value = Resources._000_Tick_h32bit_16;
                        break;
                    case RecoverState.Recovering:
                        imageCell.Value = animatedImage;
                        break;
                    case RecoverState.Error:
                        imageCell.Value = Resources._000_Abort_h32bit_16;
                        break;
                    case RecoverState.NotRecovered:
                        imageCell.Value = new Bitmap(1, 1);
                        break;
                }
                statusCell.Value = value;
                statusCell.ToolTipText = toolTipText;
            }
        }

        public enum RecoverState
        {
            NotRecovered,
            Recovered,
            Recovering,
            Error
        }
    }
}
