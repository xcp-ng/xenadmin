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
using XenCenterLib;
using NUnit.Framework;

namespace XenAdminTests.MiscTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    class TimeUtilTests
    {
        private readonly long ticksSecondsFactor = Convert.ToInt64(1e7);

        [Test]
        public void TicksToSecondsConversion()
        {
            Assert.AreEqual(73, TimeUtil.TicksToSeconds(73*ticksSecondsFactor));
            Assert.AreEqual(0, TimeUtil.TicksToSeconds(0));
        }

        [Test]
        public void TicksBefore1970Check()
        {
            Assert.AreEqual(621355968000000000, TimeUtil.TicksBefore1970);
        }

        [Test]
        public void ISO8601DateFormatStringCheck()
        {
            Assert.AreEqual("yyyyMMddTHH:mm:ssZ", TimeUtil.ISO8601DateFormat);
        }

        [Test]
        public void TicksToSecondsSince1970Conversion()
        {
            Assert.AreEqual((-1 * TimeUtil.TicksBefore1970 / ticksSecondsFactor), TimeUtil.TicksToSecondsSince1970(0));
            Assert.AreEqual(0, TimeUtil.TicksToSecondsSince1970(TimeUtil.TicksBefore1970));
            Assert.AreEqual(1324771200, TimeUtil.TicksToSecondsSince1970(new DateTime(2011,12,25).Ticks));
        }

        [Test]
        public void ISODateTimeParse()
        {
            DateTime derived = TimeUtil.ParseISO8601DateTime("20111225T10:20:37Z");
            Assert.AreEqual(new DateTime(2011, 12, 25, 10, 20, 37), derived );
            Assert.AreEqual(DateTimeKind.Utc, derived.Kind);
        }

        [Test]
        public void ISODateTimeParseWithBadFormat()
        {
            Assert.That(() => TimeUtil.ParseISO8601DateTime("20111225T1020:37Z"), Throws.Exception.With.TypeOf(typeof(FormatException)));
        }

        [Test]
        public void ISODateTimeParseWithNullArg()
        {
            Assert.That(() => TimeUtil.ParseISO8601DateTime(null), Throws.ArgumentNullException);
        }

        [Test]
        public void ToISODateTime()
        {
            string derived = TimeUtil.ToISO8601DateTime(new DateTime(2011, 12, 25, 10, 20, 37, DateTimeKind.Utc));
            Assert.AreEqual("20111225T10:20:37Z", derived);
        }

        [Test]
        public void ISODateTimeRoundTrip()
        {
            const string dateToParse = "20111225T10:20:37Z";
            string derived = TimeUtil.ToISO8601DateTime(TimeUtil.ParseISO8601DateTime(dateToParse));
            Assert.AreEqual(dateToParse, derived);
        }

    }
}
