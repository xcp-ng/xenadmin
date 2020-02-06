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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Dialogs;
using System.Drawing;

namespace XenAdmin.Commands
{
    /// <summary>
    /// Assign VMs to a group of VMs (e.g., to a VMPP or a vApp)
    /// </summary>
    internal class AssignGroupToolStripMenuItem<T> : CommandToolStripMenuItem where T : XenObject<T>
    {
        public AssignGroupToolStripMenuItem()
            : base(new AssignVMsToGroup(), false)
        {
            base.DropDownItems.Add(new ToolStripMenuItem());
        }

        public AssignGroupToolStripMenuItem(IMainWindow mainWindow, SelectedItemCollection selection, bool inContextMenu)
            : base(new AssignVMsToGroup(mainWindow, selection), inContextMenu)
        {
            base.DropDownItems.Add(new ToolStripMenuItem());
        }

        protected override void OnDropDownOpening(EventArgs e)
        {
            base.DropDownItems.Clear();

            var cmd = new NewGroupCommand(Command.MainWindowCommandInterface, Command.GetSelection());
            var item = new CommandToolStripMenuItem(cmd);
            base.DropDownItems.Add(item);

            T[] groups = VMGroup<T>.GroupsInCache(Command.GetSelection()[0].Connection.Cache);

            Array.Sort(groups);

            for (int index = 0, offset = 0; index < groups.Length; index++)
            {
                T group = groups[index];

                /* do not add unsupported policies to the drop down for VMSS */
                XenAPI.VMSS policy = group as VMSS;
                if (policy != null && policy.type == vmss_type.snapshot_with_quiesce)
                {
                    List<VM> vms = Command.GetSelection().AsXenObjects<VM>();
                    bool doNotInclude = vms.Any(vm => !vm.allowed_operations.Contains(vm_operations.snapshot_with_quiesce));
                    if (doNotInclude)
                    {
                        offset--;
                        continue;
                    }
                }

                var menuText = (index + offset) < 9
                    ? String.Format(Messages.DYNAMIC_MENUITEM_WITH_ACCESS_KEY, (index + offset) + 1, group.Name())
                    : String.Format(Messages.DYNAMIC_MENUITEM_WITHOUT_ACCESS_KEY, group.Name());

                var cmdGroup = new AssignGroupToVMCommand(Command.MainWindowCommandInterface, Command.GetSelection(), group, menuText);
                var itemGroup = new CommandToolStripMenuItem(cmdGroup);
                if (Command.GetSelection().Count == 1 &&
                    VMGroup<T>.VmToGroup((VM)Command.GetSelection()[0].XenObject).opaque_ref == group.opaque_ref)
                    itemGroup.Checked = true;
                base.DropDownItems.Add(itemGroup);
            }
            if (base.DropDownItems.Count > 1)
                base.DropDownItems.Insert(1, new ToolStripSeparator());
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

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new event EventHandler DropDownOpening
        {
            add
            {
                throw new InvalidOperationException();
            }
            remove
            {
                throw new InvalidOperationException();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ToolStripItemCollection DropDownItems
        {
            get
            {
                throw new InvalidOperationException();
            }
        }

        public class AssignGroupToVMCommand : Command
        {
            private readonly T _group;
            private readonly string _menuText;

            public AssignGroupToVMCommand(IMainWindow mainWindow, SelectedItemCollection selection, T group, string menuText)
                : base(mainWindow, selection)
            {
                _group = group;
                _menuText = menuText;
            }

            public override string MenuText
            {
                get { return (String.IsNullOrEmpty(_menuText) ? _group.Name() : _menuText).Ellipsise(30); }
            }

            protected override string EnabledToolTipText
            {
                get { return _group.Name(); }
            }

            protected override bool CanExecuteCore(SelectedItemCollection selection)
            {
                return true;
            }

            /// <summary>
            /// Find out if any VMs are already assigned to a different group, and if so, check they can be moved
            /// </summary>
            /// <param name="vms">All the VMs to be assigned to the group</param>
            /// <param name="group">The group to assign the VMs to (null for a new group)</param>
            /// <param name="groupName">The name of the group to assign the VMs to</param>
            /// <returns>Whether the user is happy to proceed</returns>
            public static bool ChangesOK(List<VM> vms, T group, string groupName)
            {
                var vmsWithExistingGroup = vms.FindAll(vm =>
                {
                    T oldGroup = vm.Connection.Resolve(VMGroup<T>.VmToGroup(vm));
                    return oldGroup != null && (group == null || oldGroup.opaque_ref != group.opaque_ref);
                });

                if (vmsWithExistingGroup.Count == 0)
                    return true;

                string text;
                if (vmsWithExistingGroup.Count == 1)
                {
                    VM vm = vmsWithExistingGroup[0];
                    T oldGroup = vm.Connection.Resolve(VMGroup<T>.VmToGroup(vm));
                    text = string.Format(VMGroup<T>.ChangeOneWarningString,
                        vm.Name().Ellipsise(250), oldGroup.Name().Ellipsise(250), groupName.Ellipsise(250));
                }
                else
                {
                    text = string.Format(VMGroup<T>.ChangeMultipleWarningString, groupName.Ellipsise(250));
                }

                DialogResult dialogResult;
                using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Warning, text, VMGroup<T>.ChangeVMsGroupString),
                        ThreeButtonDialog.ButtonYes,
                        ThreeButtonDialog.ButtonNo))
                {
                    dialogResult = dlg.ShowDialog();
                }
                return dialogResult == DialogResult.Yes;
            }

            protected override void ExecuteCore(SelectedItemCollection selection)
            {
                // remove single VM from group
                if (selection.Count == 1)
                {
                    XenRef<VM> vmRefInGroup = VMGroup<T>.GroupToVMs(_group).FirstOrDefault(vmRef => vmRef.opaque_ref == selection[0].XenObject.opaque_ref);
                    if (vmRefInGroup != null)
                    {
                        var vmRefs = new List<XenRef<VM>> { vmRefInGroup };
                        VMGroup<T>.RemoveVMsFromGroupAction(_group, vmRefs).RunAsync();
                        return;
                    }
                }

                if (!ChangesOK(selection.AsXenObjects<VM>(), _group, _group.Name()))
                    return;

                var selectedRefVMs = selection.AsXenObjects().ConvertAll<XenRef<VM>>(converterVMRefs);
                selectedRefVMs.AddRange(VMGroup<T>.GroupToVMs(_group));
                VMGroup<T>.AssignVMsToGroupAction(_group, selectedRefVMs, false).RunAsync();
            }

            private XenRef<VM> converterVMRefs(IXenObject input)
            {
                var vm = input as VM;
                if (vm == null)
                    return null;
                return new XenRef<VM>(vm.opaque_ref);
            }
        }

        public class NewGroupCommand : Command
        {
            public NewGroupCommand(IMainWindow mainWindowCommandInterface, SelectedItemCollection getSelection)
                : base(mainWindowCommandInterface, getSelection)
            {
            }

            protected override void ExecuteCore(SelectedItemCollection selection)
            {
                MainWindowCommandInterface.ShowPerConnectionWizard(selection[0].Connection,
                    VMGroup<T>.NewGroupWizard(Helpers.GetPoolOfOne(selection[0].Connection), selection.AsXenObjects<VM>()));
            }

            public override string MenuText
            {
                get { return VMGroup<T>.NewGroupString; }
            }

            protected override bool CanExecuteCore(SelectedItemCollection selection)
            {
                return !Helpers.FeatureForbidden(selection.FirstAsXenObject.Connection, VMGroup<T>.FeatureRestricted)
                    && (selection.PoolAncestor != null || selection.HostAncestor != null); //CA-61207: this check ensures there's no cross-pool selection 
            }
        }

        private class AssignVMsToGroup : Command
        {
            /// <summary>
            /// Initializes a new instance of this Command. The parameter-less constructor is required if 
            /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
            /// </summary>
            public AssignVMsToGroup()
            {
            }

            public AssignVMsToGroup(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
                : base(mainWindow, selection)
            {
            }

            public bool CanExecute(VM vm)
            {
                return vm != null && vm.is_a_real_vm() && !vm.Locked && VMGroup<T>.FeaturePossible(vm.Connection) &&
                       !Helpers.FeatureForbidden(vm.Connection, VMGroup<T>.FeatureRestricted);
            }
            protected override bool CanExecuteCore(SelectedItemCollection selection)
            {
                return selection.AllItemsAre<VM>() && selection.AtLeastOneXenObjectCan<VM>(CanExecute)
                    && (selection.PoolAncestor != null || selection.HostAncestor != null); //CA-61207: this check ensures there's no cross-pool selection 
            }

            public override string MenuText
            {
                get { return VMGroup<T>.AssignMainMenuString; }
            }

            public override string ContextMenuText
            {
                get { return VMGroup<T>.AssignContextMenuString; }
            }
        }
    }

    /// <summary>
    /// Class used for the benefit of visual studio's form designer which has trouble with generic controls
    /// </summary>
    internal sealed class AssignGroupToolStripMenuItemVMSS : AssignGroupToolStripMenuItem<VMSS>
    { }

    /// <summary>
    /// Class used for the benefit of visual studio's form designer which has trouble with generic controls
    /// </summary>
    internal sealed class AssignGroupToolStripMenuItemVM_appliance : AssignGroupToolStripMenuItem<VM_appliance>
    { }
}