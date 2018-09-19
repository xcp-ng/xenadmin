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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Controls.Ballooning
{
    public partial class VMShinyBar : ShinyBar
    {
        public event EventHandler SliderDragged;

        public VMShinyBar()
        {
            InitializeComponent();
        }

        // Variables passed in
        private long memoryUsed;
        private double static_min, static_max, dynamic_min, dynamic_max, dynamic_min_orig, dynamic_max_orig;
        public double dynamic_min_lowlimit, dynamic_min_highlimit, dynamic_max_lowlimit, dynamic_max_highlimit;
        private bool has_ballooning;
        private bool allowEdit;
        private bool multiple;

        // The increment in which the user can move the draggers, in bytes
        [Browsable(false)]
        public double Increment { get; set; }

        public double Dynamic_min
        {
            get { return dynamic_min; }
        }

        public double Dynamic_max
        {
            get { return dynamic_max; }
        }

        private bool Equal
        {
            get { return Dynamic_min == Dynamic_max;  }
        }

        public void SetRanges(double dynamic_min_lowlimit, double dynamic_min_highlimit, double dynamic_max_lowlimit, double dynamic_max_highlimit, string units)
        {
            if(units == "MB")
            {
                // Round to nearest MB inwards to agree with MemorySpinner and avoid bugs like CA-34996.
                double lowMB, highMB;
                MemorySpinner.CalcMBRanges(dynamic_min_lowlimit, dynamic_min_highlimit, out lowMB, out highMB);
                this.dynamic_min_lowlimit = lowMB * Util.BINARY_MEGA;
                this.dynamic_min_highlimit = highMB * Util.BINARY_MEGA;
                MemorySpinner.CalcMBRanges(dynamic_max_lowlimit, dynamic_max_highlimit, out lowMB, out highMB);
                this.dynamic_max_lowlimit = lowMB * Util.BINARY_MEGA;
                this.dynamic_max_highlimit = highMB * Util.BINARY_MEGA;
            }
            else
            {
                // Round to nearest GB inwards to agree with MemorySpinner and avoid bugs like CA-34996.
                double lowGB, highGB;
                MemorySpinner.CalcGBRanges(dynamic_min_lowlimit, dynamic_min_highlimit, out lowGB, out highGB);
                this.dynamic_min_lowlimit = lowGB * Util.BINARY_GIGA;
                this.dynamic_min_highlimit = highGB * Util.BINARY_GIGA;
                MemorySpinner.CalcGBRanges(dynamic_max_lowlimit, dynamic_max_highlimit, out lowGB, out highGB);                
                this.dynamic_max_lowlimit = lowGB * Util.BINARY_GIGA;
                this.dynamic_max_highlimit = highGB * Util.BINARY_GIGA;
            }          
        }

        private double SliderMinLimit
        {
            get
            {
                System.Diagnostics.Trace.Assert(activeSlider != Slider.NONE);
                return (activeSlider == Slider.MAX ? dynamic_max_lowlimit : dynamic_min_lowlimit);
            }
        }

        private double SliderMaxLimit
        {
            get
            {
                System.Diagnostics.Trace.Assert(activeSlider != Slider.NONE);
                return (activeSlider == Slider.MIN ? dynamic_min_highlimit : dynamic_max_highlimit);
            }
        }

        private enum Slider { NONE, MIN, MAX };

        // Internal state
        private Point mouseLocation = new Point(-1, -1);
        private Rectangle min_slider_rect, max_slider_rect;
        private Slider activeSlider = Slider.NONE;
        private bool mouseIsDown = false;
        private double BytesPerPixel;

        public void Populate(List<VM> vms, bool allowMemEdit)
        {
            var vm = vms[0];
            multiple = vms.Count > 1;
            memoryUsed = CalcMemoryUsed(vms);
            static_min = vm.memory_static_min;
            static_max = vm.memory_static_max;
            dynamic_min = dynamic_min_orig = Util.CorrectRoundingErrors(vm.memory_dynamic_min);
            dynamic_max = dynamic_max_orig = Util.CorrectRoundingErrors(vm.memory_dynamic_max);
            has_ballooning = vm.has_ballooning();
            allowEdit = allowMemEdit;
        }

        private static long CalcMemoryUsed(List<VM> vms)
        {
            var memories = (from VM vm in vms
                            where vm != null && vm.metrics != null &&
                                  (vm.power_state == vm_power_state.Running || vm.power_state == vm_power_state.Paused)
                            let metrics = vm.Connection.Resolve(vm.metrics)
                            where metrics != null
                            select metrics.memory_actual).ToList();

            if (memories.Count > 0)
                try
                {
                    return Convert.ToInt64(memories.Average());
                }
                catch
                {
                    // ignore; we'll return 0
                }

            return 0;
        }

        public void ChangeSettings(double static_min, double dynamic_min, double dynamic_max, double static_max)
        {
            this.static_min = static_min;

            // If we're editing, we never reduce the static_max (really, the "static_max" is just the top
            // of the bar: the real static_max is the position of the top of the range).
            if (!allowEdit || this.static_max < static_max)
                this.static_max = static_max;

            // If they're already equal, we don't reset the dynamic_min_orig.
            // (They've probably been set through the sliders not the spinners).
            if (dynamic_min != this.dynamic_min)
                this.dynamic_min = dynamic_min_orig = dynamic_min;
            if (dynamic_max != this.dynamic_max)
                this.dynamic_max = dynamic_max_orig = dynamic_max;
        }

        private void SetMemory(Slider slider, double bytes)
        {
            bool dragged = false;
            if (slider == Slider.MIN && dynamic_min != bytes)
            {
                dynamic_min = bytes;
                dragged = true;
            }
            if (slider == Slider.MAX && dynamic_max != bytes)
            {
                dynamic_max = bytes;
                dragged = true;
            }
            if (dragged)
                OnSliderDragged();
        }

        private void OnSliderDragged()
        {
            if (SliderDragged != null)
                SliderDragged(this, null);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (static_max == 0)  // not initialised
                return;

            Graphics g = e.Graphics;
            Rectangle barArea = barRect;
            BytesPerPixel = (double)static_max / (double)barArea.Width;

            // Grid

            DrawGrid(g, barArea, BytesPerPixel, static_max);

            // The bar

            int left_width = (int)((double)memoryUsed / BytesPerPixel);
            if (left_width > barArea.Width)  // Happens if the user is reducing static_max to below current memory usage.
                left_width = barArea.Width;  // I wanted to add a right-arrow to the bytesString in that case too, but the glyph isn't present in the font: and too much work to add an image.
            Rectangle rect = new Rectangle(barArea.Left, barArea.Top, left_width, barArea.Height);
            string bytesString = Util.MemorySizeStringSuitableUnits(memoryUsed, false);
            string toolTip = string.Format(multiple ? Messages.CURRENT_MEMORY_USAGE_MULTIPLE : Messages.CURRENT_MEMORY_USAGE, bytesString);
            DrawToTarget(g, barArea, rect, BallooningColors.VMShinyBar_Used, bytesString, BallooningColors.VMShinyBar_Text, HorizontalAlignment.Right, toolTip);

            rect = new Rectangle(barArea.Left + left_width, barArea.Top, barArea.Width - left_width, barArea.Height);
            DrawToTarget(g, barArea, rect, BallooningColors.VMShinyBar_Unused);

            // Sliders

            if (has_ballooning)
            {
                DrawSliderRanges(g);
                DrawSliders(g, dynamic_min, dynamic_max);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (allowEdit)
            {
                mouseLocation = e.Location;
                if (activeSlider != Slider.NONE)
                {
                    double min = SliderMinLimit;
                    double max = SliderMaxLimit;
                    double orig = (activeSlider == Slider.MIN ? dynamic_min_orig : dynamic_max_orig);
                    double posBytes = (mouseLocation.X - barRect.Left) * BytesPerPixel;
                    if (posBytes <= min)
                        posBytes = min;
                    else if (posBytes >= max)
                        posBytes = max;
                    else
                    {
                        double incrBytes = Increment;

                        // round to nearest incrBytes
                        // We need to do a rounding because the result should always have only 1 dp. If it does not have
                        // 1 dp it is not because of the formula, but because of the way the computer does the calculation.
                        double roundedBytes = Math.Round((int)((posBytes + incrBytes / 2) / incrBytes) * incrBytes, 1);

                        // We also allow the original value, even if it's not a multiple of
                        // incrBytes. That's so that we don't jump as soon as we click it
                        // (also so that we can get back to the original value if we want to).
                        double distRound = (posBytes - roundedBytes > 0 ? posBytes - roundedBytes : roundedBytes - posBytes);
                        double distOrig = (posBytes - orig > 0 ? posBytes - orig : orig - posBytes);
                        if (distRound >= distOrig)
                            roundedBytes = orig;

                        // posBytes can fall outside its range before or after the rounding,
                        // and both want to be truncated
                        if (roundedBytes <= min)
                            posBytes = min;
                        else if (roundedBytes >= max)
                            posBytes = max;
                        else
                            posBytes = roundedBytes;
                    }
                    SetMemory(activeSlider, posBytes);
                }
                Refresh();
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (allowEdit)
            {
                mouseIsDown = false;
                mouseLocation = new Point(-1, -1);
                activeSlider = Slider.NONE;
                Refresh();
            }

            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (allowEdit && e.Button == MouseButtons.Left)
            {
                mouseIsDown = true;
                if (min_slider_rect.Contains(mouseLocation))
                    activeSlider = Slider.MIN;
                else if (max_slider_rect.Contains(mouseLocation))
                    activeSlider = Slider.MAX;
                Refresh();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (allowEdit)
            {
                mouseIsDown = false;
                activeSlider = Slider.NONE;
                Refresh();
            }

            base.OnMouseUp(e);
        }

        private void DrawSliderRanges(Graphics g)
        {
            // Draw slider ranges if we're dragging right now
            if (activeSlider == Slider.NONE)
                return;

            Rectangle barArea = barRect;
            const int Height = 10;
            int min = barArea.Left + (int)(SliderMinLimit / BytesPerPixel);
            int max = barArea.Left + (int)(SliderMaxLimit / BytesPerPixel);

            using (Brush brush = new SolidBrush(BallooningColors.SliderLimits))
            {
                g.FillRectangle(brush, min, barRect.Bottom, max - min, Height);
            }
        }

        private void DrawSliders(Graphics g, double min, double max)
        {
            Rectangle barArea = barRect;
            Image min_image, max_image;
            if (allowEdit)
            {
                min_image = XenAdmin.Properties.Resources.memory_dynmin_slider;
                max_image = XenAdmin.Properties.Resources.memory_dynmax_slider;
            }
            else
            {
                min_image = XenAdmin.Properties.Resources.memory_dynmin_slider_noedit;
                max_image = XenAdmin.Properties.Resources.memory_dynmax_slider_noedit;                
            }

            // Calculate where to draw the sliders
            Point min_pt = new Point(barArea.Left + (int)(min / BytesPerPixel) - min_image.Width + (allowEdit ? 0 : 1), barArea.Bottom);
            Point max_pt = new Point(barArea.Left + (int)(max / BytesPerPixel) - (allowEdit ? 0 : 1), barArea.Bottom);
            min_slider_rect = new Rectangle(min_pt, min_image.Size);
            max_slider_rect = new Rectangle(max_pt, max_image.Size);

            // Recalculate the images to draw in case the mouse is over one of them
            if (allowEdit)
            {
                if (activeSlider == Slider.MIN)
                    min_image = XenAdmin.Properties.Resources.memory_dynmin_slider_dark;
                if (activeSlider == Slider.MAX)
                    max_image = XenAdmin.Properties.Resources.memory_dynmax_slider_dark;

                if (activeSlider == Slider.NONE && !mouseIsDown)
                {
                    if (min_slider_rect.Contains(mouseLocation))
                        min_image = XenAdmin.Properties.Resources.memory_dynmin_slider_light;
                    else if (max_slider_rect.Contains(mouseLocation))
                        max_image = XenAdmin.Properties.Resources.memory_dynmax_slider_light;
                }
            }

            // Draw the images
            g.DrawImageUnscaled(min_image, min_pt);
            g.DrawImageUnscaled(max_image, max_pt);
        }

        protected override int barHeight
        {
            get
            {
                return 20;
            }
        }
    }
}
