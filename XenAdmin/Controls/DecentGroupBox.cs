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
using System.Drawing;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using XenAdmin.Core;


namespace XenAdmin.Controls
{
    /// <summary>
    /// A System.Windows.Forms.GroupBox with the additional property 'UseMnemonic'.
    /// </summary>
    public class DecentGroupBox : GroupBox
    {
        private bool useMnemonic;
        /// <summary>
        /// If set to true, any ampersands in the Text property are interpreted as keyboard shortcuts. Otherwise the ampersands are rendered correctly.
        /// </summary>
        [DefaultValue(false)]
        public bool UseMnemonic
        {
            get
            {
                return useMnemonic;
            }
            set
            {
                useMnemonic = value;
                RefreshText();
            }
        }

        private string rawText;
        public new string Text
        {
            get
            {
                return rawText;
            }
            set
            {
                rawText = value;
                RefreshText();
            }
        }

        private bool autoEllipsis = true;
        [DefaultValue(true)]
        public bool AutoEllipsis
        {
            get
            {
                return autoEllipsis;
            }
            set
            {
                autoEllipsis = value;
                RefreshText();
            }
        }

        /// <summary>
        /// The text that has been passed to the base System.Windows.Forms.GroupBox class, escaped as necessary.
        /// </summary>
        public string EscapedText => base.Text;

        private const int FUDGE = 15;

        private void RefreshText()
        {
            string newText = rawText;
            if (!useMnemonic)
                newText = rawText.EscapeAmpersands();

            int maxWidth = Width - FUDGE;
            if (maxWidth <= 0)
            {
                // Cope with excessively narrow groupboxes
                base.Text = String.Empty;
                return;
            }

            if (autoEllipsis && newText != null)
            {
                newText = newText.Ellipsise(new Rectangle(0, 0, maxWidth, Height), Font);
                if (newText == ".") //If ellipsise shortens the string to less than the ellipsis
                {
                    base.Text = String.Empty;
                    return;
                }

                //Avoid sending &... to the base class, so ensure we have an even number of ampersands on the end of the string.
                if (!useMnemonic && newText.Count(c => c == '&') % 2 > 0)
                    newText = newText.Remove(newText.LastIndexOf('&'), 1);
            }

            base.Text = newText;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            RefreshText();            
            base.OnSizeChanged(e);
        }
    }
}
