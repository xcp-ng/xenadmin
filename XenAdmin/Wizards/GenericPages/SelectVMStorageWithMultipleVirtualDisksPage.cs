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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.Common;
using XenAdmin.Core;
using XenAdmin.Mappings;
using XenAdmin.Network;
using XenAPI;
using XenCenterLib;


namespace XenAdmin.Wizards.GenericPages
{
	/// <summary>
	/// Class representing the page of the ImportAppliance wizard where the user specifies
	/// storage repositories for the VMs in the improted appliance
	/// </summary>
	internal abstract partial class SelectVMStorageWithMultipleVirtualDisksPage : XenTabPage
	{
		private class StorageDetail
		{
			public string SysId { get; set; }
			public IStorageResource ResourceData { get; set; }
			public ulong RequiredSpace {
				get { return (ResourceData == null) ? 0 : ResourceData.RequiredDiskCapacity; }
			}
		}

        private class EnableableSrComboboxItem : IEnableableXenObjectComboBoxItem
        {
            private readonly ToStringWrapper<SR> stringWrapper;
            private readonly ulong spaceRequired;
            
            public EnableableSrComboboxItem(ToStringWrapper<SR> stringWrapper, ulong spaceRequired)
            {
                this.stringWrapper = stringWrapper;
                this.spaceRequired = spaceRequired;
            }

            public IXenObject Item
            {
                get { return stringWrapper.item; }
            }

            public SR Sr
            {
                get { return Item as SR; }
            }

            public bool Enabled
            {
                get { return (ulong)stringWrapper.item.FreeSpace() > spaceRequired; }
            }

            public override string ToString()
            {
                if(Enabled)
                    return stringWrapper.ToString();
                return String.Format(Messages.SELECT_STORAGE_DROPDOWN_ERROR_NOT_ENOUGH_SPACE, stringWrapper);
            }

            public string SrName
            {
                get { return stringWrapper.item.name_label; }
            }
        }

		#region Private fields

		private Dictionary<string, ulong> m_spaceRequirements = new Dictionary<string, ulong>();
		private Dictionary<string, VmMapping> m_vmMappings;
        private bool m_buttonNextEnabled;
        private bool m_buttonPreviousEnabled;

		#endregion

		public SelectVMStorageWithMultipleVirtualDisksPage()
		{
			InitializeComponent();
		    InitializeText();
		}

        protected void InitializeText()
        {
            m_labelIntro.Text = IntroductionText;
            m_radioAllOnSameSr.Text = AllOnSameSRRadioButtonText;
            m_radioSpecifySr.Text = OnSpecificSRRadioButtonText;
            m_comboBoxSr.GotFocus += m_comboBoxSr_GotFocus;
            m_colVmDisk.HeaderText = VmDiskColumnHeaderText;
            SetupTabIndices();
        }

        ~SelectVMStorageWithMultipleVirtualDisksPage()
        {
            m_comboBoxSr.GotFocus -= m_comboBoxSr_GotFocus;
        }

        private void SetupTabIndices()
        {
            m_radioAllOnSameSr.TabIndex = 3;
            m_comboBoxSr.TabIndex = 4;
            m_radioSpecifySr.TabIndex = 5;
            m_dataGridView.TabIndex = 6;
        }

	    protected abstract string IntroductionText { get; }
        protected abstract string AllOnSameSRRadioButtonText { get; }
        protected abstract string OnSpecificSRRadioButtonText { get; }

        protected virtual string VmDiskColumnHeaderText
        {
            get
            {
                return m_colVmDisk.HeaderText;
            }
        }

        protected virtual void AddAditionalMappings(Dictionary<string, VmMapping> vmMappings)
        {}

		#region Base class (XenTabPage) overrides

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
		{
		    TargetConnection = null;

			if (!cancel && direction == PageLoadedDirection.Forward && IsDirty && ImplementsIsDirty())
					cancel = !PerformCheck(CheckStorageRequirements);
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            SetButtonPreviousEnabled(true);
        }

	    public abstract StorageResourceContainer ResourceData(string sysId);

	    private bool displayDiskCapacity = true;
	    public bool DisplayDiskCapacity
	    {
	        set { displayDiskCapacity = value; }
	    }

	    private IXenConnection targetConnection;
        /// <summary>
        /// The connection from which the target storage is read
        /// Defaults to the base class connection if not set
        /// </summary>
	    public IXenConnection TargetConnection
	    {
	        get
	        {
	            if(targetConnection == null)
	                return Connection;
	            return targetConnection;
	        }
            set { targetConnection = value; }
	    }

        protected virtual bool SrIsSuitable(SR sr)
        {
            return true;
        }

		protected virtual bool IsExtraSpaceNeeded(XenRef<SR> sourceRef, XenRef<SR> targetRef)
		{
			return true;
		}


		public override void PopulatePage()
		{
			m_dataGridView.Rows.Clear();
            m_ctrlError.HideError();
			bool checkSpaceRequirements = true;

			var targetRefs = new List<object>();
			foreach (var mapping in m_vmMappings.Values.Where(mapping => !targetRefs.Contains(mapping.XenRef)))
				targetRefs.Add(mapping.XenRef);

			var commonSRs = new List<ToStringWrapper<SR>>();

			foreach (var kvp in m_vmMappings)
			{
				string sysId = kvp.Key;
				var vmMapping = kvp.Value;

				int i = 0;
                foreach (IStorageResource resourceData in ResourceData(sysId))
                {
                    if(!resourceData.CanCalculateDiskCapacity)
                        continue;

                    string disklabel = !string.IsNullOrEmpty(resourceData.DiskLabel)
                                        ? string.Format("{0} - {1}", vmMapping.VmNameLabel, resourceData.DiskLabel)
					                   	: string.Format("{0} - {1} {2}", vmMapping.VmNameLabel, Messages.STORAGE_DISK, i);

                    var cellVmDisk = new DataGridViewTextBoxCell
                                         {
                                             Tag = new StorageDetail
                                                     {SysId = sysId, ResourceData = resourceData},
                                             Value = FormatDiskValueText(resourceData, disklabel)
					                 	};

					DataGridViewRow row = new DataGridViewRow();
					row.Cells.AddRange(cellVmDisk);

                    var cb = FillGridComboBox(vmMapping.XenRef, targetRefs, resourceData, ref commonSRs);

					if (cb.Items.Count > 0)
					{
						cb.DisplayMember = ToStringWrapper<SR>.DisplayMember;
						cb.ValueMember = ToStringWrapper<SR>.ValueMember;
						if (cb.Value == null)
							cb.Value = cb.Items[0];
						cb.Tag = resourceData.Tag;
						row.Cells.Add(cb);
					}
					else
					{
						var cellError = new DataGridViewTextBoxCell { Value = Messages.IMPORT_SELECT_STORAGE_PAGE_ERROR_NO_SR_AVAILABLE};
						row.Cells.Add(cellError);
						cellError.ReadOnly = true; //this has to be set after the cell is added to a row
						SetButtonNextEnabled(false);
						checkSpaceRequirements = false;
					}

					m_dataGridView.Rows.Add(row);
					i++;
				}
			}
            HelpersGUI.ResizeGridViewColumnToAllCells(m_colStorage);//set properly the width of the last column

			m_comboBoxSr.Items.Clear();

            PopulateSrComboBox(commonSRs);

            if (m_comboBoxSr.Items.Count == 0)
			{
				m_comboBoxSr.Items.Add(Messages.IMPORT_SELECT_STORAGE_PAGE_ERROR_NO_COMMON_SR_AVAILABLE);
				SetButtonNextEnabled(false);
				checkSpaceRequirements = false;
				m_radioSpecifySr.Checked = true;
			}
			else
				m_radioAllOnSameSr.Checked = true;

            m_comboBoxSr.SelectedItem = m_comboBoxSr.Items[DetermineDefaultSrPositionInComboBox()];

			if (checkSpaceRequirements)
				PerformCheck(CheckStorageRequirements);
		}

	    private void PopulateSrComboBox(List<ToStringWrapper<SR>> commonSRs)
	    {
			List<EnableableSrComboboxItem> listToAdd = new List<EnableableSrComboboxItem>();
			foreach (var toStringWrapper in commonSRs)
			{
				ulong requiredSpace = 0;
				foreach (var kvp in m_vmMappings)
				{
					foreach (IStorageResource resourceData in ResourceData(kvp.Key))
					{
						if (IsExtraSpaceNeeded(resourceData.SR, new XenRef<SR>(toStringWrapper.item)))
						{
							requiredSpace += resourceData.RequiredDiskCapacity;
						}
					}
				}
				listToAdd.Add(new EnableableSrComboboxItem(toStringWrapper, requiredSpace));
			} 
	           
            if (listToAdd.Any(item => item.Enabled))
            {
                var sortedlistToAdd = from item in listToAdd
                                      where SrIsSuitable(item.Sr)
                                      orderby item.Enabled descending , item.SrName
                                      select item;

                m_comboBoxSr.Items.AddRange(sortedlistToAdd.ToArray());
            }
	    }

	    private string FormatDiskValueText(IStorageResource resourceData, string disklabel)
	    {
	        if(displayDiskCapacity)
	            return string.Format("{0} ({1})", disklabel,
	                                      Util.DiskSizeString(resourceData.RequiredDiskCapacity));
	        return disklabel;
	    }

	    public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

        public override bool EnablePrevious()
        {
            return m_buttonPreviousEnabled;
        }

	    #endregion

		#region Accessors

        public Dictionary<string, VmMapping> VmMappings
        {
            get
            {
                var comboItem = m_comboBoxSr.SelectedItem as EnableableSrComboboxItem;

                foreach (DataGridViewRow row in m_dataGridView.Rows)
                {
                    var storageDetail = (StorageDetail)row.Cells[0].Tag;

                    if (m_vmMappings.ContainsKey(storageDetail.SysId))
                    {
                        var mapping = m_vmMappings[storageDetail.SysId];
                        var gridItem = row.Cells[1].Value as ToStringWrapper<SR>;
                        var rasdId = row.Cells[1].Tag as string;

                        if (!string.IsNullOrEmpty(rasdId))
                        {
                            if (m_radioAllOnSameSr.Checked && comboItem != null)
                                mapping.Storage[rasdId] = comboItem.Sr;
                            else if (gridItem != null)
                                mapping.Storage[rasdId] = gridItem.item;
                        }
                    }
                }

                AddAditionalMappings(m_vmMappings);

                return m_vmMappings;
            }
            set { m_vmMappings = value; }
        }

		#endregion

		#region Private methods

        /// <summary>
        /// Performs certain checks on the pages's input data and shows/hides an error accordingly
        /// </summary>
        /// <param name="checks">The checks to perform</param>
        private bool PerformCheck(params CheckDelegate[] checks)
        {
            bool success = m_ctrlError.PerformCheck(checks);
            SetButtonNextEnabled(success);
            return success;
        }

        protected void SetButtonNextEnabled(bool enabled)
        {
            m_buttonNextEnabled = enabled;
            OnPageUpdated();
        }

        protected void SetButtonPreviousEnabled(bool enabled)
        {
            m_buttonPreviousEnabled = enabled;
            OnPageUpdated();
        }

		private bool CheckStorageRequirements(out string error)
		{
			error = string.Empty;
			m_spaceRequirements = new Dictionary<string, ulong>();

			if (m_radioAllOnSameSr.Checked)
			{
                var selectedItem = m_comboBoxSr.SelectedItem as EnableableSrComboboxItem;

				if (selectedItem == null) //no storage available
					return false;

				if (!selectedItem.Enabled)
				{
					error = String.Format(Messages.IMPORT_SELECT_STORAGE_PAGE_ERROR_SPACE_LIMITATION_COMMON, selectedItem.SrName);
					return false;
				}
			}
			else if (m_radioSpecifySr.Checked)
			{
				foreach (DataGridViewRow row in m_dataGridView.Rows)
				{
					var selectedItem = row.Cells[1].Value as ToStringWrapper<SR>;

					if (selectedItem == null) //no storage available
						return false;

					var storageDetail = (StorageDetail)row.Cells[0].Tag;
					string uuid = selectedItem.item.uuid;
					ulong requiredSpace =
						IsExtraSpaceNeeded(storageDetail.ResourceData.SR, new XenRef<SR>(selectedItem.item))
							? storageDetail.RequiredSpace
							: 0;
					
					if (m_spaceRequirements.ContainsKey(uuid))
						m_spaceRequirements[uuid] += requiredSpace;
					else
						m_spaceRequirements.Add(uuid, requiredSpace);

					if ((ulong)selectedItem.item.FreeSpace() <= m_spaceRequirements[uuid])
					{
						error = String.Format(Messages.IMPORT_SELECT_STORAGE_PAGE_ERROR_SPACE_LIMITATION, selectedItem.item.Name());
						return false;
					}
				}
			}
			return true;
		}

		private DataGridViewComboBoxCell FillGridComboBox(object xenRef, List<object> targetRefs, IStorageResource resource, ref List<ToStringWrapper<SR>> commonSRs)
		{
			var cb = new DataGridViewComboBoxCell { FlatStyle = FlatStyle.Flat };

            foreach (var pbd in TargetConnection.Cache.PBDs)
			{
				if (pbd.SR == null)
					continue;

                var sr = TargetConnection.Resolve(pbd.SR);
                if (sr == null || sr.IsDetached() || !sr.Show(XenAdminConfigManager.Provider.ShowHiddenVMs))
					continue;

                if ((sr.content_type.ToLower() == "iso" || sr.type.ToLower() == "iso") && !resource.SRTypeInvalid)
                    continue;
                if (sr.content_type.ToLower() != "iso" && resource.SRTypeInvalid)
                    continue;

				bool srOnHost = pbd.host != null && pbd.host.Equals(xenRef);
				
				if ((sr.shared || srOnHost) && (!IsExtraSpaceNeeded(resource.SR, pbd.SR) || (ulong)sr.FreeSpace() > resource.RequiredDiskCapacity) && SrIsSuitable(sr))
				{
					var count = (from ToStringWrapper<SR> existingItem in cb.Items
					             where existingItem.item.opaque_ref == sr.opaque_ref
					             select existingItem).Count();
					if (count > 0)
						continue; //iterating through pbds

					var newItem = new ToStringWrapper<SR>(sr, GetSRDropDownItemDisplayString);
					cb.Items.Add(newItem);

					if (SR.IsDefaultSr(sr))
						cb.Value = newItem;

					//store items to populate the m_comboBoxSr

					if ((sr.shared || (targetRefs.Count == 1 && srOnHost && targetRefs[0].Equals(xenRef))))
					{
						var num = (from ToStringWrapper<SR> existingItem in commonSRs
						           where existingItem.item.opaque_ref == sr.opaque_ref
						           select existingItem).Count();
						if (num <= 0)
							commonSRs.Add(newItem);
					}
				}
			}
			return cb;
		}

        /// <summary>
        /// Determine the location of the SR that has been selected as the default option
        /// </summary>
        /// <returns>Position of the default SR</returns>
        private int DetermineDefaultSrPositionInComboBox()
        {
            int defaultSrLocation = 0;

            foreach (var item in m_comboBoxSr.Items)
            {
                var sr = item as EnableableSrComboboxItem;

                if (sr != null && SR.IsDefaultSr(sr.Sr) && sr.Enabled)
                    defaultSrLocation = m_comboBoxSr.Items.IndexOf(sr);
            }
            return defaultSrLocation;
        }

		private string GetSRDropDownItemDisplayString(SR sr)
		{
			var availableSpace = Util.DiskSizeString(sr.FreeSpace());
			return String.Format("{0}, {1} {2}", sr.Name(), availableSpace, Messages.AVAILABLE);
		}

		#endregion

		#region Control event handlers

		private void m_radioAllOnSameSr_CheckedChanged(object sender, EventArgs e)
		{
			IsDirty = true;
			PerformCheck(CheckStorageRequirements);

            if(m_radioAllOnSameSr.Checked)
            {
                m_dataGridView.Enabled = false;
                m_comboBoxSr.Enabled = true;
                m_comboBoxSr.Focus();
            }    
		}

		private void m_radioSpecifySr_CheckedChanged(object sender, EventArgs e)
		{
			IsDirty = true;
			PerformCheck(CheckStorageRequirements);
            
            //Draw focus on the 0,0 cell
            if(m_radioSpecifySr.Checked)
            {
                m_comboBoxSr.Enabled = false;
                m_dataGridView.Enabled = true;
                if (m_dataGridView.Rows.Count > 0 && m_dataGridView.Rows[0].Cells.Count > 0)
                {
                    m_dataGridView.ClearSelection();
                    m_dataGridView.Focus();
                    m_dataGridView.Rows[0].Cells[0].Selected = true;
                    m_dataGridView.CurrentCell = m_dataGridView.SelectedCells[0];
                    m_dataGridView.BeginEdit(true);
                }
            }
		}

		private void m_comboBoxSr_SelectedIndexChanged(object sender, EventArgs e)
		{
			IsDirty = true;
			PerformCheck(CheckStorageRequirements);
		}

        private void m_comboBoxSr_GotFocus(object sender, EventArgs e)
        {
            m_radioAllOnSameSr.Checked = true;
        }

		private void m_dataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != m_colStorage.Index || e.RowIndex < 0 || e.RowIndex >= m_dataGridView.RowCount)
				return;

			m_dataGridView.BeginEdit(false);

			if (m_dataGridView.EditingControl != null && m_dataGridView.EditingControl is ComboBox)
				(m_dataGridView.EditingControl as ComboBox).DroppedDown = true;
		}

		private void m_dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			m_dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
			IsDirty = true;
		}

		private void m_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			PerformCheck(CheckStorageRequirements);
		}

		#endregion
	}
}
