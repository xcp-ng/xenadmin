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

using System.ComponentModel;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public partial class VcpuWarningDialog : XenDialogBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly VM vm;

        /// <summary>
        /// Shows a dialog warning that the user has configured vCPUs > pCPUs for the selected VM.
        /// If the user clicks 'ignore', the IgnoreExcessiveVcpus flag will be set to true in
        /// the given vm.
        /// </summary>
        public VcpuWarningDialog(VM vm)
            : base(vm.Connection)
        {
            InitializeComponent();
            this.vm = vm;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (checkBox1.Checked)
            {
                // User clicked 'ignore': set flag in VM.
                log.DebugFormat("Setting IgnoreExcessiveVcpus flag to true for VM {0}", vm.Name());

                VM copyVM = (VM)vm.Clone();
                copyVM.SetIgnoreExcessiveVcpus(true);
                // Save changes to server
                try
                {
                    vm.Locked = true;
                    copyVM.SaveChanges(vm.Connection.Session);
                }
                finally
                {
                    vm.Locked = false;
                }
            }
            else if (Program.MainWindow.SelectObjectInTree(vm))
            {
                Program.MainWindow.SwitchToTab(MainWindow.Tab.General);
            }
        }
    }
}
