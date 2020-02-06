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
using System.Drawing;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Actions;


namespace XenAdmin.Dialogs
{
    public partial class AttachDiskDialog : XenDialogBase
    {
        private readonly VM TheVM;
        private DiskListVdiItem oldSelected = null;
        private bool oldROState = false;

        private Pool poolofone;
        private readonly CollectionChangeEventHandler SR_CollectionChangedWithInvoke;
        public AttachDiskDialog(VM vm) : base(vm.Connection)
        {
            TheVM = vm;

            InitializeComponent();
            
            DiskListTreeView.ShowCheckboxes = false;
            DiskListTreeView.ShowDescription = true;
            DiskListTreeView.ShowImages = true;
            DiskListTreeView.SelectedIndexChanged += DiskListTreeView_SelectedIndexChanged;
            SR_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(SR_CollectionChanged);
            connection.Cache.RegisterCollectionChanged<SR>(SR_CollectionChangedWithInvoke);

            poolofone = Helpers.GetPoolOfOne(connection);
            if (poolofone != null)
                poolofone.PropertyChanged += Server_Changed;

            readonlyCheckboxToolTipContainer.SuppressTooltip = false;
            DeactivateControlsForHvmVm();

            BuildList();
            DiskListTreeView_SelectedIndexChanged(null, null);
        }

        void SR_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.Invoke(this, BuildList);
        }

        private bool skipSelectedIndexChanged;
        void DiskListTreeView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(skipSelectedIndexChanged)
                return;

            if (DiskListTreeView.SelectedItem is DiskListSrItem || DiskListTreeView.SelectedItem == null)
            {
                //As we're changing the index below - stall the SelectedIndexChanged event until we're done
                skipSelectedIndexChanged = true;
                try
                {
                    DiskListTreeView.SelectedItem = oldSelected;
                    OkBtn.Enabled = DiskListTreeView.SelectedItem != null;
                    DeactivateAttachButtonIfHvm();
                }
                finally
                {
                    //Ensure the lock is released
                    skipSelectedIndexChanged = false;
                }
                    
                return;
            }

            DiskListVdiItem item = DiskListTreeView.SelectedItem as DiskListVdiItem;
            ReadOnlyCheckBox.Enabled = !item.ForceReadOnly;
            ReadOnlyCheckBox.Checked = item.ForceReadOnly ? true : oldROState;
            OkBtn.Enabled = true;
            oldSelected = item;
            DeactivateControlsForHvmVm();
        }

        private void DeactivateControlsForHvmVm()
        {
            DeactivateReadOnlyCheckBoxForHvmVm();
            DeactivateAttachButtonIfHvm();
        }

        private void DeactivateAttachButtonIfHvm()
        {
            if (!TheVM.IsHVM())
                return;

            DiskListVdiItem vdiItem = DiskListTreeView.SelectedItem as DiskListVdiItem;
            if (vdiItem != null && vdiItem.ForceReadOnly)
                OkBtn.Enabled = false;
        }

        private void DeactivateReadOnlyCheckBoxForHvmVm()
        {
            if (!TheVM.IsHVM()) 
                return;

            readonlyCheckboxToolTipContainer.SetToolTip(Messages.ATTACH_DISK_DIALOG_READONLY_DISABLED_FOR_HVM);
            ReadOnlyCheckBox.Enabled = false;
        }

        private void BuildList()
        {
            Program.AssertOnEventThread();

            DiskListVdiItem lastSelected = DiskListTreeView.SelectedItem as DiskListVdiItem;

            String oldRef = "";
            if (lastSelected != null)
                oldRef = lastSelected.TheVDI.opaque_ref;

            DiskListTreeView.BeginUpdate();
            try
            {
                DiskListTreeView.ClearAllNodes();

                foreach (SR sr in connection.Cache.SRs)
                {
                    DiskListSrItem item = new DiskListSrItem(sr, TheVM);
                    if (item.Show)
                    {
                        DiskListTreeView.AddNode(item);
                        foreach (VDI TheVDI in sr.Connection.ResolveAllShownXenModelObjects(sr.VDIs, Properties.Settings.Default.ShowHiddenVMs))
                        {
                            DiskListVdiItem VDIitem = new DiskListVdiItem(TheVDI);
                            if (VDIitem.Show)
                                DiskListTreeView.AddChildNode(item, VDIitem);
                            TheVDI.PropertyChanged -= new PropertyChangedEventHandler(Server_Changed);
                            TheVDI.PropertyChanged += new PropertyChangedEventHandler(Server_Changed);
                        }
                    }
                    sr.PropertyChanged -= new PropertyChangedEventHandler(Server_Changed);
                    sr.PropertyChanged += new PropertyChangedEventHandler(Server_Changed);
                }
            }
            finally
            {
                DiskListTreeView.EndUpdate();
                DiskListTreeView.SelectedItem = SelectByRef(oldRef);
            }
        }

        private object SelectByRef(string oldRef)
        {
            for (int i = 0; i < DiskListTreeView.Items.Count; i++)
            {
                DiskListVdiItem item = DiskListTreeView.Items[i] as DiskListVdiItem;
                if (item == null)
                    continue;
                else if (item.TheVDI.opaque_ref == oldRef)
                    return item;
            }
            return null;
        }

        private void Server_Changed(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label" || e.PropertyName == "VDIs" || e.PropertyName == "VBDs" || e.PropertyName == "default_SR")
            {
                Program.Invoke(this, BuildList);
            }
        }

        private void ReadOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ReadOnlyCheckBox.Enabled)
                oldROState = ReadOnlyCheckBox.Checked;
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            DiskListVdiItem item = DiskListTreeView.SelectedItem as DiskListVdiItem;
            if (item == null)
                return;

            VDI TheVDI = item.TheVDI;

            // Run this stuff off the event thread, since it involves a server call
            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                // Get a spare userdevice
                string[] uds = VM.get_allowed_VBD_devices(connection.DuplicateSession(), TheVM.opaque_ref);
                if (uds.Length == 0)
                {
                    Program.Invoke(Program.MainWindow, delegate()
                    {
                        using (var dlg = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(
                                SystemIcons.Error,
                                FriendlyErrorNames.VBDS_MAX_ALLOWED, Messages.DISK_ATTACH)))
                        {
                            dlg.ShowDialog(Program.MainWindow);
                        }
                    });

                    return;
                }
                string ud = uds[0];

                VBD vbd = new VBD();
                vbd.VDI = new XenRef<VDI>(TheVDI);
                vbd.VM = new XenRef<VM>(TheVM);
                vbd.bootable = ud == "0";
                vbd.device = "";
                vbd.SetIsOwner(TheVDI.VBDs.Count == 0);
                vbd.empty = false;
                vbd.userdevice = ud;
                vbd.type = vbd_type.Disk;
                vbd.mode = ReadOnlyCheckBox.Checked ? vbd_mode.RO : vbd_mode.RW;
                vbd.unpluggable = true;

                // Try to hot plug the VBD.
                var action = new VbdSaveAndPlugAction(TheVM, vbd, TheVDI.Name(), null, false);
                action.ShowUserInstruction += Action_ShowUserInstruction;
                action.RunAsync();
            });

            Close();
        }

        private void Action_ShowUserInstruction(string message)
        {
            Program.Invoke(Program.MainWindow, () =>
            {
                if (!Program.RunInAutomatedTestMode)
                {
                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Information, message)))
                    {
                        dlg.ShowDialog(Program.MainWindow);
                    }
                }
            });
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        private void AttachDiskDialog_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            // unsubscribe to events
            foreach (SR sr in connection.Cache.SRs)
            {
                sr.PropertyChanged -= new PropertyChangedEventHandler(Server_Changed);
                foreach (VDI TheVDI in sr.Connection.ResolveAllShownXenModelObjects(sr.VDIs, Properties.Settings.Default.ShowHiddenVMs))
                {
                    TheVDI.PropertyChanged -= new PropertyChangedEventHandler(Server_Changed);
                }
            }

            DiskListTreeView.SelectedIndexChanged -= DiskListTreeView_SelectedIndexChanged;

            if (poolofone != null)
                poolofone.PropertyChanged -= Server_Changed;

            connection.Cache.DeregisterCollectionChanged<SR>(SR_CollectionChangedWithInvoke);
        }
    }

    public class DiskListSrItem : CustomTreeNode
    {
        public enum DisabledReason { None, Broken, NotSeen };

        public SR TheSR;
        public VM TheVM;
        public bool Show = false;
        public DisabledReason Reason;

        public DiskListSrItem(SR sr, VM vm)
            : base(false)
        {
            TheSR = sr;
            TheVM = vm;
            Update();
        }

        private void Update()
        {
            Text = TheSR.NameWithoutHost();
            Image = Images.GetImage16For(Images.GetIconFor(TheSR));

            Host affinity = TheVM.Connection.Resolve<Host>(TheVM.affinity);
            Host activeHost = TheVM.Connection.Resolve<Host>(TheVM.resident_on);
            if (!TheSR.Show(Properties.Settings.Default.ShowHiddenVMs))
            {
                Show = false;
            }
            else if (TheSR.content_type != SR.Content_Type_ISO && !((affinity != null && !TheSR.CanBeSeenFrom(affinity)) || (activeHost != null && TheVM.power_state == vm_power_state.Running && !TheSR.CanBeSeenFrom(activeHost))  || TheSR.IsBroken(false) || TheSR.PBDs.Count < 1))
            {
                Description = "";
                Enabled = true;
                Show = true;
                Reason = DisabledReason.None;
            }
            else if (((affinity != null && !TheSR.CanBeSeenFrom(affinity)) || (activeHost != null && TheVM.power_state == vm_power_state.Running 
                && !TheSR.CanBeSeenFrom(activeHost))) && !TheSR.IsBroken(false) && TheSR.PBDs.Count < 1)
            {
                Description = string.Format(Messages.SR_CANNOT_BE_SEEN, Helpers.GetName(affinity));
                Enabled = false;
                Show = true;
                Reason = DisabledReason.NotSeen;
            }
            else if (TheSR.IsBroken(false) && TheSR.PBDs.Count >= 1)
            {
                Description = Messages.SR_IS_BROKEN;
                Enabled = false;
                Show = true;
                Reason = DisabledReason.Broken;
            }
            else
            {
                Show = false;
            }
        }

        protected override int SameLevelSortOrder(CustomTreeNode other)
        {
            DiskListSrItem otherItem = other as DiskListSrItem;
            if (otherItem == null) //shouldnt ever happen!!!
                return -1;
            int rank = this.SrRank() - otherItem.SrRank();
            if (rank == 0)
                return base.SameLevelSortOrder(other);
            else
                return rank;
        }

        public int SrRank()
        {
            if (Enabled && TheSR.VDIs.Count > 0)
                return 0;
            else
                return 1 + (int)Reason;
        }
    }

    public class DiskListVdiItem : CustomTreeNode, IEquatable<DiskListVdiItem>
    {
        public VDI TheVDI;
        public bool Show = false;
        public bool ForceReadOnly = false;

        public DiskListVdiItem(VDI vdi)
        {
            TheVDI = vdi;
            Update();
        }

        private void Update()
        {
            if (TheVDI.type == vdi_type.crashdump || TheVDI.type == vdi_type.ephemeral || TheVDI.type == vdi_type.suspend)
                Show = false;
            else if ((TheVDI.VBDs.Count > 0 && !TheVDI.sharable))
            {
                bool allRO = true;
                foreach (VBD vbd in TheVDI.Connection.ResolveAll<VBD>(TheVDI.VBDs))
                {
                    if (!vbd.IsReadOnly() && vbd.currently_attached)
                    {
                        allRO = false;
                        break;
                    }
                }
                Show = allRO;
                if (Show)
                {
                    ForceReadOnly = true;
                    Text = String.IsNullOrEmpty(TheVDI.Name()) ? Messages.NO_NAME : TheVDI.Name();
                    if (!string.IsNullOrEmpty(TheVDI.Description()))
                        Description = string.Format(Messages.ATTACHDISK_SIZE_DESCRIPTION, TheVDI.Description(), Util.DiskSizeString(TheVDI.virtual_size));
                    else
                        Description = Util.DiskSizeString(TheVDI.virtual_size);
                    Image = Images.GetImage16For(Icons.VDI);
                }
            }
            else
            {
                Show = true;
                ForceReadOnly = TheVDI.read_only;
                Text = String.IsNullOrEmpty(TheVDI.Name()) ? Messages.NO_NAME : TheVDI.Name();
                if (!string.IsNullOrEmpty(TheVDI.Description()))
                    Description = string.Format(Messages.ATTACHDISK_SIZE_DESCRIPTION, TheVDI.Description(), Util.DiskSizeString(TheVDI.virtual_size));
                else
                    Description = Util.DiskSizeString(TheVDI.virtual_size);
                Image = Images.GetImage16For(Icons.VDI);
            }
        }

        public bool Equals(DiskListVdiItem other)
        {
            return this.TheVDI.opaque_ref == other.TheVDI.opaque_ref;
        }
    }
}