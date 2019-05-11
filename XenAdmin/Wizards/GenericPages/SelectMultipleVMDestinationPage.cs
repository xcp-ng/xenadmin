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
using System.Windows.Forms;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Mappings;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Wizards.GenericPages
{
	/// <summary>
	/// Class representing the page of the ImportAppliance wizard where the user specifies
	/// the targets where the VMs of the appliance will be imported
	/// </summary>
	internal abstract partial class SelectMultipleVMDestinationPage : XenTabPage
	{
		private Dictionary<string, VmMapping> m_vmMappings;
		private IXenObject m_selectedObject;
		private bool updatingDestinationCombobox;
        private bool restoreGridHomeServerSelection;
        private bool updatingHomeServerList;
        private bool m_buttonNextEnabled;
        protected List<IXenConnection> ignoredConnections = new List<IXenConnection>();
        private readonly CollectionChangeEventHandler Host_CollectionChangedWithInvoke;

        /// <summary>
        /// Combobox item that can execute a command but also be an IEnableableComboBoxItem
        /// </summary>
        private class AddHostExecutingComboBoxItem : IEnableableComboBoxItem
        {
            public override string ToString()
            {
                return Messages.ADD_POOL_OR_SERVER;
            }

            public void ExecuteCommand(Control parent)
            {
                new AddHostCommand(Program.MainWindow, parent).Execute();
            }

            public bool Enabled
            {
                get { return true; }
            }
        }

	    protected SelectMultipleVMDestinationPage()
		{
			InitializeComponent();
            InitializeText();
            Host_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged);
			ConnectionsManager.XenConnections.CollectionChanged += CollectionChanged;
            ShowWarning(null);
		}

	    protected void InitializeText()
	    {
	        m_labelIntro.Text = InstructionText;
	        label1.Text = TargetServerText;
	        label2.Text = TargetServerSelectionIntroText;
            m_colVmName.HeaderText = VmColumnHeaderText;
	        m_colTarget.HeaderText = TargetColumnHeaderText;
	    }

	    private IXenObject _chosenItem;
	    public IXenObject ChosenItem
	    {
	        get { return _chosenItem; }
            protected set
            {
                _chosenItem = value;
                OnChosenItemChanged();
            }
	    }

	    /// <summary>
        /// Text containing instructions for use - at the top of the page
        /// </summary>
	    protected abstract string InstructionText { get; }

        /// <summary>
        /// Text demarking what the label for the target server drop down should be
        /// </summary>
	    protected abstract string TargetServerText { get; }

        protected virtual string VmColumnHeaderText 
        {
            get
            {
                return m_colVmName.HeaderText;
            }
        }

	    protected virtual string TargetColumnHeaderText
	    {
	        get
	        {
	            return m_colTarget.HeaderText;
	        }
	    }

        /// <summary>
        /// Text above the table containing a list of VMs and concomitant home server
        /// </summary>
        protected abstract string TargetServerSelectionIntroText { get; }

        protected virtual void OnChosenItemChanged()
        {}

	    protected void ShowWarning(string warningText)
        {
            if (string.IsNullOrEmpty(warningText))
                tableLayoutPanelWarning.Visible = false;
            else
            {
                labelWarning.Text = warningText;
                tableLayoutPanelWarning.Visible = true;
            }
        }

		#region Base class (XenTabPage) overrides

        protected override void PageLoadedCore(PageLoadedDirection direction)
		{
            ChosenItem = null;
            restoreGridHomeServerSelection = direction == PageLoadedDirection.Back;
            PopulateComboBox();
		}

        public override void PageCancelled(ref bool cancel)
        {
            UnregisterHandlers();
            CancelFilters();
            ClearComboBox();
            ClearDataGridView();
            ChosenItem = null;
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (!PerformCheck())
            {
                cancel = true;
                SetButtonNextEnabled(false);
                return;
            }

            UnregisterHandlers();
            SetDefaultTarget(ChosenItem);
            ClearComboBox();
        }

        public override void SelectDefaultControl()
        {
            m_comboBoxConnection.Select();
        }

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

		#endregion

		#region Accessors

		public Dictionary<string, VmMapping> VmMappings
		{
			get
			{
				foreach (DataGridViewRow row in m_dataGridView.Rows)
				{
					string sysId = (string)row.Cells[0].Tag;

					if (m_vmMappings.ContainsKey(sysId))
					{
						var mapping = m_vmMappings[sysId];
                        DataGridViewEnableableComboBoxCell cbCell = row.Cells[m_colTarget.Index] as DataGridViewEnableableComboBoxCell;
                        System.Diagnostics.Debug.Assert(cbCell != null, "ComboBox cell was not found");
					    
                        IEnableableXenObjectComboBoxItem selectedItem = cbCell.Value as IEnableableXenObjectComboBoxItem;
						System.Diagnostics.Debug.Assert(selectedItem != null, "Vm has no target mapped");
						var type = selectedItem.Item.GetType();

						if (type == typeof(Pool))
                            mapping.XenRef = new XenRef<Pool>(selectedItem.Item.opaque_ref);
						else if (type == typeof(Host))
                            mapping.XenRef = new XenRef<Host>(selectedItem.Item.opaque_ref);

                        mapping.TargetName = selectedItem.Item.Name();
					}
				}

				return m_vmMappings;
			}
			set { m_vmMappings = value; }
		}

		#endregion

	    public void SetDefaultTarget(IXenObject xenObject)
		{
			m_selectedObject = xenObject;
		}

        protected abstract DelayLoadingOptionComboBoxItem CreateDelayLoadingOptionComboBoxItem(IXenObject xenItem);

		#region Private methods

        private void SetButtonNextEnabled(bool enabled)
        {
            m_buttonNextEnabled = enabled;
            OnPageUpdated();
        }

	    protected virtual bool PerformCheck()
	    {
	        return true;
	    }

        private void ClearComboBox()
        {
            Program.AssertOnEventThread();

            foreach (var item in m_comboBoxConnection.Items)
            {
                DelayLoadingOptionComboBoxItem tempItem = item as DelayLoadingOptionComboBoxItem;
                if (tempItem != null)
                    tempItem.ReasonUpdated -= DelayLoadedComboBoxItem_ReasonChanged;
            }
            m_comboBoxConnection.Items.Clear();
        }

        private void ClearDataGridView()
        {
            //Clear up comboboxes
            foreach (DataGridViewRow row in m_dataGridView.Rows)
            {
                row.Cells[m_colTarget.Index].Dispose();
            }

            m_dataGridView.Rows.Clear();
            m_dataGridView.Refresh();
        }

		private void PopulateComboBox()
		{
			Program.AssertOnEventThread();

		    ClearDataGridView();

			updatingDestinationCombobox = true;
		    ClearComboBox();

		    var targetConnections = ConnectionsManager.XenConnectionsCopy.Where(con => con.IsConnected).Except(ignoredConnections).ToList();
            foreach (var xenConnection in targetConnections)
			{
			    DelayLoadingOptionComboBoxItem item = null;

				Pool pool = Helpers.GetPool(xenConnection);

				if (pool == null)
				{
					Host host = Helpers.GetMaster(xenConnection);

					if (host != null)
					{
                        item = CreateDelayLoadingOptionComboBoxItem(host);
                        m_comboBoxConnection.Items.Add(item);
                        item.ReasonUpdated += DelayLoadedComboBoxItem_ReasonChanged;
                        item.LoadAsync();
					    host.PropertyChanged -= PropertyChanged;
					    host.PropertyChanged += PropertyChanged;
					}
				}
				else
				{
                    item = CreateDelayLoadingOptionComboBoxItem(pool);
                    m_comboBoxConnection.Items.Add(item);
                    item.ReasonUpdated += DelayLoadedComboBoxItem_ReasonChanged;
                    item.LoadAsync();
			        pool.PropertyChanged -= PropertyChanged;
			        pool.PropertyChanged += PropertyChanged;
				}

				if (item != null && m_selectedObject != null && item.Item.Connection == m_selectedObject.Connection)
				    item.PreferAsSelectedItem = true;

				xenConnection.ConnectionStateChanged -= xenConnection_ConnectionStateChanged;
				xenConnection.ConnectionStateChanged += xenConnection_ConnectionStateChanged;
				xenConnection.CachePopulated -= xenConnection_CachePopulated;
				xenConnection.CachePopulated += xenConnection_CachePopulated;
				xenConnection.Cache.RegisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);
			}

			m_comboBoxConnection.Items.Add(new AddHostExecutingComboBoxItem());
			updatingDestinationCombobox = false;
		}

        private bool MatchingWithXenRefObject(IEnableableXenObjectComboBoxItem item, object xenRef)
        {
            XenRef<Host> hostRef = xenRef as XenRef<Host>;
            if (hostRef != null)
                return hostRef.opaque_ref == item.Item.opaque_ref;

            XenRef<Pool> poolRef = xenRef as XenRef<Pool>;
            if (poolRef != null)
                return poolRef.opaque_ref == item.Item.opaque_ref;

            return false;
        }

        private void RestoreGridHomeServerSelectionFromMapping()
        {
            foreach (DataGridViewRow row in m_dataGridView.Rows)
            {
                string sysId = (string)row.Cells[0].Tag;
                if (m_vmMappings.ContainsKey(sysId))
                {
                    var mapping = m_vmMappings[sysId];
                    var cbCell = row.Cells[m_colTarget.Index] as DataGridViewEnableableComboBoxCell;
                    if (cbCell == null)
                        return;

                    var list = cbCell.Items.OfType<IEnableableXenObjectComboBoxItem>().ToList();
                    var item = list.FirstOrDefault(cbi => MatchingWithXenRefObject(cbi, mapping.XenRef));
                    if (item != null)
                        cbCell.Value = item;
                }
            }
        }
        
        private void PopulateDataGridView(IEnableableXenObjectComboBoxItem selectedItem)
        {
            Program.AssertOnEventThread();

            updatingHomeServerList = true;
            try
            {
                var target = m_comboBoxConnection.SelectedItem as DelayLoadingOptionComboBoxItem;

                m_dataGridView.SuspendLayout();

                ClearDataGridView();

                var hasPoolSharedStorage = target != null && HasPoolSharedStorage(target.Item.Connection);

                foreach (var kvp in m_vmMappings)
                {
                    var tb = new DataGridViewTextBoxCell {Value = kvp.Value.VmNameLabel, Tag = kvp.Key};
                    var cb = new DataGridViewEnableableComboBoxCell{FlatStyle = FlatStyle.Flat};
                    var homeserverFilters = CreateTargetServerFilterList(selectedItem, new List<string> {kvp.Key});

                    if (target != null)
                    {
                        if (hasPoolSharedStorage)
                        {
                            foreach (var pool in target.Item.Connection.Cache.Pools)
                            {
                                var item = new NoTargetServerPoolItem(pool);
                                cb.Items.Add(item);

                                if ((m_selectedObject != null && m_selectedObject.opaque_ref == pool.opaque_ref) ||
                                    (target.Item.opaque_ref == pool.opaque_ref))
                                    cb.Value = item;

                                break; //there exists one pool per connection
                            }
                        }

                        var sortedHosts = new List<Host>(target.Item.Connection.Cache.Hosts);
                        sortedHosts.Sort();

                        foreach (var host in sortedHosts)
                        {
                            var item = new DelayLoadingOptionComboBoxItem(host, homeserverFilters);
                            cb.Items.Add(item);
                            item.ParentComboBox = cb;
                            item.PreferAsSelectedItem = m_selectedObject != null && m_selectedObject.opaque_ref == host.opaque_ref ||
                                                 target.Item.opaque_ref == host.opaque_ref;
                            item.ReasonUpdated += DelayLoadedGridComboBoxItem_ReasonChanged;
                            item.LoadAsync();
                        }
                    }

                    SetComboBoxPreSelection(cb);

                    var row = new DataGridViewRow();
                    row.Cells.AddRange(tb, cb);
                    m_dataGridView.Rows.Add(row);
                }

                HelpersGUI.ResizeGridViewColumnToAllCells(m_colTarget); //set properly the width of the last column

                if (restoreGridHomeServerSelection)
                {
                    RestoreGridHomeServerSelectionFromMapping();
                    restoreGridHomeServerSelection = false;
                }
            }
            finally
            {
                updatingHomeServerList = false;
                m_dataGridView.ResumeLayout();
            }
		}

	    private void SetComboBoxPreSelection(DataGridViewEnableableComboBoxCell cb)
	    {
	        if (cb.Value == null)
	        {
	            var firstEnabled = cb.Items.OfType<IEnableableComboBoxItem>().FirstOrDefault(i => i.Enabled);
	            if (firstEnabled != null)
	            {
	                cb.Value = firstEnabled;
	                SetButtonNextEnabled(true);
	            }
	            else
	            {
	                SetButtonNextEnabled(false);
	            }
	        }
	        else
	        {
                SetButtonNextEnabled(true);
	        }
	    }

	    private static bool HasPoolSharedStorage(IXenConnection conn)
		{
            if (conn == null)
				return false;

            foreach (var pbd in conn.Cache.PBDs.Where(thePbd => thePbd.SR != null))
			{
                var sr = conn.Resolve(pbd.SR);
				
				if (sr != null && sr.SupportsVdiCreate() && sr.shared)
						return true;
			}
			return false;
		}

		#endregion

		#region Event Handlers

        private void DelayLoadedComboBoxItem_ReasonChanged(DelayLoadingOptionComboBoxItem item)
        {
            if (item == null)
                throw new NullReferenceException("Trying to update delay loaded reason but failed to extract reason");

            Program.Invoke(this, () =>
            {
                int index = m_comboBoxConnection.Items.IndexOf(item);
                if (index < 0 || index >= m_comboBoxConnection.Items.Count)
                    return;

                if (updatingDestinationCombobox || updatingHomeServerList)
                    return;

                int selectedIndex = m_comboBoxConnection.SelectedIndex;

                var tempItem = m_comboBoxConnection.Items[index] as DelayLoadingOptionComboBoxItem;
                if (tempItem == null)
                    throw new NullReferenceException("Trying to update delay loaded reason but failed to extract reason");

                tempItem.CopyFrom(item);

                try
                {
                    m_comboBoxConnection.BeginUpdate();
                    m_comboBoxConnection.Items.RemoveAt(index);

                    if (updatingDestinationCombobox || updatingHomeServerList)
                        return;

                    m_comboBoxConnection.Items.Insert(index, tempItem);
                    m_comboBoxConnection.SelectedIndex = selectedIndex;

                    if (tempItem.PreferAsSelectedItem)
                        m_comboBoxConnection.SelectedItem = tempItem;
                }
                finally
                {
                    m_comboBoxConnection.EndUpdate();
                    item.ReasonUpdated -= DelayLoadedComboBoxItem_ReasonChanged;
                }
            });
        }

	    private void DelayLoadedGridComboBoxItem_ReasonChanged(DelayLoadingOptionComboBoxItem item)
	    {
	        if (item == null)
	            throw new NullReferenceException("Trying to update delay loaded reason but failed to extract reason");

	        var cb = item.ParentComboBox as DataGridViewEnableableComboBoxCell;
	        if (cb == null)
	            return;

            Program.Invoke(this, () =>
	        {
	            try
	            {
	                var selectedValue = cb.Value;
                    cb.DataGridView.RefreshEdit();
	                if (item.Enabled && item.PreferAsSelectedItem)
	                    cb.Value = item;
	                else
	                    cb.Value = selectedValue;
	            }
	            finally
	            {
	                item.ReasonUpdated -= DelayLoadedGridComboBoxItem_ReasonChanged;
	            }
	        });
	    }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
	        if (e.PropertyName == "name_label" || e.PropertyName == "metrics" ||
	            e.PropertyName == "enabled" || e.PropertyName == "live" || e.PropertyName == "patches")
	            Program.Invoke(this, PopulateComboBox);
		}

		private void CollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			Program.BeginInvoke(this, PopulateComboBox);
		}

		private void xenConnection_CachePopulated(object sender, EventArgs e)
        {
			Program.Invoke(this, PopulateComboBox);
        }

		private void xenConnection_ConnectionStateChanged(object sender, EventArgs e)
		{
			Program.Invoke(this, PopulateComboBox);
		}

		#endregion

		#region Control event handlers
		private void m_comboBoxConnection_SelectedIndexChanged(object sender, EventArgs e)
		{
            if (updatingHomeServerList)
                return;

            //If the item is delay loading and them item is disabled, null the selection made 
            //and clear the table containing server data
            IEnableableXenObjectComboBoxItem item = m_comboBoxConnection.SelectedItem as IEnableableXenObjectComboBoxItem;
            if(item != null && !item.Enabled)
            {
                m_comboBoxConnection.SelectedIndex = -1;
                m_dataGridView.Rows.Clear();
                ChosenItem = null;
                return;
            }

		    AddHostExecutingComboBoxItem exeItem = m_comboBoxConnection.SelectedItem as AddHostExecutingComboBoxItem;
            if (exeItem != null && !updatingDestinationCombobox)
                exeItem.ExecuteCommand(this);
				
			else if(!updatingDestinationCombobox)
			{
			    try
			    {
			        Cursor.Current = Cursors.WaitCursor;
			        ChosenItem = item == null ? null : item.Item;
			        PopulateDataGridView(item);
			    }
			    finally
			    {
                    Cursor.Current = Cursors.Default;
			    }
			}
                
			IsDirty = true;
		}

        /// <summary>
        /// Create a set of filters for the homeserver combo box selection
        /// </summary>
        /// <param name="item">selected item from the host combobox</param>
        /// <param name="vmOpaqueRefs">OpaqRefs of VMs which need to apply those filters</param>
        /// <returns></returns>
        protected virtual List<ReasoningFilter> CreateTargetServerFilterList(IEnableableXenObjectComboBoxItem item, List<string> vmOpaqueRefs)
        {
            return new List<ReasoningFilter>();
        }

        private void m_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != m_colTarget.Index || e.RowIndex < 0 || e.RowIndex >= m_dataGridView.RowCount)
				return;

			m_dataGridView.BeginEdit(false);

		    var editingControl = m_dataGridView.EditingControl as ComboBox;
		    if (editingControl != null)
		        editingControl.DroppedDown = true;
		}

		private void m_dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			m_dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
			IsDirty = true;
		}

		#endregion
        
        private void UnregisterHandlers()
        {
            ConnectionsManager.XenConnections.CollectionChanged -= CollectionChanged;
            foreach (var xenConnection in ConnectionsManager.XenConnectionsCopy)
            {
                var pool = Helpers.GetPool(xenConnection);
                if (pool == null)
                {
                    var host = Helpers.GetMaster(xenConnection);
                    if (host != null)
                        host.PropertyChanged -= PropertyChanged;
                }
                else
                {
                    pool.PropertyChanged -= PropertyChanged;
                }

                xenConnection.ConnectionStateChanged -= xenConnection_ConnectionStateChanged;
                xenConnection.CachePopulated -= xenConnection_CachePopulated;
                xenConnection.Cache.DeregisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);
            }
        }

	    private void CancelFilters()
	    {
	        foreach (var item in m_comboBoxConnection.Items)
	        {
	            DelayLoadingOptionComboBoxItem comboBoxItem = item as DelayLoadingOptionComboBoxItem;
                if (comboBoxItem != null)
                    comboBoxItem.CancelFilters();
	        }
        }

    }
}
