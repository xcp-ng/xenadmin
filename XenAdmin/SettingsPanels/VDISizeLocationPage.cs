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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAPI;


namespace XenAdmin.SettingsPanels
{
    public partial class VDISizeLocationPage : UserControl, IEditPage
    {
        private VDI vdi;
        private bool _validToSave;

        public VDISizeLocationPage()
        {
            InitializeComponent();
            Text = Messages.SIZE_AND_LOCATION;
        }

        public String SubText
        {
            get
            {
                return string.Format(Messages.SIZE_LOCATION_SUB,
                    Util.DiskSizeString(diskSpinner1.CanResize ? diskSpinner1.SelectedSize : vdi.virtual_size, 2),
                    vdi.Connection.Resolve<SR>(vdi.SR));
            }
        }

        public Image Image => Properties.Resources._000_VirtualStorage_h32bit_16;

        public bool ValidToSave => _validToSave;

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            vdi = clone as VDI;
            if (vdi == null)
                return;

            SR sr = vdi.Connection.Resolve<SR>(vdi.SR);
            labelLocationValueRO.Text = string.Format("'{0}'", sr.NameWithoutHost());

            diskSpinner1.Populate(vdi.virtual_size, vdi.virtual_size);

            var canResize = vdi.allowed_operations.Contains(vdi_operations.resize) ||
                            vdi.allowed_operations.Contains(vdi_operations.resize_online);

            diskSpinner1.CanResize = canResize;
            tableLayoutPanelInfo.Visible = !canResize;
        }

        public bool HasChanged => diskSpinner1.CanResize && diskSpinner1.SelectedSize - vdi.virtual_size > 10 * Util.BINARY_MEGA;

        public void ShowLocalValidationMessages()
        {
        }

        public void Cleanup()
        {
        }

        public AsyncAction SaveSettings()
        {
            if (!HasChanged)
                return null;

            if (vdi.allowed_operations.Contains(vdi_operations.resize))
                return new DelegatedAsyncAction(
                    vdi.Connection,
                    Messages.ACTION_CHANGE_DISK_SIZE,
                    string.Format(Messages.ACTION_CHANGING_DISK_SIZE_FOR, vdi),
                    string.Format(Messages.ACTION_CHANGED_DISK_SIZE_FOR, vdi),
                    delegate(Session session) { VDI.resize(session, vdi.opaque_ref, diskSpinner1.SelectedSize); },
                    true,
                    "vdi.resize"
                );
            else
            {
                return new DelegatedAsyncAction(
                    vdi.Connection,
                    Messages.ACTION_CHANGE_DISK_SIZE,
                    string.Format(Messages.ACTION_CHANGING_DISK_SIZE_FOR, vdi),
                    string.Format(Messages.ACTION_CHANGED_DISK_SIZE_FOR, vdi),
                    delegate(Session session) { VDI.resize_online(session, vdi.opaque_ref, diskSpinner1.SelectedSize); },
                    true,
                    "vdi.resize_online"
                );
            }
        }

        private void diskSpinner1_SelectedSizeChanged()
        {
            if (!diskSpinner1.IsSizeValid)
            {
                _validToSave = false;
                return;
            }

            SR sr = vdi.Connection.Resolve(vdi.SR);
            SM sm = sr == null ? null : sr.GetSM();

            bool vdiSizeUnlimited = sm != null && Array.IndexOf(sm.capabilities, "LARGE_VDI") != -1;
            if (!vdiSizeUnlimited && diskSpinner1.SelectedSize > SR.DISK_MAX_SIZE)
            {
                diskSpinner1.SetError(string.Format(Messages.DISK_TOO_BIG_MAX_SIZE, Util.DiskSizeString(SR.DISK_MAX_SIZE)));
                _validToSave = false;
                return;
            }

            bool isThinlyProvisioned = sm != null && Array.IndexOf(sm.capabilities, "THIN_PROVISIONING") != -1;
            if (!isThinlyProvisioned && sr != null && diskSpinner1.SelectedSize - vdi.virtual_size > sr.FreeSpace())
            {
                diskSpinner1.SetError(Messages.DISK_TOO_BIG);
                _validToSave = false;
                return;
            }

            _validToSave = true;
            diskSpinner1.SetError(null);
        }
    }
}
