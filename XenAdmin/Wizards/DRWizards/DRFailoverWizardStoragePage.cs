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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Wizards.NewSRWizard_Pages;
using XenAPI;
using XenAdmin.Controls;

using XenAdmin.Actions.DR;

namespace XenAdmin.Wizards.DRWizards
{
    public partial class DRFailoverWizardStoragePage : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public event Action<string> NewDrTaskIntroduced;
        private List<SR> _availableSRs = new List<SR>();

        public DRFailoverWizardStoragePage()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides

        public override string Text
        {
            get { return Messages.DR_WIZARD_STORAGEPAGE_TEXT; }
        }

        public override string PageTitle
        {
            get { return Messages.DR_WIZARD_STORAGEPAGE_TITLE; }
        }

        public override string HelpID
        {
            get
            {
                switch (WizardType)
                {
                    case DRWizardType.Failback:
                        return "Failback_Storage";
                    case DRWizardType.Dryrun:
                        return "Dryrun_Storage";
                    default:
                        return "Failover_Storage";
                }
            }
        }

        public override bool EnableNext()
        {
            if (_worker.IsBusy)
                return false;

            buttonClearAll.Enabled = SelectedSRsUuids.Count > 0;
            buttonSelectAll.Enabled = SelectedSRsUuids.Count < dataGridViewSRs.Rows.OfType<SrRow>().Count();
            return SelectedSRsUuids.Count > 0;
        }

        public override bool EnablePrevious()
        {
            return !_worker.IsBusy;
        }

        private readonly ConnectionLostDialogLauncher cldl = new ConnectionLostDialogLauncher();

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            _worker.CancelAsync();

            if (direction == PageLoadedDirection.Forward)
            {
                IntroduceSRs();
                LoadMetadata();
            }
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction == PageLoadedDirection.Forward)
                SetupLabels();
        }

        public override void PopulatePage()
        {
            dataGridViewSRs.Rows.Clear();
            SelectedSRsUuids.Clear();
            _availableSRs.Clear();
            OnPageUpdated();
            spinnerIcon1.StartSpinning();
            _worker.RunWorkerAsync();
        }

        public override void PageCancelled()
        {
           _worker.CancelAsync();
        }

        #endregion

        #region Initial page setup

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Pool currentPool = Helpers.GetPoolOfOne(Connection);
            if (currentPool == null)
                return;

            var srs = new List<SR>(Connection.Cache.SRs);
            for (int i = 0; i < srs.Count; i++)
            {
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                var sr = srs[i];
                SR checkedSr = SR.SupportsDatabaseReplication(sr.Connection, sr) ? sr : null;
                int percentage = (i + 1)*100/srs.Count;
                _worker.ReportProgress(percentage, checkedSr);
            }
        }

        private void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SR sr = e.UserState as SR;
            if (sr != null)
                _availableSRs.Add(sr);
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            spinnerIcon1.StopSpinning();
            try
            {
                dataGridViewSRs.SuspendLayout();

                foreach (SR sr in _availableSRs)
                {
                    var vdis = sr.Connection.ResolveAll(sr.VDIs);
                    bool poolMetadataDetected = vdis.Any(vdi => vdi.type == vdi_type.metadata);

                    SrRow row;
                    if (!FindRowByUuid(sr.uuid, out row))
                    {
                        row = new SrRow(sr, poolMetadataDetected, SelectedSRsUuids.Contains(sr.uuid));
                        dataGridViewSRs.Rows.Add(row);
                    }
                }

                // add new SRs 
                foreach (var scannedDevice in ScannedDevices.Values)
                {
                    foreach (var srInfo in scannedDevice.SRList)
                    {
                        SrRow row;
                        if (!FindRowByUuid(srInfo.UUID, out row))
                        {
                            row = new SrRow(srInfo, scannedDevice.Type, srInfo.PoolMetadataDetected,
                                            SelectedSRsUuids.Contains(srInfo.UUID));
                            dataGridViewSRs.Rows.Add(row);
                        }
                    }
                }

                if (dataGridViewSRs.Rows.Count > 0)
                    SortRows();
            }
            finally
            {
                dataGridViewSRs.ResumeLayout();
            }

            OnPageUpdated();
        }

        private void SetupLabels()
        {
            switch (WizardType)
            {
                case DRWizardType.Failback:
                    labelText.Text = Messages.DR_WIZARD_STORAGEPAGE_DESCRIPTION_FAILBACK;
                    break;
                default:
                    labelText.Text = Messages.DR_WIZARD_STORAGEPAGE_DESCRIPTION_FAILOVER;
                    break;
            }
        }

        #endregion

        #region Accessors

        public List<string> SelectedSRsUuids = new List<String>();

        internal List<string> GetSelectedSRsNames()
        {
            return (from SrRow row in dataGridViewSRs.Rows where IsRowChecked(row) select row.SrName).ToList();
        }

        public DRWizardType WizardType { private get; set; }

        #endregion

        #region Scan for SRs

        private Dictionary<string, ScannedDeviceInfo> ScannedDevices = new Dictionary<string, ScannedDeviceInfo>();

        private const String LUNSERIAL = "LUNSerial";
        private const String SCSIID = "SCSIid";
        private const String METADATA = "metadata";

        private void ScanForSRs(SR.SRTypes type)
        {
            var srs = new List<SR.SRInfo>();

            switch (type)
            {
                case SR.SRTypes.lvmohba:
                    var devices = FiberChannelScan();
                    if (devices != null && devices.Count > 0)
                    {
                        foreach (FibreChannelDevice device in devices)
                        {
                            string deviceId = string.IsNullOrEmpty(device.SCSIid) ? device.Path : device.SCSIid;
                            var metadataSrs = ScanDeviceForSRs(SR.SRTypes.lvmohba, deviceId,
                                new Dictionary<string, string> {{SCSIID, device.SCSIid}});
                            if (metadataSrs != null && metadataSrs.Count > 0)
                                srs.AddRange(metadataSrs);
                        }
                    }
                    AddScanResultsToDataGridView(srs, SR.SRTypes.lvmohba);
                    break;

                case SR.SRTypes.lvmoiscsi:
                    using (var dialog = new IscsiDeviceConfigDialog(Connection))
                    {
                        if (dialog.ShowDialog(this) == DialogResult.OK)
                        {
                            Dictionary<String, String> dconf = dialog.DeviceConfig;
                            string deviceId = string.IsNullOrEmpty(dconf[SCSIID]) ? dconf[LUNSERIAL] : dconf[SCSIID];

                            var metadataSrs = ScanDeviceForSRs(SR.SRTypes.lvmoiscsi, deviceId, dconf);
                            if (metadataSrs != null && metadataSrs.Count > 0)
                                srs.AddRange(metadataSrs);
                        }
                        else
                        {
                            // if the user cancels the dialog there is no need to show the "no SRs found" pop-up
                            return;
                        }
                    }
                    AddScanResultsToDataGridView(srs, SR.SRTypes.lvmoiscsi);
                    break;
            }

            if (srs.Count == 0)
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Information,
                        Messages.DR_WIZARD_STORAGEPAGE_SCAN_RESULT_NONE,
                        Messages.XENCENTER)))
                {
                    dlg.ShowDialog(this);
                }
        }

        private List<FibreChannelDevice> FiberChannelScan()
        {
            Host master = Helpers.GetMaster(Connection);
            if (master == null)
                return null;

            var action = new FibreChannelProbeAction(master);
            using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                dialog.ShowDialog(this); //Will block until dialog closes, action completed

            return action.Succeeded ? action.FibreChannelDevices : null;
        }

        private List<SR.SRInfo> ScanDeviceForSRs(SR.SRTypes type, string deviceId, Dictionary<string, string> dconf)
        {
            Host master = Helpers.GetMaster(Connection);
            if (master == null || dconf == null)
                return null;

            // Start probe
            SrProbeAction srProbeAction = new SrProbeAction(Connection, master, type, dconf,
                new Dictionary<string, string> {{METADATA, "true"}});
            using (var dlg = new ActionProgressDialog(srProbeAction, ProgressBarStyle.Marquee))
                dlg.ShowDialog(this);

            if (!srProbeAction.Succeeded)
                return null;

            try
            {
                var metadataSrs = SR.ParseSRListXML(srProbeAction.Result);

                if (ScannedDevices.ContainsKey(deviceId))
                {
                    //update SR list
                    ScannedDevices[deviceId].SRList.Clear();
                    ScannedDevices[deviceId].SRList.AddRange(metadataSrs);
                }
                else
                {
                    ScannedDevices.Add(deviceId, new ScannedDeviceInfo(type, dconf, metadataSrs));
                }

                return metadataSrs;
            }
            catch
            {
                return null;
            }
        }

        private void AddScanResultsToDataGridView(List<SR.SRInfo> metadataSrs, SR.SRTypes type)
        {
            if (metadataSrs == null || metadataSrs.Count == 0)
                return;

            foreach (SR.SRInfo srInfo in metadataSrs)
            {
                SrRow row;
                if (!FindRowByUuid(srInfo.UUID, out row))
                {
                    row = new SrRow(srInfo, type, srInfo.PoolMetadataDetected, srInfo.PoolMetadataDetected);
                    dataGridViewSRs.Rows.Add(row);
                    ToggleRowChecked(row);
                }
            }
            SortRows();
        }

        #endregion

        #region Introduce SRs
        private void IntroduceSRs()
        {
            Pool pool = Helpers.GetPoolOfOne(Connection);
            if (pool == null)
            {
                log.Error("New SR Wizard: Pool has disappeared");
                return;
            }

            Host master = Connection.Resolve(pool.master);
            if (master == null)
            {
                log.Error("New SR Wizard: Master has disappeared");
                return;
            }

            // Start DR task
            foreach (var kvp in SelectedNewDevices)
            {
                var newDevice = kvp.Value;
                DrTaskCreateAction action = new DrTaskCreateAction(Connection, newDevice);
                using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                {
                    dialog.ShowCancel = true;
                    dialog.ShowDialog(this);
                }

                if(!cldl.IsStillConnected(Connection))
                    return;

                if (action.Succeeded)
                {
                    ScannedDevices[kvp.Key].SRList.Clear();
                    if (NewDrTaskIntroduced != null)
                        NewDrTaskIntroduced(action.Result);
                }
                else
                {
                    Exception exn = action.Exception;
                    log.Warn(exn, exn);
                    Failure failure = exn as Failure;
                    if (failure != null && failure.ErrorDescription[0] == "SR_BACKEND_FAILURE_140")
                    {
                        MessageBox.Show(this, failure.Message);
                    }
                    break;
                }
            }
        }

        private Dictionary<string, ScannedDeviceInfo> SelectedNewDevices
        {
            get
            {
                Dictionary<string, ScannedDeviceInfo> result = new Dictionary<string, ScannedDeviceInfo>();
                if (SelectedSRsUuids.Count > 0)
                {
                    foreach (var scannedDevice in ScannedDevices)
                    {
                        List<SR.SRInfo> lst = new List<SR.SRInfo>();
                        foreach (var srInfo in scannedDevice.Value.SRList)
                        {
                            if (SelectedSRsUuids.Contains(srInfo.UUID))
                                lst.Add(srInfo);
                        }
                        if (lst.Count > 0)
                        {
                            result.Add(scannedDevice.Key, new ScannedDeviceInfo(scannedDevice.Value.Type, scannedDevice.Value.DeviceConfig, lst));
                        }
                    }
                }
                return result;
            }
        }
        #endregion

        #region Load Pool Metadata
        private Dictionary<XenRef<VDI>, PoolMetadata> allPoolMetadata = new Dictionary<XenRef<VDI>, PoolMetadata>();
        public Dictionary<XenRef<VDI>, PoolMetadata> AllPoolMetadata { get { return allPoolMetadata; } }

        private void LoadMetadata()
        {
            allPoolMetadata.Clear();

            GetMetadataVDIsAction action = new GetMetadataVDIsAction(Connection, SelectedSRsUuids);

            if (!cldl.IsStillConnected(Connection))
                return;

            using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                dialog.ShowDialog(this);

            if (!cldl.IsStillConnected(Connection))
                return;

            if (action.Succeeded && action.VDIs != null)
            {
                foreach (VDI vdi in action.VDIs)
                {
                    var inUse = Connection.ResolveAll(vdi.VBDs).Any(vbd => vbd.currently_attached);
                    if (!inUse)
                        LoadPoolMetadata(vdi);
                    else
                        log.DebugFormat("This metadata VDI is in use: '{0}' (UUID='{1}', SR='{2})'; will not attempt to load metadata from it", vdi.Name(),
                                       vdi.uuid, Connection.Resolve(vdi.SR).Name());
                }
            }
        }

        private void LoadPoolMetadata(VDI vdi)
        {
            Session metadataSession = null;
            try
            {
                VdiLoadMetadataAction action = new VdiLoadMetadataAction(Connection, vdi);
                using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                    dialog.ShowDialog(this); //Will block until dialog closes, action completed

                if (action.Succeeded && action.MetadataSession != null)
                {
                    metadataSession = action.MetadataSession;
                    XenRef<VDI> vdiRef = new XenRef<VDI>(vdi);
                    if (action.PoolMetadata != null && !allPoolMetadata.ContainsKey(vdiRef))
                    {
                        allPoolMetadata.Add(vdiRef, action.PoolMetadata);
                    }
                }

            }
            finally
            {
                if (metadataSession != null)
                    metadataSession.logout();
            }
        }
        #endregion

        #region Control event handlers

        private void FindSrsButton_Click(object sender, EventArgs e)
        {
            FindSrsOptionsMenuStrip.Show(this,
                            new Point(FindSrsButton.Left, FindSrsButton.Bottom));
        }

        private void fcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScanForSRs(SR.SRTypes.lvmohba);
        }

        private void iscsiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScanForSRs(SR.SRTypes.lvmoiscsi);
        }

        private void dataGridViewSRs_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 || e.RowIndex < 0 || e.RowIndex > dataGridViewSRs.RowCount - 1)
                return;

            dataGridViewSRs.Rows[e.RowIndex].Cells[0].Value = !(bool)dataGridViewSRs.Rows[e.RowIndex].Cells[0].Value;
        }

        private void dataGridViewSRs_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 || e.RowIndex < 0 || e.RowIndex > dataGridViewSRs.RowCount - 1)
                return;

            ToggleRowChecked(dataGridViewSRs.Rows[e.RowIndex] as SrRow);
        }

        #endregion

        #region SrRow class and DataGridView helper methods

        private class SrRow : DataGridViewRow
        {
            private readonly SR Sr;
            private readonly SR.SRInfo SrInfo;

            public SrRow(SR sr, bool poolMetadataDetected, bool selected)
            {
                Sr = sr;

                var cellTick = new DataGridViewCheckBoxCell { Value = selected };
                var cellName = new DataGridViewTextBoxCell { Value = sr.Name() };
                var cellDesc = new DataGridViewTextBoxCell { Value = sr.Description() };
                var cellType = new DataGridViewTextBoxCell { Value = sr.FriendlyTypeName() };
                var cellMetadata = new DataGridViewTextBoxCell { Value = poolMetadataDetected.ToStringI18n() };
                Cells.AddRange(cellTick, cellName, cellDesc, cellType, cellMetadata);
            }

            public SrRow(SR.SRInfo srInfo, SR.SRTypes type, bool poolMetadataDetected, bool selected)
            {
                SrInfo = srInfo;

                var cellTick = new DataGridViewCheckBoxCell { Value = selected };
                var cellName = new DataGridViewTextBoxCell { Value = srInfo.Name };
                var cellDesc = new DataGridViewTextBoxCell { Value = srInfo.Description };
                var cellType = new DataGridViewTextBoxCell { Value = SR.getFriendlyTypeName(type) };
                var cellMetadata = new DataGridViewTextBoxCell { Value = poolMetadataDetected.ToStringI18n() };
                Cells.AddRange(cellTick, cellName, cellDesc, cellType, cellMetadata);
            }

            public string SrUuid
            {
                get
                {
                    if (Sr != null)
                        return Sr.uuid;
                    return SrInfo != null ? SrInfo.UUID : string.Empty;
                }
            }

            public string SrName
            {
                get
                {
                    if (Sr != null)
                        return Sr.Name();
                    return SrInfo != null ? SrInfo.Name : string.Empty;
                }
            }
        }

        private bool IsRowChecked(SrRow row)
        {
            if (row == null)
                return false;

            var cell = row.Cells[0] as DataGridViewCheckBoxCell;

            return cell == null ? false : (bool)cell.Value;
        }

        private void ToggleRowChecked(SrRow row)
        {
            if (row == null)
                return;

            if (IsRowChecked(row))
                SelectedSRsUuids.Add(row.SrUuid);
            else
                SelectedSRsUuids.Remove(row.SrUuid);

            OnPageUpdated();
        }

        private bool FindRowByUuid(string uuid, out SrRow row)
        {
            row = null;
            foreach (var srRow in dataGridViewSRs.Rows.Cast<SrRow>().Where(srRow => srRow.SrUuid == uuid))
            {
                row = srRow;
                return true;
            }
            return false;
        }

        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            SelectAllRows(true);
        }

        private void buttonClearAll_Click(object sender, EventArgs e)
        {
            SelectAllRows(false);
        }

        private void SelectAllRows(bool selected)
        {
            foreach (var row in dataGridViewSRs.Rows.OfType<SrRow>())
            {
                var cell = row.Cells[0] as DataGridViewCheckBoxCell;
                if (cell != null && (bool)cell.Value != selected)
                    cell.Value = selected;
            }
        }

        private void SortRows()
        {
            dataGridViewSRs.Sort(columnMetadata, ListSortDirection.Descending);
            dataGridViewSRs.Rows[0].Selected = true;
        }

        #endregion
    }
}
