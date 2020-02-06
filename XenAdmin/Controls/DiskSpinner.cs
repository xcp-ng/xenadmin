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
using System.Windows.Forms;


namespace XenAdmin.Controls
{
    public partial class DiskSpinner : UserControl
    {
        private const long DEFAULT_MINIMUM = Util.BINARY_MEGA; //default minimum size for disks
        private const long DEFAULT_MAXIMUM = Util.BINARY_PETA; //default maximum size for disks

        private long _minDiskSize = DEFAULT_MINIMUM;
        private bool _updating;
        private bool _isSizeValid;

        public event Action SelectedSizeChanged;

        public DiskSpinner()
        {
            InitializeComponent();
            DiskSizeNumericUpDown.TextChanged += DiskSizeNumericUpDown_TextChanged;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanResize
        {
            get => DiskSizeNumericUpDown.Enabled && comboBoxUnits.Enabled;
            set => DiskSizeNumericUpDown.Enabled = comboBoxUnits.Enabled = value;
        }
        
        public long SelectedSize { get; private set; } = Util.BINARY_GIGA; //default size for disks

        public bool IsSizeValid
        {
            get => _isSizeValid;
            private set
            {
                _isSizeValid = value;
                SelectedSizeChanged?.Invoke();
            }
        }

        public void Populate(long selectedSize = Util.BINARY_GIGA, long minSize = DEFAULT_MINIMUM)
        {
            if (minSize < DEFAULT_MINIMUM)
                minSize = DEFAULT_MINIMUM;
            if (minSize > DEFAULT_MAXIMUM)
                minSize = DEFAULT_MAXIMUM;

            if (selectedSize < minSize)
                selectedSize = minSize;
            if (selectedSize > DEFAULT_MAXIMUM)
                selectedSize = DEFAULT_MAXIMUM;

            SelectedSize = selectedSize;
            _minDiskSize = minSize;

            comboBoxUnits.Items.Add(new DiskSizeWithUnits(3, 1, 0.001M, Util.BINARY_KILO, Util.BINARY_TERA, Messages.VAL_TERB, Util.ToTB));
            comboBoxUnits.Items.Add(new DiskSizeWithUnits(3, 1, 0.001M, Util.BINARY_MEGA, Util.BINARY_GIGA, Messages.VAL_GIGB, Util.ToGB));
            comboBoxUnits.Items.Add(new DiskSizeWithUnits(0, 256, 1, Util.BINARY_GIGA, Util.BINARY_MEGA, Messages.VAL_MEGB, Util.ToMB));

            foreach (DiskSizeWithUnits item in comboBoxUnits.Items)
            {
                if (item.TryRoundOptimal(selectedSize, out _))
                {
                    comboBoxUnits.SelectedItem = item;
                    break;
                }
            }
        }

        public void ValidateSize()
        {
            if (_updating || !(comboBoxUnits.SelectedItem is DiskSizeWithUnits dsk))
                return;

            if (string.IsNullOrEmpty(DiskSizeNumericUpDown.Text.Trim())) //do not issue error here
            {
                SetError(null);
                IsSizeValid = false;
                return;
            }

            // Don't use DiskSizeNumericUpDown.Value here, as it will fire the NumericUpDown built-in validation.
            // Use Text property instead. (CA-46028)

            if (!decimal.TryParse(DiskSizeNumericUpDown.Text.Trim(), out decimal result) || result < 0)
            {
                SetError(Messages.INVALID_NUMBER);
                IsSizeValid = false;
                return;
            }

            try
            {
                SelectedSize = (long)(result * dsk.Multiplier);
            }
            catch (OverflowException) //CA-71312
            {
                SelectedSize = long.MaxValue;
            }

            if (SelectedSize < _minDiskSize)
            {
                SetError(string.Format(Messages.DISK_TOO_SMALL, Util.DiskSizeString(_minDiskSize, dsk.DecimalPlaces)));
                IsSizeValid = false;
                return;
            }

            if (SelectedSize > DEFAULT_MAXIMUM)
            {
                SetError(Messages.INVALID_NUMBER);
                IsSizeValid = false;
                return;
            }

            SetError(null);
            IsSizeValid = true;
        }

        public void SetError(string error)
        {
            if (string.IsNullOrEmpty(error))
                tableLayoutPanelError.Visible = false;
            else
            {
                tableLayoutPanelError.Visible = true;
                labelError.Text = error;
            }
        }


        private void DiskSizeNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            ValidateSize();
        }

        private void DiskSizeNumericUpDown_TextChanged(object sender, EventArgs e)
        {
            ValidateSize();
        }

        private void DiskSizeNumericUpDown_KeyUp(object sender, KeyEventArgs e)
        {
            ValidateSize();
        }

        private void comboBoxUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _updating = true;
                if (!(comboBoxUnits.SelectedItem is DiskSizeWithUnits dsk))
                    return;

                DiskSizeNumericUpDown.Increment = dsk.Increment;
                DiskSizeNumericUpDown.DecimalPlaces = dsk.DecimalPlaces;
                DiskSizeNumericUpDown.Minimum = dsk.Minimum;
                DiskSizeNumericUpDown.Maximum = dsk.Maximum;
                DiskSizeNumericUpDown.Value = dsk.RoundSize(SelectedSize);
            }
            finally
            {
                _updating = false;
            }

            ValidateSize();
        }


        private struct DiskSizeWithUnits
        {
            public int DecimalPlaces { get; }
            public int Increment { get; }
            public decimal Minimum { get; }
            public decimal Maximum { get; }
            public long Multiplier { get; }
            public string Unit { get; }
            public Func<double, RoundingBehaviour, int, double> RoundingFunction { get; }

            public DiskSizeWithUnits(int decimalPlaces, int increment, decimal minimum, decimal maximum, long multiplier,
                string unit, Func<double, RoundingBehaviour, int, double> roundingFunction)
            {
                DecimalPlaces = decimalPlaces;
                Increment = increment;
                Minimum = minimum;
                Maximum = maximum;
                Multiplier = multiplier;
                Unit = unit;
                RoundingFunction = roundingFunction;
            }

            public override string ToString()
            {
                return Unit;
            }

            public decimal RoundSize(decimal size)
            {
                var rounded = (decimal)RoundingFunction((double)size, RoundingBehaviour.Up, DecimalPlaces);
                if (rounded < Minimum)
                    return Minimum;
                if (rounded > Maximum)
                    return Maximum;
                return rounded;
            }

            public bool TryRoundOptimal(decimal size, out decimal result)
            {
                if (size >= Multiplier)
                {
                    result = (decimal)RoundingFunction((double)size, RoundingBehaviour.Up, DecimalPlaces);
                    return true;
                }

                result = 0;
                return false;
            }
        }
    }
}
