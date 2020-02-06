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
using NUnit.Framework;
using XenAdmin.Alerts;

namespace XenAdminTests.LicensingTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class LicenceTimerTests
    {
        private static Dictionary<TimeSpan, string> sharedResults = new Dictionary<TimeSpan, string>()
        {
            {new TimeSpan(3650, 0, 0, 0), "121 months"},
            {new TimeSpan(365, 0, 0, 0), "12 months"},
            {new TimeSpan(90, 0, 0, 0), "3 months"},
            {new TimeSpan(34, 0, 0, 0), "34 days"},
            {new TimeSpan(14, 0, 0, 0), "14 days"},
            {new TimeSpan(7, 0, 0, 0), "7 days"},
            {new TimeSpan(3, 0, 0, 0), "3 days"},
            {new TimeSpan(2, 5, 0, 0), "2 days"},
            {new TimeSpan(2, 0, 0, 0), "48 hours"},
            {new TimeSpan(1, 0, 0, 0), "24 hours"},
            {new TimeSpan(0, 1, 33, 0), "93 minutes"},
            {new TimeSpan(0, 1, 0, 0), "60 minutes"},
            {new TimeSpan(0, 0, 2, 0), "2 minutes"},
            {new TimeSpan(0, 0, 1, 0), "1 minutes"},
            {new TimeSpan(0, 0, 0, 25), "1 minutes"},
            {new TimeSpan(0, 0, 0, 0), "0 minutes"},
            {new TimeSpan(0, 0, 0, -1), ""}
        };

        private static Dictionary<TimeSpan, string> uncappedResults = new Dictionary<TimeSpan, string>(sharedResults)
        {
            {new TimeSpan(7300, 0, 0, 0), "243 months"}
        };

        private static Dictionary<TimeSpan, string> cappedResults = new Dictionary<TimeSpan, string>(sharedResults)
        {
            {new TimeSpan(7300, 0, 0, 0), "Unlimited"}
        };


        [Test]
        [TestCaseSource(typeof(LicenceTimerTests), nameof(uncappedResults))]
        public void UncappedTimeToStringConversion(KeyValuePair<TimeSpan, string> kvp)
        {
            Assert.AreEqual(kvp.Value, LicenseAlert.GetLicenseTimeLeftString(kvp.Key, false));
        }

        [Test]
        [TestCaseSource(typeof(LicenceTimerTests), nameof(cappedResults))]
        public void CappedTimeToStringConversion(KeyValuePair<TimeSpan, string> kvp)
        {
            Assert.AreEqual(kvp.Value, LicenseAlert.GetLicenseTimeLeftString(kvp.Key, true));
        }
    }
}
