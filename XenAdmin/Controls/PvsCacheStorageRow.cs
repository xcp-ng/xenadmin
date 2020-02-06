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
using System.Linq;
using System.Windows.Forms;
using XenAPI;

namespace XenAdmin.Controls
{
    public partial class PvsCacheStorageRow : UserControl
    {

        public Host Host { get; private set; }
        public PVS_site PvsSite { get; private set; }
        public PVS_cache_storage OrigPvsCacheStorage { get; private set; }

        public event EventHandler Changed;

        private const decimal MIN_CACHE_SIZE_GB = 1;
        private const decimal MAX_CACHE_SIZE_GB = 2 * Util.BINARY_KILO; // 2TB
        private const decimal DEFAULT_CACHE_SIZE_GB = 10;
        private decimal origCacheSizeGb;

        public PvsCacheStorageRow(Host host, PVS_site site)
        {
            InitializeComponent();
            Host = host;
            PvsSite = site;
            OrigPvsCacheStorage = site != null ? site.PvsCacheStorage(host) : null;
            Populate();
        }

        private void Populate()
        {
            labelHostName.Text = Host.Name();

            // initialize cacheSize
            SetupCacheSizeSpinner(OrigPvsCacheStorage == null ? DEFAULT_CACHE_SIZE_GB : (decimal)Util.ToGB(OrigPvsCacheStorage.size, RoundingBehaviour.Nearest, 1), 
                MIN_CACHE_SIZE_GB, 
                MAX_CACHE_SIZE_GB);
            origCacheSizeGb = numericUpDownCacheSize.Value;

            comboBoxCacheSr.Items.Clear();

            // add the "Not configured" item first
            var notConfiguredItem = new SrComboBoxItem(null, Messages.PVS_CACHE_NOT_CONFIGURED);
            comboBoxCacheSr.Items.Add(notConfiguredItem);

            // add Memory SR; if no memory SR  found, add a placeholder (we will create the memory SR in ConfigurePvsCacheAction)
            var memorySrs = Host.Connection.Cache.SRs.Where(s => s.GetSRType(false) == SR.SRTypes.tmpfs && s.CanBeSeenFrom(Host)).ToList();

            SR memorySr = null;

            if (memorySrs.Count == 0)
            {
                // create a placeholder for the memory SR
                memorySr = new SR
                {
                    type = SR.SRTypes.tmpfs.ToString(),
                    name_label = Messages.PVS_CACHE_MEMORY_SR_NAME,
                    shared = false,
                    opaque_ref = Helper.NullOpaqueRef
                };
            }
            else
            {
                if (OrigPvsCacheStorage != null)
                    memorySr = memorySrs.FirstOrDefault(sr => sr.opaque_ref == OrigPvsCacheStorage.SR.opaque_ref);

                if (memorySr == null)
                    memorySr = memorySrs.First();
            }

            var enabled = Host.dom0_memory_extra() >= MIN_CACHE_SIZE_GB * Util.BINARY_GIGA;
            var label = enabled ? Messages.PVS_CACHE_MEMORY_ONLY : Messages.PVS_CACHE_MEMORY_ONLY_DISABLED;
            var memorySrItem = new SrComboBoxItem(memorySr, label, enabled);
            comboBoxCacheSr.Items.Add(memorySrItem);

            if (OrigPvsCacheStorage != null && memorySr.opaque_ref == OrigPvsCacheStorage.SR.opaque_ref)
                comboBoxCacheSr.SelectedItem = memorySrItem;

            // add all suitable SRs
            var availableSRs = Host.Connection.Cache.SRs.Where(s => s.CanBeSeenFrom(Host) && SrIsSuitableForPvsCache(s)).ToList();
            availableSRs.Sort();
            foreach (var sr in availableSRs)
            {
                var newItem = new SrComboBoxItem(sr, sr.Name());
                comboBoxCacheSr.Items.Add(newItem);
                if (OrigPvsCacheStorage != null && sr.opaque_ref == OrigPvsCacheStorage.SR.opaque_ref)
                    comboBoxCacheSr.SelectedItem = newItem;
            }

            if (comboBoxCacheSr.SelectedItem == null)
                comboBoxCacheSr.SelectedItem = notConfiguredItem;

            ReadOnly = OrigPvsCacheStorage != null && OrigPvsCacheStorage.IsInUse();
            comboBoxCacheSr.Enabled = numericUpDownCacheSize.Enabled = !ReadOnly;
        }

        private bool SrIsSuitableForPvsCache(SR sr)
        {
            return sr.Show(Properties.Settings.Default.ShowHiddenVMs) && sr.SupportsVdiCreate() && sr.FreeSpace() >= MIN_CACHE_SIZE_GB * Util.BINARY_GIGA;
        }

        private void SetupCacheSizeSpinner(decimal value, decimal min, decimal max)
        {
            if (min > max)
                max = min;
            numericUpDownCacheSize.Minimum = min;
            numericUpDownCacheSize.Maximum = max;

            if (value < numericUpDownCacheSize.Minimum)
                value = numericUpDownCacheSize.Minimum;
            if (value > numericUpDownCacheSize.Maximum)
                value = numericUpDownCacheSize.Maximum;
            numericUpDownCacheSize.Value = value;
        }

        private bool _showHeader;
        public bool ShowHeader
        {
            get { return _showHeader; }
            set
            {
                _showHeader = value;
                labelCacheStorage.Visible = labelCacheSize.Visible = _showHeader;
                tableLayoutPanel1.Refresh();
            }
        }

        public SR CacheSr
        {
            get
            {
                var selectedItem = (SrComboBoxItem)comboBoxCacheSr.SelectedItem;
                return selectedItem != null ? selectedItem.Item : null;
            }
        }

        public long CacheSize
        {
            get { return (long)numericUpDownCacheSize.Value * Util.BINARY_GIGA;  }
        }


        public bool ValidToSave
        {
            get { return comboBoxCacheSr.SelectedItem != null && numericUpDownCacheSize.Validate(); }
        }

        public bool HasChanged
        {
            get
            {
                if (OrigPvsCacheStorage == null)
                    return CacheSr != null;
                if (CacheSr == null)
                    return OrigPvsCacheStorage != null;
                return OrigPvsCacheStorage.SR.opaque_ref != CacheSr.opaque_ref || origCacheSizeGb != numericUpDownCacheSize.Value;
            }
        }

        public bool ReadOnly { get; private set; }

        private void SomethingChanged(object sender, EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        private void comboBoxCacheSr_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedSr = CacheSr;
            if (selectedSr != null)
            {
                var maxSize = (decimal)Util.ToGB(selectedSr.GetSRType(false) == SR.SRTypes.tmpfs ? Host.dom0_memory_extra() : selectedSr.FreeSpace(), RoundingBehaviour.Down, 1); 
                maxSize = Math.Min(maxSize, MAX_CACHE_SIZE_GB);

                if (maxSize != numericUpDownCacheSize.Maximum)
                    SetupCacheSizeSpinner(numericUpDownCacheSize.Value, numericUpDownCacheSize.Minimum, maxSize);
            }
            SomethingChanged(this, e);
        }
    }

    public class SrComboBoxItem : IEnableableComboBoxItem
    {
        public SR Item;
        private readonly string _toString;
        private readonly bool _enabled;

        public SrComboBoxItem(SR sr, string label, bool enabled=true)
        {
            Item = sr;
            _toString = label;
            _enabled = enabled;
        }

        public override string ToString()
        {
            return _toString;
        }

        public bool Enabled
        {
            get { return _enabled; }
        }
    }
}
