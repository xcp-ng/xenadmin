/* Copyright (c) XCP-ng Project.
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
using System.Windows.Forms;
using XenAdmin.Controls;

namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class LocalStorageFrontend : XenTabPage
    {
        public LocalStorageFrontend()
        {
            InitializeComponent();
        }

        public override string Text { get { return Messages.NEWSR_LOCATION; } }

        public override string PageTitle { get { return Messages.NEWSR_LOCALSTORAGE_DEVINFO; } }

        public string UUID
        {
            get
            {
                return null;
            }
        }

        public Dictionary<string, string> DeviceConfig
        {
            get
            {
                var dconf = new Dictionary<string, string>();
                dconf["device"] = textBoxDevicePath.Text;
                return dconf;
            }
        }

        public SrWizardType SrWizardType { private get; set; }

        public string SrDescription
        {
            get
            {
                return string.IsNullOrEmpty(textBoxDevicePath.Text)
                    ? null
                    : string.Format(Messages.NEWSR_LOCALSTORE_ACTION, textBoxDevicePath.Text);
            }
        }

        private void radioButtonExt4_CheckedChanged(object sender, EventArgs e)
        {
            switch (((RadioButton)sender).Text)
            {
                case "ext":
                    ((SrWizardType_LocalStorage)SrWizardType).SrType = "ext";
                    break;
                case "LVM":
                    ((SrWizardType_LocalStorage)SrWizardType).SrType = "lvm";
                    break;
                case "xfs":
                    ((SrWizardType_LocalStorage)SrWizardType).SrType = "xfs";
                    break;
            }
        }

        public override void SelectDefaultControl()
        {
            textBoxDevicePath.Select();
        }

        public override bool EnableNext()
        {
            return string.IsNullOrWhiteSpace(textBoxDevicePath.Text) == false;
        }

        private void textBoxDevicePath_TextChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }
    }
}
