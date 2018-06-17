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
using XenAdmin.Controls;
using System.Drawing;

namespace XenAdmin.Commands
{
    internal class VMOperationToolStripMenuSubItem : VisualMenuItem
    {
        private VMOperationCommand _command;

        public VMOperationToolStripMenuSubItem(string text, Image image)
            : base(text, image)
        {
            Enabled = false;
        }

        public VMOperationToolStripMenuSubItem(VMOperationCommand command)
        {
            Util.ThrowIfParameterNull(command, "command");
            Command = command;
            Enabled = command != null && command.CanExecute();
        }

        private void Update()
        {
            if (_command != null)
            {
                if (_command.MenuText != null)
                {
                    Text = _command.MenuText;
                }

                if (_command.MenuImage != null)
                {
                    Image = _command.MenuImage;
                }

                if (_command.SecondImage != null)
                {
                    SecondImage = _command.SecondImage;
                }

                //No tooltip needed as the reason is included in the menu text
                //ToolTipText = _command.ToolTipText;

                StarRating = _command.StarRating;
            }
        }

        public VMOperationCommand Command
        {
            get
            {
                return _command;
            }
            set
            {
                _command = value;
                Update();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            if (_command != null && _command.CanExecute())
            {
                _command.Execute();
            }

            base.OnClick(e);
        }
    }
}
