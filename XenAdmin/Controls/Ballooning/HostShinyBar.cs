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


namespace XenAdmin.Controls.Ballooning
{
    public partial class HostShinyBar : ShinyBar
    {
        public HostShinyBar()
        {
            InitializeComponent();
        }

        private Host host;
        private Host_metrics host_metrics;
        List<VM> vms;
        Dictionary<VM, VM_metrics> vm_metrics;
        long xen_memory;
        long dom0_memory;
        long memory_of_biggest_host;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="xen_memory"></param>
        /// <param name="dom0_memory"></param>
        /// <param name="memory_of_biggest_host">If set > 0, calculate the width of the shiny bar along the host with most memory for relative display</param>
        public void Initialize(Host host, long xen_memory, long dom0_memory, long memory_of_biggest_host = 0)
        {
            this.host = host;
            this.host_metrics = host.Connection.Resolve(host.metrics);
            this.xen_memory = xen_memory;
            this.dom0_memory = dom0_memory;
            this.memory_of_biggest_host = memory_of_biggest_host;
            vms = host.Connection.ResolveAll(host.resident_VMs);
            vm_metrics = new Dictionary<VM, VM_metrics>();
            foreach (VM vm in vms)
                vm_metrics[vm] = vm.Connection.Resolve(vm.metrics);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (host == null || host_metrics == null || vms == null || host_metrics.memory_total == 0)
                return;

            Graphics g = e.Graphics;
            Rectangle barArea = barRect;

            long reverece_memory;
            if (memory_of_biggest_host > 0)
                reverece_memory = memory_of_biggest_host;
            else
                reverece_memory = host_metrics.memory_total;


            double bytesPerPixel = (double)reverece_memory / (double)barArea.Width;

            // Grid
            DrawGrid(g, barArea, bytesPerPixel, host_metrics.memory_total);

            // A bar for Xen memory
            double left = (double)barArea.Left;
            DrawSegment(g, xen_memory - dom0_memory, bytesPerPixel, Messages.MEMORY_XEN, null, BallooningColors.HostShinyBar_Xen, ref left);

            // A bar for Dom0 memory
            DrawSegment(g, dom0_memory, bytesPerPixel, string.Format(Messages.CONTROL_DOM_ON_HOST, host.Name()), null, BallooningColors.HostShinyBar_ControlDomain, ref left);

            // A bar for each VM
            int i = 0;
            vms.Sort();
            foreach (VM vm in vms)
            {
                if (vm.is_control_domain)
                    continue;

                VM_metrics metrics = vm_metrics[vm];
                if (metrics != null)
                {
                    DrawSegment(g, metrics.memory_actual, bytesPerPixel, vm.Name(), vm,
                        BallooningColors.HostShinyBar_VMs[i++ % BallooningColors.HostShinyBar_VMs.Length],
                        ref left);
                }
            }

            // One final bar for free space
            long factor_percent = (host_metrics.memory_total*100) / memory_of_biggest_host;

            Rectangle rectFree = new Rectangle((int)left, barArea.Top, (int)(((barArea.Right - (int)left) * factor_percent)/100), barArea.Height);
            DrawToTarget(g, barArea, rectFree, BallooningColors.HostShinyBar_Unused);
        }

        private void DrawSegment(Graphics g, long mem, double bytesPerPixel, string name, VM vm, Color color, ref double left)
        {
            // This should never happen, but actually does happen in older server dbs because
            // we didn't used to have the amount of free memory accurately (see CA-31223).
            if (mem < 0)
                return;

            Rectangle barArea = barRect;
            double width = mem / bytesPerPixel;
            Rectangle rect = new Rectangle((int)left, barArea.Top,
                (int)(left + width) - (int)left,  // this is not necessarily the same as (int)width, which can leave a 1 pixel gap
                barArea.Height);
            string bytesString = Util.MemorySizeStringSuitableUnits(mem, false);
            string caption = name + "\n" + bytesString;
            string toolTip = name + "\n" + string.Format(Messages.CURRENT_MEMORY_USAGE, Util.MemorySizeStringSuitableUnits(mem, true));
            if (vm != null && vm.has_ballooning())
            {
                if (vm.memory_dynamic_max == vm.memory_static_max)
                    toolTip += string.Format("\n{0}: {1}\n{2}: {3}",
                                             Messages.DYNAMIC_MIN, Util.MemorySizeStringSuitableUnits(vm.memory_dynamic_min, true),
                                             Messages.DYNAMIC_MAX, Util.MemorySizeStringSuitableUnits(vm.memory_dynamic_max, true));
                else
                    toolTip += string.Format("\n{0}: {1}\n{2}: {3}\n{4}: {5}",
                                             Messages.DYNAMIC_MIN, Util.MemorySizeStringSuitableUnits(vm.memory_dynamic_min, true),
                                             Messages.DYNAMIC_MAX, Util.MemorySizeStringSuitableUnits(vm.memory_dynamic_max, true),
                                             Messages.STATIC_MAX, Util.MemorySizeStringSuitableUnits(vm.memory_static_max, true));
            }
            DrawToTarget(g, barArea, rect, color, caption, BallooningColors.HostShinyBar_Text, HorizontalAlignment.Center, toolTip);
            left += width;
        }

        protected override int barHeight
        {
            get
            {
                return 40;
            }
        }
    }
}
