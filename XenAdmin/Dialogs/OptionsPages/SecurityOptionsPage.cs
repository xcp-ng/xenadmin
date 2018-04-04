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
using System.Windows.Forms;
using XenAdmin.Properties;
using XenAdmin.Core;


namespace XenAdmin.Dialogs.OptionsPages
{
    public partial class SecurityOptionsPage : UserControl, IOptionsPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SecurityOptionsPage()
        {
            InitializeComponent();

            build();
        }

        private void build()
        {
            // SSL Certificates
            CertificateFoundCheckBox.Checked    = Properties.Settings.Default.WarnUnrecognizedCertificate || 
                                                    Registry.AlwaysShowSSLCertificates == SSLCertificateTypes.All;
            CertificateChangedCheckBox.Checked  = Properties.Settings.Default.WarnChangedCertificate || 
                                                    Registry.AlwaysShowSSLCertificates != SSLCertificateTypes.None;
            CertificateFoundCheckBox.Enabled    = Registry.AlwaysShowSSLCertificates != SSLCertificateTypes.All;
            CertificateChangedCheckBox.Enabled  = Registry.AlwaysShowSSLCertificates == SSLCertificateTypes.None;
        }

        public static void Log()
        {
            // SSL Certificates
            log.Info("=== WarnUnrecognizedCertificate: " + Properties.Settings.Default.WarnUnrecognizedCertificate.ToString());
            log.Info("=== WarnChangedCertificate: " + Properties.Settings.Default.WarnChangedCertificate.ToString());
        }

        #region IOptionsPage Members

        public void Save()
        {
            // SSL Certificates
            if (CertificateFoundCheckBox.Enabled && CertificateFoundCheckBox.Checked != Properties.Settings.Default.WarnUnrecognizedCertificate)
                Properties.Settings.Default.WarnUnrecognizedCertificate = CertificateFoundCheckBox.Checked;
            if (CertificateChangedCheckBox.Enabled && CertificateChangedCheckBox.Checked != Properties.Settings.Default.WarnChangedCertificate)
                Properties.Settings.Default.WarnChangedCertificate = CertificateChangedCheckBox.Checked;
        }

        #endregion

        #region IVerticalTab Members

        public override string Text
        {
            get { return Messages.SECURITY; }
        }

        public string SubText
        {
            get { return Messages.SECURITY_DESC; }
        }

        public Image Image
        {
            get { return Resources.padlock; }
        }

        #endregion
    }
}
