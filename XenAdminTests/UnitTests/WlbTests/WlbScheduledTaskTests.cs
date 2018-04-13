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
using XenAdminTests.UnitTests.UnitTestHelper;

namespace XenAdminTests.UnitTests.WlbTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class WlbScheduledTaskTests
    {
        #region Private Class Data
        private const int NUMBER_OF_PROPERTIES = 18;
        private WlbScheduledTask task;

        private ScheduledTaskData exampleData = new ScheduledTaskData()
                                                    {
                                                        DeleteTask = true,
                                                        Name = "John Doe",
                                                        Description = "Friendly",
                                                        Enabled = true,
                                                        Owner = "You",
                                                        LastRunResult = true,
                                                        LastTouchedBy = "Me",
                                                        LastTouched = new DateTime(2011, 12, 25),
                                                        TriggerInterval = WlbScheduledTask.WlbTaskTriggerType.Daily,
                                                        DaysOfWeek = WlbScheduledTask.WlbTaskDaysOfWeek.Weekdays,
                                                        ExecuteTime = new DateTime(2011, 12, 26),
                                                        LastRunDate = new DateTime(2011, 12, 27),
                                                        EnableDate = new DateTime(2011, 12, 28),
                                                        DisableTime = new DateTime(2011, 12, 29),
                                                        ActionType =
                                                            WlbScheduledTask.WlbTaskActionType.SetOptimizationMode,
                                                        TaskParameters =
                                                            new Dictionary<string, string>() { { "key", "value" } }
                                                    }; 
        #endregion

        [SetUp]
        public void Setup()
        {
            task = new WlbScheduledTask("73");
        }

        [Test]
        public void VerifyGettersAndSetters()
        {
            IUnitTestVerifier validator = new VerifyGettersAndSetters(task);
            validator.Verify(exampleData);

            IUnitTestVerifier countValidator = new VerifyPropertyCounter(task);
            countValidator.Verify(NUMBER_OF_PROPERTIES);
            Assert.AreEqual(73, task.TaskId, "Task ID as set in ctor");
        }

        [Test]
        public void NonNumericItemIDInCtor()
        {
            WlbScheduledTask task = new WlbScheduledTask("not a number");
            Assert.AreEqual(0, task.TaskId, "Non-numeric task ID");
        }

        [Test]
        public void CheckClone()
        {
            IUnitTestVerifier originalTaskSetterValidator = new VerifyGettersAndSetters(task);
            IUnitTestVerifier originalTaskCounterValidator = new VerifyPropertyCounter(task);
            WlbScheduledTask clone = task.Clone();
            IUnitTestVerifier clonedTaskSetterValidator = new VerifyGettersAndSetters(clone);
            IUnitTestVerifier clonedTaskCounterValidator = new VerifyPropertyCounter(clone);

            Assert.AreNotEqual(task, clone);

            //Check contents are all equal to the expected
            originalTaskCounterValidator.Verify(NUMBER_OF_PROPERTIES);
            clonedTaskCounterValidator.Verify(NUMBER_OF_PROPERTIES);

            originalTaskSetterValidator.Verify(exampleData);
            clonedTaskSetterValidator.Verify(exampleData);

        }

        [Test]
        public void ExceptionRaisedIfOptModeNotSetButRequested()
        {
            Assert.That(() => WlbScheduledTask.GetTaskOptMode(task), Throws.Exception.With.TypeOf(typeof(KeyNotFoundException)));
        }

        [Test]
        public void VerifyGetTaskOptModeOperationMaximizePerformance()
        {
            task.AddTaskParameter("OptMode", "0");
            Assert.AreEqual(WlbPoolPerformanceMode.MaximizePerformance, WlbScheduledTask.GetTaskOptMode(task));
        }

        [Test]
        public void VerifyGetTaskOptModeOperationMaximizeDensity()
        {
            task.AddTaskParameter("OptMode", "1");
            Assert.AreEqual(WlbPoolPerformanceMode.MaximizeDensity, WlbScheduledTask.GetTaskOptMode(task));
        }

        [Test]
        public void AddingTaskParameters()
        {
            int initialTaskCount = task.TaskParameters.Count;
            task.AddTaskParameter("OptMode", "1");
            Assert.AreEqual( initialTaskCount + 1, task.TaskParameters.Count, "Adding task parameters" );
        }

        [Test]
        public void TaskExecutionTime()
        {
            const string expectedTime = "11:34 AM";
            string executionTime = WlbScheduledTask.GetTaskExecuteTime(new DateTime(2011, 11, 20, 11, 34, 01));
            Assert.AreEqual(expectedTime, executionTime);
        }

        [Test]
        public void NextAndPreviousDayOfTheWeek()
        {
            //Single days
            Dictionary<WlbScheduledTask.WlbTaskDaysOfWeek, WlbScheduledTask.WlbTaskDaysOfWeek> dotw =
                new Dictionary<WlbScheduledTask.WlbTaskDaysOfWeek, WlbScheduledTask.WlbTaskDaysOfWeek>()
                    {
                        { WlbScheduledTask.WlbTaskDaysOfWeek.Saturday, WlbScheduledTask.WlbTaskDaysOfWeek.Sunday},
                        { WlbScheduledTask.WlbTaskDaysOfWeek.Sunday, WlbScheduledTask.WlbTaskDaysOfWeek.Monday},
                        { WlbScheduledTask.WlbTaskDaysOfWeek.Monday, WlbScheduledTask.WlbTaskDaysOfWeek.Tuesday},
                        { WlbScheduledTask.WlbTaskDaysOfWeek.Tuesday, WlbScheduledTask.WlbTaskDaysOfWeek.Wednesday},
                        { WlbScheduledTask.WlbTaskDaysOfWeek.Wednesday, WlbScheduledTask.WlbTaskDaysOfWeek.Thursday},
                        { WlbScheduledTask.WlbTaskDaysOfWeek.Thursday, WlbScheduledTask.WlbTaskDaysOfWeek.Friday},
                        { WlbScheduledTask.WlbTaskDaysOfWeek.Friday, WlbScheduledTask.WlbTaskDaysOfWeek.Saturday}
                    };
            foreach (var day in dotw)
            {
                Assert.AreEqual(day.Value, WlbScheduledTask.NextDay(day.Key), "next day of the week");
                Assert.AreEqual(day.Key, WlbScheduledTask.PreviousDay(day.Value), "previous day of the week");
            }

            //Weekends
            const WlbScheduledTask.WlbTaskDaysOfWeek beforeWeekend = (WlbScheduledTask.WlbTaskDaysOfWeek.Friday |
                                                                      WlbScheduledTask.WlbTaskDaysOfWeek.Saturday);
            const WlbScheduledTask.WlbTaskDaysOfWeek afterWeekend = (WlbScheduledTask.WlbTaskDaysOfWeek.Sunday |
                                                                     WlbScheduledTask.WlbTaskDaysOfWeek.Monday);
            Assert.AreEqual(afterWeekend, WlbScheduledTask.NextDay(WlbScheduledTask.WlbTaskDaysOfWeek.Weekends));
            Assert.AreEqual(beforeWeekend, WlbScheduledTask.PreviousDay(WlbScheduledTask.WlbTaskDaysOfWeek.Weekends));

            //Weekdays
            const WlbScheduledTask.WlbTaskDaysOfWeek beforeWeek = (WlbScheduledTask.WlbTaskDaysOfWeek.Sunday |
                                                                   WlbScheduledTask.WlbTaskDaysOfWeek.Monday |
                                                                   WlbScheduledTask.WlbTaskDaysOfWeek.Tuesday |
                                                                   WlbScheduledTask.WlbTaskDaysOfWeek.Wednesday |
                                                                   WlbScheduledTask.WlbTaskDaysOfWeek.Thursday);

            const WlbScheduledTask.WlbTaskDaysOfWeek afterWeek = (WlbScheduledTask.WlbTaskDaysOfWeek.Tuesday |
                                                                  WlbScheduledTask.WlbTaskDaysOfWeek.Wednesday |
                                                                  WlbScheduledTask.WlbTaskDaysOfWeek.Thursday |
                                                                  WlbScheduledTask.WlbTaskDaysOfWeek.Friday |
                                                                  WlbScheduledTask.WlbTaskDaysOfWeek.Saturday);

            Assert.AreEqual(afterWeek, WlbScheduledTask.NextDay(WlbScheduledTask.WlbTaskDaysOfWeek.Weekdays));
            Assert.AreEqual(beforeWeek, WlbScheduledTask.PreviousDay(WlbScheduledTask.WlbTaskDaysOfWeek.Weekdays));


            const WlbScheduledTask.WlbTaskDaysOfWeek everyDay = (WlbScheduledTask.WlbTaskDaysOfWeek.Sunday |
                                                                 WlbScheduledTask.WlbTaskDaysOfWeek.Monday |
                                                                 WlbScheduledTask.WlbTaskDaysOfWeek.Tuesday |
                                                                 WlbScheduledTask.WlbTaskDaysOfWeek.Wednesday |
                                                                 WlbScheduledTask.WlbTaskDaysOfWeek.Thursday |
                                                                 WlbScheduledTask.WlbTaskDaysOfWeek.Friday |
                                                                 WlbScheduledTask.WlbTaskDaysOfWeek.Saturday);

            Assert.AreEqual(everyDay, WlbScheduledTask.NextDay(WlbScheduledTask.WlbTaskDaysOfWeek.All));
            Assert.AreEqual(everyDay, WlbScheduledTask.PreviousDay(WlbScheduledTask.WlbTaskDaysOfWeek.All));

        }

        [Test]
        public void DayOfWeekConversionRoundTrip()
        {
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                DayOfWeek roundTrippedDay =
                    WlbScheduledTask.ConvertFromWlbTaskDayOfWeek(WlbScheduledTask.ConvertToWlbTaskDayOfWeek(day));
                Assert.AreEqual( day, roundTrippedDay);
            }
        }

        #region Helpers
        private struct ScheduledTaskData
        {
            public bool DeleteTask;
            public string Name;
            public string Description;
            public bool Enabled;
            public string Owner;
            public bool LastRunResult;
            public string LastTouchedBy;
            public DateTime LastTouched;
            public WlbScheduledTask.WlbTaskTriggerType TriggerInterval;
            public WlbScheduledTask.WlbTaskDaysOfWeek DaysOfWeek;
            public DateTime ExecuteTime;
            public DateTime LastRunDate;
            public DateTime EnableDate;
            public DateTime DisableTime;
            public WlbScheduledTask.WlbTaskActionType ActionType;
            public Dictionary<string, string> TaskParameters;
        } 
        #endregion
    }
}
