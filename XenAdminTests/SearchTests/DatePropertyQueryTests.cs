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
using NUnit.Framework;
using XenAdmin.XenSearch;

namespace XenAdminTests.UnitTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class DatePropertyQueryTests
    {
        [Test, Sequential]
        public void Today(
            [Values(0, 2009)] int year,
            [Values(0,    1)] int month,
            [Values(0,    1)] int day,
            [Values(0,   12)] int hour,
            [Values(0,    0)] int minute,
            [Values(0,    0)] int second)
        {
            DateTime dt = new DateTime(2008, 6, 2, 0, 0, 0);  // arbitrary date, shouldn't matter
            DatePropertyQuery query = new DatePropertyQuery(PropertyNames.start_time, dt, DatePropertyQuery.PropertyQueryType.today);
            if (year != 0)
                query.PretendNow = new DateTime(year, month, day, hour, minute, second);
            CheckMatch(false, query, 2008, 6, 2, 0, 0, 0);

            DateTime today = query.Now.Date;  // 00:00 today
            DateTime yesterday = today - new TimeSpan(12, 0, 0);  // some time yesterday (midday, except across DST changes)
            DateTime tomorrow = today + new TimeSpan(36, 0, 0);   // some time tomorrow (midday, except across DST changes)
            CheckMatch(false, query, yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);
            CheckMatch(true, query, today.Year, today.Month, today.Day, 0, 0, 0);
            CheckMatch(true, query, today.Year, today.Month, today.Day, 23, 59, 59);
            CheckMatch(false, query, tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 0);

            DateTime lastYear1 = yesterday - new TimeSpan(364, 0, 0, 0);  // midday 365 days ago
            DateTime lastYear2 = yesterday - new TimeSpan(365, 0, 0, 0);  // midday 366 days ago
            CheckMatch(false, query, lastYear1.Year, lastYear1.Month, lastYear1.Day, 12, 0, 0);
            CheckMatch(false, query, lastYear2.Year, lastYear2.Month, lastYear2.Day, 12, 0, 0);
        }

        [Test, Sequential]
        public void Yesterday(
            [Values(0, 2009, 2008, 2008, 2008, 2008, 2007)] int year,
            [Values(0,    1,   12,    3,    2,    1,   12)] int month,
            [Values(0,    1,   31,    1,   29,    1,   31)] int day,
            [Values(0,   12,   12,    0,   23,    0,   23)] int hour,
            [Values(0,    0,    0,    0,   59,    0,   59)] int minute,
            [Values(0,    0,    0,    0,   59,    0,   59)] int second)
        {
            DateTime dt = new DateTime(2008, 6, 2, 0, 0, 0);  // arbitrary date, shouldn't matter
            DatePropertyQuery query = new DatePropertyQuery(PropertyNames.start_time, dt, DatePropertyQuery.PropertyQueryType.yesterday);
            if (year != 0)
                query.PretendNow = new DateTime(year, month, day, hour, minute, second);
            CheckMatch(false, query, 2008, 6, 1, 0, 0, 0);

            DateTime today = query.Now.Date;  // 00:00 today
            DateTime yesterday = today - new TimeSpan(12, 0, 0);  // some time yesterday (midday, except across DST changes)
            DateTime daybefore = yesterday -  new TimeSpan(24, 0, 0);   // some time the day before yesterday (midday, except across DST changes)
            CheckMatch(false, query, daybefore.Year, daybefore.Month, daybefore.Day, 23, 59, 59);
            CheckMatch(true, query, yesterday.Year, yesterday.Month, yesterday.Day, 0, 0, 0);
            CheckMatch(true, query, yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);
            CheckMatch(false, query, today.Year, today.Month, today.Day, 0, 0, 0);
        }

        [Test, Sequential]
        public void ThisWeek(
            [Values(0, 2009, 2008, 2008, 2008, 2008, 2007)] int year,
            [Values(0,    1,   12,    3,    2,    1,   12)] int month,
            [Values(0,    1,   31,    1,   29,    1,   31)] int day,
            [Values(0,   12,   12,    0,   23,    0,   23)] int hour,
            [Values(0,    0,    0,    0,   59,    0,   59)] int minute,
            [Values(0,    0,    0,    0,   59,    0,   59)] int second)
        {
            DateTime dt = new DateTime(2008, 6, 2, 0, 0, 0);  // arbitrary date, shouldn't matter
            DatePropertyQuery query = new DatePropertyQuery(PropertyNames.start_time, dt, DatePropertyQuery.PropertyQueryType.thisweek);
            if (year != 0)
                query.PretendNow = new DateTime(year, month, day, hour, minute, second);
            CheckMatch(false, query, 2008, 6, 1, 0, 0, 0);

            DateTime today = query.Now.Date;  // 00:00 today
            DateTime sixdaysago = today - new TimeSpan(5, 12, 0, 0);  // some time six days ago (midday, except across DST changes)
            DateTime sevendaysago = today - new TimeSpan(6, 12, 0, 0);   // some time seven days ago (midday, except across DST changes)
            DateTime tomorrow = today + new TimeSpan(36, 0, 0);   // some time tomorrow (midday, except across DST changes)
            CheckMatch(false, query, sevendaysago.Year, sevendaysago.Month, sevendaysago.Day, 23, 59, 59);
            CheckMatch(true, query, sixdaysago.Year, sixdaysago.Month, sixdaysago.Day, 0, 0, 0);
            CheckMatch(true, query, today.Year, today.Month, today.Day, 23, 59, 59);
            CheckMatch(false, query, tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 0);
        }

        [Test, Sequential]
        public void LastWeek(
            [Values(0, 2009, 2008, 2008, 2008, 2008, 2007)] int year,
            [Values(0,    1,   12,    3,    2,    1,   12)] int month,
            [Values(0,    1,   31,    1,   29,    1,   31)] int day,
            [Values(0,   12,    0,    0,   23,    0,   23)] int hour,
            [Values(0,    0,    0,    0,   59,    0,   59)] int minute,
            [Values(0,    0,    0,    0,   59,    0,   59)] int second)
        {
            DateTime dt = new DateTime(2008, 6, 2, 0, 0, 0);  // arbitrary date, shouldn't matter
            DatePropertyQuery query = new DatePropertyQuery(PropertyNames.start_time, dt, DatePropertyQuery.PropertyQueryType.lastweek);
            if (year != 0)
                query.PretendNow = new DateTime(year, month, day, hour, minute, second);
            CheckMatch(false, query, 2008, 5, 28, 0, 0, 0);

            DateTime today = query.Now.Date;  // 00:00 today
            DateTime sixdaysago = today - new TimeSpan(5, 12, 0, 0);  // some time six days ago (midday, except across DST changes)
            DateTime sevendaysago = today - new TimeSpan(6, 12, 0, 0);   // some time seven days ago (midday, except across DST changes)
            DateTime thirteendaysago = today - new TimeSpan(12, 12, 0, 0);  // some time six days ago (midday, except across DST changes)
            DateTime fourteendaysago = today - new TimeSpan(13, 12, 0, 0);   // some time seven days ago (midday, except across DST changes)
            CheckMatch(false, query, fourteendaysago.Year, fourteendaysago.Month, fourteendaysago.Day, 23, 59, 59);
            CheckMatch(true, query, thirteendaysago.Year, thirteendaysago.Month, thirteendaysago.Day, 0, 0, 0);
            CheckMatch(true, query, sevendaysago.Year, sevendaysago.Month, sevendaysago.Day, 23, 59, 59);
            CheckMatch(false, query, sixdaysago.Year, sixdaysago.Month, sixdaysago.Day, 0, 0, 0);
        }

        [Test, Sequential]
        public void Before(
            [Values(2009, 2009, 2008, 2008, 2008, 2008, 2007)] int year,
            [Values(   6,    1,   12,    3,    2,    1,   12)] int month,
            [Values(  15,    1,   31,    1,   29,    1,   31)] int day,
            [Values(  12,   12,    0,    0,   23,    0,   23)] int hour,
            [Values(   0,    0,    0,    0,   59,    0,   59)] int minute,
            [Values(   0,    0,    0,    0,   59,    0,   59)] int second)

        {
            DateTime dt = new DateTime(year, month, day, hour, minute, second);
            DatePropertyQuery query = new DatePropertyQuery(PropertyNames.start_time, dt, DatePropertyQuery.PropertyQueryType.before);
            CheckMatch(true, query, year, month, day, 0, 0, 0);
            CheckMatch(true, query, year, month, day, 23, 59, 59);
            DateTime daybefore = dt - new TimeSpan(1, 0, 0, 0);
            CheckMatch(true, query, daybefore.Year, daybefore.Month, daybefore.Day, 23, 59, 59);
            DateTime dayafter = dt + new TimeSpan(1, 0, 0, 0);
            CheckMatch(false, query, dayafter.Year, dayafter.Month, dayafter.Day, 0, 0, 0);
            DateTime yearbefore1 = dt - new TimeSpan(365, 0, 0, 0);
            DateTime yearbefore2 = dt - new TimeSpan(366, 0, 0, 0);
            CheckMatch(true, query, yearbefore1.Year, yearbefore1.Month, yearbefore1.Day, 12, 0, 0);
            CheckMatch(true, query, yearbefore2.Year, yearbefore2.Month, yearbefore2.Day, 12, 0, 0);
            DateTime yearafter1 = dt + new TimeSpan(365, 0, 0, 0);
            DateTime yearafter2 = dt + new TimeSpan(366, 0, 0, 0);
            CheckMatch(false, query, yearafter1.Year, yearafter1.Month, yearafter1.Day, 12, 0, 0);
            CheckMatch(false, query, yearafter2.Year, yearafter2.Month, yearafter2.Day, 12, 0, 0);
        }

        [Test, Sequential]
        public void After(
            [Values(2009, 2009, 2008, 2008, 2008, 2008, 2007)] int year,
            [Values(   6,    1,   12,    3,    2,    1,   12)] int month,
            [Values(  15,    1,   31,    1,   29,    1,   31)] int day,
            [Values(  12,   12,    0,    0,   23,    0,   23)] int hour,
            [Values(   0,    0,    0,    0,   59,    0,   59)] int minute,
            [Values(   0,    0,    0,    0,   59,    0,   59)] int second)
        {
            DateTime dt = new DateTime(year, month, day, hour, minute, second);
            DatePropertyQuery query = new DatePropertyQuery(PropertyNames.start_time, dt, DatePropertyQuery.PropertyQueryType.after);
            CheckMatch(true, query, year, month, day, 0, 0, 0);
            CheckMatch(true, query, year, month, day, 23, 59, 59);
            DateTime daybefore = dt - new TimeSpan(1, 0, 0, 0);
            CheckMatch(false, query, daybefore.Year, daybefore.Month, daybefore.Day, 23, 59, 59);
            DateTime dayafter = dt + new TimeSpan(1, 0, 0, 0);
            CheckMatch(true, query, dayafter.Year, dayafter.Month, dayafter.Day, 0, 0, 0);
            DateTime yearbefore1 = dt - new TimeSpan(365, 0, 0, 0);
            DateTime yearbefore2 = dt - new TimeSpan(366, 0, 0, 0);
            CheckMatch(false, query, yearbefore1.Year, yearbefore1.Month, yearbefore1.Day, 12, 0, 0);
            CheckMatch(false, query, yearbefore2.Year, yearbefore2.Month, yearbefore2.Day, 12, 0, 0);
            DateTime yearafter1 = dt + new TimeSpan(365, 0, 0, 0);
            DateTime yearafter2 = dt + new TimeSpan(366, 0, 0, 0);
            CheckMatch(true, query, yearafter1.Year, yearafter1.Month, yearafter1.Day, 12, 0, 0);
            CheckMatch(true, query, yearafter2.Year, yearafter2.Month, yearafter2.Day, 12, 0, 0);
        }

        [Test, Sequential]
        public void Exact(
            [Values(2009, 2009, 2008, 2008, 2008, 2008, 2007)] int year,
            [Values(   6,    1,   12,    3,    2,    1,   12)] int month,
            [Values(  15,    1,   31,    1,   29,    1,   31)] int day,
            [Values(  12,   12,    0,    0,   23,    0,   23)] int hour,
            [Values(   0,    0,    0,    0,   59,    0,   59)] int minute,
            [Values(   0,    0,    0,    0,   59,    0,   59)] int second)
        {
            DateTime dt = new DateTime(year, month, day, hour, minute, second);
            DatePropertyQuery query = new DatePropertyQuery(PropertyNames.start_time, dt, DatePropertyQuery.PropertyQueryType.exact);
            CheckMatch(true, query, year, month, day, 0, 0, 0);
            CheckMatch(true, query, year, month, day, 23, 59, 59);
            DateTime daybefore = dt - new TimeSpan(1, 0, 0, 0);
            CheckMatch(false, query, daybefore.Year, daybefore.Month, daybefore.Day, 23, 59, 59);
            DateTime dayafter = dt + new TimeSpan(1, 0, 0, 0);
            CheckMatch(false, query, dayafter.Year, dayafter.Month, dayafter.Day, 0, 0, 0);
            DateTime yearbefore1 = dt - new TimeSpan(365, 0, 0, 0);
            DateTime yearbefore2 = dt - new TimeSpan(366, 0, 0, 0);
            CheckMatch(false, query, yearbefore1.Year, yearbefore1.Month, yearbefore1.Day, 12, 0, 0);
            CheckMatch(false, query, yearbefore2.Year, yearbefore2.Month, yearbefore2.Day, 12, 0, 0);
            DateTime yearafter1 = dt + new TimeSpan(365, 0, 0, 0);
            DateTime yearafter2 = dt + new TimeSpan(366, 0, 0, 0);
            CheckMatch(false, query, yearafter1.Year, yearafter1.Month, yearafter1.Day, 12, 0, 0);
            CheckMatch(false, query, yearafter2.Year, yearafter2.Month, yearafter2.Day, 12, 0, 0);
        }

        private void CheckMatch(bool expected, DatePropertyQuery query, int year, int month, int day, int hour, int minute, int second)
        {
            DateTime dt = new DateTime(year, month, day, hour, minute, second);
            bool? match = query.MatchProperty(dt.ToUniversalTime());
            Assert.IsTrue(match.HasValue);
            Assert.AreEqual(expected, match.Value);
        }
    }
}
