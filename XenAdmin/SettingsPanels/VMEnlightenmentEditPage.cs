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

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;

namespace XenAdmin.SettingsPanels
{
    public partial class VMEnlightenmentEditPage : UserControl, IEditPage
    {
        public VM vm;
        private bool currentValue;

        public VMEnlightenmentEditPage()
        {
            InitializeComponent();
        }

        #region Implementation of IVerticalTab

        public override string Text
        {
            get { return Messages.VM_ENLIGHTENMENT; }
        }

        public string SubText
        {
            get { return enlightenCheckBox.Checked ? Messages.ENABLED : Messages.DISABLED; }
        }

        public Image Image
        {
            get { return Resources.DC_16; }
        }

        #endregion

        #region Implementation of IEditPage

        public AsyncAction SaveSettings()
        {
            return enlightenCheckBox.Checked
                       ? (AsyncAction) new EnableVMEnlightenmentAction(vm, true)
                       : new DisableVMEnlightenmentAction(vm, true);
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            Trace.Assert(clone is VM);  // only VMs should show this page
            Trace.Assert(Helpers.ContainerCapability(clone.Connection));  // If no container capability, we shouldn't see this page

            vm = (VM)clone;

            Trace.Assert(vm.CanBeEnlightened());  // If the VM cannot be enlightened, we shouldn't see this page

            PopulatePage();
        }

        public bool ValidToSave
        {
            get { return true; }
        }

        public void ShowLocalValidationMessages()
        { }

        public void Cleanup()
        { }

        public bool HasChanged
        {
            get { return enlightenCheckBox.Checked != currentValue; }
        }

        #endregion

        private void PopulatePage()
        {
            currentValue = vm.IsEnlightened();
            enlightenCheckBox.Checked = currentValue;
        }
    }
}
