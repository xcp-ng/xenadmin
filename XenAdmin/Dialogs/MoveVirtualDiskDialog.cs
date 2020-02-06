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
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Network;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public partial class MoveVirtualDiskDialog : XenDialogBase
    {
        protected const int BATCH_SIZE = 3;

        protected readonly List<VDI> _vdis= new List<VDI>();

        /// <summary>
        /// Designer support only. Do not use.
        /// </summary>
        public MoveVirtualDiskDialog()
        {
            InitializeComponent();
        }

        public MoveVirtualDiskDialog(IXenConnection connection, List<VDI> vdis)
            : base(connection)
        {
            InitializeComponent();
            _vdis = vdis ?? new List<VDI>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            srPicker1.Usage = SrPickerType;
            srPicker1.SetExistingVDIs(_vdis.ToArray());
            srPicker1.DiskSize = _vdis.Sum(d => d.physical_utilisation);
            srPicker1.Connection = connection;
            srPicker1.Invalidate();
            srPicker1.selectDefaultSROrAny();
        }

        protected SR SelectedSR
        {
            get { return srPicker1.SR; }
        }

        protected virtual SrPicker.SRPickerType SrPickerType
        {
            get { return SrPicker.SRPickerType.MoveOrCopy; }
        }

        private void srPicker1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateButtons();
        }

        private void srPicker1_DoubleClickOnRow(object sender, EventArgs e)
        {
            if (buttonMove.Enabled)
                buttonMove.PerformClick();
        }

        private void updateButtons()
        {
            buttonMove.Enabled = srPicker1.SR != null;
        }

        private void buttonMove_Click(object sender, EventArgs e)
        {
            CreateAndRunParallelActions();
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected virtual void CreateAndRunParallelActions()
        {
            if (_vdis.Count == 1)
            {
                new MoveVirtualDiskAction(connection, _vdis[0], SelectedSR).RunAsync();
            }
            else if (_vdis.Count > 1)
            {
                string title = string.Format(Messages.ACTION_MOVING_X_VDIS, _vdis.Count, SelectedSR.name_label);

                var batch = from VDI vdi in _vdis
                    select (AsyncAction)new MoveVirtualDiskAction(connection, vdi, SelectedSR);

                new ParallelAction(connection, title, Messages.ACTION_MOVING_X_VDIS_STARTED,
                    Messages.ACTION_MOVING_X_VDIS_COMPLETED, batch.ToList(), BATCH_SIZE).RunAsync();
            }
        }

        internal override string HelpName
        {
            get { return "VDIMigrateDialog"; }
        }

        internal static Command MoveMigrateCommand(IMainWindow mainWindow, SelectedItemCollection selection)
        {
            var cmd = new MigrateVirtualDiskCommand(mainWindow, selection);
            var con = selection.GetConnectionOfFirstItem();

            if (cmd.CanExecute() && !Helpers.FeatureForbidden(con, Host.RestrictCrossPoolMigrate))
                return cmd;

            return new MoveVirtualDiskCommand(mainWindow, selection);
        }
    }


    public class MigrateVirtualDiskDialog : MoveVirtualDiskDialog
    {
        public MigrateVirtualDiskDialog(IXenConnection connection, List<VDI> vdis)
            : base(connection, vdis)
        {
        }

        protected override SrPicker.SRPickerType SrPickerType
        {
            get { return SrPicker.SRPickerType.Migrate; }
        }

        protected override void CreateAndRunParallelActions()
        {
            if (_vdis.Count == 1)
            {
                new MigrateVirtualDiskAction(connection, _vdis[0], SelectedSR).RunAsync();
            }
            else if (_vdis.Count > 1)
            {
                string title = string.Format(Messages.ACTION_MIGRATING_X_VDIS, _vdis.Count, SelectedSR.name_label);

                var batch = from VDI vdi in _vdis
                    select (AsyncAction)new MigrateVirtualDiskAction(connection, vdi, SelectedSR);

                new ParallelAction(connection, title, Messages.ACTION_MIGRATING_X_VDIS_STARTED,
                    Messages.ACTION_MIGRATING_X_VDIS_COMPLETED, batch.ToList(), BATCH_SIZE).RunAsync();
            }
        }
    }
}
