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
using System.Text;
using XenAPI;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using System.Collections.ObjectModel;
using System.Drawing;

namespace XenAdmin.Commands
{
    /// <summary>
    /// Shows the attach virtual disk dialog for the specified VM.
    /// </summary>
    internal class AttachVirtualDiskCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public AttachVirtualDiskCommand()
        {
        }

        public AttachVirtualDiskCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public AttachVirtualDiskCommand(IMainWindow mainWindow, VM vm)
            : base(mainWindow, vm)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            VM vm = (VM)selection[0].XenObject;

            if (vm.VBDs.Count >= vm.MaxVBDsAllowed())
            {
                using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(
                            SystemIcons.Error,
                            FriendlyErrorNames.VBDS_MAX_ALLOWED,
                            Messages.DISK_ATTACH)))
                {
                    dlg.ShowDialog(Program.MainWindow);
                }
            }
            else
            {
                new AttachDiskDialog(vm).ShowPerXenObject(vm, Program.MainWindow);
            }
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.Count == 1)
            {
                VM vm = selection[0].XenObject as VM;

                return vm != null && !vm.is_a_snapshot && !vm.Locked;

            }
            return false;
        }
    }
}
