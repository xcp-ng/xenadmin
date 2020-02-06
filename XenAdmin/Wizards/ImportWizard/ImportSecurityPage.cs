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
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using XenOvf;
using XenAdmin.Controls;


namespace XenAdmin.Wizards.ImportWizard
{
	/// <summary>
	/// Class representing the page of the ImportAppliance wizard where the user verifies
	/// digital signature and encryption (if these options where included with the imported appliance)
	/// </summary>
	internal partial class ImportSecurityPage : XenTabPage
	{
		private bool m_hasManifest;
		private bool m_hasSignature;
		private bool m_detectedMissingCertificate;
        private bool m_buttonNextEnabled;

		public ImportSecurityPage()
		{
			InitializeComponent();
            m_ctrlError.HideError();
		}

		#region Base class (XenTabPage) overrides

		/// <summary>
		/// Gets the page's title (headline)
		/// </summary>
        public override string PageTitle => Messages.IMPORT_SECURITY_PAGE_TITLE;

		/// <summary>
		/// Gets the page's label in the (left hand side) wizard progress panel
		/// </summary>
        public override string Text => Messages.IMPORT_SECURITY_PAGE_TEXT;

		/// <summary>
		/// Gets the value by which the help files section for this page is identified
		/// </summary>
        public override string HelpID => "Security";

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
		{
			if (direction == PageLoadedDirection.Forward)
				SetButtonNextEnabled(OkToProceed());
		}

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
		{
			if (direction == PageLoadedDirection.Forward && IsDirty)
				cancel = !OkToProceed();
		}

        public override void CheckPageDisabled()
        {
            m_hasManifest = SelectedOvfPackage.HasManifest();
            m_hasSignature = SelectedOvfPackage.HasSignature();
            DisableStep = !m_hasManifest && !m_hasSignature;
        }

        public override void PopulatePage()
        {
            if (DisableStep)//defensive check
                return;

            m_linkCertificate.Visible = m_hasSignature;

            if (m_hasSignature)//check for signature first because m_hasManifest is True if m_hasSignature is True
            {
                m_checkBoxVerify.Text = Messages.IMPORT_SECURITY_PAGE_VERIFY_PRODUCER;
                m_labelVerify.Text = Messages.IMPORT_SECURITY_PAGE_VERIFY_PRODUCER_DESCRIPTION;
            }
            else
            {
                m_checkBoxVerify.Text = Messages.IMPORT_SECURITY_PAGE_VERIFY_CONTENT;
                m_labelVerify.Text = Messages.IMPORT_SECURITY_PAGE_VERIFY_CONTENT_DESCRIPTION;
            }
        }

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

		#endregion

		#region Accessors

		/// <summary>
		/// Can be null if no encryption is detected
		/// </summary>
		public string Password { get; private set; }

        public bool VerifyManifest => m_hasManifest && m_checkBoxVerify.Checked;

        public bool VerifySignature => m_hasSignature && m_checkBoxVerify.Checked;

        /// <summary>
		/// Package containing the selected OVF appliance.
		/// </summary>
        public Package SelectedOvfPackage { private get; set; }

		#endregion

		#region Private methods

        private void SetButtonNextEnabled(bool enabled)
        {
            m_buttonNextEnabled = enabled;
            OnPageUpdated();
        }       

		private bool OkToProceed()
		{
			if (m_checkBoxVerify.Checked && m_detectedMissingCertificate)
				return false;

			return true;
		}

		private void CheckCanDisplayCertificate()
		{
            m_ctrlError.HideError();

			try
            {
                using (var certificate = new X509Certificate2(SelectedOvfPackage.RawCertificate))
                    X509Certificate2UI.DisplayCertificate(certificate);
            }
			catch (CryptographicException)
			{
				m_ctrlError.ShowError(Messages.IMPORT_SECURITY_PAGE_ERROR_MISSING_CERTIFICATE);
				m_detectedMissingCertificate = true;
			}
		}

		#endregion

		#region Control event handlers

		private void m_linkCertificate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			CheckCanDisplayCertificate();
			SetButtonNextEnabled(OkToProceed());
		}

		private void m_checkBoxVerify_CheckedChanged(object sender, EventArgs e)
		{
			SetButtonNextEnabled(OkToProceed());
			IsDirty = true;
		}
		
		#endregion
	}
}
