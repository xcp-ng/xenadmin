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
using XenAdmin.Controls;
using XenAdmin.Controls.Ballooning;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.TabPages
{
    public partial class BallooningPage : BaseTabPage
    {
        private const int ROW_GAP = 10;

        public BallooningPage()
        {
            InitializeComponent();
            // http://alt.pluralsight.com/wiki/default.aspx/Craig.FlickerFreeControlDrawing
            SetStyle(ControlStyles.DoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.Opaque |
                ControlStyles.UserPaint, true);
            Text = Messages.DYNAMIC_MEMORY_CONTROL;
            Host_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(Host_CollectionChanged);
            VM_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(VM_CollectionChanged);
        }

        IXenObject xenObject;
        List<Host> hosts = new List<Host>();
        List<VM> vms = new List<VM>();
        private readonly CollectionChangeEventHandler Host_CollectionChangedWithInvoke;

        //solution from: http://stackoverflow.com/questions/2612487/how-to-fix-the-flickering-in-user-controls
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public IXenObject XenObject
        {
            set
            {
                System.Diagnostics.Trace.Assert(value is Pool || value is Host || value is VM);
                xenObject = value;

                UnregisterHandlers();

                vms.Clear();
                hosts.Clear();

                if (value is VM)
                    vms.Add((VM)value);

                else if (value is Host)
                {
                    Host host = value as Host;
                    hosts.Add(host);
                    foreach (VM vm in host.Connection.Cache.VMs)
                    {
                        if (VMWanted(vm, host))
                            vms.Add(vm);
                    }
                }

                else  // value is XenObject<Pool>
                {
                    value.Connection.Cache.RegisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);
                    hosts.AddRange(value.Connection.Cache.Hosts);
                }

                value.Connection.XenObjectsUpdated += XenObjectsUpdated;

                foreach (Host host in hosts)
                    RegisterHostHandlers(host);

                foreach (VM vm in vms)
                    RegisterVMHandlers(vm);

                Program.BeginInvoke(Program.MainWindow, () =>
                {
                    Rebuild();
                });
            }
        }

        public override string HelpID => "TabPageBallooning";

        private bool VMWanted(VM vm, Host host)
        {
            return vm.is_a_real_vm() && vm.Show(Properties.Settings.Default.ShowHiddenVMs) && vm.Home() == host;
        }

        private readonly CollectionChangeEventHandler VM_CollectionChangedWithInvoke;
        private void RegisterHostHandlers(Host host)
        {
            host.PropertyChanged += host_PropertyChanged;
            host.Connection.Cache.RegisterCollectionChanged<VM>(VM_CollectionChangedWithInvoke);
            foreach (VM vm in host.Connection.Cache.VMs)
                RegisterAllVMHandlers(vm);
        }

        private void host_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label" && hosts.Count >= 2)
                _rebuild_needed = true;  // might change the sort order
        }

        private void VM_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Remove)
            {
                VM vm = e.Element as VM;
                UnregisterVMHandlers(vm);
            }
            XenObject = xenObject;  // have to recalculate list of VMs etc.
        }

        private void Host_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Remove)
            {
                Host host = e.Element as Host;
                UnregisterHostHandlers(host);
            }
            XenObject = xenObject;
        }

        private void RegisterVMHandlers(VM vm)
        {
            vm.PropertyChanged -= vm_PropertyChanged;
            vm.PropertyChanged += vm_PropertyChanged;
            RegisterVMGuestMetrics(vm);
        }

        private void RegisterAllVMHandlers(VM vm)
        {
            vm.PropertyChanged -= vm_PropertyChanged_allVMs;
            vm.PropertyChanged += vm_PropertyChanged_allVMs;
        }

        private void RegisterVMGuestMetrics(VM vm)
        {
            VM_guest_metrics guest_metrics = vm.Connection.Resolve(vm.guest_metrics);
            if (guest_metrics != null)
            {
                guest_metrics.PropertyChanged -= guest_metrics_PropertyChanged;
                guest_metrics.PropertyChanged += guest_metrics_PropertyChanged;
            }
        }

        private void UnregisterHandlers()
        {
            foreach (Host host in hosts)
                UnregisterHostHandlers(host);

            if (xenObject != null)
            {
                Pool pool = xenObject as Pool;
                if (pool != null)
                    pool.Connection.Cache.DeregisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);

                xenObject.Connection.XenObjectsUpdated -= XenObjectsUpdated;
                foreach (VM vm in xenObject.Connection.Cache.VMs)
                    UnregisterVMHandlers(vm);
            }

            foreach (VM vm in vms)
                UnregisterVMHandlers(vm);  // Should duplicate above line, but let's be safe

            foreach (Control c in pageContainerPanel.Controls)
            {
                VMMemoryRow vmRow = c as VMMemoryRow;
                if (vmRow != null)
                {
                    vmRow.UnregisterHandlers();
                    continue;
                }

                HostMemoryRow hostRow = c as HostMemoryRow;
                if (hostRow != null)
                {
                    hostRow.UnregisterHandlers();
                }
            }
        }

        private void UnregisterHostHandlers(Host host)
        {
            host.Connection.Cache.DeregisterCollectionChanged<VM>(VM_CollectionChangedWithInvoke);
        }

        private void UnregisterVMHandlers(VM vm)
        {
            vm.PropertyChanged -= vm_PropertyChanged;
            vm.PropertyChanged -= vm_PropertyChanged_allVMs;
            VM_guest_metrics guest_metrics = vm.Connection.Resolve(vm.guest_metrics);
            if (guest_metrics != null)
                guest_metrics.PropertyChanged -= guest_metrics_PropertyChanged;
        }

        public override void PageHidden()
        {
            UnregisterHandlers();
        }

        private void vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "guest_metrics")
                RegisterVMGuestMetrics((VM)sender);

            if (e.PropertyName == "memory_static_min" || e.PropertyName == "memory_static_max" ||
                e.PropertyName == "memory_dynamic_min" || e.PropertyName == "memory_dynamic_max" ||
                e.PropertyName == "metrics" || e.PropertyName == "guest_metrics")
            {
                // We could just redraw the row for this VM (and for the host), provided it doesn't share a row
                // with any other VMs before or after. But doesn't seem necessary to figure all that out.
                _rebuild_needed = true;
            }
        }

        private void vm_PropertyChanged_allVMs(object sender, PropertyChangedEventArgs e)
        {
            // Only observe real VMs (but templates come through here too because
            // they change into real VMs during VM creation).
            if (!((VM)sender).is_a_real_vm())
                return;

            // These are used by MainWindow.VMHome() to determine which host the VM belongs to
            if (e.PropertyName == "power_state" || e.PropertyName == "VBDs" || e.PropertyName == "affinity" ||

            // These can change whether the VM is shown
                e.PropertyName == "name_label" || e.PropertyName == "other_config"|| e.PropertyName=="resident_on")
            {
                if (xenObject is Pool)
                {
                    XenObject = xenObject;  // have to recalculate list of VMs etc.
                    return;
                }
                if (xenObject is Host)
                {
                    bool vmWanted = VMWanted((VM)sender, (Host)xenObject);
                    if (vmWanted != vms.Contains((VM)sender))
                    {
                        XenObject = xenObject;  // have to recalculate list of VMs etc.
                        return;
                    }
                }
                // We also have to redisplay if the power_state or name_label changes (can change the sort order)
                if ((e.PropertyName == "power_state" || e.PropertyName == "name_label") && vms.Contains((VM)sender))
                    _rebuild_needed = true;
            }
        }

        private void guest_metrics_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "other")
                _rebuild_needed = true;
        }

        // Whether a change to the properties of any object means we need to Rebuild.
        // This is in order to batch up changes to several objects at once (see CA-35330).
        private bool _rebuild_needed = false;
        private void XenObjectsUpdated(object sender, EventArgs e)
        {
            if (_rebuild_needed)
                Rebuild();
        }

        private bool _rebuilding = false;

        private void Rebuild()
        {
            Program.AssertOnEventThread();
            _rebuild_needed = false;
            if (!this.Visible)
                return;
            _rebuilding = true;
            pageContainerPanel.SuspendLayout();

            // Store a list of the current controls. We remove them at the end because it makes less flicker that way.
            // While we're going through them, remember which VMs were on expanded rows.
            List<Control> oldControls = new List<Control>(pageContainerPanel.Controls.Count);
            List<VM> expandedVMs = new List<VM>(vms.Count);
            foreach (Control c in pageContainerPanel.Controls)
            {
                oldControls.Add(c);
                VMMemoryRow vmRow = c as VMMemoryRow;
                if (vmRow != null && vmRow.Expanded)
                    expandedVMs.AddRange(vmRow.VMs);
            }

            // Group VMs with the same settings
            Dictionary<MemSettings, List<VM>> settingsToVMs = new Dictionary<MemSettings,List<VM>>();  // all VMs with a particular setting
            List<MemSettings> listSettings = new List<MemSettings>();  // also make a list of MemSettings to preserve the order

            vms.Sort();
            foreach (VM vm in vms)
            {
                MemSettings settings =
                    vm.has_ballooning()
                        ? new MemSettings(true, vm.power_state, vm.memory_static_min, vm.memory_static_max,
                            vm.memory_dynamic_min, vm.memory_dynamic_max)
                        : new MemSettings(false, vm.power_state, 0, vm.memory_static_max, 0, 0);  // don't consider other mem settings if ballooning off
                if (!settingsToVMs.ContainsKey(settings))  // we've not seen these settings on another VM
                {
                    settingsToVMs.Add(settings, new List<VM>());
                    listSettings.Add(settings);
                }
                settingsToVMs[settings].Add(vm);
            }

            // Add server rows
            int initScroll = pageContainerPanel.VerticalScroll.Value;
            int top = pageContainerPanel.Padding.Top - initScroll;
            hosts.Sort();
            foreach (Host host in hosts)
            {
                Host_metrics metrics = host.Connection.Resolve(host.metrics);
                if (metrics == null || !metrics.live)
                    continue;
                AddRowToPanel(new HostMemoryRow(host), ref top);
            }

            // Add VM rows.
            // Sort the rows first by power state then by usual sort order of first VM (because of vms.Sort() above).
            // Easier to traverse listSettings five times, but more complicated sorts could be achieved by listSettings.Sort().
            vm_power_state[] power_state_order = {vm_power_state.Running, vm_power_state.Paused, vm_power_state.Suspended, vm_power_state.Halted, vm_power_state.unknown};
            foreach (vm_power_state ps in power_state_order)
            {
                foreach (MemSettings settings in listSettings)
                {
                    if (settings.power_state == ps)
                    {
                        List<VM> rowVMs = settingsToVMs[settings];
                        bool expand = Helpers.ListsIntersect(expandedVMs, rowVMs);  // expand header if any of its VMs were expanded before
                        AddRowToPanel(new VMMemoryRow(rowVMs, expand), ref top);
                    }
                }
            }

            // Remove old controls
            foreach (Control c in oldControls)
            {
                pageContainerPanel.Controls.Remove(c);
                int scroll = initScroll;
                if (scroll > pageContainerPanel.VerticalScroll.Maximum)
                    scroll = pageContainerPanel.VerticalScroll.Maximum;
                pageContainerPanel.VerticalScroll.Value = scroll;  // Without this, the scroll bar can jump around while the page is being rebuilt
                c.Dispose();
            }
            _rebuilding = false;       
            pageContainerPanel.ResumeLayout();
            ReLayout();

            SetupDeprecationBanner();
        }

        private void ReLayout()
        {
            Program.AssertOnEventThread();
            if (_rebuilding)
                return;

            int initScroll = pageContainerPanel.VerticalScroll.Value;
            int top = pageContainerPanel.Padding.Top - initScroll;
            foreach (Control row in pageContainerPanel.Controls)
            {
                row.Top = top;
                top += row.Height + ROW_GAP;
            }
        }

        void row_Resize(object sender, EventArgs e)
        {
            ReLayout();
        }

        private void AddRowToPanel(UserControl row, ref int top)
        {
            row.Top = top;
            row.Left = pageContainerPanel.Padding.Left - pageContainerPanel.HorizontalScroll.Value;
            SetRowWidth(row);
            row.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            top += row.Height + ROW_GAP;
            row.Resize += row_Resize;
            pageContainerPanel.Controls.Add(row);
        }

        private struct MemSettings
        {
            public readonly bool has_ballooning;
            public readonly vm_power_state power_state;
            public readonly long static_min, static_max, dynamic_min, dynamic_max;

            public MemSettings(bool has_ballooning, vm_power_state power_state,
                long static_min, long static_max, long dynamic_min, long dynamic_max)
            {
                this.has_ballooning = has_ballooning;
                this.power_state = power_state;
                this.static_min = static_min;
                this.static_max = static_max;
                this.dynamic_min = dynamic_min;
                this.dynamic_max = dynamic_max;
            }
        }

        private void pageContainerPanel_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control row in pageContainerPanel.Controls)
                SetRowWidth(row);
        }

        private void SetRowWidth(Control row)
        {
            row.Width = pageContainerPanel.Width - pageContainerPanel.Padding.Left - 25;  // It won't drop below row.MinimumSize.Width though
        }

        private void SetupDeprecationBanner()
        {
            Banner.Visible = false;
            if (!Helpers.QuebecOrGreater(xenObject.Connection))
                return;
                
            if (vms.Any(vm => vm.has_ballooning() && vm.memory_dynamic_min != vm.memory_static_max))
            {
                Banner.AppliesToVersion = Messages.XENSERVER_8_1;
                Banner.BannerType = DeprecationBanner.Type.Deprecation;
                Banner.FeatureName = Messages.DMC;
                Banner.LinkUri = HiddenFeatures.LinkLabelHidden ? null : new Uri(InvisibleMessages.DEPRECATION_URL);
                Banner.Visible = !HiddenFeatures.LinkLabelHidden;
            }
        }
    }
}

