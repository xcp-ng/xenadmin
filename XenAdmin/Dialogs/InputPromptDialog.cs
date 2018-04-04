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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace XenAdmin.Dialogs
{
    public partial class InputPromptDialog : XenDialogBase
    {
        // Do not use this constructor: use InputPromptDialog.Prompt(..) instead
        public InputPromptDialog()
        {
            InitializeComponent();
            textBox1_TextChanged(null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="helpID"></param>
        /// <returns>null if the user cancelled, otherwise the string they gave.</returns>
        public static String Prompt(IWin32Window owner, String message, String title, String helpID)
        {
            return Prompt(owner, message, title, "", helpID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="defaultInput"></param>
        /// <param name="helpID"></param>
        /// <returns>null if the user cancelled, otherwise the string they gave.</returns>
        public static String Prompt(IWin32Window owner, String message, String title, String defaultInput, String helpID)
        {
            InputPromptDialog messageBox = new InputPromptDialog();

            messageBox.Text = title;
            messageBox.promptLabel.Text = message;
            messageBox.textBox1.Text = defaultInput;
            messageBox.HelpID = helpID;

            if (messageBox.ShowDialog(owner) == DialogResult.Cancel)
                return null;

            return messageBox.textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = !String.IsNullOrEmpty(textBox1.Text.Trim());
        }

        private string helpID = null;
        public string HelpID
        {
            set
            {
                helpID = value;
            }
        }
        internal override string HelpName
        {
            get
            {
                return helpID ?? base.HelpName;
            }
        }
    }
}