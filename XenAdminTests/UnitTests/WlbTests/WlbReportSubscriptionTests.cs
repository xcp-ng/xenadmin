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
using XenAdmin.Wlb;


namespace XenAdminTests.UnitTests.WlbTests
{

    [TestFixture, Category(TestCategories.Unit)]
    public class WlbReportSubscriptionTests
    {
        [Test]
        public void TestNullConstructionDoesNotThrow()
        {
            Assert.DoesNotThrow(() => new WlbReportSubscription(null));
        }

        [Test]
        public void GettersAndSetters()
        {
            var expected = new SubscriptionData
            {
                Id = "subs Id",
                Created = new DateTime( 2011, 12, 25),
                Name = "my name",
                Description = "meaningful description",
                SubscriberId = "1",
                SubscriberName = "subscriber name",
                ScheduleId = "2",
                DaysOfWeek = WlbScheduledTask.WlbTaskDaysOfWeek.Monday,
                ExecuteTimeOfDay = new DateTime(2011, 12, 26),
                TriggerType = 3,
                Enabled = true,
                EnableDate = new DateTime(2011, 12, 27),
                LastTouched = new DateTime(2011, 12, 28),
                LastTouchedBy = "wasn't me!!",
                LastRun = new DateTime(2011, 12, 29),
                LastRunResult = "broken",
                EmailTo = "you",
                EmailReplyTo = "definitely you",
                ReportRenderFormat = 4,
                EmailSubject = "stuff is broken",
                EmailComment = "badly",
                EmailCc = "me",
                EmailBcc = "someone else",
                ReportId = 5,
                ReportName = "Report name",
                ReportParameters = new Dictionary<string, string>(){{"key", "value"}},
                ReportDisplayName = "Display name"
            };

            ClassVerifiers.VerifySettersAndGetters(new WlbReportSubscription("some id"), expected);

        }

        #region Helper structs

        private struct SubscriptionData
        {
            public string Id;
            public DateTime Created;
            public string Name;
            public string Description;
            public string SubscriberId;
            public string SubscriberName;
            public string ScheduleId;
            public WlbScheduledTask.WlbTaskDaysOfWeek DaysOfWeek;
            public DateTime ExecuteTimeOfDay;
            public int TriggerType;
            public bool Enabled;
            public DateTime EnableDate;
            public DateTime LastTouched;
            public string LastTouchedBy;
            public DateTime LastRun;
            public string LastRunResult;
            public string EmailTo;
            public string EmailReplyTo;
            public int ReportRenderFormat;
            public string EmailSubject;
            public string EmailComment;
            public string EmailCc;
            public string EmailBcc;
            public int ReportId;
            public string ReportName;
            public Dictionary<string, string> ReportParameters;
            public string ReportDisplayName;
        }

        #endregion
    }
}
