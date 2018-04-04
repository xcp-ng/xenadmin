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
using System.Data;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAPI;



namespace XenAdmin.SettingsPanels
{
    public partial class WlbMetricWeightingPage : UserControl, IEditPage
    {
        private const int TRACKBAR_INTERVAL = 10;

        private WlbPoolConfiguration _poolConfiguration;
        private bool _loading = false;
        private bool _hasChanged = false;
        private XenAdmin.Network.IXenConnection _connection;

        public WlbMetricWeightingPage()
        {
            InitializeComponent();

            Text = Messages.WLB_METRIC_WEIGHTING;
        }

        public WlbPoolConfiguration PoolConfiguration
        {
            set
            {
                if (null != value)
                {
                    _poolConfiguration = value;
                    InitializeControls();
                }
            }
        }

        public XenAdmin.Network.IXenConnection Connection
        {
            set
            {
                if (null != value)
                {
                    _connection = value;
                }
            }
        }

        private void InitializeControls()
        {
            _loading = true;
            trackbarCPUPriority.Value = GetSafeTrackbarValue(trackbarCPUPriority, _poolConfiguration.VmCpuUtilizationWeightHigh / TRACKBAR_INTERVAL);
            trackbarMemoryPriority.Value = GetSafeTrackbarValue(trackbarMemoryPriority, _poolConfiguration.VmMemoryWeightHigh / TRACKBAR_INTERVAL);
            trackbarNetReadPriority.Value = GetSafeTrackbarValue(trackbarNetReadPriority, _poolConfiguration.VmNetworkReadWeightHigh / TRACKBAR_INTERVAL);
            trackbarNetWritePriority.Value = GetSafeTrackbarValue(trackbarNetWritePriority, _poolConfiguration.VmNetworkWriteWeightHigh / TRACKBAR_INTERVAL);
            trackbarDiskReadPriority.Value = GetSafeTrackbarValue(trackbarDiskReadPriority, _poolConfiguration.VmDiskReadWeightHigh / TRACKBAR_INTERVAL);
            trackbarDiskWritePriority.Value = GetSafeTrackbarValue(trackbarDiskWritePriority, _poolConfiguration.VmDiskWriteWeightHigh / TRACKBAR_INTERVAL);
            // CA-194940:
            // Host disk read/write threshold and weight settings work since Dundee.
            // For previous XenServer, hide the host disk read/write settings.
            if (!Helpers.DundeeOrGreater(_connection))
            {
                trackbarDiskReadPriority.Visible = false;
                trackbarDiskWritePriority.Visible = false;
                label10.Visible = false;
                label11.Visible = false;
            }
            _loading = false; ;
        }

        private void trackbarValueChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                _hasChanged = true;
            }
        }

        private int GetSafeTrackbarValue(TrackBar thisControl, int value)
        {
            if (value < thisControl.Minimum)
            {
                value = thisControl.Minimum;
            }
            if (value > thisControl.Maximum)
            {
                value = thisControl.Maximum;
            }
            return value;
        }


        #region IEditPage Members

        public AsyncAction SaveSettings()
        {
            _poolConfiguration.VmCpuUtilizationWeightHigh = trackbarCPUPriority.Value * TRACKBAR_INTERVAL;
            _poolConfiguration.VmMemoryWeightHigh = trackbarMemoryPriority.Value * TRACKBAR_INTERVAL;
            _poolConfiguration.VmNetworkReadWeightHigh = trackbarNetReadPriority.Value * TRACKBAR_INTERVAL;
            _poolConfiguration.VmNetworkWriteWeightHigh = trackbarNetWritePriority.Value * TRACKBAR_INTERVAL;
            _poolConfiguration.VmDiskReadWeightHigh = trackbarDiskReadPriority.Value * TRACKBAR_INTERVAL;
            _poolConfiguration.VmDiskWriteWeightHigh = trackbarDiskWritePriority.Value * TRACKBAR_INTERVAL;
            return null;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool ValidToSave
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void ShowLocalValidationMessages()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Cleanup()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool HasChanged
        {
            get { return _hasChanged; }
        }

        #endregion

        #region IVerticalTab Members


        public string SubText
        {
            get { return Messages.WLB_METRIC_WEIGHTING_SUBTEXT; }
        }

        public Image Image
        {
            get { return Properties.Resources._000_weighting_h32bit_16; }
        }

        #endregion

    }
}
