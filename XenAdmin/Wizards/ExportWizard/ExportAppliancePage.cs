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
using System.IO;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.Common;
using XenAdmin.Core;
using XenAdmin.Wizards.ExportWizard.ApplianceChecks;
using XenCenterLib;

namespace XenAdmin.Wizards.ExportWizard
{
	/// <summary>
	/// Class representing the page of the ExportAppliance wizard where the user defines
	/// the name and location where the appliance will be exported
	/// </summary>
	internal partial class ExportAppliancePage : XenTabPage
	{
        private bool m_buttonNextEnabled;

		public ExportAppliancePage()
		{
			InitializeComponent();
		    m_ctrlError.HideError();
		}

		#region Accessors

		/// <summary>
		/// Gets the parent directory of the exported appliance folder. It's not the appliance folder because
		/// this property is used to deduce space requirements on the target drive before the wizard closes
		/// and the appliance folder is only created once the wizard finishes.
		/// </summary>
		public string ApplianceDirectory { get { return m_textBoxFolderName.Text.Trim(); } }

		/// <summary>
		/// Gets or sets the exported appliance name (ovf/ova filename without extension).
		/// </summary>
		public string ApplianceFileName
		{
			get { return m_textBoxApplianceName.Text.Trim(); }
			set { m_textBoxApplianceName.Text = value; }
		}

		public bool ExportAsXva
		{
			get
			{
				var selectedItem = m_comboBoxFormat.SelectedItem as ToStringWrapper<bool>;
				return selectedItem == null ? false : selectedItem.item;
			}
		}

		public bool OvfModeOnly { private get; set; }

		#endregion

		#region Base class (XenTabPage) overrides

		/// <summary>
		/// Gets the page's title (headline)
		/// </summary>
		public override string PageTitle { get { return Messages.EXPORT_APPLIANCE_PAGE_TITLE; } }

		/// <summary>
		/// Gets the page's label in the (left hand side) wizard progress panel
		/// </summary>
		public override string Text { get { return Messages.EXPORT_APPLIANCE_PAGE_TEXT; } }

		/// <summary>
		/// Gets the value by which the help files section for this page is identified
		/// </summary>
		public override string HelpID { get { return "Appliance"; } }

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
		{
            if (direction == PageLoadedDirection.Forward)
            {
                m_textBoxFolderName.Text = Win32.GetKnownFolderPath(Win32.KnownFolders.Downloads);
                PerformCheck(CheckPathValid);
            }
		}

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
		{
			if (direction == PageLoadedDirection.Forward && IsDirty)
			{
				m_textBoxFolderName.Text = m_textBoxFolderName.Text.Trim();
				m_textBoxApplianceName.Text = m_textBoxApplianceName.Text.Trim();
                cancel = !PerformCheck(CheckDestinationFolderExists, CheckApplianceExists, CheckPermissions);
			}
		}

        public override void PopulatePage()
		{
			m_comboBoxFormat.Items.Clear();
			var ovfItem = new ToStringWrapper<bool>(false, Messages.EXPORT_APPLIANCE_PAGE_FORMAT_OVFOVA);
			var xvaItem = new ToStringWrapper<bool>(true, Messages.EXPORT_APPLIANCE_PAGE_FORMAT_XVA);

			if (OvfModeOnly)
				m_comboBoxFormat.Items.Add(ovfItem);
			else
				m_comboBoxFormat.Items.AddRange(new[] {ovfItem, xvaItem});

			m_comboBoxFormat.SelectedItem = ovfItem;
		}

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

		#endregion

		#region Private methods

        /// <summary>
        /// Performs certain checks on the pages's input data and shows/hides an error accordingly
        /// </summary>
        /// <param name="checks">The checks to perform</param>
        private bool PerformCheck(params CheckDelegate[] checks)
        {
            m_buttonNextEnabled = m_ctrlError.PerformCheck(checks);
            OnPageUpdated();
            return m_buttonNextEnabled;
        }

		private bool CheckPathValid(out string error)
		{
			error = string.Empty;

			if (String.IsNullOrEmpty(ApplianceFileName))
				return false;

			if (!PathValidator.IsFileNameValid(ApplianceFileName))
			{
				error = Messages.EXPORT_APPLIANCE_PAGE_ERROR_INALID_APP;
				return false;
			}

			if (String.IsNullOrEmpty(ApplianceDirectory))
				return false;

			string path = String.Format("{0}\\{1}", ApplianceDirectory, ApplianceFileName);

			if (!PathValidator.IsPathValid(path))
			{
				error = Messages.EXPORT_APPLIANCE_PAGE_ERROR_INVALID_DIR;
				return false;
			}

			return true;
		}

        private bool CheckPermissions(out string error)
        {
            error = string.Empty;
            var touchFile = Path.Combine(ApplianceDirectory, "_");

            try
            {
                //attempt writing in the destination directory
                FileStream fs = File.Create(touchFile);
                fs.Close();
                File.Delete(touchFile);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                error = Messages.EXPORT_APPLIANCE_PAGE_ERROR_PERMISSIONS;
                return false;
            }
        }

		private bool CheckDestinationFolderExists(out string error)
		{
			error = string.Empty;

			if (Directory.Exists(ApplianceDirectory))
				return true;

			error = Messages.ERROR_DESTINATION_DIR_NON_EXIST;
			return false;
		}

		private bool CheckApplianceExists(out string error)
		{

		    ApplianceCheck.FileExtension extension = m_comboBoxFormat.SelectedItem.ToString().Contains("xva")
		                                                 ? ApplianceCheck.FileExtension.xva
		                                                 : ApplianceCheck.FileExtension.ovaovf;

		    ApplianceCheck check = new ApplianceExistsCheck(ApplianceDirectory, ApplianceFileName, extension);
		    check.Validate();
		    error = check.ErrorReason;
		    return check.IsValid;
		}

		#endregion

		#region Control event handlers

		private void m_buttonBrowse_Click(object sender, EventArgs e)
		{
            using (var dlog = new FolderBrowserDialog
            {
                Description = Messages.FOLDER_BROWSER_EXPORT_APPLIANCE,
                SelectedPath = ApplianceDirectory
            })
			{
				if (dlog.ShowDialog() == DialogResult.OK)
					m_textBoxFolderName.Text = dlog.SelectedPath;
			}
		}

		private void m_textBoxApplianceName_TextChanged(object sender, EventArgs e)
		{
			PerformCheck(CheckPathValid);
			IsDirty = true;
		}

		private void m_textBoxFolderName_TextChanged(object sender, EventArgs e)
		{
			PerformCheck(CheckPathValid);
			IsDirty = true;
		}

		private void m_comboBoxFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			IsDirty = true;
		}

		#endregion
	}
}
