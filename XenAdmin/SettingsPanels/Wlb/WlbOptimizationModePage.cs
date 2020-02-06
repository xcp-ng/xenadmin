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
    public partial class WlbOptimizationModePage : UserControl, IEditPage
    {
        private WlbPoolConfiguration _poolConfiguration;
        private Pool _pool;
        private bool _hasChanged = false;
        private bool _loading = false;

        //OptMode parameter keys
        private const string KEY_OPTIMIZATION_MODE = "OptMode";
        private const string KEY_POOL_UUID         = "PoolUUID";

        public WlbOptimizationModePage()
        {
            InitializeComponent();

            this.Text = Messages.WLB_OPTIMIZATION_MODE;
        }

        public WlbPoolConfiguration PoolConfiguration
        {
            set
            {
                if (null != value)
                {
                    _poolConfiguration = value;

                    if (_pool != null)
                    {
                        InitializeControls();
                    }
                }
            }
        }

        public Pool Pool
        {
            set
            {
                if (null != value)
                {
                    _pool = value;

                    if (_poolConfiguration != null)
                    {
                        InitializeControls();
                    }
                }
            }
        }

        private void InitializeControls()
        {
            _loading = true;

            // Pass the pool connection
            this.wlbOptModeScheduler1.Pool = _pool;
            this.wlbOptModeScheduler1.ScheduledTasks = _poolConfiguration.ScheduledTasks;
            this.wlbOptModeScheduler1.BaseMode = _poolConfiguration.PerformanceMode;

            radioButtonAutomatedMode.Checked = _poolConfiguration.AutomateOptimizationMode;
            radioButtonMaxDensity.Checked = (_poolConfiguration.PerformanceMode == WlbPoolPerformanceMode.MaximizeDensity);

            _loading = false;
        }

        private void radioButtonOptMode_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                _hasChanged = true;
            }
        }

        private void radioButtonMode_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                _hasChanged = true;
            }
            decentGroupBoxFixedMode.Enabled = radioButtonFixedMode.Checked;
            decentGroupBoxAutomatedMode.Enabled = radioButtonAutomatedMode.Checked;

            // CA-37454: Adding a redraw of the scheduled tasks here so they don;t disappear when 
            // mode is switched to Fixed.
            this.wlbOptModeScheduler1.RefreshScheduleList();
        }

        #region IEditPage Members

        public AsyncAction SaveSettings()
        {
            // We have to do this because of the way we delete existing scheduled tasks
            bool hasValidTasks = false;
            foreach (string key in wlbOptModeScheduler1.ScheduledTasks.TaskList.Keys)
            {
                if (!wlbOptModeScheduler1.ScheduledTasks.TaskList[key].DeleteTask
                    && wlbOptModeScheduler1.ScheduledTasks.TaskList[key].Enabled)
                {
                    hasValidTasks = true;
                }
            }
            // Save all the tasks in the control, as some may have Delete flags set
            _poolConfiguration.ScheduledTasks.TaskList.Clear();
            foreach (string key in wlbOptModeScheduler1.ScheduledTasks.TaskList.Keys)
            {
                _poolConfiguration.ScheduledTasks.TaskList.Add(key, wlbOptModeScheduler1.ScheduledTasks.TaskList[key]);
            }
            
            // Set the Automate flag if the radio button is checked and there are scheduled tasks
            _poolConfiguration.AutomateOptimizationMode = 
                (radioButtonAutomatedMode.Checked && 
                 hasValidTasks);

            if (!_poolConfiguration.AutomateOptimizationMode)
            {
                if (radioButtonMaxDensity.Checked)
                {
                    _poolConfiguration.PerformanceMode = WlbPoolPerformanceMode.MaximizeDensity;
                }
                else
                {
                    _poolConfiguration.PerformanceMode = WlbPoolPerformanceMode.MaximizePerformance;
                }
            }            

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

        public bool HasChanged => _hasChanged || wlbOptModeScheduler1.HasChanged;

        #endregion

        #region IVerticalTab Members


        public string SubText => Messages.WLB_OPTIMIZATION_MODE_SUBTEXT;

        public Image Image => Properties.Resources._000_Optimize_h32bit_16;

        #endregion

   }
}
