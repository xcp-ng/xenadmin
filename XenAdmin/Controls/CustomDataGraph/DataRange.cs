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
using System.Text;
using System.Diagnostics;
using XenAPI;


namespace XenAdmin.Controls.CustomDataGraph
{
    public class DataRange
    {
        public static DataRange UnitRange
        {
            get { return new DataRange(1, 0, 1); }
        }

        public double Max;
        public double Min;
        public double Resolution;
        public Unit Units = Unit.None;
        public RangeScaleMode ScaleMode = RangeScaleMode.Fixed;

        public Func<IXenObject, double> UpdateMax;
        public Func<IXenObject, double> UpdateMin;
        public Func<IXenObject, double> UpdateResolution;

        public DataRange(double max, double min, double resolution)
        {
            Max = max;
            Min = min;
            Resolution = resolution;
        }

        public DataRange(double max, double min, double resolution, Unit units)
            : this(max, min, resolution)
        {
            Units = units;
        }

        public DataRange(double max, double min, double resolution, Unit units, RangeScaleMode scaleMode)
            : this(max, min, resolution, units)
        {
            ScaleMode = scaleMode;
        }

        public double Delta
        {
            get { return Max - Min; }
        }

        private double ConstrainedValue(double value)
        {
            return value < Min ? Min : value > Max ? Max : value;
        }

        /// <summary>
        /// This is the value together with the unit, e.g. "10 kB"
        /// </summary>
        public virtual string GetString(double value)
        {
            double constrVal = ConstrainedValue(value);

            switch (Units)
            {
                case Unit.Bytes:
                    return Util.MemorySizeStringVariousUnits(constrVal);
                case Unit.BytesPerSecond:
                    return Util.DataRateString(constrVal);
                case Unit.Percentage:
                    return string.Format("{0}%", constrVal);
                case Unit.NanoSeconds:
                    return Util.NanoSecondsString(constrVal);
                case Unit.CountsPerSecond:
                    return Util.CountsPerSecondString(constrVal);
                case Unit.MilliWatt:
                    return Util.MilliWattString(constrVal);
                case Unit.Centigrade:
                    return string.Format("{0}\u2103", constrVal.ToString("0"));
                case Unit.MegaHertz:
                    return Util.MegaHertzString(constrVal);
                default:
                    return constrVal.ToString();
            }
        }

        /// <summary>
        /// This is the just the unit, e.g. "kB"
        /// </summary>
        public string UnitString
        {
            get
            {
                string unit;

                switch (Units)
                {
                    case Unit.Bytes:
                        Util.MemorySizeValueVariousUnits(Max, out unit);
                        return unit;
                    case Unit.BytesPerSecond:
                        Util.DataRateValue(Max, out unit);
                        return unit;
                    case Unit.Percentage:
                        return "%";
                    case Unit.NanoSeconds:
                        Util.NanoSecondsValue(Max, out unit);
                        return unit;
                    case Unit.CountsPerSecond:
                        return Messages.COUNTS_PER_SEC_UNIT;
                    case Unit.Centigrade:
                        return "\u2103";
                    case Unit.MilliWatt:
                        Util.MilliWattValue(Max, out unit);
                        return unit;
                    case Unit.MegaHertz:
                        Util.MegaHertzValue(Max, out unit);
                        return unit;
                    default:
                        return "";
                }
            }
        }

        /// <summary>
        /// This is the value string, e.g. "10"
        /// </summary>
        public virtual string GetRelativeString(double value)
        {
            double constrVal = ConstrainedValue(value);
            string unit;

            switch (Units)
            {
                case Unit.Bytes:
                    return Util.MemorySizeValueVariousUnits(constrVal, out unit);
                case Unit.BytesPerSecond:
                    return Util.DataRateValue(constrVal, out unit);
                case Unit.NanoSeconds:
                    return Util.NanoSecondsValue(constrVal, out unit);
                case Unit.MilliWatt:
                    return Util.MilliWattValue(constrVal, out unit);
                case Unit.MegaHertz:
                    return Util.MegaHertzValue(constrVal, out unit);
                case Unit.CountsPerSecond://fall through
                default:
                    return constrVal.ToString();
            }
        }

        public void Shift(double delta)
        {
            Max += delta;
            Min += delta;
        }

        internal void UpdateAll(IXenObject xo)
        {
            if (UpdateMin != null)
                Min = UpdateMin(xo);
            if (UpdateMax != null)
                Max = UpdateMax(xo);
            if (UpdateResolution != null)
                Resolution = UpdateResolution(xo);
        }

        public void RoundToNearestPowerOf10()
        {
            double max;

            switch (Units)
            {
                case Unit.None:
                case Unit.Percentage:
                case Unit.NanoSeconds:
                case Unit.CountsPerSecond:
                case Unit.MilliWatt:
                case Unit.Centigrade:
                case Unit.MegaHertz:
                    int pow = 0;
                    max = Max;
                    if (Max > 1)
                    {
                        while (max > 1)
                        {
                            max /= 10;
                            pow++;
                        }
                    }
                    else if (0 < Max && Max < 1)
                    {
                        while (max < 1)
                        {
                            max *= 10;
                            pow--;
                        }
                    }
                    Max = Math.Pow(10, pow);
                    Resolution = Max > 10 ? Max / 10d : 1;
                    break;
                case Unit.Bytes:
                case Unit.BytesPerSecond:
                default:
                    {
                        int pows = 1;
                        int tens = 0;
                        max = Max / 1024;
                        while (max > 1024)
                        {
                            max /= 1024;
                            pows++;
                        }
                        while (max > 1)
                        {
                            max /= 10;
                            tens++;
                        }
                        if (tens == 3)
                        {
                            tens = 0;
                            pows++;
                        }
                        Max = Math.Pow(10, tens) * Math.Pow(1024, pows);
                        Resolution = Max > 8 ? Max / 8d : 1;
                    }
                    break;
            }
        }
    }

    public enum RangeScaleMode { Fixed, Auto, Delegate }

    public enum Unit { None, Percentage, BytesPerSecond, Bytes, NanoSeconds, CountsPerSecond, MilliWatt, Centigrade, MegaHertz }
}
