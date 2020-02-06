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
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Controls.CustomDataGraph;
using XenAdmin.Dialogs;
using XenAdmin.Actions;


namespace XenAdmin.TabPages
{
    public partial class PerformancePage : BaseTabPage
    {
        private IXenObject _xenObject;
        private bool _disposed;
        private readonly CollectionChangeEventHandler Message_CollectionChangedWithInvoke;
        private readonly ArchiveMaintainer ArchiveMaintainer = new ArchiveMaintainer();

        public PerformancePage()
        {
            InitializeComponent();

            ArchiveMaintainer.ArchivesUpdated += ArchiveMaintainer_ArchivesUpdated;
            Message_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(Message_CollectionChanged);
            GraphList.ArchiveMaintainer = ArchiveMaintainer;
            GraphList.SelectedGraphChanged += GraphList_SelectedGraphChanged;
            GraphList.MouseDown += GraphList_MouseDown;
            DataPlotNav.ArchiveMaintainer = ArchiveMaintainer;
            this.DataEventList.SetPlotNav(this.DataPlotNav);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //set text colour for gradient bar
            EventsLabel.ForeColor = Program.TitleBarForeColor;
            base.Text = Messages.PERFORMANCE_TAB_TITLE;
            UpdateMoveButtons();
        }

        public override string HelpID => "TabPagePerformance";

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            
            ArchiveMaintainer.Stop();

            if (disposing)
            {
                ArchiveMaintainer.ArchivesUpdated -= ArchiveMaintainer_ArchivesUpdated;

                if (components != null)
                    components.Dispose();

                _disposed = true;
            }
            base.Dispose(disposing);
        }

        private void GraphList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip.Show(GraphList, e.X, e.Y);
            }
        }

        private bool CanEnableGraphButtons()
        {
            return XenObject != null && GraphList.SelectedGraph != null;
        }

        private void UpdateMoveButtons()
        {
            bool canEnable = CanEnableGraphButtons();
            moveUpButton.Enabled = canEnable && GraphList.SelectedGraphIndex != 0;
            moveUpToolStripMenuItem.Enabled = moveUpButton.Enabled;

            moveDownButton.Enabled = canEnable && GraphList.SelectedGraphIndex != GraphList.Count - 1;
            moveDownToolStripMenuItem.Enabled = moveDownButton.Enabled;
        }

        private void UpdateActionButtons()
        {
            bool canEnable = CanEnableGraphButtons();

            VM vm = XenObject as VM;
            bool isRunning = XenObject != null && (vm != null)
                                 ? vm.current_operations.Count == 0 && vm.IsRunning()
                                 : (XenObject is Host);
            
            string newText = (vm != null && !isRunning)
                                 ? string.Format(Messages.GRAPHS_CANNOT_ADD_VM_HALTED, vm.Name())
                                 : string.Empty;

            newGraphToolStripMenuItem.Enabled = isRunning;
            newGraphToolStripMenuItem.ToolTipText = newText;

            newGraphToolStripContextMenuItem.Enabled = isRunning;
            newGraphToolStripContextMenuItem.ToolTipText = newText;

            string editText=(vm != null && !isRunning)
                                 ? string.Format(Messages.GRAPHS_CANNOT_EDIT_VM_HALTED, vm.Name())
                                 : string.Empty;

            editGraphToolStripMenuItem.Enabled = canEnable && isRunning;
            editGraphToolStripMenuItem.ToolTipText = editText;

            editGraphToolStripContextMenuItem.Enabled = canEnable && isRunning;
            editGraphToolStripContextMenuItem.ToolTipText = editText;

            deleteGraphToolStripMenuItem.Enabled = canEnable && GraphList.Count > 1;
            deleteGraphToolStripContextMenuItem.Enabled = canEnable && GraphList.Count > 1;

            restoreDefaultGraphsToolStripMenuItem.Enabled = XenObject != null && !GraphList.ShowingDefaultGraphs;
            restoreDefaultGraphsToolStripContextMenuItem.Enabled = XenObject != null && !GraphList.ShowingDefaultGraphs;
        }

        private void UpdateAllButtons()
        {
            UpdateMoveButtons();
            UpdateActionButtons();
        }

        private void GraphList_SelectedGraphChanged()
        {
            UpdateAllButtons();
        }

        public IXenObject XenObject
        {
            private get
            {
                return _xenObject;
            }
            set
            {
                ArchiveMaintainer.Pause();
                DataEventList.Clear();

                DeregEvents();
                _xenObject = value;
                RegEvents();

                ArchiveMaintainer.XenObject = value;

                if (_xenObject != null)
                {
                    GraphList.LoadGraphs(XenObject);
                    LoadEvents();
                    ArchiveMaintainer.Start(); 
                }
                RefreshAll();
            }
        }

        private bool FeatureForbidden
        {
            get { return Helpers.FeatureForbidden(XenObject, Host.RestrictPerformanceGraphs); }
        }

        private void LoadEvents()
        {
            foreach(XenAPI.Message m in XenObject.Connection.Cache.Messages)
            {
                CheckMessage(m, CollectionChangeAction.Add);    
            }
        }

        private void CheckMessage(XenAPI.Message m, CollectionChangeAction a)
        {
            if (!m.ShowOnGraphs() || m.cls != cls.VM)
                return;

            Host h = XenObject as Host;
            if (h != null)
            {
                List<VM> resVMs = h.Connection.ResolveAll<VM>(h.resident_VMs);
                foreach (VM v in resVMs)
                {
                    if (v.uuid == m.obj_uuid)
                    {
                        if (a == CollectionChangeAction.Add)
                            DataEventList.AddEvent(new DataEvent(m.timestamp.ToLocalTime().Ticks, 0, m));
                        else
                            DataEventList.RemoveEvent(new DataEvent(m.timestamp.ToLocalTime().Ticks, 0, m));

                        break;
                    }
                }

            }
            else if (XenObject is VM)
            {
                if (m.obj_uuid != Helpers.GetUuid(XenObject))
                    return;

                if (a == CollectionChangeAction.Add)
                    DataEventList.AddEvent(new DataEvent(m.timestamp.ToLocalTime().Ticks, 0, m));
                else
                    DataEventList.RemoveEvent(new DataEvent(m.timestamp.ToLocalTime().Ticks, 0, m));
            }
        }

        private void RegEvents()
        {
            if (XenObject == null)
                return;

            Pool pool = Helpers.GetPoolOfOne(XenObject.Connection);

            if(pool != null)
                pool.PropertyChanged += pool_PropertyChanged;

            XenObject.Connection.Cache.RegisterCollectionChanged<XenAPI.Message>(Message_CollectionChangedWithInvoke);
        }

        private void Message_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.AssertOnEventThread();
            XenAPI.Message m = ((XenAPI.Message)e.Element);
            CheckMessage(m, e.Action);
        }

        private void pool_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "gui_config")
            {
                Dictionary<string, string> gui_config = Helpers.GetGuiConfig((IXenObject)sender);
                string uuid = Helpers.GetUuid(XenObject);

                foreach (string key in gui_config.Keys)
                {
                    if (!Palette.OtherConfigUUIDRegex.IsMatch(key) || !key.Contains(uuid))
                        continue;

                    string value = gui_config[key];
                    int argb;
                    if (!Int32.TryParse(value, out argb))
                        continue;

                    string[] strs = key.Split('.');

                    // just set the color, we dont care what it is
                    Palette.SetCustomColor(Palette.GetUuid(strs[strs.Length - 1], XenObject), Color.FromArgb(argb));
                }

                Program.Invoke(this, () =>
                {
                    GraphList.LoadGraphs(XenObject);
                    RefreshAll();
                });
            }
        }

        private void DeregEvents()
        {
            if (XenObject == null)
                return;

            Pool pool = Helpers.GetPoolOfOne(XenObject.Connection);

            if (pool != null)
                pool.PropertyChanged -= pool_PropertyChanged;

            XenObject.Connection.Cache.DeregisterCollectionChanged<XenAPI.Message>(Message_CollectionChangedWithInvoke);
        }

        public override void PageHidden()
        {
            DeregEvents();

            if (ArchiveMaintainer != null)
                ArchiveMaintainer.Pause();
        }

        private void ArchiveMaintainer_ArchivesUpdated()
        {
            Program.Invoke(this, RefreshAll);
        }

        private void RefreshAll()
        {
            DataPlotNav.RefreshXRange(false);
        }

        private void ShowUpsell()
        {
            using (var upsellDialog = new UpsellDialog(HiddenFeatures.LinkLabelHidden ? Messages.UPSELL_BLURB_PERFORMANCE : Messages.UPSELL_BLURB_PERFORMANCE + Messages.UPSELL_BLURB_TRIAL,
                                                        InvisibleMessages.UPSELL_LEARNMOREURL_TRIAL))
                upsellDialog.ShowDialog(this);
        }

        private void MoveGraphUp()
        {
            if (XenObject == null)
                return;

            int index = GraphList.SelectedGraphIndex;
            if (GraphList.AuthorizedRole)
            {
                GraphList.ExchangeGraphs(index, index - 1);
                GraphList.SaveGraphs(null);
            }
        }

        private void MoveGraphDown()
        {
            if (XenObject == null)
                return;

            int index = GraphList.SelectedGraphIndex;
            if (GraphList.AuthorizedRole)
            {
                GraphList.ExchangeGraphs(index, index + 1);
                GraphList.SaveGraphs(null);
            }
        }

        private void NewGraph()
        {
            if (XenObject == null)
                return;
            
            if (GraphList.AuthorizedRole)
            {
                GraphDetailsDialog dialog = new GraphDetailsDialog(GraphList, null);
                dialog.ShowDialog();
            }
        }

        private void EditGraph()
        {
            if (XenObject == null)
                return;

            if (GraphList.AuthorizedRole)
            {
                GraphDetailsDialog dialog = new GraphDetailsDialog(GraphList, GraphList.SelectedGraph);
                dialog.ShowDialog();
            }
        }

        private void DeleteGraph()
        {
            using (ThreeButtonDialog dlog = new ThreeButtonDialog(
                new ThreeButtonDialog.Details(SystemIcons.Warning,
                    string.Format(Messages.DELETE_GRAPH_MESSAGE, GraphList.SelectedGraph.DisplayName.EscapeAmpersands()),
                    Messages.XENCENTER),
                ThreeButtonDialog.ButtonYes,
                ThreeButtonDialog.ButtonNo))
            {
                if (dlog.ShowDialog(this) == DialogResult.Yes)
                    if (GraphList.AuthorizedRole)
                    {
                        GraphList.DeleteGraph(GraphList.SelectedGraph);
                        GraphList.LoadDataSources(SaveGraphs);
                    }
            }
        }

        private void RestoreDefaultGraphs()
        {
            using (ThreeButtonDialog dlog = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning,
                        Messages.GRAPHS_RESTORE_DEFAULT_MESSAGE,
                        Messages.XENCENTER),
                    ThreeButtonDialog.ButtonYes,
                    ThreeButtonDialog.ButtonNo))
            {
                if (dlog.ShowDialog(this) == DialogResult.Yes)
                    if (GraphList.AuthorizedRole)
                    {
                        GraphList.RestoreDefaultGraphs();
                        GraphList.LoadDataSources(SaveGraphs);
                    }
            }
        }

        private void SaveGraphs(ActionBase sender)
        {
            Program.Invoke(Program.MainWindow, delegate
            {
                var action = sender as GetDataSourcesAction;
                if (action != null)
                {
                    var dataSources = DataSourceItemList.BuildList(action.IXenObject, action.DataSources);
                    GraphList.SaveGraphs(dataSources);
                }
            });
        }

        private void LastYearZoom()
        {
            if (FeatureForbidden)
                ShowUpsell();
            else
                DataPlotNav.ZoomToRange(TimeSpan.Zero, TimeSpan.FromDays(366) - TimeSpan.FromSeconds(1));
        }

        private void LastMonthZoom()
        {
            if (FeatureForbidden)
                ShowUpsell();
            else
                DataPlotNav.ZoomToRange(TimeSpan.Zero, TimeSpan.FromDays(30) - TimeSpan.FromSeconds(1));
        }

        private void LastWeekZoom()
        {
            if (FeatureForbidden)
                ShowUpsell();
            else
                DataPlotNav.ZoomToRange(TimeSpan.Zero, TimeSpan.FromDays(7) - TimeSpan.FromSeconds(1));
        }

        private void LastDayZoom()
        {
            DataPlotNav.ZoomToRange(TimeSpan.Zero, TimeSpan.FromDays(1) - TimeSpan.FromSeconds(1));
        }

        private void LastHourZoom()
        {
            DataPlotNav.ZoomToRange(TimeSpan.Zero, TimeSpan.FromHours(1) - TimeSpan.FromSeconds(1));
        }

        private void LastTenMinutesZoom()
        {
            DataPlotNav.ZoomToRange(TimeSpan.Zero, TimeSpan.FromMinutes(10) - TimeSpan.FromSeconds(1));
        }

        #region Control event handlers

        private void addGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGraph();
        }

        private void editGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditGraph();
        }

        private void deleteGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteGraph();
        }

        private void restoreDefaultGraphsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreDefaultGraphs();
        }

        
        private void moveUpButton_Click(object sender, EventArgs e)
        {
            MoveGraphUp();
        }

        private void moveDownButton_Click(object sender, EventArgs e)
        {
            MoveGraphDown();
        }


        private void graphActionsButton_Click(object sender, EventArgs e)
        {
            graphActionsMenuStrip.Show(graphActionsButton, 0, graphActionsButton.Height);
        }

        private void graphActionsMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            UpdateActionButtons();
        }

        private void zoomButton_Click(object sender, EventArgs e)
        {
            zoomMenuStrip.Show(zoomButton, 0, zoomButton.Height);
        }

        
        private void lastYearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LastYearZoom();
        }

        private void lastMonthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LastMonthZoom();
        }

        private void lastWeekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LastWeekZoom();
        }

        private void lastDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LastDayZoom();
        }

        private void lastHourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LastHourZoom();
        }

        private void lastTenMinutesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LastTenMinutesZoom();
        }

        
        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            UpdateActionButtons();
        }
        
        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveGraphUp();
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveGraphDown();
        }

        
        private void newGraphToolStripContextMenuItem_Click(object sender, EventArgs e)
        {
            NewGraph();
        }

        private void editGraphToolStripContextMenuItem_Click(object sender, EventArgs e)
        {
            EditGraph();
        }

        private void deleteGraphToolStripContextMenuItem_Click(object sender, EventArgs e)
        {
            DeleteGraph();
        }

        private void restoreDefaultGraphsToolStripContextMenuItem_Click(object sender, EventArgs e)
        {
            RestoreDefaultGraphs();
        }

        
        private void lastYearToolStripContextMenuItem_Click(object sender, EventArgs e)
        {
            LastYearZoom();
        }

        private void lastMonthToolStripContextMenuItem_Click(object sender, EventArgs e)
        {
            LastMonthZoom();
        }

        private void lastWeekToolStripContextMenuItem_Click(object sender, EventArgs e)
        {
            LastWeekZoom();
        }

        private void lastDayToolStripContextMenuItem_Click(object sender, EventArgs e)
        {
            LastDayZoom();
        }

        private void lastHourToolStripContextMenuItem_Click(object sender, EventArgs e)
        {
            LastHourZoom();
        }

        private void lastTenMinutesToolStripContextMenuItem_Click(object sender, EventArgs e)
        {
            LastTenMinutesZoom();
        }

        
        private void GraphList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            UpdateActionButtons();
            if (editGraphToolStripMenuItem.Enabled)
                EditGraph();
        }

        #endregion
    }
}
