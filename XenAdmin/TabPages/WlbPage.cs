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
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.Actions.Wlb;
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.Wlb;
using XenAPI;


namespace XenAdmin.TabPages
{
    public partial class WlbPage : BaseTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Pool _pool;
        private WlbPoolConfiguration _wlbPoolConfiguration;

        /// <summary>
        /// The pool that the panel is displaying WLB info for. Must be set on the event thread.
        /// </summary>
        public Pool Pool
        {
            set
            {
                Program.AssertOnEventThread();
                if (null != value)
                {
                    this.wlbOptimizePool.XenObject = value;

                    if (_pool != value)
                    {
                        if (_pool != null)
                            _pool.PropertyChanged -= Pool_PropertyChanged;
                        _pool = value;
                        _pool.PropertyChanged += Pool_PropertyChanged;
                    }

                    SetRetrievingConfiguration();
                    RetrieveConfiguration();
                    RefreshControls();
                }
            }
        }

        public WlbPage()
        {
            InitializeComponent();

            base.Text = Messages.WORKLOAD_BALANCING;
            pdSectionConfiguration.fixFirstColumnWidth(200);
            pictureBoxWarningTriangle.Image = SystemIcons.Warning.ToBitmap();

            RefreshControls();
        }

        public override string HelpID => "TabPageWLB";

        #region ControlEvents

        private void buttonConfigure_Click(object sender, EventArgs e)
        {

            if (_pool == null)
            {
                return;
            }

            // if the _wlbPoolConfiguration is null, it means we cannot communicate with the WLB 
            // server for some reason do nothing
            if (null != _wlbPoolConfiguration) 
            {
                DisableButtons();
                EditWLB(_pool);
            }

        }

        private void buttonEnableDisableWlb_Click(object sender, EventArgs e)
        {
            if (WlbServerState.GetState(_pool) != WlbServerState.ServerState.Enabled)
            {
                DisableButtons();
                EnableWLB();
            }
            else
            {
                DisableButtons();
                DisableWLB(false);
            }
        }

        #endregion

        #region Wlb Actions

        private void EnableWLB()
        {
            // Enable WLB.
            EnableWLBAction action = new EnableWLBAction(_pool);
            // We will need to re-enable buttons when the action completes
            action.Completed += Program.MainWindow.action_Completed;
            action.Completed += this.action_Completed;
            action.RunAsync();
        }

        private void DisableWLB(bool deconfigure)
        {
            DisableWLBAction action = new DisableWLBAction(_pool, deconfigure);
            action.Completed += Program.MainWindow.action_Completed;
            action.Completed += this.action_Completed;
            action.RunAsync();
        }

        private void RetrieveConfiguration()
        {
            // only get the config if there are no other pending wlb actions
            if (null == HelpersGUI.FindActiveWLBAction(_pool.Connection))
            {
                RetrieveWlbConfigurationAction action = new RetrieveWlbConfigurationAction(_pool);
                action.Completed += this.action_Completed;
                action.RunAsync();
            }
        }

        private void SaveWLBConfig(WlbPoolConfiguration PoolConfiguration)
        {
            Dictionary<string, string> completeConfiguration = PoolConfiguration.ToDictionary();
            SendWlbConfigurationKind kind = SendWlbConfigurationKind.SetPoolConfiguration;

            // check for host configurations in the pool configuration
            if (PoolConfiguration.HostConfigurations.Count > 0)
            {
                // add the flag denoting that there are host configs to be saved
                kind |= SendWlbConfigurationKind.SetHostConfiguration;
                // get the key for each host config (host.uuid)
                foreach (string key in PoolConfiguration.HostConfigurations.ToDictionary().Keys)
                {
                    //Drop any exising copy from the pool configuration
                    if (completeConfiguration.ContainsKey(key))
                    {
                        completeConfiguration.Remove(key);
                    }
                    // and add the task to the collection
                    completeConfiguration.Add(key, PoolConfiguration.HostConfigurations.ToDictionary()[key]);
                }
            }

            // check for scheduled tasks in the pool configuration
            if (PoolConfiguration.ScheduledTasks.TaskList.Count > 0)
            {
                // add the flag denoting that there are scheduled tasks to be saved
                kind |= SendWlbConfigurationKind.SetScheduledTask;
                // get the key for each scheduled task
                foreach (string key in PoolConfiguration.ScheduledTasks.ToDictionary().Keys)
                {
                    //Drop any exising copy from the pool configuration
                    if (completeConfiguration.ContainsKey(key))
                    {
                        completeConfiguration.Remove(key);
                    }
                    // and add the task to the collection
                    completeConfiguration.Add(key, PoolConfiguration.ScheduledTasks.ToDictionary()[key]);
                }
            }

            SendWlbConfigurationAction action = new SendWlbConfigurationAction(_pool, PoolConfiguration.ToDictionary(), kind);
            using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Blocks))
            {
                dialog.ShowCancel = true;
                dialog.ShowDialog(this);
            }
            Program.MainWindow.UpdateToolbars();
        }

        #endregion

        #region Event Handlers

        protected void action_Completed(ActionBase sender)
        {
            // This seems to be called off the event thread
            AsyncAction action = (AsyncAction)sender;
            if (action.IsCompleted)
            {
                action.Completed -= action_Completed;
                if (action is EnableWLBAction || action is RetrieveWlbConfigurationAction || action is DisableWLBAction)
                {
                    if (action is EnableWLBAction)
                    {
                        EnableWLBAction thisAction = (EnableWLBAction)action;
                        _wlbPoolConfiguration = new WlbPoolConfiguration(thisAction.WlbConfiguration);
                    }
                    else if (action is RetrieveWlbConfigurationAction)
                    {
                        RetrieveWlbConfigurationAction thisAction = (RetrieveWlbConfigurationAction)action;
                        if (thisAction.Succeeded)
                        {
                            _wlbPoolConfiguration = new WlbPoolConfiguration(thisAction.WlbConfiguration);
                        }
                        else
                        {
                            //_statusError = thisAction.Exception.Message;
                            _wlbPoolConfiguration = null;
                        }
                    }
                    else if (action is DisableWLBAction)
                    {
                    }

                    Program.Invoke(Program.MainWindow, delegate()
                    {
                        if (_pool != null && _pool.Connection == action.Connection)
                        {
                            RefreshControls();
                        }
                    });
                }
            }
        }

        private void Pool_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            // Refresh controls if a wlb pool preoprty changes, or if other_config changes
            if (e.PropertyName.StartsWith("wlb") || e.PropertyName == "other_config")
            {
                RefreshControls();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Rebuilds the panel contents. Must be called on the event thread.
        /// </summary>
        private void RefreshControls()
        {
            Program.AssertOnEventThread();
            if ((!this.Visible) ||
                (null == _pool))
            {
                return;
            }

            try
            {

                if (null != _wlbPoolConfiguration)
                {
                    this.wlbOptimizePool.SuspendLayout();
                    // Update optimizePool control properties (versionSupport, apply button) 
                    this.wlbOptimizePool.SetOptControlProperties(_wlbPoolConfiguration.AutoBalanceEnabled, _wlbPoolConfiguration.PowerManagementEnabled);
                    this.wlbOptimizePool.ResumeLayout();
                }

                SetButtonState();

                SetConfigurationItems();
            }
            catch
            {
                //do nothing
            }
        }

        private void SetRetrievingConfiguration()
        {
            pdSectionConfiguration.PauseLayout();
            pdSectionConfiguration.ClearData();
            pdSectionConfiguration.AddEntry(Messages.WLB_RETRIEVING_CONFIGURATION, Messages.WLB_PLEASE_WAIT);
            pdSectionConfiguration.StartLayout();
            pdSectionConfiguration.Expand();
        }

        private void SetConfigurationItems()
        {
            // Populate the Configuration Items
            pdSectionConfiguration.PauseLayout();
            pdSectionConfiguration.ClearData();
            if (WlbServerState.GetState(_pool) == WlbServerState.ServerState.ConnectionError)
            {
                pdSectionConfiguration.AddEntry(Messages.WLB_STATUS, getPoolWLBStatus(_pool));
                pdSectionConfiguration.AddEntry(Messages.WLB_SERVER_URL, getPoolWLBServer(_pool));
            }
            else
            {
                pdSectionConfiguration.AddEntry(Messages.WLB_SERVER_URL, getPoolWLBServer(_pool));
                pdSectionConfiguration.AddEntry(Messages.WLB_OPT_MODE, getPoolOptMode(_pool));
                if (null != _wlbPoolConfiguration)
                {
                    if (_wlbPoolConfiguration.AutomateOptimizationMode)
                    {
                        pdSectionConfiguration.AddEntry(Messages.WLB_NEXT_OPT_MODE_SCHEDULE_TEXT, getNextOptModeTask(_pool));
                    }
                    pdSectionConfiguration.AddEntry(Messages.WLB_AUTO_OPT, getAutoOptimization(_pool));
                    pdSectionConfiguration.AddEntry(Messages.WLB_PWR_MGT, getPoolWlbPowerManagement(_pool));
                }
            }
            pdSectionConfiguration.Expand();
            pdSectionConfiguration.StartLayout();

        }

        private RbacMethodList WLB_PERMISSION_CHECKS = new RbacMethodList(
            "pool.initialize_wlb",
            "pool.set_wlb_enabled",
            "pool.send_wlb_configuration"
            );

        private bool PassedRbacChecks()
        {
            return Role.CanPerform(WLB_PERMISSION_CHECKS, _pool.Connection);
        }

        private void SetButtonState()
        {
            string statusMessage = string.Empty;
            string rbacUser = string.Empty;
            bool passedRbacChecks = PassedRbacChecks();

            if (!passedRbacChecks)
            {
                List<Role> roleList = _pool.Connection.Session.Roles;
                roleList.Sort();
                rbacUser = roleList[0].FriendlyName();
            }

            AsyncAction thisAction = HelpersGUI.FindActiveWLBAction(_pool.Connection);
            if (thisAction is RetrieveWlbConfigurationAction)
            {
                DisableButtons();
                return;
            }

            //if wlb is not configured,
            // disable the config button and change the text of the Enable button to Initialize WLB...
            //else if (!wlbConfigured)
            else if (WlbServerState.GetState(_pool) == WlbServerState.ServerState.NotConfigured)
            {
                buttonConnect.Visible = true;
                buttonConnect.Text = Messages.WLB_CONNECT;

                buttonConfigure.Visible = false;
                buttonEnableDisableWlb.Visible = false;
                buttonReports.Visible = false;
                pictureBoxWarningTriangle.Visible = false;

                if (passedRbacChecks)
                {
                    statusMessage = string.Format(Messages.WLB_INITIALIZE_WLB_BLURB, _pool.Name());
                }
                else
                {
                    statusMessage = string.Format(Messages.WLB_INITIALIZE_WLB_BLURB_NO_PRIV, _pool.Name(), rbacUser);
                }
                labelStatus.Text = statusMessage;
                pictureBoxWarningTriangle.Visible = (WlbServerState.GetState(_pool) == WlbServerState.ServerState.ConnectionError);

                panelConfiguration.Visible = false;
                wlbOptimizePool.Visible = false;
            }

            //if there is an error contacting the WLB server,
            // disable the config button and change the text of the Enable button to Initialize WLB...
            //else if (!wlbConfigured)
            else if (WlbServerState.GetState(_pool) == WlbServerState.ServerState.ConnectionError)
            {
                buttonConnect.Visible = true;
                buttonConnect.Text = Messages.WLB_RECONNECT;
        
                buttonConfigure.Visible = false;
                buttonEnableDisableWlb.Visible = false;
                buttonReports.Visible = false;
                pictureBoxWarningTriangle.Visible = true;

                if (passedRbacChecks)
                {
                    statusMessage = Messages.WLB_CONNECTION_ERROR_BLURB;
                }
                else
                {
                    statusMessage = string.Format(Messages.WLB_CONNECTION_ERROR_BLURB_NO_PRIV, rbacUser);
                }
                labelStatus.Text = statusMessage;
                labelStatus.Visible = true;

                pictureBoxWarningTriangle.Visible = (WlbServerState.GetState(_pool) == WlbServerState.ServerState.ConnectionError);
                buttonReports.Enabled = false;
                panelConfiguration.Visible = false;
                wlbOptimizePool.Visible = false;
            }

            //if wlb is configured but not enabled, 
            //enable the configure button and enble the enable button
            //else if (wlbConfigured && !wlbEnabled)
            else if (WlbServerState.GetState(_pool) == WlbServerState.ServerState.Disabled)
            {
                base.Text = Messages.WORKLOAD_BALANCING;

                buttonConnect.Visible = false;
                buttonConfigure.Visible = true;
                buttonEnableDisableWlb.Visible = true;
                buttonReports.Visible = true;
                pictureBoxWarningTriangle.Visible = true;

                buttonConfigure.Enabled = true;
                buttonEnableDisableWlb.Enabled = true;
                buttonEnableDisableWlb.Text = Messages.ENABLE_WLB_ELLIPSIS;
                buttonEnableDisableWlb.ImageIndex = 1; //Play arrow

                if (!passedRbacChecks)
                {
                    statusMessage = string.Format(Messages.WLB_ENABLE_WLB_BLURB_NO_PRIV, _pool.Name(), rbacUser);
                }
                else
                {
                    statusMessage = string.Format(Messages.WLB_ENABLE_WLB_BLURB, _pool.Name());
                }
                labelStatus.Text = statusMessage;

                buttonReports.Enabled = true;
                panelConfiguration.Visible = true;
                wlbOptimizePool.Visible = true;
            }
            //otherwise, if wlb is configured and enabled, allow configuration, and show enable as Disable
            //else if (wlbEnabled)
            else if (WlbServerState.GetState(_pool) == WlbServerState.ServerState.Enabled)
            {
                buttonConnect.Visible = false;
                buttonConfigure.Visible = true;
                buttonEnableDisableWlb.Visible = true;
                buttonReports.Visible = true;
                pictureBoxWarningTriangle.Visible = false;
               
                buttonConfigure.Enabled = true;
                buttonEnableDisableWlb.Enabled = true;
                buttonEnableDisableWlb.Text = Messages.DISABLE_WLB_ELLIPSIS;
                buttonEnableDisableWlb.ImageIndex = 0; //Pause hashes

                if (!passedRbacChecks)
                {
                    statusMessage = string.Format(Messages.WLB_NO_PERMISSION_BLURB, rbacUser);
                }
                else
                {
                    statusMessage = string.Format(Messages.WLB_ENABLED_BLURB, _pool.Name());
                }

                labelStatus.Text = statusMessage;
                buttonReports.Enabled = true;
                panelConfiguration.Visible = true;
                wlbOptimizePool.Visible = true;
            }


            if (PassedRbacChecks())
            {
                //Show the buttons
                flowLayoutPanelLeftButtons.Visible = true;
                //panelButtons.Visible = true;
            }
            else
            {
                //disable and hide the buttons
                DisableButtons();
                flowLayoutPanelLeftButtons.Visible = false;
                //panelButtons.Visible = false;
            }
        }

        private void DisableButtons()
        {
            buttonConfigure.Enabled = false;
            buttonEnableDisableWlb.Enabled = false;
        }

        internal void EditWLB(Pool pool)
        {
            // Do nothing if there is a WLB action in progress 
            if (HelpersGUI.FindActiveWLBAction(pool.Connection) != null)
            {
                log.Debug("Not opening WLB dialog: an WLB action is in progress");
                return;
            }

            if (!pool.Connection.IsConnected)
            {
                log.Debug("Not opening WLB dialog: the connection to the pool is now closed");
                return;
            }

            try
            {
                WlbConfigurationDialog wlbConfigDialog = new WlbConfigurationDialog(pool);
                DialogResult dr = wlbConfigDialog.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    _wlbPoolConfiguration = wlbConfigDialog.WlbPoolConfiguration;

                    //check to see if the current opt mode matches the current schedule
                    if (_wlbPoolConfiguration.AutomateOptimizationMode)
                    {
                        WlbPoolPerformanceMode scheduledPerfMode = _wlbPoolConfiguration.ScheduledTasks.GetCurrentScheduledPerformanceMode();
                        if (scheduledPerfMode != _wlbPoolConfiguration.PerformanceMode)
                        {
                            string blurb = string.Format(Messages.WLB_PROMPT_FOR_MODE_CHANGE_BLURB, getOptModeText(scheduledPerfMode), getOptModeText(_wlbPoolConfiguration.PerformanceMode));
                            DialogResult drModeCheck;
                            using (var dlg = new ThreeButtonDialog(
                                new ThreeButtonDialog.Details(null, blurb, Messages.WLB_PROMPT_FOR_MODE_CHANGE_CAPTION),
                                ThreeButtonDialog.ButtonYes,
                                ThreeButtonDialog.ButtonNo))
                            {
                                drModeCheck = dlg.ShowDialog(this);
                            }

                            if (drModeCheck == DialogResult.Yes)
                            {
                                _wlbPoolConfiguration.PerformanceMode = scheduledPerfMode;
                            }
                        }
                    }
                    SaveWLBConfig(_wlbPoolConfiguration);
                }
            }
            catch (Exception ex)
            {
                log.Debug("Unable to open the WLB configuration dialog.", ex);
                return;
            }

            if (!(WlbServerState.GetState(_pool) == WlbServerState.ServerState.NotConfigured))
            {
                RetrieveConfiguration();
            }
        }

        private string getPoolWLBStatus(Pool pool)
        {
            return String.Format(Messages.WLB_RECONFIGURE, String.IsNullOrEmpty(WlbServerState.GetFailureMessage(_pool)) ? string.Empty : WlbServerState.GetFailureMessage(_pool));
        }

        private string getPoolWLBServer(Pool pool)
        {
            if (Helpers.WlbConfigured(pool.Connection))
            {
                return pool.wlb_url;
            }
            return Messages.NONE_DEFINED;
        }

        private string getPoolOptMode(Pool pool)
        {
            string optMode = Messages.UNKNOWN;
            if (null != _wlbPoolConfiguration && Helpers.WlbConfigured(pool.Connection))
            {
                if (_wlbPoolConfiguration.AutomateOptimizationMode)
                {
                    optMode = string.Format(Messages.WLB_SCHEDULED_OPT_MODE, getOptModeText(_wlbPoolConfiguration.PerformanceMode));
                }
                else
                {
                    optMode = getOptModeText(_wlbPoolConfiguration.PerformanceMode);
                }
            }
            return optMode;
        }

        private string getOptModeText(WlbPoolPerformanceMode mode)
        {
            return (mode == WlbPoolPerformanceMode.MaximizePerformance) ? Messages.WLB_OPT_MODE_MAXIMIZEPERFORMANCE : Messages.WLB_OPT_MODE_MAXIMIZEDENSITY;
        }

        private string getNextOptModeTask(Pool pool)
        {
            //figure out the next task and return a nice string for display
            WlbScheduledTask nextTask = _wlbPoolConfiguration.ScheduledTasks.GetNextExecutingTask();
            WlbScheduledTask.WlbTaskDaysOfWeek dayOfWeek;
            DateTime executeTime;

            WlbScheduledTask.GetLocalTaskTimes(nextTask.DaysOfWeek, nextTask.ExecuteTime, out dayOfWeek, out executeTime);

            string localDayOfWeek = WlbScheduledTask.DaysOfWeekL10N(dayOfWeek);

            return string.Format(Messages.WLB_NEXT_OPT_MODE_SCHEDULE_FORMAT, localDayOfWeek, HelpersGUI.DateTimeToString(executeTime, Messages.DATEFORMAT_HM, true));
        }
        
        private string getAutoOptimization(Pool pool)
        {
            if (null != _wlbPoolConfiguration && Helpers.WlbConfigured(pool.Connection))
            {
                return _wlbPoolConfiguration.AutoBalanceEnabled ? Messages.YES : Messages.NO;
            }
            return Messages.UNKNOWN;
        }

        private string getPoolWlbPowerManagement(Pool pool)
        {
            if (null != _wlbPoolConfiguration && Helpers.WlbConfigured(pool.Connection))
            {
                return _wlbPoolConfiguration.PowerManagementEnabled ? Messages.YES : Messages.NO;
            }
            return Messages.UNKNOWN;
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (Helpers.WlbConfigured(_pool.Connection) && 
                !(WlbServerState.GetState(_pool) == WlbServerState.ServerState.ConnectionError ||
                  WlbServerState.GetState(_pool) == WlbServerState.ServerState.NotConfigured))
            {
                return;
            }

            DisableButtons();
            WlbCredentialsDialog wlbCredentialsDialog = new WlbCredentialsDialog(_pool);
            DialogResult dr = wlbCredentialsDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                EnableWLB();
            }
            else
            {
                SetButtonState();
            }

        }

        private void buttonReports_Click(object sender, EventArgs e)
        {
            ViewWorkloadReportsCommand viewWorkloadReportsCommand = new ViewWorkloadReportsCommand(Program.MainWindow, _pool);
            viewWorkloadReportsCommand.Execute();
        }

        #endregion
    }
}

