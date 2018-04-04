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


namespace XenAdmin.Wlb
{
    public class WlbScheduledTask : WlbConfigurationBase
    {
        private const string KEY_TASK_NAME = "TaskName";
        private const string KEY_TASK_DESCRIPTION = "TaskDescription";
        private const string KEY_TASK_ENABLED = "TaskEnabled";
        private const string KEY_TASK_OWNER = "TaskOwner";
        private const string KEY_TASK_LAST_RUN_RESULT = "TaskLastRunResult";
        private const string KEY_TASK_LAST_TOUCHED_BY = "TaskLastTouchedBy";
        private const string KEY_TASK_LAST_TOUCHED = "TaskLastTouched";

        private const string KEY_TRIGGER_TYPE = "TriggerType";
        private const string KEY_TRIGGER_DAYS_OF_WEEK = "TriggerDaysOfWeek";
        private const string KEY_TRIGGER_EXECUTE_TIME = "TriggerExecuteTime";
        private const string KEY_TRIGGER_LAST_RUN = "TriggerLastRun";
        private const string KEY_TRIGGER_ENABLED_DATE = "TriggerEndabledDate";
        private const string KEY_TRIGGER_DISABLED_DATE = "TriggerDisabledDate";

        private const string KEY_DELETE_TASK = "TaskDelete";

        private const string KEY_ACTION_TYPE = "ActionType";

        /// <summary>
        /// Public enumeration describing the interval period of a WlbTaskTrigger
        /// </summary>
        public enum WlbTaskTriggerType : int
        {
            /// <summary>
            /// A single-shot trigger
            /// </summary>
            Once = 0,
            /// <summary>
            /// A trigger that occurs every day at a particular time
            /// </summary>
            Daily = 1,
            /// <summary>
            /// A trigger that occurs every week on a set of days at a particulat time
            /// </summary>
            Weekly = 2,
            /// <summary>
            /// A trigger that occurs once every month on a given date
            /// </summary>
            Monthly = 3
        }

        public enum WlbTaskActionType : int
        {
            Unknown = 0,
            SetOptimizationMode = 1,
            ReportSubscription = 2
        }

        /// <summary>
        /// Public enumeration of the days on which a weekly WlbTaskTrigger will execute
        /// </summary>
        [FlagsAttribute]
        public enum WlbTaskDaysOfWeek
        {
            /// <summary>
            /// None
            /// </summary>
            None = 0,
            /// <summary>
            /// Sunday
            /// </summary>
            Sunday = 1,
            /// <summary>
            /// Monday
            /// </summary>
            Monday = 2,
            /// <summary>
            /// Tuesday
            /// </summary>
            Tuesday = 4,
            /// <summary>
            /// Wednesday
            /// </summary>
            Wednesday = 8,
            /// <summary>
            /// Thursday
            /// </summary>
            Thursday = 16,
            /// <summary>
            /// Friday
            /// </summary>
            Friday = 32,
            /// <summary>
            /// Saturday
            /// </summary>
            Saturday = 64,
            /// <summary>
            /// All weekdays
            /// </summary>
            Weekends = Sunday | Saturday,
            /// <summary>
            /// Only weekend days
            /// </summary>
            Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday,
            /// <summary>
            /// All days
            /// </summary>
            All = Weekdays | Weekends
        }

        public static string DaysOfWeekL10N(WlbTaskDaysOfWeek days)
        {
            switch (days)
            {
                case WlbTaskDaysOfWeek.Sunday:
                    return Messages.SUNDAY_LONG;
                case WlbTaskDaysOfWeek.Monday:
                    return Messages.MONDAY_LONG;
                case WlbTaskDaysOfWeek.Tuesday:
                    return Messages.TUESDAY_LONG;
                case WlbTaskDaysOfWeek.Wednesday:
                    return Messages.WEDNESDAY_LONG;
                case WlbTaskDaysOfWeek.Thursday:
                    return Messages.THURSDAY_LONG;
                case WlbTaskDaysOfWeek.Friday:
                    return Messages.FRIDAY_LONG;
                case WlbTaskDaysOfWeek.Saturday:
                    return Messages.SATURDAY_LONG;
                case WlbTaskDaysOfWeek.Weekdays:
                    return Messages.WLB_DAY_WEEKDAYS;
                case WlbTaskDaysOfWeek.Weekends:
                    return Messages.WLB_DAY_WEEKENDS;
                case WlbTaskDaysOfWeek.All:
                    return Messages.WLB_DAY_ALL;
                default:
                    return "";
            }
        }

        public static WlbPoolPerformanceMode GetTaskOptMode(WlbScheduledTask task)
        {
            WlbPoolPerformanceMode mode = WlbPoolPerformanceMode.MaximizePerformance;

            if (task.TaskParameters["OptMode"] == "0")
            {
                mode = WlbPoolPerformanceMode.MaximizePerformance;
            }
            else
            {
                mode = WlbPoolPerformanceMode.MaximizeDensity;
            }
            return mode;
        }

        public static string GetTaskExecuteTime(DateTime TaskExecuteTime)
        {
            return HelpersGUI.DateTimeToString(TaskExecuteTime, Messages.DATEFORMAT_HM, true);
        }


        /// <summary>
        /// Returns the offset minutes between Utc and local time
        /// Add this to local time to get UTC
        /// Subtract from UTC to get local time
        /// </summary>
        /// <returns>(double) number of minutes between Utc and Local time</returns>
        private static double LocalOffsetMinutes()
        {
            TimeSpan difference = DateTime.UtcNow.Subtract(DateTime.Now);
            return difference.TotalMinutes;
        }

        /// <summary>
        /// Accepts a client's local time DayOfWeek and ExecuteTime of a scheduled task 
        /// and returns the DaysOfWeek and ExecuteTime adjusted to UTC time
        /// </summary>
        /// <param name="LocalDaysOfWeek">Task's DaysOfWeek value in local time</param>
        /// <param name="LocalExecuteTime">Task's ExecuteTime in local time</param>
        /// <param name="UtcDaysOfWeek">(Output) Task's DaysOfWeek value adjusted to UTC</param>
        /// <param name="UtcExecuteTime">(Output) Task's ExecuteTime value adjusted to UTC</param>
        public static void GetUTCTaskTimes(WlbScheduledTask.WlbTaskDaysOfWeek LocalDaysOfWeek, DateTime LocalExecuteTime, 
            out WlbScheduledTask.WlbTaskDaysOfWeek UtcDaysOfWeek, out DateTime UtcExecuteTime)
        {
            UtcDaysOfWeek = LocalDaysOfWeek;
            UtcExecuteTime = LocalExecuteTime.AddMinutes(LocalOffsetMinutes());
            if (DateTime.Compare(LocalExecuteTime.Date, UtcExecuteTime.Date) < 0)
            {
                UtcDaysOfWeek = WlbScheduledTask.NextDay(LocalDaysOfWeek);
            }
            else if (DateTime.Compare(LocalExecuteTime.Date, UtcExecuteTime.Date) > 0)
            {
                UtcDaysOfWeek = WlbScheduledTask.PreviousDay(LocalDaysOfWeek);
            }
        }

        /// <summary>
        /// Accepts UTC DayOfWeek and ExecuteTime of a scheduled task 
        /// and returns the DaysOfWeek and ExecuteTime adjusted to client's local time
        /// </summary>
        /// <param name="UtcDaysOfWeek">Task's DaysOfWeek value in UTC</param>
        /// <param name="UtcExecuteTime">Task's ExecuteTime in UTC</param>
        /// <param name="LocalDaysOfWeek">(Output) Task's DaysOfWeek value adjusted to local time</param>
        /// <param name="LocalExecuteTime">(Output) Task's ExecuteTime value adjusted to local time</param>
        public static void GetLocalTaskTimes(WlbScheduledTask.WlbTaskDaysOfWeek UtcDaysOfWeek, DateTime UtcExecuteTime, 
            out WlbScheduledTask.WlbTaskDaysOfWeek LocalDaysOfWeek, out DateTime LocalExecuteTime)
        {
            LocalDaysOfWeek = UtcDaysOfWeek;
            LocalExecuteTime = UtcExecuteTime.AddMinutes(LocalOffsetMinutes() * -1);
            
            if (UtcDaysOfWeek != WlbTaskDaysOfWeek.None &&
                UtcDaysOfWeek != WlbTaskDaysOfWeek.All &&
                UtcDaysOfWeek != WlbTaskDaysOfWeek.Weekdays &&
                UtcDaysOfWeek != WlbTaskDaysOfWeek.Weekends)
            {
                if (DateTime.Compare(UtcExecuteTime.Date, LocalExecuteTime.Date) < 0)
                {
                    LocalDaysOfWeek = WlbScheduledTask.NextDay(UtcDaysOfWeek);
                }
                else if (DateTime.Compare(UtcExecuteTime.Date, LocalExecuteTime.Date) > 0)
                {
                    LocalDaysOfWeek = WlbScheduledTask.PreviousDay(UtcDaysOfWeek);
                }
            }
        }

        public static WlbTaskDaysOfWeek NextDay(WlbTaskDaysOfWeek daysOfWeek)
        {
            // Doing some hackery here to shift days in the enumeration
            switch (daysOfWeek)
            {
                case WlbTaskDaysOfWeek.Saturday:
                    {
                        return WlbTaskDaysOfWeek.Sunday;
                    }
                case WlbTaskDaysOfWeek.Weekends:
                    {
                        return (WlbTaskDaysOfWeek.Sunday |
                                WlbTaskDaysOfWeek.Monday);
                    }
                case WlbTaskDaysOfWeek.Weekdays:
                    {
                        return (WlbTaskDaysOfWeek.Tuesday |
                                WlbTaskDaysOfWeek.Wednesday |
                                WlbTaskDaysOfWeek.Thursday |
                                WlbTaskDaysOfWeek.Friday |
                                WlbTaskDaysOfWeek.Saturday);
                    }
                case WlbTaskDaysOfWeek.All:
                    {
                        return daysOfWeek;
                    }
                // single days, Sunday through Friday, which can easily be 
                //  shifted back by one.  This also handles None (0).
                default:
                    {
                        //do the circular shift of rightmost 7 bits, discard the rest
                        int tempDays = (int)daysOfWeek;
                        tempDays = (tempDays << 1 | tempDays >> 6) & 0x0000007F;
                        return (WlbTaskDaysOfWeek)tempDays;
                        //return (WlbTaskDaysOfWeek)(((int)daysOfWeek) * 2);
                    }
            }
        }       

        public static WlbTaskDaysOfWeek PreviousDay(WlbTaskDaysOfWeek daysOfWeek)
        {
            // Doing some hackery here to shift days in the enumeration
            switch (daysOfWeek)
            {
                case WlbTaskDaysOfWeek.Sunday:
                    {
                        return WlbTaskDaysOfWeek.Saturday;
                    }
                case WlbTaskDaysOfWeek.Weekends:
                    {
                        return (WlbTaskDaysOfWeek.Friday |
                                WlbTaskDaysOfWeek.Saturday);
                    }
                case WlbTaskDaysOfWeek.Weekdays:
                    {
                        return (WlbTaskDaysOfWeek.Sunday |
                               WlbTaskDaysOfWeek.Monday |
                               WlbTaskDaysOfWeek.Tuesday |
                               WlbTaskDaysOfWeek.Wednesday |
                               WlbTaskDaysOfWeek.Thursday);
                    }
                case WlbTaskDaysOfWeek.All:
                    {
                        return daysOfWeek;
                    }
                // single days, monday through saturday, which can easily be 
                //  shifted back by one.  This also handles None (0).
                default:
                    {
                        //do the circular shift of rightmost 7 bits, discard the rest
                        int tempDays = (int)daysOfWeek;
                        tempDays = (tempDays >> 1 | tempDays << 6) & 0x0000007F;
                        return (WlbTaskDaysOfWeek)tempDays;
                        //return (WlbTaskDaysOfWeek)(((int)daysOfWeek) / 2);
                    }
            }
        }

        public static WlbScheduledTask.WlbTaskDaysOfWeek ConvertToWlbTaskDayOfWeek(DayOfWeek dayOfWeek)
        {
            return (WlbScheduledTask.WlbTaskDaysOfWeek)(Math.Pow(2,(int)dayOfWeek) % 127);
        }

        public static DayOfWeek ConvertFromWlbTaskDayOfWeek(WlbScheduledTask.WlbTaskDaysOfWeek wlbDayOfWeek)
        {
            return (DayOfWeek)(Math.Log((int)wlbDayOfWeek, 2));
        }

        public WlbScheduledTask(string TaskId)
        {
            base.Configuration = new Dictionary<string, string>();
            base.ItemId = TaskId;

            //Define the key base
            base.KeyBase = WlbConfigurationKeyBase.schedTask;

            //Define the known keys
            WlbConfigurationKeys = new List<string>(new string[] 
                { 
                    KEY_TASK_NAME,
                    KEY_TASK_DESCRIPTION,
                    KEY_TASK_ENABLED,
                    KEY_TASK_OWNER,
                    KEY_TASK_LAST_RUN_RESULT,
                    KEY_TASK_LAST_TOUCHED_BY,
                    KEY_TASK_LAST_TOUCHED,

                    KEY_TRIGGER_TYPE,
                    KEY_TRIGGER_DAYS_OF_WEEK,
                    KEY_TRIGGER_EXECUTE_TIME,
                    KEY_TRIGGER_LAST_RUN,
                    KEY_TRIGGER_ENABLED_DATE,
                    KEY_TRIGGER_DISABLED_DATE,

                    KEY_DELETE_TASK,

                    KEY_ACTION_TYPE
                });
        }

        public bool DeleteTask
        {
            get { return GetConfigValueBool(BuildComplexKey(KEY_DELETE_TASK)); }
            set { SetConfigValueBool(BuildComplexKey(KEY_DELETE_TASK), value, true); }
        }

        public string Name
        {
            get { return GetConfigValueString(BuildComplexKey(KEY_TASK_NAME)); }
            set { SetConfigValueString(BuildComplexKey(KEY_TASK_NAME), value, true); }
        }

        public string Description
        {
            get { return GetConfigValueString(BuildComplexKey(KEY_TASK_DESCRIPTION)); }
            set { SetConfigValueString(BuildComplexKey(KEY_TASK_DESCRIPTION), value, true); }
        }

        public bool Enabled
        {
            get { return GetConfigValueBool(BuildComplexKey(KEY_TASK_ENABLED)); }
            set { SetConfigValueBool(BuildComplexKey(KEY_TASK_ENABLED), value, true); }
        }

        public string Owner
        {
            get { return GetConfigValueString(BuildComplexKey(KEY_TASK_OWNER)); }
            set { SetConfigValueString(BuildComplexKey(KEY_TASK_OWNER), value, true); }
        }

        public bool LastRunResult
        {
            get { return GetConfigValueBool(BuildComplexKey(KEY_TASK_LAST_RUN_RESULT)); }
            set { SetConfigValueBool(BuildComplexKey(KEY_TASK_LAST_RUN_RESULT), value, true); }
        }

        public string LastTouchedBy
        {
            get { return GetConfigValueString(BuildComplexKey(KEY_TASK_LAST_TOUCHED_BY)); }
            set { SetConfigValueString(BuildComplexKey(KEY_TASK_LAST_TOUCHED_BY), value, true); }
        }

        public DateTime LastTouched
        {
            get { return GetConfigValueUTCDateTime(BuildComplexKey(KEY_TASK_LAST_TOUCHED)); }
            set { SetConfigValueUTCDateTime(BuildComplexKey(KEY_TASK_LAST_TOUCHED), value, true); }
        }

        public WlbTaskTriggerType TriggerInterval
        {
            get { return (WlbTaskTriggerType)GetConfigValueInt(BuildComplexKey(KEY_TRIGGER_TYPE)); }
            set { SetConfigValueInt(BuildComplexKey(KEY_TRIGGER_TYPE), (int)value, true); }
        }

        public WlbTaskDaysOfWeek DaysOfWeek
        {
            get { return (WlbTaskDaysOfWeek)GetConfigValueInt(BuildComplexKey(KEY_TRIGGER_DAYS_OF_WEEK)); }
            set { SetConfigValueInt(BuildComplexKey(KEY_TRIGGER_DAYS_OF_WEEK), (int)value, true); }
        }

        public DateTime ExecuteTime
        {
            get { return GetConfigValueUTCDateTime(BuildComplexKey(KEY_TRIGGER_EXECUTE_TIME)); }
            set { SetConfigValueUTCDateTime(BuildComplexKey(KEY_TRIGGER_EXECUTE_TIME), value, true); }
        }

        public DateTime LastRunDate
        {
            get { return GetConfigValueUTCDateTime(BuildComplexKey(KEY_TRIGGER_LAST_RUN)); }
            set { SetConfigValueUTCDateTime(BuildComplexKey(KEY_TRIGGER_LAST_RUN), value, true); }
        }

        public DateTime EnableDate
        {
            get { return GetConfigValueUTCDateTime(BuildComplexKey(KEY_TRIGGER_ENABLED_DATE)); }
            set { SetConfigValueUTCDateTime(BuildComplexKey(KEY_TRIGGER_ENABLED_DATE), value, true); }
        }

        public DateTime DisableTime
        {
            get { return GetConfigValueUTCDateTime(BuildComplexKey(KEY_TRIGGER_DISABLED_DATE)); }
            set { SetConfigValueUTCDateTime(BuildComplexKey(KEY_TRIGGER_DISABLED_DATE), value, true); }
        }

        public WlbTaskActionType ActionType
        {
            get { return (WlbTaskActionType)GetConfigValueInt(BuildComplexKey(KEY_ACTION_TYPE)); }
            set { SetConfigValueInt(BuildComplexKey(KEY_ACTION_TYPE), (int)value, true); }
        }

        public Dictionary<string, string> TaskParameters
        {
            get { return GetOtherParameters(); }
            set { SetOtherParameters(value); }
        }

        public void AddTaskParameter(string key, string value)
        {
            SetOtherParameter(key, value);
        }

        public int TaskId
        {
            get
            {
                int taskId = 0;
                Int32.TryParse(ItemId, out taskId);
                return taskId;
            }
        }

        public WlbScheduledTask Clone()
        {
            WlbScheduledTask newTask = new WlbScheduledTask(this.TaskId.ToString());
            newTask.ActionType = this.ActionType;
            newTask.DaysOfWeek = this.DaysOfWeek;
            newTask.DeleteTask = this.DeleteTask;
            newTask.Description = this.Description;
            newTask.DisableTime = this.DisableTime;
            newTask.Enabled = this.Enabled;
            newTask.EnableDate = this.EnableDate;
            newTask.ExecuteTime = this.ExecuteTime;
            newTask.LastRunDate = this.LastRunDate;
            newTask.LastRunResult = this.LastRunResult;
            newTask.LastTouched = this.LastTouched;
            newTask.LastTouchedBy = this.LastTouchedBy;
            newTask.Name = this.Name;
            newTask.Owner = this.Owner;
            newTask.TaskParameters = this.TaskParameters;
            newTask.TriggerInterval = this.TriggerInterval;

            return newTask;
        }
    }

    public class WlbScheduledTasks
    {

        private Dictionary<string, WlbScheduledTask> _tasks = new Dictionary<string, WlbScheduledTask>();

        public WlbScheduledTasks() { ;}

        /// <summary>
        /// Exposes the actual list of WlbScheduledTasks
        /// </summary>
        public Dictionary<string, WlbScheduledTask> TaskList
        {
            set { _tasks = value; }
            get { return _tasks; }
        }

        /// <summary>
        /// Exposes a sorted version of the WlbScheduledTasks collection
        /// </summary>
        public SortedDictionary<int, WlbScheduledTask> SortedTaskList
        {
            get
            {
                SortedDictionary<int, WlbScheduledTask> sortedTasks = new SortedDictionary<int, WlbScheduledTask>();
                foreach (WlbScheduledTask task in _tasks.Values)
                {
                    if (!task.DeleteTask)
                    {
                        WlbScheduledTask.WlbTaskDaysOfWeek localDaysOfWeek;
                        DateTime localExecuteTime;
                        WlbScheduledTask.GetLocalTaskTimes((task.DaysOfWeek), task.ExecuteTime, out localDaysOfWeek, out localExecuteTime);

                        int sortKey = GetSortKey(localDaysOfWeek, localExecuteTime);

                        //if the task is disabled, bump the sort key to prevent conficts
                        // with any new tasks.  This could start to get wierd after 100 duplicate
                        // disabled tasks, but then it will be the user's problem.
                        if (!task.Enabled)
                        {
                            sortKey += 1;
                            while (sortedTasks.ContainsKey(sortKey))
                            {
                                sortKey += 1;
                            }
                        }

                        WlbScheduledTask virtualTask = task.Clone();
                        virtualTask.DaysOfWeek = task.DaysOfWeek;
                        if (!sortedTasks.ContainsKey(sortKey))
                        {
                            sortedTasks.Add(sortKey, virtualTask);
                        }
                    }
                }
                return sortedTasks;
            }
        }

        /// <summary>
        /// Exposes a virtual representation of the WlbScheduledTasks collection, in which aggregate days
        /// are separated into individual days.  The entire list is also presorted chronologically.
        /// </summary>
        public SortedDictionary<int, WlbScheduledTask> VirtualTaskList
        {
            get
            {
                SortedDictionary<int, WlbScheduledTask> virtualTasks = new SortedDictionary<int, WlbScheduledTask>();
                foreach (WlbScheduledTask task in _tasks.Values)
                {
                    if (!task.DeleteTask)
                    {
                        foreach (WlbScheduledTask.WlbTaskDaysOfWeek dayValue in Enum.GetValues(typeof(WlbScheduledTask.WlbTaskDaysOfWeek)))
                        {
                            if (dayValue != WlbScheduledTask.WlbTaskDaysOfWeek.None &&
                                dayValue != WlbScheduledTask.WlbTaskDaysOfWeek.All &&
                                dayValue != WlbScheduledTask.WlbTaskDaysOfWeek.Weekdays &&
                                dayValue != WlbScheduledTask.WlbTaskDaysOfWeek.Weekends &&
                                ((task.DaysOfWeek & dayValue) == dayValue))
                            {
                                DateTime localExecuteTime;
                                WlbScheduledTask.WlbTaskDaysOfWeek localDaysOfWeek;
                                WlbScheduledTask.GetLocalTaskTimes((task.DaysOfWeek & dayValue), task.ExecuteTime, out localDaysOfWeek, out localExecuteTime);

                                int sortKey = GetSortKey(localDaysOfWeek, localExecuteTime);
                                //if the task is disabled, bump the sort key to prevent conficts
                                // with any new tasks.  This could start to get wierd after 100 duplicate
                                // disabled tasks, but then it will be the user's problem.
                                if (!task.Enabled)
                                {
                                    sortKey += 1;
                                    while (virtualTasks.ContainsKey(sortKey))
                                    {
                                        sortKey += 1;
                                    }
                                }

                                WlbScheduledTask virtualTask = task.Clone();
                                virtualTask.DaysOfWeek = dayValue;
                                if (!virtualTasks.ContainsKey(sortKey))
                                {
                                    virtualTasks.Add(sortKey, virtualTask);
                                }
                            }
                        }
                    }
                }
                return virtualTasks;
            }
        }

        /// <summary>
        /// Creates an artificial sort key that is used to sort the presentation of scheduled tasks.
        /// </summary>
        /// <param name="localDaysOfWeek">WlbScheduledTask.DaysOfWeek enumeration of the task denoting on which days it will execute</param>
        /// <param name="localExecuteTime">DateTime execute time of the task denoting on when the task </param>
        /// <returns></returns>
        private static int GetSortKey(WlbScheduledTask.WlbTaskDaysOfWeek localDaysOfWeek, DateTime localExecuteTime)
        {
            int sortKey;
            
            //The day of week is the primary sort item
            switch(localDaysOfWeek)
            {
                //Put ALL tasks at the front of the list
                case WlbScheduledTask.WlbTaskDaysOfWeek.All:
                    {
                        sortKey = 0;
                        break;
                    }
                //Next are WEEKDAY tasks
                case WlbScheduledTask.WlbTaskDaysOfWeek.Weekdays:
                    {
                        sortKey = 200000;
                        break;
                    }
                //Next are WEEKEND tasks
                case WlbScheduledTask.WlbTaskDaysOfWeek.Weekends:
                    {
                        sortKey = 400000;
                        break;
                    }
                //Finally, single-day tasks
                default:
                    {
                        sortKey = (int)localDaysOfWeek * 1000000;
                        break;
                    }
            }

            //Add the execute time of day as a secondary sort item
            //Multiply it by 100 to allow room for disabled tasks
            sortKey += (int)localExecuteTime.TimeOfDay.TotalMinutes * 100;

            return  sortKey;
        }

        public WlbScheduledTasks(Dictionary<string,string> Configuration)
        {
            foreach (string key in Configuration.Keys)
            {
                if (key.StartsWith("schedTask_"))
                {
                    string[] keyElements = key.Split('_');
                    string taskId = keyElements[1];
                    if (!_tasks.ContainsKey(taskId))
                    {
                        _tasks.Add(taskId, new WlbScheduledTask(taskId));
                        _tasks[taskId].AddParameter(key, Configuration[key]);
                    }
                    else
                    {
                        _tasks[taskId].AddParameter(key, Configuration[key]);
                    }
                }
            }
        }

        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> collectionDictionary = null;

            foreach (WlbScheduledTask scheduleTask in _tasks.Values)
            {
                Dictionary<string, string> taskDictionary = scheduleTask.ToDictionary();
                foreach (string key in taskDictionary.Keys)
                {
                    if (null == collectionDictionary)
                    {
                        collectionDictionary = new Dictionary<string, string>();
                    }
                    collectionDictionary.Add(key, taskDictionary[key]);
                }
                foreach (string key in scheduleTask.TaskParameters.Keys)
                {
                    string complexKey = string.Format("{0}_{1}_{2}", scheduleTask.KeyBase, scheduleTask.TaskId.ToString(), key);
                    if (collectionDictionary.ContainsKey(complexKey))
                    {
                        collectionDictionary[complexKey] = scheduleTask.TaskParameters[key];
                    }
                    else
                    {
                        collectionDictionary.Add(complexKey, scheduleTask.TaskParameters[key]);
                    }
                }
            }
            return collectionDictionary;
        }

        public WlbScheduledTask GetNextExecutingTask()
        {
            WlbScheduledTask firstTask = null;

            int currentTimeSortKey = GetSortKey(WlbScheduledTask.ConvertToWlbTaskDayOfWeek(DateTime.Now.DayOfWeek), DateTime.Now);

            foreach (int key in this.VirtualTaskList.Keys)
            {
                //only consider Enabled tasks
                if (this.VirtualTaskList[key].Enabled)
                {
                    if (null == firstTask)
                    {
                        firstTask = this.VirtualTaskList[key];
                    }

                    if (key > currentTimeSortKey)
                    {
                        return this.VirtualTaskList[key];
                    }
                }
            }
            //we are still here, so we have not found the next task.  this means that we 
            // need to account for week wrapping.  This shoudl be the first task of the Virtual
            // Task List
            return firstTask;
        }

        public WlbScheduledTask GetLastExecutingTask()
        {
            int[] taskKeys = new int[this.VirtualTaskList.Keys.Count];
            this.VirtualTaskList.Keys.CopyTo(taskKeys,0);
            WlbScheduledTask lastTask = this.VirtualTaskList[taskKeys[taskKeys.Length-1]];
            
            int currentTimeSortKey = GetSortKey(WlbScheduledTask.ConvertToWlbTaskDayOfWeek(DateTime.Now.DayOfWeek), DateTime.Now);

            for (int i = taskKeys.Length-1; i>=0; i--)
            {
                //Only consider Enabled tasks
                if (this.VirtualTaskList[taskKeys[i]].Enabled)
                {
                    if (taskKeys[i] < currentTimeSortKey)
                    {
                        return this.VirtualTaskList[taskKeys[i]];
                    }
                }
            }
            //we are still here, so we have not found the previous task.  this means that we 
            // need to account for week wrapping.  This should be the last task of the Virtual
            // Task List
            return lastTask;
        }

        public WlbPoolPerformanceMode GetCurrentScheduledPerformanceMode()
        {
            WlbScheduledTask lastTask = GetLastExecutingTask();

            return (WlbPoolPerformanceMode)int.Parse(lastTask.TaskParameters["OptMode"]);
        }
    }
}

    