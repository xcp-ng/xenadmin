/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.CustomFields;
using XenAdmin.Dialogs;
using XenAdmin.Model;
using XenAPI;
using System.Diagnostics;
using XenAdmin.Core;
using XenAdmin.Commands;


namespace XenAdmin.TabPages
{
    public sealed partial class SnapshotsPage : BaseTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private VM m_VM = null;
        private bool m_NeedToUpdate = false;
        private float defaultWidthProperties;

        private readonly ToolTip toolTipDescriptionLabel = new ToolTip();
        private readonly ToolTip toolTipFolderLabel = new ToolTip();
        private readonly ToolTip toolTipTagsLabel = new ToolTip();

        private enum SnapshotsView
        {
            ListView,
            TreeView
        }

        private Dictionary<string, SnapshotsView> _storedViewsPerVm = new Dictionary<string, SnapshotsView>();

        public SnapshotsPage()
        {
            InitializeComponent();
            viewToolStripMenuItem.DropDown = contextMenuStripView;
            this.DoubleBuffered = true;
            base.Text = Messages.SNAPSHOTS_PAGE_TITLE;
            dataGridView.TabIndex = snapshotTreeView.TabIndex;
            dataGridView.Sorted += DataGridView_Sorted;
            columnCreated.DefaultCellStyle.Format = Messages.DATEFORMAT_DMY_HMS;
            columnCreated.ValueType = typeof(DateTime);
            ConnectionsManager.History.CollectionChanged += History_CollectionChanged;
        }

        public override string HelpID => "TabPageSnapshots";

        void DataGridView_Sorted(object sender, EventArgs e)
        {
            if (dataGridView.SortedColumn.Index == columnType.Index)
            {
                sortByTypeToolStripMenuItem.Checked = true;
                sortByNameToolStripMenuItem.Checked = false;
                sortByCreatedOnToolStripMenuItem.Checked = false;
                sortBySizeToolStripMenuItem.Checked = false;
            }
            else if (dataGridView.SortedColumn.Index == columnName.Index)
            {
                sortByTypeToolStripMenuItem.Checked = false;
                sortByNameToolStripMenuItem.Checked = true;
                sortByCreatedOnToolStripMenuItem.Checked = false;
                sortBySizeToolStripMenuItem.Checked = false;
            }
            else if (dataGridView.SortedColumn.Index == columnCreated.Index)
            {
                sortByTypeToolStripMenuItem.Checked = false;
                sortByNameToolStripMenuItem.Checked = false;
                sortByCreatedOnToolStripMenuItem.Checked = true;
                sortBySizeToolStripMenuItem.Checked = false;
            }
            else if (dataGridView.SortedColumn.Index == columnSize.Index)
            {
                sortByTypeToolStripMenuItem.Checked = false;
                sortByNameToolStripMenuItem.Checked = false;
                sortByCreatedOnToolStripMenuItem.Checked = false;
                sortBySizeToolStripMenuItem.Checked = true;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public VM VM
        {
            set
            {
                UnregisterHandlers();
                if (value != null)
                {
                    m_VM = value;
                    Program.Invoke(Program.MainWindow, () => snapshotTreeView.ChangeVMToSpinning(false, null));
                    m_VM.Connection.Cache.RegisterBatchCollectionChanged<VM>(
                        VM_BatchCollectionChanged);
                    m_VM.PropertyChanged += snapshot_PropertyChanged;
                    //Version setup
                    toolStripMenuItemScheduledSnapshots.Available = toolStripSeparatorView.Available = !Helpers.FeatureForbidden(VM.Connection, Host.RestrictVMSnapshotSchedule);

                    if (_storedViewsPerVm.ContainsKey(m_VM.opaque_ref) && _storedViewsPerVm[m_VM.opaque_ref] == SnapshotsView.ListView)
                        GridViewChecked();
                    else
                        TreeViewChecked();

                    toolStripButtonTreeView.Enabled = true;
                    toolStripButtonTreeView.ToolTipText = "";

                    revertButton.Enabled = true;
                    toolTipContainerRevertButton.SetToolTip("");

                    //toolStripButtonListView.Enabled = toolStripButtonTreeView.Enabled = true;
                    //Refresh items
                    BuildList();
                    RefreshDetailsGroupBox(true);
                    RefreshToolStripButtons();
                    RefreshVMSSPanel();
                }
            }
            get
            {
                return m_VM;
            }
        }

        private void RefreshVMSSPanel()
        {
            if (Helpers.FalconOrGreater(VM.Connection))
            {
                panelVMPP.Visible = true;
                var vmss = VM.Connection.Resolve(VM.snapshot_schedule);
                if (vmss == null || Helpers.FeatureForbidden(VM.Connection, Host.RestrictVMSnapshotSchedule))
                {
                    labelVMPPInfo.Text = Messages.THIS_VM_IS_NOT_IN_VMSS;
                    pictureBoxVMPPInfo.Image = Images.StaticImages._000_Info3_h32bit_16;

                    linkLabelVMPPInfo.Text = Helpers.FeatureForbidden(VM.Connection, Host.RestrictVMSnapshotSchedule) ? Messages.TELL_ME_MORE : Messages.VIEW_VMSS_POLICIES;

                }
                else
                {
                    labelVMPPInfo.Text = string.Format(Messages.THIS_VM_IS_IN_VMSS, vmss.Name());
                    pictureBoxVMPPInfo.Image = Images.StaticImages._000_Tick_h32bit_16;
                    linkLabelVMPPInfo.Text = Messages.VIEW_VMSS_POLICIES;
                }
            }
            else
            {
                panelVMPP.Visible = false;
            }
        }



        private void UpdateSpinningIcon()
        {
            ActionBase action = ConnectionsManager.History.Find(SpinningPredicate());
            if (action != null)
            {
                string message = action is VMSnapshotCreateAction ? Messages.SNAPSHOTTING : Messages.VM_REVERTING_ELLIPSIS;
                Program.Invoke(Program.MainWindow, () => snapshotTreeView.ChangeVMToSpinning(true, message));
            }
        }

        private Predicate<ActionBase> SpinningPredicate()
        {
            return a => a.VM == VM && !a.IsCompleted && (a is VMSnapshotCreateAction || a is VMSnapshotRevertAction);
        }

        private void History_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            //Program.AssertOnEventThread();
            Program.BeginInvoke(Program.MainWindow, () =>
                                                        {
                                                            if (e.Action == CollectionChangeAction.Add &&
                                                                (e.Element is VMSnapshotCreateAction ||
                                                                 e.Element is VMSnapshotRevertAction))
                                                            {
                                                                ActionBase createAction = (ActionBase)e.Element;
                                                                createAction.Completed += CreateActionCompleted;
                                                                string message = createAction is VMSnapshotCreateAction
                                                                                     ? Messages.SNAPSHOTTING
                                                                                     : Messages.VM_REVERTING_ELLIPSIS;
                                                                if (snapshotTreeView.SpinningMessage != Messages.SNAPSHOTTING)
                                                                    Program.Invoke(Program.MainWindow,
                                                                                   () =>
                                                                                   snapshotTreeView.ChangeVMToSpinning(true,
                                                                                                               message));
                                                            }
                                                        });
        }

        private void CreateActionCompleted(ActionBase senderAction)
        {
            senderAction.Completed -= CreateActionCompleted;
            ActionBase action = ConnectionsManager.History.Find(SpinningPredicate());
            if (action == null)
            {
                Program.Invoke(Program.MainWindow, () => snapshotTreeView.ChangeVMToSpinning(false, null));
            }
        }


        private void VM_BatchCollectionChanged(object sender, EventArgs e)
        {

            if (m_NeedToUpdate)
            {
                BuildList();
                RefreshDetailsGroupBox(true);
#if DEBUG
                Debug.WriteLine("Refreshing the list of snapshots...");
#endif
            }

            m_NeedToUpdate = false;
        }

        private void UnregisterHandlers()
        {
            if (m_VM == null)
                return;
            m_VM.Connection.Cache.DeregisterBatchCollectionChanged<VM>(VM_BatchCollectionChanged);
            m_VM.PropertyChanged -= snapshot_PropertyChanged;
            foreach (VM snapshot in m_VM.Connection.ResolveAll<VM>(m_VM.snapshots))
            {
                snapshot.PropertyChanged -= snapshot_PropertyChanged;
            }
        }

        public override void PageHidden()
        {
            UnregisterHandlers();
        }

        private void BuildList()
        {
            if (!this.Visible)
                return;
            lock (snapshotTreeView)
            {
                if (VM == null || VM.snapshots == null)
                    return;
                var snapshots = VM.Connection.ResolveAll(VM.snapshots);


                if (snapshots.Count == 0)
                {
                    snapshotTreeView.Clear();
                    TreeViewChecked();
                    RefreshDetailsGroupBox(true);
                    toolStripButtonTreeView.Enabled = toolStripButtonListView.Enabled = false; ;
                    return;
                }
                toolStripButtonListView.Enabled = true;
                IList<VM> roots = RefreshDataGridView(snapshots);
                toolStripButtonTreeView.Enabled = true;
                RefreshTreeView(roots);
                SelectPreviousItemVMTreeView();
                UpdateSpinningIcon();
            }
        }

        private void SelectPreviousItemVMTreeView()
        {
            if (snapshotTreeView.Items.Count > 0)
            {
                snapshotTreeView.Select();
                foreach (ListViewItem item in snapshotTreeView.Items)
                {
                    if (item.ImageIndex == SnapshotIcon.VMImageIndex)
                    {
                        SnapshotIcon icon = item as SnapshotIcon;
                        if (icon == null) continue;
                        if (icon.Parent.Selectable)
                        {
                            icon.Parent.Selected = true;
                            icon.Parent.Focused = true;
                            break;
                        }
                        else
                        {
                            foreach (SnapshotIcon sibling in icon.Parent.Children)
                            {
                                if (sibling.Selectable)
                                {
                                    sibling.Selected = true;
                                    sibling.Focused = true;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                RefreshDetailsGroupBox(true);
            }
        }

        private void RefreshTreeView(IList<VM> roots)
        {
            if (snapshotTreeView.Parent == null) return;

            snapshotTreeView.SuspendLayout();
            snapshotTreeView.BeginUpdate();
            snapshotTreeView.Clear();
            Debug.WriteLine("Start refreshing Tree");
            SnapshotIcon rootIcon = null;
            rootIcon = new SnapshotIcon(VM.Name(), Messages.BASE, null, snapshotTreeView, SnapshotIcon.Template);
            snapshotTreeView.AddSnapshot(rootIcon);

            //Set VM
            VM vmParent = VM.Connection.Resolve<VM>(VM.parent);
            if (vmParent == null || !vmParent.is_a_snapshot)
            {
                SnapshotIcon vmIcon = new SnapshotIcon(Messages.NOW, "", rootIcon, snapshotTreeView, SnapshotIcon.VMImageIndex);
                snapshotTreeView.AddSnapshot(vmIcon);
            }

            foreach (VM root in roots)
            {
                if (!((root.is_snapshot_from_vmpp || root.is_vmss_snapshot) && !toolStripMenuItemScheduledSnapshots.Checked))
                {
                    int icon;
                    if (root.is_snapshot_from_vmpp || root.is_vmss_snapshot)
                        icon = root.power_state == vm_power_state.Suspended ? SnapshotIcon.ScheduledDiskMemorySnapshot : SnapshotIcon.ScheduledDiskSnapshot;
                    else
                        icon = root.power_state == vm_power_state.Suspended ? SnapshotIcon.DiskAndMemorySnapshot : SnapshotIcon.DiskSnapshot;
                    DateTime time = root.snapshot_time.ToLocalTime() + root.Connection.ServerTimeOffset;
                    SnapshotIcon currentIcon = new SnapshotIcon(root.name_label, HelpersGUI.DateTimeToString(time, Messages.DATEFORMAT_DMY_HMS, true), rootIcon, snapshotTreeView, icon);
                    snapshotTreeView.AddSnapshot(currentIcon);
                    currentIcon.Tag = root;
                    BuildTree(root, currentIcon);
                }
                else
                    BuildTree(root, rootIcon);
            }

            if (snapshotTreeView.Items.Count < 4)
                snapshotTreeView.Clear();

            snapshotTreeView.EndUpdate();
            snapshotTreeView.ResumeLayout();
            snapshotTreeView.PerformLayout();


        }

        private void RefreshDataGridView()
        {
            IList<VM> snapshots = VM.Connection.ResolveAll(VM.snapshots);
            if (snapshots.Count == 0)
            {
                snapshotTreeView.Clear();
                TreeViewChecked();
                return;
            }
            RefreshDataGridView(snapshots);
        }

        private IList<VM> RefreshDataGridView(IList<VM> snapshots)
        {
            dataGridView.Rows.Clear();
            IList<VM> roots = new List<VM>();
            for (int i = 0; i < snapshots.Count; i++)
            {
                VM snapshot = snapshots[i];
                if (!(snapshot.is_snapshot_from_vmpp || snapshot.is_vmss_snapshot) || toolStripMenuItemScheduledSnapshots.Checked)
                {
                    snapshot.PropertyChanged -= snapshot_PropertyChanged;
                    snapshot.PropertyChanged += snapshot_PropertyChanged;
                    //Build DataGridView
                    SnapshotDataGridViewRow row = new SnapshotDataGridViewRow(snapshot);
                    row.Tag = snapshot;
                    dataGridView.Rows.Add(row);
                }

                VM parent = VM.Connection.Resolve<VM>(snapshot.parent);
                if (parent == null)
                    roots.Add(snapshot);
                else if (!snapshots.Contains(parent) && !roots.Contains(parent))
                    roots.Add(snapshot);


            }
            if (dataGridView.SortedColumn == null)
                dataGridView.Sort(columnName, ListSortDirection.Ascending);
            return roots;
        }

        private class SnapshotDataGridViewRow : DataGridViewRow
        {
            private DataGridViewTextBoxCell _type = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _name = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _creationTime = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _size = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _tags = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _description = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _sortType = new DataGridViewTextBoxCell();

            public SnapshotDataGridViewRow(VM snapshot)
            {
                //Icon
                this.Cells.Add(_type);
                _type.Value = snapshot.power_state == vm_power_state.Suspended
                                                                 ? Messages.DISKS_AND_MEMORY
                                                                 : Messages.DISKS_ONLY;
                //Name
                Cells.Add(_name);
                _name.Value = snapshot.name_label;
                //Created On
                Cells.Add(_creationTime);
                _creationTime.Value = snapshot.snapshot_time.ToLocalTime() + snapshot.Connection.ServerTimeOffset;
                //Size
                Cells.Add(_size);
                _size.Value = GetStringSnapshotSize(snapshot);
                //Tags
                Cells.Add(_tags);
                _tags.Value = ConcatWithSeparator(Tags.GetTags(snapshot), ", ");
                //Description
                Cells.Add(_description);
                _description.Value = snapshot.Description() != ""
                                         ? snapshot.Description()
                                         : Messages.NONE;
            }

        }

        void snapshot_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "current_operations")
            {
                VM vm = null;
                if (sender is VM)
                    vm = ((VM)sender);
                else
                    vm = (VM)sender;
                if (vm.current_operations.Count == 0 && !vm.is_a_snapshot)
                    m_NeedToUpdate = true;
#if DEBUG

                foreach (KeyValuePair<string, vm_operations> op in vm.current_operations)
                {
                    Debug.WriteLine(vm.name_label + " " + op.Value);
                }
                if (vm.current_operations.Count == 0)
                    Debug.WriteLine("End of operation");
#endif
            }

            switch (e.PropertyName)
            {
                case "name_label":
                    BuildList();
                    RefreshDetailsGroupBox(true);
                    break;
                case "snapshot_schedule":
                    RefreshVMSSPanel();
                    break;
                case "name_description":
                case "tags":
                case "other_config":
                case "Path":
                    RefreshDetailsGroupBox(true);
                    RefreshDataGridView();
                    RefreshToolStripButtons();
                    break;
                case "snapshots":
                    m_NeedToUpdate = true;
                    break;
            }
        }



        private static string GetStringSnapshotSize(VM snapshot)
        {
            long size = GetSnapshotSize(snapshot);

            return Util.DiskSizeString(size);
        }

        private static long GetSnapshotSize(VM snapshot)
        {
            long size = 0;

            foreach (VBD vbd in snapshot.Connection.ResolveAll(snapshot.VBDs))
            {
                if (vbd.type == vbd_type.Disk)
                {
                    VDI vdi = snapshot.Connection.Resolve(vbd.VDI);
                    if (vdi == null || vdi.physical_utilisation == -1)
                        continue;

                    size += vdi.physical_utilisation;
                }
            }

            VDI suspendedVdi = snapshot.Connection.Resolve(snapshot.suspend_VDI);
            if (suspendedVdi != null && suspendedVdi.physical_utilisation > 0)
                size += suspendedVdi.physical_utilisation;
            return size;
        }

        private void BuildTree(VM currentNode, SnapshotIcon currentIcon)
        {
            //Set VM
            if (VM.Connection.Resolve<VM>(VM.parent) == currentNode)
            {
                SnapshotIcon vmIcon = new SnapshotIcon(Messages.NOW, "", currentIcon, snapshotTreeView, SnapshotIcon.VMImageIndex);
                snapshotTreeView.AddSnapshot(vmIcon);
            }

            //Do childs
            foreach (XenRef<VM> child in currentNode.children)
            {

                if (VM.snapshots.Contains(child))
                {
                    VM childSnapshot = VM.Connection.Resolve<VM>(child);
                    if (!((childSnapshot.is_snapshot_from_vmpp || childSnapshot.is_vmss_snapshot) && !toolStripMenuItemScheduledSnapshots.Checked))
                    {
                        int icon;
                        if (childSnapshot.is_snapshot_from_vmpp || childSnapshot.is_vmss_snapshot)
                            icon = childSnapshot.power_state == vm_power_state.Suspended ? SnapshotIcon.ScheduledDiskMemorySnapshot : SnapshotIcon.ScheduledDiskSnapshot;
                        else
                            icon = childSnapshot.power_state == vm_power_state.Suspended ? SnapshotIcon.DiskAndMemorySnapshot : SnapshotIcon.DiskSnapshot;
                        DateTime time = childSnapshot.snapshot_time.ToLocalTime() + childSnapshot.Connection.ServerTimeOffset;
                        SnapshotIcon childIcon = new SnapshotIcon(childSnapshot.name_label, HelpersGUI.DateTimeToString(time, Messages.DATEFORMAT_DMY_HMS, true), currentIcon, snapshotTreeView, icon);
                        snapshotTreeView.AddSnapshot(childIcon);
                        childIcon.Tag = childSnapshot;
                        BuildTree(childSnapshot, childIcon);
                    }
                    else
                        BuildTree(childSnapshot, currentIcon);
                }
            }
        }

        private void view_SelectionChanged(object sender, System.EventArgs e)
        {
            RefreshDetailsGroupBox(false);
            RefreshToolStripButtons();
            snapshotTreeView.Invalidate();
        }

        private void RefreshToolStripButtons()
        {
            IList<VM> snapshots = GetSelectedSnapshots();
            if (snapshots != null && snapshots.Count == 1)
                EnableAllButtons();
            else if (snapshots != null && snapshots.Count > 1)
                EnableDelete();
            else
                DisableAllButtons();
        }

        private void DisableAllButtons()
        {
            revertButton.Enabled = false;
            toolTipContainerRevertButton.SetToolTip("");
            saveButton.Enabled = false;
            deleteButton.Enabled = false;
        }

        private void EnableAllButtons()
        {
            revertButton.Enabled = true;
            saveButton.Enabled = true;
            deleteButton.Enabled = true;
        }

        private void EnableDelete()
        {
            revertButton.Enabled = false;
            toolTipContainerRevertButton.SetToolTip("");
            saveButton.Enabled = false;
            deleteButton.Enabled = true;
        }

        private void RefreshDetailsGroupBox(bool force)
        {
            IList<VM> selectedSnaps = GetSelectedSnapshots();

            if (selectedSnaps == null || selectedSnaps.Count == 0)
            {
                ShowDisabledPanel();
            }
            else if (toolStripButtonListView.Checked && selectedSnaps.Count == 1)
            {
                ShowDetailsPanel(selectedSnaps[0], force);
            }
            else if (toolStripButtonTreeView.Checked && selectedSnaps.Count == 1)
            {
                ShowDetailsPanel(selectedSnaps[0], force);
            }
            else if (selectedSnaps.Count > 1)
            {
                ShowDetailsPanelMultipleSelection(selectedSnaps);
            }
            else
            {
                ShowDisabledPanel();
            }
        }

        private void ShowDisabledPanel()
        {
            propertiesGroupBox.Enabled = false;
            propertiesGroupBox.Tag = null;
            propertiesTableLayoutPanel.Controls.Remove(tableLayoutPanelMultipleSelection);
            propertiesTableLayoutPanel.Controls.Add(tableLayoutPanelSimpleSelection, 0, 2);
            propertiesGroupBox.Text = String.Format(Messages.SNAPSHOT_CREATED_ON, "");
            nameLabel.Text = "";
            sizeLabel.Text = "";
            labelMode.Text = "";
            descriptionLabel.Text = "";
            tagsLabel.Text = "";
            folderLabel.Text = "";
            screenshotPictureBox.Image = GetNoScreenshotImage();
            tableLayoutPanelSimpleSelection.RowStyles[5].Height = 0;
            tableLayoutPanelSimpleSelection.RowStyles[6].Height = 0;
        }



        private void ShowDetailsPanel(VM snapshot, bool force)
        {
            propertiesGroupBox.Enabled = true;
            if (propertiesGroupBox.Tag != snapshot || force || !propertiesGroupBox.Enabled)
            {
                propertiesTableLayoutPanel.SuspendLayout();
                tableLayoutPanelSimpleSelection.SuspendLayout();
                propertiesTableLayoutPanel.Controls.Remove(tableLayoutPanelMultipleSelection);
                List<CustomField> customFields = CustomFieldsManager.CustomFieldValues(snapshot);
                if (customFields.Count > 0)
                {
                    tableLayoutPanelSimpleSelection.RowStyles[5].Height = 30;
                    customFieldTitle1.Text = String.Format("{0}:", customFields[0].Definition.Name.ToString());
                    customFieldContent1.Text = customFields[0].Value.ToString();
                    tableLayoutPanelSimpleSelection.RowStyles[6].Height = 0;

                    if (customFields.Count >= 2)
                    {
                        tableLayoutPanelSimpleSelection.RowStyles[6].Height = 30;
                        customFieldTitle2.Text = String.Format("{0}:", customFields[1].Definition.Name.ToString());
                        customFieldContent2.Text = customFields[1].Value.ToString();
                    }

                }
                else
                {
                    tableLayoutPanelSimpleSelection.RowStyles[5].Height = 0;
                    tableLayoutPanelSimpleSelection.RowStyles[6].Height = 0;
                }
                propertiesTableLayoutPanel.Controls.Add(tableLayoutPanelSimpleSelection, 0, 2);
                tableLayoutPanelSimpleSelection.ResumeLayout();
                propertiesTableLayoutPanel.ResumeLayout();
                DateTime time = snapshot.snapshot_time.ToLocalTime() + snapshot.Connection.ServerTimeOffset;
                propertiesGroupBox.Text = String.Format(Messages.SNAPSHOT_CREATED_ON,
                                                        HelpersGUI.DateTimeToString(time, Messages.DATEFORMAT_DMY_HMS, true));
                nameLabel.Text = snapshot.name_label;
                sizeLabel.Text = GetStringSnapshotSize(snapshot);
                labelMode.Text = snapshot.power_state == vm_power_state.Halted ? Messages.DISKS_ONLY : Messages.DISKS_AND_MEMORY;

                var descr = snapshot.Description();
                descriptionLabel.Text = string.IsNullOrEmpty(descr) ? Messages.NONE : descr;
                toolTipDescriptionLabel.SetToolTip(descriptionLabel,
                                            descriptionLabel.Height == descriptionLabel.MaximumSize.Height ? descriptionLabel.Text : "");

                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorkerDoWork);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
                backgroundWorker.RunWorkerAsync(snapshot);
                tagsLabel.Text = ConcatWithSeparator(Tags.GetTags(snapshot), ", ");
                toolTipTagsLabel.SetToolTip(tagsLabel,
                                            tagsLabel.Height == tagsLabel.MaximumSize.Height ? tagsLabel.Text : "");
                Folder folder = Folders.GetFolder(snapshot);
                if (folder != null)
                    folderLabel.Text = folder.name_label;
                else
                    folderLabel.Text = Messages.NONE;
                toolTipFolderLabel.SetToolTip(folderLabel,
                                            folderLabel.Height == folderLabel.MaximumSize.Height ? folderLabel.Text : "");
                propertiesGroupBox.Tag = snapshot;
                propertiesButton.Enabled = true;
            }
        }

        private void ShowDetailsPanelMultipleSelection(IList<VM> snapshots)
        {
            propertiesTableLayoutPanel.Controls.Remove(tableLayoutPanelSimpleSelection);
            propertiesTableLayoutPanel.Controls.Add(tableLayoutPanelMultipleSelection, 0, 2);
            nameLabel.Text = String.Format(Messages.SNAPSHOTS_SELECTED, snapshots.Count);
            long totalSize = 0;
            List<string> tags = new List<string>();
            DateTime bigger = DateTime.MinValue;
            DateTime smaller = DateTime.MaxValue;
            foreach (VM snapshot in snapshots)
            {
                totalSize += GetSnapshotSize(snapshot);
                foreach (string tag in Tags.GetTags(snapshot))
                {
                    if (!tags.Contains(tag))
                        tags.Add(tag);
                }
                DateTime time = snapshot.snapshot_time.ToLocalTime() + snapshot.Connection.ServerTimeOffset;
                if (smaller > time)
                    smaller = time;
                if (bigger < time)
                    bigger = time;
            }
            propertiesGroupBox.Text = String.Format("{0} - {1}",
                                                    HelpersGUI.DateTimeToString(smaller, Messages.DATEFORMAT_DMY_HMS, true),
                                                    HelpersGUI.DateTimeToString(bigger, Messages.DATEFORMAT_DMY_HMS, true));
            multipleSelectionTotalSize.Text = Util.DiskSizeString(totalSize);
            multipleSelectionTags.Text = ConcatWithSeparator(tags.ToArray(), ", ");
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorkerDoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker.RunWorkerAsync(snapshots[0]);
            propertiesButton.Enabled = false;
            propertiesGroupBox.Tag = null;
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((TreeViewButtonsChecked && snapshotTreeView.SelectedIndices.Count >= 1) || (GridViewButtonsChecked && dataGridView.SelectedRows.Count >= 1))
                screenshotPictureBox.Image = (Image)e.Result;
            else
            {
                screenshotPictureBox.Image = GetNoScreenshotImage();
            }
        }


        void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                VM snapshot = (VM)e.Argument;
                Image image = GetSnapshotImage(snapshot, false);
                if (image == null)
                {
                    image = GetNoScreenshotImage();
                }
                e.Result = image;
            }
            catch (Exception ex)
            {
                log.Debug("Error creating the screenshot for simple selection.", ex);
            }


        }

        private Image GetNoScreenshotImage()
        {
            Image image;
            image = new Bitmap(100, 75);
            using (Graphics g = Graphics.FromImage(image))
            {
                g.FillRectangle(Brushes.Black, new Rectangle(0, 0, 100, 75));
                g.DrawString(Messages.NO_SCREENSHOT, Font, Brushes.White, 10, 30);
            }
            return image;
        }

        private static string ConcatWithSeparator(string[] strings, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string str in strings)
            {
                sb.Append(str);
                sb.Append(separator);
            }

            if (strings.Length > 0)
                sb.Remove(sb.Length - separator.Length, separator.Length);
            else
                sb.Append(Messages.NONE);

            return sb.ToString();
        }



        #region ScreenShot methods
        private Image GetSnapshotImage(VM snapshot, bool style)
        {

            Image image = GetBlobImage(snapshot);

            if (image == null)
                image = GetOtherConfigImage(snapshot);

            if (style)
                return ChangeStyle(image);
            else
                return image;
        }

        private Image ChangeStyle(Image image)
        {
            int SHADOW_OFFSET = 3;
            try
            {
                Image finalImage = new Bitmap(image.Width + SHADOW_OFFSET,
                                              image.Height + SHADOW_OFFSET);
                Rectangle rectangle = new Rectangle(0, 0, image.Width, image.Height);

                using (Graphics g = Graphics.FromImage(finalImage))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    //// Draw drop shadow
                    Rectangle shadow = new Rectangle(rectangle.Location, rectangle.Size);
                    shadow.Offset(SHADOW_OFFSET, SHADOW_OFFSET);

                    using (GraphicsPath path = GetRoundedPath(shadow, 4f))
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.FocusScales = new PointF(0.8f, 0.8f);
                        brush.SurroundColors = new Color[] { Color.FromArgb(0, Color.Black) };
                        brush.CenterColor = Color.Black;

                        g.FillPath(brush, path);
                    }

                    // Draw image with rounded corners
                    g.SetClip(GetRoundedPath(rectangle, 4f));
                    g.DrawImageUnscaled(image, 0, 0, image.Width, image.Height);

                    //// Darken image
                    using (Brush brush = new LinearGradientBrush(rectangle,
                                                                 Color.FromArgb(100, Color.Black),
                                                                 Color.FromArgb(140, Color.Black),
                                                                 LinearGradientMode.Vertical))
                        g.FillRectangle(brush, rectangle);

                    // Draw shimmer
                    using (GraphicsPath path = GetShimmer(rectangle))
                    using (Brush brush = new LinearGradientBrush(rectangle,
                                                                 Color.FromArgb(100, Color.White),
                                                                 Color.FromArgb(0, Color.White),
                                                                 LinearGradientMode.Vertical))
                        g.FillPath(brush, path);

                    return finalImage;
                }
            }
            finally
            {
                image.Dispose();
            }
        }

        private static GraphicsPath GetShimmer(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();

            int sweepHeight = bounds.Height / 3;
            int rightHeight = bounds.X + sweepHeight;
            int leftHeight = bounds.X + sweepHeight * 2;
            int border = 3;

            path.StartFigure();

            path.AddArc(bounds.Left + border, bounds.Top + border,
                2 * border, 2 * border, 180, 90);
            path.AddLine(bounds.Left + 2 * border, bounds.Top + border,
                bounds.Right - 2 * border, bounds.Top + border);
            path.AddArc(bounds.Right - (3 * border) - 1, bounds.Top + border,
                2 * border, 2 * border, 270, 90);

            path.AddLine(bounds.Right - border - 1, rightHeight,
                bounds.Left + border, leftHeight);

            path.CloseFigure();

            return path;
        }

        private static Image GetOtherConfigImage(VM snapshot)
        {
            try
            {
                string imageStr;
                if (!snapshot.other_config.TryGetValue(VMSnapshotCreateAction.VNC_SNAPSHOT, out imageStr))
                    return null;

                byte[] imageBuffer = Convert.FromBase64String(imageStr);
                using (MemoryStream imageStream = new MemoryStream(imageBuffer))
                    return Image.FromStream(imageStream);
            }
            catch
            {
                return null;
            }
        }

        private static GraphicsPath GetRoundedPath(RectangleF rect, float radius)
        {
            float diameter = 2 * radius;

            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter - 1, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter - 1, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private Image GetBlobImage(VM snapshot)
        {
            try
            {
                XenRef<Blob> blobRef;
                if (!snapshot.blobs.TryGetValue(VMSnapshotCreateAction.VNC_SNAPSHOT, out blobRef))
                    return null;

                Blob blob = VM.Connection.Resolve(blobRef);
                if (blob == null)
                    return null;

                using (Stream imageStream = blob.Load())
                    return Image.FromStream(imageStream);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        private void treeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TreeViewButtonsChecked)
            {
                TreeViewChecked();
                BuildList();
            }
        }

        private bool GridViewButtonsChecked
        {
            get
            {
                return toolStripButtonListView.Checked;
            }
            set
            {
                toolStripButtonListView.Checked = value;
            }
        }

        private bool TreeViewButtonsChecked
        {
            get
            {
                return toolStripButtonTreeView.Checked;
            }
            set
            {
                toolStripButtonTreeView.Checked = value;
            }
        }

        private void TreeViewChecked()
        {
            viewPanel.Controls.Clear();
            viewPanel.Controls.Add(snapshotTreeView);
            snapshotTreeView.Dock = DockStyle.Fill;
            TreeViewButtonsChecked = true;
            _storedViewsPerVm[VM.opaque_ref] = SnapshotsView.TreeView;
            GridViewButtonsChecked = false;
            snapshotTreeView.SelectedItems.Clear();
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                foreach (ListViewItem item in snapshotTreeView.Items)
                {
                    if (row.Tag == item.Tag)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
            RefreshToolStripButtons();
        }

        private void GridViewChecked()
        {
            viewPanel.Controls.Clear();
            viewPanel.Controls.Add(dataGridView);
            dataGridView.Dock = DockStyle.Fill;
            GridViewButtonsChecked = true;
            _storedViewsPerVm[VM.opaque_ref] = SnapshotsView.ListView;
            TreeViewButtonsChecked = false;

            RefreshToolStripButtons();
        }

        private void takeSnapshotToolStripButton_Click(object sender, EventArgs e)
        {
            TakeSnapshotCommand command = new TakeSnapshotCommand(Program.MainWindow, VM);
            command.Run();
        }

        private void deleteToolStripButton_Click(object sender, EventArgs e)
        {
            List<VM> snapshots = GetSelectedSnapshots();
            if (snapshots != null)
                DeleteSnapshots(snapshots);
        }

        private VM GetSelectedSnapshot()
        {
            if (TreeViewButtonsChecked)
            {
                if (snapshotTreeView.SelectedItems.Count == 1)
                {
                    return (VM)snapshotTreeView.SelectedItems[0].Tag;
                }
            }
            else if (GridViewButtonsChecked)
            {
                if (dataGridView.SelectedRows.Count == 1)
                {
                    return (VM)dataGridView.SelectedRows[0].Tag;
                }
            }
            return null;
        }

        private List<VM> GetSelectedSnapshots()
        {
            if (TreeViewButtonsChecked)
            {
                if (snapshotTreeView.SelectedItems.Count > 0)
                {
                    List<VM> snapshots = new List<VM>();
                    foreach (ListViewItem item in snapshotTreeView.SelectedItems)
                    {
                        VM snap = (VM)item.Tag;
                        if (snap != null && snap.is_a_snapshot)
                        {
                            if (!(snap.is_snapshot_from_vmpp || snap.is_vmss_snapshot) || toolStripMenuItemScheduledSnapshots.Checked)
                                snapshots.Add(snap);
                        }

                    }
                    return snapshots;
                }
            }
            else if (GridViewButtonsChecked)
            {
                if (dataGridView.SelectedRows.Count > 0)
                {
                    List<VM> snapshots = new List<VM>();
                    foreach (DataGridViewRow row in dataGridView.SelectedRows)
                    {
                        VM snap = (VM)row.Tag;
                        if (snap != null && snap.is_a_snapshot)
                        {
                            if (!(snap.is_snapshot_from_vmpp || snap.is_vmss_snapshot) || toolStripMenuItemScheduledSnapshots.Checked)
                                snapshots.Add(snap);
                        }
                    }
                    return snapshots;
                }
            }

            return null;
        }


        private void restoreToolStripButton_Click(object sender, EventArgs e)
        {
            VM snapshot = GetSelectedSnapshot();
            if (snapshot != null)
                RevertVM(VM, snapshot);
        }

        private void propertiesButton_Click(object sender, EventArgs e)
        {
            VM snapshot = (VM)propertiesGroupBox.Tag;
            if (snapshot != null)
            {
                PropertiesDialog dialog = new PropertiesDialog(snapshot);
                dialog.ShowDialog();
            }
        }



        private void SetContextMenuMultiSelection()
        {
            contextMenuStrip.Items.Clear();

            contextMenuStrip.Items.AddRange(new ToolStripItem[]
                {
                    viewToolStripMenuItem,
                    sortToolStripSeparator,
                    deleteToolStripMenuItem
                });
        }


        private void SetContextMenuSnapshot()
        {
            contextMenuStrip.Items.Clear();

            contextMenuStrip.Items.AddRange(new ToolStripItem[]
                {
                    revertToolStripMenuItem,
                    saveVMToolStripSeparator
                });
            contextMenuStrip.Items.AddRange(new ToolStripItem[]
                {
                    saveVMToolStripMenuItem,
                    saveTemplateToolStripMenuItem,
                    exportToolStripMenuItem
                });

            contextMenuStrip.Items.AddRange(new ToolStripItem[]
                {
                    separatorDeleteToolStripSeparator,
                    viewToolStripMenuItem
                });

            if (toolStripButtonListView.Checked)
                contextMenuStrip.Items.Add(sortByToolStripMenuItem);

            contextMenuStrip.Items.AddRange(new ToolStripItem[]
                {
                    sortToolStripSeparator,
                    deleteToolStripMenuItem,
                    propertiesToolStripMenuItem
                });
        }


        private void SetContextMenuNoneSelected()
        {
            contextMenuStrip.Items.Clear();

            contextMenuStrip.Items.Add(TakeSnapshotToolStripMenuItem);

            if (VM.snapshots.Count > 0)
                contextMenuStrip.Items.Add(viewToolStripMenuItem);
        }


        private void saveAsATemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new NewTemplateFromSnapshotCommand(Program.MainWindow, GetSelectedSnapshot()).Run();
        }

        private void saveAsAVirtualMachineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new NewVMFromSnapshotCommand(Program.MainWindow, GetSelectedSnapshot()).Run();
        }

        private void exportSnapshotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ExportSnapshotAsTemplateCommand(Program.MainWindow, GetSelectedSnapshot()).Run();
        }

        private void snapshotTreeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo test = snapshotTreeView.HitTest(e.Location);
            SnapshotIcon snapIcon = test.Item as SnapshotIcon;
            if (snapIcon != null && snapIcon.Selectable)
                revertButton_Click(sender, e);
        }

        private void sortByTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!sortByTypeToolStripMenuItem.Checked)
                dataGridView.Sort(columnType, ListSortDirection.Ascending);
        }

        private void sortByNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!sortByNameToolStripMenuItem.Checked)
                dataGridView.Sort(columnName, ListSortDirection.Ascending);
        }

        private void sortByCreatedOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!sortByCreatedOnToolStripMenuItem.Checked)
                dataGridView.Sort(columnCreated, ListSortDirection.Ascending);
        }

        private void sortBySizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!sortBySizeToolStripMenuItem.Checked)
                dataGridView.Sort(columnSize, ListSortDirection.Ascending);
        }

        private void screenshotPictureBox_Click(object sender, EventArgs e)
        {
            VM snapshot = (VM)propertiesGroupBox.Tag;
            if (snapshot != null && screenshotPictureBox.Image.Size.Width > 100)
                new ScreenShotDialog(snapshot.name_label, screenshotPictureBox.Image).ShowDialog();
        }


        private void chevronButton1_ButtonClick(object sender, EventArgs e)
        {
            if (contentTableLayoutPanel.ColumnStyles[1].Width == 0)
            {
                contentTableLayoutPanel.ColumnStyles[1].Width = defaultWidthProperties;
                chevronButton1.Text = Messages.HIDE_DETAILS;
                chevronButton1.Image = Images.StaticImages.PDChevronRight;
            }
            else
            {
                defaultWidthProperties = contentTableLayoutPanel.ColumnStyles[1].Width;
                contentTableLayoutPanel.ColumnStyles[1].Width = 0;
                chevronButton1.Text = Messages.SHOW_DETAILS;
                chevronButton1.Image = Images.StaticImages.PDChevronLeft;
            }
            snapshotTreeView.Invalidate();
        }


        private void revertButton_Click(object sender, EventArgs e)
        {
            VM snapshot = GetSelectedSnapshot();
            if (snapshot != null)
                RevertVM(VM, snapshot);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            List<VM> snapshots = GetSelectedSnapshots();
            if (snapshots != null)
                DeleteSnapshots(snapshots);
        }


        private void saveButton_Click(object sender, EventArgs e)
        {
            saveMenuStrip.Show(saveButton, 0, saveButton.Height);
        }



        private void saveAsTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new NewTemplateFromSnapshotCommand(Program.MainWindow, GetSelectedSnapshot()).Run();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new NewVMFromSnapshotCommand(Program.MainWindow, GetSelectedSnapshot()).Run();
        }

        private void exportAsBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ExportSnapshotAsTemplateCommand(Program.MainWindow, GetSelectedSnapshot()).Run();
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GridViewButtonsChecked)
            {
                GridViewChecked();
            }
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (GridViewButtonsChecked)
            {
                if (dataGridView.SelectedRows.Count > 1)
                {
                    SetContextMenuMultiSelection();
                }
                else if (dataGridView.SelectedRows.Count == 1)
                    SetContextMenuSnapshot();
                else
                    SetContextMenuNoneSelected();

            }
            else if (TreeViewButtonsChecked)
            {

                if (snapshotTreeView.SelectedItems.Count > 1)
                {
                    SetContextMenuMultiSelection();
                }
                else if (snapshotTreeView.SelectedItems.Count < 1)
                {
                    SetContextMenuNoneSelected();
                }
                else if (snapshotTreeView.SelectedItems.Count == 1)
                {
                    VM snapshot = (VM)snapshotTreeView.SelectedItems[0].Tag;

                    if (snapshot != null && snapshot.is_a_snapshot)
                    {
                        SetContextMenuSnapshot();
                    }
                }
            }

        }

        private void chevronButton1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                chevronButton1_ButtonClick(sender, e);
        }

        private static void DeleteSnapshots(List<VM> snapshots)
        {

            DeleteSnapshotCommand deleteSnapshotCommand = new DeleteSnapshotCommand(
                Program.MainWindow, snapshots.ConvertAll<SelectedItem>(input => new SelectedItem(input)));
            deleteSnapshotCommand.Run();

        }


        private void RevertVM(VM vm, VM snapshot)
        {
            if (vm.current_operations.Count == 0)
            {
                RevertToSnapshotCommand cmd = new RevertToSnapshotCommand(Program.MainWindow, vm, snapshot);
                cmd.Run();
            }
        }



        private void dataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo hitTestInfo = dataGridView.HitTest(e.X, e.Y);

                if (hitTestInfo.RowIndex >= 0)
                {
                    if (dataGridView.SelectedRows.Count == 1)
                        dataGridView.ClearSelection();
                    dataGridView.Rows[hitTestInfo.RowIndex].Selected = true;

                }
                else
                {
                    dataGridView.ClearSelection();
                }
                contextMenuStrip.Show(dataGridView, e.X, e.Y);

            }
        }

        private void snapshotTreeView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListViewHitTestInfo hitTestInfo = snapshotTreeView.HitTest(e.X, e.Y);
                if (hitTestInfo.Item != null)
                {
                    SnapshotIcon snapshotIcon = hitTestInfo.Item as SnapshotIcon;
                    if (snapshotIcon != null && snapshotIcon.Selectable)
                    {
                        if (snapshotTreeView.SelectedItems.Count == 1)
                            snapshotTreeView.SelectedItems.Clear();
                        hitTestInfo.Item.Selected = true;
                    }
                    else
                    {
                        snapshotTreeView.SelectedItems.Clear();
                    }
                }
                else
                {
                    snapshotTreeView.SelectedItems.Clear();
                }
                contextMenuStrip.Show(snapshotTreeView, e.X, e.Y);

            }
        }


        private void snapshotTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            SnapshotIcon icon = e.Item as SnapshotIcon;
            if (icon != null && icon.ImageIndex == SnapshotIcon.VMImageIndex)
            {
                snapshotTreeView.DoDragDrop(e.Item, DragDropEffects.Move);
            }

        }

        private void snapshotTreeView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            Point hitPoint = snapshotTreeView.PointToClient(new Point(e.X, e.Y));
            ListViewHitTestInfo info = snapshotTreeView.HitTest(hitPoint);
            SnapshotIcon icon = info.Item as SnapshotIcon;
            if (icon != null && (icon.ImageIndex == SnapshotIcon.DiskAndMemorySnapshot || icon.ImageIndex == SnapshotIcon.DiskSnapshot))
            {

                icon.Selected = true;
                if (snapshotTreeView.SelectedItems.Count > 1)
                    snapshotTreeView.SelectedItems.Clear();
            }

        }

        private void snapshotTreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;

        }

        private void snapshotTreeView_DragDrop(object sender, DragEventArgs e)
        {
            Point hitPoint = snapshotTreeView.PointToClient(new Point(e.X, e.Y));
            ListViewHitTestInfo info = snapshotTreeView.HitTest(hitPoint);
            SnapshotIcon icon = info.Item as SnapshotIcon;
            if (icon != null && (icon.ImageIndex == SnapshotIcon.DiskAndMemorySnapshot || icon.ImageIndex == SnapshotIcon.DiskSnapshot))
            {
                VM snapshot = icon.Tag as VM;
                if (snapshot != null)
                    RevertVM(VM, snapshot);
            }

        }


        private void snapshotTreeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (Type.GetType("Mono.Runtime") == null)
            {
                SnapshotIcon icon = snapshotTreeView.HitTest(e.X, e.Y).Item as SnapshotIcon;
                if (icon != null && icon.ImageIndex == SnapshotIcon.VMImageIndex)
                {
                    this.Cursor = Cursors.Hand;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
            else
            {
                // 
                // TODO: Under Mono there is an exception
                //

                // System.ArgumentOutOfRangeException: Specified argument was out of the range of valid values.
                // Parameter name: index
                // at System.Windows.Forms.ListView+ListViewItemCollection.get_Item (System.Int32 index)
                // at System.Windows.Forms.ListView+ItemControl.ItemsMouseMove (System.Object sender, System.Windows.Forms.MouseEventArgs me)
                // at System.Windows.Forms.Control.OnMouseMove (System.Windows.Forms.MouseEventArgs e)
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            contextMenuStripView.Show(buttonView, 0, buttonView.Height);
        }

        private void toolStripMenuItemScheduledSnapshots_Click(object sender, EventArgs e)
        {
            toolStripMenuItemScheduledSnapshots.Checked = !toolStripMenuItemScheduledSnapshots.Checked;
            BuildList();
        }

        private void linkLabelVMPPInfo_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var command = new VMGroupCommand<VMSS>(Program.MainWindow, VM);
            command.Run();
        }

        private void snapshotTreeView_Leave(object sender, EventArgs e)
        {
            snapshotTreeView.Invalidate();
        }

        private void snapshotTreeView_Enter(object sender, EventArgs e)
        {
            snapshotTreeView.Invalidate();
        }
    }
}
