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
using System.Threading;
using System.Windows.Forms;

using XenAdmin;
using XenAdmin.Controls;
using XenAdmin.Controls.CustomGridView;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAdmin.Network;
using XenAdmin.XenSearch;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Actions.Wlb;
using XenAdmin.Dialogs;
using System.Drawing.Drawing2D;
using XenAdmin.Commands;


namespace XenAdmin.Controls.Wlb
{
    public partial class WlbOptimizePool : UserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ListViewColumnSorter columnSorter = new ListViewColumnSorter();

        private IXenObject _xenObject;
        private Pool _pool;
        private Dictionary<VM, WlbOptimizationRecommendation> _vmOptList = new Dictionary<VM, WlbOptimizationRecommendation>();
        private Dictionary<XenRef<VM>, string[]> _recommendations = new Dictionary<XenRef<VM>, string[]>();
        private string _optId = String.Empty;
        private bool _autoOptEnabled = false;
        private bool _powerManagementEnabled = false;

        // TODO: change to proper report file name
        private readonly string ReportFile = "pool_optimization_history";

        // All used in the listview.
        private readonly Font OperationTextFont;
        private readonly Color OperationTextColor;
        private readonly Color ShadedRowColor;
        private readonly Color DisabledShadedRowColor;
        
        // Listview minimum column widths
        private int[] minimumColumnWidths = { 0, 25, 25, 25 };

        /// <summary>
        /// Initialize optimize pool listview
        /// </summary>
        public WlbOptimizePool()
        {
            InitializeComponent();
            Host_CollectionChangedWithInvoke += Program.ProgramInvokeHandler(Host_CollectionChanged);
            VM_CollectionChangedWithInvoke=Program.ProgramInvokeHandler(VM_CollectionChanged);
            OperationTextFont = new Font(this.Font, FontStyle.Bold);
            OperationTextColor = Color.Black;
            ShadedRowColor = Color.FromArgb(230, 240, 255);
            DisabledShadedRowColor = Color.FromArgb(224, 222, 205);

            optimizePoolListView.SmallImageList = Images.ImageList16;
            optimizePoolListView.ListViewItemSorter = columnSorter;

            //linkLabelReportHistory.Visible = false;

            recommendationUpdateTimer = new System.Threading.Timer(TimerCallback, null, 0, 0);
            StartTimer();
        }

        #region public methods

        /// <summary>
        /// Set XenModelOpject and populate optimize pool listview properly
        /// </summary>
        public IXenObject XenObject
        {
            set
            {
                Program.AssertOnEventThread();

                if (_xenObject != null)
                {
                    if (_xenObject is Pool)
                    {
                        this._pool.PropertyChanged -= Pool_PropertyChanged;

                        _pool.Connection.Cache.DeregisterCollectionChanged<XenAPI.Host>(Host_CollectionChangedWithInvoke);
                        foreach (Host host in _pool.Connection.Cache.Hosts)
                        {
                            host.PropertyChanged -= Host_PropertyChanged;

                            host.Connection.Cache.DeregisterCollectionChanged<VM>(VM_CollectionChangedWithInvoke);
                            foreach (VM vm in host.Connection.Cache.VMs)
                            {
                                vm.PropertyChanged -= VM_PropertyChanged;
                            }
                        }
                    }
                }

                _xenObject = value;
                _pool = null;

                if (_xenObject != null)
                {
                    if (_xenObject is Pool)
                    {
                        _pool = (Pool)_xenObject;

                        this._pool.PropertyChanged += Pool_PropertyChanged;

                        _pool.Connection.Cache.RegisterCollectionChanged<XenAPI.Host>(Host_CollectionChangedWithInvoke);
                        foreach (Host host in _pool.Connection.Cache.Hosts)
                        {
                            host.PropertyChanged -= Host_PropertyChanged;
                            host.PropertyChanged += Host_PropertyChanged;

                            host.Connection.Cache.RegisterCollectionChanged<VM>(VM_CollectionChangedWithInvoke);
                            foreach (VM vm in host.Connection.Cache.VMs)
                            {
                                vm.PropertyChanged -= VM_PropertyChanged;
                                vm.PropertyChanged += VM_PropertyChanged;
                            }
                        }
                    }
                }

                BuildRecList();
            }
        }

        public void SetOptControlProperties(bool autoOptEnabled, bool powerManagementEnabled)
        {
            _autoOptEnabled = autoOptEnabled;
            _powerManagementEnabled = powerManagementEnabled;

            this.SuspendLayout();
            if (WlbServerState.GetState(_pool) == WlbServerState.ServerState.Enabled)
            {
                //linkLabelReportHistory.Visible = true;

                // Update listView VM/Host header text
                this.optimizePoolListView.BeginUpdate();
                this.columnHeader1.Text = Messages.WLB_OPT_HEADER_VMHOST;
                this.optimizePoolListView.EndUpdate();
            }
            else
            {
                //linkLabelReportHistory.Visible = false;

                // Update listView VM/Host header text
                this.optimizePoolListView.BeginUpdate();
                this.columnHeader1.Text = Messages.WLB_OPT_HEADER_VM;
                this.optimizePoolListView.EndUpdate();
            }

            this.ResumeLayout();
        }
        
        /// <summary>
        /// Called from wlb page, used to disable/enable optimize pool controls properly when WLB is Enabled/Disabled
        /// </summary>
        /// <param name="disable">disable controls if it's true, else enable controls</param>
        public void WLBOptDisable(bool disable)
        {
            optimizePoolListView.BeginUpdate();
            if (disable)
            {
                this.statusLabel.Visible = false;
                this.optimizePoolListView.Items.Clear();
                this.optimizePoolListView.Visible = true;
                this.optimizePoolListView.Enabled = false;
                this.applyButton.Visible = true;
                this.applyButton.Enabled = false;
            }
            else
            {
                if (!this.optimizePoolListView.Visible)
                {
                    this.statusLabel.Visible = true;
                    this.optimizePoolListView.Items.Clear();
                    this.optimizePoolListView.Visible = true;
                    this.applyButton.Visible = true;
                    this.applyButton.Enabled = false;
                }
                else
                {
                    this.optimizePoolListView.Enabled = true;
                    if (this.optimizePoolListView.Items.Count > 0)
                        this.applyButton.Enabled = true;
                    else
                        this.statusLabel.Visible = true;
                }
            }
            optimizePoolListView.EndUpdate();
        }
        #endregion
        
        // TODO: remove unused event handler
        #region Event Handlers

        // draw the gradient line, override OnPaint in System.Drawing.Drawing2D
        /*
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Brush brush = new LinearGradientBrush(Point.Empty, new Point(this.Width / 2, 0), XenAdmin.TabPages.BaseTabPage.HeaderBorderColor, BackColor))
            {
                e.Graphics.FillRectangle(brush, 0, 20, this.Width / 2, 1);
            }
        }
        */

        /// <summary>
        /// Triggered when pool wlb enabled/disabled and optimize pool
        /// </summary>
        /// <param name="sender">XenObject&lt;Pool&gt;</param>
        /// <param name="e">PropertyChangedEventArgs</param>
        private void Pool_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Program.Invoke(this, delegate
            {
                if (_xenObject != null && Program.MainWindow.TheTabControl.SelectedTab == Program.MainWindow.TabPageWLB)
                {
                    if (e.PropertyName == "wlb_enabled")
                    {
                        // enable/disble controls and display message properly
                        WLBOptDisable(!Helpers.WlbEnabled(((Pool)sender).Connection));
                    }

                    if (e.PropertyName == "other_config")
                    {
                        // check whether it's optimize pool event
                        if (((Pool)sender).other_config.ContainsKey(WlbOptimizationRecommendation.OPTIMIZINGPOOL))
                        {
                            BuildRecList();
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Triggered when hosts leave/join pool
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">CollectionChangeEventArgs</param>
        private void Host_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    ((Host)e.Element).PropertyChanged -= Host_PropertyChanged;
                    ((Host)e.Element).PropertyChanged += Host_PropertyChanged;
                    break;
                case CollectionChangeAction.Remove:
                    ((Host)e.Element).PropertyChanged -= Host_PropertyChanged;
                    break;
            }
            // It's not necessary to call BuildRecList here
            // because Remove/add Hosts also triger VM_CollectionChange, 
        }

        /// <summary>
        /// respond to host name change and vms leaving/joining the host.
        /// </summary>
        /// <param name="sender">XenObject&lt;Host&gt;</param>
        /// <param name="e">PropertyChangedEventArgs</param>      
        private void Host_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_pool != null && 
                Helpers.WlbEnabled(_pool.Connection) && 
                applyButton.Enabled && 
                IsHostOnListView(((Host)sender), false))
            {
                if (e.PropertyName == "name_label")
                {
                    optimizePoolListView.BeginUpdate();
                    foreach (ListViewItem item in optimizePoolListView.Items)
                    {
                        UpdateRow(item);
                    }
                    optimizePoolListView.EndUpdate();
                }

                if ((e.PropertyName == "resident_VMs" && IsHostOnListView(((Host)sender), true)) 
                    || (e.PropertyName == "enabled" && !((Host)sender).enabled))
                {
                    Program.Invoke(this, delegate()
                     {
                         BuildRecList();
                     });
                }
            }
        }

        /// <summary>
        /// Check whether Host property change relates to optimize pool list view. Will return true if the host is on the list view.
        /// Must be called on the event thread.
        /// </summary>
        /// <param name="host">XenObject&lt;Host&gt;</param>
        /// <param name="checkVMHost">Whether to check VM host</param>
        /// <returns>True if host is on the optimize pool list view, else return false</returns>
        private bool IsHostOnListView(Host host, bool checkVMHost)
        {
            Program.AssertOnEventThread();
            
            bool onList = false;

            // should find if any vms that not on host but in listview
            foreach (ListViewItem item in optimizePoolListView.Items)
            {
                Host fromHost = ((WlbOptimizationRecommendation)item.Tag).fromHost;
                Host toHost = ((WlbOptimizationRecommendation)item.Tag).toHost;
                VM vm = ((WlbOptimizationRecommendation)item.Tag).vm;

                // whether check vm's host changes
                if (checkVMHost)
                {   
                    // update optimize pool listview if vm from host is to move to host
                    if (vm.Connection.Resolve(vm.resident_on) == toHost)
                    {
                        onList = true;
                    }
                }
                // whether host is related to optimize pool listview
                else if ((fromHost != null && fromHost == host) 
                        || (toHost != null && toHost== host))
                {
                    onList = true;
                }
            }
            return onList;
        }

        /// <summary>
        /// Triggered when vm collection is changed, pool is connected and optimize pool listview is visiable.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">CollectionChangeEventArgs</param>
        private void VM_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.AssertOnEventThread();

            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    ((VM)e.Element).PropertyChanged -= VM_PropertyChanged;
                    ((VM)e.Element).PropertyChanged += VM_PropertyChanged;
                    break;
                case CollectionChangeAction.Remove:
                    ((VM)e.Element).PropertyChanged -= VM_PropertyChanged;
                    break;
            }

            // only continue if pool is connect and pool is not null
            if (_xenObject == null || !_xenObject.Connection.IsConnected)
                    return;

            // only update recommendation listview if applyButton is enabled and a vm has been removed from collection
            if (applyButton.Enabled && e.Action == CollectionChangeAction.Remove)
                BuildRecList();
        }


        /// <summary>
        /// Triggered when vm state is changed
        /// </summary>
        /// <param name="sender">XenObject&lt;VM&gt;</param>
        /// <param name="e">PropertyChangedEventArgs</param>
        private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Program.Invoke(Program.MainWindow, () =>
                                                   {

                                                       // Find row for VM
                                                       ListViewItem item = FindItemFromVM((VM) sender);
                                                       if (item != null)
                                                       {
                                                           optimizePoolListView.BeginUpdate();
                                                           try
                                                           {
                                                               UpdateRow(item);
                                                           }
                                                           finally
                                                           {
                                                               optimizePoolListView.EndUpdate();
                                                           }
                                                       }
                                                   });
        }

        /// <summary>
        /// Finds the ListViewItem for the given VM. Will return null if no corresponding item could be found.
        /// Must be called on the event thread.
        /// </summary>
        /// <param name="vm">XenObject&lt;VM&gt;</param>
        /// <returns>Return the listview item when found, null if nothing is found</returns>
        private ListViewItem FindItemFromVM(VM vm)
        {
            Program.AssertOnEventThread();

            foreach (ListViewItem item in optimizePoolListView.Items)
            {
                if (((WlbOptimizationRecommendation)item.Tag).vm == vm)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Apply optimization to a pool
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        private void ButtonApply_Click(object sender, EventArgs e)
        {
            if (this._pool == null)
                return;

            applyButton.Enabled = false;

            action = HelpersGUI.FindActiveOptAction(_xenObject.Connection);
            if ((action != null && action.GetType() != typeof(WlbOptimizePoolAction)) || action == null)
            {
                new WlbOptimizePoolAction(_pool, _vmOptList, _optId).RunAsync();
                Program.MainWindow.UpdateToolbars();
            }
        }

        private void linkLabelReportHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new ViewWorkloadReportsCommand(Program.MainWindow, _pool, ReportFile, true).Execute();
        }

        private static void UpdateRow(ListViewItem row)
        {

            try  // In try block to fix CA-40197
            {

                Program.AssertOnEventThread();

                WlbOptimizationRecommendation optVMmSetting = (WlbOptimizationRecommendation)row.Tag;

                // update icon and vm/host name
                //row.ImageIndex = (int)Images.GetIconFor(optVMmSetting.vm);
                //row.SubItems[0].Text = optVMmSetting.vm.name_label;
                //row.SubItems[1].Text = optVMmSetting.fromHost.name_label;
                //row.SubItems[2].Text = optVMmSetting.toHost.name_label;
                if (String.IsNullOrEmpty(optVMmSetting.toHost.name_label) || String.IsNullOrEmpty(optVMmSetting.fromHost.name_label))
                {
                    row.ImageIndex = (int)Images.GetIconFor(optVMmSetting.toHost ?? optVMmSetting.fromHost);
                    row.SubItems[1].Text = Helpers.GetName(optVMmSetting.toHost ?? optVMmSetting.fromHost);
                    row.SubItems[2].Text = optVMmSetting.toHost != null ? Messages.WLB_OPT_OPERATION_HOST_POWERON : Messages.WLB_OPT_REASON_POWEROFF;
                }
                else
                {
                    row.ImageIndex = (int)Images.GetIconFor(optVMmSetting.vm);
                    row.SubItems[1].Text = optVMmSetting.vm.name_label;
                    row.SubItems[2].Text = String.Format(Messages.WLB_OPT_OPERATION_VM, optVMmSetting.fromHost.name_label, optVMmSetting.toHost.name_label);
                }

                row.SubItems[3].Text = optVMmSetting.reason;
            }
            catch(Exception ex)
            {
                log.Error(ex, ex);
            }
        }
        #endregion

        #region Refresh Timer
        private System.Threading.Timer recommendationUpdateTimer;
        private const int RECOMMENDATION_INITIAL_INTERVAL = 1000 * 5; // 5 seconds interval
        // Default to 2 minutes due to Kirkwood recommendation is generated every 2 minutes
        private const int RECOMMENDATION_UPDATE_INTERVAL = 1000 * 30; // 30 seconds interval
        AsyncAction action = null;

        /// <summary>
        /// start timer with 5 seconds interval
        /// </summary>
        private void StartTimer()
        {
            recommendationUpdateTimer.Change(RECOMMENDATION_INITIAL_INTERVAL, 0);
        }

        /// <summary>
        /// Timer call back, used to refresh optimize pool listview. 
        /// Dispose if program exits, do nothing if wlb tab is not focused
        /// </summary>
        /// <param name="o">object</param>
        private void TimerCallback(object o)
        {
            if (recommendationUpdateTimer == null)
                return;

           if (Program.Exiting)
           {
                recommendationUpdateTimer.Dispose();
                return;
           }

           if (Program.MainWindow != null && Program.MainWindow.WlbPage != null)
           {
                Program.Invoke(Program.MainWindow, delegate()
                {
                    if (_xenObject != null && Program.MainWindow.TheTabControl.SelectedTab == Program.MainWindow.TabPageWLB
                        && _pool != null &&_pool.Connection.IsConnected && _pool.Connection.Session != null)
                    {
                        BuildRecList();
                    }
                });
            }
            recommendationUpdateTimer.Change(RECOMMENDATION_UPDATE_INTERVAL, 0);
        }
        #endregion


        #region load optimizePoolListView properly
        /// <summary>
        /// Build optimize pool list view properly
        /// </summary>
        private void BuildRecList()
        {
            Program.AssertOnEventThread();

            if(_xenObject == null)
                return;

            if (Helpers.WlbEnabled(_xenObject.Connection))
            {
                try
                {
                    if (_xenObject is Pool)
                    {
                        _pool = (Pool)_xenObject;

                        // get any active WLB action
                        action = HelpersGUI.FindActiveWLBAction(_xenObject.Connection);

                        // make sure we are not initializing, starting or stopping WLB
                        if (action is DisableWLBAction || action is EnableWLBAction || action is InitializeWLBAction)
                            return;

                        optimizePoolListView.BeginUpdate();

                        // check whether optimize pool is running before load optimize pool listview
                        if ((action != null && action is WlbOptimizePoolAction)
                            || (action == null && _pool.other_config.ContainsKey(WlbOptimizationRecommendation.OPTIMIZINGPOOL)
                                && _pool.other_config[WlbOptimizationRecommendation.OPTIMIZINGPOOL] == Messages.IN_PROGRESS))
                        {
                            //statusLabel.Text = Messages.WLB_OPT_OPTIMIZING;
                            this.applyButton.Text = Messages.WLB_OPT_OPTIMIZING;
                            EnableControls(false, false);
                        }
                        else if (action == null || (action != null && action.GetType() != typeof(WlbRetrieveRecommendationAction)))
                        {
                            this.applyButton.Text = Messages.WLB_OPT_APPLY_RECOMMENDATIONS;
                            // retrieve recommendations, and load optimize pool listview properly
                            WlbRetrieveRecommendationAction optAction = new WlbRetrieveRecommendationAction(_pool);
                            optAction.Completed += this.OptRecRetrieveAction_Completed;
                            optAction.RunAsync();
                        }
                    }
                }
                catch (Failure f)
                {
                    statusLabel.Text = Messages.WLB_OPT_LOADING_ERROR;
                    log.Error(f, f);
                }
                catch (Exception e)
                {
                    statusLabel.Text = Messages.WLB_OPT_LOADING_ERROR;
                    log.Error($"There was an error calling retrieve_wlb_recommendations on pool {_pool.name_label}.", e);
                }

                finally
                {
                    optimizePoolListView.EndUpdate();
                }
            }
            else
            {
                this.WLBOptDisable(true);
            }
        }

        /// <summary>
        /// Retrieve wlb optimize pool recommendation action complete handler. 
        /// Populate optimize pool listview and enable controls properly.
        /// </summary>
        protected void OptRecRetrieveAction_Completed(ActionBase sender)
        {
            AsyncAction action = (AsyncAction)sender;
            if (action.IsCompleted)
            {
                action.Completed -= OptRecRetrieveAction_Completed;

                if (action is WlbRetrieveRecommendationAction)
                {
                    WlbRetrieveRecommendationAction thisAction = (WlbRetrieveRecommendationAction)action;
                    _recommendations = thisAction.WLBOptPoolRecommendations;
                    if (_recommendations != null && IsGoodRecommendation(_recommendations) && _xenObject.Connection == action.Connection)
                    {
                        Program.Invoke(this, delegate()
                        {
                            PopulateData(_recommendations);

                            // In case optimizePoolListView is empty
                            if (optimizePoolListView.Items.Count == 0)
                            {
                                statusLabel.Text = Messages.WLB_OPT_POOL_NO_RECOMMENDATION;
                                EnableControls(true, false);
                            }
                            else
                                EnableControls(false, true);
                        });
                    }
                    else
                    {
                        Program.Invoke(this, delegate()
                        {
                            statusLabel.Text = Messages.WLB_OPT_POOL_NO_RECOMMENDATION;
                            EnableControls(true, false);
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Check if there are recommendations and verify if recommendations are valid and good;
        /// return false if no valid recommendations
        /// </summary>
        /// <param name="recommendations">Optimize pool recommendations</param>
        /// <returns>true if recommendations are valid, else return false</returns>
        private bool IsGoodRecommendation(Dictionary<XenRef<VM>, string[]> recommendations)
        {
            bool goodRec = false;

            if (recommendations != null)
            {
                foreach (KeyValuePair<XenRef<VM>, string[]> rec in recommendations)
                {
                    // make sure at least one recommendation is good
                    if (rec.Value[(int)RecProperties.WLB].Trim().ToLower() == "wlb")
                    {
                        // get opt id from new recommendations
                        _optId = rec.Value[(int)RecProperties.OptId];
                        
                        // get opt id of the most recent optimized action from pool's other configuration property
                        string optimizeId = String.Empty;
                        (Helpers.GetOtherConfig(this._pool)).TryGetValue(WlbOptimizationRecommendation.OPTIMIZINGPOOL, out optimizeId);
                        
                        // make sure pool's recommendations are not old/just optimized recommendations 
                        if (optimizeId != _optId)
                        {
                            goodRec = true;
                            break;
                        }
                    }
                }
            }

            return goodRec;
        }

        /// <summary>
        /// Checks to see if all the wlb recommendations are for PowerOff or PowerOn operations
        /// </summary>
        /// <param name="recommendations"></param>
        /// <returns></returns>
        private bool IsPowerOnlyRecommendation(Dictionary<XenRef<VM>, string[]> recommendations)
        {
            bool powerOnly = true;

            if (recommendations != null)
            {
                foreach (KeyValuePair<XenRef<VM>, string[]> rec in recommendations)
                {
                    if (rec.Value[(int)RecProperties.WLB].Trim().ToLower() == "wlb" &&
                        !(rec.Value[(int)RecProperties.Reason].Trim().ToLower().StartsWith("power")))
                    {
                        powerOnly = false;
                    }
                }
            }

            return powerOnly;
        }

        /// <summary>
        /// Populate optimize pool listview properly
        /// </summary>
        /// <param name="recommendations">optimize pool recommendations</param>
        private void PopulateData(Dictionary<XenRef<VM>, string[]> recommendations)
        {
            Program.AssertOnEventThread();

            optimizePoolListView.BeginUpdate();
            try
            {
                if (_vmOptList != null)
                    _vmOptList.Clear();

                optimizePoolListView.Items.Clear();
                List<ListViewItem> newItems = new List<ListViewItem>();

                WlbOptimizationRecommendationCollection sortedRecommendations = new WlbOptimizationRecommendationCollection(_pool, recommendations);

                // WLB: recommendation return string format ["WLB"; vm/vm dom0; optId; recID; reason]

                foreach (WlbOptimizationRecommendation optVmSetting in sortedRecommendations)
                {
                    _optId = optVmSetting.optId.ToString();
                    _vmOptList.Add(optVmSetting.vm, optVmSetting);

                    ListViewItem row = new ListViewItem();
                    row.Tag = optVmSetting; 
                    newItems.Add(row);

                    row.SubItems.Add(new ListViewItem.ListViewSubItem());
                    row.SubItems.Add(new ListViewItem.ListViewSubItem());
                    row.SubItems.Add(new ListViewItem.ListViewSubItem());
                    row.SubItems.Add(new ListViewItem.ListViewSubItem());

                    // Power on/off hosts
                    if (optVmSetting.toHost == null)
                    {
                        row.ImageIndex = (int)Images.GetIconFor(optVmSetting.toHost != null ? optVmSetting.toHost : optVmSetting.fromHost);
                        row.SubItems[1].Text = Helpers.GetName(optVmSetting.toHost != null ? optVmSetting.toHost : optVmSetting.fromHost);
                        row.SubItems[2].Text = optVmSetting.powerOperation; 
                    }
                    else // VMs
                    {
                        row.ImageIndex = (int)Images.GetIconFor(optVmSetting.vm);
                        row.SubItems[1].Text = Helpers.GetName(optVmSetting.vm);
                        row.SubItems[2].Text = String.Format(Messages.WLB_OPT_OPERATION_VM, Helpers.GetName(optVmSetting.fromHost), Helpers.GetName(optVmSetting.toHost));
                    }
                    row.SubItems[3].Text = optVmSetting.reason;
                }

                optimizePoolListView.Items.AddRange(newItems.ToArray());
            }
            finally
            {
                optimizePoolListView.EndUpdate();
            }
        }

        /// <summary>
        /// Enable optimize pool controls properly
        /// </summary>
        private void EnableControls(bool enableLabel, bool enableButton)
        {
            // Rules for enabling the button:
            //  Passed RBAC checks, and
            //  If we are not in automatic mode, or
            //  (   We are in automatic mode, and 
            //      We are in automated mode but PowerManagement is not enabled, and
            //      All recommdations are power-related)

            if (enableLabel)
            {
                optimizePoolListView.Items.Clear();
            }
            optimizePoolListView.Visible = true;
            optimizePoolListView.Enabled = PassedRbacChecks();
            
            statusLabel.Visible = enableLabel;
            
            applyButton.Visible = true;
            //if ((_autoOptEnabled && _powerManagementEnabled) || !PassedRbacChecks())
            if (PassedRbacChecks() &&
                (!_autoOptEnabled || 
                 (_autoOptEnabled && !_powerManagementEnabled && IsPowerOnlyRecommendation(_recommendations))))
            {
                applyButton.Enabled = enableButton;
            }
            else
            {
                applyButton.Enabled = false;
            }

        }

        private RbacMethodList WLB_PERMISSION_CHECKS = new RbacMethodList(
            "pool.initialize_wlb",
            "pool.set_wlb_enabled",
            "pool.send_wlb_configuration"
            );

        private readonly CollectionChangeEventHandler Host_CollectionChangedWithInvoke;
        private readonly CollectionChangeEventHandler VM_CollectionChangedWithInvoke;

        private bool PassedRbacChecks()
        {
            return Role.CanPerform(WLB_PERMISSION_CHECKS, this._pool.Connection);
        }
        #endregion


        #region OptimizePoolListView item drawing event and colum event handlers
        /// <summary>
        /// Draw default column header
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">DrawListViewColumnHeaderEventArgs</param>
        private void OptimizePoolListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            // Draw default column header
            e.DrawDefault = true;
        }

        /// <summary>
        /// Draw items on optimize pool listview properly
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">DrawListViewItemEventArgs</param>
        private void OptimizePoolListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            // Fill the full width of the control, not just to the end of the column headers.
            Rectangle r = e.Bounds;
            int i = 0;
            foreach (ColumnHeader col in optimizePoolListView.Columns)
            {
                i += col.Width;
            }
            // Fill from the right of the rightmost item to the end of the control
            r.Width = Math.Max(i, optimizePoolListView.ClientSize.Width);
            r.X = i;

            using (Brush backBrush = new SolidBrush(e.Item.Index % 2 == 1 ? ShadedRowColor : SystemColors.Window))
                e.Graphics.FillRectangle(backBrush, r);
        }

        /// <summary>
        /// Draw sub items in optimize pool list view
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">DrawListViewSubItemEventArgs</param>
        private void OptimizePoolListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            Rectangle rect = e.Bounds;
            using (Brush backBrush = new SolidBrush(e.Item.Index % 2 == 1 ? ShadedRowColor : SystemColors.Window))
                e.Graphics.FillRectangle(backBrush, rect);

            Color fontColor = Color.Black;

            if (e.ColumnIndex == 2)
            {
                TextRenderer.DrawText(e.Graphics, e.Item.SubItems[e.ColumnIndex].Text, OperationTextFont, e.Bounds, OperationTextColor, TextFormatFlags.EndEllipsis);
            }
            else if (e.ColumnIndex == 3)
            {
                TextRenderer.DrawText(e.Graphics, e.Item.SubItems[e.ColumnIndex].Text, this.Font, e.Bounds, fontColor, TextFormatFlags.EndEllipsis);
            }
            else
            {
                DrawSubItem1(fontColor, e);
            }
        }


        private void DrawSubItem1(Color fontColor, DrawListViewSubItemEventArgs e)
        {
            Image icon = e.Item.ImageList.Images[e.Item.ImageIndex];
            Point p = new Point(e.Bounds.Left, e.Bounds.Top + (e.Bounds.Height - icon.Height) / 2);

            // Draw normally
            e.Graphics.DrawImageUnscaled(icon, p);

            Point p2 = new Point(p.X + icon.Width + 2, p.Y);
            Rectangle r = new Rectangle(p2.X, p2.Y, e.Bounds.Right - p2.X, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, e.Item.SubItems[e.ColumnIndex].Text, this.Font, r, fontColor, TextFormatFlags.EndEllipsis);
        }

        private void optimizePoolListView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            //Maintain a minimum column widths


            if (minimumColumnWidths.Length>=e.ColumnIndex &&
                optimizePoolListView.Columns[e.ColumnIndex].Width < minimumColumnWidths[e.ColumnIndex])
            {
                optimizePoolListView.Columns[e.ColumnIndex].Width = minimumColumnWidths[e.ColumnIndex];
            }
        }
        #endregion    



    }
}
