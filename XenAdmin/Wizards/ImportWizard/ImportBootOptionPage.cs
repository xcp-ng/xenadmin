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

using XenAdmin.Controls;
using XenAdmin.Actions.VMActions;

namespace XenAdmin.Wizards.ImportWizard
{
    public partial class ImportBootOptionPage : XenTabPage
    {
        public ImportBootOptionPage()
        {
            InitializeComponent();
        }

        #region Base class (XenTabPage) overrides

        /// <summary>
        /// Gets the page's title (headline)
        /// </summary>
        public override string PageTitle { get { return Messages.IMPORT_SELECT_BOOT_OPTIONS_PAGE_TITLE; } }

        /// <summary>
        /// Gets the page's label in the (left hand side) wizard progress panel
        /// </summary>
        public override string Text { get { return Messages.IMPORT_SELECT_BOOT_OPTIONS_PAGE_TEXT; } }

        /// <summary>
        /// Gets the value by which the help files section for this page is identified
        /// </summary>
        public override string HelpID { get { return "VMConfig"; } }

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction == PageLoadedDirection.Forward)
                bootModesControl1.Connection = Connection;
        }

        public override void PopulatePage()
        {
            bootModesControl1.CheckBIOSBootMode();
        }

        #endregion

        #region Accessors
		
        public BootMode SelectedBootMode => bootModesControl1.SelectedOption;

        public string BootParams
        {
            get
            {
                switch (SelectedBootMode)
                {
                    case BootMode.UEFI_BOOT:
                    case BootMode.UEFI_SECURE_BOOT:
                        return "firmware=uefi;";
                    default:
                        return string.Empty;
                }
            }
        }

        public string PlatformSettings
        {
            get
            {
                switch (SelectedBootMode)
                {
                    case BootMode.UEFI_SECURE_BOOT:
                        return "secureboot=true;";
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
    }
}
