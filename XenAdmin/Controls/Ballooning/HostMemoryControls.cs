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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Dialogs;
using XenAdmin.Core;


namespace XenAdmin.Controls.Ballooning
{
    public partial class HostMemoryControls : UserControl
    {
        public HostMemoryControls()
        {
            InitializeComponent();
        }

        private Host _host;
        private Host_metrics host_metrics;
        private long memory_of_biggest_host;

        [Browsable(false)]
        public void SetHost(Host host, long memory_of_biggest_host)
        {
            this.memory_of_biggest_host = memory_of_biggest_host;
            this.host = host;
        }

        [Browsable(false)]
        public Host host
        {
            private get { return _host; }
            set
            {
                _host = value;
                host.PropertyChanged += host_PropertyChanged;
                host_metrics = _host.Connection.Resolve(host.metrics);

                foreach (VM vm in _host.Connection.ResolveAll(_host.resident_VMs))
                {
                    vm.PropertyChanged += vm_PropertyChanged;
                    VM_metrics metrics = vm.Connection.Resolve(vm.metrics);
                    if (metrics != null)
                        metrics.PropertyChanged += vm_metrics_PropertyChanged;
                }
                if (Helpers.ElyOrGreater(_host))
                {
                    valueControlDomain.LinkBehavior = LinkBehavior.AlwaysUnderline;
                    valueControlDomain.Links[0].Enabled = true;
                }
                else
                {
                    valueControlDomain.LinkBehavior = LinkBehavior.NeverUnderline;
                    valueControlDomain.Links[0].Enabled = false;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Calculate values to display
            if (host == null || host_metrics == null)
                return;
            long total = host_metrics.memory_total;
            long free = host.memory_free_calc();
            long used = total - free;
            long xen_memory = host.xen_memory_calc();
            long avail = host.memory_available_calc();
            long tot_dyn_max = host.tot_dyn_max() + xen_memory;
            long dom0 = host.dom0_memory();

            long overcommit = total > 0
                ? (long)Math.Round((double)tot_dyn_max / (double)total * 100.0)
                : 0;

            // Initialize the shiny bar
            hostShinyBar.Initialize(host, xen_memory, dom0, this.memory_of_biggest_host);

            // Set the text values
            valueTotal.Text = Util.MemorySizeStringSuitableUnits(total, true);
            valueUsed.Text = Util.MemorySizeStringSuitableUnits(used, true);
            valueAvail.Text = Util.MemorySizeStringSuitableUnits(avail, true);
            valueTotDynMax.Text = Util.MemorySizeStringSuitableUnits(tot_dyn_max, true);
            labelOvercommit.Text = string.Format(Messages.OVERCOMMIT, overcommit);
            valueControlDomain.Text = Util.MemorySizeStringSuitableUnits(dom0, true);
        }

        void vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "memory_overhead")
                this.Refresh();
        }

        void host_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "memory_overhead")
                this.Refresh();
        }

        void vm_metrics_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "memory_actual")
                this.Refresh();
        }

        private void valueControlDomain_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var dlg = new ControlDomainMemoryDialog(host))
                dlg.ShowDialog(Program.MainWindow);
        }

        internal void UnregisterHandlers()
        {
            if (_host == null)
                return;
            host.PropertyChanged -= host_PropertyChanged;

            foreach (var vm in _host.Connection.Cache.VMs)
            {
                vm.PropertyChanged -= vm_PropertyChanged;
                var metrics = vm.Connection.Resolve(vm.metrics);
                if (metrics != null)
                    metrics.PropertyChanged -= vm_metrics_PropertyChanged;
            }
        }
    }
}
