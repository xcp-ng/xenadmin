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
using System.ComponentModel;
using XenAdmin.Actions;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Controls
{
    public class SrPicker : CustomTreeView
    {
        // Migrate is the live VDI move operation
        public enum SRPickerType { VM, InstallFromTemplate, MoveOrCopy, Migrate, LunPerVDI };
        private SRPickerType usage = SRPickerType.VM;
        
        //Used in the MovingVDI usage
        private VDI[] existingVDIs;
        public void SetExistingVDIs(VDI[] vdis)
        {
            existingVDIs = vdis;
        }

        private IXenConnection connection;
        private Host affinity;

        public long DiskSize = 0;

        private readonly CollectionChangeEventHandler SR_CollectionChangedWithInvoke;

        public SrPicker()
        {
            SR_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(SR_CollectionChanged);
        }

        public override bool ShowCheckboxes => false;

        public override bool ShowDescription => true;

        public override bool ShowImages => true;

        public override int NodeIndent => 3;

        public SRPickerType Usage
        {
            set => usage = value;
        }

        /// <summary>
        /// For new disk dialog only
        /// </summary>
        public IXenConnection Connection
        {
            get
            {
                return connection;
            }
            set
            {
                if (value == null)
                    return;
                connection = value;

                Pool pool = Helpers.GetPoolOfOne(connection);
                if (pool != null)
                {
                    pool.PropertyChanged -= Server_PropertyChanged;
                    pool.PropertyChanged += Server_PropertyChanged;
                }

                connection.Cache.RegisterCollectionChanged<SR>(SR_CollectionChangedWithInvoke);

                refresh();
                foreach (SrPickerItem srItem in Items)
                {
                    SrRefreshAction a = new SrRefreshAction(srItem.TheSR, true);
                    a.RunAsync();
                }
            }
        }

        public SR SR => SelectedItem is SrPickerItem srpITem && srpITem.Enabled ? srpITem.TheSR : null;

        public SR DefaultSR = null;

		public void SetAffinity(Host host)
		{
			affinity = host;
			refresh();
		}

    	private void refresh()
        {
            Program.AssertOnEventThread();

            SR selectedSr = SR;
            bool selected = false;
            BeginUpdate();
            try
            {
                ClearAllNodes();

                foreach (SR sr in connection.Cache.SRs)
                {
                    var item = SrPickerItem.Create(sr, usage, affinity, DiskSize, existingVDIs);
                    if (item.Show)
                        AddNode(item);
                    foreach (PBD pbd in sr.Connection.ResolveAll(sr.PBDs))
                    {
                        if (pbd != null)
                        {
                            pbd.PropertyChanged -= Server_PropertyChanged;
                            pbd.PropertyChanged += Server_PropertyChanged;
                        }
                    }
                    sr.PropertyChanged -= Server_PropertyChanged;
                    sr.PropertyChanged += Server_PropertyChanged;
                }
            }
            finally
            {
                EndUpdate();
            }

            if (selectedSr != null)
            {
                foreach (SrPickerItem node in Items)
                {
                    if (node.TheSR != null && node.TheSR.uuid == selectedSr.uuid)
                    {
                        SelectedItem = node;
                        selected = true;
                        break;
                    }
                }
            }

            if (!selected && Items.Count > 0)
            {// If no selection made, select default SR
                if (!selectDefaultSR())
                {
                    // If no default SR, select first entry in list
                    SelectedIndex = 0;
                }
            }
        }

        public void UpdateDiskSize()
        {
            Program.AssertOnEventThread();
            try
            {
                foreach (SrPickerItem node in Items)
                {
                    node.UpdateDiskSize(DiskSize);
                }
            }
            finally
            {
                Refresh();
            }
        }

        private void Server_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label" || e.PropertyName == "PBDs" || e.PropertyName == "physical_utilisation" || e.PropertyName == "currently_attached" || e.PropertyName == "default_SR")
                Program.Invoke(this, refresh);
        }

        void SR_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.Invoke(this, refresh);
        }

        private void UnregisterHandlers()
        {
            if (connection == null)
                return;

            var pool = Helpers.GetPoolOfOne(connection);
            if (pool != null)
                pool.PropertyChanged -= Server_PropertyChanged;

            foreach (var sr in connection.Cache.SRs)
            {
                foreach (var pbd in sr.Connection.ResolveAll(sr.PBDs))
                {
                    if (pbd != null)
                        pbd.PropertyChanged -= Server_PropertyChanged;
                }
                sr.PropertyChanged -= Server_PropertyChanged;
            }

            connection.Cache.DeregisterCollectionChanged<SR>(SR_CollectionChangedWithInvoke);
        }

        /// <summary>
        /// Selects the default SR, if it exists.
        /// </summary>
        /// <returns>true if the default SR was selected, otherwise false.</returns>
        public bool selectDefaultSR()
        {
            if (DefaultSR == null)
                return false;

            foreach (SrPickerItem node in Items)
            {
                if (node.TheSR != null && node.TheSR == DefaultSR && node.Enabled)
                {
                    SelectedItem = node;
                    return true;
                }
            }
            return false;
        }

        internal void selectSRorNone(SR TheSR)
        {
            foreach (SrPickerItem node in Items)
            {
                if (node.TheSR != null && node.TheSR.opaque_ref == TheSR.opaque_ref)
                {
                    SelectedItem = node;
                    return;
                }
            }
        }

        internal void selectDefaultSROrAny()
        {
            if (selectDefaultSR())
                return;
            foreach (SrPickerItem item in Items)
            {
                if (item == null)
                    continue;
                if (item.Enabled)
                {
                    selectSRorNone(item.TheSR);
                    return;
                }
            }
        }

        public void selectSRorDefaultorAny(SR sr)
        {
            if (sr != null)
            {
                foreach (SrPickerItem node in Items)
                {
                    if (node.TheSR != null && node.TheSR.opaque_ref == sr.opaque_ref)
                    {
                        SelectedItem = node;
                        return;
                    }
                }
            }
            selectDefaultSROrAny();
        }

        public bool ValidSelectionExists
        {
            get
            {
                foreach (SrPickerItem item in Items)
                {
                    if (item == null)
                        continue;
                    if (item.Enabled)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                UnregisterHandlers();

            base.Dispose(disposing);
        }
    }
}
