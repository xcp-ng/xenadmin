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
using System.IO;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Controls.DataGridViewEx;


namespace XenAdmin.Wizards.BugToolWizardFiles
{
    public partial class BugToolPageRetrieveData : XenTabPage
    {
        public BugToolPageRetrieveData()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides

        public override string Text { get { return Messages.BUGTOOL_PAGE_RETRIEVEDATA_TEXT; } }

        public override string PageTitle { get { return Messages.BUGTOOL_PAGE_RETRIEVEDATA_PAGE_TITLE; } }

        public override string HelpID { get { return "CompileReport"; } }

        public override bool EnableNext()
        {
            var allCompleted = AllActionsCompleted(out bool successExists, out _);
            return allCompleted && successExists;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction == PageLoadedDirection.Forward)
                RunActions();
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Forward)
                return;

            CancelActions(ref cancel);
        }

        public override void PageCancelled(ref bool cancel)
        {
            CancelActions(ref cancel);
        }

        #endregion

        #region Properties
        public List<Host> SelectedHosts { private get; set; }
        public List<Capability> CapabilityList { private get; set; }
        public string OutputFolder { get; private set; }

        private bool AllActionsCompleted(out bool successExists, out bool failureExists)
        {
            successExists = false;
            failureExists = false;

            foreach (var row in dataGridViewEx1.Rows)
            {
                var srRow = (StatusReportRow)row;

                if (!srRow.IsCompleted)
                    return false;

                if (srRow.IsSuccessful)
                    successExists = true;
                else
                    failureExists = true;
            }
            return true;
        }

        #endregion

        private void CancelActions(ref bool cancel)
        {
            var allCompleted = AllActionsCompleted(out _, out _);
            if (allCompleted)
                return;

            using (var dlog = new ThreeButtonDialog(
                new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.BUGTOOL_PAGE_RETRIEVEDATA_CONFIRM_CANCEL, Messages.BUGTOOL_PAGE_RETRIEVEDATA_PAGE_TITLE),
                ThreeButtonDialog.ButtonYes,
                ThreeButtonDialog.ButtonNo))
            {
                if (dlog.ShowDialog(this) != DialogResult.Yes)
                {
                    cancel = true;
                    return;
                }
            }

            foreach (var r in dataGridViewEx1.Rows)
            {
                if (r is StatusReportRow row)
                {
                    row.DeRegisterEvents();
                    DeRegisterRowEvents(row);
                    row.CancelAction();
                }
            }
        }

        private void RunActions()
        {
            try
            {
                labelError.Text = "";
                progressBar1.Value = 0;
                dataGridViewEx1.SuspendLayout();
                dataGridViewEx1.Rows.Clear();

                var capabilityKeys = new List<string>();
                long size = 0;
                foreach (Capability c in CapabilityList)
                {
                    if (c.Key != "client-logs")
                        size += c.MaxSize;

                    capabilityKeys.Add(c.Key);
                }

                var rowList = new List<DataGridViewRow>();

                var includeClientLogs = capabilityKeys.Contains("client-logs");
                if (includeClientLogs || SelectedHosts.Count > 0)
                    rowList.Add(new ClientSideDataRow(SelectedHosts, includeClientLogs));

                foreach (Host host in SelectedHosts)
                    rowList.Add(new HostStatusRow(host, size, capabilityKeys));

                dataGridViewEx1.Rows.AddRange(rowList.ToArray());
            }
            finally
            {
                dataGridViewEx1.ResumeLayout();
            }

            OutputFolder = CreateOutputFolder();
            string timeString = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

            foreach (var r in dataGridViewEx1.Rows)
            {
                var row = (StatusReportRow)r;
                RegisterRowEvents(row);
                row.RunAction(OutputFolder, timeString);
            }

            OnPageUpdated();
        }

        private void Row_RowStatusChanged(StatusReportRow row)
        {
            UpdateTotalPercentComplete();
            OnPageUpdated();
        }

        private void Row_RowStatusCompleted(StatusReportRow row)
        {
            DeRegisterRowEvents(row);
            UpdateTotalPercentComplete();

            var allCompleted = AllActionsCompleted(out bool successExists, out bool failureExists);

            if (allCompleted)
            {
                if (!successExists)
                    labelError.Text = Messages.ACTION_SYSTEM_STATUS_FAILED;
                else if (!failureExists)
                    labelError.Text = Messages.ACTION_SYSTEM_STATUS_SUCCESSFUL;
                else
                    labelError.Text = Messages.ACTION_SYSTEM_STATUS_SUCCESSFUL_PARTIAL;
            }

            OnPageUpdated();
        }

        private void RegisterRowEvents(StatusReportRow row)
        {
            row.RowStatusChanged += Row_RowStatusChanged;
            row.RowStatusCompleted += Row_RowStatusCompleted;
        }

        private void DeRegisterRowEvents(StatusReportRow row)
        {
            row.RowStatusChanged -= Row_RowStatusChanged;
            row.RowStatusCompleted -= Row_RowStatusCompleted;
        }

        private void UpdateTotalPercentComplete()
        {
            int total = 0;
            foreach (var r in dataGridViewEx1.Rows)
            {
                var row = (StatusReportRow)r;
                total += row.PercentComplete;
            }

            var percentage = total / dataGridViewEx1.RowCount;

            if (percentage < 0)
                percentage = 0;
            if (percentage > 100)
                percentage = 100;

            if (percentage > progressBar1.Value)
                progressBar1.Value = percentage;
        }

        private static string CreateOutputFolder()
        {
            var folder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            if (Directory.Exists(folder))
                Directory.Delete(folder);
            Directory.CreateDirectory(folder);

            return folder;
        }

        #region Nested classes

        private abstract class StatusReportRow : DataGridViewRow
        {
            protected readonly DataGridViewExImageCell cellHostImg = new DataGridViewExImageCell();
            protected readonly DataGridViewTextBoxCell cellHost = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell cellStatus = new DataGridViewTextBoxCell();
            private readonly DataGridViewExImageCell cellResultImg = new DataGridViewExImageCell();

            public event Action<StatusReportRow> RowStatusChanged;
            public event Action<StatusReportRow> RowStatusCompleted;

            protected StatusReportRow()
            {
                Cells.AddRange(cellHostImg, cellHost, cellStatus, cellResultImg);
                cellResultImg.Value = new Bitmap(1, 1);
            }
           
            public abstract StatusReportAction Action { get; }
            public int PercentComplete { get; private set; }

            public bool IsCompleted
            {
                get { return Action != null && Action.IsCompleted; }
            }

            public bool IsSuccessful
            {
                get
                {
                    return Action != null && Action.IsCompleted && Action.Status == ReportStatus.succeeded;
                }
            }

            public void CancelAction()
            {
                if (Action != null && !Action.IsCompleted)
                    Action.Cancel();
            }

            public void RunAction(string path, string time)
            {
                CreateAction(path, time);
                Action.Changed += Action_Changed;
                Action.Completed += Action_Completed;
                UpdateCells(0);
                Action.RunAsync();
            }

            public void DeRegisterEvents()
            {
                if (Action == null)
                    return;

                Action.Changed -= Action_Changed;
                Action.Completed -= Action_Completed;
            }

            private void Action_Changed(ActionBase action)
            {
                Program.Invoke(DataGridView, () =>
                {
                    UpdateCells(action.PercentComplete);

                    if (RowStatusChanged != null)
                        RowStatusChanged(this);
                });
            }

            private void Action_Completed(ActionBase action)
            {
                DeRegisterEvents();

                Program.Invoke(DataGridView, () =>
                {
                    UpdateCells(100);

                    if (RowStatusCompleted != null)
                        RowStatusCompleted(this);
                });
            }

            protected abstract void CreateAction(string path, string time);

            protected virtual void UpdateCells(int percentComplete)
            {
                var statusString = GetStatus(out Image statusImage);
                cellStatus.Value = statusString;
                PercentComplete = percentComplete;

                if (statusImage != null)
                    cellResultImg.Value = statusImage;
            }

            protected virtual string GetStatus(out Image img)
            {
                img = null;
                if (Action == null)
                    return string.Empty;

                switch (Action.Status)
                {
                    case ReportStatus.compiling:
                        return Messages.BUGTOOL_REPORTSTATUS_COMPILING;
                   
                    case ReportStatus.succeeded:
                        img = Images.StaticImages._000_Tick_h32bit_16;
                        return Messages.COMPLETED;
                    
                    case ReportStatus.failed:
                        img = Images.StaticImages._000_Abort_h32bit_16;
                        return Messages.BUGTOOL_REPORTSTATUS_FAILED;

                    case ReportStatus.cancelled:
                        img = Images.StaticImages.cancelled_action_16;
                        return Messages.BUGTOOL_REPORTSTATUS_CANCELLED;
                    
                    case ReportStatus.queued:
                        return Messages.BUGTOOL_REPORTSTATUS_QUEUED;
                    
                    default:
                        return string.Empty;
                }
            }
        }

        private class ClientSideDataRow : StatusReportRow
        {
            private readonly List<Host> hosts;
            private readonly bool includeClientLogs;
            private StatusReportClientSideAction _action;

            public ClientSideDataRow(List<Host> hosts, bool includeClientLogs)
            {
                this.hosts = hosts;
                this.includeClientLogs = includeClientLogs;
                cellHostImg.Value = Images.StaticImages._000_GetServerReport_h32bit_16;
                cellHost.Value = includeClientLogs ? Messages.BUGTOOL_CLIENT_LOGS_META : Messages.BUGTOOL_CLIENT_META;
            }

            public override StatusReportAction Action
            {
                get { return _action; }
            }

            protected override void CreateAction(string path, string time)
            {
                _action = new StatusReportClientSideAction(hosts, includeClientLogs, path, time);
            }
        }

        private class HostStatusRow : StatusReportRow
        {
            private readonly long size;
            private readonly List<string> capabilityKeys;
            private readonly Host Host;
            private SingleHostStatusAction _action;

            public HostStatusRow(Host host, long size, List<string> capabilityKeys)
            {
                Host = host;
                this.size = size;
                this.capabilityKeys = capabilityKeys;
            }

            protected override void UpdateCells(int percentComplete)
            {
                cellHostImg.Value = Images.GetImage16For(Host);
                cellHost.Value = Host.Name();
                base.UpdateCells(percentComplete);
            }

            public override StatusReportAction Action
            {
                get { return _action; }
            }

            protected override void CreateAction(string path, string time)
            {
                _action = new SingleHostStatusAction(Host, size, capabilityKeys, path, time);
            }

            protected override string GetStatus(out Image img)
            {
                img = null;
                if (_action == null)
                    return string.Empty;

                switch (_action.Status)
                {
                    case ReportStatus.downloading:
                        return string.Format(Messages.BUGTOOL_REPORTSTATUS_DOWNLOADING,
                            Util.MemorySizeStringSuitableUnits(_action.DataTransferred, false));

                    default:
                        return base.GetStatus(out img);
                }
               
            }
        }

        #endregion
    }

}
