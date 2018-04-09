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

using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.Network;

namespace XenAdminTests.DialogTests.boston.CertificateDialogTests
{

    public struct CertificateDialogControlNames
    {
        public string CheckBox;
        public string OKButton;
        public string CancelButton;
        public string ViewCertButton;
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class CertificateChangedDialogTests : CertificateDialogTest<CertificateChangedDialog>
    {
        public CertificateChangedDialogTests()
        {
            CertificateDialogControlNames = new CertificateDialogControlNames
                               {
                                   CancelButton = "Cancelbutton",
                                   OKButton = "Okbutton",
                                   ViewCertButton = "ViewCertificateButton",
                                   CheckBox = "AlwaysIgnoreCheckBox"
                               };
        }

        protected override CertificateChangedDialog NewDialog()
        {
            return new CertificateChangedDialog(Certificate, "Inflames");
        }
    }


    [TestFixture, Category(TestCategories.UICategoryA)]
    public class UnknownCertificateDialogTests : CertificateDialogTest<UnknownCertificateDialog>
    {
        public UnknownCertificateDialogTests()
        {
            CertificateDialogControlNames = new CertificateDialogControlNames
            {
                CancelButton = "Cancelbutton",
                OKButton = "Okbutton",
                ViewCertButton = "ViewCertificateButton",
                CheckBox = "AutoAcceptCheckBox"
            };
        }

        protected override UnknownCertificateDialog NewDialog()
        {
            return new UnknownCertificateDialog(Certificate, "Inflames");
        }
    }

    /// <summary>
    /// Base class for certifiacte dialog tests 
    /// </summary>
    /// <typeparam name="T">type of certificate dialog</typeparam>
    public abstract class CertificateDialogTest<T> : DialogTest<T> where T : XenDialogBase
    {
        private CertificateDialogControlNames certificateDialogControlNames;

        protected CertificateDialogControlNames CertificateDialogControlNames
        {
            set { certificateDialogControlNames = value; }
        }

        /// Generated test certificate frim VS prompt with command:
        /// makecert -r -n "CN=DevTest" -a sha1 -r TestDevCertificate.cer
        protected X509Certificate Certificate
        {
            get
            {
                string certFileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestResources", "TestDevCertificate.cer");
                return X509Certificate.CreateFromCertFile(certFileName); 
            }
        }

        protected override void RunAfter()
        {
            VerifyButtonStates();
            MW(VerifyCheckboxActionAndState);
        }

        private void VerifyCheckboxActionAndState()
        {
            CheckBox cb = TestUtils.GetCheckBox(dialog, certificateDialogControlNames.CheckBox);
            Assert.IsFalse(cb.Checked, "Check box initial state");
            cb.Checked = true;
            Assert.IsTrue(cb.Checked, "Check box final state");
        }

        private void VerifyButtonStates()
        {
            Assert.IsTrue(TestUtils.GetButton(dialog, certificateDialogControlNames.ViewCertButton ).Enabled, "View certificate button");
            Assert.IsTrue(TestUtils.GetButton(dialog, certificateDialogControlNames.OKButton ).Enabled, "OK button");
            Assert.IsTrue(TestUtils.GetButton(dialog, certificateDialogControlNames.CancelButton ).Enabled, "Cancel button");
        }
    }
}
