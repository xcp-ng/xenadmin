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
using System.IO;
using System.Linq;
using XenAdmin.Controls;
using XenAdmin.Controls.Common;
using XenCenterLib;
using XenAdmin.Mappings;
using XenAPI;
using XenOvf;

namespace XenAdmin.Wizards.ImportWizard
{
	/// <summary>
	/// Class representing the page of the ImportAppliance wizard where the user specifies
	/// whether to run OS fixups.
	/// </summary>
	internal partial class ImportOptionsPage : XenTabPage
	{
		private IEnumerable<Host> m_hostTargets;
        private bool m_buttonNextEnabled;
        private readonly List<SR> m_srsWithRegisteredEvents = new List<SR>();
        private readonly CollectionChangeEventHandler PBD_CollectionChangedWithInvoke;

		public ImportOptionsPage()
		{
			InitializeComponent();
            m_radioButtonDontRunOSFixups.Checked = true;
            PBD_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(PBD_CollectionChanged);
            m_ctrlError.HideError();
		}

		#region Base class (XenTabPage) overrides

		/// <summary>
		/// Gets the page's title (headline)
		/// </summary>
        public override string PageTitle => Messages.IMPORT_OPTIONS_PAGE_TITLE;

		/// <summary>
		/// Gets the page's label in the (left hand side) wizard progress panel
		/// </summary>
        public override string Text => Messages.IMPORT_OPTIONS_PAGE_TEXT;

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            PopulateComboBox();

            Connection.Cache.RegisterCollectionChanged<PBD>(PBD_CollectionChangedWithInvoke);
            foreach (PBD pbd in Connection.Cache.PBDs)
            {
                pbd.PropertyChanged -= server_PropertyChanged;
                pbd.PropertyChanged += server_PropertyChanged;
            }
		}

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            DeregisterEvents();
        }

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

		#endregion

		#region Accessors

        public bool RunFixups => m_radioButtonRunOSFixups.Checked;

        public SR SelectedIsoSR { get; private set; }

        public Dictionary<string, VmMapping> VmMappings
        {
            set { m_hostTargets = from VmMapping mapping in value.Values select Connection.Resolve(mapping.XenRef as XenRef<Host>); }
        }

		#endregion

		#region Private methods

        /// <summary>
        /// Performs certain checks on the pages's input data and shows/hides an error accordingly
        /// </summary>
        /// <param name="checks">The checks to perform</param>
        private bool PerformCheck(params CheckDelegate[] checks)
        {
            var success = m_ctrlError.PerformCheck(checks);
            m_buttonNextEnabled = success;
            m_pictureBoxInfo.Visible = m_labelFixupISOInfo.Visible = success;
            OnPageUpdated();
            return success;
        }

		private bool IsIsoSrSelectable(SR sr)
		{
			return sr.content_type.ToLower() == "iso"
			       && sr.type.ToLower() == "iso"
			       && !sr.IsToolsSR()
			       && m_hostTargets.All(sr.CanBeSeenFrom); //returns true if the list is empty
		}

        private void DeregisterEvents()
        {
            if (Connection != null)
            {
                Connection.Cache.DeregisterCollectionChanged<PBD>(PBD_CollectionChangedWithInvoke);

                foreach (PBD pbd in Connection.Cache.PBDs)
                    pbd.PropertyChanged -= server_PropertyChanged;
            }

            foreach (var sr in m_srsWithRegisteredEvents)
                sr.PropertyChanged -= server_PropertyChanged;
        }

        private string GetSRDropDownItemDisplayString(SR sr)
        {
            return string.Format("{0} ({1})", sr.Name(), Util.DiskSizeString(sr.FreeSpace()));
        }

        private void PopulateComboBox()
        {
            try
            {
                m_comboBoxISOLibraries.SuspendLayout();
                int selectedIndex = m_comboBoxISOLibraries.SelectedIndex;

                foreach (var item in m_comboBoxISOLibraries.Items)
                {
                    if (item is ToStringWrapper<SR> wrapper && wrapper.item != null)
                        wrapper.item.PropertyChanged -= server_PropertyChanged;
                }
                m_comboBoxISOLibraries.Items.Clear();

                m_comboBoxISOLibraries.Items.Add(new ToStringWrapper<SR>(null, Messages.IMPORT_OPTIONS_PAGE_CHOOSE_ISO_SR));
                m_comboBoxISOLibraries.Items.Add(new ToStringWrapper<SR>(null, Messages.IMPORT_OPTIONS_PAGE_NEW_ISO_LIBRARY));

                var isoName = OVF.GetISOFixupFileName();

                var availableSRs = Connection.Cache.SRs;
                foreach (var sr in availableSRs)
                {
                    if (!IsIsoSrSelectable(sr))
                        continue;

                    int index = AddReplaceSrItem(sr);
                    if (selectedIndex < 0)
                    {
                        foreach (var vdiRef in sr.VDIs)
                        {
                            var vdi = Connection.Resolve(vdiRef);
                            if (vdi != null && vdi.name_label == isoName)
                            {
                                selectedIndex = index;
                                break;
                            }
                        }
                    }
                }

                if (m_comboBoxISOLibraries.SelectedIndex < 0)
                    SetSelectedIndexProgrammatically(selectedIndex < 0 ? 0 : selectedIndex);
            }
			finally
			{
                m_comboBoxISOLibraries.ResumeLayout();
			}
		}

		private bool CheckFixupIsoInXencenterInstallation(out string error)
		{
			error = string.Empty;

            var theVdi = Connection.Cache.VDIs.FirstOrDefault(vdi => vdi != null && vdi.name_label == OVF.GetISOFixupFileName());

            if (theVdi != null || File.Exists(OVF.GetISOFixupPath()))
                return true;
            
			error = Messages.IMPORT_OPTIONS_PAGE_ERROR_NO_FIXUP_ISO_INSTALLED;
		    return false;
		}

        private bool CheckCanCreateVdiOnIsoSr(out string error)
        {
            error = string.Empty;

            SM sm = null;
            if (SelectedIsoSR != null)
                sm = SM.GetByType(Connection, SelectedIsoSR.type);

            if (sm != null && sm.capabilities.Contains("VDI_CREATE"))
                return true;

            error = Messages.IMPORT_OPTIONS_PAGE_CANNOT_USE_SELECTED_ISO_LIBRARY;
            return false;
        }

		private void PBD_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (!(e.Element is PBD pbd))
                return;

            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    pbd.PropertyChanged -= server_PropertyChanged;
                    pbd.PropertyChanged += server_PropertyChanged;
                    break;
                case CollectionChangeAction.Remove:
                    pbd.PropertyChanged -= server_PropertyChanged;
                    break;
            }
        }

        private void server_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
            if (e.PropertyName != "name_label" && e.PropertyName != "physical_size" &&
                e.PropertyName != "physical_utilisation" &&
                e.PropertyName != "currently_attached")
                return;

            var sr = sender is PBD pbd ? Connection.Resolve(pbd.SR) : sender as SR;
            if (sr == null)
				return;

            if (IsIsoSrSelectable(sr))
                AddReplaceSrItem(sr);
            else
                RemoveSrItem(sr);
        }

        private int FindSrItem(SR sr)
        {
            var existing = m_comboBoxISOLibraries.Items.OfType<ToStringWrapper<SR>>().Where(wrapper => sr.Equals(wrapper.item)).ToList();
            return existing.Count > 0 ? m_comboBoxISOLibraries.Items.IndexOf(existing.First()) : -1;
        }

        private int AddReplaceSrItem(SR sr)
        {
            try
            {
                m_comboBoxISOLibraries.SuspendLayout();

                int selectedIndex = m_comboBoxISOLibraries.SelectedIndex;
                int index = FindSrItem(sr);

                if (index < 0)
                {
                    if (!m_srsWithRegisteredEvents.Contains(sr))
                        m_srsWithRegisteredEvents.Add(sr);

                    sr.PropertyChanged -= server_PropertyChanged;
                    sr.PropertyChanged += server_PropertyChanged;

                    index = m_comboBoxISOLibraries.Items.Count - 1;
                }
                else
                {
                    m_comboBoxISOLibraries.Items.RemoveAt(index);
                }

                m_comboBoxISOLibraries.Items.Insert(index, new ToStringWrapper<SR>(sr, GetSRDropDownItemDisplayString));
                SetSelectedIndexProgrammatically(selectedIndex);
                return index;
            }
            finally
            {
                m_comboBoxISOLibraries.ResumeLayout();
            }
        }

        private void RemoveSrItem(SR sr)
        {
            int index = FindSrItem(sr);
            if (index < 0)
                return;

            sr.PropertyChanged -= server_PropertyChanged;
            m_comboBoxISOLibraries.Items.RemoveAt(index);

            if (m_comboBoxISOLibraries.SelectedIndex < 0)
                SetSelectedIndexProgrammatically(0);
        }

        private void SetSelectedIndexProgrammatically(int newIndex)
        {
            if (m_radioButtonRunOSFixups.Checked)
            {
                m_comboBoxISOLibraries.SelectedIndex = newIndex;
                return;
            }

            m_comboBoxISOLibraries.SelectedIndexChanged -= m_comboBoxISOLibraries_SelectedIndexChanged;
            try
            {
                m_comboBoxISOLibraries.SelectedIndex = newIndex;
            }
            finally
            {
                m_comboBoxISOLibraries.SelectedIndexChanged += m_comboBoxISOLibraries_SelectedIndexChanged;
            }
        }

        private bool IsNewISOLibraryItem()
        {
            return m_comboBoxISOLibraries.SelectedItem is ToStringWrapper<SR> wrapper &&
                   wrapper.Equals(m_comboBoxISOLibraries.Items[m_comboBoxISOLibraries.Items.Count - 1]) &&
                   wrapper.ToStringProperty.Equals(Messages.IMPORT_OPTIONS_PAGE_NEW_ISO_LIBRARY);
        }

        private void ValidateSelection()
        {
            if (m_radioButtonDontRunOSFixups.Checked)
            {
                m_buttonNextEnabled = true;
                m_pictureBoxInfo.Visible = m_labelFixupISOInfo.Visible = m_ctrlError.Visible = false;
                OnPageUpdated();
                return;
            }

            SelectedIsoSR = m_radioButtonRunOSFixups.Checked && m_comboBoxISOLibraries.SelectedItem is ToStringWrapper<SR> wrapper
                ? wrapper.item
                : null;

            if (!PerformCheck((out string error) =>
            {
                error = null;
                return SelectedIsoSR != null;
            }))
                return;

            if (!PerformCheck(CheckFixupIsoInXencenterInstallation))
                return;

            var isoName = OVF.GetISOFixupFileName();

            foreach (var vdiRef in SelectedIsoSR.VDIs)
            {
                var vdi = Connection.Resolve(vdiRef);
                if (vdi != null && vdi.name_label == isoName)
                {
                    m_labelFixupISOInfo.Text = Messages.IMPORT_OPTIONS_PAGE_FOUND_EXISTING_ISO;
                    return;
                }
            }

            if (!PerformCheck(CheckCanCreateVdiOnIsoSr))
                return;

            m_labelFixupISOInfo.Text = Messages.IMPORT_OPTIONS_PAGE_USE_SELECTED_ISO_LIBRARY;
        }

        #endregion

        #region Control event Handlers

        private void m_comboBoxISOLibraries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_radioButtonRunOSFixups.Checked)
                m_radioButtonRunOSFixups.Checked = true;

            ValidateSelection();

            if (IsNewISOLibraryItem())
            {
                var wizard = new NewSRWizard(Connection);
                wizard.CheckNFSISORadioButton();
                wizard.ShowDialog();
            }
        }

        private void m_radioButtonRunOSFixups_CheckedChanged(object sender, EventArgs e)
        {
            if (m_radioButtonRunOSFixups.Checked)
                ValidateSelection();
        }

        private void m_radioButtonDontRunOSFixups_CheckedChanged(object sender, EventArgs e)
        {
            if (m_radioButtonDontRunOSFixups.Checked)
                ValidateSelection();
        }

        #endregion
    }
}
