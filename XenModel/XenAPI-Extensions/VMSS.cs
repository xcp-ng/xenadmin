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
using System.Linq;


namespace XenAPI
{
    public partial class VMSS 
    {
        public DateTime? GetNextRunTime(DateTime serverLocalTime)
        {
            if (!TryGetScheduleMin(schedule, out int min))
                return null;

            if (frequency == vmss_frequency.hourly)
                return GetHourlyDate(serverLocalTime, min);

            if (!TryGetScheduleHour(schedule, out int hour))
                return null;

            if (frequency == vmss_frequency.daily)
                return GetDailyDate(serverLocalTime, min, hour);

            if (frequency == vmss_frequency.weekly)
            {
                var dates = GetWeeklyDates(serverLocalTime, min, hour, BackUpScheduleDays(schedule));
                if (dates.Count > 0)
                    return dates[0];
            }

            return null;
        }

        public static DateTime GetHourlyDate(DateTime time, int min)
        {
            var nextDateTime = new DateTime(time.Year, time.Month, time.Day, time.Hour, min, 0);
            if (time > nextDateTime)
                nextDateTime = nextDateTime.AddHours(1);
            return nextDateTime;
        }

        public static DateTime GetDailyDate(DateTime time, int min, int hour)
        {
            var nextDateTime = new DateTime(time.Year, time.Month, time.Day, hour, min, 0);
            if (time > nextDateTime)
                nextDateTime = nextDateTime.AddDays(1);
            return nextDateTime;
        }

        public static List<DateTime> GetWeeklyDates(DateTime time, int min, int hour, DayOfWeek[] listDaysOfWeek)
        {
            var nextRun = new DateTime(time.Year, time.Month, time.Day, hour, min, 0);

            var runs = new List<DateTime>();
            foreach (var d in listDaysOfWeek)
            {
                if (nextRun.DayOfWeek < d)
                    runs.Add(nextRun.AddDays(d - nextRun.DayOfWeek));
                else if (d < nextRun.DayOfWeek)
                    runs.Add(nextRun.AddDays(7 - (nextRun.DayOfWeek - d)));
                else if (time < nextRun)
                    runs.Add(nextRun);
                else
                    runs.Add(nextRun.AddDays(7));
            }

            runs.Sort();
            return runs;
        }

        public override string Name()
        {
            return name_label;
        }

        public override string Description()
        {
            return name_description;
        }

        private static bool TryGetScheduleMin(Dictionary<string, string> schedule, out int result)
        {
            result = 0;
            return schedule.TryGetValue("min", out var outStr) && int.TryParse(outStr, out result);
        }

        private static bool TryGetScheduleHour(Dictionary<string, string> schedule, out int result)
        {
            result = 0;
            return schedule.TryGetValue("hour", out var outStr) && int.TryParse(outStr, out result);
        }

        public static DayOfWeek[] BackUpScheduleDays(Dictionary<string, string> schedule)
        {
            if (!schedule.ContainsKey("days"))
                return new DayOfWeek[] { };

            var days = schedule["days"].Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);

            var list = new List<DayOfWeek>();
            foreach (var d in days)
            {
                if (Enum.TryParse(d, true, out DayOfWeek result))
                    list.Add(result);
            }
            return list.ToArray();
        }
    }
}
