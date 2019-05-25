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
using XenAdmin.Core;


namespace XenAdmin.Controls.CustomDataGraph
{
    public class DataTimeRange
    {
        private static readonly TimeSpan TEN_MINUTES = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan ONE_HOUR = TimeSpan.FromHours(1);
        private static readonly TimeSpan TWO_DAYS = TimeSpan.FromDays(2);
        private static readonly TimeSpan THIRTY_DAYS = TimeSpan.FromDays(30);
        private static readonly TimeSpan ONE_YEAR = TimeSpan.FromDays(366);
        private const long FIVE_SECONDS_TICKS = 5 * TimeSpan.TicksPerSecond;

        public long Max;
        public long Min;
        public long Resolution;

        public static DataTimeRange UnitRange
        {
            get { return new DataTimeRange(1, 0, 1); }
        }

        public long Delta
        {
            get { return Max - Min; }
        }

        public static DataTimeRange MaxRange
        {
            get
            {
                return new DataTimeRange(DateTime.Now - ONE_YEAR, DateTime.Now, -THIRTY_DAYS);
            }
        }

        public DataTimeRange(DateTime max, DateTime min, TimeSpan resolution)
        {
            Max = max.Ticks;
            Min = min.Ticks;
            Resolution = resolution.Ticks;
        }

        public DataTimeRange(long max, long min, long resolution)
        {
            Max = max;
            Min = min;
            Resolution = resolution;
        }

        public string GetString(long value)
        {
            DateTime dt = new DateTime(value - FIVE_SECONDS_TICKS);
            string format = Resolution > -ONE_HOUR.Ticks
                                ? "T"
                                : "D";
            return HelpersGUI.DateTimeToString(dt, format, true);
        }

        public string GetRelativeString(long value, DateTime serverTime)
        {
            DateTime dt = new DateTime(value);
            TimeSpan diff = serverTime - new DateTime(Max);
            string format = diff < TWO_DAYS
                                ? "T"
                                : "D";
            return HelpersGUI.DateTimeToString(dt, format, true);
        }

        /// <summary>
        /// convert a delta on screen to a virtal delta for a given screen width
        /// and that corresponding virtual width
        /// </summary>
        public static long DeTranslateDelta(long screendelta, long virtalwidth, long screenwidth)
        {
            return (screendelta * virtalwidth) / screenwidth;
        }
    }
}
