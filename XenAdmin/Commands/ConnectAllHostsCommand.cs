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
using XenAdmin.Network;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Connects all hosts.
    /// </summary>
    internal class ConnectAllHostsCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public ConnectAllHostsCommand()
        {
        }

        public ConnectAllHostsCommand(IMainWindow mainWindow)
            : base(mainWindow)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            bool expand = MainWindowCommandInterface.GetXenConnectionsCopy().Count < 30;
            foreach (IXenConnection c in MainWindowCommandInterface.GetXenConnectionsCopy())
            {
                if (!c.IsConnected && !c.InProgress)
                {
                    MainWindowCommandInterface.Invoke(delegate
                    {
                        if (expand)
                        {
                            c.CachePopulated += c_CachePopulated;
                        }
                        XenConnectionUI.BeginConnect(c, false, Program.MainWindow, false);
                    });
                }
            }
        }

        private void c_CachePopulated(IXenConnection c)
        {
            c.CachePopulated -= c_CachePopulated;

            MainWindowCommandInterface.TrySelectNewObjectInTree(c, false, true, false);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            foreach (IXenConnection c in MainWindowCommandInterface.GetXenConnectionsCopy())
            {
                if (!c.InProgress && !c.IsConnected)
                {
                    return true;
                }
            }
            return false;
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_CONNECT_ALL_HOSTS;
            }
        }
    }
}
