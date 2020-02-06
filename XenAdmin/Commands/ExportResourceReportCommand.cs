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
using XenAdmin.Network;
using System.Windows.Forms;
using System.IO;
using XenAdmin.Core;
using XenAdmin;
using XenAPI;

namespace XenAdmin.Commands
{
    /// <summary>
    /// export resource report for the pool.
    /// </summary>
    internal class ExportResourceReportCommand : Command
    {
        public ExportResourceReportCommand()
        {
        }

        public ExportResourceReportCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public ExportResourceReportCommand(IMainWindow mainWindow, SelectedItem selection)
            : base(mainWindow, selection)
        {
        }
        
        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            if (selection.FirstAsXenObject != null)
            {
                Execute(selection.FirstAsXenObject.Connection);
            }
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.FirstAsXenObject != null && selection.FirstAsXenObject.Connection != null && selection.FirstAsXenObject.Connection.IsConnected &&
                (selection.PoolAncestor != null || selection.HostAncestor != null)) // this check ensures there's no cross-pool 
                return !Helpers.FeatureForbidden(selection.FirstAsXenObject.Connection, Host.RestrictExportResourceData);
            return false;
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_EXPORT_POOL_RESOURCE_DATA;
            }
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.MAINWINDOW_EXPORT_POOL_RESOURCE_DATA;
            }
        }

        private bool Execute(IXenConnection connection)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "xls files(*.xls)|*.xls|csv files(*.csv)|*.csv";
            saveFileDialog.FilterIndex = 1;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                new XenAdmin.Actions.ExportResourceReportAction(connection, saveFileDialog.FileName, saveFileDialog.FilterIndex).RunAsync();
            }
            return true;
        }
    }
}
