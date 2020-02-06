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
using System.Security.Cryptography.X509Certificates;

using XenAdmin.Core;
using XenAdmin.Network;


namespace XenAdmin.Dialogs.Network
{
    public partial class CertificateChangedDialog : XenDialogBase
    {
        private X509Certificate NewCertificate;
        private string _hostname;

        public CertificateChangedDialog(X509Certificate certificate, string hostname)
        {
            InitializeComponent();
            AlwaysIgnoreCheckBox.Enabled = Registry.SSLCertificateTypes == SSLCertificateTypes.None;
            NewCertificate = certificate;
            _hostname = hostname;

            string h = _hostname.Ellipsise(35);
            X509Certificate2 cert2 = new X509Certificate2(NewCertificate);
            labelTrusted.Text = string.Format(labelTrusted.Text, h,
                                              cert2.VerifyInAllStores() ? Messages.TRUSTED : Messages.NOT_TRUSTED);
            BlurbLabel.Text = string.Format(BlurbLabel.Text, h);
            Blurb2Label.Text = string.Format(Blurb2Label.Text, h);
        }

        private void ViewCertificateButton_Click(object sender, EventArgs e)
        {
            X509Certificate2UI.DisplayCertificate(NewCertificate as X509Certificate2, Handle);
        }

        private void Okbutton_Click(object sender, EventArgs e)
        {
            Settings.ReplaceCertificate(_hostname, NewCertificate);
            if (AlwaysIgnoreCheckBox.Enabled && AlwaysIgnoreCheckBox.Checked)
            {
                Properties.Settings.Default.WarnChangedCertificate = false;
                Settings.TrySaveSettings();
            }
        }
    }
}