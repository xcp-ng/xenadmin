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
using XenAdmin.Actions;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Dialogs
{
    /// <summary>
    /// When an existing SR is found on an iSCSI LUN, asks the user if they want
    /// to attach the SR, wipe the LUN and create a new SR, or do nothing (cancel).
    /// 
    /// DialogResult.Yes - Reattach
    /// DialogResult.No - Format
    /// DialogResult.Cancel - Cancel
    /// </summary>
    public partial class IscsiChoicesDialog : XenDialogBase
    {
        public IscsiChoicesDialog(IXenConnection connection, SR.SRInfo srInfo, SR.SRTypes existingSrType, SR.SRTypes requestedSrType)
            : base(connection)
        {
            InitializeComponent();

            this.labelSRinfo.Text = String.Format(Messages.ISCSI_DIALOG_SR_DETAILS,
                Util.DiskSizeString(srInfo.Size), srInfo.UUID);

            this.labelMessage.Text = String.Format(Messages.EXISTING_SR_FOUND_ON_LUN, SR.getFriendlyTypeName(existingSrType),
                SR.getFriendlyTypeName(requestedSrType));
        }

        public IscsiChoicesDialog(IXenConnection connection, FibreChannelDevice dev)
            : base(connection)
        {
            InitializeComponent();

            this.labelSRinfo.Text = String.Format(Messages.ISCSI_DIALOG_SR_DETAILS_FOR_FIBRECHANNEL,
                dev.Vendor, dev.Serial, string.IsNullOrEmpty(dev.SCSIid) ? dev.Path : dev.SCSIid, Util.DiskSizeString(dev.Size));
        }
    }
}
