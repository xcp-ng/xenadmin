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

using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

using XenAdmin.Core;
using XenAdmin.Dialogs.Network;


namespace XenAdmin.Network
{
    internal class SSL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly object CertificateValidationLock = new object();

        internal static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                log.Debug("SslPolicyErrors is set to None, exiting validation");
                return true;
            }
            lock (CertificateValidationLock)
            {
                bool AcceptCertificate = false;
                HttpWebRequest webreq = (HttpWebRequest)sender;

                if (webreq.Address.Host == InvisibleMessages.ACTIVATION_SERVER)
                {
                    // Strict checking on the activation server certificate.
                    // Also, this ensures that it doesn't get added to the user settings
                    // through Settings.AddCertificate or Settings.ReplaceCertificate below.
                    log.Debug("SslPolicyErrors is set to None, exiting validation");
                    return sslPolicyErrors == SslPolicyErrors.None;
                }

                //This allows to run tests without MainWindow
                if (Program.MainWindow == null) return true;

                foreach (KeyValuePair<string, string> kvp in Settings.KnownServers)
                {
                    if (kvp.Key != webreq.Address.Host)
                        continue;

                    if (kvp.Value == certificate.GetCertHashString())
                    {
                        return true;
                    }
                    else if (!Properties.Settings.Default.WarnChangedCertificate && Registry.SSLCertificateTypes == SSLCertificateTypes.None)
                    {
                        Settings.ReplaceCertificate(kvp.Key, certificate);
                        log.Debug("Updating cert silently");
                        return true;
                    }
                    else
                    {
                        Program.Invoke(Program.MainWindow, () =>
                        {
                            using (var dialog = new CertificateChangedDialog(certificate, webreq.Address.Host))
                                AcceptCertificate = dialog.ShowDialog(Program.MainWindow) == DialogResult.OK;
                        });

                        if (AcceptCertificate)
                            log.Debug("Updating cert after confirmation");
                        else
                            log.Debug("User rejected changed cert");
                        return AcceptCertificate;
                    }
                }

                if (!Properties.Settings.Default.WarnUnrecognizedCertificate && Registry.SSLCertificateTypes != SSLCertificateTypes.All)
                {
                    // user has chosen to ignore new certificates
                    Settings.AddCertificate(certificate, webreq.Address.Host);
                    log.Debug("Adding new cert silently");
                    return true;
                }

                Program.Invoke(Program.MainWindow, () =>
                {
                    using (var dialog = new UnknownCertificateDialog(certificate, webreq.Address.Host))
                        AcceptCertificate = dialog.ShowDialog(Program.MainWindow) == DialogResult.OK;
                });

                if (AcceptCertificate)
                    log.Debug("Adding cert after confirmation");
                else
                    log.Debug("User rejected new cert");
                return AcceptCertificate;
            }
        }

    }

    public static class X509Ext
    {
        public static bool VerifyInAllStores(this X509Certificate2 certificate2)
        {
            try
            {
                X509Chain chain = new X509Chain(true);
                if (chain.Build(certificate2))
                    return true;
                return certificate2.Verify();
            }
            catch (CryptographicException)
            {
                return false;
            }
        }
    }
}
