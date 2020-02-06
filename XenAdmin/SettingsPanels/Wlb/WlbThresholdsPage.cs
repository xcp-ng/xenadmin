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
using System.Drawing;
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAPI;


namespace XenAdmin.SettingsPanels
{
    public partial class WlbThresholdsPage : UserControl, IEditPage
    {
        private WlbPoolConfiguration _poolConfiguration;
        private bool _loading = false;
        private bool _hasChanged = false;
        private XenAdmin.Network.IXenConnection _connection;

        public WlbThresholdsPage()
        {
            InitializeComponent();

            Text = Messages.WLB_THRESHOLDS;
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
            updownCPUCriticalPoint.Value = GetSafeUpDownValue(_poolConfiguration.HostCpuThresholdCritical, updownCPUCriticalPoint);
            updownMemoryCriticalPoint.Value = GetSafeUpDownValue(WlbPoolConfiguration.ConvertToMB(_poolConfiguration.HostMemoryThresholdCritical), updownMemoryCriticalPoint);
            updownDiskReadCriticalPoint.Value = GetSafeUpDownValue(WlbPoolConfiguration.ConvertToMB(_poolConfiguration.HostDiskReadThresholdCritical), updownDiskReadCriticalPoint);
            updownDiskWriteCriticalPoint.Value = GetSafeUpDownValue(WlbPoolConfiguration.ConvertToMB(_poolConfiguration.HostDiskWriteThresholdCritical), updownDiskWriteCriticalPoint);
            updownNetworkReadCriticalPoint.Value = GetSafeUpDownValue(WlbPoolConfiguration.ConvertToMB(_poolConfiguration.HostNetworkReadThresholdCritical), updownNetworkReadCriticalPoint);
            updownNetworkWriteCriticalPoint.Value = GetSafeUpDownValue(WlbPoolConfiguration.ConvertToMB(_poolConfiguration.HostNetworkWriteThresholdCritical), updownNetworkWriteCriticalPoint);

            labelCPUUnits.Text = string.Format(labelCPUUnits.Text, updownCPUCriticalPoint.Minimum, updownCPUCriticalPoint.Maximum);
            labelFreeMemoryUnits.Text = string.Format(labelFreeMemoryUnits.Text, updownMemoryCriticalPoint.Minimum, updownMemoryCriticalPoint.Maximum);
            labelDiskReadUnits.Text = string.Format(labelDiskReadUnits.Text, updownDiskReadCriticalPoint.Minimum, updownDiskReadCriticalPoint.Maximum);
            labelDiskWriteUnits.Text = string.Format(labelDiskWriteUnits.Text, updownDiskWriteCriticalPoint.Minimum, updownDiskWriteCriticalPoint.Maximum);
            labelNetworkReadUnits.Text = string.Format(labelNetworkReadUnits.Text, updownNetworkReadCriticalPoint.Minimum, updownNetworkReadCriticalPoint.Maximum);
            labelNetworkWriteUnits.Text = string.Format(labelNetworkWriteUnits.Text, updownNetworkWriteCriticalPoint.Minimum, updownNetworkWriteCriticalPoint.Maximum);

            // CA-194940:
            // Host disk read/write threshold and weight settings work since Dundee.
            // For previous XenServer, hide the host disk read/write settings.
            if (!Helpers.DundeeOrGreater(_connection))
            {
                updownDiskReadCriticalPoint.Visible = false;
                updownDiskWriteCriticalPoint.Visible = false;
                labelDiskReadUnits.Visible = false;
                labelDiskWriteUnits.Visible = false;
                labelDiskRead.Visible = false;
                label1DiskWrite.Visible = false;
            }
            _loading = false; ;
        }

        private decimal GetSafeUpDownValue(decimal value, NumericUpDown thisControl)
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

        private void updown_ValueChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                if (sender is NumericUpDown)
                {
                    NumericUpDown thisNumericUpDown = sender as NumericUpDown;
                    thisNumericUpDown.Value = GetSafeUpDownValue(thisNumericUpDown.Value, thisNumericUpDown);
                    _hasChanged = true;
                }
            }
        }

        private void updown_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender is NumericUpDown)
            {
                NumericUpDown thisNumericUpDown = sender as NumericUpDown;
                thisNumericUpDown.Value = GetSafeUpDownValue(thisNumericUpDown.Value, thisNumericUpDown);
                _hasChanged = true;
            }

        }
        
        #region IEditPage Members

        public AsyncAction SaveSettings()
        {
            _poolConfiguration.HostCpuThresholdCritical = (int)updownCPUCriticalPoint.Value;
            _poolConfiguration.HostMemoryThresholdCritical = WlbPoolConfiguration.ConvertFromMB((int)updownMemoryCriticalPoint.Value);
            _poolConfiguration.HostDiskReadThresholdCritical = WlbPoolConfiguration.ConvertFromMB((int)updownDiskReadCriticalPoint.Value);
            _poolConfiguration.HostDiskWriteThresholdCritical = WlbPoolConfiguration.ConvertFromMB((int)updownDiskWriteCriticalPoint.Value);
            _poolConfiguration.HostNetworkReadThresholdCritical = WlbPoolConfiguration.ConvertFromMB((int)updownNetworkReadCriticalPoint.Value);
            _poolConfiguration.HostNetworkWriteThresholdCritical = WlbPoolConfiguration.ConvertFromMB((int)updownNetworkWriteCriticalPoint.Value);

            return null;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            throw new NotImplementedException();
        }

        public bool ValidToSave => throw new NotImplementedException();

        public void ShowLocalValidationMessages()
        {
            throw new NotImplementedException();
        }

        public void Cleanup()
        {
            throw new NotImplementedException();
        }

        public bool HasChanged => _hasChanged;

        #endregion

        #region IVerticalTab Members


        public string SubText => Messages.WLB_THRESHOLDS_SUBTEXT;

        public Image Image => Properties.Resources._000_GetMemoryInfo_h32bit_16;

        #endregion
    }
}
