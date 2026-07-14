/*
 * Copyright (c) Cloud Software Group, Inc.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 *   1) Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *
 *   2) Redistributions in binary form must reproduce the above
 *      copyright notice, this list of conditions and the following
 *      disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;


namespace XenAPI
{
    /// <summary>
    /// A long-running asynchronous task
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class Task : XenObject<Task>
    {
        #region Constructors

        public Task()
        {
        }

        public Task(string uuid,
            string name_label,
            string name_description,
            List<task_allowed_operations> allowed_operations,
            Dictionary<string, task_allowed_operations> current_operations,
            DateTime created,
            DateTime finished,
            task_status_type status,
            XenRef<Host> resident_on,
            double progress,
            string type,
            string result,
            string[] error_info,
            Dictionary<string, string> other_config,
            XenRef<Task> subtask_of,
            List<XenRef<Task>> subtasks,
            string backtrace)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.created = created;
            this.finished = finished;
            this.status = status;
            this.resident_on = resident_on;
            this.progress = progress;
            this.type = type;
            this.result = result;
            this.error_info = error_info;
            this.other_config = other_config;
            this.subtask_of = subtask_of;
            this.subtasks = subtasks;
            this.backtrace = backtrace;
        }

        /// <summary>
        /// Creates a new Task from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Task(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Task.
        /// </summary>
        public override void UpdateFrom(Task record)
        {
            uuid = record.uuid;
            name_label = record.name_label;
            name_description = record.name_description;
            allowed_operations = record.allowed_operations;
            current_operations = record.current_operations;
            created = record.created;
            finished = record.finished;
            status = record.status;
            resident_on = record.resident_on;
            progress = record.progress;
            type = record.type;
            result = record.result;
            error_info = record.error_info;
            other_config = record.other_config;
            subtask_of = record.subtask_of;
            subtasks = record.subtasks;
            backtrace = record.backtrace;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Task
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("name_label"))
                name_label = Marshalling.ParseString(table, "name_label");
            if (table.ContainsKey("name_description"))
                name_description = Marshalling.ParseString(table, "name_description");
            if (table.ContainsKey("allowed_operations"))
                allowed_operations = Helper.StringArrayToEnumList<task_allowed_operations>(Marshalling.ParseStringArray(table, "allowed_operations"));
            if (table.ContainsKey("current_operations"))
                current_operations = Maps.ToDictionary_string_task_allowed_operations(Marshalling.ParseHashTable(table, "current_operations"));
            if (table.ContainsKey("created"))
                created = Marshalling.ParseDateTime(table, "created");
            if (table.ContainsKey("finished"))
                finished = Marshalling.ParseDateTime(table, "finished");
            if (table.ContainsKey("status"))
                status = (task_status_type)Helper.EnumParseDefault(typeof(task_status_type), Marshalling.ParseString(table, "status"));
            if (table.ContainsKey("resident_on"))
                resident_on = Marshalling.ParseRef<Host>(table, "resident_on");
            if (table.ContainsKey("progress"))
                progress = Marshalling.ParseDouble(table, "progress");
            if (table.ContainsKey("type"))
                type = Marshalling.ParseString(table, "type");
            if (table.ContainsKey("result"))
                result = Marshalling.ParseString(table, "result");
            if (table.ContainsKey("error_info"))
                error_info = Marshalling.ParseStringArray(table, "error_info");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("subtask_of"))
                subtask_of = Marshalling.ParseRef<Task>(table, "subtask_of");
            if (table.ContainsKey("subtasks"))
                subtasks = Marshalling.ParseSetRef<Task>(table, "subtasks");
            if (table.ContainsKey("backtrace"))
                backtrace = Marshalling.ParseString(table, "backtrace");
        }

        public bool DeepEquals(Task other, bool ignoreCurrentOperations)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (!ignoreCurrentOperations && !Helper.AreEqual2(current_operations, other.current_operations))
                return false;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name_label, other._name_label) &&
                Helper.AreEqual2(_name_description, other._name_description) &&
                Helper.AreEqual2(_allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(_created, other._created) &&
                Helper.AreEqual2(_finished, other._finished) &&
                Helper.AreEqual2(_status, other._status) &&
                Helper.AreEqual2(_resident_on, other._resident_on) &&
                Helper.AreEqual2(_progress, other._progress) &&
                Helper.AreEqual2(_type, other._type) &&
                Helper.AreEqual2(_result, other._result) &&
                Helper.AreEqual2(_error_info, other._error_info) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_subtask_of, other._subtask_of) &&
                Helper.AreEqual2(_subtasks, other._subtasks) &&
                Helper.AreEqual2(_backtrace, other._backtrace);
        }


        /// <summary>
        /// Get a record containing the current state of the given task.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Task get_record(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_record(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get a reference to the task instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.task_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get all the task instances with the given label.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Task>> get_by_name_label(Session session, string _label)
        {
            return session.JsonRpcClient.task_get_by_name_label(session.opaque_ref, _label);
        }

        /// <summary>
        /// Get the uuid field of the given task.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_uuid(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get the name/label field of the given task.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_label(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_name_label(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get the name/description field of the given task.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_description(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_name_description(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get the allowed_operations field of the given task.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<task_allowed_operations> get_allowed_operations(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_allowed_operations(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get the current_operations field of the given task.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, task_allowed_operations> get_current_operations(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_current_operations(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get the created field of the given task.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_created(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_created(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get the finished field of the given task.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DateTime get_finished(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_finished(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get the status field of the given task.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static task_status_type get_status(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_status(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get the resident_on field of the given task.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host> get_resident_on(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_resident_on(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get the progress field of the given task.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static double get_progress(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_progress(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get the type field of the given task.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_type(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_type(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get the result field of the given task.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_result(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_result(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get the error_info field of the given task.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_error_info(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_error_info(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get the other_config field of the given task.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_other_config(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get the subtask_of field of the given task.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> get_subtask_of(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_subtask_of(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get the subtasks field of the given task.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Task>> get_subtasks(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_subtasks(session.opaque_ref, _task);
        }

        /// <summary>
        /// Get the backtrace field of the given task.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_backtrace(Session session, string _task)
        {
            return session.JsonRpcClient.task_get_backtrace(session.opaque_ref, _task);
        }

        /// <summary>
        /// Create a new task object which must be manually destroyed.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">short label for the new task</param>
        /// <param name="_description">longer description for the new task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> create(Session session, string _label, string _description)
        {
            return session.JsonRpcClient.task_create(session.opaque_ref, _label, _description);
        }

        /// <summary>
        /// Destroy the task object
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static void destroy(Session session, string _task)
        {
            session.JsonRpcClient.task_destroy(session.opaque_ref, _task);
        }

        /// <summary>
        /// Request that a task be cancelled. Note that a task may fail to be cancelled and may complete or fail normally and note that, even when a task does cancel, it might take an arbitrary amount of time.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static void cancel(Session session, string _task)
        {
            session.JsonRpcClient.task_cancel(session.opaque_ref, _task);
        }

        /// <summary>
        /// Request that a task be cancelled. Note that a task may fail to be cancelled and may complete or fail normally and note that, even when a task does cancel, it might take an arbitrary amount of time.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_cancel(Session session, string _task)
        {
          return session.JsonRpcClient.async_task_cancel(session.opaque_ref, _task);
        }

        /// <summary>
        /// Set the task status
        /// First published in XenServer 7.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <param name="_value">task status value to be set</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static void set_status(Session session, string _task, task_status_type _value)
        {
            session.JsonRpcClient.task_set_status(session.opaque_ref, _task, _value);
        }

        /// <summary>
        /// Set the task progress
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <param name="_value">Task progress value to be set</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static void set_progress(Session session, string _task, double _value)
        {
            session.JsonRpcClient.task_set_progress(session.opaque_ref, _task, _value);
        }

        /// <summary>
        /// Set the task result
        /// First published in 21.3.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <param name="_value">Task result to be set</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static void set_result(Session session, string _task, string _value)
        {
            session.JsonRpcClient.task_set_result(session.opaque_ref, _task, _value);
        }

        /// <summary>
        /// Set the task error info
        /// First published in 21.3.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <param name="_value">Task error info to be set</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static void set_error_info(Session session, string _task, string[] _value)
        {
            session.JsonRpcClient.task_set_error_info(session.opaque_ref, _task, _value);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given task.
        /// Experimental. First published in 25.8.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static void add_to_other_config(Session session, string _task, string _key, string _value)
        {
            session.JsonRpcClient.task_add_to_other_config(session.opaque_ref, _task, _key, _value);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given task.
        /// Experimental. First published in 25.8.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_add_to_other_config(Session session, string _task, string _key, string _value)
        {
          return session.JsonRpcClient.async_task_add_to_other_config(session.opaque_ref, _task, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given task. If the key is not in that Map, then do nothing.
        /// Experimental. First published in 25.8.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <param name="_key">Key of entry to remove</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static void remove_from_other_config(Session session, string _task, string _key)
        {
            session.JsonRpcClient.task_remove_from_other_config(session.opaque_ref, _task, _key);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given task. If the key is not in that Map, then do nothing.
        /// Experimental. First published in 25.8.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <param name="_key">Key of entry to remove</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_remove_from_other_config(Session session, string _task, string _key)
        {
          return session.JsonRpcClient.async_task_remove_from_other_config(session.opaque_ref, _task, _key);
        }

        /// <summary>
        /// Set the other_config field of the given task.
        /// Experimental. First published in 25.8.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <param name="_value">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static void set_other_config(Session session, string _task, Dictionary<string, string> _value)
        {
            session.JsonRpcClient.task_set_other_config(session.opaque_ref, _task, _value);
        }

        /// <summary>
        /// Set the other_config field of the given task.
        /// Experimental. First published in 25.8.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_task">The opaque_ref of the given task</param>
        /// <param name="_value">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Task> async_set_other_config(Session session, string _task, Dictionary<string, string> _value)
        {
          return session.JsonRpcClient.async_task_set_other_config(session.opaque_ref, _task, _value);
        }

        /// <summary>
        /// Return a list of all the tasks known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Task>> get_all(Session session)
        {
            return session.JsonRpcClient.task_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of task references to task records for all tasks known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Task>, Task> get_all_records(Session session)
        {
            return session.JsonRpcClient.task_get_all_records(session.opaque_ref);
        }

        /// <summary>
        /// Unique identifier/object reference
        /// </summary>
        public virtual string uuid
        {
            get { return _uuid; }
            set
            {
                if (!Helper.AreEqual(value, _uuid))
                {
                    _uuid = value;
                    NotifyPropertyChanged("uuid");
                }
            }
        }
        private string _uuid = "";

        /// <summary>
        /// a human-readable name
        /// </summary>
        public virtual string name_label
        {
            get { return _name_label; }
            set
            {
                if (!Helper.AreEqual(value, _name_label))
                {
                    _name_label = value;
                    NotifyPropertyChanged("name_label");
                }
            }
        }
        private string _name_label = "";

        /// <summary>
        /// a notes field containing human-readable description
        /// </summary>
        public virtual string name_description
        {
            get { return _name_description; }
            set
            {
                if (!Helper.AreEqual(value, _name_description))
                {
                    _name_description = value;
                    NotifyPropertyChanged("name_description");
                }
            }
        }
        private string _name_description = "";

        /// <summary>
        /// list of the operations allowed in this state. This list is advisory only and the server state may have changed by the time this field is read by a client.
        /// </summary>
        public virtual List<task_allowed_operations> allowed_operations
        {
            get { return _allowed_operations; }
            set
            {
                if (!Helper.AreEqual(value, _allowed_operations))
                {
                    _allowed_operations = value;
                    NotifyPropertyChanged("allowed_operations");
                }
            }
        }
        private List<task_allowed_operations> _allowed_operations = new List<task_allowed_operations>() {};

        /// <summary>
        /// links each of the running tasks using this object (by reference) to a current_operation enum which describes the nature of the task.
        /// </summary>
        public virtual Dictionary<string, task_allowed_operations> current_operations
        {
            get { return _current_operations; }
            set
            {
                if (!Helper.AreEqual(value, _current_operations))
                {
                    _current_operations = value;
                    NotifyPropertyChanged("current_operations");
                }
            }
        }
        private Dictionary<string, task_allowed_operations> _current_operations = new Dictionary<string, task_allowed_operations>() {};

        /// <summary>
        /// Time task was created
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime created
        {
            get { return _created; }
            set
            {
                if (!Helper.AreEqual(value, _created))
                {
                    _created = value;
                    NotifyPropertyChanged("created");
                }
            }
        }
        private DateTime _created;

        /// <summary>
        /// Time task finished (i.e. succeeded or failed). If task-status is pending, then the value of this field has no meaning
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime finished
        {
            get { return _finished; }
            set
            {
                if (!Helper.AreEqual(value, _finished))
                {
                    _finished = value;
                    NotifyPropertyChanged("finished");
                }
            }
        }
        private DateTime _finished;

        /// <summary>
        /// current status of the task
        /// </summary>
        [JsonConverter(typeof(task_status_typeConverter))]
        public virtual task_status_type status
        {
            get { return _status; }
            set
            {
                if (!Helper.AreEqual(value, _status))
                {
                    _status = value;
                    NotifyPropertyChanged("status");
                }
            }
        }
        private task_status_type _status;

        /// <summary>
        /// the host on which the task is running
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Host>))]
        public virtual XenRef<Host> resident_on
        {
            get { return _resident_on; }
            set
            {
                if (!Helper.AreEqual(value, _resident_on))
                {
                    _resident_on = value;
                    NotifyPropertyChanged("resident_on");
                }
            }
        }
        private XenRef<Host> _resident_on = new XenRef<Host>(Helper.NullOpaqueRef);

        /// <summary>
        /// This field contains the estimated fraction of the task which is complete. This field should not be used to determine whether the task is complete - for this the status field of the task should be used.
        /// </summary>
        public virtual double progress
        {
            get { return _progress; }
            set
            {
                if (!Helper.AreEqual(value, _progress))
                {
                    _progress = value;
                    NotifyPropertyChanged("progress");
                }
            }
        }
        private double _progress;

        /// <summary>
        /// if the task has completed successfully, this field contains the type of the encoded result (i.e. name of the class whose reference is in the result field). Undefined otherwise.
        /// </summary>
        public virtual string type
        {
            get { return _type; }
            set
            {
                if (!Helper.AreEqual(value, _type))
                {
                    _type = value;
                    NotifyPropertyChanged("type");
                }
            }
        }
        private string _type = "";

        /// <summary>
        /// if the task has completed successfully, this field contains the result value (either Void or an object reference). Undefined otherwise.
        /// </summary>
        public virtual string result
        {
            get { return _result; }
            set
            {
                if (!Helper.AreEqual(value, _result))
                {
                    _result = value;
                    NotifyPropertyChanged("result");
                }
            }
        }
        private string _result = "";

        /// <summary>
        /// if the task has failed, this field contains the set of associated error strings. Undefined otherwise.
        /// </summary>
        public virtual string[] error_info
        {
            get { return _error_info; }
            set
            {
                if (!Helper.AreEqual(value, _error_info))
                {
                    _error_info = value;
                    NotifyPropertyChanged("error_info");
                }
            }
        }
        private string[] _error_info = {};

        /// <summary>
        /// additional configuration
        /// First published in XenServer 4.1.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> other_config
        {
            get { return _other_config; }
            set
            {
                if (!Helper.AreEqual(value, _other_config))
                {
                    _other_config = value;
                    NotifyPropertyChanged("other_config");
                }
            }
        }
        private Dictionary<string, string> _other_config = new Dictionary<string, string>() {};

        /// <summary>
        /// Ref pointing to the task this is a substask of.
        /// First published in XenServer 5.0.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Task>))]
        public virtual XenRef<Task> subtask_of
        {
            get { return _subtask_of; }
            set
            {
                if (!Helper.AreEqual(value, _subtask_of))
                {
                    _subtask_of = value;
                    NotifyPropertyChanged("subtask_of");
                }
            }
        }
        private XenRef<Task> _subtask_of = new XenRef<Task>(Helper.NullOpaqueRef);

        /// <summary>
        /// List pointing to all the substasks.
        /// First published in XenServer 5.0.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Task>))]
        public virtual List<XenRef<Task>> subtasks
        {
            get { return _subtasks; }
            set
            {
                if (!Helper.AreEqual(value, _subtasks))
                {
                    _subtasks = value;
                    NotifyPropertyChanged("subtasks");
                }
            }
        }
        private List<XenRef<Task>> _subtasks = new List<XenRef<Task>>() {};

        /// <summary>
        /// Function call trace for debugging.
        /// First published in XenServer 7.0.
        /// </summary>
        public virtual string backtrace
        {
            get { return _backtrace; }
            set
            {
                if (!Helper.AreEqual(value, _backtrace))
                {
                    _backtrace = value;
                    NotifyPropertyChanged("backtrace");
                }
            }
        }
        private string _backtrace = "()";
    }
}
