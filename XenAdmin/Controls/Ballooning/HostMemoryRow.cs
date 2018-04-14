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

using System.Windows.Forms;
using XenAPI;

namespace XenAdmin.Controls.Ballooning
{
    public partial class HostMemoryRow : UserControl
    {
        long memory_of_biggest_host;

        public HostMemoryRow()
        {
            InitializeComponent();
        }

        public HostMemoryRow(Host host)
            : this()
        {
            this.host = host;
        }

        public HostMemoryRow(Host host, long maxmemory)
            : this()
        {
            this.memory_of_biggest_host = maxmemory;
            this.host = host;
        }

        private Host host
        {
            set
            {
                // For a host, the labelPanel only ever has one row, so we don't need
                // to worry about all the sizing stuff like in the VM case.
                memoryRowLabel.Initialize(false, value);
                //hostMemoryControls.host = value;
                hostMemoryControls.SetHost(value, memory_of_biggest_host);
                Refresh();
            }
        }

        internal void UnregisterHandlers()
        {
            memoryRowLabel.UnsubscribeEvents();
            hostMemoryControls.UnregisterHandlers();
        }
    }
}
