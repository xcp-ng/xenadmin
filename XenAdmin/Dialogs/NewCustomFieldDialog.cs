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
using XenAdmin.CustomFields;
using XenAdmin.Network;

namespace XenAdmin.Dialogs
{
    public partial class NewCustomFieldDialog : XenDialogBase
    {
        public NewCustomFieldDialog(IXenConnection conn)
            :base(conn)
        {
            InitializeComponent();

            okButton.Enabled = !string.IsNullOrEmpty(NameTextBox.Text);
            TypeComboBox.SelectedIndex = 0;
        }

        public CustomFieldDefinition Definition
        {
            get
            {
                return new CustomFieldDefinition(NameTextBox.Text.Trim(), 
                    (CustomFieldDefinition.Types)TypeComboBox.SelectedIndex);
            }
        }

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            okButton.Enabled = EnableOKButton();
            DuplicateWarning.Visible = IsDuplicate();
        }

        private bool IsDuplicate()
        {
            foreach (CustomFieldDefinition customFieldDefinition in CustomFieldsManager.GetCustomFields(connection))
                if (customFieldDefinition.Name.Trim() == Definition.Name.Trim())
                    return true;
            return false;
        }

        private bool EnableOKButton()
        {
            return !string.IsNullOrEmpty(NameTextBox.Text.Trim()) && !IsDuplicate();
        }
    }
}