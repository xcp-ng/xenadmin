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
using System.Text.RegularExpressions;
using XenAdmin.Controls;
using XenAdmin.Controls.Common;
using XenCenterLib;


namespace XenAdmin.Wizards.GenericPages
{
	/// <summary>
	/// Class representing the page of the wizard where the user specifies the IP address to be used by the Transfer VM.
	/// </summary>
	public partial class TvmIpPage : XenTabPage
	{
        private bool m_buttonNextEnabled;
        
		public TvmIpPage()
		{
			InitializeComponent();
            m_ctrlError.HideError();
		}

		#region Base class (XenTabPage) overrides

		/// <summary>
		/// Gets the page's title (headline)
		/// </summary>
		public override string PageTitle { get { return Messages.TVM_PAGE_TITLE; } }

			/// <summary>
		/// Gets the page's label in the (left hand side) wizard progress panel
		/// </summary>
		public override string Text { get { return Messages.TVM_PAGE_TEXT; } }

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
		{
		    m_buttonNextEnabled = true;
		    OnPageUpdated();
		}

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
		{
			if (direction == PageLoadedDirection.Forward && IsDirty)
			{
				if (!PerformCheck(CheckValidData))
					cancel = true;
			}
		}

        public override void PopulatePage()
        {
            m_comboBoxNetwork.PopulateComboBox(Connection, item => item.IsManagement);
			m_radioAutomatic.Checked = true;
		}

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

		#endregion

		#region Accessors

        public bool IsExportMode
        {
            set
            {
                autoHeightLabel1.Text = value
                                            ? Messages.TVM_PAGE_DESCRIPTION_EXPORT
                                            : Messages.TVM_PAGE_DESCRIPTION_IMPORT;
            }
        }

		public KeyValuePair<string, string> NetworkUuid
		{
		    get { return m_comboBoxNetwork.SelectedNetworkUuid; }
		}

		public bool IsTvmIpStatic { get { return m_radioManual.Checked; } }

		public string TvmIpAddress { get { return m_textBoxIP.Text; } }

		public string TvmSubnetMask { get { return m_textBoxMask.Text; } }

		public string TvmGateway { get { return m_textBoxGateway.Text; } }

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

		private bool CheckRequiredFieldsCompleted(out string error)
		{
			error = string.Empty;

			if (m_radioManual.Checked && (string.IsNullOrEmpty(m_textBoxIP.Text) || string.IsNullOrEmpty(m_textBoxMask.Text) || string.IsNullOrEmpty(m_textBoxGateway.Text)))
				return false;

			return true;
		}

		private bool CheckValidData(out string error)
		{
			error = string.Empty;

			if (m_radioManual.Checked)
			{
                if (!StringUtility.IsIPAddress(m_textBoxIP.Text))
				{
					error = Messages.ERROR_INVALID_IP;
					return false;
				}

                if (!StringUtility.IsIPAddress(m_textBoxMask.Text))
				{
					error = Messages.ERROR_INVALID_MASK;
					return false;
				}

                if (!StringUtility.IsIPAddress(m_textBoxGateway.Text))
				{
					error = Messages.ERROR_INVALID_GATEWAY;
					return false;
				}
			}

			return true;
		}

		private void ToggleRadioManualCheckedState()
		{
			if (m_radioManual.Checked)
				return;

			if (!String.IsNullOrEmpty(m_textBoxIP.Text) || !String.IsNullOrEmpty(m_textBoxMask.Text) || !String.IsNullOrEmpty(m_textBoxGateway.Text))
				m_radioManual.Checked = true;
		}

		#endregion

		#region Control event handlers

		private void m_radioAutomatic_CheckedChanged(object sender, EventArgs e)
		{
			//only examine the Checked case to avoid cyclical calls between the radion button handlers
			if (m_radioAutomatic.Checked)
			{
				m_radioManual.Checked = false;
				PerformCheck(CheckRequiredFieldsCompleted);
				IsDirty = true;
			}
		}

		private void m_radioManual_CheckedChanged(object sender, EventArgs e)
		{
			//only examine the Checked case to avoid cyclical calls between the radion button handlers
			if (m_radioManual.Checked)
			{
				m_radioAutomatic.Checked = false;
				PerformCheck(CheckRequiredFieldsCompleted);
				IsDirty = true;
			}
		}

		private void m_textBoxIP_TextChanged(object sender, EventArgs e)
		{
			ToggleRadioManualCheckedState();
			PerformCheck(CheckRequiredFieldsCompleted);
			IsDirty = true;
		}

		private void m_textBoxMask_TextChanged(object sender, EventArgs e)
		{
			ToggleRadioManualCheckedState();
			PerformCheck(CheckRequiredFieldsCompleted);
			IsDirty = true;
		}

		private void m_textBoxGateway_TextChanged(object sender, EventArgs e)
		{
			ToggleRadioManualCheckedState();
			PerformCheck(CheckRequiredFieldsCompleted);
			IsDirty = true;
		}

        private void m_comboBoxNetwork_SelectedIndexChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }

		#endregion
	}
}
