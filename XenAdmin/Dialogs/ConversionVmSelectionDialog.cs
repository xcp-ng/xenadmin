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
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public partial class ConversionVmSelectionDialog : XenDialogBase
    {
        private readonly IEnumerable<VM> _conversionVms;

        public ConversionVmSelectionDialog(IXenConnection connection, IEnumerable<VM> conversionVms)
            : base(connection)
        {
            InitializeComponent();
            _conversionVms = conversionVms;
        }

        public VM ConversionVm
        {
            get
            {
                foreach (var row in dataGridViewEx1.SelectedRows)
                {
                    if (row is ConversionVmRow vmRow)
                        return vmRow.Vm;
                }

                return null;
            }
        }

        internal override string HelpName => "ConversionManager";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Text = string.Format(Messages.CONVERSION_SELECT_VPX_TITLE, Helpers.GetName(connection));

            foreach (var vm in _conversionVms)
                dataGridViewEx1.Rows.Add(new ConversionVmRow(vm));
        }

        private void dataGridViewEx1_SelectionChanged(object sender, EventArgs e)
        {
            buttonOk.Enabled = ConversionVm != null;
        }

        private void dataGridViewEx1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.ColumnIndex >= dataGridViewEx1.ColumnCount ||
                e.RowIndex < 0 || e.RowIndex >= dataGridViewEx1.RowCount)
                return;

            if (ConversionVm != null)
                DialogResult = DialogResult.OK;
        }


        private class ConversionVmRow : DataGridViewRow
        {
            private readonly DataGridViewImageCell _cellImage = new DataGridViewImageCell();
            private readonly DataGridViewTextBoxCell _cellVm = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _cellUuid = new DataGridViewTextBoxCell();

            public VM Vm { get; }

            public ConversionVmRow(VM vm)
            {
                Cells.AddRange(_cellImage, _cellVm, _cellUuid);
                Vm = vm;

                _cellImage.Value = Images.GetImage16For(Images.GetIconFor(Vm));
                _cellVm.Value = vm.Name();
                _cellUuid.Value = vm.uuid;
            }
        }
    }
}
