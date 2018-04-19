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
    public class WlbScheduledTasksTests
    {
        [Test]
        public void MethodCallsFromAnEmptyConstructor()
        {
            WlbScheduledTasks tasks = new WlbScheduledTasks();
            Assert.AreEqual(0, tasks.TaskList.Count, "TaskList");
            Assert.AreEqual(0, tasks.SortedTaskList.Count, "SortedTaskList");
            Assert.AreEqual(0, tasks.VirtualTaskList.Count, "VirtualTaskList");
            Assert.IsNull(tasks.ToDictionary(), "Conversion to dictionary");
            Assert.IsNull(tasks.GetNextExecutingTask(), "GetNextExecutingTask");
        }

        [Test]
        public void EmptyConstructorCausesCurrentScheduledPerformanceModeToThrow()
        {
            WlbScheduledTasks tasks = new WlbScheduledTasks();
            Assert.That(()=> tasks.GetCurrentScheduledPerformanceMode(), Throws.Exception.With.TypeOf(typeof(IndexOutOfRangeException)));
        }

        [Test]
        public void EmptyConstructorCausesLastTaskToThrow()
        {
            WlbScheduledTasks tasks = new WlbScheduledTasks();
            Assert.That(() => tasks.GetLastExecutingTask(), Throws.Exception.With.TypeOf(typeof(IndexOutOfRangeException)));
        }

        [Test]
        public void MethodCallsFromADictionaryConstructedObject()
        {
            WlbScheduledTasks tasks = new WlbScheduledTasks(new Dictionary<string, string>()
                                                                {
                                                                   {"schedTask_dosomething", "now"},
                                                                   {"schedTask_3", "later"},
                                                                   {"schedTask_1", "sooner"},
                                                                   {"domoresomethings", "will not be added"} 
                                                                });
            //Task List Construction
            Assert.AreEqual(3, tasks.TaskList.Count );
            Assert.AreEqual(0, tasks.TaskList["dosomething"].TaskId);
            Assert.AreEqual(1, tasks.TaskList["1"].TaskId);
            Assert.AreEqual(3, tasks.TaskList["3"].TaskId);

            //Dictionary Conversion
            Assert.AreEqual(3, tasks.ToDictionary().Count, "Conversion to dictionary");

            //Sorted Tasks
            SortedDictionary<int, WlbScheduledTask> sortedTasks = tasks.SortedTaskList;
            Assert.AreEqual(3, sortedTasks.Count, "SortedTaskList");
            List<WlbScheduledTask> tasksValues = new List<WlbScheduledTask>(sortedTasks.Values);
            Assert.AreEqual(0, tasksValues[0].TaskId);
            Assert.AreEqual(3, tasksValues[1].TaskId);
            Assert.AreEqual(1, tasksValues[2].TaskId);

            //Virtual Tasks
            Assert.AreEqual(0, tasks.VirtualTaskList.Count, "VirtualTaskList");
            
            //Next task
            WlbScheduledTask nextTask = tasks.GetNextExecutingTask();
            Assert.IsNull( nextTask, "GetNextExecutingTask");
        }

        [Test]
        public void VirtualTaskListWithAddedTasks()
        {
            WlbScheduledTasks tasks = BuildSampleTasksWithTimes();
            SortedDictionary<int, WlbScheduledTask> virtualTasks = tasks.VirtualTaskList;
            Assert.AreEqual(3, tasks.TaskList.Count);
            Assert.AreEqual(3, tasks.SortedTaskList.Count);
            Assert.AreEqual(4, virtualTasks.Count);

            //Verify sort keys from the virtual list and their task id
            List<WlbScheduledTask> virtualTasksValues = new List<WlbScheduledTask>(virtualTasks.Values);
            Assert.AreEqual(3, virtualTasksValues[0].TaskId);
            Assert.AreEqual(2, virtualTasksValues[1].TaskId);
            Assert.AreEqual(1, virtualTasksValues[2].TaskId);
            Assert.AreEqual(3, virtualTasksValues[3].TaskId);

            //Next Task
            Assert.IsNull(tasks.GetNextExecutingTask());

            //Last Task
            Assert.IsNotNull(tasks.GetLastExecutingTask());

            //Check performance mode fetch from last task
            Assert.AreEqual(WlbPoolPerformanceMode.MaximizeDensity, tasks.GetCurrentScheduledPerformanceMode());
        }

        private WlbScheduledTasks BuildSampleTasksWithTimes()
        {
            WlbScheduledTasks tasks = new WlbScheduledTasks();

            Dictionary<string, string > taskParameters = new Dictionary<string, string>()
                                                             {
                                                                 { "OptMode", "1"} // Performance mode
                                                             };

            WlbScheduledTask taskA = new WlbScheduledTask("1")
                                         { 
                                             DaysOfWeek = WlbScheduledTask.WlbTaskDaysOfWeek.Friday,
                                             TaskParameters = taskParameters
                                         };
            WlbScheduledTask taskB = new WlbScheduledTask("2")
                                         {
                                             DaysOfWeek = WlbScheduledTask.WlbTaskDaysOfWeek.Monday,
                                             TaskParameters = taskParameters
                                         };

            //Weekend tasks adds 2 to the virtual task list, one for each day
            WlbScheduledTask taskC = new WlbScheduledTask("3")
                                         {
                                             DaysOfWeek = WlbScheduledTask.WlbTaskDaysOfWeek.Weekends,
                                             TaskParameters = taskParameters
                                         };
            Dictionary<string, WlbScheduledTask> taskCollection = new Dictionary<string, WlbScheduledTask>()
                                                                      {
                                                                          {"schedTask_1", taskA},
                                                                          {"schedTask_2", taskB},
                                                                          {"schedTask_3", taskC}
                                                                      };

            Assert.AreEqual(3, taskCollection.Count, "Setting up task collection");
            tasks.TaskList = taskCollection;
            return tasks;
        }

    }
}
