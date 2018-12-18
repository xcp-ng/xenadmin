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
using System.ComponentModel;
using System.Windows.Forms;

namespace XenAdmin.Commands
{
    internal class VMLifeCycleToolStripMenuItem : CommandToolStripMenuItem
    {
        public VMLifeCycleToolStripMenuItem()
            : base(new CmdInt())
        {
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Command Command
        {
            get
            {
                return base.Command;
            }
        }

        protected override void Update()
        {
            base.Update();

            IMainWindow mainWindow = Command.MainWindowCommandInterface;
            SelectedItemCollection selection = Command.GetSelection();

            if (mainWindow != null)
            {
                base.DropDownItems.Clear();

                Command cmd = new ShutDownVMCommand(mainWindow, selection);
                if (cmd.CanExecute())
                {
                    base.DropDownItems.Add(new CommandToolStripMenuItem(cmd));
                }
                else
                {
                    base.DropDownItems.Add(new CommandToolStripMenuItem(new StartVMCommand(mainWindow, selection)));
                }

                cmd = new ResumeVMCommand(mainWindow, selection);
                if (cmd.CanExecute())
                {
                    base.DropDownItems.Add(new CommandToolStripMenuItem(cmd));
                }
                else
                {
                    base.DropDownItems.Add(new CommandToolStripMenuItem(new SuspendVMCommand(mainWindow, selection)));
                }

                cmd = new UnPauseVMCommand(mainWindow, selection);
                if (cmd.CanExecute())
                {
                    base.DropDownItems.Add(new CommandToolStripMenuItem(cmd));
                }
                else
                {
                    base.DropDownItems.Add(new CommandToolStripMenuItem(new PauseVMCommand(mainWindow, selection)));
                }                

                base.DropDownItems.Add(new CommandToolStripMenuItem(new RebootVMCommand(mainWindow, selection)));
                base.DropDownItems.Add(new CommandToolStripMenuItem(new VMRecoveryModeCommand(mainWindow, selection)));
                base.DropDownItems.Add(new ToolStripSeparator());
                base.DropDownItems.Add(new CommandToolStripMenuItem(new ForceVMShutDownCommand(mainWindow, selection)));
                base.DropDownItems.Add(new CommandToolStripMenuItem(new ForceVMRebootCommand(mainWindow, selection)));

                DropDownItems.Add(new ToolStripSeparator());
                DropDownItems.Add(new CommandToolStripMenuItem(new VappStartCommand(mainWindow, selection)));
                DropDownItems.Add(new CommandToolStripMenuItem(new VappShutDownCommand(mainWindow, selection)));
            }
        }

        private class CmdInt : Command
        {
            private static bool CanExecute(VM vm)
            {
                return !vm.Locked && !vm.is_a_template;
            }

            protected override bool CanExecuteCore(SelectedItemCollection selection)
            {
                return selection.AllItemsAre<VM>(CanExecute);
            }
        }
    }
}
