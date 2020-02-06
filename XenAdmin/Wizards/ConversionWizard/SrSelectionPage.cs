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
using System.Linq;
using XenAdmin.Controls;
using XenAdmin.XCM;
using XenAPI;
using XenCenterLib;


namespace XenAdmin.Wizards.ConversionWizard
{
    public partial class SrSelectionPage : XenTabPage
    {
        private bool _buttonNextEnabled;
        private long _requiredDiskSize;
        public SrSelectionPage()
        {
            InitializeComponent();
        }

        #region XenTabPage Implementation

        public override string Text => Messages.CONVERSION_STORAGE_PAGE_TEXT;

        public override string PageTitle => Messages.CONVERSION_STORAGE_PAGE_TITLE;

        public override string HelpID => "SrSelection";

        public override bool EnableNext()
        {
            return _buttonNextEnabled;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction == PageLoadedDirection.Forward)
                RebuildSrList();
        }

        public override void PageCancelled(ref bool cancel)
        {
           backgroundWorker1.CancelAsync();
        }

        #endregion

        #region Properties

        public ConversionClient ConversionClient { private get; set; }

        public VmInstance[] SelectedVms
        {
            set { _requiredDiskSize = value.Select(vm => vm.CommittedStorage + vm.UncommittedStorage).Sum(); }
        }

        public SR SelectedSR { get; private set; }

        #endregion

        private void RebuildSrList()
        {
            comboBoxSr.Items.Clear();
            buttonRefresh.Enabled = false;

            pictureBoxError.Image = Images.StaticImages.ajax_loader;
            labelError.Text = Messages.CONVERSION_STORAGE_PAGE_QUERYING_SRS;
            tableLayoutPanelError.Visible = true;

            UpdatePieChart();
            UpdateButtons();

            backgroundWorker1.RunWorkerAsync();
        }

        private void UpdatePieChart()
        {
            var wrapper = comboBoxSr.SelectedItem as SrWrapper;
            if (wrapper == null)
            {
                chart1.Visible = false;
                return;
            }

            SR sr = wrapper.item;
            var availableSpace = wrapper.AvailableSpace;
            var usedSpace = sr.physical_size - availableSpace;

            //order of colours in palette: blue, yellow, red

            string[] xValues =
            {
                string.Format(Messages.CONVERSION_STORAGE_PAGE_FREE_SPACE, Util.DiskSizeString(availableSpace)),
                string.Format(Messages.CONVERSION_STORAGE_PAGE_REQUIRED_SPACE, Util.DiskSizeString(_requiredDiskSize)),
                string.Format(Messages.CONVERSION_STORAGE_PAGE_USED_SPACE, Util.DiskSizeString(usedSpace))
            };
            long[] yValues =
            {
                availableSpace > _requiredDiskSize? availableSpace - _requiredDiskSize : 0,
                availableSpace > _requiredDiskSize ? _requiredDiskSize : availableSpace,
                usedSpace
            };

            chart1.Series[0].Points.DataBindXY(xValues, yValues);
            chart1.Visible = true;
        }

        private void UpdateButtons()
        {
            _buttonNextEnabled = comboBoxSr.SelectedItem is SrWrapper wrapper && wrapper.CanUse;
            OnPageUpdated();
        }

        private void ShowError(string msg)
        {
            pictureBoxError.Image = Images.StaticImages._000_error_h32bit_16;
            labelError.Text = msg;
            tableLayoutPanelError.Visible = true;
        }

        private void ShowWarning(string msg)
        {
            pictureBoxError.Image = Images.StaticImages._000_Alert2_h32bit_16;
            labelError.Text = msg;
            tableLayoutPanelError.Visible = true;
        }

        #region Event handler

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var spacePerSr = new Dictionary<SR, long>();

            foreach (var pbd in Connection.Cache.PBDs)
            {
                if (pbd.SR == null)
                    continue;

                var sr = Connection.Resolve(pbd.SR);
                if (sr == null || sr.IsDetached() || !sr.Show(XenAdminConfigManager.Provider.ShowHiddenVMs))
                    continue;

                if (sr.content_type.ToLower() == "iso" || sr.type.ToLower() == "iso")
                    continue;

                var hosts = Connection.Cache.Hosts;
                if (hosts.Any(h => !sr.CanBeSeenFrom(h)))
                    continue;

                var reservedSpace = ConversionClient.GetReservedDiskSpace(sr.uuid);
                var srFreeSpace = sr.FreeSpace();

                if (srFreeSpace > reservedSpace)
                    spacePerSr[sr] = srFreeSpace - reservedSpace;
            }

            e.Result = spacePerSr;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Dictionary<SR, long> spacePerSr;

            if (e.Cancelled)
            {
                tableLayoutPanelError.Visible = false;
            }
            else if (e.Error != null)
            {
                ShowError(Messages.CONVERSION_STORAGE_PAGE_QUERYING_SRS_FAILURE);
            }
            else if ((spacePerSr = e.Result as Dictionary<SR, long>) != null && spacePerSr.Count > 0)
            {
                tableLayoutPanelError.Visible = false;

                foreach (var kvp in spacePerSr)
                {
                    var sr = kvp.Key;
                    var wrapper = new SrWrapper(sr, kvp.Value);
                    comboBoxSr.Items.Add(wrapper);

                    if (SelectedSR != null && SelectedSR.uuid == sr.uuid)
                        comboBoxSr.SelectedItem = wrapper;
                    else if (SelectedSR == null && SR.IsDefaultSr(sr))
                        comboBoxSr.SelectedItem = wrapper;
                }
            }
            else if (spacePerSr != null)
            {
                ShowError(Messages.CONVERSION_STORAGE_PAGE_QUERYING_SRS_EMPTY);
            }

            buttonRefresh.Enabled = true;
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RebuildSrList();
        }

        private void comboBoxSr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSr.SelectedItem is SrWrapper wrapper)
            {
                SelectedSR = wrapper.item;
                SM sm = SelectedSR.GetSM();
                bool vdiSizeUnlimited = sm != null && Array.IndexOf(sm.capabilities, "LARGE_VDI") != -1;
                bool isThinlyProvisioned = sm != null && Array.IndexOf(sm.capabilities, "THIN_PROVISIONING") != -1;
                if (_requiredDiskSize > SR.DISK_MAX_SIZE && !vdiSizeUnlimited)
                {
                    ShowError(string.Format(Messages.SR_DISKSIZE_EXCEEDS_DISK_MAX_SIZE, Util.DiskSizeString(SR.DISK_MAX_SIZE, 0)));
                    wrapper.CanUse = false;
                }
                else if (_requiredDiskSize > wrapper.AvailableSpace && !isThinlyProvisioned)
                {
                    ShowError(Messages.CONVERSION_STORAGE_PAGE_SR_TOO_SMALL);
                    wrapper.CanUse = false;
                }
                else if (_requiredDiskSize > wrapper.AvailableSpace)
                {
                    ShowWarning(Messages.CONVERSION_STORAGE_PAGE_SR_OVERCOMMIT);
                    wrapper.CanUse = true;
                }
                else
                {
                    tableLayoutPanelError.Visible = false;
                    wrapper.CanUse = true;
                }
            }
            UpdatePieChart();
            UpdateButtons();
        }

        #endregion

        #region Nested Items

        private class SrWrapper : ToStringWrapper<SR>
        {
            public readonly long AvailableSpace;
            public bool CanUse;
            public SrWrapper(SR sr, long availableSpace)
                : base(sr, $"{sr.Name()}, {Util.DiskSizeString(availableSpace)} {Messages.AVAILABLE}")
            {
                AvailableSpace = availableSpace;
            }
        }

        #endregion
    }
}
